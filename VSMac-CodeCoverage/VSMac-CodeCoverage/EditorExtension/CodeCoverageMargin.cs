using System;
using AppKit;
using CodeCoverage.Coverage;
using CoreGraphics;
using Foundation;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using System.Collections.Generic;
using System.Diagnostics;
using ICSharpCode.NRefactory.MonoCSharp;

namespace CodeCoverage
{
  class CodeCoverageMargin : NSView, ICocoaTextViewMargin
  {
    public double MarginSize => MarginWidth;
    public NSView VisualElement => this;
    public bool Enabled => true;

    private const float MarginWidth = 20.0f;
    private static readonly CGColor CoveredColor = new CGColor(0.54f, 0.97f, 0.57f);
    private static readonly CGColor UncoveredColor = new CGColor(0.97f, 0.57f, 0.54f);

    private readonly ITextView textView;
    private Dictionary<int, int> coverage;

    public override bool IsFlipped => true;
    public override CGSize IntrinsicContentSize => new CGSize(MarginWidth, NoIntrinsicMetric);

    private CoveragePadWidget CoveragePadWidget
    {
      get
      {
        if (!(IdeApp.Workbench.GetPad<CoveragePad>()?.Content is CoveragePad coveragePad)) return null;
        return (CoveragePadWidget)coveragePad.Control;
      }
    }

    public CodeCoverageMargin(ITextView textView)
    {
      this.textView = textView;
      this.textView.LayoutChanged += OnTextViewLayoutChanged;
      CoveragePadWidget.SelectedTestProjectChanged += HandleSelectedTestProjectChanged;
      UpdateCoverage();
    }

    private void HandleSelectedTestProjectChanged(object sender, Project e) => UpdateCoverage();

    void UpdateCoverage()
    {
      Debug.WriteLine("Updating Coverage for Margin...");
      if (!TryGetCoverageFor(textView, out var results)) return;
      coverage = results;
    }

    bool TryGetCoverageFor(ITextView textView, out Dictionary<int, int> coverage)
    {
      var filePath = GetFilePathFor(textView);
      var project = CoveragePadWidget.SelectedTestProject;

      if (project == null || filePath == null)
      {
        coverage = null;
        return false;
      }

      var configuration = IdeApp.Workspace.ActiveConfiguration;
      var results = CoverageResultsRepository.Instance.ResultsFor(project, configuration);
      coverage = results.CoverageForFile(filePath);
      return true;
    }

    string GetFilePathFor(ITextView textView)
    {
      var documentBuffer = textView.TextDataModel.DocumentBuffer;
      if (!documentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument document)) return null;
      return document.FilePath;
    }

    public override void DrawRect(CGRect dirtyRect)
    {
      if (coverage == null) return;

      var context = NSGraphicsContext.CurrentContext.CGContext;
      context.SetLineWidth(MarginWidth);

      foreach (var line in textView.TextViewLines)
      {
        var lineNumber = line.Start.GetContainingLine().LineNumber + 1;
        if (!coverage.TryGetValue(lineNumber, out int visitCount)) continue;

        context.SetFillColor(visitCount > 0 ? CoveredColor : UncoveredColor);
        var rect = new CGRect(0, line.Top, MarginWidth, line.Height);
        context.FillRect(rect);

        var attrs = new NSStringAttributes();
        var font = NSFont.SystemFontOfSize(11f);
        attrs.Font = font;

        var fontHeight = font.PointSize;
        var yOffset = (rect.Size.Height - fontHeight) / 2.0;
        CGRect textRect = new CGRect(0, rect.Y - yOffset, rect.Size.Width, fontHeight);

        var coverageString = new NSString(visitCount.ToString());
        coverageString.DrawString(textRect, new NSDictionary());
      }
    }

    private void OnTextViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
    {
      NeedsDisplay = true;
    }

    public ITextViewMargin GetTextViewMargin(string marginName)
    {
      return marginName == nameof(CodeCoverageMargin) ? this : null;
    }

    protected override void Dispose(bool disposing)
    {
      textView.LayoutChanged -= OnTextViewLayoutChanged;
      CoveragePadWidget.SelectedTestProjectChanged -= HandleSelectedTestProjectChanged;
    }
  }
}