using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using N6;

public partial class frmGhiChuHocSinh : frmDraggableRoundedPopup
{
    private string _maGV, _maMon;
    private ComboBox cbLop;
    private ComboBox cbHocSinh;
    private TextBox txtGhiChu;

    public frmGhiChuHocSinh(string maGV, string maMon) : base()
    {
        _maGV = maGV;
        _maMon = maMon;
        this.Text = "Ghi chú cho Học sinh";
        this.Size = new Size(400, 380);
        InitializeModernComponent();
        LoadLopHoc();
    }

    private void InitializeModernComponent()
    {
        cbLop = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Margin = new Padding(0, 0, 0, 10) };
        cbHocSinh = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Margin = new Padding(0, 0, 0, 10) };

        var txtPanel = new RoundedPanel { Dock = DockStyle.Fill, BackColor = Color.White, CornerRadius = 10, Padding = new Padding(5) };
        txtGhiChu = new TextBox { Dock = DockStyle.Fill, Multiline = true, Font = new Font("Segoe UI", 10F), BorderStyle = BorderStyle.None };
        txtPanel.Controls.Add(txtGhiChu);

        var btnLuu = new RoundedButton { Dock = DockStyle.Bottom, Text = "Lưu Ghi Chú", Height = 40, CornerRadius = 12, BackColor = Color.MediumSeaGreen, Margin = new Padding(0, 10, 0, 0) };
        btnLuu.Click += BtnLuu_Click;

        this.ContentPanel.Controls.Add(txtPanel);
        this.ContentPanel.Controls.Add(cbHocSinh);
        this.ContentPanel.Controls.Add(cbLop);
        this.ContentPanel.Controls.Add(btnLuu);

        cbLop.SelectedIndexChanged += CbLop_SelectedIndexChanged;
    }

    private void LoadLopHoc()
    {
        DataTable dt = DatabaseHelper.GetLopByGiaoVien(_maGV);
        cbLop.DataSource = dt;
        cbLop.DisplayMember = "TenLop";
        cbLop.ValueMember = "MaLop";
        cbLop.SelectedIndex = -1;
    }

    private void CbLop_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cbLop.SelectedValue != null)
        {
            string maLop = cbLop.SelectedValue.ToString();
            DataTable dt = DatabaseHelper.GetHocSinhByLop(maLop);
            cbHocSinh.DataSource = dt;
            cbHocSinh.DisplayMember = "HoTen";
            cbHocSinh.ValueMember = "MaHS";
            cbHocSinh.SelectedIndex = -1;
        }
        else
        {
            cbHocSinh.DataSource = null;
        }
    }

    private void BtnLuu_Click(object sender, EventArgs e)
    {
        if (cbHocSinh.SelectedValue == null) { MessageBox.Show("Vui lòng chọn lớp và học sinh."); return; }
        if (string.IsNullOrWhiteSpace(txtGhiChu.Text)) { MessageBox.Show("Vui lòng nhập nội dung ghi chú."); return; }
        try
        {
            string maHS = cbHocSinh.SelectedValue.ToString();
            DatabaseHelper.AddGhiChuChoHocSinh(maHS, _maMon, txtGhiChu.Text);
            MessageBox.Show("Đã lưu ghi chú thành công!");
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi: " + ex.Message);
        }
    }
}