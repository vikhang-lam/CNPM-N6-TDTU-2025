using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PdfiumViewer;
using System.Diagnostics; // Thêm
using System.Linq; // Thêm

namespace N6
{
    /// <summary>
    /// UserControl quản lý việc tải lên, xem, chia sẻ và xóa tài liệu.
    /// </summary>
    public partial class UC_QuanLyTaiLieu : UserControl
    {
        private string maGV;
        private string storagePath; // Đường dẫn thư mục lưu trữ tài liệu

        /// <summary>
        /// Khởi tạo UserControl với mã giáo viên.
        /// </summary>
        /// <param name="maGVien">Mã của giáo viên đang đăng nhập.</param>
        public UC_QuanLyTaiLieu(string maGVien)
        {
            InitializeComponent();
            maGV = maGVien;

            // Đảm bảo thư mục lưu trữ tồn tại
            storagePath = Path.Combine(Application.StartupPath, "TaiLieu");
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);

            // Tải danh sách tài liệu
            LoadTaiLieu(false); // Tải tài liệu của tôi
            LoadTaiLieu(true);  // Tải tài liệu được chia sẻ
        }

        /// <summary>
        /// Tải (hoặc tải lại) danh sách tài liệu lên FlowLayoutPanel.
        /// </summary>
        /// <param name="shared">True để tải tài liệu được chia sẻ, False để tải tài liệu cá nhân.</param>
        private void LoadTaiLieu(bool shared = false)
        {
            FlowLayoutPanel targetPanel = shared ? flowSharedDocs : flowMyDocs;

            // Dọn dẹp các control cũ và gỡ sự kiện để tránh memory leak
            foreach (Panel shadowPanel in targetPanel.Controls.OfType<Panel>().ToList())
            {
                // Truy cập vào card bên trong
                var card = shadowPanel.Controls.OfType<Panel>().FirstOrDefault();
                if (card != null)
                {
                    // Truy cập vào TableLayoutPanel chứa các nút
                    var panelButtons = card.Controls.OfType<TableLayoutPanel>().FirstOrDefault();
                    if (panelButtons != null)
                    {
                        // Gỡ sự kiện Click của tất cả các nút bên trong
                        foreach (Button btn in panelButtons.Controls.OfType<Button>().ToList())
                        {
                            btn.Click -= ViewDocument_Click;
                            btn.Click -= DownloadDocument_Click;
                            btn.Click -= ShareDocument_Click;
                            btn.Click -= UnshareDocument_Click;
                            btn.Click -= DeleteDocument_Click;
                        }
                    }
                }
                targetPanel.Controls.Remove(shadowPanel); // Xóa control
                shadowPanel.Dispose(); // Hủy control
            }
            targetPanel.Controls.Clear(); // Dọn dẹp lần cuối

            // Lấy dữ liệu từ CSDL
            DataTable dt = shared
                ? DatabaseHelper.GetSharedDocumentsWithUploader()
                : DatabaseHelper.GetDocumentsByTeacher(maGV);

            // Tạo các thẻ (card) cho từng tài liệu
            foreach (DataRow r in dt.Rows)
            {
                // Tạo bản sao của DataRow để tránh lỗi closure
                DataRow rowCopy = r;
                string maTL = rowCopy["MaTL"].ToString();

                Panel shadowPanel = new Panel { Width = 225, Height = 225, Margin = new Padding(15), BackColor = Color.Gainsboro };
                Panel card = new Panel { Width = 220, Height = 220, BackColor = Color.White, Dock = DockStyle.Fill, Padding = new Padding(10) };
                Label lblIcon = new Label { Text = GetFileIcon(rowCopy["Kieu"].ToString()), Font = new Font("Segoe UI Emoji", 36), Dock = DockStyle.Top, Height = 70, TextAlign = ContentAlignment.MiddleCenter };
                Label lblName = new Label { Text = rowCopy["TenTL"].ToString(), Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold), Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter, AutoEllipsis = true };
                Label lblSharedBy = new Label { Dock = DockStyle.Top, Height = 20, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 8F, FontStyle.Italic) };

