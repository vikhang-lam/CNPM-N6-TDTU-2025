

CREATE DATABASE quanlilophoc_giangday;
GO
USE quanlilophoc_giangday;
GO
PRINT 'ĐÃ TẠO DATABASE MỚI: quanlilophoc_giangday.';

--================================================================
-- BƯỚC 1: TẠO CÁC BẢNG 
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
    -- Luồng 1: Nếu Tài liệu bị xóa -> Xóa quyền này 
    FOREIGN KEY (MaTL) REFERENCES TaiLieu(MaTL) ON DELETE CASCADE,
    -- Luồng 2:Nếu Giáo viên (người nhận) bị xóa -> KHÔNG làm gì cả
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV) ON DELETE NO ACTION 
);
GO

PRINT 'ĐÃ TẠO TẤT CẢ CÁC BẢNG.';
GO

--================================================================
-- BƯỚC 2: CHÈN DỮ LIỆU MẪU
--================================================================

-- 1. Admin
INSERT INTO Admin (MaAdmin, Username, Password, Email) VALUES ('AD001', 'admin', '123456', 'admin@example.com');

-- 2. Môn Học
INSERT INTO MonHoc (MaMon, TenMon) VALUES 
('TV', N'Tiếng Việt'),
('TOAN', N'Toán'),
('KH', N'Khoa học'),
('LS_DL', N'Lịch sử và Địa lí'),
('ANH', N'Tiếng Anh'),
('DD', N'Đạo đức'),
('AN', N'Âm nhạc'),
('MT', N'Mĩ thuật'),
('TIN', N'Tin học và Công nghệ (Tin học)'),
('CN', N'Tin học và Công nghệ (Công nghệ)'),
('GDTC', N'Giáo dục thể chất'),
('TDT', N'Tiếng dân tộc'),
('HDTN', N'Hoạt động trải nghiệm');

-- 3. Lớp Học 
INSERT INTO LopHoc (MaLop, TenLop, Khoi, NamHoc) VALUES
('1A1', N'Lớp 1A1', N'Khối 1', '2025'),
('2A1', N'Lớp 2A1', N'Khối 2', '2025'),
('3A1', N'Lớp 3A1', N'Khối 3', '2025'),
('4A1', N'Lớp 4A1', N'Khối 4', '2025'),
('5A1', N'Lớp 5A1', N'Khối 5', '2025');

-- 4. Giáo Viên
INSERT INTO GiaoVien (MaGV, Ten, Username, Password, Email, SDT, MaAdmin, TrangThai) VALUES
('GV001', N'Cô Minh Anh', 'minhanh', '123456', 'minhanh@example.com', '0905123001', 'AD001', N'Đã xác nhận'),
('GV002', N'Thầy Quốc Hưng', 'quochung', '123456', 'quochung@example.com', '0912345002', 'AD001', N'Đã xác nhận'),
('GV003', N'Cô Thu Hà', 'thuha', '123456', 'thuha@example.com', '0912345003', 'AD001', N'Đã xác nhận'),
('GV004', N'Thầy Bá Trung', 'batrung', '123456', 'batrung@example.com', '0912345004', 'AD001', N'Đã xác nhận'),
('GV005', N'Cô Thanh Tâm', 'thanhtam', '123456', 'tam@example.com', '0912345005', 'AD001', N'Đã xác nhận');

-- 5. Giáo Viên - Môn Học
INSERT INTO GiaoVien_MonHoc (MaGV, MaMon) VALUES
('GV001', 'TV'), ('GV001', 'DD'), ('GV001', 'HDTN'),
('GV002', 'TOAN'), ('GV002', 'KH'),
('GV003', 'ANH'),
('GV004', 'TIN'), ('GV004', 'CN'),
('GV005', 'TV'), ('GV005', 'LS_DL');

-- 6. Phân Công GVCN cho Lớp
UPDATE LopHoc SET MaGVCN = 'GV001' WHERE MaLop = '1A1';
UPDATE LopHoc SET MaGVCN = 'GV002' WHERE MaLop = '2A1';
UPDATE LopHoc SET MaGVCN = 'GV005' WHERE MaLop = '3A1';
UPDATE LopHoc SET MaGVCN = 'GV002' WHERE MaLop = '4A1';
UPDATE LopHoc SET MaGVCN = 'GV005' WHERE MaLop = '5A1';

-- 7. Phân Công Giảng Dạy
INSERT INTO PhanCongGiangDay (MaGV, MaLop, MaMon) VALUES
('GV001', '1A1', 'TV'), ('GV002', '1A1', 'TOAN'), ('GV003', '1A1', 'ANH'), ('GV001', '1A1', 'DD'), ('GV001', '1A1', 'HDTN'),
('GV002', '2A1', 'TOAN'), ('GV002', '2A1', 'KH'), ('GV003', '2A1', 'ANH'), ('GV004', '2A1', 'TIN'),
('GV005', '3A1', 'TV'), ('GV002', '3A1', 'TOAN'), ('GV003', '3A1', 'ANH'), ('GV004', '3A1', 'TIN'), ('GV005', '3A1', 'LS_DL'),
('GV002', '4A1', 'TOAN'), ('GV002', '4A1', 'KH'), ('GV003', '4A1', 'ANH'), ('GV004', '4A1', 'TIN'), ('GV005', '4A1', 'LS_DL'),
('GV005', '5A1', 'TV'), ('GV002', '5A1', 'TOAN'), ('GV003', '5A1', 'ANH'), ('GV002', '5A1', 'KH'), ('GV005', '5A1', 'LS_DL'), ('GV004', '5A1', 'TIN');

-- 8. Học Sinh (100 HỌC SINH - 20 mỗi lớp)
-- Lớp 1A1
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS001', '1A1', N'Nguyễn Hoàng An', N'Kinh', N'Nam', '0912345001', N'Hà Nội', '2019-01-01'),
('HS002', '1A1', N'Trần Bảo Bình', N'Kinh', N'Nữ', '0912345002', N'Hà Nội', '2019-02-02'),
('HS003', '1A1', N'Lê Gia Cát', N'Kinh', N'Nam', '0912345003', N'Hà Nội', '2019-03-03'),
('HS004', '1A1', N'Phạm Minh Dũng', N'Kinh', N'Nam', '0912345004', N'Hà Nội', '2019-04-04'),
('HS005', '1A1', N'Đỗ Phương Giang', N'Kinh', N'Nữ', '0912345005', N'Hà Nội', '2019-05-05'),
('HS006', '1A1', N'Vũ Gia Hân', N'Kinh', N'Nữ', '0912345006', N'Hà Nội', '2019-06-06'),
('HS007', '1A1', N'Hoàng Tuấn Kiệt', N'Kinh', N'Nam', '0912345007', N'Hà Nội', '2019-07-07'),
('HS008', '1A1', N'Bùi Khánh Linh', N'Kinh', N'Nữ', '0912345008', N'Hà Nội', '2019-08-08'),
('HS009', '1A1', N'Đặng Quốc Minh', N'Kinh', N'Nam', '0912345009', N'Hà Nội', '2019-09-09'),
('HS010', '1A1', N'Ngô Bảo Nam', N'Kinh', N'Nam', '0912345010', N'Hà Nội', '2019-10-10'),
('HS011', '1A1', N'Hồ Thùy Oanh', N'Kinh', N'Nữ', '0912345011', N'Hà Nội', '2019-11-11'),
('HS012', '1A1', N'Dương Minh Phúc', N'Kinh', N'Nam', '0912345012', N'Hà Nội', '2019-12-12'),
('HS013', '1A1', N'Mai Tú Quyên', N'Kinh', N'Nữ', '0912345013', N'Hà Nội', '2019-01-13'),
('HS014', '1A1', N'Phan Hoàng Quân', N'Kinh', N'Nam', '0912345014', N'Hà Nội', '2019-02-14'),
('HS015', '1A1', N'Lý Gia Hân', N'Kinh', N'Nữ', '0912345015', N'Hà Nội', '2019-03-15'),
('HS016', '1A1', N'Vương Minh Tâm', N'Kinh', N'Nam', '0912345016', N'Hà Nội', '2019-04-16'),
('HS017', '1A1', N'Tô Phương Uyên', N'Kinh', N'Nữ', '0912345017', N'Hà Nội', '2019-05-17'),
('HS018', '1A1', N'Trịnh Tuấn Vũ', N'Kinh', N'Nam', '0912345018', N'Hà Nội', '2019-06-18'),
('HS019', '1A1', N'Cao Hoàng Yến', N'Kinh', N'Nữ', '0912345019', N'Hà Nội', '2019-07-19'),
('HS020', '1A1', N'Giang Minh Triết', N'Kinh', N'Nam', '0912345020', N'Hà Nội', '2019-08-20');

-- Lớp 2A1
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS021', '2A1', N'Nguyễn Văn A', N'Kinh', N'Nam', '0922345001', N'Hải Phòng', '2018-01-01'),
('HS022', '2A1', N'Trần Thị B', N'Kinh', N'Nữ', '0922345002', N'Hải Phòng', '2018-02-02'),
('HS023', '2A1', N'Lê Văn C', N'Kinh', N'Nam', '0922345003', N'Hải Phòng', '2018-03-03'),
('HS024', '2A1', N'Phạm Thị D', N'Kinh', N'Nữ', '0922345004', N'Hải Phòng', '2018-04-04'),
('HS025', '2A1', N'Đỗ Văn E', N'Kinh', N'Nam', '0922345005', N'Hải Phòng', '2018-05-05'),
('HS026', '2A1', N'Vũ Thị F', N'Kinh', N'Nữ', '0922345006', N'Hải Phòng', '2018-06-06'),
('HS027', '2A1', N'Hoàng Văn G', N'Kinh', N'Nam', '0922345007', N'Hải Phòng', '2018-07-07'),
('HS028', '2A1', N'Bùi Thị H', N'Kinh', N'Nữ', '0922345008', N'Hải Phòng', '2018-08-08'),
('HS029', '2A1', N'Đặng Văn I', N'Kinh', N'Nam', '0922345009', N'Hải Phòng', '2018-09-09'),
('HS030', '2A1', N'Ngô Văn K', N'Kinh', N'Nam', '0922345010', N'Hải Phòng', '2018-10-10'),
('HS031', '2A1', N'Hồ Thị L', N'Kinh', N'Nữ', '0922345011', N'Hải Phòng', '2018-11-11'),
('HS032', '2A1', N'Dương Văn M', N'Kinh', N'Nam', '0922345012', N'Hải Phòng', '2018-12-12'),
('HS033', '2A1', N'Mai Thị N', N'Kinh', N'Nữ', '0922345013', N'Hải Phòng', '2018-01-13'),
('HS034', '2A1', N'Phan Văn P', N'Kinh', N'Nam', '0922345014', N'Hải Phòng', '2018-02-14'),
('HS035', '2A1', N'Lý Thị Q', N'Kinh', N'Nữ', '0922345015', N'Hải Phòng', '2018-03-15'),
('HS036', '2A1', N'Vương Văn R', N'Kinh', N'Nam', '0922345016', N'Hải Phòng', '2018-04-16'),
('HS037', '2A1', N'Tô Thị S', N'Kinh', N'Nữ', '0922345017', N'Hải Phòng', '2018-05-17'),
('HS038', '2A1', N'Trịnh Văn T', N'Kinh', N'Nam', '0922345018', N'Hải Phòng', '2018-06-18'),
('HS039', '2A1', N'Cao Thị U', N'Kinh', N'Nữ', '0922345019', N'Hải Phòng', '2018-07-19'),
('HS040', '2A1', N'Giang Văn V', N'Kinh', N'Nam', '0922345020', N'Hải Phòng', '2018-08-20');

