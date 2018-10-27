using System;
using System.Collections.Generic;
using System.Linq;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Editor;
using MonoDevelop.Ide.Editor.Extension;
using MonoDevelop.Projects;
using Xwt.Drawing;

namespace CodeCoverage.Coverage
{
  public class CoverageTextEditorExtension : TextEditorExtension
  {
    Dictionary<string, DocumentCoverage> documentCoverage;

    struct DocumentCoverage
    {
      public DateTime ResultDate { get; }
      public IReadOnlyCollection<IUnitTestMarker> Markers { get; }

      public DocumentCoverage(DateTime resultDate, IReadOnlyCollection<IUnitTestMarker> markers)
      {
        ResultDate = resultDate;
        Markers = markers;
      }
    }

    protected override void Initialize()
    {
      base.Initialize();
      documentCoverage = new Dictionary<string, DocumentCoverage>();
      DocumentContext.DocumentParsed += HandleDocumentParsed;
    }

    public override void Dispose()
    {
      DocumentContext.DocumentParsed -= HandleDocumentParsed;
      base.Dispose();
    }

    void HandleDocumentParsed(object sender, EventArgs e)
    {
      var testProject = GetTestProject();
      if (testProject == null) return;

      var coverageResults = new CoverageResults(testProject, IdeApp.Workspace.ActiveConfiguration);
      if (!coverageResults.Valid || !coverageResults.HasResultsForProject(DocumentContext.Project))
      {
        ClearDocumentMarkers();
        return;
      }

      if (!documentCoverage.TryGetValue(DocumentContext.Name, out var previousCoverage) ||
          previousCoverage.ResultDate.CompareTo(CoverageService.Instance.LastCoverageCollection) < 0)
        AddCoverageMarkers(coverageResults);
    }

    Project GetTestProject()
    {
      var coveragePad = IdeApp.Workbench.GetPad<CoveragePad>()?.Content as CoveragePad;
      var selectedTestProject = coveragePad?.SelectedTestProject;

      if (selectedTestProject != null)
        return selectedTestProject;

      return TestProjectService.Instance.TestProjects?.FirstOrDefault();
    }

    void AddCoverageMarkers(CoverageResults results)
    {
      ClearDocumentMarkers();
      if (DocumentContext.Project == null) return;

      var fileCoverage = results.CoverageForFile(DocumentContext.Name, DocumentContext.Project);
      if (fileCoverage == null) return;

      var documentMarkers = new List<IUnitTestMarker>();
      foreach (var coveragePoint in fileCoverage)
        AddMarker(coveragePoint.Key, coveragePoint.Value, documentMarkers);

      documentCoverage[DocumentContext.Name] = new DocumentCoverage(DateTime.Now, documentMarkers);
    }

    void ClearDocumentMarkers()
    {
      if (!documentCoverage.TryGetValue(DocumentContext.Name, out var coverage)) return;
      foreach (var marker in coverage.Markers)
        Editor.RemoveMarker(marker);
    }

    void AddMarker(int line, int coverage, List<IUnitTestMarker> documentMarkers)
    {
      var unitTextMarker = TextMarkerFactory.CreateUnitTestMarker(Editor, new CoverageUnitTestHost(coverage), new UnitTestLocation(-1));
      Editor.AddMarker(line, unitTextMarker);
      documentMarkers.Add(unitTextMarker);
    }

    public static void RefreshActiveDocument()
    {
      IdeApp.Workbench.ActiveDocument?.UpdateParseDocument();
    }
  }

  class CoverageUnitTestHost : UnitTestMarkerHost
  {
    readonly static Color green = new Color(0.73, 0.96, 0.79).WithAlpha(0.5);
    readonly static Color red = new Color(1, 0.44, 0.26).WithAlpha(0.5);
    readonly static Color textColor = Colors.Black;
    const double IconWidth = 20;
    const double IconHeight = 15;
    const int LeftPadding = 3;
    const int IconFontSize = 10;

    Image cachedImage;
    int hits;

    public CoverageUnitTestHost(int hits)
    {
      this.hits = hits;
    }

    public override Image GetStatusIcon(string unitTestIdentifier, string caseId = null)
      => cachedImage ?? (cachedImage = CreateImage());

    Image CreateImage()
    {
      ImageBuilder builder = new ImageBuilder(IconWidth, IconHeight);
      var ctx = builder.Context;
      ctx.SetColor(hits == 0 ? red : green);
      ctx.Rectangle(LeftPadding, 0, IconWidth - LeftPadding, IconHeight);
      ctx.Fill();

      ctx.SetColor(textColor);
      ctx.DrawTextLayout(new TextLayout
      {
        Text = hits.ToString(),
        TextAlignment = Xwt.Alignment.Center,
        Trimming = TextTrimming.WordElipsis,
        Font = Font.SystemMonospaceFont.WithSize(IconFontSize)
      }, LeftPadding, 0);

      return builder.ToVectorImage();
    }

    public override string GetMessage(string unitTestIdentifier, string caseId = null) => "TEST";

    public override bool HasResult(string unitTestIdentifier, string caseId = null) => true;

    public override bool IsFailure(string unitTestIdentifier, string caseId = null) => false;

    public override void PopupContextMenu(UnitTestLocation unitTest, int x, int y) { }
  }
}
