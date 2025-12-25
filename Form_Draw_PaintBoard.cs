using System;

using System.Drawing;

using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing.Imaging; // 【新增】用於儲存圖片


namespace LifeHelper

{

    public partial class Form_Draw_PaintBoard : Form

    {

        private bool isDrawing = false;

        private Point lastPoint;

        private Bitmap drawingSurface;
      
        private List<Bitmap> undoHistory = new List<Bitmap>();
        private List<Bitmap> redoHistory = new List<Bitmap>();
        // 【新增】畫筆相關屬性
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color CurrentColor { get; set; } = Color.Red; // 預設顏色
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float PenThickness { get; set; } = 10; // 預設粗細
        // 【新增 1】文字控制項相關變數
        public ResizableLabel currentLabel = null; // 當前操作的 Label
        private ResizableImage currentImage = null;
        private bool isMovingLabel = false;
        private bool isResizingLabel = false;
        private Point dragStartPoint;
        private const int ResizeHandleSize = 8; // 調整大小的熱區大小
        private AnchorType currentAnchor = AnchorType.None; // 調整大小的類型
        // 錨點類型
        private enum AnchorType { None, BottomRight }
        // 【新增】繪圖模式
        public enum DrawMode { Dot, Line, Rectangle } // 點、線、矩形(面)
        public DrawMode currentDrawMode = DrawMode.Line; // 預設為線

        public Form_Draw_PaintBoard()

        {

            InitializeComponent();



            // 🔹設定全螢幕

            this.FormBorderStyle = FormBorderStyle.None;

            this.WindowState = FormWindowState.Maximized;





            // 🔹背景設為假透明：黑色 + 半透明 (不會被穿透)

            this.BackColor = Color.Black;

            this.Opacity = 0.5; // ❗ 保持視窗本身完全不透明，這樣畫出來的線條才是清晰的 (非 0.1)

            this.DoubleBuffered = true;

            this.KeyPreview = true;



            // 🔹初始化畫布

            drawingSurface = new Bitmap(
                Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb // 確保畫布能夠支援透明度和彩色。
            );

            // 【新增 2】第一次啟動時，儲存初始的透明狀態
            SaveState(isStartup: true);

            this.ShowInTaskbar = false;

            // 🔹確保可以接收滑鼠事件

            EnableMouseInput();

        }
        // ===============================================
        //  檔案操作功能 (匯入/匯出)
        // ===============================================
        // Form_Draw_PaintBoard.cs

        // 【新增 1】圖片控制項相關變數

        // ... (其他 ResizableLabel 相關變數)

        // 錨點類型 (AnchorType) 保持不變

        // ... (其他程式碼)

        // ===============================================
        //  檔案操作功能 (匯入/匯出) - 修改 ImportImage
        // ===============================================

        public void ImportImage(string filePath)
        {
            // 【修正：使用 MemoryStream 確保檔案句柄立即釋放，並安全讀取圖片】
            Image importedImage = null;
            try
            {
                // 1. 將檔案內容讀取到記憶體流 (MemoryStream)
                byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageBytes))
                {
                    // 2. 從記憶體流創建 Image
                    // 使用 Image.FromStream 確保圖片讀取正確，且檔案句柄在離開 using 區塊後立即釋放
                    importedImage = Image.FromStream(ms);
                }

                

                // 3. 每次新增一個 Image
                // 注意：這裡必須使用 Clone() 來創建一個新的 Bitmap 副本，以防止 MemoryStream 關閉後圖片失效。
                // ResizableImage 的建構子會再次 Clone 一次，但這個外部的 Clone 是必要的。
                using (Bitmap initialBitmap = new Bitmap(importedImage))
                {
                    ResizableImage newImage = new ResizableImage(this, initialBitmap);
                    // 螢幕中央顯示
                    newImage.Location = new Point(this.Width / 2 - newImage.Width / 2, this.Height / 2 - newImage.Height / 2); //

                    // 綁定滑鼠事件
                    newImage.MouseDown += Image_MouseDown; 
                    newImage.MouseMove += Image_MouseMove;
                    newImage.MouseUp += Image_MouseUp; 

                    this.Controls.Add(newImage); 
                    newImage.BringToFront();

                    currentImage = newImage; 
                }