-- Lớp 3A1
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS041', '3A1', N'Nguyễn Ánh Dương', N'Kinh', N'Nam', '0932345001', N'Đà Nẵng', '2017-01-01'),
('HS042', '3A1', N'Trần Ngọc Bích', N'Kinh', N'Nữ', '0932345002', N'Đà Nẵng', '2017-02-02'),
('HS043', '3A1', N'Lê Minh Châu', N'Kinh', N'Nam', '0932345003', N'Đà Nẵng', '2017-03-03'),
('HS044', '3A1', N'Phạm Hải Đăng', N'Kinh', N'Nam', '0932345004', N'Đà Nẵng', '2017-04-04'),
('HS045', '3A1', N'Đỗ Hà Giang', N'Kinh', N'Nữ', '0932345005', N'Đà Nẵng', '2017-05-05'),
('HS046', '3A1', N'Vũ Hoàng Hải', N'Kinh', N'Nam', '0932345006', N'Đà Nẵng', '2017-06-06'),
('HS047', '3A1', N'Hoàng Khánh Huyền', N'Kinh', N'Nữ', '0932345007', N'Đà Nẵng', '2017-07-07'),
('HS048', '3A1', N'Bùi Minh Khang', N'Kinh', N'Nam', '0932345008', N'Đà Nẵng', '2017-08-08'),
('HS049', '3A1', N'Đặng Tuệ Lâm', N'Kinh', N'Nữ', '0932345009', N'Đà Nẵng', '2017-09-09'),
('HS050', '3A1', N'Ngô Gia Long', N'Kinh', N'Nam', '0932345010', N'Đà Nẵng', '2017-10-10'),
('HS051', '3A1', N'Hồ Ngọc Mai', N'Kinh', N'Nữ', '0932345011', N'Đà Nẵng', '2017-11-11'),
('HS052', '3A1', N'Dương Quốc Phong', N'Kinh', N'Nam', '0932345012', N'Đà Nẵng', '2017-12-12'),
('HS053', '3A1', N'Mai Bảo Quyên', N'Kinh', N'Nữ', '0932345013', N'Đà Nẵng', '2017-01-13'),
('HS054', '3A1', N'Phan Minh Sơn', N'Kinh', N'Nam', '0932345014', N'Đà Nẵng', '2017-02-14'),
('HS055', '3A1', N'Lý Thảo Trang', N'Kinh', N'Nữ', '0932345015', N'Đà Nẵng', '2017-03-15'),
('HS056', '3A1', N'Vương Anh Tuấn', N'Kinh', N'Nam', '0932345016', N'Đà Nẵng', '2017-04-16'),
('HS057', '3A1', N'Tô Diệp Vy', N'Kinh', N'Nữ', '0932345017', N'Đà Nẵng', '2017-05-17'),
('HS058', '3A1', N'Trịnh Xuân Trường', N'Kinh', N'Nam', '0932345018', N'Đà Nẵng', '2017-06-18'),
('HS059', '3A1', N'Cao Thùy Anh', N'Kinh', N'Nữ', '0932345019', N'Đà Nẵng', '2017-07-19'),
('HS060', '3A1', N'Giang Tuấn Phong', N'Kinh', N'Nam', '0932345020', N'Đà Nẵng', '2017-08-20');

-- Lớp 4A1
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS061', '4A1', N'Nguyễn Hoàng An', N'Kinh', N'Nam', '0912345001', N'Hà Nội', '2016-01-01'),
('HS062', '4A1', N'Trần Bảo Bình', N'Kinh', N'Nữ', '0912345002', N'Hà Nội', '2016-02-02'),
('HS063', '4A1', N'Lê Gia Cát', N'Kinh', N'Nam', '0912345003', N'Hà Nội', '2016-03-03'),
('HS064', '4A1', N'Phạm Minh Dũng', N'Kinh', N'Nam', '0912345004', N'Hà Nội', '2016-04-04'),
('HS065', '4A1', N'Đỗ Phương Giang', N'Kinh', N'Nữ', '0912345005', N'Hà Nội', '2016-05-05'),
('HS066', '4A1', N'Vũ Gia Hân', N'Kinh', N'Nữ', '0912345006', N'Hà Nội', '2016-06-06'),
('HS067', '4A1', N'Hoàng Tuấn Kiệt', N'Kinh', N'Nam', '0912345007', N'Hà Nội', '2016-07-07'),
('HS068', '4A1', N'Bùi Khánh Linh', N'Kinh', N'Nữ', '0912345008', N'Hà Nội', '2016-08-08'),
('HS069', '4A1', N'Đặng Quốc Minh', N'Kinh', N'Nam', '0912345009', N'Hà Nội', '2016-09-09'),
('HS070', '4A1', N'Ngô Bảo Nam', N'Kinh', N'Nam', '0912345010', N'Hà Nội', '2016-10-10'),
('HS071', '4A1', N'Hồ Thùy Oanh', N'Kinh', N'Nữ', '0912345011', N'Hà Nội', '2016-11-11'),
('HS072', '4A1', N'Dương Minh Phúc', N'Kinh', N'Nam', '0912345012', N'Hà Nội', '2016-12-12'),
('HS073', '4A1', N'Mai Tú Quyên', N'Kinh', N'Nữ', '0912345013', N'Hà Nội', '2016-01-13'),
('HS074', '4A1', N'Phan Hoàng Quân', N'Kinh', N'Nam', '0912345014', N'Hà Nội', '2016-02-14'),
('HS075', '4A1', N'Lý Gia Hân', N'Kinh', N'Nữ', '0912345015', N'Hà Nội', '2016-03-15'),
('HS076', '4A1', N'Vương Minh Tâm', N'Kinh', N'Nam', '0912345016', N'Hà Nội', '2016-04-16'),
('HS077', '4A1', N'Tô Phương Uyên', N'Kinh', N'Nữ', '0912345017', N'Hà Nội', '2016-05-17'),
('HS078', '4A1', N'Trịnh Tuấn Vũ', N'Kinh', N'Nam', '0912345018', N'Hà Nội', '2016-06-18'),
('HS079', '4A1', N'Cao Hoàng Yến', N'Kinh', N'Nữ', '0912345019', N'Hà Nội', '2016-07-19'),
('HS080', '4A1', N'Giang Minh Triết', N'Kinh', N'Nam', '0912345020', N'Hà Nội', '2016-08-20');

-- Lớp 5A1
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS081', '5A1', N'Nguyễn Hoàng An', N'Kinh', N'Nam', '0912345001', N'Hà Nội', '2015-01-01'),
('HS082', '5A1', N'Trần Bảo Bình', N'Kinh', N'Nữ', '0912345002', N'Hà Nội', '2015-02-02'),
('HS083', '5A1', N'Lê Gia Cát', N'Kinh', N'Nam', '0912345003', N'Hà Nội', '2015-03-03'),
('HS084', '5A1', N'Phạm Minh Dũng', N'Kinh', N'Nam', '0912345004', N'Hà Nội', '2015-04-04'),
('HS085', '5A1', N'Đỗ Phương Giang', N'Kinh', N'Nữ', '0912345005', N'Hà Nội', '2015-05-05'),
('HS086', '5A1', N'Vũ Gia Hân', N'Kinh', N'Nữ', '0912345006', N'Hà Nội', '2015-06-06'),
('HS087', '5A1', N'Hoàng Tuấn Kiệt', N'Kinh', N'Nam', '0912345007', N'Hà Nội', '2015-07-07'),
('HS088', '5A1', N'Bùi Khánh Linh', N'Kinh', N'Nữ', '0912345008', N'Hà Nội', '2015-08-08'),
('HS089', '5A1', N'Đặng Quốc Minh', N'Kinh', N'Nam', '0912345009', N'Hà Nội', '2015-09-09'),
('HS090', '5A1', N'Ngô Bảo Nam', N'Kinh', N'Nam', '0912345010', N'Hà Nội', '2015-10-10'),
('HS091', '5A1', N'Hồ Thùy Oanh', N'Kinh', N'Nữ', '0912345011', N'Hà Nội', '2015-11-11'),
('HS092', '5A1', N'Dương Minh Phúc', N'Kinh', N'Nam', '0912345012', N'Hà Nội', '2015-12-12'),
('HS093', '5A1', N'Mai Tú Quyên', N'Kinh', N'Nữ', '0912345013', N'Hà Nội', '2015-01-13'),
('HS094', '5A1', N'Phan Hoàng Quân', N'Kinh', N'Nam', '0912345014', N'Hà Nội', '2015-02-14'),
('HS095', '5A1', N'Lý Gia Hân', N'Kinh', N'Nữ', '0912345015', N'Hà Nội', '2015-03-15'),
('HS096', '5A1', N'Vương Minh Tâm', N'Kinh', N'Nam', '0912345016', N'Hà Nội', '2015-04-16'),
('HS097', '5A1', N'Tô Phương Uyên', N'Kinh', N'Nữ', '0912345017', N'Hà Nội', '2015-05-17'),
('HS098', '5A1', N'Trịnh Tuấn Vũ', N'Kinh', N'Nam', '0912345018', N'Hà Nội', '2015-06-18'),
('HS099', '5A1', N'Cao Hoàng Yến', N'Kinh', N'Nữ', '0912345019', N'Hà Nội', '2015-07-19'),
('HS100', '5A1', N'Giang Minh Triết', N'Kinh', N'Nam', '0912345020', N'Hà Nội', '2015-08-20');

-- 9. Thời Khóa Biểu
INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, MaMon, GhiChu, MaGV, MaLop) VALUES
('TKB001', '2025-10-06', 1, 'TV', N'Ôn tập chương 1', 'GV005', '5A1'),
('TKB002', '2025-10-06', 2, 'TOAN', N'Luyện tập cộng trừ phân số', 'GV002', '5A1'),
('TKB003', '2025-10-07', 1, 'ANH', N'Học từ vựng chủ đề gia đình', 'GV003', '5A1'),
('TKB004', '2025-10-07', 2, 'TOAN', N'Bài tập ứng dụng thực tế', 'GV002', '5A1'),
('TKB005', '2025-10-08', 3, 'TIN', N'Luyện gõ 10 ngón', 'GV004', '5A1'),
('TKB006', '2025-10-06', 1, 'TV', N'Học vần', 'GV001', '1A1'),
('TKB007', '2025-10-06', 2, 'TOAN', N'Học số đếm 1-10', 'GV002', '1A1');

-- 10. Minigame
INSERT INTO Minigame (MaMNG, Ten, DuLieu) VALUES
('MNG01', N'Quiz nhanh', NULL), ('MNG02', N'Gọi tên ngẫu nhiên', NULL), ('MNG03', N'Flashcard', NULL),
('MNG04', N'Ghép chữ', NULL), ('MNG05', N'Nghe - chọn hình', NULL), ('MNG06', N'Sắp xếp câu', NULL),
('MNG07', N'Điền từ', NULL), ('MNG08', N'Lật thẻ', NULL), ('MNG09', N'Random số', NULL), ('MNG10', N'Pass a ball', NULL);

-- 11. DỮ LIỆU MẪU MỚI: ThoiHanDiem
-- Khối 1
INSERT INTO ThoiHanDiem (MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong)
VALUES
('Thang1_Ki1', N'Điểm Tháng 1 (Kỳ 1)', N'Khối 1', 1, '2024-09-01', '2024-09-30', 0),
('Thang2_Ki1', N'Điểm Tháng 2 (Kỳ 1)', N'Khối 1', 1, '2024-10-01', '2024-10-31', 0),
('Thang3_Ki1', N'Điểm Tháng 3 (Kỳ 1)', N'Khối 1', 1, '2024-11-01', '2024-11-30', 0),
('GiuaKi1', N'Điểm Giữa Kỳ 1', N'Khối 1', 1, '2024-11-05', '2024-11-20', 0),
('CuoiKi1', N'Điểm Cuối Kỳ 1', N'Khối 1', 1, '2024-12-15', '2025-01-15', 0),
('Thang1_Ki2', N'Điểm Tháng 1 (Kỳ 2)', N'Khối 1', 2, '2025-01-20', '2025-02-28', 0),
('Thang2_Ki2', N'Điểm Tháng 2 (Kỳ 2)', N'Khối 1', 2, '2025-03-01', '2025-03-31', 0),
('Thang3_Ki2', N'Điểm Tháng 3 (Kỳ 2)', N'Khối 1', 2, '2025-04-01', '2025-04-30', 0),
('GiuaKi2', N'Điểm Giữa Kỳ 2', N'Khối 1', 2, '2025-04-05', '2025-04-20', 0),
('CuoiKi2', N'Điểm Cuối Kỳ 2', N'Khối 1', 2, '2025-05-15', '2025-06-15', 0);

-- Khối 2
INSERT INTO ThoiHanDiem (MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong)
VALUES
('Thang1_Ki1', N'Điểm Tháng 1 (Kỳ 1)', N'Khối 2', 1, '2024-09-01', '2024-09-30', 0),
('Thang2_Ki1', N'Điểm Tháng 2 (Kỳ 1)', N'Khối 2', 1, '2024-10-01', '2024-10-31', 0),
('Thang3_Ki1', N'Điểm Tháng 3 (Kỳ 1)', N'Khối 2', 1, '2024-11-01', '2024-11-30', 0),
('GiuaKi1', N'Điểm Giữa Kỳ 1', N'Khối 2', 1, '2024-11-05', '2024-11-20', 0),
('CuoiKi1', N'Điểm Cuối Kỳ 1', N'Khối 2', 1, '2024-12-15', '2025-01-15', 0),
('Thang1_Ki2', N'Điểm Tháng 1 (Kỳ 2)', N'Khối 2', 2, '2025-01-20', '2025-02-28', 0),
('Thang2_Ki2', N'Điểm Tháng 2 (Kỳ 2)', N'Khối 2', 2, '2025-03-01', '2025-03-31', 0),
('Thang3_Ki2', N'Điểm Tháng 3 (Kỳ 2)', N'Khối 2', 2, '2025-04-01', '2025-04-30', 0),
('GiuaKi2', N'Điểm Giữa Kỳ 2', N'Khối 2', 2, '2025-04-05', '2025-04-20', 0),
('CuoiKi2', N'Điểm Cuối Kỳ 2', N'Khối 2', 2, '2025-05-15', '2025-06-15', 0);

