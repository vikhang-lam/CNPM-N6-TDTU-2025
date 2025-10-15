CREATE DATABASE quanlilophoc_giangday;
GO
USE quanlilophoc_giangday;
GO
select * from Giaovien
----------------
-- BẢNG KHÔNG CÓ KHÓA NGOẠI
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

--------------------------------------------------
-- BẢNG CÓ KHÓA NGOẠI
--------------------------------------------------
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

-- BẢNG TRUNG GIAN MỚI: QUAN HỆ NHIỀU-NHIỀU GIỮA GIÁO VIÊN VÀ LỚP HỌC
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
	ThoiGianCapNhat DATETIME.
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

--------------------------------------------------
-- DỮ LIỆU MẪU
--------------------------------------------------
-- 1. Admin & MonHoc
INSERT INTO Admin (MaAdmin, Username, Password, Email) VALUES ('AD001', 'admin', '123456', 'admin@example.com');
INSERT INTO MonHoc (MaMon, TenMon) VALUES ('VAN', N'Ngữ văn'), ('TOAN', N'Toán'), ('ANH', N'Tiếng Anh');

-- 2. LopHoc (Thêm lớp mới ở đây)
INSERT INTO LopHoc (MaLop, TenLop, Khoi, NamHoc) VALUES
('5A10', N'Lớp 5A10', N'Khối 5', '2025'),
('5A11', N'Lớp 5A11', N'Khối 5', '2025'),
('5A12', N'Lớp 5A12', N'Khối 5', '2025');

-- 3. GiaoVien (không còn MaLop)
INSERT INTO GiaoVien (MaGV, Ten, Username, Password, MaMon, Email, SDT, MaAdmin, TrangThai) VALUES
('GV001', N'Cô Minh Anh', 'minhanh', '123456', 'VAN', 'minhanh@example.com', '0905123456', 'AD001', N'Đã xác nhận'),
('GV002', N'Thầy Quốc Hưng', 'quochung', '123456', 'TOAN', 'quochung@example.com', '0912345002', 'AD001', N'Đã xác nhận'),
('GV003', N'Cô Thu Hà', 'thuha', '123456', 'ANH', 'thuha@example.com', '0912345003', 'AD001', N'Đã xác nhận'),
('GV004', N'Thầy Trung', 'quoTrung', '123456', NULL, 'quoctrung@example.com', '09123450012', 'AD001', N'Chưa xác nhận');

-- 4. Set GVCN cho LopHoc
UPDATE LopHoc SET MaGVCN = 'GV001' WHERE MaLop = '5A10';
-- Giả sử thầy Hưng làm GVCN lớp 5A11
UPDATE LopHoc SET MaGVCN = 'GV002' WHERE MaLop = '5A11';

-- 5. PhanCongGiangDay (Thêm phân công cho lớp mới ở đây)
INSERT INTO PhanCongGiangDay (MaGV, MaLop) VALUES
-- Lớp 5A10
('GV001', '5A10'),
('GV002', '5A10'),
('GV003', '5A10'),
-- Lớp 5A11
('GV001', '5A11'), -- Cô Minh Anh (Văn) dạy cả lớp 5A11
('GV002', '5A11'), -- Thầy Quốc Hưng (Toán) dạy cả lớp 5A11
-- Lớp 5A12
('GV002', '5A12'); -- Thầy Quốc Hưng (Toán) dạy cả lớp 5A12

-- 6. HocSinh
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS001', '5A10', N'Nguyễn Văn Nam', N'Kinh', N'Nam', '0912345678', N'Hà Nội', '2015-09-10'),
('HS002', '5A10', N'Trần Thị Lan', N'Kinh', N'Nữ', '0987654321', N'Hà Nội', '2015-04-15'),
('HS003', '5A10', N'Lê Hoàng Anh', N'Kinh', N'Nam', '0977123456', N'Hà Nội', '2015-12-01'),
-- Thêm học sinh cho lớp 5A11
('HS004', '5A11', N'Phạm Thị Bích', N'Kinh', N'Nữ', '0911223344', N'Hải Phòng', '2015-01-20'),
('HS005', '5A11', N'Đặng Văn Long', N'Kinh', N'Nam', '0922334455', N'Hà Nội', '2015-03-25');

