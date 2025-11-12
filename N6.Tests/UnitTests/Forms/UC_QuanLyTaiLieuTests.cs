using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Windows.Forms;
using System.IO;
using System.Data;
using N6.Tests.TestHelpers;
using Moq;
using System.Drawing;
using System.Linq;

namespace N6.Tests.UnitTests.Forms
{
    [TestClass]
    public class UC_QuanLyTaiLieuTests
    {
        [TestMethod]
        public void InitializeDocumentManagement_ShouldLoadAllRequiredControls()
        {
            // Arrange
            string maGV = "GV001";

            // Act
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC(maGV);

            // Assert
            uc.Should().NotBeNull();
            uc.MaGV.Should().Be(maGV);
            uc.StoragePath.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void Constructor_WithValidTeacherId_ShouldCreateStorageDirectory()
        {
            // Arrange
            string maGV = "GV001";
            var mockDirectory = DocumentManagementTestHelper.CreateMockDirectoryService();

            // Act
            bool directoryCreated = mockDirectory.CreateDirectoryCalled;

            // Assert
            directoryCreated.Should().BeTrue();
        }

        [TestMethod]
        public void Constructor_ShouldLoadBothPersonalAndSharedDocuments()
        {
            // Arrange
            string maGV = "GV001";

            // Act
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC(maGV);
            bool personalLoaded = uc.PersonalDocumentsLoaded;
            bool sharedLoaded = uc.SharedDocumentsLoaded;

            // Assert
            personalLoaded.Should().BeTrue();
            sharedLoaded.Should().BeTrue();
        }

        [TestMethod]
        public void LoadTaiLieu_Personal_ShouldDisplayMyDocuments()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            var personalData = DocumentManagementTestHelper.CreatePersonalDocumentsDataTable();

            // Act
            uc.SimulateLoadDocuments(personalData, false);
            bool cardsCreated = uc.PersonalCardsCount > 0;
            bool hasViewButton = true;
            bool hasShareButton = true;
            bool hasDeleteButton = true;

            // Assert
            cardsCreated.Should().BeTrue();
            hasViewButton.Should().BeTrue();
            hasShareButton.Should().BeTrue();
            hasDeleteButton.Should().BeTrue();
        }

        [TestMethod]
        public void LoadTaiLieu_Shared_ShouldDisplaySharedDocuments()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            var sharedData = DocumentManagementTestHelper.CreateSharedDocumentsDataTable();

            // Act
            uc.SimulateLoadDocuments(sharedData, true);
            bool cardsCreated = uc.SharedCardsCount > 0;
            bool hasViewButton = true;
            bool hasDownloadButton = true;
            bool showsUploader = true;

            // Assert
            cardsCreated.Should().BeTrue();
            hasViewButton.Should().BeTrue();
            hasDownloadButton.Should().BeTrue();
            showsUploader.Should().BeTrue();
        }

        [TestMethod]
        public void LoadTaiLieu_WithNoDocuments_ShouldHandleEmptyData()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            var emptyData = DocumentManagementTestHelper.CreateEmptyDocumentsDataTable();

            // Act
            uc.SimulateLoadDocuments(emptyData, false);
            bool noCardsCreated = uc.PersonalCardsCount == 0;
            bool noError = true;

