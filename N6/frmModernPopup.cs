using System; // Thêm
using System.Drawing;
using System.Drawing.Drawing2D; // Thêm
using System.Windows.Forms;

/// <summary>
/// Lớp Form cơ sở tùy chỉnh, không viền, bo góc, và có thể kéo thả.
/// Sử dụng TransparencyKey để tạo hiệu ứng "khoét rỗng" (cho phép bo góc).
/// </summary>
public class frmModernPopup : Form
{
    private bool isDragging = false;
    private Point dragStartPoint;
    private Label lblTitle;

    /// <summary>
    /// Panel chính chứa nội dung của form popup.
    /// Các control con nên được thêm vào đây.
    /// </summary>
    public Panel ContentPanel;

    // Các biến để lưu trữ control/sự kiện (dùng cho việc Dispose)
    private Panel topPanel;
    private Label btnClose;
    private EventHandler btnCloseClickHandler;
    private EventHandler btnCloseEnterHandler;
    private EventHandler btnCloseLeaveHandler;
    private MouseEventHandler mouseDownHandler;
    private MouseEventHandler mouseMoveHandler;
    private MouseEventHandler mouseUpHandler;

    /// <summary>
    /// Ghi đè thuộc tính Text để tự động cập nhật lblTitle.
    /// </summary>
    public override string Text
    {
        get { return base.Text; }
        set { base.Text = value; if (lblTitle != null) lblTitle.Text = value; }
    }

    public frmModernPopup()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = Color.Gainsboro; // Màu này sẽ bị làm trong suốt
        this.Padding = new Padding(1);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.TransparencyKey = Color.Gainsboro; // Làm trong suốt nền

        var mainPanel = new RoundedPanel // Giả định RoundedPanel là một class tùy chỉnh
        {
            Dock = DockStyle.Fill,
            CornerRadius = 15,
            BackColor = Color.White
        };

        topPanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.Transparent };
        lblTitle = new Label { Dock = DockStyle.Fill, Text = this.Text, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.DimGray, TextAlign = ContentAlignment.MiddleCenter, Padding = new Padding(30, 0, 30, 0) };

        btnClose = new Label { Text = "✕", Dock = DockStyle.Right, Width = 40, Font = new Font("Segoe UI", 12F), ForeColor = Color.Gray, TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand };

        // Gán sự kiện và lưu trữ handler
        btnCloseClickHandler = (s, e) => this.Close();
        btnClose.Click += btnCloseClickHandler;

        btnCloseEnterHandler = (s, e) => btnClose.BackColor = Color.LightCoral;
        btnClose.MouseEnter += btnCloseEnterHandler;

        btnCloseLeaveHandler = (s, e) => btnClose.BackColor = Color.Transparent;
        btnClose.MouseLeave += btnCloseLeaveHandler;

        topPanel.Controls.Add(lblTitle);
        topPanel.Controls.Add(btnClose);

        ContentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(15) };

        mainPanel.Controls.Add(ContentPanel);
        mainPanel.Controls.Add(topPanel);
        this.Controls.Add(mainPanel);

        // Gán sự kiện kéo thả và lưu trữ handler
        mouseDownHandler = OnMouseDown;
        mouseMoveHandler = OnMouseMove;
        mouseUpHandler = OnMouseUp;

        topPanel.MouseDown += mouseDownHandler;
        topPanel.MouseMove += mouseMoveHandler;
        topPanel.MouseUp += mouseUpHandler;
        lblTitle.MouseDown += mouseDownHandler;
        lblTitle.MouseMove += mouseMoveHandler;
        lblTitle.MouseUp += mouseUpHandler;
    }

    #region Drag Logic (Logic kéo thả)

    private void OnMouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            isDragging = true;
            dragStartPoint = new Point(e.X, e.Y);
        }
    }
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (isDragging)
        {
            this.Location = new Point(this.Location.X + (e.X - dragStartPoint.X), this.Location.Y + (e.Y - dragStartPoint.Y));
        }
    }
    private void OnMouseUp(object sender, MouseEventArgs e)
    {
        isDragging = false;
    }

    #endregion

    #region Dispose (Dọn dẹp)

    /// <summary>
    /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Gỡ bỏ sự kiện của các control động
            if (btnClose != null)
            {
                btnClose.Click -= btnCloseClickHandler;
                btnClose.MouseEnter -= btnCloseEnterHandler;
                btnClose.MouseLeave -= btnCloseLeaveHandler;
            }
            if (topPanel != null)
            {
                topPanel.MouseDown -= mouseDownHandler;
                topPanel.MouseMove -= mouseMoveHandler;
                topPanel.MouseUp -= mouseUpHandler;
            }
            if (lblTitle != null)
            {
                lblTitle.MouseDown -= mouseDownHandler;
                lblTitle.MouseMove -= mouseMoveHandler;
                lblTitle.MouseUp -= mouseUpHandler;
            }
        }
        base.Dispose(disposing);
    }

    #endregion
}