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
    // ### ADMIN CHANGE: Đổi tên lớp ###
    public partial class UC_BaoCao_Admin : UserControl
    {
        // ### ADMIN CHANGE: Removed maGV dependency ###
        // private string maGV;

        private List<RadioButton> reportTypeRadioButtons = new List<RadioButton>();
        private DataTable allLopHoc; // ### ADMIN CHANGE: Store all classes ###
        private DataTable allMonHoc; // ### ADMIN CHANGE: Store all subjects ###
        private bool isProgrammaticChange = false; // Flag to prevent cascading events

        // ### ADMIN CHANGE: Constructor doesn't need maGV ###
        public UC_BaoCao_Admin()
        {
            // ### ADMIN CHANGE: Removed maGV parameter/assignment ###
            // maGV = maGVien; 
            InitializeComponent();
            ApplyModernStyles();
            LoadInitialAdminData(); // ### ADMIN CHANGE: Renamed loading function ###
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
                // Set initial check later in LoadInitialAdminData after controls are fully set up
                // reportTypeRadioButtons[0].Checked = true;
            }

            StyleActionButton(btnXuatExcel, Color.FromArgb(16, 124, 65));
            StyleActionButton(btnXuatPDF, Color.FromArgb(217, 83, 79));

            splitContainer1.BackColor = Color.FromArgb(220, 220, 220);

            // ### ADMIN CHANGE: TabControl không còn tồn tại, không cần ẩn ###
            /*
            if (this.tabLoaiGiaoVien != null)
            {
                 this.tabLoaiGiaoVien.Visible = false; 
            }
            */
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
            // RadioButtons handled in ApplyModernStyles

            // ### ADMIN CHANGE: Removed TabControl handler ###
            // this.tabLoaiGiaoVien.SelectedIndexChanged += new System.EventHandler(this.tabLoaiGiaoVien_SelectedIndexChanged);

            this.cboLop.SelectedIndexChanged += new System.EventHandler(this.cboLop_SelectedIndexChanged_Handler);
            this.cboKhoi.SelectedIndexChanged += new System.EventHandler(this.cboKhoi_SelectedIndexChanged_Handler); // ### ADMIN CHANGE: Separate handler ###
            this.cboHocKy.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            this.cboMonDay.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            this.btnXuatExcel.Click += new System.EventHandler(this.btnXuatExcel_Click);
            this.btnXuatPDF.Click += new System.EventHandler(this.btnXuatPDF_Click);
        }

        // ### ADMIN CHANGE: Load data needed for Admin view ###
        private void LoadInitialAdminData()
        {
            isProgrammaticChange = true; // Prevent event firing during setup
            try
            {
                // Load all classes and subjects into memory
                allLopHoc = DatabaseHelper.GetAllLopHoc();
                allMonHoc = DatabaseHelper.GetAllMonHoc();

                // Populate Khoi ComboBox
                var khoiList = allLopHoc.AsEnumerable()
                                        .Select(row => row.Field<string>("Khoi"))
                                        .Distinct()
                                        .OrderBy(k => k) // Sort Khoi 1, Khoi 2...
                                        .ToList();
                khoiList.Insert(0, "Tất cả các khối"); // Add option for all grades
                cboKhoi.DataSource = khoiList;
                cboKhoi.SelectedIndex = -1; // No initial selection

                // Populate Mon Hoc ComboBox
                LoadAllSubjects(); // Load all subjects initially

                // Populate Hoc Ky ComboBox (can be static)
                cboHocKy.Items.Clear();
                cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2", "Cả năm" });
                cboHocKy.SelectedIndex = -1;

                // Set initial placeholder text
                ResetPlaceholders();

                splitContainer1.Panel2Collapsed = true;

                // Set initial checked RadioButton *after* controls are ready
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

        // ### ADMIN CHANGE: Helper to reset placeholder text ###
        private void ResetPlaceholders()
        {
            isProgrammaticChange = true;
            cboKhoi.SelectedIndex = -1;
            cboKhoi.Text = "Chọn khối...";
            cboLop.DataSource = null; // Clear items first
            cboLop.Items.Clear();
            cboLop.Text = "Chọn lớp...";
            cboHocKy.SelectedIndex = -1;
            cboHocKy.Text = "Chọn học kỳ...";
            cboMonDay.SelectedIndex = -1;
            cboMonDay.Text = "Chọn môn...";
            isProgrammaticChange = false;
        }

        private void ReportType_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked && !isProgrammaticChange) // Added !isProgrammaticChange check
            {
                // Update RadioButton styles
                foreach (var button in reportTypeRadioButtons)
                {
                    // Use Invoke if called from a different thread, though unlikely here
                    button.ForeColor = button.Checked ? Color.White : Color.Black;
                    button.BackColor = button.Checked ? Color.FromArgb(0, 120, 215) : Color.FromArgb(240, 240, 240);
                }

                UpdateControlsVisibility(); // Update filter visibility
                ClearReportData(); // Clear previous report
                ResetPlaceholders(); // Reset filter selections AFTER visibility update
            }
        }

        // ### ADMIN CHANGE: Separate handler for Khoi selection ###
        private void cboKhoi_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            if (isProgrammaticChange || cboKhoi.SelectedIndex < 0) return;

            LoadClassesForSelectedGrade(); // Reload class list based on selected grade
            // Trigger report loading only if all necessary filters are selected for the current report type
            AutoLoadReport_Trigger(sender, e);
        }

        // ### ADMIN CHANGE: Handler for Lop selection ###
        private void cboLop_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            if (isProgrammaticChange) return;
            // Trigger report loading only if all necessary filters are selected
            AutoLoadReport_Trigger(sender, e);
        }

        // ### ADMIN CHANGE: Loads ALL subjects into the ComboBox ###
        private void LoadAllSubjects()
        {
            isProgrammaticChange = true;
            try
            {
                // Create a copy to add "All" option without modifying the original cache
                DataTable dtMonDisplay = allMonHoc.Copy();

                // Add "All Subjects" option
                DataRow allRow = dtMonDisplay.NewRow();
                allRow["MaMon"] = "ALL"; // Use a special value
                allRow["TenMon"] = "Tất cả các môn";
                dtMonDisplay.Rows.InsertAt(allRow, 0);

                cboMonDay.DataSource = dtMonDisplay;
                cboMonDay.DisplayMember = "TenMon";
                cboMonDay.ValueMember = "MaMon";
                cboMonDay.SelectedIndex = -1; // Default to no selection
                cboMonDay.Text = "Chọn môn...";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách môn học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cboMonDay.DataSource = null;
                cboMonDay.Items.Clear();
                cboMonDay.Text = "Lỗi tải môn";
            }
            finally
            {
                isProgrammaticChange = false;
            }
        }

        // ### ADMIN CHANGE: Loads classes based on selected grade ###
        private void LoadClassesForSelectedGrade()
        {
            isProgrammaticChange = true;
            cboLop.DataSource = null; // Clear previous items
            cboLop.Items.Clear();

            try
            {
                string selectedKhoi = cboKhoi.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(selectedKhoi) || selectedKhoi == "Tất cả các khối")
                {
                    // If "All Grades" or no grade selected, show all classes
                    var allClassList = allLopHoc.AsEnumerable()
                                       .OrderBy(row => row.Field<string>("Khoi"))
                                       .ThenBy(row => row.Field<string>("TenLop"))
                                       .CopyToDataTable();
                    // Add "All Classes" option only when "All Grades" is selected
                    if (selectedKhoi == "Tất cả các khối")
                    {
                        DataRow allRow = allClassList.NewRow();
                        allRow["MaLop"] = "ALL"; // Special value
                        allRow["TenLop"] = "Tất cả các lớp";
                        allClassList.Rows.InsertAt(allRow, 0);
                    }
                    cboLop.DataSource = allClassList;
                }
                else
                {
                    // Filter classes by the selected grade
                    var filteredClasses = allLopHoc.AsEnumerable()
                                            .Where(row => row.Field<string>("Khoi") == selectedKhoi)
                                            .OrderBy(row => row.Field<string>("TenLop"))
                                            .CopyToDataTable();

                    // Add "All Classes in this Grade" option
                    DataRow allInGradeRow = filteredClasses.NewRow();
                    allInGradeRow["MaLop"] = "ALL_KHOI"; // Special value for all in grade
                    allInGradeRow["TenLop"] = $"Tất cả lớp ({selectedKhoi})";
                    filteredClasses.Rows.InsertAt(allInGradeRow, 0);

                    cboLop.DataSource = filteredClasses;
                }

                cboLop.DisplayMember = "TenLop";
                cboLop.ValueMember = "MaLop";
                cboLop.SelectedIndex = -1; // Default to no selection
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


        // Helper to get selected report type from RadioButtons
        private string GetSelectedReportType()
        {
            foreach (var rb in reportTypeRadioButtons)
            {
                if (rb.Checked)
                {
                    // Use Text property which holds the display name
                    return rb.Text;
                }
            }
            return null; // Should not happen if one is checked by default
        }

        // ### ADMIN CHANGE: Adjusted visibility logic for Admin ###
        private void UpdateControlsVisibility()
        {
            string reportType = GetSelectedReportType();

            // Start by assuming most are visible, then hide as needed
            lblKhoi.Visible = cboKhoi.Visible = true;
            lblLop.Visible = cboLop.Visible = true;
            lblHocKy.Visible = cboHocKy.Visible = true;
            lblMonDay.Visible = cboMonDay.Visible = true;

            // Reset HocKy items initially
            isProgrammaticChange = true; // Prevent event firing during item changes
            cboHocKy.Items.Clear();
            cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2", "Cả năm" });
            isProgrammaticChange = false;

            // Hide controls based on report type
            switch (reportType)
            {
                case "Báo cáo chuyên cần":
                    lblMonDay.Visible = cboMonDay.Visible = false; // Not needed
                    // HocKy defaults to All options
                    break;

                case "Bảng điểm học kỳ":
                    // All filters (Khoi, Lop, HocKy, Mon) are potentially needed
                    // HocKy needs to exclude "Cả năm" for detailed grades
                    isProgrammaticChange = true;
                    cboHocKy.Items.Clear();
                    cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2" });
                    isProgrammaticChange = false;
                    break;

                case "Hồ sơ học sinh":
                    lblHocKy.Visible = cboHocKy.Visible = false; // Not needed
                    lblMonDay.Visible = cboMonDay.Visible = false; // Not needed
                    break;

                case "Thống kê tổng hợp khối":
                    lblLop.Visible = cboLop.Visible = false; // Only filter by Khoi
                    lblHocKy.Visible = cboHocKy.Visible = false; // Not needed
                    lblMonDay.Visible = cboMonDay.Visible = false; // Not needed
                    break;

                default: // Hide everything if no report type selected (shouldn't happen)
                    lblKhoi.Visible = cboKhoi.Visible = false;
                    lblLop.Visible = cboLop.Visible = false;
                    lblHocKy.Visible = cboHocKy.Visible = false;
                    lblMonDay.Visible = cboMonDay.Visible = false;
                    break;
            }
            // Ensure placeholders are reset *after* visibility is set
            // ResetPlaceholders(); // Moved call to ReportType_CheckedChanged
        }


        private void AutoLoadReport_Trigger(object sender, EventArgs e)
        {
            if (isProgrammaticChange) return; // Prevent loading during setup or programmatic changes
            if (sender is ComboBox cbo && cbo.SelectedIndex < 0) return; // Don't load if placeholder selected

            string reportType = GetSelectedReportType();
            if (string.IsNullOrEmpty(reportType)) return;

            // Check if all *required* filters for the *current* report type have a valid selection
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
                    canLoad = cboKhoi.SelectedIndex != -1; // Only Khoi is needed
                    break;
            }


            if (canLoad)
            {
                LoadReportData();
            }
            else
            {
                ClearReportData(); // Clear if filters are incomplete
            }
        }

        // ### ADMIN CHANGE: Adapted LoadReportData for Admin (needs SP adjustments potentially) ###
        private void LoadReportData()
        {
            string reportType = GetSelectedReportType();
            if (string.IsNullOrEmpty(reportType)) return;

            DataTable dtReport = null;
            try
            {
                // 1. Lấy giá trị từ các bộ lọc
                string selectedKhoi = cboKhoi.SelectedItem?.ToString();
                string selectedMaLop = cboLop.SelectedValue?.ToString();
                int selectedHocKyIndex = cboHocKy.SelectedIndex; // 0=HK1, 1=HK2, 2=Cả năm
                int hocKyParam = selectedHocKyIndex + 1; // Tham số cho SP: 1, 2, 3
                string selectedMaMon = cboMonDay.SelectedValue?.ToString();

                // 2. Chuẩn hóa giá trị tham số (Xử lý "Tất cả...")
                string khoiParam = (selectedKhoi == "Tất cả các khối") ? null : selectedKhoi;
                string maLopParam = (selectedMaLop == "ALL" || selectedMaLop == "ALL_KHOI") ? null : selectedMaLop;
                string maMonParam = (selectedMaMon == "ALL") ? null : selectedMaMon;

                // 3. Gọi SP tương ứng dựa trên loại báo cáo
                switch (reportType)
                {
                    case "Báo cáo chuyên cần":
                        dtReport = DatabaseHelper.GetBaoCaoChuyenCan_Admin(khoiParam, maLopParam, hocKyParam);
                        break;

                    case "Bảng điểm học kỳ":
                        if (!string.IsNullOrEmpty(maMonParam))
                        {
                            // Nếu chọn MỘT môn cụ thể (và phải chọn lớp cụ thể)
                            if (!string.IsNullOrEmpty(maLopParam))
                            {
                                dtReport = DatabaseHelper.GetBangDiemPivot(maLopParam, hocKyParam, maMonParam);
                                CalculateAndAddAverageColumn(dtReport); // Tính TB môn
                            }
                            else
                            {
                                MessageBox.Show("Vui lòng chọn một lớp cụ thể để xem bảng điểm chi tiết theo môn.", "Yêu cầu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ClearReportData(); // Xóa dữ liệu cũ nếu có
                                return; // Không tải dữ liệu
                            }
                        }
                        else
                        {
                            // Nếu chọn "Tất cả các môn" hoặc không cần chọn môn
                            dtReport = DatabaseHelper.GetBangDiemHocKy_Admin(khoiParam, maLopParam, hocKyParam);
                            // SP này đã trả về cột "Trung bình chung", không cần tính lại ở C#
                        }
                        break;

                    case "Hồ sơ học sinh":
                        dtReport = DatabaseHelper.GetHoSoHocSinh_Admin(khoiParam, maLopParam);
                        break;

                    case "Thống kê tổng hợp khối":
                        dtReport = DatabaseHelper.GetThongKeKhoi_Admin(khoiParam); // SP này xử lý null cho toàn trường
                        break;
                }

                // 4. Hiển thị dữ liệu
                DisplayReportData(dtReport, reportType);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearReportData(); // Xóa dữ liệu nếu có lỗi
            }
        }


        private void ClearReportData()
        {
            dgvDuLieu.DataSource = null;
            flpCharts.Controls.Clear();
            splitContainer1.Panel2Collapsed = true;
        }

        private void DisplayReportData(DataTable dtReport, string reportType)
        {
            dgvDuLieu.DataSource = dtReport;
            flpCharts.Controls.Clear();

            if (dtReport != null && dtReport.Rows.Count > 0)
            {
                RenameDataGridViewColumns();
                UpdateCharts(dtReport, reportType);
                splitContainer1.Panel2Collapsed = flpCharts.Controls.Count == 0;
            }
            else
            {
                splitContainer1.Panel2Collapsed = true;
                // Optionally show a "No data" message
                // MessageBox.Show("Không có dữ liệu cho lựa chọn này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // --- Charting and Helper methods (DrawAttendancePieChart, DrawScoreDistributionChart, etc.) ---
        // --- Keep these methods as they were in the original UC_BaoCao.cs ---
        // --- They should work correctly with the data loaded by the Admin version ---

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
                    // Add cases for other charts if needed
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
            // Check if required columns exist before proceeding
            if (!dt.Columns.Contains("SoBuoiCoMat") || !dt.Columns.Contains("SoBuoiVang") || !dt.Columns.Contains("SoBuoiVangCoPhep"))
            {
                // Optionally log an error or show a message
                return;
            }

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
                double percentPresent = (double)totalPresent / totalAll;
                series.Points.Add(totalPresent);
                int pIdx = series.Points.Count - 1;
                series.Points[pIdx].Color = Color.FromArgb(75, 192, 192);
                series.Points[pIdx].LegendText = $"Có mặt ({totalPresent})";
                series.Points[pIdx].Label = percentPresent.ToString("P1");
            }
            if (totalAbsent > 0)
            {
                double percentAbsent = (double)totalAbsent / totalAll;
                series.Points.Add(totalAbsent);
                int pIdx = series.Points.Count - 1;
                series.Points[pIdx].Color = Color.FromArgb(255, 99, 132);
                series.Points[pIdx].LegendText = $"Vắng ({totalAbsent})";
                series.Points[pIdx].Label = percentAbsent.ToString("P1");
            }
            if (totalExcused > 0)
            {
                double percentExcused = (double)totalExcused / totalAll;
                series.Points.Add(totalExcused);
                int pIdx = series.Points.Count - 1;
                series.Points[pIdx].Color = Color.FromArgb(255, 205, 86);
                series.Points[pIdx].LegendText = $"Vắng có phép ({totalExcused})";
                series.Points[pIdx].Label = percentExcused.ToString("P1");
            }

            chart.Series.Add(series);
            flpCharts.Controls.Add(chart);
        }

        private void DrawScoreDistributionChart(DataTable dt)
        {
            // Determine which average column is available
            string scoreColumnName = "";
            if (dt.Columns.Contains("DiemTB")) // From Pivot SP with added column
                scoreColumnName = "DiemTB";
            else if (dt.Columns.Contains("Trung bình chung")) // From Homeroom SP
                scoreColumnName = "Trung bình chung";
            else if (dt.Columns.Contains("DiemTrungBinh")) // From Khoi SP
                scoreColumnName = "DiemTrungBinh";

            if (string.IsNullOrEmpty(scoreColumnName)) return; // No average score column found

            var scoreCounts = new Dictionary<string, int> {
                {"Yếu (<5)", 0}, {"TB (5-<7)", 0}, {"Khá (7-<9)", 0}, {"Giỏi (>=9)", 0}
            };
            int validScores = 0;

            foreach (DataRow row in dt.Rows)
            {
                // Handle potential DBNull values safely
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
                    catch (InvalidCastException) { /* Ignore rows where conversion fails */ }
                    catch (FormatException) { /* Ignore rows where conversion fails */ }
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
                // Only add categories with counts > 0 to avoid empty columns
                if (kvp.Value > 0)
                {
                    int pIdx = series.Points.AddXY(kvp.Key, kvp.Value);
                    double percentage = (double)kvp.Value / validScores;
                    series.Points[pIdx].ToolTip = $"{kvp.Key}: {kvp.Value} HS ({percentage:P1})";
                    series.Points[pIdx].Label = kvp.Value.ToString(); // Show count on top
                    series.Points[pIdx].Color = colors[pointIndex % colors.Length];
                }
                pointIndex++; // Increment index even if count is 0 to maintain color order
            }

            // Only add series if it has points
            if (series.Points.Count > 0)
            {
                series["PixelPointWidth"] = "40"; // Adjust column width
                chart.Series.Add(series);
                flpCharts.Controls.Add(chart);
            }
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
                    catch (InvalidCastException) { /* Ignore row if conversion fails */ }
                    catch (FormatException) { /* Ignore row if conversion fails */ }
                }
            }

            if (series.Points.Count > 0)
            {
                series["PixelPointWidth"] = "30";
                chart.Series.Add(series);
                flpCharts.Controls.Add(chart);
            }
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
                LabelFormat = "N1" // Format to 1 decimal place
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
                    catch (InvalidCastException) { /* Ignore row */ }
                    catch (FormatException) { /* Ignore row */ }
                }
            }

            if (series.Points.Count > 0)
            {
                series["PixelPointWidth"] = "30";
                chart.Series.Add(series);
                flpCharts.Controls.Add(chart);
            }
        }

        private void CalculateAndAddAverageColumn(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;
            // Add DiemTB column if it doesn't exist (important!)
            if (!dt.Columns.Contains("DiemTB"))
            {
                dt.Columns.Add("DiemTB", typeof(double));
            }

            // Identify score columns dynamically (handles Pivot results)
            var scoreCols = dt.Columns.Cast<DataColumn>()
                                .Where(c => c.DataType == typeof(double) || c.DataType == typeof(float) || c.DataType == typeof(decimal))
                                .Where(c => c.ColumnName != "DiemTB" && c.ColumnName != "Trung bình chung" && c.ColumnName != "DiemTrungBinh") // Exclude existing averages
                                .Select(c => c.ColumnName).ToList();

            // Try to map Pivot columns back to standard names if needed for weighting,
            // otherwise, assume equal weight (simple average). Here, we assume simple average.
            int scoreColCount = scoreCols.Count;
            if (scoreColCount == 0) return; // No score columns found to average

            foreach (DataRow row in dt.Rows)
            {
                double totalScore = 0;
                int validScoresCount = 0; // Count actual scores found

                foreach (string colName in scoreCols)
                {
                    if (row[colName] != DBNull.Value)
                    {
                        try
                        {
                            totalScore += Convert.ToDouble(row[colName]);
                            validScoresCount++;
                        }
                        catch { /* Ignore if conversion fails */ }
                    }
                }

                if (validScoresCount > 0)
                {
                    double avg = totalScore / validScoresCount;
                    row["DiemTB"] = Math.Round(avg, 2);
                }
                else
                {
                    row["DiemTB"] = DBNull.Value;
                }
            }
        }


        // ### ADMIN CHANGE: Adapted Rename Columns for Admin view ###
        private void RenameDataGridViewColumns()
        {
            if (dgvDuLieu.DataSource == null) return;
            foreach (DataGridViewColumn col in dgvDuLieu.Columns)
            {
                col.Tag = col.DataPropertyName; // Store original binding name

                switch (col.DataPropertyName)
                {
                    // Common Columns
                    case "MaHS": col.HeaderText = "Mã HS"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "HoTen": col.HeaderText = "Họ Tên"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "TenLop": col.HeaderText = "Lớp"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break; // Show Class for Admin

                    // Attendance Report
                    case "SoBuoiCoMat": col.HeaderText = "Có Mặt"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "SoBuoiVang": col.HeaderText = "Vắng KP"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break; // Không Phép
                    case "SoBuoiVangCoPhep": col.HeaderText = "Vắng CP"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break; // Có Phép
                    case "TongSoBuoi": col.HeaderText = "Tổng Buổi"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "TyLeChuyenCan": col.HeaderText = "Tỷ Lệ CC (%)"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N0"; break;

                    // Grade Report (Pivot/Detailed)
                    case "Thang1": col.HeaderText = "T1"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "Thang2": col.HeaderText = "T2"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "Thang3": col.HeaderText = "T3"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "GiuaKi": col.HeaderText = "GK"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "CuoiKi": col.HeaderText = "CK"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "DiemTB": col.HeaderText = "Điểm TB Môn"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N2"; col.DefaultCellStyle.Font = new Font(dgvDuLieu.Font, FontStyle.Bold); break;
                    case "Trung bình chung": col.HeaderText = "TB Chung Kỳ"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N2"; col.DefaultCellStyle.Font = new Font(dgvDuLieu.Font, FontStyle.Bold); break;
                    case "NhanXet": col.HeaderText = "Nhận Xét"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "GhiChu": col.HeaderText = "Ghi Chú"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;

                    // Student Profile Report
                    case "GioiTinh": col.HeaderText = "Giới Tính"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "NgaySinh": col.HeaderText = "Ngày Sinh"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "dd/MM/yyyy"; break;
                    case "DanToc": col.HeaderText = "Dân Tộc"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "DiaChi": col.HeaderText = "Địa Chỉ"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "SDTPhuHuynh": col.HeaderText = "SĐT PH"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;

                    // Grade Summary Report
                    case "SoHocSinh": col.HeaderText = "Sĩ Số"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "DiemTrungBinh": col.HeaderText = "Điểm TB Khối"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N2"; break;
                    case "SoNam": col.HeaderText = "Số Nam"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "SoNu": col.HeaderText = "Số Nữ"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;

                    // Hide internal IDs
                    case "MaGV":
                    case "MaLop": // Keep MaLop potentially visible if needed, but usually hidden
                    case "MaMon": col.Visible = false; break;

                    // Handle potential dynamic columns from PIVOT (Subject Scores)
                    default:
                        // If it looks like a subject name (e.g., has Vietnamese chars, maybe length > 3)
                        // This is a heuristic and might need adjustment based on your TenMon format
                        bool isLikelySubject = col.DataPropertyName.Length > 3 || System.Text.RegularExpressions.Regex.IsMatch(col.DataPropertyName, @"\p{IsVietnamese}");

                        if (isLikelySubject && (col.ValueType == typeof(double) || col.ValueType == typeof(decimal) || col.ValueType == typeof(float)))
                        {
                            col.HeaderText = col.DataPropertyName; // Show subject name
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            col.DefaultCellStyle.Format = "N1"; // Format as score
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        else if (!col.Visible)
                        {
                            // Keep hidden if already hidden
                        }
                        else
                        {
                            // Fallback: Show original name, adjust width
                            col.HeaderText = col.DataPropertyName;
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        }
                        break;
                }

                // Standard Alignment Rules
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                if (col.ValueType == typeof(int) || col.ValueType == typeof(long) || col.ValueType == typeof(double) || col.ValueType == typeof(decimal) || col.ValueType == typeof(float) || col.HeaderText.Contains("%") || col.HeaderText.StartsWith("T") || col.HeaderText == "GK" || col.HeaderText == "CK")
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else if (!string.IsNullOrEmpty(col.DefaultCellStyle.Format) && col.DefaultCellStyle.Format.Contains("dd/MM/yyyy"))
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Center dates
                }
                else
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
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
                FileName = $"BaoCao_{GetSelectedReportType()?.Replace(" ", "")}_{DateTime.Now:yyyyMMdd_HHmmss}.csv" // Sanitize filename
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
            if (dgvDuLieu.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Lưu file PDF",
                FileName = $"BaoCao_{GetSelectedReportType()?.Replace(" ", "")}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf" // Sanitize filename
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Build a more detailed title for PDF
                    string reportTypeTitle = GetSelectedReportType()?.ToUpper() ?? "BÁO CÁO";
                    string reportTitle = reportTypeTitle;
                    List<string> filters = new List<string>();
                    if (cboKhoi.Visible && cboKhoi.SelectedIndex != -1 && cboKhoi.SelectedItem.ToString() != "Tất cả các khối") filters.Add($"Khối: {cboKhoi.SelectedItem}");
                    if (cboLop.Visible && cboLop.SelectedIndex != -1 && cboLop.SelectedValue?.ToString() != "ALL" && cboLop.SelectedValue?.ToString() != "ALL_KHOI") filters.Add($"Lớp: {cboLop.Text}");
                    if (cboHocKy.Visible && cboHocKy.SelectedIndex != -1) filters.Add($"{cboHocKy.Text}");
                    if (cboMonDay.Visible && cboMonDay.SelectedIndex != -1 && cboMonDay.SelectedValue?.ToString() != "ALL") filters.Add($"Môn: {cboMonDay.Text}");

                    if (filters.Any())
                        reportTitle += "\n" + string.Join(" - ", filters);

                    var chartImages = flpCharts.Controls.OfType<Chart>().Select(chart =>
                    {
                        using (var ms = new MemoryStream())
                        {
                            chart.SaveImage(ms, ChartImageFormat.Png);
                            return (Image)Image.FromStream(ms).Clone();
                        }
                    }).ToArray();

                    ExportHelper.ExportToPDF(dgvDuLieu, saveFileDialog.FileName, reportTitle, chartImages);
                    MessageBox.Show($"Đã xuất báo cáo ra file:\n{saveFileDialog.FileName}", "Xuất PDF thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    foreach (var img in chartImages) img?.Dispose();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Add this method to handle the Paint event for pnlFilters
        private void pnlFilters_Paint(object sender, PaintEventArgs e)
        {
            // Draw the border for the filters panel
            ControlPaint.DrawBorder(e.Graphics, pnlFilters.ClientRectangle,
               Color.FromArgb(220, 220, 220), ButtonBorderStyle.Solid);
        }
    }
}