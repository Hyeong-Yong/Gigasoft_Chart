using Gigasoft.ProEssentials.Enums;

namespace chart_example3
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            timer1.Start();
            timer2.Start();
        }

        Queue<double> trigger_signal_queue = new Queue<double>();



        private void Form1_Load(object sender, EventArgs e)
        {
            //Thread.Sleep(5000);

        }



        private List<double> THz_values = new List<double>();

        private void ContourPlotUpdate(Queue<double> trigger_signal_queue, int length_x, int length_y, double interval_x, double interval_y)
        {
            while (trigger_signal_queue.Count > 0)
                THz_values.Add(trigger_signal_queue.Dequeue());


            if (pesgo1 == null) { return; }
            pesgo1.PeFunction.Reset();

            pesgo1.PeUserInterface.Dialog.ModelessAutoClose = true;

            pesgo1.PeFunction.Reinitialize();

            pesgo1.PeConfigure.PrepareImages = true;
            Random Rand_Num = new Random(unchecked((int)DateTime.Now.Ticks));


            pesgo1.PeUserInterface.Scrollbar.MouseDraggingX = true;
            pesgo1.PeUserInterface.Scrollbar.MouseDraggingY = true;

            int number_y, number_x, frame_size;
            number_x = Convert.ToInt32((length_x + 1) / interval_x);
            number_y = Convert.ToInt32((length_y + 1) / interval_y);
            frame_size = number_x * number_y;

            if (THz_values.Count > frame_size) { return; }

            float[] pMyXData = new float[frame_size];
            float[] pMyYData = new float[frame_size];
            float[] pMyZData = new float[frame_size];

            int s;
            int index;
            for (int y = 0; y < number_y; y++)
            {
                for (int x = 0; x < number_x; x++)
                {
                    index = number_x * y + x;

                    float position_x = (float)(x * interval_x);
                    pMyXData[index] = position_x;

                    float position_y = (float)(y * interval_y);
                    pMyYData[index] = position_y;

                    if (THz_values.Count > index)
                    {
                        float f = (float)(THz_values[index]);
                        pMyZData[index] = f;
                    }
                }
            }

            pesgo1.PeData.NullDataValueZ = 0.0F;

            pesgo1.PeData.Subsets = number_y; // y
            pesgo1.PeData.Points = number_x; //x

            Gigasoft.ProEssentials.Api.PEvsetW(pesgo1.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.XData, pMyXData, frame_size);
            Gigasoft.ProEssentials.Api.PEvsetW(pesgo1.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.YData, pMyYData, frame_size);
            Gigasoft.ProEssentials.Api.PEvsetW(pesgo1.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.ZData, pMyZData, frame_size);

            pesgo1.PeLegend.ContourLegendPrecision = ContourLegendPrecision.OneDecimal;
            // Optionally to manually create a contour scale independent of data //
            // As data changes between various charts, the scale remains the same. //
            //pesgo1.PeGrid.Configure.ManualScaleControlZ = ManualScaleControl.MinMax;
            //pesgo1.PeGrid.Configure.ManualMinZ = 144.0F;
            //pesgo1.PeGrid.Configure.ManualMaxZ = 169.0F;

            // Optional method of Setting Contour Color regions //			
            //for( s = 0; s <= 31; s++)
            //{
            //    pesgo1.PeColor.SubsetColors[s] = Color.FromArgb(255, 0, (byte) (31 + (s * 7)), (byte) (95 + (s * 5)));
            //    pesgo1.PeColor.SubsetColors[s + 32] = Color.FromArgb(255, 0, (byte) (95 + (s * 5)), 0);
            //}				
            //for(s = 0; s <= 35; s++)
            //    pesgo1.PeColor.SubsetColors[s + 64] = Color.FromArgb(255, (byte) (128 + (s * 3)), (byte) (128 + (s * 3)), (byte) (128 + (s * 3)));

            // v9 features, // optional method of setting sizes of contour color regions 
            pesgo1.PeColor.ContourColorProportions[0] = .1F;
            pesgo1.PeColor.ContourColorProportions[1] = .05F;
            pesgo1.PeColor.ContourColorProportions[2] = .1F;
            pesgo1.PeColor.ContourColorProportions[3] = .63F;
            pesgo1.PeColor.ContourColorProportions[4] = .06F;
            pesgo1.PeColor.ContourColorProportions[5] = .06F;
            pesgo1.PeColor.ContourColorBlends = 10;  // this must be set before COLORSET, COLORSET ALWAYS LAST 
            pesgo1.PeColor.ContourColorAlpha = 155;
            pesgo1.PeColor.ContourColorSet = ContourColorSet.BlueCyanGreenYellowBrownWhite;

            pesgo1.PeGrid.Configure.AutoMinMaxPadding = 0;

            // For when ViewingStyle is monochrome //				
            for (s = 0; s < 30; s++)
                pesgo1.PeColor.SubsetShades[s] = Color.FromArgb(255, (byte)(50 + (s * 2)), (byte)(50 + (s * 2)), (byte)(50 + (s * 2)));

            pesgo1.PeLegend.ContourStyle = true;
            pesgo1.PePlot.Option.GraphDataLabels = true;

            // Set Various Other Properties ///
            pesgo1.PeColor.BitmapGradientMode = true;
            pesgo1.PeColor.QuickStyle = QuickStyle.MediumLine;
            pesgo1.PeConfigure.BorderTypes = TABorder.Inset;

            // Set the plotting method ///
            pesgo1.PePlot.Allow.ContourColors = true;
            pesgo1.PePlot.Allow.ContourLines = true;
            pesgo1.PePlot.Allow.ContourColorsShadows = true;
            pesgo1.PePlot.Option.ContourLinesColored = true;

            pesgo1.PePlot.Method = SGraphPlottingMethod.ContourColors;
            pesgo1.PeUserInterface.Menu.DataShadow = MenuControl.Hide;

            pesgo1.PeLegend.Location = LegendLocation.Right;

            // Optional, instead of showing default numeric labels, replace with custom strings
            //pesgo1.PeString.ContourLabels[0] = "0";
            //pesgo1.PeString.ContourLabels[19] = "20";
            //pesgo1.PeString.ContourLabels[39] = "40";
            //pesgo1.PeString.ContourLabels[59] = "60";
            //pesgo1.PeColor.ContourColorSet = ContourColorSet.BlueGreenYellowRed;

            pesgo1.PeGrid.InFront = true;
            pesgo1.PeGrid.LineControl = GridLineControl.Both;
            pesgo1.PeGrid.Style = GridStyle.Dot;
            pesgo1.PeConfigure.PrepareImages = true;
            pesgo1.PeConfigure.CacheBmp = true;

            pesgo1.PeUserInterface.Allow.ZoomStyle = ZoomStyle.Ro2Not;
            pesgo1.PeUserInterface.Allow.Zooming = AllowZooming.HorzAndVert;
            pesgo1.PeUserInterface.Scrollbar.MouseWheelFunction = MouseWheelFunction.HorizontalVerticalZoom;
            pesgo1.PeUserInterface.Scrollbar.MouseWheelZoomSmoothness = 4;
            pesgo1.PeUserInterface.Scrollbar.MouseWheelZoomFactor = 1.35F;
            pesgo1.PeGrid.GridBands = false;

            // Disable other non contour plotting method //
            pesgo1.PePlot.Allow.Line = false;
            pesgo1.PePlot.Allow.Point = false;
            pesgo1.PePlot.Allow.Bar = false;
            pesgo1.PePlot.Allow.Area = false;
            pesgo1.PePlot.Allow.Spline = false;
            pesgo1.PePlot.Allow.SplineArea = false;
            pesgo1.PePlot.Allow.PointsPlusLine = false;
            pesgo1.PePlot.Allow.PointsPlusSpline = false;
            pesgo1.PePlot.Allow.BestFitCurve = false;
            pesgo1.PePlot.Allow.BestFitLine = false;
            pesgo1.PePlot.Allow.Stick = false;

            // Set Titles //
            pesgo1.PeString.MainTitle = "Terahertz Amplitude";
            pesgo1.PeString.SubTitle = "";

            // So contour goes to edge of grid //
            pesgo1.PeGrid.Configure.AutoMinMaxPadding = 0;

            // Set small font size //
            pesgo1.PeFont.FontSize = FontSize.Small;
            pesgo1.PeFont.Fixed = true;

            // Disable appropriate tabs //
            pesgo1.PeUserInterface.Dialog.Axis = false;
            pesgo1.PeUserInterface.Dialog.Style = false;
            pesgo1.PeUserInterface.Dialog.Subsets = false;

            pesgo1.PeUserInterface.Scrollbar.ScrollingVertZoom = true;
            pesgo1.PeUserInterface.Scrollbar.ScrollingHorzZoom = true;

            pesgo1.PeConfigure.TextShadows = TextShadows.BoldText;
            pesgo1.PeFont.MainTitle.Bold = true;
            pesgo1.PeFont.SubTitle.Bold = true;
            pesgo1.PeFont.Label.Bold = true;
            pesgo1.PeFont.FontSize = FontSize.Medium;

            // Set various export defaults //
            pesgo1.PeSpecial.DpiX = 600;
            pesgo1.PeSpecial.DpiY = 600;

            // default export setting //
            pesgo1.PeUserInterface.Dialog.ExportSizeDef = ExportSizeDef.NoSizeOrPixel;
            pesgo1.PeUserInterface.Dialog.ExportTypeDef = ExportTypeDef.Png;
            pesgo1.PeUserInterface.Dialog.ExportDestDef = ExportDestDef.Clipboard;
            pesgo1.PeUserInterface.Dialog.ExportUnitXDef = "1280";
            pesgo1.PeUserInterface.Dialog.ExportUnitYDef = "768";
            pesgo1.PeUserInterface.Dialog.ExportImageDpi = 300;

            pesgo1.PeUserInterface.HotSpot.Data = true; // try both ways true and false;
            pesgo1.PeUserInterface.Cursor.PromptTracking = true;
            pesgo1.PeUserInterface.Cursor.PromptStyle = CursorPromptStyle.ZValue;

            // v9 features 
            pesgo1.PeUserInterface.Cursor.TrackingTooltipTitle = "Position";
            pesgo1.PeUserInterface.Cursor.PromptLocation = CursorPromptLocation.ToolTip;
            pesgo1.PeUserInterface.Cursor.TrackingCustomDataText = true;
            pesgo1.PeUserInterface.Cursor.Hand = (int)Gigasoft.ProEssentials.Enums.MouseCursorStyles.Arrow;

            // v9 features 
            pesgo1.PeColor.GraphBmpAlways = true;
            pesgo1.PeColor.GraphBackground = Color.Empty;
            pesgo1.PeColor.GraphBmpFilename = "usmid.jpg";
            pesgo1.PeColor.GraphBmpStyle = BitmapStyle.BitBltZooming;

            // v9 features 
            // optionally control the relationship between the zoomable background bitmap and the data units. 
            //pesgo1.PeGrid.Configure.GraphBmpMinX = 200.0F;
            //pesgo1.PeGrid.Configure.GraphBmpMaxX = 260.0F;
            //pesgo1.PeGrid.Configure.GraphBmpMinY = 100.0F;
            //pesgo1.PeGrid.Configure.GraphBmpMaxY = 160.0F;

            pesgo1.PeConfigure.AntiAliasGraphics = true;
            pesgo1.PeConfigure.AntiAliasText = true;

            pesgo1.PeConfigure.Composite2D3D = Composite2D3D.Foreground;
            pesgo1.PeConfigure.RenderEngine = RenderEngine.Direct3D;

            pesgo1.PeFunction.Force3dxNewColors = true;
            pesgo1.PeFunction.Force3dxVerticeRebuild = true;


            pesgo1.PeFunction.Reinitialize();
            pesgo1.PeFunction.ReinitializeResetImage();
            pesgo1.Invalidate();

        }

        private void pesgo1_PeCustomTrackingDataText(object sender, Gigasoft.ProEssentials.EventArg.CustomTrackingDataTextEventArgs e)
        {
            string s;
            s = String.Format("X : {0:0.00}  \n", pesgo1.PeUserInterface.Cursor.CursorValueX);
            s += String.Format("Y : {0:0.00} \n", pesgo1.PeUserInterface.Cursor.CursorValueY);
            s += String.Format("Z : {0:0.00} ", pesgo1.PeUserInterface.Cursor.CursorValueZ);
            e.TrackingText = s;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Random random = new Random();
            double value = random.Next(1, 100);
            trigger_signal_queue.Enqueue(value);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            ContourPlotUpdate(trigger_signal_queue, 30, 20, 1, 1);
        }
    }
}