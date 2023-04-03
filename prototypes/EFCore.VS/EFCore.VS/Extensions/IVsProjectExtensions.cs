using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop;

internal static class IVsProjectExtensions
{
    public static EnvDTE.Project ToProject(this IVsProject vsProject)
    {
        var vsHierarchy = (IVsHierarchy)vsProject;
        var hr = vsHierarchy.GetProperty(
            (uint)VSConstants.VSITEMID.Root,
            (int)__VSHPROPID.VSHPROPID_ExtObject,
            out var project);
        Marshal.ThrowExceptionForHR(hr);

        return (EnvDTE.Project)project;
    }
}
