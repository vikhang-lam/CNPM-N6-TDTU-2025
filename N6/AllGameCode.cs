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
        public static List<SentenceScrambleItem> GetSentenceScrambleItems(string maMNG)
        {
            var data = new List<SentenceScrambleItem>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return data;
            
            // Tách theo ký tự | để lấy từng câu riêng biệt
            var sentences = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sentence in sentences)
            {
                var trimmedSentence = sentence.Trim();
                if (!string.IsNullOrEmpty(trimmedSentence))
                {
                    data.Add(new SentenceScrambleItem { CorrectSentence = trimmedSentence });
                }
            }
            return data;
        }

        public static void SaveSentenceScrambleItems(string maMNG, List<SentenceScrambleItem> items)
        {
            var sentences = items.Where(item => !string.IsNullOrWhiteSpace(item.CorrectSentence))
                                 .Select(item => item.CorrectSentence.Trim());
            SaveRawData(maMNG, string.Join("|", sentences));
        }
        
        public static SentenceScrambleItem GetSentenceScrambleItem(string maMNG) { return new SentenceScrambleItem { CorrectSentence = GetRawData(maMNG) ?? "" }; }
        public static void SaveSentenceScrambleItem(string maMNG, SentenceScrambleItem item) { SaveRawData(maMNG, item.CorrectSentence); }
        public static List<FillBlankQuestion> GetFillBlankQuestions(string maMNG)
        {
            var data = new List<FillBlankQuestion>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return data;

            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]))
                {
                    data.Add(new FillBlankQuestion
                    {
                        QuestionText = parts[0].Trim(),
                        Answer = parts[1].Trim()
                    });
                }
            }
            return data;
        }

        public static void SaveFillBlankQuestions(string maMNG, List<FillBlankQuestion> questions)
        {
            var lines = questions.Where(q => !string.IsNullOrWhiteSpace(q.QuestionText))
                                 .Select(q => $"{q.QuestionText.Trim()};{q.Answer.Trim()}");
            SaveRawData(maMNG, string.Join("|", lines));
        }
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
            this.Size = new Size(680, 220);
            this.BackColor = Color.White;
            this.Padding = new Padding(15);
            this.Margin = new Padding(5, 10, 5, 10);
            this.BorderStyle = BorderStyle.None;

            // Tạo border
            this.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                }
            };

            // ===== PANEL HEADER =====
            Panel pnlHeader = new Panel
            {
                Location = new Point(15, 15),
                Size = new Size(560, 35),
                BackColor = Color.FromArgb(245, 248, 250)
            };

            Label lblQuestionIcon = new Label
            {
                Text = "❓",
                Font = new Font("Segoe UI", 14F, FontStyle.Regular),
                Location = new Point(10, 5),
                AutoSize = true,
                ForeColor = Color.FromArgb(19, 104, 206)
            };

            Label lblQuestionLabel = new Label
            {
                Text = "Câu hỏi:",
                Font = new Font("Lexend", 10F, FontStyle.Bold),
                Location = new Point(40, 8),
                AutoSize = true,
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            pnlHeader.Controls.AddRange(new Control[] { lblQuestionIcon, lblQuestionLabel });

            // ===== NÚT XÓA (ĐẶT BÊN PHẢI HEADER) =====
            BtnRemove = new Button
            {
                Text = "🗑️ Xóa",
                Location = new Point(580, 17),
                Size = new Size(80, 28),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Lexend", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            BtnRemove.FlatAppearance.BorderSize = 0;

            // ===== TEXTBOX CÂU HỎI =====
            TxtQuestion = new TextBox
            {
                Location = new Point(15, 60),
                Size = new Size(650, 32),
                Font = new Font("Lexend", 11F, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            PlaceholderProvider.SetPlaceholder(TxtQuestion, "Nhập nội dung câu hỏi tại đây...");

            // ===== LABEL ĐÁP ÁN + COMBOBOX CÙNG DÒNG =====
            Label lblOptionsLabel = new Label
            {
                Text = "📝 Các đáp án:",
                Font = new Font("Lexend", 9F, FontStyle.Bold),
                Location = new Point(15, 108),
                AutoSize = true,
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            Label lblCorrectLabel = new Label
            {
                Text = "✓ Đáp án đúng:",
                Font = new Font("Lexend", 9F, FontStyle.Bold),
                Location = new Point(450, 108),
                AutoSize = true,
                ForeColor = Color.FromArgb(40, 135, 63)
            };

            CboCorrectAnswer = new ComboBox
            {
                Location = new Point(565, 105),
                Size = new Size(100, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Lexend", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(212, 237, 218),
                FlatStyle = FlatStyle.Flat
            };
            CboCorrectAnswer.Items.AddRange(new object[] { "A", "B", "C", "D" });
            CboCorrectAnswer.SelectedIndex = 0;

            // ===== CÁC TEXTBOX ĐÁP ÁN (CÂN ĐỐI BADGE VỚI TEXTBOX) =====
            int badgeWidth = 20;
            int badgeHeight = 20;
            int textBoxWidth = 285;
            int textBoxHeight = 30;
            int leftColumnX = 15;
            int rightColumnX = 345;

            // Đáp án A
            Label lblA = new Label
            {
                Text = "A",
                Font = new Font("Lexend", 10F, FontStyle.Bold),
                Location = new Point(leftColumnX, 145),
                Size = new Size(badgeWidth, badgeHeight),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(87, 187, 247),
                ForeColor = Color.White
            };

            TxtOptionA = new TextBox
            {
                Location = new Point(leftColumnX + badgeWidth + 5, 145),
                Size = new Size(textBoxWidth, textBoxHeight),
                Font = new Font("Lexend", 9F),
                BorderStyle = BorderStyle.FixedSingle
            };
            PlaceholderProvider.SetPlaceholder(TxtOptionA, "Nhập đáp án A");

            // Đáp án B
            Label lblB = new Label
            {
                Text = "B",
                Font = new Font("Lexend", 10F, FontStyle.Bold),
                Location = new Point(rightColumnX, 145),
                Size = new Size(badgeWidth, badgeHeight),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(255, 189, 89),
                ForeColor = Color.White
            };

            TxtOptionB = new TextBox
            {
                Location = new Point(rightColumnX + badgeWidth + 5, 145),
                Size = new Size(textBoxWidth, textBoxHeight),
                Font = new Font("Lexend", 9F),
                BorderStyle = BorderStyle.FixedSingle
            };
            PlaceholderProvider.SetPlaceholder(TxtOptionB, "Nhập đáp án B");

            // Đáp án C
            Label lblC = new Label
            {
                Text = "C",
                Font = new Font("Lexend", 10F, FontStyle.Bold),
                Location = new Point(leftColumnX, 185),
                Size = new Size(badgeWidth, badgeHeight),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(29, 209, 161),
                ForeColor = Color.White
            };

            TxtOptionC = new TextBox
            {
                Location = new Point(leftColumnX + badgeWidth + 5, 185),
                Size = new Size(textBoxWidth, textBoxHeight),
                Font = new Font("Lexend", 9F),
                BorderStyle = BorderStyle.FixedSingle
            };
            PlaceholderProvider.SetPlaceholder(TxtOptionC, "Nhập đáp án C");

            // Đáp án D
            Label lblD = new Label
            {
                Text = "D",
                Font = new Font("Lexend", 10F, FontStyle.Bold),
                Location = new Point(rightColumnX, 185),
                Size = new Size(badgeWidth, badgeHeight),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(255, 118, 117),
                ForeColor = Color.White
            };

            TxtOptionD = new TextBox
            {
                Location = new Point(rightColumnX + badgeWidth + 5, 185),
                Size = new Size(textBoxWidth, textBoxHeight),
                Font = new Font("Lexend", 9F),
                BorderStyle = BorderStyle.FixedSingle
            };
            PlaceholderProvider.SetPlaceholder(TxtOptionD, "Nhập đáp án D");

            this.Controls.AddRange(new Control[]
            {
                pnlHeader,
                BtnRemove,
                TxtQuestion,
                lblOptionsLabel,
                lblCorrectLabel,
                CboCorrectAnswer,
                lblA, TxtOptionA,
                lblB, TxtOptionB,
                lblC, TxtOptionC,
                lblD, TxtOptionD
            });
        }

        public QuizQuestion GetData()
        {
            return new QuizQuestion
            {
                QuestionText = TxtQuestion.Text,
                Options = new List<string> { TxtOptionA.Text, TxtOptionB.Text, TxtOptionC.Text, TxtOptionD.Text },
                CorrectAnswer = CboCorrectAnswer.SelectedItem.ToString()
            };
        }

        public void SetData(QuizQuestion q)
        {
            TxtQuestion.Text = q.QuestionText;
            TxtQuestion.ForeColor = Color.Black;
            TxtOptionA.Text = q.Options[0];
            TxtOptionA.ForeColor = Color.Black;
            TxtOptionB.Text = q.Options[1];
            TxtOptionB.ForeColor = Color.Black;
            TxtOptionC.Text = q.Options[2];
            TxtOptionC.ForeColor = Color.Black;
            TxtOptionD.Text = q.Options[3];
            TxtOptionD.ForeColor = Color.Black;
            CboCorrectAnswer.SelectedItem = q.CorrectAnswer;
        }
    }

    public class WordScrambleControl : UserControl
    {
        public TextBox TxtImageName { get; private set; }
        public TextBox TxtQuestion { get; private set; }
        public TextBox TxtAnswer { get; private set; }
        public Button BtnRemove { get; private set; }
        public Button BtnBrowseImage { get; private set; }
        public PictureBox PicPreview { get; private set; }

        public WordScrambleControl()
        {
            this.Size = new Size(720, 180);
            this.BackColor = Color.White;
            this.Margin = new Padding(5, 8, 15, 8);
            this.BorderStyle = BorderStyle.None;

            // Rounded corners và shadow
            this.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 4, 4, this.Width - 4, this.Height - 4);
                }

                // Background with rounded corners
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, this.Width - 1, this.Height - 1), 12))
                {
                    using (var brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    using (var pen = new Pen(Color.FromArgb(230, 230, 230), 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            // ===== LEFT SECTION: IMAGE PREVIEW =====
            Panel pnlImageSection = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(120, 140),
                BackColor = Color.Transparent
            };

            PicPreview = new PictureBox
            {
                Location = new Point(0, 0),
                Size = new Size(120, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.None
            };

            PicPreview.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(220, 220, 220), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, PicPreview.Width - 1, PicPreview.Height - 1);
                }
            };

            // 1. THU NHỎ NÚT "CHỌN ẢNH"
            BtnBrowseImage = new Button
            {
                Text = "📁 Chọn ảnh",
                Location = new Point(0, 108),
                Size = new Size(120, 28),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            BtnBrowseImage.FlatAppearance.BorderSize = 0;
            BtnBrowseImage.Click += BtnBrowseImage_Click;
            BtnBrowseImage.MouseEnter += (s, e) => BtnBrowseImage.BackColor = Color.FromArgb(0, 103, 235);
            BtnBrowseImage.MouseLeave += (s, e) => BtnBrowseImage.BackColor = Color.FromArgb(0, 123, 255);

            pnlImageSection.Controls.AddRange(new Control[] { PicPreview, BtnBrowseImage });

            // ===== MIDDLE SECTION: FORM INPUTS =====
            Panel pnlFormSection = new Panel
            {
                Location = new Point(155, 20),
                Size = new Size(390, 140),
                BackColor = Color.Transparent
            };

            // Hình ảnh gợi ý
            Label lblImagePath = new Label
            {
                Text = "🖼️ Hình ảnh gợi ý:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Location = new Point(0, 5),
                AutoSize = true
            };

            TxtImageName = new TextBox
            {
                Location = new Point(0, 28),
                Size = new Size(390, 28),
                Font = new Font("Segoe UI", 9F),
                ReadOnly = true,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };
            PlaceholderProvider.SetPlaceholder(TxtImageName, "Chưa chọn ảnh...");

            // Câu hỏi
            Label lblQuestion = new Label
            {
                Text = "💡 Câu hỏi/Gợi ý:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 193, 7),
                Location = new Point(0, 68),
                AutoSize = true
            };

            TxtQuestion = new TextBox
            {
                Location = new Point(0, 91),
                Size = new Size(390, 28),
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            PlaceholderProvider.SetPlaceholder(TxtQuestion, "Ví dụ: Hãy ghép câu hoàn chỉnh...");

            pnlFormSection.Controls.AddRange(new Control[]
            {
            lblImagePath, TxtImageName,
            lblQuestion, TxtQuestion
            });

            // ===== RIGHT SECTION: ANSWER + DELETE =====
            Panel pnlRightSection = new Panel
            {
                Location = new Point(560, 20),
                Size = new Size(150, 140),
                BackColor = Color.Transparent
            };

            // 4. THU NHỎ DẤU TICK
            Label lblAnswerCheck = new Label
            {
                Text = "✓",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                Location = new Point(0, 3),
                AutoSize = true
            };

            Label lblAnswerLabel = new Label
            {
                Text = "Đáp án:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                Location = new Point(25, 5),
                AutoSize = true
            };

            TxtAnswer = new TextBox
            {
                Location = new Point(0, 28),
                Size = new Size(150, 70),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(212, 237, 218),
                ForeColor = Color.FromArgb(40, 167, 69),
                TextAlign = HorizontalAlignment.Center,
                Multiline = true
            };
            PlaceholderProvider.SetPlaceholder(TxtAnswer, "ĐÁP ÁN");
            TxtAnswer.TextChanged += (s, e) =>
            {
                if (TxtAnswer.ForeColor != Color.Gray)
                {
                    int pos = TxtAnswer.SelectionStart;
                    TxtAnswer.Text = TxtAnswer.Text.ToUpper();
                    TxtAnswer.SelectionStart = pos;
                }
            };

            BtnRemove = new Button
            {
                Text = "🗑️ Xóa",
                Location = new Point(0, 105),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            BtnRemove.FlatAppearance.BorderSize = 0;
            BtnRemove.MouseEnter += (s, e) => BtnRemove.BackColor = Color.FromArgb(200, 35, 51);
            BtnRemove.MouseLeave += (s, e) => BtnRemove.BackColor = Color.FromArgb(220, 53, 69);

            pnlRightSection.Controls.AddRange(new Control[] { lblAnswerCheck, lblAnswerLabel, TxtAnswer, BtnRemove });

            this.Controls.AddRange(new Control[] { pnlImageSection, pnlFormSection, pnlRightSection });
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void BtnBrowseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                ofd.Title = "Chọn hình ảnh gợi ý";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (PicPreview.Image != null)
                        {
                            PicPreview.Image.Dispose();
                        }

                        using (var tempImage = Image.FromFile(ofd.FileName))
                        {
                            PicPreview.Image = new Bitmap(tempImage);
                        }

                        TxtImageName.Text = Path.GetFileName(ofd.FileName);
                        TxtImageName.ForeColor = Color.Black;
                        TxtImageName.Tag = ofd.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể tải ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public WordScrambleItem GetData()
        {
            return new WordScrambleItem
            {
                ImageHintResourceName = TxtImageName.Tag?.ToString() ?? TxtImageName.Text,
                Question = TxtQuestion.Text,
                Answer = TxtAnswer.Text.ToUpper()
            };
        }

        public void SetData(WordScrambleItem item)
        {
            TxtImageName.Text = item.ImageHintResourceName;
            TxtImageName.ForeColor = Color.Black;

            try
            {
                if (File.Exists(item.ImageHintResourceName))
                {
                    if (PicPreview.Image != null) PicPreview.Image.Dispose();
                    using (var tempImage = Image.FromFile(item.ImageHintResourceName))
                    {
                        PicPreview.Image = new Bitmap(tempImage);
                    }
                    TxtImageName.Tag = item.ImageHintResourceName;
                }
                else
                {
                    var resourceImage = Properties.Resources.ResourceManager.GetObject(item.ImageHintResourceName);
                    if (resourceImage != null) PicPreview.Image = (Image)resourceImage;
                }
            }
            catch { PicPreview.Image = null; }

            TxtQuestion.Text = item.Question;
            TxtQuestion.ForeColor = Color.Black;
            TxtAnswer.Text = item.Answer;
            TxtAnswer.ForeColor = Color.FromArgb(40, 167, 69);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                PicPreview?.Image?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    public class FillBlankControl : UserControl
    {
        public TextBox TxtQuestion { get; private set; }
        public TextBox TxtAnswer { get; private set; }
        public Button BtnRemove { get; private set; }

        public FillBlankControl()
        {
            // THU NHỎ CHIỀU CAO: 140 → 115
            this.Size = new Size(720, 115);
            this.BackColor = Color.White;
            this.Margin = new Padding(5, 10, 5, 10);
            this.BorderStyle = BorderStyle.None;

            // Vẽ shadow và rounded corners
            this.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Shadow effect
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 4, 4, this.Width - 4, this.Height - 4);
                }

                // Background với rounded corners
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, this.Width - 1, this.Height - 1), 12))
                {
                    using (var brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    using (var pen = new Pen(Color.FromArgb(220, 230, 240), 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            // ===== HEADER CARD - THU NHỎ CHIỀU CAO: 40 → 35 =====
            Panel pnlCardHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(720, 35),
                BackColor = Color.FromArgb(245, 248, 250)
            };

            Label lblCardIcon = new Label
            {
                Text = "📝",
                Font = new Font("Segoe UI Emoji", 14F), // THU NHỎ: 16F → 14F
                Location = new Point(14, 6),
                AutoSize = true
            };

            Label lblCardTitle = new Label
            {
                Text = "Câu hỏi Điền từ",
                Font = new Font("Lexend", 9F, FontStyle.Bold), // THU NHỎ: 10F → 9F
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(50, 8),
                AutoSize = true
            };

            pnlCardHeader.Controls.AddRange(new Control[] { lblCardIcon, lblCardTitle });

            // ===== NÚT XÓA - DI CHUYỂN LÊN HEADER =====
            BtnRemove = new Button
            {
                Text = "🗑️ Xóa",
                Location = new Point(640, 3), // DI CHUYỂN LÊN: 73 → 3 (trong header)
                Size = new Size(70, 29),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            BtnRemove.FlatAppearance.BorderSize = 0;
            BtnRemove.MouseEnter += (s, e) => BtnRemove.BackColor = Color.FromArgb(200, 35, 51);
            BtnRemove.MouseLeave += (s, e) => BtnRemove.BackColor = Color.FromArgb(220, 53, 69);

            // ===== CỘT TRÁI - CÂU HỎI =====
            Label lblQuestionLabel = new Label
            {
                Text = "💡 Câu hỏi (sử dụng ___ cho chỗ trống):",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), // THU NHỎ: 9F → 8.5F
                ForeColor = Color.FromArgb(0, 123, 255),
                Location = new Point(20, 45), // ĐIỀU CHỈNH: 50 → 45
                AutoSize = true
            };

            TxtQuestion = new TextBox
            {
                Location = new Point(20, 65), // ĐIỀU CHỈNH: 73 → 65
                Size = new Size(450, 30),
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            PlaceholderProvider.SetPlaceholder(TxtQuestion, "Ví dụ: Con ___ là loài vật quý hiếm");

            // ===== CỘT PHẢI - ĐÁP ÁN =====
            Label lblAnswerLabel = new Label
            {
                Text = "✓ Đáp án đúng:",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), // THU NHỎ: 9F → 8.5F
                ForeColor = Color.FromArgb(40, 167, 69),
                Location = new Point(490, 45), // ĐIỀU CHỈNH: 50 → 45
                AutoSize = true
            };

            TxtAnswer = new TextBox
            {
                Location = new Point(490, 65), // ĐIỀU CHỈNH: 73 → 65
                Size = new Size(210, 30), // TĂNG WIDTH: 140 → 210 (vì bỏ nút Xóa)
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(212, 237, 218),
                ForeColor = Color.FromArgb(40, 167, 69),
                TextAlign = HorizontalAlignment.Center
            };
            PlaceholderProvider.SetPlaceholder(TxtAnswer, "Ví dụ: gấu trúc");

            // ===== ADD NÚT XÓA VÀO HEADER =====
            pnlCardHeader.Controls.Add(BtnRemove);

            this.Controls.AddRange(new Control[]
            {
                pnlCardHeader,
                lblQuestionLabel,
                TxtQuestion,
                lblAnswerLabel,
                TxtAnswer
            });
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        public FillBlankQuestion GetData()
        {
            return new FillBlankQuestion
            {
                QuestionText = TxtQuestion.Text,
                Answer = TxtAnswer.Text
            };
        }

        public void SetData(FillBlankQuestion q)
        {
            TxtQuestion.Text = q.QuestionText;
            TxtQuestion.ForeColor = Color.FromArgb(64, 64, 64);
            TxtAnswer.Text = q.Answer;
            TxtAnswer.ForeColor = Color.FromArgb(40, 167, 69);
        }
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
        private void BuildQuizUI()
        {
            pnlInputArea.Controls.Clear();

            // ===== HEADER SECTION =====
            Panel pnlHeaderSection = new Panel
            {
                Width = 720,
                Height = 100,
                Margin = new Padding(5, 10, 5, 20),
                BackColor = Color.FromArgb(245, 248, 250)
            };

            Label lblHeaderIcon = new Label
            {
                Text = "❓",
                Font = new Font("Segoe UI Emoji", 28F),
                Location = new Point(20, 15),
                AutoSize = true
            };

            Label lblHeaderTitle = new Label
            {
                Text = "Tạo Bộ Câu Hỏi Trắc Nghiệm",
                Font = new Font("Lexend", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 31, 137),
                Location = new Point(100, 12),
                AutoSize = true
            };

            Label lblHeaderDesc = new Label
            {
                Text = "💡 Mỗi câu hỏi có 4 đáp án (A, B, C, D) và 1 đáp án đúng",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                Location = new Point(100, 45),
                AutoSize = true
            };

            Label lblSubDesc = new Label
            {
                Text = "Gợi ý: Sử dụng câu hỏi ngắn gọn, rõ ràng và đáp án chính xác",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(100, 70),
                AutoSize = true
            };

            pnlHeaderSection.Controls.AddRange(new Control[] { lblHeaderIcon, lblHeaderTitle, lblHeaderDesc, lblSubDesc });
            pnlInputArea.Controls.Add(pnlHeaderSection);

            // Load existing questions
            foreach (var q in GameDataManager.GetQuizQuestions(_maMNG))
                AddQuizQuestionControl(q);

            var btnAdd = new RoundedButton
            {
                Text = "+ Thêm câu hỏi",
                Width = 700,
                Height = 50,
                Margin = new Padding(5, 15, 5, 40),
                BackColor = kahootRed,
                ForeColor = Color.White,
                Font = new Font("Lexend", 13F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                CornerRadius = 12,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => AddQuizQuestionControl(null);
            pnlInputArea.Controls.Add(btnAdd);
        }

        private void AddQuizQuestionControl(QuizQuestion data)
        {
            var qc = new QuizQuestionControl();
            if (data != null) qc.SetData(data);
            qc.BtnRemove.Click += (s, e) => pnlInputArea.Controls.Remove(qc);

            // Thêm vào cuối trước
            pnlInputArea.Controls.Add(qc);

            // Tìm vị trí của nút "Thêm câu hỏi"
            int btnAddIndex = -1;
            for (int i = 0; i < pnlInputArea.Controls.Count; i++)
            {
                if (pnlInputArea.Controls[i] is RoundedButton btn && btn.Text.Contains("Thêm câu hỏi"))
                {
                    btnAddIndex = i;
                    break;
                }
            }

            // Di chuyển lên trước nút "Thêm câu hỏi"
            if (btnAddIndex >= 0)
            {
                pnlInputArea.Controls.SetChildIndex(qc, btnAddIndex);
            }
        }
        private void BuildWordScrambleUI()
        {
            pnlInputArea.Controls.Clear();
            pnlInputArea.AutoScroll = true;

            // ===== HEADER SECTION =====
            Panel pnlHeaderSection = new Panel
            {
                Width = 720,
                Height = 100,
                Margin = new Padding(5, 10, 5, 20),
                BackColor = Color.FromArgb(245, 248, 250)
            };

            Label lblHeaderIcon = new Label
            {
                Text = "🧩",
                Font = new Font("Segoe UI Emoji", 28F),
                Location = new Point(20, 15),
                AutoSize = true
            };

            Label lblHeaderTitle = new Label
            {
                Text = "Tạo Bộ Từ Vựng Ghép Chữ",
                Font = new Font("Lexend", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 31, 137),
                Location = new Point(100, 12),
                AutoSize = true
            };

            Label lblHeaderDesc = new Label
            {
                Text = "💡 Mỗi từ vựng gồm: Hình ảnh gợi ý, Câu hỏi và Đáp án đúng",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                Location = new Point(100, 45),
                AutoSize = true
            };

            Label lblSubDesc = new Label
            {
                Text = "Gợi ý: Sử dụng hình ảnh rõ nét và đáp án viết HOA không dấu",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(100, 70),
                AutoSize = true
            };

            pnlHeaderSection.Controls.AddRange(new Control[] { lblHeaderIcon, lblHeaderTitle, lblHeaderDesc, lblSubDesc });
            pnlInputArea.Controls.Add(pnlHeaderSection);

            foreach (var item in GameDataManager.GetWordScrambleItems(_maMNG))
                AddWordScrambleControl(item);

            var btnAdd = new RoundedButton
            {
                Text = "➕  Thêm từ vựng mới",
                Width = 720,
                Height = 55,
                BackColor = Color.FromArgb(226, 27, 60),
                ForeColor = Color.White,
                Font = new Font("Lexend", 13F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                CornerRadius = 12,
                Margin = new Padding(5, 15, 5, 40),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.MouseEnter += (s, e) => btnAdd.BackColor = Color.FromArgb(206, 17, 50);
            btnAdd.MouseLeave += (s, e) => btnAdd.BackColor = Color.FromArgb(226, 27, 60);
            btnAdd.Click += (s, e) => AddWordScrambleControl(null);
            pnlInputArea.Controls.Add(btnAdd);
        }

        private void AddWordScrambleControl(WordScrambleItem data)
        {
            var wc = new WordScrambleControl();
            if (data != null) wc.SetData(data);
            wc.BtnRemove.Click += (s, e) => pnlInputArea.Controls.Remove(wc);

            // Thêm vào cuối trước
            pnlInputArea.Controls.Add(wc);

            // Tìm vị trí của nút "Thêm từ vựng mới"
            int btnAddIndex = -1;
            for (int i = 0; i < pnlInputArea.Controls.Count; i++)
            {
                if (pnlInputArea.Controls[i] is RoundedButton btn && btn.Text.Contains("Thêm từ vựng"))
                {
                    btnAddIndex = i;
                    break;
                }
            }

            // Di chuyển lên trước nút "Thêm từ vựng mới"
            if (btnAddIndex >= 0)
            {
                pnlInputArea.Controls.SetChildIndex(wc, btnAddIndex);
            }
        }
        private void BuildFlashcardUI()
        {
            pnlInputArea.Controls.Clear();
            // ===== 1. HEADER SECTION (Di chuyển lên đầu) =====
            Panel pnlHeaderSection = new Panel
            {
                Width = 720,
                Height = 100,
                Margin = new Padding(5, 10, 5, 20),
                BackColor = Color.FromArgb(245, 248, 250)
            };

            Label lblHeaderIcon = new Label
            {
                Text = "📚",
                Font = new Font("Segoe UI Emoji", 28F),
                Location = new Point(20, 15),
                AutoSize = true
            };

            Label lblHeaderTitle = new Label
            {
                Text = "Tạo Bộ Flashcard",
                Font = new Font("Lexend", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 31, 137),
                Location = new Point(100, 12),
                AutoSize = true
            };

            // Gợi ý được đặt SONG SONG với tiêu đề (DÒNG TRÊN)
            Label lblHeaderDesc = new Label
            {
                Text = "💡 Mỗi thẻ gồm 2 mặt: Thuật ngữ (Mặt trước) và Định nghĩa (Mặt sau)",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                Location = new Point(100, 45),
                AutoSize = true
            };

            Label lblSubDesc = new Label
            {
                Text = "Gợi ý: Sử dụng từ/cụm từ ngắn gọn để dễ ghi nhớ",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(100, 70),
                AutoSize = true
            };

            pnlHeaderSection.Controls.AddRange(new Control[] { lblHeaderIcon, lblHeaderTitle, lblHeaderDesc, lblSubDesc });
            pnlInputArea.Controls.Add(pnlHeaderSection);

            // ===== 2. LOAD EXISTING CARDS OR CREATE SAMPLES =====
            var items = GameDataManager.GetFlashcardItems(_maMNG);

            if (items.Count == 0)
            {
                // Tạo 2 thẻ mẫu
                AddFlashcardCard(new FlashcardItem { Term = "con mèo", Definition = "cat" });
                AddFlashcardCard(new FlashcardItem { Term = "con chó", Definition = "dog" });
            }
            else
            {
                foreach (var item in items)
                {
                    AddFlashcardCard(item);
                }
            }

            // ===== 3. ADD NEW CARD BUTTON =====
            RoundedButton btnAdd = new RoundedButton
            {
                Text = "➕  Thêm thẻ mới",
                Width = 720,
                Height = 55,
                BackColor = Color.FromArgb(226, 27, 60),
                ForeColor = Color.White,
                Font = new Font("Lexend", 13F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                CornerRadius = 12,
                Margin = new Padding(5, 15, 5, 40),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => AddFlashcardCard(null);
            pnlInputArea.Controls.Add(btnAdd);
        }

        private void AddFlashcardCard(FlashcardItem item)
        {
            // ===== MAIN CARD CONTAINER (GIẢM CHIỀU CAO ĐỂ KHÔNG DƯ TRỐNG) =====
            Panel cardContainer = new Panel
            {
                Width = 720,
                Height = 100,
                Margin = new Padding(5, 8, 5, 8),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Tag = "FLASHCARD_PANEL"
            };

            // Vẽ border và shadow cho card
            cardContainer.Paint += (s, e) =>
            {
                Rectangle rect = new Rectangle(0, 0, cardContainer.Width - 1, cardContainer.Height - 1);

                using (Pen shadowPen = new Pen(Color.FromArgb(30, 0, 0, 0), 3))
                {
                    e.Graphics.DrawRectangle(shadowPen, 2, 2, rect.Width - 2, rect.Height - 2);
                }

                using (Pen borderPen = new Pen(Color.FromArgb(220, 220, 220), 2))
                {
                    e.Graphics.DrawRectangle(borderPen, rect);
                }
            };

            // ===== CARD HEADER =====
            Panel pnlCardHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(720, 38),
                BackColor = Color.FromArgb(245, 248, 250)
            };

            Label lblCardIcon = new Label
            {
                Text = "🎴",
                Font = new Font("Segoe UI Emoji", 14F),
                Location = new Point(14, 6),
                AutoSize = true
            };

            Label lblCardTitle = new Label
            {
                Text = "Thẻ Flashcard",
                Font = new Font("Lexend", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(50, 10),
                AutoSize = true
            };

            Button btnRemove = new Button
            {
                Text = "🗑️ Xóa",
                Location = new Point(630, 5),
                Size = new Size(80, 28),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Lexend", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.Click += (s, e) =>
            {
                if (MessageBox.Show("Xóa thẻ này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    pnlInputArea.Controls.Remove(cardContainer);
                }
            };

            pnlCardHeader.Controls.AddRange(new Control[] { lblCardIcon, lblCardTitle, btnRemove });

            // ===== MẶT TRƯỚC (TERM) =====
            Label lblFrontLabel = new Label
            {
                Text = "📌 Mặt trước (Thuật ngữ)",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(87, 187, 247),
                Location = new Point(20, 48),
                AutoSize = true
            };

            TextBox txtTerm = new TextBox
            {
                Location = new Point(20, 68),
                Width = 300,
                Height = 35,
                Font = new Font("Lexend", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(64, 64, 64),
                Tag = "TERM"
            };
            PlaceholderProvider.SetPlaceholder(txtTerm, "Ví dụ: con mèo");

            // ===== ICON SWAP =====
            Label lblSwap = new Label
            {
                Text = "⇄",
                Font = new Font("Segoe UI", 20F),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(335, 50),
                AutoSize = true
            };

            // ===== MẶT SAU (DEFINITION) =====
            Label lblBackLabel = new Label
            {
                Text = "📝 Mặt sau (Định nghĩa)",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                Location = new Point(390, 48),
                AutoSize = true
            };

            TextBox txtDefinition = new TextBox
            {
                Location = new Point(390, 68),
                Width = 310,
                Height = 35,
                Font = new Font("Lexend", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(64, 64, 64),
                Tag = "DEFINITION"
            };
            PlaceholderProvider.SetPlaceholder(txtDefinition, "Ví dụ: cat");

            // ===== SET DATA IF EXISTS =====
            if (item != null)
            {
                txtTerm.Text = item.Term;
                txtTerm.ForeColor = Color.Black;
                txtDefinition.Text = item.Definition;
                txtDefinition.ForeColor = Color.Black;
            }

            // ===== ADD ALL CONTROLS TO CARD =====
            cardContainer.Controls.AddRange(new Control[]
            {
                pnlCardHeader,
                lblFrontLabel, txtTerm,
                lblSwap,
                lblBackLabel, txtDefinition
            });

            // ===== ADD CARD TO INPUT AREA =====
            int buttonIndex = -1;
            for (int i = pnlInputArea.Controls.Count - 1; i >= 0; i--)
            {
                if (pnlInputArea.Controls[i] is RoundedButton)
                {
                    buttonIndex = i;
                    break;
                }
            }

            if (buttonIndex >= 0)
            {
                // Thêm thẻ vào cuối
                pnlInputArea.Controls.Add(cardContainer);
                // Di chuyển thẻ lên trước nút "Thêm thẻ mới"
                pnlInputArea.Controls.SetChildIndex(cardContainer, buttonIndex);
            }
            else
            {
                // Nếu không tìm thấy nút, thêm vào cuối
                pnlInputArea.Controls.Add(cardContainer);
            }
        }
        private void BuildListUI() { var items = GameDataManager.GetListFromString(_maMNG); var rtb = new RichTextBox { Width = 650, Height = 400, Font = this.Font, Text = string.Join("\n", items) }; var lblGuide = new Label { Text = "Nhập mỗi mục trên một dòng.", Width = 650, AutoSize = true, ForeColor = Color.Gray }; pnlInputArea.Controls.AddRange(new Control[] { lblGuide, rtb }); }
        private void BuildMinMaxUI() { var list = GameDataManager.GetListFromString(_maMNG); var pnl = new Panel { Width = 400, Height = 50 }; var numMin = new NumericUpDown { Minimum = 0, Maximum = 9999, Width = 150, Dock = DockStyle.Left, Font = this.Font, Value = list.Count > 0 ? int.Parse(list[0]) : 1 }; var numMax = new NumericUpDown { Minimum = 1, Maximum = 10000, Width = 150, Dock = DockStyle.Right, Font = this.Font, Value = list.Count > 1 ? int.Parse(list[1]) : 100 }; pnl.Controls.AddRange(new Control[] { numMin, numMax }); pnlInputArea.Controls.Add(pnl); }
        private void BuildFillBlankUI()
        {
            pnlInputArea.Controls.Clear();

            // ===== HEADER SECTION (GIỐNG FLASHCARD) =====
            Panel pnlHeaderSection = new Panel
            {
                Width = 720,
                Height = 100,
                Margin = new Padding(5, 10, 5, 20),
                BackColor = Color.FromArgb(245, 248, 250)
            };

            Label lblHeaderIcon = new Label
            {
                Text = "✍️",
                Font = new Font("Segoe UI Emoji", 28F),
                Location = new Point(20, 15),
                AutoSize = true
            };

            Label lblHeaderTitle = new Label
            {
                Text = "Tạo Câu Hỏi Điền Từ",
                Font = new Font("Lexend", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 31, 137),
                Location = new Point(100, 12),
                AutoSize = true
            };

            // GỢI Ý GỘP CHUNG VỚI HEADER
            Label lblHeaderDesc = new Label
            {
                Text = "💡 Sử dụng ___ để đánh dấu chỗ trống trong câu hỏi",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                Location = new Point(100, 45),
                AutoSize = true
            };

            Label lblSubDesc = new Label
            {
                Text = "Ví dụ: Con ___ là loài vật quý hiếm → Đáp án: gấu trúc",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(100, 70),
                AutoSize = true
            };

            pnlHeaderSection.Controls.AddRange(new Control[] { lblHeaderIcon, lblHeaderTitle, lblHeaderDesc, lblSubDesc });
            pnlInputArea.Controls.Add(pnlHeaderSection);

            // Load các câu đã có
            var questions = GameDataManager.GetFillBlankQuestions(_maMNG);

            // Nếu không có câu nào, thêm 1 câu mẫu
            if (questions.Count == 0)
            {
                AddFillBlankControl(null);
            }
            else
            {
                foreach (var q in questions)
                {
                    AddFillBlankControl(q);
                }
            }

            // NÚT THÊM CÂU HỎI
            var btnAdd = new RoundedButton
            {
                Text = "+ Thêm câu hỏi",
                Width = 720,
                Height = 50,
                BackColor = Color.FromArgb(226, 27, 60),
                ForeColor = Color.White,
                Font = new Font("Lexend", 13F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                CornerRadius = 10,
                Margin = new Padding(5, 20, 5, 30),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => AddFillBlankControl(null);
            pnlInputArea.Controls.Add(btnAdd);
        }

        private void AddFillBlankControl(FillBlankQuestion data)
        {
            var fbc = new FillBlankControl();
            if (data != null) fbc.SetData(data);
            fbc.BtnRemove.Click += (s, e) => pnlInputArea.Controls.Remove(fbc);

            // Thêm vào cuối trước
            pnlInputArea.Controls.Add(fbc);

            // Tìm vị trí của nút "Thêm câu hỏi"
            int btnAddIndex = -1;
            for (int i = 0; i < pnlInputArea.Controls.Count; i++)
            {
                if (pnlInputArea.Controls[i] is RoundedButton btn && btn.Text.Contains("Thêm câu hỏi"))
                {
                    btnAddIndex = i;
                    break;
                }
            }

            // Di chuyển lên trước nút "Thêm câu hỏi"
            if (btnAddIndex >= 0)
            {
                pnlInputArea.Controls.SetChildIndex(fbc, btnAddIndex);
            }
        }
        private void BuildSentenceScrambleUI()
        {
            pnlInputArea.AutoScroll = true;
            pnlInputArea.Padding = new Padding(20);

            // ===== HEADER CARD (XÓA PHẦN GỢI Ý, ĐỔI ICON) =====
            Panel pnlHeader = new Panel
            {
                Width = 720,
                Height = 100,
                Margin = new Padding(0, 0, 0, 20),
                BackColor = Color.FromArgb(245, 248, 250)
            };

            pnlHeader.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(200, 210, 220), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlHeader.Width - 1, pnlHeader.Height - 1);
                }
            };

            Label lblIcon = new Label
            {
                Text = "📚",
                Font = new Font("Segoe UI Emoji", 22F),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Tiêu đề
            Label lblTitle = new Label
            {
                Text = "Tạo Câu Mẫu Sắp Xếp",
                Font = new Font("Lexend", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 31, 137),
                Location = new Point(90, 25),
                AutoSize = true
            };

            // Số câu đã nhập (SỬA LẠI NAME ĐỂ HOẠT ĐỘNG)
            Label lblCount = new Label
            {
                Name = "COUNT_LABEL",
                Text = "📊 Đã nhập: 0 câu",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Location = new Point(90, 60),
                AutoSize = true
            };

            pnlHeader.Controls.AddRange(new Control[] { lblIcon, lblTitle, lblCount });
            pnlInputArea.Controls.Add(pnlHeader);

            // ===== INPUT CARD =====
            Panel pnlInputCard = new Panel
            {
                Width = 720,
                Height = 450,
                Margin = new Padding(0, 0, 0, 20),
                BackColor = Color.White
            };

            pnlInputCard.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(200, 210, 220), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlInputCard.Width - 1, pnlInputCard.Height - 1);
                }
            };

            // Label "Danh sách câu"
            Label lblListTitle = new Label
            {
                Text = "📝 Danh sách câu mẫu:",
                Font = new Font("Lexend", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(20, 15),
                AutoSize = true
            };

            // Tải các câu đã có
            var items = GameDataManager.GetSentenceScrambleItems(_maMNG);
            var existingText = string.Join("\n", items.Select((item, index) => $"{index + 1}. {item.CorrectSentence}"));

            // RichTextBox với SCROLL HOẠT ĐỘNG
            var txtSentences = new RichTextBox
            {
                Name = "SENTENCES_INPUT",
                Location = new Point(20, 45),
                Size = new Size(680, 350),
                Font = new Font("Consolas", 11F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                Text = existingText,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                WordWrap = true
            };

            // Placeholder
            bool isPlaceholder = false;
            if (string.IsNullOrEmpty(existingText))
            {
                txtSentences.Text = "Ví dụ:\n1. She likes to read books.";
                txtSentences.ForeColor = Color.Gray;
                isPlaceholder = true;

                txtSentences.Enter += (s, e) =>
                {
                    if (txtSentences.ForeColor == Color.Gray)
                    {
                        txtSentences.Text = "1. ";
                        txtSentences.ForeColor = Color.Black;
                        txtSentences.SelectionStart = txtSentences.Text.Length;
                    }
                };

                txtSentences.Leave += (s, e) =>
                {
                    var lines = txtSentences.Lines.Where(line => !string.IsNullOrWhiteSpace(line)
                        && !line.Trim().StartsWith("Ví dụ:")).ToList();

                    if (lines.Count == 0)
                    {
                        txtSentences.Text = "Ví dụ:\n1. She likes to read books.";
                        txtSentences.ForeColor = Color.Gray;
                    }
                };
            }
            else
            {
                txtSentences.ForeColor = Color.Black;
            }

            // ĐÁNH SỐ TỰ ĐỘNG KHI NHẬP
            txtSentences.TextChanged += (s, e) =>
            {
                if (txtSentences.ForeColor == Color.Gray) return;

                // Đếm số câu (SỬA LẠI CÁCH TÌM CONTROL)
                var lines = txtSentences.Lines
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Where(line => !line.Trim().StartsWith("Ví dụ:"))
                    .ToList();

                // Tìm label đếm theo Name thay vì Find
                var countLabel = pnlHeader.Controls.Cast<Control>()
                    .FirstOrDefault(c => c.Name == "COUNT_LABEL") as Label;

                if (countLabel != null)
                {
                    countLabel.Text = $"📊 Đã nhập: {lines.Count} câu";
                    countLabel.ForeColor = lines.Count > 0 ? Color.FromArgb(40, 167, 69) : Color.FromArgb(0, 123, 255);
                }

                // Cập nhật số ký tự
                var charLabel = pnlInputCard.Controls.Cast<Control>()
                    .FirstOrDefault(c => c.Name == "CHAR_COUNT") as Label;

                if (charLabel != null)
                {
                    charLabel.Text = $"Tổng số ký tự: {txtSentences.Text.Length}";
                }
            };

            // Tự động đánh số khi Enter
            txtSentences.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && txtSentences.ForeColor != Color.Gray)
                {
                    e.SuppressKeyPress = true;
                    int currentLineCount = txtSentences.Lines
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .Count();

                    // Chèn số thứ tự cho dòng mới
                    txtSentences.AppendText(Environment.NewLine + $"{currentLineCount + 1}. ");
                }
            };

            // Hiển thị số câu ban đầu
            if (!string.IsNullOrEmpty(existingText))
            {
                var initialCount = items.Count;
                var countLabel = pnlHeader.Controls.Cast<Control>()
                    .FirstOrDefault(c => c.Name == "COUNT_LABEL") as Label;

                if (countLabel != null)
                {
                    countLabel.Text = $"📊 Đã nhập: {initialCount} câu";
                    countLabel.ForeColor = Color.FromArgb(40, 167, 69);
                }
            }

            // Label số ký tự
            Label lblCharCount = new Label
            {
                Name = "CHAR_COUNT",
                Text = $"Tổng số ký tự: {txtSentences.Text.Length}",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 405),
                AutoSize = true
            };

            pnlInputCard.Controls.AddRange(new Control[] { lblListTitle, txtSentences, lblCharCount });
            pnlInputArea.Controls.Add(pnlInputCard);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                switch (_maMNG)
                {
                    case "MNG01": GameDataManager.SaveQuizQuestions(_maMNG, pnlInputArea.Controls.OfType<QuizQuestionControl>().Select(qc => qc.GetData()).ToList()); break;
                    case "MNG04": GameDataManager.SaveWordScrambleItems(_maMNG, pnlInputArea.Controls.OfType<WordScrambleControl>().Select(wc => wc.GetData()).ToList()); break;
                    case "MNG03":
                        var flashcardData = pnlInputArea.Controls
                            .OfType<Panel>()
                            .Where(p => p.Tag != null && p.Tag.ToString() == "FLASHCARD_PANEL")
                            .Select(panel =>
                            {
                                var txtTerm = panel.Controls.OfType<TextBox>().FirstOrDefault(t => t.Tag?.ToString() == "TERM");
                                var txtDef = panel.Controls.OfType<TextBox>().FirstOrDefault(t => t.Tag?.ToString() == "DEFINITION");

                                return new FlashcardItem
                                {
                                    Term = txtTerm?.Text ?? "",
                                    Definition = txtDef?.Text ?? ""
                                };
                            })
                            .Where(f => !string.IsNullOrWhiteSpace(f.Term) && !string.IsNullOrWhiteSpace(f.Definition))
                            .ToList();

                        GameDataManager.SaveFlashcardItems(_maMNG, flashcardData);
                        break;
                    case "MNG02": case "MNG08": GameDataManager.SaveListToString(_maMNG, pnlInputArea.Controls.OfType<RichTextBox>().First().Text.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim()).ToList()); break;
                    case "MNG09": var panelMinMax = pnlInputArea.Controls.OfType<Panel>().First(); GameDataManager.SaveListToString(_maMNG, new List<string> { panelMinMax.Controls.OfType<NumericUpDown>().First().Value.ToString(), panelMinMax.Controls.OfType<NumericUpDown>().Last().Value.ToString() }); break;
                    case "MNG06":
                        // Tìm RichTextBox theo Name thay vì OfType (vì nó nằm trong Panel con)
                        var txtSentences = FindControlByName(pnlInputArea, "SENTENCES_INPUT") as RichTextBox;

                        if (txtSentences == null)
                        {
                            MessageBox.Show("Không tìm thấy ô nhập câu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var sentenceText = txtSentences.Text ?? "";
                        var sentences = sentenceText.Split('\n')
                                                    .Where(line => !string.IsNullOrWhiteSpace(line)
                                                                && !line.Trim().StartsWith("Ví dụ:")
                                                                && !line.Trim().StartsWith("Nhập mỗi câu"))
                                                    .Select(line => {
                                                        // Loại bỏ số thứ tự "1. 2. 3." ở đầu câu
                                                        string cleaned = System.Text.RegularExpressions.Regex.Replace(
                                                            line.Trim(),
                                                            @"^\d+\.\s*",
                                                            ""
                                                        );
                                                        return new SentenceScrambleItem { CorrectSentence = cleaned };
                                                    })
                                                    .Where(item => !string.IsNullOrWhiteSpace(item.CorrectSentence))
                                                    .ToList();

                        if (sentences.Count == 0)
                        {
                            MessageBox.Show("Chưa nhập câu nào! Hãy nhập ít nhất 1 câu.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        GameDataManager.SaveSentenceScrambleItems(_maMNG, sentences);
                        break;
                    case "MNG07":
                        var fillBlankQuestions = pnlInputArea.Controls.OfType<FillBlankControl>()
                            .Select(fbc => fbc.GetData())
                            .Where(q => !string.IsNullOrWhiteSpace(q.QuestionText))
                            .ToList();
                        GameDataManager.SaveFillBlankQuestions(_maMNG, fillBlankQuestions);
                        break;
                }
                MessageBox.Show("Lưu dữ liệu thành công!", "Thành công");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi khi lưu dữ liệu. Vui lòng kiểm tra lại định dạng đã nhập.\nChi tiết: " + ex.Message, "Lỗi"); }
        }
        // Helper method để tìm control theo Name (đệ quy vào các Panel con)
        private Control FindControlByName(Control parent, string name)
        {
            if (parent.Name == name)
                return parent;

            foreach (Control child in parent.Controls)
            {
                Control found = FindControlByName(child, name);
                if (found != null)
                    return found;
            }

            return null;
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
                    case "MNG06": gameForm = new SapXepCauForm(GameDataManager.GetSentenceScrambleItems(_maMNG)); break;
                    case "MNG07":
                        var fillBlankQs = GameDataManager.GetFillBlankQuestions(_maMNG);
                        if (fillBlankQs.Count == 0) fillBlankQs.Add(GameDataManager.GetFillBlankQuestion(_maMNG));
                        gameForm = new DienTuForm(fillBlankQs);
                        break;
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
        private Panel pnlQuestionCard;

        public QuizGameForm(List<QuizQuestion> questions)
        {
            if (questions == null || questions.Count == 0) { CloseWithWarning(); return; }
            _questions = questions;
            InitializeComponent();
            LoadQuestion();
        }

        private void InitializeComponent()
        {
            this.Text = "📝 Quiz Vui Vẻ";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 247, 255);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White
            };

            pnlHeader.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    pnlHeader.ClientRectangle,
                    Color.FromArgb(87, 187, 247),
                    Color.FromArgb(19, 104, 206),
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, pnlHeader.ClientRectangle);
                }
            };

            Label lblIcon = new Label
            {
                Text = "📝",
                Font = new Font("Segoe UI", 22F),
                ForeColor = Color.White,
                Location = new Point(30, 10),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label lblGameTitle = new Label
            {
                Text = "Quiz Vui Vẻ",
                Font = new Font("Lexend", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(80, 12),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // ### THÊM SỐ CÂU ĐÚNG ###
            lblQuestionCount = new Label
            {
                Location = new Point(80, 42),
                AutoSize = true,
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 215, 0),
                BackColor = Color.Transparent
            };

            lblScore = new Label
            {
                Text = "Điểm: 0",
                Location = new Point(750, 25),
                AutoSize = true,
                Font = new Font("Lexend", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 215, 0),
                BackColor = Color.Transparent
            };

            pnlHeader.Controls.AddRange(new Control[] { lblIcon, lblGameTitle, lblQuestionCount, lblScore });

            pnlQuestionCard = new Panel
            {
                Location = new Point(50, 120),
                Size = new Size(800, 180),
                BackColor = Color.White
            };

            pnlQuestionCard.Paint += (s, e) =>
            {
                Rectangle rect = pnlQuestionCard.ClientRectangle;
                using (GraphicsPath path = GetRoundedRectangle(rect, 15))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (PathGradientBrush shadowBrush = new PathGradientBrush(path))
                    {
                        shadowBrush.CenterColor = Color.White;
                        shadowBrush.SurroundColors = new[] { Color.FromArgb(30, 0, 0, 0) };
                        e.Graphics.FillPath(shadowBrush, path);
                    }
                    using (SolidBrush brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    using (Pen pen = new Pen(Color.FromArgb(200, 220, 240), 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            lblQuestion = new Label
            {
                Location = new Point(30, 30),
                Size = new Size(740, 120),
                Font = new Font("Lexend", 18F, FontStyle.Regular),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            pnlQuestionCard.Controls.Add(lblQuestion);

            TableLayoutPanel tlp = new TableLayoutPanel
            {
                Location = new Point(50, 340),
                Size = new Size(800, 300),
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent
            };

            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            optionButtons = new List<RoundedButton>();
            string[] prefixes = { "A", "B", "C", "D" };
            Color[] colors = {
            Color.FromArgb(87, 187, 247),
            Color.FromArgb(255, 189, 89),
            Color.FromArgb(29, 209, 161),
            Color.FromArgb(255, 118, 117)
        };

            for (int i = 0; i < 4; i++)
            {
                var btn = new RoundedButton
                {
                    Font = new Font("Lexend", 16F, FontStyle.Bold),
                    Tag = prefixes[i],
                    CornerRadius = 15,
                    BackColor = colors[i],
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(15),
                    Cursor = Cursors.Hand
                };

                btn.FlatAppearance.BorderSize = 0;
                btn.MouseEnter += (s, e) =>
                {
                    var button = s as Button;
                    button.BackColor = ControlPaint.Light(button.BackColor, 0.1f);
                };

                btn.MouseLeave += (s, e) =>
                {
                    var button = s as Button;
                    if (!optionButtons.Any(ob => ob.Enabled == false))
                    {
                        button.BackColor = colors[optionButtons.IndexOf(button as RoundedButton)];
                    }
                };

                optionButtons.Add(btn);
                tlp.Controls.Add(btn, i % 2, i / 2);
            }

            this.Controls.AddRange(new Control[] { pnlHeader, pnlQuestionCard, tlp });
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
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

        private void LoadQuestion()
        {
            if (currentQuestionIndex < _questions.Count)
            {
                // ### CẬP NHẬT HIỂN THỊ SỐ CÂU ĐÚNG ###
                lblQuestionCount.Text = $" Câu {currentQuestionIndex + 1}/{_questions.Count} | Đúng: {score}";

                QuizQuestion q = _questions[currentQuestionIndex];
                lblQuestion.Text = q.QuestionText;

                Color[] colors = {
                Color.FromArgb(87, 187, 247),
                Color.FromArgb(255, 189, 89),
                Color.FromArgb(29, 209, 161),
                Color.FromArgb(255, 118, 117)
            };

                for (int i = 0; i < 4; i++)
                {
                    optionButtons[i].Text = $"{((char)('A' + i))}. {q.Options[i]}";
                    optionButtons[i].Click -= OptionButton_Click;
                    optionButtons[i].Click += OptionButton_Click;
                    optionButtons[i].BackColor = colors[i];
                    optionButtons[i].ForeColor = Color.White;
                    optionButtons[i].Enabled = true;
                }
            }
            else
            {
                EndGame();
            }
        }

        private async void OptionButton_Click(object sender, EventArgs e)
        {
            RoundedButton clickedButton = sender as RoundedButton;
            QuizQuestion q = _questions[currentQuestionIndex];

            optionButtons.ForEach(btn => btn.Enabled = false);

            string originalText = clickedButton.Text;
            Color[] originalColors = {
            Color.FromArgb(87, 187, 247),
            Color.FromArgb(255, 189, 89),
            Color.FromArgb(29, 209, 161),
            Color.FromArgb(255, 118, 117)
        };

            if (clickedButton.Tag.ToString() == q.CorrectAnswer.ToUpper())
            {
                score++;

                // ### CẬP NHẬT NGAY LẬP TỨC ###
                lblScore.Text = $"Điểm: {score}";
                lblQuestionCount.Text = $" Câu {currentQuestionIndex + 1}/{_questions.Count} | Đúng: {score}";

                for (int i = 0; i < optionButtons.Count; i++)
                {
                    if (optionButtons[i] != clickedButton)
                    {
                        optionButtons[i].BackColor = Color.FromArgb(180, originalColors[i]);
                        optionButtons[i].ForeColor = Color.FromArgb(150, 150, 150);
                    }
                }

                clickedButton.BackColor = Color.FromArgb(40, 167, 69);
                clickedButton.ForeColor = Color.White;
                clickedButton.Text = "✓ " + originalText;
                clickedButton.Font = new Font("Lexend", 18F, FontStyle.Bold);

                for (int i = 0; i < 3; i++)
                {
                    clickedButton.Font = new Font("Lexend", 20F, FontStyle.Bold);
                    await Task.Delay(150);
                    clickedButton.Font = new Font("Lexend", 18F, FontStyle.Bold);
                    await Task.Delay(150);
                }

                for (int i = 0; i < 3; i++)
                {
                    clickedButton.BackColor = Color.FromArgb(60, 220, 100);
                    await Task.Delay(200);
                    clickedButton.BackColor = Color.FromArgb(40, 167, 69);
                    await Task.Delay(200);
                }
            }
            else
            {
                var correctButton = optionButtons.First(btn => btn.Tag.ToString() == q.CorrectAnswer.ToUpper());

                for (int i = 0; i < optionButtons.Count; i++)
                {
                    if (optionButtons[i] != clickedButton && optionButtons[i] != correctButton)
                    {
                        optionButtons[i].BackColor = Color.FromArgb(180, originalColors[i]);
                        optionButtons[i].ForeColor = Color.FromArgb(150, 150, 150);
                    }
                }

                clickedButton.BackColor = Color.FromArgb(220, 53, 69);
                clickedButton.ForeColor = Color.White;
                clickedButton.Text = "✗ " + originalText;
                clickedButton.Font = new Font("Lexend", 18F, FontStyle.Bold);

                int clickedButtonIndex = optionButtons.IndexOf(clickedButton);
                Point originalLocation = new Point(
                    clickedButton.Margin.Left,
                    clickedButton.Margin.Top
                );

                for (int i = 0; i < 5; i++)
                {
                    clickedButton.Padding = new Padding(5, 0, 0, 0);
                    await Task.Delay(50);
                    clickedButton.Padding = new Padding(0, 0, 5, 0);
                    await Task.Delay(50);
                }
                clickedButton.Padding = new Padding(0);

                for (int i = 0; i < 2; i++)
                {
                    clickedButton.BackColor = Color.FromArgb(255, 100, 100);
                    await Task.Delay(200);
                    clickedButton.BackColor = Color.FromArgb(220, 53, 69);
                    await Task.Delay(200);
                }

                await Task.Delay(500);

                string correctOriginalText = correctButton.Text;
                correctButton.BackColor = Color.FromArgb(40, 167, 69);
                correctButton.ForeColor = Color.White;
                correctButton.Text = "✓ " + correctOriginalText;
                correctButton.Font = new Font("Lexend", 18F, FontStyle.Bold);

                for (int i = 0; i < 2; i++)
                {
                    correctButton.BackColor = Color.FromArgb(60, 220, 100);
                    await Task.Delay(200);
                    correctButton.BackColor = Color.FromArgb(40, 167, 69);
                    await Task.Delay(200);
                }
            }

            await Task.Delay(1500);
            currentQuestionIndex++;
            LoadQuestion();
        }

        // ### MÀN HÌNH HOÀN THÀNH GIỐNG ĐIỀN TỪ ###
        private void EndGame()
        {
            this.Controls.Clear();
            this.BackColor = Color.FromArgb(240, 247, 255);

            Panel pnlResult = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 247, 255),
                Padding = new Padding(50)
            };

            Label lblIcon = new Label
            {
                Text = "🎊",
                Dock = DockStyle.Top,
                Height = 80,
                Font = new Font("Segoe UI Emoji", 60F),
                ForeColor = Color.FromArgb(29, 209, 161),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblCongrats = new Label
            {
                Text = "Hoàn thành!",
                Dock = DockStyle.Top,
                Height = 80,
                Font = new Font("Lexend", 32F, FontStyle.Bold),
                ForeColor = Color.FromArgb(29, 209, 161),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblScoreResult = new Label
            {
                Text = $"Bạn đã trả lời đúng {score}/{_questions.Count} câu",
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font("Segoe UI", 20F),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            double percentage = (score * 100.0) / _questions.Count;
            string comment = percentage >= 80 ? "Xuất sắc! 🌟" :
                            percentage >= 60 ? "Tốt lắm! 👍" :
                            "Cố gắng thêm! 💪";

            Label lblComment = new Label
            {
                Text = comment,
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 18F, FontStyle.Italic),
                ForeColor = Color.FromArgb(255, 189, 89),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            RoundedButton btnClose = new RoundedButton
            {
                Text = "Đóng",
                Size = new Size(200, 60),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                BackColor = Color.FromArgb(87, 187, 247),
                ForeColor = Color.White,
                CornerRadius = 15,
                Location = new Point((this.ClientSize.Width - 200) / 2, 400)
            };
            btnClose.Click += (s, e) => this.Close();

            pnlResult.Controls.AddRange(new Control[] { lblComment, lblScoreResult, lblCongrats, lblIcon });
            this.Controls.Add(pnlResult);
            this.Controls.Add(btnClose);
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
        private Panel cardPanel, cardShadowPanel;
        private Label cardLabel, lblCardCount, lblHint, lblFrontIndicator, lblBackIndicator;
        private RoundedButton btnNext, btnPrev, btnFlip;
        private Panel pnlHeader;

        public FlashcardForm(List<FlashcardItem> cards)
        {
            if (cards == null || cards.Count == 0) { CloseWithWarning(); return; }
            _cards = cards;
            InitializeComponent();
            ShowCard();
        }

        private void InitializeComponent()
        {
            this.Text = "📇 Thẻ Ghi Nhớ Flashcard";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ===== PANEL HEADER VỚI GRADIENT =====
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.White
            };

            pnlHeader.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    pnlHeader.ClientRectangle,
                    Color.FromArgb(87, 187, 247),
                    Color.FromArgb(19, 104, 206),
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, pnlHeader.ClientRectangle);
                }
            };

            // Icon Flashcard
            Label lblIcon = new Label
            {
                Text = "📇",
                Font = new Font("Segoe UI Emoji", 22F),
                ForeColor = Color.White,
                Location = new Point(30, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Tiêu đề
            Label lblTitle = new Label
            {
                Text = "Thẻ Ghi Nhớ Flashcard",
                Font = new Font("Lexend", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(80, 12),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Số thẻ (tiến độ)
            lblCardCount = new Label
            {
                Location = new Point(80, 42),
                AutoSize = true,
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 215, 0),
                BackColor = Color.Transparent
            };

            pnlHeader.Controls.AddRange(new Control[] { lblIcon, lblTitle, lblCardCount });

            // ===== HINT TEXT =====
            lblHint = new Label
            {
                Text = "💡 Nhấn vào thẻ hoặc nút 'Lật thẻ' để xem mặt sau",
                Font = new Font("Segoe UI", 12F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 400) / 2, 120)
            };

            // ===== CARD SHADOW PANEL =====
            cardShadowPanel = new Panel
            {
                Size = new Size(660, 360),
                Location = new Point((this.ClientSize.Width - 660) / 2, 160),
                BackColor = Color.FromArgb(200, 200, 200)
            };
            cardShadowPanel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, 660, 360, 30, 30));

            // ===== CARD PANEL =====
            cardPanel = new Panel
            {
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                Size = new Size(650, 350),
                Location = new Point(5, 5)
            };
            cardPanel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, 650, 350, 25, 25));
            cardPanel.Click += (s, e) => FlipCard();

            // Paint card với border đẹp hơn
            cardPanel.Paint += (s, e) =>
            {
                Rectangle rect = cardPanel.ClientRectangle;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (Pen borderPen = new Pen(isFlipped ? Color.FromArgb(40, 167, 69) : Color.FromArgb(87, 187, 247), 4))
                {
                    using (GraphicsPath path = GetRoundedRectPath(rect, 25))
                    {
                        e.Graphics.DrawPath(borderPen, path);
                    }
                }
            };

            // Indicator - Mặt trước/sau
            lblFrontIndicator = new Label
            {
                Text = "📌 MẶT TRƯỚC",
                Font = new Font("Lexend", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(87, 187, 247),
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            lblBackIndicator = new Label
            {
                Text = "📝 MẶT SAU",
                Font = new Font("Lexend", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent,
                Visible = false
            };

            // Card content label
            cardLabel = new Label
            {
                Location = new Point(30, 80),
                Size = new Size(590, 230),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Lexend", 32F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            cardLabel.Click += (s, e) => FlipCard();

            cardPanel.Controls.AddRange(new Control[] { lblFrontIndicator, lblBackIndicator, cardLabel });
            cardShadowPanel.Controls.Add(cardPanel);

            // ===== NAVIGATION BUTTONS =====
            btnPrev = new RoundedButton
            {
                Text = "◀",
                Size = new Size(70, 70),
                Font = new Font("Segoe UI", 24F),
                CornerRadius = 35,
                BackColor = Color.FromArgb(87, 187, 247),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(80, (this.ClientSize.Height - 70) / 2),
                Cursor = Cursors.Hand
            };
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.Click += (s, e) => Navigate(-1);

            // Hover effect
            btnPrev.MouseEnter += (s, e) => btnPrev.BackColor = Color.FromArgb(67, 167, 227);
            btnPrev.MouseLeave += (s, e) => btnPrev.BackColor = btnPrev.Enabled ? Color.FromArgb(87, 187, 247) : Color.LightGray;

            btnNext = new RoundedButton
            {
                Text = "▶",
                Size = new Size(70, 70),
                Font = new Font("Segoe UI", 24F),
                CornerRadius = 35,
                BackColor = Color.FromArgb(87, 187, 247),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(this.ClientSize.Width - 150, (this.ClientSize.Height - 70) / 2),
                Cursor = Cursors.Hand
            };
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += (s, e) => Navigate(1);

            // Hover effect
            btnNext.MouseEnter += (s, e) => btnNext.BackColor = Color.FromArgb(67, 167, 227);
            btnNext.MouseLeave += (s, e) => btnNext.BackColor = btnNext.Enabled ? Color.FromArgb(87, 187, 247) : Color.LightGray;

            // ===== FLIP BUTTON =====
            btnFlip = new RoundedButton
            {
                Text = "🔄 Lật thẻ",
                Size = new Size(200, 60),
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                CornerRadius = 15,
                BackColor = Color.FromArgb(255, 189, 89),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point((this.ClientSize.Width - 200) / 2, 560),
                Cursor = Cursors.Hand
            };
            btnFlip.FlatAppearance.BorderSize = 0;
            btnFlip.Click += (s, e) => FlipCard();

            // Hover effect
            btnFlip.MouseEnter += (s, e) => btnFlip.BackColor = Color.FromArgb(235, 169, 69);
            btnFlip.MouseLeave += (s, e) => btnFlip.BackColor = Color.FromArgb(255, 189, 89);

            this.Controls.AddRange(new Control[] { pnlHeader, lblHint, cardShadowPanel, btnPrev, btnNext, btnFlip });
        }

        // Helper: Tạo rounded rect path
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

            // Thay đổi màu card
            cardPanel.BackColor = isFlipped ? Color.FromArgb(245, 255, 250) : Color.White;

            // Hiển thị indicator
            lblFrontIndicator.Visible = !isFlipped;
            lblBackIndicator.Visible = isFlipped;

            // Refresh để vẽ lại border
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

    // ===================================================================
    // GAME 4: GHÉP CHỮ (MNG04)
    // ===================================================================
    public class GheChuForm : GameFormWithMusic
    {
        private List<WordScrambleItem> _items;
        private int _currentItemIndex = 0;
        private int _correctAnswers = 0;
        private PictureBox picHint;
        private Label lblQuestion, lblStatus, lblProgress;
        private FlowLayoutPanel pnlAnswer, pnlChoices;
        private RoundedButton btnNext, btnReset;
        private Panel pnlButtons;

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
            this.Size = new Size(1024, 788);
            this.BackColor = Color.FromArgb(240, 247, 255);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White
            };

            pnlHeader.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    pnlHeader.ClientRectangle,
                    Color.FromArgb(87, 187, 247),
                    Color.FromArgb(19, 104, 206),
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, pnlHeader.ClientRectangle);
                }
            };

            Label lblIcon = new Label
            {
                Text = "🧩",
                Font = new Font("Segoe UI Emoji", 22F),
                ForeColor = Color.White,
                Location = new Point(30, 10),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label lblGameTitle = new Label
            {
                Text = "Ghép Chữ Đoán Hình",
                Font = new Font("Lexend", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(80, 12),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // ### THÊM SỐ CÂU ĐÚNG ###
            lblProgress = new Label
            {
                Location = new Point(80, 42),
                AutoSize = true,
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 215, 0),
                BackColor = Color.Transparent
            };

            pnlHeader.Controls.AddRange(new Control[] { lblIcon, lblGameTitle, lblProgress });

            Panel pnlMainCard = new Panel
            {
                Size = new Size(900, 560),
                Location = new Point(62, 85),
                BackColor = Color.White,
                Padding = new Padding(30)
            };

            pnlMainCard.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 5, 5, pnlMainCard.Width - 5, pnlMainCard.Height - 5);
                }
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, pnlMainCard.Width - 1, pnlMainCard.Height - 1), 20))
                {
                    using (var brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    using (var pen = new Pen(Color.FromArgb(220, 230, 240), 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            Panel pnlImage = new Panel
            {
                Location = new Point(300, 15),
                Size = new Size(300, 200),
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(5)
            };

            pnlImage.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(200, 210, 220), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlImage.Width - 1, pnlImage.Height - 1);
                }
            };

            picHint = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            pnlImage.Controls.Add(picHint);

            Panel pnlQuestionBg = new Panel
            {
                Location = new Point(30, 225),
                Size = new Size(840, 50),
                BackColor = Color.FromArgb(240, 247, 255),
                Padding = new Padding(10, 8, 10, 8)
            };

            pnlQuestionBg.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(200, 220, 240), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlQuestionBg.Width - 1, pnlQuestionBg.Height - 1);
                }
            };

            lblQuestion = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            pnlQuestionBg.Controls.Add(lblQuestion);

            Label lblAnswerTitle = new Label
            {
                Text = "Đáp án của bạn:",
                Font = new Font("Lexend", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(87, 187, 247),
                Location = new Point(30, 290),
                AutoSize = true
            };

            pnlAnswer = new FlowLayoutPanel
            {
                Size = new Size(840, 80),
                Location = new Point(30, 320),
                BackColor = Color.White,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.None,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = false
            };

            pnlAnswer.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(87, 187, 247), 3))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlAnswer.Width - 1, pnlAnswer.Height - 1);
                }
            };

            Label lblChoicesTitle = new Label
            {
                Text = "Chọn các chữ cái:",
                Font = new Font("Lexend", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(108, 117, 125),
                Location = new Point(30, 405),
                AutoSize = true
            };

            pnlChoices = new FlowLayoutPanel
            {
                Size = new Size(840, 90),
                Location = new Point(30, 430),
                Padding = new Padding(10),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.None,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            pnlChoices.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(200, 210, 220), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlChoices.Width - 1, pnlChoices.Height - 1);
                }
            };

            pnlMainCard.Controls.AddRange(new Control[]
            {
            pnlImage,
            pnlQuestionBg,
            lblAnswerTitle,
            pnlAnswer,
            lblChoicesTitle,
            pnlChoices
            });

            Panel pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            btnReset = new RoundedButton
            {
                Text = "🔄 Làm lại",
                Size = new Size(140, 50),
                Location = new Point(100, 15),
                Font = new Font("Lexend", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                CornerRadius = 12
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += BtnReset_Click;
            btnReset.MouseEnter += (s, e) => btnReset.BackColor = Color.FromArgb(88, 97, 105);
            btnReset.MouseLeave += (s, e) => btnReset.BackColor = Color.FromArgb(108, 117, 125);

            lblStatus = new Label
            {
                Location = new Point(350, 20),
                Size = new Size(324, 40),
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            btnNext = new RoundedButton
            {
                Text = "➡️ Tiếp theo",
                Size = new Size(160, 50),
                Location = new Point(764, 15),
                Font = new Font("Lexend", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(29, 209, 161),
                ForeColor = Color.White,
                CornerRadius = 12,
                Visible = false
            };
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += BtnNext_Click;
            btnNext.MouseEnter += (s, e) => btnNext.BackColor = Color.FromArgb(20, 189, 141);
            btnNext.MouseLeave += (s, e) => btnNext.BackColor = Color.FromArgb(29, 209, 161);

            pnlButtons.Controls.AddRange(new Control[] { btnReset, lblStatus, btnNext });

            this.Controls.Add(pnlMainCard);
            this.Controls.Add(pnlButtons);
            this.Controls.Add(pnlHeader);
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

        private void LoadCurrentItem()
        {
            pnlAnswer.Controls.Clear();
            pnlChoices.Controls.Clear();
            lblStatus.Visible = false;
            btnNext.Visible = false;
            pnlChoices.Enabled = true;
            pnlAnswer.Enabled = true;

            if (_currentItemIndex >= _items.Count)
            {
                EndGame();
                return;
            }

            // ### CẬP NHẬT SỐ CÂU ĐÚNG ###
            lblProgress.Text = $" Câu {_currentItemIndex + 1}/{_items.Count} | Đúng: {_correctAnswers}";

            WordScrambleItem current = _items[_currentItemIndex];

            if (picHint.Image != null)
            {
                picHint.Image.Dispose();
                picHint.Image = null;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(current.ImageHintResourceName))
                {
                    if (File.Exists(current.ImageHintResourceName))
                    {
                        using (var fs = new FileStream(current.ImageHintResourceName, FileMode.Open, FileAccess.Read))
                        {
                            using (var ms = new MemoryStream())
                            {
                                fs.CopyTo(ms);
                                ms.Position = 0;
                                picHint.Image = Image.FromStream(ms);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            var resourceImage = Properties.Resources.ResourceManager.GetObject(current.ImageHintResourceName);
                            if (resourceImage != null)
                            {
                                picHint.Image = (Image)resourceImage;
                            }
                            else
                            {
                                picHint.Image = Properties.Resources.placeholder;
                            }
                        }
                        catch
                        {
                            picHint.Image = Properties.Resources.placeholder;
                        }
                    }
                }
                else
                {
                    picHint.Image = Properties.Resources.placeholder;
                }
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show(
                    "Ảnh gợi ý bị lỗi hoặc quá lớn.\nSử dụng ảnh placeholder thay thế.",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                try { picHint.Image = Properties.Resources.placeholder; }
                catch { picHint.Image = null; }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { picHint.Image = Properties.Resources.placeholder; }
                catch { picHint.Image = null; }
            }

            lblQuestion.Text = current.Question;
            lblQuestion.Left = (this.ClientSize.Width - lblQuestion.Width) / 2;

            string[] words = current.Answer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            int totalWidth = 0;
            int slotWidth = 55;
            int slotMargin = 3;
            int spacerWidth = 15;

            foreach (string word in words)
            {
                totalWidth += word.Length * (slotWidth + slotMargin * 2);
            }
            if (words.Length > 1)
            {
                totalWidth += (words.Length - 1) * spacerWidth;
            }

            int availableWidth = pnlAnswer.Width - 10;
            int leftPadding = Math.Max(5, (availableWidth - totalWidth) / 2);

            pnlAnswer.SuspendLayout();

            if (leftPadding > 5)
            {
                var leftSpacer = new Label
                {
                    Text = "",
                    Size = new Size(leftPadding, 55),
                    Margin = new Padding(0),
                    BorderStyle = BorderStyle.None,
                    BackColor = Color.Transparent,
                    Tag = "SPACER"
                };
                pnlAnswer.Controls.Add(leftSpacer);
            }

            for (int wordIndex = 0; wordIndex < words.Length; wordIndex++)
            {
                string word = words[wordIndex];

                for (int i = 0; i < word.Length; i++)
                {
                    var slot = new Label
                    {
                        Text = "",
                        Size = new Size(slotWidth, 55),
                        Margin = new Padding(slotMargin),
                        Font = new Font("Lexend", 24F, FontStyle.Bold),
                        BorderStyle = BorderStyle.FixedSingle,
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.White,
                        Cursor = Cursors.Hand
                    };
                    slot.Click += Answer_Click;
                    pnlAnswer.Controls.Add(slot);
                }

                if (wordIndex < words.Length - 1)
                {
                    var spacer = new Label
                    {
                        Text = "",
                        Size = new Size(spacerWidth, 55),
                        Margin = new Padding(0),
                        BorderStyle = BorderStyle.None,
                        BackColor = Color.Transparent,
                        Tag = "SPACER"
                    };
                    pnlAnswer.Controls.Add(spacer);
                }
            }

            pnlAnswer.ResumeLayout();

            var random = new Random();
            string answerWithoutSpaces = current.Answer.Replace(" ", "");
            string shuffledAnswer = new string(answerWithoutSpaces.ToCharArray()
                                                                  .OrderBy(c => random.Next())
                                                                  .ToArray());

            foreach (char c in shuffledAnswer)
            {
                var choice = new RoundedButton
                {
                    Text = c.ToString(),
                    Size = new Size(65, 65),
                    Font = new Font("Lexend", 24F, FontStyle.Bold),
                    CornerRadius = 10,
                    BackColor = Color.FromArgb(87, 187, 247),
                    ForeColor = Color.White,
                    Margin = new Padding(5)
                };
                choice.FlatAppearance.BorderSize = 0;
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
                if (slot.Tag != null && slot.Tag.ToString() == "SPACER")
                {
                    continue;
                }

                if (string.IsNullOrEmpty(slot.Text))
                {
                    slot.Text = choice.Text;
                    slot.Tag = choice;
                    break;
                }
            }
            CheckAnswer();
        }

        private void Answer_Click(object sender, EventArgs e)
        {
            Label slot = sender as Label;

            if (slot.Tag != null && slot.Tag.ToString() == "SPACER")
            {
                return;
            }

            if (!string.IsNullOrEmpty(slot.Text) && slot.Tag is Button)
            {
                (slot.Tag as Button).Visible = true;
                slot.Text = "";
                slot.Tag = null;
            }
        }

        private async void CheckAnswer()
        {
            var answerSlots = pnlAnswer.Controls.OfType<Label>()
                                       .Where(l => l.Tag == null || l.Tag.ToString() != "SPACER")
                                       .ToList();

            string userAnswer = string.Concat(answerSlots.Select(l => l.Text));
            string correctAnswer = _items[_currentItemIndex].Answer.Replace(" ", "");

            if (userAnswer.Length == correctAnswer.Length)
            {
                pnlChoices.Enabled = false;
                pnlAnswer.Enabled = false;

                if (userAnswer == correctAnswer)
                {
                    _correctAnswers++;

                    // ### CẬP NHẬT NGAY ###
                    lblProgress.Text = $" Câu {_currentItemIndex + 1}/{_items.Count} | Đúng: {_correctAnswers}";

                    lblStatus.Text = "🎉 Chính xác! 🎉";
                    lblStatus.ForeColor = Color.White;
                    lblStatus.BackColor = Color.FromArgb(29, 209, 161);
                    lblStatus.Visible = true;

                    foreach (Label slot in answerSlots)
                    {
                        slot.BackColor = Color.FromArgb(29, 209, 161);
                        slot.ForeColor = Color.White;
                    }

                    await Task.Delay(1500);

                    if (_currentItemIndex < _items.Count - 1)
                    {
                        btnNext.Visible = true;
                        lblStatus.Text = "Nhấn 'Tiếp theo' để chơi câu kế tiếp";
                        lblStatus.BackColor = Color.Transparent;
                        lblStatus.ForeColor = Color.FromArgb(87, 187, 247);
                    }
                    else
                    {
                        await Task.Delay(1000);
                        EndGame();
                    }
                }
                else
                {
                    lblStatus.Text = "❌ Chưa đúng, thử lại nào!";
                    lblStatus.ForeColor = Color.White;
                    lblStatus.BackColor = Color.FromArgb(255, 118, 117);
                    lblStatus.Visible = true;

                    foreach (Label slot in answerSlots)
                    {
                        slot.BackColor = Color.FromArgb(255, 118, 117);
                        slot.ForeColor = Color.White;
                    }

                    await Task.Delay(2000);

                    foreach (Label slot in answerSlots)
                    {
                        if (slot.Tag is Button btn)
                        {
                            btn.Visible = true;
                        }
                        slot.Text = "";
                        slot.Tag = null;
                        slot.BackColor = Color.White;
                        slot.ForeColor = Color.FromArgb(64, 64, 64);
                    }

                    pnlChoices.Enabled = true;
                    pnlAnswer.Enabled = true;
                    lblStatus.Visible = false;
                }
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            _currentItemIndex++;
            LoadCurrentItem();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            var answerSlots = pnlAnswer.Controls.OfType<Label>()
                                       .Where(l => l.Tag == null || l.Tag.ToString() != "SPACER")
                                       .ToList();

            foreach (Label slot in answerSlots)
            {
                if (slot.Tag is Button btn)
                {
                    btn.Visible = true;
                }
                slot.Text = "";
                slot.Tag = null;
                slot.BackColor = Color.White;
                slot.ForeColor = Color.FromArgb(64, 64, 64);
            }

            pnlChoices.Enabled = true;
            pnlAnswer.Enabled = true;
            lblStatus.Visible = false;
            btnNext.Visible = false;
        }

        // ### MÀN HÌNH HOÀN THÀNH ###
        private void EndGame()
        {
            this.Controls.Clear();
            this.BackColor = Color.FromArgb(240, 247, 255);

            Panel pnlResult = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 247, 255),
                Padding = new Padding(50)
            };

            Label lblIcon = new Label
            {
                Text = "🎊",
                Dock = DockStyle.Top,
                Height = 80,
                Font = new Font("Segoe UI Emoji", 60F),
                ForeColor = Color.FromArgb(29, 209, 161),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblTitle = new Label
            {
                Text = "Hoàn thành!",
                Dock = DockStyle.Top,
                Height = 80,
                Font = new Font("Lexend", 32F, FontStyle.Bold),
                ForeColor = Color.FromArgb(29, 209, 161),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblScore = new Label
            {
                Text = $"Bạn đã trả lời đúng {_correctAnswers}/{_items.Count} câu",
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font("Segoe UI", 20F),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            double percentage = (_correctAnswers * 100.0) / _items.Count;
            string comment = percentage >= 90 ? "Xuất sắc! 🌟" :
                            percentage >= 70 ? "Tốt lắm! 👍" :
                            percentage >= 50 ? "Khá tốt! 💪" :
                            "Cần cố gắng thêm nhé! 📚";

            Label lblComment = new Label
            {
                Text = comment,
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 18F, FontStyle.Italic),
                ForeColor = Color.FromArgb(255, 189, 89),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            RoundedButton btnClose = new RoundedButton
            {
                Text = "Đóng",
                Size = new Size(200, 60),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                BackColor = Color.FromArgb(87, 187, 247),
                ForeColor = Color.White,
                CornerRadius = 15,
                Location = new Point((this.ClientSize.Width - 200) / 2, 400)
            };
            btnClose.Click += (s, e) => this.Close();

            pnlResult.Controls.AddRange(new Control[] { lblComment, lblScore, lblTitle, lblIcon });
            this.Controls.Add(pnlResult);
            this.Controls.Add(btnClose);
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
        private List<SentenceScrambleItem> _sentences;
        private int _currentSentenceIndex = 0;
        private int _correctCount = 0; // ### THÊM MỚI ###
        private FlowLayoutPanel pnlChoices, pnlAnswer;
        private Label lblResult, lblInstruction, lblOriginalSentence, lblProgress;
        private RoundedButton btnCheck, btnReset, btnShowHint, btnNext;
        private bool _showingHint = false;
        private bool isCompleted = false;

        public SapXepCauForm(List<SentenceScrambleItem> sentences)
        {
            if (sentences == null || sentences.Count == 0)
            {
                CloseWithWarning("Không có câu nào để chơi!");
                return;
            }

            _sentences = sentences.Where(s => !string.IsNullOrWhiteSpace(s.CorrectSentence)).ToList();
            if (_sentences.Count == 0)
            {
                CloseWithWarning("Không có câu hợp lệ để chơi!");
                return;
            }

            InitializeComponent();
            LoadCurrentSentence();
        }

        public SapXepCauForm(SentenceScrambleItem item) : this(new List<SentenceScrambleItem> { item }) { }

        private void InitializeComponent()
        {
            this.Text = "🔄 Sắp xếp câu";
            this.Size = new Size(900, 700);
            this.BackColor = Color.FromArgb(240, 247, 255);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 650);

            // ===== PANEL HEADER VỚI GRADIENT =====
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White
            };

            pnlHeader.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    pnlHeader.ClientRectangle,
                    Color.FromArgb(87, 187, 247),
                    Color.FromArgb(19, 104, 206),
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, pnlHeader.ClientRectangle);
                }
            };

            Label lblIcon = new Label
            {
                Text = "📚",
                Font = new Font("Segoe UI Emoji", 22F),
                ForeColor = Color.White,
                Location = new Point(30, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label lblTitle = new Label
            {
                Text = "Sắp xếp câu",
                Font = new Font("Lexend", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(80, 12),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // ### THÊM SỐ CÂU ĐÚNG ###
            lblProgress = new Label
            {
                Location = new Point(80, 45),
                AutoSize = true,
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 215, 0),
                BackColor = Color.Transparent
            };

            pnlHeader.Controls.AddRange(new Control[] { lblIcon, lblTitle, lblProgress });

            Panel pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(20)
            };

            lblInstruction = new Label
            {
                Text = "🎯 Hãy click vào các từ theo thứ tự đúng để tạo thành câu hoàn chỉnh:",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblOriginalSentence = new Label
            {
                Text = "",
                Dock = DockStyle.Bottom,
                Height = 25,
                Font = new Font("Segoe UI", 11F, FontStyle.Italic),
                ForeColor = Color.FromArgb(40, 167, 69),
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false
            };

            pnlTop.Controls.AddRange(new Control[] { lblOriginalSentence, lblInstruction });

            pnlAnswer = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.White,
                Padding = new Padding(20),
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            pnlChoices = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(248, 249, 250),
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 100, BackColor = Color.White };

            var pnlButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(20, 25, 20, 20),
                WrapContents = false
            };

            btnCheck = new RoundedButton
            {
                Text = "✅ Kiểm tra",
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                CornerRadius = 8,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnCheck.Click += BtnCheck_Click;

            btnReset = new RoundedButton
            {
                Text = "🔄 Làm lại",
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                CornerRadius = 8,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnReset.Click += BtnReset_Click;

            btnShowHint = new RoundedButton
            {
                Text = "💡 Gợi ý",
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                CornerRadius = 8,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnShowHint.Click += BtnShowHint_Click;

            btnNext = new RoundedButton
            {
                Text = "➡️ Tiếp theo",
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(29, 209, 161),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                CornerRadius = 8,
                Margin = new Padding(0, 0, 10, 0),
                Visible = false
            };
            btnNext.Click += BtnNext_Click;

            lblResult = new Label
            {
                Dock = DockStyle.Right,
                Width = 200,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            pnlButtons.Controls.AddRange(new Control[] { btnCheck, btnReset, btnShowHint, btnNext, lblResult });
            pnlBottom.Controls.Add(pnlButtons);

            this.Controls.AddRange(new Control[] { pnlChoices, pnlAnswer, pnlBottom, pnlTop, pnlHeader });
        }

        private void LoadCurrentSentence()
        {
            if (_currentSentenceIndex >= _sentences.Count)
            {
                EndAllSentences();
                return;
            }

            var currentSentence = _sentences[_currentSentenceIndex];

            // ### CẬP NHẬT SỐ CÂU ĐÚNG ###
            lblProgress.Text = $" Câu {_currentSentenceIndex + 1}/{_sentences.Count} | Đúng: {_correctCount}";

            isCompleted = false;
            lblResult.Visible = false;
            lblOriginalSentence.Visible = false;
            btnNext.Visible = false;
            btnCheck.Visible = true;
            btnShowHint.Text = "💡 Gợi ý";
            btnShowHint.BackColor = Color.FromArgb(255, 193, 7);
            _showingHint = false;

            var words = currentSentence.CorrectSentence.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                      .OrderBy(x => Guid.NewGuid())
                                                      .ToList();

            pnlChoices.Controls.Clear();
            pnlAnswer.Controls.Clear();

            foreach (var word in words)
            {
                CreateWordButton(word, pnlChoices);
            }

            lblOriginalSentence.Text = $"💡 Câu mẫu: \"{currentSentence.CorrectSentence}\"";
        }

        private RoundedButton CreateWordButton(string word, FlowLayoutPanel parent)
        {
            var btn = new RoundedButton
            {
                Text = word,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(15, 8, 15, 8),
                Margin = new Padding(8),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                CornerRadius = 20,
                Cursor = Cursors.Hand
            };

            btn.Click += WordButton_Click;
            parent.Controls.Add(btn);
            return btn;
        }

        private async void WordButton_Click(object sender, EventArgs e)
        {
            if (isCompleted) return;

            var btn = sender as RoundedButton;

            btn.BackColor = Color.FromArgb(40, 167, 69);
            await Task.Delay(150);

            if (btn.Parent == pnlChoices)
            {
                pnlChoices.Controls.Remove(btn);
                pnlAnswer.Controls.Add(btn);
                btn.BackColor = Color.FromArgb(40, 167, 69);
            }
            else
            {
                pnlAnswer.Controls.Remove(btn);
                pnlChoices.Controls.Add(btn);
                btn.BackColor = Color.FromArgb(0, 123, 255);
            }

            if (lblResult.Visible)
            {
                lblResult.Visible = false;
            }
        }

        private async void BtnCheck_Click(object sender, EventArgs e)
        {
            if (pnlAnswer.Controls.Count == 0)
            {
                ShowResult("⚠️ Hãy chọn ít nhất một từ!", Color.FromArgb(255, 118, 117));
                return;
            }

            var currentSentence = _sentences[_currentSentenceIndex];
            string userAnswer = string.Join(" ", pnlAnswer.Controls.OfType<RoundedButton>().Select(b => b.Text));

            btnCheck.Enabled = false;
            btnCheck.Text = "Đang kiểm tra...";

            await Task.Delay(500);

            if (userAnswer.Equals(currentSentence.CorrectSentence, StringComparison.OrdinalIgnoreCase))
            {
                _correctCount++; // ### TĂNG SỐ CÂU ĐÚNG ###

                // ### CẬP NHẬT NGAY ###
                lblProgress.Text = $" Câu {_currentSentenceIndex + 1}/{_sentences.Count} | Đúng: {_correctCount}";

                isCompleted = true;
                ShowResult("🎉 Chính xác! 🎉", Color.FromArgb(29, 209, 161));

                foreach (RoundedButton btn in pnlAnswer.Controls)
                {
                    btn.BackColor = Color.FromArgb(29, 209, 161);
                }

                btnCheck.Visible = false;
                if (_currentSentenceIndex < _sentences.Count - 1)
                {
                    btnNext.Visible = true;
                }
                else
                {
                    await Task.Delay(3000);
                    EndAllSentences();
                }
            }
            else
            {
                ShowResult("❌ Chưa đúng! Thử lại nhé!", Color.FromArgb(255, 118, 117));

                foreach (RoundedButton btn in pnlAnswer.Controls)
                {
                    btn.BackColor = Color.FromArgb(255, 235, 235);
                }

                await Task.Delay(1000);

                foreach (RoundedButton btn in pnlAnswer.Controls)
                {
                    btn.BackColor = Color.FromArgb(40, 167, 69);
                }
            }

            btnCheck.Enabled = true;
            btnCheck.Text = "✅ Kiểm tra";
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            _currentSentenceIndex++;
            LoadCurrentSentence();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            var buttonsInAnswer = pnlAnswer.Controls.OfType<RoundedButton>().ToArray();
            foreach (var btn in buttonsInAnswer)
            {
                pnlAnswer.Controls.Remove(btn);
                pnlChoices.Controls.Add(btn);
                btn.BackColor = Color.FromArgb(0, 123, 255);
            }

            lblResult.Visible = false;
            lblOriginalSentence.Visible = false;
            isCompleted = false;
            btnNext.Visible = false;
            btnCheck.Visible = true;
            _showingHint = false;
            btnShowHint.Text = "💡 Gợi ý";
            btnShowHint.BackColor = Color.FromArgb(255, 193, 7);
        }

        private void BtnShowHint_Click(object sender, EventArgs e)
        {
            if (!_showingHint)
            {
                lblOriginalSentence.Visible = true;
                btnShowHint.Text = "🙈 Ẩn gợi ý";
                btnShowHint.BackColor = Color.FromArgb(108, 117, 125);
                _showingHint = true;
            }
            else
            {
                lblOriginalSentence.Visible = false;
                btnShowHint.Text = "💡 Gợi ý";
                btnShowHint.BackColor = Color.FromArgb(255, 193, 7);
                _showingHint = false;
            }
        }

        private void ShowResult(string message, Color color)
        {
            lblResult.Text = message;
            lblResult.ForeColor = Color.White;
            lblResult.BackColor = color;
            lblResult.Visible = true;
        }

        // ### MÀN HÌNH HOÀN THÀNH ###
        private void EndAllSentences()
        {
            this.Controls.Clear();
            this.BackColor = Color.FromArgb(240, 247, 255);

            Panel pnlResult = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 247, 255),
                Padding = new Padding(50)
            };

            Label lblIcon = new Label
            {
                Text = "🎊",
                Dock = DockStyle.Top,
                Height = 80,
                Font = new Font("Segoe UI Emoji", 60F),
                ForeColor = Color.FromArgb(29, 209, 161),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblTitle = new Label
            {
                Text = "Hoàn thành!",
                Dock = DockStyle.Top,
                Height = 80,
                Font = new Font("Lexend", 32F, FontStyle.Bold),
                ForeColor = Color.FromArgb(29, 209, 161),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblScore = new Label
            {
                Text = $"Bạn đã trả lời đúng {_correctCount}/{_sentences.Count} câu",
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font("Segoe UI", 20F),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            double percent = (_correctCount * 100.0) / _sentences.Count;
            string comment = percent >= 80 ? "Xuất sắc! 🌟" :
                            percent >= 60 ? "Tốt lắm! 👍" :
                            "Cần cố gắng thêm! 💪";

            Label lblComment = new Label
            {
                Text = comment,
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 18F, FontStyle.Italic),
                ForeColor = Color.FromArgb(255, 189, 89),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            RoundedButton btnClose = new RoundedButton
            {
                Text = "Đóng",
                Size = new Size(200, 60),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                BackColor = Color.FromArgb(87, 187, 247),
                ForeColor = Color.White,
                CornerRadius = 15,
                Location = new Point((this.ClientSize.Width - 200) / 2, 400)
            };
            btnClose.Click += (s, e) => this.Close();

            pnlResult.Controls.AddRange(new Control[] { lblComment, lblScore, lblTitle, lblIcon });
            this.Controls.Add(pnlResult);
            this.Controls.Add(btnClose);
        }
    }

    // ===================================================================
    // GAME 7: ĐIỀN TỪ (MNG07)
    // ===================================================================
    // ===================================================================
    // GAME 7: ĐIỀN TỪ (MNG07)
    // ===================================================================
    public class DienTuForm : GameFormWithMusic
    {
        private List<FillBlankQuestion> _questions;
        private int _currentIndex = 0;
        private int _correctCount = 0;

        private Label lblHeader, lblQuestion, lblResult, lblProgress;
        private TextBox txtAnswer;
        private RoundedButton btnCheck, btnSkip, btnNext;

        public DienTuForm(List<FillBlankQuestion> questions)
        {
            if (questions == null || questions.Count == 0 || string.IsNullOrWhiteSpace(questions[0].QuestionText))
            {
                CloseWithWarning("Không có câu hỏi hợp lệ để chơi!");
                return;
            }
            _questions = questions;
            InitializeComponent();
            LoadQuestion();
        }

        private void InitializeComponent()
        {
            this.Text = "✍️ Điền từ vào chỗ trống";
            this.Size = new Size(900, 600);  // Tăng chiều cao: 550 → 600
            this.BackColor = BgColor;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ===== PANEL HEADER VỚI GRADIENT (MỚI) =====
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White
            };

            pnlHeader.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    pnlHeader.ClientRectangle,
                    Color.FromArgb(87, 187, 247),
                    Color.FromArgb(19, 104, 206),
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, pnlHeader.ClientRectangle);
                }
            };

            // Icon game
            Label lblIcon = new Label
            {
                Text = "✍️",
                Font = new Font("Segoe UI Emoji", 22F),
                ForeColor = Color.White,
                Location = new Point(30, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Tiêu đề game
            Label lblGameTitle = new Label
            {
                Text = "Điền từ vào chỗ trống",
                Font = new Font("Lexend", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(80, 12),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Tiến độ (dưới tiêu đề)
            lblProgress = new Label
            {
                Location = new Point(80, 45),
                AutoSize = true,
                Font = new Font("Lexend", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 215, 0),
                BackColor = Color.Transparent
            };

            pnlHeader.Controls.AddRange(new Control[] { lblIcon, lblGameTitle, lblProgress });

            // ===== PANEL HƯỚNG DẪN (XÓA lblHeader CŨ, THAY BẰNG PANEL NÀY) =====
            Panel pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(20, 15, 20, 15)
            };

            Label lblInstruction = new Label
            {
                Text = "📝 Hãy điền từ thích hợp vào chỗ trống (___):",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlTop.Controls.Add(lblInstruction);

            // ===== PANEL CHÍNH - CARD TRẮNG =====
            Panel pnlMainCard = new Panel
            {
                Size = new Size(820, 250),
                Location = new Point(40, 165),
                BackColor = Color.White,
                Padding = new Padding(30)
            };

            // Vẽ shadow và bo góc
            pnlMainCard.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 5, 5, pnlMainCard.Width - 5, pnlMainCard.Height - 5);
                }

                // Background với bo góc
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, pnlMainCard.Width - 1, pnlMainCard.Height - 1), 15))
                {
                    using (var brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    using (var pen = new Pen(Color.FromArgb(220, 230, 240), 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            // Label câu hỏi
            lblQuestion = new Label
            {
                Location = new Point(30, 30),
                Size = new Size(760, 80),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };

            // TextBox trả lời
            txtAnswer = new TextBox
            {
                Font = new Font("Segoe UI", 16F),
                Location = new Point(30, 130),
                Width = 500,
                Height = 40,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtAnswer.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    btnCheck.PerformClick();
                }
            };

            // Label kết quả
            lblResult = new Label
            {
                Location = new Point(30, 185),
                Size = new Size(760, 40),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false
            };

            pnlMainCard.Controls.AddRange(new Control[] { lblQuestion, txtAnswer, lblResult });

            // ===== PANEL ĐIỀU KHIỂN DƯỚI =====
            Panel pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            var pnlButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(20, 10, 20, 10),
                WrapContents = false
            };

            btnCheck = new RoundedButton
            {
                Text = "✅ Kiểm tra",
                Size = new Size(150, 50),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                CornerRadius = 10,
                Margin = new Padding(0, 0, 15, 0)
            };
            btnCheck.FlatAppearance.BorderSize = 0;
            btnCheck.Click += BtnCheck_Click;

            btnSkip = new RoundedButton
            {
                Text = "⏭️ Bỏ qua",
                Size = new Size(150, 50),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                CornerRadius = 10,
                Margin = new Padding(0, 0, 15, 0)
            };
            btnSkip.FlatAppearance.BorderSize = 0;
            btnSkip.Click += BtnSkip_Click;

            btnNext = new RoundedButton
            {
                Text = "➡️ Tiếp tục",
                Size = new Size(150, 50),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = CorrectColor,
                ForeColor = Color.White,
                CornerRadius = 10,
                Margin = new Padding(0, 0, 15, 0),
                Visible = false
            };
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += BtnNext_Click;

            pnlButtons.Controls.AddRange(new Control[] { btnCheck, btnSkip, btnNext });
            pnlBottom.Controls.Add(pnlButtons);

            this.Controls.AddRange(new Control[] { pnlMainCard, pnlBottom, pnlTop, pnlHeader });
        }

        // Helper method để vẽ bo góc (thêm vào class DienTuForm)
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

        private void LoadQuestion()
        {
            if (_currentIndex >= _questions.Count)
            {
                ShowFinalResult();
                return;
            }

            // Cập nhật tiến trình (sử dụng lblProgress thay vì lblHeader)
            lblProgress.Text = $" Câu {_currentIndex + 1}/{_questions.Count} | Đúng: {_correctCount}";

            // Hiển thị câu hỏi
            lblQuestion.Text = _questions[_currentIndex].QuestionText;

            // Reset UI
            txtAnswer.Clear();
            txtAnswer.BackColor = Color.White;
            txtAnswer.Enabled = true;
            txtAnswer.Focus();

            lblResult.Visible = false;
            btnCheck.Visible = true;
            btnSkip.Visible = true;
            btnNext.Visible = false;
        }

        private async void BtnCheck_Click(object sender, EventArgs e)
        {
            string userAnswer = txtAnswer.Text.Trim();
            string correctAnswer = _questions[_currentIndex].Answer;

            // Disable controls
            txtAnswer.Enabled = false;
            btnCheck.Enabled = false;
            btnSkip.Enabled = false;

            if (string.Equals(userAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase))
            {
                // Đúng
                _correctCount++;
                txtAnswer.BackColor = CorrectColor;
                lblResult.Text = "🎉 Chính xác! 🎉";
                lblResult.ForeColor = Color.White;
                lblResult.BackColor = CorrectColor;
                lblResult.Visible = true;

                await Task.Delay(1500);

                // ### THAY ĐỔI 1: Kiểm tra nếu đây là câu cuối ###
                if (_currentIndex < _questions.Count - 1)
                {
                    // Chưa phải câu cuối -> Hiện nút tiếp tục
                    btnCheck.Visible = false;
                    btnSkip.Visible = false;
                    btnNext.Visible = true;
                }
                else
                {
                    // ### ĐÂY LÀ CÂU CUỐI -> Tự động chuyển sang màn hình hoàn thành ###
                    await Task.Delay(1000);
                    ShowFinalResult();
                }
            }
            else
            {
                // Sai
                txtAnswer.BackColor = IncorrectColor;
                lblResult.Text = $"❌ Chưa đúng! Đáp án là: {correctAnswer}";
                lblResult.ForeColor = Color.White;
                lblResult.BackColor = IncorrectColor;
                lblResult.Visible = true;

                // ### THAY ĐỔI 2: Kiểm tra nếu đây là câu cuối ###
                if (_currentIndex < _questions.Count - 1)
                {
                    // Chưa phải câu cuối -> Hiện nút tiếp tục
                    btnCheck.Visible = false;
                    btnSkip.Visible = false;
                    btnNext.Visible = true;
                }
                else
                {
                    // ### ĐÂY LÀ CÂU CUỐI -> Tự động chuyển sang màn hình hoàn thành ###
                    await Task.Delay(2000);
                    ShowFinalResult();
                }
            }

            btnCheck.Enabled = true;
            btnSkip.Enabled = true;
        }

        private void BtnSkip_Click(object sender, EventArgs e)
        {
            string correctAnswer = _questions[_currentIndex].Answer;

            txtAnswer.BackColor = Color.FromArgb(248, 249, 250);
            lblResult.Text = $"⏭️ Đã bỏ qua. Đáp án là: {correctAnswer}";
            lblResult.ForeColor = Color.FromArgb(108, 117, 125);
            lblResult.BackColor = Color.FromArgb(248, 249, 250);
            lblResult.Visible = true;

            txtAnswer.Enabled = false;

            // ### THAY ĐỔI 3: Kiểm tra nếu đây là câu cuối ###
            if (_currentIndex < _questions.Count - 1)
            {
                // Chưa phải câu cuối -> Hiện nút tiếp tục
                btnCheck.Visible = false;
                btnSkip.Visible = false;
                btnNext.Visible = true;
            }
            else
            {
                // ### ĐÂY LÀ CÂU CUỐI -> Tự động chuyển sang màn hình hoàn thành ###
                // Delay ngắn để người dùng kịp đọc thông báo
                Task.Delay(1500).ContinueWith(t =>
                {
                    if (!this.IsDisposed)
                    {
                        this.Invoke(new Action(() => ShowFinalResult()));
                    }
                });
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            _currentIndex++;
            LoadQuestion();
        }

        private void ShowFinalResult()
        {
            this.Controls.Clear();

            Panel pnlResult = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BgColor,
                Padding = new Padding(50)
            };

            Label lblTitle = new Label
            {
                Text = "🎊 Hoàn thành! 🎊",
                Dock = DockStyle.Top,
                Height = 80,
                Font = new Font("Segoe UI", 32F, FontStyle.Bold),
                ForeColor = CorrectColor,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblScore = new Label
            {
                Text = $"Bạn đã trả lời đúng {_correctCount}/{_questions.Count} câu",
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font("Segoe UI", 20F),
                ForeColor = TextColor,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Đánh giá
            string comment = "";
            double percent = (_correctCount * 100.0) / _questions.Count;
            if (percent >= 80) comment = "Xuất sắc! 🌟";
            else if (percent >= 60) comment = "Tốt lắm! 👍";
            else if (percent >= 40) comment = "Cần cố gắng thêm! 💪";
            else comment = "Hãy học thêm nhé! 📚";

            Label lblComment = new Label
            {
                Text = comment,
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 18F, FontStyle.Italic),
                ForeColor = SecondaryColor,
                TextAlign = ContentAlignment.MiddleCenter
            };

            RoundedButton btnClose = new RoundedButton
            {
                Text = "Đóng",
                Size = new Size(200, 60),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                CornerRadius = 15,
                Location = new Point((this.ClientSize.Width - 200) / 2, 350)
            };
            btnClose.Click += (s, e) => this.Close();

            pnlResult.Controls.AddRange(new Control[] { lblComment, lblScore, lblTitle });
            this.Controls.Add(pnlResult);
            this.Controls.Add(btnClose);
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
            if ( items == null || items.Count < 2) { CloseWithWarning("Cần ít nhất 2 mục để chơi."); return; }
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
            btnSpin.Location = new Point(275, 450);
            btnSpin.Click += BtnSpin_Click;
            spinTimer = new Timer { Interval = 25 };
            spinTimer.Tick += SpinTimer_Tick;
            this.Controls.Add(lblResult);
            this.Controls.Add(btnSpin);
        }

        private void BtnSpin_Click(object sender, EventArgs e)
        {
            if (_numbers == null || _numbers.Count < 2) { MessageBox.Show("Cần ít nhất 2 số (min và max) trong danh sách.", "Dữ liệu không hợp lệ"); return; }
            btnSpin.Enabled = false;
            btnSpin.Text = "ĐANG QUAY...";
            btnSpin.BackColor = Color.Gray;
            int min = _numbers.Min();
            int max = _numbers.Max();
            lblResult.Text = "00";
            spinTicks = 0;
            spinTimer.Start();
        }

        private void SpinTimer_Tick(object sender, EventArgs e)
        {
            spinTicks++;
            int min = _numbers.Min();
            int max = _numbers.Max();
            lblResult.Text = random.Next(min, max + 1).ToString("00");
            if (spinTicks > 40)
            {
                spinTimer.Stop();
                btnSpin.Enabled = true;
                int finalNumber = random.Next(min, max + 1);
                lblResult.Text = finalNumber.ToString("00");
                lblResult.ForeColor = SecondaryColor;
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
            this.Text = "⚽ Chuyền Bóng Nóng"; this.Size = new Size(800, 600); this.BackColor = BgColor; this.StartPosition = FormStartPosition.CenterScreen;
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
            btnStart.Enabled = false;
            btnStart.Text = "ĐANG CHẠY...";
            btnStart.BackColor = Color.Gray;
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