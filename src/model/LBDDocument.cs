using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using LSDView.model;

namespace LSDView.model
{
    public class LBDDocument : AbstractDocument<LBD>, IDocument
    {
        public DocumentType Type => DocumentType.LBD;

        public EventHandler OnLoad { get; set; }

        public EventHandler OnUnload { get; set; }

        public override LBD Document { get; }

        public LBDDocument(LBD lbd)
        {
            Document = lbd;
        }
    }
}
