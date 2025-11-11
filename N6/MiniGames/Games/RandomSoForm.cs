using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Form quay số ngẫu nhiên trong một khoảng (min..max).
    /// </summary>
    public class RandomSoForm : GameFormWithMusic
    {
        #region Fields

        private List<int> _numbers;
        private Label lblResult;
        private RoundedButton btnSpin;
        private Timer spinTimer;
        private Random random = new Random();
        private int spinTicks;

        #endregion

        #region Constructor & Initialization

        /// <summary>
        /// Khởi tạo RandomSoForm với danh sách số (thường lấy từ input strings).
        /// </summary>
        /// <param name="numberStrings">Danh sách chuỗi có chứa min/max hoặc các số. Sẽ parse sang int và loại bỏ 0.</param>
        public RandomSoForm(List<string> numberStrings)
        {
            if (numberStrings == null || numberStrings.Count < 2)
            {
                CloseWithWarning("Cần ít nhất 2 số (min và max).");
                return;
            }

            // Parse sang int; lưu ý giữ nguyên logic: trả về 0 khi parse fail => lọc ra
            _numbers = numberStrings.Select(s => { int.TryParse(s, out int n); return n; })
                                     .Where(n => n != 0)
                                     .ToList();

            InitializeComponent();
        }

        /// <summary>
        /// Thiết lập UI cho form quay số.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "Quay Số Ngẫu Nhiên";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(26, 176, 134);

            lblResult = new Label
            {
                Text = "00",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = GameFont(150),
                ForeColor = Color.White
            };

            btnSpin = new RoundedButton
            {
                Text = "QUAY SỐ",
                Dock = DockStyle.Bottom,
                Height = 80,
                Font = GameFont(20),
                BackColor = Color.White,
                ForeColor = TextColor,
                FlatStyle = FlatStyle.Flat,
                CornerRadius = 35
            };

            btnSpin.FlatAppearance.BorderSize = 0;
            btnSpin.Location = new Point(275, 450);
            btnSpin.Click += BtnSpin_Click;

            spinTimer = new Timer
            {
                Interval = 25
            };
            spinTimer.Tick += SpinTimer_Tick;

            this.Controls.Add(lblResult);
            this.Controls.Add(btnSpin);
        }

        #endregion

        #region UI Events

        /// <summary>
        /// Bắt đầu quay số: kiểm tra dữ liệu, vô hiệu nút, bắt đầu timer hiển thị số ngẫu nhiên.
        /// </summary>
        private void BtnSpin_Click(object sender, EventArgs e)
        {
            if (_numbers == null || _numbers.Count < 2)
            {
                MessageBox.Show("Cần ít nhất 2 số (min và max) trong danh sách.", "Dữ liệu không hợp lệ");
                return;
            }

            btnSpin.Enabled = false;
            btnSpin.Text = "ĐANG QUAY...";
            btnSpin.BackColor = Color.Gray;

            // Lấy min/max từ danh sách
            int min = _numbers.Min();
            int max = _numbers.Max();

            lblResult.Text = "00";
            spinTicks = 0;
            spinTimer.Start();
        }

        /// <summary>
        /// Tick của timer: hiển thị số ngẫu nhiên nhanh, dừng sau một số tick và hiển thị kết quả cuối.
        /// </summary>
        private void SpinTimer_Tick(object sender, EventArgs e)
        {
            spinTicks++;

            int min = _numbers.Min();
            int max = _numbers.Max();

            lblResult.Text = random.Next(min, max + 1).ToString("00");

            // Khi đạt ngưỡng, dừng tiến trình "quay"
            if (spinTicks > 40)
            {
                spinTimer.Stop();
                btnSpin.Enabled = true;

                int finalNumber = random.Next(min, max + 1);
                lblResult.Text = finalNumber.ToString("00");
                lblResult.ForeColor = SecondaryColor;

                MessageBox.Show($"Số may mắn là: {finalNumber}", "Kết quả");

                // Khôi phục màu chữ
                lblResult.ForeColor = Color.White;
            }
        }

        #endregion
    }
}