using Microsoft.WizardFramework;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

public partial class DbContextWizardForm : WizardForm
{
    private bool _cancelled;

    public DbContextWizardForm()
    {
        InitializeComponent();
        AddPage(new DbContextWizardStartPage(this));
    }

    public bool Cancelled
        => _cancelled;

    public override void OnCancel()
    {
        _cancelled = true;

        base.OnCancel();
    }
}
