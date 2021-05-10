using System;
using Foundation;
using AppKit;
using System.Reflection;
using System.IO;
using CodeCoverage.Core.Presentation;
using CodeCoverage.Core;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Utilities.Internal;

namespace CodeCoverage.Pad.Native
{
  public partial class PadView : NSView, ICoveragePad
  {
    private const string padViewNibResourceId = "__xammac_content_PadView.nib";

    #region Constructors
    // Called when created from unmanaged code
    public PadView(IntPtr handle) : base(handle) { }

    // Called when created directly from a XIB file
    [Export("initWithCoder:")]
    public PadView(NSCoder coder) : base(coder) { }

    public PadView RootView { get; }
    private CoveragePadPresenter presenter;
    private IReadOnlyDictionary<string, CoverageSummary> currentResults;

    public PadView(ILoggingService loggingService, ICoverageResultsRepository repository, ICoverageProvider provider)
    {
      Assembly assembly = typeof(PadView).Assembly;
      Stream nibStream = assembly.GetManifestResourceStream(padViewNibResourceId);
      NSData nibData = NSData.FromStream(nibStream);
      NSNib nib = new NSNib(nibData, NSBundle.MainBundle);

      if (nib.InstantiateNibWithOwner(this, out NSArray nibObjects))
      {
        RootView = GetPadViewFrom(nibObjects);
        RootView.presenter = new CoveragePadPresenter(RootView, loggingService, repository, provider);        
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

    public override void ViewDidUnhide()
    {
      base.ViewDidUnhide();
    }
    #endregion

    public event Action OpeningPreferences;

    partial void PreferencesTapped(NSButton sender)
    {
      OpeningPreferences?.Invoke();
    }

    #region Test Project Dropdown
    public TestProject SelectedTestProject { get => selectedTestProject; set => selectedTestProject = value; }
    TestProject selectedTestProject;

    public void SetTestProjects(IEnumerable<TestProject> testProjects)
    {
      TestProjectDropdown.RemoveAllItems();
      TestProjectDropdown.AddItems(testProjects.Select(p => p.DisplayName).ToArray());      
    }
    #endregion


    public void DisableUI()
    {
      
    }

    public void EnableUI()
    {
      
    }

    static readonly Dictionary<LogLevel, NSColor> statusMessageColorMap = new Dictionary<LogLevel, NSColor>()
    {
      { LogLevel.Info, NSColor.FromRgb(1, 1, 1) },
      { LogLevel.Warn, NSColor.FromRgb(1, 1, 1) },
      { LogLevel.Error, NSColor.FromRgb(1, 0, 0) },
    };

    public void SetStatusMessage(string message, LogLevel level)
    {
      StatusLabel.StringValue = message;
      StatusLabel.TextColor = statusMessageColorMap[level];
    }

    public void ClearCoverageResults()
    {
      
    }

    public void SetCoverageResults(IReadOnlyDictionary<string, CoverageSummary> results)
    {
      currentResults = results;

    }

    partial void NextTestedProjectTapped(NSButton sender)
    {

    }

    partial void PreviousTestedProjectTapped(NSButton sender)
    {

    }

    async partial void GatherCoverageTapped(NSButton sender)
    {
      await presenter.GatherCoverageAsync();
    }
  }
}
