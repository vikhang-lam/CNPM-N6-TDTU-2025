using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_QuanLyLopHocSinh : UserControl
    {
        private DataTable allLopHoc;
        private DataTable allGiaoVien;
        private bool isProgrammaticChange = false;

        public UC_QuanLyLopHocSinh()
        {
            InitializeComponent();
            LoadInitialData();
            StyleControls();
        }

        // --- SETUP GIAO DIỆN & DỮ LIỆU ---

        private void LoadInitialData()
        {
            try
            {
                allLopHoc = DatabaseHelper.GetAllLopHoc();
                allGiaoVien = DatabaseHelper.GetAllGiaoVien();

                var khoiList = allLopHoc.AsEnumerable()
                                        .Select(row => row.Field<string>("Khoi"))
                                        .Distinct()
                                        .OrderBy(k => k)
                                        .ToList();
                khoiList.Insert(0, "Tất cả các khối");
                cboKhoi.DataSource = khoiList;

                LoadUnassignedGvcn();

                DatabaseHelper.StyleDataGridView(dgvHocSinh);
                DatabaseHelper.StyleDataGridView(dgvPhanCong);
                StyleClassListGrid();

                // Thêm bộ xử lý lỗi dữ liệu cho bảng phân công
                dgvPhanCong.DataError += new DataGridViewDataErrorEventHandler(dgvPhanCong_DataError);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu ban đầu: " + ex.Message);
            }
        }

        private void LoadUnassignedGvcn()
        {
            cboGvcn.DataSource = DatabaseHelper.GetUnassignedHomeroomTeachers();
            cboGvcn.DisplayMember = "Ten";
            cboGvcn.ValueMember = "MaGV";
            cboGvcn.SelectedIndex = -1;
            cboGvcn.Text = "Chọn giáo viên...";
        }

        private void StyleControls()
        {
            pnlFilter.Paint += (s, e) => DrawShadow(s, e, 12, Color.White);
            pnlClassInfoCard.Paint += (s, e) => DrawShadow(s, e, 12, Color.White);
            pnlAssignGvcn.Paint += (s, e) => DrawShadow(s, e, 8, Color.FromArgb(248, 249, 250), true);

            Button[] buttons = { btnThemHS, btnSuaHS, btnAssignGvcn };
            foreach (var btn in buttons)
            {
                btn.BackColor = Color.FromArgb(0, 123, 255);
                btn.ForeColor = Color.White;
                btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                btn.Size = new Size(150, 40);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
            }
            btnXoaHS.BackColor = Color.FromArgb(220, 53, 69);
            btnAssignGvcn.BackColor = Color.FromArgb(40, 167, 69);
        }

        private void StyleClassListGrid()
        {
            isProgrammaticChange = true;
            dgvLopHoc.DataSource = allLopHoc.DefaultView;
            if (dgvLopHoc.Columns["TenLop"] != null) dgvLopHoc.Columns["TenLop"].HeaderText = "DANH SÁCH LỚP HỌC";
            if (dgvLopHoc.Columns["MaLop"] != null) dgvLopHoc.Columns["MaLop"].Visible = false;
            if (dgvLopHoc.Columns["Khoi"] != null) dgvLopHoc.Columns["Khoi"].Visible = false;
            DatabaseHelper.StyleDataGridView(dgvLopHoc);
            dgvLopHoc.SelectionChanged -= dgvLopHoc_SelectionChanged;
            dgvLopHoc.SelectionChanged += dgvLopHoc_SelectionChanged;
            isProgrammaticChange = false;
        }

        // --- XỬ LÝ SỰ KIỆN ---

        private void cboKhoi_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedKhoi = cboKhoi.SelectedItem.ToString();
            DataView dv = allLopHoc.DefaultView;
            dv.RowFilter = (selectedKhoi == "Tất cả các khối") ? string.Empty : $"Khoi = '{selectedKhoi}'";
            ClearAllDetails();
        }

        private void dgvLopHoc_SelectionChanged(object sender, EventArgs e)
        {
            if (isProgrammaticChange || dgvLopHoc.CurrentRow == null) return;
            string selectedMaLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
            LoadClassDetails(selectedMaLop);
        }

        private void btnAssignGvcn_Click(object sender, EventArgs e)
        {
            if (dgvLopHoc.CurrentRow == null || cboGvcn.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn lớp và giáo viên để phân công.", "Thông tin thiếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                string maLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
                string maGV = cboGvcn.SelectedValue.ToString();
                DatabaseHelper.UpdateGvcnForLop(maLop, maGV);
                MessageBox.Show("Phân công giáo viên chủ nhiệm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUnassignedGvcn();
                LoadClassDetails(maLop);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi phân công GVCN: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvPhanCong_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvPhanCong.CurrentCell.ColumnIndex == dgvPhanCong.Columns["AssignTeacherColumn"].Index && e.Control is ComboBox comboBox)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                string maMon = dgvPhanCong.CurrentRow.Cells["MaMon"].Value.ToString();

                // Lọc danh sách giáo viên theo MaMon của dòng hiện tại
                DataView dv = new DataView(allGiaoVien)
                {
                    RowFilter = string.IsNullOrEmpty(maMon) ? "MaMon IS NULL" : $"MaMon = '{maMon}'"
                };

                DataTable filteredTeachers = dv.ToTable();
                // Thêm lựa chọn "Trống" để gỡ phân công
                DataRow emptyRow = filteredTeachers.NewRow();
                emptyRow["Ten"] = "(Trống)";
                emptyRow["MaGV"] = DBNull.Value;
                filteredTeachers.Rows.InsertAt(emptyRow, 0);

                comboBox.DataSource = filteredTeachers;
                comboBox.DisplayMember = "Ten";
                comboBox.ValueMember = "MaGV";
            }
        }

        private void dgvPhanCong_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isProgrammaticChange || e.RowIndex < 0 || dgvPhanCong.Columns[e.ColumnIndex].Name != "AssignTeacherColumn") return;
            try
            {
                string maLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
                string maMon = dgvPhanCong.Rows[e.RowIndex].Cells["MaMon"].Value.ToString();
                object newMaGVObj = dgvPhanCong.Rows[e.RowIndex].Cells["AssignTeacherColumn"].Value;
                string newMaGV = (newMaGVObj == DBNull.Value || newMaGVObj == null) ? null : newMaGVObj.ToString();

                DatabaseHelper.UpdatePhanCong(maLop, maMon, newMaGV);

                var gv = allGiaoVien.AsEnumerable().FirstOrDefault(r => r.Field<string>("MaGV") == newMaGV);
                isProgrammaticChange = true;
                dgvPhanCong.Rows[e.RowIndex].Cells["TenGV"].Value = gv != null ? gv.Field<string>("Ten") : "Chưa phân công";
                isProgrammaticChange = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật phân công: " + ex.Message);
            }
        }

        private void dgvPhanCong_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show($"Lỗi dữ liệu tại dòng {e.RowIndex + 1}, cột '{dgvPhanCong.Columns[e.ColumnIndex].HeaderText}'.\nChi tiết: {e.Exception.Message}",
                "Lỗi hiển thị dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.ThrowException = false;
        }

        // --- CÁC HÀM TẢI DỮ LIỆU & TÙY CHỈNH ---

        private void LoadClassDetails(string maLop)
        {
            isProgrammaticChange = true;
            try
            {
                DataRow classInfo = DatabaseHelper.GetLopHocDetails(maLop);
                if (classInfo != null)
                {
                    lblTenLop.Text = classInfo["TenLop"].ToString();
                    lblNamHoc.Text = $"🗓️ Năm học: {classInfo["NamHoc"]}";
                    lblGVCN.Text = $"👤 GVCN: {classInfo["TenGVCN"]}";
                    lblSiSo.Text = $"👥 Sĩ số: {classInfo["SiSo"]}";
                    if (classInfo["TenGVCN"].ToString() == "Chưa có")
                    {
                        pnlAssignGvcn.Visible = true;
                        lblGVCN.ForeColor = Color.FromArgb(220, 53, 69);
                    }
                    else
                    {
                        pnlAssignGvcn.Visible = false;
                        lblGVCN.ForeColor = Color.FromArgb(108, 117, 125);
                    }
                }
                dgvHocSinh.DataSource = DatabaseHelper.GetHocSinhByLop(maLop);
                CustomizeStudentGrid();
                dgvPhanCong.DataSource = DatabaseHelper.GetPhanCongGiangDayByLop(maLop);
                CustomizeAssignmentGrid();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi khi tải chi tiết lớp: " + ex.Message); }
            finally { isProgrammaticChange = false; }
        }

        private void ClearAllDetails()
        {
            lblTenLop.Text = "Chọn lớp để xem thông tin";
            lblNamHoc.Text = "🗓️ Năm học: -";
            lblGVCN.Text = "👤 GVCN: -";
            lblGVCN.ForeColor = Color.FromArgb(108, 117, 125);
            lblSiSo.Text = "👥 Sĩ số: -";
            dgvHocSinh.DataSource = null;
            dgvPhanCong.DataSource = null;
            pnlAssignGvcn.Visible = false;
        }

        private void CustomizeStudentGrid()
        {
            if (dgvHocSinh.DataSource == null || dgvHocSinh.Columns.Count == 0) return;
            dgvHocSinh.Columns["MaHS"].HeaderText = "Mã Học Sinh";
            dgvHocSinh.Columns["HoTen"].HeaderText = "Họ và Tên";
            dgvHocSinh.Columns["GioiTinh"].HeaderText = "Giới Tính";
            dgvHocSinh.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
            dgvHocSinh.Columns["DiaChi"].HeaderText = "Địa Chỉ";
            dgvHocSinh.Columns["DanToc"].HeaderText = "Dân Tộc";
            dgvHocSinh.Columns["SDTPhuHuynh"].HeaderText = "SĐT Phụ Huynh";
        }

        private void CustomizeAssignmentGrid()
        {
            if (dgvPhanCong.DataSource == null || dgvPhanCong.Columns.Count == 0) return;
            if (dgvPhanCong.Columns.Contains("AssignTeacherColumn")) dgvPhanCong.Columns.Remove("AssignTeacherColumn");

            dgvPhanCong.Columns["MaMon"].Visible = false;
            dgvPhanCong.Columns["MaGV"].Visible = false;
            dgvPhanCong.Columns["TenMon"].HeaderText = "Môn Học";
            dgvPhanCong.Columns["TenMon"].ReadOnly = true;
            dgvPhanCong.Columns["TenGV"].HeaderText = "Giáo Viên Hiện Tại";
            dgvPhanCong.Columns["TenGV"].ReadOnly = true;

            var comboBoxColumn = new DataGridViewComboBoxColumn
            {
                Name = "AssignTeacherColumn",
                HeaderText = "Phân Công Mới",
                DataSource = allGiaoVien.Copy(), // Gán datasource tổng để xác thực
                DisplayMember = "Ten",
                ValueMember = "MaGV",
                FlatStyle = FlatStyle.Flat
            };
            dgvPhanCong.Columns.Add(comboBoxColumn);

            foreach (DataGridViewRow row in dgvPhanCong.Rows)
            {
                row.Cells["AssignTeacherColumn"].Value = row.Cells["MaGV"].Value;
            }

            dgvPhanCong.EditingControlShowing -= dgvPhanCong_EditingControlShowing;
            dgvPhanCong.EditingControlShowing += dgvPhanCong_EditingControlShowing;
            dgvPhanCong.CellValueChanged -= dgvPhanCong_CellValueChanged;
            dgvPhanCong.CellValueChanged += dgvPhanCong_CellValueChanged;
        }

        // --- HÀM VẼ GIAO DIỆN ---
        private void DrawShadow(object sender, PaintEventArgs e, int radius, Color color, bool border = false)
        {
            Control control = sender as Control;
            if (control == null) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = CreateRoundedRect(control.ClientRectangle, radius))
            {
                using (SolidBrush brush = new SolidBrush(color)) { e.Graphics.FillPath(brush, path); }
                if (border)
                {
                    using (Pen pen = new Pen(Color.FromArgb(222, 226, 230))) { e.Graphics.DrawPath(pen, path); }
                }
            }
        }

        private GraphicsPath CreateRoundedRect(Rectangle r, int radius)
        {
            r.Width--; r.Height--;
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            if (d <= 0) { path.AddRectangle(r); return path; }
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}