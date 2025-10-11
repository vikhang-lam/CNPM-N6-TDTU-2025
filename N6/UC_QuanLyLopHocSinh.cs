using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OfficeOpenXml;

namespace N6
{
    public partial class UC_QuanLyLopHocSinh : UserControl
    {
        public UC_QuanLyLopHocSinh()
        {
            InitializeComponent();
            LoadHocSinh();
            SetupPlaceholderText();
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
            dgvHocSinh.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);

            // Rename columns to Vietnamese
            if (dgvHocSinh.Columns["MaHS"] != null) dgvHocSinh.Columns["MaHS"].HeaderText = "Mã Học Sinh";
            if (dgvHocSinh.Columns["MaLop"] != null) dgvHocSinh.Columns["MaLop"].HeaderText = "Mã Lớp";
            if (dgvHocSinh.Columns["HoTen"] != null) dgvHocSinh.Columns["HoTen"].HeaderText = "Họ và Tên";
            if (dgvHocSinh.Columns["NgaySinh"] != null) dgvHocSinh.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
            if (dgvHocSinh.Columns["GioiTinh"] != null) dgvHocSinh.Columns["GioiTinh"].HeaderText = "Giới Tính";
            if (dgvHocSinh.Columns["SDTPhuHuynh"] != null) dgvHocSinh.Columns["SDTPhuHuynh"].HeaderText = "SĐT Phụ Huynh";
            if (dgvHocSinh.Columns["DiaChi"] != null) dgvHocSinh.Columns["DiaChi"].HeaderText = "Địa Chỉ";
            if (dgvHocSinh.Columns["DanToc"] != null) dgvHocSinh.Columns["DanToc"].HeaderText = "Dân Tộc";
        }

        private void SetupPlaceholderText()
        {
            txtNgaySinh.GotFocus += (s, e) => {
                if (txtNgaySinh.Text == "dd/MM/yyyy")
                {
                    txtNgaySinh.Text = "";
                    txtNgaySinh.ForeColor = Color.Black;
                }
            };
            txtNgaySinh.LostFocus += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtNgaySinh.Text))
                {
                    txtNgaySinh.Text = "dd/MM/yyyy";
                    txtNgaySinh.ForeColor = Color.Gray;
                }
            };
            txtNgaySinh.ForeColor = Color.Gray; // Initial color
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtMaHS.Text) ||
                string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                txtNgaySinh.Text == "dd/MM/yyyy" ||
                cboGioiTinh.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtSDTPhuHuynh.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ các trường thông tin bắt buộc: Mã HS, Họ Tên, Ngày Sinh, Giới Tính, SĐT Phụ Huynh.", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!DateTime.TryParseExact(txtNgaySinh.Text.Trim(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
            {
                MessageBox.Show("Ngày sinh phải có định dạng dd/MM/yyyy.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            txtMaHS.Clear();
            txtMaLop.Clear();
            txtHoTen.Clear();
            txtNgaySinh.Text = "dd/MM/yyyy";
            txtNgaySinh.ForeColor = Color.Gray;
            cboGioiTinh.SelectedIndex = -1;
            txtSDTPhuHuynh.Clear();
            txtDiaChi.Clear();
            txtDanToc.Clear();
            txtMaHS.Focus();
        }

        private void dgvHocSinh_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvHocSinh.Rows[e.RowIndex];
            txtMaHS.Text = row.Cells["MaHS"].Value?.ToString() ?? "";
            txtMaLop.Text = row.Cells["MaLop"].Value?.ToString() ?? "";
            txtHoTen.Text = row.Cells["HoTen"].Value?.ToString() ?? "";
            var ns = row.Cells["NgaySinh"].Value;
            if (ns != DBNull.Value && ns != null)
            {
                txtNgaySinh.Text = Convert.ToDateTime(ns).ToString("dd/MM/yyyy");
                txtNgaySinh.ForeColor = Color.Black;
            }
            else
            {
                txtNgaySinh.Text = "dd/MM/yyyy";
                txtNgaySinh.ForeColor = Color.Gray;
            }
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
                    string.IsNullOrWhiteSpace(txtMaLop.Text) ? null : txtMaLop.Text.Trim(),
                    txtHoTen.Text.Trim(),
                    DateTime.ParseExact(txtNgaySinh.Text.Trim(), "dd/MM/yyyy", null),
                    cboGioiTinh.Text,
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
                if (string.IsNullOrWhiteSpace(txtMaHS.Text))
                {
                    MessageBox.Show("Vui lòng chọn một học sinh để sửa.", "Chưa chọn học sinh", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!ValidateInput()) return;

                DatabaseHelper.UpdateHocSinh(
                    txtMaHS.Text.Trim(),
                    txtHoTen.Text.Trim(),
                    DateTime.ParseExact(txtNgaySinh.Text.Trim(), "dd/MM/yyyy", null),
                    cboGioiTinh.Text,
                    txtSDTPhuHuynh.Text.Trim(),
                    txtDiaChi.Text.Trim(),
                    txtDanToc.Text.Trim()
                );

                // Cập nhật thêm MaLop
                DatabaseHelper.ExecuteQuery($"UPDATE HocSinh SET MaLop = '{txtMaLop.Text.Trim()}' WHERE MaHS = '{txtMaHS.Text.Trim()}'");

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
                if (dgvHocSinh.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn một học sinh để xóa.", "Chưa chọn học sinh", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string maHS = dgvHocSinh.CurrentRow.Cells["MaHS"].Value?.ToString();
                if (string.IsNullOrEmpty(maHS)) return;

                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa học sinh '{maHS}' không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DatabaseHelper.DeleteHocSinh(maHS);
                    MessageBox.Show("Xóa thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadHocSinh();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    DataTable dt = new DataTable();
                    foreach (var firstRowCell in sheet.Cells[1, 1, 1, sheet.Dimension.End.Column])
                    {
                        dt.Columns.Add(firstRowCell.Text);
                    }

                    for (int r = 2; r <= sheet.Dimension.End.Row; r++)
                    {
                        var row = dt.NewRow();
                        for (int c = 1; c <= sheet.Dimension.End.Column; c++)
                        {
                            row[c - 1] = sheet.Cells[r, c].Text;
                        }
                        dt.Rows.Add(row);
                    }

                    dgvHocSinh.DataSource = dt;
                    CustomizeGrid();

                    if (MessageBox.Show($"Đã tải {dt.Rows.Count} dòng từ file Excel. Bạn có muốn import vào cơ sở dữ liệu không?\n(Các học sinh có mã trùng lặp sẽ bị bỏ qua)", "Xác nhận import", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        var result = DatabaseHelper.ImportHocSinhFromDataTable(dt);
                        string msg = $"Import hoàn tất.\n- Thành công: {result.Success}\n- Bỏ qua (đã tồn tại): {result.Skipped}\n- Lỗi (dữ liệu không hợp lệ): {result.Failed}";
                        MessageBox.Show(msg, "Kết quả Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadHocSinh();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi import file Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}