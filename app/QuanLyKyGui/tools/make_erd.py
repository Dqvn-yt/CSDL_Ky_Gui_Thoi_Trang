# -*- coding: utf-8 -*-
"""Sinh ERD cho CSDL QuanLyKyGuiThoiTrang: file draw.io (editable) + PNG."""
import os
from xml.sax.saxutils import escape

import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
from matplotlib.patches import Rectangle
from matplotlib.lines import Line2D

BASE = r"E:\Quanap\docs"
os.makedirs(BASE, exist_ok=True)

W = 300          # table width
HEAD = 32        # header height
ROW = 22         # row height

# ten, danh sach cot (tag, ten, kieu), x, y
TABLES = [
    ("KHACH_HANG", [
        ("PK", "MaKH", "INT"),
        ("UQ", "SoDienThoai", "VARCHAR(20)"),
        ("", "HoTen", "NVARCHAR(100)"),
        ("", "Email", "VARCHAR(254)"),
        ("", "DiaChi", "NVARCHAR(255)"),
        ("", "NgayTao", "DATETIME2"),
    ], 40, 110),
    ("NHAN_VIEN", [
        ("PK", "MaNV", "INT"),
        ("", "HoTen", "NVARCHAR(100)"),
        ("", "SoDienThoai", "VARCHAR(20)"),
        ("", "Email", "VARCHAR(254)"),
        ("", "ChucVu", "NVARCHAR(20)"),
        ("", "TrangThai", "NVARCHAR(20)"),
    ], 400, 110),
    ("THUONG_HIEU", [
        ("PK", "MaTH", "INT"),
        ("UQ", "TenThuongHieu", "NVARCHAR(100)"),
        ("", "MoTa", "NVARCHAR(500)"),
    ], 760, 110),
    ("DANH_MUC", [
        ("PK", "MaDM", "INT"),
        ("UQ", "TenDanhMuc", "NVARCHAR(100)"),
        ("", "MoTa", "NVARCHAR(500)"),
    ], 760, 260),
    ("PHIEU_KY_GUI", [
        ("PK", "MaPKG", "INT"),
        ("FK", "MaKH", "INT"),
        ("FK", "MaNV", "INT"),
        ("", "NgayKyGui", "DATETIME2"),
        ("", "TrangThai", "NVARCHAR(20)"),
        ("", "GhiChu", "NVARCHAR(500)"),
    ], 40, 370),
    ("SAN_PHAM", [
        ("PK", "MaSP", "INT"),
        ("FK", "MaPKG", "INT"),
        ("FK", "MaDM", "INT"),
        ("FK", "MaTH", "INT"),
        ("", "TenSP", "NVARCHAR(200)"),
        ("", "KichThuoc", "NVARCHAR(50)"),
        ("", "MauSac", "NVARCHAR(50)"),
        ("", "ChatLieu", "NVARCHAR(50)"),
        ("", "TinhTrang", "NVARCHAR(100)"),
        ("", "GiaDeXuat", "DECIMAL(12,0)"),
        ("", "GiaDangBan", "DECIMAL(12,0)"),
        ("", "TrangThaiTiepNhan", "NVARCHAR(20)"),
        ("", "TrangThaiBan", "NVARCHAR(20)"),
        ("", "NgayDangBan", "DATETIME2"),
    ], 400, 370),
    ("DON_HANG", [
        ("PK", "MaDH", "INT"),
        ("FK", "MaKH", "INT"),
        ("FK", "MaNV", "INT"),
        ("", "NgayLap", "DATETIME2"),
        ("", "HanThanhToan", "DATETIME2"),
        ("", "TrangThai", "NVARCHAR(20)"),
        ("", "LyDoHuy", "NVARCHAR(500)"),
        ("", "GhiChu", "NVARCHAR(500)"),
    ], 760, 510),
    ("CHI_TIET_DON_HANG", [
        ("PK", "MaCTDH", "INT"),
        ("FK", "MaDH", "INT"),
        ("FK", "MaSP", "INT"),
        ("", "GiaNiemYetLucBan", "DECIMAL(12,0)"),
        ("", "GiamGia", "DECIMAL(12,0)"),
        ("TINH", "GiaBanThucTe", "DECIMAL(12,0)"),
    ], 400, 830),
    ("THANH_TOAN_DON_HANG", [
        ("PK", "MaTTDH", "INT"),
        ("FK", "MaDH", "INT"),
        ("", "SoTien", "DECIMAL(12,0)"),
        ("", "NgayTao", "DATETIME2"),
        ("", "NgayHoanTat", "DATETIME2"),
        ("", "PhuongThuc", "NVARCHAR(20)"),
        ("", "TrangThai", "NVARCHAR(20)"),
        ("", "MaGiaoDich", "VARCHAR(100)"),
        ("", "GhiChu", "NVARCHAR(500)"),
    ], 760, 830),
    ("PHIEU_THANH_TOAN_KY_GUI", [
        ("PK", "MaPTTKG", "INT"),
        ("FK", "MaKH", "INT"),
        ("FK", "MaNV", "INT"),
        ("", "NgayLap", "DATETIME2"),
        ("", "NgayThanhToan", "DATETIME2"),
        ("", "PhuongThuc", "NVARCHAR(20)"),
        ("", "TrangThai", "NVARCHAR(20)"),
        ("", "MaGiaoDich", "VARCHAR(100)"),
        ("", "GhiChu", "NVARCHAR(500)"),
    ], 40, 830),
    ("CHI_TIET_THANH_TOAN_KY_GUI", [
        ("PK", "MaCTTTKG", "INT"),
        ("FK", "MaPTTKG", "INT"),
        ("UQ", "MaCTDH", "INT"),
        ("", "GiaBanTinhToan", "DECIMAL(12,0)"),
        ("TINH", "TienHoaHong", "DECIMAL(12,0)"),
        ("TINH", "TienThucNhan", "DECIMAL(12,0)"),
    ], 40, 1170),
]

