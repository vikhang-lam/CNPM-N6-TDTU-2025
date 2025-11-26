using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;

namespace N6
{
    public class StudentItem
    {
        public string HoTen { get; set; }
        public string MaHS { get; set; }

        public override string ToString()
        {
            return HoTen;
        }
    }

    public partial class UC_QuanLyLopHocSinh : UserControl
    {
        private DataTable allLopHoc;
        private DataTable allGiaoVien;
        private bool isProgrammaticChange = false;

        private CheckedListBox clbHocSinhChuyen;
        private ComboBox cboLopMoi_Inline;
        private Button btnXacNhanChuyen;
        private Button btnHuyChuyen;

        private EventHandler btnXacNhanChuyenClickHandler;
        private EventHandler btnHuyChuyenClickHandler;
        private PaintEventHandler pnlFilterPaintHandler;
        private PaintEventHandler pnlClassInfoCardPaintHandler;
        private PaintEventHandler pnlAssignGvcnPaintHandler;
        private DataGridViewEditingControlShowingEventHandler dgvEditingControlShowingHandler;
        private DataGridViewCellEventHandler dgvCellValueChangedHandler;
        private DataGridViewDataErrorEventHandler dgvDataErrorHandler;

        // Regex validation patterns
        private readonly Regex regexName = new Regex(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ\s]+$");
        private readonly Regex regexPhone = new Regex(@"^0\d{9}$");

        public UC_QuanLyLopHocSinh()
        {
            InitializeComponent();
            AttachEventHandlers();
            InitializeChuyenLopPanel();
            LoadInitialData();
            StyleControls();
        }

        private void AttachEventHandlers()
        {
            this.cboKhoi.SelectedIndexChanged += new System.EventHandler(this.cboKhoi_SelectedIndexChanged);
            this.dgvLopHoc.SelectionChanged += new System.EventHandler(this.dgvLopHoc_SelectionChanged);
            this.btnLuuHS.Click += new System.EventHandler(this.btnLuuHS_Click);
            this.btnThemHS.Click += new System.EventHandler(this.btnThemHS_Click);
            this.btnXoaHS.Click += new System.EventHandler(this.btnXoaHS_Click);
            this.btnImportHS.Click += new System.EventHandler(this.btnImportHS_Click);
            this.btnAssignGvcn.Click += new System.EventHandler(this.btnAssignGvcn_Click);
            this.btnImportPhanCong.Click += new System.EventHandler(this.btnImportPhanCong_Click);
            this.btnChuyenLop.Click += new System.EventHandler(this.btnChuyenLop_Click);
            this.btnThemLop.Click += new System.EventHandler(this.btnThemLop_Click);
            this.btnXoaLop.Click += new System.EventHandler(this.btnXoaLop_Click);
            this.dgvHocSinh.DataError += new DataGridViewDataErrorEventHandler(this.dgvHocSinh_DataError);
        }

        private void dgvHocSinh_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            string colName = dgvHocSinh.Columns[e.ColumnIndex].Name;

            if (colName == "NgaySinh")
            {
                MessageBox.Show("❌ Ngày sinh không hợp lệ!\nVui lòng nhập đúng định dạng (dd/MM/yyyy) và không chứa chữ cái.",
                                "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (colName == "STT")
            {
                MessageBox.Show("❌ STT phải là số nguyên.",
                                "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("❌ Dữ liệu nhập vào không đúng định dạng.",
                                "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializeChuyenLopPanel()
        {
            if (this.pnlChuyenLop == null)
            {
                Debug.WriteLine("Lỗi: pnlChuyenLop chưa được khởi tạo trong Designer.");
                return;
            }

            var lblTitle = new Label
            {
                Text = "Chuyển Lớp Hàng Loạt",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Height = 30,
                ForeColor = Color.FromArgb(0, 123, 255)
            };

            var lblChonHS = new Label
            {
                Text = "1. Chọn học sinh cần chuyển:",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 9F),
                Height = 20
            };

            clbHocSinhChuyen = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblChonLop = new Label
            {
                Text = "2. Chọn lớp chuyển đến:",
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 9F),
                Height = 20
            };

            cboLopMoi_Inline = new ComboBox
            {
                Dock = DockStyle.Bottom,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                Height = 28
            };

            var pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(0, 5, 0, 0)
            };

            btnXacNhanChuyen = new Button
            {
                Text = "Xác nhận",
                Dock = DockStyle.Right,
                Width = 100,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnHuyChuyen = new Button
            {
                Text = "Hủy",
                Dock = DockStyle.Right,
                Width = 80,
                FlatStyle = FlatStyle.Flat
            };

            pnlButtons.Controls.Add(btnXacNhanChuyen);
            pnlButtons.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 10 });
            pnlButtons.Controls.Add(btnHuyChuyen);

            btnXacNhanChuyenClickHandler = new EventHandler(this.btnXacNhanChuyen_Click);
            btnHuyChuyenClickHandler = new EventHandler(this.btnHuyChuyen_Click);
            btnXacNhanChuyen.Click += btnXacNhanChuyenClickHandler;
            btnHuyChuyen.Click += btnHuyChuyenClickHandler;

            this.pnlChuyenLop.Controls.Add(clbHocSinhChuyen);
            this.pnlChuyenLop.Controls.Add(lblChonHS);
            this.pnlChuyenLop.Controls.Add(lblTitle);
            this.pnlChuyenLop.Controls.Add(pnlButtons);
            this.pnlChuyenLop.Controls.Add(lblChonLop);
            this.pnlChuyenLop.Controls.Add(cboLopMoi_Inline);
        }

        #region SETUP GIAO DIỆN & DỮ LIỆU

        private void LoadInitialData()
        {
            try
            {
                ReloadClassData();
                allGiaoVien = DatabaseHelper.GetAllTeachers();

                var khoiList = allLopHoc.AsEnumerable()
                                        .Select(row => row.Field<string>("Khoi"))
                                        .Distinct()
                                        .OrderBy(k => k)
                                        .ToList();
                khoiList.Insert(0, "Tất cả các khối");
                cboKhoi.DataSource = khoiList;

                LoadUnassignedGvcn();

                StyleDataGridView(dgvHocSinh);
                StyleDataGridView(dgvPhanCong);
                StyleClassListGrid();

                dgvDataErrorHandler = new DataGridViewDataErrorEventHandler(dgvPhanCong_DataError);
                dgvPhanCong.DataError += dgvDataErrorHandler;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu ban đầu: " + ex.Message);
            }
        }

        private void LoadUnassignedGvcn()
        {
            cboGvcn.DataSource = DatabaseHelper.GetUnassignedHomeroomTeachers();
            cboGvcn.DisplayMember = "Ten";
            cboGvcn.ValueMember = "MaGV";
            cboGvcn.SelectedIndex = -1;
            cboGvcn.Text = "Chọn giáo viên...";
        }

