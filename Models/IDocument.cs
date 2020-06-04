using System;

namespace LSDView.Models
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
