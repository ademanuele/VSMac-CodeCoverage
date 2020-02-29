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

  class CoverageService: IDisposable
  {
    readonly ICoverageProvider provider;
    readonly ICoverageResultsRepository repository;
    bool isBusy;

    public CoverageService(ICoverageProvider provider, ICoverageResultsRepository repository)
    {
      this.provider = provider;
      this.repository = repository;
      UnitTestService.TestSessionStarting += UnitTestService_TestSessionStarting;
    }

    public async Task CollectCoverageForTestProject(Project testProject)
    {
      isBusy = true;
      await RunTests(testProject);
    }

    protected virtual async Task RunTests(Project testProject)
    {
      IExecutionHandler mode = null;
      ExecutionContext context = new ExecutionContext(mode, IdeApp.Workbench.ProgressMonitors.ConsoleFactory, null);
      var firstRootTest = UnitTestService.FindRootTest(testProject);
      if (firstRootTest == null || !firstRootTest.CanRun(mode)) return;
      await UnitTestService.RunTest(firstRootTest, context, true).Task;
    }

    private void UnitTestService_TestSessionStarting(object sender, TestSessionEventArgs e)
    {
      if (!isBusy || !(e.Test.OwnerObject is Project testProject)) return;      
      var configuration = IdeApp.Workspace.ActiveConfiguration;
      provider.Prepare(testProject, configuration);
      e.Session.Task.ContinueWith(task =>
      {
        var results = provider.GetCoverage(testProject, configuration);
        SaveResults(results, testProject, configuration);
        isBusy = false;
      });
    }

    protected virtual void SaveResults(ICoverageResults results, Project testProject, ConfigurationSelector configuration)
    {
      repository.SaveResults(results, testProject, configuration);
    }

    public void Dispose()
    {
      UnitTestService.TestSessionStarting -= UnitTestService_TestSessionStarting;
    }
  }
}
