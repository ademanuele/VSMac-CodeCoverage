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
    void PresentTestCommandDialog();
    void ClearCoverageResults();
    void SetCoverageResults(IReadOnlyDictionary<string, Coverage> results);
  }

  public class CoveragePadPresenter : IDisposable
  {
    readonly ICoveragePad pad;

    public CoveragePadPresenter(ICoveragePad pad)
    {
      this.pad = pad;
    }

    public void OnShown()
    {
      TestProjectService.Instance.TestProjectsChanged += RefreshTestProjects;
      RefreshTestProjects();
    }

    public void Dispose()
    {
      TestProjectService.Instance.TestProjectsChanged -= RefreshTestProjects;
      TestProjectService.Instance.Dispose();
    }

    void RefreshTestProjects()
    {
      var testProjects = TestProjectService.Instance.TestProjects;
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
      var results = new CoverageResults(pad.SelectedTestProject, IdeApp.Workspace.ActiveConfiguration);
      if (!results.Valid)
      {
        pad.ClearCoverageResults();
        return;
      }
      pad.SetCoverageResults(results.AllModulesCoverage());
    }

    public async Task GatherCoverageAsync()
    {
      if (pad.SelectedTestProject == null) return;

      if (TestCommandSetting.Current() == null)
      {
        pad.PresentTestCommandDialog();
        return;
      }

      try
      {
        pad.DisableUI();
        var progress = new Progress<Log>(l => pad.SetStatusMessage(l.Message, l.Level));
        await GatherCoverageForProjectAsync(pad.SelectedTestProject, progress);
      }
      catch (Exception e)
      {
        LogException(e);
      }
      finally
      {
        pad.EnableUI();
      }
    }

    async Task GatherCoverageForProjectAsync(Project project, Progress<Log> progress)
    {
      await CoverageService.Instance.CollectCoverageForTestProject(project, progress);
      TestProjectSelectionChanged();
      CoverageTextEditorExtension.RefreshActiveDocument();
    }

    void LogException(Exception e)
    {
      LoggingService.Error(e.ToString());
      pad.SetStatusMessage(e.Message, LogLevel.Error);
    }
  }
}
