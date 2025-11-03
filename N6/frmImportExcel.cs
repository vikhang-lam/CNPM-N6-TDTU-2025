using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel; // Thêm cho BackgroundWorker

namespace N6
{
    /// <summary>
    /// Form xử lý việc Import dữ liệu từ file Excel/CSV cho các chức năng khác nhau.
    /// </summary>
    public partial class frmImportExcel : Form
    {
        /// <summary>
        /// Xác định loại dữ liệu đang được import.
        /// </summary>
        public enum ImportType
        {
            HocSinhTheoLop,
            HocSinhTheoKhoi,
            PhanCong,
            ThoiKhoaBieu
        }

        /// <summary>
        /// Nếu Import TKB, lưu lại ngày đầu tiên để UserControl có thể điều hướng đến.
        /// </summary>
        public DateTime? FirstImportedDate { get; private set; }

        private readonly ImportType _importType;
        private readonly string _contextId; // Mã (MaLop hoặc MaGV)
        private readonly string _contextName; // Tên (TenLop hoặc TenGV)
        private readonly string _khoi;
        private string _selectedFilePath;

        // --- P/Invoke để di chuyển Form ---
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        // ---------------------------------

        public frmImportExcel(ImportType type, string contextId, string contextName, string khoi = null)
        {
            InitializeComponent();
            _importType = type;
            _contextId = contextId;
            _contextName = contextName;
            _khoi = khoi;
        }

        private void frmImportExcel_Load(object sender, EventArgs e)
        {
            // Cài đặt tiêu đề và nút bấm dựa trên loại Import
            switch (_importType)
            {
                case ImportType.HocSinhTheoLop:
                    lblTitle.Text = $"Import Học Sinh cho lớp {_contextName}";
                    btnDownloadTemplate.Text = "📥 Tải mẫu HS";
                    break;
                case ImportType.HocSinhTheoKhoi:
                    lblTitle.Text = $"Import Học Sinh cho khối {_khoi}";
                    btnDownloadTemplate.Text = "📥 Tải mẫu HS (có cột MaLop)";
                    break;
                case ImportType.PhanCong:
                    lblTitle.Text = $"Import Phân Công cho lớp {_contextName}";
                    btnDownloadTemplate.Text = "📥 Tải mẫu Phân công";
                    break;
                case ImportType.ThoiKhoaBieu:
                    lblTitle.Text = $"Import TKB cho giáo viên {_contextName}";
                    btnDownloadTemplate.Text = "📥 Tải mẫu TKB";
                    break;
            }
            ShowState_SelectFile();

            // Gán sự kiện (để có thể gỡ trong Dispose)
            this.Paint += frmImportExcel_Paint;
            this.pnlTopBar.MouseDown += pnlTopBar_MouseDown;
            this.lblClose.Click += lblClose_Click;
            this.btnDownloadTemplate.Click += btnDownloadTemplate_Click;
            this.btnImport.Click += btnImport_Click;
            this.pnlDropZone.Click += pnlDropZone_Click;
            this.pnlDropZone.DragEnter += pnlDropZone_DragEnter;
            this.pnlDropZone.DragDrop += pnlDropZone_DragDrop;
            this.backgroundWorker.DoWork += backgroundWorker_DoWork;
            this.backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
        }

        #region UI States & Events

        /// <summary>
        /// Trạng thái ban đầu: chờ chọn file.
        /// </summary>
        private void ShowState_SelectFile()
        {
            pnlDropZone.Visible = true;
            pnlResults.Visible = false;
            progressBar.Visible = false;
            btnImport.Text = "Bắt đầu Import";
            btnImport.Enabled = false;
        }

        /// <summary>
        /// Trạng thái đang xử lý: hiển thị ProgressBar.
        /// </summary>
        private void ShowState_InProgress()
        {
            progressBar.Visible = true;
            btnImport.Enabled = false;
            pnlDropZone.Enabled = false;
            btnDownloadTemplate.Enabled = false;
        }

        /// <summary>
        /// Trạng thái kết quả: hiển thị thông báo thành công hoặc thất bại.
        /// </summary>
        private void ShowState_Results(bool success, string message, string errorLog = "")
        {
            progressBar.Visible = false;
            pnlDropZone.Visible = false;
            pnlResults.Visible = true;
            picResultIcon.Image = success ? Properties.Resources.success_icon : Properties.Resources.error_icon;

            lblResultStatus.Text = message;
            lblResultStatus.ForeColor = success ? Color.ForestGreen : Color.Red;
            txtErrorLog.ForeColor = success ? Color.Black : Color.Red;

            txtErrorLog.Visible = !string.IsNullOrEmpty(errorLog);
            txtErrorLog.Text = errorLog;
            btnImport.Text = "Đóng";
            btnImport.Enabled = true;

            if (success)
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void pnlDropZone_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ProcessFile(openFileDialog.FileName);
            }
        }

