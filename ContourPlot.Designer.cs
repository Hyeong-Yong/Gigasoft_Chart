namespace chart_example3
{
    partial class ContourPlot
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pesgo1 = new Gigasoft.ProEssentials.Pesgo();
            timer1 = new System.Windows.Forms.Timer(components);
            timer2 = new System.Windows.Forms.Timer(components);
            pe3do1 = new Gigasoft.ProEssentials.Pe3do();
            btn_3DPlot = new Button();
            SuspendLayout();
            // 
            // pesgo1
            // 
            pesgo1.Location = new Point(845, 56);
            pesgo1.Name = "pesgo1";
            pesgo1.Size = new Size(372, 455);
            pesgo1.TabIndex = 0;
            pesgo1.Text = "pesgo1";
            pesgo1.PeCustomTrackingDataText += pesgo1_PeCustomTrackingDataText;
            pesgo1.Click += pesgo1_Click;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // timer2
            // 
            timer2.Interval = 3000;
            timer2.Tick += timer2_Tick;
            // 
            // pe3do1
            // 
            pe3do1.Location = new Point(58, 32);
            pe3do1.Name = "pe3do1";
            pe3do1.Size = new Size(687, 539);
            pe3do1.TabIndex = 1;
            pe3do1.Text = "pe3do1";
            // 
            // btn_3DPlot
            // 
            btn_3DPlot.Location = new Point(620, 577);
            btn_3DPlot.Name = "btn_3DPlot";
            btn_3DPlot.Size = new Size(94, 29);
            btn_3DPlot.TabIndex = 2;
            btn_3DPlot.Text = "LOAD";
            btn_3DPlot.UseVisualStyleBackColor = true;
            btn_3DPlot.Click += btn_3DPlot_Click;
            // 
            // ContourPlot
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btn_3DPlot);
            Controls.Add(pe3do1);
            Controls.Add(pesgo1);
            Name = "ContourPlot";
            Size = new Size(1350, 632);
            Load += Graph_Load;
            ResumeLayout(false);
        }

        #endregion

        private Gigasoft.ProEssentials.Pesgo pesgo1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private Gigasoft.ProEssentials.Pe3do pe3do1;
        private Button btn_3DPlot;
    }
}
