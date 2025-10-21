using System;
using System.Collections.Generic; // Cần thêm cái này
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace N6
{
    // Lớp helper nhỏ để lưu MaHS trong CheckedListBox
    public class StudentItem
    {
        public string HoTen { get; set; }
        public string MaHS { get; set; }
        public override string ToString()
        {
            return HoTen;
        }
    }

    public partial class UC_QuanLyLopHocSinh : UserControl
    {
        private DataTable allLopHoc;
        private DataTable allGiaoVien;
        private bool isProgrammaticChange = false;

        // ### MỚI: Khai báo các control cho panel chuyển lớp ###
        private CheckedListBox clbHocSinhChuyen;
        private ComboBox cboLopMoi_Inline;
        private Button btnXacNhanChuyen;
        private Button btnHuyChuyen;

        public UC_QuanLyLopHocSinh()
        {
            InitializeComponent();

            AttachEventHandlers();

            // ### MỚI: Khởi tạo panel chuyển lớp ###
            InitializeChuyenLopPanel();
            // #####################################

            LoadInitialData();
            StyleControls();
        }

        // ### MỚI: Hàm khởi tạo các control trong pnlChuyenLop ###
        private void InitializeChuyenLopPanel()
        {
            // Kiểm tra xem pnlChuyenLop đã được thêm từ Designer chưa
            if (this.pnlChuyenLop == null)
            {
                // Nếu không, tạo một cái (dự phòng)
                this.pnlChuyenLop = new Panel
                {
                    Name = "pnlChuyenLop",
                    Dock = DockStyle.Right,
                    Width = 300,
                    Visible = false,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White,
                    Padding = new Padding(10)
                };
                this.Controls.Add(this.pnlChuyenLop);
            }

            // --- Tạo các controls ---
            var lblTitle = new Label { Text = "Chuyển Lớp Hàng Loạt", Dock = DockStyle.Top, Font = new Font("Segoe UI", 12F, FontStyle.Bold), Height = 30, ForeColor = Color.FromArgb(0, 123, 255) };
            var lblChonHS = new Label { Text = "1. Chọn học sinh cần chuyển:", Dock = DockStyle.Top, Font = new Font("Segoe UI", 9F), Height = 20 };

            clbHocSinhChuyen = new CheckedListBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F), BorderStyle = BorderStyle.FixedSingle };

            var lblChonLop = new Label { Text = "2. Chọn lớp chuyển đến:", Dock = DockStyle.Bottom, Font = new Font("Segoe UI", 9F), Height = 20 };

            cboLopMoi_Inline = new ComboBox { Dock = DockStyle.Bottom, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Height = 28 };

            var pnlButtons = new Panel { Dock = DockStyle.Bottom, Height = 40, Padding = new Padding(0, 5, 0, 0) };

            btnXacNhanChuyen = new Button { Text = "Xác nhận", Dock = DockStyle.Right, Width = 100, BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnHuyChuyen = new Button { Text = "Hủy", Dock = DockStyle.Right, Width = 80, FlatStyle = FlatStyle.Flat };

            pnlButtons.Controls.Add(btnXacNhanChuyen);
            pnlButtons.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 10 }); // Spacer
            pnlButtons.Controls.Add(btnHuyChuyen);

            // Gán sự kiện
            btnXacNhanChuyen.Click += new EventHandler(this.btnXacNhanChuyen_Click);
            btnHuyChuyen.Click += new EventHandler(this.btnHuyChuyen_Click);

            // Thêm controls vào panel (thứ tự quan trọng)
            this.pnlChuyenLop.Controls.Add(clbHocSinhChuyen); // Thêm listbox vào giữa
            this.pnlChuyenLop.Controls.Add(lblChonHS);
            this.pnlChuyenLop.Controls.Add(lblTitle);
            this.pnlChuyenLop.Controls.Add(pnlButtons); // Thêm panel nút ở dưới
            this.pnlChuyenLop.Controls.Add(cboLopMoi_Inline);
            this.pnlChuyenLop.Controls.Add(lblChonLop);
        }

        private void AttachEventHandlers()
        {
            // Sự kiện lọc Khối
            this.cboKhoi.SelectedIndexChanged -= new System.EventHandler(this.cboKhoi_SelectedIndexChanged);
            this.cboKhoi.SelectedIndexChanged += new System.EventHandler(this.cboKhoi_SelectedIndexChanged);

            // Sự kiện chọn Lớp
            this.dgvLopHoc.SelectionChanged -= new System.EventHandler(this.dgvLopHoc_SelectionChanged);
            this.dgvLopHoc.SelectionChanged += new System.EventHandler(this.dgvLopHoc_SelectionChanged);

            // Sự kiện các nút quản lý Học Sinh
            this.btnLuuHS.Click -= new System.EventHandler(this.btnLuuHS_Click);
            this.btnLuuHS.Click += new System.EventHandler(this.btnLuuHS_Click);

            this.btnXoaHS.Click -= new System.EventHandler(this.btnXoaHS_Click);
            this.btnXoaHS.Click += new System.EventHandler(this.btnXoaHS_Click);

            this.btnImportHS.Click -= new System.EventHandler(this.btnImportHS_Click);
            this.btnImportHS.Click += new System.EventHandler(this.btnImportHS_Click);

            // Sự kiện nút Phân công GVCN
            this.btnAssignGvcn.Click -= new System.EventHandler(this.btnAssignGvcn_Click);
            this.btnAssignGvcn.Click += new System.EventHandler(this.btnAssignGvcn_Click);

            // Sự kiện nút Import Phân công (Môn học)
            this.btnImportPhanCong.Click -= new System.EventHandler(this.btnImportPhanCong_Click);
            this.btnImportPhanCong.Click += new System.EventHandler(this.btnImportPhanCong_Click);

            // Sự kiện nút Chuyển Lớp (NÚT MỚI)
            if (this.btnChuyenLop != null)
            {
                this.btnChuyenLop.Click -= new System.EventHandler(this.btnChuyenLop_Click);
                this.btnChuyenLop.Click += new System.EventHandler(this.btnChuyenLop_Click);
            }
        }


        #region SETUP GIAO DIỆN & DỮ LIỆU

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

            Button[] buttons = { btnThemHS, btnLuuHS, btnAssignGvcn, btnImportHS, btnImportPhanCong };
            foreach (var btn in buttons)
            {
                if (btn == null) continue; // Bỏ qua nếu nút không tồn tại
                btn.BackColor = Color.FromArgb(0, 123, 255);
                btn.ForeColor = Color.White;
                btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
            }

            btnXoaHS.BackColor = Color.FromArgb(220, 53, 69);
            btnXoaHS.ForeColor = Color.White;
            btnXoaHS.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnXoaHS.FlatStyle = FlatStyle.Flat;
            btnXoaHS.FlatAppearance.BorderSize = 0;

            if (this.btnChuyenLop != null)
            {
                btnChuyenLop.BackColor = Color.FromArgb(253, 126, 20); // Màu cam
                btnChuyenLop.ForeColor = Color.White;
                btnChuyenLop.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                btnChuyenLop.FlatStyle = FlatStyle.Flat;
                btnChuyenLop.FlatAppearance.BorderSize = 0;
                btnChuyenLop.Text = "Chuyển Lớp";
            }

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

            isProgrammaticChange = false;
        }

        #endregion

        #region XỬ LÝ SỰ KIỆN CHUNG

        private void cboKhoi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboKhoi.SelectedItem == null) return;
            string selectedKhoi = cboKhoi.SelectedItem.ToString();
            DataView dv = allLopHoc.DefaultView;
            dv.RowFilter = (selectedKhoi == "Tất cả các khối") ? string.Empty : $"Khoi = '{selectedKhoi}'";
            ClearAllDetails();
        }

        private void dgvLopHoc_SelectionChanged(object sender, EventArgs e)
        {
            if (isProgrammaticChange || dgvLopHoc.CurrentRow == null) return;

            // ### SỬA LỖI: Khi đổi lớp, phải ẩn panel chuyển lớp
            if (pnlChuyenLop.Visible)
            {
                pnlChuyenLop.Visible = false;
            }
            // ###############################################

            string selectedMaLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
            LoadClassDetails(selectedMaLop);
        }

        #endregion

        #region HỌC SINH: THÊM, SỬA, XÓA, IMPORT, CHUYỂN LỚP

        private void btnXoaHS_Click(object sender, EventArgs e)
        {
            if (dgvHocSinh.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một học sinh để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maHS = dgvHocSinh.CurrentRow.Cells["MaHS"].Value.ToString();
            string tenHS = dgvHocSinh.CurrentRow.Cells["HoTen"].Value.ToString();

            DialogResult confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa học sinh '{tenHS}' không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteHocSinh(maHS);
                    MessageBox.Show("Xóa học sinh thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    string selectedMaLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
                    LoadClassDetails(selectedMaLop); // Tải lại chi tiết để cập nhật sĩ số
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa học sinh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLuuHS_Click(object sender, EventArgs e)
        {
            DataTable dt = (dgvHocSinh.DataSource as DataTable);
            if (dt == null) return;

            DataTable changes = dt.GetChanges(DataRowState.Modified);

            if (changes == null || changes.Rows.Count == 0)
            {
                MessageBox.Show("Không có thay đổi nào để lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int successCount = 0;
            try
            {
                foreach (DataRow row in changes.Rows)
                {
                    string maHS = row["MaHS"].ToString();
                    string hoTen = row["HoTen"].ToString();
                    DateTime ngaySinh = Convert.ToDateTime(row["NgaySinh"]);
                    string gioiTinh = row["GioiTinh"].ToString();
                    string sdtPH = row["SDTPhuHuynh"].ToString();
                    string diaChi = row["DiaChi"].ToString();
                    string danToc = row["DanToc"].ToString();

                    DatabaseHelper.UpdateHocSinh(maHS, hoTen, ngaySinh, gioiTinh, sdtPH, diaChi, danToc);
                    successCount++;
                }

                dt.AcceptChanges();
                MessageBox.Show($"Đã lưu thành công {successCount} thay đổi!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu thay đổi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dt.RejectChanges();
            }
        }

        private void btnImportHS_Click(object sender, EventArgs e)
        {
            string selectedKhoi = (cboKhoi.SelectedItem ?? "Tất cả các khối").ToString();
            string maLop = dgvLopHoc.CurrentRow?.Cells["MaLop"].Value.ToString();
            string tenLop = dgvLopHoc.CurrentRow?.Cells["TenLop"].Value.ToString();

            using (var optionForm = new Form { Text = "Chọn kiểu Import", Size = new Size(400, 180), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false })
            {
                var btnTheoLop = new Button { Text = $"Import cho lớp ({tenLop})", Dock = DockStyle.Top, Height = 40, DialogResult = DialogResult.OK, Font = new Font("Segoe UI", 10F) };
                var btnTheoKhoi = new Button { Text = $"Import cho khối ({selectedKhoi})", Dock = DockStyle.Top, Height = 40, DialogResult = DialogResult.Yes, Font = new Font("Segoe UI", 10F) };
                var btnCancel = new Button { Text = "Hủy", Dock = DockStyle.Bottom, Height = 40, DialogResult = DialogResult.Cancel, Font = new Font("Segoe UI", 10F) };

                btnTheoLop.Enabled = (maLop != null);
                btnTheoKhoi.Enabled = (selectedKhoi != "Tất cả các khối");
                if (btnTheoKhoi.Enabled == false) btnTheoKhoi.Text = "Import cho khối (Hãy chọn 1 khối)";

                optionForm.Controls.AddRange(new Control[] { btnTheoKhoi, btnTheoLop, btnCancel });
                optionForm.CancelButton = btnCancel;

                DialogResult choice = optionForm.ShowDialog();
                if (choice == DialogResult.Cancel) return;

                frmImportExcel.ImportType type = (choice == DialogResult.OK) ? frmImportExcel.ImportType.HocSinhTheoLop : frmImportExcel.ImportType.HocSinhTheoKhoi;

                using (var importForm = new frmImportExcel(type, maLop, tenLop, selectedKhoi))
                {
                    if (importForm.ShowDialog() == DialogResult.OK)
                    {
                        if (maLop != null)
                        {
                            LoadClassDetails(maLop); // Cập nhật lại sĩ số
                        }
                    }
                }
            }
        }

        // ### THAY THẾ HÀM CŨ BẰNG HÀM MỚI NÀY ###
        private void btnChuyenLop_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra đã chọn lớp chưa
            if (dgvLopHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một lớp trước khi thực hiện chuyển lớp.", "Chưa chọn lớp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Lấy thông tin lớp hiện tại
            string maLopHienTai = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
            DataTable dtHocSinh = dgvHocSinh.DataSource as DataTable;

            if (dtHocSinh == null || dtHocSinh.Rows.Count == 0)
            {
                MessageBox.Show("Lớp hiện tại không có học sinh nào để chuyển.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 3. Lọc danh sách lớp đến (loại bỏ lớp hiện tại)
            var filteredRows = allLopHoc.AsEnumerable()
                                .Where(row => row.Field<string>("MaLop") != maLopHienTai);

            if (!filteredRows.Any())
            {
                MessageBox.Show("Không có lớp nào khác để chuyển đến.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataTable dtLopDen = filteredRows.CopyToDataTable();

            // 4. Đổ dữ liệu vào các control trong panel

            // Đổ dữ liệu Học sinh vào CheckedListBox
            clbHocSinhChuyen.Items.Clear();
            foreach (DataRow row in dtHocSinh.Rows)
            {
                clbHocSinhChuyen.Items.Add(new StudentItem
                {
                    HoTen = row["HoTen"].ToString(),
                    MaHS = row["MaHS"].ToString()
                });
            }

            // Đổ dữ liệu Lớp đến vào ComboBox
            cboLopMoi_Inline.DataSource = dtLopDen;
            cboLopMoi_Inline.DisplayMember = "TenLop";
            cboLopMoi_Inline.ValueMember = "MaLop";
            cboLopMoi_Inline.SelectedIndex = 0;

            // 5. Hiển thị panel
            pnlChuyenLop.Visible = true;
            pnlChuyenLop.BringToFront();
        }

        // ### MỚI: Sự kiện cho nút "Hủy" trên panel
        private void btnHuyChuyen_Click(object sender, EventArgs e)
        {
            pnlChuyenLop.Visible = false;
            // Xóa các lựa chọn
            for (int i = 0; i < clbHocSinhChuyen.Items.Count; i++)
            {
                clbHocSinhChuyen.SetItemChecked(i, false);
            }
        }

        // ### MỚI: Sự kiện cho nút "Xác nhận" trên panel
        private void btnXacNhanChuyen_Click(object sender, EventArgs e)
        {
            // 1. Lấy danh sách học sinh được chọn
            List<string> maHocSinhList = new List<string>();
            foreach (object itemChecked in clbHocSinhChuyen.CheckedItems)
            {
                if (itemChecked is StudentItem hs)
                {
                    maHocSinhList.Add(hs.MaHS);
                }
            }

            if (maHocSinhList.Count == 0)
            {
                MessageBox.Show("Bạn chưa chọn học sinh nào để chuyển.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Lấy lớp chuyển đến
            if (cboLopMoi_Inline.SelectedValue == null)
            {
                MessageBox.Show("Lỗi: Không xác định được lớp chuyển đến.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string maLopMoi = cboLopMoi_Inline.SelectedValue.ToString();
            string tenLopMoi = cboLopMoi_Inline.Text;
            string maLopHienTai = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();

            // 3. Thực hiện chuyển
            try
            {
                int soHocSinhDaChuyen = DatabaseHelper.UpdateHocSinhLop_Multi(maHocSinhList, maLopMoi);

                MessageBox.Show($"Đã chuyển thành công {soHocSinhDaChuyen} học sinh sang lớp '{tenLopMoi}'.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 4. Ẩn panel và tải lại dữ liệu
                pnlChuyenLop.Visible = false;
                LoadClassDetails(maLopHienTai); // Tải lại lớp hiện tại (sĩ số giảm)
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thực hiện chuyển lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #endregion

        #region PHÂN CÔNG GIẢNG DẠY: GVCN, MÔN HỌC, IMPORT

        private void btnAssignGvcn_Click(object sender, EventArgs e)
        {
            if (dgvLopHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn lớp.", "Thông tin thiếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboGvcn.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn giáo viên (hoặc 'Trống') để phân công.", "Thông tin thiếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string maLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
                string maGV = cboGvcn.SelectedValue.ToString();

                DatabaseHelper.UpdateGvcnForLop(maLop, maGV);

                MessageBox.Show("Cập nhật giáo viên chủ nhiệm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // --- BẮT ĐẦU SỬA LỖI ---

                // 1. Lưu lại bộ lọc khối hiện tại
                string selectedKhoi = (cboKhoi.SelectedItem ?? "Tất cả các khối").ToString();

                // 2. Tải lại cache (tạo ra bảng mới)
                allLopHoc = DatabaseHelper.GetAllLopHoc();

                // 3. (FIX) Gán lại DataSource của Grid vào DefaultView của Bảng MỚI
                isProgrammaticChange = true;
                dgvLopHoc.DataSource = allLopHoc.DefaultView;
                isProgrammaticChange = false;

                // 4. (FIX) Áp dụng lại bộ lọc khối cho Bảng Mới
                allLopHoc.DefaultView.RowFilter = (selectedKhoi == "Tất cả các khối") ? string.Empty : $"Khoi = '{selectedKhoi}'";

                // 5. Tải lại chi tiết lớp
                LoadClassDetails(maLop);

                // --- KẾT THÚC SỬA LỖI ---
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật GVCN: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvPhanCong_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvPhanCong.CurrentCell.ColumnIndex == dgvPhanCong.Columns["AssignTeacherColumn"].Index && e.Control is ComboBox comboBox)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

                string tenMon = dgvPhanCong.CurrentRow.Cells["TenMon"].Value.ToString();

                DataView dv = new DataView(allGiaoVien);

                if (string.IsNullOrEmpty(tenMon))
                {
                    dv.RowFilter = "CacMonDay = 'Chưa có môn'";
                }
                else
                {
                    string safeTenMon = tenMon.Replace("'", "''");
                    dv.RowFilter = $"CacMonDay LIKE '%{safeTenMon}%'";
                }

                DataTable filteredTeachers = dv.ToTable();

                DataRow emptyRow = filteredTeachers.NewRow();
                emptyRow["Ten"] = "(Trống)";
                emptyRow["MaGV"] = "";

                foreach (DataColumn col in filteredTeachers.Columns)
                {
                    if (emptyRow.IsNull(col) && col.DataType == typeof(string))
                    {
                        emptyRow[col] = "";
                    }
                }
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
                dgvPhanCong.Rows[e.RowIndex].Cells["TenGV"].Value = gv != null ? gv.Field<string>("Ten") : "Trống";
                isProgrammaticChange = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật phân công: " + ex.Message);
            }
        }

        private void dgvPhanCong_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void btnImportPhanCong_Click(object sender, EventArgs e)
        {
            if (dgvLopHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một lớp để import phân công.", "Chưa chọn lớp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string maLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
            string tenLop = dgvLopHoc.CurrentRow.Cells["TenLop"].Value.ToString();

            using (var importForm = new frmImportExcel(frmImportExcel.ImportType.PhanCong, maLop, tenLop, null))
            {
                if (importForm.ShowDialog() == DialogResult.OK)
                {
                    dgvPhanCong.DataSource = DatabaseHelper.GetPhanCongGiangDayByLop(maLop);
                    CustomizeAssignmentGrid();
                }
            }
        }

        #endregion

        #region HÀM TẢI DỮ LIỆU & TÙY CHỈNH GIAO DIỆN

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

                    pnlAssignGvcn.Visible = true;

                    DataTable dtAvailableTeachers = DatabaseHelper.GetUnassignedHomeroomTeachers();

                    string currentMaGVCN = classInfo["MaGVCN"]?.ToString() ?? "";
                    string currentTenGVCN = classInfo["TenGVCN"]?.ToString() ?? "Chưa có";

                    DataRow emptyRow = dtAvailableTeachers.NewRow();
                    emptyRow["Ten"] = "(Trống)";
                    emptyRow["MaGV"] = "";
                    dtAvailableTeachers.Rows.InsertAt(emptyRow, 0);

                    if (!string.IsNullOrEmpty(currentMaGVCN))
                    {
                        bool exists = dtAvailableTeachers.AsEnumerable()
                                        .Any(r => r.Field<string>("MaGV") == currentMaGVCN);
                        if (!exists)
                        {
                            DataRow currentRow = dtAvailableTeachers.NewRow();
                            currentRow["Ten"] = currentTenGVCN;
                            currentRow["MaGV"] = currentMaGVCN;
                            dtAvailableTeachers.Rows.Add(currentRow);
                        }
                    }

                    cboGvcn.DataSource = dtAvailableTeachers;
                    cboGvcn.DisplayMember = "Ten";
                    cboGvcn.ValueMember = "MaGV";

                    cboGvcn.SelectedValue = currentMaGVCN;

                    if (currentTenGVCN == "Chưa có")
                    {
                        lblGVCN.ForeColor = Color.FromArgb(220, 53, 69);
                    }
                    else
                    {
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

            // ### MỚI: Ẩn panel chuyển lớp khi clear
            if (pnlChuyenLop != null)
            {
                pnlChuyenLop.Visible = false;
            }
        }

        private void CustomizeStudentGrid()
        {
            if (dgvHocSinh.DataSource == null || dgvHocSinh.Columns.Count == 0) return;

            dgvHocSinh.ReadOnly = false;

            if (dgvHocSinh.Columns.Contains("STT"))
            {
                var col = dgvHocSinh.Columns["STT"];
                col.HeaderText = "STT";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                col.DisplayIndex = 0;
                col.ReadOnly = true;
            }

            dgvHocSinh.Columns["MaHS"].HeaderText = "Mã Học Sinh";
            dgvHocSinh.Columns["MaHS"].ReadOnly = true;

            dgvHocSinh.Columns["HoTen"].HeaderText = "Họ và Tên";
            if (dgvHocSinh.Columns.Contains("STT"))
                dgvHocSinh.Columns["HoTen"].DisplayIndex = 1;

            dgvHocSinh.Columns["GioiTinh"].HeaderText = "Giới Tính";
            dgvHocSinh.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
            dgvHocSinh.Columns["DiaChi"].HeaderText = "Địa Chỉ";
            dgvHocSinh.Columns["DanToc"].HeaderText = "Dân Tộc";
            dgvHocSinh.Columns["SDTPhuHuynh"].HeaderText = "SĐT Phụ Huynh";

            dgvHocSinh.Columns["MaHS"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvHocSinh.Columns["HoTen"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvHocSinh.Columns["GioiTinh"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvHocSinh.Columns["NgaySinh"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvHocSinh.Columns["DanToc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvHocSinh.Columns["SDTPhuHuynh"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvHocSinh.Columns["DiaChi"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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

            DataTable dtColumnSource = allGiaoVien.Copy();
            DataRow emptyRowCol = dtColumnSource.NewRow();
            emptyRowCol["Ten"] = "(Trống)";
            emptyRowCol["MaGV"] = "";

            foreach (DataColumn col in dtColumnSource.Columns)
            {
                if (emptyRowCol.IsNull(col) && col.DataType == typeof(string))
                {
                    emptyRowCol[col] = "";
                }
            }
            dtColumnSource.Rows.InsertAt(emptyRowCol, 0);

            var comboBoxColumn = new DataGridViewComboBoxColumn
            {
                Name = "AssignTeacherColumn",
                HeaderText = "Phân Công Mới",
                DataSource = dtColumnSource,
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

        #endregion

        #region HÀM VẼ GIAO DIỆN PHỤ

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

        #endregion
    }
}