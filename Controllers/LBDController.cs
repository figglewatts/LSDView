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
        private readonly Shader _shader;

        public LBDController(TreeController treeController, VRAMController vramController)
        {
            _treeController = treeController;
            _vramController = vramController;
            _shader = new Shader("basic", "Shaders/basic");
        }

        public void LoadLBD(string lbdPath)
        {
            LBD lbd;
            using (BinaryReader br = new BinaryReader(File.Open(lbdPath, FileMode.Open)))
            {
                lbd = new LBD(br);
            }

            LBDDocument document = createDocument(lbd);
            _treeController.PopulateTreeWithDocument(document, Path.GetFileName(lbdPath));
        }

        private LBDDocument createDocument(LBD lbd)
        {
            List<Mesh> tileMeshes = LibLSDUtil.CreateMeshesFromTMD(lbd.Tiles, _shader, _vramController.VRAMTexture);
            List<Mesh> tileLayout = new List<Mesh>();

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

            return new LBDDocument(lbd, tileMeshes, tileLayout);

            // TODO: LBD MML in document
            // if (lbd.Header.HasMML)
            // {
            //     foreach (MOM mom in lbd.MML?.MOMs)
            //     {
            //         List<Mesh> momTmd = LibLSDUtil.CreateMeshesFromTMD(mom.TMD, _shader, _vramController.VRAMTexture);
            //         List<TODAnimation> momAnimations = new List<TODAnimation>();
            //         foreach (var anim in mom.MOS.TODs)
            //         {
            //             List<Mesh> animatedMeshes =
            //                 LibLSDUtil.CreateMeshesFromTMD(mom.TMD, _shader, _vramController.VRAMTexture);
            //             TODAnimation animationObj = new TODAnimation(animatedMeshes, anim);
            //             momAnimations.Add(animationObj);
            //         }
            //
            //         MOMData momData = new MOMData(mom, momTmd, momAnimations);
            //         Moms.Add(momData);
            //     }
            // }
        }
    }
}
