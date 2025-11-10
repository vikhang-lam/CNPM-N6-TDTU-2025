using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
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
                Width = 680,
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
                Width = 680,
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

            foreach (var item in items)
            {
                AddFlashcardCard(item);
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
                Text = "Mặt trước (Thuật ngữ)",
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
                Text = "Mặt sau (Định nghĩa)",
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
}