-- Khối 3
INSERT INTO ThoiHanDiem (MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong)
VALUES
('Thang1_Ki1', N'Điểm Tháng 1 (Kỳ 1)', N'Khối 3', 1, '2024-09-01', '2024-09-30', 0),
('Thang2_Ki1', N'Điểm Tháng 2 (Kỳ 1)', N'Khối 3', 1, '2024-10-01', '2024-10-31', 0),
('Thang3_Ki1', N'Điểm Tháng 3 (Kỳ 1)', N'Khối 3', 1, '2024-11-01', '2024-11-30', 0),
('GiuaKi1', N'Điểm Giữa Kỳ 1', N'Khối 3', 1, '2024-11-01', '2024-11-15', 0),
('CuoiKi1', N'Điểm Cuối Kỳ 1', N'Khối 3', 1, '2024-12-15', '2025-01-15', 0),
('Thang1_Ki2', N'Điểm Tháng 1 (Kỳ 2)', N'Khối 3', 2, '2025-01-20', '2025-02-28', 0),
('Thang2_Ki2', N'Điểm Tháng 2 (Kỳ 2)', N'Khối 3', 2, '2025-03-01', '2025-03-31', 0),
('Thang3_Ki2', N'Điểm Tháng 3 (Kỳ 2)', N'Khối 3', 2, '2025-04-01', '2025-04-30', 0),
('GiuaKi2', N'Điểm Giữa Kỳ 2', N'Khối 3', 2, '2025-04-01', '2025-04-15', 0),
('CuoiKi2', N'Điểm Cuối Kỳ 2', N'Khối 3', 2, '2025-05-15', '2025-06-15', 0);

-- Khối 4
INSERT INTO ThoiHanDiem (MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong)
VALUES
('Thang1_Ki1', N'Điểm Tháng 1 (Kỳ 1)', N'Khối 4', 1, '2024-09-01', '2024-09-30', 0),
('Thang2_Ki1', N'Điểm Tháng 2 (Kỳ 1)', N'Khối 4', 1, '2024-10-01', '2024-10-31', 0),
('Thang3_Ki1', N'Điểm Tháng 3 (Kỳ 1)', N'Khối 4', 1, '2024-11-01', '2024-11-30', 0),
('GiuaKi1', N'Điểm Giữa Kỳ 1', N'Khối 4', 1, '2024-11-01', '2024-11-15', 0),
('CuoiKi1', N'Điểm Cuối Kỳ 1', N'Khối 4', 1, '2024-12-15', '2025-01-15', 0),
('Thang1_Ki2', N'Điểm Tháng 1 (Kỳ 2)', N'Khối 4', 2, '2025-01-20', '2025-02-28', 0),
('Thang2_Ki2', N'Điểm Tháng 2 (Kỳ 2)', N'Khối 4', 2, '2025-03-01', '2025-03-31', 0),
('Thang3_Ki2', N'Điểm Tháng 3 (Kỳ 2)', N'Khối 4', 2, '2025-04-01', '2025-04-30', 0),
('GiuaKi2', N'Điểm Giữa Kỳ 2', N'Khối 4', 2, '2025-04-01', '2025-04-15', 0),
('CuoiKi2', N'Điểm Cuối Kỳ 2', N'Khối 4', 2, '2025-05-15', '2025-06-15', 0);

-- Khối 5
INSERT INTO ThoiHanDiem (MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong)
VALUES
('Thang1_Ki1', N'Điểm Tháng 1 (Kỳ 1)', N'Khối 5', 1, '2024-09-01', '2024-09-30', 0),
('Thang2_Ki1', N'Điểm Tháng 2 (Kỳ 1)', N'Khối 5', 1, '2024-10-01', '2024-10-31', 0),
('Thang3_Ki1', N'Điểm Tháng 3 (Kỳ 1)', N'Khối 5', 1, '2024-11-01', '2024-11-30', 0),
('GiuaKi1', N'Điểm Giữa Kỳ 1', N'Khối 5', 1, '2024-10-20', '2024-11-10', 0),
('CuoiKi1', N'Điểm Cuối Kỳ 1', N'Khối 5', 1, '2024-12-10', '2025-01-10', 0),
('Thang1_Ki2', N'Điểm Tháng 1 (Kỳ 2)', N'Khối 5', 2, '2025-01-20', '2025-02-28', 0),
('Thang2_Ki2', N'Điểm Tháng 2 (Kỳ 2)', N'Khối 5', 2, '2025-03-01', '2025-03-31', 0),
('Thang3_Ki2', N'Điểm Tháng 3 (Kỳ 2)', N'Khối 5', 2, '2025-04-01', '2025-04-30', 0),
('GiuaKi2', N'Điểm Giữa Kỳ 2', N'Khối 5', 2, '2025-03-20', '2025-04-10', 0),
('CuoiKi2', N'Điểm Cuối Kỳ 2', N'Khối 5', 2, '2025-05-10', '2025-06-10', 0);
GO

PRINT 'ĐÃ CHÈN TẤT CẢ DỮ LIỆU MẪU.';
GO

--================================================================
-- BƯỚC 3: TẠO CÁC TRIGGERS
--================================================================

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

CREATE TRIGGER trg_TaoKetQuaHocTapHocSinhMoi ON HocSinh AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @loai TABLE (Loai NVARCHAR(20));
    INSERT INTO @loai (Loai)
    SELECT DISTINCT MaCotDiem FROM ThoiHanDiem;

    IF NOT EXISTS (SELECT 1 FROM @loai)
    BEGIN
        PRINT N'Không có loại điểm nào trong ThoiHanDiem. Bỏ qua Trigger trg_TaoKetQuaHocTapHocSinhMoi.';
        RETURN;
    END

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

--================================================================
-- BƯỚC 4: TẠO CÁC TABLE TYPES (CHO IMPORT)
--================================================================

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
    TenMon NVARCHAR(100),
    TenLop NVARCHAR(50),
    GhiChu NVARCHAR(200) NULL,
    MauSac VARCHAR(20) NULL
);
GO

CREATE TYPE ut_MaMonList AS TABLE(
    MaMon VARCHAR(10) PRIMARY KEY
);
GO

CREATE TYPE ut_MaHSList AS TABLE(
    MaHS VARCHAR(10) PRIMARY KEY
);
GO

PRINT 'ĐÃ TẠO CÁC TABLE TYPES.';
GO

--================================================================
-- BƯỚC 5: TẠO TẤT CẢ CÁC STORED PROCEDURE THEO MODULE
--================================================================

-- ================================================================
-- MODULE 1: ĐĂNG NHẬP & XÁC THỰC
-- ================================================================
PRINT 'TẠO STORED PROCEDURE CHO MODULE ĐĂNG NHẬP & XÁC THỰC...';
GO

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

Create PROCEDURE sp_CreateTeacherRequest
    @Ten NVARCHAR(100),
    @Username NVARCHAR(50),
    @Password VARCHAR(30),
    @Email NVARCHAR(50),
    @SDT VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Kiểm tra Tên đăng nhập
    IF EXISTS (SELECT 1 FROM GiaoVien WHERE Username=@Username)
    BEGIN
        RAISERROR(N'Tên đăng nhập này đã tồn tại. Vui lòng chọn tên khác.', 16, 1);
        RETURN;
    END

    -- [CẬP NHẬT] Thêm kiểm tra Email
    IF EXISTS (SELECT 1 FROM GiaoVien WHERE Email=@Email)
    BEGIN
        RAISERROR(N'Email này đã tồn tại. Vui lòng sử dụng email khác.', 16, 1);
        RETURN;
    END

    -- Tiếp tục logic tạo mã GV nếu không trùng
    DECLARE @newId INT;
    SELECT @newId = ISNULL(MAX(CAST(SUBSTRING(MaGV, 3, LEN(MaGV)) AS INT)), 0) + 1 FROM GiaoVien;
    
    DECLARE @newMaGV VARCHAR(10) = 'GV' + RIGHT('00' + CAST(@newId AS VARCHAR), 3);

    INSERT INTO GiaoVien (MaGV, Ten, Username, Password, Email, SDT, MaAdmin, TrangThai) 
    VALUES (@newMaGV, @Ten, @Username, @Password, @Email, @SDT, 'AD001', N'Chưa xác nhận');
END;
GO

CREATE PROCEDURE sp_RequestPasswordReset
    @UsernameOrEmail NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Email NVARCHAR(50);
    DECLARE @MaGV VARCHAR(10);
    DECLARE @OTP VARCHAR(6);
    
    SELECT @Email = Email, @MaGV = MaGV
    FROM GiaoVien
    WHERE (Username = @UsernameOrEmail OR Email = @UsernameOrEmail)
      AND TrangThai = N'Đã xác nhận';

    IF @MaGV IS NOT NULL
    BEGIN
        SET @OTP = CAST(FLOOR(RAND() * (999999 - 100000 + 1) + 100000) AS VARCHAR(6));
        
        UPDATE GiaoVien
        SET ResetOTP = @OTP,
            OTPExpiry = DATEADD(minute, 10, GETDATE())
        WHERE MaGV = @MaGV;
        
        SELECT @Email AS Email, @OTP AS OTP;
        RETURN;
    END
    
    SELECT NULL AS Email, NULL AS OTP;
END;
GO

CREATE PROCEDURE sp_ResetPasswordWithOtp
    @UsernameOrEmail NVARCHAR(50),
    @OTP VARCHAR(6),
    @NewPassword VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @MaGV VARCHAR(10);
    DECLARE @StoredOTP VARCHAR(6);
    DECLARE @Expiry DATETIME;

    SELECT 
        @MaGV = MaGV,
        @StoredOTP = ResetOTP,
        @Expiry = OTPExpiry
    FROM GiaoVien
    WHERE (Username = @UsernameOrEmail OR Email = @UsernameOrEmail)
      AND TrangThai = N'Đã xác nhận';

    IF @MaGV IS NULL
    BEGIN
        SELECT 0;
        RETURN;
    END

    IF @StoredOTP IS NULL OR @StoredOTP != @OTP
    BEGIN
        SELECT 1;
        RETURN;
    END

    IF GETDATE() > @Expiry
    BEGIN
        SELECT 2;
        RETURN;
    END

    UPDATE GiaoVien
    SET Password = @NewPassword,
        ResetOTP = NULL,
        OTPExpiry = NULL
    WHERE MaGV = @MaGV;
    
    SELECT 100;
END;
GO

GO
create PROCEDURE sp_CheckOtp
    @UsernameOrEmail NVARCHAR(50),
    @OTP VARCHAR(6)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaGV VARCHAR(10);
    DECLARE @StoredOtp VARCHAR(6);
    DECLARE @OtpExpiry DATETIME;

    SELECT TOP 1 
        @MaGV = MaGV,
        @StoredOtp = ResetOTP,
        @OtpExpiry = OTPExpiry
    FROM GiaoVien
    WHERE 
        (Username = @UsernameOrEmail OR Email = @UsernameOrEmail)
        AND TrangThai = N'Đã xác nhận';

    IF @MaGV IS NULL
    BEGIN
        SELECT 0 AS Result; RETURN;
    END

    IF @StoredOtp IS NULL OR @StoredOtp != @OTP
    BEGIN
        SELECT 1 AS Result; RETURN;
    END

    IF @OtpExpiry < GETDATE()
    BEGIN
        SELECT 2 AS Result; RETURN;
    END

    SELECT 100 AS Result;
END
GO


-- ================================================================
-- MODULE 2: QUẢN LÝ HỒ SƠ (PROFILE)
-- ================================================================
PRINT 'TẠO STORED PROCEDURE CHO MODULE QUẢN LÝ HỒ SƠ...';
GO

