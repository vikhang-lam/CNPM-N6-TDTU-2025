using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;

namespace N6
{
    /// <summary>
    /// UserControl quản lý nghiệp vụ toàn trường: Môn học, Thời hạn điểm, Lên lớp, Lưu trữ.
    /// </summary>
    public partial class UC_QuanLyTruongHoc : UserControl
    {
        #region Fields (Biến thành viên)

        // Cache dữ liệu
        private DataTable _allLopHocCache;
        private DataTable _allThoiHanDiemCache;

        // Dữ liệu cho tab Lên Lớp
        private List<StudentPromotionInfo> _currentClassStudents = null;
        private string _currentMaLopCu = null;
        private string _currentKhoi = null;
        private bool _currentIsLop5 = false;

        // Biến hỗ trợ tính toán Lên lớp tự động
        private string _autoTargetClassIdForPassers = null;
        private string _autoTargetClassNameForPassers = "";

        // Controls cho tab Kho Lưu Trữ (Dynamic UI)
        private TabPage _tabKhoLuuTru;
        private ComboBox _cboArchiveNamHoc;
        private ComboBox _cboArchiveKhoi;
        private ComboBox _cboArchiveLop;     // Thay thế Grid lớp cũ
        private TextBox _txtSearchStudent;   // Thanh tìm kiếm
        private DataGridView _dgvArchiveStudents;
        private DataGridView _dgvArchiveTeachers;
        private Label _lblArchiveGVCN;

        #endregion

        #region Constructor & Load

        public UC_QuanLyTruongHoc()
        {
            InitializeComponent();
        }

        private void UC_QuanLyTruongHoc_Load(object sender, EventArgs e)
        {
            // 1. Tải dữ liệu cho các tab tĩnh (đã có trong Designer)
            LoadKhoiFilters();
            LoadMonHoc();
            LoadThoiHanDiem();
            LoadLopCuComboBox();
            ResetLenLopUI();

            // 2. Khởi tạo giao diện tab Kho Lưu Trữ (Code-behind)
            SetupArchiveTab();
        }

        private void LoadKhoiFilters()
        {
            string[] khoiItems =
            {
                "Tất cả", "Khối 1", "Khối 2", "Khối 3", "Khối 4", "Khối 5"
            };

            cboKhoiFilter.Items.Clear();
            cboKhoiFilter.Items.AddRange(khoiItems);
            cboKhoiFilter.SelectedIndex = 0;
        }

        #endregion

        #region 1. Quản lý Môn Học

        private void LoadMonHoc()
        {
            try
            {
                dgvMonHoc.DataSource = DatabaseHelper.GetAllSubjects();

                // Cấu hình cột tự động giãn, tránh thanh trượt
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
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải môn học: " + ex.Message);
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
                btnThemMon.Enabled = false;
                btnSuaMon.Enabled = true;
                btnXoaMon.Enabled = true;
            }
        }

