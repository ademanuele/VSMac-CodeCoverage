using AppKit;
using CodeCoverage.Coverage;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using System;
using System.Collections.Generic;

namespace CodeCoverage
{
  class CodeCoverageMargin : ICocoaTextViewMargin
  {
    public double MarginSize => 25.0f;
    public NSView VisualElement => marginView;
    public bool Enabled => true;

    private CoverageWidget CoverageWidget
    {
      get
      {
        if (!(IdeApp.Workbench.GetPad<CoveragePad>()?.Content is CoveragePad coveragePad)) return null;
        return ((CoveragePadWidget)coveragePad.Control).CoverageWidget;
      }
    }

    private readonly ITextView textView;
    private readonly CodeCoverageMarginView marginView;

    public CodeCoverageMargin(ITextView textView)
    {
      this.textView = textView;
      marginView = new CodeCoverageMarginView(textView, MarginSize, Settings.Default.MarginColors);
      this.textView.LayoutChanged += OnTextViewLayoutChanged;
      StartListeningForMaginChanges();
      UpdateCoverage();
    }

    private void StartListeningForMaginChanges()
    {
      CoverageWidget.SelectedTestProjectChanged += HandleSelectedTestProjectChanged;
      CoverageWidget.CoverageResultsUpdated += HandleCoverageResultsUpdated;
      CoverageWidget.CoverageResultsCleared += HandleCoverageResultsCleared;
      Settings.Default.SettingsChanged += HandleSettingsChanged;
    }

    private void OnTextViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) => marginView.NeedsDisplay = true;

    private void HandleSelectedTestProjectChanged(object sender, Project e) => UpdateCoverage();

    private void HandleCoverageResultsUpdated(object sender, EventArgs e) => UpdateCoverage();

    private void HandleCoverageResultsCleared(object sender, EventArgs e) => marginView.Coverage = null;

    private void HandleSettingsChanged(object sender, EventArgs e) => marginView.Colors = Settings.Default.MarginColors;

    void UpdateCoverage()
    {
      if (!TryGetCoverageFor(textView, out var results)) return;
      marginView.Coverage = results;
    }

    bool TryGetCoverageFor(ITextView textView, out Dictionary<int, int> coverage)
    {
      var filePath = GetFilePathFor(textView);
      var project = CoverageWidget.SelectedTestProject;

      if (project == null || filePath == null)
      {
        coverage = null;
        return false;
      }

      var configuration = IdeApp.Workspace.ActiveConfiguration;
      var results = CoveragePad.Repository.ResultsFor(project, configuration);
      if (results == null)
      {
        coverage = null;
        return false;
      }

      coverage = results.CoverageForFile(filePath);
      return true;
    }

    string GetFilePathFor(ITextView textView)
    {
      var documentBuffer = textView.TextDataModel.DocumentBuffer;
      if (!documentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument document)) return null;
      return document.FilePath;
    }

    public ITextViewMargin GetTextViewMargin(string marginName) => marginName == nameof(CodeCoverageMargin) ? this : null;

    public void Dispose()
    {
      textView.LayoutChanged -= OnTextViewLayoutChanged;
      CoverageWidget.SelectedTestProjectChanged -= HandleSelectedTestProjectChanged;
      CoverageWidget.CoverageResultsUpdated -= HandleCoverageResultsUpdated;
      CoverageWidget.CoverageResultsCleared -= HandleCoverageResultsCleared;
      Settings.Default.SettingsChanged -= HandleSettingsChanged;
      marginView.Dispose();
    }
  }
}