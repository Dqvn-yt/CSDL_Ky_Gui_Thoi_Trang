# -*- coding: utf-8 -*-
"""Sinh báo cáo Word (.docx) cho đồ án CSDL QuanLyKyGuiThoiTrang.

Cách làm: dựng HTML có định dạng đầy đủ (bảng, ảnh, tiêu đề, khối SQL),
sau đó dùng Word COM mở HTML và lưu thành .docx.
"""
import html
import os
import subprocess

BASE = r"E:\Quanap"
DOCS = os.path.join(BASE, "docs")
SS = os.path.join(BASE, "screenshots")
OUT_HTML = os.path.join(DOCS, "_report_build.html")
OUT_DOCX = os.path.join(DOCS, "BaoCao_BaiTapLon.docx")


def esc(s):
    return html.escape(s, quote=False)


def read_sql(name):
    with open(os.path.join(BASE, "db", name), "r", encoding="utf-8") as f:
        return f.read()


def img(rel, caption, width=None):
    src = "file:///" + os.path.join(SS, rel).replace("\\", "/") if not rel.startswith("docs/") else \
        "file:///" + os.path.join(BASE, rel.replace("/", "\\")).replace("\\", "/")
    return ('<div style="page-break-inside:avoid">'
            f'<p class="fig"><img src="{src}" style="width:100%" /></p>'
            f'<p class="cap">{esc(caption)}</p></div>')


def table(headers, rows, widths=None, cls="tbl"):
    out = [f'<table class="{cls}">']
    out.append("<thead><tr>" + "".join(f"<th>{h}</th>" for h in headers) + "</tr></thead><tbody>")
    for r in rows:
        out.append("<tr>" + "".join(f"<td>{c}</td>" for c in r) + "</tr>")
    out.append("</tbody></table>")
    return "\n".join(out)


def sql_block(title, sql):
    return (f'<p class="sqlcap">{esc(title)}</p>'
            f'<pre class="sql">{esc(sql)}</pre>')


# ---------------------------------------------------------------------------
# Nội dung
# ---------------------------------------------------------------------------
CSS = """
@page { size: A4; margin: 2.2cm 2.0cm 2.0cm 2.6cm; }
body { font-family: 'Times New Roman', serif; font-size: 13pt; color: #1f1b20; line-height: 1.45; }
h1 { font-family: 'Segoe UI', sans-serif; font-size: 18pt; color: #7C1F35; margin: 0 0 8pt 0;
     border-bottom: 2px solid #A9822F; padding-bottom: 4pt; page-break-after: avoid; }
h2 { font-family: 'Segoe UI', sans-serif; font-size: 14pt; color: #7C1F35; margin: 16pt 0 6pt 0;
     page-break-after: avoid; }
h3 { font-family: 'Segoe UI', sans-serif; font-size: 12.5pt; color: #2B2530; margin: 12pt 0 4pt 0;
     page-break-after: avoid; }
img { max-width: 100%; }
table.tbl tr { page-break-inside: avoid; }
p { margin: 4pt 0; text-align: justify; }
ul, ol { margin: 4pt 0 4pt 18pt; }
li { margin: 2pt 0; text-align: justify; }
table.tbl { border-collapse: collapse; width: 100%; margin: 8pt 0 10pt 0;
            font-family: 'Segoe UI', sans-serif; font-size: 9.5pt; }
table.tbl th { background: #7C1F35; color: #ffffff; border: 1px solid #5A1626;
               padding: 5pt 6pt; text-align: left; }
table.tbl td { border: 1px solid #CBBFB2; padding: 4pt 6pt; vertical-align: top; }
table.tbl tr:nth-child(even) td { background: #F7F3EC; }
table.meta { border-collapse: collapse; width: 100%; margin-top: 22pt; font-size: 11.5pt; }
table.meta td { border: 1px solid #A9822F; padding: 4pt 10pt; }
.fig { text-align: center; margin: 10pt 0 2pt 0; }
.cap { text-align: center; font-style: italic; font-size: 10pt; color: #5b514b; margin: 0 0 12pt 0; }
.cover { text-align: center; }
.cover .school { font-family: 'Segoe UI', sans-serif; font-size: 12pt; letter-spacing: 1px; }
.cover .title { font-family: 'Segoe UI', sans-serif; font-size: 26pt; font-weight: bold;
                color: #7C1F35; margin: 34pt 20pt 8pt 20pt; line-height: 1.3; }
.cover .sub { font-family: 'Segoe UI', sans-serif; font-size: 13pt; color: #A9822F;
              letter-spacing: 1px; margin-bottom: 26pt; }
.cover .rule { border-top: 3px double #A9822F; width: 60%; margin: 16pt auto; }
.toc { font-family: 'Segoe UI', sans-serif; font-size: 11pt; }
.toc div { margin: 3pt 0; }
.toc .l2 { margin-left: 18pt; color: #4a423c; }
pre.sql { font-family: 'Consolas', monospace; font-size: 7.6pt; background: #F7F3EC;
          border: 1px solid #D8CEC0; border-left: 3px solid #7C1F35; padding: 6pt;
          white-space: pre-wrap; line-height: 1.22; }
.sqlcap { font-family: 'Segoe UI', sans-serif; font-size: 10pt; font-weight: bold;
          color: #7C1F35; margin: 12pt 0 3pt 0; }
.note { background: #FBF3DC; border-left: 4px solid #A9822F; padding: 7pt 10pt; margin: 8pt 0;
        font-size: 11.5pt; }
.brk { page-break-before: always; }
.small { font-size: 10pt; color: #5b514b; }
"""

PARTS = []

PARTS.append("""<div class="cover">
<p class="school">TRƯỜNG ĐẠI HỌC ................................</p>
<p class="school">KHOA CÔNG NGHỆ THÔNG TIN</p>
<div class="rule"></div>
<p class="school" style="margin-top:50pt">BÁO CÁO BÀI TẬP LỚN</p>
<p class="school" style="font-size:14pt; color:#A9822F">MÔN: CƠ SỞ DỮ LIỆU</p>
<p class="title">HỆ THỐNG QUẢN LÝ SÀN KÝ GỬI<br/>ĐỒ THỜI TRANG CŨ</p>
<p class="sub">SQL SERVER 2025 &nbsp;•&nbsp; C# WINFORMS (.NET 9)</p>
<table class="meta" style="width:72%; margin-left:auto; margin-right:auto">
<tr><td style="width:38%">Sinh viên thực hiện</td><td>................................</td></tr>
<tr><td>Mã số sinh viên</td><td>................................</td></tr>
<tr><td>Lớp</td><td>................................</td></tr>
<tr><td>Giảng viên hướng dẫn</td><td>................................</td></tr>
<tr><td>Cơ sở dữ liệu</td><td>QuanLyKyGuiThoiTrang</td></tr>
<tr><td>Ngày hoàn thành</td><td>15/11/2026</td></tr>
</table>
</div>""")

