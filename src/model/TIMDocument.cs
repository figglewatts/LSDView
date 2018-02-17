using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using LSDView.model;

namespace LSDView.model
{
    public class TIMDocument : AbstractDocument<TIM>, IDocument
    {
        public DocumentType Type => DocumentType.TIM;

        public EventHandler OnLoad { get; set; }

        public EventHandler OnUnload { get; set; }

        public override TIM Document { get; }

        public TIMDocument(TIM tim)
        {
            Document = tim;
        }
    }
}
