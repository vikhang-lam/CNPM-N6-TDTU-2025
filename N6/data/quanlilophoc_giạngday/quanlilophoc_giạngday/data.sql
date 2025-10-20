
CREATE DATABASE quanlilophoc_giangday;
GO
USE quanlilophoc_giangday;
GO
PRINT 'ĐÃ TẠO DATABASE MỚI.';

--------------------------------------------------
-- BƯỚC 3: TẠO CÁC BẢNG
--------------------------------------------------
CREATE TABLE Admin (
    MaAdmin VARCHAR(10) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    Password VARCHAR(30) NOT NULL,
    Email NVARCHAR(50) NOT NULL
);

CREATE TABLE MonHoc (
    MaMon VARCHAR(10) PRIMARY KEY,
    TenMon NVARCHAR(50) NOT NULL
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
    MaMon VARCHAR(10),
    Email NVARCHAR(50),
    SDT VARCHAR(15),
    MaAdmin VARCHAR(10),
    AnhDaiDien NVARCHAR(200),
    TrangThai NVARCHAR(20) DEFAULT N'Chưa xác nhận',
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon),
    FOREIGN KEY (MaAdmin) REFERENCES Admin(MaAdmin)
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
    PRIMARY KEY (MaGV, MaLop),
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV) ON DELETE CASCADE,
    FOREIGN KEY (MaLop) REFERENCES LopHoc(MaLop) ON DELETE CASCADE
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
PRINT 'ĐÃ TẠO TẤT CẢ CÁC BẢNG.';
GO
--------------------------------------------------
-- BƯỚC 4: CHÈN DỮ LIỆU MẪU (GIỮ NGUYÊN)
--------------------------------------------------
INSERT INTO Admin (MaAdmin, Username, Password, Email) VALUES ('AD001', 'admin', '123456', 'admin@example.com');
INSERT INTO MonHoc (MaMon, TenMon) VALUES ('VAN', N'Ngữ văn'), ('TOAN', N'Toán'), ('ANH', N'Tiếng Anh');

INSERT INTO LopHoc (MaLop, TenLop, Khoi, NamHoc) VALUES
('5A10', N'Lớp 5A10', N'Khối 5', '2025'),
('5A11', N'Lớp 5A11', N'Khối 5', '2025'),
('5A12', N'Lớp 5A12', N'Khối 5', '2025');

INSERT INTO GiaoVien (MaGV, Ten, Username, Password, MaMon, Email, SDT, MaAdmin, TrangThai) VALUES
('GV001', N'Cô Minh Anh', 'minhanh', '123456', 'VAN', 'minhanh@example.com', '0905123456', 'AD001', N'Đã xác nhận'),
('GV002', N'Thầy Quốc Hưng', 'quochung', '123456', 'TOAN', 'quochung@example.com', '0912345002', 'AD001', N'Đã xác nhận'),
('GV003', N'Cô Thu Hà', 'thuha', '123456', 'ANH', 'thuha@example.com', '0912345003', 'AD001', N'Đã xác nhận'),
('GV004', N'Thầy Trung', 'quoTrung', '123456', NULL, 'quoctrung@example.com', '09123450012', 'AD001', N'Chưa xác nhận');

UPDATE LopHoc SET MaGVCN = 'GV001' WHERE MaLop = '5A10';
UPDATE LopHoc SET MaGVCN = 'GV002' WHERE MaLop = '5A11';

INSERT INTO PhanCongGiangDay (MaGV, MaLop) VALUES
('GV001', '5A10'), ('GV002', '5A10'), ('GV003', '5A10'),
('GV001', '5A11'), ('GV002', '5A11'),
('GV002', '5A12');

INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS001', '5A10', N'Nguyễn Văn Nam', N'Kinh', N'Nam', '0912345678', N'Hà Nội', '2015-09-10'),
('HS002', '5A10', N'Trần Thị Lan', N'Kinh', N'Nữ', '0987654321', N'Hà Nội', '2015-04-15'),
('HS003', '5A10', N'Lê Hoàng Anh', N'Kinh', N'Nam', '0977123456', N'Hà Nội', '2015-12-01'),
('HS004', '5A11', N'Phạm Thị Bích', N'Kinh', N'Nữ', '0911223344', N'Hải Phòng', '2015-01-20'),
('HS005', '5A11', N'Đặng Văn Long', N'Kinh', N'Nam', '0922334455', N'Hà Nội', '2015-03-25');

INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, MaMon, GhiChu, MaGV, MaLop) VALUES
('TKB001', '2025-10-06', 1, 'VAN', N'Ôn tập chương 1', 'GV001', '5A10'),
('TKB002', '2025-10-06', 2, 'TOAN', N'Luyện tập cộng trừ phân số', 'GV002', '5A10'),
('TKB003', '2025-10-07', 1, 'ANH', N'Học từ vựng chủ đề gia đình', 'GV003', '5A10'),
('TKB004', '2025-10-07', 2, 'TOAN', N'Bài tập ứng dụng thực tế', 'GV002', '5A10'),
('TKB005', '2025-10-08', 3, 'VAN', N'Đọc hiểu văn bản', 'GV001', '5A10'),
('TKB006', '2025-10-08', 4, 'ANH', N'Luyện nghe hội thoại', 'GV003', '5A10'),
('TKB007', '2025-10-09', 1, 'TOAN', N'Giải toán có lời văn', 'GV002', '5A10'),
('TKB008', '2025-10-09', 5, 'VAN', N'Tập làm văn miêu tả', 'GV001', '5A10'),
('TKB009', '2025-10-10', 2, 'ANH', N'Kiểm tra 15 phút', 'GV003', '5A10'),
('TKB010', '2025-10-10', 3, 'TOAN', N'Ôn tập chương 2', 'GV002', '5A10'),
('TKB011', '2025-10-06', 3, 'VAN', N'Giới thiệu tác phẩm mới', 'GV001', '5A11'),
('TKB012', '2025-10-07', 4, 'TOAN', N'Hình học', 'GV002', '5A11');

INSERT INTO Minigame (MaMNG, Ten, DuLieu) VALUES
('MNG01', N'Quiz nhanh', NULL), ('MNG02', N'Gọi tên ngẫu nhiên', NULL), ('MNG03', N'Flashcard', NULL),
('MNG04', N'Ghép chữ', NULL), ('MNG05', N'Nghe - chọn hình', NULL), ('MNG06', N'Sắp xếp câu', NULL),
('MNG07', N'Điền từ', NULL), ('MNG08', N'Lật thẻ', NULL), ('MNG09', N'Random số', NULL), ('MNG10', N'Pass a ball', NULL);

PRINT 'ĐÃ CHÈN TẤT CẢ DỮ LIỆU MẪU.';
GO

--------------------------------------------------
-- BƯỚC 5: TẠO CÁC TRIGGERS VÀ SP GỐC
--------------------------------------------------

CREATE PROCEDURE sp_TaoDiemDanhMacDinh
    @MaLop VARCHAR(10),
    @Ngay DATE = NULL,
    @Buoi NVARCHAR(10) = N'Sáng'
AS
BEGIN
    SET NOCOUNT ON;
    IF @Ngay IS NULL SET @Ngay = CAST(GETDATE() AS DATE);

    INSERT INTO DiemDanh (MaDD, MaHS, NgayDD, Buoi, TrangThai)
    SELECT LEFT(NEWID(), 8), hs.MaHS, CAST(@Ngay AS DATETIME), @Buoi, N'Có mặt'
    FROM HocSinh hs
    WHERE hs.MaLop = @MaLop
      AND NOT EXISTS (
          SELECT 1 FROM DiemDanh dd
          WHERE dd.MaHS = hs.MaHS AND CAST(dd.NgayDD AS DATE) = @Ngay AND dd.Buoi = @Buoi
      );
