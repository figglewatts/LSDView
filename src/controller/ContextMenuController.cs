using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSDView.model;
using LSDView.view;

namespace LSDView.controller
{
    public class ContextMenuController : IOutlineTreeViewVisitor
    {
        private ILSDView _view;
        public ILSDView View
        {
            get => _view;
            set
            {
                _view = value;
                _exportMenu = (ToolStripMenuItem) _view.OutlineContextMenu.Items["exportAsContextMenuItem"];
            }
        }

        public ExportController ExportController { get; set; }

        private ToolStripMenuItem _exportMenu;

        public ContextMenuController(ILSDView view)
        {
            View = view;
        }

        public void ClearExportMenu()
        {
            _exportMenu.DropDownItems.Clear();
        }

        public void SetExportEnabled(bool state)
        {
            _exportMenu.Enabled = state;
        }

        public void Visit(LBDTreeNode node)
        {
            ClearExportMenu();
            ToolStripMenuItem exportAsOriginal = new ToolStripMenuItem("LBD");
            exportAsOriginal.Click += (sender, args) => ExportController.ExportLBD("", node.Lbd);
            ToolStripMenuItem exportAsOBJ = new ToolStripMenuItem("OBJ");
            
            _exportMenu.DropDownItems.Add(exportAsOriginal);
            _exportMenu.DropDownItems.Add(exportAsOBJ);
        }

        public void Visit(MOMTreeNode node)
        {
            ClearExportMenu();
            ToolStripMenuItem exportAsOriginal = new ToolStripMenuItem("MOM");
            exportAsOriginal.Click += (sender, args) => ExportController.ExportMOM("", node.Mom);
            ToolStripMenuItem exportAsOBJ = new ToolStripMenuItem("OBJ");
            _exportMenu.DropDownItems.Add(exportAsOriginal);
            _exportMenu.DropDownItems.Add(exportAsOBJ);
        }

        public void Visit(TIMTreeNode node)
        {
            ClearExportMenu();
            ToolStripMenuItem exportAsOriginal = new ToolStripMenuItem("TIM");
            exportAsOriginal.Click += (sender, args) => ExportController.ExportTIM("", node.Tim);
            ToolStripMenuItem exportAsPNG = new ToolStripMenuItem("PNG");
            exportAsPNG.Click += (sender, args) => ExportController.ExportImage(node.Tim, ImageFormat.Png);
            ToolStripMenuItem exportAsBMP = new ToolStripMenuItem("BMP");
            exportAsBMP.Click += (sender, args) => ExportController.ExportImage(node.Tim, ImageFormat.Bmp);
            _exportMenu.DropDownItems.Add(exportAsOriginal);
            _exportMenu.DropDownItems.Add(exportAsPNG);
            _exportMenu.DropDownItems.Add(exportAsBMP);
        }

        public void Visit(TIXTreeNode node)
        {
            ClearExportMenu();
            ToolStripMenuItem exportAsOriginal = new ToolStripMenuItem("TIX");
            exportAsOriginal.Click += (sender, args) => ExportController.ExportTIX("", node.Tix);
            ToolStripMenuItem exportAsTIM = new ToolStripMenuItem("TIM");
            exportAsTIM.Click += (sender, args) => ExportController.ExportTIXTIMs("", node.Tix);
            ToolStripMenuItem exportAsPNG = new ToolStripMenuItem("PNG");
            ToolStripMenuItem exportAsBMP = new ToolStripMenuItem("BMP");
            _exportMenu.DropDownItems.Add(exportAsOriginal);
            _exportMenu.DropDownItems.Add(exportAsTIM);
            _exportMenu.DropDownItems.Add(exportAsPNG);
            _exportMenu.DropDownItems.Add(exportAsBMP);
        }

        public void Visit(TMDTreeNode node)
        {
            ClearExportMenu();
            ToolStripMenuItem exportAsOriginal = new ToolStripMenuItem("TMD");
            exportAsOriginal.Click += (sender, args) => ExportController.ExportTMD("", node.Tmd); 
            ToolStripMenuItem exportAsOBJ = new ToolStripMenuItem("OBJ");
            exportAsOBJ.Click += (sender, args) => ExportController.ExportOBJ(node.Tmd);
            _exportMenu.DropDownItems.Add(exportAsOriginal);
            _exportMenu.DropDownItems.Add(exportAsOBJ);
        }

        public void Visit(TMDObjectTreeNode node)
        {
            ClearExportMenu();
            ToolStripMenuItem exportAsOBJ = new ToolStripMenuItem("OBJ");
            exportAsOBJ.Click += (sender, args) => ExportController.ExportOBJ(node.Renderables);
            _exportMenu.DropDownItems.Add(exportAsOBJ);
        }

        public void Visit(RenderableAnimationTreeNode node)
        {
            ClearExportMenu();
            ToolStripMenuItem exportAsOriginal = new ToolStripMenuItem("TOD");
            exportAsOriginal.Click += (sender, args) => ExportController.ExportTOD("", node.Animation.AnimationData);
            ToolStripMenuItem exportAsOBJ = new ToolStripMenuItem("OBJ");
            _exportMenu.DropDownItems.Add(exportAsOriginal);
            _exportMenu.DropDownItems.Add(exportAsOBJ);
        }
    }
}