# (ten, diem bat dau, cac diem giua, diem ket thuc, nhan)
EDGES = [
    ("KHACH_HANG", (190, 274), [], (190, 370), "1..N"),
    ("NHAN_VIEN", (500, 274), [(500, 320), (340, 320)], (340, 420), "1..N"),
    ("PHIEU_KY_GUI", (340, 450), [], (400, 450), "1..N"),
    ("THUONG_HIEU", (760, 170), [(730, 170), (730, 335), (650, 335)], (650, 370), "1..N"),
    ("DANH_MUC", (760, 310), [(740, 310), (740, 345), (580, 345)], (580, 370), "1..N"),
    ("KHACH_HANG", (340, 240), [(370, 240), (370, 770), (900, 770)], (900, 718), "1..N"),
    ("NHAN_VIEN", (700, 190), [(730, 190), (730, 470), (850, 470)], (850, 510), "1..N"),
    ("SAN_PHAM", (550, 710), [], (550, 830), "1..N"),
    ("DON_HANG", (850, 718), [(850, 770), (650, 770)], (650, 830), "1..N"),
    ("DON_HANG", (1000, 718), [], (1000, 830), "1..N"),
    ("KHACH_HANG", (40, 150), [(20, 150), (20, 920)], (40, 920), "1..N"),
    ("NHAN_VIEN", (400, 160), [(370, 160), (370, 910)], (340, 910), "1..N"),
    ("PHIEU_THANH_TOAN_KY_GUI", (190, 1060), [], (190, 1170), "1..N"),
    ("CHI_TIET_DON_HANG", (480, 994), [(480, 1070), (370, 1070), (370, 1250)], (340, 1250), "1..1"),
]

HEIGHT = {name: HEAD + len(cols) * ROW for name, cols, _, _ in TABLES}
POS = {name: (x, y) for name, _, x, y in TABLES}
COLS = {name: cols for name, cols, _, _ in TABLES}

FILL = {
    "PK": "#FBEAEE",
    "FK": "#EEF3FA",
    "UQ": "#FBF3DC",
    "TINH": "#EDF5EA",
    "": "#FFFFFF",
}
TAG_COLOR = {
    "PK": "#9C2A46",
    "FK": "#2E5A8A",
    "UQ": "#8A6A1F",
    "TINH": "#4F7A3A",
    "": "#000000",
}


