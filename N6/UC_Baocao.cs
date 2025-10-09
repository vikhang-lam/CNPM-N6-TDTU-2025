using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_BaoCao : UserControl
    {
        private string maGV;

        public UC_BaoCao(string maGVien)
        {
            maGV = maGVien;
            InitializeComponent();
            LoadDuLieu();
        }

        private void LoadDuLieu()
        {
            // Load danh sách khối
            cboKhoi.Items.Clear();
            cboKhoi.Items.AddRange(new object[] { "Khối 1", "Khối 2", "Khối 3", "Khối 4", "Khối 5", "Khối 6", "Khối 7", "Khối 8", "Khối 9" });

            // Load danh sách lớp từ database
            LoadDanhSachLop();
        }

        private void LoadDanhSachLop()
        {
            try
            {
                DataTable dtLop = DatabaseHelper.GetLopByGiaoVien(maGV);

                // Bind đúng: hiển thị TenLop nhưng giá trị là MaLop
                cboLop.DataSource = null;
                cboLop.Items.Clear();
                cboLop.DisplayMember = "TenLop";
                cboLop.ValueMember = "MaLop";
                cboLop.DataSource = dtLop;
                cboLop.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboLoaiBaoCao_SelectedIndexChanged(object sender, EventArgs e)
        {
            string loaiBaoCao = cboLoaiBaoCao.SelectedItem?.ToString();

            // Ẩn tất cả controls trước
            cboKhoi.Visible = false;
            cboLop.Visible = false;
            cboHocKy.Visible = false;

            switch (loaiBaoCao)
            {
                case "Báo cáo chuyên cần":
                    cboLop.Visible = true;
                    break;
                case "Bảng điểm học kỳ":
                    cboLop.Visible = true;
                    cboHocKy.Visible = true;
                    break;
                case "Hồ sơ học sinh":
                    cboLop.Visible = true;
                    break;
                case "Thống kê tổng hợp khối":
                    cboKhoi.Visible = true;
                    break;
            }

            // Xóa dữ liệu cũ trong grid
            dgvDuLieu.DataSource = null;
        }

        private void btnXuatBaoCao_Click(object sender, EventArgs e)
        {
            if (cboLoaiBaoCao.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn loại báo cáo!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string loaiBaoCao = cboLoaiBaoCao.SelectedItem.ToString();

            try
            {
                DataTable dtBaoCao = null;

                switch (loaiBaoCao)
                {
                    case "Báo cáo chuyên cần":
                        {
                            var maLop = cboLop.SelectedValue?.ToString();
                            if (string.IsNullOrEmpty(maLop))
                            {
                                MessageBox.Show("Vui lòng chọn lớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            dtBaoCao = DatabaseHelper.GetBaoCaoChuyenCan(maLop);
                            break;
                        }

                    case "Bảng điểm học kỳ":
                        {
                            var maLop = cboLop.SelectedValue?.ToString();
                            if (string.IsNullOrEmpty(maLop) || cboHocKy.SelectedItem == null)
                            {
                                MessageBox.Show("Vui lòng chọn lớp và học kỳ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            int hocKy = cboHocKy.SelectedIndex + 1;
                            dtBaoCao = DatabaseHelper.GetBangDiemHocKy(maLop, hocKy);
                            break;
                        }

                    case "Hồ sơ học sinh":
                        {
                            var maLop = cboLop.SelectedValue?.ToString();
                            if (string.IsNullOrEmpty(maLop))
                            {
                                MessageBox.Show("Vui lòng chọn lớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            dtBaoCao = DatabaseHelper.GetHoSoHocSinh(maLop);
                            break;
                        }

                    case "Thống kê tổng hợp khối":
                        {
                            var khoi = cboKhoi.SelectedItem?.ToString();
                            if (string.IsNullOrEmpty(khoi))
                            {
                                MessageBox.Show("Vui lòng chọn khối!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            dtBaoCao = DatabaseHelper.GetThongKeKhoi(khoi);
                            break;
                        }
                }

                if (dtBaoCao != null && dtBaoCao.Rows.Count > 0)
                {
                    dgvDuLieu.AutoGenerateColumns = true;
                    dgvDuLieu.DataSource = dtBaoCao;
                    MessageBox.Show($"Đã tải {dtBaoCao.Rows.Count} bản ghi cho báo cáo {loaiBaoCao}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    dgvDuLieu.DataSource = null;
                    MessageBox.Show("Không có dữ liệu cho báo cáo này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (dgvDuLieu.DataSource == null || dgvDuLieu.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng tạo báo cáo trước khi xuất Excel!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files (*.csv)|*.csv";
                saveFileDialog.Title = "Lưu file Excel";
                saveFileDialog.FileName = $"BaoCao_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportHelper.ExportToExcel(dgvDuLieu, saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXuatPDF_Click(object sender, EventArgs e)
        {
            if (dgvDuLieu.DataSource == null || dgvDuLieu.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng tạo báo cáo trước khi xuất PDF!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                saveFileDialog.Title = "Lưu file PDF";
                saveFileDialog.FileName = $"BaoCao_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string reportTitle = "BÁO CÁO";
                    if (cboLoaiBaoCao.SelectedItem != null)
                    {
                        reportTitle = cboLoaiBaoCao.SelectedItem.ToString().ToUpper();
                    }
                    
                    ExportHelper.ExportToPDF(dgvDuLieu, saveFileDialog.FileName, reportTitle);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}