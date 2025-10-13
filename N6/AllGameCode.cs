using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N6
{
    #region Lớp tiện ích và Lớp cơ sở
    // ===================================================================
    // LỚP MỚI: CUNG CẤP CHỨC NĂNG PLACEHOLDER CHO TEXTBOX
    // ===================================================================
    public static class PlaceholderProvider
    {
        public static void SetPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.ForeColor = Color.Gray;

            textBox.Enter += (sender, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }
    }

    // ===================================================================
    // LỚP QUẢN LÝ NHẠC NỀN
    // ===================================================================
    public static class MusicPlayer
    {
        private static SoundPlayer _player;

        public static void Play(UnmanagedMemoryStream musicResource)
        {
            if (musicResource == null) return;
            Stop();
            _player = new SoundPlayer(musicResource);
            _player.PlayLooping();
        }

        public static void Stop()
        {
            if (_player != null)
            {
                _player.Stop();
                _player.Dispose();
                _player = null;
            }
        }
    }

    // ===================================================================
    // LỚP QUẢN LÝ DỮ LIỆU GAME (KẾT NỐI DATABASEHELPER)
    // ===================================================================
    public static class GameDataManager
    {
        private static string GetRawData(string maMNG) { try { return DatabaseHelper.GetGameData(maMNG); } catch { return ""; } }
        private static void SaveRawData(string maMNG, string rawData) { try { DatabaseHelper.SaveGameData(maMNG, rawData); } catch { } }

        public static List<QuizQuestion> GetQuizQuestions(string maMNG)
        {
            var data = new List<QuizQuestion>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return data;
            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 6) { data.Add(new QuizQuestion { QuestionText = parts[0], Options = new List<string> { parts[1], parts[2], parts[3], parts[4] }, CorrectAnswer = parts[5].Trim().ToUpper() }); }
            }
            return data;
        }
        public static void SaveQuizQuestions(string maMNG, List<QuizQuestion> questions) { SaveRawData(maMNG, string.Join("|", questions.Select(q => $"{q.QuestionText.Trim()};{string.Join(";", q.Options.Select(o => o.Trim()))};{q.CorrectAnswer.Trim().ToUpper()}"))); }
        public static List<string> GetListFromString(string maMNG)
        {
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return new List<string>();
            return rawData.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
        }
        public static void SaveListToString(string maMNG, List<string> items) { SaveRawData(maMNG, string.Join(";", items.Select(s => s.Trim()))); }
        public static List<FlashcardItem> GetFlashcardItems(string maMNG)
        {
            var data = new List<FlashcardItem>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return data;
            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 2) { data.Add(new FlashcardItem { Term = parts[0].Trim(), Definition = parts[1].Trim() }); }
            }
            return data;
        }
        public static void SaveFlashcardItems(string maMNG, List<FlashcardItem> items) { SaveRawData(maMNG, string.Join("|", items.Select(item => $"{item.Term.Trim()};{item.Definition.Trim()}"))); }
        public static List<WordScrambleItem> GetWordScrambleItems(string maMNG)
        {
            var data = new List<WordScrambleItem>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return data;
            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 3) { data.Add(new WordScrambleItem { ImageHintResourceName = parts[0], Question = parts[1], Answer = parts[2].ToUpper() }); }
            }
            return data;
        }
        public static void SaveWordScrambleItems(string maMNG, List<WordScrambleItem> items) { SaveRawData(maMNG, string.Join("|", items.Select(item => $"{item.ImageHintResourceName};{item.Question};{item.Answer}"))); }
        public static SentenceScrambleItem GetSentenceScrambleItem(string maMNG) { return new SentenceScrambleItem { CorrectSentence = GetRawData(maMNG) ?? "" }; }
        public static void SaveSentenceScrambleItem(string maMNG, SentenceScrambleItem item) { SaveRawData(maMNG, item.CorrectSentence); }
        public static FillBlankQuestion GetFillBlankQuestion(string maMNG)
        {
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return new FillBlankQuestion { QuestionText = "", Answer = "" };
            var parts = rawData.Split(';');
            return new FillBlankQuestion { QuestionText = parts.Length > 0 ? parts[0] : "", Answer = parts.Length > 1 ? parts[1] : "" };
        }
        public static void SaveFillBlankQuestion(string maMNG, FillBlankQuestion item) { SaveRawData(maMNG, $"{item.QuestionText};{item.Answer}"); }
        public static List<ListenChooseItem> GetListenChooseItems(string maMNG)
        {
            var data = new List<ListenChooseItem>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return data;
            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 5)
                {
                    var choices = new List<ImageChoice> { new ImageChoice { ImageResourceName = parts[1], IsCorrect = true }, new ImageChoice { ImageResourceName = parts[2], IsCorrect = false }, new ImageChoice { ImageResourceName = parts[3], IsCorrect = false }, new ImageChoice { ImageResourceName = parts[4], IsCorrect = false } };
                    data.Add(new ListenChooseItem { SoundResourceName = parts[0], Choices = choices });
                }
            }
            return data;
        }
        public static void SaveListenChooseItems(string maMNG, List<ListenChooseItem> items) { SaveRawData(maMNG, string.Join("|", items.Select(item => $"{item.SoundResourceName};{item.Choices.First(c => c.IsCorrect).ImageResourceName};{string.Join(";", item.Choices.Where(c => !c.IsCorrect).Select(c => c.ImageResourceName))}"))); }
    }

    // ===================================================================
    // CÁC LỚP FORM CƠ SỞ VÀ CONTROL PHỤ
    // ===================================================================
    public abstract class BaseGameForm : Form
    {
        public Font GameFont(float size, FontStyle style = FontStyle.Bold) { return new Font("Lexend", size, style, GraphicsUnit.Point, ((byte)(0))); }
        public readonly Color BgColor = Color.FromArgb(240, 247, 255);
        public readonly Color PrimaryColor = Color.FromArgb(87, 187, 247);
        public readonly Color SecondaryColor = Color.FromArgb(255, 189, 89);
        public readonly Color CorrectColor = Color.FromArgb(29, 209, 161);
        public readonly Color IncorrectColor = Color.FromArgb(255, 118, 117);
        public readonly Color TextColor = Color.FromArgb(64, 64, 64);
        public readonly Color MutedTextColor = Color.FromArgb(150, 150, 150);

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);

        protected void CloseWithWarning(string message = "Không có dữ liệu để bắt đầu game.")
        {
            this.Load += (s, e) => {
                MessageBox.Show(message, "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.BeginInvoke(new MethodInvoker(this.Close));
            };
        }
    }

    public abstract class GameFormWithMusic : BaseGameForm
    {
        public GameFormWithMusic()
        {
            try { this.Load += (s, e) => MusicPlayer.Play(Properties.Resources.GameMusic); }
            catch { }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            MusicPlayer.Stop();
        }
    }

    public class RoundedButton : Button
    {
        private int cornerRadius = 25;
        public int CornerRadius { get => cornerRadius; set { cornerRadius = value; Invalidate(); } }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            GraphicsPath grPath = new GraphicsPath();
            if (cornerRadius > 0 && this.Width > 0 && this.Height > 0)
            {
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                int rad = cornerRadius; // Sửa lại để trực quan hơn, không cần nhân 2
                if (rad * 2 > rect.Width) rad = rect.Width / 2;
                if (rad * 2 > rect.Height) rad = rect.Height / 2;

                grPath.AddArc(rect.X, rect.Y, rad * 2, rad * 2, 180, 90);
                grPath.AddArc(rect.Right - (rad * 2), rect.Y, rad * 2, rad * 2, 270, 90);
                grPath.AddArc(rect.Right - (rad * 2), rect.Bottom - (rad * 2), rad * 2, rad * 2, 0, 90);
                grPath.AddArc(rect.X, rect.Bottom - (rad * 2), rad * 2, rad * 2, 90, 90);
                grPath.CloseFigure();
                this.Region = new Region(grPath);
            }
            base.OnPaint(pevent);
        }
    }
    #endregion

    #region Cấu trúc dữ liệu cho game
    public class QuizQuestion { public string QuestionText { get; set; } public List<string> Options { get; set; } public string CorrectAnswer { get; set; } }
    public class FlashcardItem { public string Term { get; set; } public string Definition { get; set; } }
    public class WordScrambleItem { public string ImageHintResourceName { get; set; } public string Question { get; set; } public string Answer { get; set; } }
    public class ImageChoice { public string ImageResourceName { get; set; } public bool IsCorrect { get; set; } }
    public class ListenChooseItem { public string SoundResourceName { get; set; } public List<ImageChoice> Choices { get; set; } }
    public class SentenceScrambleItem { public string CorrectSentence { get; set; } }
    public class FillBlankQuestion { public string QuestionText { get; set; } public string Answer { get; set; } }
    #endregion

    #region User Controls cho Form Nhập Liệu
    public class QuizQuestionControl : UserControl
    {
        public TextBox TxtQuestion { get; private set; }
        public TextBox TxtOptionA { get; private set; }
        public TextBox TxtOptionB { get; private set; }
        public TextBox TxtOptionC { get; private set; }
        public TextBox TxtOptionD { get; private set; }
        public ComboBox CboCorrectAnswer { get; private set; }
        public Button BtnRemove { get; private set; }

        public QuizQuestionControl()
        {
            this.Size = new Size(650, 150); this.BackColor = Color.White; this.Padding = new Padding(10); this.Margin = new Padding(5); this.BorderStyle = BorderStyle.FixedSingle;

            TxtQuestion = new TextBox { Location = new Point(15, 15), Size = new Size(620, 30), Font = new Font("Lexend", 12F, FontStyle.Bold) };
            PlaceholderProvider.SetPlaceholder(TxtQuestion, "Nhập câu hỏi tại đây...");

            TxtOptionA = new TextBox { Location = new Point(15, 55), Size = new Size(300, 28), Font = new Font("Lexend", 10F) };
            PlaceholderProvider.SetPlaceholder(TxtOptionA, "Đáp án A");

            TxtOptionB = new TextBox { Location = new Point(335, 55), Size = new Size(300, 28), Font = new Font("Lexend", 10F) };
            PlaceholderProvider.SetPlaceholder(TxtOptionB, "Đáp án B");

            TxtOptionC = new TextBox { Location = new Point(15, 95), Size = new Size(300, 28), Font = new Font("Lexend", 10F) };
            PlaceholderProvider.SetPlaceholder(TxtOptionC, "Đáp án C");

            TxtOptionD = new TextBox { Location = new Point(335, 95), Size = new Size(300, 28), Font = new Font("Lexend", 10F) };
            PlaceholderProvider.SetPlaceholder(TxtOptionD, "Đáp án D");

            CboCorrectAnswer = new ComboBox { Location = new Point(535, 125), Size = new Size(100, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Lexend", 10F) };
            CboCorrectAnswer.Items.AddRange(new object[] { "A", "B", "C", "D" }); CboCorrectAnswer.SelectedIndex = 0;
            BtnRemove = new Button { Text = "Xóa", Location = new Point(455, 125), BackColor = Color.LightCoral, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            this.Controls.AddRange(new Control[] { TxtQuestion, TxtOptionA, TxtOptionB, TxtOptionC, TxtOptionD, CboCorrectAnswer, BtnRemove });
        }
        public QuizQuestion GetData() { return new QuizQuestion { QuestionText = TxtQuestion.Text, Options = new List<string> { TxtOptionA.Text, TxtOptionB.Text, TxtOptionC.Text, TxtOptionD.Text }, CorrectAnswer = CboCorrectAnswer.SelectedItem.ToString() }; }
        public void SetData(QuizQuestion q) { TxtQuestion.Text = q.QuestionText; TxtQuestion.ForeColor = Color.Black; TxtOptionA.Text = q.Options[0]; TxtOptionA.ForeColor = Color.Black; TxtOptionB.Text = q.Options[1]; TxtOptionB.ForeColor = Color.Black; TxtOptionC.Text = q.Options[2]; TxtOptionC.ForeColor = Color.Black; TxtOptionD.Text = q.Options[3]; TxtOptionD.ForeColor = Color.Black; CboCorrectAnswer.SelectedItem = q.CorrectAnswer; }
    }

    public class WordScrambleControl : UserControl
    {
        public TextBox TxtImageName { get; private set; }
        public TextBox TxtQuestion { get; private set; }
        public TextBox TxtAnswer { get; private set; }
        public Button BtnRemove { get; private set; }

        public WordScrambleControl()
        {
            this.Size = new Size(650, 80); this.BackColor = Color.White; this.Padding = new Padding(10); this.Margin = new Padding(5); this.BorderStyle = BorderStyle.FixedSingle;

            TxtImageName = new TextBox { Location = new Point(15, 15), Size = new Size(200, 28), Font = new Font("Lexend", 10F) };
            PlaceholderProvider.SetPlaceholder(TxtImageName, "Tên ảnh gợi ý (vd: apple.png)");

            TxtQuestion = new TextBox { Location = new Point(225, 15), Size = new Size(410, 28), Font = new Font("Lexend", 10F) };
            PlaceholderProvider.SetPlaceholder(TxtQuestion, "Câu hỏi hoặc gợi ý");

            TxtAnswer = new TextBox { Location = new Point(15, 50), Size = new Size(200, 28), Font = new Font("Lexend", 10F, FontStyle.Bold) };
            PlaceholderProvider.SetPlaceholder(TxtAnswer, "Đáp án đúng");

            BtnRemove = new Button { Text = "Xóa", Location = new Point(585, 48), BackColor = Color.LightCoral, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            this.Controls.AddRange(new Control[] { TxtImageName, TxtQuestion, TxtAnswer, BtnRemove });
        }
        public WordScrambleItem GetData() { return new WordScrambleItem { ImageHintResourceName = TxtImageName.Text, Question = TxtQuestion.Text, Answer = TxtAnswer.Text.ToUpper() }; }
        public void SetData(WordScrambleItem item) { TxtImageName.Text = item.ImageHintResourceName; TxtImageName.ForeColor = Color.Black; TxtQuestion.Text = item.Question; TxtQuestion.ForeColor = Color.Black; TxtAnswer.Text = item.Answer; TxtAnswer.ForeColor = Color.Black; }
    }
    #endregion

    #region Form Nhập Liệu Game
    public class GameDataInputForm : Form
    {
        private readonly string _maMNG; private readonly string _tenMNG;
        private readonly Color kahootRed = Color.FromArgb(226, 27, 60), kahootBlue = Color.FromArgb(19, 104, 206), kahootYellow = Color.FromArgb(216, 158, 0), kahootGreen = Color.FromArgb(40, 135, 63), kahootPurple = Color.FromArgb(70, 31, 137), lightGrayBg = Color.FromArgb(242, 242, 242);
        private Panel pnlHeader, pnlToolbar; private PictureBox picGameIcon; private Label lblGameName; private FlowLayoutPanel pnlInputArea; private RoundedButton btnSave, btnPlay, btnLoadExcel, btnReload;

        public GameDataInputForm(string maMNG, string tenMNG) { _maMNG = maMNG; _tenMNG = tenMNG; InitializeComponent(); BuildInputUI(); }

        private void InitializeComponent()
        {
            this.Text = "Soạn nội dung cho game: " + _tenMNG; this.Size = new Size(800, 700); this.StartPosition = FormStartPosition.CenterScreen; this.BackColor = lightGrayBg; this.Font = new Font("Lexend", 10F);
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.White };
            picGameIcon = new PictureBox { Size = new Size(50, 50), Location = new Point(20, 15), SizeMode = PictureBoxSizeMode.Zoom };
            try { picGameIcon.Image = (Image)Properties.Resources.ResourceManager.GetObject(_maMNG); } catch { try { picGameIcon.Image = Properties.Resources.placeholder; } catch { } }
            lblGameName = new Label { Text = _tenMNG, Font = new Font("Lexend", 18F, FontStyle.Bold), ForeColor = kahootPurple, AutoSize = true, Location = new Point(80, 20) };
            pnlHeader.Controls.AddRange(new Control[] { picGameIcon, lblGameName });
            pnlToolbar = new Panel { Dock = DockStyle.Bottom, Height = 80, BackColor = Color.White, Padding = new Padding(10) };
            btnPlay = new RoundedButton { Text = "Bắt đầu chơi", Dock = DockStyle.Right, Width = 150, BackColor = kahootGreen, ForeColor = Color.White, Font = new Font("Lexend", 11F, FontStyle.Bold), CornerRadius = 10 };
            btnSave = new RoundedButton { Text = "Lưu dữ liệu", Dock = DockStyle.Right, Width = 150, BackColor = kahootBlue, ForeColor = Color.White, Font = new Font("Lexend", 11F, FontStyle.Bold), Margin = new Padding(0, 0, 10, 0), CornerRadius = 10 };
            btnReload = new RoundedButton { Text = "Tải lại", Dock = DockStyle.Left, Width = 120, BackColor = kahootYellow, ForeColor = Color.White, Font = new Font("Lexend", 11F, FontStyle.Bold), CornerRadius = 10 };
            btnLoadExcel = new RoundedButton { Text = "Tải từ Excel", Dock = DockStyle.Left, Width = 120, BackColor = Color.Gray, ForeColor = Color.White, Font = new Font("Lexend", 11F, FontStyle.Bold), Margin = new Padding(10, 0, 0, 0), CornerRadius = 10 };
            pnlToolbar.Controls.AddRange(new Control[] { btnPlay, btnSave, btnReload, btnLoadExcel });
            pnlInputArea = new FlowLayoutPanel { Dock = DockStyle.Fill, BackColor = lightGrayBg, Padding = new Padding(20), AutoScroll = true };
            this.Controls.AddRange(new Control[] { pnlInputArea, pnlToolbar, pnlHeader });
            btnSave.Click += BtnSave_Click; btnPlay.Click += BtnPlay_Click; btnReload.Click += (s, e) => BuildInputUI(); btnLoadExcel.Click += BtnLoadExcel_Click;
        }

        private void BuildInputUI()
        {
            pnlInputArea.Controls.Clear();
            switch (_maMNG)
            {
                case "MNG01": BuildQuizUI(); break;
                case "MNG03": BuildFlashcardUI(); break;
                case "MNG02": case "MNG08": BuildListUI(); break;
                case "MNG09": BuildMinMaxUI(); break;
                case "MNG04": BuildWordScrambleUI(); break;
                case "MNG06": BuildSentenceScrambleUI(); break;
                case "MNG07": BuildFillBlankUI(); break;
                default:
                    var lbl = new Label { Text = "Game này không yêu cầu nhập liệu hoặc chưa được triển khai.", Font = new Font("Lexend", 14F), AutoSize = true };
                    pnlInputArea.Controls.Add(lbl); btnSave.Enabled = btnReload.Enabled = btnLoadExcel.Enabled = false; break;
            }
        }
        private void BuildQuizUI() { foreach (var q in GameDataManager.GetQuizQuestions(_maMNG)) AddQuizQuestionControl(q); var btnAdd = new Button { Text = "+ Thêm câu hỏi", Width = 650, Height = 50, BackColor = kahootRed, ForeColor = Color.White, Font = new Font("Lexend", 12F), FlatStyle = FlatStyle.Flat }; btnAdd.FlatAppearance.BorderSize = 0; btnAdd.Click += (s, e) => AddQuizQuestionControl(null); pnlInputArea.Controls.Add(btnAdd); }
        private void AddQuizQuestionControl(QuizQuestion data) { var qc = new QuizQuestionControl(); if (data != null) qc.SetData(data); qc.BtnRemove.Click += (s, e) => pnlInputArea.Controls.Remove(qc); int idx = pnlInputArea.Controls.Count > 0 ? pnlInputArea.Controls.Count - 1 : 0; pnlInputArea.Controls.Add(qc); pnlInputArea.Controls.SetChildIndex(qc, idx); }
        private void BuildWordScrambleUI() { foreach (var item in GameDataManager.GetWordScrambleItems(_maMNG)) AddWordScrambleControl(item); var btnAdd = new Button { Text = "+ Thêm từ vựng", Width = 650, Height = 50, BackColor = kahootRed, ForeColor = Color.White, Font = new Font("Lexend", 12F), FlatStyle = FlatStyle.Flat }; btnAdd.FlatAppearance.BorderSize = 0; btnAdd.Click += (s, e) => AddWordScrambleControl(null); pnlInputArea.Controls.Add(btnAdd); }
        private void AddWordScrambleControl(WordScrambleItem data) { var wc = new WordScrambleControl(); if (data != null) wc.SetData(data); wc.BtnRemove.Click += (s, e) => pnlInputArea.Controls.Remove(wc); int idx = pnlInputArea.Controls.Count > 0 ? pnlInputArea.Controls.Count - 1 : 0; pnlInputArea.Controls.Add(wc); pnlInputArea.Controls.SetChildIndex(wc, idx); }
        private void BuildFlashcardUI()
        {
            var items = GameDataManager.GetFlashcardItems(_maMNG);
            foreach (var item in items) { AddFlashcardPanel(item); }
            var btnAdd = new Button { Text = "+ Thêm thẻ", Width = 650, Height = 40, BackColor = kahootRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => AddFlashcardPanel(null);
            pnlInputArea.Controls.Add(btnAdd);
        }
        private void AddFlashcardPanel(FlashcardItem item)
        {
            var pnl = new Panel { Width = 650, Height = 50, Margin = new Padding(5), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
            var txtTerm = new TextBox { Width = 300, Dock = DockStyle.Left, Font = this.Font };
            var txtDef = new TextBox { Width = 300, Dock = DockStyle.Right, Font = this.Font };
            PlaceholderProvider.SetPlaceholder(txtTerm, "Thuật ngữ (Mặt trước)");
            PlaceholderProvider.SetPlaceholder(txtDef, "Định nghĩa (Mặt sau)");
            if (item != null) { txtTerm.Text = item.Term; txtTerm.ForeColor = Color.Black; txtDef.Text = item.Definition; txtDef.ForeColor = Color.Black; }
            pnl.Controls.AddRange(new Control[] { txtTerm, txtDef });
            int idx = pnlInputArea.Controls.Count > 0 ? pnlInputArea.Controls.Count - 1 : 0;
            pnlInputArea.Controls.Add(pnl); pnlInputArea.Controls.SetChildIndex(pnl, idx);
        }
        private void BuildListUI() { var items = GameDataManager.GetListFromString(_maMNG); var rtb = new RichTextBox { Width = 650, Height = 400, Font = this.Font, Text = string.Join("\n", items) }; var lblGuide = new Label { Text = "Nhập mỗi mục trên một dòng.", Width = 650, AutoSize = true, ForeColor = Color.Gray }; pnlInputArea.Controls.AddRange(new Control[] { lblGuide, rtb }); }
        private void BuildMinMaxUI() { var list = GameDataManager.GetListFromString(_maMNG); var pnl = new Panel { Width = 400, Height = 50 }; var numMin = new NumericUpDown { Minimum = 0, Maximum = 9999, Width = 150, Dock = DockStyle.Left, Font = this.Font, Value = list.Count > 0 ? int.Parse(list[0]) : 1 }; var numMax = new NumericUpDown { Minimum = 1, Maximum = 10000, Width = 150, Dock = DockStyle.Right, Font = this.Font, Value = list.Count > 1 ? int.Parse(list[1]) : 100 }; pnl.Controls.AddRange(new Control[] { numMin, numMax }); pnlInputArea.Controls.Add(pnl); }
        private void BuildFillBlankUI() { var item = GameDataManager.GetFillBlankQuestion(_maMNG); var pnl = new Panel { Width = 650, Height = 80, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White, Padding = new Padding(10) }; var txtQuestion = new TextBox { Dock = DockStyle.Top, Font = new Font("Lexend", 11F) }; PlaceholderProvider.SetPlaceholder(txtQuestion, "Câu hỏi (dùng ___ cho chỗ trống)"); if (!string.IsNullOrEmpty(item.QuestionText)) { txtQuestion.Text = item.QuestionText; txtQuestion.ForeColor = Color.Black; } var txtAnswer = new TextBox { Dock = DockStyle.Bottom, Font = new Font("Lexend", 11F, FontStyle.Bold) }; PlaceholderProvider.SetPlaceholder(txtAnswer, "Đáp án đúng"); if (!string.IsNullOrEmpty(item.Answer)) { txtAnswer.Text = item.Answer; txtAnswer.ForeColor = Color.Black; } pnl.Controls.AddRange(new Control[] { txtQuestion, txtAnswer }); pnlInputArea.Controls.Add(pnl); }
        private void BuildSentenceScrambleUI() { var item = GameDataManager.GetSentenceScrambleItem(_maMNG); var txtSentence = new TextBox { Width = 650, Height = 100, Multiline = true, Font = new Font("Lexend", 11F) }; PlaceholderProvider.SetPlaceholder(txtSentence, "Nhập câu hoàn chỉnh tại đây"); if (!string.IsNullOrEmpty(item.CorrectSentence)) { txtSentence.Text = item.CorrectSentence; txtSentence.ForeColor = Color.Black; } pnlInputArea.Controls.Add(txtSentence); }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                switch (_maMNG)
                {
                    case "MNG01": GameDataManager.SaveQuizQuestions(_maMNG, pnlInputArea.Controls.OfType<QuizQuestionControl>().Select(qc => qc.GetData()).ToList()); break;
                    case "MNG04": GameDataManager.SaveWordScrambleItems(_maMNG, pnlInputArea.Controls.OfType<WordScrambleControl>().Select(wc => wc.GetData()).ToList()); break;
                    case "MNG03": var flashcardData = pnlInputArea.Controls.OfType<Panel>().Select(pnl => new FlashcardItem { Term = pnl.Controls.OfType<TextBox>().First().Text, Definition = pnl.Controls.OfType<TextBox>().Last().Text }).ToList(); GameDataManager.SaveFlashcardItems(_maMNG, flashcardData); break;
                    case "MNG02": case "MNG08": GameDataManager.SaveListToString(_maMNG, pnlInputArea.Controls.OfType<RichTextBox>().First().Text.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim()).ToList()); break;
                    case "MNG09": var panelMinMax = pnlInputArea.Controls.OfType<Panel>().First(); GameDataManager.SaveListToString(_maMNG, new List<string> { panelMinMax.Controls.OfType<NumericUpDown>().First().Value.ToString(), panelMinMax.Controls.OfType<NumericUpDown>().Last().Value.ToString() }); break;
                    case "MNG06": GameDataManager.SaveSentenceScrambleItem(_maMNG, new SentenceScrambleItem { CorrectSentence = pnlInputArea.Controls.OfType<TextBox>().First().Text }); break;
                    case "MNG07": var panelFillBlank = pnlInputArea.Controls.OfType<Panel>().First(); GameDataManager.SaveFillBlankQuestion(_maMNG, new FillBlankQuestion { QuestionText = panelFillBlank.Controls.OfType<TextBox>().First().Text, Answer = panelFillBlank.Controls.OfType<TextBox>().Last().Text }); break;
                }
                MessageBox.Show("Lưu dữ liệu thành công!", "Thành công");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi khi lưu dữ liệu. Vui lòng kiểm tra lại định dạng đã nhập.\nChi tiết: " + ex.Message, "Lỗi"); }
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            Form gameForm = null;
            try
            {
                switch (_maMNG)
                {
                    case "MNG01": gameForm = new QuizGameForm(GameDataManager.GetQuizQuestions(_maMNG)); break;
                    case "MNG02": gameForm = new LuckyWheelForm(GameDataManager.GetListFromString(_maMNG)); break;
                    case "MNG03": gameForm = new FlashcardForm(GameDataManager.GetFlashcardItems(_maMNG)); break;
                    case "MNG08": gameForm = new LatTheForm(GameDataManager.GetListFromString(_maMNG)); break;
                    case "MNG09": gameForm = new RandomSoForm(GameDataManager.GetListFromString(_maMNG)); break;
                    case "MNG04": gameForm = new GheChuForm(GameDataManager.GetWordScrambleItems(_maMNG)); break;
                    case "MNG05": gameForm = new NgheChonHinhForm(GameDataManager.GetListenChooseItems(_maMNG)); break;
                    case "MNG10": gameForm = new PassBallForm(GameDataManager.GetQuizQuestions("MNG01")); break;
                    case "MNG06": gameForm = new SapXepCauForm(GameDataManager.GetSentenceScrambleItem(_maMNG)); break;
                    case "MNG07": gameForm = new DienTuForm(new List<FillBlankQuestion> { GameDataManager.GetFillBlankQuestion(_maMNG) }); break;
                    default: MessageBox.Show($"Game '{_tenMNG}' chưa có màn hình chơi.", "Thông báo"); return;
                }
                if (gameForm != null && !gameForm.IsDisposed) { this.Hide(); gameForm.ShowDialog(); }
            }
            catch (Exception ex) { MessageBox.Show("Không thể khởi động game: " + ex.Message, "Lỗi"); }
            finally { if (!this.IsDisposed) this.Close(); }
        }
        private void BtnLoadExcel_Click(object sender, EventArgs e) { MessageBox.Show("Chức năng này cần được lập trình để đọc file Excel."); }
    }
    #endregion

    #region Các Form Game
    // ===================================================================
    // GAME 1: QUIZ NHANH (MNG01)
    // ===================================================================
    public class QuizGameForm : GameFormWithMusic
    {
        private List<QuizQuestion> _questions;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private Label lblQuestion, lblScore, lblQuestionCount;
        private List<RoundedButton> optionButtons;

        public QuizGameForm(List<QuizQuestion> questions)
        {
            if (questions == null || questions.Count == 0) { CloseWithWarning(); return; }
            _questions = questions;
            InitializeComponent();
            LoadQuestion();
        }

        private void InitializeComponent()
        {
            this.Text = "📝 Quiz Vui Vẻ"; this.Size = new Size(800, 600); this.StartPosition = FormStartPosition.CenterParent; this.BackColor = BgColor; this.FormBorderStyle = FormBorderStyle.FixedSingle; this.MaximizeBox = false;
            Panel headerPanel = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(20) };
            lblQuestionCount = new Label { Dock = DockStyle.Left, ForeColor = MutedTextColor, Font = GameFont(14, FontStyle.Regular), AutoSize = true };
            lblScore = new Label { Dock = DockStyle.Right, Text = "Điểm: 0", ForeColor = SecondaryColor, Font = GameFont(16), AutoSize = true };
            headerPanel.Controls.AddRange(new Control[] { lblQuestionCount, lblScore });
            lblQuestion = new Label { MaximumSize = new Size(700, 0), AutoSize = true, Text = "Câu hỏi...", ForeColor = TextColor, Font = GameFont(20), TextAlign = ContentAlignment.MiddleCenter, Location = new Point(50, 150) };
            TableLayoutPanel tlp = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 220, Padding = new Padding(50, 10, 50, 20), ColumnCount = 2, RowCount = 2 };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 50F)); tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            optionButtons = new List<RoundedButton>();
            string[] prefixes = { "A", "B", "C", "D" };
            for (int i = 0; i < 4; i++)
            {
                var btn = new RoundedButton { Font = GameFont(14), Tag = prefixes[i], CornerRadius = 30, BackColor = Color.White, ForeColor = TextColor, FlatStyle = FlatStyle.Flat, Height = 70, Dock = DockStyle.Fill, Margin = new Padding(15) };
                btn.FlatAppearance.BorderSize = 1; btn.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                optionButtons.Add(btn); tlp.Controls.Add(btn, i % 2, i / 2);
            }
            this.Controls.AddRange(new Control[] { lblQuestion, tlp, headerPanel });
        }

        private void LoadQuestion()
        {
            if (currentQuestionIndex < _questions.Count)
            {
                lblQuestionCount.Text = $"Câu hỏi {currentQuestionIndex + 1}/{_questions.Count}";
                QuizQuestion q = _questions[currentQuestionIndex];
                lblQuestion.Text = q.QuestionText; lblQuestion.Location = new Point((this.ClientSize.Width - lblQuestion.Width) / 2, 150);
                for (int i = 0; i < 4; i++)
                {
                    optionButtons[i].Text = $"{((char)('A' + i))}. {q.Options[i]}";
                    optionButtons[i].Click -= OptionButton_Click; optionButtons[i].Click += OptionButton_Click;
                    optionButtons[i].BackColor = Color.White; optionButtons[i].ForeColor = TextColor; optionButtons[i].Enabled = true;
                }
            }
            else EndGame();
        }

        private async void OptionButton_Click(object sender, EventArgs e)
        {
            RoundedButton clickedButton = sender as RoundedButton;
            QuizQuestion q = _questions[currentQuestionIndex];
            optionButtons.ForEach(btn => btn.Enabled = false);
            if (clickedButton.Tag.ToString() == q.CorrectAnswer.ToUpper())
            {
                score++; lblScore.Text = $"Điểm: {score}";
                clickedButton.BackColor = CorrectColor; clickedButton.ForeColor = Color.White;
            }
            else
            {
                clickedButton.BackColor = IncorrectColor; clickedButton.ForeColor = Color.White;
                var correctButton = optionButtons.First(btn => btn.Tag.ToString() == q.CorrectAnswer.ToUpper());
                correctButton.BackColor = CorrectColor; correctButton.ForeColor = Color.White;
            }
            await Task.Delay(1500); currentQuestionIndex++; LoadQuestion();
        }

        private void EndGame()
        {
            this.Controls.Clear();
            this.Controls.Add(new Label { Text = $"Hoàn thành!\nĐiểm của bạn là: {score}/{_questions.Count}", Font = GameFont(26), ForeColor = TextColor, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
        }
    }

    // ===================================================================
    // GAME 2: GỌI TÊN NGẪU NHIÊN (MNG02)
    // ===================================================================
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

    // ===================================================================
    // GAME 3: FLASHCARD (MNG03)
    // ===================================================================
    public class FlashcardForm : GameFormWithMusic
    {
        private List<FlashcardItem> _cards;
        private int currentIndex = 0;
        private bool isFlipped = false;
        private Panel cardPanel;
        private Label cardLabel, lblCardCount;
        private RoundedButton btnNext, btnPrev;

        public FlashcardForm(List<FlashcardItem> cards)
        {
            if (cards == null || cards.Count == 0) { CloseWithWarning(); return; }
            _cards = cards;
            InitializeComponent();
            ShowCard();
        }

        private void InitializeComponent()
        {
            this.Text = "📇 Thẻ Ghi Nhớ";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = BgColor;

            lblCardCount = new Label { Dock = DockStyle.Top, Height = 50, ForeColor = TextColor, Font = GameFont(14), Padding = new Padding(0, 15, 0, 0), TextAlign = ContentAlignment.MiddleCenter };

            Panel cardShadowPanel = new Panel { Size = new Size(510, 310), Location = new Point((this.ClientSize.Width - 510) / 2, (this.ClientSize.Height - 310) / 2), BackColor = Color.FromArgb(200, 200, 200) };
            cardShadowPanel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, 510, 310, 25, 25));

            cardPanel = new Panel { BackColor = Color.White, Cursor = Cursors.Hand, Size = new Size(500, 300), Location = new Point(5, 5) };
            cardPanel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, 500, 300, 20, 20));
            cardPanel.Click += (s, e) => { isFlipped = !isFlipped; ShowCardContent(); };

            cardLabel = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = GameFont(28), ForeColor = TextColor, Padding = new Padding(30), Cursor = Cursors.Hand };
            cardLabel.Click += (s, e) => { isFlipped = !isFlipped; ShowCardContent(); };
            cardPanel.Controls.Add(cardLabel);
            cardShadowPanel.Controls.Add(cardPanel);

            btnPrev = new RoundedButton { Text = "<", Size = new Size(60, 60), Font = GameFont(18), CornerRadius = 30, BackColor = PrimaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(50, (this.ClientSize.Height - 60) / 2) };
            btnNext = new RoundedButton { Text = ">", Size = new Size(60, 60), Font = GameFont(18), CornerRadius = 30, BackColor = PrimaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(this.ClientSize.Width - 110, (this.ClientSize.Height - 60) / 2) };
            btnPrev.FlatAppearance.BorderSize = btnNext.FlatAppearance.BorderSize = 0;
            btnPrev.Click += (s, e) => Navigate(-1);
            btnNext.Click += (s, e) => Navigate(1);

            this.Controls.Add(cardShadowPanel);
            this.Controls.Add(btnPrev);
            this.Controls.Add(btnNext);
            this.Controls.Add(lblCardCount);
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
            lblCardCount.Text = $"{currentIndex + 1} / {_cards.Count}";
            btnPrev.Enabled = currentIndex > 0;
            btnPrev.BackColor = btnPrev.Enabled ? PrimaryColor : Color.LightGray;
            btnNext.Enabled = currentIndex < _cards.Count - 1;
            btnNext.BackColor = btnNext.Enabled ? PrimaryColor : Color.LightGray;
        }

        private void ShowCardContent()
        {
            cardLabel.Text = isFlipped ? _cards[currentIndex].Definition : _cards[currentIndex].Term;
            cardPanel.BackColor = isFlipped ? Color.FromArgb(245, 251, 255) : Color.White;
        }
    }

    // ===================================================================
    // GAME 4: GHÉP CHỮ (MNG04)
    // ===================================================================
    public class GheChuForm : GameFormWithMusic
    {
        private List<WordScrambleItem> _items;
        private int _currentItemIndex = 0;
        private PictureBox picHint;
        private Label lblQuestion, lblStatus;
        private FlowLayoutPanel pnlAnswer, pnlChoices;

        public GheChuForm(List<WordScrambleItem> items)
        {
            if (items == null || items.Count == 0) { CloseWithWarning(); return; }
            _items = items;
            InitializeComponent();
            LoadCurrentItem();
        }

        private void InitializeComponent()
        {
            this.Text = "🧩 Ghép Chữ Đoán Hình";
            this.Size = new Size(800, 600);
            this.BackColor = BgColor;
            this.StartPosition = FormStartPosition.CenterScreen;

            picHint = new PictureBox { Size = new Size(200, 200), Location = new Point(300, 30), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            lblQuestion = new Label { Font = GameFont(16), ForeColor = TextColor, AutoSize = true, MaximumSize = new Size(700, 0), TextAlign = ContentAlignment.MiddleCenter, Location = new Point(50, 240) };

            pnlAnswer = new FlowLayoutPanel { Size = new Size(600, 70), Location = new Point(100, 300), BackColor = Color.FromArgb(220, 235, 255), Padding = new Padding(5) };
            pnlChoices = new FlowLayoutPanel { Size = new Size(600, 100), Location = new Point(100, 400), Padding = new Padding(5) };

            lblStatus = new Label { Dock = DockStyle.Bottom, Height = 50, Font = GameFont(14), TextAlign = ContentAlignment.MiddleCenter, Visible = false };

            this.Controls.Add(picHint);
            this.Controls.Add(lblQuestion);
            this.Controls.Add(pnlAnswer);
            this.Controls.Add(pnlChoices);
            this.Controls.Add(lblStatus);
        }

        private void LoadCurrentItem()
        {
            pnlAnswer.Controls.Clear();
            pnlChoices.Controls.Clear();
            lblStatus.Visible = false;

            if (_currentItemIndex >= _items.Count) { EndGame(); return; }

            WordScrambleItem current = _items[_currentItemIndex];
            try { picHint.Image = (Image)Properties.Resources.ResourceManager.GetObject(current.ImageHintResourceName); }
            catch { try { picHint.Image = Properties.Resources.placeholder; } catch { } }

            lblQuestion.Text = current.Question;
            lblQuestion.Left = (this.ClientSize.Width - lblQuestion.Width) / 2;

            var random = new Random();
            string shuffledAnswer = new string(current.Answer.ToCharArray().OrderBy(c => random.Next()).ToArray());

            for (int i = 0; i < current.Answer.Length; i++)
            {
                var slot = new Label { Text = "", Size = new Size(50, 50), Margin = new Padding(5), Font = GameFont(20), BorderStyle = BorderStyle.FixedSingle, TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.White, Cursor = Cursors.Hand };
                slot.Click += Answer_Click;
                pnlAnswer.Controls.Add(slot);
            }

            foreach (char c in shuffledAnswer)
            {
                var choice = new RoundedButton { Text = c.ToString(), Size = new Size(60, 60), Font = GameFont(22), CornerRadius = 30, BackColor = Color.White, ForeColor = TextColor };
                choice.Click += Choice_Click;
                pnlChoices.Controls.Add(choice);
            }
        }

        private void Choice_Click(object sender, EventArgs e)
        {
            Button choice = sender as Button;
            choice.Visible = false;
            foreach (Label slot in pnlAnswer.Controls)
            {
                if (string.IsNullOrEmpty(slot.Text))
                {
                    slot.Text = choice.Text;
                    slot.Tag = choice; // Lưu trữ nút đã được chọn
                    break;
                }
            }
            CheckAnswer();
        }

        private void Answer_Click(object sender, EventArgs e)
        {
            Label slot = sender as Label;
            if (!string.IsNullOrEmpty(slot.Text) && slot.Tag is Button)
            {
                (slot.Tag as Button).Visible = true; // Hiện lại nút ở khu vực chọn
                slot.Text = "";
                slot.Tag = null;
            }
        }

        private async void CheckAnswer()
        {
            string userAnswer = string.Concat(pnlAnswer.Controls.OfType<Label>().Select(l => l.Text));
            string correctAnswer = _items[_currentItemIndex].Answer;

            if (userAnswer.Length == correctAnswer.Length)
            {
                pnlChoices.Enabled = false;
                pnlAnswer.Enabled = false;

                if (userAnswer == correctAnswer)
                {
                    lblStatus.Text = "🎉 Chính xác! 🎉";
                    lblStatus.ForeColor = Color.White;
                    lblStatus.BackColor = CorrectColor;
                    foreach (Label slot in pnlAnswer.Controls) slot.BackColor = CorrectColor;
                    await Task.Delay(2000);
                    _currentItemIndex++;
                    LoadCurrentItem();
                }
                else
                {
                    lblStatus.Text = "Chưa đúng, thử lại nào!";
                    lblStatus.ForeColor = Color.White;
                    lblStatus.BackColor = IncorrectColor;
                    foreach (Label slot in pnlAnswer.Controls) slot.BackColor = IncorrectColor;
                    await Task.Delay(2000);
                }

                lblStatus.Visible = true;
                foreach (Label slot in pnlAnswer.Controls) slot.BackColor = Color.White;
                pnlChoices.Enabled = true;
                pnlAnswer.Enabled = true;
                lblStatus.BackColor = Color.Transparent;
            }
        }

        private void EndGame()
        {
            MessageBox.Show("Bạn đã hoàn thành tất cả các câu đố!", "Chúc mừng!");
            this.Close();
        }
    }

    // ===================================================================
    // GAME 5: NGHE - CHỌN HÌNH (MNG05)
    // ===================================================================
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

    // ===================================================================
    // GAME 6: SẮP XẾP CÂU (MNG06)
    // ===================================================================
    public class SapXepCauForm : GameFormWithMusic
    {
        private SentenceScrambleItem _item;
        private FlowLayoutPanel pnlChoices, pnlAnswer;
        private Label lblResult;

        public SapXepCauForm(SentenceScrambleItem item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.CorrectSentence)) { CloseWithWarning(); return; }
            _item = item;
            InitializeComponent();
            LoadGame();
        }

        private void InitializeComponent()
        {
            this.Text = "🔄 Sắp xếp câu"; this.Size = new Size(800, 600); this.BackColor = BgColor; this.StartPosition = FormStartPosition.CenterScreen;
            pnlAnswer = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 150, BackColor = Color.White, Padding = new Padding(20), Margin = new Padding(20), BorderStyle = BorderStyle.FixedSingle };
            pnlChoices = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            lblResult = new Label { Dock = DockStyle.Bottom, Height = 60, Font = GameFont(16), TextAlign = ContentAlignment.MiddleCenter, Visible = false };
            var btnCheck = new RoundedButton { Text = "Kiểm tra", Dock = DockStyle.Bottom, Height = 60, BackColor = PrimaryColor, ForeColor = Color.White, Font = GameFont(14), CornerRadius = 0 };
            btnCheck.Click += BtnCheck_Click;
            this.Controls.AddRange(new Control[] { pnlChoices, pnlAnswer, btnCheck, lblResult });
        }

        private void LoadGame()
        {
            var words = _item.CorrectSentence.Split(' ').OrderBy(x => Guid.NewGuid());
            foreach (var word in words)
            {
                var btn = new RoundedButton { Text = word, Font = GameFont(14), AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(10), Margin = new Padding(5), BackColor = SecondaryColor, ForeColor = Color.White, CornerRadius = 15 };
                btn.Click += WordButton_Click;
                pnlChoices.Controls.Add(btn);
            }
        }

        private void WordButton_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn.Parent == pnlChoices) { pnlChoices.Controls.Remove(btn); pnlAnswer.Controls.Add(btn); }
            else { pnlAnswer.Controls.Remove(btn); pnlChoices.Controls.Add(btn); }
        }

        private async void BtnCheck_Click(object sender, EventArgs e)
        {
            string userAnswer = string.Join(" ", pnlAnswer.Controls.OfType<Button>().Select(b => b.Text));
            lblResult.Visible = true;
            if (userAnswer.Equals(_item.CorrectSentence, StringComparison.OrdinalIgnoreCase))
            {
                lblResult.Text = "🎉 Chính xác! 🎉"; lblResult.ForeColor = CorrectColor;
                await Task.Delay(2000); this.Close();
            }
            else
            {
                lblResult.Text = "Chưa đúng rồi, thử lại nhé!"; lblResult.ForeColor = IncorrectColor;
            }
        }
    }

    // ===================================================================
    // GAME 7: ĐIỀN TỪ (MNG07)
    // ===================================================================
    public class DienTuForm : GameFormWithMusic
    {
        private List<FillBlankQuestion> _questions; private int _currentIndex = 0;
        private Label lblQuestion; private TextBox txtAnswer; private RoundedButton btnCheck;

        public DienTuForm(List<FillBlankQuestion> questions)
        {
            if (questions == null || questions.Count == 0 || string.IsNullOrWhiteSpace(questions[0].QuestionText)) { CloseWithWarning(); return; }
            _questions = questions; InitializeComponent(); LoadQuestion();
        }
        private void InitializeComponent()
        {
            this.Text = "✍️ Điền từ vào chỗ trống"; this.Size = new Size(800, 400); this.BackColor = BgColor; this.StartPosition = FormStartPosition.CenterScreen;
            lblQuestion = new Label { Font = GameFont(18), ForeColor = TextColor, Location = new Point(50, 80), AutoSize = true };
            txtAnswer = new TextBox { Font = GameFont(18), Location = new Point(50, 150), Width = 300 };
            btnCheck = new RoundedButton { Text = "Kiểm tra", Font = GameFont(16), BackColor = PrimaryColor, ForeColor = Color.White, CornerRadius = 20, Size = new Size(150, 40), Location = new Point(370, 148) };
            btnCheck.Click += BtnCheck_Click;
            this.Controls.AddRange(new Control[] { lblQuestion, txtAnswer, btnCheck });
        }
        private void LoadQuestion()
        {
            lblQuestion.Text = _questions[_currentIndex].QuestionText; txtAnswer.Clear(); txtAnswer.BackColor = Color.White;
            lblQuestion.Left = (this.ClientSize.Width - lblQuestion.Width) / 2;
            txtAnswer.Left = (this.ClientSize.Width - txtAnswer.Width - btnCheck.Width - 20) / 2;
            btnCheck.Left = txtAnswer.Right + 20;
        }
        private async void BtnCheck_Click(object sender, EventArgs e)
        {
            if (string.Equals(txtAnswer.Text.Trim(), _questions[_currentIndex].Answer, StringComparison.OrdinalIgnoreCase))
            {
                txtAnswer.BackColor = CorrectColor; await Task.Delay(1500);
                MessageBox.Show("Chính xác!"); this.Close();
            }
            else { txtAnswer.BackColor = IncorrectColor; }
        }
    }

    // ===================================================================
    // GAME 8: LẬT THẺ (MNG08)
    // ===================================================================
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
            this.Text = "🃏 Lật thẻ trí nhớ"; this.Size = new Size(800, 600); this.StartPosition = FormStartPosition.CenterParent; this.BackColor = BgColor;
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

        private async void Card_Click(object sender, EventArgs e)
        {
            var panel = (sender is Label) ? (sender as Label).Parent as Panel : sender as Panel;
            if (panel == null || panel.BackColor != PrimaryColor || _secondCard != null) return;
            FlipUp(panel);
            if (_firstCard == null) { _firstCard = panel; return; }
            _secondCard = panel;
            if ((_firstCard.Controls[0] as Label).Tag.ToString() == (_secondCard.Controls[0] as Label).Tag.ToString())
            {
                _matchesFound++;
                await Task.Delay(200);
                _firstCard.BackColor = _secondCard.BackColor = CorrectColor;
                _firstCard = _secondCard = null;
                if (_matchesFound == _items.Count)
                {
                    MessageBox.Show("🎉 Chúc mừng! Bạn đã tìm thấy tất cả các cặp! 🎉", "Chiến thắng!");
                    this.Close();
                }
            }
            else _timer.Start();
        }
    }

    // ===================================================================
    // GAME 9: RANDOM SỐ (MNG09)
    // ===================================================================
    public class RandomSoForm : GameFormWithMusic
    {
        private List<int> _numbers;
        private Label lblResult;
        private RoundedButton btnSpin;
        private Timer spinTimer;
        private Random random = new Random();
        private int spinTicks;

        public RandomSoForm(List<string> numberStrings)
        {
            if (numberStrings == null || numberStrings.Count < 2) { CloseWithWarning("Cần ít nhất 2 số (min và max)."); return; }
            _numbers = numberStrings.Select(s => { int.TryParse(s, out int n); return n; }).Where(n => n != 0).ToList();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "🎲 Quay Số Ngẫu Nhiên"; this.Size = new Size(800, 600); this.StartPosition = FormStartPosition.CenterParent; this.BackColor = Color.FromArgb(26, 176, 134);
            lblResult = new Label { Text = "00", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = GameFont(150), ForeColor = Color.White };
            btnSpin = new RoundedButton { Text = "QUAY SỐ", Dock = DockStyle.Bottom, Height = 80, Font = GameFont(20), BackColor = Color.White, ForeColor = TextColor, FlatStyle = FlatStyle.Flat, CornerRadius = 35 };
            btnSpin.FlatAppearance.BorderSize = 0;
            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 120, Padding = new Padding(200, 20, 200, 20) };
            pnlBottom.Controls.Add(btnSpin);
            spinTimer = new Timer { Interval = 25 }; spinTimer.Tick += SpinTimer_Tick; btnSpin.Click += BtnSpin_Click;
            this.Controls.Add(lblResult); this.Controls.Add(pnlBottom);
        }

        private void BtnSpin_Click(object sender, EventArgs e)
        {
            if (_numbers == null || _numbers.Count < 2) { MessageBox.Show("Cần ít nhất 2 số (min và max) trong danh sách.", "Dữ liệu không hợp lệ"); return; }
            btnSpin.Enabled = false; spinTicks = 0; spinTimer.Start();
        }

        private void SpinTimer_Tick(object sender, EventArgs e)
        {
            spinTicks++; int min = _numbers.Min(); int max = _numbers.Max();
            lblResult.Text = random.Next(min, max + 1).ToString("00");
            if (spinTicks > 40)
            {
                spinTimer.Stop(); btnSpin.Enabled = true;
                int finalNumber = random.Next(min, max + 1);
                lblResult.Text = finalNumber.ToString("00"); lblResult.ForeColor = SecondaryColor;
                MessageBox.Show($"Số may mắn là: {finalNumber}", "Kết quả");
                lblResult.ForeColor = Color.White;
            }
        }
    }

    // ===================================================================
    // GAME 10: PASS A BALL (MNG10)
    // ===================================================================
    public class PassBallForm : GameFormWithMusic
    {
        private List<QuizQuestion> _questions;
        private Timer _gameTimer;
        private int _timeLeft;
        private Random _random = new Random();
        private Label lblTimer;
        private PictureBox picBall;
        private RoundedButton btnStart;
        private SoundPlayer _tickPlayer, _buzzerPlayer;

        public PassBallForm(List<QuizQuestion> questions)
        {
            if (questions == null || questions.Count == 0) { CloseWithWarning("Cần có câu hỏi từ game Quiz để chơi."); return; }
            _questions = questions;
            InitializeComponent();
            SetupSounds();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _tickPlayer?.Dispose();
            _buzzerPlayer?.Dispose();
        }

        private void SetupSounds()
        {
            try { _tickPlayer = new SoundPlayer(Properties.Resources.tick_sound); } catch { }
            try { _buzzerPlayer = new SoundPlayer(Properties.Resources.buzzer_sound); } catch { }
        }

        private void InitializeComponent()
        {
            this.Text = "⚽ Chuyền Bóng Nóng"; this.Size = new Size(800, 600); this.BackColor = Color.FromArgb(41, 52, 98); this.StartPosition = FormStartPosition.CenterScreen;
            lblTimer = new Label { Font = GameFont(120), Text = "00", ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            picBall = new PictureBox { Size = new Size(150, 150), Location = new Point(325, 50), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Transparent };
            try { picBall.Image = Properties.Resources.pass_ball; } catch { }
            btnStart = new RoundedButton { Text = "BẮT ĐẦU", Size = new Size(250, 70), Location = new Point(275, 450), Font = GameFont(24), BackColor = SecondaryColor, ForeColor = Color.White, CornerRadius = 35 };
            btnStart.Click += Start_Click;
            _gameTimer = new Timer { Interval = 1000 }; _gameTimer.Tick += GameTimer_Tick;
            this.Controls.AddRange(new Control[] { picBall, lblTimer, btnStart });
        }

        private void Start_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false; btnStart.Text = "ĐANG CHẠY..."; btnStart.BackColor = Color.Gray;
            _timeLeft = _random.Next(15, 31);
            lblTimer.Text = _timeLeft.ToString("00");
            _tickPlayer?.PlayLooping();
            _gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            _timeLeft--;
            lblTimer.Text = _timeLeft.ToString("00");
            if (_timeLeft <= 0)
            {
                _gameTimer.Stop();
                _tickPlayer?.Stop();
                _buzzerPlayer?.Play();
                ShowQuestion();
            }
        }

        private void ShowQuestion()
        {
            QuizQuestion randomQ = _questions[_random.Next(_questions.Count)];
            string questionText = $"{randomQ.QuestionText}\n\nA. {randomQ.Options[0]}\nB. {randomQ.Options[1]}\nC. {randomQ.Options[2]}\nD. {randomQ.Options[3]}";
            MessageBox.Show(questionText, "Câu hỏi!", MessageBoxButtons.OK);
            MessageBox.Show($"Đáp án đúng là: {randomQ.CorrectAnswer}", "Đáp án");
            ResetGame();
        }

        private void ResetGame()
        {
            lblTimer.Text = "00";
            btnStart.Enabled = true; btnStart.Text = "BẮT ĐẦU"; btnStart.BackColor = SecondaryColor;
        }
    }
    #endregion
}