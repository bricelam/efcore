using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.RelationalObjectModel;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.WizardFramework;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

public partial class DbContextWizardTablesPage : WizardPage
{
    public DbContextWizardTablesPage(DbContextWizardForm wizard)
        : base(wizard)
    {
        Wizard = wizard;
        InitializeComponent();
        Logo = KnownMonikers.TableGroup.ToBitmap(64);
        // TODO: Handle parent-child checking behaviors
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

    protected new DbContextWizardForm Wizard { get; }

    public override void OnActivated()
    {
        // TODO: Fail gracefully
        var connectionFactory = (IVsDataConnectionFactory)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataConnectionFactory));
        var connection = connectionFactory.CreateConnection(Wizard.VsDataProvider, Wizard.ConnectionString, encryptedString: false);

        var info = (IVsDataSourceInformation)connection.GetService(typeof(IVsDataSourceInformation));
        var defaultCatalog = info["DefaultCatalog"];

        var selector = (IVsDataMappedObjectSelector)connection.GetService(typeof(IVsDataMappedObjectSelector));
        var restrictions = new object[] { defaultCatalog, /* schema: */ null, /* name: */ null };

        _tablesTreeView.Nodes.Clear();

        // TODO: Handle empty schema better
        var tables = selector.SelectMappedObjects<IVsDataTable>(restrictions)
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
                            s ?? "(default schema)",
                            2, 2,
                            tables
                                .Where(t => t.Schema == s)
                                .Select(t => new TreeNode(t.Name, 3, 3) { Tag = t, Checked = true })
                                .ToArray())
                        { Checked = true })
                    .ToArray())
            { Checked = true });

        var views = selector.SelectMappedObjects<IVsDataView>(restrictions)
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
                            s ?? "(default schema)",
                            2, 2,
                            views
                                .Where(v => v.Schema == s)
                                .Select(v => new TreeNode(v.Name, 4, 4) { Tag = v, Checked = true })
                                .ToArray())
                        { Checked = true })
                    .ToArray())
            { Checked = true });

        base.OnActivated();
    }

    public override bool OnDeactivate()
    {
        // TODO: Empty when all checked; Add SelectedSchemas too
        Wizard.SelectedTables.Clear();

        var nodes = new Queue<TreeNode>(_tablesTreeView.Nodes.Cast<TreeNode>());
        while (nodes.Count > 0)
        {
            var node = nodes.Dequeue();
            foreach (TreeNode child in node.Nodes)
            {
                nodes.Enqueue(child);
            }

            if (node.Checked
                && node.Tag is IVsDataTabularObject tableOrView)
            {
                Wizard.SelectedTables.Add(
                    !string.IsNullOrEmpty(tableOrView.Schema)
                        ? tableOrView.Schema + "." + tableOrView.Name
                        : tableOrView.Name);
            }
        }

        return base.OnDeactivate();
    }
}
