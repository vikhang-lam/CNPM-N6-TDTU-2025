using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ScottPlot;
using System.Diagnostics; // Thêm

namespace N6
{
    /// <summary>
    /// UserControl hiển thị giao diện phân tích AI, sử dụng AIAnalyzer và ScottPlot.
    /// </summary>
    public partial class UC_PhanTichAI : UserControl
    {
        private string maGV;
        private bool isFiltersLoading = true; // Cờ để ngăn RunAnalysis() chạy khi đang tải filter

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
            RunAnalysis(); // Chạy phân tích lần đầu
        }

        #region Initialization & Filter Loading

        /// <summary>
        /// Tải dữ liệu ban đầu cho các ComboBox bộ lọc.
        /// </summary>
        private void LoadFilters()
        {
            // --- Cài đặt Phạm vi (Scope) ---
            cbScope.Items.Clear();
            cbScope.Items.Add("Các lớp tôi dạy");
            cbScope.Items.Add("Toàn khối");
            cbScope.SelectedIndex = 0;

            // --- Cài đặt Môn học ---
            DataTable dtMonHoc = DatabaseHelper.GetAllSubjects();
            DataRow allRow = dtMonHoc.NewRow();
            allRow["MaMon"] = "ALL";
            allRow["TenMon"] = "Tất cả các môn";
            dtMonHoc.Rows.InsertAt(allRow, 0);
            cbMonHoc.DataSource = dtMonHoc;
            cbMonHoc.DisplayMember = "TenMon";
            cbMonHoc.ValueMember = "MaMon";

            // --- Cài đặt Học kỳ ---
            cbHocKy.Items.Clear();
            cbHocKy.Items.Add("Học kỳ 1");
            cbHocKy.Items.Add("Học kỳ 2");
            cbHocKy.SelectedIndex = 0;

            // --- Gán sự kiện (sẽ được gỡ trong Dispose) ---
            cbScope.SelectedIndexChanged += btnPhanTich_Click;
            cbDetail.SelectedIndexChanged += btnPhanTich_Click;
            cbMonHoc.SelectedIndexChanged += btnPhanTich_Click;
            cbHocKy.SelectedIndexChanged += btnPhanTich_Click;
            btnPhanTich.Click += btnPhanTich_Click;
        }

        /// <summary>
        /// Cập nhật ComboBox Chi tiết (Lớp hoặc Khối) dựa trên Phạm vi đã chọn.
        /// </summary>
        private void UpdateDetailComboBox()
        {
            // Tạm gỡ sự kiện để tránh kích hoạt RunAnalysis()
            cbDetail.SelectedIndexChanged -= btnPhanTich_Click;

            cbDetail.DataSource = null;
            if (cbScope.SelectedIndex == 0) // Các lớp tôi dạy
            {
                var lopDataSource = DatabaseHelper.GetClassesByTeacher(maGV);
                if (lopDataSource != null && lopDataSource.Rows.Count > 0)
                {
                    cbDetail.DataSource = lopDataSource;
                    cbDetail.DisplayMember = "TenLop";
                    cbDetail.ValueMember = "MaLop";
                }
            }
            else // Toàn khối
            {
                var khoiList = new List<string> { "Khối 5", "Khối 4", "Khối 3", "Khối 2", "Khối 1" };
                cbDetail.DataSource = khoiList;
            }

            cbDetail.SelectedIndexChanged += btnPhanTich_Click; // Gán lại sự kiện
        }

        #endregion

        #region Analysis & Charting

        /// <summary>
        /// Xử lý sự kiện click nút "Phân tích" (hoặc khi ComboBox thay đổi).
        /// </summary>
        private void btnPhanTich_Click(object sender, EventArgs e)
        {
            if (isFiltersLoading) return; // Không chạy nếu form đang load

            if (sender == cbScope) // Nếu thay đổi phạm vi -> cập nhật chi tiết
            {
                UpdateDetailComboBox();
            }

            RunAnalysis(); // Chạy phân tích
        }

