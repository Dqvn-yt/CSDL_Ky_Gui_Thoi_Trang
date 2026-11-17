# Phân tích chuẩn hóa 3NF — CSDL QuanLyKyGuiThoiTrang

Tài liệu này trình bày việc kiểm tra và chỉnh sửa lược đồ quan hệ của hệ thống
**Quản lý sàn ký gửi đồ thời trang cũ** để đạt **dạng chuẩn 3 (3NF)**.

- Hệ quản trị: Microsoft SQL Server 2025
- Collation: `Vietnamese_CI_AS`
- Kiểu tiền: `DECIMAL(12,0)` (đơn vị đồng), ngày giờ: `DATETIME2(0)`
- Khóa chính: `INT IDENTITY(1,1)`

---

## 1. Cơ sở lý thuyết áp dụng

Một quan hệ đạt 3NF khi:

1. **1NF** — mọi thuộc tính là nguyên tố (không có nhóm lặp, không có thuộc tính đa trị).
2. **2NF** — đạt 1NF và không có phụ thuộc hàm bộ phận vào một phần của khóa chính.
3. **3NF** — đạt 2NF và không có phụ thuộc hàm bắc cầu: nếu `X → Y` và `Y → Z`
   với `Y` không là khóa thì `X → Z` không được tồn tại dưới dạng thuộc tính lưu trữ độc lập.

Do toàn bộ bảng dùng **khóa chính đơn (surrogate key)** `INT IDENTITY`, điều kiện 2NF
được thỏa hiển nhiên: không thể có phụ thuộc bộ phận vì khóa chỉ có một thuộc tính.
Trọng tâm kiểm tra vì vậy là **1NF** và **3NF**.

---

## 2. Phụ thuộc hàm của từng quan hệ

Ký hiệu: `→` là phụ thuộc hàm; gạch chân là khóa chính; `UQ` là khóa dự tuyển.

### 2.1 KHACH_HANG
```
MaKH → HoTen, SoDienThoai, Email, DiaChi, NgayTao
SoDienThoai → MaKH            (UQ, khóa dự tuyển)
```
Không có thuộc tính không khóa nào xác định thuộc tính không khóa khác → **3NF**.

### 2.2 NHAN_VIEN
```
MaNV → HoTen, SoDienThoai, Email, ChucVu, TrangThai
```
`ChucVu`, `TrangThai` là thuộc tính trạng thái của nhân viên, không phụ thuộc bắc cầu → **3NF**.

### 2.3 THUONG_HIEU
```
MaTH → TenThuongHieu, MoTa
TenThuongHieu → MaTH          (UQ, khóa dự tuyển)
```
→ **3NF**.

### 2.4 DANH_MUC
```
MaDM → TenDanhMuc, MoTa
TenDanhMuc → MaDM             (UQ, khóa dự tuyển)
```
→ **3NF**.

### 2.5 PHIEU_KY_GUI
```
MaPKG → MaKH, MaNV, NgayKyGui, TrangThai, GhiChu
```
`TrangThai` là **trạng thái nghiệp vụ** của phiếu, được suy ra từ tập sản phẩm
thuộc phiếu (`fn_TrangThaiPhieuKyGui`) và được đồng bộ bằng trigger. Đây là
**dư thừa có kiểm soát** — xem mục 4.2.

### 2.6 SAN_PHAM
```
MaSP → MaPKG, MaDM, MaTH, TenSP, MoTa, KichThuoc, MauSac, ChatLieu,
       TinhTrang, GiaDeXuat, GiaDangBan, TrangThaiTiepNhan, TrangThaiBan, NgayDangBan
```
Không có phụ thuộc bắc cầu giữa các thuộc tính không khóa → **3NF**.

### 2.7 DON_HANG
```
MaDH → MaKH, MaNV, NgayLap, HanThanhToan, TrangThai, LyDoHuy, GhiChu
```
Tổng tiền đơn **không lưu trữ** trong bảng mà tính qua `vw_DonHang_TongHop`
(`SUM(CHI_TIET_DON_HANG.GiaBanThucTe)`) → không phát sinh phụ thuộc bắc cầu → **3NF**.

