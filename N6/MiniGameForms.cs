using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N6
{
    // Lớp chứa các hàm và thuộc tính dùng chung cho các game
    public abstract class BaseGameForm : Form
    {
        public Font GameFont(float size, FontStyle style = FontStyle.Bold)
        {
            return new Font("Lexend", size, style, GraphicsUnit.Point, ((byte)(0)));
        }

        public readonly Color BgColor = Color.FromArgb(240, 247, 255);
        public readonly Color PrimaryColor = Color.FromArgb(87, 187, 247);
        public readonly Color CorrectColor = Color.FromArgb(26, 176, 134);
        public readonly Color IncorrectColor = Color.FromArgb(255, 118, 117);
        public readonly Color TextColor = Color.FromArgb(64, 64, 64);
    }

    #region Data Structures for Games
    public class QuizQuestion
    {
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
    }

    public class FlashcardItem
    {
        public string Term { get; set; } // Mặt trước
        public string Definition { get; set; } // Mặt sau
    }
    #endregion

    // ========== QUIZ NHANH (MNG01) ==========
    public class QuizGameForm : BaseGameForm
    {
        private List<QuizQuestion> _questions;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private Label lblQuestion, lblScore, lblQuestionCount;
        private List<RoundedButton> optionButtons;

        public QuizGameForm(List<QuizQuestion> questions)
        {
            _questions = questions.Where(q => q != null && !string.IsNullOrWhiteSpace(q.QuestionText)).ToList();
            InitializeComponent();
            LoadQuestion();
        }

        private void InitializeComponent()
        {
            this.Text = "📝 Quiz Nhanh";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = BgColor;

            lblQuestionCount = new Label { Dock = DockStyle.Top, Height = 40, ForeColor = TextColor, Font = GameFont(12), Padding = new Padding(20, 10, 0, 0) };
            lblQuestion = new Label { Dock = DockStyle.Fill, Text = "Câu hỏi", ForeColor = TextColor, Font = GameFont(22), TextAlign = ContentAlignment.MiddleCenter, Padding = new Padding(50) };
            lblScore = new Label { Dock = DockStyle.Bottom, Height = 50, Text = "Điểm: 0", ForeColor = PrimaryColor, Font = GameFont(14), TextAlign = ContentAlignment.MiddleCenter };

            Panel answerPanel = new Panel { Dock = DockStyle.Bottom, Height = 180, Padding = new Padding(50, 20, 50, 20) };

            optionButtons = new List<RoundedButton>();
            string[] prefixes = { "A", "B", "C", "D" };
            for (int i = 0; i < 4; i++)
            {
                var btn = new RoundedButton
                {
                    Text = prefixes[i],
                    Font = GameFont(14),
                    Tag = prefixes[i],
                    Size = new Size(150, 60),
                    CornerRadius = 25,
                    BackColor = Color.White,
                    ForeColor = TextColor,
                    FlatStyle = FlatStyle.Flat,
                };
                btn.FlatAppearance.BorderSize = 0;
                optionButtons.Add(btn);
            }

            TableLayoutPanel tlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            tlp.Controls.Add(optionButtons[0], 0, 0);
            tlp.Controls.Add(optionButtons[1], 1, 0);
            tlp.Controls.Add(optionButtons[2], 0, 1);
            tlp.Controls.Add(optionButtons[3], 1, 1);

            answerPanel.Controls.Add(tlp);

            this.Controls.Add(lblQuestion);
            this.Controls.Add(answerPanel);
            this.Controls.Add(lblScore);
            this.Controls.Add(lblQuestionCount);
        }

        private void LoadQuestion()
        {
            if (currentQuestionIndex < _questions.Count)
            {
                lblQuestionCount.Text = $"Câu {currentQuestionIndex + 1} / {_questions.Count}";
                QuizQuestion q = _questions[currentQuestionIndex];
                lblQuestion.Text = q.QuestionText;
                for (int i = 0; i < 4; i++)
                {
                    optionButtons[i].Text = q.Options[i];
                    optionButtons[i].Click -= OptionButton_Click; // Remove old handler
                    optionButtons[i].Click += OptionButton_Click; // Add new handler
                    optionButtons[i].BackColor = Color.White;
                    optionButtons[i].Enabled = true;
                }
            }
            else EndGame();
        }

        private async void OptionButton_Click(object sender, EventArgs e)
        {
            RoundedButton clickedButton = sender as RoundedButton;
            QuizQuestion q = _questions[currentQuestionIndex];

            optionButtons.ForEach(btn => btn.Enabled = false);

            if (clickedButton.Tag.ToString() == q.CorrectAnswer.ToUpper())
            {
                score++;
                lblScore.Text = $"Điểm: {score}";
                clickedButton.BackColor = CorrectColor;
            }
            else
            {
                clickedButton.BackColor = IncorrectColor;
                var correctButton = optionButtons.First(btn => btn.Tag.ToString() == q.CorrectAnswer.ToUpper());
                correctButton.BackColor = CorrectColor;
            }

            await Task.Delay(2000);
            currentQuestionIndex++;
            LoadQuestion();
        }

        private void EndGame()
        {
            lblQuestion.Font = GameFont(28);
            lblQuestion.Text = $"Hoàn thành!\nĐiểm cuối cùng của bạn là: {score}/{_questions.Count}";
            this.Controls.OfType<Panel>().First().Visible = false;
        }
    }

    // ========== GỌI TÊN NGẪU NHIÊN (MNG02) ==========
    public class LuckyWheelForm : BaseGameForm
    {
        private List<string> _names;
        private Label lblResult;
        private RoundedButton btnSpin;
        private Timer spinTimer;
        private Random random = new Random();
        private int spinTicks = 0;
        private int totalTicks = 50;

        public LuckyWheelForm(List<string> names)
        {
            _names = names.Where(n => !string.IsNullOrWhiteSpace(n)).ToList();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "🎡 Gọi tên ngẫu nhiên";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(41, 52, 98);

            lblResult = new Label { Text = "Sẵn sàng?", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = GameFont(60), ForeColor = Color.White };
            btnSpin = new RoundedButton { Text = "QUAY", Dock = DockStyle.Bottom, Height = 80, Font = GameFont(20), BackColor = Color.FromArgb(255, 118, 117), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, CornerRadius = 20, Margin = new Padding(100) };
            btnSpin.FlatAppearance.BorderSize = 0;

            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 120, Padding = new Padding(200, 20, 200, 20) };
            pnlBottom.Controls.Add(btnSpin);

            spinTimer = new Timer { Interval = 10 };
            spinTimer.Tick += SpinTimer_Tick;
            btnSpin.Click += BtnSpin_Click;

            this.Controls.Add(lblResult);
            this.Controls.Add(pnlBottom);
        }

        private void BtnSpin_Click(object sender, EventArgs e)
        {
            if (_names == null || _names.Count == 0)
            {
                lblResult.Text = "Danh sách rỗng!";
                return;
            }
            btnSpin.Enabled = false;
            btnSpin.BackColor = Color.Gray;
            spinTicks = 0;
            totalTicks = random.Next(40, 60); // Randomize duration
            spinTimer.Interval = 10;
            spinTimer.Start();
        }

        private void SpinTimer_Tick(object sender, EventArgs e)
        {
            spinTicks++;
            lblResult.Text = _names[random.Next(_names.Count)];

            // Hiệu ứng chậm dần
            if (spinTicks > totalTicks * 0.6) spinTimer.Interval = (int)(spinTimer.Interval * 1.1);

            if (spinTicks > totalTicks)
            {
                spinTimer.Stop();
                btnSpin.Enabled = true;
                btnSpin.BackColor = Color.FromArgb(255, 118, 117);

                string winner = _names[random.Next(_names.Count)];
                lblResult.Text = winner;
                lblResult.ForeColor = Color.FromArgb(252, 221, 98);

                MessageBox.Show($"🎉 Chúc mừng: {winner} 🎉", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblResult.ForeColor = Color.White;
            }
        }
    }

    // ========== FLASHCARD (MNG03) ==========
    public class FlashcardForm : BaseGameForm
    {
        private List<FlashcardItem> _cards;
        private int currentIndex = 0;
        private bool isFlipped = false;

        private Panel cardPanel;
        private Label cardLabel;
        private RoundedButton btnNext, btnPrev;
        private Label lblCardCount;

        public FlashcardForm(List<FlashcardItem> cards)
        {
            _cards = cards;
            InitializeComponent();
            ShowCard();
        }

        private void InitializeComponent()
        {
            this.Text = "📇 Flashcard";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = BgColor;

            lblCardCount = new Label { Dock = DockStyle.Top, Height = 40, ForeColor = TextColor, Font = GameFont(12), Padding = new Padding(20, 10, 0, 0), TextAlign = ContentAlignment.MiddleLeft };

            cardPanel = new Panel { BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(100), Cursor = Cursors.Hand };
            cardPanel.Size = new Size(500, 300);
            cardPanel.Location = new Point((this.ClientSize.Width - cardPanel.Width) / 2, (this.ClientSize.Height - cardPanel.Height) / 2);
            cardPanel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, cardPanel.Width, cardPanel.Height, 20, 20));
            cardPanel.Click += Card_Click;

            cardLabel = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = GameFont(24), ForeColor = TextColor, Padding = new Padding(20) };
            cardPanel.Controls.Add(cardLabel);

            btnPrev = new RoundedButton { Text = "< Trước", Size = new Size(120, 50), Font = GameFont(12), CornerRadius = 20, BackColor = PrimaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(40, this.ClientSize.Height - 80) };
            btnNext = new RoundedButton { Text = "Tiếp >", Size = new Size(120, 50), Font = GameFont(12), CornerRadius = 20, BackColor = PrimaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(this.ClientSize.Width - 160, this.ClientSize.Height - 80) };

            btnPrev.FlatAppearance.BorderSize = 0;
            btnNext.FlatAppearance.BorderSize = 0;
            btnPrev.Click += (s, e) => { if (currentIndex > 0) { currentIndex--; ShowCard(); } };
            btnNext.Click += (s, e) => { if (currentIndex < _cards.Count - 1) { currentIndex++; ShowCard(); } };

            this.Controls.Add(cardPanel);
            this.Controls.Add(btnPrev);
            this.Controls.Add(btnNext);
            this.Controls.Add(lblCardCount);
        }

        private void Card_Click(object sender, EventArgs e)
        {
            isFlipped = !isFlipped;
            ShowCardContent();
        }

        private void ShowCard()
        {
            isFlipped = false;
            ShowCardContent();
            lblCardCount.Text = $"Thẻ {currentIndex + 1} / {_cards.Count}";
            btnPrev.Enabled = currentIndex > 0;
            btnNext.Enabled = currentIndex < _cards.Count - 1;
        }

        private void ShowCardContent()
        {
            if (_cards.Count == 0)
            {
                cardLabel.Text = "Không có thẻ nào.";
                return;
            }
            cardLabel.Text = isFlipped ? _cards[currentIndex].Definition : _cards[currentIndex].Term;
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
    }

    // ========== LẬT THẺ (MNG08) ==========
    public class LatTheForm : BaseGameForm
    {
        private List<string> _items;
        private List<Panel> _cards = new List<Panel>();
        private Panel _firstCard = null, _secondCard = null;
        private Timer _timer;
        private int _matchesFound = 0;

        public LatTheForm(List<string> items)
        {
            _items = items;
            InitializeComponent();
            CreateCards();
        }

        private void InitializeComponent()
        {
            this.Text = "🃏 Lật thẻ trí nhớ";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = BgColor;

            _timer = new Timer { Interval = 750 };
            _timer.Tick += (s, e) =>
            {
                _timer.Stop();
                _firstCard.BackColor = PrimaryColor;
                _secondCard.BackColor = PrimaryColor;
                (_firstCard.Controls[0] as Label).Text = "❓";
                (_secondCard.Controls[0] as Label).Text = "❓";
                _firstCard = null;
                _secondCard = null;
            };
        }

        private void CreateCards()
        {
            if (_items.Count == 0)
            {
                var lbl = new Label { Text = "Không có dữ liệu để tạo thẻ!", Dock = DockStyle.Fill, Font = GameFont(20), TextAlign = ContentAlignment.MiddleCenter };
                this.Controls.Add(lbl);
                return;
            }

            var cardValues = _items.Concat(_items).ToList();
            var random = new Random();
            var shuffledValues = cardValues.OrderBy(x => random.Next()).ToList();

            TableLayoutPanel tlp = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            int cols = 4;
            int rows = (int)Math.Ceiling(shuffledValues.Count / (double)cols);

            tlp.ColumnCount = cols;
            tlp.RowCount = rows;
            for (int i = 0; i < cols; i++) tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / cols));
            for (int i = 0; i < rows; i++) tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rows));

            foreach (var value in shuffledValues)
            {
                var card = new Panel
                {
                    BackColor = PrimaryColor,
                    Margin = new Padding(10),
                    Dock = DockStyle.Fill,
                    Cursor = Cursors.Hand,
                    Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, 150, 150, 20, 20)),
                };
                var lbl = new Label
                {
                    Text = "❓",
                    Tag = value, // Giấu giá trị thật trong Tag
                    Dock = DockStyle.Fill,
                    Font = GameFont(24),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.White
                };
                card.Controls.Add(lbl);
                card.Click += Card_Click;
                lbl.Click += Card_Click;
                _cards.Add(card);
            }

            for (int i = 0; i < _cards.Count; i++)
            {
                tlp.Controls.Add(_cards[i], i % cols, i / cols);
            }
            this.Controls.Add(tlp);
        }

        private async void Card_Click(object sender, EventArgs e)
        {
            var panel = (sender is Label) ? (sender as Label).Parent as Panel : sender as Panel;
            if (panel == null || panel.BackColor == CorrectColor || _secondCard != null) return;

            var lbl = panel.Controls[0] as Label;
            panel.BackColor = Color.White;
            lbl.Text = lbl.Tag.ToString();

            if (_firstCard == null)
            {
                _firstCard = panel;
                return;
            }

            _secondCard = panel;

            if ((_firstCard.Controls[0] as Label).Tag.ToString() == (_secondCard.Controls[0] as Label).Tag.ToString())
            {
                await Task.Delay(200); // Wait for UI to update
                _firstCard.BackColor = CorrectColor;
                _secondCard.BackColor = CorrectColor;
                _firstCard = null;
                _secondCard = null;
                _matchesFound++;
                if (_matchesFound == _items.Count)
                {
                    MessageBox.Show("🎉 Chúc mừng! Bạn đã tìm thấy tất cả các cặp! 🎉", "Chiến thắng!");
                    this.Close();
                }
            }
            else
            {
                _timer.Start();
            }
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
    }

    // ========== RANDOM SỐ (MNG09) ==========
    public class RandomSoForm : BaseGameForm
    {
        private List<int> _numbers;
        private Label lblResult;
        private RoundedButton btnSpin;
        private Timer spinTimer;
        private Random random = new Random();
        private int spinTicks = 0;

        public RandomSoForm(List<string> numberStrings)
        {
            _numbers = numberStrings.Select(s => { int.TryParse(s, out int n); return n; }).Where(n => n != 0).ToList();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "🎲 Random Số";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(26, 176, 134);

            lblResult = new Label { Text = "00", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = GameFont(120), ForeColor = Color.White };
            btnSpin = new RoundedButton { Text = "QUAY SỐ", Dock = DockStyle.Bottom, Height = 80, Font = GameFont(20), BackColor = Color.White, ForeColor = TextColor, FlatStyle = FlatStyle.Flat, CornerRadius = 20 };
            btnSpin.FlatAppearance.BorderSize = 0;

            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 120, Padding = new Padding(200, 20, 200, 20) };
            pnlBottom.Controls.Add(btnSpin);

            spinTimer = new Timer { Interval = 25 };
            spinTimer.Tick += SpinTimer_Tick;
            btnSpin.Click += BtnSpin_Click;

            this.Controls.Add(lblResult);
            this.Controls.Add(pnlBottom);
        }

        private void BtnSpin_Click(object sender, EventArgs e)
        {
            if (_numbers == null || _numbers.Count < 2)
            {
                lblResult.Text = "Lỗi!";
                MessageBox.Show("Cần ít nhất 2 số (min và max) trong danh sách.", "Dữ liệu không hợp lệ");
                return;
            }
            btnSpin.Enabled = false;
            spinTicks = 0;
            spinTimer.Start();
        }

        private void SpinTimer_Tick(object sender, EventArgs e)
        {
            spinTicks++;
            int min = _numbers.Min();
            int max = _numbers.Max();
            lblResult.Text = random.Next(min, max + 1).ToString("00");

            if (spinTicks > 30)
            {
                spinTimer.Stop();
                btnSpin.Enabled = true;

                int finalNumber = random.Next(min, max + 1);
                lblResult.Text = finalNumber.ToString("00");
                lblResult.ForeColor = Color.FromArgb(252, 221, 98);
                MessageBox.Show($"Số may mắn là: {finalNumber}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblResult.ForeColor = Color.White;
            }
        }
    }

    // ========== PLACEHOLDER FORMS ==========
    public class GheChuForm : Form { public GheChuForm() { this.Text = "🧩 Ghép chữ (Chưa hiện thực)"; } }
    public class NgheChonHinhForm : Form { public NgheChonHinhForm() { this.Text = "🔊 Nghe chọn hình (Chưa hiện thực)"; } }
    public class SapXepCauForm : Form { public SapXepCauForm() { this.Text = "🔄 Sắp xếp câu (Chưa hiện thực)"; } }
    public class DienTuForm : Form { public DienTuForm() { this.Text = "✍️ Điền từ (Chưa hiện thực)"; } }
    public class PassBallForm : Form { public PassBallForm() { this.Text = "⚽ Pass a Ball (Chưa hiện thực)"; } }
}