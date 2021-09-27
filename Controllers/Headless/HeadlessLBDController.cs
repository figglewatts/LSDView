using System.Collections.Generic;
using libLSD.Formats;
using LSDView.Controllers.Interface;
using LSDView.Graphics;
using LSDView.Models;
using LSDView.Util;

namespace LSDView.Controllers.Headless
{
    public class HeadlessLBDController : IFileFormatController<LBD, LBDDocument>
    {
        protected readonly IVRAMController _vramController;
        protected readonly IFileFormatController<TMD, TMDDocument> _tmdController;
        protected readonly IFileFormatController<MOM, MOMDocument> _momController;

        public HeadlessLBDController(IVRAMController vramController,
            IFileFormatController<TMD, TMDDocument> tmdController,
            IFileFormatController<MOM, MOMDocument> momController)
        {
            _vramController = vramController;
            _tmdController = tmdController;
            _momController = momController;
        }

        public LBD Load(string path) { return LibLSDUtil.LoadLBD(path); }

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
                    tileLayout.AddRange(LibLSDUtil.CreateLBDTileMesh(tile, lbd.ExtraTiles, x, y, lbd.Tiles, null,
                        _vramController.VRAM, headless: true));
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
