using System;
using Gtk;

namespace CodeCoverage
{
  [System.ComponentModel.ToolboxItem(true)]
  public partial class PreferencesWidget : Bin
  {
    public PreferencesWidget()
    {
      Build();
      LoadSettings();
    }

    void LoadSettings()
    {
      var marginColors = CodeCoverage.Settings.Default.MarginColors;
      foregroundColorButton.Color = marginColors.Foreground;
      coveredColorButton.Color = marginColors.BackgroundCovered;
      uncoveredColorButton.Color = marginColors.BackgroundUncovered;
    }

    protected void OnColorSelectionSet(object sender, EventArgs e)
    {
      SaveSettings();
    }

    void SaveSettings()
    {
      var settings = CodeCoverage.Settings.Default;
      settings.MarginColors = new MarginColors(foregroundColorButton.Color, coveredColorButton.Color, uncoveredColorButton.Color);
    }
  }
}
