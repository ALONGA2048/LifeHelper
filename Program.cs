namespace LifeHelper
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MessageBox.Show("『生活小幫手』已啟動\n\n快捷鍵：\nCtrl+R 音樂播放器\nCtrl+T 螢幕繪圖",
                "生活小幫手", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Application.Run(new MainForm());
        }
    }
}