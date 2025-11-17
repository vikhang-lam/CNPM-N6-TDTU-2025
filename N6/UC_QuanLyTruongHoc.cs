using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// UserControl quản lý nghiệp vụ toàn trường: Môn học, Thời hạn điểm, Lên lớp, Lưu trữ.
    /// </summary>
    public partial class UC_QuanLyTruongHoc : UserControl
    {
        #region Fields (Biến thành viên)

        // Cache dữ liệu
        private DataTable allLopHocCache;
        private DataTable allThoiHanDiemCache;

        // Dữ liệu cho tab Lên Lớp
        private List<StudentPromotionInfo> currentClassStudents = null;
        private string currentMaLopCu = null;
        private string currentKhoi = null;
        private bool currentIsLop5 = false;

        // Biến hỗ trợ tính toán Lên lớp tự động
        private string _autoTargetClassIdForPassers = null;
        private string _autoTargetClassNameForPassers = "";

        // Controls cho tab Kho Lưu Trữ (Dynamic UI)
        private TabPage tabKhoLuuTru;
        private ComboBox cboArchiveNamHoc;
        private ComboBox cboArchiveKhoi;
        private ComboBox cboArchiveLop;     // Thay thế Grid lớp cũ
        private TextBox txtSearchStudent;   // Thanh tìm kiếm
        private DataGridView dgvArchiveStudents;
        private DataGridView dgvArchiveTeachers;
        private Label lblArchiveGVCN;

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
            if (string.IsNullOrWhiteSpace(txtMaMon.Text)) return;

            DialogResult dr = MessageBox.Show($"Xóa môn '{txtTenMon.Text}'?", "Xác nhận",
                                              MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

        #region 2. Quản lý Thời Hạn Điểm

        private void LoadThoiHanDiem()
        {
            try
            {
                allThoiHanDiemCache = DatabaseHelper.GetScoreDeadlines();
                dgvThoiHanDiem.DataSource = allThoiHanDiemCache;

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

            var colSettings = new Dictionary<string, (string Header, int Weight)>
            {
                { "MaCotDiem", ("Mã", 15) },
                { "TenHienThi", ("Tên Cột Điểm", 30) },
                { "Khoi", ("Khối", 10) },
                { "HocKy", ("HK", 10) },
                { "NgayMoDiem", ("Ngày Mở", 15) },
                { "NgayKhoaDiem", ("Ngày Khóa", 15) },
                { "KhoaThuCong", ("Khóa Tay", 10) },
                { "DaKhoa", ("Trạng Thái", 10) }
            };

            foreach (var col in colSettings)
            {
                if (dgvThoiHanDiem.Columns.Contains(col.Key))
                {
                    dgvThoiHanDiem.Columns[col.Key].HeaderText = col.Value.Header;
                    dgvThoiHanDiem.Columns[col.Key].FillWeight = col.Value.Weight;
                }
            }

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
            if (dgv.Columns.Contains(colName) && !(dgv.Columns[colName] is DataGridViewCheckBoxColumn))
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
                DataTable changes = allThoiHanDiemCache.GetChanges(DataRowState.Modified);

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
                    allThoiHanDiemCache.AcceptChanges();
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
            if (allThoiHanDiemCache == null) return;

            string selected = cboKhoiFilter.SelectedItem?.ToString();
            string filter = (selected == "Tất cả" || selected == null) ? "" : $"Khoi = '{selected}'";

            allThoiHanDiemCache.DefaultView.RowFilter = filter;
        }

        #endregion

        #region 3. Quản lý Lên Lớp (Logic Tự động & Xác nhận Sĩ số)

        private class StudentPromotionInfo
        {
            public string MaHS { get; set; }
            public string HoTen { get; set; }
            public double DiemTB { get; set; }
            public bool PassStatus => DiemTB >= 5.0;

            public override string ToString()
            {
                return $"{HoTen} ({MaHS}) - ĐTB: {DiemTB:F2}";
            }
        }

        private void LoadLopCuComboBox()
        {
            try
            {
                allLopHocCache = DatabaseHelper.GetAllClasses();

                DataView dv = new DataView(allLopHocCache);
                dv.Sort = "Khoi, TenLop";
                DataTable dt = dv.ToTable();

                DataRow dr = dt.NewRow();
                dr["MaLop"] = DBNull.Value;
                dr["TenLop"] = "-- Chọn Lớp Cần Xét --";
                dt.Rows.InsertAt(dr, 0);

                cboLopCu.DataSource = dt;
                cboLopCu.DisplayMember = "TenLop";
                cboLopCu.ValueMember = "MaLop";
            }
            catch { }
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
            ResetLenLopUI();
        }

        private string DetermineNextClassName(string currentClassName, string khoiHienTai)
        {
            if (string.IsNullOrEmpty(currentClassName)) return null;

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
            if (cboLopCu.SelectedValue == null || cboLopCu.SelectedValue == DBNull.Value) return;

            currentMaLopCu = cboLopCu.SelectedValue.ToString();
            string tenLopCu = cboLopCu.Text;

            this.Cursor = Cursors.WaitCursor;
            lblSummary.Text = "Đang tính toán điểm và phân loại...";

            try
            {
                DataRow lopCuInfo = DatabaseHelper.GetClassDetails(currentMaLopCu);
                if (lopCuInfo == null) return;

                currentKhoi = lopCuInfo["Khoi"]?.ToString();
                currentIsLop5 = currentKhoi.Equals("Khối 5", StringComparison.OrdinalIgnoreCase);

                currentClassStudents = CalculateStudentAverages(currentMaLopCu);
                if (currentClassStudents == null || currentClassStudents.Count == 0)
                {
                    lblSummary.Text = "Lớp chưa có dữ liệu điểm hoặc không có học sinh.";
                    this.Cursor = Cursors.Default;
                    return;
                }

                var passing = currentClassStudents.Where(s => s.PassStatus).ToList();
                var failing = currentClassStudents.Where(s => !s.PassStatus).ToList();

                _autoTargetClassIdForPassers = null;
                _autoTargetClassNameForPassers = "";

                if (currentIsLop5)
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
                    string targetName = DetermineNextClassName(tenLopCu, currentKhoi);

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

                lstPassingStudents.DataSource = passing;
                pnlPassingStudents.Visible = true;

                lblFailingCount.Text = $"Lưu ban ({failing.Count} hs)";
                lstFailingStudents.DataSource = failing;
                pnlFailingStudents.Visible = true;

                LoadFailersDestinationCombo(currentKhoi, currentMaLopCu);

                btnThucHienLenLop_SingleClass.Enabled = true;
                lblSummary.Text = "Phân tích hoàn tất. Vui lòng kiểm tra và thực hiện.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private List<StudentPromotionInfo> CalculateStudentAverages(string maLop)
        {
            DataTable dt = DatabaseHelper.GetSemesterScoreboard(maLop, 3);
            var list = new List<StudentPromotionInfo>();

            foreach (DataRow row in dt.Rows)
            {
                if (row["MaHS"] == DBNull.Value) continue;

                double.TryParse(row["Trung bình chung"].ToString(), out double dtb);

                list.Add(new StudentPromotionInfo
                {
                    MaHS = row["MaHS"].ToString(),
                    HoTen = row["HoTen"].ToString(),
                    DiemTB = dtb
                });
            }
            return list;
        }

        private void LoadFailersDestinationCombo(string currentKhoi, string currentMaLop)
        {
            try
            {
                DataView dv = new DataView(allLopHocCache);
                dv.RowFilter = $"Khoi = '{currentKhoi}'";

                cboLopMoi_OLaiLop.DataSource = dv.ToTable();
                cboLopMoi_OLaiLop.DisplayMember = "TenLop";
                cboLopMoi_OLaiLop.ValueMember = "MaLop";

                cboLopMoi_OLaiLop.SelectedValue = currentMaLop;
            }
            catch { }
        }

        private void btnThucHienLenLop_SingleClass_Click(object sender, EventArgs e)
        {
            if (currentMaLopCu == null) return;

            bool missingTarget = string.IsNullOrEmpty(_autoTargetClassIdForPassers);
            if (!currentIsLop5 && missingTarget && lstPassingStudents.Items.Count > 0)
            {
                string err = $"Chưa có lớp đích '{_autoTargetClassNameForPassers}'. Vui lòng tạo lớp này trước.";
                MessageBox.Show(err, "Thiếu lớp đích", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string maLopLuuBan = null;
            string tenLopLuuBan = "";
            if (lstFailingStudents.Items.Count > 0)
            {
                if (cboLopMoi_OLaiLop.SelectedValue == null) return;
                maLopLuuBan = cboLopMoi_OLaiLop.SelectedValue.ToString();
                tenLopLuuBan = cboLopMoi_OLaiLop.Text;
            }

            int countPass = lstPassingStudents.Items.Count;
            int countFail = lstFailingStudents.Items.Count;
            int siSoHienTai_Pass = 0;

            if (!currentIsLop5 && !string.IsNullOrEmpty(_autoTargetClassIdForPassers))
            {
                siSoHienTai_Pass = DatabaseHelper.GetCurrentClassCount(_autoTargetClassIdForPassers);
            }

            string msg = $"XÁC NHẬN XÉT LÊN LỚP CHO: {cboLopCu.Text.ToUpper()}\n\n";

            if (countPass > 0)
            {
                if (currentIsLop5)
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
                    string target = currentIsLop5 ? null : _autoTargetClassIdForPassers;
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

        #region 4. Kho Lưu Trữ (Re-Layout & Optimized)

        private void SetupArchiveTab()
        {
            if (tabKhoLuuTru != null) return;

            tabKhoLuuTru = new TabPage("Kho Lưu Trữ");
            tabKhoLuuTru.BackColor = Color.WhiteSmoke;

            // 1. PANEL BỘ LỌC (TOP)
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 110, Padding = new Padding(5) };
            GroupBox grpFilter = new GroupBox { Text = "Bộ Lọc & Tìm Kiếm", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };

            // --- Các Controls Bộ lọc (Giữ nguyên như cũ) ---
            Label lblNam = new Label { Text = "Năm Học:", Location = new Point(20, 30), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Regular) };
            cboArchiveNamHoc = new ComboBox { Location = new Point(95, 27), Width = 130, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cboArchiveNamHoc.SelectedIndexChanged += (s, e) => LoadArchiveClassesForCombo();

            Label lblKhoi = new Label { Text = "Khối:", Location = new Point(260, 30), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Regular) };
            cboArchiveKhoi = new ComboBox { Location = new Point(305, 27), Width = 100, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cboArchiveKhoi.Items.AddRange(new string[] { "Tất cả", "Khối 1", "Khối 2", "Khối 3", "Khối 4", "Khối 5" });
            cboArchiveKhoi.SelectedIndex = 0;
            cboArchiveKhoi.SelectedIndexChanged += (s, e) => LoadArchiveClassesForCombo();

            Label lblLop = new Label { Text = "Lớp:", Location = new Point(440, 30), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Regular) };
            cboArchiveLop = new ComboBox { Location = new Point(480, 27), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cboArchiveLop.SelectedIndexChanged += CboArchiveLop_SelectedIndexChanged;

            Label lblSearch = new Label { Text = "Tìm HS:", Location = new Point(20, 75), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Regular) };
            txtSearchStudent = new TextBox { Location = new Point(95, 72), Width = 310, Font = new Font("Segoe UI", 9.5f) };
            txtSearchStudent.TextChanged += TxtSearchStudent_TextChanged;

            lblArchiveGVCN = new Label { Text = "GVCN: ---", Location = new Point(440, 75), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(0, 123, 255) };

            grpFilter.Controls.AddRange(new Control[] { lblNam, cboArchiveNamHoc, lblKhoi, cboArchiveKhoi, lblLop, cboArchiveLop, lblSearch, txtSearchStudent, lblArchiveGVCN });
            pnlTop.Controls.Add(grpFilter);

            // 2. PANEL CHÍNH (SPLIT CONTAINER)
            var splitMain = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                FixedPanel = FixedPanel.Panel1 // [QUAN TRỌNG] Cố định panel trái (Học sinh) để nó không bị giãn lung tung
            };

            // Cột Trái: Danh sách Học sinh
            GroupBox grpStudents = new GroupBox { Text = "Danh Sách Học Sinh", Dock = DockStyle.Fill };
            dgvArchiveStudents = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None
            };
            dgvArchiveStudents.DoubleClick += DgvArchiveStudents_DoubleClick;
            grpStudents.Controls.Add(dgvArchiveStudents);

            // Cột Phải: Phân công Giảng dạy
            GroupBox grpTeachers = new GroupBox { Text = "Phân Công Giảng Dạy", Dock = DockStyle.Fill };
            dgvArchiveTeachers = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None
            };
            grpTeachers.Controls.Add(dgvArchiveTeachers);

            splitMain.Panel1.Controls.Add(grpStudents);
            splitMain.Panel2.Controls.Add(grpTeachers);

            // Thêm vào Tab
            tabKhoLuuTru.Controls.Add(splitMain);
            tabKhoLuuTru.Controls.Add(pnlTop);
            tabControlMain.TabPages.Add(tabKhoLuuTru);

            // [CỰC KỲ QUAN TRỌNG] Set SplitterDistance Ở CUỐI CÙNG
            // 400 là chiều rộng của bảng Học sinh. Phần còn lại sẽ dành hết cho bảng Giáo viên.
            // Bạn có thể giảm xuống 350 nếu muốn bảng Giáo viên to hơn nữa.
            splitMain.SplitterDistance = 600;

            LoadArchiveYears();
        }

        private void LoadArchiveYears()
        {
            try
            {
                DataTable dt = DatabaseHelper.GetArchiveYears();
                cboArchiveNamHoc.DataSource = dt;
                cboArchiveNamHoc.DisplayMember = "NamHoc";
                cboArchiveNamHoc.ValueMember = "NamHoc";

                if (dt.Rows.Count == 0)
                {
                    cboArchiveNamHoc.Items.Add(DateTime.Now.Year.ToString());
                    cboArchiveNamHoc.SelectedIndex = 0;
                }
            }
            catch { }
        }

        private void LoadArchiveClassesForCombo()
        {
            if (cboArchiveNamHoc.Text == "") return;

            string nam = cboArchiveNamHoc.SelectedValue?.ToString() ?? cboArchiveNamHoc.Text;
            string khoi = cboArchiveKhoi.SelectedItem?.ToString();

            try
            {
                DataTable dt = DatabaseHelper.GetArchiveClasses(nam, khoi);

                DataRow dr = dt.NewRow();
                dr["MaLop"] = DBNull.Value;
                dr["TenLop"] = "-- Chọn Lớp --";
                dt.Rows.InsertAt(dr, 0);

                cboArchiveLop.DataSource = dt;
                cboArchiveLop.DisplayMember = "TenLop";
                cboArchiveLop.ValueMember = "MaLop";
                cboArchiveLop.SelectedIndex = 0;
            }
            catch { }
        }

        private void CboArchiveLop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtSearchStudent != null) txtSearchStudent.Clear();

            if (cboArchiveLop.SelectedValue == null || cboArchiveLop.SelectedValue == DBNull.Value)
            {
                dgvArchiveStudents.DataSource = null;
                dgvArchiveTeachers.DataSource = null;
                lblArchiveGVCN.Text = "GVCN: ---";
                return;
            }

            string maLop = cboArchiveLop.SelectedValue.ToString();

            DataRowView row = cboArchiveLop.SelectedItem as DataRowView;
            string maGVCN = row?["MaGVCN"]?.ToString();
            string tenGVCN = string.IsNullOrEmpty(maGVCN)
                             ? "Chưa phân công"
                             : DatabaseHelper.GetTeacherNameById(maGVCN);
            lblArchiveGVCN.Text = "GVCN: " + tenGVCN;

            LoadArchiveStudents(maLop);
            LoadArchiveTeachers(maLop);
        }

        private void LoadArchiveStudents(string maLop)
        {
            try
            {
                DataTable dtHS = DatabaseHelper.GetStudentsByClass(maLop);
                dgvArchiveStudents.DataSource = dtHS;

                string[] hide = { "STT", "MaLop", "DiaChi", "DanToc", "SDTPhuHuynh" };
                foreach (var c in hide)
                {
                    if (dgvArchiveStudents.Columns.Contains(c))
                        dgvArchiveStudents.Columns[c].Visible = false;
                }

                // Tự động giãn cột, nhưng set FillWeight để tránh thanh trượt
                dgvArchiveStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dgvArchiveStudents.Columns.Contains("MaHS"))
                {
                    dgvArchiveStudents.Columns["MaHS"].HeaderText = "Mã HS";
                    dgvArchiveStudents.Columns["MaHS"].FillWeight = 20;
                }

                if (dgvArchiveStudents.Columns.Contains("HoTen"))
                {
                    dgvArchiveStudents.Columns["HoTen"].HeaderText = "Họ và Tên";
                    dgvArchiveStudents.Columns["HoTen"].FillWeight = 50; // Tên cần rộng nhất
                }

                if (dgvArchiveStudents.Columns.Contains("NgaySinh"))
                {
                    dgvArchiveStudents.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
                    dgvArchiveStudents.Columns["NgaySinh"].FillWeight = 20;
                    dgvArchiveStudents.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvArchiveStudents.Columns["NgaySinh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (dgvArchiveStudents.Columns.Contains("GioiTinh"))
                {
                    dgvArchiveStudents.Columns["GioiTinh"].HeaderText = "Giới Tính";
                    dgvArchiveStudents.Columns["GioiTinh"].FillWeight = 10; // Cột nhỏ
                    dgvArchiveStudents.Columns["GioiTinh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                dgvArchiveStudents.ClearSelection();
            }
            catch { }
        }

        private void LoadArchiveTeachers(string maLop)
        {
            try
            {
                DataTable dtGV = DatabaseHelper.GetTeachingAssignmentsByClass(maLop);
                dgvArchiveTeachers.DataSource = dtGV;

                if (dgvArchiveTeachers.Columns.Contains("MaGV"))
                    dgvArchiveTeachers.Columns["MaGV"].Visible = false;

                if (dgvArchiveTeachers.Columns.Contains("MaMon"))
                    dgvArchiveTeachers.Columns["MaMon"].Visible = false;

                dgvArchiveTeachers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Điều chỉnh tỷ lệ cột: Môn học 45%, Giáo viên 55%
                if (dgvArchiveTeachers.Columns.Contains("TenMon"))
                {
                    dgvArchiveTeachers.Columns["TenMon"].HeaderText = "Môn Học";
                    dgvArchiveTeachers.Columns["TenMon"].FillWeight = 45; // Tăng lên 45 để hiển thị đủ tên dài
                }

                if (dgvArchiveTeachers.Columns.Contains("TenGV"))
                {
                    dgvArchiveTeachers.Columns["TenGV"].HeaderText = "Giáo Viên";
                    dgvArchiveTeachers.Columns["TenGV"].FillWeight = 55;
                    dgvArchiveTeachers.Columns["TenGV"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }

                dgvArchiveTeachers.ClearSelection();
            }
            catch { }
        }

        private void TxtSearchStudent_TextChanged(object sender, EventArgs e)
        {
            if (dgvArchiveStudents.DataSource is DataTable dt)
            {
                string keyword = txtSearchStudent.Text.Trim();
                string filter = string.IsNullOrEmpty(keyword)
                    ? ""
                    : $"HoTen LIKE '%{keyword}%' OR MaHS LIKE '%{keyword}%'";

                dt.DefaultView.RowFilter = filter;
            }
        }

        private void DgvArchiveStudents_DoubleClick(object sender, EventArgs e)
        {
            if (dgvArchiveStudents.CurrentRow != null)
            {
                string maHS = dgvArchiveStudents.CurrentRow.Cells["MaHS"].Value.ToString();
                string tenHS = dgvArchiveStudents.CurrentRow.Cells["HoTen"].Value.ToString();

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