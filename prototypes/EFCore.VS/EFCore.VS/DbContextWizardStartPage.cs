using Microsoft.EntityFrameworkCore.VisualStudio.Properties;
using Microsoft.VisualStudio.Data.Core;
using Microsoft.VisualStudio.Data.Services.RelationalObjectModel;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.WizardFramework;
using System.Linq;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

public partial class DbContextWizardStartPage : WizardPage
{
    public DbContextWizardStartPage(WizardForm wizard)
        : base(wizard)
    {
        InitializeComponent();
        _providerComboBox.Items.AddRange(ProviderRegistry.GetEntityFrameworkCoreProviders());
        Logo = Resources.WizardPageLogo;
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

        var selectedProvider = providerManager.Providers[dialog.SelectedProvider];
        _connectionTextBox.Text = dialog.SafeConnectionString;

        if (ProviderRegistry.TryGetEntityFrameworkCoreProvider(selectedProvider, out var providerName))
        {
            _providerComboBox.Text = providerName;
        }

        // TODO: Move to new page
        // TODO: Fail gracefully
        var connectionFactory = (IVsDataConnectionFactory)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataConnectionFactory));
        var connection = connectionFactory.CreateConnection(selectedProvider.Guid, _connectionTextBox.Text, encryptedString: false);
        var selector = (IVsDataMappedObjectSelector)connection.GetService(typeof(IVsDataMappedObjectSelector));
        var tables = selector.SelectMappedObjects<IVsDataTable>().Select(t => $"{t.Schema}.{t.Name}").ToList();
        //var views = selector.SelectMappedObjects<IVsDataView>();
    }
}
