using System.Diagnostics;
using System.Reflection;
using Gtk;

namespace CodeCoverage.Pad
{
  [System.ComponentModel.ToolboxItem(true)]
  public partial class InfoWidget : Gtk.Bin
  {
    public InfoWidget()
    {
      Build();
      SetupTitle();
      SetupVersionLabel();
      AddMoreInfoButton();
    }

    void SetupTitle()
    {
      var font = Pango.FontDescription.FromString("Courier 30");
      titleLabel.ModifyFont(font);
    }

    void SetupVersionLabel()
    {
      Assembly assembly = Assembly.GetExecutingAssembly();
      FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      string version = fvi.ProductVersion;
      var font = Pango.FontDescription.FromString("Courier 20");
      versionLabel.ModifyFont(font);
      versionLabel.Text = $"Version {version}";
    }

    void AddMoreInfoButton()
    {
      var button = new LinkButton("https://github.com/ademanuele/VSMac-CodeCoverage", "GitHub");
      rootVBox.PackStart(button, false, false, 0);
    }
  }
}
