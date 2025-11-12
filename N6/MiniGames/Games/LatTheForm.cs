using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Form trò chơi "Lật thẻ" (Memory Match).
    /// </summary>
    public class LatTheForm : GameFormWithMusic
    {
        #region Fields

        private List<string> _items;
        private List<Panel> _cards = new List<Panel>();
        private Panel _firstCard;
        private Panel _secondCard;
        private Timer _timer;
        private int _matchesFound;

        #endregion

        #region Constructor & Initialization

        /// <summary>
        /// Khởi tạo form LatThe với danh sách các mục (mỗi mục sẽ xuất hiện 2 lần để ghép cặp).
        /// </summary>
        /// <param name="items">Danh sách chuỗi làm giá trị của thẻ.</param>
        public LatTheForm(List<string> items)
        {
            if (items == null || items.Count < 2)
            {
                CloseWithWarning("Cần ít nhất 2 mục để chơi.");
                return;
            }

            _items = items;
            InitializeComponent();
            CreateCards();
        }

        /// <summary>
        /// Thiết lập các thuộc tính cơ bản của form và timer.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "Lật thẻ trí nhớ";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgColor;

            _timer = new Timer
            {
                Interval = 800
            };

            // Khi timeout => lật về trạng thái ban đầu cho 2 thẻ không trùng
            _timer.Tick += (s, e) =>
            {
                _timer.Stop();
                FlipBack(_firstCard);
                FlipBack(_secondCard);
                _firstCard = _secondCard = null;
            };
        }

        #endregion

        #region UI Construction

        /// <summary>
        /// Tạo các thẻ (Panel) dựa trên danh sách giá trị, nhân đôi và xáo trộn.
        /// </summary>
        private void CreateCards()
        {
            // Nhân đôi danh sách để có cặp
            var cardValues = _items.Concat(_items).ToList();

            // Xáo trộn; lưu ý vẫn giữ logic gốc (mỗi lần new Random trong OrderBy)
            var shuffledValues = cardValues.OrderBy(x => new Random().Next()).ToList();

            TableLayoutPanel tlp = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            int cols = 4;
            int rows = (int)Math.Ceiling(shuffledValues.Count / (double)cols);

            tlp.ColumnCount = cols;
            tlp.RowCount = rows;

            for (int i = 0; i < cols; i++)
            {
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / cols));
            }

            for (int i = 0; i < rows; i++)
            {
                tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rows));
            }

            foreach (var value in shuffledValues)
            {
                var card = new Panel
                {
                    BackColor = PrimaryColor,
                    Margin = new Padding(10),
                    Dock = DockStyle.Fill,
                    Cursor = Cursors.Hand
                };

                // Bo góc thông qua region (giữ nguyên gọi CreateRoundRectRgn)
                card.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, card.Width, card.Height, 20, 20));

                var lbl = new Label
                {
                    Text = "★",
                    Tag = value,
                    Dock = DockStyle.Fill,
                    Font = GameFont(36),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.White,
                    Cursor = Cursors.Hand
                };

                card.Controls.Add(lbl);

                // Cả panel và label đều bắt click để tiện UX
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

        #endregion

        #region Game Logic (Flip / Match)

        /// <summary>
        /// Lật lên (hiển thị giá trị) cho 1 thẻ.
        /// </summary>
        /// <param name="card">Panel đại diện cho thẻ.</param>
        private void FlipUp(Panel card)
        {
            var lbl = card.Controls[0] as Label;
            card.BackColor = Color.White;
            lbl.Text = lbl.Tag.ToString();
            lbl.Font = GameFont(18);
            lbl.ForeColor = TextColor;
        }

        /// <summary>
        /// Lật lại về trạng thái úp (ký hiệu ★) nếu thẻ chưa được ghép đúng.
        /// </summary>
        /// <param name="card">Panel đại diện cho thẻ.</param>
        private void FlipBack(Panel card)
        {
            if (card == null || card.BackColor == CorrectColor)
            {
                // Nếu thẻ null hoặc đã đánh dấu đúng (CorrectColor) thì không lật lại
                return;
            }

            var lbl = card.Controls[0] as Label;
            card.BackColor = PrimaryColor;
            lbl.Text = "★";
            lbl.Font = GameFont(36);
            lbl.ForeColor = Color.White;
        }

        #endregion

        #region UI Events

        /// <summary>
        /// Xử lý khi người chơi click vào một thẻ (Panel hoặc Label).
        /// </summary>
        /// <param name="sender">Panel hoặc Label được click.</param>
        /// <param name="e">Event arguments.</param>
        private void Card_Click(object sender, EventArgs e)
        {
            // Sender có thể là Label hoặc Panel; chuẩn hóa lấy Panel
            Panel card = sender as Panel;

            if (card == null)
            {
                // Nếu sender là Label thì lấy parent
                Label lbl = sender as Label;
                if (lbl != null)
                {
                    card = lbl.Parent as Panel;
                }
            }

            if (card == null)
            {
                return;
            }

            // Nếu thẻ đã lật hoặc đang chờ 2nd card thì bỏ qua
            if (card.BackColor != PrimaryColor || _secondCard != null)
            {
                return;
            }

            FlipUp(card);

            if (_firstCard == null)
            {
                _firstCard = card;
                return;
            }

            _secondCard = card;

            // So sánh Tag (giá trị) của 2 thẻ
            if ((_firstCard.Controls[0] as Label).Tag.ToString() == (_secondCard.Controls[0] as Label).Tag.ToString())
            {
                _matchesFound++;

                // Đánh dấu 2 thẻ đã ghép đúng
                foreach (Panel c in new List<Panel> { _firstCard, _secondCard })
                {
                    c.BackColor = CorrectColor;
                    c.Enabled = false;
                }

                _firstCard = _secondCard = null;

                // Nếu đã tìm hết => thông báo chiến thắng
                if (_matchesFound == _items.Count)
                {
                    MessageBox.Show("🎉 Chúc mừng! Bạn đã tìm thấy tất cả các cặp! 🎉", "Chiến thắng!");
                    this.Close();
                }
            }
            else
            {
                // Không trùng => bắt đầu timer để lật lại sau một khoảng
                _timer.Start();
            }
        }

        #endregion
    }
}