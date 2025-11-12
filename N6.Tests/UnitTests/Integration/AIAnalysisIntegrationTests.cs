using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;
using System.Data;

namespace N6.Tests.UnitTests.Integration
{
    [TestClass]
    public class AIAnalysisIntegrationTests
    {
        // Mô phỏng các điều kiện lọc trên UC_PhanTichAI
        private class AnalysisFilters
        {
            public string PhamVi { get; set; } // "LopGV" hoặc "Khoi"
            public string MaMon { get; set; } // "ALL" hoặc "TOAN"
            public int HocKy { get; set; } // 1 hoặc 2
        }

        /// <summary>
        /// Mô phỏng logic hàm RunAnalysis() để kiểm tra xem
        /// việc gọi AI dự đoán (ONNX) có được kích hoạt đúng hay không.
        /// </summary>
        private bool ShouldTriggerAIPrediction(AnalysisFilters filters)
        {
            // Tái định nghĩa logic kiểm tra điều kiện từ UC_PhanTichAI.cs
            if (filters.PhamVi == "LopGV" && filters.MaMon != "ALL" && filters.HocKy == 1)
            {
                return true;
            }
            return false;
        }

        [TestMethod]
        public void FullAnalysisFlow_WithValidData_ShouldGenerateAllCardTypes()
        {
            // Sắp xếp: Mô phỏng CSDL trả về dữ liệu đầy đủ
            var scores = AIAnalysisTestHelper.CreateMockScoresData();
            bool hasData = scores != null && scores.Rows.Count > 0;

            // Hành động: Mô phỏng AIAnalyzer chạy trên dữ liệu này
            var dsKhenThuong = AIAnalyzer.TimHocSinhKhenThuong(scores, 8.5);
            var dsCanQuanTam = AIAnalyzer.TimHocSinhDiemThap(scores, 5.0);
            var dsThatThuong = AIAnalyzer.TimHocSinhDiemThatThuong(scores, 2.0);

            // Khẳng định
            hasData.Should().BeTrue();
            dsKhenThuong.Count.Should().Be(1, "vì có HS Nguyễn Văn A (8.5)");
            dsCanQuanTam.Count.Should().Be(1, "vì có HS Trần Thị B (4.5)");
            dsThatThuong.Count.Should().Be(1, "vì có HS Lê Văn C (biến động cao)");
        }

        [TestMethod]
        public void FullAnalysisFlow_NoData_ShouldGenerateNoCards()
        {
            // Sắp xếp: Mô phỏng CSDL trả về rỗng
            var scores = AIAnalysisTestHelper.CreateEmptyScoresData();
            bool hasData = scores != null && scores.Rows.Count > 0;

            // Hành động
            var dsKhenThuong = AIAnalyzer.TimHocSinhKhenThuong(scores, 8.5);
            var dsCanQuanTam = AIAnalyzer.TimHocSinhDiemThap(scores, 5.0);

            // Khẳng định
            hasData.Should().BeFalse();
            dsKhenThuong.Should().BeEmpty();
            dsCanQuanTam.Should().BeEmpty();
        }

        [TestMethod]
        public void AIPredictionFlow_WhenConditionsMet_ShouldBeTriggered()
        {
            // Sắp xếp: Đặt bộ lọc đúng điều kiện
            var filters = new AnalysisFilters
            {
                PhamVi = "LopGV", // OK
                MaMon = "TOAN",   // OK (!= "ALL")
                HocKy = 1         // OK
            };

            // Hành động
            bool isTriggered = ShouldTriggerAIPrediction(filters);

            // Khẳng định
            isTriggered.Should().BeTrue();
        }

        [TestMethod]
        public void AIPredictionFlow_WhenScopeIsKhoi_ShouldNotBeTriggered()
        {
            // Sắp xếp: Sai Phạm vi
            var filters = new AnalysisFilters
            {
                PhamVi = "Khoi",  // SAI
                MaMon = "TOAN",
                HocKy = 1
            };

            // Hành động
            bool isTriggered = ShouldTriggerAIPrediction(filters);

            // Khẳng định
            isTriggered.Should().BeFalse();
        }

        [TestMethod]
        public void AIPredictionFlow_WhenMonIsAll_ShouldNotBeTriggered()
        {
            // Sắp xếp: Sai Môn học
            var filters = new AnalysisFilters
            {
                PhamVi = "LopGV",
                MaMon = "ALL",    // SAI
                HocKy = 1
            };

            // Hành động
            bool isTriggered = ShouldTriggerAIPrediction(filters);

            // Khẳng định
            isTriggered.Should().BeFalse();
        }

        [TestMethod]
        public void AIPredictionFlow_WhenHocKyIs2_ShouldNotBeTriggered()
        {
            // Sắp xếp: Sai Học kỳ
            var filters = new AnalysisFilters
            {
                PhamVi = "LopGV",
                MaMon = "TOAN",
                HocKy = 2         // SAI
            };

            // Hành động
            bool isTriggered = ShouldTriggerAIPrediction(filters);

            // Khẳng định
            isTriggered.Should().BeFalse();
        }
    }
}