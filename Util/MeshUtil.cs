using System.Collections.Generic;
using System.Linq;
using LSDView.Graphics;
using OpenTK;

namespace LSDView.Util
{
    public static class MeshUtil
    {
        public static string RenderableToObjFile(IRenderable mesh)
        {
            int vertCount = mesh.Verts.Vertices.Length;
            int triCount = mesh.Verts.Tris;

            ObjBuilder objString = new ObjBuilder();
            WriteOBJHeader(objString, vertCount, triCount);

            // fix OBJ export transformation
            Matrix4 rotateX180 = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-180));

            foreach (var vert in mesh.Verts.Vertices)
            {
                objString.Vertex(Vector3.TransformPosition(vert.Position, mesh.Transform.Matrix * rotateX180));
            }

            foreach (var vert in mesh.Verts.Vertices)
            {
                objString.Normal(vert.Normal);
            }

            foreach (var vert in mesh.Verts.Vertices)
            {
                objString.UV(vert.TexCoord);
            }

            ObjFaceBuilder faceBuilder = new ObjFaceBuilder();
            for (int i = 0; i < mesh.Verts.Indices.Length; i += 3)
            {
                // plus 1 here as OBJ is not zero-indexed
                int idx1 = mesh.Verts.Indices[i] + 1;
                int idx2 = mesh.Verts.Indices[i + 1] + 1;
                int idx3 = mesh.Verts.Indices[i + 2] + 1;
                faceBuilder.Vertex(idx1, idx1, idx1);
                faceBuilder.Vertex(idx2, idx2, idx2);
                faceBuilder.Vertex(idx3, idx3, idx3);
                objString.Face(faceBuilder.Build());
                faceBuilder.Clear();
            }

            return objString.ToString();
        }

        public static string RenderableListToObjFile(IEnumerable<IRenderable> meshes, bool combine = false)
        {
            int vertCount = 0;
            int triCount = 0;
            IEnumerable<IRenderable> renderables = meshes as IRenderable[] ?? meshes.ToArray();
            foreach (var mesh in renderables)
            {
                vertCount += mesh.Verts.Vertices.Length;
                triCount += mesh.Verts.Tris;
            }

            ObjBuilder objString = new ObjBuilder();
            WriteOBJHeader(objString, vertCount, triCount);

            List<Vector3> positions = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            // fix OBJ export transformation
            Matrix4 rotateX180 = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-180));

            foreach (var mesh in renderables)
            {
                foreach (var vert in mesh.Verts.Vertices)
                {
                    positions.Add(Vector3.TransformPosition(vert.Position, mesh.Transform.Matrix * rotateX180));
                    normals.Add(vert.Normal);
                    uvs.Add(vert.TexCoord);
                }
            }

            foreach (var pos in positions)
            {
                objString.Vertex(pos);
            }

            foreach (var norm in normals)
            {
                objString.Normal(norm);
            }

            foreach (var uv in uvs)
            {
                objString.UV(uv);
            }

            int faceBase = 0;
            int meshNumber = 0;
            foreach (var mesh in renderables)
            {
                // if we're not combining the meshes then we need to make groups for each
                if (!combine) objString.Group($"Mesh {meshNumber}");

                ObjFaceBuilder faceBuilder = new ObjFaceBuilder();
                for (int i = 0; i < mesh.Verts.Length; i += 3)
                {
                    int objIndex1 = faceBase + mesh.Verts.Indices[i] + 1;
                    int objIndex2 = faceBase + mesh.Verts.Indices[i + 1] + 1;
                    int objIndex3 = faceBase + mesh.Verts.Indices[i + 2] + 1;
                    faceBuilder.Vertex(objIndex1, objIndex1, objIndex1);
                    faceBuilder.Vertex(objIndex2, objIndex2, objIndex2);
                    faceBuilder.Vertex(objIndex3, objIndex3, objIndex3);
                    objString.Face(faceBuilder.Build());
                    faceBuilder.Clear();
                }

                meshNumber++;
                faceBase += mesh.Verts.Vertices.Length;
            }

            return objString.ToString();
        }


        public static string RenderableToPlyFile(IRenderable mesh)
        {
            int vertCount = mesh.Verts.Vertices.Length;
            int triCount = mesh.Verts.Tris;

            PlyBuilder plyString = new PlyBuilder();
            plyString.WriteHeader(vertCount, triCount);

            // Multply the transform matrix by -90 along x to Fix PLY Co-ord Space
            Matrix4 rotateX90 = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-90));

            // Vertex Elements
            foreach (var vert in mesh.Verts.Vertices)
            {
                plyString.BuildVertexElement(
                    Vector3.TransformPosition(vert.Position,
                        mesh.Transform.Matrix * rotateX90),
                    vert.Normal, vert.TexCoord, vert.Color);
            }

            // Face Indices
            for (var i = 0; i < mesh.Verts.Indices.Length; i += 3)
            {
                int idx1 = mesh.Verts.Indices[i];
                int idx2 = mesh.Verts.Indices[i + 1];
                int idx3 = mesh.Verts.Indices[i + 2];
                plyString.BuildFaceElement(idx1, idx2, idx3);
            }


            return plyString.ToString();
        }

        public static string RenderableListToPlyFile(IEnumerable<IRenderable> meshes)
        {
            int vertCount = 0;
            int triCount = 0;
            IEnumerable<IRenderable> renderables = meshes as IRenderable[] ?? meshes.ToArray();
            foreach (var mesh in renderables)
            {
                vertCount += mesh.Verts.Vertices.Length;
                triCount += mesh.Verts.Tris;
            }

            PlyBuilder plyString = new PlyBuilder();
            plyString.WriteHeader(vertCount, triCount);

            // Multply the transform matrix by -90 along x to Fix PLY Co-ord Space
            Matrix4 rotateX90 = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-90));

            // Vertex Elements
            foreach (var mesh in renderables)
            {
                foreach (var vert in mesh.Verts.Vertices)
                {
                    plyString.BuildVertexElement(
                        Vector3.TransformPosition(vert.Position,
                            mesh.Transform.Matrix * rotateX90),
                        vert.Normal, vert.TexCoord, vert.Color);
                }
            }

            // Face Indices
            int faceBase = 0;
            foreach (var mesh in renderables)
            {
                for (var i = 0; i < mesh.Verts.Indices.Length; i += 3)
                {
                    int idx1 = faceBase + mesh.Verts.Indices[i];
                    int idx2 = faceBase + mesh.Verts.Indices[i + 1];
                    int idx3 = faceBase + mesh.Verts.Indices[i + 2];
                    plyString.BuildFaceElement(idx1, idx2, idx3);
                }

                faceBase += mesh.Verts.Vertices.Length;
            }

            return plyString.ToString();
        }


        private static void WriteOBJHeader(ObjBuilder builder, int verts, int faces)
        {
            builder.Comment($"Generated by LSDView {Version.String}, created by Figglewatts, 2020");
            builder.Comment("https://github.com/Figglewatts/LSDView");
            builder.Comment($"Vertices: {verts}");
            builder.Comment($"Faces: {faces}");
        }
    }
}
