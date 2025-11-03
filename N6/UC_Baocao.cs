using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics; // Thêm

namespace N6
{
    /// <summary>
    /// UserControl để xem, lọc và xuất các loại báo cáo khác nhau (chuyên cần, điểm số, hồ sơ).
    /// </summary>
    public partial class UC_BaoCao : UserControl
    {
        #region Fields (Biến thành viên)

        private string maGV;
        private Label lblNoDataMessage;
        // Giữ tham chiếu đến các RadioButton để dễ dàng truy cập và gỡ sự kiện
        private List<RadioButton> reportTypeRadioButtons = new List<RadioButton>();

        #endregion

        #region Initialization & Styling

        public UC_BaoCao(string maGVien)
        {
            maGV = maGVien;
            InitializeComponent(); // Gọi mã do Designer tạo
            InitializeNoDataLabel();
            ApplyModernStyles(); // Áp dụng style tùy chỉnh
            LoadDuLieu(); // Tải dữ liệu ban đầu cho ComboBox
            SetupEventHandlers(); // Gán các sự kiện
            UpdateControlsVisibility(); // Cập nhật hiển thị control lần đầu
        }
        /// <summary>
        /// Khởi tạo Label dùng để hiển thị thông báo "Không có dữ liệu".
        /// </summary>
        
        private void InitializeNoDataLabel()
        {
            lblNoDataMessage = new Label
            {
                Name = "lblNoDataMessage",
                Text = "Không có dữ liệu để hiển thị.",
                Font = new Font("Segoe UI", 14F, FontStyle.Italic),
                ForeColor = Color.Gray,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false, // Ẩn ban đầu
                BackColor = Color.White
            };

            // Thêm label này vào Panel1 của SplitContainer (Panel chứa DataGridView)
            this.splitContainer1.Panel1.Controls.Add(lblNoDataMessage);
            lblNoDataMessage.BringToFront(); // Đảm bảo nó che dgvDuLieu
        }

        /// <summary>
        /// Áp dụng các style hiện đại không thể cài đặt trong Designer.
        /// </summary>
        private void ApplyModernStyles()
        {
            StyleDataGridViewModern(this.dgvDuLieu);

            // Style các RadioButton trong panel chọn loại báo cáo
            foreach (Control ctrl in pnlReportTypeSelector.Controls)
            {
                if (ctrl is RadioButton rb)
                {
                    rb.Appearance = Appearance.Button;
                    rb.FlatStyle = FlatStyle.Flat;
                    rb.FlatAppearance.BorderSize = 0;
                    rb.FlatAppearance.CheckedBackColor = Color.FromArgb(0, 120, 215); // Xanh dương khi chọn
                    rb.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 235, 250); // Xanh nhạt khi hover
                    rb.BackColor = Color.FromArgb(240, 240, 240); // Xám nhạt mặc định
                    rb.ForeColor = Color.Black;
                    rb.TextAlign = ContentAlignment.MiddleCenter;
                    rb.Padding = new Padding(10, 0, 10, 0);
                    rb.MinimumSize = new Size(150, 35);
                    rb.AutoSize = true;
                    reportTypeRadioButtons.Add(rb); // Thêm vào danh sách để quản lý
                    rb.CheckedChanged += ReportType_CheckedChanged; // Gán sự kiện
                }
            }

            // Chọn mục đầu tiên làm mặc định
            if (reportTypeRadioButtons.Count > 0)
            {
                reportTypeRadioButtons[0].Checked = true;
            }

            // Style các nút hành động (Xuất file)
            StyleActionButton(btnXuatExcel, Color.FromArgb(16, 124, 65)); // Xanh lá
            StyleActionButton(btnXuatPDF, Color.FromArgb(217, 83, 79)); // Đỏ

            splitContainer1.BackColor = Color.FromArgb(220, 220, 220);
        }

        /// <summary>
        /// Áp dụng style chung cho các nút hành động (Button).
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
            // Ghi chú: Thêm Image/TextImageRelation ở đây nếu cần icon
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
            // Sự kiện RadioButton đã gán trong ApplyModernStyles
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

