using System.Drawing;
using System.Windows.Forms;
using ImageProcessing.Core;
using ImageProcessing.UI.Properties;

namespace ImageProcessing.UI
{
    public partial class ImageForm : Form
    {
        private const int DefaultBrightnessValue = 255;

        private ImageProcessor _imageProcessor;

        public ImageForm()
        {
            InitializeComponent();
            menuStrip1.Visible = false;
            menuStrip1.AllowMerge = false;
            BrightnessTextBox.Text = @"0";
            BrightnessTrackBar.Value = DefaultBrightnessValue;
            BrightnessTrackBar.ValueChanged += OnBrightnessTrackBarValueChanged;
            BrightnessTrackBar.TickFrequency = 30;
            BrightnessTrackBar.LargeChange = 10;
            BrightnessTrackBar.Minimum = 0;
            BrightnessTrackBar.Maximum = 510;
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
                menuStrip1.Visible = true;
            }
        }

        private void FileSaveMenuItem_Click(object sender, System.EventArgs e)
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

        private async void ImageForm_Load(object sender, System.EventArgs e)
        {
            if (ImageProcessor != null)
            {
                Image = await ImageProcessor.Process();
            }
        }

        private async void OnBrightnessTrackBarValueChanged(object sender, System.EventArgs e)
        {
            var delta = BrightnessTrackBar.Value - DefaultBrightnessValue;

            BrightnessTextBox.Text = delta.ToString();
            Image = await ImageProcessor.AdjustBrightness(delta);
        }
    }
}
