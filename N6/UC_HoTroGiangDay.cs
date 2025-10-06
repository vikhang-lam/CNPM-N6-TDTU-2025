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
        private Bitmap _canvas;
        private bool _isDrawing;
        private bool _eraserMode;
        private Point _lastPoint;
        private Color _penColor = Color.Black;
        private int _penWidth = 4;

        // Auto-save ghi chú
        private Timer _notesAutoSaveTimer;
        private string _notesFilePath;

        public UC_HoTroGiangDay()
        {
            InitializeComponent();
            if (cboWidth.Items.Count > 0) cboWidth.SelectedIndex = 1; // "4"
            SetupNotesPersistence();
        }

        private void UC_HoTroGiangDay_Load(object sender, EventArgs e)
        {
            EnsureCanvas();
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

        private void EnsureCanvas()
        {
            if (panelCanvas.Width <= 0 || panelCanvas.Height <= 0) return;

            if (_canvas == null)
            {
                _canvas = new Bitmap(panelCanvas.Width, panelCanvas.Height);
                using (Graphics g = Graphics.FromImage(_canvas)) g.Clear(Color.White);
            }
            else if (_canvas.Width != panelCanvas.Width || _canvas.Height != panelCanvas.Height)
            {
                var newBmp = new Bitmap(panelCanvas.Width, panelCanvas.Height);
                using (Graphics g = Graphics.FromImage(newBmp))
                {
                    g.Clear(Color.White);
                    g.DrawImage(_canvas, Point.Empty);
                }
                _canvas.Dispose();
                _canvas = newBmp;
            }
            panelCanvas.Invalidate();
        }

        private void panelCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (_canvas != null) e.Graphics.DrawImageUnscaled(_canvas, 0, 0);
        }

        private void panelCanvas_Resize(object sender, EventArgs e)
        {
            EnsureCanvas();
        }

        private void panelCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _isDrawing = true;
            _lastPoint = e.Location;
        }

        private void panelCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing || _canvas == null) return;

            using (Graphics g = Graphics.FromImage(_canvas))
            using (Pen pen = new Pen(_eraserMode ? Color.White : _penColor, _penWidth)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                EndCap = System.Drawing.Drawing2D.LineCap.Round,
                LineJoin = System.Drawing.Drawing2D.LineJoin.Round
            })
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawLine(pen, _lastPoint, e.Location);
            }

            _lastPoint = e.Location;
            panelCanvas.Invalidate(new Rectangle(e.X - _penWidth, e.Y - _penWidth, _penWidth * 2, _penWidth * 2));
        }

        private void panelCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _isDrawing = false;
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            using (var dlg = new ColorDialog { Color = _penColor })
            {
                if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
                {
                    _penColor = dlg.Color;
                    if (_eraserMode) btnEraser.Checked = false;
                }
            }
        }

        private void btnEraser_CheckedChanged(object sender, EventArgs e)
        {
            _eraserMode = btnEraser.Checked;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (_canvas == null) return;
            using (Graphics g = Graphics.FromImage(_canvas)) g.Clear(Color.White);
            panelCanvas.Invalidate();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_canvas == null) return;

            using (var sfd = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                FileName = $"Whiteboard_{DateTime.Now:yyyyMMdd_HHmmss}.png"
            })
            {
                if (sfd.ShowDialog(FindForm()) == DialogResult.OK)
                {
                    _canvas.Save(sfd.FileName, ImageFormat.Png);
                    MessageBox.Show("Đã lưu ảnh bảng trắng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void cboWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(cboWidth.SelectedItem?.ToString(), out int w)) _penWidth = w;
        }

        private void DisposeCanvas()
        {
            if (_canvas != null)
            {
                _canvas.Dispose();
                _canvas = null;
            }
            if (_notesAutoSaveTimer != null)
            {
                _notesAutoSaveTimer.Stop();
                _notesAutoSaveTimer.Dispose();
                _notesAutoSaveTimer = null;
            }
        }
    }
}