END;
GO

CREATE TRIGGER trg_TaoDiemDanhHocSinhMoi ON HocSinh AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @today DATE = CAST(GETDATE() AS DATE);
    INSERT INTO DiemDanh (MaDD, MaHS, NgayDD, Buoi, TrangThai)
    SELECT LEFT(NEWID(), 8), i.MaHS, @today, N'Sáng', N'Có mặt'
    FROM INSERTED i;
END;
GO

CREATE PROCEDURE sp_TaoKetQuaHocTapMacDinh
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @loai TABLE (Loai NVARCHAR(20));
    INSERT INTO @loai VALUES
    (N'Thang1_Ki1'),(N'Thang2_Ki1'),(N'GiuaKi1'),(N'CuoiKi1'),
    (N'Thang1_Ki2'),(N'Thang2_Ki2'),(N'GiuaKi2'),(N'CuoiKi2');

    INSERT INTO KetQuaHocTap (MaKQ, MaMon, MaHS, NgayNhap, Loai)
    SELECT LEFT(NEWID(), 8), m.MaMon, hs.MaHS, GETDATE(), l.Loai
    FROM HocSinh hs
    CROSS JOIN MonHoc m
    CROSS JOIN @loai l
    WHERE NOT EXISTS (
        SELECT 1 FROM KetQuaHocTap kq
        WHERE kq.MaHS = hs.MaHS AND kq.MaMon = m.MaMon AND kq.Loai = l.Loai
    );
END;
GO

CREATE TRIGGER trg_TaoKetQuaHocTapHocSinhMoi ON HocSinh AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @loai TABLE (Loai NVARCHAR(20));
    INSERT INTO @loai VALUES
    (N'Thang1_Ki1'),(N'Thang2_Ki1'),(N'GiuaKi1'),(N'CuoiKi1'),
    (N'Thang1_Ki2'),(N'Thang2_Ki2'),(N'GiuaKi2'),(N'CuoiKi2');

    INSERT INTO KetQuaHocTap (MaKQ, MaMon, MaHS, NgayNhap, Loai)
    SELECT LEFT(NEWID(), 8), m.MaMon, i.MaHS, GETDATE(), l.Loai
    FROM INSERTED i
    CROSS JOIN MonHoc m
    CROSS JOIN @loai l;
END;
GO
CREATE TRIGGER trg_UpdateDiemDanhTimestamp
ON DiemDanh
AFTER UPDATE
AS
BEGIN
    IF UPDATE(TrangThai)
    BEGIN
        UPDATE DiemDanh
        SET ThoiGianCapNhat = GETDATE()
        FROM DiemDanh
        INNER JOIN inserted ON DiemDanh.MaDD = inserted.MaDD;
    END
END;
GO

CREATE PROCEDURE sp_GetHomeroomGradebook
    @MaLop VARCHAR(10),
    @LoaiDiem NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @cols AS NVARCHAR(MAX),
            @query AS NVARCHAR(MAX);

    SELECT @cols = STUFF((SELECT DISTINCT ',' + QUOTENAME(mh.TenMon) 
                    FROM KetQuaHocTap kq
                    JOIN MonHoc mh ON kq.MaMon = mh.MaMon
                    JOIN HocSinh hs ON kq.MaHS = hs.MaHS
                    WHERE hs.MaLop = @MaLop AND kq.Loai = @LoaiDiem AND kq.Diem IS NOT NULL
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

    IF @cols IS NULL
    BEGIN
        SELECT MaHS, HoTen FROM HocSinh WHERE MaLop = @MaLop ORDER BY HoTen;
        RETURN;
    END

    SET @query = 'SELECT MaHS, HoTen, ' + @cols + ' from 
            (
                SELECT 
                    hs.MaHS,
                    hs.HoTen,
                    mh.TenMon,
                    kq.Diem
                FROM KetQuaHocTap kq
                JOIN HocSinh hs ON kq.MaHS = hs.MaHS
                JOIN MonHoc mh ON kq.MaMon = mh.MaMon
                WHERE hs.MaLop = ''' + @MaLop + ''' AND kq.Loai = ''' + @LoaiDiem + '''
            ) x
            pivot 
            (
                MAX(Diem)
                for TenMon in (' + @cols + ')
            ) p 
            ORDER BY HoTen'

    EXECUTE(@query);
END
GO
--------------------------------------------------
-- KHỞI TẠO DỮ LIỆU BAN ĐẦU
--------------------------------------------------
EXEC sp_TaoDiemDanhMacDinh @MaLop = '5A10';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '5A11';
EXEC sp_TaoKetQuaHocTapMacDinh;
GO

-- Cập nhật điểm mẫu (GIỮ NGUYÊN)
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS001' AND MaMon = 'TOAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 7.0 WHERE MaHS = 'HS001' AND MaMon = 'VAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS001' AND MaMon = 'ANH' AND Loai = 'CuoiKi1';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS001' AND MaMon = 'TOAN' AND Loai = 'CuoiKi1';

UPDATE KetQuaHocTap SET Diem = 6.5 WHERE MaHS = 'HS002' AND MaMon = 'TOAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 5.0 WHERE MaHS = 'HS002' AND MaMon = 'VAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 4.0 WHERE MaHS = 'HS002' AND MaMon = 'ANH' AND Loai = 'CuoiKi1';
UPDATE KetQuaHocTap SET Diem = 2.0 WHERE MaHS = 'HS002' AND MaMon = 'TOAN' AND Loai = 'CuoiKi1';

UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS003' AND MaMon = 'TOAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 10.0 WHERE MaHS = 'HS003' AND MaMon = 'VAN' AND Loai = 'CuoiKi1';

UPDATE KetQuaHocTap SET Diem = 4.5 WHERE MaHS = 'HS004' AND MaMon = 'TOAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS004' AND MaMon = 'TOAN' AND Loai = 'CuoiKi1';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS004' AND MaMon = 'VAN' AND Loai = 'CuoiKi1';

UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS005' AND MaMon = 'TOAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS005' AND MaMon = 'VAN' AND Loai = 'CuoiKi1';
GO

-- CÁC SP BÁO CÁO GỐC
CREATE PROCEDURE sp_BaoCaoChuyenCan_Lop
    @maLop VARCHAR(10),
    @hocKy NVARCHAR(20)
AS
BEGIN
    DECLARE @NamHoc INT = YEAR(GETDATE());
    DECLARE @StartDate DATETime, @EndDate DATETime;

    IF @hocKy = N'Học kỳ 1'
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHoc - 1, 9, 1);
        SET @EndDate = DATEFROMPARTS(@NamHoc, 1, 15);
    END
    ELSE IF @hocKy = N'Học kỳ 2'
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHoc, 1, 16);
        SET @EndDate = DATEFROMPARTS(@NamHoc, 5, 31);
    END
    ELSE -- Cả năm
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHoc - 1, 9, 1);
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
    LEFT JOIN DiemDanh dd ON hs.MaHS = dd.MaHS
    WHERE hs.MaLop = @maLop AND dd.NgayDD BETWEEN @StartDate AND @EndDate
    GROUP BY hs.MaHS, hs.HoTen;
END
GO
CREATE PROCEDURE sp_BaoCaoDiemSo_Lop
    @maLop VARCHAR(10),
    @hocKy NVARCHAR(20)
