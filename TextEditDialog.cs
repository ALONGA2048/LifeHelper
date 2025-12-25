using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LifeHelper
{
    public partial class TextEditDialog : Form
    {
        private TextBox textBox;
        private Button btnFont;
        private Button btnColor;
        private Button btnOK;
        private FontDialog fontDialog;
        private ColorDialog colorDialog;

        public string EditedText { get; private set; }
        public Font EditedFont { get; private set; }
        public Color EditedColor { get; private set; }

        public TextEditDialog(string initialText, Font initialFont, Color initialColor)
        {
            this.Text = "編輯文字";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(300, 200);

            EditedText = initialText;
            EditedFont = initialFont;
            EditedColor = initialColor;

            fontDialog = new FontDialog { Font = initialFont };
            colorDialog = new ColorDialog { Color = initialColor };

            textBox = new TextBox
            {
                Multiline = true,
                Text = initialText,
                Location = new Point(10, 10),
                Size = new Size(260, 80)
            };
            this.Controls.Add(textBox);

            btnFont = new Button { Text = "字體", Location = new Point(10, 100), Size = new Size(80, 30) };
            btnFont.Click += (s, e) => {
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    EditedFont = fontDialog.Font;
                }
            };
            this.Controls.Add(btnFont);

            btnColor = new Button { Text = "顏色", Location = new Point(100, 100), Size = new Size(80, 30) };
            btnColor.Click += (s, e) => {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    EditedColor = colorDialog.Color;
                }
            };
            this.Controls.Add(btnColor);

            btnOK = new Button { Text = "確定", DialogResult = DialogResult.OK, Location = new Point(190, 100), Size = new Size(80, 30) };
            btnOK.Click += (s, e) => {
                EditedText = textBox.Text;
            };
            this.Controls.Add(btnOK);
        }
    }
}
