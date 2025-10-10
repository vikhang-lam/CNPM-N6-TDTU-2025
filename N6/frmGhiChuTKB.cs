using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using N6;

public partial class frmGhiChuTKB : frmDraggableRoundedPopup
{
    private string _maGV;
    private ComboBox cbLop;
    private DateTimePicker dtpNgay;
    private TextBox txtGhiChu;
    private NumericUpDown numTiet; // <<-- KHAI BÁO CONTROL MỚI

    public frmGhiChuTKB(string maGV) : base()
    {
        _maGV = maGV;
        this.Text = "Ghi chú vào Thời khóa biểu";
        this.Size = new Size(400, 380); // Tăng chiều cao để có chỗ cho control mới
        InitializeModernComponent();
        LoadLopHoc();
    }

    private void InitializeModernComponent()
    {
        cbLop = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Margin = new Padding(0, 0, 0, 10) };
        dtpNgay = new DateTimePicker { Dock = DockStyle.Top, Format = DateTimePickerFormat.Long, Font = new Font("Segoe UI", 10F), Margin = new Padding(0, 0, 0, 10) };

        // ====> THIẾT KẾ CHO Ô CHỌN TIẾT HỌC <====
        var tietPanel = new Panel { Dock = DockStyle.Top, Height = 35, Margin = new Padding(0, 0, 0, 10) };
        var lblTiet = new Label { Text = "Chọn tiết:", Dock = DockStyle.Left, Font = new Font("Segoe UI", 10F), AutoSize = true, Padding = new Padding(0, 5, 0, 0) };
        numTiet = new NumericUpDown { Dock = DockStyle.Left, Font = new Font("Segoe UI", 10F), Minimum = 0, Maximum = 10, Width = 60 };
        var lblGhiChuTiet0 = new Label { Text = "(Tiết 0 là ghi chú chung cho cả ngày)", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 8F, FontStyle.Italic), ForeColor = Color.Gray, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 5, 0, 0) };
        tietPanel.Controls.Add(lblGhiChuTiet0);
        tietPanel.Controls.Add(numTiet);
        tietPanel.Controls.Add(lblTiet);
        // ===========================================

        var txtPanel = new RoundedPanel { Dock = DockStyle.Fill, BackColor = Color.White, CornerRadius = 10, Padding = new Padding(5) };
        txtGhiChu = new TextBox { Dock = DockStyle.Fill, Multiline = true, Font = new Font("Segoe UI", 10F), BorderStyle = BorderStyle.None };
        txtPanel.Controls.Add(txtGhiChu);

        var btnLuu = new RoundedButton { Dock = DockStyle.Bottom, Text = "Lưu Ghi Chú", Height = 40, CornerRadius = 12, BackColor = Color.MediumSeaGreen, Margin = new Padding(0, 10, 0, 0) };
        btnLuu.Click += BtnLuu_Click;

        this.ContentPanel.Controls.Add(txtPanel);
        this.ContentPanel.Controls.Add(tietPanel); // Thêm panel chọn tiết
        this.ContentPanel.Controls.Add(dtpNgay);
        this.ContentPanel.Controls.Add(cbLop);
        this.ContentPanel.Controls.Add(btnLuu);
    }

    private void LoadLopHoc()
    {
        DataTable dt = DatabaseHelper.GetLopByGiaoVien(_maGV);
        cbLop.DataSource = dt;
        cbLop.DisplayMember = "TenLop";
        cbLop.ValueMember = "MaLop";
        if (dt.Rows.Count > 0) cbLop.SelectedIndex = 0;
    }

    private void BtnLuu_Click(object sender, EventArgs e)
    {
        if (cbLop.SelectedValue == null) { MessageBox.Show("Vui lòng chọn lớp."); return; }
        if (string.IsNullOrWhiteSpace(txtGhiChu.Text)) { MessageBox.Show("Vui lòng nhập nội dung ghi chú."); return; }

        string maLop = cbLop.SelectedValue.ToString();
        int tiet = (int)numTiet.Value; // <<-- LẤY GIÁ TRỊ TỪ Ô CHỌN TIẾT

        try
        {
            // Gọi hàm đã được nâng cấp với tham số "tiet"
            DatabaseHelper.AddGhiChuTKB(_maGV, maLop, dtpNgay.Value, tiet, txtGhiChu.Text);
            MessageBox.Show("Đã lưu ghi chú thành công!");
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi: " + ex.Message);
        }
    }
}