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
using System.Reflection;
using Color = System.Windows.Media.Color;

[assembly: AssemblyVersion("1.1.2")]
/*
 *  v1.1.1
 *  修改按著 combo key 會連續開關numLock的問題
 *  v1.1.2
 *  修改按著 combo key 會偶而切換numLock的問題
 */

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
        /// <summary>替換按鍵表_用於micro與file [press key ,switch to]</summary>
        Dictionary<Key, string> SwitchKeyDic_ex = new Dictionary<Key, string>();

        /// <summary>The combo key</summary>
        Key ComboKey = Key.CapsLock;
        /// <summary>The combo key press = True</summary>
        bool ComboKeyPress = false;
        /// <summary>Wait to set a combo key press</summary>
        bool waitAComboKey = false;

        //virtual keyboard
        [DllImport("user32.dll")]
        static extern void keybd_event(int bVk, byte bScan, uint dwFlags, int dwExtraInfo);//用來打字的
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);
        bool CapsLock = false;
        bool NumLock = false;
        bool ScrollLock = false;
        bool UserNumLock = true;

        ImageSource imgKeyMask;

        //UI value
        bool needSave = false;

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
                ReadTxtInfo_andHook();
            }
            else
            {
                WriteTxtInfo();
                ReadTxtInfo_andHook();
            }

            //啟動時要注意numlock的狀態//應該放在Hook時
            //然後要綁定numlock讓程式永遠知道numlock而不需要每次檢查
            CapsLock = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
            NumLock = (((ushort)GetKeyState(0x90)) & 0xffff) != 0;
            UserNumLock = NumLock;
            ScrollLock = (((ushort)GetKeyState(0x91)) & 0xffff) != 0;
            gkh.HookedKeys.Add(Key.NumLock);
        }
        /// <summary>read text and hook</summary>
        void ReadTxtInfo_andHook()
        {
            string[] allString = System.IO.File.ReadAllLines(infoFile);
            //取得combo key
            string str_combo = allString[0].Substring(9, allString[0].Length - 10);//comboKey(CapsLock)
            ComboKey = Name2Key[str_combo];
            gkh.HookedKeys.Add(ComboKey);
            tb_comboKey.Text = str_combo;

            for (int L = 1; L < allString.Count(); L++)
            {
                string line = allString[L];
                if (line == "")
                    continue;
                bool error1 = Name2Key.TryGetValue(line.Split('\t')[0], out Key press_key);
                if (error1 == false) { MessageBox.Show("File text error"); return; }
                bool error2 = Name2Key.TryGetValue(line.Split('\t')[1], out Key switch_key);
                if (error2 == false) { MessageBox.Show("File text error"); return; }

                if (switch_key == Key.F20 || switch_key == Key.F21)
                {
                    SwitchKeyDic.Add(press_key, switch_key);
                }
                else
                {
                    SwitchKeyDic.Add(press_key, switch_key);
                }


                ListViewData_SwitchKey.Add(new ListViewData(line.Split('\t')[0], line.Split('\t')[1]));
                gkh.HookedKeys.Add(press_key);
            }

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

        /// <summary>設定combo功能</summary>
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
                NumLock = (((ushort)GetKeyState(0x90)) & 0xffff) != 0;
                UserNumLock = NumLock;
                gkh.KeyDown += new globalKeyboardHook.KeyPressHandler(gkh_KeyDown);
                gkh.KeyUp += new globalKeyboardHook.KeyPressHandler(gkh_KeyUp);
            }
            else
            {
                gkh.KeyDown -= gkh_KeyDown;
                gkh.KeyUp -= gkh_KeyUp;
            }
        }


        Grid Grid_LastClick = null;
        /// <summary>Keyboard UI image mouse down</summary>
        private void keyboardImageClick_MouseDown(object sender, MouseButtonEventArgs e)
        {
            tb_switchKey.Text = "";
            if (Grid_LastClick != null)
                Grid_LastClick.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            if (Grid_LastClick == (Grid)sender)
            {
                Grid_LastClick.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                Grid_LastClick = null;
                tb_Select.Text = "";
                return;
            }
            string Name = ((Grid)sender).Name;
            ((Grid)sender).Background = new SolidColorBrush(Color.FromArgb(100, 38, 25, 176));


            foreach (ListViewData lv in ListViewData_SwitchKey)
            {

                if (NameOfKey(lv.OrgKey) == Name)
                {
                    tb_switchKey.Text = lv.SwiKey;
                    switchKeyUI(1);
                    break;
                }
            }

            if (waitAComboKey)
            {//set the combo key
                ComboKey = Name2Key[Name];
                if (!gkh.HookedKeys.Exists(x => x == ComboKey))
                    gkh.HookedKeys.Add(ComboKey);

                waitAComboKey = false;
                tb_comboKey.Text = Name;
            }

            tb_Select.Text = Name;
            Grid_LastClick = ((Grid)sender);
        }


        //gkh Hook
        void gkh_KeyEvent(KeyArgs e)
        {

            Console.WriteLine(e.Key.ToString());
            e.Handled = true;

        }
        void gkh_KeyDown(KeyArgs e)
        {
            if (e.Key == ComboKey)
            {
                if (ComboKeyPress == true)
                {//如果按下 卻沒有放開 就再一次的進來，代表一直按著，必須排除避免bug
                    e.Handled = true;
                    return;
                }
                ComboKeyPress = true;
                if (UserNumLock == true)
                {//若使用者原先有NumLock則必須取消，避免key上下左右 變成數字的windows bug
                    keybd_event(KeyInterop.VirtualKeyFromKey(Key.NumLock), 0, 0, 1);
                    keybd_event(KeyInterop.VirtualKeyFromKey(Key.NumLock), 0, 0x0002, 1);
                    //會在 KeyUp 後還回
                }
                //Console.WriteLine("combo press");
                e.Handled = true;
            }
            else
            {
                if (ComboKeyPress == true)
                {
                    // SwitchKeyDic.TryGetValue();//應該不會有這個問題，因為你要在dic裡面才會被hook                
                    //Console.WriteLine(e.Key.ToString() + "_press =>" + SwitchKeyDic[e.Key].ToString());
                    try
                    {//numlock不在其中，但是有被hook
                        keybd_event(KeyInterop.VirtualKeyFromKey(SwitchKeyDic[e.Key]), 0, 0, 1);
                    }
                    catch { }
                    e.Handled = true;
                }
                else if (e.Key == Key.NumLock)
                {
                    NumLock = !((((ushort)GetKeyState(0x90)) & 0xffff) != 0);//給個反向，因為先get才換
                    UserNumLock = NumLock;
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
                if (UserNumLock == true)
                {
                    keybd_event(KeyInterop.VirtualKeyFromKey(Key.NumLock), 0, 0, 1);
                    keybd_event(KeyInterop.VirtualKeyFromKey(Key.NumLock), 0, 0x0002, 1);
                }
                //Console.WriteLine("combo release");
                e.Handled = true;
            }
            else
            {
                if (ComboKeyPress == true)
                {
                    // SwitchKeyDic.TryGetValue();//應該不會有這個問題，因為你要在dic裡面才會被hook                
                    //Console.WriteLine(e.Key.ToString() + "_release");
                    try
                    {
                        if (SwitchKeyDic[e.Key] == Key.CapsLock)
                            CapsLockSwitch();
                        else
                            keybd_event(KeyInterop.VirtualKeyFromKey(SwitchKeyDic[e.Key]), 0, 0x0002, 1);
                    }
                    catch { } //numlock不在其中，但是有被hook

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

        //test
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NumLock = (((ushort)GetKeyState(0x90)) & 0xffff) != 0;//給個反向，因為先get才換
            MessageBox.Show($"{NumLock.ToString()},user:{UserNumLock.ToString()}");
            //    if (NumLock == true)
            //keybd_event(KeyInterop.VirtualKeyFromKey(Key.NumLock), 0, 0, 0);//結束後numLock自己變成False了
            //keybd_event(KeyInterop.VirtualKeyFromKey(Key.NumLock), 0, 0x0002, 0);
            //MessageBox.Show($"up:{KeyInterop.VirtualKeyFromKey(Key.Up)},num8:{KeyInterop.VirtualKeyFromKey(Key.NumPad8)}");
            //for (int i = 0; i < 10; i++)
            //{
            //    System.Threading.Thread.Sleep(1000);
            //    keybd_event(KeyInterop.VirtualKeyFromKey(Key.Up), 0, 0, 0);
            //}

        }
        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Console.WriteLine(e.Key.ToString());
        }
        private void Img_keyboard_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        //folder and file
        private void Btn_openFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(keyPath);
        }
        private void Btn_reloadFile_Click(object sender, RoutedEventArgs e)
        {
            ListViewData_SwitchKey.Clear();
            SwitchKeyDic.Clear();
            gkh.HookedKeys.Clear();
            gkh.HookedKeys.Add(Key.NumLock);
            ReadTxtInfo_andHook();
        }
        private void Btn_saveFile_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter streamWriter = new StreamWriter(infoFile, false);
            streamWriter.WriteLine($"comboKey({Key2Name[ComboKey]})");
            foreach (ListViewData lv in ListViewData_SwitchKey)
            {
                streamWriter.WriteLine($"{ lv.OrgKey}\t{lv.SwiKey}");
            }
            streamWriter.Flush();
            streamWriter.Close();
        }
        private void Btn_StartUp_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
        }

        /// <summary>switch key的radio button UI (1-key, 2-micro, 3-file)</summary>
        int nowRadio = 0;
        void switchKeyUI(int index)
        {
            if (rb_switchMicro == null)
                return;
            if (rb_switchFile == null)
                return;

            if (index == 1)
            {
                nowRadio = 1;
                rb_switchKey.IsChecked = true;
                rb_switchMicro.IsChecked = false;
                rb_switchFile.IsChecked = false;

                tb_switchKey.Visibility = Visibility.Visible;
                tb_switchMicro.Visibility = Visibility.Hidden;
                tb_switchFile.Visibility = Visibility.Hidden;
            }
            else if (index == 2)
            {
                nowRadio = 2;
                rb_switchKey.IsChecked = false;
                rb_switchMicro.IsChecked = true;
                rb_switchFile.IsChecked = false;

                tb_switchKey.Visibility = Visibility.Hidden;
                tb_switchMicro.Visibility = Visibility.Visible;
                tb_switchFile.Visibility = Visibility.Hidden;

            }
            else if (index == 3)
            {
                nowRadio = 3;
                rb_switchKey.IsChecked = false;
                rb_switchMicro.IsChecked = false;
                rb_switchFile.IsChecked = true;

                tb_switchKey.Visibility = Visibility.Hidden;
                tb_switchMicro.Visibility = Visibility.Hidden;
                tb_switchFile.Visibility = Visibility.Visible;
            }
        }
        private void rb_switch_switched(object sender, RoutedEventArgs e)
        {
            if (rb_switchKey.IsChecked == true)
            {
                switchKeyUI(1);
            }
            else if (rb_switchMicro.IsChecked == true)
            {
                switchKeyUI(2);
            }
            else if (rb_switchFile.IsChecked == true)
            {
                switchKeyUI(3);
            }
        }
        //switch to
        bool lock_switchKey = false;//true-locked;
        private void Tb_switchKey_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lock_switchKey = true;
            ((TextBox)sender).Text = "--press--";
            ((TextBox)sender).Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 100));
        }
        private void Tb_switchKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (lock_switchKey == true)
            {
                ((TextBox)sender).Text = Key2Name[e.Key];
                ((TextBox)sender).Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                lock_switchKey = false;
            }

        }

        private void Tb_switchFile_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
        private void Tb_switchFile_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Count() > 1)
                {
                    MessageBox.Show("請勿放多個檔案");
                    return;
                }
                tb_switchFile.Text = files[0];
                /* foreach (string folderPath in files)
                 {
                     DirectoryInfo OpenDirectory = new DirectoryInfo(folderPath);
                     try
                     {
                         var OpenFiles = OpenDirectory.GetFiles("*.png"); //Getting Text files
                     }
                     catch
                     {
                         MessageBox.Show("Drop in root Folder! NOT files!");
                         return;
                     }

                     string addStr = folderPath.Substring(folderPath.LastIndexOf("\\") + 1);
                     addStr += "/";

                 }
             }

             if (e.Data.GetDataPresent(DataFormats.FileDrop))
             {
                 string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                 if (files.Count() > 1)
                 {
                     MessageBox.Show("one file only");
                     return;
                 }
                 ((TextBox)sender).Text = files[0];
                 Console.WriteLine(files[0]);*/
            }
        }

        private void Btn_SetKey_Click(object sender, RoutedEventArgs e)
        {
            if (nowRadio == 1)
            {//key
             //  ListViewData_SwitchKey.Add(new ListViewData());
            }
            else if (nowRadio == 2)
            {//micro

            }
            else if (nowRadio == 3)
            {//file

            }
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
