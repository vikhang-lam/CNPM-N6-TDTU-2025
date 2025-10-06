namespace N6
{
    partial class UC_HoTroGiangDay
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabWhiteboard;
        private System.Windows.Forms.TabPage tabNotes;

        private System.Windows.Forms.Panel panelWhiteboardContainer;
        private System.Windows.Forms.Panel panelCanvas;
        private System.Windows.Forms.ToolStrip toolWhiteboard;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cboWidth;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnColor;
        private System.Windows.Forms.ToolStripButton btnEraser;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.ToolStripButton btnSave;

        private System.Windows.Forms.TextBox txtNotes;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                DisposeCanvas();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabWhiteboard = new System.Windows.Forms.TabPage();
            this.panelWhiteboardContainer = new System.Windows.Forms.Panel();
            this.panelCanvas = new System.Windows.Forms.Panel();
            this.toolWhiteboard = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cboWidth = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnColor = new System.Windows.Forms.ToolStripButton();
            this.btnEraser = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.tabNotes = new System.Windows.Forms.TabPage();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.tabMain.SuspendLayout();
            this.tabWhiteboard.SuspendLayout();
            this.panelWhiteboardContainer.SuspendLayout();
            this.toolWhiteboard.SuspendLayout();
            this.tabNotes.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabWhiteboard);
            this.tabMain.Controls.Add(this.tabNotes);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(900, 600);
            this.tabMain.TabIndex = 0;
            // 
            // tabWhiteboard
            // 
            this.tabWhiteboard.Controls.Add(this.panelWhiteboardContainer);
            this.tabWhiteboard.Location = new System.Drawing.Point(4, 25);
            this.tabWhiteboard.Name = "tabWhiteboard";
            this.tabWhiteboard.Padding = new System.Windows.Forms.Padding(3);
            this.tabWhiteboard.Size = new System.Drawing.Size(892, 571);
            this.tabWhiteboard.TabIndex = 0;
            this.tabWhiteboard.Text = "Bảng trắng";
            this.tabWhiteboard.UseVisualStyleBackColor = true;
            // 
            // panelWhiteboardContainer
            // 
            this.panelWhiteboardContainer.Controls.Add(this.panelCanvas);
            this.panelWhiteboardContainer.Controls.Add(this.toolWhiteboard);
            this.panelWhiteboardContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelWhiteboardContainer.Location = new System.Drawing.Point(3, 3);
            this.panelWhiteboardContainer.Name = "panelWhiteboardContainer";
            this.panelWhiteboardContainer.Size = new System.Drawing.Size(886, 565);
            this.panelWhiteboardContainer.TabIndex = 0;
            // 
            // panelCanvas
            // 
            this.panelCanvas.BackColor = System.Drawing.Color.White;
            this.panelCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCanvas.Location = new System.Drawing.Point(0, 27);
            this.panelCanvas.Name = "panelCanvas";
            this.panelCanvas.Size = new System.Drawing.Size(886, 538);
            this.panelCanvas.TabIndex = 1;
            this.panelCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCanvas_Paint);
            this.panelCanvas.Resize += new System.EventHandler(this.panelCanvas_Resize);
            this.panelCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelCanvas_MouseDown);
            this.panelCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelCanvas_MouseMove);
            this.panelCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelCanvas_MouseUp);
            // 
            // toolWhiteboard
            // 
            this.toolWhiteboard.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolWhiteboard.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.cboWidth,
            this.toolStripSeparator1,
            this.btnColor,
            this.btnEraser,
            this.toolStripSeparator2,
            this.btnClear,
            this.btnSave});
            this.toolWhiteboard.Location = new System.Drawing.Point(0, 0);
            this.toolWhiteboard.Name = "toolWhiteboard";
            this.toolWhiteboard.Size = new System.Drawing.Size(886, 27);
            this.toolWhiteboard.TabIndex = 0;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(36, 24);
            this.toolStripLabel1.Text = "Bút:";
            // 
            // cboWidth
            // 
            this.cboWidth.AutoSize = false;
            this.cboWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWidth.Items.AddRange(new object[] {
            "2",
            "4",
            "6",
            "8",
            "10",
            "14",
            "18"});
            this.cboWidth.Name = "cboWidth";
            this.cboWidth.Size = new System.Drawing.Size(75, 27);
            this.cboWidth.ToolTipText = "Độ dày nét vẽ";
            this.cboWidth.SelectedIndexChanged += new System.EventHandler(this.cboWidth_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnColor
            // 
            this.btnColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(42, 24);
            this.btnColor.Text = "Màu";
            this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnEraser
            // 
            this.btnEraser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEraser.Name = "btnEraser";
            this.btnEraser.Size = new System.Drawing.Size(33, 24);
            this.btnEraser.Text = "Tẩy";
            this.btnEraser.Checked = false;
            this.btnEraser.CheckOnClick = true;
            this.btnEraser.CheckedChanged += new System.EventHandler(this.btnEraser_CheckedChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(62, 24);
            this.btnClear.Text = "Xóa hết";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(65, 24);
            this.btnSave.Text = "Lưu ảnh";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tabNotes
            // 
            this.tabNotes.Controls.Add(this.txtNotes);
            this.tabNotes.Location = new System.Drawing.Point(4, 25);
            this.tabNotes.Name = "tabNotes";
            this.tabNotes.Padding = new System.Windows.Forms.Padding(3);
            this.tabNotes.Size = new System.Drawing.Size(892, 571);
            this.tabNotes.TabIndex = 1;
            this.tabNotes.Text = "Ghi chú nhanh";
            this.tabNotes.UseVisualStyleBackColor = true;
            // 
            // txtNotes
            // 
            this.txtNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNotes.Location = new System.Drawing.Point(3, 3);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(886, 565);
            this.txtNotes.TabIndex = 0;
            // 
            // UC_HoTroGiangDay (UserControl)
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabMain);
            this.Name = "UC_HoTroGiangDay";
            this.Size = new System.Drawing.Size(900, 600);
            this.Load += new System.EventHandler(this.UC_HoTroGiangDay_Load);
            this.tabMain.ResumeLayout(false);
            this.tabWhiteboard.ResumeLayout(false);
            this.panelWhiteboardContainer.ResumeLayout(false);
            this.panelWhiteboardContainer.PerformLayout();
            this.toolWhiteboard.ResumeLayout(false);
            this.toolWhiteboard.PerformLayout();
            this.tabNotes.ResumeLayout(false);
            this.tabNotes.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}