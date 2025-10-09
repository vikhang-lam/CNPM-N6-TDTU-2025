using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
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

        // Thủ công
        private DateTimePicker dtpNgayTC;
        private ComboBox cbBuoiTC;
        private Button btnBatDauTC;
        private Button btnLuuTC;
        private Label lblThongKeTC;
        private bool _manualSessionActive = false;

        // QR
        private DateTimePicker dtpNgayQR;
        private ComboBox cbBuoiQR;
        private Button btnStartQRScan;
        private Button btnStopQRScan;
        private Button btnLuuQR;
        private Label lblQRStatus;
        private PictureBox picQR;
        private bool _qrScanning = false;

        // Camera
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Timer qrTimer;
        private NewFrameEventHandler videoFrameHandler;
        private Bitmap latestFrame;
        private readonly object _frameLock = new object();

        // Tabs
        private TabControl _tabDiemDanh;
        private TabPage _tabThuCong;
        private TabPage _tabQR;

        public UC_QuanLyLop(string username)
        {
            InitializeComponent();
            _username = username ?? string.Empty;
            _maLop = DatabaseHelper.GetLopByTeacher(username);
            _currentMaMon = DatabaseHelper.GetMonByTeacher(username);

            StyleSidebarButtons();
            btnDiemDanh.Click += BtnDiemDanh_Click;
            btnQR.Click += (s,e) => { SaveAllCurrentEdits(); ShowDiemDanh(selectQRTab:true); };
            btnKetQua.Click += BtnKetQua_Click;
            btnHocSinh.Click += BtnHocSinh_Click;

            ApplyGridStyle(dgvDiemDanh);
            ApplyGridStyle(dgvKetQua);
            ApplyGridStyle(dgvHocSinh);
            ApplyGridStyle(dgvKi1);
            ApplyGridStyle(dgvKi2);

            LoadHocSinh();
            ShowDiemDanh();
        }

        #region UI styling
        private void StyleSidebarButtons()
        {
            try
            {
                Button[] arr = { btnDiemDanh, btnQR, btnKetQua, btnHocSinh };
                foreach (var b in arr)
                {
                    if (b == null) continue;
                    b.Dock = DockStyle.Top;
                    b.Height = 50;
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 0;
                    b.ForeColor = Color.White;
                    b.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    b.TextAlign = ContentAlignment.MiddleLeft;
                    b.Padding = new Padding(15, 0, 0, 0);
                    b.BackColor = Color.FromArgb(45, 45, 65);
                }
            }
            catch { }
        }
        private void ApplyGridStyle(DataGridView dgv)
        {
            if (dgv == null) return;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 65);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowTemplate.Height = 36;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
        #endregion

        #region Data loaders
        private void LoadHocSinh()
        {
            if (string.IsNullOrEmpty(_maLop)) return;
            try { dgvHocSinh.DataSource = DatabaseHelper.GetHocSinhByLop(_maLop); } catch (Exception ex) { MessageBox.Show("Lỗi load học sinh: " + ex.Message); }
        }
        #endregion

        #region Sidebar handlers
        private void BtnDiemDanh_Click(object sender, EventArgs e) { SaveAllCurrentEdits(); ShowDiemDanh(); }
        private void BtnKetQua_Click(object sender, EventArgs e) { SaveAllCurrentEdits(); ShowKetQua(); }
        private void BtnHocSinh_Click(object sender, EventArgs e) { SaveAllCurrentEdits(); ShowHocSinh(); }
        #endregion

        #region Master attendance view
        private void ShowDiemDanh(bool selectQRTab = false)
        {
            StopQrCamera();
            panelContent.Controls.Clear();
            if (string.IsNullOrEmpty(_maLop)) { MessageBox.Show("Giáo viên chưa được gán lớp."); return; }
            _tabDiemDanh = new TabControl { Dock = DockStyle.Fill };
            _tabThuCong = new TabPage("Điểm danh thủ công");
            _tabQR = new TabPage("Điểm danh QR");
            BuildThuCongTab();
            BuildQRTab();
            _tabDiemDanh.TabPages.Add(_tabThuCong);
            _tabDiemDanh.TabPages.Add(_tabQR);
            panelContent.Controls.Add(_tabDiemDanh);
            if (selectQRTab) _tabDiemDanh.SelectedTab = _tabQR;
        }
        #endregion

        #region Thủ công (UC015)
        private void BuildThuCongTab()
        {
            var container = new Panel { Dock = DockStyle.Fill };
            var top = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 65,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            dtpNgayTC = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today, Width = 120 };
            cbBuoiTC = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 90 };
            cbBuoiTC.Items.AddRange(new[] { "Sáng", "Chiều" });
            cbBuoiTC.SelectedIndex = DateTime.Now.Hour < 12 ? 0 : 1;

            var btnXemTC = new Button { Text = "Xem", Width = 80, Height = 32, BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnXemTC.Click += (s, e) => LoadThuCong(readOnly: dtpNgayTC.Value.Date < DateTime.Today, createIfEmpty: false);

            btnBatDauTC = new Button { Text = "Bắt đầu", Width = 90, Height = 32, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnBatDauTC.Click += BtnBatDauTC_Click;

            btnLuuTC = new Button { Text = "Lưu", Width = 80, Height = 32, BackColor = Color.OrangeRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
            btnLuuTC.Click += BtnLuuTC_Click;

            lblThongKeTC = new Label { AutoSize = true, ForeColor = Color.Maroon, Padding = new Padding(20, 8, 0, 0) };

            top.Controls.Add(new Label { Text = "Ngày:", AutoSize = true, Padding = new Padding(0, 8, 5, 0) });
            top.Controls.Add(dtpNgayTC);
            top.Controls.Add(new Label { Text = "Buổi:", AutoSize = true, Padding = new Padding(15, 8, 5, 0) });
            top.Controls.Add(cbBuoiTC);
            top.Controls.Add(btnXemTC);
            top.Controls.Add(btnBatDauTC);
            top.Controls.Add(btnLuuTC);
            top.Controls.Add(lblThongKeTC);

            dgvDiemDanh.DataSource = null;
            dgvDiemDanh.Columns.Clear();
            dgvDiemDanh.ReadOnly = true;
            dgvDiemDanh.CurrentCellDirtyStateChanged -= DgvThuCong_CurrentCellDirtyStateChanged;
            dgvDiemDanh.CellEndEdit -= DgvThuCong_CellEndEdit;

            container.Controls.Add(dgvDiemDanh);
            container.Controls.Add(top);
            _tabThuCong.Controls.Clear();
            _tabThuCong.Controls.Add(container);

            // Load lần đầu: chỉ xem, không chỉnh cho tới khi bấm 'Bắt đầu'
            LoadThuCong(readOnly: true, createIfEmpty: false);
        }

        private void BtnBatDauTC_Click(object sender, EventArgs e)
        {
            if (dtpNgayTC.Value.Date < DateTime.Today)
            {
                MessageBox.Show("Không thể chỉnh ngày đã qua.");
                return;
            }
            _manualSessionActive = true;
            btnLuuTC.Enabled = true;
            DatabaseHelper.ExecTaoDiemDanhMacDinh(_maLop, dtpNgayTC.Value.Date, cbBuoiTC.SelectedItem?.ToString());
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
                    DatabaseHelper.ExecTaoDiemDanhMacDinh(_maLop, ngay, buoi);
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
                    // Bổ sung hiển thị NgayDD, Buoi nếu null để giáo viên thấy bối cảnh
                    if (r.Table.Columns.Contains("NgayDD") && (r["NgayDD"] == DBNull.Value || string.IsNullOrWhiteSpace(r["NgayDD"].ToString())))
                        r["NgayDD"] = ngay;
                    if (r.Table.Columns.Contains("Buoi") && (r["Buoi"] == DBNull.Value || string.IsNullOrWhiteSpace(r["Buoi"].ToString())))
                        r["Buoi"] = buoi;
                }

                dgvDiemDanh.DataSource = null;
                dgvDiemDanh.Columns.Clear();
                dgvDiemDanh.AutoGenerateColumns = true;
                dgvDiemDanh.DataSource = dt;

                if (dgvDiemDanh.Columns.Contains("TrangThai"))
                {
                    int idx = dgvDiemDanh.Columns["TrangThai"].Index;
                    dgvDiemDanh.Columns.Remove("TrangThai");
                    var col = new DataGridViewComboBoxColumn
                    {
                        Name = "TrangThai",
                        HeaderText = "Trạng thái",
                        DataPropertyName = "TrangThai",
                        FlatStyle = FlatStyle.Flat,
                        DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                    };
                    col.Items.AddRange("Có mặt", "Vắng", "Đi trễ", "Có phép", "Chưa điểm danh");
                    dgvDiemDanh.Columns.Insert(idx, col);
                }
                // Chỉ loại bỏ MaDD; giữ NgayDD và Buoi để giáo viên xem
                if (dgvDiemDanh.Columns.Contains("MaDD")) dgvDiemDanh.Columns.Remove("MaDD");
                if (dgvDiemDanh.Columns.Contains("NgayDD")) dgvDiemDanh.Columns["NgayDD"].HeaderText = "Ngày";
                if (dgvDiemDanh.Columns.Contains("Buoi")) dgvDiemDanh.Columns["Buoi"].HeaderText = "Buổi";

                // Cột NgayDD, Buoi luôn chỉ đọc
                if (dgvDiemDanh.Columns.Contains("NgayDD")) dgvDiemDanh.Columns["NgayDD"].ReadOnly = true;
                if (dgvDiemDanh.Columns.Contains("Buoi")) dgvDiemDanh.Columns["Buoi"].ReadOnly = true;

                dgvDiemDanh.ReadOnly = readOnly || !_manualSessionActive;
                // Cho phép riêng cột TrangThai chỉnh khi phiên active
                if (!dgvDiemDanh.ReadOnly && dgvDiemDanh.Columns.Contains("TrangThai"))
                {
                    foreach (DataGridViewColumn c in dgvDiemDanh.Columns) c.ReadOnly = c.Name != "TrangThai";
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
            catch (Exception ex) { MessageBox.Show("Lỗi tải: " + ex.Message); }
        }

        private void BtnLuuTC_Click(object sender, EventArgs e)
        {
            if (!_manualSessionActive) { MessageBox.Show("Chưa ở phiên chỉnh."); return; }
            try
            {
                dgvDiemDanh.EndEdit();
                int saved = 0;
                foreach (DataGridViewRow row in dgvDiemDanh.Rows)
                {
                    if (row.IsNewRow) continue;
                    string maHS = row.Cells["MaHS"]?.Value?.ToString();
                    if (string.IsNullOrEmpty(maHS)) continue;
                    string tt = row.Cells["TrangThai"]?.Value?.ToString();
                    if (string.Equals(tt, "Vắng mặt", StringComparison.OrdinalIgnoreCase)) tt = "Vắng";
                    if (string.IsNullOrWhiteSpace(tt)) tt = "Có mặt";
                    DatabaseHelper.UpsertDiemDanh(maHS, _maLop, dtpNgayTC.Value.Date, cbBuoiTC.SelectedItem?.ToString(), tt);
                    saved++;
                }
                MessageBox.Show($"Đã lưu {saved} bản ghi.");
                _manualSessionActive = false; btnLuuTC.Enabled = false;
                LoadThuCong(readOnly: false, createIfEmpty: false);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi lưu: " + ex.Message); }
        }

        private void DgvThuCong_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        { try { if (dgvDiemDanh.IsCurrentCellDirty) dgvDiemDanh.CommitEdit(DataGridViewDataErrorContexts.Commit); } catch { } }
        private void DgvThuCong_CellEndEdit(object sender, DataGridViewCellEventArgs e) { UpdateThongKeThuCong(); }
        private void DgvDiemDanh_DataError(object sender, DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }
        private void UpdateThongKeThuCong()
        {
            if (lblThongKeTC == null || dgvDiemDanh.DataSource == null) return;
            int total = 0, vang = 0, ditre = 0;
            foreach (DataGridViewRow r in dgvDiemDanh.Rows)
            {
                if (r.IsNewRow) continue; total++;
                var tt = r.Cells["TrangThai"]?.Value?.ToString();
                if (string.Equals(tt, "Vắng", StringComparison.OrdinalIgnoreCase) || string.Equals(tt, "Vắng mặt", StringComparison.OrdinalIgnoreCase)) vang++;
                else if (string.Equals(tt, "Đi trễ", StringComparison.OrdinalIgnoreCase)) ditre++;
            }
            lblThongKeTC.Text = $"Tổng: {total} | Vắng: {vang} | Đi trễ: {ditre}";
        }
        #endregion

        #region QR (UC016)
        private void BuildQRTab()
        {
            var container = new Panel { Dock = DockStyle.Fill };
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 65, Padding = new Padding(10), FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            dtpNgayQR = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today, Width = 120 };
            cbBuoiQR = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 90 };
            cbBuoiQR.Items.AddRange(new[] { "Sáng", "Chiều" }); cbBuoiQR.SelectedIndex = DateTime.Now.Hour < 12 ? 0 : 1;
            btnStartQRScan = new Button { Text = "Quét", Width = 80, Height = 32, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnStopQRScan = new Button { Text = "Dừng", Width = 80, Height = 32, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnLuuQR = new Button { Text = "Lưu", Width = 80, Height = 32, BackColor = Color.OrangeRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            lblQRStatus = new Label { AutoSize = true, ForeColor = Color.Navy, Padding = new Padding(15, 8, 0, 0), Text = "Chưa quét" };
            btnStartQRScan.Click += BtnStartQRScan_Click;
            btnStopQRScan.Click += (s,e)=> { StopQrCamera(); lblQRStatus.Text = "Đã dừng"; };
            btnLuuQR.Click += (s,e)=> MessageBox.Show("Điểm danh QR đã được lưu theo từng lượt quét.");
            top.Controls.Add(new Label { Text = "Ngày:", AutoSize = true, Padding = new Padding(0,8,5,0)});
            top.Controls.Add(dtpNgayQR);
            top.Controls.Add(new Label { Text = "Buổi:", AutoSize = true, Padding = new Padding(15,8,5,0)});
            top.Controls.Add(cbBuoiQR);
            top.Controls.Add(btnStartQRScan);
            top.Controls.Add(btnStopQRScan);
            top.Controls.Add(btnLuuQR);
            top.Controls.Add(lblQRStatus);
            picQR = new PictureBox { Dock = DockStyle.Top, Height = 350, BackColor = Color.Black, SizeMode = PictureBoxSizeMode.Zoom };
            container.Controls.Add(picQR); container.Controls.Add(top);
            _tabQR.Controls.Clear(); _tabQR.Controls.Add(container);
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
                qrTimer = new Timer { Interval = 800 }; qrTimer.Tick += QrTimer_Tick; qrTimer.Start();
                _qrScanning = true; lblQRStatus.Text = "Đang quét...";
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
                        picQR.Invoke(new Action(()=> { picQR.Image?.Dispose(); picQR.Image = (Bitmap)frame.Clone(); }));
                    else { picQR.Image?.Dispose(); picQR.Image = (Bitmap)frame.Clone(); }
                }
                frame.Dispose();
            } catch { }
        }
        private void QrTimer_Tick(object sender, EventArgs e)
        {
            if (!_qrScanning) return;
            try
            {
                Bitmap bmp = null; lock (_frameLock) { if (latestFrame != null) bmp = (Bitmap)latestFrame.Clone(); }
                if (bmp == null) return;
                var reader = new BarcodeReader(); var result = reader.Decode(bmp); bmp.Dispose();
                if (result != null)
                {
                    string maHS = result.Text?.Trim();
                    if (!string.IsNullOrEmpty(maHS))
                    {
                        DatabaseHelper.LuuDiemDanh(maHS, "Có mặt", dtpNgayQR.Value.Date, cbBuoiQR.SelectedItem?.ToString());
                        if (lblQRStatus != null && !lblQRStatus.IsDisposed)
                            lblQRStatus.Invoke(new Action(()=> lblQRStatus.Text = $"✅ {maHS} - {DateTime.Now:T}"));
                    }
                }
            } catch { }
        }
        private void StopQrCamera()
        {
            try
            {
                _qrScanning = false;
                qrTimer?.Stop(); qrTimer?.Dispose(); qrTimer = null;
                if (videoSource != null)
                {
                    if (videoSource.IsRunning) { videoSource.SignalToStop(); if (videoFrameHandler != null) videoSource.NewFrame -= videoFrameHandler; }
                    videoSource = null;
                }
                picQR?.Image?.Dispose(); if (picQR!=null) picQR.Image = null;
                latestFrame?.Dispose(); latestFrame = null;
            } catch { }
        }
        #endregion

        #region Kết quả học tập
        private void ShowKetQua()
        {
            StopQrCamera();
            panelContent.Controls.Clear();
            if (string.IsNullOrEmpty(_maLop)) { MessageBox.Show("Giáo viên chưa được gán lớp."); return; }
            if (string.IsNullOrEmpty(_currentMaMon)) { MessageBox.Show("Giáo viên chưa được gán môn."); return; }
            TabControl tab = new TabControl { Dock = DockStyle.Fill };
            TabPage p1 = new TabPage("Kì 1"); TabPage p2 = new TabPage("Kì 2");
            DataTable dt1 = DatabaseHelper.GetBangDiemPivot(_maLop, 1, _currentMaMon);
            DataTable dt2 = DatabaseHelper.GetBangDiemPivot(_maLop, 2, _currentMaMon);
            SetupResultGrid(dgvKi1, dt1, 1, _currentMaMon);
            SetupResultGrid(dgvKi2, dt2, 2, _currentMaMon);
            p1.Controls.Add(dgvKi1); p2.Controls.Add(dgvKi2);
            tab.TabPages.Add(p1); tab.TabPages.Add(p2);
            panelContent.Controls.Add(tab);
        }
        private void SetupResultGrid(DataGridView dgv, DataTable dt, int ki, string maMon)
        {
            if (dgv == null) return;
            dgv.CellEndEdit -= ResultGrid_CellEndEdit;
            dgv.CurrentCellDirtyStateChanged -= ResultGrid_CurrentCellDirtyStateChanged;
            dgv.DataSource = null; dgv.Columns.Clear(); dgv.AutoGenerateColumns = true; dgv.DataSource = dt;
            if (dt != null && dt.Columns.Contains("HoTen") && dgv.Columns.Contains("HoTen")) dgv.Columns["HoTen"].ReadOnly = true;
            if (dt != null && dt.Columns.Contains("MaHS") && dgv.Columns.Contains("MaHS")) dgv.Columns["MaHS"].Visible = false;
            dgv.Tag = Tuple.Create(ki, maMon);
            dgv.CurrentCellDirtyStateChanged += ResultGrid_CurrentCellDirtyStateChanged;
            dgv.CellEndEdit += ResultGrid_CellEndEdit;
        }
        private void ResultGrid_CurrentCellDirtyStateChanged(object s, EventArgs e) { var g = s as DataGridView; if (g!=null && g.IsCurrentCellDirty) g.CommitEdit(DataGridViewDataErrorContexts.Commit); }
        private void ResultGrid_CellEndEdit(object s, DataGridViewCellEventArgs e) { var g = s as DataGridView; if (g==null|| e.RowIndex<0) return; SaveKetQuaRow(g,e.RowIndex,e.ColumnIndex); }
        private void SaveKetQuaRow(DataGridView dgv, int row, int col)
        {
            if (dgv.Tag==null) return; var ctx = dgv.Tag as Tuple<int,string>; if (ctx==null) return; int ki = ctx.Item1; string maMon = ctx.Item2;
            var r = dgv.Rows[row]; string maHS = r.Cells["MaHS"]?.Value?.ToString(); if (string.IsNullOrEmpty(maHS)) return;
            string colName = dgv.Columns[col].Name; if (string.IsNullOrEmpty(colName)) colName = dgv.Columns[col].HeaderText;
            string loai = MapColumnToLoai(colName, ki); if (string.IsNullOrEmpty(loai)) return;
            object val = r.Cells[col].Value; float? diem = null; if (val!=null && val!=DBNull.Value && float.TryParse(val.ToString(), out float p)) diem = p;
            DatabaseHelper.UpdateKetQuaHocTap(maHS, maMon, loai, diem);
        }
        private string MapColumnToLoai(string c, int ki)
        {
            if (string.IsNullOrEmpty(c)) return null; c = c.Trim();
            if (c.IndexOf("Thang1",StringComparison.OrdinalIgnoreCase)>=0 || c.IndexOf("Tháng 1",StringComparison.OrdinalIgnoreCase)>=0) return $"Thang1_Ki{ki}";
            if (c.IndexOf("Thang2",StringComparison.OrdinalIgnoreCase)>=0 || c.IndexOf("Tháng 2",StringComparison.OrdinalIgnoreCase)>=0) return $"Thang2_Ki{ki}";
            if (c.IndexOf("Thang3",StringComparison.OrdinalIgnoreCase)>=0 || c.IndexOf("Tháng 3",StringComparison.OrdinalIgnoreCase)>=0) return $"Thang3_Ki{ki}";
            if (c.IndexOf("GiuaKi",StringComparison.OrdinalIgnoreCase)>=0 || c.IndexOf("Giữa",StringComparison.OrdinalIgnoreCase)>=0) return $"GiuaKi{ki}";
            if (c.IndexOf("CuoiKi",StringComparison.OrdinalIgnoreCase)>=0 || c.IndexOf("Cuối",StringComparison.OrdinalIgnoreCase)>=0) return $"CuoiKi{ki}";
            if (c.IndexOf("NhanXet",StringComparison.OrdinalIgnoreCase)>=0 || c.IndexOf("Nhận",StringComparison.OrdinalIgnoreCase)>=0) return "NhanXet";
            if (c.IndexOf("GhiChu",StringComparison.OrdinalIgnoreCase)>=0 || c.IndexOf("Ghi chú",StringComparison.OrdinalIgnoreCase)>=0) return "GhiChu";
            return null;
        }
        #endregion

        #region Học sinh
        private void ShowHocSinh()
        {
            StopQrCamera();
            panelContent.Controls.Clear();
            dgvHocSinh.DataSource = null; dgvHocSinh.Columns.Clear(); dgvHocSinh.AutoGenerateColumns = true; dgvHocSinh.DataSource = DatabaseHelper.GetHocSinhByLop(_maLop);
            dgvHocSinh.RowValidated -= DgvHocSinh_RowValidated;
            dgvHocSinh.RowValidated += DgvHocSinh_RowValidated;
            panelContent.Controls.Add(dgvHocSinh);
        }
        private void DgvHocSinh_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvHocSinh.CurrentRow == null) return; var row = dgvHocSinh.CurrentRow; string maHS = row.Cells["MaHS"]?.Value?.ToString(); if (string.IsNullOrEmpty(maHS)) return;
                string hoTen = row.Cells["HoTen"]?.Value?.ToString(); string gioiTinh = row.Cells["GioiTinh"]?.Value?.ToString(); DateTime? ns = null; if (row.Cells["NgaySinh"]?.Value!=null && row.Cells["NgaySinh"].Value!=DBNull.Value) ns = Convert.ToDateTime(row.Cells["NgaySinh"].Value); string diaChi = row.Cells["DiaChi"]?.Value?.ToString();
                DatabaseHelper.UpdateHocSinh(maHS, hoTen, gioiTinh, ns, diaChi);
            } catch (Exception ex) { MessageBox.Show("Lỗi lưu học sinh: " + ex.Message); }
        }
        #endregion

        #region Helpers
        private void SaveAllCurrentEdits()
        { try { dgvDiemDanh?.EndEdit(); } catch { } try { dgvKi1?.EndEdit(); } catch { } try { dgvKi2?.EndEdit(); } catch { } }
        protected override void Dispose(bool disposing) { if (disposing) StopQrCamera(); base.Dispose(disposing); }
        #endregion
    }
}
