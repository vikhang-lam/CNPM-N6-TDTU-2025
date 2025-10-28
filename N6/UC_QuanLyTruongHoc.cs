using System;
using System.Collections.Generic; // Cần cho List
using System.Data;
using System.Drawing; // Cần cho việc vẽ TabControl
using System.Linq; // Cần cho LINQ
using System.Windows.Forms;

namespace N6
{
    public partial class UC_QuanLyTruongHoc : UserControl
    {
        // Cache để ComboBox dùng (cần cập nhật sau khi lên lớp)
        private DataTable allLopHocCache;
        private DataTable allThoiHanDiemCache; // Cache cho tab Thời Hạn

        // Dữ liệu cho tab Lên lớp
        private List<StudentPromotionInfo> currentClassStudents = null;
        private string currentMaLopCu = null;
        private string currentKhoi = null;
        private bool currentIsLop5 = false;


        public UC_QuanLyTruongHoc()
        {
            InitializeComponent();
            // Style DataGridView được áp dụng trong Designer mới
        }

        private void UC_QuanLyTruongHoc_Load(object sender, EventArgs e)
        {
            LoadKhoiFilters();
            LoadMonHoc();
            LoadThoiHanDiem();
            LoadLopCuComboBox(); // Tải danh sách lớp vào ComboBox tab Lên lớp
            ResetLenLopUI();     // Đặt lại giao diện tab Lên lớp
        }

        private void LoadKhoiFilters()
        {
            string[] khoiItems = { "Tất cả", "Khối 1", "Khối 2", "Khối 3", "Khối 4", "Khối 5" };
            cboKhoiFilter.Items.Clear();
            cboKhoiFilter.Items.AddRange(khoiItems);
            cboKhoiFilter.SelectedIndex = 0;
        }

        #region Quản lý Môn Học

