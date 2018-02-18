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
        public List<Mesh> TileMeshes { get; private set; }
        public List<Mesh> TileLayout { get; private set; }
        public List<MOMData> Moms { get; private set; }

		private LBD _lbdFile;
		private VRAMController _vramController;
		private Shader _shader;
	    private DocumentController _documentController;

		public LBDController(ILSDView view, VRAMController vramController, DocumentController documentController)
		{
			View = view;
			_vramController = vramController;
			TileMeshes = new List<Mesh>();
			TileLayout = new List<Mesh>();
			Moms = new List<MOMData>();
		    _documentController = documentController;

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

			LBDDocument document = new LBDDocument(_lbdFile);
		    document.OnLoad += (sender, args) => CreateMeshes();
		    document.OnUnload += (sender, args) => UnloadLBD();
            _documentController.LoadDocument(document, Path.GetFileName(LBDPath));
		}

	    private void CreateMeshes()
	    {
	        TileMeshes = LibLSDUtil.CreateMeshesFromTMD(_lbdFile.Tiles, _shader, _vramController.VRAMTexture);

	        int tileNo = 0;
	        foreach (LBDTile tile in _lbdFile.TileLayout)
	        {
	            int x = tileNo / _lbdFile.Header.TileWidth;
	            int y = tileNo % _lbdFile.Header.TileWidth;

	            if (tile.DrawTile)
	            {
	                TileLayout.AddRange(LibLSDUtil.CreateLBDTileMesh(tile, _lbdFile.ExtraTiles, x, y, _lbdFile.Tiles, _shader,
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
	                MOMData momData = new MOMData(mom, momTmd, momAnimations);
	                Moms.Add(momData);
	            }
	        }
        }

	    private void UnloadLBD()
	    {
	        foreach (var tile in TileMeshes)
	        {
	            tile.Dispose();
	        }
	        TileMeshes.Clear();

	        foreach (var tile in TileLayout)
	        {
	            tile.Dispose();
	        }
	        TileLayout.Clear();

	        foreach (var mom in Moms)
	        {
	            mom.Dispose();
	        }
	        Moms.Clear();
        }

	    public void WriteLBD(string path, LBD? lbd = null)
	    {
	        using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create)))
	        {
	            if (lbd == null)
	            {
	                _lbdFile.Write(bw);
	            }
	            else
	            {
	                lbd.Value.Write(bw);
	            }
	        }
	    }
	}
}
