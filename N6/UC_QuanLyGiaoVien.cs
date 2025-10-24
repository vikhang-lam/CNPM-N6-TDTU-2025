using System;
using System.Data;
using System.Drawing;
using System.Linq; // Thêm
using System.Collections.Generic; // Thêm
using System.Windows.Forms;

namespace N6
{
    public partial class UC_QuanLyGiaoVien : UserControl
    {
        // Biến để lưu trữ toàn bộ môn học
        private DataTable allMonHoc;

        public UC_QuanLyGiaoVien()
        {
            InitializeComponent();
            this.tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tabControl1.DrawItem += new DrawItemEventHandler(this.tabControl1_DrawItem);

            LoadAllMonHoc(); // <-- GỌI HÀM MỚI

            LoadDataForCurrentTab();
            UpdatePanelVisibility();
        }

        // HÀM MỚI: Tải tất cả môn học vào CheckedListBox
        private void LoadAllMonHoc()
        {
            try
            {
                // Lấy tất cả môn học từ DB
                allMonHoc = DatabaseHelper.GetAllMonHoc();

                // Giả sử bạn đã thêm control tên là clbMonHoc vào designer
                clbMonHoc.DataSource = allMonHoc;
                clbMonHoc.DisplayMember = "TenMon";
                clbMonHoc.ValueMember = "MaMon";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiêm trọng khi tải danh sách môn học: " + ex.Message);
            }
        }

        // HÀM MỚI: Check các ô dựa trên giáo viên được chọn
        private void LoadMonHocForGiaoVien(string maGV)
        {
            // Lấy danh sách MaMon của GV này
            DataTable dtTeacherSubjects = DatabaseHelper.GetMonHocByGiaoVien(maGV);

            // Dùng HashSet để tra cứu nhanh hơn
            var teacherMaMonList = new HashSet<string>(
                dtTeacherSubjects.AsEnumerable().Select(r => r.Field<string>("MaMon"))
            );

            // Tắt tạm thời control để tránh việc check/uncheck bị giật
            clbMonHoc.Enabled = false;

            // Lặp qua tất cả các item trong CheckedListBox
            for (int i = 0; i < clbMonHoc.Items.Count; i++)
            {
                DataRowView drv = (DataRowView)clbMonHoc.Items[i];
                string maMon = drv["MaMon"].ToString();

                // Set trạng thái checked
                clbMonHoc.SetItemChecked(i, teacherMaMonList.Contains(maMon));
            }

            // Bật lại control
            clbMonHoc.Enabled = true;
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
            if (dgvGV.Columns["CacMonDay"] != null) dgvGV.Columns["CacMonDay"].HeaderText = "Môn Dạy";
        }

        private void UpdatePanelVisibility()
        {
            bool isChoDuyetTab = tabControl1.SelectedTab == tabChoDuyet;
            pnlDuyet.Visible = isChoDuyetTab;
            btnSua.Visible = true;
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

            // Thêm logic để uncheck tất cả các môn học
            clbMonHoc.Enabled = false; // Tắt tạm thời
            for (int i = 0; i < clbMonHoc.Items.Count; i++)
            {
                clbMonHoc.SetItemChecked(i, false);
            }
            clbMonHoc.Enabled = true; // Bật lại
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
                string maGV = row.Cells["MaGV"].Value.ToString(); // Lấy MaGV

                txtTen.Text = row.Cells["Ten"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtSDT.Text = row.Cells["SDT"].Value?.ToString();
                lblSelectedGV.Text = $"Đang chọn: {row.Cells["Ten"].Value?.ToString()} (Mã: {maGV})";

                // GỌI HÀM MỚI
                LoadMonHocForGiaoVien(maGV);
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

            try
            {
                // Cập nhật trạng thái và lấy email, tên giáo viên
                var result = DatabaseHelper.UpdateTrangThaiGiaoVien(maGV, "Đã xác nhận");
                string email = result.Item1;
                string tenGV = result.Item2;

                // Gửi email thông báo
                if (!string.IsNullOrEmpty(email))
                {
                    bool emailSent = EmailHelper.SendAccountStatusEmail(email, tenGV, isApproved: true);
                    if (emailSent)
                    {
                        MessageBox.Show($"Xác nhận tài khoản thành công!\nEmail thông báo đã được gửi đến: {email}",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Xác nhận tài khoản thành công!\nNhưng không thể gửi email đến: {email}",
                            "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Xác nhận tài khoản thành công! (Giáo viên chưa cung cấp email)",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LoadDataForCurrentTab();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (dgvGV.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một tài khoản để hủy.", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maGV = dgvGV.CurrentRow.Cells["MaGV"].Value.ToString();
            string tenGV = dgvGV.CurrentRow.Cells["Ten"].Value.ToString();

            if (MessageBox.Show($"Bạn có chắc muốn hủy yêu cầu tạo tài khoản của '{tenGV}' không?",
                "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    // Lấy email trước khi xóa
                    string email = dgvGV.CurrentRow.Cells["Email"].Value?.ToString();

                    // Xóa giáo viên
                    DatabaseHelper.DeleteGiaoVien(maGV);

                    // Gửi email thông báo từ chối
                    if (!string.IsNullOrEmpty(email))
                    {
                        bool emailSent = EmailHelper.SendAccountStatusEmail(email, tenGV, isApproved: false);
                        if (emailSent)
                        {
                            MessageBox.Show($"Đã hủy yêu cầu thành công.\nEmail thông báo đã được gửi đến: {email}",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Đã hủy yêu cầu thành công. (Không gửi được email thông báo)",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Đã hủy yêu cầu thành công.",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    LoadDataForCurrentTab();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

            try
            {
                // 1. Cập nhật thông tin cơ bản (tên, email, sdt)
                DatabaseHelper.UpdateGiaoVien(maGV, ten, email, sdt);

                // 2. Cập nhật danh sách môn học

                // Tạo DataTable để chứa danh sách MaMon
                DataTable dtMaMonList = new DataTable();
                dtMaMonList.Columns.Add("MaMon", typeof(string));

                // Lặp qua các mục ĐƯỢC CHỌN trong clbMonHoc
                foreach (var item in clbMonHoc.CheckedItems)
                {
                    DataRowView drv = (DataRowView)item;
                    dtMaMonList.Rows.Add(drv["MaMon"].ToString());
                }

                // Gọi SP cập nhật
                DatabaseHelper.UpdateGiaoVien_MonHoc(maGV, dtMaMonList);

                MessageBox.Show("Cập nhật thông tin giáo viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tải lại grid để thấy cột "Môn Dạy" được cập nhật
                LoadDataForCurrentTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật thông tin: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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