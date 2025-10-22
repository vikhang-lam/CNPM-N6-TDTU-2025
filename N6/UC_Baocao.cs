using System;
using System.Collections.Generic; // Added for Dictionary
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace N6
{
    public partial class UC_BaoCao : UserControl
    {
        private string maGV;
        // Keep references to RadioButtons for easy access
        private List<RadioButton> reportTypeRadioButtons = new List<RadioButton>();

        public UC_BaoCao(string maGVien)
        {
            maGV = maGVien;
            InitializeComponent(); // Now calls the Designer-generated code
            ApplyModernStyles(); // Apply styles after controls are created
            LoadDuLieu();
            SetupEventHandlers();
            UpdateControlsVisibility(); // Initial setup
        }

        // Apply styles not easily set in Designer or common styles
        private void ApplyModernStyles()
        {
            StyleDataGridViewModern(this.dgvDuLieu);

            // Style RadioButtons as Tabs/Buttons
            foreach (Control ctrl in pnlReportTypeSelector.Controls)
            {
                if (ctrl is RadioButton rb)
                {
                    rb.Appearance = Appearance.Button;
                    rb.FlatStyle = FlatStyle.Flat;
                    rb.FlatAppearance.BorderSize = 0;
                    rb.FlatAppearance.CheckedBackColor = Color.FromArgb(0, 120, 215); // Blue when checked
                    rb.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 235, 250); // Light blue on hover
                    rb.BackColor = Color.FromArgb(240, 240, 240); // Light gray default
                    rb.ForeColor = Color.Black; // Black text default
                    rb.TextAlign = ContentAlignment.MiddleCenter;
                    rb.Padding = new Padding(10, 0, 10, 0);
                    rb.MinimumSize = new Size(150, 35); // Ensure minimum width
                    rb.AutoSize = true; // Let width adjust slightly based on text
                    reportTypeRadioButtons.Add(rb); // Add to list
                    rb.CheckedChanged += ReportType_CheckedChanged; // Add handler
                }
            }
            // Initial check (optional, can be done in LoadDuLieu)
            if (reportTypeRadioButtons.Count > 0)
            {
                reportTypeRadioButtons[0].Checked = true; // Check the first one by default
            }


            // Style Action Buttons
            StyleActionButton(btnXuatExcel, Color.FromArgb(16, 124, 65)); // Green
            // TODO: Add Excel Icon: btnXuatExcel.Image = Properties.Resources.excel_icon;
            StyleActionButton(btnXuatPDF, Color.FromArgb(217, 83, 79)); // Red
                                                                        // TODO: Add PDF Icon: btnXuatPDF.Image = Properties.Resources.pdf_icon;

            // Ensure Splitter has a subtle color
            splitContainer1.BackColor = Color.FromArgb(220, 220, 220); // Light gray splitter
        }

        private void StyleActionButton(Button btn, Color backColor)
        {
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI Semibold", 9.5F);
            btn.Size = new Size(130, 38); // Slightly wider for icon + text
            btn.TextAlign = ContentAlignment.MiddleCenter;
            // Properties for adding icons (uncomment and assign Image when you have them)
            // btn.ImageAlign = ContentAlignment.MiddleLeft;
            // btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            // btn.Padding = new Padding(10, 0, 10, 0); // Add padding if using icons
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
            // ReportType RadioButtons already handled in ApplyModernStyles
            // Remove old ComboBox handler:
            // this.cboLoaiBaoCao.SelectedIndexChanged += new System.EventHandler(this.cboLoaiBaoCao_SelectedIndexChanged);

            this.tabLoaiGiaoVien.SelectedIndexChanged += new System.EventHandler(this.tabLoaiGiaoVien_SelectedIndexChanged);
            this.cboLop.SelectedIndexChanged += new System.EventHandler(this.cboLop_SelectedIndexChanged_Handler);
            this.cboKhoi.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            this.cboHocKy.SelectedIndexChanged += new System.EventHandler(this.cboHocKy_SelectedIndexChanged_Handler); // *** MODIFIED ***
            this.cboMonDay.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            this.cboThang.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger); // *** NEW ***
            this.btnXuatExcel.Click += new System.EventHandler(this.btnXuatExcel_Click);
            this.btnXuatPDF.Click += new System.EventHandler(this.btnXuatPDF_Click);
        }

        private void LoadDuLieu()
        {
            cboKhoi.Items.Clear();
            // Add items as strings representing the grade number
            for (int i = 1; i <= 5; i++)
            {
                cboKhoi.Items.Add(i.ToString());
            }
            splitContainer1.Panel2Collapsed = true;

            // Set placeholder text for ComboBoxes (if not already set in Designer)
            cboKhoi.Text = "Chọn khối...";
            cboLop.Text = "Chọn lớp...";
            cboHocKy.Text = "Chọn học kỳ...";
            cboMonDay.Text = "Chọn môn...";
            cboThang.Text = "Chọn tháng..."; // *** NEW ***
        }

        // New handler for Report Type RadioButtons
        private void ReportType_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                // Update RadioButton styles
                foreach (var button in reportTypeRadioButtons)
                {
                    button.ForeColor = button.Checked ? Color.White : Color.Black;
                    button.BackColor = button.Checked ? Color.FromArgb(0, 120, 215) : Color.FromArgb(240, 240, 240); // Explicitly set backcolor on change
                }

                UpdateControlsVisibility();
                ClearReportData();
            }
        }


        // Old handler (no longer needed if using RadioButtons)
        // private void cboLoaiBaoCao_SelectedIndexChanged(object sender, EventArgs e)
        // {
        //     UpdateControlsVisibility();
        //     ClearReportData();
        // }
        // Add this method to handle the Paint event for pnlFilters
        private void pnlFilters_Paint(object sender, PaintEventArgs e)
        {
            // Draw the border for the filters panel
            ControlPaint.DrawBorder(e.Graphics, pnlFilters.ClientRectangle,
                Color.FromArgb(220, 220, 220), ButtonBorderStyle.Solid);
        }

        // Add this method to handle the Resize event for pnlActions
        private void pnlActions_Resize(object sender, EventArgs e)
        {
            // Vertically center the buttons within the pnlActions panel
            int buttonTop = (btnXuatExcel.Height - 20) / 2;
            if (buttonTop < 0) buttonTop = 0; // Prevent negative Top value

            // Set Top position relative to the panel's client area
            btnXuatExcel.Top = buttonTop;
            btnXuatPDF.Top = buttonTop;
        }
        private void tabLoaiGiaoVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
            LoadClassListBasedOnTab();
            ClearReportData();
        }

        private void cboLop_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            string reportType = GetSelectedReportType(); // Use new helper

            if (reportType == "Bảng điểm học kỳ" && tabLoaiGiaoVien.SelectedTab == tabGiangDay)
            {
                LoadMonHocForSelectedLop();
            }
            // *** NEW: Load months if this report is selected ***
            else if (reportType == "Báo cáo tháng")
            {
                LoadMonHocForSelectedLop();
                LoadThangForSelectedHocKy();
            }

            AutoLoadReport_Trigger(sender, e);
        }

        // *** NEW: Handler for HocKy changed ***
        private void cboHocKy_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            string reportType = GetSelectedReportType();
            if (reportType == "Báo cáo tháng")
            {
                LoadThangForSelectedHocKy();
            }

            // Also trigger the general load
            AutoLoadReport_Trigger(sender, e);
        }


        // *** NEW: Load Monthly Score Types ***
        private void LoadThangForSelectedHocKy()
        {
            var maLop = cboLop.SelectedValue?.ToString();
            int hocKyIndex = cboHocKy.SelectedIndex; // 0 for HK1, 1 for HK2

            this.cboThang.SelectedIndexChanged -= new System.EventHandler(this.AutoLoadReport_Trigger);
            cboThang.DataSource = null;

            if (string.IsNullOrEmpty(maLop) || hocKyIndex == -1)
            {
                cboThang.Text = "Chọn lớp/học kỳ";
                this.cboThang.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
                return;
            }

            int hocKy = hocKyIndex + 1; // 1 for HK1, 2 for HK2

            try
            {
                DataTable dtThang = DatabaseHelper.GetMonthlyScoreTypes(maLop, hocKy); // New helper
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
                this.cboThang.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            }
        }


        private void LoadMonHocForSelectedLop()
        {
            var maLop = cboLop.SelectedValue?.ToString();

            this.cboMonDay.SelectedIndexChanged -= new System.EventHandler(this.AutoLoadReport_Trigger);
            cboMonDay.DataSource = null;

            if (!string.IsNullOrEmpty(maLop))
            {
                try
                {
                    // For monthly report, we need *all* subjects for the class, not just by teacher
                    // But wait, the teacher should only report on *their* subjects.
                    // Let's assume they report on the subjects they teach for that class.
                    // This re-uses the logic from "BangDiem-GiangDay"
                    DataTable dtMon = DatabaseHelper.GetMonHocByGiaoVienAndLop(maGV, maLop);
                    if (dtMon != null && dtMon.Rows.Count > 0)
                    {
                        cboMonDay.DataSource = dtMon;
                        cboMonDay.DisplayMember = "TenMon";
                        cboMonDay.ValueMember = "MaMon";
                        cboMonDay.SelectedIndex = -1;
                        cboMonDay.Text = "Chọn môn...";
                    }
                    else
                    {
                        cboMonDay.Text = "Không có môn";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải danh sách môn học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                cboMonDay.Text = "Chọn lớp trước";
            }

            this.cboMonDay.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
        }

        private void LoadClassListBasedOnTab()
        {
            string reportType = GetSelectedReportType(); // Use new helper
            if (string.IsNullOrEmpty(reportType)) return;

            bool isHomeroomTab = tabLoaiGiaoVien.SelectedTab == tabChuNhiem;
            DataTable dtLop = null;

            this.cboLop.SelectedIndexChanged -= new System.EventHandler(this.cboLop_SelectedIndexChanged_Handler);
            cboLop.DataSource = null;

            try
            {
                if (reportType == "Bảng điểm học kỳ")
                {
                    dtLop = isHomeroomTab ? DatabaseHelper.GetHomeroomClassesByTeacher(maGV) : DatabaseHelper.GetLopByGiaoVien(maGV);
                }
                else
                {
                    // For "Chuyen Can", "Ho So", "Bao Cao Thang" -> Get all classes teacher is associated with
                    dtLop = DatabaseHelper.GetLopByGiaoVien(maGV);
                }

                if (dtLop != null && dtLop.Rows.Count > 0)
                {
                    cboLop.DataSource = dtLop;
                    cboLop.DisplayMember = "TenLop";
                    cboLop.ValueMember = "MaLop";
                    cboLop.SelectedIndex = -1;
                    cboLop.Text = "Chọn lớp...";
                }
                else
                {
                    cboLop.Text = "Chưa có lớp";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cboLop.Text = "Lỗi tải lớp";
            }
            finally
            {
                this.cboLop.SelectedIndexChanged += new System.EventHandler(this.cboLop_SelectedIndexChanged_Handler);
            }
        }

        // Helper to get selected report type from RadioButtons
        private string GetSelectedReportType()
        {
            foreach (var rb in reportTypeRadioButtons)
            {
                if (rb.Checked)
                {
                    return rb.Text;
                }
            }
            return null; // Should not happen if one is checked by default
        }


        private void UpdateControlsVisibility()
        {
            string reportType = GetSelectedReportType(); // Use new helper

            // Hide all optional controls and their labels first
            tabLoaiGiaoVien.Visible = false;
            lblKhoi.Visible = cboKhoi.Visible = false;
            lblLop.Visible = cboLop.Visible = false;
            lblHocKy.Visible = cboHocKy.Visible = false;
            lblMonDay.Visible = cboMonDay.Visible = false;
            lblThang.Visible = cboThang.Visible = false; // *** NEW ***

            // Reset dependent dropdowns to placeholder
            if (cboLop.SelectedIndex != -1) cboLop.SelectedIndex = -1;
            cboLop.Text = "Chọn lớp...";
            if (cboMonDay.DataSource != null) cboMonDay.DataSource = null;
            cboMonDay.Text = "Chọn môn...";
            if (cboKhoi.SelectedIndex != -1) cboKhoi.SelectedIndex = -1;
            cboKhoi.Text = "Chọn khối...";
            if (cboHocKy.SelectedIndex != -1) cboHocKy.SelectedIndex = -1;
            cboHocKy.Text = "Chọn học kỳ...";
            if (cboThang.DataSource != null) cboThang.DataSource = null; // *** NEW ***
            cboThang.Text = "Chọn tháng..."; // *** NEW ***


            // Show controls based on report type
            switch (reportType)
            {
                case "Báo cáo chuyên cần":
                    LoadClassListBasedOnTab();
                    lblLop.Visible = cboLop.Visible = true;
                    lblHocKy.Visible = cboHocKy.Visible = true;
                    cboHocKy.Items.Clear();
                    cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2", "Cả năm" });
                    break;

                case "Bảng điểm học kỳ":
                    tabLoaiGiaoVien.Visible = true;
                    lblLop.Visible = cboLop.Visible = true;
                    lblHocKy.Visible = cboHocKy.Visible = true;
                    LoadClassListBasedOnTab();

                    bool isHomeroomTab = tabLoaiGiaoVien.SelectedTab == tabChuNhiem;

                    if (isHomeroomTab)
                    {
                        cboHocKy.Items.Clear();
                        cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2", "Cả năm" });
                    }
                    else // Tab Giảng Dạy
                    {
                        lblMonDay.Visible = cboMonDay.Visible = true;
                        cboHocKy.Items.Clear();
                        cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2" });
                        LoadMonHocForSelectedLop();
                    }
                    break;

                case "Hồ sơ học sinh":
                    LoadClassListBasedOnTab();
                    lblLop.Visible = cboLop.Visible = true;
                    break;

                // *** NEW CASE ***
                case "Báo cáo tháng":
                    LoadClassListBasedOnTab();
                    lblLop.Visible = cboLop.Visible = true;
                    lblHocKy.Visible = cboHocKy.Visible = true;
                    lblMonDay.Visible = cboMonDay.Visible = true;
                    lblThang.Visible = cboThang.Visible = true; // Show new combo

                    cboHocKy.Items.Clear();
                    cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2" });

                    LoadMonHocForSelectedLop(); // Re-use existing
                    LoadThangForSelectedHocKy(); // New
                    break;

                    // *** REMOVED CASE ***
                    //case "Thống kê tổng hợp khối":
                    //    lblKhoi.Visible = cboKhoi.Visible = true;
                    //    break;
            }
        }


        private void AutoLoadReport_Trigger(object sender, EventArgs e)
        {
            if (sender is ComboBox cbo && cbo.SelectedIndex == -1) return;

            string reportType = GetSelectedReportType(); // Use new helper
            if (string.IsNullOrEmpty(reportType)) return;

            // Validation logic remains the same
            if (reportType == "Bảng điểm học kỳ" && tabLoaiGiaoVien.SelectedTab == tabGiangDay)
            {
                if (cboLop.SelectedIndex == -1 || cboMonDay.SelectedIndex == -1 || cboHocKy.SelectedIndex == -1)
                {
                    ClearReportData();
                    return;
                }
            }
            else if (reportType == "Bảng điểm học kỳ" && tabLoaiGiaoVien.SelectedTab == tabChuNhiem)
            {
                if (cboLop.SelectedIndex == -1 || cboHocKy.SelectedIndex == -1)
                {
                    ClearReportData();
                    return;
                }
            }
            else if (reportType == "Báo cáo chuyên cần")
            {
                if (cboLop.SelectedIndex == -1 || cboHocKy.SelectedIndex == -1)
                {
                    ClearReportData();
                    return;
                }
            }
            else if (reportType == "Hồ sơ học sinh")
            {
                if (cboLop.SelectedIndex == -1)
                {
                    ClearReportData();
                    return;
                }
            }
            // *** NEW VALIDATION ***
            else if (reportType == "Báo cáo tháng")
            {
                if (cboLop.SelectedIndex == -1 || cboHocKy.SelectedIndex == -1 || cboMonDay.SelectedIndex == -1 || cboThang.SelectedIndex == -1)
                {
                    ClearReportData();
                    return;
                }
            }
            // *** REMOVED VALIDATION ***
            //else if (reportType == "Thống kê tổng hợp khối")
            //{
            //    if (cboKhoi.SelectedIndex == -1)
            //    {
            //        ClearReportData();
            //        return;
            //    }
            //}

            LoadReportData();
        }

        private void LoadReportData()
        {
            string reportType = GetSelectedReportType(); // Use new helper
            if (string.IsNullOrEmpty(reportType)) return;

            DataTable dtReport = null;
            try
            {
                switch (reportType)
                {
                    case "Báo cáo chuyên cần":
                        var maLop1 = cboLop.SelectedValue?.ToString();
                        int hocKy1 = cboHocKy.SelectedIndex + 1;
                        dtReport = DatabaseHelper.GetBaoCaoChuyenCan(maLop1, hocKy1);
                        break;
                    case "Bảng điểm học kỳ":
                        var maLop2 = cboLop.SelectedValue?.ToString();
                        int hocKy2 = cboHocKy.SelectedIndex + 1;

                        if (tabLoaiGiaoVien.SelectedTab == tabChuNhiem)
                        {
                            dtReport = DatabaseHelper.GetBangDiemHocKy(maLop2, hocKy2);
                        }
                        else
                        {
                            var maMon = cboMonDay.SelectedValue?.ToString();
                            dtReport = DatabaseHelper.GetBangDiemPivot(maLop2, hocKy2, maMon);
                            CalculateAndAddAverageColumn(dtReport);
                        }
                        break;
                    case "Hồ sơ học sinh":
                        var maLop3 = cboLop.SelectedValue?.ToString();
                        dtReport = DatabaseHelper.GetHoSoHocSinh(maLop3);
                        break;
                    // *** NEW CASE ***
                    case "Báo cáo tháng":
                        var maLop4 = cboLop.SelectedValue?.ToString();
                        var maMon4 = cboMonDay.SelectedValue?.ToString();
                        var loaiDiem4 = cboThang.SelectedValue?.ToString();
                        dtReport = DatabaseHelper.GetBaoCaoThang_ThongKe(maLop4, maMon4, loaiDiem4); // New helper
                        break;
                        // *** REMOVED CASE ***
                        //case "Thống kê tổng hợp khối":
                        //    var khoi = cboKhoi.SelectedItem?.ToString();
                        //    if (!string.IsNullOrEmpty(khoi))
                        //        dtReport = DatabaseHelper.GetThongKeKhoi("Khối " + khoi);
                        //    break;
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
        }

        // *** MODIFIED: To handle the new report type ***
        private void DisplayReportData(DataTable dtReport, string reportType)
        {
            if (reportType == "Báo cáo tháng")
            {
                dgvDuLieu.DataSource = null; // Clear main grid
                flpCharts.Controls.Clear(); // Clear bottom panel

                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    // 1. Create and populate the Score Stats table (dgvDuLieu)
                    DataTable dtDiem = CreateStatsTable(dtReport, "Diem");
                    dgvDuLieu.DataSource = dtDiem;
                    RenameMonthlyReportColumns(dgvDuLieu, true); // Custom renamer

                    // 2. Create and populate the Ranking Stats table (in flpCharts)
                    DataTable dtXepLoai = CreateStatsTable(dtReport, "XepLoai");
                    DataGridView dgvXepLoai = new DataGridView();
                    StyleDataGridViewModern(dgvXepLoai); // Use existing style
                    dgvXepLoai.DataSource = dtXepLoai;
                    RenameMonthlyReportColumns(dgvXepLoai, false); // Custom renamer

                    // Set size for the second grid
                    int height = dgvXepLoai.ColumnHeadersHeight + (dtXepLoai.Rows.Count * dgvXepLoai.RowTemplate.Height) + 3;
                    dgvXepLoai.Size = new Size(flpCharts.ClientSize.Width - 25, height);
                    dgvXepLoai.MinimumSize = new Size(400, 150);
                    dgvXepLoai.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;


                    flpCharts.Controls.Add(dgvXepLoai);

                    splitContainer1.Panel2Collapsed = false;
                }
                else
                {
                    splitContainer1.Panel2Collapsed = true;
                }
            }
            else
            {
                // Original logic for other reports
                dgvDuLieu.DataSource = dtReport;
                flpCharts.Controls.Clear();

                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    RenameDataGridViewColumns(); // Original renamer
                    UpdateCharts(dtReport, reportType); // Original chart logic
                    splitContainer1.Panel2Collapsed = flpCharts.Controls.Count == 0;
                }
                else
                {
                    splitContainer1.Panel2Collapsed = true;
                }
            }
        }

        // *** NEW: Helper to create the stats tables from the SP result ***
        private DataTable CreateStatsTable(DataTable sourceDt, string loaiThongKe)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PhanLoai", typeof(string));
            dt.Columns.Add("TS", typeof(int));
            dt.Columns.Add("Nu", typeof(int));
            dt.Columns.Add("DanToc", typeof(int));
            dt.Columns.Add("NDT", typeof(int));

            DataRow[] rows = sourceDt.Select($"LoaiThongKe = '{loaiThongKe}'");
            var dataMap = rows.ToDictionary(r => r["PhanLoai"].ToString(), r => r);

            int totalTS = 0, totalNu = 0, totalDanToc = 0, totalNDT = 0;

            if (loaiThongKe == "Diem")
            {
                string[] scoreOrder = { "10", "9", "8", "7", "6", "5", "<5" };
                foreach (string key in scoreOrder)
                {
                    int ts = 0, nu = 0, dtoc = 0, ndt = 0;
                    if (dataMap.ContainsKey(key))
                    {
                        ts = Convert.ToInt32(dataMap[key]["TS"]);
                        nu = Convert.ToInt32(dataMap[key]["Nu"]);
                        dtoc = Convert.ToInt32(dataMap[key]["DanToc"]);
                        ndt = Convert.ToInt32(dataMap[key]["NDT"]);
                    }
                    dt.Rows.Add(key, ts, nu, dtoc, ndt);
                    totalTS += ts;
                    totalNu += nu;
                    totalDanToc += dtoc;
                    totalNDT += ndt;
                }
                dt.Rows.Add("Tổng", totalTS, totalNu, totalDanToc, totalNDT);
            }
            else // XepLoai
            {
                string[] rankOrder = { "T", "H", "C" };
                foreach (string key in rankOrder)
                {
                    int ts = 0, nu = 0, dtoc = 0, ndt = 0;
                    if (dataMap.ContainsKey(key))
                    {
                        ts = Convert.ToInt32(dataMap[key]["TS"]);
                        nu = Convert.ToInt32(dataMap[key]["Nu"]);
                        dtoc = Convert.ToInt32(dataMap[key]["DanToc"]);
                        ndt = Convert.ToInt32(dataMap[key]["NDT"]);
                    }
                    dt.Rows.Add(key, ts, nu, dtoc, ndt);
                    // No total row for ranking table per user image
                }
            }
            return dt;
        }

        // *** NEW: Helper to rename columns for the new report ***
        private void RenameMonthlyReportColumns(DataGridView dgv, bool isScoreTable)
        {
            if (dgv.DataSource == null) return;
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                switch (col.DataPropertyName)
                {
                    case "PhanLoai":
                        col.HeaderText = isScoreTable ? "Điểm" : "Xếp loại";
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        col.DefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        break;
                    case "TS": col.HeaderText = "TS"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "Nu": col.HeaderText = "Nữ"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "DanToc": col.HeaderText = "Dân tộc"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "NDT": col.HeaderText = "NDT"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                }
            }
            // Bold the "Tổng" row
            if (isScoreTable)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells[0].Value?.ToString() == "Tổng")
                    {
                        row.DefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
                        row.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                    }
                }
            }
        }


        // *** MODIFIED: Removed ThongKeKhoi ***
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
                    // *** REMOVED ***
                    //case "Thống kê tổng hợp khối":
                    //    DrawStudentCountChart(dt);
                    //    DrawAverageScoreChart(dt);
                    //    break;
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
            string scoreColumnName = "";
            if (dt.Columns.Contains("DiemTB"))
                scoreColumnName = "DiemTB";
            else if (dt.Columns.Contains("Trung bình chung"))
                scoreColumnName = "Trung bình chung";

            if (string.IsNullOrEmpty(scoreColumnName)) return;

            var scoreCounts = new Dictionary<string, int> {
                {"Yếu (<5)", 0}, {"TB (5-<7)", 0}, {"Khá (7-<9)", 0}, {"Giỏi (>=9)", 0}
            };
            int validScores = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row[scoreColumnName] != DBNull.Value)
                {
                    double score = Convert.ToDouble(row[scoreColumnName]);
                    validScores++;
                    if (score < 5) scoreCounts["Yếu (<5)"]++;
                    else if (score < 7) scoreCounts["TB (5-<7)"]++;
                    else if (score < 9) scoreCounts["Khá (7-<9)"]++;
                    else scoreCounts["Giỏi (>=9)"]++;
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
                    double percentage = (double)kvp.Value / validScores;
                    series.Points[pIdx].ToolTip = $"{kvp.Key}: {kvp.Value} HS ({percentage:P1})";
                    series.Points[pIdx].Label = kvp.Value.ToString();
                    series.Points[pIdx].Color = colors[pointIndex % colors.Length];
                }
                pointIndex++;
            }


            series["PixelPointWidth"] = "40";
            chart.Series.Add(series);
            flpCharts.Controls.Add(chart);
        }

        // *** REMOVED: DrawStudentCountChart ***
        // *** REMOVED: DrawAverageScoreChart ***

        private void CalculateAndAddAverageColumn(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;
            if (!dt.Columns.Contains("DiemTB"))
            {
                dt.Columns.Add("DiemTB", typeof(double));
            }
            foreach (DataRow row in dt.Rows)
            {
                double totalScore = 0;
                int totalWeight = 0;
                var scoreCols = dt.Columns.Cast<DataColumn>()
                                        .Where(c => c.ColumnName.StartsWith("Thang") || c.ColumnName == "GiuaKi" || c.ColumnName == "CuoiKi")
                                        .Select(c => c.ColumnName).ToList();

                foreach (string colName in scoreCols)
                {
                    if (row.Table.Columns.Contains(colName) && row[colName] != DBNull.Value)
                    {
                        double score = Convert.ToDouble(row[colName]);
                        int weight = 1;
                        if (colName == "GiuaKi") weight = 2;
                        else if (colName == "CuoiKi") weight = 3;

                        totalScore += score * weight;
                        totalWeight += weight;
                    }
                }

                if (totalWeight > 0)
                {
                    double avg = totalScore / totalWeight;
                    row["DiemTB"] = Math.Round(avg, 2);
                }
                else
                {
                    row["DiemTB"] = DBNull.Value;
                }
            }
        }

        private void RenameDataGridViewColumns()
        {
            if (dgvDuLieu.DataSource == null) return;
            foreach (DataGridViewColumn col in dgvDuLieu.Columns)
            {
                // Store original name if needed, though DataPropertyName is better
                col.Tag = col.DataPropertyName; // Store original binding name if needed later

                switch (col.DataPropertyName)
                {
                    case "MaHS": col.HeaderText = "Mã HS"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "HoTen": col.HeaderText = "Họ Tên"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "TenLop": col.HeaderText = "Lớp"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "SoBuoiCoMat": col.HeaderText = "Có Mặt"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "SoBuoiVang": col.HeaderText = "Vắng KP"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "SoBuoiVangCoPhep": col.HeaderText = "Vắng CP"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "TongSoBuoi": col.HeaderText = "Tổng Buổi"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "TyLeChuyenCan": col.HeaderText = "Tỷ Lệ CC (%)"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; col.DefaultCellStyle.Format = "N0"; break;
                    case "Thang1": col.HeaderText = "T1"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "Thang2": col.HeaderText = "T2"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "Thang3": col.HeaderText = "T3"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "GiuaKi": col.HeaderText = "GK"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "CuoiKi": col.HeaderText = "CK"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N1"; break;
                    case "DiemTB": col.HeaderText = "Điểm TB"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N2"; col.DefaultCellStyle.Font = new Font(dgvDuLieu.Font, FontStyle.Bold); break;
                    case "Trung bình chung": col.HeaderText = "TB Chung"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "N2"; col.DefaultCellStyle.Font = new Font(dgvDuLieu.Font, FontStyle.Bold); break;
                    case "NhanXet": col.HeaderText = "Nhận Xét"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "GhiChu": col.HeaderText = "Ghi Chú"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "GioiTinh": col.HeaderText = "Giới Tính"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "NgaySinh": col.HeaderText = "Ngày Sinh"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; col.DefaultCellStyle.Format = "dd/MM/yyyy"; break;
                    case "DanToc": col.HeaderText = "Dân Tộc"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "DiaChi": col.HeaderText = "Địa Chỉ"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "SDTPhuHuynh": col.HeaderText = "SĐT PH"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; break;
                    case "SoHocSinh": col.HeaderText = "Sĩ Số"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "DiemTrungBinh": col.HeaderText = "Điểm TB Khối"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; col.DefaultCellStyle.Format = "N2"; break;
                    case "SoNam": col.HeaderText = "Số Nam"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "SoNu": col.HeaderText = "Số Nữ"; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; break;
                    case "MaGV":
                    case "MaLop":
                    case "MaMon": col.Visible = false; break;
                    // Handle potential dynamic columns from PIVOT for scores by GVCN
                    default:
                        if (col.ValueType == typeof(double) || col.ValueType == typeof(decimal) || col.ValueType == typeof(float))
                        {
                            col.HeaderText = col.DataPropertyName; // Show subject name
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            col.DefaultCellStyle.Format = "N1";
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        else if (!col.Visible)
                        {
                            // Keep hidden if already hidden
                        }
                        else
                        {
                            col.HeaderText = col.DataPropertyName; // Default show original name
                        }
                        break;
                }
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                if (col.ValueType == typeof(int) || col.ValueType == typeof(long) || col.ValueType == typeof(double) || col.ValueType == typeof(decimal) || col.ValueType == typeof(float) || col.HeaderText.Contains("%"))
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
                FileName = $"BaoCao_{GetSelectedReportType()}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // *** TODO: This only exports the main grid.
                    // For "Bao Cao Thang", you might want to export both grids.
                    // For simplicity, this implementation only exports the main grid (dgvDuLieu).
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
            string reportType = GetSelectedReportType();
            if (dgvDuLieu.Rows.Count == 0 && reportType != "Báo cáo tháng")
            {
                MessageBox.Show("Chưa có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (reportType == "Báo cáo tháng" && dgvDuLieu.Rows.Count == 0 && flpCharts.Controls.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Lưu file PDF",
                FileName = $"BaoCao_{reportType}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string reportTypeTitle = reportType?.ToUpper() ?? "BÁO CÁO";
                    string reportTitle = reportTypeTitle;

                    if (cboLop.Visible && cboLop.SelectedIndex != -1) reportTitle += $"\nLỚP: {cboLop.Text}";
                    if (cboKhoi.Visible && cboKhoi.SelectedIndex != -1) reportTitle += $"\nKHỐI: {cboKhoi.SelectedItem}";
                    if (cboHocKy.Visible && cboHocKy.SelectedIndex != -1) reportTitle += $" - {cboHocKy.Text.ToUpper()}";
                    if (cboMonDay.Visible && cboMonDay.SelectedIndex != -1) reportTitle += $"\nMÔN: {cboMonDay.Text}";
                    if (cboThang.Visible && cboThang.SelectedIndex != -1) reportTitle += $"\nTHÁNG: {cboThang.Text}"; // *** NEW ***

                    // Get charts (for old reports)
                    var chartImages = flpCharts.Controls.OfType<Chart>().Select(chart =>
                    {
                        using (var ms = new MemoryStream())
                        {
                            chart.SaveImage(ms, ChartImageFormat.Png);
                            return (Image)Image.FromStream(ms).Clone();
                        }
                    }).ToList(); // <-- Changed to List

                    // *** NEW: Get secondary grid (for new report) ***
                    Image secondaryGridImage = null;
                    if (reportType == "Báo cáo tháng")
                    {
                        var dgvSecondary = flpCharts.Controls.OfType<DataGridView>().FirstOrDefault();
                        if (dgvSecondary != null)
                        {
                            // Ensure the grid has a proper size before drawing
                            dgvSecondary.Size = new Size(Math.Max(500, dgvSecondary.Width), dgvSecondary.Height);
                            secondaryGridImage = new Bitmap(dgvSecondary.Width, dgvSecondary.Height);
                            dgvSecondary.DrawToBitmap((Bitmap)secondaryGridImage, new Rectangle(0, 0, dgvSecondary.Width, dgvSecondary.Height));
                            chartImages.Add(secondaryGridImage);
                        }
                    }

                    ExportHelper.ExportToPDF(dgvDuLieu, saveFileDialog.FileName, reportTitle, null, chartImages.ToArray());
                    MessageBox.Show($"Đã xuất báo cáo ra file:\n{saveFileDialog.FileName}", "Xuất PDF thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    foreach (var img in chartImages) img?.Dispose();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}