        private void pnlDropZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void pnlDropZone_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                ProcessFile(files[0]);
            }
        }

        /// <summary>
        /// Xử lý file đã chọn (hiển thị tên file và kích hoạt nút Import).
        /// </summary>
        private void ProcessFile(string filePath)
        {
            _selectedFilePath = filePath;
            lblFileName.Text = Path.GetFileName(filePath);
            btnImport.Enabled = true;
        }

        #endregion

        #region Button Clicks

        /// <summary>
        /// Xử lý sự kiện tải file mẫu (template) về máy.
        /// </summary>
        private void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "CSV (Phân cách bằng dấu chấm phẩy)|*.csv";
            DataTable template = new DataTable();
            var listSeparator = ";";

            switch (_importType)
            {
                case ImportType.HocSinhTheoLop:
                    saveFileDialog.FileName = $"Mau_Import_HS_{_contextName}.csv";
                    template.Columns.AddRange(new DataColumn[] {
                        new DataColumn("MaHS"), new DataColumn("HoTen"), new DataColumn("NgaySinh"),
                        new DataColumn("GioiTinh"), new DataColumn("SDTPhuHuynh"), new DataColumn("DiaChi"), new DataColumn("DanToc")
                    });
                    template.Rows.Add("HS001", "Nguyễn Văn An", "15/05/2010", "Nam", "0909123456", "123 Đường ABC, Q1", "Kinh");
                    break;
                case ImportType.HocSinhTheoKhoi:
                    saveFileDialog.FileName = $"Mau_Import_HS_Khoi_{_khoi}.csv";
                    template.Columns.AddRange(new DataColumn[] {
                        new DataColumn("MaHS"), new DataColumn("MaLop"), new DataColumn("HoTen"), new DataColumn("NgaySinh"),
                        new DataColumn("GioiTinh"), new DataColumn("SDTPhuHuynh"), new DataColumn("DiaChi"), new DataColumn("DanToc")
                    });
                    template.Rows.Add("HS002", "1A1", "Trần Thị Bình", "20/08/2010", "Nữ", "0909789123", "456 Đường XYZ, Q2", "Kinh");
                    break;
                case ImportType.PhanCong:
                    saveFileDialog.FileName = $"Mau_PhanCong_{_contextName}.csv";
                    template.Columns.AddRange(new DataColumn[] { new DataColumn("TenMon"), new DataColumn("TenGV") });
                    template.Rows.Add("Toán", "Thầy Quốc Hưng");
                    template.Rows.Add("Tiếng Việt", "Cô Minh Anh");
                    break;
                case ImportType.ThoiKhoaBieu:
                    saveFileDialog.FileName = $"Mau_TKB_{_contextName}.csv";
                    template.Columns.AddRange(new DataColumn[] {
                        new DataColumn("Ngay"), new DataColumn("Tiet"), new DataColumn("TenMon"),
                        new DataColumn("TenLop"), new DataColumn("GhiChu"), new DataColumn("MauSac")
                    });
                    template.Rows.Add("06/10/2025", "1", "Tiếng Việt", "Lớp 5A1", "Ghi chú tiết học", "#FFDDC1");
                    template.Rows.Add("06/10/2025", "2", "Toán", "Lớp 1A1", "", "");
                    break;
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Join(listSeparator, template.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
                    foreach (DataRow row in template.Rows)
                    {
                        var fields = row.ItemArray.Select(field => $"\"{field.ToString().Replace("\"", "\"\"")}\"");
                        sb.AppendLine(string.Join(listSeparator, fields));
                    }
                    // Ghi file với UTF-8 BOM để Excel đọc đúng tiếng Việt
                    File.WriteAllText(saveFileDialog.FileName, sb.ToString(), new UTF8Encoding(true));
                    MessageBox.Show("Đã lưu file mẫu thành công!", "Thành công");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tạo file mẫu: " + ex.Message, "Lỗi");
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút "Import" (hoặc "Đóng").
        /// </summary>
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (btnImport.Text == "Đóng")
            {
                this.Close();
                return;
            }
            if (string.IsNullOrEmpty(_selectedFilePath))
            {
                MessageBox.Show("Vui lòng chọn một file để import.");
                return;
            }
            ShowState_InProgress();
            backgroundWorker.RunWorkerAsync();
        }

        #endregion

        /// <summary>
        /// Kiểm tra xem DataTable có chứa tất cả các cột bắt buộc hay không (không phân biệt hoa thường).
        /// </summary>
        private bool ValidateColumns(DataTable dt, string[] requiredColumns, out string missingColumns)
        {
            var columns = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.Trim()).ToList();
            var missing = new List<string>();
            foreach (var col in requiredColumns)
            {
                if (!columns.Any(c => c.Equals(col, StringComparison.OrdinalIgnoreCase)))
                {
                    missing.Add(col);
                }
            }

            if (missing.Count > 0)
            {
                missingColumns = string.Join(", ", missing);
                return false;
            }
            missingColumns = string.Empty;
            return true;
        }

        #region Background Worker (Xử lý đa luồng)

        /// <summary>
        /// Luồng chạy ngầm để đọc và xử lý file Excel, tránh treo giao diện.
        /// </summary>
        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DataTable dt = null;
            try
            {
                dt = ExcelHelper.ReadExcelFile(_selectedFilePath);
            }
            catch (Exception ex)
            {
                // Bắt lỗi nếu ExcelHelper không đọc được file
                e.Result = new { Success = false, Message = "Không thể đọc file!", Log = $"Lỗi: {ex.Message}\nFile có thể bị hỏng hoặc không đúng định dạng Excel." };
                return;
            }

            if (dt == null || dt.Rows.Count == 0)
            {
                e.Result = new { Success = false, Message = "File rỗng hoặc không hợp lệ!", Log = "Không tìm thấy dữ liệu trong file Excel hoặc file không thể đọc." };
                return;
            }

            try
            {
                string[] requiredColumns;
                string missingColumns;

                switch (_importType)
                {
                    case ImportType.HocSinhTheoLop:
                        requiredColumns = new string[] { "MaHS", "HoTen", "NgaySinh" }; // Cột tối thiểu
                        if (!ValidateColumns(dt, requiredColumns, out missingColumns))
                        {
                            e.Result = new { Success = false, Message = "Sai định dạng Import!", Log = $"File Excel thiếu các cột bắt buộc: {missingColumns}.\nHãy tải và xem định dạng trong file mẫu." };
                            return;
                        }
                        var resultLop = DatabaseHelper.ImportStudentsToClass(dt, _contextId);
                        e.Result = new { Success = true, Message = $"Import hoàn tất!", Log = $"Thành công: {resultLop.Success}\nBỏ qua (đã tồn tại): {resultLop.Skipped}\nThất bại (sai dữ liệu): {resultLop.Failed}" };
                        break;

                    case ImportType.HocSinhTheoKhoi:
                        requiredColumns = new string[] { "MaHS", "MaLop", "HoTen", "NgaySinh" }; // Cột tối thiểu
                        if (!ValidateColumns(dt, requiredColumns, out missingColumns))
                        {
                            e.Result = new { Success = false, Message = "Sai định dạng Import!", Log = $"File Excel thiếu các cột bắt buộc: {missingColumns}.\nHãy tải và xem định dạng trong file mẫu." };
                            return;
                        }
                        var resultKhoi = DatabaseHelper.ImportStudentsFromDataTable(dt);
                        e.Result = new { Success = true, Message = $"Import hoàn tất!", Log = $"Thành công: {resultKhoi.Success}\nBỏ qua (đã tồn tại): {resultKhoi.Skipped}\nThất bại (sai dữ liệu): {resultKhoi.Failed}" };
                        break;

                    case ImportType.PhanCong:
                        requiredColumns = new string[] { "TenMon", "TenGV" };
                        if (!ValidateColumns(dt, requiredColumns, out missingColumns))
                        {
                            e.Result = new { Success = false, Message = "Sai định dạng Import!", Log = $"File Excel thiếu các cột bắt buộc: {missingColumns}.\nHãy tải và xem định dạng trong file mẫu." };
                            return;
                        }

                        DataTable allMonHoc = DatabaseHelper.GetAllSubjects();
                        DataTable allGiaoVien = DatabaseHelper.GetAllTeachers();
                        var errorList = new StringBuilder();
                        int successCount = 0;

                        foreach (DataRow row in dt.Rows)
                        {
                            string tenMon = row["TenMon"]?.ToString().Trim();
                            string tenGV = row["TenGV"]?.ToString().Trim();
                            if (string.IsNullOrEmpty(tenMon) || string.IsNullOrEmpty(tenGV)) continue;

                            var monHocRow = allMonHoc.AsEnumerable().FirstOrDefault(r => r.Field<string>("TenMon").Equals(tenMon, StringComparison.OrdinalIgnoreCase));
                            if (monHocRow == null) { errorList.AppendLine($"- Môn '{tenMon}': không tồn tại."); continue; }
                            string maMon = monHocRow["MaMon"].ToString();

                            var gvRow = allGiaoVien.AsEnumerable().FirstOrDefault(r => r.Field<string>("Ten").Equals(tenGV, StringComparison.OrdinalIgnoreCase));
                            if (gvRow == null) { errorList.AppendLine($"- GV '{tenGV}': không tồn tại."); continue; }
                            string maGV = gvRow["MaGV"].ToString();

                            DataTable gvMonHocTable = DatabaseHelper.GetSubjectsByTeacher(maGV);
                            bool gvDayMonNay = gvMonHocTable.AsEnumerable().Any(r => r.Field<string>("MaMon").Equals(maMon, StringComparison.OrdinalIgnoreCase));

                            if (!gvDayMonNay)
                            {
                                errorList.AppendLine($"- GV '{tenGV}': không được phân công dạy môn '{tenMon}'.");
                                continue;
                            }

                            DatabaseHelper.UpdateTeachingAssignment(_contextId, maMon, maGV);
                            successCount++;
                        }
                        string logPhanCong = $"Thành công: {successCount} môn.\n" + (errorList.Length > 0 ? "Lỗi:\n" + errorList.ToString() : "");
                        e.Result = new { Success = successCount > 0, Message = "Import phân công hoàn tất!", Log = logPhanCong };
                        break;

                    case ImportType.ThoiKhoaBieu:
                        requiredColumns = new string[] { "Ngay", "Tiet", "TenMon", "TenLop" };
                        if (!ValidateColumns(dt, requiredColumns, out missingColumns))
                        {
                            e.Result = new { Success = false, Message = "Sai định dạng Import!", Log = $"File Excel thiếu các cột bắt buộc: {missingColumns}.\nHãy tải và xem định dạng trong file mẫu." };
                            return;
                        }

                        // Lấy ngày đầu tiên trong file Excel để trả về cho UC TKB
                        var firstDate = dt.AsEnumerable()
                            .Select(row => {
                                DateTime date;
                                if (DateTime.TryParse(row["Ngay"]?.ToString(), out date)) return (DateTime?)date;
                                if (DateTime.TryParseExact(row["Ngay"]?.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) return (DateTime?)date;
                                return null;
                            })
                            .Where(d => d.HasValue)
                            .OrderBy(d => d.Value)
                            .FirstOrDefault();

                        this.FirstImportedDate = firstDate;

                        var resultTKB = DatabaseHelper.ImportTimetableForTeacher(_contextId, dt);
                        e.Result = new { Success = resultTKB.Success > 0, Message = "Import TKB hoàn tất!", Log = $"Thành công: {resultTKB.Success}\nThất bại/Sai dữ liệu: {resultTKB.Failed}" };
                        break;
                }
            }
            catch (Exception ex)
            {
                // Bắt lỗi chung (ví dụ: sai kiểu dữ liệu "Tiet" = "abc")
                e.Result = new
                {
                    Success = false,
                    Message = "Sai định dạng Import!",
                    Log = $"Đã xảy ra lỗi khi xử lý dữ liệu: {ex.Message}\nHãy tải và xem định dạng trong file mẫu."
                };
            }
        }

        /// <summary>
        /// Hoàn tất luồng chạy ngầm, hiển thị kết quả lên giao diện.
        /// </summary>
        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Lỗi nghiêm trọng (lỗi lập trình)
                ShowState_Results(false, "Đã xảy ra lỗi nghiêm trọng!", e.Error.Message + "\nHãy tải và xem định dạng trong file mẫu.");
            }
            else
            {
                // Lỗi nghiệp vụ (do người dùng) hoặc thành công
                dynamic result = e.Result;
                ShowState_Results(result.Success, result.Message, result.Log);
            }
        }

        #endregion

        #region Window Drag & Paint

        private void frmImportExcel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(222, 226, 230)), 0, 0, Width - 1, Height - 1);
        }

        private void pnlTopBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Gỡ bỏ sự kiện của các control trong Designer
                if (this.backgroundWorker != null)
                {
                    this.backgroundWorker.DoWork -= backgroundWorker_DoWork;
                    this.backgroundWorker.RunWorkerCompleted -= backgroundWorker_RunWorkerCompleted;
                }
                this.Paint -= frmImportExcel_Paint;
                if (this.pnlTopBar != null) this.pnlTopBar.MouseDown -= pnlTopBar_MouseDown;
                if (this.lblClose != null) this.lblClose.Click -= lblClose_Click;
                if (this.btnDownloadTemplate != null) this.btnDownloadTemplate.Click -= btnDownloadTemplate_Click;
                if (this.btnImport != null) this.btnImport.Click -= btnImport_Click;
                if (this.pnlDropZone != null)
                {
                    this.pnlDropZone.Click -= pnlDropZone_Click;
                    this.pnlDropZone.DragEnter -= pnlDropZone_DragEnter;
                    this.pnlDropZone.DragDrop -= pnlDropZone_DragDrop;
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}