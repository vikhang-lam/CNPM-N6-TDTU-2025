using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Form trò chơi "Chuyền Bóng Nóng" — sau một khoảng thời gian ngẫu nhiên sẽ dừng và hiển thị câu hỏi.
    /// </summary>
    public class PassBallForm : GameFormWithMusic
    {
        #region Fields

        private List<QuizQuestion> _questions;
        private Timer _gameTimer;
        private int _timeLeft;
        private Random _random = new Random();
        private Label lblTimer;
        private PictureBox picBall;
        private RoundedButton btnStart;
        private SoundPlayer _tickPlayer;
        private SoundPlayer _buzzerPlayer;

        #endregion

        #region Constructor & Lifecycle

        /// <summary>
        /// Khởi tạo PassBallForm với danh sách câu hỏi (dùng chung với Quiz).
        /// </summary>
        /// <param name="questions">Danh sách QuizQuestion.</param>
        public PassBallForm(List<QuizQuestion> questions)
        {
            if (questions == null || questions.Count == 0)
            {
                CloseWithWarning("Cần có câu hỏi từ game Quiz để chơi.");
                return;
            }

            _questions = questions;
            InitializeComponent();
            SetupSounds();
        }

        /// <summary>
        /// Dọn dẹp tài nguyên âm thanh khi form đóng.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _tickPlayer?.Dispose();
            _buzzerPlayer?.Dispose();
        }

        #endregion

        #region Initialization Helpers

        /// <summary>
        /// Tạo SoundPlayer từ resources nếu có (không ném lỗi nếu không tìm thấy).
        /// </summary>
        private void SetupSounds()
        {
            try
            {
                _tickPlayer = new SoundPlayer(Properties.Resources.tick_sound);
            }
            catch
            {
                _tickPlayer = null;
            }

            try
            {
                _buzzerPlayer = new SoundPlayer(Properties.Resources.buzzer_sound);
            }
            catch
            {
                _buzzerPlayer = null;
            }
        }

        /// <summary>
        /// Thiết lập UI controls cho trò chơi.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "⚽ Chuyền Bóng Nóng";
            this.Size = new Size(800, 600);
            this.BackColor = BgColor;
            this.StartPosition = FormStartPosition.CenterScreen;

            lblTimer = new Label
            {
                Font = GameFont(120),
                Text = "00",
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            picBall = new PictureBox
            {
                Size = new Size(150, 150),
                Location = new Point(325, 50),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            try
            {
                picBall.Image = Properties.Resources.pass_ball;
            }
            catch
            {
                // fallback: không cần ảnh nếu không có resource
            }

            btnStart = new RoundedButton
            {
                Text = "BẮT ĐẦU",
                Size = new Size(250, 70),
                Location = new Point(275, 450),
                Font = GameFont(24),
                BackColor = SecondaryColor,
                ForeColor = Color.White,
                CornerRadius = 35
            };

            btnStart.Click += Start_Click;

            _gameTimer = new Timer
            {
                Interval = 1000
            };
            _gameTimer.Tick += GameTimer_Tick;

            this.Controls.AddRange(new Control[] { picBall, lblTimer, btnStart });
        }

        #endregion

        #region Game Events

        /// <summary>
        /// Bắt đầu game: chọn thời gian ngẫu nhiên, bật tick sound (nếu có) và start timer.
        /// </summary>
        private void Start_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStart.Text = "ĐANG CHẠY...";
            btnStart.BackColor = Color.Gray;

            // Thời gian ngẫu nhiên giữa 15-30 giây
            _timeLeft = _random.Next(15, 31);
            lblTimer.Text = _timeLeft.ToString("00");

            // Bật âm thanh tick lặp nếu có
            try
            {
                _tickPlayer?.PlayLooping();
            }
            catch
            {
                // ignore sound errors
            }

            _gameTimer.Start();
        }

        /// <summary>
        /// Tick mỗi giây: cập nhật hiển thị đếm ngược, dừng khi bằng 0 và hiển thị câu hỏi.
        /// </summary>
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            _timeLeft--;
            lblTimer.Text = _timeLeft.ToString("00");

            if (_timeLeft <= 0)
            {
                _gameTimer.Stop();

                try
                {
                    _tickPlayer?.Stop();
                }
                catch
                {
                    // ignore
                }

                try
                {
                    _buzzerPlayer?.Play();
                }
                catch
                {
                    // ignore
                }

                ShowQuestion();
            }
        }

        #endregion

        #region Gameplay Helpers

        /// <summary>
        /// Chọn một câu hỏi ngẫu nhiên và hiển thị, sau đó reset trạng thái game.
        /// </summary>
        private void ShowQuestion()
        {
            QuizQuestion randomQ = _questions[_random.Next(_questions.Count)];

            string questionText = $"{randomQ.QuestionText}\n\nA. {randomQ.Options[0]}\nB. {randomQ.Options[1]}\nC. {randomQ.Options[2]}\nD. {randomQ.Options[3]}";
            MessageBox.Show(questionText, "Câu hỏi!", MessageBoxButtons.OK);

            MessageBox.Show($"Đáp án đúng là: {randomQ.CorrectAnswer}", "Đáp án");

            ResetGame();
        }

        /// <summary>
        /// Khôi phục UI về trạng thái ban đầu sau khi show question.
        /// </summary>
        private void ResetGame()
        {
            lblTimer.Text = "00";
            btnStart.Enabled = true;
            btnStart.Text = "BẮT ĐẦU";
            btnStart.BackColor = SecondaryColor;
        }

        #endregion
    }
}