using System.Runtime.CompilerServices;
using MonoDevelop.Core;

namespace CodeCoverage
{
  class Settings
  {
    public static Settings Default = new Settings();

    private Settings() { }

    public MarginColors MarginColors
    {
      get => MarginColors.TryParse(GetProperty(""), out MarginColors colors) ?
        colors : MarginColors.DefaultColors;

      set => SaveProperty(value.ToString());
    }

    void SaveProperty<T>(T value, [CallerMemberName] string propertyName = "")
    {
      PropertyService.Set($"CodeCoverage.{propertyName}", value);
    }

    T GetProperty<T>(T defaultValue, [CallerMemberName] string propertyName = "")
    {
      return PropertyService.Get($"CodeCoverage.{propertyName}", defaultValue);
    }
  }
}
