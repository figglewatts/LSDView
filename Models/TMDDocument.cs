using System;
using System.Collections.Generic;
using libLSD.Formats;
using LSDView.Graphics;

namespace LSDView.Models
{
    public class TMDDocument : AbstractDocument<TMD>, IDocument
    {
        public override TMD Document { get; }
        public DocumentType Type => DocumentType.TMD;
        public EventHandler OnLoad { get; set; }
        public EventHandler OnUnload { get; set; }

        public List<Mesh> ObjectMeshes { get; }

        public TMDDocument(TMD tmd, List<Mesh> objectMeshes)
        {
            Document = tmd;
            ObjectMeshes = objectMeshes;
        }
    }
}
