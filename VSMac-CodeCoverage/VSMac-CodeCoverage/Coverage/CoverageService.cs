using System;
using System.Threading.Tasks;
using MonoDevelop.Core.Execution;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using MonoDevelop.UnitTesting;

namespace CodeCoverage.Coverage
{
  public interface ICoverageService
  {
    Task CollectCoverageForTestProject(Project testProject);
  }

  interface ICoverageProvider
  {
    void Prepare(Project testProject, ConfigurationSelector configuration);
    ICoverageResults GetCoverage(Project testProject, ConfigurationSelector configuration);
  }

  class CoverageService
  {
    public DateTime LastCoverageCollection { get; private set; }
    readonly ICoverageProvider provider;
    protected readonly ICoverageResultsRepository repository;

    public CoverageService(ICoverageProvider provider, ICoverageResultsRepository repository)
    {
      this.provider = provider;
      this.repository = repository;
    }

    public async Task CollectCoverageForTestProject(Project testProject)
    {
      var configuration = IdeApp.Workspace.ActiveConfiguration;
      provider.Prepare(testProject, configuration);      
      await RunTests(testProject).ContinueWith(t =>
      {
        var results = provider.GetCoverage(testProject, configuration);
        SaveResults(results, testProject, configuration);
      });
    }

    protected virtual void SaveResults(ICoverageResults results, Project testProject, ConfigurationSelector configuration)
    {
      repository.SaveResults(results, testProject, configuration);
    }

    protected virtual async Task RunTests(Project testProject)
    {
      IExecutionHandler mode = null;      
      ExecutionContext context = new ExecutionContext(mode, IdeApp.Workbench.ProgressMonitors.ConsoleFactory, null);      
      var firstRootTest = UnitTestService.FindRootTest(testProject);
      if (firstRootTest == null || !firstRootTest.CanRun(mode)) return;
      await UnitTestService.RunTest(firstRootTest, context, true).Task;
    }
  }
}
