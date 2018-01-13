namespace LSDView.view
{
    partial class LSDViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._viewingWindow = new OpenTK.GLControl();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._viewOutline = new System.Windows.Forms.TreeView();
            this.MainContainer = new System.Windows.Forms.SplitContainer();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).BeginInit();
            this.MainContainer.Panel1.SuspendLayout();
            this.MainContainer.Panel2.SuspendLayout();
            this.MainContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // _viewingWindow
            // 
            this._viewingWindow.AutoSize = true;
            this._viewingWindow.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this._viewingWindow.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._viewingWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._viewingWindow.Location = new System.Drawing.Point(0, 0);
            this._viewingWindow.MaximumSize = new System.Drawing.Size(9999, 9999);
            this._viewingWindow.MinimumSize = new System.Drawing.Size(540, 540);
            this._viewingWindow.Name = "_viewingWindow";
            this._viewingWindow.Size = new System.Drawing.Size(600, 540);
            this._viewingWindow.TabIndex = 0;
            this._viewingWindow.VSync = false;
            this._viewingWindow.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl_Paint);
            this._viewingWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this._viewingWindow_MouseDown);
            this._viewingWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this._viewingWindow_MouseMove);
            this._viewingWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this._viewingWindow_MouseUp);
            this._viewingWindow.Resize += new System.EventHandler(this.glControl_Resize);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(804, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // _viewOutline
            // 
            this._viewOutline.Dock = System.Windows.Forms.DockStyle.Fill;
            this._viewOutline.Location = new System.Drawing.Point(0, 0);
            this._viewOutline.Name = "_viewOutline";
            this._viewOutline.Size = new System.Drawing.Size(200, 540);
            this._viewOutline.TabIndex = 2;
            // 
            // MainContainer
            // 
            this.MainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.MainContainer.IsSplitterFixed = true;
            this.MainContainer.Location = new System.Drawing.Point(0, 24);
            this.MainContainer.Name = "MainContainer";
            // 
            // MainContainer.Panel1
            // 
            this.MainContainer.Panel1.Controls.Add(this._viewOutline);
            this.MainContainer.Panel1MinSize = 200;
            // 
            // MainContainer.Panel2
            // 
            this.MainContainer.Panel2.Controls.Add(this._viewingWindow);
            this.MainContainer.Size = new System.Drawing.Size(804, 540);
            this.MainContainer.SplitterDistance = 200;
            this.MainContainer.TabIndex = 3;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Title = "Open LSD file...";
            // 
            // LSDViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 564);
            this.Controls.Add(this.MainContainer);
            this.Controls.Add(this.menuStrip);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(820, 603);
            this.Name = "LSDViewForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "LSDView";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.MainContainer.Panel1.ResumeLayout(false);
            this.MainContainer.Panel2.ResumeLayout(false);
            this.MainContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).EndInit();
            this.MainContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl _viewingWindow;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.SplitContainer MainContainer;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TreeView _viewOutline;
    }
}