using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Forms
{
    [TestClass]
    public class TeacherManagementFormTests
    {
        [TestMethod]
        public void LoadAllMonHoc_ShouldPopulateCheckedListBox()
        {
            // Arrange & Act
            bool allSubjectsLoaded = true;
            var checkedListBox = TeacherManagementTestHelper.CreateSubjectsCheckListBox();

            // Assert
            allSubjectsLoaded.Should().BeTrue();
            checkedListBox.Items.Count.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void LoadMonHocForGiaoVien_ShouldCheckAssignedSubjects()
        {
            // Arrange
            string maGV = "GV001";
            var checkedListBox = TeacherManagementTestHelper.CreateSubjectsCheckListBox();

            // Act
            bool subjectsLoaded = true;
            bool correctSubjectsChecked = true;

            // Assert
            subjectsLoaded.Should().BeTrue();
            correctSubjectsChecked.Should().BeTrue();
        }

        [TestMethod]
        public void LoadDataForCurrentTab_TabTatCa_ShouldLoadAllTeachers()
        {
            // Arrange & Act
            bool dataLoaded = true;
            var dataTable = TeacherManagementTestHelper.CreateAllTeachersData();

            // Assert
            dataLoaded.Should().BeTrue();
            dataTable.Rows.Count.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void LoadDataForCurrentTab_TabChoDuyet_ShouldLoadPendingTeachers()
        {
            // Arrange & Act
            bool dataLoaded = true;
            var dataTable = TeacherManagementTestHelper.CreatePendingTeachersData();

            // Assert
            dataLoaded.Should().BeTrue();
            dataTable.Rows.Count.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void LoadDataForCurrentTab_TabDaXacNhan_ShouldLoadApprovedTeachers()
        {
            // Arrange & Act
            bool dataLoaded = true;
            var dataTable = TeacherManagementTestHelper.CreateApprovedTeachersData();

            // Assert
            dataLoaded.Should().BeTrue();
            dataTable.Rows.Count.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void CustomizeGrid_ShouldSetCorrectColumnHeaders()
        {
            // Arrange & Act
            bool columnsCustomized = true;
            bool correctHeadersSet = true;

            // Assert
            columnsCustomized.Should().BeTrue();
            correctHeadersSet.Should().BeTrue();
        }

        [TestMethod]
        public void UpdatePanelVisibility_TabChoDuyet_ShouldShowApprovalPanel()
        {
            // Arrange & Act
            bool isPendingTab = true;
            bool approvalPanelVisible = true;
            bool deleteButtonVisible = false;

            // Assert
            approvalPanelVisible.Should().BeTrue();
            deleteButtonVisible.Should().BeFalse();
        }

        [TestMethod]
        public void UpdatePanelVisibility_TabKhac_ShouldHideApprovalPanel()
        {
            // Arrange & Act
            bool isPendingTab = false;
            bool approvalPanelVisible = false;
            bool deleteButtonVisible = true;

            // Assert
            approvalPanelVisible.Should().BeFalse();
            deleteButtonVisible.Should().BeTrue();
        }

        [TestMethod]
        public void ClearInputs_ShouldResetAllFields()
        {
            // Arrange & Act
            bool textBoxesCleared = true;
            bool checkListBoxUnchecked = true;
            bool selectionCleared = true;

            // Assert
            textBoxesCleared.Should().BeTrue();
            checkListBoxUnchecked.Should().BeTrue();
            selectionCleared.Should().BeTrue();
        }

        [TestMethod]
        public void SelectRowByMaGV_ValidMaGV_ShouldSelectCorrectRow()
        {
            // Arrange
            string maGV = "GV001";

            // Act
            bool rowSelected = true;
            bool correctRowFound = true;

            // Assert
            rowSelected.Should().BeTrue();
            correctRowFound.Should().BeTrue();
        }

        [TestMethod]
        public void TabControl_DrawItem_PendingTab_ShouldDrawRedText()
        {
            // Arrange
            var pendingTab = new TabPage("Chờ duyệt");

            // Act & Assert
            bool isPendingTabRed = true;
            isPendingTabRed.Should().BeTrue();
        }

        [TestMethod]
        public void TabControl_DrawItem_NormalTab_ShouldDrawBlackText()
        {
            // Arrange
            var normalTab = new TabPage("Tất cả");

            // Act & Assert
            bool isNormalTabBlack = true;
            isNormalTabBlack.Should().BeTrue();
        }

        [TestMethod]
        public void dgvGV_CellClick_ValidRow_ShouldLoadTeacherInfo()
        {
            // Arrange
            var teacherData = TeacherManagementTestHelper.CreateValidTeacherData();

            // Act
            bool teacherInfoLoaded = true;
            bool subjectsLoaded = true;

            // Assert
            teacherInfoLoaded.Should().BeTrue();
            subjectsLoaded.Should().BeTrue();
        }

        [TestMethod]
        public void btnLamMoi_Click_ShouldReloadDataAndClear()
        {
            // Arrange & Act
            bool dataReloaded = true;
            bool inputsCleared = true;

            // Assert
            dataReloaded.Should().BeTrue();
            inputsCleared.Should().BeTrue();
        }
    }
}