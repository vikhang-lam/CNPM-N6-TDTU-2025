using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;      // Cần cho OtpRequestResult
using System.Net.Mail; // Cần cho OtpRequestResult
using N6.Properties;   // Để truy cập Settings.Default


public enum ResetPasswordStatus
{
    AccountNotFound = 0,
    InvalidOtp = 1,
    OtpExpired = 2,
    Success = 100
}
public enum LoginStatus
{
    Success,
    InvalidCredentials,
    AccountNotActivated
}
/// <summary>
/// Đại diện cho thông tin hồ sơ cơ bản của giáo viên.
/// </summary>
public class TeacherProfile
{
    public string Ten { get; set; }
    public string TenMon { get; set; }
    public string Email { get; set; }
    public string SDT { get; set; }
    public string AnhDaiDien { get; set; }
}

/// <summary>
/// Gói kết quả trả về từ SP sp_RequestPasswordReset
/// </summary>
public class OtpRequestResult
{
    public bool Success { get; set; }
    public string Email { get; set; }
    public string Otp { get; set; }
}

/// <summary>
/// Cung cấp các phương thức tĩnh để tương tác với CSDL SQL Server.
/// Lớp này tuân thủ quy tắc chỉ gọi Stored Procedures.
/// </summary>
public static class DatabaseHelper
{
    // Đọc chuỗi kết nối TỰ ĐỘNG từ file App.config
    private static string connectionString = Settings.Default.ConnectionString;

    #region Core Helpers

    /// <summary>
    /// (Không khuyến khích) Thực thi một câu lệnh SQL query text.
    /// </summary>
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

    /// <summary>
    /// Thực thi một Stored Procedure và trả về kết quả dưới dạng DataTable (ví dụ: SELECT).
    /// </summary>
    /// <param name="procedureName">Tên của Stored Procedure.</param>
    /// <param name="parameters">Danh sách các tham số SqlParameter.</param>
    /// <returns>Một DataTable chứa kết quả.</returns>
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

    /// <summary>
    /// Thực thi một Stored Procedure không trả về giá trị (ví dụ: INSERT, UPDATE, DELETE).
    /// </summary>
    /// <param name="procedureName">Tên của Stored Procedure.</param>
    /// <param name="parameters">Danh sách các tham số SqlParameter.</param>
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

    /// <summary>
    /// Thực thi một Stored Procedure và trả về một giá trị đơn lẻ (ví dụ: COUNT, MAX, ID).
    /// </summary>
    /// <param name="procedureName">Tên của Stored Procedure.</param>
    /// <param name="parameters">Danh sách các tham số SqlParameter.</param>
    /// <returns>Đối tượng (object) là giá trị đơn lẻ trả về.</returns>
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

    #endregion

    #region Login
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

    #region Teacher Info
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

    public static string GetSubjectByTeacher(string identifier)
    {
        var pId = new SqlParameter("@id", identifier);
        object result = ExecuteScalarStoredProcedure("sp_GetMonByTeacher", pId);
        return result?.ToString() ?? "";
    }
    #endregion

    #region Student
    public static DataTable GetStudentsByClass(string maLop)
    {
        var pMaLop = new SqlParameter("@malop", maLop);
        return ExecuteStoredProcedure("sp_GetHocSinhByLop", pMaLop);
    }

