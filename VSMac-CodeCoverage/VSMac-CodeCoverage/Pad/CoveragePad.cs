using System;
using System.Diagnostics;
using CodeCoverage.Core;
using CodeCoverage.Coverlet;
using CodeCoverage.Pad.Native;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

namespace CodeCoverage.Coverage
{
  public class CoveragePad : PadContent
  {
    public override string Id => "CodeCoverage.Coverage.CoveragePad";

    public override Control Control => PadView;

    internal PadView PadView { get; private set; }
    private PreferencesWindow preferencesWindow;
    private ILoggingService log;    

    protected override void Initialize(IPadWindow window)
    {
      base.Initialize(window);
      log = new LoggingService();
      CoverageExtension.Setup(log, new CoverletCoverageProvider(log), new CoverletResultsParser());
      PadView = new PadView().RootView;
      PadView.OpeningPreferences += PadView_OpeningPreferences;
    }

    public override void Dispose()
    {
      base.Dispose();
      PadView.OpeningPreferences -= PadView_OpeningPreferences;
      preferencesWindow?.Dispose();
    }

    private void PadView_OpeningPreferences()
    {    
      if (preferencesWindow == null)
      {
        preferencesWindow = new PreferencesWindow(log);
        preferencesWindow.Destroyed += HandleConsoleWindowDestroyed;
      }

      preferencesWindow.ShowAll();
    }

    void HandleConsoleWindowDestroyed(object sender, EventArgs e)
    {
      preferencesWindow.Destroyed -= HandleConsoleWindowDestroyed;
      preferencesWindow = null;
    }
  }
}