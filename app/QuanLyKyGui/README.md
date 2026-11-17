# Hệ thống quản lý sàn ký gửi đồ thời trang cũ

Đồ án môn **Cơ sở dữ liệu** — CSDL `QuanLyKyGuiThoiTrang` trên Microsoft SQL Server 2025,
ứng dụng minh họa C# WinForms (.NET 9), báo cáo Word và các tệp Excel xuất từ ứng dụng.

```
E:\Quanap
├─ db\        Script T-SQL: tạo CSDL, bảng, trigger, view, procedure, dữ liệu mẫu, kiểm thử
├─ bak\       Bản sao lưu .bak đã kiểm tra
├─ app\       Project WinForms QuanLyKyGui
├─ docs\      ERD (draw.io + PNG), phân tích 3NF, báo cáo Word
├─ screenshots\ 6 ảnh chụp màn hình minh họa
├─ exports\   3 tệp Excel xuất từ ứng dụng
└─ tools\     Script sinh dữ liệu mẫu, ERD và báo cáo
```

## 1. Yêu cầu môi trường

| Thành phần | Phiên bản |
|---|---|
| SQL Server | 2025 (bản .bak định dạng 17.0, chỉ khôi phục được trên 2025+) |
| .NET SDK | 9.0 (Windows) |
| Microsoft Word | Office 2016 trở lên (chỉ cần khi tạo lại báo cáo) |
| Python | 3.10+ kèm matplotlib (chỉ cần khi sinh lại ERD / dữ liệu mẫu) |

Chuỗi kết nối mặc định của ứng dụng:

```
Server=localhost;Database=QuanLyKyGuiThoiTrang;Integrated Security=true;TrustServerCertificate=true;Encrypt=false;
```

## 2. Cài đặt cơ sở dữ liệu

### Cách A — Chạy toàn bộ script (từ đầu)

Mở Command Prompt tại `E:\Quanap` và chạy lần lượt:

```bat
sqlcmd -S localhost -E -C -I -f 65001 -i db\00_create_database.sql
sqlcmd -S localhost -E -C -I -f 65001 -i db\01_schema.sql
sqlcmd -S localhost -E -C -I -f 65001 -i db\02_triggers.sql
sqlcmd -S localhost -E -C -I -f 65001 -i db\03_views.sql
sqlcmd -S localhost -E -C -I -f 65001 -i db\04_procedures.sql
sqlcmd -S localhost -E -C -I -f 65001 -i db\05_seed_master.sql
sqlcmd -S localhost -E -C -I -f 65001 -i db\06_seed_operations.sql
sqlcmd -S localhost -E -C -I -f 65001 -i db\07_validation.sql
sqlcmd -S localhost -E -C -I -f 65001 -i db\08_negative_tests.sql
```

> `-I` bắt buộc để bật `QUOTED_IDENTIFIER` cho cột tính toán; `-f 65001` bảo đảm đọc đúng
> tiếng Việt UTF-8.

### Cách B — Khôi phục bản sao lưu

```bat
sqlcmd -S localhost -E -C -I -f 65001 -i db\10_restore_database.sql
```

Hoặc thủ công:

```sql
RESTORE VERIFYONLY FROM DISK = N'E:\Quanap\bak\QuanLyKyGuiThoiTrang_Full_20261115.bak';
RESTORE DATABASE QuanLyKyGuiThoiTrang
FROM DISK = N'E:\Quanap\bak\QuanLyKyGuiThoiTrang_Full_20261115.bak'
WITH REPLACE, RECOVERY;
```

## 3. Chạy ứng dụng

```bat
cd E:\Quanap\app\QuanLyKyGui
dotnet build -v q
dotnet run
```

Hoặc chạy một lệnh từ thư mục gốc (kiểm tra/tạo CSDL → build → mở ứng dụng):

```bat
build.bat            :: chạy toàn bộ dự án
build.bat help       :: xem các chế độ: db, build, test, capture, export, <màn hình>
```

