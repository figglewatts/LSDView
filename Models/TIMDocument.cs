using System;
using libLSD.Formats;
using LSDView.Graphics;

namespace LSDView.Models
{
    public class TIMDocument : AbstractDocument<TIM>, IDocument
    {
        public override TIM Document { get; }
        public DocumentType Type => DocumentType.TIM;
        public EventHandler OnLoad { get; set; }
        public EventHandler OnUnload { get; set; }

        public Mesh TextureMesh { get; }

        public TIMDocument(TIM tim, Mesh textureMesh)
        {
            Document = tim;
            TextureMesh = textureMesh;
        }
    }
}
