using CodeCoverage.Coverage;
using Gtk;

namespace CodeCoverage
{
  public partial class PreferencesWindow : Window
  {
    readonly ConsoleWidget console;

    public PreferencesWindow(ILoggingService loggingService) : base(WindowType.Toplevel)
    {
      Build();
      var rootNotebook = new Notebook();
      Child = rootNotebook;

      console = new ConsoleWidget(loggingService);
      rootNotebook.AppendPage(console, new Label("Console"));

      rootNotebook.ShowAll();
    }
  }
}
