namespace N6
{
    partial class Form_ShareDocument
    {
        private System.ComponentModel.IContainer components = null;

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
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblCloseButton = new System.Windows.Forms.Label();
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelSpecific = new System.Windows.Forms.Panel();
            this.panelTeacherList = new System.Windows.Forms.Panel();
            this.clbTeachers = new System.Windows.Forms.CheckedListBox();
            this.txtSearchTeacher = new System.Windows.Forms.TextBox();
            this.lblSearchIcon = new System.Windows.Forms.Label();
            this.lblSpecificRadio = new System.Windows.Forms.Label();
            this.lblSpecificDesc = new System.Windows.Forms.Label();
            this.lblSpecificTitle = new System.Windows.Forms.Label();
            this.lblSpecificIcon = new System.Windows.Forms.Label();
            this.panelPublic = new System.Windows.Forms.Panel();
            this.lblPublicRadio = new System.Windows.Forms.Label();
            this.lblPublicDesc = new System.Windows.Forms.Label();
            this.lblPublicTitle = new System.Windows.Forms.Label();
            this.lblPublicIcon = new System.Windows.Forms.Label();
            this.panelPrivate = new System.Windows.Forms.Panel();
            this.lblPrivateRadio = new System.Windows.Forms.Label();
            this.lblPrivateDesc = new System.Windows.Forms.Label();
            this.lblPrivateTitle = new System.Windows.Forms.Label();
            this.lblPrivateIcon = new System.Windows.Forms.Label();
            this.panelFooter.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelSpecific.SuspendLayout();
            this.panelTeacherList.SuspendLayout();
            this.panelPublic.SuspendLayout();
            this.panelPrivate.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(119)))), ((int)(((byte)(242)))));
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.FlatAppearance.BorderSize = 0;
            this.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirm.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Location = new System.Drawing.Point(421, 15);
            this.btnConfirm.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(168, 43);
            this.btnConfirm.TabIndex = 6;
            this.btnConfirm.Text = "Lưu";
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Gainsboro;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(38, 15);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(165, 45);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // panelFooter
            // 
            this.panelFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.panelFooter.Controls.Add(this.btnCancel);
            this.panelFooter.Controls.Add(this.btnConfirm);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 597);
            this.panelFooter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(625, 73);
            this.panelFooter.TabIndex = 9;
            this.panelFooter.Paint += new System.Windows.Forms.PaintEventHandler(this.panelFooter_Paint);
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelHeader.Controls.Add(this.lblCloseButton);
            this.panelHeader.Controls.Add(this.lblFileName);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(625, 74);
            this.panelHeader.TabIndex = 10;
            this.panelHeader.Paint += new System.Windows.Forms.PaintEventHandler(this.panelHeader_Paint);
            // 
            // lblCloseButton
            // 
            this.lblCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCloseButton.AutoSize = true;
            this.lblCloseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCloseButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCloseButton.ForeColor = System.Drawing.Color.Gray;
            this.lblCloseButton.Location = new System.Drawing.Point(587, 9);
            this.lblCloseButton.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCloseButton.Name = "lblCloseButton";
            this.lblCloseButton.Size = new System.Drawing.Size(25, 28);
            this.lblCloseButton.TabIndex = 2;
            this.lblCloseButton.Text = "X";
            this.lblCloseButton.Click += new System.EventHandler(this.lblCloseButton_Click);
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblFileName.Location = new System.Drawing.Point(17, 38);
            this.lblFileName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(126, 23);
            this.lblFileName.TabIndex = 1;
            this.lblFileName.Text = "[TenTaiLieu.pdf]";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(156, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Chia sẻ tài liệu:";
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.LightGray;
            this.panelMain.Controls.Add(this.panelSpecific);
            this.panelMain.Controls.Add(this.panelPublic);
            this.panelMain.Controls.Add(this.panelPrivate);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 74);
            this.panelMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(20, 18, 20, 18);
            this.panelMain.Size = new System.Drawing.Size(625, 523);
            this.panelMain.TabIndex = 11;
            // 
            // panelSpecific
            // 
            this.panelSpecific.BackColor = System.Drawing.Color.White;
            this.panelSpecific.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSpecific.Controls.Add(this.panelTeacherList);
            this.panelSpecific.Controls.Add(this.lblSpecificRadio);
            this.panelSpecific.Controls.Add(this.lblSpecificDesc);
            this.panelSpecific.Controls.Add(this.lblSpecificTitle);
            this.panelSpecific.Controls.Add(this.lblSpecificIcon);
            this.panelSpecific.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelSpecific.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSpecific.Location = new System.Drawing.Point(20, 222);
            this.panelSpecific.Margin = new System.Windows.Forms.Padding(4, 4, 4, 12);
            this.panelSpecific.Name = "panelSpecific";
            this.panelSpecific.Padding = new System.Windows.Forms.Padding(13, 12, 13, 12);
            this.panelSpecific.Size = new System.Drawing.Size(585, 356);
            this.panelSpecific.TabIndex = 2;
            this.panelSpecific.Click += new System.EventHandler(this.panelSpecific_Click);
            // 
            // panelTeacherList
            // 
            this.panelTeacherList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTeacherList.BackColor = System.Drawing.Color.White;
            this.panelTeacherList.Controls.Add(this.clbTeachers);
            this.panelTeacherList.Controls.Add(this.txtSearchTeacher);
            this.panelTeacherList.Controls.Add(this.lblSearchIcon);
            this.panelTeacherList.Location = new System.Drawing.Point(17, 92);
            this.panelTeacherList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelTeacherList.Name = "panelTeacherList";
            this.panelTeacherList.Size = new System.Drawing.Size(548, 246);
            this.panelTeacherList.TabIndex = 5;
            // 
            // clbTeachers
            // 
            this.clbTeachers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbTeachers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbTeachers.CheckOnClick = true;
            this.clbTeachers.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clbTeachers.FormattingEnabled = true;
            this.clbTeachers.Location = new System.Drawing.Point(4, 50);
            this.clbTeachers.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.clbTeachers.Name = "clbTeachers";
            this.clbTeachers.Size = new System.Drawing.Size(540, 144);
            this.clbTeachers.TabIndex = 1;
            this.clbTeachers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbTeachers_ItemCheck);
            // 
            // txtSearchTeacher
            // 
            this.txtSearchTeacher.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchTeacher.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtSearchTeacher.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSearchTeacher.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchTeacher.Location = new System.Drawing.Point(40, 12);
            this.txtSearchTeacher.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtSearchTeacher.Name = "txtSearchTeacher";
            this.txtSearchTeacher.Size = new System.Drawing.Size(504, 22);
            this.txtSearchTeacher.TabIndex = 0;
            this.txtSearchTeacher.Text = "Tìm kiếm giáo viên...";
            this.txtSearchTeacher.TextChanged += new System.EventHandler(this.txtSearchTeacher_TextChanged);
            this.txtSearchTeacher.Enter += new System.EventHandler(this.txtSearchTeacher_Enter);
            this.txtSearchTeacher.Leave += new System.EventHandler(this.txtSearchTeacher_Leave);
            // 
            // lblSearchIcon
            // 
            this.lblSearchIcon.AutoSize = true;
            this.lblSearchIcon.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchIcon.Location = new System.Drawing.Point(9, 12);
            this.lblSearchIcon.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSearchIcon.Name = "lblSearchIcon";
            this.lblSearchIcon.Size = new System.Drawing.Size(27, 23);
            this.lblSearchIcon.TabIndex = 2;
            this.lblSearchIcon.Text = "🔎";
            // 
            // lblSpecificRadio
            // 
            this.lblSpecificRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSpecificRadio.AutoSize = true;
            this.lblSpecificRadio.Font = new System.Drawing.Font("Segoe UI Symbol", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecificRadio.Location = new System.Drawing.Point(529, 12);
            this.lblSpecificRadio.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSpecificRadio.Name = "lblSpecificRadio";
            this.lblSpecificRadio.Size = new System.Drawing.Size(38, 32);
            this.lblSpecificRadio.TabIndex = 4;
            this.lblSpecificRadio.Text = "🔘";
            this.lblSpecificRadio.Click += new System.EventHandler(this.panelSpecific_Click);
            // 
            // lblSpecificDesc
            // 
            this.lblSpecificDesc.AutoSize = true;
            this.lblSpecificDesc.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecificDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblSpecificDesc.Location = new System.Drawing.Point(77, 50);
            this.lblSpecificDesc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSpecificDesc.Name = "lblSpecificDesc";
            this.lblSpecificDesc.Size = new System.Drawing.Size(316, 20);
            this.lblSpecificDesc.TabIndex = 3;
            this.lblSpecificDesc.Text = "Chỉ những giáo viên bạn chọn mới có thể xem.";
            this.lblSpecificDesc.Click += new System.EventHandler(this.panelSpecific_Click);
            // 
            // lblSpecificTitle
            // 
            this.lblSpecificTitle.AutoSize = true;
            this.lblSpecificTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecificTitle.Location = new System.Drawing.Point(77, 23);
            this.lblSpecificTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSpecificTitle.Name = "lblSpecificTitle";
            this.lblSpecificTitle.Size = new System.Drawing.Size(135, 23);
            this.lblSpecificTitle.TabIndex = 2;
            this.lblSpecificTitle.Text = "Giáo viên cụ thể";
            this.lblSpecificTitle.Click += new System.EventHandler(this.panelSpecific_Click);
            // 
            // lblSpecificIcon
            // 
            this.lblSpecificIcon.AutoSize = true;
            this.lblSpecificIcon.Font = new System.Drawing.Font("Segoe UI Symbol", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecificIcon.Location = new System.Drawing.Point(17, 23);
            this.lblSpecificIcon.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSpecificIcon.Name = "lblSpecificIcon";
            this.lblSpecificIcon.Size = new System.Drawing.Size(38, 32);
            this.lblSpecificIcon.TabIndex = 1;
            this.lblSpecificIcon.Text = "👥";
            this.lblSpecificIcon.Click += new System.EventHandler(this.panelSpecific_Click);
            // 
            // panelPublic
            // 
            this.panelPublic.BackColor = System.Drawing.Color.White;
            this.panelPublic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPublic.Controls.Add(this.lblPublicRadio);
            this.panelPublic.Controls.Add(this.lblPublicDesc);
            this.panelPublic.Controls.Add(this.lblPublicTitle);
            this.panelPublic.Controls.Add(this.lblPublicIcon);
            this.panelPublic.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelPublic.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelPublic.Location = new System.Drawing.Point(20, 114);
            this.panelPublic.Margin = new System.Windows.Forms.Padding(4, 4, 4, 12);
            this.panelPublic.Name = "panelPublic";
            this.panelPublic.Padding = new System.Windows.Forms.Padding(13, 12, 13, 12);
            this.panelPublic.Size = new System.Drawing.Size(585, 108);
            this.panelPublic.TabIndex = 1;
            this.panelPublic.Click += new System.EventHandler(this.panelPublic_Click);
            // 
            // lblPublicRadio
            // 
            this.lblPublicRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPublicRadio.AutoSize = true;
            this.lblPublicRadio.Font = new System.Drawing.Font("Segoe UI Symbol", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPublicRadio.Location = new System.Drawing.Point(529, 12);
            this.lblPublicRadio.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPublicRadio.Name = "lblPublicRadio";
            this.lblPublicRadio.Size = new System.Drawing.Size(38, 32);
            this.lblPublicRadio.TabIndex = 4;
            this.lblPublicRadio.Text = "🔘";
            this.lblPublicRadio.Click += new System.EventHandler(this.panelPublic_Click);
            // 
            // lblPublicDesc
            // 
            this.lblPublicDesc.AutoSize = true;
            this.lblPublicDesc.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPublicDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblPublicDesc.Location = new System.Drawing.Point(77, 50);
            this.lblPublicDesc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPublicDesc.Name = "lblPublicDesc";
            this.lblPublicDesc.Size = new System.Drawing.Size(323, 20);
            this.lblPublicDesc.TabIndex = 3;
            this.lblPublicDesc.Text = "Bất kỳ ai trong tổ chức cũng có thể xem tài liệu.";
            this.lblPublicDesc.Click += new System.EventHandler(this.panelPublic_Click);
            // 
            // lblPublicTitle
            // 
            this.lblPublicTitle.AutoSize = true;
            this.lblPublicTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPublicTitle.Location = new System.Drawing.Point(77, 23);
            this.lblPublicTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPublicTitle.Name = "lblPublicTitle";
            this.lblPublicTitle.Size = new System.Drawing.Size(88, 23);
            this.lblPublicTitle.TabIndex = 2;
            this.lblPublicTitle.Text = "Công khai";
            this.lblPublicTitle.Click += new System.EventHandler(this.panelPublic_Click);
            // 
            // lblPublicIcon
            // 
            this.lblPublicIcon.AutoSize = true;
            this.lblPublicIcon.Font = new System.Drawing.Font("Segoe UI Symbol", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPublicIcon.Location = new System.Drawing.Point(17, 23);
            this.lblPublicIcon.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPublicIcon.Name = "lblPublicIcon";
            this.lblPublicIcon.Size = new System.Drawing.Size(38, 32);
            this.lblPublicIcon.TabIndex = 1;
            this.lblPublicIcon.Text = "🌎";
            this.lblPublicIcon.Click += new System.EventHandler(this.panelPublic_Click);
            // 
            // panelPrivate
            // 
            this.panelPrivate.BackColor = System.Drawing.Color.White;
            this.panelPrivate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPrivate.Controls.Add(this.lblPrivateRadio);
            this.panelPrivate.Controls.Add(this.lblPrivateDesc);
            this.panelPrivate.Controls.Add(this.lblPrivateTitle);
            this.panelPrivate.Controls.Add(this.lblPrivateIcon);
            this.panelPrivate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelPrivate.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelPrivate.Location = new System.Drawing.Point(20, 18);
            this.panelPrivate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 12);
            this.panelPrivate.Name = "panelPrivate";
            this.panelPrivate.Padding = new System.Windows.Forms.Padding(13, 12, 13, 12);
            this.panelPrivate.Size = new System.Drawing.Size(585, 96);
            this.panelPrivate.TabIndex = 0;
            this.panelPrivate.Click += new System.EventHandler(this.panelPrivate_Click);
            // 
            // lblPrivateRadio
            // 
            this.lblPrivateRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPrivateRadio.AutoSize = true;
            this.lblPrivateRadio.Font = new System.Drawing.Font("Segoe UI Symbol", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrivateRadio.Location = new System.Drawing.Point(529, 12);
            this.lblPrivateRadio.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPrivateRadio.Name = "lblPrivateRadio";
            this.lblPrivateRadio.Size = new System.Drawing.Size(38, 32);
            this.lblPrivateRadio.TabIndex = 4;
            this.lblPrivateRadio.Text = "🔘";
            this.lblPrivateRadio.Click += new System.EventHandler(this.panelPrivate_Click);
            // 
            // lblPrivateDesc
            // 
            this.lblPrivateDesc.AutoSize = true;
            this.lblPrivateDesc.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrivateDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblPrivateDesc.Location = new System.Drawing.Point(77, 50);
            this.lblPrivateDesc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPrivateDesc.Name = "lblPrivateDesc";
            this.lblPrivateDesc.Size = new System.Drawing.Size(256, 20);
            this.lblPrivateDesc.TabIndex = 3;
            this.lblPrivateDesc.Text = "Chỉ một mình bạn có thể xem tài liệu.";
            this.lblPrivateDesc.Click += new System.EventHandler(this.panelPrivate_Click);
            // 
            // lblPrivateTitle
            // 
            this.lblPrivateTitle.AutoSize = true;
            this.lblPrivateTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrivateTitle.Location = new System.Drawing.Point(77, 23);
            this.lblPrivateTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPrivateTitle.Name = "lblPrivateTitle";
            this.lblPrivateTitle.Size = new System.Drawing.Size(76, 23);
            this.lblPrivateTitle.TabIndex = 2;
            this.lblPrivateTitle.Text = "Riêng tư";
            this.lblPrivateTitle.Click += new System.EventHandler(this.panelPrivate_Click);
            // 
            // lblPrivateIcon
            // 
            this.lblPrivateIcon.AutoSize = true;
            this.lblPrivateIcon.Font = new System.Drawing.Font("Segoe UI Symbol", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrivateIcon.Location = new System.Drawing.Point(17, 23);
            this.lblPrivateIcon.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPrivateIcon.Name = "lblPrivateIcon";
            this.lblPrivateIcon.Size = new System.Drawing.Size(38, 32);
            this.lblPrivateIcon.TabIndex = 1;
            this.lblPrivateIcon.Text = "🔒";
            this.lblPrivateIcon.Click += new System.EventHandler(this.panelPrivate_Click);
            // 
            // Form_ShareDocument
            // 
            this.AcceptButton = this.btnConfirm;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(625, 670);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelFooter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form_ShareDocument";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tùy chọn chia sẻ";
            this.Load += new System.EventHandler(this.Form_ShareDocument_Load);
            this.panelFooter.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelSpecific.ResumeLayout(false);
            this.panelSpecific.PerformLayout();
            this.panelTeacherList.ResumeLayout(false);
            this.panelTeacherList.PerformLayout();
            this.panelPublic.ResumeLayout(false);
            this.panelPublic.PerformLayout();
            this.panelPrivate.ResumeLayout(false);
            this.panelPrivate.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelPrivate;
        private System.Windows.Forms.Label lblPrivateRadio;
        private System.Windows.Forms.Label lblPrivateDesc;
        private System.Windows.Forms.Label lblPrivateTitle;
        private System.Windows.Forms.Label lblPrivateIcon;
        private System.Windows.Forms.Panel panelSpecific;
        private System.Windows.Forms.Label lblSpecificRadio;
        private System.Windows.Forms.Label lblSpecificDesc;
        private System.Windows.Forms.Label lblSpecificTitle;
        private System.Windows.Forms.Label lblSpecificIcon;
        private System.Windows.Forms.Panel panelPublic;
        private System.Windows.Forms.Label lblPublicRadio;
        private System.Windows.Forms.Label lblPublicDesc;
        private System.Windows.Forms.Label lblPublicTitle;
        private System.Windows.Forms.Label lblPublicIcon;
        private System.Windows.Forms.Panel panelTeacherList;
        private System.Windows.Forms.CheckedListBox clbTeachers;
        private System.Windows.Forms.TextBox txtSearchTeacher;
        private System.Windows.Forms.Label lblSearchIcon;
        private System.Windows.Forms.Label lblCloseButton;
    }
}