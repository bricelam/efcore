using Microsoft.VisualStudio.Data.Core;
using Microsoft.WizardFramework;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

public partial class DbContextWizardForm : WizardForm
{
    private bool _cancelled;

    public DbContextWizardForm()
    {
        InitializeComponent();
        AddPage(new DbContextWizardStartPage(this));
        AddPage(new DbContextWizardConnectionPage(this));
        AddPage(new DbContextWizardTablesPage(this));
    }

    public bool Cancelled
        => _cancelled;

    public string ConnectionString { get; set; }
    public IVsDataProvider VSProvider { get; set; }
    public string EFProvider { get; set; }

    public override void OnCancel()
    {
        _cancelled = true;

        base.OnCancel();
    }
}
