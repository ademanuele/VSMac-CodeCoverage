using CodeCoverage.Pad.Native;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

namespace CodeCoverage.Coverage
{
  public class CoveragePad : PadContent
  {
    public override string Id => "CodeCoverage.Coverage.CoveragePad";

    public override Control Control => padView;
    
    private PadView padView;    

    protected override void Initialize(IPadWindow window)
    {
      base.Initialize(window);
      padView = new PadView().RootView;
    }
  }
}