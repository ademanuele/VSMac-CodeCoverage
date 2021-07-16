using AppKit;
using CodeCoverage.Core;
using CodeCoverage.Coverage;
using CodeCoverage.Pad.Native;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using MonoDevelop.Ide;
using System;
using System.Collections.Generic;

namespace CodeCoverage
{
  class CodeCoverageMargin : ICocoaTextViewMargin
  {
    public double MarginSize => 25.0f;
    public NSView VisualElement => marginView;
    public bool Enabled => true;

    private PadView CoveragePad
    {
      get
      {
        var pad = IdeApp.Workbench.GetPad<CoveragePad>();
        if (pad?.Content is not CoveragePad coveragePad) return null;
        return coveragePad.PadView;
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
      Settings.Default.SettingsChanged += HandleSettingsChanged;
      if (CoveragePad == null) return;
      CoveragePad.SelectedTestProjectChanged += HandleSelectedTestProjectChanged;
      CoveragePad.CoverageResultsUpdated += HandleCoverageResultsUpdated;
      CoveragePad.CoverageResultsCleared += HandleCoverageResultsCleared;
    }

    private void OnTextViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) => marginView.NeedsDisplay = true;

    private void HandleSelectedTestProjectChanged() => UpdateCoverage();

    private void HandleCoverageResultsUpdated() => UpdateCoverage();

    private void HandleCoverageResultsCleared() => marginView.Coverage = null;

    private void HandleSettingsChanged(object sender, EventArgs e) => marginView.Colors = Settings.Default.MarginColors;

    void UpdateCoverage()
    {
      if (!TryGetCoverageFor(textView, out var results)) return;
      marginView.Coverage = results;
    }

    bool TryGetCoverageFor(ITextView textView, out Dictionary<int, int> coverage)
    {
      var filePath = GetFilePathFor(textView);
      var project = CoverageExtension.Presenter(CoveragePad).SelectedTestProject;

      if (project == null || filePath == null)
      {
        coverage = null;
        return false;
      }

      var configuration = IdeApp.Workspace.ActiveConfiguration;
      var results = CoverageExtension.Repository?.ResultsFor(project, configuration);
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
      Settings.Default.SettingsChanged -= HandleSettingsChanged;
      marginView.Dispose();
      if (CoveragePad == null) return;
      CoveragePad.SelectedTestProjectChanged -= HandleSelectedTestProjectChanged;
      CoveragePad.CoverageResultsUpdated -= HandleCoverageResultsUpdated;
      CoveragePad.CoverageResultsCleared -= HandleCoverageResultsCleared;
    }
  }
}