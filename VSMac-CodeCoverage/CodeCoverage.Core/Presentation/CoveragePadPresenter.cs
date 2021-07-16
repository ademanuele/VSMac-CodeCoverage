using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace CodeCoverage.Core.Presentation
{
  public interface ICoveragePad
  {
    void SetTestProjects(IEnumerable<string> testProjects);
    void DisableUI();
    void EnableUI();
    void SetStatusMessage(string message, LogLevel level);
    void ClearCoverageResults();
    void SetCoverageResults(IReadOnlyDictionary<string, CoverageSummary> results);
  }

  public class CoveragePadPresenter : IDisposable
  {
    readonly ICoveragePad pad;
    readonly ILoggingService log;
    readonly ICoverageResultsRepository repository;
    readonly TestProjectService testProjectService;
    readonly LoggedCoverageService coverageService;

    public Project SelectedTestProject
    {
      get {
        if (testProjects == null || selectedTestProjectIndex >= testProjects.Count) return null;
        return testProjects[selectedTestProjectIndex];
      }
    }

    int selectedTestProjectIndex;
    List<Project> testProjects;

    internal CoveragePadPresenter(ICoveragePad pad, ILoggingService log, ICoverageResultsRepository repository, ICoverageProvider provider)
    {
      this.pad = pad;
      this.log = log;
      this.repository = repository;
      testProjectService = new TestProjectService();
      testProjectService.TestProjectsChanged += RefreshTestProjects;
      coverageService = new LoggedCoverageService(provider, repository, log);
    }

    public void OnShown()
    {
      RefreshTestProjects();
    }

    public void Dispose()
    {
      testProjectService.TestProjectsChanged -= RefreshTestProjects;
      testProjectService.Dispose();
      coverageService.Dispose();
    }

    void RefreshTestProjects()
    {
      testProjects = testProjectService.TestProjects.ToList();      
      pad.ClearCoverageResults();
      pad.SetTestProjects(testProjects.Select(p => p.Name));
      TestProjectSelectionChanged(0);
    }

    public void TestProjectSelectionChanged(int projectIndex)
    {
      selectedTestProjectIndex = projectIndex;
      var project = SelectedTestProject;
      var configuration = IdeApp.Workspace.ActiveConfiguration;

      if (project == null)
      {
        pad.ClearCoverageResults();
        return;
      }

      try
      {
        var results = repository.ResultsFor(project, configuration);
        if (results == null)
        {
          pad.ClearCoverageResults();
          return;
        }
        pad.SetCoverageResults(results.ModuleCoverage);
      }
      catch (Exception e)
      {
        pad.ClearCoverageResults();
        log.Error($"Failed to load results for project {project.Name}.");
        log.Error(e.Message);
      }
    }

    public async Task GatherCoverageAsync()
    {
      Project testProject = SelectedTestProject;
      if (testProject == null) return;

      pad.DisableUI();
      pad.ClearCoverageResults();

      Progress<Log> progress = new Progress<Log>(HandleCoverageServiceUpdate);
      await coverageService.CollectCoverageForTestProject(testProject, progress);
      TestProjectSelectionChanged(selectedTestProjectIndex);
      pad.EnableUI();
    }

    void HandleCoverageServiceUpdate(Log log)
    {
      pad.SetStatusMessage(log.Message, log.Level);
      if (log.Exception == null) return;
      this.log.Error(log.Exception.ToString());
    }
  }
}
