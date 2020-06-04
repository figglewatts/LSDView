using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using LSDView.controller;
using LSDView.Graphics;
using LSDView.Models;
using LSDView.Util;

namespace LSDView.Controllers
{
    public class LBDController
    {
        private readonly TreeController _treeController;
        private readonly VRAMController _vramController;
        private readonly TMDController _tmdController;
        private readonly MOMController _momController;
        private readonly Shader _shader;

        public LBDController(TreeController treeController,
            VRAMController vramController,
            TMDController tmdController,
            MOMController momController)
        {
            _treeController = treeController;
            _vramController = vramController;
            _tmdController = tmdController;
            _momController = momController;
            _shader = new Shader("basic", "Shaders/basic");
        }

        public void LoadLBD(string lbdPath)
        {
            LBD lbd;
            using (BinaryReader br = new BinaryReader(File.Open(lbdPath, FileMode.Open)))
            {
                lbd = new LBD(br);
            }

            LBDDocument document = CreateDocument(lbd);
            _treeController.PopulateTreeWithDocument(document, Path.GetFileName(lbdPath));
        }

        public LBDDocument CreateDocument(LBD lbd)
        {
            TMDDocument tileTmd = _tmdController.CreateDocument(lbd.Tiles);
            List<IRenderable> tileLayout = new List<IRenderable>();

            int tileNo = 0;
            foreach (LBDTile tile in lbd.TileLayout)
            {
                int x = tileNo / lbd.Header.TileWidth;
                int y = tileNo % lbd.Header.TileWidth;

                if (tile.DrawTile)
                {
                    tileLayout.AddRange(LibLSDUtil.CreateLBDTileMesh(tile, lbd.ExtraTiles, x, y, lbd.Tiles, _shader,
                        _vramController.VRAMTexture));
                }

                tileNo++;
            }

            List<MOMDocument> entities = null;
            if (lbd.Header.HasMML)
            {
                entities = new List<MOMDocument>();
                foreach (MOM mom in lbd.MML?.MOMs)
                {
                    entities.Add(_momController.CreateDocument(mom));
                }
            }

            return new LBDDocument(lbd, tileTmd, tileLayout, entities);
        }
    }
}
