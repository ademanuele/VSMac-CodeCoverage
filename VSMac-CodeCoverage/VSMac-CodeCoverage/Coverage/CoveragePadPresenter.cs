using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace CodeCoverage.Coverage
{
  public interface ICoveragePad
  {
    Project SelectedTestProject { get; set; }
    void SetTestProjects(IEnumerable<Project> testProjects);
    void DisableUI();
    void EnableUI();
    void SetStatusMessage(string message, LogLevel level);
    void ClearCoverageResults();
    void SetCoverageResults(IReadOnlyDictionary<string, CoverageSummary> results);
  }

  class CoveragePadPresenter : IDisposable
  {
    readonly ICoveragePad pad;
    readonly ILoggingService log;
    readonly TestProjectService testProjectService;
    readonly ICoverageResultsRepository resultsRepository;
    readonly LoggedCoverageService coverageService;

    public CoveragePadPresenter(ICoveragePad pad, ILoggingService log)
    {
      this.pad = pad;
      this.log = log;
      testProjectService = new TestProjectService();
      testProjectService.TestProjectsChanged += RefreshTestProjects;
      resultsRepository = CoverageResultsRepository.Instance;
      coverageService = new LoggedCoverageService(new CoverletCoverageProvider(log), resultsRepository);
    }

    public void OnShown() => RefreshTestProjects();

    public void Dispose()
    {
      testProjectService.TestProjectsChanged -= RefreshTestProjects;
      testProjectService.Dispose();
    }

    void RefreshTestProjects()
    {
      var testProjects = testProjectService.TestProjects;
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
        var results = resultsRepository.ResultsFor(project, configuration);
        if (results == null)
        {
          pad.ClearCoverageResults();
          return;
        }
        pad.SetCoverageResults(results.ModuleCoverage);
      }
      catch (Exception)
      {
        pad.ClearCoverageResults();
        log.Warn($"Failed to load results for project {project.Name}.");
      }
    }

    public async Task GatherCoverageAsync()
    {
      if (pad.SelectedTestProject == null) return;

      pad.DisableUI();
      pad.ClearCoverageResults();

      try
      {
        var progress = new Progress<Log>(l => pad.SetStatusMessage(l.Message, l.Level));
        await coverageService.CollectCoverageForTestProject(pad.SelectedTestProject, progress);
      }
      catch (Exception e)
      {
        LogException(e);
      }
      finally
      {
        TestProjectSelectionChanged();
        pad.EnableUI();
      }
    }

    void LogException(Exception e)
    {
      log.Error(e.ToString());
      pad.SetStatusMessage("Failed to gather coverage. See log for details.", LogLevel.Error);
    }
  }
}
