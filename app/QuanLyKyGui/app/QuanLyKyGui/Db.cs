using System.Data;
using Microsoft.Data.SqlClient;

namespace QuanLyKyGui;

/// <summary>Lớp truy cập dữ liệu dùng chung cho toàn bộ ứng dụng.</summary>
internal static class Db
{
    public const string ServerName = "localhost";
    public const string DatabaseName = "QuanLyKyGuiThoiTrang";

    public const string ConnectionString =
        "Server=localhost;Database=QuanLyKyGuiThoiTrang;Integrated Security=true;" +
        "TrustServerCertificate=true;Encrypt=false;Application Name=QuanLyKyGuiThoiTrang;";

    public static SqlConnection Open()
    {
        var cn = new SqlConnection(ConnectionString);
        cn.Open();
        return cn;
    }

    public static DataTable Query(string sql, params SqlParameter[] ps)
    {
        var table = new DataTable();
        using var cn = Open();
        using var cmd = new SqlCommand(sql, cn);
        if (ps.Length > 0) cmd.Parameters.AddRange(ps);
        using var da = new SqlDataAdapter(cmd);
        da.Fill(table);
        return table;
    }

    public static DataTable Procedure(string name, params SqlParameter[] ps)
    {
        var table = new DataTable();
        using var cn = Open();
        using var cmd = new SqlCommand(name, cn) { CommandType = CommandType.StoredProcedure };
        if (ps.Length > 0) cmd.Parameters.AddRange(ps);
        using var da = new SqlDataAdapter(cmd);
        da.Fill(table);
        return table;
    }

    public static int Execute(string sql, params SqlParameter[] ps)
    {
        using var cn = Open();
        using var cmd = new SqlCommand(sql, cn);
        if (ps.Length > 0) cmd.Parameters.AddRange(ps);
        return cmd.ExecuteNonQuery();
    }

    public static object? Scalar(string sql, params SqlParameter[] ps)
    {
        using var cn = Open();
        using var cmd = new SqlCommand(sql, cn);
        if (ps.Length > 0) cmd.Parameters.AddRange(ps);
        var value = cmd.ExecuteScalar();
        return value is DBNull ? null : value;
    }

    public static long Long(string sql, params SqlParameter[] ps)
    {
        var v = Scalar(sql, ps);
        return v is null ? 0 : Convert.ToInt64(v);
    }

    public static decimal Money(string sql, params SqlParameter[] ps)
    {
        var v = Scalar(sql, ps);
        return v is null ? 0m : Convert.ToDecimal(v);
    }

    public static SqlParameter Param(string name, object? value)
    {
        return new SqlParameter(name, value ?? DBNull.Value);
    }

    public static SqlParameter Structured(string name, string typeName, DataTable value)
    {
        return new SqlParameter(name, SqlDbType.Structured)
        {
            TypeName = typeName,
            Value = value
        };
    }

    public static DataTable NewProductTable()
    {
        var t = new DataTable();
        t.Columns.Add("MaDM", typeof(int));
        t.Columns.Add("MaTH", typeof(int));
        t.Columns.Add("TenSP", typeof(string));
        t.Columns.Add("MoTa", typeof(string));
        t.Columns.Add("KichThuoc", typeof(string));
        t.Columns.Add("MauSac", typeof(string));
        t.Columns.Add("ChatLieu", typeof(string));
        t.Columns.Add("TinhTrang", typeof(string));
        t.Columns.Add("GiaDeXuat", typeof(decimal));
        t.Columns.Add("GiaDangBan", typeof(decimal));
        t.Columns.Add("TrangThaiTiepNhan", typeof(string));
        return t;
    }

    public static DataTable NewOrderLineTable()
    {
        var t = new DataTable();
        t.Columns.Add("MaSP", typeof(int));
        t.Columns.Add("GiamGia", typeof(decimal));
        return t;
    }

    public static DataTable NewSettlementLineTable()
    {
        var t = new DataTable();
        t.Columns.Add("MaCTDH", typeof(int));
        return t;
    }

    public static string ErrorMessage(Exception ex)
    {
        if (ex is SqlException sql)
        {
            var msg = sql.Message;
            // Bỏ tiền tố "Thao tác ..." nếu có, giữ thông điệp nghiệp vụ tiếng Việt.
            var idx = msg.IndexOf(": ", StringComparison.Ordinal);
            if (msg.StartsWith("Thao tác", StringComparison.Ordinal) && idx > 0)
                msg = msg[(idx + 2)..];
            return msg.Trim();
        }
        return ex.Message;
    }
}
