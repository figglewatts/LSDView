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

        public TMDDocument TilesTMD { get; }

        public List<IRenderable> TileLayout { get; }

        public List<MOMDocument> Entities { get; }

        public LBDDocument(LBD lbd, TMDDocument tilesTmd, List<IRenderable> tileLayout, List<MOMDocument> entities)
        {
            Document = lbd;
            TilesTMD = tilesTmd;
            TileLayout = tileLayout;
            Entities = entities;
        }
    }
}
