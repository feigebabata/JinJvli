using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FGUFW.Core
{
    static public class WinPlatform
    {
        #region 显示器分辨率

        // GetSystemMetrics实际获取的是系统记录的分辨率，不是物理分辨率，如屏幕2560*1600，显示缩放200%，这里获取到的是1280*800
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetSystemMetrics(int nIndex);
        private static int SM_CXSCREEN = 0; //主屏幕分辨率宽度
        private static int SM_CYSCREEN = 1; //主屏幕分辨率高度
        // private static int SM_CYCAPTION = 4; //标题栏高度
        // private static int SM_CXFULLSCREEN = 16; //最大化窗口宽度（减去任务栏）
        // private static int SM_CYFULLSCREEN = 17; //最大化窗口高度（减去任务栏）

        static public Vector2Int GetScreenSize()
        {
            var size = Vector2Int.zero;
            #if UNITY_STANDALONE_WIN
            size.x = GetSystemMetrics(SM_CXSCREEN);
            size.y = GetSystemMetrics(SM_CYSCREEN);
            #endif
            return size;
        }

        #endregion

        #region 文件管理器

        /// <summary>
        /// 文件日志类
        /// </summary>
        // [特性(布局种类.有序,字符集=字符集.自动)]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class ChinarFileDlog
        {
            public int    structSize    = 0;
            public IntPtr dlgOwner      = IntPtr.Zero;
            public IntPtr instance      = IntPtr.Zero;

            /// <summary>
            /// 过滤
            /// </summary>
            public String filter        = null;
            public String customFilter  = null;
            public int    maxCustFilter = 0;
            public int    filterIndex   = 0;

            /// <summary>
            /// 文件路径
            /// </summary>
            public String file          = null;
            public int    maxFile       = 0;
            public String fileTitle     = null;
            public int    maxFileTitle  = 0;

            /// <summary>
            /// 默认路径
            /// </summary>
            public String initialDir    = null;

            /// <summary>
            /// 弹窗标题
            /// </summary>
            public String title         = null;
            public int    flags         = 0;
            public short  fileOffset    = 0;
            public short  fileExtension = 0;
            public String defExt        = null;
            public IntPtr custData      = IntPtr.Zero;
            public IntPtr hook          = IntPtr.Zero;
            public String templateName  = null;
            public IntPtr reservedPtr   = IntPtr.Zero;
            public int    reservedInt   = 0;
            public int    flagsEx       = 0;
        }

        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] ChinarFileDlog ofd);

        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] ChinarFileDlog ofd);

        static public string OpenFilePath(string initialDir,string extension="*",string title="打开文件")
        {
            string path = null;
            #if UNITY_STANDALONE_WIN
            // string filter = $"{extension} files (*.{extension})|*.{extension}";
            var dlog = new ChinarFileDlog();
            dlog.structSize = Marshal.SizeOf(dlog);
            dlog.filter = $"{extension}文件(*.{extension})\0*.{extension}";
            dlog.file = new string(new char[256]);
            dlog.maxFile = dlog.file.Length;
            dlog.fileTitle = new string(new char[64]);
            dlog.maxFileTitle = dlog.fileTitle.Length;
            dlog.initialDir = initialDir;
            dlog.title = title;
            dlog.defExt = "dat";
            dlog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
            if(GetOpenFileName(dlog))
            {
                path = dlog.file;
            }
            #endif
            return path;
        }        
        
        static public string SaveFilePath(string initialDir,string extension="*",string title="保存文件")
        {
            string path = null;
            #if UNITY_STANDALONE_WIN
            string filter = $"{extension} files (*.{extension})|*.{extension}";
            var dlog = new ChinarFileDlog();
            dlog.structSize = Marshal.SizeOf(dlog);
            dlog.filter = filter;
            dlog.file = new string(new char[256]);
            dlog.maxFile = dlog.file.Length;
            dlog.fileTitle = new string(new char[64]);
            dlog.maxFileTitle = dlog.fileTitle.Length;
            dlog.initialDir = initialDir;
            dlog.title = title;
            dlog.defExt = "dat";
            dlog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
            if(GetSaveFileName(dlog))
            {
                path = dlog.file;
            }
            #endif
            return path;
        }

        [Conditional("UNITY_STANDALONE_WIN")]
        static public void OpenExplorer(string path)
        {
            path = path.Replace('/','\\');
            Process p = new Process();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = $" {path}";
            p.Start();
        }

        #endregion
    }
}