using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public class LuckyWheelForm : GameFormWithMusic
    {
        private List<string> _names;
        private Label lblResult;
        private RoundedButton btnSpin;
        private Timer spinTimer;
        private Random random = new Random();
        private int spinTicks, totalTicks;

        public LuckyWheelForm(List<string> names)
        {
            if (names == null || names.Count == 0) { CloseWithWarning(); return; }
            _names = names;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "🎡 Vòng Quay May Mắn";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(41, 52, 98);

            lblResult = new Label { Text = "Sẵn sàng?", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = GameFont(72, FontStyle.Italic), ForeColor = Color.White };
            btnSpin = new RoundedButton { Text = "QUAY", Size = new Size(200, 200), Font = GameFont(28), BackColor = SecondaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, CornerRadius = 100 };
            btnSpin.FlatAppearance.BorderSize = 0;
            btnSpin.Location = new Point((this.ClientSize.Width - btnSpin.Width) / 2, this.ClientSize.Height - 250);
            btnSpin.Click += BtnSpin_Click;

            spinTimer = new Timer { Interval = 10 };
            spinTimer.Tick += SpinTimer_Tick;

            this.Controls.Add(lblResult);
            this.Controls.Add(btnSpin);
        }

        private void BtnSpin_Click(object sender, EventArgs e)
        {
            btnSpin.Enabled = false;
            btnSpin.BackColor = Color.Gray;
            btnSpin.Text = "...";
            spinTicks = 0;
            totalTicks = random.Next(50, 80);
            spinTimer.Interval = 10;
            spinTimer.Start();
        }

        private void SpinTimer_Tick(object sender, EventArgs e)
        {
            spinTicks++;
            lblResult.Text = _names[random.Next(_names.Count)];
            if (spinTicks > totalTicks * 0.7) spinTimer.Interval = (int)(spinTimer.Interval * 1.15);

            if (spinTicks > totalTicks)
            {
                spinTimer.Stop();
                string winner = _names[random.Next(_names.Count)];
                lblResult.Text = winner;
                lblResult.Font = GameFont(80);
                lblResult.ForeColor = Color.FromArgb(252, 221, 98);
                MessageBox.Show($"🎉 Chúc mừng: {winner} 🎉", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetUI();
            }
        }

        private void ResetUI()
        {
            btnSpin.Enabled = true;
            btnSpin.BackColor = SecondaryColor;
            btnSpin.Text = "QUAY";
            lblResult.ForeColor = Color.White;
            lblResult.Font = GameFont(72, FontStyle.Italic);
            lblResult.Text = "Tiếp tục?";
        }
    }
}
