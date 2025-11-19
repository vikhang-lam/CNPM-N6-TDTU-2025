using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Dialog to show sample Excel/CSV format and allow saving a template.
    /// User can choose "Import from Excel" (DialogResult.OK) to proceed with file selection.
    /// </summary>
    public class ExcelFormatForm : Form
    {
        private readonly string _maMNG;
        private DataGridView dgvSample;
        private Button btnSaveTemplate;
        private Button btnImport;
        private Button btnClose;
        private Label lblDesc;

        public ExcelFormatForm(string maMNG)
        {
            _maMNG = maMNG?.ToUpper() ?? string.Empty;
            InitializeComponent();
            BuildSampleForGame();
        }

        private void InitializeComponent()
        {
            this.Text = "Mẫu Excel / CSV cho nhập liệu";
            this.Size = new Size(760, 420);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9F);

            lblDesc = new Label
            {
                AutoSize = false,
                Width = 720,
                Height = 40,
                Location = new Point(12, 8),
                ForeColor = Color.FromArgb(80, 80, 80)
            };

            dgvSample = new DataGridView
            {
                Location = new Point(12, 56),
                Size = new Size(720, 260),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White
            };

            btnSaveTemplate = new RoundedButton
            {
                Text = "Tải mẫu (CSV)",
                Location = new Point(12, 330),
                Size = new Size(140, 36),
                BackColor = Color.FromArgb(19, 104, 206),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSaveTemplate.FlatAppearance.BorderSize = 0;
            btnSaveTemplate.Click += BtnSaveTemplate_Click;

            btnImport = new RoundedButton
            {
                Text = "Chọn file để tải",
                Location = new Point(520, 330),
                Size = new Size(140, 36),
                BackColor = Color.FromArgb(40, 135, 63),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnImport.FlatAppearance.BorderSize = 0;

            btnClose = new RoundedButton
            {
                Text = "Đóng",
                Location = new Point(660, 330),
                Size = new Size(72, 36),
                BackColor = Color.LightGray,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnClose.FlatAppearance.BorderSize = 0;

            this.Controls.AddRange(new Control[] { lblDesc, dgvSample, btnSaveTemplate, btnImport, btnClose });

            this.AcceptButton = btnImport;
            this.CancelButton = btnClose;
        }

        private void BuildSampleForGame()
        {
            dgvSample.Columns.Clear();
            dgvSample.Rows.Clear();

            // Default description
            lblDesc.Text = "Kiểm tra và tải mẫu trước khi import. Mở 'Chọn file để tải' nếu bạn đã có file Excel/CSV đúng định dạng.";

            switch (_maMNG)
            {
                case "MNG01": // Quiz:  Câu hỏi, Đáp án A, Đáp án B, Đáp án C, Đáp án D, Đáp án đúng
                    lblDesc.Text = "Quiz (Cần có 6 cột): Câu hỏi, Đáp án A, Đáp án B, Đáp án C, Đáp án D, Đáp án đúng";
                    dgvSample.Columns.Add("Question", "Câu hỏi");
                    dgvSample.Columns.Add("OptionA", "Đáp án A");
                    dgvSample.Columns.Add("OptionB", "Đáp án B");
                    dgvSample.Columns.Add("OptionC", "Đáp án C");
                    dgvSample.Columns.Add("OptionD", "Đáp án D");
                    dgvSample.Columns.Add("Correct", "Đáp án đúng (A/B/C/D)");

                    dgvSample.Rows.Add(
                        "What is the capital of France?",
                        "Berlin",
                        "Madrid",
                        "Paris",
                        "Rome",
                        "C"
                    );
                    dgvSample.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    break;

                case "MNG03": // Flashcard: Thuật ngữ, Định nghĩa
                    lblDesc.Text = "Flashcard (Cần có 2 cột): Thuật ngữ, Định nghĩa";
                    dgvSample.Columns.Add("Term", "Thuật ngữ");
                    dgvSample.Columns.Add("Definition", "Định nghĩa");
                    dgvSample.Rows.Add("Cat", "A small domesticated carnivorous mammal.");
                    dgvSample.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    break;

                case "MNG04": // Word Scramble: ImageResourceName, Question, Answer
                    lblDesc.Text = "Ghép chữ (Cần có 3 cột): Đường dẫn tệp hình ảnh, Câu hỏi - Gợi ý, Đáp án";
                    dgvSample.Columns.Add("ImageResourceName", "Đường dẫn tệp hình ảnh");
                    dgvSample.Columns.Add("Question", "Gợi ý / Câu hỏi");
                    dgvSample.Columns.Add("Answer", "Đáp án");
                    dgvSample.Rows.Add("C:\\Users\\Public\\Pictures\\Sample Pictures\\cat_image.png", "Gợi ý: con vật có meo meo", "cat");
                    dgvSample.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    break;

                case "MNG06": // Sentence Scramble: Sentence
                    lblDesc.Text = "Sắp xếp câu  (Cần có 1 cột): Mỗi dòng chứa 1 câu hoàn chỉnh";
                    dgvSample.Columns.Add("Sentence", "Câu hoàn chỉnh");
                    dgvSample.Rows.Add("She likes to read books.");
                    dgvSample.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    break;

                case "MNG07": // Fill Blank: QuestionText, Answer
                    lblDesc.Text = "Fill Blank (Cần có 2 cột): Câu hỏi, Đáp án";
                    dgvSample.Columns.Add("QuestionText", "Câu hỏi (dùng ___ để đánh dấu chỗ trống)");
                    dgvSample.Columns.Add("Answer", "Đáp án");
                    dgvSample.Rows.Add("Con ___ là loài vật quý hiếm", "gấu trúc");
                    dgvSample.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    break;

                default:
                    lblDesc.Text = "Game chưa có mẫu định dạng cụ thể. Bạn có thể mở file Excel/CSV chứa các cột theo yêu cầu game tương ứng.";
                    dgvSample.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    break;
            }
        }

        private void BtnSaveTemplate_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                sfd.FileName = $"{_maMNG}_template.csv";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var csv = BuildCsvFromGrid();
                    File.WriteAllText(sfd.FileName, csv, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                    MessageBox.Show("Đã lưu mẫu thành công.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu file mẫu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string BuildCsvFromGrid()
        {
            var cols = dgvSample.Columns.Cast<DataGridViewColumn>().Select(c => EscapeCsv(c.HeaderText)).ToArray();
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", cols));

            foreach (DataGridViewRow row in dgvSample.Rows)
            {
                var cells = new List<string>();
                foreach (DataGridViewColumn col in dgvSample.Columns)
                {
                    var val = row.Cells[col.Index].Value?.ToString() ?? string.Empty;
                    cells.Add(EscapeCsv(val));
                }
                sb.AppendLine(string.Join(",", cells));
            }

            return sb.ToString();
        }

        private string EscapeCsv(string input)
        {
            if (input == null) return string.Empty;
            if (input.Contains(",") || input.Contains("\"") || input.Contains("\n"))
            {
                return "\"" + input.Replace("\"", "\"\"") + "\"";
            }
            return input;
        }
    }
}