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

            // Gán sự kiện (sẽ được gỡ trong Dispose)
            this.tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tabControl1.DrawItem += new DrawItemEventHandler(this.tabControl1_DrawItem);

            //this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            //this.dgvGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGV_CellClick);
            //this.btnLamMoi.Click += new System.EventHandler(this.btnLamMoi_Click);
            //this.btnXacNhan.Click += new System.EventHandler(this.btnXacNhan_Click);
            //this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            //this.btnSua.Click += new System.EventHandler(this.btnSua_Click);
            //this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);

            LoadAllMonHoc(); // Tải cache môn họcs
            LoadDataForCurrentTab();
            UpdatePanelVisibility();
        }

        #region Data Loading & UI Setup (Tải dữ liệu & Cài đặt UI)

        /// <summary>
        /// Tải tất cả các môn học từ CSDL vào CheckedListBox (clbMonHoc).
        /// </summary>
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

        /// <summary>
        /// Cập nhật trạng thái (check/uncheck) của clbMonHoc dựa trên các môn GV đang dạy.
        /// </summary>
        /// <param name="maGV">Mã giáo viên đang được chọn.</param>
        private void LoadMonHocForGiaoVien(string maGV)
        {
            try
            {
                DataTable dtTeacherSubjects = DatabaseHelper.GetSubjectsByTeacher(maGV);

                // Dùng HashSet để tra cứu nhanh (O(1))
                var teacherMaMonList = new HashSet<string>(
                    dtTeacherSubjects.AsEnumerable().Select(r => r.Field<string>("MaMon"))
                );

                clbMonHoc.Enabled = false; // Tắt tạm thời để tránh giật

                // Lặp qua tất cả các item trong CheckedListBox
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
                clbMonHoc.Enabled = true; // Bật lại control
            }
        }

        /// <summary>
        /// Tải dữ liệu DataGridView dựa trên Tab đang được chọn.
        /// </summary>
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

        /// <summary>
        /// Tùy chỉnh tiêu đề và style cho các cột của DataGridView.
        /// </summary>
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

        /// <summary>
        /// Cập nhật hiển thị của các panel (panel Duyệt) và nút bấm (Sửa/Xóa).
        /// </summary>
        private void UpdatePanelVisibility()
        {
            bool isChoDuyetTab = (tabControl1.SelectedTab == tabChoDuyet);
            pnlDuyet.Visible = isChoDuyetTab;
            btnSua.Visible = true; // Nút Sửa luôn hiển thị
            btnXoa.Visible = !isChoDuyetTab; // Nút Xóa chỉ ẩn ở tab Chờ Duyệt

            if (dgvGV.CurrentRow == null)
            {
                ClearInputs();
            }
        }

        /// <summary>
        /// Xóa sạch các ô nhập liệu và reset CheckedListBox.
        /// </summary>
        private void ClearInputs()
        {
            txtTen.Clear();
            txtEmail.Clear();
            txtSDT.Clear();
            lblSelectedGV.Text = "Chưa chọn giáo viên";
            dgvGV.ClearSelection();

            // Uncheck tất cả các môn học
            clbMonHoc.Enabled = false;
            for (int i = 0; i < clbMonHoc.Items.Count; i++)
            {
                clbMonHoc.SetItemChecked(i, false);
            }
            clbMonHoc.Enabled = true;
        }

        /// <summary>
        /// Chọn một dòng trong DataGridView dựa trên MaGV.
        /// </summary>
        private void SelectRowByMaGV(string maGV)
        {
            foreach (DataGridViewRow row in dgvGV.Rows)
            {
                if (row.Cells["MaGV"].Value?.ToString() == maGV)
                {
                    row.Selected = true;
                    dgvGV.CurrentCell = row.Cells[0]; // Focus vào dòng đó
                    dgvGV.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        #endregion

        #region Event Handlers (Xử lý sự kiện)

        /// <summary>
        /// Tùy chỉnh việc vẽ TabPage (tô đỏ tab "Chờ duyệt").
        /// </summary>
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage currentPage = tabControl1.TabPages[e.Index];
            Rectangle tabBounds = e.Bounds;
            Brush backgroundBrush;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                backgroundBrush = Brushes.White; // Màu nền khi tab được chọn
            }
            else
            {
                backgroundBrush = SystemBrushes.Control; // Màu nền mặc định
            }
            e.Graphics.FillRectangle(backgroundBrush, tabBounds);

            Brush textBrush;
            // Tô đỏ chữ tab "Chờ duyệt"
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

        /// <summary>
        /// Xử lý khi chuyển tab (tải lại dữ liệu, xóa input).
        /// </summary>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDataForCurrentTab();
            ClearInputs();
            UpdatePanelVisibility();
        }

        /// <summary>
        /// Xử lý khi click vào một ô trong DataGridView (hiển thị thông tin lên các TextBox).
        /// </summary>
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

        /// <summary>
        /// Xử lý nút "Làm mới" (tải lại dữ liệu tab hiện tại).
        /// </summary>
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadDataForCurrentTab();
            ClearInputs();
        }

        /// <summary>
        /// Xử lý nút "Xác nhận" (duyệt tài khoản).
        /// </summary>
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
                // 1. Cập nhật trạng thái và lấy email, tên
                var result = DatabaseHelper.UpdateTeacherStatus(maGV, "Đã xác nhận");
                string email = result.Item1;
                string tenGV = result.Item2;

                // 2. Lấy email Admin
                string adminEmail = DatabaseHelper.GetAdminEmail("AD001");

                // 3. Gửi email thông báo
                if (!string.IsNullOrEmpty(email))
                {
                    // Lớp UI gọi EmailHelper (đúng quy tắc)
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
                // Bắt lỗi nếu EmailHelper ném ra
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý nút "Hủy" (từ chối/xóa yêu cầu).
        /// </summary>
        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (dgvGV.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một tài khoản để hủy.", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maGV = dgvGV.CurrentRow.Cells["MaGV"].Value.ToString();
            string tenGV = dgvGV.CurrentRow.Cells["Ten"].Value.ToString();

            if (MessageBox.Show($"Bạn có chắc muốn hủy yêu cầu tạo tài khoản của '{tenGV}' không?", "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string email = dgvGV.CurrentRow.Cells["Email"].Value?.ToString();
                    string adminEmail = DatabaseHelper.GetAdminEmail("AD001");

                    // Xóa giáo viên (yêu cầu)
                    DatabaseHelper.DeleteTeacher(maGV);

                    // Gửi email thông báo từ chối
                    if (!string.IsNullOrEmpty(email))
                    {
                        bool emailSent = EmailHelper.SendAccountStatusEmail(email, tenGV, isApproved: false, adminEmail);
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
        }

        /// <summary>
        /// Xử lý nút "Sửa" (cập nhật thông tin GV và môn học).
        /// </summary>
        private void btnSua_Click(object sender, EventArgs e)
        {
            // Lấy MaGV từ dòng đang chọn
            string maGV = dgvGV.CurrentRow?.Cells["MaGV"].Value?.ToString();

            if (string.IsNullOrEmpty(maGV))
            {
                MessageBox.Show("Vui lòng chọn một giáo viên để sửa.", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy thông tin từ giao diện
            string ten = txtTen.Text.Trim();
            string email = txtEmail.Text.Trim();
            string sdt = txtSDT.Text.Trim();

            // --- BẮT ĐẦU KIỂM TRA DỮ LIỆU NHẬP ---
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
            // --- KẾT THÚC KIỂM TRA DỮ LIỆU NHẬP ---

            try
            {
                // 1. Cập nhật thông tin cơ bản
                DatabaseHelper.UpdateTeacher(maGV, ten, email, sdt);

                // 2. Chuẩn bị danh sách mã môn học mới
                DataTable dtMaMonList = new DataTable();
                dtMaMonList.Columns.Add("MaMon", typeof(string));

                foreach (var item in clbMonHoc.CheckedItems)
                {
                    if (item is DataRowView drv)
                    {
                        dtMaMonList.Rows.Add(drv["MaMon"].ToString());
                    }
                }

                // 3. KIỂM TRA: Giáo viên có đang dạy lớp nào với môn bị bỏ chọn không?
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

                // 4. Gọi SP cập nhật môn học
                DatabaseHelper.UpdateTeacher_Subjects(maGV, dtMaMonList);

                MessageBox.Show("Cập nhật thông tin giáo viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 5. Tải lại dữ liệu và chọn lại dòng đã sửa
                LoadDataForCurrentTab();
                SelectRowByMaGV(maGV);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật thông tin: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Xử lý nút "Xóa" (xóa vĩnh viễn GV đã xác nhận).
        /// </summary>
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

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Gỡ bỏ các sự kiện đã gán thủ công
                if (this.tabControl1 != null)
                {
                    this.tabControl1.DrawItem -= new DrawItemEventHandler(this.tabControl1_DrawItem);
                    this.tabControl1.SelectedIndexChanged -= new System.EventHandler(this.tabControl1_SelectedIndexChanged);
                }
                if (this.dgvGV != null)
                {
                    this.dgvGV.CellClick -= new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGV_CellClick);
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