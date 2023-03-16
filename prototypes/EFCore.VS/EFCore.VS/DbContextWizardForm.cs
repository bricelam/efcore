using System.Collections.Generic;
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

    public bool EmptySelected { get; set; }
    public string ConnectionString { get; set; }
    public Guid VsDataProvider { get; set; }
    public string Provider { get; set; }
    public bool Pluralize { get; set; }
    public bool DataAnnotations { get; set; }
    public bool DatabaseNames { get; set; }
    public ICollection<string> SelectedTables { get; } = new List<string>();

    public override void OnCancel()
    {
        _cancelled = true;

        base.OnCancel();
    }
}
