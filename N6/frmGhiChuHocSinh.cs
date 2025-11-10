using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using Microsoft.VisualBasic;
using Whisper.net;
using Whisper.net.Ggml;
using NAudio.Wave;
using N6;

public partial class frmGhiChuHocSinh : frmDraggableRoundedPopup
{
    private string _maGV;
    private ComboBox cbLop;
    private ComboBox cbMonHoc;
    private ComboBox cbHocSinh;
    private TextBox txtGhiChu;
    private RoundedButton btnLuu;
    private RoundedButton btnRecord;

    private WhisperFactory _whisperFactory;
    private WhisperProcessor _processor;
    private WaveInEvent _waveIn;
    private MemoryStream _audioStream;
    private bool _isRecording = false;

    private readonly string _modelPath = "ggml-base.bin";

    public frmGhiChuHocSinh(string maGV) : base()
    {
        _maGV = maGV;
        this.Text = "Ghi chú cho Học sinh";
        this.Size = new Size(400, 470);
        InitializeModernComponent();
        LoadLopHoc();
        InitializeWhisper();
    }

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
        btnRecord.Click += BtnRecord_Click_Async;

        this.ContentPanel.Controls.Add(txtPanel);
        this.ContentPanel.Controls.Add(cbHocSinh);
        this.ContentPanel.Controls.Add(cbMonHoc);
        this.ContentPanel.Controls.Add(cbLop);
        this.ContentPanel.Controls.Add(btnRecord);
        this.ContentPanel.Controls.Add(btnLuu);

        cbLop.SelectedIndexChanged += CbLop_SelectedIndexChanged;
    }

    private void InitializeWhisper()
    {
        try
        {
            if (!File.Exists(_modelPath))
            {
                MessageBox.Show($"Không tìm thấy file mô hình Whisper: {_modelPath}\nVui lòng kiểm tra lại.", "Lỗi Tải Mô Hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnRecord.Enabled = false;
                btnRecord.Text = "Lỗi (Không tìm thấy model)";
                btnRecord.BackColor = Color.Gray;
                return;
            }

            _whisperFactory = WhisperFactory.FromPath(_modelPath);
            _processor = _whisperFactory.CreateBuilder().WithLanguage("vi").Build();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi nghiêm trọng khi khởi tạo Whisper:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            btnRecord.Enabled = false;
            btnRecord.Text = "Lỗi Khởi tạo";
            btnRecord.BackColor = Color.Gray;
        }
    }

    private async void BtnRecord_Click_Async(object sender, EventArgs e)
    {
        if (_processor == null)
        {
            MessageBox.Show("Mô hình Whisper chưa sẵn sàng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_isRecording)
        {
            try
            {
                _waveIn.StopRecording();
                _isRecording = false;

                btnRecord.Enabled = false;
                btnRecord.Text = "⏳ Đang xử lý...";
                btnRecord.BackColor = Color.Orange;

                _audioStream.Seek(0, SeekOrigin.Begin);

                string recognizedText = "";
                await foreach (var result in _processor.ProcessAsync(_audioStream))
                {
                    recognizedText += result.Text;
                }

                if (!string.IsNullOrWhiteSpace(recognizedText))
                {
                    if (txtGhiChu.Text.Length > 0 && !txtGhiChu.Text.EndsWith(" "))
                    {
                        txtGhiChu.AppendText(" ");
                    }
                    txtGhiChu.AppendText(recognizedText.Trim());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi trong quá trình nhận diện: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _waveIn?.Dispose();
                _audioStream?.Dispose();
                _waveIn = null;
                _audioStream = null;

                btnRecord.Enabled = true;
                btnRecord.Text = "🎤 Ghi Âm (Tiếng Việt)";
                btnRecord.BackColor = Color.RoyalBlue;
            }
        }
        else
        {
            try
            {
                _audioStream = new MemoryStream();
                _waveIn = new WaveInEvent();
                _waveIn.WaveFormat = new WaveFormat(16000, 16, 1);
                var waveWriter = new WaveFileWriter(_audioStream, _waveIn.WaveFormat);

                _waveIn.DataAvailable += (s, a) =>
                {
                    waveWriter.Write(a.Buffer, 0, a.BytesRecorded);
                    waveWriter.Flush();
                };

                _waveIn.RecordingStopped += (s, a) =>
                {
                    waveWriter.Dispose();
                };
                _waveIn.StartRecording();

                _isRecording = true;

                btnRecord.Text = "🎧 Đang nghe... (Nhấn để dừng)";
                btnRecord.BackColor = Color.Crimson;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi bắt đầu ghi âm (Kiểm tra Micro):\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _isRecording = false;
            }
        }
    }

    private void LoadLopHoc()
    {
        DataTable dt = DatabaseHelper.GetClassesByTeacher(_maGV);
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

            DataTable dtMon = DatabaseHelper.GetSubjectsByTeacherAndClass(_maGV, maLop);
            cbMonHoc.DataSource = dtMon;
            cbMonHoc.DisplayMember = "TenMon";
            cbMonHoc.ValueMember = "MaMon";
            cbMonHoc.SelectedIndex = -1;
            cbMonHoc.Text = "Chọn môn học";

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

    private void BtnLuu_Click(object sender, EventArgs e)
    {
        if (cbLop.SelectedValue == null) { MessageBox.Show("Vui lòng chọn lớp."); return; }
        if (cbMonHoc.SelectedValue == null) { MessageBox.Show("Vui lòng chọn môn học."); return; }
        if (cbHocSinh.SelectedValue == null) { MessageBox.Show("Vui lòng chọn học sinh."); return; }
        if (string.IsNullOrWhiteSpace(txtGhiChu.Text) || txtGhiChu.ForeColor == Color.Gray)
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_isRecording)
            {
                _waveIn?.StopRecording();
                _isRecording = false;
            }
            _waveIn?.Dispose();
            _audioStream?.Dispose();
            _processor?.Dispose();
            _whisperFactory?.Dispose();

            _waveIn = null;
            _audioStream = null;
            _processor = null;
            _whisperFactory = null;

            if (btnLuu != null) btnLuu.Click -= BtnLuu_Click;
            if (btnRecord != null) btnRecord.Click -= BtnRecord_Click_Async;
            if (cbLop != null) cbLop.SelectedIndexChanged -= CbLop_SelectedIndexChanged;
        }
        base.Dispose(disposing);
    }
}