CREATE PROCEDURE sp_GetTeacherProfile
    @user NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        gv.Ten, 
        gv.Email, 
        gv.SDT, 
        gv.AnhDaiDien,
        ISNULL(STUFF((
            SELECT N', ' + mh.TenMon
            FROM GiaoVien_MonHoc gvm
            JOIN MonHoc mh ON gvm.MaMon = mh.MaMon
            WHERE gvm.MaGV = gv.MaGV
            ORDER BY mh.TenMon
            FOR XML PATH('')
        ), 1, 2, N''), N'Chưa có môn') AS TenMon
    FROM GiaoVien gv
    WHERE gv.Username = @user OR gv.Ten = @user;
END;
GO

CREATE PROCEDURE sp_GetTeacherNameById
    @MaGV VARCHAR(10)
AS
BEGIN
    SELECT Ten FROM GiaoVien WHERE MaGV = @MaGV;
END;
GO

CREATE PROCEDURE sp_GetMaGVByUsername
    @u NVARCHAR(50)
AS
BEGIN
    SELECT MaGV FROM GiaoVien WHERE Username=@u OR Ten=@u;
END;
GO

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
        SELECT 0;
        RETURN;
    END

    UPDATE GiaoVien SET Password=@newPass WHERE Username=@u OR Ten =@u;
    SELECT 1;
END;
GO

CREATE PROCEDURE sp_GetAdminProfile
    @u NVARCHAR(50)
AS
BEGIN
    SELECT * FROM Admin WHERE Username=@u;
END;
GO

CREATE PROCEDURE sp_GetAdminEmail
    @MaAdmin VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Email FROM Admin WHERE MaAdmin = @MaAdmin;
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
        SELECT 0;
        RETURN;
    END

    UPDATE Admin SET Password=@newPass WHERE Username=@u;
    SELECT 1;
END;
GO

-- ================================================================
-- MODULE 3: QUẢN LÝ LỚP (GVCN)
-- ================================================================
PRINT 'TẠO STORED PROCEDURE CHO MODULE QUẢN LÝ LỚP (GVCN)...';
GO

-- 📊 ĐIỂM DANH
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

CREATE PROCEDURE sp_UpdateDiemDanh
    @MaDD VARCHAR(10),
    @TrangThai NVARCHAR(20)
AS
BEGIN
    UPDATE DiemDanh SET TrangThai=@TrangThai WHERE MaDD=@MaDD;
END;
GO

-- 📈 SỔ ĐIỂM
CREATE PROCEDURE sp_TaoKetQuaHocTapMacDinh
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @loai TABLE (Loai NVARCHAR(20));
    INSERT INTO @loai (Loai)
    SELECT DISTINCT MaCotDiem FROM ThoiHanDiem;

    IF NOT EXISTS (SELECT 1 FROM @loai)
    BEGIN
        PRINT N'Không có loại điểm nào trong ThoiHanDiem. Bỏ qua khởi tạo KetQuaHocTap.';
        RETURN;
    END

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
    DECLARE @khoi NVARCHAR(20);
    SELECT @khoi = Khoi FROM LopHoc WHERE MaLop = @malop;

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

    SET @sql = N'
    SELECT 
        hs.MaHS, 
        hs.HoTen,
        lh.TenLop,
        MAX(CASE WHEN kq.Loai = @Thang1Loai THEN kq.Diem END) AS Thang1,
        MAX(CASE WHEN kq.Loai = @Thang2Loai THEN kq.Diem END) AS Thang2,
        MAX(CASE WHEN kq.Loai = @Thang3Loai THEN kq.Diem END) AS Thang3,
        MAX(CASE WHEN kq.Loai = @GiuaKiLoai THEN kq.Diem END) AS GiuaKi,
        MAX(CASE WHEN kq.Loai = @CuoiKiLoai THEN kq.Diem END) AS CuoiKi,
        MAX(CASE WHEN kq.Loai LIKE @loaiFilter THEN kq.NhanXet END) AS NhanXet,
        MAX(CASE WHEN kq.Loai LIKE @loaiFilter THEN kq.GhiChu END) AS GhiChu
    FROM HocSinh hs
    JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS AND kq.MaMon = @maMon
    WHERE hs.MaLop = @malop
    GROUP BY hs.MaHS, hs.HoTen, lh.TenLop
    ORDER BY hs.HoTen';

    EXEC sp_executesql @sql, 
        N'@malop VARCHAR(10), @maMon VARCHAR(10), @loaiFilter NVARCHAR(10), @Thang1Loai NVARCHAR(20), @Thang2Loai NVARCHAR(20), @Thang3Loai NVARCHAR(20), @GiuaKiLoai NVARCHAR(20), @CuoiKiLoai NVARCHAR(20)', 
        @malop, @maMon, @loaiFilter, @Thang1Loai, @Thang2Loai, @Thang3Loai, @GiuaKiLoai, @CuoiKiLoai;
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

CREATE PROCEDURE sp_InsertKetQuaHocTap
    @MaHS VARCHAR(10),
    @MaMon VARCHAR(10),
    @Diem FLOAT,
    @NhanXet NVARCHAR(200)
AS
BEGIN
    INSERT INTO KetQuaHocTap(MaKQ, MaMon, MaHS, NgayNhap, Diem, NhanXet, Loai) 
    VALUES(LEFT(NEWID(), 8), @MaMon, @MaHS, GETDATE(), @Diem, @NhanXet, N'GiuaKi1');
END;
GO

Create PROCEDURE sp_UpsertKetQuaHocTap
    @MaHS VARCHAR(30),
    @MaMon VARCHAR(20),
    @Loai VARCHAR(30),
    @Diem FLOAT = NULL,
    @NhanXet NVARCHAR(200) = NULL,
    @GhiChu NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @KhoiCuaHS NVARCHAR(20);
    SELECT @KhoiCuaHS = lh.Khoi 
    FROM HocSinh hs 
    JOIN LopHoc lh ON hs.MaLop = lh.MaLop 
    WHERE hs.MaHS = @MaHS;

    IF @KhoiCuaHS IS NULL
    BEGIN
        RAISERROR (N'Lỗi: Không tìm thấy học sinh [%s] hoặc học sinh chưa được xếp lớp.', 16, 1, @MaHS);
        RETURN;
    END

    IF @Diem IS NOT NULL
    BEGIN
        DECLARE @DaKhoa BIT = 0;
        DECLARE @TenCotDiem NVARCHAR(100);
        DECLARE @HomNay DATE = CAST(GETDATE() AS DATE); 
        DECLARE @HocKyCuaLoaiDiem INT;

        BEGIN TRY
            SELECT 
                @TenCotDiem = ISNULL(TenHienThi, @Loai),
                @HocKyCuaLoaiDiem = HocKy,
                @DaKhoa = CASE 
                    WHEN KhoaThuCong = 1 THEN 1 
                    WHEN @HomNay > CAST(NgayKhoaDiem AS DATE) THEN 1 
                    WHEN @HomNay < CAST(NgayMoDiem AS DATE) THEN 1  
                    ELSE 0
                END
            FROM 
                ThoiHanDiem
            WHERE 
                MaCotDiem = @Loai
                AND Khoi = @KhoiCuaHS;

            IF @TenCotDiem IS NULL
            BEGIN
                RAISERROR (N'Lỗi: Cột điểm [%s] cho khối [%s] không được định nghĩa trong Bảng Thời Hạn Điểm. Vui lòng liên hệ Admin.', 
                           16, 1, @Loai, @KhoiCuaHS);
                RETURN;
            END

            IF @DaKhoa = 1
            BEGIN
                RAISERROR (N'Lỗi: Cột điểm [%s] (Khối %s) đã bị khóa hoặc chưa đến hạn nhập điểm. Không thể lưu.', 
                           16, 1, @TenCotDiem, @KhoiCuaHS);
                RETURN;
            END
        END TRY
        BEGIN CATCH
            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
            RAISERROR (@ErrorMessage, 16, 1);
            RETURN;
        END CATCH
    END

    BEGIN TRY
        IF EXISTS (SELECT 1 
                   FROM KetQuaHocTap 
                   WHERE MaHS = @MaHS 
                     AND MaMon = @MaMon 
                     AND Loai = @Loai)
        BEGIN
            UPDATE KetQuaHocTap
            SET 
                Diem = CASE WHEN @Diem IS NOT NULL THEN @Diem ELSE Diem END,
                NhanXet = CASE WHEN @NhanXet IS NOT NULL THEN @NhanXet ELSE NhanXet END,
                GhiChu = CASE WHEN @GhiChu IS NOT NULL THEN @GhiChu ELSE GhiChu END,
                NgayNhap = GETDATE()
            WHERE 
                MaHS = @MaHS 
                AND MaMon = @MaMon 
                AND Loai = @Loai;
        END
        ELSE
        BEGIN
            IF @Diem IS NOT NULL OR @NhanXet IS NOT NULL OR @GhiChu IS NOT NULL
            BEGIN
                INSERT INTO KetQuaHocTap (MaKQ, MaHS, MaMon, Loai, Diem, NhanXet, GhiChu, NgayNhap)
                VALUES (LEFT(NEWID(), 8), @MaHS, @MaMon, @Loai, @Diem, @NhanXet, @GhiChu, GETDATE());
            END
        END
    END TRY
    BEGIN CATCH
        DECLARE @UpsertErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (N'Lỗi khi lưu điểm: %s', 16, 1, @UpsertErrorMessage);
        RETURN;
    END CATCH
END
GO

-- 💰 QUỸ LỚP
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

-- ================================================================
-- MODULE 4: CHỨC NĂNG GIẢNG DẠY
-- ================================================================
PRINT 'TẠO STORED PROCEDURE CHO MODULE CHỨC NĂNG GIẢNG DẠY...';
GO

-- 📅 THỜI KHÓA BIỂU
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

CREATE PROCEDURE sp_DeleteTKBEntry
    @MaGV VARCHAR(10),
    @Ngay DATE,
    @Tiet INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM ThoiKhoaBieu
    WHERE MaGV = @MaGV
      AND Ngay = @Ngay
      AND Tiet = @Tiet;
END
GO

CREATE PROCEDURE sp_DeleteTKBByWeek
    @MaGV VARCHAR(10),
    @Monday DATE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Sunday DATE = DATEADD(day, 6, @Monday);
    DELETE FROM ThoiKhoaBieu
    WHERE MaGV = @MaGV
      AND Ngay >= @Monday
      AND Ngay <= @Sunday;
END
GO

create PROCEDURE [dbo].[sp_ImportTKBForGV]
    @MaGV VARCHAR(10),
    @NgayList ut_DateList READONLY,
    @TKBData ut_TKBImport READONLY
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DefaultColor VARCHAR(20) = '#4682B4';

    SELECT 
        t.Ngay, 
        t.Tiet, 
        m.MaMon, 
        l.MaLop,
        NULLIF(t.GhiChu, '') AS GhiChu,
        COALESCE(NULLIF(t.MauSac, ''), @DefaultColor) AS MauSac
    INTO #ProcessedTKB
    FROM @TKBData t
    LEFT JOIN MonHoc m ON t.TenMon = m.TenMon
    LEFT JOIN LopHoc l ON t.TenLop = l.TenLop
    WHERE CAST(t.Ngay AS DATE) IN (SELECT Ngay FROM @NgayList);

    DECLARE @Failed INT = 0;
    DECLARE @Success INT = 0;

    SELECT @Failed = COUNT(*) FROM #ProcessedTKB WHERE MaMon IS NULL OR MaLop IS NULL;
    SELECT @Success = COUNT(*) FROM #ProcessedTKB WHERE MaMon IS NOT NULL AND MaLop IS NOT NULL;

    IF @Success > 0
    BEGIN
        BEGIN TRANSACTION;
        BEGIN TRY
            DELETE TKB
            FROM ThoiKhoaBieu TKB
            INNER JOIN @NgayList DL ON CAST(TKB.Ngay AS DATE) = DL.Ngay
            WHERE TKB.MaGV = @MaGV;

            INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, MaMon, MaLop, GhiChu, MauSac, MaGV)
            SELECT
                LEFT(NEWID(), 10),
                p.Ngay, p.Tiet, p.MaMon, p.MaLop,
                p.GhiChu,
                p.MauSac,
                @MaGV
            FROM #ProcessedTKB p
            WHERE p.MaMon IS NOT NULL AND p.MaLop IS NOT NULL;
            
            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            RAISERROR(N'Lỗi import TKB. Đã hoàn tác.', 16, 1);
            SELECT 0 AS [Success], (ISNULL(@Success, 0) + ISNULL(@Failed, 0)) AS [Failed];
            IF OBJECT_ID('tempdb..#ProcessedTKB') IS NOT NULL DROP TABLE #ProcessedTKB;
            RETURN;
        END CATCH
    END
    
    SELECT @Success AS [Success], @Failed AS [Failed];
    IF OBJECT_ID('tempdb..#ProcessedTKB') IS NOT NULL DROP TABLE #ProcessedTKB;