PARTS.append("""<div class="brk"></div>
<h1 style="border:none; text-align:center; font-size:20pt">MỤC LỤC</h1>
<div class="toc">
<div><b>1. Tổng quan yêu cầu dữ liệu</b></div>
<div class="l2">1.1 Bối cảnh và mục tiêu &nbsp; 1.2 Phạm vi, tác nhân &nbsp; 1.3 Chức năng</div>
<div class="l2">1.4 Quy tắc nghiệp vụ &nbsp; 1.5 Yêu cầu dữ liệu &nbsp; 1.6 Môi trường công nghệ</div>
<div><b>2. Sơ đồ thực thể – liên kết (ERD)</b></div>
<div class="l2">2.1 Phương pháp &nbsp; 2.2 Thực thể và thuộc tính &nbsp; 2.3 Quan hệ và bản số &nbsp; 2.4 Hình ERD</div>
<div><b>3. Mô hình quan hệ</b></div>
<div class="l2">3.1 Quy tắc chuyển đổi &nbsp; 3.2 Lược đồ quan hệ &nbsp; 3.3 Khóa và toàn vẹn &nbsp; 3.4 Đối tượng CSDL</div>
<div><b>4. Kiểm tra và chuẩn hóa 3NF</b></div>
<div class="l2">4.1 Phụ thuộc hàm &nbsp; 4.2 Các điểm sửa &nbsp; 4.3 Kết luận</div>
<div><b>5. Hai màn hình nhập liệu và minh họa ràng buộc</b></div>
<div class="l2">5.1 Tiếp nhận ký gửi &nbsp; 5.2 Lập đơn hàng và thu tiền &nbsp; 5.3 Kịch bản kiểm thử</div>
<div><b>6. Màn hình tra cứu sản phẩm</b></div>
<div><b>7. Hai báo cáo nghiệp vụ</b></div>
<div class="l2">7.1 Báo cáo doanh thu &nbsp; 7.2 Báo cáo quyết toán &nbsp; 7.3 Quy tắc trả thừa</div>
<div><b>Phụ lục A. Danh mục sản phẩm bàn giao</b></div>
<div><b>Phụ lục B. Mã SQL cho mục 5 – 7</b></div>
</div>""")

# --- 1 -----------------------------------------------------------------------
PARTS.append("""<div class="brk"></div>
<h1>1. Tổng quan yêu cầu dữ liệu</h1>

<h2>1.1 Bối cảnh và mục tiêu</h2>
<p>Sàn ký gửi đồ thời trang cũ là mô hình kinh doanh trong đó khách hàng mang sản phẩm
đến gửi bán, cửa hàng kiểm tra – định giá – trưng bày, người mua đến chọn và thanh toán,
cuối cùng cửa hàng quyết toán tiền bán cho người ký gửi sau khi trừ hoa hồng.
Bài toán đặt ra là xây dựng một cơ sở dữ liệu quan hệ lưu trữ toàn bộ vòng đời đó và
một ứng dụng desktop minh họa các nghiệp vụ then chốt.</p>
<p>Mục tiêu của đồ án:</p>
<ul>
<li>Thiết kế CSDL chuẩn hóa 3NF cho nghiệp vụ ký gửi, bán hàng và quyết toán.</li>
<li>Cài đặt trên Microsoft SQL Server 2025 với đầy đủ ràng buộc, trigger, view, procedure.</li>
<li>Xây dựng ứng dụng C# WinForms (.NET 9) minh họa nhập liệu, tra cứu và báo cáo.</li>
<li>Sinh báo cáo Word và các tệp Excel xuất từ ứng dụng.</li>
</ul>

<h2>1.2 Phạm vi và tác nhân</h2>
""")
PARTS.append(table(
    ["Tác nhân", "Vai trò trong hệ thống", "Chức năng sử dụng"],
    [
        ["Khách ký gửi", "Chủ sở hữu sản phẩm mang đến gửi bán", "Được ghi nhận thông tin, nhận phiếu ký gửi, nhận tiền quyết toán"],
        ["Khách mua", "Người mua sản phẩm trưng bày", "Được lập đơn hàng, thanh toán, nhận hàng"],
        ["Nhân viên tiếp nhận", "Kiểm tra, định giá, duyệt và đăng bán sản phẩm", "Màn hình tiếp nhận ký gửi, duyệt sản phẩm"],
        ["Thu ngân / Kế toán", "Lập đơn, thu tiền, lập phiếu quyết toán", "Màn hình lập đơn hàng, báo cáo doanh thu, quyết toán"],
        ["Quản lý cửa hàng", "Giám sát hoạt động, nhận báo cáo", "Báo cáo doanh thu, báo cáo quyết toán, tra cứu"],
    ]))

