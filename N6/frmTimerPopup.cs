using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;
using N6;

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

    // Large centered toggle button
    private RoundedButton btnToggle;

    // NEW: button to stop/mute the warning alarm
    private RoundedButton btnMuteAlarm;

    // Warning alarm resources (loop until user stops)
    private SoundPlayer _warningPlayer;
    private Timer _warningBeepTimer; // fallback repeating beeps
    private bool _isWarningRinging = false;

    public frmTimerPopup() : base()
    {
        InitializeComponent();
    }

    public frmTimerPopup(string subjectName, int durationMinutes, int warningBeforeEnd = 5) : base()
    {
        InitializeComponent();

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

    private void InitializeTimerComponent(string subjectName)
    {
        lblSubjectName = new Label
        {
            Text = "📚 " + subjectName,
            Dock = DockStyle.Top,
            Height = 35,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(52, 73, 94)
        };

        lblTimeDisplay = new Label
        {
            Text = "00:00",
            Dock = DockStyle.Top,
            Height = 80,
            Font = new Font("Consolas", 42F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(41, 128, 185)
        };

        progressBar = new ProgressBar
        {
            Dock = DockStyle.Top,
            Height = 15,
            Maximum = totalSeconds,
            Value = 0,
            Style = ProgressBarStyle.Continuous,
            Margin = new Padding(20, 10, 20, 10)
        };

        lblStatus = new Label
        {
            Text = "⏳ Đang đếm ngược...",
            Dock = DockStyle.Top,
            Height = 26,
            Font = new Font("Segoe UI", 10F, FontStyle.Italic),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.Gray
        };

        // Bottom container to center the big toggle button
        var bottomPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 78
        };

        btnToggle = new RoundedButton
        {
            Text = "⏸",
            Width = 120,
            Height = 48,
            BackColor = Color.FromArgb(231, 76, 60),
            ForeColor = Color.White,
            CornerRadius = 24,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold)
        };
        btnToggle.Anchor = AnchorStyles.None;

        btnMuteAlarm = new RoundedButton
        {
            Text = "🔕",
            Width = 50,
            Height = 48,
            BackColor = Color.FromArgb(149, 165, 166),
            ForeColor = Color.White,
            CornerRadius = 18,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Visible = false
        };
        btnMuteAlarm.Click += (s, e) =>
        {
            StopWarningAlarm();
            lblStatus.Text = "🔕 Đã tắt chuông cảnh báo";
        };

        bottomPanel.Resize += (s, e) =>
        {
            btnToggle.Left = (bottomPanel.Width - btnToggle.Width) / 2;
            btnToggle.Top = (bottomPanel.Height - btnToggle.Height) / 2;

            btnMuteAlarm.Left = bottomPanel.Width - btnMuteAlarm.Width - 12;
            btnMuteAlarm.Top = (bottomPanel.Height - btnMuteAlarm.Height) / 2;
        };

        btnToggle.Click += (s, e) =>
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
        };

        bottomPanel.Controls.Add(btnToggle);
        bottomPanel.Controls.Add(btnMuteAlarm);

        // Build layout
        this.ContentPanel.Controls.Add(bottomPanel);
        this.ContentPanel.Controls.Add(lblStatus);
        this.ContentPanel.Controls.Add(progressBar);
        this.ContentPanel.Controls.Add(lblTimeDisplay);
        this.ContentPanel.Controls.Add(lblSubjectName);
    }

    private void StartCountdown()
    {
        countdownTimer = new Timer { Interval = 1000 };
        countdownTimer.Tick += CountdownTimer_Tick;
        countdownTimer.Start();
    }

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

            StartWarningAlarm();
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

        if (remaining.TotalMinutes <= warningMinutes &&
            remaining.TotalMinutes > (warningMinutes - 0.05) &&
            !hasWarned)
        {
            hasWarned = true;
            lblTimeDisplay.ForeColor = Color.FromArgb(241, 196, 15);
            lblStatus.Text = $"⚠️ Còn {warningMinutes} phút!";
            this.BackColor = Color.FromArgb(254, 249, 231);

            ShowWarning(warningMinutes);
        }

        if (remaining.TotalMinutes <= 1)
        {
            lblTimeDisplay.ForeColor = Color.FromArgb(231, 76, 60);
            lblStatus.Text = "🔴 Sắp hết giờ!";
            this.BackColor = Color.FromArgb(255, 235, 238);
        }
    }

    private void ShowWarning(int minutesLeft)
    {
        StartWarningAlarm();
        btnMuteAlarm.Visible = true;
        BlinkForm();
    }

    private void StartWarningAlarm()
    {
        if (_isWarningRinging) return;
        _isWarningRinging = true;

        StopWarningAlarmInternal(disposeOnly: true);

        try
        {
            string wavPath = @"..\..\Resources\alarm-clock-warning.wav";

            if (File.Exists(wavPath))
            {
                _warningPlayer = new SoundPlayer(wavPath);
                _warningPlayer.PlayLooping();
                return;
            }
        }
        catch { }

        // Fallback: beep repeatedly until stopped
        _warningBeepTimer = new Timer { Interval = 600 };
        _warningBeepTimer.Tick += (s, e) =>
        {
            try { SystemSounds.Exclamation.Play(); } catch { }
        };
        _warningBeepTimer.Start();
    }

    private void StopWarningAlarm()
    {
        StopWarningAlarmInternal(disposeOnly: false);
        btnMuteAlarm.Visible = false;
        _isWarningRinging = false;
    }

    private void StopWarningAlarmInternal(bool disposeOnly)
    {
        if (_warningBeepTimer != null)
        {
            _warningBeepTimer.Stop();
            _warningBeepTimer.Tick -= (s, e) => { }; // safe detach no-op
            _warningBeepTimer.Dispose();
            _warningBeepTimer = null;
        }

        if (_warningPlayer != null)
        {
            try { _warningPlayer.Stop(); } catch { }
            if (!disposeOnly)
            {
                _warningPlayer.Dispose();
            }
            _warningPlayer = null;
        }
    }
    // ===========================================

    private void ShowEndNotification()
    {
        try { SystemSounds.Asterisk.Play(); } catch { }

        MessageBox.Show(
            "🔔 Đã hết giờ tiết học!\n\nChúc các bạn học tốt!",
            "Kết thúc",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }

    private async void BlinkForm()
    {
        for (int i = 0; i < 3; i++)
        {
            this.Opacity = 0.5;
            await System.Threading.Tasks.Task.Delay(200);
            this.Opacity = 1.0;
            await System.Threading.Tasks.Task.Delay(200);
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        countdownTimer?.Stop();
        countdownTimer?.Dispose();
        countdownTimer = null;

        // Ensure alarm is stopped/cleaned
        StopWarningAlarm();

        base.OnFormClosing(e);
    }
}