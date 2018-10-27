using System.Linq;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
namespace CodeCoverage.Coverage
{
  public class TestCommandDialogPresenter
  {
    public string Command
    {
      private get => command; set
      {
        command = value;
        UpdatePreview();
      }
    }
    string command;

    const string DummyOutputFilePath = "/path/to/UnitTests.dll";

    ITestCommandDialog widget;

    public TestCommandDialogPresenter(ITestCommandDialog widget)
    {
      this.widget = widget;
      Command = TestCommandSetting.Current() ?? "";
      widget.SetCommandField(Command);
      UpdatePreview();
    }

    void UpdatePreview()
    {
      string preview = DefaultPreview();

      if (TestProjectService.Instance.TestProjects.Count != 0)
        preview = MakePreview(TestProjectService.Instance.TestProjects.ElementAt(0));

      widget.SetPreview(preview);
    }

    string MakePreview(Project project)
    {
      return TestCommandSetting.FormatForProject(Command, project, IdeApp.Workspace.ActiveConfiguration);
    }

    string DefaultPreview()
    {
      return Command.Replace(TestCommandSetting.ProjectOutputFilePlaceholder, DummyOutputFilePath);
    }

    public void Save()
    {
      TestCommandSetting.Update(Command);
    }
  }

  public interface ITestCommandDialog
  {
    void SetCommandField(string command);
    void SetPreview(string preview);
  }

  public static class TestCommandSetting
  {
    public const string ProjectOutputFilePlaceholder = "{ProjectOutputFile}";

    const string PreferencesKey = "CodeCoverage.67db9c64-f509-4ee4-b466-807fd1d93c6e";

    public static string Current()
    {
      return PropertyService.Get<string>(PreferencesKey, null);
    }

    public static string ForProject(Project project, ConfigurationSelector configuration)
    {
      string setting = PropertyService.Get<string>(PreferencesKey, null);
      if (setting == null) return null;

      return FormatForProject(setting, project, configuration);
    }

    public static string FormatForProject(string command, Project project, ConfigurationSelector configuration)
    {
      var outputFile = project.GetOutputFileName(configuration);
      return command.Replace(ProjectOutputFilePlaceholder, outputFile);
    }

    public static void Update(string newCommand)
    {
      PropertyService.Set(PreferencesKey, newCommand);
    }
  }
}
