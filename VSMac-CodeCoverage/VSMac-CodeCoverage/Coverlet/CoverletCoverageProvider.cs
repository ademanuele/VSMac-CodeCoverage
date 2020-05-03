using System;
using System.Collections.Generic;
using Coverlet.Core.Abstracts;
using Coverlet.Core.Helpers;
using MonoDevelop.Projects;
using CoverletCoverage = Coverlet.Core.Coverage;

namespace CodeCoverage.Coverage
{
  class CoverletCoverageProvider : ICoverageProvider
  {
    readonly Dictionary<Tuple<Project, ConfigurationSelector>, CoverletCoverage> projectCoverageMap;
    readonly ILogger logger;
    readonly FileSystem fileSystem;    

    public CoverletCoverageProvider(ILoggingService log)
    {
      logger = new LoggingServiceCoverletLogger(log);
      fileSystem = new FileSystem();
      projectCoverageMap = new Dictionary<Tuple<Project, ConfigurationSelector>, CoverletCoverage>();
    }

    public void Prepare(Project testProject, ConfigurationSelector configuration)
    {
      var unitTestDll = testProject.GetOutputFileName(configuration).ToString();
      var instrumentationHelper = new InstrumentationHelper(new ProcessExitHandler(), new RetryHelper(), fileSystem);
      var coverage = new CoverletCoverage(unitTestDll, 
          new string[0], // Include Filters
          new string[0], // Include directories
          new string[0], // Exclude Filters
          new string[0], // Excluded Source Files
          new string[0], // Exclude Attributes
          false, // Include test assembly
          false, // Single hit
          null, // Merge with
          false, // Use source link
          logger,
          instrumentationHelper,
          fileSystem);
      coverage.PrepareModules();
      projectCoverageMap[new Tuple<Project, ConfigurationSelector>(testProject, configuration)] = coverage;
    }

    public ICoverageResults GetCoverage(Project testProject, ConfigurationSelector configuration)
    {
      var key = new Tuple<Project, ConfigurationSelector>(testProject, configuration);
      if (!projectCoverageMap.TryGetValue(key, out CoverletCoverage coverage))
        return null;

      var results = coverage.GetCoverageResult();
      projectCoverageMap.Remove(key);
      return new CoverletCoverageResults(results);
    }
  }
}
