// File: UC_MiniGames.cs
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq; // Thêm

namespace N6
{
    public partial class UC_MiniGames : UserControl
    {
        // --- Constants ---
        private const int CARD_WIDTH = 200;
        private const int CARD_HEIGHT = 220;
        private const int CARD_CORNER_RADIUS = 20;

        // CHUẨN HÓA: Tên trường (field) readonly tuân thủ camelCase
        private readonly Font _gameNameFont = new Font("Lexend", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
        private readonly Color _gameNameColor = Color.FromArgb(64, 64, 64);
        private readonly Color _controlBackgroundColor = Color.FromArgb(240, 247, 255);

        public UC_MiniGames()
        {
            InitializeComponent();
            this.BackColor = _controlBackgroundColor;
            LoadGames();
        }

        /// <summary>
        /// Tải danh sách các game từ CSDL và tạo các thẻ (card) tương ứng.
        /// </summary>
        private void LoadGames()
        {
            // Suspend layout để tránh giật lag
            panelGames.SuspendLayout();

            // CHUẨN HÓA: Dọn dẹp control cũ và gỡ sự kiện trước khi tải lại
            DisposeGameCards();
            panelGames.Controls.Clear();
            panelGames.BackColor = this.BackColor;

            DataTable dt = DatabaseHelper.GetMiniGames();
            if (dt == null || dt.Rows.Count == 0)
            {
                Label noGamesLabel = new Label
                {
                    Text = "Không tìm thấy trò chơi nào.",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = _gameNameFont
                };
                panelGames.Controls.Add(noGamesLabel);
                panelGames.ResumeLayout(true);
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                string maMNG = row["MaMNG"].ToString();
                string tenMNG = row["Ten"].ToString();

                Panel gameCard = CreateGameCard(maMNG, tenMNG);
                panelGames.Controls.Add(gameCard);
            }

            panelGames.ResumeLayout(true);
        }

        /// <summary>
        /// Tạo một Panel (thẻ) đã được định dạng cho một game.
        /// </summary>
        private Panel CreateGameCard(string maMNG, string tenMNG)
        {
            Panel card = new Panel
            {
                Width = CARD_WIDTH,
                Height = CARD_HEIGHT,
                Margin = new Padding(20),
                BackColor = Color.White,
                Tag = maMNG
            };
            card.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, card.Width, card.Height, CARD_CORNER_RADIUS, CARD_CORNER_RADIUS));

            PictureBox pic = new PictureBox
            {
                Size = new Size(120, 120),
                Location = new Point((card.Width - 120) / 2, 15),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            try
            {
                pic.Image = (Image)Properties.Resources.ResourceManager.GetObject(maMNG);
            }
            catch
            {
                // (Bỏ qua nếu không có ảnh)
            }

            Label name = new Label
            {
                Text = tenMNG,
                Dock = DockStyle.Bottom,
                Height = 70,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = _gameNameFont,
                ForeColor = _gameNameColor,
                Padding = new Padding(10, 0, 10, 10),
                Cursor = Cursors.Hand
            };

            card.Controls.Add(pic);
            card.Controls.Add(name);

            // Gán sự kiện
            Action<object, EventArgs> clickAction = (sender, e) => OnGameCardClicked(maMNG, tenMNG);
            card.Click += new EventHandler(clickAction);
            pic.Click += new EventHandler(clickAction);
            name.Click += new EventHandler(clickAction);

            return card;
        }

        /// <summary>
        /// Xử lý sự kiện khi click vào một thẻ game.
        /// </summary>
        private void OnGameCardClicked(string maMNG, string tenMNG)
        {
            if (string.IsNullOrEmpty(maMNG)) return;

            using (GameDataInputForm dataInputForm = new GameDataInputForm(maMNG, tenMNG))
            {
                dataInputForm.ShowDialog();
            }
        }

        // P/Invoke để bo góc
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        /// <summary>
        /// CHUẨN HÓA: Dọn dẹp các sự kiện và control động.
        /// </summary>
        private void DisposeGameCards()
        {
            if (panelGames == null) return;

            // Dùng ToList() để tạo bản sao collection trước khi sửa đổi nó
            foreach (var card in panelGames.Controls.OfType<Panel>().ToList())
            {
                // Gỡ bỏ sự kiện cho chính thẻ (card)
                // (Không thể gỡ EventHandler động (lambda) một cách trực tiếp,
                // nhưng việc Dispose control sẽ tự động làm điều đó)

                // Gỡ sự kiện cho các control con
                foreach (var child in card.Controls.OfType<Control>().ToList())
                {
                    // (Tương tự, sự kiện lambda sẽ được dọn khi control bị dispose)
                    child.Dispose();
                }

                // Hủy control
                panelGames.Controls.Remove(card);
                card.Dispose();
            }
        }

        /// <summary> 
        /// Dọn dẹp tài nguyên.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dọn dẹp các thẻ game động
                DisposeGameCards();

                // Dọn dẹp các font đã tạo
                _gameNameFont?.Dispose();

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}