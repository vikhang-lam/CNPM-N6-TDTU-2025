using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZXing;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Diagnostics; // Thêm

namespace N6
{
    /// <summary>
    /// UserControl chính cho phép giáo viên quản lý các hoạt động của lớp học,
    /// bao gồm cả lớp giảng dạy (điểm danh, kết quả) và lớp chủ nhiệm (sổ điểm, quỹ lớp).
    /// </summary>
    public partial class UC_QuanLyLop : UserControl
    {
        #region Fields (Biến thành viên)

        // CHUẨN HÓA: Đã xóa tiền tố '_'
        private string username;
        private string maLop;
        private string maGV;
        /// <summary>
        /// Lấy mã lớp (MaLop) hiện đang được chọn.
        /// </summary>
        public string SelectedMaLop => maLop; // CHUẨN HÓA: Đã xóa '_'

        // Caches
        private DataGridView dgvHomeroomGradebook;
        private ComboBox cbLoaiDiem;
        private bool isHomeroomView = false;
        private KeyValuePair<string, string> homeroomClassInfo;
        private Dictionary<string, bool> lockStatusCache; // CHUẨN HÓA: Đã xóa '_'
        private Label lblNoGradebookData;

        // Dynamic Controls - Attendance
        private DataGridView dgvDiemDanh;
        private DateTimePicker dtpNgayTC, dtpNgayQR, dtpThoiGianTC;
        private ComboBox cbBuoiTC, cbBuoiQR;
        private Button btnBatDauTC, btnLuuTC, btnStartQRScan, btnStopQRScan;
        private Label lblThongKeTC, lblQRStatus;
        private PictureBox picQR;
        private TabControl tabDiemDanh; // CHUẨN HÓA: Đã xóa '_'
        private TabPage tabThuCong, tabQR; // CHUẨN HÓA: Đã xóa '_'
        private bool manualSessionActive = false; // CHUẨN HÓA: Đã xóa '_'
        private bool qrScanning = false; // CHUẨN HÓA: Đã xóa '_'

        // Dynamic Controls - Results
        private DataGridView dgvKetQua, dgvHocSinh, dgvKi1, dgvKi2;
        private ComboBox cbMonHocChon;

        // Dynamic Controls - Homeroom Fund
        private DataGridView dgvQuyLop;
        private Label lblTongThu_Value, lblTongChi_Value, lblTonQuy_Value;
        private Button btnThemKhoanQuy;

        // Camera & QR
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Timer qrTimer;
        private NewFrameEventHandler videoFrameHandler;
        private Bitmap latestFrame;
        private readonly object frameLock = new object(); // CHUẨN HÓA: Đã xóa '_'

        #endregion

        #region Constructor & Initialization

        public UC_QuanLyLop(string username)
        {
            InitializeComponent();
            this.flowLayoutPanelLop.WrapContents = true;

            // CHUẨN HÓA: Sử dụng this. để gán cho field
            this.username = username ?? string.Empty;
            this.maGV = DatabaseHelper.GetTeacherIdByUsername(this.username);

            InitializeDynamicControls();

            lockStatusCache = new Dictionary<string, bool>(); // CHUẨN HÓA: Đã xóa '_'

            // Gán sự kiện cho các control designer
            btnQuayLaiChonLop.Click += btnQuayLaiChonLop_Click;
            rbDiemDanh.CheckedChanged += TabButton_CheckedChanged;
            rbQR.CheckedChanged += TabButton_CheckedChanged;
            rbKetQua.CheckedChanged += TabButton_CheckedChanged;
            rbHocSinh.CheckedChanged += TabButton_CheckedChanged;
            flowLayoutPanelLop.Resize += FlowLayoutPanelLop_Resize;

            ShowLopChonUI();
        }

