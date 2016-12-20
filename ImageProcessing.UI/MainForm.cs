using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ImageProcessing.Core;
using ImageProcessing.UI.Properties;
using ImageProcessing.NeuralNetwork.Teaching;

namespace ImageProcessing.UI
{
    public partial class MainForm : Form
    {
        public Bitmap OriginalImage { get; set; }

        public MainForm()
        {
            InitializeComponent();
            IsMdiContainer = true;
        }

        private void OnNegativeImageMenuItemClick(object sender, EventArgs e)
        {
            if (!CanContinueProcessing(Resources.NegativeImage))
            {
                return;
            }

            CreateWindow(Resources.NegativeImage, InitializeProcessor(new NegativeImageProcessor()))
                .Show();
        }

        private void OnSobelFilterMenuItemClick(object sender, EventArgs e)
        {
            if (!CanContinueProcessing(Resources.SobelEdgeDetection))
            {
                return;
            }

            CreateWindow(Resources.SobelEdgeDetection, InitializeProcessor(new SobelEdgeProcessor()))
                .Show();
        }

        private void OnGrayscaleImageMenuItemClick(object sender, EventArgs e)
        {
            if (!CanContinueProcessing(Resources.Grayscale))
            {
                return;
            }

            CreateWindow(Resources.Grayscale, InitializeProcessor(new GrayscaleImageProcessor()))
                .Show();
        }

        private void OnForstnerDetectorMenuItemClick(object sender, EventArgs e)
        {
            if (!CanContinueProcessing(Resources.ForstnerDetector))
            {
                return;
            }

            CreateWindow(Resources.ForstnerDetector, InitializeProcessor(new ForstnerDetectorProcessor()))
                .Show();
        }

        private void OnFileOpenMenuItemClick(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Resources.DefaultFileDialogPath,
                Filter = Resources.FileTypesFilter,
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                OriginalImage = (Bitmap)Image.FromFile(openFileDialog.FileName);

                CreateWindow(Resources.OriginalImageTitle, InitializeProcessor(new ImageProcessor()), brightnessToolsVisible: true)
                    .Show();
            }
        }

        private Form CreateWindow(string handle, ImageProcessor processor, bool brightnessToolsVisible = false)
        {
            var aspect = 1.0 * OriginalImage.Height / OriginalImage.Width;

            return new ImageForm
            {
                MdiParent = this,
                WindowState = FormWindowState.Normal,
                ImageProcessor = processor,
                Text = handle,
                Width = OriginalImage.Width > 800 ? 800 : OriginalImage.Width,
                Height = OriginalImage.Width > 800 ? (int)(800 * aspect) : OriginalImage.Height + ImageForm.OffsetToImageFromTop,
                BrightnessChangeToolsVisible = brightnessToolsVisible
            };
        }

        private bool CanContinueProcessing(string handle)
        {
            return OriginalImage != null &&
                   MdiChildren.FirstOrDefault(f => f.Text == handle) == null;
        }

        private ImageProcessor InitializeProcessor(ImageProcessor processor)
        {
            processor.OriginalImage = (Bitmap)OriginalImage.Clone();
            return processor;
        }

        private void teachNetworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new UtilsTest().PrepareTestFiles();
            new TrainingTest().TrainNetwork();
        }
    }
}
