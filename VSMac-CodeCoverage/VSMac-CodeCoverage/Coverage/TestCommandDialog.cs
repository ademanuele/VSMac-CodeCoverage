namespace CodeCoverage.Coverage
{
  public partial class TestCommandDialog : Gtk.Dialog, ITestCommandDialog
  {
    TestCommandDialogPresenter presenter;

    public TestCommandDialog()
    {
      Build();
      presenter = new TestCommandDialogPresenter(this);
    }

    public void SetCommandField(string command)
      => expressionEntry.Text = command;

    public void SetPreview(string preview)
      => previewLabel.Text = preview;

    protected void OnCommandTextChanged(object sender, System.EventArgs e)
    {
      if (presenter == null) return;
      presenter.Command = expressionEntry.Text;
    }

    protected void OnCancelClicked(object sender, System.EventArgs e)
    {
      Destroy();
      Dispose();
    }

    protected void OnOkClicked(object sender, System.EventArgs e)
    {
      presenter.Save();
      Destroy();
      Dispose();
    }

    protected override void OnShown()
    {
      base.OnShown();
      expressionEntry.GrabFocus();
    }

    protected void expressionEntryKeyReleased(object o, Gtk.KeyReleaseEventArgs args)
    {
      switch(args.Event.Key) {
        case Gdk.Key.Return:
          OnOkClicked(this, null);
          break;

        case Gdk.Key.Escape:
          OnCancelClicked(this, null);
          break;
      }
    }
  }
}
