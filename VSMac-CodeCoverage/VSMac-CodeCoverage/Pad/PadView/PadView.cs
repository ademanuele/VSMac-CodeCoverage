using System;
using Foundation;
using AppKit;

namespace CodeCoverage.Coverage.Pad.PadView
{
  public partial class PadView : NSView
  {
    // Called when created from unmanaged code
    public PadView(IntPtr handle) : base(handle)
    {
      Initialize();
    }

    // Called when created directly from a XIB file
    [Export("initWithCoder:")]
    public PadView(NSCoder coder) : base(coder)
    {
      Initialize();
    }

    // Shared initialization code
    void Initialize()
    {
    }
  }
}