        /// <summary>
        /// Khởi tạo các đối tượng control sẽ được dùng động (DataGridViews, ComboBoxes, etc.)
        /// </summary>
        private void InitializeDynamicControls()
        {
            // Khởi tạo và đặt tên để dễ debug
            dgvDiemDanh = new DataGridView { Name = "dgvDiemDanh" };
            dgvKetQua = new DataGridView { Name = "dgvKetQua" };
            dgvHocSinh = new DataGridView { Name = "dgvHocSinh" };
            dgvKi1 = new DataGridView { Name = "dgvKi1" };
            dgvKi2 = new DataGridView { Name = "dgvKi2" };
            dgvHomeroomGradebook = new DataGridView { Name = "dgvHomeroomGradebook" };
            dgvQuyLop = new DataGridView { Name = "dgvQuyLop" };

            // Áp dụng style chung
            StyleDataGridViewModern(dgvDiemDanh);
            StyleDataGridViewModern(dgvKetQua);
            StyleDataGridViewModern(dgvHocSinh);
            StyleDataGridViewModern(dgvKi1);
            StyleDataGridViewModern(dgvKi2);
            StyleDataGridViewModern(dgvHomeroomGradebook);
            StyleDataGridViewModern(dgvQuyLop);

            // Cài đặt riêng
            dgvHomeroomGradebook.ReadOnly = true;
            dgvHomeroomGradebook.CellFormatting += dgvHomeroomGradebook_CellFormatting;

            lblNoGradebookData = new Label
            {
                Name = "lblNoGradebookData",
                Text = "Chưa có thông tin điểm để hiển thị.",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.Gray,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            // Gán sự kiện DataBindingComplete
            dgvDiemDanh.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvHocSinh.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvKi1.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvKi2.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvHomeroomGradebook.DataBindingComplete += DataGridView_DataBindingComplete;
            dgvQuyLop.DataBindingComplete += DataGridView_DataBindingComplete;
        }

        #endregion

        #region UI States & Navigation (Trạng thái UI & Điều hướng)

        /// <summary>
        /// Hiển thị màn hình chọn lớp và ẩn màn hình quản lý chi tiết.
        /// </summary>
        private void ShowLopChonUI()
        {
            StopQrCamera(); // Đảm bảo camera tắt khi quay lại
            isHomeroomView = false;
            panelLopChon.Visible = true;
            panelLopChon.BringToFront();
            panelMainView.Visible = false;

            PopulateClassSelectionScreen();
        }

        /// <summary>
        /// Tải danh sách các lớp (chủ nhiệm và giảng dạy) vào FlowLayoutPanel.
        /// </summary>
        private void PopulateClassSelectionScreen()
        {
            // Dọn dẹp các control cũ và gỡ sự kiện
            foreach (Button btn in flowLayoutPanelLop.Controls.OfType<Button>().ToList())
            {
                btn.Click -= HomeroomButton_Click;
                btn.Click -= LopButton_Click;
                btn.Dispose();
            }
            flowLayoutPanelLop.Controls.Clear();

            // CHUẨN HÓA: Đã xóa '_'
            if (string.IsNullOrEmpty(maGV))
            {
                lblChonLopTitle.Text = "Tài khoản chưa được phân công.";
                return;
            }

            // Tải lớp chủ nhiệm
            // CHUẨN HÓA: Đã xóa '_'
            DataTable dtHomeroom = DatabaseHelper.GetHomeroomClassesByTeacher(maGV);
            if (dtHomeroom.Rows.Count > 0)
            {
                homeroomClassInfo = new KeyValuePair<string, string>(dtHomeroom.Rows[0]["MaLop"].ToString(), dtHomeroom.Rows[0]["TenLop"].ToString());
                int availableWidth = flowLayoutPanelLop.ClientSize.Width - flowLayoutPanelLop.Padding.Left - flowLayoutPanelLop.Padding.Right;

                var btnHomeroom = new Button
                {
                    Text = "⭐ Lớp Chủ Nhiệm: " + dtHomeroom.Rows[0]["TenLop"].ToString(),
                    Tag = "HOMEROOM",
                    Size = new Size(availableWidth - 20, 80),
                    Margin = new Padding(10, 10, 10, 20),
                    Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                    BackColor = Color.FromArgb(255, 184, 77),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                btnHomeroom.FlatAppearance.BorderSize = 0;
                btnHomeroom.Click += HomeroomButton_Click;
                flowLayoutPanelLop.Controls.Add(btnHomeroom);
                flowLayoutPanelLop.SetFlowBreak(btnHomeroom, true);
            }

            // Tải các lớp giảng dạy
            // CHUẨN HÓA: Đã xóa '_'
            DataTable dsLop = DatabaseHelper.GetClassesByTeacher(maGV);
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

        /// <summary>
        /// Xử lý responsive cho nút "Lớp Chủ Nhiệm" khi thay đổi kích thước.
        /// </summary>
        private void FlowLayoutPanelLop_Resize(object sender, EventArgs e)
        {
            if (flowLayoutPanelLop == null || !flowLayoutPanelLop.IsHandleCreated)
                return;

            int availableWidth = flowLayoutPanelLop.ClientSize.Width - flowLayoutPanelLop.Padding.Left - flowLayoutPanelLop.Padding.Right;

            foreach (Control ctrl in flowLayoutPanelLop.Controls)
            {
                if (ctrl is Button btn && btn.Tag != null && btn.Tag.ToString() == "HOMEROOM")
                {
                    btn.Width = availableWidth - btn.Margin.Left - btn.Margin.Right;
                }
            }
        }

        /// <summary>
        /// Kích hoạt giao diện quản lý chi tiết (ẩn màn hình chọn lớp).
        /// </summary>
        private void ActivateMainView(string title)
        {
            panelMainView.Visible = true;
            panelMainView.BringToFront();
            panelLopChon.Visible = false;
            lblTenLopHienTai.Text = title;
        }

        /// <summary>
        /// Sự kiện click cho các nút lớp Giảng Dạy.
        /// </summary>
        private void LopButton_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            maLop = btn.Tag.ToString(); // CHUẨN HÓA: Đã xóa '_'
            isHomeroomView = false;

            ActivateMainView($"Giảng dạy: {btn.Text}");

            panelTabs.Visible = true;
            panelContent.Controls.Clear();

            // Kích hoạt tab Điểm danh làm mặc định
            if (!rbDiemDanh.Checked)
            {
                rbDiemDanh.Checked = true;
            }
            else
            {
                TabButton_CheckedChanged(rbDiemDanh, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sự kiện click cho nút lớp Chủ Nhiệm.
        /// </summary>
        private void HomeroomButton_Click(object sender, EventArgs e)
        {
            maLop = homeroomClassInfo.Key; // CHUẨN HÓA: Đã xóa '_'
            isHomeroomView = true;

            ActivateMainView($"Chủ nhiệm: {homeroomClassInfo.Value}");

            panelTabs.Visible = false; // Ẩn các tab (Điểm danh, Kết quả...)
            ShowHomeroomView();
        }

        /// <summary>
        /// Quay lại màn hình chọn lớp.
        /// </summary>
        private void btnQuayLaiChonLop_Click(object sender, EventArgs e)
        {
            if (manualSessionActive)
            {
                var r = MessageBox.Show(
                    "Bạn chưa lưu điểm danh. Nếu thoát, dữ liệu sẽ bị mất.\nBạn có chắc muốn thoát?",
                    "Chưa lưu",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (r == DialogResult.No)
                    return;

                manualSessionActive = false;
            }

            ShowLopChonUI();
        }

        /// <summary>
        /// Sự kiện khi thay đổi tab (Điểm danh, QR, Kết quả, Học sinh).
        /// </summary>
        private void TabButton_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb == null || !rb.Checked || isHomeroomView) return;

            // Nếu đang điểm danh mà chưa lưu → cảnh báo
            if (manualSessionActive)
            {
                var r = MessageBox.Show(
                    "Phiên điểm danh chưa được lưu. Bạn có chắc muốn rời khỏi?",
                    "Chưa lưu dữ liệu",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (r == DialogResult.No)
                {
                    rb.Checked = false; // Hủy chuyển tab
                    return;
                }

                manualSessionActive = false;
            }

            UpdateTabStyles();

            if (rb == rbDiemDanh) ShowDiemDanh(false);
            else if (rb == rbQR) ShowDiemDanh(true);
            else if (rb == rbKetQua) ShowKetQua();
            else if (rb == rbHocSinh) ShowHocSinh();
        }

        /// <summary>
        /// Cập nhật style (màu sắc) cho các nút tab.
        /// </summary>
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

        #region UI styling & Formatting (Định dạng và style)

        /// <summary>
        /// Áp dụng style hiện đại cho DataGridView.
        /// </summary>
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

        /// <summary>
        /// Tùy chỉnh tiêu đề và định dạng cột cho DataGridView sau khi tải dữ liệu.
        /// </summary>
        private void SetGridColumnHeaders(DataGridView dgv)
        {
            if (dgv == null || dgv.Columns.Count == 0) return;
            try
            {
                if (dgv.Columns.Contains("STT"))
                {
                    var col = dgv.Columns["STT"];
                    col.HeaderText = "STT";
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    col.DisplayIndex = 0;
                    col.ReadOnly = true;
                }

                if (dgv.Columns.Contains("MaHS")) dgv.Columns["MaHS"].Visible = false;
                if (dgv.Columns.Contains("HoTen"))
                {
                    var col = dgv.Columns["HoTen"];
                    col.HeaderText = "Họ và Tên";
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    if (dgv.Columns.Contains("STT"))
                        col.DisplayIndex = 1;
                }

                if (dgv.Columns.Contains("GioiTinh")) dgv.Columns["GioiTinh"].HeaderText = "Giới Tính";
                if (dgv.Columns.Contains("NgaySinh")) { dgv.Columns["NgaySinh"].HeaderText = "Ngày Sinh"; dgv.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy"; }
                if (dgv.Columns.Contains("DiaChi")) dgv.Columns["DiaChi"].HeaderText = "Địa Chỉ";

                if (dgv.Columns.Contains("TenLop"))
                {
                    var col = dgv.Columns["TenLop"];
                    col.HeaderText = "Tên Lớp";
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

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
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi SetGridColumnHeaders");
            }
        }

        /// <summary>
        /// Sự kiện kích hoạt sau khi DataGridView tải xong dữ liệu để tùy chỉnh cột.
        /// </summary>
        private void DataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            SetGridColumnHeaders(dgv);
        }

        /// <summary>
        /// Tô màu điểm số trong sổ điểm chung của GVCN.
        /// </summary>
        private void dgvHomeroomGradebook_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var dgv = sender as DataGridView;
            // Bỏ qua cột tên và mã
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

        #region Homeroom Teacher View (Chế độ GV Chủ nhiệm)

        /// <summary>
        /// Hiển thị giao diện chính của GVCN (Sổ điểm chung và Quỹ lớp).
        /// </summary>
        private void ShowHomeroomView()
        {
            panelContent.Controls.Clear(); // Dọn dẹp control cũ

            TabControl tabHomeroom = new TabControl { Name = "tabHomeroom", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
            TabPage tabGradebook = new TabPage("Sổ Điểm Chung");
            TabPage tabFund = new TabPage("Quản Lý Quỹ Lớp");

            // Xây dựng nội dung cho từng tab
            BuildHomeroomGradebookTab(tabGradebook);
            BuildQuyLopTab(tabFund);

            tabHomeroom.TabPages.Add(tabGradebook);
            tabHomeroom.TabPages.Add(tabFund);

            panelContent.Controls.Add(tabHomeroom);
        }

        /// <summary>
        /// Xây dựng nội dung cho tab "Sổ Điểm Chung".
        /// </summary>
        private void BuildHomeroomGradebookTab(TabPage tab)
        {
            tab.Controls.Clear();
            tab.BackColor = Color.White;

            Panel filterPanel = new Panel { Name = "filterPanel", Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };
            Label lblFilter = new Label { Text = "Xem điểm:", Dock = DockStyle.Left, AutoSize = true, Padding = new Padding(0, 5, 0, 0), Font = new Font("Segoe UI", 10F) };

            // Khởi tạo ComboBox chọn loại điểm (biến class)
            cbLoaiDiem = new ComboBox { Name = "cbLoaiDiem", Dock = DockStyle.Left, DropDownStyle = ComboBoxStyle.DropDownList, Width = 150, Font = new Font("Segoe UI", 10F) };
            cbLoaiDiem.Items.AddRange(new object[] { "GiuaKi1", "CuoiKi1", "GiuaKi2", "CuoiKi2" });
            cbLoaiDiem.SelectedIndex = 0;
            cbLoaiDiem.SelectedIndexChanged += LoadHomeroomGradebookData; // Gán sự kiện

            filterPanel.Controls.Add(cbLoaiDiem);
            filterPanel.Controls.Add(lblFilter);

            dgvHomeroomGradebook.Dock = DockStyle.Fill;

            tab.Controls.Add(lblNoGradebookData); // Label thông báo rỗng
            tab.Controls.Add(dgvHomeroomGradebook);
            tab.Controls.Add(filterPanel);

            LoadHomeroomGradebookData(); // Tải dữ liệu lần đầu
        }

        /// <summary>
        /// Tải dữ liệu sổ điểm chung dựa trên lựa chọn của ComboBox.
        /// </summary>
        private void LoadHomeroomGradebookData(object sender = null, EventArgs e = null) // Thêm tham số mặc định
        {
            if (cbLoaiDiem == null || cbLoaiDiem.SelectedItem == null) return;
            string loaiDiem = cbLoaiDiem.SelectedItem.ToString();

            // CHUẨN HÓA: Đã xóa '_'
            DataTable dt = DatabaseHelper.GetHomeroomGradebook(maLop, loaiDiem);

            // Kiểm tra nếu không có dữ liệu (chỉ có cột MaHS, HoTen)
            if (dt == null || dt.Columns.Count <= 2)
            {
                dgvHomeroomGradebook.Visible = false;
                dgvHomeroomGradebook.DataSource = null;
                lblNoGradebookData.Visible = true;
                lblNoGradebookData.BringToFront();
            }
            else
            {
                dgvHomeroomGradebook.Visible = true;
                lblNoGradebookData.Visible = false;
                dgvHomeroomGradebook.DataSource = dt;
                dgvHomeroomGradebook.BringToFront();
            }
        }

        #endregion

        #region Quỹ Lớp (Homeroom Fund)

        /// <summary>
        /// Tạo một thẻ thống kê (Tổng Thu, Tổng Chi, Tồn Quỹ).
        /// </summary>
        private Panel CreateStatCard(string title, Color valueColor, out Label valueLabel)
        {
            Panel card = new Panel
            {
                Size = new Size(180, 70),
                BackColor = Color.White,
                Margin = new Padding(5),
                Padding = new Padding(10)
            };
            // Thêm sự kiện vẽ viền cho card
            card.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid);
            };

            Label lblTitle = new Label { Text = title, Dock = DockStyle.Top, Font = new Font("Segoe UI Semibold", 9F), ForeColor = Color.Gray, TextAlign = ContentAlignment.MiddleLeft };
            valueLabel = new Label { Text = "0", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = valueColor, TextAlign = ContentAlignment.MiddleLeft };

            card.Controls.Add(valueLabel);
            card.Controls.Add(lblTitle);
            return card;
        }

        /// <summary>
        /// Xây dựng nội dung cho tab "Quản Lý Quỹ Lớp".
        /// </summary>
        private void BuildQuyLopTab(TabPage tab)
        {
            tab.Controls.Clear();
            tab.BackColor = Color.White;
            var mainPanel = new Panel { Dock = DockStyle.Fill };
            var dashboardPanel = new Panel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(10), BackColor = Color.White };

            btnThemKhoanQuy = new Button
            {
                Name = "btnThemKhoanQuy",
                Text = "Tạo mới Thu/Chi",
                Dock = DockStyle.Left,
                Size = new Size(160, 70),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnThemKhoanQuy.FlatAppearance.BorderSize = 0;
            btnThemKhoanQuy.Click += BtnThemKhoanQuy_Click;

            var statsPanel = new TableLayoutPanel { Dock = DockStyle.Right, ColumnCount = 3, RowCount = 1, AutoSize = true, BackColor = Color.White };

            // Khởi tạo các label thống kê
            Panel thuCard = CreateStatCard("TỔNG THU", Color.FromArgb(0, 150, 64), out lblTongThu_Value);
            Panel chiCard = CreateStatCard("TỔNG CHI", Color.FromArgb(210, 43, 43), out lblTongChi_Value);
            Panel tonCard = CreateStatCard("TỒN QUỸ", Color.FromArgb(0, 80, 155), out lblTonQuy_Value);

            statsPanel.Controls.Add(thuCard, 0, 0);
            statsPanel.Controls.Add(chiCard, 1, 0);
            statsPanel.Controls.Add(tonCard, 2, 0);
            dashboardPanel.Controls.Add(statsPanel);
            dashboardPanel.Controls.Add(btnThemKhoanQuy);

            dgvQuyLop.Dock = DockStyle.Fill;
            dgvQuyLop.ReadOnly = true;
            dgvQuyLop.CellClick += DgvQuyLop_CellClick;

            mainPanel.Controls.Add(dgvQuyLop);
            mainPanel.Controls.Add(dashboardPanel);
            tab.Controls.Add(mainPanel);

            LoadQuyLopData();
        }

        /// <summary>
        /// Hiển thị Form (Dialog) để thêm một khoản thu/chi mới.
        /// </summary>
        private void BtnThemKhoanQuy_Click(object sender, EventArgs e)
        {
            // Sử dụng Form động để nhập liệu
            using (var form = new Form())
            {
                // ... (Logic tạo Form động) ...
                #region Dynamic Form Creation
                form.Text = "Tạo Mới Khoản Thu/Chi";
                form.Size = new Size(520, 390);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.BackColor = Color.White;
                form.Font = new Font("Segoe UI", 10F);

                var tlp = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(25),
                    ColumnCount = 2,
                    RowCount = 5,
                    BackColor = Color.White
                };
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
                tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

                Func<string, Label> createLabel = (text) => new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Font = new Font("Segoe UI Semibold", 10F), ForeColor = Color.FromArgb(64, 64, 64), Margin = new Padding(0, 0, 10, 0) };
                Action<Control> styleControl = (ctrl) => { ctrl.Dock = DockStyle.Fill; ctrl.Font = new Font("Segoe UI", 10F); ctrl.Margin = new Padding(3, 10, 3, 10); if (ctrl is TextBox) ((TextBox)ctrl).BorderStyle = BorderStyle.FixedSingle; else if (ctrl is ComboBox) ((ComboBox)ctrl).FlatStyle = FlatStyle.System; else if (ctrl is DateTimePicker) ((DateTimePicker)ctrl).Format = DateTimePickerFormat.Short; else if (ctrl is NumericUpDown) ((NumericUpDown)ctrl).BorderStyle = BorderStyle.FixedSingle; };

                var txtGhiChu = new TextBox();
                var cbLoai = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                cbLoai.Items.AddRange(new string[] { "Thu", "Chi" });
                cbLoai.SelectedIndex = 0;
                var dtpNgay = new DateTimePicker();
                var numSoTien = new NumericUpDown { Maximum = 1000000000, ThousandsSeparator = true, Increment = 1000 };

                styleControl(txtGhiChu); styleControl(cbLoai); styleControl(dtpNgay); styleControl(numSoTien);
                tlp.Controls.Add(createLabel("Tên khoản / Diễn giải:"), 0, 0); tlp.Controls.Add(txtGhiChu, 1, 0);
                tlp.Controls.Add(createLabel("Loại giao dịch:"), 0, 1); tlp.Controls.Add(cbLoai, 1, 1);
                tlp.Controls.Add(createLabel("Ngày thực hiện:"), 0, 2); tlp.Controls.Add(dtpNgay, 1, 2);
                tlp.Controls.Add(createLabel("Số tiền (VNĐ):"), 0, 3); tlp.Controls.Add(numSoTien, 1, 3);

                var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 65, BackColor = Color.FromArgb(245, 245, 245), Padding = new Padding(0, 0, 25, 0) };
                var btnLuu = new Button { Text = "Lưu", DialogResult = DialogResult.OK, Size = new Size(100, 38), BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 10F), Dock = DockStyle.Right };
                btnLuu.FlatAppearance.BorderSize = 0;
                var spacer = new Panel { Width = 10, Dock = DockStyle.Right, BackColor = Color.Transparent };
                var btnHuy = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel, Size = new Size(90, 38), BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F), Dock = DockStyle.Right };
                btnHuy.FlatAppearance.BorderSize = 1; btnHuy.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                btnLuu.Margin = new Padding(0, (buttonPanel.Height - btnLuu.Height) / 2, 0, 0);
                btnHuy.Margin = new Padding(0, (buttonPanel.Height - btnHuy.Height) / 2, 0, 0);

                buttonPanel.Controls.Add(btnLuu); buttonPanel.Controls.Add(spacer); buttonPanel.Controls.Add(btnHuy);
                form.Controls.Add(tlp); form.Controls.Add(buttonPanel);
                form.AcceptButton = btnLuu; form.CancelButton = btnHuy;
                #endregion

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    // Validation
                    if (string.IsNullOrWhiteSpace(txtGhiChu.Text)) { MessageBox.Show("Tên khoản/Diễn giải không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    if (numSoTien.Value <= 0) { MessageBox.Show("Số tiền phải lớn hơn 0.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    try
                    {
                        // Gọi DatabaseHelper
                        // CHUẨN HÓA: Đã xóa '_'
                        DatabaseHelper.InsertClassFundEntry(maLop, cbLoai.SelectedItem.ToString(), numSoTien.Value, dtpNgay.Value, txtGhiChu.Text);
                        LoadQuyLopData(); // Tải lại dữ liệu
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Tải và tính toán dữ liệu quỹ lớp, hiển thị lên DataGridView và các thẻ thống kê.
        /// </summary>
        private void LoadQuyLopData()
        {
            if (dgvQuyLop == null) return;
            try
            {
                // CHUẨN HÓA: Đã xóa '_'
                DataTable dt = DatabaseHelper.GetClassFundByClass(maLop);

                // Thêm các cột tính toán (không có trong CSDL)
                dt.Columns.Add("Thu", typeof(decimal));
                dt.Columns.Add("Chi", typeof(decimal));
                dt.Columns.Add("Tồn", typeof(decimal));

                decimal tongThu = 0;
                decimal tongChi = 0;
                decimal ton = 0;

                // Tính toán số dư
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

                // Thêm cột Xóa
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

                // Cập nhật các thẻ thống kê
                if (lblTongThu_Value != null) lblTongThu_Value.Text = $"{tongThu:N0}đ";
                if (lblTongChi_Value != null) lblTongChi_Value.Text = $"{tongChi:N0}đ";
                if (lblTonQuy_Value != null) lblTonQuy_Value.Text = $"{ton:N0}đ";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu quỹ lớp");
            }
        }

        /// <summary>
        /// Xử lý click nút "Xóa" trên lưới quỹ lớp.
        /// </summary>
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
                        DatabaseHelper.DeleteClassFundEntry(maQL);
                        LoadQuyLopData(); // Tải lại
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(" Đã có lỗi hệ thống xảy ra , hãy thử lại sau ");
                    }
                }
            }
        }

