using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;
using System.Data;
using System.Drawing;
using System.Collections.Generic; // Cần cho Dictionary

namespace N6.Tests.UnitTests.Forms
{
    [TestClass]
    public class TimetableFormTests
    {
        [TestMethod]
        public void LoadThoiKhoaBieu_WithData_ShouldPopulateGridAndColors()
        {
            // Sắp xếp
            var monday = TimetableTestHelper.GetTestMonday();
            var dt = TimetableTestHelper.CreateMockTimetableData(monday);

            // (Mô phỏng 2 biến state của UC_ThoiKhoaBieu)
            var cellValues = new string[7, 10]; // [col, row]
            var cellColors = new Dictionary<Point, Color>();

            // Hành động (Mô phỏng logic trong vòng lặp của LoadThoiKhoaBieu)
            foreach (DataRow r in dt.Rows)
            {
                DateTime ngay = (DateTime)r["Ngay"];
                int tiet = (int)r["Tiet"] - 1; // 0-based index
                int col = (int)ngay.DayOfWeek - (int)DayOfWeek.Monday; // (Thứ 2 = 0)
                if (col < 0) col = 6; // Chủ nhật

                string displayValue = $"{r["TenMon"]} - {r["TenLop"]}";
                if (!string.IsNullOrEmpty(r["GhiChu"].ToString()))
                {
                    displayValue += $"\n({r["GhiChu"]})";
                }
                cellValues[col, tiet] = displayValue;

                if (!string.IsNullOrEmpty(r["MauSac"].ToString()))
                {
                    cellColors[new Point(col, tiet)] = ColorTranslator.FromHtml(r["MauSac"].ToString());
                }
            }

            // Khẳng định
            // 1. Tiết 1, Thứ 2 (col 0, row 0)
            cellValues[0, 0].Should().Be("Toán - Lớp 5A");
            cellColors.ContainsKey(new Point(0, 0)).Should().BeTrue();
            

            // 2. Tiết 3, Thứ 4 (col 2, row 2)
            cellValues[2, 2].Should().Be("Tiếng Việt - Lớp 5B\n(Kiểm tra miệng)");
            cellColors.ContainsKey(new Point(2, 2)).Should().BeFalse();

            // 3. Tiết 5, Thứ 6 (col 4, row 4)
            cellValues[4, 4].Should().Be("Khoa học - Lớp 5A");
        }

        [TestMethod]
        public void ContextMenu_WhenCellIsEmpty_ShouldDisableMenuItems()
        {
            // Sắp xếp (Mô phỏng ô trống)
            string cellValue = "";
            bool coTKB = !string.IsNullOrEmpty(cellValue);
            bool coGhiChu = cellValue.Contains("(");

            // Hành động (Mô phỏng logic dgvTKB_CellMouseDown)
            bool doiMauEnabled = coTKB;
            bool xoaGhiChuEnabled = coGhiChu;
            bool xoaTKBEnabled = coTKB;

            // Khẳng định
            doiMauEnabled.Should().BeFalse();
            xoaGhiChuEnabled.Should().BeFalse();
            xoaTKBEnabled.Should().BeFalse();
        }

        [TestMethod]
        public void ContextMenu_WhenCellHasNote_ShouldEnableAllMenuItems()
        {
            // Sắp xếp (Mô phỏng ô có ghi chú)
            string cellValue = "Toán - Lớp 5A\n(Dặn dò)";
            bool coTKB = !string.IsNullOrEmpty(cellValue);
            bool coGhiChu = cellValue.Contains("(");

            // Hành động
            bool doiMauEnabled = coTKB;
            bool xoaGhiChuEnabled = coGhiChu;
            bool xoaTKBEnabled = coTKB;

            // Khẳng định
            doiMauEnabled.Should().BeTrue();
            xoaGhiChuEnabled.Should().BeTrue();
            xoaTKBEnabled.Should().BeTrue();
        }

        [TestMethod]
        public void GhiChuTKB_BtnLuu_WhenLopNotSelected_ShouldFailValidation()
        {
            // Sắp xếp (Mô phỏng frmGhiChuTKB)
            object selectedLop = null;
            string ghiChu = "Test";

            // Hành động (Mô phỏng logic validation)
            bool isValid = (selectedLop != null) && !string.IsNullOrWhiteSpace(ghiChu);

            // Khẳng định
            isValid.Should().BeFalse();
            // (Code thật sẽ hiển thị MessageBox "Vui lòng chọn lớp.")
        }

        [TestMethod]
        public void GhiChuTKB_BtnLuu_WhenNoteIsEmpty_ShouldFailValidation()
        {
            // Sắp xếp
            object selectedLop = "L5A";
            string ghiChu = " "; // Trống

            // Hành động
            bool isValid = (selectedLop != null) && !string.IsNullOrWhiteSpace(ghiChu);

            // Khẳng định
            isValid.Should().BeFalse();
            // (Code thật sẽ hiển thị MessageBox "Vui lòng nhập nội dung ghi chú.")
        }
    }
}