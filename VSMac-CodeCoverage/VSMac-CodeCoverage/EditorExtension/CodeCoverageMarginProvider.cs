using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace CodeCoverage
{
  [Export(typeof(ICocoaTextViewMarginProvider))]
  [Name(nameof(CodeCoverageMarginProvider))]
  [Order(Before = PredefinedMarginNames.LineNumber)]
  [Order(After = PredefinedMarginNames.Glyph)]
  [MarginContainer(PredefinedMarginNames.Left)]
  [ContentType("code")]
  [TextViewRole(PredefinedTextViewRoles.Interactive)]
  public class CodeCoverageMarginProvider : ICocoaTextViewMarginProvider
  {
    public ICocoaTextViewMargin CreateMargin(ICocoaTextViewHost textViewHost, ICocoaTextViewMargin marginContainer)
    {      
      return new CodeCoverageMargin(textViewHost.TextView);
    }   
  }
}