PARTS.append("""
<h2>1.3 Chức năng chính của hệ thống</h2>
<ol>
<li><b>Tiếp nhận ký gửi:</b> tạo phiếu ký gửi cho khách, nhập nhiều sản phẩm kèm tình trạng,
kích thước, màu sắc, giá đề xuất và trạng thái duyệt.</li>
<li><b>Duyệt và đăng bán:</b> chấp nhận/từ chối sản phẩm, gán giá đăng bán, đưa sản phẩm lên bán.</li>
<li><b>Lập đơn hàng:</b> chọn sản phẩm đang bán, áp giảm giá, giữ hàng, tính tổng tiền.</li>
<li><b>Thanh toán:</b> ghi nhận một hoặc nhiều giao dịch, tự chuyển trạng thái đơn khi thu đủ.</li>
<li><b>Hoàn tất / hủy đơn:</b> hoàn tất khi đã thu đủ và sản phẩm đã bán; hủy khi chưa phát sinh tiền.</li>
<li><b>Trả hàng:</b> trả sản phẩm chưa bán được cho người ký gửi.</li>
<li><b>Quyết toán:</b> lập phiếu trả tiền cho người ký gửi, trừ hoa hồng 20%, xác nhận đã chi.</li>
<li><b>Tra cứu:</b> tìm sản phẩm theo từ khóa, danh mục, thương hiệu, kích thước, trạng thái, người ký gửi, khoảng giá.</li>
<li><b>Báo cáo:</b> doanh thu theo tháng và quyết toán theo người ký gửi, xem trên màn hình và xuất Excel.</li>
</ol>

<h2>1.4 Quy tắc nghiệp vụ</h2>
""")
PARTS.append(table(
    ["Mã", "Quy tắc nghiệp vụ", "Cơ chế thực thi"],
    [
        ["BR1", "Mỗi phiếu ký gửi phải có ít nhất một sản phẩm", "Procedure + kiểm tra khi lưu"],
        ["BR2", "Sản phẩm đã chấp nhận phải có giá đăng bán lớn hơn 0", "CHECK + procedure + trigger"],
        ["BR3", "Chỉ sản phẩm đang bán mới được thêm vào đơn", "Trigger TR_CTDH"],
        ["BR4", "Một sản phẩm chỉ thuộc một đơn hàng còn hiệu lực", "Trigger + UQ (MaDH, MaSP)"],
        ["BR5", "Đơn chuyển Đang xử lý khi đã thu đủ tiền", "Trigger TR_THANH_TOAN"],
        ["BR6", "Chỉ hoàn tất đơn khi đã thu đủ và mọi sản phẩm đã bán", "Trigger TR_DON_HANG"],
        ["BR7", "Không hủy đơn đã có thanh toán thành công hoặc đang chờ", "Procedure + trigger"],
        ["BR8", "Hoa hồng cố định 20% giá bán thực tế, làm tròn đến đồng", "Cột tính toán + view"],
        ["BR9", "Mỗi chi tiết đơn chỉ được quyết toán một lần", "UQ MaCTDH + trigger TR_CTTTKG"],
        ["BR10", "Phiếu quyết toán chỉ gồm sản phẩm của cùng một người ký gửi", "Trigger TR_CTTTKG + procedure"],
        ["BR11", "Thanh toán vượt quá giá trị đơn được coi là đã trả đủ, phần chênh đối soát riêng", "Trigger TR_THANH_TOAN + view đối soát"],
        ["BR12", "Không sửa/xóa giao dịch thanh toán hoặc phiếu quyết toán đã hoàn tất", "Trigger TR_THANH_TOAN, TR_PTTKG"],
    ]))

PARTS.append("""
<h2>1.5 Yêu cầu dữ liệu</h2>
<ul>
<li><b>Người dùng:</b> khách hàng (ký gửi và mua) và nhân viên (tiếp nhận, thu ngân, quản lý).</li>
<li><b>Danh mục:</b> danh mục sản phẩm, thương hiệu.</li>
<li><b>Giao dịch:</b> phiếu ký gửi, sản phẩm, đơn hàng, chi tiết đơn hàng, thanh toán đơn hàng,
phiếu thanh toán ký gửi, chi tiết thanh toán ký gửi.</li>
<li><b>Trạng thái:</b> phiếu ký gửi (Đang xử lý, Đang ký gửi, Đã hoàn tất, Đã hủy);
duyệt sản phẩm (Chờ xử lý, Đã chấp nhận, Từ chối); bán (Chưa đăng bán, Đang bán, Đang giữ,
Đã bán, Đã trả lại); đơn hàng (Chờ thanh toán, Đang xử lý, Hoàn tất, Đã hủy).</li>
<li><b>Tiền tệ:</b> DECIMAL(12,0) — đơn vị đồng, không phần lẻ.</li>
<li><b>Thời gian:</b> DATETIME2(0) — độ chính xác đến giây.</li>
<li><b>Tiếng Việt:</b> toàn bộ chuỗi dùng NVARCHAR, collation Vietnamese_CI_AS.</li>
</ul>

<h2>1.6 Môi trường công nghệ</h2>
""")
PARTS.append(table(
    ["Thành phần", "Công nghệ / phiên bản"],
    [
        ["Hệ quản trị CSDL", "Microsoft SQL Server 2025 (16.x/17.x), collation Vietnamese_CI_AS"],
        ["Ngôn ngữ CSDL", "T-SQL: bảng, CHECK, FK, computed column, trigger, view, stored procedure, table type"],
        ["Ứng dụng minh họa", "C# WinForms, .NET 9 (net9.0-windows)"],
        ["Truy cập dữ liệu", "Microsoft.Data.SqlClient 7.1.2"],
        ["Xuất Excel", "ClosedXML 0.105.1"],
        ["Báo cáo Word", "Microsoft Word (COM Automation), tệp .docx"],
        ["Sao lưu", "SQL Server Backup .bak (định dạng 2025)"],
    ]))
PARTS.append("""
<div class="note"><b>Ghi chú kỹ thuật:</b> các script được chạy bằng
<code>sqlcmd -S localhost -E -C -I -f 65001</code>. Tham số <code>-I</code> bắt buộc để
bật QUOTED_IDENTIFIER cho cột tính toán; <code>-f 65001</code> bảo đảm đọc đúng tiếng Việt UTF-8.
Chuỗi kết nối của ứng dụng:
<code>Server=localhost;Database=QuanLyKyGuiThoiTrang;Integrated Security=true;TrustServerCertificate=true;Encrypt=false;</code>
</div>""")

# --- 2 ERD -------------------------------------------------------------------
PARTS.append("""<div class="brk"></div>
<h1>2. Sơ đồ thực thể – liên kết (ERD)</h1>

<h2>2.1 Phương pháp thiết kế</h2>
<p>Áp dụng mô hình thực thể – liên kết (Chen notation) với 11 thực thể. Mỗi thực thể có
khóa chính là mã số nguyên tự tăng; quan hệ một – nhiều thể hiện bằng khóa ngoại ở phía
nhiều; quan hệ một – một (chi tiết đơn hàng ↔ chi tiết quyết toán) được cài bằng ràng buộc
duy nhất trên khóa ngoại. Sản phẩm là trung tâm: thuộc một phiếu ký gửi, một danh mục,
có thể thuộc một thương hiệu; mỗi sản phẩm đã bán sinh ra đúng một chi tiết đơn hàng,
và chi tiết đó được quyết toán tối đa một lần.</p>

<h2>2.2 Thực thể và thuộc tính chính</h2>
""")
PARTS.append(table(
    ["Thực thể", "Ý nghĩa", "Thuộc tính chính"],
    [
        ["KHACH_HANG", "Khách hàng (ký gửi và mua)", "MaKH, HoTen, SoDienThoai, Email, DiaChi, NgayTao"],
        ["NHAN_VIEN", "Nhân viên cửa hàng", "MaNV, HoTen, SoDienThoai, Email, ChucVu, TrangThai"],
        ["THUONG_HIEU", "Thương hiệu sản phẩm", "MaTH, TenThuongHieu, MoTa"],
        ["DANH_MUC", "Danh mục sản phẩm", "MaDM, TenDanhMuc, MoTa"],
        ["PHIEU_KY_GUI", "Phiếu ghi nhận ký gửi của một khách", "MaPKG, MaKH, MaNV, NgayKyGui, TrangThai, GhiChu"],
        ["SAN_PHAM", "Sản phẩm ký gửi", "MaSP, MaPKG, MaDM, MaTH, TenSP, KichThuoc, MauSac, ChatLieu, TinhTrang, GiaDeXuat, GiaDangBan, TrangThaiTiepNhan, TrangThaiBan"],
        ["DON_HANG", "Đơn mua hàng", "MaDH, MaKH, MaNV, NgayLap, HanThanhToan, TrangThai, LyDoHuy"],
        ["CHI_TIET_DON_HANG", "Dòng sản phẩm trong đơn", "MaCTDH, MaDH, MaSP, GiaNiemYetLucBan, GiamGia, GiaBanThucTe (tính toán)"],
        ["THANH_TOAN_DON_HANG", "Giao dịch thu tiền của đơn", "MaTTDH, MaDH, SoTien, NgayTao, NgayHoanTat, PhuongThuc, TrangThai"],
        ["PHIEU_THANH_TOAN_KY_GUI", "Phiếu quyết toán trả tiền người ký gửi", "MaPTTKG, MaKH, MaNV, NgayLap, NgayThanhToan, PhuongThuc, TrangThai"],
        ["CHI_TIET_THANH_TOAN_KY_GUI", "Dòng quyết toán cho từng sản phẩm", "MaCTTTKG, MaPTTKG, MaCTDH, GiaBanTinhToan, TienHoaHong, TienThucNhan"],
    ]))

