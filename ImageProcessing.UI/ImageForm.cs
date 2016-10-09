using System;
using System.Drawing;
using System.Windows.Forms;
using ImageProcessing.Core;
using ImageProcessing.UI.Properties;

namespace ImageProcessing.UI
{
    public partial class ImageForm : Form
    {
        public const int OffsetToImageFromTop = 42;

        private ImageProcessor _imageProcessor;

        public ImageForm()
        {
            InitializeComponent();
            MaximizeBox = false;

            menuStrip.Visible = false;
            menuStrip.AllowMerge = false;
            brightnessTextBox.Text = @"0";
            brightnessTrackBar.Value = 0;
            brightnessTrackBar.ValueChanged += OnBrightnessTrackBarValueChanged;
            brightnessTrackBar.TickFrequency = 30;
            brightnessTrackBar.LargeChange = 10;
            brightnessTrackBar.Minimum = -255;
            brightnessTrackBar.Maximum = 255;
            imageBox.Top = OffsetToImageFromTop;
        }

        public Image Image
        {
            get { return imageBox.Image; }
            set { imageBox.Image = value; }
        }

        public ImageProcessor ImageProcessor
        {
            get
            {
                return _imageProcessor;
            }
            set
            {
                _imageProcessor = value;
                menuStrip.Visible = true;
            }
        }

        private void FileSaveMenuItem_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Resources.DefaultFileDialogPath,
                Filter = Resources.FileTypesFilter,
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (DialogResult.OK == saveFileDialog.ShowDialog())
            {
                Image?.Save(saveFileDialog.FileName);
            }
        }

        private async void ImageForm_Load(object sender, EventArgs e)
        {
            if (ImageProcessor == null) return;

            Image = await ImageProcessor.Process();
        }

        private async void OnBrightnessTrackBarValueChanged(object sender, EventArgs e)
        {
            brightnessTextBox.Text = brightnessTrackBar.Value.ToString();
            Image = await ImageProcessor.AdjustBrightness(brightnessTrackBar.Value);
        }

        private async void OnDiagramMenuItemClick(object sender, EventArgs e)
        {
            var diagramForm = new DigramForm
            {
                Text = string.Format(Resources.Diagram, Text),
                Width = 600,
                Height = 600,
                MdiParent = MdiParent
            };

            diagramForm.Show();

            diagramForm.Points = await ImageProcessor.GetBrightnessHistogramValues();
        }
    }
}