AS
BEGIN
    SELECT
        hs.MaHS,
        hs.HoTen,
        mh.TenMon,
        kqht.Diem
    FROM HocSinh hs
    JOIN KetQuaHocTap kqht ON hs.MaHS = kqht.MaHS
    JOIN MonHoc mh ON kqht.MaMon = mh.MaMon
    WHERE hs.MaLop = @maLop AND kqht.Loai = @hocKy;
END
GO
CREATE PROCEDURE sp_ThongKeDiemTB_Khoi
    @khoi NVARCHAR(20),
    @hocKy NVARCHAR(20)
AS
BEGIN
    SELECT
        hs.MaLop,
        AVG(kqht.Diem) as DiemTrungBinh
    FROM HocSinh hs
    JOIN KetQuaHocTap kqht ON hs.MaHS = kqht.MaHS
    WHERE hs.MaLop LIKE @khoi + '%' AND kqht.Loai = @hocKy
    GROUP BY hs.MaLop;
END
GO
CREATE PROCEDURE sp_ThongKeHocLuc_Khoi
    @khoi NVARCHAR(20),
    @hocKy NVARCHAR(20)
AS
BEGIN
    SELECT
        HocLuc,
        COUNT(*) as SoLuong
    FROM (
        SELECT
            CASE
                WHEN AVG(kqht.Diem) >= 8.5 THEN N'Giỏi'
                WHEN AVG(kqht.Diem) >= 6.5 THEN N'Khá'
                WHEN AVG(kqht.Diem) >= 5.0 THEN N'Trung bình'
                ELSE N'Yếu'
            END as HocLuc
        FROM HocSinh hs
        JOIN KetQuaHocTap kqht ON hs.MaHS = kqht.MaHS
        WHERE hs.MaLop LIKE @khoi + '%' AND kqht.Loai = @hocKy
        GROUP BY hs.MaHS
    ) as BangHocLuc
    GROUP BY HocLuc;
END
GO
PRINT 'ĐÃ TẠO SP GỐC VÀ CẬP NHẬT ĐIỂM MẪU.';

--------------------------------------------------
-- BƯỚC 6: TẠO CÁC TABLE TYPES MỚI
--------------------------------------------------

CREATE TYPE ut_HocSinhImport AS TABLE(
    MaHS VARCHAR(10) PRIMARY KEY,
    MaLop VARCHAR(10) NULL,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE NOT NULL,
    GioiTinh NVARCHAR(10) NULL,
    SDTPhuHuynh VARCHAR(15) NULL,
    DiaChi NVARCHAR(200) NULL,
    DanToc NVARCHAR(50) NULL
);
GO
CREATE TYPE ut_DateList AS TABLE(
    Ngay DATE PRIMARY KEY
);
GO
CREATE TYPE ut_TKBImport AS TABLE(
    Ngay DATE,
    Tiet INT,
    TenMon NVARCHAR(50),
    TenLop NVARCHAR(50),
    GhiChu NVARCHAR(200) NULL,
    MauSac VARCHAR(20) NULL
);
GO


--------------------------------------------------
-- BƯỚC 7: TẠO TẤT CẢ CÁC SP ĐÃ CHUYỂN ĐỔI (ĐÃ SỬA LỖI)
--------------------------------------------------

-- #region Đăng nhập
CREATE PROCEDURE sp_CheckTeacherLogin
    @user NVARCHAR(50),
    @pass VARCHAR(30)
AS
BEGIN
    SELECT TrangThai 
    FROM GiaoVien 
    WHERE Username=@user AND Password=@pass;
END;
GO
CREATE PROCEDURE sp_CheckAdminLogin
    @user NVARCHAR(50),
    @pass VARCHAR(30)
AS
BEGIN
    SELECT COUNT(*) 
    FROM Admin 
    WHERE Username=@user AND Password=@pass;
END;
GO
-- #endregion

-- #region Thông tin giáo viên
CREATE PROCEDURE sp_GetTeacherProfile
    @user NVARCHAR(50)
AS
BEGIN
    SELECT gv.Ten, gv.Email, gv.SDT, gv.AnhDaiDien, mh.TenMon
    FROM GiaoVien gv
    LEFT JOIN MonHoc mh ON gv.MaMon = mh.MaMon
    WHERE gv.Username = @user OR gv.Ten = @user;
END;
GO
CREATE PROCEDURE sp_UpdateTeacherAvatar
    @user NVARCHAR(50),
    @avatar NVARCHAR(200)
AS
BEGIN
    UPDATE GiaoVien 
    SET AnhDaiDien = @avatar 
    WHERE Username = @user OR Ten = @user;
END;
GO
CREATE PROCEDURE sp_GetMonByTeacher
    @id NVARCHAR(50)
AS
BEGIN
    SELECT MaMon 
    FROM GiaoVien 
    WHERE Username=@id OR Ten=@id;
END;
GO
-- #endregion

-- #region Học sinh
CREATE PROCEDURE sp_GetHocSinhByLop
    @malop VARCHAR(10)
AS
BEGIN
    SELECT MaHS, HoTen, GioiTinh, NgaySinh, DiaChi, DanToc, SDTPhuHuynh 
    FROM HocSinh 
    WHERE MaLop = @malop 
    ORDER BY HoTen;
END;
GO
CREATE PROCEDURE sp_GetHocSinhProfile
    @maHS VARCHAR(10)
AS
BEGIN
    SELECT * FROM HocSinh 
    WHERE MaHS=@maHS;
END;
GO
CREATE PROCEDURE sp_UpdateHocSinhProfile
    @MaHS VARCHAR(10),
    @HoTen NVARCHAR(100),
    @GioiTinh NVARCHAR(10),
    @NgaySinh DATE,
    @DiaChi NVARCHAR(200)
AS
BEGIN
    UPDATE HocSinh 
    SET HoTen=@HoTen, GioiTinh=@GioiTinh, NgaySinh=@NgaySinh, DiaChi=@DiaChi
    WHERE MaHS=@MaHS;
END;
GO
-- #endregion

-- #region Điểm danh
CREATE PROCEDURE sp_GetDiemDanhByLop
    @maLop VARCHAR(10)
AS
BEGIN
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
    ORDER BY hs.HoTen, dd.NgayDD;
END;
GO
CREATE PROCEDURE sp_UpsertDiemDanh
    @MaHS VARCHAR(10),
    @Ngay DATETIME,
    @Buoi NVARCHAR(10),
    @TrangThai NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Buoi IS NULL OR @Buoi = '' SET @Buoi = N'Sáng';
    IF @TrangThai IS NULL OR @TrangThai = '' SET @TrangThai = N'Có mặt';

    DECLARE @ExistingMaDD VARCHAR(10);
    DECLARE @NgayDate DATE = CAST(@Ngay AS DATE);

    SELECT @ExistingMaDD = MaDD 
    FROM DiemDanh 
    WHERE MaHS = @MaHS AND CAST(NgayDD AS DATE) = @NgayDate AND Buoi = @Buoi;

    IF @ExistingMaDD IS NOT NULL
    BEGIN
        UPDATE DiemDanh 
        SET TrangThai = @TrangThai, NgayDD = @Ngay
        WHERE MaDD = @ExistingMaDD;
    END
    ELSE
    BEGIN
        INSERT INTO DiemDanh(MaDD, MaHS, NgayDD, Buoi, TrangThai)
        VALUES(LEFT(NEWID(), 8), @MaHS, @Ngay, @Buoi, @TrangThai);
    END
END;
GO
CREATE PROCEDURE sp_GetDiemDanhByLopAndDate
    @maLop VARCHAR(10),
    @ngay DATE,
    @buoi NVARCHAR(10)
AS
BEGIN
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
    ORDER BY hs.HoTen;
END;
GO
CREATE PROCEDURE sp_UpdateDiemDanh
    @MaDD VARCHAR(10),
    @TrangThai NVARCHAR(20)
