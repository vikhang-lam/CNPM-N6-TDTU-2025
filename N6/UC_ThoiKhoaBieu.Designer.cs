namespace N6
{
    partial class UC_ThoiKhoaBieu
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvTKB;
        private System.Windows.Forms.Label lblWeek;
        private System.Windows.Forms.Button btnPrevWeek;
        private System.Windows.Forms.Button btnNextWeek;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvTKB = new System.Windows.Forms.DataGridView();
            this.lblWeek = new System.Windows.Forms.Label();
            this.btnPrevWeek = new System.Windows.Forms.Button();
            this.btnNextWeek = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTKB)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvTKB
            // 
            this.dgvTKB.AllowUserToAddRows = false;
            this.dgvTKB.AllowUserToDeleteRows = false;
            this.dgvTKB.AllowUserToResizeRows = false;
            this.dgvTKB.BackgroundColor = System.Drawing.Color.White;
            this.dgvTKB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTKB.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvTKB.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTKB.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvTKB.Location = new System.Drawing.Point(0, 60);
            this.dgvTKB.MultiSelect = false;
            this.dgvTKB.Name = "dgvTKB";
            this.dgvTKB.ReadOnly = true;
            this.dgvTKB.RowHeadersWidth = 70;
            this.dgvTKB.RowTemplate.Height = 60;
            this.dgvTKB.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvTKB.Size = new System.Drawing.Size(900, 500);
            this.dgvTKB.TabIndex = 0;
            // 
            // lblWeek
            // 
            this.lblWeek.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblWeek.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblWeek.ForeColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.lblWeek.Location = new System.Drawing.Point(0, 0);
            this.lblWeek.Name = "lblWeek";
            this.lblWeek.Size = new System.Drawing.Size(900, 40);
            this.lblWeek.TabIndex = 1;
            this.lblWeek.Text = "Thời khóa biểu tuần";
            this.lblWeek.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnPrevWeek
            // 
            this.btnPrevWeek.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnPrevWeek.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnPrevWeek.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrevWeek.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrevWeek.Location = new System.Drawing.Point(10, 10);
            this.btnPrevWeek.Name = "btnPrevWeek";
            this.btnPrevWeek.Size = new System.Drawing.Size(80, 35);
            this.btnPrevWeek.TabIndex = 2;
            this.btnPrevWeek.Text = "← Trước";
            this.btnPrevWeek.UseVisualStyleBackColor = false;
            this.btnPrevWeek.Click += new System.EventHandler(this.btnPrevWeek_Click);
            // 
            // btnNextWeek
            // 
            this.btnNextWeek.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnNextWeek.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnNextWeek.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNextWeek.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNextWeek.Location = new System.Drawing.Point(810, 10);
            this.btnNextWeek.Name = "btnNextWeek";
            this.btnNextWeek.Size = new System.Drawing.Size(80, 35);
            this.btnNextWeek.TabIndex = 3;
            this.btnNextWeek.Text = "Sau →";
            this.btnNextWeek.UseVisualStyleBackColor = false;
            this.btnNextWeek.Click += new System.EventHandler(this.btnNextWeek_Click);
            // 
            // UC_ThoiKhoaBieu
            // 
            this.Controls.Add(this.btnNextWeek);
            this.Controls.Add(this.btnPrevWeek);
            this.Controls.Add(this.lblWeek);
            this.Controls.Add(this.dgvTKB);
            this.Name = "UC_ThoiKhoaBieu";
            this.Size = new System.Drawing.Size(900, 560);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTKB)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
