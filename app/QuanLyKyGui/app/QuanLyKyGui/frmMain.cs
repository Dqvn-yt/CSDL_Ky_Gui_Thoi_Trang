namespace QuanLyKyGui;

internal sealed class frmMain : Form
{
    private readonly Label _lblUpdated = new() { AutoSize = true, Font = Theme.Small, ForeColor = Theme.Muted };
    private readonly StatCard _cardBan = new("SẢN PHẨM ĐANG BÁN", Theme.Accent);
    private readonly StatCard _cardDon = new("ĐƠN CHỜ THANH TOÁN", Theme.Gold);
    private readonly StatCard _cardDoanhThu = new("DOANH THU KỲ GẦN NHẤT", Theme.Success);
    private readonly StatCard _cardCongNo = new("CÔNG NỢ NGƯỜI KÝ GỬI", ColorTranslator.FromHtml("#3C5A78"));
    private readonly DataGridView _gridOrders = new();
    private readonly DataGridView _gridSlips = new();
    private readonly Label _footerInfo = new() { AutoSize = true, Font = Theme.Small, ForeColor = Theme.Muted };
    private readonly Panel _contentHost = new() { Dock = DockStyle.Fill, Margin = new Padding(0) };
    private readonly List<Button> _navButtons = new();
    private readonly string? _startPage;
    private Control? _dashboard;
    private Form? _page;
    private bool _switching;

    public frmMain(string? startPage = null)
    {
        _startPage = startPage;
        Text = "Thiên Kim Consignment — Quản lý sàn ký gửi đồ thời trang cũ";
        ClientSize = new Size(1360, 830);
        MinimumSize = new Size(1180, 740);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Theme.Canvas;
        Font = Theme.Body;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Theme.Canvas
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildBody(), 0, 1);
        root.Controls.Add(BuildFooter(), 0, 2);
        Controls.Add(root);

