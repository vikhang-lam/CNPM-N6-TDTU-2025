using System.Windows.Forms;

namespace N6
{
    partial class UC_BaoCao
    {
        private System.ComponentModel.IContainer components = null;

        // --- TẤT CẢ CÁC CONTROL ĐÃ ĐƯỢC KHAI BÁO LẠI ---
        private Label lblTitle;
        private Button btnXuatExcel;
        private Button btnXuatPDF;
        private ComboBox cboLoaiBaoCao;
        private ComboBox cboLop;
        private ComboBox cboHocKy;
        private ComboBox cboKhoi;
        private Button btnXuatBaoCao;
        private DataGridView dgvDuLieu;
        private TableLayoutPanel layoutRoot;
        private Panel pnlHeader;
        private FlowLayoutPanel pnlHeaderRight;
        private FlowLayoutPanel pnlFilters;
        private TabControl tabLoaiGiaoVien;
        private TabPage tabChuNhiem;
        private TabPage tabGiangDay;
        private ComboBox cboMonDay;
        private Label lblMonDay;
        private SplitContainer splitContainer1;
        private FlowLayoutPanel pnlCharts;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnXuatExcel = new System.Windows.Forms.Button();
            this.btnXuatPDF = new System.Windows.Forms.Button();
            this.cboLoaiBaoCao = new System.Windows.Forms.ComboBox();
            this.cboLop = new System.Windows.Forms.ComboBox();
            this.cboHocKy = new System.Windows.Forms.ComboBox();
            this.cboKhoi = new System.Windows.Forms.ComboBox();
            this.btnXuatBaoCao = new System.Windows.Forms.Button();
            this.dgvDuLieu = new System.Windows.Forms.DataGridView();
            this.layoutRoot = new System.Windows.Forms.TableLayoutPanel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlHeaderRight = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlFilters = new System.Windows.Forms.FlowLayoutPanel();
            this.tabLoaiGiaoVien = new System.Windows.Forms.TabControl();
            this.tabChuNhiem = new System.Windows.Forms.TabPage();
            this.tabGiangDay = new System.Windows.Forms.TabPage();
            this.lblMonDay = new System.Windows.Forms.Label();
            this.cboMonDay = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlCharts = new System.Windows.Forms.FlowLayoutPanel();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuLieu)).BeginInit();
            this.layoutRoot.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlHeaderRight.SuspendLayout();
            this.pnlFilters.SuspendLayout();
            this.tabLoaiGiaoVien.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlCharts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(422, 41);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "📑 BÁO CÁO & XUẤT DỮ LIỆU";
            // 
            // btnXuatExcel
            // 
            this.btnXuatExcel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnXuatExcel.FlatAppearance.BorderSize = 0;
            this.btnXuatExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXuatExcel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXuatExcel.ForeColor = System.Drawing.Color.White;
            this.btnXuatExcel.Location = new System.Drawing.Point(190, 5);
            this.btnXuatExcel.Margin = new System.Windows.Forms.Padding(10, 5, 0, 5);
            this.btnXuatExcel.Name = "btnXuatExcel";
            this.btnXuatExcel.Size = new System.Drawing.Size(150, 36);
            this.btnXuatExcel.TabIndex = 0;
            this.btnXuatExcel.Text = "📊 Xuất Excel";
            this.btnXuatExcel.UseVisualStyleBackColor = false;
            this.btnXuatExcel.Click += new System.EventHandler(this.btnXuatExcel_Click);
            // 
            // btnXuatPDF
            // 
            this.btnXuatPDF.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnXuatPDF.FlatAppearance.BorderSize = 0;
            this.btnXuatPDF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXuatPDF.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXuatPDF.ForeColor = System.Drawing.Color.White;
            this.btnXuatPDF.Location = new System.Drawing.Point(20, 5);
            this.btnXuatPDF.Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.btnXuatPDF.Name = "btnXuatPDF";
            this.btnXuatPDF.Size = new System.Drawing.Size(150, 36);
            this.btnXuatPDF.TabIndex = 1;
            this.btnXuatPDF.Text = "📄 Xuất PDF";
            this.btnXuatPDF.UseVisualStyleBackColor = false;
            this.btnXuatPDF.Click += new System.EventHandler(this.btnXuatPDF_Click);
            // 
            // cboLoaiBaoCao
            // 
            this.cboLoaiBaoCao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLoaiBaoCao.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboLoaiBaoCao.Items.AddRange(new object[] {
            "Báo cáo chuyên cần",
            "Bảng điểm học kỳ",
            "Hồ sơ học sinh",
            "Thống kê tổng hợp khối"});
            this.cboLoaiBaoCao.Location = new System.Drawing.Point(20, 15);
            this.cboLoaiBaoCao.Margin = new System.Windows.Forms.Padding(0, 5, 15, 5);
            this.cboLoaiBaoCao.Name = "cboLoaiBaoCao";
            this.cboLoaiBaoCao.Size = new System.Drawing.Size(230, 31);
            this.cboLoaiBaoCao.TabIndex = 0;
            // 
            // cboLop
            // 
            this.cboLop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLop.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboLop.Location = new System.Drawing.Point(430, 15);
            this.cboLop.Margin = new System.Windows.Forms.Padding(0, 5, 15, 5);
            this.cboLop.Name = "cboLop";
            this.cboLop.Size = new System.Drawing.Size(180, 31);
            this.cboLop.TabIndex = 2;
            this.cboLop.Visible = false;
            // 
            // cboHocKy
            // 
            this.cboHocKy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHocKy.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboHocKy.Location = new System.Drawing.Point(625, 15);
            this.cboHocKy.Margin = new System.Windows.Forms.Padding(0, 5, 15, 5);
            this.cboHocKy.Name = "cboHocKy";
            this.cboHocKy.Size = new System.Drawing.Size(130, 31);
            this.cboHocKy.TabIndex = 3;
            this.cboHocKy.Visible = false;
            // 
            // cboKhoi
            // 
            this.cboKhoi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKhoi.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboKhoi.Location = new System.Drawing.Point(265, 15);
            this.cboKhoi.Margin = new System.Windows.Forms.Padding(0, 5, 15, 5);
            this.cboKhoi.Name = "cboKhoi";
            this.cboKhoi.Size = new System.Drawing.Size(150, 31);
            this.cboKhoi.TabIndex = 1;
            this.cboKhoi.Visible = false;
            // 
            // btnXuatBaoCao
            // 
            this.btnXuatBaoCao.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(139)))), ((int)(((byte)(34)))));
            this.btnXuatBaoCao.FlatAppearance.BorderSize = 0;
            this.btnXuatBaoCao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXuatBaoCao.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXuatBaoCao.ForeColor = System.Drawing.Color.White;
            this.btnXuatBaoCao.Location = new System.Drawing.Point(770, 15);
            this.btnXuatBaoCao.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.btnXuatBaoCao.Name = "btnXuatBaoCao";
            this.btnXuatBaoCao.Size = new System.Drawing.Size(160, 36);
            this.btnXuatBaoCao.TabIndex = 4;
            this.btnXuatBaoCao.Text = "📋 Xuất Báo Cáo";
            this.btnXuatBaoCao.UseVisualStyleBackColor = false;
            this.btnXuatBaoCao.Click += new System.EventHandler(this.btnXuatBaoCao_Click);
            // 
            // dgvDuLieu
            // 
            this.dgvDuLieu.AllowUserToAddRows = false;
            this.dgvDuLieu.AllowUserToDeleteRows = false;
            this.dgvDuLieu.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDuLieu.BackgroundColor = System.Drawing.Color.White;
            this.dgvDuLieu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDuLieu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDuLieu.Location = new System.Drawing.Point(0, 0);
            this.dgvDuLieu.Name = "dgvDuLieu";
            this.dgvDuLieu.ReadOnly = true;
            this.dgvDuLieu.RowHeadersWidth = 51;
            this.dgvDuLieu.RowTemplate.Height = 24;
            this.dgvDuLieu.Size = new System.Drawing.Size(934, 215);
            this.dgvDuLieu.TabIndex = 2;
            // 
            // layoutRoot
            // 
            this.layoutRoot.ColumnCount = 1;
            this.layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutRoot.Controls.Add(this.pnlHeader, 0, 0);
            this.layoutRoot.Controls.Add(this.pnlFilters, 0, 2);
            this.layoutRoot.Controls.Add(this.tabLoaiGiaoVien, 0, 1);
            this.layoutRoot.Controls.Add(this.splitContainer1, 0, 3);
            this.layoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutRoot.Name = "layoutRoot";
            this.layoutRoot.RowCount = 4;
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutRoot.Size = new System.Drawing.Size(940, 600);
            this.layoutRoot.TabIndex = 0;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.pnlHeader.Controls.Add(this.pnlHeaderRight);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(3, 3);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.pnlHeader.Size = new System.Drawing.Size(934, 54);
            this.pnlHeader.TabIndex = 0;
            // 
            // pnlHeaderRight
            // 
            this.pnlHeaderRight.Controls.Add(this.btnXuatExcel);
            this.pnlHeaderRight.Controls.Add(this.btnXuatPDF);
            this.pnlHeaderRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlHeaderRight.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlHeaderRight.Location = new System.Drawing.Point(574, 10);
            this.pnlHeaderRight.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeaderRight.Name = "pnlHeaderRight";
            this.pnlHeaderRight.Size = new System.Drawing.Size(340, 34);
            this.pnlHeaderRight.TabIndex = 0;
            this.pnlHeaderRight.WrapContents = false;
            // 
            // pnlFilters
            // 
            this.pnlFilters.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(40)))), ((int)(((byte)(55)))));
            this.pnlFilters.Controls.Add(this.cboLoaiBaoCao);
            this.pnlFilters.Controls.Add(this.cboKhoi);
            this.pnlFilters.Controls.Add(this.cboLop);
            this.pnlFilters.Controls.Add(this.cboHocKy);
            this.pnlFilters.Controls.Add(this.lblMonDay);
            this.pnlFilters.Controls.Add(this.cboMonDay);
            this.pnlFilters.Controls.Add(this.btnXuatBaoCao);
            this.pnlFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFilters.Location = new System.Drawing.Point(3, 103);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.pnlFilters.Size = new System.Drawing.Size(934, 54);
            this.pnlFilters.TabIndex = 1;
            this.pnlFilters.WrapContents = false;
            // 
            // tabLoaiGiaoVien
            // 
            this.tabLoaiGiaoVien.Controls.Add(this.tabChuNhiem);
            this.tabLoaiGiaoVien.Controls.Add(this.tabGiangDay);
            this.tabLoaiGiaoVien.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabLoaiGiaoVien.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabLoaiGiaoVien.Location = new System.Drawing.Point(3, 63);
            this.tabLoaiGiaoVien.Name = "tabLoaiGiaoVien";
            this.tabLoaiGiaoVien.SelectedIndex = 0;
            this.tabLoaiGiaoVien.Size = new System.Drawing.Size(934, 34);
            this.tabLoaiGiaoVien.TabIndex = 3;
            this.tabLoaiGiaoVien.Visible = false;
            // 
            // tabChuNhiem
            // 
            this.tabChuNhiem.Location = new System.Drawing.Point(4, 29);
            this.tabChuNhiem.Name = "tabChuNhiem";
            this.tabChuNhiem.Padding = new System.Windows.Forms.Padding(3);
            this.tabChuNhiem.Size = new System.Drawing.Size(926, 1);
            this.tabChuNhiem.TabIndex = 0;
            this.tabChuNhiem.Text = "Lớp chủ nhiệm";
            this.tabChuNhiem.UseVisualStyleBackColor = true;
            // 
            // tabGiangDay
            // 
            this.tabGiangDay.Location = new System.Drawing.Point(4, 29);
            this.tabGiangDay.Name = "tabGiangDay";
            this.tabGiangDay.Padding = new System.Windows.Forms.Padding(3);
            this.tabGiangDay.Size = new System.Drawing.Size(926, 1);
            this.tabGiangDay.TabIndex = 1;
            this.tabGiangDay.Text = "Lớp giảng dạy";
            this.tabGiangDay.UseVisualStyleBackColor = true;
            // 
            // lblMonDay
            // 
            this.lblMonDay.AutoSize = true;
            this.lblMonDay.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblMonDay.ForeColor = System.Drawing.Color.White;
            this.lblMonDay.Location = new System.Drawing.Point(770, 15);
            this.lblMonDay.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.lblMonDay.Name = "lblMonDay";
            this.lblMonDay.Size = new System.Drawing.Size(53, 23);
            this.lblMonDay.TabIndex = 6;
            this.lblMonDay.Text = "Môn:";
            this.lblMonDay.Visible = false;
            // 
            // cboMonDay
            // 
            this.cboMonDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMonDay.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboMonDay.FormattingEnabled = true;
            this.cboMonDay.Location = new System.Drawing.Point(828, 15);
            this.cboMonDay.Margin = new System.Windows.Forms.Padding(0, 5, 15, 5);
            this.cboMonDay.Name = "cboMonDay";
            this.cboMonDay.Size = new System.Drawing.Size(121, 31);
            this.cboMonDay.TabIndex = 7;
            this.cboMonDay.Visible = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 163);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvDuLieu);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlCharts);
            this.splitContainer1.Size = new System.Drawing.Size(934, 434);
            this.splitContainer1.SplitterDistance = 215;
            this.splitContainer1.TabIndex = 4;
            // 
            // pnlCharts
            // 
            this.pnlCharts.AutoScroll = true;
            this.pnlCharts.BackColor = System.Drawing.Color.White;
            this.pnlCharts.Controls.Add(this.chart1);
            this.pnlCharts.Controls.Add(this.chart2);
            this.pnlCharts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCharts.Location = new System.Drawing.Point(0, 0);
            this.pnlCharts.Name = "pnlCharts";
            this.pnlCharts.Size = new System.Drawing.Size(934, 215);
            this.pnlCharts.TabIndex = 0;
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(3, 3);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(450, 200);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // chart2
            // 
            chartArea2.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart2.Legends.Add(legend2);
            this.chart2.Location = new System.Drawing.Point(459, 3);
            this.chart2.Name = "chart2";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart2.Series.Add(series2);
            this.chart2.Size = new System.Drawing.Size(450, 200);
            this.chart2.TabIndex = 1;
            this.chart2.Text = "chart2";
            // 
            // UC_BaoCao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.layoutRoot);
            this.Name = "UC_BaoCao";
            this.Size = new System.Drawing.Size(940, 600);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuLieu)).EndInit();
            this.layoutRoot.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlHeaderRight.ResumeLayout(false);
            this.pnlFilters.ResumeLayout(false);
            this.pnlFilters.PerformLayout();
            this.tabLoaiGiaoVien.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pnlCharts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);
        }
    }
}