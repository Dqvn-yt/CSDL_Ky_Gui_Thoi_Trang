namespace QuanLyKyGui;

internal sealed class frmNhanVienMoi : Form
{
    private readonly TextBox _name = Theme.TextBox(null, 430);
    private readonly TextBox _phone = Theme.TextBox(null, 430);
    private readonly TextBox _email = Theme.TextBox(null, 430);
    private readonly ComboBox _role = Theme.Combo(430);
    private readonly Label _error = new() { Dock = DockStyle.Fill, ForeColor = Theme.Danger, Font = Theme.Small };

    public frmNhanVienMoi()
    {
        Text = "Thêm nhân viên — Thiên Kim Consignment";
        ClientSize = new Size(500, 460);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Theme.Canvas;
        Font = Theme.Body;
        _name.MaxLength = 100;
        _phone.MaxLength = 20;
        _email.MaxLength = 254;
        _role.Items.AddRange(new object[] { "Nhân viên", "Thu ngân", "Quản lý" });
        _role.SelectedIndex = 0;

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24), ColumnCount = 1, RowCount = 7 };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        for (var i = 0; i < 4; i++) root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        root.Controls.Add(new Label { Text = "Thêm nhân viên", AutoSize = true, Font = Theme.H2, ForeColor = Theme.Ink }, 0, 0);
        root.Controls.Add(Fields.Field("Họ tên *", _name), 0, 1);
        root.Controls.Add(Fields.Field("Số điện thoại (không bắt buộc)", _phone), 0, 2);
        root.Controls.Add(Fields.Field("Email (không bắt buộc)", _email), 0, 3);
        root.Controls.Add(Fields.Field("Chức vụ *", _role), 0, 4);
        root.Controls.Add(_error, 0, 5);
        var cancel = Theme.GhostButton("Hủy", 100);
        cancel.DialogResult = DialogResult.Cancel;
        var save = Theme.PrimaryButton("Lưu nhân viên", 170);
        save.Click += (_, _) =>
        {
            save.Enabled = false;
            try
            {
                EmployeeService.Add(_name.Text, _phone.Text, _email.Text, Convert.ToString(_role.SelectedItem) ?? "");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { _error.Text = Db.ErrorMessage(ex); }
            finally { save.Enabled = true; }
        };
        var actions = Theme.Actions(cancel, save);
        actions.Dock = DockStyle.Right;
        root.Controls.Add(actions, 0, 6);
        Controls.Add(root);
        AcceptButton = save;
        CancelButton = cancel;
        Shown += (_, _) => _name.Focus();
    }
}
