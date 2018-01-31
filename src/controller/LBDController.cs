using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libLSD.Formats;
using LSDView.graphics;
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
		private VRAMController _vramController;
		private Shader _shader;

		public LBDController(ILSDView view, VRAMController vramController)
		{
			View = view;
			_vramController = vramController;
			_tileMeshes = new List<Mesh>();
			_tileLayout = new List<Mesh>();

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

			Console.WriteLine($"Has MML?: {_lbdFile.Header.HasMML}");

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

			lbdNode.Nodes.Add(tilesTmdNode);

			View.ViewOutline.Nodes.Add(lbdNode);
			View.ViewOutline.EndUpdate();
			View.ViewOutline.SelectedNode = lbdNode;
		}
	}
}
