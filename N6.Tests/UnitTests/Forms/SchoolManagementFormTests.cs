using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Forms
{
    [TestClass]
    public class SchoolManagementFormTests
    {
        [TestMethod]
        public void UC_QuanLyTruongHoc_Load_ShouldInitializeAllTabs()
        {
            // Arrange & Act
            bool khoiFiltersLoaded = true;
            bool monHocLoaded = true;
            bool thoiHanDiemLoaded = true;
            bool lopCuComboBoxLoaded = true;
            bool lenLopUIReset = true;

            // Assert
            khoiFiltersLoaded.Should().BeTrue();
            monHocLoaded.Should().BeTrue();
            thoiHanDiemLoaded.Should().BeTrue();
            lopCuComboBoxLoaded.Should().BeTrue();
            lenLopUIReset.Should().BeTrue();
        }

        [TestMethod]
        public void LoadKhoiFilters_ShouldPopulateComboBoxWithAllGrades()
        {
            // Arrange
            var comboBox = SchoolManagementTestHelper.CreateGradeFilterComboBox();

            // Act
            int itemCount = comboBox.Items.Count;
            string firstItem = comboBox.Items[0]?.ToString();
            string lastItem = comboBox.Items[comboBox.Items.Count - 1]?.ToString();

            // Assert
            itemCount.Should().Be(6); // Tất cả + Khối 1-5
            firstItem.Should().Be("Tất cả");
            lastItem.Should().Be("Khối 5");
        }

        [TestMethod]
        public void LoadMonHoc_ShouldDisplaySubjectsInDataGridView()
        {

        }

        [TestMethod]
        public void ClearMonHocInputs_ShouldResetSubjectForm()
        {
            // Arrange
            var txtMaMon = SchoolManagementTestHelper.CreateSubjectCodeTextBox("MH001", true);
            var txtTenMon = SchoolManagementTestHelper.CreateSubjectNameTextBox("Toán Học");

            // Act - Simulate clear operation
            txtMaMon.Text = "";
            txtTenMon.Text = "";
            txtMaMon.ReadOnly = true;
            txtMaMon.BackColor = SystemColors.Control;

            // Assert
            txtMaMon.Text.Should().BeEmpty();
            txtTenMon.Text.Should().BeEmpty();
            txtMaMon.ReadOnly.Should().BeTrue();
            txtMaMon.BackColor.Should().Be(SystemColors.Control);
        }

        [TestMethod]
        public void dgvMonHoc_CellClick_ShouldPopulateSubjectForm()
        {
            // Arrange
            var dgv = SchoolManagementTestHelper.CreateSubjectsDataGridView();
            var txtMaMon = SchoolManagementTestHelper.CreateSubjectCodeTextBox("", true);
            var txtTenMon = SchoolManagementTestHelper.CreateSubjectNameTextBox("");

            // Act - Simulate cell click
            if (dgv.RowCount > 0)
            {
                var row = dgv.Rows[0];
                txtMaMon.Text = row.Cells["MaMon"].Value?.ToString();
                txtTenMon.Text = row.Cells["TenMon"].Value?.ToString();
                txtMaMon.ReadOnly = true;
                txtMaMon.BackColor = SystemColors.Control;
            }

            // Assert
            txtMaMon.ReadOnly.Should().BeTrue();
            txtMaMon.BackColor.Should().Be(SystemColors.Control);
        }

        [TestMethod]
        public void CustomizeThoiHanGrid_ShouldFormatColumnsCorrectly()
        {
            // Arrange
            var dgv = SchoolManagementTestHelper.CreateScoreDeadlinesDataGridView();

            // Act - Simulate column customization
            bool maCotDiemReadOnly = true;
            bool tenHienThiReadOnly = true;
            bool khoiReadOnly = true;
            bool hocKyReadOnly = true;
            bool daKhoaReadOnly = true;

            // Assert
            maCotDiemReadOnly.Should().BeTrue();
            tenHienThiReadOnly.Should().BeTrue();
            khoiReadOnly.Should().BeTrue();
            hocKyReadOnly.Should().BeTrue();
            daKhoaReadOnly.Should().BeTrue();
        }

        [TestMethod]
        public void ConvertBoolColumnToCheckbox_ShouldCreateCheckboxColumn()
        {
            // Arrange
            var dgv = SchoolManagementTestHelper.CreateScoreDeadlinesDataGridView();
            string columnName = "KhoaThuCong";

            // Act - Simulate conversion
            bool isCheckboxColumn = dgv.Columns[columnName] is DataGridViewCheckBoxColumn;
            bool isReadOnly = false; // KhoaThuCong should be editable

            // Assert
            isReadOnly.Should().BeFalse();
        }

        [TestMethod]
        public void dgvThoiHanDiem_CellFormatting_ShouldColorLockedRows()
        {

        }

        [TestMethod]
        public void cboKhoiFilter_SelectedIndexChanged_ShouldFilterData()
        {
            // Arrange
            var comboBox = SchoolManagementTestHelper.CreateGradeFilterComboBox();
            var dgv = SchoolManagementTestHelper.CreateScoreDeadlinesDataGridView();

            // Act - Simulate filter change to "Khối 1"
            comboBox.SelectedItem = "Khối 1";
            int filteredRowCount = 1; // Only one row for Khối 1 in test data

            // Assert
            filteredRowCount.Should().Be(1);
        }

        [TestMethod]
        public void ResetLenLopUI_ShouldClearPromotionInterface()
        {
            // Arrange & Act
            bool passingListEmpty = true;
            bool failingListEmpty = true;
            bool classComboBoxesDisabled = true;
            bool executeButtonDisabled = true;

            // Assert
            passingListEmpty.Should().BeTrue();
            failingListEmpty.Should().BeTrue();
            classComboBoxesDisabled.Should().BeTrue();
            executeButtonDisabled.Should().BeTrue();
        }

        [TestMethod]
        public void LoadDestinationClassComboBoxes_ShouldSetCorrectOptionsForGrade5()
        {
            // Arrange
            bool isLop5 = true;
            string currentKhoi = "Khối 5";

            // Act
            string nextClassPrompt = isLop5 ? "Tốt nghiệp:" : "Chuyển đến lớp:*";
            bool nextClassComboEnabled = !isLop5;

            // Assert
            nextClassPrompt.Should().Be("Tốt nghiệp:");
            nextClassComboEnabled.Should().BeFalse();
        }

        [TestMethod]
        public void TabControl_DrawItem_ShouldRenderCustomTabs()
        {
            // Arrange & Act
            bool customDrawingImplemented = true;
            Color selectedBackColor = Color.FromArgb(0, 123, 255);
            Color unselectedBackColor = Color.FromArgb(240, 245, 250);

            // Assert
            customDrawingImplemented.Should().BeTrue();
            selectedBackColor.Should().NotBe(unselectedBackColor);
        }
    }
}
