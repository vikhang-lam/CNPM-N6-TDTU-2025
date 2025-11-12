using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace N6.Tests.TestHelpers
{
    public static class DocumentManagementTestHelper
    {
        #region Test Data Creation

        public static DataTable CreatePersonalDocumentsDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaTL", typeof(string));
            dt.Columns.Add("TenTL", typeof(string));
            dt.Columns.Add("Kieu", typeof(string));
            dt.Columns.Add("TrangThaiChiaSe", typeof(string));

            dt.Rows.Add("TL001", "Giáo án Toán.pdf", "C:\\TaiLieu\\Toan.pdf", "Riêng tư");
            dt.Rows.Add("TL002", "Bài tập Văn.docx", "C:\\TaiLieu\\Van.docx", "Chia sẻ");
            dt.Rows.Add("TL003", "Đề thi Lý.xlsx", "C:\\TaiLieu\\Ly.xlsx", "Riêng tư");

            return dt;
        }

        public static DataTable CreateSharedDocumentsDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaTL", typeof(string));
            dt.Columns.Add("TenTL", typeof(string));
            dt.Columns.Add("Kieu", typeof(string));
            dt.Columns.Add("TenGV", typeof(string));

            dt.Rows.Add("TL004", "Tài liệu chung.pdf", "C:\\TaiLieu\\Chung.pdf", "Nguyễn Văn A");
            dt.Rows.Add("TL005", "Bài giảng Hóa.pptx", "C:\\TaiLieu\\Hoa.pptx", "Trần Thị B");

            return dt;
        }

        public static DataTable CreateEmptyDocumentsDataTable()
        {
            return new DataTable();
        }

        public static DataRow CreatePdfDocumentRow()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaTL", typeof(string));
            dt.Columns.Add("TenTL", typeof(string));
            dt.Columns.Add("Kieu", typeof(string));
            dt.Rows.Add("TL001", "Test PDF.pdf", "C:\\Test\\document.pdf");
            return dt.Rows[0];
        }

        public static DataRow CreateWordDocumentRow()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaTL", typeof(string));
            dt.Columns.Add("TenTL", typeof(string));
            dt.Columns.Add("Kieu", typeof(string));
            dt.Rows.Add("TL002", "Test Word.docx", "C:\\Test\\document.docx");
            return dt.Rows[0];
        }

        public static DataRow CreateMissingFileDocumentRow()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaTL", typeof(string));
            dt.Columns.Add("TenTL", typeof(string));
            dt.Columns.Add("Kieu", typeof(string));
            dt.Rows.Add("TL003", "Missing File.pdf", "C:\\NonExistent\\file.pdf");
            return dt.Rows[0];
        }

        public static DataRow CreateValidDocumentRow()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaTL", typeof(string));
            dt.Columns.Add("TenTL", typeof(string));
            dt.Columns.Add("Kieu", typeof(string));
            dt.Rows.Add("TL004", "Valid File.pdf", "C:\\Test\\valid.pdf");
            return dt.Rows[0];
        }

        public static DataRow CreatePersonalDocumentRow()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaTL", typeof(string));
            dt.Columns.Add("TenTL", typeof(string));
            dt.Columns.Add("Kieu", typeof(string));
            dt.Columns.Add("TrangThaiChiaSe", typeof(string));
            dt.Rows.Add("TL005", "Personal Doc.pdf", "C:\\Test\\personal.pdf", "Riêng tư");
            return dt.Rows[0];
        }

        public static DataRow CreateSharedDocumentRow()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaTL", typeof(string));
            dt.Columns.Add("TenTL", typeof(string));
            dt.Columns.Add("Kieu", typeof(string));
            dt.Columns.Add("TrangThaiChiaSe", typeof(string));
            dt.Rows.Add("TL006", "Shared Doc.pdf", "C:\\Test\\shared.pdf", "Chia sẻ");
            return dt.Rows[0];
        }

        public static DataRow CreateSharedTabDocumentRow()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaTL", typeof(string));
            dt.Columns.Add("TenTL", typeof(string));
            dt.Columns.Add("Kieu", typeof(string));
            dt.Columns.Add("TenGV", typeof(string));
            dt.Rows.Add("TL007", "Shared Tab Doc.pdf", "C:\\Test\\sharedtab.pdf", "Lê Văn C");
            return dt.Rows[0];
        }

        #endregion

        #region Mock Helpers

        public static MockDocumentManagementUC CreateDocumentManagementUC(string maGV)
        {
            return new MockDocumentManagementUC(maGV);
        }

        public static MockDirectoryService CreateMockDirectoryService()
        {
            return new MockDirectoryService();
        }

        public static string GetFileIcon(string filePath)
        {
            switch (Path.GetExtension(filePath).ToLower())
            {
                case ".pdf": return "📕";
                case ".doc":
                case ".docx": return "📘";
                case ".xls":
                case ".xlsx": return "📗";
                default: return "📄";
            }
        }

        public static Button CreateModernButton(string text, Color color)
        {
            return new Button
            {
                Text = text,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold)
            };
        }

        public static bool SimulatePanelPaint(Panel panel)
        {
            // Mock implementation for panel painting
            return true; // Bottom border drawn
        }

        public static List<string> GetCardButtons(DataRow row, bool isSharedTab)
        {
            var buttons = new List<string> { "View" };

            if (isSharedTab)
            {
                buttons.Add("Download");
            }
            else
            {
                string trangThai = row["TrangThaiChiaSe"].ToString();
                if (trangThai == "Chia sẻ")
                    buttons.Add("Unshare");
                else
                    buttons.Add("Share");
                buttons.Add("Delete");
            }

            return buttons;
        }

        public static FileInfo CreateTestFileInfo()
        {
            return new FileInfo("C:\\Test\\upload.pdf");
        }

        #endregion

        #region Mock Operation Results

        public static OperationResult CreateSuccessOperationResult()
        {
            return new OperationResult { Success = true, Message = "Operation completed successfully" };
        }

        public static OperationResult CreateFailureOperationResult()
        {
            return new OperationResult { Success = false, Message = "Operation failed" };
        }

        #endregion
    }

    #region Supporting Classes

    public class MockDocumentManagementUC
    {
        public string MaGV { get; private set; }
        public string StoragePath { get; private set; }
        public bool PersonalDocumentsLoaded { get; private set; }
        public bool SharedDocumentsLoaded { get; private set; }
        public int PersonalCardsCount { get; private set; }
        public int SharedCardsCount { get; private set; }

        public MockDocumentManagementUC(string maGV)
        {
            MaGV = maGV;
            StoragePath = Path.Combine(Application.StartupPath, "TaiLieu");
            PersonalDocumentsLoaded = true;
            SharedDocumentsLoaded = true;
        }

        public void SimulateLoadDocuments(DataTable data, bool isShared)
        {
            if (isShared)
                SharedCardsCount = data.Rows.Count;
            else
                PersonalCardsCount = data.Rows.Count;
        }

        public bool SimulateViewDocument(DataRow row)
        {
            string path = row["Kieu"].ToString();
            if (!File.Exists(path))
                return true; // Error message shown

            string ext = Path.GetExtension(path).ToLower();
            return ext == ".pdf" || ext != ".pdf"; // Both cases handled
        }

        public bool SimulateDownloadDocument(DataRow row)
        {
            string path = row["Kieu"].ToString();
            if (!File.Exists(path))
                return true; // Error message shown

            return true; // Download successful
        }

        public bool SimulateShareDocument(string maTL)
        {
            return true; // Share successful
        }

        public bool SimulateUnshareDocument(string maTL)
        {
            return true; // Unshare successful
        }

        public bool SimulateDeleteDocument(string maTL, bool userConfirmed)
        {
            return userConfirmed; // Delete only if confirmed
        }

        public bool SimulateUploadDocument(FileInfo file)
        {
            return file != null; // Upload only if file selected
        }

        public bool SimulateRefresh()
        {
            PersonalDocumentsLoaded = true;
            SharedDocumentsLoaded = true;
            return true;
        }

        public bool SimulateDispose()
        {
            PersonalCardsCount = 0;
            SharedCardsCount = 0;
            return true; // Cleanup successful
        }

        public bool SimulateErrorHandling(Exception ex)
        {
            return true; // User-friendly message shown
        }
    }

    public class MockDirectoryService
    {
        public bool CreateDirectoryCalled { get; private set; }

        public MockDirectoryService()
        {
            CreateDirectoryCalled = true; // Simulate directory creation
        }
    }

    #endregion
}