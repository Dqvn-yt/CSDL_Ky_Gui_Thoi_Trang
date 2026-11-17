using System.Drawing;
using System.Windows.Forms;

namespace QuanLyKyGui;

internal sealed class StatCard : Panel
{
	private readonly Label _title;

	private readonly Label _value;

	private readonly Label _note;

	private readonly Color _accent;

	public StatCard(string title, Color accent)
	{
		_accent = accent;
		BackColor = Theme.Surface;
		DoubleBuffered = true;
		base.Padding = new Padding(18, 14, 14, 12);
		_title = new Label
		{
			Text = title,
			Font = Theme.Small,
			ForeColor = Theme.Muted,
			AutoSize = true,
			Location = new Point(18, 12)
		};
		_value = new Label
		{
			Text = "—",
			Font = new Font("Segoe UI Semibold", 21f),
			ForeColor = Theme.Ink,
			AutoSize = true,
			Location = new Point(16, 32)
		};
		_note = new Label
		{
			Text = string.Empty,
			Font = Theme.Small,
			ForeColor = Theme.Muted,
			AutoSize = true,
			Location = new Point(18, 78)
		};
		base.Controls.Add(_title);
		base.Controls.Add(_value);
		base.Controls.Add(_note);
	}

	public void SetValue(string value, string note)
	{
		_value.Text = value;
		_note.Text = note;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		using (Pen pen = new Pen(Theme.Line))
		{
			e.Graphics.DrawRectangle(pen, 0, 0, base.Width - 1, base.Height - 1);
		}
		using SolidBrush brush = new SolidBrush(Color.FromArgb(26, _accent));
		e.Graphics.FillRectangle(brush, 0, 0, base.Width, 3);
	}
}
