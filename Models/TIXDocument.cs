using System;
using System.Collections.Generic;
using libLSD.Formats;

namespace LSDView.Models
{
    public class TIXDocument : AbstractDocument<TIX>, IDocument
    {
        public override TIX Document { get; }
        public DocumentType Type => DocumentType.TIX;
        public EventHandler OnLoad { get; set; }
        public EventHandler OnUnload { get; set; }

        public List<TIMDocument> TIMs { get; }

        public TIXDocument(TIX tix, List<TIMDocument> tims)
        {
            Document = tix;
            TIMs = tims;
        }
    }
}
