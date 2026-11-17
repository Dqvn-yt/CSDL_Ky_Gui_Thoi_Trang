using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;

namespace QuanLyKyGui;

internal sealed class frmBaoCaoDoanhThu : Form
{
    private const string PreparedBy = "Nhân viên kế toán (thu ngân)";
    private const string ReceivedBy = "Quản lý cửa hàng / Ban giám đốc";

    private readonly DateTimePicker _dtpFrom = Theme.DatePicker(150);
    private readonly DateTimePicker _dtpTo = Theme.DatePicker(150);
    private readonly DataGridView _grid = new();
    private readonly BarChart _chart = new();
    private readonly StatCard _cardRevenue = new("TỔNG DOANH THU", Theme.Accent);
    private readonly StatCard _cardCommission = new("HOA HỒNG SÀN (20%)", Theme.Gold);
    private readonly StatCard _cardPayout = new("TIỀN TRẢ NGƯỜI KÝ GỬI", Theme.Success);
    private readonly StatCard _cardOrders = new("ĐƠN HÀNG HOÀN TẤT", ColorTranslator.FromHtml("#3C5A78"));
    private readonly Label _lblPeriod = new() { AutoSize = true, Font = Theme.Small, ForeColor = Theme.Muted };
    private readonly Label _lblMessage = new() { AutoSize = true, Font = Theme.Small, ForeColor = Theme.Danger };
    private DataTable _current = new();

