using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using MonoDevelop.Projects;

namespace CodeCoverage.Coverage
{
  public class CoverageResults
  {
    public Project TestProject { get; }
    public ConfigurationSelector Configuration { get; }
    public string ResultsFile { get; }
    public bool Valid => File.Exists(ResultsFile);

    readonly XPathDocument results;
    readonly XPathNavigator resultsNavigator;

    public CoverageResults(Project testProject, ConfigurationSelector configuration)
    {
      TestProject = testProject;
      Configuration = configuration;
      ResultsFile = CoverageService.CoverageFilePathForProject(TestProject, Configuration);
      if (!File.Exists(ResultsFile)) return;
      results = new XPathDocument(ResultsFile);
      resultsNavigator = results.CreateNavigator();
    }

    public bool HasResultsForProject(Project project)
    {
      string moduleXPath = XPathForModuleForProject(project);
      return resultsNavigator.SelectSingleNode(moduleXPath) != null;
    }

    public Dictionary<string, Coverage> AllModulesCoverage()
    {
      var coverage = new Dictionary<string, Coverage>();

      var moduleNamesXPath = "/CoverageSession/Modules/Module/ModuleName";
      var nameNodes = resultsNavigator.Select(moduleNamesXPath);

      foreach (XPathNavigator nameNode in nameNodes)
      {
        var moduleName = nameNode.Value;
        var summaryXPath = $"/CoverageSession/Modules/Module[ModuleName/text()=\"{moduleName}\"]/Summary";
        var moduleSummary = resultsNavigator.SelectSingleNode(summaryXPath);
        coverage.Add(moduleName, CoverageFromSummaryNode(moduleSummary));
      }

      return coverage;
    }

    public Coverage CoverageForProject(Project project)
    {
      var moduleSummaryXPath = $"{XPathForModuleForProject(project)}/Summary";
      var summaryNode = resultsNavigator.SelectSingleNode(moduleSummaryXPath);
      return CoverageFromSummaryNode(summaryNode);
    }

    Coverage CoverageFromSummaryNode(XPathNavigator summaryNode)
    {
      var hasSequencePoints = double.TryParse(summaryNode?.GetAttribute("numSequencePoints", ""), out var sequencePoints);
      var hasVisitedSequencePoints = double.TryParse(summaryNode?.GetAttribute("visitedSequencePoints", ""), out var visitedSequencePoints);

      var hasBranchPoints = double.TryParse(summaryNode?.GetAttribute("numBranchPoints", ""), out var branchPoints);
      var hasVisitedBranchPoints = double.TryParse(summaryNode?.GetAttribute("visitedBranchPoints", ""), out var visitedBranchPoints);

      if (summaryNode == null || !hasSequencePoints || !hasVisitedSequencePoints || !hasBranchPoints || !hasVisitedBranchPoints)
        return Coverage.NaN;

      var lineCoverage = (visitedSequencePoints / sequencePoints) * 100;
      var branchCoverage = (visitedBranchPoints / branchPoints) * 100;
      return new Coverage(lineCoverage, branchCoverage);
    }

    string XPathForModuleForProject(Project project)
    {
      var projectOutputFile = Path.GetFileName(project.GetOutputFileName(Configuration));
      var coverageOutputPath = CoverageService.CoverageOutputPathForProject(TestProject, Configuration);
      var coveredOutputFile = Path.Combine(coverageOutputPath, projectOutputFile);
      return $"/CoverageSession/Modules/Module[ModulePath/text()=\"{coveredOutputFile}\"]";
    }

    public Dictionary<int, int> CoverageForFile(string filePath, Project project)
    {
      if (!HasResultsForProject(project)) return null;
      var sequencePoints = GetSequencePointNodesForFile(filePath, project);

      if (sequencePoints == null || sequencePoints.Count == 0) return null;
      return GetCoverageFromSequencePointNodes(sequencePoints);
    }

    XPathNodeIterator GetSequencePointNodesForFile(string filePath, Project project)
    {
      string fileUID = FileUID(filePath, project);
      if (fileUID == null) return null;

      var sequencePointsXPath = XPathForSequencePointsForFileWithUID(fileUID, project);
      return resultsNavigator.Select(sequencePointsXPath);
    }

    string FileUID(string filePath, Project project)
    {
      var fileXpath = $"{XPathForModuleForProject(project)}/Files/File[@fullPath=\"{filePath}\"]";
      var fileNode = resultsNavigator.SelectSingleNode(fileXpath);
      return fileNode?.GetAttribute("uid", "");
    }

    string XPathForSequencePointsForFileWithUID(string uid, Project project)
      => $"{XPathForModuleForProject(project)}//Method[FileRef/@uid=\"{uid}\"]/SequencePoints/SequencePoint";

    Dictionary<int, int> GetCoverageFromSequencePointNodes(XPathNodeIterator sequencePoints)
    {
      Dictionary<int, int> coverage = new Dictionary<int, int>();
      foreach (XPathNavigator sequencePoint in sequencePoints)
      {
        var startLineAttribute = sequencePoint.GetAttribute("sl", "");
        var visitCountAttribute = sequencePoint.GetAttribute("vc", "");
        if (int.TryParse(startLineAttribute, out int startLine) && int.TryParse(visitCountAttribute, out int visitCount))
          try
          {
            coverage.Add(startLine, visitCount);
          }
          catch (ArgumentException)
          {
            coverage[startLine] += visitCount;
          }
      }

      return coverage;
    }
  }

  public struct Coverage
  {
    public double Line { get; }
    public double Branch { get; }

    public Coverage(double line, double branch)
    {
      Line = line;
      Branch = branch;
    }

    public static Coverage NaN { get; } = new Coverage(-1, -1);
  }
}
