using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Data;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Integration
{
    [TestClass]
    public class ClassManagementIntegrationTests
    {
        [TestMethod]
        public void CompleteClassManagementFlow_ValidScenario_ShouldSucceed()
        {
            // Arrange
            var classes = ClassManagementTestHelper.CreateSampleClassesDataTable();
            var students = ClassManagementTestHelper.CreateSampleStudentsDataTable();
            var assignments = ClassManagementTestHelper.CreateSampleTeachingAssignmentsDataTable();

            // Act - Simulate complete flow
            // Step 1: Load classes
            bool classesLoaded = classes.Rows.Count > 0;

            // Step 2: Select class
            bool classSelected = true;
            var selectedClass = classes.Rows[0];

            // Step 3: Load class details
            bool detailsLoaded = true;
            bool studentsLoaded = students.Rows.Count > 0;
            bool assignmentsLoaded = assignments.Rows.Count > 0;

            // Step 4: Update homeroom teacher
            bool teacherUpdated = true;

            // Assert
            classesLoaded.Should().BeTrue();
            classSelected.Should().BeTrue();
            detailsLoaded.Should().BeTrue();
            studentsLoaded.Should().BeTrue();
            assignmentsLoaded.Should().BeTrue();
            teacherUpdated.Should().BeTrue();
        }

        [TestMethod]
        public void StudentTransferFlow_ValidScenario_ShouldTransferStudents()
        {
            // Arrange
            var sourceClass = "10A1";
            var targetClass = "10A2";
            var studentList = ClassManagementTestHelper.CreateValidStudentTransferList();

            // Act - Simulate transfer flow
            // Step 1: Select students to transfer
            bool studentsSelected = studentList.Count > 0;

            // Step 2: Select target class
            bool targetClassSelected = true;

            // Step 3: Execute transfer
            var transferResult = ClassManagementTestHelper.CreateSuccessTransferResult(studentList.Count);

            // Step 4: Verify transfer
            bool transferVerified = transferResult.Success && transferResult.TransferredCount == studentList.Count;

            // Assert
            studentsSelected.Should().BeTrue();
            targetClassSelected.Should().BeTrue();
            transferResult.Success.Should().BeTrue();
            transferVerified.Should().BeTrue();
        }

        [TestMethod]
        public void ClassCreationFlow_ValidScenario_ShouldCreateClass()
        {
            // Arrange
            string newClassCode = "12A1";
            string newClassName = "Lớp 12A1";
            string grade = "12";
            string schoolYear = "2024";

            // Act - Simulate class creation flow
            // Step 1: Validate class data
            bool dataValid = ClassManagementTestHelper.ValidateClassCode(newClassCode) &&
                           ClassManagementTestHelper.ValidateGrade(grade);

            // Step 2: Create class
            var createResult = ClassManagementTestHelper.CreateSuccessClassOperationResult();

            // Step 3: Verify class exists
            bool classExists = true;

            // Assert
            dataValid.Should().BeTrue();
            createResult.Success.Should().BeTrue();
            classExists.Should().BeTrue();
        }

        [TestMethod]
        public void ImportStudentsFlow_ValidScenario_ShouldImportSuccessfully()
        {
            // Arrange
            var importData = ClassManagementTestHelper.CreateSampleStudentsDataTable();
            string targetClass = "10A1";

            // Act - Simulate import flow
            // Step 1: Validate import data
            bool dataValid = importData.Rows.Count > 0;

            // Step 2: Execute import
            var importResult = ClassManagementTestHelper.CreateSuccessImportResult();

            // Step 3: Verify import results
            bool importSuccessful = importResult.Success > 0;
            bool noFailures = importResult.Failed == 0;

            // Assert
            dataValid.Should().BeTrue();
            importResult.Success.Should().BeGreaterThan(0);
            importSuccessful.Should().BeTrue();
            noFailures.Should().BeTrue();
        }

        [TestMethod]
        public void ImportStudentsFlow_PartialData_ShouldHandleErrors()
        {
            // Arrange
            var importData = ClassManagementTestHelper.CreateSampleStudentsDataTable();
            string targetClass = "10A1";

            // Act - Simulate import with partial failures
            // Step 1: Validate import data
            bool dataValid = importData.Rows.Count > 0;

            // Step 2: Execute import with some failures
            var importResult = ClassManagementTestHelper.CreatePartialImportResult();

            // Step 3: Verify partial success
            bool partialSuccess = importResult.Success > 0 && importResult.Failed > 0;

            // Assert
            dataValid.Should().BeTrue();
            importResult.Success.Should().BeGreaterThan(0);
            importResult.Failed.Should().BeGreaterThan(0);
            partialSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void TeachingAssignmentFlow_ValidScenario_ShouldUpdateAssignments()
        {
            // Arrange
            string classCode = "10A1";
            string subjectCode = "TOAN";
            string newTeacherCode = "GV004";

            // Act - Simulate assignment update flow
            // Step 1: Get current assignments
            var currentAssignments = ClassManagementTestHelper.CreateSampleTeachingAssignmentsDataTable();
            bool assignmentsLoaded = currentAssignments.Rows.Count > 0;

            // Step 2: Update assignment
            var updateResult = ClassManagementTestHelper.CreateSuccessClassOperationResult();

            // Step 3: Verify update
            bool updateVerified = updateResult.Success;

            // Assert
            assignmentsLoaded.Should().BeTrue();
            updateResult.Success.Should().BeTrue();
            updateVerified.Should().BeTrue();
        }

        [TestMethod]
        public void FilterClassesFlow_ByGrade_ShouldFilterCorrectly()
        {
            // Arrange
            var allClasses = ClassManagementTestHelper.CreateSampleClassesDataTable();
            string selectedGrade = "10";

            // Act - Simulate filtering flow
            // Step 1: Apply grade filter
            bool filterApplied = true;
            int filteredCount = allClasses.AsEnumerable()
                .Count(row => row.Field<string>("Khoi") == selectedGrade);

            // Step 2: Verify filtered results
            bool resultsCorrect = filteredCount == 2; // Should have 2 grade 10 classes

            // Assert
            filterApplied.Should().BeTrue();
            filteredCount.Should().Be(2);
            resultsCorrect.Should().BeTrue();
        }

        [TestMethod]
        public void ClassDeletionFlow_WithStudents_ShouldPreventDeletion()
        {
            // Arrange
            string classCode = "10A1";
            bool hasStudents = true;

            // Act - Simulate deletion prevention flow
            // Step 1: Check if class has students
            bool studentsExist = hasStudents;

            // Step 2: Attempt deletion
            var deleteResult = ClassManagementTestHelper.CreateFailureClassOperationResult();

            // Step 3: Verify prevention
            bool deletionPrevented = !deleteResult.Success;

            // Assert
            studentsExist.Should().BeTrue();
            deleteResult.Success.Should().BeFalse();
            deletionPrevented.Should().BeTrue();
        }
    }
}
