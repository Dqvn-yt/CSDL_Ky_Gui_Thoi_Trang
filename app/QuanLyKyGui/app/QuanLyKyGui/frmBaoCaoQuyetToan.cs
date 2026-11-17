using System.Data;
using Microsoft.Data.SqlClient;

namespace QuanLyKyGui;

internal sealed class frmBaoCaoQuyetToan : Form
{
    private const string PreparedBy = "Nhân viên kế toán";
    private const string ReceivedBy = "Người ký gửi (liên 1) — Lưu tại cửa hàng (liên 2)";

    private readonly ComboBox _cboCustomer = Theme.Combo(190);
    private readonly ComboBox _cboStatus = Theme.Combo(150);
    private readonly DateTimePicker _dtpFrom = Theme.DatePicker(140);
    private readonly DateTimePicker _dtpTo = Theme.DatePicker(140);
    private readonly DataGridView _grid = new();
    private readonly DataGridView _gridPending = new();

    private readonly StatCard _cardSales = new("TỔNG GIÁ BÁN", Theme.Accent);
    private readonly StatCard _cardCommission = new("HOA HỒNG SÀN", Theme.Gold);
    private readonly StatCard _cardPayout = new("TIỀN PHẢI TRẢ", Theme.Success);
    private readonly StatCard _cardItems = new("SẢN PHẨM ĐÃ BÁN", ColorTranslator.FromHtml("#3C5A78"));

    private readonly Label _lblSelected = new() { AutoSize = false, Height = 48, Dock = DockStyle.Fill, Font = Theme.Bold, ForeColor = Theme.Ink };
    private readonly ComboBox _cboMethod = Theme.Combo(150);
    private readonly ComboBox _cboEmployee = Theme.Combo(220);
    private readonly TextBox _txtNote = Theme.TextBox(null, 250);
    private readonly Label _lblMessage = new() { AutoSize = false, Height = 22, Font = Theme.Small, ForeColor = Theme.Danger };
    private readonly Label _lblPeriod = new() { AutoSize = true, Font = Theme.Small, ForeColor = Theme.Muted };

    private DataTable _current = new();
    private int _selectedCustomer;

