using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSDView.model;
using LSDView.view;

namespace LSDView.controller
{
    public class DocumentController
    {
        public ILSDView View { get; set; }

        public IDocument Document { get; private set; }

        private OutlineController _outlineController;

        public DocumentController(ILSDView view, OutlineController outlineController)
        {
            View = view;
            _outlineController = outlineController;
        }

        public void LoadDocument(IDocument doc)
        {
            Document?.OnUnload(this, EventArgs.Empty);

            Document = doc;
            Document.OnLoad += (sender, args) => _outlineController.PopulateOutlineWithDocument(Document);
            Document.OnUnload += (sender, args) => _outlineController.ClearOutline();

            Document.OnLoad(this, EventArgs.Empty);
        }
    }
}
