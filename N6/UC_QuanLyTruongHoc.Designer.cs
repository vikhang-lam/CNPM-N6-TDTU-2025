namespace N6
{
    partial class UC_QuanLyTruongHoc
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControlMain = new System.Windows.Forms.TabControl(); // Giữ nguyên TabControl
            this.tabMonHoc = new System.Windows.Forms.TabPage();
            this.dgvMonHoc = new System.Windows.Forms.DataGridView();
            this.panelMonHocInput = new System.Windows.Forms.Panel();
            this.btnMoiMon = new System.Windows.Forms.Button();
            this.btnXoaMon = new System.Windows.Forms.Button();
            this.btnSuaMon = new System.Windows.Forms.Button();
            this.btnThemMon = new System.Windows.Forms.Button();
            this.txtTenMon = new System.Windows.Forms.TextBox();
            this.lblTenMon = new System.Windows.Forms.Label();
            this.txtMaMon = new System.Windows.Forms.TextBox();
            this.lblMaMon = new System.Windows.Forms.Label();
            this.lblTitleMonHoc = new System.Windows.Forms.Label();
            this.tabThoiHanDiem = new System.Windows.Forms.TabPage();
            this.cboKhoiFilter = new System.Windows.Forms.ComboBox();
            this.lblKhoiFilter = new System.Windows.Forms.Label();
            this.btnLuuThoiHan = new System.Windows.Forms.Button();
            this.lblThoiHanDesc = new System.Windows.Forms.Label();
            this.lblThoiHanTitle = new System.Windows.Forms.Label();
            this.dgvThoiHanDiem = new System.Windows.Forms.DataGridView();
            this.tabLenLop = new System.Windows.Forms.TabPage();
            this.pnlLenLopMain = new System.Windows.Forms.Panel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.btnThucHienLenLop_SingleClass = new System.Windows.Forms.Button();
            this.pnlFailingStudents = new System.Windows.Forms.Panel();
            this.lblFailingNote = new System.Windows.Forms.Label();
            this.cboLopMoi_OLaiLop = new System.Windows.Forms.ComboBox();
            this.lblRepeatClassPrompt = new System.Windows.Forms.Label();
            this.lstFailingStudents = new System.Windows.Forms.ListBox();
            this.lblFailingCount = new System.Windows.Forms.Label();
            this.pnlPassingStudents = new System.Windows.Forms.Panel();
            this.cboLopMoi_LenLop = new System.Windows.Forms.ComboBox();
            this.lblNextClassPrompt = new System.Windows.Forms.Label();
            this.lstPassingStudents = new System.Windows.Forms.ListBox();
            this.lblPassingCount = new System.Windows.Forms.Label();
            this.btnLoadLopData = new System.Windows.Forms.Button();
            this.cboLopCu = new System.Windows.Forms.ComboBox();
            this.lblSelectClassPrompt = new System.Windows.Forms.Label();
            this.lblLenLopTitle = new System.Windows.Forms.Label();
            this.tabControlMain.SuspendLayout();
            this.tabMonHoc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonHoc)).BeginInit();
            this.panelMonHocInput.SuspendLayout();
            this.tabThoiHanDiem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThoiHanDiem)).BeginInit();
            this.tabLenLop.SuspendLayout();
            this.pnlLenLopMain.SuspendLayout();
            this.pnlFailingStudents.SuspendLayout();
            this.pnlPassingStudents.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            // KHÔNG dùng Appearance = FlatButtons nữa
            // this.tabControlMain.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControlMain.Controls.Add(this.tabMonHoc);
            this.tabControlMain.Controls.Add(this.tabThoiHanDiem);
            this.tabControlMain.Controls.Add(this.tabLenLop);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlMain.ItemSize = new System.Drawing.Size(180, 40);
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.Padding = new System.Drawing.Point(15, 5);
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(950, 650);
            this.tabControlMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlMain.TabIndex = 0;
            // *** THÊM 2 DÒNG NÀY ĐỂ VẼ CUSTOM ***
            this.tabControlMain.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlMain.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControlMain_DrawItem);
            // *** HẾT PHẦN THÊM ***
            // 
            // tabMonHoc
            // 
            this.tabMonHoc.BackColor = System.Drawing.Color.White;
            this.tabMonHoc.Controls.Add(this.dgvMonHoc);
            this.tabMonHoc.Controls.Add(this.panelMonHocInput);
            this.tabMonHoc.Location = new System.Drawing.Point(4, 44); // Vị trí Y có thể thay đổi tùy DrawMode
            this.tabMonHoc.Name = "tabMonHoc";
            this.tabMonHoc.Padding = new System.Windows.Forms.Padding(15);
            this.tabMonHoc.Size = new System.Drawing.Size(942, 602);
            this.tabMonHoc.TabIndex = 0;
            this.tabMonHoc.Text = "  Quản lý Môn học  ";
            // 
            // dgvMonHoc
            // 
            this.dgvMonHoc.AllowUserToAddRows = false;
            this.dgvMonHoc.AllowUserToDeleteRows = false;
            this.dgvMonHoc.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.dgvMonHoc.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvMonHoc.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMonHoc.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvMonHoc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvMonHoc.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvMonHoc.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(239)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMonHoc.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvMonHoc.ColumnHeadersHeight = 45;
            this.dgvMonHoc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMonHoc.EnableHeadersVisualStyles = false;
            this.dgvMonHoc.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvMonHoc.Location = new System.Drawing.Point(15, 125);
            this.dgvMonHoc.MultiSelect = false;
            this.dgvMonHoc.Name = "dgvMonHoc";
            this.dgvMonHoc.ReadOnly = true;
            this.dgvMonHoc.RowHeadersVisible = false;
            this.dgvMonHoc.RowHeadersWidth = 51;
            this.dgvMonHoc.RowTemplate.Height = 40;
            this.dgvMonHoc.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMonHoc.Size = new System.Drawing.Size(912, 462);
            this.dgvMonHoc.TabIndex = 0;
            this.dgvMonHoc.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMonHoc_CellClick);
            // 
            // panelMonHocInput
            // 
            this.panelMonHocInput.Controls.Add(this.btnMoiMon);
            this.panelMonHocInput.Controls.Add(this.btnXoaMon);
            this.panelMonHocInput.Controls.Add(this.btnSuaMon);
            this.panelMonHocInput.Controls.Add(this.btnThemMon);
            this.panelMonHocInput.Controls.Add(this.txtTenMon);
            this.panelMonHocInput.Controls.Add(this.lblTenMon);
            this.panelMonHocInput.Controls.Add(this.txtMaMon);
            this.panelMonHocInput.Controls.Add(this.lblMaMon);
            this.panelMonHocInput.Controls.Add(this.lblTitleMonHoc);
            this.panelMonHocInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMonHocInput.Location = new System.Drawing.Point(15, 15);
            this.panelMonHocInput.Name = "panelMonHocInput";
            this.panelMonHocInput.Padding = new System.Windows.Forms.Padding(10);
            this.panelMonHocInput.Size = new System.Drawing.Size(912, 110);
            this.panelMonHocInput.TabIndex = 1;
            // 
            // btnMoiMon
            // 
            this.btnMoiMon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnMoiMon.FlatAppearance.BorderSize = 0;
            this.btnMoiMon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoiMon.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnMoiMon.ForeColor = System.Drawing.Color.White;
            this.btnMoiMon.Location = new System.Drawing.Point(806, 60);
            this.btnMoiMon.Name = "btnMoiMon";
            this.btnMoiMon.Size = new System.Drawing.Size(93, 35);
            this.btnMoiMon.TabIndex = 8;
            this.btnMoiMon.Text = "Làm Mới";
            this.btnMoiMon.UseVisualStyleBackColor = false;
            this.btnMoiMon.Click += new System.EventHandler(this.btnMoiMon_Click);
            // 
            // btnXoaMon
            // 
            this.btnXoaMon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnXoaMon.FlatAppearance.BorderSize = 0;
            this.btnXoaMon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXoaMon.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnXoaMon.ForeColor = System.Drawing.Color.White;
            this.btnXoaMon.Location = new System.Drawing.Point(707, 60);
            this.btnXoaMon.Name = "btnXoaMon";
            this.btnXoaMon.Size = new System.Drawing.Size(93, 35);
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
            this.btnSuaMon.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSuaMon.ForeColor = System.Drawing.Color.Black;
            this.btnSuaMon.Location = new System.Drawing.Point(608, 60);
            this.btnSuaMon.Name = "btnSuaMon";
            this.btnSuaMon.Size = new System.Drawing.Size(93, 35);
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
            this.btnThemMon.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnThemMon.ForeColor = System.Drawing.Color.White;
            this.btnThemMon.Location = new System.Drawing.Point(509, 60);
            this.btnThemMon.Name = "btnThemMon";
            this.btnThemMon.Size = new System.Drawing.Size(93, 35);
            this.btnThemMon.TabIndex = 5;
            this.btnThemMon.Text = "Thêm";
            this.btnThemMon.UseVisualStyleBackColor = false;
            this.btnThemMon.Click += new System.EventHandler(this.btnThemMon_Click);
            // 
            // txtTenMon
            // 
            this.txtTenMon.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTenMon.Location = new System.Drawing.Point(308, 63);
            this.txtTenMon.Name = "txtTenMon";
            this.txtTenMon.Size = new System.Drawing.Size(180, 30);
            this.txtTenMon.TabIndex = 4;
            // 
            // lblTenMon
            // 
            this.lblTenMon.AutoSize = true;
            this.lblTenMon.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenMon.Location = new System.Drawing.Point(227, 66);
            this.lblTenMon.Name = "lblTenMon";
            this.lblTenMon.Size = new System.Drawing.Size(81, 23);
            this.lblTenMon.TabIndex = 3;
            this.lblTenMon.Text = "Tên môn:";
            // 
            // txtMaMon
            // 
            this.txtMaMon.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaMon.Location = new System.Drawing.Point(97, 63);
            this.txtMaMon.Name = "txtMaMon";
            this.txtMaMon.ReadOnly = true;
            this.txtMaMon.Size = new System.Drawing.Size(110, 30);
            this.txtMaMon.TabIndex = 2;
            // 
            // lblMaMon
            // 
            this.lblMaMon.AutoSize = true;
            this.lblMaMon.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaMon.Location = new System.Drawing.Point(13, 66);
            this.lblMaMon.Name = "lblMaMon";
            this.lblMaMon.Size = new System.Drawing.Size(79, 23);
            this.lblMaMon.TabIndex = 1;
            this.lblMaMon.Text = "Mã môn:";
            // 
            // lblTitleMonHoc
            // 
            this.lblTitleMonHoc.AutoSize = true;
            this.lblTitleMonHoc.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleMonHoc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.lblTitleMonHoc.Location = new System.Drawing.Point(10, 10);
            this.lblTitleMonHoc.Name = "lblTitleMonHoc";
            this.lblTitleMonHoc.Size = new System.Drawing.Size(206, 31);
            this.lblTitleMonHoc.TabIndex = 0;
            this.lblTitleMonHoc.Text = "Danh sách Môn học";
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
            this.tabThoiHanDiem.Location = new System.Drawing.Point(4, 44);
            this.tabThoiHanDiem.Name = "tabThoiHanDiem";
            this.tabThoiHanDiem.Padding = new System.Windows.Forms.Padding(15);
            this.tabThoiHanDiem.Size = new System.Drawing.Size(942, 602);
            this.tabThoiHanDiem.TabIndex = 1;
            this.tabThoiHanDiem.Text = "  Khóa/Mở Nhập điểm  ";
            // 
            // cboKhoiFilter
            // 
            this.cboKhoiFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboKhoiFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKhoiFilter.Font = new System.Drawing.Font("Segoe UI", 10.2F);
            this.cboKhoiFilter.FormattingEnabled = true;
            this.cboKhoiFilter.Location = new System.Drawing.Point(796, 75);
            this.cboKhoiFilter.Name = "cboKhoiFilter";
            this.cboKhoiFilter.Size = new System.Drawing.Size(130, 31);
            this.cboKhoiFilter.TabIndex = 5;
            this.cboKhoiFilter.SelectedIndexChanged += new System.EventHandler(this.cboKhoiFilter_SelectedIndexChanged);
            // 
            // lblKhoiFilter
            // 
            this.lblKhoiFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblKhoiFilter.AutoSize = true;
            this.lblKhoiFilter.Font = new System.Drawing.Font("Segoe UI", 10.2F);
            this.lblKhoiFilter.Location = new System.Drawing.Point(714, 78);
            this.lblKhoiFilter.Name = "lblKhoiFilter";
            this.lblKhoiFilter.Size = new System.Drawing.Size(78, 23);
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
            this.btnLuuThoiHan.Location = new System.Drawing.Point(726, 537);
            this.btnLuuThoiHan.Name = "btnLuuThoiHan";
            this.btnLuuThoiHan.Size = new System.Drawing.Size(200, 45);
            this.btnLuuThoiHan.TabIndex = 3;
            this.btnLuuThoiHan.Text = "💾 Lưu Thay Đổi";
            this.btnLuuThoiHan.UseVisualStyleBackColor = false;
            this.btnLuuThoiHan.Click += new System.EventHandler(this.btnLuuThoiHan_Click);
            // 
            // lblThoiHanDesc
            // 
            this.lblThoiHanDesc.AutoSize = true;
            this.lblThoiHanDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblThoiHanDesc.Location = new System.Drawing.Point(18, 55);
            this.lblThoiHanDesc.Name = "lblThoiHanDesc";
            this.lblThoiHanDesc.Size = new System.Drawing.Size(811, 23);
            this.lblThoiHanDesc.TabIndex = 2;
            this.lblThoiHanDesc.Text = "Chỉnh sửa ngày bắt đầu, ngày kết thúc nhập điểm và trạng thái khóa thủ công. Cột" +
    " \'Đã Khóa\' sẽ tự động tính toán.";
            // 
            // lblThoiHanTitle
            // 
            this.lblThoiHanTitle.AutoSize = true;
            this.lblThoiHanTitle.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThoiHanTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.lblThoiHanTitle.Location = new System.Drawing.Point(15, 15);
            this.lblThoiHanTitle.Name = "lblThoiHanTitle";
            this.lblThoiHanTitle.Size = new System.Drawing.Size(342, 31);
            this.lblThoiHanTitle.TabIndex = 1;
            this.lblThoiHanTitle.Text = "Cài đặt Thời hạn Nhập điểm";
            // 
            // dgvThoiHanDiem
            // 
            this.dgvThoiHanDiem.AllowUserToAddRows = false;
            this.dgvThoiHanDiem.AllowUserToDeleteRows = false;
            this.dgvThoiHanDiem.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.dgvThoiHanDiem.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvThoiHanDiem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvThoiHanDiem.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvThoiHanDiem.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvThoiHanDiem.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvThoiHanDiem.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvThoiHanDiem.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(239)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvThoiHanDiem.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvThoiHanDiem.ColumnHeadersHeight = 45;
            this.dgvThoiHanDiem.EnableHeadersVisualStyles = false;
            this.dgvThoiHanDiem.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvThoiHanDiem.Location = new System.Drawing.Point(18, 118);
            this.dgvThoiHanDiem.Name = "dgvThoiHanDiem";
            this.dgvThoiHanDiem.RowHeadersVisible = false;
            this.dgvThoiHanDiem.RowHeadersWidth = 51;
            this.dgvThoiHanDiem.RowTemplate.Height = 40;
            this.dgvThoiHanDiem.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvThoiHanDiem.Size = new System.Drawing.Size(908, 401);
            this.dgvThoiHanDiem.TabIndex = 0;
            this.dgvThoiHanDiem.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvThoiHanDiem_CellFormatting);
            // 
            // tabLenLop
            // 
            this.tabLenLop.BackColor = System.Drawing.Color.White;
            this.tabLenLop.Controls.Add(this.pnlLenLopMain);
            this.tabLenLop.Location = new System.Drawing.Point(4, 44);
            this.tabLenLop.Name = "tabLenLop";
            this.tabLenLop.Padding = new System.Windows.Forms.Padding(15);
            this.tabLenLop.Size = new System.Drawing.Size(942, 602);
            this.tabLenLop.TabIndex = 2;
            this.tabLenLop.Text = "  Xét Lên Lớp / Cuối Năm  ";
            // 
            // pnlLenLopMain
            // 
            this.pnlLenLopMain.Controls.Add(this.lblSummary);
            this.pnlLenLopMain.Controls.Add(this.btnThucHienLenLop_SingleClass);
            this.pnlLenLopMain.Controls.Add(this.pnlFailingStudents);
            this.pnlLenLopMain.Controls.Add(this.pnlPassingStudents);
            this.pnlLenLopMain.Controls.Add(this.btnLoadLopData);
            this.pnlLenLopMain.Controls.Add(this.cboLopCu);
            this.pnlLenLopMain.Controls.Add(this.lblSelectClassPrompt);
            this.pnlLenLopMain.Controls.Add(this.lblLenLopTitle);
            this.pnlLenLopMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLenLopMain.Location = new System.Drawing.Point(15, 15);
            this.pnlLenLopMain.Name = "pnlLenLopMain";
            this.pnlLenLopMain.Size = new System.Drawing.Size(912, 572);
            this.pnlLenLopMain.TabIndex = 0;
            // 
            // lblSummary
            // 
            this.lblSummary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSummary.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.lblSummary.Location = new System.Drawing.Point(16, 524);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(612, 35);
            this.lblSummary.TabIndex = 7;
            this.lblSummary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnThucHienLenLop_SingleClass
            // 
            this.btnThucHienLenLop_SingleClass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnThucHienLenLop_SingleClass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnThucHienLenLop_SingleClass.Enabled = false;
            this.btnThucHienLenLop_SingleClass.FlatAppearance.BorderSize = 0;
            this.btnThucHienLenLop_SingleClass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThucHienLenLop_SingleClass.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThucHienLenLop_SingleClass.ForeColor = System.Drawing.Color.White;
            this.btnThucHienLenLop_SingleClass.Location = new System.Drawing.Point(634, 514);
            this.btnThucHienLenLop_SingleClass.Name = "btnThucHienLenLop_SingleClass";
            this.btnThucHienLenLop_SingleClass.Size = new System.Drawing.Size(262, 45);
            this.btnThucHienLenLop_SingleClass.TabIndex = 6;
            this.btnThucHienLenLop_SingleClass.Text = "⚠️ Thực Hiện Cho Lớp Này";
            this.btnThucHienLenLop_SingleClass.UseVisualStyleBackColor = false;
            this.btnThucHienLenLop_SingleClass.Click += new System.EventHandler(this.btnThucHienLenLop_SingleClass_Click);
            // 
            // pnlFailingStudents
            // 
            this.pnlFailingStudents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFailingStudents.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.pnlFailingStudents.Controls.Add(this.lblFailingNote);
            this.pnlFailingStudents.Controls.Add(this.cboLopMoi_OLaiLop);
            this.pnlFailingStudents.Controls.Add(this.lblRepeatClassPrompt);
            this.pnlFailingStudents.Controls.Add(this.lstFailingStudents);
            this.pnlFailingStudents.Controls.Add(this.lblFailingCount);
            this.pnlFailingStudents.Location = new System.Drawing.Point(469, 118);
            this.pnlFailingStudents.Name = "pnlFailingStudents";
            this.pnlFailingStudents.Padding = new System.Windows.Forms.Padding(10);
            this.pnlFailingStudents.Size = new System.Drawing.Size(427, 375);
            this.pnlFailingStudents.TabIndex = 5;
            this.pnlFailingStudents.Visible = false;
            // 
            // lblFailingNote
            // 
            this.lblFailingNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFailingNote.AutoSize = true;
            this.lblFailingNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFailingNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblFailingNote.Location = new System.Drawing.Point(17, 342);
            this.lblFailingNote.Name = "lblFailingNote";
            this.lblFailingNote.Size = new System.Drawing.Size(273, 19);
            this.lblFailingNote.TabIndex = 4;
            this.lblFailingNote.Text = "*Chỉ hiển thị các lớp cùng khối với lớp cũ.";
            // 
            // cboLopMoi_OLaiLop
            // 
            this.cboLopMoi_OLaiLop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLopMoi_OLaiLop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLopMoi_OLaiLop.FormattingEnabled = true;
            this.cboLopMoi_OLaiLop.Location = new System.Drawing.Point(170, 308);
            this.cboLopMoi_OLaiLop.Name = "cboLopMoi_OLaiLop";
            this.cboLopMoi_OLaiLop.Size = new System.Drawing.Size(237, 31);
            this.cboLopMoi_OLaiLop.TabIndex = 3;
            // 
            // lblRepeatClassPrompt
            // 
            this.lblRepeatClassPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRepeatClassPrompt.AutoSize = true;
            this.lblRepeatClassPrompt.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRepeatClassPrompt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblRepeatClassPrompt.Location = new System.Drawing.Point(13, 311);
            this.lblRepeatClassPrompt.Name = "lblRepeatClassPrompt";
            this.lblRepeatClassPrompt.Size = new System.Drawing.Size(149, 23);
            this.lblRepeatClassPrompt.TabIndex = 2;
            this.lblRepeatClassPrompt.Text = "Chuyển đến lớp:*";
            // 
            // lstFailingStudents
            // 
            this.lstFailingStudents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFailingStudents.FormattingEnabled = true;
            this.lstFailingStudents.IntegralHeight = false; // <<< THÊM
            this.lstFailingStudents.ItemHeight = 23;
            this.lstFailingStudents.Location = new System.Drawing.Point(13, 46);
            this.lstFailingStudents.Name = "lstFailingStudents";
            this.lstFailingStudents.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstFailingStudents.Size = new System.Drawing.Size(398, 248); // <<< SỬA
            this.lstFailingStudents.TabIndex = 1;
            // 
            // lblFailingCount
            // 
            this.lblFailingCount.AutoSize = true;
            this.lblFailingCount.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblFailingCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.lblFailingCount.Location = new System.Drawing.Point(10, 10);
            this.lblFailingCount.Name = "lblFailingCount";
            this.lblFailingCount.Size = new System.Drawing.Size(262, 28);
            this.lblFailingCount.TabIndex = 0;
            this.lblFailingCount.Text = "Không Đủ Điều Kiện (0 hs)";
            // 
            // pnlPassingStudents
            // 
            this.pnlPassingStudents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPassingStudents.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(249)))), ((int)(((byte)(237)))));
            this.pnlPassingStudents.Controls.Add(this.cboLopMoi_LenLop);
            this.pnlPassingStudents.Controls.Add(this.lblNextClassPrompt);
            this.pnlPassingStudents.Controls.Add(this.lstPassingStudents);
            this.pnlPassingStudents.Controls.Add(this.lblPassingCount);
            this.pnlPassingStudents.Location = new System.Drawing.Point(16, 118);
            this.pnlPassingStudents.Name = "pnlPassingStudents";
            this.pnlPassingStudents.Padding = new System.Windows.Forms.Padding(10);
            this.pnlPassingStudents.Size = new System.Drawing.Size(427, 375);
            this.pnlPassingStudents.TabIndex = 4;
            this.pnlPassingStudents.Visible = false;
            // 
            // cboLopMoi_LenLop
            // 
            this.cboLopMoi_LenLop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLopMoi_LenLop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLopMoi_LenLop.FormattingEnabled = true;
            this.cboLopMoi_LenLop.Location = new System.Drawing.Point(165, 308);
            this.cboLopMoi_LenLop.Name = "cboLopMoi_LenLop";
            this.cboLopMoi_LenLop.Size = new System.Drawing.Size(242, 31);
            this.cboLopMoi_LenLop.TabIndex = 3;
            // 
            // lblNextClassPrompt
            // 
            this.lblNextClassPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNextClassPrompt.AutoSize = true;
            this.lblNextClassPrompt.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNextClassPrompt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.lblNextClassPrompt.Location = new System.Drawing.Point(13, 311);
            this.lblNextClassPrompt.Name = "lblNextClassPrompt";
            this.lblNextClassPrompt.Size = new System.Drawing.Size(143, 23);
            this.lblNextClassPrompt.TabIndex = 2;
            this.lblNextClassPrompt.Text = "Chuyển đến lớp:*";
            // 
            // lstPassingStudents
            // 
            this.lstPassingStudents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPassingStudents.FormattingEnabled = true;
            this.lstPassingStudents.IntegralHeight = false; // <<< THÊM
            this.lstPassingStudents.ItemHeight = 23;
            this.lstPassingStudents.Location = new System.Drawing.Point(13, 46);
            this.lstPassingStudents.Name = "lstPassingStudents";
            this.lstPassingStudents.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstPassingStudents.Size = new System.Drawing.Size(398, 248); // <<< SỬA
            this.lstPassingStudents.TabIndex = 1;
            // 
            // lblPassingCount
            // 
            this.lblPassingCount.AutoSize = true;
            this.lblPassingCount.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblPassingCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.lblPassingCount.Location = new System.Drawing.Point(10, 10);
            this.lblPassingCount.Name = "lblPassingCount";
            this.lblPassingCount.Size = new System.Drawing.Size(215, 28);
            this.lblPassingCount.TabIndex = 0;
            this.lblPassingCount.Text = "Đủ Điều Kiện (0 hs)";
            // 
            // btnLoadLopData
            // 
            this.btnLoadLopData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnLoadLopData.FlatAppearance.BorderSize = 0;
            this.btnLoadLopData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadLopData.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLoadLopData.ForeColor = System.Drawing.Color.White;
            this.btnLoadLopData.Location = new System.Drawing.Point(469, 63);
            this.btnLoadLopData.Name = "btnLoadLopData";
            this.btnLoadLopData.Size = new System.Drawing.Size(147, 31);
            this.btnLoadLopData.TabIndex = 3;
            this.btnLoadLopData.Text = "Xem Dữ Liệu Lớp";
            this.btnLoadLopData.UseVisualStyleBackColor = false;
            this.btnLoadLopData.Click += new System.EventHandler(this.btnLoadLopData_Click);
            // 
            // cboLopCu
            // 
            this.cboLopCu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLopCu.FormattingEnabled = true;
            this.cboLopCu.Location = new System.Drawing.Point(198, 63);
            this.cboLopCu.Name = "cboLopCu";
            this.cboLopCu.Size = new System.Drawing.Size(250, 31);
            this.cboLopCu.TabIndex = 2;
            this.cboLopCu.SelectedIndexChanged += new System.EventHandler(this.cboLopCu_SelectedIndexChanged);
            // 
            // lblSelectClassPrompt
            // 
            this.lblSelectClassPrompt.AutoSize = true;
            this.lblSelectClassPrompt.Location = new System.Drawing.Point(16, 66);
            this.lblSelectClassPrompt.Name = "lblSelectClassPrompt";
            this.lblSelectClassPrompt.Size = new System.Drawing.Size(166, 23);
            this.lblSelectClassPrompt.TabIndex = 1;
            this.lblSelectClassPrompt.Text = "Chọn lớp cần xử lý:";
            // 
            // lblLenLopTitle
            // 
            this.lblLenLopTitle.AutoSize = true;
            this.lblLenLopTitle.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLenLopTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.lblLenLopTitle.Location = new System.Drawing.Point(10, 10);
            this.lblLenLopTitle.Name = "lblLenLopTitle";
            this.lblLenLopTitle.Size = new System.Drawing.Size(331, 31);
            this.lblLenLopTitle.TabIndex = 0;
            this.lblLenLopTitle.Text = "Nghiệp vụ Cuối Năm Học";
            // 
            // UC_QuanLyTruongHoc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tabControlMain);
            this.Name = "UC_QuanLyTruongHoc";
            this.Size = new System.Drawing.Size(950, 650);
            this.Load += new System.EventHandler(this.UC_QuanLyTruongHoc_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tabMonHoc.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonHoc)).EndInit();
            this.panelMonHocInput.ResumeLayout(false);
            this.panelMonHocInput.PerformLayout();
            this.tabThoiHanDiem.ResumeLayout(false);
            this.tabThoiHanDiem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThoiHanDiem)).EndInit();
            this.tabLenLop.ResumeLayout(false);
            this.pnlLenLopMain.ResumeLayout(false);
            this.pnlLenLopMain.PerformLayout();
            this.pnlFailingStudents.ResumeLayout(false);
            this.pnlFailingStudents.PerformLayout();
            this.pnlPassingStudents.ResumeLayout(false);
            this.pnlPassingStudents.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabMonHoc;
        private System.Windows.Forms.TabPage tabThoiHanDiem;
        private System.Windows.Forms.DataGridView dgvMonHoc;
        private System.Windows.Forms.Panel panelMonHocInput;
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
        private System.Windows.Forms.ComboBox cboKhoiFilter;
        private System.Windows.Forms.Label lblKhoiFilter;
        private System.Windows.Forms.Panel pnlLenLopMain;
        private System.Windows.Forms.Button btnLoadLopData;
        private System.Windows.Forms.ComboBox cboLopCu;
        private System.Windows.Forms.Label lblSelectClassPrompt;
        private System.Windows.Forms.Label lblLenLopTitle;
        private System.Windows.Forms.Panel pnlFailingStudents;
        private System.Windows.Forms.ComboBox cboLopMoi_OLaiLop;
        private System.Windows.Forms.Label lblRepeatClassPrompt;
        private System.Windows.Forms.ListBox lstFailingStudents;
        private System.Windows.Forms.Label lblFailingCount;
        private System.Windows.Forms.Panel pnlPassingStudents;
        private System.Windows.Forms.ComboBox cboLopMoi_LenLop;
        private System.Windows.Forms.Label lblNextClassPrompt;
        private System.Windows.Forms.ListBox lstPassingStudents;
        private System.Windows.Forms.Label lblPassingCount;
        private System.Windows.Forms.Button btnThucHienLenLop_SingleClass;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Label lblFailingNote;
    }
}