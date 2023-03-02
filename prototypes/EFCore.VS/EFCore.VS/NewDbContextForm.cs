using System.Windows.Forms;
using Microsoft.VisualStudio.Data.Core;
using Microsoft.VisualStudio.Data.Services;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

public partial class NewDbContextForm : Form
{
    private static readonly Guid _adoDotNetTechnology = new("77AB9A9D-78B9-4ba7-91AC-873F5338F1D2");
    private IVsDataProvider _selectedProvider;

    public NewDbContextForm()
    {
        // TODO: Set _providerComboBox.Items
        InitializeComponent();
    }

    private void _chooseButton_Click(object sender, EventArgs e)
    {
        var dialogFactory = (IVsDataConnectionDialogFactory)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataConnectionDialogFactory));
        var dialog = dialogFactory.CreateConnectionDialog();
        dialog.AddSources(_adoDotNetTechnology);
        if (!dialog.ShowDialog())
        {
            return;
        }

        var providerManager = (IVsDataProviderManager)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataProviderManager));
        _selectedProvider = providerManager.Providers[dialog.SelectedProvider];
        _connectionTextBox.Text = dialog.SafeConnectionString;

        var connectionType = _selectedProvider.GetConnectionType();
        if (connectionType is not null
            && ProviderRegistry.TryGetForConnectionType(connectionType, out var providerName))
        {
            _providerComboBox.Text = providerName;
        }
    }

    private void _providerComboBox_TextChanged(object sender, EventArgs e)
    {
        if (_selectedProvider is null)
            return;

        var connectionType = _selectedProvider.GetConnectionType();
        if (connectionType is not null
            && ProviderRegistry.TryGetForConnectionType(connectionType, out var providerName)
            && _providerComboBox.Text != providerName)
        {
            // TODO: Show error/warning
        }
    }
}
