using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

namespace CodeCoverage.Coverage
{
  public class CoveragePad : PadContent
  {
    public override string Id => "CodeCoverage.Coverage.CoveragePad";

    public override Control Control => padWidget;

    private CoveragePadWidget padWidget;

    protected override void Initialize(IPadWindow window)
    {
      base.Initialize(window);
      padWidget = new CoveragePadWidget();
      padWidget.ShowAll();
    }
  }
}