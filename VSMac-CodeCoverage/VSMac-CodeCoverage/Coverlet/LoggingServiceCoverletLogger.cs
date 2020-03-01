using System;
using Coverlet.Core.Abstracts;

namespace CodeCoverage.Coverage
{
  class LoggingServiceCoverletLogger : ILogger
  {
    readonly ILoggingService loggingService;

    public LoggingServiceCoverletLogger(ILoggingService loggingService)
    {
      this.loggingService = loggingService;
    }

    public void LogError(string message)
    {
      loggingService.Error(message);
    }

    public void LogError(Exception exception)
    {
      loggingService.Error(exception.ToString());
    }

    public void LogInformation(string message, bool important = false)
    {
      loggingService.Info(message);
    }

    public void LogVerbose(string message)
    {
      loggingService.Info(message);
    }

    public void LogWarning(string message)
    {
      loggingService.Warn(message);
    }
  }
}
