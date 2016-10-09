namespace ImageProcessing.UI
{
    partial class DigramForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.histogram = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.loadingImageBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.histogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadingImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Histogram
            // 
            chartArea2.Name = "ChartArea1";
            this.histogram.ChartAreas.Add(chartArea2);
            this.histogram.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Name = "Legend1";
            this.histogram.Legends.Add(legend2);
            this.histogram.Location = new System.Drawing.Point(0, 0);
            this.histogram.Name = "histogram";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.histogram.Series.Add(series2);
            this.histogram.Size = new System.Drawing.Size(507, 393);
            this.histogram.TabIndex = 0;
            this.histogram.Text = "chart1";
            // 
            // pictureBox1
            // 
            this.loadingImageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadingImageBox.Location = new System.Drawing.Point(0, 0);
            this.loadingImageBox.Name = "loadingImageBox";
            this.loadingImageBox.Size = new System.Drawing.Size(507, 393);
            this.loadingImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.loadingImageBox.TabIndex = 1;
            this.loadingImageBox.TabStop = false;
            // 
            // DigramForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 393);
            this.Controls.Add(this.loadingImageBox);
            this.Controls.Add(this.histogram);
            this.Name = "DigramForm";
            this.Text = "DigramForm";
            ((System.ComponentModel.ISupportInitialize)(this.histogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadingImageBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart histogram;
        private System.Windows.Forms.PictureBox loadingImageBox;
    }
}