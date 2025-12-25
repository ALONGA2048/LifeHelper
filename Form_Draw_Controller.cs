using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LifeHelper
{
    public partial class Form_Draw_Controller : Form
    {
        private Form_Draw_PaintBoard drawForm;
        // 【新增 DllImport】用於最小化/恢復視窗
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // ShowWindow 參數
        private const int SW_MINIMIZE = 6;       // 最小化視窗
        private const int SW_RESTORE = 9;        // 恢復視窗
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_SHOWWINDOW = 0x0040;

        public Form_Draw_Controller()
        {
            InitializeComponent();
            // 【新增】初始化畫筆控制的 UI 狀態
            // 預設顏色為紅色，顯示在 pictureBox1 上
            pictureBox1.BackColor = Color.Red;
            // 預設繪圖模式為 '線' (對應 Form_Draw_PaintBoard.DrawMode.Line)
            radioButton2.Checked = true;
            // 預設粗細
            trackBar1.Value = 3;
            trackBar1.Minimum = 3;
            trackBar1.Maximum = 20;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (drawForm == null || drawForm.IsDisposed)
                drawForm = new Form_Draw_PaintBoard();

            // 繪畫功能設定
            drawForm.CurrentColor = pictureBox1.BackColor;
            drawForm.PenThickness = trackBar1.Value;
            if (radioButton1.Checked) drawForm.SetDrawMode(Form_Draw_PaintBoard.DrawMode.Dot);
            else if (radioButton2.Checked) drawForm.SetDrawMode(Form_Draw_PaintBoard.DrawMode.Line);
            else if (radioButton3.Checked) drawForm.SetDrawMode(Form_Draw_PaintBoard.DrawMode.Rectangle);

            drawForm.Show();

            // 讓主視窗壓在塗鴉層上面
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

           

            this.BringToFront();
            this.Activate();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (drawForm != null && !drawForm.IsDisposed)
                drawForm.ClearDrawing();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (drawForm != null && !drawForm.IsDisposed)
                drawForm.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // 【新增】點擊 PictureBox 開啟顏色對話框
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.BackColor = colorDialog1.Color;
                if (drawForm != null && !drawForm.IsDisposed)
                    drawForm.SetPenColor(colorDialog1.Color);
            }
        }
        // 【新增】繪圖模式 RadioButton 點擊事件

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (drawForm != null && !drawForm.IsDisposed)
            {
                if (radioButton1.Checked)
                    drawForm.SetDrawMode(Form_Draw_PaintBoard.DrawMode.Dot); // 點
                else if (radioButton2.Checked)
                    drawForm.SetDrawMode(Form_Draw_PaintBoard.DrawMode.Line); // 線
                else if (radioButton3.Checked)
                    // 【修正】 '面' 模式現在綁定為 Rectangle
                    drawForm.SetDrawMode(Form_Draw_PaintBoard.DrawMode.Rectangle);
            }
        }
        // 【新增】TrackBar 數值改變事件
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (drawForm != null && !drawForm.IsDisposed)
            {
                // 將 TrackBar 的數值 (int) 傳遞給 SetPenThickness (float)
                drawForm.SetPenThickness(trackBar1.Value);
            }
        }

        

        private void button4_Click(object sender, EventArgs e)
        {
            if (drawForm != null && !drawForm.IsDisposed)
            {
                drawForm.FinalizeCurrentItem();
                drawForm.Undo();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (drawForm != null && !drawForm.IsDisposed)
            {
                drawForm.FinalizeCurrentItem();
                drawForm.Redo();
            }
        }
        // ===============================================
        //  檔案操作和文字按鈕事件
        // ===============================================

        // 【新增 1】匯入圖片按鈕 
        private void button1_Click(object sender, EventArgs e)
        {
            if (drawForm == null || drawForm.IsDisposed)
            {
                MessageBox.Show("請先啟動塗鴉板。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "圖片檔案 (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp|所有檔案 (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    drawForm.ImportImage(ofd.FileName);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (drawForm == null || drawForm.IsDisposed)
            {
                MessageBox.Show("請先啟動塗鴉板。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG 圖片 (*.png)|*.png";
                sfd.DefaultExt = "png";

                // 1. 在視窗顯示時彈出對話框，取得檔案路徑
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // 檔案總管對話框在此時已經關閉。
                    string filePath = sfd.FileName;
                    bool exportSuccess = false;

                    try
                    {
                        // 確保任何操作中的項目已定稿
                        drawForm.FinalizeCurrentItem();

                        // 2. 【關鍵修正點】：隱藏 Form_Draw (控制台) 視窗
                        this.Visible = false;
                        // 強制系統立即處理隱藏指令
                        Application.DoEvents();
                        Thread.Sleep(500); // 通常 50ms (0.05秒) 即可解決殘影問題

                        // 3. 執行截圖 (ExportDrawing 內部會處理 drawForm 的隱藏與恢復)
                        // 傳入 hideSelf: true，讓 PaintBoard 自身隱藏並截圖。
                        drawForm.ExportDrawing(filePath, true, true);
                        exportSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"匯出圖片失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        // 4. 無論成功失敗，都確保控制台視窗恢復顯示
                        this.Visible = true;
                    }

                    if (exportSuccess) MessageBox.Show("圖片匯出成功！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (drawForm != null && !drawForm.IsDisposed)
            {
                
                // 彈出一個 FontDialog 讓使用者先選擇字體和文字
                drawForm.AddTextLabel();
            }
        }
    }
}