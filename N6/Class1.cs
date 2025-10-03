using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

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

    /// <summary>
    /// Lấy tất cả bản ghi điểm danh kèm thông tin học sinh của 1 lớp (có thể có nhiều ngày).
    /// Sử dụng khi cần xem lịch sử (UI có thể lọc theo ngày/buổi).
    /// </summary>
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

    /// <summary>
    /// Lấy danh sách điểm danh cho 1 lớp, 1 ngày cụ thể và 1 buổi cụ thể.
    /// Trả về tất cả học sinh lớp (nếu học sinh chưa có bản ghi cho ngày đó thì các cột dd sẽ NULL).
    /// </summary>
    public static DataTable GetDiemDanhByLopAndDate(string maLop, DateTime ngay, string buoi)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            // tạo subquery chỉ lấy bản ghi DiemDanh cho ngày & buổi cụ thể
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

    /// <summary>
    /// Tạo bản ghi điểm danh mặc định ("Có mặt") cho tất cả học sinh trong lớp cho 1 ngày & buổi nhất định.
    /// Nếu đã có bản ghi cho học sinh đó ở ngày/buổi tương ứng thì không chèn trùng.
    /// </summary>
    public static void ExecTaoDiemDanhMacDinh(string maLop, DateTime ngay, string buoi)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            // Lấy danh sách MaHS của lớp
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

            // Chuẩn bị insert nếu chưa tồn tại
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

    /// <summary>
    /// Overload cũ cho backward-compatibility: nếu gọi không truyền ngày/buổi thì dùng ngày hôm nay & buổi 'Sáng'.
    /// </summary>
    public static void ExecTaoDiemDanhMacDinh(string maLop)
    {
        ExecTaoDiemDanhMacDinh(maLop, DateTime.Today, "Sáng");
    }

    /// <summary>
    /// Lưu / cập nhật điểm danh cho 1 học sinh ở 1 ngày & buổi cụ thể.
    /// Nếu 'ngay' null => dùng ngày hôm nay. Nếu 'buoi' null => mặc định "Sáng".
    /// </summary>
    public static void LuuDiemDanh(string maHS, string trangThai, DateTime? ngay = null, string buoi = null)
    {
        DateTime actualDate = (ngay ?? DateTime.Now).Date;
        string actualBuoi = string.IsNullOrEmpty(buoi) ? "Sáng" : buoi;

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            // check exists for that student/date/buoi
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
                cmd.Parameters.AddWithValue("@kieu", filePath); // lưu đường dẫn
                cmd.Parameters.AddWithValue("@tt", trangThai ?? "Riêng tư");
                cmd.Parameters.AddWithValue("@gv", maGV);
                cmd.ExecuteNonQuery();
            }
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


}
