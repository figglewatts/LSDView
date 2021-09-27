using System.Collections.Generic;
using System.Drawing.Imaging;
using libLSD.Formats;
using libLSD.Interfaces;
using LSDView.Graphics;

namespace LSDView.Controllers.Interface
{
    public interface IExportController
    {
        void ExportOriginal(IWriteable original, string filePath);
        void ExportImage(TIM tim, int clutIndex, string filePath, ImageFormat format);
        void ExportImages(TIX tix, string filePath, bool separate, ImageFormat format);
        void ExportTexture(ITexture2D tex, string filePath, ImageFormat format);
        void ExportMesh(IRenderable mesh, string filePath, MeshExportFormat format);
        void ExportMeshes(IEnumerable<IRenderable> meshes, string filePath, MeshExportFormat format);
    }
}
