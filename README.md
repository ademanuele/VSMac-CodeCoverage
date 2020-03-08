# VSMac-CodeCoverage

A code coverage extension for Visual Studio for Mac that provides a new pad for displaying coverage statistics and visualizing line coverage in the editor margin.

![](doc/preview.gif "Preview")

## Installing

1. Download the extension from the [Releases](https://github.com/ademanuele/VSMac-CodeCoverage/releases) section.

2. In Visual Studio for Mac, open `Extension Manager -> Install from file...` and install the downloaded file.

3. Restart Visual Studio for Mac.

4. Done.

## Usage
You can access the Coverage pad through `View -> Pads -> Coverage`.

Select any test project that is currently open in the workspace using the dropdown and hit "Gather Coverage".
Your test project should start running. When complete, line and branch coverage results for each covered project are shown on the pad as well as in margins for any editors that you have open.

## Planned TODOs

- Gather coverage keyboard shortcut
- Make editor margin background and text colors configurable
- Add coverage graphs

## Authors

I, **Arthur Demanuele** am the sole author of this Visual Studio for Mac coverage extension.

This extension also uses **Toni Solarin-Sodara**'s [coverlet](https://github.com/tonerdo/coverlet) coverage tool for gathering coverage.

## License

This project is licensed under the MIT License - [full details](LICENSE.md).

See also [coverlet's licence](https://github.com/tonerdo/coverlet/blob/master/LICENSE).

## Acknowledgments

Big thanks to the [coverlet](https://github.com/tonerdo/coverlet) project, which made this extension possible.


Big thanks to [David Karlas](https://developercommunity.visualstudio.com/users/25964/06b25657-7e73-4eef-bfae-8a6c57e7e6c9.html) for resolving an [issue](https://developercommunity.visualstudio.com/content/problem/907691/unable-to-create-custom-vs-for-mac-editor-margin.html) with the editor margin functionality.

## Support

If you like this tool and would like to support its development, you can...

<style>.bmc-button img{height: 34px !important;width: 35px !important;margin-bottom: 1px !important;box-shadow: none !important;border: none !important;vertical-align: middle !important;}.bmc-button{padding: 7px 10px 7px 10px !important;line-height: 35px !important;height:51px !important;min-width:217px !important;text-decoration: none !important;display:inline-flex !important;color:#FFFFFF !important;background-color:#FF813F !important;border-radius: 5px !important;border: 1px solid transparent !important;padding: 7px 10px 7px 10px !important;font-size: 28px !important;letter-spacing:0.6px !important;box-shadow: 0px 1px 2px rgba(190, 190, 190, 0.5) !important;-webkit-box-shadow: 0px 1px 2px 2px rgba(190, 190, 190, 0.5) !important;margin: 0 auto !important;font-family:'Cookie', cursive !important;-webkit-box-sizing: border-box !important;box-sizing: border-box !important;-o-transition: 0.3s all linear !important;-webkit-transition: 0.3s all linear !important;-moz-transition: 0.3s all linear !important;-ms-transition: 0.3s all linear !important;transition: 0.3s all linear !important;}.bmc-button:hover, .bmc-button:active, .bmc-button:focus {-webkit-box-shadow: 0px 1px 2px 2px rgba(190, 190, 190, 0.5) !important;text-decoration: none !important;box-shadow: 0px 1px 2px 2px rgba(190, 190, 190, 0.5) !important;opacity: 0.85 !important;color:#FFFFFF !important;}</style><link href="https://fonts.googleapis.com/css?family=Cookie" rel="stylesheet"><a class="bmc-button" target="_blank" href="https://www.buymeacoffee.com/arthurdemanuele"><img src="https://cdn.buymeacoffee.com/buttons/bmc-new-btn-logo.svg" alt="Buy me a coffee"><span style="margin-left:15px;font-size:28px !important;">Buy me a coffee</span></a>