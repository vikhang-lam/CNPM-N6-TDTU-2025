// File: frmGhiChuHocSinh.cs - PHIÊN BẢN CẬP NHẬT
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using N6;
using System.Speech.Recognition; // THÊM MỚI
using System.Globalization;     // THÊM MỚI
using System.Linq;              // THÊM MỚI

public partial class frmGhiChuHocSinh : frmDraggableRoundedPopup
{
    private string _maGV;
    private ComboBox cbLop;
    private ComboBox cbMonHoc;
    private ComboBox cbHocSinh;
    private TextBox txtGhiChu;
    private RoundedButton btnLuu;       // THÊM MỚI: Di chuyển ra biến class
    private RoundedButton btnRecord;    // THÊM MỚI: Nút ghi âm

    // THÊM MỚI: Các biến cho Speech Recognition
    private SpeechRecognitionEngine _speechEngine;
    private bool _isListening = false;

    // Sửa lại Constructor, không cần truyền MaMon vào nữa
    public frmGhiChuHocSinh(string maGV) : base()
    {
        _maGV = maGV;
        this.Text = "Ghi chú cho Học sinh";
        this.Size = new Size(400, 470); // SỬA ĐỔI: Tăng chiều cao cho nút mới
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

        // SỬA ĐỔI: Khởi tạo nút Lưu (đã chuyển thành biến class)
        btnLuu = new RoundedButton { Dock = DockStyle.Bottom, Text = "Lưu Ghi Chú", Height = 40, CornerRadius = 12, BackColor = Color.MediumSeaGreen, Margin = new Padding(0, 10, 0, 0) };
        btnLuu.Click += BtnLuu_Click;

        // THÊM MỚI: Nút Ghi Âm
        btnRecord = new RoundedButton { Dock = DockStyle.Bottom, Text = "🎤 Ghi Âm (Tiếng Việt)", Height = 40, CornerRadius = 12, BackColor = Color.RoyalBlue, Margin = new Padding(0, 5, 0, 0) };
        btnRecord.Click += BtnRecord_Click;

        this.ContentPanel.Controls.Add(txtPanel);
        this.ContentPanel.Controls.Add(cbHocSinh);
        this.ContentPanel.Controls.Add(cbMonHoc);
        this.ContentPanel.Controls.Add(cbLop);

        // SỬA ĐỔI: Thêm 2 nút vào
        this.ContentPanel.Controls.Add(btnRecord); // Nút Ghi Âm
        this.ContentPanel.Controls.Add(btnLuu);      // Nút Lưu

        cbLop.SelectedIndexChanged += CbLop_SelectedIndexChanged;
    }

    // THÊM MỚI: Khởi tạo bộ nhận diện giọng nói
    private bool InitializeSpeechEngine()
    {
        try
        {
            // Tìm bộ nhận diện "vi-VN" đã cài đặt trên Windows
            var culture = new CultureInfo("vi-VN");
            var recognizer = SpeechRecognitionEngine.InstalledRecognizers()
                                .FirstOrDefault(r => r.Culture.Equals(culture));

            if (recognizer == null)
            {
                MessageBox.Show("Không tìm thấy bộ nhận diện giọng nói Tiếng Việt.\n\nVui lòng cài đặt Gói Ngôn Ngữ Tiếng Việt (bao gồm tính năng Giọng nói) trong Cài đặt Windows.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            _speechEngine = new SpeechRecognitionEngine(recognizer);
            _speechEngine.SetInputToDefaultAudioDevice();
            _speechEngine.LoadGrammar(new DictationGrammar()); // Tải ngữ pháp mặc định

            // Đăng ký sự kiện
            _speechEngine.SpeechRecognized += SpeechEngine_SpeechRecognized;
            _speechEngine.RecognizeCompleted += SpeechEngine_RecognizeCompleted;

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi khởi tạo nhận diện giọng nói: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    // THÊM MỚI: Sự kiện khi nhấn nút Ghi Âm
    private void BtnRecord_Click(object sender, EventArgs e)
    {
        if (_isListening)
        {
            // Dừng ghi âm
            if (_speechEngine != null)
            {
                _speechEngine.RecognizeAsyncStop();
            }
        }
        else
        {
            // Bắt đầu ghi âm
            if (_speechEngine == null)
            {
                // Khởi tạo lần đầu
                if (!InitializeSpeechEngine())
                {
                    return; // Khởi tạo thất bại, đã báo lỗi
                }
            }

            try
            {
                // Bắt đầu nhận diện (chế độ multiple để nhận diện liên tục)
                _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                _isListening = true;
                btnRecord.Text = "🎧 Đang nghe... (Nhấn để dừng)";
                btnRecord.BackColor = Color.Crimson;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể bắt đầu ghi âm: " + ex.Message);
            }
        }
    }

    // THÊM MỚI: Xử lý khi nhận diện xong (dừng, lỗi, hoặc timeout)
    private void SpeechEngine_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
    {
        // Đảm bảo cập nhật UI trên đúng luồng (thread)
        this.Invoke(new Action(() =>
        {
            _isListening = false;
            btnRecord.Text = "🎤 Ghi Âm (Tiếng Việt)";
            btnRecord.BackColor = Color.RoyalBlue;
        }));
    }

    // THÊM MỚI: Xử lý khi có kết quả nhận diện
    private void SpeechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
    {
        // Đảm bảo cập nhật UI trên đúng luồng (thread)
        this.Invoke(new Action(() =>
        {
            if (txtGhiChu.Text.Length > 0 && !txtGhiChu.Text.EndsWith(" "))
            {
                txtGhiChu.AppendText(" "); // Thêm khoảng trắng
            }
            txtGhiChu.AppendText(e.Result.Text);
        }));
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

    // THÊM MỚI: Ghi đè phương thức Dispose để giải phóng _speechEngine
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Giải phóng tài nguyên speech
            if (_speechEngine != null)
            {
                _speechEngine.SpeechRecognized -= SpeechEngine_SpeechRecognized;
                _speechEngine.RecognizeCompleted -= SpeechEngine_RecognizeCompleted;
                _speechEngine.Dispose();
                _speechEngine = null;
            }
        }
        base.Dispose(disposing);
    }
}