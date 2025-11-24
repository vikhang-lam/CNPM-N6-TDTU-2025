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
using System.ComponentModel;
using System.Text.RegularExpressions;

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

        // --- P/Invoke để di chuyển Form không viền ---
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
                    btnDownloadTemplate.Text = "📥 Tải mẫu HS (Không cần Mã)";
                    break;
                case ImportType.HocSinhTheoKhoi:
                    lblTitle.Text = $"Import Học Sinh cho khối {_khoi}";
                    btnDownloadTemplate.Text = "📥 Tải mẫu HS (Kèm cột Mã Lớp)";
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

        }

        #region UI States & Events

        private void ShowState_SelectFile()
        {
            pnlDropZone.Visible = true;
            pnlResults.Visible = false;
            progressBar.Visible = false;
            btnImport.Text = "Bắt đầu Import";
            btnImport.Enabled = false;
        }

        private void ShowState_InProgress()
        {
            progressBar.Visible = true;
            btnImport.Enabled = false;
            pnlDropZone.Enabled = false;
            btnDownloadTemplate.Enabled = false;
        }

        private void ShowState_Results(bool success, string message, string errorLog = "")
        {
            progressBar.Visible = false;
            pnlDropZone.Visible = false;
            pnlResults.Visible = true;
            picResultIcon.Image = success ? Properties.Resources.success_icon : Properties.Resources.error_icon;

            lblResultStatus.Text = message;
            lblResultStatus.ForeColor = success ? Color.ForestGreen : Color.Red;

            txtErrorLog.Visible = !string.IsNullOrEmpty(errorLog);
            txtErrorLog.Text = errorLog;
            txtErrorLog.ForeColor = success ? Color.Black : Color.Red;

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

        private void ProcessFile(string filePath)
        {
            _selectedFilePath = filePath;
            lblFileName.Text = Path.GetFileName(filePath);
            btnImport.Enabled = true;
        }

        #endregion

        #region Button Clicks

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
                        new DataColumn("HoTen"), new DataColumn("NgaySinh"),
                        new DataColumn("GioiTinh"), new DataColumn("SDTPhuHuynh"), new DataColumn("DiaChi"), new DataColumn("DanToc")
                    });
                    template.Rows.Add("Nguyễn Văn An", "15/05/2015", "Nam", "0909123456", "123 Đường ABC, Q1", "Kinh");
                    break;

                case ImportType.HocSinhTheoKhoi:
                    saveFileDialog.FileName = $"Mau_Import_HS_Khoi_{_khoi}.csv";
                    template.Columns.AddRange(new DataColumn[] {
                        new DataColumn("MaLop"), new DataColumn("HoTen"), new DataColumn("NgaySinh"),
                        new DataColumn("GioiTinh"), new DataColumn("SDTPhuHuynh"), new DataColumn("DiaChi"), new DataColumn("DanToc")
                    });
                    template.Rows.Add("1A1", "Trần Thị Bình", "20/08/2015", "Nữ", "0909789123", "456 Đường XYZ, Q2", "Kinh");
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
                    template.Rows.Add(DateTime.Now.ToString("dd/MM/yyyy"), "1", "Tiếng Việt", "Lớp 5A1", "Ghi chú tiết học", "#FFDDC1");
                    template.Rows.Add(DateTime.Now.ToString("dd/MM/yyyy"), "2", "Toán", "Lớp 1A1", "", "");
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
                    File.WriteAllText(saveFileDialog.FileName, sb.ToString(), new UTF8Encoding(true));
                    MessageBox.Show("Đã lưu file mẫu thành công!", "Thành công");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tạo file mẫu: " + ex.Message, "Lỗi");
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy) return;

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

        #region Background Worker (Logic chính - Đã Fix lỗi ngày tháng)

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DataTable dtRaw = null;
            try
            {
                dtRaw = ExcelHelper.ReadExcelFile(_selectedFilePath);
            }
            catch (Exception ex)
            {
                e.Result = new { Success = false, Message = "Không thể đọc file!", Log = $"Lỗi: {ex.Message}\nFile có thể bị hỏng hoặc đang mở." };
                return;
            }

            if (dtRaw == null || dtRaw.Rows.Count == 0)
            {
                e.Result = new { Success = false, Message = "File rỗng!", Log = "Không tìm thấy dữ liệu." };
                return;
            }

            try
            {
                string missingColumns;

                if (_importType == ImportType.HocSinhTheoLop || _importType == ImportType.HocSinhTheoKhoi)
                {
                    // 1. Check cột bắt buộc
                    string[] requiredCols = (_importType == ImportType.HocSinhTheoLop)
                        ? new string[] { "HoTen", "NgaySinh" }
                        : new string[] { "MaLop", "HoTen", "NgaySinh" };

                    if (!ValidateColumns(dtRaw, requiredCols, out missingColumns))
                    {
                        e.Result = new { Success = false, Message = "Sai định dạng!", Log = $"Thiếu cột: {missingColumns}." };
                        return;
                    }

                    // 2. Validation & Làm sạch dữ liệu
                    DataTable dtClean = dtRaw.Clone();
                    if (dtClean.Columns["NgaySinh"].DataType != typeof(DateTime))
                        dtClean.Columns["NgaySinh"].DataType = typeof(DateTime);

                    StringBuilder errorLog = new StringBuilder();
                    int validCount = 0;
                    int rowIndex = 1;

                    var regexName = new Regex(@"^[\p{L}\s]+$");
                    var regexPhone = new Regex(@"^\d{10}$");

                    foreach (DataRow row in dtRaw.Rows)
                    {
                        rowIndex++;
                        List<string> rowErrors = new List<string>();
                        bool isValid = true;

                        // a. Validate Tên
                        string hoTen = row["HoTen"]?.ToString().Trim();
                        if (string.IsNullOrEmpty(hoTen))
                        {
                            rowErrors.Add("Tên trống");
                            isValid = false;
                        }
                        else if (!regexName.IsMatch(hoTen))
                        {
                            rowErrors.Add("Tên chứa số/kí tự lạ");
                            isValid = false;
                        }

                        // b. Validate Ngày Sinh (SỬA LỖI 40314 TẠI ĐÂY)
                        DateTime ngaySinh = DateTime.MinValue;
                        string rawNS = row["NgaySinh"]?.ToString().Trim();
                        bool dateParseOk = false;
                        double oaDateValue;

                        // Thử 1: Chuẩn dd/MM/yyyy
                        if (DateTime.TryParseExact(rawNS, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ngaySinh))
                        {
                            dateParseOk = true;
                        }
                        // Thử 2: Dạng số Excel (ví dụ 40314)
                        else if (double.TryParse(rawNS, out oaDateValue))
                        {
                            try
                            {
                                ngaySinh = DateTime.FromOADate(oaDateValue);
                                dateParseOk = true;
                            }
                            catch { }
                        }
                        // Thử 3: Parse tự động khác
                        else if (DateTime.TryParse(rawNS, out ngaySinh))
                        {
                            dateParseOk = true;
                        }

                        if (!dateParseOk)
                        {
                            rowErrors.Add($"Ngày sinh '{rawNS}' sai định dạng");
                            isValid = false;
                        }
                        else
                        {
                            if (ngaySinh > DateTime.Now)
                            {
                                rowErrors.Add("Ngày sinh ở tương lai");
                                isValid = false;
                            }
                            else if (DateTime.Now.Year - ngaySinh.Year < 5)
                            {
                                rowErrors.Add("Tuổi nhỏ (< 5 tuổi)");
                                isValid = false;
                            }
                        }

                        // c. Validate Giới tính
                        string gioitinh = row["GioiTinh"]?.ToString().Trim();
                        if (!string.IsNullOrEmpty(gioitinh) && gioitinh != "Nam" && gioitinh != "Nữ")
                        {
                            rowErrors.Add($"Giới tính '{gioitinh}' không hợp lệ");
                            isValid = false;
                        }

                        // d. Validate SĐT
                        string sdt = row["SDTPhuHuynh"]?.ToString().Trim();
                        if (!string.IsNullOrEmpty(sdt) && !regexPhone.IsMatch(sdt))
                        {
                            rowErrors.Add($"SĐT '{sdt}' không đúng 10 số");
                            isValid = false;
                        }

                        // -> Tổng hợp
                        if (isValid)
                        {
                            DataRow newRow = dtClean.NewRow();
                            if (dtClean.Columns.Contains("MaLop") && dtRaw.Columns.Contains("MaLop"))
                                newRow["MaLop"] = row["MaLop"];

                            newRow["HoTen"] = hoTen;
                            newRow["NgaySinh"] = ngaySinh;
                            newRow["GioiTinh"] = gioitinh;
                            newRow["SDTPhuHuynh"] = sdt;
                            newRow["DiaChi"] = row["DiaChi"];
                            newRow["DanToc"] = row["DanToc"];

                            dtClean.Rows.Add(newRow);
                            validCount++;
                        }
                        else
                        {
                            errorLog.AppendLine($"Dòng {rowIndex}: {string.Join(", ", rowErrors)}");
                        }
                    }

                    if (validCount == 0)
                    {
                        e.Result = new { Success = false, Message = "Không có dòng nào hợp lệ!", Log = errorLog.ToString() };
                        return;
                    }

                    dynamic dbResult = null;
                    if (_importType == ImportType.HocSinhTheoLop)
                    {
                        dbResult = DatabaseHelper.ImportStudentsToClass(dtClean, _contextId);
                    }
                    else
                    {
                        dbResult = DatabaseHelper.ImportStudentsFromDataTable(dtClean);
                    }

                    string finalLog = $"Kiểm tra dữ liệu:\n- Hợp lệ: {validCount}\n- Lỗi định dạng (bỏ qua): {dtRaw.Rows.Count - validCount}\n\n" +
                                      $"Kết quả Lưu Database:\n- Thêm mới thành công: {dbResult.Success}\n- Trùng lặp (bỏ qua): {dbResult.Skipped}";

                    if (errorLog.Length > 0)
                        finalLog += "\n\n-----------------\nChi tiết lỗi định dạng:\n" + errorLog.ToString();

                    e.Result = new { Success = true, Message = "Xử lý hoàn tất!", Log = finalLog };
                }
                // === LOGIC CÁC LOẠI KHÁC (GIỮ NGUYÊN) ===
                else if (_importType == ImportType.PhanCong)
                {
                    string[] requiredColumns = new string[] { "TenMon", "TenGV" };
                    if (!ValidateColumns(dtRaw, requiredColumns, out missingColumns))
                    {
                        e.Result = new { Success = false, Message = "Sai định dạng!", Log = $"Thiếu cột: {missingColumns}" };
                        return;
                    }

                    DataTable allMonHoc = DatabaseHelper.GetAllSubjects();
                    DataTable allGiaoVien = DatabaseHelper.GetAllTeachers();
                    var errorList = new StringBuilder();
                    int successCount = 0;

                    foreach (DataRow row in dtRaw.Rows)
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
                }
                else if (_importType == ImportType.ThoiKhoaBieu)
                {
                    string[] requiredColumns = new string[] { "Ngay", "Tiet", "TenMon", "TenLop" };
                    if (!ValidateColumns(dtRaw, requiredColumns, out missingColumns))
                    {
                        e.Result = new { Success = false, Message = "Sai định dạng!", Log = $"Thiếu cột: {missingColumns}" };
                        return;
                    }

                    var firstDate = dtRaw.AsEnumerable()
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

                    var resultTKB = DatabaseHelper.ImportTimetableForTeacher(_contextId, dtRaw);
                    e.Result = new { Success = resultTKB.Success > 0, Message = "Import TKB hoàn tất!", Log = $"Thành công: {resultTKB.Success}\nThất bại/Sai dữ liệu: {resultTKB.Failed}" };
                }
            }
            catch (Exception ex)
            {
                e.Result = new { Success = false, Message = "Lỗi xử lý!", Log = ex.ToString() };
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ShowState_Results(false, "Đã xảy ra lỗi nghiêm trọng!", e.Error.Message);
            }
            else
            {
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.backgroundWorker != null)
                {
                    this.backgroundWorker.DoWork -= backgroundWorker_DoWork;
                    this.backgroundWorker.RunWorkerCompleted -= backgroundWorker_RunWorkerCompleted;
                }
                this.Paint -= frmImportExcel_Paint;
                if (this.pnlTopBar != null) this.pnlTopBar.MouseDown -= pnlTopBar_MouseDown;
                if (this.lblClose != null) this.lblClose.Click -= lblClose_Click;
                if (this.btnDownloadTemplate != null) this.btnDownloadTemplate.Click -= btnDownloadTemplate_Click;

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