                this.Invalidate(); 
            }
            catch (Exception ex)
            {
                // 如果錯誤資訊是 "無法匯入透明項"，通常會在這裡被捕獲
                MessageBox.Show($"匯入圖片失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error); //
            }
        }


        // Form_Draw_PaintBoard.cs (FinalizeImage 方法)

        public void FinalizeImage()
        {
            if (currentImage != null)
            {
                // 1. 確保視窗是完全不透明的 (讓圖片畫到 drawingSurface 上時沒有 Opacity 影響)
                this.Opacity = 1.0;
                Application.DoEvents(); // 讓系統立即處理 Opacity 改變

                SaveState();

                using (Graphics g = Graphics.FromImage(drawingSurface))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    // 【核心修正：強制轉換為不透明的 Bitmap 副本】
                    // 創建一個新的、完全不透明的 Bitmap，將原圖繪製到上面
                    using (Bitmap solidImage = new Bitmap(currentImage.Width, currentImage.Height, PixelFormat.Format32bppArgb))
                    {
                        using (Graphics tempG = Graphics.FromImage(solidImage))
                        {
                            // 確保背景是實色 (例如白色)，避免透明 PNG 的透明區域變成半透明
                            tempG.Clear(Color.White);
                            // 繪製圖片到這個實心背景上
                            tempG.DrawImage(currentImage.Image, 0, 0, currentImage.Width, currentImage.Height);
                        }

                        // 2. 將這個強制不透明的 solidImage 畫到 drawingSurface 上
                        g.DrawImage(
                            solidImage,
                            currentImage.Location.X,
                            currentImage.Location.Y,
                            solidImage.Width,
                            solidImage.Height
                        );
                    }
                }

                // 移除 Image 控制項
                this.Controls.Remove(currentImage);
                currentImage.Dispose();
                currentImage = null;

                this.Invalidate();

                // 恢復半透明
                this.Opacity = 0.5; //
                Application.DoEvents();

                SaveState();
            }
        }


        // 【新增 3】圖片的滑鼠事件處理 (邏輯與 Label 類似，但 Image 控制項需要實作)
        private void Image_MouseDown(object sender, MouseEventArgs e)
        {
            // 💡 邏輯可以完全借鑒 Label_MouseDown
            if (e.Button == MouseButtons.Left)
            {
                ResizableImage image = (ResizableImage)sender;
                dragStartPoint = e.Location;
                currentAnchor = GetAnchorType(image, e.Location); // 需要一個 GetAnchorType(ResizableImage, ...) 版本

                if (currentAnchor != AnchorType.None)
                {
                    isResizingLabel = true; // 可以共用 isResizingLabel
                    Cursor = Cursors.SizeNWSE; // 【新增】

                }
                else
                {
                    isMovingLabel = true; // 可以共用 isMovingLabel
                    Cursor = Cursors.SizeAll; // 【新增】
                }
                if (currentImage == (ResizableImage)sender)
                 {
                this.Opacity = 1.0;
                }
            }
            
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            // 💡 邏輯可以完全借鑒 Label_MouseMove，只是操作的物件是 ResizableImage
            ResizableImage image = (ResizableImage)sender;

            if (isMovingLabel)
            {
                image.Left += e.X - dragStartPoint.X;
                image.Top += e.Y - dragStartPoint.Y;
                
                image.Invalidate();
            }
            else if (isResizingLabel && currentAnchor == AnchorType.BottomRight)
            {
                // 縮放 (右下角)
                int deltaX = e.X - dragStartPoint.X;
                int deltaY = e.Y - dragStartPoint.Y;

                // 【修正 1: 設定合理的最小尺寸 (例如 50x50)】
                const int MIN_SIZE = 50;

                // 改變寬度
                image.Width = Math.Max(image.Width + deltaX, MIN_SIZE);

                // 改變高度
                image.Height = Math.Max(image.Height + deltaY, MIN_SIZE);

                // 更新起始點以實現連續拖曳縮放
                // 必須重設 dragStartPoint，讓下一次 MouseMove 的 deltaX/Y 從新的點開始計算
                dragStartPoint = e.Location;

                // 【新增】強制重繪圖片以更新大小和邊界
                image.Invalidate();
            }
            else
            {
                // 根據 GetAnchorType 判斷當前游標應該是什麼
                AnchorType current = GetAnchorType(image, e.Location);

                if (current == AnchorType.BottomRight)
                {
                    Cursor = Cursors.SizeNWSE;
                }
                else
                {
                    // 如果不在調整大小熱區，但滑鼠在圖片上，則顯示移動游標
                    Cursor = Cursors.SizeAll;
                }
            }

        }

        private void Image_MouseUp(object sender, MouseEventArgs e)
        {
            // 💡 邏輯可以完全借鑒 Label_MouseUp
            isMovingLabel = false;
            isResizingLabel = false;
            currentAnchor = AnchorType.None;
            if (currentImage == (ResizableImage)sender)
            {
                this.Opacity = 0.5;
            }
            ((ResizableImage)sender).Invalidate();
            
        }

        // 【新增 4】圖片的 GetAnchorType 方法
        private AnchorType GetAnchorType(ResizableImage image, Point location)
        {
            // 邏輯與 GetAnchorType(ResizableLabel, ...) 相同
            Rectangle resizeArea = new Rectangle(
                image.Width - ResizeHandleSize,
                image.Height - ResizeHandleSize,
                ResizeHandleSize,
                ResizeHandleSize);

            if (resizeArea.Contains(location))
            {
                return AnchorType.BottomRight;
            }
            return AnchorType.None;
        }

        // 【新增 5】用於 Undo/Redo 時呼叫
        public void FinalizeCurrentItem()
        {
            FinalizeText();
            FinalizeImage();
        }


        public void ExportDrawing(string filePath, bool includeDesktop, bool hideSelf = true)
        {
            try
            {
                Bitmap finalImage;
                // 確保先定稿任何正在操作的文字或圖片
                FinalizeCurrentItem();

                if (includeDesktop)
                {
                    // 匯出 (桌面 + 塗鴉)

                    if (hideSelf)
                    {
                        // **【修正點 A】在截圖前徹底隱藏，並讓系統有時間處理**
                        // 這裡假設呼叫者已經處理了 FileDialog 的彈出，我們只需要執行截圖和隱藏。
                        this.Visible = false;
                        // 增加一個 DoEvents 確保視窗隱藏指令被立即處理
                        Application.DoEvents();
                    }

                    Rectangle screenRect = Screen.PrimaryScreen.Bounds;
                    Bitmap desktopImage = new Bitmap(screenRect.Width, screenRect.Height);

                    //  截圖桌面 (此時 PaintBoard 應該已經隱藏)
                    using (Graphics gDesktop = Graphics.FromImage(desktopImage))
                    {
                        gDesktop.CopyFromScreen(screenRect.X, screenRect.Y, 0, 0, screenRect.Size);
                    }

                    // 2. 將 drawingSurface (塗鴉層) 畫到桌面截圖上
                    using (Graphics gFinal = Graphics.FromImage(desktopImage))
                    {
                        gFinal.DrawImage(drawingSurface, 0, 0);
                    }

                    finalImage = desktopImage;

                    if (hideSelf)
                    {
                        // **【修正點 B】截圖完成後立刻恢復 PaintBoard 顯示**
                        this.Visible = true;
                        Application.DoEvents();
                    }
                }
                else
                {
                    // 匯出 (僅塗鴉層) 邏輯保持不變
                    finalImage = drawingSurface;
                }

                // 儲存圖片
                finalImage.Save(filePath, ImageFormat.Png);

                if (includeDesktop)
                {
                    finalImage.Dispose(); // 如果是截圖，記得釋放
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"匯出圖片失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // **【修正點 C】確保即使失敗也要恢復顯示**
                if (this.Visible == false)
                {
                    this.Visible = true;
                }
            }
        }

        // ===============================================
        //  文字按鈕 / Label 處理
        // ===============================================

        public void AddTextLabel()
        {
            
            ResizableLabel newLabel = new ResizableLabel(this);
            newLabel.Location = new Point(this.Width / 2 - 50, this.Height / 2 - 20); 
            newLabel.Text = "雙擊編輯文字";
            newLabel.Font = new Font("微軟正黑體", 20);
            newLabel.AutoSize = true; 
            newLabel.BackColor = Color.Transparent;
            newLabel.ForeColor = CurrentColor; // 使用當前畫筆顏色

            // 綁定滑鼠事件，以便移動和調整大小
            newLabel.MouseDown += Label_MouseDown;
            newLabel.MouseMove += Label_MouseMove;
            newLabel.MouseUp += Label_MouseUp;
            newLabel.MouseDoubleClick += Label_MouseDoubleClick;

            this.Controls.Add(newLabel);
            newLabel.BringToFront();

            // 設定為當前操作的 Label
            currentLabel = newLabel;
        }

        // 當使用者完成所有操作 (移動/編輯/縮放) 後，將 Label 畫到 Bitmap 上並移除
        public void FinalizeText()
        {
            if (currentLabel != null && currentLabel.Text.Length > 0)
            {
                SaveState(); // 儲存操作前的狀態

                // 將 Label 內容畫到 drawingSurface
                using (Graphics g = Graphics.FromImage(drawingSurface))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // 繪製文字 (使用 Label 的位置、大小、文字、字體和顏色)
                    using (SolidBrush brush = new SolidBrush(currentLabel.ForeColor))
                    {
                        g.DrawString(currentLabel.Text, currentLabel.Font, brush, currentLabel.Location);
                    }
                }

                // 移除 Label 控制項
                this.Controls.Remove(currentLabel);
                currentLabel.Dispose();
                currentLabel = null;

                this.Invalidate(); // 更新畫布
                SaveState(); // 儲存操作後的狀態
            }
        }

        // ===============================================
        //  動態 Label 的滑鼠事件處理 (移動和縮放)
        // ===============================================

        private void Label_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ResizableLabel label = (ResizableLabel)sender;

                // 【修正 1: 儲存滑鼠在 Label 內的起始相對位置】
                // e.Location 是滑鼠相對於 Label 左上角的座標 (相對座標)。
                dragStartPoint = e.Location;

                currentAnchor = GetAnchorType(label, e.Location);

                if (currentAnchor != AnchorType.None)
                {
                    isResizingLabel = true;
                }
                else
                {
                    isMovingLabel = true;
                    // 無需修改 dragStartPoint，它已經是我們需要的相對起點。
                }
            }
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            ResizableLabel label = (ResizableLabel)sender;

            if (isMovingLabel)
            {
                // 【修正 2: 使用位移量計算新位置】
                // Label.Left/Top += (當前滑鼠相對位置 - 拖曳起始相對位置)
                // 這樣無論 Label 在哪，左上角和滑鼠之間的距離都會保持不變，實現平滑移動。
                label.Left += e.X - dragStartPoint.X;
                label.Top += e.Y - dragStartPoint.Y;

                // 移除這裡的 label.Invalidate()，以減少閃爍。
            }
            // ... (isResizingLabel 邏輯保持不變)
            // ... (else 改變游標邏輯保持不變)
        }

        private void Label_MouseUp(object sender, MouseEventArgs e)
        {
            isMovingLabel = false;
            isResizingLabel = false;
            currentAnchor = AnchorType.None;
            ((ResizableLabel)sender).Invalidate(); // 重新繪製 Label (移除邊界)
        }

        private void Label_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 雙擊時彈出對話框讓使用者編輯文字和字體
            ResizableLabel label = (ResizableLabel)sender;
            using (TextEditDialog dialog = new TextEditDialog(label.Text, label.Font, label.ForeColor))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    label.Text = dialog.EditedText;
                    label.Font = dialog.EditedFont;
                    label.ForeColor = dialog.EditedColor;
                }
            }
        }

        // 判斷滑鼠是否在右下角調整熱區內
        private AnchorType GetAnchorType(ResizableLabel label, Point location)
        {
            Rectangle resizeArea = new Rectangle(
                label.Width - ResizeHandleSize,
                label.Height - ResizeHandleSize,
                ResizeHandleSize,
                ResizeHandleSize);

            if (resizeArea.Contains(location))
            {
                return AnchorType.BottomRight;
            }
            return AnchorType.None;
        }

        // ===============================================
        //  自訂 Label 類別 (用於繪製邊界)
        // ===============================================

       

        // 【新增 3】儲存當前狀態的方法
        private void SaveState(bool isStartup = false)
        {
            // 如果不是啟動時，且處於繪圖中，則清除 Redo 歷史
            if (!isStartup && undoHistory.Count > 0)
            {
                redoHistory.Clear();
            }

            // 儲存當前畫布的副本
            Bitmap snapshot = (Bitmap)drawingSurface.Clone();
            undoHistory.Add(snapshot);

            // 控制歷史紀錄最大數量，避免記憶體爆炸 (例如：限制為 50 步)
            if (undoHistory.Count > 50)
            {
                undoHistory[0].Dispose();
                undoHistory.RemoveAt(0);
            }
        }
        private void RestoreState(Bitmap state)
        {
            // 釋放舊的 drawingSurface
            drawingSurface.Dispose();
            // 替換為歷史紀錄中的狀態
            drawingSurface = state;
            this.Invalidate(); // 重繪畫布
        }
        // 【新增 4】公開的 Undo 和 Redo 方法

        public void Undo()
        {
            FinalizeCurrentItem();
           
            if (undoHistory.Count > 1)
            {
                // 儲存上一步的狀態陣列  
                Bitmap currentState = undoHistory[undoHistory.Count - 1];

                // 移除當前狀態
                undoHistory.RemoveAt(undoHistory.Count - 1);

                // 將移除的狀態加入 Redo 歷史
                redoHistory.Add(currentState);

                // 恢復上一個狀態 (即新的最後一個狀態)
                Bitmap previousState = (Bitmap)undoHistory[undoHistory.Count - 1].Clone();
                RestoreState(previousState);
            }
            else
            {
                // 【新增邊界條件提示】
                MessageBox.Show("已經是第一步，無法再執行上一步操作。", "上一步 (Undo) 失敗", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void Redo()
        {
            FinalizeCurrentItem();
            if (redoHistory.Count > 0)
            {
                // 1. 取得 Redo 狀態 (即 Redo 歷史的最後一個元素)
                Bitmap nextState = redoHistory[redoHistory.Count - 1];

                // 2. 移除 Redo 狀態
                redoHistory.RemoveAt(redoHistory.Count - 1);

                // 3. 將狀態移到 undoHistory
                undoHistory.Add(nextState);

                // 4. 恢復狀態
                // 這裡必須 Clone() 給 RestoreState，以保護 undoHistory 中的 Bitmap
                Bitmap stateToRestore = (Bitmap)nextState.Clone();
                RestoreState(stateToRestore);
            }
            else
            {
                // 【新增邊界條件提示】
                MessageBox.Show("沒有更多下一步操作可以執行。", "下一步 (Redo) 失敗", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void DrawForm_Paint(object sender, PaintEventArgs e)

        {

            e.Graphics.DrawImageUnscaled(drawingSurface, 0, 0);

        }



        private void DrawForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                
                isDrawing = true;
                lastPoint = e.Location;

               
                if (currentDrawMode == DrawMode.Dot)
                {
                    using (Graphics g = Graphics.FromImage(drawingSurface))
                    {
                        Color brightColor = Color.FromArgb(255, CurrentColor);

                       
                        float diameter = PenThickness; 
                        float x = e.X - diameter / 2;
                        float y = e.Y - diameter / 2;

                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        // 畫實心圓 (點)
                        g.FillEllipse(new SolidBrush(brightColor), x, y, diameter, diameter);
                    }
                    this.Invalidate(); // 立即重繪畫面
                    SaveState();
                }


                // 線模式 (Line) 採用 MouseMove 連續畫線
                // 面模式 (Rectangle) 採用 MouseDown 記起點，MouseUp 畫矩形
            }
        }



        private void DrawForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                using (Graphics g = Graphics.FromImage(drawingSurface))
                {
                    Color brightColor = Color.FromArgb(255, CurrentColor);

                    // 【修正 1: 設定 Pen 的 LineJoin 和 Cap 樣式】
                    using (Pen pen = new Pen(brightColor, PenThickness))
                    {
                        // 確保線段之間平滑連接，防止尖角或縫隙
                        pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                        // 確保線條起點和終點是圓形，填補線段間的縫隙
                        pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                        pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                        // 設定高品質繪圖
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        switch (currentDrawMode)
                        {
                            case DrawMode.Line:
                                // 【修正 2: 畫線模式】
                                // 由於設定了圓形線帽，DrawLine 就能有效連接大粗細線條
                                g.DrawLine(pen, lastPoint, e.Location);
                                break;

                            case DrawMode.Dot:
                                // 點模式：使用 FillEllipse (此處保持不變，因為是畫圓點)
                                g.FillEllipse(new SolidBrush(brightColor), e.X - PenThickness / 2, e.Y - PenThickness / 2, PenThickness, PenThickness);
                                break;

                            case DrawMode.Rectangle:
                                // 拖曳畫矩形，MouseMove 階段不繪圖
                                break;
                        }
                    }
                }

                // ... (後續的 Invalidate 和 lastPoint 更新邏輯保持不變)
                var rect = new Rectangle(
                    Math.Min(lastPoint.X, e.X) - 10,
                    Math.Min(lastPoint.Y, e.Y) - 10,
                    Math.Abs(lastPoint.X - e.X) + 20,
                    Math.Abs(lastPoint.Y - e.Y) + 20);
                this.Invalidate(rect);

                if (currentDrawMode == DrawMode.Line || currentDrawMode == DrawMode.Dot)
                {
                    lastPoint = e.Location;
                }
            }
        }


        private void DrawForm_MouseUp(object sender, MouseEventArgs e)

        {

            if (e.Button == MouseButtons.Left)
            {
                isDrawing = false;

               
                if (currentDrawMode == DrawMode.Rectangle)
                {
                    using (Graphics g = Graphics.FromImage(drawingSurface))
                    {
                        Color brightColor = Color.FromArgb(255, CurrentColor);
                        using (Pen pen = new Pen(brightColor, PenThickness))
                        {
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                            int x = Math.Min(lastPoint.X, e.X);
                            int y = Math.Min(lastPoint.Y, e.Y);
                            int width = Math.Abs(lastPoint.X - e.X);
                            int height = Math.Abs(lastPoint.Y - e.Y);
                            Rectangle rect = new Rectangle(x, y, width, height);

                            // 繪製實心矩形 (面)
                            g.FillRectangle(new SolidBrush(brightColor), rect);
                        }
                    }
                    this.Invalidate(); // 更新整個畫面來顯示畫出的形狀
                    SaveState();
                }
                else if (currentDrawMode == DrawMode.Line)
                {
                    // 【新增】線模式在 MouseUp 時也儲存狀態 (雖然 MouseMove 持續繪圖，但 MouseUp 標記一個動作結束)
                    SaveState();
                }
            }
                if (this.Owner != null)
            {
                this.Owner.TopMost = true;

                this.Owner.BringToFront();

                this.Owner.Activate();
            }

        }



        private void DrawForm_KeyDown(object sender, KeyEventArgs e)

        {

            if (e.KeyCode == Keys.Escape)

                this.Close();

        }

        public void SetDrawMode(DrawMode mode)
        {
            this.currentDrawMode = mode;
        }

        public void SetPenThickness(int thickness)
        {
            this.PenThickness = thickness;
        }

        public void SetPenColor(Color color)
        {
            this.CurrentColor = color;
            // 可以在 PictureBox 中顯示新的顏色
        }

        public void ClearDrawing()

        {
            SaveState();
            using (Graphics g = Graphics.FromImage(drawingSurface))

            {

                g.Clear(Color.Transparent);

            }

            this.Invalidate();

        }



        // === 讓透明視窗仍可接收滑鼠事件 ===

        [DllImport("user32.dll", SetLastError = true)]

        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]

        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        const int GWL_EXSTYLE = -20;

        const int WS_EX_LAYERED = 0x80000;

        const int WS_EX_TRANSPARENT = 0x20;



        private void EnableMouseInput()

        {

            int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);

            exStyle |= WS_EX_LAYERED;

            exStyle &= ~WS_EX_TRANSPARENT; // ❗取消穿透

            SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle);

        }
        public bool IsCurrentImage(ResizableImage image)
        {
            // currentImage 是 private 變數，只有在 Form_Draw_PaintBoard 內可以存取
            return currentImage == image;
        }

        // 【新增 2】提供給外部 ResizableLabel 判斷是否為當前操作物件的方法
        public bool IsCurrentLabel(ResizableLabel label)
        {
            // currentLabel 是 private 變數，只有在 Form_Draw_PaintBoard 內可以存取
            return currentLabel == label;
        }






       

    }

}