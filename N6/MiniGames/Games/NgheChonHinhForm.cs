using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N6
{
    public class NgheChonHinhForm : GameFormWithMusic
    {
        private List<ListenChooseItem> _items;
        private int _currentItemIndex = 0;
        private RoundedButton btnPlaySound;
        private List<Panel> _choicePanels = new List<Panel>();
        private TableLayoutPanel tlp;

        public NgheChonHinhForm(List<ListenChooseItem> items)
        {
            if (items == null || items.Count == 0) { CloseWithWarning(); return; }
            _items = items;
            InitializeComponent();
            LoadCurrentItem();
        }

        private void InitializeComponent()
        {
            this.Text = "🔊 Nghe Âm Thanh - Chọn Hình Ảnh";
            this.Size = new Size(800, 600);
            this.BackColor = BgColor;
            this.StartPosition = FormStartPosition.CenterScreen;

            btnPlaySound = new RoundedButton { Text = "Nghe", Size = new Size(200, 200), Location = new Point(300, 30), Font = GameFont(36), BackColor = SecondaryColor, ForeColor = Color.White, CornerRadius = 100 };
            btnPlaySound.Click += (s, e) => PlaySound();

            tlp = new TableLayoutPanel { Dock = DockStyle.None, Size = new Size(700, 300), Location = new Point(50, 250), ColumnCount = 2, RowCount = 2 };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 50F)); tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            for (int i = 0; i < 4; i++)
            {
                Panel p = new Panel { Dock = DockStyle.Fill, Margin = new Padding(15), BackColor = Color.White, Cursor = Cursors.Hand };
                p.Paint += (s, e) => { using (var pen = new Pen(Color.Gainsboro, 5)) { e.Graphics.DrawRectangle(pen, 2, 2, p.Width - 4, p.Height - 4); } };
                PictureBox pic = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Transparent, Padding = new Padding(10) };
                p.Controls.Add(pic);
                p.Click += Choice_Click; pic.Click += Choice_Click;
                _choicePanels.Add(p);
                tlp.Controls.Add(p, i % 2, i / 2);
            }

            this.Controls.Add(btnPlaySound);
            this.Controls.Add(tlp);
        }

        private void LoadCurrentItem()
        {
            if (_currentItemIndex >= _items.Count) { EndGame(); return; }

            ListenChooseItem current = _items[_currentItemIndex];
            var randomChoices = current.Choices.OrderBy(c => new Random().Next()).ToList();

            for (int i = 0; i < 4; i++)
            {
                var panel = _choicePanels[i];
                var pic = panel.Controls[0] as PictureBox;
                panel.Tag = randomChoices[i]; pic.Tag = randomChoices[i];
                try { pic.Image = (Image)Properties.Resources.ResourceManager.GetObject(randomChoices[i].ImageResourceName); }
                catch { try { pic.Image = Properties.Resources.placeholder; } catch { } }
                panel.BackColor = Color.White;
            }
            tlp.Enabled = true;
            PlaySound();
        }

        private void PlaySound()
        {
            try
            {
                var soundResource = (Stream)Properties.Resources.ResourceManager.GetObject(_items[_currentItemIndex].SoundResourceName);
                if (soundResource != null)
                {
                    using (var player = new SoundPlayer(soundResource)) { player.Play(); }
                }
            }
            catch { /* Handle missing sound */ }
        }

        private async void Choice_Click(object sender, EventArgs e)
        {
            var control = sender as Control;
            var panel = (control is PictureBox) ? control.Parent as Panel : control as Panel;
            var choice = panel.Tag as ImageChoice;

            tlp.Enabled = false;

            if (choice.IsCorrect)
            {
                panel.BackColor = CorrectColor;
                await Task.Delay(1500);
                _currentItemIndex++;
                LoadCurrentItem();
            }
            else
            {
                panel.BackColor = IncorrectColor;
                var correctPanel = _choicePanels.First(p => (p.Tag as ImageChoice).IsCorrect);
                correctPanel.BackColor = CorrectColor;
                await Task.Delay(2500);
                LoadCurrentItem(); // Tải lại câu hỏi hiện tại để thử lại
            }
        }
        private void EndGame() { MessageBox.Show("Hoàn thành!"); this.Close(); }
    }
}
