using System;
using System.Collections.Generic;
using System.Linq;
using CodeCoverage.Core;
using CodeCoverage.Core.Presentation;
using CodeCoverage.Coverage;
using Gtk;
using MonoDevelop.Projects;

namespace CodeCoverage
{
  [System.ComponentModel.ToolboxItem(true)]
  public partial class CoverageWidget : Bin
  {
    public Project SelectedTestProject
    {
      get
      {
        return !testProjectDropdown.GetActiveIter(out TreeIter selected)
          ? null
          : (testProjectDropdown.Model as TestProjectDropdownStore).IterToProject[selected];
      }
      set
      {
        var iter = (testProjectDropdown.Model as TestProjectDropdownStore).ProjectToIter[value];
        testProjectDropdown.SetActiveIter(iter);
        SelectedTestProjectChanged?.Invoke(this, value);
      }
    }

    public event EventHandler<Project> SelectedTestProjectChanged;
    public event EventHandler CoverageResultsUpdated;
    public event EventHandler CoverageResultsCleared;

    readonly CoveragePadPresenter presenter;
    readonly IStatusBar statusBar;

    IReadOnlyDictionary<string, CoverageSummary> coverageResults;
    int presentedResultIndex;

    public CoverageWidget(ILoggingService loggingService, IStatusBar statusBar)
    {      
      this.statusBar = statusBar;

      Build();
      SetupCoverageLabels();
      //presenter = new CoveragePadPresenter(this, loggingService, null, null);

      void SetupCoverageLabels()
      {
        var font = Pango.FontDescription.FromString("Courier 30");
        lineCoverageLabel.ModifyFont(font);
        branchCoverageLabel.ModifyFont(font);
      }
    }

    protected override void OnShown()
    {
      base.OnShown();
      presenter.OnShown();
    }

    public override void Dispose()
    {
      base.Dispose();
      presenter.Dispose();
    }

    void PresentCoverageAtIndex(int index)
    {
      if (coverageResults == null || coverageResults.Count - 1 < index)
      {
        ClearCoverageResults();
        return;
      }

      var coverage = coverageResults.ElementAt(index);
      coveredProjectLabel.Text = coverage.Key;
      lineCoverageLabel.Text = $"{Math.Round(coverage.Value.Line, 2)}%";
      branchCoverageLabel.Text = $"{Math.Round(coverage.Value.Branch, 2)}%";
      presentedResultIndex = index;
      EnableCoverageResultsUI();
    }

    protected void OnNextCoverageResultClicked(object sender, EventArgs e)
    {
      if (coverageResults == null) return;
      PresentCoverageAtIndex((presentedResultIndex + 1) % coverageResults.Count);
    }

    protected void OnPreviousCoverageResultClicked(object sender, EventArgs e)
    {
      if (coverageResults == null) return;
      PresentCoverageAtIndex(Math.Abs(presentedResultIndex - 1) % coverageResults.Count);
    }

    public void ClearCoverageResults()
    {
      coverageResults = null;
      coveredProjectLabel.Text = string.Empty;
      lineCoverageLabel.Text = "--";
      branchCoverageLabel.Text = "--";
      DisableCoverageResultsUI();
      CoverageResultsCleared?.Invoke(this, new EventArgs());
    }

    void DisableCoverageResultsUI()
    {
      coveredProjectNextButton.Sensitive = false;
      coveredProjectPreviouButton.Sensitive = false;
    }

    void EnableCoverageResultsUI()
    {
      coveredProjectNextButton.Sensitive = true;
      coveredProjectPreviouButton.Sensitive = true;
    }

    public void DisableUI()
    {
      testProjectDropdown.Sensitive = false;
      gatherCoverageButton.Sensitive = false;
    }

    public void EnableUI()
    {
      testProjectDropdown.Sensitive = true;
      gatherCoverageButton.Sensitive = true;
    }

    public void SetTestProjects(IEnumerable<Project> testProjects)
      => testProjectDropdown.Model = new TestProjectDropdownStore(testProjects);

    void OnTestProjectSelectionChanged(object sender, EventArgs e)
    {
      presenter.TestProjectSelectionChanged();
      SelectedTestProjectChanged?.Invoke(this, SelectedTestProject);
    }

    async void OnGatherCoverageClicked(object sender, EventArgs e)
      => await presenter.GatherCoverageAsync();

    public void SetCoverageResults(IReadOnlyDictionary<string, CoverageSummary> results)
    {
      coverageResults = results;
      PresentCoverageAtIndex(0);
      CoverageResultsUpdated?.Invoke(this, new EventArgs());
    }

    public void SetStatusMessage(string message, LogLevel level)
      => statusBar.SetStatusMessage(message, level);
  }

  public interface IStatusBar
  {
    void SetStatusMessage(string message, LogLevel style);
  }
}
