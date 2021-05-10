// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CodeCoverage.Pad.Native
{
	[Register ("PadView")]
	partial class PadView
	{
		[Outlet]
		AppKit.NSTextField BranchCoverageLabel { get; set; }

		[Outlet]
		AppKit.NSTextField LineCoverageLabel { get; set; }

		[Outlet]
		AppKit.NSTextField StatusLabel { get; set; }

		[Outlet]
		AppKit.NSTextField TestedProjectLabel { get; set; }

		[Outlet]
		AppKit.NSPopUpButton TestProjectDropdown { get; set; }

		[Action ("GatherCoverageTapped:")]
		partial void GatherCoverageTapped (AppKit.NSButton sender);

		[Action ("NextTestedProjectTapped:")]
		partial void NextTestedProjectTapped (AppKit.NSButton sender);

		[Action ("PreferencesTapped:")]
		partial void PreferencesTapped (AppKit.NSButton sender);

		[Action ("PreviousTestedProjectTapped:")]
		partial void PreviousTestedProjectTapped (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BranchCoverageLabel != null) {
				BranchCoverageLabel.Dispose ();
				BranchCoverageLabel = null;
			}

			if (LineCoverageLabel != null) {
				LineCoverageLabel.Dispose ();
				LineCoverageLabel = null;
			}

			if (StatusLabel != null) {
				StatusLabel.Dispose ();
				StatusLabel = null;
			}

			if (TestedProjectLabel != null) {
				TestedProjectLabel.Dispose ();
				TestedProjectLabel = null;
			}

			if (TestProjectDropdown != null) {
				TestProjectDropdown.Dispose ();
				TestProjectDropdown = null;
			}
		}
	}
}