### 2.8 CHI_TIET_DON_HANG
```
MaCTDH → MaDH, MaSP, GiaNiemYetLucBan, GiamGia
(MaDH, MaSP) → MaCTDH         (UQ)
GiaNiemYetLucBan, GiamGia → GiaBanThucTe     ← phụ thuộc bắc cầu nếu lưu thường
```
**Điểm sửa 3NF #1:** `GiaBanThucTe` phụ thuộc hàm vào hai thuộc tính không khóa,
nếu lưu như cột thường sẽ vi phạm 3NF và gây dị thường cập nhật (sửa `GiamGia`
mà quên sửa thành tiền). Giải pháp: khai báo **cột tính toán PERSISTED**:

```sql
GiaBanThucTe AS (CAST(GiaNiemYetLucBan - GiamGia AS DECIMAL(12,0))) PERSISTED
```

SQL Server tự tính lại khi cập nhật, không thể ghi sai, do đó **loại bỏ dị thường**
và quan hệ trở lại 3NF về mặt lưu trữ logic.

### 2.9 THANH_TOAN_DON_HANG
```
MaTTDH → MaDH, SoTien, NgayTao, NgayHoanTat, PhuongThuc, TrangThai, MaGiaoDich, GhiChu
```
`NgayHoanTat` chỉ có giá trị khi `TrangThai = N'Thành công'` — ràng buộc này được
bảo đảm bằng `CK_THANH_TOAN_DON_HANG_NgayHoanTat`, là phụ thuộc có điều kiện chứ
không phải bắc cầu → **3NF**.

### 2.10 PHIEU_THANH_TOAN_KY_GUI
```
MaPTTKG → MaKH, MaNV, NgayLap, NgayThanhToan, PhuongThuc, TrangThai, MaGiaoDich, GhiChu
```
→ **3NF**.

### 2.11 CHI_TIET_THANH_TOAN_KY_GUI
```
MaCTTTKG → MaPTTKG, MaCTDH, GiaBanTinhToan
MaCTDH → MaCTTTKG             (UQ: mỗi chi tiết đơn chỉ quyết toán một lần)
GiaBanTinhToan → TienHoaHong, TienThucNhan   ← phụ thuộc bắc cầu nếu lưu thường
```
**Điểm sửa 3NF #2:** hoa hồng cố định 20% và tiền thực nhận là hàm của giá bán.
Khai báo cột tính toán:

```sql
TienHoaHong  AS (CAST(ROUND(GiaBanTinhToan * 0.20, 0) AS DECIMAL(12,0))) PERSISTED,
TienThucNhan AS (CAST(GiaBanTinhToan - ROUND(GiaBanTinhToan * 0.20, 0) AS DECIMAL(12,0))) PERSISTED
```

Nhờ vậy giá bán, hoa hồng và tiền thực nhận luôn nhất quán, không có dị thường
cập nhật — đạt 3NF.

---

## 3. Kiểm tra 1NF và 2NF

| Quan hệ | 1NF | 2NF | Ghi chú |
|---|---|---|---|
| KHACH_HANG | Đạt | Đạt | Thuộc tính nguyên tố |
| NHAN_VIEN | Đạt | Đạt | |
| THUONG_HIEU | Đạt | Đạt | |
| DANH_MUC | Đạt | Đạt | |
| PHIEU_KY_GUI | Đạt | Đạt | |
| SAN_PHAM | Đạt | Đạt | |
| DON_HANG | Đạt | Đạt | Không lưu tổng tiền |
| CHI_TIET_DON_HANG | Đạt | Đạt | Tách dòng hàng khỏi đơn (chống nhóm lặp) |
| THANH_TOAN_DON_HANG | Đạt | Đạt | Nhiều giao dịch/đơn tách bảng riêng |
| PHIEU_THANH_TOAN_KY_GUI | Đạt | Đạt | |
| CHI_TIET_THANH_TOAN_KY_GUI | Đạt | Đạt | Tách chi tiết quyết toán |