        Shown += (_, _) =>
        {
            RefreshData();
            if (!string.IsNullOrWhiteSpace(_startPage)) OpenPage(_startPage!);
        };
    }

    private Control BuildHeader()
    {
        var header = new Panel { Dock = DockStyle.Fill, BackColor = Theme.HeaderBg, Margin = new Padding(0) };
        header.Controls.Add(new Panel { Location = new Point(26, 22), Size = new Size(3, 52), BackColor = Theme.Gold });
        header.Controls.Add(new Label
        {
            Text = "THIÊN KIM",
            Font = new Font("Segoe UI Semibold", 22f),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(42, 14)
        });
        header.Controls.Add(new Label
        {
            Text = "HỆ THỐNG QUẢN LÝ SÀN KÝ GỬI ĐỒ THỜI TRANG CŨ",
            Font = new Font("Segoe UI", 9f),
            ForeColor = ColorTranslator.FromHtml("#C9BFC8"),
            AutoSize = true,
            Location = new Point(44, 56)
        });

        var right = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 430,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 22, 26, 0)
        };
        right.Controls.Add(new Label
        {
            Text = "●  KẾT NỐI CSDL: QUANLYKYGUITHOITRANG",
            Font = new Font("Segoe UI Semibold", 8.75f),
            ForeColor = ColorTranslator.FromHtml("#86D3A7"),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 6)
        });
        right.Controls.Add(new Label
        {
            Text = "SQL Server 2025  •  localhost  •  Windows Authentication",
            Font = Theme.Small,
            ForeColor = ColorTranslator.FromHtml("#BDB2BE"),
            AutoSize = true
        });
        header.Controls.Add(right);
        return header;
    }

    private Control BuildBody()
    {
        var body = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0)
        };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 246));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        body.Controls.Add(BuildNav(), 0, 0);
        _dashboard = BuildContent();
        _contentHost.Controls.Add(_dashboard);
        body.Controls.Add(_contentHost, 1, 0);
        return body;
    }

    private Control BuildNav()
    {
        var nav = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Surface, Margin = new Padding(0) };
        nav.Controls.Add(new Label
        {
            Text = "CHỨC NĂNG",
            Font = new Font("Segoe UI Semibold", 8f),
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(22, 22)
        });

        var items = new (string Text, Action? Click)[]
        {
            ("Tổng quan", ShowDashboard),
            ("Tiếp nhận ký gửi", OpenIntake),
            ("Lập đơn hàng", OpenOrder),
            ("Tra cứu sản phẩm", OpenSearch),
            ("Báo cáo doanh thu", OpenRevenue),
            ("Báo cáo quyết toán", OpenSettlement)
        };

        _navButtons.Clear();
        var y = 52;
        foreach (var item in items)
        {
            var button = new Button
            {
                Text = "     " + item.Text,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(206, 42),
                Location = new Point(20, y),
                Font = Theme.Bold,
                BackColor = Theme.Surface,
                ForeColor = Theme.Ink,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderSize = 0;
            if (item.Click is not null)
            {
                var handler = item.Click;
                button.Click += (_, _) => handler();
            }
            nav.Controls.Add(button);
            _navButtons.Add(button);
            y += 46;
        }
        SetActiveNav(0);

        var divider = new Panel { Location = new Point(20, y + 10), Size = new Size(206, 1), BackColor = Theme.Line };
        nav.Controls.Add(divider);
        nav.Controls.Add(new Label
        {
            Text = "NGHIỆP VỤ",
            Font = new Font("Segoe UI Semibold", 8f),
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(22, y + 26)
        });
        var notes = "• Ký gửi: 20% hoa hồng\n• Giữ hàng tối đa 3 ngày\n• Quyết toán theo sản phẩm";
        nav.Controls.Add(new Label
        {
            Text = notes,
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(22, y + 48)
        });

        nav.Controls.Add(new Label
        {
            Text = "Phiên bản 1.0  •  15/11/2026",
            Font = Theme.Small,
            ForeColor = ColorTranslator.FromHtml("#9C948A"),
            AutoSize = true,
            Location = new Point(22, 640),
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom
        });
        return nav;
    }

    private Control BuildContent()
    {
        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(24, 18, 24, 4),
            BackColor = Theme.Canvas
        };
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 128));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var titleRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
        titleRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        titleRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var titles = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        titles.Controls.Add(new Label
        {
            Text = "Tổng quan hoạt động",
            Font = Theme.H1,
            ForeColor = Theme.Ink,
            AutoSize = true,
            Location = new Point(0, 0)
        });
        _lblUpdated.Location = new Point(272, 10);
        _lblUpdated.Text = "Đang tải dữ liệu…";
        titles.Controls.Add(_lblUpdated);
        titleRow.Controls.Add(titles, 0, 0);

        var refresh = Theme.GhostButton("↻  Làm mới", 110);
        refresh.Click += (_, _) => RefreshData();
        var revenue = Theme.PrimaryButton("Báo cáo doanh thu", 200);
        revenue.Click += (_, _) => OpenRevenue();
        var actions = Theme.Actions(refresh, revenue);
        titleRow.Controls.Add(actions, 1, 0);
        content.Controls.Add(titleRow, 0, 0);

        var cards = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1, Margin = new Padding(0) };
        for (var i = 0; i < 4; i++) cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        cards.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var all = new[] { _cardBan, _cardDon, _cardDoanhThu, _cardCongNo };
        for (var i = 0; i < all.Length; i++)
        {
            all[i].Dock = DockStyle.Fill;
            all[i].Margin = new Padding(0, 0, i == 3 ? 0 : 12, 12);
            cards.Controls.Add(all[i], i, 0);
        }
        content.Controls.Add(cards, 0, 1);

        var bottom = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0, 6, 0, 0) };
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        bottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var ordersCard = new Card("Đơn hàng đang cần xử lý");
        ordersCard.Dock = DockStyle.Fill;
        ordersCard.Margin = new Padding(0, 0, 12, 0);
        Theme.StyleGrid(_gridOrders);
        _gridOrders.Dock = DockStyle.Fill;
        ordersCard.Body.Controls.Add(_gridOrders);
        var orderHint = new Label
        {
            Text = "Nhấp đúp một dòng để mở màn hình lập đơn hàng và thu tiền.",
            Dock = DockStyle.Bottom,
            Height = 24,
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            TextAlign = ContentAlignment.BottomLeft
        };
        ordersCard.Body.Controls.Add(orderHint);
        orderHint.BringToFront();
        bottom.Controls.Add(ordersCard, 0, 0);

        var slipsCard = new Card("Phiếu ký gửi gần đây");
        slipsCard.Dock = DockStyle.Fill;
        Theme.StyleGrid(_gridSlips);
        _gridSlips.Dock = DockStyle.Fill;
        slipsCard.Body.Controls.Add(_gridSlips);
        var slipHint = new Label
        {
            Text = "Tiến độ duyệt và bán sản phẩm theo từng phiếu.",
            Dock = DockStyle.Bottom,
            Height = 24,
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            TextAlign = ContentAlignment.BottomLeft
        };
        slipsCard.Body.Controls.Add(slipHint);
        slipHint.BringToFront();
        bottom.Controls.Add(slipsCard, 1, 0);
        content.Controls.Add(bottom, 0, 2);

        _gridOrders.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0) OpenOrder();
        };
        _gridSlips.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0) OpenSlipDetail();
        };

        return content;
    }

    private Control BuildFooter()
    {
        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Theme.Surface, Margin = new Padding(0) };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        _footerInfo.Text = "CSDL: QuanLyKyGuiThoiTrang  •  Máy chủ: localhost  •  Đăng nhập Windows";
        _footerInfo.Margin = new Padding(24, 6, 0, 0);
        footer.Controls.Add(_footerInfo, 0, 0);
        var right = new Label
        {
            Text = "Bài tập lớn môn Cơ sở dữ liệu  •  Nhóm thực hiện: 3 thành viên",
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Anchor = AnchorStyles.Right,
            Margin = new Padding(0, 6, 24, 0)
        };
        footer.Controls.Add(right, 1, 0);
        return footer;
    }

    private void RefreshData()
    {
        try
        {
            var onSale = Db.Long("SELECT COUNT(*) FROM dbo.SAN_PHAM WHERE TrangThaiBan = N'Đang bán'");
            var pendingOrders = Db.Long("SELECT COUNT(*) FROM dbo.DON_HANG WHERE TrangThai = N'Chờ thanh toán'");
            var dueOrders = Db.Long("SELECT COUNT(*) FROM dbo.vw_DonHang_TongHop WHERE TrangThai = N'Chờ thanh toán' AND TinhTrangHan = N'Quá hạn'");
            _cardBan.SetValue(onSale.ToString("N0"), "Sản phẩm sẵn sàng bán");
            _cardDon.SetValue(pendingOrders.ToString("N0"), $"{dueOrders} đơn đã quá hạn thanh toán");

            var revenue = Db.Query("SELECT TOP 1 Thang, DoanhThu FROM dbo.vw_DoanhThu_TheoThang ORDER BY Thang DESC");
            if (revenue.Rows.Count > 0)
            {
                var month = Convert.ToString(revenue.Rows[0]["Thang"]) ?? "";
                var value = Convert.ToDecimal(revenue.Rows[0]["DoanhThu"]);
                _cardDoanhThu.SetValue(value.ToString("#,##0") + " đ", "Tháng " + month);
            }
            else
            {
                _cardDoanhThu.SetValue("0 đ", "Chưa có đơn hoàn tất");
            }

            var debt = Db.Money("SELECT ISNULL(SUM(TienPhaiTra), 0) FROM dbo.vw_CongNo_NguoiKyGui");
            var debtItems = Db.Long("SELECT ISNULL(SUM(SoSanPhamChuaQuyetToan), 0) FROM dbo.vw_CongNo_NguoiKyGui");
            _cardCongNo.SetValue(debt.ToString("#,##0") + " đ", $"{debtItems} sản phẩm chưa quyết toán");

            _gridOrders.DataSource = Db.Query(@"
SELECT TOP 12 MaDH, TenKhachHang, NgayLap, TrangThai, TongTien, DaThu, ConLai, TinhTrangHan
FROM dbo.vw_DonHang_TongHop
WHERE TrangThai IN (N'Chờ thanh toán', N'Đang xử lý')
ORDER BY CASE WHEN TrangThai = N'Chờ thanh toán' THEN 0 ELSE 1 END, NgayLap DESC;");
            Grid.Bind(_gridOrders, (System.Data.DataTable)_gridOrders.DataSource, new Dictionary<string, string>
            {
                ["MaDH"] = "Mã",
                ["TenKhachHang"] = "Khách hàng",
                ["TrangThai"] = "Trạng thái",
                ["TongTien"] = "Tổng tiền",
                ["ConLai"] = "Còn lại"
            }, "NgayLap", "DaThu", "TinhTrangHan");
            foreach (var name in new[] { "TongTien", "ConLai" })
                Theme.MoneyColumn(_gridOrders.Columns[name]);
            _gridOrders.Columns["MaDH"].FillWeight = 55;
            _gridOrders.Columns["TenKhachHang"].FillWeight = 150;
            _gridOrders.Columns["TrangThai"].FillWeight = 150;
            _gridOrders.Columns["TongTien"].FillWeight = 100;
            _gridOrders.Columns["ConLai"].FillWeight = 100;

            _gridSlips.DataSource = Db.Query(@"
SELECT TOP 10 MaPKG, TenNguoiKyGui, NgayKyGui, TenNhanVien, TongSanPham, DaBan, TrangThai
FROM dbo.vw_PhieuKyGui_TienDo
ORDER BY NgayKyGui DESC;");
            Grid.Bind(_gridSlips, (System.Data.DataTable)_gridSlips.DataSource, new Dictionary<string, string>
            {
                ["MaPKG"] = "Mã",
                ["TenNguoiKyGui"] = "Người ký gửi",
                ["NgayKyGui"] = "Ngày",
                ["TrangThai"] = "Trạng thái"
            }, "TenNhanVien", "TongSanPham", "DaBan");
            _gridSlips.Columns["NgayKyGui"].DefaultCellStyle.Format = "dd/MM/yyyy";
            _gridSlips.Columns["MaPKG"].FillWeight = 50;
            _gridSlips.Columns["TenNguoiKyGui"].FillWeight = 145;
            _gridSlips.Columns["NgayKyGui"].FillWeight = 110;
            _gridSlips.Columns["TrangThai"].FillWeight = 130;

            _lblUpdated.Text = "Cập nhật lúc " + DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
            _footerInfo.Text = $"CSDL: QuanLyKyGuiThoiTrang  •  Máy chủ: localhost  •  {onSale} sản phẩm đang bán  •  {pendingOrders} đơn chờ thu";
        }
        catch (Exception ex)
        {
            _lblUpdated.Text = "Không tải được dữ liệu: " + Db.ErrorMessage(ex);
            _lblUpdated.ForeColor = Theme.Danger;
        }
    }

    private void SetActiveNav(int index)
    {
        for (var i = 0; i < _navButtons.Count; i++)
        {
            var active = i == index;
            var button = _navButtons[i];
            button.BackColor = active ? Theme.AccentSoft : Theme.Surface;
            button.ForeColor = active ? Theme.Accent : Theme.Ink;
            button.FlatAppearance.MouseOverBackColor = active ? Theme.AccentSoft : ColorTranslator.FromHtml("#F4EFE6");
        }
    }

    private void ShowPage(Form form, int navIndex)
    {
        _switching = true;
        try
        {
            if (_page is not null)
            {
                _page.FormClosed -= PageClosed;
                _contentHost.Controls.Remove(_page);
                _page.Dispose();
            }

            _page = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.MinimumSize = Size.Empty;
            form.Dock = DockStyle.Fill;
            form.ShowInTaskbar = false;
            form.StartPosition = FormStartPosition.Manual;
            form.FormClosed += PageClosed;

            _contentHost.SuspendLayout();
            _contentHost.Controls.Clear();
            _contentHost.Controls.Add(form);
            _contentHost.ResumeLayout();
            form.Show();
            SetActiveNav(navIndex);
        }
        finally
        {
            _switching = false;
        }
    }

    private void PageClosed(object? sender, FormClosedEventArgs e)
    {
        if (_switching || !ReferenceEquals(sender, _page)) return;
        _page = null;
        ShowDashboard();
    }

    private void ShowDashboard()
    {
        _switching = true;
        try
        {
            if (_page is not null)
            {
                _page.FormClosed -= PageClosed;
                _contentHost.Controls.Remove(_page);
                _page.Dispose();
                _page = null;
            }

            _dashboard ??= BuildContent();
            _contentHost.SuspendLayout();
            _contentHost.Controls.Clear();
            _contentHost.Controls.Add(_dashboard);
            _contentHost.ResumeLayout();
            SetActiveNav(0);
        }
        finally
        {
            _switching = false;
        }
        RefreshData();
    }

    private void OpenSlipDetail()
    {
        if (_gridSlips.CurrentRow is null) return;
        var value = _gridSlips.CurrentRow.Cells["MaPKG"].Value;
        if (value is null || value is DBNull) return;
        OpenSlipDetail(Convert.ToInt32(value));
    }

    private void OpenSlipDetail(int maPKG)
    {
        using var dialog = new frmChiTietPhieuKyGui(maPKG);
        dialog.ShowDialog(this);
    }

    private void OpenPage(string key)
    {
        switch (key.ToLowerInvariant())
        {
            case "tiepnhan": OpenIntake(); return;
            case "banhang": OpenOrder(); return;
            case "tracuu": OpenSearch(); return;
            case "doanhthu": OpenRevenue(); return;
            case "quyettoan": OpenSettlement(); return;
        }

        if (key.StartsWith("phieu:", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(key[6..], out var maPKG))
            OpenSlipDetail(maPKG);
    }

    private void OpenIntake() => ShowPage(new frmTiepNhanKyGui(), 1);
    private void OpenOrder() => ShowPage(new frmLapDonHang(), 2);
    private void OpenSearch() => ShowPage(new frmTraCuuSanPham(), 3);
    private void OpenRevenue() => ShowPage(new frmBaoCaoDoanhThu(), 4);
    private void OpenSettlement() => ShowPage(new frmBaoCaoQuyetToan(), 5);
}
