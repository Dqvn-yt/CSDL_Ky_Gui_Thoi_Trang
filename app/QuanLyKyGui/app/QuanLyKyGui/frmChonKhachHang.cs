using System.ComponentModel;
using System.Data;

namespace QuanLyKyGui;

/// <summary>Tra cứu và xác nhận hồ sơ khách cho tiếp nhận hoặc lập đơn mua.</summary>
internal sealed class frmChonKhachHang : Form
{
    private readonly TextBox _search = Theme.TextBox(null, 400);
    private readonly DataGridView _grid = new();
    private readonly Label _status = new() { Dock = DockStyle.Fill, Font = Theme.Small, ForeColor = Theme.Muted };
    private readonly Button _choose = Theme.PrimaryButton("Chọn khách này", 170);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataRow? SelectedCustomer { get; private set; }

    public frmChonKhachHang(int? currentCustomerId = null)
        : this(currentCustomerId, "người ký gửi")
    {
    }

    public frmChonKhachHang(int? currentCustomerId, string role)
    {
        Text = $"Chọn {role} — Thiên Kim Consignment";
        ClientSize = new Size(850, 540);
        MinimumSize = new Size(740, 460);
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        MinimizeBox = false;
        BackColor = Theme.Canvas;
        Font = Theme.Body;

        var root = new TableLayoutPanel {
            Dock = DockStyle.Fill, Padding = new Padding(24), ColumnCount = 1, RowCount = 6
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        foreach (var height in new[] { 40, 24, 44 }) root.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        root.Controls.Add(new Label { Text = $"Tìm và chọn {role}", Font = Theme.H2, ForeColor = Theme.Ink, AutoSize = true }, 0, 0);
        root.Controls.Add(new Label { Text = "Tìm theo họ tên hoặc số điện thoại", Font = Theme.Small, ForeColor = Theme.Muted, AutoSize = true }, 0, 1);

        var searchRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, Margin = new Padding(0) };
        searchRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        searchRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96));
        searchRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 106));
        _search.Dock = DockStyle.Fill;
        _search.Margin = new Padding(0, 4, 10, 0);
        _search.PlaceholderText = "Nhập họ tên hoặc số điện thoại";
        var find = Theme.PrimaryButton("Tìm", 86);
        find.Click += (_, _) => LoadCustomers();
        var all = Theme.GhostButton("Tất cả", 96);
        all.Click += (_, _) => { _search.Clear(); LoadCustomers(); };
        searchRow.Controls.Add(_search, 0, 0);
        searchRow.Controls.Add(find, 1, 0);
        searchRow.Controls.Add(all, 2, 0);
        root.Controls.Add(searchRow, 0, 2);

        Theme.StyleGrid(_grid);
        _grid.Dock = DockStyle.Fill;
        _grid.MultiSelect = false;
        root.Controls.Add(_grid, 0, 3);
        _status.Padding = new Padding(0, 6, 0, 0);
        root.Controls.Add(_status, 0, 4);
        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var add = Theme.GhostButton("+ Thêm khách mới", 180);
        add.Click += (_, _) => AddCustomer();
        footer.Controls.Add(add, 0, 0);
        var cancel = Theme.GhostButton("Hủy", 90);
        cancel.DialogResult = DialogResult.Cancel;
        footer.Controls.Add(Theme.Actions(cancel, _choose), 1, 0);
        root.Controls.Add(footer, 0, 5);
        Controls.Add(root);

        _choose.Enabled = false;
        _choose.Click += (_, _) => ChooseCustomer();
        _grid.SelectionChanged += (_, _) => _choose.Enabled = _grid.CurrentRow?.DataBoundItem is DataRowView;
        _grid.CurrentCellChanged += (_, _) => _choose.Enabled = _grid.CurrentRow?.DataBoundItem is DataRowView;
        _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) ChooseCustomer(); };
        _search.KeyDown += (_, e) => {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; LoadCustomers(); }
        };
        CancelButton = cancel;
        Shown += (_, _) => { LoadCustomers(currentCustomerId); _search.Focus(); };
    }

    private void LoadCustomers(int? selectId = null)
    {
        try
        {
            var table = Db.Query("""
                SELECT MaKH, HoTen, SoDienThoai, Email, DiaChi FROM dbo.KHACH_HANG
                WHERE @Key = N'' OR CHARINDEX(@Key, HoTen) > 0 OR CHARINDEX(@Key, SoDienThoai) > 0
                ORDER BY HoTen, MaKH;
                """, Db.Param("@Key", _search.Text.Trim()));
            Grid.Bind(_grid, table, new Dictionary<string, string> {
                ["MaKH"] = "Mã KH", ["HoTen"] = "Họ tên", ["SoDienThoai"] = "Số điện thoại", ["DiaChi"] = "Địa chỉ"
            }, "Email");
            _grid.Columns["MaKH"]!.FillWeight = 45;
            _grid.Columns["HoTen"]!.FillWeight = 130;
            _grid.Columns["SoDienThoai"]!.FillWeight = 95;
            _grid.Columns["DiaChi"]!.FillWeight = 160;
            _grid.ClearSelection();
            _grid.CurrentCell = null;
            if (selectId.HasValue)
            {
                foreach (DataGridViewRow row in _grid.Rows)
                    if (Convert.ToInt32(row.Cells["MaKH"].Value) == selectId.Value)
                    {
                        _grid.CurrentCell = row.Cells["HoTen"];
                        row.Selected = true;
                        break;
                    }
            }
            _choose.Enabled = _grid.CurrentRow?.DataBoundItem is DataRowView;
            _status.ForeColor = Theme.Muted;
            _status.Text = table.Rows.Count == 0
                ? "Không tìm thấy khách. Bạn có thể thêm khách mới ở bên dưới."
                : $"{table.Rows.Count} khách hàng. Chọn một dòng rồi bấm “Chọn khách này”.";
        }
        catch (Exception ex)
        {
            _grid.DataSource = null;
            _choose.Enabled = false;
            _status.ForeColor = Theme.Danger;
            _status.Text = "Không tải được khách hàng: " + Db.ErrorMessage(ex);
        }
    }

    private void ChooseCustomer()
    {
        if (_grid.CurrentRow?.DataBoundItem is not DataRowView row) return;
        SelectedCustomer = row.Row;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void AddCustomer()
    {
        var keyword = _search.Text.Trim();
        var phone = keyword.Length > 0 && keyword.All(ch => char.IsDigit(ch) || "+ .-".Contains(ch));
        using var dialog = new frmKhachHangMoi(phone ? null : keyword, phone ? keyword : null);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        _search.Clear();
        LoadCustomers(dialog.NewCustomerId);
        _status.Text = "Đã thêm khách mới. Kiểm tra hồ sơ rồi bấm “Chọn khách này”.";
    }
}
