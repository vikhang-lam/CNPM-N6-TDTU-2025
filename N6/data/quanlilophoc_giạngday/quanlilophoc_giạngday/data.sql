
CREATE DATABASE quanlilophoc_giangday;
GO
USE quanlilophoc_giangday;
GO
PRINT 'ĐÃ TẠO DATABASE MỚI: quanlilophoc_giangday (CLEAN VERSION).';

--================================================================
-- BƯỚC 1: TẠO CÁC BẢNG (STRUCTURE)
--================================================================
CREATE TABLE Admin (
    MaAdmin VARCHAR(10) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    Password VARCHAR(30) NOT NULL,
    Email NVARCHAR(50) NOT NULL
);

CREATE TABLE MonHoc (
    MaMon VARCHAR(10) PRIMARY KEY,
    TenMon NVARCHAR(100) NOT NULL
);

CREATE TABLE Minigame (
    MaMNG VARCHAR(10) PRIMARY KEY,
    Ten NVARCHAR(100) NOT NULL,
    DuLieu NVARCHAR(MAX)
);

CREATE TABLE GiaoVien (
    MaGV VARCHAR(10) PRIMARY KEY,
    Ten NVARCHAR(100) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    Password VARCHAR(30) NOT NULL,
    Email NVARCHAR(50),
    SDT VARCHAR(15),
    MaAdmin VARCHAR(10),
    AnhDaiDien NVARCHAR(200),
    TrangThai NVARCHAR(20) DEFAULT N'Chưa xác nhận',
	ResetOTP VARCHAR(6) NULL,       
    OTPExpiry DATETIME NULL,
    FOREIGN KEY (MaAdmin) REFERENCES Admin(MaAdmin)
);

CREATE TABLE GiaoVien_MonHoc (
    MaGV VARCHAR(10) NOT NULL,
    MaMon VARCHAR(10) NOT NULL,
    PRIMARY KEY (MaGV, MaMon),
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV) ON DELETE CASCADE,
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon) ON DELETE CASCADE
);

CREATE TABLE LopHoc (
    MaLop VARCHAR(10) PRIMARY KEY,
    TenLop NVARCHAR(50) NOT NULL,
    Khoi NVARCHAR(20),
    NamHoc VARCHAR(10),
    MaGVCN VARCHAR(10),
    FOREIGN KEY (MaGVCN) REFERENCES GiaoVien(MaGV)
);

CREATE TABLE HocSinh (
    MaHS VARCHAR(10) PRIMARY KEY,
    MaLop VARCHAR(10),
    HoTen NVARCHAR(100) NOT NULL,
    DanToc NVARCHAR(50),
    GioiTinh NVARCHAR(10),
    SDTPhuHuynh VARCHAR(15),
    DiaChi NVARCHAR(200),
    NgaySinh DATE,
    FOREIGN KEY (MaLop) REFERENCES LopHoc(MaLop)
);

CREATE TABLE PhanCongGiangDay (
    MaGV VARCHAR(10) NOT NULL,
    MaLop VARCHAR(10) NOT NULL,
    MaMon VARCHAR(10) NOT NULL,
    PRIMARY KEY (MaLop, MaMon), 
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV) ON DELETE CASCADE,
    FOREIGN KEY (MaLop) REFERENCES LopHoc(MaLop) ON DELETE CASCADE,
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon) ON DELETE CASCADE
);

CREATE TABLE DiemDanh (
    MaDD VARCHAR(10) PRIMARY KEY,
    MaHS VARCHAR(10),
    NgayDD DATETIME  DEFAULT GETDATE(),
    Buoi NVARCHAR(10),
    TrangThai NVARCHAR(20),
	ThoiGianCapNhat DATETIME,
    FOREIGN KEY (MaHS) REFERENCES HocSinh(MaHS) ON DELETE CASCADE
);

CREATE TABLE KetQuaHocTap (
    MaKQ VARCHAR(10) PRIMARY KEY,
    MaMon VARCHAR(10),
    MaHS VARCHAR(10),
    NgayNhap DATE,
    NhanXet NVARCHAR(200),
    GhiChu NVARCHAR(200),
    Loai NVARCHAR(20),
    Diem FLOAT,
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon),
    FOREIGN KEY (MaHS) REFERENCES HocSinh(MaHS) ON DELETE CASCADE
);

CREATE TABLE TaiLieu (
    MaTL VARCHAR(10) PRIMARY KEY,
    TenTL NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(200),
    Kieu NVARCHAR(200),
    NgayTaiLen DATE,
    TrangThaiChiaSe NVARCHAR(20),
    MaGV VARCHAR(10),
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV) ON DELETE CASCADE
);

CREATE TABLE ThoiKhoaBieu (
    MaTKB VARCHAR(10) PRIMARY KEY,
    Ngay DATE,
    Tiet INT,
    MaMon VARCHAR(10),
    GhiChu NVARCHAR(200),
    MaGV VARCHAR(10),
    MaLop VARCHAR(10),
    MauSac VARCHAR(20) NULL,
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon),
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV),
    FOREIGN KEY (MaLop) REFERENCES LopHoc(MaLop)
);

CREATE TABLE BaoCao (
    MaBC VARCHAR(10) PRIMARY KEY,
    TenBC NVARCHAR(100),
    ThoiGian DATETIME,
    DinhDang NVARCHAR(20),
    TenFolder NVARCHAR(100),
    MaGV VARCHAR(10),
    NoiDung NVARCHAR(500),
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV)
);

CREATE TABLE QuyLop (
    MaQL VARCHAR(10) PRIMARY KEY,
    MaLop VARCHAR(10),
    SoTien DECIMAL(12,2),
    Ngay DATE,
    GhiChu NVARCHAR(200),
    Loai NVARCHAR(10),
    FOREIGN KEY (MaLop) REFERENCES LopHoc(MaLop)
);

CREATE TABLE ThoiHanDiem (
    MaCotDiem VARCHAR(20) NOT NULL,
    TenHienThi NVARCHAR(100) NOT NULL,
    Khoi NVARCHAR(20) NOT NULL,
    HocKy INT NOT NULL,
    NgayMoDiem DATE,
    NgayKhoaDiem DATE,
    KhoaThuCong BIT NOT NULL DEFAULT 0,
    PRIMARY KEY (MaCotDiem, Khoi, HocKy)
);

CREATE TABLE TaiLieu_ChiaSe_GiaoVien (
    MaTL VARCHAR(10) NOT NULL,
    MaGV VARCHAR(10) NOT NULL,
    PRIMARY KEY (MaTL, MaGV),
    FOREIGN KEY (MaTL) REFERENCES TaiLieu(MaTL) ON DELETE CASCADE,
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV) ON DELETE NO ACTION 
);
GO

CREATE TABLE NamHoc (
    MaNamHoc VARCHAR(10) PRIMARY KEY, 
    TenNamHoc NVARCHAR(50) NOT NULL, 
    IsCurrent BIT DEFAULT 0
);
GO

CREATE TABLE HoSoLuuTru (
    MaHoSo VARCHAR(50) PRIMARY KEY,
    MaHS VARCHAR(10),
    MaNamHoc VARCHAR(10),
    MaLop VARCHAR(10),
    MaGVCN VARCHAR(10),
    DiemTB_CuoiNam FLOAT,
    KetQua NVARCHAR(50),
    NgayLuuTru DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaHS) REFERENCES HocSinh(MaHS),
    FOREIGN KEY (MaNamHoc) REFERENCES NamHoc(MaNamHoc)
);
GO

CREATE TABLE ChiTietDiemLuuTru (
    ID BIGINT IDENTITY(1,1) PRIMARY KEY,
    MaHoSo VARCHAR(50),      
    MaMon VARCHAR(10),       
    LoaiDiem VARCHAR(20),
    Diem FLOAT,
    NhanXet NVARCHAR(200),
    FOREIGN KEY (MaHoSo) REFERENCES HoSoLuuTru(MaHoSo),
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon)
);
GO

PRINT 'ĐÃ TẠO TẤT CẢ CÁC BẢNG.';
GO

--================================================================
-- BƯỚC 2: KHỞI TẠO DỮ LIỆU CẦN THIẾT (SYSTEM DATA ONLY)
--================================================================

-- 1. Năm Học (Giữ lại năm hiện tại để hệ thống hoạt động)
INSERT INTO NamHoc(MaNamHoc, TenNamHoc, IsCurrent) VALUES ('2024-2025', N'Năm học 2024 - 2025', 1);

-- 2. Admin Mặc định (Cần thiết để đăng nhập lần đầu)
INSERT INTO Admin (MaAdmin, Username, Password, Email) VALUES ('AD001', 'admin', '123456', 'admin@example.com');

-- 3. Danh mục Môn Học chuẩn (Giữ lại để đỡ phải nhập tay)
INSERT INTO MonHoc (MaMon, TenMon) VALUES 
('TV', N'Tiếng Việt'), ('TOAN', N'Toán'), ('KH', N'Khoa học'), ('LS_DL', N'Lịch sử và Địa lí'),
('ANH', N'Tiếng Anh'), ('DD', N'Đạo đức'), ('AN', N'Âm nhạc'), ('MT', N'Mĩ thuật'),
('TIN', N'Tin học và Công nghệ (Tin học)'), ('CN', N'Tin học và Công nghệ (Công nghệ)'),
('GDTC', N'Giáo dục thể chất'), ('TDT', N'Tiếng dân tộc'), ('HDTN', N'Hoạt động trải nghiệm');

-- 4. Cấu hình Minigame (Dữ liệu hệ thống)
INSERT INTO Minigame (MaMNG, Ten, DuLieu) VALUES
('MNG01', N'Quiz nhanh', NULL), ('MNG02', N'Gọi tên ngẫu nhiên', NULL), ('MNG03', N'Flashcard', NULL),
('MNG04', N'Ghép chữ', NULL), ('MNG05', N'Nghe - chọn hình', NULL), ('MNG06', N'Sắp xếp câu', NULL),
('MNG07', N'Điền từ', NULL), ('MNG08', N'Lật thẻ', NULL), ('MNG09', N'Random số', NULL), ('MNG10', N'Pass a ball', NULL);

-- 5. Cấu hình Thời Hạn Điểm Mặc Định (Cần thiết để Trigger nhập điểm hoạt động)
-- (Tạo cho cả 5 khối)

DECLARE @khoi INT = 1;
WHILE @khoi <= 5
BEGIN
    DECLARE @tenKhoi NVARCHAR(20) = N'Khối ' + CAST(@khoi AS NVARCHAR);
    
    INSERT INTO ThoiHanDiem (MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong)
    VALUES
    -- HỌC KỲ 1
    ('Thang1_Ki1', N'Điểm Tháng 1 (Kỳ 1)', @tenKhoi, 1, '2024-09-01', '2024-09-30', 0),
    ('Thang2_Ki1', N'Điểm Tháng 2 (Kỳ 1)', @tenKhoi, 1, '2024-10-01', '2024-10-31', 0),
    ('Thang3_Ki1', N'Điểm Tháng 3 (Kỳ 1)', @tenKhoi, 1, '2024-11-01', '2024-11-30', 0),
    ('GiuaKi1',    N'Điểm Giữa Kỳ 1',      @tenKhoi, 1, '2024-10-15', '2024-11-20', 0),
    ('CuoiKi1',    N'Điểm Cuối Kỳ 1',      @tenKhoi, 1, '2024-12-15', '2025-01-15', 0),
    
    -- HỌC KỲ 2
    ('Thang1_Ki2', N'Điểm Tháng 1 (Kỳ 2)', @tenKhoi, 2, '2025-01-20', '2025-02-28', 0),
    ('Thang2_Ki2', N'Điểm Tháng 2 (Kỳ 2)', @tenKhoi, 2, '2025-03-01', '2025-03-31', 0),
    ('Thang3_Ki2', N'Điểm Tháng 3 (Kỳ 2)', @tenKhoi, 2, '2025-04-01', '2025-04-30', 0),
    ('GiuaKi2',    N'Điểm Giữa Kỳ 2',      @tenKhoi, 2, '2025-03-15', '2025-04-20', 0),
    ('CuoiKi2',    N'Điểm Cuối Kỳ 2',      @tenKhoi, 2, '2025-05-10', '2025-06-10', 0);

    SET @khoi = @khoi + 1;
END
GO

PRINT 'ĐÃ KHỞI TẠO DỮ LIỆU CẤU HÌNH CẦN THIẾT.';
GO

--================================================================
-- BƯỚC 3: TẠO TYPES & TRIGGERS
--================================================================

-- Types
CREATE TYPE ut_HocSinhImport AS TABLE(
    MaHS VARCHAR(10) PRIMARY KEY, MaLop VARCHAR(10) NULL, HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE NOT NULL, GioiTinh NVARCHAR(10) NULL, SDTPhuHuynh VARCHAR(15) NULL,
    DiaChi NVARCHAR(200) NULL, DanToc NVARCHAR(50) NULL
);
GO
CREATE TYPE ut_DateList AS TABLE(Ngay DATE PRIMARY KEY);
GO
CREATE TYPE ut_TKBImport AS TABLE(
    Ngay DATE, Tiet INT, TenMon NVARCHAR(100), TenLop NVARCHAR(50), GhiChu NVARCHAR(200) NULL, MauSac VARCHAR(20) NULL
);
GO
CREATE TYPE ut_MaMonList AS TABLE(MaMon VARCHAR(10) PRIMARY KEY);
GO
CREATE TYPE ut_MaHSList AS TABLE(MaHS VARCHAR(10) PRIMARY KEY);
GO

-- Triggers
CREATE TRIGGER trg_TaoDiemDanhHocSinhMoi ON HocSinh AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @today DATE = CAST(GETDATE() AS DATE);
    INSERT INTO DiemDanh (MaDD, MaHS, NgayDD, Buoi, TrangThai)
    SELECT LEFT(NEWID(), 8), i.MaHS, @today, N'Sáng', N'Có mặt' FROM INSERTED i;
END;
GO

CREATE TRIGGER trg_TaoKetQuaHocTapHocSinhMoi ON HocSinh AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @loai TABLE (Loai NVARCHAR(20));
    INSERT INTO @loai (Loai) SELECT DISTINCT MaCotDiem FROM ThoiHanDiem;

    IF NOT EXISTS (SELECT 1 FROM @loai) RETURN;

    INSERT INTO KetQuaHocTap (MaKQ, MaMon, MaHS, NgayNhap, Loai)
    SELECT LEFT(NEWID(), 8), m.MaMon, i.MaHS, GETDATE(), l.Loai
    FROM INSERTED i CROSS JOIN MonHoc m CROSS JOIN @loai l;
END;
GO

CREATE TRIGGER trg_UpdateDiemDanhTimestamp ON DiemDanh AFTER UPDATE
AS
BEGIN
    IF UPDATE(TrangThai)
    BEGIN
        UPDATE DiemDanh SET ThoiGianCapNhat = GETDATE()
        FROM DiemDanh INNER JOIN inserted ON DiemDanh.MaDD = inserted.MaDD;
    END
END;
GO

PRINT 'ĐÃ TẠO TYPES & TRIGGERS.';
GO

--================================================================
-- BƯỚC 4: TẠO STORED PROCEDURES (GIỮ NGUYÊN TOÀN BỘ LOGIC)
--================================================================

-- 1. LOGIN
create PROCEDURE sp_CheckTeacherLogin 
    @user NVARCHAR(50), 
    @pass VARCHAR(30) 
AS
BEGIN 
    SELECT TrangThai 
    FROM GiaoVien 
    WHERE Username = @user COLLATE Latin1_General_CS_AS 
      AND Password = @pass COLLATE Latin1_General_CS_AS; 
END;
GO

create PROCEDURE sp_CheckAdminLogin 
    @user NVARCHAR(50), 
    @pass VARCHAR(30) 
AS
BEGIN 
    SELECT COUNT(*) 
    FROM Admin 
    WHERE Username = @user COLLATE Latin1_General_CS_AS 
      AND Password = @pass COLLATE Latin1_General_CS_AS; 
END;
GO

CREATE PROCEDURE sp_CreateTeacherRequest @Ten NVARCHAR(100), @Username NVARCHAR(50), @Password VARCHAR(30), @Email NVARCHAR(50), @SDT VARCHAR(15) AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM GiaoVien WHERE Username=@Username) BEGIN RAISERROR(N'Tên đăng nhập này đã tồn tại.', 16, 1); RETURN; END
    IF EXISTS (SELECT 1 FROM GiaoVien WHERE Email=@Email) BEGIN RAISERROR(N'Email này đã tồn tại.', 16, 1); RETURN; END
    DECLARE @newId INT; SELECT @newId = ISNULL(MAX(CAST(SUBSTRING(MaGV, 3, LEN(MaGV)) AS INT)), 0) + 1 FROM GiaoVien;
    INSERT INTO GiaoVien (MaGV, Ten, Username, Password, Email, SDT, MaAdmin, TrangThai) 
    VALUES ('GV' + RIGHT('00' + CAST(@newId AS VARCHAR), 3), @Ten, @Username, @Password, @Email, @SDT, 'AD001', N'Chưa xác nhận');
