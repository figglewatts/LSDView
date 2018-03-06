using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using LSDView.graphics;
using LSDView.util;

namespace LSDView.controller
{
    public class MeshController
    {
        public void WriteMeshAsOBJ(string path, IRenderable mesh)
        {
            File.WriteAllText(path, MeshUtil.RenderableToObjFile(mesh));
        }

        public void WriteMeshesAsOBJ(string path, List<IRenderable> meshes)
        {
            File.WriteAllText(path, MeshUtil.RenderableListToObjFile(meshes));
        }

        public void WriteTMDAsOBJ(string path, TMD tmd)
        {
            File.WriteAllText(path, MeshUtil.TMDToOBJFile(tmd));
        }
    }
}
