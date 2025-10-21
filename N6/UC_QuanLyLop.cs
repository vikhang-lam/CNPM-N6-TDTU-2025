using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;

namespace N6
{
    public partial class UC_QuanLyLop : UserControl
    {
        private string _username;
        private string _maLop;
        // private string _currentMaMon; // ### SỬA ###: Đã xóa, không còn dùng biến toàn cục này
        private string _maGV;
        public string SelectedMaLop { get { return _maLop; } }

        private DataGridView dgvHomeroomGradebook;
        private ComboBox cbLoaiDiem;
        private bool isHomeroomView = false;
        private KeyValuePair<string, string> homeroomClassInfo;

        private DataGridView dgvDiemDanh, dgvKetQua, dgvHocSinh, dgvKi1, dgvKi2;
        private DateTimePicker dtpNgayTC, dtpNgayQR, dtpThoiGianTC;
        private ComboBox cbBuoiTC, cbBuoiQR;
        private Button btnBatDauTC, btnLuuTC, btnStartQRScan, btnStopQRScan;
        private Label lblThongKeTC, lblQRStatus;
        private PictureBox picQR;
        private TabControl _tabDiemDanh;
        private TabPage _tabThuCong, _tabQR;
        private bool _manualSessionActive = false;
        private bool _qrScanning = false;

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Timer qrTimer;
        private NewFrameEventHandler videoFrameHandler;
        private Bitmap latestFrame;
        private readonly object _frameLock = new object();

        // ### NEW: Controls cho menu trượt ###
        private Button btnToggleMenu; // Nút để mở/đóng menu
        private Timer animationTimer;
        private bool isMenuOpen = false;
        private const int menuWidth = 200;

        // ### REDESIGNED: Controls cho Quỹ Lớp ###
        private DataGridView dgvQuyLop;
        private Label lblTongThu_Value, lblTongChi_Value, lblTonQuy_Value; // Labels for values
        private Button btnThemKhoanQuy;

        // ### MỚI ###: ComboBox chọn môn học để nhập điểm
        private ComboBox cbMonHocChon;


        public UC_QuanLyLop(string username)
        {
            InitializeComponent();
            _username = username ?? string.Empty;
            _maGV = DatabaseHelper.GetMaGVByUsername(_username);
            // _currentMaMon = DatabaseHelper.GetMonByTeacher(username); // ### SỬA ###: Đã xóa dòng này

            InitializeDynamicControls();


            btnQuayLaiChonLop.Click += (s, e) => ShowLopChonUI();
            rbDiemDanh.CheckedChanged += TabButton_CheckedChanged;
            rbQR.CheckedChanged += TabButton_CheckedChanged;
            rbKetQua.CheckedChanged += TabButton_CheckedChanged;
            rbHocSinh.CheckedChanged += TabButton_CheckedChanged;

            ShowLopChonUI();
        }



        private void InitializeDynamicControls()
        {
            dgvDiemDanh = new DataGridView { Name = "dgvDiemDanh" };
            dgvKetQua = new DataGridView();
            dgvHocSinh = new DataGridView();
            dgvKi1 = new DataGridView();
            dgvKi2 = new DataGridView();
            dgvHomeroomGradebook = new DataGridView();
            dgvQuyLop = new DataGridView { Name = "dgvQuyLop" }; // NEW

            StyleDataGridViewModern(dgvDiemDanh);
            StyleDataGridViewModern(dgvKetQua);
            StyleDataGridViewModern(dgvHocSinh);
            StyleDataGridViewModern(dgvKi1);
            StyleDataGridViewModern(dgvKi2);
            StyleDataGridViewModern(dgvHomeroomGradebook);
            StyleDataGridViewModern(dgvQuyLop); // NEW

            dgvHomeroomGradebook.ReadOnly = true;
            dgvHomeroomGradebook.CellFormatting += dgvHomeroomGradebook_CellFormatting;

            dgvDiemDanh.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvHocSinh.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvKi1.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvKi2.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvHomeroomGradebook.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvQuyLop.DataBindingComplete += DataGridView_DataBindingComplete; // NEW
        }

        #region UI States & Navigation

        private void ShowLopChonUI()
        {
            StopQrCamera();
            isHomeroomView = false;
            panelLopChon.Visible = true;
            panelLopChon.BringToFront();
            panelMainView.Visible = false;

            PopulateClassSelectionScreen();
        }

        private void PopulateClassSelectionScreen()
        {
            flowLayoutPanelLop.Controls.Clear();
            if (string.IsNullOrEmpty(_maGV))
            {
                lblChonLopTitle.Text = "Tài khoản chưa được phân công.";
                return;
            }

            DataTable dtHomeroom = DatabaseHelper.GetHomeroomClassesByTeacher(_maGV);
            if (dtHomeroom.Rows.Count > 0)
            {
                homeroomClassInfo = new KeyValuePair<string, string>(dtHomeroom.Rows[0]["MaLop"].ToString(), dtHomeroom.Rows[0]["TenLop"].ToString());
                var btnHomeroom = new Button
                {
                    Text = "⭐ Lớp Chủ Nhiệm: " + dtHomeroom.Rows[0]["TenLop"].ToString(),
                    Tag = "HOMEROOM",
                    Size = new Size(flowLayoutPanelLop.Width - 40, 80),
                    Margin = new Padding(10, 10, 10, 20),
                    Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                    BackColor = Color.FromArgb(255, 184, 77),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnHomeroom.FlatAppearance.BorderSize = 0;
                btnHomeroom.Click += HomeroomButton_Click;
                flowLayoutPanelLop.Controls.Add(btnHomeroom);
            }

            DataTable dsLop = DatabaseHelper.GetLopByGiaoVien(_maGV);
            if (dsLop.Rows.Count == 0 && dtHomeroom.Rows.Count == 0)
            {
                lblChonLopTitle.Text = "Giáo viên này chưa được phân công lớp nào.";
                return;
            }

            lblChonLopTitle.Text = "Vui lòng chọn lớp để quản lý";
            foreach (DataRow row in dsLop.Rows)
            {
                var btn = new Button
                {
                    Text = row["TenLop"].ToString(),
                    Tag = row["MaLop"].ToString(),
                    Size = new Size(200, 80),
                    Margin = new Padding(10),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    BackColor = Color.FromArgb(45, 45, 65),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += LopButton_Click;
                flowLayoutPanelLop.Controls.Add(btn);
            }
        }

        private void ActivateMainView(string title)
        {
            panelMainView.Visible = true;
            panelLopChon.Visible = false;
            lblTenLopHienTai.Text = title;
        }


        private void LopButton_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            _maLop = btn.Tag.ToString();
            isHomeroomView = false;

            ActivateMainView($"Giảng dạy: {btn.Text}");

            panelTabs.Visible = true;
            panelContent.Controls.Clear();

            if (!rbDiemDanh.Checked)
            {
                rbDiemDanh.Checked = true;
            }
            else
            {
                TabButton_CheckedChanged(rbDiemDanh, EventArgs.Empty);
            }
        }

        private void HomeroomButton_Click(object sender, EventArgs e)
        {
            _maLop = homeroomClassInfo.Key;
            isHomeroomView = true;

            ActivateMainView($"Chủ nhiệm: {homeroomClassInfo.Value}");

            panelTabs.Visible = false;
            ShowHomeroomView(); // NEW
        }


        private void TabButton_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb == null || !rb.Checked || isHomeroomView) return;

