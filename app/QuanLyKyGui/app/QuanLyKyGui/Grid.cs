using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace QuanLyKyGui;

internal static class Grid
{
	public static void Bind(DataGridView g, DataTable table, IDictionary<string, string>? headers = null, params string[] hidden)
	{
		g.DataSource = table;
		if (headers != null)
		{
			foreach (KeyValuePair<string, string> header in headers)
			{
				if (g.Columns.Contains(header.Key))
				{
					g.Columns[header.Key].HeaderText = header.Value;
				}
			}
		}
		foreach (string columnName in hidden)
		{
			if (g.Columns.Contains(columnName))
			{
				g.Columns[columnName].Visible = false;
			}
		}
	}
}
