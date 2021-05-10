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
    TestProject SelectedTestProject { get; set; }
    void SetTestProjects(IEnumerable<TestProject> testProjects);
    void DisableUI();
    void EnableUI();
    void SetStatusMessage(string message, LogLevel level);
    void ClearCoverageResults();
    void SetCoverageResults(IReadOnlyDictionary<string, CoverageSummary> results);
  }

  public class TestProject
  {
    public string DisplayName => IdeProject.Name;
    internal Project IdeProject { get; }

    internal TestProject(Project project) {
      IdeProject = project;
    }
  }

  public class CoveragePadPresenter : IDisposable
  {
    readonly ICoveragePad pad;
    readonly ILoggingService log;
    readonly TestProjectService testProjectService;    
    readonly LoggedCoverageService coverageService;
    readonly ICoverageResultsRepository resultsRepository;

    public CoveragePadPresenter(ICoveragePad pad, ILoggingService log, ICoverageResultsRepository repository, ICoverageProvider provider)
    {
      this.pad = pad;
      this.log = log;
      testProjectService = new TestProjectService();
      testProjectService.TestProjectsChanged += RefreshTestProjects;
      coverageService = new LoggedCoverageService(provider, resultsRepository, log);
      resultsRepository = repository;
    }

    public void OnShown() => RefreshTestProjects();

    public void Dispose()
    {
      testProjectService.TestProjectsChanged -= RefreshTestProjects;
      testProjectService.Dispose();
      coverageService.Dispose();
    }

    void RefreshTestProjects()
    {
      var testProjects = testProjectService.TestProjects
        .Select(p => new TestProject(p)).AsEnumerable();
      pad.ClearCoverageResults();
      pad.SetTestProjects(testProjects);
      SelectFirstProject();

      void SelectFirstProject()
      {
        var first = testProjects?.FirstOrDefault();
        if (first == null) return;
        pad.SelectedTestProject = first;
      }
    }

    public void TestProjectSelectionChanged()
    {
      var project = pad.SelectedTestProject;
      var configuration = IdeApp.Workspace.ActiveConfiguration;
      try
      {
        var results = resultsRepository.ResultsFor(project.IdeProject, configuration);
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
        log.Error($"Failed to load results for project {project.IdeProject.Name}.");
        log.Error(e.Message);
      }
    }

    public async Task GatherCoverageAsync()
    {
      if (pad.SelectedTestProject == null) return;

      pad.DisableUI();
      pad.ClearCoverageResults();

      var progress = new Progress<Log>(HandleCoverageServiceUpdate);
      await coverageService.CollectCoverageForTestProject(pad.SelectedTestProject.IdeProject, progress);

      TestProjectSelectionChanged();
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
