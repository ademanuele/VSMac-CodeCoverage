using System;
using System.Threading.Tasks;
using System.Xml;
using MonoDevelop.Projects;

namespace CodeCoverage.Core
{
  public interface ILoggedCoverageService : ICoverageService
  {
    Task CollectCoverageForTestProject(Project testProject, IProgress<Log> progress);
  }

  public struct Log
  {
    public string Message { get; }
    public LogLevel Level { get; }
    public Exception Exception { get; }

    public Log(string message, LogLevel level, Exception exception = null)
    {
      Message = message;
      Level = level;
      Exception = exception;
    }
  }

  public enum LogLevel
  {
    Info,
    Warn,
    Error
  }

  public class LoggedCoverageService : CoverageService, ILoggedCoverageService
  {
    IProgress<Log> progress;
    readonly ILoggingService loggingService;

    public LoggedCoverageService(ICoverageProvider provider, ICoverageResultsRepository repository, ILoggingService loggingService) : base(provider, repository) {
      this.loggingService = loggingService;
    }

    public async Task CollectCoverageForTestProject(Project testProject, IProgress<Log> progress)
    {
      this.progress = progress;
      loggingService.Info($"\n----\n");
      loggingService.Info($"Starting coverage gathering: {testProject.Name}");
      try
      {
        await CollectCoverageForTestProject(testProject);
      }
      catch (Exception e)
      {
        progress.Report(new Log("Failed to gather coverage. See log for details.", LogLevel.Error, e));
        loggingService.Error("Failed to gather coverage.");
      }
    }

    protected override async Task RunTests(Project testProject)
    {
      progress.Report(new Log("Running unit tests...", LogLevel.Info));
      loggingService.Info($"Running unit tests: {testProject.Name}");
      await base.RunTests(testProject);
    }

    protected override XmlNode ParseRunSettings(string runSettingsFile)
    {
      loggingService.Info($"Using run settings file: {runSettingsFile}");
      return base.ParseRunSettings(runSettingsFile);
    }

    protected override void SaveResults(ICoverageResults results, Project testProject, ConfigurationSelector configuration)
    {
      progress.Report(new Log("Saving coverage results...", LogLevel.Info));
      loggingService.Info($"Saving coverage results...");
      base.SaveResults(results, testProject, configuration);
      FinishedGatheringCoveage();
    }

    void FinishedGatheringCoveage()
    {
      progress.Report(new Log("Done gathering coverage.", LogLevel.Info));
      loggingService.Info("Done gathering coverage.");
      loggingService.Info($"\n----\n");
    }
  }
}
