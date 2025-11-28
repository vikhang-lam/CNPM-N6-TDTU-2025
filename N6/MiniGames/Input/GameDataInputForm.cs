using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data;

namespace N6
{
    /// <summary>
    /// Form soạn dữ liệu cho từng MiniGame.
    /// </summary>
    public class GameDataInputForm : Form
    {
        #region Fields

        private readonly string _maMNG;
        private readonly string _tenMNG;

        private readonly Color kahootRed = Color.FromArgb(226, 27, 60);
        private readonly Color kahootBlue = Color.FromArgb(19, 104, 206);
        private readonly Color kahootYellow = Color.FromArgb(216, 158, 0);
        private readonly Color kahootGreen = Color.FromArgb(40, 135, 63);
        private readonly Color kahootPurple = Color.FromArgb(70, 31, 137);
        private readonly Color lightGrayBg = Color.FromArgb(242, 242, 242);

        private Panel pnlHeader;
        private Panel pnlToolbar;
        private PictureBox picGameIcon;
        private Label lblGameName;
        private FlowLayoutPanel pnlInputArea;
        private RoundedButton btnSave;
        private RoundedButton btnPlay;
        private RoundedButton btnLoadExcel;
        private RoundedButton btnReload;
        private RoundedButton btnDeleteAll;

        // New: filter UI + state (range filter, 1-based indices)
        private RoundedButton btnFilter;
        private Label lblFilterSummary;
        private int? _filterStart = null;
        private int? _filterEnd = null;
        private Panel pnlFilterBar;
        private string _sentenceFullText = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Khởi tạo form soạn dữ liệu cho mã game và tên game tương ứng.
        /// </summary>
        /// <param name="maMNG">Mã MiniGame (ví dụ "MNG01").</param>
        /// <param name="tenMNG">Tên hiển thị của MiniGame.</param>
        public GameDataInputForm(string maMNG, string tenMNG)
        {
            _maMNG = maMNG;
            _tenMNG = tenMNG;
            InitializeComponent();
            BuildInputUI();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Khởi tạo component cơ bản của Form (header, toolbar, vùng nhập).
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "Soạn nội dung cho game: " + _tenMNG;
            this.Size = new Size(800, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = lightGrayBg;
            this.Font = new Font("Lexend", 10F);

            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White
            };

            picGameIcon = new PictureBox
            {
                Size = new Size(50, 50),
                Location = new Point(20, 15),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            try
            {
                picGameIcon.Image = (Image)Properties.Resources.ResourceManager.GetObject(_maMNG);
            }
            catch
            {
                try
                {
                    picGameIcon.Image = Properties.Resources.placeholder;
                }
                catch { }
            }

            lblGameName = new Label
            {
                Text = _tenMNG,
                Font = new Font("Lexend", 18F, FontStyle.Bold),
                ForeColor = kahootPurple,
                AutoSize = true,
                Location = new Point(80, 20)
            };

            pnlHeader.Controls.AddRange(new Control[]
            {
                picGameIcon,
                lblGameName
            });

            pnlToolbar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            btnPlay = new RoundedButton
            {
                Text = "Bắt đầu chơi",
                Size = new Size(130, 48),
                Location = new Point(620, 16),
                BackColor = kahootGreen,
                ForeColor = Color.White,
                Font = new Font("Lexend", 11F, FontStyle.Bold),
                CornerRadius = 10
            };

            btnSave = new RoundedButton
            {
                Text = "Lưu dữ liệu",
                Size = new Size(130, 48),
                Location = new Point(480, 16),
                BackColor = kahootBlue,
                ForeColor = Color.White,
                Font = new Font("Lexend", 11F, FontStyle.Bold),
                CornerRadius = 10
            };

            btnReload = new RoundedButton
            {
                Text = "Tải lại",
                Size = new Size(120, 48),
                Location = new Point(140, 16),
                BackColor = kahootYellow,
                ForeColor = Color.White,
                Font = new Font("Lexend", 11F, FontStyle.Bold),
                CornerRadius = 10
            };

            btnLoadExcel = new RoundedButton
            {
                Text = "Tải từ Excel",
                Size = new Size(120, 48),
                Location = new Point(10, 16), // cách trái 10px
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = new Font("Lexend", 11F, FontStyle.Bold),
                CornerRadius = 10
            };

            btnDeleteAll = new RoundedButton
            {
                Text = "Xóa tất cả",
                Size = new Size(120, 48),
                Location = new Point(270, 16),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Lexend", 11F, FontStyle.Bold),
                CornerRadius = 10
            };

            btnFilter = new RoundedButton
            {
                Text = "🔍 Bộ lọc",
                Size = new Size(100, 36),
                BackColor = Color.FromArgb(90, 90, 90),
                ForeColor = Color.White,
                Font = new Font("Lexend", 9F, FontStyle.Bold),
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };

            lblFilterSummary = new Label
            {
                Text = "",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.Gray
            };

            pnlFilterBar = new Panel
            {
                Width = 720,
                Height = 48,
                BackColor = Color.Transparent,
                Margin = new Padding(5, 0, 5, 10)
            };

            btnFilter.Location = new Point(10, 6);
            lblFilterSummary.Location = new Point(124, 12);

            pnlFilterBar.Controls.Add(btnFilter);
            pnlFilterBar.Controls.Add(lblFilterSummary);

            // Thêm từng nút vào toolbar theo thứ tự mong muốn
            pnlToolbar.Controls.Add(btnLoadExcel);
            pnlToolbar.Controls.Add(btnReload);
            pnlToolbar.Controls.Add(btnDeleteAll);
            //pnlToolbar.Controls.Add(btnFilter);
            //pnlToolbar.Controls.Add(lblFilterSummary);
            pnlToolbar.Controls.Add(btnPlay);
            pnlToolbar.Controls.Add(btnSave);


            pnlInputArea = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = lightGrayBg,
                Padding = new Padding(20),
                AutoScroll = true
            };

            this.Controls.AddRange(new Control[]
            {
                pnlInputArea,
                pnlToolbar,
                pnlHeader
            });

            // Gán sự kiện chính
            btnSave.Click += BtnSave_Click;
            btnPlay.Click += BtnPlay_Click;
            btnReload.Click += (s, e) => BuildInputUI();
            btnLoadExcel.Click += BtnLoadExcel_Click;
            btnDeleteAll.Click += BtnDeleteAll_Click;

            // Bộ lọc
            btnFilter.Click += BtnFilter_Click;
            UpdateFilterLabel();
        }

        #endregion

        #region UI Builders

        /// <summary>
        /// Xây dựng UI tương ứng cho từng loại MiniGame.
        /// </summary>
        private void BuildInputUI()
        {
            pnlInputArea.Controls.Clear();

            switch (_maMNG.ToUpper())
            {
                case "MNG01":
                    BuildQuizUI();
                    break;

                case "MNG03":
                    BuildFlashcardUI();
                    break;

                case "MNG02":
                case "MNG08":
                    BuildListUI();
                    break;

                case "MNG09":
                    BuildMinMaxUI();
                    break;

                case "MNG04":
                    BuildWordScrambleUI();
                    break;

                case "MNG06":
                    BuildSentenceScrambleUI();
                    break;

                case "MNG07":
                    BuildFillBlankUI();
                    break;

                default:
                    var lbl = new Label
                    {
                        Text = "Game này không yêu cầu nhập liệu hoặc chưa được triển khai.",
                        Font = new Font("Lexend", 14F),
                        AutoSize = true
                    };

                    pnlInputArea.Controls.Add(lbl);
                    btnSave.Enabled = btnReload.Enabled = btnLoadExcel.Enabled = false;
                    break;
            }
        }

