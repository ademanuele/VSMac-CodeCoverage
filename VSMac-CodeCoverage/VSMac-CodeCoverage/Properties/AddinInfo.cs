using Mono.Addins;
using Mono.Addins.Description;

[assembly: Addin(
    "Code Coverage",
    Namespace = "CodeCoverage",
    Version = "2.0.1"
)]

[assembly: AddinName("CodeCoverage")]
[assembly: AddinCategory("IDE extensions")]
[assembly: AddinDescription("A code coverage extension for Visual Studio for Mac.")]
[assembly: AddinAuthor("Arthur Demanuele")]
