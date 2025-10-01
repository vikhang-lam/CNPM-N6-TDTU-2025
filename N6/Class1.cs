using System;
using System.Data;
using System.Data.SqlClient;

public class TeacherProfile
{
    public string Ten { get; set; }
    public string TenMon { get; set; }
}

public static class DatabaseHelper
{
    private static string connectionString =
        @"Data Source=LAPTOP-3IRTDDBK;Initial Catalog=quanlilophoc_giangday;Integrated Security=True;";

    // ✅ Đăng nhập giáo viên
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
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
    }

    // ✅ Đăng nhập admin
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
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
    }

    // ✅ Lấy thông tin giáo viên
    public static TeacherProfile GetTeacherProfile(string username)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = @"
                SELECT gv.Ten, mh.TenMon
                FROM GiaoVien gv
                LEFT JOIN MonHoc mh ON gv.MaMon = mh.MaMon
                WHERE gv.Username = @user";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@user", username);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        return new TeacherProfile
                        {
                            Ten = r["Ten"] == DBNull.Value ? "" : r["Ten"].ToString(),
                            TenMon = r["TenMon"] == DBNull.Value ? "Chưa có môn" : r["TenMon"].ToString()
                        };
                    }
                }
            }
        }
        return null;
    }

    // ✅ Lấy lớp mà giáo viên đang phụ trách
    public static string GetLopByTeacher(string identifier)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"
            SELECT MaLop 
            FROM GiaoVien 
            WHERE Username=@id OR Ten=@id";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", identifier);
                object result = cmd.ExecuteScalar();
                return result == null ? "" : result.ToString();
            }
        }
    }


    // ✅ Lấy danh sách học sinh theo lớp
    public static DataTable GetHocSinhByLop(string maLop)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT MaHS, HoTen, GioiTinh, NgaySinh FROM HocSinh WHERE MaLop=@malop";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            da.SelectCommand.Parameters.AddWithValue("@malop", maLop);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    // ✅ Lưu điểm danh
    public static void LuuDiemDanh(string maHS, string trangThai)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"INSERT INTO DiemDanh(MaDD, MaHS, NgayDD, Buoi, TrangThai) 
                           VALUES(@MaDD, @MaHS, GETDATE(), N'Sáng', @TrangThai)";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaDD", Guid.NewGuid().ToString().Substring(0, 8));
                cmd.Parameters.AddWithValue("@MaHS", maHS);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // ✅ Lưu kết quả học tập
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
                cmd.Parameters.AddWithValue("@NhanXet", nhanXet);
                cmd.ExecuteNonQuery();
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
    // ✅ Lấy hồ sơ học sinh
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
                if (dt.Rows.Count > 0) return dt.Rows[0];
                return null;
            }
        }
    }
}