        #endregion

        #region Quiz

        /// <summary>
        /// Xây dựng UI cho Quiz (MNG01).
        /// </summary>
        private void BuildQuizUI()
        {
            pnlInputArea.Controls.Clear();

            // ===== HEADER SECTION =====
            Panel pnlHeaderSection = new Panel
            {
                Width = 680,
                Height = 100,
                Margin = new Padding(5, 10, 5, 5),
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

            pnlHeaderSection.Controls.AddRange(new Control[]
            { lblHeaderIcon,
                lblHeaderTitle,
                lblHeaderDesc,
                lblSubDesc
            });
            pnlInputArea.Controls.Add(pnlHeaderSection);

            if (pnlFilterBar.Parent != pnlInputArea)
            {
                pnlFilterBar.Parent?.Controls.Remove(pnlFilterBar);
                pnlInputArea.Controls.Add(pnlFilterBar);
            }

            var spacer = new Panel
            {
                Width = 680,
                Height = 4,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 0)
            };
            pnlInputArea.Controls.Add(spacer);

            // Load existing questions
            foreach (var q in GameDataManager.GetQuizQuestions(_maMNG))
            {
                AddQuizQuestionControl(q);
            }

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
            RefreshQuizIndices();
        }

        /// <summary>
        /// Làm mới đánh số hiển thị (bắt đầu từ 1) cho các câu hỏi.
        /// </summary>
        private void RefreshQuizIndices()
        {
            try
            {
                var quizControls = pnlInputArea.Controls
                    .OfType<QuizQuestionControl>()
                    .ToList();

                for (int i = 0; i < quizControls.Count; i++)
                {
                    quizControls[i].SetIndex(i + 1);
                }
            }
            catch { }
        }

        /// <summary>
        /// Thêm một QuizQuestionControl vào pnlInputArea. Nếu data != null thì set giá trị.
        /// </summary>
        /// <param name="data">Dữ liệu câu hỏi (có thể null để tạo mới).</param>
        private void AddQuizQuestionControl(QuizQuestion data)
        {
            var qc = new QuizQuestionControl();

            if (data != null)
            {
                qc.SetData(data);
            }

            qc.BtnRemove.Click += (s, e) =>
            {
                pnlInputArea.Controls.Remove(qc);
                RefreshQuizIndices();
            };

            // Thêm vào pnl và đặt trước nút "Thêm câu hỏi"
            pnlInputArea.Controls.Add(qc);

            int btnAddIndex = -1;

            for (int i = 0; i < pnlInputArea.Controls.Count; i++)
            {
                if (pnlInputArea.Controls[i] is RoundedButton btn && btn.Text.Contains("Thêm câu hỏi"))
                {
                    btnAddIndex = i;
                    break;
                }
            }

            if (btnAddIndex >= 0)
            {
                pnlInputArea.Controls.SetChildIndex(qc, btnAddIndex);
            }
            ApplyUiRangeFilter();
        }

        #endregion

        #region Ghép chữ

        /// <summary>
        /// Xây dựng UI cho Word Scramble (MNG04).
        /// </summary>
        private void BuildWordScrambleUI()
        {
            pnlInputArea.Controls.Clear();
            pnlInputArea.AutoScroll = true;

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

            pnlHeaderSection.Controls.AddRange(new Control[]
            {
                lblHeaderIcon,
                lblHeaderTitle,
                lblHeaderDesc,
                lblSubDesc
            });
            pnlInputArea.Controls.Add(pnlHeaderSection);

            if (pnlFilterBar.Parent != pnlInputArea)
            {
                pnlFilterBar.Parent?.Controls.Remove(pnlFilterBar);
                pnlInputArea.Controls.Add(pnlFilterBar);
            }

            var spacer = new Panel
            {
                Width = 680,
                Height = 4,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 0)
            };
            pnlInputArea.Controls.Add(spacer);

            foreach (var item in GameDataManager.GetWordScrambleItems(_maMNG))
            {
                AddWordScrambleControl(item);
            }

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

        /// <summary>
        /// Thêm WordScrambleControl với dữ liệu (nếu có).
        /// </summary>
        /// <param name="data">WordScrambleItem hoặc null.</param>
        private void AddWordScrambleControl(WordScrambleItem data)
        {
            var wc = new WordScrambleControl();

            if (data != null)
            {
                wc.SetData(data);
            }

            wc.BtnRemove.Click += (s, e) => pnlInputArea.Controls.Remove(wc);

            pnlInputArea.Controls.Add(wc);

            int btnAddIndex = -1;

            for (int i = 0; i < pnlInputArea.Controls.Count; i++)
            {
                if (pnlInputArea.Controls[i] is RoundedButton btn && btn.Text.Contains("Thêm từ vựng"))
                {
                    btnAddIndex = i;
                    break;
                }
            }

            if (btnAddIndex >= 0)
            {
                pnlInputArea.Controls.SetChildIndex(wc, btnAddIndex);
            }
            ApplyUiRangeFilter();
        }

        #endregion

        #region Flashcard

        /// <summary>
        /// Xây dựng UI cho Flashcard (MNG03).
        /// </summary>
        private void BuildFlashcardUI()
        {
            pnlInputArea.Controls.Clear();

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

            pnlHeaderSection.Controls.AddRange(new Control[]
            {
                lblHeaderIcon,
                lblHeaderTitle,
                lblHeaderDesc,
                lblSubDesc
            });
            pnlInputArea.Controls.Add(pnlHeaderSection);

            if (pnlFilterBar.Parent != pnlInputArea)
            {
                pnlFilterBar.Parent?.Controls.Remove(pnlFilterBar);
                pnlInputArea.Controls.Add(pnlFilterBar);
            }

            var spacer = new Panel
            {
                Width = 680,
                Height = 4,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 0)
            };
            pnlInputArea.Controls.Add(spacer);

            var items = GameDataManager.GetFlashcardItems(_maMNG);

            foreach (var item in items)
            {
                AddFlashcardCard(item);
            }

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
            RefreshFlashcardIndices();
        }

        /// <summary>
        /// Refresh visible 1-based numbering for flashcard panels.
        /// </summary>
        private void RefreshFlashcardIndices()
        {
            try
            {
                IEnumerable<Control> Descendants(Control root)
                {
                    foreach (Control c in root.Controls)
                    {
                        yield return c;
                        foreach (var d in Descendants(c)) yield return d;
                    }
                }

                var flashcardPanels = pnlInputArea.Controls
                    .OfType<Panel>()
                    .Where(p => p.Tag != null && p.Tag.ToString() == "FLASHCARD_PANEL")
                    .ToList();

                for (int i = 0; i < flashcardPanels.Count; i++)
                {
                    // search all descendants for the title label
                    var titleLabel = Descendants(flashcardPanels[i])
                        .OfType<Label>()
                        .FirstOrDefault(l => l.Tag != null && l.Tag.ToString() == "FLASHCARD_TITLE");

                    if (titleLabel != null)
                    {
                        titleLabel.Text = $"Thẻ {i + 1}:";
                    }
                }
            }
            catch
            {
                // silent
            }
        }

