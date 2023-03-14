using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.WizardFramework;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

public partial class DbContextWizardStartPage : WizardPage
{
    public DbContextWizardStartPage(WizardForm wizard)
        : base(wizard)
    {
        InitializeComponent();
        Logo = KnownMonikers.EntityDatabase.ToBitmap(64);
    }
}
