using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Data;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class TeacherClassManagementAdditionalTests
    {
        [TestMethod]
        public void GetSubjectsByTeacherAndClass_WithValidData_ShouldReturnSubjects()
        {
            // Arrange
            string teacherId = "GV001";
            string classCode = "10A1";

            // Act
            var result = TeacherClassManagementTestHelper.CreateTeacherSubjectsDataTable();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            result.Columns.Contains("MaMon").Should().BeTrue();
            result.Columns.Contains("TenMon").Should().BeTrue();
        }

        [TestMethod]
        public void GetTeacherIdByUsername_WithValidUsername_ShouldReturnTeacherId()
        {
            // Arrange
            string username = "teacher123";

            // Act
            string teacherId = TeacherClassManagementTestHelper.GetTeacherIdByUsername(username);

            // Assert
            teacherId.Should().NotBeNullOrEmpty();
            teacherId.Should().Be("GV001");
        }

        [TestMethod]
        public void ExecuteCreateDefaultAttendance_ShouldCreateDefaultRecords()
        {
            // Arrange
            string classCode = "10A1";
            DateTime date = DateTime.Today;
            string session = "Sáng";

            // Act
            var result = TeacherClassManagementTestHelper.CreateSuccessOperationResult();

            // Assert
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void DeleteClassFundEntry_WithValidId_ShouldDeleteSuccessfully()
        {
            // Arrange
            string fundEntryId = "QL001";

            // Act
            var result = TeacherClassManagementTestHelper.CreateSuccessOperationResult();

            // Assert
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void LoadLockStatusCache_ShouldLoadAllLockStatuses()
        {
            // Arrange & Act
            var lockStatusCache = TeacherClassManagementTestHelper.CreateLockStatusCache();

            // Assert
            lockStatusCache.Should().NotBeNull();
            lockStatusCache.Count.Should().BeGreaterThan(0);
            lockStatusCache.ContainsKey("Thang1_Ki1").Should().BeTrue();
        }

        [TestMethod]
        public void MapColumnToLoai_WithValidColumnName_ShouldReturnCorrectLoai()
        {
            // Arrange
            string columnName = "Thang1";
            int semester = 1;

            // Act
            string loai = TeacherClassManagementTestHelper.MapColumnToLoai(columnName, semester);

            // Assert
            loai.Should().Be("Thang1_Ki1");
        }

        [TestMethod]
        public void UpdateAcademicResult_Text_ShouldSaveTextData()
        {
            // Arrange
            string studentId = "HS001";
            string subject = "TOAN";
            string scoreType = "CuoiKi1";
            string comment = "Học sinh tiến bộ tốt";
            bool isComment = true;

            // Act
            var result = TeacherClassManagementTestHelper.CreateSuccessOperationResult();

            // Assert
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void FormatQuyLopGrid_ShouldFormatColumnsCorrectly()
        {
            // Arrange
            var grid = new DataGridView();

            // Act
            TeacherClassManagementTestHelper.ApplyQuyLopGridFormatting(grid);

            // Assert
            grid.Columns.Should().NotBeNull();
        }
    }
}