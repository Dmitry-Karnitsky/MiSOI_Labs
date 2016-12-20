using System.ComponentModel;
using System.Windows.Forms;

namespace ImageProcessing.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NegativeImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SobelFilterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GrayscaleImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forstnerDetectorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teachNetworkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileOpenMenuItem,
            this.NegativeImageMenuItem,
            this.SobelFilterMenuItem,
            this.GrayscaleImageMenuItem,
            this.forstnerDetectorToolStripMenuItem,
            this.teachNetworkToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1095, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // FileOpenMenuItem
            // 
            this.FileOpenMenuItem.Name = "FileOpenMenuItem";
            this.FileOpenMenuItem.Size = new System.Drawing.Size(48, 20);
            this.FileOpenMenuItem.Text = "Open";
            this.FileOpenMenuItem.Click += new System.EventHandler(this.OnFileOpenMenuItemClick);
            // 
            // NegativeImageMenuItem
            // 
            this.NegativeImageMenuItem.Name = "NegativeImageMenuItem";
            this.NegativeImageMenuItem.Size = new System.Drawing.Size(66, 20);
            this.NegativeImageMenuItem.Text = "Negative";
            this.NegativeImageMenuItem.Click += new System.EventHandler(this.OnNegativeImageMenuItemClick);
            // 
            // SobelFilterMenuItem
            // 
            this.SobelFilterMenuItem.Name = "SobelFilterMenuItem";
            this.SobelFilterMenuItem.Size = new System.Drawing.Size(131, 20);
            this.SobelFilterMenuItem.Text = "Sobel Edge Detection";
            this.SobelFilterMenuItem.Click += new System.EventHandler(this.OnSobelFilterMenuItemClick);
            // 
            // GrayscaleImageMenuItem
            // 
            this.GrayscaleImageMenuItem.Name = "GrayscaleImageMenuItem";
            this.GrayscaleImageMenuItem.Size = new System.Drawing.Size(69, 20);
            this.GrayscaleImageMenuItem.Text = "Grayscale";
            this.GrayscaleImageMenuItem.Click += new System.EventHandler(this.OnGrayscaleImageMenuItemClick);
            // 
            // forstnerDetectorToolStripMenuItem
            // 
            this.forstnerDetectorToolStripMenuItem.Name = "forstnerDetectorToolStripMenuItem";
            this.forstnerDetectorToolStripMenuItem.Size = new System.Drawing.Size(110, 20);
            this.forstnerDetectorToolStripMenuItem.Text = "Forstner Detector";
            this.forstnerDetectorToolStripMenuItem.Click += new System.EventHandler(this.OnForstnerDetectorMenuItemClick);
            // 
            // teachNetworkToolStripMenuItem
            // 
            this.teachNetworkToolStripMenuItem.Name = "teachNetworkToolStripMenuItem";
            this.teachNetworkToolStripMenuItem.Size = new System.Drawing.Size(99, 20);
            this.teachNetworkToolStripMenuItem.Text = "Teach Network";
            this.teachNetworkToolStripMenuItem.Click += new System.EventHandler(this.teachNetworkToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1095, 565);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Image Processing";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem FileOpenMenuItem;
        private ToolStripMenuItem NegativeImageMenuItem;
        private ToolStripMenuItem SobelFilterMenuItem;
        private ToolStripMenuItem GrayscaleImageMenuItem;
        private ToolStripMenuItem forstnerDetectorToolStripMenuItem;
        private ToolStripMenuItem teachNetworkToolStripMenuItem;
    }
}

