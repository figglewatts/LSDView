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
            this.redButton = new System.Windows.Forms.Button();
            this.greenButton = new System.Windows.Forms.Button();
            this.blueButton = new System.Windows.Forms.Button();
            this.viewingWindow = new OpenTK.GLControl();
            this.SuspendLayout();
            // 
            // redButton
            // 
            this.redButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.redButton.Location = new System.Drawing.Point(639, 13);
            this.redButton.Name = "redButton";
            this.redButton.Size = new System.Drawing.Size(133, 23);
            this.redButton.TabIndex = 1;
            this.redButton.Text = "Red";
            this.redButton.UseVisualStyleBackColor = true;
            this.redButton.Click += new System.EventHandler(this.redButton_Click);
            // 
            // greenButton
            // 
            this.greenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.greenButton.Location = new System.Drawing.Point(639, 43);
            this.greenButton.Name = "greenButton";
            this.greenButton.Size = new System.Drawing.Size(133, 23);
            this.greenButton.TabIndex = 2;
            this.greenButton.Text = "Green";
            this.greenButton.UseVisualStyleBackColor = true;
            this.greenButton.Click += new System.EventHandler(this.greenButton_Click);
            // 
            // blueButton
            // 
            this.blueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.blueButton.Location = new System.Drawing.Point(639, 73);
            this.blueButton.Name = "blueButton";
            this.blueButton.Size = new System.Drawing.Size(133, 23);
            this.blueButton.TabIndex = 3;
            this.blueButton.Text = "Blue";
            this.blueButton.UseVisualStyleBackColor = true;
            this.blueButton.Click += new System.EventHandler(this.blueButton_Click);
            // 
            // viewingWindow
            // 
            this.viewingWindow.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.viewingWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewingWindow.Location = new System.Drawing.Point(0, 0);
            this.viewingWindow.Name = "viewingWindow";
            this.viewingWindow.Size = new System.Drawing.Size(781, 564);
            this.viewingWindow.TabIndex = 0;
            this.viewingWindow.VSync = false;
            this.viewingWindow.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl_Paint);
            this.viewingWindow.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl_KeyDown);
            this.viewingWindow.Resize += new System.EventHandler(this.glControl_Resize);
            // 
            // LSDViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 564);
            this.Controls.Add(this.blueButton);
            this.Controls.Add(this.greenButton);
            this.Controls.Add(this.redButton);
            this.Controls.Add(this.viewingWindow);
            this.Name = "LSDViewForm";
            this.Text = "OpenTK Windows Forms Tutorial 01 - Your first window";
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.GLControl viewingWindow;
        private System.Windows.Forms.Button redButton;
        private System.Windows.Forms.Button greenButton;
        private System.Windows.Forms.Button blueButton;
    }
}