using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ScottPlot;

namespace N6
{
    public partial class UC_PhanTichAI : UserControl
    {
        private string maGV;
        private bool isFiltersLoading = true;

        public UC_PhanTichAI(string maGiaoVien)
        {
            InitializeComponent();
            maGV = maGiaoVien;
            this.Load += UC_PhanTichAI_Load;
        }

        private void UC_PhanTichAI_Load(object sender, EventArgs e)
        {
            if (this.DesignMode) return;
            LoadFilters();
            isFiltersLoading = false;
            RunAnalysis();
        }

        private void LoadFilters()
        {
            cbScope.Items.Clear();
            cbScope.Items.Add("Các lớp tôi dạy");
            cbScope.Items.Add("Toàn khối");
            cbScope.SelectedIndex = 0;

            DataTable dtMonHoc = DatabaseHelper.GetAllMonHoc();
            DataRow allRow = dtMonHoc.NewRow();
            allRow["MaMon"] = "ALL";
            allRow["TenMon"] = "Tất cả các môn";
            dtMonHoc.Rows.InsertAt(allRow, 0);
            cbMonHoc.DataSource = dtMonHoc;
            cbMonHoc.DisplayMember = "TenMon";
            cbMonHoc.ValueMember = "MaMon";

            cbHocKy.Items.Clear();
            cbHocKy.Items.Add("Học kỳ 1");
            cbHocKy.Items.Add("Học kỳ 2");
            cbHocKy.SelectedIndex = 0;

            cbScope.SelectedIndexChanged += btnPhanTich_Click;
            cbDetail.SelectedIndexChanged += btnPhanTich_Click;
            cbMonHoc.SelectedIndexChanged += btnPhanTich_Click;
            cbHocKy.SelectedIndexChanged += btnPhanTich_Click;
            btnPhanTich.Click += btnPhanTich_Click;
        }

        private void UpdateDetailComboBox()
        {
            cbDetail.SelectedIndexChanged -= btnPhanTich_Click;

            cbDetail.DataSource = null;
            if (cbScope.SelectedIndex == 0)
            {
                var lopDataSource = DatabaseHelper.GetLopByGiaoVien(maGV);
                if (lopDataSource != null && lopDataSource.Rows.Count > 0)
                {
                    cbDetail.DataSource = lopDataSource;
                    cbDetail.DisplayMember = "TenLop";
                    cbDetail.ValueMember = "MaLop";
                }
            }
            else
            {
                var khoiList = new List<string> { "Khối 5", "Khối 4", "Khối 3", "Khối 2", "Khối 1" };
                cbDetail.DataSource = khoiList;
            }

            cbDetail.SelectedIndexChanged += btnPhanTich_Click;
        }

        private void btnPhanTich_Click(object sender, EventArgs e)
        {
            if (isFiltersLoading) return;

            if (sender == cbScope)
            {
                UpdateDetailComboBox();
            }

            RunAnalysis();
        }

        private void RunAnalysis()
        {
            if (cbScope.SelectedItem == null || cbDetail.SelectedItem == null || cbMonHoc.SelectedValue == null || cbHocKy.SelectedItem == null)
                return;

            string phamVi = (cbScope.SelectedIndex == 0) ? "LopGV" : "Khoi";
            string chiTiet = (cbScope.SelectedIndex == 0) ? (cbDetail.SelectedValue?.ToString() ?? "") : (cbDetail.SelectedItem?.ToString() ?? "");
            if (string.IsNullOrEmpty(chiTiet)) return;

            string maMon = cbMonHoc.SelectedValue.ToString();
            int hocKy = cbHocKy.SelectedIndex + 1;

            DataTable scores = DatabaseHelper.GetScoresForAnalysis(maGV, phamVi, chiTiet, maMon, hocKy);

            flowLayoutPanelCards.Controls.Clear();
            formsPlot1.Plot.Clear();

            if (scores == null || scores.Rows.Count == 0)
            {
                formsPlot1.Plot.Title("Không có dữ liệu để phân tích");
                formsPlot1.Refresh();
                System.Windows.Forms.Label noDataLabel = new System.Windows.Forms.Label
                {
                    Text = "Không có dữ liệu phù hợp với lựa chọn của bạn.",
                    Dock = DockStyle.Fill,
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic),
                    ForeColor = System.Drawing.Color.Gray
                };
                flowLayoutPanelCards.Controls.Add(noDataLabel);
                return;
            }

