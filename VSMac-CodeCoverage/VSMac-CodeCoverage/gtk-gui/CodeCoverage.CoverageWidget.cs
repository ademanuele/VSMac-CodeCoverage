
// This file has been generated by the GUI designer. Do not modify.
namespace CodeCoverage
{
	public partial class CoverageWidget
	{
		private global::Gtk.VBox vbox1;

		private global::Gtk.HBox hbox2;

		private global::Gtk.ComboBox testProjectDropdown;

		private global::Gtk.Button gatherCoverageButton;

		private global::Gtk.VBox vbox4;

		private global::Gtk.HBox hbox6;

		private global::Gtk.Button coveredProjectPreviouButton;

		private global::Gtk.Label coveredProjectLabel;

		private global::Gtk.Button coveredProjectNextButton;

		private global::Gtk.HBox hbox1;

		private global::Gtk.Frame frame1;

		private global::Gtk.Alignment GtkAlignment5;

		private global::Gtk.Label lineCoverageLabel;

		private global::Gtk.Label GtkLabel6;

		private global::Gtk.Frame frame3;

		private global::Gtk.Alignment GtkAlignment6;

		private global::Gtk.Label branchCoverageLabel;

		private global::Gtk.Label GtkLabel7;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget CodeCoverage.CoverageWidget
			global::Stetic.BinContainer.Attach(this);
			this.Name = "CodeCoverage.CoverageWidget";
			// Container child CodeCoverage.CoverageWidget.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 2;
			this.hbox2.BorderWidth = ((uint)(2));
			// Container child hbox2.Gtk.Box+BoxChild
			this.testProjectDropdown = global::Gtk.ComboBox.NewText();
			this.testProjectDropdown.Name = "testProjectDropdown";
			this.hbox2.Add(this.testProjectDropdown);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.testProjectDropdown]));
			w1.Position = 0;
			// Container child hbox2.Gtk.Box+BoxChild
			this.gatherCoverageButton = new global::Gtk.Button();
			this.gatherCoverageButton.CanFocus = true;
			this.gatherCoverageButton.Name = "gatherCoverageButton";
			this.gatherCoverageButton.UseUnderline = true;
			this.gatherCoverageButton.Label = global::Mono.Unix.Catalog.GetString("Gather Coverage");
			this.hbox2.Add(this.gatherCoverageButton);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.gatherCoverageButton]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			this.vbox1.Add(this.hbox2);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox2]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.vbox4 = new global::Gtk.VBox();
			this.vbox4.Name = "vbox4";
			// Container child vbox4.Gtk.Box+BoxChild
			this.hbox6 = new global::Gtk.HBox();
			this.hbox6.Name = "hbox6";
			this.hbox6.Homogeneous = true;
			this.hbox6.Spacing = 6;
			// Container child hbox6.Gtk.Box+BoxChild
			this.coveredProjectPreviouButton = new global::Gtk.Button();
			this.coveredProjectPreviouButton.CanFocus = true;
			this.coveredProjectPreviouButton.Name = "coveredProjectPreviouButton";
			this.coveredProjectPreviouButton.UseUnderline = true;
			global::Gtk.Image w4 = new global::Gtk.Image();
			w4.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-go-back", global::Gtk.IconSize.Menu);
			this.coveredProjectPreviouButton.Image = w4;
			this.hbox6.Add(this.coveredProjectPreviouButton);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox6[this.coveredProjectPreviouButton]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child hbox6.Gtk.Box+BoxChild
			this.coveredProjectLabel = new global::Gtk.Label();
			this.coveredProjectLabel.Name = "coveredProjectLabel";
			this.hbox6.Add(this.coveredProjectLabel);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox6[this.coveredProjectLabel]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			// Container child hbox6.Gtk.Box+BoxChild
			this.coveredProjectNextButton = new global::Gtk.Button();
			this.coveredProjectNextButton.CanFocus = true;
			this.coveredProjectNextButton.Name = "coveredProjectNextButton";
			this.coveredProjectNextButton.UseUnderline = true;
			global::Gtk.Image w7 = new global::Gtk.Image();
			w7.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-go-forward", global::Gtk.IconSize.Menu);
			this.coveredProjectNextButton.Image = w7;
			this.hbox6.Add(this.coveredProjectNextButton);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox6[this.coveredProjectNextButton]));
			w8.Position = 2;
			w8.Expand = false;
			w8.Fill = false;
			this.vbox4.Add(this.hbox6);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox4[this.hbox6]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox();
			this.hbox1.WidthRequest = 70;
			this.hbox1.HeightRequest = 70;
			this.hbox1.Name = "hbox1";
			this.hbox1.Homogeneous = true;
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame();
			this.frame1.WidthRequest = 50;
			this.frame1.HeightRequest = 50;
			this.frame1.Name = "frame1";
			this.frame1.ShadowType = ((global::Gtk.ShadowType)(1));
			this.frame1.BorderWidth = ((uint)(15));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment5 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.GtkAlignment5.Name = "GtkAlignment5";
			this.GtkAlignment5.LeftPadding = ((uint)(12));
			// Container child GtkAlignment5.Gtk.Container+ContainerChild
			this.lineCoverageLabel = new global::Gtk.Label();
			this.lineCoverageLabel.Name = "lineCoverageLabel";
			this.lineCoverageLabel.LabelProp = global::Mono.Unix.Catalog.GetString("20%");
			this.lineCoverageLabel.Justify = ((global::Gtk.Justification)(2));
			this.GtkAlignment5.Add(this.lineCoverageLabel);
			this.frame1.Add(this.GtkAlignment5);
			this.GtkLabel6 = new global::Gtk.Label();
			this.GtkLabel6.Name = "GtkLabel6";
			this.GtkLabel6.LabelProp = global::Mono.Unix.Catalog.GetString("<b>Line</b>");
			this.GtkLabel6.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel6;
			this.hbox1.Add(this.frame1);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.frame1]));
			w12.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.frame3 = new global::Gtk.Frame();
			this.frame3.Name = "frame3";
			this.frame3.ShadowType = ((global::Gtk.ShadowType)(1));
			this.frame3.BorderWidth = ((uint)(15));
			// Container child frame3.Gtk.Container+ContainerChild
			this.GtkAlignment6 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.GtkAlignment6.Name = "GtkAlignment6";
			this.GtkAlignment6.LeftPadding = ((uint)(12));
			// Container child GtkAlignment6.Gtk.Container+ContainerChild
			this.branchCoverageLabel = new global::Gtk.Label();
			this.branchCoverageLabel.Name = "branchCoverageLabel";
			this.branchCoverageLabel.LabelProp = global::Mono.Unix.Catalog.GetString("20%");
			this.GtkAlignment6.Add(this.branchCoverageLabel);
			this.frame3.Add(this.GtkAlignment6);
			this.GtkLabel7 = new global::Gtk.Label();
			this.GtkLabel7.Name = "GtkLabel7";
			this.GtkLabel7.LabelProp = global::Mono.Unix.Catalog.GetString("<b>Branch</b>");
			this.GtkLabel7.UseMarkup = true;
			this.frame3.LabelWidget = this.GtkLabel7;
			this.hbox1.Add(this.frame3);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.frame3]));
			w15.Position = 1;
			this.vbox4.Add(this.hbox1);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox4[this.hbox1]));
			w16.Position = 1;
			this.vbox1.Add(this.vbox4);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.vbox4]));
			w17.Position = 1;
			this.Add(this.vbox1);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.Hide();
			this.testProjectDropdown.Changed += new global::System.EventHandler(this.OnTestProjectSelectionChanged);
			this.gatherCoverageButton.Clicked += new global::System.EventHandler(this.OnGatherCoverageClicked);
			this.coveredProjectPreviouButton.Clicked += new global::System.EventHandler(this.OnPreviousCoverageResultClicked);
			this.coveredProjectNextButton.Clicked += new global::System.EventHandler(this.OnNextCoverageResultClicked);
		}
	}
}