using System;

namespace CodeCoverage.Coverage
{
  public partial class CoverageConsoleWindow : Gtk.Window
  {
    public CoverageConsoleWindow() : base(Gtk.WindowType.Toplevel)
    {
      Build();
    }

    protected override void OnShown()
    {
      base.OnShown();
      consoleTextView.Buffer.Text = LoggingService.log;
      LoggingService.Logged += LoggingService_Logged;
      LoggingService.Cleared += LoggingService_Cleared;
    }

    protected override void OnHidden()
    {
      base.OnHidden();
      Dispose();
    }

    public override void Dispose()
    {
      base.Dispose();
      LoggingService.Logged -= LoggingService_Logged;
      LoggingService.Cleared -= LoggingService_Cleared; 
    }

    void LoggingService_Logged(string msg)
    {
      Gtk.Application.Invoke(delegate {
        consoleTextView.Buffer.Text += $"{msg}\n";
      });
    }

    void LoggingService_Cleared()
    {
      Gtk.Application.Invoke(delegate {
        consoleTextView.Buffer.Clear();
      });
    }

    protected void HandleClearConsoleClicked(object sender, EventArgs e)
      => LoggingService.Clear();
  }

  public static class LoggingService
  {
    const string Copyright = "Copyright 2018 Arthur Demanuele\n";

    public static string log = Copyright;
    public static event Action<string> Logged;
    public static event Action Cleared;

    public static void Clear()
    {
      log = Copyright;
      Cleared?.Invoke();
    }

    public static void Info(string message)
    {
      var m = $"\n{message}";
      log += m;
      Logged?.Invoke(m);
    }

    public static void Warn(string message)
    {
      var m = $"\nWARN:\n{message}";
      log += m;
      Logged?.Invoke(m);
    }

    public static void Error(string message)
    {
      var m = $"\nERROR:\n{message}";
      log += m;
      Logged?.Invoke(m);
    }

    public static void Echo(string message)
    {
      var m = $"\nECHO:\n{message}";
      log += m;
      Logged?.Invoke(m);
    }

  }
}