**Ví dụ chống nhóm lặp:** nếu lưu danh sách sản phẩm của đơn hàng ngay trong
`DON_HANG` dưới dạng chuỗi, sẽ vi phạm 1NF. Hệ thống tách thành
`CHI_TIET_DON_HANG` với khóa ngoại `MaDH`, mỗi dòng là một sản phẩm — đạt 1NF.
Tương tự, một đơn có nhiều lần thanh toán được tách sang `THANH_TOAN_DON_HANG`.

---

## 4. Các điểm cần xử lý và cách sửa

### 4.1 Bảng tổng hợp

| # | Vấn đề tiềm ẩn | Vi phạm | Cách xử lý |
|---|---|---|---|
| 1 | Thành tiền dòng hàng = giá niêm yết − giảm giá | 3NF (bắc cầu) | Cột tính toán `GiaBanThucTe` PERSISTED |
| 2 | Hoa hồng = 20% giá bán, tiền thực nhận = giá − hoa hồng | 3NF (bắc cầu) | Hai cột tính toán PERSISTED |
| 3 | Tổng tiền đơn hàng | 3NF nếu lưu | Không lưu; tính trong view `vw_DonHang_TongHop` |
| 4 | Trạng thái phiếu ký gửi suy ra từ trạng thái sản phẩm | Dư thừa có kiểm soát | Hàm `fn_TrangThaiPhieuKyGui` + trigger đồng bộ |
| 5 | Trạng thái bán của sản phẩm lặp theo đơn hàng | Không vi phạm | Là thuộc tính trạng thái của chính sản phẩm, cập nhật bằng trigger |

### 4.2 Vì sao vẫn giữ `PHIEU_KY_GUI.TrangThai`

Về lý thuyết, `TrangThai` của phiếu có thể suy ra hoàn toàn từ các sản phẩm thuộc
phiếu. Tuy nhiên:

- Đây là **trạng thái nghiệp vụ** của thực thể phiếu (đang xử lý / đang ký gửi /
  hoàn tất / đã hủy), không phải thuộc tính mô tả của bản ghi khác.
- Nếu tính lại mỗi lần hiển thị sẽ phải quét toàn bộ `SAN_PHAM` và `DON_HANG`,
  ảnh hưởng hiệu năng danh sách phiếu.
- Hệ thống **bảo đảm nhất quán bằng trigger**: mọi thay đổi trên `SAN_PHAM`,
  `CHI_TIET_DON_HANG`, `DON_HANG` đều gọi `fn_TrangThaiPhieuKyGui` và cập nhật lại
  khi khác biệt. Vì vậy không có dị thường dữ liệu.
- Truy vấn đối chiếu `07_validation.sql` kiểm tra 18 bất biến, trong đó có kiểm tra
  trạng thái phiếu khớp với trạng thái sản phẩm.

Kết luận: chấp nhận dư thừa có kiểm soát, đã được tài liệu hóa; lược đồ vẫn đạt
3NF về mặt thiết kế lưu trữ (mọi thuộc tính không khóa phụ thuộc trực tiếp vào khóa).

---

## 5. Kết luận

- 11 quan hệ đều đạt **1NF, 2NF, 3NF**.
- Ba cột tính toán `GiaBanThucTe`, `TienHoaHong`, `TienThucNhan` là giải pháp
  chuẩn hóa cho các phụ thuộc bắc cầu về giá trị tiền.
- Tổng tiền đơn hàng, công nợ, doanh thu **không lưu trữ** mà tính qua view,
  tránh mọi dị thường cập nhật.
- Ràng buộc toàn vẹn được thực thi ở ba tầng: `CHECK`/`UNIQUE`/`FOREIGN KEY`,
  stored procedure, và trigger — bảo đảm dữ liệu luôn đúng ngay cả khi thao tác
  ngoài ứng dụng.
