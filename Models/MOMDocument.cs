using System;
using libLSD.Formats;

namespace LSDView.Models
{
    public class MOMDocument : AbstractDocument<MOM>, IDocument
    {
        public override MOM Document { get; }
        public DocumentType Type => DocumentType.MOM;
        public EventHandler OnLoad { get; set; }
        public EventHandler OnUnload { get; set; }

        public TMDDocument Models { get; }

        public MOMDocument(MOM mom, TMDDocument models)
        {
            Document = mom;
            Models = models;
        }
    }
}
