using System;
using System.Collections.Generic;
using System.IO;
using MonoDevelop.Projects;

namespace CodeCoverage.Coverage
{
  interface ICoverageResultsRepository
  {
    ICoverageResults ResultsFor(Project testProject, ConfigurationSelector configuration);
    void SaveResults(ICoverageResults results, Project testProject, ConfigurationSelector conguration);
  }

  interface ICoverageResultsParser
  {
    string FileExtension { get; }
    ICoverageResults ParseFrom(Stream stream);
  }

  class CoverageResultsRepository : ICoverageResultsRepository
  {
    public static ICoverageResultsRepository Instance { get; } = new CoverageResultsRepository(new CoverletResultsParser());

    readonly ICoverageResultsParser parser;
    readonly Dictionary<Tuple<Project, ConfigurationSelector>, ICoverageResults> cache;

    private CoverageResultsRepository(ICoverageResultsParser parser)
    {
      this.parser = parser;
      cache = new Dictionary<Tuple<Project, ConfigurationSelector>, ICoverageResults>();
    }

    public ICoverageResults ResultsFor(Project testProject, ConfigurationSelector configuration)
    {
      if (cache.TryGetValue(new Tuple<Project, ConfigurationSelector>(testProject, configuration), out var result))
        return result;

      string resultsFilePath = CoverageFilePathForProject(testProject, configuration);
      if (!File.Exists(resultsFilePath)) return null;
      using FileStream stream = new FileStream(resultsFilePath, FileMode.Open);
      return parser.ParseFrom(stream);
    }

    public void SaveResults(ICoverageResults results, Project testProject, ConfigurationSelector configuration)
    {
      cache[new Tuple<Project, ConfigurationSelector>(testProject, configuration)] = results;
      Directory.CreateDirectory(CoverageDirectoryPathForProject(testProject, configuration));
      string resultsPath = CoverageFilePathForProject(testProject, configuration);
      using (var stream = new FileStream(resultsPath, FileMode.Create))
        results.SaveTo(stream);
    }

    string CoverageFilePathForProject(Project project, ConfigurationSelector configuration)
    {
      var coverageDirectoryPath = CoverageDirectoryPathForProject(project, configuration);
      return Path.Combine(coverageDirectoryPath, $"coverage.{parser.FileExtension}");
    }

    string CoverageDirectoryPathForProject(Project project, ConfigurationSelector configuration)
    {
      var projectOutputDirectory = project.GetOutputFileName(configuration).ParentDirectory;
      return projectOutputDirectory.Combine(".coverage");
    }
  }
}
