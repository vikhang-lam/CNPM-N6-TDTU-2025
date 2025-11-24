using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace N6
{
    /// <summary>
    /// UserControl để quản lý các tài khoản Giáo viên (duyệt, sửa, xóa, phân công môn học).
    /// </summary>
    public partial class UC_QuanLyGiaoVien : UserControl
    {
        // Biến để lưu trữ toàn bộ môn học (cache)
        private DataTable allMonHoc;

        public UC_QuanLyGiaoVien()
        {
            InitializeComponent();

            this.tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tabControl1.DrawItem += new DrawItemEventHandler(this.tabControl1_DrawItem);

            LoadAllMonHoc();
            LoadDataForCurrentTab();
            UpdatePanelVisibility();
        }

        #region Data Loading & UI Setup (Tải dữ liệu & Cài đặt UI)

        /// <summary>
        /// Xử lý logic lọc dữ liệu khi nhập text.
        /// Sự kiện này đã được gán trong file Designer.
        /// </summary>
        private void TxtTimKiem_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = dgvGV.DataSource as DataTable;
                if (dt != null)
                {
                    string keyword = txtTimKiem.Text.Trim();
                    // Xử lý ký tự đặc biệt
                    keyword = keyword.Replace("'", "''").Replace("[", "\\[").Replace("]", "\\]");

                    if (string.IsNullOrEmpty(keyword))
                    {
                        dt.DefaultView.RowFilter = "";
                    }
                    else
                    {
                        // Lọc theo Tên HOẶC Mã HOẶC Email
                        // Vì cả 3 tab đều có các cột này nên bộ lọc hoạt động tốt cho tất cả
                        dt.DefaultView.RowFilter = string.Format(
                            "Ten LIKE '%{0}%' OR MaGV LIKE '%{0}%' OR Email LIKE '%{0}%'",
                            keyword
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        private void LoadAllMonHoc()
        {
            try
            {
                allMonHoc = DatabaseHelper.GetAllSubjects();
                clbMonHoc.DataSource = allMonHoc;
                clbMonHoc.DisplayMember = "TenMon";
                clbMonHoc.ValueMember = "MaMon";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiêm trọng khi tải danh sách môn học: " + ex.Message);
            }
        }

        private void LoadMonHocForGiaoVien(string maGV)
        {
            try
            {
                DataTable dtTeacherSubjects = DatabaseHelper.GetSubjectsByTeacher(maGV);
                var teacherMaMonList = new HashSet<string>(
                    dtTeacherSubjects.AsEnumerable().Select(r => r.Field<string>("MaMon"))
                );

                clbMonHoc.Enabled = false;

                for (int i = 0; i < clbMonHoc.Items.Count; i++)
                {
                    DataRowView drv = (DataRowView)clbMonHoc.Items[i];
                    string maMon = drv["MaMon"].ToString();
                    clbMonHoc.SetItemChecked(i, teacherMaMonList.Contains(maMon));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải môn học của giáo viên: " + ex.Message);
            }
            finally
            {
                clbMonHoc.Enabled = true;
            }
        }

        private void LoadDataForCurrentTab()
        {
            DataTable dt = null;
            try
            {
                if (tabControl1.SelectedTab == tabTatCa)
                {
                    dt = DatabaseHelper.GetAllTeachers();
                }
                else if (tabControl1.SelectedTab == tabChoDuyet)
                {
                    dt = DatabaseHelper.GetTeachersByStatus("Chưa xác nhận");
                }
                else if (tabControl1.SelectedTab == tabDaXacNhan)
                {
                    dt = DatabaseHelper.GetTeachersByStatus("Đã xác nhận");
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

            try
            {
                if (dgvGV.Columns["MaGV"] != null)
                {
                    dgvGV.Columns["MaGV"].HeaderText = "Mã Giáo Viên";
                    dgvGV.Columns["MaGV"].ReadOnly = true;
                }
                if (dgvGV.Columns["Ten"] != null) dgvGV.Columns["Ten"].HeaderText = "Họ và Tên";
                if (dgvGV.Columns["Username"] != null) dgvGV.Columns["Username"].HeaderText = "Tên đăng nhập";
                if (dgvGV.Columns["Email"] != null) dgvGV.Columns["Email"].HeaderText = "Email";
                if (dgvGV.Columns["SDT"] != null) dgvGV.Columns["SDT"].HeaderText = "Số Điện Thoại";
                if (dgvGV.Columns["TrangThai"] != null) dgvGV.Columns["TrangThai"].HeaderText = "Trạng Thái";

                if (dgvGV.Columns["LopChuNhiem"] != null)
                {
                    dgvGV.Columns["LopChuNhiem"].HeaderText = "Chủ Nhiệm Lớp";
                    dgvGV.Columns["LopChuNhiem"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (dgvGV.Columns["CacMonDay"] != null) dgvGV.Columns["CacMonDay"].HeaderText = "Môn Dạy";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Lỗi CustomizeGrid: " + ex.Message);
            }
        }

        private void UpdatePanelVisibility()
        {
            bool isChoDuyetTab = (tabControl1.SelectedTab == tabChoDuyet);

            pnlDuyet.Visible = isChoDuyetTab;
            btnSua.Visible = true;
            btnXoa.Visible = !isChoDuyetTab;

            // Đã xóa phần ẩn thanh tìm kiếm. 
            // Bây giờ txtTimKiem và lblTimKiem luôn hiển thị (Visible = true mặc định từ Designer)
            // nên ta không cần code can thiệp vào Visible của nó ở đây nữa.

            if (dgvGV.CurrentRow == null)
            {
                ClearInputs();
            }
        }

        private void ClearInputs()
        {
            txtTen.Clear();
            txtEmail.Clear();
            txtSDT.Clear();
            lblSelectedGV.Text = "Chưa chọn giáo viên";
            dgvGV.ClearSelection();

            clbMonHoc.Enabled = false;
            for (int i = 0; i < clbMonHoc.Items.Count; i++)
            {
                clbMonHoc.SetItemChecked(i, false);
            }
            clbMonHoc.Enabled = true;
        }

        private void SelectRowByMaGV(string maGV)
        {
            foreach (DataGridViewRow row in dgvGV.Rows)
            {
                if (row.Cells["MaGV"].Value?.ToString() == maGV)
                {
                    row.Selected = true;
                    dgvGV.CurrentCell = row.Cells[0];
                    dgvGV.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        #endregion

        #region Event Handlers (Xử lý sự kiện)

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

            StringFormat stringFlags = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            e.Graphics.DrawString(currentPage.Text, e.Font, textBrush, tabBounds, stringFlags);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset ô tìm kiếm mỗi khi chuyển tab để tránh nhầm lẫn dữ liệu giữa các tab
            txtTimKiem.Clear();

            LoadDataForCurrentTab();
            ClearInputs();
            UpdatePanelVisibility();
        }

        private void dgvGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvGV.Rows[e.RowIndex];
                string maGV = row.Cells["MaGV"].Value.ToString();

                txtTen.Text = row.Cells["Ten"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtSDT.Text = row.Cells["SDT"].Value?.ToString();
                lblSelectedGV.Text = $"Đang chọn: {row.Cells["Ten"].Value?.ToString()} (Mã: {maGV})";

                LoadMonHocForGiaoVien(maGV);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadDataForCurrentTab();
            ClearInputs();
            // Reset tìm kiếm khi bấm làm mới
            txtTimKiem.Clear();
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
                var result = DatabaseHelper.UpdateTeacherStatus(maGV, "Đã xác nhận");
                string email = result.Item1;
                string tenGV = result.Item2;

                string adminEmail = DatabaseHelper.GetAdminEmail("AD001");

                if (!string.IsNullOrEmpty(email))
                {
                    bool emailSent = EmailHelper.SendAccountStatusEmail(email, tenGV, isApproved: true, adminEmail);
                    if (emailSent)
                    {
                        MessageBox.Show($"Xác nhận tài khoản thành công!\nEmail thông báo đã được gửi đến: {email}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Xác nhận tài khoản thành công!\nNhưng không thể gửi email đến: {email}", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Xác nhận tài khoản thành công! (Giáo viên chưa cung cấp email)", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            string email = dgvGV.CurrentRow.Cells["Email"].Value?.ToString();

            Form inputBox = new Form();
            inputBox.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputBox.ClientSize = new Size(400, 220);
            inputBox.Text = "Xác nhận từ chối yêu cầu";
            inputBox.StartPosition = FormStartPosition.CenterParent;
            inputBox.MaximizeBox = false;
            inputBox.MinimizeBox = false;

            Label lblMessage = new Label();
            lblMessage.Text = $"Bạn đang từ chối giáo viên: {tenGV}\nNhập lý do từ chối (để trống nếu không cần):";
            lblMessage.AutoSize = false;
            lblMessage.Size = new Size(380, 40);
            lblMessage.Location = new Point(10, 10);
            inputBox.Controls.Add(lblMessage);

            TextBox txtLyDo = new TextBox();
            txtLyDo.Multiline = true;
            txtLyDo.ScrollBars = ScrollBars.Vertical;
            txtLyDo.Size = new Size(360, 100);
            txtLyDo.Location = new Point(20, 50);
            inputBox.Controls.Add(txtLyDo);

            Button btnOK = new Button();
            btnOK.Text = "Xác nhận Hủy";
            btnOK.BackColor = Color.FromArgb(220, 80, 80);
            btnOK.ForeColor = Color.White;
            btnOK.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Size = new Size(100, 35);
            btnOK.Location = new Point(180, 165);
            inputBox.Controls.Add(btnOK);

            Button btnCancel = new Button();
            btnCancel.Text = "Thoát";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Size = new Size(80, 35);
            btnCancel.Location = new Point(300, 165);
            inputBox.Controls.Add(btnCancel);

            inputBox.AcceptButton = btnOK;
            inputBox.CancelButton = btnCancel;

            if (inputBox.ShowDialog() == DialogResult.OK)
            {
                string lyDo = txtLyDo.Text.Trim();

                try
                {
                    string adminEmail = DatabaseHelper.GetAdminEmail("AD001");

                    DatabaseHelper.DeleteTeacher(maGV);

                    if (!string.IsNullOrEmpty(email))
                    {
                        bool emailSent = EmailHelper.SendAccountStatusEmail(email, tenGV, isApproved: false, adminEmail, rejectionReason: lyDo);
                        if (emailSent)
                        {
                            MessageBox.Show($"Đã hủy yêu cầu thành công.\nEmail thông báo đã được gửi đến: {email}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Đã hủy yêu cầu thành công. (Không gửi được email thông báo)", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Đã hủy yêu cầu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    LoadDataForCurrentTab();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            inputBox.Dispose();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maGV = dgvGV.CurrentRow?.Cells["MaGV"].Value?.ToString();

            if (string.IsNullOrEmpty(maGV))
            {
                MessageBox.Show("Vui lòng chọn một giáo viên để sửa.", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string ten = txtTen.Text.Trim();
            string email = txtEmail.Text.Trim();
            string sdt = txtSDT.Text.Trim();

            if (string.IsNullOrWhiteSpace(ten))
            {
                MessageBox.Show("Tên giáo viên không được để trống.", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Regex.IsMatch(ten, @"^[\p{L}\s]+$"))
            {
                MessageBox.Show("Tên giáo viên không hợp lệ. Không được chứa số hoặc ký tự đặc biệt.", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Email không được để trống.", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                MessageBox.Show("Email không đúng định dạng (ví dụ: ten@gmail.com).", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Regex.IsMatch(sdt, @"^\d{10}$"))
            {
                MessageBox.Show("Số điện thoại phải có đúng 10 chữ số và chỉ bao gồm số.", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DatabaseHelper.UpdateTeacher(maGV, ten, email, sdt);

                DataTable dtMaMonList = new DataTable();
                dtMaMonList.Columns.Add("MaMon", typeof(string));

                foreach (var item in clbMonHoc.CheckedItems)
                {
                    if (item is DataRowView drv)
                    {
                        dtMaMonList.Rows.Add(drv["MaMon"].ToString());
                    }
                }

                DataTable dtPhanCong = DatabaseHelper.GetTeacherAssignments(maGV);

                var dsMonDangDay = dtPhanCong.AsEnumerable()
                    .Select(r => r.Field<string>("MaMon"))
                    .Distinct()
                    .ToList();

                var dsMonMoi = dtMaMonList.AsEnumerable()
                    .Select(r => r.Field<string>("MaMon"))
                    .ToList();

                var monDangDayBiBo = dsMonDangDay.Where(mon => !dsMonMoi.Contains(mon)).ToList();

                if (monDangDayBiBo.Count > 0)
                {
                    var chiTiet = string.Join(", ", dtPhanCong.AsEnumerable()
                        .Where(r => monDangDayBiBo.Contains(r.Field<string>("MaMon")))
                        .Select(r => $"{r["TenMon"]} (Lớp {r["MaLop"]})"));

                    MessageBox.Show(
                        $"Không thể thay đổi bộ môn vì giáo viên đang được phân công dạy: {chiTiet}. " +
                        $"Vui lòng hủy phân công trước khi đổi môn.",
                        "Không thể thay đổi môn dạy",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                DatabaseHelper.UpdateTeacher_Subjects(maGV, dtMaMonList);

                MessageBox.Show("Cập nhật thông tin giáo viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadDataForCurrentTab();
                SelectRowByMaGV(maGV);
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
                    DatabaseHelper.DeleteTeacher(maGV);
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

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.tabControl1 != null)
                {
                    this.tabControl1.DrawItem -= new DrawItemEventHandler(this.tabControl1_DrawItem);
                    this.tabControl1.SelectedIndexChanged -= new System.EventHandler(this.tabControl1_SelectedIndexChanged);
                }
                if (this.dgvGV != null)
                {
                    this.dgvGV.CellClick -= new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGV_CellClick);
                }
                if (this.txtTimKiem != null)
                {
                    this.txtTimKiem.TextChanged -= TxtTimKiem_TextChanged;
                }
                if (this.btnLamMoi != null) this.btnLamMoi.Click -= new System.EventHandler(this.btnLamMoi_Click);
                if (this.btnXacNhan != null) this.btnXacNhan.Click -= new System.EventHandler(this.btnXacNhan_Click);
                if (this.btnHuy != null) this.btnHuy.Click -= new System.EventHandler(this.btnHuy_Click);
                if (this.btnSua != null) this.btnSua.Click -= new System.EventHandler(this.btnSua_Click);
                if (this.btnXoa != null) this.btnXoa.Click -= new System.EventHandler(this.btnXoa_Click);

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}