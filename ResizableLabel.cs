using LifeHelper;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D; // 【新增】引入，為了使用 DashStyle

public class ResizableLabel : Label
{
    // 💡 必須在 ResizableLabel 內部定義或從 PaintBoard 獲取這個常量
    // 由於 Form_Draw_PaintBoard.cs 中定義為 private const int ResizeHandleSize = 8;
    // 為了讓 ResizableLabel 能夠獨立編譯和繪製，我們將其定義為一個 public static readonly 或直接定義。
    // 為了與 PaintBoard 的邏輯保持一致，我們選擇在 Label 內定義常量。
    private const int ResizeHandleSize = 8;

    public ResizableLabel(Control parent)
    {
        // 為了讓 Label 在 Form_Draw_PaintBoard 上顯示，我們將其父級設為 Form
        // 🔹 這裡不需要更動
        this.Parent = parent;
        this.Text = "Text";
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // 當 Label 處於被選中狀態時，繪製邊界和調整點
        // 💡 修正判斷條件，使用 Form_Draw_PaintBoard 提供的 IsCurrentLabel 方法
        if (this.Parent is Form_Draw_PaintBoard paintBoard && paintBoard.currentLabel == this) // 原本的判斷條件是正確的
        {
            // 由於 Form_Draw_PaintBoard 內的 ResizableLabel 類別沒有 DashStyle 的完整命名空間，
            // 為了在單獨的 ResizableLabel.cs 檔案中正確使用，需要確保 using System.Drawing.Drawing2D;
            // 或者使用完整的命名空間 System.Drawing.Drawing2D.DashStyle.Dot
            using (Pen pen = new Pen(Color.Cyan, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
            {
                // 繪製虛線邊界
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);

                // 繪製右下角調整點
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
}