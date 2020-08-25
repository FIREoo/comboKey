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
using System.Runtime.InteropServices;

namespace Wpf_comboKeyboard
{
    public partial class MainWindow : Window
    {
        //[DllImport("user32.dll")]
        //static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);//用來打字的

        //const uint KEYEVENTF_KEYUP = 0x0002;
        /*
        void downKey(Keys _key)
        {
            keybd_event(KeysToVK(_key), 0, 0, 0);
        }
        void upKey(Keys _key)
        {
            keybd_event(KeysToVK(_key), 0, KEYEVENTF_KEYUP, 0);
        }
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

        Keys findKeyInImage(Point p)
        {
            if (p.Y > 0 && p.Y < 74)
            {//F1~
                if (p.X < 77)
                    return Keys.Escape;
                else if (p.X > 153 && p.X < 229)
                    return Keys.F1;
                else if (p.X > 229 && p.X < 305)
                    return Keys.F2;
                else if (p.X > 305 && p.X < 381)
                    return Keys.F3;
                else if (p.X > 381 && p.X < 457)
                    return Keys.F4;
                else if (p.X > 457 && p.X < 571)
                    return Keys.F5;
                else if (p.X > 571 && p.X < 647)
                    return Keys.F6;
                else if (p.X > 647 && p.X < 723)
                    return Keys.F7;
                else if (p.X > 723 && p.X < 798)
                    return Keys.F8;
                else if (p.X > 838 && p.X < 913)
                    return Keys.F9;
                else if (p.X > 913 && p.X < 989)
                    return Keys.F10;
                else if (p.X > 989 && p.X < 1065)
                    return Keys.F11;
                else if (p.X > 1065 && p.X < 1142)
                    return Keys.F12;
            }
            else if (p.Y > 74 && p.Y < 98) //none 
            { }
            else if (p.Y > 98 && p.Y < 172)
            {//1 2 3~
                if (p.X > 0 && p.X < 77)
                    return Keys.Oemtilde;
                else if (p.X > 77 && p.X < 153)
                    return Keys.D1;
                else if (p.X > 153 && p.X < 229)
                    return Keys.D2;
                else if (p.X > 229 && p.X < 305)
                    return Keys.D3;
                else if (p.X > 305 && p.X < 381)
                    return Keys.D4;
                else if (p.X > 381 && p.X < 457)
                    return Keys.D5;
                else if (p.X > 457 && p.X < 533)
                    return Keys.D6;
                else if (p.X > 533 && p.X < 609)
                    return Keys.D7;
                else if (p.X > 609 && p.X < 685)
                    return Keys.D8;
                else if (p.X > 685 && p.X < 761)
                    return Keys.D9;
                else if (p.X > 761 && p.X < 837)
                    return Keys.D0;
                else if (p.X > 837 && p.X < 913)
                    return Keys.OemMinus;
                else if (p.X > 913 && p.X < 989)
                    return Keys.Oemplus;
                else if (p.X > 989 && p.X < 1142)
                    return Keys.Back;
            }
            else if (p.Y > 172 && p.Y < 248)
            {//QWE~
                if (p.X < 115)
                    return Keys.Tab;
                else if (p.X < 191)
                    return Keys.Q;
                else if (p.X < 267)
                    return Keys.W;
                else if (p.X < 343)
                    return Keys.E;
                else if (p.X < 419)
                    return Keys.R;
                else if (p.X < 495)
                    return Keys.T;
                else if (p.X < 571)
                    return Keys.Y;
                else if (p.X < 647)
                    return Keys.U;
                else if (p.X < 723)
                    return Keys.I;
                else if (p.X < 799)
                    return Keys.O;
                else if (p.X < 875)
                    return Keys.P;
                else if (p.X < 951)
                    return Keys.OemOpenBrackets;// [
                else if (p.X < 1027)
                    return Keys.OemCloseBrackets;// ] Oem6
                else if (p.X < 1142)
                    return Keys.OemPipe;// |  Oem5
            }
            else if (p.Y > 248 && p.Y < 324)
            {//ASD~
                if (p.X < 134)
                    return Keys.CapsLock;
                else if (p.X < 210)
                    return Keys.A;
                else if (p.X < 286)
                    return Keys.S;
                else if (p.X < 362)
                    return Keys.D;
                else if (p.X < 438)
                    return Keys.F;
                else if (p.X < 514)
                    return Keys.G;
                else if (p.X < 590)
                    return Keys.H;
                else if (p.X < 666)
                    return Keys.J;
                else if (p.X < 742)
                    return Keys.K;
                else if (p.X < 818)
                    return Keys.L;
                else if (p.X < 894)
                    return Keys.OemSemicolon;// ; oem1
                else if (p.X < 969)
                    return Keys.OemQuotes;// ' oem7
                else if (p.X < 1142)
                    return Keys.Enter;// Keys.return
            }
            else if (p.Y > 324 && p.Y < 400)
            {//ZXC~
                if (p.X < 172)
                    return Keys.LShiftKey;
                else if (p.X < 248)
                    return Keys.Z;
                else if (p.X < 324)
                    return Keys.X;
                else if (p.X < 400)
                    return Keys.C;
                else if (p.X < 476)
                    return Keys.V;
                else if (p.X < 552)
                    return Keys.B;
                else if (p.X < 628)
                    return Keys.N;
                else if (p.X < 704)
                    return Keys.M;
                else if (p.X < 780)
                    return Keys.Oemcomma;
                else if (p.X < 856)
                    return Keys.OemPeriod;
                else if (p.X < 932)
                    return Keys.OemQuestion;
                else if (p.X < 1142)
                    return Keys.RShiftKey;
            }
            else if (p.Y > 400 && p.Y < 477)
            {//Ctrl~
                if (p.X < 100)
                    return Keys.LControlKey;
                else if (p.X < 198)
                    return Keys.LWin;
                else if (p.X < 297)
                    return Keys.LMenu;//Left Alt
                else if (p.X < 746)
                    return Keys.Space;
                else if (p.X < 844)
                    return Keys.RMenu;
                else if (p.X < 943)
                    return Keys.RWin;
                else if (p.X < 1042)
                    return Keys.Apps;
                else if (p.X < 1142)
                    return Keys.RControlKey;
            }
            return Keys.OemMinus;
        }
        Keys NameToKeys(string name)
        {
            switch (name)
            {
                case "Esc":
                    return Keys.Escape;
                case "F1":
                    return Keys.F1;
                case "F2":
                    return Keys.F2;
                case "F3":
                    return Keys.F3;
                case "F4":
                    return Keys.F4;
                case "F5":
                    return Keys.F5;
                case "F6":
                    return Keys.F6;
                case "F7":
                    return Keys.F7;
                case "F8":
                    return Keys.F8;
                case "F9":
                    return Keys.F9;
                case "F10":
                    return Keys.F10;
                case "F11":
                    return Keys.F11;
                case "F12":
                    return Keys.F12;
                case "Tilde":
                    return Keys.Oemtilde;
                case "num1":
                    return Keys.D1;
                case "num2":
                    return Keys.D2;
                case "num3":
                    return Keys.D3;
                case "num4":
                    return Keys.D4;
                case "num5":
                    return Keys.D5;
                case "num6":
                    return Keys.D6;
                case "num7":
                    return Keys.D7;
                case "num8":
                    return Keys.D8;
                case "num9":
                    return Keys.D9;
                case "num0":
                    return Keys.D0;
                case "Dash":
                    return Keys.OemMinus;
                case "Equal":
                    return Keys.Oemplus;
                case "BackSpace":
                    return Keys.Back;
                case "Tab":
                    return Keys.Tab;
                case "Q":
                    return Keys.Q;
                case "W":
                    return Keys.W;
                case "E":
                    return Keys.E;
                case "R":
                    return Keys.R;
                case "T":
                    return Keys.T;
                case "Y":
                    return Keys.Y;
                case "U":
                    return Keys.U;
                case "I":
                    return Keys.I;
                case "O":
                    return Keys.O;
                case "P":
                    return Keys.P;
                case "OpenBracket":
                    return Keys.OemOpenBrackets;// [
                case "CloseBracket":
                    return Keys.OemCloseBrackets;// ] Oem6
                case "BackSlash":
                    return Keys.OemPipe;// |  Oem5
                case "CapsLock":
                    return Keys.CapsLock;
                case "A":
                    return Keys.A;
                case "S":
                    return Keys.S;
                case "D":
                    return Keys.D;
                case "F":
                    return Keys.F;
                case "G":
                    return Keys.G;
                case "H":
                    return Keys.H;
                case "J":
                    return Keys.J;
                case "K":
                    return Keys.K;
                case "L":
                    return Keys.L;
                case "Semicolon":
                    return Keys.OemSemicolon;// ; oem1
                case "SingleQuote":
                    return Keys.OemQuotes;// ' oem7
                case "Enter":
                    return Keys.Enter;
                case "LShift":
                    return Keys.LShiftKey;
                case "Z":
                    return Keys.Z;
                case "X":
                    return Keys.X;
                case "C":
                    return Keys.C;
                case "V":
                    return Keys.V;
                case "B":
                    return Keys.B;
                case "N":
                    return Keys.N;
                case "M":
                    return Keys.M;
                case "Comma":
                    return Keys.Oemcomma;
                case "Period":
                    return Keys.OemPeriod;
                case "Slash":
                    return Keys.OemQuestion;
                case "RShift":
                    return Keys.RShiftKey;
                case "LCrtl":
                    return Keys.LControlKey;
                case "LWin":
                    return Keys.LWin;
                case "LAlt":
                    return Keys.LMenu;
                case "Space":
                    return Keys.Space;
                case "RAlt":
                    return Keys.RMenu;
                case "RWin":
                    return Keys.RWin;
                case "Menu":
                    return Keys.Apps;
                case "RCtrl":
                    return Keys.RControlKey;

                default:
                    return Keys.CapsLock;
            }
        }
        */
        void keyboardImageGridIni()
        {
            grid_001.MouseDown += keyboardImageClick_MouseDown;
            grid_001.Name = "Esc";
            grid_002.MouseDown += keyboardImageClick_MouseDown;
            grid_002.Name = "F1";
            grid_003.MouseDown += keyboardImageClick_MouseDown;
            grid_003.Name = "F2";
            grid_004.MouseDown += keyboardImageClick_MouseDown;
            grid_004.Name = "F3";
            grid_005.MouseDown += keyboardImageClick_MouseDown;
            grid_005.Name = "F4";
            grid_006.MouseDown += keyboardImageClick_MouseDown;
            grid_006.Name = "F5";
            grid_007.MouseDown += keyboardImageClick_MouseDown;
            grid_007.Name = "F6";
            grid_008.MouseDown += keyboardImageClick_MouseDown;
            grid_008.Name = "F7";
            grid_009.MouseDown += keyboardImageClick_MouseDown;
            grid_009.Name = "F8";
            grid_010.MouseDown += keyboardImageClick_MouseDown;
            grid_010.Name = "F9";
            grid_011.MouseDown += keyboardImageClick_MouseDown;
            grid_011.Name = "F10";
            grid_012.MouseDown += keyboardImageClick_MouseDown;
            grid_012.Name = "F11";
            grid_013.MouseDown += keyboardImageClick_MouseDown;
            grid_013.Name = "F12";

            grid_014.MouseDown += keyboardImageClick_MouseDown;
            grid_014.Name = "Tilde";
            grid_015.MouseDown += keyboardImageClick_MouseDown;
            grid_015.Name = "num1";
            grid_016.MouseDown += keyboardImageClick_MouseDown;
            grid_016.Name = "num2";
            grid_017.MouseDown += keyboardImageClick_MouseDown;
            grid_017.Name = "num3";
            grid_018.MouseDown += keyboardImageClick_MouseDown;
            grid_018.Name = "num4";
            grid_019.MouseDown += keyboardImageClick_MouseDown;
            grid_019.Name = "num5";
            grid_020.MouseDown += keyboardImageClick_MouseDown;
            grid_020.Name = "num6";
            grid_021.MouseDown += keyboardImageClick_MouseDown;
            grid_021.Name = "num7";
            grid_022.MouseDown += keyboardImageClick_MouseDown;
            grid_022.Name = "num8";
            grid_023.MouseDown += keyboardImageClick_MouseDown;
            grid_023.Name = "num9";
            grid_024.MouseDown += keyboardImageClick_MouseDown;
            grid_024.Name = "num0";
            grid_025.MouseDown += keyboardImageClick_MouseDown;
            grid_025.Name = "Dash";
            grid_026.MouseDown += keyboardImageClick_MouseDown;
            grid_026.Name = "Equal";
            grid_027.MouseDown += keyboardImageClick_MouseDown;
            grid_027.Name = "BackSpace";

            grid_028.MouseDown += keyboardImageClick_MouseDown;
            grid_028.Name = "Tab";
            grid_029.MouseDown += keyboardImageClick_MouseDown;
            grid_029.Name = "Q";
            grid_030.MouseDown += keyboardImageClick_MouseDown;
            grid_030.Name = "W";
            grid_031.MouseDown += keyboardImageClick_MouseDown;
            grid_031.Name = "E";
            grid_032.MouseDown += keyboardImageClick_MouseDown;
            grid_032.Name = "R";
            grid_033.MouseDown += keyboardImageClick_MouseDown;
            grid_033.Name = "T";
            grid_034.MouseDown += keyboardImageClick_MouseDown;
            grid_034.Name = "Y";
            grid_035.MouseDown += keyboardImageClick_MouseDown;
            grid_035.Name = "U";
            grid_036.MouseDown += keyboardImageClick_MouseDown;
            grid_036.Name = "I";
            grid_037.MouseDown += keyboardImageClick_MouseDown;
            grid_037.Name = "O";
            grid_038.MouseDown += keyboardImageClick_MouseDown;
            grid_038.Name = "P";
            grid_039.MouseDown += keyboardImageClick_MouseDown;
            grid_039.Name = "OpenBracket";
            grid_040.MouseDown += keyboardImageClick_MouseDown;
            grid_040.Name = "CloseBracket";
            grid_041.MouseDown += keyboardImageClick_MouseDown;
            grid_041.Name = "BackSlash";

            grid_042.MouseDown += keyboardImageClick_MouseDown;
            grid_042.Name = "CapsLock";
            grid_043.MouseDown += keyboardImageClick_MouseDown;
            grid_043.Name = "A";
            grid_044.MouseDown += keyboardImageClick_MouseDown;
            grid_044.Name = "S";
            grid_045.MouseDown += keyboardImageClick_MouseDown;
            grid_045.Name = "D";
            grid_046.MouseDown += keyboardImageClick_MouseDown;
            grid_046.Name = "F";
            grid_047.MouseDown += keyboardImageClick_MouseDown;
            grid_047.Name = "G";
            grid_048.MouseDown += keyboardImageClick_MouseDown;
            grid_048.Name = "H";
            grid_049.MouseDown += keyboardImageClick_MouseDown;
            grid_049.Name = "J";
            grid_050.MouseDown += keyboardImageClick_MouseDown;
            grid_050.Name = "K";
            grid_051.MouseDown += keyboardImageClick_MouseDown;
            grid_051.Name = "L";
            grid_052.MouseDown += keyboardImageClick_MouseDown;
            grid_052.Name = "Semicolon";
            grid_053.MouseDown += keyboardImageClick_MouseDown;
            grid_053.Name = "SingleQuote";
            grid_054.MouseDown += keyboardImageClick_MouseDown;
            grid_054.Name = "Enter";

            grid_055.MouseDown += keyboardImageClick_MouseDown;
            grid_055.Name = "LShift";
            grid_056.MouseDown += keyboardImageClick_MouseDown;
            grid_056.Name = "Z";
            grid_057.MouseDown += keyboardImageClick_MouseDown;
            grid_057.Name = "X";
            grid_058.MouseDown += keyboardImageClick_MouseDown;
            grid_058.Name = "C";
            grid_059.MouseDown += keyboardImageClick_MouseDown;
            grid_059.Name = "V";
            grid_060.MouseDown += keyboardImageClick_MouseDown;
            grid_060.Name = "B";
            grid_061.MouseDown += keyboardImageClick_MouseDown;
            grid_061.Name = "N";
            grid_062.MouseDown += keyboardImageClick_MouseDown;
            grid_062.Name = "M";
            grid_063.MouseDown += keyboardImageClick_MouseDown;
            grid_063.Name = "Comma";
            grid_064.MouseDown += keyboardImageClick_MouseDown;
            grid_064.Name = "Period";
            grid_065.MouseDown += keyboardImageClick_MouseDown;
            grid_065.Name = "Slash";
            grid_066.MouseDown += keyboardImageClick_MouseDown;
            grid_066.Name = "RShift";

            grid_067.MouseDown += keyboardImageClick_MouseDown;
            grid_067.Name = "LCrtl";
            grid_068.MouseDown += keyboardImageClick_MouseDown;
            grid_068.Name = "LWin";
            grid_069.MouseDown += keyboardImageClick_MouseDown;
            grid_069.Name = "LAlt";
            grid_070.MouseDown += keyboardImageClick_MouseDown;
            grid_070.Name = "Space";
            grid_071.MouseDown += keyboardImageClick_MouseDown;
            grid_071.Name = "RAlt";
            grid_072.MouseDown += keyboardImageClick_MouseDown;
            grid_072.Name = "RWin";
            grid_073.MouseDown += keyboardImageClick_MouseDown;
            grid_073.Name = "Menu";
            grid_074.MouseDown += keyboardImageClick_MouseDown;
            grid_074.Name = "RCtrl";
        }
        Dictionary<string, Key> Name2Key = new Dictionary<string, Key>();
        Dictionary<Key, string> Key2Name = new Dictionary<Key, string>();
        void keyNameIni()
        {
            Name2Key.Add("Esc", Key.Escape); Name2Key.Add("esc", Key.Escape);
            Name2Key.Add("F1", Key.F1);
            Name2Key.Add("F2", Key.F2);
            Name2Key.Add("F3", Key.F3);
            Name2Key.Add("F4", Key.F4);
            Name2Key.Add("F5", Key.F5);
            Name2Key.Add("F6", Key.F6);
            Name2Key.Add("F7", Key.F7);
            Name2Key.Add("F8", Key.F8);
            Name2Key.Add("F9", Key.F9);
            Name2Key.Add("F10", Key.F10);
            Name2Key.Add("F11", Key.F11);
            Name2Key.Add("F12", Key.F12);
            Name2Key.Add("Tilde", Key.OemTilde); Name2Key.Add("`", Key.OemTilde);
            Name2Key.Add("num1", Key.D1);
            Name2Key.Add("num2", Key.D2);
            Name2Key.Add("num3", Key.D3);
            Name2Key.Add("num4", Key.D4);
            Name2Key.Add("num5", Key.D5);
            Name2Key.Add("num6", Key.D6);
            Name2Key.Add("num7", Key.D7);
            Name2Key.Add("num8", Key.D8);
            Name2Key.Add("num9", Key.D9);
            Name2Key.Add("num0", Key.D0);
            Name2Key.Add("Dash", Key.OemMinus); Name2Key.Add("-", Key.OemMinus);
            Name2Key.Add("Equal", Key.OemPlus); Name2Key.Add("=", Key.OemPlus);
            Name2Key.Add("BackSpace", Key.Back);
            Name2Key.Add("Tab", Key.Tab); Name2Key.Add("tab", Key.Tab);
            Name2Key.Add("Q", Key.Q);
            Name2Key.Add("W", Key.W);
            Name2Key.Add("E", Key.E);
            Name2Key.Add("R", Key.R);
            Name2Key.Add("T", Key.T);
            Name2Key.Add("Y", Key.Y);
            Name2Key.Add("U", Key.U);
            Name2Key.Add("I", Key.I);
            Name2Key.Add("O", Key.O);
            Name2Key.Add("P", Key.P);
            Name2Key.Add("OpenBracket", Key.OemOpenBrackets); Name2Key.Add("[", Key.OemOpenBrackets);
            Name2Key.Add("CloseBracket", Key.OemCloseBrackets); Name2Key.Add("]", Key.OemCloseBrackets);// ] Oem6
            Name2Key.Add("BackSlash", Key.OemPipe); Name2Key.Add("\\", Key.OemPipe);// |  Oem5
            Name2Key.Add("CapsLock", Key.CapsLock); Name2Key.Add("Caps", Key.CapsLock);
            Name2Key.Add("A", Key.A);
            Name2Key.Add("S", Key.S);
            Name2Key.Add("D", Key.D);
            Name2Key.Add("F", Key.F);
            Name2Key.Add("G", Key.G);
            Name2Key.Add("H", Key.H);
            Name2Key.Add("J", Key.J);
            Name2Key.Add("K", Key.K);
            Name2Key.Add("L", Key.L);
            Name2Key.Add("Semicolon", Key.OemSemicolon); Name2Key.Add(";", Key.OemSemicolon);// ; oem1
            Name2Key.Add("SingleQuote", Key.OemQuotes); Name2Key.Add("'", Key.OemQuotes);// ' oem7
            Name2Key.Add("Enter", Key.Enter);
            Name2Key.Add("LShift", Key.LeftShift); Name2Key.Add("LeftShift", Key.LeftShift);
            Name2Key.Add("Z", Key.Z);
            Name2Key.Add("X", Key.X);
            Name2Key.Add("C", Key.C);
            Name2Key.Add("V", Key.V);
            Name2Key.Add("B", Key.B);
            Name2Key.Add("N", Key.N);
            Name2Key.Add("M", Key.M);
            Name2Key.Add("Comma", Key.OemComma); Name2Key.Add(",", Key.OemComma);
            Name2Key.Add("Period", Key.OemPeriod); Name2Key.Add(".", Key.OemPeriod);
            Name2Key.Add("Slash", Key.OemQuestion); Name2Key.Add("/", Key.OemQuestion);
            Name2Key.Add("RShift", Key.RightShift); Name2Key.Add("RightShift", Key.RightShift);
            Name2Key.Add("LCrtl", Key.LeftCtrl); Name2Key.Add("LeftCtrl", Key.LeftCtrl);
            Name2Key.Add("LWin", Key.LWin);
            Name2Key.Add("LAlt", Key.LeftAlt);
            Name2Key.Add("Space", Key.Space);
            Name2Key.Add("RAlt", Key.RightAlt);
            Name2Key.Add("RWin", Key.RWin);
            Name2Key.Add("Menu", Key.Apps);
            Name2Key.Add("RCtrl", Key.RightCtrl);
            Name2Key.Add("Insert", Key.Insert); Name2Key.Add("Ins", Key.Insert);
            Name2Key.Add("Home", Key.Home); Name2Key.Add("Hme", Key.Home);
            Name2Key.Add("End", Key.End); 
            Name2Key.Add("Delete", Key.Delete); Name2Key.Add("Del", Key.Delete);
            Name2Key.Add("PageUp", Key.PageUp); Name2Key.Add("Pgu", Key.PageUp);
            Name2Key.Add("PageDown", Key.PageDown); Name2Key.Add("Pgd", Key.PageDown);
            Name2Key.Add( "Up",Key.Up); Name2Key.Add("ArrowUp", Key.Up);
            Name2Key.Add( "Down",Key.Down); Name2Key.Add("ArrowDown", Key.Down);
            Name2Key.Add( "Left",Key.Left); Name2Key.Add("ArrowLeft", Key.Left);
            Name2Key.Add( "Right",Key.Right); Name2Key.Add("ArrowRight", Key.Right);


            Key2Name.Add(Key.Escape, "Esc");
            Key2Name.Add(Key.F1, "F1");
            Key2Name.Add(Key.F2, "F2");
            Key2Name.Add(Key.F3, "F3");
            Key2Name.Add(Key.F4, "F4");
            Key2Name.Add(Key.F5, "F5");
            Key2Name.Add(Key.F6, "F6");
            Key2Name.Add(Key.F7, "F7");
            Key2Name.Add(Key.F8, "F8");
            Key2Name.Add(Key.F9, "F9");
            Key2Name.Add(Key.F10, "F10");
            Key2Name.Add(Key.F11, "F11");
            Key2Name.Add(Key.F12, "F12");
            Key2Name.Add(Key.OemTilde, "Tilde");
            Key2Name.Add(Key.D1, "num1");
            Key2Name.Add(Key.D2, "num2");
            Key2Name.Add(Key.D3, "num3");
            Key2Name.Add(Key.D4, "num4");
            Key2Name.Add(Key.D5, "num5");
            Key2Name.Add(Key.D6, "num6");
            Key2Name.Add(Key.D7, "num7");
            Key2Name.Add(Key.D8, "num8");
            Key2Name.Add(Key.D9, "num9");
            Key2Name.Add(Key.D0, "num0");
            Key2Name.Add(Key.OemMinus, "Dash");
            Key2Name.Add(Key.OemPlus, "Equal");
            Key2Name.Add(Key.Back, "BackSpace");
            Key2Name.Add(Key.Tab, "Tab");
            Key2Name.Add(Key.Q, "Q");
            Key2Name.Add(Key.W, "W");
            Key2Name.Add(Key.E, "E");
            Key2Name.Add(Key.R, "R");
            Key2Name.Add(Key.T, "T");
            Key2Name.Add(Key.Y, "Y");
            Key2Name.Add(Key.U, "U");
            Key2Name.Add(Key.I, "I");
            Key2Name.Add(Key.O, "O");
            Key2Name.Add(Key.P, "P");
            Key2Name.Add(Key.OemOpenBrackets, "OpenBracket");// [
            Key2Name.Add(Key.OemCloseBrackets, "CloseBracket");// ] Oem6
            Key2Name.Add(Key.OemPipe, "BackSlash");// |  Oem5
            Key2Name.Add(Key.CapsLock, "CapsLock");
            Key2Name.Add(Key.A, "A");
            Key2Name.Add(Key.S, "S");
            Key2Name.Add(Key.D, "D");
            Key2Name.Add(Key.F, "F");
            Key2Name.Add(Key.G, "G");
            Key2Name.Add(Key.H, "H");
            Key2Name.Add(Key.J, "J");
            Key2Name.Add(Key.K, "K");
            Key2Name.Add(Key.L, "L");
            Key2Name.Add(Key.OemSemicolon, "Semicolon");//;  oem1
            Key2Name.Add(Key.OemQuotes, "SingleQuote");// ' oem7
            Key2Name.Add(Key.Enter, "Enter");
            Key2Name.Add(Key.LeftShift, "LShift");
            Key2Name.Add(Key.Z, "Z");
            Key2Name.Add(Key.X, "X");
            Key2Name.Add(Key.C, "C");
            Key2Name.Add(Key.V, "V");
            Key2Name.Add(Key.B, "B");
            Key2Name.Add(Key.N, "N");
            Key2Name.Add(Key.M, "M");
            Key2Name.Add(Key.OemComma, "Comma");// ,
            Key2Name.Add(Key.OemPeriod, "Period");// .
            Key2Name.Add(Key.OemQuestion, "Slash");// /
            Key2Name.Add(Key.RightShift, "RShift");
            Key2Name.Add(Key.LeftCtrl, "LCrtl");
            Key2Name.Add(Key.LWin, "LWin");
            Key2Name.Add(Key.LeftAlt, "LAlt");
            Key2Name.Add(Key.Space, "Space");
            Key2Name.Add(Key.RightAlt, "RAlt");
            Key2Name.Add(Key.RWin, "RWin");
            Key2Name.Add(Key.Apps, "Menu");
            Key2Name.Add(Key.RightCtrl, "RCtrl");

            Key2Name.Add(Key.Insert, "Insert");
            Key2Name.Add(Key.Home, "Home");
            Key2Name.Add(Key.End, "End");
            Key2Name.Add(Key.Delete, "Delete");
            Key2Name.Add(Key.PageUp, "PageUp");
            Key2Name.Add(Key.PageDown, "PageDown");

            Key2Name.Add(Key.Up, "Up");
            Key2Name.Add(Key.Down, "Down");
            Key2Name.Add(Key.Left, "Left");
            Key2Name.Add(Key.Right, "Right");
        }

    }
}
