using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Keys = System.Windows.Forms.Keys;
using Point = System.Windows.Point;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Wpf_comboKeyboard
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
        globalKeyboardHook gkh = new globalKeyboardHook();//the only gkh 請不要建立兩個(我不知道會怎樣)

        /// <summary>替換按鍵List view data</summary>
        ObservableCollection<ListViewData> ListViewData_SwitchKey = new ObservableCollection<ListViewData>();
        /// <summary>替換按鍵表 [press key ,switch to]</summary>
        Dictionary<Key, Key> SwitchKeyDic = new Dictionary<Key, Key>();
        /// <summary>The combo key</summary>
        Key ComboKey = Key.CapsLock;
        /// <summary>The combo key press = True</summary>
        bool ComboKeyPress = false;
        /// <summary>Wait to set a combo key press</summary>
        bool waitAComboKey = false;

        //virtual keyboard
        [DllImport("user32.dll")]
        static extern void keybd_event(int bVk, byte bScan, uint dwFlags, int dwExtraInfo);//用來打字的

        ImageSource imgKeyMask;

        public MainWindow()
        {
            InitializeComponent();

            //Key  board UI 顯示
            var imgKeyboard = Wpf_comboKeyboard.Properties.Resources.keyboard80_word;
            MemoryStream memory = new MemoryStream();
            imgKeyboard.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            ImageSourceConverter converter = new ImageSourceConverter();
            ImageSource source = (ImageSource)converter.ConvertFrom(memory);
            img_keyboard.Source = source;

            //key mask UI
            var bitKeyMask = Wpf_comboKeyboard.Properties.Resources.mask_normal;
            memory = new MemoryStream();
            bitKeyMask.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            converter = new ImageSourceConverter();
            imgKeyMask = (ImageSource)converter.ConvertFrom(memory);

            //window icon UI
            var myIcon = Wpf_comboKeyboard.Properties.Resources.keyboard_icon1;
            memory = new MemoryStream();
            myIcon.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            converter = new ImageSourceConverter();
            source = (ImageSource)converter.ConvertFrom(memory);
            myWindow.Icon = source;

            // notifycation
            nIcon.Icon = Properties.Resources.keyboard_icon;
            nIcon.Visible = true;

            nIcon.DoubleClick += notify_DoubleClick;

            //notifycation menu
            System.Windows.Forms.ContextMenu nIconMenu = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem nIconMenuItem = new System.Windows.Forms.MenuItem();
            nIconMenuItem.Index = 0;
            nIconMenuItem.Text = "Close";
            nIconMenuItem.Click += new System.EventHandler((sender, e) => Close());
            nIconMenu.MenuItems.Add(nIconMenuItem);
            nIcon.ContextMenu = nIconMenu;

            keyboardImageGridIni();
            keyNameIni();

            //switch key list view
            LV_switchKey.ItemsSource = ListViewData_SwitchKey;
        }

        //combo key added in window loaded
        string infoFile = "";
        string keyPath = "KeyFile";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string MyDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            keyPath = MyDocumentsPath + "\\KeyFile";
            //check folder         
            if (Directory.Exists(keyPath) == false)
                Directory.CreateDirectory(keyPath);

            //creat info file
            infoFile = keyPath + "\\Combokey.txt";
            if (File.Exists(infoFile))
            {
                ReadTxtInfo();
            }
            else
            {
                WriteTxtInfo();
                ReadTxtInfo();
            }

            //gkh.HookedKeys.Add(Keys.RControlKey);
            //gkh.HookedKeys.Add(Keys.CapsLock);
            //gkh.HookedKeys.Add(Keys.I);//↑
            //gkh.HookedKeys.Add(Keys.K);//↓
            //gkh.HookedKeys.Add(Keys.J);//←
            //gkh.HookedKeys.Add(Keys.L);//→

            //gkh.HookedKeys.Add(Keys.U);//home
            //gkh.HookedKeys.Add(Keys.O);//end

            //gkh.HookedKeys.Add(Keys.P);//home
            //gkh.HookedKeys.Add(Keys.OemSemicolon);//end

            //gkh.HookedKeys.Add(Keys.H);//delet
            //gkh.HookedKeys.Add(Keys.Q);//esc

            //gkh.HookedKeys.Add(Keys.Tab);//capsLock

        }
        /// <summary>read text and hook</summary>
        void ReadTxtInfo()
        {
            string[] allString = System.IO.File.ReadAllLines(infoFile);
            //取得combo key
            string str = allString[0].Substring(9, allString[0].Length - 10);//comboKey(CapsLock)
            for (int L = 1; L < allString.Count(); L++)
            {
                string line = allString[L];
                if (line == "")
                    continue;
                bool error1 = Name2Key.TryGetValue(line.Split('\t')[0], out Key press_key);
                bool error2 = Name2Key.TryGetValue(line.Split('\t')[1], out Key switch_key);
                if (error1 == false || error2 == false)
                {
                    MessageBox.Show("File text error");
                    this.Close();
                }
                ListViewData_SwitchKey.Add(new ListViewData(line.Split('\t')[0], line.Split('\t')[1]));
                SwitchKeyDic.Add(press_key, switch_key);
                gkh.HookedKeys.Add(press_key);
            }
            ComboKey = Name2Key[str];
            gkh.HookedKeys.Add(ComboKey);
        }
        void WriteTxtInfo()
        {
            StreamWriter sw = new StreamWriter(infoFile, false);
            sw.WriteLine($"comboKey({Key2Name[ComboKey]})");
            sw.Flush();
            sw.Close();
        }

        //notify
        private void notify_DoubleClick(object sender, EventArgs e)
        {
            this.Show();//讓icon 在 底下出現
            this.Show();//不知道什麼bug，必須要連續show兩次，一次只會顯示在下面，一種沒有啟動的感覺。
            this.WindowState = WindowState.Normal;

            //this.Topmost = true ;
            //this.Activate();
            //this.Focus();
        }
        private void MyWindow_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Minimized:
                    {
                        this.Hide();//讓icon 在 底下消失
                        break;
                    }
            }
        }

        private void Tb_comboKey_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            tb_comboKey.Text = "---";
            gkh.HookedKeys.Remove(ComboKey);
            waitAComboKey = true;
        }

        /// <summary>啟用combo功能</summary>
        private void Cb_hook_Click(object sender, RoutedEventArgs e)
        {
            if (cb_hook.IsChecked == true)
            {
                gkh.KeyDown += new globalKeyboardHook.KeyPressHandler(gkh_KeyDown);
                gkh.KeyUp += new globalKeyboardHook.KeyPressHandler(gkh_KeyUp);
            }
            else
            {
                gkh.KeyDown -= gkh_KeyDown;
                gkh.KeyUp -= gkh_KeyUp;
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            


        }

        Grid Grid_LastClick = null;
        /// <summary>Keyboard UI image mouse down</summary>
        private void keyboardImageClick_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(Grid_LastClick != null)
            Grid_LastClick.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0,0,0,0));
               Grid_LastClick = ((Grid)sender);
            string Name = ((Grid)sender).Name;
            // ((Grid)sender).Background = new SolidBrush(Color.FromArgb(30, 30, 120, 210));
            ((Grid)sender).Background = new ImageBrush(imgKeyMask);
            Console.WriteLine(Name);

            if (waitAComboKey)
            {//set the combo key
                ComboKey = Name2Key[Name];
                if (!gkh.HookedKeys.Exists(x => x == ComboKey))
                    gkh.HookedKeys.Add(ComboKey);

                waitAComboKey = false;
                tb_comboKey.Text = Name;
            }

            //   select_002.Margin = ((Grid)sender).Margin;
        }

        void gkh_KeyEvent(KeyArgs e)
        {

            Console.WriteLine(e.Key.ToString());
            e.Handled = true;

        }
        void gkh_KeyDown(KeyArgs e)
        {
            if (e.Key == ComboKey)
            {
                ComboKeyPress = true;
                //Console.WriteLine("combo press");
                e.Handled = true;
            }
            else
            {
                if (ComboKeyPress == true)
                {
                    // SwitchKeyDic.TryGetValue();//應該不會有這個問題，因為你要在dic裡面才會被hook                
                    //Console.WriteLine(e.Key.ToString() + "_press =>" + SwitchKeyDic[e.Key].ToString());

                    keybd_event(KeyInterop.VirtualKeyFromKey(SwitchKeyDic[e.Key]), 0, 0, 0);
                    e.Handled = true;
                }
            }

        }
        void gkh_KeyUp(KeyArgs e)
        {
            //要注意一個問題
            //com press, A press, com release, A release  你A就會被吃掉
            if (e.Key == ComboKey)
            {
                ComboKeyPress = false;
                //Console.WriteLine("combo release");
                e.Handled = true;
            }
            else
            {
                if (ComboKeyPress == true)
                {
                    // SwitchKeyDic.TryGetValue();//應該不會有這個問題，因為你要在dic裡面才會被hook                
                    //Console.WriteLine(e.Key.ToString() + "_release");
                    if (SwitchKeyDic[e.Key] == Key.CapsLock)
                        CapsLockSwitch();
                    else
                        keybd_event(KeyInterop.VirtualKeyFromKey(SwitchKeyDic[e.Key]), 0, 0x0002, 0);
                    e.Handled = true;
                }
            }
        }
        void CapsLockSwitch()
        {
            gkh.unhook();
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            gkh.hook();
        }

        //select mask
        private void Img_keyboard_MouseEnter(object sender, MouseEventArgs e)
        {
            //   img_selectMask.Visibility = Visibility.Visible;
        }
        private void Img_keyboard_MouseLeave(object sender, MouseEventArgs e)
        {
            // img_selectMask.Visibility = Visibility.Hidden;
        }
        private void Img_keyboard_MouseMove(object sender, MouseEventArgs e)
        {

        }


        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Console.WriteLine(e.Key.ToString());
        }

        private void Img_keyboard_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Btn_openFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(keyPath);
        }
    }//main class
    public class ListViewData : INotifyPropertyChanged
    {
        bool _check;
        string _OrgKey;
        string _SwiKey;

        public ListViewData(string orgKey, string swiKey)
        {
            _check = false;
            OrgKey = orgKey;
            SwiKey = swiKey;
        }
        public bool isChecked
        {
            set
            {
                _check = value;
                NotifyPropertyChanged("isChecked");
            }
            get { return _check; }
        }
        public string OrgKey
        {
            set
            {
                _OrgKey = value;
                NotifyPropertyChanged("OrgKey");
            }
            get { return _OrgKey; }
        }
        public string SwiKey
        {
            set
            {
                _SwiKey = value;
                NotifyPropertyChanged("SwiKey");
            }
            get { return _SwiKey; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }
    }
}
