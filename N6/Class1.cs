using System;
using System.Data;
using System.Data.SqlClient;
using Xceed.Wpf.AvalonDock.Themes;

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
            WHERE gv.Username = @user OR Ten = @user";
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


    public static string GetLopByTeacher(string identifier)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"SELECT MaLop FROM GiaoVien WHERE Username=@id OR Ten=@id";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", identifier);
                object result = cmd.ExecuteScalar();
                return result?.ToString() ?? "";
            }
        }
    }

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
            dd.MaDD,      -- ✅ thêm cột này
            hs.MaHS,
            hs.HoTen,
            dd.NgayDD,
            dd.Buoi,
            dd.TrangThai
        FROM HocSinh hs
        LEFT JOIN DiemDanh dd ON hs.MaHS = dd.MaHS
        WHERE hs.MaLop = @maLop
        ORDER BY hs.HoTen, dd.NgayDD;";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@maLop", maLop ?? string.Empty);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }


    public static void LuuDiemDanh(string maHS, string trangThai)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            // kiểm tra đã có điểm danh hôm nay chưa
            string check = @"SELECT COUNT(*) FROM DiemDanh WHERE MaHS=@MaHS AND CAST(NgayDD AS date)=CAST(GETDATE() AS date)";
            using (SqlCommand cmdCheck = new SqlCommand(check, conn))
            {
                cmdCheck.Parameters.AddWithValue("@MaHS", maHS);
                int count = (int)cmdCheck.ExecuteScalar();
                if (count > 0)
                {
                    string update = "UPDATE DiemDanh SET TrangThai=@TrangThai WHERE MaHS=@MaHS AND CAST(NgayDD AS date)=CAST(GETDATE() AS date)";
                    using (SqlCommand cmdUp = new SqlCommand(update, conn))
                    {
                        cmdUp.Parameters.AddWithValue("@TrangThai", trangThai);
                        cmdUp.Parameters.AddWithValue("@MaHS", maHS);
                        cmdUp.ExecuteNonQuery();
                    }
                }
                else
                {
                    string insert = @"INSERT INTO DiemDanh(MaDD, MaHS, NgayDD, Buoi, TrangThai) 
                                      VALUES(@MaDD, @MaHS, GETDATE(), N'Sáng', @TrangThai)";
                    using (SqlCommand cmdIn = new SqlCommand(insert, conn))
                    {
                        cmdIn.Parameters.AddWithValue("@MaDD", Guid.NewGuid().ToString().Substring(0, 8));
                        cmdIn.Parameters.AddWithValue("@MaHS", maHS);
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
    public static void ExecTaoDiemDanhMacDinh(string maLop)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("sp_TaoDiemDanhMacDinh", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaLop", maLop);
                cmd.ExecuteNonQuery();
            }
        }
    }
    #endregion
    #region Hồ sơ cá nhân (Profile)

    // Lấy đầy đủ thông tin giáo viên
    
    

    // Cập nhật Email, SDT, Avatar
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

    // Đổi mật khẩu giáo viên
    public static bool ChangeTeacherPassword(string username, string oldPass, string newPass)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            // 1. Kiểm tra mật khẩu cũ chính xác không
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
                return false; // Sai mật khẩu cũ
            }

            // 2. Kiểm tra mật khẩu mới có ký tự đặc biệt không
            if (System.Text.RegularExpressions.Regex.IsMatch(newPass, @"[^a-zA-Z0-9]"))
            {
                throw new ArgumentException("Mật khẩu không được chứa ký tự đặc biệt!");
            }

            // 3. Cập nhật mật khẩu mới
            string update = "UPDATE GiaoVien SET Password=@new WHERE Username=@u OR Ten =@u" ;
            using (SqlCommand cmd = new SqlCommand(update, conn))
            {
                cmd.Parameters.Add("@new", SqlDbType.VarChar).Value = newPass;
                cmd.Parameters.Add("@u", SqlDbType.VarChar).Value = username;
                cmd.ExecuteNonQuery();
                return true;
            }
        }
    }


    #endregion

}
