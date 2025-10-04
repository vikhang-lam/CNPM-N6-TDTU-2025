using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_QuanLyGiaoVien : UserControl
    {
        public UC_QuanLyGiaoVien()
        {
            InitializeComponent();
            LoadTatCaGiaoVien();
        }

        private void LoadTatCaGiaoVien()
        {
            DataTable dt = DatabaseHelper.GetAllGiaoVien();
            dgvGV.DataSource = dt;
            CustomizeGrid();
        }

        private void LoadChoDuyet()
        {
            DataTable dt = DatabaseHelper.GetGiaoVienByTrangThai("Chưa xác nhận");
            dgvGV.DataSource = dt;
            CustomizeGrid();
        }

        private void CustomizeGrid()
        {
            dgvGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGV.EnableHeadersVisualStyles = false;
            dgvGV.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 150, 200);
            dgvGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGV.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvGV.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvGV.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 245, 255);
            dgvGV.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvGV.RowTemplate.Height = 35;
            dgvGV.BorderStyle = BorderStyle.None;
            dgvGV.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadTatCaGiaoVien();
        }

        private void btnChoDuyet_Click(object sender, EventArgs e)
        {
            LoadChoDuyet();
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (dgvGV.CurrentRow == null) return;
            string maGV = dgvGV.CurrentRow.Cells["MaGV"].Value.ToString();
            DatabaseHelper.UpdateTrangThaiGiaoVien(maGV, "Đã xác nhận");
            LoadChoDuyet();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (dgvGV.CurrentRow == null) return;
            string maGV = dgvGV.CurrentRow.Cells["MaGV"].Value.ToString();
            if (MessageBox.Show("Hủy yêu cầu tạo tài khoản này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteGiaoVien(maGV);
                LoadChoDuyet();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvGV.CurrentRow == null) return;
            string maGV = dgvGV.CurrentRow.Cells["MaGV"].Value.ToString();
            string ten = txtTen.Text;
            string email = txtEmail.Text;
            string sdt = txtSDT.Text;

            if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DatabaseHelper.UpdateGiaoVien(maGV, ten, email, sdt);
            MessageBox.Show("Cập nhật thành công!", "Thông báo");
            LoadTatCaGiaoVien();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvGV.CurrentRow == null) return;
            string maGV = dgvGV.CurrentRow.Cells["MaGV"].Value.ToString();

            if (MessageBox.Show("Bạn có chắc muốn xóa giáo viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteGiaoVien(maGV);
                LoadTatCaGiaoVien();
            }
        }

        private void dgvGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtTen.Text = dgvGV.Rows[e.RowIndex].Cells["Ten"].Value?.ToString();
                txtEmail.Text = dgvGV.Rows[e.RowIndex].Cells["Email"].Value?.ToString();
                txtSDT.Text = dgvGV.Rows[e.RowIndex].Cells["SDT"].Value?.ToString();
            }
        }
    }
}
