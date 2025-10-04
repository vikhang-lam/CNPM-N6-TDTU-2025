namespace N6
{
    partial class UC_QuanLyLopHocSinh
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvHocSinh;
        private System.Windows.Forms.Panel panelNhap;
        private System.Windows.Forms.TextBox txtMaHS;
        private System.Windows.Forms.TextBox txtMaLop;
        private System.Windows.Forms.TextBox txtHoTen;
        private System.Windows.Forms.TextBox txtNgaySinh;
        private System.Windows.Forms.ComboBox cboGioiTinh;
        private System.Windows.Forms.TextBox txtSDTPhuHuynh;
        private System.Windows.Forms.TextBox txtDiaChi;
        private System.Windows.Forms.TextBox txtDanToc;
        private System.Windows.Forms.Button btnThem;
        private System.Windows.Forms.Button btnSua;
        private System.Windows.Forms.Button btnXoa;
        private System.Windows.Forms.Button btnLamMoi;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Label lblMaHS;
        private System.Windows.Forms.Label lblMaLop;
        private System.Windows.Forms.Label lblHoTen;
        private System.Windows.Forms.Label lblNgaySinh;
        private System.Windows.Forms.Label lblGioiTinh;
        private System.Windows.Forms.Label lblSDT;
        private System.Windows.Forms.Label lblDiaChi;
        private System.Windows.Forms.Label lblDanToc;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvHocSinh = new System.Windows.Forms.DataGridView();
            this.panelNhap = new System.Windows.Forms.Panel();
            this.lblMaHS = new System.Windows.Forms.Label();
            this.txtMaHS = new System.Windows.Forms.TextBox();
            this.lblMaLop = new System.Windows.Forms.Label();
            this.txtMaLop = new System.Windows.Forms.TextBox();
            this.lblHoTen = new System.Windows.Forms.Label();
            this.txtHoTen = new System.Windows.Forms.TextBox();
            this.lblNgaySinh = new System.Windows.Forms.Label();
            this.txtNgaySinh = new System.Windows.Forms.TextBox();
            this.lblGioiTinh = new System.Windows.Forms.Label();
            this.cboGioiTinh = new System.Windows.Forms.ComboBox();
            this.lblSDT = new System.Windows.Forms.Label();
            this.txtSDTPhuHuynh = new System.Windows.Forms.TextBox();
            this.lblDiaChi = new System.Windows.Forms.Label();
            this.txtDiaChi = new System.Windows.Forms.TextBox();
            this.lblDanToc = new System.Windows.Forms.Label();
            this.txtDanToc = new System.Windows.Forms.TextBox();
            this.btnThem = new System.Windows.Forms.Button();
            this.btnSua = new System.Windows.Forms.Button();
            this.btnXoa = new System.Windows.Forms.Button();
            this.btnLamMoi = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvHocSinh)).BeginInit();
            this.panelNhap.SuspendLayout();
            this.SuspendLayout();

            // dgvHocSinh
            this.dgvHocSinh.Location = new System.Drawing.Point(20, 20);
            this.dgvHocSinh.Name = "dgvHocSinh";
            this.dgvHocSinh.Size = new System.Drawing.Size(900, 300);
            this.dgvHocSinh.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvHocSinh.TabIndex = 0;
            this.dgvHocSinh.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHocSinh_CellClick);

            // panelNhap
            this.panelNhap.Location = new System.Drawing.Point(20, 340);
            this.panelNhap.Name = "panelNhap";
            this.panelNhap.Size = new System.Drawing.Size(900, 260);
            this.panelNhap.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelNhap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // labels & inputs fixed positions
            // MaHS
            this.lblMaHS.AutoSize = true;
            this.lblMaHS.Location = new System.Drawing.Point(16, 16);
            this.lblMaHS.Name = "lblMaHS";
            this.lblMaHS.Size = new System.Drawing.Size(42, 17);
            this.lblMaHS.Text = "Mã HS:";
            this.txtMaHS.Location = new System.Drawing.Point(120, 12);
            this.txtMaHS.Size = new System.Drawing.Size(240, 25);
            this.txtMaHS.Name = "txtMaHS";

            // MaLop
            this.lblMaLop.AutoSize = true;
            this.lblMaLop.Location = new System.Drawing.Point(380, 16);
            this.lblMaLop.Text = "Mã Lớp:";
            this.txtMaLop.Location = new System.Drawing.Point(460, 12);
            this.txtMaLop.Size = new System.Drawing.Size(240, 25);
            this.txtMaLop.Name = "txtMaLop";

            // HoTen
            this.lblHoTen.AutoSize = true;
            this.lblHoTen.Location = new System.Drawing.Point(16, 52);
            this.lblHoTen.Text = "Họ và tên:";
            this.txtHoTen.Location = new System.Drawing.Point(120, 48);
            this.txtHoTen.Size = new System.Drawing.Size(240, 25);
            this.txtHoTen.Name = "txtHoTen";

            // NgaySinh
            this.lblNgaySinh.AutoSize = true;
            this.lblNgaySinh.Location = new System.Drawing.Point(380, 52);
            this.lblNgaySinh.Text = "Ngày sinh (dd/MM/yyyy):";
            this.txtNgaySinh.Location = new System.Drawing.Point(540, 48);
            this.txtNgaySinh.Size = new System.Drawing.Size(160, 25);
            this.txtNgaySinh.Name = "txtNgaySinh";

            // GioiTinh
            this.lblGioiTinh.AutoSize = true;
            this.lblGioiTinh.Location = new System.Drawing.Point(16, 88);
            this.lblGioiTinh.Text = "Giới tính:";
            this.cboGioiTinh.Location = new System.Drawing.Point(120, 84);
            this.cboGioiTinh.Size = new System.Drawing.Size(240, 25);
            this.cboGioiTinh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGioiTinh.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" });
            this.cboGioiTinh.Name = "cboGioiTinh";

            // SDT
            this.lblSDT.AutoSize = true;
            this.lblSDT.Location = new System.Drawing.Point(380, 88);
            this.lblSDT.Text = "SĐT PH:";
            this.txtSDTPhuHuynh.Location = new System.Drawing.Point(460, 84);
            this.txtSDTPhuHuynh.Size = new System.Drawing.Size(240, 25);
            this.txtSDTPhuHuynh.Name = "txtSDTPhuHuynh";

            // DiaChi
            this.lblDiaChi.AutoSize = true;
            this.lblDiaChi.Location = new System.Drawing.Point(16, 124);
            this.lblDiaChi.Text = "Địa chỉ:";
            this.txtDiaChi.Location = new System.Drawing.Point(120, 120);
            this.txtDiaChi.Size = new System.Drawing.Size(580, 25);
            this.txtDiaChi.Name = "txtDiaChi";

            // DanToc
            this.lblDanToc.AutoSize = true;
            this.lblDanToc.Location = new System.Drawing.Point(16, 160);
            this.lblDanToc.Text = "Dân tộc:";
            this.txtDanToc.Location = new System.Drawing.Point(120, 156);
            this.txtDanToc.Size = new System.Drawing.Size(240, 25);
            this.txtDanToc.Name = "txtDanToc";

            // Buttons
            // Them
            this.btnThem.Location = new System.Drawing.Point(20, 200);
            this.btnThem.Size = new System.Drawing.Size(140, 38);
            this.btnThem.Text = "➕ Thêm";
            this.btnThem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThem.BackColor = System.Drawing.Color.FromArgb(0, 150, 200);
            this.btnThem.ForeColor = System.Drawing.Color.White;
            this.btnThem.Click += new System.EventHandler(this.btnThem_Click);

            // Sua
            this.btnSua.Location = new System.Drawing.Point(180, 200);
            this.btnSua.Size = new System.Drawing.Size(140, 38);
            this.btnSua.Text = "✏️ Sửa";
            this.btnSua.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSua.BackColor = System.Drawing.Color.FromArgb(0, 150, 200);
            this.btnSua.ForeColor = System.Drawing.Color.White;
            this.btnSua.Click += new System.EventHandler(this.btnSua_Click);

            // Xoa
            this.btnXoa.Location = new System.Drawing.Point(340, 200);
            this.btnXoa.Size = new System.Drawing.Size(140, 38);
            this.btnXoa.Text = "🗑️ Xóa";
            this.btnXoa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXoa.BackColor = System.Drawing.Color.FromArgb(220, 80, 80);
            this.btnXoa.ForeColor = System.Drawing.Color.White;
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);

            // LamMoi
            this.btnLamMoi.Location = new System.Drawing.Point(500, 200);
            this.btnLamMoi.Size = new System.Drawing.Size(140, 38);
            this.btnLamMoi.Text = "🔄 Làm mới";
            this.btnLamMoi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLamMoi.BackColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this.btnLamMoi.ForeColor = System.Drawing.Color.White;
            this.btnLamMoi.Click += new System.EventHandler(this.btnLamMoi_Click);

            // Import
            this.btnImport.Location = new System.Drawing.Point(660, 200);
            this.btnImport.Size = new System.Drawing.Size(140, 38);
            this.btnImport.Text = "📂 Import Excel";
            this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImport.BackColor = System.Drawing.Color.FromArgb(60, 170, 80);
            this.btnImport.ForeColor = System.Drawing.Color.White;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);

            // add controls to panel
            this.panelNhap.Controls.Add(this.lblMaHS);
            this.panelNhap.Controls.Add(this.txtMaHS);
            this.panelNhap.Controls.Add(this.lblMaLop);
            this.panelNhap.Controls.Add(this.txtMaLop);
            this.panelNhap.Controls.Add(this.lblHoTen);
            this.panelNhap.Controls.Add(this.txtHoTen);
            this.panelNhap.Controls.Add(this.lblNgaySinh);
            this.panelNhap.Controls.Add(this.txtNgaySinh);
            this.panelNhap.Controls.Add(this.lblGioiTinh);
            this.panelNhap.Controls.Add(this.cboGioiTinh);
            this.panelNhap.Controls.Add(this.lblSDT);
            this.panelNhap.Controls.Add(this.txtSDTPhuHuynh);
            this.panelNhap.Controls.Add(this.lblDiaChi);
            this.panelNhap.Controls.Add(this.txtDiaChi);
            this.panelNhap.Controls.Add(this.lblDanToc);
            this.panelNhap.Controls.Add(this.txtDanToc);
            this.panelNhap.Controls.Add(this.btnThem);
            this.panelNhap.Controls.Add(this.btnSua);
            this.panelNhap.Controls.Add(this.btnXoa);
            this.panelNhap.Controls.Add(this.btnLamMoi);
            this.panelNhap.Controls.Add(this.btnImport);

            // main UC
            this.Controls.Add(this.dgvHocSinh);
            this.Controls.Add(this.panelNhap);
            this.BackColor = System.Drawing.Color.FromArgb(245, 250, 255);
            this.Size = new System.Drawing.Size(940, 620);

            ((System.ComponentModel.ISupportInitialize)(this.dgvHocSinh)).EndInit();
            this.panelNhap.ResumeLayout(false);
            this.panelNhap.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
