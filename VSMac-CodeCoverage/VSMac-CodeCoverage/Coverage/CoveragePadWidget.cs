using System;
using System.Collections.Generic;
using Gtk;
using Xwt.Drawing;
using Xwt.GtkBackend;

namespace CodeCoverage.Coverage
{
  [System.ComponentModel.ToolboxItem(true)]
  public partial class CoveragePadWidget : Bin, IStatusBar
  {

    static readonly Dictionary<LogLevel, Color> statusMessageColorMap = new Dictionary<LogLevel, Color>
    {
      { LogLevel.Info, new Color(1, 1, 1) },
      { LogLevel.Warn, new Color(1, 1, 1) },
      { LogLevel.Error, new Color(1, 0, 0) }
    };

    public CoverageWidget CoverageWidget { get; }
    readonly ILoggingService log;
    CoverageConsoleWindow consoleWindow;

    public CoveragePadWidget()
    {
      Build();
      log = new LoggingService();
      CoverageWidget = new CoverageWidget(log, this);
      rootVBox.PackStart(CoverageWidget, true, true, 5);
      SetupStatusLabel();

      void SetupStatusLabel()
      {
        var font = Pango.FontDescription.FromString("Courier 12");
        statusLabel.ModifyFont(font);
      }
    }

    public override void Dispose()
    {
      base.Dispose();
      CoverageWidget.Dispose();
      consoleWindow.Dispose();
    }

    public void SetStatusMessage(string message, LogLevel style)
    {
      statusLabel.Text = message;
      statusLabel.SetForegroundColor(statusMessageColorMap[style]);
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
}