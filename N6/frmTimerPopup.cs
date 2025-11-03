using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;
using N6;

/// <summary>
/// Form popup hiển thị đồng hồ đếm ngược, hỗ trợ tạm dừng, cảnh báo và báo thức.
/// </summary>
public partial class frmTimerPopup : frmDraggableRoundedPopup
{
    private Timer countdownTimer;
    private DateTime endTime;
    private int warningMinutes = 5;
    private bool hasWarned = false;

    private Label lblTimeDisplay;
    private Label lblSubjectName;
    private ProgressBar progressBar;
    private Label lblStatus;
    private int totalSeconds;

    private bool isPaused = false;
    private TimeSpan? pausedRemaining = null;

    private RoundedButton btnToggle;
    private RoundedButton btnMuteAlarm;
    private Panel bottomPanel; // Thêm biến để gỡ sự kiện Resize

    private SoundPlayer _warningPlayer;
    private Timer _warningBeepTimer;
    private bool _isWarningRinging = false;

    // Biến lưu trữ sự kiện để gỡ bỏ
    private EventHandler btnToggleClickHandler;
    private EventHandler btnMuteAlarmClickHandler;
    private EventHandler bottomPanelResizeHandler;
    private EventHandler countdownTimerTickHandler;
    private EventHandler warningBeepTimerTickHandler;

    public frmTimerPopup() : base()
    {
        InitializeComponent(); // Gọi Designer (nếu có)
    }

    public frmTimerPopup(string subjectName, int durationMinutes, int warningBeforeEnd = 5) : base()
    {
        this.warningMinutes = warningBeforeEnd;
        this.totalSeconds = durationMinutes * 60;
        this.endTime = DateTime.Now.AddMinutes(durationMinutes);

        this.Text = "⏰ Đồng hồ Hẹn giờ";
        this.Size = new Size(380, 320);
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.TopMost = true;

        InitializeTimerComponent(subjectName);
        StartCountdown();
    }

