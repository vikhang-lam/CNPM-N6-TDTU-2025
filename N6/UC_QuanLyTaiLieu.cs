using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PdfiumViewer;

namespace N6
{
    public partial class UC_QuanLyTaiLieu : UserControl
    {
        private string maGV;
        private string storagePath;

        public UC_QuanLyTaiLieu(string maGVien)
        {
            InitializeComponent();
            maGV = maGVien;

            // Tạo thư mục lưu trữ riêng nếu chưa có
            storagePath = Path.Combine(Application.StartupPath, "TaiLieu");
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);

            LoadTaiLieu(false); // tài liệu của tôi
            LoadTaiLieu(true);  // tài liệu được chia sẻ
        }

        private void LoadTaiLieu(bool shared = false)
        {
            FlowLayoutPanel targetPanel = shared ? flowSharedDocs : flowMyDocs;
            targetPanel.Controls.Clear();

            DataTable dt = shared
                ? DatabaseHelper.GetTaiLieuShared()
                : DatabaseHelper.GetTaiLieuByGV(maGV);

            foreach (DataRow r in dt.Rows)
            {
                Panel card = new Panel
                {
                    Width = 200,
                    Height = 160,
                    Margin = new Padding(15),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label lblIcon = new Label
                {
                    Text = "📄",
                    Font = new Font("Segoe UI Emoji", 28),
                    Dock = DockStyle.Top,
                    Height = 50,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                Label lblName = new Label
                {
                    Text = r["TenTL"].ToString(),
                    Dock = DockStyle.Top,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                FlowLayoutPanel panelButtons = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 40,
                    FlowDirection = FlowDirection.LeftToRight
                };

                // Nút Xem
                Button btnView = new Button { Text = "👁 Xem", Width = 55, Height = 28 };
                btnView.Click += (s, e) =>
                {
                    try
                    {
                        string path = r["Kieu"].ToString();
                        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                        {
                            MessageBox.Show("File không tồn tại: " + path);
                            return;
                        }

                        string ext = Path.GetExtension(path).ToLower();
                        if (ext == ".pdf")
                        {
                            Form viewer = new Form();
                            viewer.Text = "Xem PDF - " + r["TenTL"].ToString();
                            viewer.Size = new Size(900, 600);

                            var pdfViewer = new PdfViewer();
                            pdfViewer.Dock = DockStyle.Fill;
                            pdfViewer.Document = PdfDocument.Load(path);

                            viewer.Controls.Add(pdfViewer);
                            viewer.ShowDialog();
                        }
                        else
                        {
                            System.Diagnostics.Process.Start(path);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi mở file: " + ex.Message);
                    }
                };

                // Nút Chia sẻ (chỉ hiển thị trong tab tài liệu của tôi)
                Button btnShare = new Button { Text = "🔗 Chia sẻ", Width = 65, Height = 28 };
                btnShare.Click += (s, e) =>
                {
                    try
                    {
                        DatabaseHelper.ShareTaiLieu(r["MaTL"].ToString());
                        MessageBox.Show("Đã chia sẻ!");
                        LoadTaiLieu(false);
                        LoadTaiLieu(true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi chia sẻ: " + ex.Message);
                    }
                };

                // Nút Xóa
                Button btnDelete = new Button { Text = "🗑 Xóa", Width = 55, Height = 28 };
                btnDelete.Click += (s, e) =>
                {
                    if (MessageBox.Show("Bạn có chắc chắn muốn xóa tài liệu này?",
                        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            DatabaseHelper.DeleteTaiLieu(r["MaTL"].ToString());
                            LoadTaiLieu(shared);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                        }
                    }
                };

                // Thêm nút vào card
                if (!shared) panelButtons.Controls.Add(btnShare);
                panelButtons.Controls.Add(btnView);
                panelButtons.Controls.Add(btnDelete);

                card.Controls.Add(panelButtons);
                card.Controls.Add(lblName);
                card.Controls.Add(lblIcon);

                targetPanel.Controls.Add(card);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = Path.GetFileName(ofd.FileName);

                    if (string.IsNullOrEmpty(maGV))
                    {
                        MessageBox.Show("Không xác định được giáo viên hiện tại!");
                        return;
                    }

                    // Copy file vào thư mục lưu trữ riêng
                    string destPath = Path.Combine(storagePath, fileName);
                    File.Copy(ofd.FileName, destPath, true);

                    // Lưu vào DB
                    DatabaseHelper.InsertTaiLieu(maGV, fileName, "Tài liệu mới", destPath, "Riêng tư");

                    LoadTaiLieu(false);
                    MessageBox.Show("Tải tài liệu thành công!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải tài liệu: " + ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTaiLieu(false);
            LoadTaiLieu(true);
        }
    }
}