        private void StyleControls()
        {
            pnlFilterPaintHandler = (s, e) => DrawShadow(s, e, 12, Color.White);
            pnlClassInfoCardPaintHandler = (s, e) => DrawShadow(s, e, 12, Color.White);
            pnlAssignGvcnPaintHandler = (s, e) => DrawShadow(s, e, 8, Color.FromArgb(248, 249, 250), true);

            pnlFilter.Paint += pnlFilterPaintHandler;
            pnlClassInfoCard.Paint += pnlClassInfoCardPaintHandler;
            pnlAssignGvcn.Paint += pnlAssignGvcnPaintHandler;

            Button[] buttons = { btnThemHS, btnLuuHS, btnAssignGvcn, btnImportHS, btnImportPhanCong, btnThemLop };
            foreach (var btn in buttons)
            {
                if (btn == null) continue;
                btn.BackColor = Color.FromArgb(0, 123, 255);
                btn.ForeColor = Color.White;
                btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
            }

            btnXoaHS.BackColor = Color.FromArgb(220, 53, 69);
            btnXoaHS.ForeColor = Color.White;
            btnXoaHS.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnXoaHS.FlatStyle = FlatStyle.Flat;
            btnXoaHS.FlatAppearance.BorderSize = 0;

            btnXoaLop.BackColor = Color.FromArgb(220, 53, 69);
            btnXoaLop.ForeColor = Color.White;
            btnXoaLop.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnXoaLop.FlatStyle = FlatStyle.Flat;
            btnXoaLop.FlatAppearance.BorderSize = 0;

            btnChuyenLop.BackColor = Color.FromArgb(253, 126, 20);
            btnChuyenLop.ForeColor = Color.White;
            btnChuyenLop.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnChuyenLop.FlatStyle = FlatStyle.Flat;
            btnChuyenLop.FlatAppearance.BorderSize = 0;
            btnChuyenLop.Text = "Chuyển Lớp";

            btnAssignGvcn.BackColor = Color.FromArgb(40, 167, 69);
        }

        private void StyleClassListGrid()
        {
            isProgrammaticChange = true;
            dgvLopHoc.DataSource = allLopHoc.DefaultView;
            if (dgvLopHoc.Columns["TenLop"] != null)
                dgvLopHoc.Columns["TenLop"].HeaderText = "DANH SÁCH LỚP HỌC";
            if (dgvLopHoc.Columns["MaLop"] != null)
                dgvLopHoc.Columns["MaLop"].Visible = false;
            if (dgvLopHoc.Columns["Khoi"] != null)
                dgvLopHoc.Columns["Khoi"].Visible = false;
            StyleDataGridView(dgvLopHoc);
            isProgrammaticChange = false;
        }

        #endregion

        #region XỬ LÝ SỰ KIỆN CHUNG

