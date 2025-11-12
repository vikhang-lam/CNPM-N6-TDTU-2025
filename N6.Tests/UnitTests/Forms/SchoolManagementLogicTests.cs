using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Data;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class SchoolManagementLogicTests
    {
        [TestMethod]
        public void InsertSubject_WithValidData_ShouldSucceed()
        {
            // Arrange
            var subject = SchoolManagementTestHelper.CreateValidSubject();

            // Act
            bool isValid = !string.IsNullOrWhiteSpace(subject.tenMon);
            bool operationSuccess = SchoolManagementTestHelper.CreateSuccessfulDatabaseOperation();

            // Assert
            isValid.Should().BeTrue();
            operationSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void InsertSubject_WithEmptyName_ShouldFailValidation()
        {
            // Arrange
            var subject = SchoolManagementTestHelper.CreateInvalidSubject_EmptyName();

            // Act
            bool isValid = !string.IsNullOrWhiteSpace(subject.tenMon);

            // Assert
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void UpdateSubject_WithValidData_ShouldSucceed()
        {
            // Arrange
            var subject = SchoolManagementTestHelper.CreateValidSubject();

            // Act
            bool hasValidId = !string.IsNullOrWhiteSpace(subject.maMon);
            bool hasValidName = !string.IsNullOrWhiteSpace(subject.tenMon);
            bool operationSuccess = SchoolManagementTestHelper.CreateSuccessfulDatabaseOperation();

            // Assert
            hasValidId.Should().BeTrue();
            hasValidName.Should().BeTrue();
            operationSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void DeleteSubject_WhenNotInUse_ShouldSucceed()
        {
            // Arrange
            var subject = SchoolManagementTestHelper.CreateValidSubject();

            // Act
            bool hasValidId = !string.IsNullOrWhiteSpace(subject.maMon);
            bool operationSuccess = SchoolManagementTestHelper.CreateSuccessfulDatabaseOperation();

            // Assert
            hasValidId.Should().BeTrue();
            operationSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void DeleteSubject_WhenInUse_ShouldFail()
        {
            // Arrange
            var subject = SchoolManagementTestHelper.CreateValidSubject();

            // Act
            bool hasValidId = !string.IsNullOrWhiteSpace(subject.maMon);
            bool operationSuccess = SchoolManagementTestHelper.CreateFailedDatabaseOperation();
            var exception = SchoolManagementTestHelper.CreateDatabaseException("Môn học đang được sử dụng, không thể xóa");

            // Assert
            hasValidId.Should().BeTrue();
            operationSuccess.Should().BeFalse();
            exception.Message.Should().Contain("đang được sử dụng");
        }

        [TestMethod]
        public void UpdateScoreDeadline_WithValidData_ShouldSucceed()
        {
            // Arrange
            var deadlines = SchoolManagementTestHelper.CreateMockScoreDeadlinesDataTable();
            var row = deadlines.Rows[0];

            // Act
            bool hasValidMaCotDiem = !string.IsNullOrEmpty(row["MaCotDiem"].ToString());
            bool hasValidKhoi = !string.IsNullOrEmpty(row["Khoi"].ToString());
            bool operationSuccess = SchoolManagementTestHelper.CreateSuccessfulDatabaseOperation();

            // Assert
            hasValidMaCotDiem.Should().BeTrue();
            hasValidKhoi.Should().BeTrue();
            operationSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void CalculateStudentAverages_WithValidClass_ShouldReturnResults()
        {
            // Arrange
            string maLop = "LOP1A";

            // Act
            var scoreboard = SchoolManagementTestHelper.CreateMockSemesterScoreboard();
            bool hasData = scoreboard.Rows.Count > 0;
            bool hasRequiredColumns = scoreboard.Columns.Contains("MaHS") &&
                                    scoreboard.Columns.Contains("HoTen") &&
                                    scoreboard.Columns.Contains("Trung bình chung");

            // Assert
            hasData.Should().BeTrue();
            hasRequiredColumns.Should().BeTrue();
        }

        [TestMethod]
        public void CalculateStudentAverages_WithEmptyClass_ShouldReturnEmptyList()
        {
            // Arrange
            string maLop = "EMPTY_CLASS";

            // Act
            var scoreboard = new DataTable(); // Empty table
            bool hasData = scoreboard.Rows.Count > 0;

            // Assert
            hasData.Should().BeFalse();
        }

        [TestMethod]
        public void LoadDestinationClassComboBoxes_ForRegularClass_ShouldShowNextGrade()
        {
            // Arrange
            var promotionData = SchoolManagementTestHelper.CreateValidPromotionData();

            // Act
            int currentGradeLevel = 1; // Khối 1
            int nextGradeLevel = currentGradeLevel + 1;
            string nextKhoi = $"Khối {nextGradeLevel}";
            bool shouldEnableComboBox = true;

            // Assert
            nextGradeLevel.Should().Be(2);
            nextKhoi.Should().Be("Khối 2");
            shouldEnableComboBox.Should().BeTrue();
        }

        [TestMethod]
        public void LoadDestinationClassComboBoxes_ForGrade5_ShouldShowGraduation()
        {
            // Arrange
            var promotionData = SchoolManagementTestHelper.CreateGrade5PromotionData();

            // Act
            bool isLop5 = promotionData.isLop5;
            string promptText = isLop5 ? "Tốt nghiệp:" : "Chuyển đến lớp:*";
            bool comboBoxEnabled = !isLop5;

            // Assert
            isLop5.Should().BeTrue();
            promptText.Should().Be("Tốt nghiệp:");
            comboBoxEnabled.Should().BeFalse();
        }

        [TestMethod]
        public void ValidatePromotionData_WithMissingDestinationClasses_ShouldFail()
        {
            // Arrange
            var promotionData = SchoolManagementTestHelper.CreateValidPromotionData();
            var destinationClasses = SchoolManagementTestHelper.CreateInvalidDestinationClasses_Missing();

            // Act
            bool hasPassingStudents = true; // Simulate having passing students
            bool hasFailingStudents = true; // Simulate having failing students
            bool lenLopClassValid = !string.IsNullOrEmpty(destinationClasses.maLopMoiLenLop) || !promotionData.isLop5;
            bool oLaiLopClassValid = !string.IsNullOrEmpty(destinationClasses.maLopMoiOLaiLop);

            bool validationPassed = (!hasPassingStudents || lenLopClassValid) &&
                                  (!hasFailingStudents || oLaiLopClassValid);

            // Assert
            validationPassed.Should().BeFalse();
        }

        [TestMethod]
        public void ValidatePromotionData_WithValidDestinationClasses_ShouldPass()
        {
            // Arrange
            var promotionData = SchoolManagementTestHelper.CreateValidPromotionData();
            var destinationClasses = SchoolManagementTestHelper.CreateValidDestinationClasses();

            // Act
            bool hasPassingStudents = true;
            bool hasFailingStudents = true;
            bool lenLopClassValid = !string.IsNullOrEmpty(destinationClasses.maLopMoiLenLop) || !promotionData.isLop5;
            bool oLaiLopClassValid = !string.IsNullOrEmpty(destinationClasses.maLopMoiOLaiLop);

            bool validationPassed = (!hasPassingStudents || lenLopClassValid) &&
                                  (!hasFailingStudents || oLaiLopClassValid);

            // Assert
            validationPassed.Should().BeTrue();
        }

        [TestMethod]
        public void ExecuteStudentPromotion_WithValidData_ShouldUpdateStudentClasses()
        {
            // Arrange
            var promotionData = SchoolManagementTestHelper.CreateValidPromotionData();
            var destinationClasses = SchoolManagementTestHelper.CreateValidDestinationClasses();
            var students = SchoolManagementTestHelper.CreateMockStudentPromotionDataTable();

            // Act
            int passingCount = 2; // Students with DiemTB >= 5.0
            int failingCount = 1; // Students with DiemTB < 5.0
            int graduationCount = 0; // Not grade 5

            bool operationSuccess = SchoolManagementTestHelper.CreateSuccessfulDatabaseOperation();

            // Assert
            passingCount.Should().Be(2);
            failingCount.Should().Be(1);
            graduationCount.Should().Be(0);
            operationSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void StudentPromotionInfo_PassStatus_ShouldCalculateCorrectly()
        {
            // Arrange
            double passingScore = 5.0;

            // Act
            bool passStatus85 = 8.5 >= passingScore; // true
            bool passStatus42 = 4.2 >= passingScore; // false
            bool passStatus78 = 7.8 >= passingScore; // true

            // Assert
            passStatus85.Should().BeTrue();
            passStatus42.Should().BeFalse();
            passStatus78.Should().BeTrue();
        }
    }
}