END;
GO

-- 📚 TÀI LIỆU
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

CREATE PROCEDURE sp_UnshareTaiLieu
    @MaTL VARCHAR(10)
AS
BEGIN
    UPDATE TaiLieu SET TrangThaiChiaSe = N'Riêng tư' WHERE MaTL = @MaTL;
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

-- 2. TẠO SP LẤY DANH SÁCH GIÁO VIÊN (ĐỂ CHIA SẺ)
-- (SP này không cần tạo lại nếu đã tạo rồi)
IF OBJECT_ID('sp_GetAllTeachersForSharing', 'P') IS NULL
BEGIN
    EXEC('
    CREATE PROCEDURE sp_GetAllTeachersForSharing
        @MaGvOwner VARCHAR(10)
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT MaGV, Ten 
        FROM GiaoVien 
        WHERE MaGV != @MaGvOwner 
          AND TrangThai = N''Đã xác nhận'' 
        ORDER BY Ten;
    END;
    ')
    PRINT N'Đã tạo SP sp_GetAllTeachersForSharing.';
END
GO

-- 3. TẠO SP LẤY TRẠNG THÁI CHIA SẺ HIỆN TẠI
-- (SP này không cần tạo lại nếu đã tạo rồi)
IF OBJECT_ID('sp_GetDocumentStatus', 'P') IS NULL
BEGIN
    EXEC('
    CREATE PROCEDURE sp_GetDocumentStatus
        @MaTL VARCHAR(10)
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT TrangThaiChiaSe FROM TaiLieu WHERE MaTL = @MaTL;
    END;
    ')
    PRINT N'Đã tạo SP sp_GetDocumentStatus.';
END
GO

-- 4. TẠO SP LẤY DANH SÁCH GIÁO VIÊN ĐÃ ĐƯỢC CHIA SẺ
-- (SP này không cần tạo lại nếu đã tạo rồi)
IF OBJECT_ID('sp_GetSharedWithTeachers', 'P') IS NULL
BEGIN
    EXEC('
    CREATE PROCEDURE sp_GetSharedWithTeachers
        @MaTL VARCHAR(10)
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT MaGV FROM TaiLieu_ChiaSe_GiaoVien WHERE MaTL = @MaTL;
    END;
    ')
    PRINT N'Đã tạo SP sp_GetSharedWithTeachers.';
END
GO

-- 5. TẠO SP CẬP NHẬT TỔNG QUÁT (SP NÀY LÀM HẾT MỌI VIỆC)
-- (SP này không cần tạo lại nếu đã tạo rồi)
IF OBJECT_ID('sp_UpdateDocumentSharing', 'P') IS NULL
BEGIN
    EXEC('
    CREATE PROCEDURE sp_UpdateDocumentSharing
        @MaTL VARCHAR(10),
        @TrangThai NVARCHAR(20),
        @GiaoVienList ut_MaHSList READONLY -- Tái sử dụng Type có sẵn
    AS
    BEGIN
        SET NOCOUNT ON;
        BEGIN TRANSACTION;
        BEGIN TRY
            UPDATE TaiLieu SET TrangThaiChiaSe = @TrangThai WHERE MaTL = @MaTL;
            DELETE FROM TaiLieu_ChiaSe_GiaoVien WHERE MaTL = @MaTL;
            
            IF @TrangThai = N''Giáo viên cụ thể''
            BEGIN
                INSERT INTO TaiLieu_ChiaSe_GiaoVien (MaTL, MaGV)
                SELECT @MaTL, MaHS FROM @GiaoVienList;
            END

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            ;THROW;
        END CATCH
    END;
    ')
    PRINT N'Đã tạo SP sp_UpdateDocumentSharing.';
END
GO

-- 6. SỬA SP "LẤY TÀI LIỆU ĐƯỢC CHIA SẺ"
-- Sửa SP cũ (Nếu SP 'sp_GetTaiLieuSharedWithUploader' không tồn tại thì tạo mới)
IF OBJECT_ID('sp_GetTaiLieuSharedWithUploader', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetTaiLieuSharedWithUploader;
GO

-- Đổi tên SP này cho rõ nghĩa hơn
IF OBJECT_ID('sp_GetSharedDocumentsForTeacher', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetSharedDocumentsForTeacher;
GO

CREATE PROCEDURE sp_GetSharedDocumentsForTeacher
    @MaGV_HienTai VARCHAR(10) -- SP này cần biết AI ĐANG XEM
AS
BEGIN
    SET NOCOUNT ON;

    -- Lấy tài liệu chia sẻ công khai (TrangThai = 'Chia sẻ')
    SELECT 
        tl.MaTL, tl.TenTL, tl.MoTa, tl.Kieu, tl.NgayTaiLen, 
        gv.Ten AS TenGV,
        N'Công khai' AS LoaiChiaSe
    FROM TaiLieu tl
    INNER JOIN GiaoVien gv ON tl.MaGV = gv.MaGV
    WHERE tl.TrangThaiChiaSe = N'Chia sẻ'
      AND tl.MaGV != @MaGV_HienTai

    UNION

    -- Lấy tài liệu được chia sẻ CỤ THỂ cho mình
    SELECT 
        tl.MaTL, tl.TenTL, tl.MoTa, tl.Kieu, tl.NgayTaiLen, 
        gv.Ten AS TenGV,
        N'Chia sẻ riêng' AS LoaiChiaSe
    FROM TaiLieu_ChiaSe_GiaoVien tsgv
    INNER JOIN TaiLieu tl ON tsgv.MaTL = tl.MaTL
    INNER JOIN GiaoVien gv ON tl.MaGV = gv.MaGV
    WHERE tsgv.MaGV = @MaGV_HienTai;
END;
GO
-- 🎮 MINI-GAME
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

-- ✍️ GHI CHÚ NHANH
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
    
    DECLARE @Loai NVARCHAR(20);
    SELECT @Loai = thd.MaCotDiem
    FROM ThoiHanDiem thd
    JOIN HocSinh hs ON thd.Khoi = (SELECT Khoi FROM LopHoc WHERE MaLop = hs.MaLop)
    WHERE hs.MaHS = @MaHS AND thd.MaCotDiem LIKE 'CuoiKi2%';

    IF @Loai IS NULL SET @Loai = 'CuoiKi2';

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

-- ================================================================
-- MODULE 5: BÁO CÁO & PHÂN TÍCH
-- ================================================================
PRINT 'TẠO STORED PROCEDURE CHO MODULE BÁO CÁO & PHÂN TÍCH...';
GO

-- 📊 BÁO CÁO (GIÁO VIÊN)
CREATE PROCEDURE sp_GetLopByGiaoVien
    @maGV VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
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

CREATE PROCEDURE sp_GetMonHocByGiaoVienAndLop
    @maGV VARCHAR(10),
    @maLop VARCHAR(10)
AS
BEGIN
    SELECT DISTINCT m.MaMon, m.TenMon 
    FROM PhanCongGiangDay pc
    JOIN MonHoc m ON pc.MaMon = m.MaMon
    WHERE pc.MaGV = @maGV AND pc.MaLop = @maLop
    ORDER BY m.TenMon;
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

    DECLARE @khoi NVARCHAR(20);
    SELECT @khoi = Khoi FROM LopHoc WHERE MaLop = @maLop;

    SELECT @monHocCols = STUFF((SELECT DISTINCT ',' + QUOTENAME(mh.TenMon) 
                                FROM PhanCongGiangDay pcg
                                JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
                                WHERE pcg.MaLop = @maLop
                                ORDER BY 1 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'');
    
    SELECT @monHocColsSelect = STUFF((SELECT DISTINCT ',ROUND(ISNULL(' + QUOTENAME(mh.TenMon) + ', 0), 2) AS ' + QUOTENAME(mh.TenMon)
                                    FROM PhanCongGiangDay pcg
                                    JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
                                    WHERE pcg.MaLop = @maLop
                                    ORDER BY 1 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'');
    
    SELECT @tongMon = STUFF((SELECT DISTINCT ' + ISNULL(' + QUOTENAME(mh.TenMon) + ', 0)'
                            FROM PhanCongGiangDay pcg
                            JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
                            WHERE pcg.MaLop = @maLop
                            ORDER BY 1 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,3,'');

    SELECT @monCount = COUNT(DISTINCT MaMon) FROM PhanCongGiangDay WHERE MaLop = @maLop;
    
    IF @monCount = 0 OR @monHocCols IS NULL
    BEGIN
        SELECT MaHS, HoTen FROM HocSinh WHERE MaLop = @maLop;
        RETURN;
    END;
    
    SET @loaiFilter = CASE WHEN @hocKy = 3 THEN N'%' ELSE CAST(@hocKy AS NVARCHAR) END;

    SET @sql = N'
    ;WITH DiemTB AS (
        SELECT 
            hs.MaHS,
            hs.HoTen,
            lh.TenLop,
            mh.TenMon,
            AVG(kq.Diem) AS DiemTB
        FROM HocSinh hs
        INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop
        INNER JOIN PhanCongGiangDay pcg ON hs.MaLop = pcg.MaLop
        INNER JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
        LEFT JOIN KetQuaHocTap kq 
            ON hs.MaHS = kq.MaHS 
            AND mh.MaMon = kq.MaMon 
            AND kq.Loai IN (
                SELECT MaCotDiem 
                FROM ThoiHanDiem 
                WHERE Khoi = @khoi AND CAST(HocKy AS NVARCHAR) LIKE @loaiFilter
            )
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

    EXEC sp_executesql @sql, N'@maLop VARCHAR(10), @khoi NVARCHAR(20), @loaiFilter NVARCHAR(10)', @maLop, @khoi, @loaiFilter;
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

Create PROCEDURE sp_GetBaoCaoChuyenCan
    @maLop VARCHAR(10),
    @hocKy INT
AS
BEGIN
    DECLARE @CurrentDate DATE = GETDATE();
    DECLARE @CurrentMonth INT = MONTH(@CurrentDate);
    DECLARE @CurrentYear INT = YEAR(@CurrentDate);
    
    DECLARE @NamHocStartYear INT;
    
    IF @CurrentMonth >= 8 
        SET @NamHocStartYear = @CurrentYear;
    ELSE 
        SET @NamHocStartYear = @CurrentYear - 1;

    DECLARE @StartDate DATE, @EndDate DATE;

    IF @hocKy = 1 
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHocStartYear, 8, 1);
        SET @EndDate = DATEFROMPARTS(@NamHocStartYear, 12, 31);
    END
    ELSE IF @hocKy = 2 
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHocStartYear + 1, 1, 1);
        SET @EndDate = DATEFROMPARTS(@NamHocStartYear + 1, 5, 31);
    END
    ELSE
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHocStartYear, 8, 1);
        SET @EndDate = DATEFROMPARTS(@NamHocStartYear + 1, 5, 31);
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

CREATE PROCEDURE sp_GetMonthlyScoreTypes
    @MaLop VARCHAR(10),
    @HocKy INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Khoi NVARCHAR(20);
    SELECT @Khoi = Khoi FROM LopHoc WHERE MaLop = @MaLop;

    IF @Khoi IS NULL
    BEGIN
        SELECT TOP 0 '' AS MaCotDiem, '' AS TenHienThi;
        RETURN;
    END

    SELECT MaCotDiem, TenHienThi 
    FROM ThoiHanDiem
    WHERE Khoi = @Khoi
      AND HocKy = @HocKy
      AND MaCotDiem LIKE 'Thang%'
    ORDER BY NgayMoDiem;
END;
GO

CREATE PROCEDURE sp_GetBaoCaoThang_ThongKe
    @MaLop VARCHAR(10),
    @MaMon VARCHAR(10),
    @LoaiDiem VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH RawData AS (
        SELECT 
            hs.GioiTinh,
            hs.DanToc,
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
                ELSE '<5'
            END AS NhomDiem,
            CASE 
                WHEN Diem >= 7 THEN 'T'
                WHEN Diem >= 5 THEN 'H'
                ELSE 'C'
            END AS XepLoai,
            CASE WHEN GioiTinh = N'Nữ' THEN 1 ELSE 0 END AS IsNu,
            CASE WHEN DanToc IS NOT NULL AND DanToc != N'Kinh' THEN 1 ELSE 0 END AS IsDanTocThieuSo,
            CASE WHEN GioiTinh = N'Nữ' AND (DanToc IS NOT NULL AND DanToc != N'Kinh') THEN 1 ELSE 0 END AS IsNuDanTocThieuSo
        FROM RawData
    )
    SELECT 
        'Diem' AS LoaiThongKe,
        NhomDiem AS PhanLoai,
        COUNT(*) AS TS,
        SUM(IsNu) AS Nu,
        SUM(IsDanTocThieuSo) AS DanToc,
        SUM(IsNuDanTocThieuSo) AS NDT
    FROM ClassifiedData
    GROUP BY NhomDiem

    UNION ALL

    SELECT 
        'XepLoai' AS LoaiThongKe,
        XepLoai AS PhanLoai,
        COUNT(*) AS TS,
        SUM(IsNu) AS Nu,
        SUM(IsDanTocThieuSo) AS DanToc,
        SUM(IsNuDanTocThieuSo) AS NDT
    FROM ClassifiedData
    GROUP BY XepLoai;
END;
GO

-- 🤖 PHÂN TÍCH AI
CREATE PROCEDURE sp_GetScoresForAnalysis
    @maGV VARCHAR(10),
    @phamVi NVARCHAR(20),
    @chiTiet NVARCHAR(50),
    @maMon VARCHAR(10),
    @hocKy INT
AS
BEGIN
    DECLARE @kyFilter NVARCHAR(10) = N'%' + CAST(@hocKy AS VARCHAR);
    DECLARE @sql NVARCHAR(MAX);

    SET @sql = N'
    SELECT 
        hs.MaHS, hs.HoTen, lh.MaLop, lh.TenLop,
        mh.MaMon, mh.TenMon, kq.Loai, kq.Diem
    FROM KetQuaHocTap kq
    JOIN HocSinh hs ON kq.MaHS = hs.MaHS
    JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    JOIN MonHoc mh ON kq.MaMon = mh.MaMon
    JOIN ThoiHanDiem thd ON kq.Loai = thd.MaCotDiem AND lh.Khoi = thd.Khoi
    WHERE kq.Diem IS NOT NULL
    AND thd.HocKy = @hocKy';

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
        N'@hocKy INT, @chiTiet NVARCHAR(50), @maMon VARCHAR(10)', 
        @hocKy, @chiTiet, @maMon;
END;
GO

create PROCEDURE sp_GetStudentDataForPrediction
    @maLop VARCHAR(10),
    @maMon VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @g1Type VARCHAR(20) = 'GiuaKi1';
    DECLARE @g2Type VARCHAR(20) = 'CuoiKi1';

    ;WITH ScoresG1 AS (
        SELECT MaHS, Diem 
        FROM KetQuaHocTap 
        WHERE Loai = @g1Type AND MaMon = @maMon
    ),
    ScoresG2 AS (
        SELECT MaHS, Diem 
        FROM KetQuaHocTap 
        WHERE Loai = @g2Type AND MaMon = @maMon
    ),
    LowScores AS (
        SELECT 
            MaHS,
            SUM(CASE WHEN Loai = @g1Type AND ISNULL(Diem, 0) < 5 THEN 1 ELSE 0 END) +
            SUM(CASE WHEN Loai = @g2Type AND ISNULL(Diem, 0) < 5 THEN 1 ELSE 0 END)
            AS NumLowScores
        FROM KetQuaHocTap
        WHERE Loai IN (@g1Type, @g2Type) AND MaMon = @maMon
        GROUP BY MaHS
    ),
    Absences AS (
        SELECT 
            MaHS, 
            COUNT(*) as TotalAbsences
        FROM DiemDanh
        WHERE TrangThai = N'Vắng'
        GROUP BY MaHS
    )
    SELECT 
        hs.MaHS,
        hs.HoTen,
        lh.TenLop,
        ISNULL(g1.Diem, 0) AS G1,
        ISNULL(g2.Diem, 0) AS G2,
        ISNULL(ls.NumLowScores, 0) AS NumLowScores,
        ISNULL(ab.TotalAbsences, 0) AS Absences
    FROM HocSinh hs
    INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    LEFT JOIN ScoresG1 g1 ON hs.MaHS = g1.MaHS
    LEFT JOIN ScoresG2 g2 ON hs.MaHS = g2.MaHS
    LEFT JOIN LowScores ls ON hs.MaHS = ls.MaHS
    LEFT JOIN Absences ab ON hs.MaHS = ab.MaHS
    WHERE hs.MaLop = @maLop;
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

-- ================================================================
-- MODULE 6: QUẢN TRỊ (ADMIN)
-- ================================================================
PRINT 'TẠO STORED PROCEDURE CHO MODULE QUẢN TRỊ...';
GO

-- 🧑‍💼 QUẢN LÝ GIÁO VIÊN
CREATE PROCEDURE sp_GetAllGiaoVien
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        gv.MaGV, 
        gv.Ten, 
        gv.Username, 
        gv.Email, 
        gv.SDT, 
        gv.TrangThai,
        ISNULL(STUFF((
            SELECT N', ' + mh.TenMon
            FROM GiaoVien_MonHoc gvm
            JOIN MonHoc mh ON gvm.MaMon = mh.MaMon
            WHERE gvm.MaGV = gv.MaGV
            ORDER BY mh.TenMon
            FOR XML PATH('')
        ), 1, 2, N''), N'Chưa có môn') AS CacMonDay
    FROM GiaoVien gv;
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

create PROCEDURE sp_UpdateTrangThaiGiaoVien
    @id VARCHAR(10),
    @tt NVARCHAR(20),
    @Email NVARCHAR(50) OUTPUT,
    @Ten NVARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE GiaoVien 
    SET TrangThai=@tt 
    WHERE MaGV=@id;
    
    SELECT @Email = Email, @Ten = Ten
    FROM GiaoVien
    WHERE MaGV = @id;
END;
GO

create PROCEDURE sp_UpdateGiaoVien
    @id VARCHAR(10),
    @t NVARCHAR(100),
    @e NVARCHAR(50),
    @s VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;

    -- [THÊM MỚI] Kiểm tra email trùng lặp với một giáo viên KHÁC
    IF EXISTS (SELECT 1 FROM GiaoVien WHERE Email = @e AND MaGV != @id)
    BEGIN
        RAISERROR(N'Email này đã được sử dụng bởi một giáo viên khác. Vui lòng chọn email khác.', 16, 1);
        RETURN;
    END

    -- Giữ lại logic cập nhật cũ
    UPDATE GiaoVien 
    SET Ten=@t, Email=@e, SDT=@s 
    WHERE MaGV=@id;
END;
GO

create PROCEDURE sp_DeleteGiaoVien
    @id VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Conf_PhanCongGiangDay NVARCHAR(MAX);
    DECLARE @Conf_GVCN NVARCHAR(MAX);
    DECLARE @ErrorMessage NVARCHAR(MAX);

    -- 1. Kiểm tra bảng Phân Công Giảng Dạy
    -- (Tập hợp tất cả các môn/lớp mà giáo viên đang dạy)
    SELECT @Conf_PhanCongGiangDay = STUFF(
        (SELECT N', ' + mh.TenMon + N' (' + lh.TenLop + N')'
         FROM PhanCongGiangDay pcg
         JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
         JOIN LopHoc lh ON pcg.MaLop = lh.MaLop
         WHERE pcg.MaGV = @id
         FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, N'');

    -- 2. Kiểm tra bảng Lớp Học (xem có làm GVCN không)
    SELECT @Conf_GVCN = STUFF(
        (SELECT N',   ' + lh.TenLop
         FROM LopHoc lh
         WHERE lh.MaGVCN = @id
         FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, N'');

    -- 3. Xây dựng thông báo lỗi nếu có xung đột
    SET @ErrorMessage = N'';
    IF @Conf_PhanCongGiangDay IS NOT NULL
    BEGIN
        SET @ErrorMessage = @ErrorMessage + N' - Đang giảng dạy: ' + @Conf_PhanCongGiangDay + N'.';
    END
    IF @Conf_GVCN IS NOT NULL
    BEGIN
        SET @ErrorMessage = @ErrorMessage + N' - Đang chủ nhiệm: ' + @Conf_GVCN + N'.';
    END

    -- 4. Nếu có lỗi (ErrorMessage không rỗng), thì báo lỗi. Ngược lại, tiến hành xóa.
    IF @ErrorMessage != N''
    BEGIN
        SET @ErrorMessage = N'Không thể xóa giáo viên. Giáo viên này hiện đang có các phân công sau:' + @ErrorMessage + N' Vui lòng gỡ các phân công này trước khi xóa.';
        RAISERROR(@ErrorMessage, 16, 1); -- Mã lỗi 16, mức độ 1
        RETURN;
    END
    ELSE
    BEGIN
        -- Không có xung đột, tiến hành xóa
        -- (Bảng GiaoVien_MonHoc và PhanCongGiangDay sẽ tự động xóa theo ON DELETE CASCADE
        -- mà bạn đã định nghĩa trong data.sql, nhưng LopHoc (GVCN) thì không,
        -- nên logic kiểm tra ở trên là rất quan trọng)
        DELETE FROM GiaoVien WHERE MaGV = @id;
    END
END;
GO

CREATE PROCEDURE sp_GetMonHocByGiaoVien
    @MaGV VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaMon FROM GiaoVien_MonHoc WHERE MaGV = @MaGV;
END;
GO

CREATE PROCEDURE sp_UpdateGiaoVien_MonHoc
    @MaGV VARCHAR(10),
    @MonHocList ut_MaMonList READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DELETE FROM GiaoVien_MonHoc WHERE MaGV = @MaGV;
        INSERT INTO GiaoVien_MonHoc (MaGV, MaMon)
        SELECT @MaGV, MaMon FROM @MonHocList
        WHERE MaMon IS NOT NULL AND MaMon != '';
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        ;THROW; 
    END CATCH
END;
GO

CREATE PROCEDURE sp_UpdateTeacherSubjects
    @MaGV VARCHAR(10),
    @MonHocList ut_MaMonList READONLY
AS
BEGIN
    DELETE FROM GiaoVien_MonHoc WHERE MaGV = @MaGV;
    INSERT INTO GiaoVien_MonHoc (MaGV, MaMon)
    SELECT @MaGV, MaMon FROM @MonHocList;
END;
GO

-- 🏫 QUẢN LÝ LỚP & HỌC SINH

-- Kiểm tra lớp học tồn tại
CREATE PROCEDURE sp_CheckClassExists
    @MaLop NVARCHAR(10)
AS
BEGIN
    SELECT COUNT(1) FROM LopHoc WHERE MaLop = @MaLop
END
GO

-- Kiểm tra lớp có học sinh không
CREATE PROCEDURE sp_CheckClassHasStudents
    @MaLop NVARCHAR(10)
AS
BEGIN
    SELECT COUNT(1) FROM HocSinh WHERE MaLop = @MaLop
END
GO

-- Lấy danh sách khối có sẵn
CREATE PROCEDURE sp_GetAvailableGrades
AS
BEGIN
    SELECT DISTINCT Khoi FROM LopHoc ORDER BY Khoi
END
GO
-- Stored Procedure thêm lớp học mới
create PROCEDURE sp_InsertLopHoc
    @MaLop NVARCHAR(10),
    @TenLop NVARCHAR(50),
    @Khoi NVARCHAR(20),
    @NamHoc NVARCHAR(10)
AS
BEGIN
    BEGIN TRY
        -- Kiểm tra mã lớp đã tồn tại chưa
        IF EXISTS (SELECT 1 FROM LopHoc WHERE MaLop = @MaLop)
        BEGIN
            RAISERROR('Mã lớp đã tồn tại!', 16, 1)
            RETURN
        END

        INSERT INTO LopHoc (MaLop, TenLop, Khoi, NamHoc)
        VALUES (@MaLop, @TenLop, @Khoi, @NamHoc)
        
    END TRY
    BEGIN CATCH
        THROW
    END CATCH
END
GO

-- Stored Procedure xóa lớp học
create PROCEDURE sp_DeleteLopHoc
    @MaLop NVARCHAR(10)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION

        -- Kiểm tra lớp có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM LopHoc WHERE MaLop = @MaLop)
        BEGIN
            RAISERROR('Lớp không tồn tại!', 16, 1)
            RETURN
        END

        -- Kiểm tra lớp có học sinh không
        IF EXISTS (SELECT 1 FROM HocSinh WHERE MaLop = @MaLop)
        BEGIN
            RAISERROR('Không thể xóa lớp vì lớp đang có học sinh!', 16, 1)
            RETURN
        END

        -- Xóa phân công giảng dạy liên quan
        DELETE FROM PhanCongGiangDay WHERE MaLop = @MaLop
        
        -- Xóa lớp học
        DELETE FROM LopHoc WHERE MaLop = @MaLop

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW
    END CATCH
END
GO

CREATE PROCEDURE sp_GetAllHocSinh
AS
BEGIN
    SELECT MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc 
    FROM HocSinh 
    ORDER BY MaLop, HoTen;
END;
GO

CREATE PROCEDURE sp_GetHocSinhByLop
    @malop VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ROW_NUMBER() OVER (ORDER BY HoTen) AS STT, 
        MaHS, 
        HoTen, 
        GioiTinh, 
        NgaySinh, 
        DiaChi, 
        DanToc, 
        SDTPhuHuynh 
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

CREATE PROCEDURE sp_DeleteHocSinh
    @MaHS VARCHAR(10)
AS
BEGIN
    DELETE FROM HocSinh WHERE MaHS=@MaHS;
END;
GO

CREATE PROCEDURE sp_UpdateHocSinhLop
    @MaHS VARCHAR(10),
    @MaLopMoi VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE HocSinh
    SET MaLop = @MaLopMoi
    WHERE MaHS = @MaHS;
END;
GO

CREATE PROCEDURE sp_UpdateHocSinhLop_Multi
    @MaHSList ut_MaHSList READONLY,
    @MaLopMoi VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE HocSinh
    SET MaLop = @MaLopMoi
    WHERE MaHS IN (SELECT MaHS FROM @MaHSList);
    SELECT @@ROWCOUNT AS SoHocSinhDaChuyen;
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

CREATE PROCEDURE sp_GetAllLopHoc
AS
BEGIN
    SELECT MaLop, TenLop, Khoi FROM LopHoc ORDER BY Khoi, TenLop;
END;
GO

CREATE PROCEDURE sp_GetLopHocDetails
    @MaLop VARCHAR(10)
AS
BEGIN
    SELECT 
        l.MaLop, l.TenLop, l.Khoi, l.NamHoc, 
        ISNULL(gv.Ten, N'Chưa có') AS TenGVCN,
        l.MaGVCN,
        (SELECT COUNT(*) FROM HocSinh WHERE MaLop = l.MaLop) AS SiSo
    FROM LopHoc l
    LEFT JOIN GiaoVien gv ON l.MaGVCN = gv.MaGV
    WHERE l.MaLop = @MaLop;
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

CREATE PROCEDURE sp_GetPhanCongGiangDayByLop
    @MaLop VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        m.MaMon, 
        m.TenMon,
        ISNULL(pc.MaGV, '') AS MaGV, 
        ISNULL(gv.Ten, 'Chưa phân công') AS TenGV
    FROM MonHoc m
    LEFT JOIN PhanCongGiangDay pc ON m.MaMon = pc.MaMon AND pc.MaLop = @MaLop
    LEFT JOIN GiaoVien gv ON pc.MaGV = gv.MaGV
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
        
        DELETE FROM PhanCongGiangDay 
        WHERE MaLop = @MaLop AND MaMon = @MaMon;

        IF @NewMaGV IS NOT NULL AND @NewMaGV != ''
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM GiaoVien_MonHoc WHERE MaGV = @NewMaGV AND MaMon = @MaMon)
            BEGIN
                DECLARE @TenGV NVARCHAR(100), @TenMon NVARCHAR(100);
                SELECT @TenGV = Ten FROM GiaoVien WHERE MaGV = @NewMaGV;
                SELECT @TenMon = TenMon FROM MonHoc WHERE MaMon = @MaMon;
                
                RAISERROR(N'Không thể phân công. Giáo viên [%s] không dạy Môn [%s].', 16, 1, @TenGV, @TenMon);
                ROLLBACK TRANSACTION;
                RETURN;
            END

            INSERT INTO PhanCongGiangDay (MaGV, MaLop, MaMon) 
            VALUES (@NewMaGV, @MaLop, @MaMon);
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

-- 🏛️ QUẢN LÝ TRƯỜNG HỌC
CREATE PROCEDURE sp_GetAllMonHoc
AS
BEGIN
    SELECT MaMon, TenMon FROM MonHoc ORDER BY TenMon;
END;
GO

create PROCEDURE sp_InsertMonHoc
    @TenMon NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CleanedTenMon VARCHAR(100); -- Dùng VARCHAR để loại bỏ dấu
    DECLARE @NewMaMon VARCHAR(10);

    -- BƯỚC 1: [SỬA LỖI] Chuyển NVARCHAR (tiếng Việt có dấu) sang VARCHAR (không dấu)
    -- Bằng cách sử dụng Collation 'Latin1_General_CI_AS' để loại bỏ dấu
    SET @CleanedTenMon = @TenMon COLLATE Latin1_General_CI_AS;

    -- BƯỚC 2: Loại bỏ các ký tự đặc biệt (giữ lại logic cũ của bạn)
    SET @CleanedTenMon = REPLACE(@CleanedTenMon, ' ', '');
    SET @CleanedTenMon = REPLACE(@CleanedTenMon, '(', '');
    SET @CleanedTenMon = REPLACE(@CleanedTenMon, ')', '');
    SET @CleanedTenMon = REPLACE(@CleanedTenMon, '-', '');
    SET @CleanedTenMon = REPLACE(@CleanedTenMon, '/', '');
    -- Bạn có thể thêm các lệnh REPLACE khác ở đây nếu cần

    -- BƯỚC 3: Lấy 10 ký tự đầu và viết hoa
    SET @NewMaMon = UPPER(SUBSTRING(@CleanedTenMon, 1, 10));

    -- BƯỚC 4: Kiểm tra tồn tại (giữ nguyên logic cũ)
    IF EXISTS (SELECT 1 FROM MonHoc WHERE MaMon = @NewMaMon OR TenMon = @TenMon)
    BEGIN
        RAISERROR(N'Mã môn hoặc Tên môn này đã tồn tại.', 16, 1);
        RETURN;
    END

    -- BƯỚC 5: Thêm mới
    INSERT INTO MonHoc (MaMon, TenMon) VALUES (@NewMaMon, @TenMon);
END;
GO

CREATE PROCEDURE sp_UpdateMonHoc
    @MaMon VARCHAR(10),
    @TenMon NVARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM MonHoc WHERE TenMon = @TenMon AND MaMon != @MaMon)
    BEGIN
        RAISERROR(N'Tên môn này đã tồn tại ở một mã môn khác.', 16, 1);
        RETURN;
    END
    UPDATE MonHoc SET TenMon = @TenMon WHERE MaMon = @MaMon;
END;
GO

CREATE PROCEDURE sp_DeleteMonHoc
    @MaMon VARCHAR(10)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM GiaoVien_MonHoc WHERE MaMon = @MaMon) OR
       EXISTS (SELECT 1 FROM PhanCongGiangDay WHERE MaMon = @MaMon) OR
       EXISTS (SELECT 1 FROM KetQuaHocTap WHERE MaMon = @MaMon) OR
       EXISTS (SELECT 1 FROM ThoiKhoaBieu WHERE MaMon = @MaMon)
    BEGIN
        RAISERROR(N'Không thể xóa. Môn học này đang được sử dụng trong bảng phân công, kết quả học tập hoặc TKB.', 16, 1);
        RETURN;
    END
    DELETE FROM MonHoc WHERE MaMon = @MaMon;
END;
GO

CREATE PROCEDURE sp_GetThoiHanDiem
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        MaCotDiem, 
        TenHienThi, 
        Khoi,
        HocKy,
        NgayMoDiem, 
        NgayKhoaDiem, 
        KhoaThuCong,
        CASE 
            WHEN (GETDATE() NOT BETWEEN NgayMoDiem AND NgayKhoaDiem) OR (KhoaThuCong = 1) THEN 1 
            ELSE 0 
        END AS DaKhoa
    FROM ThoiHanDiem
    ORDER BY Khoi, HocKy, NgayMoDiem;
END;
GO

CREATE PROCEDURE sp_UpdateThoiHanDiem
    @MaCotDiem VARCHAR(20),
    @Khoi NVARCHAR(20),
    @HocKy INT,
    @NgayMoDiem DATE,
    @NgayKhoaDiem DATE,
    @KhoaThuCong BIT
AS
BEGIN
    UPDATE ThoiHanDiem
    SET 
        NgayMoDiem = @NgayMoDiem,
        NgayKhoaDiem = @NgayKhoaDiem,
        KhoaThuCong = @KhoaThuCong
    WHERE MaCotDiem = @MaCotDiem
      AND Khoi = @Khoi
      AND HocKy = @HocKy;
END;
GO

CREATE PROCEDURE sp_ProcessStudentPromotion
    @MaLopCu VARCHAR(10),
    @MaLopMoi_LenLop VARCHAR(10),
    @MaLopMoi_OLaiLop VARCHAR(10),
    @IsLop5_TotNghiep BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @KhoiCu NVARCHAR(20);
    SELECT @KhoiCu = Khoi FROM LopHoc WHERE MaLop = @MaLopCu;
    
    CREATE TABLE #TempDiemTB (
        MaHS VARCHAR(10) PRIMARY KEY,
        DTB_CaNam FLOAT
    );

    DECLARE @monHocCols NVARCHAR(MAX), 
            @tongMon NVARCHAR(MAX),
            @sql NVARCHAR(MAX);
    DECLARE @monCount INT;

    SELECT @monHocCols = STUFF((SELECT DISTINCT ',' + QUOTENAME(mh.TenMon) 
                                FROM PhanCongGiangDay pcg
                                JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
                                WHERE pcg.MaLop = @MaLopCu
                                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'');
    SELECT @tongMon = STUFF((SELECT DISTINCT ' + ISNULL(' + QUOTENAME(mh.TenMon) + ', 0)'
                            FROM PhanCongGiangDay pcg
                            JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
                            WHERE pcg.MaLop = @MaLopCu
                            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,3,'');
    SELECT @monCount = COUNT(DISTINCT MaMon) FROM PhanCongGiangDay WHERE MaLop = @MaLopCu;

    IF @monCount = 0 
    BEGIN
        PRINT N'Lớp cũ không có môn học nào được phân công. Không thể tính điểm.';
        SELECT 0 AS SoHSLenLop, 0 AS SoHSOLaiLop, 0 AS SoHSTotNghiep;
        IF OBJECT_ID('tempdb..#TempDiemTB') IS NOT NULL DROP TABLE #TempDiemTB;
        RETURN;
    END; 

    SET @sql = N'
    ;WITH DiemTB AS (
        SELECT 
            hs.MaHS,
            mh.TenMon,
            AVG(kq.Diem) AS DiemTB
        FROM HocSinh hs
        INNER JOIN PhanCongGiangDay pcg ON hs.MaLop = pcg.MaLop
        INNER JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
        LEFT JOIN KetQuaHocTap kq 
            ON hs.MaHS = kq.MaHS 
            AND mh.MaMon = kq.MaMon 
            AND kq.Loai IN (
                SELECT MaCotDiem 
                FROM ThoiHanDiem 
                WHERE Khoi = @KhoiCu
            )
        WHERE hs.MaLop = @MaLopCu
        GROUP BY hs.MaHS, mh.TenMon
    ),
    PivotData AS (
        SELECT MaHS, ' + @monHocCols + N'
        FROM DiemTB
        PIVOT (AVG(DiemTB) FOR TenMon IN (' + @monHocCols + N')) AS PivotTable
    )
    INSERT INTO #TempDiemTB (MaHS, DTB_CaNam)
    SELECT MaHS, 
           ROUND((' + @tongMon + N') / NULLIF(' + CAST(@monCount AS VARCHAR) + N', 0), 2)
    FROM PivotData;';

    EXEC sp_executesql @sql, N'@MaLopCu VARCHAR(10), @KhoiCu NVARCHAR(20)', @MaLopCu, @KhoiCu;

    CREATE TABLE #PhanLoai (MaHS VARCHAR(10), HanhDong INT);

    INSERT INTO #PhanLoai (MaHS, HanhDong)
    SELECT 
        hs.MaHS,
        CASE WHEN ISNULL(td.DTB_CaNam, 0) >= 5.0 THEN 1 ELSE 0 END
    FROM HocSinh hs
    LEFT JOIN #TempDiemTB td ON hs.MaHS = td.MaHS
    WHERE hs.MaLop = @MaLopCu;

    DECLARE @SoHocSinhLenLop INT = 0;
    DECLARE @SoHocSinhOLaiLop INT = 0;
    DECLARE @SoHocSinhTotNghiep INT = 0;

    UPDATE HocSinh
    SET MaLop = @MaLopMoi_OLaiLop
    WHERE MaHS IN (SELECT MaHS FROM #PhanLoai WHERE HanhDong = 0);
    SET @SoHocSinhOLaiLop = @@ROWCOUNT;

    IF @IsLop5_TotNghiep = 0
    BEGIN
        UPDATE HocSinh
        SET MaLop = @MaLopMoi_LenLop
        WHERE MaHS IN (SELECT MaHS FROM #PhanLoai WHERE HanhDong = 1);
        SET @SoHocSinhLenLop = @@ROWCOUNT;
    END
    ELSE
    BEGIN
        UPDATE HocSinh
        SET MaLop = NULL
        WHERE MaHS IN (SELECT MaHS FROM #PhanLoai WHERE HanhDong = 1);
        SET @SoHocSinhTotNghiep = @@ROWCOUNT;
    END

    SELECT @SoHocSinhLenLop AS SoHSLenLop, 
           @SoHocSinhOLaiLop AS SoHSOLaiLop, 
           @SoHocSinhTotNghiep AS SoHSTotNghiep;

    DROP TABLE #TempDiemTB;
    DROP TABLE #PhanLoai;
END;
GO

CREATE PROCEDURE sp_GetMonByTeacher
    @id NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 gvm.MaMon
    FROM GiaoVien gv
    JOIN GiaoVien_MonHoc gvm ON gv.MaGV = gvm.MaGV
    WHERE gv.Username=@id OR gv.Ten=@id
    ORDER BY gvm.MaMon;
END;
GO

-- 📈 BÁO CÁO (ADMIN)
CREATE PROCEDURE sp_Admin_GetBaoCaoChuyenCan
    @Khoi NVARCHAR(20) = NULL,
    @MaLop VARCHAR(10) = NULL,
    @HocKy INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StartDate DATE, @EndDate DATE;
    DECLARE @NamHocStr VARCHAR(10);
    DECLARE @NamHocStartYear INT;

    IF @MaLop IS NOT NULL
        SELECT @NamHocStr = NamHoc FROM LopHoc WHERE MaLop = @MaLop;

    IF @NamHocStr IS NULL
    BEGIN
        DECLARE @CurrentMonth INT = MONTH(GETDATE());
        DECLARE @CurrentYear INT = YEAR(GETDATE());
        IF @CurrentMonth >= 8 
            SET @NamHocStartYear = @CurrentYear;
        ELSE 
            SET @NamHocStartYear = @CurrentYear - 1;
    END
    ELSE
    BEGIN
         SET @NamHocStartYear = CAST(@NamHocStr AS INT) - 1;
    END;

    IF @HocKy = 1 
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHocStartYear, 8, 1);
        SET @EndDate = DATEFROMPARTS(@NamHocStartYear, 12, 31);
    END
    ELSE IF @HocKy = 2 
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHocStartYear + 1, 1, 1);
        SET @EndDate = DATEFROMPARTS(@NamHocStartYear + 1, 5, 31);
    END
    ELSE
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHocStartYear, 8, 1);
        SET @EndDate = DATEFROMPARTS(@NamHocStartYear + 1, 5, 31);
    END;

    SELECT 
        hs.MaHS, 
        hs.HoTen,
        lh.TenLop,
        COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) as SoBuoiCoMat,
        COUNT(CASE WHEN dd.TrangThai = N'Vắng' THEN 1 END) as SoBuoiVang,
        COUNT(CASE WHEN dd.TrangThai LIKE N'%Có phép%' THEN 1 END) as SoBuoiVangCoPhep,
        COUNT(dd.MaDD) as TongSoBuoi,
        CAST(
            (COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) * 100.0) / NULLIF(COUNT(dd.MaDD), 0) 
            AS DECIMAL(5,0)
        ) as TyLeChuyenCan
    FROM HocSinh hs
    INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    LEFT JOIN DiemDanh dd ON hs.MaHS = dd.MaHS AND CAST(dd.NgayDD AS DATE) BETWEEN @StartDate AND @EndDate
    WHERE 
        (@MaLop IS NOT NULL AND hs.MaLop = @MaLop)
        OR 
        (@MaLop IS NULL AND @Khoi IS NOT NULL AND lh.Khoi = @Khoi)
        OR
        (@MaLop IS NULL AND @Khoi IS NULL)
    GROUP BY hs.MaHS, hs.HoTen, lh.TenLop
    ORDER BY lh.TenLop, hs.HoTen;
END;
GO

CREATE PROCEDURE sp_Admin_GetBangDiemHocKy
    @Khoi NVARCHAR(20) = NULL,
    @MaLop VARCHAR(10) = NULL,
    @HocKy INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @loaiFilter NVARCHAR(20);
    DECLARE @khoiFilter NVARCHAR(20) = @Khoi;

    IF @MaLop IS NOT NULL
    BEGIN
        SELECT @khoiFilter = Khoi FROM LopHoc WHERE MaLop = @MaLop;
    END

    IF @HocKy = 1 SET @loaiFilter = N'%Ki1';
    ELSE IF @HocKy = 2 SET @loaiFilter = N'%Ki2';
    ELSE SET @loaiFilter = N'%Ki%';

    ;WITH RelevantScores AS (
        SELECT 
            kq.MaHS,
            kq.MaMon,
            kq.Diem
        FROM KetQuaHocTap kq
        INNER JOIN HocSinh hs ON kq.MaHS = hs.MaHS
        INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop
        WHERE kq.Diem IS NOT NULL
          AND kq.Loai IN (SELECT MaCotDiem 
                          FROM ThoiHanDiem thd
                          WHERE thd.MaCotDiem LIKE @loaiFilter 
                            AND (@khoiFilter IS NULL OR thd.Khoi = @khoiFilter)) 
          AND (
                (@MaLop IS NOT NULL AND hs.MaLop = @MaLop)
                OR (@MaLop IS NULL AND @Khoi IS NOT NULL AND lh.Khoi = @Khoi)
                OR (@MaLop IS NULL AND @Khoi IS NULL)
              )
    ),
    AvgMon AS (
        SELECT
            MaHS,
            MaMon,
            AVG(Diem) AS DiemTBMon
        FROM RelevantScores
        GROUP BY MaHS, MaMon
    ),
    AvgCaNhan AS (
         SELECT 
             MaHS,
             AVG(DiemTBMon) AS DiemTBCaNhan
         FROM AvgMon
         GROUP BY MaHS
    )
    SELECT 
        hs.MaHS, 
        hs.HoTen, 
        lh.TenLop,
        ISNULL(acn.DiemTBCaNhan, 0) AS [Trung bình chung]
    FROM HocSinh hs
    INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    LEFT JOIN AvgCaNhan acn ON hs.MaHS = acn.MaHS
    WHERE 
        (@MaLop IS NOT NULL AND hs.MaLop = @MaLop)
        OR 
        (@MaLop IS NULL AND @Khoi IS NOT NULL AND lh.Khoi = @Khoi)
        OR
        (@MaLop IS NULL AND @Khoi IS NULL)
    ORDER BY lh.TenLop, hs.HoTen;
END;
GO

CREATE PROCEDURE sp_Admin_GetHoSoHocSinh
    @Khoi NVARCHAR(20) = NULL,
    @MaLop VARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        hs.MaHS, 
        hs.HoTen, 
        lh.TenLop,
        hs.GioiTinh, 
        hs.NgaySinh, 
        hs.DanToc, 
        hs.DiaChi, 
        hs.SDTPhuHuynh
    FROM HocSinh hs
    INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    WHERE 
        (@MaLop IS NOT NULL AND hs.MaLop = @MaLop)
        OR 
        (@MaLop IS NULL AND @Khoi IS NOT NULL AND lh.Khoi = @Khoi)
        OR
        (@MaLop IS NULL AND @Khoi IS NULL)
    ORDER BY lh.TenLop, hs.HoTen;
END;
GO

Create PROCEDURE sp_GetThongKeKhoi_Admin
    @khoi NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @khoiFilter NVARCHAR(25) = @khoi;

    DECLARE @loaiList TABLE (Loai NVARCHAR(20));
    INSERT INTO @loaiList (Loai)
    SELECT MaCotDiem 
    FROM ThoiHanDiem 
    WHERE (@khoiFilter IS NULL OR Khoi = @khoiFilter);

    WITH StudentCounts AS (
        SELECT
            l.Khoi,
            l.MaLop,
            l.TenLop,
            COUNT(hs.MaHS) AS SoHocSinh,
            SUM(CASE WHEN hs.GioiTinh = N'Nam' THEN 1 ELSE 0 END) AS SoNam,
            SUM(CASE WHEN hs.GioiTinh = N'Nữ' THEN 1 ELSE 0 END) AS SoNu
        FROM LopHoc l
        LEFT JOIN HocSinh hs ON l.MaLop = hs.MaLop
        WHERE (@khoiFilter IS NULL OR l.Khoi = @khoiFilter)
        GROUP BY l.Khoi, l.MaLop, l.TenLop
    ),
    AvgScores AS (
        SELECT
            l.MaLop,
            ROUND(AVG(kq.Diem), 2) AS DiemTrungBinh
        FROM LopHoc l
        LEFT JOIN HocSinh hs ON l.MaLop = hs.MaLop
        LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS
        WHERE (@khoiFilter IS NULL OR l.Khoi = @khoiFilter) 
          AND kq.Diem IS NOT NULL
          AND kq.Loai IN (SELECT Loai FROM @loaiList)
        GROUP BY l.MaLop
    )
    SELECT
        sc.Khoi,
        sc.TenLop,
        sc.SoHocSinh,
        ISNULL(av.DiemTrungBinh, 0) AS DiemTrungBinh,
        sc.SoNam,
        sc.SoNu
    FROM StudentCounts sc
    LEFT JOIN AvgScores av ON sc.MaLop = av.MaLop
    ORDER BY sc.Khoi, sc.TenLop;
END;
GO

CREATE PROCEDURE sp_Admin_GetBaoCaoThang_ThongKe
    @Khoi NVARCHAR(20) = NULL,
    @MaMon VARCHAR(10),
    @LoaiDiem VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH RawData AS (
        SELECT 
            hs.GioiTinh,
            hs.DanToc,
            kq.Diem
        FROM KetQuaHocTap kq
        JOIN HocSinh hs ON kq.MaHS = hs.MaHS
        JOIN LopHoc lh ON hs.MaLop = lh.MaLop
        WHERE 
            (@Khoi IS NULL OR lh.Khoi = @Khoi)
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
                ELSE N'Dưới 5'
            END AS NhomDiem,
            CASE 
                WHEN Diem >= 7 THEN 'T'
                WHEN Diem >= 5 THEN 'H'
                ELSE 'C'
            END AS XepLoai,
            CASE WHEN GioiTinh = N'Nữ' THEN 1 ELSE 0 END AS IsNu,
            CASE WHEN DanToc IS NOT NULL AND DanToc != N'Kinh' THEN 1 ELSE 0 END AS IsDanTocThieuSo,
            CASE WHEN GioiTinh = N'Nữ' AND (DanToc IS NOT NULL AND DanToc != N'Kinh') THEN 1 ELSE 0 END AS IsNuDanTocThieuSo
        FROM RawData
    ),
    DiemStats AS (
        SELECT 
            'Diem' AS LoaiThongKe,
            NhomDiem AS PhanLoai,
            COUNT(*) AS TS,
            SUM(IsNu) AS Nu,
            SUM(IsDanTocThieuSo) AS DanToc,
            SUM(IsNuDanTocThieuSo) AS NDT,
            CAST(NULL AS FLOAT) AS TyLe
        FROM ClassifiedData
        GROUP BY NhomDiem
    ),
    XepLoaiStats AS (
        SELECT 
            'XepLoai' AS LoaiThongKe,
            XepLoai AS PhanLoai,
            COUNT(*) AS TS,
            NULL AS Nu,
            NULL AS DanToc,
            NULL AS NDT,
            CAST( (COUNT(*) * 100.0) / NULLIF((SELECT COUNT(*) FROM RawData), 0) AS DECIMAL(5, 1)) AS TyLe
        FROM ClassifiedData
        GROUP BY XepLoai
    )
    SELECT * FROM DiemStats
    UNION ALL
    SELECT * FROM XepLoaiStats;
END;
GO

-- ================================================================
-- KHỞI TẠO DỮ LIỆU BAN ĐẦU
-- ================================================================
PRINT N'Đang khởi tạo điểm danh mặc định cho 5 lớp...';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '1A1';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '2A1';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '3A1';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '4A1';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '5A1';
PRINT N'Đang khởi tạo kết quả học tập mặc định cho 100 học sinh...';
EXEC sp_TaoKetQuaHocTapMacDinh;
GO

PRINT 'TẤT CẢ STORED PROCEDURES VÀ TRIGGERS ĐÃ ĐƯỢC TẠO VÀ SẮP XẾP THEO MODULE.';
PRINT 'QUÁ TRÌNH TÁI TẠO HOÀN TẤT!';