PARTS.append("""<h2>2.3 Quan hệ và bản số</h2>""")
PARTS.append(table(
    ["Quan hệ", "Bản số", "Diễn giải"],
    [
        ["KHACH_HANG – PHIEU_KY_GUI", "1 : N", "Một khách có nhiều phiếu ký gửi"],
        ["NHAN_VIEN – PHIEU_KY_GUI", "1 : N", "Một nhân viên tiếp nhận nhiều phiếu"],
        ["PHIEU_KY_GUI – SAN_PHAM", "1 : N", "Một phiếu chứa nhiều sản phẩm"],
        ["DANH_MUC – SAN_PHAM", "1 : N", "Một danh mục có nhiều sản phẩm"],
        ["THUONG_HIEU – SAN_PHAM", "1 : N (tùy chọn)", "Sản phẩm có thể không rõ thương hiệu"],
        ["KHACH_HANG – DON_HANG", "1 : N", "Một khách mua nhiều đơn"],
        ["NHAN_VIEN – DON_HANG", "1 : N", "Một nhân viên lập nhiều đơn"],
        ["DON_HANG – CHI_TIET_DON_HANG", "1 : N", "Một đơn gồm nhiều dòng sản phẩm"],
        ["SAN_PHAM – CHI_TIET_DON_HANG", "1 : N (mỗi sản phẩm bán một lần)", "Sản phẩm xuất hiện trong tối đa một chi tiết"],
        ["DON_HANG – THANH_TOAN_DON_HANG", "1 : N", "Một đơn có thể thu nhiều lần"],
        ["KHACH_HANG – PHIEU_THANH_TOAN_KY_GUI", "1 : N", "Một khách nhận nhiều đợt quyết toán"],
        ["NHAN_VIEN – PHIEU_THANH_TOAN_KY_GUI", "1 : N", "Một nhân viên lập nhiều phiếu chi"],
        ["PHIEU_THANH_TOAN_KY_GUI – CHI_TIET_THANH_TOAN_KY_GUI", "1 : N", "Một phiếu gồm nhiều dòng"],
        ["CHI_TIET_DON_HANG – CHI_TIET_THANH_TOAN_KY_GUI", "1 : 1", "Mỗi dòng bán được quyết toán tối đa một lần"],
    ]))

PARTS.append("""<h2>2.4 Hình ERD</h2>""")
PARTS.append(img("docs/ERD_Chen.png", "Hình 2.1 — ERD hệ thống Quản lý sàn ký gửi đồ thời trang cũ", 700))
PARTS.append("""<p class="small">Tệp chỉnh sửa: <code>docs/ERD_Chen.drawio</code> (mở bằng draw.io);
tệp ảnh: <code>docs/ERD_Chen.png</code>. Ký hiệu: PK khóa chính, FK khóa ngoại,
UQ ràng buộc duy nhất, TÍNH cột tính toán; đường nối 1–N và 1–1.</p>""")

# --- 3 relational schema -----------------------------------------------------
PARTS.append("""<div class="brk"></div>
<h1>3. Mô hình quan hệ</h1>

<h2>3.1 Quy tắc chuyển đổi từ ERD</h2>
<ol>
<li>Mỗi thực thể thành một quan hệ; thuộc tính đơn trị thành cột.</li>
<li>Quan hệ 1–N: thêm khóa chính phía 1 làm khóa ngoại phía N.</li>
<li>Quan hệ 1–1: đặt khóa ngoại kèm ràng buộc UNIQUE (MaCTDH trong chi tiết quyết toán).</li>
<li>Quan hệ N–N nếu có được tách thành bảng trung gian; trong hệ thống này chi tiết đơn hàng
đóng vai trò bảng nối giữa đơn hàng và sản phẩm, có thuộc tính riêng (giá, giảm giá).</li>
<li>Thuộc tính đa trị (nhiều sản phẩm trong phiếu/đơn, nhiều lần thanh toán) tách bảng riêng.</li>
</ol>

<h2>3.2 Lược đồ quan hệ</h2>
<p class="small">Quy ước: <b>in đậm</b> = khóa chính; <i>nghiêng</i> = khóa ngoại; (UQ) = ràng buộc duy nhất.</p>
""")

