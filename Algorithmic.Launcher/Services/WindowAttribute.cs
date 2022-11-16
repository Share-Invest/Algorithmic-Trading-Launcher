using System;
using System.Runtime.InteropServices;

namespace ShareInvest.Services;

static class WindowAttribute
{
    [DllImport("dwmapi.dll",
               CharSet = CharSet.Unicode,
               PreserveSig = false)]
    internal static extern void DwmSetWindowAttribute(IntPtr hwnd,
                                                      DWMWINDOWATTRIBUTE attribute,
                                                      ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                      uint cbAttribute);
    [DllImport("gdi32.dll",
               EntryPoint = "CreateRoundRectRgn")]
    internal static extern IntPtr CreateRoundRectRgn(int nLeftRect,
                                                     int nTopRect,
                                                     int nRightRect,
                                                     int nBottomRect,
                                                     int nWidthEllipse,
                                                     int nHeightEllipse);
    [DllImport("user32.dll")]
    internal static extern int SetWindowRgn(IntPtr hWnd,
                                            IntPtr hRgn,
                                            bool bRedraw);
}
enum DWMWINDOWATTRIBUTE
{
    DWMWA_WINDOW_CORNER_PREFERENCE = 33
}
enum DWM_WINDOW_CORNER_PREFERENCE
{
    DWMWCP_DEFAULT = 0,
    DWMWCP_DONOTROUND = 1,
    DWMWCP_ROUND = 2,
    DWMWCP_ROUNDSMALL = 3
}