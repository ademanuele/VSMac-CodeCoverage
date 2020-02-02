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

    public Log(string message, LogLevel level)
    {
      Message = message;
      Level = level;
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

    public Task CollectCoverageForTestProject(Project testProject, IProgress<Log> progress)
    {
      this.progress = progress;
      return CollectCoverageForTestProject(testProject);
    }

    protected override Task RunTests(Project testProject)
    {
      progress.Report(new Log("Running unit tests...", LogLevel.Info));
      return base.RunTests(testProject);
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