        /// <summary>
        /// Tải dữ liệu ban đầu cho các ComboBox (ví dụ: danh sách khối).
        /// </summary>
        private void LoadDuLieu()
        {
            cboKhoi.Items.Clear();
            for (int i = 1; i <= 5; i++)
            {
                cboKhoi.Items.Add(i.ToString());
            }
            splitContainer1.Panel2Collapsed = true;

            // Đặt placeholder
            cboKhoi.Text = "Chọn khối...";
            cboLop.Text = "Chọn lớp...";
            cboHocKy.Text = "Chọn học kỳ...";
            cboMonDay.Text = "Chọn môn...";
            cboThang.Text = "Chọn tháng...";
        }

        #endregion

        #region UI Event Handlers (Xử lý sự kiện UI)

        /// <summary>
        /// Xử lý khi chọn một loại báo cáo (RadioButton).
        /// </summary>
        private void ReportType_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                // Cập nhật style cho các nút RadioButton
                foreach (var button in reportTypeRadioButtons)
                {
                    button.ForeColor = button.Checked ? Color.White : Color.Black;
                    button.BackColor = button.Checked ? Color.FromArgb(0, 120, 215) : Color.FromArgb(240, 240, 240);
                }

                UpdateControlsVisibility(); // Ẩn/hiện các bộ lọc
                ClearReportData(); // Xóa dữ liệu báo cáo cũ
            }
        }

        /// <summary>
        /// Vẽ viền cho panel bộ lọc.
        /// </summary>
        private void pnlFilters_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, pnlFilters.ClientRectangle,
                Color.FromArgb(220, 220, 220), ButtonBorderStyle.Solid);
        }

        /// <summary>
        /// Xử lý khi chuyển tab "Giảng dạy" / "Chủ nhiệm".
        /// </summary>
        private void tabLoaiGiaoVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
            LoadClassListBasedOnTab();
            ClearReportData();
        }

        /// <summary>
        /// Xử lý khi chọn một lớp (load các môn học/tháng tương ứng).
        /// </summary>
        private void cboLop_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
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

            AutoLoadReport_Trigger(sender, e); // Tự động tải báo cáo
        }

        /// <summary>
        /// Xử lý khi chọn học kỳ (load các tháng tương ứng).
        /// </summary>
        private void cboHocKy_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            string reportType = GetSelectedReportType();
            if (reportType == "Báo cáo tháng")
            {
                LoadThangForSelectedHocKy();
            }
            AutoLoadReport_Trigger(sender, e); // Tự động tải báo cáo
        }

        /// <summary>
        /// Kích hoạt tải báo cáo khi một bộ lọc thay đổi (và đủ điều kiện).
        /// </summary>
        private void AutoLoadReport_Trigger(object sender, EventArgs e)
        {
            if (sender is ComboBox cbo && cbo.SelectedIndex == -1)
            {
                return; // Không tải nếu người dùng chưa chọn
            }

            string reportType = GetSelectedReportType();
            if (string.IsNullOrEmpty(reportType))
            {
                return;
            }

            // Kiểm tra điều kiện đủ
            if (reportType == "Bảng điểm học kỳ" && tabLoaiGiaoVien.SelectedTab == tabGiangDay)
            {
                if (cboLop.SelectedIndex == -1 || cboMonDay.SelectedIndex == -1 || cboHocKy.SelectedIndex == -1)
                {
                    ClearReportData(); return;
                }
            }
            else if (reportType == "Bảng điểm học kỳ" && tabLoaiGiaoVien.SelectedTab == tabChuNhiem)
            {
                if (cboLop.SelectedIndex == -1 || cboHocKy.SelectedIndex == -1)
                {
                    ClearReportData(); return;
                }
            }
            else if (reportType == "Báo cáo chuyên cần")
            {
                if (cboLop.SelectedIndex == -1 || cboHocKy.SelectedIndex == -1)
                {
                    ClearReportData(); return;
                }
            }
            else if (reportType == "Hồ sơ học sinh")
            {
                if (cboLop.SelectedIndex == -1)
                {
                    ClearReportData(); return;
                }
            }
            else if (reportType == "Báo cáo tháng")
            {
                if (cboLop.SelectedIndex == -1 || cboHocKy.SelectedIndex == -1 || cboMonDay.SelectedIndex == -1 || cboThang.SelectedIndex == -1)
                {
                    ClearReportData(); return;
                }
            }

            // Nếu đủ điều kiện, tải báo cáo
            LoadReportData();
        }

        #endregion

        #region Data Loading (Tải dữ liệu)

        /// <summary>
        /// Tải danh sách lớp dựa trên tab (Giảng dạy / Chủ nhiệm) đang chọn.
        /// </summary>
        private void LoadClassListBasedOnTab()
        {
            string reportType = GetSelectedReportType();
            if (string.IsNullOrEmpty(reportType)) return;

            bool isHomeroomTab = tabLoaiGiaoVien.SelectedTab == tabChuNhiem;
            DataTable dtLop = null;

            this.cboLop.SelectedIndexChanged -= new System.EventHandler(this.cboLop_SelectedIndexChanged_Handler);
            cboLop.DataSource = null;

            try
            {
                if (reportType == "Bảng điểm học kỳ")
                {
                    dtLop = isHomeroomTab ? DatabaseHelper.GetHomeroomClassesByTeacher(maGV) : DatabaseHelper.GetClassesByTeacher(maGV);
                }
                else
                {
                    // Các báo cáo còn lại lấy tất cả lớp GV có liên quan
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
                this.cboLop.SelectedIndexChanged += new System.EventHandler(this.cboLop_SelectedIndexChanged_Handler);
            }
        }

        /// <summary>
        /// Tải danh sách môn học mà GV dạy tại lớp đã chọn.
        /// </summary>
        private void LoadMonHocForSelectedLop()
        {
            var maLop = cboLop.SelectedValue?.ToString();

            this.cboMonDay.SelectedIndexChanged -= new System.EventHandler(this.AutoLoadReport_Trigger);
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

            this.cboMonDay.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
        }

        /// <summary>
        /// Tải danh sách các cột điểm tháng (VD: Thang1_Ki1) dựa trên lớp và học kỳ.
        /// </summary>
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
                this.cboThang.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            }
        }

        /// <summary>
        /// Lấy loại báo cáo đang được chọn từ RadioButton.
        /// </summary>
        private string GetSelectedReportType()
        {
            foreach (var rb in reportTypeRadioButtons)
            {
                if (rb.Checked)
                {
                    return rb.Text;
                }
            }
            return null;
        }

        /// <summary>
        /// Ẩn/hiện các control bộ lọc dựa trên loại báo cáo được chọn.
        /// </summary>
        private void UpdateControlsVisibility()
        {
            string reportType = GetSelectedReportType();

            // Ẩn tất cả bộ lọc tùy chọn
            tabLoaiGiaoVien.Visible = false;
            lblKhoi.Visible = cboKhoi.Visible = false;
            lblLop.Visible = cboLop.Visible = false;
            lblHocKy.Visible = cboHocKy.Visible = false;
            lblMonDay.Visible = cboMonDay.Visible = false;
            lblThang.Visible = cboThang.Visible = false;

            // Đặt lại (reset) các ComboBox
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

            // Hiển thị các bộ lọc cần thiết
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

        #region Report Generation (Tạo báo cáo)

        /// <summary>
        /// Tải dữ liệu báo cáo chính từ CSDL dựa trên các bộ lọc đã chọn.
        /// </summary>
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

        /// <summary>
        /// Xóa dữ liệu báo cáo hiện tại khỏi DataGridView và biểu đồ.
        /// </summary>
        private void ClearReportData()
        {
            if (lblNoDataMessage != null)
                lblNoDataMessage.Visible = false;
            if (dgvDuLieu != null)
                dgvDuLieu.Visible = true; // Đảm bảo DGV hiện lại

            dgvDuLieu.DataSource = null;
            // Dọn dẹp biểu đồ cũ
            foreach (Chart chart in flpCharts.Controls.OfType<Chart>().ToList())
            {
                chart.Dispose();
            }
            flpCharts.Controls.Clear();
            splitContainer1.Panel2Collapsed = true;
        }

        /// <summary>
        /// Hiển thị dữ liệu báo cáo lên DataGridView và tạo biểu đồ (nếu có).
        /// </summary>
        private void DisplayReportData(DataTable dtReport, string reportType)
        {
            // Kiểm tra chung xem có dữ liệu hay không
            bool hasData = (dtReport != null && dtReport.Rows.Count > 0);

            if (reportType == "Báo cáo tháng")
            {
                dgvDuLieu.ColumnHeadersVisible = !hasData; // Ẩn header nếu có data (vì data tự có header)
                ClearReportData(); // Xóa biểu đồ cũ

                if (hasData)
                {
                    lblNoDataMessage.Visible = false;
                    dgvDuLieu.Visible = true;
                    DisplayCombinedMonthlyReport(dtReport); // Hàm này gán DataSource
                    splitContainer1.Panel2Collapsed = true;
                }
                else
                {
                    // KHÔNG CÓ DỮ LIỆU
                    lblNoDataMessage.Visible = true;
                    dgvDuLieu.Visible = false; // Ẩn DGV
                    splitContainer1.Panel2Collapsed = true;
                }
            }
            else // Các loại báo cáo khác
            {
                dgvDuLieu.ColumnHeadersVisible = true;
                ClearReportData(); // Xóa biểu đồ cũ
                dgvDuLieu.DataSource = dtReport; // Gán data

                if (hasData)
                {
                    // CÓ DỮ LIỆU
                    lblNoDataMessage.Visible = false;
                    dgvDuLieu.Visible = true;
                    RenameDataGridViewColumns();
                    UpdateCharts(dtReport, reportType);
                    splitContainer1.Panel2Collapsed = flpCharts.Controls.Count == 0;
                }
                else
                {
                    // KHÔNG CÓ DỮ LIỆU
                    lblNoDataMessage.Visible = true;
                    dgvDuLieu.Visible = false; // Ẩn DGV
                    splitContainer1.Panel2Collapsed = true;
                }
            }
        }

        /// <summary>
        /// Xử lý và hiển thị dữ liệu cho "Báo cáo tháng" (gộp điểm và xếp loại).
        /// </summary>
        private void DisplayCombinedMonthlyReport(DataTable dtReport)
        {
            // 1. Dọn dẹp
            ClearReportData();
            splitContainer1.Panel2Collapsed = true; // Báo cáo này không dùng panel dưới

            // 2. Tách dữ liệu Điểm và Xếp loại
            var dataMapDiem = dtReport.Select("LoaiThongKe = 'Diem'")
                                      .ToDictionary(r => r["PhanLoai"].ToString(), r => r);
            var dataMapXepLoai = dtReport.Select("LoaiThongKe = 'XepLoai'")
                                         .ToDictionary(r => r["PhanLoai"].ToString(), r => r);

            // 3. Tạo DataTable mới để hiển thị
            DataTable dtCombined = new DataTable();
            dtCombined.Columns.Add("PhanLoai", typeof(string));
            dtCombined.Columns.Add("Col_TS", typeof(string));
            dtCombined.Columns.Add("Col_Nu", typeof(string));
            dtCombined.Columns.Add("Col_DanToc_Percent", typeof(string));
            dtCombined.Columns.Add("Col_NDT", typeof(string));
            dtCombined.Columns.Add("IsHeader", typeof(int)); // Cột cờ cho PDF

            // 4. Thêm hàng tiêu đề ĐIỂM
            dtCombined.Rows.Add("ĐIỂM", "TS", "Nữ", "Dân tộc", "NDT", 1);

            // 5. Thêm các hàng dữ liệu ĐIỂM
            string[] scoreOrder = { "10", "9", "8", "7", "6", "5", "Dưới 5" };
            foreach (string key in scoreOrder)
            {
                string ts = "0", nu = "0", dtoc = "0", ndt = "0";
                string dbKey = (key == "Dưới 5") ? "<5" : key; // Key trong CSDL là "<5"

                if (dataMapDiem.ContainsKey(dbKey))
                {
                    ts = dataMapDiem[dbKey]["TS"].ToString();
                    nu = dataMapDiem[dbKey]["Nu"].ToString();
                    dtoc = dataMapDiem[dbKey]["DanToc"].ToString();
                    ndt = dataMapDiem[dbKey]["NDT"].ToString();
                }
                dtCombined.Rows.Add(key, ts, nu, dtoc, ndt, 0);
            }

            // 6. Thêm hàng TỔNG CỘNG
            int totalTS = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["TS"]));
            int totalNu = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["Nu"]));
            int totalDanToc = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["DanToc"]));
            int totalNDT = dataMapDiem.Values.Sum(r => Convert.ToInt32(r["NDT"]));
            dtCombined.Rows.Add("Tổng", totalTS.ToString(), totalNu.ToString(), totalDanToc.ToString(), totalNDT.ToString(), 0);

            // 7. Thêm hàng tiêu đề XẾP LOẠI
            dtCombined.Rows.Add("XẾP LOẠI", "TS", "Nữ", "Dân tộc", "NDT", 1);

            // 8. Thêm các hàng dữ liệu XẾP LOẠI
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

            // 9. Hiển thị
            dgvDuLieu.DataSource = dtCombined;
            FormatCombinedMonthlyReportGrid(dgvDuLieu); // Định dạng lưới
        }

        /// <summary>
        /// Định dạng các cột và hàng cho lưới báo cáo tháng (gộp chung).
        /// </summary>
        private void FormatCombinedMonthlyReportGrid(DataGridView dgv)
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

            // 2. Định dạng hàng
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

        #endregion

        #region Charting (Vẽ biểu đồ)

        /// <summary>
        /// Tạo và hiển thị các biểu đồ tương ứng với loại báo cáo.
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
            }
        }

        /// <summary>
        /// Tạo một đối tượng Chart (biểu đồ) cơ sở với style chung.
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
        /// Vẽ biểu đồ tròn cho báo cáo chuyên cần.
        /// </summary>
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

            // Thêm dữ liệu vào series
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
        /// Vẽ biểu đồ cột phân loại học lực.
        /// </summary>
        private void DrawScoreDistributionChart(DataTable dt)
        {
            string scoreColumnName = "";
            if (dt.Columns.Contains("DiemTB"))
                scoreColumnName = "DiemTB";
            else if (dt.Columns.Contains("Trung bình chung"))
                scoreColumnName = "Trung bình chung";

            if (string.IsNullOrEmpty(scoreColumnName)) return;

            // Đếm số lượng
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

        #endregion

        #region Data Processing & Export

        /// <summary>
        /// Tính và thêm cột Điểm Trung Bình (DiemTB) vào DataTable (cho báo cáo GV Giảng Dạy).
        /// </summary>
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
                    row["DiemTB"] = Math.Round(totalScore / totalWeight, 2);
                }
                else
                {
                    row["DiemTB"] = DBNull.Value;
                }
            }
        }

        /// <summary>
        /// Đổi tên các cột trong DataGridView để thân thiện với người dùng.
        /// </summary>
        private void RenameDataGridViewColumns()
        {
            if (dgvDuLieu.DataSource == null) return;
            foreach (DataGridViewColumn col in dgvDuLieu.Columns)
            {
                col.Tag = col.DataPropertyName; // Lưu tên gốc

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
                            col.HeaderText = col.DataPropertyName; // Hiển thị tên môn học
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            col.DefaultCellStyle.Format = "N1";
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        else if (!col.Visible)
                        {
                            // Giữ ẩn
                        }
                        else
                        {
                            col.HeaderText = col.DataPropertyName;
                        }
                        break;
                }

                // Căn chỉnh cột
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                if (col.ValueType == typeof(int) || col.ValueType == typeof(long) || col.ValueType == typeof(double) || col.ValueType == typeof(decimal) || col.ValueType == typeof(float) || col.HeaderText.Contains("%"))
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else if (!string.IsNullOrEmpty(col.DefaultCellStyle.Format) && col.DefaultCellStyle.Format.Contains("dd/MM/yyyy"))
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Căn giữa ngày tháng
                }
                else
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút "Xuất Excel" (CSV).
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

        /// <summary>
        /// Xử lý sự kiện click nút "Xuất PDF".
        /// </summary>
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
                    // Tạo tiêu đề tài liệu (metadata)
                    string reportTypeTitle = reportType?.ToUpper() ?? "BÁO CÁO";
                    string documentTitle = reportTypeTitle;
                    documentTitle += $"\nLỚP: {cboLop.Text}";
                    if (cboHocKy.Visible && cboHocKy.SelectedIndex != -1) documentTitle += $" - {cboHocKy.Text.ToUpper()}";
                    if (cboMonDay.Visible && cboMonDay.SelectedIndex != -1) documentTitle += $"\nMÔN: {cboMonDay.Text}";
                    if (cboThang.Visible && cboThang.SelectedIndex != -1) documentTitle += $"\nTHÁNG: {cboThang.Text}";

                    // Lấy ảnh từ các biểu đồ
                    chartImages = flpCharts.Controls.OfType<Chart>().Select(chart =>
                    {
                        using (var ms = new MemoryStream())
                        {
                            chart.SaveImage(ms, ChartImageFormat.Png);
                            return (Image)Image.FromStream(ms).Clone();
                        }
                    }).ToList();

                    // Truyền vào ExportHelper
                    ExportHelper.ExportToPDF(dgvDuLieu, saveFileDialog.FileName, documentTitle, mainHeader, chartImages.ToArray());
                    MessageBox.Show($"Đã xuất báo cáo ra file:\n{saveFileDialog.FileName}", "Xuất PDF thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Dọn dẹp các ảnh đã clone
                    foreach (var img in chartImages)
                    {
                        img?.Dispose();
                    }
                }
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Gỡ bỏ sự kiện của các RadioButton động
                foreach (var rb in reportTypeRadioButtons)
                {
                    if (rb != null)
                    {
                        rb.CheckedChanged -= ReportType_CheckedChanged;
                    }
                }

                // Gỡ bỏ sự kiện của các control trong Designer
                if (this.tabLoaiGiaoVien != null) this.tabLoaiGiaoVien.SelectedIndexChanged -= new System.EventHandler(this.tabLoaiGiaoVien_SelectedIndexChanged);
                if (this.cboLop != null) this.cboLop.SelectedIndexChanged -= new System.EventHandler(this.cboLop_SelectedIndexChanged_Handler);
                if (this.cboKhoi != null) this.cboKhoi.SelectedIndexChanged -= new System.EventHandler(this.AutoLoadReport_Trigger);
                if (this.cboHocKy != null) this.cboHocKy.SelectedIndexChanged -= new System.EventHandler(this.cboHocKy_SelectedIndexChanged_Handler);
                if (this.cboMonDay != null) this.cboMonDay.SelectedIndexChanged -= new System.EventHandler(this.AutoLoadReport_Trigger);
                if (this.cboThang != null) this.cboThang.SelectedIndexChanged -= new System.EventHandler(this.AutoLoadReport_Trigger);
                if (this.btnXuatExcel != null) this.btnXuatExcel.Click -= new System.EventHandler(this.btnXuatExcel_Click);
                if (this.btnXuatPDF != null) this.btnXuatPDF.Click -= new System.EventHandler(this.btnXuatPDF_Click);
                if (this.pnlFilters != null) this.pnlFilters.Paint -= new System.Windows.Forms.PaintEventHandler(this.pnlFilters_Paint);
                

                // Dọn dẹp các control động (biểu đồ)
                if (flpCharts != null)
                {
                    foreach (Chart chart in flpCharts.Controls.OfType<Chart>().ToList())
                    {
                        chart.Dispose();
                    }
                    flpCharts.Controls.Clear();
                }

                if (components != null)
                {
                    components.Dispose();
                }

                if (lblNoDataMessage != null)
                {
                    lblNoDataMessage.Dispose();
                    lblNoDataMessage = null;
                }

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