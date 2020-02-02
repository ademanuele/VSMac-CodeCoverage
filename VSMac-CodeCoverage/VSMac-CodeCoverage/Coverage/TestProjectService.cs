using System;
using System.Collections.Generic;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using MonoDevelop.UnitTesting;

namespace CodeCoverage.Coverage
{
  class TestProjectService : IDisposable
  {
    public IEnumerable<Project> TestProjects { get; private set; }

    public event TestProjectChangedDelegate TestProjectsChanged;
    public delegate void TestProjectChangedDelegate();

    public TestProjectService()
    {
      RefreshTestProjectList();
      UnitTestService.TestSuiteChanged += OnTestSuiteChanged;
    }

    public void Dispose()
    {
      UnitTestService.TestSuiteChanged -= OnTestSuiteChanged;
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
      TestProjects = projects;
      TestProjectsChanged?.Invoke();
    }
  }
}
