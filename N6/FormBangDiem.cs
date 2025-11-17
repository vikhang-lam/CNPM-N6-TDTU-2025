using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public partial class FormBangDiem : Form
    {
        // Controls
        private DataGridView dgvDiem;
        private Panel pnlHeader;
        private Label lblTitle;
        private Button btnClose;

        // Variables for Dragging (Kéo thả form)
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public FormBangDiem(string maHS, string tenHS)
        {
            InitializeComponent();

            // Cấu hình Form không viền (Modern Style)
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(900, 500);
            this.Padding = new Padding(1); // Tạo viền mỏng
            this.BackColor = Color.FromArgb(0, 123, 255); // Màu viền bao quanh

            SetupUI(tenHS);
            LoadData(maHS);
        }

        private void SetupUI(string tenHS)
        {
            // 1. Header Panel
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.White
            };
            pnlHeader.MouseDown += PnlHeader_MouseDown;
            pnlHeader.MouseMove += PnlHeader_MouseMove;
            pnlHeader.MouseUp += PnlHeader_MouseUp;

            // 2. Title Label
            lblTitle = new Label
            {
                Text = $"BẢNG ĐIỂM CHI TIẾT: {tenHS.ToUpper()}",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 0, 0, 0)
            };
            lblTitle.MouseDown += PnlHeader_MouseDown;
            lblTitle.MouseMove += PnlHeader_MouseMove;
            lblTitle.MouseUp += PnlHeader_MouseUp;

            // 3. Close Button 
            btnClose = new Button
            {
                Text = "✕",
                Dock = DockStyle.Right,
                Width = 45,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Gray, // Màu mặc định
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 12, FontStyle.Regular)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 17, 35); // Màu nền khi hover (Đỏ)

            // Xử lý đổi màu chữ thủ công bằng sự kiện
            btnClose.MouseEnter += (s, e) => btnClose.ForeColor = Color.White;
            btnClose.MouseLeave += (s, e) => btnClose.ForeColor = Color.Gray;

            btnClose.Click += (s, e) => this.Close();

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(btnClose);

            // 4. DataGridView
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Style Header
            dgvDiem.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvDiem.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvDiem.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvDiem.ColumnHeadersHeight = 45;
            dgvDiem.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // Style Cell
            dgvDiem.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvDiem.DefaultCellStyle.ForeColor = Color.Black;
            dgvDiem.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 242, 255);
            dgvDiem.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvDiem.DefaultCellStyle.Padding = new Padding(5);
            dgvDiem.RowTemplate.Height = 40;
            dgvDiem.GridColor = Color.WhiteSmoke;
            dgvDiem.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            Panel pnlBody = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            pnlBody.Controls.Add(dgvDiem);

            this.Controls.Add(pnlBody);
            this.Controls.Add(pnlHeader);
        }

        #region Drag Form Logic
        private void PnlHeader_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void PnlHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void PnlHeader_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        #endregion

        private void LoadData(string maHS)
        {
            try
            {
                dgvDiem.DataSource = DatabaseHelper.GetStudentFullTranscript(maHS);

                var headers = new Dictionary<string, string>
                {
                    { "TenMon", "Môn Học" },
                    { "T1_K1", "T1" }, { "T2_K1", "T2" }, { "T3_K1", "T3" }, { "GK1", "GK" }, { "CK1", "CK" },
                    { "T1_K2", "T1" }, { "T2_K2", "T2" }, { "T3_K2", "T3" }, { "GK2", "GK" }, { "CK2", "CK" },
                    { "TB_Nam", "TB Năm" }
                };

                foreach (DataGridViewColumn col in dgvDiem.Columns)
                {
                    if (headers.ContainsKey(col.Name))
                    {
                        col.HeaderText = headers[col.Name];
                        if (col.Name != "TenMon")
                        {
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }
                }

                // Cấu hình lại kích thước cột
                if (dgvDiem.Columns.Contains("TenMon"))
                {
                    dgvDiem.Columns["TenMon"].FillWeight = 150;
                    dgvDiem.Columns["TenMon"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    dgvDiem.Columns["TenMon"].DefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
                }

                foreach (var key in headers.Keys)
                {
                    if (key != "TenMon" && dgvDiem.Columns.Contains(key))
                    {
                        dgvDiem.Columns[key].FillWeight = 45;
                    }
                }

                if (dgvDiem.Columns.Contains("TB_Nam"))
                {
                    dgvDiem.Columns["TB_Nam"].FillWeight = 60;
                    dgvDiem.Columns["TB_Nam"].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    dgvDiem.Columns["TB_Nam"].DefaultCellStyle.ForeColor = Color.FromArgb(220, 53, 69);
                }

                dgvDiem.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        }
        private System.ComponentModel.IContainer components = null;
    }
}