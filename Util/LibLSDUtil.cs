using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using libLSD.Formats.Packets;
using libLSD.Types;
using LSDView.Graphics;
using LSDView.Graphics.Headless;
using OpenTK;
using Serilog;

namespace LSDView.Util
{
    public static class LibLSDUtil
    {
        public const int VRAM_WIDTH = 2056;
        public const int VRAM_HEIGHT = 512;
        public const int TEX_PAGE_WIDTH = 128;
        public const int TEX_PAGE_HEIGHT = 256;
        public const int TEX_PAGE_PER_ROW = 16;
        public const int DOUBLE_BUFFER_WIDTH = TEX_PAGE_WIDTH * 5;

        public static LBD LoadLBD(string filePath)
        {
            Log.Information($"Loading LBD from: {filePath}");

            LBD lbd;
            using (BinaryReader br = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                lbd = new LBD(br);
            }

            Log.Information("Successfully loaded LBD");
            return lbd;
        }

        public static MOM LoadMOM(string filePath)
        {
            Log.Information($"Loading MOM from: {filePath}");

            MOM mom;
            using (BinaryReader br = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                mom = new MOM(br);
            }

            Log.Information("Successfully loaded MOM");
            return mom;
        }

        public static TIM LoadTIM(string filePath)
        {
            Log.Information($"Loading TIM from: {filePath}");

            TIM tim;
            using (BinaryReader br = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                tim = new TIM(br);
            }

            Log.Information("Successfully loaded TIM");
            return tim;
        }

        public static TIX LoadTIX(string filePath)
        {
            Log.Information($"Loading TIX from: {filePath}");

            TIX tix;
            using (BinaryReader br = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                tix = new TIX(br);
            }

            Log.Information("Successfully loaded TIX");
            return tix;
        }

        public static TMD LoadTMD(string filePath)
        {
            Log.Information($"Loading TMD from: {filePath}");

            TMD tmd;
            using (BinaryReader br = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                tmd = new TMD(br);
            }

            Log.Information("Successfully loaded TMD");
            return tmd;
        }

        public static List<IRenderable> CreateMeshesFromTMD(TMD tmd, Shader shader, ITexture2D vram, bool headless)
        {
            List<IRenderable> meshList = new List<IRenderable>();

            foreach (var obj in tmd.ObjectTable)
            {
                IRenderable objMesh = MeshFromTMDObject(obj, shader, headless);
                objMesh.Textures.Add(vram);
                meshList.Add(objMesh);
            }

            return meshList;
        }

