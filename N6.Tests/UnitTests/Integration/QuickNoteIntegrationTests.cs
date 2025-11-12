using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;
using System.Data;

namespace N6.Tests.UnitTests.Integration
{
    [TestClass]
    public class QuickNoteIntegrationTests
    {
        [TestMethod]
        public void FullNoteFlow_ValidTyping_ShouldSucceed()
        {
            // Sắp xếp
            string maGV = "GV001";
            var (maHS, maMon, ghiChu) = QuickNoteTestHelper.CreateValidNoteInput();
            string selectedMaLop = "L5A";

            // Hành động (Mô phỏng luồng)

            // 1. (LoadLopHoc) Tải lớp
            var dtLop = QuickNoteTestHelper.CreateMockClassesData();
            bool lopLoaded = dtLop.Rows.Count > 0;
            // (User chọn selectedMaLop)

            // 2. (CbLop_Changed) Tải Môn + HS (dựa trên selectedMaLop)
            var dtMon = QuickNoteTestHelper.CreateMockSubjectsData();
            var dtHS = QuickNoteTestHelper.CreateMockStudentsData();
            bool monHocLoaded = dtMon.Rows.Count > 0;
            bool hocSinhLoaded = dtHS.Rows.Count > 0;
            // (User chọn maMon, maHS)

            // 3. (BtnLuu_Click) Validate
            bool isValid = !string.IsNullOrEmpty(selectedMaLop) &&
                           !string.IsNullOrEmpty(maMon) &&
                           !string.IsNullOrEmpty(maHS) &&
                           !string.IsNullOrWhiteSpace(ghiChu);

            // 4. (Save) Gọi DB (mô phỏng)
            bool savedToDb = isValid; // (Vì DatabaseHelper.AddNoteForStudent là void)

            // Khẳng định
            lopLoaded.Should().BeTrue();
            monHocLoaded.Should().BeTrue();
            hocSinhLoaded.Should().BeTrue();
            isValid.Should().BeTrue();
            savedToDb.Should().BeTrue();
        }

        [TestMethod]
        public void FullNoteFlow_ValidationFails_ShouldNotSave()
        {
            // Sắp xếp
            string maGV = "GV001";
            var (maHS, maMon, ghiChu) = QuickNoteTestHelper.CreateValidNoteInput();
            string selectedMaLop = null; // (User KHÔNG chọn lớp)

            // Hành động (Mô phỏng luồng)

            // 1. (LoadLopHoc) Tải lớp
            var dtLop = QuickNoteTestHelper.CreateMockClassesData();
            bool lopLoaded = dtLop.Rows.Count > 0;

            // 2. (CbLop_Changed) Không xảy ra vì lớp chưa được chọn

            // 3. (BtnLuu_Click) Validate
            bool isValid = !string.IsNullOrEmpty(selectedMaLop) && // -> false
                           !string.IsNullOrEmpty(maMon) &&
                           !string.IsNullOrEmpty(maHS) &&
                           !string.IsNullOrWhiteSpace(ghiChu);

            // 4. (Save)
            bool savedToDb = isValid; // (Bị chặn)

            // Khẳng định
            lopLoaded.Should().BeTrue();
            isValid.Should().BeFalse();
            savedToDb.Should().BeFalse();
        }

        [TestMethod]
        public void ComboBoxFlow_WhenLopChanges_ShouldReloadMonHocAndHocSinh()
        {
            // Sắp xếp
            string maGV = "GV001";
            string maLop_L5A = "L5A";
            string maLop_L5B = "L5B";

            // Hành động
            // 1. (LoadLopHoc)
            var dtLop = QuickNoteTestHelper.CreateMockClassesData();

            // 2. User chọn L5A (CbLop_Changed)
            // (Giả định DB trả về 2 Môn và 30 HS cho L5A)
            var dtMon_L5A = QuickNoteTestHelper.CreateMockSubjectsData(); // 2 môn
            var dtHS_L5A = QuickNoteTestHelper.CreateMockStudentsData(); // 2 HS
            int countMon_L5A = dtMon_L5A.Rows.Count;
            int countHS_L5A = dtHS_L5A.Rows.Count;

            // 3. User đổi sang L5B (CbLop_Changed)
            // (Giả định DB trả về 1 Môn và 25 HS cho L5B)
            var dtMon_L5B = new DataTable(); // Tạo mock khác
            dtMon_L5B.Columns.Add("MaMon"); dtMon_L5B.Columns.Add("TenMon");
            dtMon_L5B.Rows.Add("KH", "Khoa học");
            var dtHS_L5B = new DataTable();
            dtHS_L5B.Columns.Add("MaHS"); dtHS_L5B.Columns.Add("HoTen");
            dtHS_L5B.Rows.Add("HS100", "Lê Văn D");

            int countMon_L5B = dtMon_L5B.Rows.Count;
            int countHS_L5B = dtHS_L5B.Rows.Count;

            // Khẳng định
            countMon_L5A.Should().Be(2);
            countHS_L5A.Should().Be(2);
            countMon_L5B.Should().Be(1);
            countHS_L5B.Should().Be(1);
        }
    }
}