// File: UC_MiniGames.cs
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_MiniGames : UserControl
    {
        // --- Constants for easy styling and maintenance ---
        private const int CARD_WIDTH = 200;
        private const int CARD_HEIGHT = 220;
        private const int CARD_CORNER_RADIUS = 20;
        private readonly Font GAME_NAME_FONT = new Font("Lexend", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
        private readonly Color GAME_NAME_COLOR = Color.FromArgb(64, 64, 64);
        private readonly Color CONTROL_BACKGROUND_COLOR = Color.FromArgb(240, 247, 255);

        public UC_MiniGames()
        {
            InitializeComponent();
            this.BackColor = CONTROL_BACKGROUND_COLOR;
            LoadGames();
        }

        private void LoadGames()
        {
            // Suspend layout to prevent flickering while adding controls
            panelGames.SuspendLayout();

            panelGames.Controls.Clear();
            panelGames.BackColor = this.BackColor;

            DataTable dt = DatabaseHelper.GetMiniGames();
            if (dt == null || dt.Rows.Count == 0)
            {
                // Handle case where no games are found
                Label noGamesLabel = new Label
                {
                    Text = "Không tìm thấy trò chơi nào.",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = GAME_NAME_FONT
                };
                panelGames.Controls.Add(noGamesLabel);
                panelGames.ResumeLayout(true); // Resume layout even if there's an error
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                string maMNG = row["MaMNG"].ToString();
                string tenMNG = row["Ten"].ToString();

                Panel gameCard = CreateGameCard(maMNG, tenMNG);
                panelGames.Controls.Add(gameCard);
            }

            // Resume layout and repaint the panel with all new controls
            panelGames.ResumeLayout(true);
        }

        /// <summary>
        /// Creates a styled Panel representing a single game card.
        /// </summary>
        private Panel CreateGameCard(string maMNG, string tenMNG)
        {
            // Create the main card panel
            Panel card = new Panel
            {
                Width = CARD_WIDTH,
                Height = CARD_HEIGHT,
                Margin = new Padding(20),
                BackColor = Color.White,
                Tag = maMNG // Keep the tag on the main card for reference if needed
            };
            card.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, card.Width, card.Height, CARD_CORNER_RADIUS, CARD_CORNER_RADIUS));

            // PictureBox for the game image
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
                // Try to load the specific game image
                pic.Image = (Image)Properties.Resources.ResourceManager.GetObject(maMNG);
            }
            catch
            {
                // If the image is not found, use a default placeholder image
                // Ensure you have a 'placeholder' image in your Properties.Resources
                
            }

            // Label for the game name
            Label name = new Label
            {
                Text = tenMNG,
                Dock = DockStyle.Bottom,
                Height = 70,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = GAME_NAME_FONT,
                ForeColor = GAME_NAME_COLOR,
                Padding = new Padding(10, 0, 10, 10),
                Cursor = Cursors.Hand
            };

            // Add controls to the card
            card.Controls.Add(pic);
            card.Controls.Add(name);

            // --- Simplified Event Handling ---
            // All controls on the card trigger the same simple, direct action.
            // We use a lambda expression to pass the game info directly.
            Action<object, EventArgs> clickAction = (sender, e) => OnGameCardClicked(maMNG, tenMNG);
            card.Click += new EventHandler(clickAction);
            pic.Click += new EventHandler(clickAction);
            name.Click += new EventHandler(clickAction);

            return card;
        }

        /// <summary>
        /// Handles the click event for any part of a game card.
        /// </summary>
        private void OnGameCardClicked(string maMNG, string tenMNG)
        {
            // The logic is now extremely simple, no need to find the sender's tag.
            if (string.IsNullOrEmpty(maMNG)) return;

            using (GameDataInputForm dataInputForm = new GameDataInputForm(maMNG, tenMNG))
            {
                dataInputForm.ShowDialog();
            }
        }

        // P/Invoke for creating rounded regions
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
    }
}