            DisplayInsightCards(scores);
            DrawScoreDistributionChart(scores);
        }

        private void DisplayInsightCards(DataTable scores)
        {
            var dsKhenThuong = AIAnalyzer.TimHocSinhKhenThuong(scores, 8.5);
            var card1 = new InsightCard();
            card1.SetData("⭐", "Thành Tích Xuất Sắc", System.Drawing.Color.FromArgb(230, 255, 230), dsKhenThuong);

            var dsCanQuanTam = AIAnalyzer.TimHocSinhDiemThap(scores, 5.0);
            var card2 = new InsightCard();
            card2.SetData("⚠️", "Cần Quan Tâm", System.Drawing.Color.FromArgb(255, 240, 230), dsCanQuanTam);

            var dsThatThuong = AIAnalyzer.TimHocSinhDiemThatThuong(scores, 2.0);
            var card3 = new InsightCard();
            card3.SetData("📉", "Phong Độ Thất Thường", System.Drawing.Color.FromArgb(230, 240, 255), dsThatThuong);

            flowLayoutPanelCards.Controls.Add(card1);
            flowLayoutPanelCards.Controls.Add(card2);
            flowLayoutPanelCards.Controls.Add(card3);
        }

        private void DrawScoreDistributionChart(DataTable scores)
        {
            formsPlot1.Plot.Clear();

            var distribution = new double[] { 0, 0, 0, 0 };
            var diemList = scores.AsEnumerable().Select(r => r.Field<double?>("Diem") ?? 0).ToList();

            if (!diemList.Any()) { formsPlot1.Refresh(); return; }

            foreach (var diem in diemList)
            {
                if (diem < 5) distribution[0]++;
                else if (diem < 6.5) distribution[1]++;
                else if (diem < 8) distribution[2]++;
                else distribution[3]++;
            }

            var barPlot = formsPlot1.Plot.Add.Bars(distribution);
            barPlot.Color = ScottPlot.Colors.SteelBlue;

            

            string[] labels = { "Yếu (<5)", "TB (5-6.4)", "Khá (6.5-7.9)", "Giỏi (>=8)" };
            double[] positions = Enumerable.Range(0, labels.Length).Select(i => (double)i).ToArray();
            formsPlot1.Plot.Axes.Bottom.SetTicks(positions, labels);

            formsPlot1.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            formsPlot1.Plot.Axes.Bottom.TickLabelStyle.Rotation = 0;

            for (int i = 0; i < distribution.Length; i++)
            {
                if (distribution[i] > 0)
                {
                    var textLabel = formsPlot1.Plot.Add.Text(distribution[i].ToString("0"), i, distribution[i]);
                    textLabel.Bold = true;
                    textLabel.FontSize = 14;
                    textLabel.Color = ScottPlot.Colors.Black;
                    textLabel.Alignment = Alignment.LowerCenter;
                    textLabel.OffsetY = -5;
                }
            }

            formsPlot1.Plot.Title("Biểu Đồ Phân Bố Điểm Số");
            formsPlot1.Plot.YLabel("Số Lượng Học Sinh");
            formsPlot1.Plot.XLabel("");

            var yAxis = formsPlot1.Plot.Axes.Left;
            yAxis.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(5);

            double maxValue = distribution.Any() ? distribution.Max() : 0;
            if (maxValue > 0)
            {
                double limitTop = Math.Ceiling((maxValue + 1) / 5) * 5;
                yAxis.Min = 0;
                yAxis.Max = limitTop;
            }
            else
            {
                yAxis.Min = 0;
                yAxis.Max = 5;
            }

            formsPlot1.Plot.Axes.Right.IsVisible = false;
            formsPlot1.Plot.Axes.Top.IsVisible = false;
            formsPlot1.Plot.Grid.IsVisible = false;

            formsPlot1.Refresh();
        }
    }
}