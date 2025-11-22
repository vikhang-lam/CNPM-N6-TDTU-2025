using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Control cho một câu hỏi trắc nghiệm (1 câu, 4 đáp án).
    /// Hỗ trợ ẩn/hiện các phần nhập liệu theo bộ lọc (không xóa, chỉ ẩn).
    /// </summary>
    public class QuizQuestionControl : UserControl
    {
        #region Fields / Properties

        // TextBox chứa nội dung câu hỏi.
        public TextBox TxtQuestion { get; private set; }

        // TextBox cho đáp án A.
        public TextBox TxtOptionA { get; private set; }

        // TextBox cho đáp án B.
        public TextBox TxtOptionB { get; private set; }

        // TextBox cho đáp án C.
        public TextBox TxtOptionC { get; private set; }

        // TextBox cho đáp án D.
        public TextBox TxtOptionD { get; private set; }

        // ComboBox chọn đáp án đúng (A/B/C/D).
        public ComboBox CboCorrectAnswer { get; private set; }

        // Nút xóa control.
        public Button BtnRemove { get; private set; }
        private Label lblQuestionLabel;

        // Các label cần truy cập để ẩn/hiện
        private Label lblOptionsLabel;
        private Label lblCorrectLabel;
        private Label lblA;
        private Label lblB;
        private Label lblC;
        private Label lblD;

        // Make header panel a field so we can hide/show it properly
        private Panel pnlHeader;

        #endregion

        #region Constructors

        /// <summary>
        /// Khởi tạo QuizQuestionControl và xây dựng giao diện.
        /// </summary>
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

            BuildUi();
        }

        #endregion

        #region UI Build

        /// <summary>
        /// Xây dựng giao diện chi tiết cho control.
        /// </summary>
        private void BuildUi()
        {
            // ===== PANEL HEADER =====
            pnlHeader = new Panel
            {
                Location = new Point(15, 15),
                Size = new Size(560, 35),
                BackColor = Color.FromArgb(245, 248, 250)
            };

            Label lblQuestionIcon = new Label
            {
                Text = "❓",
                Font = new Font("Segoe UI", 14F, FontStyle.Regular),
                Location = new Point(0, 5),
                AutoSize = true,
                ForeColor = Color.FromArgb(19, 104, 206)
            };

            lblQuestionLabel = new Label
            {
                Text = "Câu hỏi:",
                Font = new Font("Lexend", 10F, FontStyle.Bold),
                Location = new Point(30, 8),
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
            lblOptionsLabel = new Label
            {
                Text = "📝 Các đáp án:",
                Font = new Font("Lexend", 9F, FontStyle.Bold),
                Location = new Point(15, 108),
                AutoSize = true,
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            lblCorrectLabel = new Label
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

            // ===== CÁC TEXTBOX ĐÁP ÁN =====
            int badgeWidth = 20;
            int badgeHeight = 20;
            int textBoxWidth = 285;
            int textBoxHeight = 30;
            int leftColumnX = 15;
            int rightColumnX = 345;

            // Đáp án A
            lblA = new Label
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
            lblB = new Label
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
            lblC = new Label
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
            lblD = new Label
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

        #endregion

        #region Set Data Methods

        /// <summary>
        /// Lấy dữ liệu câu hỏi trắc nghiệm từ control.
        /// Trims values and treats placeholder/gray text as empty.
        /// </summary>
        /// <returns>QuizQuestion chứa câu hỏi, danh sách đáp án và đáp án đúng.</returns>
        public QuizQuestion GetData()
        {
            string question = NormalizeTextBoxText(TxtQuestion);
            var options = new List<string>
            {
                NormalizeTextBoxText(TxtOptionA),
                NormalizeTextBoxText(TxtOptionB),
                NormalizeTextBoxText(TxtOptionC),
                NormalizeTextBoxText(TxtOptionD)
            };

            string correct = CboCorrectAnswer.SelectedItem != null ? CboCorrectAnswer.SelectedItem.ToString() : "";

            return new QuizQuestion
            {
                QuestionText = question,
                Options = options,
                CorrectAnswer = correct
            };
        }

        /// <summary>
        /// Gán dữ liệu cho control từ một QuizQuestion.
        /// </summary>
        /// <param name="q">Dữ liệu câu hỏi.</param>
        public void SetData(QuizQuestion q)
        {
            if (q == null) return;

            TxtQuestion.Text = q.QuestionText ?? "";
            TxtQuestion.ForeColor = string.IsNullOrWhiteSpace(q.QuestionText) ? Color.Gray : Color.Black;

            if (q.Options != null && q.Options.Count >= 4)
            {
                TxtOptionA.Text = q.Options[0] ?? "";
                TxtOptionA.ForeColor = string.IsNullOrWhiteSpace(q.Options[0]) ? Color.Gray : Color.Black;

                TxtOptionB.Text = q.Options[1] ?? "";
                TxtOptionB.ForeColor = string.IsNullOrWhiteSpace(q.Options[1]) ? Color.Gray : Color.Black;

                TxtOptionC.Text = q.Options[2] ?? "";
                TxtOptionC.ForeColor = string.IsNullOrWhiteSpace(q.Options[2]) ? Color.Gray : Color.Black;

                TxtOptionD.Text = q.Options[3] ?? "";
                TxtOptionD.ForeColor = string.IsNullOrWhiteSpace(q.Options[3]) ? Color.Gray : Color.Black;
            }

            if (!string.IsNullOrWhiteSpace(q.CorrectAnswer) && CboCorrectAnswer.Items.Contains(q.CorrectAnswer))
            {
                CboCorrectAnswer.SelectedItem = q.CorrectAnswer;
            }
        }

        #endregion

        #region Helpers

        private string NormalizeTextBoxText(TextBox tb)
        {
            if (tb == null) return string.Empty;
            if (tb.ForeColor == Color.Gray)
            {
                return string.Empty;
            }

            return tb.Text?.Trim() ?? string.Empty;
        }

        private int CorrectAnswerIndex(string answer)
        {
            switch ((answer ?? "").Trim().ToUpper())
            {
                case "A": return 0;
                case "B": return 1;
                case "C": return 2;
                case "D": return 3;
                default: return -1;
            }
        }

        /// <summary>
        /// Thiết lập nhãn chỉ mục hiển thị (bắt đầu từ 1) cho câu hỏi
        /// </summary>
        public void SetIndex(int index)
        {
            if (lblQuestionLabel == null) return;
            lblQuestionLabel.Text = index > 0 ? $"Câu hỏi {index}:" : "Câu hỏi:";
        }

        #endregion

        #region Filtering API (ẩn/hiện các phần nhập liệu)

        /// <summary>
        /// Hiển thị chỉ các trường được truyền vào, ẩn các trường còn lại.
        /// Tên trường (case-insensitive): "Question", "OptionA"/"A", "OptionB"/"B", "OptionC"/"C", "OptionD"/"D", "CorrectAnswer", "Header".
        /// Nếu không truyền tên => hiển thị tất cả.
        /// Việc ẩn chỉ đặt Visible = false (không xóa dữ liệu).
        /// </summary>
        /// <param name="fieldNames">Tên các phần muốn hiển thị</param>
        public void ShowOnlyFields(params string[] fieldNames)
        {
            // Ensure call happens on UI thread
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowOnlyFields(fieldNames)));
                return;
            }

            // If no filter provided => show all
            if (fieldNames == null || fieldNames.Length == 0)
            {
                SetAllVisible(true);
                AdjustLayout();
                return;
            }

            var set = new HashSet<string>(fieldNames.Select(f => (f ?? "").Trim().ToUpperInvariant()));

            bool showHeader = set.Contains("HEADER");
            bool showQuestion = set.Contains("QUESTION");
            bool showOptionA = set.Contains("OPTIONA") || set.Contains("A");
            bool showOptionB = set.Contains("OPTIONB") || set.Contains("B");
            bool showOptionC = set.Contains("OPTIONC") || set.Contains("C");
            bool showOptionD = set.Contains("OPTIOND") || set.Contains("D");
            bool showCorrect = set.Contains("CORRECTANSWER") || set.Contains("CORRECT");

            // Header panel and remove button
            if (pnlHeader != null) pnlHeader.Visible = showHeader;
            if (lblQuestionLabel != null) lblQuestionLabel.Visible = showHeader; // redundant if pnlHeader hidden, but safe
            if (BtnRemove != null) BtnRemove.Visible = showHeader;

            // Question textbox
            if (TxtQuestion != null) TxtQuestion.Visible = showQuestion;

            // Options label row + combo
            bool anyOptionVisible = showOptionA || showOptionB || showOptionC || showOptionD;
            if (lblOptionsLabel != null) lblOptionsLabel.Visible = anyOptionVisible;
            if (lblCorrectLabel != null) lblCorrectLabel.Visible = showCorrect;
            if (CboCorrectAnswer != null) CboCorrectAnswer.Visible = showCorrect;

            // Individual option badges + textboxes
            if (lblA != null) lblA.Visible = showOptionA;
            if (TxtOptionA != null) TxtOptionA.Visible = showOptionA;

            if (lblB != null) lblB.Visible = showOptionB;
            if (TxtOptionB != null) TxtOptionB.Visible = showOptionB;

            if (lblC != null) lblC.Visible = showOptionC;
            if (TxtOptionC != null) TxtOptionC.Visible = showOptionC;

            if (lblD != null) lblD.Visible = showOptionD;
            if (TxtOptionD != null) TxtOptionD.Visible = showOptionD;

            AdjustLayout();
        }

        /// <summary>
        /// Hiển thị hoặc ẩn tất cả control (dùng để reset filter).
        /// </summary>
        private void SetAllVisible(bool visible)
        {
            if (pnlHeader != null) pnlHeader.Visible = visible;
            if (lblQuestionLabel != null) lblQuestionLabel.Visible = visible;
            if (BtnRemove != null) BtnRemove.Visible = visible;

            if (TxtQuestion != null) TxtQuestion.Visible = visible;

            if (lblOptionsLabel != null) lblOptionsLabel.Visible = visible;
            if (lblCorrectLabel != null) lblCorrectLabel.Visible = visible;
            if (CboCorrectAnswer != null) CboCorrectAnswer.Visible = visible;

            if (lblA != null) lblA.Visible = visible;
            if (TxtOptionA != null) TxtOptionA.Visible = visible;

            if (lblB != null) lblB.Visible = visible;
            if (TxtOptionB != null) TxtOptionB.Visible = visible;

            if (lblC != null) lblC.Visible = visible;
            if (TxtOptionC != null) TxtOptionC.Visible = visible;

            if (lblD != null) lblD.Visible = visible;
            if (TxtOptionD != null) TxtOptionD.Visible = visible;
        }

        /// <summary>
        /// Điều chỉnh chiều cao control dựa trên các phần đang hiển thị.
        /// Giúp giảm khoảng trắng khi một số phần bị ẩn.
        /// </summary>
        private void AdjustLayout()
        {
            // Base paddings and sizes (phải đồng bộ với BuildUi)
            int topPadding = 15;
            int headerHeight = (pnlHeader != null && pnlHeader.Visible) ? 35 : 0;
            int spacingAfterHeader = headerHeight > 0 ? 10 : 0;

            int questionHeight = (TxtQuestion != null && TxtQuestion.Visible) ? 32 : 0;
            int spacingAfterQuestion = questionHeight > 0 ? 10 : 0;

            // Options label row (kèm combobox)
            int optionsLabelRowHeight = (lblOptionsLabel != null && (lblOptionsLabel.Visible || (lblCorrectLabel != null && lblCorrectLabel.Visible) || (CboCorrectAnswer != null && CboCorrectAnswer.Visible))) ? 28 : 0;
            int spacingAfterOptionsLabel = optionsLabelRowHeight > 0 ? 10 : 0;

            // Option rows: two rows of textboxes (A/B) and (C/D)
            int optionRowHeight = 0;
            bool topRowVisible = (TxtOptionA != null && TxtOptionA.Visible) || (TxtOptionB != null && TxtOptionB.Visible);
            bool bottomRowVisible = (TxtOptionC != null && TxtOptionC.Visible) || (TxtOptionD != null && TxtOptionD.Visible);
            if (topRowVisible) optionRowHeight += 30;
            if (bottomRowVisible) optionRowHeight += 30;
            int spacingBetweenRows = (topRowVisible && bottomRowVisible) ? 10 : 0;
            int spacingAfterOptionRows = optionRowHeight > 0 ? 15 : 0;

            int bottomPadding = 15;

            int newHeight = topPadding
                            + headerHeight + spacingAfterHeader
                            + questionHeight + spacingAfterQuestion
                            + optionsLabelRowHeight + spacingAfterOptionsLabel
                            + optionRowHeight + spacingBetweenRows + spacingAfterOptionRows
                            + bottomPadding;

            // Ensure a minimum height so border and layout still look correct
            int minHeight = 80;
            this.Height = Math.Max(newHeight, minHeight);

            this.PerformLayout();
            this.Invalidate();
        }

        #endregion
    }
}