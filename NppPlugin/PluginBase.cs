using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LogAnalyzerPlugin.NppPlugin
{
    // Notepad++ 消息常量
    public static class NppMsg
    {
        public const int NPPM_GETCURRENTSCINTILLA = 2038;
        public const int NPPM_GETFULLCURRENTPATH = 2041;
        public const int NPPM_MENUCOMMAND = 2008;
        public const int NPPM_GETPLUGINSHOMEPATH = 2070;
        public const int NPPM_GETNBOPENFILES = 2048;
        public const int NPPM_GETOPENFILENAMES = 2049;
    }

    public static class SciMsg
    {
        public const int SCI_GETTEXT = 2182;
        public const int SCI_GETTEXTLENGTH = 2183;
        public const int SCI_INDICSETSTYLE = 2080;
        public const int SCI_INDICSETFORE = 2082;
        public const int SCI_SETINDICATORCURRENT = 2500;
        public const int SCI_INDICATORFILLRANGE = 2504;
        public const int SCI_INDICATORCLEARRANGE = 2505;
        public const int SCI_INDICSETALPHA = 2523;
        public const int INDIC_ROUNDBOX = 7;
        public const int INDIC_STRAIGHTBOX = 8;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NppData
    {
        public IntPtr _nppHandle;
        public IntPtr _scintillaMainHandle;
        public IntPtr _scintillaSecondHandle;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FuncItem
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string _itemName;
        public IntPtr _pFunc;
        public int _cmdID;
        public bool _init2Check;
        public IntPtr _pShKey;
    }

    public static class Win32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, out int lParam);
    }
}