SCHEMAS = [
    ("KHACH_HANG", "<b>MaKH</b>, HoTen, <i>SoDienThoai (UQ)</i>, Email, DiaChi, NgayTao"),
    ("NHAN_VIEN", "<b>MaNV</b>, HoTen, SoDienThoai, Email, ChucVu, TrangThai"),
    ("THUONG_HIEU", "<b>MaTH</b>, <i>TenThuongHieu (UQ)</i>, MoTa"),
    ("DANH_MUC", "<b>MaDM</b>, <i>TenDanhMuc (UQ)</i>, MoTa"),
    ("PHIEU_KY_GUI", "<b>MaPKG</b>, <i>MaKH</i>, <i>MaNV</i>, NgayKyGui, TrangThai, GhiChu"),
    ("SAN_PHAM", "<b>MaSP</b>, <i>MaPKG</i>, <i>MaDM</i>, <i>MaTH</i>, TenSP, MoTa, KichThuoc, MauSac, ChatLieu, TinhTrang, GiaDeXuat, GiaDangBan, TrangThaiTiepNhan, TrangThaiBan, NgayDangBan"),
    ("DON_HANG", "<b>MaDH</b>, <i>MaKH</i>, <i>MaNV</i>, NgayLap, HanThanhToan, TrangThai, LyDoHuy, GhiChu"),
    ("CHI_TIET_DON_HANG", "<b>MaCTDH</b>, <i>MaDH</i>, <i>MaSP</i>, GiaNiemYetLucBan, GiamGia, GiaBanThucTe (tính toán)"),
    ("THANH_TOAN_DON_HANG", "<b>MaTTDH</b>, <i>MaDH</i>, SoTien, NgayTao, NgayHoanTat, PhuongThuc, TrangThai, MaGiaoDich, GhiChu"),
    ("PHIEU_THANH_TOAN_KY_GUI", "<b>MaPTTKG</b>, <i>MaKH</i>, <i>MaNV</i>, NgayLap, NgayThanhToan, PhuongThuc, TrangThai, MaGiaoDich, GhiChu"),
    ("CHI_TIET_THANH_TOAN_KY_GUI", "<b>MaCTTTKG</b>, <i>MaPTTKG</i>, <i>MaCTDH (UQ)</i>, GiaBanTinhToan, TienHoaHong (tính toán), TienThucNhan (tính toán)"),
]
for name, cols in SCHEMAS:
    PARTS.append(f'<p><b>{name}</b>({cols})</p>')

PARTS.append("""
<h2>3.3 Khóa và ràng buộc toàn vẹn</h2>
<ul>
<li><b>Khóa chính:</b> mỗi bảng dùng INT IDENTITY, không tái sử dụng giá trị.</li>
<li><b>Khóa ngoại:</b> tất cả quan hệ đều khai báo FOREIGN KEY, ngăn bản ghi mồ côi.</li>
<li><b>Ràng buộc duy nhất:</b> SoDienThoai khách hàng, tên thương hiệu, tên danh mục,
cặp (MaDH, MaSP) trong chi tiết đơn, MaCTDH trong chi tiết quyết toán.</li>
<li><b>Ràng buộc CHECK:</b> định dạng tên/số điện thoại/email, miền giá trị trạng thái,
quan hệ giữa trạng thái và các cột ngày/giá (ví dụ: Đang bán phải có giá và ngày đăng bán),
hạn thanh toán không trước ngày lập, đơn hủy phải có lý do.</li>
<li><b>Toàn vẹn nghiệp vụ:</b> 6 trigger và 13 stored procedure (xem mục 5 và Phụ lục B).</li>
<li><b>Chỉ mục:</b> đánh chỉ mục cho toàn bộ khóa ngoại và các cột lọc thường dùng
(trạng thái + ngày, tên sản phẩm, hạn thanh toán).</li>
</ul>

<h2>3.4 Đối tượng cơ sở dữ liệu đã cài đặt</h2>
""")
PARTS.append(table(
    ["Loại đối tượng", "Số lượng", "Chi tiết"],
    [
        ["Bảng", "11", "KHACH_HANG … CHI_TIET_THANH_TOAN_KY_GUI"],
        ["Cột tính toán", "3", "GiaBanThucTe, TienHoaHong, TienThucNhan (PERSISTED)"],
        ["Trigger", "6", "TR_SAN_PHAM, TR_CTDH, TR_DON_HANG, TR_THANH_TOAN, TR_CTTTKG, TR_PTTKG"],
        ["Hàm", "1", "fn_TrangThaiPhieuKyGui"],
        ["View", "9", "vw_SanPham_DayDu, vw_DonHang_TongHop, vw_DoanhThu_TheoThang, vw_CongNo_NguoiKyGui, vw_TonKho_TheoTrangThai, vw_PhieuKyGui_TienDo, vw_DoiSoat_ThanhToan, vw_HieuSuat_NhanVien, vw_QuyetToan_SanPham"],
        ["Stored procedure", "13", "Tiếp nhận, duyệt, đăng bán, tạo đơn, thu tiền, hủy, hoàn tất, quyết toán, trả hàng, tra cứu, 2 báo cáo"],
        ["Table type", "3", "SanPhamKyGuiType, ChiTietDonHangType, DanhSachCTDHType"],
        ["Chỉ mục", "22", "Khóa ngoại và cột lọc"],
    ]))

# --- 4 3NF -------------------------------------------------------------------
PARTS.append("""<div class="brk"></div>
<h1>4. Kiểm tra và chuẩn hóa 3NF</h1>

<h2>4.1 Phân tích phụ thuộc hàm</h2>
<p>Toàn bộ bảng dùng khóa chính đơn nên 2NF thỏa hiển nhiên. Trọng tâm kiểm tra là 1NF và 3NF.
Các phụ thuộc hàm tiêu biểu:</p>
""")
PARTS.append(sql_block("Phụ thuộc hàm chính của các quan hệ",
"""KHACH_HANG:        MaKH -> HoTen, SoDienThoai, Email, DiaChi, NgayTao
                   SoDienThoai -> MaKH                (khoa du tuyen)
THUONG_HIEU:       MaTH -> TenThuongHieu, MoTa
                   TenThuongHieu -> MaTH             (khoa du tuyen)
DANH_MUC:          MaDM -> TenDanhMuc, MoTa
PHIEU_KY_GUI:      MaPKG -> MaKH, MaNV, NgayKyGui, TrangThai, GhiChu
SAN_PHAM:          MaSP -> MaPKG, MaDM, MaTH, TenSP, ..., TrangThaiBan
DON_HANG:          MaDH -> MaKH, MaNV, NgayLap, HanThanhToan, TrangThai, LyDoHuy
CHI_TIET_DON_HANG: MaCTDH -> MaDH, MaSP, GiaNiemYetLucBan, GiamGia
                   (MaDH, MaSP) -> MaCTDH            (khoa du tuyen)
                   GiaNiemYetLucBan, GiamGia -> GiaBanThucTe   (bac cau neu luu thuong)
CHI_TIET_TTKG:     MaCTTTKG -> MaPTTKG, MaCTDH, GiaBanTinhToan
                   MaCTDH -> MaCTTTKG                (khoa du tuyen)
                   GiaBanTinhToan -> TienHoaHong, TienThucNhan (bac cau neu luu thuong)"""))