    public static DataRow GetStudentProfile(string maHS)
    {
        var pMaHS = new SqlParameter("@maHS", maHS);
        DataTable dt = ExecuteStoredProcedure("sp_GetHocSinhProfile", pMaHS);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public static void UpdateStudent(string maHS, string hoTen, string gioiTinh, DateTime? ngaySinh, string diaChi)
    {
        var pMaHS = new SqlParameter("@MaHS", maHS);
        var pHoTen = new SqlParameter("@HoTen", hoTen ?? "");
        var pGioiTinh = new SqlParameter("@GioiTinh", gioiTinh ?? "");
        var pNgaySinh = new SqlParameter("@NgaySinh", (object)ngaySinh ?? DBNull.Value);
        var pDiaChi = new SqlParameter("@DiaChi", diaChi ?? "");

        ExecuteNonQueryStoredProcedure("sp_UpdateHocSinhProfile", pMaHS, pHoTen, pGioiTinh, pNgaySinh, pDiaChi);
    }
    #endregion

    #region Attendance (Điểm danh)

    public static DataTable GetAttendanceByClass(string maLop)
    {
        var pMaLop = new SqlParameter("@maLop", maLop ?? string.Empty);
        return ExecuteStoredProcedure("sp_GetDiemDanhByLop", pMaLop);
    }

    public static void UpsertAttendance(string maHS, string maLop, DateTime ngay, string buoi, string trangThai)
    {
        if (string.IsNullOrWhiteSpace(maHS)) return;

        var pMaHS = new SqlParameter("@MaHS", maHS);
        var pNgay = new SqlParameter("@Ngay", ngay);
        var pBuoi = new SqlParameter("@Buoi", (object)buoi ?? DBNull.Value);
        var pTrangThai = new SqlParameter("@TrangThai", (object)trangThai ?? DBNull.Value);

        ExecuteNonQueryStoredProcedure("sp_UpsertDiemDanh", pMaHS, pNgay, pBuoi, pTrangThai);
    }
    public static DataTable GetAttendanceByClassAndDate(string maLop, DateTime ngay, string buoi)
    {
        var pMaLop = new SqlParameter("@maLop", maLop ?? "");
        var pNgay = new SqlParameter("@ngay", ngay.Date);
        var pBuoi = new SqlParameter("@buoi", string.IsNullOrEmpty(buoi) ? (object)DBNull.Value : buoi);

        return ExecuteStoredProcedure("sp_GetDiemDanhByLopAndDate", pMaLop, pNgay, pBuoi);
    }

    public static void ExecuteCreateDefaultAttendance(string maLop, DateTime ngay, string buoi)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        var pNgay = new SqlParameter("@Ngay", ngay.Date);
        var pBuoi = new SqlParameter("@Buoi", buoi);

        ExecuteNonQueryStoredProcedure("sp_TaoDiemDanhMacDinh", pMaLop, pNgay, pBuoi);
    }

    public static void ExecuteCreateDefaultAttendance(string maLop)
    {
        ExecuteCreateDefaultAttendance(maLop, DateTime.Today, "Sáng");
    }

    public static void SaveAttendance(string maHS, string trangThai, DateTime? ngay = null, string buoi = null)
    {
        DateTime actualDate = (ngay ?? DateTime.Now).Date;
        string actualBuoi = string.IsNullOrEmpty(buoi) ? "Sáng" : buoi;

        var pMaHS = new SqlParameter("@MaHS", maHS);
        var pNgay = new SqlParameter("@Ngay", actualDate);
        var pBuoi = new SqlParameter("@Buoi", actualBuoi);
        var pTrangThai = new SqlParameter("@TrangThai", trangThai);

        ExecuteNonQueryStoredProcedure("sp_UpsertDiemDanh", pMaHS, pNgay, pBuoi, pTrangThai);
    }

    public static void UpdateAttendance(string maDD, string trangThai)
    {
        var pMaDD = new SqlParameter("@MaDD", maDD);
        var pTrangThai = new SqlParameter("@TrangThai", trangThai);
        ExecuteNonQueryStoredProcedure("sp_UpdateDiemDanh", pMaDD, pTrangThai);
    }
    #endregion

    #region Academic Results (Kết quả học tập)
    public static void SaveAcademicResult(string maHS, string maMon, double diem, string nhanXet = "")
    {
        var pMaHS = new SqlParameter("@MaHS", maHS);
        var pMaMon = new SqlParameter("@MaMon", maMon);
        var pDiem = new SqlParameter("@Diem", diem);
        var pNhanXet = new SqlParameter("@NhanXet", nhanXet ?? "");

        ExecuteNonQueryStoredProcedure("sp_InsertKetQuaHocTap", pMaHS, pMaMon, pDiem, pNhanXet);
    }

