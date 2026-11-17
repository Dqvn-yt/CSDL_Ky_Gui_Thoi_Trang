using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyKyGui;

internal static class Fields
{
	public static Panel Field(string label, Control input)
	{
		Panel panel = new Panel
		{
			Width = input.Width,
			Height = 54,
			Margin = new Padding(0, 0, 12, 6),
			BackColor = Color.Transparent
		};
		Label label2 = new Label
		{
			Text = label,
			AutoSize = true,
			Font = Theme.Small,
			ForeColor = Theme.Muted,
			Location = new Point(0, 0)
		};
		input.Location = new Point(0, 20);
		panel.Controls.Add(label2);
		panel.Controls.Add(input);
		int val = TextRenderer.MeasureText(label2.Text, label2.Font).Width + 4;
		panel.Width = Math.Max(input.Width, val);
		return panel;
	}

	public static FlowLayoutPanel Flow(int width, int height, bool wrap = true)
	{
		return new FlowLayoutPanel
		{
			Width = width,
			Height = height,
			AutoScroll = true,
			WrapContents = wrap,
			BackColor = Color.Transparent,
			Margin = new Padding(0),
			Padding = new Padding(0)
		};
	}
}
