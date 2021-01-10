using CodeCoverage.Coverage;
using CodeCoverage.Pad;
using Gtk;

namespace CodeCoverage
{
  public partial class PreferencesWindow : Window
  {
    readonly PreferencesWidget preferences;
    readonly ConsoleWidget console;
    readonly InfoWidget info;

    public PreferencesWindow(ILoggingService loggingService) : base(WindowType.Toplevel)
    {
      Build();

      var rootNotebook = new Notebook();
      rootVBox.PackStart(rootNotebook);

      console = new ConsoleWidget(loggingService);
      rootNotebook.AppendPage(console, new Label("Console"));

      preferences = new PreferencesWidget();
      rootNotebook.AppendPage(preferences, new Label("Appearance"));

      info = new InfoWidget();
      rootNotebook.AppendPage(info, new Label("Info"));

      rootNotebook.ShowAll();
    }

    protected void OnCloseButtonClicked(object sender, System.EventArgs e)
    {
      Destroy();
      Dispose();
    }
  }
}
