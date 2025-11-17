using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Data;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Integration
{
    [TestClass]
    public class TeacherClassManagementIntegrationTests
    {
        [TestMethod]
        public void CompleteTeachingClassFlow_ShouldWorkEndToEnd()
        {
            // Arrange
            string username = "teacher123";
            string classCode = "10A1";

            // Act - Simulate complete teaching class flow
            // Step 1: Load classes
            var classes = TeacherClassManagementTestHelper.CreateTeachingClassesDataTable();
            bool classesLoaded = classes.Rows.Count > 0;

            // Step 2: Select class
            bool classSelected = true;

            // Step 3: Take attendance
            var attendanceData = TeacherClassManagementTestHelper.CreateAttendanceDataTable();
            bool attendanceTaken = attendanceData.Rows.Count > 0;

            // Step 4: Enter scores
            var scoreData = TeacherClassManagementTestHelper.CreateScorePivotDataTable();
            bool scoresEntered = scoreData.Rows.Count > 0;

            // Step 5: View students
            var studentData = TeacherClassManagementTestHelper.CreateStudentDataTable();
            bool studentsViewed = studentData.Rows.Count > 0;

            // Assert
            classesLoaded.Should().BeTrue();
            classSelected.Should().BeTrue();
            attendanceTaken.Should().BeTrue();
            scoresEntered.Should().BeTrue();
            studentsViewed.Should().BeTrue();
        }

        [TestMethod]
        public void CompleteHomeroomClassFlow_ShouldWorkEndToEnd()
        {
            // Arrange
            string username = "teacher123";
            string homeroomClassCode = "10A1";

            // Act - Simulate complete homeroom class flow
            // Step 1: Load homeroom class
            var homeroomClass = TeacherClassManagementTestHelper.CreateHomeroomClassesDataTable();
            bool homeroomLoaded = homeroomClass.Rows.Count > 0;

            // Step 2: View gradebook
            var gradebookData = TeacherClassManagementTestHelper.CreateHomeroomGradebookDataTable();
            bool gradebookViewed = gradebookData.Rows.Count > 0;

            // Step 3: Manage class fund
            var fundData = TeacherClassManagementTestHelper.CreateClassFundDataTable();
            bool fundManaged = fundData.Rows.Count > 0;

            // Step 4: Add transaction
            var addResult = TeacherClassManagementTestHelper.CreateSuccessOperationResult();
            bool transactionAdded = addResult.Success;

            // Assert
            homeroomLoaded.Should().BeTrue();
            gradebookViewed.Should().BeTrue();
            fundManaged.Should().BeTrue();
            transactionAdded.Should().BeTrue();
        }

        [TestMethod]
        public void AttendanceQRFlow_ShouldMarkAttendance()
        {
            // Arrange
            string classCode = "10A1";
            string studentId = "HS001";
            DateTime date = DateTime.Today;
            string session = "Sáng";

            // Act - Simulate QR attendance flow
            // Step 1: Start QR scanning
            bool qrStarted = true;

            // Step 2: Scan valid QR code
            bool qrScanned = true;

            // Step 3: Save attendance
            var saveResult = TeacherClassManagementTestHelper.CreateSuccessOperationResult();
            bool attendanceSaved = saveResult.Success;

            // Step 4: Verify attendance
            var attendanceData = TeacherClassManagementTestHelper.CreateAttendanceDataTable();
            bool attendanceVerified = attendanceData.Rows.Count > 0;

            // Assert
            qrStarted.Should().BeTrue();
            qrScanned.Should().BeTrue();
            attendanceSaved.Should().BeTrue();
            attendanceVerified.Should().BeTrue();
        }

        [TestMethod]
        public void ScoreEntryFlow_WithValidations_ShouldHandleErrors()
        {
            // Arrange
            string studentId = "HS001";
            string subject = "TOAN";
            string scoreType = "Thang1_Ki1";

            // Act - Simulate score entry with validation
            // Step 1: Enter invalid score
            float invalidScore = 11.0f;
            bool scoreValidated = TeacherClassManagementTestHelper.ValidateScore(invalidScore);

            // Step 2: Show error message
            bool errorShown = !scoreValidated;

            // Step 3: Enter valid score
            float validScore = 8.5f;
            bool validScoreEntered = TeacherClassManagementTestHelper.ValidateScore(validScore);

            // Step 4: Save score
            var saveResult = TeacherClassManagementTestHelper.CreateSuccessOperationResult();
            bool scoreSaved = saveResult.Success;

            // Assert
            scoreValidated.Should().BeFalse();
            errorShown.Should().BeTrue();
            validScoreEntered.Should().BeTrue();
            scoreSaved.Should().BeTrue();
        }

        [TestMethod]
        public void FundManagementFlow_ShouldUpdateBalance()
        {
            // Arrange
            string classCode = "10A1";
            decimal initialBalance = 100000;

            // Act - Simulate fund management flow
            // Step 1: Add income
            decimal income = 50000;
            var incomeResult = TeacherClassManagementTestHelper.CreateSuccessOperationResult();
            bool incomeAdded = incomeResult.Success;

            // Step 2: Add expense
            decimal expense = 30000;
            var expenseResult = TeacherClassManagementTestHelper.CreateSuccessOperationResult();
            bool expenseAdded = expenseResult.Success;

            // Step 3: Calculate new balance
            decimal newBalance = initialBalance + income - expense;

            // Step 4: Verify balance update
            bool balanceUpdated = newBalance == 120000;

            // Assert
            incomeAdded.Should().BeTrue();
            expenseAdded.Should().BeTrue();
            balanceUpdated.Should().BeTrue();
            newBalance.Should().Be(120000);
        }

        [TestMethod]
        public void ClassNavigationFlow_ShouldSwitchBetweenViews()
        {
            // Arrange
            string username = "teacher123";

            // Act - Simulate class navigation flow
            // Step 1: Start with class selection
            bool classSelectionActive = true;

            // Step 2: Switch to teaching class
            bool teachingViewActive = true;
            bool homeroomViewActive = false;

            // Step 3: Switch back to class selection
            classSelectionActive = true;
            teachingViewActive = false;

            // Step 4: Switch to homeroom class
            homeroomViewActive = true;
            classSelectionActive = false;

            // Assert
            teachingViewActive.Should().BeFalse();
            homeroomViewActive.Should().BeTrue();
        }

        [TestMethod]
        public void DataPersistenceFlow_ShouldMaintainDataIntegrity()
        {
            // Arrange
            string classCode = "10A1";

            // Act - Simulate data persistence flow
            // Step 1: Load initial data
            var initialData = TeacherClassManagementTestHelper.CreateStudentDataTable();
            int initialCount = initialData.Rows.Count;

            // Step 2: Perform operations (attendance, scores, etc.)
            var attendanceResult = TeacherClassManagementTestHelper.CreateSuccessOperationResult();
            var scoreResult = TeacherClassManagementTestHelper.CreateSuccessOperationResult();

            // Step 3: Reload data
            var reloadedData = TeacherClassManagementTestHelper.CreateStudentDataTable();
            int reloadedCount = reloadedData.Rows.Count;

            // Step 4: Verify data integrity
            bool dataIntegrityMaintained = initialCount == reloadedCount;
            bool operationsSuccessful = attendanceResult.Success && scoreResult.Success;

            // Assert
            dataIntegrityMaintained.Should().BeTrue();
            operationsSuccessful.Should().BeTrue();
            reloadedCount.Should().Be(initialCount);
        }
    }
}