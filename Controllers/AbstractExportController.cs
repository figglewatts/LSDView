using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using libLSD.Formats;
using libLSD.Interfaces;
using LSDView.Controllers.Interface;
using LSDView.Graphics;
using LSDView.Graphics.Headless;
using LSDView.Models;
using LSDView.Util;
using OpenTK;
using Serilog;

namespace LSDView.Controllers
{
    public abstract class AbstractExportController : IExportController
    {
        /// <summary>
        /// Export a LibLSD type as it was originally.
        /// </summary>
        /// <param name="original">The LibLSD type (MOM, TIX, TOD, etc).</param>
        /// <param name="filePath">The path to write the file to.</param>
        public void ExportOriginal(IWriteable original, string filePath)
        {
            Log.Information($"Exporting original to: {filePath}");

            using (BinaryWriter bw = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                original.Write(bw);
            }
        }

        /// <summary>
        /// Export a TIM file to a common image format.
        /// </summary>
        /// <param name="tim">The TIM file.</param>
        /// <param name="clutIndex">The index of the CLUT to export with.</param>
        /// <param name="filePath">The file path to export to.</param>
        /// <param name="format">The image format to export to.</param>
        public void ExportImage(TIM tim, int clutIndex, string filePath, ImageFormat format)
        {
            Log.Information($"Exporting image ({format}) to: {filePath}");

            var image = LibLSDUtil.GetImageDataFromTIM(tim, clutIndex, flip: false);
            Bitmap bmp = ImageUtil.ImageDataToBitmap(image.data, image.width, image.height);
            bmp.Save(filePath, format);
        }

        /// <summary>
        /// Export the TIM files in a TIX to common image formats.
        /// </summary>
        /// <param name="tix">The TIX file.</param>
        /// <param name="filePath">The file path to export to.</param>
        /// <param name="separate">Whether or not to separate images in the output.</param>
        /// <param name="format">The image format to export to.</param>
        public void ExportImages(TIX tix, string filePath, bool separate, ImageFormat format)
        {
            if (separate)
            {
                Log.Information($"Exporting images ({format}) in TIX to: {filePath}");

                var allTims = tix.AllTIMs;
                for (int i = 0; i < allTims.Count; i++)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var ext = Path.GetExtension(filePath);
                    var dir = Path.GetDirectoryName(filePath);
                    ExportImage(allTims[i], 0, Path.Combine(dir, $"{fileName}-{i}{ext}"), format);
                }
            }
            else
            {
                ITexture2D tixTex = LibLSDUtil.TIXToTexture2D(tix, headless: true, flip: false);
                ExportTexture(tixTex, filePath, format);
            }
        }

        public void ExportTexture(ITexture2D tex, string filePath, ImageFormat format)
        {
            float[] imageData = tex.GetData();
            Bitmap bmp = ImageUtil.ImageDataToBitmap(imageData, tex.Width, tex.Height);
            bmp.Save(filePath, format);
        }