    public frmBaoCaoQuyetToan()
    {
        Text = "Báo cáo quyết toán — Thiên Kim Consignment";
        ClientSize = new Size(1360, 880);
        MinimumSize = new Size(1180, 740);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Theme.Canvas;
        Font = Theme.Body;

        BuildUi();
        LoadLookups();
        var today = DateTime.Today;
        _dtpFrom.Value = new DateTime(today.Year, 1, 1);
        _dtpTo.Value = today;
        Shown += (_, _) =>
        {
            LoadReport();
            LoadPendingSlips();
        };
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(20, 14, 20, 14),
            BackColor = Theme.Canvas
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 142));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        root.Controls.Add(BuildTitle(), 0, 0);
        root.Controls.Add(BuildFilterCard(), 0, 1);
        root.Controls.Add(BuildSummaryRow(), 0, 2);
        root.Controls.Add(BuildContent(), 0, 3);
        root.Controls.Add(BuildFooter(), 0, 4);
        Controls.Add(root);
    }

    private Control BuildTitle()
    {
        var row = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var titles = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        titles.Controls.Add(new Label
        {
            Text = "Báo cáo quyết toán người ký gửi",
            Font = Theme.H1,
            ForeColor = Theme.Ink,
            AutoSize = true,
            Location = new Point(0, 0)
        });
        titles.Controls.Add(new Label
        {
            Text = "Đối chiếu tiền bán, hoa hồng 20% và số tiền phải trả từng người ký gửi; lập phiếu chi ngay trên màn hình.",
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(2, 40)
        });
        row.Controls.Add(titles, 0, 0);

        var close = Theme.GhostButton("Đóng", 100);
        close.Click += (_, _) => Close();
        var export = Theme.GoldButton("Xuất Excel", 150);
        export.Click += (_, _) => Export();
        var actions = Theme.Actions(close, export);
        row.Controls.Add(actions, 1, 0);
        return row;
    }

    private Control BuildFilterCard()
    {
        var card = new Card("Bộ lọc báo cáo", 40);
        card.Dock = DockStyle.Fill;
        card.Margin = new Padding(0, 0, 0, 12);
        card.Padding = new Padding(16, 38, 16, 8);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(0) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Margin = new Padding(0) };
        flow.Controls.Add(Fields.Field("Người ký gửi", _cboCustomer));
        flow.Controls.Add(Fields.Field("Trạng thái quyết toán", _cboStatus));
        flow.Controls.Add(Fields.Field("Từ ngày bán", _dtpFrom));
        flow.Controls.Add(Fields.Field("Đến ngày bán", _dtpTo));
        var run = Theme.PrimaryButton("Xem báo cáo", 140);
        run.Margin = new Padding(4, 22, 8, 0);
        run.Click += (_, _) => LoadReport();
        flow.Controls.Add(run);
        layout.Controls.Add(flow, 0, 0);

        _lblPeriod.Text = "Chưa tải dữ liệu.";
        layout.Controls.Add(_lblPeriod, 0, 1);
        card.Body.Controls.Add(layout);
        return card;
    }

    private Control BuildSummaryRow()
    {
        var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1, Margin = new Padding(0) };
        for (var i = 0; i < 4; i++) table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var cards = new[] { _cardSales, _cardCommission, _cardPayout, _cardItems };
        for (var i = 0; i < cards.Length; i++)
        {
            cards[i].Dock = DockStyle.Fill;
            cards[i].Margin = new Padding(0, 0, i == 3 ? 0 : 12, 12);
            table.Controls.Add(cards[i], i, 0);
        }
        return table;
    }

    private Control BuildContent()
    {
        var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36));
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var gridCard = new Card("Chi tiết sản phẩm đã bán theo người ký gửi");
        gridCard.Dock = DockStyle.Fill;
        gridCard.Margin = new Padding(0, 0, 12, 0);
        Theme.StyleGrid(_grid);
        _grid.Dock = DockStyle.Fill;
        _grid.SelectionChanged += (_, _) => UpdateSelectedCustomer();
        gridCard.Body.Controls.Add(_grid);
        table.Controls.Add(gridCard, 0, 0);

        table.Controls.Add(BuildActionCard(), 1, 0);
        return table;
    }

    private Control BuildActionCard()
    {
        var card = new Card("Lập phiếu quyết toán");
        card.Dock = DockStyle.Fill;
        card.Margin = new Padding(0);
        card.Padding = new Padding(16, 42, 16, 10);

        var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 9,
            Margin = new Padding(0),
            Padding = new Padding(0, 0, 6, 8)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (var i = 0; i < layout.RowCount; i++) layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _lblSelected.Text = "Chọn một dòng để chọn người ký gửi.\nPhiếu quyết toán gom toàn bộ sản phẩm đã bán nhưng chưa được chi tiền.";
        layout.Controls.Add(_lblSelected, 0, 0);

        _cboMethod.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản" });
        _cboMethod.SelectedIndex = 0;
        layout.Controls.Add(BuildActionField("Hình thức chi", _cboMethod), 0, 1);
        layout.Controls.Add(BuildActionField("Nhân viên lập", _cboEmployee), 0, 2);
        layout.Controls.Add(BuildActionField("Ghi chú phiếu chi", _txtNote), 0, 3);
        var create = Theme.PrimaryButton("Lập phiếu cho người ký gửi", 240);
        create.Dock = DockStyle.Top;
        create.Click += (_, _) => CreateSettlement();
        layout.Controls.Add(create, 0, 4);
        _lblMessage.Dock = DockStyle.Top;
        _lblMessage.AutoSize = true;
        layout.Controls.Add(_lblMessage, 0, 5);

        var pendingTitle = new Label
        {
            Text = "Phiếu quyết toán đang chờ chi",
            Font = Theme.H3,
            ForeColor = Theme.Ink,
            AutoSize = true,
            Margin = new Padding(0, 6, 0, 2)
        };
        layout.Controls.Add(pendingTitle, 0, 6);

        Theme.StyleGrid(_gridPending);
        _gridPending.Height = 102;
        _gridPending.Dock = DockStyle.Top;
        layout.Controls.Add(_gridPending, 0, 7);

        var pay = Theme.GoldButton("Xác nhận đã chi tiền", 210);
        pay.Margin = new Padding(0, 6, 0, 0);
        pay.Dock = DockStyle.Top;
        pay.Click += (_, _) => ConfirmPayout();
        layout.Controls.Add(pay, 0, 8);

        _lblSelected.AutoSize = true;
        layout.ClientSizeChanged += (_, _) =>
        {
            var wrapWidth = Math.Max(1, layout.ClientSize.Width - layout.Padding.Horizontal - 6);
            var wrap = new Size(wrapWidth, 0);
            _lblSelected.MaximumSize = wrap;
            _lblMessage.MaximumSize = wrap;
            pendingTitle.MaximumSize = wrap;
        };

        scroll.Controls.Add(layout);
        card.Body.Controls.Add(scroll);
        return card;
    }

    private static Panel BuildActionField(string label, Control input)
    {
        var field = Fields.Field(label, input);
        field.Dock = DockStyle.Top;
        field.Margin = new Padding(0, 0, 0, 4);
        input.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        return field;
    }

    private Control BuildFooter()
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        var hint = new Label
        {
            Text = "Mỗi sản phẩm chỉ được quyết toán một lần; chỉ sản phẩm thuộc đơn Hoàn tất mới được chi.",
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Margin = new Padding(2, 8, 0, 0)
        };
        panel.Controls.Add(hint, 0, 0);
        var owners = new Label
        {
            Text = $"Người lập: {PreparedBy}   •   Người nhận: Người ký gửi / Quản lý",
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Anchor = AnchorStyles.Right,
            Margin = new Padding(0, 8, 2, 0)
        };
        panel.Controls.Add(owners, 1, 0);
        return panel;
    }

    private void LoadLookups()
    {
        try
        {
            var customers = Db.Query("SELECT MaKH, HoTen, SoDienThoai FROM dbo.KHACH_HANG ORDER BY HoTen");
            var cv = customers.Copy();
            cv.Columns.Add("HienThi", typeof(string));
            foreach (DataRow row in cv.Rows) row["HienThi"] = $"{row["HoTen"]} — {row["SoDienThoai"]}";
            var all = cv.NewRow();
            all["MaKH"] = DBNull.Value;
            all["HienThi"] = "(Tất cả người ký gửi)";
            cv.Rows.InsertAt(all, 0);
            _cboCustomer.DataSource = cv;
            _cboCustomer.DisplayMember = "HienThi";
            _cboCustomer.ValueMember = "MaKH";

            _cboStatus.Items.AddRange(new object[]
            {
                "(Tất cả trạng thái)", "Chưa quyết toán", "Chờ thanh toán", "Đã thanh toán"
            });
            _cboStatus.SelectedIndex = 0;

            var employees = Db.Query("SELECT MaNV, HoTen, ChucVu FROM dbo.NHAN_VIEN WHERE TrangThai = N'Đang làm việc' ORDER BY ChucVu, HoTen");
            var ev = employees.Copy();
            ev.Columns.Add("HienThi", typeof(string));
            foreach (DataRow row in ev.Rows) row["HienThi"] = $"{row["HoTen"]} ({row["ChucVu"]})";
            _cboEmployee.DataSource = ev;
            _cboEmployee.DisplayMember = "HienThi";
            _cboEmployee.ValueMember = "MaNV";
            foreach (DataRow row in ev.Rows)
            {
                if (Convert.ToString(row["ChucVu"]) == "Thu ngân")
                {
                    _cboEmployee.SelectedValue = row["MaNV"];
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Không tải được bộ lọc: " + Db.ErrorMessage(ex), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadReport()
    {
        _lblMessage.Text = string.Empty;
        try
        {
            var from = _dtpFrom.Value.Date;
            var to = _dtpTo.Value.Date;
            if (from > to)
            {
                _lblMessage.Text = "Từ ngày phải nhỏ hơn hoặc bằng Đến ngày.";
                return;
            }

            var status = _cboStatus.SelectedIndex <= 0 ? null : Convert.ToString(_cboStatus.SelectedItem);
            var customer = _cboCustomer.SelectedValue is null or DBNull ? (int?)null : Convert.ToInt32(_cboCustomer.SelectedValue);
            var table = Db.Procedure("dbo.sp_BaoCaoQuyetToan",
                new SqlParameter("@MaKH", SqlDbType.Int) { Value = (object?)customer ?? DBNull.Value },
                new SqlParameter("@TuNgay", SqlDbType.Date) { Value = from },
                new SqlParameter("@DenNgay", SqlDbType.Date) { Value = to },
                new SqlParameter("@TrangThai", SqlDbType.NVarChar, 20) { Value = (object?)status ?? DBNull.Value });
            _current = table;

            table.Columns["MaCTDH"].Caption = "Mã CT";
            table.Columns["MaDH"].Caption = "Mã đơn";
            table.Columns["NgayLap"].Caption = "Ngày bán";
            table.Columns["MaSP"].Caption = "Mã SP";
            table.Columns["TenSP"].Caption = "Sản phẩm";
            table.Columns["KichThuoc"].Caption = "Size";
            table.Columns["MauSac"].Caption = "Màu";
            table.Columns["MaKH"].Caption = "Mã KH";
            table.Columns["TenNguoiKyGui"].Caption = "Người ký gửi";
            table.Columns["SoDienThoai"].Caption = "SĐT";
            table.Columns["GiaBan"].Caption = "Giá bán";
            table.Columns["TienHoaHong"].Caption = "Hoa hồng";
            table.Columns["TienThucNhan"].Caption = "Thực nhận";
            table.Columns["MaCTTTKG"].Caption = "CT quyết toán";
            table.Columns["MaPTTKG"].Caption = "Phiếu chi";
            table.Columns["TrangThaiQuyetToan"].Caption = "Trạng thái";
            table.Columns["NgayLapPhieu"].Caption = "Ngày lập phiếu";
            table.Columns["NgayThanhToan"].Caption = "Ngày chi tiền";

            _grid.DataSource = table;
            foreach (var name in new[]
                     {
                         "MaCTDH", "MaSP", "KichThuoc", "MauSac", "MaKH", "MaCTTTKG", "MaPTTKG",
                         "NgayLapPhieu", "SoDienThoai", "NgayThanhToan"
                     })
                if (_grid.Columns.Contains(name)) _grid.Columns[name].Visible = false;
            _grid.Columns["MaDH"].HeaderText = "Mã đơn";
            _grid.Columns["NgayLap"].HeaderText = "Ngày bán";
            _grid.Columns["TenSP"].HeaderText = "Sản phẩm";
            _grid.Columns["TenNguoiKyGui"].HeaderText = "Người ký gửi";
            _grid.Columns["GiaBan"].HeaderText = "Giá bán";
            _grid.Columns["TienHoaHong"].HeaderText = "Hoa hồng";
            _grid.Columns["TienThucNhan"].HeaderText = "Thực nhận";
            _grid.Columns["TrangThaiQuyetToan"].HeaderText = "Trạng thái";
            _grid.Columns["NgayLap"].DefaultCellStyle.Format = "dd/MM/yyyy";
            _grid.Columns["MaDH"].FillWeight = 46;
            _grid.Columns["NgayLap"].FillWeight = 100;
            _grid.Columns["TenSP"].FillWeight = 125;
            _grid.Columns["TenNguoiKyGui"].FillWeight = 110;
            _grid.Columns["GiaBan"].FillWeight = 85;
            _grid.Columns["TienHoaHong"].FillWeight = 75;
            _grid.Columns["TienThucNhan"].FillWeight = 85;
            _grid.Columns["TrangThaiQuyetToan"].FillWeight = 135;
            Theme.MoneyColumn(_grid.Columns["GiaBan"]);
            Theme.MoneyColumn(_grid.Columns["TienHoaHong"]);
            Theme.MoneyColumn(_grid.Columns["TienThucNhan"]);

            var sales = table.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["GiaBan"]));
            var commission = table.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["TienHoaHong"]));
            var payout = table.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["TienThucNhan"]));
            _cardSales.SetValue(sales.ToString("#,##0") + " đ", $"{table.Rows.Count} sản phẩm");
            _cardCommission.SetValue(commission.ToString("#,##0") + " đ", "Tỷ lệ cố định 20%");
            _cardPayout.SetValue(payout.ToString("#,##0") + " đ", "Số tiền phải chi cho khách");
            _cardItems.SetValue(table.Rows.Count.ToString("N0"), "Đã bán và đủ điều kiện");

            _lblPeriod.Text = $"Kỳ báo cáo: {from:dd/MM/yyyy} – {to:dd/MM/yyyy}   •   {table.Rows.Count} dòng   •   Lập lúc {DateTime.Now:HH:mm dd/MM/yyyy}";
            UpdateSelectedCustomer();
        }
        catch (Exception ex)
        {
            _lblMessage.Text = "Lỗi: " + Db.ErrorMessage(ex);
        }
    }

    private void UpdateSelectedCustomer()
    {
        var row = _grid.CurrentRow;
        if (row is null || row.DataBoundItem is not DataRowView view)
        {
            _selectedCustomer = 0;
            _lblSelected.Text = "Chọn một dòng để chọn người ký gửi.\nPhiếu quyết toán gom toàn bộ sản phẩm đã bán nhưng chưa được chi tiền.";
            return;
        }

        _selectedCustomer = Convert.ToInt32(view["MaKH"]);
        var nameLine = $"{view["TenNguoiKyGui"]} — {view["SoDienThoai"]}";
        try
        {
            var info = Db.Query(@"
SELECT COUNT(*) AS SoSP, ISNULL(SUM(TienThucNhan), 0) AS Tien
FROM dbo.vw_QuyetToan_SanPham
WHERE MaKH = @MaKH AND TrangThaiQuyetToan = N'Chưa quyết toán';",
                Db.Param("@MaKH", _selectedCustomer));
            var count = Convert.ToInt32(info.Rows[0]["SoSP"]);
            var money = Convert.ToDecimal(info.Rows[0]["Tien"]);
            _lblSelected.Text = count == 0
                ? nameLine + "\nKhông còn sản phẩm chưa quyết toán."
                : nameLine + $"\n{count} sản phẩm chưa quyết toán — cần chi {money:#,##0} đ.";
        }
        catch (Exception ex)
        {
            _lblSelected.Text = nameLine + "\nKhông tính được công nợ: " + Db.ErrorMessage(ex);
        }
    }

    private void LoadPendingSlips()
    {
        try
        {
            var table = Db.Query(@"
SELECT p.MaPTTKG, p.NgayLap, kh.HoTen AS TenKhachHang, p.PhuongThuc,
       SUM(ct.TienThucNhan) AS SoTien, COUNT(*) AS SoSanPham
FROM dbo.PHIEU_THANH_TOAN_KY_GUI p
JOIN dbo.KHACH_HANG kh ON kh.MaKH = p.MaKH
JOIN dbo.CHI_TIET_THANH_TOAN_KY_GUI ct ON ct.MaPTTKG = p.MaPTTKG
WHERE p.TrangThai = N'Chờ thanh toán'
GROUP BY p.MaPTTKG, p.NgayLap, kh.HoTen, p.PhuongThuc
ORDER BY p.NgayLap DESC;");
            _gridPending.DataSource = table;
            Grid.Bind(_gridPending, table, new Dictionary<string, string>
            {
                ["MaPTTKG"] = "Phiếu",
                ["NgayLap"] = "Ngày lập",
                ["TenKhachHang"] = "Người ký gửi",
                ["PhuongThuc"] = "Hình thức",
                ["SoTien"] = "Số tiền",
                ["SoSanPham"] = "SP"
            });
            _gridPending.Columns["NgayLap"].DefaultCellStyle.Format = "dd/MM/yyyy";
            _gridPending.Columns["NgayLap"].Visible = false;
            _gridPending.Columns["PhuongThuc"].Visible = false;
            _gridPending.Columns["MaPTTKG"].FillWeight = 50;
            _gridPending.Columns["TenKhachHang"].FillWeight = 140;
            _gridPending.Columns["SoTien"].FillWeight = 85;
            _gridPending.Columns["SoSanPham"].FillWeight = 32;
            Theme.MoneyColumn(_gridPending.Columns["SoTien"]);
        }
        catch (Exception ex)
        {
            _lblMessage.Text = "Không tải được phiếu chờ chi: " + Db.ErrorMessage(ex);
        }
    }

    private void CreateSettlement()
    {
        _lblMessage.Text = string.Empty;
        if (_selectedCustomer == 0)
        {
            _lblMessage.Text = "Chọn một dòng trong báo cáo để xác định người ký gửi.";
            return;
        }
        try
        {
            var lines = Db.Query(@"
SELECT c.MaCTDH
FROM dbo.CHI_TIET_DON_HANG c
JOIN dbo.DON_HANG d ON d.MaDH = c.MaDH AND d.TrangThai = N'Hoàn tất'
JOIN dbo.SAN_PHAM sp ON sp.MaSP = c.MaSP
JOIN dbo.PHIEU_KY_GUI pkg ON pkg.MaPKG = sp.MaPKG
WHERE pkg.MaKH = @MaKH
  AND NOT EXISTS (SELECT 1 FROM dbo.CHI_TIET_THANH_TOAN_KY_GUI ct WHERE ct.MaCTDH = c.MaCTDH);",
                Db.Param("@MaKH", _selectedCustomer));
            if (lines.Rows.Count == 0)
            {
                _lblMessage.Text = "Người ký gửi này không còn sản phẩm nào chưa quyết toán.";
                return;
            }

            var data = Db.NewSettlementLineTable();
            foreach (DataRow row in lines.Rows)
            {
                var item = data.NewRow();
                item["MaCTDH"] = row["MaCTDH"];
                data.Rows.Add(item);
            }

            using var cn = Db.Open();
            using var cmd = new SqlCommand("dbo.sp_LapPhieuQuyetToan", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@MaKH", _selectedCustomer);
            cmd.Parameters.AddWithValue("@MaNV", Convert.ToInt32(_cboEmployee.SelectedValue ?? 1));
            cmd.Parameters.AddWithValue("@PhuongThuc", Convert.ToString(_cboMethod.SelectedItem));
            cmd.Parameters.AddWithValue("@GhiChu", _txtNote.Text.Trim().Length == 0 ? DBNull.Value : _txtNote.Text.Trim());
            cmd.Parameters.Add(Db.Structured("@DanhSach", "dbo.DanhSachCTDHType", data));
            var output = new SqlParameter("@MaPTTKG", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(output);
            cmd.ExecuteNonQuery();

            var id = Convert.ToInt32(output.Value);
            MessageBox.Show($"Đã lập phiếu quyết toán #{id} cho {lines.Rows.Count} sản phẩm.\nTrạng thái: Chờ thanh toán — xác nhận chi tiền ở danh sách bên dưới.",
                "Lập phiếu thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _txtNote.Clear();
            LoadReport();
            LoadPendingSlips();
        }
        catch (Exception ex)
        {
            _lblMessage.Text = Db.ErrorMessage(ex);
        }
    }

    private void ConfirmPayout()
    {
        _lblMessage.Text = string.Empty;
        if (_gridPending.CurrentRow is null)
        {
            _lblMessage.Text = "Chọn một phiếu quyết toán đang chờ chi.";
            return;
        }
        try
        {
            var maPttkg = Convert.ToInt32(_gridPending.CurrentRow.Cells["MaPTTKG"].Value);
            using var cn = Db.Open();
            using var cmd = new SqlCommand("dbo.sp_ThanhToanPhieuKyGui", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@MaPTTKG", maPttkg);
            cmd.Parameters.AddWithValue("@MaGiaoDich", "PC" + DateTime.Now.ToString("yyMMddHHmm"));
            cmd.Parameters.AddWithValue("@GhiChu", "Xác nhận chi tiền tại quầy");
            cmd.ExecuteNonQuery();

            MessageBox.Show($"Đã xác nhận chi tiền cho phiếu #{maPttkg}.", "Quyết toán", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadReport();
            LoadPendingSlips();
        }
        catch (Exception ex)
        {
            _lblMessage.Text = Db.ErrorMessage(ex);
        }
    }

    private void Export()
    {
        var path = ExportCore();
        if (path is not null)
            MessageBox.Show("Đã xuất Excel:\n" + path, "Xuất thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    internal string? ExportForDemo() => ExportCore();

    private string? ExportCore()
    {
        if (_current.Rows.Count == 0)
        {
            MessageBox.Show("Không có dữ liệu để xuất. Hãy chọn kỳ báo cáo có sản phẩm đã bán.", "Xuất Excel",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return null;
        }
        try
        {
            var sales = _current.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["GiaBan"]));
            var commission = _current.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["TienHoaHong"]));
            var payout = _current.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["TienThucNhan"]));
            var summary = new (string, string)[]
            {
                ("Tổng giá bán", sales.ToString("#,##0") + " đ"),
                ("Hoa hồng sàn (20%)", commission.ToString("#,##0") + " đ"),
                ("Tiền phải trả người ký gửi", payout.ToString("#,##0") + " đ"),
                ("Số sản phẩm", _current.Rows.Count.ToString("N0"))
            };
            var exportTable = _current.DefaultView.ToTable(false,
                "MaDH", "NgayLap", "TenSP", "KichThuoc", "MauSac", "TenNguoiKyGui", "SoDienThoai",
                "GiaBan", "TienHoaHong", "TienThucNhan", "TrangThaiQuyetToan");
            var captions = new Dictionary<string, string>
            {
                ["MaDH"] = "Mã đơn",
                ["NgayLap"] = "Ngày bán",
                ["TenSP"] = "Sản phẩm",
                ["KichThuoc"] = "Size",
                ["MauSac"] = "Màu",
                ["TenNguoiKyGui"] = "Người ký gửi",
                ["SoDienThoai"] = "SĐT",
                ["GiaBan"] = "Giá bán",
                ["TienHoaHong"] = "Hoa hồng",
                ["TienThucNhan"] = "Thực nhận",
                ["TrangThaiQuyetToan"] = "Trạng thái"
            };
            foreach (var pair in captions) exportTable.Columns[pair.Key].Caption = pair.Value;

            return ExcelExporter.Export(
                exportTable,
                "Báo cáo quyết toán người ký gửi",
                $"Kỳ báo cáo: {_dtpFrom.Value:dd/MM/yyyy} – {_dtpTo.Value:dd/MM/yyyy}",
                "bao-cao-quyet-toan",
                PreparedBy,
                ReceivedBy,
                summary,
                new[] { "GiaBan", "TienHoaHong", "TienThucNhan" });
        }
        catch (Exception ex)
        {
            MessageBox.Show("Không xuất được Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }

    public void PrepareCapture()
    {
        _cboCustomer.SelectedIndex = 0;
        _cboStatus.SelectedIndex = 1;
        _dtpFrom.Value = new DateTime(2026, 1, 1);
        _dtpTo.Value = new DateTime(2026, 11, 30);
        LoadReport();
        LoadPendingSlips();
        if (_grid.Rows.Count > 0)
        {
            _grid.CurrentCell = _grid.Rows[0].Cells["TenSP"];
            _grid.Rows[0].Selected = true;
        }
        UpdateSelectedCustomer();
    }
}
