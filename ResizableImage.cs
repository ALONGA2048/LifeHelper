using System.Drawing;
using System.Windows.Forms;

namespace LifeHelper
{
    // 錨點類型必須被 Form_Draw_PaintBoard 訪問，所以放在這裡或作為 Form_Draw_PaintBoard 的 public enum
    // 為了簡化，如果 Form_Draw_PaintBoard 內已經定義了 AnchorType，這裡可以省略。

    public class ResizableImage : Control
    {
        private Bitmap _image;
        public Bitmap Image => _image;

        // 調整大小的熱區大小 (假設 Form_Draw_PaintBoard 中的 ResizeHandleSize 是 8)
        // 最好是讓 Form_Draw_PaintBoard 傳遞這個值，或直接定義一個常量
        private const int ResizeHandleSize = 8;

        public ResizableImage(Control parent, Bitmap image)
        {
            this.Parent = parent;
            // 使用 Clone 確保傳入的 Bitmap 檔案句柄可以釋放
            _image = (Bitmap)image.Clone();
            this.Width = image.Width;
            this.Height = image.Height;
            this.BackColor = Color.White;
            this.DoubleBuffered = true; // 減少移動/縮放時的閃爍
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
           
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
          
            e.Graphics.DrawImage(_image, 0, 0, this.Width, this.Height);
  
            if (this.Parent is Form_Draw_PaintBoard paintBoard && paintBoard.IsCurrentImage(this))
            {
                using (Pen pen = new Pen(Color.Yellow, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
                {
                    
                    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);

                    
                    Rectangle resizeHandle = new Rectangle(
                        this.Width - ResizeHandleSize,
                        this.Height - ResizeHandleSize,
                        ResizeHandleSize,
                        ResizeHandleSize);
                    e.Graphics.FillRectangle(Brushes.White, resizeHandle);
                    e.Graphics.DrawRectangle(Pens.Black, resizeHandle);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _image?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}