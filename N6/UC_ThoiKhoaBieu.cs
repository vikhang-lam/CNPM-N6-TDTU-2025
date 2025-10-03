using System;
using System.Data;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_ThoiKhoaBieu : UserControl
    {
        private string maGV;
        private DateTime currentMonday;

        public UC_ThoiKhoaBieu(string maGVien)
        {
            InitializeComponent();
            maGV = maGVien;
            InitGrid();

            // Xác định ngày thứ 2 của tuần hiện tại
            DateTime today = DateTime.Today;
            currentMonday = today.AddDays(-(int)today.DayOfWeek + 1);
            if (today.DayOfWeek == DayOfWeek.Sunday)
                currentMonday = today.AddDays(-6);

            LoadThoiKhoaBieu();
        }

        private void InitGrid()
        {
            dgvTKB.Columns.Clear();
            dgvTKB.Rows.Clear();

            dgvTKB.ColumnCount = 7;
            string[] thu = { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN" };
            for (int i = 0; i < 7; i++)
            {
                dgvTKB.Columns[i].Name = thu[i];
                dgvTKB.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dgvTKB.RowCount = 10; // ví dụ có 10 tiết
            for (int i = 0; i < 10; i++)
                dgvTKB.Rows[i].HeaderCell.Value = "Tiết " + (i + 1);

            dgvTKB.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTKB.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dgvTKB.CellDoubleClick += DgvTKB_CellDoubleClick;
        }

        private void LoadThoiKhoaBieu()
        {
            foreach (DataGridViewRow row in dgvTKB.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Value = "";

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

                int col = (int)ngay.DayOfWeek - 1;
                if (col < 0) col = 6; // CN

                dgvTKB[col, tiet].Value = mon + " - " + lop +
                    (string.IsNullOrEmpty(ghichu) ? "" : "\n(" + ghichu + ")");
            }
        }

        private void DgvTKB_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DateTime cellDate = currentMonday.AddDays(e.ColumnIndex);
            int tiet = e.RowIndex + 1;

            // Tạo InputBox đơn giản
            Form inputForm = new Form()
            {
                Width = 400,
                Height = 180,
                Text = "Ghi chú",
                StartPosition = FormStartPosition.CenterParent
            };
            Label lbl = new Label() { Text = "Nhập ghi chú:", Left = 10, Top = 20, Width = 360 };
            TextBox txt = new TextBox() { Left = 10, Top = 50, Width = 360 };
            Button ok = new Button() { Text = "OK", Left = 220, Width = 70, Top = 90, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Hủy", Left = 300, Width = 70, Top = 90, DialogResult = DialogResult.Cancel };
            inputForm.Controls.Add(lbl);
            inputForm.Controls.Add(txt);
            inputForm.Controls.Add(ok);
            inputForm.Controls.Add(cancel);
            inputForm.AcceptButton = ok;
            inputForm.CancelButton = cancel;

            if (inputForm.ShowDialog() == DialogResult.OK)
            {
                string note = txt.Text.Trim();
                DatabaseHelper.UpdateGhiChuTKB(maGV, cellDate, tiet, note);
                LoadThoiKhoaBieu();
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
    }
}
