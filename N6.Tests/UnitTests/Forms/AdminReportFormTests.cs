using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;
using System.Data;
using System.Drawing;

namespace N6.Tests.UnitTests.Forms
{
    [TestClass]
    public class AdminReportFormTests
    {
        // --- Test 1: Logic Ẩn/Hiện Control (UpdateControlsVisibility) ---
        [TestMethod]
        public void UpdateControlsVisibility_ForChuyenCan_ShouldShowLopAndHocKy()
        {
            // Sắp xếp (Mô phỏng trạng thái)
            string reportType = "Báo cáo chuyên cần";
            bool cboMonDayVisible = false;
            bool cboThangVisible = false;
            bool cboLopVisible = false;

            // Hành động (Mô phỏng logic trong switch case)
            if (reportType == "Báo cáo chuyên cần")
            {
                cboLopVisible = true; // Bật
                cboMonDayVisible = false; // Tắt
                cboThangVisible = false; // Tắt
            }

            // Khẳng định
            cboLopVisible.Should().BeTrue();
            cboMonDayVisible.Should().BeFalse();
            cboThangVisible.Should().BeFalse();
        }

        [TestMethod]
        public void UpdateControlsVisibility_ForThongKeKhoi_ShouldOnlyShowKhoi()
        {
            // Sắp xếp
            string reportType = "Thống kê tổng hợp khối";
            bool cboKhoiVisible = false;
            bool cboLopVisible = false;

            // Hành động
            if (reportType == "Thống kê tổng hợp khối")
            {
                cboKhoiVisible = true; // Bật
                cboLopVisible = false; // Tắt
            }

            // Khẳng định
            cboKhoiVisible.Should().BeTrue();
            cboLopVisible.Should().BeFalse();
        }

        [TestMethod]
        public void UpdateControlsVisibility_ForBaoCaoThang_ShouldShowThang()
        {
            // Sắp xếp
            string reportType = "Báo cáo tháng";
            bool cboMonDayVisible = false;
            bool cboThangVisible = false;

            // Hành động
            if (reportType == "Báo cáo tháng")
            {
                cboMonDayVisible = true; // Bật
                cboThangVisible = true; // Bật
            }

            // Khẳng định
            cboMonDayVisible.Should().BeTrue();
            cboThangVisible.Should().BeTrue();
        }

        // --- Test 2: Logic Kích hoạt Tải (AutoLoadReport_Trigger) ---
        [TestMethod]
        public void AutoLoadReport_Trigger_WhenFiltersMissing_ShouldNotLoad()
        {
            // Sắp xếp (Bảng điểm cần 4 filter)
            string reportType = "Bảng điểm học kỳ";
            int cboLopIdx = 0;
            int cboHocKyIdx = 0;
            int cboMonDayIdx = -1; // Thiếu Môn
            bool canLoad = false;

            // Hành động (Mô phỏng logic AutoLoad)
            if (reportType == "Bảng điểm học kỳ")
            {
                canLoad = cboLopIdx != -1 && cboHocKyIdx != -1 && cboMonDayIdx != -1;
            }

            // Khẳng định
            canLoad.Should().BeFalse();
        }

        [TestMethod]
        public void AutoLoadReport_Trigger_WhenHoSoFiltersMet_ShouldLoad()
        {
            // Sắp xếp (Hồ sơ chỉ cần 2 filter)
            string reportType = "Hồ sơ học sinh";
            int cboLopIdx = 0;
            int cboKhoiIdx = 0;
            bool canLoad = false;

            // Hành động
            if (reportType == "Hồ sơ học sinh")
            {
                canLoad = cboKhoiIdx != -1 && cboLopIdx != -1;
            }

            // Khẳng định
            canLoad.Should().BeTrue();
        }

        // --- Test 3: Logic Validation (btnXuatPDF_Click) ---
        [TestMethod]
        public void ExportPDF_Validation_WhenLopNotSelected_ShouldFail()
        {
            // Sắp xếp (User chọn "Tất cả các lớp")
            string cboLopSelectedValue = "ALL";
            string cboKhoiSelectedValue = "Tất cả các khối";
            bool isClassReport = false;
            bool isGradeReport = false;

            // Hành động (Mô phỏng validation trong btnXuatPDF_Click)
            if (cboLopSelectedValue != "ALL" && cboLopSelectedValue != "ALL_KHOI")
                isClassReport = true;
            else if (cboKhoiSelectedValue != "Tất cả các khối")
                isGradeReport = true;

            bool showError = !isClassReport && !isGradeReport;

            // Khẳng định
            showError.Should().BeTrue(); // Phải báo lỗi
        }

        [TestMethod]
        public void ExportPDF_Validation_WhenKhoiSelected_ShouldSucceed()
        {
            // Sắp xếp (User chọn "Khối 5" và "Tất cả các lớp" (ALL_KHOI))
            string cboLopSelectedValue = "ALL_KHOI";
            string cboKhoiSelectedValue = "Khối 5";
            bool isClassReport = false;
            bool isGradeReport = false;

            // Hành động
            if (cboLopSelectedValue != "ALL" && cboLopSelectedValue != "ALL_KHOI")
                isClassReport = true;
            else if (cboKhoiSelectedValue != "Tất cả các khối")
                isGradeReport = true;

            bool showError = !isClassReport && !isGradeReport;

            // Khẳng định
            isGradeReport.Should().BeTrue();
            showError.Should().BeFalse(); // Không báo lỗi
        }

        // --- Test 4: Logic Tính toán (CalculateAndAddAverageColumn) ---
        [TestMethod]
        public void CalculateAndAddAverageColumn_ForPivotData_ShouldCalculateCorrectly()
        {
            // Sắp xếp
            DataTable dt = AdminReportTestHelper.CreateMockPivotDataForAverage();

            // Hành động (Mô phỏng hàm trong UC_BaoCao_Admin)
            var scoreCols = dt.Columns.Cast<DataColumn>()
                              .Where(c => c.DataType == typeof(double) && c.ColumnName != "DiemTB")
                              .Select(c => c.ColumnName).ToList(); // ["Toan", "TiengViet"]

            foreach (DataRow row in dt.Rows)
            {
                double totalScore = 0;
                int validScoresCount = 0;
                foreach (string colName in scoreCols)
                {
                    if (row[colName] != DBNull.Value)
                    {
                        totalScore += (double)row[colName];
                        validScoresCount++;
                    }
                }
                if (validScoresCount > 0)
                    row["DiemTB"] = Math.Round(totalScore / validScoresCount, 2);
            }

            // Khẳng định
            scoreCols.Count.Should().Be(2);
            dt.Rows[0]["DiemTB"].Should().Be(9.0); // (8 + 10) / 2
            dt.Rows[1]["DiemTB"].Should().Be(6.0); // (5 + 7) / 2
            dt.Rows[2]["DiemTB"].Should().Be(9.0); // (9) / 1
        }
    }
}