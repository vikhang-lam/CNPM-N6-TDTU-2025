using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Integration
{
    [TestClass]
    public class AdminReportIntegrationTests
    {
        // Mô phỏng các SP sẽ được gọi
        private enum StoredProcedureCalled
        {
            None,
            GetAttendanceReport, // (Teacher)
            GetAttendanceReport_Admin,
            GetScoreboardPivot, // (Teacher)
            GetSemesterScoreboard, // (Teacher)
            GetSemesterScoreboard_Admin,
            GetStudentRecords, // (Teacher)
            GetStudentRecords_Admin,
            GetGradeStatistics_Admin,
            GetMonthlyReport_Statistics, // (Teacher)
            GetMonthlyReport_Statistics_Admin
        }

        /// <summary>
        /// Mô phỏng logic hàm LoadReportData() để kiểm tra SP nào sẽ được gọi.
        /// </summary>
        private StoredProcedureCalled Simulate_LoadReportData(
            string reportType, string selectedKhoi, string selectedMaLopValue, string selectedMaMon)
        {
            // 2. Chuẩn hóa giá trị tham số (như trong code thật)
            string khoiParam = (selectedKhoi == "Tất cả các khối") ? null : selectedKhoi;
            string maLopParam_Admin = (selectedMaLopValue == "ALL" || selectedMaLopValue == "ALL_KHOI") ? null : selectedMaLopValue;
            string maMonParam_Pivot = (selectedMaMon == "ALL") ? null : selectedMaMon;

            // 3. Biến kiểm tra
            bool isSpecificClass = !string.IsNullOrEmpty(selectedMaLopValue) &&
                                   selectedMaLopValue != "ALL" &&
                                   selectedMaLopValue != "ALL_KHOI";

            // 4. Gọi SP tương ứng
            switch (reportType)
            {
                case "Báo cáo chuyên cần":
                    if (isSpecificClass) return StoredProcedureCalled.GetAttendanceReport;
                    else return StoredProcedureCalled.GetAttendanceReport_Admin;

                case "Bảng điểm học kỳ":
                    if (!string.IsNullOrEmpty(maMonParam_Pivot))
                    {
                        if (isSpecificClass) return StoredProcedureCalled.GetScoreboardPivot;
                        else return StoredProcedureCalled.None; // (Code thật báo lỗi)
                    }
                    else
                    {
                        if (isSpecificClass) return StoredProcedureCalled.GetSemesterScoreboard;
                        else return StoredProcedureCalled.GetSemesterScoreboard_Admin;
                    }

                case "Hồ sơ học sinh":
                    if (isSpecificClass) return StoredProcedureCalled.GetStudentRecords;
                    else return StoredProcedureCalled.GetStudentRecords_Admin;

                case "Thống kê tổng hợp khối":
                    return StoredProcedureCalled.GetGradeStatistics_Admin;

                case "Báo cáo tháng":
                    if (isSpecificClass) return StoredProcedureCalled.GetMonthlyReport_Statistics;
                    else return StoredProcedureCalled.GetMonthlyReport_Statistics_Admin;
            }
            return StoredProcedureCalled.None;
        }

        [TestMethod]
        public void LoadReportData_Logic_WhenChuyenCanAndSpecificClass_ShouldUseTeacherSP()
        {
            // Sắp xếp: Chọn "Báo cáo chuyên cần" cho "Lớp 5A"
            var result = Simulate_LoadReportData("Báo cáo chuyên cần", "Khối 5", "L5A", null);
            // Khẳng định
            result.Should().Be(StoredProcedureCalled.GetAttendanceReport);
        }

        [TestMethod]
        public void LoadReportData_Logic_WhenChuyenCanAndKhoi_ShouldUseAdminSP()
        {
            // Sắp xếp: Chọn "Báo cáo chuyên cần" cho "Khối 5" (tức là cboLop = "ALL_KHOI")
            var result = Simulate_LoadReportData("Báo cáo chuyên cần", "Khối 5", "ALL_KHOI", null);
            // Khẳng định
            result.Should().Be(StoredProcedureCalled.GetAttendanceReport_Admin);
        }

        [TestMethod]
        public void LoadReportData_Logic_WhenBangDiemAndSpecificClassAndMon_ShouldUsePivotSP()
        {
            // Sắp xếp: "Bảng điểm", "Lớp 5A", "Môn Toán"
            var result = Simulate_LoadReportData("Bảng điểm học kỳ", "Khối 5", "L5A", "TOAN");
            // Khẳng định
            result.Should().Be(StoredProcedureCalled.GetScoreboardPivot);
        }

        [TestMethod]
        public void LoadReportData_Logic_WhenBangDiemAndSpecificClassAndAllMon_ShouldUseTeacherSP()
        {
            // Sắp xếp: "Bảng điểm", "Lớp 5A", "Tất cả các môn"
            var result = Simulate_LoadReportData("Bảng điểm học kỳ", "Khối 5", "L5A", "ALL");
            // Khẳng định
            result.Should().Be(StoredProcedureCalled.GetSemesterScoreboard);
        }

        [TestMethod]
        public void LoadReportData_Logic_WhenBangDiemAndKhoiAndAllMon_ShouldUseAdminSP()
        {
            // Sắp xếp: "Bảng điểm", "Khối 5" (ALL_KHOI), "Tất cả các môn"
            var result = Simulate_LoadReportData("Bảng điểm học kỳ", "Khối 5", "ALL_KHOI", "ALL");
            // Khẳng định
            result.Should().Be(StoredProcedureCalled.GetSemesterScoreboard_Admin);
        }

        [TestMethod]
        public void LoadReportData_Logic_WhenBangDiemAndKhoiAndSpecificMon_ShouldFail()
        {
            // Sắp xếp: "Bảng điểm", "Khối 5" (ALL_KHOI), "Môn Toán"
            // (Code thật sẽ báo lỗi vì Pivot cần lớp cụ thể)
            var result = Simulate_LoadReportData("Bảng điểm học kỳ", "Khối 5", "ALL_KHOI", "TOAN");
            // Khẳng định
            result.Should().Be(StoredProcedureCalled.None);
        }

        [TestMethod]
        public void LoadReportData_Logic_WhenBaoCaoThangAndKhoi_ShouldUseAdminSP()
        {
            // Sắp xếp: "Báo cáo tháng", "Khối 5" (ALL_KHOI)
            var result = Simulate_LoadReportData("Báo cáo tháng", "Khối 5", "ALL_KHOI", "TOAN");
            // Khẳng định
            result.Should().Be(StoredProcedureCalled.GetMonthlyReport_Statistics_Admin);
        }

        [TestMethod]
        public void LoadReportData_Logic_WhenBaoCaoThangAndSpecificClass_ShouldUseTeacherSP()
        {
            // Sắp xếp: "Báo cáo tháng", "Lớp 5A"
            var result = Simulate_LoadReportData("Báo cáo tháng", "Khối 5", "L5A", "TOAN");
            // Khẳng định
            result.Should().Be(StoredProcedureCalled.GetMonthlyReport_Statistics);
        }

        [TestMethod]
        public void LoadReportData_Logic_WhenThongKeKhoi_ShouldUseAdminSP()
        {
            // Sắp xếp: "Thống kê tổng hợp khối", "Khối 5"
            var result = Simulate_LoadReportData("Thống kê tổng hợp khối", "Khối 5", null, null);
            // Khẳng định
            result.Should().Be(StoredProcedureCalled.GetGradeStatistics_Admin);
        }
    }
}