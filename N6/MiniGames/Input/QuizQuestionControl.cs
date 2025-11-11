using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Control cho một câu hỏi trắc nghiệm (1 câu, 4 đáp án).
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

            // ===== CÁC TEXTBOX ĐÁP ÁN =====
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

        #endregion

        #region Set Data Methods

        /// <summary>
        /// Lấy dữ liệu câu hỏi trắc nghiệm từ control.
        /// </summary>
        /// <returns>QuizQuestion chứa câu hỏi, danh sách đáp án và đáp án đúng.</returns>
        public QuizQuestion GetData()
        {
            return new QuizQuestion
            {
                QuestionText = TxtQuestion.Text,
                Options = new List<string> { TxtOptionA.Text, TxtOptionB.Text, TxtOptionC.Text, TxtOptionD.Text },
                CorrectAnswer = CboCorrectAnswer.SelectedItem.ToString()
            };
        }

        /// <summary>
        /// Gán dữ liệu cho control từ một QuizQuestion.
        /// </summary>
        /// <param name="q">Dữ liệu câu hỏi.</param>
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

        #endregion
    }
}