AS
BEGIN
    UPDATE DiemDanh SET TrangThai=@TrangThai WHERE MaDD=@MaDD;
END;
GO
-- #endregion

-- #region Kết quả học tập
CREATE PROCEDURE sp_InsertKetQuaHocTap
    @MaHS VARCHAR(10),
    @MaMon VARCHAR(10),
    @Diem FLOAT,
    @NhanXet NVARCHAR(200)
AS
BEGIN
    INSERT INTO KetQuaHocTap(MaKQ, MaMon, MaHS, NgayNhap, Diem, NhanXet) 
    VALUES(LEFT(NEWID(), 8), @MaMon, @MaHS, GETDATE(), @Diem, @NhanXet);
END;
GO
CREATE PROCEDURE sp_UpsertKetQuaHocTap
    @MaHS VARCHAR(10),
    @MaMon VARCHAR(10),
    @Loai NVARCHAR(20),
    @Diem FLOAT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM KetQuaHocTap WHERE MaHS=@MaHS AND MaMon=@MaMon AND Loai=@Loai)
    BEGIN
        UPDATE KetQuaHocTap 
        SET Diem=@Diem 
        WHERE MaHS=@MaHS AND MaMon=@MaMon AND Loai=@Loai;
    END
    ELSE
    BEGIN
        INSERT INTO KetQuaHocTap(MaKQ, MaHS, MaMon, Loai, Diem) 
        VALUES(LEFT(NEWID(), 8), @MaHS, @MaMon, @Loai, @Diem);
    END
END;
GO
CREATE PROCEDURE sp_GetKetQuaHocTapByLop
    @malop VARCHAR(10)
AS
BEGIN
    SELECT hs.MaHS, hs.HoTen, mh.TenMon, kq.Diem, kq.NhanXet, kq.GhiChu, kq.Loai
    FROM HocSinh hs
    INNER JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS
    INNER JOIN MonHoc mh ON kq.MaMon = mh.MaMon
    WHERE hs.MaLop = @malop
    ORDER BY hs.MaHS, mh.MaMon;
END;
GO
CREATE PROCEDURE sp_GetBangDiemPivot
    @malop VARCHAR(10),
    @ki INT,
    @maMon VARCHAR(10)
AS
BEGIN
    DECLARE @loaiFilter NVARCHAR(10) = N'%Ki' + CAST(@ki AS VARCHAR) + N'%';
    DECLARE @sql NVARCHAR(MAX);

    SET @sql = N'
    SELECT 
        hs.MaHS, 
        hs.HoTen,
        lh.TenLop,
        MAX(CASE WHEN kq.Loai = ''Thang1_Ki' + CAST(@ki AS VARCHAR) + ''' THEN kq.Diem END) AS Thang1,
        MAX(CASE WHEN kq.Loai = ''Thang2_Ki' + CAST(@ki AS VARCHAR) + ''' THEN kq.Diem END) AS Thang2,
        MAX(CASE WHEN kq.Loai = ''Thang3_Ki' + CAST(@ki AS VARCHAR) + ''' THEN kq.Diem END) AS Thang3,
        MAX(CASE WHEN kq.Loai = ''GiuaKi' + CAST(@ki AS VARCHAR) + ''' THEN kq.Diem END) AS GiuaKi,
        MAX(CASE WHEN kq.Loai = ''CuoiKi' + CAST(@ki AS VARCHAR) + ''' THEN kq.Diem END) AS CuoiKi,
        MAX(CASE WHEN kq.Loai LIKE @loaiFilter THEN kq.NhanXet END) AS NhanXet,
        MAX(CASE WHEN kq.Loai LIKE @loaiFilter THEN kq.GhiChu END) AS GhiChu
    FROM HocSinh hs
    JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS AND kq.MaMon = @maMon
    WHERE hs.MaLop = @malop
    GROUP BY hs.MaHS, hs.HoTen, lh.TenLop
    ORDER BY hs.HoTen';

    EXEC sp_executesql @sql, 
        N'@malop VARCHAR(10), @maMon VARCHAR(10), @loaiFilter NVARCHAR(10)', 
        @malop, @maMon, @loaiFilter;
END;
GO
-- #endregion

-- #region Hồ sơ cá nhân (Profile)
CREATE PROCEDURE sp_UpdateTeacherProfile
    @u NVARCHAR(50),
    @e NVARCHAR(50),
    @s VARCHAR(15),
    @a NVARCHAR(200)
AS
BEGIN
    UPDATE GiaoVien 
    SET Email=@e, SDT=@s, AnhDaiDien=@a
    WHERE Username=@u;
END;
GO
CREATE PROCEDURE sp_ChangeTeacherPassword
    @u NVARCHAR(50),
    @oldPass VARCHAR(30),
    @newPass VARCHAR(30)
AS
BEGIN
    DECLARE @currentPass VARCHAR(30);
    SELECT @currentPass = Password FROM GiaoVien WHERE Username=@u OR Ten =@u;

    IF @currentPass IS NULL OR @currentPass != @oldPass
    BEGIN
        SELECT 0; -- Thất bại (mật khẩu cũ sai)
        RETURN;
    END

    UPDATE GiaoVien SET Password=@newPass WHERE Username=@u OR Ten =@u;
    SELECT 1; -- Thành công
END;
GO
CREATE PROCEDURE sp_GetAdminProfile
    @u NVARCHAR(50)
AS
BEGIN
    SELECT * FROM Admin WHERE Username=@u;
END;
GO
CREATE PROCEDURE sp_UpdateAdminEmail
    @u NVARCHAR(50),
    @e NVARCHAR(50)
AS
BEGIN
    UPDATE Admin SET Email=@e WHERE Username=@u;
END;
GO
CREATE PROCEDURE sp_ChangeAdminPassword
    @u NVARCHAR(50),
    @oldPass VARCHAR(30),
    @newPass VARCHAR(30)
AS
BEGIN
    DECLARE @currentPass VARCHAR(30);
    SELECT @currentPass = Password FROM Admin WHERE Username=@u;

    IF @currentPass IS NULL OR @currentPass != @oldPass
    BEGIN
        SELECT 0; -- Thất bại
        RETURN;
    END

    UPDATE Admin SET Password=@newPass WHERE Username=@u;
    SELECT 1; -- Thành công
END;
GO
-- #endregion

-- #region Tài liệu
CREATE PROCEDURE sp_GetTaiLieuByGV
    @gv VARCHAR(10)
AS
BEGIN
    SELECT MaTL, TenTL, MoTa, Kieu, NgayTaiLen, TrangThaiChiaSe 
    FROM TaiLieu 
    WHERE MaGV=@gv;
END;
GO
CREATE PROCEDURE sp_InsertTaiLieu
    @gv VARCHAR(10),
    @ten NVARCHAR(100),
    @moTa NVARCHAR(200),
    @kieu NVARCHAR(200),
    @tt NVARCHAR(20)
AS
BEGIN
    INSERT INTO TaiLieu (MaTL, TenTL, MoTa, Kieu, NgayTaiLen, TrangThaiChiaSe, MaGV)
    VALUES(LEFT(NEWID(), 10), @ten, @moTa, @kieu, GETDATE(), @tt, @gv);
END;
GO
CREATE PROCEDURE sp_GetTaiLieuShared
AS
BEGIN
    SELECT MaTL, TenTL, MoTa, Kieu, NgayTaiLen, TrangThaiChiaSe 
    FROM TaiLieu 
    WHERE TrangThaiChiaSe=N'Chia sẻ';
END;
GO
CREATE PROCEDURE sp_DeleteTaiLieu
    @id VARCHAR(10)
