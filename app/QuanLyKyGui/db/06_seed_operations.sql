USE [QuanLyKyGuiThoiTrang];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
ALTER TABLE dbo.CHI_TIET_THANH_TOAN_KY_GUI DISABLE TRIGGER ALL;
ALTER TABLE dbo.PHIEU_THANH_TOAN_KY_GUI DISABLE TRIGGER ALL;
ALTER TABLE dbo.THANH_TOAN_DON_HANG DISABLE TRIGGER ALL;
ALTER TABLE dbo.CHI_TIET_DON_HANG DISABLE TRIGGER ALL;
ALTER TABLE dbo.DON_HANG DISABLE TRIGGER ALL;
ALTER TABLE dbo.SAN_PHAM DISABLE TRIGGER ALL;
GO
BEGIN TRANSACTION;
DELETE FROM dbo.CHI_TIET_THANH_TOAN_KY_GUI;
DELETE FROM dbo.PHIEU_THANH_TOAN_KY_GUI;
DELETE FROM dbo.THANH_TOAN_DON_HANG;
DELETE FROM dbo.CHI_TIET_DON_HANG;
DELETE FROM dbo.DON_HANG;
DELETE FROM dbo.SAN_PHAM;
DELETE FROM dbo.PHIEU_KY_GUI;
GO
DBCC CHECKIDENT ('dbo.PHIEU_KY_GUI', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.SAN_PHAM', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.DON_HANG', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.CHI_TIET_DON_HANG', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.THANH_TOAN_DON_HANG', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.PHIEU_THANH_TOAN_KY_GUI', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.CHI_TIET_THANH_TOAN_KY_GUI', RESEED, 0) WITH NO_INFOMSGS;
GO
SET IDENTITY_INSERT dbo.PHIEU_KY_GUI ON;
INSERT dbo.PHIEU_KY_GUI (MaPKG, MaKH, MaNV, NgayKyGui, TrangThai, GhiChu) VALUES
(1, 1, 3, CAST('2026-01-05 09:15' AS DATETIME2(0)), N'Đang xử lý', N'Khách ký gửi lần đầu, đã kiểm tra cùng khách.'),
(2, 2, 4, CAST('2026-01-12 14:20' AS DATETIME2(0)), N'Đang xử lý', N'Nhận 3 món, hẹn duyệt trong ngày.'),
(3, 3, 7, CAST('2026-01-20 10:05' AS DATETIME2(0)), N'Đang xử lý', N'Khách mang váy và sandal cũ, cần kiểm định kỹ.'),
(4, 4, 5, CAST('2026-02-02 15:40' AS DATETIME2(0)), N'Đang xử lý', N'Khách muốn giữ lại kính râm để cân nhắc.'),
(5, 5, 4, CAST('2026-02-10 11:10' AS DATETIME2(0)), N'Đang xử lý', N'Túi Gucci có kèm túi chống bụi.'),
(6, 6, 3, CAST('2026-02-18 16:25' AS DATETIME2(0)), N'Đang xử lý', N'Khách ký gửi đầm và nhẫn bạc.'),
(7, 7, 5, CAST('2026-03-02 09:50' AS DATETIME2(0)), N'Đang xử lý', N'Thắt lưng da bị xước nhẹ, khách đồng ý giá thấp.'),
(8, 8, 4, CAST('2026-03-11 13:35' AS DATETIME2(0)), N'Đang xử lý', N'Boots size 39, khách đi ít.'),
(9, 9, 3, CAST('2026-03-20 10:45' AS DATETIME2(0)), N'Đang xử lý', N'Giày Gucci còn hộp và túi vải.'),
(10, 10, 8, CAST('2026-04-01 15:05' AS DATETIME2(0)), N'Đang xử lý', N'Khách quen, ký gửi túi và dây chuyền.'),
(11, 11, 4, CAST('2026-04-09 09:30' AS DATETIME2(0)), N'Đang xử lý', N'Áo khoác Adidas và giày chạy bộ.'),
(12, 12, 3, CAST('2026-04-17 14:55' AS DATETIME2(0)), N'Đang xử lý', N'Túi LV có hóa đơn mua hàng.'),
(13, 13, 5, CAST('2026-04-26 11:20' AS DATETIME2(0)), N'Đang xử lý', N'Mũ bucket không đạt, sẽ từ chối.'),
(14, 14, 4, CAST('2026-05-05 16:10' AS DATETIME2(0)), N'Đang xử lý', N'Trang sức Chanel còn hộp.'),
(15, 15, 3, CAST('2026-05-14 09:40' AS DATETIME2(0)), N'Đang xử lý', N'Khách ký gửi đồ công sở.'),
(16, 16, 5, CAST('2026-05-22 14:15' AS DATETIME2(0)), N'Đang xử lý', N'Giày cao gót bị xước gót.'),
(17, 17, 4, CAST('2026-06-03 10:30' AS DATETIME2(0)), N'Đang xử lý', N'Áo khoác và mũ thể thao.'),
(18, 18, 3, CAST('2026-06-12 15:50' AS DATETIME2(0)), N'Đang xử lý', N'Túi Chanel và áo sơ mi.'),
(19, 19, 5, CAST('2026-06-21 11:05' AS DATETIME2(0)), N'Đang xử lý', N'Giày Samba size 42.'),
(20, 20, 4, CAST('2026-07-02 09:20' AS DATETIME2(0)), N'Đang xử lý', N'Kính râm gọng đồi mồi bị trầy.'),
(21, 21, 3, CAST('2026-07-11 16:35' AS DATETIME2(0)), N'Đang xử lý', N'Blazer Uniqlo và túi Mango.'),
(22, 22, 5, CAST('2026-07-20 10:15' AS DATETIME2(0)), N'Đang xử lý', N'Dây chuyền Chanel còn hộp.'),
(23, 23, 4, CAST('2026-08-03 14:45' AS DATETIME2(0)), N'Đang xử lý', N'Giày Gucci và mũ beret.'),
(24, 1, 3, CAST('2026-08-12 09:55' AS DATETIME2(0)), N'Đang xử lý', N'Khách ký gửi thêm túi và áo thun.'),
(25, 24, 5, CAST('2026-08-21 15:25' AS DATETIME2(0)), N'Đang xử lý', N'Khách mới, ký gửi váy và giày sandal.'),
(26, 2, 4, CAST('2026-09-04 10:40' AS DATETIME2(0)), N'Đang xử lý', N'Áo khoác Zara và jeans H&M.'),
(27, 3, 3, CAST('2026-09-18 14:05' AS DATETIME2(0)), N'Đang xử lý', N'Khách quay lại ký gửi sau lần trước.'),
(28, 5, 5, CAST('2026-11-06 10:50' AS DATETIME2(0)), N'Đang xử lý', N'Hai món hàng hiệu đang chờ kiểm định.');
SET IDENTITY_INSERT dbo.PHIEU_KY_GUI OFF;
SET IDENTITY_INSERT dbo.SAN_PHAM ON;
INSERT dbo.SAN_PHAM (MaSP, MaPKG, MaDM, MaTH, TenSP, MoTa, KichThuoc, MauSac, ChatLieu, TinhTrang, GiaDeXuat, GiaDangBan, TrangThaiTiepNhan, TrangThaiBan, NgayDangBan) VALUES
(1, 1, 1, 1, N'Áo khoác gió Nike Windrunner', N'Áo khoác gió Nike Windrunner. Màu Đen, chất liệu Polyester, kích thước L. Đã qua sử dụng, còn tốt 90%.', N'L', N'Đen', N'Polyester', N'Đã qua sử dụng, còn tốt 90%', 800000, 750000, N'Đã chấp nhận', N'Đã bán', CAST('2026-01-07 09:00' AS DATETIME2(0))),
(2, 1, 1, 6, N'Quần jeans Levi''s 501', N'Quần jeans Levi''s 501. Màu Xanh nhạt, chất liệu Denim, kích thước 31. Đã qua sử dụng, còn tốt 85%.', N'31', N'Xanh nhạt', N'Denim', N'Đã qua sử dụng, còn tốt 85%', 600000, 550000, N'Đã chấp nhận', N'Đã bán', CAST('2026-01-07 09:00' AS DATETIME2(0))),
(3, 1, 4, 3, N'Túi xách Zara da PU', N'Túi xách Zara da PU. Màu Nâu, chất liệu Da PU, kích thước One size. Cũ nhẹ, có vài vết xước nhỏ.', N'One size', N'Nâu', N'Da PU', N'Cũ nhẹ, có vài vết xước nhỏ', 500000, 450000, N'Đã chấp nhận', N'Đã trả lại', CAST('2026-01-07 09:00' AS DATETIME2(0))),
(4, 2, 2, 2, N'Giày sneaker Adidas Ultraboost 22', N'Giày sneaker Adidas Ultraboost 22. Màu Xám, chất liệu Vải dệt, kích thước 42. Đã qua sử dụng, còn tốt 90%.', N'42', N'Xám', N'Vải dệt', N'Đã qua sử dụng, còn tốt 90%', 1000000, 900000, N'Đã chấp nhận', N'Đã bán', CAST('2026-01-14 09:00' AS DATETIME2(0))),
(5, 2, 1, 4, N'Áo thun Uniqlo U oversize', N'Áo thun Uniqlo U oversize. Màu Trắng, chất liệu Cotton, kích thước M. Mới 95%, chưa giặt.', N'M', N'Trắng', N'Cotton', N'Mới 95%, chưa giặt', 300000, 250000, N'Đã chấp nhận', N'Đã bán', CAST('2026-01-14 09:00' AS DATETIME2(0))),
(6, 2, 3, 1, N'Mũ lưỡi trai Nike Heritage', N'Mũ lưỡi trai Nike Heritage. Màu Đen, chất liệu Cotton, kích thước Free size. Đã qua sử dụng, còn tốt 90%.', N'Free size', N'Đen', N'Cotton', N'Đã qua sử dụng, còn tốt 90%', 250000, 200000, N'Đã chấp nhận', N'Đã bán', CAST('2026-01-14 09:00' AS DATETIME2(0))),
(7, 3, 1, 3, N'Váy hoa Zara dáng dài', N'Váy hoa Zara dáng dài. Màu Hồng, chất liệu Vải voan, kích thước S. Đã qua sử dụng, còn tốt 80%.', N'S', N'Hồng', N'Vải voan', N'Đã qua sử dụng, còn tốt 80%', 400000, NULL, N'Từ chối', NULL, NULL),
(8, 3, 2, 5, N'Sandal H&M quai ngang', N'Sandal H&M quai ngang. Màu Be, chất liệu Da PU, kích thước 38. Đã qua sử dụng, còn tốt 85%.', N'38', N'Be', N'Da PU', N'Đã qua sử dụng, còn tốt 85%', 200000, NULL, N'Từ chối', NULL, NULL),
(9, 4, 1, 4, N'Áo sơ mi Uniqlo Oxford', N'Áo sơ mi Uniqlo Oxford. Màu Xanh nhạt, chất liệu Cotton, kích thước L. Còn tốt 95%.', N'L', N'Xanh nhạt', N'Cotton', N'Còn tốt 95%', 350000, 300000, N'Đã chấp nhận', N'Đã bán', CAST('2026-02-04 09:00' AS DATETIME2(0))),
(10, 4, 4, 10, N'Balo mini Mango', N'Balo mini Mango. Màu Đen, chất liệu Da PU, kích thước One size. Đã qua sử dụng, còn tốt 90%.', N'One size', N'Đen', N'Da PU', N'Đã qua sử dụng, còn tốt 90%', 450000, 400000, N'Đã chấp nhận', N'Đã bán', CAST('2026-02-04 09:00' AS DATETIME2(0))),
(11, 4, 6, 7, N'Kính râm Gucci GG', N'Kính râm Gucci GG. Màu Đen, chất liệu Nhựa, kích thước One size. Còn mới 98%, chưa dùng.', N'One size', N'Đen', N'Nhựa', N'Còn mới 98%, chưa dùng', 1300000, NULL, N'Chờ xử lý', NULL, NULL),
(12, 5, 1, 3, N'Áo len cardigan Zara', N'Áo len cardigan Zara. Màu Kem, chất liệu Len pha, kích thước M. Đã qua sử dụng, còn tốt 90%.', N'M', N'Kem', N'Len pha', N'Đã qua sử dụng, còn tốt 90%', 500000, 450000, N'Đã chấp nhận', N'Đã bán', CAST('2026-02-12 09:00' AS DATETIME2(0))),
(13, 5, 2, 10, N'Giày cao gót Mango', N'Giày cao gót Mango. Màu Đen, chất liệu Da lộn, kích thước 38. Đã qua sử dụng, còn tốt 90%.', N'38', N'Đen', N'Da lộn', N'Đã qua sử dụng, còn tốt 90%', 550000, 500000, N'Đã chấp nhận', N'Đã bán', CAST('2026-02-12 09:00' AS DATETIME2(0))),
(14, 5, 4, 7, N'Túi đeo chéo Gucci Marmont', N'Túi đeo chéo Gucci Marmont. Màu Đen, chất liệu Da thật, kích thước Mini. Còn tốt 95%.', N'Mini', N'Đen', N'Da thật', N'Còn tốt 95%', 2800000, 2500000, N'Đã chấp nhận', N'Đã bán', CAST('2026-02-12 09:00' AS DATETIME2(0))),
(15, 6, 1, 5, N'Đầm suông H&M', N'Đầm suông H&M. Màu Xanh navy, chất liệu Viscose, kích thước M. Đã qua sử dụng, còn tốt 90%.', N'M', N'Xanh navy', N'Viscose', N'Đã qua sử dụng, còn tốt 90%', 400000, 350000, N'Đã chấp nhận', N'Đã bán', CAST('2026-02-20 09:00' AS DATETIME2(0))),
(16, 6, 5, NULL, N'Nhẫn bạc đôi', N'Nhẫn bạc đôi. Màu Bạc, chất liệu Bạc 925, kích thước Size 10. Đã qua sử dụng, còn tốt 90%.', N'Size 10', N'Bạc', N'Bạc 925', N'Đã qua sử dụng, còn tốt 90%', 350000, 300000, N'Đã chấp nhận', N'Đã bán', CAST('2026-02-20 09:00' AS DATETIME2(0))),
(17, 7, 1, 1, N'Áo polo Nike Dri-FIT', N'Áo polo Nike Dri-FIT. Màu Xanh rêu, chất liệu Polyester, kích thước L. Đã qua sử dụng, còn tốt 90%.', N'L', N'Xanh rêu', N'Polyester', N'Đã qua sử dụng, còn tốt 90%', 450000, 400000, N'Đã chấp nhận', N'Đã bán', CAST('2026-03-04 09:00' AS DATETIME2(0))),
(18, 7, 1, 3, N'Quần tây Zara ống đứng', N'Quần tây Zara ống đứng. Màu Đen, chất liệu Polyester pha, kích thước 30. Còn tốt 95%.', N'30', N'Đen', N'Polyester pha', N'Còn tốt 95%', 500000, 450000, N'Đã chấp nhận', N'Đã bán', CAST('2026-03-04 09:00' AS DATETIME2(0))),
(19, 7, 6, 6, N'Thắt lưng da Levi''s', N'Thắt lưng da Levi''s. Màu Nâu, chất liệu Da bò, kích thước 95cm. Cũ nhẹ, có vài vết xước nhỏ.', N'95cm', N'Nâu', N'Da bò', N'Cũ nhẹ, có vài vết xước nhỏ', 400000, NULL, N'Từ chối', NULL, NULL),
(20, 8, 2, 3, N'Boots cổ ngắn Zara', N'Boots cổ ngắn Zara. Màu Nâu, chất liệu Da lộn, kích thước 39. Đã qua sử dụng, còn tốt 90%.', N'39', N'Nâu', N'Da lộn', N'Đã qua sử dụng, còn tốt 90%', 750000, 700000, N'Đã chấp nhận', N'Đã trả lại', CAST('2026-03-13 09:00' AS DATETIME2(0))),
(21, 8, 1, 10, N'Áo blazer Mango', N'Áo blazer Mango. Màu Be, chất liệu Polyester, kích thước S. Còn tốt 95%.', N'S', N'Be', N'Polyester', N'Còn tốt 95%', 700000, 650000, N'Đã chấp nhận', N'Đã bán', CAST('2026-03-13 09:00' AS DATETIME2(0))),
(22, 9, 2, 7, N'Giày lười Gucci da bò', N'Giày lười Gucci da bò. Màu Đen, chất liệu Da bò, kích thước 41. Còn tốt 95%.', N'41', N'Đen', N'Da bò', N'Còn tốt 95%', 2000000, 1800000, N'Đã chấp nhận', N'Đã bán', CAST('2026-03-22 09:00' AS DATETIME2(0))),
(23, 9, 1, 5, N'Áo thun oversize H&M', N'Áo thun oversize H&M. Màu Trắng, chất liệu Cotton, kích thước XL. Đã qua sử dụng, còn tốt 90%.', N'XL', N'Trắng', N'Cotton', N'Đã qua sử dụng, còn tốt 90%', 250000, 200000, N'Đã chấp nhận', N'Đã bán', CAST('2026-03-22 09:00' AS DATETIME2(0))),
(24, 9, 3, NULL, N'Mũ beret nỉ', N'Mũ beret nỉ. Màu Đen, chất liệu Nỉ, kích thước Free size. Mới 95%, chưa giặt.', N'Free size', N'Đen', N'Nỉ', N'Mới 95%, chưa giặt', 180000, 150000, N'Đã chấp nhận', N'Đang giữ', CAST('2026-03-22 09:00' AS DATETIME2(0))),
(25, 10, 4, 10, N'Túi tote da Mango', N'Túi tote da Mango. Màu Nâu nhạt, chất liệu Da PU, kích thước One size. Đã qua sử dụng, còn tốt 90%.', N'One size', N'Nâu nhạt', N'Da PU', N'Đã qua sử dụng, còn tốt 90%', 650000, 600000, N'Đã chấp nhận', N'Đã bán', CAST('2026-04-03 09:00' AS DATETIME2(0))),
(26, 10, 5, NULL, N'Dây chuyền bạc mặt ngọc', N'Dây chuyền bạc mặt ngọc. Màu Bạc, chất liệu Bạc 925, kích thước 45cm. Còn tốt 95%.', N'45cm', N'Bạc', N'Bạc 925', N'Còn tốt 95%', 400000, 350000, N'Đã chấp nhận', N'Đã bán', CAST('2026-04-03 09:00' AS DATETIME2(0))),
(27, 11, 1, 2, N'Áo khoác Adidas Track Top', N'Áo khoác Adidas Track Top. Màu Xanh dương, chất liệu Polyester, kích thước M. Đã qua sử dụng, còn tốt 90%.', N'M', N'Xanh dương', N'Polyester', N'Đã qua sử dụng, còn tốt 90%', 700000, 650000, N'Đã chấp nhận', N'Đã bán', CAST('2026-04-11 09:00' AS DATETIME2(0))),
(28, 11, 1, 4, N'Quần jeans Uniqlo Selvedge', N'Quần jeans Uniqlo Selvedge. Màu Xanh đậm, chất liệu Denim, kích thước 32. Còn tốt 95%.', N'32', N'Xanh đậm', N'Denim', N'Còn tốt 95%', 450000, 400000, N'Đã chấp nhận', N'Đã bán', CAST('2026-04-11 09:00' AS DATETIME2(0))),
(29, 11, 2, 1, N'Giày chạy bộ Nike Pegasus', N'Giày chạy bộ Nike Pegasus. Màu Trắng, chất liệu Vải lưới, kích thước 42. Đã qua sử dụng, còn tốt 90%.', N'42', N'Trắng', N'Vải lưới', N'Đã qua sử dụng, còn tốt 90%', 900000, 850000, N'Đã chấp nhận', N'Đã bán', CAST('2026-04-11 09:00' AS DATETIME2(0))),
(30, 12, 1, 3, N'Áo sơ mi Zara lụa', N'Áo sơ mi Zara lụa. Màu Trắng ngà, chất liệu Lụa pha, kích thước M. Còn tốt 95%.', N'M', N'Trắng ngà', N'Lụa pha', N'Còn tốt 95%', 400000, 350000, N'Đã chấp nhận', N'Đã bán', CAST('2026-04-19 09:00' AS DATETIME2(0))),
(31, 12, 4, 9, N'Túi xách Louis Vuitton Speedy 25', N'Túi xách Louis Vuitton Speedy 25. Màu Nâu, chất liệu Canvas, kích thước 25. Đã qua sử dụng, còn tốt 90%.', N'25', N'Nâu', N'Canvas', N'Đã qua sử dụng, còn tốt 90%', 5000000, 4500000, N'Đã chấp nhận', N'Đã bán', CAST('2026-04-19 09:00' AS DATETIME2(0))),
(32, 13, 1, 3, N'Áo blazer Zara hai cúc', N'Áo blazer Zara hai cúc. Màu Đen, chất liệu Polyester, kích thước M. Đã qua sử dụng, còn tốt 90%.', N'M', N'Đen', N'Polyester', N'Đã qua sử dụng, còn tốt 90%', 750000, 700000, N'Đã chấp nhận', N'Đã bán', CAST('2026-04-28 09:00' AS DATETIME2(0))),
(33, 13, 2, 1, N'Giày sneaker Nike Air Force 1', N'Giày sneaker Nike Air Force 1. Màu Trắng, chất liệu Da tổng hợp, kích thước 41. Đã qua sử dụng, còn tốt 90%.', N'41', N'Trắng', N'Da tổng hợp', N'Đã qua sử dụng, còn tốt 90%', 1000000, 950000, N'Đã chấp nhận', N'Đã bán', CAST('2026-04-28 09:00' AS DATETIME2(0))),
(34, 13, 3, 5, N'Mũ bucket H&M', N'Mũ bucket H&M. Màu Be, chất liệu Cotton, kích thước Free size. Đã qua sử dụng, còn tốt 90%.', N'Free size', N'Be', N'Cotton', N'Đã qua sử dụng, còn tốt 90%', 200000, NULL, N'Từ chối', NULL, NULL),
(35, 14, 1, 3, N'Đầm suông Zara', N'Đầm suông Zara. Màu Đen, chất liệu Viscose, kích thước S. Còn tốt 95%.', N'S', N'Đen', N'Viscose', N'Còn tốt 95%', 550000, 500000, N'Đã chấp nhận', N'Đã bán', CAST('2026-05-07 09:00' AS DATETIME2(0))),
(36, 14, 5, 8, N'Bông tai ngọc trai Chanel', N'Bông tai ngọc trai Chanel. Màu Trắng, chất liệu Ngọc trai nhân tạo, kích thước One size. Còn mới 98%, chưa dùng.', N'One size', N'Trắng', N'Ngọc trai nhân tạo', N'Còn mới 98%, chưa dùng', 1700000, 1500000, N'Đã chấp nhận', N'Đã bán', CAST('2026-05-07 09:00' AS DATETIME2(0))),
(37, 14, 4, 7, N'Ví cầm tay Gucci', N'Ví cầm tay Gucci. Màu Đen, chất liệu Da thật, kích thước One size. Đã qua sử dụng, còn tốt 90%.', N'One size', N'Đen', N'Da thật', N'Đã qua sử dụng, còn tốt 90%', 2400000, 2200000, N'Đã chấp nhận', N'Đã bán', CAST('2026-05-07 09:00' AS DATETIME2(0))),
(38, 15, 1, 4, N'Áo polo Uniqlo pique', N'Áo polo Uniqlo pique. Màu Xanh navy, chất liệu Cotton, kích thước L. Đã qua sử dụng, còn tốt 90%.', N'L', N'Xanh navy', N'Cotton', N'Đã qua sử dụng, còn tốt 90%', 300000, 250000, N'Đã chấp nhận', N'Đã bán', CAST('2026-05-16 09:00' AS DATETIME2(0))),
(39, 15, 1, 6, N'Quần jeans Levi''s 511', N'Quần jeans Levi''s 511. Màu Xanh đậm, chất liệu Denim, kích thước 32. Đã qua sử dụng, còn tốt 85%.', N'32', N'Xanh đậm', N'Denim', N'Đã qua sử dụng, còn tốt 85%', 650000, 600000, N'Đã chấp nhận', N'Đã bán', CAST('2026-05-16 09:00' AS DATETIME2(0))),
(40, 16, 1, 2, N'Áo thun Adidas Trefoil', N'Áo thun Adidas Trefoil. Màu Đen, chất liệu Cotton, kích thước M. Đã qua sử dụng, còn tốt 90%.', N'M', N'Đen', N'Cotton', N'Đã qua sử dụng, còn tốt 90%', 350000, 300000, N'Đã chấp nhận', N'Đã bán', CAST('2026-05-24 09:00' AS DATETIME2(0))),
(41, 16, 2, 3, N'Giày cao gót Zara', N'Giày cao gót Zara. Màu Đỏ, chất liệu Da PU, kích thước 37. Cũ nhẹ, có vài vết xước nhỏ.', N'37', N'Đỏ', N'Da PU', N'Cũ nhẹ, có vài vết xước nhỏ', 500000, 450000, N'Đã chấp nhận', N'Đã trả lại', CAST('2026-05-24 09:00' AS DATETIME2(0))),
(42, 16, 5, NULL, N'Lắc tay bạc khắc hoa', N'Lắc tay bạc khắc hoa. Màu Bạc, chất liệu Bạc 925, kích thước 16cm. Còn tốt 95%.', N'16cm', N'Bạc', N'Bạc 925', N'Còn tốt 95%', 450000, 400000, N'Đã chấp nhận', N'Đã bán', CAST('2026-05-24 09:00' AS DATETIME2(0))),
(43, 17, 1, 5, N'Áo khoác H&M bomber', N'Áo khoác H&M bomber. Màu Xanh rêu, chất liệu Polyester, kích thước L. Đã qua sử dụng, còn tốt 90%.', N'L', N'Xanh rêu', N'Polyester', N'Đã qua sử dụng, còn tốt 90%', 450000, 400000, N'Đã chấp nhận', N'Đã bán', CAST('2026-06-05 09:00' AS DATETIME2(0))),
(44, 17, 3, 2, N'Mũ lưỡi trai Adidas', N'Mũ lưỡi trai Adidas. Màu Đen, chất liệu Cotton, kích thước Free size. Mới 95%, chưa giặt.', N'Free size', N'Đen', N'Cotton', N'Mới 95%, chưa giặt', 250000, 220000, N'Đã chấp nhận', N'Đã bán', CAST('2026-06-05 09:00' AS DATETIME2(0))),
(45, 18, 4, 8, N'Túi xách Chanel Classic Flap', N'Túi xách Chanel Classic Flap. Màu Đen, chất liệu Da caviar, kích thước Medium. Còn tốt 95%.', N'Medium', N'Đen', N'Da caviar', N'Còn tốt 95%', 6000000, 5500000, N'Đã chấp nhận', N'Đã bán', CAST('2026-06-14 09:00' AS DATETIME2(0))),
(46, 18, 1, 10, N'Áo sơ mi Mango voan', N'Áo sơ mi Mango voan. Màu Hồng nhạt, chất liệu Voan, kích thước S. Còn tốt 95%.', N'S', N'Hồng nhạt', N'Voan', N'Còn tốt 95%', 350000, 320000, N'Đã chấp nhận', N'Đã bán', CAST('2026-06-14 09:00' AS DATETIME2(0))),
(47, 18, 2, 3, N'Sandal Zara quai mảnh', N'Sandal Zara quai mảnh. Màu Đen, chất liệu Da PU, kích thước 38. Cũ nhẹ, có vài vết xước nhỏ.', N'38', N'Đen', N'Da PU', N'Cũ nhẹ, có vài vết xước nhỏ', 420000, 380000, N'Đã chấp nhận', N'Đã trả lại', CAST('2026-06-14 09:00' AS DATETIME2(0))),
(48, 19, 2, 2, N'Giày sneaker Adidas Samba', N'Giày sneaker Adidas Samba. Màu Xanh navy, chất liệu Da lộn, kích thước 42. Đã qua sử dụng, còn tốt 90%.', N'42', N'Xanh navy', N'Da lộn', N'Đã qua sử dụng, còn tốt 90%', 900000, 850000, N'Đã chấp nhận', N'Đã bán', CAST('2026-06-23 09:00' AS DATETIME2(0))),
(49, 19, 1, 1, N'Áo thun Nike Sportswear', N'Áo thun Nike Sportswear. Màu Trắng, chất liệu Cotton, kích thước L. Đã qua sử dụng, còn tốt 90%.', N'L', N'Trắng', N'Cotton', N'Đã qua sử dụng, còn tốt 90%', 400000, 350000, N'Đã chấp nhận', N'Đã bán', CAST('2026-06-23 09:00' AS DATETIME2(0))),
(50, 20, 1, 4, N'Áo len Uniqlo merino', N'Áo len Uniqlo merino. Màu Xám, chất liệu Len merino, kích thước M. Còn tốt 95%.', N'M', N'Xám', N'Len merino', N'Còn tốt 95%', 350000, 300000, N'Đã chấp nhận', N'Đang bán', CAST('2026-07-04 09:00' AS DATETIME2(0))),
(51, 20, 1, 5, N'Quần tây H&M ống suông', N'Quần tây H&M ống suông. Màu Đen, chất liệu Polyester pha, kích thước 29. Đã qua sử dụng, còn tốt 90%.', N'29', N'Đen', N'Polyester pha', N'Đã qua sử dụng, còn tốt 90%', 400000, 350000, N'Đã chấp nhận', N'Đang bán', CAST('2026-07-04 09:00' AS DATETIME2(0))),
(52, 20, 6, NULL, N'Kính râm gọng đồi mồi', N'Kính râm gọng đồi mồi. Màu Nâu đồi mồi, chất liệu Nhựa, kích thước One size. Cũ nhẹ, có vài vết xước nhỏ.', N'One size', N'Nâu đồi mồi', N'Nhựa', N'Cũ nhẹ, có vài vết xước nhỏ', 250000, NULL, N'Từ chối', NULL, NULL),
(53, 21, 1, 4, N'Áo blazer Uniqlo', N'Áo blazer Uniqlo. Màu Xanh navy, chất liệu Polyester, kích thước M. Còn tốt 95%.', N'M', N'Xanh navy', N'Polyester', N'Còn tốt 95%', 550000, 500000, N'Đã chấp nhận', N'Đã bán', CAST('2026-07-13 09:00' AS DATETIME2(0))),
(54, 21, 2, 3, N'Giày lười Zara', N'Giày lười Zara. Màu Đen, chất liệu Da PU, kích thước 40. Đã qua sử dụng, còn tốt 90%.', N'40', N'Đen', N'Da PU', N'Đã qua sử dụng, còn tốt 90%', 600000, 550000, N'Đã chấp nhận', N'Đã bán', CAST('2026-07-13 09:00' AS DATETIME2(0))),
(55, 21, 4, 10, N'Túi đeo chéo Mango', N'Túi đeo chéo Mango. Màu Be, chất liệu Da PU, kích thước Mini. Còn tốt 95%.', N'Mini', N'Be', N'Da PU', N'Còn tốt 95%', 500000, 450000, N'Đã chấp nhận', N'Đã bán', CAST('2026-07-13 09:00' AS DATETIME2(0))),
(56, 22, 1, 6, N'Áo khoác Levi''s trucker', N'Áo khoác Levi''s trucker. Màu Xanh nhạt, chất liệu Denim, kích thước L. Đã qua sử dụng, còn tốt 90%.', N'L', N'Xanh nhạt', N'Denim', N'Đã qua sử dụng, còn tốt 90%', 650000, 600000, N'Đã chấp nhận', N'Đã bán', CAST('2026-07-22 09:00' AS DATETIME2(0))),
(57, 22, 5, 8, N'Dây chuyền Chanel ngọc trai', N'Dây chuyền Chanel ngọc trai. Màu Trắng, chất liệu Ngọc trai nhân tạo, kích thước 42cm. Còn mới 98%, chưa dùng.', N'42cm', N'Trắng', N'Ngọc trai nhân tạo', N'Còn mới 98%, chưa dùng', 3200000, 3000000, N'Đã chấp nhận', N'Đã bán', CAST('2026-07-22 09:00' AS DATETIME2(0))),
(58, 23, 1, 5, N'Áo sơ mi H&M cotton', N'Áo sơ mi H&M cotton. Màu Trắng, chất liệu Cotton, kích thước M. Đã qua sử dụng, còn tốt 90%.', N'M', N'Trắng', N'Cotton', N'Đã qua sử dụng, còn tốt 90%', 300000, 250000, N'Đã chấp nhận', N'Đã bán', CAST('2026-08-05 09:00' AS DATETIME2(0))),
(59, 23, 2, 7, N'Giày cao gót Gucci', N'Giày cao gót Gucci. Màu Đen, chất liệu Da thật, kích thước 38. Còn tốt 95%.', N'38', N'Đen', N'Da thật', N'Còn tốt 95%', 1800000, 1600000, N'Đã chấp nhận', N'Đã bán', CAST('2026-08-05 09:00' AS DATETIME2(0))),
(60, 23, 3, 3, N'Mũ beret Zara dạ', N'Mũ beret Zara dạ. Màu Đen, chất liệu Dạ, kích thước Free size. Mới 95%, chưa giặt.', N'Free size', N'Đen', N'Dạ', N'Mới 95%, chưa giặt', 250000, 200000, N'Đã chấp nhận', N'Đã bán', CAST('2026-08-05 09:00' AS DATETIME2(0))),
(61, 24, 4, 4, N'Túi tote Uniqlo canvas', N'Túi tote Uniqlo canvas. Màu Trắng, chất liệu Canvas, kích thước One size. Đã qua sử dụng, còn tốt 90%.', N'One size', N'Trắng', N'Canvas', N'Đã qua sử dụng, còn tốt 90%', 350000, 300000, N'Đã chấp nhận', N'Chưa đăng bán', NULL),
(62, 24, 1, 10, N'Áo thun Mango cotton', N'Áo thun Mango cotton. Màu Đen, chất liệu Cotton, kích thước S. Mới 95%, chưa giặt.', N'S', N'Đen', N'Cotton', N'Mới 95%, chưa giặt', 250000, 220000, N'Đã chấp nhận', N'Chưa đăng bán', NULL),
(63, 25, 1, 10, N'Váy hoa Mango dáng xòe', N'Váy hoa Mango dáng xòe. Màu Xanh, chất liệu Viscose, kích thước M. Còn tốt 95%.', N'M', N'Xanh', N'Viscose', N'Còn tốt 95%', 450000, 400000, N'Đã chấp nhận', N'Đang giữ', CAST('2026-08-23 09:00' AS DATETIME2(0))),
(64, 25, 2, 2, N'Giày sandal Adidas Adilette', N'Giày sandal Adidas Adilette. Màu Đen, chất liệu Synthetic, kích thước 40. Đã qua sử dụng, còn tốt 90%.', N'40', N'Đen', N'Synthetic', N'Đã qua sử dụng, còn tốt 90%', 550000, 500000, N'Đã chấp nhận', N'Đang giữ', CAST('2026-08-23 09:00' AS DATETIME2(0))),
(65, 25, 6, 8, N'Kính râm Chanel', N'Kính râm Chanel. Màu Đen, chất liệu Nhựa, kích thước One size. Còn mới 98%, chưa dùng.', N'One size', N'Đen', N'Nhựa', N'Còn mới 98%, chưa dùng', 2000000, 1800000, N'Đã chấp nhận', N'Đang giữ', CAST('2026-08-23 09:00' AS DATETIME2(0))),
(66, 26, 1, 3, N'Áo khoác Zara dáng dài', N'Áo khoác Zara dáng dài. Màu Be, chất liệu Polyester, kích thước M. Còn tốt 95%.', N'M', N'Be', N'Polyester', N'Còn tốt 95%', 700000, 650000, N'Đã chấp nhận', N'Đang giữ', CAST('2026-09-06 09:00' AS DATETIME2(0))),
(67, 26, 1, 5, N'Quần jeans H&M ống suông', N'Quần jeans H&M ống suông. Màu Xanh nhạt, chất liệu Denim, kích thước 31. Đã qua sử dụng, còn tốt 90%.', N'31', N'Xanh nhạt', N'Denim', N'Đã qua sử dụng, còn tốt 90%', 400000, 350000, N'Đã chấp nhận', N'Đang giữ', CAST('2026-09-06 09:00' AS DATETIME2(0))),
(68, 27, 1, 10, N'Áo len Mango dệt kim', N'Áo len Mango dệt kim. Màu Nâu, chất liệu Len pha, kích thước M. Đã qua sử dụng, còn tốt 90%.', N'M', N'Nâu', N'Len pha', N'Đã qua sử dụng, còn tốt 90%', 450000, 400000, N'Đã chấp nhận', N'Đang bán', CAST('2026-09-20 09:00' AS DATETIME2(0))),
(69, 27, 2, 3, N'Giày boots Zara da lộn', N'Giày boots Zara da lộn. Màu Đen, chất liệu Da lộn, kích thước 39. Còn tốt 95%.', N'39', N'Đen', N'Da lộn', N'Còn tốt 95%', 800000, 750000, N'Đã chấp nhận', N'Chưa đăng bán', NULL),
(70, 27, 4, 9, N'Túi xách Louis Vuitton Speedy 30', N'Túi xách Louis Vuitton Speedy 30. Màu Nâu, chất liệu Canvas, kích thước 30. Đã qua sử dụng, còn tốt 90%.', N'30', N'Nâu', N'Canvas', N'Đã qua sử dụng, còn tốt 90%', 6500000, 6000000, N'Đã chấp nhận', N'Đang giữ', CAST('2026-09-20 09:00' AS DATETIME2(0))),
(71, 28, 1, 8, N'Áo blazer Chanel tweed', N'Áo blazer Chanel tweed. Màu Trắng đen, chất liệu Tweed, kích thước S. Còn tốt 95%.', N'S', N'Trắng đen', N'Tweed', N'Còn tốt 95%', 3200000, NULL, N'Chờ xử lý', NULL, NULL),
(72, 28, 2, 7, N'Giày sneaker Gucci Ace', N'Giày sneaker Gucci Ace. Màu Trắng, chất liệu Da thật, kích thước 41. Còn tốt 95%.', N'41', N'Trắng', N'Da thật', N'Còn tốt 95%', 2800000, NULL, N'Chờ xử lý', NULL, NULL);
SET IDENTITY_INSERT dbo.SAN_PHAM OFF;
SET IDENTITY_INSERT dbo.DON_HANG ON;
INSERT dbo.DON_HANG (MaDH, MaKH, MaNV, NgayLap, HanThanhToan, TrangThai, LyDoHuy, GhiChu) VALUES
(1, 2, 3, CAST('2026-01-20 10:15' AS DATETIME2(0)), CAST('2026-01-23 10:15' AS DATETIME2(0)), N'Hoàn tất', NULL, N'Khách nhận tại cửa hàng.'),
(2, 6, 2, CAST('2026-01-25 15:30' AS DATETIME2(0)), CAST('2026-01-28 15:30' AS DATETIME2(0)), N'Hoàn tất', NULL, NULL),
(3, 7, 4, CAST('2026-01-31 09:40' AS DATETIME2(0)), CAST('2026-02-03 09:40' AS DATETIME2(0)), N'Hoàn tất', NULL, N'Khách thanh toán thẻ lần đầu bị lỗi.'),
(4, 9, 3, CAST('2026-02-14 11:20' AS DATETIME2(0)), CAST('2026-02-17 11:20' AS DATETIME2(0)), N'Hoàn tất', NULL, NULL),
(5, 11, 5, CAST('2026-02-26 14:10' AS DATETIME2(0)), CAST('2026-03-01 14:10' AS DATETIME2(0)), N'Hoàn tất', NULL, N'Thanh toán kết hợp tiền mặt và chuyển khoản.'),
(6, 13, 3, CAST('2026-03-15 10:05' AS DATETIME2(0)), CAST('2026-03-18 10:05' AS DATETIME2(0)), N'Hoàn tất', NULL, NULL),
(7, 15, 4, CAST('2026-03-19 16:20' AS DATETIME2(0)), CAST('2026-03-22 16:20' AS DATETIME2(0)), N'Hoàn tất', NULL, N'Giao dịch thẻ thất bại, khách chuyển khoản lại.'),
(8, 18, 5, CAST('2026-03-30 09:35' AS DATETIME2(0)), CAST('2026-04-02 09:35' AS DATETIME2(0)), N'Hoàn tất', NULL, NULL),
(9, 20, 3, CAST('2026-04-10 14:50' AS DATETIME2(0)), CAST('2026-04-13 14:50' AS DATETIME2(0)), N'Hoàn tất', NULL, NULL),
(10, 22, 4, CAST('2026-04-22 10:30' AS DATETIME2(0)), CAST('2026-04-25 10:30' AS DATETIME2(0)), N'Hoàn tất', NULL, N'Đơn gồm 3 món từ 2 người ký gửi.'),
(11, 3, 5, CAST('2026-05-05 15:15' AS DATETIME2(0)), CAST('2026-05-08 15:15' AS DATETIME2(0)), N'Hoàn tất', NULL, NULL),
(12, 10, 3, CAST('2026-05-17 11:45' AS DATETIME2(0)), CAST('2026-05-20 11:45' AS DATETIME2(0)), N'Hoàn tất', NULL, N'Ví điện tử lỗi, khách chuyển khoản và trả tiền mặt.'),
(13, 12, 4, CAST('2026-05-29 09:25' AS DATETIME2(0)), CAST('2026-06-01 09:25' AS DATETIME2(0)), N'Hoàn tất', NULL, NULL),
(14, 14, 5, CAST('2026-06-10 14:40' AS DATETIME2(0)), CAST('2026-06-13 14:40' AS DATETIME2(0)), N'Hoàn tất', NULL, N'Chuyển khoản 2 lần trong cùng ngày.'),
(15, 16, 3, CAST('2026-06-21 10:10' AS DATETIME2(0)), CAST('2026-06-24 10:10' AS DATETIME2(0)), N'Hoàn tất', NULL, NULL),
(16, 19, 4, CAST('2026-07-03 15:55' AS DATETIME2(0)), CAST('2026-07-06 15:55' AS DATETIME2(0)), N'Hoàn tất', NULL, NULL),
(17, 21, 5, CAST('2026-07-15 11:30' AS DATETIME2(0)), CAST('2026-07-18 11:30' AS DATETIME2(0)), N'Hoàn tất', NULL, N'Đơn giá trị cao, khách chuyển khoản toàn bộ.'),
(18, 23, 3, CAST('2026-08-02 09:45' AS DATETIME2(0)), CAST('2026-08-05 09:45' AS DATETIME2(0)), N'Hoàn tất', NULL, N'Ví điện tử thất bại, thu tiền mặt và chuyển khoản.'),
(19, 24, 4, CAST('2026-08-20 14:25' AS DATETIME2(0)), CAST('2026-08-23 14:25' AS DATETIME2(0)), N'Hoàn tất', NULL, NULL),
(20, 1, 5, CAST('2026-09-12 10:00' AS DATETIME2(0)), CAST('2026-09-15 10:00' AS DATETIME2(0)), N'Hoàn tất', NULL, N'Khách trả thừa 50.000đ, cần đối soát riêng.'),
(21, 4, 4, CAST('2026-10-10 15:20' AS DATETIME2(0)), CAST('2026-10-13 15:20' AS DATETIME2(0)), N'Đang xử lý', NULL, N'Đã thu đủ, chờ bàn giao.'),
(22, 8, 3, CAST('2026-10-18 09:50' AS DATETIME2(0)), CAST('2026-10-21 09:50' AS DATETIME2(0)), N'Đang xử lý', NULL, N'Đã thu đủ, chờ khách đến lấy.'),
(23, 12, 5, CAST('2026-10-25 14:35' AS DATETIME2(0)), CAST('2026-10-28 14:35' AS DATETIME2(0)), N'Đang xử lý', NULL, N'Đã thu đủ, chuẩn bị giao hàng.'),
(24, 16, 4, CAST('2026-11-01 10:25' AS DATETIME2(0)), CAST('2026-11-04 10:25' AS DATETIME2(0)), N'Đang xử lý', NULL, N'Sản phẩm được bán lại sau khi đơn cũ bị hủy.'),
(25, 6, 3, CAST('2026-11-10 09:30' AS DATETIME2(0)), CAST('2026-11-13 09:30' AS DATETIME2(0)), N'Chờ thanh toán', NULL, N'Khách hẹn chuyển khoản.'),
(26, 9, 5, CAST('2026-11-11 15:10' AS DATETIME2(0)), CAST('2026-11-14 15:10' AS DATETIME2(0)), N'Chờ thanh toán', NULL, NULL),
(27, 13, 4, CAST('2026-11-12 10:45' AS DATETIME2(0)), CAST('2026-11-15 10:45' AS DATETIME2(0)), N'Chờ thanh toán', NULL, N'Giao dịch chuyển khoản đang chờ xác nhận.'),
(28, 17, 3, CAST('2026-11-13 14:20' AS DATETIME2(0)), CAST('2026-11-16 14:20' AS DATETIME2(0)), N'Chờ thanh toán', NULL, N'Khách đặt cọc một phần.'),
(29, 20, 5, CAST('2026-11-14 11:35' AS DATETIME2(0)), CAST('2026-11-17 11:35' AS DATETIME2(0)), N'Chờ thanh toán', NULL, NULL),
(30, 14, 3, CAST('2026-09-05 10:15' AS DATETIME2(0)), CAST('2026-09-08 10:15' AS DATETIME2(0)), N'Đã hủy', N'Khách đổi ý, không thanh toán trong hạn.', NULL),
(31, 17, 4, CAST('2026-09-20 14:50' AS DATETIME2(0)), CAST('2026-09-23 14:50' AS DATETIME2(0)), N'Đã hủy', N'Khách báo chọn nhầm kích thước.', NULL),
(32, 22, 5, CAST('2026-10-05 09:40' AS DATETIME2(0)), CAST('2026-10-08 09:40' AS DATETIME2(0)), N'Đã hủy', N'Khách không đến nhận hàng.', NULL);
SET IDENTITY_INSERT dbo.DON_HANG OFF;
SET IDENTITY_INSERT dbo.CHI_TIET_DON_HANG ON;
INSERT dbo.CHI_TIET_DON_HANG (MaCTDH, MaDH, MaSP, GiaNiemYetLucBan, GiamGia) VALUES
(1, 1, 1, 750000, 50000),
(2, 2, 2, 550000, 0),
(3, 2, 5, 250000, 0),
(4, 3, 4, 900000, 100000),
(5, 3, 6, 200000, 0),
(6, 4, 9, 300000, 0),
(7, 4, 12, 450000, 0),
(8, 5, 13, 500000, 0),
(9, 5, 15, 350000, 0),
(10, 6, 16, 300000, 0),
(11, 6, 21, 650000, 50000),
(12, 7, 17, 400000, 0),
(13, 7, 18, 450000, 0),
(14, 8, 10, 400000, 0),
(15, 8, 14, 2500000, 0),
(16, 9, 22, 1800000, 0),
(17, 9, 23, 200000, 0),
(18, 10, 25, 600000, 0),
(19, 10, 26, 350000, 0),
(20, 10, 30, 350000, 0),
(21, 11, 27, 650000, 0),
(22, 11, 28, 400000, 0),
(23, 12, 29, 850000, 0),
(24, 12, 32, 700000, 50000),
(25, 13, 33, 950000, 0),
(26, 13, 35, 500000, 0),
(27, 14, 36, 1500000, 0),
(28, 14, 37, 2200000, 0),
(29, 15, 38, 250000, 0),
(30, 15, 39, 600000, 0),
(31, 15, 40, 300000, 0),
(32, 16, 42, 400000, 0),
(33, 16, 43, 400000, 0),
(34, 17, 44, 220000, 0),
(35, 17, 45, 5500000, 0),
(36, 18, 46, 320000, 0),
(37, 18, 48, 850000, 0),
(38, 19, 49, 350000, 0),
(39, 19, 54, 550000, 0),
(40, 20, 31, 4500000, 0),
(41, 20, 55, 450000, 0),
(42, 21, 56, 600000, 0),
(43, 21, 57, 3000000, 0),
(44, 22, 58, 250000, 0),
(45, 23, 59, 1600000, 0),
(46, 23, 60, 200000, 0),
(47, 24, 53, 500000, 0),
(48, 25, 63, 400000, 0),
(49, 25, 64, 500000, 0),
(50, 26, 65, 1800000, 0),
(51, 27, 66, 650000, 0),
(52, 28, 67, 350000, 0),
(53, 28, 70, 6000000, 0),
(54, 29, 24, 150000, 0),
(55, 30, 24, 150000, 0),
(56, 30, 63, 400000, 0),
(57, 31, 50, 300000, 0),
(58, 32, 53, 500000, 0),
(59, 32, 51, 350000, 0);
SET IDENTITY_INSERT dbo.CHI_TIET_DON_HANG OFF;
SET IDENTITY_INSERT dbo.THANH_TOAN_DON_HANG ON;
INSERT dbo.THANH_TOAN_DON_HANG (MaTTDH, MaDH, SoTien, NgayTao, NgayHoanTat, PhuongThuc, TrangThai, MaGiaoDich, GhiChu) VALUES
(1, 1, 700000, CAST('2026-01-20 10:30' AS DATETIME2(0)), CAST('2026-01-20 10:30' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, N'Khách trả đủ khi nhận hàng.'),
(2, 2, 500000, CAST('2026-01-25 15:40' AS DATETIME2(0)), CAST('2026-01-25 15:40' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT260125001', N'Chuyển khoản trước.'),
(3, 2, 300000, CAST('2026-01-26 09:15' AS DATETIME2(0)), CAST('2026-01-26 09:15' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, N'Trả phần còn lại khi nhận hàng.'),
(4, 3, 1000000, CAST('2026-01-31 09:50' AS DATETIME2(0)), NULL, N'Chuyển khoản', N'Thất bại', 'FT260131009', N'Ngân hàng báo lỗi kết nối.'),
(5, 3, 1000000, CAST('2026-02-01 10:05' AS DATETIME2(0)), CAST('2026-02-01 10:05' AS DATETIME2(0)), N'Thẻ', N'Thành công', 'POS260201003', N'Quẹt thẻ tại cửa hàng.'),
(6, 4, 750000, CAST('2026-02-14 11:30' AS DATETIME2(0)), CAST('2026-02-14 11:30' AS DATETIME2(0)), N'Ví điện tử', N'Thành công', 'MOMO260214001', NULL),
(7, 5, 400000, CAST('2026-02-26 14:20' AS DATETIME2(0)), CAST('2026-02-26 14:20' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, NULL),
(8, 5, 450000, CAST('2026-02-27 10:00' AS DATETIME2(0)), CAST('2026-02-27 10:00' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT260227004', NULL),
(9, 6, 900000, CAST('2026-03-15 10:15' AS DATETIME2(0)), CAST('2026-03-15 10:15' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT260315002', NULL),
(10, 7, 850000, CAST('2026-03-19 16:30' AS DATETIME2(0)), NULL, N'Thẻ', N'Thất bại', 'POS260319001', N'Máy POS mất kết nối.'),
(11, 7, 850000, CAST('2026-03-20 09:20' AS DATETIME2(0)), CAST('2026-03-20 09:20' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT260320003', N'Khách chuyển khoản lại.'),
(12, 8, 2000000, CAST('2026-03-30 09:45' AS DATETIME2(0)), CAST('2026-03-30 09:45' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT260330005', NULL),
(13, 8, 900000, CAST('2026-03-31 15:10' AS DATETIME2(0)), CAST('2026-03-31 15:10' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, NULL),
(14, 9, 2000000, CAST('2026-04-10 15:00' AS DATETIME2(0)), CAST('2026-04-10 15:00' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, NULL),
(15, 10, 500000, CAST('2026-04-22 10:40' AS DATETIME2(0)), CAST('2026-04-22 10:40' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, NULL),
(16, 10, 500000, CAST('2026-04-22 16:00' AS DATETIME2(0)), CAST('2026-04-22 16:00' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT260422006', NULL),
(17, 10, 300000, CAST('2026-04-23 09:10' AS DATETIME2(0)), CAST('2026-04-23 09:10' AS DATETIME2(0)), N'Ví điện tử', N'Thành công', 'MOMO260423002', NULL),
(18, 11, 1050000, CAST('2026-05-05 15:25' AS DATETIME2(0)), CAST('2026-05-05 15:25' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT260505007', NULL),
(19, 12, 1500000, CAST('2026-05-17 11:55' AS DATETIME2(0)), NULL, N'Ví điện tử', N'Thất bại', 'MOMO260517003', N'Ví hết hạn mức.'),
(20, 12, 1000000, CAST('2026-05-18 09:30' AS DATETIME2(0)), CAST('2026-05-18 09:30' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT260518008', NULL),
(21, 12, 500000, CAST('2026-05-18 16:45' AS DATETIME2(0)), CAST('2026-05-18 16:45' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, NULL),
(22, 13, 1450000, CAST('2026-05-29 09:35' AS DATETIME2(0)), CAST('2026-05-29 09:35' AS DATETIME2(0)), N'Thẻ', N'Thành công', 'POS260529002', NULL),
(23, 14, 2000000, CAST('2026-06-10 14:50' AS DATETIME2(0)), CAST('2026-06-10 14:50' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT260610009', NULL),
(24, 14, 1700000, CAST('2026-06-11 10:20' AS DATETIME2(0)), CAST('2026-06-11 10:20' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT260611010', NULL),
(25, 15, 1150000, CAST('2026-06-21 10:20' AS DATETIME2(0)), CAST('2026-06-21 10:20' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, NULL),
(26, 16, 500000, CAST('2026-07-03 16:05' AS DATETIME2(0)), CAST('2026-07-03 16:05' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, NULL),
(27, 16, 300000, CAST('2026-07-04 09:15' AS DATETIME2(0)), CAST('2026-07-04 09:15' AS DATETIME2(0)), N'Ví điện tử', N'Thành công', 'MOMO260704004', NULL),
(28, 17, 5720000, CAST('2026-07-15 11:40' AS DATETIME2(0)), CAST('2026-07-15 11:40' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT260715011', NULL),
(29, 18, 1170000, CAST('2026-08-02 09:55' AS DATETIME2(0)), NULL, N'Ví điện tử', N'Thất bại', 'MOMO260802005', N'Xác thực OTP thất bại.'),
(30, 18, 700000, CAST('2026-08-03 10:10' AS DATETIME2(0)), CAST('2026-08-03 10:10' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT260803012', NULL),
(31, 18, 470000, CAST('2026-08-03 15:30' AS DATETIME2(0)), CAST('2026-08-03 15:30' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, NULL),
(32, 19, 900000, CAST('2026-08-20 14:35' AS DATETIME2(0)), CAST('2026-08-20 14:35' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, NULL),
(33, 30, 550000, CAST('2026-09-05 10:30' AS DATETIME2(0)), NULL, N'Chuyển khoản', N'Thất bại', 'FT260905020', N'Khách chuyển thiếu, giao dịch bị hủy.'),
(34, 20, 3000000, CAST('2026-09-12 10:10' AS DATETIME2(0)), CAST('2026-09-12 10:10' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT260912013', NULL),
(35, 20, 2000000, CAST('2026-09-12 15:20' AS DATETIME2(0)), CAST('2026-09-12 15:20' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, N'Khách trả thừa 50.000đ, đã ghi nhận đối soát.'),
(36, 21, 2000000, CAST('2026-10-10 15:30' AS DATETIME2(0)), CAST('2026-10-10 15:30' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT261010014', NULL),
(37, 21, 1600000, CAST('2026-10-10 16:10' AS DATETIME2(0)), CAST('2026-10-10 16:10' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, NULL),
(38, 22, 250000, CAST('2026-10-18 10:00' AS DATETIME2(0)), CAST('2026-10-18 10:00' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, NULL),
(39, 23, 1000000, CAST('2026-10-25 14:45' AS DATETIME2(0)), CAST('2026-10-25 14:45' AS DATETIME2(0)), N'Chuyển khoản', N'Thành công', 'FT261025015', NULL),
(40, 23, 800000, CAST('2026-10-25 15:20' AS DATETIME2(0)), CAST('2026-10-25 15:20' AS DATETIME2(0)), N'Thẻ', N'Thành công', 'POS261025003', NULL),
(41, 24, 500000, CAST('2026-11-01 10:35' AS DATETIME2(0)), CAST('2026-11-01 10:35' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, NULL),
(42, 25, 900000, CAST('2026-11-10 10:00' AS DATETIME2(0)), NULL, N'Chuyển khoản', N'Thất bại', 'FT261110016', N'Tài khoản không đủ số dư.'),
(43, 26, 500000, CAST('2026-11-11 15:20' AS DATETIME2(0)), NULL, N'Thẻ', N'Thất bại', 'POS261111004', N'Thẻ bị từ chối.'),
(44, 27, 650000, CAST('2026-11-12 10:55' AS DATETIME2(0)), NULL, N'Thẻ', N'Thất bại', 'POS261112005', N'Giao dịch bị treo.'),
(45, 27, 650000, CAST('2026-11-12 11:10' AS DATETIME2(0)), NULL, N'Chuyển khoản', N'Chờ xử lý', 'FT261112017', N'Đang chờ ngân hàng xác nhận.'),
(46, 28, 3000000, CAST('2026-11-13 14:30' AS DATETIME2(0)), CAST('2026-11-13 14:30' AS DATETIME2(0)), N'Tiền mặt', N'Thành công', NULL, N'Khách tạm ứng trước.'),
(47, 28, 1000000, CAST('2026-11-13 15:00' AS DATETIME2(0)), NULL, N'Chuyển khoản', N'Thất bại', 'FT261113018', N'Sai nội dung chuyển khoản.'),
(48, 29, 150000, CAST('2026-11-14 11:45' AS DATETIME2(0)), NULL, N'Chuyển khoản', N'Thất bại', 'FT261114019', N'Khách hủy giao dịch.');
SET IDENTITY_INSERT dbo.THANH_TOAN_DON_HANG OFF;
SET IDENTITY_INSERT dbo.PHIEU_THANH_TOAN_KY_GUI ON;
INSERT dbo.PHIEU_THANH_TOAN_KY_GUI (MaPTTKG, MaKH, MaNV, NgayLap, NgayThanhToan, PhuongThuc, TrangThai, MaGiaoDich, GhiChu) VALUES
(1, 1, 2, CAST('2026-01-30 09:30' AS DATETIME2(0)), CAST('2026-01-31 10:30' AS DATETIME2(0)), N'Tiền mặt', N'Đã thanh toán', 'PC260130001', N'Quyết toán đợt 1 cho khách Lan.'),
(2, 2, 2, CAST('2026-02-10 14:20' AS DATETIME2(0)), CAST('2026-02-11 15:20' AS DATETIME2(0)), N'Chuyển khoản', N'Đã thanh toán', 'PC260210002', NULL),
(3, 4, 6, CAST('2026-03-10 10:15' AS DATETIME2(0)), CAST('2026-03-11 11:15' AS DATETIME2(0)), N'Chuyển khoản', N'Đã thanh toán', 'PC260310003', N'Khách nhận qua tài khoản.'),
(4, 5, 2, CAST('2026-04-05 15:40' AS DATETIME2(0)), CAST('2026-04-06 16:40' AS DATETIME2(0)), N'Chuyển khoản', N'Đã thanh toán', 'PC260405004', NULL),
(5, 6, 6, CAST('2026-04-12 11:05' AS DATETIME2(0)), CAST('2026-04-13 12:05' AS DATETIME2(0)), N'Tiền mặt', N'Đã thanh toán', 'PC260412005', NULL),
(6, 7, 2, CAST('2026-04-20 09:50' AS DATETIME2(0)), CAST('2026-04-21 10:50' AS DATETIME2(0)), N'Chuyển khoản', N'Đã thanh toán', 'PC260420006', N'Một sản phẩm chưa quyết toán.'),
(7, 9, 2, CAST('2026-05-20 16:30' AS DATETIME2(0)), CAST('2026-05-21 17:30' AS DATETIME2(0)), N'Tiền mặt', N'Đã thanh toán', 'PC260520007', NULL),
(8, 8, 6, CAST('2026-06-15 10:40' AS DATETIME2(0)), CAST('2026-06-16 11:40' AS DATETIME2(0)), N'Chuyển khoản', N'Đã thanh toán', 'PC260615008', NULL),
(9, 10, 2, CAST('2026-06-25 14:10' AS DATETIME2(0)), CAST('2026-06-26 15:10' AS DATETIME2(0)), N'Chuyển khoản', N'Đã thanh toán', 'PC260625009', NULL),
(10, 11, 2, CAST('2026-07-15 09:25' AS DATETIME2(0)), CAST('2026-07-16 10:25' AS DATETIME2(0)), N'Chuyển khoản', N'Đã thanh toán', 'PC260715010', N'Đợt 1, còn một sản phẩm chờ xử lý.'),
(11, 12, 6, CAST('2026-09-20 15:35' AS DATETIME2(0)), NULL, N'Chuyển khoản', N'Chờ thanh toán', NULL, N'Khách yêu cầu gộp đợt sau.'),
(12, 13, 2, CAST('2026-08-15 11:20' AS DATETIME2(0)), CAST('2026-08-16 12:20' AS DATETIME2(0)), N'Tiền mặt', N'Đã thanh toán', 'PC260815012', NULL),
(13, 14, 1, CAST('2026-09-28 10:05' AS DATETIME2(0)), CAST('2026-09-29 11:05' AS DATETIME2(0)), N'Chuyển khoản', N'Đã thanh toán', 'PC260928013', N'Quản lý duyệt chi.'),
(14, 15, 2, CAST('2026-09-30 16:15' AS DATETIME2(0)), NULL, N'Tiền mặt', N'Chờ thanh toán', NULL, N'Hẹn khách nhận tiền mặt.'),
(15, 16, 6, CAST('2026-10-10 09:45' AS DATETIME2(0)), NULL, N'Chuyển khoản', N'Chờ thanh toán', NULL, NULL),
(16, 17, 2, CAST('2026-10-20 14:50' AS DATETIME2(0)), CAST('2026-10-21 15:50' AS DATETIME2(0)), N'Tiền mặt', N'Đã thanh toán', 'PC261020016', NULL),
(17, 18, 6, CAST('2026-10-30 10:30' AS DATETIME2(0)), NULL, N'Chuyển khoản', N'Chờ thanh toán', NULL, N'Khách chưa cung cấp số tài khoản.'),
(18, 19, 2, CAST('2026-11-05 15:00' AS DATETIME2(0)), NULL, N'Tiền mặt', N'Chờ thanh toán', NULL, N'Chờ khách đến nhận.');
SET IDENTITY_INSERT dbo.PHIEU_THANH_TOAN_KY_GUI OFF;
SET IDENTITY_INSERT dbo.CHI_TIET_THANH_TOAN_KY_GUI ON;
INSERT dbo.CHI_TIET_THANH_TOAN_KY_GUI (MaCTTTKG, MaPTTKG, MaCTDH, GiaBanTinhToan) VALUES
(1, 1, 1, 700000),
(2, 1, 2, 550000),
(3, 2, 3, 250000),
(4, 2, 4, 800000),
(5, 2, 5, 200000),
(6, 3, 6, 300000),
(7, 4, 7, 450000),
(8, 4, 8, 500000),
(9, 5, 9, 350000),
(10, 5, 10, 300000),
(11, 6, 12, 400000),
(12, 7, 16, 1800000),
(13, 7, 17, 200000),
(14, 8, 11, 600000),
(15, 9, 18, 600000),
(16, 9, 19, 350000),
(17, 10, 21, 650000),
(18, 10, 22, 400000),
(19, 11, 20, 350000),
(20, 12, 24, 650000),
(21, 12, 25, 950000),
(22, 13, 26, 500000),
(23, 13, 27, 1500000),
(24, 14, 29, 250000),
(25, 14, 30, 600000),
(26, 15, 31, 300000),
(27, 16, 33, 400000),
(28, 16, 34, 220000),
(29, 17, 35, 5500000),
(30, 18, 37, 850000);
SET IDENTITY_INSERT dbo.CHI_TIET_THANH_TOAN_KY_GUI OFF;
COMMIT TRANSACTION;
GO
ALTER TABLE dbo.CHI_TIET_THANH_TOAN_KY_GUI ENABLE TRIGGER ALL;
ALTER TABLE dbo.PHIEU_THANH_TOAN_KY_GUI ENABLE TRIGGER ALL;
ALTER TABLE dbo.THANH_TOAN_DON_HANG ENABLE TRIGGER ALL;
ALTER TABLE dbo.CHI_TIET_DON_HANG ENABLE TRIGGER ALL;
ALTER TABLE dbo.DON_HANG ENABLE TRIGGER ALL;
ALTER TABLE dbo.SAN_PHAM ENABLE TRIGGER ALL;
GO
UPDATE p
SET TrangThai = dbo.fn_TrangThaiPhieuKyGui(p.MaPKG)
FROM dbo.PHIEU_KY_GUI p
WHERE p.TrangThai <> dbo.fn_TrangThaiPhieuKyGui(p.MaPKG);
GO
PRINT N'Seed operations: 28 phieu, 72 san pham, 32 don, 48 giao dich, 18 phieu quyet toan, 30 chi tiet.';
GO