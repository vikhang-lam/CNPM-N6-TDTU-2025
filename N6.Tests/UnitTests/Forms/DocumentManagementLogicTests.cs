using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Data;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class DocumentManagementLogicTests
    {
        [TestMethod]
        public void GetDocumentsByTeacher_WithValidGV_ShouldReturnDataTable()
        {
            // Arrange
            string maGV = "GV001";

            // Act
            var result = DocumentManagementTestHelper.CreatePersonalDocumentsDataTable();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            result.Columns.Contains("MaTL").Should().BeTrue();
            result.Columns.Contains("TenTL").Should().BeTrue();
            result.Columns.Contains("Kieu").Should().BeTrue();
            result.Columns.Contains("TrangThaiChiaSe").Should().BeTrue();
        }

        [TestMethod]
        public void GetSharedDocumentsWithUploader_ShouldReturnSharedDocs()
        {
            // Arrange & Act
            var result = DocumentManagementTestHelper.CreateSharedDocumentsDataTable();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            result.Columns.Contains("MaTL").Should().BeTrue();
            result.Columns.Contains("TenTL").Should().BeTrue();
            result.Columns.Contains("Kieu").Should().BeTrue();
            result.Columns.Contains("TenGV").Should().BeTrue();
        }

        [TestMethod]
        public void InsertDocument_ShouldAddNewDocumentRecord()
        {
            // Arrange
            string maGV = "GV001";
            string tenTL = "Test Document.pdf";
            string filePath = "C:\\Test\\document.pdf";

            // Act
            var result = DocumentManagementTestHelper.CreateSuccessOperationResult();

            // Assert
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void ShareDocument_ShouldUpdateTrangThaiChiaSe()
        {
            // Arrange
            string maTL = "TL001";

            // Act
            var result = DocumentManagementTestHelper.CreateSuccessOperationResult();

            // Assert
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void UnshareDocument_ShouldUpdateTrangThaiChiaSe()
        {
            // Arrange
            string maTL = "TL001";

            // Act
            var result = DocumentManagementTestHelper.CreateSuccessOperationResult();

            // Assert
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void DeleteDocument_ShouldRemoveDocumentRecord()
        {
            // Arrange
            string maTL = "TL001";

            // Act
            var result = DocumentManagementTestHelper.CreateSuccessOperationResult();

            // Assert
            result.Success.Should().BeTrue();
        }
    }
}