            UpdateTabStyles();
            if (rb == rbDiemDanh) ShowDiemDanh(selectQRTab: false);
            else if (rb == rbQR) ShowDiemDanh(selectQRTab: true);
            else if (rb == rbKetQua) ShowKetQua();
            else if (rb == rbHocSinh) ShowHocSinh();
        }

        private void UpdateTabStyles()
        {
            foreach (var ctrl in tableLayoutPanel_Tabs.Controls)
            {
                if (ctrl is RadioButton r)
                {
                    r.BackColor = r.Checked ? Color.FromArgb(45, 45, 65) : Color.WhiteSmoke;
                    r.ForeColor = r.Checked ? Color.White : Color.Black;
                }
            }
        }

        #endregion

        #region UI styling & Formatting

        private void StyleDataGridViewModern(DataGridView dgv)
        {
            if (dgv == null) return;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(242, 245, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 10, 0);
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(45, 45, 45);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Padding = new Padding(10, 0, 10, 0);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 252);
            dgv.RowTemplate.Height = 40;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void SetGridColumnHeaders(DataGridView dgv)
        {
            if (dgv == null || dgv.Columns.Count == 0) return;
            try
            {
                // ### THÊM MỚI ĐOẠN NÀY ###
                if (dgv.Columns.Contains("STT"))
                {
                    var col = dgv.Columns["STT"];
                    col.HeaderText = "STT";
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; // Tự động co dãn
                    col.DisplayIndex = 0; // Đảm bảo nó là cột đầu tiên
                    col.ReadOnly = true; // Không cho sửa
                }
                // #########################

                if (dgv.Columns.Contains("MaHS")) dgv.Columns["MaHS"].Visible = false;
                if (dgv.Columns.Contains("HoTen"))
                {
                    dgv.Columns["HoTen"].HeaderText = "Họ và Tên";
                    if (dgv.Columns.Contains("STT"))
                        dgv.Columns["HoTen"].DisplayIndex = 1; // Đẩy Họ Tên ra sau STT
                }

                if (dgv.Columns.Contains("GioiTinh")) dgv.Columns["GioiTinh"].HeaderText = "Giới Tính";
                if (dgv.Columns.Contains("NgaySinh")) { dgv.Columns["NgaySinh"].HeaderText = "Ngày Sinh"; dgv.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy"; }
                if (dgv.Columns.Contains("DiaChi")) dgv.Columns["DiaChi"].HeaderText = "Địa Chỉ";
                if (dgv.Columns.Contains("TenLop")) dgv.Columns["TenLop"].HeaderText = "Tên Lớp";
                if (dgv.Columns.Contains("DanToc")) dgv.Columns["DanToc"].HeaderText = "Dân Tộc";
                if (dgv.Columns.Contains("SDTPhuHuynh")) dgv.Columns["SDTPhuHuynh"].HeaderText = "SĐT Phụ Huynh";
                if (dgv.Columns.Contains("TrangThai")) dgv.Columns["TrangThai"].HeaderText = "Trạng Thái";
                if (dgv.Columns.Contains("ThoiGianCapNhat")) { var col = dgv.Columns["ThoiGianCapNhat"]; col.HeaderText = "Giờ Điểm Danh"; col.DefaultCellStyle.Format = "HH:mm:ss"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; }
                if (dgv.Columns.Contains("Thang1")) dgv.Columns["Thang1"].HeaderText = "Điểm T1";
                if (dgv.Columns.Contains("Thang2")) dgv.Columns["Thang2"].HeaderText = "Điểm T2";
                if (dgv.Columns.Contains("Thang3")) dgv.Columns["Thang3"].HeaderText = "Điểm T3";
                if (dgv.Columns.Contains("GiuaKi")) dgv.Columns["GiuaKi"].HeaderText = "Điểm Giữa Kì";
                if (dgv.Columns.Contains("CuoiKi")) dgv.Columns["CuoiKi"].HeaderText = "Điểm Cuối Kì";
                if (dgv.Columns.Contains("NhanXet")) dgv.Columns["NhanXet"].HeaderText = "Nhận Xét";
                if (dgv.Columns.Contains("GhiChu")) dgv.Columns["GhiChu"].HeaderText = "Ghi Chú";

                if (dgv.Name == "dgvDiemDanh")
                {
                    if (dgv.Columns.Contains("MaDD")) dgv.Columns["MaDD"].Visible = false;
                    if (dgv.Columns.Contains("NgayDD")) dgv.Columns["NgayDD"].Visible = false;
                    if (dgv.Columns.Contains("Buoi")) dgv.Columns["Buoi"].Visible = false;
                }

                if (dgv == dgvQuyLop)
                {
                    FormatQuyLopGrid();
                }
            }
            catch (Exception) { /* Bỏ qua lỗi nếu cột không tồn tại */ }
        }

        private void DataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            SetGridColumnHeaders(dgv);
        }

        private void dgvHomeroomGradebook_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var dgv = sender as DataGridView;
            if (dgv.Columns[e.ColumnIndex].Name == "HoTen" || dgv.Columns[e.ColumnIndex].Name == "MaHS") return;

