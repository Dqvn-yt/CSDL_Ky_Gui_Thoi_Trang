using System.Data;
using Microsoft.Data.SqlClient;

namespace QuanLyKyGui;

internal sealed class frmLapDonHang : Form
{
    private readonly ComboBox _cboCustomer = Theme.Combo(240);
    private readonly ComboBox _cboEmployee = Theme.Combo(230);
    private readonly DateTimePicker _dtpOrder = Theme.DatePicker(170);
    private readonly DateTimePicker _dtpDue = Theme.DatePicker(170);
    private readonly TextBox _txtNote = Theme.TextBox(null, 260);

    private readonly TextBox _txtSearch = Theme.TextBox(null, 196);
    private readonly DataGridView _gridProducts = new();
    private readonly NumericUpDown _numDiscount = Theme.Number(100_000_000m, 110);
    private readonly Label _lblProductCount = new() { AutoSize = true, Font = Theme.Small, ForeColor = Theme.Muted };

    private readonly DataGridView _gridDraft = new();
    private readonly DataTable _draft;
    private readonly Label _lblTotal = new() { AutoSize = true, Font = new Font("Segoe UI Semibold", 12f), ForeColor = Theme.Accent };
    private readonly Label _lblDraftError = new() { AutoSize = true, Font = Theme.Small, ForeColor = Theme.Danger };

    private readonly DataGridView _gridOrders = new();
    private readonly Label _lblOrderInfo = new() { AutoSize = false, Height = 24, Font = Theme.Small, ForeColor = Theme.Muted };
    private readonly NumericUpDown _numPay = Theme.Number(1_000_000_000m, 120);
    private readonly ComboBox _cboMethod = Theme.Combo(104);
    private readonly ComboBox _cboPayStatus = Theme.Combo(104);
    private readonly TextBox _txtTxn = Theme.TextBox(null, 120);
    private readonly TextBox _txtPayNote = Theme.TextBox(null, 150);
    private readonly TextBox _txtCancelReason = Theme.TextBox(null, 170);

    private (int MaDH, decimal TongTien, decimal DaThu, decimal ConLai, string TrangThai)? _selected;