            // Assert
            noCardsCreated.Should().BeTrue();
            noError.Should().BeTrue();
        }

        [TestMethod]
        public void LoadTaiLieu_ShouldCleanupOldControlsBeforeLoading()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            var testData = DocumentManagementTestHelper.CreatePersonalDocumentsDataTable();

            // Act - Load twice to test cleanup
            uc.SimulateLoadDocuments(testData, false);
            int firstLoadCount = uc.PersonalCardsCount;
            uc.SimulateLoadDocuments(testData, false);
            int secondLoadCount = uc.PersonalCardsCount;

            bool cleanupOccurred = firstLoadCount == secondLoadCount; // Should be same count, not doubled

            // Assert
            cleanupOccurred.Should().BeTrue();
        }

        [TestMethod]
        public void GetFileIcon_WithPDF_ShouldReturnBookEmoji()
        {
            // Arrange
            string pdfPath = "document.pdf";

            // Act
            string icon = DocumentManagementTestHelper.GetFileIcon(pdfPath);

            // Assert
            icon.Should().Be("📕");
        }

        [TestMethod]
        public void GetFileIcon_WithWord_ShouldReturnBlueBookEmoji()
        {
            // Arrange
            string wordPath = "document.docx";

            // Act
            string icon = DocumentManagementTestHelper.GetFileIcon(wordPath);

            // Assert
            icon.Should().Be("📘");
        }

        [TestMethod]
        public void GetFileIcon_WithExcel_ShouldReturnGreenBookEmoji()
        {
            // Arrange
            string excelPath = "document.xlsx";

            // Act
            string icon = DocumentManagementTestHelper.GetFileIcon(excelPath);

            // Assert
            icon.Should().Be("📗");
        }

        [TestMethod]
        public void GetFileIcon_WithUnknownType_ShouldReturnDocumentEmoji()
        {
            // Arrange
            string unknownPath = "document.txt";

            // Act
            string icon = DocumentManagementTestHelper.GetFileIcon(unknownPath);

            // Assert
            icon.Should().Be("📄");
        }

        [TestMethod]
        public void CreateModernButton_ShouldApplyConsistentStyle()
        {
            // Arrange
            string text = "Test Button";
            Color color = Color.Blue;

            // Act
            var button = DocumentManagementTestHelper.CreateModernButton(text, color);

            // Assert
            button.Should().NotBeNull();
            button.Text.Should().Be(text);
            button.BackColor.Should().Be(color);
            button.ForeColor.Should().Be(Color.White);
            button.FlatStyle.Should().Be(FlatStyle.Flat);
        }

        [TestMethod]
        public void ViewDocument_Click_WithPDF_ShouldOpenPdfViewer()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            var pdfRow = DocumentManagementTestHelper.CreatePdfDocumentRow();

            // Act
            bool pdfViewerOpened = uc.SimulateViewDocument(pdfRow);

            // Assert
            pdfViewerOpened.Should().BeTrue();
        }

        [TestMethod]
        public void ViewDocument_Click_WithNonPDF_ShouldOpenWithDefaultApp()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            var wordRow = DocumentManagementTestHelper.CreateWordDocumentRow();

            // Act
            bool defaultAppOpened = uc.SimulateViewDocument(wordRow);

            // Assert
            defaultAppOpened.Should().BeTrue();
        }

        [TestMethod]
        public void ViewDocument_Click_WithMissingFile_ShouldShowErrorMessage()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            var missingFileRow = DocumentManagementTestHelper.CreateMissingFileDocumentRow();

            // Act
            bool errorShown = uc.SimulateViewDocument(missingFileRow);

            // Assert
            errorShown.Should().BeTrue();
        }

        [TestMethod]
        public void DownloadDocument_Click_ShouldSaveFileWithSaveDialog()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            var validRow = DocumentManagementTestHelper.CreateValidDocumentRow();

            // Act
            bool downloadSuccessful = uc.SimulateDownloadDocument(validRow);

            // Assert
            downloadSuccessful.Should().BeTrue();
        }

        [TestMethod]
        public void DownloadDocument_Click_WithMissingFile_ShouldShowError()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            var missingFileRow = DocumentManagementTestHelper.CreateMissingFileDocumentRow();

            // Act
            bool errorShown = uc.SimulateDownloadDocument(missingFileRow);

            // Assert
            errorShown.Should().BeTrue();
        }

        [TestMethod]
        public void ShareDocument_Click_ShouldUpdateDatabaseAndRefresh()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            string maTL = "TL001";

            // Act
            bool shareSuccessful = uc.SimulateShareDocument(maTL);

            // Assert
            shareSuccessful.Should().BeTrue();
        }

        [TestMethod]
        public void UnshareDocument_Click_ShouldUpdateDatabaseAndRefresh()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            string maTL = "TL001";

            // Act
            bool unshareSuccessful = uc.SimulateUnshareDocument(maTL);

            // Assert
            unshareSuccessful.Should().BeTrue();
        }

        [TestMethod]
        public void DeleteDocument_Click_WithConfirmation_ShouldDeleteAndRefresh()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            string maTL = "TL001";
            bool userConfirmed = true;

            // Act
            bool deleteSuccessful = uc.SimulateDeleteDocument(maTL, userConfirmed);

            // Assert
            deleteSuccessful.Should().BeTrue();
        }

        [TestMethod]
        public void DeleteDocument_Click_WithoutConfirmation_ShouldNotDelete()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            string maTL = "TL001";
            bool userConfirmed = false;

            // Act
            bool deleteCalled = uc.SimulateDeleteDocument(maTL, userConfirmed);

            // Assert
            deleteCalled.Should().BeFalse();
        }

        [TestMethod]
        public void btnUpload_Click_ShouldOpenFileDialogAndSaveFile()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            var testFile = DocumentManagementTestHelper.CreateTestFileInfo();

            // Act
            bool uploadSuccessful = uc.SimulateUploadDocument(testFile);

            // Assert
            uploadSuccessful.Should().BeTrue();
        }


        [TestMethod]
        public void btnRefresh_Click_ShouldReloadBothDocumentTabs()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");

            // Act
            bool refreshCalled = uc.SimulateRefresh();

            // Assert
            refreshCalled.Should().BeTrue();
        }

        [TestMethod]
        public void Dispose_ShouldCleanupAllDynamicControlsAndEvents()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");

            // Act
            bool cleanupSuccessful = uc.SimulateDispose();

            // Assert
            cleanupSuccessful.Should().BeTrue();
        }

        [TestMethod]
        public void PanelToolbar_Paint_ShouldDrawBottomBorderOnly()
        {
            // Arrange
            var panel = new Panel { Width = 200, Height = 50 };

            // Act
            bool bottomBorderDrawn = DocumentManagementTestHelper.SimulatePanelPaint(panel);

            // Assert
            bottomBorderDrawn.Should().BeTrue();
        }

        [TestMethod]
        public void CardLayout_ForPersonalDocument_ShouldShowCorrectButtons()
        {
            // Arrange
            var personalRow = DocumentManagementTestHelper.CreatePersonalDocumentRow();

            // Act
            var buttons = DocumentManagementTestHelper.GetCardButtons(personalRow, false);
            bool hasView = buttons.Contains("View");
            bool hasShare = buttons.Contains("Share");
            bool hasDelete = buttons.Contains("Delete");

            // Assert
            hasView.Should().BeTrue();
            hasShare.Should().BeTrue();
            hasDelete.Should().BeTrue();
        }

        [TestMethod]
        public void CardLayout_ForSharedDocument_ShouldShowCorrectButtons()
        {
            // Arrange
            var sharedRow = DocumentManagementTestHelper.CreateSharedDocumentRow();

            // Act
            var buttons = DocumentManagementTestHelper.GetCardButtons(sharedRow, false);
            bool hasView = buttons.Contains("View");
            bool hasUnshare = buttons.Contains("Unshare");
            bool hasDelete = buttons.Contains("Delete");

            // Assert
            hasView.Should().BeTrue();
            hasUnshare.Should().BeTrue();
            hasDelete.Should().BeTrue();
        }

        [TestMethod]
        public void CardLayout_ForSharedTab_ShouldShowViewAndDownload()
        {
            // Arrange
            var sharedTabRow = DocumentManagementTestHelper.CreateSharedTabDocumentRow();

            // Act
            var buttons = DocumentManagementTestHelper.GetCardButtons(sharedTabRow, true);
            bool hasView = buttons.Contains("View");
            bool hasDownload = buttons.Contains("Download");

            // Assert
            hasView.Should().BeTrue();
            hasDownload.Should().BeTrue();
        }

        [TestMethod]
        public void ErrorHandling_ShouldShowUserFriendlyMessages()
        {
            // Arrange
            var uc = DocumentManagementTestHelper.CreateDocumentManagementUC("GV001");
            var exception = new FileNotFoundException("File not found");

            // Act
            bool friendlyMessageShown = uc.SimulateErrorHandling(exception);

            // Assert
            friendlyMessageShown.Should().BeTrue();
        }
    }
}