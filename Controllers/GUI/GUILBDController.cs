using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using LSDView.Controllers.Interface;
using LSDView.Graphics;
using LSDView.Models;
using LSDView.Util;

namespace LSDView.Controllers.GUI
{
    public class GUILBDController : IFileFormatController<LBD, LBDDocument>
    {
        protected readonly ITreeController _treeController;
        protected readonly IVRAMController _vramController;
        protected readonly IFileFormatController<TMD, TMDDocument> _tmdController;
        protected readonly IFileFormatController<MOM, MOMDocument> _momController;
        protected readonly Shader _shader;

        public GUILBDController(ITreeController treeController,
            IVRAMController vramController,
            IFileFormatController<TMD, TMDDocument> tmdController,
            IFileFormatController<MOM, MOMDocument> momController)
        {
            _treeController = treeController;
            _vramController = vramController;
            _tmdController = tmdController;
            _momController = momController;
            _shader = new Shader("basic", "Shaders/basic");
        }

        public LBD Load(string lbdPath)
        {
            var lbd = LibLSDUtil.LoadLBD(lbdPath);

            LBDDocument document = CreateDocument(lbd);
            _treeController.PopulateWithDocument(document, Path.GetFileName(lbdPath));

            return lbd;
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
                        _vramController.VRAM, headless: false));
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