PARTS.append("""
<h2>4.2 Các vi phạm tiềm ẩn và cách sửa</h2>
<h3>4.2.1 Thành tiền dòng hàng</h3>
<p>Thành tiền = giá niêm yết − giảm giá. Nếu lưu thành cột thường, quan hệ vi phạm 3NF vì
thuộc tính không khóa phụ thuộc vào hai thuộc tính không khóa khác; khi sửa giảm giá mà
quên cập nhật thành tiền sẽ phát sinh dị thường. Giải pháp là <b>cột tính toán PERSISTED</b>:</p>
""")
PARTS.append(sql_block("Sửa 3NF — CHI_TIET_DON_HANG.GiaBanThucTe",
"""GiaBanThucTe AS (CAST(GiaNiemYetLucBan - GiamGia AS DECIMAL(12,0))) PERSISTED"""))

PARTS.append("""
<h3>4.2.2 Hoa hồng và tiền thực nhận</h3>
<p>Hoa hồng cố định 20% giá bán, tiền thực nhận = giá bán − hoa hồng. Cả hai đều là hàm của
`GiaBanTinhToan`, nên được khai báo cột tính toán để SQL Server tự tính và không thể ghi sai:</p>
""")
PARTS.append(sql_block("Sửa 3NF — CHI_TIET_THANH_TOAN_KY_GUI",
"""TienHoaHong  AS (CAST(ROUND(GiaBanTinhToan * 0.20, 0) AS DECIMAL(12,0))) PERSISTED,
TienThucNhan AS (CAST(GiaBanTinhToan - ROUND(GiaBanTinhToan * 0.20, 0) AS DECIMAL(12,0))) PERSISTED"""))

PARTS.append("""
<h3>4.2.3 Tổng tiền đơn hàng</h3>
<p>Tổng tiền là tổng thành tiền các dòng. Hệ thống <b>không lưu</b> tổng tiền trong DON_HANG
mà tính trong view <code>vw_DonHang_TongHop</code>. Nhờ vậy tránh hoàn toàn phụ thuộc bắc cầu
và dị thường cập nhật, đồng thời vẫn đáp ứng truy vấn nhờ chỉ mục trên MaDH.</p>

<h3>4.2.4 Trạng thái phiếu ký gửi</h3>
<p>Trạng thái phiếu suy ra từ tập sản phẩm. Đây là dư thừa có kiểm soát: trạng thái là thuộc tính
nghiệp vụ của chính phiếu, được đồng bộ tự động bằng <code>fn_TrangThaiPhieuKyGui</code> trong
các trigger. Script kiểm tra <code>07_validation.sql</code> xác nhận trạng thái phiếu luôn khớp
trạng thái sản phẩm trên toàn bộ dữ liệu mẫu (18/18 bất biến đạt).</p>

<h2>4.3 Kết luận</h2>
<p>Sau khi thay ba thuộc tính dẫn xuất bằng cột tính toán và loại bỏ tổng tiền khỏi bảng,
lược đồ gồm 11 quan hệ đạt <b>1NF, 2NF và 3NF</b>. Các thuộc tính trạng thái được kiểm soát
bằng trigger và đã được tài liệu hóa. Chi tiết đầy đủ xem <code>docs/3NF_Analysis.md</code>.</p>""")

# --- 5 entry screens ---------------------------------------------------------
PARTS.append("""<div class="brk"></div>
<h1>5. Hai màn hình nhập liệu</h1>
<p>Ứng dụng WinForms gồm hai màn hình nhập liệu chính. Ràng buộc nghiệp vụ được bảo đảm ngay
trong CSDL bằng procedure và trigger; khi một thao tác bị chặn, ứng dụng hiển thị nguyên văn
thông báo lỗi do SQL Server trả về.</p>

<h2>5.1 Màn hình Tiếp nhận ký gửi</h2>
<p>Chức năng: tìm/khách hàng, chọn nhân viên tiếp nhận, ngày ký gửi, ghi chú; nhập danh sách
sản phẩm (danh mục, thương hiệu, tên, kích thước, màu sắc, chất liệu, tình trạng, giá đề xuất,
giá đăng bán, trạng thái duyệt) rồi lưu phiếu. Thao tác lưu gọi procedure
<code>sp_TiepNhanPhieuKyGui</code> với table-valued parameter.</p>
""")
PARTS.append(img("02-tiep-nhan-ky-gui.png", "Hình 5.1 — Màn hình Tiếp nhận ký gửi với 3 sản phẩm trong phiếu"))

PARTS.append("""
<h2>5.2 Màn hình Lập đơn hàng và thu tiền</h2>
<p>Chức năng: chọn khách mua, nhân viên lập, ngày lập, hạn thanh toán; thêm sản phẩm đang bán
kèm giảm giá; xem tổng cộng; tạo đơn; ghi nhận thanh toán nhiều lần; hoàn tất hoặc hủy đơn có
lý do. Màn hình hiển thị cảnh báo trạng thái và số tiền còn lại theo thời gian thực.</p>
""")
PARTS.append(img("03-lap-don-hang.png", "Hình 5.2 — Màn hình Lập đơn hàng, thu tiền và xử lý đơn gần đây"))

PARTS.append("""
<h2>5.3 Bảng kịch bản kiểm thử ràng buộc</h2>
<p>Script <code>08_negative_tests.sql</code> chạy 23 kịch bản
vi phạm và ghi nhận kết quả, tất cả đều <b>PASS</b> (CSDL chặn đúng). Trích các kịch bản:</p>
""")
PARTS.append(table(
    ["#", "Kịch bản vi phạm", "Cơ chế chặn", "Kết quả"],
    [
        ["1", "Duyệt sản phẩm không nhập giá đăng bán", "sp_DuyetSanPham", "PASS"],
        ["2", "Lập phiếu ký gửi không có sản phẩm", "sp_TiepNhanPhieuKyGui", "PASS"],
        ["3", "Phiếu có sản phẩm đã chấp nhận thiếu giá", "sp_TiepNhanPhieuKyGui", "PASS"],
        ["4", "Thêm sản phẩm Chờ xử lý vào đơn", "TR_CTDH / sp_TaoDonHang", "PASS"],
        ["5", "Thêm sản phẩm Đang giữ vào đơn khác", "TR_CTDH", "PASS"],
        ["6", "Giảm giá lớn hơn giá niêm yết", "sp_TaoDonHang", "PASS"],
        ["7", "Trùng sản phẩm trong cùng một đơn", "UQ_CHI_TIET_DON_HANG_DH_SP", "PASS"],
        ["8", "Hoàn tất đơn Chờ thanh toán", "TR_DON_HANG", "PASS"],
        ["9", "Hủy đơn đã thu một phần", "TR_DON_HANG", "PASS"],
        ["10", "Hủy đơn có giao dịch Chờ xử lý", "TR_DON_HANG", "PASS"],
        ["11", "Quyết toán sản phẩm thuộc đơn Đang xử lý", "TR_CTTTKG", "PASS"],
        ["12", "Quyết toán lại sản phẩm đã quyết toán", "UQ + TR_CTTTKG", "PASS"],
        ["13", "Quyết toán sản phẩm của người ký gửi khác", "TR_CTTTKG", "PASS"],
        ["14", "Sửa phiếu quyết toán đã thanh toán", "TR_PTTKG", "PASS"],
        ["15", "Xóa giao dịch thanh toán đã thành công", "TR_THANH_TOAN", "PASS"],
        ["16", "Sửa đơn hàng đã hoàn tất", "TR_DON_HANG", "PASS"],
        ["17", "Trả lại sản phẩm đã bán", "TR_SAN_PHAM", "PASS"],
        ["18", "Thêm sản phẩm vào đơn Đang xử lý", "TR_CTDH", "PASS"],
        ["19", "Ghi nhận thanh toán 0 đồng", "sp_GhiNhanThanhToan", "PASS"],
        ["20", "Ghi nhận thanh toán cho đơn đã hủy", "sp_GhiNhanThanhToan", "PASS"],
        ["21", "Trả thừa được coi là đã trả đủ", "TR_THANH_TOAN", "PASS"],
        ["22", "Thu một phần giữ nguyên Chờ thanh toán", "TR_THANH_TOAN", "PASS"],
        ["23", "Thêm sản phẩm đang bán vào đơn thứ hai", "TR_CTDH", "PASS"],
    ]))
