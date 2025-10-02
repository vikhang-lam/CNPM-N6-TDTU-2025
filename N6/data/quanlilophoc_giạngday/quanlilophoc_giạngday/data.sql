
USE quanlilophoc_giangday;
GO


--------------------------------------------------
-- 1. Bảng Admin
--------------------------------------------------
CREATE TABLE Admin (
    MaAdmin VARCHAR(10) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    Password VARCHAR(30) NOT NULL,
    Email NVARCHAR(50) NOT NULL
);

--------------------------------------------------
-- 2. Bảng Môn học
--------------------------------------------------
CREATE TABLE MonHoc (
    MaMon VARCHAR(10) PRIMARY KEY,
    TenMon NVARCHAR(50) NOT NULL
);

--------------------------------------------------
-- 3. Bảng Lớp học
--------------------------------------------------
CREATE TABLE LopHoc (
    MaLop VARCHAR(10) PRIMARY KEY,
    TenLop NVARCHAR(50) NOT NULL,
    Khoi NVARCHAR(20),
    NamHoc VARCHAR(10),
    MaGVCN VARCHAR(10)
);

--------------------------------------------------
-- 4. Bảng Giáo viên
--------------------------------------------------
CREATE TABLE GiaoVien (
    MaGV VARCHAR(10) PRIMARY KEY,
    Ten NVARCHAR(100) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    Password VARCHAR(30) NOT NULL,
    MaLop VARCHAR(10),
    MaMon VARCHAR(10),
    Email NVARCHAR(50),
    SDT VARCHAR(15),
    MaAdmin VARCHAR(10),
    AnhDaiDien NVARCHAR(200),
    TrangThai NVARCHAR(20) DEFAULT N'Chưa xác nhận',
    FOREIGN KEY (MaLop) REFERENCES LopHoc(MaLop),
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon),
    FOREIGN KEY (MaAdmin) REFERENCES Admin(MaAdmin)
);

ALTER TABLE LopHoc
ADD CONSTRAINT FK_LopHoc_GVCN FOREIGN KEY (MaGVCN) REFERENCES GiaoVien(MaGV);

--------------------------------------------------
-- 5. Bảng Học sinh
--------------------------------------------------
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

--------------------------------------------------
-- 6. Bảng Điểm danh
--------------------------------------------------
CREATE TABLE DiemDanh (
    MaDD VARCHAR(10) PRIMARY KEY,
    MaHS VARCHAR(10),
    NgayDD DATE,
    Buoi NVARCHAR(10),
    TrangThai NVARCHAR(20),
    FOREIGN KEY (MaHS) REFERENCES HocSinh(MaHS)
);

--------------------------------------------------
-- 7. Bảng Kết quả học tập
--------------------------------------------------
CREATE TABLE KetQuaHocTap (
    MaKQ VARCHAR(10) PRIMARY KEY,
    MaMon VARCHAR(10),
    MaHS VARCHAR(10),
    NgayNhap DATE,
    NhanXet NVARCHAR(200),
    GhiChu NVARCHAR(200),
    Loai NVARCHAR(20),   -- Thang1_Ki1, GiuaKi1, CuoiKi1, ...
    Diem FLOAT,
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon),
    FOREIGN KEY (MaHS) REFERENCES HocSinh(MaHS)
);

--------------------------------------------------
-- 8. Bảng Tài liệu
--------------------------------------------------
CREATE TABLE TaiLieu (
    MaTL VARCHAR(10) PRIMARY KEY,
    TenTL NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(200),
    Kieu NVARCHAR(200),
    NgayTaiLen DATE,
    TrangThaiChiaSe NVARCHAR(20),
    MaGV VARCHAR(10),
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV)
);

--------------------------------------------------
-- 9. Bảng Thời khóa biểu
--------------------------------------------------
CREATE TABLE ThoiKhoaBieu (
    MaTKB VARCHAR(10) PRIMARY KEY,
    Ngay DATE,
    Tiet INT,
    MaMon VARCHAR(10),
    GhiChu NVARCHAR(200),
    MaGV VARCHAR(10),
    MaLop VARCHAR(10),
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon),
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV),
    FOREIGN KEY (MaLop) REFERENCES LopHoc(MaLop)
);

--------------------------------------------------
-- 10. Bảng Báo cáo
--------------------------------------------------
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

--------------------------------------------------
-- 11. Bảng Quỹ lớp
--------------------------------------------------
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
-- 12. Bảng Minigame
--------------------------------------------------
CREATE TABLE Minigame (
    MaMNG VARCHAR(10) PRIMARY KEY,
    Ten NVARCHAR(100) NOT NULL,
    DuLieu NVARCHAR(MAX)
);

--------------------------------------------------
-- DỮ LIỆU MẪU
--------------------------------------------------
INSERT INTO Admin (MaAdmin, Username, Password, Email)
VALUES ('AD001', 'admin', '123456', 'admin@example.com');

INSERT INTO MonHoc (MaMon, TenMon) VALUES
('VAN', N'Ngữ văn'),
('TOAN', N'Toán'),
('ANH', N'Tiếng Anh');

INSERT INTO LopHoc (MaLop, TenLop, Khoi, NamHoc)
VALUES ('5A10', N'Lớp 5A10', N'Khối 5', '2025');

INSERT INTO GiaoVien (MaGV, Ten, Username, Password, MaLop, MaMon, Email, SDT, MaAdmin, AnhDaiDien, TrangThai)
VALUES (
    'GV001',
    N'Cô Minh Anh',
    'minhanh',
    '123456',
    '5A10',
    'VAN',
    'minhanh@example.com',
    '0905123456',
    'AD001',
    NULL,
    N'Đã xác nhận'
);

