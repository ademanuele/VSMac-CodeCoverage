using System;
using System.Threading.Tasks;
using AltCover;
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

  public class LoggedCoverageService : CoverageService, ILoggedCoverageService
  {
    IProgress<Log> progress;

    public Task CollectCoverageForTestProject(Project testProject, IProgress<Log> progress)
    {
      this.progress = progress;
      return CollectCoverageForTestProject(testProject);
    }

    protected override Task RebuildTestProject(Project testProject)
    {
      progress.Report(new Log("Building test project...", LogLevel.Info));
      return base.RebuildTestProject(testProject);
    }

    protected override PrepareArgs PrepareProjectForCoverage(Project testProject, ConfigurationSelector configuration)
    {
      try
      {
        progress.Report(new Log("Preparing project for coverage...", LogLevel.Info));
        return base.PrepareProjectForCoverage(testProject, configuration);
      }
      catch (Exception e)
      {
        progress.Report(new Log(e.Message, LogLevel.Error));
        throw e;
      }
    }

    protected override Task RunTests(Project testProject, ConfigurationSelector configuration)
    {
      progress.Report(new Log("Running unit tests...", LogLevel.Info));
      return base.RunTests(testProject, configuration);
    }

    protected override void CollectCoverageData(Project testProject, PrepareArgs prepareParams)
    {
      try
      {
        progress.Report(new Log("Collecting coverage data...", LogLevel.Info));
        base.CollectCoverageData(testProject, prepareParams);
        FinishedGatheringCoveage();
      }
      catch (Exception e)
      {
        progress.Report(new Log(e.Message, LogLevel.Error));
        throw e;
      }
    }

    void FinishedGatheringCoveage()
    {
      progress.Report(new Log("Done gathering coverage.", LogLevel.Info));
    }
  }
}
