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

  class CoverageService : IDisposable
  {
    readonly ICoverageProvider provider;
    readonly ICoverageResultsRepository repository;

    TaskCompletionSource<bool> coverageCollectionCompletion;

    public CoverageService(ICoverageProvider provider, ICoverageResultsRepository repository)
    {
      this.provider = provider;
      this.repository = repository;
      UnitTestService.TestSessionStarting += UnitTestService_TestSessionStarting;
    }

    public async Task CollectCoverageForTestProject(Project testProject)
    {
      await RunTests(testProject);
      if (coverageCollectionCompletion == null) return;
      await coverageCollectionCompletion.Task;
      coverageCollectionCompletion = null;
    }

    protected virtual async Task RunTests(Project testProject)
    {      
      IExecutionHandler mode = null;
      ExecutionContext context = new ExecutionContext(mode, IdeApp.Workbench.ProgressMonitors.ConsoleFactory, null);
      var firstRootTest = UnitTestService.FindRootTest(testProject);
      if (coverageCollectionCompletion != null || firstRootTest == null || !firstRootTest.CanRun(mode)) return;
      coverageCollectionCompletion = new TaskCompletionSource<bool>();
      await UnitTestService.RunTest(firstRootTest, context, true).Task;
    }

    private async void UnitTestService_TestSessionStarting(object sender, TestSessionEventArgs e)
    {
      if (coverageCollectionCompletion == null || !(e.Test.OwnerObject is Project testProject)) return;

      var configuration = IdeApp.Workspace.ActiveConfiguration;
      provider.Prepare(testProject, configuration);
      await e.Session.Task;
      var results = provider.GetCoverage(testProject, configuration);
      if (results != null) SaveResults(results, testProject, configuration);
      coverageCollectionCompletion.SetResult(true);
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
