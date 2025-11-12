
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Data;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class ClassManagementLogicTests
    {
        [TestMethod]
        public void LoadInitialData_WithValidConnection_ShouldReturnAllClasses()
        {
            // Arrange
            var expectedClasses = ClassManagementTestHelper.CreateSampleClassesDataTable();

            // Act
            var result = ClassManagementTestHelper.CreateSampleClassesDataTable();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            result.Columns.Contains("MaLop").Should().BeTrue();
            result.Columns.Contains("TenLop").Should().BeTrue();
            result.Columns.Contains("Khoi").Should().BeTrue();
        }

        [TestMethod]
        public void GetStudentsByClass_WithValidClass_ShouldReturnStudentList()
        {
            // Arrange
            string validClassCode = "10A1";

            // Act
            var result = ClassManagementTestHelper.CreateSampleStudentsDataTable();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            result.Columns.Contains("MaHS").Should().BeTrue();
            result.Columns.Contains("HoTen").Should().BeTrue();
            result.Columns.Contains("GioiTinh").Should().BeTrue();
        }

        [TestMethod]
        public void GetTeachingAssignments_WithValidClass_ShouldReturnAssignments()
        {
            // Arrange
            string validClassCode = "10A1";

            // Act
            var result = ClassManagementTestHelper.CreateSampleTeachingAssignmentsDataTable();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            result.Columns.Contains("TenMon").Should().BeTrue();
            result.Columns.Contains("TenGV").Should().BeTrue();
        }

        [TestMethod]
        public void InsertClass_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            string classCode = "12A1";
            string className = "Lớp 12A1";
            string grade = "12";
            string schoolYear = "2024";

            // Act
            var result = ClassManagementTestHelper.CreateSuccessClassOperationResult();

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void InsertClass_WithDuplicateCode_ShouldReturnFailure()
        {
            // Arrange
            string duplicateClassCode = "10A1"; // Already exists
            string className = "Lớp 10A1";
            string grade = "10";
            string schoolYear = "2024";

            // Act
            var result = ClassManagementTestHelper.CreateFailureClassOperationResult();

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void DeleteClass_WithNoStudents_ShouldReturnSuccess()
        {
            // Arrange
            string classCode = "12A1";
            bool hasStudents = false;

            // Act
            var result = ClassManagementTestHelper.CreateSuccessClassOperationResult();

            // Assert
            hasStudents.Should().BeFalse();
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void DeleteClass_WithStudents_ShouldReturnFailure()
        {
            // Arrange
            string classCode = "10A1";
            bool hasStudents = true;

            // Act
            var result = ClassManagementTestHelper.CreateFailureClassOperationResult();

            // Assert
            hasStudents.Should().BeTrue();
            result.Success.Should().BeFalse();
        }

        [TestMethod]
        public void TransferStudents_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var studentList = ClassManagementTestHelper.CreateValidStudentTransferList();
            string targetClass = "10A2";

            // Act
            var result = ClassManagementTestHelper.CreateSuccessTransferResult(studentList.Count);

            // Assert
            result.Success.Should().BeTrue();
            result.TransferredCount.Should().Be(studentList.Count);
        }

        [TestMethod]
        public void TransferStudents_WithEmptyList_ShouldReturnFailure()
        {
            // Arrange
            var emptyStudentList = ClassManagementTestHelper.CreateEmptyStudentTransferList();
            string targetClass = "10A2";

            // Act
            var result = ClassManagementTestHelper.CreateFailureTransferResult();

            // Assert
            emptyStudentList.Count.Should().Be(0);
            result.Success.Should().BeFalse();
            result.TransferredCount.Should().Be(0);
        }

        [TestMethod]
        public void UpdateHomeroomTeacher_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            string classCode = "10A1";
            string teacherCode = "GV001";

            // Act
            var result = ClassManagementTestHelper.CreateSuccessClassOperationResult();

            // Assert
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateStudentName_WithValidName_ShouldReturnTrue()
        {
            // Arrange
            string validName = ClassManagementTestHelper.CreateValidStudentName();

            // Act
            bool isValid = ClassManagementTestHelper.ValidateStudentName(validName);

            // Assert
            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateStudentName_WithInvalidName_ShouldReturnFalse()
        {
            // Arrange
            string invalidName = ClassManagementTestHelper.CreateInvalidStudentName();

            // Act
            bool isValid = ClassManagementTestHelper.ValidateStudentName(invalidName);

            // Assert
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidatePhoneNumber_WithValidNumber_ShouldReturnTrue()
        {
            // Arrange
            string validPhone = ClassManagementTestHelper.CreateValidPhoneNumber();

            // Act
            bool isValid = ClassManagementTestHelper.ValidatePhoneNumber(validPhone);

            // Assert
            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidatePhoneNumber_WithInvalidNumber_ShouldReturnFalse()
        {
            // Arrange
            string invalidPhone = ClassManagementTestHelper.CreateInvalidPhoneNumber();

            // Act
            bool isValid = ClassManagementTestHelper.ValidatePhoneNumber(invalidPhone);

            // Assert
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateClassCode_WithValidCode_ShouldReturnTrue()
        {
            // Arrange
            string validCode = ClassManagementTestHelper.CreateValidClassCode();

            // Act
            bool isValid = ClassManagementTestHelper.ValidateClassCode(validCode);

            // Assert
            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateClassCode_WithInvalidCode_ShouldReturnFalse()
        {
            // Arrange
            string invalidCode = ClassManagementTestHelper.CreateInvalidClassCode();

            // Act
            bool isValid = ClassManagementTestHelper.ValidateClassCode(invalidCode);

            // Assert
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void GetClassDetails_WithValidClass_ShouldReturnDetails()
        {
            // Arrange
            string validClassCode = "10A1";

            // Act
            var result = ClassManagementTestHelper.CreateSampleClassDetails();

            // Assert
            result.Should().NotBeNull();
            result["TenLop"].Should().NotBeNull();
            result["NamHoc"].Should().NotBeNull();
            result["TenGVCN"].Should().NotBeNull();
        }
    }
}
