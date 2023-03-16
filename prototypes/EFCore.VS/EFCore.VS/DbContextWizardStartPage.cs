using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.WizardFramework;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

public partial class DbContextWizardStartPage : WizardPage
{
    public DbContextWizardStartPage(DbContextWizardForm wizard)
        : base(wizard)
    {
        Wizard = wizard;
        InitializeComponent();
        Logo = KnownMonikers.EntityDatabase.ToBitmap(64);
    }

    protected new DbContextWizardForm Wizard { get; }

    public override bool OnDeactivate()
    {
        // TODO: Skip remaining pages when empty
        Wizard.EmptySelected = _emptyRadioButton.Checked;

        return base.OnDeactivate();
    }
}
