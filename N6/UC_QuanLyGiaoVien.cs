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
            this.tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;

            this.tabControl1.DrawItem += new DrawItemEventHandler(this.tabControl1_DrawItem);
            LoadDataForCurrentTab();
            UpdatePanelVisibility();
        }

        private void LoadDataForCurrentTab()
        {
            DataTable dt = null;
            try
            {
                if (tabControl1.SelectedTab == tabTatCa)
                {
                    dt = DatabaseHelper.GetAllGiaoVien();
                }
                else if (tabControl1.SelectedTab == tabChoDuyet)
                {
                    dt = DatabaseHelper.GetGiaoVienByTrangThai("Chưa xác nhận");
                }
                else if (tabControl1.SelectedTab == tabDaXacNhan)
                {
                    dt = DatabaseHelper.GetGiaoVienByTrangThai("Đã xác nhận");
                }
                dgvGV.DataSource = dt;
                CustomizeGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomizeGrid()
        {
            dgvGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);

            if (dgvGV.Columns["MaGV"] != null) dgvGV.Columns["MaGV"].HeaderText = "Mã Giáo Viên";
            if (dgvGV.Columns["Ten"] != null) dgvGV.Columns["Ten"].HeaderText = "Họ và Tên";
            if (dgvGV.Columns["Username"] != null) dgvGV.Columns["Username"].HeaderText = "Tên đăng nhập";
            if (dgvGV.Columns["Email"] != null) dgvGV.Columns["Email"].HeaderText = "Email";
            if (dgvGV.Columns["SDT"] != null) dgvGV.Columns["SDT"].HeaderText = "Số Điện Thoại";
            if (dgvGV.Columns["TrangThai"] != null) dgvGV.Columns["TrangThai"].HeaderText = "Trạng Thái";
        }

        private void UpdatePanelVisibility()
        {
            bool isChoDuyetTab = tabControl1.SelectedTab == tabChoDuyet;
            pnlDuyet.Visible = isChoDuyetTab;
            btnSua.Visible = !isChoDuyetTab;
            btnXoa.Visible = !isChoDuyetTab;

            if (dgvGV.CurrentRow == null)
            {
                ClearInputs();
            }
        }
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage currentPage = tabControl1.TabPages[e.Index];
            Rectangle tabBounds = e.Bounds;

            Brush backgroundBrush;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                backgroundBrush = Brushes.White;
            }
            else
            {
                backgroundBrush = SystemBrushes.Control;
            }

            e.Graphics.FillRectangle(backgroundBrush, tabBounds);

            Brush textBrush;

            if (currentPage == tabChoDuyet)
            {
                textBrush = Brushes.Red;
            }
            else
            {
                textBrush = Brushes.Black;
            }

            StringFormat stringFlags = new StringFormat();
            stringFlags.Alignment = StringAlignment.Center;
            stringFlags.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString(currentPage.Text, e.Font, textBrush, tabBounds, stringFlags);
        }
        private void ClearInputs()
        {
            txtTen.Clear();
            txtEmail.Clear();
            txtSDT.Clear();
            lblSelectedGV.Text = "Chưa chọn giáo viên";
            dgvGV.ClearSelection();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDataForCurrentTab();
            ClearInputs();
            UpdatePanelVisibility();
        }

        private void dgvGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvGV.Rows[e.RowIndex];
                txtTen.Text = row.Cells["Ten"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtSDT.Text = row.Cells["SDT"].Value?.ToString();
                lblSelectedGV.Text = $"Đang chọn: {row.Cells["Ten"].Value?.ToString()} (Mã: {row.Cells["MaGV"].Value?.ToString()})";
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadDataForCurrentTab();
            ClearInputs();
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (dgvGV.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một tài khoản để xác nhận.", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string maGV = dgvGV.CurrentRow.Cells["MaGV"].Value.ToString();
            DatabaseHelper.UpdateTrangThaiGiaoVien(maGV, "Đã xác nhận");
            MessageBox.Show("Xác nhận tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadDataForCurrentTab(); // Tải lại tab chờ duyệt
            ClearInputs();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (dgvGV.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một tài khoản để hủy.", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string maGV = dgvGV.CurrentRow.Cells["MaGV"].Value.ToString();
            if (MessageBox.Show("Bạn có chắc muốn hủy yêu cầu tạo tài khoản này không?", "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteGiaoVien(maGV);
                MessageBox.Show("Đã hủy yêu cầu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataForCurrentTab();
                ClearInputs();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvGV.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một giáo viên để sửa.", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string maGV = dgvGV.CurrentRow.Cells["MaGV"].Value.ToString();
            string ten = txtTen.Text.Trim();
            string email = txtEmail.Text.Trim();
            string sdt = txtSDT.Text.Trim();

            if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Tên và Email không được để trống!", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DatabaseHelper.UpdateGiaoVien(maGV, ten, email, sdt);
            MessageBox.Show("Cập nhật thông tin giáo viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadDataForCurrentTab();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvGV.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một giáo viên để xóa.", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string maGV = dgvGV.CurrentRow.Cells["MaGV"].Value.ToString();
            string tenGV = dgvGV.CurrentRow.Cells["Ten"].Value.ToString();

            if (MessageBox.Show($"Bạn có chắc muốn xóa vĩnh viễn giáo viên '{tenGV}' (Mã: {maGV})?\nHành động này không thể hoàn tác.", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteGiaoVien(maGV);
                    MessageBox.Show("Xóa giáo viên thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataForCurrentTab();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa giáo viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}