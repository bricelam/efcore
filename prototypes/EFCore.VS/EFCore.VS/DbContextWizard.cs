using System.Collections.Generic;
using System.Diagnostics;
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
        ThreadHelper.ThrowIfNotOnUIThread();
        Debug.Assert(runKind == WizardRunKind.AsNewItem);

        var dte = (DTE)automationObject;

        // targetframeworkversion
        // targetframeworkidentifier
        // rootname
        // safeitemname
        // rootnamespace
        // defaultnamespace

        using var form = new DbContextWizardForm();
        form.Start();
        if (form.Cancelled)
        {
            throw new WizardCancelledException();
        }

        // TODO
        //form.EFProvider
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
