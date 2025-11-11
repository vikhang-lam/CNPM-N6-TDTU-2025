using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Form trò chơi "Ghép chữ" (Word Scramble) kèm ảnh gợi ý, chức năng gợi ý chữ,
    /// kiểm tra đáp án và thống kê tiến trình.
    /// </summary>
    public class GheChuForm : GameFormWithMusic
    {
        #region Fields

        private List<WordScrambleItem> _items;
        private int _currentItemIndex = 0;
        private int _correctAnswers = 0;
        private PictureBox picHint;
        private Label lblStatus;
        private Label lblProgress;
        private FlowLayoutPanel pnlAnswer;
        private FlowLayoutPanel pnlChoices;
        private RoundedButton btnHint;
        private RoundedButton btnCheck;
        private Panel pnlButtons;
        private int _nextWordHintIndex = 0;

        // UI title
        private Label lblGameTitleTop;

        #endregion

        #region Constructor & Initialization

        /// <summary>
        /// Khởi tạo form Ghép chữ với danh sách item (hình + đáp án).
        /// </summary>
        /// <param name="items">Danh sách WordScrambleItem.</param>
        public GheChuForm(List<WordScrambleItem> items)
        {
            if (items == null || items.Count == 0)
            {
                CloseWithWarning();
                return;
            }

            _items = items;
            InitializeComponent();
            MusicPlayer.PlaySpecificMusic("MNG04");
            LoadCurrentItem();
        }

        /// <summary>
        /// Thiết lập các control UI cho form ghép chữ.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "🧩 Ghép Chữ Đoán Hình";
            this.Size = new Size(1024, 788);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            try
            {
                this.BackgroundImage = Image.FromFile(@"..\..\Resources\ghepchu_bg.png");
                this.BackgroundImageLayout = ImageLayout.Stretch;
                this.BackColor = Color.FromArgb(215, 235, 225);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải ảnh nền flashcard: " + ex.Message);
                this.BackColor = Color.FromArgb(245, 247, 250);
            }

            lblProgress = new Label
            {
                Location = new Point(700, 20),
                AutoSize = true,
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                BackColor = Color.Transparent
            };

            // Title top above image
            lblGameTitleTop = new Label
            {
                Text = "GHÉP CHỮ",
                Font = new Font("Lexend", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(111, 108, 97),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(300, 40),
                Location = new Point(362, 50),
                BackColor = Color.Transparent
            };

            Panel pnlImage = new Panel
            {
                Location = new Point(362, 130),
                Size = new Size(300, 200),
                BackColor = Color.Transparent,
                Padding = new Padding(5)
            };

            pnlImage.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (var pen = new Pen(Color.FromArgb(220, 215, 205), 2))
                {
                    Rectangle rect = new Rectangle(0, 0, pnlImage.Width - 1, pnlImage.Height - 1);
                    using (GraphicsPath path = GetRoundedRectPath(rect, 10))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            picHint = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            pnlImage.Controls.Add(picHint);

            Panel pnlQuestionBg = new Panel
            {
                Location = new Point(92, 330),
                Size = new Size(840, 100),
                BackColor = Color.Transparent,
                Padding = new Padding(10)
            };

            pnlAnswer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                BorderStyle = BorderStyle.None,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = false,
                Location = new Point(0, (pnlQuestionBg.Height - 80) / 2)
            };

            pnlQuestionBg.Controls.Add(pnlAnswer);

            Label lblChoicesTitle = new Label
            {
                Text = "Chọn các chữ cái:",
                Font = new Font("Lexend", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(111, 108, 97),
                Location = new Point(92, 475),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            pnlChoices = new FlowLayoutPanel
            {
                Size = new Size(840, 90),
                Location = new Point(92, 500),
                Padding = new Padding(10),
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.None,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 15, 20, 15)
            };

            btnCheck = new RoundedButton
            {
                Text = "✔️ Kiểm tra",
                Size = new Size(160, 50),
                Location = new Point(100, 15),
                Font = new Font("Lexend", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(87, 187, 247),
                ForeColor = Color.White,
                CornerRadius = 12
            };

            btnCheck.FlatAppearance.BorderSize = 0;
            btnCheck.Click += BtnCheck_Click;
            btnCheck.MouseEnter += (s, e) => btnCheck.BackColor = Color.FromArgb(67, 167, 227);
            btnCheck.MouseLeave += (s, e) => btnCheck.BackColor = Color.FromArgb(87, 187, 247);

            lblStatus = new Label
            {
                Location = new Point(350, 20),
                Size = new Size(324, 40),
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false,
                BackColor = Color.Transparent
            };

            btnHint = new RoundedButton
            {
                Text = "💡 Gợi ý",
                Size = new Size(160, 50),
                Location = new Point(280, 15),
                Font = new Font("Lexend", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(244, 179, 80),
                ForeColor = Color.White,
                CornerRadius = 12,
                Visible = true
            };

            btnHint.FlatAppearance.BorderSize = 0;
            btnHint.Click += BtnHint_Click;
            btnHint.MouseEnter += (s, e) => btnHint.BackColor = Color.FromArgb(229, 159, 60);
            btnHint.MouseLeave += (s, e) => btnHint.BackColor = Color.FromArgb(244, 179, 80);

            // Ensure controls are added once
            pnlButtons.Controls.AddRange(new Control[] { btnCheck, lblStatus, btnHint });

            this.Controls.AddRange(new Control[] { lblGameTitleTop, pnlImage, pnlQuestionBg, lblChoicesTitle, pnlChoices });
            this.Controls.Add(pnlButtons);
            this.Controls.Add(lblProgress);
            lblProgress.BringToFront();
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

        /// <summary>
        /// Vẽ khung cho ô slot (sử dụng Paint event).
        /// </summary>
        private void Slot_Paint(object sender, PaintEventArgs e)
        {
            Label lbl = sender as Label;
            if (lbl == null)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen pen = new Pen(Color.FromArgb(220, 215, 205), 2))
            {
                e.Graphics.DrawRectangle(pen, 1, 1, lbl.Width - 2, lbl.Height - 2);
            }
        }

        #endregion

        #region Game Logic & Events

        /// <summary>
        /// Tải item (ảnh + đáp án) hiện tại lên giao diện, xây dựng các ô đáp án và các nút chữ cái (choices).
        /// </summary>
        private void LoadCurrentItem()
        {
            pnlAnswer.Controls.Clear();
            pnlChoices.Controls.Clear();
            lblStatus.Visible = false;
            pnlChoices.Enabled = true;
            pnlAnswer.Enabled = true;
            _nextWordHintIndex = 0;

            if (_currentItemIndex >= _items.Count)
            {
                EndGame();
                return;
            }

            btnCheck.Visible = true;
            btnHint.Visible = true;
            lblProgress.Text = $" Câu {_currentItemIndex + 1}/{_items.Count} | Đúng: {_correctAnswers}";

            WordScrambleItem current = _items[_currentItemIndex];

            if (picHint.Image != null)
            {
                picHint.Image.Dispose();
                picHint.Image = null;
            }

            // Load image safely: support file path or embedded resources, with fallbacks.
            try
            {
                if (!string.IsNullOrWhiteSpace(current.ImageHintResourceName))
                {
                    if (File.Exists(current.ImageHintResourceName))
                    {
                        using (var fs = new FileStream(current.ImageHintResourceName, FileMode.Open, FileAccess.Read))
                        {
                            using (var ms = new MemoryStream())
                            {
                                fs.CopyTo(ms);
                                ms.Position = 0;
                                picHint.Image = Image.FromStream(ms);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            var resourceImage = Properties.Resources.ResourceManager.GetObject(current.ImageHintResourceName);
                            if (resourceImage != null)
                            {
                                picHint.Image = (Image)resourceImage;
                            }
                            else
                            {
                                picHint.Image = Properties.Resources.placeholder;
                            }
                        }
                        catch
                        {
                            picHint.Image = Properties.Resources.placeholder;
                        }
                    }
                }
                else
                {
                    picHint.Image = Properties.Resources.placeholder;
                }
            }
            catch (OutOfMemoryException)
            {
                // Hình ảnh corrupt hoặc quá lớn -> dùng placeholder
                MessageBox.Show(
                    "Ảnh gợi ý bị lỗi hoặc quá lớn.\nSử dụng ảnh placeholder thay thế.",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                try
                {
                    picHint.Image = Properties.Resources.placeholder;
                }
                catch
                {
                    picHint.Image = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    picHint.Image = Properties.Resources.placeholder;
                }
                catch
                {
                    picHint.Image = null;
                }
            }

            // Tách thành các từ (nếu có)
            string[] words = current.Answer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Tính toán kích thước các slot để căn giữa
            int totalWidth = 0;
            int slotWidth = 55;
            int slotMargin = 3;
            int spacerWidth = 15;

            foreach (string word in words)
            {
                totalWidth += word.Length * (slotWidth + slotMargin * 2);
            }

            if (words.Length > 1)
            {
                totalWidth += (words.Length - 1) * spacerWidth;
            }

            int availableWidth = pnlAnswer.Width - 10;
            int leftPadding = Math.Max(5, (availableWidth - totalWidth) / 2);

            pnlAnswer.SuspendLayout();

            if (leftPadding > 5)
            {
                var leftSpacer = new Label
                {
                    Text = "",
                    Size = new Size(leftPadding, 55),
                    Margin = new Padding(0),
                    BorderStyle = BorderStyle.None,
                    BackColor = Color.Transparent,
                    Tag = "SPACER"
                };

                pnlAnswer.Controls.Add(leftSpacer);
            }

            // Tạo các ô slot cho mỗi ký tự, giữa các từ chèn spacer
            for (int wordIndex = 0; wordIndex < words.Length; wordIndex++)
            {
                string word = words[wordIndex];

                for (int i = 0; i < word.Length; i++)
                {
                    var slot = new Label
                    {
                        Text = "",
                        Size = new Size(slotWidth, 55),
                        Margin = new Padding(slotMargin),
                        Font = new Font("Lexend", 24F, FontStyle.Bold),
                        BorderStyle = BorderStyle.None,
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.White,
                        ForeColor = Color.FromArgb(64, 64, 64),
                        Cursor = Cursors.Hand
                    };

                    slot.Click += Answer_Click;
                    slot.Paint += Slot_Paint;
                    pnlAnswer.Controls.Add(slot);
                }

                if (wordIndex < words.Length - 1)
                {
                    var spacer = new Label
                    {
                        Text = "",
                        Size = new Size(spacerWidth, 55),
                        Margin = new Padding(0),
                        BorderStyle = BorderStyle.None,
                        BackColor = Color.Transparent,
                        Tag = "SPACER"
                    };

                    pnlAnswer.Controls.Add(spacer);
                }
            }

            pnlAnswer.ResumeLayout();

            // Chuẩn bị các chữ cái lựa chọn (shuffled)
            var random = new Random();
            string answerWithoutSpaces = current.Answer.Replace(" ", "");
            string shuffledAnswer = new string(answerWithoutSpaces.ToCharArray()
                                                                .OrderBy(c => random.Next())
                                                                .ToArray());

            foreach (char c in shuffledAnswer)
            {
                var choice = new RoundedButton
                {
                    Text = c.ToString(),
                    Size = new Size(65, 65),
                    Font = new Font("Lexend", 24F, FontStyle.Bold),
                    CornerRadius = 10,
                    BackColor = Color.FromArgb(100, 204, 240),
                    ForeColor = Color.White,
                    Margin = new Padding(5)
                };

                choice.FlatAppearance.BorderSize = 0;
                choice.Click += Choice_Click;
                choice.MouseEnter += (s, e) => ((Button)s).BackColor = Color.FromArgb(80, 184, 220);
                choice.MouseLeave += (s, e) => ((Button)s).BackColor = Color.FromArgb(100, 204, 240);

                pnlChoices.Controls.Add(choice);
            }
        }

        /// <summary>
        /// Khi click một nút chữ cái (choice) sẽ tìm ô trống đầu tiên và gán chữ cái vào đó, ẩn nút tương ứng.
        /// </summary>
        private void Choice_Click(object sender, EventArgs e)
        {
            Button choice = sender as Button;
            choice.Visible = false;

            foreach (Label slot in pnlAnswer.Controls)
            {
                if (slot.Tag != null && slot.Tag.ToString() == "SPACER")
                {
                    continue;
                }

                if (string.IsNullOrEmpty(slot.Text))
                {
                    slot.Text = choice.Text;
                    slot.Tag = choice;
                    break;
                }
            }
        }

        /// <summary>
        /// Khi click một slot đã có chữ sẽ trả lại chữ về phần choices (hiện lại nút) và xóa nội dung slot.
        /// </summary>
        private void Answer_Click(object sender, EventArgs e)
        {
            Label slot = sender as Label;

            if (slot.Tag != null && slot.Tag.ToString() == "SPACER")
            {
                return;
            }

            if (!string.IsNullOrEmpty(slot.Text) && slot.Tag is Button)
            {
                (slot.Tag as Button).Visible = true;
                slot.Text = "";
                slot.Tag = null;
            }
        }

        /// <summary>
        /// Đặt lại trạng thái hiện tại của câu hỏi (hiện lại các nút chữ, xóa slot).
        /// </summary>
        private void ResetCurrentState()
        {
            var answerSlots = pnlAnswer.Controls.OfType<Label>()
                                                .Where(l => l.Tag == null || l.Tag.ToString() != "SPACER")
                                                .ToList();

            foreach (Label slot in answerSlots)
            {
                if (slot.Tag is Button btn)
                {
                    btn.Visible = true;
                }

                slot.Text = "";
                slot.Tag = null;
                slot.BackColor = Color.White;
                slot.ForeColor = Color.FromArgb(64, 64, 64);
            }

            pnlChoices.Enabled = true;
            pnlAnswer.Enabled = true;
            lblStatus.Visible = false;
        }

        /// <summary>
        /// Kiểm tra đáp án hiện tại; nếu đúng tăng điểm và hiển thị popup; nếu sai cho phép retry hoặc next.
        /// </summary>
        private async void CheckAnswer()
        {
            var answerSlots = pnlAnswer.Controls.OfType<Label>()
                                                .Where(l => l.Tag == null || l.Tag.ToString() != "SPACER")
                                                .ToList();

            string userAnswer = string.Concat(answerSlots.Select(l => l.Text));
            string correctAnswer = _items[_currentItemIndex].Answer.Replace(" ", "");

            if (userAnswer.Length == correctAnswer.Length)
            {
                pnlChoices.Enabled = false;
                pnlAnswer.Enabled = false;

                bool isCorrect = (userAnswer == correctAnswer);

                if (isCorrect)
                {
                    _correctAnswers++;
                    lblProgress.Text = $" Câu {_currentItemIndex + 1}/{_items.Count} | Đúng: {_correctAnswers}";

                    foreach (Label slot in answerSlots)
                    {
                        slot.BackColor = Color.FromArgb(139, 195, 74); // xanh lá
                        slot.ForeColor = Color.White;
                    }

                    AnswerPopupResult res;
                    if (_currentItemIndex < _items.Count - 1)
                    {
                        res = ShowAnswerPopup(true, "Tuyệt vời! Bạn giỏi quá!", "Tiếp theo", showRetry: false);
                    }
                    else
                    {
                        res = ShowAnswerPopup(true, "Tuyệt vời! Bạn giỏi quá!", "Xem Kết quả", showRetry: false);
                    }

                    if (res == AnswerPopupResult.Next)
                    {
                        if (_currentItemIndex < _items.Count - 1)
                        {
                            _currentItemIndex++;
                            LoadCurrentItem();
                        }
                        else
                        {
                            EndGame();
                        }
                    }
                }
                else
                {
                    foreach (Label slot in answerSlots)
                    {
                        slot.BackColor = Color.FromArgb(255, 118, 117); // đỏ
                        slot.ForeColor = Color.White;
                    }

                    AnswerPopupResult res;
                    if (_currentItemIndex < _items.Count - 1)
                    {
                        res = ShowAnswerPopup(false, "Tiếc quá, chưa đúng!", "Tiếp theo", showRetry: true);
                    }
                    else
                    {
                        res = ShowAnswerPopup(false, "Tiếc quá, chưa đúng!", "Xem Kết quả", showRetry: true);
                    }

                    if (res == AnswerPopupResult.Retry)
                    {
                        ResetCurrentState();
                        return;
                    }
                    else if (res == AnswerPopupResult.Next)
                    {
                        if (_currentItemIndex < _items.Count - 1)
                        {
                            _currentItemIndex++;
                            LoadCurrentItem();
                        }
                        else
                        {
                            EndGame();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Xử lý khi nhấn nút Kiểm tra (validate đủ chữ rồi gọi CheckAnswer).
        /// </summary>
        private void BtnCheck_Click(object sender, EventArgs e)
        {
            var answerSlots = pnlAnswer.Controls.OfType<Label>()
                                                .Where(l => l.Tag == null || l.Tag.ToString() != "SPACER")
                                                .ToList();

            if (answerSlots.Any(l => string.IsNullOrEmpty(l.Text)))
            {
                lblStatus.Text = "Hãy điền đủ chữ cái trước khi kiểm tra!";
                lblStatus.ForeColor = Color.FromArgb(244, 179, 80);
                lblStatus.Visible = true;
                return;
            }

            lblStatus.Visible = false;
            CheckAnswer();
        }

        /// <summary>
        /// Xử lý khi nhấn nút Gợi ý: chọn một từ chưa đầy, chọn ngẫu nhiên một ô trống trong từ đó,
        /// đặt ký tự đúng và ẩn nút chữ tương ứng.
        /// </summary>
        private void BtnHint_Click(object sender, EventArgs e)
        {
            WordScrambleItem currentItem = _items[_currentItemIndex];
            string[] words = currentItem.Answer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string correctAnswerNoSpaces = currentItem.Answer.Replace(" ", "");

            // Lấy tất cả các slot (không phải spacer)
            var allSlots = pnlAnswer.Controls.OfType<Label>()
                                             .Where(l => l.Tag == null || l.Tag.ToString() != "SPACER")
                                             .ToList();

            // Gom nhóm slots theo từng từ
            List<List<Label>> wordGroups = new List<List<Label>>();
            int currentStartIndex = 0;

            foreach (string word in words)
            {
                var slots = allSlots.Skip(currentStartIndex).Take(word.Length).ToList();
                wordGroups.Add(slots);
                currentStartIndex += word.Length;
            }

            if (!wordGroups.Any())
            {
                return;
            }

            // Tìm từ mục tiêu dựa trên _nextWordHintIndex (vòng quay nếu từ đã đầy)
            List<Label> targetWordSlots = null;
            int wordsCount = wordGroups.Count;
            int startingIndex = _nextWordHintIndex;
            int wordsChecked = 0;

            while (wordsChecked < wordsCount)
            {
                int indexToHint = startingIndex % wordsCount;
                var currentGroup = wordGroups[indexToHint];

                if (currentGroup.Any(l => string.IsNullOrEmpty(l.Text)))
                {
                    targetWordSlots = currentGroup;
                    _nextWordHintIndex = (indexToHint + 1) % wordsCount;
                    break;
                }

                startingIndex++;
                wordsChecked++;
            }

            if (targetWordSlots == null)
            {
                lblStatus.Text = "Đáp án đã được điền hết!";
                lblStatus.ForeColor = Color.Red;
                lblStatus.Visible = true;
                return;
            }

            // Lấy danh sách ô trống trong từ mục tiêu
            List<Label> emptySlotsInTargetWord = targetWordSlots.Where(l => string.IsNullOrEmpty(l.Text)).ToList();

            if (emptySlotsInTargetWord.Count == 0)
            {
                return;
            }

            // Chọn ngẫu nhiên một ô trống để gợi ý
            var random = new Random();
            Label targetSlot = emptySlotsInTargetWord[random.Next(emptySlotsInTargetWord.Count)];

            // Tìm ký tự đúng dựa trên vị trí global của slot
            int globalIndex = allSlots.IndexOf(targetSlot);
            char correctChar = correctAnswerNoSpaces[globalIndex];

            // Tìm nút chữ tương ứng và ẩn nó, gán ký tự vào slot
            RoundedButton choiceButton = pnlChoices.Controls.OfType<RoundedButton>()
                .FirstOrDefault(b => b.Text.Equals(correctChar.ToString(), StringComparison.OrdinalIgnoreCase) && b.Visible);

            if (choiceButton != null)
            {
                choiceButton.Visible = false;
                targetSlot.Text = correctChar.ToString();
                targetSlot.Tag = choiceButton;
                lblStatus.Visible = false;
            }
            else
            {
                lblStatus.Text = "Không tìm thấy chữ cái để gợi ý.";
                lblStatus.ForeColor = Color.Red;
                lblStatus.Visible = true;
            }
        }

        /// <summary>
        /// Kết thúc game và hiển thị kết quả cuối.
        /// </summary>
        private void EndGame()
        {
            ShowFinalResultDialog(_correctAnswers, _items.Count);
        }

        #endregion
    }
}