AS
BEGIN
    DELETE FROM TaiLieu WHERE MaTL=@id;
END;
GO
CREATE PROCEDURE sp_ShareTaiLieu
    @id VARCHAR(10)
AS
BEGIN
    UPDATE TaiLieu SET TrangThaiChiaSe=N'Chia sẻ' WHERE MaTL=@id;
END;
GO
CREATE PROCEDURE sp_GetMaGVByUsername
    @u NVARCHAR(50)
AS
BEGIN
    SELECT MaGV FROM GiaoVien WHERE Username=@u OR Ten=@u;
END;
GO
-- #endregion

-- #region Thời khóa biểu
CREATE PROCEDURE sp_GetTKBByGV
    @MaGV VARCHAR(10),
    @Monday DATE,
    @Sunday DATE
AS
BEGIN
    SELECT 
        t.Ngay, 
        t.Tiet, 
        ISNULL(m.TenMon, '') AS TenMon,
        ISNULL(l.TenLop, '') AS TenLop,
        ISNULL(t.GhiChu, '') AS GhiChu,
        ISNULL(t.MauSac, '') AS MauSac
    FROM ThoiKhoaBieu t
    LEFT JOIN MonHoc m ON t.MaMon = m.MaMon
    LEFT JOIN LopHoc l ON t.MaLop = l.MaLop
    WHERE t.MaGV = @MaGV 
      AND t.Ngay >= @Monday 
      AND t.Ngay <= @Sunday;
END;
GO
CREATE PROCEDURE sp_UpsertTKBColor
    @MaGV VARCHAR(10),
    @Ngay DATE,
    @Tiet INT,
    @MauSac VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaTKB VARCHAR(10);
    SELECT @MaTKB = MaTKB FROM ThoiKhoaBieu WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet;

    IF @MaTKB IS NOT NULL
    BEGIN
        UPDATE ThoiKhoaBieu SET MauSac=@MauSac WHERE MaTKB=@MaTKB;
    END
    ELSE
    BEGIN
        INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, MauSac, MaGV) 
        VALUES (LEFT(NEWID(), 10), @Ngay, @Tiet, @MauSac, @MaGV);
    END
END;
GO
CREATE PROCEDURE sp_UpsertTKBGhiChu
    @MaGV VARCHAR(10),
    @Ngay DATE,
    @Tiet INT,
    @Note NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaTKB VARCHAR(10);
    SELECT @MaTKB = MaTKB FROM ThoiKhoaBieu WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet;

    IF @MaTKB IS NOT NULL
    BEGIN
        UPDATE ThoiKhoaBieu SET GhiChu=@Note WHERE MaTKB=@MaTKB;
    END
    ELSE
    BEGIN
        INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, GhiChu, MaGV) 
        VALUES (LEFT(NEWID(), 10), @Ngay, @Tiet, @Note, @MaGV);
    END
END;
GO
-- #endregion

-- #region Minigame
CREATE PROCEDURE sp_GetMiniGames
AS
BEGIN
    SELECT MaMNG, Ten, DuLieu FROM Minigame ORDER BY MaMNG;
END;
GO
CREATE PROCEDURE sp_GetGameData
    @maMNG VARCHAR(10)
AS
BEGIN
    SELECT DuLieu FROM Minigame WHERE MaMNG = @maMNG;
END;
GO
CREATE PROCEDURE sp_SaveGameData
    @maMNG VARCHAR(10),
    @data NVARCHAR(MAX)
AS
BEGIN
    UPDATE Minigame SET DuLieu = @data WHERE MaMNG = @maMNG;
END;
GO
-- #endregion

-- #region Quản lý
CREATE PROCEDURE sp_DeleteTKBGhiChu
    @MaGV VARCHAR(10),
    @Ngay DATE,
    @Tiet INT
AS
BEGIN
    DECLARE @maMon VARCHAR(10);
    DECLARE @maTKB VARCHAR(10);

    SELECT @maTKB = MaTKB, @maMon = MaMon 
    FROM ThoiKhoaBieu 
    WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet;

    IF @maTKB IS NULL RETURN;

    IF @maMon IS NULL
    BEGIN
        DELETE FROM ThoiKhoaBieu WHERE MaTKB=@maTKB;
    END
    ELSE
    BEGIN
        UPDATE ThoiKhoaBieu SET GhiChu = NULL WHERE MaTKB=@maTKB;
    END
END;
GO
CREATE PROCEDURE sp_GetGiaoVienByTrangThai
    @tt NVARCHAR(20)
AS
BEGIN
    SELECT MaGV, Ten, Username, Email, SDT, TrangThai 
    FROM GiaoVien 
    WHERE TrangThai = @tt;
END;
GO
CREATE PROCEDURE sp_UpdateTrangThaiGiaoVien
    @id VARCHAR(10),
    @tt NVARCHAR(20)
AS
BEGIN
    UPDATE GiaoVien SET TrangThai=@tt WHERE MaGV=@id;
END;
GO
CREATE PROCEDURE sp_UpdateGiaoVien
    @id VARCHAR(10),
    @t NVARCHAR(100),
    @e NVARCHAR(50),
    @s VARCHAR(15)
AS
BEGIN
    UPDATE GiaoVien SET Ten=@t, Email=@e, SDT=@s WHERE MaGV=@id;
END;
GO
CREATE PROCEDURE sp_DeleteGiaoVien
    @id VARCHAR(10)
AS
BEGIN
    DELETE FROM GiaoVien WHERE MaGV=@id;
END;
GO
CREATE PROCEDURE sp_GetAllGiaoVien
AS
BEGIN
    SELECT MaGV, Ten, Username, Email, SDT, TrangThai, MaMon FROM GiaoVien;
END;
GO
CREATE PROCEDURE sp_GetAllHocSinh
AS
BEGIN
    SELECT MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc 
    FROM HocSinh 
    ORDER BY MaLop, HoTen;
END;
GO
CREATE PROCEDURE sp_InsertHocSinh
    @MaHS VARCHAR(10),
    @MaLop VARCHAR(10),
    @HoTen NVARCHAR(100),
    @NgaySinh DATE,
    @GioiTinh NVARCHAR(10),
    @SDT VARCHAR(15),
    @DiaChi NVARCHAR(200),
    @DanToc NVARCHAR(50)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM HocSinh WHERE MaHS=@MaHS)
    BEGIN
        DECLARE @ErrorMsg NVARCHAR(100) = N'Học sinh ' + @MaHS + N' đã tồn tại.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END

    INSERT INTO HocSinh (MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc)
    VALUES (@MaHS, @MaLop, @HoTen, @NgaySinh, @GioiTinh, @SDT, @DiaChi, @DanToc);
END;
GO
CREATE PROCEDURE sp_UpdateHocSinh
    @MaHS VARCHAR(10),
    @HoTen NVARCHAR(100),
    @NgaySinh DATE,
    @GioiTinh NVARCHAR(10),
    @SDT VARCHAR(15),
    @DiaChi NVARCHAR(200),
    @DanToc NVARCHAR(50)
AS
BEGIN
    UPDATE HocSinh
    SET HoTen=@HoTen, NgaySinh=@NgaySinh, GioiTinh=@GioiTinh,
        SDTPhuHuynh=@SDT, DiaChi=@DiaChi, DanToc=@DanToc
    WHERE MaHS=@MaHS;
END;
GO
CREATE PROCEDURE sp_DeleteHocSinh
    @MaHS VARCHAR(10)
AS
BEGIN
    DELETE FROM HocSinh WHERE MaHS=@MaHS;
