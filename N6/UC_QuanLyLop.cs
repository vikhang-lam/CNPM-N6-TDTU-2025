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

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Timer qrTimer;

        // Giữ delegate để tránh GC dọn (QR)
        private NewFrameEventHandler _frameHandler;

        // Đánh dấu các DataGridView đã style để không đăng ký event nhiều lần
        private readonly HashSet<DataGridView> _styledGrids = new HashSet<DataGridView>();

        public UC_QuanLyLop(string username)
        {
            InitializeComponent();
            _username = username;
            _maLop = DatabaseHelper.GetLopByTeacher(username);

            // gắn event handler chuẩn
            btnDiemDanh.Click += btnDiemDanh_Click;
            btnQR.Click += btnQR_Click;
            btnKetQua.Click += btnKetQua_Click;
            btnHocSinh.Click += btnHocSinh_Click;

            // Áp style — an toàn khi gọi nhiều lần
            ApplyGridStyle(dgvDiemDanh);
            ApplyGridStyle(dgvKetQua);
            ApplyGridStyle(dgvHocSinh);

            LoadHocSinh();
            ShowDiemDanh();
        }

        /// <summary>
        /// Áp style giống bootstrap, KHÔNG dùng lambda cho event để tránh lỗi.
        /// Idempotent: nếu đã styled rồi thì không làm lại (không đăng ký event thêm).
        /// </summary>
        private void ApplyGridStyle(DataGridView dgv)
        {
            if (dgv == null) return;

            // nếu đã áp style -> thoát (tránh đăng ký handler nhiều lần)
            if (_styledGrids.Contains(dgv)) return;

            // Layout / sizing
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.AllowUserToResizeRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToAddRows = false;

            // Visual
            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;

            // Header
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 123, 255);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 45;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Cells
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.Padding = new Padding(6);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Alternating (striped)
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            // Grid lines
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(230, 230, 230);

            dgv.RowTemplate.Height = 40;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;

            // Registered named handlers (an toàn, có thể remove nếu cần)
            dgv.CellMouseEnter += DataGridView_CellMouseEnter;
            dgv.CellMouseLeave += DataGridView_CellMouseLeave;
            dgv.Paint += DataGridView_Paint;

            _styledGrids.Add(dgv);
        }

        // ===== Named handlers for DataGridView hover & paint =====
        private void DataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null) return;
            if (e.RowIndex < 0 || e.RowIndex >= dgv.Rows.Count) return;

            try
            {
                dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 245, 255);
            }
            catch { /* swallow safely */ }
        }

        private void DataGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null) return;
            if (e.RowIndex < 0 || e.RowIndex >= dgv.Rows.Count) return;

            try
            {
                bool isAlt = (e.RowIndex % 2) == 1;
                dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = isAlt ? Color.FromArgb(248, 249, 250) : Color.White;
            }
            catch { /* swallow safely */ }
        }

        private void DataGridView_Paint(object sender, PaintEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null) return;

            try
            {
                Rectangle rect = dgv.ClientRectangle;
                // vẽ border nhẹ giả card
                ControlPaint.DrawBorder(e.Graphics, rect,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            }
            catch { }
        }

        // ===== Load dữ liệu =====
        private void LoadHocSinh()
        {
            if (string.IsNullOrEmpty(_maLop))
            {
                MessageBox.Show("Giáo viên chưa có lớp được gán!");
                return;
            }

            dgvDiemDanh.DataSource = DatabaseHelper.GetHocSinhByLop(_maLop);
            dgvKetQua.DataSource = DatabaseHelper.GetHocSinhByLop(_maLop);
            dgvHocSinh.DataSource = DatabaseHelper.GetHocSinhByLop(_maLop);

            if (!dgvDiemDanh.Columns.Contains("TrangThai"))
            {
                var col = new DataGridViewComboBoxColumn
                {
                    Name = "TrangThai",
                    HeaderText = "Trạng thái"
                };
                col.Items.AddRange("Có mặt", "Vắng");
                dgvDiemDanh.Columns.Add(col);
            }
        }

        // ==== Sidebar Handlers ====
        private void btnDiemDanh_Click(object sender, EventArgs e) => ShowDiemDanh();
        private void btnQR_Click(object sender, EventArgs e) => ShowQR();
        private void btnKetQua_Click(object sender, EventArgs e) => ShowKetQua();
        private void btnHocSinh_Click(object sender, EventArgs e) => ShowHocSinh();

        // ==== Điểm danh ====
        private void ShowDiemDanh()
        {
            panelContent.Controls.Clear();
            Panel p = new Panel { Dock = DockStyle.Fill };

            dgvDiemDanh.Dock = DockStyle.Top;
            dgvDiemDanh.Height = 450;

            btnLuuDiemDanh.Text = "💾 Lưu điểm danh";
            btnLuuDiemDanh.Dock = DockStyle.Bottom;
            btnLuuDiemDanh.BackColor = Color.SeaGreen;
            btnLuuDiemDanh.ForeColor = Color.White;
            btnLuuDiemDanh.FlatStyle = FlatStyle.Flat;
            btnLuuDiemDanh.Height = 45;
            btnLuuDiemDanh.Click -= btnLuuDiemDanh_Click;
            btnLuuDiemDanh.Click += btnLuuDiemDanh_Click;

            p.Controls.Add(dgvDiemDanh);
            p.Controls.Add(btnLuuDiemDanh);
            panelContent.Controls.Add(p);
        }

        private void btnLuuDiemDanh_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvDiemDanh.Rows)
            {
                if (!row.IsNewRow)
                {
                    string maHS = row.Cells["MaHS"].Value.ToString();
                    string trangThai = row.Cells["TrangThai"].Value.ToString();
                    DatabaseHelper.LuuDiemDanh(maHS, trangThai);
                }
            }
            MessageBox.Show("✅ Đã lưu điểm danh!");
        }

        // ==== QR ====
        private void ShowQR()
        {
            panelContent.Controls.Clear();
            Panel p = new Panel { Dock = DockStyle.Fill };

            pictureQR.Dock = DockStyle.Top;
            pictureQR.Height = 400;
            pictureQR.SizeMode = PictureBoxSizeMode.Zoom;

            FlowLayoutPanel fl = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 80 };
            btnStartQR.Text = "▶ Bắt đầu quét";
            btnStopQR.Text = "⏹ Dừng quét";
            lblQRStatus.Text = "Chưa quét...";

            btnStartQR.Click -= btnStartQR_Click;
            btnStopQR.Click -= btnStopQR_Click;
            btnStartQR.Click += btnStartQR_Click;
            btnStopQR.Click += btnStopQR_Click;

            fl.Controls.Add(btnStartQR);
            fl.Controls.Add(btnStopQR);
            fl.Controls.Add(lblQRStatus);

            p.Controls.Add(pictureQR);
            p.Controls.Add(fl);
            panelContent.Controls.Add(p);
        }

        private void btnStartQR_Click(object sender, EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

                // giữ delegate, tránh bị GC
                _frameHandler = new NewFrameEventHandler(VideoSource_NewFrame);
                videoSource.NewFrame += _frameHandler;

                videoSource.Start();

                qrTimer = new Timer { Interval = 1000 };
                qrTimer.Tick += QrTimer_Tick;
                qrTimer.Start();
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs ev)
        {
            try
            {
                Bitmap frame = (Bitmap)ev.Frame.Clone();
                if (pictureQR.InvokeRequired)
                {
                    pictureQR.Invoke(new Action(() =>
                    {
                        pictureQR.Image?.Dispose();
                        pictureQR.Image = frame;
                    }));
                }
                else
                {
                    pictureQR.Image?.Dispose();
                    pictureQR.Image = frame;
                }
            }
            catch { }
        }

        private void QrTimer_Tick(object sender, EventArgs e)
        {
            if (pictureQR.Image != null)
            {
                BarcodeReader reader = new BarcodeReader();
                var result = reader.Decode((Bitmap)pictureQR.Image);
                if (result != null)
                {
                    string maHS = result.Text;
                    DatabaseHelper.LuuDiemDanh(maHS, "Có mặt");
                    lblQRStatus.Text = $"✅ Điểm danh thành công: {maHS}";
                    lblQRStatus.ForeColor = Color.Green;
                }
            }
        }

        private void btnStopQR_Click(object sender, EventArgs e)
        {
            qrTimer?.Stop();
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.NewFrame -= _frameHandler; // bỏ đăng ký
                _frameHandler = null;
                videoSource = null;
            }
            pictureQR.Image?.Dispose();
            pictureQR.Image = null;
        }

        // ==== Kết quả ====
        private void ShowKetQua()
        {
            panelContent.Controls.Clear();
            Panel p = new Panel { Dock = DockStyle.Fill };

            dgvKetQua.Dock = DockStyle.Top;
            dgvKetQua.Height = 450;

            btnLuuKQ.Text = "💾 Lưu kết quả";
            btnLuuKQ.Dock = DockStyle.Bottom;
            btnLuuKQ.BackColor = Color.DodgerBlue;
            btnLuuKQ.ForeColor = Color.White;
            btnLuuKQ.FlatStyle = FlatStyle.Flat;
            btnLuuKQ.Click -= btnLuuKQ_Click;
            btnLuuKQ.Click += btnLuuKQ_Click;

            p.Controls.Add(dgvKetQua);
            p.Controls.Add(btnLuuKQ);
            panelContent.Controls.Add(p);
        }

        private void btnLuuKQ_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvKetQua.Rows)
            {
                if (!row.IsNewRow && row.Cells["Diem"].Value != null)
                {
                    string maHS = row.Cells["MaHS"].Value.ToString();
                    double diem = double.Parse(row.Cells["Diem"].Value.ToString());
                    DatabaseHelper.LuuKetQuaHocTap(maHS, "TOAN", diem);
                }
            }
            MessageBox.Show("✅ Đã lưu kết quả!");
        }

        // ==== Hồ sơ ====
        private void ShowHocSinh()
        {
            panelContent.Controls.Clear();
            Panel p = new Panel { Dock = DockStyle.Fill };

            dgvHocSinh.Dock = DockStyle.Top;
            dgvHocSinh.Height = 450;

            btnXemHoSo.Text = "👤 Xem hồ sơ";
            btnXemHoSo.Dock = DockStyle.Bottom;
            btnXemHoSo.BackColor = Color.MediumPurple;
            btnXemHoSo.ForeColor = Color.White;
            btnXemHoSo.FlatStyle = FlatStyle.Flat;
            btnXemHoSo.Click -= btnXemHoSo_Click;
            btnXemHoSo.Click += btnXemHoSo_Click;

            p.Controls.Add(dgvHocSinh);
            p.Controls.Add(btnXemHoSo);
            panelContent.Controls.Add(p);
        }

        private void btnXemHoSo_Click(object sender, EventArgs e)
        {
            if (dgvHocSinh.SelectedRows.Count > 0)
            {
                string maHS = dgvHocSinh.SelectedRows[0].Cells["MaHS"].Value.ToString();
                DataRow hs = DatabaseHelper.GetHocSinhProfile(maHS);
                if (hs != null)
                {
                    MessageBox.Show(
                        $"📌 Họ tên: {hs["HoTen"]}\n👤 Giới tính: {hs["GioiTinh"]}\n📅 Ngày sinh: {hs["NgaySinh"]}\n🏠 Địa chỉ: {hs["DiaChi"]}",
                        "Hồ sơ học sinh",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