            if (e.Value != null && e.Value != DBNull.Value)
            {
                if (float.TryParse(e.Value.ToString(), out float diem))
                {
                    if (diem < 5.0) e.CellStyle.ForeColor = Color.Red;
                    else if (diem >= 8.0) e.CellStyle.ForeColor = Color.Green;
                    else if (diem >= 6.5) e.CellStyle.ForeColor = Color.FromArgb(0, 120, 215);
                    else e.CellStyle.ForeColor = Color.Black;
                }
            }
        }

        #endregion

        #region Homeroom Teacher View

        private void ShowHomeroomView()
        {
            panelContent.Controls.Clear();

            TabControl tabHomeroom = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
            TabPage tabGradebook = new TabPage("Sổ Điểm Chung");
            TabPage tabFund = new TabPage("Quản Lý Quỹ Lớp");

            BuildHomeroomGradebookTab(tabGradebook);
            BuildQuyLopTab(tabFund);

            tabHomeroom.TabPages.Add(tabGradebook);
            tabHomeroom.TabPages.Add(tabFund);

            panelContent.Controls.Add(tabHomeroom);
        }

        private void BuildHomeroomGradebookTab(TabPage tab)
        {
            tab.Controls.Clear();
            tab.BackColor = Color.White;

            Panel filterPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };
            Label lblFilter = new Label { Text = "Xem điểm:", Dock = DockStyle.Left, AutoSize = true, Padding = new Padding(0, 5, 0, 0), Font = new Font("Segoe UI", 10F) };
            cbLoaiDiem = new ComboBox { Dock = DockStyle.Left, DropDownStyle = ComboBoxStyle.DropDownList, Width = 150, Font = new Font("Segoe UI", 10F) };
            cbLoaiDiem.Items.AddRange(new object[] { "GiuaKi1", "CuoiKi1", "GiuaKi2", "CuoiKi2" });
            cbLoaiDiem.SelectedIndex = 0;
            cbLoaiDiem.SelectedIndexChanged += (s, e) => LoadHomeroomGradebookData();

            filterPanel.Controls.Add(cbLoaiDiem);
            filterPanel.Controls.Add(lblFilter);

            dgvHomeroomGradebook.Dock = DockStyle.Fill;

            tab.Controls.Add(dgvHomeroomGradebook);
            tab.Controls.Add(filterPanel);

            LoadHomeroomGradebookData();
        }

        private void LoadHomeroomGradebookData()
        {
            if (cbLoaiDiem.SelectedItem == null) return;
            string loaiDiem = cbLoaiDiem.SelectedItem.ToString();
            DataTable dt = DatabaseHelper.GetHomeroomGradebook(_maLop, loaiDiem);
            dgvHomeroomGradebook.DataSource = dt;
        }

        #endregion

        #region Quỹ Lớp (REDESIGNED)

        // Helper function to create modern stat cards
        private Panel CreateStatCard(string title, Color valueColor, out Label valueLabel)
        {
            Panel card = new Panel
            {
                Size = new Size(180, 70),
                BackColor = Color.White,
                Margin = new Padding(5),
                Padding = new Padding(10)
            };
            // Thêm bo viền cho card
            card.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid);
            };

            Label lblTitle = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI Semibold", 9F),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft
            };

            valueLabel = new Label
            {
                Text = "0",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = valueColor,
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.Add(valueLabel);
            card.Controls.Add(lblTitle);
            return card;
        }

        private void BuildQuyLopTab(TabPage tab)
        {
            tab.Controls.Clear();
            tab.BackColor = Color.White;
            var mainPanel = new Panel { Dock = DockStyle.Fill };

            // Top "Dashboard" Panel
            var dashboardPanel = new Panel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(10), BackColor = Color.White };

            btnThemKhoanQuy = new Button
            {
                Text = "Tạo mới Thu/Chi",
                Dock = DockStyle.Left,
                Size = new Size(160, 70),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Image = null, // Bạn có thể thêm Icon ở đây nếu muốn
                TextImageRelation = TextImageRelation.ImageBeforeText
            };
            btnThemKhoanQuy.FlatAppearance.BorderSize = 0;
            btnThemKhoanQuy.Click += BtnThemKhoanQuy_Click;

            // Stats Panel (using TableLayoutPanel for alignment)
            var statsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Right,
                ColumnCount = 3,
                RowCount = 1,
                AutoSize = true,
                BackColor = Color.White
            };

            Panel thuCard = CreateStatCard("TỔNG THU", Color.FromArgb(0, 150, 64), out lblTongThu_Value);
            Panel chiCard = CreateStatCard("TỔNG CHI", Color.FromArgb(210, 43, 43), out lblTongChi_Value);
            Panel tonCard = CreateStatCard("TỒN QUỸ", Color.FromArgb(0, 80, 155), out lblTonQuy_Value);

            statsPanel.Controls.Add(thuCard, 0, 0);
            statsPanel.Controls.Add(chiCard, 1, 0);
            statsPanel.Controls.Add(tonCard, 2, 0);

            dashboardPanel.Controls.Add(statsPanel);
            dashboardPanel.Controls.Add(btnThemKhoanQuy);

            // DataGridView
            dgvQuyLop.Dock = DockStyle.Fill;
            dgvQuyLop.ReadOnly = true;
            dgvQuyLop.CellClick -= DgvQuyLop_CellClick;
            dgvQuyLop.CellClick += DgvQuyLop_CellClick;

            mainPanel.Controls.Add(dgvQuyLop); // Add grid first
            mainPanel.Controls.Add(dashboardPanel); // Add dashboard on top
            tab.Controls.Add(mainPanel);

            LoadQuyLopData();
        }

        private void BtnThemKhoanQuy_Click(object sender, EventArgs e)
        {
            // Create a modern, responsive dialog form
            using (var form = new Form())
            {
                form.Text = "Tạo Mới Khoản Thu/Chi";
                form.Size = new Size(520, 390); // Rộng hơn một chút
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.BackColor = Color.White;
                form.Font = new Font("Segoe UI", 10F);

                // --- Panel Nội dung (Dùng TableLayoutPanel cho responsive) ---
                var tlp = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(25), // Tăng padding
                    ColumnCount = 2,
                    RowCount = 5,
                    BackColor = Color.White
                };
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150)); // Cột cho Label
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));  // Cột cho Control

                // Định nghĩa chiều cao các dòng
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
                tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Dòng trống ở cuối

                // --- Helper function for labels (Canh lề phải cho đẹp) ---
                Func<string, Label> createLabel = (text) => new Label
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI Semibold", 10F),
                    ForeColor = Color.FromArgb(64, 64, 64),
                    Margin = new Padding(0, 0, 10, 0) // Cách control 10px
                };

                // --- Helper function for controls (Style phẳng, hiện đại) ---
                Action<Control> styleControl = (ctrl) => {
                    ctrl.Dock = DockStyle.Fill;
                    ctrl.Font = new Font("Segoe UI", 10F);
                    ctrl.Margin = new Padding(3, 10, 3, 10); // Căn control vào giữa theo chiều dọc

                    if (ctrl is TextBox)
                    {
                        ((TextBox)ctrl).BorderStyle = BorderStyle.FixedSingle;
                    }
                    else if (ctrl is ComboBox)
                    {
                        ((ComboBox)ctrl).FlatStyle = FlatStyle.System;
                    }
                    else if (ctrl is DateTimePicker)
                    {
                        ((DateTimePicker)ctrl).Format = DateTimePickerFormat.Short;
                    }
                    else if (ctrl is NumericUpDown)
                    {
                        ((NumericUpDown)ctrl).BorderStyle = BorderStyle.FixedSingle;
                    }
                };

                // --- Khởi tạo Controls ---
                var txtGhiChu = new TextBox();
                var cbLoai = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                cbLoai.Items.AddRange(new string[] { "Thu", "Chi" });
                cbLoai.SelectedIndex = 0;

                var dtpNgay = new DateTimePicker();
                var numSoTien = new NumericUpDown
                {
                    Maximum = 1000000000,
                    ThousandsSeparator = true,
                    Increment = 1000 // <-- BƯỚC NHẢY 1000 THEO YÊU CẦU
                };

                // Áp dụng style
                styleControl(txtGhiChu);
                styleControl(cbLoai);
                styleControl(dtpNgay);
                styleControl(numSoTien);

                // Add controls to TLP
                tlp.Controls.Add(createLabel("Tên khoản / Diễn giải:"), 0, 0);
                tlp.Controls.Add(txtGhiChu, 1, 0);

                tlp.Controls.Add(createLabel("Loại giao dịch:"), 0, 1);
                tlp.Controls.Add(cbLoai, 1, 1);

                tlp.Controls.Add(createLabel("Ngày thực hiện:"), 0, 2);
                tlp.Controls.Add(dtpNgay, 1, 2);

                tlp.Controls.Add(createLabel("Số tiền (VNĐ):"), 0, 3);
                tlp.Controls.Add(numSoTien, 1, 3);

                // --- Button Bar Panel (Đảm bảo nút không bị mất) ---
                var buttonPanel = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 65,
                    BackColor = Color.FromArgb(245, 245, 245),
                    Padding = new Padding(0, 0, 25, 0) // Đẩy nút về bên phải
                };

                var btnLuu = new Button
                {
                    Text = "Lưu",
                    DialogResult = DialogResult.OK,
                    Size = new Size(100, 38),
                    BackColor = Color.FromArgb(0, 120, 215),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Semibold", 10F),
                    Dock = DockStyle.Right // <-- Dùng Dock
                };
                btnLuu.FlatAppearance.BorderSize = 0;

                var spacer = new Panel { Width = 10, Dock = DockStyle.Right, BackColor = Color.Transparent }; // Đệm giữa 2 nút

                var btnHuy = new Button
                {
                    Text = "Hủy",
                    DialogResult = DialogResult.Cancel,
                    Size = new Size(90, 38),
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10F),
                    Dock = DockStyle.Right // <-- Dùng Dock
                };
                btnHuy.FlatAppearance.BorderSize = 1;
                btnHuy.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);

                // Canh lề giữa cho các nút
                btnLuu.Margin = new Padding(0, (buttonPanel.Height - btnLuu.Height) / 2, 0, 0);
                btnHuy.Margin = new Padding(0, (buttonPanel.Height - btnHuy.Height) / 2, 0, 0);

                // Thêm nút (theo thứ tự ngược lại vì dùng Dock.Right)
                buttonPanel.Controls.Add(btnLuu);
                buttonPanel.Controls.Add(spacer);
                buttonPanel.Controls.Add(btnHuy);

                form.Controls.Add(tlp);
                form.Controls.Add(buttonPanel);
                form.AcceptButton = btnLuu;
                form.CancelButton = btnHuy;

                // --- Logic xử lý khi bấm Lưu ---
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    if (string.IsNullOrWhiteSpace(txtGhiChu.Text))
                    {
                        MessageBox.Show("Tên khoản/Diễn giải không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (numSoTien.Value <= 0)
                    {
                        MessageBox.Show("Số tiền phải lớn hơn 0.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    try
                    {
                        DatabaseHelper.InsertQuyLop(_maLop, cbLoai.SelectedItem.ToString(), numSoTien.Value, dtpNgay.Value, txtGhiChu.Text);
                        LoadQuyLopData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void LoadQuyLopData()
        {
            if (dgvQuyLop == null) return;
            try
            {
                DataTable dt = DatabaseHelper.GetQuyLopByLop(_maLop);

                dt.Columns.Add("Thu", typeof(decimal));
                dt.Columns.Add("Chi", typeof(decimal));
                dt.Columns.Add("Tồn", typeof(decimal));

                decimal tongThu = 0;
                decimal tongChi = 0;
                decimal ton = 0;

                foreach (DataRow row in dt.Rows)
                {
                    string loai = row["Loai"].ToString();
                    decimal soTien = Convert.ToDecimal(row["SoTien"]);

                    if (loai == "Thu")
                    {
                        row["Thu"] = soTien;
                        tongThu += soTien;
                        ton += soTien;
                    }
                    else if (loai == "Chi")
                    {
                        row["Chi"] = soTien;
                        tongChi += soTien;
                        ton -= soTien;
                    }
                    row["Tồn"] = ton;
                }

                dgvQuyLop.DataSource = null;
                dgvQuyLop.Columns.Clear();
                dgvQuyLop.DataSource = dt;

                if (!dgvQuyLop.Columns.Contains("DeleteColumn"))
                {
                    var deleteCol = new DataGridViewButtonColumn
                    {
                        Name = "DeleteColumn",
                        Text = "Xóa",
                        HeaderText = "",
                        UseColumnTextForButtonValue = true,
                        Width = 60,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                        FlatStyle = FlatStyle.Flat
                    };
                    deleteCol.DefaultCellStyle.BackColor = Color.FromArgb(254, 235, 235);
                    deleteCol.DefaultCellStyle.ForeColor = Color.Maroon;
                    deleteCol.DefaultCellStyle.SelectionBackColor = Color.FromArgb(254, 235, 235);
                    deleteCol.DefaultCellStyle.SelectionForeColor = Color.Maroon;
                    dgvQuyLop.Columns.Add(deleteCol);
                }

                // Update totals in the new dashboard labels
                if (lblTongThu_Value != null) lblTongThu_Value.Text = $"{tongThu:N0}đ";
                if (lblTongChi_Value != null) lblTongChi_Value.Text = $"{tongChi:N0}đ";
                if (lblTonQuy_Value != null) lblTonQuy_Value.Text = $"{ton:N0}đ";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu quỹ lớp: " + ex.Message);
            }
        }

        private void DgvQuyLop_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvQuyLop.Columns[e.ColumnIndex].Name == "DeleteColumn")
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa mục này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        string maQL = dgvQuyLop.Rows[e.RowIndex].Cells["MaQL"].Value.ToString();
                        DatabaseHelper.DeleteQuyLop(maQL);
                        LoadQuyLopData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                    }
                }
            }
        }

        private void FormatQuyLopGrid()
        {
            if (dgvQuyLop == null || dgvQuyLop.Columns.Count == 0) return;
            try
            {
                if (dgvQuyLop.Columns.Contains("MaQL")) dgvQuyLop.Columns["MaQL"].Visible = false;
                if (dgvQuyLop.Columns.Contains("Loai")) dgvQuyLop.Columns["Loai"].Visible = false;
                if (dgvQuyLop.Columns.Contains("SoTien")) dgvQuyLop.Columns["SoTien"].Visible = false;

                if (dgvQuyLop.Columns.Contains("Ngay"))
                {
                    dgvQuyLop.Columns["Ngay"].HeaderText = "Ngày";
                    dgvQuyLop.Columns["Ngay"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvQuyLop.Columns["Ngay"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                if (dgvQuyLop.Columns.Contains("GhiChu"))
                {
                    dgvQuyLop.Columns["GhiChu"].HeaderText = "Diễn giải / Ghi chú";
                    dgvQuyLop.Columns["GhiChu"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                if (dgvQuyLop.Columns.Contains("Thu"))
                {
                    dgvQuyLop.Columns["Thu"].DefaultCellStyle.Format = "N0";
                    dgvQuyLop.Columns["Thu"].DefaultCellStyle.ForeColor = Color.DarkGreen;
                    dgvQuyLop.Columns["Thu"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                if (dgvQuyLop.Columns.Contains("Chi"))
                {
                    dgvQuyLop.Columns["Chi"].DefaultCellStyle.Format = "N0";
                    dgvQuyLop.Columns["Chi"].DefaultCellStyle.ForeColor = Color.Maroon;
                    dgvQuyLop.Columns["Chi"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                if (dgvQuyLop.Columns.Contains("Tồn"))
                {
                    dgvQuyLop.Columns["Tồn"].DefaultCellStyle.Format = "N0";
                    dgvQuyLop.Columns["Tồn"].DefaultCellStyle.Font = new Font(dgvQuyLop.Font, FontStyle.Bold);
                    dgvQuyLop.Columns["Tồn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
            catch (Exception) { /* ignore */ }
        }


        #endregion

        #region Regular Teacher Views (DiemDanh, KetQua, HocSinh)

        private void ShowDiemDanh(bool selectQRTab = false)
        {
            SaveAllCurrentEdits();
            StopQrCamera();
            panelContent.Controls.Clear();
            if (string.IsNullOrEmpty(_maLop))
            {
                MessageBox.Show("Vui lòng chọn một lớp trước.");
                return;
            }

            _tabDiemDanh = new TabControl { Dock = DockStyle.Fill, Appearance = TabAppearance.FlatButtons, ItemSize = new Size(0, 1), SizeMode = TabSizeMode.Fixed };
            _tabThuCong = new TabPage("Điểm danh thủ công");
            _tabQR = new TabPage("Điểm danh QR");

            BuildThuCongTab();
            BuildQRTab();

            _tabDiemDanh.TabPages.Add(_tabThuCong);
            _tabDiemDanh.TabPages.Add(_tabQR);
            panelContent.Controls.Add(_tabDiemDanh);
            _tabDiemDanh.SelectedTab = selectQRTab ? _tabQR : _tabThuCong;
        }

        private void BuildThuCongTab()
        {
            var container = new Panel { Dock = DockStyle.Fill };
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(5), FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            dtpNgayTC = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today, Width = 110, Font = new Font("Segoe UI", 9F) };
            cbBuoiTC = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 80, Font = new Font("Segoe UI", 9F) };
            cbBuoiTC.Items.AddRange(new[] { "Sáng", "Chiều" });
            cbBuoiTC.SelectedIndex = DateTime.Now.Hour < 12 ? 0 : 1;
            dtpThoiGianTC = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "HH:mm", ShowUpDown = true, Value = DateTime.Now, Width = 70, Font = new Font("Segoe UI", 9F) };

            dtpNgayTC.ValueChanged += AttendanceFilter_Changed;
            cbBuoiTC.SelectedIndexChanged += AttendanceFilter_Changed;

            var btnXemTC = new Button { Text = "Xem", Width = 70, Height = 28, BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnXemTC.Click += (s, e) => LoadThuCong(readOnly: dtpNgayTC.Value.Date < DateTime.Today, createIfEmpty: false);
            btnBatDauTC = new Button { Text = "Bắt đầu", Width = 80, Height = 28, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnBatDauTC.Click += BtnBatDauTC_Click;
            btnLuuTC = new Button { Text = "Lưu", Width = 70, Height = 28, BackColor = Color.OrangeRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
            btnLuuTC.Click += BtnLuuTC_Click;
            lblThongKeTC = new Label { AutoSize = true, ForeColor = Color.Maroon, Padding = new Padding(15, 5, 0, 0), Font = new Font("Segoe UI", 11F, FontStyle.Bold) };

            top.Controls.Add(new Label { Text = "Ngày:", AutoSize = true, Padding = new Padding(0, 5, 5, 0) });
            top.Controls.Add(dtpNgayTC);
            top.Controls.Add(new Label { Text = "Buổi:", AutoSize = true, Padding = new Padding(10, 5, 5, 0) });
            top.Controls.Add(cbBuoiTC);
            top.Controls.Add(new Label { Text = "Giờ:", AutoSize = true, Padding = new Padding(10, 5, 5, 0) });
            top.Controls.Add(dtpThoiGianTC);
            top.Controls.Add(btnXemTC);
            top.Controls.Add(btnBatDauTC);
            top.Controls.Add(btnLuuTC);
            top.Controls.Add(lblThongKeTC);

            dgvDiemDanh.Dock = DockStyle.Fill;
            dgvDiemDanh.DataSource = null;
            dgvDiemDanh.Columns.Clear();
            dgvDiemDanh.ReadOnly = true;

            container.Controls.Add(dgvDiemDanh);
            container.Controls.Add(top);
            _tabThuCong.Controls.Clear();
            _tabThuCong.Controls.Add(container);
            LoadThuCong(readOnly: true, createIfEmpty: false);
        }

        private void AttendanceFilter_Changed(object sender, EventArgs e)
        {
            bool isPastDate = dtpNgayTC.Value.Date < DateTime.Today.Date;
            LoadThuCong(readOnly: isPastDate, createIfEmpty: false);
        }

        private void BtnBatDauTC_Click(object sender, EventArgs e)
        {
            if (dtpNgayTC.Value.Date > DateTime.Today)
            {
                MessageBox.Show("Không thể điểm danh cho một ngày trong tương lai.");
                return;
            }
            _manualSessionActive = true;
            btnLuuTC.Enabled = true;
            DateTime thoiDiemDiemDanh = dtpNgayTC.Value.Date + dtpThoiGianTC.Value.TimeOfDay;
            DatabaseHelper.ExecTaoDiemDanhMacDinh(_maLop, thoiDiemDiemDanh, cbBuoiTC.SelectedItem?.ToString());
            LoadThuCong(readOnly: false, createIfEmpty: false);
        }

        private void LoadThuCong(bool readOnly, bool createIfEmpty)
        {
            try
            {
                DateTime ngay = dtpNgayTC.Value.Date;
                string buoi = cbBuoiTC.SelectedItem?.ToString();
                DataTable dt = DatabaseHelper.GetDiemDanhByLopAndDate(_maLop, ngay, buoi);

                if (createIfEmpty && dt.Rows.Count == 0 && ngay >= DateTime.Today && _manualSessionActive && !readOnly)
                {
                    DateTime thoiDiemDiemDanh = dtpNgayTC.Value.Date + dtpThoiGianTC.Value.TimeOfDay;
                    DatabaseHelper.ExecTaoDiemDanhMacDinh(_maLop, thoiDiemDiemDanh, buoi);
                    dt = DatabaseHelper.GetDiemDanhByLopAndDate(_maLop, ngay, buoi);
                }

                foreach (DataRow r in dt.Rows)
                {
                    bool missing = r["TrangThai"] == DBNull.Value || string.IsNullOrWhiteSpace(r["TrangThai"].ToString());
                    if (missing)
                    {
                        if (_manualSessionActive && !readOnly)
                            r["TrangThai"] = "Có mặt";
                        else
                            r["TrangThai"] = "Chưa điểm danh";
                    }
                    else if (string.Equals(r["TrangThai"].ToString(), "Vắng mặt", StringComparison.OrdinalIgnoreCase))
                    {
                        r["TrangThai"] = "Vắng";
                    }
                }

                dgvDiemDanh.DataSource = null;
                dgvDiemDanh.Columns.Clear();
                dgvDiemDanh.AutoGenerateColumns = true;
                dgvDiemDanh.DataSource = dt;

                if (dgvDiemDanh.Columns.Contains("TrangThai"))
                {
                    int idx = dgvDiemDanh.Columns["TrangThai"].Index;
                    dgvDiemDanh.Columns.Remove("TrangThai");
                    var col = new DataGridViewComboBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", DataPropertyName = "TrangThai", FlatStyle = FlatStyle.Flat, DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing };
                    col.Items.AddRange("Có mặt", "Vắng", "Đi trễ", "Vắng (Có phép)", "Chưa điểm danh");
                    dgvDiemDanh.Columns.Insert(idx, col);
                }

                if (dgvDiemDanh.Columns.Contains("MaHS")) dgvDiemDanh.Columns["MaHS"].ReadOnly = true;
                if (dgvDiemDanh.Columns.Contains("HoTen")) dgvDiemDanh.Columns["HoTen"].ReadOnly = true;
                if (dgvDiemDanh.Columns.Contains("ThoiGianCapNhat")) dgvDiemDanh.Columns["ThoiGianCapNhat"].ReadOnly = true;

                dgvDiemDanh.ReadOnly = readOnly || !_manualSessionActive;
                if (!dgvDiemDanh.ReadOnly && dgvDiemDanh.Columns.Contains("TrangThai"))
                {
                    foreach (DataGridViewColumn c in dgvDiemDanh.Columns)
                        c.ReadOnly = (c.Name != "TrangThai");
                }

                dgvDiemDanh.CurrentCellDirtyStateChanged -= DgvThuCong_CurrentCellDirtyStateChanged;
                dgvDiemDanh.CellEndEdit -= DgvThuCong_CellEndEdit;
                dgvDiemDanh.DataError -= DgvDiemDanh_DataError;
                dgvDiemDanh.DataError += DgvDiemDanh_DataError;
                if (!dgvDiemDanh.ReadOnly)
                {
                    dgvDiemDanh.CurrentCellDirtyStateChanged += DgvThuCong_CurrentCellDirtyStateChanged;
                    dgvDiemDanh.CellEndEdit += DgvThuCong_CellEndEdit;
                }
                UpdateThongKeThuCong();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải điểm danh: " + ex.Message);
            }
        }

        private void BtnLuuTC_Click(object sender, EventArgs e)
        {
            if (!_manualSessionActive)
            {
                MessageBox.Show("Chưa ở phiên điểm danh.");
                return;
            }
            try
            {
                dgvDiemDanh.EndEdit();
                int saved = 0;
                DateTime thoiDiemLuu = dtpNgayTC.Value.Date + dtpThoiGianTC.Value.TimeOfDay;
                foreach (DataGridViewRow row in dgvDiemDanh.Rows)
                {
                    if (row.IsNewRow) continue;
                    string maHS = row.Cells["MaHS"]?.Value?.ToString();
                    if (string.IsNullOrEmpty(maHS)) continue;
                    string tt = row.Cells["TrangThai"]?.Value?.ToString();
                    if (string.Equals(tt, "Vắng mặt", StringComparison.OrdinalIgnoreCase)) tt = "Vắng";
                    if (string.IsNullOrWhiteSpace(tt)) tt = "Có mặt";
                    DatabaseHelper.UpsertDiemDanh(maHS, _maLop, thoiDiemLuu, cbBuoiTC.SelectedItem?.ToString(), tt);
                    saved++;
                }
                MessageBox.Show($"Đã lưu {saved} bản ghi.");
                _manualSessionActive = false;
                btnLuuTC.Enabled = false;
                LoadThuCong(readOnly: false, createIfEmpty: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu điểm danh: " + ex.Message);
            }
        }

        private void DgvThuCong_CurrentCellDirtyStateChanged(object sender, EventArgs e) { if (dgvDiemDanh.IsCurrentCellDirty) dgvDiemDanh.CommitEdit(DataGridViewDataErrorContexts.Commit); }
        private void DgvThuCong_CellEndEdit(object sender, DataGridViewCellEventArgs e) { UpdateThongKeThuCong(); }
        private void DgvDiemDanh_DataError(object sender, DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }

        private void UpdateThongKeThuCong()
        {
            if (lblThongKeTC == null || dgvDiemDanh.Rows == null) return;
            int total = 0, absent = 0;
            foreach (DataGridViewRow r in dgvDiemDanh.Rows)
            {
                if (r.IsNewRow) continue;
                total++;
                var status = r.Cells["TrangThai"]?.Value?.ToString();
                if (status != null && status.Contains("Vắng"))
                {
                    absent++;
                }
            }
            int present = total - absent;
            lblThongKeTC.Text = $"Sỉ số: {total} | Hiện diện: {present} | Vắng: {absent}";
        }

        private void BuildQRTab()
        {
            var container = new Panel { Dock = DockStyle.Fill };
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(5), FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            dtpNgayQR = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today, Width = 100 };
            cbBuoiQR = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 80 };
            cbBuoiQR.Items.AddRange(new[] { "Sáng", "Chiều" });
            cbBuoiQR.SelectedIndex = DateTime.Now.Hour < 12 ? 0 : 1;
            btnStartQRScan = new Button { Text = "Quét", Width = 70, Height = 28, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnStopQRScan = new Button { Text = "Dừng", Width = 70, Height = 28, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            lblQRStatus = new Label { AutoSize = true, ForeColor = Color.Navy, Padding = new Padding(15, 5, 0, 0), Text = "Chưa quét" };
            btnStartQRScan.Click += BtnStartQRScan_Click;
            btnStopQRScan.Click += (s, e) => { StopQrCamera(); lblQRStatus.Text = "Đã dừng"; };
            top.Controls.Add(new Label { Text = "Ngày:", AutoSize = true, Padding = new Padding(0, 5, 5, 0) });
            top.Controls.Add(dtpNgayQR);
            top.Controls.Add(new Label { Text = "Buổi:", AutoSize = true, Padding = new Padding(10, 5, 5, 0) });
            top.Controls.Add(cbBuoiQR);
            top.Controls.Add(btnStartQRScan);
            top.Controls.Add(btnStopQRScan);
            top.Controls.Add(lblQRStatus);
            picQR = new PictureBox { Dock = DockStyle.Fill, BackColor = Color.Black, SizeMode = PictureBoxSizeMode.Zoom };
            container.Controls.Add(picQR);
            container.Controls.Add(top);
            _tabQR.Controls.Clear();
            _tabQR.Controls.Add(container);
        }

        private void BtnStartQRScan_Click(object sender, EventArgs e)
        {
            if (dtpNgayQR.Value.Date < DateTime.Today) { MessageBox.Show("Không quét ngày đã qua."); return; }
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices == null || videoDevices.Count == 0) { MessageBox.Show("Không tìm thấy camera."); return; }
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoFrameHandler = new NewFrameEventHandler(Video_NewFrame);
                videoSource.NewFrame += videoFrameHandler;
                videoSource.Start();
                qrTimer = new Timer { Interval = 800 };
                qrTimer.Tick += QrTimer_Tick;
                qrTimer.Start();
                _qrScanning = true;
                lblQRStatus.Text = "Đang quét...";
            }
            catch (Exception ex) { MessageBox.Show("Lỗi camera: " + ex.Message); }
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs e)
        {
            try
            {
                Bitmap frame = (Bitmap)e.Frame.Clone();
                lock (_frameLock)
                {
                    latestFrame?.Dispose();
                    latestFrame = (Bitmap)frame.Clone();
                }
                if (picQR != null && !picQR.IsDisposed)
                {
                    if (picQR.InvokeRequired)
                        picQR.Invoke(new Action(() => { picQR.Image?.Dispose(); picQR.Image = (Bitmap)frame.Clone(); }));
                    else { picQR.Image?.Dispose(); picQR.Image = (Bitmap)frame.Clone(); }
                }
                frame.Dispose();
            }
            catch { }
        }

        private void QrTimer_Tick(object sender, EventArgs e)
        {
            if (!_qrScanning) return;
            try
            {
                Bitmap bmp = null;
                lock (_frameLock)
                {
                    if (latestFrame != null)
                        bmp = (Bitmap)latestFrame.Clone();
                }
                if (bmp == null) return;
                var reader = new BarcodeReader();
                var result = reader.Decode(bmp);
                bmp.Dispose();
                if (result != null)
                {
                    string maHS = result.Text?.Trim();
                    if (!string.IsNullOrEmpty(maHS))
                    {
                        DatabaseHelper.LuuDiemDanh(maHS, "Có mặt", dtpNgayQR.Value.Date, cbBuoiQR.SelectedItem?.ToString());
                        if (lblQRStatus != null && !lblQRStatus.IsDisposed)
                            lblQRStatus.Invoke(new Action(() => lblQRStatus.Text = $"✅ {maHS} - {DateTime.Now:T}"));
                    }
                }
            }
            catch { }
        }

        private void StopQrCamera()
        {
            try
            {
                _qrScanning = false;
                qrTimer?.Stop();
                qrTimer?.Dispose();
                qrTimer = null;
                if (videoSource != null)
                {
                    if (videoSource.IsRunning)
                    {
                        videoSource.SignalToStop();
                        if (videoFrameHandler != null)
                            videoSource.NewFrame -= videoFrameHandler;
                    }
                    videoSource = null;
                }
                if (picQR?.Image != null)
                {
                    picQR.Image.Dispose();
                    picQR.Image = null;
                }
                latestFrame?.Dispose();
                latestFrame = null;
            }
            catch { }
        }

        // ### SỬA ###: Toàn bộ hàm ShowKetQua() đã được viết lại
        private void ShowKetQua()
        {
            SaveAllCurrentEdits();
            StopQrCamera();
            panelContent.Controls.Clear();
            if (string.IsNullOrEmpty(_maLop)) { MessageBox.Show("Vui lòng chọn một lớp trước."); return; }

            // Lấy danh sách môn GV này dạy ở lớp này
            DataTable dtMonHoc = DatabaseHelper.GetMonHocByGiaoVienAndLop(_maGV, _maLop);
            if (dtMonHoc == null || dtMonHoc.Rows.Count == 0)
            {
                MessageBox.Show("Bạn không được phân công giảng dạy môn nào tại lớp này.");
                return;
            }

            // --- Panel chọn môn học ---
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10),
                BackColor = Color.WhiteSmoke
            };
            Label lblChonMon = new Label
            {
                Text = "Chọn môn học:",
                Dock = DockStyle.Left,
                AutoSize = true,
                Padding = new Padding(0, 5, 0, 0),
                Font = new Font("Segoe UI", 10F)
            };
            cbMonHocChon = new ComboBox
            {
                Dock = DockStyle.Left,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 200,
                Font = new Font("Segoe UI", 10F)
            };

            cbMonHocChon.DataSource = dtMonHoc;
            cbMonHocChon.DisplayMember = "TenMon";
            cbMonHocChon.ValueMember = "MaMon";

            cbMonHocChon.SelectedIndexChanged += CbMonHocChon_SelectedIndexChanged;

            topPanel.Controls.Add(cbMonHocChon); // Add ComboBox first
            topPanel.Controls.Add(lblChonMon); // Add Label (it will sit to the left of CB)

            // --- TabControl cho 2 học kỳ ---
            TabControl tab = new TabControl { Dock = DockStyle.Fill };
            TabPage p1 = new TabPage("Học Kì 1");
            TabPage p2 = new TabPage("Học Kì 2");

            // Khởi tạo dgvKi1, dgvKi2 (chúng đã là field)
            dgvKi1.DataSource = null;
            dgvKi1.Columns.Clear();
            dgvKi2.DataSource = null;
            dgvKi2.Columns.Clear();

            p1.Controls.Add(dgvKi1);
            p2.Controls.Add(dgvKi2);
            tab.TabPages.Add(p1);
            tab.TabPages.Add(p2);

            panelContent.Controls.Add(tab); // Add TabControl to fill
            panelContent.Controls.Add(topPanel); // Add TopPanel on top

            // Tải dữ liệu lần đầu
            LoadKetQuaGrids();
        }

        // ### MỚI ###: Hàm tải dữ liệu điểm dựa trên ComboBox
        private void LoadKetQuaGrids()
        {
            if (cbMonHocChon == null || cbMonHocChon.SelectedValue == null) return;

            string selectedMaMon = cbMonHocChon.SelectedValue.ToString();

            if (string.IsNullOrEmpty(selectedMaMon)) return;

            DataTable dt1 = DatabaseHelper.GetBangDiemPivot(_maLop, 1, selectedMaMon);
            DataTable dt2 = DatabaseHelper.GetBangDiemPivot(_maLop, 2, selectedMaMon);

            SetupResultGrid(dgvKi1, dt1, 1, selectedMaMon);
            SetupResultGrid(dgvKi2, dt2, 2, selectedMaMon);
        }

        // ### MỚI ###: Sự kiện khi thay đổi môn học
        private void CbMonHocChon_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadKetQuaGrids();
        }

        private void SetupResultGrid(DataGridView dgv, DataTable dt, int ki, string maMon)
        {
            if (dgv == null) return;
            dgv.Dock = DockStyle.Fill;
            dgv.CellEndEdit -= ResultGrid_CellEndEdit;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.CurrentCellDirtyStateChanged -= ResultGrid_CurrentCellDirtyStateChanged;
            dgv.DataSource = null;
            dgv.Columns.Clear();
            dgv.AutoGenerateColumns = true;
            dgv.DataSource = dt;
            if (dt != null && dt.Columns.Contains("HoTen") && dgv.Columns.Contains("HoTen")) dgv.Columns["HoTen"].ReadOnly = true;
            if (dt != null && dt.Columns.Contains("MaHS") && dgv.Columns.Contains("MaHS")) dgv.Columns["MaHS"].Visible = false;
            dgv.Tag = Tuple.Create(ki, maMon); // ### SỬA ###: Lưu maMon được chọn
            dgv.CurrentCellDirtyStateChanged += ResultGrid_CurrentCellDirtyStateChanged;
            dgv.CellEndEdit += ResultGrid_CellEndEdit;
        }

        private void ResultGrid_CurrentCellDirtyStateChanged(object s, EventArgs e) { var g = s as DataGridView; if (g != null && g.IsCurrentCellDirty) g.CommitEdit(DataGridViewDataErrorContexts.Commit); }
        private void ResultGrid_CellEndEdit(object s, DataGridViewCellEventArgs e) { var g = s as DataGridView; if (g == null || e.RowIndex < 0) return; SaveKetQuaRow(g, e.RowIndex, e.ColumnIndex); }

        private void SaveKetQuaRow(DataGridView dgv, int row, int col)
        {
            if (dgv.Tag == null) return;
            var ctx = dgv.Tag as Tuple<int, string>;
            if (ctx == null) return;
            int ki = ctx.Item1;
            string maMon = ctx.Item2; // ### SỬA ###: Lấy maMon từ Tag
            var r = dgv.Rows[row];
            string maHS = r.Cells["MaHS"]?.Value?.ToString();
            if (string.IsNullOrEmpty(maHS)) return;
            string colName = dgv.Columns[col].DataPropertyName;
            string loai = MapColumnToLoai(colName, ki);
            if (string.IsNullOrEmpty(loai)) return;
            object val = r.Cells[col].Value;
            float? diem = null;
            if (val != null && val != DBNull.Value && float.TryParse(val.ToString(), out float p))
                diem = p;
            DatabaseHelper.UpdateKetQuaHocTap(maHS, maMon, loai, diem);
        }

        private string MapColumnToLoai(string c, int ki)
        {
            if (string.IsNullOrEmpty(c)) return null;
            c = c.Trim();
            if (c.Equals("Thang1", StringComparison.OrdinalIgnoreCase)) return $"Thang1_Ki{ki}";
            if (c.Equals("Thang2", StringComparison.OrdinalIgnoreCase)) return $"Thang2_Ki{ki}";
            if (c.Equals("Thang3", StringComparison.OrdinalIgnoreCase)) return $"Thang3_Ki{ki}";
            if (c.Equals("GiuaKi", StringComparison.OrdinalIgnoreCase)) return $"GiuaKi{ki}";
            if (c.Equals("CuoiKi", StringComparison.OrdinalIgnoreCase)) return $"CuoiKi{ki}";
            if (c.Equals("NhanXet", StringComparison.OrdinalIgnoreCase)) return "NhanXet";
            if (c.Equals("GhiChu", StringComparison.OrdinalIgnoreCase)) return "GhiChu";
            return null;
        }

        private void ShowHocSinh()
        {
            SaveAllCurrentEdits();
            StopQrCamera();
            panelContent.Controls.Clear();
            if (string.IsNullOrEmpty(_maLop)) { MessageBox.Show("Vui lòng chọn một lớp trước."); return; }
            dgvHocSinh.Dock = DockStyle.Fill;
            dgvHocSinh.DataSource = null;
            dgvHocSinh.Columns.Clear();
            dgvHocSinh.AutoGenerateColumns = true;
            dgvHocSinh.AllowUserToAddRows = false;
            dgvHocSinh.AllowUserToDeleteRows = false;
            dgvHocSinh.DataSource = DatabaseHelper.GetHocSinhByLop(_maLop);
            panelContent.Controls.Add(dgvHocSinh);
            dgvHocSinh.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvHocSinh.CellBorderStyle = DataGridViewCellBorderStyle.Single;
        }

        private void SaveAllCurrentEdits()
        {
            try { dgvDiemDanh?.EndEdit(); } catch { }
            try { dgvKi1?.EndEdit(); } catch { }
            try { dgvKi2?.EndEdit(); } catch { }
        }

        #endregion
    }
}