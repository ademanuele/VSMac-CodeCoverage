using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CodeCoverage.Core;
using Coverlet.Core;
using Coverlet.Core.Abstractions;
using Coverlet.Core.Helpers;
using Coverlet.Core.Symbols;
using MonoDevelop.Projects;
using CoverletCoverage = Coverlet.Core.Coverage;

namespace CodeCoverage.Coverlet
{
  public class CoverletCoverageProvider : ICoverageProvider
  {
    readonly Dictionary<Tuple<Project, ConfigurationSelector>, CoverletCoverage> projectCoverageMap;
    readonly ILogger logger;
    readonly FileSystem fileSystem;

    public string RunSettingsDataCollectorFriendlyName => "XPlat code coverage";

    private static CoverageParameters defaultCoverageParameters = new CoverageParameters
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

    public CoverletCoverageProvider(ILoggingService log)
    {
      logger = new LoggingServiceCoverletLogger(log);
      fileSystem = new FileSystem();
      projectCoverageMap = new Dictionary<Tuple<Project, ConfigurationSelector>, CoverletCoverage>();
    }

    public void Prepare(CoverageSettings settings)
    {
      var unitTestDll = settings.TestProject.GetOutputFileName(settings.Configuration).ToString();
      var sourceRootTranslator = new SourceRootTranslator(logger, fileSystem);
      var cecilSymbolHelper = new CecilSymbolHelper();
      var instrumentationHelper = new InstrumentationHelper(new ProcessExitHandler(), new RetryHelper(), fileSystem, logger, sourceRootTranslator);
      var coverageParameters = GetCoverageParameters(settings.CollectionSettings);

      var coverage = new CoverletCoverage(unitTestDll,
          coverageParameters,
          logger,
          instrumentationHelper,
          fileSystem,
          sourceRootTranslator,
          cecilSymbolHelper);
      coverage.PrepareModules();
      projectCoverageMap[new Tuple<Project, ConfigurationSelector>(settings.TestProject, settings.Configuration)] = coverage;
    }

    CoverageParameters GetCoverageParameters(XmlNode coverageSettings)
    {
      if (coverageSettings is null) return defaultCoverageParameters;
      return ParseSettings(coverageSettings);
    }

    CoverageParameters ParseSettings(XmlNode coverageSettings)
    {
      try
      {        
        string configurationXml = coverageSettings.InnerXml;
        XmlSerializer serializer = new XmlSerializer(typeof(CoverletRunSettingsConfiguration));
        using StringReader reader = new StringReader(configurationXml);
        CoverletRunSettingsConfiguration settings = (CoverletRunSettingsConfiguration)serializer.Deserialize(reader);
        return settings.ToParameters();
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
        return defaultCoverageParameters;
      }
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
