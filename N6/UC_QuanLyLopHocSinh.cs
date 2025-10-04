using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OfficeOpenXml; // EPPlus

namespace N6
{
    public partial class UC_QuanLyLopHocSinh : UserControl
    {
        public UC_QuanLyLopHocSinh()
        {
            InitializeComponent();
            LoadHocSinh();
        }

        private void LoadHocSinh()
        {
            try
            {
                DataTable dt = DatabaseHelper.GetAllHocSinh();
                dgvHocSinh.DataSource = dt;
                CustomizeGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách học sinh: " + ex.Message);
            }
        }

        private void CustomizeGrid()
        {
            dgvHocSinh.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHocSinh.EnableHeadersVisualStyles = false;
            dgvHocSinh.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 150, 200);
            dgvHocSinh.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHocSinh.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvHocSinh.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvHocSinh.RowTemplate.Height = 36;
            dgvHocSinh.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtMaHS.Text) ||
                string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtNgaySinh.Text) ||
                string.IsNullOrWhiteSpace(cboGioiTinh.Text) ||
                string.IsNullOrWhiteSpace(txtSDTPhuHuynh.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin bắt buộc.", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!DateTime.TryParseExact(txtNgaySinh.Text.Trim(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
            {
                MessageBox.Show("Ngày sinh phải có định dạng dd/MM/yyyy.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!txtSDTPhuHuynh.Text.All(char.IsDigit) || txtSDTPhuHuynh.Text.Length != 10)
            {
                MessageBox.Show("SĐT phụ huynh phải là 10 chữ số.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            txtMaHS.Clear();
            txtMaLop.Clear();
            txtHoTen.Clear();
            txtNgaySinh.Clear();
            cboGioiTinh.SelectedIndex = -1;
            txtSDTPhuHuynh.Clear();
            txtDiaChi.Clear();
            txtDanToc.Clear();
        }

        private void dgvHocSinh_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvHocSinh.Rows[e.RowIndex];
            txtMaHS.Text = row.Cells["MaHS"].Value?.ToString() ?? "";
            txtMaLop.Text = row.Cells["MaLop"].Value?.ToString() ?? "";
            txtHoTen.Text = row.Cells["HoTen"].Value?.ToString() ?? "";
            var ns = row.Cells["NgaySinh"].Value;
            txtNgaySinh.Text = ns == DBNull.Value || ns == null ? "" : Convert.ToDateTime(ns).ToString("dd/MM/yyyy");
            cboGioiTinh.Text = row.Cells["GioiTinh"].Value?.ToString() ?? "";
            txtSDTPhuHuynh.Text = row.Cells["SDTPhuHuynh"].Value?.ToString() ?? "";
            txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString() ?? "";
            txtDanToc.Text = row.Cells["DanToc"].Value?.ToString() ?? "";
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput()) return;

                DatabaseHelper.InsertHocSinh(
                    txtMaHS.Text.Trim(),
                    txtMaLop.Text.Trim(),
                    txtHoTen.Text.Trim(),
                    DateTime.ParseExact(txtNgaySinh.Text.Trim(), "dd/MM/yyyy", null),
                    cboGioiTinh.Text.Trim(),
                    txtSDTPhuHuynh.Text.Trim(),
                    txtDiaChi.Text.Trim(),
                    txtDanToc.Text.Trim()
                );

                MessageBox.Show("Thêm học sinh thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadHocSinh();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm học sinh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput()) return;

                DatabaseHelper.UpdateHocSinh(
                    txtMaHS.Text.Trim(),
                    txtHoTen.Text.Trim(),
                    DateTime.ParseExact(txtNgaySinh.Text.Trim(), "dd/MM/yyyy", null),
                    cboGioiTinh.Text.Trim(),
                    txtSDTPhuHuynh.Text.Trim(),
                    txtDiaChi.Text.Trim(),
                    txtDanToc.Text.Trim()
                );

                MessageBox.Show("Cập nhật học sinh thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadHocSinh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvHocSinh.CurrentRow == null) return;
                string maHS = dgvHocSinh.CurrentRow.Cells["MaHS"].Value?.ToString();
                if (string.IsNullOrEmpty(maHS)) return;

                if (MessageBox.Show($"Xác nhận xóa học sinh {maHS} ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DatabaseHelper.DeleteHocSinh(maHS);
                    MessageBox.Show("Xóa thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadHocSinh();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearInputs();
            LoadHocSinh();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Chọn file Excel chứa danh sách học sinh"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            try
            {
                FileInfo file = new FileInfo(ofd.FileName);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    var sheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (sheet == null)
                    {
                        MessageBox.Show("File Excel không có sheet hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Chuẩn: header (dòng 1) phải có: MaHS, MaLop, HoTen, NgaySinh(dd/MM/yyyy), GioiTinh, SDTPhuHuynh, DiaChi, DanToc
                    DataTable dt = new DataTable();
                    dt.Columns.Add("MaHS");
                    dt.Columns.Add("MaLop");
                    dt.Columns.Add("HoTen");
                    dt.Columns.Add("NgaySinh");
                    dt.Columns.Add("GioiTinh");
                    dt.Columns.Add("SDTPhuHuynh");
                    dt.Columns.Add("DiaChi");
                    dt.Columns.Add("DanToc");

                    int startRow = 2;
                    for (int r = startRow; r <= sheet.Dimension.End.Row; r++)
                    {
                        // bỏ qua các dòng trống hoàn toàn
                        bool empty = true;
                        for (int c = 1; c <= 8; c++)
                        {
                            if (!string.IsNullOrWhiteSpace(sheet.Cells[r, c].Text))
                            {
                                empty = false; break;
                            }
                        }
                        if (empty) continue;

                        DataRow dr = dt.NewRow();
                        dr["MaHS"] = sheet.Cells[r, 1].Text.Trim();
                        dr["MaLop"] = sheet.Cells[r, 2].Text.Trim();
                        dr["HoTen"] = sheet.Cells[r, 3].Text.Trim();
                        dr["NgaySinh"] = sheet.Cells[r, 4].Text.Trim();
                        dr["GioiTinh"] = sheet.Cells[r, 5].Text.Trim();
                        dr["SDTPhuHuynh"] = sheet.Cells[r, 6].Text.Trim();
                        dr["DiaChi"] = sheet.Cells[r, 7].Text.Trim();
                        dr["DanToc"] = sheet.Cells[r, 8].Text.Trim();
                        dt.Rows.Add(dr);
                    }

                    // Hiển thị preview
                    dgvHocSinh.DataSource = dt;
                    if (MessageBox.Show($"Preview {dt.Rows.Count} dòng. Xác nhận import vào cơ sở dữ liệu?", "Xác nhận import", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        var result = DatabaseHelper.ImportHocSinhFromDataTable(dt); // returns summary
                        string msg = $"Import xong.\nThành công: {result.Success}\nBị bỏ qua (đã tồn tại): {result.Skipped}\nLỗi: {result.Failed}";
                        MessageBox.Show(msg, "Kết quả import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadHocSinh();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi import: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
