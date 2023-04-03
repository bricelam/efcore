using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TemplateWizard;
using NuGet.VisualStudio;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

internal class DbContextWizard : IWizard2
{
    private bool _emptyModel;

    // Called as the template file begins to be processed.
    public void RunStarted(
        object automationObject,
        Dictionary<string, string> replacementsDictionary,
        WizardRunKind runKind,
        object[] customParams,
        IVsProject vsProject,
        uint parentItemId)
    {
        //var dte = (DTE)automationObject;
        var project = vsProject.ToProject();
        Debug.Assert(runKind == WizardRunKind.AsNewItem);

        using var form = new DbContextWizardForm();
        form.Start();
        if (form.Cancelled)
        {
            throw new WizardCancelledException();
        }

        // TODO: Install dotnet-ef
        var componentModel = (IComponentModel)ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel));
        var packageInstaller = componentModel.GetService<IVsPackageInstaller2>();
        packageInstaller.InstallLatestPackage(
            source: null,
            project,
            form.Provider,
            includePrerelease: false,
            ignoreDependencies: false);

        _emptyModel = form.EmptySelected;
        if (_emptyModel)
        {
            return;
        }

        // TODO: Tools?
        // TODO: Match provider version
        packageInstaller.InstallLatestPackage(
            source: null,
            project,
            "Microsoft.EntityFrameworkCore.Design",
            includePrerelease: false,
            ignoreDependencies: false);

        // TODO: --startup-project, --framework, --runtime?
        var args = new List<string>
        {
            "ef",
            "dbcontext",
            "scaffold",
            form.ConnectionString,
            form.Provider,
            "--context",
            replacementsDictionary["$safeitemname$"],
            "--project",
            project.FullName,
            "--configuration",
            project.ConfigurationManager.ActiveConfiguration.ConfigurationName
        };

        if (form.DataAnnotations)
        {
            args.Add("--data-annotations");
        }

        var targetNamespace = replacementsDictionary["$rootnamespace$"];
        var rootNamespace = replacementsDictionary["$defaultnamespace$"];
        if (targetNamespace != rootNamespace)
        {
            var outputDir = targetNamespace.Substring(rootNamespace.Length + 1)
            .Replace('.', Path.DirectorySeparatorChar);

            args.Add("--output-dir");
            args.Add(outputDir);
        }

        foreach (var table in form.SelectedTables)
        {
            args.Add("--table");
            args.Add(table);
        }

        if (form.DatabaseNames)
        {
            args.Add("--use-database-names");
        }

        if (!form.Pluralize)
        {
            args.Add("--no-pluralize");
        }

        // TODO: Wait for packages to install
        var process = System.Diagnostics.Process.Start(
            new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = ToArguments(args),
                //CreateNoWindow = true,
                UseShellExecute = false,
                //RedirectStandardOutput = true,
                WorkingDirectory = (string)project.Properties.Item("FullPath").Value
            })!;
        //string? line;
        //while ((line = process.StandardOutput.ReadLine()) != null)
        //{
        //    // TODO: Surface to user
        //    Debug.WriteLine(line);
        //}

        //// TODO: Move to RunFinished?
        //process.WaitForExit();

        // TODO: Open context file
    }

    void IWizard.RunStarted(
        object automationObject,
        Dictionary<string, string> replacementsDictionary,
        WizardRunKind runKind,
        object[] customParams)
        => throw new NotImplementedException();

    public bool ShouldAddProjectItem(string filePath)
        => _emptyModel;

    // Called when a project item has finished being generated.
    public void ProjectItemFinishedGenerating(ProjectItem projectItem)
    {
    }

    // Called before a project item is opened within the editor.
    public void BeforeOpeningFile(ProjectItem projectItem)
    {
    }

    void IWizard.ProjectFinishedGenerating(EnvDTE.Project project)
        => throw new NotImplementedException();

    // Called when the wizard has finished generating outputs.
    public void RunFinished()
    {
    }

    private static string ToArguments(IReadOnlyList<string> args)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < args.Count; i++)
        {
            if (i != 0)
            {
                builder.Append(' ');
            }

            if (args[i].Length == 0)
            {
                builder.Append("\"\"");

                continue;
            }

            if (args[i].IndexOf(' ') == -1)
            {
                builder.Append(args[i]);

                continue;
            }

            builder.Append('"');

            var pendingBackslashes = 0;
            for (var j = 0; j < args[i].Length; j++)
            {
                switch (args[i][j])
                {
                    case '\"':
                        if (pendingBackslashes != 0)
                        {
                            builder.Append('\\', pendingBackslashes * 2);
                            pendingBackslashes = 0;
                        }

                        builder.Append("\\\"");
                        break;

                    case '\\':
                        pendingBackslashes++;
                        break;

                    default:
                        if (pendingBackslashes != 0)
                        {
                            if (pendingBackslashes == 1)
                            {
                                builder.Append('\\');
                            }
                            else
                            {
                                builder.Append('\\', pendingBackslashes * 2);
                            }

                            pendingBackslashes = 0;
                        }

                        builder.Append(args[i][j]);
                        break;
                }
            }

            if (pendingBackslashes != 0)
            {
                builder.Append('\\', pendingBackslashes * 2);
            }

            builder.Append('"');
        }

        return builder.ToString();
    }
}
