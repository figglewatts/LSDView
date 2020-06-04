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

        public List<IRenderable> ObjectMeshes { get; }

        public TMDDocument(TMD tmd, List<IRenderable> objectMeshes)
        {
            Document = tmd;
            ObjectMeshes = objectMeshes;
        }
    }
}