END;
GO
CREATE PROCEDURE sp_RequestPasswordReset @UsernameOrEmail NVARCHAR(50) AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Email NVARCHAR(50), @MaGV VARCHAR(10), @OTP VARCHAR(6);
    SELECT @Email = Email, @MaGV = MaGV FROM GiaoVien WHERE (Username = @UsernameOrEmail OR Email = @UsernameOrEmail) AND TrangThai = N'Đã xác nhận';
    IF @MaGV IS NOT NULL BEGIN
        SET @OTP = CAST(FLOOR(RAND() * (999999 - 100000 + 1) + 100000) AS VARCHAR(6));
        UPDATE GiaoVien SET ResetOTP = @OTP, OTPExpiry = DATEADD(minute, 10, GETDATE()) WHERE MaGV = @MaGV;
        SELECT @Email AS Email, @OTP AS OTP; RETURN;
    END
    SELECT NULL AS Email, NULL AS OTP;
END;
GO
CREATE PROCEDURE sp_ResetPasswordWithOtp @UsernameOrEmail NVARCHAR(50), @OTP VARCHAR(6), @NewPassword VARCHAR(30) AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaGV VARCHAR(10), @StoredOTP VARCHAR(6), @Expiry DATETIME;
    SELECT @MaGV = MaGV, @StoredOTP = ResetOTP, @Expiry = OTPExpiry FROM GiaoVien WHERE (Username = @UsernameOrEmail OR Email = @UsernameOrEmail) AND TrangThai = N'Đã xác nhận';
    IF @MaGV IS NULL BEGIN SELECT 0; RETURN; END
    IF @StoredOTP IS NULL OR @StoredOTP != @OTP BEGIN SELECT 1; RETURN; END
    IF GETDATE() > @Expiry BEGIN SELECT 2; RETURN; END
    UPDATE GiaoVien SET Password = @NewPassword, ResetOTP = NULL, OTPExpiry = NULL WHERE MaGV = @MaGV;
    SELECT 100;
END;
GO
CREATE PROCEDURE sp_CheckOtp @UsernameOrEmail NVARCHAR(50), @OTP VARCHAR(6) AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaGV VARCHAR(10), @StoredOtp VARCHAR(6), @OtpExpiry DATETIME;
    SELECT TOP 1 @MaGV = MaGV, @StoredOtp = ResetOTP, @OtpExpiry = OTPExpiry FROM GiaoVien WHERE (Username = @UsernameOrEmail OR Email = @UsernameOrEmail) AND TrangThai = N'Đã xác nhận';
    IF @MaGV IS NULL BEGIN SELECT 0 AS Result; RETURN; END
    IF @StoredOtp IS NULL OR @StoredOtp != @OTP BEGIN SELECT 1 AS Result; RETURN; END
    IF @OtpExpiry < GETDATE() BEGIN SELECT 2 AS Result; RETURN; END
    SELECT 100 AS Result;
END
GO

-- 2. PROFILE
CREATE PROCEDURE sp_GetTeacherProfile @user NVARCHAR(50) AS
BEGIN
    SET NOCOUNT ON;
    SELECT gv.Ten, gv.Email, gv.SDT, gv.AnhDaiDien, ISNULL(STUFF((SELECT N', ' + mh.TenMon FROM GiaoVien_MonHoc gvm JOIN MonHoc mh ON gvm.MaMon = mh.MaMon WHERE gvm.MaGV = gv.MaGV ORDER BY mh.TenMon FOR XML PATH('')), 1, 2, N''), N'Chưa có môn') AS TenMon
    FROM GiaoVien gv WHERE gv.Username = @user OR gv.Ten = @user;
END;
GO
CREATE PROCEDURE sp_GetTeacherNameById @MaGV VARCHAR(10) AS BEGIN SELECT Ten FROM GiaoVien WHERE MaGV = @MaGV; END;
GO
CREATE PROCEDURE sp_GetMaGVByUsername @u NVARCHAR(50) AS BEGIN SELECT MaGV FROM GiaoVien WHERE Username=@u OR Ten=@u; END;
GO
CREATE PROCEDURE sp_UpdateTeacherProfile @u NVARCHAR(50), @e NVARCHAR(50), @s VARCHAR(15), @a NVARCHAR(200) AS BEGIN UPDATE GiaoVien SET Email=@e, SDT=@s, AnhDaiDien=@a WHERE Username=@u; END;
GO
CREATE PROCEDURE sp_UpdateTeacherAvatar @user NVARCHAR(50), @avatar NVARCHAR(200) AS BEGIN UPDATE GiaoVien SET AnhDaiDien = @avatar WHERE Username = @user OR Ten = @user; END;
GO
CREATE PROCEDURE sp_ChangeTeacherPassword @u NVARCHAR(50), @oldPass VARCHAR(30), @newPass VARCHAR(30) AS
BEGIN
    DECLARE @currentPass VARCHAR(30); SELECT @currentPass = Password FROM GiaoVien WHERE Username=@u OR Ten =@u;
    IF @currentPass IS NULL OR @currentPass != @oldPass BEGIN SELECT 0; RETURN; END
    UPDATE GiaoVien SET Password=@newPass WHERE Username=@u OR Ten =@u; SELECT 1;
END;
GO
CREATE PROCEDURE sp_GetAdminProfile @u NVARCHAR(50) AS BEGIN SELECT * FROM Admin WHERE Username=@u; END;
GO
CREATE PROCEDURE sp_GetAdminEmail @MaAdmin VARCHAR(10) AS BEGIN SET NOCOUNT ON; SELECT Email FROM Admin WHERE MaAdmin = @MaAdmin; END;
GO
CREATE PROCEDURE sp_UpdateAdminEmail @u NVARCHAR(50), @e NVARCHAR(50) AS BEGIN UPDATE Admin SET Email=@e WHERE Username=@u; END;
GO
CREATE PROCEDURE sp_ChangeAdminPassword @u NVARCHAR(50), @oldPass VARCHAR(30), @newPass VARCHAR(30) AS
BEGIN
    DECLARE @currentPass VARCHAR(30); SELECT @currentPass = Password FROM Admin WHERE Username=@u;
    IF @currentPass IS NULL OR @currentPass != @oldPass BEGIN SELECT 0; RETURN; END
    UPDATE Admin SET Password=@newPass WHERE Username=@u; SELECT 1;
END;
GO

-- 3. QUẢN LÝ LỚP (GVCN)
CREATE PROCEDURE sp_TaoDiemDanhMacDinh @MaLop VARCHAR(10), @Ngay DATE = NULL, @Buoi NVARCHAR(10) = N'Sáng' AS
BEGIN
    SET NOCOUNT ON; IF @Ngay IS NULL SET @Ngay = CAST(GETDATE() AS DATE);
    INSERT INTO DiemDanh (MaDD, MaHS, NgayDD, Buoi, TrangThai)
    SELECT LEFT(NEWID(), 8), hs.MaHS, CAST(@Ngay AS DATETIME), @Buoi, N'Có mặt' FROM HocSinh hs
    WHERE hs.MaLop = @MaLop AND NOT EXISTS (SELECT 1 FROM DiemDanh dd WHERE dd.MaHS = hs.MaHS AND CAST(dd.NgayDD AS DATE) = @Ngay AND dd.Buoi = @Buoi);
END;
GO
CREATE PROCEDURE sp_GetDiemDanhByLop @maLop VARCHAR(10) AS
BEGIN
    SELECT dd.MaDD, hs.MaHS, hs.HoTen, dd.NgayDD, dd.Buoi, dd.TrangThai, dd.ThoiGianCapNhat
    FROM HocSinh hs LEFT JOIN DiemDanh dd ON hs.MaHS = dd.MaHS WHERE hs.MaLop = @maLop ORDER BY hs.HoTen, dd.NgayDD;
END;
GO
CREATE PROCEDURE sp_GetDiemDanhByLopAndDate @maLop VARCHAR(10), @ngay DATE, @buoi NVARCHAR(10) AS
BEGIN
    SELECT hs.MaHS, hs.HoTen, dd.MaDD, dd.NgayDD, dd.Buoi, dd.TrangThai, dd.ThoiGianCapNhat
    FROM HocSinh hs LEFT JOIN (SELECT MaDD, MaHS, NgayDD, Buoi, TrangThai, ThoiGianCapNhat FROM DiemDanh WHERE CAST(NgayDD AS date) = @ngay AND (@buoi IS NULL OR Buoi = @buoi)) dd ON hs.MaHS = dd.MaHS
    WHERE hs.MaLop = @maLop ORDER BY hs.HoTen;
END;
GO
CREATE PROCEDURE sp_UpsertDiemDanh @MaHS VARCHAR(10), @Ngay DATETIME, @Buoi NVARCHAR(10), @TrangThai NVARCHAR(20) AS
BEGIN
    SET NOCOUNT ON; IF @Buoi IS NULL OR @Buoi = '' SET @Buoi = N'Sáng'; IF @TrangThai IS NULL OR @TrangThai = '' SET @TrangThai = N'Có mặt';
    DECLARE @ExistingMaDD VARCHAR(10); DECLARE @NgayDate DATE = CAST(@Ngay AS DATE);
    SELECT @ExistingMaDD = MaDD FROM DiemDanh WHERE MaHS = @MaHS AND CAST(NgayDD AS DATE) = @NgayDate AND Buoi = @Buoi;
    IF @ExistingMaDD IS NOT NULL BEGIN UPDATE DiemDanh SET TrangThai = @TrangThai, NgayDD = @Ngay WHERE MaDD = @ExistingMaDD; END
    ELSE BEGIN INSERT INTO DiemDanh(MaDD, MaHS, NgayDD, Buoi, TrangThai) VALUES(LEFT(NEWID(), 8), @MaHS, @Ngay, @Buoi, @TrangThai); END
END;
GO
CREATE PROCEDURE sp_UpdateDiemDanh @MaDD VARCHAR(10), @TrangThai NVARCHAR(20) AS BEGIN UPDATE DiemDanh SET TrangThai=@TrangThai WHERE MaDD=@MaDD; END;
GO
CREATE PROCEDURE sp_TaoKetQuaHocTapMacDinh AS
BEGIN
    SET NOCOUNT ON; DECLARE @loai TABLE (Loai NVARCHAR(20)); INSERT INTO @loai (Loai) SELECT DISTINCT MaCotDiem FROM ThoiHanDiem;
    IF NOT EXISTS (SELECT 1 FROM @loai) RETURN;
    INSERT INTO KetQuaHocTap (MaKQ, MaMon, MaHS, NgayNhap, Loai)
    SELECT LEFT(NEWID(), 8), m.MaMon, hs.MaHS, GETDATE(), l.Loai
    FROM HocSinh hs CROSS JOIN MonHoc m CROSS JOIN @loai l
    WHERE NOT EXISTS (SELECT 1 FROM KetQuaHocTap kq WHERE kq.MaHS = hs.MaHS AND kq.MaMon = m.MaMon AND kq.Loai = l.Loai);
END;
GO
CREATE PROCEDURE sp_GetKetQuaHocTapByLop @malop VARCHAR(10) AS
BEGIN
    SELECT hs.MaHS, hs.HoTen, mh.TenMon, kq.Diem, kq.NhanXet, kq.GhiChu, kq.Loai
    FROM HocSinh hs INNER JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS INNER JOIN MonHoc mh ON kq.MaMon = mh.MaMon WHERE hs.MaLop = @malop ORDER BY hs.MaHS, mh.MaMon;
END;
GO
CREATE PROCEDURE sp_GetBangDiemPivot @malop VARCHAR(10), @ki INT, @maMon VARCHAR(10) AS
BEGIN
    DECLARE @khoi NVARCHAR(20); SELECT @khoi = Khoi FROM LopHoc WHERE MaLop = @malop;
    DECLARE @Thang1Loai NVARCHAR(20), @Thang2Loai NVARCHAR(20), @Thang3Loai NVARCHAR(20), @GiuaKiLoai NVARCHAR(20), @CuoiKiLoai NVARCHAR(20);
    SELECT @Thang1Loai = MaCotDiem FROM ThoiHanDiem WHERE Khoi = @khoi AND HocKy = @ki AND MaCotDiem LIKE 'Thang1%';
    SELECT @Thang2Loai = MaCotDiem FROM ThoiHanDiem WHERE Khoi = @khoi AND HocKy = @ki AND MaCotDiem LIKE 'Thang2%';
    SELECT @Thang3Loai = MaCotDiem FROM ThoiHanDiem WHERE Khoi = @khoi AND HocKy = @ki AND MaCotDiem LIKE 'Thang3%';
    SELECT @GiuaKiLoai = MaCotDiem FROM ThoiHanDiem WHERE Khoi = @khoi AND HocKy = @ki AND MaCotDiem LIKE 'GiuaKi%';
    SELECT @CuoiKiLoai = MaCotDiem FROM ThoiHanDiem WHERE Khoi = @khoi AND HocKy = @ki AND MaCotDiem LIKE 'CuoiKi%';
    SET @Thang1Loai = ISNULL(@Thang1Loai, 'Thang1_Ki' + CAST(@ki AS VARCHAR));
    SET @Thang2Loai = ISNULL(@Thang2Loai, 'Thang2_Ki' + CAST(@ki AS VARCHAR));
    SET @Thang3Loai = ISNULL(@Thang3Loai, 'Thang3_Ki' + CAST(@ki AS VARCHAR));
    SET @GiuaKiLoai = ISNULL(@GiuaKiLoai, 'GiuaKi' + CAST(@ki AS VARCHAR));
    SET @CuoiKiLoai = ISNULL(@CuoiKiLoai, 'CuoiKi' + CAST(@ki AS VARCHAR));
    DECLARE @loaiFilter NVARCHAR(10) = N'%Ki' + CAST(@ki AS VARCHAR) + N'%';
    DECLARE @sql NVARCHAR(MAX);
    SET @sql = N'SELECT hs.MaHS, hs.HoTen, lh.TenLop,
        MAX(CASE WHEN kq.Loai = @Thang1Loai THEN kq.Diem END) AS Thang1,
        MAX(CASE WHEN kq.Loai = @Thang2Loai THEN kq.Diem END) AS Thang2,
        MAX(CASE WHEN kq.Loai = @Thang3Loai THEN kq.Diem END) AS Thang3,
        MAX(CASE WHEN kq.Loai = @GiuaKiLoai THEN kq.Diem END) AS GiuaKi,
        MAX(CASE WHEN kq.Loai = @CuoiKiLoai THEN kq.Diem END) AS CuoiKi,
        MAX(CASE WHEN kq.Loai LIKE @loaiFilter THEN kq.NhanXet END) AS NhanXet,
        MAX(CASE WHEN kq.Loai LIKE @loaiFilter THEN kq.GhiChu END) AS GhiChu
    FROM HocSinh hs JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS AND kq.MaMon = @maMon
    LEFT JOIN ThoiHanDiem thd ON kq.Loai = thd.MaCotDiem AND lh.Khoi = thd.Khoi AND thd.HocKy = @ki
    WHERE hs.MaLop = @malop AND (kq.Diem IS NULL OR kq.NgayNhap >= DATEADD(day, -30, thd.NgayMoDiem))
    GROUP BY hs.MaHS, hs.HoTen, lh.TenLop ORDER BY hs.HoTen';
    EXEC sp_executesql @sql, N'@malop VARCHAR(10), @ki INT, @maMon VARCHAR(10), @loaiFilter NVARCHAR(10), @Thang1Loai NVARCHAR(20), @Thang2Loai NVARCHAR(20), @Thang3Loai NVARCHAR(20), @GiuaKiLoai NVARCHAR(20), @CuoiKiLoai NVARCHAR(20)', @malop, @ki, @maMon, @loaiFilter, @Thang1Loai, @Thang2Loai, @Thang3Loai, @GiuaKiLoai, @CuoiKiLoai;
