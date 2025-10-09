using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace N6
{
    public partial class PopupWhiteboard : Form
    {
        private Bitmap _canvas;
        private bool _isDrawing;
        private bool _eraserMode;
        private Point _lastPoint;
        private Color _penColor = Color.Black;
        private int _penWidth = 4;
        
        // Minimized state
        private bool _isMinimized = false;
        private Size _originalSize;
        private Point _originalLocation;
        private Button _minimizedButton;
        
        // Form dragging
        private bool _isDraggingForm = false;
        private Point _formDragStartPoint;

        public PopupWhiteboard()
        {
            InitializeComponent();
            InitializePopup();
            EnsureCanvas();
        }

        private void InitializePopup()
        {
            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(700, 500);
            this.BackColor = Color.White;
            
            // Position at top-right corner
            this.Location = new Point(
                Screen.PrimaryScreen.WorkingArea.Width - this.Width - 20,
                50
            );
            
            _originalSize = this.Size;
            _originalLocation = this.Location;

            // Add rounded corners
            this.Region = CreateRoundedRegion(this.Size, 15);
            
            // Set default pen width
            if (cboWidth.Items.Count > 0) 
                cboWidth.SelectedIndex = 1; // "4"

            // Create minimized button (initially hidden)
            CreateMinimizedButton();
            
            // Add drag functionality to form
            EnableFormDragging();
        }

        private void EnableFormDragging()
        {
            // Make the toolbar draggable
            toolStrip.MouseDown += ToolStrip_MouseDown;
            toolStrip.MouseMove += ToolStrip_MouseMove;
            toolStrip.MouseUp += ToolStrip_MouseUp;
            
            // Make the title label draggable
            toolStripLabel1.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left) {
                    _isDraggingForm = true;
                    _formDragStartPoint = new Point(e.X, e.Y);
                }
            };
        }

        private void ToolStrip_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !_isMinimized)
            {
                _isDraggingForm = true;
                _formDragStartPoint = e.Location;
            }
        }

        private void ToolStrip_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDraggingForm && !_isMinimized)
            {
                Point currentScreen = PointToScreen(e.Location);
                this.Location = new Point(
                    currentScreen.X - _formDragStartPoint.X,
                    currentScreen.Y - _formDragStartPoint.Y
                );
            }
        }

        private void ToolStrip_MouseUp(object sender, MouseEventArgs e)
        {
            _isDraggingForm = false;
        }

        private void CreateMinimizedButton()
        {
            _minimizedButton = new Button();
            _minimizedButton.Size = new Size(60, 60);
            _minimizedButton.BackColor = Color.FromArgb(0, 122, 255);
            _minimizedButton.ForeColor = Color.White;
            _minimizedButton.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            _minimizedButton.Text = "W"; // W for Whiteboard
            _minimizedButton.FlatStyle = FlatStyle.Flat;
            _minimizedButton.FlatAppearance.BorderSize = 0;
            _minimizedButton.Cursor = Cursors.Hand;
            _minimizedButton.Click += MinimizedButton_Click;
            _minimizedButton.MouseDown += MinimizedButton_MouseDown;
            _minimizedButton.MouseMove += MinimizedButton_MouseMove;
            _minimizedButton.MouseUp += MinimizedButton_MouseUp;
            _minimizedButton.Visible = false;
            
            // Make it circular
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, 60, 60);
            _minimizedButton.Region = new Region(path);
            
            this.Controls.Add(_minimizedButton);
        }

        private Region CreateRoundedRegion(Size size, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(size.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(size.Width - radius, size.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, size.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return new Region(path);
        }

        private void EnsureCanvas()
        {
            if (panelCanvas == null || panelCanvas.Width <= 0 || panelCanvas.Height <= 0) return;

            if (_canvas == null)
            {
                _canvas = new Bitmap(panelCanvas.Width, panelCanvas.Height);
                using (Graphics g = Graphics.FromImage(_canvas)) 
                    g.Clear(Color.White);
            }
            else if (_canvas.Width != panelCanvas.Width || _canvas.Height != panelCanvas.Height)
            {
                var newBmp = new Bitmap(panelCanvas.Width, panelCanvas.Height);
                using (Graphics g = Graphics.FromImage(newBmp))
                {
                    g.Clear(Color.White);
                    if (_canvas != null)
                        g.DrawImage(_canvas, Point.Empty);
                }
                _canvas?.Dispose();
                _canvas = newBmp;
            }
            panelCanvas?.Invalidate();
        }

        public void ToggleMinimize()
        {
            if (_isMinimized)
            {
                // Restore to full size
                this.Size = _originalSize;
                this.Location = _originalLocation;
                this.Region = CreateRoundedRegion(this.Size, 15);
                
                // Show main panel, hide minimized button
                if (mainPanel != null) mainPanel.Visible = true;
                if (_minimizedButton != null) _minimizedButton.Visible = false;
                _isMinimized = false;
                
                // Re-ensure canvas after restore
                this.Invoke(new Action(() => {
                    EnsureCanvas();
                }));
            }
            else
            {
                // Store current state before minimizing
                if (!_isMinimized) // Only store if not already minimized
                {
                    _originalSize = this.Size;
                    _originalLocation = this.Location;
                }
                
                // Minimize to small button
                this.Size = new Size(60, 60);
                this.Location = new Point(
                    Screen.PrimaryScreen.WorkingArea.Width - 80,
                    Screen.PrimaryScreen.WorkingArea.Height - 150
                );
                
                // Create circular region
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(0, 0, 60, 60);
                this.Region = new Region(path);
                
                // Hide main panel, show minimized button
                if (mainPanel != null) mainPanel.Visible = false;
                if (_minimizedButton != null) 
                {
                    _minimizedButton.Dock = DockStyle.Fill;
                    _minimizedButton.Visible = true;
                    _minimizedButton.BringToFront();
                }
                _isMinimized = true;
            }
        }

        #region Drawing Events
        private void panelCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (_canvas != null) 
                e.Graphics.DrawImageUnscaled(_canvas, 0, 0);
        }

        private void panelCanvas_Resize(object sender, EventArgs e)
        {
            EnsureCanvas();
        }

        private void panelCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _isMinimized) return;
            _isDrawing = true;
            _lastPoint = e.Location;
        }

        private void panelCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing || _canvas == null || _isMinimized) return;

            using (Graphics g = Graphics.FromImage(_canvas))
            using (Pen pen = new Pen(_eraserMode ? Color.White : _penColor, _penWidth)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            })
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
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
        #endregion

        #region Tool Events
        private void btnColor_Click(object sender, EventArgs e)
        {
            using (var dlg = new ColorDialog { Color = _penColor })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _penColor = dlg.Color;
                    _eraserMode = false;
                    if (btnEraser != null) btnEraser.Checked = false;
                }
            }
        }

        private void btnEraser_CheckedChanged(object sender, EventArgs e)
        {
            _eraserMode = btnEraser?.Checked ?? false;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (_canvas == null) return;
            using (Graphics g = Graphics.FromImage(_canvas)) 
                g.Clear(Color.White);
            panelCanvas?.Invalidate();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_canvas == null) return;

            using (var sfd = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                FileName = $"PopupWhiteboard_{DateTime.Now:yyyyMMdd_HHmmss}.png"
            })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    _canvas.Save(sfd.FileName, ImageFormat.Png);
                    MessageBox.Show("Da luu anh bang trang.", "Thong bao", 
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            ToggleMinimize();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void cboWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(cboWidth?.SelectedItem?.ToString(), out int w)) 
                _penWidth = w;
        }
        #endregion

        #region Minimized Button Events
        private bool _isDraggingButton = false;
        private Point _buttonDragStartPoint;

        private void MinimizedButton_Click(object sender, EventArgs e)
        {
            if (!_isDraggingButton)
                ToggleMinimize();
        }

        private void MinimizedButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDraggingButton = true;
                _buttonDragStartPoint = e.Location;
            }
        }

        private void MinimizedButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDraggingButton)
            {
                Point currentScreen = PointToScreen(e.Location);
                this.Location = new Point(
                    currentScreen.X - _buttonDragStartPoint.X, 
                    currentScreen.Y - _buttonDragStartPoint.Y
                );
            }
        }

        private void MinimizedButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isDraggingButton)
            {
                _isDraggingButton = false;
                // Small delay to prevent click event after drag
                Timer timer = new Timer();
                timer.Interval = 150;
                timer.Tick += (s, args) => { 
                    _isDraggingButton = false;
                    timer.Stop(); 
                    timer.Dispose(); 
                };
                timer.Start();
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _canvas?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PopupWhiteboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
