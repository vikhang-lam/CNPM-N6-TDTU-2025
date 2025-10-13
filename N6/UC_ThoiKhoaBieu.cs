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
        private Point selectedCellForColorChange;

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
        }

        private void SetCurrentWeek(DateTime dateInWeek)
        {
            int diff = (int)DayOfWeek.Monday - (int)dateInWeek.DayOfWeek;
            if (diff > 0) diff -= 7;
            currentMonday = dateInWeek.AddDays(diff).Date;
        }

        private void OnThoiKhoaBieuChanged(object sender, EventArgs e)
        {
            LoadThoiKhoaBieu();
        }

        private void InitializeTimer()
        {
            refreshTimer = new Timer { Interval = 60000 };
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadThoiKhoaBieu();
        }

        private void InitGrid()
        {
            dgvTKB.Columns.Clear();
            dgvTKB.Rows.Clear();
            dgvTKB.ColumnCount = 7;

            for (int i = 0; i < 7; i++)
            {
                dgvTKB.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvTKB.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }

            dgvTKB.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvTKB.RowTemplate.MinimumHeight = 60;

            dgvTKB.RowCount = 10;
            for (int i = 0; i < 10; i++)
            {
                dgvTKB.Rows[i].HeaderCell.Value = "Tiết " + (i + 1);
            }
            dgvTKB.RowHeadersWidth = 100;

            dgvTKB.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvTKB.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        }

        private void UpdateColumnHeaders()
        {
            string[] thu = { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật" };
            for (int i = 0; i < 7; i++)
            {
                dgvTKB.Columns[i].HeaderText = $"{thu[i]}\n{currentMonday.AddDays(i):dd/MM}";
            }
        }

        private void LoadThoiKhoaBieu()
        {
            UpdateColumnHeaders();

            foreach (DataGridViewRow row in dgvTKB.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Value = "";
                    cell.ToolTipText = "";
                }
            }

            cellColors.Clear();

            DateTime weekEnd = currentMonday.AddDays(6);
            lblWeek.Text = $"Thời khóa biểu: {currentMonday:dd/MM} - {weekEnd:dd/MM/yyyy}";

            DataTable dt = DatabaseHelper.GetTKBByGV(maGV, currentMonday);

            foreach (DataRow r in dt.Rows)
            {
                DateTime ngay = Convert.ToDateTime(r["Ngay"]);
                int tiet = Convert.ToInt32(r["Tiet"]) - 1;
                string mon = r["TenMon"].ToString();
                string lop = r["TenLop"].ToString();
                string ghichu = r["GhiChu"].ToString();
                string mauSac = r["MauSac"].ToString();

                int col = (int)ngay.DayOfWeek - (int)DayOfWeek.Monday;
                if (col < 0) col = 6;

                if (tiet >= 0 && tiet < dgvTKB.RowCount && col >= 0 && col < dgvTKB.ColumnCount)
                {
                    Point cellPosition = new Point(col, tiet);
                    string displayValue = !string.IsNullOrEmpty(mon) ? $"{mon} - {lop}" : "";
                    if (!string.IsNullOrEmpty(ghichu))
                    {
                        displayValue += (string.IsNullOrEmpty(displayValue) ? "" : "\n") + $"({ghichu})";
                    }

                    dgvTKB[col, tiet].Value = displayValue;
                    dgvTKB[col, tiet].ToolTipText = displayValue;

                    if (!string.IsNullOrEmpty(mauSac)) { try { cellColors[cellPosition] = ColorTranslator.FromHtml(mauSac); } catch { } }
                    else if (!string.IsNullOrEmpty(ghichu)) { cellColors[cellPosition] = Color.FromArgb(230, 230, 230); }
                }
            }
            dgvTKB.Invalidate();
        }

        private void btnImportTKB_Click(object sender, EventArgs e)
        {
            string tenGV = DatabaseHelper.GetTeacherNameById(this.maGV);

            using (var importForm = new frmImportExcel(frmImportExcel.ImportType.ThoiKhoaBieu, this.maGV, tenGV))
            {
                if (importForm.ShowDialog() == DialogResult.OK)
                {
                    if (importForm.FirstImportedDate.HasValue)
                    {
                        SetCurrentWeek(importForm.FirstImportedDate.Value);
                    }
                    LoadThoiKhoaBieu();
                }
            }
        }

        private void dgvTKB_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dgvTKB.CurrentCell = dgvTKB[e.ColumnIndex, e.RowIndex];
                selectedCellForColorChange = new Point(e.ColumnIndex, e.RowIndex);
                string cellValue = dgvTKB.CurrentCell.Value as string ?? string.Empty;
                doiMauMenuItem.Enabled = !string.IsNullOrEmpty(cellValue);
                xoaGhiChuMenuItem.Enabled = cellValue.Contains("(");
            }
        }

        private void doiMauMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = cellColors.ContainsKey(selectedCellForColorChange) ? cellColors[selectedCellForColorChange] : Color.White;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    cellColors[selectedCellForColorChange] = colorDialog.Color;
                    DateTime cellDate = currentMonday.AddDays(selectedCellForColorChange.X);
                    int tiet = selectedCellForColorChange.Y + 1;
                    string colorHex = ColorTranslator.ToHtml(colorDialog.Color);
                    DatabaseHelper.UpdateCellColor(maGV, cellDate, tiet, colorHex);
                    dgvTKB.Invalidate();
                }
            }
        }

        private void xoaGhiChuMenuItem_Click(object sender, EventArgs e)
        {
            DateTime cellDate = currentMonday.AddDays(selectedCellForColorChange.X);
            int tiet = selectedCellForColorChange.Y + 1;
            DatabaseHelper.DeleteGhiChuTKB(maGV, cellDate, tiet);
            DatabaseHelper.UpdateCellColor(maGV, cellDate, tiet, null);
            cellColors.Remove(selectedCellForColorChange);
            LoadThoiKhoaBieu();
        }

        private void DgvTKB_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            e.PaintBackground(e.ClipBounds, true);

            Point cellPosition = new Point(e.ColumnIndex, e.RowIndex);
            if (cellColors.TryGetValue(cellPosition, out Color cellColor) && cellColor != Color.White)
            {
                using (Brush backBrush = new SolidBrush(cellColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.CellBounds);
                }
            }

            e.Graphics.DrawRectangle(Pens.LightGray, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width - 1, e.CellBounds.Height - 1);

            if (e.Value is string cellValue && !string.IsNullOrEmpty(cellValue))
            {
                Color fontColor = (cellColor.GetBrightness() < 0.6 && cellColor != Color.White) ? Color.White : Color.Black;
                Rectangle textBounds = e.CellBounds;
                textBounds.Inflate(-10, -10);
                TextRenderer.DrawText(e.Graphics, cellValue, e.CellStyle.Font, textBounds, fontColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak);
            }
            e.Handled = true;
        }

        private void DgvTKB_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DateTime cellDate = currentMonday.AddDays(e.ColumnIndex);
            int tiet = e.RowIndex + 1;
            string currentValue = dgvTKB[e.ColumnIndex, e.RowIndex].Value?.ToString() ?? "";
            string currentNote = "";
            int noteStartIndex = currentValue.IndexOf('(');
            if (noteStartIndex != -1)
            {
                currentNote = currentValue.Substring(noteStartIndex + 1).TrimEnd(')');
            }

            using (Form inputForm = new Form() { Width = 400, Height = 180, Text = "Ghi chú", StartPosition = FormStartPosition.CenterParent })
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
                    DatabaseHelper.UpsertGhiChuTKB(maGV, cellDate, tiet, note);
                    LoadThoiKhoaBieu();
                }
            }
        }

        private void btnPrevWeek_Click(object sender, EventArgs e)
        {
            currentMonday = currentMonday.AddDays(-7);
            LoadThoiKhoaBieu();
        }

        private void btnNextWeek_Click(object sender, EventArgs e)
        {
            currentMonday = currentMonday.AddDays(7);
            LoadThoiKhoaBieu();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DatabaseHelper.ThoiKhoaBieuChanged -= OnThoiKhoaBieuChanged;
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}