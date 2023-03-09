using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore.VisualStudio.Properties;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.RelationalObjectModel;
using Microsoft.WizardFramework;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

public partial class DbContextWizardTablesPage : WizardPage
{
    private readonly DbContextWizardForm _wizard;

    public DbContextWizardTablesPage(DbContextWizardForm wizard)
        : base(wizard)
    {
        _wizard = wizard;
        InitializeComponent();
        Logo = Resources.TableGroup;
    }

    protected new DbContextWizardForm Wizard
        => _wizard;

    public override bool OnActivate()
    {
        // TODO: Fail gracefully
        var connectionFactory = (IVsDataConnectionFactory)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataConnectionFactory));
        var connection = connectionFactory.CreateConnection(Wizard.VSProvider.Guid, Wizard.ConnectionString, encryptedString: false);
        var selector = (IVsDataMappedObjectSelector)connection.GetService(typeof(IVsDataMappedObjectSelector));

        _tablesTreeView.Nodes.Clear();

        var tables = selector.SelectMappedObjects<IVsDataTable>();
        _tablesTreeView.Nodes.Add(
            new TreeNode(
                "Tables",
                tables
                    .Select(t => t.Schema)
                    .Distinct()
                    .Select(
                        s => new TreeNode(
                            s,
                            tables
                                .Where(t => t.Schema == s)
                                .Select(t => new TreeNode(t.Name))
                                .ToArray()))
                    .ToArray()));

        var views = selector.SelectMappedObjects<IVsDataView>();
        _tablesTreeView.Nodes.Add(
            new TreeNode(
                "Views",
                views
                    .Select(v => v.Schema)
                    .Distinct()
                    .Select(
                        s => new TreeNode(
                            s,
                            views
                                .Where(v => v.Schema == s)
                                .Select(v => new TreeNode(v.Name))
                                .ToArray()))
                    .ToArray()));

        return base.OnActivate();
    }
}
