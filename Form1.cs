using Gigasoft.ProEssentials.Enums;

namespace chart_example3
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }




        private void Form1_Load(object sender, EventArgs e)
        {
            contourPlot1.contourPlotEventSender += ContourPlot1_contourPlotEventSender;


        }
        private void ContourPlot1_contourPlotEventSender(object sender, EventArgs e)
        {
            contourPlot1.Graph_Load(sender, e);
        }

    }
}