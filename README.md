[![Build Status](https://dev.azure.com/arthur-demanuele/VSMac-CodeCoverage/_apis/build/status/ademanuele.VSMac-CodeCoverage?branchName=master)](https://dev.azure.com/arthur-demanuele/VSMac-CodeCoverage/_build/latest?definitionId=1&branchName=master)

# VSMac-CodeCoverage

A code coverage extension for Visual Studio for Mac that provides a new pad for displaying coverage statistics and visualizing line coverage in the editor margin.

![](doc/preview.gif "Preview")

## Installation

1. Open Visual Studio Extension Repository Manager via `Visual Studio -> Extensions... -> Gallery -> Repositories Dropdown -> Manage Repositories`.

2. Add `https://raw.githubusercontent.com/ademanuele/VSMac-Extensions/master/main.mrep` to your repository sources.

3. Back in extension manager, you should now be able to see the extension listed as "Code Coverage". Select and press `install`.

4. Restart Visual Studio for Mac.

5. Done.

Any future updates to the extension should show up in the `Updates` tab of the Extension Manager.

## Usage

You can access the Coverage pad through `View -> Pads -> Coverage`.

Select any test project that is currently open in the workspace using the dropdown menu and hit `Gather Coverage`.
Your test project should start running. When complete, line and branch coverage results for each covered project are shown on the pad as well as in margins for any editors that you have open.

### Configuring Coverage Collection
This extension supports most of the coverage configuration options currently provided by coverlet.

These can be set by creating a file with the extension `.runsettings` in the directory alongside your solution file.

You can use this [sample runsettings file](doc/example.runsettings) as a starting point.

More info can be found on [coverlet's runsettings documentation.](https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/VSTestIntegration.md#advanced-options-supported-via-runsettings)

## Reporting Issues

If you find a bug or have a feature request, please report them at this repository's issues section.

## Support

If you like this tool and would like to support its development, you can...

<a href="https://www.buymeacoffee.com/arthurdemanuele" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/default-orange.png" alt="Buy Me A Coffee" style="height: 36px !important;width: 152px !important;" ></a>

## Authors

* **Arthur Demanuele** - Author

* This extension also uses **Toni Solarin-Sodara**'s [coverlet](https://github.com/tonerdo/coverlet) coverage tool for gathering coverage.

## Acknowledgments

Big thanks to the [coverlet](https://github.com/tonerdo/coverlet) project, which made this extension possible.

Big thanks to [David Karlas](https://developercommunity.visualstudio.com/users/25964/06b25657-7e73-4eef-bfae-8a6c57e7e6c9.html) for resolving an [issue](https://developercommunity.visualstudio.com/content/problem/907691/unable-to-create-custom-vs-for-mac-editor-margin.html) with the editor margin functionality.

## License

This project is licensed under the MIT License - [full details](LICENSE.md).

See also [coverlet's licence](https://github.com/tonerdo/coverlet/blob/master/LICENSE).