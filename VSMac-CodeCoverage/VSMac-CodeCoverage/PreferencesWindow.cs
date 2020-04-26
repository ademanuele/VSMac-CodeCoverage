using CodeCoverage.Coverage;
using Gtk;

namespace CodeCoverage
{
  public partial class PreferencesWindow : Window
  {
    readonly ConsoleWidget console;
    readonly PreferencesWidget preferences;

    public PreferencesWindow(ILoggingService loggingService) : base(WindowType.Toplevel)
    {
      Build();

      var rootNotebook = new Notebook();
      rootVBox.PackStart(rootNotebook);

      preferences = new PreferencesWidget();
      rootNotebook.AppendPage(preferences, new Label("Preferences"));

      console = new ConsoleWidget(loggingService);
      rootNotebook.AppendPage(console, new Label("Console"));

      rootNotebook.ShowAll();
    }

    protected void OnCloseButtonClicked(object sender, System.EventArgs e)
    {
      Destroy();
      Dispose();
    }
  }
}
