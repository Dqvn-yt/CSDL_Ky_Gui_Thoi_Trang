# -*- coding: utf-8 -*-
"""Du lieu goc cho seed QuanLyKyGuiThoiTrang."""

EMPLOYEES = [
    (1, "Nguyễn Văn Hùng", "0912345001", "hung.nguyen@thienkimconsign.vn", "Quản lý", "Đang làm việc"),
    (2, "Trần Thị Mai", "0912345002", "mai.tran@thienkimconsign.vn", "Thu ngân", "Đang làm việc"),
    (3, "Lê Quốc Bảo", "0912345003", "bao.le@thienkimconsign.vn", "Nhân viên", "Đang làm việc"),
    (4, "Phạm Thu Trang", "0912345004", "trang.pham@thienkimconsign.vn", "Nhân viên", "Đang làm việc"),
    (5, "Hoàng Minh Tuấn", "0912345005", "tuan.hoang@thienkimconsign.vn", "Nhân viên", "Đang làm việc"),
    (6, "Đỗ Thị Hồng", "0912345006", "hong.do@thienkimconsign.vn", "Thu ngân", "Đang làm việc"),
    (7, "Vũ Đức Thắng", "0912345007", None, "Nhân viên", "Đã nghỉ"),
    (8, "Ngô Thanh Hà", "0912345008", "ha.ngo@thienkimconsign.vn", "Nhân viên", "Đã nghỉ"),
]

CUSTOMERS = [
    (1, "Nguyễn Thị Lan", "0901000001", "lan.nguyen@gmail.com", "12 Ngõ 45 Cầu Giấy, Hà Nội"),
    (2, "Trần Văn Minh", "0901000002", "minh.tran@gmail.com", "78 Lê Thanh Nghị, Hà Nội"),
    (3, "Lê Thu Hà", "0901000003", "ha.le@gmail.com", "25 Nguyễn Chí Thanh, Hà Nội"),
    (4, "Phạm Quốc Bảo", "0901000004", None, "102 Hoàng Diệu, Đà Nẵng"),
    (5, "Hoàng Thị Ngọc", "0901000005", "ngoc.hoang@gmail.com", "33 Lý Thường Kiệt, Hà Nội"),
    (6, "Đặng Thùy Linh", "0901000006", "linh.dang@gmail.com", "56 Điện Biên Phủ, TP. Hồ Chí Minh"),
    (7, "Bùi Minh Khoa", "0901000007", "khoa.bui@gmail.com", "89 Cách Mạng Tháng 8, TP. Hồ Chí Minh"),
    (8, "Vũ Đức Anh", "0901000008", "anh.vu@gmail.com", "14 Phan Đình Phùng, Đà Nẵng"),
    (9, "Đinh Công Sơn", "0901000009", None, "67 Trần Hưng Đạo, Hà Nội"),
    (10, "Nguyễn Mai Phương", "0901000010", "phuong.nguyen@gmail.com", "21 Nguyễn Văn Cừ, Hà Nội"),
    (11, "Trần Hoàng Nam", "0901000011", "nam.tran@gmail.com", "45 Bạch Đằng, Đà Nẵng"),
    (12, "Lê Thị Kim Oanh", "0901000012", "oanh.le@gmail.com", "73 Hai Bà Trưng, TP. Hồ Chí Minh"),
    (13, "Phạm Đức Huy", "0901000013", "huy.pham@gmail.com", "8 Tôn Đức Thắng, Hà Nội"),
    (14, "Hoàng Lan Anh", "0901000014", "lananh.hoang@gmail.com", "19 Nguyễn Huệ, TP. Hồ Chí Minh"),
    (15, "Nguyễn Quốc Trung", "0901000015", "trung.nguyen@gmail.com", "91 Đống Đa, Đà Nẵng"),
    (16, "Trần Thị Bích", "0901000016", "bich.tran@gmail.com", "37 Kim Mã, Hà Nội"),
    (17, "Lê Văn Tùng", "0901000017", "tung.le@gmail.com", "64 Nguyễn Trãi, Hà Nội"),
    (18, "Phạm Thị Hương", "0901000018", None, "15 Lê Lợi, Đà Nẵng"),
    (19, "Hoàng Văn Sơn", "0901000019", "son.hoang@gmail.com", "82 Pasteur, TP. Hồ Chí Minh"),
    (20, "Nguyễn Thị Thu", "0901000020", "thu.nguyen@gmail.com", "29 Quang Trung, Hà Nội"),
    (21, "Trần Minh Đức", "0901000021", "duc.tran@gmail.com", "53 Ngô Quyền, Hà Nội"),
    (22, "Lê Thị Ngân", "0901000022", "ngan.le@gmail.com", "41 Trần Phú, Đà Nẵng"),
    (23, "Phạm Văn Long", "0901000023", "long.pham@gmail.com", "76 Võ Văn Tần, TP. Hồ Chí Minh"),
    (24, "Hoàng Thu Hằng", "0901000024", None, None),
]

