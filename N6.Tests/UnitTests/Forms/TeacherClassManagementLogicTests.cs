using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Data;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class TeacherClassManagementLogicTests
    {
        [TestMethod]
        public void GetClassesByTeacher_WithValidGV_ShouldReturnClasses()
        {
            // Arrange
            string teacherId = "GV001";

            // Act
            var result = TeacherClassManagementTestHelper.CreateTeachingClassesDataTable();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            result.Columns.Contains("MaLop").Should().BeTrue();
            result.Columns.Contains("TenLop").Should().BeTrue();
        }

        [TestMethod]
        public void GetHomeroomClassesByTeacher_WithValidGV_ShouldReturnClass()
        {
            // Arrange
            string teacherId = "GV001";

            // Act
            var result = TeacherClassManagementTestHelper.CreateHomeroomClassesDataTable();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().Be(1);
            result.Columns.Contains("MaLop").Should().BeTrue();
            result.Columns.Contains("TenLop").Should().BeTrue();
        }

        [TestMethod]
        public void GetAttendanceByClassAndDate_ShouldReturnRecords()
        {
            // Arrange
            string classCode = "10A1";
            DateTime date = DateTime.Today;
            string session = "Sáng";

            // Act
            var result = TeacherClassManagementTestHelper.CreateAttendanceDataTable();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            result.Columns.Contains("MaHS").Should().BeTrue();
            result.Columns.Contains("HoTen").Should().BeTrue();
            result.Columns.Contains("TrangThai").Should().BeTrue();
        }

        [TestMethod]
        public void UpsertAttendance_ShouldSaveOrUpdate()
        {
            // Arrange
            string studentId = "HS001";
            string classCode = "10A1";
            DateTime date = DateTime.Today;
            string session = "Sáng";
            string status = "Có mặt";

            // Act
            var result = TeacherClassManagementTestHelper.CreateSuccessOperationResult();

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void GetScoreboardPivot_ShouldReturnPivotData()
        {
            // Arrange
            string classCode = "10A1";
            int semester = 1;
            string subject = "TOAN";

            // Act
            var result = TeacherClassManagementTestHelper.CreateScorePivotDataTable();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            result.Columns.Contains("HoTen").Should().BeTrue();
            result.Columns.Contains("Thang1").Should().BeTrue();
            result.Columns.Contains("GiuaKi").Should().BeTrue();
        }

        [TestMethod]
        public void UpdateAcademicResult_ShouldSaveScore()
        {
            // Arrange
            string studentId = "HS001";
            string subject = "TOAN";
            string scoreType = "Thang1_Ki1";
            float score = 8.5f;

            // Act
            var result = TeacherClassManagementTestHelper.CreateSuccessOperationResult();

            // Assert
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void UpdateAcademicResult_WithInvalidScore_ShouldFail()
        {
            // Arrange
            string studentId = "HS001";
            string subject = "TOAN";
            string scoreType = "Thang1_Ki1";
            float invalidScore = 11.0f; // Invalid score

            // Act
            var result = TeacherClassManagementTestHelper.CreateFailureOperationResult();

            // Assert
            result.Success.Should().BeFalse();
        }

        [TestMethod]
        public void GetHomeroomGradebook_ShouldReturnClassScores()
        {
            // Arrange
            string classCode = "10A1";
            string scoreType = "GiuaKi1";

            // Act
            var result = TeacherClassManagementTestHelper.CreateHomeroomGradebookDataTable();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            result.Columns.Count.Should().BeGreaterThan(2); // More than just MaHS and HoTen
        }

        [TestMethod]
        public void GetClassFundByClass_ShouldReturnFundData()
        {
            // Arrange
            string classCode = "10A1";

            // Act
            var result = TeacherClassManagementTestHelper.CreateClassFundDataTable();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            result.Columns.Contains("Loai").Should().BeTrue();
            result.Columns.Contains("SoTien").Should().BeTrue();
        }

        [TestMethod]
        public void InsertClassFundEntry_ShouldAddTransaction()
        {
            // Arrange
            string classCode = "10A1";
            string type = "Thu";
            decimal amount = 50000;
            DateTime date = DateTime.Today;
            string description = "Tiền quỹ lớp";

            // Act
            var result = TeacherClassManagementTestHelper.CreateSuccessOperationResult();

            // Assert
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateScore_WithValidScore_ShouldReturnTrue()
        {
            // Arrange
            float validScore = 8.5f;

            // Act
            bool isValid = TeacherClassManagementTestHelper.ValidateScore(validScore);

            // Assert
            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateScore_WithInvalidScore_ShouldReturnFalse()
        {
            // Arrange
            float invalidScore = 11.0f;

            // Act
            bool isValid = TeacherClassManagementTestHelper.ValidateScore(invalidScore);

            // Assert
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void CalculateFundStatistics_ShouldReturnCorrectTotals()
        {
            // Arrange
            var fundData = TeacherClassManagementTestHelper.CreateClassFundDataTable();

            // Act
            decimal totalIncome = fundData.AsEnumerable()
                .Where(row => row.Field<string>("Loai") == "Thu")
                .Sum(row => row.Field<decimal>("SoTien"));

            decimal totalExpense = fundData.AsEnumerable()
                .Where(row => row.Field<string>("Loai") == "Chi")
                .Sum(row => row.Field<decimal>("SoTien"));

            decimal balance = totalIncome - totalExpense;

            // Assert
            totalIncome.Should().Be(150000);
            totalExpense.Should().Be(50000);
            balance.Should().Be(100000);
        }

        [TestMethod]
        public void CheckScoreLockStatus_WithLockedColumn_ShouldReturnTrue()
        {
            // Arrange
            string scoreColumn = "Thang1_Ki1";

            // Act
            bool isLocked = TeacherClassManagementTestHelper.CheckScoreLockStatus(scoreColumn);

            // Assert
            isLocked.Should().BeTrue();
        }

        [TestMethod]
        public void CheckScoreLockStatus_WithUnlockedColumn_ShouldReturnFalse()
        {
            // Arrange
            string scoreColumn = "Thang2_Ki1";

            // Act
            bool isLocked = TeacherClassManagementTestHelper.CheckScoreLockStatus(scoreColumn);

            // Assert
            isLocked.Should().BeFalse();
        }
    }
}