        private void LoadMonHoc()
        {
            try
            {
                dgvMonHoc.DataSource = DatabaseHelper.GetAllMonHoc();
                if (dgvMonHoc.Columns.Contains("MaMon"))
                {
                    dgvMonHoc.Columns["MaMon"].HeaderText = "Mã Môn";
                    dgvMonHoc.Columns["MaMon"].FillWeight = 30;
                }
                if (dgvMonHoc.Columns.Contains("TenMon"))
                {
                    dgvMonHoc.Columns["TenMon"].HeaderText = "Tên Môn Học";
                    dgvMonHoc.Columns["TenMon"].FillWeight = 70;
                }

                ClearMonHocInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách môn học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearMonHocInputs()
        {
            txtMaMon.Clear();
            txtTenMon.Clear();
            txtMaMon.ReadOnly = true;
            txtMaMon.BackColor = SystemColors.Control;
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
                txtMaMon.Text = row.Cells["MaMon"].Value?.ToString();
                txtTenMon.Text = row.Cells["TenMon"].Value?.ToString();

                txtMaMon.ReadOnly = true;
                txtMaMon.BackColor = SystemColors.Control;
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
                txtTenMon.Focus();
                return;
            }

            try
            {
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
                MessageBox.Show("Vui lòng chọn một môn học và nhập tên mới.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            DialogResult confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa môn '{txtTenMon.Text}' ({txtMaMon.Text})?\nLưu ý: Chỉ xóa được khi môn học không đang được sử dụng.", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                allThoiHanDiemCache = DatabaseHelper.GetThoiHanDiem();
                dgvThoiHanDiem.DataSource = allThoiHanDiemCache;
                cboKhoiFilter_SelectedIndexChanged(null, null);
                CustomizeThoiHanGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thời hạn nhập điểm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            dgvThoiHanDiem.Columns["DaKhoa"].HeaderText = "Đã Khóa";
            dgvThoiHanDiem.Columns["DaKhoa"].ReadOnly = true;

            dgvThoiHanDiem.Columns["MaCotDiem"].FillWeight = 80;
            dgvThoiHanDiem.Columns["TenHienThi"].FillWeight = 120;
            dgvThoiHanDiem.Columns["Khoi"].FillWeight = 50;
            dgvThoiHanDiem.Columns["HocKy"].FillWeight = 40;
            dgvThoiHanDiem.Columns["NgayMoDiem"].FillWeight = 70;
            dgvThoiHanDiem.Columns["NgayKhoaDiem"].FillWeight = 70;
            dgvThoiHanDiem.Columns["KhoaThuCong"].FillWeight = 60;
            dgvThoiHanDiem.Columns["DaKhoa"].FillWeight = 50;

            dgvThoiHanDiem.Columns["NgayMoDiem"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvThoiHanDiem.Columns["NgayKhoaDiem"].DefaultCellStyle.Format = "dd/MM/yyyy";
            ConvertBoolColumnToCheckbox(dgvThoiHanDiem, "KhoaThuCong");
            ConvertBoolColumnToCheckbox(dgvThoiHanDiem, "DaKhoa", true);
        }
        private void ConvertBoolColumnToCheckbox(DataGridView dgv, string columnName, bool readOnly = false)
        {
            if (dgv.Columns.Contains(columnName) && !(dgv.Columns[columnName] is DataGridViewCheckBoxColumn))
            {
                int colIndex = dgv.Columns[columnName].Index;
                string headerText = dgv.Columns[columnName].HeaderText;
                float fillWeight = dgv.Columns[columnName].FillWeight;

                dgv.Columns.Remove(columnName);

                DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn
                {
                    Name = columnName,
                    HeaderText = headerText,
                    DataPropertyName = columnName,
                    FillWeight = fillWeight,
                    ReadOnly = readOnly,
                    FlatStyle = FlatStyle.Standard
                };
                dgv.Columns.Insert(colIndex, chkCol);
            }
            else if (dgv.Columns.Contains(columnName) && dgv.Columns[columnName] is DataGridViewCheckBoxColumn)
            {
                ((DataGridViewCheckBoxColumn)dgv.Columns[columnName]).ReadOnly = readOnly;
            }
        }

        private void dgvThoiHanDiem_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.RowIndex >= dgvThoiHanDiem.Rows.Count) return;

            DataGridViewCell cell = dgvThoiHanDiem.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string colName = dgvThoiHanDiem.Columns[e.ColumnIndex].Name;

            if (colName == "DaKhoa")
            {
                bool isLocked = false;
                if (cell.Value != null && cell.Value != DBNull.Value)
                {
                    isLocked = Convert.ToBoolean(cell.Value);
                }

                if (isLocked)
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 223, 223);
                    e.CellStyle.SelectionBackColor = Color.FromArgb(255, 180, 180);
                    cell.ToolTipText = "Đã khóa (Quá hạn hoặc khóa thủ công)";
                }
                else
                {
                    e.CellStyle.BackColor = (e.RowIndex % 2 == 0) ? dgvThoiHanDiem.DefaultCellStyle.BackColor : dgvThoiHanDiem.AlternatingRowsDefaultCellStyle.BackColor;
                    e.CellStyle.SelectionBackColor = dgvThoiHanDiem.DefaultCellStyle.SelectionBackColor;
                    cell.ToolTipText = "Đang mở";
                }
                e.FormattingApplied = true;
            }
            else if (colName == "KhoaThuCong")
            {
                bool isManualLock = false;
                if (cell.Value != null && cell.Value != DBNull.Value)
                {
                    isManualLock = Convert.ToBoolean(cell.Value);
                }
                cell.ToolTipText = isManualLock ? "Đang khóa thủ công" : "Mở thủ công";
                e.CellStyle.BackColor = (e.RowIndex % 2 == 0) ? dgvThoiHanDiem.DefaultCellStyle.BackColor : dgvThoiHanDiem.AlternatingRowsDefaultCellStyle.BackColor;
                e.CellStyle.SelectionBackColor = dgvThoiHanDiem.DefaultCellStyle.SelectionBackColor;
                e.FormattingApplied = true;
            }
            else if (e.CellStyle != null)
            {
                e.CellStyle.BackColor = (e.RowIndex % 2 == 0) ? dgvThoiHanDiem.DefaultCellStyle.BackColor : dgvThoiHanDiem.AlternatingRowsDefaultCellStyle.BackColor;
                e.CellStyle.SelectionBackColor = dgvThoiHanDiem.DefaultCellStyle.SelectionBackColor;
                if (string.IsNullOrEmpty(cell.ToolTipText))
                {
                    cell.ToolTipText = string.Empty;
                }
            }
        }


