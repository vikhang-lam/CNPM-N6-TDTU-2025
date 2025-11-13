using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Form trò chơi "Điền từ vào chỗ trống" cho phép người chơi nhập đáp án, kiểm tra và xem kết quả.
    /// </summary>
    public class DienTuForm : GameFormWithMusic
    {
        #region Fields

        private List<FillBlankQuestion> _questions;
        private int _currentIndex = 0;
        private int _correctCount = 0;

        // UI controls
        private Label lblQuestion;
        private Label lblResult;
        private Label lblProgress;
        private TextBox txtAnswer;
        private RoundedButton btnCheck;
        private RoundedButton btnSkip;
        private RoundedButton btnNext;
        private Panel pnlMainCard;

        #endregion

        #region Constructor & Initialization

        /// <summary>
        /// Khởi tạo form Điền từ với danh sách câu hỏi.
        /// </summary>
        /// <param name="questions">Danh sách câu hỏi loại FillBlankQuestion.</param>
        public DienTuForm(List<FillBlankQuestion> questions)
        {
            if (questions == null || questions.Count == 0 || string.IsNullOrWhiteSpace(questions[0].QuestionText))
            {
                CloseWithWarning("Không có dữ liệu để bắt đầu game.");
                return;
            }

            _questions = questions;
            InitializeComponent();
            MusicPlayer.PlaySpecificMusic("MNG07");
            LoadQuestion();
        }

        /// <summary>
        /// Thiết lập các control UI cho form.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "Điền từ";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 650);
            this.MaximizeBox = false;

            try
            {
                this.BackgroundImage = Image.FromFile(@"..\..\Resources\flashcard_bg.png");
                this.BackgroundImageLayout = ImageLayout.Stretch;
                this.BackColor = Color.FromArgb(215, 235, 225);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải ảnh nền flashcard: " + ex.Message);
                this.BackColor = Color.FromArgb(245, 247, 250);
            }

            // Header banner: Title + Progress
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.Transparent
            };

            Label lblGameTitle = new Label
            {
                Text = "ĐIỀN TỪ VÀO CHỖ TRỐNG",
                Font = new Font("Lexend", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(141, 94, 61),
                Location = new Point(10, 45),
                AutoSize = false,
                Width = this.ClientSize.Width,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            lblProgress = new Label
            {
                Location = new Point(30, 10),
                AutoSize = true,
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(19, 104, 206),
                BackColor = Color.Transparent
            };

            pnlHeader.Controls.AddRange(new Control[] { lblGameTitle, lblProgress });

            Label lblInstruction = new Label
            {
                Text = "✍️ Hãy điền từ thích hợp vào chỗ trống (___)",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 350) / 2, 150),
                BackColor = Color.Transparent
            };

            this.Resize += (s, e) =>
            {
                if (lblGameTitle != null)
                {
                    lblGameTitle.Width = this.ClientSize.Width;
                }

                if (lblInstruction != null)
                {
                    using (Graphics g = this.CreateGraphics())
                    {
                        SizeF size = g.MeasureString(lblInstruction.Text, lblInstruction.Font);
                        lblInstruction.Location = new Point((this.ClientSize.Width - (int)size.Width) / 2, 95);
                    }
                }

                if (pnlMainCard != null)
                {
                    pnlMainCard.Location = new Point((this.ClientSize.Width - 820) / 2, 130);
                }
            };

            pnlMainCard = new Panel
            {
                Size = new Size(820, 200),
                Location = new Point((this.ClientSize.Width - 820) / 2, 180),
                Padding = new Padding(30)
            };

            pnlMainCard.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 5, 5, pnlMainCard.Width - 5, pnlMainCard.Height - 5);
                }

                // Background with rounded corners
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, pnlMainCard.Width - 1, pnlMainCard.Height - 1), 15))
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

            lblQuestion = new Label
            {
                Location = new Point(30, 20),
                Size = new Size(760, 40),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };

            txtAnswer = new TextBox
            {
                Font = new Font("Segoe UI", 16F),
                Location = new Point(30, 70),
                Width = 760,
                Height = 40,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Enter để submit
            txtAnswer.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    btnCheck.PerformClick();
                }
            };

            lblResult = new Label
            {
                Location = new Point(30, 130),
                Size = new Size(760, 40),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false
            };

            pnlMainCard.Controls.AddRange(new Control[] { lblQuestion, txtAnswer, lblResult });

            Panel pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 15, 20, 15)
            };

            var pnlButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Location = new Point((pnlBottom.ClientSize.Width - 330) / 2, 0),
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 10),
                WrapContents = false,
                BackColor = Color.Transparent
            };

            btnCheck = new RoundedButton
            {
                Text = "✅ Kiểm tra",
                Size = new Size(150, 50),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                CornerRadius = 10,
                Margin = new Padding(0, 0, 15, 0)
            };

            btnCheck.Click += BtnCheck_Click;

            btnSkip = new RoundedButton
            {
                Text = "⏭️ Bỏ qua",
                Size = new Size(150, 50),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                CornerRadius = 10,
                Margin = new Padding(0, 0, 15, 0)
            };

            btnSkip.Click += BtnSkip_Click;

            btnNext = new RoundedButton
            {
                Text = "➡️ Tiếp tục",
                Size = new Size(150, 50),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = CorrectColor,
                ForeColor = Color.White,
                CornerRadius = 10,
                Margin = new Padding(0, 0, 15, 0),
                Visible = false
            };

            btnNext.Click += BtnNext_Click;

            pnlButtons.Controls.AddRange(new Control[] { btnCheck, btnSkip, btnNext });
            pnlBottom.Controls.Add(pnlButtons);

            this.Controls.AddRange(new Control[] { pnlMainCard, pnlBottom, lblInstruction, pnlHeader });
        }

        #endregion

        #region UI Helpers

        /// <summary>
        /// Tạo GraphicsPath hình chữ nhật bo góc.
        /// </summary>
        /// <param name="rect">Hình chữ nhật nguồn.</param>
        /// <param name="radius">Bán kính bo góc.</param>
        /// <returns>GraphicsPath chứa đường path bo góc.</returns>
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        #endregion

        #region Game Logic & Events

        /// <summary>
        /// Tải câu hỏi hiện tại lên giao diện hoặc hiển thị kết quả nếu đã hết câu.
        /// </summary>
        private void LoadQuestion()
        {
            if (_currentIndex >= _questions.Count)
            {
                ShowFinalResult();
                return;
            }

            // Cập nhật tiến trình
            lblProgress.Text = $" Câu {_currentIndex + 1}/{_questions.Count} | Đúng: {_correctCount}";

            // Hiển thị câu hỏi
            lblQuestion.Text = _questions[_currentIndex].QuestionText;

            // Reset UI
            txtAnswer.Clear();
            txtAnswer.BackColor = Color.White;
            txtAnswer.Enabled = true;
            txtAnswer.Focus();

            lblResult.Visible = false;
            btnCheck.Visible = true;
            btnSkip.Visible = true;
            btnNext.Visible = false;
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Kiểm tra.
        /// </summary>
        private async void BtnCheck_Click(object sender, EventArgs e)
        {
            string userAnswer = txtAnswer.Text.Trim();
            string correctAnswer = _questions[_currentIndex].Answer;

            txtAnswer.Enabled = false;
            btnCheck.Enabled = false;
            btnSkip.Enabled = false;

            // Delay nhỏ để UX (hiệu ứng)
            await Task.Delay(500);

            bool isCorrect = string.Equals(userAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase);

            txtAnswer.BackColor = isCorrect ? CorrectColor : IncorrectColor;
            lblResult.Visible = false;

            string nextActionText = (_currentIndex < _questions.Count - 1) ? "Tiếp theo" : "Xem Kết quả";
            AnswerPopupResult res;

            if (isCorrect)
            {
                _correctCount++;
                res = ShowAnswerPopup(true, "Tuyệt vời! Chính xác!", nextActionText, showRetry: false);
            }
            else
            {
                res = ShowAnswerPopup(false, "Tiếc quá, chưa đúng!", nextActionText, showRetry: true);
            }

            if (res == AnswerPopupResult.Retry)
            {
                // Cho phép người chơi thử lại
                txtAnswer.Enabled = true;
                btnCheck.Enabled = true;
                btnSkip.Enabled = true;

                txtAnswer.BackColor = Color.White;
                txtAnswer.Focus();
            }
            else if (res == AnswerPopupResult.Next)
            {
                if (_currentIndex < _questions.Count - 1)
                {
                    _currentIndex++;
                    LoadQuestion();
                }
                else
                {
                    ShowFinalResultDialog(_correctCount, _questions.Count);
                }
            }

            btnCheck.Enabled = true;
            btnSkip.Enabled = true;

            if (res != AnswerPopupResult.Retry)
            {
                txtAnswer.BackColor = Color.White;
            }
        }

        /// <summary>
        /// Bỏ qua câu hỏi hiện tại (hiển thị đáp án và tùy chọn tiếp).
        /// </summary>
        private void BtnSkip_Click(object sender, EventArgs e)
        {
            string correctAnswer = _questions[_currentIndex].Answer;
            string nextActionText = (_currentIndex < _questions.Count - 1) ? "Tiếp theo" : "Xem Kết quả";

            AnswerPopupResult res = ShowAnswerPopup(
                false,
                $"Đã bỏ qua. Đáp án là: {correctAnswer}",
                nextActionText,
                showRetry: false
            );

            if (res == AnswerPopupResult.Next)
            {
                if (_currentIndex < _questions.Count - 1)
                {
                    _currentIndex++;
                    LoadQuestion();
                }
                else
                {
                    ShowFinalResultDialog(_correctCount, _questions.Count);
                }
            }
        }

        /// <summary>
        /// Chuyển đến câu tiếp theo (dùng khi người chơi chọn Next).
        /// </summary>
        private void BtnNext_Click(object sender, EventArgs e)
        {
            _currentIndex++;
            LoadQuestion();
        }

        /// <summary>
        /// Hiển thị kết quả cuối cùng.
        /// </summary>
        private void ShowFinalResult()
        {
            ShowFinalResultDialog(_correctCount, _questions.Count);
        }

        #endregion
    }
}