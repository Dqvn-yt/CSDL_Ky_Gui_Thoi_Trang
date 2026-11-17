using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace QuanLyKyGui;

internal sealed class frmTiepNhanKyGui : Form
{
	private readonly ComboBox _cboCustomer = Theme.Combo(340);

	private readonly ComboBox _cboEmployee = Theme.Combo(200);

	private readonly DateTimePicker _dtpDate = Theme.DatePicker(150);

	private readonly TextBox _txtPhoneSearch = Theme.TextBox(null, 190);

	private readonly TextBox _txtNote = new TextBox
	{
		Multiline = true,
		Height = 50,
		Width = 340,
		Font = Theme.Body,
		BorderStyle = BorderStyle.FixedSingle
	};

	private readonly Label _lblContact = new Label
	{
		AutoSize = false,
		Width = 368,
		Height = 36,
		Font = Theme.Small,
		ForeColor = Theme.Muted,
		AutoEllipsis = false
	};

	private readonly Label _lblSearchResult = new Label
	{
		AutoSize = true,
		Font = Theme.Small,
		ForeColor = Theme.Muted
	};

	private readonly ComboBox _cboCategory = Theme.Combo(140);

	private readonly ComboBox _cboBrand = Theme.Combo(190);

	private readonly TextBox _txtName = Theme.TextBox(null, 200);

	private readonly TextBox _txtSize = Theme.TextBox(null, 80);

	private readonly TextBox _txtColor = Theme.TextBox(null, 80);

	private readonly TextBox _txtMaterial = Theme.TextBox(null, 90);

	private readonly TextBox _txtCondition = Theme.TextBox("Đã qua sử dụng, còn tốt 90%", 180);

	private readonly NumericUpDown _numProposed = Theme.Number(500000000m, 120);

	private readonly NumericUpDown _numListed = Theme.Number(500000000m, 120);

	private readonly ComboBox _cboApproval = Theme.Combo(140);

	private readonly DataGridView _grid = new DataGridView();

	private readonly DataTable _draft;

	private readonly Label _lblCount = new Label
	{
		AutoSize = true,
		Font = Theme.Bold,
		ForeColor = Theme.Ink
	};

	private readonly Label _lblFormError = new Label
	{
		Dock = DockStyle.Fill,
		Font = Theme.Small,
		ForeColor = Theme.Danger
	};

	public frmTiepNhanKyGui()
	{
		Text = "Tiếp nhận ký gửi — Thiên Kim Consignment";
		base.ClientSize = new Size(1360, 830);
		MinimumSize = new Size(1180, 740);
		base.StartPosition = FormStartPosition.CenterParent;
		BackColor = Theme.Canvas;
		Font = Theme.Body;
		_draft = BuildDraftTable();
		BuildUi();
		LoadLookups();
		base.Shown += delegate
		{
			_cboCustomer.Focus();
		};
	}

