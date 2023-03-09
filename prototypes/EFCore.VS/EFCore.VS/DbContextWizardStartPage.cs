using Microsoft.EntityFrameworkCore.VisualStudio.Properties;
using Microsoft.WizardFramework;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

public partial class DbContextWizardStartPage : WizardPage
{
    public DbContextWizardStartPage(WizardForm wizard)
        : base(wizard)
    {
        InitializeComponent();
        Logo = Resources.ConnectToDatabase;
    }
}
