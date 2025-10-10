partial class InsightCard
{
    private System.ComponentModel.IContainer components = null;
    protected override void Dispose(bool disposing) { if (disposing && (components != null)) { components.Dispose(); } base.Dispose(disposing); }
    private void InitializeComponent()
    {
        this.lblIcon = new System.Windows.Forms.Label();
        this.lblTitle = new System.Windows.Forms.Label();
        this.rtbContent = new System.Windows.Forms.RichTextBox();
        this.SuspendLayout();
        // 
        // lblIcon
        // 
        this.lblIcon.Dock = System.Windows.Forms.DockStyle.Left;
        this.lblIcon.Font = new System.Drawing.Font("Segoe UI Emoji", 24F);
        this.lblIcon.Name = "lblIcon";
        this.lblIcon.Size = new System.Drawing.Size(60, 150);
        this.lblIcon.TabIndex = 0;
        this.lblIcon.Text = "⭐";
        this.lblIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // lblTitle
        // 
        this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
        this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
        this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
        this.lblTitle.Name = "lblTitle";
        this.lblTitle.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
        this.lblTitle.Size = new System.Drawing.Size(240, 40);
        this.lblTitle.TabIndex = 1;
        this.lblTitle.Text = "Tiêu đề";
        this.lblTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        // 
        // rtbContent
        // 
        this.rtbContent.BackColor = System.Drawing.Color.White;
        this.rtbContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.rtbContent.Dock = System.Windows.Forms.DockStyle.Fill;
        this.rtbContent.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.rtbContent.Location = new System.Drawing.Point(65, 45);
        this.rtbContent.Name = "rtbContent";
        this.rtbContent.ReadOnly = true;
        this.rtbContent.Size = new System.Drawing.Size(180, 100);
        this.rtbContent.TabIndex = 2;
        this.rtbContent.Text = "";
        // 
        // InsightCard
        // 
        this.BackColor = System.Drawing.Color.White;
        this.Controls.Add(this.rtbContent);
        this.Controls.Add(this.lblTitle);
        this.Controls.Add(this.lblIcon);
        this.Name = "InsightCard";
        this.Padding = new System.Windows.Forms.Padding(5);
        this.Size = new System.Drawing.Size(250, 150);
        this.ResumeLayout(false);
    }
    private System.Windows.Forms.Label lblIcon;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.RichTextBox rtbContent;
}