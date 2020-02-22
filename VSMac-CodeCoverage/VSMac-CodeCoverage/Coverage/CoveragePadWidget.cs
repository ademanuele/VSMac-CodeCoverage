using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using MonoDevelop.Projects;
using Xwt.Drawing;
using Xwt.GtkBackend;

namespace CodeCoverage.Coverage
{
  [System.ComponentModel.ToolboxItem(true)]
  public partial class CoveragePadWidget : Bin, ICoveragePad
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

    readonly CoveragePadPresenter presenter;
    readonly ILoggingService log;

    CoverageConsoleWindow consoleWindow;

    IReadOnlyDictionary<string, CoverageSummary> coverageResults;
    int presentedResultIndex;

    public CoveragePadWidget()
    {
      Build();
      log = new LoggingService();
      presenter = new CoveragePadPresenter(this, log);
      SetupCoverageLabels();
      testProjectDropdown.Changed += OnTestProjectSelectionChanged;

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
      consoleWindow.Dispose();
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

    public void SetStatusMessage(string message, LogLevel style)
    {
      statusLabel.Text = message;
      statusLabel.SetForegroundColor(ColorForStyle());

      Color ColorForStyle()
      {
        Color color;

        switch (style)
        {
          case LogLevel.Error:
            Color.TryParse("#FF0000", out color);
            break;
          default:
            Color.TryParse("#FFFFFF", out color);
            break;
        }

        return color;
      }
    }

    public void SetCoverageResults(IReadOnlyDictionary<string, CoverageSummary> results)
    {
      coverageResults = results;
      PresentCoverageAtIndex(0);
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

    protected void OnShowConsoleClicked(object sender, EventArgs e)
    {
      if (consoleWindow == null)
      {
        consoleWindow = new CoverageConsoleWindow(log);
        consoleWindow.Destroyed += HandleConsoleWindowDestroyed;
      }

      consoleWindow.ShowAll();
    }

    void HandleConsoleWindowDestroyed(object sender, EventArgs e)
    {
      consoleWindow.Destroyed -= HandleConsoleWindowDestroyed;
      consoleWindow = null;
    }
  }

  class TestProjectDropdownStore : ListStore
  {
    public IReadOnlyDictionary<Project, TreeIter> ProjectToIter => projectToIter;
    public IReadOnlyDictionary<TreeIter, Project> IterToProject => iterToProject;

    Dictionary<Project, TreeIter> projectToIter;
    Dictionary<TreeIter, Project> iterToProject;

    public TestProjectDropdownStore(IEnumerable<Project> projects) : base(typeof(string))
    {
      projectToIter = new Dictionary<Project, TreeIter>();
      iterToProject = new Dictionary<TreeIter, Project>();

      foreach (Project project in projects)
      {
        var newIter = AppendValues(project.Name);
        projectToIter.Add(project, newIter);
        iterToProject.Add(newIter, project);
      }
    }
  }
}
