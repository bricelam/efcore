using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Imaging.Interop;

internal static class ImageMonikerExtensions
{
    public static Bitmap ToBitmap(this ImageMoniker moniker, int size)
    {
        var imageService = (IVsImageService2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsImageService));

        return (Bitmap)Microsoft.Internal.VisualStudio.PlatformUI.Utilities.GetObjectData(
            imageService.GetImage(
                moniker,
                new ImageAttributes
                {
                    StructSize = Marshal.SizeOf<ImageAttributes>(),
                    ImageType = (uint)_UIImageType.IT_Bitmap,
                    Format = (uint)_UIDataFormat.DF_WinForms,
                    LogicalWidth = size,
                    LogicalHeight = size,
                    Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags,
                }));
    }
}
