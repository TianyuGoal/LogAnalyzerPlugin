using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using LogAnalyzerPlugin.NppPlugin;
using LogAnalyzerPlugin.Forms;

namespace LogAnalyzerPlugin
{
    /// <summary>
    /// 插件入口类，实现 Notepad++ Plugin API
    /// </summary>
    public static class Main
    {
        // ─── 插件名称（必须与目录名一致）─────────────────────────
        public const string PluginName = "LogAnalyzer";

        // ─── Notepad++ 句柄 ──────────────────────────────────────
        private static NppData _nppData;

        // ─── 菜单命令 ────────────────────────────────────────────
        private static FuncItem[] _funcItems;
        private static MainForm _form = null;

        // ─── Notepad++ Plugin API 入口（必须按此签名 DllExport）──

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool isUnicode()
        {
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void setInfo(NppData notepadPlusData)
        {
            _nppData = notepadPlusData;
            InitFuncItems();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr getFuncsArray(ref int nbF)
        {
            nbF = _funcItems.Length;

            var funcItemsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FuncItem)) * nbF);
            for (int i = 0; i < nbF; i++)
            {
                var ptr = new IntPtr(funcItemsPtr.ToInt64() + i * Marshal.SizeOf(typeof(FuncItem)));
                Marshal.StructureToPtr(_funcItems[i], ptr, false);
            }
            return funcItemsPtr;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint messageProc(uint message, IntPtr wParam, IntPtr lParam)
        {
            return 1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void beNotified(IntPtr notifyCode)
        {
            // 处理 Notepad++ 通知（如文件打开/切换等）
        }

        // ─── 内部实现 ────────────────────────────────────────────

        private static void InitFuncItems()
        {
            _funcItems = new FuncItem[2];

            // 菜单项 0：打开日志分析器
            _funcItems[0]._itemName = "打开日志分析器";
            _funcItems[0]._pFunc = Marshal.GetFunctionPointerForDelegate(
                (NppFuncItemDelegate)OpenLogAnalyzer);

            // 菜单项 1：关于
            _funcItems[1]._itemName = "关于...";
            _funcItems[1]._pFunc = Marshal.GetFunctionPointerForDelegate(
                (NppFuncItemDelegate)ShowAbout);
        }

        private static void OpenLogAnalyzer()
        {
            if (_form == null || _form.IsDisposed)
            {
                _form = new MainForm();
            }

            string text = GetCurrentDocumentText();
            _form.SetText(text);

            if (!_form.Visible)
                _form.Show();
            else
                _form.Activate();
        }

        private static void ShowAbout()
        {
            MessageBox.Show(
                "日志分析器 v1.0\n\n" +
                "功能：多关键字搜索、上下文提取、共现分析、时间线视图\n\n" +
                "支持 Notepad++ 8.7.8 (64位)",
                "关于 LogAnalyzer",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>获取当前编辑器文档的全部文本</summary>
        private static string GetCurrentDocumentText()
        {
            try
            {
                IntPtr sci = GetCurrentScintillaHandle();

                int len = (int)Win32.SendMessage(
                    sci, (uint)SciMsg.SCI_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);

                if (len == 0) return "";

                var sb = new StringBuilder(len + 1);
                Win32.SendMessage(sci, (uint)SciMsg.SCI_GETTEXT, new IntPtr(len + 1), sb);
                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }

        private static IntPtr GetCurrentScintillaHandle()
        {
            Win32.SendMessage(
                _nppData._nppHandle,
                (uint)NppMsg.NPPM_GETCURRENTSCINTILLA,
                IntPtr.Zero,
                out int which);

            return which == 0 ? _nppData._scintillaMainHandle : _nppData._scintillaSecondHandle;
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void NppFuncItemDelegate();
}
