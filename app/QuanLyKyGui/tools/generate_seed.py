# -*- coding: utf-8 -*-
"""Sinh 05_seed_master.sql va 06_seed_operations.sql, co kiem tra tinh nhat quan."""
import os
import sys
from datetime import datetime, timedelta

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import seed_master as M
import seed_products_a as PA
import seed_products_b as PB
import seed_ops as O

OUT = r"E:\Quanap\db"
COND = PA.CONDITIONS
APPROVAL = {"A": "Đã chấp nhận", "P": "Chờ xử lý", "R": "Từ chối"}
SALE = {"S": "Đã bán", "H": "Đang giữ", "B": "Đang bán", "U": "Chưa đăng bán", "T": "Đã trả lại", "N": None}

PRODUCTS = PA.PRODUCTS_A + PB.PRODUCTS_B


def n(v):
    if v is None:
        return "NULL"
    return "N'" + str(v).replace("'", "''") + "'"


def s(v):
    if v is None:
        return "NULL"
    return "'" + str(v).replace("'", "''") + "'"


def dt(v):
    if v is None:
        return "NULL"
    if isinstance(v, str):
        return "CAST('" + v.replace("T", " ") + "' AS DATETIME2(0))"
    return "CAST('" + v.strftime("%Y-%m-%d %H:%M:%S") + "' AS DATETIME2(0))"


def parse(v):
    return datetime.strptime(v, "%Y-%m-%d %H:%M")


# ---------- Kiem tra du lieu ----------
assert len(PRODUCTS) == 72, len(PRODUCTS)
assert len({p[0] for p in PRODUCTS}) == 72
assert len(M.SLIPS) == 28
slip_by_id = {x[0]: x for x in M.SLIPS}
prod_by_id = {p[0]: p for p in PRODUCTS}
cat_ids = {c[0] for c in M.CATEGORIES}
brand_ids = {b[0] for b in M.BRANDS}
for p in PRODUCTS:
    assert p[1] in slip_by_id, p
    assert p[2] in cat_ids, p
    assert p[3] is None or p[3] in brand_ids, p
    assert p[11] in APPROVAL and p[12] in SALE, p
    if p[11] != "A":
        assert p[10] is None, p
    if p[11] == "A":
        assert p[10] and p[10] > 0, p

# CTDH
ctdh_map = {}
ctdh_rows = []
ctdh_id = 1
for o in O.ORDERS:
    oid, odate, okh, onv, ostatus, days, note, reason, items = o
    assert ostatus in ("Hoàn tất", "Đang xử lý", "Chờ thanh toán", "Đã hủy")
    if ostatus == "Đã hủy":
        assert reason, oid
    else:
        assert reason is None, oid
    for pid, disc in items:
        p = prod_by_id[pid]
        assert disc <= p[10], (oid, pid)
        ctdh_map[ctdh_id] = (oid, pid, p[10], disc)
        ctdh_rows.append(ctdh_id)
        ctdh_id += 1

# active usage
active_orders = {o[0] for o in O.ORDERS if o[4] != "Đã hủy"}
prod_active = {}
for cid, (oid, pid, price, disc) in ctdh_map.items():
    if oid in active_orders:
        assert pid not in prod_active, ("san pham trong 2 don active", pid, oid, prod_active.get(pid))
        prod_active[pid] = oid

for p in PRODUCTS:
    pid, sale = p[0], p[12]
    if sale == "S":
        assert pid in prod_active, ("san pham da ban chua co don", pid)
        assert O.ORDERS[[x[0] for x in O.ORDERS].index(prod_active[pid])][4] in ("Hoàn tất", "Đang xử lý")
    elif sale == "H":
        assert pid in prod_active, ("san pham dang giu chua co don", pid)
        oid = prod_active[pid]
        assert next(x for x in O.ORDERS if x[0] == oid)[4] == "Chờ thanh toán"
    else:
        assert pid not in prod_active, ("san pham khong ban duoc nam trong don", pid)

# payments
order_by_id = {o[0]: o for o in O.ORDERS}
total_by_order = {}
for cid, (oid, pid, price, disc) in ctdh_map.items():
    total_by_order[oid] = total_by_order.get(oid, 0) + price - disc