PARTS.append("""<div class="note"><b>Kết quả tổng hợp:</b> 18/18 bất biến nghiệp vụ đạt trong
<code>07_validation.sql</code>; 23/23 kịch bản vi phạm bị chặn trong <code>08_negative_tests.sql</code>.</div>""")

# --- 6 search ----------------------------------------------------------------
PARTS.append("""<div class="brk"></div>
<h1>6. Màn hình tra cứu sản phẩm</h1>
<p>Màn hình tra cứu cho phép tìm theo từ khóa (tên sản phẩm, thương hiệu, người ký gửi),
kết hợp bộ lọc danh mục, thương hiệu, kích thước, trạng thái bán, người ký gửi và khoảng giá.
Kết quả hiển thị dạng lưới kèm thông tin người ký gửi; chọn một dòng để xem chi tiết bên dưới.
Nút <i>Xuất Excel</i> kết xuất toàn bộ kết quả đang lọc.</p>
""")
PARTS.append(img("04-tra-cuu-san-pham.png", "Hình 6.1 — Tra cứu sản phẩm: từ khóa “Zara”, tìm thấy 14 sản phẩm"))
PARTS.append("""
<p>Truy vấn sử dụng procedure <code>sp_TraCuuSanPham</code> trên view
<code>vw_SanPham_DayDu</code> (đã nối sẵn danh mục, thương hiệu, phiếu ký gửi, khách hàng,
nhân viên). Các tham số NULL được bỏ qua, cho phép kết hợp linh hoạt. Kết quả sắp xếp theo
ngày đăng bán và mã sản phẩm giảm dần. Mã SQL đầy đủ ở Phụ lục B.</p>""")

# --- 7 reports ---------------------------------------------------------------
PARTS.append("""<div class="brk"></div>
<h1>7. Hai báo cáo nghiệp vụ</h1>
<p>Cả hai báo cáo đều xem được trên màn hình, có thẻ tổng hợp, bảng chi tiết và nút
<b>Xuất Excel (.xlsx)</b> bằng ClosedXML. Mỗi báo cáo ghi rõ <b>người lập</b> và <b>người nhận</b>
ngay trên màn hình và trong tệp Excel xuất ra.</p>

<h2>7.1 Báo cáo doanh thu theo tháng</h2>
<ul>
<li><b>Người lập:</b> Nhân viên kế toán (thu ngân).</li>
<li><b>Người nhận:</b> Quản lý cửa hàng / Ban giám đốc.</li>
<li><b>Mục đích:</b> theo dõi doanh thu, hoa hồng 20% và số tiền phải trả người ký gửi theo từng tháng;
chỉ tính các đơn đã Hoàn tất.</li>
<li><b>Nội dung:</b> thẻ tổng doanh thu, tổng hoa hồng, tổng tiền trả người ký gửi, số đơn hoàn tất;
biểu đồ cột theo tháng và bảng chi tiết từng tháng (số đơn, số sản phẩm, doanh thu, hoa hồng, trả ký gửi).</li>
<li><b>Nguồn dữ liệu:</b> <code>sp_BaoCaoDoanhThu</code> / view <code>vw_DoanhThu_TheoThang</code>.</li>
<li><b>Tệp mẫu:</b> <code>exports/bao-cao-doanh-thu_*.xlsx</code>.</li>
</ul>
""")
PARTS.append(img("05-bao-cao-doanh-thu.png", "Hình 7.1 — Báo cáo doanh thu theo tháng kèm biểu đồ và bảng chi tiết"))

PARTS.append("""
<h2>7.2 Báo cáo quyết toán người ký gửi</h2>
<ul>
<li><b>Người lập:</b> Nhân viên kế toán.</li>
<li><b>Người nhận:</b> Người ký gửi (khách nhận tiền) và Quản lý cửa hàng.</li>
<li><b>Mục đích:</b> đối chiếu tiền bán, hoa hồng và số tiền phải trả từng người ký gửi;
lọc theo người gửi, trạng thái quyết toán và khoảng ngày bán.</li>
<li><b>Nội dung:</b> thẻ tổng giá bán, hoa hồng, tiền phải trả, số sản phẩm đã bán; bảng chi tiết
từng sản phẩm (đơn, ngày bán, người gửi, giá bán, hoa hồng, thực nhận, trạng thái quyết toán);
khu vực lập phiếu quyết toán ngay trên màn hình và danh sách phiếu đang chờ chi.</li>
<li><b>Nguồn dữ liệu:</b> <code>sp_BaoCaoQuyetToan</code> / view <code>vw_QuyetToan_SanPham</code>.</li>
<li><b>Tệp mẫu:</b> <code>exports/bao-cao-quyet-toan_*.xlsx</code>.</li>
</ul>
""")
PARTS.append(img("06-bao-cao-quyet-toan.png", "Hình 7.2 — Báo cáo quyết toán người ký gửi và khu vực lập phiếu chi"))

