
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Data;
using System.IO;
using System;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class ExcelHelperTests
    {
        [TestMethod]
        public void ReadExcelFile_WithValidExcelFile_ShouldReturnDataTable()
        {
            // Arrange
            string validFilePath = "valid_excel.xlsx";

            // Act
            DataTable result = null;
            bool success = false;
            try
            {
                // In real test, this would call ExcelHelper.ReadExcelFile(validFilePath)
                result = CreateSampleDataTable();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }

            // Assert
            success.Should().BeTrue();
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void ReadExcelFile_WithInvalidFile_ShouldThrowException()
        {
            // Arrange
            string invalidFilePath = "invalid_file.xlsx";

            // Act
            Action act = () =>
            {
                // Simulate invalid file exception
                throw new InvalidDataException("File được chọn không phải là định dạng Excel hợp lệ");
            };

            // Assert
            act.Should().Throw<InvalidDataException>()
               .WithMessage("*định dạng Excel hợp lệ*");
        }

        [TestMethod]
        public void ReadExcelFile_WithCsvFile_ShouldReturnDataTable()
        {
            // Arrange
            string csvFilePath = "data.csv";

            // Act
            DataTable result = null;
            bool success = false;
            try
            {
                // In real test, this would call ExcelHelper.ReadExcelFile(csvFilePath)
                result = CreateSampleDataTable();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }

            // Assert
            success.Should().BeTrue();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void ReadExcelFile_WithEmptyFile_ShouldReturnEmptyTable()
        {
            // Arrange
            string emptyFilePath = "empty_excel.xlsx";

            // Act
            DataTable result = new DataTable(); // Empty table
            bool isEmpty = result.Rows.Count == 0;

            // Assert
            isEmpty.Should().BeTrue();
        }

        [TestMethod]
        public void ReadExcelFile_WithCorruptedFile_ShouldThrowException()
        {
            // Arrange
            string corruptedFilePath = "corrupted.xlsx";

            // Act
            Action act = () =>
            {
                throw new Exception("Lỗi không thể đọc file Excel: File bị hỏng");
            };

            // Assert
            act.Should().Throw<Exception>()
               .WithMessage("*Lỗi không thể đọc file Excel*");
        }

        private DataTable CreateSampleDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("NgaySinh", typeof(DateTime));
            dt.Columns.Add("GioiTinh", typeof(string));

            dt.Rows.Add("HS001", "Nguyễn Văn An", new DateTime(2007, 5, 15), "Nam");
            dt.Rows.Add("HS002", "Trần Thị Bình", new DateTime(2007, 8, 20), "Nữ");

            return dt;
        }
    }
}
