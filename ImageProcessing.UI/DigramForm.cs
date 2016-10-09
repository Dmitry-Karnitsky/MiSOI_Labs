using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ImageProcessing.UI
{
    public partial class DigramForm : Form
    {
        public DigramForm()
        {
            InitializeComponent();
            MaximizeBox = false;

            histogram.Series.Clear();
            histogram.Legends.Clear();

            var series = new Series
            {
                ChartType = SeriesChartType.Area,
                IsVisibleInLegend = false,
                Color = Color.Black
            };

            histogram.Series.Add(series);
            histogram.ChartAreas[0].AxisX.Minimum = 0;
            histogram.ChartAreas[0].AxisX.Maximum = 255;

            loadingImageBox.Image = Image.FromFile("Loading_icon.gif");
            loadingImageBox.BackColor = Color.White;
        }

        public int[] Points
        {
            set
            {
                histogram.Series[0].Points.Clear();

                for (var i = 0; i < value.Length; i++)
                {
                    histogram.Series[0].Points.AddXY(i, value[i]);
                }

                loadingImageBox.Visible = false;
            }
        }
    }
}
