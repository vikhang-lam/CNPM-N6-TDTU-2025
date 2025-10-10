using ScottPlot;
using ScottPlot.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_PhanTichAI : UserControl
    {
        private DataTable allScores;
        private string maGV;

        public UC_PhanTichAI()
        {
            InitializeComponent();
        }

        public UC_PhanTichAI(string maGiaoVien)
        {
            InitializeComponent();
            maGV = maGiaoVien;
            LoadDataAndSetupUI();
        }

        private void LoadDataAndSetupUI()
        {
            allScores = DatabaseHelper.GetAllKetQuaHocTap();

            cbScope.Items.Add("Các lớp tôi dạy");
            cbScope.Items.Add("Toàn khối");
            cbScope.SelectedIndex = 0;
        }

        private void cbScope_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbDetail.DataSource = null;
            cbDetail.Visible = true;

            if (cbScope.SelectedIndex == 0) // Các lớp tôi dạy
            {
                var lopDataSource = DatabaseHelper.GetLopByGiaoVien(maGV);
                if (lopDataSource.Rows.Count > 0)
                {
                    cbDetail.DataSource = lopDataSource;
                    cbDetail.DisplayMember = "TenLop";
                    cbDetail.ValueMember = "MaLop";
                }
            }
            else // Toàn khối
            {
                var khoiList = allScores.AsEnumerable()
                                .Select(r => r.Field<string>("TenLop").Substring(0, 1))
                                .Distinct()
                                .OrderBy(k => k)
                                .Select(k => $"Khối {k}")
                                .ToList();
                if (khoiList.Any())
                {
                    cbDetail.DataSource = khoiList;
                }
            }
        }

        private void btnPhanTich_Click(object sender, EventArgs e)
        {
            DataTable filteredScores = FilterScores();
            if (filteredScores == null || filteredScores.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu điểm để phân tích cho lựa chọn này.");
                flowLayoutPanelCards.Controls.Clear();
                formsPlot1.Plot.Clear();
                formsPlot1.Refresh();
                return;
            }

            DisplayInsightCards(filteredScores);
            DrawScoreDistributionChart(filteredScores);
        }

        private DataTable FilterScores()
        {
            if (cbScope.SelectedIndex < 0 || cbDetail.SelectedItem == null) return null;

            IEnumerable<DataRow> query;

            if (cbScope.SelectedIndex == 0) // Lọc theo lớp
            {
                if (cbDetail.SelectedValue == null) return null;
                string maLop = cbDetail.SelectedValue.ToString();
                query = allScores.AsEnumerable().Where(r => r.Field<string>("MaLop") == maLop);
            }
            else // Lọc theo khối
            {
                string khoi = cbDetail.SelectedItem.ToString().Replace("Khối ", "");
                query = allScores.AsEnumerable().Where(r => r.Field<string>("TenLop").StartsWith(khoi));
            }

            return query.Any() ? query.CopyToDataTable() : allScores.Clone();
        }

        private void DisplayInsightCards(DataTable scores)
        {
            flowLayoutPanelCards.Controls.Clear();

            var dsKhenThuong = AIAnalyzer.TimHocSinhKhenThuong(scores, 8.5);
            var card1 = new InsightCard();
            card1.SetData("⭐", "Đáng Khen Thưởng", Color.FromArgb(230, 255, 230), dsKhenThuong);

            var dsCanQuanTam = AIAnalyzer.TimHocSinhDiemThap(scores, 5.0);
            var card2 = new InsightCard();
            card2.SetData("⚠️", "Cần Quan Tâm", Color.FromArgb(255, 240, 230), dsCanQuanTam);

            var dsThatThuong = AIAnalyzer.TimHocSinhDiemThatThuong(scores, 2.0);
            var card3 = new InsightCard();
            card3.SetData("📉", "Điểm Thất Thường", Color.FromArgb(230, 240, 255), dsThatThuong);

            flowLayoutPanelCards.Controls.Add(card1);
            flowLayoutPanelCards.Controls.Add(card2);
            flowLayoutPanelCards.Controls.Add(card3);
        }

        private void DrawScoreDistributionChart(DataTable scores)
        {
            var distribution = new double[] { 0, 0, 0, 0 }; // Yếu, TB, Khá, Giỏi
            var diemList = scores.AsEnumerable().Select(r => r.Field<double?>("Diem") ?? 0);

            foreach (var diem in diemList)
            {
                if (diem < 4) distribution[0]++;
                else if (diem < 6.5) distribution[1]++;
                else if (diem < 8) distribution[2]++;
                else distribution[3]++;
            }

            formsPlot1.Plot.Clear();

            var barPlot = formsPlot1.Plot.Add.Bars(distribution);
            barPlot.Color = Colors.DodgerBlue;

            string[] labels = { "Yếu (<4)", "TB (4-6.4)", "Khá (6.5-7.9)", "Giỏi (8-10)" };
            formsPlot1.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                Enumerable.Range(0, labels.Length).Select(i => new Tick(i, labels[i])).ToArray());
            formsPlot1.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            formsPlot1.Plot.Axes.Bottom.TickLabelStyle.Rotation = 15;

            for (int i = 0; i < distribution.Length; i++)
            {
                if (distribution[i] > 0)
                {
                    formsPlot1.Plot.Add.Text(distribution[i].ToString(), i, distribution[i])
                        .Label.Alignment = Alignment.LowerCenter;
                }
            }

            formsPlot1.Plot.Title("Phân bổ điểm số");
            formsPlot1.Plot.Axes.Left.Label.Text = "Số lượng học sinh";
            formsPlot.Plot.ShowGrid(false);
            formsPlot1.Plot.Axes.Right.Hide();
            formsPlot1.Plot.Axes.Top.Hide();

            formsPlot1.Refresh();
        }
    }
}