using System;
using System.Collections.Generic;
using libLSD.Formats;
using LSDView.Graphics;

namespace LSDView.Models
{
    public class LBDDocument : AbstractDocument<LBD>, IDocument
    {
        public DocumentType Type => DocumentType.LBD;

        public EventHandler OnLoad { get; set; }

        public EventHandler OnUnload { get; set; }

        public override LBD Document { get; }

        public List<Mesh> TileMeshes { get; }

        public List<Mesh> TileLayout { get; }

        public LBDDocument(LBD lbd, List<Mesh> tileMeshes, List<Mesh> tileLayout)
        {
            Document = lbd;
            TileMeshes = tileMeshes;
            TileLayout = tileLayout;
        }
    }
}
