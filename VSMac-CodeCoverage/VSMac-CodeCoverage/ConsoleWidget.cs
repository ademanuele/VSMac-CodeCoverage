using System;
using CodeCoverage.Coverage;
using Pango;

namespace CodeCoverage
{
  [System.ComponentModel.ToolboxItem(true)]
  public partial class ConsoleWidget : Gtk.Bin
  {
    readonly ILoggingService loggingService;

    public ConsoleWidget(ILoggingService log)
    {
      loggingService = log;
      Build();
    }

    protected override void OnShown()
    {
      base.OnShown();
      consoleTextView.Buffer.Text = loggingService.Log;
      consoleTextView.ModifyFont(FontDescription.FromString("Courier 12"));
      loggingService.Logged += LoggingService_Logged;
      loggingService.Cleared += LoggingService_Cleared;
    }

    protected override void OnHidden()
    {
      base.OnHidden();
      Dispose();
    }

    public override void Dispose()
    {
      base.Dispose();
      loggingService.Logged -= LoggingService_Logged;
      loggingService.Cleared -= LoggingService_Cleared;
    }

    void LoggingService_Logged(object sender, string msg)
    {
      Gtk.Application.Invoke(delegate {
        consoleTextView.Buffer.Text += $"{msg}\n";
      });
    }

    void LoggingService_Cleared(object sender, EventArgs e)
    {
      Gtk.Application.Invoke(delegate {
        consoleTextView.Buffer.Clear();
      });
    }

    protected void HandleClearConsoleClicked(object sender, EventArgs e)
      => loggingService.Clear();
  }
}