END;
GO
CREATE PROCEDURE sp_ImportHocSinh
    @HocSinhData ut_HocSinhImport READONLY
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT * INTO #TempHocSinh FROM @HocSinhData;

    DECLARE @Skipped INT;
    SELECT @Skipped = COUNT(t.MaHS) 
    FROM #TempHocSinh t
    INNER JOIN HocSinh hs ON t.MaHS = hs.MaHS;

    INSERT INTO HocSinh (MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc)
    SELECT 
        t.MaHS, t.MaLop, t.HoTen, t.NgaySinh, t.GioiTinh, t.SDTPhuHuynh, t.DiaChi, t.DanToc
    FROM #TempHocSinh t
    WHERE NOT EXISTS (
        SELECT 1 FROM HocSinh hs 
        WHERE hs.MaHS = t.MaHS
    );

    DECLARE @Success INT;
    SET @Success = @@ROWCOUNT;

    SELECT @Success AS [Success], @Skipped AS [Skipped];

    DROP TABLE #TempHocSinh;
END;
GO
-- #endregion

-- #region Báo cáo
CREATE PROCEDURE sp_GetLopByGiaoVien
    @maGV VARCHAR(10)
AS
BEGIN
    SELECT DISTINCT l.MaLop, l.TenLop 
    FROM LopHoc l
    JOIN PhanCongGiangDay pc ON l.MaLop = pc.MaLop
    WHERE pc.MaGV = @maGV
    UNION
    SELECT MaLop, TenLop 
    FROM LopHoc
    WHERE MaGVCN = @maGV;
END;
GO
CREATE PROCEDURE sp_GetBangDiemHocKy
    @maLop VARCHAR(10),
    @hocKy INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @monHocCols NVARCHAR(MAX), 
            @monHocColsSelect NVARCHAR(MAX), 
            @tongMon NVARCHAR(MAX),
            @sql NVARCHAR(MAX);
    DECLARE @loaiFilter NVARCHAR(10);
    DECLARE @monCount INT;

    SELECT @monHocCols = STUFF((SELECT DISTINCT ',' + QUOTENAME(TenMon) 
                                FROM MonHoc ORDER BY 1 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'');
    
    SELECT @monHocColsSelect = STUFF((SELECT DISTINCT ',ROUND(ISNULL(' + QUOTENAME(TenMon) + ', 0), 2) AS ' + QUOTENAME(TenMon)
                                    FROM MonHoc ORDER BY 1 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'');
    
    SELECT @tongMon = STUFF((SELECT DISTINCT ' + ISNULL(' + QUOTENAME(TenMon) + ', 0)'
                            FROM MonHoc ORDER BY 1 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,3,'');

    SELECT @monCount = COUNT(*) FROM MonHoc;
    
    IF @monCount = 0 OR @monHocCols IS NULL
    BEGIN
        SELECT MaHS, HoTen FROM HocSinh WHERE MaLop = @maLop;
        RETURN;
    END;
    
    SET @loaiFilter = CASE WHEN @hocKy = 3 THEN N'%Ki%' ELSE N'%Ki' + CAST(@hocKy AS VARCHAR) + N'%' END;

    SET @sql = N'
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
        SELECT MaHS, HoTen, TenLop, ' + @monHocCols + N'
        FROM DiemTB
        PIVOT
        (
            AVG(DiemTB)
            FOR TenMon IN (' + @monHocCols + N')
        ) AS PivotTable
    )
    SELECT MaHS, HoTen, ' + @monHocColsSelect + N',
           ROUND((' + @tongMon + N') / NULLIF(' + CAST(@monCount AS VARCHAR) + N', 0), 2) AS [Trung bình chung]
    FROM PivotData
    ORDER BY HoTen;';

    EXEC sp_executesql @sql, N'@maLop VARCHAR(10), @loaiFilter NVARCHAR(10)', @maLop, @loaiFilter;
END;
GO
CREATE PROCEDURE sp_GetHoSoHocSinh
    @maLop VARCHAR(10)
AS
BEGIN
    SELECT 
        MaHS, HoTen, GioiTinh, NgaySinh, DanToc, DiaChi, SDTPhuHuynh
    FROM HocSinh 
    WHERE MaLop = @maLop
    ORDER BY HoTen;
END;
GO
CREATE PROCEDURE sp_GetThongKeKhoi
    @khoi NVARCHAR(20)
AS
BEGIN
    SELECT 
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
    ORDER BY l.TenLop;
END;
GO
-- #endregion

-- #region Hỗ trợ Giảng dạy
CREATE PROCEDURE sp_AddGhiChuTKB
    @MaGV VARCHAR(10),
    @MaLop VARCHAR(10),
    @Ngay DATE,
    @Tiet INT,
    @GhiChu NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaTKB VARCHAR(10);
    SELECT @MaTKB = MaTKB 
    FROM ThoiKhoaBieu 
    WHERE MaLop = @MaLop AND Ngay = @Ngay AND Tiet = @Tiet;

    IF @MaTKB IS NOT NULL
    BEGIN
        UPDATE ThoiKhoaBieu 
        SET GhiChu = ISNULL(GhiChu, '') + NCHAR(13) + NCHAR(10) + @GhiChu, 
            MaGV = @MaGV 
        WHERE MaTKB = @MaTKB;
    END
    ELSE
    BEGIN
        INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, GhiChu, MaGV, MaLop)
        VALUES (LEFT(NEWID(), 10), @Ngay, @Tiet, @GhiChu, @MaGV, @MaLop);
    END
END;
GO
CREATE PROCEDURE sp_AddGhiChuChoHocSinh
    @MaHS VARCHAR(10),
    @MaMon VARCHAR(10),
    @GhiChu NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Loai NVARCHAR(20) = N'CuoiKi2'; 

    IF EXISTS (SELECT 1 FROM KetQuaHocTap WHERE MaHS = @MaHS AND MaMon = @MaMon AND Loai = @Loai)
    BEGIN
        UPDATE KetQuaHocTap SET GhiChu = @GhiChu WHERE MaHS = @MaHS AND MaMon = @MaMon AND Loai = @Loai;
    END
    ELSE
    BEGIN
        INSERT INTO KetQuaHocTap (MaKQ, MaMon, MaHS, NgayNhap, GhiChu, Loai)
        VALUES (LEFT(NEWID(), 10), @MaMon, @MaHS, GETDATE(), @GhiChu, @Loai);
    END
END;
GO
CREATE PROCEDURE sp_GetAllKetQuaHocTap
AS
BEGIN
    SELECT 
        hs.MaHS, hs.HoTen, lh.MaLop, lh.TenLop,
        mh.MaMon, mh.TenMon, kq.Loai, kq.Diem
    FROM KetQuaHocTap kq
    JOIN HocSinh hs ON kq.MaHS = hs.MaHS
    JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    JOIN MonHoc mh ON kq.MaMon = mh.MaMon
    WHERE kq.Diem IS NOT NULL;
END;
GO
-- #endregion

-- #region Chung
CREATE PROCEDURE sp_GetAllMonHoc
AS
BEGIN
    SELECT MaMon, TenMon FROM MonHoc ORDER BY TenMon;
END;
GO
CREATE PROCEDURE sp_GetScoresForAnalysis
    @maGV VARCHAR(10),
    @phamVi NVARCHAR(20),
    @chiTiet NVARCHAR(50),
    @maMon VARCHAR(10),
    @hocKy INT
AS
BEGIN
    DECLARE @kyFilter NVARCHAR(10) = N'%Ki' + CAST(@hocKy AS VARCHAR);
    DECLARE @sql NVARCHAR(MAX);

    SET @sql = N'
    SELECT 
        hs.MaHS, hs.HoTen, lh.MaLop, lh.TenLop,
        mh.MaMon, mh.TenMon, kq.Loai, kq.Diem
    FROM KetQuaHocTap kq
    JOIN HocSinh hs ON kq.MaHS = hs.MaHS
    JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    JOIN MonHoc mh ON kq.MaMon = mh.MaMon
    WHERE kq.Diem IS NOT NULL
    AND kq.Loai LIKE @kyFilter';

    IF @phamVi = 'LopGV'
    BEGIN
        SET @sql = @sql + N' AND lh.MaLop = @chiTiet';
    END
    ELSE IF @phamVi = 'Khoi'
    BEGIN
        SET @sql = @sql + N' AND lh.Khoi = @chiTiet';
    END

    IF @maMon IS NOT NULL AND @maMon != 'ALL'
    BEGIN
        SET @sql = @sql + N' AND mh.MaMon = @maMon';
    END

    EXEC sp_executesql @sql, 
        N'@kyFilter NVARCHAR(10), @chiTiet NVARCHAR(50), @maMon VARCHAR(10)', 
        @kyFilter, @chiTiet, @maMon;
END;
GO
CREATE PROCEDURE sp_CreateTeacherRequest
    @Ten NVARCHAR(100),
    @Username NVARCHAR(50),
    @Password VARCHAR(30),
    @MaMon VARCHAR(10),
    @Email NVARCHAR(50),
    @SDT VARCHAR(15)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM GiaoVien WHERE Username=@Username)
    BEGIN
        RAISERROR(N'Tên đăng nhập này đã tồn tại. Vui lòng chọn tên khác.', 16, 1);
        RETURN;
    END

    DECLARE @newId INT;
    SELECT @newId = ISNULL(MAX(CAST(SUBSTRING(MaGV, 3, LEN(MaGV)) AS INT)), 0) + 1 FROM GiaoVien;
    
    DECLARE @newMaGV VARCHAR(10) = 'GV' + RIGHT('000' + CAST(@newId AS VARCHAR), 3);

    INSERT INTO GiaoVien (MaGV, Ten, Username, Password, MaMon, Email, SDT, MaAdmin, TrangThai) 
    VALUES (@newMaGV, @Ten, @Username, @Password, @MaMon, @Email, @SDT, 'AD001', N'Chưa xác nhận');
