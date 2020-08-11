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


namespace Wpf_comboKeyboard
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
        globalKeyboardHook gkh = new globalKeyboardHook();
        Key ComboKey = Key.CapsLock;
        bool capsPress = false;
        public MainWindow()
        {
            InitializeComponent();

            var imgKeyboard = Wpf_comboKeyboard.Properties.Resources.keyboard80_word;
            MemoryStream memory = new MemoryStream();
            imgKeyboard.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            ImageSourceConverter converter = new ImageSourceConverter();
            ImageSource source = (ImageSource)converter.ConvertFrom(memory);
            img_keyboard.Source = source;

            var testImg = Wpf_comboKeyboard.Properties.Resources.mask_normal;
            memory = new MemoryStream();
            testImg.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            converter = new ImageSourceConverter();
            source = (ImageSource)converter.ConvertFrom(memory);
            img_selectMask.Source = source;
            select_002.Source = source;
            // select_002.Visibility = Visibility.Hidden;
            img_selectMask.Visibility = Visibility.Hidden;

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

            select_002.MouseDown += keyboardImageClick_MouseDown;
            gkh.KeyEvent += new globalKeyboardHook.KeyPressHandler(gkh_KeyEvent);
            
        }
        string infoFile = "";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //creat info file
            string MyDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            infoFile = MyDocumentsPath + "\\Combokey.txt";
            if (File.Exists(infoFile))
            {
                ReadTxtInfo();
            }
            else
            {
                WriteTxtInfo();
                ReadTxtInfo();
            }
            gkh.HookedKeys.Add(ComboKey);
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

            //gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
            //gkh.KeyUp += new KeyEventHandler(gkh_KeyUp);
        }
        void ReadTxtInfo()
        {
            string[] allString = System.IO.File.ReadAllLines(infoFile);
            string str = allString[0].Substring(9, allString[0].Length - 10);
            ComboKey = Name2Key[str];
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
            waitComboKey = true;
        }
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
            Dictionary<int, string> test_dic = new Dictionary<int, string>();
            List<string> test_string = new List<string>();
            string[] test_ar = new string[1000000];
            for (int i = 0; i < 1000000; i++)
            {
                test_dic.Add(i, i.ToString());
                test_string.Add(i.ToString());
                test_string[i] = i.ToString();
            }

            var a = test_dic[5000];

            for (int i = 0; i < 1000000; i++)
            {
                if (i == 5000)
                {
                    var b = test_string[i];
                }
            }

            test_string.Find(X => X == 5000.ToString());


        }
        private void keyboardImageClick_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string Name = ((Grid)sender).Name;
            Console.WriteLine(Name);
            if (waitComboKey)
            {
                ComboKey = Name2Key[Name];
                if (!gkh.HookedKeys.Exists(x => x == ComboKey))
                    gkh.HookedKeys.Add(ComboKey);

                waitComboKey = false;
                tb_comboKey.Text = Name;
            }


            //   select_002.Margin = ((Grid)sender).Margin;
        }

        void gkh_KeyEvent(KeyArgs e)
        {
            if (e.Key == ComboKey)
            {
                capsPress = true;
                Console.WriteLine("combo press");
                e.Handled = true;
            }
        }
        void gkh_KeyDown(KeyArgs e)
        {
            if (e.Key == ComboKey)
            {
                capsPress = true;
                Console.WriteLine("combo press");
                e.Handled = true;
            }
        }
        void gkh_KeyUp(KeyArgs e)
        {

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

        bool waitComboKey = false;


    }
}
