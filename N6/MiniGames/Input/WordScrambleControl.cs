using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Control cho một mục Word Scramble (ảnh gợi ý + câu hỏi + đáp án).
    /// </summary>
    public class WordScrambleControl : UserControl
    {
        #region Fields / Properties

        // Tên file ảnh (hiển thị).
        public TextBox TxtImageName { get; private set; }

        // TextBox chứa gợi ý / câu hỏi.
        public TextBox TxtQuestion { get; private set; }

        // TextBox chứa đáp án.
        public TextBox TxtAnswer { get; private set; }

        // Nút xóa control.
        public Button BtnRemove { get; private set; }

        // Nút mở dialog chọn ảnh.
        public Button BtnBrowseImage { get; private set; }

        // PictureBox xem trước ảnh.
        public PictureBox PicPreview { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Khởi tạo control và xây dựng giao diện.
        /// </summary>
        public WordScrambleControl()
        {
            this.Size = new Size(720, 180);
            this.BackColor = Color.White;
            this.Margin = new Padding(5, 8, 15, 8);
            this.BorderStyle = BorderStyle.None;

            // Rounded corners và shadow
            this.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 4, 4, this.Width - 4, this.Height - 4);
                }

                // Background with rounded corners
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, this.Width - 1, this.Height - 1), 12))
                {
                    using (var brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    using (var pen = new Pen(Color.FromArgb(230, 230, 230), 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            BuildUi();
        }

        #endregion

        #region UI Build

        /// <summary>
        /// Xây dựng giao diện chi tiết cho WordScrambleControl.
        /// </summary>
        private void BuildUi()
        {
            // ===== LEFT SECTION: IMAGE PREVIEW =====
            Panel pnlImageSection = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(120, 140),
                BackColor = Color.Transparent
            };

            PicPreview = new PictureBox
            {
                Location = new Point(0, 0),
                Size = new Size(120, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.None
            };

            PicPreview.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(220, 220, 220), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, PicPreview.Width - 1, PicPreview.Height - 1);
                }
            };

            // 1. THU NHỎ NÚT "CHỌN ẢNH"
            BtnBrowseImage = new Button
            {
                Text = "📁 Chọn ảnh",
                Location = new Point(0, 108),
                Size = new Size(120, 28),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            BtnBrowseImage.FlatAppearance.BorderSize = 0;
            BtnBrowseImage.Click += BtnBrowseImage_Click;
            BtnBrowseImage.MouseEnter += (s, e) => BtnBrowseImage.BackColor = Color.FromArgb(0, 103, 235);
            BtnBrowseImage.MouseLeave += (s, e) => BtnBrowseImage.BackColor = Color.FromArgb(0, 123, 255);

            pnlImageSection.Controls.AddRange(new Control[] { PicPreview, BtnBrowseImage });

            // ===== MIDDLE SECTION: FORM INPUTS =====
            Panel pnlFormSection = new Panel
            {
                Location = new Point(155, 20),
                Size = new Size(390, 140),
                BackColor = Color.Transparent
            };

            Label lblImagePath = new Label
            {
                Text = "🖼️ Hình ảnh gợi ý:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Location = new Point(0, 5),
                AutoSize = true
            };

            TxtImageName = new TextBox
            {
                Location = new Point(0, 28),
                Size = new Size(390, 28),
                Font = new Font("Segoe UI", 9F),
                ReadOnly = true,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            PlaceholderProvider.SetPlaceholder(TxtImageName, "Chưa chọn ảnh...");

            Label lblQuestion = new Label
            {
                Text = "💡 Câu hỏi/Gợi ý:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 193, 7),
                Location = new Point(0, 68),
                AutoSize = true
            };

            TxtQuestion = new TextBox
            {
                Location = new Point(0, 91),
                Size = new Size(390, 28),
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            PlaceholderProvider.SetPlaceholder(TxtQuestion, "Ví dụ: Hãy ghép câu hoàn chỉnh...");

            pnlFormSection.Controls.AddRange(new Control[]
            {
                lblImagePath, TxtImageName,
                lblQuestion, TxtQuestion
            });

            // ===== RIGHT SECTION: ANSWER + DELETE =====
            Panel pnlRightSection = new Panel
            {
                Location = new Point(560, 20),
                Size = new Size(150, 140),
                BackColor = Color.Transparent
            };

            Label lblAnswerCheck = new Label
            {
                Text = "✓",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                Location = new Point(0, 3),
                AutoSize = true
            };

            Label lblAnswerLabel = new Label
            {
                Text = "Đáp án:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                Location = new Point(25, 5),
                AutoSize = true
            };

            TxtAnswer = new TextBox
            {
                Location = new Point(0, 28),
                Size = new Size(150, 70),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(212, 237, 218),
                ForeColor = Color.FromArgb(40, 167, 69),
                TextAlign = HorizontalAlignment.Center,
                Multiline = true
            };

            PlaceholderProvider.SetPlaceholder(TxtAnswer, "ĐÁP ÁN");

            // Biến đổi nhập thành chữ hoa (giữ logic)
            TxtAnswer.TextChanged += (s, e) =>
            {
                if (TxtAnswer.ForeColor != Color.Gray)
                {
                    int pos = TxtAnswer.SelectionStart;
                    TxtAnswer.Text = TxtAnswer.Text.ToUpper();
                    TxtAnswer.SelectionStart = pos;
                }
            };

            BtnRemove = new Button
            {
                Text = "🗑️ Xóa",
                Location = new Point(0, 105),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            BtnRemove.FlatAppearance.BorderSize = 0;
            BtnRemove.MouseEnter += (s, e) => BtnRemove.BackColor = Color.FromArgb(200, 35, 51);
            BtnRemove.MouseLeave += (s, e) => BtnRemove.BackColor = Color.FromArgb(220, 53, 69);

            pnlRightSection.Controls.AddRange(new Control[] { lblAnswerCheck, lblAnswerLabel, TxtAnswer, BtnRemove });

            this.Controls.AddRange(new Control[] { pnlImageSection, pnlFormSection, pnlRightSection });
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Tạo GraphicsPath bo góc (helper).
        /// </summary>
        /// <param name="rect">Hình chữ nhật.</param>
        /// <param name="radius">Bán kính bo góc.</param>
        /// <returns>GraphicsPath tương ứng.</returns>
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Xử lý chọn ảnh từ đĩa, hiển thị preview và lưu đường dẫn vào Tag.
        /// </summary>
        private void BtnBrowseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                ofd.Title = "Chọn hình ảnh gợi ý";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (PicPreview.Image != null)
                        {
                            PicPreview.Image.Dispose();
                        }

                        // Lấy ảnh tạm rồi clone vào PictureBox để tránh lock file
                        using (var tempImage = Image.FromFile(ofd.FileName))
                        {
                            PicPreview.Image = new Bitmap(tempImage);
                        }

                        TxtImageName.Text = Path.GetFileName(ofd.FileName);
                        TxtImageName.ForeColor = Color.Black;
                        TxtImageName.Tag = ofd.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể tải ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

        #region Set Data Methods

        /// <summary>
        /// Lấy dữ liệu WordScrambleItem từ control này.
        /// </summary>
        /// <returns>WordScrambleItem chứa đường dẫn/ tên ảnh, câu hỏi và đáp án.</returns>
        public WordScrambleItem GetData()
        {
            return new WordScrambleItem
            {
                ImageHintResourceName = TxtImageName.Tag?.ToString() ?? TxtImageName.Text,
                Question = NormalizeTextBoxText(TxtQuestion),
                Answer = NormalizeTextBoxText(TxtAnswer).ToUpper()
            };
        }

        /// <summary>
        /// Cố gắng lấy dữ liệu và validate; trả về false nếu không hợp lệ.
        /// </summary>
        /// <param name="item">Item hợp lệ.</param>
        /// <param name="validationMessage">Thông báo khi không hợp lệ.</param>
        /// <returns>true nếu hợp lệ.</returns>
        public bool TryGetData(out WordScrambleItem item, out string validationMessage)
        {
            item = GetData();

            if (string.IsNullOrWhiteSpace(item.Question))
            {
                validationMessage = "Bạn phải nhập câu hỏi/gợi ý cho mục Ghép chữ.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(item.Answer))
            {
                validationMessage = "Bạn phải nhập đáp án cho mục Ghép chữ.";
                return false;
            }

            validationMessage = null;
            return true;
        }

        /// <summary>
        /// Gán dữ liệu cho control từ một WordScrambleItem.
        /// </summary>
        /// <param name="item">Dữ liệu cần gán.</param>
        public void SetData(WordScrambleItem item)
        {
            TxtImageName.Text = item.ImageHintResourceName;
            TxtImageName.ForeColor = Color.Black;

            try
            {
                if (File.Exists(item.ImageHintResourceName))
                {
                    if (PicPreview.Image != null)
                    {
                        PicPreview.Image.Dispose();
                    }

                    using (var tempImage = Image.FromFile(item.ImageHintResourceName))
                    {
                        PicPreview.Image = new Bitmap(tempImage);
                    }

                    TxtImageName.Tag = item.ImageHintResourceName;
                }
                else
                {
                    var resourceImage = Properties.Resources.ResourceManager.GetObject(item.ImageHintResourceName);
                    if (resourceImage != null)
                    {
                        PicPreview.Image = (Image)resourceImage;
                    }
                }
            }
            catch
            {
                // Nếu không tải được ảnh thì đặt Null (giữ logic ban đầu)
                PicPreview.Image = null;
            }

            TxtQuestion.Text = item.Question;
            TxtQuestion.ForeColor = Color.Black;
            TxtAnswer.Text = item.Answer;
            TxtAnswer.ForeColor = Color.FromArgb(40, 167, 69);
        }

        #endregion

        #region Helpers

        private string NormalizeTextBoxText(TextBox tb)
        {
            if (tb == null) return string.Empty;
            if (tb.ForeColor == Color.Gray) return string.Empty; // placeholder
            return (tb.Text ?? string.Empty).Trim();
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dọn dẹp tài nguyên (hình ảnh).
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                PicPreview?.Image?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}