    /// <summary>
    /// Khởi tạo và sắp xếp các control động.
    /// </summary>
    private void InitializeTimerComponent(string subjectName)
    {
        lblSubjectName = new Label { Text = "📚 " + subjectName, Dock = DockStyle.Top, Height = 35, Font = new Font("Segoe UI", 12F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.FromArgb(52, 73, 94) };
        lblTimeDisplay = new Label { Text = "00:00", Dock = DockStyle.Top, Height = 80, Font = new Font("Consolas", 42F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.FromArgb(41, 128, 185) };
        progressBar = new ProgressBar { Dock = DockStyle.Top, Height = 15, Maximum = totalSeconds, Value = 0, Style = ProgressBarStyle.Continuous, Margin = new Padding(20, 10, 20, 10) };
        lblStatus = new Label { Text = "⏳ Đang đếm ngược...", Dock = DockStyle.Top, Height = 26, Font = new Font("Segoe UI", 10F, FontStyle.Italic), TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Gray };

        bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 78 };

        btnToggle = new RoundedButton { Text = "⏸", Width = 120, Height = 48, BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, CornerRadius = 24, Font = new Font("Segoe UI", 18F, FontStyle.Bold), Anchor = AnchorStyles.None };
        btnMuteAlarm = new RoundedButton { Text = "🔕", Width = 50, Height = 48, BackColor = Color.FromArgb(149, 165, 166), ForeColor = Color.White, CornerRadius = 18, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Visible = false };

        // Định nghĩa các handler
        bottomPanelResizeHandler = (s, e) =>
        {
            btnToggle.Left = (bottomPanel.Width - btnToggle.Width) / 2;
            btnToggle.Top = (bottomPanel.Height - btnToggle.Height) / 2;
            btnMuteAlarm.Left = bottomPanel.Width - btnMuteAlarm.Width - 12;
            btnMuteAlarm.Top = (bottomPanel.Height - btnMuteAlarm.Height) / 2;
        };
        bottomPanel.Resize += bottomPanelResizeHandler;
        bottomPanelResizeHandler(bottomPanel, EventArgs.Empty); // Gọi lần đầu

        btnToggleClickHandler = (s, e) => { TogglePause(); };
        btnToggle.Click += btnToggleClickHandler;

        btnMuteAlarmClickHandler = (s, e) =>
        {
            StopWarningAlarm();
            lblStatus.Text = "🔕 Đã tắt chuông cảnh báo";
        };
        btnMuteAlarm.Click += btnMuteAlarmClickHandler;

        bottomPanel.Controls.Add(btnToggle);
        bottomPanel.Controls.Add(btnMuteAlarm);

        this.ContentPanel.Controls.Add(bottomPanel);
        this.ContentPanel.Controls.Add(lblStatus);
        this.ContentPanel.Controls.Add(progressBar);
        this.ContentPanel.Controls.Add(lblTimeDisplay);
        this.ContentPanel.Controls.Add(lblSubjectName);
    }

    /// <summary>
    /// Bật/Tắt tạm dừng đồng hồ.
    /// </summary>
    private void TogglePause()
    {
        if (!isPaused)
        {
            pausedRemaining = endTime - DateTime.Now;
            if (pausedRemaining.Value.TotalSeconds < 0) pausedRemaining = TimeSpan.Zero;
            countdownTimer?.Stop();
            isPaused = true;
            lblStatus.Text = "⏸️ Đã tạm dừng";
            btnToggle.Text = "▶";
            btnToggle.BackColor = Color.FromArgb(46, 204, 113);
        }
        else
        {
            if (pausedRemaining.HasValue)
            {
                endTime = DateTime.Now + pausedRemaining.Value;
                pausedRemaining = null;
            }
            countdownTimer?.Start();
            isPaused = false;
            lblStatus.Text = "⏳ Đang đếm ngược...";
            btnToggle.Text = "⏸";
            btnToggle.BackColor = Color.FromArgb(231, 76, 60);
        }
    }

    #region Countdown Logic (Logic đếm ngược)

    /// <summary>
    /// Khởi tạo và bắt đầu Timer đếm ngược.
    /// </summary>
    private void StartCountdown()
    {
        countdownTimer = new Timer { Interval = 1000 };
        countdownTimerTickHandler = new EventHandler(CountdownTimer_Tick);
        countdownTimer.Tick += countdownTimerTickHandler;
        countdownTimer.Start();
    }

    /// <summary>
    /// Xử lý sự kiện Tick mỗi giây của đồng hồ.
    /// </summary>
    private void CountdownTimer_Tick(object sender, EventArgs e)
    {
        if (isPaused) return;

        TimeSpan remaining = endTime - DateTime.Now;

        if (remaining.TotalSeconds <= 0)
        {
            countdownTimer.Stop();
            StopWarningAlarm();

            lblTimeDisplay.Text = "00:00";
            lblTimeDisplay.ForeColor = Color.FromArgb(231, 76, 60);
            lblStatus.Text = "🔔 Đã hết giờ!";
            progressBar.Value = progressBar.Maximum;

            StartWarningAlarm(true); // Bắt đầu báo thức Hết giờ
            return;
        }

        int minutes = (int)remaining.TotalMinutes;
        int seconds = remaining.Seconds;
        lblTimeDisplay.Text = $"{minutes:D2}:{seconds:D2}";

        int elapsed = totalSeconds - (int)remaining.TotalSeconds;
        if (elapsed >= 0 && elapsed <= progressBar.Maximum)
        {
            progressBar.Value = elapsed;
        }

        // Cảnh báo (ví dụ: 5 phút cuối)
        if (remaining.TotalMinutes <= warningMinutes && !hasWarned)
        {
            hasWarned = true;
            lblTimeDisplay.ForeColor = Color.FromArgb(241, 196, 15);
            lblStatus.Text = $"⚠️ Còn {warningMinutes} phút!";
            this.BackColor = Color.FromArgb(254, 249, 231);
            ShowWarning(warningMinutes);
        }

        // Cảnh báo (1 phút cuối)
        if (remaining.TotalMinutes < 1 && hasWarned) // Đảm bảo chỉ đổi màu sau cảnh báo đầu
        {
            lblTimeDisplay.ForeColor = Color.FromArgb(231, 76, 60);
            lblStatus.Text = "🔴 Sắp hết giờ!";
            this.BackColor = Color.FromArgb(255, 235, 238);
        }
    }

    #endregion

    #region Alarm & Warning (Báo thức & Cảnh báo)

    /// <summary>
    /// Kích hoạt cảnh báo (âm thanh, nháy màn hình).
    /// </summary>
    private void ShowWarning(int minutesLeft)
    {
        StartWarningAlarm(false); // Bắt đầu báo thức Cảnh báo
        btnMuteAlarm.Visible = true;
        BlinkForm();
    }

    /// <summary>
    /// Bắt đầu phát âm thanh báo thức (Cảnh báo hoặc Hết giờ).
    /// </summary>
    private void StartWarningAlarm(bool isFinalAlarm)
    {
        if (_isWarningRinging) return;
        _isWarningRinging = true;

        StopWarningAlarmInternal(disposeOnly: true); // Dọn dẹp timer/player cũ

        try
        {
            // Ưu tiên file wav
            string wavPath = isFinalAlarm
                ? @"..\..\Resources\alarm-clock-warning.wav" // Thay bằng file báo hết giờ
                : @"..\..\Resources\alarm-clock-warning.wav";

            if (File.Exists(wavPath))
            {
                _warningPlayer = new SoundPlayer(wavPath);
                _warningPlayer.PlayLooping();
                return;
            }
        }
        catch { }

        // Nếu không có file wav, dùng tiếng Beep
        _warningBeepTimer = new Timer { Interval = isFinalAlarm ? 400 : 800 }; // Hết giờ kêu nhanh hơn
        warningBeepTimerTickHandler = (s, e) =>
        {
            try { SystemSounds.Exclamation.Play(); } catch { }
        };
        _warningBeepTimer.Tick += warningBeepTimerTickHandler;
        _warningBeepTimer.Start();
    }

    /// <summary>
    /// Dừng báo thức (khi người dùng nhấn nút Mute).
    /// </summary>
    private void StopWarningAlarm()
    {
        StopWarningAlarmInternal(disposeOnly: false);
        btnMuteAlarm.Visible = false;
        _isWarningRinging = false;
    }

    /// <summary>
    /// Lõi xử lý dừng âm thanh.
    /// </summary>
    private void StopWarningAlarmInternal(bool disposeOnly)
    {
        if (_warningBeepTimer != null)
        {
            _warningBeepTimer.Stop();
            if (warningBeepTimerTickHandler != null)
            {
                _warningBeepTimer.Tick -= warningBeepTimerTickHandler;
            }
            _warningBeepTimer.Dispose();
            _warningBeepTimer = null;
        }

        if (_warningPlayer != null)
        {
            try { _warningPlayer.Stop(); } catch { }
            if (!disposeOnly)
            {
                _warningPlayer.Dispose();
                _warningPlayer = null;
            }
        }
    }

    /// <summary>
    /// Làm Form nhấp nháy 3 lần.
    /// </summary>
    private async void BlinkForm()
    {
        for (int i = 0; i < 3; i++)
        {
            if (this.IsDisposed) return;
            this.Opacity = 0.5;
            await System.Threading.Tasks.Task.Delay(200);
            if (this.IsDisposed) return;
            this.Opacity = 1.0;
            await System.Threading.Tasks.Task.Delay(200);
        }
    }

    #endregion

    /// <summary>
    /// Dọn dẹp tài nguyên (Timers, SoundPlayers, Sự kiện).
    /// </summary>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        StopWarningAlarmInternal(disposeOnly: true); // Dừng và dọn dẹp âm thanh

        if (countdownTimer != null)
        {
            countdownTimer.Stop();
            countdownTimer.Tick -= countdownTimerTickHandler;
            countdownTimer.Dispose();
            countdownTimer = null;
        }

        base.OnFormClosing(e); // Gọi base.OnFormClosing
    }

    // Ghi đè Dispose (từ frmDraggableRoundedPopup)
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dọn dẹp Timer và SoundPlayer (đảm bảo)
            if (countdownTimer != null)
            {
                countdownTimer.Stop();
                countdownTimer.Tick -= countdownTimerTickHandler;
                countdownTimer.Dispose();
                countdownTimer = null;
            }
            StopWarningAlarmInternal(disposeOnly: true);

            // Gỡ bỏ sự kiện
            if (btnToggle != null) btnToggle.Click -= btnToggleClickHandler;
            if (btnMuteAlarm != null) btnMuteAlarm.Click -= btnMuteAlarmClickHandler;
            if (bottomPanel != null) bottomPanel.Resize -= bottomPanelResizeHandler;
        }
        base.Dispose(disposing);
    }
}