                if (shared)
                {
                    lblSharedBy.Text = "bởi " + rowCopy["TenGV"].ToString();
                }

                TableLayoutPanel panelButtons = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 40, ColumnCount = 3, RowCount = 1 };
                panelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                panelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                panelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));

                // Tạo các nút
                Button btnView = CreateModernButton("👁️ Xem", Color.FromArgb(24, 119, 242));
                btnView.Tag = rowCopy; // Lưu DataRow vào Tag
                btnView.Click += ViewDocument_Click;

                Button btnDownload = CreateModernButton("📥 Tải", Color.FromArgb(24, 119, 242));
                btnDownload.Tag = rowCopy;
                btnDownload.Click += DownloadDocument_Click;

                Button btnShare = CreateModernButton("🔗 Chia sẻ", Color.FromArgb(67, 181, 129));
                btnShare.Tag = maTL;
                btnShare.Click += ShareDocument_Click;

                Button btnUnshare = CreateModernButton("🗑 Hủy", Color.FromArgb(114, 118, 125));
                btnUnshare.Tag = maTL;
                btnUnshare.Click += UnshareDocument_Click;

                Button btnDelete = CreateModernButton("❌", Color.FromArgb(237, 66, 69));
                btnDelete.Tag = new Tuple<string, bool>(maTL, shared); // Lưu MaTL và context (tab nào)
                btnDelete.Click += DeleteDocument_Click;

                // Thêm các nút dựa trên ngữ cảnh (chung/cá nhân)
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
                    string trangThai = rowCopy["TrangThaiChiaSe"].ToString();
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

        #region Helper Methods (Hàm hỗ trợ)

        /// <summary>
        /// Tạo một Button với style hiện đại.
        /// </summary>
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

        /// <summary>
        /// Lấy biểu tượng emoji dựa trên đuôi file.
        /// </summary>
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

        #endregion

        #region Actions (Xử lý sự kiện)

        /// <summary>
        /// Xử lý sự kiện xem tài liệu (PDF hoặc mở bằng app mặc định).
        /// </summary>
        private void ViewDocument_Click(object sender, EventArgs e)
        {
            if (!((sender as Button)?.Tag is DataRow row)) return;

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
                    // Mở PDF bằng PdfiumViewer
                    using (Form viewer = new Form())
                    {
                        viewer.Text = "Xem PDF - " + row["TenTL"].ToString();
                        viewer.Size = new Size(900, 700);
                        viewer.StartPosition = FormStartPosition.CenterParent;
                        var pdfViewer = new PdfViewer { Dock = DockStyle.Fill };
                        pdfViewer.Document = PdfDocument.Load(path);
                        viewer.Controls.Add(pdfViewer);
                        viewer.ShowDialog();
                    }
                }
                else
                {
                    // Mở các file khác (Word, Excel...) bằng ứng dụng mặc định
                    System.Diagnostics.Process.Start(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở file: " + ex.Message);
            }
        }

        /// <summary>
        /// Xử lý sự kiện tải tài liệu về máy.
        /// </summary>
        private void DownloadDocument_Click(object sender, EventArgs e)
        {
            if (!((sender as Button)?.Tag is DataRow row)) return;

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

        /// <summary>
        /// Xử lý sự kiện chia sẻ tài liệu.
        /// </summary>
        private void ShareDocument_Click(object sender, EventArgs e)
        {
            string maTL = (sender as Button)?.Tag?.ToString();
            if (string.IsNullOrEmpty(maTL)) return;

            try
            {
                DatabaseHelper.ShareDocument(maTL);
                MessageBox.Show("Đã chia sẻ tài liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnRefresh_Click(null, null); // Tải lại cả 2 tab
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chia sẻ: " + ex.Message);
            }
        }

        /// <summary>
        /// Xử lý sự kiện hủy chia sẻ tài liệu.
        /// </summary>
        private void UnshareDocument_Click(object sender, EventArgs e)
        {
            string maTL = (sender as Button)?.Tag?.ToString();
            if (string.IsNullOrEmpty(maTL)) return;

            try
            {
                DatabaseHelper.UnshareDocument(maTL);
                MessageBox.Show("Đã hủy chia sẻ tài liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnRefresh_Click(null, null); // Tải lại cả 2 tab
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hủy chia sẻ: " + ex.Message);
            }
        }

        /// <summary>
        /// Xử lý sự kiện xóa tài liệu.
        /// </summary>
        private void DeleteDocument_Click(object sender, EventArgs e)
        {
            if (!((sender as Button)?.Tag is Tuple<string, bool> tag)) return;

            string maTL = tag.Item1;
            bool isSharedTab = tag.Item2; // Để biết cần load lại tab nào

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa tài liệu này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteDocument(maTL);
                    LoadTaiLieu(isSharedTab); // Chỉ tải lại tab hiện tại
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút "Tải lên".
        /// </summary>
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

                        // Copy file vào thư mục lưu trữ của ứng dụng
                        string destPath = Path.Combine(storagePath, fileName);
                        File.Copy(ofd.FileName, destPath, true);

                        // Lưu đường dẫn vào CSDL
                        DatabaseHelper.InsertDocument(maGV, fileName, "Tài liệu mới", destPath, "Riêng tư");
                        LoadTaiLieu(false); // Tải lại tab "Tài liệu của tôi"
                        MessageBox.Show("Tải tài liệu thành công!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải tài liệu: " + ex.Message);
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút "Làm mới".
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTaiLieu(false);
            LoadTaiLieu(true);
        }

        /// <summary>
        /// Vẽ đường kẻ viền dưới cho thanh Toolbar.
        /// </summary>
        private void panelToolbar_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panelToolbar.ClientRectangle,
                Color.White, 0, ButtonBorderStyle.None,
                Color.White, 0, ButtonBorderStyle.None,
                Color.White, 0, ButtonBorderStyle.None,
                Color.Gainsboro, 1, ButtonBorderStyle.Solid); // Chỉ vẽ viền dưới
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Gỡ bỏ sự kiện của các control trong Designer
                if (this.btnUpload != null) this.btnUpload.Click -= new System.EventHandler(this.btnUpload_Click);
                if (this.btnRefresh != null) this.btnRefresh.Click -= new System.EventHandler(this.btnRefresh_Click);
                if (this.panelToolbar != null) this.panelToolbar.Paint -= new System.Windows.Forms.PaintEventHandler(this.panelToolbar_Paint);

                // ### PHẦN SỬA LỖI QUAN TRỌNG ###
                // Dọn dẹp các control động (Card) trong cả 2 FlowLayoutPanel
                foreach (FlowLayoutPanel targetPanel in new[] { flowMyDocs, flowSharedDocs })
                {
                    if (targetPanel != null)
                    {
                        // Dùng ToList() để tạo bản sao trước khi thay đổi collection
                        foreach (Panel shadowPanel in targetPanel.Controls.OfType<Panel>().ToList())
                        {
                            var card = shadowPanel.Controls.OfType<Panel>().FirstOrDefault();
                            if (card != null)
                            {
                                var panelButtons = card.Controls.OfType<TableLayoutPanel>().FirstOrDefault();
                                if (panelButtons != null)
                                {
                                    // Gỡ sự kiện Click của tất cả các nút động
                                    foreach (Button btn in panelButtons.Controls.OfType<Button>().ToList())
                                    {
                                        btn.Click -= ViewDocument_Click;
                                        btn.Click -= DownloadDocument_Click;
                                        btn.Click -= ShareDocument_Click;
                                        btn.Click -= UnshareDocument_Click;
                                        btn.Click -= DeleteDocument_Click;
                                    }
                                }
                            }
                            shadowPanel.Dispose(); // Hủy control
                        }
                    }
                }
                // ### KẾT THÚC PHẦN SỬA LỖI ###

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}