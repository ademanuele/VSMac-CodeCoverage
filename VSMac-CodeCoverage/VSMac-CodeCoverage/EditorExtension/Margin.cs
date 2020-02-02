using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace CodeCoverage.EditorExtension
{
  [Export(typeof(ITextViewMargin))]
  [Name("Code Coverage Margin")]
  [Order(Before = "Wpf Line Number Margin")]
  [MarginContainer(PredefinedMarginNames.Left)]
  [ContentType("code")]
  public class CodeCoverageMargin : ITextViewMargin
  {
    public double MarginSize => 50;

    public bool Enabled => true;

    public void Dispose()
    {
      
    }

    public ITextViewMargin GetTextViewMargin(string marginName)
    {
      return this;
    }
  }
}
