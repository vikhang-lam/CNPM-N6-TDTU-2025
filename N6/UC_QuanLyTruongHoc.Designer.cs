namespace N6
{
    partial class UC_QuanLyTruongHoc
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

        #region Component Designer generated code
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabMonHoc = new System.Windows.Forms.TabPage();
            this.pnlMonHoc = new System.Windows.Forms.Panel();
            this.btnMoiMon = new System.Windows.Forms.Button();
            this.btnXoaMon = new System.Windows.Forms.Button();
            this.btnSuaMon = new System.Windows.Forms.Button();
            this.btnThemMon = new System.Windows.Forms.Button();
            this.txtTenMon = new System.Windows.Forms.TextBox();
            this.lblTenMon = new System.Windows.Forms.Label();
            this.txtMaMon = new System.Windows.Forms.TextBox();
            this.lblMaMon = new System.Windows.Forms.Label();
            this.lblTitleMonHoc = new System.Windows.Forms.Label();
            this.dgvMonHoc = new System.Windows.Forms.DataGridView();
            this.tabThoiHanDiem = new System.Windows.Forms.TabPage();
            this.cboKhoiFilter = new System.Windows.Forms.ComboBox();
            this.lblKhoiFilter = new System.Windows.Forms.Label();
            this.btnLuuThoiHan = new System.Windows.Forms.Button();
            this.lblThoiHanDesc = new System.Windows.Forms.Label();
            this.lblThoiHanTitle = new System.Windows.Forms.Label();
            this.dgvThoiHanDiem = new System.Windows.Forms.DataGridView();
            this.tabLenLop = new System.Windows.Forms.TabPage();
            this.cboKhoiFilter_LenLop = new System.Windows.Forms.ComboBox();
            this.lblKhoiFilter_LenLop = new System.Windows.Forms.Label();
            this.btnThucHienLenLop = new System.Windows.Forms.Button();
            this.lblLenLopDesc = new System.Windows.Forms.Label();
            this.lblLenLopTitle = new System.Windows.Forms.Label();
            this.dgvLenLop = new System.Windows.Forms.DataGridView();
            this.tabControl1.SuspendLayout();
            this.tabMonHoc.SuspendLayout();
            this.pnlMonHoc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonHoc)).BeginInit();
            this.tabThoiHanDiem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThoiHanDiem)).BeginInit();
            this.tabLenLop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLenLop)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabMonHoc);
            this.tabControl1.Controls.Add(this.tabThoiHanDiem);
            this.tabControl1.Controls.Add(this.tabLenLop);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(10, 5);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(950, 650);
            this.tabControl1.TabIndex = 0;
            // 
            // tabMonHoc
            // 
            this.tabMonHoc.BackColor = System.Drawing.Color.White;
            this.tabMonHoc.Controls.Add(this.dgvMonHoc);
            this.tabMonHoc.Controls.Add(this.pnlMonHoc);
            this.tabMonHoc.Location = new System.Drawing.Point(4, 38);
            this.tabMonHoc.Name = "tabMonHoc";
            this.tabMonHoc.Padding = new System.Windows.Forms.Padding(10);
            this.tabMonHoc.Size = new System.Drawing.Size(942, 608);
            this.tabMonHoc.TabIndex = 0;
            this.tabMonHoc.Text = "Quản lý Môn học";
            // 
            // pnlMonHoc
            // 
            this.pnlMonHoc.Controls.Add(this.btnMoiMon);
            this.pnlMonHoc.Controls.Add(this.btnXoaMon);
            this.pnlMonHoc.Controls.Add(this.btnSuaMon);
            this.pnlMonHoc.Controls.Add(this.btnThemMon);
            this.pnlMonHoc.Controls.Add(this.txtTenMon);
            this.pnlMonHoc.Controls.Add(this.lblTenMon);
            this.pnlMonHoc.Controls.Add(this.txtMaMon);
            this.pnlMonHoc.Controls.Add(this.lblMaMon);
            this.pnlMonHoc.Controls.Add(this.lblTitleMonHoc);
            this.pnlMonHoc.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlMonHoc.Location = new System.Drawing.Point(590, 10);
            this.pnlMonHoc.Name = "pnlMonHoc";
            this.pnlMonHoc.Padding = new System.Windows.Forms.Padding(10);
            this.pnlMonHoc.Size = new System.Drawing.Size(342, 588);
            this.pnlMonHoc.TabIndex = 1;
            // 
            // btnMoiMon
            // 
            this.btnMoiMon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnMoiMon.FlatAppearance.BorderSize = 0;
            this.btnMoiMon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoiMon.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnMoiMon.ForeColor = System.Drawing.Color.White;
            this.btnMoiMon.Location = new System.Drawing.Point(16, 203);
            this.btnMoiMon.Name = "btnMoiMon";
            this.btnMoiMon.Size = new System.Drawing.Size(80, 40);
            this.btnMoiMon.TabIndex = 8;
            this.btnMoiMon.Text = "Mới";
            this.btnMoiMon.UseVisualStyleBackColor = false;
            this.btnMoiMon.Click += new System.EventHandler(this.btnMoiMon_Click);
            // 
            // btnXoaMon
            // 
            this.btnXoaMon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnXoaMon.FlatAppearance.BorderSize = 0;
            this.btnXoaMon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXoaMon.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXoaMon.ForeColor = System.Drawing.Color.White;
            this.btnXoaMon.Location = new System.Drawing.Point(223, 260);
            this.btnXoaMon.Name = "btnXoaMon";
            this.btnXoaMon.Size = new System.Drawing.Size(96, 40);
            this.btnXoaMon.TabIndex = 7;
            this.btnXoaMon.Text = "Xóa";
            this.btnXoaMon.UseVisualStyleBackColor = false;
            this.btnXoaMon.Click += new System.EventHandler(this.btnXoaMon_Click);
            // 
            // btnSuaMon
            // 
            this.btnSuaMon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.btnSuaMon.FlatAppearance.BorderSize = 0;
            this.btnSuaMon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSuaMon.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSuaMon.ForeColor = System.Drawing.Color.Black;
            this.btnSuaMon.Location = new System.Drawing.Point(119, 260);
            this.btnSuaMon.Name = "btnSuaMon";
            this.btnSuaMon.Size = new System.Drawing.Size(96, 40);
            this.btnSuaMon.TabIndex = 6;
            this.btnSuaMon.Text = "Sửa";
            this.btnSuaMon.UseVisualStyleBackColor = false;
            this.btnSuaMon.Click += new System.EventHandler(this.btnSuaMon_Click);
            // 
            // btnThemMon
            // 
            this.btnThemMon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnThemMon.FlatAppearance.BorderSize = 0;
            this.btnThemMon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThemMon.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnThemMon.ForeColor = System.Drawing.Color.White;
            this.btnThemMon.Location = new System.Drawing.Point(16, 260);
            this.btnThemMon.Name = "btnThemMon";
            this.btnThemMon.Size = new System.Drawing.Size(96, 40);
            this.btnThemMon.TabIndex = 5;
            this.btnThemMon.Text = "Thêm";
            this.btnThemMon.UseVisualStyleBackColor = false;
            this.btnThemMon.Click += new System.EventHandler(this.btnThemMon_Click);
            // 
            // txtTenMon
            // 
            this.txtTenMon.Location = new System.Drawing.Point(119, 149);
            this.txtTenMon.Name = "txtTenMon";
            this.txtTenMon.Size = new System.Drawing.Size(200, 31);
            this.txtTenMon.TabIndex = 4;
            // 
            // lblTenMon
            // 
            this.lblTenMon.AutoSize = true;
            this.lblTenMon.Location = new System.Drawing.Point(11, 152);
            this.lblTenMon.Name = "lblTenMon";
            this.lblTenMon.Size = new System.Drawing.Size(85, 25);
            this.lblTenMon.TabIndex = 3;
            this.lblTenMon.Text = "Tên môn:";
            // 
            // txtMaMon
            // 
            this.txtMaMon.Location = new System.Drawing.Point(119, 101);
            this.txtMaMon.Name = "txtMaMon";
            this.txtMaMon.Size = new System.Drawing.Size(200, 31);
            this.txtMaMon.TabIndex = 2;
            // 
            // lblMaMon
            // 
            this.lblMaMon.AutoSize = true;
            this.lblMaMon.Location = new System.Drawing.Point(11, 104);
            this.lblMaMon.Name = "lblMaMon";
            this.lblMaMon.Size = new System.Drawing.Size(84, 25);
            this.lblMaMon.TabIndex = 1;
            this.lblMaMon.Text = "Mã môn:";
            // 
            // lblTitleMonHoc
            // 
            this.lblTitleMonHoc.AutoSize = true;
            this.lblTitleMonHoc.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleMonHoc.Location = new System.Drawing.Point(10, 10);
            this.lblTitleMonHoc.Name = "lblTitleMonHoc";
            this.lblTitleMonHoc.Size = new System.Drawing.Size(234, 31);
            this.lblTitleMonHoc.TabIndex = 0;
            this.lblTitleMonHoc.Text = "Chi tiết Môn học";
            // 
            // dgvMonHoc
            // 
            this.dgvMonHoc.AllowUserToAddRows = false;
            this.dgvMonHoc.AllowUserToDeleteRows = false;
            this.dgvMonHoc.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvMonHoc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMonHoc.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvMonHoc.ColumnHeadersHeight = 40;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMonHoc.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvMonHoc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMonHoc.Location = new System.Drawing.Point(10, 10);
            this.dgvMonHoc.MultiSelect = false;
            this.dgvMonHoc.Name = "dgvMonHoc";
            this.dgvMonHoc.ReadOnly = true;
            this.dgvMonHoc.RowHeadersWidth = 51;
            this.dgvMonHoc.RowTemplate.Height = 35;
            this.dgvMonHoc.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMonHoc.Size = new System.Drawing.Size(580, 588);
            this.dgvMonHoc.TabIndex = 0;
            this.dgvMonHoc.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMonHoc_CellClick);
            // 
            // tabThoiHanDiem
            // 
            this.tabThoiHanDiem.BackColor = System.Drawing.Color.White;
            this.tabThoiHanDiem.Controls.Add(this.cboKhoiFilter);
            this.tabThoiHanDiem.Controls.Add(this.lblKhoiFilter);
            this.tabThoiHanDiem.Controls.Add(this.btnLuuThoiHan);
            this.tabThoiHanDiem.Controls.Add(this.lblThoiHanDesc);
            this.tabThoiHanDiem.Controls.Add(this.lblThoiHanTitle);
            this.tabThoiHanDiem.Controls.Add(this.dgvThoiHanDiem);
            this.tabThoiHanDiem.Location = new System.Drawing.Point(4, 38);
            this.tabThoiHanDiem.Name = "tabThoiHanDiem";
            this.tabThoiHanDiem.Padding = new System.Windows.Forms.Padding(10);
            this.tabThoiHanDiem.Size = new System.Drawing.Size(942, 608);
            this.tabThoiHanDiem.TabIndex = 1;
            this.tabThoiHanDiem.Text = "Khóa/Mở Nhập điểm";
            // 
            // cboKhoiFilter
            // 
            this.cboKhoiFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboKhoiFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKhoiFilter.FormattingEnabled = true;
            this.cboKhoiFilter.Location = new System.Drawing.Point(753, 90);
            this.cboKhoiFilter.Name = "cboKhoiFilter";
            this.cboKhoiFilter.Size = new System.Drawing.Size(176, 33);
            this.cboKhoiFilter.TabIndex = 5;
            this.cboKhoiFilter.SelectedIndexChanged += new System.EventHandler(this.cboKhoiFilter_SelectedIndexChanged);
            // 
            // lblKhoiFilter
            // 
            this.lblKhoiFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblKhoiFilter.AutoSize = true;
            this.lblKhoiFilter.Location = new System.Drawing.Point(663, 93);
            this.lblKhoiFilter.Name = "lblKhoiFilter";
            this.lblKhoiFilter.Size = new System.Drawing.Size(84, 25);
            this.lblKhoiFilter.TabIndex = 4;
            this.lblKhoiFilter.Text = "Lọc khối:";
            // 
            // btnLuuThoiHan
            // 
            this.btnLuuThoiHan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLuuThoiHan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnLuuThoiHan.FlatAppearance.BorderSize = 0;
            this.btnLuuThoiHan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLuuThoiHan.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLuuThoiHan.ForeColor = System.Drawing.Color.White;
            this.btnLuuThoiHan.Location = new System.Drawing.Point(707, 545);
            this.btnLuuThoiHan.Name = "btnLuuThoiHan";
            this.btnLuuThoiHan.Size = new System.Drawing.Size(222, 50);
            this.btnLuuThoiHan.TabIndex = 3;
            this.btnLuuThoiHan.Text = "💾 Lưu Thay Đổi";
            this.btnLuuThoiHan.UseVisualStyleBackColor = false;
            this.btnLuuThoiHan.Click += new System.EventHandler(this.btnLuuThoiHan_Click);
            // 
            // lblThoiHanDesc
            // 
            this.lblThoiHanDesc.AutoSize = true;
            this.lblThoiHanDesc.Location = new System.Drawing.Point(13, 56);
            this.lblThoiHanDesc.Name = "lblThoiHanDesc";
            this.lblThoiHanDesc.Size = new System.Drawing.Size(864, 25);
            this.lblThoiHanDesc.TabIndex = 2;
            this.lblThoiHanDesc.Text = "Chỉnh sửa ngày bắt đầu, ngày kết thúc và trạng thái khóa thủ công. Cột \'Đã Khóa\' " +
    "sẽ tự động tính toán.";
            // 
            // lblThoiHanTitle
            // 
            this.lblThoiHanTitle.AutoSize = true;
            this.lblThoiHanTitle.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThoiHanTitle.Location = new System.Drawing.Point(13, 13);
            this.lblThoiHanTitle.Name = "lblThoiHanTitle";
            this.lblThoiHanTitle.Size = new System.Drawing.Size(342, 31);
            this.lblThoiHanTitle.TabIndex = 1;
            this.lblThoiHanTitle.Text = "Cài đặt Thời hạn Nhập điểm";
            // 
            // dgvThoiHanDiem
            // 
            this.dgvThoiHanDiem.AllowUserToAddRows = false;
            this.dgvThoiHanDiem.AllowUserToDeleteRows = false;
            this.dgvThoiHanDiem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvThoiHanDiem.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvThoiHanDiem.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvThoiHanDiem.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvThoiHanDiem.ColumnHeadersHeight = 40;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvThoiHanDiem.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvThoiHanDiem.Location = new System.Drawing.Point(13, 129);
            this.dgvThoiHanDiem.Name = "dgvThoiHanDiem";
            this.dgvThoiHanDiem.RowHeadersWidth = 51;
            this.dgvThoiHanDiem.RowTemplate.Height = 35;
            this.dgvThoiHanDiem.Size = new System.Drawing.Size(916, 398);
            this.dgvThoiHanDiem.TabIndex = 0;
            // 
            // tabLenLop
            // 
            this.tabLenLop.BackColor = System.Drawing.Color.White;
            this.tabLenLop.Controls.Add(this.cboKhoiFilter_LenLop);
            this.tabLenLop.Controls.Add(this.lblKhoiFilter_LenLop);
            this.tabLenLop.Controls.Add(this.btnThucHienLenLop);
            this.tabLenLop.Controls.Add(this.lblLenLopDesc);
            this.tabLenLop.Controls.Add(this.lblLenLopTitle);
            this.tabLenLop.Controls.Add(this.dgvLenLop);
            this.tabLenLop.Location = new System.Drawing.Point(4, 38);
            this.tabLenLop.Name = "tabLenLop";
            this.tabLenLop.Padding = new System.Windows.Forms.Padding(10);
            this.tabLenLop.Size = new System.Drawing.Size(942, 608);
            this.tabLenLop.TabIndex = 2;
            this.tabLenLop.Text = "Xét Lên Lớp";
            // 
            // cboKhoiFilter_LenLop
            // 
            this.cboKhoiFilter_LenLop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboKhoiFilter_LenLop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKhoiFilter_LenLop.FormattingEnabled = true;
            this.cboKhoiFilter_LenLop.Location = new System.Drawing.Point(753, 90);
            this.cboKhoiFilter_LenLop.Name = "cboKhoiFilter_LenLop";
            this.cboKhoiFilter_LenLop.Size = new System.Drawing.Size(176, 33);
            this.cboKhoiFilter_LenLop.TabIndex = 8;
            this.cboKhoiFilter_LenLop.SelectedIndexChanged += new System.EventHandler(this.cboKhoiFilter_LenLop_SelectedIndexChanged);
            // 
            // lblKhoiFilter_LenLop
            // 
            this.lblKhoiFilter_LenLop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblKhoiFilter_LenLop.AutoSize = true;
            this.lblKhoiFilter_LenLop.Location = new System.Drawing.Point(663, 93);
            this.lblKhoiFilter_LenLop.Name = "lblKhoiFilter_LenLop";
            this.lblKhoiFilter_LenLop.Size = new System.Drawing.Size(84, 25);
            this.lblKhoiFilter_LenLop.TabIndex = 7;
            this.lblKhoiFilter_LenLop.Text = "Lọc khối:";
            // 
            // btnThucHienLenLop
            // 
            this.btnThucHienLenLop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnThucHienLenLop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnThucHienLenLop.FlatAppearance.BorderSize = 0;
            this.btnThucHienLenLop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThucHienLenLop.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThucHienLenLop.ForeColor = System.Drawing.Color.White;
            this.btnThucHienLenLop.Location = new System.Drawing.Point(629, 545);
            this.btnThucHienLenLop.Name = "btnThucHienLenLop";
            this.btnThucHienLenLop.Size = new System.Drawing.Size(300, 50);
            this.btnThucHienLenLop.TabIndex = 6;
            this.btnThucHienLenLop.Text = "⚠️ Thực Hiện Lên Lớp (Hàng loạt)";
            this.btnThucHienLenLop.UseVisualStyleBackColor = false;
            this.btnThucHienLenLop.Click += new System.EventHandler(this.btnThucHienLenLop_Click);
            // 
            // lblLenLopDesc
            // 
            this.lblLenLopDesc.AutoSize = true;
            this.lblLenLopDesc.Location = new System.Drawing.Point(13, 56);
            this.lblLenLopDesc.Name = "lblLenLopDesc";
            this.lblLenLopDesc.Size = new System.Drawing.Size(840, 25);
            this.lblLenLopDesc.TabIndex = 5;
            this.lblLenLopDesc.Text = "Chọn lớp mới cho học sinh Lên Lớp (> 5đ) và Ở Lại Lớp (<= 5đ). Lớp 5 sẽ tự động x" +
    "ét Tốt nghiệp.";
            // 
            // lblLenLopTitle
            // 
            this.lblLenLopTitle.AutoSize = true;
            this.lblLenLopTitle.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLenLopTitle.Location = new System.Drawing.Point(13, 13);
            this.lblLenLopTitle.Name = "lblLenLopTitle";
            this.lblLenLopTitle.Size = new System.Drawing.Size(331, 31);
            this.lblLenLopTitle.TabIndex = 4;
            this.lblLenLopTitle.Text = "Nghiệp vụ Cuối Năm Học";
            // 
            // dgvLenLop
            // 
            this.dgvLenLop.AllowUserToAddRows = false;
            this.dgvLenLop.AllowUserToDeleteRows = false;
            this.dgvLenLop.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLenLop.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvLenLop.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvLenLop.ColumnHeadersHeight = 40;
            this.dgvLenLop.Location = new System.Drawing.Point(13, 129);
            this.dgvLenLop.Name = "dgvLenLop";
            this.dgvLenLop.RowHeadersWidth = 51;
            this.dgvLenLop.RowTemplate.Height = 35;
            this.dgvLenLop.Size = new System.Drawing.Size(916, 398);
            this.dgvLenLop.TabIndex = 1;
            // 
            // UC_QuanLyTruongHoc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tabControl1);
            this.Name = "UC_QuanLyTruongHoc";
            this.Size = new System.Drawing.Size(950, 650);
            this.Load += new System.EventHandler(this.UC_QuanLyTruongHoc_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabMonHoc.ResumeLayout(false);
            this.pnlMonHoc.ResumeLayout(false);
            this.pnlMonHoc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonHoc)).EndInit();
            this.tabThoiHanDiem.ResumeLayout(false);
            this.tabThoiHanDiem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThoiHanDiem)).EndInit();
            this.tabLenLop.ResumeLayout(false);
            this.tabLenLop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLenLop)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabMonHoc;
        private System.Windows.Forms.TabPage tabThoiHanDiem;
        private System.Windows.Forms.DataGridView dgvMonHoc;
        private System.Windows.Forms.Panel pnlMonHoc;
        private System.Windows.Forms.Button btnXoaMon;
        private System.Windows.Forms.Button btnSuaMon;
        private System.Windows.Forms.Button btnThemMon;
        private System.Windows.Forms.TextBox txtTenMon;
        private System.Windows.Forms.Label lblTenMon;
        private System.Windows.Forms.TextBox txtMaMon;
        private System.Windows.Forms.Label lblMaMon;
        private System.Windows.Forms.Label lblTitleMonHoc;
        private System.Windows.Forms.Button btnMoiMon;
        private System.Windows.Forms.DataGridView dgvThoiHanDiem;
        private System.Windows.Forms.Label lblThoiHanTitle;
        private System.Windows.Forms.Label lblThoiHanDesc;
        private System.Windows.Forms.Button btnLuuThoiHan;
        private System.Windows.Forms.TabPage tabLenLop;
        private System.Windows.Forms.DataGridView dgvLenLop;
        private System.Windows.Forms.Button btnThucHienLenLop;
        private System.Windows.Forms.Label lblLenLopDesc;
        private System.Windows.Forms.Label lblLenLopTitle;
        private System.Windows.Forms.ComboBox cboKhoiFilter;
        private System.Windows.Forms.Label lblKhoiFilter;
        private System.Windows.Forms.ComboBox cboKhoiFilter_LenLop;
        private System.Windows.Forms.Label lblKhoiFilter_LenLop;
    }
}