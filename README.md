# VSMac-CodeCoverage

A code coverage extension for Visual Studio for Mac that provides a new pad for displaying coverage statistics and visualizing line coverage in the editor margin.

![](doc/preview.gif "Preview")

## Installing

1. Download the extension from the [Releases](https://github.com/ademanuele/VSMac-CodeCoverage/releases) section.

2. In Visual Studio for Mac, open `Extension Manager -> Install from file...` and install the downloaded file.

![](doc/extension_manager.png "Extension Manager")

3. Restart Visual Studio for Mac.

4. Done.

## Usage
You can access the Coverage pad through `View -> Pads -> Coverage`.

Select any test project that is currently open in the workspace using the dropdown and hit "Gather Coverage".
Your test project should start running. When complete, line and branch coverage results for each covered project are shown on the pad as well as in margins for any editors that you have open.

## Planned TODOs

- Filter irrelevant assemblies from Coverage
- Refresh test projects dropdown and results when changing solutions
- Gather coverage keyboard shortcut
- Make editor margin background and text colors configurable
- Add coverage graphs
	- Coverage trend over time graph
		- Keep X historic coverage results (settable as preference)
		- Plot summary

## Authors

* **Arthur Demanuele** - Coverage Extension

This extension uses **Toni Solarin-Sodara**'s [coverlet](https://github.com/tonerdo/coverlet) coverage tool for gathering coverage.

## License

This project is licensed under the MIT License - [full details](LICENSE.md).

See also [coverlet's licence](https://github.com/tonerdo/coverlet/blob/master/LICENSE).

## Acknowledgments

Big thanks to the [coverlet](https://github.com/tonerdo/coverlet) project, which made this extension possible.


Big thanks to [David Karlas](https://developercommunity.visualstudio.com/users/25964/06b25657-7e73-4eef-bfae-8a6c57e7e6c9.html) for resolving an [issue](https://developercommunity.visualstudio.com/content/problem/907691/unable-to-create-custom-vs-for-mac-editor-margin.html) with the editor margin functionality.

