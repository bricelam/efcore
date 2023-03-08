using Microsoft.WizardFramework;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

public partial class DbContextWizardForm : WizardForm
{
    private bool _cancelled;

    public DbContextWizardForm()
    {
        InitializeComponent();
        // 1. Existing database or empty model?
        // 2. Connection and provider
        // 3. Version
        // 3. Tables and options
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
