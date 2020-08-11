using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Utilities;

namespace CS_keyboardHook
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);//用來打字的
        const uint KEYEVENTF_KEYUP = 0x0002;
        List<microInfo> microInfos = new List<microInfo>();

        globalKeyboardHook gkh = new globalKeyboardHook();
        bool capsPress = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gkh.HookedKeys.Add(Keys.CapsLock);
            gkh.HookedKeys.Add(Keys.I);//↑
            gkh.HookedKeys.Add(Keys.K);//↓
            gkh.HookedKeys.Add(Keys.J);//←
            gkh.HookedKeys.Add(Keys.L);//→

            gkh.HookedKeys.Add(Keys.U);//home
            gkh.HookedKeys.Add(Keys.O);//end

            gkh.HookedKeys.Add(Keys.P);//home
            gkh.HookedKeys.Add(Keys.OemSemicolon);//end

            gkh.HookedKeys.Add(Keys.H);//delet
            gkh.HookedKeys.Add(Keys.Q);//esc

            gkh.HookedKeys.Add(Keys.Tab);//capsLock

            gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
            gkh.KeyUp += new KeyEventHandler(gkh_KeyUp);

            //micro //if want to add micro
            microInfos.Add(new microInfo(Keys.I, Keys.Up));
            microInfos.Add(new microInfo(Keys.K, Keys.Down));
            microInfos.Add(new microInfo(Keys.J, Keys.Left));
            microInfos.Add(new microInfo(Keys.L, Keys.Right));
            microInfos.Add(new microInfo(Keys.U, Keys.Home));
            microInfos.Add(new microInfo(Keys.O, Keys.End));
            microInfos.Add(new microInfo(Keys.P, Keys.PageUp));
            microInfos.Add(new microInfo(Keys.OemSemicolon, Keys.PageDown));
            microInfos.Add(new microInfo(Keys.H, Keys.Delete));
            microInfos.Add(new microInfo(Keys.Q, Keys.Escape));
            microInfos.Add(new microInfo(Keys.Tab, Keys.CapsLock));
            //0x64
        }
        void downKey(Keys _key)
        {
            keybd_event(KeysToVK(_key), 0, 0, 0);
        }
        void upKey(Keys _key)
        {
            keybd_event(KeysToVK(_key), 0, KEYEVENTF_KEYUP, 0);
        }

        /// <summary>Forms.Keys to Virtual-Key Codes</summary>
        byte KeysToVK(Keys _key)
        {
            //https://docs.microsoft.com/zh-tw/windows/desktop/inputdev/virtual-key-codes       
            switch (_key)
            {
                case Keys.Up:
                    return 0x26;
                case Keys.Down:
                    return 0x28;
                case Keys.Left:
                    return 0x25;
                case Keys.Right:
                    return 0x27;
                case Keys.Home:
                    return 0x24;
                case Keys.End:
                    return 0x23;
                case Keys.PageUp:
                    return 0x21;
                case Keys.PageDown:
                    return 0x22;
                case Keys.Delete:
                    return 0x2E;
                case Keys.Escape:
                    return 0x1B;
                case Keys.CapsLock:
                    return 0x14;
                default:
                    return 0;
            }
        }
        void capsLockSwitch()
        {
            gkh.unhook();
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            gkh.hook();
        }

        void gkh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.CapsLock)
            {
                //不知道為什麼只要進去這裡，按鍵就會被吃掉，也就是按了原本的會沒作用
                capsPress = true;
                e.Handled = true;//有了這個才會取代原本的event(也就是上面說的吃掉)
            }

            if (capsPress == true)
            {
                for (int i = 0; i < microInfos.Count; i++)
                {
                    if (e.KeyCode == Keys.Tab)
                        break;
                    if (e.KeyCode == microInfos[i].pressKey)
                    {
                        downKey(microInfos[i].outputKey);
                        break;
                    }
                }
                e.Handled = true;
            }
        }
        void gkh_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.CapsLock)
            {
                capsPress = false;
                e.Handled = true;
            }
            if (capsPress == true)
            {
                for (int i = 0; i < microInfos.Count; i++)
                {
                    if (e.KeyCode == Keys.Tab)
                    {
                        capsLockSwitch();
                        break;
                    }
                    if (e.KeyCode == microInfos[i].pressKey)
                    {
                        upKey(microInfos[i].outputKey);
                        break;
                    }
                }
                e.Handled = true;
            }

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //如果目前是縮小狀態，才要回覆成一般大小的視窗
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            // Activate the form.
            this.Activate();
            this.Focus();

        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void debugreleaseAllKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < microInfos.Count; i++)
                upKey(microInfos[i].outputKey);
        }


    }
    struct microInfo
    {
        public Keys pressKey;
        public Keys outputKey;
        public microInfo(Keys _press, Keys _out)
        {
            pressKey = _press;
            outputKey = _out;
        }
    }

}

