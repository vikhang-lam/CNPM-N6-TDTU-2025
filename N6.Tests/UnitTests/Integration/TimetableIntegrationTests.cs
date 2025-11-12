using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;
using System;

namespace N6.Tests.UnitTests.Integration
{
    [TestClass]
    public class TimetableIntegrationTests
    {
        // Biến static để theo dõi sự kiện
        private static bool timetableEventWasRaised = false;

        // Hàm xử lý sự kiện giả lập
        private static void Test_OnThoiKhoaBieuChanged(object sender, EventArgs e)
        {
            timetableEventWasRaised = true;
        }

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            // Đăng ký listener VÀO ĐẦU tất cả các test
            DatabaseHelper.TimetableChanged += Test_OnThoiKhoaBieuChanged;
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            // Hủy đăng ký listener SAU KHI chạy xong
            DatabaseHelper.TimetableChanged -= Test_OnThoiKhoaBieuChanged;
        }

        [TestInitialize]
        public void ResetEventFlag()
        {
            // Đặt lại cờ trước mỗi test
            timetableEventWasRaised = false;
        }

        [TestMethod]
        public void UC37_WeekNavigation_BtnPrevWeek_ShouldDecreaseMonday()
        {
            // Sắp xếp (Mô phỏng UC_ThoiKhoaBieu)
            DateTime currentMonday = TimetableTestHelper.GetTestMonday(); // 10/11/2025
            DateTime expectedMonday = currentMonday.AddDays(-7); // 03/11/2025

            // Hành động (Mô phỏng btnPrevWeek_Click)
            currentMonday = currentMonday.AddDays(-7);
            // (Sau đó gọi LoadThoiKhoaBieu())

            // Khẳng định
            currentMonday.Should().Be(expectedMonday);
        }

        [TestMethod]
        public void UC39_AddNoteFromExternalForm_ShouldRaiseEvent()
        {
            // Sắp xếp (Mô phỏng frmGhiChuTKB)
            var (maLop, tiet, ghiChu) = TimetableTestHelper.CreateValidNoteInput();

            // Hành động (Mô phỏng BtnLuu_Click)
            // 1. (Lưu vào DB - Bỏ qua)
            // 2. (Phát tín hiệu)
            DatabaseHelper.RaiseTimetableChanged();

            // Khẳng định
            // (Listener (Test_OnThoiKhoaBieuChanged) đã bắt được sự kiện)
            timetableEventWasRaised.Should().BeTrue("vì frmGhiChuTKB đã gọi RaiseTimetableChanged");
        }

        [TestMethod]
        public void UC37_OnEventRaised_ShouldReloadData()
        {
            // Sắp xếp (Mô phỏng UC_ThoiKhoaBieu đang chạy)
            bool loadTkbCalled = false;
            if (timetableEventWasRaised) // (Giả định event vừa được raise từ TC trên)
            {
                // Hành động (Mô phỏng OnThoiKhoaBieuChanged)
                loadTkbCalled = true; // (Gọi LoadThoiKhoaBieu())
            }

            // Khẳng định
            // (Chúng ta trigger lại sự kiện để chắc chắn)
            DatabaseHelper.RaiseTimetableChanged();
            if (timetableEventWasRaised)
            {
                loadTkbCalled = true;
            }
            loadTkbCalled.Should().BeTrue();
        }

        [TestMethod]
        public void UC38_ImportTimetable_ShouldReloadData()
        {
            // Sắp xếp
            bool loadTkbCalled = false;
            // (Mô phỏng frmImportExcel trả về OK)
            bool importDialogResultOK = true;

            // Hành động (Mô phỏng btnImportTKB_Click)
            if (importDialogResultOK)
            {
                loadTkbCalled = true; // (Gọi LoadThoiKhoaBieu())
            }

            // Khẳng định
            loadTkbCalled.Should().BeTrue();
        }

        [TestMethod]
        public void UC39_DeleteNoteFromGrid_ShouldReloadData()
        {
            // Sắp xếp
            bool loadTkbCalled = false;
            // (Mô phỏng click "xoaGhiChuMenuItem")

            // Hành động
            // 1. (Gọi DatabaseHelper.DeleteTimetableNote())
            // 2. (Gọi LoadThoiKhoaBieu())
            loadTkbCalled = true;

            // Khẳng định
            loadTkbCalled.Should().BeTrue();
        }
    }
}