BRANDS = [
    (1, "Nike", "Thương hiệu thể thao Mỹ, phổ biến với giày chạy và đồ thể thao."),
    (2, "Adidas", "Thương hiệu thể thao Đức, mạnh về giày sneaker và áo khoác."),
    (3, "Zara", "Thương hiệu thời trang nhanh Tây Ban Nha, nhiều mẫu áo khoác và đầm."),
    (4, "Uniqlo", "Thương hiệu Nhật Bản, chú trọng đồ basic và chất liệu thoải mái."),
    (5, "H&M", "Thương hiệu thời trang bình dân Thụy Điển."),
    (6, "Levi's", "Thương hiệu jeans lâu đời của Mỹ."),
    (7, "Gucci", "Nhà mốt Ý với túi xách, giày và phụ kiện da cao cấp."),
    (8, "Chanel", "Nhà mốt Pháp nổi tiếng với túi flap và trang sức."),
    (9, "Louis Vuitton", "Nhà mốt Pháp với túi monogram đặc trưng."),
    (10, "Mango", "Thương hiệu thời trang Tây Ban Nha, thiên về công sở và thanh lịch."),
]

CATEGORIES = [
    (1, "Quần áo", "Áo, quần, váy, đầm và các loại trang phục."),
    (2, "Giày dép", "Giày sneaker, cao gót, boots, sandal."),
    (3, "Mũ nón", "Mũ lưỡi trai, bucket, beret và các loại mũ."),
    (4, "Túi xách", "Túi xách, túi đeo chéo, balo, ví."),
    (5, "Trang sức", "Nhẫn, dây chuyền, bông tai, lắc tay."),
    (6, "Phụ kiện", "Kính râm, thắt lưng, khăn và phụ kiện nhỏ."),
]

# (MaPKG, ngay, MaKH, MaNV, ghi chu)
SLIPS = [
    (1, "2026-01-05 09:15", 1, 3, "Khách ký gửi lần đầu, đã kiểm tra cùng khách."),
    (2, "2026-01-12 14:20", 2, 4, "Nhận 3 món, hẹn duyệt trong ngày."),
    (3, "2026-01-20 10:05", 3, 7, "Khách mang váy và sandal cũ, cần kiểm định kỹ."),
    (4, "2026-02-02 15:40", 4, 5, "Khách muốn giữ lại kính râm để cân nhắc."),
    (5, "2026-02-10 11:10", 5, 4, "Túi Gucci có kèm túi chống bụi."),
    (6, "2026-02-18 16:25", 6, 3, "Khách ký gửi đầm và nhẫn bạc."),
    (7, "2026-03-02 09:50", 7, 5, "Thắt lưng da bị xước nhẹ, khách đồng ý giá thấp."),
    (8, "2026-03-11 13:35", 8, 4, "Boots size 39, khách đi ít."),
    (9, "2026-03-20 10:45", 9, 3, "Giày Gucci còn hộp và túi vải."),
    (10, "2026-04-01 15:05", 10, 8, "Khách quen, ký gửi túi và dây chuyền."),
    (11, "2026-04-09 09:30", 11, 4, "Áo khoác Adidas và giày chạy bộ."),
    (12, "2026-04-17 14:55", 12, 3, "Túi LV có hóa đơn mua hàng."),
    (13, "2026-04-26 11:20", 13, 5, "Mũ bucket không đạt, sẽ từ chối."),
    (14, "2026-05-05 16:10", 14, 4, "Trang sức Chanel còn hộp."),
    (15, "2026-05-14 09:40", 15, 3, "Khách ký gửi đồ công sở."),
    (16, "2026-05-22 14:15", 16, 5, "Giày cao gót bị xước gót."),
    (17, "2026-06-03 10:30", 17, 4, "Áo khoác và mũ thể thao."),
    (18, "2026-06-12 15:50", 18, 3, "Túi Chanel và áo sơ mi."),
    (19, "2026-06-21 11:05", 19, 5, "Giày Samba size 42."),
    (20, "2026-07-02 09:20", 20, 4, "Kính râm gọng đồi mồi bị trầy."),
    (21, "2026-07-11 16:35", 21, 3, "Blazer Uniqlo và túi Mango."),
    (22, "2026-07-20 10:15", 22, 5, "Dây chuyền Chanel còn hộp."),
    (23, "2026-08-03 14:45", 23, 4, "Giày Gucci và mũ beret."),
    (24, "2026-08-12 09:55", 1, 3, "Khách ký gửi thêm túi và áo thun."),
    (25, "2026-08-21 15:25", 24, 5, "Khách mới, ký gửi váy và giày sandal."),
    (26, "2026-09-04 10:40", 2, 4, "Áo khoác Zara và jeans H&M."),
    (27, "2026-09-18 14:05", 3, 3, "Khách quay lại ký gửi sau lần trước."),
    (28, "2026-11-06 10:50", 5, 5, "Hai món hàng hiệu đang chờ kiểm định."),
]
