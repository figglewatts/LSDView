using System;
using libLSD.Formats;

namespace LSDView.Models
{
    public class LBDDocument : AbstractDocument<LBD>, IDocument
    {
        public DocumentType Type => DocumentType.LBD;

        public EventHandler OnLoad { get; set; }

        public EventHandler OnUnload { get; set; }

        public override LBD Document { get; }

        public LBDDocument(LBD lbd) { Document = lbd; }
    }
}
