using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Form chơi Quiz nhanh với hỗ trợ nhạc nền và giao diện các lựa chọn tròn.
    /// </summary>
    public class QuizGameForm : GameFormWithMusic
    {
        #region Fields

        private List<QuizQuestion> _questions;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private Label lblQuestion, lblQuestionCount;
        private List<RoundedButton> optionButtons;
        private Panel pnlQuestionCard;

        #endregion

        #region Constructor & Initialization

        /// <summary>
        /// Khởi tạo QuizGameForm với danh sách câu hỏi.
        /// </summary>
        /// <param name="questions">Danh sách câu hỏi ngang (QuizQuestion).</param>
        public QuizGameForm(List<QuizQuestion> questions)
        {
            if (questions == null || questions.Count == 0)
            {
                CloseWithWarning();
                return;
            }

            _questions = questions;
            InitializeComponent();
            MusicPlayer.PlaySpecificMusic("MNG01");
            LoadQuestion();
        }

        /// <summary>
        /// Thiết lập các control UI động cho form.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "Quiz nhanh";
            this.Size = new Size(960, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            try
            {
                this.BackgroundImage = Image.FromFile(@"..\..\Resources\quiz_bg.png");
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch (Exception)
            {
                this.BackColor = Color.FromArgb(240, 247, 255);
            }

            Panel pnlHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(this.Width, 100),
                BackColor = Color.Transparent
            };

            this.Controls.Add(pnlHeader);

            Label lblGameTitle = new Label
            {
                Text = "Quiz nhanh",
                ForeColor = Color.FromArgb(17, 45, 78),
                Font = new Font("Lexend", 20F, FontStyle.Bold),
                BackColor = Color.Transparent,
                Location = new Point(350, 25),
                AutoSize = true
            };

            lblQuestionCount = new Label
            {
                Location = new Point(this.Width/2 - 450, 5),
                Size = new Size(210, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(220, 53, 69),
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                BackColor = Color.Transparent
            };

            pnlHeader.Controls.AddRange(new Control[]
            {
                lblGameTitle,
                lblQuestionCount
            });

            pnlQuestionCard = new Panel
            {
                Location = new Point(250, 100),
                Size = new Size(450, 180),
                BackColor = Color.Transparent
            };

            // Custom paint để vẽ bubble có mũi tên
            pnlQuestionCard.Paint += (s, e) =>
            {
                Rectangle rect = pnlQuestionCard.ClientRectangle;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle bubbleRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height - 15);

                using (GraphicsPath path = GetRoundedRectangle(bubbleRect, 15))
                {
                    // Tọa độ mũi nhọn
                    int tipX = rect.Width / 2;
                    int tipY = rect.Height - 15;
                    int tipSize = 15;

                    // tạo mũi tên xuống
                    path.AddLine(tipX - tipSize, tipY - 1, tipX, tipY + tipSize);
                    path.AddLine(tipX, tipY + tipSize, tipX + tipSize, tipY - 1);
                    path.CloseFigure();

                    // vẽ nền bubble bán trong suốt để không còn "white box"
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(230, 255, 255, 255)))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    using (Pen pen = new Pen(Color.FromArgb(141, 94, 61), 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            lblQuestion = new Label
            {
                Location = new Point(10, 5),
                // đặt rộng hơn để chữ không bị wrap sớm; bật AutoEllipsis để hiển thị "..." nếu quá dài
                Size = new Size(pnlQuestionCard.Width - 20, pnlQuestionCard.Height - 30),
                AutoSize = false,
                AutoEllipsis = true,
                Font = new Font("Lexend", 16F, FontStyle.Regular),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            pnlQuestionCard.Controls.Add(lblQuestion);

            TableLayoutPanel tlp = new TableLayoutPanel
            {
                Location = new Point(140, 420),
                Size = new Size(680, 240),
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent
            };

            // reduce flicker
            typeof(TableLayoutPanel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(tlp, true);

            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            optionButtons = new List<RoundedButton>();
            string[] prefixes = { "A", "B", "C", "D" };
            Color[] colors =
            {
                Color.FromArgb(87, 187, 247),
                Color.FromArgb(255, 189, 89),
                Color.FromArgb(29, 209, 161),
                Color.FromArgb(255, 118, 117)
            };

            for (int i = 0; i < 4; i++)
            {
                var btn = new RoundedButton
                {
                    Font = new Font("Arial", 20F, FontStyle.Bold),
                    Tag = prefixes[i],
                    CornerRadius = 24,
                    BackColor = colors[i],
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(12),
                    Cursor = Cursors.Hand
                };

                btn.FlatAppearance.BorderSize = 0;

                // Hover effect: nhẹ làm sáng khi di chuột nếu button còn enable
                btn.MouseEnter += (s, e) =>
                {
                    var b = s as RoundedButton;
                    if (b.Enabled)
                    {
                        b.BackColor = ControlPaint.Light(b.BackColor, 0.08f);
                    }
                };

                // Trả lại màu gốc khi rời chuột
                btn.MouseLeave += (s, e) =>
                {
                    var b = s as RoundedButton;
                    if (b.Enabled)
                    {
                        int idx = Array.IndexOf(prefixes, b.Tag.ToString());
                        if (idx >= 0)
                        {
                            b.BackColor = colors[idx];
                        }
                    }
                };

                optionButtons.Add(btn);
                tlp.Controls.Add(btn, i % 2, i / 2);
            }

            // add controls
            this.Controls.AddRange(new Control[]
            {
                pnlHeader,
                pnlQuestionCard,
                tlp
            });
        }

        #endregion

        #region UI Helpers

        /// <summary>
        /// Tạo GraphicsPath hình chữ nhật bo góc.
        /// </summary>
        /// <param name="rect">Hình chữ nhật nguồn.</param>
        /// <param name="radius">Bán kính bo góc.</param>
        /// <returns>GraphicsPath chứa đường path bo góc.</returns>
        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);

            // Góc trên bên trái
            path.AddArc(arc, 180, 90);

            // Góc trên bên phải
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Góc dưới bên phải
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Góc dưới bên trái
            arc.X = rect.X;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        #endregion

        #region Game Logic

        /// <summary>
        /// Tải câu hỏi hiện tại lên giao diện, hoặc kết thúc game nếu hết câu hỏi.
        /// </summary>
        private void LoadQuestion()
        {
            if (currentQuestionIndex < _questions.Count)
            {
                lblQuestionCount.Text = $" Câu {currentQuestionIndex + 1}/{_questions.Count} | Đúng: {score}";
                QuizQuestion q = _questions[currentQuestionIndex];
                lblQuestion.Text = q.QuestionText;

                Color[] colors =
                {
                    Color.FromArgb(87, 187, 247),
                    Color.FromArgb(255, 189, 89),
                    Color.FromArgb(29, 209, 161),
                    Color.FromArgb(255, 118, 117)
                };

                for (int i = 0; i < 4; i++)
                {
                    optionButtons[i].Text = $"{((char)('A' + i))}. {q.Options[i]}";
                    optionButtons[i].Click -= OptionButton_Click;
                    optionButtons[i].Click += OptionButton_Click;
                    optionButtons[i].BackColor = colors[i];
                    optionButtons[i].ForeColor = Color.White;
                    optionButtons[i].Enabled = true;

                    optionButtons[i].Font = new Font("Arial", 16F, FontStyle.Bold);
                }
            }
            else
            {
                EndGame();
            }
        }

        /// <summary>
        /// Đặt lại trạng thái các nút cho câu hỏi hiện tại (màu, văn bản, enabled).
        /// </summary>
        private void ResetButtonsForCurrentQuestion()
        {
            if (_questions == null || currentQuestionIndex >= _questions.Count)
            {
                return;
            }

            var q = _questions[currentQuestionIndex];
            Color[] colors =
            {
                Color.FromArgb(87, 187, 247),
                Color.FromArgb(255, 189, 89),
                Color.FromArgb(29, 209, 161),
                Color.FromArgb(255, 118, 117)
            };

            for (int i = 0; i < optionButtons.Count && i < q.Options.Count; i++)
            {
                string prefix = ((char)('A' + i)).ToString();
                optionButtons[i].Text = $"{prefix}. {q.Options[i]}";
                optionButtons[i].BackColor = colors[i];
                optionButtons[i].ForeColor = Color.White;
                optionButtons[i].Font = new Font("Lexend", 16F, FontStyle.Bold);
                optionButtons[i].Enabled = true;
            }
        }

        #endregion

        #region UI Events

        /// <summary>
        /// Xử lý khi người chơi chọn một phương án.
        /// </summary>
        /// <param name="sender">Button được click.</param>
        /// <param name="e">Event args.</param>
        private async void OptionButton_Click(object sender, EventArgs e)
        {
            if (_questions == null || currentQuestionIndex >= _questions.Count)
            {
                return;
            }

            var clickedButton = sender as RoundedButton;
            if (clickedButton == null)
            {
                return;
            }

            // Vô hiệu toàn bộ nút để tránh nhiều click
            optionButtons.ForEach(b => b.Enabled = false);

            var q = _questions[currentQuestionIndex];
            bool isLast = (currentQuestionIndex == _questions.Count - 1);

            string prefix = clickedButton.Tag?.ToString() ?? "";
            string originalText = q.Options[Array.IndexOf(optionButtons.ToArray(), clickedButton)];
            if (string.IsNullOrWhiteSpace(originalText))
            {
                var t = clickedButton.Text;
                originalText = t.Contains(". ") ? t.Substring(t.IndexOf(' ') + 1) : t;
            }

            bool isCorrect = string.Equals(clickedButton.Tag?.ToString(), q.CorrectAnswer, StringComparison.OrdinalIgnoreCase);

            if (isCorrect)
            {
                score++;
                lblQuestionCount.Text = $" Câu {currentQuestionIndex + 1}/{_questions.Count} | Đúng: {score}";

                clickedButton.BackColor = Color.FromArgb(40, 167, 69);
                clickedButton.ForeColor = Color.White;
                clickedButton.Text = $"{prefix}. ✓ {originalText}";
                clickedButton.Font = new Font("Lexend", 20F, FontStyle.Bold);

                var primaryText = isLast ? "Kết quả" : "Tiếp theo";
                var res = ShowAnswerPopup(true, "Các bạn giỏi quá! 🎉", primaryText, showRetry: false);

                if (res == AnswerPopupResult.Next)
                {
                    if (isLast)
                    {
                        ShowFinalResultDialog(score, _questions.Count);
                        return;
                    }

                    currentQuestionIndex++;
                    LoadQuestion();
                }
                else
                {
                    // Trường hợp đóng popup khác: cho phép tiếp tục tương tác (hồi enabled)
                    optionButtons.ForEach(b => b.Enabled = true);
                }
            }
            else
            {
                clickedButton.BackColor = Color.FromArgb(220, 53, 69);
                clickedButton.ForeColor = Color.White;
                clickedButton.Text = $"{prefix}. ✗ {originalText}";
                clickedButton.Font = new Font("Lexend", 20F, FontStyle.Bold);

                var primaryText = isLast ? "Kết quả" : "Tiếp theo";
                var res = ShowAnswerPopup(false, "Tiếc quá, chưa đúng!", primaryText, showRetry: true);

                if (res == AnswerPopupResult.Retry)
                {
                    // Người chơi muốn thử lại => reset về trạng thái ban đầu cho câu này
                    ResetButtonsForCurrentQuestion();
                }
                else if (res == AnswerPopupResult.Next)
                {
                    if (isLast)
                    {
                        ShowFinalResultDialog(score, _questions.Count);
                        return;
                    }

                    currentQuestionIndex++;
                    LoadQuestion();
                }
                else
                {
                    // Các trường hợp khác: bật lại tương tác
                    optionButtons.ForEach(b => b.Enabled = true);
                }
            }
        }

        /// <summary>
        /// Kết thúc trò chơi và hiển thị kết quả cuối cùng.
        /// </summary>
        private void EndGame()
        {
            ShowFinalResultDialog(score, _questions.Count);
        }

        #endregion
    }
}