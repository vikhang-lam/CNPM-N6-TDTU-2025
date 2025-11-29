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
    public partial class UC_BaoCao : UserControl
    {
        #region Fields

        private string maGV;
        private List<RadioButton> reportTypeRadioButtons = new List<RadioButton>();
        private bool isProgrammaticChange = false;

        #endregion

        #region Initialization & Styling

        public UC_BaoCao(string maGVien)
        {
            maGV = maGVien;
            InitializeComponent();
            ApplyModernStyles();
            LoadDuLieu();
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
                reportTypeRadioButtons[0].Checked = true;
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
            this.tabLoaiGiaoVien.SelectedIndexChanged += new System.EventHandler(this.tabLoaiGiaoVien_SelectedIndexChanged);
            this.cboLop.SelectedIndexChanged += new System.EventHandler(this.cboLop_SelectedIndexChanged_Handler);
            this.cboKhoi.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            this.cboHocKy.SelectedIndexChanged += new System.EventHandler(this.cboHocKy_SelectedIndexChanged_Handler);
            this.cboMonDay.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            this.cboThang.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            this.btnXuatExcel.Click += new System.EventHandler(this.btnXuatExcel_Click);
            this.btnXuatPDF.Click += new System.EventHandler(this.btnXuatPDF_Click);
            this.pnlFilters.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlFilters_Paint);
        }

        private void LoadDuLieu()
        {
            isProgrammaticChange = true;
            cboKhoi.Items.Clear();
            for (int i = 1; i <= 5; i++)
            {
                cboKhoi.Items.Add(i.ToString());
            }
            splitContainer1.Panel2Collapsed = true;

            cboKhoi.Text = "Chọn khối...";
            cboLop.Text = "Chọn lớp...";
            cboHocKy.Text = "Chọn học kỳ...";
            cboMonDay.Text = "Chọn môn...";
            cboThang.Text = "Chọn tháng...";

            ToggleNoDataMessage(true);

            isProgrammaticChange = false;
        }

        #endregion

        #region UI Event Handlers

        private void ReportType_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                foreach (var button in reportTypeRadioButtons)
                {
                    button.ForeColor = button.Checked ? Color.White : Color.Black;
                    button.BackColor = button.Checked ? Color.FromArgb(0, 120, 215) : Color.FromArgb(240, 240, 240);
                }

                UpdateControlsVisibility();
                ClearReportData();
            }
        }

        private void pnlFilters_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, pnlFilters.ClientRectangle,
                Color.FromArgb(220, 220, 220), ButtonBorderStyle.Solid);
        }

        private void tabLoaiGiaoVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
            LoadClassListBasedOnTab();
            ClearReportData();
        }

        private void cboLop_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            if (isProgrammaticChange) return;

            string reportType = GetSelectedReportType();
            if (reportType == "Bảng điểm học kỳ" && tabLoaiGiaoVien.SelectedTab == tabGiangDay)
            {
                LoadMonHocForSelectedLop();
            }
            else if (reportType == "Báo cáo tháng")
            {
                LoadMonHocForSelectedLop();
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

        private void AutoLoadReport_Trigger(object sender, EventArgs e)
        {
            if (isProgrammaticChange) return;
            if (sender is ComboBox cbo && cbo.SelectedIndex == -1) return;

            string reportType = GetSelectedReportType();
            if (string.IsNullOrEmpty(reportType)) return;

            bool canLoad = false;
            if (reportType == "Bảng điểm học kỳ" && tabLoaiGiaoVien.SelectedTab == tabGiangDay)
            {
                canLoad = (cboLop.SelectedIndex != -1 && cboMonDay.SelectedIndex != -1 && cboHocKy.SelectedIndex != -1);
            }
            else if (reportType == "Bảng điểm học kỳ" && tabLoaiGiaoVien.SelectedTab == tabChuNhiem)
            {
                canLoad = (cboLop.SelectedIndex != -1 && cboHocKy.SelectedIndex != -1);
            }
            else if (reportType == "Báo cáo chuyên cần")
            {
                canLoad = (cboLop.SelectedIndex != -1 && cboHocKy.SelectedIndex != -1);
            }
            else if (reportType == "Hồ sơ học sinh")
            {
                canLoad = (cboLop.SelectedIndex != -1);
            }
            else if (reportType == "Báo cáo tháng")
            {
                canLoad = (cboLop.SelectedIndex != -1 && cboHocKy.SelectedIndex != -1 && cboMonDay.SelectedIndex != -1 && cboThang.SelectedIndex != -1);
            }

            if (canLoad)
            {
                LoadReportData();
            }
        }

        #endregion

        #region Data Loading Helpers

        private void LoadClassListBasedOnTab()
        {
            string reportType = GetSelectedReportType();
            if (string.IsNullOrEmpty(reportType)) return;

            bool isHomeroomTab = tabLoaiGiaoVien.SelectedTab == tabChuNhiem;
            DataTable dtLop = null;

            isProgrammaticChange = true;
            cboLop.DataSource = null;

            try
            {
                if (reportType == "Bảng điểm học kỳ")
                {
                    dtLop = isHomeroomTab ? DatabaseHelper.GetHomeroomClassesByTeacher(maGV) : DatabaseHelper.GetClassesByTeacher(maGV);
                }
                else
                {
                    dtLop = DatabaseHelper.GetClassesByTeacher(maGV);
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
                isProgrammaticChange = false;
            }
        }

        private void LoadMonHocForSelectedLop()
        {
            var maLop = cboLop.SelectedValue?.ToString();
            isProgrammaticChange = true;
            cboMonDay.DataSource = null;

            if (!string.IsNullOrEmpty(maLop))
            {
                try
                {
                    DataTable dtMon = DatabaseHelper.GetSubjectsByTeacherAndClass(maGV, maLop);
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
            isProgrammaticChange = false;
        }

        private void LoadThangForSelectedHocKy()
        {
            var maLop = cboLop.SelectedValue?.ToString();
            int hocKyIndex = cboHocKy.SelectedIndex;

            isProgrammaticChange = true;
            cboThang.DataSource = null;

            if (string.IsNullOrEmpty(maLop) || hocKyIndex == -1)
            {
                cboThang.Text = "Chọn lớp/học kỳ";
                isProgrammaticChange = false;
                return;
            }

            int hocKy = hocKyIndex + 1;

            try
            {
                DataTable dtThang = DatabaseHelper.GetMonthlyScoreTypes(maLop, hocKy);
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

            tabLoaiGiaoVien.Visible = false;
            lblKhoi.Visible = cboKhoi.Visible = false;
            lblLop.Visible = cboLop.Visible = false;
            lblHocKy.Visible = cboHocKy.Visible = false;
            lblMonDay.Visible = cboMonDay.Visible = false;
            lblThang.Visible = cboThang.Visible = false;

            isProgrammaticChange = true;
            if (cboLop.SelectedIndex != -1) cboLop.SelectedIndex = -1;
            cboLop.Text = "Chọn lớp...";
            if (cboMonDay.DataSource != null) cboMonDay.DataSource = null;
            cboMonDay.Text = "Chọn môn...";
            if (cboKhoi.SelectedIndex != -1) cboKhoi.SelectedIndex = -1;
            cboKhoi.Text = "Chọn khối...";
            if (cboHocKy.SelectedIndex != -1) cboHocKy.SelectedIndex = -1;
            cboHocKy.Text = "Chọn học kỳ...";
            if (cboThang.DataSource != null) cboThang.DataSource = null;
            cboThang.Text = "Chọn tháng...";
            isProgrammaticChange = false;

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
                    else
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

                case "Báo cáo tháng":
                    LoadClassListBasedOnTab();
                    lblLop.Visible = cboLop.Visible = true;
                    lblHocKy.Visible = cboHocKy.Visible = true;
                    lblMonDay.Visible = cboMonDay.Visible = true;
                    lblThang.Visible = cboThang.Visible = true;

                    cboHocKy.Items.Clear();
                    cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2" });

                    LoadMonHocForSelectedLop();
                    LoadThangForSelectedHocKy();
                    break;
            }
        }

        #endregion

        #region Report Generation & Display

        private void LoadReportData()
        {
            string reportType = GetSelectedReportType();
            if (string.IsNullOrEmpty(reportType)) return;

            DataTable dtReport = null;
            try
            {
                switch (reportType)
                {
                    case "Báo cáo chuyên cần":
                        var maLop1 = cboLop.SelectedValue?.ToString();
                        int hocKy1 = cboHocKy.SelectedIndex + 1;
                        dtReport = DatabaseHelper.GetAttendanceReport(maLop1, hocKy1);
                        break;
                    case "Bảng điểm học kỳ":
                        var maLop2 = cboLop.SelectedValue?.ToString();
                        int hocKy2 = cboHocKy.SelectedIndex + 1;

                        if (tabLoaiGiaoVien.SelectedTab == tabChuNhiem)
                        {
                            dtReport = DatabaseHelper.GetSemesterScoreboard(maLop2, hocKy2);
                        }
                        else
                        {
                            var maMon = cboMonDay.SelectedValue?.ToString();
                            dtReport = DatabaseHelper.GetScoreboardPivot(maLop2, hocKy2, maMon);
                            CalculateAndAddAverageColumn(dtReport);
                        }
                        break;
                    case "Hồ sơ học sinh":
                        var maLop3 = cboLop.SelectedValue?.ToString();
                        dtReport = DatabaseHelper.GetStudentRecords(maLop3);
                        break;
                    case "Báo cáo tháng":
                        var maLop4 = cboLop.SelectedValue?.ToString();
                        var maMon4 = cboMonDay.SelectedValue?.ToString();
                        var loaiDiem4 = cboThang.SelectedValue?.ToString();
                        dtReport = DatabaseHelper.GetMonthlyReport_Statistics(maLop4, maMon4, loaiDiem4);
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
            foreach (Chart chart in flpCharts.Controls.OfType<Chart>().ToList())
            {
                chart.Dispose();
            }
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
            ClearReportData();
            splitContainer1.Panel2Collapsed = true;

            var dataMapDiem = dtReport.Select("LoaiThongKe = 'Diem'").ToDictionary(r => r["PhanLoai"].ToString(), r => r);
            var dataMapXepLoai = dtReport.Select("LoaiThongKe = 'XepLoai'").ToDictionary(r => r["PhanLoai"].ToString(), r => r);

            DataTable dtCombined = new DataTable();
            dtCombined.Columns.Add("PhanLoai", typeof(string));
            dtCombined.Columns.Add("Col_TS", typeof(string));
            dtCombined.Columns.Add("Col_Nu", typeof(string));
            dtCombined.Columns.Add("Col_DanToc_Percent", typeof(string));
            dtCombined.Columns.Add("Col_NDT", typeof(string));
            dtCombined.Columns.Add("IsHeader", typeof(int));

            dtCombined.Rows.Add("ĐIỂM", "TS", "Nữ", "Dân tộc", "NDT", 1);

            string[] scoreOrder = { "10", "9", "8", "7", "6", "5", "Dưới 5" };
            foreach (string key in scoreOrder)
            {
                string ts = "0", nu = "0", dtoc = "0", ndt = "0";
                string dbKey = (key == "Dưới 5") ? "<5" : key;

                if (dataMapDiem.ContainsKey(dbKey))
                {
                    ts = dataMapDiem[dbKey]["TS"].ToString();
                    nu = dataMapDiem[dbKey]["Nu"].ToString();
                    dtoc = dataMapDiem[dbKey]["DanToc"].ToString();
                    ndt = dataMapDiem[dbKey]["NDT"].ToString();
                }
                dtCombined.Rows.Add(key, ts, nu, dtoc, ndt, 0);
            }

            int totalTS = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["TS"]));
            int totalNu = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["Nu"]));
            int totalDanToc = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["DanToc"]));
            int totalNDT = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["NDT"]));
            dtCombined.Rows.Add("Tổng", totalTS.ToString(), totalNu.ToString(), totalDanToc.ToString(), totalNDT.ToString(), 0);

            dtCombined.Rows.Add("XẾP LOẠI", "TS", "Nữ", "Dân tộc", "NDT", 1);

            string[] rankOrder = { "T", "H", "C" };
            foreach (string key in rankOrder)
            {
                string ts = "0", nu = "0", dtoc = "0", ndt = "0";
                if (dataMapXepLoai.ContainsKey(key))
                {
                    ts = dataMapXepLoai[key]["TS"].ToString();
                    nu = dataMapXepLoai[key]["Nu"].ToString();
                    dtoc = dataMapXepLoai[key]["DanToc"].ToString();
                    ndt = dataMapXepLoai[key]["NDT"].ToString();
                }
                dtCombined.Rows.Add(key, ts, nu, dtoc, ndt, 0);
            }

            dgvDuLieu.DataSource = dtCombined;

            ToggleNoDataMessage(false);

            FormatCombinedMonthlyReportGrid(dgvDuLieu);
        }

        private void FormatCombinedMonthlyReportGrid(DataGridView dgv)
        {
            if (dgv.DataSource == null) return;
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                switch (col.DataPropertyName)
                {
                    case "PhanLoai":
                        col.HeaderText = "";
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        break;
                    case "Col_TS": col.HeaderText = "TS"; break;
                    case "Col_Nu": col.HeaderText = "Nữ"; break;
                    case "Col_DanToc_Percent": col.HeaderText = "Dân tộc"; break;
                    case "Col_NDT": col.HeaderText = "NDT"; break;
                    case "IsHeader": col.Visible = false; break;
                }
            }
            foreach (DataGridViewRow row in dgv.Rows)
            {
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
            if (dt.Columns.Contains("DiemTB")) scoreColumnName = "DiemTB";
            else if (dt.Columns.Contains("Trung bình chung")) scoreColumnName = "Trung bình chung";

            if (string.IsNullOrEmpty(scoreColumnName)) return;

            var scoreCounts = new Dictionary<string, int> { { "Yếu (<5)", 0 }, { "TB (5-<7)", 0 }, { "Khá (7-<9)", 0 }, { "Giỏi (>=9)", 0 } };
            int validScores = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row[scoreColumnName] != DBNull.Value)
                {
                    try
                    {
                        double score = Convert.ToDouble(row[scoreColumnName]);
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

        #endregion

        #region Other Logic

        private void CalculateAndAddAverageColumn(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;
            if (!dt.Columns.Contains("DiemTB")) dt.Columns.Add("DiemTB", typeof(double));

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
                        try
                        {
                            double score = Convert.ToDouble(row[colName]);
                            int weight = 1;
                            if (colName == "GiuaKi") weight = 2;
                            else if (colName == "CuoiKi") weight = 3;

                            totalScore += score * weight;
                            totalWeight += weight;
                        }
                        catch { }
                    }
                }

                if (totalWeight > 0) row["DiemTB"] = Math.Round(totalScore / totalWeight, 2);
                else row["DiemTB"] = DBNull.Value;
            }
        }

        private void RenameDataGridViewColumns()
        {
            if (dgvDuLieu.DataSource == null) return;
            foreach (DataGridViewColumn col in dgvDuLieu.Columns)
            {
                col.Tag = col.DataPropertyName;
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
                    default:
                        if (col.ValueType == typeof(double) || col.ValueType == typeof(decimal) || col.ValueType == typeof(float))
                        {
                            col.HeaderText = col.DataPropertyName;
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            col.DefaultCellStyle.Format = "N1";
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        else if (!col.Visible) { }
                        else col.HeaderText = col.DataPropertyName;
                        break;
                }

                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                if (col.ValueType == typeof(int) || col.ValueType == typeof(long) || col.ValueType == typeof(double) || col.ValueType == typeof(decimal) || col.ValueType == typeof(float) || col.HeaderText.Contains("%"))
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                else if (!string.IsNullOrEmpty(col.DefaultCellStyle.Format) && col.DefaultCellStyle.Format.Contains("dd/MM/yyyy"))
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                else
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
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
            if (dgvDuLieu.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mainHeader = "";
            string className = "";
            if (cboLop.Visible && cboLop.SelectedIndex != -1)
            {
                className = cboLop.Text.Trim();
                mainHeader = $"{className.ToUpper()}";
            }
            else
            {
                MessageBox.Show("Vui lòng chọn Lớp trước khi xuất.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Lưu file PDF",
                FileName = $"BaoCao_{reportType.Replace(" ", "")}_{className.Replace(" ", "")}_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<Image> chartImages = new List<Image>();
                try
                {
                    string reportTypeTitle = reportType?.ToUpper() ?? "BÁO CÁO";
                    string documentTitle = reportTypeTitle;
                    documentTitle += $"\nMÃ: {cboLop.Text}";
                    if (cboHocKy.Visible && cboHocKy.SelectedIndex != -1) documentTitle += $" - {cboHocKy.Text.ToUpper()}";
                    if (cboMonDay.Visible && cboMonDay.SelectedIndex != -1) documentTitle += $"\nMÔN: {cboMonDay.Text}";
                    if (cboThang.Visible && cboThang.SelectedIndex != -1) documentTitle += $"\nTHÁNG: {cboThang.Text}";

                    chartImages = flpCharts.Controls.OfType<Chart>().Select(chart =>
                    {
                        using (var ms = new MemoryStream())
                        {
                            chart.SaveImage(ms, ChartImageFormat.Png);
                            return (Image)Image.FromStream(ms).Clone();
                        }
                    }).ToList();

                    ExportHelper.ExportToPDF(dgvDuLieu, saveFileDialog.FileName, documentTitle, mainHeader, chartImages.ToArray());
                    MessageBox.Show($"Đã xuất báo cáo ra file:\n{saveFileDialog.FileName}", "Xuất PDF thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    foreach (var img in chartImages) img?.Dispose();
                }
            }
        }

        #endregion
    }
}