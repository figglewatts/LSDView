using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using LSDView.model;

namespace LSDView.model
{
    public class MOMDocument : AbstractDocument<MOMData>, IDocument
    {
        public DocumentType Type => DocumentType.MOM;

        public EventHandler OnLoad { get; set; }

        public EventHandler OnUnload { get; set; }

        public override MOMData Document { get; }

        public MOMDocument(MOMData mom)
        {
            Document = mom;
        }
    }
}