Ứng dụng gồm các màn hình:

| Màn hình | Chức năng |
|---|---|
| Tổng quan | Thẻ số liệu, đơn cần xử lý, phiếu ký gửi gần đây |
| Tiếp nhận ký gửi | Tạo phiếu + nhiều sản phẩm |
| Lập đơn hàng & thu tiền | Giữ hàng, thu tiền, hoàn tất/hủy đơn |
| Tra cứu sản phẩm | Lọc đa tiêu chí, xuất Excel |
| Báo cáo doanh thu | Doanh thu/hoa hồng/theo tháng, xuất Excel |
| Báo cáo quyết toán | Công nợ người ký gửi, lập phiếu chi, xuất Excel |

Khách chưa có trong hệ thống: tại màn hình **Tiếp nhận ký gửi**, bấm **+ Khách mới** cạnh ô tìm
để nhập họ tên, số điện thoại, email, địa chỉ. Ứng dụng kiểm tra định dạng và trùng số điện thoại,
lưu vào `KHACH_HANG` rồi tự chọn khách vừa tạo cho phiếu. Nếu tìm không thấy, hộp thoại cũng hỏi
tạo khách mới với từ khóa đã nhập.

Khách mua chưa có trong hệ thống: tại màn hình **Lập đơn hàng**, bấm **+ Khách mới** cạnh ô
**Khách hàng mua**, nhập thông tin và lưu. Khách vừa tạo được chọn ngay cho đơn đang lập, không
cần rời màn hình hay tạo trước ở nơi khác.

Đưa sản phẩm lên bán: mở **Tra cứu sản phẩm**, chọn sản phẩm **Chờ xử lý** hoặc **Đã chấp nhận /
Chưa đăng bán**, bấm **Đăng bán**, nhập giá và xác nhận. Sản phẩm chuyển sang **Đang bán** và xuất
hiện trong màn hình **Lập đơn hàng**. Nếu sản phẩm đang Chờ xử lý, thao tác này duyệt và đăng bán
luôn; nếu đang Đang bán thì dùng để đổi giá.

Trả hàng khi người ký gửi không muốn ký gửi nữa: vẫn ở **Tra cứu sản phẩm**, chọn sản phẩm
**Đã chấp nhận / Chưa đăng bán** hoặc **Đang bán**, bấm **Trả hàng** (nút cạnh **Đăng bán**) rồi xác
nhận. Sản phẩm chuyển sang **Đã trả lại** và gỡ khỏi sàn; CSDL chặn nếu sản phẩm đang giữ, đã bán
hoặc còn nằm trong đơn hàng chưa hủy. Khi phiếu không còn sản phẩm nào đang bán/đang giữ, phiếu tự
chuyển sang **Đã hoàn tất**.

Ở màn hình **Tổng quan**, nhấp đúp một dòng **Phiếu ký gửi gần đây** để mở chi tiết phiếu kèm đúng
danh sách sản phẩm thuộc phiếu. Ô tìm sản phẩm trong **Lập đơn hàng** lọc ngay theo tên khi gõ.

Chế độ dòng lệnh:

```bat
dotnet run --no-build -- --open banhang     :: mở cửa sổ chính và vào thẳng một màn hình (banhang, tiepnhan, tracuu, doanhthu, quyettoan)
dotnet run --no-build -- --capture E:\Quanap\screenshots   :: chụp lại 6 ảnh màn hình
dotnet run --no-build -- --export                          :: xuất 3 tệp Excel mẫu vào exports\
```

Trong cửa sổ chính, sidebar **CHỨC NĂNG** luôn hiển thị: bấm mục nào thì nội dung đổi ngay
tại chỗ, không mở cửa sổ rời. Nút **Đóng** trong từng trang đưa về màn hình Tổng quan.

## 4. Dữ liệu mẫu và kiểm thử