        /// <summary>
        /// Thêm thẻ Flashcard vào giao diện (giữ nguyên logic ban đầu).
        /// </summary>
        /// <param name="item">FlashcardItem hoặc null.</param>
        private void AddFlashcardCard(FlashcardItem item)
        {
            Panel cardContainer = new Panel
            {
                Width = 720,
                Height = 100,
                Margin = new Padding(5, 8, 5, 8),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Tag = "FLASHCARD_PANEL"
            };

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
                Text = "Thẻ",
                Font = new Font("Lexend", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(50, 10),
                AutoSize = true,
                Tag = "FLASHCARD_TITLE"
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
                    RefreshFlashcardIndices();
                }
            };

            pnlCardHeader.Controls.AddRange(new Control[] { lblCardIcon, lblCardTitle, btnRemove });

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

            Label lblSwap = new Label
            {
                Text = "⇄",
                Font = new Font("Segoe UI", 20F),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(335, 50),
                AutoSize = true
            };

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

            if (item != null)
            {
                txtTerm.Text = item.Term;
                txtTerm.ForeColor = Color.Black;
                txtDefinition.Text = item.Definition;
                txtDefinition.ForeColor = Color.Black;
            }

            cardContainer.Controls.AddRange(new Control[]
            {
                pnlCardHeader,
                lblFrontLabel, txtTerm,
                lblSwap,
                lblBackLabel, txtDefinition
            });

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
                pnlInputArea.Controls.Add(cardContainer);
                pnlInputArea.Controls.SetChildIndex(cardContainer, buttonIndex);
            }
            else
            {
                pnlInputArea.Controls.Add(cardContainer);
            }
            RefreshFlashcardIndices();
            ApplyUiRangeFilter();
        }

        private void BuildListUI()
        {
            var items = GameDataManager.GetListFromString(_maMNG);
            var rtb = new RichTextBox
            {
                Width = 650,
                Height = 400,
                Font = this.Font,
                Text = string.Join("\n", items)
            };

            var lblGuide = new Label
            {
                Text = "Nhập mỗi mục trên một dòng.",
                Width = 650,
                AutoSize = true,
                ForeColor = Color.Gray
            };

            pnlInputArea.Controls.AddRange(new Control[] { lblGuide, rtb });
        }

        private void BuildMinMaxUI()
        {
            var list = GameDataManager.GetListFromString(_maMNG);

            var pnl = new Panel { Width = 400, Height = 50 };

            var numMin = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 9999,
                Width = 150,
                Dock = DockStyle.Left,
                Font = this.Font,
                Value = list.Count > 0 ? int.Parse(list[0]) : 1
            };

            var numMax = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 10000,
                Width = 150,
                Dock = DockStyle.Right,
                Font = this.Font,
                Value = list.Count > 1 ? int.Parse(list[1]) : 100
            };

            pnl.Controls.AddRange(new Control[] { numMin, numMax });
            pnlInputArea.Controls.Add(pnl);
        }

        #endregion

        #region Điền từ

        /// <summary>
        /// Xây dựng UI cho Fill Blank (MNG07).
        /// </summary>
        private void BuildFillBlankUI()
        {
            pnlInputArea.Controls.Clear();

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

            pnlHeaderSection.Controls.AddRange(new Control[]
            {
                lblHeaderIcon,
                lblHeaderTitle,
                lblHeaderDesc,
                lblSubDesc
            });
            pnlInputArea.Controls.Add(pnlHeaderSection);

            if (pnlFilterBar.Parent != pnlInputArea)
            {
                pnlFilterBar.Parent?.Controls.Remove(pnlFilterBar);
                pnlInputArea.Controls.Add(pnlFilterBar);
            }

            var spacer = new Panel
            {
                Width = 680,
                Height = 4,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 0)
            };
            pnlInputArea.Controls.Add(spacer);

            var questions = GameDataManager.GetFillBlankQuestions(_maMNG);

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
            RefreshFillBlankIndices();
        }

        private void RefreshFillBlankIndices()
        {
            try
            {
                var fillControls = pnlInputArea.Controls
                    .OfType<FillBlankControl>()
                    .ToList();

                for (int i = 0; i < fillControls.Count; i++)
                {
                    fillControls[i].SetIndex(i + 1);
                }
            }
            catch
            {
                // silent
            }
        }

        /// <summary>
        /// Thêm FillBlankControl.
        /// </summary>
        /// <param name="data">FillBlankQuestion hoặc null.</param>
        private void AddFillBlankControl(FillBlankQuestion data)
        {
            var fbc = new FillBlankControl();

            if (data != null)
            {
                fbc.SetData(data);
            }

            fbc.BtnRemove.Click += (s, e) =>
            {
                pnlInputArea.Controls.Remove(fbc);
                RefreshFillBlankIndices();
            };

            pnlInputArea.Controls.Add(fbc);
            

            int btnAddIndex = -1;

            for (int i = 0; i < pnlInputArea.Controls.Count; i++)
            {
                if (pnlInputArea.Controls[i] is RoundedButton btn && btn.Text.Contains("Thêm câu hỏi"))
                {
                    btnAddIndex = i;
                    break;
                }
            }

            if (btnAddIndex >= 0)
            {
                pnlInputArea.Controls.SetChildIndex(fbc, btnAddIndex);
            }
            RefreshFillBlankIndices();
            ApplyUiRangeFilter();
        }

        #endregion

        #region Sắp xếp câu

        /// <summary>
        /// Xây dựng UI cho Sentence Scramble (MNG06).
        /// </summary>
        private void BuildSentenceScrambleUI()
        {
            pnlInputArea.AutoScroll = true;
            pnlInputArea.Padding = new Padding(20);

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

            Label lblTitle = new Label
            {
                Text = "Tạo Câu Mẫu Sắp Xếp",
                Font = new Font("Lexend", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 31, 137),
                Location = new Point(90, 25),
                AutoSize = true
            };

            Label lblCount = new Label
            {
                Name = "COUNT_LABEL",
                Text = "📊 Đã nhập: 0 câu",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Location = new Point(90, 60),
                AutoSize = true
            };

            pnlHeader.Controls.AddRange(new Control[]
            {
                lblIcon,
                lblTitle,
                lblCount
            });
            pnlInputArea.Controls.Add(pnlHeader);

            if (pnlFilterBar.Parent != pnlInputArea)
            {
                pnlFilterBar.Parent?.Controls.Remove(pnlFilterBar);
                pnlInputArea.Controls.Add(pnlFilterBar);
            }

            var spacer = new Panel
            {
                Width = 680,
                Height = 4,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 0)
            };
            pnlInputArea.Controls.Add(spacer);

            var items = GameDataManager.GetSentenceScrambleItems(_maMNG);
            var existingText = string.Join("\n", items.Select((item, index) => $"{index + 1}. {item.CorrectSentence}"));
            _sentenceFullText = existingText;

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

            txtSentences.TextChanged += (s, e) =>
            {
                if (txtSentences.ForeColor == Color.Gray)
                {
                    return;
                }

                var lines = txtSentences.Lines
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Where(line => !line.Trim().StartsWith("Ví dụ:"))
                    .ToList();

                var countLabel = pnlHeader.Controls.Cast<Control>()
                    .FirstOrDefault(c => c.Name == "COUNT_LABEL") as Label;

                if (countLabel != null)
                {
                    countLabel.Text = $"📊 Đã nhập: {lines.Count} câu";
                    countLabel.ForeColor = lines.Count > 0 ? Color.FromArgb(40, 167, 69) : Color.FromArgb(0, 123, 255);
                }

                var charLabel = pnlInputArea.Controls.Cast<Control>()
                    .SelectMany(c => c.Controls.Cast<Control>())
                    .FirstOrDefault(c => c.Name == "CHAR_COUNT") as Label;

                if (charLabel != null)
                {
                    charLabel.Text = $"Tổng số ký tự: {txtSentences.Text.Length}";
                }
            };

            txtSentences.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && txtSentences.ForeColor != Color.Gray)
                {
                    e.SuppressKeyPress = true;
                    int currentLineCount = txtSentences.Lines
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .Count();

                    txtSentences.AppendText(Environment.NewLine + $"{currentLineCount + 1}. ");
                }
            };

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

            Label lblCharCount = new Label
            {
                Name = "CHAR_COUNT",
                Text = $"Tổng số ký tự: {txtSentences.Text.Length}",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 405),
                AutoSize = true
            };

            // Tạo card chứa danh sách để consistent với layout khác
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

            Label lblListTitle = new Label
            {
                Text = "📝 Danh sách câu mẫu:",
                Font = new Font("Lexend", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(20, 15),
                AutoSize = true
            };

            pnlInputCard.Controls.AddRange(new Control[] { lblListTitle, txtSentences, lblCharCount });
            pnlInputArea.Controls.Add(pnlInputCard);
            ApplyUiRangeFilter();
        }

        #endregion

        #region Save / Play / Load Buttons

        /// <summary>
        /// Lưu dữ liệu hiện tại xuống GameDataManager (theo type).
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool anySaved = false;
                switch (_maMNG.ToUpper())
                {
                    case "MNG01":
                        var allQuizControls = pnlInputArea.Controls.OfType<QuizQuestionControl>().ToList();
                        var validQuizQuestions = new List<QuizQuestion>();

                        // 1. Lặp qua TẤT CẢ các câu hỏi để kiểm tra tính đầy đủ
                        foreach (var qc in allQuizControls)
                        {
                            var data = qc.GetData();

                            // Kiểm tra Câu hỏi (nếu trống/placeholder)
                            if (string.IsNullOrWhiteSpace(data.QuestionText))
                            {
                                MessageBox.Show("Câu hỏi không được để trống! Vui lòng nhập đầy đủ nội dung cho tất cả các mục Quiz.", "Lỗi nhập liệu Quiz", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return; // Dừng và ngăn lưu
                            }

                            // Kiểm tra 4 Đáp án (A, B, C, D)
                            if (data.Options == null || data.Options.Count < 4 || data.Options.Any(o => string.IsNullOrWhiteSpace(o)))
                            {
                                MessageBox.Show($"Các đáp án (A, B, C, D) không được để trống! Vui lòng nhập đầy đủ nội dung cho tất cả các mục Quiz.", "Lỗi nhập liệu Quiz", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            validQuizQuestions.Add(data);
                        }

                        // 2. Nếu không có lỗi, tiến hành lưu và THÔNG BÁO THÀNH CÔNG
                        GameDataManager.SaveQuizQuestions(_maMNG, validQuizQuestions);
                        break; ;

                    case "MNG04":
                        var wsControls = pnlInputArea.Controls.OfType<WordScrambleControl>().ToList();

                        var wsList = new List<WordScrambleItem>();

                        for (int i = 0; i < wsControls.Count; i++)
                        {
                            var wc = wsControls[i];

                            if (!wc.TryGetData(out var wsItem, out var wsMsg))
                            {
                                MessageBox.Show($"Mục #{i + 1}: {wsMsg}", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                // focus offending field
                                if (string.IsNullOrWhiteSpace(wsItem?.Question))
                                {
                                    wc.TxtQuestion.Focus();
                                }
                                else
                                {
                                    wc.TxtAnswer.Focus();
                                }

                                return; // abort save
                            }

                            wsList.Add(wsItem);
                        }

                        GameDataManager.SaveWordScrambleItems(_maMNG, wsList);
                        anySaved = true;
                        break;

                    case "MNG03":
                        // Validate flashcard panels before saving.
                        var flashcardPanels = pnlInputArea.Controls
                            .OfType<Panel>()
                            .Where(p => p.Tag != null && p.Tag.ToString() == "FLASHCARD_PANEL")
                            .ToList();

                        var flashcardsToSave = new List<FlashcardItem>();

                        for (int i = 0; i < flashcardPanels.Count; i++)
                        {
                            var panel = flashcardPanels[i];
                            var txtTerm = panel.Controls.OfType<TextBox>().FirstOrDefault(t => t.Tag?.ToString() == "TERM");
                            var txtDef = panel.Controls.OfType<TextBox>().FirstOrDefault(t => t.Tag?.ToString() == "DEFINITION");

                            // Treat placeholder/gray text as empty (PlaceholderProvider uses Gray)
                            string term = txtTerm == null ? string.Empty : (txtTerm.ForeColor == Color.Gray ? string.Empty : txtTerm.Text?.Trim() ?? string.Empty);
                            string def = txtDef == null ? string.Empty : (txtDef.ForeColor == Color.Gray ? string.Empty : txtDef.Text?.Trim() ?? string.Empty);

                            if (string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(def))
                            {
                                MessageBox.Show($"Thẻ #{i + 1} chưa nhập đầy đủ. Vui lòng nhập cả \"Thuật ngữ\" và \"Định nghĩa\".", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                if (string.IsNullOrWhiteSpace(term) && txtTerm != null)
                                {
                                    txtTerm.Focus();
                                }
                                else if (string.IsNullOrWhiteSpace(def) && txtDef != null)
                                {
                                    txtDef.Focus();
                                }

                                return;
                            }

                            flashcardsToSave.Add(new FlashcardItem
                            {
                                Term = term,
                                Definition = def
                            });
                        }

                        GameDataManager.SaveFlashcardItems(_maMNG, flashcardsToSave);
                        break;

                    case "MNG02":
                    case "MNG08":
                        GameDataManager.SaveListToString(_maMNG, pnlInputArea.Controls.OfType<RichTextBox>().First().Text.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim()).ToList());
                        break;

                    case "MNG09":
                        var panelMinMax = pnlInputArea.Controls.OfType<Panel>().First();
                        GameDataManager.SaveListToString(_maMNG, new List<string>
                        {
                            panelMinMax.Controls.OfType<NumericUpDown>().First().Value.ToString(),
                            panelMinMax.Controls.OfType<NumericUpDown>().Last().Value.ToString()
                        });
                        break;

                    case "MNG06":
                        var txtSentences = FindControlByName(pnlInputArea, "SENTENCES_INPUT") as RichTextBox;

                        if (txtSentences == null)
                        {
                            MessageBox.Show("Không tìm thấy ô nhập câu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var sentenceText = (_filterStart.HasValue && _filterEnd.HasValue && !string.IsNullOrEmpty(_sentenceFullText))
                            ? _sentenceFullText
                            : txtSentences.Text ?? "";
                        var sentences = sentenceText.Split('\n')
                                                    .Where(line => !string.IsNullOrWhiteSpace(line)
                                                                && !line.Trim().StartsWith("Ví dụ:")
                                                                && !line.Trim().StartsWith("Nhập mỗi câu"))
                                                    .Select(line =>
                                                    {
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
                        var fillControls = pnlInputArea.Controls.OfType<FillBlankControl>().ToList();

                        var fillList = new List<FillBlankQuestion>();

                        for (int i = 0; i < fillControls.Count; i++)
                        {
                            var fbc = fillControls[i];
                            if (!fbc.TryGetData(out var fItem, out var fMsg))
                            {
                                MessageBox.Show($"Câu #{i + 1}: {fMsg}", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                if (string.IsNullOrWhiteSpace(fItem?.QuestionText)) fbc.TxtQuestion.Focus();
                                else fbc.TxtAnswer.Focus();

                                return; // abort save
                            }

                            fillList.Add(fItem);
                        }

                        GameDataManager.SaveFillBlankQuestions(_maMNG, fillList);
                        anySaved = true;
                        break;
                }
                MessageBox.Show("Lưu dữ liệu thành công!", "Thành công");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu. Vui lòng kiểm tra lại định dạng đã nhập.\nChi tiết: " + ex.Message, "Lỗi");
            }
        }

        /// <summary>
        /// Mở màn chơi tương ứng với dữ liệu đã lưu.
        /// Áp dụng bộ lọc (nếu có) là Range [start..end] (1-based).
        /// </summary>
        private void BtnPlay_Click(object sender, EventArgs e)
        {
            Form gameForm = null;
            bool launchingGame = false;

            try
            {
                switch (_maMNG.ToUpper())
                {
                    case "MNG01":
                        var quiz = GameDataManager.GetQuizQuestions(_maMNG);
                        quiz = ApplyRangeFilter(quiz);
                        if (!quiz.Any())
                        {
                            MessageBox.Show("Không có câu nào được chọn để chơi. Kiểm tra bộ lọc hoặc nhập dữ liệu.", "Chưa có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        gameForm = new QuizGameForm(quiz);
                        launchingGame = true;
                        break;

                    case "MNG02":
                        var list02 = GameDataManager.GetListFromString(_maMNG);
                        list02 = ApplyRangeFilter(list02);
                        if (!list02.Any())
                        {
                            MessageBox.Show("Không có mục nào được chọn để chơi. Kiểm tra bộ lọc hoặc nhập dữ liệu.", "Chưa có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        gameForm = new LuckyWheelForm(list02);
                        launchingGame = true;
                        break;

                    case "MNG03":
                        var flash = GameDataManager.GetFlashcardItems(_maMNG);
                        flash = ApplyRangeFilter(flash);
                        if (!flash.Any())
                        {
                            MessageBox.Show("Không có thẻ nào được chọn để chơi. Kiểm tra bộ lọc hoặc nhập dữ liệu.", "Chưa có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        gameForm = new FlashcardForm(flash);
                        launchingGame = true;
                        break;

                    case "MNG04":
                        var ws = GameDataManager.GetWordScrambleItems(_maMNG);
                        ws = ApplyRangeFilter(ws);
                        if (!ws.Any())
                        {
                            MessageBox.Show("Không có mục nào được chọn để chơi. Kiểm tra bộ lọc hoặc nhập dữ liệu.", "Chưa có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        gameForm = new GheChuForm(ws);
                        launchingGame = true;
                        break;

                    case "MNG05":
                        var listen = GameDataManager.GetListenChooseItems(_maMNG);
                        if (!listen.Any())
                        {
                            MessageBox.Show("Chưa có dữ liệu cho game này. Vui lòng nhập dữ liệu trước khi chơi.", "Chưa có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        gameForm = new NgheChonHinhForm(listen);
                        launchingGame = true;
                        break;

                    case "MNG06":
                        var sc = GameDataManager.GetSentenceScrambleItems(_maMNG);
                        sc = ApplyRangeFilter(sc);
                        if (!sc.Any())
                        {
                            MessageBox.Show("Không có câu nào được chọn để chơi. Kiểm tra bộ lọc hoặc nhập dữ liệu.", "Chưa có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        gameForm = new SapXepCauForm(sc);
                        launchingGame = true;
                        break;

                    case "MNG07":
                        var fill = GameDataManager.GetFillBlankQuestions(_maMNG);
                        fill = ApplyRangeFilter(fill);
                        if (!fill.Any())
                        {
                            MessageBox.Show("Không có câu nào được chọn để chơi. Kiểm tra bộ lọc hoặc nhập dữ liệu.", "Chưa có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        gameForm = new DienTuForm(fill);
                        launchingGame = true;
                        break;

                    case "MNG08":
                        var list08 = GameDataManager.GetListFromString(_maMNG);
                        list08 = ApplyRangeFilter(list08);
                        if (!list08.Any())
                        {
                            MessageBox.Show("Không có mục nào được chọn để chơi. Kiểm tra bộ lọc hoặc nhập dữ liệu.", "Chưa có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        gameForm = new LatTheForm(list08);
                        launchingGame = true;
                        break;

                    case "MNG09":
                        var list09 = GameDataManager.GetListFromString(_maMNG);
                        list09 = ApplyRangeFilter(list09);
                        if (!list09.Any())
                        {
                            MessageBox.Show("Không có mục nào được chọn để chơi. Kiểm tra bộ lọc hoặc nhập dữ liệu.", "Chưa có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        gameForm = new RandomSoForm(list09);
                        launchingGame = true;
                        break;

                    case "MNG10":
                        var quizForPassBall = GameDataManager.GetQuizQuestions(_maMNG);
                        quizForPassBall = ApplyRangeFilter(quizForPassBall);
                        if (!quizForPassBall.Any())
                        {
                            MessageBox.Show("Không có câu nào được chọn để chơi. Kiểm tra bộ lọc hoặc nhập dữ liệu.", "Chưa có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        gameForm = new PassBallForm(quizForPassBall);
                        launchingGame = true;
                        break;

                    default:
                        MessageBox.Show($"Game '{_tenMNG}' chưa có màn hình chơi.", "Thông báo");
                        return;
                }

                // Nếu có gameForm thì khởi động như trước; nếu không (vì trả về do thiếu dữ liệu) thì form nhập liệu vẫn ở lại
                if (launchingGame && gameForm != null && !gameForm.IsDisposed)
                {
                    this.Hide();
                    gameForm.ShowDialog();
                    // giữ hành vi cũ: sau khi đóng màn chơi, đóng form soạn hoặc quay về tuỳ logic ứng dụng
                    if (!this.IsDisposed)
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể khởi động game: " + ex.Message, "Lỗi");
            }
        }

        #endregion

        #region Filter: range dialog + helpers

        private List<T> ApplyRangeFilter<T>(List<T> items)
        {
            if (items == null || items.Count == 0) return new List<T>();
            if (!_filterStart.HasValue || !_filterEnd.HasValue) return new List<T>(items);

            int start = Math.Max(1, _filterStart.Value);
            int end = Math.Min(_filterEnd.Value, items.Count);
            if (start > end) return new List<T>();

            return items.Skip(start - 1).Take(end - start + 1).ToList();
        }

        private List<string> ApplyRangeFilter(List<string> items) => ApplyRangeFilter<string>(items);
        private List<QuizQuestion> ApplyRangeFilter(List<QuizQuestion> items) => ApplyRangeFilter<QuizQuestion>(items);
        private List<FlashcardItem> ApplyRangeFilter(List<FlashcardItem> items) => ApplyRangeFilter<FlashcardItem>(items);
        private List<WordScrambleItem> ApplyRangeFilter(List<WordScrambleItem> items) => ApplyRangeFilter<WordScrambleItem>(items);
        private List<SentenceScrambleItem> ApplyRangeFilter(List<SentenceScrambleItem> items) => ApplyRangeFilter<SentenceScrambleItem>(items);
        private List<FillBlankQuestion> ApplyRangeFilter(List<FillBlankQuestion> items) => ApplyRangeFilter<FillBlankQuestion>(items);

        private void UpdateFilterLabel()
        {
            if (_filterStart.HasValue && _filterEnd.HasValue)
            {
                lblFilterSummary.Text = $"Đã lọc: Bắt đầu chơi từ Câu {_filterStart.Value} → Câu {_filterEnd.Value}";
            }
            else
            {
                lblFilterSummary.Text = "";
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            int total = 0;
            try
            {
                switch (_maMNG.ToUpper())
                {
                    case "MNG01":
                        total = GameDataManager.GetQuizQuestions(_maMNG).Count;
                        break;
                    case "MNG02":
                    case "MNG08":
                    case "MNG09":
                        total = GameDataManager.GetListFromString(_maMNG).Count;
                        break;
                    case "MNG03":
                        total = GameDataManager.GetFlashcardItems(_maMNG).Count;
                        break;
                    case "MNG04":
                        total = GameDataManager.GetWordScrambleItems(_maMNG).Count;
                        break;
                    case "MNG06":
                        total = GameDataManager.GetSentenceScrambleItems(_maMNG).Count;
                        break;
                    case "MNG07":
                        total = GameDataManager.GetFillBlankQuestions(_maMNG).Count;
                        break;
                    default:
                        total = 0;
                        break;
                }
            }
            catch
            {
                total = 0;
            }

            using (var dlg = new FilterRangeForm(total, _filterStart, _filterEnd))
            {
                var res = dlg.ShowDialog(this);
                if (res == DialogResult.OK)
                {
                    _filterStart = dlg.StartIndex;
                    _filterEnd = dlg.EndIndex;
                }
                else if (res == DialogResult.Abort) // Clear filter
                {
                    _filterStart = null;
                    _filterEnd = null;
                }
                UpdateFilterLabel();
                ApplyUiRangeFilter();
            }
        }

        /// <summary>
        /// Chọn chỉ mục bắt đầu/kết thúc (bắt đầu từ 1)
        /// </summary>
        private class FilterRangeForm : Form
        {
            public int? StartIndex { get; private set; }
            public int? EndIndex { get; private set; }

            private NumericUpDown numStart;
            private NumericUpDown numEnd;
            private Button btnOk;
            private Button btnCancel;
            private Button btnClear;
            private int _total;

            public FilterRangeForm(int totalItems, int? currentStart, int? currentEnd)
            {
                _total = totalItems;
                this.Text = "Bộ lọc (Chọn khoảng câu)";
                this.Size = new Size(360, 180);
                this.StartPosition = FormStartPosition.CenterParent;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;

                Label lblInfo = new Label
                {
                    Text = totalItems > 0 ? $"Tổng: {totalItems} câu. Chọn khoảng 1..{totalItems}" : "Chưa có dữ liệu để lọc.",
                    Location = new Point(12, 8),
                    AutoSize = true
                };

                numStart = new NumericUpDown
                {
                    Minimum = 1,
                    Maximum = Math.Max(1, totalItems),
                    Value = currentStart.HasValue ? Math.Min(Math.Max(1, currentStart.Value), Math.Max(1, totalItems)) : 1,
                    Location = new Point(12, 36),
                    Width = 140
                };

                numEnd = new NumericUpDown
                {
                    Minimum = 1,
                    Maximum = Math.Max(1, totalItems),
                    Value = currentEnd.HasValue ? Math.Min(Math.Max(1, currentEnd.Value), Math.Max(1, totalItems)) : Math.Max(1, totalItems),
                    Location = new Point(180, 36),
                    Width = 140
                };

                Label lblDash = new Label
                {
                    Text = "→",
                    Location = new Point(158, 40),
                    AutoSize = true
                };

                btnOk = new Button
                {
                    Text = "Áp dụng",
                    Location = new Point(12, 80),
                    Width = 100,
                    Enabled = totalItems > 0
                };

                btnCancel = new Button
                {
                    Text = "Hủy",
                    Location = new Point(128, 80),
                    Width = 80
                };

                btnClear = new Button
                {
                    Text = "Xóa bộ lọc",
                    Location = new Point(220, 80),
                    Width = 100,
                    Enabled = currentStart.HasValue || currentEnd.HasValue
                };

                btnOk.Click += (s, e) =>
                {
                    int sIdx = (int)numStart.Value;
                    int eIdx = (int)numEnd.Value;
                    if (sIdx > eIdx)
                    {
                        MessageBox.Show("Khoảng lọc không hợp lệ. Start phải <= End.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    StartIndex = sIdx;
                    EndIndex = eIdx;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };

                btnCancel.Click += (s, e) =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                };

                btnClear.Click += (s, e) =>
                {
                    StartIndex = null;
                    EndIndex = null;
                    this.DialogResult = DialogResult.Abort; // indicate clear
                    this.Close();
                };

                this.Controls.AddRange(new Control[] { lblInfo, numStart, numEnd, lblDash, btnOk, btnCancel, btnClear });
            }
        }

        #endregion

        #region Load from Excel (unchanged)

        /// <summary>
        /// (Placeholder) Khi người dùng muốn tải từ Excel.
        /// </summary>
        private void BtnLoadExcel_Click(object sender, EventArgs e)
        {
            using (var fmt = new ExcelFormatForm(_maMNG))
            {
                var dlgRes = fmt.ShowDialog(this);
                if (dlgRes != DialogResult.OK)
                {
                    return;
                }
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel Files|*.xlsx;*.xls;*.csv|All Files|*.*";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    System.Data.DataTable dataTable = ExcelHelper.ReadExcelFile(ofd.FileName);

                    if (dataTable == null || dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("File Excel trống hoặc không đọc được dữ liệu.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // kiểm tra số cột PHÙ HỢP CHÍNH XÁC theo loại game
                    int fileCols = dataTable.Columns.Count;
                    int? expectedCols = _maMNG.ToUpper() switch
                    {
                        "MNG01" => 6, // Quiz
                        "MNG03" => 2, // Flashcard
                        "MNG04" => 3, // Word Scramble
                        "MNG06" => 1, // Sentence Scramble
                        "MNG07" => 2, // Fill Blank
                        _ => null
                    };

                    if (expectedCols.HasValue && fileCols != expectedCols.Value)
                    {
                        MessageBox.Show(
                            $"Số cột ({fileCols} cột) trong file không phù hợp cho game \"{_tenMNG}\". Game này yêu cầu chính xác {expectedCols.Value} cột. Vui lòng chọn file đúng định dạng hoặc tải mẫu trước khi import.",
                            "Lỗi định dạng file",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    int itemsCount = 0;
                    var skipMessages = new System.Collections.Generic.List<string>();

                    switch (_maMNG)
                    {
                        case "MNG01": // QUIZ (6 cột)
                            {
                                var existing = GameDataManager.GetQuizQuestions(_maMNG) ?? new System.Collections.Generic.List<QuizQuestion>();
                                var newItems = new System.Collections.Generic.List<QuizQuestion>();

                                for (int r = 0; r < dataTable.Rows.Count; r++)
                                {
                                    var row = dataTable.Rows[r];
                                    int rowIndex = r + 1;

                                    // now fileCols == expectedCols, chỉ validate nội dung
                                    string q = row[0]?.ToString()?.Trim() ?? "";
                                    string a = row[1]?.ToString()?.Trim() ?? "";
                                    string b = row[2]?.ToString()?.Trim() ?? "";
                                    string c = row[3]?.ToString()?.Trim() ?? "";
                                    string d = row[4]?.ToString()?.Trim() ?? "";
                                    string correct = row[5]?.ToString()?.Trim().ToUpper() ?? "";

                                    if (string.IsNullOrWhiteSpace(q))
                                    {
                                        skipMessages.Add($"Câu {rowIndex}: Cột 'Câu hỏi' rỗng.");
                                        continue;
                                    }

                                    if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b) || string.IsNullOrWhiteSpace(c) || string.IsNullOrWhiteSpace(d))
                                    {
                                        skipMessages.Add($"Câu {rowIndex}: Một trong các đáp án A/B/C/D bị rỗng.");
                                        continue;
                                    }

                                    if (!(new[] { "A", "B", "C", "D" }.Contains(correct)))
                                    {
                                        skipMessages.Add($"Câu {rowIndex}: Giá trị 'Đáp án đúng' không hợp lệ (phải A/B/C/D).");
                                        continue;
                                    }

                                    newItems.Add(new QuizQuestion
                                    {
                                        QuestionText = q,
                                        Options = new System.Collections.Generic.List<string> { a, b, c, d },
                                        CorrectAnswer = correct
                                    });
                                }

                                existing.AddRange(newItems);
                                GameDataManager.SaveQuizQuestions(_maMNG, existing);
                                itemsCount = newItems.Count;
                                break;
                            }

                        case "MNG03": // FLASHCARD (2 cột)
                            {
                                var existing = GameDataManager.GetFlashcardItems(_maMNG) ?? new System.Collections.Generic.List<FlashcardItem>();
                                var newItems = new System.Collections.Generic.List<FlashcardItem>();

                                for (int r = 0; r < dataTable.Rows.Count; r++)
                                {
                                    var row = dataTable.Rows[r];
                                    int rowIndex = r + 1;

                                    string term = row[0]?.ToString()?.Trim() ?? "";
                                    string def = row[1]?.ToString()?.Trim() ?? "";

                                    if (string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(def))
                                    {
                                        skipMessages.Add($"Câu {rowIndex}: Thuật ngữ hoặc Định nghĩa rỗng.");
                                        continue;
                                    }

                                    newItems.Add(new FlashcardItem { Term = term, Definition = def });
                                }

                                existing.AddRange(newItems);
                                GameDataManager.SaveFlashcardItems(_maMNG, existing);
                                itemsCount = newItems.Count;
                                break;
                            }

                        case "MNG04": // WORD SCRAMBLE (3 cột)
                            {
                                var existing = GameDataManager.GetWordScrambleItems(_maMNG) ?? new System.Collections.Generic.List<WordScrambleItem>();
                                var newItems = new System.Collections.Generic.List<WordScrambleItem>();

                                for (int r = 0; r < dataTable.Rows.Count; r++)
                                {
                                    var row = dataTable.Rows[r];
                                    int rowIndex = r + 1;

                                    string img = row[0]?.ToString()?.Trim() ?? "";
                                    string question = row[1]?.ToString()?.Trim() ?? "";
                                    string answer = row[2]?.ToString()?.Trim() ?? "";

                                    if (string.IsNullOrWhiteSpace(answer))
                                    {
                                        skipMessages.Add($"Câu {rowIndex}: Đáp án trống (bắt buộc).");
                                        continue;
                                    }

                                    if (string.IsNullOrWhiteSpace(question))
                                    {
                                        skipMessages.Add($"Câu {rowIndex}: Câu hỏi/Gợi ý trống.");
                                        continue;
                                    }

                                    newItems.Add(new WordScrambleItem
                                    {
                                        ImageHintResourceName = img,
                                        Question = question,
                                        Answer = answer.ToUpper()
                                    });
                                }

                                existing.AddRange(newItems);
                                GameDataManager.SaveWordScrambleItems(_maMNG, existing);
                                itemsCount = newItems.Count;
                                break;
                            }

                        case "MNG06": // SENTENCE SCRAMBLE (1 cột)
                            {
                                var existing = GameDataManager.GetSentenceScrambleItems(_maMNG) ?? new System.Collections.Generic.List<SentenceScrambleItem>();
                                var newItems = new System.Collections.Generic.List<SentenceScrambleItem>();

                                for (int r = 0; r < dataTable.Rows.Count; r++)
                                {
                                    var row = dataTable.Rows[r];
                                    int rowIndex = r + 1;

                                    string raw = row[0]?.ToString()?.Trim() ?? "";
                                    if (string.IsNullOrWhiteSpace(raw))
                                    {
                                        skipMessages.Add($"Câu {rowIndex}: Câu rỗng.");
                                        continue;
                                    }

                                    string cleaned = Regex.Replace(raw, @"^\d+\.\s*", "");
                                    newItems.Add(new SentenceScrambleItem { CorrectSentence = cleaned });
                                }

                                existing.AddRange(newItems);
                                GameDataManager.SaveSentenceScrambleItems(_maMNG, existing);
                                itemsCount = newItems.Count;
                                break;
                            }

                        case "MNG07": // FILL BLANK (2 cột)
                            {
                                var existing = GameDataManager.GetFillBlankQuestions(_maMNG) ?? new System.Collections.Generic.List<FillBlankQuestion>();
                                var newItems = new System.Collections.Generic.List<FillBlankQuestion>();

                                for (int r = 0; r < dataTable.Rows.Count; r++)
                                {
                                    var row = dataTable.Rows[r];
                                    int rowIndex = r + 1;

                                    string q = row[0]?.ToString()?.Trim() ?? "";
                                    string ans = row[1]?.ToString()?.Trim() ?? "";

                                    if (string.IsNullOrWhiteSpace(q))
                                    {
                                        skipMessages.Add($"Câu {rowIndex}: Câu hỏi rỗng.");
                                        continue;
                                    }

                                    if (string.IsNullOrWhiteSpace(ans))
                                    {
                                        skipMessages.Add($"Câu {rowIndex}: Đáp án rỗng.");
                                        continue;
                                    }

                                    newItems.Add(new FillBlankQuestion { QuestionText = q, Answer = ans });
                                }

                                existing.AddRange(newItems);
                                GameDataManager.SaveFillBlankQuestions(_maMNG, existing);
                                itemsCount = newItems.Count;
                                break;
                            }

                        default:
                            MessageBox.Show($"Game chưa được hỗ trợ nhập liệu Excel.", "Lỗi");
                            return;
                    }

                    if (skipMessages.Count == 0)
                    {
                        MessageBox.Show($"Đã nhập {itemsCount} bản ghi cho game.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string first = skipMessages[0];
                        try
                        {
                            System.Windows.Forms.Clipboard.SetText(string.Join(Environment.NewLine, skipMessages));
                        }
                        catch { }

                        MessageBox.Show($"Đã nhập {itemsCount} bản ghi. Bỏ qua {skipMessages.Count} câu do lỗi/thiếu dữ liệu.\n\nVí dụ: {first}.", "Hoàn tất với cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    BuildInputUI();
                }
                catch (System.IO.InvalidDataException idEx)
                {
                    MessageBox.Show(idEx.Message, "Lỗi Định dạng File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi không xác định trong quá trình xử lý: {ex.Message}", "Lỗi Hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Delete All (unchanged)

        /// <summary>
        /// Xóa tất cả dữ liệu đã lưu cho các mini-game đã chọn
        /// </summary>
        private void BtnDeleteAll_Click(object sender, EventArgs e)
        {
            try
            {
                bool hasData = false;

                switch (_maMNG.ToUpper())
                {
                    case "MNG01": // Quiz
                        var quiz = GameDataManager.GetQuizQuestions(_maMNG);
                        hasData = quiz != null && quiz.Any();
                        break;
                    case "MNG03": // Flashcard
                        var flash = GameDataManager.GetFlashcardItems(_maMNG);
                        hasData = flash != null && flash.Any();
                        break;
                    case "MNG04": // Ghép chữ
                        var ws = GameDataManager.GetWordScrambleItems(_maMNG);
                        hasData = ws != null && ws.Any();
                        break;
                    case "MNG06": // Sắp xếp câu
                        var sc = GameDataManager.GetSentenceScrambleItems(_maMNG);
                        hasData = sc != null && sc.Any();
                        break;
                    case "MNG07": // Điền từ
                        var fill = GameDataManager.GetFillBlankQuestions(_maMNG);
                        hasData = fill != null && fill.Any();
                        break;
                    default:
                        MessageBox.Show("Chức năng Xóa tất cả hiện chưa hỗ trợ game này.", "Không hỗ trợ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                }

                if (!hasData)
                {
                    MessageBox.Show("Không có dữ liệu để xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirm = MessageBox.Show($"Bạn có chắc muốn xóa toàn bộ dữ liệu cũ của game \"{_tenMNG}\"? Hành động này không thể hoàn tác.", "Xác nhận xóa tất cả", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes) return;

                switch (_maMNG.ToUpper())
                {
                    case "MNG01": // Quiz
                        GameDataManager.SaveQuizQuestions(_maMNG, new List<QuizQuestion>());
                        break;
                    case "MNG03": // Flashcard
                        GameDataManager.SaveFlashcardItems(_maMNG, new List<FlashcardItem>());
                        break;
                    case "MNG04": // Ghép chữ
                        GameDataManager.SaveWordScrambleItems(_maMNG, new List<WordScrambleItem>());
                        break;
                    case "MNG06": // Sắp xếp câu
                        GameDataManager.SaveSentenceScrambleItems(_maMNG, new List<SentenceScrambleItem>());
                        break;
                    case "MNG07": // Điền từ
                        GameDataManager.SaveFillBlankQuestions(_maMNG, new List<FillBlankQuestion>());
                        break;
                }

                MessageBox.Show("Đã xóa toàn bộ dữ liệu cũ.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BuildInputUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Tìm control theo Name trong cây controls (đệ quy).
        /// </summary>
        /// <param name="parent">Control gốc để tìm.</param>
        /// <param name="name">Tên control cần tìm.</param>
        /// <returns>Control nếu tìm thấy, ngược lại null.</returns>
        private Control FindControlByName(Control parent, string name)
        {
            if (parent.Name == name)
            {
                return parent;
            }

            foreach (Control child in parent.Controls)
            {
                Control found = FindControlByName(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Áp dụng phạm vi hiện tại (bắt đầu từ 1) cho giao diện nhập liệu.
        /// </summary>
        private void ApplyUiRangeFilter()
        {
            if (pnlInputArea == null) return;

            try
            {
                if (!_filterStart.HasValue || !_filterEnd.HasValue)
                {
                    switch (_maMNG.ToUpper())
                    {
                        case "MNG01":
                            pnlInputArea.Controls.OfType<QuizQuestionControl>().ToList().ForEach(c => c.Visible = true);
                            break;
                        case "MNG03":
                            pnlInputArea.Controls.OfType<Panel>().Where(p => p.Tag != null && p.Tag.ToString() == "FLASHCARD_PANEL").ToList().ForEach(p => p.Visible = true);
                            break;
                        case "MNG04":
                            pnlInputArea.Controls.OfType<WordScrambleControl>().ToList().ForEach(c => c.Visible = true);
                            break;
                        case "MNG07":
                            pnlInputArea.Controls.OfType<FillBlankControl>().ToList().ForEach(c => c.Visible = true);
                            break;
                        default:
                            break;
                    }
                    return;
                }

                int start = Math.Max(1, _filterStart.Value);
                int end = Math.Max(start, _filterEnd.Value);

                switch (_maMNG.ToUpper())
                {
                    case "MNG01":
                        var quizControls = pnlInputArea.Controls.OfType<QuizQuestionControl>().ToList();
                        for (int i = 0; i < quizControls.Count; i++)
                        {
                            int oneBased = i + 1;
                            quizControls[i].Visible = oneBased >= start && oneBased <= end;
                        }
                        break;

                    case "MNG03":
                        var flashPanels = pnlInputArea.Controls.OfType<Panel>().Where(p => p.Tag != null && p.Tag.ToString() == "FLASHCARD_PANEL").ToList();
                        for (int i = 0; i < flashPanels.Count; i++)
                        {
                            int oneBased = i + 1;
                            flashPanels[i].Visible = oneBased >= start && oneBased <= end;
                        }
                        break;

                    case "MNG04":
                        var wsControls = pnlInputArea.Controls.OfType<WordScrambleControl>().ToList();
                        for (int i = 0; i < wsControls.Count; i++)
                        {
                            int oneBased = i + 1;
                            wsControls[i].Visible = oneBased >= start && oneBased <= end;
                        }
                        break;
                    case "MNG06":
                        var txt = FindControlByName(pnlInputArea, "SENTENCES_INPUT") as RichTextBox;

                        if (txt == null) break;
                        if (string.IsNullOrEmpty(_sentenceFullText))
                        {
                            _sentenceFullText = txt.Text ?? "";
                        }

                        if (!_filterStart.HasValue || !_filterEnd.HasValue)
                       {
                            txt.Text = _sentenceFullText;
                        }
                        else
                        {
                            start = Math.Max(1, _filterStart.Value);
                            end = Math.Max(start, _filterEnd.Value);
                            var allLines = _sentenceFullText
                                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(line => Regex.Replace(line.Trim(), @"^\d+\.\s*", ""))
                                .ToList();

                            end = Math.Min(end, allLines.Count);
                            if (start > end)
                            {
                                txt.Text = "";
                            }
                            else
                            {
                                var filtered = allLines.Skip(start - 1).Take(end - start + 1)
                                    .Select((line, idx) => $"{start + idx}. {line}");
                                txt.Text = string.Join(Environment.NewLine, filtered);
                            }
                        }
                        break;
                    case "MNG07":
                        var fillControls = pnlInputArea.Controls.OfType<FillBlankControl>().ToList();
                        for (int i = 0; i < fillControls.Count; i++)
                        {
                            int oneBased = i + 1;
                            fillControls[i].Visible = oneBased >= start && oneBased <= end;
                        }
                        break;

                    default:
                        // other types (single control editors) don't have per-item UI to hide
                        break;
                }
            }
            catch
            {
                // silently fail — do not break user flow
            }
        }

        #endregion
    }
}