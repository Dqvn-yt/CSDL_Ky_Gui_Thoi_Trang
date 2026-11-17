using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyKyGui;

internal sealed class BarChart : Panel
{
	public List<(string Label, decimal Value)> Data { get; } = new List<(string, decimal)>();

	public BarChart()
	{
		DoubleBuffered = true;
		BackColor = Theme.Surface;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		Graphics graphics = e.Graphics;
		graphics.SmoothingMode = SmoothingMode.AntiAlias;
		graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
		if (Data.Count == 0)
		{
			TextRenderer.DrawText(graphics, "Chưa có dữ liệu trong khoảng thời gian đã chọn.", Theme.Body, base.ClientRectangle, Theme.Muted, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
			return;
		}
		int num = Math.Max(40, base.Width - 64 - 16);
		int num2 = Math.Max(40, base.Height - 18 - 42);
		decimal num3 = Data.Max<(string, decimal)>(((string Label, decimal Value) x) => x.Value);
		if (num3 <= 0m)
		{
			num3 = 1m;
		}
		using Pen pen = new Pen(Theme.Line);
		for (int num4 = 0; num4 <= 4; num4++)
		{
			float num5 = (float)(18 + num2) - (float)(num2 * num4) / 4f;
			graphics.DrawLine(pen, 64f, num5, 64 + num, num5);
			string text = (num3 * (decimal)num4 / 4m / 1000000m).ToString("0.#") + " tr";
			TextRenderer.DrawText(graphics, text, Theme.Small, new Point(4, (int)num5 - 8), Theme.Muted);
		}
		float num6 = (float)num / (float)Data.Count;
		float num7 = Math.Min(56f, num6 * 0.52f);
		for (int num8 = 0; num8 < Data.Count; num8++)
		{
			(string, decimal) tuple = Data[num8];
			float num9 = (float)(tuple.Item2 / num3) * (float)num2;
			float num10 = 64f + num6 * (float)num8 + (num6 - num7) / 2f;
			RectangleF rect = new RectangleF(num10, (float)(18 + num2) - num9, num7, num9);
			using LinearGradientBrush brush = new LinearGradientBrush(new RectangleF(rect.X, rect.Y, Math.Max(1f, rect.Width), Math.Max(1f, rect.Height + 1f)), Theme.Accent, ColorTranslator.FromHtml("#B65B72"), LinearGradientMode.Vertical);
			if (num9 > 0.5f)
			{
				graphics.FillRectangle(brush, rect);
			}
			string text2 = ((tuple.Item2 >= 1000000m) ? ((tuple.Item2 / 1000000m).ToString("0.#") + "tr") : tuple.Item2.ToString("#,##0"));
			TextRenderer.DrawText(graphics, text2, Theme.Small, new Rectangle((int)num10 - 12, (int)((float)(18 + num2) - num9) - 20, (int)num7 + 24, 18), Theme.Ink, TextFormatFlags.HorizontalCenter);
			TextRenderer.DrawText(graphics, tuple.Item1, Theme.Small, new Rectangle((int)(64f + num6 * (float)num8), 18 + num2 + 8, (int)num6, 20), Theme.Muted, TextFormatFlags.HorizontalCenter);
		}
	}
}
