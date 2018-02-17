using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using LSDView.model;

namespace LSDView.model
{
    public class TIXDocument : AbstractDocument<TIX>, IDocument
    {
        public DocumentType Type => DocumentType.TIX;

        public EventHandler OnLoad { get; set; }

        public EventHandler OnUnload { get; set; }

        public override TIX Document { get; }
        
        public TIXDocument(TIX tix)
        {
            Document = tix;
        }
    }
}
