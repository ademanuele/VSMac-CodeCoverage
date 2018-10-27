using System;
using MonoDevelop.Projects;

namespace CodeCoverage.Coverage
{
  public class CodeCoverageException : Exception
  {
    public Project FailedProject { get; }

    public CodeCoverageException(Project failedProject, string message, Exception innerException) : base(message, innerException)
    {
      FailedProject = failedProject;
    }

    public CodeCoverageException(Project failedProject, string message) : base(message)
    {
      FailedProject = failedProject;
    }
  }
}