- Danh mục: 8 nhân viên, 24 khách hàng, 10 thương hiệu, 6 danh mục.
- Giao dịch: 28 phiếu ký gửi, 72 sản phẩm, 32 đơn hàng, 59 chi tiết đơn,
  48 giao dịch thanh toán, 18 phiếu quyết toán, 30 chi tiết quyết toán.
- Bao phủ đủ mọi trạng thái: phiếu (Đang xử lý / Đang ký gửi / Đã hoàn tất / Đã hủy),
  duyệt sản phẩm (Chờ xử lý / Đã chấp nhận / Từ chối), bán (Chưa đăng bán / Đang bán /
  Đang giữ / Đã bán / Đã trả lại), đơn (Chờ thanh toán / Đang xử lý / Hoàn tất / Đã hủy).
- `07_validation.sql`: **18/18** bất biến nghiệp vụ đạt.
- `08_negative_tests.sql`: **23/23** kịch bản vi phạm bị CSDL chặn (PASS).
- Quy tắc trả thừa: đơn DH20 tổng 4.950.000 đ, đã thu 5.000.000 đ → đơn Hoàn tất,
  chênh lệch 50.000 đ hiển thị ở view `vw_DoiSoat_ThanhToan` với kết quả “Trả thừa”.

## 5. Tạo lại tài liệu (tuỳ chọn)

```bat
python tools\generate_seed.py     :: sinh lại script dữ liệu mẫu
python tools\make_erd.py          :: sinh docs\ERD_Chen.drawio và ERD_Chen.png
python tools\build_report.py      :: dựng HTML rồi chuyển thành docs\BaoCao_BaiTapLon.docx
powershell -ExecutionPolicy Bypass -File tools\_embed_images.ps1   :: nhúng ảnh vào docx
```

`build_report.py` cần Microsoft Word (COM Automation) và dùng ảnh trong `screenshots/`.
Sau khi chạy, nhớ chạy `_embed_images.ps1` để ảnh được nhúng thay vì liên kết ngoài.

## 6. Sản phẩm bàn giao

| Tệp | Mô tả |
|---|---|
| `db\00…10_*.sql` | Script CSDL, dữ liệu mẫu, kiểm thử, sao lưu/khôi phục |
| `bak\QuanLyKyGuiThoiTrang_Full_20261115.bak` | Bản sao lưu đã kiểm tra |
| `app\QuanLyKyGui\` | Mã nguồn ứng dụng WinForms |
| `docs\ERD_Chen.drawio`, `docs\ERD_Chen.png` | ERD chỉnh sửa được và ảnh |
| `docs\3NF_Analysis.md` | Phân tích chuẩn hóa 3NF |
| `docs\BaoCao_BaiTapLon.docx` | Báo cáo đồ án (7 mục + phụ lục SQL) |
| `screenshots\01…06*.png` | 6 ảnh minh họa các màn hình |
| `exports\*.xlsx` | Excel mẫu: tra cứu, doanh thu, quyết toán |

## 7. Quy tắc nghiệp vụ cốt lõi

- Hoa hồng cố định **20%** giá bán thực tế, làm tròn đến đồng; tiền thực nhận = giá bán − hoa hồng.
- Chỉ sản phẩm **Đang bán** mới được thêm vào đơn; thêm vào đơn sẽ chuyển sang **Đang giữ**.
- Thu đủ tiền → đơn **Đang xử lý**, sản phẩm **Đã bán**; giao hàng xong → **Hoàn tất**.
- Thanh toán vượt giá trị đơn được coi là **đã trả đủ**; phần chênh đối soát riêng.
- Mỗi chi tiết đơn chỉ được quyết toán **một lần**; phiếu quyết toán chỉ gồm sản phẩm của
  cùng một người ký gửi và chỉ lập cho đơn đã **Hoàn tất**.
- Không sửa/xóa giao dịch thanh toán hoặc phiếu quyết toán đã thanh toán.
