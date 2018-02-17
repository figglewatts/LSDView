using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSDView.model;
using LSDView.view;

namespace LSDView.controller
{
    public class ExportController
    {
        private ILSDView _view;
        public ILSDView View
        {
            get => _view;
            set
            {
                _view = value;
                _exportMenuItem =
                    ((ToolStripMenuItem) View.MenuStrip.Items["fileToolStripMenuItem"]).DropDownItems[
                        "exportToolStripMenuItem"] as ToolStripMenuItem;
            }
        }
        public DocumentController DocumentController { get; set; }
        public SaveFileDialogController SaveFileDialogController { get; set; }
        public TMDController TMDController { get; set; }
        public TIMController TIMController { get; set; }
        public TIXController TIXController { get; set; }
        public MOMController MOMController { get; set; }
        public LBDController LBDController { get; set; }

        private ToolStripMenuItem _exportMenuItem;

        public ExportController(ILSDView view)
        {
            View = view;
        }

        public void SetExportButtonsEnabled(bool state)
        {
            foreach (ToolStripMenuItem btn in _exportMenuItem.DropDownItems)
            {
                btn.Enabled = state;
            }
        }

        public void ExportDocumentAsOriginal()
        {
            IDocument currentDoc = DocumentController.Document;

            switch (currentDoc.Type)
            {
                case DocumentType.TMD:
                {
                    ExportTMD(Path.GetFileName(TMDController.TMDPath));
                    break;
                }
                case DocumentType.TIM:
                {
                    ExportTIM(Path.GetFileName(TIMController.TIMPath));
                    break;
                }
                case DocumentType.TIX:
                {
                    ExportTIX(Path.GetFileName(TIXController.TIXPath));
                    break;
                }
            }
        }

        private void ExportTMD(string filename)
        {
            SaveFileDialogController.Filter = SaveFileDialogController.TMD_FILTER;
            SaveFileDialogController.FileName = filename;

            if (SaveFileDialogController.ShowDialog() == DialogResult.OK)
            {
                TMDController.WriteTMD(SaveFileDialogController.FileName);
            }
        }

        private void ExportTIM(string filename)
        {
            SaveFileDialogController.Filter = SaveFileDialogController.TIM_FILTER;
            SaveFileDialogController.FileName = filename;

            if (SaveFileDialogController.ShowDialog() == DialogResult.OK)
            {
                TIMController.WriteTIM(SaveFileDialogController.FileName);
            }
        }

        private void ExportTIX(string filename)
        {
            SaveFileDialogController.Filter = SaveFileDialogController.TIX_FILTER;
            SaveFileDialogController.FileName = filename;

            if (SaveFileDialogController.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(SaveFileDialogController.FilterIndex);
                switch (SaveFileDialogController.FilterIndex)
                {
                    case 1: // write the TIX file
                    {
                        TIXController.WriteTIX(SaveFileDialogController.FileName);
                        break;
                    }
                    case 2: // write the TIM files
                    {
                        TIXController.WriteTIXTIMs(SaveFileDialogController.FileName);
                        break;
                    }
                }
            }
        }
    }
}
