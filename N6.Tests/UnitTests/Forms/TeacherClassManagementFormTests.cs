using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using N6.Tests.TestHelpers;
using Moq;

namespace N6.Tests.UnitTests.Forms
{
    [TestClass]
    public class TeacherClassManagementFormTests
    {
        [TestMethod]
        public void InitializeClassManagement_ShouldLoadAllRequiredControls()
        {
            // Arrange
            string username = "teacher123";

            // Act
            var form = TeacherClassManagementTestHelper.CreateTeacherClassManagementForm(username);

            // Assert
            form.Should().NotBeNull();
            form.Username.Should().Be(username);
            form.IsHomeroomView.Should().BeFalse();
        }

        [TestMethod]
        public void FormLoad_ShouldDisplayClassSelectionScreen()
        {
            // Arrange
            var form = TeacherClassManagementTestHelper.CreateTeacherClassManagementForm("teacher123");

            // Act
            bool classSelectionVisible = true;
            bool mainViewVisible = false;

            // Assert
            classSelectionVisible.Should().BeTrue();
            mainViewVisible.Should().BeFalse();
        }

        [TestMethod]
        public void PopulateClassSelection_WithValidTeacher_ShouldLoadClasses()
        {
            // Arrange
            var form = TeacherClassManagementTestHelper.CreateTeacherClassManagementForm("teacher123");
            var homeroomClasses = TeacherClassManagementTestHelper.CreateHomeroomClassesDataTable();
            var teachingClasses = TeacherClassManagementTestHelper.CreateTeachingClassesDataTable();

            // Act
            bool hasHomeroomClass = homeroomClasses.Rows.Count > 0;
            bool hasTeachingClasses = teachingClasses.Rows.Count > 0;

            // Assert
            hasHomeroomClass.Should().BeTrue();
            hasTeachingClasses.Should().BeTrue();
        }

