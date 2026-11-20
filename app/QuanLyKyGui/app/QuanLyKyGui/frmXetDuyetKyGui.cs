using System.Data;

namespace QuanLyKyGui;

/// <summary>Xét duyệt riêng từng món sau khi phiếu tiếp nhận đã được lưu.</summary>
internal sealed class frmXetDuyetKyGui : Form
{
    private readonly ComboBox _slips = Theme.Combo(560);
    private readonly DataGridView _products = new();
    private readonly Label _slipInfo = new() { Dock = DockStyle.Fill, Font = Theme.Body, ForeColor = Theme.Ink };
    private readonly Label _detail = new() { Dock = DockStyle.Fill, Font = Theme.Body, ForeColor = Theme.Ink };
    private readonly Label _message = new() { Dock = DockStyle.Fill, Font = Theme.Small, ForeColor = Theme.Muted };
    private readonly Button _accept = Theme.PrimaryButton("Chấp nhận ký gửi", 185);
    private readonly Button _reject = Theme.GhostButton("Từ chối", 120);
    private bool _loading;
    private readonly int? _initialSlip;

    public frmXetDuyetKyGui(int? initialSlip = null)
    {
        _initialSlip = initialSlip;
        Text = "Xét duyệt ký gửi — Thiên Kim Consignment";
        ClientSize = new Size(1100, 740);
        MinimumSize = new Size(920, 620);
        StartPosition = FormStartPosition.CenterParent;
        Font = Theme.Body;
        BackColor = Theme.Canvas;

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20, 14, 20, 14), ColumnCount = 1, RowCount = 5 };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 126));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

        var heading = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
        heading.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        heading.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var title = new Panel { Dock = DockStyle.Fill };
        title.Controls.Add(new Label { Text = "Xét duyệt ký gửi", Font = Theme.H1, ForeColor = Theme.Ink, AutoSize = true });
        title.Controls.Add(new Label { Text = "Kiểm tra và quyết định nhận từng món. Chốt giá và đăng bán ở bước tiếp theo.",
            Font = Theme.Small, ForeColor = Theme.Muted, AutoSize = true, Location = new Point(2, 40) });
        heading.Controls.Add(title, 0, 0);
        var refresh = Theme.GhostButton("Làm mới", 110);
        refresh.Click += (_, _) => LoadSlips(CurrentSlip);
        var close = Theme.GhostButton("Đóng", 100);
        close.Click += (_, _) => Close();
        heading.Controls.Add(Theme.Actions(close, refresh), 1, 0);
        root.Controls.Add(heading, 0, 0);

        var filter = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Margin = new Padding(0) };
        filter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        filter.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        filter.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        filter.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        filter.Controls.Add(new Label { Text = "Phiếu tiếp nhận cần xem (ưu tiên phiếu còn món chờ duyệt)", AutoSize = true, Font = Theme.Small, ForeColor = Theme.Muted }, 0, 0);
        _slips.Dock = DockStyle.Fill;
        _slips.Margin = new Padding(0, 0, 0, 6);
        filter.Controls.Add(_slips, 0, 1);
        filter.Controls.Add(_slipInfo, 0, 2);
        root.Controls.Add(filter, 0, 1);

        var card = new Card("Sản phẩm trong phiếu — chọn từng món để xét duyệt") { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 12) };
        Theme.StyleGrid(_products);
        _products.Dock = DockStyle.Fill;
        _products.MultiSelect = false;
        card.Body.Controls.Add(_products);
        root.Controls.Add(card, 0, 2);
        var detailCard = new Card("Thông tin sản phẩm đang chọn", 36) { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 8) };
        detailCard.Body.Controls.Add(_detail);
        root.Controls.Add(detailCard, 0, 3);
        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        footer.Controls.Add(_message, 0, 0);
        footer.Controls.Add(Theme.Actions(_reject, _accept), 1, 0);
        root.Controls.Add(footer, 0, 4);
        Controls.Add(root);
        _slips.SelectedIndexChanged += (_, _) => { if (!_loading) LoadProducts(); };
        _products.SelectionChanged += (_, _) => UpdateSelection();
        _products.CurrentCellChanged += (_, _) => UpdateSelection();
        _accept.Click += (_, _) => Review("Đã chấp nhận");
        _reject.Click += (_, _) => Review("Từ chối");
        _reject.ForeColor = Theme.Danger;
        Shown += (_, _) => LoadSlips(_initialSlip);
    }

    private int? CurrentSlip => _slips.SelectedValue is int id ? id : null;
    private DataRowView? SelectedProduct => _products.CurrentRow?.DataBoundItem as DataRowView;

    private void LoadSlips(int? selectId = null)
    {
        try
        {
            _loading = true;
            var slips = Db.Query("""
                SELECT p.MaPKG, p.NgayKyGui, p.TrangThai, kh.HoTen, nv.HoTen AS NhanVien,
                       COUNT(sp.MaSP) AS TongMon,
                       SUM(CASE WHEN sp.TrangThaiTiepNhan = N'Chờ xử lý' THEN 1 ELSE 0 END) AS ChoDuyet
                FROM dbo.PHIEU_KY_GUI p
                JOIN dbo.KHACH_HANG kh ON kh.MaKH = p.MaKH
                JOIN dbo.NHAN_VIEN nv ON nv.MaNV = p.MaNV
                LEFT JOIN dbo.SAN_PHAM sp ON sp.MaPKG = p.MaPKG
                GROUP BY p.MaPKG, p.NgayKyGui, p.TrangThai, kh.HoTen, nv.HoTen
                ORDER BY CASE WHEN SUM(CASE WHEN sp.TrangThaiTiepNhan = N'Chờ xử lý' THEN 1 ELSE 0 END) > 0 THEN 0 ELSE 1 END,
                         p.NgayKyGui DESC, p.MaPKG DESC;
                """);
            slips.Columns.Add("HienThi", typeof(string));
            foreach (DataRow row in slips.Rows)
                row["HienThi"] = $"Phiếu #{row["MaPKG"]} — {row["HoTen"]} — {row["ChoDuyet"]}/{row["TongMon"]} món chờ duyệt";
            _slips.DisplayMember = "HienThi";
            _slips.ValueMember = "MaPKG";
            _slips.DataSource = slips;
            if (selectId.HasValue && slips.Rows.Cast<DataRow>().Any(r => Convert.ToInt32(r["MaPKG"]) == selectId))
                _slips.SelectedValue = selectId.Value;
            _loading = false;
            LoadProducts();
        }
        catch (Exception ex) { ShowError(ex); }
        finally { _loading = false; }
    }

    private void LoadProducts()
    {
        try
        {
            _message.Text = "";
            _message.ForeColor = Theme.Muted;
            var table = Db.Query("""
                SELECT MaSP, TenSP, TenDanhMuc, TenThuongHieu, KichThuoc, MauSac, ChatLieu,
                       TinhTrang, GiaDeXuat, TrangThaiTiepNhan, TrangThaiBan
                FROM dbo.vw_SanPham_DayDu WHERE MaPKG = @Id
                ORDER BY CASE WHEN TrangThaiTiepNhan = N'Chờ xử lý' THEN 0 ELSE 1 END, MaSP;
                """, Db.Param("@Id", CurrentSlip));
            Grid.Bind(_products, table, new Dictionary<string, string> {
                ["MaSP"] = "Mã SP", ["TenSP"] = "Sản phẩm", ["TenDanhMuc"] = "Danh mục",
                ["TenThuongHieu"] = "Thương hiệu", ["TinhTrang"] = "Tình trạng",
                ["GiaDeXuat"] = "Giá đề xuất", ["TrangThaiTiepNhan"] = "Kết quả duyệt", ["TrangThaiBan"] = "Trạng thái bán"
            }, "KichThuoc", "MauSac", "ChatLieu");
            _products.Columns["MaSP"]!.FillWeight = 45;
            _products.Columns["TenSP"]!.FillWeight = 155;
            _products.Columns["TinhTrang"]!.FillWeight = 130;
            Theme.MoneyColumn(_products.Columns["GiaDeXuat"]);
            _slipInfo.Text = _slips.SelectedItem is DataRowView slip
                ? $"Trạng thái phiếu: {slip["TrangThai"]}   •   Ngày nhận: {Convert.ToDateTime(slip["NgayKyGui"]):dd/MM/yyyy}   •   Tiếp nhận: {slip["NhanVien"]}"
                : "Chưa có phiếu tiếp nhận. Hãy lập và lưu phiếu trước.";
            if (table.Rows.Count > 0 && !table.Rows.Cast<DataRow>().Any(r => Convert.ToString(r["TrangThaiTiepNhan"]) == "Chờ xử lý"))
                _message.Text = "Phiếu đã xét duyệt hết. Món được chấp nhận có thể chuyển sang bước đăng bán.";
            UpdateSelection();
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void UpdateSelection()
    {
        var row = SelectedProduct;
        var pending = row is not null && Convert.ToString(row["TrangThaiTiepNhan"]) == "Chờ xử lý";
        _accept.Enabled = pending;
        _reject.Enabled = pending;
        _detail.Text = row is null ? "Chọn một sản phẩm trong phiếu."
            : $"{row["TenSP"]} — {row["TrangThaiTiepNhan"]}\r\nTình trạng: {row["TinhTrang"]}\r\nKích thước: {row["KichThuoc"]}   •   Màu sắc: {row["MauSac"]}   •   Chất liệu: {row["ChatLieu"]}";
    }

    private void Review(string result)
    {
        var row = SelectedProduct;
        if (row is null || Convert.ToString(row["TrangThaiTiepNhan"]) != "Chờ xử lý") return;
        var action = result == "Đã chấp nhận" ? "chấp nhận ký gửi" : "từ chối nhận ký gửi";
        var detail = result == "Đã chấp nhận" ? "Món này sẽ ở trạng thái Chưa đăng bán. Giá được chốt ở bước đăng bán."
            : "Món này không được đăng bán. Hồ sơ tiếp nhận vẫn được giữ lại.";
        if (MessageBox.Show(this, $"Bạn có chắc muốn {action} sản phẩm “{row["TenSP"]}”?\n\n{detail}",
            "Xác nhận xét duyệt", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            Db.Execute("EXEC dbo.sp_DuyetSanPham @MaSP, @KetQua", Db.Param("@MaSP", row["MaSP"]), Db.Param("@KetQua", result));
            LoadSlips(CurrentSlip);
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void ShowError(Exception ex)
    {
        _message.ForeColor = Theme.Danger;
        _message.Text = Db.ErrorMessage(ex);
        _accept.Enabled = false;
        _reject.Enabled = false;
    }
}
