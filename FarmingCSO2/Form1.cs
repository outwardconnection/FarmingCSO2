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

        private bool Begin { set; get; }
        private Color[] startButtonColor = new Color[4];
        private IntPtr CSO2Window { set; get; }
        public static int screenWidth { get { return Screen.PrimaryScreen.Bounds.Width; } }
        private static int screenHeight { get { return Screen.PrimaryScreen.Bounds.Height; } }

        public Form1()
        {
            InitializeComponent();
            Begin = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(screenWidth / 2 - this.Width / 2 , screenHeight / 2 - this.Width / 2);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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

            if (Begin)
            {
                button2.Text = "開始掛機";
                Begin = false;
            }
            else
            {
                if (true)
                {
                    button2.Text = "停止掛機";
                    Begin = true;
                    TimerLabel.Show();
                    Farming();
                }
                else
                {
                    MessageBox.Show("還沒啟用WinIO");
                }

            }

        }

        private void Farming()
        {
            while (true)
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (i % 100 == 0)
                    {
                        TimerLabel.Text = "還剩" + (10 - i / 100).ToString() + "秒";
                    }
                    if (!Begin) { return; }
                    Thread.Sleep(10);
                    Application.DoEvents();
                }
                TimerLabel.Hide();
                
                ImitateOperating.MouseMoveTo(screenWidth/2, screenHeight/2);

                for (int i = 0; i < 200; i++)
                {
                    if (!Begin) { return; }
                    Thread.Sleep(10);
                    Application.DoEvents();
                }
                //一直往右滑
                for(short i = 0 ; i < 20 ; i++)
                {
                    ImitateOperating.MouseMove(screenWidth/2, 0);
                    for (short j = 0; j < 5; j++)
                    {
                        if (!Begin) { return; }
                        Thread.Sleep(10);
                        Application.DoEvents();
                    }

                }

                ImitateOperating.MouseMove(screenWidth, 0);

                for (int i = 0; i < 200; i++)
                {
                    if (!Begin) { return; }
                    Thread.Sleep(10);
                    Application.DoEvents();
                }

                ImitateOperating.MouseMove((-1*screenWidth*2), 0);

                for (int i = 0; i < 200; i++)
                {
                    if (!Begin) { return; }
                    Thread.Sleep(10);
                    Application.DoEvents();
                }

                //按 返回大廳 鍵
                ImitateOperating.MouseMoveTo((int)((float)1096 / 1536 * screenWidth), (int)((float)802 / 864 * screenHeight));
                ImitateOperating.MouseLeftClick();
                for (int i = 0; i < 1000; i++)
                {
                    if (!Begin) { return; }
                    Thread.Sleep(10);
                    Application.DoEvents();
                }

                //按 開始/準備鍵
                ImitateOperating.MouseMoveTo( (int)((float)1410/1536*screenWidth) , (int)((float)614/864*screenHeight));

                for (int i = 0; i < 200; i++)
                {
                    if (!Begin) { return; }
                    Thread.Sleep(10);
                    Application.DoEvents();
                }

                ImitateOperating.MouseLeftClick();

            }
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
            Begin = false;
            ImitateOperating.Shutdown();

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

}
