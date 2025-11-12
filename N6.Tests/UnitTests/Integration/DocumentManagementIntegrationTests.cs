using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Integration
{
    [TestClass]
    public class DocumentManagementIntegrationTests
    {
        [TestMethod]
        public void CompleteDocumentManagementFlow_ShouldWorkEndToEnd()
        {
            // Arrange
            string maGV = "GV001";

            // Act - Simulate complete document management flow
            // Step 1: Initialize UC
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC(maGV);
            bool ucInitialized = uc != null;

            // Step 2: Load documents
            bool documentsLoaded = uc.PersonalDocumentsLoaded && uc.SharedDocumentsLoaded;

            // Step 3: Upload document
            var testFile = DocumentManagementTestHelper.CreateTestFileInfo();
            bool uploadSuccessful = uc.SimulateUploadDocument(testFile);

            // Step 4: Share document
            bool shareSuccessful = uc.SimulateShareDocument("TL001");

            // Step 5: View document
            var documentRow = DocumentManagementTestHelper.CreateValidDocumentRow();
            bool viewSuccessful = uc.SimulateViewDocument(documentRow);

            // Assert
            ucInitialized.Should().BeTrue();
            documentsLoaded.Should().BeTrue();
            uploadSuccessful.Should().BeTrue();
            shareSuccessful.Should().BeTrue();
            viewSuccessful.Should().BeTrue();
        }

        [TestMethod]
        public void DocumentSharingFlow_ShouldUpdateBothTabs()
        {
            // Arrange
            string maGV = "GV001";
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC(maGV);

            // Act
            // Step 1: Share document
            bool shareSuccessful = uc.SimulateShareDocument("TL001");

            // Step 2: Refresh both tabs
            bool refreshSuccessful = uc.SimulateRefresh();

            // Step 3: Verify both tabs updated
            bool bothTabsUpdated = uc.PersonalDocumentsLoaded && uc.SharedDocumentsLoaded;

            // Assert
            shareSuccessful.Should().BeTrue();
            refreshSuccessful.Should().BeTrue();
            bothTabsUpdated.Should().BeTrue();
        }
    }
}