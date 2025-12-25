namespace LifeHelper
{
    partial class Form_Draw_PaintBoard
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            fontDialog1 = new FontDialog();
            SuspendLayout();
            // 
            // Form_Draw_PaintBoard
            // 
            ClientSize = new Size(800, 450);
            Name = "Form_Draw_PaintBoard";
            Text = "DrawTool";
            Paint += DrawForm_Paint;
            KeyDown += DrawForm_KeyDown;
            MouseDown += DrawForm_MouseDown;
            MouseMove += DrawForm_MouseMove;
            MouseUp += DrawForm_MouseUp;
            ResumeLayout(false);
        }

        private FontDialog fontDialog1;
    }
}
