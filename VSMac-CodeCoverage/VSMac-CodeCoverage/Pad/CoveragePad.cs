using System;
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

    public override Control Control => padView;

    private PadView padView;
    private PreferencesWindow preferencesWindow;
    private ILoggingService log;
    internal static ICoverageResultsRepository Repository { get; } = new CoverageResultsRepository(new CoverletResultsParser());
    private ICoverageProvider provider;

    protected override void Initialize(IPadWindow window)
    {
      base.Initialize(window);      
      provider = new CoverletCoverageProvider(log);
      log = new LoggingService();
      padView = new PadView(log, Repository, provider).RootView;
      padView.OpeningPreferences += PadView_OpeningPreferences;
    }

    public override void Dispose()
    {
      base.Dispose();
      preferencesWindow?.Dispose();
      padView.OpeningPreferences -= PadView_OpeningPreferences;
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