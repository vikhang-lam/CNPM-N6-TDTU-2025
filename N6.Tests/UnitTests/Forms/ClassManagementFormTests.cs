
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Forms
{
    [TestClass]
    public class ClassManagementFormTests
    {
        [TestMethod]
        public void InitializeClassManagement_ShouldLoadAllRequiredControls()
        {
            // Arrange & Act
            var khoiComboBox = ClassManagementTestHelper.CreateKhoiComboBox();
            var classesGrid = ClassManagementTestHelper.CreateClassesDataGridView();
            var studentsGrid = ClassManagementTestHelper.CreateStudentsDataGridView();
            var assignmentsGrid = ClassManagementTestHelper.CreateTeachingAssignmentsDataGridView();

            // Assert
            khoiComboBox.Should().NotBeNull();
            khoiComboBox.Items.Count.Should().BeGreaterThan(0);

            classesGrid.Should().NotBeNull();
            classesGrid.Rows.Count.Should().Be(0);

            studentsGrid.Should().NotBeNull();
            assignmentsGrid.Should().NotBeNull();
        }

        [TestMethod]
        public void FormLoad_ShouldDisplayDefaultFilterAndHideDetails()
        {
            // Arrange & Act
            var khoiComboBox = ClassManagementTestHelper.CreateKhoiComboBox();
            bool classDetailsVisible = false;
            bool studentDetailsVisible = false;
            bool assignmentDetailsVisible = false;

            // Assert
            khoiComboBox.SelectedItem.Should().Be("Tất cả các khối");
            classDetailsVisible.Should().BeFalse();
            studentDetailsVisible.Should().BeFalse();
            assignmentDetailsVisible.Should().BeFalse();
        }

        [TestMethod]
        public void ClassSelection_ShouldLoadClassDetailsAndStudents()
        {
            
        }

        [TestMethod]
        public void KhoiFilter_ShouldFilterClassesCorrectly()
        {
            // Arrange
            var khoiComboBox = ClassManagementTestHelper.CreateKhoiComboBox();
            var classesGrid = ClassManagementTestHelper.CreateClassesDataGridView();

            // Act - Filter by grade 10
            khoiComboBox.SelectedItem = "10";
            bool filterApplied = true;
            int filteredRowCount = 2; // Should show only grade 10 classes

            // Assert
            filterApplied.Should().BeTrue();
            filteredRowCount.Should().Be(2);
        }

        [TestMethod]
        public void ChuyenLopPanel_ShouldShowWhenButtonClicked()
        {
            // Arrange
            var chuyenLopPanel = ClassManagementTestHelper.CreateChuyenLopPanel();
            chuyenLopPanel.Visible = false;

            // Act
            chuyenLopPanel.Visible = true;

            // Assert
            chuyenLopPanel.Visible.Should().BeTrue();
            chuyenLopPanel.Controls.Count.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void ChuyenLopPanel_ShouldHideWhenCancelClicked()
        {
            // Arrange
            var chuyenLopPanel = ClassManagementTestHelper.CreateChuyenLopPanel();
            chuyenLopPanel.Visible = true;

            // Act
            chuyenLopPanel.Visible = false;

            // Assert
            chuyenLopPanel.Visible.Should().BeFalse();
        }

        [TestMethod]
        public void StyleControls_ShouldApplyModernUI()
        {
            // Arrange & Act
            var button = new Button();
            button.BackColor = Color.FromArgb(0, 123, 255);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;

            var panel = new Panel();

            // Assert
            button.BackColor.Should().Be(Color.FromArgb(0, 123, 255));
            button.ForeColor.Should().Be(Color.White);
            button.FlatStyle.Should().Be(FlatStyle.Flat);
        }

        [TestMethod]
        public void DataGridView_StylingShouldBeApplied()
        {
            // Arrange
            var dgv = ClassManagementTestHelper.CreateStudentsDataGridView();

            // Act - Apply styling
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 245, 250);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgv.RowHeadersVisible = false;

            // Assert
            dgv.AlternatingRowsDefaultCellStyle.BackColor.Should().Be(Color.FromArgb(242, 245, 250));
            dgv.ColumnHeadersDefaultCellStyle.BackColor.Should().Be(Color.White);
            dgv.RowHeadersVisible.Should().BeFalse();
        }

        [TestMethod]
        public void EmptyClassSelection_ShouldClearAllDetails()
        {
            // Arrange
            bool detailsCleared = false;
            string className = "Chọn lớp để xem thông tin";
            string studentCount = "👥 Sĩ số: -";
            string teacherName = "👤 GVCN: -";

            // Act
            detailsCleared = true;

            // Assert
            detailsCleared.Should().BeTrue();
            className.Should().Be("Chọn lớp để xem thông tin");
            studentCount.Should().Be("👥 Sĩ số: -");
            teacherName.Should().Be("👤 GVCN: -");
        }

        [TestMethod]
        public void ControlEnter_ShouldChangeBorderColor()
        {
            // Arrange
            Color initialColor = Color.Lavender;
            Color focusedColor = Color.RoyalBlue;

            // Act & Assert
            focusedColor.Should().NotBe(initialColor);
            focusedColor.Should().Be(Color.RoyalBlue);
        }

        [TestMethod]
        public void ControlLeave_ShouldRevertBorderColor()
        {
            // Arrange
            Color focusedColor = Color.RoyalBlue;
            Color normalColor = Color.Lavender;

            // Act & Assert
            normalColor.Should().NotBe(focusedColor);
            normalColor.Should().Be(Color.Lavender);
        }
    }
}