        public void ExportMesh(IRenderable mesh, string filePath, MeshExportFormat format)
        {
            switch (format)
            {
                case MeshExportFormat.OBJ:
                    ExportOBJ(mesh, filePath);
                    break;
                case MeshExportFormat.PLY:
                    ExportPLY(mesh, filePath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public void ExportMeshes(IEnumerable<IRenderable> meshes, string filePath, MeshExportFormat format)
        {
            switch (format)
            {
                case MeshExportFormat.OBJ:
                    ExportOBJ(meshes, filePath);
                    break;
                case MeshExportFormat.PLY:
                    ExportPLY(meshes, filePath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        /// <summary>
        /// Export a renderable mesh to an OBJ file.
        /// </summary>
        /// <param name="mesh">The mesh to export.</param>
        /// <param name="filePath">The path to export the OBJ file to.</param>
        public void ExportOBJ(IRenderable mesh, string filePath)
        {
            Log.Information($"Exporting OBJ mesh to: {filePath}");

            if (String.IsNullOrEmpty(Path.GetExtension(filePath)))
            {
                filePath += ".obj";
            }

            var objFile = MeshUtil.RenderableToObjFile(mesh);
            File.WriteAllText(filePath, objFile);
        }

        /// <summary>
        /// Export a list of renderable meshes to an OBJ file.
        /// </summary>
        /// <param name="meshes">The meshes to export.</param>
        /// <param name="filePath">The path to export the OBJ file to.</param>
        public void ExportOBJ(IEnumerable<IRenderable> meshes, string filePath)
        {
            Log.Information($"Exporting OBJ meshes to: {filePath}");

            if (String.IsNullOrEmpty(Path.GetExtension(filePath)))
            {
                filePath += ".obj";
            }

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
            Log.Information($"Exporting PLY mesh to: {filePath}");

            if (String.IsNullOrEmpty(Path.GetExtension(filePath)))
            {
                filePath += ".ply";
            }

            var plyFile = MeshUtil.RenderableToPlyFile(mesh);
            File.WriteAllText(filePath, plyFile);
        }

        /// <summary>
        /// Export a list of renderable meshes to an PLY file.
        /// </summary>
        /// <param name="meshes">The meshes to export.</param>
        /// <param name="filePath">The path to export the PLY file to.</param>
        public void ExportPLY(IEnumerable<IRenderable> meshes, string filePath)
        {
            Log.Information($"Exporting PLY meshes to: {filePath}");

            if (String.IsNullOrEmpty(Path.GetExtension(filePath)))
            {
                filePath += ".ply";
            }

            var plyFile = MeshUtil.RenderableListToPlyFile(meshes);
            File.WriteAllText(filePath, plyFile);
        }

        /// <summary>
        /// Export an entire dream to a 3D format with textures.
        /// </summary>
        /// <param name="dream">The dream we're exporting.</param>
        /// <param name="combineChunks">Whether or not chunks should be combined into a single mesh.</param>
        /// <param name="exportFormat">The mesh format to export the level mesh in.</param>
        /// <param name="exportDirectory">The directory we're exporting meshes to.</param>
        public void ExportDream(Dream dream, bool combineChunks, MeshExportFormat exportFormat, string exportDirectory)
        {
            Log.Information($"Exporting dream to: {exportDirectory}");

            Log.Information($"Dream width: {dream.LevelWidth}");

            // create the directory
            Directory.CreateDirectory(exportDirectory);

            // combine tilelayouts of each LBDDocument into single renderables
            List<IRenderable> chunkRenderables = new List<IRenderable>();
            for (int i = 0; i < dream.Chunks.Count; i++)
            {
                Log.Information($"Processing dream chunk {i + 1}/{dream.Chunks.Count}...");

                var lbdChunk = dream.Chunks[i];
                var chunkRenderable = HeadlessMesh.CombineMeshes(lbdChunk.TileLayout.ToArray());

                // position the LBD chunk based on tiling (if we're tiling)
                if (dream.LevelWidth > 0)
                {
                    int xPos = i % dream.LevelWidth;
                    int yPos = i / dream.LevelWidth;

                    // handle staggered tiling, every other row
                    int xMod = 0;
                    if (yPos % 2 == 1)
                    {
                        xMod = Dream.CHUNK_DIMENSION / 2; // stagger by half the width of a chunk
                    }

                    chunkRenderable.Transform.Position = new Vector3(xPos * Dream.CHUNK_DIMENSION - xMod, 0,
                        yPos * Dream.CHUNK_DIMENSION);
                }

                chunkRenderables.Add(chunkRenderable);
            }

            string pathToLevel = Path.Combine(exportDirectory, "dream");
            if (combineChunks)
            {
                // if we're combining chunks, we now need to combine everything into a single renderable
                IRenderable dreamRenderable = HeadlessMesh.CombineMeshes(chunkRenderables.ToArray());
                ExportMesh(dreamRenderable, pathToLevel, exportFormat);
            }
            else
            {
                // otherwise we can export the list of chunk renderables
                ExportMeshes(chunkRenderables, pathToLevel, exportFormat);
            }

            // now, export each TIXDocument texture set as a combined VRAM export
            string pathToTextureSet = Path.Combine(exportDirectory, "textureset");
            string[] textureSetNames = { "-normal.png", "-kanji.png", "-downer.png", "-upper.png" };
            for (int i = 0; i < 4; i++)
            {
                Log.Information($"Exporting texture set {i + 1}/4...");
                var textureSetTix = dream.TextureSets[i].Document;
                string exportPath = pathToTextureSet + textureSetNames[i];
                ExportImages(textureSetTix, exportPath, separate: false, ImageFormat.Png);
            }

            Log.Information("Dream export complete");
        }
    }
}
