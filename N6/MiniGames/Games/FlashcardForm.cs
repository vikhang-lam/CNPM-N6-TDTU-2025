using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace N6
{
    public class FlashcardForm : GameFormWithMusic
    {
        private List<FlashcardItem> _cards;
        private int currentIndex = 0;
        private bool isFlipped = false;
        private Panel cardPanel, cardShadowPanel;
        private Label cardLabel, lblCardCount, lblHint, lblFrontIndicator, lblBackIndicator;
        private RoundedButton btnNext, btnPrev, btnFlip;
        private Panel pnlHeader;
        private Panel pnlCardCountContainer;

        public FlashcardForm(List<FlashcardItem> cards)
        {
            if (cards == null || cards.Count == 0) { CloseWithWarning(); return; }
            _cards = cards;
            InitializeComponent();
            ShowCard();
        }

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

        private void InitializeComponent()
        {
            this.Text = "📇 Flashcard";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // TẢI ẢNH NỀN
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

            Label lblTitle = new Label
            {
                Text = "Flashcard",
                Font = new Font("Lexend", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(141, 94, 61),
                Location = new Point(450, 60),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            lblHint = new Label
            {
                Text = "💡 Nhấn vào thẻ hoặc nút 'Lật thẻ' để xem mặt sau",
                Font = new Font("Segoe UI", 12F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 350) / 2, 130)
            };


            cardPanel = new Panel
            {
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Size = new Size(580, 330),
                Location = new Point(210, 150)

            };
            int cornerRadius = 20;
            cardPanel.Region = new Region(GetRoundedRectPath(new Rectangle(0, 0, cardPanel.Width, cardPanel.Height), cornerRadius));

            cardPanel.Click += (s, e) => FlipCard();

            cardPanel.Paint += (s, e) =>
            {
                Rectangle rect = cardPanel.ClientRectangle;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (Pen borderPen = new Pen(isFlipped ? Color.FromArgb(40, 167, 69) : Color.FromArgb(87, 187, 247), 3))
                {
                    borderPen.DashStyle = DashStyle.Dot;
                    using (GraphicsPath path = GetRoundedRectPath(rect, 20))
                    {
                        e.Graphics.DrawPath(borderPen, path);
                    }
                }
            };

            lblFrontIndicator = new Label
            {
                Text = "MẶT TRƯỚC",
                Font = new Font("Lexend", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(87, 187, 247),
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            lblBackIndicator = new Label
            {
                Text = "MẶT SAU",
                Font = new Font("Lexend", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent,
                Visible = false
            };
            cardLabel = new Label
            {
                Location = new Point(30, 80),
                Size = new Size(520, 200),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Lexend", 32F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            cardLabel.Click += (s, e) => FlipCard();

            cardPanel.Controls.AddRange(new Control[] { lblFrontIndicator, lblBackIndicator, cardLabel });

            int controlY = 510;
            int navButtonWidth = 120;
            int navButtonHeight = 40;
            int navButtonRadius = navButtonHeight / 2;
            int center_x = (this.ClientSize.Width / 2);

            btnPrev = new RoundedButton
            {
                Text = "◀ Trước",
                Size = new Size(navButtonWidth, navButtonHeight),
                Font = new Font("Lexend", 12F, FontStyle.Bold),
                CornerRadius = navButtonRadius,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(center_x - navButtonWidth - 100, controlY),
                Cursor = Cursors.Hand
            };
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.Click += (s, e) => Navigate(-1);

            btnPrev.MouseEnter += (s, e) => btnPrev.BackColor = ControlPaint.Light(Color.White, 0.1f);
            btnPrev.MouseLeave += (s, e) => btnPrev.BackColor = btnPrev.Enabled ? Color.White : Color.LightGray;

            pnlCardCountContainer = new Panel
            {
                Location = new Point(center_x - 75, controlY),
                Size = new Size(150, navButtonHeight),
                BackColor = Color.FromArgb(255, 245, 220),
                Cursor = Cursors.Default
            };

            pnlCardCountContainer.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pnlCardCountContainer.Width, pnlCardCountContainer.Height, navButtonRadius * 2, navButtonRadius * 2));

            pnlCardCountContainer.Paint += (s, e) =>
            {
                Rectangle rect = pnlCardCountContainer.ClientRectangle;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (GraphicsPath path = GetRoundedRectPath(rect, navButtonRadius))
                {
                    using (Pen pen = new Pen(Color.FromArgb(255, 189, 89), 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            lblCardCount = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Lexend", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(141, 94, 61),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            pnlCardCountContainer.Controls.Add(lblCardCount);

            btnNext = new RoundedButton
            {
                Text = "Sau ▶",
                Size = new Size(navButtonWidth, navButtonHeight),
                Font = new Font("Lexend", 12F, FontStyle.Bold),
                CornerRadius = navButtonRadius,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(center_x + 100, controlY),
                Cursor = Cursors.Hand
            };
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += (s, e) => Navigate(1);

            btnNext.MouseEnter += (s, e) => btnNext.BackColor = ControlPaint.Light(Color.White, 0.1f);
            btnNext.MouseLeave += (s, e) => btnNext.BackColor = btnNext.Enabled ? Color.White : Color.LightGray;

            btnFlip = new RoundedButton
            {
                Text = "🔄 Lật thẻ",
                Size = new Size(200, 60),
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                CornerRadius = 15,
                BackColor = Color.FromArgb(29, 209, 161),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point((this.ClientSize.Width - 200) / 2, 580),
                Cursor = Cursors.Hand
            };
            btnFlip.FlatAppearance.BorderSize = 0;
            btnFlip.Click += (s, e) => FlipCard();

            btnFlip.MouseEnter += (s, e) => btnFlip.BackColor = ControlPaint.Light(btnFlip.BackColor, 0.1f);
            btnFlip.MouseLeave += (s, e) => btnFlip.BackColor = Color.FromArgb(29, 209, 161);

            this.Controls.AddRange(new Control[] { lblTitle, lblHint, cardPanel, btnPrev, pnlCardCountContainer, btnNext, btnFlip });
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse
        );

        private void FlipCard()
        {
            isFlipped = !isFlipped;
            ShowCardContent();
        }

        private void Navigate(int direction)
        {
            int newIndex = currentIndex + direction;
            if (newIndex >= 0 && newIndex < _cards.Count)
            {
                currentIndex = newIndex;
                ShowCard();
            }
        }

        private void ShowCard()
        {
            isFlipped = false;
            ShowCardContent();

            // Cập nhật số thẻ
            lblCardCount.Text = $" Thẻ {currentIndex + 1} / {_cards.Count}";

            // Enable/Disable navigation buttons
            btnPrev.Enabled = currentIndex > 0;
            btnPrev.BackColor = btnPrev.Enabled ? Color.FromArgb(87, 187, 247) : Color.LightGray;

            btnNext.Enabled = currentIndex < _cards.Count - 1;
            btnNext.BackColor = btnNext.Enabled ? Color.FromArgb(87, 187, 247) : Color.LightGray;
        }

        private void ShowCardContent()
        {
            cardLabel.Text = isFlipped ? _cards[currentIndex].Definition : _cards[currentIndex].Term;

            Color frontColor = Color.FromArgb(250, 250, 245);
            Color backColor = Color.FromArgb(235, 255, 245);
            cardPanel.BackColor = isFlipped ? backColor : frontColor;

            // Hiển thị indicator
            lblFrontIndicator.Visible = !isFlipped;
            lblBackIndicator.Visible = isFlipped;

            cardPanel.Invalidate();

            // Animation đơn giản
            cardLabel.Font = new Font("Lexend", 28F, FontStyle.Bold);
            System.Threading.Tasks.Task.Delay(100).ContinueWith(t =>
            {
                if (!this.IsDisposed && cardLabel != null && !cardLabel.IsDisposed)
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        if (!cardLabel.IsDisposed)
                            cardLabel.Font = new Font("Lexend", 32F, FontStyle.Bold);
                    }));
                }
            });
        }
    }
}
