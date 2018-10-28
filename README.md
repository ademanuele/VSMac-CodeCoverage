# VSMac-CodeCoverage

A code coverage extension for Visual Studio for Mac that provides a new pad for displaying coverage statistics and visualizing line coverage in the editor gutter.

![](https://raw.githubusercontent.com/ademanuele/vsmac-codecoverage/master/doc/preview.png "Preview")

## Installing

1. Download the extension from the [Releases](https://github.com/ademanuele/VSMac-CodeCoverage/releases) section.

2. In Visual Studio for Mac, open `Extension Manager -> Install from file...` and install the downloaded file.

![](https://raw.githubusercontent.com/ademanuele/vsmac-codecoverage/master/doc/extension_manager.png "Extension Manager")

3. Restart Visual Studio for Mac

4. Done.

In order access the code coverage pad, navigate to `View -> Pads -> Coverage`.

## Setting the Test Command

The coverage extension runs your unit tests while gathering coverage, which it does by using the unit testing command. This should be set to whatever is used for running the tests on the command line.

_Note: if the terminal command is usually prepended with the `mono` command, this should also be included. Very strange behaviour has been observed when it is not.`_

**Example Command:**
```
/path/to/NUnit.ConsoleRunner.x.x.x/tools/nunit3-console.exe --noresult {ProjectOutputFile}
```

The `{ProjectOutputFile}` placeholder is substituted by your unit testing project's `.dll` when running the tests.

## Planned TODOs

- Filter irrelevant assemblies from Coverage
- Refresh test projects dropdown and results when changing solutions
- Remove the need for a test command
	- Or at least, store a different test command per project/solution
- Gather coverage keyboard shortcut
- Make line marker background and text colors configurable
- Add coverage graphs
- Coverage trend over time graph
	- Keep X historic coverage results (settable as preference)
	- Plot summary

## Authors

* **Arthur Demanuele** - Coverage Extension

This extension uses **Steve Gilham**'s [altcover](https://github.com/SteveGilham/altcover) coverage tool for gathering coverage.

## License

This project is licensed under the MIT License - [full details](LICENSE.md).

See also [altcover's licence](https://github.com/SteveGilham/altcover/blob/master/LICENSE).

## Acknowledgments

Big thanks to the [altcover](https://github.com/SteveGilham/altcover) project, which made this extension possible.