-- 7. ThoiKhoaBieu
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
-- Thêm TKB cho lớp 5A11
('TKB011', '2025-10-06', 3, 'VAN', N'Giới thiệu tác phẩm mới', 'GV001', '5A11'),
('TKB012', '2025-10-07', 4, 'TOAN', N'Hình học', 'GV002', '5A11');

-- 8. Minigame
INSERT INTO Minigame (MaMNG, Ten, DuLieu) VALUES
('MNG01', N'Quiz nhanh', NULL), ('MNG02', N'Gọi tên ngẫu nhiên', NULL), ('MNG03', N'Flashcard', NULL),
('MNG04', N'Ghép chữ', NULL), ('MNG05', N'Nghe - chọn hình', NULL), ('MNG06', N'Sắp xếp câu', NULL),
('MNG07', N'Điền từ', NULL), ('MNG08', N'Lật thẻ', NULL), ('MNG09', N'Random số', NULL), ('MNG10', N'Pass a ball', NULL);


--------------------------------------------------
-- PROCEDURES & TRIGGERS (Không thay đổi)
--------------------------------------------------

-- PROCEDURE: TẠO ĐIỂM DANH MẶC ĐỊnh
GO
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

-- TRIGGER: TẠO ĐIỂM DANH KHI CÓ HỌC SINH MỚI
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

-- PROCEDURE: TẠO KẾT QUẢ HỌC TẬP MẶC ĐỊNH
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

-- TRIGGER: TẠO KẾT QUẢ HỌC TẬP KHI CÓ HỌC SINH MỚI
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
    -- Chỉ thực thi khi cột 'TrangThai' được cập nhật
    IF UPDATE(TrangThai)
    BEGIN
        UPDATE DiemDanh
        SET ThoiGianCapNhat = GETDATE() -- Lấy giờ hiện tại của hệ thống
        FROM DiemDanh
        INNER JOIN inserted ON DiemDanh.MaDD = inserted.MaDD;
    END
END;
GO


GO
CREATE PROCEDURE sp_GetHomeroomGradebook
    @MaLop VARCHAR(10),
    @LoaiDiem NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @cols AS NVARCHAR(MAX),
            @query AS NVARCHAR(MAX);

    -- Lấy danh sách các môn học có điểm để làm tên cột động
    SELECT @cols = STUFF((SELECT DISTINCT ',' + QUOTENAME(mh.TenMon) 
                    FROM KetQuaHocTap kq
                    JOIN MonHoc mh ON kq.MaMon = mh.MaMon
                    JOIN HocSinh hs ON kq.MaHS = hs.MaHS
                    WHERE hs.MaLop = @MaLop AND kq.Loai = @LoaiDiem AND kq.Diem IS NOT NULL
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

    -- Nếu không có môn nào có điểm, trả về bảng rỗng
    IF @cols IS NULL
    BEGIN
        SELECT MaHS, HoTen FROM HocSinh WHERE MaLop = @MaLop ORDER BY HoTen;
        RETURN;
    END

    -- Xây dựng câu lệnh PIVOT động
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
-- Chạy cho tất cả các lớp đã tạo
EXEC sp_TaoDiemDanhMacDinh @MaLop = '5A10';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '5A11';
EXEC sp_TaoKetQuaHocTapMacDinh;
GO

-- KIỂM TRA (Tùy chọn)
-- SELECT * FROM LopHoc;
-- SELECT * FROM GiaoVien;
-- SELECT * FROM PhanCongGiangDay;
-- GO
-- Cập nhật điểm cho học sinh lớp 5A10
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

-- Cập nhật điểm cho học sinh lớp 5A11
UPDATE KetQuaHocTap SET Diem = 4.5 WHERE MaHS = 'HS004' AND MaMon = 'TOAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS004' AND MaMon = 'TOAN' AND Loai = 'CuoiKi1'; -- Điểm tiến bộ
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS004' AND MaMon = 'VAN' AND Loai = 'CuoiKi1';


UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS005' AND MaMon = 'TOAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS005' AND MaMon = 'VAN' AND Loai = 'CuoiKi1';
GO
USE quanlilophoc_giangday;
GO


-- Báo cáo chuyên cần theo lớp
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

-- Báo cáo điểm số theo lớp
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

-- Thống kê điểm trung bình theo khối
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

-- Thống kê học lực theo khối
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