using System.Data;

namespace QuanLyKyGui;

/// <summary>Danh tính quản lý cho bản demo; không thay thế cơ chế đăng nhập.</summary>
internal static class DemoSession
{
    private static int? _managerId;

    public static int ManagerId
    {
        get
        {
            if (_managerId is int id) return id;
            var value = Db.Scalar("""
                SELECT TOP (1) MaNV FROM dbo.NHAN_VIEN
                WHERE ChucVu = N'Quản lý' AND TrangThai = N'Đang làm việc'
                ORDER BY MaNV;
                """);
            if (value is null)
                throw new InvalidOperationException("Cần có ít nhất một quản lý đang làm việc để mở phiên demo quản lý.");
            return (_managerId = Convert.ToInt32(value)).Value;
        }
    }

    public static string ManagerName => Convert.ToString(Db.Scalar(
        "SELECT HoTen FROM dbo.NHAN_VIEN WHERE MaNV = @Id", Db.Param("@Id", ManagerId))) ?? "";

    public static void SelectManager(ComboBox combo)
    {
        if (combo.DataSource is DataTable table && table.Rows.Cast<DataRow>()
            .Any(row => Convert.ToInt32(row["MaNV"]) == ManagerId))
            combo.SelectedValue = ManagerId;
    }
}
