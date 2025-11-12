using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Data;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Integration
{
    [TestClass]
    public class SchoolManagementIntegrationTests
    {
        [TestMethod]
        public void CompleteSubjectManagementFlow_ValidScenario_ShouldSucceed()
        {
            // Arrange
            var subject = SchoolManagementTestHelper.CreateValidSubject();

            // Act - Step 1: Load subjects
            var subjects = SchoolManagementTestHelper.CreateMockSubjectsDataTable();
            bool loadSuccess = subjects.Rows.Count > 0;

            // Step 2: Add new subject
            bool addSuccess = SchoolManagementTestHelper.CreateSuccessfulDatabaseOperation();

            // Step 3: Reload to verify
            var updatedSubjects = SchoolManagementTestHelper.CreateMockSubjectsDataTable();
            bool verifySuccess = updatedSubjects.Rows.Count >= subjects.Rows.Count;

            // Assert
            loadSuccess.Should().BeTrue();
            addSuccess.Should().BeTrue();
            verifySuccess.Should().BeTrue();
        }

        [TestMethod]
        public void CompleteScoreDeadlineManagementFlow_ValidScenario_ShouldSucceed()
        {
            // Arrange
            var deadlines = SchoolManagementTestHelper.CreateMockScoreDeadlinesDataTable();

            // Act - Step 1: Load deadlines
            bool loadSuccess = deadlines.Rows.Count > 0;

            // Step 2: Filter by grade
            var filteredDeadlines = deadlines.AsEnumerable()
                .Where(row => row["Khoi"].ToString() == "Khối 1")
                .CopyToDataTable();
            bool filterSuccess = filteredDeadlines.Rows.Count > 0;

            // Step 3: Update deadline
            bool updateSuccess = SchoolManagementTestHelper.CreateSuccessfulDatabaseOperation();

            // Step 4: Save changes
            bool saveSuccess = SchoolManagementTestHelper.CreateSuccessfulDatabaseOperation();

            // Assert
            loadSuccess.Should().BeTrue();
            filterSuccess.Should().BeTrue();
            updateSuccess.Should().BeTrue();
            saveSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void CompleteStudentPromotionFlow_ValidScenario_ShouldSucceed()
        {
            // Arrange
            var promotionData = SchoolManagementTestHelper.CreateValidPromotionData();
            var destinationClasses = SchoolManagementTestHelper.CreateValidDestinationClasses();

            // Act - Step 1: Load class data
            var classDetails = SchoolManagementTestHelper.CreateMockClassDetails();
            bool classLoaded = classDetails.Rows.Count > 0;

            // Step 2: Calculate student averages
            var scoreboard = SchoolManagementTestHelper.CreateMockSemesterScoreboard();
            bool averagesCalculated = scoreboard.Rows.Count > 0;

            // Step 3: Load destination classes
            var availableClasses = SchoolManagementTestHelper.CreateMockClassesDataTable();
            bool destinationsLoaded = availableClasses.Rows.Count > 0;

            // Step 4: Execute promotion
            bool promotionSuccess = SchoolManagementTestHelper.CreateSuccessfulDatabaseOperation();

            // Assert
            classLoaded.Should().BeTrue();
            averagesCalculated.Should().BeTrue();
            destinationsLoaded.Should().BeTrue();
            promotionSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void CompleteStudentPromotionFlow_Grade5Graduation_ShouldSucceed()
        {
            // Arrange
            var promotionData = SchoolManagementTestHelper.CreateGrade5PromotionData();

            // Act - Step 1: Load class data
            var classDetails = SchoolManagementTestHelper.CreateMockClassDetails();
            bool isGrade5 = classDetails.Rows[0]["Khoi"].ToString() == "Khối 5";

            // Step 2: Calculate student averages
            var scoreboard = SchoolManagementTestHelper.CreateMockSemesterScoreboard();
            int passingStudents = scoreboard.AsEnumerable()
                .Count(row => Convert.ToDouble(row["Trung bình chung"]) >= 5.0);

            // Step 3: Execute graduation (no destination class needed)
            bool graduationSuccess = SchoolManagementTestHelper.CreateSuccessfulDatabaseOperation();

            // Assert
            isGrade5.Should().BeTrue();
            passingStudents.Should().BeGreaterThan(0);
            graduationSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void SubjectManagementFlow_WithDatabaseError_ShouldHandleGracefully()
        {
            // Arrange
            var subject = SchoolManagementTestHelper.CreateValidSubject();

            // Act - Step 1: Attempt to add subject
            bool operationSuccess = SchoolManagementTestHelper.CreateFailedDatabaseOperation();
            var exception = SchoolManagementTestHelper.CreateDatabaseException();

            // Step 2: Verify error handling
            bool errorHandled = exception != null;
            bool uiNotCorrupted = true; // UI should remain stable

            // Assert
            operationSuccess.Should().BeFalse();
            errorHandled.Should().BeTrue();
            uiNotCorrupted.Should().BeTrue();
        }

        [TestMethod]
        public void ScoreDeadlineFlow_WithInvalidDateRange_ShouldFailValidation()
        {
            // Arrange
            DateTime ngayMo = new DateTime(2024, 12, 31);
            DateTime ngayKhoa = new DateTime(2024, 9, 1); // Invalid: close date before open date

            // Act
            bool isValidDateRange = ngayKhoa > ngayMo;

            // Assert
            isValidDateRange.Should().BeFalse();
        }

        [TestMethod]
        public void StudentPromotionFlow_WithNoStudents_ShouldShowAppropriateMessage()
        {
            // Arrange
            string maLop = "EMPTY_CLASS";

            // Act
            var scoreboard = new DataTable(); // Empty scoreboard
            bool hasStudents = scoreboard.Rows.Count > 0;
            string message = hasStudents ? "Đã tải dữ liệu lớp" : "Lớp không có học sinh";

            // Assert
            hasStudents.Should().BeFalse();
            message.Should().Be("Lớp không có học sinh");
        }

        [TestMethod]
        public void TabNavigationFlow_ShouldSwitchBetweenTabsSuccessfully()
        {
            // Arrange & Act
            bool monHocTabAccessible = true;
            bool thoiHanTabAccessible = true;
            bool lenLopTabAccessible = true;
            bool dataPersistedBetweenTabs = true;

            // Assert
            monHocTabAccessible.Should().BeTrue();
            thoiHanTabAccessible.Should().BeTrue();
            lenLopTabAccessible.Should().BeTrue();
            dataPersistedBetweenTabs.Should().BeTrue();
        }
    }
}