        /// <summary>
        /// Hàm chính: Lấy dữ liệu từ CSDL, gọi AIAnalyzer, và hiển thị kết quả.
        /// </summary>
        private void RunAnalysis()
        {
            // 1. Kiểm tra đầu vào
            if (cbScope.SelectedItem == null || cbDetail.SelectedItem == null || cbMonHoc.SelectedValue == null || cbHocKy.SelectedItem == null)
                return;

            string phamVi = (cbScope.SelectedIndex == 0) ? "LopGV" : "Khoi";
            string chiTiet = (cbScope.SelectedIndex == 0) ? (cbDetail.SelectedValue?.ToString() ?? "") : (cbDetail.SelectedItem?.ToString() ?? "");
            if (string.IsNullOrEmpty(chiTiet)) return;

            string maMon = cbMonHoc.SelectedValue.ToString();
            int hocKy = cbHocKy.SelectedIndex + 1;

            // 2. Lấy dữ liệu
            DataTable scores = DatabaseHelper.GetScoresForAnalysis(maGV, phamVi, chiTiet, maMon, hocKy);

            // 3. Dọn dẹp UI
            DọnDẹpCards();
            formsPlot1.Plot.Clear();

            // 4. Kiểm tra dữ liệu rỗng
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
                    ForeColor = System.Drawing.Color.Gray,
                    Name = "lblNoData" // Đặt tên để Dispose
                };
                flowLayoutPanelCards.Controls.Add(noDataLabel);
                return;
            }

            // 5. Hiển thị kết quả
            DisplayInsightCards(scores);
            DrawScoreDistributionChart(scores);
        }

        /// <summary>
        /// Tạo và hiển thị các thẻ InsightCard (Khen thưởng, Cần quan tâm...).
        /// </summary>
        private void DisplayInsightCards(DataTable scores)
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi DisplayInsightCards: {ex.Message}");
            }
        }

        /// <summary>
        /// Vẽ biểu đồ phân bố điểm số (Yếu, TB, Khá, Giỏi) bằng ScottPlot.
        /// </summary>
        private void DrawScoreDistributionChart(DataTable scores)
        {
            formsPlot1.Plot.Clear();

            var distribution = new double[] { 0, 0, 0, 0 }; // Yếu, TB, Khá, Giỏi
            var diemList = scores.AsEnumerable().Select(r => r.Field<double?>("Diem") ?? 0).ToList();

            if (!diemList.Any())
            {
                formsPlot1.Refresh();
                return;
            }

            // 1. Tổng hợp dữ liệu
            foreach (var diem in diemList)
            {
                if (diem < 5) distribution[0]++;
                else if (diem < 6.5) distribution[1]++;
                else if (diem < 8) distribution[2]++;
                else distribution[3]++;
            }

            // 2. Vẽ biểu đồ cột
            var barPlot = formsPlot1.Plot.Add.Bars(distribution);
            barPlot.Color = ScottPlot.Colors.SteelBlue;

            // 3. Tùy chỉnh trục X (Ticks)
            string[] labels = { "Yếu (<5)", "TB (5-6.4)", "Khá (6.5-7.9)", "Giỏi (>=8)" };
            double[] positions = Enumerable.Range(0, labels.Length).Select(i => (double)i).ToArray();
            formsPlot1.Plot.Axes.Bottom.SetTicks(positions, labels);
            formsPlot1.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            formsPlot1.Plot.Axes.Bottom.TickLabelStyle.Rotation = 0;

            // 4. Thêm nhãn số lượng trên đỉnh cột
            for (int i = 0; i < distribution.Length; i++)
            {
                if (distribution[i] > 0)
                {
                    var textLabel = formsPlot1.Plot.Add.Text(distribution[i].ToString("0"), i, distribution[i]);
                    textLabel.Bold = true;
                    textLabel.FontSize = 12;
                    textLabel.Color = ScottPlot.Colors.Black;
                    textLabel.Alignment = Alignment.LowerCenter;
                    textLabel.OffsetY = -15;
                }
            }

            // 5. Tùy chỉnh chung
            formsPlot1.Plot.Title("Biểu Đồ Phân Bố Điểm Số");
            formsPlot1.Plot.YLabel("Số Lượng Học Sinh");
            formsPlot1.Plot.XLabel("");

            // 6. Tùy chỉnh trục Y (Giới hạn và bước nhảy)
            var yAxis = formsPlot1.Plot.Axes.Left;
            yAxis.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(5); // Bước nhảy là 5

            double maxValue = distribution.Any() ? distribution.Max() : 0;
            if (maxValue > 0)
            {
                double limitTop = Math.Ceiling((maxValue + 1) / 5) * 5; // Làm tròn lên 5
                yAxis.Min = 0;
                yAxis.Max = limitTop;
            }
            else
            {
                yAxis.Min = 0;
                yAxis.Max = 5; // Giá trị tối thiểu
            }

            // 7. Ẩn các trục không cần thiết
            formsPlot1.Plot.Axes.Right.IsVisible = false;
            formsPlot1.Plot.Axes.Top.IsVisible = false;
            formsPlot1.Plot.Grid.IsVisible = false;

            formsPlot1.Refresh();
        }

        #endregion

        #region Dispose & Helpers

        /// <summary>
        /// Dọn dẹp các Card và Label động trong FlowLayoutPanel.
        /// </summary>
        private void DọnDẹpCards()
        {
            // Dùng ToList() để tạo bản sao, tránh lỗi thay đổi collection khi đang duyệt
            foreach (var ctrl in flowLayoutPanelCards.Controls.OfType<Control>().ToList())
            {
                flowLayoutPanelCards.Controls.Remove(ctrl);
                ctrl.Dispose();
            }
        }

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Gỡ bỏ sự kiện (quan trọng)
                this.Load -= UC_PhanTichAI_Load;
                if (cbScope != null) cbScope.SelectedIndexChanged -= btnPhanTich_Click;
                if (cbDetail != null) cbDetail.SelectedIndexChanged -= btnPhanTich_Click;
                if (cbMonHoc != null) cbMonHoc.SelectedIndexChanged -= btnPhanTich_Click;
                if (cbHocKy != null) cbHocKy.SelectedIndexChanged -= btnPhanTich_Click;
                if (btnPhanTich != null) btnPhanTich.Click -= btnPhanTich_Click;

                // Dọn dẹp các card động
                DọnDẹpCards();

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}