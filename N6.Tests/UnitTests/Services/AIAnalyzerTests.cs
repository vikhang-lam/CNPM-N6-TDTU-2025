using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;
using System.Data;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class AIAnalyzerTests
    {
        [TestMethod]
        public void TimHocSinhDiemThap_WithMixedData_ShouldReturnOnlyLowScoreStudents()
        {
            // Sắp xếp
            var scores = AIAnalysisTestHelper.CreateMockScoresData();
            double nguongDiem = 5.0;

            // Hành động
            var result = AIAnalyzer.TimHocSinhDiemThap(scores, nguongDiem);

            // Khẳng định
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].HoTen.Should().Be("Trần Thị B");
        }

        [TestMethod]
        public void TimHocSinhKhenThuong_WithMixedData_ShouldReturnOnlyHighScoreStudents()
        {
            // Sắp xếp
            var scores = AIAnalysisTestHelper.CreateMockScoresData();
            double nguongDiem = 8.5;

            // Hành động
            var result = AIAnalyzer.TimHocSinhKhenThuong(scores, nguongDiem);

            // Khẳng định
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].HoTen.Should().Be("Nguyễn Văn A");
        }

        [TestMethod]
        public void TimHocSinhDiemThatThuong_WithMixedData_ShouldReturnOnlyUnstableStudents()
        {
            // Sắp xếp
            var scores = AIAnalysisTestHelper.CreateMockScoresData();
            double nguongBienDong = 2.0;

            // Hành động
            var result = AIAnalyzer.TimHocSinhDiemThatThuong(scores, nguongBienDong);

            // Khẳng định
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].HoTen.Should().Be("Lê Văn C");
            result[0].LyDo.Should().Contain("Biến động lớn");
        }

        [TestMethod]
        public void TimHocSinhDiemThap_WithEmptyData_ShouldReturnEmptyList()
        {
            // Sắp xếp
            var scores = AIAnalysisTestHelper.CreateEmptyScoresData();
            double nguongDiem = 5.0;

            // Hành động
            var result = AIAnalyzer.TimHocSinhDiemThap(scores, nguongDiem);

            // Khẳng định
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void DuDoanHocSinhNguyCo_WhenModelFileMissing_ShouldReturnEmptyList()
        {
            // Sắp xếp
            // Giả định rằng tệp .onnx không tồn tại trong thư mục chạy test
            var predictionData = AIAnalysisTestHelper.CreateMockPredictionData();

            // Hành động
            // Hàm này sẽ cố gắng tải tệp .onnx và thất bại (ném exception hoặc trả về rỗng)
            var result = AIAnalyzer.DuDoanHocSinhNguyCo(predictionData, "Toán");

            // Khẳng định
            // Trong file AIAnalyzer.cs, nếu không tìm thấy model, nó sẽ return ds rỗng.
            result.Should().NotBeNull();
            result.Should().BeEmpty("vì tệp 'student_g3_predictor.onnx' không có trong thư mục test");
        }

        // LƯU Ý: Không thể test logic dự đoán ONNX thành công
        // nếu không có file "student_g3_predictor.onnx" thật đi kèm.
        // Test case trên (WhenModelFileMissing) là kịch bản an toàn nhất.
    }
}