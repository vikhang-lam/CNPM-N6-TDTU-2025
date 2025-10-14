// File: frmGhiChuHocSinh.cs - PHIÊN BẢN ĐÃ SỬA LỖI
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using N6;

public partial class frmGhiChuHocSinh : frmDraggableRoundedPopup
{
    private string _maGV;
    private ComboBox cbLop;
    private ComboBox cbMonHoc; // Thêm ComboBox Môn học
    private ComboBox cbHocSinh;
    private TextBox txtGhiChu;

    // Sửa lại Constructor, không cần truyền MaMon vào nữa
    public frmGhiChuHocSinh(string maGV) : base()
    {
        _maGV = maGV;
        this.Text = "Ghi chú cho Học sinh";
        this.Size = new Size(400, 420); // Tăng chiều cao form một chút
        InitializeModernComponent();
        LoadLopHoc();
    }

    private void InitializeModernComponent()
    {
        cbLop = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Margin = new Padding(0, 0, 0, 10) };
        cbMonHoc = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Margin = new Padding(0, 0, 0, 10) };
        cbHocSinh = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Margin = new Padding(0, 0, 0, 10) };

        var txtPanel = new RoundedPanel { Dock = DockStyle.Fill, BackColor = Color.White, CornerRadius = 10, Padding = new Padding(5) };
        txtGhiChu = new TextBox { Dock = DockStyle.Fill, Multiline = true, Font = new Font("Segoe UI", 10F), BorderStyle = BorderStyle.None };
        txtPanel.Controls.Add(txtGhiChu);

        var btnLuu = new RoundedButton { Dock = DockStyle.Bottom, Text = "Lưu Ghi Chú", Height = 40, CornerRadius = 12, BackColor = Color.MediumSeaGreen, Margin = new Padding(0, 10, 0, 0) };
        btnLuu.Click += BtnLuu_Click;

        this.ContentPanel.Controls.Add(txtPanel);
        this.ContentPanel.Controls.Add(cbHocSinh);
        this.ContentPanel.Controls.Add(cbMonHoc); // Thêm cbMonHoc vào form
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
        cbLop.Text = "Chọn lớp học";
    }

    private void CbLop_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cbLop.SelectedValue != null)
        {
            string maLop = cbLop.SelectedValue.ToString();

            // Load danh sách môn học mà GV dạy ở lớp này
            DataTable dtMon = DatabaseHelper.GetMonHocByGiaoVienAndLop(_maGV, maLop);
            cbMonHoc.DataSource = dtMon;
            cbMonHoc.DisplayMember = "TenMon";
            cbMonHoc.ValueMember = "MaMon";
            cbMonHoc.SelectedIndex = -1;
            cbMonHoc.Text = "Chọn môn học";

            // Load danh sách học sinh của lớp
            DataTable dtHS = DatabaseHelper.GetHocSinhByLop(maLop);
            cbHocSinh.DataSource = dtHS;
            cbHocSinh.DisplayMember = "HoTen";
            cbHocSinh.ValueMember = "MaHS";
            cbHocSinh.SelectedIndex = -1;
            cbHocSinh.Text = "Chọn học sinh";
        }
        else
        {
            cbMonHoc.DataSource = null;
            cbHocSinh.DataSource = null;
        }
    }

    private void BtnLuu_Click(object sender, EventArgs e)
    {
        // Thêm kiểm tra đã chọn Môn học chưa
        if (cbLop.SelectedValue == null) { MessageBox.Show("Vui lòng chọn lớp."); return; }
        if (cbMonHoc.SelectedValue == null) { MessageBox.Show("Vui lòng chọn môn học."); return; }
        if (cbHocSinh.SelectedValue == null) { MessageBox.Show("Vui lòng chọn học sinh."); return; }
        if (string.IsNullOrWhiteSpace(txtGhiChu.Text)) { MessageBox.Show("Vui lòng nhập nội dung ghi chú."); return; }

        try
        {
            string maHS = cbHocSinh.SelectedValue.ToString();
            // Lấy MaMon từ ComboBox đã chọn, không gọi hàm cũ nữa
            string maMon = cbMonHoc.SelectedValue.ToString();

            DatabaseHelper.AddGhiChuChoHocSinh(maHS, maMon, txtGhiChu.Text);
            MessageBox.Show("Đã lưu ghi chú thành công!");
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi: " + ex.Message);
        }
    }
}