PARTS.append("""
<h2>7.3 Quy tắc xử lý trả thừa</h2>
<p>Khi khách thanh toán vượt giá trị đơn, hệ thống <b>coi như đã trả đủ</b>: trigger
<code>TR_THANH_TOAN</code> so sánh tổng đã thu với tổng thành tiền và chuyển đơn sang
<i>Đang xử lý</i> ngay khi đã thu đủ hoặc thừa; sản phẩm chuyển <i>Đã bán</i>. Phần chênh lệch
không bị chặn mà được đưa vào view đối soát <code>vw_DoiSoat_ThanhToan</code> với cột
<code>ChenhLech</code> và <code>KetQuaDoiSoat = N'Trả thừa'</code> để kế toán hoàn trả riêng.</p>
<div class="note"><b>Ví dụ dữ liệu mẫu:</b> đơn DH20 có tổng tiền 4.950.000&nbsp;đ, khách đã trả
5.000.000&nbsp;đ. Đơn được ghi nhận Hoàn tất, chênh lệch 50.000&nbsp;đ xuất hiện trong báo cáo
đối soát với kết quả “Trả thừa”.</div>
""")

# --- Appendix A --------------------------------------------------------------
PARTS.append("""<div class="brk"></div>
<h1>Phụ lục A. Danh mục sản phẩm bàn giao</h1>
""")
PARTS.append(table(
    ["Đường dẫn", "Nội dung"],
    [
        ["db/00_create_database.sql", "Tạo database với collation Vietnamese_CI_AS"],
        ["db/01_schema.sql", "11 bảng, khóa, CHECK, cột tính toán, chỉ mục"],
        ["db/02_triggers.sql", "6 trigger và hàm trạng thái phiếu"],
        ["db/03_views.sql", "9 view phục vụ tra cứu, báo cáo, đối soát"],
        ["db/04_procedures.sql", "13 stored procedure và 3 table type"],
        ["db/05_seed_master.sql", "Dữ liệu danh mục: khách hàng, nhân viên, thương hiệu, danh mục"],
        ["db/06_seed_operations.sql", "Dữ liệu giao dịch: phiếu, sản phẩm, đơn, thanh toán, quyết toán"],
        ["db/07_validation.sql", "18 bất biến nghiệp vụ và thống kê dữ liệu"],
        ["db/08_negative_tests.sql", "23 kịch bản vi phạm, tất cả bị chặn"],
        ["db/09_backup.sql", "Sao lưu database ra tệp .bak"],
        ["db/10_restore_database.sql", "Khôi phục và kiểm tra bản sao lưu"],
        ["bak/QuanLyKyGuiThoiTrang_Full_20261115.bak", "Bản sao lưu đã kiểm tra RESTORE VERIFYONLY"],
        ["app/QuanLyKyGui/", "Project C# WinForms (.NET 9) đầy đủ mã nguồn"],
        ["docs/ERD_Chen.drawio", "ERD chỉnh sửa được (draw.io)"],
        ["docs/ERD_Chen.png", "ERD dạng ảnh"],
        ["docs/3NF_Analysis.md", "Phân tích chuẩn hóa 3NF chi tiết"],
        ["docs/BaoCao_BaiTapLon.docx", "Báo cáo đồ án (tệp này)"],
        ["screenshots/01-main.png … 06-bao-cao-quyet-toan.png", "6 ảnh chụp màn hình minh họa"],
        ["exports/*.xlsx", "3 tệp Excel xuất từ ứng dụng (tra cứu, doanh thu, quyết toán)"],
        ["README.md", "Hướng dẫn cài đặt, khôi phục và chạy"],
    ]))

# --- Appendix B SQL ----------------------------------------------------------
PARTS.append("""<div class="brk"></div>
<h1>Phụ lục B. Mã SQL cho mục 5 – 7</h1>
<p>Toàn bộ mã T-SQL dưới đây là bản chính xác đang cài đặt trong database
<code>QuanLyKyGuiThoiTrang</code>: trigger và hàm phục vụ kiểm tra ràng buộc (mục 5),
procedure tra cứu (mục 6) và view/procedure báo cáo (mục 7).</p>
""")

PARTS.append(sql_block("B.1 — 02_triggers.sql (ràng buộc nghiệp vụ cho màn hình nhập liệu)",
                       read_sql("02_triggers.sql")))
PARTS.append('<div class="brk"></div>')
PARTS.append(sql_block("B.2 — 03_views.sql (nguồn dữ liệu cho tra cứu và báo cáo)",
                       read_sql("03_views.sql")))
PARTS.append('<div class="brk"></div>')
PARTS.append(sql_block("B.3 — 04_procedures.sql (nhập liệu, tra cứu, báo cáo)",
                       read_sql("04_procedures.sql")))

HTML_DOC = f"""<!DOCTYPE html>
<html lang="vi"><head><meta charset="utf-8" />
<title>Bao cao do an CSDL - QuanLyKyGuiThoiTrang</title>
<style>{CSS}</style></head>
<body>
{''.join(PARTS)}
</body></html>"""

# Word bỏ qua div rỗng có page-break-before -> gắn break trực tiếp vào phần tử kế tiếp
HTML_DOC = HTML_DOC.replace('<div class="brk"></div>\n<h1', '<h1 class="brk"')
HTML_DOC = HTML_DOC.replace('<div class="brk"></div>\n<p class="sqlcap"',
                            '<p class="brk sqlcap"')
HTML_DOC = HTML_DOC.replace('<div class="brk"></div>\n<p><b>',
                            '<p class="brk"><b>')

with open(OUT_HTML, "w", encoding="utf-8") as f:
    f.write(HTML_DOC)

print("HTML:", OUT_HTML, os.path.getsize(OUT_HTML), "bytes")

# Chuyển HTML -> DOCX bằng Word COM
ps = r'''
$ErrorActionPreference = "Stop"
$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.DisplayAlerts = 0
try {
    $doc = $word.Documents.Open("__HTML__", [ref]$false, [ref]$false)
    $doc.SaveAs2("__DOCX__", 16)
    $doc.Close(0)
    Write-Output "SAVED __DOCX__"
} finally {
    $word.Quit()
    [System.Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null
}
'''.replace("__HTML__", OUT_HTML).replace("__DOCX__", OUT_DOCX)

ps_file = os.path.join(BASE, "tools", "_convert_docx.ps1")
with open(ps_file, "w", encoding="utf-8-sig") as f:
    f.write(ps)

r = subprocess.run(["powershell", "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", ps_file],
                   capture_output=True, text=True, encoding="utf-8", errors="replace")
print("STDOUT:", r.stdout.strip())
if r.stderr.strip():
    print("STDERR:", r.stderr.strip())
print("DOCX exists:", os.path.exists(OUT_DOCX), os.path.getsize(OUT_DOCX) if os.path.exists(OUT_DOCX) else 0)
