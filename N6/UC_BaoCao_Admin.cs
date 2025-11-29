using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace N6
{
    public partial class UC_BaoCao_Admin : UserControl
    {
        #region Fields (Biến thành viên)

        private List<RadioButton> reportTypeRadioButtons = new List<RadioButton>();
        private DataTable allLopHoc;
        private DataTable allMonHoc;
        private bool isProgrammaticChange = false;

        #endregion

        #region Constructor & Load

        public UC_BaoCao_Admin()
        {
            InitializeComponent();
            ApplyModernStyles();
            LoadInitialAdminData();
            SetupEventHandlers();
            UpdateControlsVisibility();
        }

        private void ApplyModernStyles()
        {
            StyleDataGridViewModern(this.dgvDuLieu);

            foreach (Control ctrl in pnlReportTypeSelector.Controls)
            {
                if (ctrl is RadioButton rb)
                {
                    rb.Appearance = Appearance.Button;
                    rb.FlatStyle = FlatStyle.Flat;
                    rb.FlatAppearance.BorderSize = 0;
                    rb.FlatAppearance.CheckedBackColor = Color.FromArgb(0, 120, 215);
                    rb.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 235, 250);
                    rb.BackColor = Color.FromArgb(240, 240, 240);
                    rb.ForeColor = Color.Black;
                    rb.TextAlign = ContentAlignment.MiddleCenter;
                    rb.Padding = new Padding(10, 0, 10, 0);
                    rb.MinimumSize = new Size(150, 35);
                    rb.AutoSize = true;
                    reportTypeRadioButtons.Add(rb);
                    rb.CheckedChanged += ReportType_CheckedChanged;
                }
            }
            if (reportTypeRadioButtons.Count > 0)
            {
                // reportTypeRadioButtons[0].Checked = true;
            }

            StyleActionButton(btnXuatExcel, Color.FromArgb(16, 124, 65));
            StyleActionButton(btnXuatPDF, Color.FromArgb(217, 83, 79));

            splitContainer1.BackColor = Color.FromArgb(220, 220, 220);
        }

        private void StyleActionButton(Button btn, Color backColor)
        {
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI Semibold", 9.5F);
            btn.Size = new Size(130, 38);
            btn.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void StyleDataGridViewModern(DataGridView dgv)
        {
            if (dgv == null) return;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(242, 245, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 10, 0);
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(45, 45, 45);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Padding = new Padding(10, 0, 10, 0);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 252);
            dgv.RowTemplate.Height = 40;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void SetupEventHandlers()
        {
            this.cboLop.SelectedIndexChanged += new System.EventHandler(this.cboLop_SelectedIndexChanged_Handler);
            this.cboKhoi.SelectedIndexChanged += new System.EventHandler(this.cboKhoi_SelectedIndexChanged_Handler);
            this.cboHocKy.SelectedIndexChanged += new System.EventHandler(this.cboHocKy_SelectedIndexChanged_Handler);
            this.cboMonDay.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            this.cboThang.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            this.btnXuatExcel.Click += new System.EventHandler(this.btnXuatExcel_Click);
            this.btnXuatPDF.Click += new System.EventHandler(this.btnXuatPDF_Click);
            this.pnlFilters.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlFilters_Paint);
        }

        private void LoadInitialAdminData()
        {
            isProgrammaticChange = true;
            try
            {
                allLopHoc = DatabaseHelper.GetAllClasses();
                allMonHoc = DatabaseHelper.GetAllSubjects();

                var khoiList = allLopHoc.AsEnumerable()
                                        .Select(row => row.Field<string>("Khoi"))
                                        .Distinct()
                                        .OrderBy(k => k)
                                        .ToList();
                khoiList.Insert(0, "Tất cả các khối");
                cboKhoi.DataSource = khoiList;
                cboKhoi.SelectedIndex = -1;

                LoadAllSubjects();

                cboHocKy.Items.Clear();
                cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2", "Cả năm" });
                cboHocKy.SelectedIndex = -1;

                ResetPlaceholders();

                splitContainer1.Panel2Collapsed = true;
                ToggleNoDataMessage(true);

                if (reportTypeRadioButtons.Count > 0 && !reportTypeRadioButtons.Any(rb => rb.Checked))
                {
                    reportTypeRadioButtons[0].Checked = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu ban đầu cho Admin: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isProgrammaticChange = false;
            }
        }

        private void ResetPlaceholders()
        {
            isProgrammaticChange = true;
            cboKhoi.SelectedIndex = -1;
            cboKhoi.Text = "Chọn khối...";
            cboLop.DataSource = null;
            cboLop.Items.Clear();
            cboLop.Text = "Chọn lớp...";
            cboHocKy.SelectedIndex = -1;
            cboHocKy.Text = "Chọn học kỳ...";
            cboMonDay.SelectedIndex = -1;
            cboMonDay.Text = "Chọn môn...";
            cboThang.DataSource = null;
            cboThang.Items.Clear();
            cboThang.Text = "Chọn tháng...";
            isProgrammaticChange = false;
        }

        #endregion

        #region Event Handlers

        private void ReportType_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked && !isProgrammaticChange)
            {
                foreach (var button in reportTypeRadioButtons)
                {
                    button.ForeColor = button.Checked ? Color.White : Color.Black;
                    button.BackColor = button.Checked ? Color.FromArgb(0, 120, 215) : Color.FromArgb(240, 240, 240);
                }
                UpdateControlsVisibility();
                ClearReportData();
                ResetPlaceholders();
            }
        }

        private void cboKhoi_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            if (isProgrammaticChange || cboKhoi.SelectedIndex < 0) return;

            LoadClassesForSelectedGrade();

            string reportType = GetSelectedReportType();
            if (reportType == "Báo cáo tháng")
            {
                cboThang.DataSource = null;
                cboThang.Items.Clear();
                cboThang.Text = "Chọn học kỳ/lớp...";
            }
            AutoLoadReport_Trigger(sender, e);
        }

        private void cboLop_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            if (isProgrammaticChange) return;

            string reportType = GetSelectedReportType();
            if (reportType == "Báo cáo tháng")
            {
                LoadThangForSelectedHocKy();
            }
            AutoLoadReport_Trigger(sender, e);
        }

        private void cboHocKy_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            if (isProgrammaticChange) return;

            string reportType = GetSelectedReportType();
            if (reportType == "Báo cáo tháng")
            {
                LoadThangForSelectedHocKy();
            }
            AutoLoadReport_Trigger(sender, e);
        }

        #endregion

        #region Data Loading

        private void LoadThangForSelectedHocKy()
        {
            if (isProgrammaticChange) return;
            isProgrammaticChange = true;

            string selectedMaLopValue = cboLop.SelectedValue?.ToString();
            string selectedKhoiValue = cboKhoi.SelectedItem?.ToString();
            int hocKyIndex = cboHocKy.SelectedIndex;

            this.cboThang.DataSource = null;

            if (hocKyIndex == -1 || (selectedMaLopValue == null && selectedKhoiValue == null))
            {
                cboThang.Text = "Chọn lớp/học kỳ";
                isProgrammaticChange = false;
                return;
            }

            if (hocKyIndex > 1)
            {
                cboThang.Text = "Chọn HK1 hoặc 2";
                isProgrammaticChange = false;
                return;
            }

            int hocKy = hocKyIndex + 1;
            string maLopDeTim = null;
            if (!string.IsNullOrEmpty(selectedMaLopValue) && selectedMaLopValue != "ALL" && selectedMaLopValue != "ALL_KHOI")
            {
                maLopDeTim = selectedMaLopValue;
            }
            else if (!string.IsNullOrEmpty(selectedKhoiValue) && selectedKhoiValue != "Tất cả các khối")
            {
                maLopDeTim = allLopHoc.AsEnumerable()
                                    .Where(r => r.Field<string>("Khoi") == selectedKhoiValue)
                                    .Select(r => r.Field<string>("MaLop"))
                                    .FirstOrDefault();
            }

            if (string.IsNullOrEmpty(maLopDeTim))
            {
                cboThang.Text = "Chưa rõ khối";
                isProgrammaticChange = false;
                return;
            }

            try
            {
                DataTable dtThang = DatabaseHelper.GetMonthlyScoreTypes(maLopDeTim, hocKy);
                if (dtThang != null && dtThang.Rows.Count > 0)
                {
                    cboThang.DataSource = dtThang;
                    cboThang.DisplayMember = "TenHienThi";
                    cboThang.ValueMember = "MaCotDiem";
                    cboThang.SelectedIndex = -1;
                    cboThang.Text = "Chọn tháng...";
                }
                else
                {
                    cboThang.Text = "Không có cột điểm";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách tháng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isProgrammaticChange = false;
            }
        }

        private void LoadAllSubjects()
        {
            isProgrammaticChange = true;
            try
            {
                DataTable dtMonDisplay = allMonHoc.Copy();
                DataRow allRow = dtMonDisplay.NewRow();
                allRow["MaMon"] = "ALL";
                allRow["TenMon"] = "Tất cả các môn";
                dtMonDisplay.Rows.InsertAt(allRow, 0);

                cboMonDay.DataSource = dtMonDisplay;
                cboMonDay.DisplayMember = "TenMon";
                cboMonDay.ValueMember = "MaMon";
                cboMonDay.SelectedIndex = -1;
                cboMonDay.Text = "Chọn môn...";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách môn học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isProgrammaticChange = false;
            }
        }

        private void LoadClassesForSelectedGrade()
        {
            isProgrammaticChange = true;
            cboLop.DataSource = null;
            cboLop.Items.Clear();

            try
            {
                string selectedKhoi = cboKhoi.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(selectedKhoi) || selectedKhoi == "Tất cả các khối")
                {
                    var allClassList = allLopHoc.AsEnumerable()
                                       .OrderBy(row => row.Field<string>("Khoi"))
                                       .ThenBy(row => row.Field<string>("TenLop"))
                                       .CopyToDataTable();
                    if (selectedKhoi == "Tất cả các khối")
                    {
                        DataRow allRow = allClassList.NewRow();
                        allRow["MaLop"] = "ALL";
                        allRow["TenLop"] = "Tất cả các lớp";
                        allClassList.Rows.InsertAt(allRow, 0);
                    }
                    cboLop.DataSource = allClassList;
                }
                else
                {
                    var filteredClasses = allLopHoc.AsEnumerable()
                                            .Where(row => row.Field<string>("Khoi") == selectedKhoi)
                                            .OrderBy(row => row.Field<string>("TenLop"))
                                            .CopyToDataTable();

                    DataRow allInGradeRow = filteredClasses.NewRow();
                    allInGradeRow["MaLop"] = "ALL_KHOI";
                    allInGradeRow["TenLop"] = $" ({selectedKhoi})";
                    filteredClasses.Rows.InsertAt(allInGradeRow, 0);

                    cboLop.DataSource = filteredClasses;
                }

                cboLop.DisplayMember = "TenLop";
                cboLop.ValueMember = "MaLop";
                cboLop.SelectedIndex = -1;
                cboLop.Text = "Chọn lớp...";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cboLop.Text = "Lỗi tải lớp";
            }
            finally
            {
                isProgrammaticChange = false;
            }
        }

        #endregion

        #region Report Logic

        private string GetSelectedReportType()
        {
            foreach (var rb in reportTypeRadioButtons)
            {
                if (rb.Checked) return rb.Text;
            }
            return null;
        }

        private void UpdateControlsVisibility()
        {
            string reportType = GetSelectedReportType();

            lblKhoi.Visible = cboKhoi.Visible = true;
            lblLop.Visible = cboLop.Visible = true;
            lblHocKy.Visible = cboHocKy.Visible = true;
            lblMonDay.Visible = cboMonDay.Visible = true;
            lblThang.Visible = cboThang.Visible = false;

            isProgrammaticChange = true;
            cboHocKy.Items.Clear();
            cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2", "Cả năm" });
            isProgrammaticChange = false;

            switch (reportType)
            {
                case "Báo cáo chuyên cần":
                    lblMonDay.Visible = cboMonDay.Visible = false;
                    break;

                case "Bảng điểm học kỳ":
                    isProgrammaticChange = true;
                    cboHocKy.Items.Clear();
                    cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2" });
                    isProgrammaticChange = false;
                    break;

                case "Hồ sơ học sinh":
                    lblHocKy.Visible = cboHocKy.Visible = false;
                    lblMonDay.Visible = cboMonDay.Visible = false;
                    break;

                case "Thống kê tổng hợp khối":
                    lblLop.Visible = cboLop.Visible = false;
                    lblHocKy.Visible = cboHocKy.Visible = false;
                    lblMonDay.Visible = cboMonDay.Visible = false;
                    break;

                case "Báo cáo tháng":
                    lblThang.Visible = cboThang.Visible = true;
                    isProgrammaticChange = true;
                    cboHocKy.Items.Clear();
                    cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2" });
                    isProgrammaticChange = false;
                    LoadThangForSelectedHocKy();
                    break;

                default:
                    lblKhoi.Visible = cboKhoi.Visible = false;
                    lblLop.Visible = cboLop.Visible = false;
                    lblHocKy.Visible = cboHocKy.Visible = false;
                    lblMonDay.Visible = cboMonDay.Visible = false;
                    break;
            }
        }

        private void AutoLoadReport_Trigger(object sender, EventArgs e)
        {
            if (isProgrammaticChange) return;
            if (sender is ComboBox cbo && cbo.SelectedIndex < 0) return;

            string reportType = GetSelectedReportType();
            if (string.IsNullOrEmpty(reportType)) return;

            bool canLoad = false;
            switch (reportType)
            {
                case "Báo cáo chuyên cần":
                    canLoad = cboKhoi.SelectedIndex != -1 && cboLop.SelectedIndex != -1 && cboHocKy.SelectedIndex != -1;
                    break;
                case "Bảng điểm học kỳ":
                    canLoad = cboKhoi.SelectedIndex != -1 && cboLop.SelectedIndex != -1 && cboHocKy.SelectedIndex != -1 && cboMonDay.SelectedIndex != -1;
                    break;
                case "Hồ sơ học sinh":
                    canLoad = cboKhoi.SelectedIndex != -1 && cboLop.SelectedIndex != -1;
                    break;
                case "Thống kê tổng hợp khối":
                    canLoad = cboKhoi.SelectedIndex != -1;
                    break;
                case "Báo cáo tháng":
                    canLoad = cboKhoi.SelectedIndex != -1 && cboLop.SelectedIndex != -1 && cboHocKy.SelectedIndex != -1 && cboMonDay.SelectedIndex != -1 && cboThang.SelectedIndex != -1;
                    break;
            }

            if (canLoad) LoadReportData();
        }

        private void LoadReportData()
        {
            string reportType = GetSelectedReportType();
            if (string.IsNullOrEmpty(reportType)) return;

            DataTable dtReport = null;
            try
            {
                string selectedKhoi = cboKhoi.SelectedItem?.ToString();
                string selectedMaLopValue = cboLop.SelectedValue?.ToString();
                int selectedHocKyIndex = cboHocKy.SelectedIndex;
                int hocKyParam = selectedHocKyIndex + 1;
                string selectedMaMon = cboMonDay.SelectedValue?.ToString();
                string selectedLoaiDiem = cboThang.SelectedValue?.ToString();

                string khoiParam = (selectedKhoi == "Tất cả các khối") ? null : selectedKhoi;
                string maLopParam_Admin = (selectedMaLopValue == "ALL" || selectedMaLopValue == "ALL_KHOI") ? null : selectedMaLopValue;
                string maMonParam_Pivot = (selectedMaMon == "ALL") ? null : selectedMaMon;

                bool isSpecificClass = !string.IsNullOrEmpty(selectedMaLopValue) && selectedMaLopValue != "ALL" && selectedMaLopValue != "ALL_KHOI";

                switch (reportType)
                {
                    case "Báo cáo chuyên cần":
                        dtReport = isSpecificClass ? DatabaseHelper.GetAttendanceReport(maLopParam_Admin, hocKyParam) : DatabaseHelper.GetAttendanceReport_Admin(khoiParam, maLopParam_Admin, hocKyParam);
                        break;
                    case "Bảng điểm học kỳ":
                        if (!string.IsNullOrEmpty(maMonParam_Pivot))
                        {
                            if (isSpecificClass)
                            {
                                dtReport = DatabaseHelper.GetScoreboardPivot(maLopParam_Admin, hocKyParam, maMonParam_Pivot);
                                CalculateAndAddAverageColumn(dtReport);
                            }
                            else
                            {
                                MessageBox.Show("Vui lòng chọn một lớp cụ thể để xem bảng điểm chi tiết theo môn.", "Yêu cầu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ClearReportData(); return;
                            }
                        }
                        else
                        {
                            dtReport = isSpecificClass ? DatabaseHelper.GetSemesterScoreboard(maLopParam_Admin, hocKyParam) : DatabaseHelper.GetSemesterScoreboard_Admin(khoiParam, maLopParam_Admin, hocKyParam);
                        }
                        break;
                    case "Hồ sơ học sinh":
                        dtReport = isSpecificClass ? DatabaseHelper.GetStudentRecords(maLopParam_Admin) : DatabaseHelper.GetStudentRecords_Admin(khoiParam, maLopParam_Admin);
                        break;
                    case "Thống kê tổng hợp khối":
                        dtReport = DatabaseHelper.GetGradeStatistics_Admin(khoiParam);
                        break;
                    case "Báo cáo tháng":
                        dtReport = isSpecificClass ? DatabaseHelper.GetMonthlyReport_Statistics(maLopParam_Admin, selectedMaMon, selectedLoaiDiem) : DatabaseHelper.GetMonthlyReport_Statistics_Admin(khoiParam, selectedMaMon, selectedLoaiDiem);
                        break;
                }

                DisplayReportData(dtReport, reportType);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearReportData();
            }
        }

        private void ClearReportData()
        {
            dgvDuLieu.DataSource = null;
            flpCharts.Controls.Clear();
            splitContainer1.Panel2Collapsed = true;
            ToggleNoDataMessage(true);
        }

        private void DisplayReportData(DataTable dtReport, string reportType)
        {
            if (reportType == "Báo cáo tháng")
            {
                dgvDuLieu.DataSource = null;
                flpCharts.Controls.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    dgvDuLieu.ColumnHeadersVisible = false;
                    DisplayCombinedMonthlyReport(dtReport);
                    splitContainer1.Panel2Collapsed = true;
                }
                else
                {
                    dgvDuLieu.ColumnHeadersVisible = true;
                    splitContainer1.Panel2Collapsed = true;
                    ToggleNoDataMessage(true);
                }
            }
            else
            {
                dgvDuLieu.ColumnHeadersVisible = true;
                dgvDuLieu.DataSource = dtReport;
                flpCharts.Controls.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    ToggleNoDataMessage(false);
                    RenameDataGridViewColumns();
                    UpdateCharts(dtReport, reportType);
                    splitContainer1.Panel2Collapsed = flpCharts.Controls.Count == 0;
                }
                else
                {
                    splitContainer1.Panel2Collapsed = true;
                    ToggleNoDataMessage(true);
                }
            }
        }

        private void DisplayCombinedMonthlyReport(DataTable dtReport)
        {
            // 1. Dọn dẹp giao diện và DỮ LIỆU CŨ
            flpCharts.Controls.Clear();
            splitContainer1.Panel2Collapsed = true;

            // [FIX QUAN TRỌNG] Xóa sạch cột cũ để tránh bị chồng cột hoặc dư cột ẩn
            dgvDuLieu.DataSource = null;
            dgvDuLieu.Columns.Clear();

            // Kiểm tra loại báo cáo
            bool isAdminReport = dtReport.Columns.Contains("TyLe");

            // 2. Map dữ liệu (Giữ nguyên logic cũ)
            var dataMapDiem = dtReport.Select("LoaiThongKe = 'Diem'")
                                      .ToDictionary(r => r["PhanLoai"].ToString(), r => r);
            var dataMapXepLoai = dtReport.Select("LoaiThongKe = 'XepLoai'")
                                         .ToDictionary(r => r["PhanLoai"].ToString(), r => r);

            // 3. Tạo DataTable kết hợp
            DataTable dtCombined = new DataTable();
            dtCombined.Columns.Add("PhanLoai", typeof(string));
            dtCombined.Columns.Add("Col_TS", typeof(string));
            dtCombined.Columns.Add("Col_Nu", typeof(string));
            dtCombined.Columns.Add("Col_DanToc_Percent", typeof(string));
            dtCombined.Columns.Add("Col_NDT", typeof(string));
            dtCombined.Columns.Add("IsHeader", typeof(int));

            // --- PHẦN 1: ĐIỂM SỐ ---
            dtCombined.Rows.Add("ĐIỂM", "TS", "Nữ", "Dân tộc", "NDT", 1);

            string[] scoreOrder = { "10", "9", "8", "7", "6", "5", "Dưới 5" };
            foreach (string key in scoreOrder)
            {
                string ts = "0", nu = "0", dtoc = "0", ndt = "0";
                string dbKey = key;

                // Fix lỗi tìm key "Dưới 5" vs "<5"
                bool found = dataMapDiem.ContainsKey(key);
                if (!found && key == "Dưới 5" && dataMapDiem.ContainsKey("<5")) { dbKey = "<5"; found = true; }

                if (found)
                {
                    ts = dataMapDiem[dbKey]["TS"].ToString();
                    nu = dataMapDiem[dbKey]["Nu"].ToString();
                    dtoc = dataMapDiem[dbKey]["DanToc"].ToString();
                    ndt = dataMapDiem[dbKey]["NDT"].ToString();
                }
                dtCombined.Rows.Add(key, ts, nu, dtoc, ndt, 0);
            }

            // Tính tổng Điểm
            int totalTS = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["TS"]));
            int totalNu = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["Nu"]));
            int totalDanToc = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["DanToc"]));
            int totalNDT = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["NDT"]));
            dtCombined.Rows.Add("Tổng", totalTS.ToString(), totalNu.ToString(), totalDanToc.ToString(), totalNDT.ToString(), 0);

            // --- PHẦN 2: XẾP LOẠI ---
            if (isAdminReport)
                dtCombined.Rows.Add("XẾP LOẠI", "TS", "", "%", "", 1);
            else
                dtCombined.Rows.Add("XẾP LOẠI", "TS", "Nữ", "Dân tộc", "NDT", 1);

            string[] rankOrder = { "T", "H", "C" };
            foreach (string key in rankOrder)
            {
                string ts = "0", col2 = "0", col3 = "0", col4 = "0";

                if (dataMapXepLoai.ContainsKey(key))
                {
                    ts = dataMapXepLoai[key]["TS"].ToString();
                    if (isAdminReport)
                    {
                        col2 = "";
                        col3 = Convert.ToDouble(dataMapXepLoai[key]["TyLe"]).ToString("N1") + "%";
                        col4 = "";
                    }
                    else
                    {
                        col2 = dataMapXepLoai[key]["Nu"].ToString();
                        col3 = dataMapXepLoai[key]["DanToc"].ToString();
                        col4 = dataMapXepLoai[key]["NDT"].ToString();
                    }
                }
                else if (isAdminReport)
                {
                    col2 = ""; col3 = "0.0%"; col4 = "";
                }
                dtCombined.Rows.Add(key, ts, col2, col3, col4, 0);
            }

            // 4. Gán dữ liệu
            dgvDuLieu.DataSource = dtCombined;

            // [FIX QUAN TRỌNG] Duyệt từng cột để ẩn IsHeader chắc chắn 100%
            foreach (DataGridViewColumn col in dgvDuLieu.Columns)
            {
                if (col.DataPropertyName == "IsHeader")
                {
                    col.Visible = false;
                }
            }

            ToggleNoDataMessage(false);
            FormatCombinedMonthlyReportGrid(dgvDuLieu, isAdminReport);
        }

        private void FormatCombinedMonthlyReportGrid(DataGridView dgv, bool isAdminReport)
        {
            if (dgv.DataSource == null) return;

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                // Căn giữa toàn bộ header
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                switch (col.DataPropertyName)
                {
                    case "PhanLoai":
                        col.HeaderText = ""; // Cột tên loại (10, 9, T, H...)
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        break;
                    case "Col_TS": col.HeaderText = "TS"; break;
                    case "Col_Nu": col.HeaderText = "Nữ"; break;
                    case "Col_DanToc_Percent": col.HeaderText = "Dân tộc"; break;
                    case "Col_NDT": col.HeaderText = "NDT"; break;

                    // [QUAN TRỌNG] Đảm bảo ẩn ở đây nữa
                    case "IsHeader":
                        col.Visible = false;
                        break;
                }
            }

            // Tô màu nền cho hàng tiêu đề (ĐIỂM, XẾP LOẠI) và hàng Tổng
            foreach (DataGridViewRow row in dgv.Rows)
            {
                // Kiểm tra an toàn null
                if (row.Cells["IsHeader"].Value == DBNull.Value) continue;

                bool isHeader = Convert.ToInt32(row.Cells["IsHeader"].Value) == 1;
                string phanLoai = row.Cells["PhanLoai"].Value?.ToString();

                if (isHeader)
                {
                    row.DefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
                    row.DefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    row.Cells["PhanLoai"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (phanLoai == "Tổng")
                {
                    row.DefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                }
            }
        }

        private void ToggleNoDataMessage(bool showMessage)
        {
            if (lblNoData != null)
            {
                lblNoData.Visible = showMessage;
                if (showMessage)
                {
                    lblNoData.BringToFront();
                    dgvDuLieu.Visible = false;
                }
                else
                {
                    dgvDuLieu.Visible = true;
                }
            }
        }

        #endregion

        #region Charting

        private void UpdateCharts(DataTable dt, string reportType)
        {
            switch (reportType)
            {
                case "Báo cáo chuyên cần":
                    DrawAttendancePieChart(dt);
                    break;
                case "Bảng điểm học kỳ":
                    DrawScoreDistributionChart(dt);
                    break;
                case "Thống kê tổng hợp khối":
                    DrawStudentCountChart(dt);
                    DrawAverageScoreChart(dt);
                    break;
            }
        }

        private Chart CreateBaseChart(string title)
        {
            var chart = new Chart { Size = new Size(450, 250), BackColor = Color.White };
            chart.Titles.Add(new Title(title, Docking.Top, new Font("Segoe UI Semibold", 11F), Color.FromArgb(64, 64, 64)));
            var chartArea = new ChartArea("MainArea");
            chartArea.BackColor = Color.White;
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(230, 230, 230);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(230, 230, 230);
            chartArea.AxisX.LineColor = Color.FromArgb(200, 200, 200);
            chartArea.AxisY.LineColor = Color.FromArgb(200, 200, 200);
            chartArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 9F);
            chartArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 9F);
            chartArea.AxisX.TitleFont = new Font("Segoe UI", 9F);
            chartArea.AxisY.TitleFont = new Font("Segoe UI", 9F);
            chart.ChartAreas.Add(chartArea);
            return chart;
        }

        private void DrawAttendancePieChart(DataTable dt)
        {
            if (!dt.Columns.Contains("SoBuoiCoMat") || !dt.Columns.Contains("SoBuoiVang") || !dt.Columns.Contains("SoBuoiVangCoPhep")) return;
            long totalPresent = dt.AsEnumerable().Sum(row => row.Field<object>("SoBuoiCoMat") == DBNull.Value ? 0 : Convert.ToInt64(row["SoBuoiCoMat"]));
            long totalAbsent = dt.AsEnumerable().Sum(row => row.Field<object>("SoBuoiVang") == DBNull.Value ? 0 : Convert.ToInt64(row["SoBuoiVang"]));
            long totalExcused = dt.AsEnumerable().Sum(row => row.Field<object>("SoBuoiVangCoPhep") == DBNull.Value ? 0 : Convert.ToInt64(row["SoBuoiVangCoPhep"]));
            long totalAll = totalPresent + totalAbsent + totalExcused;
            if (totalAll == 0) return;

            var chart = CreateBaseChart("Tỷ lệ chuyên cần chung");
            chart.Legends.Add(new Legend("Default") { Docking = Docking.Right, Font = new Font("Segoe UI", 9F) });

            var series = new Series("Chuyên cần")
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 9F),
                LabelForeColor = Color.White,
                CustomProperties = "PieLabelStyle=Outside"
            };

            if (totalPresent > 0)
            {
                series.Points.Add(totalPresent);
                int pIdx = series.Points.Count - 1;
                series.Points[pIdx].Color = Color.FromArgb(75, 192, 192);
                series.Points[pIdx].LegendText = $"Có mặt ({totalPresent})";
                series.Points[pIdx].Label = ((double)totalPresent / totalAll).ToString("P1");
            }
            if (totalAbsent > 0)
            {
                series.Points.Add(totalAbsent);
                int pIdx = series.Points.Count - 1;
                series.Points[pIdx].Color = Color.FromArgb(255, 99, 132);
                series.Points[pIdx].LegendText = $"Vắng ({totalAbsent})";
                series.Points[pIdx].Label = ((double)totalAbsent / totalAll).ToString("P1");
            }
            if (totalExcused > 0)
            {
                series.Points.Add(totalExcused);
                int pIdx = series.Points.Count - 1;
                series.Points[pIdx].Color = Color.FromArgb(255, 205, 86);
                series.Points[pIdx].LegendText = $"Vắng có phép ({totalExcused})";
                series.Points[pIdx].Label = ((double)totalExcused / totalAll).ToString("P1");
            }
            chart.Series.Add(series);
            flpCharts.Controls.Add(chart);
        }

        private void DrawScoreDistributionChart(DataTable dt)
        {
            string scoreColumnName = "";
            if (dt.Columns.Contains("DiemTB")) scoreColumnName = "DiemTB";
            else if (dt.Columns.Contains("Trung bình chung")) scoreColumnName = "Trung bình chung";
            else if (dt.Columns.Contains("DiemTrungBinh")) scoreColumnName = "DiemTrungBinh";
            if (string.IsNullOrEmpty(scoreColumnName)) return;

            var scoreCounts = new Dictionary<string, int> { { "Yếu (<5)", 0 }, { "TB (5-<7)", 0 }, { "Khá (7-<9)", 0 }, { "Giỏi (>=9)", 0 } };
            int validScores = 0;
            foreach (DataRow row in dt.Rows)
            {
                object scoreValue = row[scoreColumnName];
                if (scoreValue != DBNull.Value && scoreValue != null)
                {
                    try
                    {
                        double score = Convert.ToDouble(scoreValue);
                        validScores++;
                        if (score < 5) scoreCounts["Yếu (<5)"]++;
                        else if (score < 7) scoreCounts["TB (5-<7)"]++;
                        else if (score < 9) scoreCounts["Khá (7-<9)"]++;
                        else scoreCounts["Giỏi (>=9)"]++;
                    }
                    catch { }
                }
            }
            if (validScores == 0) return;

            var chart = CreateBaseChart($"Phân loại học lực ({validScores} HS có điểm)");
            chart.ChartAreas[0].AxisX.Title = "Loại học lực";
            chart.ChartAreas[0].AxisY.Title = "Số lượng học sinh";

            var series = new Series("Học lực") { ChartType = SeriesChartType.Column };
            int pointIndex = 0;
            var colors = new Color[] { Color.FromArgb(255, 99, 132), Color.FromArgb(255, 205, 86), Color.FromArgb(75, 192, 192), Color.FromArgb(54, 162, 235) };

            foreach (var kvp in scoreCounts)
            {
                if (kvp.Value > 0)
                {
                    int pIdx = series.Points.AddXY(kvp.Key, kvp.Value);
                    series.Points[pIdx].ToolTip = $"{kvp.Key}: {kvp.Value} HS ({(double)kvp.Value / validScores:P1})";
                    series.Points[pIdx].Label = kvp.Value.ToString();
                    series.Points[pIdx].Color = colors[pointIndex % colors.Length];
                }
                pointIndex++;
            }
            series["PixelPointWidth"] = "40";
            chart.Series.Add(series);
            flpCharts.Controls.Add(chart);
        }

        private void DrawStudentCountChart(DataTable dt)
        {
            if (!dt.Columns.Contains("TenLop") || !dt.Columns.Contains("SoHocSinh")) return;
            var chart = CreateBaseChart("Thống kê sĩ số theo lớp");
            chart.ChartAreas[0].AxisX.Title = "Lớp";
            chart.ChartAreas[0].AxisY.Title = "Sĩ số";
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chart.ChartAreas[0].AxisX.Interval = 1;

            var series = new Series("Sĩ số")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 9F),
                LabelForeColor = Color.FromArgb(64, 64, 64)
            };

            foreach (DataRow row in dt.Rows)
            {
                if (row["SoHocSinh"] != DBNull.Value && row["TenLop"] != DBNull.Value)
                {
                    try
                    {
                        int pIdx = series.Points.AddXY(row["TenLop"].ToString(), Convert.ToInt32(row["SoHocSinh"]));
                        series.Points[pIdx].Color = Color.FromArgb(54, 162, 235);
                    }
                    catch { }
                }
            }
            series["PixelPointWidth"] = "30";
            chart.Series.Add(series);
            flpCharts.Controls.Add(chart);
        }

        private void DrawAverageScoreChart(DataTable dt)
        {
            if (!dt.Columns.Contains("TenLop") || !dt.Columns.Contains("DiemTrungBinh")) return;
            var chart = CreateBaseChart("Điểm trung bình theo lớp");
            chart.ChartAreas[0].AxisX.Title = "Lớp";
            chart.ChartAreas[0].AxisY.Title = "Điểm trung bình";
            chart.ChartAreas[0].AxisY.Maximum = 10;
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chart.ChartAreas[0].AxisX.Interval = 1;

            var series = new Series("Điểm TB")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 9F),
                LabelForeColor = Color.FromArgb(64, 64, 64),
                LabelFormat = "N1"
            };

            foreach (DataRow row in dt.Rows)
            {
                if (row["DiemTrungBinh"] != DBNull.Value && row["TenLop"] != DBNull.Value)
                {
                    try
                    {
                        int pIdx = series.Points.AddXY(row["TenLop"].ToString(), Convert.ToDouble(row["DiemTrungBinh"]));
                        series.Points[pIdx].Color = Color.FromArgb(75, 192, 192);
                    }
                    catch { }
                }
            }
            series["PixelPointWidth"] = "30";
            chart.Series.Add(series);
            flpCharts.Controls.Add(chart);
        }

        private void CalculateAndAddAverageColumn(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;
            if (!dt.Columns.Contains("DiemTB")) dt.Columns.Add("DiemTB", typeof(double));
            var scoreCols = dt.Columns.Cast<DataColumn>()
                                .Where(c => c.DataType == typeof(double) || c.DataType == typeof(float) || c.DataType == typeof(decimal))
                                .Where(c => c.ColumnName != "DiemTB" && c.ColumnName != "Trung bình chung" && c.ColumnName != "DiemTrungBinh")
                                .Select(c => c.ColumnName).ToList();

            foreach (DataRow row in dt.Rows)
            {
                double totalScore = 0;
                int validScoresCount = 0;
                foreach (string colName in scoreCols)
                {
                    if (row[colName] != DBNull.Value)
                    {
                        try
                        {
                            totalScore += Convert.ToDouble(row[colName]);
                            validScoresCount++;
                        }
                        catch { }
                    }
                }
                if (validScoresCount > 0) row["DiemTB"] = Math.Round(totalScore / validScoresCount, 2);
                else row["DiemTB"] = DBNull.Value;
            }
        }

        #endregion

        #region Formatting & Export

        private void RenameDataGridViewColumns()
        {
            if (dgvDuLieu.DataSource == null) return;
            foreach (DataGridViewColumn col in dgvDuLieu.Columns)
            {
                col.Tag = col.DataPropertyName;
                switch (col.DataPropertyName)
                {
                    case "MaHS": col.HeaderText = "Mã HS"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "HoTen": col.HeaderText = "Họ Tên"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "TenLop": col.HeaderText = "Lớp"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "SoBuoiCoMat": col.HeaderText = "Có Mặt"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "SoBuoiVang": col.HeaderText = "Vắng KP"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "SoBuoiVangCoPhep": col.HeaderText = "Vắng CP"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "TongSoBuoi": col.HeaderText = "Tổng Buổi"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "TyLeChuyenCan": col.HeaderText = "Tỷ Lệ CC (%)"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N0"; break;
                    case "Thang1": col.HeaderText = "T1"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "Thang2": col.HeaderText = "T2"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "Thang3": col.HeaderText = "T3"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "GiuaKi": col.HeaderText = "GK"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "CuoiKi": col.HeaderText = "CK"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "DiemTB": col.HeaderText = "Điểm TB Môn"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N2"; col.DefaultCellStyle.Font = new Font(dgvDuLieu.Font, FontStyle.Bold); break;
                    case "Trung bình chung": col.HeaderText = "TB Chung Kỳ"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N2"; col.DefaultCellStyle.Font = new Font(dgvDuLieu.Font, FontStyle.Bold); break;
                    case "NhanXet": col.HeaderText = "Nhận Xét"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "GhiChu": col.HeaderText = "Ghi Chú"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "GioiTinh": col.HeaderText = "Giới Tính"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "NgaySinh": col.HeaderText = "Ngày Sinh"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "dd/MM/yyyy"; break;
                    case "DanToc": col.HeaderText = "Dân Tộc"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "DiaChi": col.HeaderText = "Địa Chỉ"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "SDTPhuHuynh": col.HeaderText = "SĐT PH"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "SoHocSinh": col.HeaderText = "Sĩ Số"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "DiemTrungBinh": col.HeaderText = "Điểm TB Khối"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N2"; break;
                    case "SoNam": col.HeaderText = "Số Nam"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "SoNu": col.HeaderText = "Số Nữ"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "MaGV":
                    case "MaLop":
                    case "MaMon": col.Visible = false; break;
                    default:
                        bool isLikelySubject = col.DataPropertyName.Length > 3 || System.Text.RegularExpressions.Regex.IsMatch(col.DataPropertyName, @"\p{IsVietnamese}");
                        if (isLikelySubject && (col.ValueType == typeof(double) || col.ValueType == typeof(decimal) || col.ValueType == typeof(float)))
                        {
                            col.HeaderText = col.DataPropertyName;
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            col.DefaultCellStyle.Format = "N1";
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        else if (!col.Visible) { }
                        else
                        {
                            col.HeaderText = col.DataPropertyName;
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        }
                        break;
                }
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                if (col.ValueType == typeof(int) || col.ValueType == typeof(long) || col.ValueType == typeof(double) || col.ValueType == typeof(decimal) || col.ValueType == typeof(float) || col.HeaderText.Contains("%") || col.HeaderText.StartsWith("T") || col.HeaderText == "GK" || col.HeaderText == "CK")
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                else if (!string.IsNullOrEmpty(col.DefaultCellStyle.Format) && col.DefaultCellStyle.Format.Contains("dd/MM/yyyy"))
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                else col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (dgvDuLieu.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.csv)|*.csv",
                Title = "Lưu file Excel",
                FileName = $"BaoCao_{GetSelectedReportType()?.Replace(" ", "")}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExportHelper.ExportToExcel(dgvDuLieu, saveFileDialog.FileName);
                    MessageBox.Show($"Đã xuất báo cáo ra file:\n{saveFileDialog.FileName}", "Xuất Excel thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnXuatPDF_Click(object sender, EventArgs e)
        {
            try
            {
                bool isClassReport = false;
                bool isGradeReport = false;
                string mainHeader = "";
                string classOrGradeName = "";

                if (cboLop.Visible && cboLop.SelectedItem != null && cboLop.SelectedValue?.ToString() != "ALL" && cboLop.SelectedValue?.ToString() != "ALL_KHOI")
                {
                    isClassReport = true;
                    classOrGradeName = cboLop.Text.Trim();
                    mainHeader = $"{classOrGradeName.ToUpper()}";
                }
                else if (cboKhoi.Visible && cboKhoi.SelectedItem != null && cboKhoi.SelectedItem?.ToString() != "Tất cả các khối")
                {
                    isGradeReport = true;
                    classOrGradeName = cboKhoi.Text.Trim();
                    mainHeader = $"{classOrGradeName.ToUpper()}";
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn Lớp hoặc Khối (không chọn 'Tất cả') trước khi xuất báo cáo.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    Title = "Lưu file PDF",
                    FileName = $"BaoCao_{mainHeader.Replace(" ", "").Replace(":", "")}_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = saveFileDialog.FileName;
                    string reportType = GetSelectedReportType();
                    string documentTitle = reportType?.ToUpper() ?? "BÁO CÁO";

                    if (isGradeReport) documentTitle += $"\nKHỐI: {cboKhoi.Text}";
                    if (isClassReport) documentTitle += $"\nMÃ: {cboLop.Text}";
                    if (cboHocKy.Visible && cboHocKy.SelectedIndex != -1) documentTitle += $" - {cboHocKy.Text.ToUpper()}";
                    if (cboMonDay.Visible && cboMonDay.SelectedIndex != -1) documentTitle += $"\nMÔN: {cboMonDay.Text}";
                    if (cboThang.Visible && cboThang.SelectedIndex != -1) documentTitle += $"\nTHÁNG: {cboThang.Text}";

                    if (isGradeReport)
                    {
                        try
                        {
                            var lopList = new List<string>();
                            foreach (DataRowView rowView in cboLop.Items)
                            {
                                string maLop = rowView["MaLop"].ToString();
                                string tenLop = rowView["TenLop"].ToString();
                                if (maLop != "ALL" && maLop != "ALL_KHOI" && !string.IsNullOrWhiteSpace(tenLop))
                                {
                                    lopList.Add(tenLop.Trim().Replace("Lớp ", "").Replace("A", "/"));
                                }
                            }
                            if (lopList.Count > 0) dgvDuLieu.Tag = string.Join("; ", lopList);
                            else dgvDuLieu.Tag = "(Không tìm thấy lớp nào)";
                        }
                        catch { dgvDuLieu.Tag = ""; }
                    }
                    else dgvDuLieu.Tag = null;

                    Image secondTable = null;
                    if (reportType != "Báo cáo tháng")
                    {
                        try
                        {
                            if (flpCharts.Controls.Count > 0 && flpCharts.Controls[0] is PictureBox pic)
                            {
                                Bitmap bmp = new Bitmap(pic.Width, pic.Height);
                                pic.DrawToBitmap(bmp, new Rectangle(0, 0, pic.Width, pic.Height));
                                secondTable = bmp;
                            }
                            else if (flpCharts.Controls.Count > 0 && flpCharts.Controls[0] is DataGridView grid)
                            {
                                Bitmap bmp = new Bitmap(grid.Width, grid.Height);
                                grid.DrawToBitmap(bmp, new Rectangle(0, 0, grid.Width, grid.Height));
                                secondTable = bmp;
                            }
                        }
                        catch { }
                    }

                    ExportHelper.ExportToPDF(dgvDuLieu, fileName, documentTitle, mainHeader, secondTable);
                    MessageBox.Show("✅ Đã xuất PDF thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (File.Exists(fileName)) System.Diagnostics.Process.Start(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pnlFilters_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, pnlFilters.ClientRectangle, Color.FromArgb(220, 220, 220), ButtonBorderStyle.Solid);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (reportTypeRadioButtons != null)
                {
                    foreach (var rb in reportTypeRadioButtons) rb.CheckedChanged -= ReportType_CheckedChanged;
                }
                if (this.cboLop != null) this.cboLop.SelectedIndexChanged -= this.cboLop_SelectedIndexChanged_Handler;
                if (this.cboKhoi != null) this.cboKhoi.SelectedIndexChanged -= this.cboKhoi_SelectedIndexChanged_Handler;
                if (this.cboHocKy != null) this.cboHocKy.SelectedIndexChanged -= this.cboHocKy_SelectedIndexChanged_Handler;
                if (this.cboMonDay != null) this.cboMonDay.SelectedIndexChanged -= this.AutoLoadReport_Trigger;
                if (this.cboThang != null) this.cboThang.SelectedIndexChanged -= this.AutoLoadReport_Trigger;
                if (this.btnXuatExcel != null) this.btnXuatExcel.Click -= this.btnXuatExcel_Click;
                if (this.btnXuatPDF != null) this.btnXuatPDF.Click -= this.btnXuatPDF_Click;
                allLopHoc?.Dispose();
                allMonHoc?.Dispose();
                if (components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}