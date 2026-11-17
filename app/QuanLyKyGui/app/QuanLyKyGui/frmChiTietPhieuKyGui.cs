using System.Data;

namespace QuanLyKyGui;

/// <summary>Xem chi tiết một phiếu ký gửi và toàn bộ sản phẩm thuộc phiếu đó.</summary>
internal sealed class frmChiTietPhieuKyGui : Form
{
    private readonly DataGridView _grid = new();

    public frmChiTietPhieuKyGui(int maPKG)
    {
        Text = $"Phiếu ký gửi #{maPKG} — Thiên Kim Consignment";
        ClientSize = new Size(1060, 660);
        MinimumSize = new Size(920, 580);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Theme.Canvas;
        Font = Theme.Body;

        var slip = Db.Query("""
            SELECT p.MaPKG, p.NgayKyGui, p.TrangThai, p.GhiChu,
                   kh.HoTen, kh.SoDienThoai,
                   nv.HoTen AS TenNhanVien
            FROM dbo.PHIEU_KY_GUI p
            JOIN dbo.KHACH_HANG kh ON kh.MaKH = p.MaKH
            JOIN dbo.NHAN_VIEN nv ON nv.MaNV = p.MaNV
            WHERE p.MaPKG = @MaPKG;
            """, Db.Param("@MaPKG", maPKG));

        var products = Db.Query("""
            SELECT MaSP, TenSP, TenDanhMuc, TenThuongHieu, KichThuoc, MauSac, ChatLieu,
                   TinhTrang, GiaDeXuat, GiaDangBan, TrangThaiTiepNhan, TrangThaiBan, NgayDangBan
            FROM dbo.vw_SanPham_DayDu
            WHERE MaPKG = @MaPKG
            ORDER BY MaSP;
            """, Db.Param("@MaPKG", maPKG));

        var row = slip.Rows.Count > 0 ? slip.Rows[0] : null;
        var customer = row is null ? "(không tìm thấy phiếu)" : Convert.ToString(row["HoTen"]);
        var phone = row is null ? "—" : Convert.ToString(row["SoDienThoai"]);
        var employee = row is null ? "—" : Convert.ToString(row["TenNhanVien"]);
        var status = row is null ? "—" : Convert.ToString(row["TrangThai"]);
        var note = row is null || row["GhiChu"] is DBNull ? "(không có)" : Convert.ToString(row["GhiChu"]);
        var date = row is null ? "—" : Convert.ToDateTime(row["NgayKyGui"]).ToString("dd/MM/yyyy HH:mm");

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(20, 14, 20, 14),
            BackColor = Theme.Canvas
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));

        var titles = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
        titles.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        titles.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var titlePanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        titlePanel.Controls.Add(new Label
        {
            Text = $"Chi tiết phiếu ký gửi #{maPKG}",
            Font = Theme.H1,
            ForeColor = Theme.Ink,
            AutoSize = true,
            Location = new Point(0, 0)
        });
        titlePanel.Controls.Add(new Label
        {
            Text = $"{customer}  •  {date}  •  {status}",
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(2, 40)
        });
        titles.Controls.Add(titlePanel, 0, 0);
        var close = Theme.GhostButton("Đóng", 100);
        close.Click += (_, _) => Close();
        var actions = Theme.Actions(close);
        titles.Controls.Add(actions, 1, 0);
        root.Controls.Add(titles, 0, 0);

        var info = new Card("Thông tin phiếu");
        info.Dock = DockStyle.Fill;
        info.Margin = new Padding(0, 0, 0, 12);
        var infoGrid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 2, Margin = new Padding(0) };
        for (var i = 0; i < 4; i++) infoGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        infoGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        infoGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        infoGrid.Controls.Add(InfoCell("Khách hàng", customer), 0, 0);
        infoGrid.Controls.Add(InfoCell("Điện thoại", phone), 1, 0);
        infoGrid.Controls.Add(InfoCell("Ngày ký gửi", date), 2, 0);
        infoGrid.Controls.Add(InfoCell("Nhân viên tiếp nhận", employee), 3, 0);
        infoGrid.Controls.Add(InfoCell("Trạng thái phiếu", status), 0, 1);
        infoGrid.Controls.Add(InfoCell("Ghi chú", note), 1, 1);
        infoGrid.SetColumnSpan(infoGrid.GetControlFromPosition(1, 1)!, 3);
        info.Body.Controls.Add(infoGrid);
        root.Controls.Add(info, 0, 1);

        var productsCard = new Card($"Sản phẩm trong phiếu ({products.Rows.Count})");
        productsCard.Dock = DockStyle.Fill;
        productsCard.Margin = new Padding(0);
        Theme.StyleGrid(_grid);
        _grid.Dock = DockStyle.Fill;
        _grid.AutoGenerateColumns = false;
        _grid.DataSource = products;
        _grid.Columns.Add(Theme.TextColumn("MaSP", "Mã", 45));
        _grid.Columns.Add(Theme.TextColumn("TenSP", "Sản phẩm", 175));
        _grid.Columns.Add(Theme.TextColumn("TenDanhMuc", "Danh mục", 85));
        _grid.Columns.Add(Theme.TextColumn("TenThuongHieu", "Thương hiệu", 90));
        _grid.Columns.Add(Theme.TextColumn("KichThuoc", "Size", 48));
        _grid.Columns.Add(Theme.TextColumn("TinhTrang", "Tình trạng", 120));
        _grid.Columns.Add(Theme.TextColumn("GiaDeXuat", "Giá đề xuất", 88, true));
        _grid.Columns.Add(Theme.TextColumn("GiaDangBan", "Giá đăng bán", 88, true));
        _grid.Columns.Add(Theme.TextColumn("TrangThaiTiepNhan", "Duyệt", 88));
        _grid.Columns.Add(Theme.TextColumn("TrangThaiBan", "Trạng thái bán", 98));
        _grid.Columns.Add(Theme.TextColumn("NgayDangBan", "Ngày đăng", 82));
        Theme.MoneyColumn(_grid.Columns["GiaDeXuat"]);
        Theme.MoneyColumn(_grid.Columns["GiaDangBan"]);
        _grid.Columns["NgayDangBan"].DefaultCellStyle.Format = "dd/MM/yyyy";
        productsCard.Body.Controls.Add(_grid);
        root.Controls.Add(productsCard, 0, 2);

        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0, 8, 0, 0) };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var summary = new Label
        {
            AutoSize = true,
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        if (products.Rows.Count > 0)
        {
            var onSale = 0;
            var sold = 0;
            var held = 0;
            foreach (DataRow product in products.Rows)
            {
                var state = product["TrangThaiBan"] is DBNull ? "" : Convert.ToString(product["TrangThaiBan"]);
                if (state == "Đang bán") onSale++;
                else if (state == "Đã bán") sold++;
                else if (state == "Đang giữ") held++;
            }
            summary.Text = $"Tổng {products.Rows.Count} sản phẩm  •  Đang bán {onSale}  •  Đang giữ {held}  •  Đã bán {sold}";
        }
        footer.Controls.Add(summary, 0, 0);
        var closeBottom = Theme.GhostButton("Đóng", 100);
        closeBottom.Click += (_, _) => Close();
        var bottomActions = Theme.Actions(closeBottom);
        footer.Controls.Add(bottomActions, 1, 0);
        root.Controls.Add(footer, 0, 3);

        Controls.Add(root);
        CancelButton = closeBottom;
    }

    private static Control InfoCell(string caption, string? value)
    {
        var panel = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 12, 4), BackColor = Color.Transparent };
        panel.Controls.Add(new Label
        {
            Text = caption,
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(0, 0)
        });
        panel.Controls.Add(new Label
        {
            Text = string.IsNullOrWhiteSpace(value) ? "—" : value,
            Font = Theme.Bold,
            ForeColor = Theme.Ink,
            AutoSize = false,
            AutoEllipsis = true,
            Dock = DockStyle.Bottom,
            Height = 24,
            TextAlign = ContentAlignment.MiddleLeft
        });
        return panel;
    }
}
