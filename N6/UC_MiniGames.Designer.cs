namespace N6
{
    partial class UC_MiniGames
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.FlowLayoutPanel panelGames;
        private System.Windows.Forms.Label labelTitle;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelGames = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(800, 60);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "🎮 Mini-games";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelGames
            // 
            this.panelGames.AutoScroll = true;
            this.panelGames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGames.Location = new System.Drawing.Point(0, 60);
            this.panelGames.Name = "panelGames";
            this.panelGames.Padding = new System.Windows.Forms.Padding(20);
            this.panelGames.Size = new System.Drawing.Size(800, 540);
            this.panelGames.TabIndex = 1;
            // 
            // UC_MiniGames
            // 
            this.Controls.Add(this.panelGames);
            this.Controls.Add(this.labelTitle);
            this.Name = "UC_MiniGames";
            this.Size = new System.Drawing.Size(800, 600);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