        public static IRenderable MeshFromTMDObject(TMDObject obj, Shader shader, bool headless)
        {
            Vec3[] verts = new Vec3[obj.NumVertices];
            List<Vertex> vertList = new List<Vertex>();
            List<int> indices = new List<int>();

            for (int i = 0; i < obj.NumVertices; i++)
            {
                verts[i] = obj.Vertices[i] / 2048f;
            }

            foreach (var prim in obj.Primitives)
            {
                if (prim.Type != TMDPrimitivePacket.Types.POLYGON)
                    continue;

                ITMDPrimitivePacket primitivePacket = prim.PacketData;
                ITMDTexturedPrimitivePacket texPrimitivePacket = prim.PacketData as ITMDTexturedPrimitivePacket;
                ITMDColoredPrimitivePacket colPrimitivePacket = prim.PacketData as ITMDColoredPrimitivePacket;
                ITMDLitPrimitivePacket litPrimitivePacket = prim.PacketData as ITMDLitPrimitivePacket;

                List<int> polyIndices = new List<int>();
                int[] packetIndices = new int[primitivePacket.Vertices.Length];
                for (int i = 0; i < primitivePacket.Vertices.Length; i++)
                {
                    int vertIndex = primitivePacket.Vertices[i];
                    packetIndices[i] = vertList.Count;

                    Vector3 vertPos = new Vector3(verts[vertIndex].X, verts[vertIndex].Y, verts[vertIndex].Z);
                    Vector4 vertCol = Vector4.One;
                    Vector3 vertNorm = Vector3.Zero;
                    Vector2 vertUV = Vector2.One;

                    // handle packet colour
                    if (colPrimitivePacket != null)
                    {
                        Vec3 packetVertCol = colPrimitivePacket.Colors[colPrimitivePacket.Colors.Length > 1 ? i : 0];
                        vertCol = new Vector4(packetVertCol.X, packetVertCol.Y, packetVertCol.Z, 1f);
                    }

                    // handle packet normals
                    if (litPrimitivePacket != null)
                    {
                        TMDNormal packetVertNorm =
                            obj.Normals[
                                litPrimitivePacket.Normals[litPrimitivePacket.Normals.Length > 1 ? i : 0]];
                        vertNorm = new Vector3(packetVertNorm.X, packetVertNorm.Y,
                            packetVertNorm.Z);
                    }

                    // handle packet UVs
                    if (texPrimitivePacket != null)
                    {
                        int texPage = texPrimitivePacket.Texture.TexturePageNumber;

                        // the PSX VRAM is split into 32 texture pages, 16 on top and 16 on bottom
                        // a texture page is 128x256 pixels large
                        // pages 0-4 and 16-20 are used as a double buffer, with a width of 640 (5*128)

                        // the X position of the texture page is it's row index multiplied by the width
                        // we're also subtracting the width of the double buffer as we don't want to include
                        // it - we're using modern graphics so this will be on the video card!
                        int texPageXPos = ((texPage % TEX_PAGE_PER_ROW) * TEX_PAGE_WIDTH) - DOUBLE_BUFFER_WIDTH;

                        // more simply, the Y position of the texture page is 0 or the height of a texture page, based
                        // on if the texture page number is on the 2nd row of pages or not
                        int texPageYPos = texPage <= TEX_PAGE_PER_ROW ? TEX_PAGE_HEIGHT : 0;

                        int uvIndex = i * 2; // 2 UVs per vertex

                        // the UV information we get from the TMD model is UVs into a specific texture page, using
                        // only that texture page's coordinate system
                        // here, we're adding the texture page position offsets (into VRAM) to the TMD UVs, transforming
                        // them into the VRAM coordinate space
                        int vramXPos = texPageXPos + texPrimitivePacket.UVs[uvIndex];

                        // we're subtracting from the texture page height for the Y position as OpenGL uses flipped
                        // Y coordinates for textures compared with the PSX
                        int vramYPos = texPageYPos + (TEX_PAGE_HEIGHT - texPrimitivePacket.UVs[uvIndex + 1]);

                        // finally, we're normalizing the UVs from pixels
                        float uCoord = vramXPos / (float)VRAM_WIDTH;
                        float vCoord = vramYPos / (float)VRAM_HEIGHT;

                        vertUV = new Vector2(uCoord, vCoord);
                    }

                    vertList.Add(new Vertex(vertPos, vertNorm, vertUV, vertCol));
                }

                bool isQuad = (prim.Options & TMDPrimitivePacket.OptionsFlags.Quad) != 0;

                polyIndices.Add(packetIndices[1]);
                polyIndices.Add(packetIndices[0]);
                polyIndices.Add(packetIndices[2]);

                if (isQuad)
                {
                    polyIndices.Add(packetIndices[1]);
                    polyIndices.Add(packetIndices[2]);
                    polyIndices.Add(packetIndices[3]);
                }

                // if primitive is double sided poly we need to add other side with reverse winding
                if ((prim.Flags & TMDPrimitivePacket.PrimitiveFlags.DoubleSided) != 0)
                {
                    polyIndices.Add(packetIndices[0]);
                    polyIndices.Add(packetIndices[1]);
                    polyIndices.Add(packetIndices[2]);

                    if (isQuad)
                    {
                        polyIndices.Add(packetIndices[2]);
                        polyIndices.Add(packetIndices[1]);
                        polyIndices.Add(packetIndices[3]);
                    }
                }

                indices.AddRange(polyIndices);
            }

            return headless
                ? (IRenderable)new HeadlessMesh(vertList.ToArray(), indices.ToArray())
                : (IRenderable)new Mesh(vertList.ToArray(), indices.ToArray(), shader);
        }

