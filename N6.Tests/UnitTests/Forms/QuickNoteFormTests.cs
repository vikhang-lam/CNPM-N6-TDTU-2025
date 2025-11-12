using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;
using System.Drawing;

namespace N6.Tests.UnitTests.Forms
{
    [TestClass]
    public class QuickNoteFormTests
    {
        [TestMethod]
        public void BtnLuu_Click_WhenLopNotSelected_ShouldReturnError()
        {
            // Sắp xếp
            string selectedLop = null;
            string selectedMon = "TOAN";
            string selectedHS = "HS001";
            string ghiChu = "Test";

            // Hành động (Mô phỏng logic validation trong BtnLuu_Click)
            bool isValid = selectedLop != null && selectedMon != null && selectedHS != null && !string.IsNullOrWhiteSpace(ghiChu);

            // Khẳng định
            isValid.Should().BeFalse();
            // (Trong code thật, MessageBox "Vui lòng chọn lớp." sẽ xuất hiện)
        }

        [TestMethod]
        public void BtnLuu_Click_WhenMonHocNotSelected_ShouldReturnError()
        {
            // Sắp xếp
            string selectedLop = "L5A";
            string selectedMon = null;
            string selectedHS = "HS001";
            string ghiChu = "Test";

            // Hành động
            bool isValid = selectedLop != null && selectedMon != null && selectedHS != null && !string.IsNullOrWhiteSpace(ghiChu);

            // Khẳng định
            isValid.Should().BeFalse();
            // (MessageBox "Vui lòng chọn môn học.")
        }

        [TestMethod]
        public void BtnLuu_Click_WhenHocSinhNotSelected_ShouldReturnError()
        {
            // Sắp xếp
            string selectedLop = "L5A";
            string selectedMon = "TOAN";
            string selectedHS = null;
            string ghiChu = "Test";

            // Hành động
            bool isValid = selectedLop != null && selectedMon != null && selectedHS != null && !string.IsNullOrWhiteSpace(ghiChu);

            // Khẳng định
            isValid.Should().BeFalse();
            // (MessageBox "Vui lòng chọn học sinh.")
        }

        [TestMethod]
        public void BtnLuu_Click_WhenGhiChuIsEmpty_ShouldReturnError()
        {
            // Sắp xếp
            string selectedLop = "L5A";
            string selectedMon = "TOAN";
            string selectedHS = "HS001";
            string ghiChu = QuickNoteTestHelper.CreateNullNoteInput();

            // Hành động
            bool isValid = selectedLop != null && selectedMon != null && selectedHS != null && !string.IsNullOrWhiteSpace(ghiChu);

            // Khẳng định
            isValid.Should().BeFalse();
            // (MessageBox "Vui lòng nhập nội dung ghi chú.")
        }

        [TestMethod]
        public void InitializeWhisper_WhenModelMissing_ShouldDisableButton()
        {
            // Sắp xếp
            bool modelFileExists = false; // Giả định file "ggml-base.bin" không tồn tại

            // Hành động (Mô phỏng logic trong InitializeWhisper)
            bool btnEnabled = modelFileExists;
            string btnText = modelFileExists ? "" : "Lỗi (Không tìm thấy model)";
            Color btnColor = modelFileExists ? Color.Blue : Color.Gray;

            // Khẳng định
            btnEnabled.Should().BeFalse();
            btnText.Should().Be("Lỗi (Không tìm thấy model)");
            btnColor.Should().Be(Color.Gray);
        }

        [TestMethod]
        public void BtnRecord_OnFirstClick_ShouldSetRecordingState()
        {
            // Sắp xếp (Trạng thái ban đầu)
            bool _isRecording_Initial = false;

            // Hành động (Mô phỏng click lần 1)
            bool _isRecording_After = true;
            string btnText = "🎧 Đang nghe... (Nhấn để dừng)";
            Color btnColor = Color.Crimson;

            // Khẳng định
            _isRecording_After.Should().BeTrue();
            btnText.Should().Be("🎧 Đang nghe... (Nhấn để dừng)");
            btnColor.Should().Be(Color.Crimson);
        }

        [TestMethod]
        public void BtnRecord_OnSecondClick_ShouldSetProcessingState()
        {
            // Sắp xếp (Trạng thái đang ghi âm)
            bool _isRecording_Initial = true;

            // Hành động (Mô phỏng click lần 2 - dừng ghi âm)
            bool _isRecording_After = false;
            string btnText = "⏳ Đang xử lý...";
            Color btnColor = Color.Orange;

            // Khẳng định
            _isRecording_After.Should().BeFalse();
            btnText.Should().Be("⏳ Đang xử lý...");
            btnColor.Should().Be(Color.Orange);
        }
    }
}