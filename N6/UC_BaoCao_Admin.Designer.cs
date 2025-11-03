namespace N6
{
    partial class UC_BaoCao_Admin
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlFilters = new System.Windows.Forms.Panel();
            this.flpFilters = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlReportTypeSelector = new System.Windows.Forms.FlowLayoutPanel();
            this.rbChuyenCan = new System.Windows.Forms.RadioButton();
            this.rbBangDiem = new System.Windows.Forms.RadioButton();
            this.rbHoSo = new System.Windows.Forms.RadioButton();
            this.rbThongKeKhoi = new System.Windows.Forms.RadioButton();
            this.rbBaoCaoThang = new System.Windows.Forms.RadioButton(); // <-- THÊM MỚI
            this.pnlKhoi = new System.Windows.Forms.FlowLayoutPanel();
            this.lblKhoi = new System.Windows.Forms.Label();
            this.cboKhoi = new System.Windows.Forms.ComboBox();
            this.pnlLop = new System.Windows.Forms.FlowLayoutPanel();
            this.lblLop = new System.Windows.Forms.Label();
            this.cboLop = new System.Windows.Forms.ComboBox();
            this.pnlHocKy = new System.Windows.Forms.FlowLayoutPanel();
            this.lblHocKy = new System.Windows.Forms.Label();
            this.cboHocKy = new System.Windows.Forms.ComboBox();
            this.pnlMon = new System.Windows.Forms.FlowLayoutPanel();
            this.lblMonDay = new System.Windows.Forms.Label();
            this.cboMonDay = new System.Windows.Forms.ComboBox();
            this.pnlThang = new System.Windows.Forms.FlowLayoutPanel(); // <-- THÊM MỚI
            this.lblThang = new System.Windows.Forms.Label(); // <-- THÊM MỚI
            this.cboThang = new System.Windows.Forms.ComboBox(); // <-- THÊM MỚI
            this.btnXuatPDF = new System.Windows.Forms.Button();
            this.btnXuatExcel = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvDuLieu = new System.Windows.Forms.DataGridView();
            this.flpCharts = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlFilters.SuspendLayout();
            this.flpFilters.SuspendLayout();
            this.pnlReportTypeSelector.SuspendLayout();
            this.pnlKhoi.SuspendLayout();
            this.pnlLop.SuspendLayout();
            this.pnlHocKy.SuspendLayout();
            this.pnlMon.SuspendLayout();
            this.pnlThang.SuspendLayout(); // <-- THÊM MỚI
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuLieu)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlFilters
            // 
            this.pnlFilters.BackColor = System.Drawing.Color.White;
            this.pnlFilters.Controls.Add(this.flpFilters);
            this.pnlFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilters.Location = new System.Drawing.Point(10, 10);
            this.pnlFilters.MinimumSize = new System.Drawing.Size(0, 130);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Padding = new System.Windows.Forms.Padding(10);
            this.pnlFilters.Size = new System.Drawing.Size(980, 130);
            this.pnlFilters.TabIndex = 0;
            this.pnlFilters.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlFilters_Paint);
            // 
            // flpFilters
            // 
            this.flpFilters.AutoScroll = true;
            this.flpFilters.Controls.Add(this.pnlReportTypeSelector);
            this.flpFilters.Controls.Add(this.pnlKhoi);
            this.flpFilters.Controls.Add(this.pnlLop);
            this.flpFilters.Controls.Add(this.pnlHocKy);
            this.flpFilters.Controls.Add(this.pnlMon);
            this.flpFilters.Controls.Add(this.pnlThang); // <-- THÊM MỚI
            this.flpFilters.Controls.Add(this.btnXuatPDF);
            this.flpFilters.Controls.Add(this.btnXuatExcel);
            this.flpFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpFilters.Location = new System.Drawing.Point(10, 10);
            this.flpFilters.Name = "flpFilters";
            this.flpFilters.Size = new System.Drawing.Size(960, 110);
            this.flpFilters.TabIndex = 1;
            // 
            // pnlReportTypeSelector
            // 
            this.pnlReportTypeSelector.AutoSize = true;
            this.pnlReportTypeSelector.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlReportTypeSelector.Controls.Add(this.rbChuyenCan);
            this.pnlReportTypeSelector.Controls.Add(this.rbBangDiem);
            this.pnlReportTypeSelector.Controls.Add(this.rbHoSo);
            this.pnlReportTypeSelector.Controls.Add(this.rbThongKeKhoi);
            this.pnlReportTypeSelector.Controls.Add(this.rbBaoCaoThang); // <-- THÊM MỚI
            this.pnlReportTypeSelector.Location = new System.Drawing.Point(3, 3);
            this.pnlReportTypeSelector.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.pnlReportTypeSelector.Name = "pnlReportTypeSelector";
            this.pnlReportTypeSelector.Padding = new System.Windows.Forms.Padding(5);
            this.pnlReportTypeSelector.Size = new System.Drawing.Size(818, 49); // <-- CẬP NHẬT KÍCH THƯỚC
            this.pnlReportTypeSelector.TabIndex = 6;
            this.pnlReportTypeSelector.WrapContents = false;
            // 
            // rbChuyenCan
            // 
            this.rbChuyenCan.Location = new System.Drawing.Point(8, 8);
            this.rbChuyenCan.Name = "rbChuyenCan";
            this.rbChuyenCan.Size = new System.Drawing.Size(150, 33);
            this.rbChuyenCan.TabIndex = 0;
            this.rbChuyenCan.TabStop = true;
            this.rbChuyenCan.Text = "Báo cáo chuyên cần";
            this.rbChuyenCan.UseVisualStyleBackColor = true;
            // 
            // rbBangDiem
            // 
            this.rbBangDiem.Location = new System.Drawing.Point(164, 8);
            this.rbBangDiem.Name = "rbBangDiem";
            this.rbBangDiem.Size = new System.Drawing.Size(150, 33);
            this.rbBangDiem.TabIndex = 1;
            this.rbBangDiem.TabStop = true;
            this.rbBangDiem.Text = "Bảng điểm học kỳ";
            this.rbBangDiem.UseVisualStyleBackColor = true;
            // 
            // rbHoSo
            // 
            this.rbHoSo.Location = new System.Drawing.Point(320, 8);
            this.rbHoSo.Name = "rbHoSo";
            this.rbHoSo.Size = new System.Drawing.Size(150, 33);
            this.rbHoSo.TabIndex = 2;
            this.rbHoSo.TabStop = true;
            this.rbHoSo.Text = "Hồ sơ học sinh";
            this.rbHoSo.UseVisualStyleBackColor = true;
            // 
            // rbThongKeKhoi
            // 
            this.rbThongKeKhoi.Location = new System.Drawing.Point(476, 8);
            this.rbThongKeKhoi.Name = "rbThongKeKhoi";
            this.rbThongKeKhoi.Size = new System.Drawing.Size(169, 33);
            this.rbThongKeKhoi.TabIndex = 3;
            this.rbThongKeKhoi.TabStop = true;
            this.rbThongKeKhoi.Text = "Thống kê tổng hợp khối";
            this.rbThongKeKhoi.UseVisualStyleBackColor = true;
            // 
            // rbBaoCaoThang
            // 
            this.rbBaoCaoThang.Location = new System.Drawing.Point(651, 8); // <-- CẬP NHẬT VỊ TRÍ
            this.rbBaoCaoThang.Name = "rbBaoCaoThang";
            this.rbBaoCaoThang.Size = new System.Drawing.Size(160, 33);
            this.rbBaoCaoThang.TabIndex = 4; // <-- THÊM MỚI
            this.rbBaoCaoThang.TabStop = true;
            this.rbBaoCaoThang.Text = "Báo cáo tháng";
            this.rbBaoCaoThang.UseVisualStyleBackColor = true;
            // 
            // pnlKhoi
            // 
            this.pnlKhoi.AutoSize = true;
            this.pnlKhoi.Controls.Add(this.lblKhoi);
            this.pnlKhoi.Controls.Add(this.cboKhoi);
            this.pnlKhoi.Location = new System.Drawing.Point(0, 67);
            this.pnlKhoi.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.pnlKhoi.Name = "pnlKhoi";
            this.pnlKhoi.Size = new System.Drawing.Size(223, 40);
            this.pnlKhoi.TabIndex = 2;
            // 
            // lblKhoi
            // 
            this.lblKhoi.AutoSize = true;
            this.lblKhoi.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.lblKhoi.Location = new System.Drawing.Point(5, 8);
            this.lblKhoi.Margin = new System.Windows.Forms.Padding(5, 8, 0, 5);
            this.lblKhoi.Name = "lblKhoi";
            this.lblKhoi.Size = new System.Drawing.Size(47, 21);
            this.lblKhoi.TabIndex = 0;
            this.lblKhoi.Text = "Khối:";
            // 
            // cboKhoi
            // 
            this.cboKhoi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKhoi.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboKhoi.FormattingEnabled = true;
            this.cboKhoi.Location = new System.Drawing.Point(57, 5);
            this.cboKhoi.Margin = new System.Windows.Forms.Padding(5);
            this.cboKhoi.Name = "cboKhoi";
            this.cboKhoi.Size = new System.Drawing.Size(161, 29);
            this.cboKhoi.TabIndex = 1;
            // 
            // pnlLop
            // 
            this.pnlLop.AutoSize = true;
            this.pnlLop.Controls.Add(this.lblLop);
            this.pnlLop.Controls.Add(this.cboLop);
            this.pnlLop.Location = new System.Drawing.Point(223, 67);
            this.pnlLop.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.pnlLop.Name = "pnlLop";
            this.pnlLop.Size = new System.Drawing.Size(207, 39);
            this.pnlLop.TabIndex = 3;
            // 
            // lblLop
            // 
            this.lblLop.AutoSize = true;
            this.lblLop.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.lblLop.Location = new System.Drawing.Point(5, 8);
            this.lblLop.Margin = new System.Windows.Forms.Padding(5, 8, 0, 5);
            this.lblLop.Name = "lblLop";
            this.lblLop.Size = new System.Drawing.Size(42, 21);
            this.lblLop.TabIndex = 0;
            this.lblLop.Text = "Lớp:";
            // 
            // cboLop
            // 
            this.cboLop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLop.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboLop.FormattingEnabled = true;
            this.cboLop.Location = new System.Drawing.Point(52, 5);
            this.cboLop.Margin = new System.Windows.Forms.Padding(5);
            this.cboLop.Name = "cboLop";
            this.cboLop.Size = new System.Drawing.Size(150, 29);
            this.cboLop.TabIndex = 1;
            // 
            // pnlHocKy
            // 
            this.pnlHocKy.AutoSize = true;
            this.pnlHocKy.Controls.Add(this.lblHocKy);
            this.pnlHocKy.Controls.Add(this.cboHocKy);
            this.pnlHocKy.Location = new System.Drawing.Point(430, 67);
            this.pnlHocKy.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.pnlHocKy.Name = "pnlHocKy";
            this.pnlHocKy.Size = new System.Drawing.Size(193, 39);
            this.pnlHocKy.TabIndex = 4;
            // 
            // lblHocKy
            // 
            this.lblHocKy.AutoSize = true;
            this.lblHocKy.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.lblHocKy.Location = new System.Drawing.Point(5, 8);
            this.lblHocKy.Margin = new System.Windows.Forms.Padding(5, 8, 0, 5);
            this.lblHocKy.Name = "lblHocKy";
            this.lblHocKy.Size = new System.Drawing.Size(64, 21);
            this.lblHocKy.TabIndex = 0;
            this.lblHocKy.Text = "Học kỳ:";
            // 
            // cboHocKy
            // 
            this.cboHocKy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHocKy.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboHocKy.FormattingEnabled = true;
            this.cboHocKy.Location = new System.Drawing.Point(74, 5);
            this.cboHocKy.Margin = new System.Windows.Forms.Padding(5);
            this.cboHocKy.Name = "cboHocKy";
            this.cboHocKy.Size = new System.Drawing.Size(114, 29);
            this.cboHocKy.TabIndex = 1;
            // 
            // pnlMon
            // 
            this.pnlMon.AutoSize = true;
            this.pnlMon.Controls.Add(this.lblMonDay);
            this.pnlMon.Controls.Add(this.cboMonDay);
            this.pnlMon.Location = new System.Drawing.Point(623, 67);
            this.pnlMon.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.pnlMon.Name = "pnlMon";
            this.pnlMon.Size = new System.Drawing.Size(262, 39);
            this.pnlMon.TabIndex = 5;
            // 
            // lblMonDay
            // 
            this.lblMonDay.AutoSize = true;
            this.lblMonDay.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.lblMonDay.Location = new System.Drawing.Point(5, 8);
            this.lblMonDay.Margin = new System.Windows.Forms.Padding(5, 8, 0, 5);
            this.lblMonDay.Name = "lblMonDay";
            this.lblMonDay.Size = new System.Drawing.Size(48, 21);
            this.lblMonDay.TabIndex = 0;
            this.lblMonDay.Text = "Môn:";
            // 
            // cboMonDay
            // 
            this.cboMonDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMonDay.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboMonDay.FormattingEnabled = true;
            this.cboMonDay.Location = new System.Drawing.Point(58, 5);
            this.cboMonDay.Margin = new System.Windows.Forms.Padding(5);
            this.cboMonDay.Name = "cboMonDay";
            this.cboMonDay.Size = new System.Drawing.Size(199, 29);
            this.cboMonDay.TabIndex = 1;
            // 
            // pnlThang
            // 
            this.pnlThang.AutoSize = true;
            this.pnlThang.Controls.Add(this.lblThang);
            this.pnlThang.Controls.Add(this.cboThang);
            this.pnlThang.Location = new System.Drawing.Point(885, 67); // <-- CẬP NHẬT VỊ TRÍ (sau pnlMon)
            this.pnlThang.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.pnlThang.Name = "pnlThang";
            this.pnlThang.Size = new System.Drawing.Size(225, 39);
            this.pnlThang.TabIndex = 6; // <-- THÊM MỚI
            // 
            // lblThang
            // 
            this.lblThang.AutoSize = true;
            this.lblThang.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.lblThang.Location = new System.Drawing.Point(5, 8);
            this.lblThang.Margin = new System.Windows.Forms.Padding(5, 8, 0, 5);
            this.lblThang.Name = "lblThang";
            this.lblThang.Size = new System.Drawing.Size(59, 21);
            this.lblThang.TabIndex = 0;
            this.lblThang.Text = "Tháng:";
            // 
            // cboThang
            // 
            this.cboThang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboThang.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboThang.FormattingEnabled = true;
            this.cboThang.Location = new System.Drawing.Point(69, 5);
            this.cboThang.Margin = new System.Windows.Forms.Padding(5);
            this.cboThang.Name = "cboThang";
            this.cboThang.Size = new System.Drawing.Size(151, 29);
            this.cboThang.TabIndex = 1;
            // 
            // btnXuatPDF
            // 
            this.btnXuatPDF.Location = new System.Drawing.Point(3, 111); // <-- CẬP NHẬT VỊ TRÍ
            this.btnXuatPDF.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.btnXuatPDF.Name = "btnXuatPDF";
            this.btnXuatPDF.Size = new System.Drawing.Size(130, 38);
            this.btnXuatPDF.TabIndex = 8;
            this.btnXuatPDF.Text = "Xuất PDF";
            this.btnXuatPDF.UseVisualStyleBackColor = false;
            // 
            // btnXuatExcel
            // 
            this.btnXuatExcel.Location = new System.Drawing.Point(146, 111); // <-- CẬP NHẬT VỊ TRÍ
            this.btnXuatExcel.Name = "btnXuatExcel";
            this.btnXuatExcel.Size = new System.Drawing.Size(130, 38);
            this.btnXuatExcel.TabIndex = 7;
            this.btnXuatExcel.Text = "Xuất Excel";
            this.btnXuatExcel.UseVisualStyleBackColor = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(10, 140);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvDuLieu);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.flpCharts);
            this.splitContainer1.Size = new System.Drawing.Size(980, 450);
            this.splitContainer1.SplitterDistance = 230;
            this.splitContainer1.TabIndex = 1;
            // 
            // dgvDuLieu
            // 
            this.dgvDuLieu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDuLieu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDuLieu.Location = new System.Drawing.Point(0, 0);
            this.dgvDuLieu.Name = "dgvDuLieu";
            this.dgvDuLieu.RowHeadersWidth = 51;
            this.dgvDuLieu.Size = new System.Drawing.Size(978, 228);
            this.dgvDuLieu.TabIndex = 0;
            // 
            // flpCharts
            // 
            this.flpCharts.AutoScroll = true;
            this.flpCharts.BackColor = System.Drawing.Color.White;
            this.flpCharts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpCharts.Location = new System.Drawing.Point(0, 0);
            this.flpCharts.Name = "flpCharts";
            this.flpCharts.Padding = new System.Windows.Forms.Padding(10);
            this.flpCharts.Size = new System.Drawing.Size(978, 214);
            this.flpCharts.TabIndex = 0;
            // 
            // UC_BaoCao_Admin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pnlFilters);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.Name = "UC_BaoCao_Admin";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(1000, 600);
            this.pnlFilters.ResumeLayout(false);
            this.flpFilters.ResumeLayout(false);
            this.flpFilters.PerformLayout();
            this.pnlReportTypeSelector.ResumeLayout(false);
            this.pnlKhoi.ResumeLayout(false);
            this.pnlKhoi.PerformLayout();
            this.pnlLop.ResumeLayout(false);
            this.pnlLop.PerformLayout();
            this.pnlHocKy.ResumeLayout(false);
            this.pnlHocKy.PerformLayout();
            this.pnlMon.ResumeLayout(false);
            this.pnlMon.PerformLayout();
            this.pnlThang.ResumeLayout(false); // <-- THÊM MỚI
            this.pnlThang.PerformLayout(); // <-- THÊM MỚI
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuLieu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.FlowLayoutPanel flpFilters;
        private System.Windows.Forms.ComboBox cboLop;
        private System.Windows.Forms.ComboBox cboKhoi;
        private System.Windows.Forms.ComboBox cboHocKy;
        private System.Windows.Forms.Label lblMonDay;
        private System.Windows.Forms.ComboBox cboMonDay;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvDuLieu;
        private System.Windows.Forms.FlowLayoutPanel flpCharts;
        private System.Windows.Forms.FlowLayoutPanel pnlKhoi;
        private System.Windows.Forms.Label lblKhoi;
        private System.Windows.Forms.FlowLayoutPanel pnlLop;
        private System.Windows.Forms.Label lblLop;
        private System.Windows.Forms.FlowLayoutPanel pnlHocKy;
        private System.Windows.Forms.Label lblHocKy;
        private System.Windows.Forms.FlowLayoutPanel pnlMon;
        private System.Windows.Forms.FlowLayoutPanel pnlReportTypeSelector;
        private System.Windows.Forms.RadioButton rbChuyenCan;
        private System.Windows.Forms.RadioButton rbBangDiem;
        private System.Windows.Forms.RadioButton rbHoSo;
        private System.Windows.Forms.RadioButton rbThongKeKhoi;
        private System.Windows.Forms.Button btnXuatPDF;
        private System.Windows.Forms.Button btnXuatExcel;
        private System.Windows.Forms.RadioButton rbBaoCaoThang; // <-- THÊM MỚI
        private System.Windows.Forms.FlowLayoutPanel pnlThang; // <-- THÊM MỚI
        private System.Windows.Forms.Label lblThang; // <-- THÊM MỚI
        private System.Windows.Forms.ComboBox cboThang; // <-- THÊM MỚI
    }
}