using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.WizardFramework;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

public partial class DbContextWizardConnectionPage : WizardPage
{
    public DbContextWizardConnectionPage(DbContextWizardForm wizard)
        : base(wizard)
    {
        Wizard = wizard;
        InitializeComponent();
        Logo = KnownMonikers.ConnectToDatabase.ToBitmap(64);
        _providerComboBox.Items.AddRange(ProviderRegistry.GetEntityFrameworkCoreProviders());
    }

    protected new DbContextWizardForm Wizard { get; }

    public override bool OnDeactivate()
    {
        // TODO: Surface --no-onconfiguring?
        Wizard.ConnectionString = _connectionTextBox.Text;
        Wizard.Provider = _providerComboBox.Text;
        Wizard.Pluralize = _pluralizeCheckBox.Checked;
        Wizard.DataAnnotations = _dataAnnotationsCheckBox.Checked;
        Wizard.DatabaseNames = _databaseNamesCheckBox.Checked;

        return base.OnDeactivate();
    }

    private void _chooseButton_Click(object sender, EventArgs e)
    {
        var dialogFactory = (IVsDataConnectionDialogFactory)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataConnectionDialogFactory));

        var dialog = dialogFactory.CreateConnectionDialog();
        // TODO: Review
        dialog.ChooseSourceHeaderLabel = "Choose wisely. If your database isn't listed, Cancel and manually enter you connection string.";
        dialog.SaveSelection = false;
        dialog.ChangeSourceHeaderLabel = dialog.ChooseSourceHeaderLabel;
        dialog.AddSources((source, provider) => ProviderRegistry.TryGetEntityFrameworkCoreProvider(provider, out _));
        if (!dialog.ShowDialog())
        {
            // TODO: Clear VS provider?
            return;
        }

        // TODO: Skip select tables page when no provider
        Wizard.VsDataProvider = dialog.SelectedProvider;
        _connectionTextBox.Text = dialog.SafeConnectionString;

        if (ProviderRegistry.TryGetEntityFrameworkCoreProvider(dialog.SelectedProvider, out var providerName))
        {
            _providerComboBox.Text = providerName;
        }
    }
}