    public frmBaoCaoDoanhThu()
    {
        Text = "Báo cáo doanh thu — Thiên Kim Consignment";
        ClientSize = new Size(1360, 830);
        MinimumSize = new Size(1180, 740);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Theme.Canvas;
        Font = Theme.Body;

        BuildUi();
        _grid.CellFormatting += (_, e) =>
        {
            if (e.ColumnIndex == _grid.Columns["Thang"].Index && e.Value is string text && text.Length == 7 && text[4] == '-')
            {
                e.Value = "T" + text[5..].TrimStart('0') + "/" + text[..4];
                e.FormattingApplied = true;
            }
        };
        var today = DateTime.Today;
        _dtpFrom.Value = new DateTime(today.Year, 1, 1);
        _dtpTo.Value = today;
        Shown += (_, _) => LoadReport();
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
            Text = "Báo cáo doanh thu",
            Font = Theme.H1,
            ForeColor = Theme.Ink,
            AutoSize = true,
            Location = new Point(0, 0)
        });
        titles.Controls.Add(new Label
        {
            Text = "Doanh thu, hoa hồng và tiền phải trả người ký gửi theo tháng — chỉ tính đơn đã Hoàn tất.",
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
        var card = new Card("Kỳ báo cáo", 40);
        card.Dock = DockStyle.Fill;
        card.Margin = new Padding(0, 0, 0, 12);
        card.Padding = new Padding(16, 38, 16, 8);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(0) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Margin = new Padding(0) };
        flow.Controls.Add(Fields.Field("Từ ngày", _dtpFrom));
        flow.Controls.Add(Fields.Field("Đến ngày", _dtpTo));
        var run = Theme.PrimaryButton("Xem báo cáo", 150);
        run.Margin = new Padding(4, 22, 8, 0);
        run.Click += (_, _) => LoadReport();
        flow.Controls.Add(run);
        var quick = Theme.GhostButton("Cả năm nay", 130);
        quick.Margin = new Padding(0, 22, 8, 0);
        quick.Click += (_, _) =>
        {
            _dtpFrom.Value = new DateTime(DateTime.Today.Year, 1, 1);
            _dtpTo.Value = DateTime.Today;
            LoadReport();
        };
        flow.Controls.Add(quick);
        flow.Controls.Add(Fields.Field("Người lập báo cáo", new Label
        {
            Text = PreparedBy,
            AutoSize = true,
            Font = Theme.Bold,
            ForeColor = Theme.Ink,
            Margin = new Padding(0, 5, 0, 0)
        }));
        flow.Controls.Add(Fields.Field("Người nhận báo cáo", new Label
        {
            Text = ReceivedBy,
            AutoSize = true,
            Font = Theme.Bold,
            ForeColor = Theme.Ink,
            Margin = new Padding(0, 5, 0, 0)
        }));
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
        var cards = new[] { _cardRevenue, _cardCommission, _cardPayout, _cardOrders };
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
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46));
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var chartCard = new Card("Biểu đồ doanh thu theo tháng");
        chartCard.Dock = DockStyle.Fill;
        chartCard.Margin = new Padding(0, 0, 12, 0);
        _chart.Dock = DockStyle.Fill;
        chartCard.Body.Controls.Add(_chart);
        table.Controls.Add(chartCard, 0, 0);

        var gridCard = new Card("Bảng số liệu chi tiết");
        gridCard.Dock = DockStyle.Fill;
        Theme.StyleGrid(_grid);
        _grid.Dock = DockStyle.Fill;
        gridCard.Body.Controls.Add(_grid);
        table.Controls.Add(gridCard, 1, 0);
        return table;
    }

    private Control BuildFooter()
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        _lblMessage.Text = "Nguồn: DON_HANG + CHI_TIET_DON_HANG (TrangThai = Hoàn tất) — hoa hồng 20%, làm tròn đến đồng.";
        _lblMessage.ForeColor = Theme.Muted;
        _lblMessage.Margin = new Padding(2, 8, 0, 0);
        panel.Controls.Add(_lblMessage, 0, 0);
        var owners = new Label
        {
            Text = $"Người lập: {PreparedBy}   •   Người nhận: {ReceivedBy}",
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Anchor = AnchorStyles.Right,
            Margin = new Padding(0, 8, 2, 0)
        };
        panel.Controls.Add(owners, 1, 0);
        return panel;
    }

    private void LoadReport()
    {
        _lblMessage.ForeColor = Theme.Danger;
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

            var table = Db.Procedure("dbo.sp_BaoCaoDoanhThu",
                new SqlParameter("@TuNgay", SqlDbType.Date) { Value = from },
                new SqlParameter("@DenNgay", SqlDbType.Date) { Value = to });
            _current = table;

            table.Columns["Thang"].Caption = "Tháng";
            table.Columns["SoDon"].Caption = "Số đơn";
            table.Columns["SoSanPham"].Caption = "Số sản phẩm";
            table.Columns["DoanhThu"].Caption = "Doanh thu";
            table.Columns["TienHoaHong"].Caption = "Hoa hồng";
            table.Columns["TienTraNguoiKyGui"].Caption = "Trả ký gửi";

            _grid.DataSource = table;
            _grid.Columns["Thang"].HeaderText = "Tháng";
            _grid.Columns["SoDon"].HeaderText = "Số đơn";
            _grid.Columns["SoSanPham"].HeaderText = "Số SP";
            _grid.Columns["DoanhThu"].HeaderText = "Doanh thu";
            _grid.Columns["TienHoaHong"].HeaderText = "Hoa hồng";
            _grid.Columns["TienTraNguoiKyGui"].HeaderText = "Trả ký gửi";
            foreach (var name in new[] { "SoDon", "SoSanPham", "TienHoaHong", "TienTraNguoiKyGui" })
                if (_grid.Columns.Contains(name)) Theme.MoneyColumn(_grid.Columns[name]);
            Theme.MoneyColumn(_grid.Columns["DoanhThu"]);
            _grid.Columns["SoDon"].DefaultCellStyle.Format = "#,##0";
            _grid.Columns["SoSanPham"].DefaultCellStyle.Format = "#,##0";
            _grid.Columns["Thang"].FillWeight = 90;
            _grid.Columns["Thang"].MinimumWidth = 85;
            _grid.Columns["SoDon"].FillWeight = 55;
            _grid.Columns["SoSanPham"].FillWeight = 70;
            _grid.Columns["DoanhThu"].FillWeight = 95;
            _grid.Columns["TienHoaHong"].FillWeight = 95;
            _grid.Columns["TienTraNguoiKyGui"].FillWeight = 100;

            var revenue = table.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["DoanhThu"]));
            var commission = table.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["TienHoaHong"]));
            var payout = table.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["TienTraNguoiKyGui"]));
            var orders = table.Rows.Cast<DataRow>().Sum(r => Convert.ToInt32(r["SoDon"]));

            _cardRevenue.SetValue(revenue.ToString("#,##0") + " đ", $"{table.Rows.Count} tháng có doanh thu");
            _cardCommission.SetValue(commission.ToString("#,##0") + " đ", "Tỷ lệ cố định 20%");
            _cardPayout.SetValue(payout.ToString("#,##0") + " đ", "Nghĩa vụ với người ký gửi");
            _cardOrders.SetValue(orders.ToString("N0"), "Đơn đã bàn giao đủ");

            _chart.Data.Clear();
            foreach (DataRow row in table.Rows)
            {
                var month = Convert.ToString(row["Thang"]) ?? "";
                var label = month.Length == 7
                    ? "T" + month[5..].TrimStart('0') + "/" + month[..4]
                    : month;
                _chart.Data.Add((label, Convert.ToDecimal(row["DoanhThu"])));
            }
            _chart.Invalidate();

            _lblPeriod.Text = $"Kỳ báo cáo: {from:dd/MM/yyyy} – {to:dd/MM/yyyy}   •   {table.Rows.Count} dòng số liệu   •   Lập lúc {DateTime.Now:HH:mm dd/MM/yyyy}";
            if (table.Rows.Count == 0)
                _lblMessage.Text = "Không có đơn hàng Hoàn tất trong khoảng thời gian này.";
        }
        catch (Exception ex)
        {
            _lblMessage.Text = "Lỗi: " + Db.ErrorMessage(ex);
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
            MessageBox.Show("Không có dữ liệu để xuất. Hãy chọn kỳ báo cáo có đơn hoàn tất.", "Xuất Excel",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return null;
        }
        try
        {
            var revenue = _current.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["DoanhThu"]));
            var commission = _current.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["TienHoaHong"]));
            var payout = _current.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["TienTraNguoiKyGui"]));
            var orders = _current.Rows.Cast<DataRow>().Sum(r => Convert.ToInt32(r["SoDon"]));
            var summary = new (string, string)[]
            {
                ("Tổng doanh thu", revenue.ToString("#,##0") + " đ"),
                ("Hoa hồng sàn (20%)", commission.ToString("#,##0") + " đ"),
                ("Tiền trả người ký gửi", payout.ToString("#,##0") + " đ"),
                ("Số đơn hoàn tất", orders.ToString("N0"))
            };
            var exportTable = _current.Copy();
            foreach (DataRow row in exportTable.Rows)
            {
                if (row["Thang"] is string text && text.Length == 7 && text[4] == '-')
                    row["Thang"] = "T" + text[5..].TrimStart('0') + "/" + text[..4];
            }
            return ExcelExporter.Export(
                exportTable,
                "Báo cáo doanh thu theo tháng",
                $"Kỳ báo cáo: {_dtpFrom.Value:dd/MM/yyyy} – {_dtpTo.Value:dd/MM/yyyy}",
                "bao-cao-doanh-thu",
                PreparedBy,
                ReceivedBy,
                summary,
                new[] { "DoanhThu", "TienHoaHong", "TienTraNguoiKyGui" });
        }
        catch (Exception ex)
        {
            MessageBox.Show("Không xuất được Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }

    public void PrepareCapture()
    {
        _dtpFrom.Value = new DateTime(2026, 1, 1);
        _dtpTo.Value = new DateTime(2026, 11, 30);
        LoadReport();
    }
}
