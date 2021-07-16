using System;
using Foundation;
using AppKit;
using System.Reflection;
using System.IO;
using CodeCoverage.Core.Presentation;
using CodeCoverage.Core;
using System.Collections.Generic;
using System.Linq;

namespace CodeCoverage.Pad.Native
{
  public partial class PadView : NSView, ICoveragePad
  {
    #region Constructors
    private const string padViewNibResourceId = "__xammac_content_PadView.nib";

    // Called when created from unmanaged code
    public PadView(IntPtr handle) : base(handle) { }

    // Called when created directly from a XIB file
    [Export("initWithCoder:")]
    public PadView(NSCoder coder) : base(coder) { }

    public PadView RootView { get; }    

    private CoveragePadPresenter presenter;

    public PadView()
    {
      Assembly assembly = typeof(PadView).Assembly;
      Stream nibStream = assembly.GetManifestResourceStream(padViewNibResourceId);
      NSData nibData = NSData.FromStream(nibStream);
      NSNib nib = new NSNib(nibData, NSBundle.MainBundle);

      if (nib.InstantiateNibWithOwner(this, out NSArray nibObjects))
      {
        RootView = GetPadViewFrom(nibObjects);
        RootView.presenter = CoverageExtension.Presenter(RootView);
      }
    }

    public override void ViewWillMoveToSuperview(NSView newSuperview)
    {
      base.ViewWillMoveToSuperview(newSuperview);
      if (newSuperview == null) return;
      presenter.OnShown();
    }

    // Need to do this as the PadView is not always the first element in the NSArray.
    private static PadView GetPadViewFrom(NSArray array)
    {
      for (uint i = 0; i < array.Count; i++)
        try
        {
          return array.GetItem<PadView>(i);
        } catch { }

      return null;
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing) return;
      presenter.Dispose();
    }
    #endregion

    public event Action OpeningPreferences;
    public event Action SelectedTestProjectChanged;
    public event Action CoverageResultsUpdated;
    public event Action CoverageResultsCleared;

    private IReadOnlyDictionary<string, CoverageSummary> currentResults;
    private int presentedResultIndex;

    #region Test Project Dropdown
    public void SetTestProjects(IEnumerable<string> testProjects)
    {      
      TestProjectDropdown.RemoveAllItems();
      TestProjectDropdown.AddItems(testProjects.ToArray());
    }

    partial void TestProjectDropdownChanged(NSPopUpButton sender)
    {
      presenter.TestProjectSelectionChanged((int)TestProjectDropdown.IndexOfSelectedItem);
      SelectedTestProjectChanged?.Invoke();
    }
    #endregion

    public void SetCoverageResults(IReadOnlyDictionary<string, CoverageSummary> results)
    {
      currentResults = results;
      PresentCoverageAtIndex(0);
      CoverageResultsUpdated?.Invoke();
    }

    partial void NextTestedProjectTapped(NSButton sender)
    {
      if (currentResults == null) return;
      PresentCoverageAtIndex((presentedResultIndex + 1) % currentResults.Count);
    }

    partial void PreviousTestedProjectTapped(NSButton sender)
    {
      if (currentResults == null) return;
      PresentCoverageAtIndex(Math.Abs(presentedResultIndex - 1) % currentResults.Count);
    }

    void PresentCoverageAtIndex(int index)
    {
      if (currentResults == null || index >= currentResults.Count)
      {
        ClearCoverageResults();
        return;
      }

      var coverage = currentResults.ElementAt(index);
      TestedProjectLabel.StringValue = coverage.Key;      
      LineCoverageLabel.StringValue = $"{Math.Round(coverage.Value.Line, 2)}%";
      BranchCoverageLabel.StringValue = $"{Math.Round(coverage.Value.Branch, 2)}%";
      presentedResultIndex = index;
      EnableCoverageResultsUI();
    }

    public void ClearCoverageResults()
    {
      currentResults = null;
      TestedProjectLabel.StringValue = string.Empty;
      LineCoverageLabel.StringValue = "--";
      BranchCoverageLabel.StringValue = "--";
      DisableCoverageResultsUI();
      CoverageResultsCleared?.Invoke();
    }

    void DisableCoverageResultsUI()
    {
      NextTestedProjectButton.Enabled = false;
      PreviousTestProjectButton.Enabled = false;      
    }

    void EnableCoverageResultsUI()
    {
      NextTestedProjectButton.Enabled = true;
      PreviousTestProjectButton.Enabled = true;
    }

    public void DisableUI()
    {
      TestProjectDropdown.Enabled = false;
      GatherCoverageButton.Enabled = false;
    }

    public void EnableUI()
    {
      TestProjectDropdown.Enabled = true;
      GatherCoverageButton.Enabled = true;
    }

    async partial void GatherCoverageTapped(NSButton sender)
    {
      await presenter.GatherCoverageAsync();
    }

    #region Status Label
    static readonly Dictionary<LogLevel, NSColor> statusMessageColorMap = new Dictionary<LogLevel, NSColor>()
    {
      { LogLevel.Info, NSColor.FromRgb(255, 255, 255) },
      { LogLevel.Warn, NSColor.FromRgb(254, 217, 70) },
      { LogLevel.Error, NSColor.FromRgb(1, 0, 0) },
    };

    public void SetStatusMessage(string message, LogLevel level)
    {
      StatusLabel.StringValue = message;
      StatusLabel.TextColor = statusMessageColorMap[level];
    }
    #endregion

    partial void PreferencesTapped(NSButton sender)
    {
      OpeningPreferences?.Invoke();
    }
  }
}
