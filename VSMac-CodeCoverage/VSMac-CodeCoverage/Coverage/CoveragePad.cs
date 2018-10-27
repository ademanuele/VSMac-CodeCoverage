using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;

namespace CodeCoverage.Coverage
{
  public class CoveragePad : PadContent
  {
    public override string Id => "CodeCoverage.Coverage.CoveragePad";

    public override Control Control => padWidget;
    public Project SelectedTestProject => padWidget?.SelectedTestProject;

    CoveragePadWidget padWidget;

    protected override void Initialize(IPadWindow window)
    {
      base.Initialize(window);
      padWidget = new CoveragePadWidget();
      padWidget.ShowAll();
    }
  }
}