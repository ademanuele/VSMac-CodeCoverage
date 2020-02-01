using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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

    public async Task CollectCoverageForTestProject(Project testProject)
    {
      var activeConfigruation = IdeApp.Workspace.ActiveConfiguration;
      await RebuildTestProject(testProject);      
      await RunTests(testProject, activeConfigruation);
      LastCoverageCollection = DateTime.Now;
    }

    protected virtual Task RebuildTestProject(Project testProject)
    {
      return IdeApp.ProjectOperations.Rebuild(testProject).Task;
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
  }
}
