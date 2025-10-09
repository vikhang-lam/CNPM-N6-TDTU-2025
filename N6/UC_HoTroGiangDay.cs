using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_HoTroGiangDay : UserControl
    {
        // Auto-save ghi chú
        private Timer _notesAutoSaveTimer;
        private string _notesFilePath;

        // Popup whiteboard
        private PopupWhiteboard _popupWhiteboard;

        public UC_HoTroGiangDay()
        {
            InitializeComponent();
            SetupNotesPersistence();
        }

        private void UC_HoTroGiangDay_Load(object sender, EventArgs e)
        {
            // No longer need canvas setup since we're using popup
        }

        private void SetupNotesPersistence()
        {
            try
            {
                // Tạo file ghi chú theo user hiện tại
                string user = Properties.Settings.Default["CurrentUser"]?.ToString();
                if (string.IsNullOrWhiteSpace(user)) user = "guest";
                foreach (var ch in Path.GetInvalidFileNameChars())
                    user = user.Replace(ch, '_');

                string notesDir = Path.Combine(Application.StartupPath, "Notes");
                if (!Directory.Exists(notesDir)) Directory.CreateDirectory(notesDir);

                _notesFilePath = Path.Combine(notesDir, $"{user}.txt");

                // Nạp ghi chú cũ nếu có
                if (File.Exists(_notesFilePath))
                {
                    txtNotes.Text = File.ReadAllText(_notesFilePath, Encoding.UTF8);
                }

                // Auto-save sau khi dừng gõ 1s
                _notesAutoSaveTimer = new Timer { Interval = 1000 };
                _notesAutoSaveTimer.Tick += (s, e) =>
                {
                    _notesAutoSaveTimer.Stop();
                    SaveNotesToFile();
                };

                txtNotes.TextChanged += (s, e) =>
                {
                    if (_notesAutoSaveTimer == null) return;
                    _notesAutoSaveTimer.Stop();
                    _notesAutoSaveTimer.Start();
                };
            }
            catch
            {
                // Không làm app crash nếu có lỗi quyền ghi file
            }
        }

        private void SaveNotesToFile()
        {
            try
            {
                if (string.IsNullOrEmpty(_notesFilePath)) return;
                File.WriteAllText(_notesFilePath, txtNotes.Text ?? string.Empty, Encoding.UTF8);
            }
            catch
            {
                // Có thể bổ sung MessageBox nếu cần cảnh báo không lưu được
            }
        }

        private void btnOpenPopupWhiteboard_Click(object sender, EventArgs e)
        {
            if (_popupWhiteboard == null || _popupWhiteboard.IsDisposed)
            {
                _popupWhiteboard = new PopupWhiteboard();
            }

            if (_popupWhiteboard.Visible)
            {
                _popupWhiteboard.Hide();
                btnOpenPopupWhiteboard.Text = "🖊️ Mở Bảng Trắng Popup";
                btnOpenPopupWhiteboard.BackColor = Color.FromArgb(0, 122, 255);
            }
            else
            {
                _popupWhiteboard.Show();
                _popupWhiteboard.BringToFront();
                btnOpenPopupWhiteboard.Text = "🖊️ Ẩn Bảng Trắng Popup";
                btnOpenPopupWhiteboard.BackColor = Color.FromArgb(255, 69, 58);
            }
        }

        private void DisposeResources()
        {
            if (_notesAutoSaveTimer != null)
            {
                _notesAutoSaveTimer.Stop();
                _notesAutoSaveTimer.Dispose();
                _notesAutoSaveTimer = null;
            }
            
            if (_popupWhiteboard != null && !_popupWhiteboard.IsDisposed)
            {
                _popupWhiteboard.Close();
                _popupWhiteboard.Dispose();
                _popupWhiteboard = null;
            }
        }
    }
}