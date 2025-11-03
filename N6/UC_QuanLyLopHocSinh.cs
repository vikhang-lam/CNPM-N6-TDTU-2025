using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics; // Thêm

namespace N6
{
    /// <summary>
    /// Lớp helper nhỏ để lưu trữ MaHS và HoTen trong CheckedListBox.
    /// </summary>
    public class StudentItem
    {
        public string HoTen { get; set; }
        public string MaHS { get; set; }
        public override string ToString()
        {
            return HoTen;
        }
    }

    /// <summary>
    /// UserControl quản lý nghiệp vụ Lớp học (chi tiết học sinh, phân công giảng dạy, chuyển lớp).
    /// </summary>
    public partial class UC_QuanLyLopHocSinh : UserControl
    {
        private DataTable allLopHoc;
        private DataTable allGiaoVien;
        private bool isProgrammaticChange = false; // Cờ để chặn các sự kiện thay đổi lồng nhau

        // Controls cho panel chuyển lớp (được tạo động)
        private CheckedListBox clbHocSinhChuyen;
        private ComboBox cboLopMoi_Inline;
        private Button btnXacNhanChuyen;
        private Button btnHuyChuyen;

        // Biến lưu trữ sự kiện động để gỡ bỏ (Dispose)
        private EventHandler btnXacNhanChuyenClickHandler;
        private EventHandler btnHuyChuyenClickHandler;
        private PaintEventHandler pnlFilterPaintHandler;
        private PaintEventHandler pnlClassInfoCardPaintHandler;
        private PaintEventHandler pnlAssignGvcnPaintHandler;
        private DataGridViewEditingControlShowingEventHandler dgvEditingControlShowingHandler;
        private DataGridViewCellEventHandler dgvCellValueChangedHandler;
        private DataGridViewDataErrorEventHandler dgvDataErrorHandler;


        public UC_QuanLyLopHocSinh()
        {
            InitializeComponent();
            AttachEventHandlers();
            InitializeChuyenLopPanel();
            LoadInitialData();
            StyleControls();
        }

        /// <summary>
        /// Gán các sự kiện cho các control đã có trong Designer.
        /// </summary>
        private void AttachEventHandlers()
        {
            this.cboKhoi.SelectedIndexChanged += new System.EventHandler(this.cboKhoi_SelectedIndexChanged);
            this.dgvLopHoc.SelectionChanged += new System.EventHandler(this.dgvLopHoc_SelectionChanged);
            this.btnLuuHS.Click += new System.EventHandler(this.btnLuuHS_Click);
            this.btnXoaHS.Click += new System.EventHandler(this.btnXoaHS_Click);
            this.btnImportHS.Click += new System.EventHandler(this.btnImportHS_Click);
            this.btnAssignGvcn.Click += new System.EventHandler(this.btnAssignGvcn_Click);
            this.btnImportPhanCong.Click += new System.EventHandler(this.btnImportPhanCong_Click);

            if (this.btnChuyenLop != null)
            {
                this.btnChuyenLop.Click += new System.EventHandler(this.btnChuyenLop_Click);
            }
        }

        /// <summary>
        /// Khởi tạo các control động bên trong pnlChuyenLop.
        /// </summary>
        private void InitializeChuyenLopPanel()
        {
            // Kiểm tra nếu pnlChuyenLop không tồn tại (an toàn)
            if (this.pnlChuyenLop == null)
            {
                Debug.WriteLine("Lỗi: pnlChuyenLop chưa được khởi tạo trong Designer.");
                return;
            }

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

            // Gán sự kiện (và lưu handler để Dispose)
            btnXacNhanChuyenClickHandler = new EventHandler(this.btnXacNhanChuyen_Click);
            btnHuyChuyenClickHandler = new EventHandler(this.btnHuyChuyen_Click);
            btnXacNhanChuyen.Click += btnXacNhanChuyenClickHandler;
            btnHuyChuyen.Click += btnHuyChuyenClickHandler;

            // Thêm controls vào panel (thứ tự quan trọng)
            this.pnlChuyenLop.Controls.Add(clbHocSinhChuyen);
            this.pnlChuyenLop.Controls.Add(lblChonHS);
            this.pnlChuyenLop.Controls.Add(lblTitle);
            this.pnlChuyenLop.Controls.Add(pnlButtons);
            this.pnlChuyenLop.Controls.Add(cboLopMoi_Inline);
            this.pnlChuyenLop.Controls.Add(lblChonLop);
        }

