using System.Collections.Generic;
using libLSD.Formats;
using libLSD.Formats.Packets;
using libLSD.Types;
using LSDView.controller;
using LSDView.Graphics;
using OpenTK;

namespace LSDView.Util
{
    public static class LibLSDUtil
    {
        public static List<Mesh> CreateMeshesFromTMD(TMD tmd, Shader shader, Texture2D vram)
        {
            List<Mesh> meshList = new List<Mesh>();

            foreach (var obj in tmd.ObjectTable)
            {
                Mesh objMesh = MeshFromTMDObject(obj, shader);
                objMesh.Textures.Add(vram);
                meshList.Add(objMesh);
            }

            return meshList;
        }

        public static Mesh MeshFromTMDObject(TMDObject obj, Shader shader)
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

                        int texPageXPos = ((texPage % 16) * 128) - 640;
                        int texPageYPos = texPage < 16 ? 256 : 0;

                        int uvIndex = i * 2;
                        int vramXPos = texPageXPos + texPrimitivePacket.UVs[uvIndex];
                        int vramYPos = texPageYPos + (256 - texPrimitivePacket.UVs[uvIndex + 1]);

                        float uCoord = vramXPos / (float)VRAMController.VRAM_WIDTH;
                        float vCoord = vramYPos / (float)VRAMController.VRAM_HEIGHT;

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

            Mesh toReturn = new Mesh(vertList.ToArray(), indices.ToArray(), shader);
            return toReturn;
        }

        public static List<Mesh> CreateLBDTileMesh(LBDTile tile,
            LBDTile[] extraTiles,
            int x,
            int y,
            TMD tilesTmd,
            Shader shader,
            Texture2D vram)
        {
            List<Mesh> returnMeshList = new List<Mesh>();

            returnMeshList.Add(createSingleLBDTileMesh(tile, x, y, tilesTmd, shader, vram));

            LBDTile currentTile = tile;
            int i = 0;
            while (currentTile.ExtraTileIndex >= 0 && i <= 1)
            {
                LBDTile extraTile = extraTiles[currentTile.ExtraTileIndex];
                returnMeshList.Add(createSingleLBDTileMesh(extraTile, x, y, tilesTmd, shader, vram));
                currentTile = extraTile;
                i++;
            }

            return returnMeshList;
        }

        private static Mesh createSingleLBDTileMesh(LBDTile tile,
            int x,
            int y,
            TMD tilesTmd,
            Shader shader,
            Texture2D vram)
        {
            TMDObject tileObj = tilesTmd.ObjectTable[tile.TileType];
            Mesh tileMesh = MeshFromTMDObject(tileObj, shader);

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

        public static (float[] data, int width, int height) GetImageDataFromTIM(TIM tim, bool flip = true)
        {
            IColor[,] imageColors = tim.GetImage();
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
