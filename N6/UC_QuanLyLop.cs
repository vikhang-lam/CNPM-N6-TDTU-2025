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
        private string _currentMaMon;
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

        public UC_QuanLyLop(string username)
        {
            InitializeComponent();
            _username = username ?? string.Empty;
            _maGV = DatabaseHelper.GetMaGVByUsername(_username);
            _currentMaMon = DatabaseHelper.GetMonByTeacher(username);

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

            StyleDataGridViewModern(dgvDiemDanh);
            StyleDataGridViewModern(dgvKetQua);
            StyleDataGridViewModern(dgvHocSinh);
            StyleDataGridViewModern(dgvKi1);
            StyleDataGridViewModern(dgvKi2);
            StyleDataGridViewModern(dgvHomeroomGradebook);

            dgvHomeroomGradebook.ReadOnly = true;
            dgvHomeroomGradebook.CellFormatting += dgvHomeroomGradebook_CellFormatting;

            dgvDiemDanh.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvHocSinh.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvKi1.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvKi2.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvHomeroomGradebook.DataBindingComplete += DataGridView_DataBindingComplete;
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
                    Text = "⭐ Lớp Chủ Nhiệm",
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

            // Thêm menu trượt vào form và đưa ra phía sau
            

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
            ShowHomeroomGradebook();
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

        // ... Các vùng code khác (UI styling, Homeroom, Regular Views...) giữ nguyên và đã được định dạng sạch sẽ
        // ... Tôi sẽ dán lại đầy đủ để đảm bảo bạn không bị thiếu code

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
                if (dgv.Columns.Contains("MaHS")) dgv.Columns["MaHS"].Visible = false;
                if (dgv.Columns.Contains("HoTen")) dgv.Columns["HoTen"].HeaderText = "Họ và Tên";
                if (dgv.Columns.Contains("GioiTinh")) dgv.Columns["GioiTinh"].HeaderText = "Giới Tính";
                if (dgv.Columns.Contains("NgaySinh")) { dgv.Columns["NgaySinh"].HeaderText = "Ngày Sinh"; dgv.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy"; }
                if (dgv.Columns.Contains("DiaChi")) dgv.Columns["DiaChi"].HeaderText = "Địa Chỉ";
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

        private void ShowHomeroomGradebook()
        {
            panelContent.Controls.Clear();

            Panel filterPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };
            Label lblFilter = new Label { Text = "Xem điểm:", Dock = DockStyle.Left, AutoSize = true, Padding = new Padding(0, 5, 0, 0), Font = new Font("Segoe UI", 10F) };
            cbLoaiDiem = new ComboBox { Dock = DockStyle.Left, DropDownStyle = ComboBoxStyle.DropDownList, Width = 150, Font = new Font("Segoe UI", 10F) };
            cbLoaiDiem.Items.AddRange(new object[] { "GiuaKi1", "CuoiKi1", "GiuaKi2", "CuoiKi2" });
            cbLoaiDiem.SelectedIndex = 0;
            cbLoaiDiem.SelectedIndexChanged += (s, e) => LoadHomeroomGradebookData();

            filterPanel.Controls.Add(cbLoaiDiem);
            filterPanel.Controls.Add(lblFilter);

            dgvHomeroomGradebook.Dock = DockStyle.Fill;

            panelContent.Controls.Add(dgvHomeroomGradebook);
            panelContent.Controls.Add(filterPanel);

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
            lblThongKeTC = new Label { AutoSize = true, ForeColor = Color.Maroon, Padding = new Padding(15, 5, 0, 0), Font = new Font("Segoe UI", 9F, FontStyle.Bold) };

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

        private void ShowKetQua()
        {
            SaveAllCurrentEdits();
            StopQrCamera();
            panelContent.Controls.Clear();
            if (string.IsNullOrEmpty(_maLop)) { MessageBox.Show("Vui lòng chọn một lớp trước."); return; }
            if (string.IsNullOrEmpty(_currentMaMon)) { MessageBox.Show("Giáo viên chưa được gán môn."); return; }
            TabControl tab = new TabControl { Dock = DockStyle.Fill };
            TabPage p1 = new TabPage("Học Kì 1");
            TabPage p2 = new TabPage("Học Kì 2");
            DataTable dt1 = DatabaseHelper.GetBangDiemPivot(_maLop, 1, _currentMaMon);
            DataTable dt2 = DatabaseHelper.GetBangDiemPivot(_maLop, 2, _currentMaMon);
            SetupResultGrid(dgvKi1, dt1, 1, _currentMaMon);
            SetupResultGrid(dgvKi2, dt2, 2, _currentMaMon);
            p1.Controls.Add(dgvKi1);
            p2.Controls.Add(dgvKi2);
            tab.TabPages.Add(p1);
            tab.TabPages.Add(p2);
            panelContent.Controls.Add(tab);
        }

        private void SetupResultGrid(DataGridView dgv, DataTable dt, int ki, string maMon)
        {
            if (dgv == null) return;
            dgv.Dock = DockStyle.Fill;
            dgv.CellEndEdit -= ResultGrid_CellEndEdit;
            dgv.CurrentCellDirtyStateChanged -= ResultGrid_CurrentCellDirtyStateChanged;
            dgv.DataSource = null;
            dgv.Columns.Clear();
            dgv.AutoGenerateColumns = true;
            dgv.DataSource = dt;
            if (dt != null && dt.Columns.Contains("HoTen") && dgv.Columns.Contains("HoTen")) dgv.Columns["HoTen"].ReadOnly = true;
            if (dt != null && dt.Columns.Contains("MaHS") && dgv.Columns.Contains("MaHS")) dgv.Columns["MaHS"].Visible = false;
            dgv.Tag = Tuple.Create(ki, maMon);
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
            string maMon = ctx.Item2;
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
            dgvHocSinh.DataSource = DatabaseHelper.GetHocSinhByLop(_maLop);
            dgvHocSinh.RowValidated -= DgvHocSinh_RowValidated;
            dgvHocSinh.RowValidated += DgvHocSinh_RowValidated;
            panelContent.Controls.Add(dgvHocSinh);
        }

        private void DgvHocSinh_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvHocSinh.CurrentRow == null || dgvHocSinh.CurrentRow.IsNewRow) return;
                var row = dgvHocSinh.CurrentRow;
                string maHS = row.Cells["MaHS"]?.Value?.ToString();
                if (string.IsNullOrEmpty(maHS)) return;
                string hoTen = row.Cells["HoTen"]?.Value?.ToString();
                string gioiTinh = row.Cells["GioiTinh"]?.Value?.ToString();
                DateTime? ns = null;
                if (row.Cells["NgaySinh"]?.Value != null && row.Cells["NgaySinh"].Value != DBNull.Value)
                    ns = Convert.ToDateTime(row.Cells["NgaySinh"].Value);
                string diaChi = row.Cells["DiaChi"]?.Value?.ToString();
                DatabaseHelper.UpdateHocSinh(maHS, hoTen, gioiTinh, ns, diaChi);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi lưu học sinh: " + ex.Message); }
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