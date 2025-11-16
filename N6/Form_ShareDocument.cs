using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    public partial class Form_ShareDocument : Form
    {
        private string maTL;
        private string maGVOwner;

        private enum ShareMode { Private, Public, Specific }
        private ShareMode currentMode = ShareMode.Private;

        // (MỚI) Bộ nhớ để lưu trữ các mục đã chọn, bất kể đang lọc hay không
        private HashSet<string> checkedTeacherMaGVs = new HashSet<string>();

        private class TeacherDisplay
        {
            public string MaGV { get; set; }
            public string TenGV { get; set; }
            public override string ToString() => TenGV;
        }

        private List<TeacherDisplay> allTeachers = new List<TeacherDisplay>();

        // Màu sắc
        private Color colorSelected = Color.FromArgb(232, 240, 254);
        private Color colorBorderSelected = Color.FromArgb(24, 119, 242);
        private Color colorDefault = Color.White;
        private Color colorBorderDefault = Color.FromArgb(222, 226, 230);
        private string textSearchPlaceholder = "Tìm kiếm giáo viên...";

        // Chiều cao cố định
        private int collapsedSpecificHeight = 70;
        private int expandedSpecificHeight = 290;

        // Hằng số để chặn di chuyển Form
        private const int WM_NCHITTEST = 0x84;
        private const int HT_CAPTION = 0x2;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
            {
                if (m.Result == (IntPtr)HT_CAPTION)
                {
                    m.Result = IntPtr.Zero;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawRectangle(
                new Pen(Color.Gray, 1),
                0, 0, this.Width - 1, this.Height - 1
            );
        }

        public Form_ShareDocument(string maTaiLieu, string tenTaiLieu, string maGiaoVienOwner)
        {
            InitializeComponent();
            this.maTL = maTaiLieu;
            this.maGVOwner = maGiaoVienOwner;
            this.lblFileName.Text = tenTaiLieu;

            RegisterClickEvents(panelPrivate);
            RegisterClickEvents(panelPublic);
            RegisterClickEvents(panelSpecific, true);

            // (MỚI) Đăng ký sự kiện ItemCheck (bạn cũng có thể làm điều này trong Designer)
            this.clbTeachers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbTeachers_ItemCheck);
        }

        private void Form_ShareDocument_Load(object sender, EventArgs e)
        {
            txtSearchTeacher.Text = textSearchPlaceholder;
            txtSearchTeacher.ForeColor = Color.Gray;

            LoadAllTeachers();
            LoadCurrentShareStatus();
        }

        #region Load Data

        private void LoadAllTeachers()
        {
            allTeachers.Clear();
            DataTable dt = DatabaseHelper.GetAllTeachersForSharing(maGVOwner);
            foreach (DataRow row in dt.Rows)
            {
                allTeachers.Add(new TeacherDisplay
                {
                    MaGV = row["MaGV"].ToString(),
                    TenGV = row["Ten"].ToString()
                });
            }
            FilterTeacherList("");
        }

        /// <summary>
        /// (ĐÃ SỬA) Hàm này giờ chỉ đọc từ "bộ nhớ" checkedTeacherMaGVs
        /// </summary>
        private void FilterTeacherList(string filterText)
        {
            // Tạm thời gỡ sự kiện ItemCheck để tránh nó kích hoạt khi chúng ta thêm item
            clbTeachers.ItemCheck -= clbTeachers_ItemCheck;

            clbTeachers.Items.Clear();

            var filteredTeachers = allTeachers;
            if (!string.IsNullOrWhiteSpace(filterText) && filterText != textSearchPlaceholder)
            {
                filteredTeachers = allTeachers
                    .Where(t => t.TenGV.ToLower().Contains(filterText.ToLower()))
                    .ToList();
            }

            foreach (var teacher in filteredTeachers)
            {
                // Kiểm tra xem giáo viên này có trong "bộ nhớ" hay không
                bool isChecked = checkedTeacherMaGVs.Contains(teacher.MaGV);
                clbTeachers.Items.Add(teacher, isChecked);
            }

            // Đăng ký lại sự kiện
            clbTeachers.ItemCheck += clbTeachers_ItemCheck;
        }

        /// <summary>
        /// (ĐÃ SỬA) Hàm này nạp dữ liệu vào "bộ nhớ" (Set)
        /// </summary>
        private void LoadCurrentShareStatus()
        {
            string currentStatus = DatabaseHelper.GetDocumentStatus(maTL);
            List<string> sharedWithList = DatabaseHelper.GetSharedWithTeachers(maTL);

            // 1. Nạp dữ liệu từ DB vào "bộ nhớ" (Set)
            checkedTeacherMaGVs.Clear();
            foreach (var maGV in sharedWithList)
            {
                checkedTeacherMaGVs.Add(maGV);
            }

            // 2. Cập nhật RadioButton
            if (currentStatus == "Chia sẻ")
            {
                currentMode = ShareMode.Public;
            }
            else if (currentStatus == "Giáo viên cụ thể")
            {
                currentMode = ShareMode.Specific;
            }
            else
            {
                currentMode = ShareMode.Private;
            }

            // 3. Cập nhật lại UI (clbTeachers) sau khi đã có dữ liệu
            // Tạm thời gỡ sự kiện
            clbTeachers.ItemCheck -= clbTeachers_ItemCheck;
            for (int i = 0; i < clbTeachers.Items.Count; i++)
            {
                if (clbTeachers.Items[i] is TeacherDisplay teacher)
                {
                    if (checkedTeacherMaGVs.Contains(teacher.MaGV))
                    {
                        clbTeachers.SetItemChecked(i, true);
                    }
                }
            }
            // Đăng ký lại sự kiện
            clbTeachers.ItemCheck += clbTeachers_ItemCheck;


            // 4. Cập nhật giao diện (panel co/giãn)
            UpdateSelectionUI();
        }

        #endregion

        #region UI Logic

        private void UpdateSelectionUI()
        {
            // Reset tất cả
            panelPrivate.BackColor = colorDefault;
            lblPrivateRadio.Text = "🔘";
            lblPrivateRadio.ForeColor = Color.Gray;

            panelPublic.BackColor = colorDefault;
            lblPublicRadio.Text = "🔘";
            lblPublicRadio.ForeColor = Color.Gray;

            panelSpecific.BackColor = colorDefault;
            lblSpecificRadio.Text = "🔘";
            lblSpecificRadio.ForeColor = Color.Gray;

            panelTeacherList.Visible = false;
            panelSpecific.Height = collapsedSpecificHeight;

            // Kích hoạt cái được chọn
            switch (currentMode)
            {
                case ShareMode.Private:
                    panelPrivate.BackColor = colorSelected;
                    lblPrivateRadio.Text = "◉";
                    lblPrivateRadio.ForeColor = colorBorderSelected;
                    break;
                case ShareMode.Public:
                    panelPublic.BackColor = colorSelected;
                    lblPublicRadio.Text = "◉";
                    lblPublicRadio.ForeColor = colorBorderSelected;
                    break;
                case ShareMode.Specific:
                    panelSpecific.BackColor = colorSelected;
                    lblSpecificRadio.Text = "◉";
                    lblSpecificRadio.ForeColor = colorBorderSelected;
                    panelTeacherList.Visible = true;
                    panelSpecific.Height = expandedSpecificHeight;
                    break;
            }

            ResizeFormToContent();
        }

        private void ResizeFormToContent()
        {
            int totalContentHeight = panelPrivate.Height + panelPublic.Height + panelSpecific.Height
                                     + (panelMain.Padding.Top + panelMain.Padding.Bottom)
                                     + 10 + 10;

            this.Height = panelHeader.Height + totalContentHeight + panelFooter.Height;
        }

        private void RegisterClickEvents(Control parent, bool skipChildren = false)
        {
            parent.Click += (s, e) => OnPanelClick(parent);

            if (skipChildren) return;

            foreach (Control child in parent.Controls)
            {
                child.Click += (s, e) => OnPanelClick(parent);
            }
        }

        private void OnPanelClick(Control panel)
        {
            if (panel == panelPrivate)
                currentMode = ShareMode.Private;
            else if (panel == panelPublic)
                currentMode = ShareMode.Public;
            else if (panel == panelSpecific)
                currentMode = ShareMode.Specific;

            UpdateSelectionUI();
        }

        private void panelPrivate_Click(object sender, EventArgs e) => OnPanelClick(panelPrivate);
        private void panelPublic_Click(object sender, EventArgs e) => OnPanelClick(panelPublic);
        private void panelSpecific_Click(object sender, EventArgs e) => OnPanelClick(panelSpecific);

        private void lblCloseButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// (MỚI) Hàm này cập nhật "bộ nhớ" (Set) mỗi khi người dùng check/uncheck
        /// </summary>
        private void clbTeachers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index < 0 || e.Index >= clbTeachers.Items.Count) return;

            if (clbTeachers.Items[e.Index] is TeacherDisplay teacher)
            {
                if (e.NewValue == CheckState.Checked)
                {
                    if (!checkedTeacherMaGVs.Contains(teacher.MaGV))
                        checkedTeacherMaGVs.Add(teacher.MaGV);
                }
                else // CheckState.Unchecked
                {
                    if (checkedTeacherMaGVs.Contains(teacher.MaGV))
                        checkedTeacherMaGVs.Remove(teacher.MaGV);
                }
            }
        }

        #endregion

        #region Save Logic

        /// <summary>
        /// (ĐÃ SỬA) Hàm này chỉ cần đọc từ "bộ nhớ"
        /// </summary>
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string newTrangThai;
            List<string> newMaGiaoVienList; // Đã bỏ new... ở tên biến

            switch (currentMode)
            {
                case ShareMode.Public:
                    newTrangThai = "Chia sẻ";
                    newMaGiaoVienList = new List<string>(); // Rỗng
                    break;
                case ShareMode.Specific:
                    newTrangThai = "Giáo viên cụ thể";
                    // Đọc trực tiếp từ "bộ nhớ" (Set)
                    newMaGiaoVienList = checkedTeacherMaGVs.ToList();

                    if (newMaGiaoVienList.Count == 0)
                    {
                        MessageBox.Show("Bạn đã chọn 'Chia sẻ cho giáo viên cụ thể' nhưng chưa chọn giáo viên nào.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                    break;
                default: // ShareMode.Private
                    newTrangThai = "Riêng tư";
                    newMaGiaoVienList = new List<string>(); // Rỗng
                    break;
            }

            try
            {
                // Gửi newTrangThai và newMaGiaoVienList
                DatabaseHelper.UpdateDocumentSharing(maTL, newTrangThai, newMaGiaoVienList);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật chia sẻ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }

        #endregion

        #region Search Placeholder
        private void txtSearchTeacher_Enter(object sender, EventArgs e)
        {
            if (txtSearchTeacher.Text == textSearchPlaceholder)
            {
                txtSearchTeacher.Text = "";
                txtSearchTeacher.ForeColor = Color.Black;
            }
        }

        private void txtSearchTeacher_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchTeacher.Text))
            {
                txtSearchTeacher.Text = textSearchPlaceholder;
                txtSearchTeacher.ForeColor = Color.Gray;
            }
        }
        private void txtSearchTeacher_TextChanged(object sender, EventArgs e)
        {
            FilterTeacherList(txtSearchTeacher.Text);
        }
        #endregion

        #region Draw Borders
        // Vẽ đường viền cho Header và Footer
        private void panelHeader_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(colorBorderDefault, 1), 0, panelHeader.Height - 1, panelHeader.Width, panelHeader.Height - 1);
        }

        private void panelFooter_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(colorBorderDefault, 1), 0, 0, panelFooter.Width, 0);
        }
        #endregion
    }
}