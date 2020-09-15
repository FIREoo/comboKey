using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
//using System.Windows.Forms;
using System.Windows.Input;

namespace Utilities
{
    /// <summary>
    /// A class that manages a global low level keyboard hook
    /// </summary>
    class globalKeyboardHook
    {
        #region Constant, Structure and Delegate Definitions
        /// <summary>
        /// defines the callback type for the hook
        /// </summary>
        public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);
        private static keyboardHookProc callbackDelegate;

        public void hook()
        {
            if (callbackDelegate != null) throw new InvalidOperationException("Can't hook more than once");
            IntPtr hInstance = LoadLibrary("User32");
            callbackDelegate = new keyboardHookProc(hookProc);
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, callbackDelegate, hInstance, 0);
        }

        public void unhook()
        {
            if (callbackDelegate == null) return;
            bool ok = UnhookWindowsHookEx(hhook);
            callbackDelegate = null;
        }

        public struct keyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;
        #endregion

        #region Instance Variables
        /// <summary>
        /// The collections of keys to watch for
        /// </summary>
        public List<Key> HookedKeys = new List<Key>();
        /// <summary>
        /// Handle to the hook, need this to unhook and call the next hook
        /// </summary>
        IntPtr hhook = IntPtr.Zero;
        #endregion

        #region Events

        public delegate void KeyPressHandler(KeyArgs e);
        public event KeyPressHandler KeyEvent;
        /// <summary>Occurs when one of the hooked keys is pressed</summary>
        public event KeyPressHandler KeyDown;
        /// <summary>Occurs when one of the hooked keys is released</summary>
        public event KeyPressHandler KeyUp;
        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the <see cref="globalKeyboardHook"/> class and installs the keyboard hook.
        /// </summary>
        public globalKeyboardHook()
        {
            hook();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="globalKeyboardHook"/> is reclaimed by garbage collection and uninstalls the keyboard hook.
        /// </summary>
        ~globalKeyboardHook()
        {
            unhook();
        }
        #endregion

        #region Public Methods
        /// <summary>Installs the global hook</summary>
        //public void hook()
        //{
        //    IntPtr hInstance = LoadLibrary("User32");
        //    hhook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hInstance, 0);
        //}

        ///// <summary>
        ///// Uninstalls the global hook
        ///// </summary>
        //public void unhook()
        //{
        //    UnhookWindowsHookEx(hhook);
        //}

        /// <summary>
        /// The callback for the keyboard hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        public int hookProc(int code, int wParam, ref keyboardHookStruct lParam)
        {
            if (lParam.dwExtraInfo != 1)//等於1的話就是用虛擬的進來的
                if (code >= 0)
                {

                    Key key = KeyInterop.KeyFromVirtualKey(lParam.vkCode);
                    KeyArgs args = new KeyArgs(key, lParam.vkCode);
                    KeyEvent?.Invoke(args);
                    if (args.Handled)
                        return 1;//吃掉 這個鍵
                    if (HookedKeys.Contains(key))
                    {
                        if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                        {
                            KeyDown(args);
                        }
                        else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                        {
                            KeyUp(args);
                        }
                        if (args.Handled)
                            return 1;//吃掉 這個鍵
                    }
                }
            return CallNextHookEx(hhook, code, wParam, ref lParam);
        }
        #endregion

        #region DLL imports
        /// <summary>
        /// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
        /// </summary>
        /// <param name="idHook">The id of the event you want to hook</param>
        /// <param name="callback">The callback.</param>
        /// <param name="hInstance">The handle you want to attach the event to, can be null</param>
        /// <param name="threadId">The thread you want to attach the event to, can be null</param>
        /// <returns>a handle to the desired hook</returns>
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

        /// <summary>
        /// Unhooks the windows hook.
        /// </summary>
        /// <param name="hInstance">The hook handle that was returned from SetWindowsHookEx</param>
        /// <returns>True if successful, false otherwise</returns>
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        /// <summary>
        /// Calls the next hook.
        /// </summary>
        /// <param name="idHook">The hook id</param>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The wparam.</param>
        /// <param name="lParam">The lparam.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);

        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the library</param>
        /// <returns>A handle to the library</returns>
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);



        #endregion
    }
    public class KeyArgs
    {
        public KeyArgs(Key key, int vk_code)
        {
            vkCode = vk_code;
            Key = key;
            Handled = false;
        }
        //
        // 摘要:
        //     取得或設定值，這個值表示路由事件在傳送路由時之事件處理的目前狀態。
        //
        // 傳回:
        //     設定時，如果事件是要標記為已處理，則請設定為  true；否則為 false。 如果讀取這個值， true 表示類別處理常式或路由中的某個執行個體處理常式已將這個事件標記為已處理。
        //     false 表示沒有這類處理常式已將事件標記為已處理。 預設值是 false。
        public bool Handled { get; set; }
        //
        // 摘要:
        //     取得與事件相關聯的鍵盤按鍵。
        //
        // 傳回:
        //     System.Windows.Input.Key 事件參考。
        public Key Key { get; }
        public int vkCode { get; }
    }


}
