using System.IO;
using System.Reflection;
using AppKit;
using CodeCoverage.Coverage.Pad.PadView;
using Foundation;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

namespace CodeCoverage.Coverage
{
  public class CoveragePad : PadContent
  {
    public override string Id => "CodeCoverage.Coverage.CoveragePad";

    public override Control Control => padView;

    private const string padViewNibResourceId = "__xammac_content_PadView.nib";
    private PadView padView;

    protected override void Initialize(IPadWindow window)
    {
      base.Initialize(window);

      Assembly assembly = typeof(CoveragePad).Assembly;
      Stream nibStream = assembly.GetManifestResourceStream(padViewNibResourceId);
      NSData nibData = NSData.FromStream(nibStream);
      NSNib nib = new(nibData, NSBundle.MainBundle);
      if (nib.InstantiateNibWithOwner(null, out NSArray views))
      {
        var view = views.GetItem<PadView>(0);
        this.padView = view;
      }
    }
  }
}