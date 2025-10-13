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
    //@"Data Source=DES-RH3KRAF\SQLEXPRESS;Initial Catalog=quanlilophoc_giangday;Integrated Security=True;";

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
    #region Đăng nhập
    public static LoginStatus CheckTeacherLogin(string username, string password)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            // Lấy ra trạng thái của tài khoản nếu user/pass đúng
            string query = "SELECT TrangThai FROM GiaoVien WHERE Username=@user AND Password=@pass";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", password);

                object result = cmd.ExecuteScalar();

                // Trường hợp 1: Sai username hoặc password
                if (result == null)
                {
                    return LoginStatus.InvalidCredentials;
                }

                string trangThai = result.ToString();

                // Trường hợp 2: Đúng user/pass nhưng tài khoản chưa được xác nhận
                if (trangThai.Equals("Chưa xác nhận", StringComparison.OrdinalIgnoreCase))
                {
                    return LoginStatus.AccountNotActivated;
                }

                // Trường hợp 3: Đăng nhập thành công
                if (trangThai.Equals("Đã xác nhận", StringComparison.OrdinalIgnoreCase))
                {
                    return LoginStatus.Success;
                }

                // Các trường hợp khác cũng coi như không hợp lệ
                return LoginStatus.InvalidCredentials;
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
            string query = @"SELECT MaHS, HoTen, GioiTinh, NgaySinh, DiaChi, DanToc, SDTPhuHuynh 
                             FROM HocSinh 
                             WHERE MaLop=@malop 
                             ORDER BY HoTen";
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
                dd.TrangThai,
                dd.ThoiGianCapNhat
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
                // Trigger trong CSDL sẽ tự động cập nhật ThoiGianCapNhat
                string update = "UPDATE DiemDanh SET TrangThai=@TrangThai, NgayDD=@NgayDD WHERE MaDD=@MaDD";
                using (SqlCommand up = new SqlCommand(update, conn))
                {
                    up.Parameters.AddWithValue("@TrangThai", trangThai);
                    up.Parameters.Add("@NgayDD", SqlDbType.DateTime).Value = ngay;
                    up.Parameters.AddWithValue("@MaDD", existingId.ToString());
                    up.ExecuteNonQuery();
                }
            }
            else
            {
                // CSDL sẽ tự động gán ThoiGianCapNhat bằng hàm DEFAULT GETDATE()
                string insert = @"INSERT INTO DiemDanh(MaDD, MaHS, NgayDD, Buoi, TrangThai)
                              VALUES(@MaDD, @MaHS, @NgayDD, @Buoi, @TrangThai)";
                using (SqlCommand ins = new SqlCommand(insert, conn))
                {
                    ins.Parameters.AddWithValue("@MaDD", Guid.NewGuid().ToString().Substring(0, 8));
                    ins.Parameters.AddWithValue("@MaHS", maHS);
                    ins.Parameters.Add("@NgayDD", SqlDbType.DateTime).Value = ngay;
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
                    dd.TrangThai,
                    dd.ThoiGianCapNhat
                FROM HocSinh hs
                LEFT JOIN (
                    SELECT MaDD, MaHS, NgayDD, Buoi, TrangThai, ThoiGianCapNhat
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

    // Dán và thay thế hàm GetBangDiemPivot cũ trong file Class1.cs của bạn
    public static DataTable GetBangDiemPivot(string maLop, int ki, string maMon)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "";
            string loaiFilter = $"%Ki{ki}%";

            sql = @"
            SELECT 
                hs.MaHS, 
                hs.HoTen,
                lh.TenLop,
                MAX(CASE WHEN kq.Loai = 'Thang1_Ki" + ki + @"' THEN kq.Diem END) AS Thang1,
                MAX(CASE WHEN kq.Loai = 'Thang2_Ki" + ki + @"' THEN kq.Diem END) AS Thang2,
                MAX(CASE WHEN kq.Loai = 'Thang3_Ki" + ki + @"' THEN kq.Diem END) AS Thang3,
                MAX(CASE WHEN kq.Loai = 'GiuaKi" + ki + @"' THEN kq.Diem END) AS GiuaKi,
                MAX(CASE WHEN kq.Loai = 'CuoiKi" + ki + @"' THEN kq.Diem END) AS CuoiKi,
                MAX(CASE WHEN kq.Loai LIKE @loaiFilter THEN kq.NhanXet END) AS NhanXet,
                MAX(CASE WHEN kq.Loai LIKE @loaiFilter THEN kq.GhiChu END) AS GhiChu
            FROM HocSinh hs
            JOIN LopHoc lh ON hs.MaLop = lh.MaLop
            LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS AND kq.MaMon = @maMon
            WHERE hs.MaLop = @malop
            GROUP BY hs.MaHS, hs.HoTen, lh.TenLop
            ORDER BY hs.HoTen";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@malop", maLop);
            da.SelectCommand.Parameters.AddWithValue("@maMon", maMon);
            da.SelectCommand.Parameters.AddWithValue("@loaiFilter", loaiFilter);
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
            // Sửa lại câu SQL để lấy thêm cột MaMon, rất quan trọng cho việc lọc
            string sql = "SELECT MaGV, Ten, Username, Email, SDT, TrangThai, MaMon FROM GiaoVien";
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
    public static DataTable GetLopByGiaoVien(string maGV)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            // Logic này đã đúng với yêu cầu của bạn: lấy từ PhanCongGiangDay và lớp chủ nhiệm
            string sql = @"
                SELECT DISTINCT l.MaLop, l.TenLop 
                FROM LopHoc l
                JOIN PhanCongGiangDay pc ON l.MaLop = pc.MaLop
                WHERE pc.MaGV = @maGV
                UNION
                SELECT MaLop, TenLop 
                FROM LopHoc
                WHERE MaGVCN = @maGV";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@maGV", maGV);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    // ĐÃ SỬA LẠI HÀM NÀY ĐỂ HỖ TRỢ "CẢ NĂM"
    public static DataTable GetBangDiemHocKy(string maLop, int hocKy)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            List<string> monHocs = new List<string>();
            using (SqlCommand cmdMon = new SqlCommand("SELECT DISTINCT TenMon FROM MonHoc ORDER BY TenMon", conn))
            {
                using (SqlDataReader reader = cmdMon.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        monHocs.Add(reader.GetString(0));
                    }
                }
            }

            if (monHocs.Count == 0)
                return new DataTable();

            string loaiFilter = (hocKy == 3) ? "Ki" : $"Ki{hocKy}";

            string monHocCols = string.Join(", ", monHocs.Select(m => $"[{m}]"));

            // Thêm ROUND(..., 2) vào từng cột điểm môn học
            string monHocColsSelect = string.Join(", ", monHocs.Select(m => $"ROUND(ISNULL([{m}], 0), 2) AS [{m}]"));

            string tongMon = string.Join(" + ", monHocs.Select(m => $"ISNULL([{m}], 0)"));

            string sql = $@"
            WITH DiemTB AS (
                SELECT 
                    hs.MaHS,
                    hs.HoTen,
                    lh.TenLop,
                    mh.TenMon,
                    AVG(kq.Diem) AS DiemTB
                FROM HocSinh hs
                INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop
                CROSS JOIN MonHoc mh
                LEFT JOIN KetQuaHocTap kq 
                    ON hs.MaHS = kq.MaHS 
                    AND mh.MaMon = kq.MaMon 
                    AND kq.Loai LIKE @loaiFilter
                WHERE hs.MaLop = @maLop
                GROUP BY hs.MaHS, hs.HoTen, lh.TenLop, mh.TenMon
            ),
            PivotData AS (
                SELECT MaHS, HoTen, TenLop, {monHocCols}
                FROM DiemTB
                PIVOT
                (
                    AVG(DiemTB)
                    FOR TenMon IN ({monHocCols})
                ) AS PivotTable
            )
            SELECT MaHS, HoTen, {monHocColsSelect},
                   ROUND(({tongMon}) / NULLIF({monHocs.Count}, 0), 2) AS [Trung bình chung]
            FROM PivotData
            ORDER BY HoTen;";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@maLop", maLop);
            da.SelectCommand.Parameters.AddWithValue("@loaiFilter", "%" + loaiFilter + "%");

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
            // Thêm ROUND(..., 2) để làm tròn điểm trung bình
            string sql = @"SELECT 
            l.TenLop,
            COUNT(hs.MaHS) as SoHocSinh,
            ROUND(AVG(kq.Diem), 2) as DiemTrungBinh, 
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

    #region Hỗ trợ Giảng dạy

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
                string sqlInsert = @"INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, GhiChu, MaGV, MaLop)
                           VALUES (@MaTKB, @Ngay, @Tiet, @GhiChu, @MaGV, @MaLop)";
                using (SqlCommand cmd = new SqlCommand(sqlInsert, conn))
                {
                    cmd.Parameters.AddWithValue("@MaTKB", "GC" + Guid.NewGuid().ToString("N").Substring(0, 7));
                    cmd.Parameters.AddWithValue("@Ngay", ngay.Date);
                    cmd.Parameters.AddWithValue("@Tiet", tiet);
                    cmd.Parameters.AddWithValue("@GhiChu", ghiChu);
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

    public static DataTable GetScoresForAnalysis(string maGV, string phamVi, string chiTiet, string maMon, int hocKy)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
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

            string kyFilter = $"%Ki{hocKy}";
            sqlBuilder.Append(" AND kq.Loai LIKE @kyFilter");
            sqlParams.Add(new SqlParameter("@kyFilter", kyFilter));

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

            string checkUser = "SELECT COUNT(*) FROM GiaoVien WHERE Username=@user";
            using (SqlCommand cmdCheck = new SqlCommand(checkUser, conn))
            {
                cmdCheck.Parameters.AddWithValue("@user", username);
                if ((int)cmdCheck.ExecuteScalar() > 0)
                {
                    throw new Exception("Tên đăng nhập này đã tồn tại. Vui lòng chọn tên khác.");
                }
            }

            string getNewIdSql = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaGV, 3, LEN(MaGV)) AS INT)), 0) + 1 FROM GiaoVien";
            int newId;
            using (SqlCommand cmdNewId = new SqlCommand(getNewIdSql, conn))
            {
                newId = (int)cmdNewId.ExecuteScalar();
            }
            string newMaGV = "GV" + newId.ToString("D3");

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
                cmd.Parameters.AddWithValue("@MaAdmin", "AD001");
                cmd.Parameters.AddWithValue("@TrangThai", "Chưa xác nhận");
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static DataTable GetTaiLieuSharedWithUploader()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = @"SELECT 
                        tl.MaTL, tl.TenTL, tl.MoTa, tl.Kieu, tl.NgayTaiLen, 
                        gv.Ten AS TenGV 
                       FROM TaiLieu tl
                       INNER JOIN GiaoVien gv ON tl.MaGV = gv.MaGV
                       WHERE tl.TrangThaiChiaSe = N'Chia sẻ'";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
    public static void UnshareTaiLieu(string maTL)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "UPDATE TaiLieu SET TrangThaiChiaSe = N'Riêng tư' WHERE MaTL = @MaTL";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaTL", maTL);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static DataTable GetHomeroomClassesByTeacher(string maGV)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = "SELECT MaLop, TenLop FROM LopHoc WHERE MaGVCN = @maGV";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@maGV", maGV);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    public static DataTable GetHomeroomGradebook(string maLop, string loaiDiem)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("sp_GetHomeroomGradebook", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaLop", maLop);
                cmd.Parameters.AddWithValue("@LoaiDiem", loaiDiem);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }

    #region Global Events
    public static event EventHandler ThoiKhoaBieuChanged;

    public static void RaiseThoiKhoaBieuChanged()
    {
        ThoiKhoaBieuChanged?.Invoke(null, EventArgs.Empty);
    }
    #endregion

    // Thay thế hàm GetBaoCaoChuyenCan cũ trong file Class1.cs của bạn
    // Thay thế hàm GetBaoCaoChuyenCan cũ trong file Class1.cs của bạn
    public static DataTable GetBaoCaoChuyenCan(string maLop, int hocKy)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            /* * CẬP NHẬT LOGIC:
             * 1. Lấy ngày/tháng/năm hiện tại của hệ thống.
             * 2. Tự động xác định năm học đang diễn ra. 
             * - Nếu tháng hiện tại >= 8 (bắt đầu năm học mới), thì năm học sẽ kết thúc vào năm sau.
             * - Nếu tháng hiện tại < 8, thì năm học sẽ kết thúc vào năm nay.
             * 3. Dùng năm học vừa xác định để tính khoảng thời gian cho các học kỳ.
            */
            string sql = @"
        DECLARE @CurrentDate DATE = GETDATE();
        DECLARE @CurrentMonth INT = MONTH(@CurrentDate);
        DECLARE @CurrentYear INT = YEAR(@CurrentDate);
        DECLARE @NamHoc INT;

        IF @CurrentMonth >= 8 -- Nếu là tháng 8 trở đi, năm học sẽ kết thúc vào năm sau
        BEGIN
            SET @NamHoc = @CurrentYear + 1;
        END
        ELSE -- Nếu là tháng 1-7, năm học kết thúc trong năm nay
        BEGIN
            SET @NamHoc = @CurrentYear;
        END;
        
        DECLARE @StartDate DATE, @EndDate DATE;

        -- Học kỳ 1: từ tháng 8 đến tháng 12 của năm trước
        IF @hocKy = 1 
        BEGIN
            SET @StartDate = DATEFROMPARTS(@NamHoc - 1, 8, 1);
            SET @EndDate = DATEFROMPARTS(@NamHoc - 1, 12, 31);
        END
        -- Học kỳ 2: từ tháng 1 đến tháng 5 của năm học
        ELSE IF @hocKy = 2 
        BEGIN
            SET @StartDate = DATEFROMPARTS(@NamHoc, 1, 1);
            SET @EndDate = DATEFROMPARTS(@NamHoc, 5, 31);
        END
        -- Cả năm: từ tháng 8 năm trước đến tháng 5 năm học
        ELSE 
        BEGIN
            SET @StartDate = DATEFROMPARTS(@NamHoc - 1, 8, 1);
            SET @EndDate = DATEFROMPARTS(@NamHoc, 5, 31);
        END;

        SELECT 
            hs.MaHS,
            hs.HoTen,
            COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) as SoBuoiCoMat,
            COUNT(CASE WHEN dd.TrangThai = N'Vắng' THEN 1 END) as SoBuoiVang,
            COUNT(CASE WHEN dd.TrangThai LIKE N'%Có phép%' THEN 1 END) as SoBuoiVangCoPhep,
            COUNT(dd.MaDD) as TongSoBuoi,
            CAST(
                (COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) * 100.0) / NULLIF(COUNT(dd.MaDD), 0) 
                AS DECIMAL(5,0)
            ) as TyLeChuyenCan
        FROM HocSinh hs
        LEFT JOIN DiemDanh dd ON hs.MaHS = dd.MaHS AND CAST(dd.NgayDD AS DATE) BETWEEN @StartDate AND @EndDate
        WHERE hs.MaLop = @maLop
        GROUP BY hs.MaHS, hs.HoTen
        ORDER BY hs.HoTen";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@maLop", maLop);
            da.SelectCommand.Parameters.AddWithValue("@hocKy", hocKy);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
    public static DataTable GetMonHocByGiaoVienAndLop(string maGV, string maLop)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = @"
                SELECT DISTINCT t.MaMon, m.TenMon 
                FROM ThoiKhoaBieu t
                JOIN MonHoc m ON t.MaMon = m.MaMon
                WHERE t.MaGV = @maGV AND t.MaLop = @maLop AND t.MaMon IS NOT NULL
                ORDER BY m.TenMon";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@maGV", maGV);
            da.SelectCommand.Parameters.AddWithValue("@maLop", maLop);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
    // Add these new methods anywhere inside the DatabaseHelper class

    public static DataTable GetAllLopHoc()
    {
        string sql = "SELECT MaLop, TenLop, Khoi FROM LopHoc ORDER BY Khoi, TenLop";
        return ExecuteQuery(sql);
    }

    // Dán 3 hàm mới này vào bất kỳ đâu bên trong class DatabaseHelper

    /// <summary>
    /// Lấy danh sách các giáo viên chưa được phân công làm GVCN cho bất kỳ lớp nào.
    /// </summary>
    public static DataTable GetUnassignedHomeroomTeachers()
    {
        string sql = @"
            SELECT MaGV, Ten 
            FROM GiaoVien 
            WHERE TrangThai = N'Đã xác nhận' AND MaGV NOT IN (SELECT DISTINCT MaGVCN FROM LopHoc WHERE MaGVCN IS NOT NULL)";
        return ExecuteQuery(sql);
    }

    /// <summary>
    /// Cập nhật giáo viên chủ nhiệm cho một lớp học.
    /// </summary>
    public static void UpdateGvcnForLop(string maLop, string maGV)
    {
        // Nếu maGV là null hoặc rỗng, ta gỡ bỏ GVCN khỏi lớp
        string sql = "UPDATE LopHoc SET MaGVCN = @MaGV WHERE MaLop = @MaLop";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaLop", maLop);
                if (string.IsNullOrEmpty(maGV))
                {
                    cmd.Parameters.AddWithValue("@MaGV", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@MaGV", maGV);
                }
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Ghi đè hàm cũ để xử lý trường hợp GVCN là NULL
    /// </summary>
    public static new DataRow GetLopHocDetails(string maLop)
    {
        string sql = $@"
            SELECT 
                l.MaLop, l.TenLop, l.Khoi, l.NamHoc, 
                ISNULL(gv.Ten, N'Chưa có') AS TenGVCN,
                (SELECT COUNT(*) FROM HocSinh WHERE MaLop = l.MaLop) AS SiSo
            FROM LopHoc l
            LEFT JOIN GiaoVien gv ON l.MaGVCN = gv.MaGV
            WHERE l.MaLop = '{maLop}'";
        DataTable dt = ExecuteQuery(sql);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public static DataTable GetPhanCongGiangDayByLop(string maLop)
    {
        string sql = @"
            -- Lấy tất cả các môn học
            SELECT 
                m.MaMon,
                m.TenMon,
                Assigned.MaGV,
                ISNULL(Assigned.TenGV, 'Chưa phân công') AS TenGV
            FROM MonHoc m
            -- Ghép với thông tin các giáo viên ĐÃ ĐƯỢC phân công cho lớp này
            LEFT JOIN (
                SELECT g.MaMon, g.MaGV, g.Ten as TenGV
                FROM PhanCongGiangDay pc
                JOIN GiaoVien g ON pc.MaGV = g.MaGV
                WHERE pc.MaLop = @MaLop
            ) AS Assigned ON m.MaMon = Assigned.MaMon
            ORDER BY m.TenMon";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@MaLop", maLop);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    // Tìm và thay thế toàn bộ hàm UpdatePhanCong cũ bằng hàm này
    public static void UpdatePhanCong(string maLop, string maMon, string newMaGV)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            // Bắt đầu một transaction để đảm bảo cả hai lệnh (xóa và thêm) cùng thành công hoặc thất bại
            using (SqlTransaction tran = conn.BeginTransaction())
            {
                try
                {
                    // Bước 1: Tìm và xóa phân công cũ cho môn học này trong lớp này.
                    // Tức là tìm giáo viên hiện tại dạy môn này và xóa họ khỏi bảng PhanCongGiangDay của lớp.
                    string findOldGvSql = @"
                        SELECT pc.MaGV 
                        FROM PhanCongGiangDay pc
                        JOIN GiaoVien g ON pc.MaGV = g.MaGV
                        WHERE pc.MaLop = @MaLop AND g.MaMon = @MaMon";

                    string oldMaGV = null;
                    using (SqlCommand findCmd = new SqlCommand(findOldGvSql, conn, tran))
                    {
                        findCmd.Parameters.AddWithValue("@MaLop", maLop);
                        findCmd.Parameters.AddWithValue("@MaMon", maMon);
                        var result = findCmd.ExecuteScalar();
                        if (result != null)
                        {
                            oldMaGV = result.ToString();
                        }
                    }

                    // Nếu tìm thấy giáo viên cũ, xóa phân công của họ
                    if (!string.IsNullOrEmpty(oldMaGV))
                    {
                        string deleteSql = "DELETE FROM PhanCongGiangDay WHERE MaLop = @MaLop AND MaGV = @OldMaGV";
                        using (SqlCommand deleteCmd = new SqlCommand(deleteSql, conn, tran))
                        {
                            deleteCmd.Parameters.AddWithValue("@MaLop", maLop);
                            deleteCmd.Parameters.AddWithValue("@OldMaGV", oldMaGV);
                            deleteCmd.ExecuteNonQuery();
                        }
                    }

                    // Bước 2: Nếu có giáo viên mới được chọn (không phải 'Trống'), thêm phân công mới.
                    if (!string.IsNullOrEmpty(newMaGV))
                    {
                        string insertSql = "INSERT INTO PhanCongGiangDay (MaGV, MaLop) VALUES (@NewMaGV, @MaLop)";
                        using (SqlCommand insertCmd = new SqlCommand(insertSql, conn, tran))
                        {
                            insertCmd.Parameters.AddWithValue("@NewMaGV", newMaGV);
                            insertCmd.Parameters.AddWithValue("@MaLop", maLop);
                            insertCmd.ExecuteNonQuery();
                        }
                    }

                    // Hoàn tất và lưu thay đổi
                    tran.Commit();
                }
                catch (Exception)
                {
                    // Nếu có lỗi, hoàn tác tất cả thay đổi
                    tran.Rollback();
                    throw; // Ném lỗi ra ngoài để C# có thể bắt và thông báo
                }
            }
        }
    }
    public static ImportResult ImportHocSinhToLop(DataTable dt, string maLopTarget)
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
                        if (!DateTime.TryParse(row["NgaySinh"]?.ToString().Trim(), out ngaySinh))
                        {
                            result.Failed++;
                            continue;
                        }

                        string check = "SELECT COUNT(1) FROM HocSinh WHERE MaHS=@MaHS";
                        using (SqlCommand chk = new SqlCommand(check, conn, tran))
                        {
                            chk.Parameters.AddWithValue("@MaHS", maHS);
                            if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
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
                            cmd.Parameters.AddWithValue("@MaLop", maLopTarget);
                            cmd.Parameters.AddWithValue("@HoTen", row["HoTen"]?.ToString().Trim() ?? "");
                            cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                            cmd.Parameters.AddWithValue("@GioiTinh", row["GioiTinh"]?.ToString().Trim() ?? "");
                            cmd.Parameters.AddWithValue("@SDT", row["SDTPhuHuynh"]?.ToString().Trim() ?? "");
                            cmd.Parameters.AddWithValue("@DiaChi", row["DiaChi"]?.ToString().Trim() ?? "");
                            cmd.Parameters.AddWithValue("@DanToc", row["DanToc"]?.ToString().Trim() ?? "");
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
    public static ImportResult ImportThoiKhoaBieuForGV(string maGV, DataTable dt)
    {
        var result = new ImportResult();
        var allMonHoc = GetAllMonHoc().AsEnumerable();
        var allLopHoc = GetAllLopHoc().AsEnumerable();

        var datesInExcel = dt.AsEnumerable()
                             .Select(row => {
                                 DateTime date;
                                 if (DateTime.TryParse(row["Ngay"]?.ToString(), out date))
                                     return (DateTime?)date.Date;
                                 return null;
                             })
                             .Where(d => d.HasValue)
                             .Select(d => d.Value)
                             .Distinct().ToList();

        if (!datesInExcel.Any())
        {
            result.Failed = dt.Rows.Count;
            return result;
        }

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlTransaction tran = conn.BeginTransaction())
            {
                try
                {
                    string deleteSql = "DELETE FROM ThoiKhoaBieu WHERE MaGV = @MaGV AND CAST(Ngay AS DATE) IN ({0})";
                    string dateParams = string.Join(",", datesInExcel.Select((d, i) => $"@date{i}"));

                    using (SqlCommand deleteCmd = new SqlCommand(string.Format(deleteSql, dateParams), conn, tran))
                    {
                        deleteCmd.Parameters.AddWithValue("@MaGV", maGV);
                        for (int i = 0; i < datesInExcel.Count; i++)
                        {
                            deleteCmd.Parameters.AddWithValue($"@date{i}", datesInExcel[i]);
                        }
                        deleteCmd.ExecuteNonQuery();
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        try
                        {
                            DateTime ngay = Convert.ToDateTime(row["Ngay"]);
                            int tiet = Convert.ToInt32(row["Tiet"]);
                            string tenMon = row["TenMon"]?.ToString().Trim();
                            string tenLop = row["TenLop"]?.ToString().Trim();
                            string ghiChu = row["GhiChu"]?.ToString().Trim();
                            string mauSac = row["MauSac"]?.ToString().Trim();

                            var monRow = allMonHoc.FirstOrDefault(m => m.Field<string>("TenMon").Equals(tenMon, StringComparison.OrdinalIgnoreCase));
                            var lopRow = allLopHoc.FirstOrDefault(l => l.Field<string>("TenLop").Equals(tenLop, StringComparison.OrdinalIgnoreCase));

                            if (monRow == null || lopRow == null)
                            {
                                result.Failed++;
                                continue;
                            }

                            string maMon = monRow["MaMon"].ToString();
                            string maLop = lopRow["MaLop"].ToString();

                            string insertSql = @"INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, MaMon, MaLop, GhiChu, MauSac, MaGV) 
                                                 VALUES (@MaTKB, @Ngay, @Tiet, @MaMon, @MaLop, @GhiChu, @MauSac, @MaGV)";

                            using (SqlCommand insertCmd = new SqlCommand(insertSql, conn, tran))
                            {
                                insertCmd.Parameters.AddWithValue("@MaTKB", "TKB" + Guid.NewGuid().ToString("N").Substring(0, 7));
                                insertCmd.Parameters.AddWithValue("@Ngay", ngay.Date);
                                insertCmd.Parameters.AddWithValue("@Tiet", tiet);
                                insertCmd.Parameters.AddWithValue("@MaMon", maMon);
                                insertCmd.Parameters.AddWithValue("@MaLop", maLop);
                                insertCmd.Parameters.AddWithValue("@GhiChu", string.IsNullOrEmpty(ghiChu) ? (object)DBNull.Value : ghiChu);
                                insertCmd.Parameters.AddWithValue("@MauSac", string.IsNullOrEmpty(mauSac) ? (object)DBNull.Value : mauSac);
                                insertCmd.Parameters.AddWithValue("@MaGV", maGV);
                                insertCmd.ExecuteNonQuery();
                            }
                            result.Success++;
                        }
                        catch
                        {
                            result.Failed++;
                        }
                    }
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        return result;
    }
    // Dán hàm này vào bất kỳ đâu trong class DatabaseHelper
    public static string GetTeacherNameById(string maGV)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT Ten FROM GiaoVien WHERE MaGV = @MaGV";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaGV", maGV);
                object result = cmd.ExecuteScalar();
                return result?.ToString() ?? "Không rõ";
            }
        }
    }
}