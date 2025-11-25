using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;

namespace N6
{
    public partial class UC_QuanLyTruongHoc : UserControl
    {
        #region Fields
        private DataTable _allLopHocCache;
        private DataTable _allThoiHanDiemCache;

        // Dữ liệu cho tab Lên Lớp
        private List<StudentPromotionInfo> _currentClassStudents = null;
        private string _currentMaLopCu = null;
        private string _currentKhoi = null;
        private bool _currentIsLop5 = false;
        private string _autoTargetClassIdForPassers = null;
        private string _autoTargetClassNameForPassers = "";

        // ĐÃ XÓA: private ComboBox _cboNamHocXetDuyet; -> Dùng cboNamHocXetDuyet của Designer
        #endregion

        #region Constructor & Load
        public UC_QuanLyTruongHoc()
        {
            InitializeComponent();
        }

        private void UC_QuanLyTruongHoc_Load(object sender, EventArgs e)
        {
            // Thiết lập giao diện chung
            LoadKhoiFilters();

            // Tab 1: Môn học
            LoadMonHoc();

            // Tab 2: Thời hạn
            LoadThoiHanDiem();

            // Tab 3: Lên lớp
            LoadLopCuComboBox();
            ResetLenLopUI();
            LoadYearData(); // Chỉ load dữ liệu vào ComboBox có sẵn, không tạo mới control nữa

            // Tab 4: Kho lưu trữ
            SetupArchiveTabLogic();
        }

        private void LoadKhoiFilters()
        {
            cboKhoiFilter.Items.Clear();
            cboKhoiFilter.Items.AddRange(new string[] { "Tất cả", "Khối 1", "Khối 2", "Khối 3", "Khối 4", "Khối 5" });
            cboKhoiFilter.SelectedIndex = 0;
        }
        #endregion

        #region 1. Quản lý Môn Học
        private void LoadMonHoc()
        {
            try
            {
                dgvMonHoc.DataSource = DatabaseHelper.GetAllSubjects();

                dgvMonHoc.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
            catch (Exception ex) { MessageBox.Show("Lỗi tải môn học: " + ex.Message); }
        }

        private void ClearMonHocInputs()
        {
            txtMaMon.Clear(); txtTenMon.Clear(); txtMaMon.ReadOnly = true;
            btnThemMon.Enabled = true; btnSuaMon.Enabled = false; btnXoaMon.Enabled = false;
            dgvMonHoc.ClearSelection();
        }

        private void dgvMonHoc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvMonHoc.Rows[e.RowIndex];
                txtMaMon.Text = row.Cells["MaMon"].Value?.ToString();
                txtTenMon.Text = row.Cells["TenMon"].Value?.ToString();
                btnThemMon.Enabled = false; btnSuaMon.Enabled = true; btnXoaMon.Enabled = true;
            }
        }

