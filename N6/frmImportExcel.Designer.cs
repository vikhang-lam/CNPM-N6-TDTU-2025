namespace N6
{
    partial class frmImportExcel
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportExcel));
            this.pnlTopBar = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblClose = new System.Windows.Forms.Label();
            this.pnlDropZone = new System.Windows.Forms.Panel();
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblDropHint = new System.Windows.Forms.Label();
            this.picUploadIcon = new System.Windows.Forms.PictureBox();
            this.btnDownloadTemplate = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.pnlResults = new System.Windows.Forms.Panel();
            this.txtErrorLog = new System.Windows.Forms.TextBox();
            this.lblResultStatus = new System.Windows.Forms.Label();
            this.picResultIcon = new System.Windows.Forms.PictureBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.pnlTopBar.SuspendLayout();
            this.pnlDropZone.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUploadIcon)).BeginInit();
            this.pnlResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picResultIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTopBar
            // 
            this.pnlTopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlTopBar.Controls.Add(this.lblTitle);
            this.pnlTopBar.Controls.Add(this.lblClose);
            this.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopBar.Location = new System.Drawing.Point(1, 1);
            this.pnlTopBar.Name = "pnlTopBar";
            this.pnlTopBar.Size = new System.Drawing.Size(598, 40);
            this.pnlTopBar.TabIndex = 0;
            this.pnlTopBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlTopBar_MouseDown);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(165, 23);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Import từ file Excel";
            // 
            // lblClose
            // 
            this.lblClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblClose.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblClose.ForeColor = System.Drawing.Color.Gray;
            this.lblClose.Location = new System.Drawing.Point(558, 0);
            this.lblClose.Name = "lblClose";
            this.lblClose.Size = new System.Drawing.Size(40, 40);
            this.lblClose.TabIndex = 0;
            this.lblClose.Text = "✕";
            this.lblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblClose.Click += new System.EventHandler(this.lblClose_Click);
            // 
            // pnlDropZone
            // 
            this.pnlDropZone.AllowDrop = true;
            this.pnlDropZone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlDropZone.Controls.Add(this.lblFileName);
            this.pnlDropZone.Controls.Add(this.lblDropHint);
            this.pnlDropZone.Controls.Add(this.picUploadIcon);
            this.pnlDropZone.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnlDropZone.Location = new System.Drawing.Point(30, 70);
            this.pnlDropZone.Name = "pnlDropZone";
            this.pnlDropZone.Size = new System.Drawing.Size(540, 150);
            this.pnlDropZone.TabIndex = 1;
            this.pnlDropZone.Click += new System.EventHandler(this.pnlDropZone_Click);
            this.pnlDropZone.DragDrop += new System.Windows.Forms.DragEventHandler(this.pnlDropZone_DragDrop);
            this.pnlDropZone.DragEnter += new System.Windows.Forms.DragEventHandler(this.pnlDropZone_DragEnter);
            // 
            // lblFileName
            // 
            this.lblFileName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblFileName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.lblFileName.Location = new System.Drawing.Point(3, 115);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(534, 23);
            this.lblFileName.TabIndex = 2;
            this.lblFileName.Text = "Chưa có file nào được chọn";
            this.lblFileName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDropHint
            // 
            this.lblDropHint.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDropHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblDropHint.Location = new System.Drawing.Point(3, 75);
            this.lblDropHint.Name = "lblDropHint";
            this.lblDropHint.Size = new System.Drawing.Size(534, 28);
            this.lblDropHint.TabIndex = 1;
            this.lblDropHint.Text = "Kéo và thả file vào đây, hoặc nhấn để chọn file";
            this.lblDropHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picUploadIcon
            // 
            this.picUploadIcon.Image = global::N6.Properties.Resources.upload_icon;
            this.picUploadIcon.Location = new System.Drawing.Point(245, 20);
            this.picUploadIcon.Name = "picUploadIcon";
            this.picUploadIcon.Size = new System.Drawing.Size(50, 50);
            this.picUploadIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picUploadIcon.TabIndex = 0;
            this.picUploadIcon.TabStop = false;
            // 
            // btnDownloadTemplate
            // 
            this.btnDownloadTemplate.BackColor = System.Drawing.Color.White;
            this.btnDownloadTemplate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(226)))), ((int)(((byte)(230)))));
            this.btnDownloadTemplate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownloadTemplate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDownloadTemplate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnDownloadTemplate.Location = new System.Drawing.Point(30, 300);
            this.btnDownloadTemplate.Name = "btnDownloadTemplate";
            this.btnDownloadTemplate.Size = new System.Drawing.Size(340, 33);
            this.btnDownloadTemplate.TabIndex = 2;
            this.btnDownloadTemplate.Text = "📥 Tải file mẫu";
            this.btnDownloadTemplate.UseVisualStyleBackColor = false;
            this.btnDownloadTemplate.Click += new System.EventHandler(this.btnDownloadTemplate_Click);
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnImport.FlatAppearance.BorderSize = 0;
            this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImport.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnImport.ForeColor = System.Drawing.Color.White;
            this.btnImport.Location = new System.Drawing.Point(37, 341);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(540, 55);
            this.btnImport.TabIndex = 3;
            this.btnImport.Text = "Bắt đầu Import";
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // pnlResults
            // 
            this.pnlResults.Controls.Add(this.progressBar);
            this.pnlResults.Controls.Add(this.txtErrorLog);
            this.pnlResults.Controls.Add(this.lblResultStatus);
            this.pnlResults.Controls.Add(this.picResultIcon);
            this.pnlResults.Location = new System.Drawing.Point(30, 70);
            this.pnlResults.Name = "pnlResults";
            this.pnlResults.Size = new System.Drawing.Size(540, 224);
            this.pnlResults.TabIndex = 4;
            this.pnlResults.Visible = false;
            // 
            // txtErrorLog
            // 
            this.txtErrorLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.txtErrorLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtErrorLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtErrorLog.Location = new System.Drawing.Point(20, 73);
            this.txtErrorLog.Multiline = true;
            this.txtErrorLog.Name = "txtErrorLog";
            this.txtErrorLog.ReadOnly = true;
            this.txtErrorLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrorLog.Size = new System.Drawing.Size(517, 135);
            this.txtErrorLog.TabIndex = 2;
            // 
            // lblResultStatus
            // 
            this.lblResultStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblResultStatus.Location = new System.Drawing.Point(3, 47);
            this.lblResultStatus.Name = "lblResultStatus";
            this.lblResultStatus.Size = new System.Drawing.Size(534, 28);
            this.lblResultStatus.TabIndex = 1;
            this.lblResultStatus.Text = "Import hoàn tất!";
            this.lblResultStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picResultIcon
            // 
            this.picResultIcon.Location = new System.Drawing.Point(245, 3);
            this.picResultIcon.Name = "picResultIcon";
            this.picResultIcon.Size = new System.Drawing.Size(50, 44);
            this.picResultIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picResultIcon.TabIndex = 0;
            this.picResultIcon.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(-3, 232);
            this.progressBar.MarqueeAnimationSpeed = 50;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(540, 10);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 5;
            this.progressBar.Visible = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Excel Workbook|*.xlsx;*.xls";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // frmImportExcel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(600, 419);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnDownloadTemplate);
            this.Controls.Add(this.pnlTopBar);
            this.Controls.Add(this.pnlResults);
            this.Controls.Add(this.pnlDropZone);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmImportExcel";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmImportExcel";
            this.Load += new System.EventHandler(this.frmImportExcel_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmImportExcel_Paint);
            this.pnlTopBar.ResumeLayout(false);
            this.pnlTopBar.PerformLayout();
            this.pnlDropZone.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picUploadIcon)).EndInit();
            this.pnlResults.ResumeLayout(false);
            this.pnlResults.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picResultIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTopBar;
        private System.Windows.Forms.Label lblClose;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlDropZone;
        private System.Windows.Forms.PictureBox picUploadIcon;
        private System.Windows.Forms.Label lblDropHint;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Button btnDownloadTemplate;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Panel pnlResults;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.PictureBox picResultIcon;
        private System.Windows.Forms.Label lblResultStatus;
        private System.Windows.Forms.TextBox txtErrorLog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
    }
}