        #region SETUP GIAO DIỆN & DỮ LIỆU

        /// <summary>
        /// Tải dữ liệu ban đầu (cache Lớp, GV) và cài đặt giao diện.
        /// </summary>
        private void LoadInitialData()
        {
            try
            {
                allLopHoc = DatabaseHelper.GetAllClasses();
                allGiaoVien = DatabaseHelper.GetAllTeachers();

                var khoiList = allLopHoc.AsEnumerable()
                                        .Select(row => row.Field<string>("Khoi"))
                                        .Distinct()
                                        .OrderBy(k => k)
                                        .ToList();
                khoiList.Insert(0, "Tất cả các khối");
                cboKhoi.DataSource = khoiList;

                LoadUnassignedGvcn();

                // Style các DataGridView động (phải gọi trước khi gán sự kiện)
                StyleDataGridView(dgvHocSinh);
                StyleDataGridView(dgvPhanCong);
                StyleClassListGrid(); // Style cho dgvLopHoc (từ Designer)

                // Gán sự kiện DataError (để gỡ trong Dispose)
                dgvDataErrorHandler = new DataGridViewDataErrorEventHandler(dgvPhanCong_DataError);
                dgvPhanCong.DataError += dgvDataErrorHandler;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu ban đầu: " + ex.Message);
            }
        }

        /// <summary>
        /// Tải danh sách GV chưa làm chủ nhiệm vào ComboBox.
        /// </summary>
        private void LoadUnassignedGvcn()
        {
            cboGvcn.DataSource = DatabaseHelper.GetUnassignedHomeroomTeachers();
            cboGvcn.DisplayMember = "Ten";
            cboGvcn.ValueMember = "MaGV";
            cboGvcn.SelectedIndex = -1;
            cboGvcn.Text = "Chọn giáo viên...";
        }

        /// <summary>
        /// Áp dụng style cho các panel và nút bấm.
        /// </summary>
        private void StyleControls()
        {
            // Gán sự kiện Paint (để gỡ trong Dispose)
            pnlFilterPaintHandler = (s, e) => DrawShadow(s, e, 12, Color.White);
            pnlClassInfoCardPaintHandler = (s, e) => DrawShadow(s, e, 12, Color.White);
            pnlAssignGvcnPaintHandler = (s, e) => DrawShadow(s, e, 8, Color.FromArgb(248, 249, 250), true);

            pnlFilter.Paint += pnlFilterPaintHandler;
            pnlClassInfoCard.Paint += pnlClassInfoCardPaintHandler;
            pnlAssignGvcn.Paint += pnlAssignGvcnPaintHandler;

            // Style các nút
            Button[] buttons = { btnThemHS, btnLuuHS, btnAssignGvcn, btnImportHS, btnImportPhanCong };
            foreach (var btn in buttons)
            {
                if (btn == null) continue;
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

            btnAssignGvcn.BackColor = Color.FromArgb(40, 167, 69); // Xanh lá
        }

        /// <summary>
        /// Áp dụng style cho lưới danh sách lớp (dgvLopHoc).
        /// </summary>
        private void StyleClassListGrid()
        {
            isProgrammaticChange = true;
            dgvLopHoc.DataSource = allLopHoc.DefaultView;
            if (dgvLopHoc.Columns["TenLop"] != null) dgvLopHoc.Columns["TenLop"].HeaderText = "DANH SÁCH LỚP HỌC";
            if (dgvLopHoc.Columns["MaLop"] != null) dgvLopHoc.Columns["MaLop"].Visible = false;
            if (dgvLopHoc.Columns["Khoi"] != null) dgvLopHoc.Columns["Khoi"].Visible = false;
            StyleDataGridView(dgvLopHoc); // Áp dụng style chung
            isProgrammaticChange = false;
        }

        #endregion

        #region XỬ LÝ SỰ KIỆN CHUNG

        /// <summary>
        /// Lọc danh sách lớp theo khối.
        /// </summary>
        private void cboKhoi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboKhoi.SelectedItem == null) return;
            string selectedKhoi = cboKhoi.SelectedItem.ToString();
            DataView dv = allLopHoc.DefaultView;
            dv.RowFilter = (selectedKhoi == "Tất cả các khối") ? string.Empty : $"Khoi = '{selectedKhoi}'";
            ClearAllDetails();
        }

