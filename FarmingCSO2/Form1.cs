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

namespace FarmingCSO2
{
    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, IntPtr dwExtraInfo);

        //定義數值
        const byte A_key = 0x41;                    //鍵盤A的虛擬掃描代碼
        const byte KEYEVENTF_EXTENDEDKEY = 0x01;
        const byte KEYEVENTF_KEYUP = 0x02;

        public Form1()
        {
            InitializeComponent();
            IsInitializeWinIO = false;
            Begin = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(IsInitializeWinIO )
            {
                WinIOLab.Shutdown();
                IsInitializeWinIO = false;
                button1.Text = "啟用WioIO";
            }
            else
            {
                WinIOLab.Initialize();

                if(WinIOLab.IsInitialize)
                {
                    IsInitializeWinIO = true;
                    button1.Text = "停用WioIO";
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Begin)
            {
                button2.Text = "開始掛機";
                Begin = false;

            }
            else
            {
                if(WinIOLab.IsInitialize)
                {
                    button2.Text = "停止掛機";
                    Begin = true;
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
            for (short i = 0; i < 50; i++)  //暫停5秒
            {
                Thread.Sleep(100);
                Application.DoEvents();
                if (!Begin) { return; }
            }

            while (Begin)
            {
                WinIOLab.KeyDown(Keys.A);

                for (short i=0;i<1;i++)  //暫停0.1秒
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                    if (!Begin) { return; }
                }
                WinIOLab.KeyUp(Keys.A);

                for (short i = 0; i < 10; i++)  //暫停1秒
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                    if (!Begin) { return; }
                }

                WinIOLab.KeyDown(Keys.D);

                for (short i = 0; i < 1; i++)  //暫停0.1秒
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                    if (!Begin) { return; }
                }
                WinIOLab.KeyUp(Keys.D);

                for (short i = 0; i < 10; i++)  //暫停1秒
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                    if (!Begin) { return; }
                }
            }
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            Begin = false;
        }


        private bool Begin { set; get; }
        private bool IsInitializeWinIO { set; get; }
    }


    public class WinIOLab
    {
        private const int KBC_KEY_CMD = 0x64;
        private const int KBC_KEY_DATA = 0x60;
        [DllImport("WinIo64.dll")]
        private static extern bool InitializeWinIo();
        [DllImport("WinIo64.dll")]
        private static extern bool GetPortVal(IntPtr wPortAddr, out int pdwPortVal, byte bSize);
        [DllImport("WinIo64.dll")]
        private static extern bool SetPortVal(uint wPortAddr, IntPtr dwPortVal, byte bSize);
        [DllImport("WinIo64.dll")]
        private static extern byte MapPhysToLin(byte pbPhysAddr, uint dwPhysSize, IntPtr PhysicalMemoryHandle);
        [DllImport("WinIo64.dll")]
        private static extern bool UnmapPhysicalMemory(IntPtr PhysicalMemoryHandle, byte pbLinAddr);
        [DllImport("WinIo64.dll")]
        private static extern bool GetPhysLong(IntPtr pbPhysAddr, byte pdwPhysVal);
        [DllImport("WinIo64.dll")]
        private static extern bool SetPhysLong(IntPtr pbPhysAddr, byte dwPhysVal);
        [DllImport("WinIo64.dll")]
        private static extern void ShutdownWinIo();
        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(uint Ucode, uint uMapType);


        private WinIOLab()
        {
            IsInitialize = true;
        }
        public static void Initialize()
        {
            
            if (InitializeWinIo())
            {
                KBCWait4IBE();
                IsInitialize = true;
                Console.WriteLine("Yes~!");
            }
            else
            {
                MessageBox.Show("Load WinIO Failed!");
            }
            
            /*
            try
            {
                InitializeWinIo();
                IsInitialize = true;
            }
            */
        }
            
        public static void Shutdown()
        {
            if (IsInitialize)
                ShutdownWinIo();
            IsInitialize = false;
        }

        public static bool IsInitialize { get; set; }
        public static bool su { set; get; }

        ///等待键盘缓冲区为空
        private static void KBCWait4IBE()
        {
            int dwVal = 0;
            do
            {
                bool flag = GetPortVal((IntPtr)0x64, out dwVal, 1);
            }
            while ((dwVal & 0x2) > 0);
        }
        /// 模拟键盘标按下
        public static void KeyDown(Keys vKeyCoad)
        {
            if (!IsInitialize) return;

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
        /// 模拟键盘弹出
        public static void KeyUp(Keys vKeyCoad)
        {
            if (!IsInitialize) return;

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
    }



}
