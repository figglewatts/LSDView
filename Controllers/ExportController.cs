using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using libLSD.Formats;
using libLSD.Interfaces;
using LSDView.Graphics;
using LSDView.GUI.GUIComponents;
using LSDView.util;
using LSDView.Util;

namespace LSDView.Controllers
{
    public class ExportController
    {
        private readonly FileDialog _fileExportDialog;

        public ExportController(FileDialog fileExportDialog, ConfigController configController)
        {
            _fileExportDialog = fileExportDialog;
            configController.Config.OnStreamingAssetsPathChange += () =>
                _fileExportDialog.InitialDir = configController.Config.StreamingAssetsPath;
        }

        public void OpenDialog(Action<string> onSubmit, string fileSaveType)
        {
            MainWindow.Instance.NextGuiRender += () =>
            {
                _fileExportDialog.Show(onSubmit, fileSaveType: fileSaveType);
            };
        }

        /// <summary>
        /// Export a LibLSD type as it was originally.
        /// </summary>
        /// <param name="original">The LibLSD type (MOM, TIX, TOD, etc).</param>
        /// <param name="filePath">The path to write the file to.</param>
        public void ExportOriginal(IWriteable original, string filePath)
        {
            Logger.Log()(LogLevel.INFO, $"Exporting original to: {filePath}");

            using (BinaryWriter bw = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                original.Write(bw);
            }
        }

        /// <summary>
        /// Export a TIM file to a common image format.
        /// </summary>
        /// <param name="tim">The TIM file.</param>
        /// <param name="filePath">The file path to export to.</param>
        /// <param name="format">The image format to export to.</param>
        public void ExportImage(TIM tim, string filePath, ImageFormat format)
        {
            Logger.Log()(LogLevel.INFO, $"Exporting image ({format}) to: {filePath}");

            var image = LibLSDUtil.GetImageDataFromTIM(tim, flip: false);
            Bitmap bmp = ImageUtil.ImageDataToBitmap(image.data, image.width, image.height);
            bmp.Save(filePath, format);
        }

        /// <summary>
        /// Export the TIM files in a TIX to common image formats.
        /// </summary>
        /// <param name="tix">The TIX file.</param>
        /// <param name="filePath">The file path to export to.</param>
        /// <param name="format">The image format to export to.</param>
        public void ExportImages(TIX tix, string filePath, ImageFormat format)
        {
            Logger.Log()(LogLevel.INFO, $"Exporting images ({format}) in TIX to: {filePath}");

            var allTims = tix.AllTIMs;
            for (int i = 0; i < allTims.Count; i++)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var ext = Path.GetExtension(filePath);
                var dir = Path.GetDirectoryName(filePath);
                ExportImage(allTims[i], Path.Combine(dir, $"{fileName}-{i}{ext}"), format);
            }
        }

        /// <summary>
        /// Export a renderable mesh to an OBJ file.
        /// </summary>
        /// <param name="mesh">The mesh to export.</param>
        /// <param name="filePath">The path to export the OBJ file to.</param>
        public void ExportOBJ(IRenderable mesh, string filePath)
        {
            Logger.Log()(LogLevel.INFO, $"Exporting mesh to: {filePath}");

            var objFile = MeshUtil.RenderableToObjFile(mesh);
            File.WriteAllText(filePath, objFile);
        }

        /// <summary>
        /// Export a list of renderable meshes to an OBJ file.
        /// </summary>
        /// <param name="meshes">The meshes to export.</param>
        /// <param name="filePath">The path to export the OBJ file to.</param>
        public void ExportOBJ(List<IRenderable> meshes, string filePath)
        {
            Logger.Log()(LogLevel.INFO, $"Exporting meshes to: {filePath}");

            var objFile = MeshUtil.RenderableListToObjFile(meshes);
            File.WriteAllText(filePath, objFile);
        }



        /// <summary>
        /// Export a renderable mesh to an PLY file.
        /// </summary>
        /// <param name="mesh">The mesh to export.</param>
        /// <param name="filePath">The path to export the PLY file to.</param>
        public void ExportPLY(IRenderable mesh, string filePath)
        {
            Logger.Log()(LogLevel.INFO, $"Exporting mesh to: {filePath}");

            var plyFile = MeshUtil.RenderableToPlyFile(mesh);
            File.WriteAllText(filePath, plyFile);
        }

        /// <summary>
        /// Export a list of renderable meshes to an PLY file.
        /// </summary>
        /// <param name="meshes">The meshes to export.</param>
        /// <param name="filePath">The path to export the PLY file to.</param>
        public void ExportPLY(List<IRenderable> meshes, string filePath)
        {
            Logger.Log()(LogLevel.INFO, $"Exporting meshes to: {filePath}");

            var plyFile = MeshUtil.RenderableListToPlyFile(meshes);
            File.WriteAllText(filePath, plyFile);
        }

        /// <summary>
        /// Export the VRAM texture to a file.
        /// </summary>
        /// <param name="vramTexture">The VRAM texture.</param>
        /// <param name="filePath">The path to export the VRAM to.</param>
        /// <param name="format">The format of the image file.</param>
        public void ExportVRAM(Texture2D vramTexture, string filePath, ImageFormat format)
        {
            float[] imageData = vramTexture.GetData();
            Bitmap bmp = ImageUtil.ImageDataToBitmap(imageData, vramTexture.Width, vramTexture.Height);
            bmp.Save(filePath, format);
        }
    }
}
