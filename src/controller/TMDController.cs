using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libLSD.Exceptions;
using libLSD.Formats;
using libLSD.Types;
using LSDView.graphics;
using LSDView.util;
using LSDView.view;
using OpenTK;

namespace LSDView.controller
{
    public class TMDController
    {
        private Shader _shader;
        private TMD _tmd;

        public string TMDPath;
        public List<Mesh> Meshes;
        public ILSDView View { get; set; }

        public TMDController(ILSDView view)
        {
            Meshes = new List<Mesh>();

            View = view;

            View.OnGLLoad += (sender, args) =>
            {
                _shader = new Shader("basic", "shaders/basic");
            };
        }

        public void LoadTMD(string path)
        {
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                _tmd = new TMD(br);
                // TODO: check if TMD loaded correctly
            }

            TMDPath = path;

            foreach (Mesh m in Meshes)
            {
                m.Dispose();
            }
            Meshes.Clear();

            Logger.Log()(LogLevel.INFO, "Loaded TMD: {0}", path);

            Meshes = createMeshes(_tmd);

            TreeNode tmdNode = new TMDTreeNode(Path.GetFileName(TMDPath));

            View.ViewOutline.BeginUpdate();
            View.ViewOutline.Nodes.Clear();

            int i = 0;
            foreach (var m in Meshes)
            {
                tmdNode.Nodes.Add(new TMDObjectTreeNode("Object " + i.ToString(), m));
                i++;
            }

            View.ViewOutline.Nodes.Add(tmdNode);
            View.ViewOutline.EndUpdate();
            View.ViewOutline.SelectedNode = tmdNode;
        }

        private List<Mesh> createMeshes(TMD tmd)
        {
            List<Mesh> meshList = new List<Mesh>();

            foreach (var obj in tmd.ObjectTable)
            {
                meshList.Add(meshFromObject(obj));
            }
            
            return meshList;
        }

        private Mesh meshFromObject(TMDObject obj)
        {
            Vec3[] verts = new Vec3[obj.NumVertices];
            List<Vertex> vertList = new List<Vertex>();
            List<int> indices = new List<int>();

            for (int i = 0; i < obj.NumVertices; i++)
            {
                verts[i] = obj.Vertices[i] / 500f;
            }
                        
            foreach (var prim in obj.Primitives)
            {
                if (prim.Type != TMDPrimitivePacket.Types.POLYGON)
                    continue;
                            
                IPrimitivePacket primitivePacket = prim.PacketData;
                ITexturedPrimitivePacket texPrimitivePacket = prim.PacketData as ITexturedPrimitivePacket;
                IColoredPrimitivePacket colPrimitivePacket = prim.PacketData as IColoredPrimitivePacket;
                ILitPrimitivePacket litPrimitivePacket = prim.PacketData as ILitPrimitivePacket;

                List<int> polyIndices = new List<int>();
                int[] packetIndices = new int[primitivePacket.Vertices.Length];
                for (int i = 0; i < primitivePacket.Vertices.Length; i++)
                {
                    int vertIndex = primitivePacket.Vertices[i];
                    packetIndices[i] = vertList.Count;

                    Vector3 vertPos = new Vector3(verts[vertIndex].X, verts[vertIndex].Y, verts[vertIndex].Z);
                    Vector4 vertCol = Vector4.One;
                    Vector3 vertNorm = Vector3.Zero;
                    Vector2 vertUV = Vector2.Zero;

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
                        int uvIndex = i * 2;
                        vertUV = new Vector2(texPrimitivePacket.UVs[uvIndex],
                            texPrimitivePacket.UVs[uvIndex + 1]);
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

            return new Mesh(vertList.ToArray(), indices.ToArray(), _shader);
        }
    }
}
