using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

internal class DbContextWizard : IWizard
{
    // Called as the template file begins to be processed.
    public void RunStarted(
        object automationObject,
        Dictionary<string, string> replacementsDictionary,
        WizardRunKind runKind,
        object[] customParams)
    {
        Debug.Assert(runKind == WizardRunKind.AsNewItem);

        //var dte = (DTE)automationObject;

        using var form = new DbContextWizardForm();
        form.Start();
        if (form.Cancelled)
        {
            throw new WizardCancelledException();
        }

        var builder = new StringBuilder();

        builder
            .Append("dotnet ef dbcontext scaffold \"")
            .Append(form.ConnectionString) // TODO: Escape
            .Append("\" ")
            .Append(form.Provider);

        if (form.DataAnnotations)
        {
            builder.Append(" --data-annotations");
        }

        builder
            .Append(" --context ")
            // TODO: Strip .cs
            .Append(replacementsDictionary["$rootname$"]) // TODO: safeitemname?
            .Append(" --output-dir ")
            // TODO: Strip root namespace, convert to path
            .Append(replacementsDictionary["$defaultnamespace$"]);

        foreach (var table in form.SelectedTables)
        {
            builder
                .Append(" --table ")
                .Append(table); // TODO: Quote
        }

        if (form.DatabaseNames)
        {
            builder.Append(" --use-database-names");
        }

        if (!form.Pluralize)
        {
            builder.Append(" --no-pluralize");
        }

        builder
            .Append(" --project ")
            .Append("TODO")
            .Append(" --startup-project ")
            .Append("TODO")
            .Append(" --framework ")
            .Append(replacementsDictionary["$targetframeworkidentifier$"])
            .Append(replacementsDictionary["$targetframeworkversion$"])
            .Append(" --configuration ")
            .Append("TODO")
            .Append(" --runtime ")
            .Append("TODO");

        replacementsDictionary.Add("$dotnetefcommand$", builder.ToString());

        // TODO: Compose over VsTemplateWizard instead?
        //var componentModel = (IComponentModel)ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel));
        //var packageInstaller = componentModel.GetService<IVsPackageInstaller2>();
        //packageInstaller.InstallLatestPackage
    }

    public bool ShouldAddProjectItem(string filePath)
        => true;

    // Called when a project item has finished being generated.
    public void ProjectItemFinishedGenerating(ProjectItem projectItem)
    {
    }

    // Called before a project item is opened within the editor.
    public void BeforeOpeningFile(ProjectItem projectItem)
    {
    }

    public void ProjectFinishedGenerating(EnvDTE.Project project)
        => throw new NotImplementedException();

    // Called when the wizard has finished generating outputs.
    public void RunFinished()
    {
    }
}
