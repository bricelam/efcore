using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.RelationalObjectModel;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
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
        Logo = KnownMonikers.TableGroup.ToBitmap(64);
        _tablesTreeView.ImageList = new ImageList
        {
            Images =
            {
                /* [0] = */ KnownMonikers.DatabaseTableGroup.ToBitmap(16),
                /* [1] = */ KnownMonikers.DataSourceView.ToBitmap(16),
                /* [2] = */ KnownMonikers.DatabaseSchema.ToBitmap(16),
                /* [3] = */ KnownMonikers.Table.ToBitmap(16),
                /* [4] = */ KnownMonikers.View.ToBitmap(16)
            }
        };
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

        var tables = selector.SelectMappedObjects<IVsDataTable>()
            .Where(t => !t.IsSystemObject)
            .ToList();
        _tablesTreeView.Nodes.Add(
            new TreeNode(
                "Tables",
                0, 0,
                tables
                    .Select(t => t.Schema)
                    .Distinct()
                    .Select(
                        s => new TreeNode(
                            s,
                            2, 2,
                            tables
                                .Where(t => t.Schema == s)
                                .Select(t => new TreeNode(t.Name, 3, 3))
                                .ToArray()))
                    .ToArray()));

        var views = selector.SelectMappedObjects<IVsDataView>()
            .Where(v => !v.IsSystemObject)
            .ToList();
        _tablesTreeView.Nodes.Add(
            new TreeNode(
                "Views",
                1, 1,
                views
                    .Select(v => v.Schema)
                    .Distinct()
                    .Select(
                        s => new TreeNode(
                            s,
                            2, 2,
                            views
                                .Where(v => v.Schema == s)
                                .Select(v => new TreeNode(v.Name, 4, 4))
                                .ToArray()))
                    .ToArray()));

        return base.OnActivate();
    }
}
