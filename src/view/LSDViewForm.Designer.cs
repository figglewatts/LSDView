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
            this.viewingWindow = new OpenTK.GLControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewOutline = new System.Windows.Forms.TreeView();
            this.MainContainer = new System.Windows.Forms.SplitContainer();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).BeginInit();
            this.MainContainer.Panel1.SuspendLayout();
            this.MainContainer.Panel2.SuspendLayout();
            this.MainContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewingWindow
            // 
            this.viewingWindow.AutoSize = true;
            this.viewingWindow.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.viewingWindow.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.viewingWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewingWindow.Location = new System.Drawing.Point(0, 0);
            this.viewingWindow.MaximumSize = new System.Drawing.Size(9999, 9999);
            this.viewingWindow.MinimumSize = new System.Drawing.Size(540, 540);
            this.viewingWindow.Name = "viewingWindow";
            this.viewingWindow.Size = new System.Drawing.Size(600, 540);
            this.viewingWindow.TabIndex = 0;
            this.viewingWindow.VSync = false;
            this.viewingWindow.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl_Paint);
            this.viewingWindow.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl_KeyDown);
            this.viewingWindow.Resize += new System.EventHandler(this.glControl_Resize);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(804, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
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
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open...";
            // 
            // ViewOutline
            // 
            this.ViewOutline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ViewOutline.Location = new System.Drawing.Point(0, 0);
            this.ViewOutline.Name = "ViewOutline";
            this.ViewOutline.Size = new System.Drawing.Size(200, 540);
            this.ViewOutline.TabIndex = 2;
            // 
            // MainContainer
            // 
            this.MainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.MainContainer.Location = new System.Drawing.Point(0, 24);
            this.MainContainer.Name = "MainContainer";
            // 
            // MainContainer.Panel1
            // 
            this.MainContainer.Panel1.Controls.Add(this.ViewOutline);
            this.MainContainer.Panel1MinSize = 200;
            // 
            // MainContainer.Panel2
            // 
            this.MainContainer.Panel2.Controls.Add(this.viewingWindow);
            this.MainContainer.Size = new System.Drawing.Size(804, 540);
            this.MainContainer.SplitterDistance = 200;
            this.MainContainer.TabIndex = 3;
            // 
            // LSDViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 564);
            this.Controls.Add(this.MainContainer);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(820, 603);
            this.Name = "LSDViewForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "LSDView";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.MainContainer.Panel1.ResumeLayout(false);
            this.MainContainer.Panel2.ResumeLayout(false);
            this.MainContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).EndInit();
            this.MainContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl viewingWindow;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.TreeView ViewOutline;
        private System.Windows.Forms.SplitContainer MainContainer;
    }
}