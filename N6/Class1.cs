using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;

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

    #region Đăng nhập
    public static bool CheckTeacherLogin(string username, string password)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT COUNT(*) FROM GiaoVien WHERE Username=@user AND Password=@pass";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", password);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }

    public static bool CheckAdminLogin(string username, string password)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT COUNT(*) FROM Admin WHERE Username=@user AND Password=@pass";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", password);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
    #endregion

    #region Thông tin giáo viên
    public static TeacherProfile GetTeacherProfile(string username)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"
            SELECT gv.Ten, gv.Email, gv.SDT, gv.AnhDaiDien, mh.TenMon
            FROM GiaoVien gv
            LEFT JOIN MonHoc mh ON gv.MaMon = mh.MaMon
            WHERE gv.Username = @user OR gv.Ten = @user";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@user", username);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        return new TeacherProfile
                        {
                            Ten = r["Ten"]?.ToString() ?? "",
                            TenMon = r["TenMon"]?.ToString() ?? "Chưa có môn",
                            Email = r["Email"]?.ToString() ?? "",
                            SDT = r["SDT"]?.ToString() ?? "",
                            AnhDaiDien = r["AnhDaiDien"]?.ToString()
                        };
                    }
                }
            }
        }
        return null;
    }
    public static void UpdateTeacherAvatar(string username, string avatarPath)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "UPDATE GiaoVien SET AnhDaiDien = @avatar WHERE Username = @user OR Ten = @user";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@avatar", (object)avatarPath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@user", username);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // =======================================================================================
    // ====> HÀM BỊ XÓA: Hàm này không còn đúng với logic mới (GV dạy nhiều lớp)
    // =======================================================================================
    /*
    public static string GetLopByTeacher(string identifier)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            // Câu truy vấn này sai vì GiaoVien không còn cột MaLop
            string sql = @"SELECT MaLop FROM GiaoVien WHERE Username=@id OR Ten=@id";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", identifier);
                object result = cmd.ExecuteScalar();
                return result?.ToString() ?? "";
            }
        }
    }
    */

    public static string GetMonByTeacher(string identifier)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"SELECT MaMon FROM GiaoVien WHERE Username=@id OR Ten=@id";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", identifier);
                object result = cmd.ExecuteScalar();
                return result?.ToString() ?? "";
            }
        }
    }
    #endregion

    #region Học sinh
    public static DataTable GetHocSinhByLop(string maLop)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT MaHS, HoTen, GioiTinh, NgaySinh, DiaChi FROM HocSinh WHERE MaLop=@malop";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            da.SelectCommand.Parameters.AddWithValue("@malop", maLop);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    public static DataRow GetHocSinhProfile(string maHS)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT * FROM HocSinh WHERE MaHS=@maHS";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@maHS", maHS);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }
    }

    public static void UpdateHocSinh(string maHS, string hoTen, string gioiTinh, DateTime? ngaySinh, string diaChi)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"UPDATE HocSinh SET HoTen=@HoTen, GioiTinh=@GioiTinh, NgaySinh=@NgaySinh, DiaChi=@DiaChi
                           WHERE MaHS=@MaHS";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaHS", maHS);
                cmd.Parameters.AddWithValue("@HoTen", hoTen ?? "");
                cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh ?? "");
                cmd.Parameters.AddWithValue("@NgaySinh", (object)ngaySinh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DiaChi", diaChi ?? "");
                cmd.ExecuteNonQuery();
            }
        }
    }
    #endregion

    #region Điểm danh

    public static DataTable GetDiemDanhByLop(string maLop)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"
        SELECT 
            dd.MaDD,
            hs.MaHS,
            hs.HoTen,
            dd.NgayDD,
            dd.Buoi,
            dd.TrangThai
        FROM HocSinh hs
        LEFT JOIN DiemDanh dd ON hs.MaHS = dd.MaHS
        WHERE hs.MaLop = @maLop
        ORDER BY hs.HoTen, dd.NgayDD";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@maLop", maLop ?? string.Empty);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    public static void UpsertDiemDanh(string maHS, string maLop, DateTime ngay, string buoi, string trangThai)
    {
        if (string.IsNullOrWhiteSpace(maHS)) return;
        if (string.IsNullOrWhiteSpace(buoi)) buoi = "Sáng";
        trangThai = string.IsNullOrWhiteSpace(trangThai) ? "Có mặt" : trangThai;

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            string check = @"SELECT MaDD 
                         FROM DiemDanh 
                         WHERE MaHS=@MaHS AND CAST(NgayDD AS DATE)=@Ngay AND Buoi=@Buoi";
            object existingId = null;
            using (SqlCommand cmd = new SqlCommand(check, conn))
            {
                cmd.Parameters.AddWithValue("@MaHS", maHS);
                cmd.Parameters.Add("@Ngay", SqlDbType.Date).Value = ngay.Date;
                cmd.Parameters.AddWithValue("@Buoi", buoi);
                existingId = cmd.ExecuteScalar();
            }

            if (existingId != null)
            {
                string update = "UPDATE DiemDanh SET TrangThai=@TrangThai WHERE MaDD=@MaDD";
                using (SqlCommand up = new SqlCommand(update, conn))
                {
                    up.Parameters.AddWithValue("@TrangThai", trangThai);
                    up.Parameters.AddWithValue("@MaDD", existingId.ToString());
                    up.ExecuteNonQuery();
                }
            }
            else
            {
                string insert = @"INSERT INTO DiemDanh(MaDD, MaHS, NgayDD, Buoi, TrangThai)
                              VALUES(@MaDD, @MaHS, @NgayDD, @Buoi, @TrangThai)";
                using (SqlCommand ins = new SqlCommand(insert, conn))
                {
                    ins.Parameters.AddWithValue("@MaDD", Guid.NewGuid().ToString().Substring(0, 8));
                    ins.Parameters.AddWithValue("@MaHS", maHS);
                    ins.Parameters.Add("@NgayDD", SqlDbType.DateTime).Value = ngay.Date;
                    ins.Parameters.AddWithValue("@Buoi", buoi);
                    ins.Parameters.AddWithValue("@TrangThai", trangThai);
                    ins.ExecuteNonQuery();
                }
            }
        }
    }
    public static DataTable GetDiemDanhByLopAndDate(string maLop, DateTime ngay, string buoi)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"
                SELECT
                    hs.MaHS,
                    hs.HoTen,
                    dd.MaDD,
                    dd.NgayDD,
                    dd.Buoi,
                    dd.TrangThai
                FROM HocSinh hs
                LEFT JOIN (
                    SELECT MaDD, MaHS, NgayDD, Buoi, TrangThai
                    FROM DiemDanh
                    WHERE CAST(NgayDD AS date) = @ngay
                    AND (@buoi IS NULL OR Buoi = @buoi)
                ) dd ON hs.MaHS = dd.MaHS
                WHERE hs.MaLop = @maLop
                ORDER BY hs.HoTen";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@maLop", SqlDbType.VarChar).Value = maLop ?? "";
                cmd.Parameters.Add("@ngay", SqlDbType.Date).Value = ngay.Date;
                if (string.IsNullOrEmpty(buoi))
                    cmd.Parameters.Add("@buoi", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@buoi", SqlDbType.NVarChar).Value = buoi;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }

    public static void ExecTaoDiemDanhMacDinh(string maLop, DateTime ngay, string buoi)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            string getHs = "SELECT MaHS FROM HocSinh WHERE MaLop = @maLop";
            List<string> listHs = new List<string>();
            using (SqlCommand cmd = new SqlCommand(getHs, conn))
            {
                cmd.Parameters.AddWithValue("@maLop", maLop ?? "");
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        listHs.Add(r["MaHS"].ToString());
                    }
                }
            }

            string checkSql = @"SELECT COUNT(*) FROM DiemDanh WHERE MaHS=@MaHS AND CAST(NgayDD AS date)=@ngay AND (@buoi IS NULL OR Buoi=@buoi)";
            string insertSql = @"INSERT INTO DiemDanh(MaDD, MaHS, NgayDD, Buoi, TrangThai)
                                 VALUES(@MaDD, @MaHS, @NgayDD, @Buoi, @TrangThai)";

            foreach (var maHS in listHs)
            {
                using (SqlCommand chk = new SqlCommand(checkSql, conn))
                {
                    chk.Parameters.AddWithValue("@MaHS", maHS);
                    chk.Parameters.Add("@ngay", SqlDbType.Date).Value = ngay.Date;
                    if (string.IsNullOrEmpty(buoi))
                        chk.Parameters.Add("@buoi", SqlDbType.NVarChar).Value = DBNull.Value;
                    else
                        chk.Parameters.Add("@buoi", SqlDbType.NVarChar).Value = buoi;

                    int cnt = Convert.ToInt32(chk.ExecuteScalar());
                    if (cnt == 0)
                    {
                        using (SqlCommand ins = new SqlCommand(insertSql, conn))
                        {
                            ins.Parameters.AddWithValue("@MaDD", Guid.NewGuid().ToString().Substring(0, 8));
                            ins.Parameters.AddWithValue("@MaHS", maHS);
                            ins.Parameters.Add("@NgayDD", SqlDbType.DateTime).Value = ngay;
                            if (string.IsNullOrEmpty(buoi))
                                ins.Parameters.Add("@Buoi", SqlDbType.NVarChar).Value = "Sáng";
                            else
                                ins.Parameters.Add("@Buoi", SqlDbType.NVarChar).Value = buoi;
                            ins.Parameters.AddWithValue("@TrangThai", "Có mặt");
                            ins.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }

    public static void ExecTaoDiemDanhMacDinh(string maLop)
    {
        ExecTaoDiemDanhMacDinh(maLop, DateTime.Today, "Sáng");
    }

    public static void LuuDiemDanh(string maHS, string trangThai, DateTime? ngay = null, string buoi = null)
    {
        DateTime actualDate = (ngay ?? DateTime.Now).Date;
        string actualBuoi = string.IsNullOrEmpty(buoi) ? "Sáng" : buoi;

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string check = @"SELECT MaDD FROM DiemDanh WHERE MaHS=@MaHS AND CAST(NgayDD AS date)=@ngay AND Buoi=@buoi";
            using (SqlCommand cmdCheck = new SqlCommand(check, conn))
            {
                cmdCheck.Parameters.AddWithValue("@MaHS", maHS);
                cmdCheck.Parameters.Add("@ngay", SqlDbType.Date).Value = actualDate;
                cmdCheck.Parameters.AddWithValue("@buoi", actualBuoi);

                object existing = cmdCheck.ExecuteScalar();
                if (existing != null)
                {
                    string existingId = existing.ToString();
                    string update = "UPDATE DiemDanh SET TrangThai=@TrangThai WHERE MaDD=@MaDD";
                    using (SqlCommand cmdUp = new SqlCommand(update, conn))
                    {
                        cmdUp.Parameters.AddWithValue("@TrangThai", trangThai);
                        cmdUp.Parameters.AddWithValue("@MaDD", existingId);
                        cmdUp.ExecuteNonQuery();
                    }
                }
                else
                {
                    string insert = @"INSERT INTO DiemDanh(MaDD, MaHS, NgayDD, Buoi, TrangThai) 
                                      VALUES(@MaDD, @MaHS, @NgayDD, @Buoi, @TrangThai)";
                    using (SqlCommand cmdIn = new SqlCommand(insert, conn))
                    {
                        cmdIn.Parameters.AddWithValue("@MaDD", Guid.NewGuid().ToString().Substring(0, 8));
                        cmdIn.Parameters.AddWithValue("@MaHS", maHS);
                        cmdIn.Parameters.Add("@NgayDD", SqlDbType.DateTime).Value = actualDate;
                        cmdIn.Parameters.AddWithValue("@Buoi", actualBuoi);
                        cmdIn.Parameters.AddWithValue("@TrangThai", trangThai);
                        cmdIn.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    public static void UpdateDiemDanh(string maDD, string trangThai)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "UPDATE DiemDanh SET TrangThai=@TrangThai WHERE MaDD=@MaDD";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@MaDD", maDD);
                cmd.ExecuteNonQuery();
            }
        }
    }
    #endregion

    #region Kết quả học tập
    public static void LuuKetQuaHocTap(string maHS, string maMon, double diem, string nhanXet = "")
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"INSERT INTO KetQuaHocTap(MaKQ, MaMon, MaHS, NgayNhap, Diem, NhanXet) 
                           VALUES(@MaKQ, @MaMon, @MaHS, GETDATE(), @Diem, @NhanXet)";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaKQ", Guid.NewGuid().ToString().Substring(0, 8));
                cmd.Parameters.AddWithValue("@MaMon", maMon);
                cmd.Parameters.AddWithValue("@MaHS", maHS);
                cmd.Parameters.AddWithValue("@Diem", diem);
                cmd.Parameters.AddWithValue("@NhanXet", nhanXet ?? "");
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static void UpdateKetQuaHocTap(string maHS, string maMon, string loai, float? diem)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string check = "SELECT COUNT(*) FROM KetQuaHocTap WHERE MaHS=@MaHS AND MaMon=@MaMon AND Loai=@Loai";
            using (SqlCommand cmdCheck = new SqlCommand(check, conn))
            {
                cmdCheck.Parameters.AddWithValue("@MaHS", maHS);
                cmdCheck.Parameters.AddWithValue("@MaMon", maMon);
                cmdCheck.Parameters.AddWithValue("@Loai", loai);

                int count = (int)cmdCheck.ExecuteScalar();
                if (count > 0)
                {
                    string update = "UPDATE KetQuaHocTap SET Diem=@Diem WHERE MaHS=@MaHS AND MaMon=@MaMon AND Loai=@Loai";
                    using (SqlCommand cmdUp = new SqlCommand(update, conn))
                    {
                        cmdUp.Parameters.AddWithValue("@Diem", (object)diem ?? DBNull.Value);
                        cmdUp.Parameters.AddWithValue("@MaHS", maHS);
                        cmdUp.Parameters.AddWithValue("@MaMon", maMon);
                        cmdUp.Parameters.AddWithValue("@Loai", loai);
                        cmdUp.ExecuteNonQuery();
                    }
                }
                else
                {
                    string insert = "INSERT INTO KetQuaHocTap(MaKQ, MaHS, MaMon, Loai, Diem) VALUES(@MaKQ, @MaHS, @MaMon, @Loai, @Diem)";
                    using (SqlCommand cmdIn = new SqlCommand(insert, conn))
                    {
                        cmdIn.Parameters.AddWithValue("@MaKQ", Guid.NewGuid().ToString().Substring(0, 8));
                        cmdIn.Parameters.AddWithValue("@MaHS", maHS);
                        cmdIn.Parameters.AddWithValue("@MaMon", maMon);
                        cmdIn.Parameters.AddWithValue("@Loai", loai);
                        cmdIn.Parameters.AddWithValue("@Diem", (object)diem ?? DBNull.Value);
                        cmdIn.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    public static DataTable GetKetQuaHocTapByLop(string maLop)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"
                SELECT hs.MaHS, hs.HoTen, mh.TenMon, kq.Diem, kq.NhanXet, kq.GhiChu, kq.Loai
                FROM HocSinh hs
                INNER JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS
                INNER JOIN MonHoc mh ON kq.MaMon = mh.MaMon
                WHERE hs.MaLop = @malop
                ORDER BY hs.MaHS, mh.MaMon";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@malop", maLop);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    public static DataTable GetBangDiemPivot(string maLop, int ki, string maMon)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "";
            if (ki == 1)
            {
                sql = @"
                    SELECT 
                        hs.MaHS, hs.HoTen,
                        MAX(CASE WHEN kq.Loai = 'Thang1_Ki1' THEN kq.Diem END) AS Thang1,
                        MAX(CASE WHEN kq.Loai = 'Thang2_Ki1' THEN kq.Diem END) AS Thang2,
                        MAX(CASE WHEN kq.Loai = 'Thang3_Ki1' THEN kq.Diem END) AS Thang3,
                        MAX(CASE WHEN kq.Loai = 'GiuaKi1' THEN kq.Diem END) AS GiuaKi,
                        MAX(CASE WHEN kq.Loai = 'CuoiKi1' THEN kq.Diem END) AS CuoiKi,
                        MAX(CASE WHEN kq.Loai LIKE '%Ki1%' THEN kq.NhanXet END) AS NhanXet,
                        MAX(CASE WHEN kq.Loai LIKE '%Ki1%' THEN kq.GhiChu END) AS GhiChu
                    FROM HocSinh hs
                    LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS AND kq.MaMon = @maMon
                    WHERE hs.MaLop = @malop
                    GROUP BY hs.MaHS, hs.HoTen
                    ORDER BY hs.HoTen";
            }
            else
            {
                sql = @"
                    SELECT 
                        hs.MaHS, hs.HoTen,
                        MAX(CASE WHEN kq.Loai = 'Thang1_Ki2' THEN kq.Diem END) AS Thang1,
                        MAX(CASE WHEN kq.Loai = 'Thang2_Ki2' THEN kq.Diem END) AS Thang2,
                        MAX(CASE WHEN kq.Loai = 'Thang3_Ki2' THEN kq.Diem END) AS Thang3,
                        MAX(CASE WHEN kq.Loai = 'GiuaKi2' THEN kq.Diem END) AS GiuaKi,
                        MAX(CASE WHEN kq.Loai = 'CuoiKi2' THEN kq.Diem END) AS CuoiKi,
                        MAX(CASE WHEN kq.Loai LIKE '%Ki2%' THEN kq.NhanXet END) AS NhanXet,
                        MAX(CASE WHEN kq.Loai LIKE '%Ki2%' THEN kq.GhiChu END) AS GhiChu
                    FROM HocSinh hs
                    LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS AND kq.MaMon = @maMon
                    WHERE hs.MaLop = @malop
                    GROUP BY hs.MaHS, hs.HoTen
                    ORDER BY hs.HoTen";
            }
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@malop", maLop);
            da.SelectCommand.Parameters.AddWithValue("@maMon", maMon);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
    #endregion

    #region Hồ sơ cá nhân (Profile)
    public static void UpdateTeacherProfile(string username, string email, string phone, string avatarPath)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"UPDATE GiaoVien 
                           SET Email=@e, SDT=@s, AnhDaiDien=@a
                           WHERE Username=@u";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@e", email ?? "");
                cmd.Parameters.AddWithValue("@s", phone ?? "");
                cmd.Parameters.AddWithValue("@a", (object)avatarPath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static bool ChangeTeacherPassword(string username, string oldPass, string newPass)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string check = "SELECT Password FROM GiaoVien WHERE Username=@u OR Ten =@u";
            string currentPass = null;

            using (SqlCommand cmd = new SqlCommand(check, conn))
            {
                cmd.Parameters.Add("@u", SqlDbType.VarChar).Value = username;
                var result = cmd.ExecuteScalar();
                if (result != null)
                    currentPass = result.ToString();
            }

            if (currentPass == null || currentPass != oldPass)
            {
                return false;
            }
            if (System.Text.RegularExpressions.Regex.IsMatch(newPass, @"[^a-zA-Z0-9]"))
            {
                throw new ArgumentException("Mật khẩu không được chứa ký tự đặc biệt!");
            }
            string update = "UPDATE GiaoVien SET Password=@new WHERE Username=@u OR Ten =@u";
            using (SqlCommand cmd = new SqlCommand(update, conn))
            {
                cmd.Parameters.Add("@new", SqlDbType.VarChar).Value = newPass;
                cmd.Parameters.Add("@u", SqlDbType.VarChar).Value = username;
                cmd.ExecuteNonQuery();
                return true;
            }
        }
    }
    public static DataRow GetAdminProfile(string username)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT * FROM Admin WHERE Username=@u";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }
    }
    public static void UpdateAdminEmail(string username, string email)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "UPDATE Admin SET Email=@e WHERE Username=@u OR Ten =@admin";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@e", email);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.ExecuteNonQuery();
            }
        }
    }
    public static bool ChangeAdminPassword(string username, string oldPass, string newPass)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string check = "SELECT Password FROM Admin WHERE Username=@u";
            string currentPass = null;
            using (SqlCommand cmd = new SqlCommand(check, conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                var result = cmd.ExecuteScalar();
                if (result != null) currentPass = result.ToString();
            }

            if (currentPass == null || currentPass != oldPass)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(newPass))
            {
                throw new ArgumentException("Mật khẩu mới không được để trống!");
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(newPass, @"[^a-zA-Z0-9]"))
            {
                throw new ArgumentException("Mật khẩu mới không được chứa ký tự đặc biệt!");
            }
            string update = "UPDATE Admin SET Password=@new WHERE Username=@u";
            using (SqlCommand cmd = new SqlCommand(update, conn))
            {
                cmd.Parameters.AddWithValue("@new", newPass);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.ExecuteNonQuery();
                return true;
            }
        }
    }

    #endregion
    #region Tài liệu
    public static DataTable GetTaiLieuByGV(string maGV)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = "SELECT MaTL, TenTL, MoTa, Kieu, NgayTaiLen, TrangThaiChiaSe FROM TaiLieu WHERE MaGV=@gv";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@gv", maGV);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    public static void InsertTaiLieu(string maGV, string ten, string moTa, string filePath, string trangThai)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"INSERT INTO TaiLieu (MaTL, TenTL, MoTa, Kieu, NgayTaiLen, TrangThaiChiaSe, MaGV)
                       VALUES(@id, @ten, @moTa, @kieu, GETDATE(), @tt, @gv)";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString("N").Substring(0, 10));
                cmd.Parameters.AddWithValue("@ten", ten);
                cmd.Parameters.AddWithValue("@moTa", moTa ?? "");
                cmd.Parameters.AddWithValue("@kieu", filePath);
                cmd.Parameters.AddWithValue("@tt", trangThai ?? "Riêng tư");
                cmd.Parameters.AddWithValue("@gv", maGV);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static DataTable GetTaiLieuShared()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = "SELECT MaTL, TenTL, MoTa, Kieu, NgayTaiLen, TrangThaiChiaSe FROM TaiLieu WHERE TrangThaiChiaSe=N'Chia sẻ'";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
    public static void DeleteTaiLieu(string maTL)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "DELETE FROM TaiLieu WHERE MaTL=@id";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", maTL);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static void ShareTaiLieu(string maTL)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "UPDATE TaiLieu SET TrangThaiChiaSe=N'Chia sẻ' WHERE MaTL=@id";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", maTL);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static string GetMaGVByUsername(string username)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT MaGV FROM GiaoVien WHERE Username=@u OR Ten=@u";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                object result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }
    }
    #endregion
    #region Thời khóa biểu
    public static DataTable GetTKBByGV(string maGV, DateTime monday)
    {
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            DateTime sunday = monday.AddDays(6);
            string sql = @"
                SELECT 
                    t.Ngay, 
                    t.Tiet, 
                    ISNULL(m.TenMon, '') AS TenMon,
                    ISNULL(l.TenLop, '') AS TenLop,
                    ISNULL(t.GhiChu, '') AS GhiChu,
                    ISNULL(t.MauSac, '') AS MauSac
                FROM 
                    ThoiKhoaBieu t
                LEFT JOIN 
                    MonHoc m ON t.MaMon = m.MaMon
                LEFT JOIN 
                    LopHoc l ON t.MaLop = l.MaLop
                WHERE 
                    t.MaGV = @MaGV 
                    AND t.Ngay >= @Monday 
                    AND t.Ngay <= @Sunday";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaGV", maGV);
                cmd.Parameters.AddWithValue("@Monday", monday.Date);
                cmd.Parameters.AddWithValue("@Sunday", sunday.Date);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }
        return dt;
    }

    public static void UpdateCellColor(string maGV, DateTime ngay, int tiet, string colorHex)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string checkSql = "SELECT MaTKB FROM ThoiKhoaBieu WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet";
            object maTKB = null;
            using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
            {
                checkCmd.Parameters.AddWithValue("@MaGV", maGV);
                checkCmd.Parameters.AddWithValue("@Ngay", ngay.Date);
                checkCmd.Parameters.AddWithValue("@Tiet", tiet);
                maTKB = checkCmd.ExecuteScalar();
            }

            if (maTKB != null)
            {
                string updateSql = "UPDATE ThoiKhoaBieu SET MauSac=@MauSac WHERE MaTKB=@MaTKB";
                using (SqlCommand updateCmd = new SqlCommand(updateSql, conn))
                {
                    updateCmd.Parameters.AddWithValue("@MauSac", string.IsNullOrEmpty(colorHex) ? (object)DBNull.Value : colorHex);
                    updateCmd.Parameters.AddWithValue("@MaTKB", maTKB);
                    updateCmd.ExecuteNonQuery();
                }
            }
            else
            {
                string insertSql = "INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, MauSac, MaGV) VALUES (@MaTKB, @Ngay, @Tiet, @MauSac, @MaGV)";
                using (SqlCommand insertCmd = new SqlCommand(insertSql, conn))
                {
                    insertCmd.Parameters.AddWithValue("@MaTKB", "TKB" + Guid.NewGuid().ToString("N").Substring(0, 7));
                    insertCmd.Parameters.AddWithValue("@Ngay", ngay.Date);
                    insertCmd.Parameters.AddWithValue("@Tiet", tiet);
                    insertCmd.Parameters.AddWithValue("@MauSac", string.IsNullOrEmpty(colorHex) ? (object)DBNull.Value : colorHex);
                    insertCmd.Parameters.AddWithValue("@MaGV", maGV);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }
    }

    public static void UpsertGhiChuTKB(string maGV, DateTime ngay, int tiet, string note)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string checkSql = "SELECT MaTKB FROM ThoiKhoaBieu WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet";
            object maTKB = null;
            using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
            {
                checkCmd.Parameters.AddWithValue("@MaGV", maGV);
                checkCmd.Parameters.AddWithValue("@Ngay", ngay.Date);
                checkCmd.Parameters.AddWithValue("@Tiet", tiet);
                maTKB = checkCmd.ExecuteScalar();
            }
            if (maTKB != null)
            {
                string updateSql = "UPDATE ThoiKhoaBieu SET GhiChu=@Note WHERE MaTKB=@MaTKB";
                using (SqlCommand updateCmd = new SqlCommand(updateSql, conn))
                {
                    updateCmd.Parameters.AddWithValue("@Note", note);
                    updateCmd.Parameters.AddWithValue("@MaTKB", maTKB);
                    updateCmd.ExecuteNonQuery();
                }
            }
            else
            {
                string insertSql = "INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, GhiChu, MaGV) VALUES (@MaTKB, @Ngay, @Tiet, @GhiChu, @MaGV)";
                using (SqlCommand insertCmd = new SqlCommand(insertSql, conn))
                {
                    insertCmd.Parameters.AddWithValue("@MaTKB", "TKB" + Guid.NewGuid().ToString("N").Substring(0, 7));
                    insertCmd.Parameters.AddWithValue("@Ngay", ngay.Date);
                    insertCmd.Parameters.AddWithValue("@Tiet", tiet);
                    insertCmd.Parameters.AddWithValue("@GhiChu", note);
                    insertCmd.Parameters.AddWithValue("@MaGV", maGV);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }
    }

    #endregion
    public static DataTable GetMiniGames()
    {
        DataTable dt = new DataTable();
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT MaMNG, Ten, DuLieu FROM Minigame ORDER BY MaMNG";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.Fill(dt);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Lỗi khi lấy danh sách Minigame: " + ex.Message);
        }
        return dt;
    }
    #region Minigame Data
    public static string GetGameData(string maMNG)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT DuLieu FROM Minigame WHERE MaMNG = @maMNG";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@maMNG", maMNG);
                object result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }
    }
    public static void SaveGameData(string maMNG, string data)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "UPDATE Minigame SET DuLieu = @data WHERE MaMNG = @maMNG";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@data", (object)data ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@maMNG", maMNG);
                cmd.ExecuteNonQuery();
            }
        }
    }
    #endregion
    public static void DeleteGhiChuTKB(string maGV, DateTime ngay, int tiet)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string maMon = null;
            string maTKB = null;

            string checkSql = "SELECT MaTKB, MaMon FROM ThoiKhoaBieu WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet";
            using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
            {
                checkCmd.Parameters.AddWithValue("@MaGV", maGV);
                checkCmd.Parameters.AddWithValue("@Ngay", ngay.Date);
                checkCmd.Parameters.AddWithValue("@Tiet", tiet);
                using (SqlDataReader reader = checkCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        maTKB = reader["MaTKB"].ToString();
                        if (reader["MaMon"] != DBNull.Value)
                        {
                            maMon = reader["MaMon"].ToString();
                        }
                    }
                }
            }

            if (maTKB == null) return;

            if (string.IsNullOrEmpty(maMon))
            {
                string deleteSql = "DELETE FROM ThoiKhoaBieu WHERE MaTKB=@MaTKB";
                using (SqlCommand deleteCmd = new SqlCommand(deleteSql, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@MaTKB", maTKB);
                    deleteCmd.ExecuteNonQuery();
                }
            }
            else
            {
                string updateSql = "UPDATE ThoiKhoaBieu SET GhiChu = NULL WHERE MaTKB=@MaTKB";
                using (SqlCommand updateCmd = new SqlCommand(updateSql, conn))
                {
                    updateCmd.Parameters.AddWithValue("@MaTKB", maTKB);
                    updateCmd.ExecuteNonQuery();
                }
            }
        }
    }
    public static DataTable GetGiaoVienByTrangThai(string trangThai)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = "SELECT MaGV, Ten, Username, Email, SDT, TrangThai FROM GiaoVien WHERE TrangThai = @tt";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@tt", trangThai);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    public static void UpdateTrangThaiGiaoVien(string maGV, string trangThai)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "UPDATE GiaoVien SET TrangThai=@tt WHERE MaGV=@id";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@tt", trangThai);
                cmd.Parameters.AddWithValue("@id", maGV);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static void UpdateGiaoVien(string maGV, string ten, string email, string sdt)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "UPDATE GiaoVien SET Ten=@t, Email=@e, SDT=@s WHERE MaGV=@id";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@t", ten);
                cmd.Parameters.AddWithValue("@e", email);
                cmd.Parameters.AddWithValue("@s", sdt);
                cmd.Parameters.AddWithValue("@id", maGV);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static void DeleteGiaoVien(string maGV)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "DELETE FROM GiaoVien WHERE MaGV=@id";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", maGV);
                cmd.ExecuteNonQuery();
            }
        }
    }
    public static DataTable GetAllGiaoVien()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = "SELECT MaGV, Ten, Username, Email, SDT, TrangThai FROM GiaoVien";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
    public static DataTable GetAllHocSinh()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = "SELECT MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc FROM HocSinh ORDER BY MaLop, HoTen";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }


    public static void InsertHocSinh(string maHS, string maLop, string hoTen, DateTime ngaySinh,
                                     string gioiTinh, string sdtPH, string diaChi, string danToc)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string check = "SELECT COUNT(1) FROM HocSinh WHERE MaHS=@MaHS";
            using (SqlCommand chk = new SqlCommand(check, conn))
            {
                chk.Parameters.AddWithValue("@MaHS", maHS);
                int cnt = Convert.ToInt32(chk.ExecuteScalar());
                if (cnt > 0) throw new Exception($"Học sinh {maHS} đã tồn tại.");
            }

            string sql = @"INSERT INTO HocSinh (MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc)
                           VALUES (@MaHS, @MaLop, @HoTen, @NgaySinh, @GioiTinh, @SDT, @DiaChi, @DanToc)";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaHS", maHS);
                cmd.Parameters.AddWithValue("@MaLop", string.IsNullOrWhiteSpace(maLop) ? (object)DBNull.Value : maLop);
                cmd.Parameters.AddWithValue("@HoTen", hoTen ?? "");
                cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh ?? "");
                cmd.Parameters.AddWithValue("@SDT", sdtPH ?? "");
                cmd.Parameters.AddWithValue("@DiaChi", diaChi ?? "");
                cmd.Parameters.AddWithValue("@DanToc", danToc ?? "");
                cmd.ExecuteNonQuery();
            }
        }
    }


    public static void UpdateHocSinh(string maHS, string hoTen, DateTime ngaySinh,
                                     string gioiTinh, string sdtPH, string diaChi, string danToc)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"UPDATE HocSinh
                           SET HoTen=@HoTen, NgaySinh=@NgaySinh, GioiTinh=@GioiTinh,
                               SDTPhuHuynh=@SDT, DiaChi=@DiaChi, DanToc=@DanToc
                           WHERE MaHS=@MaHS";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaHS", maHS);
                cmd.Parameters.AddWithValue("@HoTen", hoTen ?? "");
                cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh ?? "");
                cmd.Parameters.AddWithValue("@SDT", sdtPH ?? "");
                cmd.Parameters.AddWithValue("@DiaChi", diaChi ?? "");
                cmd.Parameters.AddWithValue("@DanToc", danToc ?? "");
                cmd.ExecuteNonQuery();
            }
        }
    }


    public static void DeleteHocSinh(string maHS)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "DELETE FROM HocSinh WHERE MaHS=@MaHS";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaHS", maHS);
                cmd.ExecuteNonQuery();
            }
        }
    }
    public struct ImportResult
    {
        public int Success;
        public int Skipped;
        public int Failed;
    }
    public static ImportResult ImportHocSinhFromDataTable(DataTable dt)
    {
        var result = new ImportResult();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlTransaction tran = conn.BeginTransaction())
            {
                try
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string maHS = row["MaHS"]?.ToString().Trim();
                        if (string.IsNullOrWhiteSpace(maHS))
                        {
                            result.Failed++;
                            continue;
                        }

                        DateTime ngaySinh;
                        string ngayStr = row["NgaySinh"]?.ToString().Trim();
                        if (!DateTime.TryParseExact(ngayStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out ngaySinh))
                        {
                            if (!DateTime.TryParse(ngayStr, out ngaySinh))
                            {
                                result.Failed++;
                                continue;
                            }
                        }

                        string maLop = row["MaLop"]?.ToString().Trim();
                        string hoTen = row["HoTen"]?.ToString().Trim();
                        string gioiTinh = row["GioiTinh"]?.ToString().Trim();
                        string sdt = row["SDTPhuHuynh"]?.ToString().Trim();
                        string diaChi = row["DiaChi"]?.ToString().Trim();
                        string danToc = row["DanToc"]?.ToString().Trim();
                        string check = "SELECT COUNT(1) FROM HocSinh WHERE MaHS=@MaHS";
                        using (SqlCommand chk = new SqlCommand(check, conn, tran))
                        {
                            chk.Parameters.AddWithValue("@MaHS", maHS);
                            int cnt = Convert.ToInt32(chk.ExecuteScalar());
                            if (cnt > 0)
                            {
                                result.Skipped++;
                                continue;
                            }
                        }
                        string insert = @"INSERT INTO HocSinh (MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc)
                                          VALUES(@MaHS, @MaLop, @HoTen, @NgaySinh, @GioiTinh, @SDT, @DiaChi, @DanToc)";
                        using (SqlCommand cmd = new SqlCommand(insert, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@MaHS", maHS);
                            cmd.Parameters.AddWithValue("@MaLop", string.IsNullOrWhiteSpace(maLop) ? (object)DBNull.Value : maLop);
                            cmd.Parameters.AddWithValue("@HoTen", hoTen ?? "");
                            cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                            cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh ?? "");
                            cmd.Parameters.AddWithValue("@SDT", sdt ?? "");
                            cmd.Parameters.AddWithValue("@DiaChi", diaChi ?? "");
                            cmd.Parameters.AddWithValue("@DanToc", danToc ?? "");
                            cmd.ExecuteNonQuery();
                        }

                        result.Success++;
                    }

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        return result;
    }

    #region Báo cáo
    // =======================================================================================
    // ====> HÀM BỊ SỬA: Cập nhật câu SQL để lấy danh sách lớp một giáo viên dạy
    // =======================================================================================
    public static DataTable GetLopByGiaoVien(string maGV)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            // Sửa câu SQL:
            // - Join LopHoc với PhanCongGiangDay (thay vì GiaoVien)
            // - Lọc theo MaGV từ bảng PhanCongGiangDay
            string sql = @"SELECT l.MaLop, l.TenLop 
                           FROM LopHoc l
                           INNER JOIN PhanCongGiangDay pc ON l.MaLop = pc.MaLop
                           WHERE pc.MaGV = @maGV";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@maGV", maGV);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    public static DataTable GetBaoCaoChuyenCan(string maLop)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = @"SELECT 
            hs.MaHS,
            hs.HoTen,
            COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) as SoNgayCoMat,
            COUNT(CASE WHEN dd.TrangThai = N'Vắng' THEN 1 END) as SoNgayVang,
            COUNT(CASE WHEN dd.TrangThai = N'Có phép' THEN 1 END) as SoNgayCoPhep,
            COUNT(dd.MaDD) as TongSoBuoi,
            CAST(COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) * 100.0 / NULLIF(COUNT(dd.MaDD), 0) as DECIMAL(5,2)) as TyLeChuyenCan
        FROM HocSinh hs
        LEFT JOIN DiemDanh dd ON hs.MaHS = dd.MaHS
        WHERE hs.MaLop = @maLop
        GROUP BY hs.MaHS, hs.HoTen
        ORDER BY hs.HoTen";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@maLop", maLop);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    public static DataTable GetBangDiemHocKy(string maLop, int hocKy)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string loaiFilter = hocKy == 1 ? "Ki1" : "Ki2";

            string sql = $@"SELECT 
            hs.MaHS,
            hs.HoTen,
            mh.TenMon,
            AVG(CASE 
                WHEN kq.Loai LIKE '%{loaiFilter}%' THEN kq.Diem 
                ELSE NULL 
            END) as DiemTrungBinh,
            MAX(CASE 
                WHEN kq.Loai = 'CuoiKi{hocKy}' THEN kq.Diem 
                ELSE NULL 
            END) as DiemCuoiKy
        FROM HocSinh hs
        CROSS JOIN MonHoc mh
        LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS AND mh.MaMon = kq.MaMon 
            AND kq.Loai LIKE '%{loaiFilter}%'
        WHERE hs.MaLop = @maLop
        GROUP BY hs.MaHS, hs.HoTen, mh.TenMon, mh.MaMon
        ORDER BY hs.HoTen, mh.MaMon";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@maLop", maLop);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    public static DataTable GetHoSoHocSinh(string maLop)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = @"SELECT 
            MaHS,
            HoTen,
            GioiTinh,
            NgaySinh,
            DanToc,
            DiaChi,
            SDTPhuHuynh
        FROM HocSinh 
        WHERE MaLop = @maLop
        ORDER BY HoTen";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@maLop", maLop);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    public static DataTable GetThongKeKhoi(string khoi)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = @"SELECT 
            l.TenLop,
            COUNT(hs.MaHS) as SoHocSinh,
            AVG(kq.Diem) as DiemTrungBinh,
            COUNT(CASE WHEN hs.GioiTinh = N'Nam' THEN 1 END) as SoNam,
            COUNT(CASE WHEN hs.GioiTinh = N'Nữ' THEN 1 END) as SoNu
        FROM LopHoc l
        LEFT JOIN HocSinh hs ON l.MaLop = hs.MaLop
        LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS
        WHERE l.Khoi = @khoi
        GROUP BY l.TenLop
        ORDER BY l.TenLop";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@khoi", khoi);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
    #endregion
    // Thêm 2 phương thức này vào trong class DatabaseHelper trong file Class1.cs

    #region Hỗ trợ Giảng dạy



    // Dán vào class DatabaseHelper, thay thế hàm AddGhiChuTKB cũ

    public static void AddGhiChuTKB(string maGV, string maLop, DateTime ngay, int tiet, string ghiChu)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sqlCheck = "SELECT MaTKB FROM ThoiKhoaBieu WHERE MaLop = @MaLop AND Ngay = @Ngay AND Tiet = @Tiet";
            object maTKB = null;
            using (var cmdCheck = new SqlCommand(sqlCheck, conn))
            {
                cmdCheck.Parameters.AddWithValue("@MaLop", maLop);
                cmdCheck.Parameters.AddWithValue("@Ngay", ngay.Date);
                cmdCheck.Parameters.AddWithValue("@Tiet", tiet);
                maTKB = cmdCheck.ExecuteScalar();
            }

            if (maTKB != null)
            {
                // ====> THAY ĐỔI LOGIC Ở ĐÂY <====
                // Nếu đã tồn tại, nối thêm ghi chú mới vào ghi chú cũ
                string sqlUpdate = "UPDATE ThoiKhoaBieu SET GhiChu = ISNULL(GhiChu, '') + NCHAR(13) + NCHAR(10) + @AppendedGhiChu, MaGV = @MaGV WHERE MaTKB = @MaTKB";
                using (SqlCommand cmd = new SqlCommand(sqlUpdate, conn))
                {
                    
                    cmd.Parameters.AddWithValue("@AppendedGhiChu", " ," + ghiChu);
                    cmd.Parameters.AddWithValue("@MaGV", maGV);
                    cmd.Parameters.AddWithValue("@MaTKB", maTKB.ToString());
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                // Nếu chưa tồn tại, tạo một mục ghi chú mới (giữ nguyên như cũ)
                string sqlInsert = @"INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, GhiChu, MaGV, MaLop)
                           VALUES (@MaTKB, @Ngay, @Tiet, @GhiChu, @MaGV, @MaLop)";
                using (SqlCommand cmd = new SqlCommand(sqlInsert, conn))
                {
                    cmd.Parameters.AddWithValue("@MaTKB", "GC" + Guid.NewGuid().ToString("N").Substring(0, 7));
                    cmd.Parameters.AddWithValue("@Ngay", ngay.Date);
                    cmd.Parameters.AddWithValue("@Tiet", tiet);
                    cmd.Parameters.AddWithValue("@GhiChu", ghiChu); // Ghi chú đầu tiên không cần dấu +
                    cmd.Parameters.AddWithValue("@MaGV", maGV);
                    cmd.Parameters.AddWithValue("@MaLop", maLop);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
    public static void AddGhiChuChoHocSinh(string maHS, string maMon, string ghiChu)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            // Cập nhật ghi chú vào mục điểm cuối kì 2 (hoặc một mục chung khác)
            // Nếu chưa có, tạo một mục mới.
            string loaiGhiChu = "CuoiKi2";
            string sql = $@"
            IF EXISTS (SELECT 1 FROM KetQuaHocTap WHERE MaHS = @MaHS AND MaMon = @MaMon AND Loai = @Loai)
            BEGIN
                UPDATE KetQuaHocTap SET GhiChu = @GhiChu WHERE MaHS = @MaHS AND MaMon = @MaMon AND Loai = @Loai
            END
            ELSE
            BEGIN
                INSERT INTO KetQuaHocTap (MaKQ, MaMon, MaHS, NgayNhap, GhiChu, Loai)
                VALUES (@MaKQ, @MaMon, @MaHS, GETDATE(), @GhiChu, @Loai)
            END";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaKQ", "KQ" + Guid.NewGuid().ToString("N").Substring(0, 7));
                cmd.Parameters.AddWithValue("@MaHS", maHS);
                cmd.Parameters.AddWithValue("@MaMon", maMon);
                cmd.Parameters.AddWithValue("@GhiChu", ghiChu);
                cmd.Parameters.AddWithValue("@Loai", loaiGhiChu);
                cmd.ExecuteNonQuery();
            }
        }
    }
    // Dán vào trong class DatabaseHelper
    public static DataTable GetAllKetQuaHocTap()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = @"
            SELECT 
                hs.MaHS,
                hs.HoTen,
                lh.MaLop,
                lh.TenLop,
                mh.MaMon,
                mh.TenMon,
                kq.Loai,
                kq.Diem
            FROM KetQuaHocTap kq
            JOIN HocSinh hs ON kq.MaHS = hs.MaHS
            JOIN LopHoc lh ON hs.MaLop = lh.MaLop
            JOIN MonHoc mh ON kq.MaMon = mh.MaMon
            WHERE kq.Diem IS NOT NULL";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
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
    // Lấy danh sách tất cả môn học để đổ vào ComboBox
    public static DataTable GetAllMonHoc()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = "SELECT MaMon, TenMon FROM MonHoc ORDER BY TenMon";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    // Hàm mới, hiệu suất cao để lấy dữ liệu cho việc phân tích
    public static DataTable GetScoresForAnalysis(string maGV, string phamVi, string chiTiet, string maMon, int hocKy)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            // Xây dựng câu lệnh SQL linh hoạt
            var sqlBuilder = new System.Text.StringBuilder(@"
            SELECT 
                hs.MaHS, hs.HoTen, lh.MaLop, lh.TenLop,
                mh.MaMon, mh.TenMon, kq.Loai, kq.Diem
            FROM KetQuaHocTap kq
            JOIN HocSinh hs ON kq.MaHS = hs.MaHS
            JOIN LopHoc lh ON hs.MaLop = lh.MaLop
            JOIN MonHoc mh ON kq.MaMon = mh.MaMon
            WHERE kq.Diem IS NOT NULL
        ");

            var sqlParams = new List<SqlParameter>();

            // 1. Lọc theo học kỳ
            string kyFilter = $"%Ki{hocKy}";
            sqlBuilder.Append(" AND kq.Loai LIKE @kyFilter");
            sqlParams.Add(new SqlParameter("@kyFilter", kyFilter));

            // 2. Lọc theo phạm vi (Lớp của GV hay Toàn Khối)
            if (phamVi == "LopGV")
            {
                sqlBuilder.Append(" AND lh.MaLop = @chiTiet");
                sqlParams.Add(new SqlParameter("@chiTiet", chiTiet));
            }
            else if (phamVi == "Khoi")
            {
                sqlBuilder.Append(" AND lh.Khoi = @chiTiet");
                sqlParams.Add(new SqlParameter("@chiTiet", chiTiet));
            }

            // 3. Lọc theo môn học (nếu có chọn)
            if (!string.IsNullOrEmpty(maMon) && maMon != "ALL")
            {
                sqlBuilder.Append(" AND mh.MaMon = @maMon");
                sqlParams.Add(new SqlParameter("@maMon", maMon));
            }

            SqlDataAdapter da = new SqlDataAdapter(sqlBuilder.ToString(), conn);
            da.SelectCommand.Parameters.AddRange(sqlParams.ToArray());
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
    public static void CreateTeacherRequest(string ten, string username, string password, string maMon, string email, string sdt)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            // Kiểm tra tên đăng nhập đã tồn tại chưa
            string checkUser = "SELECT COUNT(*) FROM GiaoVien WHERE Username=@user";
            using (SqlCommand cmdCheck = new SqlCommand(checkUser, conn))
            {
                cmdCheck.Parameters.AddWithValue("@user", username);
                if ((int)cmdCheck.ExecuteScalar() > 0)
                {
                    throw new Exception("Tên đăng nhập này đã tồn tại. Vui lòng chọn tên khác.");
                }
            }

            // Tạo MaGV mới một cách tự động
            string getNewIdSql = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaGV, 3, LEN(MaGV)) AS INT)), 0) + 1 FROM GiaoVien";
            int newId;
            using (SqlCommand cmdNewId = new SqlCommand(getNewIdSql, conn))
            {
                newId = (int)cmdNewId.ExecuteScalar();
            }
            string newMaGV = "GV" + newId.ToString("D3"); // Định dạng GV001, GV012, v.v.

            string sql = @"INSERT INTO GiaoVien (MaGV, Ten, Username, Password, MaMon, Email, SDT, MaAdmin, TrangThai) 
                           VALUES (@MaGV, @Ten, @Username, @Password, @MaMon, @Email, @SDT, @MaAdmin, @TrangThai)";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaGV", newMaGV);
                cmd.Parameters.AddWithValue("@Ten", ten);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@MaMon", maMon);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@SDT", sdt);
                cmd.Parameters.AddWithValue("@MaAdmin", "AD001"); // Gán cho Admin mặc định
                cmd.Parameters.AddWithValue("@TrangThai", "Chưa xác nhận"); // Trạng thái mặc định
                cmd.ExecuteNonQuery();
            }
        }
    }
}