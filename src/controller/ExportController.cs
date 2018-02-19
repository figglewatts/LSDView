using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libLSD.Formats;
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
        public TODController TODController { get; set; }
        public ImageController ImageController { get; set; }

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

        // TODO(sam): log exports

        public void ExportTMD(string filename = "", TMD? tmd = null)
        {
            SaveFileDialogController.Filter = SaveFileDialogController.TMD_FILTER;
            SaveFileDialogController.FileName = filename;

            if (SaveFileDialogController.ShowDialog() == DialogResult.OK)
            {
                TMDController.WriteTMD(SaveFileDialogController.FileName, tmd);
            }
        }

        public void ExportTIM(string filename = "", TIM? tim = null)
        {
            SaveFileDialogController.Filter = SaveFileDialogController.TIM_FILTER;
            SaveFileDialogController.FileName = filename;

            if (SaveFileDialogController.ShowDialog() == DialogResult.OK)
            {
                TIMController.WriteTIM(SaveFileDialogController.FileName, tim);
            }
        }

        public void ExportTIX(string filename = "", TIX? tix = null)
        {
            SaveFileDialogController.Filter = SaveFileDialogController.TIX_FILTER;
            SaveFileDialogController.FileName = filename;

            if (SaveFileDialogController.ShowDialog() == DialogResult.OK)
            {
                TIXController.WriteTIX(SaveFileDialogController.FileName, tix);
            }
        }

        public void ExportTIXTIMs(string filename = "", TIX? tix = null)
        {
            SaveFileDialogController.Filter = SaveFileDialogController.TIM_FILTER;
            SaveFileDialogController.FileName = filename;

            if (SaveFileDialogController.ShowDialog() == DialogResult.OK)
            {
                TIXController.WriteTIXTIMs(SaveFileDialogController.FileName, tix);
            }
        }

        public void ExportLBD(string filename = "", LBD? lbd = null)
        {
            SaveFileDialogController.Filter = SaveFileDialogController.LBD_FILTER;
            SaveFileDialogController.FileName = filename;

            if (SaveFileDialogController.ShowDialog() == DialogResult.OK)
            {
                LBDController.WriteLBD(SaveFileDialogController.FileName, lbd);
            }
        }

        public void ExportTOD(string filename = "", TOD? tod = null)
        {
            SaveFileDialogController.Filter = SaveFileDialogController.TOD_FILTER;
            SaveFileDialogController.FileName = filename;

            if (SaveFileDialogController.ShowDialog() == DialogResult.OK)
            {
                TODController.WriteTOD(SaveFileDialogController.FileName, tod);
            }
        }

        public void ExportMOM(string filename = "", MOM? mom = null)
        {
            SaveFileDialogController.Filter = SaveFileDialogController.MOM_FILTER;
            SaveFileDialogController.FileName = filename;

            if (SaveFileDialogController.ShowDialog() == DialogResult.OK)
            {
                MOMController.WriteMOM(SaveFileDialogController.FileName, mom);
            }
        }

        public void ExportImage(TIM tim, ImageFormat format)
        {
            if (format.Equals(ImageFormat.Bmp))
            {
                SaveFileDialogController.Filter = SaveFileDialogController.BMP_FILTER;
            }
            else if (format.Equals(ImageFormat.Png))
            {
                SaveFileDialogController.Filter = SaveFileDialogController.PNG_FILTER;
            }
            else
            {
                throw new ArgumentException($"Invalid ImageFormat {format}");
            }

            SaveFileDialogController.FileName = "";

            if (SaveFileDialogController.ShowDialog() == DialogResult.OK)
            {
                ImageController.WriteTIMAsImage(SaveFileDialogController.FileName, tim, format);
            }
        }
    }
}
