using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using N6;
using System.Speech.Recognition;
using System.Globalization;
using System.Linq;

/// <summary>
/// Form cho phép giáo viên thêm ghi chú cho một học sinh cụ thể
/// về một môn học cụ thể, hỗ trợ nhập liệu bằng giọng nói.
/// </summary>
public partial class frmGhiChuHocSinh : frmDraggableRoundedPopup
{
    private string _maGV;
    private ComboBox cbLop;
    private ComboBox cbMonHoc;
    private ComboBox cbHocSinh;
    private TextBox txtGhiChu;
    private RoundedButton btnLuu;
    private RoundedButton btnRecord;

    private SpeechRecognitionEngine _speechEngine;
    private bool _isListening = false;

    public frmGhiChuHocSinh(string maGV) : base()
    {
        _maGV = maGV;
        this.Text = "Ghi chú cho Học sinh";
        this.Size = new Size(400, 470);
        InitializeModernComponent();
        LoadLopHoc();
    }

    /// <summary>
    /// Khởi tạo và sắp xếp các control động.
    /// </summary>
    private void InitializeModernComponent()
    {
        cbLop = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Margin = new Padding(0, 0, 0, 10) };
        cbMonHoc = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Margin = new Padding(0, 0, 0, 10) };
        cbHocSinh = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Margin = new Padding(0, 0, 0, 10) };

        var txtPanel = new RoundedPanel { Dock = DockStyle.Fill, BackColor = Color.White, CornerRadius = 10, Padding = new Padding(5) };
        txtGhiChu = new TextBox { Dock = DockStyle.Fill, Multiline = true, Font = new Font("Segoe UI", 10F), BorderStyle = BorderStyle.None };
        txtPanel.Controls.Add(txtGhiChu);

        btnLuu = new RoundedButton { Dock = DockStyle.Bottom, Text = "Lưu Ghi Chú", Height = 40, CornerRadius = 12, BackColor = Color.MediumSeaGreen, Margin = new Padding(0, 10, 0, 0) };
        btnLuu.Click += BtnLuu_Click;

        btnRecord = new RoundedButton { Dock = DockStyle.Bottom, Text = "🎤 Ghi Âm (Tiếng Việt)", Height = 40, CornerRadius = 12, BackColor = Color.RoyalBlue, Margin = new Padding(0, 5, 0, 0) };
        btnRecord.Click += BtnRecord_Click;

        this.ContentPanel.Controls.Add(txtPanel);
        this.ContentPanel.Controls.Add(cbHocSinh);
        this.ContentPanel.Controls.Add(cbMonHoc);
        this.ContentPanel.Controls.Add(cbLop);
        this.ContentPanel.Controls.Add(btnRecord);
        this.ContentPanel.Controls.Add(btnLuu);

        cbLop.SelectedIndexChanged += CbLop_SelectedIndexChanged;
    }

    #region Speech Recognition (Nhận diện giọng nói)

    /// <summary>
    /// Khởi tạo bộ nhận diện giọng nói (SpeechRecognitionEngine) của Windows.
    /// </summary>
    /// <returns>True nếu tìm thấy bộ nhận diện Tiếng Việt (vi-VN).</returns>
    private bool InitializeSpeechEngine()
    {
        try
        {
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
            _speechEngine.LoadGrammar(new DictationGrammar());

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

    /// <summary>
    /// Xử lý sự kiện click nút Ghi Âm (Bắt đầu hoặc Dừng).
    /// </summary>
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
                if (!InitializeSpeechEngine())
                {
                    return; // Khởi tạo thất bại
                }
            }

            try
            {
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

    /// <summary>
    /// Sự kiện khi việc nhận diện hoàn tất (do dừng, lỗi, hoặc timeout).
    /// </summary>
    private void SpeechEngine_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
    {
        // Đảm bảo cập nhật UI trên đúng luồng (thread)
        if (this.IsDisposed || !this.IsHandleCreated) return;
        this.Invoke(new Action(() =>
        {
            _isListening = false;
            btnRecord.Text = "🎤 Ghi Âm (Tiếng Việt)";
            btnRecord.BackColor = Color.RoyalBlue;
        }));
    }

    /// <summary>
    /// Sự kiện khi nhận diện được một đoạn văn bản.
    /// </summary>
    private void SpeechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
    {
        if (this.IsDisposed || !this.IsHandleCreated) return;
        this.Invoke(new Action(() =>
        {
            if (txtGhiChu.Text.Length > 0 && !txtGhiChu.Text.EndsWith(" "))
            {
                txtGhiChu.AppendText(" "); // Thêm khoảng trắng
            }
            txtGhiChu.AppendText(e.Result.Text);
        }));
    }

    #endregion

    #region Data Loading & Saving

    /// <summary>
    /// Tải danh sách lớp học mà giáo viên này dạy.
    /// </summary>
    private void LoadLopHoc()
    {
        DataTable dt = DatabaseHelper.GetClassesByTeacher(_maGV);
        cbLop.DataSource = dt;
        cbLop.DisplayMember = "TenLop";
        cbLop.ValueMember = "MaLop";
        cbLop.SelectedIndex = -1;
        cbLop.Text = "Chọn lớp học";
    }

    /// <summary>
    /// Xử lý khi chọn lớp: tải lại danh sách môn học và học sinh.
    /// </summary>
    private void CbLop_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cbLop.SelectedValue != null)
        {
            string maLop = cbLop.SelectedValue.ToString();

            // Load danh sách môn học mà GV dạy ở lớp này
            DataTable dtMon = DatabaseHelper.GetSubjectsByTeacherAndClass(_maGV, maLop);
            cbMonHoc.DataSource = dtMon;
            cbMonHoc.DisplayMember = "TenMon";
            cbMonHoc.ValueMember = "MaMon";
            cbMonHoc.SelectedIndex = -1;
            cbMonHoc.Text = "Chọn môn học";

            // Load danh sách học sinh của lớp
            DataTable dtHS = DatabaseHelper.GetStudentsByClass(maLop);
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

    /// <summary>
    /// Xử lý sự kiện click nút "Lưu Ghi Chú".
    /// </summary>
    private void BtnLuu_Click(object sender, EventArgs e)
    {
        // Kiểm tra
        if (cbLop.SelectedValue == null) { MessageBox.Show("Vui lòng chọn lớp."); return; }
        if (cbMonHoc.SelectedValue == null) { MessageBox.Show("Vui lòng chọn môn học."); return; }
        if (cbHocSinh.SelectedValue == null) { MessageBox.Show("Vui lòng chọn học sinh."); return; }
        if (string.IsNullOrWhiteSpace(txtGhiChu.Text) || txtGhiChu.ForeColor == Color.Gray) // Kiểm tra placeholder
        {
            MessageBox.Show("Vui lòng nhập nội dung ghi chú.");
            return;
        }

        try
        {
            string maHS = cbHocSinh.SelectedValue.ToString();
            string maMon = cbMonHoc.SelectedValue.ToString();

            DatabaseHelper.AddNoteForStudent(maHS, maMon, txtGhiChu.Text);
            MessageBox.Show("Đã lưu ghi chú thành công!");
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi: " + ex.Message);
        }
    }

    #endregion

    /// <summary>
    /// Dọn dẹp tài nguyên (quan trọng: gỡ bỏ SpeechEngine).
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Giải phóng tài nguyên speech
            if (_speechEngine != null)
            {
                _speechEngine.RecognizeAsyncStop(); // Dừng nếu đang chạy
                _speechEngine.SpeechRecognized -= SpeechEngine_SpeechRecognized;
                _speechEngine.RecognizeCompleted -= SpeechEngine_RecognizeCompleted;
                _speechEngine.Dispose();
                _speechEngine = null;
            }

            // Gỡ bỏ các sự kiện khác
            if (btnLuu != null) btnLuu.Click -= BtnLuu_Click;
            if (btnRecord != null) btnRecord.Click -= BtnRecord_Click;
            if (cbLop != null) cbLop.SelectedIndexChanged -= CbLop_SelectedIndexChanged;
        }
        base.Dispose(disposing);
    }
}