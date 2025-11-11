using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace N6
{
    public abstract class GameFormWithMusic : BaseGameForm
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            MusicPlayer.Stop();
        }

        private string GetResourceFilePath(string fileName)
        {
            string resourcesPath = MusicPlayer.GetResourcesDirectory();
            return Path.Combine(resourcesPath, fileName);
        }

        private void SetFormRoundedRegion(Form form, int radius)
        {
            // Tạo một đối tượng GraphicsPath để định nghĩa hình dạng
            GraphicsPath path = new GraphicsPath();
            int w = form.Width;
            int h = form.Height;

            // Đảm bảo Form không bị bo tròn quá mức (radius không lớn hơn nửa chiều rộng/cao)
            if (radius > w / 2 || radius > h / 2)
            {
                radius = Math.Min(w / 2, h / 2);
            }

            // Định nghĩa hình dạng bo tròn (sử dụng đường cong cung tròn - AddArc)
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);           // Góc trên bên trái
            path.AddArc(w - radius * 2, 0, radius * 2, radius * 2, 270, 90); // Góc trên bên phải
            path.AddArc(w - radius * 2, h - radius * 2, radius * 2, radius * 2, 0, 90); // Góc dưới bên phải
            path.AddArc(0, h - radius * 2, radius * 2, radius * 2, 90, 90); // Góc dưới bên trái
            path.CloseAllFigures();

            // Áp dụng hình dạng đã tạo cho Region của Form
            form.Region = new Region(path);
        }

        public enum AnswerPopupResult 
        { 
            Next, Retry, None 
        }

        protected AnswerPopupResult ShowAnswerPopup(bool isCorrect, string message, string primaryText = "Tiếp theo", bool showRetry = true)
        {
            using (Form dlg = new Form())
            {
                dlg.FormBorderStyle = FormBorderStyle.None;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.Size = new Size(460, 180);
                dlg.ShowInTaskbar = false;
                dlg.TopMost = true;
                //dlg.BackColor = isCorrect ? Color.FromArgb(230, 255, 240) : Color.FromArgb(255, 230, 230);
                dlg.Font = new Font("Lexend", 11F);
                dlg.ControlBox = false;

                try
                {
                    string imagePath = GetResourceFilePath("popup_bg.png"); // Đổi tên file phù hợp
                    if (File.Exists(imagePath))
                    {
                        dlg.BackgroundImage = Image.FromFile(imagePath);

                        // SỬA: Đặt chế độ hiển thị Stretch để ảnh vừa Form
                        dlg.BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    else
                    {
                        // Fallback nếu không tìm thấy ảnh
                        dlg.BackColor = isCorrect ? Color.FromArgb(230, 255, 240) : Color.FromArgb(255, 230, 230);
                    }
                }
                catch (Exception)
                {
                    dlg.BackColor = isCorrect ? Color.FromArgb(230, 255, 240) : Color.FromArgb(255, 230, 230);
                }

                Label lblMessage = new Label
                {
                    Text = message,
                    Dock = DockStyle.Top,
                    Height = 80,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Lexend", 18F, FontStyle.Bold),
                    ForeColor = isCorrect ? Color.FromArgb(40, 167, 69) : Color.FromArgb(220, 53, 69),
                    BackColor = Color.Transparent,
                    Padding = new Padding(0, 10, 0, 0)
                };

                Panel pnlButtons = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 70,
                    Padding = new Padding(20),
                    BackColor = Color.Transparent
                };


                Size btnSize = new Size(160, 40);

                RoundedButton btnRetry = new RoundedButton
                {
                    Text = "Thử lại",
                    Size = btnSize,
                    BackColor = Color.FromArgb(255, 193, 7),
                    ForeColor = Color.Black,
                    CornerRadius = 10,
                    Font = new Font("Lexend", 12F, FontStyle.Bold),
                };
                btnRetry.Click += (s, e) => { dlg.Tag = "RETRY"; dlg.Close(); };

                RoundedButton btnNext = new RoundedButton
                {
                    Text = primaryText,
                    Size = btnSize,
                    BackColor = Color.FromArgb(87, 187, 247),
                    ForeColor = Color.White,
                    CornerRadius = 10,
                    Font = new Font("Lexend", 12F, FontStyle.Bold),
                };
                btnNext.Click += (s, e) => { dlg.Tag = "NEXT"; dlg.Close(); };

                if (showRetry)
                {
                    btnRetry.Location = new Point(40, (pnlButtons.Height - btnSize.Height) / 2);
                    btnNext.Location = new Point(dlg.ClientSize.Width - 40 - btnSize.Width, (pnlButtons.Height - btnSize.Height) / 2);
                    pnlButtons.Controls.Add(btnRetry);
                    pnlButtons.Controls.Add(btnNext);
                }
                else
                {
                    btnNext.Location = new Point((dlg.ClientSize.Width - btnSize.Width) / 2, (pnlButtons.Height - btnSize.Height) / 2);
                    pnlButtons.Controls.Add(btnNext);
                }


                SetFormRoundedRegion(dlg, 20);

                dlg.Controls.AddRange(new Control[] { lblMessage, pnlButtons });

                dlg.ShowDialog(this);

                if (dlg.Tag as string == "RETRY") return AnswerPopupResult.Retry;
                if (dlg.Tag as string == "NEXT") return AnswerPopupResult.Next;
                return AnswerPopupResult.None;
            }
        }

        public enum FinalResultAction 
        {
            Close, Restart, None
        }

        protected void ShowFinalResultDialog(int score, int totalQuestions)
        {
            using (Form dlg = new Form())
            {
                dlg.FormBorderStyle = FormBorderStyle.None;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.Size = new Size(520, 320);
                dlg.BackColor = Color.White;
                dlg.Font = new Font("Lexend", 11F);
                dlg.ControlBox = false;

                Panel pnlHeader = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 60,
                    BackColor = Color.FromArgb(29, 209, 161),
                    Padding = new Padding(10)
                };

                Label lblTitle = new Label
                {
                    Text = "HOÀN THÀNH!",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Lexend", 22F, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent
                };
                pnlHeader.Controls.Add(lblTitle);

                Label lblIcon = new Label
                {
                    Text = "🎊",
                    Size = new Size(dlg.ClientSize.Width, 70),
                    Location = new Point(0, pnlHeader.Bottom + 5),
                    Font = new Font("Segoe UI Emoji", 44F),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent
                };

                // --- Kết quả Điểm (lblScoreResult) ---
                Label lblScoreResult = new Label
                {
                    Text = $"Bạn đã trả lời đúng {score}/{totalQuestions} câu",
                    Location = new Point(0, lblIcon.Bottom + 10),
                    Size = new Size(dlg.ClientSize.Width, 40),
                    Font = new Font("Lexend", 13F, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(64, 64, 64),
                    BackColor = Color.Transparent
                };

                // --- Nhận xét (lblComment) ---
                double percentage = (totalQuestions > 0) ? (score * 100.0) / totalQuestions : 0;
                string comment = percentage >= 80 ? "Xuất sắc! 🌟" :
                                 percentage >= 60 ? "Tốt lắm! 👍" :
                                 percentage >= 40 ? "Cần cố gắng thêm! 💪" :
                                 "Hãy cố gắng hơn nhé! 📚";

                Label lblComment = new Label
                {
                    Text = comment,
                    Location = new Point(0, lblScoreResult.Bottom + 5),
                    Size = new Size(dlg.ClientSize.Width, 40),
                    Font = new Font("Lexend", 12F, FontStyle.Italic),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(255, 152, 0),
                    BackColor = Color.Transparent
                };

                // --- Nút Đóng (btnClose) ---
                RoundedButton btnClose = new RoundedButton
                {
                    Text = "Đóng",
                    Size = new Size(180, 44),
                    BackColor = Color.FromArgb(87, 187, 247),
                    ForeColor = Color.White,
                    CornerRadius = 10,
                    Font = new Font("Lexend", 12F, FontStyle.Bold),
                    Location = new Point((dlg.ClientSize.Width - 180) / 2, dlg.ClientSize.Height - 60)
                };
                btnClose.FlatAppearance.BorderSize = 0;
                btnClose.Click += (s, e) => dlg.Close();

                dlg.Controls.Add(pnlHeader);
                dlg.Controls.Add(lblIcon);
                dlg.Controls.Add(lblScoreResult);
                dlg.Controls.Add(lblComment);
                dlg.Controls.Add(btnClose);

                dlg.ShowDialog(this);

                try { if (!this.IsDisposed) this.Close(); } catch { }
            }
        }
    }
}
