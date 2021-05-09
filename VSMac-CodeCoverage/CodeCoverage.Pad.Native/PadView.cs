using System;
using Foundation;
using AppKit;
using System.Reflection;
using System.IO;

namespace CodeCoverage.Pad.Native
{
  public partial class PadView : NSView
  {
    private const string padViewNibResourceId = "__xammac_content_PadView.nib";

    #region Constructors
    // Called when created from unmanaged code
    public PadView(IntPtr handle) : base(handle) { }

    // Called when created directly from a XIB file
    [Export("initWithCoder:")]
    public PadView(NSCoder coder) : base(coder) { }

    public PadView RootView { get; }

    public PadView()
    {
      Assembly assembly = typeof(PadView).Assembly;
      Stream nibStream = assembly.GetManifestResourceStream(padViewNibResourceId);
      NSData nibData = NSData.FromStream(nibStream);
      NSNib nib = new NSNib(nibData, NSBundle.MainBundle);

      if (nib.InstantiateNibWithOwner(this, out NSArray nibObjects))
        RootView = GetPadViewFrom(nibObjects);

      Initialize();
    }

    // Need to do this as the PadView is not always the first element in the NSArray.
    private static PadView GetPadViewFrom(NSArray array)
    {
      for (uint i = 0; i < array.Count; i++)
        try
        {
          return array.GetItem<PadView>(i);
        }
        catch { }

      return null;
    }

    private void Initialize()
    {
      
    }
    #endregion

    partial void PreferencesTapped(NSButton sender)
    {
      System.Diagnostics.Debug.WriteLine("PREFERENCES!");
    }
  }
}
