using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics; // Thêm

namespace N6
{
    /// <summary>
    /// UserControl hiển thị và quản lý Thời khóa biểu cho giáo viên.
    /// Hỗ trợ xem theo tuần, tô màu, thêm/xóa ghi chú và import.
    /// </summary>
    public partial class UC_ThoiKhoaBieu : UserControl
    {
        #region Fields (Biến thành viên)

        private string maGV;
        private DateTime currentMonday; // Luôn là ngày Thứ 2 của tuần đang xem

        // Cache màu nền của các ô
        private Dictionary<Point, Color> cellColors = new Dictionary<Point, Color>();
        private Point selectedCellForContextMenu; // Tọa độ ô được click chuột phải

        private Timer refreshTimer; // Timer để tự động làm mới TKB

        #endregion

        #region Constructor & Load

        /// <summary>
        /// Khởi tạo UserControl Thời khóa biểu cho một giáo viên cụ thể.
        /// </summary>
        /// <param name="maGVien">Mã giáo viên (lấy từ form cha).</param>
        public UC_ThoiKhoaBieu(string maGVien)
        {
            InitializeComponent();
            maGV = maGVien;
            InitGrid();

            SetCurrentWeek(DateTime.Today);

            //// Gán các sự kiện (sẽ được gỡ trong Dispose)
            dgvTKB.CellPainting += DgvTKB_CellPainting;
            dgvTKB.CellMouseDown += dgvTKB_CellMouseDown;

            //// Giả định các control này tồn tại trong file Designer
            //btnPrevWeek.Click += btnPrevWeek_Click;
            //btnNextWeek.Click += btnNextWeek_Click;
            //btnImportTKB.Click += btnImportTKB_Click;
            btnXoaTKB.Click += btnXoaTKB_Click;
            //doiMauMenuItem.Click += doiMauMenuItem_Click;
            //xoaGhiChuMenuItem.Click += xoaGhiChuMenuItem_Click;
            //xoaTKBMenuItem.Click += xoaTKBMenuItem_Click;

            // Đăng ký nhận thông báo nếu TKB thay đổi ở nơi khác
            DatabaseHelper.TimetableChanged += OnThoiKhoaBieuChanged;

            LoadThoiKhoaBieu();
            InitializeTimer();
        }