        /// <summary>
        /// Tải chi tiết lớp khi chọn một lớp trong lưới.
        /// </summary>
        private void dgvLopHoc_SelectionChanged(object sender, EventArgs e)
        {
            if (isProgrammaticChange || dgvLopHoc.CurrentRow == null) return;

            // Khi đổi lớp, ẩn panel chuyển lớp
            if (pnlChuyenLop.Visible)
            {
                pnlChuyenLop.Visible = false;
            }

            string selectedMaLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
            LoadClassDetails(selectedMaLop);
        }

        #endregion

        #region HỌC SINH: THÊM, SỬA, XÓA, IMPORT, CHUYỂN LỚP

        /// <summary>
        /// Xử lý nút Xóa Học Sinh.
        /// </summary>
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
                    DatabaseHelper.DeleteStudent(maHS);
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

        /// <summary>
        /// Xử lý nút Lưu thay đổi (inline) trên lưới Học Sinh.
        /// </summary>
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

                    DatabaseHelper.UpdateStudent(maHS, hoTen, ngaySinh, gioiTinh, sdtPH, diaChi, danToc);
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

        /// <summary>
        /// Xử lý nút Import Học Sinh (mở dialog chọn kiểu Import).
        /// </summary>
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

        /// <summary>
        /// Hiển thị panel "Chuyển Lớp Hàng Loạt".
        /// </summary>
        private void btnChuyenLop_Click(object sender, EventArgs e)
        {
            if (dgvLopHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một lớp trước khi thực hiện chuyển lớp.", "Chưa chọn lớp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maLopHienTai = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
            DataTable dtHocSinh = dgvHocSinh.DataSource as DataTable;

            if (dtHocSinh == null || dtHocSinh.Rows.Count == 0)
            {
                MessageBox.Show("Lớp hiện tại không có học sinh nào để chuyển.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Lọc danh sách lớp đến (loại bỏ lớp hiện tại)
            var filteredRows = allLopHoc.AsEnumerable()
                                .Where(row => row.Field<string>("MaLop") != maLopHienTai);

            if (!filteredRows.Any())
            {
                MessageBox.Show("Không có lớp nào khác để chuyển đến.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataTable dtLopDen = filteredRows.CopyToDataTable();

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

            // Hiển thị panel
            pnlChuyenLop.Visible = true;
            pnlChuyenLop.BringToFront();
        }

        /// <summary>
        /// Xử lý nút "Hủy" trên panel chuyển lớp.
        /// </summary>
        private void btnHuyChuyen_Click(object sender, EventArgs e)
        {
            pnlChuyenLop.Visible = false;
            // Xóa các lựa chọn
            for (int i = 0; i < clbHocSinhChuyen.Items.Count; i++)
            {
                clbHocSinhChuyen.SetItemChecked(i, false);
            }
        }

        /// <summary>
        /// Xử lý nút "Xác nhận" trên panel chuyển lớp (gọi SP).
        /// </summary>
        private void btnXacNhanChuyen_Click(object sender, EventArgs e)
        {
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

            if (cboLopMoi_Inline.SelectedValue == null)
            {
                MessageBox.Show("Lỗi: Không xác định được lớp chuyển đến.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string maLopMoi = cboLopMoi_Inline.SelectedValue.ToString();
            string tenLopMoi = cboLopMoi_Inline.Text;
            string maLopHienTai = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();

            try
            {
                int soHocSinhDaChuyen = DatabaseHelper.UpdateStudentClass_Multi(maHocSinhList, maLopMoi);

                MessageBox.Show($"Đã chuyển thành công {soHocSinhDaChuyen} học sinh sang lớp '{tenLopMoi}'.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        /// <summary>
        /// Xử lý nút Gán GVCN.
        /// </summary>
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

                DatabaseHelper.UpdateHomeroomTeacherForClass(maLop, maGV);

                MessageBox.Show("Cập nhật giáo viên chủ nhiệm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Sửa lỗi: Tải lại cache và áp dụng lại filter
                string selectedKhoi = (cboKhoi.SelectedItem ?? "Tất cả các khối").ToString();
                allLopHoc = DatabaseHelper.GetAllClasses();
                isProgrammaticChange = true;
                dgvLopHoc.DataSource = allLopHoc.DefaultView;
                isProgrammaticChange = false;
                allLopHoc.DefaultView.RowFilter = (selectedKhoi == "Tất cả các khối") ? string.Empty : $"Khoi = '{selectedKhoi}'";

                LoadClassDetails(maLop);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật GVCN: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý khi ComboBox trong lưới Phân Công được hiển thị.
        /// </summary>
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
                    // Lọc GV dựa trên môn học
                    string safeTenMon = tenMon.Replace("'", "''");
                    dv.RowFilter = $"CacMonDay LIKE '%{safeTenMon}%'";
                }

                DataTable filteredTeachers = dv.ToTable();

                // Thêm dòng "(Trống)"
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

        /// <summary>
        /// Xử lý khi giá trị trong ComboBox (lưới Phân Công) thay đổi -> Lưu ngay lập tức.
        /// </summary>
        private void dgvPhanCong_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isProgrammaticChange || e.RowIndex < 0 || dgvPhanCong.Columns[e.ColumnIndex].Name != "AssignTeacherColumn") return;
            try
            {
                string maLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
                string maMon = dgvPhanCong.Rows[e.RowIndex].Cells["MaMon"].Value.ToString();
                object newMaGVObj = dgvPhanCong.Rows[e.RowIndex].Cells["AssignTeacherColumn"].Value;
                string newMaGV = (newMaGVObj == DBNull.Value || newMaGVObj == null) ? null : newMaGVObj.ToString();

                DatabaseHelper.UpdateTeachingAssignment(maLop, maMon, newMaGV);

                // Cập nhật lại cột "Tên GV hiện tại"
                var gv = allGiaoVien.AsEnumerable().FirstOrDefault(r => r.Field<string>("MaGV") == newMaGV);
                isProgrammaticChange = true;
                dgvPhanCong.Rows[e.RowIndex].Cells["TenGV"].Value = gv != null ? gv.Field<string>("Ten") : "(Trống)";
                isProgrammaticChange = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật phân công: " + ex.Message);
            }
        }

        /// <summary>
        /// Bắt lỗi DataError (ví dụ: khi ComboBox bị lỗi binding).
        /// </summary>
        private void dgvPhanCong_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false; // Ngăn không cho crash
        }

        /// <summary>
        /// Xử lý nút Import Phân công (Môn học).
        /// </summary>
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
                    // Tải lại lưới phân công
                    dgvPhanCong.DataSource = DatabaseHelper.GetTeachingAssignmentsByClass(maLop);
                    CustomizeAssignmentGrid();
                }
            }
        }

        #endregion

        #region HÀM TẢI DỮ LIỆU & TÙY CHỈNH GIAO DIỆN (Helper)

        /// <summary>
        /// Tải toàn bộ chi tiết của lớp (Info, HS, Phân công).
        /// </summary>
        private void LoadClassDetails(string maLop)
        {
            isProgrammaticChange = true;
            try
            {
                DataRow classInfo = DatabaseHelper.GetClassDetails(maLop);
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

                    // Thêm dòng "(Trống)"
                    DataRow emptyRow = dtAvailableTeachers.NewRow();
                    emptyRow["Ten"] = "(Trống)";
                    emptyRow["MaGV"] = "";
                    dtAvailableTeachers.Rows.InsertAt(emptyRow, 0);

                    // Nếu GVCN hiện tại vẫn còn (đang CN lớp khác?), thêm họ vào list
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
                    cboGvcn.SelectedValue = currentMaGVCN; // Đặt giá trị đã chọn

                    lblGVCN.ForeColor = (currentTenGVCN == "Chưa có") ? Color.FromArgb(220, 53, 69) : Color.FromArgb(108, 117, 125);
                }

                // Tải lưới
                dgvHocSinh.DataSource = DatabaseHelper.GetStudentsByClass(maLop);
                CustomizeStudentGrid();
                dgvPhanCong.DataSource = DatabaseHelper.GetTeachingAssignmentsByClass(maLop);
                CustomizeAssignmentGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải chi tiết lớp: " + ex.Message);
            }
            finally
            {
                isProgrammaticChange = false;
            }
        }

        /// <summary>
        /// Xóa chi tiết khi không có lớp nào được chọn.
        /// </summary>
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

            if (pnlChuyenLop != null)
            {
                pnlChuyenLop.Visible = false;
            }
        }

        /// <summary>
        /// Tùy chỉnh cột lưới Học Sinh.
        /// </summary>
        private void CustomizeStudentGrid()
        {
            if (dgvHocSinh.DataSource == null || dgvHocSinh.Columns.Count == 0) return;

            dgvHocSinh.ReadOnly = false; // Cho phép sửa

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

        /// <summary>
        /// Tùy chỉnh cột lưới Phân Công (thêm ComboBox động).
        /// </summary>
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

            // Nguồn dữ liệu cho ComboBox (copy từ cache)
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

            // Gán giá trị ban đầu cho ComboBox
            foreach (DataGridViewRow row in dgvPhanCong.Rows)
            {
                row.Cells["AssignTeacherColumn"].Value = row.Cells["MaGV"].Value;
            }

            // Gán sự kiện (lưu handler để Dispose)
            dgvEditingControlShowingHandler = new DataGridViewEditingControlShowingEventHandler(dgvPhanCong_EditingControlShowing);
            dgvCellValueChangedHandler = new DataGridViewCellEventHandler(dgvPhanCong_CellValueChanged);

            dgvPhanCong.EditingControlShowing -= dgvEditingControlShowingHandler;
            dgvPhanCong.EditingControlShowing += dgvEditingControlShowingHandler;
            dgvPhanCong.CellValueChanged -= dgvCellValueChangedHandler;
            dgvPhanCong.CellValueChanged += dgvCellValueChangedHandler;
        }

        #endregion

        #region HÀM VẼ GIAO DIỆN PHỤ (Helpers)

        /// <summary>
        /// Vẽ hiệu ứng bo góc và đổ bóng (hoặc viền).
        /// </summary>
        private void DrawShadow(object sender, PaintEventArgs e, int radius, Color color, bool border = false)
        {
            Control control = sender as Control;
            if (control == null) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = CreateRoundedRect(control.ClientRectangle, radius))
            {
                using (SolidBrush brush = new SolidBrush(color))
                {
                    e.Graphics.FillPath(brush, path);
                }
                if (border)
                {
                    using (Pen pen = new Pen(Color.FromArgb(222, 226, 230)))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }
        }

        /// <summary>
        /// Áp dụng style chung cho DataGridView (đã được gọi từ hàm cha).
        /// </summary>
        private void StyleDataGridView(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 245, 250);
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.DimGray;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(45, 45, 65);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 5, 0, 5);
            dgv.RowHeadersVisible = false;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgv.DefaultCellStyle.ForeColor = Color.DimGray;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowTemplate.Height = 40;
        }

