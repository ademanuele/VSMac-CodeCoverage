using System;
using System.Collections.Generic;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using MonoDevelop.UnitTesting;

namespace CodeCoverage.Coverage
{
  public class TestProjectService : IDisposable
  {
    public static TestProjectService Instance => instance ?? (instance = new TestProjectService());
    static TestProjectService instance;

    public IReadOnlyCollection<Project> TestProjects => testProjects;

    public delegate void TestProjectsChangedDelegate();
    public event TestProjectsChangedDelegate TestProjectsChanged;

    IReadOnlyCollection<Project> testProjects;

    TestProjectService()
    {
      RefreshTestProjectList();
      UnitTestService.TestSuiteChanged += OnTestSuiteChanged;
    }

    public void Dispose()
    {
      UnitTestService.TestSuiteChanged -= OnTestSuiteChanged;
      instance = null;
    }

    void OnTestSuiteChanged(object sender, EventArgs e)
      => RefreshTestProjectList();

    void RefreshTestProjectList()
    {
      List<Project> projects = new List<Project>();
      foreach (Project project in IdeApp.Workspace.GetAllProjects())
      {
        if (UnitTestService.FindRootTest(project) == null) continue;
        projects.Add(project);
      }
      testProjects = projects;
      TestProjectsChanged?.Invoke();
    }
  }
}