END;
GO
CREATE PROCEDURE sp_GetHomeroomGradebook @MaLop VARCHAR(10), @LoaiDiem NVARCHAR(20) AS
BEGIN
    SET NOCOUNT ON; DECLARE @cols AS NVARCHAR(MAX), @query AS NVARCHAR(MAX);
    SELECT @cols = STUFF((SELECT DISTINCT ',' + QUOTENAME(mh.TenMon) FROM KetQuaHocTap kq JOIN MonHoc mh ON kq.MaMon = mh.MaMon JOIN HocSinh hs ON kq.MaHS = hs.MaHS WHERE hs.MaLop = @MaLop AND kq.Loai = @LoaiDiem AND kq.Diem IS NOT NULL FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'')
    IF @cols IS NULL BEGIN SELECT MaHS, HoTen FROM HocSinh WHERE MaLop = @MaLop ORDER BY HoTen; RETURN; END
    SET @query = 'SELECT MaHS, HoTen, ' + @cols + ' from (SELECT hs.MaHS, hs.HoTen, mh.TenMon, kq.Diem FROM KetQuaHocTap kq JOIN HocSinh hs ON kq.MaHS = hs.MaHS JOIN MonHoc mh ON kq.MaMon = mh.MaMon WHERE hs.MaLop = ''' + @MaLop + ''' AND kq.Loai = ''' + @LoaiDiem + ''') x pivot (MAX(Diem) for TenMon in (' + @cols + ')) p ORDER BY HoTen'
    EXECUTE(@query);
END
GO
CREATE PROCEDURE sp_InsertKetQuaHocTap @MaHS VARCHAR(10), @MaMon VARCHAR(10), @Diem FLOAT, @NhanXet NVARCHAR(200) AS BEGIN INSERT INTO KetQuaHocTap(MaKQ, MaMon, MaHS, NgayNhap, Diem, NhanXet, Loai) VALUES(LEFT(NEWID(), 8), @MaMon, @MaHS, GETDATE(), @Diem, @NhanXet, N'GiuaKi1'); END;
GO
CREATE PROCEDURE sp_UpsertKetQuaHocTap @MaHS VARCHAR(30), @MaMon VARCHAR(20), @Loai VARCHAR(30), @Diem FLOAT = NULL, @NhanXet NVARCHAR(200) = NULL, @GhiChu NVARCHAR(200) = NULL AS
BEGIN
    SET NOCOUNT ON; DECLARE @KhoiCuaHS NVARCHAR(20); SELECT @KhoiCuaHS = lh.Khoi FROM HocSinh hs JOIN LopHoc lh ON hs.MaLop = lh.MaLop WHERE hs.MaHS = @MaHS;
    IF @KhoiCuaHS IS NULL BEGIN RAISERROR (N'Học sinh chưa xếp lớp.', 16, 1); RETURN; END
    IF @Diem IS NOT NULL BEGIN
        DECLARE @DaKhoa BIT = 0; DECLARE @HomNay DATE = CAST(GETDATE() AS DATE); 
        SELECT @DaKhoa = CASE WHEN KhoaThuCong = 1 OR @HomNay > CAST(NgayKhoaDiem AS DATE) OR @HomNay < CAST(NgayMoDiem AS DATE) THEN 1 ELSE 0 END FROM ThoiHanDiem WHERE MaCotDiem = @Loai AND Khoi = @KhoiCuaHS;
        IF @DaKhoa = 1 BEGIN RAISERROR (N'Cột điểm đã khóa.', 16, 1); RETURN; END
    END
    IF EXISTS (SELECT 1 FROM KetQuaHocTap WHERE MaHS = @MaHS AND MaMon = @MaMon AND Loai = @Loai)
        UPDATE KetQuaHocTap SET Diem = ISNULL(@Diem, Diem), NhanXet = ISNULL(@NhanXet, NhanXet), GhiChu = ISNULL(@GhiChu, GhiChu), NgayNhap = GETDATE() WHERE MaHS = @MaHS AND MaMon = @MaMon AND Loai = @Loai;
    ELSE IF @Diem IS NOT NULL OR @NhanXet IS NOT NULL OR @GhiChu IS NOT NULL
        INSERT INTO KetQuaHocTap (MaKQ, MaHS, MaMon, Loai, Diem, NhanXet, GhiChu, NgayNhap) VALUES (LEFT(NEWID(), 8), @MaHS, @MaMon, @Loai, @Diem, @NhanXet, @GhiChu, GETDATE());
END
GO
CREATE PROCEDURE sp_GetQuyLopByLop @MaLop VARCHAR(10) AS BEGIN SELECT MaQL, Ngay, GhiChu, Loai, SoTien FROM QuyLop WHERE MaLop = @MaLop ORDER BY Ngay, MaQL; END;
GO
CREATE PROCEDURE sp_InsertQuyLop @MaLop VARCHAR(10), @Loai NVARCHAR(10), @SoTien DECIMAL(12,2), @Ngay DATE, @GhiChu NVARCHAR(200) AS BEGIN INSERT INTO QuyLop (MaQL, MaLop, Loai, SoTien, Ngay, GhiChu) VALUES (LEFT(NEWID(), 10), @MaLop, @Loai, @SoTien, @Ngay, @GhiChu); END;
GO
CREATE PROCEDURE sp_DeleteQuyLop @MaQL VARCHAR(10) AS BEGIN DELETE FROM QuyLop WHERE MaQL = @MaQL; END;
GO

-- 4. GIẢNG DẠY
CREATE PROCEDURE sp_GetTKBByGV @MaGV VARCHAR(10), @Monday DATE, @Sunday DATE AS
BEGIN
    SELECT t.Ngay, t.Tiet, ISNULL(m.TenMon, '') AS TenMon, ISNULL(l.TenLop, '') AS TenLop, ISNULL(t.GhiChu, '') AS GhiChu, ISNULL(t.MauSac, '') AS MauSac
    FROM ThoiKhoaBieu t LEFT JOIN MonHoc m ON t.MaMon = m.MaMon LEFT JOIN LopHoc l ON t.MaLop = l.MaLop WHERE t.MaGV = @MaGV AND t.Ngay >= @Monday AND t.Ngay <= @Sunday;
END;
GO
CREATE PROCEDURE sp_UpsertTKBColor @MaGV VARCHAR(10), @Ngay DATE, @Tiet INT, @MauSac VARCHAR(20) AS
BEGIN
    SET NOCOUNT ON; IF EXISTS(SELECT 1 FROM ThoiKhoaBieu WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet) UPDATE ThoiKhoaBieu SET MauSac=@MauSac WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet;
    ELSE INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, MauSac, MaGV) VALUES (LEFT(NEWID(), 10), @Ngay, @Tiet, @MauSac, @MaGV);
END;
GO
CREATE PROCEDURE sp_UpsertTKBGhiChu @MaGV VARCHAR(10), @Ngay DATE, @Tiet INT, @Note NVARCHAR(200) AS
BEGIN
    SET NOCOUNT ON; IF EXISTS(SELECT 1 FROM ThoiKhoaBieu WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet) UPDATE ThoiKhoaBieu SET GhiChu=@Note WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet;
    ELSE INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, GhiChu, MaGV) VALUES (LEFT(NEWID(), 10), @Ngay, @Tiet, @Note, @MaGV);
END;
GO
CREATE PROCEDURE sp_DeleteTKBGhiChu @MaGV VARCHAR(10), @Ngay DATE, @Tiet INT AS
BEGIN
    UPDATE ThoiKhoaBieu SET GhiChu = NULL WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet;
END;
GO
CREATE PROCEDURE sp_DeleteTKBEntry @MaGV VARCHAR(10), @Ngay DATE, @Tiet INT AS BEGIN DELETE FROM ThoiKhoaBieu WHERE MaGV = @MaGV AND Ngay = @Ngay AND Tiet = @Tiet; END
GO
CREATE PROCEDURE sp_DeleteTKBByWeek @MaGV VARCHAR(10), @Monday DATE AS BEGIN DELETE FROM ThoiKhoaBieu WHERE MaGV = @MaGV AND Ngay >= @Monday AND Ngay <= DATEADD(day, 6, @Monday); END
GO
CREATE PROCEDURE [dbo].[sp_ImportTKBForGV] @MaGV VARCHAR(10), @NgayList ut_DateList READONLY, @TKBData ut_TKBImport READONLY AS
BEGIN
    SET NOCOUNT ON; DECLARE @DefaultColor VARCHAR(20) = '#4682B4';
    SELECT t.Ngay, t.Tiet, m.MaMon, l.MaLop, NULLIF(t.GhiChu, '') AS GhiChu, COALESCE(NULLIF(t.MauSac, ''), @DefaultColor) AS MauSac INTO #ProcessedTKB
    FROM @TKBData t LEFT JOIN MonHoc m ON t.TenMon = m.TenMon LEFT JOIN LopHoc l ON t.TenLop = l.TenLop WHERE CAST(t.Ngay AS DATE) IN (SELECT Ngay FROM @NgayList);
    DECLARE @Failed INT = 0; DECLARE @Success INT = 0;
    SELECT @Failed = COUNT(*) FROM #ProcessedTKB WHERE MaMon IS NULL OR MaLop IS NULL;
    SELECT @Success = COUNT(*) FROM #ProcessedTKB WHERE MaMon IS NOT NULL AND MaLop IS NOT NULL;
    IF @Success > 0 BEGIN
        BEGIN TRANSACTION;
        DELETE TKB FROM ThoiKhoaBieu TKB INNER JOIN @NgayList DL ON CAST(TKB.Ngay AS DATE) = DL.Ngay WHERE TKB.MaGV = @MaGV;
        INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, MaMon, MaLop, GhiChu, MauSac, MaGV) SELECT LEFT(NEWID(), 10), p.Ngay, p.Tiet, p.MaMon, p.MaLop, p.GhiChu, p.MauSac, @MaGV FROM #ProcessedTKB p WHERE p.MaMon IS NOT NULL AND p.MaLop IS NOT NULL;
        COMMIT TRANSACTION;
    END
    SELECT @Success AS [Success], @Failed AS [Failed]; DROP TABLE #ProcessedTKB;
END;
GO
CREATE PROCEDURE sp_GetTaiLieuByGV @gv VARCHAR(10) AS BEGIN SELECT MaTL, TenTL, MoTa, Kieu, NgayTaiLen, TrangThaiChiaSe FROM TaiLieu WHERE MaGV=@gv; END;
GO
CREATE PROCEDURE sp_InsertTaiLieu @gv VARCHAR(10), @ten NVARCHAR(100), @moTa NVARCHAR(200), @kieu NVARCHAR(200), @tt NVARCHAR(20) AS BEGIN INSERT INTO TaiLieu (MaTL, TenTL, MoTa, Kieu, NgayTaiLen, TrangThaiChiaSe, MaGV) VALUES(LEFT(NEWID(), 10), @ten, @moTa, @kieu, GETDATE(), @tt, @gv); END;
GO
CREATE PROCEDURE sp_DeleteTaiLieu @id VARCHAR(10) AS BEGIN DELETE FROM TaiLieu WHERE MaTL=@id; END;
GO
CREATE PROCEDURE sp_ShareTaiLieu @id VARCHAR(10) AS BEGIN UPDATE TaiLieu SET TrangThaiChiaSe=N'Chia sẻ' WHERE MaTL=@id; END;
GO
CREATE PROCEDURE sp_UnshareTaiLieu @MaTL VARCHAR(10) AS BEGIN UPDATE TaiLieu SET TrangThaiChiaSe = N'Riêng tư' WHERE MaTL = @MaTL; END;
GO
CREATE PROCEDURE sp_GetTaiLieuShared AS BEGIN SELECT MaTL, TenTL, MoTa, Kieu, NgayTaiLen, TrangThaiChiaSe FROM TaiLieu WHERE TrangThaiChiaSe=N'Chia sẻ'; END;
GO
CREATE PROCEDURE sp_GetTaiLieuSharedWithUploader AS BEGIN SELECT tl.MaTL, tl.TenTL, tl.MoTa, tl.Kieu, tl.NgayTaiLen, gv.Ten AS TenGV FROM TaiLieu tl INNER JOIN GiaoVien gv ON tl.MaGV = gv.MaGV WHERE tl.TrangThaiChiaSe = N'Chia sẻ'; END;
GO
CREATE PROCEDURE sp_GetAllTeachersForSharing @MaGvOwner VARCHAR(10) AS BEGIN SELECT MaGV, Ten FROM GiaoVien WHERE MaGV != @MaGvOwner AND TrangThai = N'Đã xác nhận' ORDER BY Ten; END;
GO
CREATE PROCEDURE sp_GetDocumentStatus @MaTL VARCHAR(10) AS BEGIN SELECT TrangThaiChiaSe FROM TaiLieu WHERE MaTL = @MaTL; END;
GO
CREATE PROCEDURE sp_GetSharedWithTeachers @MaTL VARCHAR(10) AS BEGIN SELECT MaGV FROM TaiLieu_ChiaSe_GiaoVien WHERE MaTL = @MaTL; END;
GO
CREATE PROCEDURE sp_UpdateDocumentSharing @MaTL VARCHAR(10), @TrangThai NVARCHAR(20), @GiaoVienList ut_MaHSList READONLY AS
BEGIN
    BEGIN TRANSACTION; UPDATE TaiLieu SET TrangThaiChiaSe = @TrangThai WHERE MaTL = @MaTL; DELETE FROM TaiLieu_ChiaSe_GiaoVien WHERE MaTL = @MaTL;
    IF @TrangThai = N'Giáo viên cụ thể' INSERT INTO TaiLieu_ChiaSe_GiaoVien (MaTL, MaGV) SELECT @MaTL, MaHS FROM @GiaoVienList;
    COMMIT TRANSACTION;
END;
GO
CREATE PROCEDURE sp_GetSharedDocumentsForTeacher @MaGV_HienTai VARCHAR(10) AS
BEGIN
    SELECT tl.MaTL, tl.TenTL, tl.MoTa, tl.Kieu, tl.NgayTaiLen, gv.Ten AS TenGV, N'Công khai' AS LoaiChiaSe FROM TaiLieu tl INNER JOIN GiaoVien gv ON tl.MaGV = gv.MaGV WHERE tl.TrangThaiChiaSe = N'Chia sẻ' AND tl.MaGV != @MaGV_HienTai
    UNION
    SELECT tl.MaTL, tl.TenTL, tl.MoTa, tl.Kieu, tl.NgayTaiLen, gv.Ten AS TenGV, N'Chia sẻ riêng' AS LoaiChiaSe FROM TaiLieu_ChiaSe_GiaoVien tsgv INNER JOIN TaiLieu tl ON tsgv.MaTL = tl.MaTL INNER JOIN GiaoVien gv ON tl.MaGV = gv.MaGV WHERE tsgv.MaGV = @MaGV_HienTai;
END;
GO
CREATE PROCEDURE sp_GetMiniGames AS BEGIN SELECT MaMNG, Ten, DuLieu FROM Minigame ORDER BY MaMNG; END;
GO
CREATE PROCEDURE sp_GetGameData @maMNG VARCHAR(10) AS BEGIN SELECT DuLieu FROM Minigame WHERE MaMNG = @maMNG; END;
GO
CREATE PROCEDURE sp_SaveGameData @maMNG VARCHAR(10), @data NVARCHAR(MAX) AS BEGIN UPDATE Minigame SET DuLieu = @data WHERE MaMNG = @maMNG; END;
GO
CREATE PROCEDURE sp_AddGhiChuTKB @MaGV VARCHAR(10), @MaLop VARCHAR(10), @Ngay DATE, @Tiet INT, @GhiChu NVARCHAR(200) AS
BEGIN
    IF EXISTS(SELECT 1 FROM ThoiKhoaBieu WHERE MaLop = @MaLop AND Ngay = @Ngay AND Tiet = @Tiet) UPDATE ThoiKhoaBieu SET GhiChu = ISNULL(GhiChu, '') + NCHAR(13) + NCHAR(10) + @GhiChu, MaGV = @MaGV WHERE MaLop = @MaLop AND Ngay = @Ngay AND Tiet = @Tiet;
    ELSE INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, GhiChu, MaGV, MaLop) VALUES (LEFT(NEWID(), 10), @Ngay, @Tiet, @GhiChu, @MaGV, @MaLop);
END;
GO
CREATE PROCEDURE sp_AddGhiChuChoHocSinh @MaHS VARCHAR(10), @MaMon VARCHAR(10), @GhiChu NVARCHAR(200), @HocKy INT AS
BEGIN
    DECLARE @Loai NVARCHAR(20) = 'CuoiKi' + CAST(@HocKy AS NVARCHAR(1));
    IF EXISTS (SELECT 1 FROM KetQuaHocTap WHERE MaHS = @MaHS AND MaMon = @MaMon AND Loai = @Loai) UPDATE KetQuaHocTap SET GhiChu = @GhiChu, NgayNhap = GETDATE() WHERE MaHS = @MaHS AND MaMon = @MaMon AND Loai = @Loai;
    ELSE INSERT INTO KetQuaHocTap (MaKQ, MaMon, MaHS, NgayNhap, GhiChu, Loai) VALUES (LEFT(NEWID(), 8), @MaMon, @MaHS, GETDATE(), @GhiChu, @Loai);
END;
GO

-- 5. BÁO CÁO
CREATE PROCEDURE sp_GetLopByGiaoVien @maGV VARCHAR(10) AS BEGIN SELECT DISTINCT l.MaLop, l.TenLop FROM LopHoc l JOIN PhanCongGiangDay pc ON l.MaLop = pc.MaLop WHERE pc.MaGV = @maGV UNION SELECT MaLop, TenLop FROM LopHoc WHERE MaGVCN = @maGV; END;
GO
CREATE PROCEDURE sp_GetHomeroomClassesByTeacher @maGV VARCHAR(10) AS BEGIN SELECT MaLop, TenLop FROM LopHoc WHERE MaGVCN = @maGV; END;
GO
CREATE PROCEDURE sp_GetHomeroomClassNameByTeacherId @MaGV VARCHAR(10) AS BEGIN SELECT TenLop FROM LopHoc WHERE MaGVCN = @maGV; END;
GO
CREATE PROCEDURE sp_GetMonHocByGiaoVienAndLop @maGV VARCHAR(10), @maLop VARCHAR(10) AS BEGIN SELECT DISTINCT m.MaMon, m.TenMon FROM PhanCongGiangDay pc JOIN MonHoc m ON pc.MaMon = m.MaMon WHERE pc.MaGV = @maGV AND pc.MaLop = @maLop ORDER BY m.TenMon; END;
GO
CREATE PROCEDURE sp_GetBangDiemHocKy @maLop VARCHAR(10), @hocKy INT AS
BEGIN
    DECLARE @monHocCols NVARCHAR(MAX), @monHocColsSelect NVARCHAR(MAX), @tongMon NVARCHAR(MAX), @sql NVARCHAR(MAX), @monCount INT, @khoi NVARCHAR(20); SELECT @khoi = Khoi FROM LopHoc WHERE MaLop = @maLop;
    SELECT @monHocCols = STUFF((SELECT DISTINCT ',' + QUOTENAME(mh.TenMon) FROM PhanCongGiangDay pcg JOIN MonHoc mh ON pcg.MaMon = mh.MaMon WHERE pcg.MaLop = @maLop ORDER BY 1 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'');
    SELECT @monHocColsSelect = STUFF((SELECT DISTINCT ',ROUND(ISNULL(' + QUOTENAME(mh.TenMon) + ', 0), 2) AS ' + QUOTENAME(mh.TenMon) FROM PhanCongGiangDay pcg JOIN MonHoc mh ON pcg.MaMon = mh.MaMon WHERE pcg.MaLop = @maLop ORDER BY 1 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'');
    SELECT @tongMon = STUFF((SELECT DISTINCT ' + ISNULL(' + QUOTENAME(mh.TenMon) + ', 0)' FROM PhanCongGiangDay pcg JOIN MonHoc mh ON pcg.MaMon = mh.MaMon WHERE pcg.MaLop = @maLop ORDER BY 1 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,3,'');
    SELECT @monCount = COUNT(DISTINCT MaMon) FROM PhanCongGiangDay WHERE MaLop = @maLop;
    IF @monCount = 0 OR @monHocCols IS NULL BEGIN SELECT MaHS, HoTen FROM HocSinh WHERE MaLop = @maLop; RETURN; END;
    DECLARE @loaiFilter NVARCHAR(10) = CASE WHEN @hocKy = 3 THEN N'%' ELSE CAST(@hocKy AS NVARCHAR) END;
    SET @sql = N';WITH DiemTB AS ( SELECT hs.MaHS, hs.HoTen, lh.TenLop, mh.TenMon, AVG(kq.Diem) AS DiemTB FROM HocSinh hs INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop INNER JOIN PhanCongGiangDay pcg ON hs.MaLop = pcg.MaLop INNER JOIN MonHoc mh ON pcg.MaMon = mh.MaMon LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS AND mh.MaMon = kq.MaMon LEFT JOIN ThoiHanDiem thd ON kq.Loai = thd.MaCotDiem AND lh.Khoi = thd.Khoi WHERE hs.MaLop = @maLop AND kq.Loai IN (SELECT MaCotDiem FROM ThoiHanDiem WHERE Khoi = @khoi AND CAST(HocKy AS NVARCHAR) LIKE @loaiFilter) AND (kq.Diem IS NULL OR kq.NgayNhap >= DATEADD(day, -60, thd.NgayMoDiem)) GROUP BY hs.MaHS, hs.HoTen, lh.TenLop, mh.TenMon ), PivotData AS ( SELECT MaHS, HoTen, TenLop, ' + @monHocCols + N' FROM DiemTB PIVOT (AVG(DiemTB) FOR TenMon IN (' + @monHocCols + N')) AS PivotTable ) SELECT MaHS, HoTen, ' + @monHocColsSelect + N', ROUND((' + @tongMon + N') / NULLIF(' + CAST(@monCount AS VARCHAR) + N', 0), 2) AS [Trung bình chung] FROM PivotData ORDER BY HoTen;';
    EXEC sp_executesql @sql, N'@maLop VARCHAR(10), @khoi NVARCHAR(20), @loaiFilter NVARCHAR(10)', @maLop, @khoi, @loaiFilter;
END;
GO
CREATE PROCEDURE sp_GetHoSoHocSinh @maLop VARCHAR(10) AS BEGIN SELECT MaHS, HoTen, GioiTinh, NgaySinh, DanToc, DiaChi, SDTPhuHuynh FROM HocSinh WHERE MaLop = @maLop ORDER BY HoTen; END;
GO
CREATE PROCEDURE sp_GetBaoCaoChuyenCan @maLop VARCHAR(10), @hocKy INT AS
BEGIN
    DECLARE @CurrentDate DATE = GETDATE(); DECLARE @CurrentMonth INT = MONTH(@CurrentDate); DECLARE @NamHocStartYear INT = CASE WHEN @CurrentMonth >= 8 THEN YEAR(@CurrentDate) ELSE YEAR(@CurrentDate) - 1 END;
    DECLARE @StartDate DATE, @EndDate DATE;
    IF @hocKy = 1 BEGIN SET @StartDate = DATEFROMPARTS(@NamHocStartYear, 8, 1); SET @EndDate = DATEFROMPARTS(@NamHocStartYear, 12, 31); END ELSE IF @hocKy = 2 BEGIN SET @StartDate = DATEFROMPARTS(@NamHocStartYear + 1, 1, 1); SET @EndDate = DATEFROMPARTS(@NamHocStartYear + 1, 5, 31); END ELSE BEGIN SET @StartDate = DATEFROMPARTS(@NamHocStartYear, 8, 1); SET @EndDate = DATEFROMPARTS(@NamHocStartYear + 1, 5, 31); END;
    SELECT hs.MaHS, hs.HoTen, COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) as SoBuoiCoMat, COUNT(CASE WHEN dd.TrangThai = N'Vắng' THEN 1 END) as SoBuoiVang, COUNT(CASE WHEN dd.TrangThai LIKE N'%Có phép%' THEN 1 END) as SoBuoiVangCoPhep, COUNT(dd.MaDD) as TongSoBuoi, CAST((COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) * 100.0) / NULLIF(COUNT(dd.MaDD), 0) AS DECIMAL(5,0)) as TyLeChuyenCan FROM HocSinh hs LEFT JOIN DiemDanh dd ON hs.MaHS = dd.MaHS AND CAST(dd.NgayDD AS DATE) BETWEEN @StartDate AND @EndDate WHERE hs.MaLop = @maLop GROUP BY hs.MaHS, hs.HoTen ORDER BY hs.HoTen;
END;
GO
CREATE PROCEDURE sp_GetMonthlyScoreTypes @MaLop VARCHAR(10), @HocKy INT AS
BEGIN
    DECLARE @Khoi NVARCHAR(20); SELECT @Khoi = Khoi FROM LopHoc WHERE MaLop = @MaLop;
    IF @Khoi IS NULL BEGIN SELECT TOP 0 '' AS MaCotDiem, '' AS TenHienThi; RETURN; END
    SELECT MaCotDiem, TenHienThi FROM ThoiHanDiem WHERE Khoi = @Khoi AND HocKy = @HocKy AND MaCotDiem LIKE 'Thang%' ORDER BY NgayMoDiem;
END;
GO
ALTER PROCEDURE sp_GetBaoCaoThang_ThongKe 
    @MaLop VARCHAR(10), 
    @MaMon VARCHAR(10), 
    @LoaiDiem VARCHAR(20) 
AS
BEGIN
    -- Phiên bản sửa lỗi: Trả về '<5' để khớp với C# UC_Baocao.cs dòng 531
    
    ;WITH RawData AS ( 
        SELECT 
            hs.GioiTinh, 
            LTRIM(RTRIM(ISNULL(hs.DanToc, ''))) AS DanTocClean, 
            kq.Diem 
        FROM KetQuaHocTap kq 
        JOIN HocSinh hs ON kq.MaHS = hs.MaHS 
        WHERE hs.MaLop = @MaLop 
          AND kq.MaMon = @MaMon 
          AND kq.Loai = @LoaiDiem 
          AND kq.Diem IS NOT NULL 
    ), 
    ClassifiedData AS ( 
        SELECT 
            CASE 
                WHEN Diem = 10 THEN '10' 
                WHEN Diem >= 9 AND Diem < 10 THEN '9' 
                WHEN Diem >= 8 AND Diem < 9 THEN '8' 
                WHEN Diem >= 7 AND Diem < 8 THEN '7' 
                WHEN Diem >= 6 AND Diem < 7 THEN '6' 
                WHEN Diem >= 5 AND Diem < 6 THEN '5' 
                ELSE '<5'  -- QUAN TRỌNG: Phải là '<5' để khớp với code C#
            END AS NhomDiem, 
            CASE 
                WHEN Diem >= 7 THEN 'T' 
                WHEN Diem >= 5 THEN 'H' 
                ELSE 'C' 
            END AS XepLoai, 
            
            -- Logic đếm Nữ/Dân tộc
            CASE WHEN LTRIM(RTRIM(GioiTinh)) = N'Nữ' THEN 1 ELSE 0 END AS IsNu, 
            
            CASE 
                WHEN DanTocClean <> '' AND LOWER(DanTocClean) <> N'kinh' THEN 1 
                ELSE 0 
            END AS IsDanTocThieuSo, 
            
            CASE 
                WHEN LTRIM(RTRIM(GioiTinh)) = N'Nữ' AND (DanTocClean <> '' AND LOWER(DanTocClean) <> N'kinh') THEN 1 
                ELSE 0 
            END AS IsNuDanTocThieuSo 
        FROM RawData 
    ) 
    SELECT 'Diem' AS LoaiThongKe, NhomDiem AS PhanLoai, COUNT(*) AS TS, SUM(IsNu) AS Nu, SUM(IsDanTocThieuSo) AS DanToc, SUM(IsNuDanTocThieuSo) AS NDT 
    FROM ClassifiedData GROUP BY NhomDiem 
    UNION ALL 
    SELECT 'XepLoai' AS LoaiThongKe, XepLoai AS PhanLoai, COUNT(*) AS TS, SUM(IsNu) AS Nu, SUM(IsDanTocThieuSo) AS DanToc, SUM(IsNuDanTocThieuSo) AS NDT 
    FROM ClassifiedData GROUP BY XepLoai;
END;
GO
CREATE PROCEDURE sp_GetScoresForAnalysis @maGV VARCHAR(10), @phamVi NVARCHAR(20), @chiTiet NVARCHAR(50), @maMon VARCHAR(10), @hocKy INT AS
BEGIN
    DECLARE @kyFilter NVARCHAR(10) = N'%' + CAST(@hocKy AS VARCHAR); DECLARE @sql NVARCHAR(MAX);
    SET @sql = N'SELECT hs.MaHS, hs.HoTen, lh.MaLop, lh.TenLop, mh.MaMon, mh.TenMon, kq.Loai, kq.Diem FROM KetQuaHocTap kq JOIN HocSinh hs ON kq.MaHS = hs.MaHS JOIN LopHoc lh ON hs.MaLop = lh.MaLop JOIN MonHoc mh ON kq.MaMon = mh.MaMon JOIN ThoiHanDiem thd ON kq.Loai = thd.MaCotDiem AND lh.Khoi = thd.Khoi WHERE kq.Diem IS NOT NULL AND thd.HocKy = @hocKy AND kq.NgayNhap >= DATEADD(day, -60, thd.NgayMoDiem)';
    IF @phamVi = 'LopGV' SET @sql = @sql + N' AND lh.MaLop = @chiTiet'; ELSE IF @phamVi = 'Khoi' SET @sql = @sql + N' AND lh.Khoi = @chiTiet';
    IF @maMon IS NOT NULL AND @maMon != 'ALL' SET @sql = @sql + N' AND mh.MaMon = @maMon';
    EXEC sp_executesql @sql, N'@hocKy INT, @chiTiet NVARCHAR(50), @maMon VARCHAR(10)', @hocKy, @chiTiet, @maMon;
END;
GO
CREATE PROCEDURE sp_GetStudentDataForPrediction @maLop VARCHAR(10), @maMon VARCHAR(10) AS
BEGIN
    DECLARE @g1Type VARCHAR(20) = 'GiuaKi1'; DECLARE @g2Type VARCHAR(20) = 'CuoiKi1'; DECLARE @Khoi NVARCHAR(20); SELECT @Khoi = Khoi FROM LopHoc WHERE MaLop = @maLop;
    DECLARE @MinDate DATE; SELECT @MinDate = MIN(NgayMoDiem) FROM ThoiHanDiem WHERE Khoi = @Khoi; IF @MinDate IS NULL SET @MinDate = DATEFROMPARTS(YEAR(GETDATE()), 8, 1);
    ;WITH ScoresG1 AS (SELECT MaHS, Diem FROM KetQuaHocTap WHERE Loai = @g1Type AND MaMon = @maMon AND NgayNhap >= @MinDate), ScoresG2 AS (SELECT MaHS, Diem FROM KetQuaHocTap WHERE Loai = @g2Type AND MaMon = @maMon AND NgayNhap >= @MinDate), LowScores AS (SELECT MaHS, SUM(CASE WHEN Loai = @g1Type AND ISNULL(Diem, 0) < 5 THEN 1 ELSE 0 END) + SUM(CASE WHEN Loai = @g2Type AND ISNULL(Diem, 0) < 5 THEN 1 ELSE 0 END) AS NumLowScores FROM KetQuaHocTap WHERE Loai IN (@g1Type, @g2Type) AND MaMon = @maMon AND NgayNhap >= @MinDate GROUP BY MaHS), Absences AS (SELECT MaHS, COUNT(*) as TotalAbsences FROM DiemDanh WHERE TrangThai = N'Vắng' AND NgayDD >= @MinDate GROUP BY MaHS)
    SELECT hs.MaHS, hs.HoTen, lh.TenLop, ISNULL(g1.Diem, 0) AS G1, ISNULL(g2.Diem, 0) AS G2, ISNULL(ls.NumLowScores, 0) AS NumLowScores, ISNULL(ab.TotalAbsences, 0) AS Absences FROM HocSinh hs INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop LEFT JOIN ScoresG1 g1 ON hs.MaHS = g1.MaHS LEFT JOIN ScoresG2 g2 ON hs.MaHS = g2.MaHS LEFT JOIN LowScores ls ON hs.MaHS = ls.MaHS LEFT JOIN Absences ab ON hs.MaHS = ab.MaHS WHERE hs.MaLop = @maLop;
END;
GO
CREATE PROCEDURE sp_GetAllKetQuaHocTap AS BEGIN SELECT hs.MaHS, hs.HoTen, lh.MaLop, lh.TenLop, mh.MaMon, mh.TenMon, kq.Loai, kq.Diem FROM KetQuaHocTap kq JOIN HocSinh hs ON kq.MaHS = hs.MaHS JOIN LopHoc lh ON hs.MaLop = lh.MaLop JOIN MonHoc mh ON kq.MaMon = mh.MaMon WHERE kq.Diem IS NOT NULL; END;
GO

-- 6. ADMIN
CREATE PROCEDURE sp_GetAllGiaoVien AS
BEGIN
    SELECT gv.MaGV, gv.Ten, gv.Username, gv.Email, gv.SDT, gv.TrangThai, ISNULL(lh.TenLop, N'') AS LopChuNhiem, ISNULL(STUFF((SELECT N', ' + mh.TenMon FROM GiaoVien_MonHoc gvm JOIN MonHoc mh ON gvm.MaMon = mh.MaMon WHERE gvm.MaGV = gv.MaGV ORDER BY mh.TenMon FOR XML PATH('')), 1, 2, N''), N'Chưa có môn') AS CacMonDay
    FROM GiaoVien gv LEFT JOIN LopHoc lh ON gv.MaGV = lh.MaGVCN;
END;
GO
CREATE PROCEDURE sp_GetGiaoVienByTrangThai @tt NVARCHAR(20) AS
BEGIN
    SELECT gv.MaGV, gv.Ten, gv.Username, gv.Email, gv.SDT, gv.TrangThai, ISNULL(lh.TenLop, N'') AS LopChuNhiem, ISNULL(STUFF((SELECT N', ' + mh.TenMon FROM GiaoVien_MonHoc gvm JOIN MonHoc mh ON gvm.MaMon = mh.MaMon WHERE gvm.MaGV = gv.MaGV ORDER BY mh.TenMon FOR XML PATH('')), 1, 2, N''), N'Chưa có môn') AS CacMonDay
    FROM GiaoVien gv LEFT JOIN LopHoc lh ON gv.MaGV = lh.MaGVCN WHERE gv.TrangThai = @tt;
END;
GO
CREATE PROCEDURE sp_UpdateTrangThaiGiaoVien @id VARCHAR(10), @tt NVARCHAR(20), @Email NVARCHAR(50) OUTPUT, @Ten NVARCHAR(100) OUTPUT AS BEGIN UPDATE GiaoVien SET TrangThai=@tt WHERE MaGV=@id; SELECT @Email = Email, @Ten = Ten FROM GiaoVien WHERE MaGV = @id; END;
GO
CREATE PROCEDURE sp_UpdateGiaoVien @id VARCHAR(10), @t NVARCHAR(100), @e NVARCHAR(50), @s VARCHAR(15) AS
BEGIN
    IF EXISTS (SELECT 1 FROM GiaoVien WHERE Email = @e AND MaGV != @id) BEGIN RAISERROR(N'Email này đã được sử dụng.', 16, 1); RETURN; END
    UPDATE GiaoVien SET Ten=@t, Email=@e, SDT=@s WHERE MaGV=@id;
END;
GO
CREATE PROCEDURE sp_DeleteGiaoVien @id VARCHAR(10) AS
BEGIN
    DECLARE @Err NVARCHAR(MAX) = N''; IF EXISTS(SELECT 1 FROM PhanCongGiangDay WHERE MaGV=@id) SET @Err = N'giảng dạy, '; IF EXISTS(SELECT 1 FROM LopHoc WHERE MaGVCN=@id) SET @Err = @Err + N'chủ nhiệm.';
    IF @Err != N'' BEGIN RAISERROR(N'Không thể xóa giáo viên đang %s', 16, 1, @Err); RETURN; END
    DELETE FROM GiaoVien WHERE MaGV = @id;
END;
GO
CREATE PROCEDURE sp_GetMonHocByGiaoVien @MaGV VARCHAR(10) AS BEGIN SELECT GM.MaMon, MH.TenMon, PC.MaLop FROM GiaoVien_MonHoc GM INNER JOIN MonHoc MH ON GM.MaMon = MH.MaMon LEFT JOIN PhanCongGiangDay PC ON GM.MaGV = PC.MaGV AND GM.MaMon = PC.MaMon WHERE GM.MaGV = @MaGV; END;
GO
CREATE PROCEDURE sp_UpdateGiaoVien_MonHoc @MaGV VARCHAR(10), @MonHocList ut_MaMonList READONLY AS
BEGIN
    BEGIN TRANSACTION; DELETE FROM GiaoVien_MonHoc WHERE MaGV = @MaGV; INSERT INTO GiaoVien_MonHoc (MaGV, MaMon) SELECT @MaGV, MaMon FROM @MonHocList WHERE MaMon IS NOT NULL AND MaMon != ''; COMMIT TRANSACTION;
END;
GO
CREATE PROCEDURE sp_UpdateTeacherSubjects @MaGV VARCHAR(10), @MonHocList ut_MaMonList READONLY AS BEGIN DELETE FROM GiaoVien_MonHoc WHERE MaGV = @MaGV; INSERT INTO GiaoVien_MonHoc (MaGV, MaMon) SELECT @MaGV, MaMon FROM @MonHocList; END;
GO
CREATE PROCEDURE sp_CheckClassExists @MaLop NVARCHAR(10) AS BEGIN SELECT COUNT(1) FROM LopHoc WHERE MaLop = @MaLop END;
GO
CREATE PROCEDURE sp_CheckClassHasStudents @MaLop NVARCHAR(10) AS BEGIN SELECT COUNT(1) FROM HocSinh WHERE MaLop = @MaLop END;
GO
CREATE PROCEDURE sp_GetAvailableGrades AS BEGIN SELECT DISTINCT Khoi FROM LopHoc ORDER BY Khoi END;
GO
CREATE PROCEDURE sp_InsertLopHoc @MaLop NVARCHAR(10), @TenLop NVARCHAR(50), @Khoi NVARCHAR(20), @NamHoc NVARCHAR(10) AS
BEGIN
    IF EXISTS (SELECT 1 FROM LopHoc WHERE MaLop = @MaLop) BEGIN RAISERROR('Mã lớp đã tồn tại!', 16, 1) RETURN END
    INSERT INTO LopHoc (MaLop, TenLop, Khoi, NamHoc) VALUES (@MaLop, @TenLop, @Khoi, @NamHoc)
END;
GO
create PROCEDURE sp_DeleteLopHoc 
    @MaLop NVARCHAR(10) 
AS
BEGIN
    BEGIN TRANSACTION;
    
    -- 1. Kiểm tra lớp có tồn tại không
    IF NOT EXISTS (SELECT 1 FROM LopHoc WHERE MaLop = @MaLop) 
    BEGIN 
        RAISERROR(N'Lớp không tồn tại!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN; 
    END

    -- 2. Kiểm tra an toàn: Không cho xóa nếu lớp đang có Học sinh
    IF EXISTS (SELECT 1 FROM HocSinh WHERE MaLop = @MaLop) 
    BEGIN 
        RAISERROR(N'Không thể xóa lớp vì lớp đang có học sinh! Hãy chuyển học sinh đi trước.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN; 
    END

    -- 3. XÓA CÁC DỮ LIỆU LIÊN QUAN (Dọn dẹp sạch sẽ trước khi xóa lớp)
    
    -- Xóa Phân công giảng dạy
    DELETE FROM PhanCongGiangDay WHERE MaLop = @MaLop;
    
    -- [MỚI] Xóa Thời khóa biểu của lớp này
    DELETE FROM ThoiKhoaBieu WHERE MaLop = @MaLop;

    -- [MỚI] Xóa Quỹ lớp (nếu có)
    DELETE FROM QuyLop WHERE MaLop = @MaLop;

    -- 4. Cuối cùng: Xóa Lớp học
    DELETE FROM LopHoc WHERE MaLop = @MaLop;

    COMMIT TRANSACTION;
END;
GO
CREATE PROCEDURE sp_GetAllHocSinh AS BEGIN SELECT MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc FROM HocSinh ORDER BY MaLop, HoTen; END;
GO
CREATE PROCEDURE sp_GetHocSinhByLop @malop VARCHAR(10) AS BEGIN SELECT ROW_NUMBER() OVER (ORDER BY HoTen) AS STT, MaHS, HoTen, GioiTinh, NgaySinh, DiaChi, DanToc, SDTPhuHuynh FROM HocSinh WHERE MaLop = @malop ORDER BY HoTen; END;
GO
CREATE PROCEDURE sp_GetHocSinhProfile @maHS VARCHAR(10) AS BEGIN SELECT * FROM HocSinh WHERE MaHS=@maHS; END;
GO
CREATE PROCEDURE sp_InsertHocSinh @MaHS VARCHAR(10), @MaLop VARCHAR(10), @HoTen NVARCHAR(100), @NgaySinh DATE, @GioiTinh NVARCHAR(10), @SDT VARCHAR(15), @DiaChi NVARCHAR(200), @DanToc NVARCHAR(50) AS
BEGIN
    IF EXISTS (SELECT 1 FROM HocSinh WHERE MaHS=@MaHS) BEGIN RAISERROR(N'Học sinh đã tồn tại.', 16, 1); RETURN; END
    INSERT INTO HocSinh (MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc) VALUES (@MaHS, @MaLop, @HoTen, @NgaySinh, @GioiTinh, @SDT, @DiaChi, @DanToc);
END;
GO
CREATE PROCEDURE sp_UpdateHocSinh @MaHS VARCHAR(10), @HoTen NVARCHAR(100), @NgaySinh DATE, @GioiTinh NVARCHAR(10), @SDT VARCHAR(15), @DiaChi NVARCHAR(200), @DanToc NVARCHAR(50) AS BEGIN UPDATE HocSinh SET HoTen=@HoTen, NgaySinh=@NgaySinh, GioiTinh=@GioiTinh, SDTPhuHuynh=@SDT, DiaChi=@DiaChi, DanToc=@DanToc WHERE MaHS=@MaHS; END;
GO
CREATE PROCEDURE sp_UpdateHocSinhProfile @MaHS VARCHAR(10), @HoTen NVARCHAR(100), @GioiTinh NVARCHAR(10), @NgaySinh DATE, @DiaChi NVARCHAR(200) AS BEGIN UPDATE HocSinh SET HoTen=@HoTen, GioiTinh=@GioiTinh, NgaySinh=@NgaySinh, DiaChi=@DiaChi WHERE MaHS=@MaHS; END;
GO
create PROCEDURE sp_DeleteHocSinh 
    @MaHS VARCHAR(10) 
AS 
BEGIN 
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- 1. Xóa Chi tiết điểm lưu trữ (Bảng con của HoSoLuuTru)
        -- Phải xóa cái này trước vì nó tham chiếu đến HoSoLuuTru
        DELETE ct 
        FROM ChiTietDiemLuuTru ct
        INNER JOIN HoSoLuuTru hslt ON ct.MaHoSo = hslt.MaHoSo
        WHERE hslt.MaHS = @MaHS;

        -- 2. Xóa Hồ sơ lưu trữ (Lịch sử các năm cũ)
        DELETE FROM HoSoLuuTru WHERE MaHS = @MaHS;

        -- 3. Xóa Học sinh
        -- (Lưu ý: Các bảng DiemDanh và KetQuaHocTap đã có ON DELETE CASCADE trong data.sql nên sẽ tự mất)
        DELETE FROM HocSinh WHERE MaHS = @MaHS;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        -- Báo lỗi chi tiết nếu có trục trặc khác
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi xóa học sinh: %s', 16, 1, @ErrMsg);
    END CATCH
END;
GO
CREATE PROCEDURE sp_UpdateHocSinhLop @MaHS VARCHAR(10), @MaLopMoi VARCHAR(10) AS BEGIN UPDATE HocSinh SET MaLop = @MaLopMoi WHERE MaHS = @MaHS; END;
GO
CREATE PROCEDURE sp_UpdateHocSinhLop_Multi @MaHSList ut_MaHSList READONLY, @MaLopMoi VARCHAR(10) AS BEGIN UPDATE HocSinh SET MaLop = @MaLopMoi WHERE MaHS IN (SELECT MaHS FROM @MaHSList); SELECT @@ROWCOUNT AS SoHocSinhDaChuyen; END;
GO
CREATE PROCEDURE sp_ImportHocSinh @HocSinhData ut_HocSinhImport READONLY AS
BEGIN
    SET NOCOUNT ON; DECLARE @MaxID INT; SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaHS, 3, 10) AS INT)), 0) FROM HocSinh WHERE MaHS LIKE 'HS%' AND ISNUMERIC(SUBSTRING(MaHS, 3, 10)) = 1;
    SELECT * INTO #TempHocSinh FROM @HocSinhData; DECLARE @Skipped INT; SELECT @Skipped = COUNT(t.HoTen) FROM #TempHocSinh t INNER JOIN HocSinh hs ON t.HoTen = hs.HoTen AND t.NgaySinh = hs.NgaySinh;
    INSERT INTO HocSinh (MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc)
    SELECT 'HS' + RIGHT('000' + CAST((@MaxID + ROW_NUMBER() OVER (ORDER BY (SELECT NULL))) AS VARCHAR), 3), t.MaLop, t.HoTen, t.NgaySinh, t.GioiTinh, t.SDTPhuHuynh, t.DiaChi, t.DanToc
    FROM #TempHocSinh t WHERE NOT EXISTS (SELECT 1 FROM HocSinh hs WHERE hs.HoTen = t.HoTen AND hs.NgaySinh = t.NgaySinh);
    DECLARE @Success INT = @@ROWCOUNT; SELECT @Success AS [Success], @Skipped AS [Skipped]; DROP TABLE #TempHocSinh;
END;
GO
CREATE PROCEDURE sp_ImportHocSinhToLop @MaLopTarget VARCHAR(10), @HocSinhData ut_HocSinhImport READONLY AS
BEGIN
    SET NOCOUNT ON; DECLARE @MaxID INT; SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaHS, 3, 10) AS INT)), 0) FROM HocSinh WHERE MaHS LIKE 'HS%' AND ISNUMERIC(SUBSTRING(MaHS, 3, 10)) = 1;
    SELECT * INTO #TempHocSinh2 FROM @HocSinhData; DECLARE @Skipped INT; SELECT @Skipped = COUNT(t.HoTen) FROM #TempHocSinh2 t INNER JOIN HocSinh hs ON t.HoTen = hs.HoTen AND t.NgaySinh = hs.NgaySinh;
    INSERT INTO HocSinh (MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc)
    SELECT 'HS' + RIGHT('000' + CAST((@MaxID + ROW_NUMBER() OVER (ORDER BY (SELECT NULL))) AS VARCHAR), 3), @MaLopTarget, t.HoTen, t.NgaySinh, t.GioiTinh, t.SDTPhuHuynh, t.DiaChi, t.DanToc
    FROM #TempHocSinh2 t WHERE NOT EXISTS (SELECT 1 FROM HocSinh hs WHERE hs.HoTen = t.HoTen AND hs.NgaySinh = t.NgaySinh);
    DECLARE @Success INT = @@ROWCOUNT; SELECT @Success AS [Success], @Skipped AS [Skipped]; DROP TABLE #TempHocSinh2;
END;
GO
CREATE PROCEDURE sp_GetAllLopHoc AS BEGIN SELECT MaLop, TenLop, Khoi FROM LopHoc ORDER BY Khoi, TenLop; END;
GO
CREATE PROCEDURE sp_GetLopHocDetails @MaLop VARCHAR(10) AS BEGIN SELECT l.MaLop, l.TenLop, l.Khoi, l.NamHoc, ISNULL(gv.Ten, N'Chưa có') AS TenGVCN, l.MaGVCN, (SELECT COUNT(*) FROM HocSinh WHERE MaLop = l.MaLop) AS SiSo FROM LopHoc l LEFT JOIN GiaoVien gv ON l.MaGVCN = gv.MaGV WHERE l.MaLop = @MaLop; END;
GO
CREATE PROCEDURE sp_GetUnassignedHomeroomTeachers AS BEGIN SELECT MaGV, Ten FROM GiaoVien WHERE TrangThai = N'Đã xác nhận' AND MaGV NOT IN (SELECT DISTINCT MaGVCN FROM LopHoc WHERE MaGVCN IS NOT NULL); END;
GO
CREATE PROCEDURE sp_UpdateGvcnForLop @MaLop VARCHAR(10), @MaGV VARCHAR(10) AS BEGIN UPDATE LopHoc SET MaGVCN = @MaGV WHERE MaLop = @MaLop; END;
GO
CREATE PROCEDURE sp_GetPhanCongGiangDayByLop @MaLop VARCHAR(10) AS BEGIN SELECT m.MaMon, m.TenMon, ISNULL(pc.MaGV, '') AS MaGV, ISNULL(gv.Ten, 'Chưa phân công') AS TenGV FROM MonHoc m LEFT JOIN PhanCongGiangDay pc ON m.MaMon = pc.MaMon AND pc.MaLop = @MaLop LEFT JOIN GiaoVien gv ON pc.MaGV = gv.MaGV ORDER BY m.TenMon; END;
GO
CREATE PROCEDURE sp_UpdatePhanCong @MaLop VARCHAR(10), @MaMon VARCHAR(10), @NewMaGV VARCHAR(10) AS
BEGIN
    BEGIN TRANSACTION; DELETE FROM PhanCongGiangDay WHERE MaLop = @MaLop AND MaMon = @MaMon;
    IF @NewMaGV IS NOT NULL AND @NewMaGV != '' BEGIN
        IF NOT EXISTS (SELECT 1 FROM GiaoVien_MonHoc WHERE MaGV = @NewMaGV AND MaMon = @MaMon) BEGIN RAISERROR(N'Giáo viên không dạy môn này.', 16, 1); ROLLBACK TRANSACTION; RETURN; END
        INSERT INTO PhanCongGiangDay (MaGV, MaLop, MaMon) VALUES (@NewMaGV, @MaLop, @MaMon);
    END
    COMMIT TRANSACTION;
END;
GO
CREATE PROCEDURE sp_GetAllMonHoc AS BEGIN SELECT MaMon, TenMon FROM MonHoc ORDER BY TenMon; END;
GO
CREATE FUNCTION [dbo].[fu_TiengVietKhongDau](@strInput NVARCHAR(MAX)) RETURNS NVARCHAR(MAX) AS
BEGIN
    IF @strInput IS NULL RETURN @strInput; SET @strInput = LOWER(@strInput);
    SET @strInput = REPLACE(@strInput, N'á', N'a'); SET @strInput = REPLACE(@strInput, N'à', N'a'); SET @strInput = REPLACE(@strInput, N'ả', N'a'); SET @strInput = REPLACE(@strInput, N'ã', N'a'); SET @strInput = REPLACE(@strInput, N'ạ', N'a'); SET @strInput = REPLACE(@strInput, N'ă', N'a'); SET @strInput = REPLACE(@strInput, N'ắ', N'a'); SET @strInput = REPLACE(@strInput, N'ằ', N'a'); SET @strInput = REPLACE(@strInput, N'ẳ', N'a'); SET @strInput = REPLACE(@strInput, N'ẵ', N'a'); SET @strInput = REPLACE(@strInput, N'ặ', N'a'); SET @strInput = REPLACE(@strInput, N'â', N'a'); SET @strInput = REPLACE(@strInput, N'ấ', N'a'); SET @strInput = REPLACE(@strInput, N'ầ', N'a'); SET @strInput = REPLACE(@strInput, N'ẩ', N'a'); SET @strInput = REPLACE(@strInput, N'ẫ', N'a'); SET @strInput = REPLACE(@strInput, N'ậ', N'a');
    SET @strInput = REPLACE(@strInput, N'đ', N'd');
    SET @strInput = REPLACE(@strInput, N'é', N'e'); SET @strInput = REPLACE(@strInput, N'è', N'e'); SET @strInput = REPLACE(@strInput, N'ẻ', N'e'); SET @strInput = REPLACE(@strInput, N'ẽ', N'e'); SET @strInput = REPLACE(@strInput, N'ẹ', N'e'); SET @strInput = REPLACE(@strInput, N'ê', N'e'); SET @strInput = REPLACE(@strInput, N'ế', N'e'); SET @strInput = REPLACE(@strInput, N'ề', N'e'); SET @strInput = REPLACE(@strInput, N'ể', N'e'); SET @strInput = REPLACE(@strInput, N'ễ', N'e'); SET @strInput = REPLACE(@strInput, N'ệ', N'e');
    SET @strInput = REPLACE(@strInput, N'í', N'i'); SET @strInput = REPLACE(@strInput, N'ì', N'i'); SET @strInput = REPLACE(@strInput, N'ỉ', N'i'); SET @strInput = REPLACE(@strInput, N'ĩ', N'i'); SET @strInput = REPLACE(@strInput, N'ị', N'i');
    SET @strInput = REPLACE(@strInput, N'ó', N'o'); SET @strInput = REPLACE(@strInput, N'ò', N'o'); SET @strInput = REPLACE(@strInput, N'ỏ', N'o'); SET @strInput = REPLACE(@strInput, N'õ', N'o'); SET @strInput = REPLACE(@strInput, N'ọ', N'o'); SET @strInput = REPLACE(@strInput, N'ô', N'o'); SET @strInput = REPLACE(@strInput, N'ố', N'o'); SET @strInput = REPLACE(@strInput, N'ồ', N'o'); SET @strInput = REPLACE(@strInput, N'ổ', N'o'); SET @strInput = REPLACE(@strInput, N'ỗ', N'o'); SET @strInput = REPLACE(@strInput, N'ộ', N'o'); SET @strInput = REPLACE(@strInput, N'ơ', N'o'); SET @strInput = REPLACE(@strInput, N'ớ', N'o'); SET @strInput = REPLACE(@strInput, N'ờ', N'o'); SET @strInput = REPLACE(@strInput, N'ở', N'o'); SET @strInput = REPLACE(@strInput, N'ỡ', N'o'); SET @strInput = REPLACE(@strInput, N'ợ', N'o');
    SET @strInput = REPLACE(@strInput, N'ú', N'u'); SET @strInput = REPLACE(@strInput, N'ù', N'u'); SET @strInput = REPLACE(@strInput, N'ủ', N'u'); SET @strInput = REPLACE(@strInput, N'ũ', N'u'); SET @strInput = REPLACE(@strInput, N'ụ', N'u'); SET @strInput = REPLACE(@strInput, N'ư', N'u'); SET @strInput = REPLACE(@strInput, N'ứ', N'u'); SET @strInput = REPLACE(@strInput, N'ừ', N'u'); SET @strInput = REPLACE(@strInput, N'ử', N'u'); SET @strInput = REPLACE(@strInput, N'ữ', N'u'); SET @strInput = REPLACE(@strInput, N'ự', N'u');
    SET @strInput = REPLACE(@strInput, N'ý', N'y'); SET @strInput = REPLACE(@strInput, N'ỳ', N'y'); SET @strInput = REPLACE(@strInput, N'ỷ', N'y'); SET @strInput = REPLACE(@strInput, N'ỹ', N'y'); SET @strInput = REPLACE(@strInput, N'ỵ', N'y');
    RETURN @strInput;
END;
GO
CREATE PROCEDURE sp_InsertMonHoc @TenMon NVARCHAR(100) AS
BEGIN
    SET NOCOUNT ON; DECLARE @CleanedTenMon NVARCHAR(100) = dbo.fu_TiengVietKhongDau(@TenMon);
    SET @CleanedTenMon = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@CleanedTenMon, ' ', ''), '(', ''), ')', ''), '-', ''), '/', ''), ',', ''), '.', '');
    DECLARE @NewMaMon VARCHAR(10) = UPPER(SUBSTRING(@CleanedTenMon, 1, 10));
    IF LEN(@NewMaMon) = 0 SET @NewMaMon = 'MON' + CAST(ABS(CHECKSUM(NEWID())) % 1000 AS VARCHAR);
    IF EXISTS (SELECT 1 FROM MonHoc WHERE MaMon = @NewMaMon OR TenMon = @TenMon) BEGIN RAISERROR(N'Môn đã tồn tại.', 16, 1); RETURN; END
    INSERT INTO MonHoc (MaMon, TenMon) VALUES (@NewMaMon, @TenMon);
END;
GO
CREATE PROCEDURE sp_UpdateMonHoc @MaMon VARCHAR(10), @TenMon NVARCHAR(100) AS
BEGIN
    IF EXISTS (SELECT 1 FROM MonHoc WHERE TenMon = @TenMon AND MaMon != @MaMon) BEGIN RAISERROR(N'Tên môn trùng.', 16, 1); RETURN; END
    UPDATE MonHoc SET TenMon = @TenMon WHERE MaMon = @MaMon;
END;
GO
CREATE PROCEDURE sp_DeleteMonHoc @MaMon VARCHAR(10) AS
BEGIN
    IF EXISTS (SELECT 1 FROM GiaoVien_MonHoc WHERE MaMon = @MaMon) OR EXISTS (SELECT 1 FROM PhanCongGiangDay WHERE MaMon = @MaMon) OR EXISTS (SELECT 1 FROM KetQuaHocTap WHERE MaMon = @MaMon) OR EXISTS (SELECT 1 FROM ThoiKhoaBieu WHERE MaMon = @MaMon) BEGIN RAISERROR(N'Không thể xóa môn đang sử dụng.', 16, 1); RETURN; END
    DELETE FROM MonHoc WHERE MaMon = @MaMon;
END;
GO
CREATE PROCEDURE sp_GetThoiHanDiem AS
BEGIN
    SET NOCOUNT ON; SELECT MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong, CASE WHEN (GETDATE() NOT BETWEEN NgayMoDiem AND NgayKhoaDiem) OR (KhoaThuCong = 1) THEN 1 ELSE 0 END AS DaKhoa FROM ThoiHanDiem ORDER BY Khoi, HocKy, NgayMoDiem;
END;
GO
CREATE PROCEDURE sp_UpdateThoiHanDiem @MaCotDiem VARCHAR(20), @Khoi NVARCHAR(20), @HocKy INT, @NgayMoDiem DATE, @NgayKhoaDiem DATE, @KhoaThuCong BIT AS
BEGIN
    UPDATE ThoiHanDiem SET NgayMoDiem = @NgayMoDiem, NgayKhoaDiem = @NgayKhoaDiem, KhoaThuCong = @KhoaThuCong WHERE MaCotDiem = @MaCotDiem AND Khoi = @Khoi AND HocKy = @HocKy;
END;
GO
CREATE PROCEDURE sp_GetMonByTeacher @id NVARCHAR(50) AS BEGIN SELECT TOP 1 gvm.MaMon FROM GiaoVien gv JOIN GiaoVien_MonHoc gvm ON gv.MaGV = gvm.MaGV WHERE gv.Username=@id OR gv.Ten=@id ORDER BY gvm.MaMon; END;
GO
CREATE PROCEDURE sp_Admin_GetBaoCaoChuyenCan @Khoi NVARCHAR(20) = NULL, @MaLop VARCHAR(10) = NULL, @HocKy INT AS
BEGIN
    SET NOCOUNT ON; DECLARE @StartDate DATE, @EndDate DATE, @NamHocStr VARCHAR(10), @NamHocStartYear INT;
    IF @MaLop IS NOT NULL SELECT @NamHocStr = NamHoc FROM LopHoc WHERE MaLop = @MaLop;
    IF @NamHocStr IS NULL BEGIN DECLARE @CurrentMonth INT = MONTH(GETDATE()); DECLARE @CurrentYear INT = YEAR(GETDATE()); IF @CurrentMonth >= 8 SET @NamHocStartYear = @CurrentYear; ELSE SET @NamHocStartYear = @CurrentYear - 1; END ELSE SET @NamHocStartYear = CAST(@NamHocStr AS INT) - 1;
    IF @HocKy = 1 BEGIN SET @StartDate = DATEFROMPARTS(@NamHocStartYear, 8, 1); SET @EndDate = DATEFROMPARTS(@NamHocStartYear, 12, 31); END ELSE IF @HocKy = 2 BEGIN SET @StartDate = DATEFROMPARTS(@NamHocStartYear + 1, 1, 1); SET @EndDate = DATEFROMPARTS(@NamHocStartYear + 1, 5, 31); END ELSE BEGIN SET @StartDate = DATEFROMPARTS(@NamHocStartYear, 8, 1); SET @EndDate = DATEFROMPARTS(@NamHocStartYear + 1, 5, 31); END;
    SELECT hs.MaHS, hs.HoTen, lh.TenLop, COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) as SoBuoiCoMat, COUNT(CASE WHEN dd.TrangThai = N'Vắng' THEN 1 END) as SoBuoiVang, COUNT(CASE WHEN dd.TrangThai LIKE N'%Có phép%' THEN 1 END) as SoBuoiVangCoPhep, COUNT(dd.MaDD) as TongSoBuoi, CAST((COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) * 100.0) / NULLIF(COUNT(dd.MaDD), 0) AS DECIMAL(5,0)) as TyLeChuyenCan FROM HocSinh hs INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop LEFT JOIN DiemDanh dd ON hs.MaHS = dd.MaHS AND CAST(dd.NgayDD AS DATE) BETWEEN @StartDate AND @EndDate WHERE (@MaLop IS NOT NULL AND hs.MaLop = @MaLop) OR (@MaLop IS NULL AND @Khoi IS NOT NULL AND lh.Khoi = @Khoi) OR (@MaLop IS NULL AND @Khoi IS NULL) GROUP BY hs.MaHS, hs.HoTen, lh.TenLop ORDER BY lh.TenLop, hs.HoTen;
END;
GO
CREATE PROCEDURE sp_Admin_GetBangDiemHocKy @Khoi NVARCHAR(20) = NULL, @MaLop VARCHAR(10) = NULL, @HocKy INT AS
BEGIN
    SET NOCOUNT ON; DECLARE @loaiFilter NVARCHAR(20); DECLARE @khoiFilter NVARCHAR(20) = @Khoi;
    IF @MaLop IS NOT NULL SELECT @khoiFilter = Khoi FROM LopHoc WHERE MaLop = @MaLop;
    IF @HocKy = 1 SET @loaiFilter = N'%Ki1'; ELSE IF @HocKy = 2 SET @loaiFilter = N'%Ki2'; ELSE SET @loaiFilter = N'%Ki%';
    ;WITH RelevantScores AS ( SELECT kq.MaHS, kq.MaMon, kq.Diem FROM KetQuaHocTap kq INNER JOIN HocSinh hs ON kq.MaHS = hs.MaHS INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop INNER JOIN ThoiHanDiem thd ON kq.Loai = thd.MaCotDiem AND lh.Khoi = thd.Khoi WHERE kq.Diem IS NOT NULL AND thd.MaCotDiem LIKE @loaiFilter AND (@khoiFilter IS NULL OR thd.Khoi = @khoiFilter) AND ((@MaLop IS NOT NULL AND hs.MaLop = @MaLop) OR (@MaLop IS NULL AND @Khoi IS NOT NULL AND lh.Khoi = @Khoi) OR (@MaLop IS NULL AND @Khoi IS NULL)) AND kq.NgayNhap >= DATEADD(day, -60, thd.NgayMoDiem) ), AvgMon AS ( SELECT MaHS, MaMon, AVG(Diem) AS DiemTBMon FROM RelevantScores GROUP BY MaHS, MaMon ), AvgCaNhan AS ( SELECT MaHS, AVG(DiemTBMon) AS DiemTBCaNhan FROM AvgMon GROUP BY MaHS )
    SELECT hs.MaHS, hs.HoTen, lh.TenLop, ISNULL(acn.DiemTBCaNhan, 0) AS [Trung bình chung] FROM HocSinh hs INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop LEFT JOIN AvgCaNhan acn ON hs.MaHS = acn.MaHS WHERE (@MaLop IS NOT NULL AND hs.MaLop = @MaLop) OR (@MaLop IS NULL AND @Khoi IS NOT NULL AND lh.Khoi = @Khoi) OR (@MaLop IS NULL AND @Khoi IS NULL) ORDER BY lh.TenLop, hs.HoTen;
END;
GO
CREATE PROCEDURE sp_Admin_GetHoSoHocSinh @Khoi NVARCHAR(20) = NULL, @MaLop VARCHAR(10) = NULL AS
BEGIN
    SET NOCOUNT ON; SELECT hs.MaHS, hs.HoTen, lh.TenLop, hs.GioiTinh, hs.NgaySinh, hs.DanToc, hs.DiaChi, hs.SDTPhuHuynh FROM HocSinh hs INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop WHERE (@MaLop IS NOT NULL AND hs.MaLop = @MaLop) OR (@MaLop IS NULL AND @Khoi IS NOT NULL AND lh.Khoi = @Khoi) OR (@MaLop IS NULL AND @Khoi IS NULL) ORDER BY lh.TenLop, hs.HoTen;
END;
GO
CREATE PROCEDURE sp_GetThongKeKhoi_Admin @khoi NVARCHAR(20) = NULL AS
BEGIN
    SET NOCOUNT ON; DECLARE @khoiFilter NVARCHAR(25) = @khoi; DECLARE @loaiList TABLE (Loai NVARCHAR(20), NgayMo DATE); INSERT INTO @loaiList (Loai, NgayMo) SELECT MaCotDiem, NgayMoDiem FROM ThoiHanDiem WHERE (@khoiFilter IS NULL OR Khoi = @khoiFilter);
    WITH StudentCounts AS ( SELECT l.Khoi, l.MaLop, l.TenLop, COUNT(hs.MaHS) AS SoHocSinh, SUM(CASE WHEN hs.GioiTinh = N'Nam' THEN 1 ELSE 0 END) AS SoNam, SUM(CASE WHEN hs.GioiTinh = N'Nữ' THEN 1 ELSE 0 END) AS SoNu FROM LopHoc l LEFT JOIN HocSinh hs ON l.MaLop = hs.MaLop WHERE (@khoiFilter IS NULL OR l.Khoi = @khoiFilter) GROUP BY l.Khoi, l.MaLop, l.TenLop ), AvgScores AS ( SELECT l.MaLop, ROUND(AVG(kq.Diem), 2) AS DiemTrungBinh FROM LopHoc l LEFT JOIN HocSinh hs ON l.MaLop = hs.MaLop LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS INNER JOIN @loaiList ll ON kq.Loai = ll.Loai WHERE (@khoiFilter IS NULL OR l.Khoi = @khoiFilter) AND kq.Diem IS NOT NULL AND kq.NgayNhap >= DATEADD(day, -30, ll.NgayMo) GROUP BY l.MaLop )
    SELECT sc.Khoi, sc.TenLop, sc.SoHocSinh, ISNULL(av.DiemTrungBinh, 0) AS DiemTrungBinh, sc.SoNam, sc.SoNu FROM StudentCounts sc LEFT JOIN AvgScores av ON sc.MaLop = av.MaLop ORDER BY sc.Khoi, sc.TenLop;
END;
GO
CREATE OR ALTER PROCEDURE sp_Admin_GetBaoCaoThang_ThongKe 
    @Khoi NVARCHAR(20) = NULL, 
    @MaMon VARCHAR(10), 
    @LoaiDiem VARCHAR(20) 
AS
BEGIN
    SET NOCOUNT ON; 
    
    ;WITH RawData AS ( 
        SELECT 
            hs.GioiTinh, 
            LTRIM(RTRIM(ISNULL(hs.DanToc, ''))) AS DanTocClean,
            kq.Diem 
        FROM KetQuaHocTap kq 
        JOIN HocSinh hs ON kq.MaHS = hs.MaHS 
        JOIN LopHoc lh ON hs.MaLop = lh.MaLop 
        WHERE (@Khoi IS NULL OR lh.Khoi = @Khoi) 
          -- [FIX] Thêm điều kiện: Nếu @MaMon là 'ALL' thì lấy hết, ngược lại thì lọc theo mã
          AND (@MaMon = 'ALL' OR kq.MaMon = @MaMon) 
          AND kq.Loai = @LoaiDiem 
          AND kq.Diem IS NOT NULL
    ), 
    ClassifiedData AS ( 
        SELECT 
            CASE 
                WHEN Diem = 10 THEN '10' 
                WHEN Diem >= 9 AND Diem < 10 THEN '9' 
                WHEN Diem >= 8 AND Diem < 9 THEN '8' 
                WHEN Diem >= 7 AND Diem < 8 THEN '7' 
                WHEN Diem >= 6 AND Diem < 7 THEN '6' 
                WHEN Diem >= 5 AND Diem < 6 THEN '5' 
                ELSE N'Dưới 5' 
            END AS NhomDiem, 
            CASE 
                WHEN Diem >= 7 THEN 'T' 
                WHEN Diem >= 5 THEN 'H' 
                ELSE 'C' 
            END AS XepLoai, 
            
            CASE WHEN LTRIM(RTRIM(GioiTinh)) = N'Nữ' THEN 1 ELSE 0 END AS IsNu, 
            
            CASE 
                WHEN DanTocClean <> '' AND LOWER(DanTocClean) <> N'kinh' THEN 1 
                ELSE 0 
            END AS IsDanTocThieuSo, 
            
            CASE 
                WHEN LTRIM(RTRIM(GioiTinh)) = N'Nữ' AND (DanTocClean <> '' AND LOWER(DanTocClean) <> N'kinh') THEN 1 
                ELSE 0 
            END AS IsNuDanTocThieuSo 
        FROM RawData 
    ), 
    DiemStats AS ( 
        SELECT 'Diem' AS LoaiThongKe, NhomDiem AS PhanLoai, COUNT(*) AS TS, SUM(IsNu) AS Nu, SUM(IsDanTocThieuSo) AS DanToc, SUM(IsNuDanTocThieuSo) AS NDT, CAST(NULL AS FLOAT) AS TyLe 
        FROM ClassifiedData GROUP BY NhomDiem 
    ), 
    XepLoaiStats AS ( 
        SELECT 'XepLoai' AS LoaiThongKe, XepLoai AS PhanLoai, 
        COUNT(*) AS TS, 
        SUM(IsNu) AS Nu, 
        SUM(IsDanTocThieuSo) AS DanToc, 
        SUM(IsNuDanTocThieuSo) AS NDT, 
        CAST( (COUNT(*) * 100.0) / NULLIF((SELECT COUNT(*) FROM RawData), 0) AS DECIMAL(5, 1)) AS TyLe 
        FROM ClassifiedData GROUP BY XepLoai 
    ) 
    SELECT * FROM DiemStats UNION ALL SELECT * FROM XepLoaiStats;
END;
GO
CREATE PROCEDURE sp_GetArchiveYears AS BEGIN SELECT DISTINCT NamHoc FROM LopHoc ORDER BY NamHoc DESC; END;
GO
CREATE OR ALTER PROCEDURE sp_GetArchiveClasses
    @NamHoc VARCHAR(10),
    @Khoi NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- Logic cũ: SELECT ... FROM LopHoc WHERE NamHoc = @NamHoc (SAI vì lớp đã sang năm mới)
    
    -- Logic mới: Quét bảng HoSoLuuTru để xem năm đó có những lớp nào
    SELECT DISTINCT 
        h.MaLop, 
        l.TenLop, 
        h.MaGVCN, -- Lấy GVCN lúc đó (đã lưu trong hồ sơ)
        l.Khoi 
    FROM HoSoLuuTru h
    JOIN LopHoc l ON h.MaLop = l.MaLop
    WHERE h.MaNamHoc = @NamHoc 
      AND (@Khoi IS NULL OR @Khoi = N'Tất cả' OR l.Khoi = @Khoi)
    ORDER BY l.TenLop;
END;
GO
CREATE PROCEDURE sp_GetStudentFullTranscript @MaHS VARCHAR(10) AS
BEGIN
    SET NOCOUNT ON; SELECT mh.TenMon, MAX(CASE WHEN thd.MaCotDiem LIKE 'Thang1_Ki1%' THEN kq.Diem END) AS T1_K1, MAX(CASE WHEN thd.MaCotDiem LIKE 'Thang2_Ki1%' THEN kq.Diem END) AS T2_K1, MAX(CASE WHEN thd.MaCotDiem LIKE 'Thang3_Ki1%' THEN kq.Diem END) AS T3_K1, MAX(CASE WHEN thd.MaCotDiem LIKE 'GiuaKi1%' THEN kq.Diem END) AS GK1, MAX(CASE WHEN thd.MaCotDiem LIKE 'CuoiKi1%' THEN kq.Diem END) AS CK1, MAX(CASE WHEN thd.MaCotDiem LIKE 'Thang1_Ki2%' THEN kq.Diem END) AS T1_K2, MAX(CASE WHEN thd.MaCotDiem LIKE 'Thang2_Ki2%' THEN kq.Diem END) AS T2_K2, MAX(CASE WHEN thd.MaCotDiem LIKE 'Thang3_Ki2%' THEN kq.Diem END) AS T3_K2, MAX(CASE WHEN thd.MaCotDiem LIKE 'GiuaKi2%' THEN kq.Diem END) AS GK2, MAX(CASE WHEN thd.MaCotDiem LIKE 'CuoiKi2%' THEN kq.Diem END) AS CK2, CAST(AVG(kq.Diem) AS DECIMAL(10, 2)) AS TB_Nam FROM KetQuaHocTap kq JOIN MonHoc mh ON kq.MaMon = mh.MaMon JOIN ThoiHanDiem thd ON kq.Loai = thd.MaCotDiem WHERE kq.MaHS = @MaHS GROUP BY mh.TenMon ORDER BY mh.TenMon;
END;
GO
CREATE OR ALTER PROCEDURE sp_GetAllNamHoc AS BEGIN SELECT MaNamHoc, TenNamHoc, IsCurrent FROM NamHoc ORDER BY MaNamHoc DESC; END;
GO
CREATE OR ALTER PROCEDURE sp_GetArchivedTranscript @MaHoSo VARCHAR(50) AS
BEGIN
    SELECT mh.TenMon, MAX(CASE WHEN ct.LoaiDiem = 'Thang1_Ki1' THEN ct.Diem END) AS [T1 (K1)], MAX(CASE WHEN ct.LoaiDiem = 'Thang2_Ki1' THEN ct.Diem END) AS [T2 (K1)], MAX(CASE WHEN ct.LoaiDiem = 'Thang3_Ki1' THEN ct.Diem END) AS [T3 (K1)], MAX(CASE WHEN ct.LoaiDiem = 'GiuaKi1' THEN ct.Diem END) AS [Giữa K1], MAX(CASE WHEN ct.LoaiDiem = 'CuoiKi1' THEN ct.Diem END) AS [Cuối K1], MAX(CASE WHEN ct.LoaiDiem = 'Thang1_Ki2' THEN ct.Diem END) AS [T1 (K2)], MAX(CASE WHEN ct.LoaiDiem = 'Thang2_Ki2' THEN ct.Diem END) AS [T2 (K2)], MAX(CASE WHEN ct.LoaiDiem = 'Thang3_Ki2' THEN ct.Diem END) AS [T3 (K2)], MAX(CASE WHEN ct.LoaiDiem = 'GiuaKi2' THEN ct.Diem END) AS [Giữa K2], MAX(CASE WHEN ct.LoaiDiem = 'CuoiKi2' THEN ct.Diem END) AS [Cuối K2], MAX(CASE WHEN ct.LoaiDiem = 'CuoiKi2' THEN ct.NhanXet END) AS [Nhận Xét Năm] FROM ChiTietDiemLuuTru ct JOIN MonHoc mh ON ct.MaMon = mh.MaMon WHERE ct.MaHoSo = @MaHoSo GROUP BY mh.TenMon ORDER BY mh.TenMon
END
GO
create PROCEDURE sp_ProcessStudentPromotion_V2
    @MaLopCu VARCHAR(10),
    @MaLopMoi_LenLop VARCHAR(10),
    @MaLopMoi_OLaiLop VARCHAR(10),
    @IsLop5_TotNghiep BIT,
    @MaNamHocHienTai VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM NamHoc WHERE MaNamHoc = @MaNamHocHienTai)
    BEGIN
        RAISERROR(N'Mã năm học không tồn tại!', 16, 1);
        RETURN;
    END

    IF @IsLop5_TotNghiep = 0 AND @MaLopMoi_LenLop IS NOT NULL
    BEGIN
        DECLARE @SiSoDich INT;
        SELECT @SiSoDich = COUNT(*) FROM HocSinh WHERE MaLop = @MaLopMoi_LenLop AND MaHS NOT IN (SELECT MaHS FROM HoSoLuuTru WHERE MaNamHoc = @MaNamHocHienTai);
        IF @SiSoDich > 0
        BEGIN
            DECLARE @TenLopDich NVARCHAR(50); SELECT @TenLopDich = TenLop FROM LopHoc WHERE MaLop = @MaLopMoi_LenLop;
            RAISERROR(N'Lớp đích [%s] vẫn còn học sinh cũ. Vui lòng xét lên lớp cho lớp đó trước.', 16, 1, @TenLopDich);
            RETURN;
        END
    END

    CREATE TABLE #DanhSachXetDuyet (MaHS VARCHAR(10), DiemTB_Nam FLOAT, TrangThai INT); 

    ;WITH DiemThanhPhan AS (
        SELECT 
            kq.MaHS, kq.MaMon,
            MAX(CASE WHEN kq.Loai LIKE 'Thang1_Ki1%' THEN kq.Diem END) AS T1_1,
            MAX(CASE WHEN kq.Loai LIKE 'Thang2_Ki1%' THEN kq.Diem END) AS T2_1,
            MAX(CASE WHEN kq.Loai LIKE 'Thang3_Ki1%' THEN kq.Diem END) AS T3_1,
            MAX(CASE WHEN kq.Loai LIKE 'GiuaKi1%'    THEN kq.Diem END) AS GK_1,
            MAX(CASE WHEN kq.Loai LIKE 'CuoiKi1%'    THEN kq.Diem END) AS CK_1,
            
            MAX(CASE WHEN kq.Loai LIKE 'Thang1_Ki2%' THEN kq.Diem END) AS T1_2,
            MAX(CASE WHEN kq.Loai LIKE 'Thang2_Ki2%' THEN kq.Diem END) AS T2_2,
            MAX(CASE WHEN kq.Loai LIKE 'Thang3_Ki2%' THEN kq.Diem END) AS T3_2,
            MAX(CASE WHEN kq.Loai LIKE 'GiuaKi2%'    THEN kq.Diem END) AS GK_2,
            MAX(CASE WHEN kq.Loai LIKE 'CuoiKi2%'    THEN kq.Diem END) AS CK_2
        FROM KetQuaHocTap kq
        JOIN HocSinh hs ON kq.MaHS = hs.MaHS
        WHERE hs.MaLop = @MaLopCu 
          AND kq.Diem IS NOT NULL
          AND hs.MaHS NOT IN (SELECT MaHS FROM HoSoLuuTru WHERE MaNamHoc = @MaNamHocHienTai)
        GROUP BY kq.MaHS, kq.MaMon
    ),
    TinhDiemTrungBinhMon AS (
        SELECT 
            MaHS, MaMon,
            CAST(
                (ISNULL(T1_1, 0) + ISNULL(T2_1, 0) + ISNULL(T3_1, 0) + (ISNULL(GK_1, 0) * 2) + (ISNULL(CK_1, 0) * 3)) 
                AS FLOAT
            ) / NULLIF(
                (CASE WHEN T1_1 IS NOT NULL THEN 1 ELSE 0 END) + 
                (CASE WHEN T2_1 IS NOT NULL THEN 1 ELSE 0 END) + 
                (CASE WHEN T3_1 IS NOT NULL THEN 1 ELSE 0 END) + 
                (CASE WHEN GK_1 IS NOT NULL THEN 2 ELSE 0 END) + 
                (CASE WHEN CK_1 IS NOT NULL THEN 3 ELSE 0 END), 0
            ) AS TBHK1,

            CAST(
                (ISNULL(T1_2, 0) + ISNULL(T2_2, 0) + ISNULL(T3_2, 0) + (ISNULL(GK_2, 0) * 2) + (ISNULL(CK_2, 0) * 3)) 
                AS FLOAT
            ) / NULLIF(
                (CASE WHEN T1_2 IS NOT NULL THEN 1 ELSE 0 END) + 
                (CASE WHEN T2_2 IS NOT NULL THEN 1 ELSE 0 END) + 
                (CASE WHEN T3_2 IS NOT NULL THEN 1 ELSE 0 END) + 
                (CASE WHEN GK_2 IS NOT NULL THEN 2 ELSE 0 END) + 
                (CASE WHEN CK_2 IS NOT NULL THEN 3 ELSE 0 END), 0
            ) AS TBHK2
        FROM DiemThanhPhan
    ),
    TongKetNam_Mon AS (
        SELECT MaHS, MaMon,
            CASE 
                WHEN TBHK1 IS NOT NULL AND TBHK2 IS NOT NULL THEN (TBHK1 + (TBHK2 * 2)) / 3.0
                WHEN TBHK1 IS NULL AND TBHK2 IS NOT NULL THEN TBHK2 
                WHEN TBHK1 IS NOT NULL AND TBHK2 IS NULL THEN TBHK1 
                ELSE 0 
            END AS TBM_CaNam
        FROM TinhDiemTrungBinhMon
    ),
    TongKetChung AS (
        SELECT MaHS, AVG(TBM_CaNam) AS DTB_Chung
        FROM TongKetNam_Mon 
        WHERE TBM_CaNam IS NOT NULL 
        GROUP BY MaHS
    )
    
    INSERT INTO #DanhSachXetDuyet (MaHS, DiemTB_Nam, TrangThai)
    SELECT 
        hs.MaHS, 
        ROUND(ISNULL(tk.DTB_Chung, 0), 2),
        CASE WHEN ISNULL(tk.DTB_Chung, 0) >= 5.0 THEN 1 ELSE 0 END
    FROM HocSinh hs 
    LEFT JOIN TongKetChung tk ON hs.MaHS = tk.MaHS 
    WHERE hs.MaLop = @MaLopCu
      AND hs.MaHS NOT IN (SELECT MaHS FROM HoSoLuuTru WHERE MaNamHoc = @MaNamHocHienTai);

    IF NOT EXISTS (SELECT 1 FROM #DanhSachXetDuyet)
    BEGIN
        SELECT 0 AS SoHSLenLop, 0 AS SoHSOLaiLop; DROP TABLE #DanhSachXetDuyet; RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO HoSoLuuTru (MaHoSo, MaHS, MaNamHoc, MaLop, MaGVCN, DiemTB_CuoiNam, KetQua)
        SELECT hs.MaHS + '_' + @MaNamHocHienTai, hs.MaHS, @MaNamHocHienTai, @MaLopCu, lh.MaGVCN, ds.DiemTB_Nam,
            CASE WHEN ds.TrangThai = 0 THEN N'Lưu ban' WHEN ds.TrangThai = 1 AND @IsLop5_TotNghiep = 1 THEN N'Tốt nghiệp' ELSE N'Lên lớp' END
        FROM #DanhSachXetDuyet ds JOIN HocSinh hs ON ds.MaHS = hs.MaHS JOIN LopHoc lh ON hs.MaLop = lh.MaLop;

        INSERT INTO ChiTietDiemLuuTru (MaHoSo, MaMon, LoaiDiem, Diem, NhanXet)
        SELECT kq.MaHS + '_' + @MaNamHocHienTai, kq.MaMon, kq.Loai, kq.Diem, kq.NhanXet
        FROM KetQuaHocTap kq INNER JOIN #DanhSachXetDuyet ds ON kq.MaHS = ds.MaHS WHERE kq.Diem IS NOT NULL;

        UPDATE HocSinh SET MaLop = @MaLopMoi_OLaiLop WHERE MaHS IN (SELECT MaHS FROM #DanhSachXetDuyet WHERE TrangThai = 0);
        IF @IsLop5_TotNghiep = 1
            UPDATE HocSinh SET MaLop = NULL WHERE MaHS IN (SELECT MaHS FROM #DanhSachXetDuyet WHERE TrangThai = 1);
        ELSE
            UPDATE HocSinh SET MaLop = @MaLopMoi_LenLop WHERE MaHS IN (SELECT MaHS FROM #DanhSachXetDuyet WHERE TrangThai = 1);

        DELETE FROM KetQuaHocTap WHERE MaHS IN (SELECT MaHS FROM #DanhSachXetDuyet);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;

    SELECT (SELECT COUNT(*) FROM #DanhSachXetDuyet WHERE TrangThai = 1) AS SoHSLenLop,
           (SELECT COUNT(*) FROM #DanhSachXetDuyet WHERE TrangThai = 0) AS SoHSOLaiLop;
    DROP TABLE #DanhSachXetDuyet;
END;
GO
CREATE OR ALTER PROCEDURE sp_GetArchiveStudentList @MaNamHoc VARCHAR(10), @MaLop VARCHAR(10) AS
BEGIN
    SELECT h.MaHoSo, h.MaHS, hs.HoTen, hs.NgaySinh, hs.GioiTinh, h.DiemTB_CuoiNam AS DiemTongKet, h.KetQua FROM HoSoLuuTru h JOIN HocSinh hs ON h.MaHS = hs.MaHS WHERE h.MaNamHoc = @MaNamHoc AND (@MaLop IS NULL OR @MaLop = '' OR h.MaLop = @MaLop) ORDER BY hs.HoTen;
END;
GO
CREATE PROCEDURE sp_ProcessStudentPromotion @MaLopCu VARCHAR(10), @MaLopMoi_LenLop VARCHAR(10), @MaLopMoi_OLaiLop VARCHAR(10), @IsLop5_TotNghiep BIT AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @KhoiCu NVARCHAR(20); SELECT @KhoiCu = Khoi FROM LopHoc WHERE MaLop = @MaLopCu;
    CREATE TABLE #TempDiemTB (MaHS VARCHAR(10) PRIMARY KEY, DTB_CaNam FLOAT);
    DECLARE @monHocCols NVARCHAR(MAX), @tongMon NVARCHAR(MAX), @sql NVARCHAR(MAX); DECLARE @monCount INT;
    SELECT @monHocCols = STUFF((SELECT DISTINCT ',' + QUOTENAME(mh.TenMon) FROM PhanCongGiangDay pcg JOIN MonHoc mh ON pcg.MaMon = mh.MaMon WHERE pcg.MaLop = @MaLopCu FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'');
    SELECT @tongMon = STUFF((SELECT DISTINCT ' + ISNULL(' + QUOTENAME(mh.TenMon) + ', 0)' FROM PhanCongGiangDay pcg JOIN MonHoc mh ON pcg.MaMon = mh.MaMon WHERE pcg.MaLop = @MaLopCu FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,3,'');
    SELECT @monCount = COUNT(DISTINCT MaMon) FROM PhanCongGiangDay WHERE MaLop = @MaLopCu;
    IF @monCount = 0 BEGIN SELECT 0 AS SoHSLenLop, 0 AS SoHSOLaiLop, 0 AS SoHSTotNghiep; IF OBJECT_ID('tempdb..#TempDiemTB') IS NOT NULL DROP TABLE #TempDiemTB; RETURN; END; 
    SET @sql = N' ;WITH DiemTB AS ( SELECT hs.MaHS, mh.TenMon, AVG(kq.Diem) AS DiemTB FROM HocSinh hs INNER JOIN PhanCongGiangDay pcg ON hs.MaLop = pcg.MaLop INNER JOIN MonHoc mh ON pcg.MaMon = mh.MaMon LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS AND mh.MaMon = kq.MaMon AND kq.Loai IN ( SELECT MaCotDiem FROM ThoiHanDiem WHERE Khoi = @KhoiCu ) WHERE hs.MaLop = @MaLopCu GROUP BY hs.MaHS, mh.TenMon ), PivotData AS ( SELECT MaHS, ' + @monHocCols + N' FROM DiemTB PIVOT (AVG(DiemTB) FOR TenMon IN (' + @monHocCols + N')) AS PivotTable ) INSERT INTO #TempDiemTB (MaHS, DTB_CaNam) SELECT MaHS, ROUND((' + @tongMon + N') / NULLIF(' + CAST(@monCount AS VARCHAR) + N', 0), 2) FROM PivotData;';
    EXEC sp_executesql @sql, N'@MaLopCu VARCHAR(10), @KhoiCu NVARCHAR(20)', @MaLopCu, @KhoiCu;
    CREATE TABLE #PhanLoai (MaHS VARCHAR(10), HanhDong INT);
    INSERT INTO #PhanLoai (MaHS, HanhDong) SELECT hs.MaHS, CASE WHEN ISNULL(td.DTB_CaNam, 0) >= 5.0 THEN 1 ELSE 0 END FROM HocSinh hs LEFT JOIN #TempDiemTB td ON hs.MaHS = td.MaHS WHERE hs.MaLop = @MaLopCu;
    DECLARE @SoHocSinhLenLop INT = 0; DECLARE @SoHocSinhOLaiLop INT = 0; DECLARE @SoHocSinhTotNghiep INT = 0;
    UPDATE HocSinh SET MaLop = @MaLopMoi_OLaiLop WHERE MaHS IN (SELECT MaHS FROM #PhanLoai WHERE HanhDong = 0); SET @SoHocSinhOLaiLop = @@ROWCOUNT;
    IF @IsLop5_TotNghiep = 0 BEGIN UPDATE HocSinh SET MaLop = @MaLopMoi_LenLop WHERE MaHS IN (SELECT MaHS FROM #PhanLoai WHERE HanhDong = 1); SET @SoHocSinhLenLop = @@ROWCOUNT; END ELSE BEGIN UPDATE HocSinh SET MaLop = NULL WHERE MaHS IN (SELECT MaHS FROM #PhanLoai WHERE HanhDong = 1); SET @SoHocSinhTotNghiep = @@ROWCOUNT; END
    SELECT @SoHocSinhLenLop AS SoHSLenLop, @SoHocSinhOLaiLop AS SoHSOLaiLop, @SoHocSinhTotNghiep AS SoHSTotNghiep;
    DROP TABLE #TempDiemTB; DROP TABLE #PhanLoai;
END;
GO
CREATE OR ALTER PROCEDURE sp_CreateNewSchoolYear
    @MaNamMoi VARCHAR(10),   -- VD: '2025-2026'
    @TenNamMoi NVARCHAR(50)  -- VD: 'Năm học 2025 - 2026'
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra trùng
    IF EXISTS (SELECT 1 FROM NamHoc WHERE MaNamHoc = @MaNamMoi)
    BEGIN
        RAISERROR(N'Năm học này đã tồn tại!', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        -- A. Tắt năm cũ, Tạo năm mới
        UPDATE NamHoc SET IsCurrent = 0;
        INSERT INTO NamHoc (MaNamHoc, TenNamHoc, IsCurrent) VALUES (@MaNamMoi, @TenNamMoi, 1);

        -- B. RESET CẤU HÌNH LỚP HỌC
        -- Cập nhật năm học mới cho tất cả các lớp, Xóa GVCN (cần phân công lại)
        UPDATE LopHoc 
        SET NamHoc = LEFT(@MaNamMoi, 4), -- Lấy năm bắt đầu (VD: 2025)
            MaGVCN = NULL;

        -- C. XÓA DỮ LIỆU VẬN HÀNH CŨ
        DELETE FROM PhanCongGiangDay; -- Xóa phân công chuyên môn
        DELETE FROM ThoiKhoaBieu;     -- Xóa thời khóa biểu
        DELETE FROM QuyLop;           -- Xóa quỹ lớp (Tùy chọn)
        DELETE FROM DiemDanh;         -- Xóa điểm danh cũ cho nhẹ data

        -- D. GIA HẠN NHẬP ĐIỂM
        -- Cộng thêm 1 năm vào các mốc thời gian nhập điểm
        UPDATE ThoiHanDiem 
        SET NgayMoDiem = DATEADD(year, 1, NgayMoDiem),
            NgayKhoaDiem = DATEADD(year, 1, NgayKhoaDiem);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- 2. SP XÉT LÊN LỚP (CÓ KIỂM TRA TRỒNG LỚP)
CREATE OR ALTER PROCEDURE sp_ProcessStudentPromotion_V2
    @MaLopCu VARCHAR(10),
    @MaLopMoi_LenLop VARCHAR(10),
    @MaLopMoi_OLaiLop VARCHAR(10),
    @IsLop5_TotNghiep BIT,
    @MaNamHocHienTai VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- A. Kiểm tra năm học
    IF NOT EXISTS (SELECT 1 FROM NamHoc WHERE MaNamHoc = @MaNamHocHienTai)
    BEGIN
        RAISERROR(N'Mã năm học không tồn tại!', 16, 1);
        RETURN;
    END

    -- B. KIỂM TRA AN TOÀN: LỚP ĐÍCH CÓ TRỐNG KHÔNG?
    IF @IsLop5_TotNghiep = 0 AND @MaLopMoi_LenLop IS NOT NULL
    BEGIN
        DECLARE @SiSoDich INT;
        SELECT @SiSoDich = COUNT(*) FROM HocSinh WHERE MaLop = @MaLopMoi_LenLop;
        
        IF @SiSoDich > 0
        BEGIN
            DECLARE @TenLopDich NVARCHAR(50);
            SELECT @TenLopDich = TenLop FROM LopHoc WHERE MaLop = @MaLopMoi_LenLop;
            
            DECLARE @Msg NVARCHAR(MAX) = N'⛔ KHÔNG THỂ THỰC HIỆN!' + CHAR(13) 
                + N'Lớp đích [' + @TenLopDich + N'] vẫn còn ' + CAST(@SiSoDich AS NVARCHAR) + N' học sinh.' 
                + CHAR(13) + N'👉 Bạn phải xét lên lớp cho [' + @TenLopDich + N'] trước (Nguyên tắc: Làm từ Khối 5 -> 4 -> 3...).';
            
            RAISERROR(@Msg, 16, 1);
            RETURN;
        END
    END

    -- C. TÍNH TOÁN ĐIỂM (CTE)
    CREATE TABLE #DanhSachXetDuyet (MaHS VARCHAR(10), DiemTB_Nam FLOAT, TrangThai INT); 

    ;WITH DiemThanhPhan AS (
        SELECT 
            kq.MaHS, kq.MaMon,
            MAX(CASE WHEN kq.Loai = 'Thang1_Ki1' THEN kq.Diem END) AS T1_1,
            MAX(CASE WHEN kq.Loai = 'Thang2_Ki1' THEN kq.Diem END) AS T2_1,
            MAX(CASE WHEN kq.Loai = 'Thang3_Ki1' THEN kq.Diem END) AS T3_1,
            MAX(CASE WHEN kq.Loai = 'GiuaKi1'    THEN kq.Diem END) AS GK_1,
            MAX(CASE WHEN kq.Loai = 'CuoiKi1'    THEN kq.Diem END) AS CK_1,
            MAX(CASE WHEN kq.Loai = 'Thang1_Ki2' THEN kq.Diem END) AS T1_2,
            MAX(CASE WHEN kq.Loai = 'Thang2_Ki2' THEN kq.Diem END) AS T2_2,
            MAX(CASE WHEN kq.Loai = 'Thang3_Ki2' THEN kq.Diem END) AS T3_2,
            MAX(CASE WHEN kq.Loai = 'GiuaKi2'    THEN kq.Diem END) AS GK_2,
            MAX(CASE WHEN kq.Loai = 'CuoiKi2'    THEN kq.Diem END) AS CK_2
        FROM KetQuaHocTap kq
        JOIN HocSinh hs ON kq.MaHS = hs.MaHS
        WHERE hs.MaLop = @MaLopCu AND kq.Diem IS NOT NULL
        GROUP BY kq.MaHS, kq.MaMon
    ),
    TinhToanHeSo AS (
        SELECT 
            MaHS, MaMon,
            (ISNULL(T1_1, 0) + ISNULL(T2_1, 0) + ISNULL(T3_1, 0) + (ISNULL(GK_1, 0)*2) + (ISNULL(CK_1, 0)*3)) AS TongDiem_HK1,
            ((CASE WHEN T1_1 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN T2_1 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN T3_1 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN GK_1 IS NOT NULL THEN 2 ELSE 0 END) + (CASE WHEN CK_1 IS NOT NULL THEN 3 ELSE 0 END)) AS TongHeSo_HK1,
            (ISNULL(T1_2, 0) + ISNULL(T2_2, 0) + ISNULL(T3_2, 0) + (ISNULL(GK_2, 0)*2) + (ISNULL(CK_2, 0)*3)) AS TongDiem_HK2,
            ((CASE WHEN T1_2 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN T2_2 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN T3_2 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN GK_2 IS NOT NULL THEN 2 ELSE 0 END) + (CASE WHEN CK_2 IS NOT NULL THEN 3 ELSE 0 END)) AS TongHeSo_HK2
        FROM DiemThanhPhan
    ),
    DiemTrungBinhMon AS (
        SELECT MaHS, MaMon,
            CAST(TongDiem_HK1 AS FLOAT) / NULLIF(TongHeSo_HK1, 0) AS TBHK1,
            CAST(TongDiem_HK2 AS FLOAT) / NULLIF(TongHeSo_HK2, 0) AS TBHK2
        FROM TinhToanHeSo
    ),
    DiemTongKetNam_Mon AS (
        SELECT MaHS, MaMon,
            CASE WHEN TBHK1 IS NOT NULL AND TBHK2 IS NOT NULL THEN (TBHK1 + (TBHK2 * 2)) / 3.0
                 WHEN TBHK1 IS NULL AND TBHK2 IS NOT NULL THEN TBHK2 ELSE NULL END AS TBM_CaNam
        FROM DiemTrungBinhMon
    ),
    TongKetChung AS (
        SELECT MaHS, AVG(TBM_CaNam) AS DTB_Chung
        FROM DiemTongKetNam_Mon WHERE TBM_CaNam IS NOT NULL GROUP BY MaHS
    )
    INSERT INTO #DanhSachXetDuyet (MaHS, DiemTB_Nam, TrangThai)
    SELECT hs.MaHS, ISNULL(tk.DTB_Chung, 0), CASE WHEN ISNULL(tk.DTB_Chung, 0) >= 5.0 THEN 1 ELSE 0 END
    FROM HocSinh hs LEFT JOIN TongKetChung tk ON hs.MaHS = tk.MaHS WHERE hs.MaLop = @MaLopCu;

    -- D. LƯU TRỮ VÀ CHUYỂN LỚP
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Lưu Hồ Sơ
        INSERT INTO HoSoLuuTru (MaHoSo, MaHS, MaNamHoc, MaLop, MaGVCN, DiemTB_CuoiNam, KetQua)
        SELECT hs.MaHS + '_' + @MaNamHocHienTai, hs.MaHS, @MaNamHocHienTai, @MaLopCu, lh.MaGVCN, ds.DiemTB_Nam,
            CASE WHEN ds.TrangThai = 0 THEN N'Lưu ban' WHEN ds.TrangThai = 1 AND @IsLop5_TotNghiep = 1 THEN N'Tốt nghiệp' ELSE N'Lên lớp' END
        FROM #DanhSachXetDuyet ds JOIN HocSinh hs ON ds.MaHS = hs.MaHS JOIN LopHoc lh ON hs.MaLop = lh.MaLop;

        -- Lưu Chi Tiết
        INSERT INTO ChiTietDiemLuuTru (MaHoSo, MaMon, LoaiDiem, Diem, NhanXet)
        SELECT kq.MaHS + '_' + @MaNamHocHienTai, kq.MaMon, kq.Loai, kq.Diem, kq.NhanXet
        FROM KetQuaHocTap kq INNER JOIN #DanhSachXetDuyet ds ON kq.MaHS = ds.MaHS WHERE kq.Diem IS NOT NULL;

        -- Chuyển lớp
        UPDATE HocSinh SET MaLop = @MaLopMoi_OLaiLop WHERE MaHS IN (SELECT MaHS FROM #DanhSachXetDuyet WHERE TrangThai = 0);
        
        IF @IsLop5_TotNghiep = 1
            UPDATE HocSinh SET MaLop = NULL WHERE MaHS IN (SELECT MaHS FROM #DanhSachXetDuyet WHERE TrangThai = 1);
        ELSE
            UPDATE HocSinh SET MaLop = @MaLopMoi_LenLop WHERE MaHS IN (SELECT MaHS FROM #DanhSachXetDuyet WHERE TrangThai = 1);

        -- Dọn dẹp
        DELETE FROM KetQuaHocTap WHERE MaHS IN (SELECT MaHS FROM #DanhSachXetDuyet);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;

    SELECT (SELECT COUNT(*) FROM #DanhSachXetDuyet WHERE TrangThai = 1) AS SoHSLenLop,
           (SELECT COUNT(*) FROM #DanhSachXetDuyet WHERE TrangThai = 0) AS SoHSOLaiLop;
    DROP TABLE #DanhSachXetDuyet;
END;
GO
create PROCEDURE sp_CheckAllClassesPromoted
    @MaNamHoc VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TongSoHocSinh INT;
    DECLARE @SoHocSinhDaXet INT;

    -- Đếm tổng số học sinh hiện đang đi học (có Mã Lớp)
    -- (Học sinh đã tốt nghiệp MaLop=NULL thì không tính)
    SELECT @TongSoHocSinh = COUNT(*) FROM HocSinh WHERE MaLop IS NOT NULL;

    -- Đếm số học sinh đã được lưu trữ trong năm nay
    -- Chỉ đếm những em nào hiện tại vẫn đang đi học (để khớp với tập dữ liệu trên)
    -- Hoặc đơn giản là đếm trong HoSoLuuTru
    SELECT @SoHocSinhDaXet = COUNT(DISTINCT MaHS) 
    FROM HoSoLuuTru 
    WHERE MaNamHoc = @MaNamHoc 
      AND MaHS IN (SELECT MaHS FROM HocSinh WHERE MaLop IS NOT NULL);

    -- Nếu số học sinh đã xét = tổng số học sinh => Cho phép qua năm mới
    -- (Thêm điều kiện >0 để tránh trường hợp chưa có học sinh nào mà vẫn hiện nút)
    IF @TongSoHocSinh > 0 AND @TongSoHocSinh <= @SoHocSinhDaXet
        SELECT 1 AS IsFinished;
    ELSE
        SELECT 0 AS IsFinished;
END;
GO
PRINT 'DONE.';
GO