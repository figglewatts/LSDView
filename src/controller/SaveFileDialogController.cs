using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSDView.view;

namespace LSDView.controller
{
    public class SaveFileDialogController
    {
        public ILSDView View { get; set; }

        public const string TMD_FILTER = "TMD file|*.tmd";

        public SaveFileDialogController(ILSDView view)
        {
            View = view;
        }

        public string Filter
        {
            get => View.SaveDialog.Filter;
            set => View.SaveDialog.Filter = value;
        }

        public string FileName
        {
            get => View.SaveDialog.FileName;
            set => View.SaveDialog.FileName = value;
        }

        public DialogResult ShowDialog()
        {
            return View.SaveDialog.ShowDialog();
        }
    }
}
