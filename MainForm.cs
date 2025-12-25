using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace LifeHelper
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int MOD_CONTROL = 0x2;
        const int WM_HOTKEY = 0x0312;

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        public MainForm()
        {
            InitializeComponent();
            // 初始化系統匣圖示與選單
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("音樂播放器", null, (s, e) => new Form_Music().Show());

            trayMenu.Items.Add("螢幕繪圖", null, (s, e) => new Form_Draw_Controller().Show());
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add("離開", null, (s, e) => Application.Exit());

            trayIcon = new NotifyIcon()
            {
                Icon = new Icon("bongoCat.ico"),
                ContextMenuStrip = trayMenu,
                Text = "生活小幫手",
                Visible = true
            };

            trayIcon.DoubleClick += (s, e) =>
            {
                MessageBox.Show("使用快捷鍵開啟功能：\n\nCtrl+R 音樂播放\nCtrl+T 螢幕繪圖",
                    "生活小幫手", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            RegisterHotKey(this.Handle, 1, MOD_CONTROL, (int)Keys.R);
            RegisterHotKey(this.Handle, 2, MOD_CONTROL, (int)Keys.T);

        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, 1);
            UnregisterHotKey(this.Handle, 2);

            trayIcon.Visible = false;
            base.OnFormClosing(e);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (id == 1)
                {

                    var musicForm = Form_Music.GetInstance();

                    if (musicForm.Visible)
                    {
                        musicForm.Activate(); // 如果已經顯示，就將它帶到最前端
                    }
                    else
                    {
                        musicForm.Show(); // 如果是隱藏狀態，就顯示出來
                    }
                }
                else if (id == 2)
                {
                    new Form_Draw_Controller().Show();
                }

            }
            base.WndProc(ref m);
        }

    }  
}