    public frmLapDonHang()
    {
        Text = "Lập đơn hàng & thu tiền — Thiên Kim Consignment";
        ClientSize = new Size(1360, 880);
        MinimumSize = new Size(1180, 740);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Theme.Canvas;
        Font = Theme.Body;

        _draft = new DataTable();
        _draft.Columns.Add("MaSP", typeof(int));
        _draft.Columns.Add("TenSP", typeof(string));
        _draft.Columns.Add("KichThuoc", typeof(string));
        _draft.Columns.Add("GiaDangBan", typeof(decimal));
        _draft.Columns.Add("GiamGia", typeof(decimal));
        _draft.Columns.Add("ThanhTien", typeof(decimal), "GiaDangBan - GiamGia");

        BuildUi();
        LoadLookups();
        LoadProducts();
        LoadOrders();
        Shown += (_, _) => _txtSearch.Focus();
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(20, 14, 20, 14),
            BackColor = Theme.Canvas
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.Controls.Add(BuildTitle(), 0, 0);

        var body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var left = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(0, 0, 14, 0) };
        left.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 292));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        left.Controls.Add(BuildInfoCard(), 0, 0);
        left.Controls.Add(BuildDraftCard(), 0, 1);
        body.Controls.Add(left, 0, 0);
        body.Controls.Add(BuildProcessColumn(), 1, 0);
        root.Controls.Add(body, 0, 1);
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
            Text = "Lập đơn hàng & thu tiền",
            Font = Theme.H1,
            ForeColor = Theme.Ink,
            AutoSize = true,
            Location = new Point(0, 0)
        });
        titles.Controls.Add(new Label
        {
            Text = "Chọn sản phẩm đang bán → giữ hàng → ghi nhận thanh toán → hoàn tất hoặc hủy đơn.",
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(2, 40)
        });
        row.Controls.Add(titles, 0, 0);

        var close = Theme.GhostButton("Đóng", 100);
        close.Click += (_, _) => Close();
        var reload = Theme.GhostButton("↻  Làm mới", 120);
        reload.Click += (_, _) =>
        {
            LoadProducts();
            LoadOrders(_selected?.MaDH);
        };
        var actions = Theme.Actions(close, reload);
        row.Controls.Add(actions, 1, 0);
        return row;
    }

    private Control BuildInfoCard()
    {
        var card = new Card("1. Thông tin đơn hàng");
        card.Dock = DockStyle.Fill;
        card.Margin = new Padding(0, 0, 0, 12);
        card.Padding = new Padding(16, 42, 16, 10);

        var flow = Fields.Flow(400, 190);
        flow.Dock = DockStyle.Fill;

        var customerRow = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0) };
        customerRow.Controls.Add(Fields.Field("Khách hàng mua", _cboCustomer));
        var newCustomer = Theme.GhostButton("+ Khách mới", 120);
        newCustomer.Margin = new Padding(0, 16, 8, 0);
        newCustomer.Click += (_, _) => AddCustomer();
        customerRow.Controls.Add(newCustomer);
        flow.Controls.Add(customerRow);

        flow.Controls.Add(Fields.Field("Nhân viên lập đơn", _cboEmployee));
        flow.Controls.Add(Fields.Field("Ngày lập", _dtpOrder));
        flow.Controls.Add(Fields.Field("Hạn thanh toán", _dtpDue));
        flow.Controls.Add(Fields.Field("Ghi chú", _txtNote));
        card.Body.Controls.Add(flow);
        return card;
    }

    private Control BuildDraftCard()
    {
        var card = new Card("2. Đơn hàng nháp");
        card.Dock = DockStyle.Fill;
        card.Margin = new Padding(0);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(0) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));

        Theme.StyleGrid(_gridDraft);
        _gridDraft.Dock = DockStyle.Fill;
        _gridDraft.AutoGenerateColumns = false;
        _gridDraft.Columns.Add(Theme.TextColumn("TenSP", "Sản phẩm", 170));
        _gridDraft.Columns.Add(Theme.TextColumn("KichThuoc", "Size", 50));
        _gridDraft.Columns.Add(Theme.TextColumn("GiaDangBan", "Giá niêm yết", 95, true));
        _gridDraft.Columns.Add(Theme.TextColumn("GiamGia", "Giảm giá", 85, true));
        _gridDraft.Columns.Add(Theme.TextColumn("ThanhTien", "Thành tiền", 95, true));
        _gridDraft.DataSource = _draft;
        layout.Controls.Add(_gridDraft, 0, 0);

        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(0) };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        footer.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        footer.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        var totals = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Dock = DockStyle.Fill };
        totals.Controls.Add(_lblTotal);
        _lblDraftError.AutoSize = false;
        _lblDraftError.Width = 220;
        _lblDraftError.Height = 20;
        _lblDraftError.AutoEllipsis = true;
        _lblDraftError.Margin = new Padding(12, 2, 0, 0);
        totals.Controls.Add(_lblDraftError);
        footer.Controls.Add(totals, 0, 0);
        var clear = Theme.GhostButton("Xóa dòng", 100);
        clear.Click += (_, _) => RemoveDraftLine();
        var create = Theme.PrimaryButton("Tạo đơn hàng", 160);
        create.Click += (_, _) => CreateOrderFromUi();
        var buttons = Theme.Actions(clear, create);
        buttons.Dock = DockStyle.Right;
        footer.Controls.Add(buttons, 0, 1);
        layout.Controls.Add(footer, 0, 1);

        card.Body.Controls.Add(layout);
        UpdateTotal();
        return card;
    }

    private Control BuildProcessColumn()
    {
        var column = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(0) };
        column.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        column.RowStyles.Add(new RowStyle(SizeType.Absolute, 266));
        column.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        column.Controls.Add(BuildPickerCard(), 0, 0);
        column.Controls.Add(BuildOrdersCard(), 0, 1);
        return column;
    }

    private Control BuildPickerCard()
    {
        var card = new Card("3. Sản phẩm đang bán");
        card.Dock = DockStyle.Fill;
        card.Margin = new Padding(0, 0, 0, 12);
        card.Padding = new Padding(16, 40, 16, 8);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Margin = new Padding(0) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

        var searchRow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Margin = new Padding(0) };
        _txtSearch.Width = 250;
        _txtSearch.PlaceholderText = "Lọc theo tên sản phẩm";
        _txtSearch.TextChanged += (_, _) => LoadProducts();
        _txtSearch.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                LoadProducts();
            }
        };
        searchRow.Controls.Add(_txtSearch);
        var find = Theme.GhostButton("Tìm", 64);
        find.Margin = new Padding(6, 3, 8, 0);
        find.Click += (_, _) => LoadProducts();
        searchRow.Controls.Add(find);
        _lblProductCount.Margin = new Padding(6, 10, 0, 0);
        searchRow.Controls.Add(_lblProductCount);
        layout.Controls.Add(searchRow, 0, 0);

        Theme.StyleGrid(_gridProducts);
        _gridProducts.Dock = DockStyle.Fill;
        _gridProducts.Margin = new Padding(0, 4, 0, 4);
        _gridProducts.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0) AddSelectedProduct();
        };
        layout.Controls.Add(_gridProducts, 0, 1);

        var addRow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Margin = new Padding(0) };
        addRow.Controls.Add(new Label
        {
            Text = "Giảm giá",
            AutoSize = true,
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            Margin = new Padding(0, 11, 8, 0)
        });
        _numDiscount.Margin = new Padding(0, 6, 8, 0);
        addRow.Controls.Add(_numDiscount);
        var add = Theme.GoldButton("+  Thêm vào đơn", 160);
        add.Margin = new Padding(0, 4, 8, 0);
        add.Click += (_, _) => AddSelectedProduct();
        addRow.Controls.Add(add);
        addRow.Controls.Add(new Label
        {
            Text = "Nhấp đúp dòng sản phẩm để thêm nhanh.",
            AutoSize = true,
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            Margin = new Padding(10, 11, 0, 0)
        });
        layout.Controls.Add(addRow, 0, 2);

        card.Body.Controls.Add(layout);
        return card;
    }

    private Control BuildOrdersCard()
    {
        var card = new Card("4. Đơn hàng gần đây & xử lý thanh toán");
        card.Dock = DockStyle.Fill;
        card.Margin = new Padding(0);
        card.Padding = new Padding(16, 40, 16, 8);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Margin = new Padding(0) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));

        Theme.StyleGrid(_gridOrders);
        _gridOrders.Dock = DockStyle.Fill;
        _gridOrders.Margin = new Padding(0, 2, 0, 4);
        _gridOrders.SelectionChanged += (_, _) => ShowSelectedOrder();
        layout.Controls.Add(_gridOrders, 0, 0);

        _lblOrderInfo.Dock = DockStyle.Fill;
        _lblOrderInfo.Text = "Chọn một đơn hàng để thu tiền hoặc hoàn tất.";
        _lblOrderInfo.AutoEllipsis = true;
        layout.Controls.Add(_lblOrderInfo, 0, 1);

        var payRow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Margin = new Padding(0) };
        _cboMethod.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản", "Thẻ", "Ví điện tử" });
        _cboMethod.SelectedIndex = 0;
        _cboPayStatus.Items.AddRange(new object[] { "Thành công", "Chờ xử lý", "Thất bại" });
        _cboPayStatus.SelectedIndex = 0;
        payRow.Controls.Add(Fields.Field("Số tiền thu", _numPay));
        payRow.Controls.Add(Fields.Field("Phương thức", _cboMethod));
        payRow.Controls.Add(Fields.Field("Kết quả giao dịch", _cboPayStatus));
        payRow.Controls.Add(Fields.Field("Mã giao dịch", _txtTxn));
        payRow.Controls.Add(Fields.Field("Ghi chú", _txtPayNote));
        layout.Controls.Add(payRow, 0, 2);

        var actionRow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Margin = new Padding(0) };
        var pay = Theme.PrimaryButton("Thu tiền", 120);
        pay.Click += (_, _) => RecordPayment();
        actionRow.Controls.Add(pay);
        var complete = Theme.GoldButton("Hoàn tất đơn", 140);
        complete.Click += (_, _) => CompleteOrder();
        actionRow.Controls.Add(complete);
        var cancel = Theme.GhostButton("Hủy đơn", 100);
        cancel.Click += (_, _) => CancelOrder();
        actionRow.Controls.Add(cancel);
        actionRow.Controls.Add(Fields.Field("Lý do hủy", _txtCancelReason));
        layout.Controls.Add(actionRow, 0, 3);

        card.Body.Controls.Add(layout);
        return card;
    }

    private void LoadCustomers(int? selectId = null)
    {
        var customers = Db.Query("SELECT MaKH, HoTen, SoDienThoai FROM dbo.KHACH_HANG ORDER BY HoTen");
        var view = customers.Copy();
        view.Columns.Add("HienThi", typeof(string));
        foreach (DataRow row in view.Rows) row["HienThi"] = $"{row["HoTen"]} — {row["SoDienThoai"]}";
        _cboCustomer.DataSource = view;
        _cboCustomer.DisplayMember = "HienThi";
        _cboCustomer.ValueMember = "MaKH";

        if (selectId is int id)
        {
            for (var i = 0; i < view.Rows.Count; i++)
            {
                if (Convert.ToInt32(view.Rows[i]["MaKH"]) == id)
                {
                    _cboCustomer.SelectedIndex = i;
                    break;
                }
            }
        }
    }

    private void AddCustomer()
    {
        using var dialog = new frmKhachHangMoi();
        var ownerWindow = Parent?.FindForm() ?? FindForm();
        var result = ownerWindow is null ? dialog.ShowDialog() : dialog.ShowDialog(ownerWindow);
        if (result != DialogResult.OK) return;

        LoadCustomers(dialog.NewCustomerId);
        SetOrderMessage($"Đã thêm khách hàng mới: {dialog.NewCustomerName}. Đơn hàng sẽ dùng khách này.", false);
    }

    private void LoadLookups()
    {
        try
        {
            LoadCustomers();

            var employees = Db.Query("SELECT MaNV, HoTen, ChucVu FROM dbo.NHAN_VIEN WHERE TrangThai = N'Đang làm việc' ORDER BY HoTen");
            var ev = employees.Copy();
            ev.Columns.Add("HienThi", typeof(string));
            foreach (DataRow row in ev.Rows) row["HienThi"] = $"{row["HoTen"]} ({row["ChucVu"]})";
            _cboEmployee.DataSource = ev;
            _cboEmployee.DisplayMember = "HienThi";
            _cboEmployee.ValueMember = "MaNV";

            _dtpOrder.Value = DateTime.Now;
            _dtpDue.Value = DateTime.Now.AddDays(3);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Không tải được danh mục: " + Db.ErrorMessage(ex), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadProducts()
    {
        try
        {
            var key = _txtSearch.Text.Trim();
            var table = Db.Query(@"
SELECT MaSP, TenSP, TenDanhMuc, KichThuoc, TenNguoiKyGui, GiaDangBan
FROM dbo.vw_SanPham_DayDu
WHERE TrangThaiBan = N'Đang bán'
  AND (@Key = N'' OR TenSP LIKE N'%' + @Key + N'%' OR TenNguoiKyGui LIKE N'%' + @Key + N'%' OR TenThuongHieu LIKE N'%' + @Key + N'%')
ORDER BY NgayDangBan DESC, MaSP DESC;", Db.Param("@Key", key));
            _gridProducts.DataSource = table;
            Grid.Bind(_gridProducts, table, new Dictionary<string, string>
            {
                ["TenSP"] = "Sản phẩm",
                ["TenDanhMuc"] = "Danh mục",
                ["KichThuoc"] = "Size",
                ["TenNguoiKyGui"] = "Người ký gửi",
                ["GiaDangBan"] = "Giá đăng bán"
            });
            Theme.MoneyColumn(_gridProducts.Columns["GiaDangBan"]);
            _gridProducts.Columns["MaSP"].FillWeight = 48;
            _gridProducts.Columns["TenSP"].FillWeight = 185;
            _gridProducts.Columns["TenDanhMuc"].FillWeight = 90;
            _gridProducts.Columns["KichThuoc"].FillWeight = 55;
            _gridProducts.Columns["TenNguoiKyGui"].FillWeight = 125;
            _gridProducts.Columns["GiaDangBan"].FillWeight = 98;
            _lblProductCount.Text = $"{table.Rows.Count} sản phẩm đang bán";
        }
        catch (Exception ex)
        {
            SetOrderMessage("Không tải được sản phẩm: " + Db.ErrorMessage(ex), true);
        }
    }

    private void LoadOrders(int? selectId = null)
    {
        try
        {
            var table = Db.Query(@"
SELECT TOP 30 MaDH, TenKhachHang, NgayLap, TrangThai, TongTien, DaThu, ConLai
FROM dbo.vw_DonHang_TongHop
ORDER BY NgayLap DESC, MaDH DESC;");
            _gridOrders.DataSource = table;
            Grid.Bind(_gridOrders, table, new Dictionary<string, string>
            {
                ["MaDH"] = "Mã",
                ["TenKhachHang"] = "Khách hàng",
                ["TrangThai"] = "Trạng thái",
                ["TongTien"] = "Tổng tiền",
                ["ConLai"] = "Còn lại"
            }, "NgayLap", "DaThu");
            foreach (var name in new[] { "TongTien", "ConLai" })
                Theme.MoneyColumn(_gridOrders.Columns[name]);
            _gridOrders.Columns["MaDH"].FillWeight = 55;
            _gridOrders.Columns["TenKhachHang"].FillWeight = 190;
            _gridOrders.Columns["TrangThai"].FillWeight = 105;
            _gridOrders.Columns["TongTien"].FillWeight = 100;
            _gridOrders.Columns["ConLai"].FillWeight = 100;

            if (selectId.HasValue)
            {
                foreach (DataGridViewRow row in _gridOrders.Rows)
                {
                    if (Convert.ToInt32(row.Cells["MaDH"].Value) == selectId.Value)
                    {
                        row.Selected = true;
                        _gridOrders.CurrentCell = row.Cells["MaDH"];
                        break;
                    }
                }
            }
            ShowSelectedOrder();
        }
        catch (Exception ex)
        {
            SetOrderMessage("Không tải được đơn hàng: " + Db.ErrorMessage(ex), true);
        }
    }

    private void ShowSelectedOrder()
    {
        if (_gridOrders.CurrentRow is null)
        {
            _selected = null;
            _lblOrderInfo.Text = "Chọn một đơn hàng để thu tiền hoặc hoàn tất.";
            _lblOrderInfo.ForeColor = Theme.Muted;
            return;
        }

        var row = _gridOrders.CurrentRow;
        var maDh = Convert.ToInt32(row.Cells["MaDH"].Value);
        var total = Convert.ToDecimal(row.Cells["TongTien"].Value);
        var paid = Convert.ToDecimal(row.Cells["DaThu"].Value);
        var remain = Convert.ToDecimal(row.Cells["ConLai"].Value);
        var status = Convert.ToString(row.Cells["TrangThai"].Value) ?? "";
        _selected = (maDh, total, paid, remain, status);

        var extra = status == "Chờ thanh toán" && remain < 0 ? "  •  Đã trả thừa, chờ đối soát" : string.Empty;
        _lblOrderInfo.ForeColor = Theme.Muted;
        _lblOrderInfo.Text =
            $"Đơn #{maDh}  •  {row.Cells["TenKhachHang"].Value}  •  {status}  —  Tổng {total:#,##0} đ  •  Đã thu {paid:#,##0} đ  •  Còn lại {remain:#,##0} đ{extra}";

        if (status == "Chờ thanh toán" && remain > 0 && _numPay.Value == 0)
            _numPay.Value = Math.Min(_numPay.Maximum, remain);
    }

    private void AddSelectedProduct()
    {
        _lblDraftError.Text = string.Empty;
        if (_gridProducts.CurrentRow is null)
        {
            _lblDraftError.Text = "Chọn một sản phẩm trong danh sách đang bán.";
            return;
        }

        var row = _gridProducts.CurrentRow;
        var maSp = Convert.ToInt32(row.Cells["MaSP"].Value);
        var name = Convert.ToString(row.Cells["TenSP"].Value) ?? "";
        var size = Convert.ToString(row.Cells["KichThuoc"].Value) ?? "";
        var price = Convert.ToDecimal(row.Cells["GiaDangBan"].Value);
        var discount = _numDiscount.Value;

        if (_draft.Rows.Cast<DataRow>().Any(x => Convert.ToInt32(x["MaSP"]) == maSp))
        {
            _lblDraftError.Text = "Sản phẩm này đã có trong đơn nháp.";
            return;
        }
        if (discount > price)
        {
            _lblDraftError.Text = "Giảm giá không được lớn hơn giá niêm yết.";
            return;
        }

        var newRow = _draft.NewRow();
        newRow["MaSP"] = maSp;
        newRow["TenSP"] = name;
        newRow["KichThuoc"] = size;
        newRow["GiaDangBan"] = price;
        newRow["GiamGia"] = discount;
        _draft.Rows.Add(newRow);
        _numDiscount.Value = 0;
        UpdateTotal();
    }

    private void RemoveDraftLine()
    {
        if (_gridDraft.CurrentRow?.DataBoundItem is DataRowView view)
        {
            _draft.Rows.Remove(view.Row);
            UpdateTotal();
        }
    }

    private void UpdateTotal()
    {
        var total = _draft.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["ThanhTien"]));
        _lblTotal.Text = $"Tổng cộng: {total:#,##0} đ";
    }

    private void CreateOrderFromUi()
    {
        _lblDraftError.Text = string.Empty;
        if (_draft.Rows.Count == 0)
        {
            _lblDraftError.Text = "Đơn hàng phải có ít nhất một sản phẩm.";
            return;
        }
        if (_cboCustomer.SelectedValue is null || _cboEmployee.SelectedValue is null)
        {
            _lblDraftError.Text = "Vui lòng chọn khách hàng và nhân viên.";
            return;
        }

        try
        {
            var lines = Db.NewOrderLineTable();
            foreach (DataRow src in _draft.Rows)
            {
                var dst = lines.NewRow();
                dst["MaSP"] = src["MaSP"];
                dst["GiamGia"] = src["GiamGia"];
                lines.Rows.Add(dst);
            }

            using var cn = Db.Open();
            using var cmd = new SqlCommand("dbo.sp_TaoDonHang", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@MaKH", Convert.ToInt32(_cboCustomer.SelectedValue));
            cmd.Parameters.AddWithValue("@MaNV", Convert.ToInt32(_cboEmployee.SelectedValue));
            cmd.Parameters.AddWithValue("@NgayLap", _dtpOrder.Value.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute));
            cmd.Parameters.AddWithValue("@HanThanhToan", _dtpDue.Value.Date.AddHours(17));
            cmd.Parameters.AddWithValue("@GhiChu", _txtNote.Text.Trim().Length == 0 ? DBNull.Value : _txtNote.Text.Trim());
            cmd.Parameters.Add(Db.Structured("@ChiTiet", "dbo.ChiTietDonHangType", lines));
            var output = new SqlParameter("@MaDH", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(output);
            cmd.ExecuteNonQuery();

            var maDh = Convert.ToInt32(output.Value);
            MessageBox.Show($"Đã tạo đơn #{maDh} và giữ {_draft.Rows.Count} sản phẩm.\nĐơn đang ở trạng thái Chờ thanh toán.",
                "Tạo đơn thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _draft.Rows.Clear();
            _txtNote.Clear();
            UpdateTotal();
            LoadProducts();
            _selected = null;
            _numPay.Value = 0;
            LoadOrders(maDh);
        }
        catch (Exception ex)
        {
            _lblDraftError.Text = Db.ErrorMessage(ex);
        }
    }

    private void RecordPayment()
    {
        if (_selected is null)
        {
            SetOrderMessage("Chọn một đơn hàng trước khi thu tiền.", true);
            return;
        }
        if (_numPay.Value <= 0)
        {
            SetOrderMessage("Số tiền thu phải lớn hơn 0.", true);
            return;
        }

        try
        {
            using var cn = Db.Open();
            using var cmd = new SqlCommand("dbo.sp_GhiNhanThanhToan", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@MaDH", _selected.Value.MaDH);
            cmd.Parameters.AddWithValue("@SoTien", _numPay.Value);
            cmd.Parameters.AddWithValue("@PhuongThuc", Convert.ToString(_cboMethod.SelectedItem));
            cmd.Parameters.AddWithValue("@TrangThai", Convert.ToString(_cboPayStatus.SelectedItem));
            cmd.Parameters.AddWithValue("@MaGiaoDich", _txtTxn.Text.Trim().Length == 0 ? DBNull.Value : _txtTxn.Text.Trim());
            cmd.Parameters.AddWithValue("@GhiChu", _txtPayNote.Text.Trim().Length == 0 ? DBNull.Value : _txtPayNote.Text.Trim());
            var output = new SqlParameter("@MaTTDH", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(output);
            cmd.ExecuteNonQuery();

            var result = Convert.ToString(_cboPayStatus.SelectedItem);
            MessageBox.Show(result == "Thành công"
                    ? "Đã ghi nhận thanh toán. Nếu tổng tiền đã đủ, đơn chuyển sang Đang xử lý và sản phẩm chuyển Đã bán."
                    : "Đã ghi nhận giao dịch ở trạng thái " + result + ". Đơn hàng chưa thay đổi trạng thái.",
                "Ghi nhận thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _numPay.Value = 0;
            _txtTxn.Clear();
            _txtPayNote.Clear();
            LoadOrders(_selected.Value.MaDH);
        }
        catch (Exception ex)
        {
            SetOrderMessage(Db.ErrorMessage(ex), true);
        }
    }

    private void CompleteOrder()
    {
        if (_selected is null)
        {
            SetOrderMessage("Chọn một đơn hàng trước khi hoàn tất.", true);
            return;
        }
        try
        {
            ExecSpDb("dbo.sp_HoanTatDonHang", ("@MaDH", _selected.Value.MaDH));
            MessageBox.Show($"Đơn #{_selected.Value.MaDH} đã hoàn tất. Sản phẩm đủ điều kiện quyết toán cho người ký gửi.",
                "Hoàn tất đơn", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadOrders(_selected.Value.MaDH);
        }
        catch (Exception ex)
        {
            SetOrderMessage(Db.ErrorMessage(ex), true);
        }
    }

    private void CancelOrder()
    {
        if (_selected is null)
        {
            SetOrderMessage("Chọn một đơn hàng trước khi hủy.", true);
            return;
        }
        var reason = _txtCancelReason.Text.Trim();
        if (reason.Length == 0)
        {
            SetOrderMessage("Phải nhập lý do hủy đơn.", true);
            _txtCancelReason.Focus();
            return;
        }
        try
        {
            ExecSpDb("dbo.sp_HuyDonHang", ("@MaDH", _selected.Value.MaDH), ("@LyDo", reason));
            MessageBox.Show($"Đơn #{_selected.Value.MaDH} đã hủy. Sản phẩm được đưa trở lại trạng thái Đang bán.",
                "Hủy đơn", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _txtCancelReason.Clear();
            LoadProducts();
            LoadOrders(_selected.Value.MaDH);
        }
        catch (Exception ex)
        {
            SetOrderMessage(Db.ErrorMessage(ex), true);
        }
    }

    private void SetOrderMessage(string message, bool error)
    {
        _lblOrderInfo.Text = message;
        _lblOrderInfo.ForeColor = error ? Theme.Danger : Theme.Muted;
    }

    private static void ExecSpDb(string name, params (string, object?)[] args)
    {
        using var cn = Db.Open();
        using var cmd = new SqlCommand(name, cn) { CommandType = CommandType.StoredProcedure };
        foreach (var (key, value) in args) cmd.Parameters.Add(Db.Param(key, value));
        if (name.EndsWith("sp_HoanTatDonHang", StringComparison.Ordinal)) cmd.CommandTimeout = 30;
        cmd.ExecuteNonQuery();
    }

    public void PrepareCapture()
    {
        if (_cboCustomer.Items.Count > 0) _cboCustomer.SelectedIndex = 0;
        if (_cboEmployee.Items.Count > 0) _cboEmployee.SelectedIndex = 0;
        _txtNote.Text = "Khách chọn 2 món, hẹn nhận hàng trong ngày.";
        if (_gridProducts.Rows.Count > 0)
        {
            _numDiscount.Value = 50000;
            _gridProducts.CurrentCell = _gridProducts.Rows[0].Cells["TenSP"];
            AddSelectedProduct();
            if (_gridProducts.Rows.Count > 1)
            {
                _numDiscount.Value = 0;
                _gridProducts.CurrentCell = _gridProducts.Rows[1].Cells["TenSP"];
                AddSelectedProduct();
            }
        }

        // Chọn đơn đã thu một phần để minh họa bảng thanh toán.
        foreach (DataGridViewRow row in _gridOrders.Rows)
        {
            if (Convert.ToString(row.Cells["TrangThai"].Value) == "Chờ thanh toán" &&
                Convert.ToDecimal(row.Cells["DaThu"].Value) > 0)
            {
                _gridOrders.CurrentCell = row.Cells["MaDH"];
                row.Selected = true;
                break;
            }
        }
        ShowSelectedOrder();
    }
}
