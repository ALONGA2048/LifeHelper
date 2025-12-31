using MaterialSkin.Controls;
using System.Windows.Forms;

namespace LifeHelper
{
    partial class Form_Music
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            btnPlay = new MaterialButton();
            btnStop = new MaterialButton();
            lblUrl = new MaterialLabel();
            txtYoutubeUrl = new MaterialTextBox2();
            materialProgressBar1 = new MaterialProgressBar();
            materialCard1 = new MaterialCard();
            showUp = new MaterialButton();
            label1 = new Label();
            label2 = new Label();
            materialRadioButton1 = new MaterialRadioButton();
            materialRadioButton2 = new MaterialRadioButton();
            materialTabControl1 = new MaterialTabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            materialCard1.SuspendLayout();
            materialTabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // btnPlay
            // 
            btnPlay.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnPlay.Density = MaterialButton.MaterialButtonDensity.Default;
            btnPlay.Depth = 0;
            btnPlay.HighEmphasis = true;
            btnPlay.Icon = null;
            btnPlay.Location = new Point(625, 138);
            btnPlay.Margin = new Padding(4, 6, 4, 6);
            btnPlay.MouseState = MaterialSkin.MouseState.HOVER;
            btnPlay.Name = "btnPlay";
            btnPlay.NoAccentTextColor = Color.Empty;
            btnPlay.Size = new Size(85, 36);
            btnPlay.TabIndex = 8;
            btnPlay.Text = "載入播放";
            btnPlay.Type = MaterialButton.MaterialButtonType.Contained;
            btnPlay.UseAccentColor = true;
            btnPlay.UseVisualStyleBackColor = true;
            btnPlay.Click += btnPlay_Click;
            // 
            // btnStop
            // 
            btnStop.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnStop.Density = MaterialButton.MaterialButtonDensity.Default;
            btnStop.Depth = 0;
            btnStop.HighEmphasis = false;
            btnStop.Icon = null;
            btnStop.Location = new Point(715, 138);
            btnStop.Margin = new Padding(4, 6, 4, 6);
            btnStop.MouseState = MaterialSkin.MouseState.HOVER;
            btnStop.Name = "btnStop";
            btnStop.NoAccentTextColor = Color.Empty;
            btnStop.Size = new Size(64, 36);
            btnStop.TabIndex = 9;
            btnStop.Text = "停止";
            btnStop.Type = MaterialButton.MaterialButtonType.Outlined;
            btnStop.UseAccentColor = false;
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // lblUrl
            // 
            lblUrl.AutoSize = true;
            lblUrl.Depth = 0;
            lblUrl.Font = new Font("Roboto", 24F, FontStyle.Bold, GraphicsUnit.Pixel);
            lblUrl.FontType = MaterialSkin.MaterialSkinManager.fontType.H5;
            lblUrl.Location = new Point(25, 95);
            lblUrl.MouseState = MaterialSkin.MouseState.HOVER;
            lblUrl.Name = "lblUrl";
            lblUrl.Size = new Size(191, 29);
            lblUrl.TabIndex = 10;
            lblUrl.Text = "YouTube 連結來源";
            // 
            // txtYoutubeUrl
            // 
            txtYoutubeUrl.AnimateReadOnly = false;
            txtYoutubeUrl.BackgroundImageLayout = ImageLayout.None;
            txtYoutubeUrl.CharacterCasing = CharacterCasing.Normal;
            txtYoutubeUrl.Depth = 0;
            txtYoutubeUrl.Font = new Font("Microsoft JhengHei UI", 12F);
            txtYoutubeUrl.HideSelection = true;
            txtYoutubeUrl.LeadingIcon = null;
            txtYoutubeUrl.Location = new Point(25, 135);
            txtYoutubeUrl.MaxLength = 32767;
            txtYoutubeUrl.MouseState = MaterialSkin.MouseState.OUT;
            txtYoutubeUrl.Name = "txtYoutubeUrl";
            txtYoutubeUrl.PasswordChar = '\0';
            txtYoutubeUrl.PrefixSuffixText = null;
            txtYoutubeUrl.ReadOnly = false;
            txtYoutubeUrl.RightToLeft = RightToLeft.No;
            txtYoutubeUrl.SelectedText = "";
            txtYoutubeUrl.SelectionLength = 0;
            txtYoutubeUrl.SelectionStart = 0;
            txtYoutubeUrl.ShortcutsEnabled = true;
            txtYoutubeUrl.Size = new Size(580, 48);
            txtYoutubeUrl.TabIndex = 0;
            txtYoutubeUrl.TabStop = false;
            txtYoutubeUrl.TextAlign = HorizontalAlignment.Left;
            txtYoutubeUrl.TrailingIcon = null;
            txtYoutubeUrl.UseSystemPasswordChar = false;
            // 
            // materialProgressBar1
            // 
            materialProgressBar1.Depth = 0;
            materialProgressBar1.Location = new Point(20, 85);
            materialProgressBar1.MouseState = MaterialSkin.MouseState.HOVER;
            materialProgressBar1.Name = "materialProgressBar1";
            materialProgressBar1.Size = new Size(715, 5);
            materialProgressBar1.TabIndex = 12;
            // 
            // materialCard1
            // 
            materialCard1.BackColor = Color.FromArgb(255, 255, 255);
            materialCard1.Controls.Add(showUp);
            materialCard1.Controls.Add(label1);
            materialCard1.Controls.Add(label2);
            materialCard1.Controls.Add(materialProgressBar1);
            materialCard1.Depth = 0;
            materialCard1.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialCard1.Location = new Point(25, 250);
            materialCard1.Margin = new Padding(14);
            materialCard1.MouseState = MaterialSkin.MouseState.HOVER;
            materialCard1.Name = "materialCard1";
            materialCard1.Padding = new Padding(14);
            materialCard1.Size = new Size(755, 110);
            materialCard1.TabIndex = 13;
            // 
            // showUp
            // 
            showUp.AutoSize = false;
            showUp.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            showUp.Density = MaterialButton.MaterialButtonDensity.Default;
            showUp.Depth = 0;
            showUp.HighEmphasis = true;
            showUp.Icon = null;
            showUp.Location = new Point(640, 15);
            showUp.Margin = new Padding(4, 6, 4, 6);
            showUp.MouseState = MaterialSkin.MouseState.HOVER;
            showUp.Name = "showUp";
            showUp.NoAccentTextColor = Color.Empty;
            showUp.Size = new Size(100, 45);
            showUp.TabIndex = 17;
            showUp.Text = "開啟播放器";
            showUp.Type = MaterialButton.MaterialButtonType.Contained;
            showUp.UseAccentColor = false;
            showUp.UseVisualStyleBackColor = true;
            showUp.Click += showUp_Click;
            // 
            // label1
            // 
            label1.AutoEllipsis = true;
            label1.Font = new Font("Microsoft JhengHei UI", 9F);
            label1.ForeColor = Color.DimGray;
            label1.Location = new Point(20, 50);
            label1.Name = "label1";
            label1.Size = new Size(500, 25);
            label1.TabIndex = 15;
            label1.Text = "等待指令中...";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Bold);
            label2.ForeColor = Color.Gray;
            label2.Location = new Point(20, 18);
            label2.Name = "label2";
            label2.Size = new Size(112, 22);
            label2.TabIndex = 14;
            label2.Text = "任務處理進度";
            // 
            // materialRadioButton1
            // 
            materialRadioButton1.AutoSize = true;
            materialRadioButton1.Depth = 0;
            materialRadioButton1.Location = new Point(25, 195);
            materialRadioButton1.Margin = new Padding(0);
            materialRadioButton1.MouseLocation = new Point(-1, -1);
            materialRadioButton1.MouseState = MaterialSkin.MouseState.HOVER;
            materialRadioButton1.Name = "materialRadioButton1";
            materialRadioButton1.Ripple = true;
            materialRadioButton1.Size = new Size(95, 37);
            materialRadioButton1.TabIndex = 14;
            materialRadioButton1.TabStop = true;
            materialRadioButton1.Text = "線上播放";
            materialRadioButton1.UseVisualStyleBackColor = true;
            // 
            // materialRadioButton2
            // 
            materialRadioButton2.AutoSize = true;
            materialRadioButton2.Depth = 0;
            materialRadioButton2.Location = new Point(135, 195);
            materialRadioButton2.Margin = new Padding(0);
            materialRadioButton2.MouseLocation = new Point(-1, -1);
            materialRadioButton2.MouseState = MaterialSkin.MouseState.HOVER;
            materialRadioButton2.Name = "materialRadioButton2";
            materialRadioButton2.Ripple = true;
            materialRadioButton2.Size = new Size(95, 37);
            materialRadioButton2.TabIndex = 15;
            materialRadioButton2.TabStop = true;
            materialRadioButton2.Text = "本地下載";
            materialRadioButton2.UseVisualStyleBackColor = true;
            // 
            // materialTabControl1
            // 
            materialTabControl1.Controls.Add(tabPage1);
            materialTabControl1.Depth = 0;
            materialTabControl1.Location = new Point(850, 0);
            materialTabControl1.MouseState = MaterialSkin.MouseState.HOVER;
            materialTabControl1.Multiline = true;
            materialTabControl1.Name = "materialTabControl1";
            materialTabControl1.SelectedIndex = 0;
            materialTabControl1.Size = new Size(200, 100);
            materialTabControl1.TabIndex = 16;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 34);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(192, 62);
            tabPage1.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(0, 0);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(200, 100);
            tabPage2.TabIndex = 0;
            // 
            // Form_Music
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(812, 385);
            Controls.Add(materialTabControl1);
            Controls.Add(materialRadioButton2);
            Controls.Add(materialRadioButton1);
            Controls.Add(materialCard1);
            Controls.Add(btnStop);
            Controls.Add(btnPlay);
            Controls.Add(txtYoutubeUrl);
            Controls.Add(lblUrl);
            DrawerTabControl = materialTabControl1;
            Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
            MaximizeBox = false;
            Name = "Form_Music";
            Padding = new Padding(3, 80, 3, 3);
            Sizable = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "音樂總管";
            Load += Form_Music_Load;
            materialCard1.ResumeLayout(false);
            materialCard1.PerformLayout();
            materialTabControl1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MaterialSkin.Controls.MaterialButton btnPlay;
        private MaterialSkin.Controls.MaterialButton btnStop;
        private MaterialSkin.Controls.MaterialLabel lblUrl;
        private MaterialSkin.Controls.MaterialTextBox2 txtYoutubeUrl;
        private MaterialSkin.Controls.MaterialProgressBar materialProgressBar1;
        private MaterialSkin.Controls.MaterialCard materialCard1;
        private MaterialSkin.Controls.MaterialRadioButton materialRadioButton1;
        private MaterialSkin.Controls.MaterialRadioButton materialRadioButton2;
        private MaterialSkin.Controls.MaterialTabControl materialTabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private MaterialSkin.Controls.MaterialButton showUp;
        // --- 請確保補上這兩行 ---
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}