        private void btnThemMon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenMon.Text))
            {
                MessageBox.Show("Tên môn không được để trống.");
                return;
            }

            try
            {
                DatabaseHelper.InsertSubject(txtTenMon.Text.Trim());
                MessageBox.Show("Thêm môn học thành công!");
                LoadMonHoc();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnSuaMon_Click(object sender, EventArgs e)
        {
            try
            {
                DatabaseHelper.UpdateSubject(txtMaMon.Text, txtTenMon.Text.Trim());
                MessageBox.Show("Cập nhật thành công!");
                LoadMonHoc();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnXoaMon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaMon.Text))
                return;

            DialogResult dr = MessageBox.Show(
                $"Xóa môn '{txtTenMon.Text}'?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (dr == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteSubject(txtMaMon.Text);
                    LoadMonHoc();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void btnMoiMon_Click(object sender, EventArgs e)
        {
            ClearMonHocInputs();
        }

        #endregion

        #region 2. Quản lý Thời Hạn Điểm (Đã sửa: Bỏ màu xám)

        private void LoadThoiHanDiem()
        {
            try
            {
                _allThoiHanDiemCache = DatabaseHelper.GetScoreDeadlines();
                dgvThoiHanDiem.DataSource = _allThoiHanDiemCache;

                // Đăng ký sự kiện kiểm tra lỗi (Chỉ đăng ký 1 lần)
                dgvThoiHanDiem.DataError -= dgvThoiHanDiem_DataError;
                dgvThoiHanDiem.DataError += dgvThoiHanDiem_DataError;

                dgvThoiHanDiem.CellValidating -= dgvThoiHanDiem_CellValidating;
                dgvThoiHanDiem.CellValidating += dgvThoiHanDiem_CellValidating;

                cboKhoiFilter_SelectedIndexChanged(null, null);
                CustomizeThoiHanGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thời hạn: " + ex.Message);
            }
        }

        private void CustomizeThoiHanGrid()
        {
            dgvThoiHanDiem.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Cập nhật tên cột
            var colSettings = new Dictionary<string, (string Header, int Weight)>
            {
                { "MaCotDiem", ("Mã", 15) },
                { "TenHienThi", ("Tên Cột Điểm", 30) },
                { "Khoi", ("Khối", 10) },
                { "HocKy", ("HK", 10) },
                { "NgayMoDiem", ("Ngày Mở", 15) },
                { "NgayKhoaDiem", ("Ngày Khóa", 15) },
                { "KhoaThuCong", ("Khóa (Thủ công)", 10) },
                { "DaKhoa", ("Khóa (Tự động)", 10) }
            };

            foreach (var col in colSettings)
            {
                if (dgvThoiHanDiem.Columns.Contains(col.Key))
                {
                    dgvThoiHanDiem.Columns[col.Key].HeaderText = col.Value.Header;
                    dgvThoiHanDiem.Columns[col.Key].FillWeight = col.Value.Weight;
                }
            }

            // ==========================================================
            // [UPDATE] KHÓA CỘT (ReadOnly) NHƯNG KHÔNG ĐỔI MÀU NỀN
            // ==========================================================
            string[] lockedColumns = { "MaCotDiem", "TenHienThi", "Khoi", "HocKy" };
            foreach (string colName in lockedColumns)
            {
                if (dgvThoiHanDiem.Columns.Contains(colName))
                {
                    dgvThoiHanDiem.Columns[colName].ReadOnly = true;
                    // Đã loại bỏ dòng set màu BackgroundColor = Color.WhiteSmoke theo yêu cầu
                }
            }

            // Định dạng ngày tháng
            if (dgvThoiHanDiem.Columns.Contains("NgayMoDiem"))
            {
                dgvThoiHanDiem.Columns["NgayMoDiem"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }

            if (dgvThoiHanDiem.Columns.Contains("NgayKhoaDiem"))
            {
                dgvThoiHanDiem.Columns["NgayKhoaDiem"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }

            ConvertBoolColumnToCheckbox(dgvThoiHanDiem, "KhoaThuCong");
            ConvertBoolColumnToCheckbox(dgvThoiHanDiem, "DaKhoa", true);
        }

        private void ConvertBoolColumnToCheckbox(DataGridView dgv, string colName, bool readOnly = false)
        {
            if (dgv.Columns.Contains(colName) &&
                !(dgv.Columns[colName] is DataGridViewCheckBoxColumn))
            {
                int idx = dgv.Columns[colName].Index;
                string header = dgv.Columns[colName].HeaderText;
                float weight = dgv.Columns[colName].FillWeight;

                dgv.Columns.Remove(colName);

                var chk = new DataGridViewCheckBoxColumn
                {
                    Name = colName,
                    DataPropertyName = colName,
                    HeaderText = header,
                    FillWeight = weight,
                    ReadOnly = readOnly
                };

                dgv.Columns.Insert(idx, chk);
            }
        }

        // Xử lý sự kiện khi có lỗi dữ liệu
        private void dgvThoiHanDiem_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;

            string columnName = dgvThoiHanDiem.Columns[e.ColumnIndex].Name;
            if (columnName == "NgayMoDiem" || columnName == "NgayKhoaDiem")
            {
                MessageBox.Show("❌ Lỗi định dạng ngày tháng!\nVui lòng không nhập chữ cái và tuân thủ định dạng dd/MM/yyyy.",
                                "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("❌ Dữ liệu nhập vào không hợp lệ với kiểu dữ liệu của cột.",
                                "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Kiểm tra tính hợp lệ của ngày tháng
        private void dgvThoiHanDiem_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string columnName = dgvThoiHanDiem.Columns[e.ColumnIndex].Name;

            if (columnName == "NgayMoDiem" || columnName == "NgayKhoaDiem")
            {
                string input = e.FormattedValue.ToString().Trim();
                if (string.IsNullOrEmpty(input)) return;

                DateTime dt;
                bool isValid = DateTime.TryParseExact(input, "dd/MM/yyyy",
                                                      CultureInfo.InvariantCulture,
                                                      DateTimeStyles.None,
                                                      out dt);

                if (!isValid)
                {
                    e.Cancel = true;
                    MessageBox.Show($"❌ Ngày tháng '{input}' không hợp lệ!\n" +
                                    "- Phải đúng định dạng: ngày/tháng/năm (dd/MM/yyyy)\n" +
                                    "- Không chứa chữ cái.\n" +
                                    "- Không được sai logic (ví dụ: 32/12, 30/02...)",
                                    "Lỗi nhập ngày tháng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void dgvThoiHanDiem_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvThoiHanDiem.Columns[e.ColumnIndex].Name == "DaKhoa" && e.Value != null)
            {
                bool isLocked = (bool)e.Value;
                if (isLocked)
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 223, 223);
                    e.CellStyle.SelectionBackColor = Color.FromArgb(255, 180, 180);
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
                        DatabaseHelper.UpdateScoreDeadline(
                            row["MaCotDiem"].ToString(),
                            row["Khoi"].ToString(),
                            Convert.ToInt32(row["HocKy"]),
                            Convert.ToDateTime(row["NgayMoDiem"]),
                            Convert.ToDateTime(row["NgayKhoaDiem"]),
                            Convert.ToBoolean(row["KhoaThuCong"])
                        );
                    }
                    _allThoiHanDiemCache.AcceptChanges();
                    MessageBox.Show("Đã lưu các thay đổi thành công!");
                    LoadThoiHanDiem();
                }
                else
                {
                    MessageBox.Show("Không có thay đổi nào để lưu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message);
            }
        }

        private void cboKhoiFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_allThoiHanDiemCache == null)
                return;

            string selected = cboKhoiFilter.SelectedItem?.ToString();
            string filter = (selected == "Tất cả" || selected == null)
                ? ""
                : $"Khoi = '{selected}'";

            _allThoiHanDiemCache.DefaultView.RowFilter = filter;
        }

        #endregion

        #region 3. Quản lý Lên Lớp

        public class StudentPromotionInfo
        {
            public string MaHS { get; set; }
            public string HoTen { get; set; }
            public double DiemTB { get; set; }
            public bool PassStatus => DiemTB >= 5.0;

            public string InfoHienThi
            {
                get { return ToString(); }
            }

            public override string ToString()
            {
                return $"{HoTen} ({MaHS}) - ĐTB: {DiemTB:F2} ";
            }
        }

        private void LoadLopCuComboBox()
        {
            try
            {
                _allLopHocCache = DatabaseHelper.GetAllClasses();

                DataView dv = new DataView(_allLopHocCache);
                dv.Sort = "Khoi, TenLop";
                DataTable dt = dv.ToTable();

                DataRow dr = dt.NewRow();
                dr["MaLop"] = DBNull.Value;
                dr["TenLop"] = "-- Chọn Lớp Cần Xét --";
                dt.Rows.InsertAt(dr, 0);

                cboLopCu.DisplayMember = "TenLop";
                cboLopCu.ValueMember = "MaLop";
                cboLopCu.DataSource = dt;

                cboLopCu.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách lớp: " + ex.Message);
            }
        }

        private void ResetLenLopUI()
        {
            lstPassingStudents.DataSource = null;
            lstFailingStudents.DataSource = null;

            pnlPassingStudents.Visible = false;
            pnlFailingStudents.Visible = false;

            btnThucHienLenLop_SingleClass.Enabled = false;
            lblSummary.Text = "";
        }

        private void cboLopCu_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentClassStudents = null;
            _currentMaLopCu = null;
            _currentKhoi = null;
            _currentIsLop5 = false;
            _autoTargetClassIdForPassers = null;
            _autoTargetClassNameForPassers = "";

            ResetLenLopUI();
        }

        private string DetermineNextClassName(string currentClassName, string khoiHienTai)
        {
            if (string.IsNullOrEmpty(currentClassName))
                return null;

            if (khoiHienTai.Equals("Khối 5", StringComparison.OrdinalIgnoreCase))
            {
                return "Tốt Nghiệp";
            }

            for (int i = 0; i < currentClassName.Length; i++)
            {
                if (char.IsDigit(currentClassName[i]))
                {
                    int digitVal = int.Parse(currentClassName[i].ToString());
                    if (digitVal < 5)
                    {
                        char nextDigit = (char)('0' + (digitVal + 1));
                        char[] chars = currentClassName.ToCharArray();
                        chars[i] = nextDigit;
                        return new string(chars);
                    }
                    break;
                }
            }
            return null;
        }

        private void btnLoadLopData_Click(object sender, EventArgs e)
        {
            if (cboLopCu.SelectedValue == null || cboLopCu.SelectedValue == DBNull.Value)
            {
                MessageBox.Show("Vui lòng chọn một lớp để xử lý.", "Thông báo",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _currentMaLopCu = cboLopCu.SelectedValue.ToString();
            string tenLopCu = cboLopCu.Text;

            this.Cursor = Cursors.WaitCursor;
            lblSummary.Text = "Đang tính toán điểm và phân loại...";
            Application.DoEvents();

            try
            {
                if (_allLopHocCache == null)
                {
                    _allLopHocCache = DatabaseHelper.GetAllClasses();
                }

                DataRow lopCuInfo = DatabaseHelper.GetClassDetails(_currentMaLopCu);
                if (lopCuInfo == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin lớp!", "Lỗi",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _currentKhoi = lopCuInfo["Khoi"]?.ToString();
                _currentIsLop5 = _currentKhoi?.Equals("Khối 5", StringComparison.OrdinalIgnoreCase) ?? false;

                _currentClassStudents = CalculateStudentAverages(_currentMaLopCu);

                if (_currentClassStudents == null)
                {
                    lblSummary.Text = "Lỗi khi tính điểm học sinh!";
                    ResetLenLopUI();
                    return;
                }

                if (_currentClassStudents.Count == 0)
                {
                    lblSummary.Text = "Lớp chưa có dữ liệu điểm hoặc không có học sinh.";
                    lstPassingStudents.DataSource = null;
                    lstFailingStudents.DataSource = null;
                    this.Cursor = Cursors.Default;
                    return;
                }

                var passing = _currentClassStudents.Where(s => s.PassStatus).ToList();
                var failing = _currentClassStudents.Where(s => !s.PassStatus).ToList();

                _autoTargetClassIdForPassers = null;
                _autoTargetClassNameForPassers = "";

                if (_currentIsLop5)
                {
                    lblNextClassPrompt.Text = "Trạng thái:";
                    lblPassingCount.Text = $"Đủ điều kiện Tốt Nghiệp ({passing.Count} hs)";

                    cboLopMoi_LenLop.DataSource = null;
                    cboLopMoi_LenLop.Items.Clear();
                    cboLopMoi_LenLop.Items.Add("Đã Tốt Nghiệp");
                    cboLopMoi_LenLop.SelectedIndex = 0;
                    cboLopMoi_LenLop.Enabled = false;
                }
                else
                {
                    lblNextClassPrompt.Text = "Lên lớp (Tự động):";
                    string targetName = DetermineNextClassName(tenLopCu, _currentKhoi);

                    _autoTargetClassIdForPassers = DatabaseHelper.GetClassIdByName(targetName);
                    _autoTargetClassNameForPassers = targetName;

                    List<string> displayList = new List<string>();
                    if (!string.IsNullOrEmpty(_autoTargetClassIdForPassers))
                    {
                        displayList.Add($"{targetName} (Tự động)");
                    }
                    else
                    {
                        displayList.Add($"Chưa tạo lớp {targetName}!");
                    }

                    cboLopMoi_LenLop.DataSource = displayList;
                    cboLopMoi_LenLop.Enabled = false;

                    lblPassingCount.Text = $"Đủ điều kiện Lên Lớp ({passing.Count} hs)";
                }

                pnlPassingStudents.Visible = true;
                pnlPassingStudents.BringToFront();

                pnlFailingStudents.Visible = true;
                pnlFailingStudents.BringToFront();

                lstPassingStudents.Items.Clear();
                foreach (var student in passing)
                {
                    lstPassingStudents.Items.Add(student);
                }

                lstFailingStudents.Items.Clear();
                foreach (var student in failing)
                {
                    lstFailingStudents.Items.Add(student);
                }

                lblFailingCount.Text = $"Lưu ban ({failing.Count} hs)";

                LoadFailersDestinationCombo(_currentKhoi, _currentMaLopCu);

                btnThucHienLenLop_SingleClass.Enabled = true;
                lblSummary.Text = "Phân tích hoàn tất. Vui lòng kiểm tra và thực hiện.";

                pnlPassingStudents.Refresh();
                pnlFailingStudents.Refresh();
                this.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu lớp: {ex.Message}", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblSummary.Text = "Lỗi khi tải dữ liệu!";
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private List<StudentPromotionInfo> CalculateStudentAverages(string maLop)
        {
            var list = new List<StudentPromotionInfo>();
            try
            {
                DataTable dt = DatabaseHelper.GetSemesterScoreboard(maLop, 3);

                if (dt == null || dt.Rows.Count == 0)
                {
                    Console.WriteLine("Không có dữ liệu điểm cho lớp: " + maLop);
                    return list;
                }

                foreach (DataRow row in dt.Rows)
                {
                    if (row["MaHS"] == DBNull.Value)
                        continue;

                    string maHS = row["MaHS"].ToString();
                    string hoTen = row["HoTen"]?.ToString() ?? "Không rõ";
                    double diemTB = 0;

                    if (dt.Columns.Contains("Trung bình chung") &&
                        row["Trung bình chung"] != DBNull.Value)
                    {
                        if (!double.TryParse(row["Trung bình chung"].ToString(), out diemTB))
                        {
                            diemTB = 0;
                        }
                    }

                    list.Add(new StudentPromotionInfo
                    {
                        MaHS = maHS,
                        HoTen = hoTen,
                        DiemTB = diemTB
                    });
                }

                Console.WriteLine($"Đã tính điểm cho {list.Count} học sinh trong lớp {maLop}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi CalculateStudentAverages: {ex.Message}");
            }
            return list;
        }

        private void LoadFailersDestinationCombo(string currentKhoi, string currentMaLop)
        {
            try
            {
                DataView dv = new DataView(_allLopHocCache);
                dv.RowFilter = $"Khoi = '{currentKhoi}'";

                cboLopMoi_OLaiLop.DataSource = dv.ToTable();
                cboLopMoi_OLaiLop.DisplayMember = "TenLop";
                cboLopMoi_OLaiLop.ValueMember = "MaLop";

                cboLopMoi_OLaiLop.SelectedValue = currentMaLop;
            }
            catch
            {
                // Ignore error
            }
        }

        private void btnThucHienLenLop_SingleClass_Click(object sender, EventArgs e)
        {
            if (_currentMaLopCu == null)
                return;

            bool missingTarget = string.IsNullOrEmpty(_autoTargetClassIdForPassers);
            if (!_currentIsLop5 && missingTarget && lstPassingStudents.Items.Count > 0)
            {
                string err = $"Chưa có lớp đích '{_autoTargetClassNameForPassers}'. " +
                           "Vui lòng tạo lớp này trước.";
                MessageBox.Show(err, "Thiếu lớp đích",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string maLopLuuBan = null;
            string tenLopLuuBan = "";
            if (lstFailingStudents.Items.Count > 0)
            {
                if (cboLopMoi_OLaiLop.SelectedValue == null)
                    return;
                maLopLuuBan = cboLopMoi_OLaiLop.SelectedValue.ToString();
                tenLopLuuBan = cboLopMoi_OLaiLop.Text;
            }

            int countPass = lstPassingStudents.Items.Count;
            int countFail = lstFailingStudents.Items.Count;
            int siSoHienTai_Pass = 0;

            if (!_currentIsLop5 && !string.IsNullOrEmpty(_autoTargetClassIdForPassers))
            {
                siSoHienTai_Pass = DatabaseHelper.GetCurrentClassCount(_autoTargetClassIdForPassers);
            }

            string msg = $"XÁC NHẬN XÉT LÊN LỚP CHO: {cboLopCu.Text.ToUpper()}\n\n";

            if (countPass > 0)
            {
                if (_currentIsLop5)
                {
                    msg += $"✅ TỐT NGHIỆP: {countPass} HS (Ra trường - Xóa khỏi lớp).\n";
                }
                else
                {
                    int tongDuKien = siSoHienTai_Pass + countPass;
                    msg += $"✅ LÊN LỚP: {countPass} HS -> {_autoTargetClassNameForPassers}\n";
                    msg += $"   (Sĩ số lớp đích: {siSoHienTai_Pass} + {countPass} = {tongDuKien})\n";
                }
            }

            if (countFail > 0)
            {
                msg += $"⚠️ LƯU BAN: {countFail} HS -> {tenLopLuuBan}.\n";
            }

            msg += "\nBạn có chắc chắn muốn thực hiện?";

            DialogResult dr = MessageBox.Show(msg, "Xác nhận Sĩ số & Lên lớp",
                                              MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                ExecutePromotion(countPass, countFail, maLopLuuBan);
            }
        }

        private void ExecutePromotion(int countPass, int countFail, string maLopLuuBan)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                var passList = ((List<StudentPromotionInfo>)lstPassingStudents.DataSource)
                               .Select(s => s.MaHS).ToList();

                if (passList.Any())
                {
                    string target = _currentIsLop5 ? null : _autoTargetClassIdForPassers;
                    DatabaseHelper.UpdateStudentClass_Multi(passList, target);
                }

                var failList = ((List<StudentPromotionInfo>)lstFailingStudents.DataSource)
                               .Select(s => s.MaHS).ToList();

                if (failList.Any() && !string.IsNullOrEmpty(maLopLuuBan))
                {
                    DatabaseHelper.UpdateStudentClass_Multi(failList, maLopLuuBan);
                }

                MessageBox.Show("Xử lý hoàn tất! Dữ liệu học sinh đã được cập nhật.");
                LoadLopCuComboBox();
                ResetLenLopUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi trong quá trình cập nhật: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region 4. Kho Lưu Trữ (Đã sửa: Thêm dòng hướng dẫn)

        private void SetupArchiveTab()
        {
            if (_tabKhoLuuTru != null)
                return;

            _tabKhoLuuTru = new TabPage("Kho Lưu Trữ");
            _tabKhoLuuTru.BackColor = Color.WhiteSmoke;

            // 1. PANEL BỘ LỌC (TOP)
            Panel pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                Padding = new Padding(5)
            };

            GroupBox grpFilter = new GroupBox
            {
                Text = "Bộ Lọc & Tìm Kiếm",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            Label lblNam = new Label
            {
                Text = "Năm Học:",
                Location = new Point(20, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };

            _cboArchiveNamHoc = new ComboBox
            {
                Location = new Point(95, 27),
                Width = 130,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };

            Label lblKhoi = new Label
            {
                Text = "Khối:",
                Location = new Point(260, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };

            _cboArchiveKhoi = new ComboBox
            {
                Location = new Point(305, 27),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };

            _cboArchiveKhoi.Items.AddRange(
                new string[] { "Tất cả", "Khối 1", "Khối 2", "Khối 3", "Khối 4", "Khối 5" }
            );
            _cboArchiveKhoi.SelectedIndex = 0;

            Label lblLop = new Label
            {
                Text = "Lớp:",
                Location = new Point(440, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };

            _cboArchiveLop = new ComboBox
            {
                Location = new Point(480, 27),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };

            Label lblSearch = new Label
            {
                Text = "Tìm HS:",
                Location = new Point(20, 75),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };

            _txtSearchStudent = new TextBox
            {
                Location = new Point(95, 72),
                Width = 310,
                Font = new Font("Segoe UI", 9.5f)
            };

            _lblArchiveGVCN = new Label
            {
                Text = "GVCN: ---",
                Location = new Point(440, 75),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255)
            };

            grpFilter.Controls.AddRange(new Control[] {
                lblNam, _cboArchiveNamHoc, lblKhoi, _cboArchiveKhoi,
                lblLop, _cboArchiveLop, lblSearch, _txtSearchStudent, _lblArchiveGVCN
            });
            pnlTop.Controls.Add(grpFilter);

            // 2. PANEL CHÍNH (SPLIT CONTAINER)
            var splitMain = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                FixedPanel = FixedPanel.Panel1
            };

            // Cột Trái: Danh sách Học sinh
            GroupBox grpStudents = new GroupBox
            {
                Text = "Danh Sách Học Sinh",
                Dock = DockStyle.Fill
            };

            // [NEW] Dòng chữ hướng dẫn
            Label lblInstruction = new Label
            {
                Text = "(* Nhấn đúp vào tên học sinh để mở bảng điểm)",
                Dock = DockStyle.Bottom,
                Height = 25,
                ForeColor = Color.Blue,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };

            _dgvArchiveStudents = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None
            };
            _dgvArchiveStudents.DoubleClick += DgvArchiveStudents_DoubleClick;

            // Thêm label hướng dẫn trước, rồi mới thêm Grid (để Grid fill phần còn lại)
            grpStudents.Controls.Add(_dgvArchiveStudents);
            grpStudents.Controls.Add(lblInstruction);

            // Cột Phải: Phân công Giảng dạy
            GroupBox grpTeachers = new GroupBox
            {
                Text = "Phân Công Giảng Dạy",
                Dock = DockStyle.Fill
            };

            _dgvArchiveTeachers = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None
            };
            grpTeachers.Controls.Add(_dgvArchiveTeachers);

            splitMain.Panel1.Controls.Add(grpStudents);
            splitMain.Panel2.Controls.Add(grpTeachers);

            // Thêm vào Tab
            _tabKhoLuuTru.Controls.Add(splitMain);
            _tabKhoLuuTru.Controls.Add(pnlTop);
            tabControlMain.TabPages.Add(_tabKhoLuuTru);

            splitMain.SplitterDistance = 600;

            _cboArchiveNamHoc.SelectedIndexChanged += LoadArchiveClassesForCombo;
            _cboArchiveKhoi.SelectedIndexChanged += LoadArchiveClassesForCombo;
            _cboArchiveLop.SelectedIndexChanged += CboArchiveLop_SelectedIndexChanged;
            _txtSearchStudent.TextChanged += TxtSearchStudent_TextChanged;

            LoadArchiveYears();
        }

        private void LoadArchiveYears()
        {
            try
            {
                DataTable dt = DatabaseHelper.GetArchiveYears();

                _cboArchiveNamHoc.SelectedIndexChanged -= LoadArchiveClassesForCombo;

                _cboArchiveNamHoc.DataSource = dt;
                _cboArchiveNamHoc.DisplayMember = "NamHoc";
                _cboArchiveNamHoc.ValueMember = "NamHoc";

                if (dt.Rows.Count == 0)
                {
                    _cboArchiveNamHoc.Items.Add(DateTime.Now.Year.ToString());
                    _cboArchiveNamHoc.SelectedIndex = 0;
                }

                _cboArchiveNamHoc.SelectedIndexChanged += LoadArchiveClassesForCombo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải năm học: " + ex.Message);
            }
        }

        private void LoadArchiveClassesForCombo(object sender, EventArgs e)
        {
            if (_cboArchiveNamHoc.Text == "")
                return;

            string nam = _cboArchiveNamHoc.SelectedValue?.ToString() ?? _cboArchiveNamHoc.Text;
            string khoi = _cboArchiveKhoi.SelectedItem?.ToString();

            try
            {
                DataTable dt = DatabaseHelper.GetArchiveClasses(nam, khoi);

                _cboArchiveLop.SelectedIndexChanged -= CboArchiveLop_SelectedIndexChanged;

                DataRow dr = dt.NewRow();
                dr["MaLop"] = DBNull.Value;
                dr["TenLop"] = "-- Chọn Lớp --";
                dt.Rows.InsertAt(dr, 0);

                _cboArchiveLop.DataSource = dt;
                _cboArchiveLop.DisplayMember = "TenLop";
                _cboArchiveLop.ValueMember = "MaLop";
                _cboArchiveLop.SelectedIndex = 0;

                _cboArchiveLop.SelectedIndexChanged += CboArchiveLop_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách lớp: " + ex.Message);
            }
        }

        private void CboArchiveLop_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_txtSearchStudent != null)
                    _txtSearchStudent.Clear();

                if (_cboArchiveLop.SelectedValue == null ||
                    _cboArchiveLop.SelectedValue == DBNull.Value ||
                    _cboArchiveLop.SelectedIndex == 0)
                {
                    _dgvArchiveStudents.DataSource = null;
                    _dgvArchiveTeachers.DataSource = null;
                    _lblArchiveGVCN.Text = "GVCN: ---";
                    return;
                }

                string maLop = _cboArchiveLop.SelectedValue.ToString();

                if (_cboArchiveLop.SelectedItem is DataRowView row)
                {
                    string maGVCN = row["MaGVCN"]?.ToString();
                    string tenGVCN = string.IsNullOrEmpty(maGVCN)
                                     ? "Chưa phân công"
                                     : DatabaseHelper.GetTeacherNameById(maGVCN);
                    _lblArchiveGVCN.Text = "GVCN: " + tenGVCN;
                }
                else
                {
                    _lblArchiveGVCN.Text = "GVCN: ---";
                }

                LoadArchiveStudents(maLop);
                LoadArchiveTeachers(maLop);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông tin lớp: " + ex.Message);
            }
        }

        private void LoadArchiveStudents(string maLop)
        {
            try
            {
                DataTable dtHS = DatabaseHelper.GetStudentsByClass(maLop);
                _dgvArchiveStudents.DataSource = dtHS;

                string[] hide = { "STT", "MaLop", "DiaChi", "DanToc", "SDTPhuHuynh" };
                foreach (var c in hide)
                {
                    if (_dgvArchiveStudents.Columns.Contains(c))
                        _dgvArchiveStudents.Columns[c].Visible = false;
                }

                _dgvArchiveStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (_dgvArchiveStudents.Columns.Contains("MaHS"))
                {
                    _dgvArchiveStudents.Columns["MaHS"].HeaderText = "Mã HS";
                    _dgvArchiveStudents.Columns["MaHS"].FillWeight = 20;
                }

                if (_dgvArchiveStudents.Columns.Contains("HoTen"))
                {
                    _dgvArchiveStudents.Columns["HoTen"].HeaderText = "Họ và Tên";
                    _dgvArchiveStudents.Columns["HoTen"].FillWeight = 50;
                }

                if (_dgvArchiveStudents.Columns.Contains("NgaySinh"))
                {
                    _dgvArchiveStudents.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
                    _dgvArchiveStudents.Columns["NgaySinh"].FillWeight = 20;
                    _dgvArchiveStudents.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    _dgvArchiveStudents.Columns["NgaySinh"].DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleCenter;
                }

                if (_dgvArchiveStudents.Columns.Contains("GioiTinh"))
                {
                    _dgvArchiveStudents.Columns["GioiTinh"].HeaderText = "Giới Tính";
                    _dgvArchiveStudents.Columns["GioiTinh"].FillWeight = 10;
                    _dgvArchiveStudents.Columns["GioiTinh"].DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleCenter;
                }

                _dgvArchiveStudents.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách học sinh: " + ex.Message);
            }
        }

        private void LoadArchiveTeachers(string maLop)
        {
            try
            {
                DataTable dtGV = DatabaseHelper.GetTeachingAssignmentsByClass(maLop);
                _dgvArchiveTeachers.DataSource = dtGV;

                if (_dgvArchiveTeachers.Columns.Contains("MaGV"))
                    _dgvArchiveTeachers.Columns["MaGV"].Visible = false;

                if (_dgvArchiveTeachers.Columns.Contains("MaMon"))
                    _dgvArchiveTeachers.Columns["MaMon"].Visible = false;

                _dgvArchiveTeachers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (_dgvArchiveTeachers.Columns.Contains("TenMon"))
                {
                    _dgvArchiveTeachers.Columns["TenMon"].HeaderText = "Môn Học";
                    _dgvArchiveTeachers.Columns["TenMon"].FillWeight = 45;
                }

                if (_dgvArchiveTeachers.Columns.Contains("TenGV"))
                {
                    _dgvArchiveTeachers.Columns["TenGV"].HeaderText = "Giáo Viên";
                    _dgvArchiveTeachers.Columns["TenGV"].FillWeight = 55;
                    _dgvArchiveTeachers.Columns["TenGV"].DefaultCellStyle.Font =
                        new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }

                _dgvArchiveTeachers.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải phân công giảng dạy: " + ex.Message);
            }
        }

        private void TxtSearchStudent_TextChanged(object sender, EventArgs e)
        {
            if (_dgvArchiveStudents.DataSource is DataTable dt)
            {
                string keyword = _txtSearchStudent.Text.Trim();
                string filter = string.IsNullOrEmpty(keyword)
                    ? ""
                    : $"HoTen LIKE '%{keyword}%' OR MaHS LIKE '%{keyword}%'";

                dt.DefaultView.RowFilter = filter;
            }
        }

        private void DgvArchiveStudents_DoubleClick(object sender, EventArgs e)
        {
            if (_dgvArchiveStudents.CurrentRow != null)
            {
                string maHS = _dgvArchiveStudents.CurrentRow.Cells["MaHS"].Value.ToString();
                string tenHS = _dgvArchiveStudents.CurrentRow.Cells["HoTen"].Value.ToString();

                FormBangDiem frm = new FormBangDiem(maHS, tenHS);
                frm.ShowDialog();
            }
        }

        #endregion

        #region TabControl Custom Drawing

        private void tabControlMain_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tc = sender as TabControl;
            TabPage page = tc.TabPages[e.Index];
            Rectangle rect = tc.GetTabRect(e.Index);

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color backColor = isSelected ? Color.FromArgb(0, 123, 255) : Color.WhiteSmoke;
            Color foreColor = isSelected ? Color.White : Color.Black;

            using (SolidBrush brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            TextRenderer.DrawText(e.Graphics, page.Text,
                new Font("Segoe UI", 10, isSelected ? FontStyle.Bold : FontStyle.Regular),
                rect, foreColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        #endregion
    }
}