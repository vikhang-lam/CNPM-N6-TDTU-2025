using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_QuanLyTruongHoc : UserControl
    {
        private DataTable allLopHocCache; // Cache để ComboBox dùng
        private DataTable allThoiHanDiemCache; // Cache cho tab Thời Hạn
        private DataTable allLenLopCache; // Cache cho tab Lên Lớp

        public UC_QuanLyTruongHoc()
        {
            InitializeComponent();
            DatabaseHelper.StyleDataGridView(dgvMonHoc);
            DatabaseHelper.StyleDataGridView(dgvThoiHanDiem);
            DatabaseHelper.StyleDataGridView(dgvLenLop);
        }

        private void UC_QuanLyTruongHoc_Load(object sender, EventArgs e)
        {
            // Tải cache
            allLopHocCache = DatabaseHelper.GetAllLopHoc();

            // Gọi các hàm load
            LoadKhoiFilters();       // <-- PHẢI NẠP FILTER TRƯỚC
            LoadMonHoc();
            LoadThoiHanDiem();
            LoadTabLenLop(); // Tải bộ lọc Khối
        }

        /// <summary>
        /// Tải dữ liệu cho 2 ComboBox lọc theo khối
        /// </summary>
        private void LoadKhoiFilters()
        {
            string[] khoiItems = { "Tất cả", "Khối 1", "Khối 2", "Khối 3", "Khối 4", "Khối 5" };

            // Setup cho tab Thời Hạn Điểm
            cboKhoiFilter.Items.Clear();
            cboKhoiFilter.Items.AddRange(khoiItems);
            cboKhoiFilter.SelectedIndex = 0;

            // Setup cho tab Lên Lớp
            cboKhoiFilter_LenLop.Items.Clear();
            cboKhoiFilter_LenLop.Items.AddRange(khoiItems);
            cboKhoiFilter_LenLop.SelectedIndex = 0;
        }

        #region Quản lý Môn Học

        private void LoadMonHoc()
        {
            try
            {
                dgvMonHoc.DataSource = DatabaseHelper.GetAllMonHoc();
                dgvMonHoc.Columns["MaMon"].HeaderText = "Mã Môn";
                dgvMonHoc.Columns["TenMon"].HeaderText = "Tên Môn Học";
                dgvMonHoc.Columns["MaMon"].FillWeight = 40;
                dgvMonHoc.Columns["TenMon"].FillWeight = 60;

                ClearMonHocInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách môn học: " + ex.Message);
            }
        }

        private void ClearMonHocInputs()
        {
            txtMaMon.Clear();
            txtTenMon.Clear();
            txtMaMon.Enabled = false; // Mã môn nên được tạo tự động
            btnThemMon.Enabled = true;
            btnSuaMon.Enabled = false;
            btnXoaMon.Enabled = false;
            dgvMonHoc.ClearSelection();
            txtTenMon.Focus();
        }

        private void dgvMonHoc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvMonHoc.Rows[e.RowIndex];
                txtMaMon.Text = row.Cells["MaMon"].Value.ToString();
                txtTenMon.Text = row.Cells["TenMon"].Value.ToString();

                txtMaMon.Enabled = false;
                btnThemMon.Enabled = false;
                btnSuaMon.Enabled = true;
                btnXoaMon.Enabled = true;
            }
        }

        private void btnMoiMon_Click(object sender, EventArgs e)
        {
            ClearMonHocInputs();
        }

        private void btnThemMon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenMon.Text))
            {
                MessageBox.Show("Tên môn học không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // SP sp_InsertMonHoc sẽ tự tạo Mã Môn dựa trên Tên Môn
                DatabaseHelper.InsertMonHoc(txtTenMon.Text.Trim());
                MessageBox.Show("Thêm môn học thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMonHoc();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm môn học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSuaMon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaMon.Text) || string.IsNullOrWhiteSpace(txtTenMon.Text))
            {
                MessageBox.Show("Vui lòng chọn một môn học và nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DatabaseHelper.UpdateMonHoc(txtMaMon.Text, txtTenMon.Text.Trim());
                MessageBox.Show("Cập nhật môn học thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMonHoc();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật môn học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoaMon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaMon.Text))
            {
                MessageBox.Show("Vui lòng chọn một môn học để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa môn '{txtTenMon.Text}'?\nLưu ý: Chỉ xóa được khi môn học không đang được sử dụng.", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteMonHoc(txtMaMon.Text);
                    MessageBox.Show("Xóa môn học thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMonHoc();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa môn học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Quản lý Thời Hạn Điểm

        private void LoadThoiHanDiem()
        {
            try
            {
                // Tải vào cache
                allThoiHanDiemCache = DatabaseHelper.GetThoiHanDiem();
                dgvThoiHanDiem.DataSource = allThoiHanDiemCache;

                // Lọc theo ComboBox (nếu cần)
                cboKhoiFilter_SelectedIndexChanged(null, null);

                CustomizeThoiHanGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thời hạn nhập điểm: " + ex.Message);
            }
        }

        private void CustomizeThoiHanGrid()
        {
            dgvThoiHanDiem.Columns["MaCotDiem"].HeaderText = "Mã Cột Điểm";
            dgvThoiHanDiem.Columns["MaCotDiem"].ReadOnly = true;
            dgvThoiHanDiem.Columns["TenHienThi"].HeaderText = "Tên Cột Điểm";
            dgvThoiHanDiem.Columns["TenHienThi"].ReadOnly = true;
            dgvThoiHanDiem.Columns["Khoi"].HeaderText = "Khối";
            dgvThoiHanDiem.Columns["Khoi"].ReadOnly = true;
            dgvThoiHanDiem.Columns["HocKy"].HeaderText = "Học Kỳ";
            dgvThoiHanDiem.Columns["HocKy"].ReadOnly = true;
            dgvThoiHanDiem.Columns["NgayMoDiem"].HeaderText = "Ngày Mở";
            dgvThoiHanDiem.Columns["NgayKhoaDiem"].HeaderText = "Ngày Khóa";
            dgvThoiHanDiem.Columns["KhoaThuCong"].HeaderText = "Khóa Thủ Công";
            dgvThoiHanDiem.Columns["DaKhoa"].HeaderText = "Đã Khóa (Tự động)";
            dgvThoiHanDiem.Columns["DaKhoa"].ReadOnly = true;

            // Tô màu cột đã khóa
            var lockedCol = dgvThoiHanDiem.Columns["DaKhoa"];
            if (lockedCol != null)
            {
                lockedCol.DefaultCellStyle.Font = new Font(dgvThoiHanDiem.Font, FontStyle.Bold);
                lockedCol.DefaultCellStyle.ForeColor = Color.Red;
            }
        }

        private void btnLuuThoiHan_Click(object sender, EventArgs e)
        {
            try
            {
                dgvThoiHanDiem.EndEdit(); // Kết thúc chỉnh sửa
                int updatedCount = 0;

                // Lấy DataTable từ DataSource (là cache)
                DataTable dt = allThoiHanDiemCache;

                // Lấy các dòng đã bị thay đổi
                DataTable changes = dt.GetChanges(DataRowState.Modified);

                if (changes != null)
                {
                    foreach (DataRow row in changes.Rows)
                    {
                        string maCotDiem = row["MaCotDiem"].ToString();
                        string khoi = row["Khoi"].ToString(); // Cần Khoi và HocKy làm PK
                        int hocKy = Convert.ToInt32(row["HocKy"]);
                        DateTime ngayMo = Convert.ToDateTime(row["NgayMoDiem"]);
                        DateTime ngayKhoa = Convert.ToDateTime(row["NgayKhoaDiem"]);
                        bool khoaThuCong = Convert.ToBoolean(row["KhoaThuCong"]);

                        DatabaseHelper.UpdateThoiHanDiem(maCotDiem, khoi, hocKy, ngayMo, ngayKhoa, khoaThuCong);
                        updatedCount++;
                    }
                }

                if (updatedCount > 0)
                {
                    MessageBox.Show($"Đã cập nhật thành công {updatedCount} thời hạn.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadThoiHanDiem(); // Tải lại để thấy cột "DaKhoa" tính toán lại
                }
                else
                {
                    MessageBox.Show("Không có thay đổi nào để lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu thời hạn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboKhoiFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (allThoiHanDiemCache == null) return;

            string selectedKhoi = cboKhoiFilter.SelectedItem.ToString();
            if (selectedKhoi == "Tất cả")
            {
                allThoiHanDiemCache.DefaultView.RowFilter = "";
            }
            else
            {
                allThoiHanDiemCache.DefaultView.RowFilter = $"Khoi = '{selectedKhoi}'";
            }
        }

        #endregion

        #region Quản lý Lên Lớp

        private void LoadTabLenLop()
        {
            try
            {
                // Lấy tất cả lớp học (đặc biệt là lớp cũ)
                DataTable dtLopHoc = DatabaseHelper.GetAllLopHoc();
                allLenLopCache = dtLopHoc; // Gán vào cache
                dgvLenLop.DataSource = allLenLopCache;

                // Lọc theo ComboBox (nếu cần)
                cboKhoiFilter_LenLop_SelectedIndexChanged(null, null);

                // Tùy chỉnh cột
                dgvLenLop.Columns["MaLop"].HeaderText = "Mã Lớp Cũ";
                dgvLenLop.Columns["MaLop"].ReadOnly = true;
                dgvLenLop.Columns["TenLop"].HeaderText = "Tên Lớp Cũ";
                dgvLenLop.Columns["TenLop"].ReadOnly = true;
                dgvLenLop.Columns["Khoi"].HeaderText = "Khối";
                dgvLenLop.Columns["Khoi"].ReadOnly = true;

                // Ẩn các cột không cần thiết (nếu có)
                // dgvLenLop.Columns["NamHoc"].Visible = false;

                // Chuẩn bị data source cho ComboBox (chứa tất cả lớp)
                // Thêm một dòng "NULL" (Trống) vào cache
                DataRow emptyRow = allLopHocCache.NewRow();
                emptyRow["MaLop"] = DBNull.Value;
                emptyRow["TenLop"] = "(Không chọn)";
                allLopHocCache.Rows.InsertAt(emptyRow, 0);

                // 1. Thêm cột ComboBox "Lên Lớp"
                if (!dgvLenLop.Columns.Contains("colLenLop"))
                {
                    var colLenLop = new DataGridViewComboBoxColumn
                    {
                        Name = "colLenLop",
                        HeaderText = "Chuyển Tới Lớp (Nếu Lên Lớp)",
                        DataSource = allLopHocCache.Copy(), // Dùng bản sao
                        ValueMember = "MaLop",
                        DisplayMember = "TenLop",
                        FlatStyle = FlatStyle.Flat,
                        FillWeight = 120
                    };
                    dgvLenLop.Columns.Add(colLenLop);
                }

                // 2. Thêm cột ComboBox "Ở Lại Lớp"
                if (!dgvLenLop.Columns.Contains("colOLaiLop"))
                {
                    var colOLaiLop = new DataGridViewComboBoxColumn
                    {
                        Name = "colOLaiLop",
                        HeaderText = "Chuyển Tới Lớp (Nếu Ở Lại)",
                        DataSource = allLopHocCache.Copy(), // Dùng bản sao
                        ValueMember = "MaLop",
                        DisplayMember = "TenLop",
                        FlatStyle = FlatStyle.Flat,
                        FillWeight = 120
                    };
                    dgvLenLop.Columns.Add(colOLaiLop);
                }

                // 3. Tự động xử lý cho Lớp 5
                foreach (DataGridViewRow row in dgvLenLop.Rows)
                {
                    string khoi = row.Cells["Khoi"].Value.ToString();
                    if (khoi.Equals("Khối 5", StringComparison.OrdinalIgnoreCase))
                    {
                        // Khóa cột "Lên Lớp" vì Lớp 5 sẽ Tốt nghiệp
                        var cell = (DataGridViewComboBoxCell)row.Cells["colLenLop"];
                        cell.Value = DBNull.Value;
                        cell.ReadOnly = true;
                        cell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                        cell.ToolTipText = "Học sinh Khối 5 sẽ tự động Tốt nghiệp nếu đủ điểm.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải tab Lên Lớp: " + ex.Message);
            }
        }

        private void btnThucHienLenLop_Click(object sender, EventArgs e)
        {
            string msg = "Bạn có chắc chắn muốn thực hiện nghiệp vụ LÊN LỚP hàng loạt không?\n\n";
            msg += "Hành động này sẽ TÍNH ĐIỂM TB và CHUYỂN TOÀN BỘ HỌC SINH ra khỏi các lớp cũ dựa trên cấu hình của bạn.\n\n";
            msg += "HÀNH ĐỘNG NÀY KHÔNG THỂ HOÀN TÁC!";

            DialogResult confirm = MessageBox.Show(msg, "XÁC NHẬN HÀNH ĐỘNG NGHIÊM TRỌNG", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.No) return;

            this.Cursor = Cursors.WaitCursor;
            int totalLenLop = 0;
            int totalOLaiLop = 0;
            int totalTotNghiep = 0;

            try
            {
                foreach (DataGridViewRow row in dgvLenLop.Rows)
                {
                    // Nếu dòng đang bị lọc ra, bỏ qua
                    if (!row.Visible) continue;

                    string maLopCu = row.Cells["MaLop"].Value?.ToString();
                    if (string.IsNullOrEmpty(maLopCu)) continue;

                    // Lấy giá trị từ ComboBox, xử lý DBNull
                    object lenLopObj = row.Cells["colLenLop"].Value;
                    object oLaiLopObj = row.Cells["colOLaiLop"].Value;

                    string maLopMoi_LenLop = (lenLopObj == DBNull.Value || lenLopObj == null) ? null : lenLopObj.ToString();
                    string maLopMoi_OLaiLop = (oLaiLopObj == DBNull.Value || oLaiLopObj == null) ? null : oLaiLopObj.ToString();

                    bool isLop5 = row.Cells["Khoi"].Value.ToString().Equals("Khối 5", StringComparison.OrdinalIgnoreCase);

                    // Nếu không chọn lớp ở lại VÀ không chọn lớp lên lớp/tốt nghiệp -> Bỏ qua
                    if (string.IsNullOrEmpty(maLopMoi_OLaiLop) && (string.IsNullOrEmpty(maLopMoi_LenLop) && !isLop5))
                    {
                        continue;
                    }

                    // Gọi SP
                    DataTable result = DatabaseHelper.ProcessStudentPromotion(maLopCu, maLopMoi_LenLop, maLopMoi_OLaiLop, isLop5);

                    if (result.Rows.Count > 0)
                    {
                        totalLenLop += (int)result.Rows[0]["SoHSLenLop"];
                        totalOLaiLop += (int)result.Rows[0]["SoHSOLaiLop"];
                        totalTotNghiep += (int)result.Rows[0]["SoHSTotNghiep"];
                    }
                }

                this.Cursor = Cursors.Default;
                string resultMsg = "Đã xử lý xong!\n\n";
                resultMsg += $"- Tổng số học sinh Lên Lớp: {totalLenLop}\n";
                resultMsg += $"- Tổng số học sinh Ở Lại Lớp: {totalOLaiLop}\n";
                resultMsg += $"- Tổng số học sinh Tốt Nghiệp (Khối 5): {totalTotNghiep}\n\n";
                resultMsg += "Vui lòng kiểm tra lại Sĩ Số các lớp trong 'Quản lý Lớp học'.";
                MessageBox.Show(resultMsg, "Hoàn thành", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tải lại dữ liệu cho tab Lên Lớp (lớp cũ giờ đã trống)
                // Cần tải lại cache trước
                allLopHocCache = DatabaseHelper.GetAllLopHoc();
                // Xóa các cột ComboBox cũ đi trước khi thêm lại
                if (dgvLenLop.Columns.Contains("colLenLop")) dgvLenLop.Columns.Remove("colLenLop");
                if (dgvLenLop.Columns.Contains("colOLaiLop")) dgvLenLop.Columns.Remove("colOLaiLop");
                LoadTabLenLop();
                LoadKhoiFilters(); // Đặt lại filter
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("Đã xảy ra lỗi nghiêm trọng trong quá trình xử lý: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboKhoiFilter_LenLop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (allLenLopCache == null) return;

            string selectedKhoi = cboKhoiFilter_LenLop.SelectedItem.ToString();
            if (selectedKhoi == "Tất cả")
            {
                allLenLopCache.DefaultView.RowFilter = "";
            }
            else
            {
                allLenLopCache.DefaultView.RowFilter = $"Khoi = '{selectedKhoi}'";
            }
        }

        #endregion
    }
}