    public static void UpdateAcademicResult(string maHS, string maMon, string loai, float? diem)
    {
        try
        {
            var pMaHS = new SqlParameter("@MaHS", maHS);
            var pMaMon = new SqlParameter("@MaMon", maMon);
            var pLoai = new SqlParameter("@Loai", loai);
            var pDiem = new SqlParameter("@Diem", (object)diem ?? DBNull.Value);
            var pNhanXet = new SqlParameter("@NhanXet", DBNull.Value);
            var pGhiChu = new SqlParameter("@GhiChu", DBNull.Value);

            ExecuteNonQueryStoredProcedure("sp_UpsertKetQuaHocTap", pMaHS, pMaMon, pLoai, pDiem, pNhanXet, pGhiChu);
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
            // Bắt lỗi RAISERROR từ SQL (lỗi 50000)
            if (ex.Number == 50000)
            {
                throw new Exception(ex.Message, ex);
            }
            else
            {
                // Ném các lỗi SQL khác
                throw;
            }
        }
        catch (Exception ex)
        {
            // Ném lại lỗi C# khác
            throw new Exception("Đã xảy ra lỗi khi lưu điểm: " + ex.Message, ex);
        }
    }

    public static void UpdateAcademicResult_Text(string maHS, string maMon, string loai, string textValue, bool isNhanXet)
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
                throw new Exception(ex.Message, ex);
            }
            else
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            // Ném lại lỗi C# khác
            throw new Exception("Đã xảy ra lỗi khi lưu nhận xét: " + ex.Message, ex);
        }
    }

    public static DataTable GetAcademicResultsByClass(string maLop)
    {
        var pMaLop = new SqlParameter("@malop", maLop);
        return ExecuteStoredProcedure("sp_GetKetQuaHocTapByLop", pMaLop);
    }

    public static DataTable GetScoreboardPivot(string maLop, int ki, string maMon)
    {
        var pMaLop = new SqlParameter("@malop", maLop);
        var pKi = new SqlParameter("@ki", ki);
        var pMaMon = new SqlParameter("@maMon", maMon);

        return ExecuteStoredProcedure("sp_GetBangDiemPivot", pMaLop, pKi, pMaMon);
    }
    #endregion

    #region Profile
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

    #region Documents (Tài liệu)
    public static DataTable GetDocumentsByTeacher(string maGV)
    {
        var pMaGV = new SqlParameter("@gv", maGV);
        return ExecuteStoredProcedure("sp_GetTaiLieuByGV", pMaGV);
    }

    public static void InsertDocument(string maGV, string ten, string moTa, string filePath, string trangThai)
    {
        var pMaGV = new SqlParameter("@gv", maGV);
        var pTen = new SqlParameter("@ten", ten);
        var pMoTa = new SqlParameter("@moTa", moTa ?? "");
        var pKieu = new SqlParameter("@kieu", filePath);
        var pTrangThai = new SqlParameter("@tt", trangThai ?? "Riêng tư");

        ExecuteNonQueryStoredProcedure("sp_InsertTaiLieu", pMaGV, pTen, pMoTa, pKieu, pTrangThai);
    }
    public static void UpdateStudentClass(string maHS, string maLopMoi)
    {
        var pMaHS = new SqlParameter("@MaHS", maHS);
        var pMaLopMoi = new SqlParameter("@MaLopMoi", (object)maLopMoi ?? DBNull.Value);
        ExecuteNonQueryStoredProcedure("sp_UpdateHocSinhLop", pMaHS, pMaLopMoi);
    }
    public static string GetAdminEmail(string maAdmin)
    {
        var pMaAdmin = new SqlParameter("@MaAdmin", maAdmin);
        object result = ExecuteScalarStoredProcedure("sp_GetAdminEmail", pMaAdmin);
        return result?.ToString();
    }
    public static int UpdateStudentClass_Multi(System.Collections.Generic.List<string> maHocSinhList, string maLopMoi)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("MaHS", typeof(string));
        foreach (string maHS in maHocSinhList)
        {
            dt.Rows.Add(maHS);
        }

        var pMaHSList = new SqlParameter("@MaHSList", SqlDbType.Structured)
        {
            TypeName = "ut_MaHSList",
            Value = dt
        };
        var pMaLopMoi = new SqlParameter("@MaLopMoi", (object)maLopMoi ?? DBNull.Value);

        object result = ExecuteScalarStoredProcedure("sp_UpdateHocSinhLop_Multi", pMaHSList, pMaLopMoi);
        return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
    }
    public static DataTable GetSharedDocuments()
    {
        return ExecuteStoredProcedure("sp_GetTaiLieuShared");
    }
    public static void DeleteDocument(string maTL)
    {
        var pMaTL = new SqlParameter("@id", maTL);
        ExecuteNonQueryStoredProcedure("sp_DeleteTaiLieu", pMaTL);
    }

    public static void ShareDocument(string maTL)
    {
        var pMaTL = new SqlParameter("@id", maTL);
        ExecuteNonQueryStoredProcedure("sp_ShareTaiLieu", pMaTL);
    }

    public static string GetTeacherIdByUsername(string username)
    {
        var pUser = new SqlParameter("@u", username);
        object result = ExecuteScalarStoredProcedure("sp_GetMaGVByUsername", pUser);
        return result?.ToString();
    }

    public static DataTable GetSharedDocumentsWithUploader()
    {
        return ExecuteStoredProcedure("sp_GetTaiLieuSharedWithUploader");
    }
    public static void UnshareDocument(string maTL)
    {
        var pMaTL = new SqlParameter("@MaTL", maTL);
        ExecuteNonQueryStoredProcedure("sp_UnshareTaiLieu", pMaTL);
    }
    #endregion

    #region Timetable (Thời khóa biểu)
    public static void DeleteTimetableEntry(string maGV, DateTime ngay, int tiet)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pNgay = new SqlParameter("@Ngay", ngay.Date);
        var pTiet = new SqlParameter("@Tiet", tiet);

        ExecuteNonQueryStoredProcedure("sp_DeleteTKBEntry", pMaGV, pNgay, pTiet);
    }
    public static DataTable GetTimetableByTeacher(string maGV, DateTime monday)
    {
        DateTime sunday = monday.AddDays(6);
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pMonday = new SqlParameter("@Monday", monday.Date);
        var pSunday = new SqlParameter("@Sunday", sunday.Date);

        return ExecuteStoredProcedure("sp_GetTKBByGV", pMaGV, pMonday, pSunday);
    }
    public static void DeleteTimetableByWeek(string maGV, DateTime monday)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pMonday = new SqlParameter("@Monday", monday.Date);

        ExecuteNonQueryStoredProcedure("sp_DeleteTKBByWeek", pMaGV, pMonday);
    }
    public static void UpdateCellColor(string maGV, DateTime ngay, int tiet, string colorHex)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pNgay = new SqlParameter("@Ngay", ngay.Date);
        var pTiet = new SqlParameter("@Tiet", tiet);
        var pColor = new SqlParameter("@MauSac", string.IsNullOrEmpty(colorHex) ? (object)DBNull.Value : colorHex);

        ExecuteNonQueryStoredProcedure("sp_UpsertTKBColor", pMaGV, pNgay, pTiet, pColor);
    }

    public static void UpsertTimetableNote(string maGV, DateTime ngay, int tiet, string note)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pNgay = new SqlParameter("@Ngay", ngay.Date);
        var pTiet = new SqlParameter("@Tiet", tiet);
        var pNote = new SqlParameter("@Note", note);

        ExecuteNonQueryStoredProcedure("sp_UpsertTKBGhiChu", pMaGV, pNgay, pTiet, pNote);
    }

    public static void DeleteTimetableNote(string maGV, DateTime ngay, int tiet)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pNgay = new SqlParameter("@Ngay", ngay.Date);
        var pTiet = new SqlParameter("@Tiet", tiet);
        ExecuteNonQueryStoredProcedure("sp_DeleteTKBGhiChu", pMaGV, pNgay, pTiet);
    }

    public static void AddTimetableNote(string maGV, string maLop, DateTime ngay, int tiet, string ghiChu)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pMaLop = new SqlParameter("@MaLop", maLop);
        var pNgay = new SqlParameter("@Ngay", ngay.Date);
        var pTiet = new SqlParameter("@Tiet", tiet);
        var pGhiChu = new SqlParameter("@GhiChu", ghiChu);

        ExecuteNonQueryStoredProcedure("sp_AddGhiChuTKB", pMaGV, pMaLop, pNgay, pTiet, pGhiChu);
    }
    #endregion

    #region MiniGames
    public static DataTable GetMiniGames()
    {
        return ExecuteStoredProcedure("sp_GetMiniGames");
    }

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

    #region Teacher Management (Admin)
    public static DataTable GetTeachersByStatus(string trangThai)
    {
        var pTT = new SqlParameter("@tt", trangThai);
        return ExecuteStoredProcedure("sp_GetGiaoVienByTrangThai", pTT);
    }

    public static Tuple<string, string> UpdateTeacherStatus(string maGV, string trangThai)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateTrangThaiGiaoVien", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", maGV);
                    cmd.Parameters.AddWithValue("@tt", trangThai);

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

    public static void UpdateTeacher(string maGV, string ten, string email, string sdt)
    {
        var pMaGV = new SqlParameter("@id", maGV);
        var pTen = new SqlParameter("@t", ten);
        var pEmail = new SqlParameter("@e", email);
        var pSdt = new SqlParameter("@s", sdt);
        ExecuteNonQueryStoredProcedure("sp_UpdateGiaoVien", pMaGV, pTen, pEmail, pSdt);
    }

    public static void DeleteTeacher(string maGV)
    {
        var pMaGV = new SqlParameter("@id", maGV);
        ExecuteNonQueryStoredProcedure("sp_DeleteGiaoVien", pMaGV);
    }
    public static DataTable GetAllTeachers()
    {
        return ExecuteStoredProcedure("sp_GetAllGiaoVien");
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
    public static string GetTeacherNameById(string maGV)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        object result = ExecuteScalarStoredProcedure("sp_GetTeacherNameById", pMaGV);
        return result?.ToString() ?? "Không rõ";
    }
    #endregion

    #region Student Management (Admin)
    public static DataTable GetAllStudents()
    {
        return ExecuteStoredProcedure("sp_GetAllHocSinh");
    }


    public static void InsertStudent(string maHS, string maLop, string hoTen, DateTime ngaySinh,
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


    public static void UpdateStudent(string maHS, string hoTen, DateTime ngaySinh,
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


    public static void DeleteStudent(string maHS)
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

    public static ImportResult ImportStudentsFromDataTable(DataTable dt)
    {
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
                if (!DateTime.TryParseExact(row["NgaySinh"]?.ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ngaySinh))
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

        DataTable resultDt = ExecuteStoredProcedure("sp_ImportHocSinh", pTVP);

        var result = new ImportResult();
        if (resultDt.Rows.Count > 0)
        {
            result.Success = (int)resultDt.Rows[0]["Success"];
            result.Skipped = (int)resultDt.Rows[0]["Skipped"];
        }
        result.Failed = dt.Rows.Count - result.Success - result.Skipped;

        return result;
    }

    public static ImportResult ImportStudentsToClass(DataTable dt, string maLopTarget)
    {
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
                if (!DateTime.TryParseExact(row["NgaySinh"]?.ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ngaySinh))
                {
                    failed++;
                    continue; 
                }
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
        result.Failed = dt.Rows.Count - result.Success - result.Skipped;

        return result;
    }

    public static ImportResult ImportTimetableForTeacher(string maGV, DataTable dt)
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
                DateTime ngay;
                int tiet;

                if (!DateTime.TryParse(row["Ngay"]?.ToString().Trim(), out ngay))
                {
                    if (!DateTime.TryParseExact(row["Ngay"]?.ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ngay))
                    {
                        failed++;
                        continue; 
                    }
                }
                ngay = ngay.Date; 

                if (!int.TryParse(row["Tiet"]?.ToString().Trim(), out tiet))
                {
                    failed++;
                    continue; 
                }


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
        else
        {
            result.Failed = failed;
        }


        return result;
    }
    #endregion

    #region Reports (Báo cáo)
    public static DataTable GetClassesByTeacher(string maGV)
    {
        var pMaGV = new SqlParameter("@maGV", maGV);
        return ExecuteStoredProcedure("sp_GetLopByGiaoVien", pMaGV);
    }

    public static DataTable GetSemesterScoreboard(string maLop, int hocKy)
    {
        var pMaLop = new SqlParameter("@maLop", maLop);
        var pHocKy = new SqlParameter("@hocKy", hocKy);
        return ExecuteStoredProcedure("sp_GetBangDiemHocKy", pMaLop, pHocKy);
    }

    public static DataTable GetStudentRecords(string maLop)
    {
        var pMaLop = new SqlParameter("@maLop", maLop);
        return ExecuteStoredProcedure("sp_GetHoSoHocSinh", pMaLop);
    }

    public static DataTable GetGradeStatistics(string khoi)
    {
        var pKhoi = new SqlParameter("@khoi", khoi);
        return ExecuteStoredProcedure("sp_GetThongKeKhoi", pKhoi);
    }

    public static DataTable GetAttendanceReport(string maLop, int hocKy)
    {
        var pMaLop = new SqlParameter("@maLop", maLop);
        var pHocKy = new SqlParameter("@hocKy", hocKy);
        return ExecuteStoredProcedure("sp_GetBaoCaoChuyenCan", pMaLop, pHocKy);
    }

    public static DataTable GetMonthlyScoreTypes(string maLop, int hocKy)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        var pHocKy = new SqlParameter("@HocKy", hocKy);
        return ExecuteStoredProcedure("sp_GetMonthlyScoreTypes", pMaLop, pHocKy);
    }

    public static DataTable GetMonthlyReport_Statistics(string maLop, string maMon, string loaiDiem)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        var pMaMon = new SqlParameter("@MaMon", maMon);
        var pLoaiDiem = new SqlParameter("@LoaiDiem", loaiDiem);
        return ExecuteStoredProcedure("sp_GetBaoCaoThang_ThongKe", pMaLop, pMaMon, pLoaiDiem);
    }
    #endregion

    #region Admin Reports (Báo cáo Admin)

    public static DataTable GetAttendanceReport_Admin(string khoi, string maLop, int hocKy)
    {
        var pKhoi = new SqlParameter("@Khoi", (object)khoi ?? DBNull.Value);
        var pMaLop = new SqlParameter("@MaLop", (object)maLop ?? DBNull.Value);
        var pHocKy = new SqlParameter("@HocKy", hocKy);

        return ExecuteStoredProcedure("sp_Admin_GetBaoCaoChuyenCan", pKhoi, pMaLop, pHocKy);
    }

    public static DataTable GetSemesterScoreboard_Admin(string khoi, string maLop, int hocKy)
    {
        var pKhoi = new SqlParameter("@Khoi", (object)khoi ?? DBNull.Value);
        var pMaLop = new SqlParameter("@MaLop", (object)maLop ?? DBNull.Value);
        var pHocKy = new SqlParameter("@HocKy", hocKy);

        return ExecuteStoredProcedure("sp_Admin_GetBangDiemHocKy", pKhoi, pMaLop, pHocKy);
    }

    public static DataTable GetStudentRecords_Admin(string khoi, string maLop)
    {
        var pKhoi = new SqlParameter("@Khoi", (object)khoi ?? DBNull.Value);
        var pMaLop = new SqlParameter("@MaLop", (object)maLop ?? DBNull.Value);

        return ExecuteStoredProcedure("sp_Admin_GetHoSoHocSinh", pKhoi, pMaLop);
    }

    public static DataTable GetGradeStatistics_Admin(string khoi)
    {
        var pKhoi = new SqlParameter("@khoi", (object)khoi ?? DBNull.Value);
        return ExecuteStoredProcedure("sp_GetThongKeKhoi_Admin", pKhoi);
    }

    public static DataTable GetMonthlyReport_Statistics_Admin(string khoi, string maMon, string loaiDiem)
    {
        var pKhoi = new SqlParameter("@Khoi", (object)khoi ?? DBNull.Value);
        var pMaMon = new SqlParameter("@MaMon", maMon);
        var pLoaiDiem = new SqlParameter("@LoaiDiem", loaiDiem);
        return ExecuteStoredProcedure("sp_Admin_GetBaoCaoThang_ThongKe", pKhoi, pMaMon, pLoaiDiem);
    }
    #endregion

    #region Teaching Support (Hỗ trợ giảng dạy)

    public static void AddNoteForStudent(string maHS, string maMon, string ghiChu)
    {
        var pMaHS = new SqlParameter("@MaHS", maHS);
        var pMaMon = new SqlParameter("@MaMon", maMon);
        var pGhiChu = new SqlParameter("@GhiChu", ghiChu);

        ExecuteNonQueryStoredProcedure("sp_AddGhiChuChoHocSinh", pMaHS, pMaMon, pGhiChu);
    }
    public static DataTable GetAllAcademicResults()
    {
        return ExecuteStoredProcedure("sp_GetAllKetQuaHocTap");
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
        return ExecuteStoredProcedure("sp_GetHomeroomGradebook",
            new SqlParameter("@MaLop", maLop),
            new SqlParameter("@LoaiDiem", loaiDiem)
        );
    }

    public static DataTable GetSubjectsByTeacherAndClass(string maGV, string maLop)
    {
        var pMaGV = new SqlParameter("@maGV", maGV);
        var pMaLop = new SqlParameter("@maLop", maLop);
        return ExecuteStoredProcedure("sp_GetMonHocByGiaoVienAndLop", pMaGV, pMaLop);
    }
    #endregion

    #region Class Management (Quản lý Lớp)
    public static DataTable GetAllClasses()
    {
        return ExecuteStoredProcedure("sp_GetAllLopHoc");
    }

    public static DataTable GetUnassignedHomeroomTeachers()
    {
        return ExecuteStoredProcedure("sp_GetUnassignedHomeroomTeachers");
    }

    public static void UpdateHomeroomTeacherForClass(string maLop, string maGV)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        var pMaGV = new SqlParameter("@MaGV", string.IsNullOrEmpty(maGV) ? (object)DBNull.Value : maGV);
        ExecuteNonQueryStoredProcedure("sp_UpdateGvcnForLop", pMaLop, pMaGV);
    }

    public static DataRow GetClassDetails(string maLop)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        DataTable dt = ExecuteStoredProcedure("sp_GetLopHocDetails", pMaLop);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public static DataTable GetTeachingAssignmentsByClass(string maLop)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        return ExecuteStoredProcedure("sp_GetPhanCongGiangDayByLop", pMaLop);
    }
    public static DataTable GetTeacherAssignments(string maGV)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        return ExecuteStoredProcedure("sp_GetTeacherAssignments", pMaGV);
    }

    public static void UpdateTeachingAssignment(string maLop, string maMon, string newMaGV)
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
            throw new Exception("Lỗi khi cập nhật phân công: " + ex.Message, ex);
        }
    }
    #endregion

    #region Class Fund (Quỹ Lớp)
    public static DataTable GetClassFundByClass(string maLop)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        return ExecuteStoredProcedure("sp_GetQuyLopByLop", pMaLop);
    }

    public static void InsertClassFundEntry(string maLop, string loai, decimal soTien, DateTime ngay, string ghiChu)
    {
        var pMaLop = new SqlParameter("@MaLop", maLop);
        var pLoai = new SqlParameter("@Loai", loai);
        var pSoTien = new SqlParameter("@SoTien", soTien);
        var pNgay = new SqlParameter("@Ngay", ngay.Date);
        var pGhiChu = new SqlParameter("@GhiChu", (object)ghiChu ?? DBNull.Value);

        ExecuteNonQueryStoredProcedure("sp_InsertQuyLop", pMaLop, pLoai, pSoTien, pNgay, pGhiChu);
    }

    public static void DeleteClassFundEntry(string maQL)
    {
        var pMaQL = new SqlParameter("@MaQL", maQL);
        ExecuteNonQueryStoredProcedure("sp_DeleteQuyLop", pMaQL);
    }
    #endregion

    #region Subject Management (Quản lý Môn học)
    public static DataTable GetAllSubjects()
    {
        return ExecuteStoredProcedure("sp_GetAllMonHoc");
    }

    public static DataTable GetSubjectsByTeacher(string maGV)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        return ExecuteStoredProcedure("sp_GetMonHocByGiaoVien", pMaGV);
    }
    public static void UpdateTeacher_Subjects(string maGV, DataTable dtMaMon)
    {
        var pMaGV = new SqlParameter("@MaGV", maGV);
        var pMonHocList = new SqlParameter("@MonHocList", SqlDbType.Structured)
        {
            TypeName = "ut_MaMonList",
            Value = dtMaMon
        };
        ExecuteNonQueryStoredProcedure("sp_UpdateGiaoVien_MonHoc", pMaGV, pMonHocList);
    }

    public static void InsertSubject(string tenMon)
    {
        try
        {
            var pTenMon = new SqlParameter("@TenMon", tenMon);
            ExecuteNonQueryStoredProcedure("sp_InsertMonHoc", pTenMon);
        }
        catch (SqlException ex)
        {
            if (ex.Number == 50000) throw new Exception(ex.Message);
            throw;
        }
    }

    public static void UpdateSubject(string maMon, string tenMon)
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

    public static void DeleteSubject(string maMon)
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
    #endregion

    #region School Management (Quản lý Trường học)

    public static DataTable GetScoreDeadlines()
    {
        return ExecuteStoredProcedure("sp_GetThoiHanDiem");
    }

    public static void UpdateScoreDeadline(string maCotDiem, string khoi, int hocKy, DateTime ngayMo, DateTime ngayKhoa, bool khoaThuCong)
    {
        var pMaCotDiem = new SqlParameter("@MaCotDiem", maCotDiem);
        var pKhoi = new SqlParameter("@Khoi", khoi);
        var pHocKy = new SqlParameter("@HocKy", hocKy);
        var pNgayMo = new SqlParameter("@NgayMoDiem", ngayMo);
        var pNgayKhoa = new SqlParameter("@NgayKhoaDiem", ngayKhoa);
        var pKhoaThuCong = new SqlParameter("@KhoaThuCong", khoaThuCong);

        ExecuteNonQueryStoredProcedure("sp_UpdateThoiHanDiem", pMaCotDiem, pKhoi, pHocKy, pNgayMo, pNgayKhoa, pKhoaThuCong);
    }

    public static DataTable ProcessStudentPromotion(string maLopCu, string maLopMoi_LenLop, string maLopMoi_OLaiLop, bool isLop5)
    {
        var pMaLopCu = new SqlParameter("@MaLopCu", maLopCu);

        var pMaLopMoi_LenLop = new SqlParameter("@MaLopMoi_LenLop",
            string.IsNullOrEmpty(maLopMoi_LenLop) ? (object)DBNull.Value : maLopMoi_LenLop);

        var pMaLopMoi_OLaiLop = new SqlParameter("@MaLopMoi_OLaiLop",
            string.IsNullOrEmpty(maLopMoi_OLaiLop) ? (object)DBNull.Value : maLopMoi_OLaiLop);

        var pIsLop5 = new SqlParameter("@IsLop5_TotNghiep", isLop5);

        return ExecuteStoredProcedure("sp_ProcessStudentPromotion",
            pMaLopCu, pMaLopMoi_LenLop, pMaLopMoi_OLaiLop, pIsLop5);
    }
    #endregion

    #region Global Events
    public static event EventHandler TimetableChanged;

    public static void RaiseTimetableChanged()
    {
        TimetableChanged?.Invoke(null, EventArgs.Empty);
    }
    #endregion

    #region Password Reset
    public static OtpRequestResult RequestPasswordReset(string usernameOrEmail)
    {
        var pUser = new SqlParameter("@UsernameOrEmail", usernameOrEmail);
        DataTable dt = ExecuteStoredProcedure("sp_RequestPasswordReset", pUser);

        if (dt.Rows.Count > 0)
        {
            var row = dt.Rows[0];
            string email = row["Email"]?.ToString();
            string otp = row["OTP"]?.ToString();

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(otp))
            {
                return new OtpRequestResult { Success = true, Email = email, Otp = otp };
            }
        }
        return new OtpRequestResult { Success = false };
    }

    public static ResetPasswordStatus ResetPasswordWithOtp(string usernameOrEmail, string otp, string newPassword)
    {
        var pUser = new SqlParameter("@UsernameOrEmail", usernameOrEmail);
        var pOtp = new SqlParameter("@OTP", otp);
        var pNewPass = new SqlParameter("@NewPassword", newPassword);

        object result = ExecuteScalarStoredProcedure("sp_ResetPasswordWithOtp", pUser, pOtp, pNewPass);

        if (result != null && result != DBNull.Value)
        {
            return (ResetPasswordStatus)Convert.ToInt32(result);
        }
        return ResetPasswordStatus.AccountNotFound;
    }

    #endregion
}