        private void btnLuuThoiHan_Click(object sender, EventArgs e)
        {
            try
            {
                this.BindingContext[dgvThoiHanDiem.DataSource].EndCurrentEdit();

                int updatedCount = 0;
                DataTable dtSource = allThoiHanDiemCache;
                DataTable changes = dtSource.GetChanges(DataRowState.Modified);


                if (changes != null)
                {
                    foreach (DataRow row in changes.Rows)
                    {
                        string maCotDiem = row["MaCotDiem", DataRowVersion.Original].ToString();
                        string khoi = row["Khoi", DataRowVersion.Original].ToString();
                        int hocKy = Convert.ToInt32(row["HocKy", DataRowVersion.Original]);

                        DateTime ngayMo = Convert.ToDateTime(row["NgayMoDiem", DataRowVersion.Current]);
                        DateTime ngayKhoa = Convert.ToDateTime(row["NgayKhoaDiem", DataRowVersion.Current]);
                        bool khoaThuCong = Convert.ToBoolean(row["KhoaThuCong", DataRowVersion.Current]);

                        DatabaseHelper.UpdateThoiHanDiem(maCotDiem, khoi, hocKy, ngayMo, ngayKhoa, khoaThuCong);
                        updatedCount++;
                    }
                    dtSource.AcceptChanges();
                }

                if (updatedCount > 0)
                {
                    MessageBox.Show($"Đã cập nhật thành công {updatedCount} thời hạn.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadThoiHanDiem();
                }
                else
                {
                    MessageBox.Show("Không có thay đổi nào để lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu thời hạn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadThoiHanDiem();
            }
        }


        private void cboKhoiFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (allThoiHanDiemCache == null) return;

            string selectedKhoi = cboKhoiFilter.SelectedItem?.ToString();
            DataView dv = allThoiHanDiemCache.DefaultView;

            if (selectedKhoi == "Tất cả" || selectedKhoi == null)
            {
                dv.RowFilter = "";
            }
            else
            {
                dv.RowFilter = $"Khoi = '{selectedKhoi}'";
            }
            dgvThoiHanDiem.Refresh();
        }


        #endregion

        #region Quản lý Lên Lớp (Quy trình mới)

        private class StudentPromotionInfo
        {
            public string MaHS { get; set; }
            public string HoTen { get; set; }
            public double DiemTB { get; set; }
            public bool PassStatus => DiemTB >= 5.0;
            public override string ToString() => $"{HoTen} ({MaHS}) - ĐTB: {DiemTB:F2}";
        }

        private void LoadLopCuComboBox()
        {
            try
            {
                allLopHocCache = DatabaseHelper.GetAllLopHoc();

                var lopHocList = allLopHocCache.AsEnumerable()
                                    .Where(row => row["MaLop"] != DBNull.Value)
                                     .OrderBy(row => row["Khoi"].ToString()).ThenBy(row => row["TenLop"].ToString())
                                    .CopyToDataTable();

                DataRow emptyRow = lopHocList.NewRow();
                emptyRow["MaLop"] = DBNull.Value;
                emptyRow["TenLop"] = "(Chọn lớp...)";
                lopHocList.Rows.InsertAt(emptyRow, 0);

                cboLopCu.DataSource = lopHocList;
                cboLopCu.DisplayMember = "TenLop";
                cboLopCu.ValueMember = "MaLop";
                cboLopCu.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException && allLopHocCache.Rows.Count == 0)
                {
                    DataTable emptyDt = allLopHocCache.Clone();
                    DataRow emptyRow = emptyDt.NewRow();
                    emptyRow["MaLop"] = DBNull.Value;
                    emptyRow["TenLop"] = "(Chưa có lớp nào)";
                    emptyDt.Rows.Add(emptyRow);
                    cboLopCu.DataSource = emptyDt;
                    cboLopCu.DisplayMember = "TenLop";
                    cboLopCu.ValueMember = "MaLop";
                    cboLopCu.SelectedIndex = 0;
                    cboLopCu.Enabled = false;
                    btnLoadLopData.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Lỗi khi tải danh sách lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void ResetLenLopUI()
        {
            // *** SỬA: Gán List rỗng ***
            lstPassingStudents.DataSource = new List<StudentPromotionInfo>();
            lstFailingStudents.DataSource = new List<StudentPromotionInfo>();
            // *** HẾT SỬA ***

            lblPassingCount.Text = "Đủ Điều Kiện";
            lblFailingCount.Text = "Không Đủ Điều Kiện";

            cboLopMoi_LenLop.DataSource = null;
            cboLopMoi_OLaiLop.DataSource = null;
            cboLopMoi_LenLop.Enabled = false;
            cboLopMoi_OLaiLop.Enabled = false;
            lblNextClassPrompt.Text = "Chuyển đến lớp:*";
            lblRepeatClassPrompt.Text = "Chuyển đến lớp:*";

            pnlPassingStudents.Visible = false;
            pnlFailingStudents.Visible = false;

            btnThucHienLenLop_SingleClass.Enabled = false;
            lblSummary.Text = "";

            currentClassStudents = null;
            currentMaLopCu = null;
            currentKhoi = null;
            currentIsLop5 = false;

            btnLoadLopData.Enabled = cboLopCu.SelectedIndex > 0;
        }

        private void cboLopCu_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetLenLopUI();
        }

        private void btnLoadLopData_Click(object sender, EventArgs e)
        {
            if (cboLopCu.SelectedValue == null || cboLopCu.SelectedValue == DBNull.Value)
            {
                MessageBox.Show("Vui lòng chọn một lớp để xử lý.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            currentMaLopCu = cboLopCu.SelectedValue.ToString();

            this.Cursor = Cursors.WaitCursor;
            lblSummary.Text = "Đang tính toán điểm và tải dữ liệu...";
            Application.DoEvents();

            try
            {
                DataRow lopCuInfo = DatabaseHelper.GetLopHocDetails(currentMaLopCu);
                if (lopCuInfo == null) throw new Exception($"Không tìm thấy thông tin lớp học với mã '{currentMaLopCu}'.");
                currentKhoi = lopCuInfo["Khoi"]?.ToString();
                if (string.IsNullOrEmpty(currentKhoi)) throw new Exception("Không xác định được khối cho lớp.");
                currentIsLop5 = currentKhoi.Equals("Khối 5", StringComparison.OrdinalIgnoreCase);

                currentClassStudents = CalculateStudentAverages(currentMaLopCu);

                if (currentClassStudents == null)
                {
                    lblSummary.Text = $"Lớp {cboLopCu.Text} chưa có đủ dữ liệu điểm để xét.";
                    pnlPassingStudents.Visible = false;
                    pnlFailingStudents.Visible = false;
                    btnThucHienLenLop_SingleClass.Enabled = false;
                    this.Cursor = Cursors.Default;
                    return;
                }
                else if (currentClassStudents.Count == 0)
                {
                    lblSummary.Text = $"Lớp {cboLopCu.Text} không có học sinh.";
                    pnlPassingStudents.Visible = false;
                    pnlFailingStudents.Visible = false;
                    btnThucHienLenLop_SingleClass.Enabled = false;
                    this.Cursor = Cursors.Default;
                    return;
                }


                var passingStudents = currentClassStudents.Where(s => s.PassStatus).ToList();
                var failingStudents = currentClassStudents.Where(s => !s.PassStatus).ToList();

                lblPassingCount.Text = $"Đủ Điều Kiện ({passingStudents.Count} hs)";
                lstPassingStudents.DataSource = passingStudents;
                lstPassingStudents.DisplayMember = "ToString";
                pnlPassingStudents.Visible = true;
                lstPassingStudents.Refresh(); // *** THÊM Refresh ***

                lblFailingCount.Text = $"Không Đủ Điều Kiện ({failingStudents.Count} hs)";
                lstFailingStudents.DataSource = failingStudents;
                lstFailingStudents.DisplayMember = "ToString";
                pnlFailingStudents.Visible = true;
                lstFailingStudents.Refresh(); // *** THÊM Refresh ***

                LoadDestinationClassComboBoxes();

                btnThucHienLenLop_SingleClass.Enabled = true;
                lblSummary.Text = $"Đã tải dữ liệu lớp {cboLopCu.Text}. Vui lòng chọn lớp mới và thực hiện.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetLenLopUI();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }


        private List<StudentPromotionInfo> CalculateStudentAverages(string maLop)
        {
            DataRow lopInfo = DatabaseHelper.GetLopHocDetails(maLop);
            if (lopInfo == null) return null;
            string khoi = lopInfo["Khoi"]?.ToString();
            if (string.IsNullOrEmpty(khoi)) return null;


            DataTable dtDiemCaNam = DatabaseHelper.GetBangDiemHocKy(maLop, 3);
            if (dtDiemCaNam == null || dtDiemCaNam.Rows.Count == 0) return new List<StudentPromotionInfo>();


            List<StudentPromotionInfo> studentList = new List<StudentPromotionInfo>();
            foreach (DataRow row in dtDiemCaNam.Rows)
            {
                if (row["MaHS"] == DBNull.Value || string.IsNullOrEmpty(row["MaHS"].ToString())) continue;

                double dtb = 0;
                if (row.Table.Columns.Contains("Trung bình chung") && row["Trung bình chung"] != DBNull.Value)
                {
                    double.TryParse(row["Trung bình chung"].ToString(), out dtb);
                }

                studentList.Add(new StudentPromotionInfo
                {
                    MaHS = row["MaHS"].ToString(),
                    HoTen = row["HoTen"]?.ToString() ?? "N/A",
                    DiemTB = Math.Round(dtb, 2)
                });
            }
            studentList = studentList.OrderBy(s => s.HoTen).ToList();
            return studentList;
        }

        private void LoadDestinationClassComboBoxes()
        {
            if (currentMaLopCu == null || allLopHocCache == null || string.IsNullOrEmpty(currentKhoi)) return;

            cboLopMoi_LenLop.DataSource = null;
            if (currentIsLop5)
            {
                lblNextClassPrompt.Text = "Tốt nghiệp:";
                var dtGrad = new DataTable();
                dtGrad.Columns.Add("MaLop", typeof(string));
                dtGrad.Columns.Add("TenLop", typeof(string));
                dtGrad.Rows.Add(DBNull.Value, "(Tốt nghiệp)");
                cboLopMoi_LenLop.DataSource = dtGrad;
                cboLopMoi_LenLop.DisplayMember = "TenLop";
                cboLopMoi_LenLop.ValueMember = "MaLop";
                cboLopMoi_LenLop.SelectedIndex = 0;
                cboLopMoi_LenLop.Enabled = false;
            }
            else
            {
                lblNextClassPrompt.Text = "Chuyển đến lớp:*";
                int currentGradeLevel = 0;
                int.TryParse(currentKhoi.Replace("Khối ", ""), out currentGradeLevel);
                int nextGradeLevel = currentGradeLevel + 1;
                string nextKhoi = $"Khối {nextGradeLevel}";

                DataTable nextGradeClasses = null;
                try
                {
                    nextGradeClasses = allLopHocCache.AsEnumerable()
                       .Where(row => row["MaLop"] != DBNull.Value && row["Khoi"] != DBNull.Value && row["Khoi"].ToString() == nextKhoi)
                       .OrderBy(row => row["TenLop"].ToString())
                       .CopyToDataTable();
                }
                catch (InvalidOperationException) { /* No rows */ }

                if (nextGradeClasses == null) nextGradeClasses = allLopHocCache.Clone();

                DataRow emptyRowNext = nextGradeClasses.NewRow();
                emptyRowNext["MaLop"] = DBNull.Value;
                emptyRowNext["TenLop"] = "(Chọn lớp...)";
                nextGradeClasses.Rows.InsertAt(emptyRowNext, 0);


                cboLopMoi_LenLop.DataSource = nextGradeClasses;
                cboLopMoi_LenLop.DisplayMember = "TenLop";
                cboLopMoi_LenLop.ValueMember = "MaLop";
                cboLopMoi_LenLop.Enabled = nextGradeClasses.Rows.Count > 1;

                string currentTenLop = allLopHocCache.AsEnumerable()
                                      .FirstOrDefault(r => r["MaLop"] != DBNull.Value && r["MaLop"].ToString() == currentMaLopCu)?["TenLop"].ToString();
                if (!string.IsNullOrEmpty(currentTenLop) && currentGradeLevel > 0)
                {
                    string suffix = "";
                    int firstDigitIndex = currentTenLop.IndexOfAny("0123456789".ToCharArray());
                    if (firstDigitIndex >= 0)
                    {
                        int firstLetterAfterDigitIndex = -1;
                        for (int i = firstDigitIndex + 1; i < currentTenLop.Length; i++)
                        {
                            if (char.IsLetter(currentTenLop[i]))
                            {
                                firstLetterAfterDigitIndex = i;
                                break;
                            }
                        }
                        if (firstLetterAfterDigitIndex >= 0)
                        {
                            suffix = currentTenLop.Substring(firstLetterAfterDigitIndex);
                        }
                    }
                    string suggestedTenLop = $"Lớp {nextGradeLevel}{suffix}";

                    var suggestedRow = nextGradeClasses.AsEnumerable()
                                       .FirstOrDefault(r => r["TenLop"].ToString().Equals(suggestedTenLop, StringComparison.OrdinalIgnoreCase));
                    if (suggestedRow != null)
                    {
                        cboLopMoi_LenLop.SelectedValue = suggestedRow["MaLop"];
                    }
                    else
                    {
                        cboLopMoi_LenLop.SelectedIndex = 0;
                    }
                }
                else
                {
                    cboLopMoi_LenLop.SelectedIndex = 0;
                }
            }

            lblRepeatClassPrompt.Text = "Chuyển đến lớp:*";
            DataTable sameGradeClasses = null;
            try
            {
                sameGradeClasses = allLopHocCache.AsEnumerable()
                   .Where(row => row["MaLop"] != DBNull.Value && row["Khoi"] != DBNull.Value && row["Khoi"].ToString() == currentKhoi && row["MaLop"].ToString() != currentMaLopCu)
                   .OrderBy(row => row["TenLop"].ToString())
                   .CopyToDataTable();
            }
            catch (InvalidOperationException) { /* No rows */ }

            if (sameGradeClasses == null) sameGradeClasses = allLopHocCache.Clone();

            DataRow emptyRowRepeat = sameGradeClasses.NewRow();
            emptyRowRepeat["MaLop"] = DBNull.Value;
            emptyRowRepeat["TenLop"] = "(Chọn lớp...)";
            sameGradeClasses.Rows.InsertAt(emptyRowRepeat, 0);

            cboLopMoi_OLaiLop.DataSource = sameGradeClasses;
            cboLopMoi_OLaiLop.DisplayMember = "TenLop";
            cboLopMoi_OLaiLop.ValueMember = "MaLop";
            cboLopMoi_OLaiLop.SelectedIndex = 0;
            cboLopMoi_OLaiLop.Enabled = sameGradeClasses.Rows.Count > 1;
        }

        private void btnThucHienLenLop_SingleClass_Click(object sender, EventArgs e)
        {
            if (currentMaLopCu == null || currentClassStudents == null) return;

            object lenLopObj = cboLopMoi_LenLop.SelectedValue;
            object oLaiLopObj = cboLopMoi_OLaiLop.SelectedValue;

            string maLopMoi_LenLop = (lenLopObj == DBNull.Value || lenLopObj == null) ? null : lenLopObj.ToString();
            string maLopMoi_OLaiLop = (oLaiLopObj == DBNull.Value || oLaiLopObj == null) ? null : oLaiLopObj.ToString();

            if (lstFailingStudents.Items.Count > 0 && string.IsNullOrEmpty(maLopMoi_OLaiLop))
            {
                MessageBox.Show("Vui lòng chọn lớp mới cho nhóm học sinh 'Không Đủ Điều Kiện'.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboLopMoi_OLaiLop.Focus();
                return;
            }
            if (lstPassingStudents.Items.Count > 0 && string.IsNullOrEmpty(maLopMoi_LenLop) && !currentIsLop5)
            {
                MessageBox.Show("Vui lòng chọn lớp mới cho nhóm học sinh 'Đủ Điều Kiện'.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboLopMoi_LenLop.Focus();
                return;
            }


            string msg = $"Bạn có chắc chắn muốn thực hiện xét lên lớp cho '{cboLopCu.Text}'?\n\n";
            msg += $"  - {lstPassingStudents.Items.Count} hs đủ điều kiện sẽ chuyển tới '{cboLopMoi_LenLop.Text}'.\n";
            msg += $"  - {lstFailingStudents.Items.Count} hs không đủ điều kiện sẽ chuyển tới '{cboLopMoi_OLaiLop.Text}'.\n\n";
            msg += "Hành động này KHÔNG THỂ HOÀN TÁC!";

            DialogResult confirm = MessageBox.Show(msg, "XÁC NHẬN LÊN LỚP", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.No) return;

            this.Cursor = Cursors.WaitCursor;
            lblSummary.Text = "Đang thực hiện chuyển lớp...";
            Application.DoEvents();

            try
            {
                int updatedPassing = 0;
                int updatedFailing = 0;
                int totalTotNghiep = 0;

                var failingHSList = currentClassStudents.Where(s => !s.PassStatus).Select(s => s.MaHS).ToList();
                if (failingHSList.Any() && !string.IsNullOrEmpty(maLopMoi_OLaiLop))
                {
                    updatedFailing = DatabaseHelper.UpdateHocSinhLop_Multi(failingHSList, maLopMoi_OLaiLop);
                }

                var passingHSList = currentClassStudents.Where(s => s.PassStatus).Select(s => s.MaHS).ToList();
                if (passingHSList.Any())
                {
                    if (currentIsLop5)
                    {
                        updatedPassing = DatabaseHelper.UpdateHocSinhLop_Multi(passingHSList, null);
                        totalTotNghiep = updatedPassing;
                        updatedPassing = 0;
                    }
                    else if (!string.IsNullOrEmpty(maLopMoi_LenLop))
                    {
                        updatedPassing = DatabaseHelper.UpdateHocSinhLop_Multi(passingHSList, maLopMoi_LenLop);
                    }
                }


                string resultMsg = "Đã xử lý xong!\n";
                resultMsg += $" - Lên Lớp: {updatedPassing} hs\n";
                resultMsg += $" - Ở Lại Lớp: {updatedFailing} hs\n";
                resultMsg += $" - Tốt Nghiệp (Khối 5): {totalTotNghiep} hs";
                lblSummary.Text = resultMsg;
                MessageBox.Show(resultMsg, "Hoàn thành", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadLopCuComboBox();
                ResetLenLopUI();

            }
            catch (Exception ex)
            {
                lblSummary.Text = "Xử lý thất bại!";
                MessageBox.Show("Đã xảy ra lỗi khi thực hiện lên lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }



        #endregion

        // === HÀM VẼ LẠI TABCONTROL ===
        private void tabControlMain_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            if (tabControl == null) return;
            TabPage tabPage = tabControl.TabPages[e.Index];
            Rectangle tabRect = tabControl.GetTabRect(e.Index);

            Color selectedBackColor = Color.FromArgb(0, 123, 255);
            Color unselectedBackColor = Color.FromArgb(240, 245, 250);
            Color selectedForeColor = Color.White;
            Color unselectedForeColor = Color.FromArgb(52, 73, 94);
            Font tabFont = new Font("Segoe UI", 10.2F, FontStyle.Regular);

            Color currentBackColor;
            Color currentForeColor;
            Font currentFont = tabFont;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                currentBackColor = selectedBackColor;
                currentForeColor = selectedForeColor;
                currentFont = new Font(tabFont, FontStyle.Bold);
            }
            else
            {
                currentBackColor = unselectedBackColor;
                currentForeColor = unselectedForeColor;
            }

            using (SolidBrush backBrush = new SolidBrush(currentBackColor))
            {
                // Vẽ bo góc
                int cornerRadius = 5;
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(tabRect.X, tabRect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
                path.AddArc(tabRect.Right - cornerRadius * 2, tabRect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
                path.AddLine(tabRect.Right, tabRect.Y + cornerRadius, tabRect.Right, tabRect.Bottom);
                path.AddLine(tabRect.Right, tabRect.Bottom, tabRect.X, tabRect.Bottom);
                path.AddLine(tabRect.X, tabRect.Bottom, tabRect.X, tabRect.Y + cornerRadius);
                e.Graphics.FillPath(backBrush, path);
                path.Dispose();

                if ((e.State & DrawItemState.Selected) != DrawItemState.Selected)
                {
                    using (Pen borderPen = new Pen(Color.LightGray))
                    {
                        e.Graphics.DrawLine(borderPen, tabRect.Left, tabRect.Bottom - 1, tabRect.Right, tabRect.Bottom - 1);
                    }
                }
            }

            TextRenderer.DrawText(e.Graphics, " " + tabPage.Text.Trim() + " ", currentFont, tabRect, currentForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            if (currentFont != tabFont)
            {
                currentFont.Dispose();
            }
            // Không dispose tabFont nếu nó là font mặc định
        }

    }
}