using Gigasoft.ProEssentials;
using Gigasoft.ProEssentials.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace chart_example3
{


    public partial class ContourPlot : UserControl
    {
        public delegate void delEvent(object sender, EventArgs e);
        public event delEvent contourPlotEventSender;

        Queue<double> trigger_signal_queue = new Queue<double>();

        struct PositionInformation
        {
            int start_x, end_x;
            int start_y, end_y;
            double interval_x, interval_y;
        }


        int start_x, end_x;
        int start_y, end_y;
        double interval_x, interval_y;



        public ContourPlot()
        {
            InitializeComponent();
        }

        public void Graph_Load(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
        }

        private List<double> THz_values = new List<double>();

        private void Plot_Save()
        {

        }

        private void Plot_Load()
        {
            int o;
            int[] pElevData = new int[2250000];

            string FileName = "";

            string filepath = System.AppDomain.CurrentDomain.BaseDirectory + $"\\{FileName}.bin";
            try
            {
                FileStream fs = new FileStream(filepath, System.IO.FileMode.Open);
                BinaryReader data = new BinaryReader(fs);

                // StartBit

                for (o = 0; o < 2250000; o++)
                    pElevData[o] = data.ReadInt32();
                fs.Close();
            }
            catch
            {
                MessageBox.Show("Demo File Not Found?", "Error", MessageBoxButtons.OK);
                //Application.Exit();
            }



        }

        private void InitContourPlot(int start_x, int end_x, int start_y, int end_y, double interval_x, double interval_y)
        {
            int length_x = Math.Abs(start_x - end_x);
            int length_y = Math.Abs(start_y - end_y);
            int number_y, number_x, frame_size;

            number_x = Convert.ToInt32(length_x / interval_x) + 1;
            number_y = Convert.ToInt32(length_y / interval_y)+1;
            frame_size = number_x * number_y;
        }

        private void UpdateContourPlot(Queue<double> trigger_signal_queue, int start_x, int end_x, int start_y, int end_y, double interval_x, double interval_y)
        {
            while (trigger_signal_queue.Count > 0)
                THz_values.Add(trigger_signal_queue.Dequeue());

            if (pesgo1 == null) { return; }

            // 초기화: 아무것도 없는 새 인스턴스 초기 상태로
            pesgo1.PeFunction.Reset();

            // Modeless가 뭘까?
            pesgo1.PeUserInterface.Dialog.ModelessAutoClose = true;

            // 이미지 생성하지않고 속성(ManualMaxY, 등)만 읽음
            pesgo1.PeFunction.Reinitialize();

            // 이미지를 메모리에 준비시킴.(버퍼 읽거나 설정하는 것으로 예상)
            pesgo1.PeConfigure.PrepareImages = true;

            Random Rand_Num = new Random(unchecked((int)DateTime.Now.Ticks));

            // pre-condition : 가로(세로) 스크롤 막대가 활성화
            // 가로(세로)방향으로 차트를 클릭 및 잡고 끌 수 있는지 여부 설정
            pesgo1.PeUserInterface.Scrollbar.MouseDraggingX = true;
            pesgo1.PeUserInterface.Scrollbar.MouseDraggingY = true;


            int length_x = Math.Abs(start_x - end_x);
            int length_y = Math.Abs(start_y - end_y);
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
                        float f = (float)(THz_values[index] + index);
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


            //소수점 표현
            pesgo1.PeLegend.ContourLegendPrecision = ContourLegendPrecision.FourDecimals;

            /*********************************색깔 설정 *********************************/
            // v9 features, // optional method of setting sizes of contour color regions 
            //배열의 총합은 1이며 비율로 따짐
            pesgo1.PeColor.ContourColorProportions[0] = 0.2F;
            pesgo1.PeColor.ContourColorProportions[1] = 0.2F;
            pesgo1.PeColor.ContourColorProportions[2] = 0.2F;
            pesgo1.PeColor.ContourColorProportions[3] = 0.2F;
            pesgo1.PeColor.ContourColorProportions[4] = 0.2F;
            //pesgo1.PeColor.ContourColorProportions[5] = 0.1F;

            //색깔[0]과 색깔[1]간의 연결을 부드럽게 해줌
            pesgo1.PeColor.ContourColorBlends = 50;  // this must be set before COLORSET, COLORSET ALWAYS LAST 
            //투명도 조절
            pesgo1.PeColor.ContourColorAlpha = 200;
            pesgo1.PeColor.ContourColorSet = ContourColorSet.BlueGreenYellowOrangeRed;
            /***************************************************************************/

            // 변화없음. 뭔지 모르겠음
            pesgo1.PeGrid.Configure.AutoMinMaxPadding = 0;

            // For when ViewingStyle is monochrome //				
            for (s = 0; s < 30; s++)
                pesgo1.PeColor.SubsetShades[s] = Color.FromArgb(255, (byte)(50 + (s * 2)), (byte)(50 + (s * 2)), (byte)(50 + (s * 2)));

            /********************Z값 Legend 표시하기******************/
            pesgo1.PeLegend.ContourStyle = true;

            // 변화 없음. 
            pesgo1.PePlot.Option.GraphDataLabels = true;
            pesgo1.PePlot.Option.AllowDataLabels = AllowDataLabels.DataValues;

            // Set Various Other Properties ///
            // 데이터 뒤의 배경색 설정
            pesgo1.PeColor.BitmapGradientMode = true;
            pesgo1.PeColor.QuickStyle = QuickStyle.LightShadow;

            // User Control 외곽선
            pesgo1.PeConfigure.BorderTypes = TABorder.DropShadow;

            // Set the plotting method ///
            // 왼쪽 버튼 선택 시, 나오는 옵션 활성화
            pesgo1.PePlot.Allow.ContourColors = true;
            pesgo1.PePlot.Allow.ContourLines = true;
            pesgo1.PePlot.Allow.ContourColorsShadows = true;
            pesgo1.PePlot.Option.ContourLinesColored = true;

            /******ContourColors, ContourColorsShadows, 또는 ContourLines ******/
            pesgo1.PePlot.Method = SGraphPlottingMethod.ContourLines;

            // 왼쪽 버튼 선택시, 나오는 옵션 활성화
            pesgo1.PeUserInterface.Menu.DataShadow = MenuControl.Show;

            /***********************Legend 위치****************/
            pesgo1.PeLegend.Location = LegendLocation.Right;

            // 변화가 없음 (앞 또는 뒤에 배치할꺼냐 인듯)
            pesgo1.PeGrid.InFront = false;
            // 그리드 활성화
            pesgo1.PeGrid.LineControl = GridLineControl.Both;
            // 그리드 형태
            pesgo1.PeGrid.Style = GridStyle.Dot;

            // 중복됨
            pesgo1.PeConfigure.PrepareImages = true;

            /*********************************캐쉬 활성화 **********************/
            pesgo1.PeConfigure.CacheBmp = true;

            // 줌 설정
            pesgo1.PeUserInterface.Allow.ZoomStyle = ZoomStyle.Ro2Not;
            pesgo1.PeUserInterface.Allow.Zooming = AllowZooming.HorzAndVert;

            pesgo1.PeUserInterface.Scrollbar.MouseWheelFunction = MouseWheelFunction.HorizontalVerticalZoom;
            pesgo1.PeUserInterface.Scrollbar.MouseWheelZoomSmoothness = 4;
            pesgo1.PeUserInterface.Scrollbar.MouseWheelZoomFactor = 1.35F;

            // 변화 없음
            pesgo1.PeGrid.GridBands = false;

            /**********************************************/
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

            /******************************* 제목 *********************************/
            pesgo1.PeString.MainTitle = "Terahertz Contour Plot";
            pesgo1.PeString.SubTitle = "Amplitude";

            // So contour goes to edge of grid //
            /************************Plot되는 데이터 그래프 영역에 여백 만들기*******************/
            pesgo1.PeGrid.Configure.AutoMinMaxPadding = 0;

            /************************Annotation 폰트 사이즈 결정*********************/
            pesgo1.PeFont.FontSize = FontSize.Small;

            // True: 모든 컨트롤 폰트 크기 같게 함. False: 큰 컨트롤은 크게, 작은 컨트롤은 작게 
            pesgo1.PeFont.Fixed = true;

            // Disable appropriate tabs //
            // 변화 없음
            pesgo1.PeUserInterface.Dialog.Axis = false;
            pesgo1.PeUserInterface.Dialog.Style = false;
            pesgo1.PeUserInterface.Dialog.Subsets = false;

            // 유저인터페이스가 뭘까?
            pesgo1.PeUserInterface.Scrollbar.ScrollingVertZoom = true;
            pesgo1.PeUserInterface.Scrollbar.ScrollingHorzZoom = true;

            //변화 없음
            pesgo1.PeConfigure.TextShadows = TextShadows.NoShadows;
            // 폰트 굶기 변화//
            pesgo1.PeFont.MainTitle.Bold = true;
            pesgo1.PeFont.SubTitle.Bold = true;
            pesgo1.PeFont.Label.Bold = true;
            pesgo1.PeFont.FontSize = FontSize.Large;

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
            // 변화없음
            pesgo1.PeConfigure.AntiAliasGraphics = true;
            pesgo1.PeConfigure.AntiAliasText = true;

            // 렌더링 방법
            pesgo1.PeConfigure.Composite2D3D = Composite2D3D.Foreground;
            pesgo1.PeConfigure.RenderEngine = RenderEngine.Direct3D;
            pesgo1.PeFunction.Force3dxNewColors = true;
            pesgo1.PeFunction.Force3dxVerticeRebuild = true;


            pesgo1.PeFunction.Reinitialize();
            pesgo1.PeFunction.ReinitializeResetImage();
            pesgo1.Invalidate();
            //pesgo1.Update();
        }

        private void UpdateContour3DWirePlot(Queue<double> trigger_signal_queue, int start_x, int end_x, int start_y, int end_y, double interval_x, double interval_y)
        {
            while (trigger_signal_queue.Count > 0)
                THz_values.Add(trigger_signal_queue.Dequeue());

            if (pe3do1 == null) { return; }

            pe3do1.PeFunction.Reset();
            pe3do1.PeUserInterface.Dialog.ModelessAutoClose = true;


            // Tip, for charts with large amounts of data, (you may need to set HourGlassThreshold property smaller than 2M)
            // if you see the Wait Cursor flash twice in a row, you are un-necessarily building polygons twice. 

            Random Rand_Num = new Random(unchecked((int)DateTime.Now.Ticks));


            // Enable smooth rotating and zooming //
            pe3do1.PeUserInterface.Scrollbar.ScrollSmoothness = 4;  // v9.5
            pe3do1.PeUserInterface.Scrollbar.MouseWheelZoomSmoothness = 4;
            pe3do1.PeUserInterface.Scrollbar.PinchZoomSmoothness = 2;

            // Enable button dragging //
            pe3do1.PeUserInterface.Scrollbar.MouseDraggingX = true;
            pe3do1.PeUserInterface.Scrollbar.MouseDraggingY = true;

            // Do not auto fit to shape of window //
            pe3do1.PePlot.Option.DxFitControlShape = false;

            // When data is not square use GridAspectX or Z 
            pe3do1.PeGrid.Option.GridAspectX = 1.5F;
            pe3do1.PeGrid.Option.GridAspectZ = 1.0F;

            // Set eye/camera distance, or Zoom amount //
            pe3do1.PePlot.Option.DxZoom = -.75F;

            // Set camera position //
            pe3do1.PeUserInterface.Scrollbar.ViewingHeight = 26;
            pe3do1.PeUserInterface.Scrollbar.DegreeOfRotation = 232;

            // Set a light location //
            pe3do1.PeFunction.SetLight(0, 2.5F, -1.5F, .25F);

            // Enable DegreePrompting, to view rotation, zoom, light location to aid 
            // in determining different default values for such properties //
            pe3do1.PePlot.Option.DegreePrompting = true;
            pe3do1.PeUserInterface.RotationSpeed = 50;
            pe3do1.PeUserInterface.RotationIncrement = RotationIncrement.IncBy1;

            // Set PlottingMethod //
            pe3do1.PePlot.Method = ThreeDGraphPlottingMethod.Four;

            // Pass Data //
            //pe3do1.PeGrid.Configure.XAxisScaleControl = Gigasoft.ProEssentials.Enums.ScaleControl.Log;
            pe3do1.PeString.XAxisLabel = "X axis [mm]";
            pe3do1.PeString.ZAxisLabel = "Y axis [mm]";



            int length_x = Math.Abs(start_x - end_x);
            int length_y = Math.Abs(start_y - end_y);
            int number_y, number_x, frame_size;

            number_x = Convert.ToInt32(length_x  / interval_x) + 1;
            number_y = Convert.ToInt32(length_y / interval_y) + 1;
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
                        float f = (float)(THz_values[index] + index);
                        pMyZData[index] = f;
                    }
                }
            }

            pesgo1.PeData.NullDataValueZ = 0.0F;

            pesgo1.PeData.Subsets = number_y; // y
            pesgo1.PeData.Points = number_x; //x

            // Perform the transfer of data //
            Gigasoft.ProEssentials.Api.PEvsetW(pe3do1.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.XData, pMyXData, frame_size);
            Gigasoft.ProEssentials.Api.PEvsetW(pe3do1.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.YData, pMyYData, frame_size);
            Gigasoft.ProEssentials.Api.PEvsetW(pe3do1.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.ZData, pMyZData, frame_size);

            // Enable smooth rotating and zooming //
            pe3do1.PeUserInterface.Scrollbar.ScrollSmoothness = 3;  // v9.5
            pe3do1.PeUserInterface.Scrollbar.MouseWheelZoomSmoothness = 3;
            pe3do1.PeUserInterface.Scrollbar.PinchZoomSmoothness = 2;

            // Enable DegreePrompting, to view rotation, zoom, light location to aid 
            // in determining different default values for such properties //
            pe3do1.PePlot.Option.DegreePrompting = true;

            // Enable button dragging //
            pe3do1.PeUserInterface.Scrollbar.MouseDraggingX = true;
            pe3do1.PeUserInterface.Scrollbar.MouseDraggingY = true;

            // Set view
            pe3do1.PeUserInterface.Scrollbar.ViewingHeight = 16;
            pe3do1.PeUserInterface.Scrollbar.DegreeOfRotation = 196;  // v9.5

            // Set a light location //
            pe3do1.PeFunction.SetLight(0, -2.2F, -7.30F, 8.3F);  // v9.5

            // Set eye/camera distance, or Zoom amount //
            pe3do1.PePlot.Option.DxZoom = -1.20F;   // v9.5
            pe3do1.PePlot.Option.DxFitControlShape = false;
            pe3do1.PeGrid.Option.GridAspectX = 2.0F;
            pe3do1.PeGrid.Option.GridAspectZ = 2.0F;
            pe3do1.PePlot.Option.DxViewportY = 0.7F;

            pe3do1.PeUserInterface.RotationSpeed = 50;
            pe3do1.PeUserInterface.RotationIncrement = RotationIncrement.IncBy1;

            // Mechanism to control polygon border color //
            pe3do1.PeColor.SubsetColors[(int)(SurfaceColors.WireFrame)] = Color.FromArgb(255, 225, 225, 225);
            pe3do1.PeColor.SubsetColors[(int)(SurfaceColors.SolidSurface)] = Color.FromArgb(255, 159, 159, 159);

            // v9 feature
            pe3do1.PeColor.ContourColorSet = ContourColorSet.BlueCyanGreenYellowBrownWhite;
            pe3do1.PeLegend.Location = LegendLocation.Left;
            pe3do1.PeLegend.ContourStyle = true;
            pe3do1.PeLegend.ContourLegendPrecision = ContourLegendPrecision.ZeroDecimals;
            pe3do1.PePlot.Option.ShowWireFrame = true;

            // Set the plotting method //
            //! There are different plotting method values for each //
            //! case of PolyMode  //
            pe3do1.PePlot.Method = ThreeDGraphPlottingMethod.Zero;
            pe3do1.PePlot.Allow.SurfaceContour = true;

            // Set various other properties //
            pe3do1.PeString.MainTitle = "";

            pe3do1.PeConfigure.RenderEngine = RenderEngine.Direct3D;
            pe3do1.PeString.SubTitle = "||Mouse Wheel zooms. Mouse Drag rotates. Mouse Drag+SHIFT shifts image. ";
            pe3do1.PeString.MultiSubTitles[0] = "||Double Click start/stop Rotation. Mouse Drag+MIDDLE rotates light. ";
            pe3do1.PeColor.BitmapGradientMode = true;

            // Note RenderEngine 3DX requires setting RenderEngine before QuickStyle 
            pe3do1.PeColor.QuickStyle = QuickStyle.MediumInset;  // v9.5
                                                                 // Add some padding around chart for prettiness if using inset or shadow borders 
            pe3do1.PeConfigure.ImageAdjustLeft = 100;
            pe3do1.PeConfigure.ImageAdjustRight = 100;
            pe3do1.PeConfigure.ImageAdjustTop = 50;
            pe3do1.PeConfigure.ImageAdjustBottom = 50;

            pe3do1.PeFont.Fixed = true;
            pe3do1.PeFont.FontSize = FontSize.Medium;
            pe3do1.PeConfigure.PrepareImages = true;
            pe3do1.PeConfigure.CacheBmp = true;
            pe3do1.PeUserInterface.Allow.FocalRect = false;

            pe3do1.PeConfigure.TextShadows = TextShadows.BoldText;
            pe3do1.PeFont.Label.Bold = true;

            // Set various export defaults //
            pe3do1.PeSpecial.DpiX = 600;
            pe3do1.PeSpecial.DpiY = 600;

            // default export setting //
            pe3do1.PeUserInterface.Dialog.ExportSizeDef = ExportSizeDef.NoSizeOrPixel;
            pe3do1.PeUserInterface.Dialog.ExportTypeDef = ExportTypeDef.Png;
            pe3do1.PeUserInterface.Dialog.ExportDestDef = ExportDestDef.Clipboard;
            pe3do1.PeUserInterface.Dialog.ExportUnitXDef = "1280";
            pe3do1.PeUserInterface.Dialog.ExportUnitYDef = "768";
            pe3do1.PeUserInterface.Dialog.ExportImageDpi = 300;
            pe3do1.PeUserInterface.Dialog.AllowEmfExport = false;
            pe3do1.PeUserInterface.Dialog.AllowSvgExport = false;
            pe3do1.PeUserInterface.Dialog.AllowWmfExport = false;

            pe3do1.PeConfigure.AntiAliasGraphics = true;
            pe3do1.PeConfigure.AntiAliasText = true;

            // Enable CursorPromptTracking HighlighColor, etc only applicable to Direct3D RenderEngine
            pe3do1.PeUserInterface.HotSpot.Data = true;
            pe3do1.PeUserInterface.Cursor.PromptTracking = true;
            pe3do1.PeUserInterface.Cursor.PromptStyle = CursorPromptStyle.YValue; // only y and xyz are options.
            pe3do1.PeData.Precision = DataPrecision.TwoDecimals;
            pe3do1.PeUserInterface.Cursor.HighlightColor = Color.FromArgb(255, 255, 0, 0);
            pe3do1.PePlot.Option.ShowContour = ShowContour.BottomColors;
            pe3do1.PeUserInterface.Menu.DataShadow = MenuControl.Show;

            pe3do1.PeUserInterface.Menu.AnnotationControl = true;  // v9.5 
            pe3do1.PeUserInterface.Menu.ShowAnnotationText = MenuControl.Show;
            pe3do1.PeUserInterface.Menu.AnnotationTextFixedSize = MenuControl.Show;

            // Add some random graph annotations //  // v9.5 
            //int aCnt = 0;
            //for (int annot = 0; annot < 10; annot++)
            //{
            //    nRndRow = (int)(Rand_Num.NextDouble() * (float)nTargetRows - 1);
            //    nRndCol = (int)(Rand_Num.NextDouble() * (float)nTargetCols - 1);
            //    pe3do1.PeAnnotation.Graph.X[aCnt] = nStartCol + nRndCol + 1;
            //    pe3do1.PeAnnotation.Graph.Z[aCnt] = nStartRow + nRndRow + 1;
            //    pe3do1.PeAnnotation.Graph.Y[aCnt] = pe3do1.PeData.Y[nRndRow, nRndCol];
            //    pe3do1.PeAnnotation.Graph.Type[aCnt] = (int)Gigasoft.ProEssentials.Enums.GraphAnnotationType.LargeDotSolid;
            //    pe3do1.PeAnnotation.Graph.Text[aCnt] = "";
            //    pe3do1.PeAnnotation.Graph.Color[aCnt] = Color.FromArgb(255, 0, 255, 0);
            //    MainWindow.m_nAnnotationStartIndices[annot] = aCnt;
            //    aCnt++;
            //    pe3do1.PeAnnotation.Graph.X[aCnt] = nStartCol + nRndCol + 1;
            //    pe3do1.PeAnnotation.Graph.Z[aCnt] = nStartRow + nRndRow + 1;
            //    pe3do1.PeAnnotation.Graph.Y[aCnt] = pe3do1.PeData.Y[nRndRow, nRndCol];
            //    pe3do1.PeAnnotation.Graph.Type[aCnt] = (int)Gigasoft.ProEssentials.Enums.GraphAnnotationType.Pointer;
            //    pe3do1.PeAnnotation.Graph.Text[aCnt] = "|lAnnotation " + annot.ToString();
            //    pe3do1.PeAnnotation.Graph.Color[aCnt] = Color.FromArgb(255, 0, 255, 0);
            //    aCnt++;
            //}
            //pe3do1.PeAnnotation.Show = true;
            //pe3do1.PeAnnotation.Graph.Show = true;
            //pe3do1.PeAnnotation.Graph.LeftJustificationOutside = true;
            //pe3do1.PeAnnotation.Graph.SymbolObstacles = true;
            //pe3do1.PeFont.GraphAnnotationTextSize = 110;
            //pe3do1.PeUserInterface.HotSpot.GraphAnnotation = false;

            pe3do1.PeUserInterface.Menu.CustomMenuText[0] = "Zoom Rotate on Center ";
            pe3do1.PeUserInterface.Menu.CustomMenuState[0, 0] = CustomMenuState.Checked;
            pe3do1.PeUserInterface.Menu.CustomMenuLocation[0] = CustomMenuLocation.AboveSeparator;
            pe3do1.PeString.MultiSubTitles[1] = "Key 0-9 rotates/zooms at annotation| |Popup Menu size/hide annotation text.";

            // Set a default ViewingAt location.
            float X, Y, Z;
            X = (float)pe3do1.PeAnnotation.Graph.X[0];
            Y = (float)pe3do1.PeAnnotation.Graph.Y[0];
            Z = (float)pe3do1.PeAnnotation.Graph.Z[0];
            pe3do1.PeFunction.SetViewingAt(X, Y, Z);

            pe3do1.PeFunction.Force3dxVerticeRebuild = true;
            pe3do1.PeFunction.Force3dxAnnotVerticeRebuild = true;

            pe3do1.PeFunction.ReinitializeResetImage();
            pe3do1.Invalidate();
            pe3do1.Refresh();

        }


        private void UpdateContour3DPlot(Queue<double> trigger_signal_queue, int start_x, int end_x, int start_y, int end_y, double interval_x, double interval_y)
        {
            while (trigger_signal_queue.Count > 0)
                THz_values.Add(trigger_signal_queue.Dequeue());

            if (pe3do1 == null) { return; }

            pe3do1.PeFunction.Reset();
            pe3do1.PeUserInterface.Dialog.ModelessAutoClose = true;


            // Tip, for charts with large amounts of data, (you may need to set HourGlassThreshold property smaller than 2M)
            // if you see the Wait Cursor flash twice in a row, you are un-necessarily building polygons twice. 

            Random Rand_Num = new Random(unchecked((int)DateTime.Now.Ticks));


            // Enable smooth rotating and zooming //
            pe3do1.PeUserInterface.Scrollbar.ScrollSmoothness = 4;  // v9.5
            pe3do1.PeUserInterface.Scrollbar.MouseWheelZoomSmoothness = 4;
            pe3do1.PeUserInterface.Scrollbar.PinchZoomSmoothness = 2;

            // Enable button dragging //
            pe3do1.PeUserInterface.Scrollbar.MouseDraggingX = true;
            pe3do1.PeUserInterface.Scrollbar.MouseDraggingY = true;

            // Do not auto fit to shape of window //
            pe3do1.PePlot.Option.DxFitControlShape = false;

            // When data is not square use GridAspectX or Z 
            pe3do1.PeGrid.Option.GridAspectX = 1.5F;
            pe3do1.PeGrid.Option.GridAspectZ = 1.0F;

            // Set eye/camera distance, or Zoom amount //
            pe3do1.PePlot.Option.DxZoom = -.75F;

            // Set camera position //
            pe3do1.PeUserInterface.Scrollbar.ViewingHeight = 26;
            pe3do1.PeUserInterface.Scrollbar.DegreeOfRotation = 232;

            // Set a light location //
            pe3do1.PeFunction.SetLight(0, 2.5F, -1.5F, .25F);

            // Enable DegreePrompting, to view rotation, zoom, light location to aid 
            // in determining different default values for such properties //
            pe3do1.PePlot.Option.DegreePrompting = true;
            pe3do1.PeUserInterface.RotationSpeed = 50;
            pe3do1.PeUserInterface.RotationIncrement = RotationIncrement.IncBy1;

            // Set PlottingMethod //
            pe3do1.PePlot.Method = ThreeDGraphPlottingMethod.Four;

            // Pass Data //
            //pe3do1.PeGrid.Configure.XAxisScaleControl = Gigasoft.ProEssentials.Enums.ScaleControl.Log;
            pe3do1.PeString.XAxisLabel = "X axis [mm]";
            pe3do1.PeString.ZAxisLabel = "Y axis [mm]";
            pe3do1.PeString.ZAxisLabel = "THz phase";


            int length_x = Math.Abs(start_x - end_x);
            int length_y = Math.Abs(start_y - end_y);
            int number_y, number_x, frame_size;

            number_x = Convert.ToInt32(length_x / interval_x) + 1;
            number_y = Convert.ToInt32(length_y / interval_y)+1;
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

            // Perform the transfer of data //
            Gigasoft.ProEssentials.Api.PEvsetW(pe3do1.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.XData, pMyXData, frame_size);
            Gigasoft.ProEssentials.Api.PEvsetW(pe3do1.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.YData, pMyYData, frame_size);
            Gigasoft.ProEssentials.Api.PEvsetW(pe3do1.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.ZData, pMyZData, frame_size);

            pe3do1.PeData.NullDataValue = 0.0F;

            pe3do1.PeLegend.Location = LegendLocation.Left;
            pe3do1.PeLegend.Show = true;
            pe3do1.PeUserInterface.Menu.LegendLocation = MenuControl.Show;

            // v9 features
            pe3do1.PeColor.ContourColorProportions[0] = .1F;
            pe3do1.PeColor.ContourColorProportions[1] = .05F;
            pe3do1.PeColor.ContourColorProportions[2] = .1F;
            pe3do1.PeColor.ContourColorProportions[3] = .63F;
            pe3do1.PeColor.ContourColorProportions[4] = .06F;
            pe3do1.PeColor.ContourColorProportions[5] = .06F;

            pe3do1.PeColor.ContourColorBlends = 0;  // this must be set before COLORSET, COLORSET ALWAYS LAST 
            pe3do1.PeColor.ContourColorSet = ContourColorSet.BlueCyanGreenYellowBrownWhite;

            pe3do1.PePlot.Option.ShowContour = ShowContour.BottomLines;
            pe3do1.PePlot.Option.ShowWireFrame = true;
            pe3do1.PeColor.BarBorderColor = Color.FromArgb(85, 0, 0, 0);

            pe3do1.PeGrid.Configure.AutoMinMaxPadding = 0;
            pe3do1.PeData.SurfaceNullDataGaps = true;
            pe3do1.PeLegend.ContourStyle = true;

            // Set various other properties //
            pe3do1.PeFont.Fixed = true;

            pe3do1.PeString.MainTitle = "";
            pe3do1.PeString.SubTitle = "||Mouse Wheel zooms. Mouse Drag rotates. Mouse Drag+SHIFT shifts image. ";
            //pe3do1.PeString.MultiSubTitles[0] = "||Double Click start/stop Rotation. Mouse Drag+MIDDLE rotates light. ";

            pe3do1.PeConfigure.RenderEngine = RenderEngine.Direct3D;
            pe3do1.PeColor.BitmapGradientMode = true;
            pe3do1.PeColor.QuickStyle = QuickStyle.LightNoBorder;

            pe3do1.PeFont.FontSize = Gigasoft.ProEssentials.Enums.FontSize.Large;
            pe3do1.PeFont.SizeGlobalCntl = 1.1F;
            pe3do1.PeLegend.ContourLegendPrecision = ContourLegendPrecision.ZeroDecimals;

            pe3do1.PeConfigure.PrepareImages = true;
            pe3do1.PeConfigure.CacheBmp = true;
            pe3do1.PeUserInterface.Allow.FocalRect = false;

            pe3do1.PeColor.SubsetColors[(int)(SurfaceColors.WireFrame)] = Color.FromArgb(255, 198, 0, 0);
            pe3do1.PeColor.SubsetColors[(int)(SurfaceColors.SolidSurface)] = Color.FromArgb(255, 0, 148, 0);

            // Add Some Padding around image //
            pe3do1.PeConfigure.ImageAdjustLeft = 100;
            pe3do1.PeConfigure.ImageAdjustRight = 100;
            pe3do1.PeConfigure.ImageAdjustBottom = 100;

            pe3do1.PeConfigure.TextShadows = TextShadows.BoldText;
            pe3do1.PeFont.Label.Bold = true;

            // Set various export defaults //
            pe3do1.PeSpecial.DpiX = 600;
            pe3do1.PeSpecial.DpiY = 600;

            // default export setting //
            pe3do1.PeUserInterface.Dialog.ExportSizeDef = ExportSizeDef.NoSizeOrPixel;
            pe3do1.PeUserInterface.Dialog.ExportTypeDef = ExportTypeDef.Png;
            pe3do1.PeUserInterface.Dialog.ExportDestDef = ExportDestDef.Clipboard;
            pe3do1.PeUserInterface.Dialog.ExportUnitXDef = "1280";
            pe3do1.PeUserInterface.Dialog.ExportUnitYDef = "768";
            pe3do1.PeUserInterface.Dialog.ExportImageDpi = 300;
            pe3do1.PeUserInterface.Dialog.AllowEmfExport = false;
            pe3do1.PeUserInterface.Dialog.AllowWmfExport = false;
            pe3do1.PeUserInterface.Dialog.AllowSvgExport = false;

            pe3do1.PeData.Precision = DataPrecision.FourDecimals;

            // Enable CursorPromptTracking HighlighColor, etc only applicable to Direct3D RenderEngine
            pe3do1.PeUserInterface.Cursor.PromptTracking = true;
            pe3do1.PeUserInterface.Cursor.PromptStyle = CursorPromptStyle.YValue; // only y and xyz are options.
            pe3do1.PeUserInterface.Cursor.TrackingCustomDataText = true;
            pe3do1.PeUserInterface.Cursor.PromptLocation = CursorPromptLocation.Text;

            pe3do1.PeUserInterface.Cursor.HighlightColor = Color.FromArgb(255, 255, 0, 0);
            pe3do1.PeUserInterface.HotSpot.Data = true; // true provides hotspot data 

            pe3do1.PeUserInterface.Menu.DataShadow = MenuControl.Show;

            pe3do1.PeUserInterface.Menu.AnnotationControl = true;  // v9.5 
            pe3do1.PeUserInterface.Menu.ShowAnnotationText = MenuControl.Show;
            pe3do1.PeUserInterface.Menu.AnnotationTextFixedSize = MenuControl.Show;

            pe3do1.PeConfigure.TextShadows = TextShadows.BoldText;

            // Add some random graph annotations //  // v9.5 
            //int aCnt = 0;
            //for (int annot = 0; annot < 10; annot++)
            //{
            //    nRndRow = (int)(Rand_Num.NextDouble() * (float)nTargetRows - 1);
            //    nRndCol = (int)(Rand_Num.NextDouble() * (float)nTargetCols - 1);
            //    float f = pe3do1.PeData.X[nRndRow, nRndCol];
            //    pe3do1.PeAnnotation.Graph.X[aCnt] = f;
            //    pe3do1.PeAnnotation.Graph.Z[aCnt] = nStartRow + nRndRow + 1;
            //    pe3do1.PeAnnotation.Graph.Y[aCnt] = pe3do1.PeData.Y[nRndRow, nRndCol];
            //    pe3do1.PeAnnotation.Graph.Type[aCnt] = (int)Gigasoft.ProEssentials.Enums.GraphAnnotationType.LargeDotSolid;
            //    pe3do1.PeAnnotation.Graph.Text[aCnt] = "";
            //    pe3do1.PeAnnotation.Graph.Color[aCnt] = Color.FromArgb(255, 0, 255, 0);
            //    aCnt++;
            //    pe3do1.PeAnnotation.Graph.X[aCnt] = f;
            //    pe3do1.PeAnnotation.Graph.Z[aCnt] = nStartRow + nRndRow + 1;
            //    pe3do1.PeAnnotation.Graph.Y[aCnt] = pe3do1.PeData.Y[nRndRow, nRndCol];
            //    pe3do1.PeAnnotation.Graph.Type[aCnt] = (int)Gigasoft.ProEssentials.Enums.GraphAnnotationType.Pointer;
            //    pe3do1.PeAnnotation.Graph.Text[aCnt] = "|lAnnotation " + annot.ToString();
            //    pe3do1.PeAnnotation.Graph.Color[aCnt] = Color.FromArgb(255, 0, 255, 0);
            //    aCnt++;
            //}
            //pe3do1.PeAnnotation.Show = true;
            //pe3do1.PeAnnotation.Graph.Show = true;
            //pe3do1.PeAnnotation.Graph.LeftJustificationOutside = true;
            //pe3do1.PeAnnotation.Graph.SymbolObstacles = true;
            //pe3do1.PeFont.GraphAnnotationTextSize = 80;
            //pe3do1.PeUserInterface.HotSpot.GraphAnnotation = true;
            //pe3do1.PePlot.Option.DxSphereComplexity = 12;
            //pe3do1.PeAnnotation.Graph.SizeCntl = .7f;

            pe3do1.PeUserInterface.Menu.CustomMenuText[0] = "Zoom Rotate on Center ";
            pe3do1.PeUserInterface.Menu.CustomMenuState[0, 0] = CustomMenuState.Checked;
            pe3do1.PeUserInterface.Menu.CustomMenuLocation[0] = CustomMenuLocation.AboveSeparator;
            pe3do1.PeString.MultiSubTitles[1] = "Key 0-9 rotates/zooms at annotation| |Popup Menu size/hide annotation text.";

            // Set a default ViewingAt location.
            float X, Y, Z;
            X = (float)pe3do1.PeAnnotation.Graph.X[0];
            Y = (float)pe3do1.PeAnnotation.Graph.Y[0];
            Z = (float)pe3do1.PeAnnotation.Graph.Z[0];
            pe3do1.PeFunction.SetViewingAt(X, Y, Z);

            pe3do1.PeConfigure.RenderEngine = RenderEngine.Direct3D;

            pe3do1.PeFunction.Force3dxVerticeRebuild = true;
            pe3do1.PeFunction.Force3dxAnnotVerticeRebuild = true;

            pe3do1.PeFunction.ReinitializeResetImage();
            pe3do1.Invalidate();
            pe3do1.Refresh();

            /*
            // PeCustomTrackingDataText event handler //
             private void pe3do1_PeCustomTrackingDataText(object sender, Gigasoft.ProEssentials.EventArg.CustomTrackingDataTextEventArgs e)
             {
                double dY = pe3do1.PeUserInterface.Cursor.CursorValueY;
                e.TrackingText = String.Format("Surface with Plateaus:\n{0:0.000}", dY);
             }

            // PeDataHotSpot event handler //
             private void pe3do1_PeDataHotSpot(object sender, Gigasoft.ProEssentials.EventArg.DataHotSpotEventArgs e)
             { 
                this.Text = "DataPoint value " + pe3do1.PeData.Y[e.SubsetIndex, e.PointIndex].ToString() + " s=" + e.SubsetIndex.ToString() + " p=" + e.PointIndex.ToString();
             }
            */

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
            UpdateContourPlot(trigger_signal_queue, 0, 10, 0, 10, 1, 1);
            UpdateContour3DWirePlot(trigger_signal_queue, 0, 10, 0, 10, 1, 1);

        }





        private void pesgo1_Click(object sender, EventArgs e)
        {

        }


    }
}