        /// <summary>
        /// Tạo GraphicsPath bo góc (helper).
        /// </summary>
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

        #region Dispose

        /// <summary>
        /// CHUẨN HÓA: Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Gỡ sự kiện của control trong Designer
                if (cboKhoi != null) this.cboKhoi.SelectedIndexChanged -= new System.EventHandler(this.cboKhoi_SelectedIndexChanged);
                if (dgvLopHoc != null) this.dgvLopHoc.SelectionChanged -= new System.EventHandler(this.dgvLopHoc_SelectionChanged);
                if (btnLuuHS != null) this.btnLuuHS.Click -= new System.EventHandler(this.btnLuuHS_Click);
                if (btnXoaHS != null) this.btnXoaHS.Click -= new System.EventHandler(this.btnXoaHS_Click);
                if (btnImportHS != null) this.btnImportHS.Click -= new System.EventHandler(this.btnImportHS_Click);
                if (btnAssignGvcn != null) this.btnAssignGvcn.Click -= new System.EventHandler(this.btnAssignGvcn_Click);
                if (btnImportPhanCong != null) this.btnImportPhanCong.Click -= new System.EventHandler(this.btnImportPhanCong_Click);
                if (btnChuyenLop != null) this.btnChuyenLop.Click -= new System.EventHandler(this.btnChuyenLop_Click);

                // Gỡ sự kiện gán động
                if (btnXacNhanChuyen != null) btnXacNhanChuyen.Click -= btnXacNhanChuyenClickHandler;
                if (btnHuyChuyen != null) btnHuyChuyen.Click -= btnHuyChuyenClickHandler;
                if (pnlFilter != null) pnlFilter.Paint -= pnlFilterPaintHandler;
                if (pnlClassInfoCard != null) pnlClassInfoCard.Paint -= pnlClassInfoCardPaintHandler;
                if (pnlAssignGvcn != null) pnlAssignGvcn.Paint -= pnlAssignGvcnPaintHandler;
                if (dgvPhanCong != null)
                {
                    dgvPhanCong.DataError -= dgvDataErrorHandler;
                    dgvPhanCong.EditingControlShowing -= dgvEditingControlShowingHandler;
                    dgvPhanCong.CellValueChanged -= dgvCellValueChangedHandler;
                }

                // Hủy các control động
                clbHocSinhChuyen?.Dispose();
                cboLopMoi_Inline?.Dispose();
                btnXacNhanChuyen?.Dispose();
                btnHuyChuyen?.Dispose();

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