paid_by_order = {}
for pay in O.PAYMENTS:
    oid, amount, method, status, created, code, note = pay
    assert oid in order_by_id
    if status == "Thành công":
        paid_by_order[oid] = paid_by_order.get(oid, 0) + amount

for o in O.ORDERS:
    oid, st = o[0], o[4]
    total = total_by_order.get(oid, 0)
    paid = paid_by_order.get(oid, 0)
    if st in ("Hoàn tất", "Đang xử lý"):
        assert paid >= total, (oid, total, paid)
    elif st == "Chờ thanh toán":
        assert paid < total, (oid, total, paid)
    elif st == "Đã hủy":
        assert paid == 0, (oid, paid)
        assert not any(p[0] == oid and p[3] == "Chờ xử lý" for p in O.PAYMENTS), oid

# settlements
assert len(O.SETTLEMENTS) == 18
detail_ids = []
settle_detail_count = 0
for st in O.SETTLEMENTS:
    sid, kh, nv, method, status, lap, details, note = st
    assert status in ("Chờ thanh toán", "Đã thanh toán")
    assert method in ("Tiền mặt", "Chuyển khoản")
    assert details
    max_order_date = None
    for cid in details:
        assert cid in ctdh_map, cid
        oid, pid, price, disc = ctdh_map[cid]
        assert order_by_id[oid][4] == "Hoàn tất", (sid, cid)
        slip = slip_by_id[prod_by_id[pid][1]]
        assert slip[2] == kh, (sid, cid, "sai nguoi ky gui")
        od = parse(order_by_id[oid][1])
        max_order_date = od if max_order_date is None else max(max_order_date, od)
        detail_ids.append(cid)
    assert parse(lap) >= max_order_date, (sid, lap, max_order_date)
assert len(detail_ids) == len(set(detail_ids))
settle_detail_count = len(detail_ids)

# ---------- SQL ----------
def master_sql():
    L = []
    a = L.append
    a("USE [QuanLyKyGuiThoiTrang];")
    a("GO")
    a("SET ANSI_NULLS ON;")
    a("SET QUOTED_IDENTIFIER ON;")
    a("SET NOCOUNT ON;")
    a("SET XACT_ABORT ON;")
    a("GO")
    a("BEGIN TRANSACTION;")
    a("DELETE FROM dbo.CHI_TIET_THANH_TOAN_KY_GUI;")
    a("DELETE FROM dbo.PHIEU_THANH_TOAN_KY_GUI;")
    a("DELETE FROM dbo.THANH_TOAN_DON_HANG;")
    a("DELETE FROM dbo.CHI_TIET_DON_HANG;")
    a("DELETE FROM dbo.DON_HANG;")
    a("DELETE FROM dbo.SAN_PHAM;")
    a("DELETE FROM dbo.PHIEU_KY_GUI;")
    a("DELETE FROM dbo.KHACH_HANG;")
    a("DELETE FROM dbo.NHAN_VIEN;")
    a("DELETE FROM dbo.THUONG_HIEU;")
    a("DELETE FROM dbo.DANH_MUC;")
    a("GO")
    a("DBCC CHECKIDENT ('dbo.KHACH_HANG', RESEED, 0) WITH NO_INFOMSGS;")
    a("DBCC CHECKIDENT ('dbo.NHAN_VIEN', RESEED, 0) WITH NO_INFOMSGS;")
    a("DBCC CHECKIDENT ('dbo.THUONG_HIEU', RESEED, 0) WITH NO_INFOMSGS;")
    a("DBCC CHECKIDENT ('dbo.DANH_MUC', RESEED, 0) WITH NO_INFOMSGS;")
    a("GO")
    a("SET IDENTITY_INSERT dbo.NHAN_VIEN ON;")
    a("INSERT dbo.NHAN_VIEN (MaNV, HoTen, SoDienThoai, Email, ChucVu, TrangThai) VALUES")
    vals = []
    for e in M.EMPLOYEES:
        vals.append("(" + ", ".join([str(e[0]), n(e[1]), s(e[2]), s(e[3]), n(e[4]), n(e[5])]) + ")")
    a(",\n".join(vals) + ";")
    a("SET IDENTITY_INSERT dbo.NHAN_VIEN OFF;")
    a("SET IDENTITY_INSERT dbo.KHACH_HANG ON;")
    a("INSERT dbo.KHACH_HANG (MaKH, HoTen, SoDienThoai, Email, DiaChi) VALUES")
    vals = []
    for c in M.CUSTOMERS:
        vals.append("(" + ", ".join([str(c[0]), n(c[1]), s(c[2]), s(c[3]), n(c[4])]) + ")")
    a(",\n".join(vals) + ";")
    a("SET IDENTITY_INSERT dbo.KHACH_HANG OFF;")
    a("SET IDENTITY_INSERT dbo.THUONG_HIEU ON;")
    a("INSERT dbo.THUONG_HIEU (MaTH, TenThuongHieu, MoTa) VALUES")
    vals = []
    for b in M.BRANDS:
        vals.append("(" + ", ".join([str(b[0]), n(b[1]), n(b[2])]) + ")")
    a(",\n".join(vals) + ";")
    a("SET IDENTITY_INSERT dbo.THUONG_HIEU OFF;")
    a("SET IDENTITY_INSERT dbo.DANH_MUC ON;")
    a("INSERT dbo.DANH_MUC (MaDM, TenDanhMuc, MoTa) VALUES")
    vals = []
    for c in M.CATEGORIES:
        vals.append("(" + ", ".join([str(c[0]), n(c[1]), n(c[2])]) + ")")
    a(",\n".join(vals) + ";")
    a("SET IDENTITY_INSERT dbo.DANH_MUC OFF;")
    a("COMMIT TRANSACTION;")
    a("GO")
    a("PRINT N'Seed master: 8 nhan vien, 24 khach hang, 10 thuong hieu, 6 danh muc.';")
    a("GO")
    return "\n".join(L)


