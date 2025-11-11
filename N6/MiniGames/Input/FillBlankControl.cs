using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Control cho câu hỏi dạng "Điền từ".
    /// </summary>
    public class FillBlankControl : UserControl
    {
        #region Fields

        // TextBox chứa nội dung câu hỏi.
        public TextBox TxtQuestion { get; private set; }

        // TextBox chứa đáp án.
        public TextBox TxtAnswer { get; private set; }

        // Nút xóa control này khỏi danh sách.
        public Button BtnRemove { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Khởi tạo FillBlankControl và xây dựng giao diện.
        /// </summary>
        public FillBlankControl()
        {
            // THU NHỎ CHIỀU CAO: 140 → 115
            this.Size = new Size(720, 115);
            this.BackColor = Color.White;
            this.Margin = new Padding(5, 10, 5, 10);
            this.BorderStyle = BorderStyle.None;

            // Vẽ shadow và rounded corners
            this.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Shadow effect
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 4, 4, this.Width - 4, this.Height - 4);
                }

                // Background với rounded corners
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, this.Width - 1, this.Height - 1), 12))
                {
                    using (var brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    using (var pen = new Pen(Color.FromArgb(220, 230, 240), 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            BuildUi();
        }

        #endregion

        #region UI Build Control

        /// <summary>
        /// Xây dựng các control con và bố cục.
        /// </summary>
        private void BuildUi()
        {
            // ===== HEADER CARD =====
            Panel pnlCardHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(720, 35),
                BackColor = Color.FromArgb(245, 248, 250)
            };

            Label lblCardIcon = new Label
            {
                Text = "📝",
                Font = new Font("Segoe UI Emoji", 14F),
                Location = new Point(14, 6),
                AutoSize = true
            };

            Label lblCardTitle = new Label
            {
                Text = "Câu hỏi Điền từ",
                Font = new Font("Lexend", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(50, 8),
                AutoSize = true
            };

            pnlCardHeader.Controls.AddRange(new Control[] { lblCardIcon, lblCardTitle });

            // ===== NÚT XÓA - DI CHUYỂN LÊN HEADER =====
            BtnRemove = new Button
            {
                Text = "🗑️ Xóa",
                Location = new Point(640, 3),
                Size = new Size(70, 29),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            BtnRemove.FlatAppearance.BorderSize = 0;
            BtnRemove.MouseEnter += (s, e) => BtnRemove.BackColor = Color.FromArgb(200, 35, 51);
            BtnRemove.MouseLeave += (s, e) => BtnRemove.BackColor = Color.FromArgb(220, 53, 69);

            // ===== CỘT TRÁI - CÂU HỎI =====
            Label lblQuestionLabel = new Label
            {
                Text = "💡 Câu hỏi (sử dụng ___ cho chỗ trống):",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Location = new Point(20, 45),
                AutoSize = true
            };

            TxtQuestion = new TextBox
            {
                Location = new Point(20, 65),
                Size = new Size(450, 30),
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            PlaceholderProvider.SetPlaceholder(TxtQuestion, "Ví dụ: Con ___ là loài vật quý hiếm");

            // ===== CỘT PHẢI - ĐÁP ÁN =====
            Label lblAnswerLabel = new Label
            {
                Text = "✓ Đáp án đúng:",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                Location = new Point(490, 45),
                AutoSize = true
            };

            TxtAnswer = new TextBox
            {
                Location = new Point(490, 65),
                Size = new Size(210, 30),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(212, 237, 218),
                ForeColor = Color.FromArgb(40, 167, 69),
                TextAlign = HorizontalAlignment.Center
            };

            PlaceholderProvider.SetPlaceholder(TxtAnswer, "Ví dụ: gấu trúc");

            // ===== ADD NÚT XÓA VÀO HEADER =====
            pnlCardHeader.Controls.Add(BtnRemove);

            this.Controls.AddRange(new Control[]
            {
                pnlCardHeader,
                lblQuestionLabel,
                TxtQuestion,
                lblAnswerLabel,
                TxtAnswer
            });
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

        #region Set Data Methods

        /// <summary>
        /// Lấy dữ liệu câu hỏi từ control.
        /// </summary>
        /// <returns>Đối tượng FillBlankQuestion chứa nội dung câu và đáp án.</returns>
        public FillBlankQuestion GetData()
        {
            return new FillBlankQuestion
            {
                QuestionText = TxtQuestion.Text,
                Answer = TxtAnswer.Text
            };
        }

        /// <summary>
        /// Gán dữ liệu vào control từ một FillBlankQuestion.
        /// </summary>
        /// <param name="q">Dữ liệu câu hỏi cần gán.</param>
        public void SetData(FillBlankQuestion q)
        {
            TxtQuestion.Text = q.QuestionText;
            TxtQuestion.ForeColor = Color.FromArgb(64, 64, 64);
            TxtAnswer.Text = q.Answer;
            TxtAnswer.ForeColor = Color.FromArgb(40, 167, 69);
        }

        #endregion
    }
}