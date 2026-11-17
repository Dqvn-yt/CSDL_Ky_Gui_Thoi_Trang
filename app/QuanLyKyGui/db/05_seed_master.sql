USE [QuanLyKyGuiThoiTrang];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
BEGIN TRANSACTION;
DELETE FROM dbo.CHI_TIET_THANH_TOAN_KY_GUI;
DELETE FROM dbo.PHIEU_THANH_TOAN_KY_GUI;
DELETE FROM dbo.THANH_TOAN_DON_HANG;
DELETE FROM dbo.CHI_TIET_DON_HANG;
DELETE FROM dbo.DON_HANG;
DELETE FROM dbo.SAN_PHAM;
DELETE FROM dbo.PHIEU_KY_GUI;
DELETE FROM dbo.KHACH_HANG;
DELETE FROM dbo.NHAN_VIEN;
DELETE FROM dbo.THUONG_HIEU;
DELETE FROM dbo.DANH_MUC;
GO
DBCC CHECKIDENT ('dbo.KHACH_HANG', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.NHAN_VIEN', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.THUONG_HIEU', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.DANH_MUC', RESEED, 0) WITH NO_INFOMSGS;
GO
SET IDENTITY_INSERT dbo.NHAN_VIEN ON;
INSERT dbo.NHAN_VIEN (MaNV, HoTen, SoDienThoai, Email, ChucVu, TrangThai) VALUES
(1, N'Nguyễn Văn Hùng', '0912345001', 'hung.nguyen@thienkimconsign.vn', N'Quản lý', N'Đang làm việc'),
(2, N'Trần Thị Mai', '0912345002', 'mai.tran@thienkimconsign.vn', N'Thu ngân', N'Đang làm việc'),
(3, N'Lê Quốc Bảo', '0912345003', 'bao.le@thienkimconsign.vn', N'Nhân viên', N'Đang làm việc'),
(4, N'Phạm Thu Trang', '0912345004', 'trang.pham@thienkimconsign.vn', N'Nhân viên', N'Đang làm việc'),
(5, N'Hoàng Minh Tuấn', '0912345005', 'tuan.hoang@thienkimconsign.vn', N'Nhân viên', N'Đang làm việc'),
(6, N'Đỗ Thị Hồng', '0912345006', 'hong.do@thienkimconsign.vn', N'Thu ngân', N'Đang làm việc'),
(7, N'Vũ Đức Thắng', '0912345007', NULL, N'Nhân viên', N'Đã nghỉ'),
(8, N'Ngô Thanh Hà', '0912345008', 'ha.ngo@thienkimconsign.vn', N'Nhân viên', N'Đã nghỉ');
SET IDENTITY_INSERT dbo.NHAN_VIEN OFF;
SET IDENTITY_INSERT dbo.KHACH_HANG ON;
INSERT dbo.KHACH_HANG (MaKH, HoTen, SoDienThoai, Email, DiaChi) VALUES
(1, N'Nguyễn Thị Lan', '0901000001', 'lan.nguyen@gmail.com', N'12 Ngõ 45 Cầu Giấy, Hà Nội'),
(2, N'Trần Văn Minh', '0901000002', 'minh.tran@gmail.com', N'78 Lê Thanh Nghị, Hà Nội'),
(3, N'Lê Thu Hà', '0901000003', 'ha.le@gmail.com', N'25 Nguyễn Chí Thanh, Hà Nội'),
(4, N'Phạm Quốc Bảo', '0901000004', NULL, N'102 Hoàng Diệu, Đà Nẵng'),
(5, N'Hoàng Thị Ngọc', '0901000005', 'ngoc.hoang@gmail.com', N'33 Lý Thường Kiệt, Hà Nội'),
(6, N'Đặng Thùy Linh', '0901000006', 'linh.dang@gmail.com', N'56 Điện Biên Phủ, TP. Hồ Chí Minh'),
(7, N'Bùi Minh Khoa', '0901000007', 'khoa.bui@gmail.com', N'89 Cách Mạng Tháng 8, TP. Hồ Chí Minh'),
(8, N'Vũ Đức Anh', '0901000008', 'anh.vu@gmail.com', N'14 Phan Đình Phùng, Đà Nẵng'),
(9, N'Đinh Công Sơn', '0901000009', NULL, N'67 Trần Hưng Đạo, Hà Nội'),
(10, N'Nguyễn Mai Phương', '0901000010', 'phuong.nguyen@gmail.com', N'21 Nguyễn Văn Cừ, Hà Nội'),
(11, N'Trần Hoàng Nam', '0901000011', 'nam.tran@gmail.com', N'45 Bạch Đằng, Đà Nẵng'),
(12, N'Lê Thị Kim Oanh', '0901000012', 'oanh.le@gmail.com', N'73 Hai Bà Trưng, TP. Hồ Chí Minh'),
(13, N'Phạm Đức Huy', '0901000013', 'huy.pham@gmail.com', N'8 Tôn Đức Thắng, Hà Nội'),
(14, N'Hoàng Lan Anh', '0901000014', 'lananh.hoang@gmail.com', N'19 Nguyễn Huệ, TP. Hồ Chí Minh'),
(15, N'Nguyễn Quốc Trung', '0901000015', 'trung.nguyen@gmail.com', N'91 Đống Đa, Đà Nẵng'),
(16, N'Trần Thị Bích', '0901000016', 'bich.tran@gmail.com', N'37 Kim Mã, Hà Nội'),
(17, N'Lê Văn Tùng', '0901000017', 'tung.le@gmail.com', N'64 Nguyễn Trãi, Hà Nội'),
(18, N'Phạm Thị Hương', '0901000018', NULL, N'15 Lê Lợi, Đà Nẵng'),
(19, N'Hoàng Văn Sơn', '0901000019', 'son.hoang@gmail.com', N'82 Pasteur, TP. Hồ Chí Minh'),
(20, N'Nguyễn Thị Thu', '0901000020', 'thu.nguyen@gmail.com', N'29 Quang Trung, Hà Nội'),
(21, N'Trần Minh Đức', '0901000021', 'duc.tran@gmail.com', N'53 Ngô Quyền, Hà Nội'),
(22, N'Lê Thị Ngân', '0901000022', 'ngan.le@gmail.com', N'41 Trần Phú, Đà Nẵng'),
(23, N'Phạm Văn Long', '0901000023', 'long.pham@gmail.com', N'76 Võ Văn Tần, TP. Hồ Chí Minh'),
(24, N'Hoàng Thu Hằng', '0901000024', NULL, NULL);
SET IDENTITY_INSERT dbo.KHACH_HANG OFF;
SET IDENTITY_INSERT dbo.THUONG_HIEU ON;
INSERT dbo.THUONG_HIEU (MaTH, TenThuongHieu, MoTa) VALUES
(1, N'Nike', N'Thương hiệu thể thao Mỹ, phổ biến với giày chạy và đồ thể thao.'),
(2, N'Adidas', N'Thương hiệu thể thao Đức, mạnh về giày sneaker và áo khoác.'),
(3, N'Zara', N'Thương hiệu thời trang nhanh Tây Ban Nha, nhiều mẫu áo khoác và đầm.'),
(4, N'Uniqlo', N'Thương hiệu Nhật Bản, chú trọng đồ basic và chất liệu thoải mái.'),
(5, N'H&M', N'Thương hiệu thời trang bình dân Thụy Điển.'),
(6, N'Levi''s', N'Thương hiệu jeans lâu đời của Mỹ.'),
(7, N'Gucci', N'Nhà mốt Ý với túi xách, giày và phụ kiện da cao cấp.'),
(8, N'Chanel', N'Nhà mốt Pháp nổi tiếng với túi flap và trang sức.'),
(9, N'Louis Vuitton', N'Nhà mốt Pháp với túi monogram đặc trưng.'),
(10, N'Mango', N'Thương hiệu thời trang Tây Ban Nha, thiên về công sở và thanh lịch.');
SET IDENTITY_INSERT dbo.THUONG_HIEU OFF;
SET IDENTITY_INSERT dbo.DANH_MUC ON;
INSERT dbo.DANH_MUC (MaDM, TenDanhMuc, MoTa) VALUES
(1, N'Quần áo', N'Áo, quần, váy, đầm và các loại trang phục.'),
(2, N'Giày dép', N'Giày sneaker, cao gót, boots, sandal.'),
(3, N'Mũ nón', N'Mũ lưỡi trai, bucket, beret và các loại mũ.'),
(4, N'Túi xách', N'Túi xách, túi đeo chéo, balo, ví.'),
(5, N'Trang sức', N'Nhẫn, dây chuyền, bông tai, lắc tay.'),
(6, N'Phụ kiện', N'Kính râm, thắt lưng, khăn và phụ kiện nhỏ.');
SET IDENTITY_INSERT dbo.DANH_MUC OFF;
COMMIT TRANSACTION;
GO
PRINT N'Seed master: 8 nhan vien, 24 khach hang, 10 thuong hieu, 6 danh muc.';
GO