        /// <summary>
        /// Khởi tạo và khởi động Timer để tự động làm mới TKB (mỗi 60 giây).
        /// </summary>
        private void InitializeTimer()
        {
            refreshTimer = new Timer { Interval = 60000 };
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        #endregion

        #region Grid & UI Initialization

        /// <summary>
        /// Cài đặt cấu trúc cột, hàng, và header ban đầu cho DataGridView TKB.
        /// </summary>
        private void InitGrid()
        {
            dgvTKB.Columns.Clear();
            dgvTKB.Rows.Clear();
            dgvTKB.ColumnCount = 7; // Thứ 2 -> Chủ nhật

            for (int i = 0; i < 7; i++)
            {
                dgvTKB.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvTKB.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True; // Cho phép xuống dòng
            }

            dgvTKB.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvTKB.RowTemplate.MinimumHeight = 60; // Chiều cao tối thiểu

            dgvTKB.RowCount = 10; // 10 tiết
            for (int i = 0; i < 10; i++)
            {
                dgvTKB.Rows[i].HeaderCell.Value = "Tiết " + (i + 1);
            }
            dgvTKB.RowHeadersWidth = 100; // Độ rộng cột Tiết

            dgvTKB.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvTKB.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        }

        /// <summary>
        /// Cập nhật tiêu đề các cột (Thứ 2 [dd/MM]...) dựa trên tuần hiện tại.
        /// </summary>
        private void UpdateColumnHeaders()
        {
            string[] thu = { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật" };
            for (int i = 0; i < 7; i++)
            {
                dgvTKB.Columns[i].HeaderText = $"{thu[i]}\n{currentMonday.AddDays(i):dd/MM}"; // Hiển thị Thứ và Ngày/Tháng
            }
        }

        #endregion

        #region Data Loading & Week Navigation

        /// <summary>
        /// Đặt ngày thứ 2 đầu tuần hiện tại dựa trên một ngày bất kỳ.
        /// </summary>
        private void SetCurrentWeek(DateTime dateInWeek)
        {
            int diff = (int)DayOfWeek.Monday - (int)dateInWeek.DayOfWeek;
            if (diff > 0) diff -= 7;
            currentMonday = dateInWeek.AddDays(diff).Date;
        }

        /// <summary>
        /// Tải (hoặc tải lại) toàn bộ dữ liệu TKB cho tuần hiện tại từ CSDL và hiển thị lên lưới.
        /// </summary>
        private void LoadThoiKhoaBieu()
        {
            UpdateColumnHeaders();

            // Xóa dữ liệu cũ trên grid
            foreach (DataGridViewRow row in dgvTKB.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Value = ""; // Xóa text
                    cell.ToolTipText = ""; // Xóa tooltip
                }
            }
            cellColors.Clear(); // Xóa bộ nhớ màu cũ

            DateTime weekEnd = currentMonday.AddDays(6);
            lblWeek.Text = $"Thời khóa biểu: {currentMonday:dd/MM} - {weekEnd:dd/MM/yyyy}";

            // Lấy TKB tuần hiện tại từ DB
            DataTable dt = DatabaseHelper.GetTimetableByTeacher(maGV, currentMonday);

            // Điền dữ liệu mới vào grid
            foreach (DataRow r in dt.Rows)
            {
                try
                {
                    DateTime ngay = Convert.ToDateTime(r["Ngay"]);
                    int tiet = Convert.ToInt32(r["Tiet"]) - 1; // Index bắt đầu từ 0
                    string mon = r["TenMon"].ToString();
                    string lop = r["TenLop"].ToString();
                    string ghichu = r["GhiChu"].ToString();
                    string mauSac = r["MauSac"].ToString(); // Mã màu hex (#RRGGBB)

                    int col = (int)ngay.DayOfWeek - (int)DayOfWeek.Monday;
                    if (col < 0) col = 6; // Chủ nhật (0) -> 6

                    if (tiet >= 0 && tiet < dgvTKB.RowCount && col >= 0 && col < dgvTKB.ColumnCount)
                    {
                        Point cellPosition = new Point(col, tiet); // Tọa độ ô

                        // Tạo chuỗi hiển thị
                        string displayValue = !string.IsNullOrEmpty(mon) ? $"{mon} - {lop}" : "";
                        if (!string.IsNullOrEmpty(ghichu))
                        {
                            displayValue += (string.IsNullOrEmpty(displayValue) ? "" : "\n") + $"({ghichu})";
                        }

                        dgvTKB[col, tiet].Value = displayValue;
                        dgvTKB[col, tiet].ToolTipText = displayValue;

                        // Lưu màu nền
                        if (!string.IsNullOrEmpty(mauSac))
                        {
                            cellColors[cellPosition] = ColorTranslator.FromHtml(mauSac);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Lỗi khi tải 1 tiết TKB: {ex.Message}");
                }
            }
            dgvTKB.Invalidate(); // Vẽ lại grid để hiển thị màu
        }

        /// <summary>
        /// Xử lý sự kiện khi TKB thay đổi (được gọi từ nơi khác qua event).
        /// </summary>
        private void OnThoiKhoaBieuChanged(object sender, EventArgs e)
        {
            LoadThoiKhoaBieu();
        }

        /// <summary>
        /// Xử lý sự kiện Tick của Timer, tự động tải lại TKB.
        /// </summary>
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadThoiKhoaBieu();
        }

        #endregion

        #region Event Handlers (Buttons & Menu)

        /// <summary>
        /// Tải TKB của tuần trước.
        /// </summary>
        private void btnPrevWeek_Click(object sender, EventArgs e)
        {
            currentMonday = currentMonday.AddDays(-7); // Lùi lại 1 tuần
            LoadThoiKhoaBieu();
        }

        /// <summary>
        /// Tải TKB của tuần sau.
        /// </summary>
        private void btnNextWeek_Click(object sender, EventArgs e)
        {
            currentMonday = currentMonday.AddDays(7); // Tiến tới 1 tuần
            LoadThoiKhoaBieu();
        }

        /// <summary>
        /// Mở form Import TKB từ Excel.
        /// </summary>
        private void btnImportTKB_Click(object sender, EventArgs e)
        {
            string tenGV = DatabaseHelper.GetTeacherNameById(this.maGV);

            using (var importForm = new frmImportExcel(frmImportExcel.ImportType.ThoiKhoaBieu, this.maGV, tenGV))
            {
                if (importForm.ShowDialog() == DialogResult.OK)
                {
                    // Nếu import thành công, chuyển đến tuần đầu tiên có dữ liệu import
                    if (importForm.FirstImportedDate.HasValue)
                    {
                        SetCurrentWeek(importForm.FirstImportedDate.Value);
                    }
                    LoadThoiKhoaBieu(); // Tải lại TKB
                }
            }
        }

        /// <summary>
        /// Xóa toàn bộ TKB của tuần hiện tại.
        /// </summary>
        private void btnXoaTKB_Click(object sender, EventArgs e)
        {
            DateTime weekEnd = currentMonday.AddDays(6);
            string tuanHienTai = $"{currentMonday:dd/MM} - {weekEnd:dd/MM/yyyy}";

            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa TOÀN BỘ thời khóa biểu của tuần này không?\n({tuanHienTai})",
                "Xác nhận xóa TKB",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteTimetableByWeek(maGV, currentMonday); // Gọi hàm xóa theo tuần
                    LoadThoiKhoaBieu(); // Tải lại TKB (bây giờ sẽ trống)
                    MessageBox.Show("Đã xóa TKB tuần thành công.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa TKB: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện click menu "Đổi màu nền".
        /// </summary>
        private void doiMauMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                // Lấy màu hiện tại (nếu có)
                colorDialog.Color = cellColors.ContainsKey(selectedCellForContextMenu) ? cellColors[selectedCellForContextMenu] : Color.White;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    cellColors[selectedCellForContextMenu] = colorDialog.Color;
                    DateTime cellDate = currentMonday.AddDays(selectedCellForContextMenu.X);
                    int tiet = selectedCellForContextMenu.Y + 1;
                    string colorHex = ColorTranslator.ToHtml(colorDialog.Color);
                    DatabaseHelper.UpdateCellColor(maGV, cellDate, tiet, colorHex); // Lưu màu vào DB
                    dgvTKB.Invalidate(); // Vẽ lại ô
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện click menu "Xóa ghi chú".
        /// </summary>
        private void xoaGhiChuMenuItem_Click(object sender, EventArgs e)
        {
            DateTime cellDate = currentMonday.AddDays(selectedCellForContextMenu.X);
            int tiet = selectedCellForContextMenu.Y + 1;

            DatabaseHelper.DeleteTimetableNote(maGV, cellDate, tiet);
            LoadThoiKhoaBieu();
        }

        /// <summary>
        /// Xử lý sự kiện click menu "Xóa TKB tiết này".
        /// </summary>
        private void xoaTKBMenuItem_Click(object sender, EventArgs e)
        {
            DateTime cellDate = currentMonday.AddDays(selectedCellForContextMenu.X);
            int tiet = selectedCellForContextMenu.Y + 1;
            string cellValue = dgvTKB[selectedCellForContextMenu.X, selectedCellForContextMenu.Y].Value?.ToString() ?? "Tiết trống";

            string tkbInfo = cellValue.Split('\n')[0];

            var confirm = MessageBox.Show(
               $"Bạn có chắc chắn muốn xóa TKB này không?\n\nNgày: {cellDate:dd/MM/yyyy}\nTiết: {tiet}\nNội dung: {tkbInfo}",
               "Xác nhận xóa TKB",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteTimetableEntry(maGV, cellDate, tiet);
                    LoadThoiKhoaBieu();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa TKB: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Grid Event Handlers (Painting, Clicks)

        /// <summary>
        /// Xử lý sự kiện nhấn chuột phải vào ô, chuẩn bị cho ContextMenu.
        /// </summary>
        private void dgvTKB_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dgvTKB.CurrentCell = dgvTKB[e.ColumnIndex, e.RowIndex]; // Chọn ô
                selectedCellForContextMenu = new Point(e.ColumnIndex, e.RowIndex);
                string cellValue = dgvTKB.CurrentCell.Value as string ?? string.Empty;

                bool coTKB = !string.IsNullOrEmpty(cellValue);
                bool coGhiChu = cellValue.Contains("(");

                doiMauMenuItem.Enabled = coTKB;
                xoaGhiChuMenuItem.Enabled = coGhiChu;
                xoaTKBMenuItem.Enabled = coTKB;
            }
        }

        /// <summary>
        /// Tùy chỉnh việc vẽ ô (CellPainting) để tô màu nền đã lưu.
        /// </summary>
        private void DgvTKB_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return; // Bỏ qua header

            e.PaintBackground(e.ClipBounds, true); // 1. Vẽ nền mặc định

            Point cellPosition = new Point(e.ColumnIndex, e.RowIndex);
            Color cellColor = Color.White; // Màu mặc định

            // 2. Lấy màu tùy chỉnh (nếu có)
            if (cellColors.TryGetValue(cellPosition, out Color customColor) && customColor != Color.White)
            {
                cellColor = customColor;
                using (Brush backBrush = new SolidBrush(cellColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.CellBounds); // Tô màu nền
                }
            }

            // 3. Vẽ lại đường viền ô
            e.Graphics.DrawRectangle(Pens.LightGray, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width - 1, e.CellBounds.Height - 1);

            // 4. Vẽ text
            if (e.Value is string cellValue && !string.IsNullOrEmpty(cellValue))
            {
                // Xác định màu chữ (trắng nếu nền tối, đen nếu nền sáng)
                bool isDark = (cellColor != Color.White && cellColor.GetBrightness() < 0.6);
                Color fontColor = isDark ? Color.White : Color.Black;

                Rectangle textBounds = e.CellBounds;
                textBounds.Inflate(-10, -10); // Thụt lề text
                TextRenderer.DrawText(e.Graphics, cellValue, e.CellStyle.Font, textBounds, fontColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak);
            }
            e.Handled = true; // Báo rằng đã tự vẽ xong
        }

        /// <summary>
        /// Xử lý sự kiện double-click vào ô (để sửa ghi chú).
        /// </summary>
        private void DgvTKB_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return; // Bỏ qua header

            string currentValue = dgvTKB[e.ColumnIndex, e.RowIndex].Value?.ToString() ?? "";

            DateTime cellDate = currentMonday.AddDays(e.ColumnIndex);
            int tiet = e.RowIndex + 1;

            // Lấy ghi chú hiện tại (nếu có)
            string currentNote = "";
            int noteStartIndex = currentValue.IndexOf('(');
            if (noteStartIndex != -1)
            {
                currentNote = currentValue.Substring(noteStartIndex + 1).TrimEnd(')');
            }

            // Hiển thị form nhập ghi chú đơn giản
            using (Form inputForm = new Form() { Width = 400, Height = 180, Text = "Ghi chú", StartPosition = FormStartPosition.CenterParent, Font = this.Font })
            {
                Label lbl = new Label() { Text = "Nhập ghi chú:", Left = 10, Top = 20, Width = 360 };
                TextBox txt = new TextBox() { Left = 10, Top = 50, Width = 360, Text = currentNote };
                Button ok = new Button() { Text = "OK", Left = 220, Width = 70, Top = 90, DialogResult = DialogResult.OK };
                Button cancel = new Button() { Text = "Hủy", Left = 300, Width = 70, Top = 90, DialogResult = DialogResult.Cancel };
                inputForm.Controls.AddRange(new Control[] { lbl, txt, ok, cancel });
                inputForm.AcceptButton = ok;
                inputForm.CancelButton = cancel;

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    string note = txt.Text.Trim();
                    DatabaseHelper.UpsertTimetableNote(maGV, cellDate, tiet, note);
                    LoadThoiKhoaBieu(); // Tải lại TKB
                }
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Hủy đăng ký sự kiện của DatabaseHelper
                DatabaseHelper.TimetableChanged -= OnThoiKhoaBieuChanged;

                // Dừng và hủy Timer
                if (refreshTimer != null)
                {
                    refreshTimer.Stop();
                    refreshTimer.Tick -= RefreshTimer_Tick;
                    refreshTimer.Dispose();
                    refreshTimer = null;
                }

                // Gỡ bỏ sự kiện của DataGridView (giả sử là Designer field)
                if (dgvTKB != null)
                {
                    dgvTKB.CellPainting -= DgvTKB_CellPainting;
                    dgvTKB.CellMouseDown -= dgvTKB_CellMouseDown;
                    dgvTKB.CellDoubleClick -= DgvTKB_CellDoubleClick;
                }

                // Gỡ bỏ sự kiện của các nút (giả sử là Designer fields)
                if (btnPrevWeek != null) btnPrevWeek.Click -= btnPrevWeek_Click;
                if (btnNextWeek != null) btnNextWeek.Click -= btnNextWeek_Click;
                if (btnImportTKB != null) btnImportTKB.Click -= btnImportTKB_Click;
                if (btnXoaTKB != null) btnXoaTKB.Click -= btnXoaTKB_Click;

                // Gỡ bỏ sự kiện của ContextMenu (giả sử là Designer fields)
                if (doiMauMenuItem != null) doiMauMenuItem.Click -= doiMauMenuItem_Click;
                if (xoaGhiChuMenuItem != null) xoaGhiChuMenuItem.Click -= xoaGhiChuMenuItem_Click;
                if (xoaTKBMenuItem != null) xoaTKBMenuItem.Click -= xoaTKBMenuItem_Click;

                // Dọn dẹp components (của Designer)
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}