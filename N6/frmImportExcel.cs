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
    public partial class frmImportExcel : Form
    {
        public enum ImportType
        {
            HocSinhTheoLop,
            HocSinhTheoKhoi,
            PhanCong,
            ThoiKhoaBieu
        }

        public DateTime? FirstImportedDate { get; private set; }

        private readonly ImportType _importType;
        private readonly string _contextId;
        private readonly string _contextName;
        private readonly string _khoi;
        private string _selectedFilePath;

        // Biến lưu trạng thái thành công để trả về cho Form cha khi bấm Đóng
        private bool _isSuccess = false;

        // --- P/Invoke để di chuyển form ---
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        // ----------------

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
            switch (_importType)
            {
                case ImportType.HocSinhTheoLop:
                    lblTitle.Text = $"Import Học Sinh cho lớp {_contextName}";
                    btnDownloadTemplate.Text = "📥 Tải mẫu HS ";
                    break;
                case ImportType.HocSinhTheoKhoi:
                    lblTitle.Text = $"Import Học Sinh cho khối {_khoi}";
                    btnDownloadTemplate.Text = "📥 Tải mẫu HS ";
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

        private void ShowState_Results(bool success, string message, string log = "")
        {
            _isSuccess = success;

            progressBar.Visible = false;
            pnlDropZone.Visible = false;
            pnlResults.Visible = true;

            // Logic hiển thị icon và màu sắc
            if (success)
            {
                // Nếu thành công nhưng có cảnh báo (trong log có từ khóa CẢNH BÁO) -> Màu vàng cam
                if (log.Contains("CẢNH BÁO") || log.Contains("BỎ QUA"))
                {
                    picResultIcon.Image = Properties.Resources.success_icon;
                    lblResultStatus.Text = message + "\n(Có cảnh báo trùng lặp)";
                    lblResultStatus.ForeColor = Color.DarkOrange; // MÀU VÀNG CAM
                }
                else
                {
                    picResultIcon.Image = Properties.Resources.success_icon;
                    lblResultStatus.Text = message;
                    lblResultStatus.ForeColor = Color.ForestGreen; // MÀU XANH LÁ
                }
            }
            else
            {
                picResultIcon.Image = Properties.Resources.error_icon;
                lblResultStatus.Text = message;
                lblResultStatus.ForeColor = Color.Red;
            }

            txtErrorLog.Visible = !string.IsNullOrEmpty(log);
            txtErrorLog.Text = log;

            // Màu chữ trong khung log: Đỏ nếu lỗi, Đen xám nếu chỉ là cảnh báo
            txtErrorLog.ForeColor = (!success) ? Color.Red : Color.FromArgb(64, 64, 64);

            btnImport.Text = "Đóng";
            btnImport.Enabled = true;
        }

        private void pnlDropZone_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK) ProcessFile(openFileDialog.FileName);
        }

        private void pnlDropZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void pnlDropZone_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0) ProcessFile(files[0]);
        }

        private void ProcessFile(string filePath)
        {
            _selectedFilePath = filePath;
            lblFileName.Text = Path.GetFileName(filePath);
            btnImport.Enabled = true;
        }

        #endregion

        #region Helpers

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
                    break;
                case ImportType.ThoiKhoaBieu:
                    saveFileDialog.FileName = $"Mau_TKB_{_contextName}.csv";
                    template.Columns.AddRange(new DataColumn[] {
                        new DataColumn("Ngay"), new DataColumn("Tiet"), new DataColumn("TenMon"),
                        new DataColumn("TenLop"), new DataColumn("GhiChu")
                    });
                    template.Rows.Add(DateTime.Now.ToString("dd/MM/yyyy"), "1", "Tiếng Việt", "Lớp 5A1", "Ghi chú");
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
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy) return;

            if (btnImport.Text == "Đóng")
            {
                if (_isSuccess)
                {
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                }
                this.Close();
                return;
            }

            if (string.IsNullOrEmpty(_selectedFilePath)) { MessageBox.Show("Vui lòng chọn file."); return; }
            ShowState_InProgress();
            backgroundWorker.RunWorkerAsync();
        }

        private bool ValidateColumns(DataTable dt, string[] requiredColumns, out string missingColumns)
        {
            foreach (DataColumn col in dt.Columns) col.ColumnName = col.ColumnName.Trim();

            var columns = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var missing = new List<string>();
            foreach (var col in requiredColumns)
            {
                if (!columns.Any(c => c.Equals(col, StringComparison.OrdinalIgnoreCase))) missing.Add(col);
            }

            if (missing.Count > 0)
            {
                missingColumns = string.Join(", ", missing);
                return false;
            }
            missingColumns = string.Empty;
            return true;
        }

        private string GetRandomPastelColor()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int r = (random.Next(0, 256) + 255) / 2;
            int g = (random.Next(0, 256) + 255) / 2;
            int b = (random.Next(0, 256) + 255) / 2;
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        private bool TryParseDate(object cellValue, out DateTime result)
        {
            result = DateTime.MinValue;
            if (cellValue == null || cellValue == DBNull.Value) return false;

            if (cellValue is DateTime dt)
            {
                result = dt;
                return true;
            }

            string dateStr = cellValue.ToString().Trim();
            if (string.IsNullOrEmpty(dateStr)) return false;

            if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result)) return true;
            if (DateTime.TryParse(dateStr, out result)) return true;
            if (double.TryParse(dateStr, out double oaDate))
            {
                try { result = DateTime.FromOADate(oaDate); return true; } catch { }
            }

            return false;
        }

        #endregion

        #region Background Worker (Main Logic)

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DataTable dtRaw = null;
            try
            {
                dtRaw = ExcelHelper.ReadExcelFile(_selectedFilePath);
            }
            catch (Exception ex)
            {
                e.Result = new { Success = false, Message = "Lỗi đọc file!", Log = ex.Message };
                return;
            }

            if (dtRaw == null || dtRaw.Rows.Count == 0)
            {
                e.Result = new { Success = false, Message = "File rỗng!", Log = "Không có dữ liệu." };
                return;
            }

            try
            {
                string missingColumns;

                // ----------------------------------------------------------------------
                // 1. IMPORT HỌC SINH (Kiểm tra trùng toàn trường)
                // ----------------------------------------------------------------------
                if (_importType == ImportType.HocSinhTheoLop || _importType == ImportType.HocSinhTheoKhoi)
                {
                    string[] requiredCols = (_importType == ImportType.HocSinhTheoLop) ? new string[] { "HoTen", "NgaySinh" } : new string[] { "MaLop", "HoTen", "NgaySinh" };

                    if (!ValidateColumns(dtRaw, requiredCols, out missingColumns))
                    {
                        e.Result = new { Success = false, Message = "Sai cột!", Log = $"Thiếu cột: {missingColumns}" };
                        return;
                    }

                    // A. Tải dữ liệu toàn hệ thống để check trùng
                    // 1. Lấy tất cả lớp học để map Mã Lớp -> Tên Lớp (cho thông báo thân thiện)
                    DataTable dtClasses = DatabaseHelper.GetAllClasses();
                    Dictionary<string, string> classNames = new Dictionary<string, string>();
                    foreach (DataRow r in dtClasses.Rows)
                    {
                        if (r["MaLop"] != DBNull.Value && r["TenLop"] != DBNull.Value)
                            classNames[r["MaLop"].ToString()] = r["TenLop"].ToString();
                    }

                    // 2. Lấy tất cả học sinh toàn trường
                    // Dictionary để tra cứu: Key = "HoTen_NgaySinh", Value = "TenLop"
                    DataTable dtAllStudents = DatabaseHelper.GetAllStudents();
                    Dictionary<string, string> existingStudentLocation = new Dictionary<string, string>();

                    foreach (DataRow row in dtAllStudents.Rows)
                    {
                        string hoTenDb = row["HoTen"].ToString().Trim().ToLower();
                        string ngaySinhDb = "";
                        if (DateTime.TryParse(row["NgaySinh"].ToString(), out DateTime d))
                            ngaySinhDb = d.ToString("yyyyMMdd");

                        string key = $"{hoTenDb}_{ngaySinhDb}";

                        // Lưu lại lớp mà học sinh này đang học
                        string maLopDb = row.Table.Columns.Contains("MaLop") ? row["MaLop"].ToString() : "";
                        string tenLopHienTai = classNames.ContainsKey(maLopDb) ? classNames[maLopDb] : (string.IsNullOrEmpty(maLopDb) ? "Chưa xếp lớp" : maLopDb);

                        if (!existingStudentLocation.ContainsKey(key))
                        {
                            existingStudentLocation.Add(key, tenLopHienTai);
                        }
                    }

                    // B. Xử lý dữ liệu từ Excel
                    DataTable dtClean = dtRaw.Clone();
                    if (dtClean.Columns["NgaySinh"].DataType != typeof(DateTime)) dtClean.Columns["NgaySinh"].DataType = typeof(DateTime);
                    if (!dtClean.Columns.Contains("MaLop")) dtClean.Columns.Add("MaLop", typeof(string));

                    StringBuilder logBuilder = new StringBuilder();
                    int validCount = 0;
                    int duplicateCount = 0;
                    int rowIdx = 1;

                    foreach (DataRow row in dtRaw.Rows)
                    {
                        rowIdx++;
                        string hoTen = row["HoTen"]?.ToString().Trim();
                        // Nếu import theo lớp thì lấy contextId, nếu theo khối thì lấy từ Excel
                        string maLopTarget = _importType == ImportType.HocSinhTheoLop ? _contextId : (row.Table.Columns.Contains("MaLop") ? row["MaLop"]?.ToString().Trim() : "");

                        if (!TryParseDate(row["NgaySinh"], out DateTime ngaySinh))
                        {
                            logBuilder.AppendLine($"Dòng {rowIdx}: Ngày sinh sai định dạng (Bỏ qua).");
                            continue;
                        }
                        if (string.IsNullOrEmpty(hoTen)) continue;

                        // KIỂM TRA TRÙNG LẶP TOÀN CỤC
                        string checkKey = $"{hoTen.ToLower()}_{ngaySinh.ToString("yyyyMMdd")}";

                        if (existingStudentLocation.ContainsKey(checkKey))
                        {
                            // Lấy tên lớp học sinh đang học
                            string lopDangHoc = existingStudentLocation[checkKey];

                            // *** THÔNG BÁO MÀU VÀNG ***
                            // Báo rõ học sinh này đã có ở lớp nào
                            logBuilder.AppendLine($"[CẢNH BÁO] Dòng {rowIdx}: HS '{hoTen}' ({ngaySinh:dd/MM/yyyy}) đã tồn tại trong lớp [{lopDangHoc}]. -> BỎ QUA.");
                            duplicateCount++;
                            continue; // Bỏ qua dòng này
                        }

                        // Nếu không trùng, thêm vào danh sách import
                        DataRow newRow = dtClean.NewRow();
                        newRow["MaLop"] = maLopTarget;
                        newRow["HoTen"] = hoTen;
                        newRow["NgaySinh"] = ngaySinh;
                        newRow["GioiTinh"] = row.Table.Columns.Contains("GioiTinh") ? row["GioiTinh"] : DBNull.Value;
                        newRow["SDTPhuHuynh"] = row.Table.Columns.Contains("SDTPhuHuynh") ? row["SDTPhuHuynh"] : DBNull.Value;
                        newRow["DiaChi"] = row.Table.Columns.Contains("DiaChi") ? row["DiaChi"] : DBNull.Value;
                        newRow["DanToc"] = row.Table.Columns.Contains("DanToc") ? row["DanToc"] : DBNull.Value;
                        dtClean.Rows.Add(newRow);
                        validCount++;
                    }

                    if (validCount == 0 && duplicateCount == 0)
                    {
                        e.Result = new { Success = false, Message = "Không có dòng hợp lệ!", Log = logBuilder.ToString() };
                        return;
                    }
                    else if (validCount == 0 && duplicateCount > 0)
                    {
                        e.Result = new { Success = true, Message = "Tất cả HS đã tồn tại!", Log = logBuilder.ToString() };
                        return;
                    }

                    // Gọi Database Helper
                    dynamic dbResult = (_importType == ImportType.HocSinhTheoLop)
                        ? DatabaseHelper.ImportStudentsToClass(dtClean, _contextId)
                        : DatabaseHelper.ImportStudentsFromDataTable(dtClean);

                    string finalMsg = "Hoàn tất!";
                    if (duplicateCount > 0) finalMsg += $" (Bỏ qua {duplicateCount} HS trùng)";

                    e.Result = new { Success = true, Message = finalMsg, Log = $"Thêm mới: {dbResult.Success} HS.\n" + logBuilder.ToString() };
                }

                // ----------------------------------------------------------------------
                // 2. IMPORT PHÂN CÔNG (Giữ nguyên)
                // ----------------------------------------------------------------------
                else if (_importType == ImportType.PhanCong)
                {
                    // 1. Kiểm tra cấu trúc file Excel
                    if (!ValidateColumns(dtRaw, new string[] { "TenMon", "TenGV" }, out missingColumns))
                    {
                        e.Result = new { Success = false, Message = "Sai cột!", Log = $"Thiếu cột: {missingColumns}" };
                        return;
                    }

                    // 2. Tải dữ liệu danh mục để đối chiếu (Môn học & Giáo viên)
                    DataTable dtAllMon = DatabaseHelper.GetAllSubjects();   // Cần có MaMon, TenMon
                    DataTable dtAllGV = DatabaseHelper.GetAllTeachers();    // Cần có MaGV, Ten

                    StringBuilder logBuilder = new StringBuilder();
                    int successCount = 0;
                    int rowIdx = 1;

                    // 3. Duyệt từng dòng Excel
                    foreach (DataRow row in dtRaw.Rows)
                    {
                        rowIdx++;
                        string tenMonEx = row["TenMon"]?.ToString().Trim();
                        string tenGVEx = row["TenGV"]?.ToString().Trim();

                        // -- Check rỗng --
                        if (string.IsNullOrEmpty(tenMonEx) || string.IsNullOrEmpty(tenGVEx))
                        {
                            logBuilder.AppendLine($"Dòng {rowIdx}: Thiếu Tên môn hoặc Tên giáo viên.");
                            continue;
                        }

                        // -- Check Môn học có tồn tại không --
                        var monDb = dtAllMon.AsEnumerable()
                            .FirstOrDefault(r => r["TenMon"].ToString().Trim().Equals(tenMonEx, StringComparison.OrdinalIgnoreCase));

                        if (monDb == null)
                        {
                            logBuilder.AppendLine($"Dòng {rowIdx}: Môn '{tenMonEx}' không tồn tại trong hệ thống.");
                            continue;
                        }
                        string maMon = monDb["MaMon"].ToString();

                        // -- Check Giáo viên có tồn tại không --
                        var gvDb = dtAllGV.AsEnumerable()
                            .FirstOrDefault(r => r["Ten"].ToString().Trim().Equals(tenGVEx, StringComparison.OrdinalIgnoreCase));

                        if (gvDb == null)
                        {
                            logBuilder.AppendLine($"Dòng {rowIdx}: Giáo viên '{tenGVEx}' không tìm thấy trong danh sách nhân sự.");
                            continue;
                        }
                        string maGV = gvDb["MaGV"].ToString();

                        // -- CHECK CHUYÊN MÔN: Giáo viên này có được phép dạy môn này không? --
                        // Lấy danh sách môn mà GV này phụ trách từ bảng GiaoVien_MonHoc
                        DataTable dtGvSubjects = DatabaseHelper.GetSubjectsByTeacher(maGV);
                        bool isQualified = dtGvSubjects.AsEnumerable()
                            .Any(r => r["MaMon"].ToString() == maMon);

                        if (!isQualified)
                        {
                            // Tùy nhu cầu: Có thể chặn luôn (Error) hoặc chỉ cảnh báo (Warning)
                            // Ở đây tôi để là Lỗi để đảm bảo dữ liệu sạch
                            logBuilder.AppendLine($"Dòng {rowIdx}: GV '{tenGVEx}' chưa đăng ký dạy môn '{tenMonEx}' (Lỗi chuyên môn).");
                            continue;
                        }

                        // -- Thực hiện Phân công (Lưu vào DB) --
                        try
                        {
                            // _contextId ở đây đóng vai trò là MaLop
                            DatabaseHelper.UpdateTeachingAssignment(_contextId, maMon, maGV);
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            logBuilder.AppendLine($"Dòng {rowIdx}: Lỗi lưu CSDL - {ex.Message}");
                        }
                    }

                    // 4. Tổng kết
                    if (successCount == 0)
                    {
                        e.Result = new { Success = false, Message = "Import thất bại!", Log = "Không có dòng nào hợp lệ.\n" + logBuilder.ToString() };
                    }
                    else
                    {
                        string msg = $"Đã phân công thành công: {successCount} môn.";
                        string finalLog = logBuilder.ToString();

                        if (!string.IsNullOrEmpty(finalLog))
                        {
                            msg += " (Có lỗi một số dòng)";
                        }

                        e.Result = new { Success = true, Message = msg, Log = finalLog };
                    }
                }

                // ----------------------------------------------------------------------
                // 3. IMPORT TKB (Giữ nguyên)
                // ----------------------------------------------------------------------
                else if (_importType == ImportType.ThoiKhoaBieu)
                {
                    if (!ValidateColumns(dtRaw, new string[] { "Ngay", "Tiet", "TenMon", "TenLop" }, out missingColumns))
                    {
                        e.Result = new { Success = false, Message = "Sai cột!", Log = $"File Excel thiếu cột: {missingColumns}" };
                        return;
                    }

                    StringBuilder errorLog = new StringBuilder();
                    int validCount = 0;

                    DataTable allLop = DatabaseHelper.GetAllClasses();
                    DataTable assignedSubjects = DatabaseHelper.GetTeacherAssignments(_contextId);

                    HashSet<string> validAssignments = new HashSet<string>();
                    foreach (DataRow row in assignedSubjects.Rows)
                    {
                        string maLopDB = row["MaLop"]?.ToString().Trim();
                        string tenMonDB = row["TenMon"]?.ToString().Trim().ToLower();
                        if (!string.IsNullOrEmpty(maLopDB) && !string.IsNullOrEmpty(tenMonDB))
                        {
                            validAssignments.Add($"{maLopDB}_{tenMonDB}");
                        }
                    }

                    DataTable dtProcessed = new DataTable();
                    dtProcessed.Columns.Add("Ngay", typeof(string));
                    dtProcessed.Columns.Add("Tiet", typeof(int));
                    dtProcessed.Columns.Add("TenMon", typeof(string));
                    dtProcessed.Columns.Add("TenLop", typeof(string));
                    dtProcessed.Columns.Add("GhiChu", typeof(string));
                    dtProcessed.Columns.Add("MauSac", typeof(string));

                    int rowIdx = 1;
                    foreach (DataRow row in dtRaw.Rows)
                    {
                        rowIdx++;

                        DateTime ngayDate;
                        if (!TryParseDate(row["Ngay"], out ngayDate))
                        {
                            errorLog.AppendLine($"Dòng {rowIdx}: Ngày '{row["Ngay"]}' lỗi định dạng.");
                            continue;
                        }

                        string tietRaw = row["Tiet"]?.ToString().Trim();
                        if (string.IsNullOrEmpty(tietRaw) || !double.TryParse(tietRaw, out double tietVal))
                        {
                            errorLog.AppendLine($"Dòng {rowIdx}: Tiết '{tietRaw}' không hợp lệ.");
                            continue;
                        }

                        string tenMon = row["TenMon"]?.ToString().Trim();
                        string tenLop = row["TenLop"]?.ToString().Trim();
                        string ghiChu = row.Table.Columns.Contains("GhiChu") ? row["GhiChu"]?.ToString().Trim() : "";

                        if (string.IsNullOrEmpty(tenMon) || string.IsNullOrEmpty(tenLop))
                        {
                            errorLog.AppendLine($"Dòng {rowIdx}: Thiếu tên môn hoặc tên lớp.");
                            continue;
                        }

                        var lopRow = allLop.AsEnumerable().FirstOrDefault(l => l.Field<string>("TenLop").Equals(tenLop, StringComparison.OrdinalIgnoreCase));
                        if (lopRow == null)
                        {
                            errorLog.AppendLine($"Dòng {rowIdx}: Lớp '{tenLop}' không tồn tại.");
                            continue;
                        }
                        string maLopFound = lopRow.Field<string>("MaLop");

                        string keyCheck = $"{maLopFound}_{tenMon.ToLower()}";
                        if (!validAssignments.Contains(keyCheck))
                        {
                            errorLog.AppendLine($"Dòng {rowIdx}: Lỗi phân công - Bạn chưa được giao dạy môn '{tenMon}' tại lớp '{tenLop}'.");
                            continue;
                        }

                        string mauSac = row.Table.Columns.Contains("MauSac") ? row["MauSac"]?.ToString().Trim() : "";
                        if (string.IsNullOrEmpty(mauSac)) mauSac = GetRandomPastelColor();

                        dtProcessed.Rows.Add(ngayDate.ToString("yyyy-MM-dd"), (int)tietVal, tenMon, tenLop, ghiChu, mauSac);
                        validCount++;
                    }

                    if (validCount == 0)
                    {
                        e.Result = new { Success = false, Message = "Import thất bại!", Log = "Không có dòng dữ liệu nào hợp lệ.\n\nCHI TIẾT LỖI:\n" + errorLog.ToString() };
                        return;
                    }

                    DateTime firstDate = DateTime.Parse(dtProcessed.Rows[0]["Ngay"].ToString());
                    foreach (DataRow r in dtProcessed.Rows)
                    {
                        DateTime d = DateTime.Parse(r["Ngay"].ToString());
                        if (d < firstDate) firstDate = d;
                    }
                    this.FirstImportedDate = firstDate;

                    var resultTKB = DatabaseHelper.ImportTimetableForTeacher(_contextId, dtProcessed);

                    bool isTotalSuccess = resultTKB.Success > 0;
                    string msg = isTotalSuccess ? "Import hoàn tất!" : "Import thất bại!";

                    string finalLog = "";
                    if (isTotalSuccess)
                    {
                        finalLog = $"Đã thêm thành công: {resultTKB.Success} tiết.";
                        if (errorLog.Length > 0)
                        {
                            finalLog += "\n\n--- CÁC DÒNG BỊ BỎ QUA ---\n" + errorLog.ToString();
                        }
                    }
                    else
                    {
                        finalLog = "Không lưu được vào CSDL.\n" + errorLog.ToString();
                    }

                    e.Result = new { Success = isTotalSuccess, Message = msg, Log = finalLog };
                }
            }
            catch (Exception ex)
            {
                e.Result = new { Success = false, Message = "Lỗi hệ thống!", Log = ex.ToString() };
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null) ShowState_Results(false, "Lỗi nghiêm trọng!", e.Error.Message);
            else
            {
                dynamic result = e.Result;
                ShowState_Results(result.Success, result.Message, result.Log);
            }
        }

        #endregion

        #region Window Drag & Paint
        private void frmImportExcel_Paint(object sender, PaintEventArgs e) { e.Graphics.DrawRectangle(new Pen(Color.FromArgb(222, 226, 230)), 0, 0, Width - 1, Height - 1); }
        private void pnlTopBar_MouseDown(object sender, MouseEventArgs e) { if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); } }
        private void lblClose_Click(object sender, EventArgs e) { this.Close(); }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (backgroundWorker != null) { backgroundWorker.DoWork -= backgroundWorker_DoWork; backgroundWorker.RunWorkerCompleted -= backgroundWorker_RunWorkerCompleted; }
                if (components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}