def operations_sql():
    L = []
    a = L.append
    a("USE [QuanLyKyGuiThoiTrang];")
    a("GO")
    a("SET ANSI_NULLS ON;")
    a("SET QUOTED_IDENTIFIER ON;")
    a("SET NOCOUNT ON;")
    a("SET XACT_ABORT ON;")
    a("GO")
    a("ALTER TABLE dbo.CHI_TIET_THANH_TOAN_KY_GUI DISABLE TRIGGER ALL;")
    a("ALTER TABLE dbo.PHIEU_THANH_TOAN_KY_GUI DISABLE TRIGGER ALL;")
    a("ALTER TABLE dbo.THANH_TOAN_DON_HANG DISABLE TRIGGER ALL;")
    a("ALTER TABLE dbo.CHI_TIET_DON_HANG DISABLE TRIGGER ALL;")
    a("ALTER TABLE dbo.DON_HANG DISABLE TRIGGER ALL;")
    a("ALTER TABLE dbo.SAN_PHAM DISABLE TRIGGER ALL;")
    a("GO")
    a("BEGIN TRANSACTION;")
    a("DELETE FROM dbo.CHI_TIET_THANH_TOAN_KY_GUI;")
    a("DELETE FROM dbo.PHIEU_THANH_TOAN_KY_GUI;")
    a("DELETE FROM dbo.THANH_TOAN_DON_HANG;")
    a("DELETE FROM dbo.CHI_TIET_DON_HANG;")
    a("DELETE FROM dbo.DON_HANG;")
    a("DELETE FROM dbo.SAN_PHAM;")
    a("DELETE FROM dbo.PHIEU_KY_GUI;")
    a("GO")
    a("DBCC CHECKIDENT ('dbo.PHIEU_KY_GUI', RESEED, 0) WITH NO_INFOMSGS;")
    a("DBCC CHECKIDENT ('dbo.SAN_PHAM', RESEED, 0) WITH NO_INFOMSGS;")
    a("DBCC CHECKIDENT ('dbo.DON_HANG', RESEED, 0) WITH NO_INFOMSGS;")
    a("DBCC CHECKIDENT ('dbo.CHI_TIET_DON_HANG', RESEED, 0) WITH NO_INFOMSGS;")
    a("DBCC CHECKIDENT ('dbo.THANH_TOAN_DON_HANG', RESEED, 0) WITH NO_INFOMSGS;")
    a("DBCC CHECKIDENT ('dbo.PHIEU_THANH_TOAN_KY_GUI', RESEED, 0) WITH NO_INFOMSGS;")
    a("DBCC CHECKIDENT ('dbo.CHI_TIET_THANH_TOAN_KY_GUI', RESEED, 0) WITH NO_INFOMSGS;")
    a("GO")

    # Phieu ky gui
    a("SET IDENTITY_INSERT dbo.PHIEU_KY_GUI ON;")
    a("INSERT dbo.PHIEU_KY_GUI (MaPKG, MaKH, MaNV, NgayKyGui, TrangThai, GhiChu) VALUES")
    vals = []
    for x in M.SLIPS:
        vals.append("(" + ", ".join([str(x[0]), str(x[2]), str(x[3]), dt(x[1]), n("Đang xử lý"), n(x[4])]) + ")")
    a(",\n".join(vals) + ";")
    a("SET IDENTITY_INSERT dbo.PHIEU_KY_GUI OFF;")

    # San pham
    a("SET IDENTITY_INSERT dbo.SAN_PHAM ON;")
    a("INSERT dbo.SAN_PHAM (MaSP, MaPKG, MaDM, MaTH, TenSP, MoTa, KichThuoc, MauSac, ChatLieu, TinhTrang, GiaDeXuat, GiaDangBan, TrangThaiTiepNhan, TrangThaiBan, NgayDangBan) VALUES")
    vals = []
    for p in PRODUCTS:
        pid, pkg, dm, th, name, size, color, material, cond, proposed, listed, appr, sale = p
        desc = name + ". Màu " + color + ", chất liệu " + material + ", kích thước " + size + ". " + COND[cond] + "."
        listing = None
        if sale in ("S", "H", "B", "T"):
            listing = (parse(slip_by_id[pkg][1]) + timedelta(days=2)).strftime("%Y-%m-%d 09:00")
        vals.append("(" + ", ".join([
            str(pid), str(pkg), str(dm), str(th) if th else "NULL", n(name), n(desc), n(size), n(color), n(material), n(COND[cond]),
            str(proposed), str(listed) if listed else "NULL", n(APPROVAL[appr]), n(SALE[sale]), dt(listing)
        ]) + ")")
    a(",\n".join(vals) + ";")
    a("SET IDENTITY_INSERT dbo.SAN_PHAM OFF;")

    # Don hang
    a("SET IDENTITY_INSERT dbo.DON_HANG ON;")
    a("INSERT dbo.DON_HANG (MaDH, MaKH, MaNV, NgayLap, HanThanhToan, TrangThai, LyDoHuy, GhiChu) VALUES")
    vals = []
    for o in O.ORDERS:
        oid, odate, okh, onv, st, days, note, reason, items = o
        han = (parse(odate) + timedelta(days=days)).strftime("%Y-%m-%d %H:%M")
        vals.append("(" + ", ".join([str(oid), str(okh), str(onv), dt(odate), dt(han), n(st), n(reason), n(note)]) + ")")
    a(",\n".join(vals) + ";")
    a("SET IDENTITY_INSERT dbo.DON_HANG OFF;")

    # CTDH
    a("SET IDENTITY_INSERT dbo.CHI_TIET_DON_HANG ON;")
    a("INSERT dbo.CHI_TIET_DON_HANG (MaCTDH, MaDH, MaSP, GiaNiemYetLucBan, GiamGia) VALUES")
    vals = []
    for cid in sorted(ctdh_map):
        oid, pid, price, disc = ctdh_map[cid]
        vals.append("(" + ", ".join([str(cid), str(oid), str(pid), str(price), str(disc)]) + ")")
    a(",\n".join(vals) + ";")
    a("SET IDENTITY_INSERT dbo.CHI_TIET_DON_HANG OFF;")

    # Thanh toan, sap xep theo thoi gian
    pays = sorted(O.PAYMENTS, key=lambda x: parse(x[4]))
    a("SET IDENTITY_INSERT dbo.THANH_TOAN_DON_HANG ON;")
    a("INSERT dbo.THANH_TOAN_DON_HANG (MaTTDH, MaDH, SoTien, NgayTao, NgayHoanTat, PhuongThuc, TrangThai, MaGiaoDich, GhiChu) VALUES")
    vals = []
    for i, pay in enumerate(pays, 1):
        oid, amount, method, status, created, code, note = pay
        done = created if status == "Thành công" else None
        vals.append("(" + ", ".join([str(i), str(oid), str(amount), dt(created), dt(done), n(method), n(status), s(code), n(note)]) + ")")
    a(",\n".join(vals) + ";")
    a("SET IDENTITY_INSERT dbo.THANH_TOAN_DON_HANG OFF;")

    # PTTKG
    a("SET IDENTITY_INSERT dbo.PHIEU_THANH_TOAN_KY_GUI ON;")
    a("INSERT dbo.PHIEU_THANH_TOAN_KY_GUI (MaPTTKG, MaKH, MaNV, NgayLap, NgayThanhToan, PhuongThuc, TrangThai, MaGiaoDich, GhiChu) VALUES")
    vals = []
    for st in O.SETTLEMENTS:
        sid, kh, nv, method, status, lap, details, note = st
        paid = None
        code = None
        if status == "Đã thanh toán":
            paid = (parse(lap) + timedelta(days=1, hours=1)).strftime("%Y-%m-%d %H:%M")
            code = "PC" + parse(lap).strftime("%y%m%d") + str(sid).zfill(3)
        vals.append("(" + ", ".join([str(sid), str(kh), str(nv), dt(lap), dt(paid), n(method), n(status), s(code), n(note)]) + ")")
    a(",\n".join(vals) + ";")
    a("SET IDENTITY_INSERT dbo.PHIEU_THANH_TOAN_KY_GUI OFF;")

    # CTTTKG
    a("SET IDENTITY_INSERT dbo.CHI_TIET_THANH_TOAN_KY_GUI ON;")
    a("INSERT dbo.CHI_TIET_THANH_TOAN_KY_GUI (MaCTTTKG, MaPTTKG, MaCTDH, GiaBanTinhToan) VALUES")
    vals = []
    ctid = 1
    for st in O.SETTLEMENTS:
        sid = st[0]
        for cid in st[6]:
            _, _, price, disc = ctdh_map[cid]
            vals.append("(" + ", ".join([str(ctid), str(sid), str(cid), str(price - disc)]) + ")")
            ctid += 1
    a(",\n".join(vals) + ";")
    a("SET IDENTITY_INSERT dbo.CHI_TIET_THANH_TOAN_KY_GUI OFF;")

    a("COMMIT TRANSACTION;")
    a("GO")
    a("ALTER TABLE dbo.CHI_TIET_THANH_TOAN_KY_GUI ENABLE TRIGGER ALL;")
    a("ALTER TABLE dbo.PHIEU_THANH_TOAN_KY_GUI ENABLE TRIGGER ALL;")
    a("ALTER TABLE dbo.THANH_TOAN_DON_HANG ENABLE TRIGGER ALL;")
    a("ALTER TABLE dbo.CHI_TIET_DON_HANG ENABLE TRIGGER ALL;")
    a("ALTER TABLE dbo.DON_HANG ENABLE TRIGGER ALL;")
    a("ALTER TABLE dbo.SAN_PHAM ENABLE TRIGGER ALL;")
    a("GO")
    a("UPDATE p")
    a("SET TrangThai = dbo.fn_TrangThaiPhieuKyGui(p.MaPKG)")
    a("FROM dbo.PHIEU_KY_GUI p")
    a("WHERE p.TrangThai <> dbo.fn_TrangThaiPhieuKyGui(p.MaPKG);")
    a("GO")
    a("PRINT N'Seed operations: 28 phieu, 72 san pham, 32 don, " + str(len(O.PAYMENTS)) + " giao dich, 18 phieu quyet toan, " + str(settle_detail_count) + " chi tiet.';")
    a("GO")
    return "\n".join(L)


with open(os.path.join(OUT, "05_seed_master.sql"), "w", encoding="utf-8", newline="\r\n") as f:
    f.write(master_sql())
with open(os.path.join(OUT, "06_seed_operations.sql"), "w", encoding="utf-8", newline="\r\n") as f:
    f.write(operations_sql())

print("OK products=%d orders=%d ctdh=%d payments=%d settlements=%d details=%d" % (
    len(PRODUCTS), len(O.ORDERS), len(ctdh_map), len(O.PAYMENTS), len(O.SETTLEMENTS), settle_detail_count))
