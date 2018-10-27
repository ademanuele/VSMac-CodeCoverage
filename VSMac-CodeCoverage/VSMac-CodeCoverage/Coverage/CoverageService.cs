using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AltCover;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace CodeCoverage.Coverage
{
  public interface ICoverageService
  {
    DateTime LastCoverageCollection { get; }
    Task CollectCoverageForTestProject(Project testProject);
  }

  public class CoverageService
  {
    public static ILoggedCoverageService Instance => instance ?? (instance = new LoggedCoverageService());
    static ILoggedCoverageService instance;

    public DateTime LastCoverageCollection { get; private set; }

    readonly LogArgs coverageLogging;

    protected CoverageService()
    {
      coverageLogging = new LogArgs
      {
        Info = LoggingService.Info,
        Warn = LoggingService.Warn,
        Error = LoggingService.Error,
        Echo = LoggingService.Echo
      };
    }

    public async Task CollectCoverageForTestProject(Project testProject)
    {
      var activeConfigruation = IdeApp.Workspace.ActiveConfiguration;
      await RebuildTestProject(testProject);
      var prepareParams = PrepareProjectForCoverage(testProject, activeConfigruation);
      await RunTests(testProject, activeConfigruation);
      CollectCoverageData(testProject, prepareParams);
      LastCoverageCollection = DateTime.Now;
    }

    protected virtual Task RebuildTestProject(Project testProject)
    {
      return IdeApp.ProjectOperations.Rebuild(testProject).Task;
    }

    protected virtual PrepareArgs PrepareProjectForCoverage(Project testProject, ConfigurationSelector configuration)
    {
      try
      {
        var options = MakePrepareParamsForTestProject();
        DeleteCoverageDirectoryIfAlreadyExists(options);
        var statusCode = CSApi.Prepare(options, coverageLogging);

        if (statusCode != 0)
          throw new CodeCoverageException(testProject, $"Failed while preparing project for coverage. Status code: {statusCode}.");

        return options;
      }
      catch (Exception e)
      {
        throw new CodeCoverageException(testProject, $"Failed while preparing project for coverage.", e);
      }

      void DeleteCoverageDirectoryIfAlreadyExists(PrepareArgs options)
      {
        if (Directory.Exists(options.OutputDirectory))
          Directory.Delete(options.OutputDirectory, true);
      }

      PrepareArgs MakePrepareParamsForTestProject()
      {
        var projectOutputDirectory = testProject.GetOutputFileName(configuration).ParentDirectory;
        var coverageOutputDirectory = CoverageOutputPathForProject(testProject, configuration);
        var xmlReportPath = CoverageFilePathForProject(testProject, configuration);

        return new PrepareArgs
        {
          InputDirectory = projectOutputDirectory,
          OutputDirectory = coverageOutputDirectory,
          XmlReport = xmlReportPath,
          OpenCover = true,
          InPlace = true,
          Save = false
        };
      }
    }

    public static string CoverageOutputPathForProject(Project project, ConfigurationSelector configuration)
    {
      var projectOutputDirectory = project.GetOutputFileName(configuration).ParentDirectory;
      return projectOutputDirectory.Combine(".coverage");
    }

    public static string CoverageFilePathForProject(Project project, ConfigurationSelector configuration)
    {
      return Path.Combine(CoverageOutputPathForProject(project, configuration), "coverage.xml");
    }

    protected virtual Task RunTests(Project testProject, ConfigurationSelector configuration)
    {
      return Task.Run(() =>
      {
        string command = TestCommandSetting.ForProject(testProject, configuration);
        int argumentsStartIndex = command.IndexOf(' ');
        string pathToExecutable = command.Substring(0, argumentsStartIndex);
        string arguments = command.Substring(argumentsStartIndex);

        using (Process p = new Process())
        {
          p.StartInfo.UseShellExecute = false;
          p.StartInfo.RedirectStandardOutput = true;
          p.StartInfo.RedirectStandardError = true;
          p.StartInfo.FileName = pathToExecutable;
          p.StartInfo.Arguments = arguments;

          LoggingService.Info($"Running Tests Using Command:\n{command}");
          var success = p.Start();
          p.WaitForExit();

          LoggingService.Info($"Testing Output:\n\n{p.StandardOutput.ReadToEnd()}");
          LoggingService.Info($"Testing Error:\n\n{p.StandardError.ReadToEnd()}");
        }
      });
    }

    protected virtual void CollectCoverageData(Project testProject, PrepareArgs prepareParams)
    {
      try
      {
        var options = MakeCollectParams();
        var logging = coverageLogging;
        var statusCode = CSApi.Collect(options, logging);

        if (statusCode != 0)
          throw new CodeCoverageException(testProject, $"Failed while collecting coverage data. Status code: {statusCode}.");
      }
      catch (Exception e)
      {
        throw new CodeCoverageException(testProject, $"Failed while collection coverage data.", e);
      }

      CollectArgs MakeCollectParams()
      {
        return new CollectArgs
        {
          RecorderDirectory = prepareParams.InputDirectory
        };
      }
    }
  }
}
