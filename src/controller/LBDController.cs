using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libLSD.Formats;
using LSDView.anim;
using LSDView.graphics;
using LSDView.model;
using LSDView.util;
using LSDView.view;

namespace LSDView.controller
{
	public class LBDController
	{
		public ILSDView View { get; set; }
		public string LBDPath { get; private set; }

		private LBD _lbdFile;
		private List<Mesh> _tileMeshes;
		private List<Mesh> _tileLayout;
	    private List<MOMData> _moms;
		private VRAMController _vramController;
		private Shader _shader;

		public LBDController(ILSDView view, VRAMController vramController)
		{
			View = view;
			_vramController = vramController;
			_tileMeshes = new List<Mesh>();
			_tileLayout = new List<Mesh>();
			_moms = new List<MOMData>();

			View.OnGLLoad += (sender, args) =>
			{
				_shader = new Shader("basic", "shaders/basic");
			};
		}

		public void LoadLBD(string path)
		{
			LBDPath = path;

			using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
			{
				_lbdFile = new LBD(br);
			}

			Logger.Log()(LogLevel.INFO, "Loaded LBD: {0}", path);

			foreach (var tile in _tileMeshes)
			{
				tile.Dispose();
			}
			_tileMeshes.Clear();

			foreach (var tile in _tileLayout)
			{
				tile.Dispose();
			}
			_tileLayout.Clear();

			foreach (var mom in _moms)
			{
			    mom.Dispose();
			}
		    _moms.Clear();

			_tileMeshes = LibLSDUtil.CreateMeshesFromTMD(_lbdFile.Tiles, _shader, _vramController.VRAMTexture);

			int tileNo = 0;
			foreach (LBDTile tile in _lbdFile.TileLayout)
			{
				int x = tileNo / _lbdFile.Header.TileWidth;
				int y = tileNo % _lbdFile.Header.TileWidth;

				if (tile.DrawTile)
				{
					_tileLayout.AddRange(LibLSDUtil.CreateLBDTileMesh(tile, _lbdFile.ExtraTiles, x, y, _lbdFile.Tiles, _shader,
						_vramController.VRAMTexture));
				}

				tileNo++;
			}

			if (_lbdFile.Header.HasMML)
			{
				foreach (MOM mom in _lbdFile.MML?.MOMs)
				{
				    List<Mesh> momTmd = LibLSDUtil.CreateMeshesFromTMD(mom.TMD, _shader, _vramController.VRAMTexture);
                    List<TODAnimation> momAnimations = new List<TODAnimation>();
				    foreach (var anim in mom.MOS.TODs)
				    {
				        List<Mesh> animatedMeshes = LibLSDUtil.CreateMeshesFromTMD(mom.TMD, _shader, _vramController.VRAMTexture);
                        TODAnimation animationObj = new TODAnimation(animatedMeshes, anim);
                        momAnimations.Add(animationObj);
				    }
                    MOMData momData = new MOMData(momTmd, momAnimations);
                    _moms.Add(momData);
				}
			}

			TreeNode lbdNode = new RenderableMeshLayoutTreeNode(Path.GetFileName(LBDPath), _tileLayout.ToArray());

			TreeNode tilesTmdNode = new RenderableMeshListTreeNode("Tiles TMD");

			View.ViewOutline.BeginUpdate();
			View.ViewOutline.Nodes.Clear();

			int i = 0;
			foreach (var m in _tileMeshes)
			{
				tilesTmdNode.Nodes.Add(new RenderableMeshTreeNode("Object " + i.ToString(), m));
				i++;
			}

			i = 0;
			foreach (var mom in _moms)
			{
				RenderableMeshListTreeNode momNode = new RenderableMeshListTreeNode("MOM " + i.ToString());
				RenderableMeshListTreeNode momTmdNode = new RenderableMeshListTreeNode("TMD");
				momNode.Nodes.Add(momTmdNode);

			    int j = 0;
			    foreach (var mesh in mom.MomTmd)
			    {
			        momTmdNode.Nodes.Add(new RenderableMeshTreeNode("Object " + j.ToString(), mesh));
			        j++;
                }

			    j = 0;
			    foreach (var anim in mom.Animations)
			    {
			        RenderableAnimationTreeNode animNode =
			            new RenderableAnimationTreeNode(View.AnimPlayer, anim, "TOD " + j.ToString());
			        momNode.Nodes.Add(animNode);
			        j++;
			    }

                lbdNode.Nodes.Add(momNode);

				i++;
			}

			lbdNode.Nodes.Add(tilesTmdNode);

			View.ViewOutline.Nodes.Add(lbdNode);
			View.ViewOutline.EndUpdate();
			View.ViewOutline.SelectedNode = lbdNode;
		}
	}
}