        private void btnThemMon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenMon.Text)) return;
            try { DatabaseHelper.InsertSubject(txtTenMon.Text.Trim()); LoadMonHoc(); } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void btnSuaMon_Click(object sender, EventArgs e)
        {
            try { DatabaseHelper.UpdateSubject(txtMaMon.Text, txtTenMon.Text.Trim()); LoadMonHoc(); } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void btnXoaMon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaMon.Text)) return;
            if (MessageBox.Show($"Xóa môn '{txtTenMon.Text}'?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try { DatabaseHelper.DeleteSubject(txtMaMon.Text); LoadMonHoc(); } catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
        private void btnMoiMon_Click(object sender, EventArgs e) { ClearMonHocInputs(); }
        #endregion

        #region 2. Quản lý Thời Hạn Điểm
        private void LoadThoiHanDiem()
        {
            try
            {
                _allThoiHanDiemCache = DatabaseHelper.GetScoreDeadlines();
                dgvThoiHanDiem.DataSource = _allThoiHanDiemCache;

                dgvThoiHanDiem.DataError += (s, e) => { e.ThrowException = false; };
                dgvThoiHanDiem.CellValidating += dgvThoiHanDiem_CellValidating;

                cboKhoiFilter_SelectedIndexChanged(null, null);
                CustomizeThoiHanGrid();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void CustomizeThoiHanGrid()
        {
            dgvThoiHanDiem.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvThoiHanDiem.RowTemplate.Height = 35;

            var headerMap = new Dictionary<string, string> {
                { "MaCotDiem", "Mã Cột" },
                { "TenHienThi", "Tên Hiển Thị" },
                { "Khoi", "Khối" },
                { "HocKy", "Học Kỳ" },
                { "NgayMoDiem", "Ngày Mở" },
                { "NgayKhoaDiem", "Ngày Khóa" },
                { "KhoaThuCong", "Khóa Thủ Công" },
                { "DaKhoa", "Khóa Tự Động" }
            };

            foreach (DataGridViewColumn col in dgvThoiHanDiem.Columns)
            {
                if (headerMap.ContainsKey(col.Name)) col.HeaderText = headerMap[col.Name];
                if (col.Name == "Khoi" || col.Name == "HocKy" || col.Name.Contains("Ngay"))
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvThoiHanDiem.Columns.Contains("NgayMoDiem")) dgvThoiHanDiem.Columns["NgayMoDiem"].DefaultCellStyle.Format = "dd/MM/yyyy";
            if (dgvThoiHanDiem.Columns.Contains("NgayKhoaDiem")) dgvThoiHanDiem.Columns["NgayKhoaDiem"].DefaultCellStyle.Format = "dd/MM/yyyy";

            ConvertBoolColumnToCheckbox(dgvThoiHanDiem, "KhoaThuCong");
            ConvertBoolColumnToCheckbox(dgvThoiHanDiem, "DaKhoa", true);

            string[] readOnlyCols = { "MaCotDiem", "TenHienThi", "Khoi", "HocKy", "DaKhoa" };
            foreach (string col in readOnlyCols) if (dgvThoiHanDiem.Columns.Contains(col)) dgvThoiHanDiem.Columns[col].ReadOnly = true;
        }

        private void ConvertBoolColumnToCheckbox(DataGridView dgv, string colName, bool readOnly = false)
        {
            if (dgv.Columns.Contains(colName) && !(dgv.Columns[colName] is DataGridViewCheckBoxColumn))
            {
                int idx = dgv.Columns[colName].Index;
                string header = dgv.Columns[colName].HeaderText;
                dgv.Columns.Remove(colName);
                var chk = new DataGridViewCheckBoxColumn { Name = colName, DataPropertyName = colName, HeaderText = header, ReadOnly = readOnly };
                dgv.Columns.Insert(idx, chk);
            }
        }

        private void dgvThoiHanDiem_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dgvThoiHanDiem.Columns[e.ColumnIndex].Name.Contains("Ngay"))
            {
                if (!DateTime.TryParseExact(e.FormattedValue.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    e.Cancel = true; MessageBox.Show("Vui lòng nhập đúng định dạng ngày: dd/MM/yyyy");
                }
            }
        }

        private void btnLuuThoiHan_Click(object sender, EventArgs e)
        {
            try
            {
                BindingContext[dgvThoiHanDiem.DataSource].EndCurrentEdit();
                DataTable changes = _allThoiHanDiemCache.GetChanges(DataRowState.Modified);
                if (changes != null)
                {
                    foreach (DataRow row in changes.Rows)
                    {
                        DatabaseHelper.UpdateScoreDeadline(row["MaCotDiem"].ToString(), row["Khoi"].ToString(), Convert.ToInt32(row["HocKy"]),
                            Convert.ToDateTime(row["NgayMoDiem"]), Convert.ToDateTime(row["NgayKhoaDiem"]), Convert.ToBoolean(row["KhoaThuCong"]));
                    }
                    _allThoiHanDiemCache.AcceptChanges();
                    MessageBox.Show("Đã lưu thành công!");
                    LoadThoiHanDiem();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void cboKhoiFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_allThoiHanDiemCache == null) return;
            string filter = (cboKhoiFilter.SelectedIndex <= 0) ? "" : $"Khoi = '{cboKhoiFilter.SelectedItem}'";
            _allThoiHanDiemCache.DefaultView.RowFilter = filter;
        }
        #endregion

        #region 3. Quản lý Lên Lớp / Cuối Năm
        public class StudentPromotionInfo
        {
            public string MaHS { get; set; }
            public string HoTen { get; set; }
            public double DiemTB { get; set; }
            public bool PassStatus => DiemTB >= 5.0;
            public override string ToString() => $"{HoTen} ({MaHS}) - ĐTB: {DiemTB:F2} ";
        }

        // ĐÃ XÓA: Hàm SetupPromotionTabYearSelector() gây trùng lặp.
        // Thay vào đó, ta sử dụng control cboNamHocXetDuyet đã có trong Designer.

        private void LoadYearData()
        {
            try
            {
                DataTable dt = DatabaseHelper.GetAllSchoolYears();
                // Sử dụng control của Designer (không có dấu gạch dưới)
                cboNamHocXetDuyet.DataSource = dt;
                cboNamHocXetDuyet.DisplayMember = "TenNamHoc";
                cboNamHocXetDuyet.ValueMember = "MaNamHoc";
                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToBoolean(row["IsCurrent"]))
                    {
                        cboNamHocXetDuyet.SelectedValue = row["MaNamHoc"];
                        break;
                    }
                }
            }
            catch { }
        }

        // Sự kiện này đã được gán trong Designer (this.btnNewYear.Click += ...)
        private void BtnNewYear_Click(object sender, EventArgs e)
        {
            string currentYear = cboNamHocXetDuyet.SelectedValue?.ToString() ?? "";
            string nextYearMa = "";
            string nextYearTen = "";

            if (currentYear.Contains("-"))
            {
                try
                {
                    string[] parts = currentYear.Split('-');
                    int y1 = int.Parse(parts[0]);
                    int y2 = int.Parse(parts[1]);
                    nextYearMa = $"{y1 + 1}-{y2 + 1}";
                    nextYearTen = $"Năm học {y1 + 1} - {y2 + 1}";
                }
                catch { }
            }

            if (MessageBox.Show($"Tạo niên khóa mới: {nextYearTen}?\nHệ thống sẽ Reset lớp và TKB cũ.", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.CreateNewSchoolYear(nextYearMa, nextYearTen);
                    MessageBox.Show("✅ Đã tạo năm mới thành công!", "Thành công");
                    LoadYearData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        private void LoadLopCuComboBox()
        {
            try
            {
                _allLopHocCache = DatabaseHelper.GetAllClasses();
                DataView dv = new DataView(_allLopHocCache); dv.Sort = "Khoi, TenLop";
                DataTable dt = dv.ToTable();
                DataRow dr = dt.NewRow(); dr["MaLop"] = DBNull.Value; dr["TenLop"] = "-- Chọn Lớp --";
                dt.Rows.InsertAt(dr, 0);
                cboLopCu.DisplayMember = "TenLop"; cboLopCu.ValueMember = "MaLop"; cboLopCu.DataSource = dt; cboLopCu.SelectedIndex = 0;
            }
            catch { }
        }

        private void ResetLenLopUI()
        {
            // Xóa dữ liệu cũ
            lstPassingStudents.Items.Clear(); // Dùng Items.Clear() thay vì gán DataSource = null để tránh lỗi
            lstFailingStudents.Items.Clear();

            // THAY ĐỔI Ở ĐÂY: Đừng ẩn panel, hãy cứ để nó hiện để giữ khung giao diện đẹp
            pnlPassingStudents.Visible = true;
            pnlFailingStudents.Visible = true;

            // Nhưng disable các control bên trong để người dùng biết chưa nhập liệu được
            cboLopMoi_LenLop.Enabled = false;
            cboLopMoi_OLaiLop.Enabled = false;

            // Reset text label đếm số lượng
            lblPassingCount.Text = "Đủ ĐK Lên Lớp (0)";
            lblFailingCount.Text = "Lưu ban (0)";

            btnThucHienLenLop_SingleClass.Enabled = false;
            lblSummary.Text = "Vui lòng chọn lớp và bấm 'Xem Dữ Liệu'";
        }

        private void cboLopCu_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentClassStudents = null; _currentMaLopCu = null; _currentKhoi = null; _currentIsLop5 = false;
            _autoTargetClassIdForPassers = null; _autoTargetClassNameForPassers = "";
            ResetLenLopUI();
        }

        private void btnLoadLopData_Click(object sender, EventArgs e)
        {
            if (cboLopCu.SelectedValue == null || cboLopCu.SelectedValue == DBNull.Value) return;

            _currentMaLopCu = cboLopCu.SelectedValue.ToString();
            string tenLopCu = cboLopCu.Text;
            this.Cursor = Cursors.WaitCursor;
            lblSummary.Text = "Đang phân tích..."; Application.DoEvents();

            try
            {
                DataRow lopCuInfo = DatabaseHelper.GetClassDetails(_currentMaLopCu);
                if (lopCuInfo == null) return;

                _currentKhoi = lopCuInfo["Khoi"]?.ToString();
                _currentIsLop5 = _currentKhoi?.Equals("Khối 5", StringComparison.OrdinalIgnoreCase) ?? false;

                _currentClassStudents = new List<StudentPromotionInfo>();
                DataTable dt = DatabaseHelper.GetSemesterScoreboard(_currentMaLopCu, 3);
                foreach (DataRow row in dt.Rows)
                {
                    if (row["MaHS"] == DBNull.Value) continue;
                    double diemTB = 0;
                    if (dt.Columns.Contains("Trung bình chung") && row["Trung bình chung"] != DBNull.Value)
                        double.TryParse(row["Trung bình chung"].ToString(), out diemTB);
                    _currentClassStudents.Add(new StudentPromotionInfo { MaHS = row["MaHS"].ToString(), HoTen = row["HoTen"].ToString(), DiemTB = diemTB });
                }

                if (_currentClassStudents.Count == 0) { lblSummary.Text = "Lớp chưa có dữ liệu."; ResetLenLopUI(); return; }

                var passing = _currentClassStudents.Where(s => s.PassStatus).ToList();
                var failing = _currentClassStudents.Where(s => !s.PassStatus).ToList();

                _autoTargetClassIdForPassers = null;
                if (_currentIsLop5)
                {
                    lblNextClassPrompt.Text = "Trạng thái:";
                    lblPassingCount.Text = $"Đủ ĐK Tốt Nghiệp ({passing.Count})";
                    cboLopMoi_LenLop.DataSource = null; cboLopMoi_LenLop.Items.Clear(); cboLopMoi_LenLop.Items.Add("Đã Tốt Nghiệp"); cboLopMoi_LenLop.SelectedIndex = 0; cboLopMoi_LenLop.Enabled = false;
                }
                else
                {
                    lblNextClassPrompt.Text = "Lên lớp (Tự động):";
                    string nextClassName = "";
                    for (int i = 0; i < tenLopCu.Length; i++) { if (char.IsDigit(tenLopCu[i])) { int v = int.Parse(tenLopCu[i].ToString()); if (v < 5) nextClassName = tenLopCu.Remove(i, 1).Insert(i, (v + 1).ToString()); break; } }

                    _autoTargetClassNameForPassers = nextClassName;
                    _autoTargetClassIdForPassers = DatabaseHelper.GetClassIdByName(nextClassName);

                    cboLopMoi_LenLop.DataSource = null; cboLopMoi_LenLop.Items.Clear();
                    if (!string.IsNullOrEmpty(_autoTargetClassIdForPassers)) cboLopMoi_LenLop.Items.Add($"{nextClassName} (Tự động)");
                    else cboLopMoi_LenLop.Items.Add($"Chưa có lớp {nextClassName}!");
                    cboLopMoi_LenLop.SelectedIndex = 0; cboLopMoi_LenLop.Enabled = false;
                    lblPassingCount.Text = $"Đủ ĐK Lên Lớp ({passing.Count})";
                }

                pnlPassingStudents.Visible = true; pnlFailingStudents.Visible = true;
                lstPassingStudents.Items.Clear(); foreach (var s in passing) lstPassingStudents.Items.Add(s);
                lstFailingStudents.Items.Clear(); foreach (var s in failing) lstFailingStudents.Items.Add(s);
                lblFailingCount.Text = $"Lưu ban ({failing.Count})";

                DataView dv = new DataView(_allLopHocCache ?? DatabaseHelper.GetAllClasses());
                dv.RowFilter = $"Khoi = '{_currentKhoi}'";
                cboLopMoi_OLaiLop.DataSource = dv.ToTable();
                cboLopMoi_OLaiLop.DisplayMember = "TenLop"; cboLopMoi_OLaiLop.ValueMember = "MaLop";
                cboLopMoi_OLaiLop.SelectedValue = _currentMaLopCu;

                btnThucHienLenLop_SingleClass.Enabled = true;
                lblSummary.Text = "Sẵn sàng thực hiện.";
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            finally { this.Cursor = Cursors.Default; }
        }

        private void btnThucHienLenLop_SingleClass_Click(object sender, EventArgs e)
        {
            // Sử dụng control của Designer
            if (cboNamHocXetDuyet.SelectedValue == null) { MessageBox.Show("Chưa chọn Năm học!"); return; }

            string msg = $"XÁC NHẬN KẾT THÚC NĂM HỌC {cboNamHocXetDuyet.Text}\nLỚP: {cboLopCu.Text}\n\nBạn có chắc chắn?";
            if (MessageBox.Show(msg, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    string maNam = cboNamHocXetDuyet.SelectedValue.ToString();
                    string maLopLen = _currentIsLop5 ? null : _autoTargetClassIdForPassers;
                    string maLopLuuBan = lstFailingStudents.Items.Count > 0 ? cboLopMoi_OLaiLop.SelectedValue?.ToString() : null;

                    DatabaseHelper.ProcessStudentPromotion_V2(_currentMaLopCu, maLopLen, maLopLuuBan, _currentIsLop5, maNam);

                    MessageBox.Show("✅ Xử lý thành công!", "Thông báo");
                    LoadLopCuComboBox(); ResetLenLopUI();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                finally { this.Cursor = Cursors.Default; }
            }
        }
        #endregion

        #region 4. Kho Lưu Trữ (Logic)

        private void SetupArchiveTabLogic()
        {
            // 1. Gán sự kiện
            cboArchiveNamHoc.SelectedIndexChanged += (s, e) => LoadArchiveClasses();
            cboArchiveKhoi.SelectedIndexChanged += (s, e) => LoadArchiveClasses();
            cboArchiveLop.SelectedIndexChanged += (s, e) => LoadArchiveStudents();
            txtSearchStudent.TextChanged += (s, e) =>
            {
                if (dgvArchiveStudents.DataSource is DataTable dt)
                    dt.DefaultView.RowFilter = $"HoTen LIKE '%{txtSearchStudent.Text}%'";
            };

            // 2. Data ban đầu
            if (cboArchiveKhoi.Items.Count == 0)
            {
                cboArchiveKhoi.Items.AddRange(new string[] { "Tất cả", "Khối 1", "Khối 2", "Khối 3", "Khối 4", "Khối 5" });
                cboArchiveKhoi.SelectedIndex = 0;
            }

            try
            {
                cboArchiveNamHoc.DataSource = DatabaseHelper.GetAllSchoolYears();
                cboArchiveNamHoc.DisplayMember = "TenNamHoc";
                cboArchiveNamHoc.ValueMember = "MaNamHoc";
            }
            catch { }
        }

        private void LoadArchiveClasses()
        {
            if (cboArchiveNamHoc.SelectedValue == null) return;
            try
            {
                string khoi = cboArchiveKhoi.SelectedItem?.ToString();
                DataTable dt = DatabaseHelper.GetArchiveClasses(cboArchiveNamHoc.SelectedValue.ToString(), khoi);
                DataRow dr = dt.NewRow(); dr["MaLop"] = DBNull.Value; dr["TenLop"] = "-- Chọn --"; dt.Rows.InsertAt(dr, 0);

                cboArchiveLop.DataSource = dt;
                cboArchiveLop.DisplayMember = "TenLop";
                cboArchiveLop.ValueMember = "MaLop";
            }
            catch { }
        }

        private void LoadArchiveStudents()
        {
            if (cboArchiveLop.SelectedValue == null || cboArchiveLop.SelectedValue == DBNull.Value)
            {
                dgvArchiveStudents.DataSource = null;
                lblArchiveGVCN.Text = "GVCN: ---";
                return;
            }
            try
            {
                string maLop = cboArchiveLop.SelectedValue.ToString();
                string nam = cboArchiveNamHoc.SelectedValue.ToString();

                if (cboArchiveLop.SelectedItem is DataRowView row)
                {
                    string maGVCN = row["MaGVCN"]?.ToString();
                    lblArchiveGVCN.Text = "GVCN: " + (string.IsNullOrEmpty(maGVCN) ? "Chưa rõ" : DatabaseHelper.GetTeacherNameById(maGVCN));
                }

                DataTable dt = DatabaseHelper.GetArchiveStudentList(nam, maLop);
                dgvArchiveStudents.DataSource = dt;

                dgvArchiveStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dgvArchiveStudents.Columns.Contains("MaHoSo")) dgvArchiveStudents.Columns["MaHoSo"].Visible = false;
                if (dgvArchiveStudents.Columns.Contains("MaHS")) dgvArchiveStudents.Columns["MaHS"].Visible = false;

                if (dgvArchiveStudents.Columns.Contains("HoTen"))
                {
                    dgvArchiveStudents.Columns["HoTen"].HeaderText = "Họ và Tên";
                    dgvArchiveStudents.Columns["HoTen"].FillWeight = 40;
                }
                if (dgvArchiveStudents.Columns.Contains("NgaySinh"))
                {
                    dgvArchiveStudents.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
                    dgvArchiveStudents.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvArchiveStudents.Columns["NgaySinh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                if (dgvArchiveStudents.Columns.Contains("GioiTinh"))
                {
                    dgvArchiveStudents.Columns["GioiTinh"].HeaderText = "Giới Tính";
                    dgvArchiveStudents.Columns["GioiTinh"].FillWeight = 15;
                }
                if (dgvArchiveStudents.Columns.Contains("DiemTongKet"))
                {
                    dgvArchiveStudents.Columns["DiemTongKet"].HeaderText = "ĐTB Cả Năm";
                    dgvArchiveStudents.Columns["DiemTongKet"].DefaultCellStyle.Format = "N2";
                    dgvArchiveStudents.Columns["DiemTongKet"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvArchiveStudents.Columns["DiemTongKet"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                }
                if (dgvArchiveStudents.Columns.Contains("KetQua"))
                {
                    dgvArchiveStudents.Columns["KetQua"].HeaderText = "Kết Quả";
                    dgvArchiveStudents.Columns["KetQua"].DefaultCellStyle.ForeColor = Color.Blue;
                }
            }
            catch { }
        }

        private void DgvArchiveStudents_DoubleClick(object sender, EventArgs e)
        {
            if (dgvArchiveStudents.CurrentRow != null)
            {
                string maHoSo = dgvArchiveStudents.CurrentRow.Cells["MaHoSo"].Value.ToString();
                string tenHS = dgvArchiveStudents.CurrentRow.Cells["HoTen"].Value.ToString();
                string nam = cboArchiveNamHoc.Text;
                FormBangDiem frm = new FormBangDiem(maHoSo, tenHS, nam);
                frm.ShowDialog();
            }
        }
        #endregion

        private void tabControlMain_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = tabControlMain.TabPages[e.Index];
            e.Graphics.FillRectangle(new SolidBrush((e.State & DrawItemState.Selected) == DrawItemState.Selected ? Color.FromArgb(0, 123, 255) : Color.WhiteSmoke), e.Bounds);
            TextRenderer.DrawText(e.Graphics, page.Text, new Font("Segoe UI", 10, (e.State & DrawItemState.Selected) == DrawItemState.Selected ? FontStyle.Bold : FontStyle.Regular), e.Bounds, (e.State & DrawItemState.Selected) == DrawItemState.Selected ? Color.White : Color.Black, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}