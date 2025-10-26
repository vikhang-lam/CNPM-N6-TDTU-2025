using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_ThoiKhoaBieu : UserControl
    {
        private string maGV;
        private DateTime currentMonday;

        private Dictionary<Point, Color> cellColors = new Dictionary<Point, Color>();
        private Point selectedCellForContextMenu; // Đổi tên biến để rõ ràng hơn

        private Timer refreshTimer;

        public UC_ThoiKhoaBieu(string maGVien)
        {
            InitializeComponent();
            maGV = maGVien;
            InitGrid();

            SetCurrentWeek(DateTime.Today);

            dgvTKB.CellPainting += DgvTKB_CellPainting;

            LoadThoiKhoaBieu();

            InitializeTimer();
            DatabaseHelper.ThoiKhoaBieuChanged += OnThoiKhoaBieuChanged;

            // Gắn sự kiện cho nút Xóa TKB Tuần (nút này đã được tạo trong Designer)
            if (this.Controls.Find("btnXoaTKB", true).FirstOrDefault() is Button btnXoa)
            {
                btnXoa.Click += new System.EventHandler(this.btnXoaTKB_Click);
            }
        }

        private void SetCurrentWeek(DateTime dateInWeek)
        {
            int diff = (int)DayOfWeek.Monday - (int)dateInWeek.DayOfWeek;
            if (diff > 0) diff -= 7;
            currentMonday = dateInWeek.AddDays(diff).Date;
        }

        private void OnThoiKhoaBieuChanged(object sender, EventArgs e)
        {
            // Cân nhắc: Chỉ load lại nếu tuần hiện tại bị ảnh hưởng?
            // Hiện tại cứ load lại cho đơn giản.
            LoadThoiKhoaBieu();
        }

        private void InitializeTimer()
        {
            refreshTimer = new Timer { Interval = 60000 }; // Làm mới mỗi phút
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadThoiKhoaBieu(); // Tải lại TKB định kỳ
        }

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

            dgvTKB.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // Tự chỉnh chiều cao dòng
            dgvTKB.RowTemplate.MinimumHeight = 60; // Chiều cao tối thiểu

            dgvTKB.RowCount = 10; // 10 tiết
            for (int i = 0; i < 10; i++)
            {
                dgvTKB.Rows[i].HeaderCell.Value = "Tiết " + (i + 1);
            }
            dgvTKB.RowHeadersWidth = 100; // Độ rộng cột Tiết

            dgvTKB.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvTKB.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize; // Tự chỉnh chiều cao header
        }

        private void UpdateColumnHeaders()
        {
            string[] thu = { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật" };
            for (int i = 0; i < 7; i++)
            {
                dgvTKB.Columns[i].HeaderText = $"{thu[i]}\n{currentMonday.AddDays(i):dd/MM}"; // Hiển thị Thứ và Ngày/Tháng
            }
        }

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

            DataTable dt = DatabaseHelper.GetTKBByGV(maGV, currentMonday); // Lấy TKB tuần hiện tại từ DB

            // Điền dữ liệu mới vào grid
            foreach (DataRow r in dt.Rows)
            {
                DateTime ngay = Convert.ToDateTime(r["Ngay"]);
                int tiet = Convert.ToInt32(r["Tiet"]) - 1; // Index bắt đầu từ 0
                string mon = r["TenMon"].ToString();
                string lop = r["TenLop"].ToString();
                string ghichu = r["GhiChu"].ToString();
                string mauSac = r["MauSac"].ToString(); // Mã màu hex (#RRGGBB)

                // Tính toán cột dựa trên ngày trong tuần (Thứ 2 = 0, CN = 6)
                int col = (int)ngay.DayOfWeek - (int)DayOfWeek.Monday;
                if (col < 0) col = 6; // Chủ nhật là 0, chuyển thành 6

                // Kiểm tra xem tiết và cột có hợp lệ không
                if (tiet >= 0 && tiet < dgvTKB.RowCount && col >= 0 && col < dgvTKB.ColumnCount)
                {
                    Point cellPosition = new Point(col, tiet); // Tọa độ ô

                    // Tạo chuỗi hiển thị (Môn - Lớp \n (Ghi chú))
                    string displayValue = !string.IsNullOrEmpty(mon) ? $"{mon} - {lop}" : "";
                    if (!string.IsNullOrEmpty(ghichu))
                    {
                        displayValue += (string.IsNullOrEmpty(displayValue) ? "" : "\n") + $"({ghichu})";
                    }

                    dgvTKB[col, tiet].Value = displayValue;
                    dgvTKB[col, tiet].ToolTipText = displayValue; // Tooltip khi hover

                    // Lưu màu nền vào dictionary
                    if (!string.IsNullOrEmpty(mauSac))
                    {
                        try { cellColors[cellPosition] = ColorTranslator.FromHtml(mauSac); }
                        catch { /* Bỏ qua nếu mã màu không hợp lệ */ }
                    }
                    // Nếu không có màu nhưng có ghi chú, dùng màu xám nhạt (đã bỏ logic này vì SP tự gán màu mặc định)
                    // else if (!string.IsNullOrEmpty(ghichu)) { cellColors[cellPosition] = Color.FromArgb(230, 230, 230); }
                }
            }
            dgvTKB.Invalidate(); // Vẽ lại grid để hiển thị màu
        }

        // Sự kiện click nút Import
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

        // Sự kiện click nút Xóa TKB Tuần
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
                    DatabaseHelper.DeleteTKBByWeek(maGV, currentMonday); // Gọi hàm xóa theo tuần
                    LoadThoiKhoaBieu(); // Tải lại TKB (bây giờ sẽ trống)
                    MessageBox.Show("Đã xóa TKB tuần thành công.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa TKB: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Sự kiện nhấn chuột phải vào ô
        private void dgvTKB_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dgvTKB.CurrentCell = dgvTKB[e.ColumnIndex, e.RowIndex]; // Chọn ô được click
                selectedCellForContextMenu = new Point(e.ColumnIndex, e.RowIndex); // Lưu lại tọa độ ô
                string cellValue = dgvTKB.CurrentCell.Value as string ?? string.Empty;

                // Bật/tắt các menu item dựa trên nội dung ô
                bool coTKB = !string.IsNullOrEmpty(cellValue); // Có TKB nếu ô không rỗng
                bool coGhiChu = cellValue.Contains("(");     // Có ghi chú nếu chứa dấu '('

                doiMauMenuItem.Enabled = coTKB;          // Chỉ đổi màu khi có TKB
                xoaGhiChuMenuItem.Enabled = coGhiChu;      // Chỉ xóa ghi chú khi có ghi chú
                xoaTKBMenuItem.Enabled = coTKB;           // Chỉ xóa TKB khi có TKB
            }
        }

        // Sự kiện click menu "Đổi màu nền"
        private void doiMauMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                // Lấy màu hiện tại (nếu có) để hiển thị trong dialog
                colorDialog.Color = cellColors.ContainsKey(selectedCellForContextMenu) ? cellColors[selectedCellForContextMenu] : Color.White;

                if (colorDialog.ShowDialog() == DialogResult.OK) // Nếu người dùng chọn màu và nhấn OK
                {
                    cellColors[selectedCellForContextMenu] = colorDialog.Color; // Cập nhật màu trong dictionary
                    DateTime cellDate = currentMonday.AddDays(selectedCellForContextMenu.X);
                    int tiet = selectedCellForContextMenu.Y + 1;
                    string colorHex = ColorTranslator.ToHtml(colorDialog.Color); // Chuyển màu thành mã hex
                    DatabaseHelper.UpdateCellColor(maGV, cellDate, tiet, colorHex); // Lưu màu vào DB
                    dgvTKB.Invalidate(); // Vẽ lại ô với màu mới
                }
            }
        }

        // Sự kiện click menu "Xóa ghi chú"
        private void xoaGhiChuMenuItem_Click(object sender, EventArgs e)
        {
            DateTime cellDate = currentMonday.AddDays(selectedCellForContextMenu.X);
            int tiet = selectedCellForContextMenu.Y + 1;

            // Gọi SP xóa ghi chú. SP này chỉ set GhiChu = NULL.
            DatabaseHelper.DeleteGhiChuTKB(maGV, cellDate, tiet);

            // Tải lại TKB. LoadThoiKhoaBieu sẽ đọc lại màu từ DB
            // (là màu gốc hoặc màu mặc định do Import gán)
            LoadThoiKhoaBieu();
        }

        // *** HÀM MỚI: Xử lý sự kiện click cho menu "Xóa TKB tiết này" ***
        private void xoaTKBMenuItem_Click(object sender, EventArgs e)
        {
            DateTime cellDate = currentMonday.AddDays(selectedCellForContextMenu.X);
            int tiet = selectedCellForContextMenu.Y + 1;
            string cellValue = dgvTKB[selectedCellForContextMenu.X, selectedCellForContextMenu.Y].Value?.ToString() ?? "Tiết trống";

            // Tách lấy phần môn học - lớp để hiển thị xác nhận
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
                    DatabaseHelper.DeleteTKBEntry(maGV, cellDate, tiet); // Gọi hàm xóa TKB cụ thể
                    LoadThoiKhoaBieu(); // Tải lại TKB (ô này sẽ trống)
                    // Không cần thông báo thành công cho từng tiết
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa TKB: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        // Sự kiện vẽ ô (để tô màu)
        private void DgvTKB_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return; // Bỏ qua header

            e.PaintBackground(e.ClipBounds, true); // Vẽ nền mặc định

            Point cellPosition = new Point(e.ColumnIndex, e.RowIndex);
            // Nếu có màu đã lưu cho ô này và không phải màu trắng
            if (cellColors.TryGetValue(cellPosition, out Color cellColor) && cellColor != Color.White)
            {
                using (Brush backBrush = new SolidBrush(cellColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.CellBounds); // Tô màu nền
                }
            }

            // Vẽ lại đường viền ô
            e.Graphics.DrawRectangle(Pens.LightGray, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width - 1, e.CellBounds.Height - 1);

            // Vẽ text
            if (e.Value is string cellValue && !string.IsNullOrEmpty(cellValue))
            {
                // Xác định màu chữ (trắng nếu nền tối, đen nếu nền sáng)
                bool isDark = (cellColor != Color.Empty && cellColor.GetBrightness() < 0.6 && cellColor != Color.White);
                Color fontColor = isDark ? Color.White : Color.Black;

                Rectangle textBounds = e.CellBounds;
                textBounds.Inflate(-10, -10); // Thụt lề text
                TextRenderer.DrawText(e.Graphics, cellValue, e.CellStyle.Font, textBounds, fontColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak); // Căn giữa, trên, xuống dòng
            }
            e.Handled = true; // Báo rằng đã tự vẽ xong
        }

        // Sự kiện double-click vào ô (để sửa ghi chú)
        private void DgvTKB_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return; // Bỏ qua header

            // Chỉ cho phép sửa ghi chú nếu ô đó có TKB (không phải ô trống)
            string currentValue = dgvTKB[e.ColumnIndex, e.RowIndex].Value?.ToString() ?? "";
            if (string.IsNullOrEmpty(currentValue))
            {
                MessageBox.Show("Không thể thêm ghi chú cho tiết trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


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
                    // Gọi SP Upsert Ghi chú (SP này chỉ cập nhật cột GhiChu)
                    DatabaseHelper.UpsertGhiChuTKB(maGV, cellDate, tiet, note);
                    LoadThoiKhoaBieu(); // Tải lại TKB để hiển thị ghi chú mới
                }
            }
        }

        // Sự kiện click nút "< Trước"
        private void btnPrevWeek_Click(object sender, EventArgs e)
        {
            currentMonday = currentMonday.AddDays(-7); // Lùi lại 1 tuần
            LoadThoiKhoaBieu();
        }

        // Sự kiện click nút "Sau >"
        private void btnNextWeek_Click(object sender, EventArgs e)
        {
            currentMonday = currentMonday.AddDays(7); // Tiến tới 1 tuần
            LoadThoiKhoaBieu();
        }

        // Dọn dẹp khi UserControl bị hủy
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Hủy đăng ký sự kiện để tránh memory leak
                DatabaseHelper.ThoiKhoaBieuChanged -= OnThoiKhoaBieuChanged;
                if (refreshTimer != null)
                {
                    refreshTimer.Stop();
                    refreshTimer.Dispose();
                    refreshTimer = null;
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}