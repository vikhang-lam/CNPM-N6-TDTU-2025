namespace N6
{
    partial class UC_QuanLyLopHocSinh
    {
        private System.ComponentModel.IContainer components = null;
        
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
            this.layoutRoot = new System.Windows.Forms.TableLayoutPanel();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.dgvLopHoc = new System.Windows.Forms.DataGridView();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.cboKhoi = new System.Windows.Forms.ComboBox();
            this.lblChonKhoi = new System.Windows.Forms.Label();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.tabControlDetails = new System.Windows.Forms.TabControl();
            this.tabHocSinh = new System.Windows.Forms.TabPage();
            this.dgvHocSinh = new System.Windows.Forms.DataGridView();
            this.pnlStudentActions = new System.Windows.Forms.FlowLayoutPanel();
            this.btnThemHS = new System.Windows.Forms.Button();
            this.btnSuaHS = new System.Windows.Forms.Button();
            this.btnXoaHS = new System.Windows.Forms.Button();
            this.tabPhanCong = new System.Windows.Forms.TabPage();
            this.dgvPhanCong = new System.Windows.Forms.DataGridView();
            this.pnlClassInfoCard = new System.Windows.Forms.Panel();
            this.mainInfoFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTenLop = new System.Windows.Forms.Label();
            this.infoLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblNamHoc = new System.Windows.Forms.Label();
            this.lblGVCN = new System.Windows.Forms.Label();
            this.lblSiSo = new System.Windows.Forms.Label();
            this.pnlAssignGvcn = new System.Windows.Forms.Panel();
            this.btnAssignGvcn = new System.Windows.Forms.Button();
            this.cboGvcn = new System.Windows.Forms.ComboBox();
            this.lblAssignGvcn = new System.Windows.Forms.Label();
            this.layoutRoot.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLopHoc)).BeginInit();
            this.pnlFilter.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.tabControlDetails.SuspendLayout();
            this.tabHocSinh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHocSinh)).BeginInit();
            this.pnlStudentActions.SuspendLayout();
            this.tabPhanCong.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhanCong)).BeginInit();
            this.pnlClassInfoCard.SuspendLayout();
            this.mainInfoFlowPanel.SuspendLayout();
            this.infoLayout.SuspendLayout();
            this.pnlAssignGvcn.SuspendLayout();
            this.SuspendLayout();
            // 
            // layoutRoot
            // 
            this.layoutRoot.ColumnCount = 2;
            this.layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 280F));
            this.layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutRoot.Controls.Add(this.pnlLeft, 0, 0);
            this.layoutRoot.Controls.Add(this.pnlRight, 1, 0);
            this.layoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutRoot.Name = "layoutRoot";
            this.layoutRoot.RowCount = 1;
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutRoot.Size = new System.Drawing.Size(940, 620);
            this.layoutRoot.TabIndex = 0;
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.dgvLopHoc);
            this.pnlLeft.Controls.Add(this.pnlFilter);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeft.Location = new System.Drawing.Point(10, 10);
            this.pnlLeft.Margin = new System.Windows.Forms.Padding(10);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(260, 600);
            this.pnlLeft.TabIndex = 0;
            // 
            // dgvLopHoc
            // 
            this.dgvLopHoc.AllowUserToAddRows = false;
            this.dgvLopHoc.AllowUserToDeleteRows = false;
            this.dgvLopHoc.AllowUserToResizeColumns = false;
            this.dgvLopHoc.AllowUserToResizeRows = false;
            this.dgvLopHoc.ColumnHeadersHeight = 29;
            this.dgvLopHoc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLopHoc.Location = new System.Drawing.Point(0, 80);
            this.dgvLopHoc.MultiSelect = false;
            this.dgvLopHoc.Name = "dgvLopHoc";
            this.dgvLopHoc.ReadOnly = true;
            this.dgvLopHoc.RowHeadersVisible = false;
            this.dgvLopHoc.RowHeadersWidth = 51;
            this.dgvLopHoc.Size = new System.Drawing.Size(260, 520);
            this.dgvLopHoc.TabIndex = 1;
            // 
            // pnlFilter
            // 
            this.pnlFilter.BackColor = System.Drawing.Color.White;
            this.pnlFilter.Controls.Add(this.cboKhoi);
            this.pnlFilter.Controls.Add(this.lblChonKhoi);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(0, 0);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(260, 80);
            this.pnlFilter.TabIndex = 0;
            // 
            // cboKhoi
            // 
            this.cboKhoi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboKhoi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKhoi.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboKhoi.FormattingEnabled = true;
            this.cboKhoi.Location = new System.Drawing.Point(15, 35);
            this.cboKhoi.Name = "cboKhoi";
            this.cboKhoi.Size = new System.Drawing.Size(230, 31);
            this.cboKhoi.TabIndex = 1;
            this.cboKhoi.SelectedIndexChanged += new System.EventHandler(this.cboKhoi_SelectedIndexChanged);
            // 
            // lblChonKhoi
            // 
            this.lblChonKhoi.AutoSize = true;
            this.lblChonKhoi.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblChonKhoi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblChonKhoi.Location = new System.Drawing.Point(12, 9);
            this.lblChonKhoi.Name = "lblChonKhoi";
            this.lblChonKhoi.Size = new System.Drawing.Size(123, 23);
            this.lblChonKhoi.TabIndex = 0;
            this.lblChonKhoi.Text = "Lọc theo khối:";
            // 
            // pnlRight
            // 
            this.pnlRight.Controls.Add(this.tabControlDetails);
            this.pnlRight.Controls.Add(this.pnlClassInfoCard);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(283, 3);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(654, 614);
            this.pnlRight.TabIndex = 1;
            // 
            // tabControlDetails
            // 
            this.tabControlDetails.Controls.Add(this.tabHocSinh);
            this.tabControlDetails.Controls.Add(this.tabPhanCong);
            this.tabControlDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlDetails.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tabControlDetails.Location = new System.Drawing.Point(0, 195);
            this.tabControlDetails.Name = "tabControlDetails";
            this.tabControlDetails.SelectedIndex = 0;
            this.tabControlDetails.Size = new System.Drawing.Size(654, 419);
            this.tabControlDetails.TabIndex = 1;
            // 
            // tabHocSinh
            // 
            this.tabHocSinh.Controls.Add(this.dgvHocSinh);
            this.tabHocSinh.Controls.Add(this.pnlStudentActions);
            this.tabHocSinh.Location = new System.Drawing.Point(4, 32);
            this.tabHocSinh.Name = "tabHocSinh";
            this.tabHocSinh.Size = new System.Drawing.Size(646, 383);
            this.tabHocSinh.TabIndex = 0;
            this.tabHocSinh.Text = "Danh sách Học sinh";
            this.tabHocSinh.UseVisualStyleBackColor = true;
            // 
            // dgvHocSinh
            // 
            this.dgvHocSinh.ColumnHeadersHeight = 29;
            this.dgvHocSinh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHocSinh.Location = new System.Drawing.Point(0, 0);
            this.dgvHocSinh.Name = "dgvHocSinh";
            this.dgvHocSinh.RowHeadersWidth = 51;
            this.dgvHocSinh.Size = new System.Drawing.Size(646, 329);
            this.dgvHocSinh.TabIndex = 1;
            // 
            // pnlStudentActions
            // 
            this.pnlStudentActions.Controls.Add(this.btnThemHS);
            this.pnlStudentActions.Controls.Add(this.btnSuaHS);
            this.pnlStudentActions.Controls.Add(this.btnXoaHS);
            this.pnlStudentActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStudentActions.Location = new System.Drawing.Point(0, 329);
            this.pnlStudentActions.Name = "pnlStudentActions";
            this.pnlStudentActions.Size = new System.Drawing.Size(646, 54);
            this.pnlStudentActions.TabIndex = 0;
            // 
            // btnThemHS
            // 
            this.btnThemHS.Location = new System.Drawing.Point(3, 3);
            this.btnThemHS.Name = "btnThemHS";
            this.btnThemHS.Size = new System.Drawing.Size(75, 33);
            this.btnThemHS.TabIndex = 0;
            this.btnThemHS.Text = "➕ Thêm HS";
            // 
            // btnSuaHS
            // 
            this.btnSuaHS.Location = new System.Drawing.Point(84, 3);
            this.btnSuaHS.Name = "btnSuaHS";
            this.btnSuaHS.Size = new System.Drawing.Size(75, 33);
            this.btnSuaHS.TabIndex = 1;
            this.btnSuaHS.Text = "✏️ Sửa HS";
            // 
            // btnXoaHS
            // 
            this.btnXoaHS.Location = new System.Drawing.Point(165, 3);
            this.btnXoaHS.Name = "btnXoaHS";
            this.btnXoaHS.Size = new System.Drawing.Size(75, 33);
            this.btnXoaHS.TabIndex = 2;
            this.btnXoaHS.Text = "🗑️ Xóa HS";
            // 
            // tabPhanCong
            // 
            this.tabPhanCong.Controls.Add(this.dgvPhanCong);
            this.tabPhanCong.Location = new System.Drawing.Point(4, 32);
            this.tabPhanCong.Name = "tabPhanCong";
            this.tabPhanCong.Size = new System.Drawing.Size(646, 373);
            this.tabPhanCong.TabIndex = 1;
            this.tabPhanCong.Text = "Phân công Giảng dạy";
            this.tabPhanCong.UseVisualStyleBackColor = true;
            // 
            // dgvPhanCong
            // 
            this.dgvPhanCong.ColumnHeadersHeight = 29;
            this.dgvPhanCong.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPhanCong.Location = new System.Drawing.Point(0, 0);
            this.dgvPhanCong.Name = "dgvPhanCong";
            this.dgvPhanCong.RowHeadersWidth = 51;
            this.dgvPhanCong.Size = new System.Drawing.Size(646, 373);
            this.dgvPhanCong.TabIndex = 0;
            // 
            // pnlClassInfoCard
            // 
            this.pnlClassInfoCard.BackColor = System.Drawing.Color.White;
            this.pnlClassInfoCard.Controls.Add(this.mainInfoFlowPanel);
            this.pnlClassInfoCard.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlClassInfoCard.Location = new System.Drawing.Point(0, 0);
            this.pnlClassInfoCard.Margin = new System.Windows.Forms.Padding(10);
            this.pnlClassInfoCard.Name = "pnlClassInfoCard";
            this.pnlClassInfoCard.Padding = new System.Windows.Forms.Padding(10);
            this.pnlClassInfoCard.Size = new System.Drawing.Size(654, 195);
            this.pnlClassInfoCard.TabIndex = 0;
            // 
            // mainInfoFlowPanel
            // 
            this.mainInfoFlowPanel.Controls.Add(this.lblTenLop);
            this.mainInfoFlowPanel.Controls.Add(this.infoLayout);
            this.mainInfoFlowPanel.Controls.Add(this.pnlAssignGvcn);
            this.mainInfoFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainInfoFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.mainInfoFlowPanel.Location = new System.Drawing.Point(10, 10);
            this.mainInfoFlowPanel.Name = "mainInfoFlowPanel";
            this.mainInfoFlowPanel.Size = new System.Drawing.Size(634, 175);
            this.mainInfoFlowPanel.TabIndex = 6;
            this.mainInfoFlowPanel.WrapContents = false;
            // 
            // lblTenLop
            // 
            this.lblTenLop.AutoSize = true;
            this.lblTenLop.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTenLop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.lblTenLop.Location = new System.Drawing.Point(3, 0);
            this.lblTenLop.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.lblTenLop.Name = "lblTenLop";
            this.lblTenLop.Size = new System.Drawing.Size(359, 37);
            this.lblTenLop.TabIndex = 0;
            this.lblTenLop.Text = "Chọn lớp để xem thông tin";
            // 
            // infoLayout
            // 
            this.infoLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLayout.AutoSize = true;
            this.infoLayout.ColumnCount = 3;
            this.infoLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.75079F));
            this.infoLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.75079F));
            this.infoLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.65615F));
            this.infoLayout.Controls.Add(this.lblNamHoc, 0, 0);
            this.infoLayout.Controls.Add(this.lblGVCN, 1, 0);
            this.infoLayout.Controls.Add(this.lblSiSo, 2, 0);
            this.infoLayout.Location = new System.Drawing.Point(0, 52);
            this.infoLayout.Margin = new System.Windows.Forms.Padding(0, 5, 0, 10);
            this.infoLayout.Name = "infoLayout";
            this.infoLayout.RowCount = 1;
            this.infoLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.infoLayout.Size = new System.Drawing.Size(634, 30);
            this.infoLayout.TabIndex = 5;
            // 
            // lblNamHoc
            // 
            this.lblNamHoc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNamHoc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblNamHoc.Location = new System.Drawing.Point(3, 0);
            this.lblNamHoc.Name = "lblNamHoc";
            this.lblNamHoc.Size = new System.Drawing.Size(226, 30);
            this.lblNamHoc.TabIndex = 0;
            this.lblNamHoc.Text = "🗓️ Năm học: -";
            this.lblNamHoc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblGVCN
            // 
            this.lblGVCN.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblGVCN.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblGVCN.Location = new System.Drawing.Point(235, 0);
            this.lblGVCN.Name = "lblGVCN";
            this.lblGVCN.Size = new System.Drawing.Size(226, 30);
            this.lblGVCN.TabIndex = 1;
            this.lblGVCN.Text = "👤 GVCN: -";
            this.lblGVCN.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSiSo
            // 
            this.lblSiSo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSiSo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblSiSo.Location = new System.Drawing.Point(467, 0);
            this.lblSiSo.Name = "lblSiSo";
            this.lblSiSo.Size = new System.Drawing.Size(137, 30);
            this.lblSiSo.TabIndex = 2;
            this.lblSiSo.Text = "👥 Sĩ số: -";
            this.lblSiSo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlAssignGvcn
            // 
            this.pnlAssignGvcn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlAssignGvcn.Controls.Add(this.btnAssignGvcn);
            this.pnlAssignGvcn.Controls.Add(this.cboGvcn);
            this.pnlAssignGvcn.Controls.Add(this.lblAssignGvcn);
            this.pnlAssignGvcn.Location = new System.Drawing.Point(3, 95);
            this.pnlAssignGvcn.Name = "pnlAssignGvcn";
            this.pnlAssignGvcn.Size = new System.Drawing.Size(628, 50);
            this.pnlAssignGvcn.TabIndex = 4;
            // 
            // btnAssignGvcn
            // 
            this.btnAssignGvcn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAssignGvcn.Location = new System.Drawing.Point(400, 8);
            this.btnAssignGvcn.Name = "btnAssignGvcn";
            this.btnAssignGvcn.Size = new System.Drawing.Size(120, 31);
            this.btnAssignGvcn.TabIndex = 2;
            this.btnAssignGvcn.Text = "Phân công";
            this.btnAssignGvcn.Click += new System.EventHandler(this.btnAssignGvcn_Click);
            // 
            // cboGvcn
            // 
            this.cboGvcn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGvcn.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboGvcn.Location = new System.Drawing.Point(140, 8);
            this.cboGvcn.Name = "cboGvcn";
            this.cboGvcn.Size = new System.Drawing.Size(250, 31);
            this.cboGvcn.TabIndex = 1;
            // 
            // lblAssignGvcn
            // 
            this.lblAssignGvcn.AutoSize = true;
            this.lblAssignGvcn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAssignGvcn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblAssignGvcn.Location = new System.Drawing.Point(3, 12);
            this.lblAssignGvcn.Name = "lblAssignGvcn";
            this.lblAssignGvcn.Size = new System.Drawing.Size(131, 23);
            this.lblAssignGvcn.TabIndex = 0;
            this.lblAssignGvcn.Text = "Chưa có GVCN:";
            // 
            // UC_QuanLyLopHocSinh
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.Controls.Add(this.layoutRoot);
            this.Name = "UC_QuanLyLopHocSinh";
            this.Size = new System.Drawing.Size(940, 620);
            this.layoutRoot.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLopHoc)).EndInit();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.tabControlDetails.ResumeLayout(false);
            this.tabHocSinh.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHocSinh)).EndInit();
            this.pnlStudentActions.ResumeLayout(false);
            this.tabPhanCong.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhanCong)).EndInit();
            this.pnlClassInfoCard.ResumeLayout(false);
            this.mainInfoFlowPanel.ResumeLayout(false);
            this.mainInfoFlowPanel.PerformLayout();
            this.infoLayout.ResumeLayout(false);
            this.pnlAssignGvcn.ResumeLayout(false);
            this.pnlAssignGvcn.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TableLayoutPanel layoutRoot;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.ComboBox cboKhoi;
        private System.Windows.Forms.Label lblChonKhoi;
        private System.Windows.Forms.DataGridView dgvLopHoc;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Panel pnlClassInfoCard;
        private System.Windows.Forms.Label lblTenLop;
        private System.Windows.Forms.Label lblNamHoc;
        private System.Windows.Forms.Label lblGVCN;
        private System.Windows.Forms.Label lblSiSo;
        private System.Windows.Forms.TabControl tabControlDetails;
        private System.Windows.Forms.TabPage tabHocSinh;
        private System.Windows.Forms.TabPage tabPhanCong;
        private System.Windows.Forms.FlowLayoutPanel pnlStudentActions;
        private System.Windows.Forms.Button btnThemHS;
        private System.Windows.Forms.Button btnSuaHS;
        private System.Windows.Forms.Button btnXoaHS;
        private System.Windows.Forms.DataGridView dgvHocSinh;
        private System.Windows.Forms.DataGridView dgvPhanCong;
        private System.Windows.Forms.Panel pnlAssignGvcn;
        private System.Windows.Forms.Button btnAssignGvcn;
        private System.Windows.Forms.ComboBox cboGvcn;
        private System.Windows.Forms.Label lblAssignGvcn;
        private System.Windows.Forms.TableLayoutPanel infoLayout;
        private System.Windows.Forms.FlowLayoutPanel mainInfoFlowPanel;
    }
}