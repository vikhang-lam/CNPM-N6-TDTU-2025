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

        // QR / camera
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Timer qrTimer;
        private NewFrameEventHandler videoFrameHandler;

        // keep current subject for teachers
        private string _currentMaMon;

        public UC_QuanLyLop(string username)
        {
            InitializeComponent();

            _username = username ?? string.Empty;
            _maLop = DatabaseHelper.GetLopByTeacher(username);
            _currentMaMon = DatabaseHelper.GetMonByTeacher(username);

            // style sidebar (separate method to avoid putting code in Designer)
            StyleSidebarButtons();

            // wire button clicks to named handlers (no lambdas)
            btnDiemDanh.Click += BtnDiemDanh_Click;
            btnQR.Click += BtnQR_Click;
            btnKetQua.Click += BtnKetQua_Click;
            btnHocSinh.Click += BtnHocSinh_Click;

            // apply grid visuals (safe)
            ApplyGridStyle(dgvDiemDanh);
            ApplyGridStyle(dgvKetQua);
            ApplyGridStyle(dgvHocSinh);
            ApplyGridStyle(dgvKi1);
            ApplyGridStyle(dgvKi2);

            // load minimal data
            LoadHocSinh();
            ShowDiemDanh();
        }

        #region Styling helpers
        private void StyleSidebarButtons()
        {
            try
            {
                Button[] sidebarBtns = { btnDiemDanh, btnQR, btnKetQua, btnHocSinh };
                foreach (var btn in sidebarBtns)
                {
                    if (btn == null) continue;
                    btn.Dock = DockStyle.Top;
                    btn.Height = 50;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    btn.TextAlign = ContentAlignment.MiddleLeft;
                    btn.Padding = new Padding(15, 0, 0, 0);
                    btn.BackColor = Color.FromArgb(45, 45, 65);
                }
            }
            catch { /* swallow: styling shouldn't break app */ }
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

        #region Loaders
        private void LoadHocSinh()
        {
            if (string.IsNullOrEmpty(_maLop))
            {
                // teacher not assigned a class
                return;
            }

            try
            {
                DataTable dtHs = DatabaseHelper.GetHocSinhByLop(_maLop);
                if (dgvHocSinh != null) dgvHocSinh.DataSource = dtHs;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load học sinh: " + ex.Message);
            }
        }
        #endregion

        #region Sidebar button handlers
        private void BtnDiemDanh_Click(object sender, EventArgs e)
        {
            // Save current working changes of other grids before switching
            SaveAllCurrentEdits();

            ShowDiemDanh();
        }

        private void BtnQR_Click(object sender, EventArgs e)
        {
            SaveAllCurrentEdits();
            ShowQR();
        }

        private void BtnKetQua_Click(object sender, EventArgs e)
        {
            SaveAllCurrentEdits();
            ShowKetQua();
        }

        private void BtnHocSinh_Click(object sender, EventArgs e)
        {
            SaveAllCurrentEdits();
            ShowHocSinh();
        }
        #endregion

        #region Điểm danh (UI + events)
        private void ShowDiemDanh()
        {
            panelContent.Controls.Clear();

            if (string.IsNullOrEmpty(_maLop))
            {
                MessageBox.Show("Giáo viên chưa được gán lớp.");
                return;
            }

            // Lấy dữ liệu điểm danh (các cột: MaDD, MaHS, HoTen, NgayDD, Buoi, TrangThai)
            DataTable dtAll = DatabaseHelper.GetDiemDanhByLop(_maLop);

            // Lọc ngày hôm nay
            DataTable dtToday = dtAll.Clone();
            DateTime today = DateTime.Today;
            foreach (DataRow r in dtAll.Rows)
            {
                if (r["NgayDD"] != DBNull.Value)
                {
                    DateTime ngay = Convert.ToDateTime(r["NgayDD"]);
                    if (ngay.Date == today.Date)
                        dtToday.ImportRow(r);
                }
            }

            // Nếu hôm nay chưa có thì tạo mặc định
            if (dtToday.Rows.Count == 0)
            {
                try
                {
                    DatabaseHelper.ExecTaoDiemDanhMacDinh(_maLop);
                    dtAll = DatabaseHelper.GetDiemDanhByLop(_maLop);
                    foreach (DataRow r in dtAll.Rows)
                    {
                        if (r["NgayDD"] != DBNull.Value)
                        {
                            DateTime ngay = Convert.ToDateTime(r["NgayDD"]);
                            if (ngay.Date == today.Date)
                                dtToday.ImportRow(r);
                        }
                    }
                }
                catch { /* ignore if helper not present */ }
            }

            // Nếu cột TrangThai rỗng thì mặc định "Có mặt"
            foreach (DataRow row in dtToday.Rows)
            {
                if (row["TrangThai"] == DBNull.Value || string.IsNullOrWhiteSpace(row["TrangThai"].ToString()))
                {
                    row["TrangThai"] = "Có mặt";
                }
            }

            // Bind vào DataGridView
            dgvDiemDanh.DataSource = null;
            dgvDiemDanh.Columns.Clear();
            dgvDiemDanh.AutoGenerateColumns = true;
            dgvDiemDanh.DataSource = dtToday;

            // Thay cột TrangThai thành ComboBox
            if (dtToday.Columns.Contains("TrangThai") && dgvDiemDanh.Columns["TrangThai"] != null)
            {
                int idx = dgvDiemDanh.Columns["TrangThai"].Index;
                dgvDiemDanh.Columns.Remove("TrangThai");

                DataGridViewComboBoxColumn cb = new DataGridViewComboBoxColumn();
                cb.Name = "TrangThai";
                cb.HeaderText = "Trạng thái";
                cb.DataPropertyName = "TrangThai";
                cb.Items.AddRange("Có mặt", "Vắng mặt", "Đi trễ");
                cb.FlatStyle = FlatStyle.Flat;

                dgvDiemDanh.Columns.Insert(idx, cb);
            }

            // Các event để xử lý lưu thay đổi
            dgvDiemDanh.CurrentCellDirtyStateChanged -= DgvDiemDanh_CurrentCellDirtyStateChanged;
            dgvDiemDanh.CurrentCellDirtyStateChanged += DgvDiemDanh_CurrentCellDirtyStateChanged;

            dgvDiemDanh.CellEndEdit -= DgvDiemDanh_CellEndEdit;
            dgvDiemDanh.CellEndEdit += DgvDiemDanh_CellEndEdit;

            panelContent.Controls.Add(dgvDiemDanh);
        }


        private void DgvDiemDanh_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // Commit immediately when combobox changed so CellEndEdit fires
            try
            {
                if (dgvDiemDanh.IsCurrentCellDirty)
                    dgvDiemDanh.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
            catch { }
        }

        private void DgvDiemDanh_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                DataGridViewRow row = dgvDiemDanh.Rows[e.RowIndex];

                // if the edited column is TrangThai or any column, save row
                SaveDiemDanhRow(row);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu điểm danh: " + ex.Message);
            }
        }

        private void SaveDiemDanhRow(DataGridViewRow row)
        {
            if (row == null) return;

            // MaDD may be null (if DB doesn't have record) -> try use MaHS and call LuuDiemDanh
            string maDD = row.Cells["MaDD"]?.Value?.ToString();
            string maHS = row.Cells["MaHS"]?.Value?.ToString();
            string trangThai = row.Cells["TrangThai"]?.Value?.ToString() ?? "Vắng mặt";

            if (!string.IsNullOrEmpty(maDD))
            {
                // update existing
                DatabaseHelper.UpdateDiemDanh(maDD, trangThai);
            }
            else if (!string.IsNullOrEmpty(maHS))
            {
                // insert or update today's record by MaHS
                DatabaseHelper.LuuDiemDanh(maHS, trangThai);
                // reload to get MaDD
                DataTable dtAll = DatabaseHelper.GetDiemDanhByLop(_maLop);
                // refresh today's subset
                ShowDiemDanh();
            }
        }
        #endregion

        #region QR (camera + decode)
        private PictureBox _pictureQR;
        private Button _btnStartQR, _btnStopQR;
        private Label _lblQRStatus;

        private void ShowQR()
        {
            panelContent.Controls.Clear();

            // Create controls dynamically so designer not required to have them
            var p = new Panel() { Dock = DockStyle.Fill };

            _pictureQR = new PictureBox() { Dock = DockStyle.Top, Height = 420, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Black };
            FlowLayoutPanel fl = new FlowLayoutPanel() { Dock = DockStyle.Bottom, Height = 80, Padding = new Padding(10) };

            _btnStartQR = new Button() { Text = "▶ Bắt đầu quét", Width = 140, Height = 40, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            _btnStopQR = new Button() { Text = "⏹ Dừng quét", Width = 140, Height = 40, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            _lblQRStatus = new Label() { Text = "Chưa quét...", AutoSize = false, Width = 300, Height = 40, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 10) };

            _btnStartQR.Click += BtnStartQR_Click;
            _btnStopQR.Click += BtnStopQR_Click;

            fl.Controls.Add(_btnStartQR);
            fl.Controls.Add(_btnStopQR);
            fl.Controls.Add(_lblQRStatus);

            p.Controls.Add(_pictureQR);
            p.Controls.Add(fl);

            panelContent.Controls.Add(p);
        }

        private void BtnStartQR_Click(object sender, EventArgs e)
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices == null || videoDevices.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy camera.");
                    return;
                }

                // choose first camera
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoFrameHandler = new NewFrameEventHandler(Video_NewFrame);
                videoSource.NewFrame += videoFrameHandler;
                videoSource.Start();

                // timer to decode frame periodically
                qrTimer = new Timer { Interval = 800 };
                qrTimer.Tick -= QrTimer_Tick;
                qrTimer.Tick += QrTimer_Tick;
                qrTimer.Start();

                _lblQRStatus.Text = "Đang quét...";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi bắt đầu camera: " + ex.Message);
            }
        }

        private void BtnStopQR_Click(object sender, EventArgs e)
        {
            StopQrCamera();
            _lblQRStatus.Text = "Đã dừng.";
        }

        private void StopQrCamera()
        {
            try
            {
                qrTimer?.Stop();
                qrTimer?.Dispose();
                qrTimer = null;

                if (videoSource != null)
                {
                    if (videoSource.IsRunning)
                    {
                        videoSource.SignalToStop();
                        videoSource.NewFrame -= videoFrameHandler;
                    }
                    videoSource = null;
                }

                if (_pictureQR != null)
                {
                    _pictureQR.Image?.Dispose();
                    _pictureQR.Image = null;
                }
            }
            catch { }
        }

        private Bitmap latestFrame;
        private readonly object _frameLock = new object();

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
                lock (_frameLock)
                {
                    latestFrame?.Dispose();
                    latestFrame = (Bitmap)frame.Clone();
                }

                // display to picture box (invoke if needed)
                if (_pictureQR != null && !_pictureQR.IsDisposed)
                {
                    if (_pictureQR.InvokeRequired)
                    {
                        _pictureQR.Invoke(new Action(() =>
                        {
                            _pictureQR.Image?.Dispose();
                            _pictureQR.Image = (Bitmap)frame.Clone();
                        }));
                    }
                    else
                    {
                        _pictureQR.Image?.Dispose();
                        _pictureQR.Image = (Bitmap)frame.Clone();
                    }
                }
                frame.Dispose();
            }
            catch { }
        }

        private void QrTimer_Tick(object sender, EventArgs e)
        {
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
                        // mark present in DB (this helper should insert/update today's DiemDanh)
                        DatabaseHelper.LuuDiemDanh(maHS, "Có mặt");

                        // update UI label
                        if (_lblQRStatus != null && !_lblQRStatus.IsDisposed)
                        {
                            _lblQRStatus.Invoke(new Action(() =>
                            {
                                _lblQRStatus.Text = $"✅ Điểm danh thành công: {maHS} - {DateTime.Now:T}";
                            }));
                        }

                        // refresh attendance grid if visible
                        if (panelContent.Controls.Contains(dgvDiemDanh))
                        {
                            // reload
                            ShowDiemDanh();
                        }
                    }
                }
            }
            catch { /* ignore decode errors */ }
        }
        #endregion

        #region Kết quả học tập (pivot) + save handlers
        private void ShowKetQua()
        {
            panelContent.Controls.Clear();

            if (string.IsNullOrEmpty(_maLop))
            {
                MessageBox.Show("Giáo viên chưa được gán lớp.");
                return;
            }
            if (string.IsNullOrEmpty(_currentMaMon))
            {
                MessageBox.Show("Giáo viên chưa được gán môn.");
                return;
            }

            TabControl tabControl = new TabControl() { Dock = DockStyle.Fill };

            // Kì 1
            TabPage tp1 = new TabPage("Kì 1");
            DataTable dt1 = DatabaseHelper.GetBangDiemPivot(_maLop, 1, _currentMaMon);
            SetupResultGrid(dgvKi1, dt1, 1, _currentMaMon);
            tp1.Controls.Add(dgvKi1);

            // Kì 2
            TabPage tp2 = new TabPage("Kì 2");
            DataTable dt2 = DatabaseHelper.GetBangDiemPivot(_maLop, 2, _currentMaMon);
            SetupResultGrid(dgvKi2, dt2, 2, _currentMaMon);
            tp2.Controls.Add(dgvKi2);

            tabControl.TabPages.Add(tp1);
            tabControl.TabPages.Add(tp2);

            panelContent.Controls.Add(tabControl);
        }

        // prepare a result grid: attach explicit handlers, set Tag for context
        private void SetupResultGrid(DataGridView dgv, DataTable dt, int ki, string maMon)
        {
            if (dgv == null) return;

            // detach previous handlers (safe)
            dgv.CellEndEdit -= ResultGrid_CellEndEdit;
            dgv.CurrentCellDirtyStateChanged -= ResultGrid_CurrentCellDirtyStateChanged;

            // reset and bind
            dgv.DataSource = null;
            dgv.Columns.Clear();
            dgv.AutoGenerateColumns = true;
            dgv.DataSource = dt;

            // mark non-editable columns
            if (dt != null && dt.Columns.Contains("HoTen"))
            {
                if (dgv.Columns.Contains("HoTen"))
                    dgv.Columns["HoTen"].ReadOnly = true;
            }
            if (dt != null && dt.Columns.Contains("MaHS"))
            {
                if (dgv.Columns.Contains("MaHS"))
                    dgv.Columns["MaHS"].Visible = false; // keep MaHS hidden but available
            }

            // store context (ki & maMon) in Tag
            dgv.Tag = Tuple.Create(ki, maMon);

            // attach handlers
            dgv.CurrentCellDirtyStateChanged += ResultGrid_CurrentCellDirtyStateChanged;
            dgv.CellEndEdit += ResultGrid_CellEndEdit;
        }

        private void ResultGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null) return;
            if (dgv.IsCurrentCellDirty)
            {
                dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void ResultGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var dgv = sender as DataGridView;
                if (dgv == null || e.RowIndex < 0 || e.ColumnIndex < 0) return;

                SaveKetQuaRow(dgv, e.RowIndex, e.ColumnIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu kết quả: " + ex.Message);
            }
        }

        private void SaveKetQuaRow(DataGridView dgv, int rowIndex, int colIndex)
        {
            // get context
            if (dgv.Tag == null) return;
            var ctx = dgv.Tag as Tuple<int, string>;
            if (ctx == null) return;
            int ki = ctx.Item1;
            string maMon = ctx.Item2;

            DataGridViewRow row = dgv.Rows[rowIndex];
            string maHS = row.Cells["MaHS"]?.Value?.ToString();
            if (string.IsNullOrEmpty(maHS)) return; // no student id

            // find column name -> map to Loai
            string colName = dgv.Columns[colIndex].Name;
            if (string.IsNullOrEmpty(colName))
                colName = dgv.Columns[colIndex].HeaderText;

            string loai = MapColumnToLoai(colName, ki);
            if (string.IsNullOrEmpty(loai)) return;

            object valueObj = row.Cells[colIndex].Value;
            // try parse to float/null
            float? diem = null;
            if (valueObj != null && valueObj != DBNull.Value)
            {
                float parsed;
                if (float.TryParse(valueObj.ToString(), out parsed)) diem = parsed;
            }

            // call DB updater
            DatabaseHelper.UpdateKetQuaHocTap(maHS, maMon, loai, diem);
        }

        // map grid column name (or header) to KetQuaHocTap.Loai value
        private string MapColumnToLoai(string columnName, int ki)
        {
            if (string.IsNullOrEmpty(columnName)) return null;
            columnName = columnName.Trim();

            // Accept either "Thang1" or "Tháng 1" etc. We use simple mapping
            if (columnName.IndexOf("Thang1", StringComparison.OrdinalIgnoreCase) >= 0 ||
                columnName.IndexOf("Tháng 1", StringComparison.OrdinalIgnoreCase) >= 0) return $"Thang1_Ki{ki}";
            if (columnName.IndexOf("Thang2", StringComparison.OrdinalIgnoreCase) >= 0 ||
                columnName.IndexOf("Tháng 2", StringComparison.OrdinalIgnoreCase) >= 0) return $"Thang2_Ki{ki}";
            if (columnName.IndexOf("Thang3", StringComparison.OrdinalIgnoreCase) >= 0 ||
                columnName.IndexOf("Tháng 3", StringComparison.OrdinalIgnoreCase) >= 0) return $"Thang3_Ki{ki}";
            if (columnName.IndexOf("GiuaKi", StringComparison.OrdinalIgnoreCase) >= 0 ||
                columnName.IndexOf("Giữa", StringComparison.OrdinalIgnoreCase) >= 0) return $"GiuaKi{ki}";
            if (columnName.IndexOf("CuoiKi", StringComparison.OrdinalIgnoreCase) >= 0 ||
                columnName.IndexOf("Cuối", StringComparison.OrdinalIgnoreCase) >= 0) return $"CuoiKi{ki}";
            // For NhanXet / GhiChu, we may store as Nhận xét/Ghi chú — we won't update those with this mapping
            if (columnName.IndexOf("NhanXet", StringComparison.OrdinalIgnoreCase) >= 0 ||
                columnName.IndexOf("Nhận", StringComparison.OrdinalIgnoreCase) >= 0) return "NhanXet"; // custom handling if needed
            if (columnName.IndexOf("GhiChu", StringComparison.OrdinalIgnoreCase) >= 0 ||
                columnName.IndexOf("Ghi chú", StringComparison.OrdinalIgnoreCase) >= 0) return "GhiChu";
            return null;
        }
        #endregion

        #region Học sinh (simple edit/save)
        private void ShowHocSinh()
        {
            panelContent.Controls.Clear();

            DataTable dt = DatabaseHelper.GetHocSinhByLop(_maLop);
            dgvHocSinh.DataSource = null;
            dgvHocSinh.Columns.Clear();
            dgvHocSinh.AutoGenerateColumns = true;
            dgvHocSinh.DataSource = dt;

            // attach row-validated to save changes
            dgvHocSinh.RowValidated -= DgvHocSinh_RowValidated;
            dgvHocSinh.RowValidated += DgvHocSinh_RowValidated;

            panelContent.Controls.Add(dgvHocSinh);
        }

        private void DgvHocSinh_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            // save row to DB
            try
            {
                if (dgvHocSinh.CurrentRow == null) return;
                var row = dgvHocSinh.CurrentRow;
                string maHS = row.Cells["MaHS"]?.Value?.ToString();
                if (string.IsNullOrEmpty(maHS)) return;

                // collect editable fields (example: HoTen, GioiTinh, NgaySinh, DiaChi)
                string hoTen = row.Cells["HoTen"]?.Value?.ToString();
                string gioiTinh = row.Cells["GioiTinh"]?.Value?.ToString();
                DateTime? ngaySinh = null;
                if (row.Cells["NgaySinh"]?.Value != null && row.Cells["NgaySinh"].Value != DBNull.Value)
                    ngaySinh = Convert.ToDateTime(row.Cells["NgaySinh"].Value);
                string diaChi = row.Cells["DiaChi"]?.Value?.ToString();

                DatabaseHelper.UpdateHocSinh(maHS, hoTen, gioiTinh, ngaySinh, diaChi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu học sinh: " + ex.Message);
            }
        }
        #endregion

        #region Helpers: Save all / cleanup
        private void SaveAllCurrentEdits()
        {
            // If attendance grid present, commit edits and save visible rows
            try
            {
                if (dgvDiemDanh != null && dgvDiemDanh.DataSource != null)
                {
                    dgvDiemDanh.EndEdit();
                    // iterate rows -> save
                    foreach (DataGridViewRow r in dgvDiemDanh.Rows)
                    {
                        SaveDiemDanhRow(r);
                    }
                }
            }
            catch { }

            // For results grids
            try
            {
                if (dgvKi1 != null && dgvKi1.DataSource != null)
                {
                    dgvKi1.EndEdit();
                    for (int i = 0; i < dgvKi1.Rows.Count; i++)
                    {
                        // attempt save for each edited cell by scanning relevant numeric columns
                        foreach (DataGridViewColumn col in dgvKi1.Columns)
                        {
                            // skip MaHS/HoTen
                            if (col.Name == "MaHS" || col.Name == "HoTen") continue;
                            SaveKetQuaRow(dgvKi1, i, col.Index);
                        }
                    }
                }
            }
            catch { }

            try
            {
                if (dgvKi2 != null && dgvKi2.DataSource != null)
                {
                    dgvKi2.EndEdit();
                    for (int i = 0; i < dgvKi2.Rows.Count; i++)
                    {
                        foreach (DataGridViewColumn col in dgvKi2.Columns)
                        {
                            if (col.Name == "MaHS" || col.Name == "HoTen") continue;
                            SaveKetQuaRow(dgvKi2, i, col.Index);
                        }
                    }
                }
            }
            catch { }
        }

        // stop camera when disposing control or switching pages
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopQrCamera();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
