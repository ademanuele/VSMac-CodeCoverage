using System;
using System.Collections.Generic;
using Coverlet.Core;
using Coverlet.Core.Abstractions;
using Coverlet.Core.Helpers;
using Coverlet.Core.Symbols;
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
      var sourceRootTranslator = new SourceRootTranslator(logger, fileSystem);
      var cecilSymbolHelper = new CecilSymbolHelper();
      var instrumentationHelper = new InstrumentationHelper(new ProcessExitHandler(), new RetryHelper(), fileSystem, logger, sourceRootTranslator);

      var coverageParameters = new CoverageParameters
      {
        IncludeFilters = new string[0],
          IncludeDirectories = new string[0],
          ExcludeFilters = new string[0],
          ExcludedSourceFiles = new string[0],
          ExcludeAttributes = new string[0],
          IncludeTestAssembly = false,
          SingleHit = false,
          MergeWith = null,
          UseSourceLink = false,
      };

      var coverage = new CoverletCoverage(unitTestDll,
          coverageParameters,
          logger,
          instrumentationHelper,
          fileSystem,
          sourceRootTranslator,
          cecilSymbolHelper);
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