        private void cboKhoi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboKhoi.SelectedItem == null) return;
            string selectedKhoi = cboKhoi.SelectedItem.ToString();
            DataView dv = allLopHoc.DefaultView;
            dv.RowFilter = (selectedKhoi == "Tất cả các khối") ? string.Empty : $"Khoi = '{selectedKhoi}'";
            ClearAllDetails();
        }

        private void dgvLopHoc_SelectionChanged(object sender, EventArgs e)
        {
            if (isProgrammaticChange || dgvLopHoc.CurrentRow == null) return;

            if (pnlChuyenLop.Visible)
            {
                pnlChuyenLop.Visible = false;
            }

            string selectedMaLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
            LoadClassDetails(selectedMaLop);
        }

        #endregion

        #region QUẢN LÝ LỚP HỌC: THÊM, XÓA
        private void btnThemLop_Click(object sender, EventArgs e)
        {
            try
            {
                using (var formThemLop = new Form
                {
                    Text = "Thêm Lớp Học Mới",
                    Size = new Size(450, 420),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.White,
                    Padding = new Padding(20)
                })
                {
                    var lblHeader = new Label
                    {
                        Text = "📚 Thêm Lớp Học Mới",
                        Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(0, 123, 255),
                        Dock = DockStyle.Top,
                        Height = 50,
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    var pnlMain = new Panel
                    {
                        Dock = DockStyle.Fill,
                        BackColor = Color.White,
                        Padding = new Padding(0, 10, 0, 0)
                    };

                    var pnlFields = new TableLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        ColumnCount = 2,
                        RowCount = 4,
                        AutoSize = true,
                        Padding = new Padding(0, 10, 0, 10),
                        BackColor = Color.Transparent
                    };

                    pnlFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
                    pnlFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

                    // Giữ chiều cao dòng 50px cho gọn
                    for (int i = 0; i < 4; i++) pnlFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

                    Padding inputMargin = new Padding(0, 12, 0, 0);

                    // --- CÁC CONTROL NHẬP LIỆU ---

                    var lblMaLop = new Label { Text = "Mã Lớp *", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(73, 80, 87), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                    var txtMaLop = new TextBox { Font = new Font("Segoe UI", 10F), Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = inputMargin, BackColor = Color.FromArgb(248, 249, 250), BorderStyle = BorderStyle.FixedSingle };

                    var lblTenLop = new Label { Text = "Tên Lớp", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(73, 80, 87), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                    var txtTenLop = new TextBox { Font = new Font("Segoe UI", 10F), Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = inputMargin, BackColor = Color.FromArgb(233, 236, 239), BorderStyle = BorderStyle.FixedSingle, ReadOnly = true, ForeColor = Color.FromArgb(108, 117, 125) };

                    var lblKhoi = new Label { Text = "Khối *", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(73, 80, 87), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                    var cboKhoiThem = new ComboBox { Font = new Font("Segoe UI", 10F), Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = inputMargin, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(248, 249, 250), FlatStyle = FlatStyle.Flat };

                    // [SỬA ĐỔI] Thay TextBox Năm Học bằng ComboBox để lấy từ DB
                    var lblNamHoc = new Label { Text = "Năm Học *", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(73, 80, 87), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                    var cboNamHoc = new ComboBox { Font = new Font("Segoe UI", 10F), Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = inputMargin, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(248, 249, 250), FlatStyle = FlatStyle.Flat };

                    // --- LOGIC LOAD DỮ LIỆU ---

                    // 1. Load danh sách Khối (Fix cứng 5 khối)
                    cboKhoiThem.Items.Clear();
                    cboKhoiThem.Items.AddRange(new string[] { "Khối 1", "Khối 2", "Khối 3", "Khối 4", "Khối 5" });
                    cboKhoiThem.SelectedIndex = 0;

                    // 2. Load danh sách Năm Học từ Database và chọn năm hiện tại
                    try
                    {
                        DataTable dtNamHoc = DatabaseHelper.GetAllSchoolYears(); // Gọi hàm lấy toàn bộ năm học
                        if (dtNamHoc != null && dtNamHoc.Rows.Count > 0)
                        {
                            cboNamHoc.DataSource = dtNamHoc;
                            cboNamHoc.DisplayMember = "TenNamHoc"; // Hiển thị: "Năm học 2024 - 2025"
                            cboNamHoc.ValueMember = "MaNamHoc";    // Giá trị: "2024-2025"

                            // Tìm năm học đang kích hoạt (IsCurrent = true/1)
                            foreach (DataRow row in dtNamHoc.Rows)
                            {
                                if (row["IsCurrent"] != DBNull.Value && Convert.ToBoolean(row["IsCurrent"]) == true)
                                {
                                    cboNamHoc.SelectedValue = row["MaNamHoc"];
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // Fallback nếu DB chưa có năm học nào (hiếm khi xảy ra nếu đã chạy script data.sql)
                            cboNamHoc.Items.Add($"{DateTime.Now.Year}-{DateTime.Now.Year + 1}");
                            cboNamHoc.SelectedIndex = 0;
                        }
                    }
                    catch
                    {
                        // Fallback khi lỗi kết nối
                        cboNamHoc.Items.Add($"{DateTime.Now.Year}-{DateTime.Now.Year + 1}");
                        cboNamHoc.SelectedIndex = 0;
                    }

                    // Tự động sinh tên lớp: "Lớp" + [Khối] + [Mã] (Ví dụ: Mã 5A -> Lớp 5A)
                    // Logic: Nếu mã lớp chưa có tên khối, tự động thêm vào dựa trên selection
                    EventHandler updateTenLop = (s, ev) =>
                    {
                        string rawMa = txtMaLop.Text.Trim().ToUpper();
                        if (string.IsNullOrEmpty(rawMa))
                        {
                            txtTenLop.Text = "";
                            return;
                        }

                        // Nếu người dùng nhập "5A" -> Tên lớp: "Lớp 5A"
                        // Nếu người dùng nhập "A" và chọn Khối 5 -> Tên lớp: "Lớp 5A" (Gợi ý)
                        txtTenLop.Text = $"Lớp {rawMa}";
                    };
                    txtMaLop.TextChanged += updateTenLop;

                    // --- THÊM CONTROL VÀO FORM ---

                    pnlFields.Controls.Add(lblMaLop, 0, 0); pnlFields.Controls.Add(txtMaLop, 1, 0);
                    pnlFields.Controls.Add(lblTenLop, 0, 1); pnlFields.Controls.Add(txtTenLop, 1, 1);
                    pnlFields.Controls.Add(lblKhoi, 0, 2); pnlFields.Controls.Add(cboKhoiThem, 1, 2);
                    pnlFields.Controls.Add(lblNamHoc, 0, 3); pnlFields.Controls.Add(cboNamHoc, 1, 3); // Dùng cboNamHoc

                    var pnlButtons = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.White };
                    var btnLuu = new Button { Text = "💾 LƯU LỚP", Font = new Font("Segoe UI", 10F, FontStyle.Bold), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Height = 38, Width = 130, Dock = DockStyle.Right };
                    var btnHuy = new Button { Text = "❌ HỦY", Font = new Font("Segoe UI", 10F, FontStyle.Bold), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Height = 38, Width = 100, Dock = DockStyle.Right };

                    btnLuu.Margin = new Padding(0, 10, 0, 0);

                    // --- SỰ KIỆN LƯU ---
                    btnLuu.Click += (s, ev) =>
                    {
                        // Validation
                        if (string.IsNullOrWhiteSpace(txtMaLop.Text))
                        {
                            MessageBox.Show("Vui lòng nhập Mã Lớp!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtMaLop.Focus();
                            return;
                        }

                        if (!Regex.IsMatch(txtMaLop.Text, @"^[a-zA-Z0-9]+$"))
                        {
                            MessageBox.Show("❌ Mã lớp chỉ được chứa Chữ cái và Số (Ví dụ: 5A, 1B)!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtMaLop.Focus();
                            return;
                        }

                        if (cboNamHoc.SelectedItem == null)
                        {
                            MessageBox.Show("Vui lòng chọn Năm Học!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        try
                        {
                            // Lấy giá trị thực: MaNamHoc (VD: "2024-2025") từ ComboBox
                            string selectedNamHoc = cboNamHoc.SelectedValue != null
                                                    ? cboNamHoc.SelectedValue.ToString()
                                                    : cboNamHoc.Text;

                            DatabaseHelper.InsertClass(
                                txtMaLop.Text.Trim().ToUpper(),
                                txtTenLop.Text.Trim(),
                                cboKhoiThem.SelectedItem.ToString(),
                                selectedNamHoc // Truyền mã năm học chuẩn
                            );

                            MessageBox.Show("✅ Thêm lớp học thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            formThemLop.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"❌ Lỗi khi thêm lớp: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };

                    btnHuy.Click += (s, ev) => formThemLop.Close();

                    Panel spacer = new Panel { Dock = DockStyle.Right, Width = 10 };
                    pnlButtons.Controls.Add(btnLuu);
                    pnlButtons.Controls.Add(spacer);
                    pnlButtons.Controls.Add(btnHuy);

                    pnlMain.Controls.Add(pnlFields);
                    pnlMain.Controls.Add(pnlButtons);
                    formThemLop.Controls.Add(pnlMain);
                    formThemLop.Controls.Add(lblHeader);
                    formThemLop.AcceptButton = btnLuu;
                    formThemLop.CancelButton = btnHuy;

                    if (formThemLop.ShowDialog() == DialogResult.OK)
                    {
                        ReloadClassData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoaLop_Click(object sender, EventArgs e)
        {
            if (dgvLopHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một lớp để xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
            string tenLop = dgvLopHoc.CurrentRow.Cells["TenLop"].Value.ToString();

            try
            {
                if (DatabaseHelper.ClassHasStudents(maLop))
                {
                    MessageBox.Show($"Không thể xóa lớp '{tenLop}' vì lớp đang có học sinh!\n" +
                        "Vui lòng chuyển hết học sinh sang lớp khác trước.",
                        "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa lớp '{tenLop}'?\n\nThao tác này không thể hoàn tác!",
                    "Xác nhận xóa lớp",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirm == DialogResult.Yes)
                {
                    DatabaseHelper.DeleteClass(maLop);

                    MessageBox.Show("Xóa lớp học thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ReloadClassData();
                    ClearAllDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa lớp: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReloadClassData()
        {
            try
            {
                allLopHoc = DatabaseHelper.GetAllClasses();
                string selectedKhoi = (cboKhoi.SelectedItem ?? "Tất cả các khối").ToString();

                isProgrammaticChange = true;
                dgvLopHoc.DataSource = allLopHoc.DefaultView;
                StyleClassListGrid();
                isProgrammaticChange = false;

                allLopHoc.DefaultView.RowFilter = (selectedKhoi == "Tất cả các khối") ?
                    string.Empty : $"Khoi = '{selectedKhoi}'";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải lại dữ liệu lớp: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region HỌC SINH: THÊM, SỬA, XÓA, IMPORT, CHUYỂN LỚP

        private void btnThemHS_Click(object sender, EventArgs e)
        {
            if (dgvLopHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một lớp để thêm học sinh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var formThemHS = new Form
                {
                    Text = "Thêm Học Sinh Mới",
                    Size = new Size(600, 650),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.White,
                    Padding = new Padding(20)
                })
                {
                    var lblHeader = new Label
                    {
                        Text = "👤 Thêm Học Sinh Mới",
                        Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(0, 123, 255),
                        Dock = DockStyle.Top,
                        Height = 50,
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    var pnlMain = new Panel
                    {
                        Dock = DockStyle.Fill,
                        BackColor = Color.White,
                        Padding = new Padding(0, 10, 0, 0)
                    };

                    var pnlFields = new TableLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        ColumnCount = 2,
                        RowCount = 7,
                        AutoSize = true,
                        Padding = new Padding(0, 10, 0, 20),
                        BackColor = Color.Transparent
                    };

                    pnlFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
                    pnlFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
                    for (int i = 0; i < 7; i++) pnlFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));

                    // ============================================
                    // [FIX LỖI ALIGNMENT] 
                    // Thay vì Dock.Fill (khiến control dính lên trên), ta dùng Anchor và Margin-Top để đẩy xuống giữa dòng
                    // Dòng cao 60px, Control cao ~26px => Margin Top khoảng 16px sẽ cân đối.
                    // ============================================
                    Padding inputMargin = new Padding(0, 16, 0, 0);

                    var lblMaHS = new Label { Text = "Mã HS", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(73, 80, 87), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                    var txtMaHS = new TextBox { Font = new Font("Segoe UI", 10F), Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = inputMargin, BackColor = Color.FromArgb(233, 236, 239), BorderStyle = BorderStyle.FixedSingle, ReadOnly = true };

                    var lblHoTen = new Label { Text = "Họ và Tên *", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(73, 80, 87), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                    var txtHoTen = new TextBox { Font = new Font("Segoe UI", 10F), Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = inputMargin, BackColor = Color.FromArgb(248, 249, 250), BorderStyle = BorderStyle.FixedSingle };

                    var lblGioiTinh = new Label { Text = "Giới Tính *", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(73, 80, 87), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                    var cboGioiTinh = new ComboBox { Font = new Font("Segoe UI", 10F), Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = inputMargin, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(248, 249, 250), FlatStyle = FlatStyle.Flat };
                    cboGioiTinh.Items.AddRange(new string[] { "Nam", "Nữ" });
                    cboGioiTinh.SelectedIndex = 0;

                    var lblNgaySinh = new Label { Text = "Ngày Sinh *", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(73, 80, 87), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                    var dtpNgaySinh = new DateTimePicker { Font = new Font("Segoe UI", 10F), Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = inputMargin, Format = DateTimePickerFormat.Short, Value = DateTime.Now.AddYears(-6) };

                    var lblDiaChi = new Label { Text = "Địa Chỉ", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(73, 80, 87), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                    var txtDiaChi = new TextBox { Font = new Font("Segoe UI", 10F), Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = inputMargin, BackColor = Color.FromArgb(248, 249, 250), BorderStyle = BorderStyle.FixedSingle };

                    var lblDanToc = new Label { Text = "Dân Tộc", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(73, 80, 87), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                    var txtDanToc = new TextBox { Font = new Font("Segoe UI", 10F), Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = inputMargin, BackColor = Color.FromArgb(248, 249, 250), BorderStyle = BorderStyle.FixedSingle, Text = "Kinh" };

                    var lblSDT = new Label { Text = "SĐT Phụ Huynh", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(73, 80, 87), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                    var txtSDT = new TextBox { Font = new Font("Segoe UI", 10F), Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = inputMargin, BackColor = Color.FromArgb(248, 249, 250), BorderStyle = BorderStyle.FixedSingle };

                    pnlFields.Controls.Add(lblMaHS, 0, 0); pnlFields.Controls.Add(txtMaHS, 1, 0);
                    pnlFields.Controls.Add(lblHoTen, 0, 1); pnlFields.Controls.Add(txtHoTen, 1, 1);
                    pnlFields.Controls.Add(lblGioiTinh, 0, 2); pnlFields.Controls.Add(cboGioiTinh, 1, 2);
                    pnlFields.Controls.Add(lblNgaySinh, 0, 3); pnlFields.Controls.Add(dtpNgaySinh, 1, 3);
                    pnlFields.Controls.Add(lblDiaChi, 0, 4); pnlFields.Controls.Add(txtDiaChi, 1, 4);
                    pnlFields.Controls.Add(lblDanToc, 0, 5); pnlFields.Controls.Add(txtDanToc, 1, 5);
                    pnlFields.Controls.Add(lblSDT, 0, 6); pnlFields.Controls.Add(txtSDT, 1, 6);

                    var pnlButtons = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.White };
                    var btnLuu = new Button { Text = "💾 LƯU HỌC SINH", Font = new Font("Segoe UI", 10F, FontStyle.Bold), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Height = 45, Width = 150, Dock = DockStyle.Right };
                    var btnHuy = new Button { Text = "❌ HỦY BỎ", Font = new Font("Segoe UI", 10F, FontStyle.Bold), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Height = 45, Width = 120, Dock = DockStyle.Right };

                    string GenerateStudentCode()
                    {
                        try
                        {
                            DataTable allStudents = DatabaseHelper.GetAllStudents();
                            if (allStudents.Rows.Count == 0) return "HS001";
                            var lastMaHS = allStudents.AsEnumerable()
                                .Select(row => row.Field<string>("MaHS"))
                                .Where(ma => !string.IsNullOrEmpty(ma) && ma.StartsWith("HS"))
                                .OrderByDescending(ma => ma).FirstOrDefault();
                            if (string.IsNullOrEmpty(lastMaHS)) return "HS001";
                            string numberPart = lastMaHS.Substring(2);
                            if (int.TryParse(numberPart, out int lastNumber)) return $"HS{(lastNumber + 1).ToString("D3")}";
                            return "HS001";
                        }
                        catch { return "HS001"; }
                    }

                    formThemHS.Load += (s, ev) => { txtMaHS.Text = GenerateStudentCode(); };

                    btnLuu.Click += (s, ev) =>
                    {
                        // 1. Check Name
                        if (string.IsNullOrWhiteSpace(txtHoTen.Text))
                        {
                            MessageBox.Show("Vui lòng nhập Họ và Tên!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtHoTen.Focus();
                            return;
                        }

                        if (!regexName.IsMatch(txtHoTen.Text))
                        {
                            MessageBox.Show("❌ Họ và Tên KHÔNG được chứa số hoặc ký tự đặc biệt!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtHoTen.Focus();
                            txtHoTen.SelectAll();
                            return;
                        }

                        // 2. Check Gender
                        if (cboGioiTinh.SelectedItem == null)
                        {
                            MessageBox.Show("Vui lòng chọn Giới Tính!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        string gt = cboGioiTinh.SelectedItem.ToString();
                        if (gt != "Nam" && gt != "Nữ")
                        {
                            MessageBox.Show("❌ Giới tính không hợp lệ (Phải là Nam hoặc Nữ)!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // 3. Check Date of Birth
                        if (dtpNgaySinh.Value.Date > DateTime.Now.Date)
                        {
                            MessageBox.Show("❌ Ngày sinh không hợp lệ (Tương lai)!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dtpNgaySinh.Focus();
                            return;
                        }

                        int age = DateTime.Now.Year - dtpNgaySinh.Value.Year;
                        if (dtpNgaySinh.Value.Date > DateTime.Now.AddYears(-age)) age--;

                        if (age < 6)
                        {
                            MessageBox.Show($"❌ Học sinh chưa đủ tuổi đi học (Hiện tại: {age} tuổi). Phải từ 6 tuổi trở lên!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dtpNgaySinh.Focus();
                            return;
                        }

                        // 4. Check Phone
                        string phone = txtSDT.Text.Trim();
                        if (!string.IsNullOrEmpty(phone))
                        {
                            if (!regexPhone.IsMatch(phone))
                            {
                                MessageBox.Show("❌ Số điện thoại không hợp lệ!\nPhải bắt đầu bằng số 0 và có đúng 10 chữ số.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtSDT.Focus();
                                txtSDT.SelectAll();
                                return;
                            }
                        }

                        try
                        {
                            string maLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();

                            DatabaseHelper.InsertStudent(
                                txtMaHS.Text.Trim(),
                                maLop,
                                txtHoTen.Text.Trim(),
                                dtpNgaySinh.Value,
                                cboGioiTinh.SelectedItem.ToString(),
                                txtSDT.Text.Trim(),
                                txtDiaChi.Text.Trim(),
                                txtDanToc.Text.Trim()
                            );

                            MessageBox.Show("✅ Thêm học sinh thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            formThemHS.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"❌ Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };

                    btnHuy.Click += (s, ev) => formThemHS.Close();

                    pnlButtons.Controls.Add(btnLuu); pnlButtons.Controls.Add(btnHuy);
                    pnlMain.Controls.Add(pnlFields); pnlMain.Controls.Add(pnlButtons);
                    formThemHS.Controls.Add(pnlMain); formThemHS.Controls.Add(lblHeader);
                    formThemHS.AcceptButton = btnLuu; formThemHS.CancelButton = btnHuy;

                    if (formThemHS.ShowDialog() == DialogResult.OK)
                    {
                        string selectedMaLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
                        LoadClassDetails(selectedMaLop);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoaHS_Click(object sender, EventArgs e)
        {
            if (dgvHocSinh.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một học sinh để xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maHS = dgvHocSinh.CurrentRow.Cells["MaHS"].Value.ToString();
            string tenHS = dgvHocSinh.CurrentRow.Cells["HoTen"].Value.ToString();

            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa học sinh '{tenHS}' không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteStudent(maHS);
                    MessageBox.Show("Xóa học sinh thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    string selectedMaLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
                    LoadClassDetails(selectedMaLop);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa học sinh: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLuuHS_Click(object sender, EventArgs e)
        {
            DataTable dt = (dgvHocSinh.DataSource as DataTable);
            if (dt == null) return;

            DataTable changes = dt.GetChanges(DataRowState.Modified);

            if (changes == null || changes.Rows.Count == 0)
            {
                MessageBox.Show("Không có thay đổi nào để lưu.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int successCount = 0;
            try
            {
                foreach (DataRow row in changes.Rows)
                {
                    string maHS = row["MaHS"].ToString();
                    string hoTen = row["HoTen"].ToString().Trim();
                    string sdtPH = row["SDTPhuHuynh"].ToString().Trim();
                    string gioiTinh = row["GioiTinh"].ToString().Trim();

                    // 1. Validation Name
                    if (string.IsNullOrWhiteSpace(hoTen))
                    {
                        MessageBox.Show($"❌ Học sinh mã {maHS}: Tên không được để trống!",
                            "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dt.RejectChanges();
                        return;
                    }

                    if (!regexName.IsMatch(hoTen))
                    {
                        MessageBox.Show($"❌ Học sinh {hoTen} ({maHS}): Tên không được chứa số hoặc ký tự đặc biệt!",
                            "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dt.RejectChanges();
                        return;
                    }

                    // 2. Validation Gender
                    gioiTinh = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(gioiTinh.ToLower());
                    if (gioiTinh != "Nam" && gioiTinh != "Nữ")
                    {
                        MessageBox.Show($"❌ Học sinh {hoTen} ({maHS}): Giới tính phải là 'Nam' hoặc 'Nữ'!",
                            "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dt.RejectChanges();
                        return;
                    }
                    row["GioiTinh"] = gioiTinh;

                    // 3. Validation Phone
                    if (!string.IsNullOrEmpty(sdtPH) && !regexPhone.IsMatch(sdtPH))
                    {
                        MessageBox.Show($"❌ Học sinh {hoTen} ({maHS}): Số điện thoại không hợp lệ! (Phải gồm 10 số và bắt đầu bằng 0)",
                            "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dt.RejectChanges();
                        return;
                    }

                    // 4. Validation Date of Birth (NGÀY SINH)
                    DateTime ngaySinh = DateTime.Now;
                    if (row["NgaySinh"] != DBNull.Value)
                    {
                        try
                        {
                            ngaySinh = Convert.ToDateTime(row["NgaySinh"]);

                            // --- [FIX] KIỂM TRA CHẶN LỖI OVERFLOW SQL ---
                            if (ngaySinh.Year < 1753 || ngaySinh.Year > 9999)
                            {
                                MessageBox.Show($"❌ Học sinh {hoTen}: Ngày sinh không hợp lệ (Năm phải từ 1753 đến 9999)!",
                                    "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dt.RejectChanges();
                                return;
                            }
                            // --------------------------------------------

                            if (ngaySinh.Date > DateTime.Now.Date)
                            {
                                MessageBox.Show($"❌ Học sinh {hoTen}: Ngày sinh không được lớn hơn ngày hiện tại!",
                                    "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                dt.RejectChanges();
                                return;
                            }

                            int age = DateTime.Now.Year - ngaySinh.Year;
                            if (ngaySinh.Date > DateTime.Now.AddYears(-age)) age--;

                            if (age < 6)
                            {
                                MessageBox.Show($"❌ Học sinh {hoTen}: Tuổi quá nhỏ ({age} tuổi). Phải từ 6 tuổi trở lên!",
                                    "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                dt.RejectChanges();
                                return;
                            }
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show($"❌ Học sinh {hoTen}: Định dạng ngày sinh không hợp lệ!",
                                    "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dt.RejectChanges();
                            return;
                        }
                    }

                    string diaChi = row["DiaChi"].ToString();
                    string danToc = row["DanToc"].ToString();

                    // --- [FIX] TRY-CATCH RIÊNG CHO LỆNH SQL ---
                    try
                    {
                        DatabaseHelper.UpdateStudent(maHS, hoTen, ngaySinh, gioiTinh, sdtPH, diaChi, danToc);
                        successCount++;
                    }
                    catch (System.Data.SqlTypes.SqlTypeException)
                    {
                        // Bắt chính xác lỗi SqlDateTime overflow ở đây
                        MessageBox.Show($"❌ Học sinh {hoTen}: Lỗi ngày sinh không hợp lệ !",
                            "Lỗi lưu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dt.RejectChanges();
                        return;
                    }
                    // ------------------------------------------
                }

                dt.AcceptChanges();
                MessageBox.Show($"✅ Đã lưu thành công {successCount} hồ sơ!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi hệ thống khi lưu: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                dt.RejectChanges();
            }
        }

        private void btnImportHS_Click(object sender, EventArgs e)
        {
            string selectedKhoi = (cboKhoi.SelectedItem ?? "Tất cả các khối").ToString();
            string maLop = dgvLopHoc.CurrentRow?.Cells["MaLop"].Value.ToString();
            string tenLop = dgvLopHoc.CurrentRow?.Cells["TenLop"].Value.ToString();

            using (var optionForm = new Form
            {
                Text = "Chọn kiểu Import",
                Size = new Size(400, 180),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            })
            {
                var btnTheoLop = new Button
                {
                    Text = $"Import cho lớp ({tenLop})",
                    Dock = DockStyle.Top,
                    Height = 40,
                    DialogResult = DialogResult.OK,
                    Font = new Font("Segoe UI", 10F)
                };

                var btnTheoKhoi = new Button
                {
                    Text = $"Import cho khối ({selectedKhoi})",
                    Dock = DockStyle.Top,
                    Height = 40,
                    DialogResult = DialogResult.Yes,
                    Font = new Font("Segoe UI", 10F)
                };

                var btnCancel = new Button
                {
                    Text = "Hủy",
                    Dock = DockStyle.Bottom,
                    Height = 40,
                    DialogResult = DialogResult.Cancel,
                    Font = new Font("Segoe UI", 10F)
                };

                btnTheoLop.Enabled = (maLop != null);
                btnTheoKhoi.Enabled = (selectedKhoi != "Tất cả các khối");
                if (btnTheoKhoi.Enabled == false)
                    btnTheoKhoi.Text = "Import cho khối (Hãy chọn 1 khối)";

                optionForm.Controls.AddRange(new Control[] { btnTheoKhoi, btnTheoLop, btnCancel });
                optionForm.CancelButton = btnCancel;

                DialogResult choice = optionForm.ShowDialog();
                if (choice == DialogResult.Cancel) return;

                frmImportExcel.ImportType type = (choice == DialogResult.OK) ?
                    frmImportExcel.ImportType.HocSinhTheoLop :
                    frmImportExcel.ImportType.HocSinhTheoKhoi;

                using (var importForm = new frmImportExcel(type, maLop, tenLop, selectedKhoi))
                {
                    if (importForm.ShowDialog() == DialogResult.OK)
                    {
                        if (maLop != null)
                        {
                            LoadClassDetails(maLop);
                        }
                    }
                }
            }
        }

        private void btnChuyenLop_Click(object sender, EventArgs e)
        {
            if (dgvLopHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một lớp trước khi thực hiện chuyển lớp.",
                    "Chưa chọn lớp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maLopHienTai = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
            DataTable dtHocSinh = dgvHocSinh.DataSource as DataTable;

            if (dtHocSinh == null || dtHocSinh.Rows.Count == 0)
            {
                MessageBox.Show("Lớp hiện tại không có học sinh nào để chuyển.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var filteredRows = allLopHoc.AsEnumerable()
                                .Where(row => row.Field<string>("MaLop") != maLopHienTai);

            if (!filteredRows.Any())
            {
                MessageBox.Show("Không có lớp nào khác để chuyển đến.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable dtLopDen = filteredRows.CopyToDataTable();

            clbHocSinhChuyen.Items.Clear();
            foreach (DataRow row in dtHocSinh.Rows)
            {
                clbHocSinhChuyen.Items.Add(new StudentItem
                {
                    HoTen = row["HoTen"].ToString(),
                    MaHS = row["MaHS"].ToString()
                });
            }

            cboLopMoi_Inline.DataSource = dtLopDen;
            cboLopMoi_Inline.DisplayMember = "TenLop";
            cboLopMoi_Inline.ValueMember = "MaLop";
            cboLopMoi_Inline.SelectedIndex = 0;

            pnlChuyenLop.Visible = true;
            pnlChuyenLop.BringToFront();
        }

        private void btnHuyChuyen_Click(object sender, EventArgs e)
        {
            pnlChuyenLop.Visible = false;
            for (int i = 0; i < clbHocSinhChuyen.Items.Count; i++)
            {
                clbHocSinhChuyen.SetItemChecked(i, false);
            }
        }

        private void btnXacNhanChuyen_Click(object sender, EventArgs e)
        {
            List<string> maHocSinhList = new List<string>();
            foreach (object itemChecked in clbHocSinhChuyen.CheckedItems)
            {
                if (itemChecked is StudentItem hs)
                {
                    maHocSinhList.Add(hs.MaHS);
                }
            }

            if (maHocSinhList.Count == 0)
            {
                MessageBox.Show("Bạn chưa chọn học sinh nào để chuyển.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboLopMoi_Inline.SelectedValue == null)
            {
                MessageBox.Show("Lỗi: Không xác định được lớp chuyển đến.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string maLopMoi = cboLopMoi_Inline.SelectedValue.ToString();
            string tenLopMoi = cboLopMoi_Inline.Text;
            string maLopHienTai = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();

            try
            {
                int soHocSinhDaChuyen = DatabaseHelper.UpdateStudentClass_Multi(maHocSinhList, maLopMoi);

                MessageBox.Show($"Đã chuyển thành công {soHocSinhDaChuyen} học sinh sang lớp '{tenLopMoi}'.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                pnlChuyenLop.Visible = false;
                LoadClassDetails(maLopHienTai);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thực hiện chuyển lớp: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region PHÂN CÔNG GIẢNG DẠY: GVCN, MÔN HỌC, IMPORT

        private void btnAssignGvcn_Click(object sender, EventArgs e)
        {
            if (dgvLopHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn lớp.", "Thông tin thiếu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboGvcn.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn giáo viên (hoặc 'Trống') để phân công.",
                    "Thông tin thiếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string maLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
                string maGV = cboGvcn.SelectedValue.ToString();

                DatabaseHelper.UpdateHomeroomTeacherForClass(maLop, maGV);

                MessageBox.Show("Cập nhật giáo viên chủ nhiệm thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                string selectedKhoi = (cboKhoi.SelectedItem ?? "Tất cả các khối").ToString();
                allLopHoc = DatabaseHelper.GetAllClasses();
                isProgrammaticChange = true;
                dgvLopHoc.DataSource = allLopHoc.DefaultView;
                isProgrammaticChange = false;
                allLopHoc.DefaultView.RowFilter = (selectedKhoi == "Tất cả các khối") ?
                    string.Empty : $"Khoi = '{selectedKhoi}'";

                LoadClassDetails(maLop);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật GVCN: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvPhanCong_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvPhanCong.CurrentCell.ColumnIndex == dgvPhanCong.Columns["AssignTeacherColumn"].Index &&
                e.Control is ComboBox comboBox)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

                string tenMon = dgvPhanCong.CurrentRow.Cells["TenMon"].Value.ToString();
                DataView dv = new DataView(allGiaoVien);

                if (string.IsNullOrEmpty(tenMon))
                {
                    dv.RowFilter = "CacMonDay = 'Chưa có môn'";
                }
                else
                {
                    string safeTenMon = tenMon.Replace("'", "''");
                    dv.RowFilter = $"CacMonDay LIKE '%{safeTenMon}%'";
                }

                DataTable filteredTeachers = dv.ToTable();

                DataRow emptyRow = filteredTeachers.NewRow();
                emptyRow["Ten"] = "(Trống)";
                emptyRow["MaGV"] = "";
                foreach (DataColumn col in filteredTeachers.Columns)
                {
                    if (emptyRow.IsNull(col) && col.DataType == typeof(string))
                    {
                        emptyRow[col] = "";
                    }
                }
                filteredTeachers.Rows.InsertAt(emptyRow, 0);

                comboBox.DataSource = filteredTeachers;
                comboBox.DisplayMember = "Ten";
                comboBox.ValueMember = "MaGV";
            }
        }

        private void dgvPhanCong_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isProgrammaticChange || e.RowIndex < 0 ||
                dgvPhanCong.Columns[e.ColumnIndex].Name != "AssignTeacherColumn") return;

            try
            {
                string maLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
                string maMon = dgvPhanCong.Rows[e.RowIndex].Cells["MaMon"].Value.ToString();
                object newMaGVObj = dgvPhanCong.Rows[e.RowIndex].Cells["AssignTeacherColumn"].Value;
                string newMaGV = (newMaGVObj == DBNull.Value || newMaGVObj == null) ? null : newMaGVObj.ToString();

                DatabaseHelper.UpdateTeachingAssignment(maLop, maMon, newMaGV);

                var gv = allGiaoVien.AsEnumerable().FirstOrDefault(r => r.Field<string>("MaGV") == newMaGV);
                isProgrammaticChange = true;
                dgvPhanCong.Rows[e.RowIndex].Cells["TenGV"].Value = gv != null ?
                    gv.Field<string>("Ten") : "(Trống)";
                isProgrammaticChange = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật phân công: " + ex.Message);
            }
        }

        private void dgvPhanCong_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void btnImportPhanCong_Click(object sender, EventArgs e)
        {
            if (dgvLopHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một lớp để import phân công.", "Chưa chọn lớp",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maLop = dgvLopHoc.CurrentRow.Cells["MaLop"].Value.ToString();
            string tenLop = dgvLopHoc.CurrentRow.Cells["TenLop"].Value.ToString();

            using (var importForm = new frmImportExcel(frmImportExcel.ImportType.PhanCong, maLop, tenLop, null))
            {
                if (importForm.ShowDialog() == DialogResult.OK)
                {
                    dgvPhanCong.DataSource = DatabaseHelper.GetTeachingAssignmentsByClass(maLop);
                    CustomizeAssignmentGrid();
                }
            }
        }

        #endregion

        #region HÀM TẢI DỮ LIỆU & TÙY CHỈNH GIAO DIỆN (Helper)

        private void LoadClassDetails(string maLop)
        {
            isProgrammaticChange = true;
            try
            {
                DataRow classInfo = DatabaseHelper.GetClassDetails(maLop);
                if (classInfo != null)
                {
                    lblTenLop.Text = classInfo["TenLop"].ToString();
                    lblNamHoc.Text = $"🗓️ Năm học: {classInfo["NamHoc"]}";
                    lblGVCN.Text = $"👤 GVCN: {classInfo["TenGVCN"]}";
                    lblSiSo.Text = $"👥 Sĩ số: {classInfo["SiSo"]}";

                    pnlAssignGvcn.Visible = true;

                    DataTable dtAvailableTeachers = DatabaseHelper.GetUnassignedHomeroomTeachers();

                    string currentMaGVCN = classInfo["MaGVCN"]?.ToString() ?? "";
                    string currentTenGVCN = classInfo["TenGVCN"]?.ToString() ?? "Chưa có";

                    DataRow emptyRow = dtAvailableTeachers.NewRow();
                    emptyRow["Ten"] = "(Trống)";
                    emptyRow["MaGV"] = "";
                    dtAvailableTeachers.Rows.InsertAt(emptyRow, 0);

                    if (!string.IsNullOrEmpty(currentMaGVCN))
                    {
                        bool exists = dtAvailableTeachers.AsEnumerable()
                                        .Any(r => r.Field<string>("MaGV") == currentMaGVCN);
                        if (!exists)
                        {
                            DataRow currentRow = dtAvailableTeachers.NewRow();
                            currentRow["Ten"] = currentTenGVCN;
                            currentRow["MaGV"] = currentMaGVCN;
                            dtAvailableTeachers.Rows.Add(currentRow);
                        }
                    }

                    cboGvcn.DataSource = dtAvailableTeachers;
                    cboGvcn.DisplayMember = "Ten";
                    cboGvcn.ValueMember = "MaGV";
                    cboGvcn.SelectedValue = currentMaGVCN;

                    lblGVCN.ForeColor = (currentTenGVCN == "Chưa có") ?
                        Color.FromArgb(220, 53, 69) : Color.FromArgb(108, 117, 125);
                }

                dgvHocSinh.DataSource = DatabaseHelper.GetStudentsByClass(maLop);
                CustomizeStudentGrid();
                dgvPhanCong.DataSource = DatabaseHelper.GetTeachingAssignmentsByClass(maLop);
                CustomizeAssignmentGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải chi tiết lớp: " + ex.Message);
            }
            finally
            {
                isProgrammaticChange = false;
            }
        }

        private void ClearAllDetails()
        {
            lblTenLop.Text = "Chọn lớp để xem thông tin";
            lblNamHoc.Text = "🗓️ Năm học: -";
            lblGVCN.Text = "👤 GVCN: -";
            lblGVCN.ForeColor = Color.FromArgb(108, 117, 125);
            lblSiSo.Text = "👥 Sĩ số: -";
            dgvHocSinh.DataSource = null;
            dgvPhanCong.DataSource = null;
            pnlAssignGvcn.Visible = false;

            if (pnlChuyenLop != null)
            {
                pnlChuyenLop.Visible = false;
            }
        }

        private void CustomizeStudentGrid()
        {
            if (dgvHocSinh.DataSource == null || dgvHocSinh.Columns.Count == 0) return;

            dgvHocSinh.AllowUserToAddRows = false;
            dgvHocSinh.ReadOnly = false;

            if (dgvHocSinh.Columns.Contains("STT"))
            {
                var col = dgvHocSinh.Columns["STT"];
                col.HeaderText = "STT";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                col.DisplayIndex = 0;
                col.ReadOnly = true;
            }

            dgvHocSinh.Columns["MaHS"].HeaderText = "Mã Học Sinh";
            dgvHocSinh.Columns["MaHS"].ReadOnly = true;

            dgvHocSinh.Columns["HoTen"].HeaderText = "Họ và Tên";
            if (dgvHocSinh.Columns.Contains("STT"))
                dgvHocSinh.Columns["HoTen"].DisplayIndex = 1;

            dgvHocSinh.Columns["GioiTinh"].HeaderText = "Giới Tính";
            dgvHocSinh.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
            dgvHocSinh.Columns["DiaChi"].HeaderText = "Địa Chỉ";
            dgvHocSinh.Columns["DanToc"].HeaderText = "Dân Tộc";
            dgvHocSinh.Columns["SDTPhuHuynh"].HeaderText = "SĐT Phụ Huynh";

            dgvHocSinh.Columns["MaHS"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvHocSinh.Columns["HoTen"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvHocSinh.Columns["GioiTinh"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvHocSinh.Columns["NgaySinh"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvHocSinh.Columns["DanToc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvHocSinh.Columns["SDTPhuHuynh"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvHocSinh.Columns["DiaChi"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void CustomizeAssignmentGrid()
        {
            if (dgvPhanCong.DataSource == null || dgvPhanCong.Columns.Count == 0) return;

            if (allGiaoVien == null)
            {
                return;
            }

            if (dgvPhanCong.Columns.Contains("AssignTeacherColumn"))
                dgvPhanCong.Columns.Remove("AssignTeacherColumn");

            dgvPhanCong.Columns["MaMon"].Visible = false;
            dgvPhanCong.Columns["MaGV"].Visible = false;
            dgvPhanCong.Columns["TenMon"].HeaderText = "Môn Học";
            dgvPhanCong.Columns["TenMon"].ReadOnly = true;
            dgvPhanCong.Columns["TenGV"].HeaderText = "Giáo Viên Hiện Tại";
            dgvPhanCong.Columns["TenGV"].ReadOnly = true;

            DataTable dtColumnSource = allGiaoVien.Copy();
            DataRow emptyRowCol = dtColumnSource.NewRow();
            emptyRowCol["Ten"] = "(Trống)";
            emptyRowCol["MaGV"] = "";
            foreach (DataColumn col in dtColumnSource.Columns)
            {
                if (emptyRowCol.IsNull(col) && col.DataType == typeof(string))
                {
                    emptyRowCol[col] = "";
                }
            }
            dtColumnSource.Rows.InsertAt(emptyRowCol, 0);

            var comboBoxColumn = new DataGridViewComboBoxColumn
            {
                Name = "AssignTeacherColumn",
                HeaderText = "Phân Công Mới",
                DataSource = dtColumnSource,
                DisplayMember = "Ten",
                ValueMember = "MaGV",
                FlatStyle = FlatStyle.Flat
            };

            dgvPhanCong.Columns.Add(comboBoxColumn);

            foreach (DataGridViewRow row in dgvPhanCong.Rows)
            {
                row.Cells["AssignTeacherColumn"].Value = row.Cells["MaGV"].Value;
            }

            dgvEditingControlShowingHandler = new DataGridViewEditingControlShowingEventHandler(dgvPhanCong_EditingControlShowing);
            dgvCellValueChangedHandler = new DataGridViewCellEventHandler(dgvPhanCong_CellValueChanged);

            dgvPhanCong.EditingControlShowing -= dgvEditingControlShowingHandler;
            dgvPhanCong.EditingControlShowing += dgvEditingControlShowingHandler;
            dgvPhanCong.CellValueChanged -= dgvCellValueChangedHandler;
            dgvPhanCong.CellValueChanged += dgvCellValueChangedHandler;
        }

        #endregion

        #region HÀM VẼ GIAO DIỆN PHỤ (Helpers)

        private void DrawShadow(object sender, PaintEventArgs e, int radius, Color color, bool border = false)
        {
            Control control = sender as Control;
            if (control == null) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = CreateRoundedRect(control.ClientRectangle, radius))
            {
                using (SolidBrush brush = new SolidBrush(color))
                {
                    e.Graphics.FillPath(brush, path);
                }
                if (border)
                {
                    using (Pen pen = new Pen(Color.FromArgb(222, 226, 230)))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }
        }

        private void StyleDataGridView(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 245, 250);
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.DimGray;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(45, 45, 65);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 5, 0, 5);
            dgv.RowHeadersVisible = false;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgv.DefaultCellStyle.ForeColor = Color.DimGray;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowTemplate.Height = 40;
        }

        private GraphicsPath CreateRoundedRect(Rectangle r, int radius)
        {
            r.Width--; r.Height--;
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            if (d <= 0) { path.AddRectangle(r); return path; }
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (cboKhoi != null) this.cboKhoi.SelectedIndexChanged -= new System.EventHandler(this.cboKhoi_SelectedIndexChanged);
                if (dgvLopHoc != null) this.dgvLopHoc.SelectionChanged -= new System.EventHandler(this.dgvLopHoc_SelectionChanged);
                if (btnLuuHS != null) this.btnLuuHS.Click -= new System.EventHandler(this.btnLuuHS_Click);
                if (btnXoaHS != null) this.btnXoaHS.Click -= new System.EventHandler(this.btnXoaHS_Click);
                if (btnImportHS != null) this.btnImportHS.Click -= new System.EventHandler(this.btnImportHS_Click);
                if (btnAssignGvcn != null) this.btnAssignGvcn.Click -= new System.EventHandler(this.btnAssignGvcn_Click);
                if (btnImportPhanCong != null) this.btnImportPhanCong.Click -= new System.EventHandler(this.btnImportPhanCong_Click);
                if (btnChuyenLop != null) this.btnChuyenLop.Click -= new System.EventHandler(this.btnChuyenLop_Click);
                if (btnThemLop != null) this.btnThemLop.Click -= new System.EventHandler(this.btnThemLop_Click);
                if (btnXoaLop != null) this.btnXoaLop.Click -= new System.EventHandler(this.btnXoaLop_Click);

                if (btnXacNhanChuyen != null) btnXacNhanChuyen.Click -= btnXacNhanChuyenClickHandler;
                if (btnHuyChuyen != null) btnHuyChuyen.Click -= btnHuyChuyenClickHandler;
                if (pnlFilter != null) pnlFilter.Paint -= pnlFilterPaintHandler;
                if (pnlClassInfoCard != null) pnlClassInfoCard.Paint -= pnlClassInfoCardPaintHandler;
                if (pnlAssignGvcn != null) pnlAssignGvcn.Paint -= pnlAssignGvcnPaintHandler;
                if (dgvPhanCong != null)
                {
                    dgvPhanCong.DataError -= dgvDataErrorHandler;
                    dgvPhanCong.EditingControlShowing -= dgvEditingControlShowingHandler;
                    dgvPhanCong.CellValueChanged -= dgvCellValueChangedHandler;
                }

                clbHocSinhChuyen?.Dispose();
                cboLopMoi_Inline?.Dispose();
                btnXacNhanChuyen?.Dispose();
                btnHuyChuyen?.Dispose();

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}