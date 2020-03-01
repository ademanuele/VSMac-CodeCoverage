using System;
using System.Threading.Tasks;
using MonoDevelop.Projects;

namespace CodeCoverage.Coverage
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

  class LoggedCoverageService : CoverageService, ILoggedCoverageService
  {
    IProgress<Log> progress;

    public LoggedCoverageService(ICoverageProvider provider, ICoverageResultsRepository repository) : base(provider, repository) { }

    public async Task CollectCoverageForTestProject(Project testProject, IProgress<Log> progress)
    {
      this.progress = progress;

      try
      {
        await CollectCoverageForTestProject(testProject);
      }
      catch (Exception e)
      {
        progress.Report(new Log("Failed to gather coverage. See log for details.", LogLevel.Error, e));
      }
    }

    protected override async Task RunTests(Project testProject)
    {
      progress.Report(new Log("Running unit tests...", LogLevel.Info));
      await base.RunTests(testProject);
    }

    protected override void SaveResults(ICoverageResults results, Project testProject, ConfigurationSelector configuration)
    {
      progress.Report(new Log("Saving coverage results...", LogLevel.Info));
      base.SaveResults(results, testProject, configuration);
      FinishedGatheringCoveage();
    }

    void FinishedGatheringCoveage()
    {
      progress.Report(new Log("Done gathering coverage.", LogLevel.Info));
    }
  }
}
