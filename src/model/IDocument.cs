using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSDView.model
{
    public interface IDocument
    {
        DocumentType Type { get; }
        EventHandler OnLoad { get; set; }
        EventHandler OnUnload { get; set; }
    }

    public enum DocumentType
    {
        TMD,
        TIM,
        MOM,
        TIX,
        LBD
    }
}
