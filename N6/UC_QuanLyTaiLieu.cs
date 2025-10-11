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

            storagePath = Path.Combine(Application.StartupPath, "TaiLieu");
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);

            LoadTaiLieu(false);
            LoadTaiLieu(true);
        }

        // ### REDESIGNED ### Toàn bộ phương thức này được thiết kế lại
        private void LoadTaiLieu(bool shared = false)
        {
            FlowLayoutPanel targetPanel = shared ? flowSharedDocs : flowMyDocs;
            targetPanel.Controls.Clear();

            DataTable dt = shared
                ? DatabaseHelper.GetTaiLieuSharedWithUploader()
                : DatabaseHelper.GetTaiLieuByGV(maGV);

            foreach (DataRow r in dt.Rows)
            {
                // Card container with shadow effect
                Panel shadowPanel = new Panel
                {
                    Width = 225,
                    Height = 225,
                    Margin = new Padding(15),
                    BackColor = Color.Gainsboro // Shadow color
                };

                Panel card = new Panel
                {
                    Width = 220,
                    Height = 220,
                    BackColor = Color.White,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(10)
                };

                Label lblIcon = new Label
                {
                    Text = GetFileIcon(r["Kieu"].ToString()),
                    Font = new Font("Segoe UI Emoji", 36),
                    Dock = DockStyle.Top,
                    Height = 70,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                Label lblName = new Label
                {
                    Text = r["TenTL"].ToString(),
                    Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold),
                    Dock = DockStyle.Top,
                    Height = 40,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoEllipsis = true
                };

                Label lblSharedBy = new Label
                {
                    Dock = DockStyle.Top,
                    Height = 20,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkGray,
                    Font = new Font("Segoe UI", 8F, FontStyle.Italic)
                };

                if (shared)
                {
                    lblSharedBy.Text = "bởi " + r["TenGV"].ToString();
                }

                // Panel for buttons
                TableLayoutPanel panelButtons = new TableLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 40,
                    ColumnCount = 3,
                    RowCount = 1
                };
                panelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                panelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                panelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40)); // For delete button

                // ### REDESIGNED ### Các nút bấm được làm mới với icon và màu sắc
                Button btnView = CreateModernButton("👁️ Xem", Color.FromArgb(24, 119, 242));
                btnView.Click += (s, e) => ViewDocument(r);

                Button btnDownload = CreateModernButton("📥 Tải", Color.FromArgb(24, 119, 242));
                btnDownload.Click += (s, e) => DownloadDocument(r);

                Button btnShare = CreateModernButton("🔗 Chia sẻ", Color.FromArgb(67, 181, 129));
                btnShare.Click += (s, e) => ShareDocument(r["MaTL"].ToString());

                Button btnUnshare = CreateModernButton("🗑 Hủy", Color.FromArgb(114, 118, 125));
                btnUnshare.Click += (s, e) => UnshareDocument(r["MaTL"].ToString());

                Button btnDelete = CreateModernButton("❌", Color.FromArgb(237, 66, 69));
                btnDelete.Click += (s, e) => DeleteDocument(r["MaTL"].ToString(), shared);

                // Add buttons based on context
                if (shared)
                {
                    panelButtons.SetColumnSpan(btnView, 1);
                    panelButtons.SetColumnSpan(btnDownload, 2);
                    panelButtons.Controls.Add(btnView, 0, 0);
                    panelButtons.Controls.Add(btnDownload, 1, 0);
                }
                else
                {
                    panelButtons.Controls.Add(btnView, 0, 0);
                    string trangThai = r["TrangThaiChiaSe"].ToString();
                    if (trangThai == "Chia sẻ")
                    {
                        panelButtons.Controls.Add(btnUnshare, 1, 0);
                    }
                    else
                    {
                        panelButtons.Controls.Add(btnShare, 1, 0);
                    }
                    panelButtons.Controls.Add(btnDelete, 2, 0);
                }

                card.Controls.Add(panelButtons);
                card.Controls.Add(lblSharedBy);
                card.Controls.Add(lblName);
                card.Controls.Add(lblIcon);

                shadowPanel.Controls.Add(card);
                targetPanel.Controls.Add(shadowPanel);
            }
        }

        #region New Helper Methods for Design & Actions

        // ### NEW ### Tạo nút bấm theo phong cách hiện đại
        private Button CreateModernButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                Margin = new Padding(2),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        // ### NEW ### Lấy icon dựa trên đuôi file
        private string GetFileIcon(string path)
        {
            switch (Path.GetExtension(path).ToLower())
            {
                case ".pdf": return "📕";
                case ".doc":
                case ".docx": return "📘";
                case ".xls":
                case ".xlsx": return "📗";
                default: return "📄";
            }
        }

        // ### NEW ### Tách logic xử lý sự kiện ra các hàm riêng
        private void ViewDocument(DataRow row)
        {
            try
            {
                string path = row["Kieu"].ToString();
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                {
                    MessageBox.Show("File không tồn tại hoặc đã bị di chuyển: " + path);
                    return;
                }

                string ext = Path.GetExtension(path).ToLower();
                if (ext == ".pdf")
                {
                    using (Form viewer = new Form())
                    {
                        viewer.Text = "Xem PDF - " + row["TenTL"].ToString();
                        viewer.Size = new Size(900, 700);
                        viewer.StartPosition = FormStartPosition.CenterParent;
                        var pdfViewer = new PdfViewer { Dock = DockStyle.Fill, Document = PdfDocument.Load(path) };
                        viewer.Controls.Add(pdfViewer);
                        viewer.ShowDialog();
                    }
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
        }

        private void DownloadDocument(DataRow row)
        {
            try
            {
                string sourcePath = row["Kieu"].ToString();
                if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                {
                    MessageBox.Show("File nguồn không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.FileName = Path.GetFileName(sourcePath);
                    sfd.Filter = "All files (*.*)|*.*";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(sourcePath, sfd.FileName, true);
                        MessageBox.Show("Tải về thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải file: " + ex.Message);
            }
        }

        private void ShareDocument(string maTL)
        {
            try
            {
                DatabaseHelper.ShareTaiLieu(maTL);
                MessageBox.Show("Đã chia sẻ tài liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chia sẻ: " + ex.Message);
            }
        }

        private void UnshareDocument(string maTL)
        {
            try
            {
                DatabaseHelper.UnshareTaiLieu(maTL);
                MessageBox.Show("Đã hủy chia sẻ tài liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hủy chia sẻ: " + ex.Message);
            }
        }

        private void DeleteDocument(string maTL, bool isSharedTab)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa tài liệu này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteTaiLieu(maTL);
                    LoadTaiLieu(isSharedTab);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                }
            }
        }

        #endregion

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "PDF Files (*.pdf)|*.pdf|Word Documents (*.doc;*.docx)|*.doc;*.docx|All files (*.*)|*.*";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string fileName = Path.GetFileName(ofd.FileName);
                        if (string.IsNullOrEmpty(maGV))
                        {
                            MessageBox.Show("Không xác định được giáo viên hiện tại!");
                            return;
                        }
                        string destPath = Path.Combine(storagePath, fileName);
                        File.Copy(ofd.FileName, destPath, true);
                        DatabaseHelper.InsertTaiLieu(maGV, fileName, "Tài liệu mới", destPath, "Riêng tư");
                        LoadTaiLieu(false);
                        MessageBox.Show("Tải tài liệu thành công!");
                    }
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

        // ### NEW ### Vẽ đường kẻ dưới cho toolbar
        private void panelToolbar_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panelToolbar.ClientRectangle,
                Color.White, 0, ButtonBorderStyle.None,
                Color.White, 0, ButtonBorderStyle.None,
                Color.White, 0, ButtonBorderStyle.None,
                Color.Gainsboro, 1, ButtonBorderStyle.Solid);
        }
    }
}