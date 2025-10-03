// File: GameDataInputForm.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ExcelDataReader;
using Newtonsoft.Json;

namespace N6
{
    public partial class GameDataInputForm : Form
    {
        private readonly string _maMNG;
        private readonly string _tenMNG;

        public GameDataInputForm(string maMNG, string tenMNG)
        {
            InitializeComponent();
            _maMNG = maMNG;
            _tenMNG = tenMNG;
        }

        private void GameDataInputForm_Load(object sender, EventArgs e)
        {
            this.Text = "Nhập dữ liệu cho game: " + _tenMNG;
            ConfigureDataGridView();
        }

        private void ConfigureDataGridView()
        {
            dgvData.Columns.Clear();
            switch (_maMNG)
            {
                case "MNG01": // Quiz nhanh
                    dgvData.Columns.Add("Question", "Câu hỏi");
                    dgvData.Columns.Add("OptionA", "Lựa chọn A");
                    dgvData.Columns.Add("OptionB", "Lựa chọn B");
                    dgvData.Columns.Add("OptionC", "Lựa chọn C");
                    dgvData.Columns.Add("OptionD", "Lựa chọn D");
                    dgvData.Columns.Add("Answer", "Đáp án đúng (chỉ điền A, B, C hoặc D)");
                    break;

                case "MNG02": // Gọi tên ngẫu nhiên
                case "MNG08": // Lật thẻ
                case "MNG09": // Random số
                    // Dùng chung một cột cho các game có dữ liệu đơn giản
                    dgvData.Columns.Add("Value", "Danh sách (Tên/Số/Từ vựng)");
                    break;

                case "MNG03": // Flashcard
                    dgvData.Columns.Add("Term", "Thuật ngữ (Mặt trước)");
                    dgvData.Columns.Add("Definition", "Định nghĩa (Mặt sau)");
                    break;

                default:
                    dgvData.Columns.Add("Data", "Dữ liệu");
                    break;
            }

            foreach (DataGridViewColumn col in dgvData.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        // =========================================================================
        // KHÔNG CÓ THAY ĐỔI TRONG CÁC HÀM btnLoad, btnSave, btnImportExcel
        // =========================================================================
        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                string jsonData = DatabaseHelper.GetGameData(_maMNG);
                if (string.IsNullOrEmpty(jsonData) || jsonData.Trim() == "null")
                {
                    MessageBox.Show("Chưa có dữ liệu nào được lưu cho game này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonData);
                dgvData.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy dữ liệu từ DataSource nếu có, nếu không thì đọc từ grid
                DataTable dt;
                if (dgvData.DataSource is DataTable)
                {
                    dt = (DataTable)dgvData.DataSource;
                }
                else
                {
                    dt = new DataTable();
                    foreach (DataGridViewColumn col in dgvData.Columns)
                    {
                        dt.Columns.Add(col.Name, col.ValueType);
                    }

                    foreach (DataGridViewRow row in dgvData.Rows)
                    {
                        if (row.IsNewRow) continue;
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < dgvData.Columns.Count; i++)
                        {
                            dr[i] = row.Cells[i].Value ?? DBNull.Value;
                        }
                        dt.Rows.Add(dr);
                    }
                }

                string jsonData = JsonConvert.SerializeObject(dt, Formatting.Indented);
                DatabaseHelper.SaveGameData(_maMNG, jsonData);
                MessageBox.Show("Lưu dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Excel Workbook|*.xlsx|Excel 97-2003 Workbook|*.xls", ValidateNames = true })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var stream = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                                });
                                DataTable dt = result.Tables[0];

                                // Đảm bảo tên cột trong DataTable từ Excel khớp với tên cột của DataGridView
                                for (int i = 0; i < dgvData.Columns.Count && i < dt.Columns.Count; i++)
                                {
                                    dt.Columns[i].ColumnName = dgvData.Columns[i].Name;
                                }

                                dgvData.DataSource = dt;
                                MessageBox.Show("Nhập dữ liệu từ Excel thành công!\nHãy đảm bảo các cột trong file Excel khớp với các cột trên màn hình.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi đọc file Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        // =========================================================================
        // THAY ĐỔI CHÍNH NẰM TRONG HÀM btnStart_Click DƯỚI ĐÂY
        // =========================================================================

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (dgvData.Rows.Count <= 1 && dgvData.Rows[0].IsNewRow)
            {
                MessageBox.Show("Vui lòng nhập dữ liệu để bắt đầu chơi.", "Yêu cầu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Hide();
            Form gameForm = null;

            try
            {
                switch (_maMNG)
                {
                    case "MNG01": // Quiz Nhanh
                        var questions = new List<QuizQuestion>();
                        foreach (DataGridViewRow row in dgvData.Rows)
                        {
                            if (row.IsNewRow) continue;
                            questions.Add(new QuizQuestion
                            {
                                // SỬA Ở ĐÂY: Dùng Tên Cột tiếng Anh
                                QuestionText = row.Cells["Question"].Value?.ToString(),
                                Options = new List<string>
                                {
                                    row.Cells["OptionA"].Value?.ToString(),
                                    row.Cells["OptionB"].Value?.ToString(),
                                    row.Cells["OptionC"].Value?.ToString(),
                                    row.Cells["OptionD"].Value?.ToString()
                                },
                                CorrectAnswer = row.Cells["Answer"].Value?.ToString()
                            });
                        }
                        gameForm = new QuizGameForm(questions.Where(q => q.Options.All(o => o != null)).ToList());
                        break;

                    case "MNG02": // Gọi tên ngẫu nhiên
                    case "MNG08": // Lật thẻ
                    case "MNG09": // Random Số
                        // Các game này dùng cột tên "Value"
                        var items = dgvData.Rows.Cast<DataGridViewRow>()
                            .Where(r => !r.IsNewRow && r.Cells["Value"].Value != null)
                            .Select(r => r.Cells["Value"].Value.ToString().Trim())
                            .ToList();

                        if (_maMNG == "MNG02") gameForm = new LuckyWheelForm(items);
                        if (_maMNG == "MNG08") gameForm = new LatTheForm(items);
                        if (_maMNG == "MNG09") gameForm = new RandomSoForm(items);
                        break;

                    case "MNG03": // Flashcard
                        var cards = new List<FlashcardItem>();
                        foreach (DataGridViewRow row in dgvData.Rows)
                        {
                            if (row.IsNewRow) continue;
                            cards.Add(new FlashcardItem
                            {
                                // SỬA Ở ĐÂY: Dùng Tên Cột tiếng Anh
                                Term = row.Cells["Term"].Value?.ToString(),
                                Definition = row.Cells["Definition"].Value?.ToString()
                            });
                        }
                        gameForm = new FlashcardForm(cards);
                        break;

                    default:
                        MessageBox.Show("Game này chưa được hiện thực hóa chức năng chơi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Show();
                        return;
                }

                if (gameForm != null)
                {
                    gameForm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi khởi tạo game. Vui lòng kiểm tra lại dữ liệu đầu vào.\nChi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }
            finally
            {
                // Đảm bảo form chỉ đóng khi game được khởi tạo thành công
                if (gameForm != null)
                {
                    this.Close();
                }
            }
        }
    }
}