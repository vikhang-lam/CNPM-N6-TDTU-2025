// File: UC_MiniGames.cs
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    // Lớp tùy chỉnh cho các nút bấm bo tròn (giữ nguyên)
    public class RoundedButton : Button
    {
        private int cornerRadius = 15;
        public int CornerRadius
        {
            get { return cornerRadius; }
            set { cornerRadius = value; Invalidate(); }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, cornerRadius * 2, cornerRadius * 2, 180, 90);
            path.AddArc(this.Width - cornerRadius * 2, 0, cornerRadius * 2, cornerRadius * 2, 270, 90);
            path.AddArc(this.Width - cornerRadius * 2, this.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            path.AddArc(0, this.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            path.CloseAllFigures();
            this.Region = new Region(path);
        }
    }

    public partial class UC_MiniGames : UserControl
    {
        public UC_MiniGames()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(240, 247, 255);
            LoadGames();
        }

        private void LoadGames()
        {
            panelGames.Controls.Clear();
            panelGames.BackColor = this.BackColor;

            DataTable dt = DatabaseHelper.GetMiniGames();
            if (dt == null || dt.Rows.Count == 0) return;

            foreach (DataRow row in dt.Rows)
            {
                string maMNG = row["MaMNG"].ToString();
                string tenMNG = row["Ten"].ToString();

                // Tạo card cho mỗi game, nền trắng
                Panel card = new Panel
                {
                    Width = 200,
                    Height = 220,
                    Margin = new Padding(20),
                    BackColor = Color.White, // Nền chính của thẻ là màu trắng
                    Tag = maMNG,
                };
                card.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, card.Width, card.Height, 20, 20));

                // PictureBox để hiển thị hình ảnh
                PictureBox pic = new PictureBox
                {
                    Width = 120,  // Tăng kích thước ảnh cho đẹp hơn
                    Height = 120,
                    Location = new Point((card.Width - 120) / 2, 15), // Đặt vị trí ảnh
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Tag = maMNG,
                    BackColor = Color.Transparent, // Quan trọng: Nền trong suốt để thấy nền trắng của card
                };

                try
                {
                    pic.Image = (Image)Properties.Resources.ResourceManager.GetObject(maMNG);
                }
                catch
                {
                    // Xử lý nếu không có ảnh
                }

                // Tên game
                Label name = new Label
                {
                    Text = tenMNG,
                    Dock = DockStyle.Bottom,
                    Height = 70, // Dành không gian còn lại cho tên
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Lexend", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                    ForeColor = Color.FromArgb(64, 64, 64),
                    Tag = maMNG,
                    Padding = new Padding(10, 0, 10, 10) // Padding cho tên game
                };

                // *** THAY ĐỔI CHÍNH: Thêm ảnh và tên trực tiếp vào card, không qua panel header ***
                card.Controls.Add(pic);
                card.Controls.Add(name);
                panelGames.Controls.Add(card);

                // Gán sự kiện click cho tất cả các control liên quan
                card.Click += Game_Click;
                pic.Click += Game_Click;
                name.Click += Game_Click;
            }
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private void Game_Click(object sender, EventArgs e)
        {
            Control clickedControl = sender as Control;
            string maMNG = null;
            string tenMNG = "Game";

            while (clickedControl != null)
            {
                if (clickedControl.Tag != null && !string.IsNullOrEmpty(clickedControl.Tag.ToString()))
                {
                    maMNG = clickedControl.Tag.ToString();
                    Panel card = clickedControl as Panel ?? clickedControl.Parent as Panel;
                    if (card != null)
                    {
                        var lbl = card.Controls.OfType<Label>().FirstOrDefault();
                        if (lbl != null) tenMNG = lbl.Text;
                    }
                    break;
                }
                clickedControl = clickedControl.Parent;
            }

            if (string.IsNullOrEmpty(maMNG)) return;

            GameDataInputForm dataInputForm = new GameDataInputForm(maMNG, tenMNG);
            dataInputForm.ShowDialog();
        }
    }
}