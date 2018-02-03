using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using LSDView.anim;
using LSDView.graphics;
using LSDView.model;
using LSDView.util;
using LSDView.view;

namespace LSDView.controller
{
    public class MOMController
    {
        public ILSDView View { get; set; }

        private VRAMController _vramController;
        private string _momPath;
        private MOM _mom;
        private MOMData _momData;
        private Shader _shader;

        public MOMController(ILSDView view, VRAMController vram)
        {
            View = view;
            _vramController = vram;

            View.OnGLLoad += (sender, args) =>
            {
                _shader = new Shader("basic", "shaders/basic");
            };
        }

        public void LoadMOM(string path)
        {
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                _mom = new MOM(br);
            }

            _momPath = path;

            if (_momData != null)
            {
                _momData.Dispose();
                _momData = null;
            }

            List<Mesh> momTmd = LibLSDUtil.CreateMeshesFromTMD(_mom.TMD, _shader, _vramController.VRAMTexture);
            List<TODAnimation> momAnimations = new List<TODAnimation>();
            foreach (var anim in _mom.MOS.TODs)
            {
                List<Mesh> animatedMeshes = LibLSDUtil.CreateMeshesFromTMD(_mom.TMD, _shader, _vramController.VRAMTexture);
                TODAnimation animationObj = new TODAnimation(animatedMeshes, anim);
                momAnimations.Add(animationObj);
            }
            _momData = new MOMData(momTmd, momAnimations);

            View.ViewOutline.BeginUpdate();
            View.ViewOutline.Nodes.Clear();

            RenderableMeshLayoutTreeNode momNode = new RenderableMeshLayoutTreeNode(Path.GetFileName(_momPath), _momData.MomTmd.ToArray());

            RenderableMeshListTreeNode momTmdNode = new RenderableMeshListTreeNode("TMD");
            momNode.Nodes.Add(momTmdNode);

            int j = 0;
            foreach (var mesh in _momData.MomTmd)
            {
                momTmdNode.Nodes.Add(new RenderableMeshTreeNode("Object " + j.ToString(), mesh));
                j++;
            }

            j = 0;
            foreach (var anim in _momData.Animations)
            {
                RenderableAnimationTreeNode animNode =
                    new RenderableAnimationTreeNode(View.AnimPlayer, anim, "TOD " + j.ToString());
                momNode.Nodes.Add(animNode);
                j++;
            }

            View.ViewOutline.Nodes.Add(momNode);
            View.ViewOutline.EndUpdate();
            View.ViewOutline.SelectedNode = momNode;
        }
    }
}
