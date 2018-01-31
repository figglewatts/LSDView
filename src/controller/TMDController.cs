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
        public string TMDPath { get; private set; }
        public List<Mesh> Meshes;
        public ILSDView View { get; set; }

        private Shader _shader;
        private TMD _tmd;
        private VRAMController _vramController;

        public TMDController(ILSDView view, VRAMController vramController)
        {
            Meshes = new List<Mesh>();

            _vramController = vramController;

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

            Meshes = LibLSDUtil.CreateMeshesFromTMD(_tmd, _shader, _vramController.VRAMTexture);

            TreeNode tmdNode = new RenderableMeshListTreeNode(Path.GetFileName(TMDPath));

            View.ViewOutline.BeginUpdate();
            View.ViewOutline.Nodes.Clear();

            int i = 0;
            foreach (var m in Meshes)
            {
                tmdNode.Nodes.Add(new RenderableMeshTreeNode("Object " + i.ToString(), m));
                i++;
            }

            View.ViewOutline.Nodes.Add(tmdNode);
            View.ViewOutline.EndUpdate();
            View.ViewOutline.SelectedNode = tmdNode;
        }
    }
}
