using AppKit;
using CoreGraphics;
using Foundation;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Collections.Generic;

namespace CodeCoverage
{
  class CodeCoverageMarginView : NSView
  {
    public Dictionary<int, int> Coverage
    {
      get => coverage;
      set
      {
        coverage = value;
        NeedsDisplay = true;
      }
    }

    private static readonly CGColor CoveredColor = new CGColor(0.18f, 0.49f, 0.20f);
    private static readonly CGColor UncoveredColor = new CGColor(0.49f, 0.20f, 0.18f);
    private const float HorizontalPadding = 2f;

    private readonly ITextView textView;
    private readonly double width;
    private Dictionary<int, int> coverage;

    public CodeCoverageMarginView(ITextView textView, double width)
    {
      this.textView = textView;
      this.width = width;
    }

    public override bool IsFlipped => true;
    public override CGSize IntrinsicContentSize => new CGSize(width, NoIntrinsicMetric);

    public override void DrawRect(CGRect dirtyRect)
    {
      if (coverage == null) return;
      var context = NSGraphicsContext.CurrentContext.CGContext;

      foreach (var line in textView.TextViewLines)
      {
        var lineNumber = line.Start.GetContainingLine().LineNumber + 1;
        if (!coverage.TryGetValue(lineNumber, out int visitCount)) continue;
        DrawCoverageForLine(line, visitCount, context);
      }
    }

    void DrawCoverageForLine(ITextViewLine line, int visitCount, CGContext context)
    {
      var top = line.Top - textView.ViewportTop;
      var rect = new CGRect(0, top, width, line.Height);

      context.SetFillColor(visitCount > 0 ? CoveredColor : UncoveredColor);
      context.FillRect(rect);

      var attrs = new NSStringAttributes();
      var fontSize = (float)line.Height - 2f;
      var font = NSFont.MonospacedDigitSystemFontOfSize(fontSize, 1); // MAKE MONOSPACED
      attrs.Font = font;

      var yOffset = (rect.Size.Height - fontSize) / 2.0;

      CGRect textRect = new CGRect(HorizontalPadding, rect.Y + yOffset, rect.Size.Width - (HorizontalPadding * 2), fontSize);

      var coverageString = new NSString(visitCount.ToString());
      var stringAttributes = new NSStringAttributes
      {
        ForegroundColor = NSColor.White,
      };
      coverageString.DrawInRect(textRect, stringAttributes);
    }
  }
}