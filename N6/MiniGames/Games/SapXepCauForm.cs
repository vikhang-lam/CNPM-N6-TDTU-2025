using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Form trò chơi "Sắp xếp câu": người chơi click các từ theo thứ tự để tạo câu đúng.
    /// </summary>
    public class SapXepCauForm : GameFormWithMusic
    {
        #region Helper types

        private class DoubleBufferedFlowLayoutPanel : FlowLayoutPanel
        {
            public DoubleBufferedFlowLayoutPanel()
            {
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
                this.UpdateStyles();
            }
        }

        #endregion

        #region Fields

        private List<SentenceScrambleItem> _sentences;
        private int _currentSentenceIndex = 0;
        private int _correctCount = 0;
        private DoubleBufferedFlowLayoutPanel pnlChoices;
        private DoubleBufferedFlowLayoutPanel pnlAnswer;
        private Label lblInstruction;
        private Label lblOriginalSentence;
        private Label lblProgress;
        private RoundedButton btnCheck;
        private RoundedButton btnReset;
        private RoundedButton btnShowHint;
        private bool _showingHint = false;
        private bool isCompleted = false;

        #endregion

        #region Constructor & Initialization

        /// <summary>
        /// Khởi tạo SapXepCauForm với danh sách câu.
        /// </summary>
        /// <param name="sentences">Danh sách SentenceScrambleItem chứa trường CorrectSentence.</param>
        public SapXepCauForm(List<SentenceScrambleItem> sentences)
        {
            if (sentences == null || sentences.Count == 0)
            {
                CloseWithWarning("Không có dữ liệu để bắt đầu game.");
                return;
            }

            _sentences = sentences.Where(s => !string.IsNullOrWhiteSpace(s.CorrectSentence)).ToList();

            if (_sentences.Count == 0)
            {
                CloseWithWarning("Không có câu hợp lệ để bắt đầu game!");
                return;
            }

            InitializeComponent();
            MusicPlayer.PlaySpecificMusic("MNG06");
            LoadCurrentSentence();
        }

        /// <summary>
        /// Khởi tạo các control UI cho form Sắp xếp câu.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "Sắp xếp câu";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 650);
            this.DoubleBuffered = true;

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

            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Header
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.Transparent
            };

            Label lblTitle = new Label
            {
                Text = "SẮP XẾP CÂU",
                Font = new Font("Lexend", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(141, 94, 61),
                Location = new Point(350, 50),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            lblProgress = new Label
            {
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(19, 104, 206),
                BackColor = Color.Transparent
            };

            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblProgress });

            Panel pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.Transparent,
                Padding = new Padding(20)
            };

            lblInstruction = new Label
            {
                Text = "🎯 Hãy click vào các từ theo thứ tự đúng để tạo thành câu hoàn chỉnh:",
                Location = new Point(20, 45),
                Height = 30,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                BackColor = Color.Transparent
            };

            lblOriginalSentence = new Label
            {
                Text = "",
                Location = new Point(20, 75),
                Height = 30,
                Font = new Font("Segoe UI", 11F, FontStyle.Italic),
                ForeColor = Color.FromArgb(40, 167, 69),
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false,
                AutoSize = false,
                BackColor = Color.Transparent
            };

            lblInstruction.Width = this.ClientSize.Width - 40;
            lblOriginalSentence.Width = this.ClientSize.Width - 40;
            this.Resize += (s, e) =>
            {
                try
                {
                    lblInstruction.Width = Math.Max(100, this.ClientSize.Width - 40);
                    lblOriginalSentence.Width = Math.Max(100, this.ClientSize.Width - 40);
                }
                catch { }
            };

            pnlTop.Controls.Clear();
            pnlTop.Controls.AddRange(new Control[] { lblInstruction, lblOriginalSentence });

            pnlAnswer = new DoubleBufferedFlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 160,
                BackColor = Color.FromArgb(150, 255, 255, 255),
                Padding = new Padding(12),
                Margin = new Padding(10),
                BorderStyle = BorderStyle.None,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            pnlChoices = new DoubleBufferedFlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.Transparent,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            Panel pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                BackColor = Color.Transparent
            };

            var pnlControl = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 25, 20, 20)
            };

            btnCheck = new RoundedButton
            {
                Text = "✅ Kiểm tra",
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                CornerRadius = 8,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnCheck.Click += BtnCheck_Click;

            btnReset = new RoundedButton
            {
                Text = "🔄 Làm lại",
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                CornerRadius = 8,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnReset.Click += BtnReset_Click;

            btnShowHint = new RoundedButton
            {
                Text = "💡 Gợi ý",
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                CornerRadius = 8,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnShowHint.Click += BtnShowHint_Click;

            pnlControl.Controls.AddRange(new Control[] { btnCheck, btnReset, btnShowHint });
            pnlBottom.Controls.Add(pnlControl);

            this.Controls.AddRange(new Control[] { pnlChoices, pnlAnswer, pnlBottom, pnlTop, pnlHeader });
        }

        #endregion

        #region Game Logic

        /// <summary>
        /// Tải câu hiện tại lên giao diện, xáo trộn từ và tạo nút từ cho pnlChoices.
        /// </summary>
        private void LoadCurrentSentence()
        {
            if (_currentSentenceIndex >= _sentences.Count)
            {
                EndAllSentences();
                return;
            }

            var currentSentence = _sentences[_currentSentenceIndex];
            lblProgress.Text = $" Câu {_currentSentenceIndex + 1}/{_sentences.Count} | Đúng: {_correctCount}";

            isCompleted = false;
            btnCheck.Enabled = true;
            btnCheck.Visible = true;

            btnShowHint.Text = "💡 Gợi ý";
            btnShowHint.BackColor = Color.FromArgb(255, 152, 0);
            _showingHint = false;
            lblOriginalSentence.Visible = false;

            var words = currentSentence.CorrectSentence
                                      .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                      .OrderBy(x => Guid.NewGuid())
                                      .ToList();

            pnlChoices.SuspendLayout();
            pnlAnswer.SuspendLayout();
            pnlChoices.Controls.Clear();
            pnlAnswer.Controls.Clear();

            foreach (var word in words)
            {
                CreateWordButton(word, pnlChoices);
            }

            pnlChoices.ResumeLayout(true);
            pnlAnswer.ResumeLayout(true);

            lblOriginalSentence.Text = GenerateHint(currentSentence.CorrectSentence);
        }

        /// <summary>
        /// Toggle hiển thị gợi ý.
        /// </summary>
        private void BtnShowHint_Click(object sender, EventArgs e)
        {
            if (!_showingHint)
            {
                lblOriginalSentence.Visible = true;
                btnShowHint.Text = "🙈 Ẩn gợi ý";
                btnShowHint.BackColor = Color.FromArgb(108, 117, 125);
                _showingHint = true;
            }
            else
            {
                lblOriginalSentence.Visible = false;
                btnShowHint.Text = "💡 Gợi ý";
                btnShowHint.BackColor = Color.FromArgb(255, 152, 0);
                _showingHint = false;
            }
        }

        private string GenerateHint(string correctSentence)
        {
            if (string.IsNullOrWhiteSpace(correctSentence))
            {
                return "💡 Gợi ý: (không có dữ liệu)";
            }

            var words = correctSentence.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int n = words.Length;

            if (n == 1) return $"💡 Gợi ý: \"{words[0]}\"";
            if (n == 2) return $"💡 Gợi ý: \"{words[0]} _\"";

            var hintParts = new List<string> { words[0] };
            for (int i = 1; i < n - 1; i++)
            {
                int maskLen = Math.Min(8, words[i].Length);
                hintParts.Add(new string('_', maskLen));
            }
            hintParts.Add(words[n - 1]);

            return $"💡 Gợi ý: \"{string.Join(" ", hintParts)}\"";
        }

        private RoundedButton CreateWordButton(string word, FlowLayoutPanel parent)
        {
            var btn = new RoundedButton
            {
                Text = word,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12, 6, 12, 6),
                Margin = new Padding(6),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                CornerRadius = 16,
                Cursor = Cursors.Hand,
                TabStop = false // reduce focus redraws
            };

            btn.Click += WordButton_Click;
            parent.Controls.Add(btn);
            return btn;
        }

        #endregion

        #region UI Events (immediate update + vertical scroll support)

        /// <summary>
        /// Move the clicked word immediately and scroll the target vertically to reveal it.
        /// This avoids animated overlays and reduces redraw flicker by using double-buffered panels
        /// and SuspendLayout/ResumeLayout during reparenting.
        /// </summary>
        private void WordButton_Click(object sender, EventArgs e)
        {
            if (isCompleted) return;
            if (!(sender is RoundedButton btn)) return;

            var prevColor = btn.BackColor;
            btn.BackColor = ControlPaint.Dark(prevColor, 0.06f);

            var source = btn.Parent as FlowLayoutPanel;
            var target = ReferenceEquals(source, pnlChoices) ? pnlAnswer : pnlChoices;

            source?.SuspendLayout();
            target?.SuspendLayout();

            try
            {
                source?.Controls.Remove(btn);
                target.Controls.Add(btn);

                if (ReferenceEquals(target, pnlAnswer))
                {
                    btn.BackColor = Color.FromArgb(23, 162, 184);
                }
                else
                {
                    btn.BackColor = Color.FromArgb(40, 167, 69);
                }

                target.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        target.ScrollControlIntoView(btn);
                    }
                    catch { }
                }));
            }
            finally
            {
                source?.ResumeLayout(true);
                target?.ResumeLayout(true);
            }
        }

        private async void BtnCheck_Click(object sender, EventArgs e)
        {
            if (pnlAnswer.Controls.Count == 0)
            {
                MessageBox.Show("Hãy chọn ít nhất một từ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var currentSentence = _sentences[_currentSentenceIndex];
            string userAnswer = string.Join(" ", pnlAnswer.Controls.OfType<RoundedButton>().Select(b => b.Text));
            string correctSentence = currentSentence.CorrectSentence;

            btnCheck.Enabled = false;
            btnCheck.Text = "Đang kiểm tra...";

            await Task.Delay(300);

            bool isCorrect = userAnswer.Equals(correctSentence, StringComparison.OrdinalIgnoreCase);

            pnlAnswer.Enabled = false;
            pnlChoices.Enabled = false;
            btnReset.Enabled = false;
            btnShowHint.Enabled = false;

            foreach (RoundedButton b in pnlAnswer.Controls.OfType<RoundedButton>())
            {
                b.BackColor = isCorrect ? Color.FromArgb(76, 175, 80) : Color.FromArgb(255, 118, 117);
            }

            AnswerPopupResult res;
            if (isCorrect)
            {
                _correctCount++;
                lblProgress.Text = $" Câu {_currentSentenceIndex + 1}/{_sentences.Count} | Đúng: {_correctCount}";
                isCompleted = true;
                string nextActionText = (_currentSentenceIndex < _sentences.Count - 1) ? "Tiếp theo" : "Xem Kết quả";
                res = ShowAnswerPopup(true, "Tuyệt vời! Chính xác!", nextActionText, showRetry: false);
            }
            else
            {
                string nextActionText = (_currentSentenceIndex < _sentences.Count - 1) ? "Tiếp theo" : "Xem Kết quả";
                res = ShowAnswerPopup(false, "Tiếc quá, chưa đúng!", nextActionText, showRetry: true);
            }

            if (res == AnswerPopupResult.Retry)
            {
                BtnReset_Click(null, null);
            }
            else if (res == AnswerPopupResult.Next)
            {
                if (_currentSentenceIndex < _sentences.Count - 1)
                {
                    _currentSentenceIndex++;
                    LoadCurrentSentence();
                }
                else
                {
                    EndAllSentences();
                }
            }

            btnCheck.Enabled = true;
            btnCheck.Text = "✅ Kiểm tra";
            pnlAnswer.Enabled = true;
            pnlChoices.Enabled = true;
            btnReset.Enabled = true;
            btnShowHint.Enabled = true;

            if (!isCorrect && res != AnswerPopupResult.Retry)
            {
                foreach (RoundedButton b in pnlAnswer.Controls.OfType<RoundedButton>())
                {
                    b.BackColor = Color.FromArgb(23, 162, 184);
                }
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            var buttonsInAnswer = pnlAnswer.Controls.OfType<RoundedButton>().ToArray();

            pnlAnswer.SuspendLayout();
            pnlChoices.SuspendLayout();

            foreach (var btn in buttonsInAnswer)
            {
                pnlAnswer.Controls.Remove(btn);
                pnlChoices.Controls.Add(btn);
                btn.BackColor = Color.FromArgb(40, 167, 69);
            }

            pnlAnswer.ResumeLayout(true);
            pnlChoices.ResumeLayout(true);

            isCompleted = false;
            _showingHint = false;
            btnShowHint.Text = "💡 Gợi ý";
            btnShowHint.BackColor = Color.FromArgb(255, 152, 0);
            lblOriginalSentence.Visible = false;

            btnCheck.Enabled = true;
            btnReset.Enabled = true;
            btnShowHint.Enabled = true;
            pnlAnswer.Enabled = true;
            pnlChoices.Enabled = true;

            btnCheck.Text = "✅ Kiểm tra";
        }

        #endregion

        #region End / Results

        /// <summary>
        /// Hiển thị kết quả cuối cùng cho toàn bộ danh sách câu.
        /// </summary>
        private void EndAllSentences()
        {
            ShowFinalResultDialog(_correctCount, _sentences.Count);
        }

        #endregion
    }
}