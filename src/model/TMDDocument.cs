using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using LSDView.model;

namespace LSDView.model
{
    public class TMDDocument : AbstractDocument<TMD>, IDocument
    {
        public DocumentType Type => DocumentType.TMD;

        public EventHandler OnLoad { get; set; }

        public EventHandler OnUnload { get; set; }

        public override TMD Document { get; }

        public TMDDocument(TMD tmd)
        {
            Document = tmd;
        }
    }
}
