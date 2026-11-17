using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace QuanLyKyGui;

internal static class Program
{
	private static string? _captureFolder;

	[DllImport("user32.dll")]
	private static extern bool PrintWindow(nint hwnd, nint hdcBlt, uint nFlags);

	[STAThread]
	private static void Main(string[] args)
	{
		ApplicationConfiguration.Initialize();
		Application.SetDefaultFont(new Font("Segoe UI", 9.75f));
		int num = Array.FindIndex(args, (string a) => a.Equals("--capture", StringComparison.OrdinalIgnoreCase));
		if (num >= 0)
		{
			_captureFolder = ((num + 1 < args.Length && !args[num + 1].StartsWith("--")) ? args[num + 1] : "E:\\Quanap\\screenshots");
			RunCapture();
			return;
		}
		if (args.Any((string a) => a.Equals("--export", StringComparison.OrdinalIgnoreCase)))
		{
			RunExport();
			return;
		}
		int num2 = Array.FindIndex(args, (string a) => a.Equals("--open", StringComparison.OrdinalIgnoreCase));
		if (num2 >= 0 && num2 + 1 < args.Length)
		{
			Application.Run(new frmMain(args[num2 + 1]));
		}
		else
		{
			Application.Run(new frmMain());
		}
	}

	private static void RunExport()
	{
		Directory.CreateDirectory("E:\\Quanap\\exports");
		string path = Path.Combine("E:\\Quanap\\exports", "export-log.txt");
		List<string> list = new List<string> { $"Bắt đầu {DateTime.Now:dd/MM/yyyy HH:mm:ss}" };
		try
		{
			using (frmTraCuuSanPham frmTraCuuSanPham2 = new frmTraCuuSanPham())
			{
				frmTraCuuSanPham2.PrepareCapture();
				list.Add("Tra cứu: " + (frmTraCuuSanPham2.ExportForDemo() ?? "(không xuất)"));
			}
			using (frmBaoCaoDoanhThu frmBaoCaoDoanhThu2 = new frmBaoCaoDoanhThu())
			{
				frmBaoCaoDoanhThu2.PrepareCapture();
				list.Add("Doanh thu: " + (frmBaoCaoDoanhThu2.ExportForDemo() ?? "(không xuất)"));
			}
			using (frmBaoCaoQuyetToan frmBaoCaoQuyetToan2 = new frmBaoCaoQuyetToan())
			{
				frmBaoCaoQuyetToan2.PrepareCapture();
				list.Add("Quyết toán: " + (frmBaoCaoQuyetToan2.ExportForDemo() ?? "(không xuất)"));
			}
			list.Add($"Hoàn tất {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
		}
		catch (Exception ex)
		{
			list.Add("LỖI: " + ex);
		}
		File.WriteAllLines(path, list);
	}

	private static void RunCapture()
	{
		Directory.CreateDirectory(_captureFolder);
		string path = Path.Combine(_captureFolder, "capture-log.txt");
		File.WriteAllText(path, $"Bắt đầu {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n");
		try
		{
			using (frmMain form = new frmMain())
			{
				Shot(form, "01-main");
			}
			using (frmTiepNhanKyGui frmTiepNhanKyGui2 = new frmTiepNhanKyGui())
			{
				frmTiepNhanKyGui2.PrepareCapture();
				Shot(frmTiepNhanKyGui2, "02-tiep-nhan-ky-gui");
			}
			using (frmLapDonHang frmLapDonHang2 = new frmLapDonHang())
			{
				frmLapDonHang2.PrepareCapture();
				Shot(frmLapDonHang2, "03-lap-don-hang");
			}
			using (frmTraCuuSanPham frmTraCuuSanPham2 = new frmTraCuuSanPham())
			{
				frmTraCuuSanPham2.PrepareCapture();
				Shot(frmTraCuuSanPham2, "04-tra-cuu-san-pham");
			}
			using (frmBaoCaoDoanhThu frmBaoCaoDoanhThu2 = new frmBaoCaoDoanhThu())
			{
				frmBaoCaoDoanhThu2.PrepareCapture();
				Shot(frmBaoCaoDoanhThu2, "05-bao-cao-doanh-thu");
			}
			using (frmBaoCaoQuyetToan frmBaoCaoQuyetToan2 = new frmBaoCaoQuyetToan())
			{
				frmBaoCaoQuyetToan2.PrepareCapture();
				Shot(frmBaoCaoQuyetToan2, "06-bao-cao-quyet-toan");
			}
			File.AppendAllText(path, $"Hoàn tất {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n");
		}
		catch (Exception ex)
		{
			File.AppendAllText(path, "LỖI: " + ex?.ToString() + "\n");
			MessageBox.Show(ex.Message, "Lỗi chụp màn hình", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private static void Shot(Form form, string name)
	{
		form.StartPosition = FormStartPosition.Manual;
		form.Location = new Point(20, 20);
		form.ShowInTaskbar = false;
		if (!form.Visible)
		{
			form.Show();
		}
		form.Activate();
		for (int i = 0; i < 40; i++)
		{
			Application.DoEvents();
			Thread.Sleep(15);
		}
		form.Refresh();
		Application.DoEvents();
		using Bitmap bitmap = new Bitmap(form.Width, form.Height);
		using (Graphics graphics = Graphics.FromImage(bitmap))
		{
			nint hdc = graphics.GetHdc();
			try
			{
				if (!PrintWindow(form.Handle, hdc, 2u))
				{
					form.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
				}
			}
			finally
			{
				graphics.ReleaseHdc(hdc);
			}
		}
		string text = Path.Combine(_captureFolder, name + ".png");
		bitmap.Save(text, ImageFormat.Png);
		File.AppendAllText(Path.Combine(_captureFolder, "capture-log.txt"), "Đã lưu " + text + "\n");
	}
}
