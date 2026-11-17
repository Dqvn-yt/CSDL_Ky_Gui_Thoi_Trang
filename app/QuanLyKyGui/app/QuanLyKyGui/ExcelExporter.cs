using System.Data;
using ClosedXML.Excel;

namespace QuanLyKyGui;

/// <summary>Xuất bảng dữ liệu ra Excel có tiêu đề, tổng hợp và khối chữ ký.</summary>
internal static class ExcelExporter
{
    public const string ExportFolder = @"E:\Quanap\exports";

    public static string Export(
        DataTable table,
        string title,
        string period,
        string filePrefix,
        string preparedBy,
        string receivedBy,
        IEnumerable<(string Label, string Value)> summary,
        IEnumerable<string> moneyColumns)
    {
        Directory.CreateDirectory(ExportFolder);
        var path = Path.Combine(ExportFolder, $"{filePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Bao cao");
        ws.Style.Font.FontName = "Times New Roman";
        ws.Style.Font.FontSize = 11;

        var cols = Math.Max(table.Columns.Count, 6);
        var row = 1;

        ws.Cell(row, 1).Value = "SÀN KÝ GỬI ĐỒ THỜI TRANG THIÊN KIM";
        ws.Range(row, 1, row, cols).Merge();
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Font.FontSize = 12;
        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        row += 1;

        ws.Cell(row, 1).Value = "Địa chỉ: Hà Nội  |  Hotline/Zalo: 0987 654 321";
        ws.Range(row, 1, row, cols).Merge();
        ws.Cell(row, 1).Style.Font.FontSize = 9;
        ws.Cell(row, 1).Style.Font.Italic = true;
        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        row += 2;

        ws.Cell(row, 1).Value = title.ToUpperInvariant();
        ws.Range(row, 1, row, cols).Merge();
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Font.FontSize = 15;
        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        row += 1;

        ws.Cell(row, 1).Value = period;
        ws.Range(row, 1, row, cols).Merge();
        ws.Cell(row, 1).Style.Font.Italic = true;
        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        row += 2;

        foreach (var (label, value) in summary)
        {
            ws.Cell(row, 1).Value = label;
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Range(row, 1, row, Math.Max(1, cols / 2)).Merge();
            ws.Cell(row, cols / 2 + 1).Value = value;
            ws.Range(row, cols / 2 + 1, row, cols).Merge();
            ws.Cell(row, cols / 2 + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            row += 1;
        }

        if (summary.Any()) row += 1;

        var headerRow = row;
        for (var c = 0; c < table.Columns.Count; c++)
        {
            var cell = ws.Cell(headerRow, c + 1);
            cell.Value = table.Columns[c].Caption;
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#7C1F35");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
        }
        ws.Row(headerRow).Height = 30;

        var moneySet = new HashSet<string>(moneyColumns, StringComparer.OrdinalIgnoreCase);
        var r = headerRow + 1;
        foreach (DataRow dataRow in table.Rows)
        {
            for (var c = 0; c < table.Columns.Count; c++)
            {
                var value = dataRow[c];
                var cell = ws.Cell(r, c + 1);
                if (value is null || value == DBNull.Value)
                {
                    cell.Value = string.Empty;
                }
                else if (value is DateTime dt)
                {
                    cell.Value = dt;
                    cell.Style.DateFormat.Format = "dd/MM/yyyy";
                }
                else if (value is decimal or double or float or long or int)
                {
                    cell.Value = Convert.ToDecimal(value);
                    if (moneySet.Contains(table.Columns[c].ColumnName))
                        cell.Style.NumberFormat.Format = "#,##0";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                else
                {
                    cell.Value = value.ToString();
                }
            }
            r++;
        }

        if (table.Rows.Count > 0)
        {
            var range = ws.Range(headerRow, 1, r - 1, table.Columns.Count);
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
        }

        r += 1;
        ws.Cell(r, 1).Value = $"Ngày lập báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm}";
        ws.Range(r, 1, r, cols).Merge();
        ws.Cell(r, 1).Style.Font.Italic = true;
        r += 2;

        var half = Math.Max(1, cols / 2);
        ws.Cell(r, 1).Value = "Người lập báo cáo";
        ws.Range(r, 1, r, half).Merge();
        ws.Cell(r, half + 1).Value = "Người nhận báo cáo";
        ws.Range(r, half + 1, r, cols).Merge();
        ws.Range(r, 1, r, cols).Style.Font.Bold = true;
        ws.Range(r, 1, r, cols).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        r += 1;
        ws.Cell(r, 1).Value = "(Ký, ghi rõ họ tên)";
        ws.Range(r, 1, r, half).Merge();
        ws.Cell(r, half + 1).Value = "(Ký, ghi rõ họ tên)";
        ws.Range(r, half + 1, r, cols).Merge();
        ws.Range(r, 1, r, cols).Style.Font.Italic = true;
        ws.Range(r, 1, r, cols).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        r += 4;
        ws.Cell(r, 1).Value = preparedBy;
        ws.Range(r, 1, r, half).Merge();
        ws.Cell(r, half + 1).Value = receivedBy;
        ws.Range(r, half + 1, r, cols).Merge();
        ws.Range(r, 1, r, cols).Style.Font.Bold = true;
        ws.Range(r, 1, r, cols).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.Columns().AdjustToContents();
        foreach (var column in ws.Columns())
        {
            if (column.Width > 42) column.Width = 42;
            if (column.Width < 11) column.Width = 11;
        }

        ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        ws.PageSetup.Margins.Top = 0.5;
        ws.PageSetup.Margins.Bottom = 0.5;
        ws.PageSetup.Footer.Center.AddText("Trang &P / &N");
        ws.SheetView.FreezeRows(headerRow);

        wb.SaveAs(path);
        return path;
    }
}
