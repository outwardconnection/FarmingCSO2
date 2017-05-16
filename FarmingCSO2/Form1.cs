using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Timers;
using System.Windows.Input;
using System.Diagnostics;
using System.Media;
using System.Resources;

namespace FarmingCSO2
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);  //以視窗名稱獲得窗口句柄

        [DllImport("kernel32.dll")]
        private static extern uint GetTickCount();

        private bool BeginFarming { set; get; }
        private bool BeginClicks = false;
        private Color[] startButtonColor = new Color[4];
        private IntPtr CSO2Window { set; get; }
        public static int screenWidth { get { return Screen.PrimaryScreen.Bounds.Width; } }
        private static int screenHeight { get { return Screen.PrimaryScreen.Bounds.Height; } }

        HotKey farmingHotKey , clicksHotKey;
        private bool farmingSound = true;
        private bool farmingLow_key = false;
        private int mouseClicksDelayTime = 100;

        Setting settingForm;
        private const string soundPath = "sound.mp3";

        public Form1()
        {
            InitializeComponent();
            BeginFarming = false;

            settingForm = new Setting();
            settingForm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.settingsFormClosed);

            farmingHotKey = new HotKey(this.Handle,settingForm.FarmingHotKey,Keys.None);
            farmingHotKey.OnHotkey += new HotKey.HotkeyEventHandler(farmingOnHotKey);
            clicksHotKey = new HotKey(this.Handle, settingForm.ClicksHotKey , Keys.None);
            clicksHotKey.OnHotkey += new HotKey.HotkeyEventHandler(clicksOnHotKey);
            farmingSound = settingForm.FarmingSound;
            farmingLow_key = settingForm.FarmingLow_key;
            mouseClicksDelayTime = settingForm.ClicksDelayTime;


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Location = new System.Drawing.Point( (screenWidth - this.Width) / 2, (screenHeight - this.Height) / 2);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            //MessageBox.Show("螢幕解析度為 " + screenWidth.ToString() + "*" + screenHeight.ToString());
            /*
            if(!ImitateOperating.InitSucess)
            {
                ImitateOperating.InitWinIo();
            }
            */
            /*
            CSO2Window = FindWindow(null, "Counter-Strike Online 2 21377 TW EXT  LIVE  ");
            if(CSO2Window.ToInt32() == 0)
            {
                MessageBox.Show("not find cso2");
            }
            */
        }

        public static void run(object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("OK");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
            Thread.Sleep(5000);
            startButtonColor[0] = GetColor(1393,615);
            startButtonColor[1] = GetColor(1410,614);
            startButtonColor[2] = GetColor(1502, 614);
            startButtonColor[3] = GetColor(960, 540);

            string message = "第一個顏色:" + ColorTranslator.ToHtml(startButtonColor[0]) + "\n" 
                + "第二個顏色:" + ColorTranslator.ToHtml(startButtonColor[1]) + "\n"
                + "第三個顏色:" + ColorTranslator.ToHtml(startButtonColor[2]) + "\n"
                 + "第四個顏色:" + ColorTranslator.ToHtml(startButtonColor[3]) + "\n";

            MessageBox.Show(message);
            */

            Farming();
        }

        private void settingButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            settingForm.MyShow();
        }

        private void Farming()
        {
            if (BeginFarming)
            {
                TimerLabel.Hide();
                button2.Text = "開始掛機";
                BeginFarming = false;
            }
            else
            {
                if (true)
                {
                    button2.Text = "停止掛機";
                    BeginFarming = true;
                    TimerLabel.Show();
                }
                else
                {
                    MessageBox.Show("還沒啟用WinIO");
                }

            }

            for (int i = 0; i < 1000; i++)
            {
                if (i % 100 == 0)
                {
                    TimerLabel.Text = "還剩" + (10 - i / 100).ToString() + "秒";
                }
                if (!BeginFarming) { return; }
                Thread.Sleep(10);
                Application.DoEvents();
            }
            TimerLabel.Hide();

            if (farmingSound) { playSound(soundPath); }
            //開始掛機
            while (true)
            {
                ImitateOperating.MouseMoveTo(screenWidth / 2, screenHeight / 2);

                for (int i = 0; i < 200; i++)
                {
                    if (!BeginFarming)
                    {
                        if (farmingSound) { playSound(soundPath); }
                        return;
                    }
                    Thread.Sleep(10);
                    Application.DoEvents();
                }

                if(farmingLow_key)
                {
                    for (short j = 0; j < 5*20; j++)
                    {
                        if (!BeginFarming)
                        {
                            if (farmingSound) { playSound(soundPath); }
                            return;
                        }
                        Thread.Sleep(10);
                        Application.DoEvents();
                    }
                }
                else
                {
                    //瘋狂往右滑
                    for (short i = 0; i < 20; i++)
                    {
                        ImitateOperating.MouseMove(screenWidth / 2, 0);
                        for (short j = 0; j < 5; j++)
                        {
                            if (!BeginFarming)
                            {
                                if (farmingSound) { playSound(soundPath); }
                                return;
                            }
                            Thread.Sleep(10);
                            Application.DoEvents();
                        }

                    }
                }

                ImitateOperating.MouseMove(screenWidth, 0);

                for (int i = 0; i < 200; i++)
                {
                    if (!BeginFarming)
                    {
                        if (farmingSound) { playSound(soundPath); }
                        return;
                    }
                    Thread.Sleep(10);
                    Application.DoEvents();
                }

                ImitateOperating.MouseMove((-1 * screenWidth), 0);

                for (int i = 0; i < 200; i++)
                {
                    if (!BeginFarming)
                    {
                        if (farmingSound) { playSound(soundPath); }
                        return;
                    }
                    Thread.Sleep(10);
                    Application.DoEvents();
                }

                //按 返回大廳 鍵
                ImitateOperating.MouseMoveTo((int)((float)1096 / 1536 * screenWidth), (int)((float)802 / 864 * screenHeight));
                ImitateOperating.MouseLeftClick();
                for (int i = 0; i < 1000; i++)
                {
                    if (!BeginFarming)
                    {
                        if (farmingSound) { playSound(soundPath); }
                        return;
                    }
                    Thread.Sleep(10);
                    Application.DoEvents();
                }

                //按 開始/準備鍵
                ImitateOperating.MouseMoveTo((int)((float)1410 / 1536 * screenWidth), (int)((float)614 / 864 * screenHeight));

                for (int i = 0; i < 200; i++)
                {
                    if (!BeginFarming)
                    {
                        if (farmingSound) { playSound(soundPath); }
                        return;
                    }
                    Thread.Sleep(10);
                    Application.DoEvents();
                }
                ImitateOperating.MouseLeftClick();

                for (int i = 0; i < 1000; i++)
                {
                    if (!BeginFarming)
                    {
                        if (farmingSound) { playSound(soundPath); }
                        return;
                    }
                    Thread.Sleep(10);
                    Application.DoEvents();
                }
            }
        }

        private void MouseClicks(int delay = 100)
        {
            while(true)
            {
                ImitateOperating.MouseLeftClick();
                for(int i=0;i<delay;i++)
                {
                    Application.DoEvents();
                    if (!BeginClicks)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }
                    //Console.WriteLine((GetTickCount() - start).ToString());
            }
            
        }

        public void farmingOnHotKey(object sender, HotKey.HotKeyEventArgs e)
        {
            Farming();
        }

        public void clicksOnHotKey(object sender , HotKey.HotKeyEventArgs s)
        {
            if (BeginClicks)
            {
                BeginClicks = false;
            }
            else
            {
                BeginClicks = true;
            }

            MouseClicks(mouseClicksDelayTime);
        }

        public void settingsFormClosed(object sender, FormClosedEventArgs e)
        {
            farmingHotKey = new HotKey(this.Handle, settingForm.FarmingHotKey, Keys.None);
            clicksHotKey = new HotKey(this.Handle, settingForm.ClicksHotKey, Keys.None);
            farmingSound = settingForm.FarmingSound;
            farmingLow_key = settingForm.FarmingLow_key;
            mouseClicksDelayTime = settingForm.ClicksDelayTime;

            this.Show();
        }

        private void playSound(string path)
        {
            WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
            wplayer.URL = path;
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException();
                }
            }
            catch(FileNotFoundException)
            {
                MessageBox.Show("找不到sound.mp3，將無法播放音效", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            
            wplayer.controls.play();
        }


        private void Delay(uint ms)
        {
            uint start = GetTickCount();
            while (GetTickCount() - start < ms)
            {
                Application.DoEvents();
                //Console.WriteLine((GetTickCount() - start).ToString());
            }
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            BeginFarming = false;
            BeginClicks = false;
            ImitateOperating.Shutdown();
            farmingHotKey.Dispose();
            clicksHotKey.Dispose();
        }

        public Color GetColor(int x, int y)
        {
            IntPtr hdc = GetDC(CSO2Window);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(CSO2Window, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF), (int)(pixel & 0x0000FF00) >> 8, (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }  
    }

    public class ImitateOperating
    {
        public const int KBC_KEY_CMD = 0x64;
        public const int KBC_KEY_DATA = 0x60;
        public const int KEY_CMD_DATA = 0xD2;
        public const int MOUSE_CMD_DATA = 0xD3;

        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;

        public static byte[] MousePacketData;
        public static bool[] MouseButtons;

        public static string AppPath = Environment.CurrentDirectory;

        [DllImport("WinIO64.dll")]
        public static extern bool InitializeWinIo();
        [DllImport("WinIO64.dll")]
        public static extern bool GetPortVal(IntPtr wPortAddr, out int pdwPortVal, byte bSize);
        [DllImport("WinIO64.dll")]
        public static extern bool SetPortVal(uint wPortAddr, IntPtr dwPortVal, byte bSize);
        [DllImport("WinIO64.dll")]
        public static extern byte MapPhysToLin(byte pbPhysAddr, uint dwPhysSize, IntPtr PhysicalMemoryHandle);
        [DllImport("WinIO64.dll")]
        public static extern bool UnmapPhysicalMemory(IntPtr PhysicalMemoryHandle, byte pbLinAddr);
        [DllImport("WinIO64.dll")]
        public static extern bool GetPhysLong(IntPtr pbPhysAddr, byte pdwPhysVal);
        [DllImport("WinIO64.dll")]
        public static extern bool SetPhysLong(IntPtr pbPhysAddr, byte dwPhysVal);
        [DllImport("WinIO64.dll")]
        public static extern void ShutdownWinIo();

        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(uint Ucode, uint uMapType);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point pt);

        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);

        public enum MouseEventFlags
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            Wheel = 0x0800,
            Absolute = 0x8000
        }

        [DllImport("User32")]
        public extern static void mouse_event(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);

        private static bool initSucess = false;
        private static int[] moveConversion;
        public bool error = true;
        public static bool InitSucess { get { return initSucess; } }

        public static string GetPath(string file)
        {
            return Path.Combine(AppPath, file);
        }

        ~ImitateOperating()
        {
            if(initSucess)
            {
                ShutdownWinIo();
                Console.WriteLine("成功卸載WinIO");
            }
        }

        public static bool InitWinIo()
        {
            if (initSucess)
            {
                return true;
            }
            else
            {
                try
                {
                    //Console.WriteLine("1");
                    initSucess = InitializeWinIo();//加载WinIO
                }
                catch (System.Exception error)
                {//WinIO加载失败异常处理
                    MessageBox.Show(error.Message, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (initSucess)
            {
                //Console.WriteLine("2");

                KBCWait4IBE();

                //Console.WriteLine("3");
                MousePacketData = new byte[4];
                MouseButtons = new bool[3];
                MouseButtons[0] = false;
                MouseButtons[1] = false;
                MouseButtons[2] = false;
                moveConversion = new int[256];

                string filePath = GetPath("Mouse.ini");
                //Console.WriteLine("4");
                if (ConversionFromIni(filePath) == false)
                {
                    InitConversion();
                }

                //Console.WriteLine("成功啟用WinIO");

                return true;
            }
            else
            {
                //Console.WriteLine("5");
                MessageBox.Show("Load WinIO Failed!");
                initSucess = false;
                return false;
            }
        }

        public static void Shutdown()
        {
            if (initSucess)
            {
                ShutdownWinIo();
                Console.WriteLine("卸載WinIO");
            }
        }

        private static void KBCWait4IBE()
        {
            System.Threading.Thread.Sleep(10);
            int dwVal = 0;
            do
            {
                bool flag = GetPortVal((IntPtr)KBC_KEY_CMD, out dwVal, 1);
            }
            while ((dwVal & 0x2) > 0);
        }
        /*
        public static void KeyDown(ScanKeyCode scanKey)
        {
            foreach (uint scanCode in scanKey.MakeCode)
            {
                KBCWait4IBE();
                SetPortVal(KBC_KEY_CMD, (IntPtr)KEY_CMD_DATA, 1);
                KBCWait4IBE();
                SetPortVal(KBC_KEY_DATA, (IntPtr)scanCode, 1);
            }
        }
        */
        public static void  KeyDown(Keys vKeyCoad)
        {
            if (!initSucess) return;

            int btScancode = 0;
            btScancode = MapVirtualKey((uint)vKeyCoad, 0);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)0x60, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)btScancode, 1);
        }
        /*
        public static void KeyUp(ScanKeyCode scanKey)
        {
            foreach (uint scanCode in scanKey.BreakCode)
            {
                KBCWait4IBE();
                SetPortVal(KBC_KEY_CMD, (IntPtr)KEY_CMD_DATA, 1);
                KBCWait4IBE();
                SetPortVal(KBC_KEY_DATA, (IntPtr)scanCode, 1);
            }
        }
        */
        public static void KeyUp(Keys vKeyCoad)
        {
            if (!initSucess) return;

            int btScancode = 0;
            btScancode = MapVirtualKey((uint)vKeyCoad, 0);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)0x60, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);
        }

        /*
        public static void MouseLeftKeyDown()
        {
            MouseButtons[0] = true;
            InitMouseData(0, 0, 0);
            SendMouseData();
        }

        public static void MouseLeftKeyUp()
        {
            MouseButtons[0] = false;
            InitMouseData(0, 0, 0);
            SendMouseData();
        }

        public static void MouseRightKeyDown()
        {
            MouseButtons[1] = true;
            InitMouseData(0, 0, 0);
            SendMouseData();
        }

        public static void MouseRightKeyUp()
        {
            MouseButtons[1] = false;
            InitMouseData(0, 0, 0);
            SendMouseData();
        }

        public static void MouseMiddleKeyDown()
        {
            MouseButtons[2] = true;
            InitMouseData(0, 0, 0);
            SendMouseData();
        }

        public static void MouseMiddleKeyUp()
        {
            MouseButtons[2] = false;
            InitMouseData(0, 0, 0);
            SendMouseData();
        }

        public static void MouseMoveTo(int x, int y)
        {
            MouseMoveTo(x, y, 0, 0);
        }

        public static void MouseMove(int cx, int cy)
        {
            InitMouseData(cx, cy, 0);
            SendMouseData();
        }

        public static void MouseWheel(int cz)
        {
            InitMouseData(0, 0, cz);
            SendMouseData();
        }

        public static void InitMouseData(int cx, int cy, int cz)
        {
            byte[] byteBit = { 1, 2, 4, 8, 16, 32 };
            MousePacketData[0] = byteBit[3];
            if (MouseButtons[0]) { MousePacketData[0] = (byte)(MousePacketData[0] | byteBit[0]); }
            if (MouseButtons[1]) { MousePacketData[0] = (byte)(MousePacketData[0] | byteBit[1]); }
            if (MouseButtons[2]) { MousePacketData[0] = (byte)(MousePacketData[0] | byteBit[2]); }
            if (cx < 0) { MousePacketData[0] = (byte)(MousePacketData[0] | byteBit[4]); }
            if (cy < 0) { MousePacketData[0] = (byte)(MousePacketData[0] | byteBit[5]); }
            MousePacketData[1] = (byte)(cx & 0xFF);
            MousePacketData[2] = (byte)(cy & 0xFF);
            MousePacketData[3] = (byte)(cz & 0xF);
        }

        public static void MouseMoveTo(int x, int y, int maxMove, int interval)

        {

            Point p = new Point();

            int cX, cY, n;
            bool inLeft, inTop;

            if (maxMove <= 0) { maxMove = 0x7FFFFFFF; }

            p.X = GetSystemMetrics(SM_CXSCREEN) - 1;
            if (x > p.X) { x = p.X; }
            p.Y = GetSystemMetrics(SM_CYSCREEN) - 1;
            if (y > p.Y) { y = p.Y; }
            GetCursorPos(out p);
            inLeft = p.X < x ? true : false; inTop = p.Y < y ? true : false;
            while (!(
                (((p.X < x) != inLeft) || (p.X == x)) &&
                (((p.Y < y) != inTop) || (p.Y == y))))
            {
                n = x - p.X;
                cX = fConversion(n, maxMove);
                p.X = p.X + n;
                //
                n = y - p.Y;
                cY = -fConversion(n, maxMove);
                p.Y = p.Y + n;
                //
                MouseMove(cX, cY);

                if (interval != 0) { System.Threading.Thread.Sleep(interval); };

            }


        }
        */
        private static string ReadIniValue(string section, string key, string filePath)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", temp, 255, filePath);
            return temp.ToString();
        }
        
        private static void InitConversion()
        {
            for (int i = 0; i <= 6; i++)
            {
                moveConversion[i] = i;
            }
            for (int i = 7; i <= 255; i++)
            {
                moveConversion[i] = i * 2;
            }
        }
        
        private static bool ConversionFromIni(string iniFilePath)
        {

            if (File.Exists(iniFilePath))
            {
                try
                {
                    moveConversion[0] = 0;
                    for (int i = 1; i <= 255; i++)
                    {
                        moveConversion[i] = Convert.ToInt32(ReadIniValue("Conversion", i.ToString(), iniFilePath));
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /*
        private static int fConversion(int dis, int MaxMove)
        {
            int fi, fn;
            int result = 0;
            if (dis == 0) { return result; }
            fn = Math.Abs(dis);
            if (fn > MaxMove) { fn = MaxMove; }
            for (fi = 1; fi <= 255; fi++)
            {
                if (moveConversion[fi] > fn) { break; }
                if (moveConversion[fi] != 0) { result = fi; }
            }
            if (result == 0)
            {
                result = 1;
                fn = 1;
            }
            else { fn = moveConversion[result]; }

            if (dis < 0)

            {
                dis = -fn;
                result = -result;
            }
            else { dis = fn; }

            return result;

        }

        
        private static void SendMouseData()
        {
            foreach (byte data in MousePacketData)
            {
                KBCWait4IBE();
                SetPortVal(KBC_KEY_CMD, (IntPtr)MOUSE_CMD_DATA, 1);
                KBCWait4IBE();
                SetPortVal(KBC_KEY_DATA, (IntPtr)data, 1);
            }
        }
        */
        //------mouse_event版本
        public static void MouseLeftDown()
        {          
            mouse_event((int)(MouseEventFlags.LeftDown | MouseEventFlags.Absolute), 0, 0, 0, IntPtr.Zero);
        }

        public static void MouseLeftUp()
        {
            mouse_event((int)(MouseEventFlags.LeftUp | MouseEventFlags.Absolute), 0, 0, 0, IntPtr.Zero);
        }

        public static void MouseLeftClick()
        {
            mouse_event((int)(MouseEventFlags.LeftDown | MouseEventFlags.Absolute), 0, 0, 0, IntPtr.Zero);
            Thread.Sleep(10);
            mouse_event((int)(MouseEventFlags.LeftUp | MouseEventFlags.Absolute), 0, 0, 0, IntPtr.Zero);
        }

        public static void MouseRighttDown()
        {
            mouse_event((int)(MouseEventFlags.RightDown | MouseEventFlags.Absolute), 0, 0, 0, IntPtr.Zero);
        }

        public static void MouseRightUp()
        {
            mouse_event((int)(MouseEventFlags.RightUp | MouseEventFlags.Absolute), 0, 0, 0, IntPtr.Zero);
        }

        public static void MouseRightClick()
        {
            mouse_event((int)(MouseEventFlags.RightDown | MouseEventFlags.Absolute), 0, 0, 0, IntPtr.Zero);
            Thread.Sleep(10);
            mouse_event((int)(MouseEventFlags.RightUp | MouseEventFlags.Absolute), 0, 0, 0, IntPtr.Zero);
        }

        public static void MouseMoveTo(int x, int y)
        {
            SetCursorPos(x,y);
        }

        public static void MouseMoveTo(Point moveTo)
        {
            SetCursorPos( moveTo.X , moveTo.Y);
        }

        public static void MouseMove(int x , int y)
        {
            if(x==0 && y == 0) { return; }
            SetCursorPos( Cursor.Position.X + x , Cursor.Position.Y + y);
        }

        public static void MouseMove(Point move)
        {
            if (move.X == 0 && move.Y == 0) { return; }
            SetCursorPos(Cursor.Position.X + move.X , Cursor.Position.Y + move.Y);
        }

    }

    public class HotKey : IMessageFilter, IDisposable
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern UInt32 GlobalAddAtom(String lpString);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern UInt32 RegisterHotKey(IntPtr hWnd, UInt32 id, UInt32 fsModifiers, UInt32 vk);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern UInt32 GlobalDeleteAtom(UInt32 nAtom);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern UInt32 UnregisterHotKey(IntPtr hWnd, UInt32 id);

        IntPtr _hWnd = IntPtr.Zero;
        UInt32 _hotKeyID;
        Keys _hotKey = Keys.None;
        Keys _comboKey = Keys.None;

        public HotKey(IntPtr formHandle, Keys hotKey, Keys comboKey)
        {
            _hWnd = formHandle; //Form Handle, 註冊系統熱鍵需要用到這個
            _hotKey = hotKey; //熱鍵
            _comboKey = comboKey; //組合鍵, 必須設定Keys.Control, Keys.Alt, Keys.Shift, Keys.None以及Keys.LWin等值才有作用

            UInt32 uint_comboKey; //由於API對於組合鍵碼的定義不一樣, 所以我們這邊做個轉換
            switch (comboKey)
            {
                case Keys.Alt:
                    uint_comboKey = 0x1;
                    break;
                case Keys.Control:
                    uint_comboKey = 0x2;
                    break;
                case Keys.Shift:
                    uint_comboKey = 0x4;
                    break;
                case Keys.LWin:
                    uint_comboKey = 0x8;
                    break;
                default: //沒有組合鍵
                    uint_comboKey = 0x0;
                    break;
            }

            _hotKeyID = GlobalAddAtom(Guid.NewGuid().ToString()); //向系統取得一組id
            RegisterHotKey((IntPtr)_hWnd, _hotKeyID, uint_comboKey, (UInt32)hotKey); //使用Form Handle與id註冊系統熱鍵
            Application.AddMessageFilter(this); //使用HotKey類別來監視訊息
        }

        public delegate void HotkeyEventHandler(object sender, HotKeyEventArgs e); //HotKeyEventArgs是自訂事件參數
        public event HotkeyEventHandler OnHotkey; //自訂事件

        const int WM_GLOBALHOTKEYDOWN = 0x312; //當按下系統熱鍵時, 系統會發送的訊息

        public bool PreFilterMessage(ref Message m)
        {
            if (OnHotkey != null && m.Msg == WM_GLOBALHOTKEYDOWN && (UInt32)m.WParam == _hotKeyID) //如果接收到系統熱鍵訊息且id相符時
            {
                OnHotkey(this, new HotKeyEventArgs(_hotKey, _comboKey)); //呼叫自訂事件, 傳遞自訂參數
                return true; //並攔截這個訊息, Form將不再接收到這個訊息
            }

            return false;
        }

        public class HotKeyEventArgs : EventArgs
        {
            private Keys _hotKey;
            public Keys HotKey //熱鍵
            {
                get { return _hotKey; }
                private set { }
            }

            private Keys _comboKey;
            public Keys ComboKey //組合鍵
            {
                get { return _comboKey; }
                private set { }
            }

            public HotKeyEventArgs(Keys hotKey, Keys comboKey)
            {
                _hotKey = hotKey;
                _comboKey = comboKey;
            }
        }

        private bool disposed = false;

        public void Dispose()
        {
            if (!disposed)
            {
                UnregisterHotKey(_hWnd, _hotKeyID); //取消熱鍵
                GlobalDeleteAtom(_hotKeyID); //刪除id
                OnHotkey = null; //取消所有關聯的事件
                Application.RemoveMessageFilter(this); //不再使用HotKey類別監視訊息

                GC.SuppressFinalize(this);
                disposed = true;
            }
        }

        ~HotKey()
        {
            Dispose();
        }

    }

}