UPDATE LopHoc SET MaGVCN = 'GV001' WHERE MaLop = '5A10';

INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh)
VALUES
('HS001', '5A10', N'Nguyễn Văn Nam', N'Kinh', N'Nam', '0912345678', N'Hà Nội', '2015-09-10'),
('HS002', '5A10', N'Trần Thị Lan', N'Kinh', N'Nữ', '0987654321', N'Hà Nội', '2015-04-15'),
('HS003', '5A10', N'Lê Hoàng Anh', N'Kinh', N'Nam', '0977123456', N'Hà Nội', '2015-12-01');

--------------------------------------------------
-- PROCEDURE: TẠO ĐIỂM DANH MẶC ĐỊNH
--------------------------------------------------
IF OBJECT_ID('sp_TaoDiemDanhMacDinh', 'P') IS NOT NULL DROP PROCEDURE sp_TaoDiemDanhMacDinh;
GO
CREATE PROCEDURE sp_TaoDiemDanhMacDinh @MaLop VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @today DATE = CAST(GETDATE() AS DATE);

    INSERT INTO DiemDanh (MaDD, MaHS, NgayDD, Buoi, TrangThai)
    SELECT LEFT(NEWID(), 8), hs.MaHS, @today, N'Sáng', N'Có mặt'
    FROM HocSinh hs
    WHERE hs.MaLop = @MaLop
    AND NOT EXISTS (
        SELECT 1 FROM DiemDanh dd
        WHERE dd.MaHS = hs.MaHS AND dd.NgayDD = @today
    );
END;
GO


--------------------------------------------------
-- TRIGGER: KHI THÊM HỌC SINH MỚI THÌ TỰ TẠO ĐIỂM DANH HÔM NAY
--------------------------------------------------
IF OBJECT_ID('trg_TaoDiemDanhHocSinhMoi', 'TR') IS NOT NULL DROP TRIGGER trg_TaoDiemDanhHocSinhMoi;
GO
CREATE TRIGGER trg_TaoDiemDanhHocSinhMoi
ON HocSinh
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @today DATE = CAST(GETDATE() AS DATE);

    INSERT INTO DiemDanh (MaDD, MaHS, NgayDD, Buoi, TrangThai)
    SELECT LEFT(NEWID(), 8), i.MaHS, @today, N'Sáng', N'Có mặt'
    FROM INSERTED i;
END;
GO

--------------------------------------------------
-- PROCEDURE: TẠO KẾT QUẢ HỌC TẬP MẶC ĐỊNH (theo loại điểm)
--------------------------------------------------
IF OBJECT_ID('sp_TaoKetQuaHocTapMacDinh', 'P') IS NOT NULL DROP PROCEDURE sp_TaoKetQuaHocTapMacDinh;
GO
CREATE PROCEDURE sp_TaoKetQuaHocTapMacDinh
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @loai TABLE (Loai NVARCHAR(20));
    INSERT INTO @loai VALUES 
    (N'Thang1_Ki1'),(N'Thang2_Ki1'),(N'GiuaKi1'),(N'CuoiKi1'),
    (N'Thang1_Ki2'),(N'Thang2_Ki2'),(N'GiuaKi2'),(N'CuoiKi2');

    INSERT INTO KetQuaHocTap (MaKQ, MaMon, MaHS, NgayNhap, NhanXet, GhiChu, Loai, Diem)
    SELECT LEFT(NEWID(), 8), m.MaMon, hs.MaHS, GETDATE(), NULL, NULL, l.Loai, NULL
    FROM HocSinh hs
    CROSS JOIN MonHoc m
    CROSS JOIN @loai l
    WHERE NOT EXISTS (
        SELECT 1 FROM KetQuaHocTap kq 
        WHERE kq.MaHS = hs.MaHS AND kq.MaMon = m.MaMon AND kq.Loai = l.Loai
    );
END;
GO

--------------------------------------------------
-- TRIGGER: KHI THÊM HỌC SINH MỚI THÌ TỰ TẠO KẾT QUẢ HỌC TẬP MẶC ĐỊNH
--------------------------------------------------
IF OBJECT_ID('trg_TaoKetQuaHocTapHocSinhMoi', 'TR') IS NOT NULL DROP TRIGGER trg_TaoKetQuaHocTapHocSinhMoi;
GO
CREATE TRIGGER trg_TaoKetQuaHocTapHocSinhMoi
ON HocSinh
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @loai TABLE (Loai NVARCHAR(20));
    INSERT INTO @loai VALUES 
    (N'Thang1_Ki1'),(N'Thang2_Ki1'),(N'GiuaKi1'),(N'CuoiKi1'),
    (N'Thang1_Ki2'),(N'Thang2_Ki2'),(N'GiuaKi2'),(N'CuoiKi2');

    INSERT INTO KetQuaHocTap (MaKQ, MaMon, MaHS, NgayNhap, NhanXet, GhiChu, Loai, Diem)
    SELECT LEFT(NEWID(), 8), m.MaMon, i.MaHS, GETDATE(), NULL, NULL, l.Loai, NULL
    FROM INSERTED i
    CROSS JOIN MonHoc m
    CROSS JOIN @loai l;
END;
GO

--------------------------------------------------
-- KHỞI TẠO DỮ LIỆU MẶC ĐỊNH
--------------------------------------------------
EXEC sp_TaoDiemDanhMacDinh;
EXEC sp_TaoKetQuaHocTapMacDinh;

-- Kiểm tra
SELECT * FROM DiemDanh;
SELECT * FROM KetQuaHocTap;
