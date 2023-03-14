using Microsoft.VisualStudio.Data.Core;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.WizardFramework;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

public partial class DbContextWizardConnectionPage : WizardPage
{
    private readonly DbContextWizardForm _wizard;

    public DbContextWizardConnectionPage(DbContextWizardForm wizard)
        : base(wizard)
    {
        _wizard = wizard;
        InitializeComponent();
        Logo = KnownMonikers.ConnectToDatabase.ToBitmap(64);
        _providerComboBox.Items.AddRange(ProviderRegistry.GetEntityFrameworkCoreProviders());
    }

    protected new DbContextWizardForm Wizard
        => _wizard;

    public override bool OnDeactivate()
    {
        Wizard.ConnectionString = _connectionTextBox.Text;
        Wizard.EFProvider = _providerComboBox.Text;

        return base.OnDeactivate();
    }

    private void _chooseButton_Click(object sender, EventArgs e)
    {
        var dialogFactory = (IVsDataConnectionDialogFactory)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataConnectionDialogFactory));
        var providerManager = (IVsDataProviderManager)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataProviderManager));

        var dialog = dialogFactory.CreateConnectionDialog();
        // TODO: Review
        dialog.ChooseSourceHeaderLabel = "Choose wisely. If your database isn't listed, Cancel and manually enter you connection string.";
        dialog.SaveSelection = false;
        dialog.ChangeSourceHeaderLabel = dialog.ChooseSourceHeaderLabel;
        dialog.AddSources((source, provider) => ProviderRegistry.TryGetEntityFrameworkCoreProvider(providerManager.Providers[provider], out _));
        if (!dialog.ShowDialog())
        {
            return;
        }

        // TODO: Dynamically add Tables page here?
        Wizard.VSProvider = providerManager.Providers[dialog.SelectedProvider];
        _connectionTextBox.Text = dialog.SafeConnectionString;

        if (ProviderRegistry.TryGetEntityFrameworkCoreProvider(Wizard.VSProvider, out var providerName))
        {
            _providerComboBox.Text = providerName;
        }
    }
}