# ----------------------------------------------------------------------------
# PNG
# ----------------------------------------------------------------------------
def draw_png(path):
    plt.rcParams["font.family"] = "Segoe UI"
    fig, ax = plt.subplots(figsize=(15.2, 15.4))
    fig.patch.set_facecolor("#FFFDF8")
    ax.set_facecolor("#FFFDF8")
    ax.set_xlim(0, 1100)
    ax.set_ylim(1420, 0)
    ax.axis("off")

    # tieu de
    ax.text(20, 34, "SƠ ĐỒ THỰC THỂ – LIÊN KẾT (ERD)", fontsize=20, weight="bold",
            color="#2B2530", ha="left", va="center")
    ax.text(20, 62, "Hệ thống quản lý sàn ký gửi đồ thời trang cũ  •  CSDL QuanLyKyGuiThoiTrang",
            fontsize=10.5, color="#6B5F57", ha="left", va="center")

    # canh
    for _, start, mids, end, label in EDGES:
        pts = [start] + list(mids) + [end]
        xs = [p[0] for p in pts]
        ys = [p[1] for p in pts]
        ax.add_line(Line2D(xs, ys, color="#7A6A60", linewidth=1.15, zorder=1,
                           solid_capstyle="round", solid_joinstyle="round"))
        draw_crow(ax, pts)
        draw_one(ax, pts)
        mx = (start[0] + end[0]) / 2
        my = (start[1] + end[1]) / 2
        if mids:
            k = len(pts) // 2
            mx = (pts[k - 1][0] + pts[k][0]) / 2
            my = (pts[k - 1][1] + pts[k][1]) / 2
        ax.text(mx + 4, my - 4, label, fontsize=7.6, color="#7A6A60",
                ha="left", va="bottom", zorder=2)

    # bang
    for name, cols, x, y in TABLES:
        h = HEIGHT[name]
        ax.add_patch(Rectangle((x, y), W, h, facecolor="#FFFFFF", edgecolor="#8E8178",
                               linewidth=1.0, zorder=3))
        ax.add_patch(Rectangle((x, y), W, HEAD, facecolor="#7C1F35", edgecolor="#5A1626",
                               linewidth=0.8, zorder=4))
        ax.text(x + W / 2, y + HEAD / 2 + 1, name, fontsize=10.2, weight="bold",
                color="#FFFFFF", ha="center", va="center", zorder=5)
        for i, (tag, col, typ) in enumerate(cols):
            ry = y + HEAD + i * ROW
            ax.add_patch(Rectangle((x, ry), W, ROW, facecolor=FILL[tag],
                                   edgecolor="#E1D9CF", linewidth=0.45, zorder=4))
            if tag:
                ax.text(x + 7, ry + ROW / 2, tag, fontsize=6.6, weight="bold",
                        color=TAG_COLOR[tag], ha="left", va="center", zorder=5)
            ax.text(x + 36, ry + ROW / 2, col, fontsize=8.0,
                    color="#2B2530" if tag in ("PK", "FK") else "#3E3731",
                    weight="bold" if tag == "PK" else "normal",
                    ha="left", va="center", zorder=5)
            ax.text(x + W - 7, ry + ROW / 2, typ, fontsize=7.0, color="#8B7F74",
                    ha="right", va="center", zorder=5)
        ax.add_patch(Rectangle((x, y), W, h, facecolor="none", edgecolor="#7C1F35",
                               linewidth=1.2, zorder=6))

    # chu thich
    legend_y = 1368
    ax.text(40, legend_y, "Chú thích:", fontsize=8.5, weight="bold", color="#2B2530")
    lx = 110
    for tag, text in [("PK", "Khóa chính"), ("FK", "Khóa ngoại"), ("UQ", "Ràng buộc duy nhất"),
                      ("TINH", "Cột tính toán (computed)")]:
        ax.add_patch(Rectangle((lx, legend_y - 8), 22, 15, facecolor=FILL[tag],
                               edgecolor="#CFC6BB", linewidth=0.6))
        ax.text(lx + 3, legend_y, tag, fontsize=6.2, weight="bold", color=TAG_COLOR[tag],
                ha="left", va="center")
        ax.text(lx + 28, legend_y, text, fontsize=8.0, color="#4A423C", ha="left", va="center")
        lx += 150
    ax.text(40, legend_y + 26, "Ký hiệu đường nối: 1 — N (một-nhiều), 1 — 1 (một-một).",
            fontsize=8.0, color="#6B5F57")

    fig.savefig(path, dpi=170, bbox_inches="tight", pad_inches=0.18,
                facecolor=fig.get_facecolor())
    plt.close(fig)


def draw_crow(ax, pts):
    (x1, y1), (x2, y2) = pts[-2], pts[-1]
    dx, dy = x2 - x1, y2 - y1
    length = max((dx * dx + dy * dy) ** 0.5, 1)
    ux, uy = dx / length, dy / length
    px, py = -uy, ux
    back = (x2 - ux * 13, y2 - uy * 13)
    for ox, oy in [(0, 0), (-px * 7, -py * 7), (px * 7, py * 7)]:
        ax.add_line(Line2D([back[0], x2 + ox], [back[1], y2 + oy],
                           color="#7A6A60", linewidth=1.0, zorder=2))


