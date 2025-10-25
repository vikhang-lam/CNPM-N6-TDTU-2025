using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public enum LoginStatus
{
    Success,
    InvalidCredentials,
    AccountNotActivated
}
public class TeacherProfile
{
    public string Ten { get; set; }
    public string TenMon { get; set; }
    public string Email { get; set; }
    public string SDT { get; set; }
    public string AnhDaiDien { get; set; }
}

public static class DatabaseHelper
{
    private static string connectionString =
        @"Data Source=LAPTOP-3IRTDDBK;Initial Catalog=quanlilophoc_giangday;Integrated Security=True;";
    //@"Data Source=DESKTOP-RH3KRAF\SQLEXPRESS;Initial Catalog=quanlilophoc_giangday;Integrated Security=True;";

    // Helper không thay đổi
    public static DataTable ExecuteQuery(string query)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
    }

    // Helper dùng cho các SP trả về BẢNG (SELECT)
    public static DataTable ExecuteStoredProcedure(string procedureName, params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
    }

    // Helper dùng cho các SP KHÔNG trả về gì (INSERT, UPDATE, DELETE)
    public static void ExecuteNonQueryStoredProcedure(string procedureName, params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }

    // Helper dùng cho các SP trả về MỘT GIÁ TRỊ (COUNT, MAX, 1 ID)
    public static object ExecuteScalarStoredProcedure(string procedureName, params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }

    #region Đăng nhập
    public static LoginStatus CheckTeacherLogin(string username, string password)
    {
        var pUser = new SqlParameter("@user", username);
        var pPass = new SqlParameter("@pass", password);

        object result = ExecuteScalarStoredProcedure("sp_CheckTeacherLogin", pUser, pPass);

        if (result == null || result == DBNull.Value)
        {
            return LoginStatus.InvalidCredentials;
        }

        string trangThai = result.ToString();

        if (trangThai.Equals("Chưa xác nhận", StringComparison.OrdinalIgnoreCase))
        {
            return LoginStatus.AccountNotActivated;
        }

        if (trangThai.Equals("Đã xác nhận", StringComparison.OrdinalIgnoreCase))
        {
            return LoginStatus.Success;
        }

        return LoginStatus.InvalidCredentials;
    }

    public static bool CheckAdminLogin(string username, string password)
    {
        var pUser = new SqlParameter("@user", username);
        var pPass = new SqlParameter("@pass", password);

        object result = ExecuteScalarStoredProcedure("sp_CheckAdminLogin", pUser, pPass);
        return (result != null && (int)result > 0);
    }
    #endregion

    #region Thông tin giáo viên
    public static TeacherProfile GetTeacherProfile(string username)
    {
        var pUser = new SqlParameter("@user", username);
        DataTable dt = ExecuteStoredProcedure("sp_GetTeacherProfile", pUser);

        if (dt.Rows.Count > 0)
        {
            DataRow r = dt.Rows[0];
            return new TeacherProfile
            {
                Ten = r["Ten"]?.ToString() ?? "",
                TenMon = r["TenMon"]?.ToString() ?? "Chưa có môn",
                Email = r["Email"]?.ToString() ?? "",
                SDT = r["SDT"]?.ToString() ?? "",
                AnhDaiDien = r["AnhDaiDien"]?.ToString()
            };
        }
        return null;
    }
    public static void UpdateTeacherAvatar(string username, string avatarPath)
    {
        var pUser = new SqlParameter("@user", username);
        var pAvatar = new SqlParameter("@avatar", (object)avatarPath ?? DBNull.Value);
        ExecuteNonQueryStoredProcedure("sp_UpdateTeacherAvatar", pUser, pAvatar);
    }

    public static string GetMonByTeacher(string identifier)
    {
        var pId = new SqlParameter("@id", identifier);
        object result = ExecuteScalarStoredProcedure("sp_GetMonByTeacher", pId);
        return result?.ToString() ?? "";
    }
    #endregion

    #region Học sinh
    public static DataTable GetHocSinhByLop(string maLop)
    {
        var pMaLop = new SqlParameter("@malop", maLop);
        return ExecuteStoredProcedure("sp_GetHocSinhByLop", pMaLop);
    }

    public static DataRow GetHocSinhProfile(string maHS)
    {
        var pMaHS = new SqlParameter("@maHS", maHS);
        DataTable dt = ExecuteStoredProcedure("sp_GetHocSinhProfile", pMaHS);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public static void UpdateHocSinh(string maHS, string hoTen, string gioiTinh, DateTime? ngaySinh, string diaChi)
    {
        var pMaHS = new SqlParameter("@MaHS", maHS);
        var pHoTen = new SqlParameter("@HoTen", hoTen ?? "");
        var pGioiTinh = new SqlParameter("@GioiTinh", gioiTinh ?? "");
        var pNgaySinh = new SqlParameter("@NgaySinh", (object)ngaySinh ?? DBNull.Value);
        var pDiaChi = new SqlParameter("@DiaChi", diaChi ?? "");

        ExecuteNonQueryStoredProcedure("sp_UpdateHocSinhProfile", pMaHS, pHoTen, pGioiTinh, pNgaySinh, pDiaChi);
    }
    #endregion

    #region Điểm danh

    public static DataTable GetDiemDanhByLop(string maLop)
    {
        var pMaLop = new SqlParameter("@maLop", maLop ?? string.Empty);
        return ExecuteStoredProcedure("sp_GetDiemDanhByLop", pMaLop);
    }

    public static void UpsertDiemDanh(string maHS, string maLop, DateTime ngay, string buoi, string trangThai)
    {
        if (string.IsNullOrWhiteSpace(maHS)) return;

        var pMaHS = new SqlParameter("@MaHS", maHS);
        var pNgay = new SqlParameter("@Ngay", ngay);
        var pBuoi = new SqlParameter("@Buoi", (object)buoi ?? DBNull.Value);
        var pTrangThai = new SqlParameter("@TrangThai", (object)trangThai ?? DBNull.Value);

        ExecuteNonQueryStoredProcedure("sp_UpsertDiemDanh", pMaHS, pNgay, pBuoi, pTrangThai);
    }
    public static DataTable GetDiemDanhByLopAndDate(string maLop, DateTime ngay, string buoi)
    {
        var pMaLop = new SqlParameter("@maLop", maLop ?? "");
        var pNgay = new SqlParameter("@ngay", ngay.Date);
        var pBuoi = new SqlParameter("@buoi", string.IsNullOrEmpty(buoi) ? (object)DBNull.Value : buoi);

        return ExecuteStoredProcedure("sp_GetDiemDanhByLopAndDate", pMaLop, pNgay, pBuoi);
    }

    public static void ExecTaoDiemDanhMacDinh(string maLop, DateTime ngay, string buoi)
    {
        // Toàn bộ logic C# cũ đã được thay thế bằng 1 lệnh gọi SP (đã có sẵn trong file .sql của bạn)
        var pMaLop = new SqlParameter("@MaLop", maLop);
        var pNgay = new SqlParameter("@Ngay", ngay.Date);
        var pBuoi = new SqlParameter("@Buoi", buoi);

        ExecuteNonQueryStoredProcedure("sp_TaoDiemDanhMacDinh", pMaLop, pNgay, pBuoi);
    }

    public static void ExecTaoDiemDanhMacDinh(string maLop)
    {
        ExecTaoDiemDanhMacDinh(maLop, DateTime.Today, "Sáng");
    }

    public static void LuuDiemDanh(string maHS, string trangThai, DateTime? ngay = null, string buoi = null)
    {
        DateTime actualDate = (ngay ?? DateTime.Now).Date;
        string actualBuoi = string.IsNullOrEmpty(buoi) ? "Sáng" : buoi;

        var pMaHS = new SqlParameter("@MaHS", maHS);
        var pNgay = new SqlParameter("@Ngay", actualDate);
        var pBuoi = new SqlParameter("@Buoi", actualBuoi);
        var pTrangThai = new SqlParameter("@TrangThai", trangThai);

        ExecuteNonQueryStoredProcedure("sp_UpsertDiemDanh", pMaHS, pNgay, pBuoi, pTrangThai);
    }

    public static void UpdateDiemDanh(string maDD, string trangThai)
    {
        var pMaDD = new SqlParameter("@MaDD", maDD);
        var pTrangThai = new SqlParameter("@TrangThai", trangThai);
        ExecuteNonQueryStoredProcedure("sp_UpdateDiemDanh", pMaDD, pTrangThai);
    }
    #endregion

    #region Kết quả học tập
    public static void LuuKetQuaHocTap(string maHS, string maMon, double diem, string nhanXet = "")
    {
        var pMaHS = new SqlParameter("@MaHS", maHS);
        var pMaMon = new SqlParameter("@MaMon", maMon);
        var pDiem = new SqlParameter("@Diem", diem);
        var pNhanXet = new SqlParameter("@NhanXet", nhanXet ?? "");

        ExecuteNonQueryStoredProcedure("sp_InsertKetQuaHocTap", pMaHS, pMaMon, pDiem, pNhanXet);
    }

    public static void UpdateKetQuaHocTap(string maHS, string maMon, string loai, float? diem)
    {
        try
        {
            var pMaHS = new SqlParameter("@MaHS", maHS);
            var pMaMon = new SqlParameter("@MaMon", maMon);
            var pLoai = new SqlParameter("@Loai", loai);
            var pDiem = new SqlParameter("@Diem", (object)diem ?? DBNull.Value);

            // ### SỬA: Thêm tham số NULL cho NhanXet và GhiChu ###
            var pNhanXet = new SqlParameter("@NhanXet", DBNull.Value);
            var pGhiChu = new SqlParameter("@GhiChu", DBNull.Value);

            ExecuteNonQueryStoredProcedure("sp_UpsertKetQuaHocTap", pMaHS, pMaMon, pLoai, pDiem, pNhanXet, pGhiChu);
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
            // Bắt lỗi RAISERROR từ SQL (lỗi 50000)
            if (ex.Number == 50000)
            {
                // Hiển thị thông báo lỗi thân thiện cho giáo viên
                MessageBox.Show(ex.Message, "Thông Báo Khóa Điểm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Ném các lỗi SQL khác
                throw;
            }
        }
        catch (Exception ex)
        {
            // Bắt các lỗi C# khác
            MessageBox.Show("Đã xảy ra lỗi khi lưu điểm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    // ### MỚI: Hàm riêng để lưu Nhận Xét / Ghi Chú ###
    public static void UpdateKetQuaHocTap_Text(string maHS, string maMon, string loai, string textValue, bool isNhanXet)
    {
        try
        {
            var pMaHS = new SqlParameter("@MaHS", maHS);
            var pMaMon = new SqlParameter("@MaMon", maMon);
            var pLoai = new SqlParameter("@Loai", loai);
            var pDiem = new SqlParameter("@Diem", DBNull.Value); // Không cập nhật điểm

            SqlParameter pNhanXet, pGhiChu;
            if (isNhanXet)
            {
                pNhanXet = new SqlParameter("@NhanXet", (object)textValue ?? DBNull.Value);
                pGhiChu = new SqlParameter("@GhiChu", DBNull.Value);
            }
            else
            {
                pNhanXet = new SqlParameter("@NhanXet", DBNull.Value);
                pGhiChu = new SqlParameter("@GhiChu", (object)textValue ?? DBNull.Value);
            }

            ExecuteNonQueryStoredProcedure("sp_UpsertKetQuaHocTap", pMaHS, pMaMon, pLoai, pDiem, pNhanXet, pGhiChu);
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
            if (ex.Number == 50000)
            {
                MessageBox.Show(ex.Message, "Thông Báo Khóa Điểm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Đã xảy ra lỗi khi lưu nhận xét: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static DataTable GetKetQuaHocTapByLop(string maLop)
    {
        var pMaLop = new SqlParameter("@malop", maLop);
        return ExecuteStoredProcedure("sp_GetKetQuaHocTapByLop", pMaLop);
    }

    public static DataTable GetBangDiemPivot(string maLop, int ki, string maMon)
    {
        var pMaLop = new SqlParameter("@malop", maLop);
        var pKi = new SqlParameter("@ki", ki);
        var pMaMon = new SqlParameter("@maMon", maMon);

        return ExecuteStoredProcedure("sp_GetBangDiemPivot", pMaLop, pKi, pMaMon);
    }
    #endregion

    #region Hồ sơ cá nhân (Profile)
    public static void UpdateTeacherProfile(string username, string email, string phone, string avatarPath)
    {
        var pUser = new SqlParameter("@u", username);
        var pEmail = new SqlParameter("@e", email ?? "");
        var pSdt = new SqlParameter("@s", phone ?? "");
        var pAvatar = new SqlParameter("@a", (object)avatarPath ?? DBNull.Value);

        ExecuteNonQueryStoredProcedure("sp_UpdateTeacherProfile", pUser, pEmail, pSdt, pAvatar);
    }

    public static bool ChangeTeacherPassword(string username, string oldPass, string newPass)
    {
        if (System.Text.RegularExpressions.Regex.IsMatch(newPass, @"[^a-zA-Z0-9]"))
        {
            throw new ArgumentException("Mật khẩu không được chứa ký tự đặc biệt!");
        }

        var pUser = new SqlParameter("@u", username);
        var pOld = new SqlParameter("@oldPass", oldPass);
        var pNew = new SqlParameter("@newPass", newPass);

        object result = ExecuteScalarStoredProcedure("sp_ChangeTeacherPassword", pUser, pOld, pNew);
        return (result != null && (int)result == 1);
    }
    public static DataRow GetAdminProfile(string username)
    {
        var pUser = new SqlParameter("@u", username);
        DataTable dt = ExecuteStoredProcedure("sp_GetAdminProfile", pUser);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }
    public static void UpdateAdminEmail(string username, string email)
    {
        var pUser = new SqlParameter("@u", username);
        var pEmail = new SqlParameter("@e", email);
        ExecuteNonQueryStoredProcedure("sp_UpdateAdminEmail", pUser, pEmail);
    }
    public static bool ChangeAdminPassword(string username, string oldPass, string newPass)
    {
        if (string.IsNullOrWhiteSpace(newPass))
        {
            throw new ArgumentException("Mật khẩu mới không được để trống!");
        }
        if (System.Text.RegularExpressions.Regex.IsMatch(newPass, @"[^a-zA-Z0-9]"))
        {
            throw new ArgumentException("Mật khẩu mới không được chứa ký tự đặc biệt!");
        }

        var pUser = new SqlParameter("@u", username);
        var pOld = new SqlParameter("@oldPass", oldPass);
        var pNew = new SqlParameter("@newPass", newPass);

        object result = ExecuteScalarStoredProcedure("sp_ChangeAdminPassword", pUser, pOld, pNew);
        return (result != null && (int)result == 1);
    }

    #endregion
    #region Tài liệu
    public static DataTable GetTaiLieuByGV(string maGV)
    {
        var pMaGV = new SqlParameter("@gv", maGV);
        return ExecuteStoredProcedure("sp_GetTaiLieuByGV", pMaGV);
    }

    public static void InsertTaiLieu(string maGV, string ten, string moTa, string filePath, string trangThai)
    {
        var pMaGV = new SqlParameter("@gv", maGV);
        var pTen = new SqlParameter("@ten", ten);
        var pMoTa = new SqlParameter("@moTa", moTa ?? "");
        var pKieu = new SqlParameter("@kieu", filePath);
        var pTrangThai = new SqlParameter("@tt", trangThai ?? "Riêng tư");

        ExecuteNonQueryStoredProcedure("sp_InsertTaiLieu", pMaGV, pTen, pMoTa, pKieu, pTrangThai);
    }
    public static void UpdateHocSinhLop(string maHS, string maLopMoi)
    {
        var pMaHS = new SqlParameter("@MaHS", maHS);
        var pMaLopMoi = new SqlParameter("@MaLopMoi", (object)maLopMoi ?? DBNull.Value);
        ExecuteNonQueryStoredProcedure("sp_UpdateHocSinhLop", pMaHS, pMaLopMoi);
    }
    public static int UpdateHocSinhLop_Multi(System.Collections.Generic.List<string> maHocSinhList, string maLopMoi)
    {
        // 1. Chuyển List<string> thành DataTable
        DataTable dt = new DataTable();
        dt.Columns.Add("MaHS", typeof(string));
        foreach (string maHS in maHocSinhList)
        {
            dt.Rows.Add(maHS);
        }

        // 2. Chuẩn bị tham số
        var pMaHSList = new SqlParameter("@MaHSList", SqlDbType.Structured)
        {
            TypeName = "ut_MaHSList",
            Value = dt
        };
        var pMaLopMoi = new SqlParameter("@MaLopMoi", (object)maLopMoi ?? DBNull.Value);

        // 3. Gọi SP và trả về số lượng hàng bị ảnh hưởng
        object result = ExecuteScalarStoredProcedure("sp_UpdateHocSinhLop_Multi", pMaHSList, pMaLopMoi);
        return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
    }
    public static DataTable GetTaiLieuShared()
    {
        return ExecuteStoredProcedure("sp_GetTaiLieuShared");
    }
    public static void DeleteTaiLieu(string maTL)
    {
        var pMaTL = new SqlParameter("@id", maTL);
        ExecuteNonQueryStoredProcedure("sp_DeleteTaiLieu", pMaTL);
    }

    public static void ShareTaiLieu(string maTL)
    {
        var pMaTL = new SqlParameter("@id", maTL);
        ExecuteNonQueryStoredProcedure("sp_ShareTaiLieu", pMaTL);
    }

    public static string GetMaGVByUsername(string username)
    {
        var pUser = new SqlParameter("@u", username);
        object result = ExecuteScalarStoredProcedure("sp_GetMaGVByUsername", pUser);
        return result?.ToString();
    }
    #endregion
    #region Thời khóa biểu
    public static DataTable GetTKBByGV(string maGV, DateTime monday)
    {
        DateTime sunday = monday.AddDays(6);
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pMonday = new SqlParameter("@Monday", monday.Date);
        var pSunday = new SqlParameter("@Sunday", sunday.Date);

        return ExecuteStoredProcedure("sp_GetTKBByGV", pMaGV, pMonday, pSunday);
    }

    public static void UpdateCellColor(string maGV, DateTime ngay, int tiet, string colorHex)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pNgay = new SqlParameter("@Ngay", ngay.Date);
        var pTiet = new SqlParameter("@Tiet", tiet);
        var pColor = new SqlParameter("@MauSac", string.IsNullOrEmpty(colorHex) ? (object)DBNull.Value : colorHex);

        ExecuteNonQueryStoredProcedure("sp_UpsertTKBColor", pMaGV, pNgay, pTiet, pColor);
    }

    public static void UpsertGhiChuTKB(string maGV, DateTime ngay, int tiet, string note)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pNgay = new SqlParameter("@Ngay", ngay.Date);
        var pTiet = new SqlParameter("@Tiet", tiet);
        var pNote = new SqlParameter("@Note", note);

        ExecuteNonQueryStoredProcedure("sp_UpsertTKBGhiChu", pMaGV, pNgay, pTiet, pNote);
    }

    #endregion
    public static DataTable GetMiniGames()
    {
        return ExecuteStoredProcedure("sp_GetMiniGames");
    }
    #region Minigame Data
    public static string GetGameData(string maMNG)
    {
        var pMaMNG = new SqlParameter("@maMNG", maMNG);
        object result = ExecuteScalarStoredProcedure("sp_GetGameData", pMaMNG);
        return result?.ToString();
    }
    public static void SaveGameData(string maMNG, string data)
    {
        var pMaMNG = new SqlParameter("@maMNG", maMNG);
        var pData = new SqlParameter("@data", (object)data ?? DBNull.Value);
        ExecuteNonQueryStoredProcedure("sp_SaveGameData", pMaMNG, pData);
    }
    #endregion
    public static void DeleteGhiChuTKB(string maGV, DateTime ngay, int tiet)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pNgay = new SqlParameter("@Ngay", ngay.Date);
        var pTiet = new SqlParameter("@Tiet", tiet);
        ExecuteNonQueryStoredProcedure("sp_DeleteTKBGhiChu", pMaGV, pNgay, pTiet);
    }
    public static DataTable GetGiaoVienByTrangThai(string trangThai)
    {
        var pTT = new SqlParameter("@tt", trangThai);
        return ExecuteStoredProcedure("sp_GetGiaoVienByTrangThai", pTT);
    }

    public static Tuple<string, string> UpdateTrangThaiGiaoVien(string maGV, string trangThai)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateTrangThaiGiaoVien", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.AddWithValue("@id", maGV);
                    cmd.Parameters.AddWithValue("@tt", trangThai);

                    // Output parameters
                    SqlParameter emailParam = new SqlParameter("@Email", SqlDbType.NVarChar, 50)
                    {
                        Direction = ParameterDirection.Output
                    };
                    SqlParameter tenParam = new SqlParameter("@Ten", SqlDbType.NVarChar, 100)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(emailParam);
                    cmd.Parameters.Add(tenParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    // Lấy giá trị trả về
                    string email = emailParam.Value?.ToString();
                    string ten = tenParam.Value?.ToString();

                    return Tuple.Create(email, ten);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Lỗi khi cập nhật trạng thái giáo viên: " + ex.Message);
        }
    }

    public static void UpdateGiaoVien(string maGV, string ten, string email, string sdt)
    {
        var pMaGV = new SqlParameter("@id", maGV);
        var pTen = new SqlParameter("@t", ten);
        var pEmail = new SqlParameter("@e", email);
        var pSdt = new SqlParameter("@s", sdt);
        ExecuteNonQueryStoredProcedure("sp_UpdateGiaoVien", pMaGV, pTen, pEmail, pSdt);
    }

    public static void DeleteGiaoVien(string maGV)
    {
        var pMaGV = new SqlParameter("@id", maGV);
        ExecuteNonQueryStoredProcedure("sp_DeleteGiaoVien", pMaGV);
    }
    public static DataTable GetAllGiaoVien()
    {
        return ExecuteStoredProcedure("sp_GetAllGiaoVien");
    }
    public static DataTable GetAllHocSinh()
    {
        return ExecuteStoredProcedure("sp_GetAllHocSinh");
    }


    public static void InsertHocSinh(string maHS, string maLop, string hoTen, DateTime ngaySinh,
                                     string gioiTinh, string sdtPH, string diaChi, string danToc)
    {
        try
        {
            var pMaHS = new SqlParameter("@MaHS", maHS);
            var pMaLop = new SqlParameter("@MaLop", string.IsNullOrWhiteSpace(maLop) ? (object)DBNull.Value : maLop);
            var pHoTen = new SqlParameter("@HoTen", hoTen ?? "");
            var pNgaySinh = new SqlParameter("@NgaySinh", ngaySinh);
            var pGioiTinh = new SqlParameter("@GioiTinh", gioiTinh ?? "");
            var pSdt = new SqlParameter("@SDT", sdtPH ?? "");
            var pDiaChi = new SqlParameter("@DiaChi", diaChi ?? "");
            var pDanToc = new SqlParameter("@DanToc", danToc ?? "");

            ExecuteNonQueryStoredProcedure("sp_InsertHocSinh", pMaHS, pMaLop, pHoTen, pNgaySinh, pGioiTinh, pSdt, pDiaChi, pDanToc);
        }
        catch (SqlException ex)
        {
            // Bắt lỗi RAISERROR từ SP
            if (ex.Number == 50000)
            {
                throw new Exception(ex.Message);
            }
            throw;
        }
    }


    public static void UpdateHocSinh(string maHS, string hoTen, DateTime ngaySinh,
                                     string gioiTinh, string sdtPH, string diaChi, string danToc)
    {
        var pMaHS = new SqlParameter("@MaHS", maHS);
        var pHoTen = new SqlParameter("@HoTen", hoTen ?? "");
        var pNgaySinh = new SqlParameter("@NgaySinh", ngaySinh);
        var pGioiTinh = new SqlParameter("@GioiTinh", gioiTinh ?? "");
        var pSdt = new SqlParameter("@SDT", sdtPH ?? "");
        var pDiaChi = new SqlParameter("@DiaChi", diaChi ?? "");
        var pDanToc = new SqlParameter("@DanToc", danToc ?? "");
        ExecuteNonQueryStoredProcedure("sp_UpdateHocSinh", pMaHS, pHoTen, pNgaySinh, pGioiTinh, pSdt, pDiaChi, pDanToc);
    }


    public static void DeleteHocSinh(string maHS)
    {
        var pMaHS = new SqlParameter("@MaHS", maHS);
        ExecuteNonQueryStoredProcedure("sp_DeleteHocSinh", pMaHS);
    }
    public struct ImportResult
    {
        public int Success;
        public int Skipped;
        public int Failed;
    }

    // Đã REFACTOR: Dùng Table-Valued Parameter
    public static ImportResult ImportHocSinhFromDataTable(DataTable dt)
    {
        // Tạo một DataTable mới chỉ chứa các cột khớp với TVP
        DataTable tvpTable = new DataTable();
        tvpTable.Columns.Add("MaHS", typeof(string));
        tvpTable.Columns.Add("MaLop", typeof(string));
        tvpTable.Columns.Add("HoTen", typeof(string));
        tvpTable.Columns.Add("NgaySinh", typeof(DateTime));
        tvpTable.Columns.Add("GioiTinh", typeof(string));
        tvpTable.Columns.Add("SDTPhuHuynh", typeof(string));
        tvpTable.Columns.Add("DiaChi", typeof(string));
        tvpTable.Columns.Add("DanToc", typeof(string));

        int failed = 0;

        foreach (DataRow row in dt.Rows)
        {
            DateTime ngaySinh;
            if (!DateTime.TryParse(row["NgaySinh"]?.ToString().Trim(), out ngaySinh))
            {
                // Thử định dạng dd/MM/yyyy
                if (!DateTime.TryParseExact(row["NgaySinh"]?.ToString().Trim(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out ngaySinh))
                {
                    failed++;
                    continue; // Bỏ qua nếu ngày sinh không hợp lệ
                }
            }

            tvpTable.Rows.Add(
                row["MaHS"]?.ToString().Trim(),
                row["MaLop"]?.ToString().Trim(),
                row["HoTen"]?.ToString().Trim(),
                ngaySinh,
                row["GioiTinh"]?.ToString().Trim(),
                row["SDTPhuHuynh"]?.ToString().Trim(),
                row["DiaChi"]?.ToString().Trim(),
                row["DanToc"]?.ToString().Trim()
            );
        }

        var pTVP = new SqlParameter("@HocSinhData", SqlDbType.Structured)
        {
            TypeName = "ut_HocSinhImport",
            Value = tvpTable
        };

        // SP này sẽ trả về 2 cột: [Success] và [Skipped]
        DataTable resultDt = ExecuteStoredProcedure("sp_ImportHocSinh", pTVP);

        var result = new ImportResult();
        if (resultDt.Rows.Count > 0)
        {
            result.Success = (int)resultDt.Rows[0]["Success"];
            result.Skipped = (int)resultDt.Rows[0]["Skipped"];
        }
        result.Failed = failed + (dt.Rows.Count - result.Success - result.Skipped - failed);

        return result;
    }

    #region Báo cáo
    public static DataTable GetLopByGiaoVien(string maGV)
    {
        var pMaGV = new SqlParameter("@maGV", maGV);
        return ExecuteStoredProcedure("sp_GetLopByGiaoVien", pMaGV);
    }

    public static DataTable GetBangDiemHocKy(string maLop, int hocKy)
    {
        var pMaLop = new SqlParameter("@maLop", maLop);
        var pHocKy = new SqlParameter("@hocKy", hocKy);
        return ExecuteStoredProcedure("sp_GetBangDiemHocKy", pMaLop, pHocKy);
    }

    public static DataTable GetHoSoHocSinh(string maLop)
    {
        var pMaLop = new SqlParameter("@maLop", maLop);
        return ExecuteStoredProcedure("sp_GetHoSoHocSinh", pMaLop);
    }

    public static DataTable GetThongKeKhoi(string khoi)
    {
        var pKhoi = new SqlParameter("@khoi", khoi);
        return ExecuteStoredProcedure("sp_GetThongKeKhoi", pKhoi);
    }
    #endregion

    #region Hỗ trợ Giảng dạy

    public static void AddGhiChuTKB(string maGV, string maLop, DateTime ngay, int tiet, string ghiChu)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pMaLop = new SqlParameter("@MaLop", maLop);
        var pNgay = new SqlParameter("@Ngay", ngay.Date);
        var pTiet = new SqlParameter("@Tiet", tiet);
        var pGhiChu = new SqlParameter("@GhiChu", ghiChu);

        ExecuteNonQueryStoredProcedure("sp_AddGhiChuTKB", pMaGV, pMaLop, pNgay, pTiet, pGhiChu);
    }
    public static void AddGhiChuChoHocSinh(string maHS, string maMon, string ghiChu)
    {
        var pMaHS = new SqlParameter("@MaHS", maHS);
        var pMaMon = new SqlParameter("@MaMon", maMon);
        var pGhiChu = new SqlParameter("@GhiChu", ghiChu);

        ExecuteNonQueryStoredProcedure("sp_AddGhiChuChoHocSinh", pMaHS, pMaMon, pGhiChu);
    }
    public static DataTable GetAllKetQuaHocTap()
    {
        return ExecuteStoredProcedure("sp_GetAllKetQuaHocTap");
    }
    public static void StyleDataGridView(DataGridView dgv)
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
    #endregion

    public static DataTable GetAllMonHoc()
    {
        return ExecuteStoredProcedure("sp_GetAllMonHoc");
    }

    public static DataTable GetScoresForAnalysis(string maGV, string phamVi, string chiTiet, string maMon, int hocKy)
    {
        var pMaGV = new SqlParameter("@maGV", maGV);
        var pPhamVi = new SqlParameter("@phamVi", phamVi);
        var pChiTiet = new SqlParameter("@chiTiet", chiTiet);
        var pMaMon = new SqlParameter("@maMon", maMon);
        var pHocKy = new SqlParameter("@hocKy", hocKy);

        return ExecuteStoredProcedure("sp_GetScoresForAnalysis", pMaGV, pPhamVi, pChiTiet, pMaMon, pHocKy);
    }
    public static void CreateTeacherRequest(string ten, string username, string password, string email, string sdt)
    {
        try
        {
            var pTen = new SqlParameter("@Ten", ten);
            var pUser = new SqlParameter("@Username", username);
            var pPass = new SqlParameter("@Password", password);
            var pEmail = new SqlParameter("@Email", email);
            var pSdt = new SqlParameter("@SDT", sdt);

            ExecuteNonQueryStoredProcedure("sp_CreateTeacherRequest", pTen, pUser, pPass, pEmail, pSdt);
        }
        catch (SqlException ex)
        {
            if (ex.Number == 50000)
            {
                throw new Exception(ex.Message);
            }
            throw;
        }
    }

    public static DataTable GetTaiLieuSharedWithUploader()
    {
        return ExecuteStoredProcedure("sp_GetTaiLieuSharedWithUploader");
    }
    public static void UnshareTaiLieu(string maTL)
    {
        var pMaTL = new SqlParameter("@MaTL", maTL);
        ExecuteNonQueryStoredProcedure("sp_UnshareTaiLieu", pMaTL);
    }

    public static DataTable GetHomeroomClassesByTeacher(string maGV)
    {
        var pMaGV = new SqlParameter("@maGV", maGV);
        return ExecuteStoredProcedure("sp_GetHomeroomClassesByTeacher", pMaGV);
    }

    public static string GetHomeroomClassNameByTeacherId(string maGV)
    {
        if (string.IsNullOrEmpty(maGV)) return null;

        var pMaGV = new SqlParameter("@MaGV", maGV);
        object result = ExecuteScalarStoredProcedure("sp_GetHomeroomClassNameByTeacherId", pMaGV);
        return result?.ToString();
    }
    public static DataTable GetHomeroomGradebook(string maLop, string loaiDiem)
    {
        // Hàm này vốn đã dùng SP, giữ nguyên
        return ExecuteStoredProcedure("sp_GetHomeroomGradebook",
            new SqlParameter("@MaLop", maLop),
            new SqlParameter("@LoaiDiem", loaiDiem)
        );
    }

    #region Global Events
    public static event EventHandler ThoiKhoaBieuChanged;

    public static void RaiseThoiKhoaBieuChanged()
    {
        ThoiKhoaBieuChanged?.Invoke(null, EventArgs.Empty);
    }
    #endregion

    public static DataTable GetBaoCaoChuyenCan(string maLop, int hocKy)
    {
        var pMaLop = new SqlParameter("@maLop", maLop);
        var pHocKy = new SqlParameter("@hocKy", hocKy);
        return ExecuteStoredProcedure("sp_GetBaoCaoChuyenCan", pMaLop, pHocKy);
    }
    public static DataTable GetMonHocByGiaoVienAndLop(string maGV, string maLop)
    {
        var pMaGV = new SqlParameter("@maGV", maGV);
        var pMaLop = new SqlParameter("@maLop", maLop);
        return ExecuteStoredProcedure("sp_GetMonHocByGiaoVienAndLop", pMaGV, pMaLop);
    }

    public static DataTable GetAllLopHoc()
    {
        return ExecuteStoredProcedure("sp_GetAllLopHoc");
    }

    public static DataTable GetUnassignedHomeroomTeachers()
    {
        return ExecuteStoredProcedure("sp_GetUnassignedHomeroomTeachers");
    }

    public static void UpdateGvcnForLop(string maLop, string maGV)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        var pMaGV = new SqlParameter("@MaGV", string.IsNullOrEmpty(maGV) ? (object)DBNull.Value : maGV);
        ExecuteNonQueryStoredProcedure("sp_UpdateGvcnForLop", pMaLop, pMaGV);
    }

    public static DataRow GetLopHocDetails(string maLop)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        DataTable dt = ExecuteStoredProcedure("sp_GetLopHocDetails", pMaLop);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public static DataTable GetPhanCongGiangDayByLop(string maLop)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        return ExecuteStoredProcedure("sp_GetPhanCongGiangDayByLop", pMaLop);
    }

    public static void UpdatePhanCong(string maLop, string maMon, string newMaGV)
    {
        try
        {
            var pMaLop = new SqlParameter("@MaLop", maLop);
            var pMaMon = new SqlParameter("@MaMon", maMon);
            var pNewMaGV = new SqlParameter("@NewMaGV", string.IsNullOrEmpty(newMaGV) ? (object)DBNull.Value : newMaGV);

            ExecuteNonQueryStoredProcedure("sp_UpdatePhanCong", pMaLop, pMaMon, pNewMaGV);
        }
        catch (Exception ex)
        {
            // Ném lỗi ra ngoài để UI có thể bắt
            throw new Exception("Lỗi khi cập nhật phân công: " + ex.Message, ex);
        }
    }

    // Đã REFACTOR: Dùng Table-Valued Parameter
    public static ImportResult ImportHocSinhToLop(DataTable dt, string maLopTarget)
    {
        DataTable tvpTable = new DataTable();
        tvpTable.Columns.Add("MaHS", typeof(string));
        tvpTable.Columns.Add("MaLop", typeof(string)); // Sẽ bị ghi đè bởi @MaLopTarget
        tvpTable.Columns.Add("HoTen", typeof(string));
        tvpTable.Columns.Add("NgaySinh", typeof(DateTime));
        tvpTable.Columns.Add("GioiTinh", typeof(string));
        tvpTable.Columns.Add("SDTPhuHuynh", typeof(string));
        tvpTable.Columns.Add("DiaChi", typeof(string));
        tvpTable.Columns.Add("DanToc", typeof(string));

        int failed = 0;

        foreach (DataRow row in dt.Rows)
        {
            DateTime ngaySinh;
            if (!DateTime.TryParse(row["NgaySinh"]?.ToString().Trim(), out ngaySinh))
            {
                failed++;
                continue;
            }
            if (string.IsNullOrWhiteSpace(row["MaHS"]?.ToString().Trim()))
            {
                failed++;
                continue;
            }

            tvpTable.Rows.Add(
                row["MaHS"]?.ToString().Trim(),
                null, // MaLop sẽ được gán từ @MaLopTarget
                row["HoTen"]?.ToString().Trim(),
                ngaySinh,
                row["GioiTinh"]?.ToString().Trim(),
                row["SDTPhuHuynh"]?.ToString().Trim(),
                row["DiaChi"]?.ToString().Trim(),
                row["DanToc"]?.ToString().Trim()
            );
        }

        var pTVP = new SqlParameter("@HocSinhData", SqlDbType.Structured)
        {
            TypeName = "ut_HocSinhImport",
            Value = tvpTable
        };
        var pMaLop = new SqlParameter("@MaLopTarget", maLopTarget);

        DataTable resultDt = ExecuteStoredProcedure("sp_ImportHocSinhToLop", pMaLop, pTVP);

        var result = new ImportResult();
        if (resultDt.Rows.Count > 0)
        {
            result.Success = (int)resultDt.Rows[0]["Success"];
            result.Skipped = (int)resultDt.Rows[0]["Skipped"];
        }
        result.Failed = failed + (dt.Rows.Count - result.Success - result.Skipped - failed);

        return result;
    }

    // Đã REFACTOR: Dùng Table-Valued Parameter
    public static ImportResult ImportThoiKhoaBieuForGV(string maGV, DataTable dt)
    {
        DataTable tvpTable = new DataTable();
        tvpTable.Columns.Add("Ngay", typeof(DateTime));
        tvpTable.Columns.Add("Tiet", typeof(int));
        tvpTable.Columns.Add("TenMon", typeof(string));
        tvpTable.Columns.Add("TenLop", typeof(string));
        tvpTable.Columns.Add("GhiChu", typeof(string));
        tvpTable.Columns.Add("MauSac", typeof(string));

        int failed = 0;
        var datesInExcel = new List<DateTime>();

        foreach (DataRow row in dt.Rows)
        {
            try
            {
                DateTime ngay = Convert.ToDateTime(row["Ngay"]).Date;
                int tiet = Convert.ToInt32(row["Tiet"]);

                if (!datesInExcel.Contains(ngay))
                {
                    datesInExcel.Add(ngay);
                }

                tvpTable.Rows.Add(
                    ngay,
                    tiet,
                    row["TenMon"]?.ToString().Trim(),
                    row["TenLop"]?.ToString().Trim(),
                    row["GhiChu"]?.ToString().Trim(),
                    row["MauSac"]?.ToString().Trim()
                );
            }
            catch
            {
                failed++;
            }
        }

        // Tạo DataTable cho danh sách ngày
        DataTable dtDates = new DataTable();
        dtDates.Columns.Add("Ngay", typeof(DateTime));
        foreach (var date in datesInExcel.Distinct())
        {
            dtDates.Rows.Add(date);
        }

        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pDates = new SqlParameter("@NgayList", SqlDbType.Structured)
        {
            TypeName = "ut_DateList",
            Value = dtDates
        };
        var pTKB = new SqlParameter("@TKBData", SqlDbType.Structured)
        {
            TypeName = "ut_TKBImport",
            Value = tvpTable
        };

        DataTable resultDt = ExecuteStoredProcedure("sp_ImportTKBForGV", pMaGV, pDates, pTKB);

        var result = new ImportResult();
        if (resultDt.Rows.Count > 0)
        {
            result.Success = (int)resultDt.Rows[0]["Success"];
            result.Failed = (int)resultDt.Rows[0]["Failed"] + failed;
        }

        return result;
    }

    public static string GetTeacherNameById(string maGV)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        object result = ExecuteScalarStoredProcedure("sp_GetTeacherNameById", pMaGV);
        return result?.ToString() ?? "Không rõ";
    }
    #region Quỹ Lớp
    public static DataTable GetQuyLopByLop(string maLop)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        return ExecuteStoredProcedure("sp_GetQuyLopByLop", pMaLop);
    }

    public static void InsertQuyLop(string maLop, string loai, decimal soTien, DateTime ngay, string ghiChu)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        var pLoai = new SqlParameter("@Loai", loai);
        var pSoTien = new SqlParameter("@SoTien", soTien);
        var pNgay = new SqlParameter("@Ngay", ngay.Date);
        var pGhiChu = new SqlParameter("@GhiChu", (object)ghiChu ?? DBNull.Value);

        ExecuteNonQueryStoredProcedure("sp_InsertQuyLop", pMaLop, pLoai, pSoTien, pNgay, pGhiChu);
    }

    public static void DeleteQuyLop(string maQL)
    {
        var pMaQL = new SqlParameter("@MaQL", maQL);
        ExecuteNonQueryStoredProcedure("sp_DeleteQuyLop", pMaQL);
    }
    #endregion
    public static DataTable GetMonHocByGiaoVien(string maGV)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        return ExecuteStoredProcedure("sp_GetMonHocByGiaoVien", pMaGV);
    }
    public static void UpdateGiaoVien_MonHoc(string maGV, DataTable dtMaMon)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pMonHocList = new SqlParameter("@MonHocList", SqlDbType.Structured)
        {
            TypeName = "ut_MaMonList",
            Value = dtMaMon
        };
        ExecuteNonQueryStoredProcedure("sp_UpdateGiaoVien_MonHoc", pMaGV, pMonHocList);
    }
    #region Quản lý Trường học (MonHoc, ThoiHanDiem, LenLop)

    /// <summary>
    /// Thêm một môn học mới. SP tự tạo MaMon dựa trên TenMon.
    /// </summary>
    public static void InsertMonHoc(string tenMon)
    {
        try
        {
            var pTenMon = new SqlParameter("@TenMon", tenMon);
            ExecuteNonQueryStoredProcedure("sp_InsertMonHoc", pTenMon);
        }
        catch (SqlException ex)
        {
            // Bắt lỗi RAISERROR từ SP
            if (ex.Number == 50000) throw new Exception(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Cập nhật tên của một môn học.
    /// </summary>
    public static void UpdateMonHoc(string maMon, string tenMon)
    {
        try
        {
            var pMaMon = new SqlParameter("@MaMon", maMon);
            var pTenMon = new SqlParameter("@TenMon", tenMon);
            ExecuteNonQueryStoredProcedure("sp_UpdateMonHoc", pMaMon, pTenMon);
        }
        catch (SqlException ex)
        {
            if (ex.Number == 50000) throw new Exception(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Xóa một môn học (chỉ khi không được sử dụng).
    /// </summary>
    public static void DeleteMonHoc(string maMon)
    {
        try
        {
            var pMaMon = new SqlParameter("@MaMon", maMon);
            ExecuteNonQueryStoredProcedure("sp_DeleteMonHoc", pMaMon);
        }
        catch (SqlException ex)
        {
            if (ex.Number == 50000) throw new Exception(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Lấy danh sách cài đặt thời hạn nhập điểm (bao gồm cột 'DaKhoa' tự tính toán).
    /// </summary>
    public static DataTable GetThoiHanDiem()
    {
        return ExecuteStoredProcedure("sp_GetThoiHanDiem");
    }

    /// <summary>
    /// Cập nhật một hàng trong bảng ThoiHanDiem.
    /// </summary>
    public static void UpdateThoiHanDiem(string maCotDiem, string khoi, int hocKy, DateTime ngayMo, DateTime ngayKhoa, bool khoaThuCong)
    {
        var pMaCotDiem = new SqlParameter("@MaCotDiem", maCotDiem);
        var pKhoi = new SqlParameter("@Khoi", khoi);
        var pHocKy = new SqlParameter("@HocKy", hocKy);
        var pNgayMo = new SqlParameter("@NgayMoDiem", ngayMo);
        var pNgayKhoa = new SqlParameter("@NgayKhoaDiem", ngayKhoa);
        var pKhoaThuCong = new SqlParameter("@KhoaThuCong", khoaThuCong);

        ExecuteNonQueryStoredProcedure("sp_UpdateThoiHanDiem", pMaCotDiem, pKhoi, pHocKy, pNgayMo, pNgayKhoa, pKhoaThuCong);
    }

    /// <summary>
    /// Thực thi SP nghiệp vụ cuối năm: tính điểm TB và chuyển học sinh sang lớp mới.
    /// Trả về 1 DataTable chứa 1 dòng kết quả (SoHSLenLop, SoHSOLaiLop, SoHSTotNghiep).
    /// </summary>
    public static DataTable ProcessStudentPromotion(string maLopCu, string maLopMoi_LenLop, string maLopMoi_OLaiLop, bool isLop5)
    {
        var pMaLopCu = new SqlParameter("@MaLopCu", maLopCu);

        var pMaLopMoi_LenLop = new SqlParameter("@MaLopMoi_LenLop",
            string.IsNullOrEmpty(maLopMoi_LenLop) ? (object)DBNull.Value : maLopMoi_LenLop);

        var pMaLopMoi_OLaiLop = new SqlParameter("@MaLopMoi_OLaiLop",
            string.IsNullOrEmpty(maLopMoi_OLaiLop) ? (object)DBNull.Value : maLopMoi_OLaiLop);

        var pIsLop5 = new SqlParameter("@IsLop5_TotNghiep", isLop5);

        // SP này trả về một bảng (DataTable) chứa 1 dòng kết quả
        return ExecuteStoredProcedure("sp_ProcessStudentPromotion",
            pMaLopCu, pMaLopMoi_LenLop, pMaLopMoi_OLaiLop, pIsLop5);
    }
    #region Báo cáo (Admin)

    /// <summary>
    /// ADMIN: Lấy báo cáo chuyên cần theo Khối, Lớp (hoặc toàn trường) và Học kỳ.
    /// </summary>
    /// <param name="khoi">Khối cần xem (vd: "Khối 1"), hoặc null cho tất cả.</param>
    /// <param name="maLop">Mã lớp cụ thể, hoặc null.</param>
    /// <param name="hocKy">1: HK1, 2: HK2, 3: Cả năm.</param>
    // Sửa lại:
    public static DataTable GetBaoCaoChuyenCan_Admin(string khoi, string maLop, int hocKy)
    {
        var pKhoi = new SqlParameter("@Khoi", (object)khoi ?? DBNull.Value);
        var pMaLop = new SqlParameter("@MaLop", (object)maLop ?? DBNull.Value);
        var pHocKy = new SqlParameter("@HocKy", hocKy);

        return ExecuteStoredProcedure("sp_Admin_GetBaoCaoChuyenCan", pKhoi, pMaLop, pHocKy);
    }

    /// <summary>
    /// ADMIN: Lấy bảng điểm tổng hợp các môn theo Khối, Lớp (hoặc toàn trường) và Học kỳ.
    /// </summary>
    /// <param name="khoi">Khối cần xem, hoặc null.</param>
    /// <param name="maLop">Mã lớp cụ thể, hoặc null.</param>
    /// <param name="hocKy">1, 2, hoặc 3 (Cả năm).</param>
    // Sửa lại:
    public static DataTable GetBangDiemHocKy_Admin(string khoi, string maLop, int hocKy)
    {
        var pKhoi = new SqlParameter("@Khoi", (object)khoi ?? DBNull.Value);
        var pMaLop = new SqlParameter("@MaLop", (object)maLop ?? DBNull.Value);
        var pHocKy = new SqlParameter("@HocKy", hocKy);

        return ExecuteStoredProcedure("sp_Admin_GetBangDiemHocKy", pKhoi, pMaLop, pHocKy);
    }

    // Lưu ý: Không cần hàm GetBangDiemPivot_Admin vì SP gốc sp_GetBangDiemPivot
    // đã hoạt động dựa trên MaLop, Admin có thể dùng trực tiếp sau khi chọn lớp.

    /// <summary>
    /// ADMIN: Lấy hồ sơ học sinh theo Khối, Lớp (hoặc toàn trường).
    /// </summary>
    /// <param name="khoi">Khối cần xem, hoặc null.</param>
    /// <param name="maLop">Mã lớp cụ thể, hoặc null.</param>
    // Sửa lại:
    public static DataTable GetHoSoHocSinh_Admin(string khoi, string maLop)
    {
        var pKhoi = new SqlParameter("@Khoi", (object)khoi ?? DBNull.Value);
        var pMaLop = new SqlParameter("@MaLop", (object)maLop ?? DBNull.Value);

        return ExecuteStoredProcedure("sp_Admin_GetHoSoHocSinh", pKhoi, pMaLop);
    }

    /// <summary>
    /// ADMIN: Lấy thống kê tổng hợp theo Khối (hoặc toàn trường).
    /// </summary>
    /// <param name="khoi">Khối cần xem, hoặc null cho toàn trường.</param>
    // Sửa lại:
    public static DataTable GetThongKeKhoi_Admin(string khoi)
    {
        var pKhoi = new SqlParameter("@khoi", (object)khoi ?? DBNull.Value);
        return ExecuteStoredProcedure("sp_GetThongKeKhoi_Admin", pKhoi);
    }

    #endregion
    #endregion
    // SP MỚI ĐỂ LẤY DANH SÁCH THÁNG
    public static DataTable GetMonthlyScoreTypes(string maLop, int hocKy)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        var pHocKy = new SqlParameter("@HocKy", hocKy);
        return ExecuteStoredProcedure("sp_GetMonthlyScoreTypes", pMaLop, pHocKy);
    }

    public static DataTable GetBaoCaoThang_ThongKe(string maLop, string maMon, string loaiDiem)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        var pMaMon = new SqlParameter("@MaMon", maMon);
        var pLoaiDiem = new SqlParameter("@LoaiDiem", loaiDiem);
        return ExecuteStoredProcedure("sp_GetBaoCaoThang_ThongKe", pMaLop, pMaMon, pLoaiDiem);
    }

    // ### THÊM MỚI: Helper cho SP Báo cáo tháng của Admin ###
    public static DataTable GetBaoCaoThang_ThongKe_Admin(string khoi, string maMon, string loaiDiem)
    {
        var pKhoi = new SqlParameter("@Khoi", (object)khoi ?? DBNull.Value);
        var pMaMon = new SqlParameter("@MaMon", maMon);
        var pLoaiDiem = new SqlParameter("@LoaiDiem", loaiDiem);
        return ExecuteStoredProcedure("sp_Admin_GetBaoCaoThang_ThongKe", pKhoi, pMaMon, pLoaiDiem);
    }
}