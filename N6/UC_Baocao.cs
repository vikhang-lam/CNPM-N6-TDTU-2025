using System;
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

        public UC_BaoCao(string maGVien)
        {
            maGV = maGVien;
            InitializeComponent();
            LoadDuLieu();
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            this.cboLoaiBaoCao.SelectedIndexChanged += new System.EventHandler(this.cboLoaiBaoCao_SelectedIndexChanged);
            this.tabLoaiGiaoVien.SelectedIndexChanged += new System.EventHandler(this.tabLoaiGiaoVien_SelectedIndexChanged);
            this.cboLop.SelectedIndexChanged += new System.EventHandler(this.cboLop_SelectedIndexChanged_Handler);
            this.cboKhoi.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            this.cboHocKy.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            this.cboMonDay.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
        }

        private void LoadDuLieu()
        {
            cboKhoi.Items.Clear();
            cboKhoi.Items.AddRange(new object[] { "Khối 1", "Khối 2", "Khối 3", "Khối 4", "Khối 5", "Khối 6", "Khối 7", "Khối 8", "Khối 9" });
            btnXuatBaoCao.Visible = false;
            splitContainer1.Panel2Collapsed = true;
        }

        private void cboLoaiBaoCao_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
            dgvDuLieu.DataSource = null;
            splitContainer1.Panel2Collapsed = true;
        }

        private void tabLoaiGiaoVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
            LoadClassListBasedOnTab();
            dgvDuLieu.DataSource = null;
        }

        private void cboLop_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            string reportType = cboLoaiBaoCao.SelectedItem?.ToString();

            if (reportType == "Bảng điểm học kỳ" && tabLoaiGiaoVien.SelectedTab == tabGiangDay)
            {
                LoadMonHocForSelectedLop();
            }

            AutoLoadReport_Trigger(sender, e);
        }

        private void LoadMonHocForSelectedLop()
        {
            var maLop = cboLop.SelectedValue?.ToString();

            this.cboMonDay.SelectedIndexChanged -= new System.EventHandler(this.AutoLoadReport_Trigger);

            if (string.IsNullOrEmpty(maLop))
            {
                cboMonDay.DataSource = null;
                this.cboMonDay.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
                return;
            }

            try
            {
                DataTable dtMon = DatabaseHelper.GetMonHocByGiaoVienAndLop(maGV, maLop);
                cboMonDay.DataSource = dtMon;
                cboMonDay.DisplayMember = "TenMon";
                cboMonDay.ValueMember = "MaMon";

                if (dtMon == null || dtMon.Rows.Count == 0)
                {
                    cboMonDay.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách môn học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cboMonDay.DataSource = null;
            }
            finally
            {
                this.cboMonDay.SelectedIndexChanged += new System.EventHandler(this.AutoLoadReport_Trigger);
            }
        }

        private void LoadClassListBasedOnTab()
        {
            string reportType = cboLoaiBaoCao.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(reportType)) return;

            bool isHomeroomTab = tabLoaiGiaoVien.SelectedTab == tabChuNhiem;
            DataTable dtLop = null;

            this.cboLop.SelectedIndexChanged -= new System.EventHandler(this.cboLop_SelectedIndexChanged_Handler);

            try
            {
                if (reportType == "Bảng điểm học kỳ")
                {
                    dtLop = isHomeroomTab ? DatabaseHelper.GetHomeroomClassesByTeacher(maGV) : DatabaseHelper.GetLopByGiaoVien(maGV);
                }
                else
                {
                    dtLop = DatabaseHelper.GetLopByGiaoVien(maGV);
                }

                cboLop.DataSource = dtLop;
                cboLop.DisplayMember = "TenLop";
                cboLop.ValueMember = "MaLop";
                cboLop.SelectedIndex = -1;
                cboLop.Text = "Chọn lớp...";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.cboLop.SelectedIndexChanged += new System.EventHandler(this.cboLop_SelectedIndexChanged_Handler);
            }
        }

        private void UpdateControlsVisibility()
        {
            string reportType = cboLoaiBaoCao.SelectedItem?.ToString();

            tabLoaiGiaoVien.Visible = false;
            cboKhoi.Visible = false;
            cboLop.Visible = false;
            cboHocKy.Visible = false;
            lblMonDay.Visible = false;
            cboMonDay.Visible = false;

            cboLop.DataSource = null;
            cboMonDay.DataSource = null;

            switch (reportType)
            {
                case "Báo cáo chuyên cần":
                    LoadClassListBasedOnTab();
                    cboLop.Visible = true;
                    cboHocKy.Visible = true;
                    cboHocKy.Items.Clear();
                    cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2", "Cả năm" });
                    break;

                case "Bảng điểm học kỳ":
                    tabLoaiGiaoVien.Visible = true;
                    cboLop.Visible = true;
                    cboHocKy.Visible = true;
                    LoadClassListBasedOnTab();

                    bool isHomeroomTab = tabLoaiGiaoVien.SelectedTab == tabChuNhiem;

                    if (isHomeroomTab)
                    {
                        cboHocKy.Items.Clear();
                        cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2", "Cả năm" });
                    }
                    else
                    {
                        lblMonDay.Visible = true;
                        cboMonDay.Visible = true;
                        cboHocKy.Items.Clear();
                        cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2" });
                    }
                    break;

                case "Hồ sơ học sinh":
                    LoadClassListBasedOnTab();
                    cboLop.Visible = true;
                    break;

                case "Thống kê tổng hợp khối":
                    cboKhoi.Visible = true;
                    break;
            }
        }

        private void AutoLoadReport_Trigger(object sender, EventArgs e)
        {
            if (sender is ComboBox cbo && cbo.SelectedIndex == -1) return;

            string reportType = cboLoaiBaoCao.SelectedItem?.ToString();
            if (reportType == "Bảng điểm học kỳ" && tabLoaiGiaoVien.SelectedTab == tabGiangDay)
            {
                if (cboMonDay.SelectedIndex == -1) return;
            }

            btnXuatBaoCao_Click(null, null);
        }

        private void btnXuatBaoCao_Click(object sender, EventArgs e)
        {
            if (cboLoaiBaoCao.SelectedItem == null) return;
            string reportType = cboLoaiBaoCao.SelectedItem.ToString();
            DataTable dtReport = null;
            try
            {
                switch (reportType)
                {
                    case "Báo cáo chuyên cần":
                        var maLop1 = cboLop.SelectedValue?.ToString();
                        if (string.IsNullOrEmpty(maLop1) || cboHocKy.SelectedItem == null) return;
                        int hocKy1 = cboHocKy.SelectedIndex + 1;
                        dtReport = DatabaseHelper.GetBaoCaoChuyenCan(maLop1, hocKy1);
                        break;
                    case "Bảng điểm học kỳ":
                        var maLop2 = cboLop.SelectedValue?.ToString();
                        if (string.IsNullOrEmpty(maLop2) || cboHocKy.SelectedItem == null) return;
                        if (tabLoaiGiaoVien.SelectedTab == tabChuNhiem)
                        {
                            int hocKy2_cn = cboHocKy.SelectedIndex + 1;
                            dtReport = DatabaseHelper.GetBangDiemHocKy(maLop2, hocKy2_cn);
                        }
                        else
                        {
                            var maMon = cboMonDay.SelectedValue?.ToString();
                            if (string.IsNullOrEmpty(maMon)) return;
                            int hocKy2_bm = cboHocKy.SelectedIndex + 1;
                            dtReport = DatabaseHelper.GetBangDiemPivot(maLop2, hocKy2_bm, maMon);
                            CalculateAndAddAverageColumn(dtReport);
                        }
                        break;
                    case "Hồ sơ học sinh":
                        var maLop3 = cboLop.SelectedValue?.ToString();
                        if (string.IsNullOrEmpty(maLop3)) return;
                        dtReport = DatabaseHelper.GetHoSoHocSinh(maLop3);
                        break;
                    case "Thống kê tổng hợp khối":
                        var khoi = cboKhoi.SelectedItem?.ToString();
                        if (string.IsNullOrEmpty(khoi)) return;
                        dtReport = DatabaseHelper.GetThongKeKhoi(khoi);
                        break;
                }
                DisplayReportData(dtReport, reportType);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayReportData(DataTable dtReport, string reportType)
        {
            dgvDuLieu.DataSource = dtReport;
            pnlCharts.Controls.Clear();
            if (dtReport != null && dtReport.Rows.Count > 0)
            {
                RenameDataGridViewColumns();
                UpdateCharts(dtReport, reportType);
            }
            else
            {
                splitContainer1.Panel2Collapsed = true;
            }
        }

        private void UpdateCharts(DataTable dt, string reportType)
        {
            splitContainer1.Panel2Collapsed = false;
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
                default:
                    splitContainer1.Panel2Collapsed = true;
                    break;
            }
        }

        // THAY THẾ HÀM NÀY ĐỂ SỬA DỨT ĐIỂM LỖI TÍNH %
        private void DrawAttendancePieChart(DataTable dt)
        {
            // 1. Tổng hợp dữ liệu thô (logic này vẫn đúng)
            long totalPresent = dt.AsEnumerable().Sum(row => Convert.ToInt64(row["SoBuoiCoMat"]));
            long totalAbsent = dt.AsEnumerable().Sum(row => Convert.ToInt64(row["SoBuoiVang"]));
            long totalExcused = dt.AsEnumerable().Sum(row => Convert.ToInt64(row["SoBuoiVangCoPhep"]));

            long totalAll = totalPresent + totalAbsent + totalExcused;
            if (totalAll == 0) return;

            // 2. Cấu hình biểu đồ
            var chart = new Chart { Size = new Size(400, 200) };
            chart.Titles.Add("Biểu đồ tỷ lệ chuyên cần");
            var chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);
            var series = new Series("Chuyên cần")
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true
                // Bỏ đi LabelFormat để chúng ta tự định dạng nhãn
            };

            // 3. Thêm dữ liệu và TỰ TÍNH TOÁN % CHO NHÃN
            // Dữ liệu thêm vào vẫn là SỐ LƯỢNG THÔ để biểu đồ chia kích thước lát cắt
            // Nhưng phần nhãn (Label) sẽ được gán bằng TỶ LỆ % ĐÃ TÍNH

            // Điểm dữ liệu "Có mặt"
            double percentPresent = (double)totalPresent / totalAll;
            series.Points.Add(totalPresent);
            series.Points.Last().Color = Color.Green;
            series.Points.Last().LegendText = $"Có mặt ({totalPresent})";
            series.Points.Last().Label = percentPresent.ToString("P1"); // Gán nhãn thủ công

            // Điểm dữ liệu "Vắng"
            double percentAbsent = (double)totalAbsent / totalAll;
            series.Points.Add(totalAbsent);
            series.Points.Last().Color = Color.Red;
            series.Points.Last().LegendText = $"Vắng ({totalAbsent})";
            series.Points.Last().Label = percentAbsent.ToString("P1"); // Gán nhãn thủ công

            // Điểm dữ liệu "Vắng có phép"
            double percentExcused = (double)totalExcused / totalAll;
            series.Points.Add(totalExcused);
            series.Points.Last().Color = Color.DodgerBlue;
            series.Points.Last().LegendText = $"Vắng có phép ({totalExcused})";
            series.Points.Last().Label = percentExcused.ToString("P1"); // Gán nhãn thủ công

            chart.Series.Add(series);
            chart.Legends.Add(new Legend("Default") { Docking = Docking.Right });
            pnlCharts.Controls.Add(chart);
        }

        private void DrawScoreDistributionChart(DataTable dt)
        {
            string scoreColumnName = "";
            if (tabLoaiGiaoVien.SelectedTab == tabChuNhiem && dt.Columns.Contains("Trung bình chung"))
                scoreColumnName = "Trung bình chung";
            else if (tabLoaiGiaoVien.SelectedTab == tabGiangDay && dt.Columns.Contains("DiemTB"))
                scoreColumnName = "DiemTB";
            if (string.IsNullOrEmpty(scoreColumnName)) return;

            var scoreCounts = new int[11];
            foreach (DataRow row in dt.Rows)
            {
                if (row[scoreColumnName] != DBNull.Value)
                {
                    int score = (int)Math.Round(Convert.ToDouble(row[scoreColumnName]));
                    if (score >= 0 && score <= 10)
                    {
                        scoreCounts[score]++;
                    }
                }
            }

            var chart = new Chart { Size = new Size(400, 200) };
            chart.Titles.Add($"Phổ điểm ({cboMonDay.Text})");
            var chartArea = new ChartArea();
            chartArea.AxisX.Title = "Điểm";
            chartArea.AxisY.Title = "Số lượng học sinh";
            chartArea.AxisX.Interval = 1;
            chart.ChartAreas.Add(chartArea);
            var series = new Series("Phổ điểm") { ChartType = SeriesChartType.Column };
            for (int i = 0; i <= 10; i++)
            {
                series.Points.AddXY(i, scoreCounts[i]);
            }
            chart.Series.Add(series);
            pnlCharts.Controls.Add(chart);
        }

        private void DrawStudentCountChart(DataTable dt)
        {
            var chart = new Chart { Size = new Size(400, 200) };
            chart.Titles.Add("Thống kê sĩ số theo lớp");
            var chartArea = new ChartArea();
            chartArea.AxisX.Title = "Lớp";
            chartArea.AxisY.Title = "Sĩ số";
            chart.ChartAreas.Add(chartArea);
            var series = new Series("Sĩ số") { ChartType = SeriesChartType.Column };
            foreach (DataRow row in dt.Rows)
            {
                series.Points.AddXY(row["TenLop"].ToString(), Convert.ToInt32(row["SoHocSinh"]));
            }
            chart.Series.Add(series);
            pnlCharts.Controls.Add(chart);
        }

        private void DrawAverageScoreChart(DataTable dt)
        {
            var chart = new Chart { Size = new Size(400, 200) };
            chart.Titles.Add("Thống kê điểm trung bình theo lớp");
            var chartArea = new ChartArea();
            chartArea.AxisX.Title = "Lớp";
            chartArea.AxisY.Title = "Điểm trung bình";
            chartArea.AxisY.Maximum = 10;
            chart.ChartAreas.Add(chartArea);

            var series = new Series("Điểm TB")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true // Hiển thị giá trị trên cột
            };

            foreach (DataRow row in dt.Rows)
            {
                // Thêm kiểm tra DBNull trước khi chuyển đổi
                if (row["DiemTrungBinh"] != DBNull.Value)
                {
                    series.Points.AddXY(row["TenLop"].ToString(), Convert.ToDouble(row["DiemTrungBinh"]));
                }
            }

            chart.Series.Add(series);
            pnlCharts.Controls.Add(chart);
        }

        private void CalculateAndAddAverageColumn(DataTable dt)
        {
            if (dt == null) return;
            if (!dt.Columns.Contains("DiemTB"))
            {
                dt.Columns.Add("DiemTB", typeof(double));
            }
            foreach (DataRow row in dt.Rows)
            {
                double totalScore = 0;
                int totalWeight = 0;
                for (int i = 1; i <= 3; i++)
                {
                    if (row[$"Thang{i}"] != DBNull.Value)
                    {
                        totalScore += Convert.ToDouble(row[$"Thang{i}"]) * 1;
                        totalWeight += 1;
                    }
                }
                if (row["GiuaKi"] != DBNull.Value)
                {
                    totalScore += Convert.ToDouble(row["GiuaKi"]) * 2;
                    totalWeight += 2;
                }
                if (row["CuoiKi"] != DBNull.Value)
                {
                    totalScore += Convert.ToDouble(row["CuoiKi"]) * 3;
                    totalWeight += 3;
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
                switch (col.Name)
                {
                    case "MaHS": col.HeaderText = "Mã HS"; break;
                    case "HoTen": col.HeaderText = "Họ Tên"; break;
                    case "TenLop": col.HeaderText = "Lớp"; break;
                    case "SoBuoiCoMat": col.HeaderText = "Có Mặt"; break;
                    case "SoBuoiVang": col.HeaderText = "Vắng"; break;
                    case "SoBuoiVangCoPhep": col.HeaderText = "Có Phép"; break;
                    case "TongSoBuoi": col.HeaderText = "Tổng Buổi"; break;
                    case "TyLeChuyenCan": col.HeaderText = "Tỷ Lệ CC (%)"; break;
                    case "Thang1": col.HeaderText = "Tháng 1"; break;
                    case "Thang2": col.HeaderText = "Tháng 2"; break;
                    case "Thang3": col.HeaderText = "Tháng 3"; break;
                    case "GiuaKi": col.HeaderText = "Giữa Kỳ"; break;
                    case "CuoiKi": col.HeaderText = "Cuối Kỳ"; break;
                    case "DiemTB": col.HeaderText = "Điểm TB"; break;
                    case "Trung bình chung": col.HeaderText = "TB Chung"; break;
                    case "NhanXet": col.HeaderText = "Nhận Xét"; break;
                    case "GhiChu": col.HeaderText = "Ghi Chú"; break;
                    case "GioiTinh": col.HeaderText = "Giới Tính"; break;
                    case "NgaySinh": col.HeaderText = "Ngày Sinh"; break;
                    case "DanToc": col.HeaderText = "Dân Tộc"; break;
                    case "DiaChi": col.HeaderText = "Địa Chỉ"; break;
                    case "SDTPhuHuynh": col.HeaderText = "SĐT PH"; break;
                    case "SoHocSinh": col.HeaderText = "Sĩ Số"; break;
                    case "DiemTrungBinh": col.HeaderText = "Điểm TB Khối"; break;
                    case "SoNam": col.HeaderText = "Số Nam"; break;
                    case "SoNu": col.HeaderText = "Số Nữ"; break;
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
                FileName = $"BaoCao_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExportHelper.ExportToExcel(dgvDuLieu, saveFileDialog.FileName);
            }
        }

        // THAY THẾ HÀM XUẤT PDF CŨ
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
                FileName = $"BaoCao_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string reportTitle = cboLoaiBaoCao.SelectedItem?.ToString().ToUpper() ?? "BÁO CÁO";

                    // Chụp ảnh các biểu đồ đang hiển thị
                    var chartImages = pnlCharts.Controls.OfType<Chart>().Select(chart =>
                    {
                        using (var ms = new MemoryStream())
                        {
                            chart.SaveImage(ms, ChartImageFormat.Png);
                            return Image.FromStream(ms);
                        }
                    }).ToArray();

                    // Gọi hàm ExportToPDF đã được nâng cấp
                    ExportHelper.ExportToPDF(dgvDuLieu, saveFileDialog.FileName, reportTitle, chartImages);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}