END;
GO
CREATE PROCEDURE sp_GetTaiLieuSharedWithUploader
AS
BEGIN
    SELECT 
        tl.MaTL, tl.TenTL, tl.MoTa, tl.Kieu, tl.NgayTaiLen, 
        gv.Ten AS TenGV 
    FROM TaiLieu tl
    INNER JOIN GiaoVien gv ON tl.MaGV = gv.MaGV
    WHERE tl.TrangThaiChiaSe = N'Chia sẻ';
END;
GO
CREATE PROCEDURE sp_UnshareTaiLieu
    @MaTL VARCHAR(10)
AS
BEGIN
    UPDATE TaiLieu SET TrangThaiChiaSe = N'Riêng tư' WHERE MaTL = @MaTL;
END;
GO
CREATE PROCEDURE sp_GetHomeroomClassesByTeacher
    @maGV VARCHAR(10)
AS
BEGIN
    SELECT MaLop, TenLop FROM LopHoc WHERE MaGVCN = @maGV;
END;
GO
CREATE PROCEDURE sp_GetHomeroomClassNameByTeacherId
    @MaGV VARCHAR(10)
AS
BEGIN
    SELECT TenLop FROM LopHoc WHERE MaGVCN = @maGV;
END;
GO
CREATE PROCEDURE sp_GetBaoCaoChuyenCan
    @maLop VARCHAR(10),
    @hocKy INT
AS
BEGIN
    DECLARE @CurrentDate DATE = GETDATE();
    DECLARE @CurrentMonth INT = MONTH(@CurrentDate);
    DECLARE @CurrentYear INT = YEAR(@CurrentDate);
    DECLARE @NamHoc INT;

    IF @CurrentMonth >= 8 
    BEGIN
        SET @NamHoc = @CurrentYear + 1;
    END
    ELSE 
    BEGIN
        SET @NamHoc = @CurrentYear;
    END;
    
    DECLARE @StartDate DATE, @EndDate DATE;

    IF @hocKy = 1 
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHoc - 1, 8, 1);
        SET @EndDate = DATEFROMPARTS(@NamHoc - 1, 12, 31);
    END
    ELSE IF @hocKy = 2 
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHoc, 1, 1);
        SET @EndDate = DATEFROMPARTS(@NamHoc, 5, 31);
    END
    ELSE 
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHoc - 1, 8, 1);
        SET @EndDate = DATEFROMPARTS(@NamHoc, 5, 31);
    END;

    SELECT 
        hs.MaHS, hs.HoTen,
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
    ORDER BY hs.HoTen;
END;
GO
CREATE PROCEDURE sp_GetMonHocByGiaoVienAndLop
    @maGV VARCHAR(10),
    @maLop VARCHAR(10)
AS
BEGIN
    SELECT DISTINCT t.MaMon, m.TenMon 
    FROM ThoiKhoaBieu t
    JOIN MonHoc m ON t.MaMon = m.MaMon
    WHERE t.MaGV = @maGV AND t.MaLop = @maLop AND t.MaMon IS NOT NULL
    ORDER BY m.TenMon;
END;
GO
CREATE PROCEDURE sp_GetAllLopHoc
AS
BEGIN
    SELECT MaLop, TenLop, Khoi FROM LopHoc ORDER BY Khoi, TenLop;
END;
GO
CREATE PROCEDURE sp_GetUnassignedHomeroomTeachers
AS
BEGIN
    SELECT MaGV, Ten 
    FROM GiaoVien 
    WHERE TrangThai = N'Đã xác nhận' 
      AND MaGV NOT IN (SELECT DISTINCT MaGVCN FROM LopHoc WHERE MaGVCN IS NOT NULL);
END;
GO
CREATE PROCEDURE sp_UpdateGvcnForLop
    @MaLop VARCHAR(10),
    @MaGV VARCHAR(10)
AS
BEGIN
    UPDATE LopHoc SET MaGVCN = @MaGV WHERE MaLop = @MaLop;
END;
GO
CREATE PROCEDURE sp_GetLopHocDetails
    @MaLop VARCHAR(10)
AS
BEGIN
    SELECT 
        l.MaLop, l.TenLop, l.Khoi, l.NamHoc, 
        ISNULL(gv.Ten, N'Chưa có') AS TenGVCN,
        (SELECT COUNT(*) FROM HocSinh WHERE MaLop = l.MaLop) AS SiSo
    FROM LopHoc l
    LEFT JOIN GiaoVien gv ON l.MaGVCN = gv.MaGV
    WHERE l.MaLop = @MaLop;
END;
GO
CREATE PROCEDURE sp_GetPhanCongGiangDayByLop
    @MaLop VARCHAR(10)
AS
BEGIN
    SELECT 
        m.MaMon, m.TenMon,
        Assigned.MaGV,
        ISNULL(Assigned.TenGV, 'Chưa phân công') AS TenGV
    FROM MonHoc m
    LEFT JOIN (
        SELECT g.MaMon, g.MaGV, g.Ten as TenGV
        FROM PhanCongGiangDay pc
        JOIN GiaoVien g ON pc.MaGV = g.MaGV
        WHERE pc.MaLop = @MaLop
    ) AS Assigned ON m.MaMon = Assigned.MaMon
    ORDER BY m.TenMon;
