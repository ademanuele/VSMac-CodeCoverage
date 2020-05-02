using System.Collections.Generic;
using Gtk;
using MonoDevelop.Projects;

namespace CodeCoverage.Coverage
{
  class TestProjectDropdownStore : ListStore
  {
    public IReadOnlyDictionary<Project, TreeIter> ProjectToIter => projectToIter;
    public IReadOnlyDictionary<TreeIter, Project> IterToProject => iterToProject;

    Dictionary<Project, TreeIter> projectToIter;
    Dictionary<TreeIter, Project> iterToProject;

    public TestProjectDropdownStore(IEnumerable<Project> projects) : base(typeof(string))
    {
      projectToIter = new Dictionary<Project, TreeIter>();
      iterToProject = new Dictionary<TreeIter, Project>();

      foreach (Project project in projects)
      {
        var newIter = AppendValues(project.Name);
        projectToIter.Add(project, newIter);
        iterToProject.Add(newIter, project);
      }
    }
  }
}
