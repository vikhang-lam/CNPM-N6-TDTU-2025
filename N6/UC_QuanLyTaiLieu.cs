using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

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

            // Thư mục lưu trữ riêng của app
            storagePath = Path.Combine(Application.StartupPath, "TaiLieu");
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);

            LoadTaiLieu();
        }

        private void LoadTaiLieu()
        {
            flowDocs.Controls.Clear();
            DataTable dt = DatabaseHelper.GetTaiLieuByGV(maGV);

            foreach (DataRow r in dt.Rows)
            {
                Panel card = new Panel();
                card.Width = 180;
                card.Height = 120;
                card.Margin = new Padding(15);
                card.BackColor = Color.White;
                card.BorderStyle = BorderStyle.FixedSingle;

                Label lblIcon = new Label()
                {
                    Text = "📄",
                    Font = new Font("Segoe UI Emoji", 28),
                    Dock = DockStyle.Top,
                    Height = 50,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                Label lblName = new Label()
                {
                    Text = r["TenTL"].ToString(),
                    Dock = DockStyle.Top,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                Label lblDate = new Label()
                {
                    Text = Convert.ToDateTime(r["NgayTaiLen"]).ToShortDateString(),
                    Dock = DockStyle.Bottom,
                    Height = 20,
                    Font = new Font("Segoe UI", 8, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                ContextMenuStrip menu = new ContextMenuStrip();
                menu.Items.Add("👁 Xem", null, (s, e) =>
                {
                    string path = r["Kieu"].ToString(); // lưu full path file

                    if (!File.Exists(path))
                    {
                        MessageBox.Show("File không tồn tại!");
                        return;
                    }

                    string ext = Path.GetExtension(path).ToLower();

                    if (ext == ".pdf")
                    {
                        try
                        {
                            Form viewer = new Form();
                            viewer.Text = "Xem PDF - " + r["TenTL"].ToString();
                            viewer.Size = new Size(900, 600);

                            // dùng PdfiumViewer
                            var pdfViewer = new PdfiumViewer.PdfViewer();
                            pdfViewer.Dock = DockStyle.Fill;
                            pdfViewer.Document = PdfiumViewer.PdfDocument.Load(path);

                            viewer.Controls.Add(pdfViewer);
                            viewer.ShowDialog();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Không mở được PDF: " + ex.Message);
                        }
                    }
                    else
                    {
                        // với file Word/Excel/PowerPoint mở bằng ứng dụng mặc định
                        try
                        {
                            System.Diagnostics.Process.Start(path);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Không mở được file: " + ex.Message);
                        }
                    }
                });

                card.ContextMenuStrip = menu;

                // hover effect
                card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(220, 240, 250);
                card.MouseLeave += (s, e) => card.BackColor = Color.White;

                card.Controls.Add(lblDate);
                card.Controls.Add(lblName);
                card.Controls.Add(lblIcon);

                flowDocs.Controls.Add(card);
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

                    // Copy file vào thư mục lưu trữ riêng của app
                    string destPath = Path.Combine(storagePath, fileName);
                    File.Copy(ofd.FileName, destPath, true);

                    // Lưu đường dẫn file đã copy vào DB (cột Kieu)
                    DatabaseHelper.InsertTaiLieu(maGV, fileName, "Tài liệu mới", destPath, "Riêng tư");

                    LoadTaiLieu();
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
            LoadTaiLieu();
        }
    }
}