END;
GO
CREATE PROCEDURE sp_UpdatePhanCong
    @MaLop VARCHAR(10),
    @MaMon VARCHAR(10),
    @NewMaGV VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @OldMaGV VARCHAR(10);
        SELECT @OldMaGV = pc.MaGV 
        FROM PhanCongGiangDay pc
        JOIN GiaoVien g ON pc.MaGV = g.MaGV
        WHERE pc.MaLop = @MaLop AND g.MaMon = @MaMon;

        IF @OldMaGV IS NOT NULL
        BEGIN
            DELETE FROM PhanCongGiangDay 
            WHERE MaLop = @MaLop AND MaGV = @OldMaGV;
        END

        IF @NewMaGV IS NOT NULL AND @NewMaGV != ''
        BEGIN
            INSERT INTO PhanCongGiangDay (MaGV, MaLop) 
            VALUES (@NewMaGV, @MaLop);
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END;
GO
CREATE PROCEDURE sp_ImportHocSinhToLop
    @MaLopTarget VARCHAR(10),
    @HocSinhData ut_HocSinhImport READONLY
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT * INTO #TempHocSinh FROM @HocSinhData;

    DECLARE @Skipped INT;
    SELECT @Skipped = COUNT(t.MaHS) 
    FROM #TempHocSinh t
    INNER JOIN HocSinh hs ON t.MaHS = hs.MaHS;

    INSERT INTO HocSinh (MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc)
    SELECT 
        t.MaHS, @MaLopTarget, t.HoTen, t.NgaySinh, t.GioiTinh, t.SDTPhuHuynh, t.DiaChi, t.DanToc
    FROM #TempHocSinh t
    WHERE NOT EXISTS (
        SELECT 1 FROM HocSinh hs 
        WHERE hs.MaHS = t.MaHS
    );

    DECLARE @Success INT;
    SET @Success = @@ROWCOUNT;

    SELECT @Success AS [Success], @Skipped AS [Skipped];

    DROP TABLE #TempHocSinh;
END;
GO
CREATE PROCEDURE sp_ImportTKBForGV
    @MaGV VARCHAR(10),
    @NgayList ut_DateList READONLY,
    @TKBData ut_TKBImport READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DELETE TKB
        FROM ThoiKhoaBieu TKB
        INNER JOIN @NgayList DL ON CAST(TKB.Ngay AS DATE) = DL.Ngay
        WHERE TKB.MaGV = @MaGV;

        SELECT * INTO #TempTKB FROM @TKBData;

        DECLARE @Failed INT = 0;
        DECLARE @Success INT = 0;

        INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, MaMon, MaLop, GhiChu, MauSac, MaGV)
        SELECT
            LEFT(NEWID(), 10),
            t.Ngay, t.Tiet, m.MaMon, l.MaLop,
            NULLIF(t.GhiChu, ''),
            NULLIF(t.MauSac, ''),
            @MaGV
        FROM #TempTKB t
        INNER JOIN MonHoc m ON t.TenMon = m.TenMon
        INNER JOIN LopHoc l ON t.TenLop = l.TenLop;

        SET @Success = @@ROWCOUNT;

        SELECT @Failed = COUNT(*)
        FROM #TempTKB t
        LEFT JOIN MonHoc m ON t.TenMon = m.TenMon
        LEFT JOIN LopHoc l ON t.TenLop = l.TenLop
        WHERE m.MaMon IS NULL OR l.MaLop IS NULL;

        COMMIT TRANSACTION;

        SELECT @Success AS [Success], @Failed AS [Failed];
        DROP TABLE #TempTKB;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        RAISERROR(N'Lỗi import TKB. Đã hoàn tác.', 16, 1);
        SELECT 0 AS [Success], 0 AS [Failed];
        IF OBJECT_ID('tempdb..#TempTKB') IS NOT NULL DROP TABLE #TempTKB;
    END CATCH
END;
GO
CREATE PROCEDURE sp_GetTeacherNameById
    @MaGV VARCHAR(10)
AS
BEGIN
    SELECT Ten FROM GiaoVien WHERE MaGV = @MaGV;
END;
GO
GO
CREATE PROCEDURE sp_GetQuyLopByLop
    @MaLop VARCHAR(10)
AS
BEGIN
    SELECT 
        MaQL,
        Ngay,
        GhiChu,
        Loai,
        SoTien
    FROM QuyLop 
    WHERE MaLop = @MaLop 
    ORDER BY Ngay, MaQL;
END;
GO
CREATE PROCEDURE sp_InsertQuyLop
    @MaLop VARCHAR(10),
    @Loai NVARCHAR(10),
    @SoTien DECIMAL(12,2),
    @Ngay DATE,
    @GhiChu NVARCHAR(200)
AS
BEGIN
    INSERT INTO QuyLop (MaQL, MaLop, Loai, SoTien, Ngay, GhiChu)
    VALUES (LEFT(NEWID(), 10), @MaLop, @Loai, @SoTien, @Ngay, @GhiChu);
END;
GO
CREATE PROCEDURE sp_DeleteQuyLop
    @MaQL VARCHAR(10)
AS
BEGIN
    DELETE FROM QuyLop WHERE MaQL = @MaQL;
END;
GO
-- #endregion
Go
ALTER PROCEDURE sp_GetThongKeKhoi 
    @khoi NVARCHAR(20) -- Đầu vào C# vẫn gửi là "Khối 5"
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @khoiFilter NVARCHAR(25) = @khoi;

    -- Bảng tạm 1: Chỉ đếm Sĩ số, Nam, Nữ (Không join KetQuaHocTap)
    -- Việc này đảm bảo mỗi học sinh CHỈ ĐƯỢC ĐẾM 1 LẦN.
    WITH StudentCounts AS (
        SELECT
            l.MaLop,
            l.TenLop,
            COUNT(hs.MaHS) AS SoHocSinh, -- Đếm tổng số HS trong lớp
            SUM(CASE WHEN hs.GioiTinh = N'Nam' THEN 1 ELSE 0 END) AS SoNam, -- Đếm số Nam
            SUM(CASE WHEN hs.GioiTinh = N'Nữ' THEN 1 ELSE 0 END) AS SoNu  -- Đếm số Nữ
        FROM LopHoc l
        LEFT JOIN HocSinh hs ON l.MaLop = hs.MaLop
        WHERE l.Khoi = @khoiFilter
        GROUP BY l.MaLop, l.TenLop
    ),
    
    -- Bảng tạm 2: Tính điểm trung bình (vẫn là cách tính cũ, 
    -- lấy TB của tất cả điểm, nhưng giờ nó không ảnh hưởng đến việc đếm)
    AvgScores AS (
        SELECT
            l.MaLop,
            ROUND(AVG(kq.Diem), 2) AS DiemTrungBinh
        FROM LopHoc l
        LEFT JOIN HocSinh hs ON l.MaLop = hs.MaLop
        LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS
        WHERE l.Khoi = @khoiFilter AND kq.Diem IS NOT NULL
        GROUP BY l.MaLop
    )
    
    -- Kết hợp hai bảng tạm lại
    SELECT
        sc.TenLop,
        sc.SoHocSinh,
        ISNULL(av.DiemTrungBinh, 0) AS DiemTrungBinh, -- Lấy điểm TB từ bảng tạm 2
        sc.SoNam,  -- Lấy số Nam từ bảng tạm 1
        sc.SoNu    -- Lấy số Nữ từ bảng tạm 1
    FROM StudentCounts sc
    LEFT JOIN AvgScores av ON sc.MaLop = av.MaLop
    ORDER BY sc.TenLop;
END;
GO
PRINT 'TẤT CẢ STORED PROCEDURES ĐÃ ĐƯỢC TẠO.';
PRINT 'QUÁ TRÌNH TÁI TẠO HOÀN TẤT!';