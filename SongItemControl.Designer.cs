namespace LifeHelper
{
    partial class SongItemControl
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            lblSongTitle = new Label();
            btnDownload = new Button();
            btnAddToLocal = new Button();
            playListcomboBox = new ComboBox();
            SuspendLayout();
            // 
            // lblSongTitle
            // 
            lblSongTitle.Font = new Font("Microsoft JhengHei UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSongTitle.Location = new Point(3, 20);
            lblSongTitle.Name = "lblSongTitle";
            lblSongTitle.Size = new Size(415, 44);
            lblSongTitle.TabIndex = 0;
            lblSongTitle.Text = "label1";
           
            // 
            // btnDownload
            // 
            btnDownload.Location = new Point(438, 20);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(81, 29);
            btnDownload.TabIndex = 1;
            btnDownload.Text = "下載";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // btnAddToLocal
            // 
            btnAddToLocal.Location = new Point(715, 20);
            btnAddToLocal.Name = "btnAddToLocal";
            btnAddToLocal.Size = new Size(81, 29);
            btnAddToLocal.TabIndex = 2;
            btnAddToLocal.Text = "加入歌單";
            btnAddToLocal.UseVisualStyleBackColor = true;
            btnAddToLocal.Click += btnAddToLocal_Click;
            // 
            // playListcomboBox
            // 
            playListcomboBox.FormattingEnabled = true;
            playListcomboBox.Location = new Point(538, 22);
            playListcomboBox.Name = "playListcomboBox";
            playListcomboBox.Size = new Size(151, 27);
            playListcomboBox.TabIndex = 3;
            // 
            // SongItemControl
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(playListcomboBox);
            Controls.Add(btnAddToLocal);
            Controls.Add(btnDownload);
            Controls.Add(lblSongTitle);
            Name = "SongItemControl";
            Size = new Size(805, 76);
            ResumeLayout(false);
        }

        #endregion

        private Label lblSongTitle;
        private Button btnDownload;
        private Button btnAddToLocal;
        private ComboBox playListcomboBox;
    }
}
