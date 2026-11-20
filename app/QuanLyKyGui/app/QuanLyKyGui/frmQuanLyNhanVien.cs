using System.Data;

namespace QuanLyKyGui;

internal sealed class frmQuanLyNhanVien : Form
{
    private readonly DataGridView _grid = new();
    private readonly TextBox _search = Theme.TextBox(null, 260);
    private readonly ComboBox _status = Theme.Combo(190);
    private readonly Label _message = new() { Dock = DockStyle.Fill, Font = Theme.Small, ForeColor = Theme.Muted };
    private readonly Button _remove = Theme.GhostButton("Xóa nhân viên", 150);
    private readonly Button _toggle = Theme.GhostButton("Cho nghỉ việc", 150);
    private DataTable? _table;

    public frmQuanLyNhanVien()
    {
        Text = "Quản lý nhân viên — Thiên Kim Consignment";
        ClientSize = new Size(1080, 720);
        BackColor = Theme.Canvas;
        Font = Theme.Body;
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24, 18, 24, 12), ColumnCount = 1, RowCount = 5 };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));

        var heading = new Panel { Dock = DockStyle.Fill };
        heading.Controls.Add(new Label { Text = "Quản lý nhân viên", Font = Theme.H1, ForeColor = Theme.Ink, AutoSize = true });
        heading.Controls.Add(new Label { Text = "Quản lý có thể thêm nhân viên và thực hiện toàn bộ nghiệp vụ cửa hàng.",
            Font = Theme.Small, ForeColor = Theme.Muted, AutoSize = true, Location = new Point(1, 40) });
        root.Controls.Add(heading, 0, 0);

        _search.PlaceholderText = "Mã, họ tên hoặc số điện thoại";
        _status.Items.AddRange(new object[] { "Tất cả trạng thái", "Đang làm việc", "Đã nghỉ" });
        _status.SelectedIndex = 0;
        var filters = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true };
        filters.Controls.Add(Fields.Field("Tìm nhân viên", _search));
        filters.Controls.Add(Fields.Field("Trạng thái", _status));
        var refresh = Theme.GhostButton("Làm mới", 110);
        refresh.Margin = new Padding(0, 20, 0, 0);
        refresh.Click += (_, _) => LoadData();
        filters.Controls.Add(refresh);
        root.Controls.Add(filters, 0, 1);

        Theme.StyleGrid(_grid);
        _grid.Dock = DockStyle.Fill;
        _grid.MultiSelect = false;
        root.Controls.Add(_grid, 0, 2);
        _message.Padding = new Padding(0, 8, 0, 0);
        root.Controls.Add(_message, 0, 3);

        var add = Theme.PrimaryButton("Thêm nhân viên", 170);
        add.Click += (_, _) =>
        {
            using var dialog = new frmNhanVienMoi();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _search.Clear();
                _status.SelectedIndex = 0;
                LoadData();
            }
        };
        _remove.Click += (_, _) => RemoveSelected();
        _toggle.Click += (_, _) => ToggleSelected();
        var close = Theme.GhostButton("Đóng", 100);
        close.Click += (_, _) => Close();
        var actions = Theme.Actions(close, _toggle, _remove, add);
        actions.Dock = DockStyle.Right;
        root.Controls.Add(actions, 0, 4);
        Controls.Add(root);
        _search.TextChanged += (_, _) => ApplyFilter();
        _status.SelectedIndexChanged += (_, _) => ApplyFilter();
        _grid.SelectionChanged += (_, _) => UpdateActions();
        Shown += (_, _) => LoadData();
    }

    private DataRowView? Selected => _grid.CurrentRow?.DataBoundItem as DataRowView;

    private void LoadData()
    {
        try
        {
            _table = EmployeeService.List();
            Grid.Bind(_grid, _table, new Dictionary<string, string> {
                ["MaNV"] = "Mã NV", ["HoTen"] = "Họ tên", ["SoDienThoai"] = "Số điện thoại",
                ["Email"] = "Email", ["ChucVu"] = "Chức vụ", ["TrangThai"] = "Trạng thái"
            }, "SoNghiepVu");
            _grid.Columns["MaNV"]!.FillWeight = 50;
            _grid.Columns["HoTen"]!.FillWeight = 145;
            _grid.Columns["Email"]!.FillWeight = 145;
            ApplyFilter();
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void ApplyFilter()
    {
        if (_table is null) return;
        // Lọc bằng giá trị trong bộ nhớ để từ khóa chứa dấu nháy hoặc ký tự LIKE vẫn được tìm đúng.
        var key = _search.Text.Trim();
        var state = _status.SelectedIndex <= 0 ? null : Convert.ToString(_status.SelectedItem);
        var rows = _table.Rows.Cast<DataRow>().Where(row =>
            (state is null || Convert.ToString(row["TrangThai"]) == state) &&
            new[] { "MaNV", "HoTen", "SoDienThoai" }.Any(column =>
                (Convert.ToString(row[column]) ?? "").Contains(key, StringComparison.CurrentCultureIgnoreCase)));
        var filtered = _table.Clone();
        foreach (var row in rows) filtered.ImportRow(row);
        _grid.DataSource = filtered;
        _message.ForeColor = Theme.Muted;
        _message.Text = string.Empty;
        UpdateActions();
    }

    private void UpdateActions()
    {
        var row = Selected;
        var allowed = row is not null && Convert.ToInt32(row["MaNV"]) != DemoSession.ManagerId;
        _remove.Enabled = allowed;
        _toggle.Enabled = allowed;
        _toggle.Text = row is not null && Convert.ToString(row["TrangThai"]) == "Đã nghỉ" ? "Làm việc lại" : "Cho nghỉ việc";
    }

    private void RemoveSelected()
    {
        var row = Selected;
        if (row is null) return;
        var id = Convert.ToInt32(row["MaNV"]);
        if (MessageBox.Show($"Bạn có chắc muốn xóa nhân viên {row["HoTen"]}?", "Xác nhận xóa nhân viên",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            var retired = EmployeeService.Remove(id);
            LoadData();
            _message.Text = retired ? "Đã chuyển nhân viên sang Đã nghỉ và giữ nguyên lịch sử nghiệp vụ." : "Đã xóa hồ sơ nhân viên chưa phát sinh nghiệp vụ.";
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void ToggleSelected()
    {
        var row = Selected;
        if (row is null) return;
        var active = Convert.ToString(row["TrangThai"]) == "Đã nghỉ";
        if (MessageBox.Show($"{(active ? "Cho làm việc lại" : "Cho nghỉ việc")} nhân viên {row["HoTen"]}?",
            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try { EmployeeService.SetActive(Convert.ToInt32(row["MaNV"]), active); LoadData(); }
        catch (Exception ex) { ShowError(ex); }
    }

    private void ShowError(Exception ex)
    {
        _message.ForeColor = Theme.Danger;
        _message.Text = Db.ErrorMessage(ex);
    }
}