	private static DataTable BuildDraftTable()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("MaDM", typeof(int));
		dataTable.Columns.Add("MaTH", typeof(int));
		dataTable.Columns.Add("TenSP", typeof(string));
		dataTable.Columns.Add("TenDanhMuc", typeof(string));
		dataTable.Columns.Add("TenThuongHieu", typeof(string));
		dataTable.Columns.Add("KichThuoc", typeof(string));
		dataTable.Columns.Add("MauSac", typeof(string));
		dataTable.Columns.Add("ChatLieu", typeof(string));
		dataTable.Columns.Add("TinhTrang", typeof(string));
		dataTable.Columns.Add("GiaDeXuat", typeof(decimal));
		dataTable.Columns.Add("GiaDangBan", typeof(decimal));
		dataTable.Columns.Add("TrangThaiTiepNhan", typeof(string));
		return dataTable;
	}

	private void BuildUi()
	{
		TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
		{
			Dock = DockStyle.Fill,
			ColumnCount = 1,
			RowCount = 2,
			Padding = new Padding(20, 14, 20, 14),
			BackColor = Theme.Canvas
		};
		tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
		tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 72f));
		tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
		tableLayoutPanel.Controls.Add(BuildTitle(), 0, 0);
		TableLayoutPanel tableLayoutPanel2 = new TableLayoutPanel
		{
			Dock = DockStyle.Fill,
			ColumnCount = 2,
			RowCount = 1,
			Margin = new Padding(0)
		};
		tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400f));
		tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
		tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
		TableLayoutPanel tableLayoutPanel3 = new TableLayoutPanel
		{
			Dock = DockStyle.Fill,
			ColumnCount = 1,
			RowCount = 2,
			Margin = new Padding(0, 0, 14, 0)
		};
		tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
		tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 388f));
		tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
		tableLayoutPanel3.Controls.Add(BuildCustomerCard(), 0, 0);
		tableLayoutPanel3.Controls.Add(BuildGuideCard(), 0, 1);
		tableLayoutPanel2.Controls.Add(tableLayoutPanel3, 0, 0);
		tableLayoutPanel2.Controls.Add(BuildProductCard(), 1, 0);
		tableLayoutPanel.Controls.Add(tableLayoutPanel2, 0, 1);
		base.Controls.Add(tableLayoutPanel);
		SyncDraftColumns();
	}

	private Control BuildTitle()
	{
		TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
		{
			Dock = DockStyle.Fill,
			ColumnCount = 2,
			RowCount = 1,
			Margin = new Padding(0)
		};
		tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
		tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
		Panel panel = new Panel
		{
			Dock = DockStyle.Fill,
			BackColor = Color.Transparent
		};
		panel.Controls.Add(new Label
		{
			Text = "Tiếp nhận ký gửi",
			Font = Theme.H1,
			ForeColor = Theme.Ink,
			AutoSize = true,
			Location = new Point(0, 0)
		});
		panel.Controls.Add(new Label
		{
			Text = "Ghi nhận người ký gửi, kiểm tra sản phẩm và lưu phiếu — nghiệp vụ then chốt của sàn.",
			Font = Theme.Small,
			ForeColor = Theme.Muted,
			AutoSize = true,
			Location = new Point(2, 40)
		});
		tableLayoutPanel.Controls.Add(panel, 0, 0);
		Button button = Theme.GhostButton("Đóng", 100);
		button.Click += delegate
		{
			Close();
		};
		Button button2 = Theme.PrimaryButton("Lưu phiếu ký gửi", 190);
		button2.Click += delegate
		{
			SaveSlip();
		};
		Panel control = Theme.Actions(button, button2);
		tableLayoutPanel.Controls.Add(control, 1, 0);
		return tableLayoutPanel;
	}

	private Control BuildCustomerCard()
	{
		Card card = new Card("1. Người ký gửi & thông tin phiếu");
		card.Dock = DockStyle.Fill;
		card.Margin = new Padding(0, 0, 0, 12);
		TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
		{
			Dock = DockStyle.Fill,
			ColumnCount = 1,
			RowCount = 6,
			Margin = new Padding(0)
		};
		for (int i = 0; i < 6; i++)
		{
			tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
		}
		FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
		{
			AutoSize = true,
			WrapContents = false,
			Margin = new Padding(0)
		};
		_txtPhoneSearch.Width = 150;
		flowLayoutPanel.Controls.Add(Fields.Field("Tìm theo SĐT / tên", _txtPhoneSearch));
		Button button = Theme.GhostButton("Tìm", 62);
		button.Margin = new Padding(0, 22, 6, 0);
		button.Click += delegate
		{
			SearchCustomer();
		};
		flowLayoutPanel.Controls.Add(button);
		Button button2 = Theme.GhostButton("+ Khách mới", 126);
		button2.Margin = new Padding(0, 22, 0, 0);
		button2.Click += delegate
		{
			AddCustomer(_txtPhoneSearch.Text.Trim());
		};
		flowLayoutPanel.Controls.Add(button2);
		tableLayoutPanel.Controls.Add(flowLayoutPanel, 0, 0);
		tableLayoutPanel.Controls.Add(_lblSearchResult, 0, 1);
		tableLayoutPanel.Controls.Add(Fields.Field("Khách hàng", _cboCustomer), 0, 2);
		_cboCustomer.SelectedIndexChanged += delegate
		{
			UpdateCustomerInfo();
		};
		_lblContact.Margin = new Padding(0, 0, 0, 4);
		tableLayoutPanel.Controls.Add(_lblContact, 0, 3);
		TableLayoutPanel tableLayoutPanel2 = new TableLayoutPanel
		{
			AutoSize = true,
			ColumnCount = 2,
			RowCount = 1,
			Margin = new Padding(0)
		};
		tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 212f));
		tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 152f));
		tableLayoutPanel2.Controls.Add(Fields.Field("Nhân viên tiếp nhận", _cboEmployee), 0, 0);
		tableLayoutPanel2.Controls.Add(Fields.Field("Ngày ký gửi", _dtpDate), 1, 0);
		tableLayoutPanel.Controls.Add(tableLayoutPanel2, 0, 4);
		Panel panel = new Panel
		{
			Height = 76,
			Width = 368,
			Margin = new Padding(0)
		};
		panel.Controls.Add(new Label
		{
			Text = "Ghi chú",
			AutoSize = true,
			Font = Theme.Small,
			ForeColor = Theme.Muted,
			Location = new Point(0, 0)
		});
		_txtNote.Location = new Point(0, 20);
		panel.Controls.Add(_txtNote);
		tableLayoutPanel.Controls.Add(panel, 0, 5);
		card.Body.Controls.Add(tableLayoutPanel);
		return card;
	}

	private Control BuildGuideCard()
	{
		Card card = new Card("Quy tắc nghiệp vụ áp dụng");
		card.Dock = DockStyle.Fill;
		card.Margin = new Padding(0);
		Label value = new Label
		{
			Dock = DockStyle.Fill,
			Font = Theme.Small,
			ForeColor = Theme.Muted,
			Text = "• Phiếu phải có ít nhất 01 sản phẩm.\n• Sản phẩm chờ duyệt chưa có giá bán; sản phẩm đã chấp nhận bắt buộc có giá đăng bán lớn hơn 0.\n• Hoa hồng sàn cố định 20% trên giá bán thực tế.\n• Không trả lại sản phẩm đã bán hoặc đang giữ trong đơn; mỗi sản phẩm chỉ quyết toán một lần."
		};
		card.Body.Controls.Add(value);
		return card;
	}

	private Control BuildProductCard()
	{
		Card card = new Card("2. Sản phẩm ký gửi trong phiếu");
		card.Dock = DockStyle.Fill;
		card.Margin = new Padding(0);
		TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
		{
			Dock = DockStyle.Fill,
			ColumnCount = 1,
			RowCount = 4,
			Margin = new Padding(0)
		};
		tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
		tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 186f));
		tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
		tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38f));
		tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50f));
		FlowLayoutPanel flowLayoutPanel = Fields.Flow(860, 186);
		flowLayoutPanel.Dock = DockStyle.Fill;
		flowLayoutPanel.Controls.Add(Fields.Field("Danh mục", _cboCategory));
		flowLayoutPanel.Controls.Add(Fields.Field("Thương hiệu", _cboBrand));
		flowLayoutPanel.Controls.Add(Fields.Field("Tên sản phẩm", _txtName));
		flowLayoutPanel.Controls.Add(Fields.Field("Kích thước", _txtSize));
		flowLayoutPanel.Controls.Add(Fields.Field("Màu sắc", _txtColor));
		flowLayoutPanel.Controls.Add(Fields.Field("Chất liệu", _txtMaterial));
		flowLayoutPanel.Controls.Add(Fields.Field("Tình trạng", _txtCondition));
		flowLayoutPanel.Controls.Add(Fields.Field("Giá đề xuất", _numProposed));
		flowLayoutPanel.Controls.Add(Fields.Field("Giá đăng bán", _numListed));
		flowLayoutPanel.Controls.Add(Fields.Field("Trạng thái duyệt", _cboApproval));
		_cboApproval.Items.AddRange("Chờ xử lý", "Đã chấp nhận");
		_cboApproval.SelectedIndexChanged += delegate
		{
			bool flag = Convert.ToString(_cboApproval.SelectedItem) == "Chờ xử lý";
			_numListed.Enabled = !flag;
			if (flag)
			{
				_numListed.Value = 0m;
			}
		};
		_cboApproval.SelectedIndex = 0;
		tableLayoutPanel.Controls.Add(flowLayoutPanel, 0, 0);
		Theme.StyleGrid(_grid);
		_grid.Dock = DockStyle.Fill;
		_grid.Margin = new Padding(0, 4, 0, 8);
		tableLayoutPanel.Controls.Add(_grid, 0, 1);
		TableLayoutPanel tableLayoutPanel2 = new TableLayoutPanel
		{
			Dock = DockStyle.Fill,
			ColumnCount = 4,
			RowCount = 1,
			Margin = new Padding(0)
		};
		tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
		tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
		tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
		tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
		_lblCount.AutoSize = false;
		_lblCount.Dock = DockStyle.Fill;
		_lblCount.TextAlign = ContentAlignment.MiddleLeft;
		tableLayoutPanel2.Controls.Add(_lblCount, 0, 0);
		tableLayoutPanel.Controls.Add(_lblFormError, 0, 2);
		Button button = Theme.GoldButton("+  Thêm sản phẩm");
		button.Click += delegate
		{
			TryAddFromInputs();
		};
		tableLayoutPanel2.Controls.Add(button, 1, 0);
		Button button2 = Theme.GhostButton("Xóa dòng", 100);
		button2.Click += delegate
		{
			RemoveSelected();
		};
		tableLayoutPanel2.Controls.Add(button2, 2, 0);
		Button button3 = Theme.PrimaryButton("Lưu phiếu ký gửi", 180);
		button3.Click += delegate
		{
			SaveSlip();
		};
		tableLayoutPanel2.Controls.Add(button3, 3, 0);
		tableLayoutPanel.Controls.Add(tableLayoutPanel2, 0, 3);
		card.Body.Controls.Add(tableLayoutPanel);
		return card;
	}

	private void LoadCustomers(int? selectId = null)
	{
		DataTable dataTable = Db.Query("SELECT MaKH, HoTen, SoDienThoai, DiaChi FROM dbo.KHACH_HANG ORDER BY HoTen");
		DataTable dataTable2 = dataTable.Copy();
		dataTable2.Columns.Add("HienThi", typeof(string));
		foreach (DataRow row in dataTable2.Rows)
		{
			row["HienThi"] = $"{row["HoTen"]} — {row["SoDienThoai"]}";
		}
		_cboCustomer.DataSource = dataTable2;
		_cboCustomer.DisplayMember = "HienThi";
		_cboCustomer.ValueMember = "MaKH";
		if (selectId.HasValue)
		{
			int valueOrDefault = selectId.GetValueOrDefault();
			if (true)
			{
				for (int i = 0; i < dataTable2.Rows.Count; i++)
				{
					if (Convert.ToInt32(dataTable2.Rows[i]["MaKH"]) == valueOrDefault)
					{
						_cboCustomer.SelectedIndex = i;
						break;
					}
				}
			}
		}
		UpdateCustomerInfo();
	}

	private void LoadLookups()
	{
		try
		{
			LoadCustomers();
			DataTable dataTable = Db.Query("SELECT MaNV, HoTen, ChucVu FROM dbo.NHAN_VIEN WHERE TrangThai = N'Đang làm việc' ORDER BY HoTen");
			DataTable dataTable2 = dataTable.Copy();
			dataTable2.Columns.Add("HienThi", typeof(string));
			foreach (DataRow row in dataTable2.Rows)
			{
				row["HienThi"] = $"{row["HoTen"]} ({row["ChucVu"]})";
			}
			_cboEmployee.DataSource = dataTable2;
			_cboEmployee.DisplayMember = "HienThi";
			_cboEmployee.ValueMember = "MaNV";
			_cboCategory.DataSource = Db.Query("SELECT MaDM, TenDanhMuc FROM dbo.DANH_MUC ORDER BY TenDanhMuc");
			_cboCategory.DisplayMember = "TenDanhMuc";
			_cboCategory.ValueMember = "MaDM";
			DataTable dataTable3 = Db.Query("SELECT MaTH, TenThuongHieu FROM dbo.THUONG_HIEU ORDER BY TenThuongHieu");
			DataTable dataTable4 = dataTable3.Copy();
			DataRow dataRow2 = dataTable4.NewRow();
			dataRow2["MaTH"] = DBNull.Value;
			dataRow2["TenThuongHieu"] = "(Không có thương hiệu)";
			dataTable4.Rows.InsertAt(dataRow2, 0);
			_cboBrand.DataSource = dataTable4;
			_cboBrand.DisplayMember = "TenThuongHieu";
			_cboBrand.ValueMember = "MaTH";
			_dtpDate.Value = DateTime.Now;
			UpdateCustomerInfo();
		}
		catch (Exception ex)
		{
			MessageBox.Show("Không tải được danh mục: " + Db.ErrorMessage(ex), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void UpdateCustomerInfo()
	{
		if (_cboCustomer.SelectedItem is DataRowView dataRowView)
		{
			string text = Convert.ToString(dataRowView["SoDienThoai"]) ?? "—";
			string text2 = Convert.ToString(dataRowView["DiaChi"]);
			string text3 = ((text2 != null && text2.Length > 0) ? text2 : "(Chưa cập nhật địa chỉ)");
			_lblContact.Text = "Điện thoại: " + text + "\r\nĐ/c: " + text3;
		}
	}

	private void AddCustomer(string? keyword)
	{
		string prefillName = null;
		string prefillPhone = null;
		string text = keyword?.Trim();
		if (!string.IsNullOrEmpty(text))
		{
			if (text.All(delegate(char ch)
			{
				bool flag = char.IsDigit(ch);
				bool flag2 = flag;
				if (!flag2)
				{
					bool flag3;
					switch (ch)
					{
					case ' ':
					case '+':
					case '-':
					case '.':
						flag3 = true;
						break;
					default:
						flag3 = false;
						break;
					}
					flag2 = flag3;
				}
				return flag2;
			}))
			{
				prefillPhone = text;
			}
			else
			{
				prefillName = text;
			}
		}
		using frmKhachHangMoi frmKhachHangMoi2 = new frmKhachHangMoi(prefillName, prefillPhone);
		Form form = (base.TopLevel ? this : base.Parent?.FindForm());
		DialogResult dialogResult = ((form == null) ? frmKhachHangMoi2.ShowDialog() : frmKhachHangMoi2.ShowDialog(form));
		if (dialogResult == DialogResult.OK)
		{
			LoadCustomers(frmKhachHangMoi2.NewCustomerId);
			_txtPhoneSearch.Clear();
			_lblSearchResult.Text = "Đã thêm khách hàng mới: " + frmKhachHangMoi2.NewCustomerName + ". Phiếu sẽ dùng khách này.";
			_lblSearchResult.ForeColor = Theme.Success;
		}
	}

	private void SearchCustomer()
	{
		string text = _txtPhoneSearch.Text.Trim();
		if (text.Length == 0)
		{
			_lblSearchResult.Text = "Nhập số điện thoại hoặc tên khách hàng cần tìm.";
			_lblSearchResult.ForeColor = Theme.Muted;
			return;
		}
		DataTable dataTable = Db.Query("SELECT TOP 5 MaKH, HoTen, SoDienThoai FROM dbo.KHACH_HANG WHERE SoDienThoai LIKE @Key OR HoTen LIKE @Key ORDER BY HoTen", Db.Param("@Key", "%" + text + "%"));
		if (dataTable.Rows.Count == 0)
		{
			_lblSearchResult.Text = "Không tìm thấy khách hàng phù hợp.";
			_lblSearchResult.ForeColor = Theme.Danger;
			if (MessageBox.Show(this, "Không tìm thấy khách hàng phù hợp. Tạo khách hàng mới?", "Thêm khách hàng", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				AddCustomer(text);
			}
			return;
		}
		int num = Convert.ToInt32(dataTable.Rows[0]["MaKH"]);
		for (int i = 0; i < _cboCustomer.Items.Count; i++)
		{
			if (_cboCustomer.Items[i] is DataRowView dataRowView && Convert.ToInt32(dataRowView["MaKH"]) == num)
			{
				_cboCustomer.SelectedIndex = i;
				break;
			}
		}
		_lblSearchResult.Text = $"Tìm thấy {dataTable.Rows.Count} khách hàng — đã chọn {dataTable.Rows[0]["HoTen"]}.";
		_lblSearchResult.ForeColor = Theme.Success;
	}

	private void SyncDraftColumns()
	{
		Theme.StyleGrid(_grid);
		_grid.AutoGenerateColumns = false;
		_grid.Columns.Clear();
		_grid.Columns.Add(Theme.TextColumn("TenSP", "Tên sản phẩm", 190));
		_grid.Columns.Add(Theme.TextColumn("TenDanhMuc", "Danh mục", 75));
		_grid.Columns.Add(Theme.TextColumn("TenThuongHieu", "Thương hiệu", 80));
		_grid.Columns.Add(Theme.TextColumn("KichThuoc", "Kích thước", 66));
		_grid.Columns.Add(Theme.TextColumn("MauSac", "Màu sắc", 50));
		_grid.Columns.Add(Theme.TextColumn("TinhTrang", "Tình trạng", 110));
		_grid.Columns.Add(Theme.TextColumn("GiaDeXuat", "Giá đề xuất", 80, money: true));
		_grid.Columns.Add(Theme.TextColumn("GiaDangBan", "Giá đăng bán", 80, money: true));
		_grid.Columns.Add(Theme.TextColumn("TrangThaiTiepNhan", "Trạng thái duyệt", 110));
		_grid.DataSource = _draft;
		_lblCount.Text = "0 sản phẩm trong phiếu";
	}

	private bool TryAddFromInputs()
	{
		_lblFormError.Text = string.Empty;
		string text = _txtName.Text.Trim();
		string text2 = _txtCondition.Text.Trim();
		if (text.Length == 0)
		{
			_lblFormError.Text = "Tên sản phẩm không được để trống.";
			_txtName.Focus();
			return false;
		}
		if (text2.Length == 0)
		{
			_lblFormError.Text = "Tình trạng sản phẩm không được để trống.";
			_txtCondition.Focus();
			return false;
		}
		if (!(_cboCategory.SelectedValue is int maDm) || 1 == 0)
		{
			_lblFormError.Text = "Vui lòng chọn danh mục sản phẩm.";
			_cboCategory.Focus();
			return false;
		}
		string text3 = Convert.ToString(_cboApproval.SelectedItem) ?? "Chờ xử lý";
		if (text3 == "Đã chấp nhận" && _numListed.Value <= 0m)
		{
			_lblFormError.Text = "Sản phẩm đã chấp nhận phải có giá đăng bán lớn hơn 0.";
			_numListed.Focus();
			return false;
		}
		string text4 = _cboCategory.Text;
		object selectedValue = _cboBrand.SelectedValue;
		AddProductRow(text, maDm, text4, (selectedValue == null || selectedValue is DBNull) ? ((int?)null) : new int?(Convert.ToInt32(_cboBrand.SelectedValue)), _cboBrand.Text, _txtSize.Text.Trim(), _txtColor.Text.Trim(), _txtMaterial.Text.Trim(), text2, _numProposed.Value, (text3 == "Đã chấp nhận") ? new decimal?(_numListed.Value) : ((decimal?)null), text3);
		_txtName.Clear();
		_txtSize.Clear();
		_txtColor.Clear();
		_numProposed.Value = 0m;
		_numListed.Value = 0m;
		_txtName.Focus();
		return true;
	}

	private void AddProductRow(string name, int maDm, string category, int? maTh, string brand, string size, string color, string material, string condition, decimal proposed, decimal? listed, string approval)
	{
		DataRow dataRow = _draft.NewRow();
		dataRow["MaDM"] = maDm;
		dataRow["MaTH"] = ((!maTh.HasValue) ? DBNull.Value : ((object)maTh.Value));
		dataRow["TenSP"] = name;
		dataRow["TenDanhMuc"] = category;
		dataRow["TenThuongHieu"] = brand;
		dataRow["KichThuoc"] = size;
		dataRow["MauSac"] = color;
		dataRow["ChatLieu"] = material;
		dataRow["TinhTrang"] = condition;
		dataRow["GiaDeXuat"] = proposed;
		dataRow["GiaDangBan"] = ((!listed.HasValue) ? DBNull.Value : ((object)listed.Value));
		dataRow["TrangThaiTiepNhan"] = approval;
		_draft.Rows.Add(dataRow);
		_lblCount.Text = $"{_draft.Rows.Count} sản phẩm trong phiếu";
	}

	private void RemoveSelected()
	{
		if (_grid.CurrentRow?.DataBoundItem is DataRowView dataRowView)
		{
			_draft.Rows.Remove(dataRowView.Row);
			_lblCount.Text = $"{_draft.Rows.Count} sản phẩm trong phiếu";
		}
	}

	private void SaveSlip()
	{
		_lblFormError.Text = string.Empty;
		if (_cboCustomer.SelectedValue is int num)
		{
			object selectedValue = _cboEmployee.SelectedValue;
			int num2 = default(int);
			int num3;
			if (selectedValue is int)
			{
				num2 = (int)selectedValue;
				num3 = 1;
			}
			else
			{
				num3 = 0;
			}
			if (num3 != 0)
			{
				if (!string.IsNullOrWhiteSpace(_txtName.Text) && !TryAddFromInputs())
				{
					return;
				}
				if (_draft.Rows.Count == 0)
				{
					_lblFormError.Text = "Phiếu ký gửi phải có ít nhất một sản phẩm. Hãy nhập tên sản phẩm.";
					_txtName.Focus();
					return;
				}
				try
				{
					DataTable dataTable = Db.NewProductTable();
					foreach (DataRow row in _draft.Rows)
					{
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2["MaDM"] = row["MaDM"];
						dataRow2["MaTH"] = row["MaTH"];
						dataRow2["TenSP"] = row["TenSP"];
						dataRow2["MoTa"] = row["TenSP"];
						dataRow2["KichThuoc"] = row["KichThuoc"];
						dataRow2["MauSac"] = row["MauSac"];
						dataRow2["ChatLieu"] = row["ChatLieu"];
						dataRow2["TinhTrang"] = row["TinhTrang"];
						dataRow2["GiaDeXuat"] = row["GiaDeXuat"];
						dataRow2["GiaDangBan"] = row["GiaDangBan"];
						dataRow2["TrangThaiTiepNhan"] = row["TrangThaiTiepNhan"];
						dataTable.Rows.Add(dataRow2);
					}
					using SqlConnection connection = Db.Open();
					using SqlCommand sqlCommand = new SqlCommand("dbo.sp_TiepNhanPhieuKyGui", connection)
					{
						CommandType = CommandType.StoredProcedure
					};
					sqlCommand.Parameters.AddWithValue("@MaKH", num);
					sqlCommand.Parameters.AddWithValue("@MaNV", num2);
					sqlCommand.Parameters.AddWithValue("@NgayKyGui", _dtpDate.Value.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute));
					sqlCommand.Parameters.AddWithValue("@GhiChu", (_txtNote.Text.Trim().Length == 0) ? ((IConvertible)DBNull.Value) : ((IConvertible)_txtNote.Text.Trim()));
					sqlCommand.Parameters.Add(Db.Structured("@SanPham", "dbo.SanPhamKyGuiType", dataTable));
					SqlParameter sqlParameter = new SqlParameter("@MaPKG", SqlDbType.Int)
					{
						Direction = ParameterDirection.Output
					};
					sqlCommand.Parameters.Add(sqlParameter);
					sqlCommand.ExecuteNonQuery();
					int value = Convert.ToInt32(sqlParameter.Value);
					MessageBox.Show($"Đã lưu phiếu ký gửi #{value} với {_draft.Rows.Count} sản phẩm.\nSản phẩm chờ duyệt sẽ được kiểm định trước khi đăng bán.", "Lưu thành công", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					_draft.Rows.Clear();
					_lblCount.Text = "0 sản phẩm trong phiếu";
					_txtNote.Clear();
					_txtName.Focus();
					return;
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, Db.ErrorMessage(ex), "Không lưu được phiếu ký gửi", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
			}
		}
		_lblFormError.Text = "Vui lòng chọn khách hàng và nhân viên tiếp nhận.";
	}

	public void PrepareCapture()
	{
		if (_cboCustomer.Items.Count > 0)
		{
			_cboCustomer.SelectedIndex = 0;
		}
		if (_cboEmployee.Items.Count > 0)
		{
			_cboEmployee.SelectedIndex = 0;
		}
		_txtNote.Text = "Khách mang 2 món đến ký gửi, đã kiểm tra cùng khách và chụp ảnh sản phẩm.";
		AddProductRow("Đầm lụa Zara dáng suông", 1, "Quần áo", 3, "Zara", "M", "Đen", "Lụa pha", "Đã qua sử dụng, còn tốt 90%", 650000m, 590000m, "Đã chấp nhận");
		AddProductRow("Túi xách da Mango", 4, "Túi xách", 10, "Mango", "One size", "Nâu", "Da PU", "Còn mới 98%, chưa dùng", 480000m, 450000m, "Đã chấp nhận");
		AddProductRow("Mũ beret nỉ", 3, "Mũ nón", null, "(Không có thương hiệu)", "Free size", "Đen", "Nỉ", "Đã qua sử dụng, còn tốt 80%", 180000m, null, "Chờ xử lý");
		_lblFormError.Text = string.Empty;
	}
}
