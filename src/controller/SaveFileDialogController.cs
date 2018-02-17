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
        public const string TIM_FILTER = "TIM file|*.tim";
        public const string TIX_FILTER = "TIX file|*.tix|All TIM files|*.tim";
        public const string MOM_FILTER = "MOM file|*.mom";
        public const string LBD_FILTER = "LBD file|*.lbd";

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

        public int FilterIndex => View.SaveDialog.FilterIndex;

        public DialogResult ShowDialog()
        {
            return View.SaveDialog.ShowDialog();
        }
    }
}
