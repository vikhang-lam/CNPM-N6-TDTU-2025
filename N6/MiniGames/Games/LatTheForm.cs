using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    public class LatTheForm : GameFormWithMusic
    {
        private List<string> _items;
        private List<Panel> _cards = new List<Panel>();
        private Panel _firstCard, _secondCard;
        private Timer _timer;
        private int _matchesFound;

        public LatTheForm(List<string> items)
        {
            if (items == null || items.Count < 2) { CloseWithWarning("Cần ít nhất 2 mục để chơi."); return; }
            _items = items;
            InitializeComponent();
            CreateCards();
        }

        private void InitializeComponent()
        {
            this.Text = "🃏 Lật thẻ trí nhớ"; this.Size = new Size(800, 600); this.StartPosition = FormStartPosition.CenterScreen; this.BackColor = BgColor;
            _timer = new Timer { Interval = 800 };
            _timer.Tick += (s, e) => { _timer.Stop(); FlipBack(_firstCard); FlipBack(_secondCard); _firstCard = _secondCard = null; };
        }

        private void CreateCards()
        {
            var cardValues = _items.Concat(_items).ToList();
            var shuffledValues = cardValues.OrderBy(x => new Random().Next()).ToList();
            TableLayoutPanel tlp = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            int cols = 4;
            int rows = (int)Math.Ceiling(shuffledValues.Count / (double)cols);

            tlp.ColumnCount = cols; tlp.RowCount = rows;
            for (int i = 0; i < cols; i++) tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / cols));
            for (int i = 0; i < rows; i++) tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rows));

            foreach (var value in shuffledValues)
            {
                var card = new Panel { BackColor = PrimaryColor, Margin = new Padding(10), Dock = DockStyle.Fill, Cursor = Cursors.Hand };
                card.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, card.Width, card.Height, 20, 20));
                var lbl = new Label { Text = "★", Tag = value, Dock = DockStyle.Fill, Font = GameFont(36), TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.White, Cursor = Cursors.Hand };
                card.Controls.Add(lbl);
                card.Click += Card_Click; lbl.Click += Card_Click;
                _cards.Add(card);
            }
            for (int i = 0; i < _cards.Count; i++) tlp.Controls.Add(_cards[i], i % cols, i / cols);
            this.Controls.Add(tlp);
        }

        private void FlipUp(Panel card)
        {
            var lbl = card.Controls[0] as Label;
            card.BackColor = Color.White; lbl.Text = lbl.Tag.ToString();
            lbl.Font = GameFont(18); lbl.ForeColor = TextColor;
        }

        private void FlipBack(Panel card)
        {
            if (card == null || card.BackColor == CorrectColor) return;
            var lbl = card.Controls[0] as Label;
            card.BackColor = PrimaryColor; lbl.Text = "★";
            lbl.Font = GameFont(36); lbl.ForeColor = Color.White;
        }

        private void Card_Click(object sender, EventArgs e)
        {
            Panel card = sender as Panel;
            if (card == null || card.BackColor != PrimaryColor || _secondCard != null) return;
            FlipUp(card);
            if (_firstCard == null) { _firstCard = card; return; }
            _secondCard = card;
            if ((_firstCard.Controls[0] as Label).Tag.ToString() == (_secondCard.Controls[0] as Label).Tag.ToString())
            {
                _matchesFound++;
                foreach (Panel c in new List<Panel> { _firstCard, _secondCard }) { c.BackColor = CorrectColor; c.Enabled = false; }
                _firstCard = _secondCard = null;
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
    }
}