        /// <summary>
        /// Định dạng các cột cho lưới quỹ lớp (ẩn cột, định dạng tiền tệ).
        /// </summary>
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi FormatQuyLopGrid: {ex.Message}");
            }
        }


        #endregion

        #region Regular Teacher Views (Giảng dạy: Điểm danh, Kết quả, Học sinh)

        /// <summary>
        /// Tải cache trạng thái khóa điểm từ CSDL.
        /// </summary>
        private void LoadLockStatusCache()
        {
            lockStatusCache = new Dictionary<string, bool>(); // CHUẨN HÓA: Đã xóa '_'
            try
            {
                DataTable dt = DatabaseHelper.GetScoreDeadlines();
                foreach (DataRow row in dt.Rows)
                {
                    string maCotDiem = row["MaCotDiem"].ToString();
                    bool daKhoa = Convert.ToBoolean(row["DaKhoa"]);
                    // CHUẨN HÓA: Đã xóa '_'
                    if (!lockStatusCache.ContainsKey(maCotDiem))
                    {
                        lockStatusCache.Add(maCotDiem, daKhoa);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải trạng thái khóa điểm");
            }
        }

        /// <summary>
        /// Xác thực dữ liệu nhập vào (chỉ cho phép số 0-10) trên lưới kết quả.
        /// </summary>
        private void ResultGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null || dgv.Rows[e.RowIndex].IsNewRow) return;

            string colName = dgv.Columns[e.ColumnIndex].DataPropertyName;
            var gradeColumns = new List<string> { "Thang1", "Thang2", "Thang3", "GiuaKi", "CuoiKi" };

            if (gradeColumns.Contains(colName, StringComparer.OrdinalIgnoreCase))
            {
                string val = e.FormattedValue?.ToString();
                if (string.IsNullOrWhiteSpace(val)) // 1. Cho phép ô trống
                {
                    dgv.Rows[e.RowIndex].ErrorText = null;
                    return;
                }
                if (!float.TryParse(val, out float diem)) // 2. Kiểm tra là số
                {
                    MessageBox.Show("Điểm phải là một con số (ví dụ: 8.5) và không được chứa chữ.", "Lỗi Nhập Điểm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    dgv.Rows[e.RowIndex].ErrorText = "Điểm phải là số.";
                }
                else if (diem < 0 || diem > 10) // 3. Kiểm tra khoảng 0-10
                {
                    MessageBox.Show("Điểm phải nằm trong khoảng từ 0 đến 10.", "Lỗi Nhập Điểm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    dgv.Rows[e.RowIndex].ErrorText = "Điểm phải từ 0 đến 10.";
                }
                else
                {
                    dgv.Rows[e.RowIndex].ErrorText = null; // Hợp lệ
                }
            }
        }

        /// <summary>
        /// Hiển thị giao diện Điểm danh (Tab Thủ công và Tab QR).
        /// </summary>
        private void ShowDiemDanh(bool selectQRTab = false)
        {
            SaveAllCurrentEdits(); // Lưu các thay đổi ở tab cũ
            StopQrCamera(); // Tắt camera (nếu đang chạy)
            panelContent.Controls.Clear(); // Dọn dẹp
            // CHUẨN HÓA: Đã xóa '_'
            if (string.IsNullOrEmpty(maLop)) return;

            // CHUẨN HÓA: Đã xóa '_'
            tabDiemDanh = new TabControl { Dock = DockStyle.Fill, Appearance = TabAppearance.FlatButtons, ItemSize = new Size(0, 1), SizeMode = TabSizeMode.Fixed, Name = "_tabDiemDanh" };
            tabThuCong = new TabPage("Điểm danh thủ công");
            tabQR = new TabPage("Điểm danh QR");

            BuildThuCongTab(); // Xây dựng tab thủ công
            BuildQRTab(); // Xây dựng tab QR

            // CHUẨN HÓA: Đã xóa '_'
            tabDiemDanh.TabPages.Add(tabThuCong);
            tabDiemDanh.TabPages.Add(tabQR);
            panelContent.Controls.Add(tabDiemDanh);
            tabDiemDanh.SelectedTab = selectQRTab ? tabQR : tabThuCong;
        }

        /// <summary>
        /// Xây dựng nội dung cho tab "Điểm danh thủ công".
        /// </summary>
        private void BuildThuCongTab()
        {
            var container = new Panel { Dock = DockStyle.Fill, Name = "containerThuCong" };
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(5), FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Name = "topThuCong" };

            // Khởi tạo các control (biến class)
            dtpNgayTC = new DateTimePicker { Name = "dtpNgayTC", Format = DateTimePickerFormat.Short, Value = DateTime.Today, Width = 110, Font = new Font("Segoe UI", 9F) };
            cbBuoiTC = new ComboBox { Name = "cbBuoiTC", DropDownStyle = ComboBoxStyle.DropDownList, Width = 80, Font = new Font("Segoe UI", 9F) };
            cbBuoiTC.Items.AddRange(new[] { "Sáng", "Chiều" });
            cbBuoiTC.SelectedIndex = DateTime.Now.Hour < 12 ? 0 : 1;
            dtpThoiGianTC = new DateTimePicker { Name = "dtpThoiGianTC", Format = DateTimePickerFormat.Custom, CustomFormat = "HH:mm", ShowUpDown = true, Value = DateTime.Now, Width = 70, Font = new Font("Segoe UI", 9F) };

            dtpNgayTC.ValueChanged += AttendanceFilter_Changed;
            cbBuoiTC.SelectedIndexChanged += AttendanceFilter_Changed;

            var btnXemTC = new Button { Text = "Xem", Width = 70, Height = 28, BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Name = "btnXemTC" };
            btnXemTC.Click += (s, e) => LoadThuCong(readOnly: dtpNgayTC.Value.Date < DateTime.Today, createIfEmpty: false);
            btnBatDauTC = new Button { Text = "Bắt đầu", Width = 80, Height = 28, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Name = "btnBatDauTC" };
            btnBatDauTC.Click += BtnBatDauTC_Click;
            btnLuuTC = new Button { Text = "Lưu", Width = 70, Height = 28, BackColor = Color.OrangeRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false, Name = "btnLuuTC" };
            btnLuuTC.Click += BtnLuuTC_Click;
            lblThongKeTC = new Label { Name = "lblThongKeTC", AutoSize = true, ForeColor = Color.Maroon, Padding = new Padding(15, 5, 0, 0), Font = new Font("Segoe UI", 11F, FontStyle.Bold) };

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
            // CHUẨN HÓA: Đã xóa '_'
            tabThuCong.Controls.Clear();
            tabThuCong.Controls.Add(container);
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
            manualSessionActive = true; // CHUẨN HÓA: Đã xóa '_'
            btnLuuTC.Enabled = true;
            DateTime thoiDiemDiemDanh = dtpNgayTC.Value.Date + dtpThoiGianTC.Value.TimeOfDay;
            // CHUẨN HÓA: Đã xóa '_'
            DatabaseHelper.ExecuteCreateDefaultAttendance(maLop, thoiDiemDiemDanh, cbBuoiTC.SelectedItem?.ToString());
            LoadThuCong(readOnly: false, createIfEmpty: false);
        }

        /// <summary>
        /// Tải dữ liệu điểm danh thủ công lên DataGridView.
        /// </summary>
        private void LoadThuCong(bool readOnly, bool createIfEmpty)
        {
            try
            {
                DateTime ngay = dtpNgayTC.Value.Date;
                string buoi = cbBuoiTC.SelectedItem?.ToString();
                // CHUẨN HÓA: Đã xóa '_'
                DataTable dt = DatabaseHelper.GetAttendanceByClassAndDate(maLop, ngay, buoi);

                // CHUẨN HÓA: Đã xóa '_'
                if (createIfEmpty && dt.Rows.Count == 0 && ngay >= DateTime.Today && manualSessionActive && !readOnly)
                {
                    DateTime thoiDiemDiemDanh = dtpNgayTC.Value.Date + dtpThoiGianTC.Value.TimeOfDay;
                    // CHUẨN HÓA: Đã xóa '_'
                    DatabaseHelper.ExecuteCreateDefaultAttendance(maLop, thoiDiemDiemDanh, buoi);
                    // CHUẨN HÓA: Đã xóa '_'
                    dt = DatabaseHelper.GetAttendanceByClassAndDate(maLop, ngay, buoi);
                }

                foreach (DataRow r in dt.Rows)
                {
                    bool missing = r["TrangThai"] == DBNull.Value || string.IsNullOrWhiteSpace(r["TrangThai"].ToString());
                    if (missing)
                    {
                        // CHUẨN HÓA: Đã xóa '_'
                        if (manualSessionActive && !readOnly)
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

                // Thay thế cột 'TrangThai' bằng ComboBoxColumn
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

                // CHUẨN HÓA: Đã xóa '_'
                dgvDiemDanh.ReadOnly = readOnly || !manualSessionActive;
                if (!dgvDiemDanh.ReadOnly && dgvDiemDanh.Columns.Contains("TrangThai"))
                {
                    foreach (DataGridViewColumn c in dgvDiemDanh.Columns)
                        c.ReadOnly = (c.Name != "TrangThai");
                }

                // Gán/Gỡ sự kiện
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
                MessageBox.Show("Lỗi tải điểm danh: ");
            }
        }

        private void BtnLuuTC_Click(object sender, EventArgs e)
        {
            if (!manualSessionActive)
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

                    if (string.Equals(tt, "Vắng mặt", StringComparison.OrdinalIgnoreCase))
                        tt = "Vắng";

                    if (string.IsNullOrWhiteSpace(tt))
                        tt = "Có mặt";

                    DatabaseHelper.UpsertAttendance(maHS, maLop, thoiDiemLuu, cbBuoiTC.SelectedItem?.ToString(), tt);
                    saved++;
                }

                MessageBox.Show($"Đã lưu {saved} bản ghi.");

                manualSessionActive = false;  // Reset trạng thái phiên điểm danh
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
        private void DgvDiemDanh_DataError(object sender, DataGridViewDataErrorEventArgs e) { e.ThrowException = false; } // Ngăn crash khi chọn item

        /// <summary>
        /// Cập nhật thống kê Sỉ số / Hiện diện / Vắng.
        /// </summary>
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

        /// <summary>
        /// Xây dựng nội dung cho tab "Điểm danh QR".
        /// </summary>
        private void BuildQRTab()
        {
            var container = new Panel { Dock = DockStyle.Fill, Name = "containerQR" };
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(5), FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Name = "topQR" };

            // Khởi tạo các control (biến class)
            dtpNgayQR = new DateTimePicker { Name = "dtpNgayQR", Format = DateTimePickerFormat.Short, Value = DateTime.Today, Width = 100 };
            cbBuoiQR = new ComboBox { Name = "cbBuoiQR", DropDownStyle = ComboBoxStyle.DropDownList, Width = 80 };
            cbBuoiQR.Items.AddRange(new[] { "Sáng", "Chiều" });
            cbBuoiQR.SelectedIndex = DateTime.Now.Hour < 12 ? 0 : 1;
            btnStartQRScan = new Button { Name = "btnStartQRScan", Text = "Quét", Width = 70, Height = 28, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnStopQRScan = new Button { Name = "btnStopQRScan", Text = "Dừng", Width = 70, Height = 28, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            lblQRStatus = new Label { Name = "lblQRStatus", AutoSize = true, ForeColor = Color.Navy, Padding = new Padding(15, 5, 0, 0), Text = "Chưa quét" };

            btnStartQRScan.Click += BtnStartQRScan_Click;
            btnStopQRScan.Click += BtnStopQRScan_Click;

            top.Controls.Add(new Label { Text = "Ngày:", AutoSize = true, Padding = new Padding(0, 5, 5, 0) });
            top.Controls.Add(dtpNgayQR);
            top.Controls.Add(new Label { Text = "Buổi:", AutoSize = true, Padding = new Padding(10, 5, 5, 0) });
            top.Controls.Add(cbBuoiQR);
            top.Controls.Add(btnStartQRScan);
            top.Controls.Add(btnStopQRScan);
            top.Controls.Add(lblQRStatus);

            picQR = new PictureBox { Name = "picQR", Dock = DockStyle.Fill, BackColor = Color.Black, SizeMode = PictureBoxSizeMode.Zoom };

            container.Controls.Add(picQR);
            container.Controls.Add(top);
            // CHUẨN HÓA: Đã xóa '_'
            tabQR.Controls.Clear();
            tabQR.Controls.Add(container);
        }

        private void BtnStopQRScan_Click(object sender, EventArgs e)
        {
            StopQrCamera();
            lblQRStatus.Text = "Đã dừng";
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
                qrScanning = true; // CHUẨN HÓA: Đã xóa '_'
                lblQRStatus.Text = "Đang quét...";
            }
            catch (Exception ex) { MessageBox.Show("Lỗi camera: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý mỗi khung hình (frame) mới từ camera.
        /// </summary>
        private void Video_NewFrame(object sender, NewFrameEventArgs e)
        {
            try
            {
                Bitmap frame = (Bitmap)e.Frame.Clone();
                // CHUẨN HÓA: Đã xóa '_'
                lock (frameLock)
                {
                    latestFrame?.Dispose();
                    latestFrame = (Bitmap)frame.Clone();
                }
                if (picQR != null && !picQR.IsDisposed)
                {
                    // Hiển thị frame lên PictureBox
                    if (picQR.InvokeRequired)
                        picQR.Invoke(new Action(() => { picQR.Image?.Dispose(); picQR.Image = (Bitmap)frame.Clone(); }));
                    else { picQR.Image?.Dispose(); picQR.Image = (Bitmap)frame.Clone(); }
                }
                frame.Dispose();
            }
            catch (Exception ex) { Debug.WriteLine($"Lỗi Video_NewFrame: {ex.Message}"); }
        }

        /// <summary>
        /// Bộ đếm thời gian (Timer) để giải mã QR từ khung hình gần nhất.
        /// </summary>
        private void QrTimer_Tick(object sender, EventArgs e)
        {
            // CHUẨN HÓA: Đã xóa '_'
            if (!qrScanning) return;
            Bitmap bmp = null;
            try
            {
                // CHUẨN HÓA: Đã xóa '_'
                lock (frameLock)
                {
                    if (latestFrame != null)
                        bmp = (Bitmap)latestFrame.Clone();
                }
                if (bmp == null) return;

                var reader = new BarcodeReader();
                var result = reader.Decode(bmp);

                if (result != null)
                {
                    string maHS = result.Text?.Trim();
                    if (!string.IsNullOrEmpty(maHS))
                    {
                        // Lưu điểm danh
                        DatabaseHelper.SaveAttendance(maHS, "Có mặt", dtpNgayQR.Value.Date, cbBuoiQR.SelectedItem?.ToString());
                        if (lblQRStatus != null && !lblQRStatus.IsDisposed)
                            lblQRStatus.Invoke(new Action(() => lblQRStatus.Text = $"✅ {maHS} - {DateTime.Now:T}"));
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Lỗi QrTimer_Tick: {ex.Message}"); }
            finally { bmp?.Dispose(); }
        }

        /// <summary>
        /// Dừng và giải phóng tài nguyên camera, timer.
        /// </summary>
        private void StopQrCamera()
        {
            try
            {
                qrScanning = false; // CHUẨN HÓA: Đã xóa '_'
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
            catch (Exception ex) { Debug.WriteLine($"Lỗi StopQrCamera: {ex.Message}"); }
        }

        /// <summary>
        /// Hiển thị giao diện "Kết Quả" (Sổ điểm chi tiết theo môn).
        /// </summary>
        private void ShowKetQua()
        {
            SaveAllCurrentEdits();
            StopQrCamera();
            panelContent.Controls.Clear();
            // CHUẨN HÓA: Đã xóa '_'
            if (string.IsNullOrEmpty(maLop)) { MessageBox.Show("Vui lòng chọn một lớp trước."); return; }

            LoadLockStatusCache(); // Tải cache khóa điểm

            // CHUẨN HÓA: Đã xóa '_'
            DataTable dtMonHoc = DatabaseHelper.GetSubjectsByTeacherAndClass(maGV, maLop);
            if (dtMonHoc == null || dtMonHoc.Rows.Count == 0)
            {
                MessageBox.Show("Bạn không được phân công giảng dạy môn nào tại lớp này.");
                return;
            }

            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10), BackColor = Color.WhiteSmoke, Name = "topKetQua" };
            Label lblChonMon = new Label { Text = "Chọn môn học:", Dock = DockStyle.Left, AutoSize = true, Padding = new Padding(0, 5, 0, 0), Font = new Font("Segoe UI", 10F) };

            cbMonHocChon = new ComboBox { Name = "cbMonHocChon", Dock = DockStyle.Left, DropDownStyle = ComboBoxStyle.DropDownList, Width = 200, Font = new Font("Segoe UI", 10F) };
            cbMonHocChon.DataSource = dtMonHoc;
            cbMonHocChon.DisplayMember = "TenMon";
            cbMonHocChon.ValueMember = "MaMon";
            cbMonHocChon.SelectedIndexChanged += CbMonHocChon_SelectedIndexChanged;

            topPanel.Controls.Add(cbMonHocChon);
            topPanel.Controls.Add(lblChonMon);

            TabControl tab = new TabControl { Dock = DockStyle.Fill, Name = "tabKetQua" };
            TabPage p1 = new TabPage("Học Kì 1");
            TabPage p2 = new TabPage("Học Kì 2");

            dgvKi1.DataSource = null; dgvKi1.Columns.Clear();
            dgvKi2.DataSource = null; dgvKi2.Columns.Clear();

            dgvKi1.DataError += DgvResult_DataError;
            dgvKi2.DataError += DgvResult_DataError;

            p1.Controls.Add(dgvKi1);
            p2.Controls.Add(dgvKi2);
            tab.TabPages.Add(p1);
            tab.TabPages.Add(p2);

            panelContent.Controls.Add(tab);
            panelContent.Controls.Add(topPanel);

            LoadKetQuaGrids(); // Tải dữ liệu
        }

        /// <summary>
        /// Tải dữ liệu điểm cho cả 2 học kỳ dựa trên môn học đã chọn.
        /// </summary>
        private void LoadKetQuaGrids()
        {
            if (cbMonHocChon == null || cbMonHocChon.SelectedValue == null) return;
            string selectedMaMon = cbMonHocChon.SelectedValue.ToString();
            if (string.IsNullOrEmpty(selectedMaMon)) return;

            // CHUẨN HÓA: Đã xóa '_'
            DataTable dt1 = DatabaseHelper.GetScoreboardPivot(maLop, 1, selectedMaMon);
            DataTable dt2 = DatabaseHelper.GetScoreboardPivot(maLop, 2, selectedMaMon);

            SetupResultGrid(dgvKi1, dt1, 1, selectedMaMon);
            SetupResultGrid(dgvKi2, dt2, 2, selectedMaMon);
        }

        private void CbMonHocChon_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadKetQuaGrids();
        }

        /// <summary>
        /// Kiểm tra data các cột.
        /// </summary>
        private void DgvResult_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;

            MessageBox.Show(
                "Điểm phải là số từ 0 đến 10 (ví dụ: 8.5).",
                "Sai định dạng",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        /// <summary>
        /// Cài đặt DataGridView hiển thị điểm (gán DataSource, sự kiện, khóa cột).
        /// </summary>
        private void SetupResultGrid(DataGridView dgv, DataTable dt, int ki, string maMon)
        {
            if (dgv == null) return;

            // Gỡ sự kiện cũ
            dgv.CellEndEdit -= ResultGrid_CellEndEdit;
            dgv.CellValidating -= ResultGrid_CellValidating;
            dgv.CurrentCellDirtyStateChanged -= ResultGrid_CurrentCellDirtyStateChanged;
            dgv.DataError -= DgvResult_DataError;

            dgv.Dock = DockStyle.Fill;
            dgv.DataSource = null;
            dgv.Columns.Clear();
            dgv.AutoGenerateColumns = true;
            dgv.DataSource = dt;

            if (dt != null && dt.Columns.Contains("HoTen") && dgv.Columns.Contains("HoTen"))
                dgv.Columns["HoTen"].ReadOnly = true;
            if (dt != null && dt.Columns.Contains("MaHS") && dgv.Columns.Contains("MaHS"))
                dgv.Columns["MaHS"].Visible = false;

            ApplyColumnLocks(dgv, ki);

            dgv.Tag = Tuple.Create(ki, maMon);

            // Gán lại sự kiện đầy đủ
            dgv.CurrentCellDirtyStateChanged += ResultGrid_CurrentCellDirtyStateChanged;
            dgv.CellEndEdit += ResultGrid_CellEndEdit;
            dgv.CellValidating += ResultGrid_CellValidating;
            dgv.DataError += DgvResult_DataError;
        }


        /// <summary>
        /// Áp dụng trạng thái ReadOnly và style khóa cho các cột điểm đã bị khóa.
        /// </summary>
        private void ApplyColumnLocks(DataGridView dgv, int ki)
        {
            // CHUẨN HÓA: Đã xóa '_'
            if (lockStatusCache == null) return;

            var defaultStyle = dgv.DefaultCellStyle;
            var lockedStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(230, 230, 230),
                ForeColor = Color.FromArgb(120, 120, 120),
                SelectionBackColor = Color.FromArgb(230, 230, 230),
                SelectionForeColor = Color.FromArgb(120, 120, 120)
            };

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                string colName = col.DataPropertyName;
                string maCotDiem = MapColumnToLoai(colName, ki);

                if (maCotDiem == null) continue;

                // Cột Nhận xét và Ghi chú luôn cho phép sửa
                if (colName.Equals("NhanXet", StringComparison.OrdinalIgnoreCase) ||
                    colName.Equals("GhiChu", StringComparison.OrdinalIgnoreCase))
                {
                    col.ReadOnly = false;
                    col.DefaultCellStyle = defaultStyle;
                    col.HeaderText = col.HeaderText.Replace(" 🔒", "");
                    continue;
                }

                // Áp dụng khóa/mở khóa cho các cột điểm
                // CHUẨN HÓA: Đã xóa '_'
                if (lockStatusCache.TryGetValue(maCotDiem, out bool isLocked) && isLocked)
                {
                    col.ReadOnly = true;
                    col.DefaultCellStyle = lockedStyle;
                    col.HeaderText = col.HeaderText.Replace(" 🔒", "") + " 🔒";
                }
                else
                {
                    col.ReadOnly = false;
                    col.DefaultCellStyle = defaultStyle;
                    col.HeaderText = col.HeaderText.Replace(" 🔒", "");
                }
            }
        }

        /// <summary>
        /// Commit ngay khi giá trị cell thay đổi (dùng cho ComboBox, CheckBox).
        /// </summary>
        private void ResultGrid_CurrentCellDirtyStateChanged(object s, EventArgs e) { var g = s as DataGridView; if (g != null && g.IsCurrentCellDirty) g.CommitEdit(DataGridViewDataErrorContexts.Commit); }

        /// <summary>
        /// Kích hoạt lưu dữ liệu khi kết thúc chỉnh sửa 1 ô.
        /// </summary>
        private void ResultGrid_CellEndEdit(object s, DataGridViewCellEventArgs e) { var g = s as DataGridView; if (g == null || e.RowIndex < 0) return; SaveKetQuaRow(g, e.RowIndex, e.ColumnIndex); }

        /// <summary>
        /// Lưu thay đổi của một hàng (Row) trong lưới kết quả.
        /// </summary>
        private void SaveKetQuaRow(DataGridView dgv, int row, int col)
        {
            if (dgv.Columns[col].ReadOnly)
            {
                MessageBox.Show("Cột điểm này đã bị khóa. Không thể lưu.", "Đã Khóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LoadKetQuaGrids(); // Tải lại để khôi phục giá trị cũ
                return;
            }

            if (dgv.Tag == null || !(dgv.Tag is Tuple<int, string> ctx)) return;
            int ki = ctx.Item1;
            string maMon = ctx.Item2;
            var r = dgv.Rows[row];
            string maHS = r.Cells["MaHS"]?.Value?.ToString();
            if (string.IsNullOrEmpty(maHS)) return;

            string colName = dgv.Columns[col].DataPropertyName;
            string loai = MapColumnToLoai(colName, ki);
            if (string.IsNullOrEmpty(loai)) return;

            object val = r.Cells[col].Value;

            try
            {
                // Phân biệt lưu Nhận xét/Ghi chú và lưu Điểm
                if (colName.Equals("NhanXet", StringComparison.OrdinalIgnoreCase))
                {
                    DatabaseHelper.UpdateAcademicResult_Text(maHS, maMon, loai, val?.ToString() ?? "", true);
                }
                else if (colName.Equals("GhiChu", StringComparison.OrdinalIgnoreCase))
                {
                    DatabaseHelper.UpdateAcademicResult_Text(maHS, maMon, loai, val?.ToString() ?? "", false);
                }
                else
                {
                    float? diem = null;
                    if (val != null && val != DBNull.Value && float.TryParse(val.ToString(), out float p))
                        diem = p;

                    DatabaseHelper.UpdateAcademicResult(maHS, maMon, loai, diem);
                }
            }
            catch (Exception ex)
            {
                // Bắt lỗi (ví dụ: lỗi 50000 từ CSDL) và tải lại lưới
                MessageBox.Show("Lỗi khi lưu ", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadKetQuaGrids();
            }
        }

        /// <summary>
        /// Ánh xạ tên cột DataPropertyName sang Mã Cột Điểm trong CSDL.
        /// </summary>
        private string MapColumnToLoai(string c, int ki)
        {
            if (string.IsNullOrEmpty(c)) return null;
            c = c.Trim();
            if (c.Equals("Thang1", StringComparison.OrdinalIgnoreCase)) return $"Thang1_Ki{ki}";
            if (c.Equals("Thang2", StringComparison.OrdinalIgnoreCase)) return $"Thang2_Ki{ki}";
            if (c.Equals("Thang3", StringComparison.OrdinalIgnoreCase)) return $"Thang3_Ki{ki}";
            if (c.Equals("GiuaKi", StringComparison.OrdinalIgnoreCase)) return $"GiuaKi{ki}";
            if (c.Equals("CuoiKi", StringComparison.OrdinalIgnoreCase)) return $"CuoiKi{ki}";

            // NhanXet và GhiChu được lưu theo mốc CuoiKi
            if (c.Equals("NhanXet", StringComparison.OrdinalIgnoreCase)) return $"CuoiKi{ki}";
            if (c.Equals("GhiChu", StringComparison.OrdinalIgnoreCase)) return $"CuoiKi{ki}";

            return null;
        }

        /// <summary>
        /// Hiển thị giao diện "Học Sinh" (Danh sách học sinh).
        /// </summary>
        private void ShowHocSinh()
        {
            SaveAllCurrentEdits();
            StopQrCamera();
            panelContent.Controls.Clear();
            // CHUẨN HÓA: Đã xóa '_'
            if (string.IsNullOrEmpty(maLop)) { MessageBox.Show("Vui lòng chọn một lớp trước."); return; }

            dgvHocSinh.Dock = DockStyle.Fill;
            dgvHocSinh.DataSource = null;
            dgvHocSinh.Columns.Clear();
            dgvHocSinh.AutoGenerateColumns = true;
            dgvHocSinh.AllowUserToAddRows = false;
            dgvHocSinh.AllowUserToDeleteRows = false;
            // CHUẨN HÓA: Đã xóa '_'
            dgvHocSinh.DataSource = DatabaseHelper.GetStudentsByClass(maLop);
            panelContent.Controls.Add(dgvHocSinh);
            dgvHocSinh.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvHocSinh.CellBorderStyle = DataGridViewCellBorderStyle.Single;
        }

        /// <summary>
        /// Đảm bảo mọi thay đổi đang chỉnh sửa trên các lưới được commit trước khi chuyển tab.
        /// </summary>
        private void SaveAllCurrentEdits()
        {
            try { dgvDiemDanh?.EndEdit(); } catch { }
            try { dgvKi1?.EndEdit(); } catch { }
            try { dgvKi2?.EndEdit(); } catch { }
        }

        #endregion

        #region Dispose (Dọn dẹp tài nguyên)

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dừng camera và timer (rất quan trọng)
                StopQrCamera();

                // Gỡ bỏ sự kiện của các control trong Designer
                if (btnQuayLaiChonLop != null) btnQuayLaiChonLop.Click -= btnQuayLaiChonLop_Click;
                if (rbDiemDanh != null) rbDiemDanh.CheckedChanged -= TabButton_CheckedChanged;
                if (rbQR != null) rbQR.CheckedChanged -= TabButton_CheckedChanged;
                if (rbKetQua != null) rbKetQua.CheckedChanged -= TabButton_CheckedChanged;
                if (rbHocSinh != null) rbHocSinh.CheckedChanged -= TabButton_CheckedChanged;
                if (flowLayoutPanelLop != null) flowLayoutPanelLop.Resize -= FlowLayoutPanelLop_Resize;

                // Gỡ bỏ sự kiện của các control động (class fields)
                if (dgvHomeroomGradebook != null) dgvHomeroomGradebook.CellFormatting -= dgvHomeroomGradebook_CellFormatting;
                if (dgvDiemDanh != null) dgvDiemDanh.DataBindingComplete -= DataGridView_DataBindingComplete;
                if (dgvHocSinh != null) dgvHocSinh.DataBindingComplete -= DataGridView_DataBindingComplete;
                if (dgvKi1 != null) dgvKi1.DataBindingComplete -= DataGridView_DataBindingComplete;
                if (dgvKi2 != null) dgvKi2.DataBindingComplete -= DataGridView_DataBindingComplete;
                if (dgvHomeroomGradebook != null) dgvHomeroomGradebook.DataBindingComplete -= DataGridView_DataBindingComplete;
                if (dgvQuyLop != null) dgvQuyLop.DataBindingComplete -= DataGridView_DataBindingComplete;

                if (cbLoaiDiem != null) cbLoaiDiem.SelectedIndexChanged -= LoadHomeroomGradebookData;
                if (btnThemKhoanQuy != null) btnThemKhoanQuy.Click -= BtnThemKhoanQuy_Click;
                if (dgvQuyLop != null) dgvQuyLop.CellClick -= DgvQuyLop_CellClick;
                if (dtpNgayTC != null) dtpNgayTC.ValueChanged -= AttendanceFilter_Changed;
                if (cbBuoiTC != null) cbBuoiTC.SelectedIndexChanged -= AttendanceFilter_Changed;
                if (btnBatDauTC != null) btnBatDauTC.Click -= BtnBatDauTC_Click;
                if (btnLuuTC != null) btnLuuTC.Click -= BtnLuuTC_Click;
                if (dgvDiemDanh != null)
                {
                    dgvDiemDanh.CurrentCellDirtyStateChanged -= DgvThuCong_CurrentCellDirtyStateChanged;
                    dgvDiemDanh.CellEndEdit -= DgvThuCong_CellEndEdit;
                    dgvDiemDanh.DataError -= DgvDiemDanh_DataError;
                }
                if (btnStartQRScan != null) btnStartQRScan.Click -= BtnStartQRScan_Click;
                if (btnStopQRScan != null) btnStopQRScan.Click -= BtnStopQRScan_Click; // THÊM MỚI
                if (cbMonHocChon != null) cbMonHocChon.SelectedIndexChanged -= CbMonHocChon_SelectedIndexChanged;
                if (dgvKi1 != null)
                {
                    dgvKi1.CellEndEdit -= ResultGrid_CellEndEdit;
                    dgvKi1.CellValidating -= ResultGrid_CellValidating;
                    dgvKi1.CurrentCellDirtyStateChanged -= ResultGrid_CurrentCellDirtyStateChanged;
                }
                if (dgvKi2 != null)
                {
                    dgvKi2.CellEndEdit -= ResultGrid_CellEndEdit;
                    dgvKi2.CellValidating -= ResultGrid_CellValidating;
                    dgvKi2.CurrentCellDirtyStateChanged -= ResultGrid_CurrentCellDirtyStateChanged;
                }

                // Dọn dẹp các control động (IDisposable)
                var dynamicDisposables = new List<IDisposable>
                {
                    dgvDiemDanh, dgvKetQua, dgvHocSinh, dgvKi1, dgvKi2, dgvHomeroomGradebook, dgvQuyLop,
                    cbLoaiDiem, lblNoGradebookData,
                    dtpNgayTC, dtpNgayQR, dtpThoiGianTC, cbBuoiTC, cbBuoiQR,
                    btnBatDauTC, btnLuuTC, btnStartQRScan, btnStopQRScan,
                    lblThongKeTC, lblQRStatus, picQR,
                    tabDiemDanh, tabThuCong, tabQR, // CHUẨN HÓA: Đã xóa '_'
                    latestFrame,
                    lblTongThu_Value, lblTongChi_Value, lblTonQuy_Value, btnThemKhoanQuy,
                    cbMonHocChon
                };

                foreach (var item in dynamicDisposables)
                {
                    item?.Dispose();
                }

                // Dọn dẹp các button lớp học (nếu còn)
                if (flowLayoutPanelLop != null)
                {
                    foreach (Button btn in flowLayoutPanelLop.Controls.OfType<Button>().ToList())
                    {
                        btn.Click -= HomeroomButton_Click;
                        btn.Click -= LopButton_Click;
                        btn.Dispose();
                    }
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}