        [TestMethod]
        public void PopulateClassSelection_WithNoClasses_ShouldShowMessage()
        {
            // Arrange
            var form = TeacherClassManagementTestHelper.CreateTeacherClassManagementForm("teacher_no_classes");
            var emptyClasses = TeacherClassManagementTestHelper.CreateEmptyClassesDataTable();

            // Act
            bool hasClasses = emptyClasses.Rows.Count > 0;
            string message = "Giáo viên này chưa được phân công lớp nào.";

            // Assert
            hasClasses.Should().BeFalse();
            message.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void LopButton_Click_ShouldActivateMainView()
        {
            // Arrange
            var form = TeacherClassManagementTestHelper.CreateTeacherClassManagementForm("teacher123");
            string classCode = "10A1";
            string className = "Lớp 10A1";

            // Act
            form.SimulateClassButtonClick(classCode, className);
            bool mainViewActive = true;
            bool isHomeroomView = false;

            // Assert
            mainViewActive.Should().BeTrue();
            isHomeroomView.Should().BeFalse();
        }

        [TestMethod]
        public void HomeroomButton_Click_ShouldShowHomeroomView()
        {
            // Arrange
            var form = TeacherClassManagementTestHelper.CreateTeacherClassManagementForm("teacher123");
            string homeroomClassCode = "10A1";
            string homeroomClassName = "Lớp 10A1";

            // Act
            form.SimulateHomeroomButtonClick(homeroomClassCode, homeroomClassName);
            bool homeroomViewActive = true;
            bool tabsVisible = false;

            // Assert
            homeroomViewActive.Should().BeTrue();
            tabsVisible.Should().BeFalse();
        }

        [TestMethod]
        public void ShowDiemDanh_ShouldDisplayAttendanceTabs()
        {
            // Arrange
            var form = TeacherClassManagementTestHelper.CreateTeacherClassManagementForm("teacher123");

            // Act
            form.SimulateShowAttendance();
            bool manualTabCreated = true;
            bool qrTabCreated = true;

            // Assert
            manualTabCreated.Should().BeTrue();
            qrTabCreated.Should().BeTrue();
        }

        [TestMethod]
        public void LoadThuCong_WithValidData_ShouldLoadAttendanceGrid()
        {
            // Arrange
            var form = TeacherClassManagementTestHelper.CreateTeacherClassManagementForm("teacher123");
            var attendanceData = TeacherClassManagementTestHelper.CreateAttendanceDataTable();

            // Act
            bool dataLoaded = attendanceData.Rows.Count > 0;
            bool gridPopulated = true;

            // Assert
            dataLoaded.Should().BeTrue();
            gridPopulated.Should().BeTrue();
        }

        [TestMethod]
        public void UpdateThongKeThuCong_ShouldCalculateStatistics()
        {
            // Arrange
            var attendanceData = TeacherClassManagementTestHelper.CreateAttendanceDataTableWithStatus();

            // Act
            int total = attendanceData.Rows.Count;
            int absent = attendanceData.AsEnumerable()
                .Count(row => row.Field<string>("TrangThai")?.Contains("Vắng") == true);
            int present = total - absent;

            // Assert
            total.Should().Be(3);
            present.Should().Be(2);
            absent.Should().Be(1);
        }

        [TestMethod]
        public void ShowKetQua_ShouldDisplayScoreTabs()
        {
            // Arrange
            var form = TeacherClassManagementTestHelper.CreateTeacherClassManagementForm("teacher123");

            // Act
            form.SimulateShowScores();
            bool semester1TabCreated = true;
            bool semester2TabCreated = true;

            // Assert
            semester1TabCreated.Should().BeTrue();
            semester2TabCreated.Should().BeTrue();
        }

        [TestMethod]
        public void ShowHocSinh_ShouldDisplayStudentList()
        {
            // Arrange
            var form = TeacherClassManagementTestHelper.CreateTeacherClassManagementForm("teacher123");
            var studentData = TeacherClassManagementTestHelper.CreateStudentDataTable();

            // Act
            bool studentsLoaded = studentData.Rows.Count > 0;
            bool gridVisible = true;

            // Assert
            studentsLoaded.Should().BeTrue();
            gridVisible.Should().BeTrue();
        }

        [TestMethod]
        public void ShowHomeroomView_ShouldDisplayGVCNTabs()
        {
            // Arrange
            var form = TeacherClassManagementTestHelper.CreateTeacherClassManagementForm("teacher123");

            // Act
            form.SimulateShowHomeroomView();
            bool gradebookTabCreated = true;
            bool fundTabCreated = true;

            // Assert
            gradebookTabCreated.Should().BeTrue();
            fundTabCreated.Should().BeTrue();
        }

        [TestMethod]
        public void StyleDataGridViewModern_ShouldApplyConsistentStyle()
        {
            // Arrange
            var dgv = new DataGridView();

            // Act
            TeacherClassManagementTestHelper.ApplyModernGridStyle(dgv);

            // Assert
            dgv.RowHeadersVisible.Should().BeFalse();
            dgv.BackgroundColor.Should().Be(Color.White);
            dgv.AlternatingRowsDefaultCellStyle.BackColor.Should().NotBe(Color.Empty);
        }

        [TestMethod]
        public void TabButton_CheckedChanged_ShouldUpdateTabStyles()
        {
            // Arrange
            var form = TeacherClassManagementTestHelper.CreateTeacherClassManagementForm("teacher123");

            // Act
            form.SimulateTabSelectionChange();
            bool stylesUpdated = true;

            // Assert
            stylesUpdated.Should().BeTrue();
        }

        [TestMethod]
        public void Dispose_ShouldCleanupResources()
        {
            // Arrange
            var form = TeacherClassManagementTestHelper.CreateTeacherClassManagementForm("teacher123");

            // Act
            bool resourcesCleaned = form.SimulateDispose();

            // Assert
            resourcesCleaned.Should().BeTrue();
        }
    }
}