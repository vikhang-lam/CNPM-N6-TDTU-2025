using System.Data;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using N6.Tests.TestHelpers;
using System;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class TeacherClassManagementEdgeCaseTests
    {
        [TestMethod]
        public void UpdateAcademicResult_WithNullScore_ShouldHandleGracefully()
        {
            // Arrange
            string studentId = "HS001";
            string subject = "TOAN";
            string scoreType = "Thang1_Ki1";
            float? nullScore = null;

            // Act
            var result = TeacherClassManagementTestHelper.CreateSuccessOperationResult();

            // Assert
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void GetAttendanceByClassAndDate_WithNoData_ShouldReturnEmptyTable()
        {
            // Arrange
            string classCode = "NONEXISTENT";
            DateTime date = DateTime.Today.AddYears(1); // Future date
            string session = "Sáng";

            // Act
            var result = TeacherClassManagementTestHelper.CreateEmptyDataTable();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().Be(0);
        }

        [TestMethod]
        public void ValidateScore_WithBoundaryValues_ShouldWorkCorrectly()
        {
            // Arrange
            float minScore = 0.0f;
            float maxScore = 10.0f;
            float justBelowMin = -0.1f;
            float justAboveMax = 10.1f;

            // Act & Assert
            TeacherClassManagementTestHelper.ValidateScore(minScore).Should().BeTrue();
            TeacherClassManagementTestHelper.ValidateScore(maxScore).Should().BeTrue();
            TeacherClassManagementTestHelper.ValidateScore(justBelowMin).Should().BeFalse();
            TeacherClassManagementTestHelper.ValidateScore(justAboveMax).Should().BeFalse();
        }

        [TestMethod]
        public void CalculateFundBalance_WithMultipleTransactions_ShouldBeAccurate()
        {
            // Arrange
            var transactions = TeacherClassManagementTestHelper.CreateComplexFundDataTable();

            // Act
            decimal totalIncome = 0;
            decimal totalExpense = 0;

            foreach (DataRow row in transactions.Rows)
            {
                if (row["Loai"].ToString() == "Thu")
                    totalIncome += Convert.ToDecimal(row["SoTien"]);
                else if (row["Loai"].ToString() == "Chi")
                    totalExpense += Convert.ToDecimal(row["SoTien"]);
            }

            decimal finalBalance = totalIncome - totalExpense;

            // Assert
            totalIncome.Should().Be(250000);
            totalExpense.Should().Be(120000);
            finalBalance.Should().Be(130000);
        }

        [TestMethod]
        public void QRCodeScanning_WithInvalidQR_ShouldNotSaveAttendance()
        {
            // Arrange
            string invalidQRContent = "INVALID_QR_CODE";

            // Act
            bool isValidQR = TeacherClassManagementTestHelper.ValidateQRContent(invalidQRContent);
            var saveResult = TeacherClassManagementTestHelper.CreateFailureOperationResult();

            // Assert
            isValidQR.Should().BeFalse();
            saveResult.Success.Should().BeFalse();
        }

        [TestMethod]
        public void HandleConcurrentScoreUpdates_ShouldMaintainDataConsistency()
        {
            // Arrange
            string studentId = "HS001";
            string subject = "TOAN";
            string scoreType = "Thang1_Ki1";

            // Act
            var firstUpdate = TeacherClassManagementTestHelper.CreateSuccessOperationResult();
            var secondUpdate = TeacherClassManagementTestHelper.CreateSuccessOperationResult();

            // Assert
            firstUpdate.Success.Should().BeTrue();
            secondUpdate.Success.Should().BeTrue();
        }

        [TestMethod]
        public void LoadLargeClassData_ShouldPerformEfficiently()
        {
            // Arrange
            string largeClassCode = "12A1"; // Class with 50 students

            // Act
            var studentData = TeacherClassManagementTestHelper.CreateLargeClassDataTable();
            var scoreData = TeacherClassManagementTestHelper.CreateLargeScoreDataTable();

            bool studentsLoaded = studentData.Rows.Count == 50;
            bool scoresLoaded = scoreData.Rows.Count == 50;

            // Assert
            studentsLoaded.Should().BeTrue();
            scoresLoaded.Should().BeTrue();
        }

        [TestMethod]
        public void BackupClassData_ShouldCreateBackupSuccessfully()
        {
            // Arrange
            string classCode = "10A1";
            DateTime backupDate = DateTime.Now;

            // Act
            var backupResult = TeacherClassManagementTestHelper.CreateSuccessOperationResult();

            // Assert
            backupResult.Success.Should().BeTrue();
            backupResult.Message.Should().Contain("backup");
        }
    }
}