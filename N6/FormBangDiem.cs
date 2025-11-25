using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

namespace N6
{
    public partial class FormBangDiem : Form
    {
        // Controls
        private DataGridView dgvDiem;
        private Panel pnlHeader;
        private Label lblTitle;
        private Button btnClose;

        // Variables for Dragging
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public FormBangDiem(string maHS, string tenHS)
        {
            InitForm();
            SetupUI(tenHS, isArchive: false);
            LoadDataCurrent(maHS);
        }

        public FormBangDiem(string maHoSo, string tenHS, string namHoc)
        {
            InitForm();
            SetupUI($"{tenHS} ({namHoc})", isArchive: true);
            LoadDataArchive(maHoSo);
        }

        private void InitForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(1100, 650); // Tăng kích thước form một chút
            this.Padding = new Padding(1);
            this.BackColor = Color.FromArgb(0, 123, 255);
        }

        private void SetupUI(string titleName, bool isArchive)
        {
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = isArchive ? Color.FromArgb(255, 193, 7) : Color.White };
            pnlHeader.MouseDown += PnlHeader_MouseDown;
            pnlHeader.MouseMove += PnlHeader_MouseMove;
            pnlHeader.MouseUp += PnlHeader_MouseUp;

            lblTitle = new Label
            {
                Text = isArchive ? $"📂 HỒ SƠ LƯU TRỮ: {titleName.ToUpper()}" : $"📊 BẢNG ĐIỂM CHI TIẾT: {titleName.ToUpper()}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = isArchive ? Color.Black : Color.FromArgb(0, 123, 255),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 0, 0, 0)
            };
            lblTitle.MouseDown += PnlHeader_MouseDown;
            lblTitle.MouseMove += PnlHeader_MouseMove;
            lblTitle.MouseUp += PnlHeader_MouseUp;

            btnClose = new Button
            {
                Text = "✕",
                Dock = DockStyle.Right,
                Width = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = isArchive ? Color.Black : Color.Gray,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 13, FontStyle.Regular)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 17, 35);
            btnClose.Click += (s, e) => this.Close();

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(btnClose);

            dgvDiem = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill // Để Fill cho đẹp
            };

            dgvDiem.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvDiem.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvDiem.ColumnHeadersHeight = 50;
            dgvDiem.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvDiem.RowTemplate.Height = 40;

            Panel pnlBody = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(10) };
            pnlBody.Controls.Add(dgvDiem);

            this.Controls.Add(pnlBody);
            this.Controls.Add(pnlHeader);
        }

        private void LoadDataCurrent(string maHS)
        {
            try
            {
                DataTable dt = DatabaseHelper.GetStudentFullTranscript(maHS);
                BindDataToGrid(dt);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void LoadDataArchive(string maHoSo)
        {
            try
            {
                DataTable dt = DatabaseHelper.GetArchivedTranscript(maHoSo);
                BindDataToGrid(dt);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải lịch sử: " + ex.Message); }
        }

        private void BindDataToGrid(DataTable dt)
        {
            dgvDiem.DataSource = dt;

            // Dictionary ánh xạ tên cột SQL sang Tiếng Việt hiển thị
            var headers = new Dictionary<string, string>
            {
                { "TenMon", "Môn Học" },
                // Dữ liệu hiện tại
                { "T1_K1", "T1 (K1)" }, { "T2_K1", "T2" }, { "T3_K1", "T3" }, { "GK1", "Giữa K1" }, { "CK1", "Cuối K1" },
                { "T1_K2", "T1 (K2)" }, { "T2_K2", "T2" }, { "T3_K2", "T3" }, { "GK2", "Giữa K2" }, { "CK2", "Cuối K2" },
                // Dữ liệu lưu trữ
                { "T1 (K1)", "T1 (K1)" }, { "T2 (K1)", "T2" }, { "T3 (K1)", "T3" }, { "Giữa K1", "Giữa K1" }, { "Cuối K1", "Cuối K1" },
                { "T1 (K2)", "T1 (K2)" }, { "T2 (K2)", "T2" }, { "T3 (K2)", "T3" }, { "Giữa K2", "Giữa K2" }, { "Cuối K2", "Cuối K2" },
                // Tổng kết
                { "TB_Nam", "ĐTB Năm" }, { "Nhận Xét Năm", "Nhận Xét" }
            };

            foreach (DataGridViewColumn col in dgvDiem.Columns)
            {
                if (headers.ContainsKey(col.Name))
                {
                    col.HeaderText = headers[col.Name];
                    col.DefaultCellStyle.Alignment = (col.Name == "TenMon" || col.Name.Contains("Nhận Xét"))
                        ? DataGridViewContentAlignment.MiddleLeft
                        : DataGridViewContentAlignment.MiddleCenter;
                }
            }

            // Tinh chỉnh độ rộng đặc biệt
            if (dgvDiem.Columns.Contains("TenMon"))
            {
                dgvDiem.Columns["TenMon"].FillWeight = 150;
                dgvDiem.Columns["TenMon"].DefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
            }

            // Cột Điểm TB Năm nổi bật
            if (dgvDiem.Columns.Contains("TB_Nam"))
            {
                dgvDiem.Columns["TB_Nam"].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgvDiem.Columns["TB_Nam"].DefaultCellStyle.ForeColor = Color.Red;
            }

            dgvDiem.ClearSelection();
        }

        private void PnlHeader_MouseDown(object sender, MouseEventArgs e) { 
            dragging = true; 
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location; 
        }
        private void PnlHeader_MouseMove(object sender, MouseEventArgs e) { if (dragging) { Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint)); this.Location = Point.Add(dragFormPoint, new Size(dif)); } }
        private void PnlHeader_MouseUp(object sender, MouseEventArgs e) { dragging = false; }

        private void InitializeComponent() { this.components = new System.ComponentModel.Container(); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; }
        private System.ComponentModel.IContainer components = null;
    }
}