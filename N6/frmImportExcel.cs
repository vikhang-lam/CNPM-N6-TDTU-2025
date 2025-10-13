using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Linq;
using System.Globalization;

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

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

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
            txtErrorLog.Visible = !string.IsNullOrEmpty(errorLog);
            txtErrorLog.Text = errorLog;
            btnImport.Text = "Đóng";
            btnImport.Enabled = true;
            this.DialogResult = DialogResult.OK;
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
                e.Effect = DragDropEffects.Copy;
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

        #region Button Clicks

        private void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "CSV (Phân cách bằng dấu chấm phẩy)|*.csv";
            DataTable template = new DataTable();
            var listSeparator = ";";

            switch (_importType)
            {
                // **FIXED HERE:** Wrapped each string in `new DataColumn()`
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
                    template.Rows.Add("HS002", "10A1", "Trần Thị Bình", "20/08/2010", "Nữ", "0909789123", "456 Đường XYZ, Q2", "Kinh");
                    break;
                case ImportType.PhanCong:
                    saveFileDialog.FileName = $"Mau_PhanCong_{_contextName}.csv";
                    template.Columns.AddRange(new DataColumn[] { new DataColumn("TenMon"), new DataColumn("TenGV") });
                    template.Rows.Add("Toán", "Nguyễn Thị Mai");
                    template.Rows.Add("Vật lý", "Trần Văn Hùng");
                    break;
                case ImportType.ThoiKhoaBieu:
                    saveFileDialog.FileName = $"Mau_TKB_{_contextName}.csv";
                    template.Columns.AddRange(new DataColumn[] {
                        new DataColumn("Ngay"), new DataColumn("Tiet"), new DataColumn("TenMon"),
                        new DataColumn("TenLop"), new DataColumn("GhiChu"), new DataColumn("MauSac")
                    });
                    template.Rows.Add("20/10/2025", "1", "Toán", "10A1", "Ghi chú tiết học", "#FFDDC1");
                    template.Rows.Add("20/10/2025", "2", "Vật lý", "11B2", "", "");
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
                catch (Exception ex) { MessageBox.Show("Lỗi khi tạo file mẫu: " + ex.Message, "Lỗi"); }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (btnImport.Text == "Đóng") { this.Close(); return; }
            if (string.IsNullOrEmpty(_selectedFilePath)) { MessageBox.Show("Vui lòng chọn một file để import."); return; }
            ShowState_InProgress();
            backgroundWorker.RunWorkerAsync();
        }

        #endregion

        #region Background Worker

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DataTable dt = ExcelHelper.ReadExcelFile(_selectedFilePath);
            if (dt == null)
            {
                e.Result = new { Success = false, Message = "File không hợp lệ hoặc không thể đọc.", Log = "" };
                return;
            }

            try
            {
                switch (_importType)
                {
                    case ImportType.HocSinhTheoLop:
                        var resultLop = DatabaseHelper.ImportHocSinhToLop(dt, _contextId);
                        e.Result = new { Success = true, Message = $"Import hoàn tất!", Log = $"Thành công: {resultLop.Success}\nBỏ qua: {resultLop.Skipped}\nThất bại: {resultLop.Failed}" };
                        break;
                    case ImportType.HocSinhTheoKhoi:
                        var resultKhoi = DatabaseHelper.ImportHocSinhFromDataTable(dt);
                        e.Result = new { Success = true, Message = $"Import hoàn tất!", Log = $"Thành công: {resultKhoi.Success}\nBỏ qua: {resultKhoi.Skipped}\nThất bại: {resultKhoi.Failed}" };
                        break;
                    case ImportType.PhanCong:
                        DataTable allMonHoc = DatabaseHelper.GetAllMonHoc();
                        DataTable allGiaoVien = DatabaseHelper.GetAllGiaoVien();
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
                            string maMonCuaGV = gvRow["MaMon"].ToString();

                            if (maMonCuaGV != maMon) { errorList.AppendLine($"- GV '{tenGV}': dạy môn khác, không phải môn '{tenMon}'."); continue; }

                            DatabaseHelper.UpdatePhanCong(_contextId, maMon, maGV);
                            successCount++;
                        }
                        string logPhanCong = $"Thành công: {successCount} môn.\n" + (errorList.Length > 0 ? "Lỗi:\n" + errorList.ToString() : "");
                        e.Result = new { Success = successCount > 0, Message = "Import phân công hoàn tất!", Log = logPhanCong };
                        break;
                    case ImportType.ThoiKhoaBieu:
                        var firstDate = dt.AsEnumerable()
                            .Select(row => {
                                DateTime date;
                                return DateTime.TryParse(row["Ngay"]?.ToString(), out date) ? (DateTime?)date : null;
                            })
                            .Where(d => d.HasValue)
                            .OrderBy(d => d.Value)
                            .FirstOrDefault();

                        this.FirstImportedDate = firstDate;

                        var resultTKB = DatabaseHelper.ImportThoiKhoaBieuForGV(_contextId, dt);
                        e.Result = new { Success = resultTKB.Success > 0, Message = "Import TKB hoàn tất!", Log = $"Thành công: {resultTKB.Success}\nThất bại/Sai dữ liệu: {resultTKB.Failed}" };
                        break;
                }
            }
            catch (Exception ex)
            {
                e.Result = new { Success = false, Message = "Import thất bại!", Log = ex.Message };
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
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
    }
}