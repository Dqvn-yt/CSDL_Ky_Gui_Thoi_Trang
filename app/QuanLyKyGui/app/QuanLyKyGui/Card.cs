using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuanLyKyGui;

internal sealed class Card : Panel
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string Title { get; set; }

	public Panel Body { get; }

	public Card(string title, int padTop = 46)
	{
		Title = title;
		BackColor = Theme.Surface;
		DoubleBuffered = true;
		base.Padding = new Padding(16, padTop, 16, 14);
		base.Margin = new Padding(0, 0, 0, 12);
		Body = new Panel
		{
			Dock = DockStyle.Fill,
			BackColor = Color.Transparent
		};
		base.Controls.Add(Body);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
		using (Pen pen = new Pen(Theme.Line))
		{
			e.Graphics.DrawRectangle(pen, 0, 0, base.Width - 1, base.Height - 1);
		}
		TextRenderer.DrawText(e.Graphics, Title, Theme.H3, new Point(18, 15), Theme.Ink, TextFormatFlags.NoPrefix);
	}
}