def draw_one(ax, pts):
    (x1, y1), (x2, y2) = pts[0], pts[1]
    dx, dy = x2 - x1, y2 - y1
    length = max((dx * dx + dy * dy) ** 0.5, 1)
    ux, uy = dx / length, dy / length
    px, py = -uy, ux
    cx, cy = x1 + ux * 8, y1 + uy * 8
    ax.add_line(Line2D([cx - px * 5, cx + px * 5], [cy - py * 5, cy + py * 5],
                       color="#7A6A60", linewidth=1.0, zorder=2))


# ----------------------------------------------------------------------------
# DRAW.IO
# ----------------------------------------------------------------------------
def draw_drawio(path):
    cells = []
    cells.append('<mxCell id="0" />')
    cells.append('<mxCell id="1" parent="0" />')
    cells.append(
        '<mxCell id="title" value="SƠ ĐỒ THỰC THỂ – LIÊN KẾT (ERD)" '
        'style="text;html=1;fontSize=22;fontStyle=1;align=left;verticalAlign=middle;fontColor=#2B2530;" '
        'vertex="1" parent="1"><mxGeometry x="40" y="8" width="720" height="34" as="geometry" /></mxCell>')
    cells.append(
        '<mxCell id="subtitle" value="Hệ thống quản lý sàn ký gửi đồ thời trang cũ  •  CSDL QuanLyKyGuiThoiTrang" '
        'style="text;html=1;fontSize=11;align=left;verticalAlign=middle;fontColor=#6B5F57;" '
        'vertex="1" parent="1"><mxGeometry x="40" y="42" width="760" height="24" as="geometry" /></mxCell>')

    for name, cols, x, y in TABLES:
        h = HEIGHT[name]
        tid = "t_" + name.lower()
        cells.append(
            f'<mxCell id="{tid}" value="{name}" '
            'style="shape=table;startSize=32;container=1;collapsible=0;childLayout=tableLayout;fixedRows=1;'
            'rowLines=0;fontStyle=1;align=center;resizeLast=1;html=1;fillColor=#7C1F35;fontColor=#FFFFFF;'
            'strokeColor=#5A1626;fontSize=11;fontFamily=Segoe UI;" vertex="1" parent="1">'
            f'<mxGeometry x="{x}" y="{y}" width="{W}" height="{h}" as="geometry" /></mxCell>')
        for i, (tag, col, typ) in enumerate(cols):
            rid = f"{tid}_r{i}"
            cid = f"{rid}_c"
            cells.append(
                f'<mxCell id="{rid}" value="" style="shape=tableRow;horizontal=0;startSize=0;swimlaneHead=0;'
                'swimlaneBody=0;fillColor=none;collapsible=0;dropTarget=0;points=[[0,0.5],[1,0.5]];'
                'portConstraint=eastwest;top=0;left=0;right=0;bottom=0;html=1;" vertex="1" '
                f'parent="{tid}"><mxGeometry y="{HEAD + i * ROW}" width="{W}" height="{ROW}" as="geometry" /></mxCell>')
            label = (f"{tag}  " if tag else "") + f"{col} : {typ}"
            cells.append(
                f'<mxCell id="{cid}" value="{escape(label)}" style="shape=partialRectangle;html=1;whiteSpace=wrap;'
                'connectable=0;strokeColor=#E1D9CF;overflow=hidden;fillColor=' + FILL[tag] + ';'
                'top=0;left=0;bottom=0;right=0;pointerEvents=1;fontSize=9;align=left;spacingLeft=6;'
                'fontColor=' + ("#2B2530" if tag in ("PK", "FK") else "#3E3731") + ';'
                'fontStyle=' + ("1" if tag == "PK" else "0") + ';fontFamily=Segoe UI;" vertex="1" '
                f'parent="{rid}"><mxGeometry width="{W}" height="{ROW}" as="geometry">'
                f'<mxRectangle width="{W}" height="{ROW}" as="alternateBounds" /></mxGeometry></mxCell>')

    for n, (name, start, mids, end, label) in enumerate(EDGES):
        sx, sy = POS[name]
        sh = HEIGHT[name]
        target_name = target_of(name, start, end)
        tx, ty = POS[target_name]
        th = HEIGHT[target_name]

        def norm(pt, x0, y0, w, h):
            return round((pt[0] - x0) / w, 4), round((pt[1] - y0) / h, 4)

        ex, ey = norm(start, sx, sy, W, sh)
        nx, ny = norm(end, tx, ty, W, th)
        points = "".join(f'<mxPoint x="{p[0]}" y="{p[1]}" />' for p in mids)
        points_xml = '<Array as="points">' + points + "</Array>" if points else ""
        cells.append(
            f'<mxCell id="e{n}" value="{label}" style="edgeStyle=orthogonalEdgeStyle;rounded=0;'
            'orthogonalLoop=1;jettySize=auto;html=1;strokeColor=#7A6A60;strokeWidth=1.2;'
            'startArrow=ERone;startFill=0;startSize=7;endArrow=ERmany;endFill=0;endSize=9;'
            'fontSize=9;fontColor=#7A6A60;labelBackgroundColor=#FFFDF8;'
            f'exitX={ex};exitY={ey};exitDx=0;exitDy=0;entryX={nx};entryY={ny};entryDx=0;entryDy=0;" '
            'edge="1" parent="1" '
            f'source="{("t_" + name.lower())}" target="{("t_" + target_name.lower())}">'
            f'<mxGeometry relative="1" as="geometry">{points_xml}</mxGeometry>'
            '</mxCell>')

    # chu thich
    legend = [
        ("PK", "Khóa chính"), ("FK", "Khóa ngoại"),
        ("UQ", "Ràng buộc duy nhất"), ("TINH", "Cột tính toán"),
    ]
    cells.append(
        '<mxCell id="legend_title" value="Chú thích" style="text;html=1;fontSize=11;fontStyle=1;'
        'align=left;verticalAlign=middle;fontColor=#2B2530;" vertex="1" parent="1">'
        '<mxGeometry x="40" y="1362" width="90" height="22" as="geometry" /></mxCell>')
    for i, (tag, text) in enumerate(legend):
        x = 130 + i * 160
        cells.append(
            f'<mxCell id="lg{i}" value="{escape(tag + "  " + text)}" style="rounded=0;whiteSpace=wrap;html=1;'
            f'fillColor={FILL[tag]};strokeColor=#CFC6BB;fontSize=9;align=left;spacingLeft=6;fontFamily=Segoe UI;" '
            f'vertex="1" parent="1"><mxGeometry x="{x}" y="1361" width="145" height="24" as="geometry" /></mxCell>')
    cells.append(
        '<mxCell id="legend_note" value="Ký hiệu đường nối: 1 — N (một-nhiều), 1 — 1 (một-một)." '
        'style="text;html=1;fontSize=9;align=left;verticalAlign=middle;fontColor=#6B5F57;" '
        'vertex="1" parent="1"><mxGeometry x="40" y="1394" width="440" height="20" as="geometry" /></mxCell>')

    xml = (
        '<?xml version="1.0" encoding="UTF-8"?>\n'
        '<mxfile host="app.diagrams.net" modified="2026-11-15T00:00:00.000Z" agent="OpenCode" version="24.0.0">\n'
        '  <diagram id="erd-quanlykygui" name="ERD - QuanLyKyGuiThoiTrang">\n'
        '    <mxGraphModel dx="1600" dy="1000" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" '
        'arrows="1" fold="1" page="1" pageScale="1" pageWidth="1654" pageHeight="1169" math="0" shadow="0">\n'
        '      <root>\n        ' + "\n        ".join(cells) + '\n      </root>\n'
        '    </mxGraphModel>\n  </diagram>\n</mxfile>\n'
    )
    with open(path, "w", encoding="utf-8") as f:
        f.write(xml)


def target_of(source, start, end):
    for name, _, _, _ in TABLES:
        if name == source:
            continue
        x, y = POS[name]
        h = HEIGHT[name]
        if abs(end[0] - x) < 1 or abs(end[0] - (x + W)) < 1:
            if y <= end[1] <= y + h:
                return name
        if abs(end[1] - y) < 1 or abs(end[1] - (y + h)) < 1:
            if x <= end[0] <= x + W:
                return name
    raise RuntimeError(f"Khong tim thay bang dich cho {source} {end}")


if __name__ == "__main__":
    png = os.path.join(BASE, "ERD_Chen.png")
    dio = os.path.join(BASE, "ERD_Chen.drawio")
    draw_png(png)
    draw_drawio(dio)
    print("OK", png)
    print("OK", dio)