        public static List<IRenderable> CreateLBDTileMesh(LBDTile tile,
            LBDTile[] extraTiles,
            int x,
            int y,
            TMD tilesTmd,
            Shader shader,
            ITexture2D vram,
            bool headless)
        {
            List<IRenderable> returnMeshList = new List<IRenderable>();
            returnMeshList.Add(createSingleLBDTileMesh(tile, x, y, tilesTmd, shader, vram, headless));

            LBDTile currentTile = tile;
            int i = 0;
            while (currentTile.ExtraTileIndex >= 0 && i <= 1)
            {
                LBDTile extraTile = extraTiles[currentTile.ExtraTileIndex];
                returnMeshList.Add(createSingleLBDTileMesh(extraTile, x, y, tilesTmd, shader, vram, headless));
                currentTile = extraTile;
                i++;
            }

            return returnMeshList;
        }

        private static IRenderable createSingleLBDTileMesh(LBDTile tile,
            int x,
            int y,
            TMD tilesTmd,
            Shader shader,
            ITexture2D vram,
            bool headless)
        {
            TMDObject tileObj = tilesTmd.ObjectTable[tile.TileType];
            IRenderable tileMesh = MeshFromTMDObject(tileObj, shader, headless);

            switch (tile.TileDirection)
            {
                case LBDTile.TileDirections.Deg90:
                {
                    tileMesh.Transform.Rotation =
                        Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(90));
                    break;
                }
                case LBDTile.TileDirections.Deg180:
                {
                    tileMesh.Transform.Rotation =
                        Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(180));
                    break;
                }
                case LBDTile.TileDirections.Deg270:
                {
                    tileMesh.Transform.Rotation =
                        Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(270));
                    break;
                }
            }

            tileMesh.Transform.Position = new Vector3(x, tile.TileHeight, y);

            tileMesh.Textures.Add(vram);

            return tileMesh;
        }

        public static ITexture2D TIXToTexture2D(TIX tix, bool headless, bool flip = true)
        {
            float[] texData = new float[VRAM_WIDTH * VRAM_HEIGHT * 4];
            ITexture2D tex = headless
                ? (ITexture2D)new HeadlessTexture2D(VRAM_WIDTH, VRAM_HEIGHT, texData)
                : (ITexture2D)new Texture2D(VRAM_WIDTH, VRAM_HEIGHT, texData);
            TIXToTexture2D(tix, ref tex, flip);
            return tex;
        }

        public static void TIXToTexture2D(TIX tix, ref ITexture2D tex, bool flip = true)
        {
            foreach (var chunk in tix.Chunks)
            {
                foreach (var tim in chunk.TIMs)
                {
                    var image = GetImageDataFromTIM(tim, flip: flip);

                    int actualXPos = (tim.PixelData.XPosition - 320) * 2;
                    int actualYPos = 512 - tim.PixelData.YPosition - image.height;
                    Log.Debug(
                        $"[{tim.PixelData.XPosition}, {tim.PixelData.YPosition}] -> ({actualXPos}, {actualYPos})");

                    tex.SubImage(image.data, actualXPos, actualYPos, image.width, image.height);
                }
            }
        }

        public static (float[] data, int width, int height) GetImageDataFromTIM(TIM tim,
            int clutIndex = 0,
            bool flip = true)
        {
            IColor[,] imageColors = tim.GetImage(clutIndex);
            int width = imageColors.GetLength(1);
            int height = imageColors.GetLength(0);
            float[] imageData = ImageColorsToData(imageColors, width, height, flip);
            return (imageData, width, height);
        }

        public static float[] ImageColorsToData(IColor[,] imageColors, int width, int height, bool flip = true)
        {
            float[] imageData = new float[imageColors.Length * 4];

            int i = 0;
            if (flip)
            {
                for (int y = height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < width; x++)
                    {
                        IColor col = imageColors[y, x];
                        SetImageColors(ref i, col, ref imageData);
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        IColor col = imageColors[y, x];
                        SetImageColors(ref i, col, ref imageData);
                    }
                }
            }

            return imageData;
        }

        private static void SetImageColors(ref int i, IColor col, ref float[] imageData)
        {
            imageData[i] = col.Red;
            imageData[i + 1] = col.Green;
            imageData[i + 2] = col.Blue;
            imageData[i + 3] = col.Alpha;
            i += 4;
        }
    }
}
