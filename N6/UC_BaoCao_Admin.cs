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
    /// <summary>
    /// UserControl dành cho Admin (Ban Giám Hiệu) để xem các báo cáo tổng hợp
    /// toàn trường, theo khối, hoặc theo lớp.
    /// </summary>
    public partial class UC_BaoCao_Admin : UserControl
    {
        #region Fields (Biến thành viên)

        private List<RadioButton> reportTypeRadioButtons = new List<RadioButton>();
        private DataTable allLopHoc;
        private DataTable allMonHoc;
        private bool isProgrammaticChange = false; // Flag to prevent cascading events

        #endregion

        #region Constructor & Load

        /// <summary>
        /// Khởi tạo UserControl báo cáo dành cho Admin.
        /// </summary>
        public UC_BaoCao_Admin()
        {
            InitializeComponent();
            ApplyModernStyles();
            LoadInitialAdminData();
            SetupEventHandlers();
            UpdateControlsVisibility();
        }

        /// <summary>
        /// Áp dụng style hiện đại cho các control (DataGridView, Buttons, etc.).
        /// </summary>
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
                // Set initial check later in LoadInitialAdminData
            }

            StyleActionButton(btnXuatExcel, Color.FromArgb(16, 124, 65));
            StyleActionButton(btnXuatPDF, Color.FromArgb(217, 83, 79));

            splitContainer1.BackColor = Color.FromArgb(220, 220, 220);
        }

        /// <summary>
        /// Định dạng style cho các nút hành động (Excel, PDF).
        /// </summary>
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


        /// <summary>
        /// Áp dụng style hiện đại cho DataGridView.
        /// </summary>
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

        /// <summary>
        /// Gán các trình xử lý sự kiện cho control.
        /// </summary>
        private void SetupEventHandlers()
        {
            this.cboLop.SelectedIndexChanged += new System.EventHandler(this.cboLop_SelectedIndexChanged_Handler);
            this.cboKhoi.SelectedIndexChanged += new System.EventHandler(this.cboKhoi_SelectedIndexChanged_Handler);
            this.cboHocKy.SelectedIndexChanged += new System.EventHandler(this.cboHocKy_SelectedIndexChanged_Handler);
            this.cboMonDay.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            this.cboThang.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            this.btnXuatExcel.Click += new System.EventHandler(this.btnXuatExcel_Click);
            this.btnXuatPDF.Click += new System.EventHandler(this.btnXuatPDF_Click);
        }

        /// <summary>
        /// Tải dữ liệu ban đầu cho Admin (danh sách Khối, Môn học).
        /// </summary>
        private void LoadInitialAdminData()
        {
            isProgrammaticChange = true; // Prevent event firing during setup
            try
            {
                // Load all classes and subjects into memory
                allLopHoc = DatabaseHelper.GetAllClasses();
                allMonHoc = DatabaseHelper.GetAllSubjects();

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

                ToggleNoDataMessage(true);

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

        /// <summary>
        /// Đặt lại văn bản placeholder cho các ComboBox.
        /// </summary>
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

            cboThang.DataSource = null;
            cboThang.Items.Clear();
            cboThang.Text = "Chọn tháng...";

            isProgrammaticChange = false;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Xử lý khi thay đổi loại báo cáo (RadioButton).
        /// </summary>
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

        /// <summary>
        /// Xử lý khi thay đổi lựa chọn Khối.
        /// </summary>
        private void cboKhoi_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            if (isProgrammaticChange || cboKhoi.SelectedIndex < 0) return;

            LoadClassesForSelectedGrade(); // Reload class list based on selected grade

            // Nếu chọn Khối, cboLop sẽ được tải lại. 
            // Nếu báo cáo tháng đang được chọn, chúng ta cũng cần cập nhật cboThang
            string reportType = GetSelectedReportType();
            if (reportType == "Báo cáo tháng")
            {
                // Tạm thời xóa cboThang để người dùng chọn lại
                cboThang.DataSource = null;
                cboThang.Items.Clear();
                cboThang.Text = "Chọn học kỳ/lớp...";
            }

            // Trigger report loading only if all necessary filters are selected for the current report type
            AutoLoadReport_Trigger(sender, e);
        }

        /// <summary>
        /// Xử lý khi thay đổi lựa chọn Lớp.
        /// </summary>
        private void cboLop_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            if (isProgrammaticChange) return;

            string reportType = GetSelectedReportType();
            if (reportType == "Báo cáo tháng")
            {
                // Khi chọn lớp, tải lại danh sách tháng
                LoadThangForSelectedHocKy();
            }

            // Trigger report loading only if all necessary filters are selected
            AutoLoadReport_Trigger(sender, e);
        }

        /// <summary>
        /// Xử lý khi thay đổi lựa chọn Học Kỳ.
        /// </summary>
        private void cboHocKy_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            if (isProgrammaticChange) return;

            string reportType = GetSelectedReportType();
            if (reportType == "Báo cáo tháng")
            {
                LoadThangForSelectedHocKy();
            }

            // Kích hoạt tải báo cáo tự động
            AutoLoadReport_Trigger(sender, e);
        }

        #endregion

        #region Data Loading (Tải dữ liệu)

        /// <summary>
        /// Tải danh sách tháng (cột điểm) dựa trên Lớp/Khối và Học Kỳ.
        /// </summary>
        private void LoadThangForSelectedHocKy()
        {
            if (isProgrammaticChange) return;
            isProgrammaticChange = true;

            string selectedMaLopValue = cboLop.SelectedValue?.ToString();
            string selectedKhoiValue = cboKhoi.SelectedItem?.ToString();
            int hocKyIndex = cboHocKy.SelectedIndex; // 0 for HK1, 1 for HK2

            this.cboThang.DataSource = null;

            if (hocKyIndex == -1 || (selectedMaLopValue == null && selectedKhoiValue == null))
            {
                cboThang.Text = "Chọn lớp/học kỳ";
                isProgrammaticChange = false;
                return;
            }

            // Báo cáo tháng không có "Cả năm"
            if (hocKyIndex > 1)
            {
                cboThang.Text = "Chọn HK1 hoặc 2";
                isProgrammaticChange = false;
                return;
            }

            int hocKy = hocKyIndex + 1; // 1 for HK1, 2 for HK2

            // Ưu tiên Lớp cụ thể
            string maLopDeTim = null;
            if (!string.IsNullOrEmpty(selectedMaLopValue) && selectedMaLopValue != "ALL" && selectedMaLopValue != "ALL_KHOI")
            {
                maLopDeTim = selectedMaLopValue;
            }
            // Nếu không chọn lớp cụ thể, nhưng chọn Khối
            else if (!string.IsNullOrEmpty(selectedKhoiValue) && selectedKhoiValue != "Tất cả các khối")
            {
                // Lấy mã lớp đầu tiên thuộc khối đó để tìm danh sách tháng
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


        /// <summary>
        /// Tải TẤT CẢ các môn học vào ComboBox (thêm "Tất cả").
        /// </summary>
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

        /// <summary>
        /// Tải danh sách Lớp vào ComboBox dựa trên Khối đã chọn.
        /// </summary>
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
                    allInGradeRow["TenLop"] = $" ({selectedKhoi})";
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

        #endregion

        #region Report Logic (Logic Báo cáo)

        /// <summary>
        /// Lấy tên (Text) của RadioButton đang được chọn.
        /// </summary>
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

        /// <summary>
        /// Cập nhật hiển thị (ẩn/hiện) các bộ lọc dựa trên loại báo cáo.
        /// </summary>
        private void UpdateControlsVisibility()
        {
            string reportType = GetSelectedReportType();

            // Start by assuming most are visible, then hide as needed
            lblKhoi.Visible = cboKhoi.Visible = true;
            lblLop.Visible = cboLop.Visible = true;
            lblHocKy.Visible = cboHocKy.Visible = true;
            lblMonDay.Visible = cboMonDay.Visible = true;
            lblThang.Visible = cboThang.Visible = false;

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
                    cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2" }); // Bảng điểm chi tiết không có "Cả năm"
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

                case "Báo cáo tháng":
                    lblThang.Visible = cboThang.Visible = true; // Hiện cboThang
                    isProgrammaticChange = true;
                    cboHocKy.Items.Clear();
                    cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2" }); // Báo cáo tháng không có "Cả năm"
                    isProgrammaticChange = false;
                    LoadThangForSelectedHocKy(); // Tải danh sách tháng
                    break;

                default: // Hide everything if no report type selected (shouldn't happen)
                    lblKhoi.Visible = cboKhoi.Visible = false;
                    lblLop.Visible = cboLop.Visible = false;
                    lblHocKy.Visible = cboHocKy.Visible = false;
                    lblMonDay.Visible = cboMonDay.Visible = false;
                    break;
            }
        }


        /// <summary>
        /// Kích hoạt tải báo cáo tự động khi một ComboBox thay đổi.
        /// </summary>
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
                case "Báo cáo tháng":
                    canLoad = cboKhoi.SelectedIndex != -1 &&
                              cboLop.SelectedIndex != -1 &&
                              cboHocKy.SelectedIndex != -1 &&
                              cboMonDay.SelectedIndex != -1 &&
                              cboThang.SelectedIndex != -1;
                    break;
            }


            if (canLoad)
            {
                LoadReportData();
            }

        }

        /// <summary>
        /// Tải dữ liệu báo cáo chính từ CSDL dựa trên các bộ lọc.
        /// </summary>
        private void LoadReportData()
        {
            string reportType = GetSelectedReportType();
            if (string.IsNullOrEmpty(reportType)) return;

            DataTable dtReport = null;
            try
            {
                // 1. Lấy giá trị từ các bộ lọc
                string selectedKhoi = cboKhoi.SelectedItem?.ToString();
                string selectedMaLopValue = cboLop.SelectedValue?.ToString(); // vd: "1A1", "ALL", "ALL_KHOI"
                int selectedHocKyIndex = cboHocKy.SelectedIndex; // 0=HK1, 1=HK2, 2=Cả năm
                int hocKyParam = selectedHocKyIndex + 1; // Tham số cho SP: 1, 2, 3
                string selectedMaMon = cboMonDay.SelectedValue?.ToString();
                string selectedLoaiDiem = cboThang.SelectedValue?.ToString();

                // 2. Chuẩn hóa giá trị tham số
                string khoiParam = (selectedKhoi == "Tất cả các khối") ? null : selectedKhoi;
                string maLopParam_Admin = (selectedMaLopValue == "ALL" || selectedMaLopValue == "ALL_KHOI") ? null : selectedMaLopValue;
                string maMonParam_Pivot = (selectedMaMon == "ALL") ? null : selectedMaMon;

                // 3. Biến kiểm tra xem có phải chọn LỚP CỤ THỂ không
                bool isSpecificClass = !string.IsNullOrEmpty(selectedMaLopValue)
                                       && selectedMaLopValue != "ALL"
                                       && selectedMaLopValue != "ALL_KHOI";

                // 4. Gọi SP tương ứng dựa trên loại báo cáo VÀ lựa chọn Lớp/Khối
                switch (reportType)
                {
                    case "Báo cáo chuyên cần":
                        if (isSpecificClass)
                        {
                            // Dùng hàm cũ (cho GV) khi chọn lớp cụ thể
                            dtReport = DatabaseHelper.GetAttendanceReport(maLopParam_Admin, hocKyParam);
                        }
                        else
                        {
                            // Dùng hàm Admin khi chọn "Tất cả các lớp" / "Tất cả các khối"
                            dtReport = DatabaseHelper.GetAttendanceReport_Admin(khoiParam, maLopParam_Admin, hocKyParam);
                        }
                        break;

                    case "Bảng điểm học kỳ":
                        if (!string.IsNullOrEmpty(maMonParam_Pivot))
                        {
                            // --- Bảng điểm PIVOT (chi tiết 1 môn) ---
                            // Yêu cầu phải chọn lớp cụ thể
                            if (isSpecificClass)
                            {
                                dtReport = DatabaseHelper.GetScoreboardPivot(maLopParam_Admin, hocKyParam, maMonParam_Pivot);
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
                            // --- Bảng điểm TỔNG HỢP (nhiều môn) ---
                            if (isSpecificClass)
                            {
                                // Dùng hàm cũ (cho GV) khi chọn lớp cụ thể
                                dtReport = DatabaseHelper.GetSemesterScoreboard(maLopParam_Admin, hocKyParam);
                            }
                            else
                            {
                                // Dùng hàm Admin khi chọn "Tất cả các lớp" / "Tất cả các khối"
                                dtReport = DatabaseHelper.GetSemesterScoreboard_Admin(khoiParam, maLopParam_Admin, hocKyParam);
                            }
                        }
                        break;

                    case "Hồ sơ học sinh":
                        if (isSpecificClass)
                        {
                            // Dùng hàm cũ (cho GV) khi chọn lớp cụ thể
                            dtReport = DatabaseHelper.GetStudentRecords(maLopParam_Admin);
                        }
                        else
                        {
                            // Dùng hàm Admin khi chọn "Tất cả các lớp" / "Tất cả các khối"
                            dtReport = DatabaseHelper.GetStudentRecords_Admin(khoiParam, maLopParam_Admin);
                        }
                        break;

                    case "Thống kê tổng hợp khối":
                        // Trường hợp này luôn ẩn cboLop, nên isSpecificClass luôn là false
                        // Luôn dùng hàm Admin, không cần thay đổi
                        dtReport = DatabaseHelper.GetGradeStatistics_Admin(khoiParam); // SP này xử lý null cho toàn trường
                        break;

                    case "Báo cáo tháng":
                        if (isSpecificClass)
                        {
                            // Xem theo Lớp cụ thể -> Dùng SP của GV
                            dtReport = DatabaseHelper.GetMonthlyReport_Statistics(maLopParam_Admin, selectedMaMon, selectedLoaiDiem);
                        }
                        else
                        {
                            // Xem theo Khối -> Dùng SP mới của Admin
                            dtReport = DatabaseHelper.GetMonthlyReport_Statistics_Admin(khoiParam, selectedMaMon, selectedLoaiDiem);
                        }
                        break;
                }

                // 5. Hiển thị dữ liệu (như cũ)
                DisplayReportData(dtReport, reportType);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearReportData(); // Xóa dữ liệu nếu có lỗi
            }
        }


        /// <summary>
        /// Xóa dữ liệu báo cáo (Grid, Chart) khỏi giao diện.
        /// </summary>
        private void ClearReportData()
        {
            dgvDuLieu.DataSource = null;
            flpCharts.Controls.Clear();
            splitContainer1.Panel2Collapsed = true;
            ToggleNoDataMessage(true);
        }

        /// <summary>
        /// Hiển thị dữ liệu báo cáo lên Grid và Chart.
        /// </summary>
        private void DisplayReportData(DataTable dtReport, string reportType)
        {
            // TRƯỜNG HỢP 1: BÁO CÁO THÁNG (Cấu trúc bảng đặc biệt)
            if (reportType == "Báo cáo tháng")
            {
                dgvDuLieu.DataSource = null; // Xóa lưới chính
                flpCharts.Controls.Clear();  // Xóa panel biểu đồ

                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    // Ẩn tiêu đề mặc định của DataGridView vì báo cáo tháng tự vẽ header
                    dgvDuLieu.ColumnHeadersVisible = false;

                    // Gọi hàm hiển thị gộp (Logic hiển thị và ẩn LabelNoData nằm trong hàm này)
                    DisplayCombinedMonthlyReport(dtReport);

                    splitContainer1.Panel2Collapsed = true; // Báo cáo tháng không có biểu đồ dưới
                }
                else
                {
                    // Không có dữ liệu
                    dgvDuLieu.ColumnHeadersVisible = true;
                    splitContainer1.Panel2Collapsed = true;

                    // Hiện thông báo "Chưa có báo cáo"
                    ToggleNoDataMessage(true);
                }
            }
            // TRƯỜNG HỢP 2: CÁC BÁO CÁO KHÁC (Cấu trúc bảng chuẩn)
            else
            {
                // Luôn hiện lại tiêu đề cột
                dgvDuLieu.ColumnHeadersVisible = true;
                dgvDuLieu.DataSource = dtReport;
                flpCharts.Controls.Clear();

                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    // Có dữ liệu: Ẩn thông báo "Chưa có báo cáo", hiện Grid
                    ToggleNoDataMessage(false);

                    RenameDataGridViewColumns();
                    UpdateCharts(dtReport, reportType);

                    // Chỉ hiện panel biểu đồ nếu có biểu đồ được vẽ
                    splitContainer1.Panel2Collapsed = flpCharts.Controls.Count == 0;
                }
                else
                {
                    // Không có dữ liệu
                    splitContainer1.Panel2Collapsed = true;

                    // Hiện thông báo "Chưa có báo cáo"
                    ToggleNoDataMessage(true);
                }
            }
        }


        /// <summary>
        /// Hiển thị báo cáo tháng (GỘP CHUNG) lên DataGridView.
        /// </summary>
        private void DisplayCombinedMonthlyReport(DataTable dtReport)
        {
            // 1. Dọn dẹp
            flpCharts.Controls.Clear();
            splitContainer1.Panel2Collapsed = true; // Không dùng panel dưới nữa

            // Kiểm tra đây là báo cáo Khối (Admin) hay Lớp
            bool isAdminReport = dtReport.Columns.Contains("TyLe");

            // 2. Lấy dữ liệu nguồn
            var dataMapDiem = dtReport.Select("LoaiThongKe = 'Diem'")
                                      .ToDictionary(r => r["PhanLoai"].ToString(), r => r);
            var dataMapXepLoai = dtReport.Select("LoaiThongKe = 'XepLoai'")
                                         .ToDictionary(r => r["PhanLoai"].ToString(), r => r);

            // 3. Tạo DataTable KẾT HỢP
            DataTable dtCombined = new DataTable();
            dtCombined.Columns.Add("PhanLoai", typeof(string));
            dtCombined.Columns.Add("Col_TS", typeof(string));
            dtCombined.Columns.Add("Col_Nu", typeof(string));
            dtCombined.Columns.Add("Col_DanToc_Percent", typeof(string)); // Cột này dùng chung
            dtCombined.Columns.Add("Col_NDT", typeof(string));
            dtCombined.Columns.Add("IsHeader", typeof(int)); // Cột cờ để PDF nhận diện

            // 4. Thêm hàng tiêu đề ĐIỂM
            dtCombined.Rows.Add("ĐIỂM", "TS", "Nữ", "Dân tộc", "NDT", 1);

            // 5. Thêm các hàng dữ liệu ĐIỂM (Dùng chung cho cả Lớp và Khối)
            string[] scoreOrder = { "10", "9", "8", "7", "6", "5", "Dưới 5" };
            foreach (string key in scoreOrder)
            {
                string ts = "0", nu = "0", dtoc = "0", ndt = "0";
                if (dataMapDiem.ContainsKey(key))
                {
                    ts = dataMapDiem[key]["TS"].ToString();
                    nu = dataMapDiem[key]["Nu"].ToString();
                    dtoc = dataMapDiem[key]["DanToc"].ToString();
                    ndt = dataMapDiem[key]["NDT"].ToString();
                }
                dtCombined.Rows.Add(key, ts, nu, dtoc, ndt, 0);
            }

            // Thêm hàng TỔNG CỘNG cho báo cáo LỚP
            if (!isAdminReport)
            {
                // Tính tổng từ dataMapDiem
                int totalTS = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["TS"]));
                int totalNu = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["Nu"]));
                int totalDanToc = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["DanToc"]));
                int totalNDT = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["NDT"]));
                dtCombined.Rows.Add("Tổng", totalTS.ToString(), totalNu.ToString(), totalDanToc.ToString(), totalNDT.ToString(), 0);
            }

            // 6. Thêm hàng tiêu đề XẾP LOẠI
            if (isAdminReport)
            {
                dtCombined.Rows.Add("XẾP LOẠI", "TS", "", "%", "", 1);
            }
            else
            {
                dtCombined.Rows.Add("XẾP LOẠI", "TS", "Nữ", "Dân tộc", "NDT", 1);
            }

            // 7. Thêm các hàng dữ liệu XẾP LOẠI
            string[] rankOrder = { "T", "H", "C" };
            foreach (string key in rankOrder)
            {
                string ts = "0";
                string col2 = ""; // Nữ (Lớp) hoặc "" (Khối)
                string col3 = ""; // Dân tộc (Lớp) hoặc % (Khối)
                string col4 = ""; // NDT (Lớp) hoặc "" (Khối)

                if (dataMapXepLoai.ContainsKey(key))
                {
                    ts = dataMapXepLoai[key]["TS"].ToString();
                    if (isAdminReport)
                    {
                        // Báo cáo Khối (Admin) lấy TyLe
                        col3 = Convert.ToDouble(dataMapXepLoai[key]["TyLe"]).ToString("N1") + "%";
                    }
                    else
                    {
                        // Báo cáo Lớp lấy chi tiết
                        col2 = dataMapXepLoai[key]["Nu"].ToString();
                        col3 = dataMapXepLoai[key]["DanToc"].ToString();
                        col4 = dataMapXepLoai[key]["NDT"].ToString();
                    }
                }
                dtCombined.Rows.Add(key, ts, col2, col3, col4, 0);
            }

            // 8. Hiển thị
            dgvDuLieu.DataSource = dtCombined;
            ToggleNoDataMessage(false);
            // Truyền cờ isAdminReport vào hàm Format
            FormatCombinedMonthlyReportGrid(dgvDuLieu, isAdminReport);
        }

        /// <summary>
        /// Định dạng các cột cho lưới Báo cáo tháng (GỘP CHUNG).
        /// </summary>
        private void FormatCombinedMonthlyReportGrid(DataGridView dgv, bool isAdminReport)
        {
            if (dgv.DataSource == null) return;

            // 1. Định dạng cột
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                switch (col.DataPropertyName)
                {
                    case "PhanLoai":
                        col.HeaderText = ""; // Cột đầu tiên không có header
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        break;
                    case "Col_TS":
                        col.HeaderText = "TS";
                        break;
                    case "Col_Nu":
                        col.HeaderText = "Nữ";
                        break;
                    case "Col_DanToc_Percent":
                        // Header vẫn là "Dân tộc" cho cả 2
                        col.HeaderText = "Dân tộc";
                        break;
                    case "Col_NDT":
                        col.HeaderText = "NDT";
                        break;
                    case "IsHeader":
                        col.Visible = false; // Ẩn cột cờ
                        break;
                }
            }

            // 2. Định dạng hàng (Hàng tiêu đề và hàng dữ liệu)
            foreach (DataGridViewRow row in dgv.Rows)
            {
                bool isHeader = Convert.ToInt32(row.Cells["IsHeader"].Value) == 1;
                string phanLoai = row.Cells["PhanLoai"].Value?.ToString();

                if (isHeader)
                {
                    // Hàng tiêu đề (ĐIỂM, XẾP LOẠI)
                    row.DefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
                    row.DefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    // Hàng dữ liệu (10, 9, T, H...)
                    row.Cells["PhanLoai"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Tô đậm hàng "Tổng" cho báo cáo lớp
                if (!isAdminReport && phanLoai == "Tổng")
                {
                    row.DefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                }

                // Ẩn/Thay đổi các ô không cần thiết cho báo cáo Khối (Admin)
                if (isAdminReport && phanLoai == "XẾP LOẠI")
                {
                    // Hàng tiêu đề XẾP LOẠI (của Khối)
                    row.Cells["Col_Nu"].Value = "";
                    row.Cells["Col_DanToc_Percent"].Value = "%";
                    row.Cells["Col_NDT"].Value = "";
                }
                else if (isAdminReport && (phanLoai == "T" || phanLoai == "H" || phanLoai == "C"))
                {
                    // Hàng data XẾP LOẠI (của Khối)
                    row.Cells["Col_Nu"].Value = "";
                    row.Cells["Col_NDT"].Value = "";
                }
            }
        }

        /// <summary>
        /// Hàm hỗ trợ ẩn/hiện thông báo "Chưa có dữ liệu"
        /// </summary>
        private void ToggleNoDataMessage(bool showMessage)
        {
            if (lblNoData != null)
            {
                lblNoData.Visible = showMessage;
                if (showMessage)
                {
                    lblNoData.BringToFront();
                    dgvDuLieu.Visible = false; // Ẩn lưới đi cho sạch
                }
                else
                {
                    dgvDuLieu.Visible = true; // Hiện lưới lại
                }
            }
        }

        #endregion

        #region Charting (Vẽ Biểu đồ)

        /// <summary>
        /// Cập nhật (vẽ) các biểu đồ dựa trên loại báo cáo.
        /// </summary>
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

        /// <summary>
        /// Tạo một đối tượng Chart cơ bản với style chung.
        /// </summary>
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

        /// <summary>
        /// Vẽ biểu đồ tròn chuyên cần.
        /// </summary>
        private void DrawAttendancePieChart(DataTable dt)
        {
            // Check if required columns exist before proceeding
            if (!dt.Columns.Contains("SoBuoiCoMat") || !dt.Columns.Contains("SoBuoiVang") || !dt.Columns.Contains("SoBuoiVangCoPhep"))
            {
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

        /// <summary>
        /// Vẽ biểu đồ cột phân phối điểm.
        /// </summary>
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


        /// <summary>
        /// Vẽ biểu đồ cột sĩ số học sinh.
        /// </summary>
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

        /// <summary>
        /// Vẽ biểu đồ cột điểm trung bình.
        /// </summary>
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

        /// <summary>
        /// Tính toán và thêm cột "DiemTB" vào DataTable (cho Bảng điểm).
        /// </summary>
        private void CalculateAndAddAverageColumn(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;
            // Add DiemTB column if it doesn't exist
            if (!dt.Columns.Contains("DiemTB"))
            {
                dt.Columns.Add("DiemTB", typeof(double));
            }

            // Identify score columns dynamically
            var scoreCols = dt.Columns.Cast<DataColumn>()
                                .Where(c => c.DataType == typeof(double) || c.DataType == typeof(float) || c.DataType == typeof(decimal))
                                .Where(c => c.ColumnName != "DiemTB" && c.ColumnName != "Trung bình chung" && c.ColumnName != "DiemTrungBinh") // Exclude existing averages
                                .Select(c => c.ColumnName).ToList();

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

        #endregion

        #region Formatting & Export (Định dạng & Xuất file)

        /// <summary>
        /// Đổi tên các tiêu đề cột (HeaderText) của DataGridView.
        /// </summary>
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
                    case "MaLop":
                    case "MaMon": col.Visible = false; break;

                    // Handle potential dynamic columns from PIVOT (Subject Scores)
                    default:
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


        /// <summary>
        /// Xử lý sự kiện click nút "Xuất Excel".
        /// </summary>
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

        /// <summary>
        /// Xử lý sự kiện click nút "Xuất PDF".
        /// </summary>
        private void btnXuatPDF_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy thông tin header TRƯỚC để làm tên file mặc định
                bool isClassReport = false;
                bool isGradeReport = false;
                string mainHeader = "";
                string classOrGradeName = "";

                // Nếu combobox lớp được chọn => Báo cáo lớp
                if (cboLop.Visible && cboLop.SelectedItem != null && cboLop.SelectedValue?.ToString() != "ALL" && cboLop.SelectedValue?.ToString() != "ALL_KHOI")
                {
                    isClassReport = true;
                    classOrGradeName = cboLop.Text.Trim();
                    mainHeader = $"{classOrGradeName.ToUpper()}";
                }
                // Nếu combobox khối được chọn => Báo cáo khối
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

                // 2. Mở SaveFileDialog để người dùng chọn vị trí
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    Title = "Lưu file PDF",
                    FileName = $"BaoCao_{mainHeader.Replace(" ", "").Replace(":", "")}_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
                };

                // 3. Nếu người dùng chọn "OK"
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = saveFileDialog.FileName;

                    // Tạo tiêu đề tài liệu chi tiết (cho phần đầu trang)
                    string reportType = GetSelectedReportType();
                    string documentTitle = reportType?.ToUpper() ?? "BÁO CÁO"; // VD: "BÁO CÁO THÁNG"

                    if (isGradeReport)
                    {
                        documentTitle += $"\nKHỐI: {cboKhoi.Text}"; // VD: "... \nKHỐI: Khối 5"
                    }
                    if (isClassReport)
                    {
                        documentTitle += $"\nLỚP: {cboLop.Text}"; // VD: "... \nLỚP: 5A1"
                    }
                    if (cboHocKy.Visible && cboHocKy.SelectedIndex != -1) documentTitle += $" - {cboHocKy.Text.ToUpper()}";
                    if (cboMonDay.Visible && cboMonDay.SelectedIndex != -1) documentTitle += $"\nMÔN: {cboMonDay.Text}";
                    if (cboThang.Visible && cboThang.SelectedIndex != -1) documentTitle += $"\nTHÁNG: {cboThang.Text}";

                    // 4. Tự động lấy danh sách lớp (nếu là báo cáo khối)
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

                            if (lopList.Count > 0)
                                dgvDuLieu.Tag = string.Join("; ", lopList);
                            else
                                dgvDuLieu.Tag = "(Không tìm thấy lớp nào)";
                        }
                        catch { dgvDuLieu.Tag = ""; }
                    }
                    else
                    {
                        dgvDuLieu.Tag = null;
                    }

                    // 5. Lấy ảnh của bảng xếp loại (nếu có)
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

                    // 6. Xuất PDF
                    ExportHelper.ExportToPDF(dgvDuLieu, fileName, documentTitle, mainHeader, secondTable);

                    MessageBox.Show("✅ Đã xuất PDF thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 7. Mở file đã lưu
                    if (File.Exists(fileName))
                        System.Diagnostics.Process.Start(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Xử lý sự kiện Paint (vẽ viền) cho panel bộ lọc.
        /// </summary>
        private void pnlFilters_Paint(object sender, PaintEventArgs e)
        {
            // Draw the border for the filters panel
            ControlPaint.DrawBorder(e.Graphics, pnlFilters.ClientRectangle,
               Color.FromArgb(220, 220, 220), ButtonBorderStyle.Solid);
        }

        #endregion

        #region Dispose

        /// <summary> 
        /// Dọn dẹp các tài nguyên đang sử dụng.
        /// </summary>
        /// <param name="disposing">true nếu tài nguyên được quản lý nên được giải phóng; ngược lại là false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 1. Gỡ bỏ sự kiện RadioButton
                if (reportTypeRadioButtons != null)
                {
                    foreach (var rb in reportTypeRadioButtons)
                    {
                        rb.CheckedChanged -= ReportType_CheckedChanged;
                    }
                }

                // 2. Gỡ bỏ sự kiện ComboBox, Button
                if (this.cboLop != null) this.cboLop.SelectedIndexChanged -= this.cboLop_SelectedIndexChanged_Handler;
                if (this.cboKhoi != null) this.cboKhoi.SelectedIndexChanged -= this.cboKhoi_SelectedIndexChanged_Handler;
                if (this.cboHocKy != null) this.cboHocKy.SelectedIndexChanged -= this.cboHocKy_SelectedIndexChanged_Handler;
                if (this.cboMonDay != null) this.cboMonDay.SelectedIndexChanged -= this.AutoLoadReport_Trigger;
                if (this.cboThang != null) this.cboThang.SelectedIndexChanged -= this.AutoLoadReport_Trigger;
                if (this.btnXuatExcel != null) this.btnXuatExcel.Click -= this.btnXuatExcel_Click;
                if (this.btnXuatPDF != null) this.btnXuatPDF.Click -= this.btnXuatPDF_Click;

                // 3. Giải phóng tài nguyên IDisposable (DataTables)
                allLopHoc?.Dispose();
                allMonHoc?.Dispose();

                // 4. Giải phóng 'components' (từ Designer)
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