namespace LifeHelper
{
    partial class Form_Music_player
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // custom code

           
            //
            pictureBoxCover = new PictureBox();
            progressBar = new ProgressBar();
            btnPrev = new Button();
            btnPlayPause = new Button();
            btnNext = new Button();
            lblTitle = new Label();
            lblArtist = new Label();
            btnPlaylist = new Button();
            timeLabel = new Label();
            btnRewind = new Button();
            btnForward = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCover).BeginInit();
            SuspendLayout();

            btnPrev.Click += btnPrev_Click;
            btnPlayPause.Click += btnPlayPause_Click;
            btnNext.Click += btnNext_Click;
            btnPlaylist.Click += btnPlaylist_Click;
            btnRewind.Click += btnRewind_Click;
            btnForward.Click += btnForward_Click;
            progressBar.MouseDown += progressBar_MouseDown;
            progressBar.MouseMove += progressBar_MouseMove;
            progressBar.MouseUp += progressBar_MouseUp;
            playbackTimer.Interval = 500;
            playbackTimer.Tick += new EventHandler(playbackTimer_Tick);
            // 
            // pictureBoxCover
            // 
            pictureBoxCover.Location = new Point(12, 66);
            pictureBoxCover.Name = "pictureBoxCover";
            pictureBoxCover.Size = new Size(228, 138);
            pictureBoxCover.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxCover.TabIndex = 0;
            pictureBoxCover.TabStop = false;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(40, 255);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(420, 15);
            progressBar.TabIndex = 2;
            // 
            // btnPrev
            // 
            btnPrev.Font = new Font("Segoe UI Symbol", 16F, FontStyle.Bold);
            btnPrev.Location = new Point(148, 305);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(48, 45);
            btnPrev.TabIndex = 3;
            btnPrev.Text = "⏮";
            btnPrev.UseVisualStyleBackColor = true;
            // 
            // btnPlayPause
            // 
            btnPlayPause.Font = new Font("Segoe UI Symbol", 18F, FontStyle.Bold);
            btnPlayPause.Location = new Point(218, 288);
            btnPlayPause.Name = "btnPlayPause";
            btnPlayPause.Size = new Size(53, 51);
            btnPlayPause.TabIndex = 4;
            btnPlayPause.Text = "▶";
            btnPlayPause.UseVisualStyleBackColor = true;
            // 
            // btnNext
            // 
            btnNext.Font = new Font("Segoe UI Symbol", 16F, FontStyle.Bold);
            btnNext.Location = new Point(289, 305);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(48, 45);
            btnNext.TabIndex = 5;
            btnNext.Text = "⏭";
            btnNext.UseVisualStyleBackColor = true;
            // 
            // lblTitle
            // 
            lblTitle.AutoEllipsis = true;
            lblTitle.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(258, 66);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(230, 65);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "音樂標題";
            // 
            // lblArtist
            // 
            lblArtist.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblArtist.Location = new Point(258, 131);
            lblArtist.Name = "lblArtist";
            lblArtist.Size = new Size(179, 50);
            lblArtist.TabIndex = 6;
            lblArtist.Text = "歌手名稱";
            // 
            // btnPlaylist
            // 
            btnPlaylist.Font = new Font("Segoe UI Symbol", 12F);
            btnPlaylist.Location = new Point(12, 12);
            btnPlaylist.Name = "btnPlaylist";
            btnPlaylist.Size = new Size(40, 40);
            btnPlaylist.TabIndex = 7;
            btnPlaylist.Text = "☰";
            btnPlaylist.UseVisualStyleBackColor = true;
            // 
            // timeLabel
            // 
            timeLabel.AutoSize = true;
            timeLabel.Location = new Point(40, 233);
            timeLabel.Name = "timeLabel";
            timeLabel.RightToLeft = RightToLeft.Yes;
            timeLabel.Size = new Size(39, 19);
            timeLabel.TabIndex = 8;
            timeLabel.Text = "0:00";
            // 
            // btnRewind
            // 
            btnRewind.Font = new Font("Segoe UI Symbol", 14F, FontStyle.Bold);
            btnRewind.Location = new Point(82, 307);
            btnRewind.Name = "btnRewind";
            btnRewind.Size = new Size(48, 45);
            btnRewind.TabIndex = 9;
            btnRewind.Text = "⏪";
            btnRewind.UseVisualStyleBackColor = true;
            // 
            // btnForward
            // 
            btnForward.Font = new Font("Segoe UI Symbol", 14F, FontStyle.Bold);
            btnForward.Location = new Point(357, 305);
            btnForward.Name = "btnForward";
            btnForward.Size = new Size(48, 45);
            btnForward.TabIndex = 10;
            btnForward.Text = "⏩";
            btnForward.UseVisualStyleBackColor = true;
            // 
            // Form_Music_player
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 380);
            Controls.Add(timeLabel);
            Controls.Add(btnPlaylist);
            Controls.Add(lblArtist);
            Controls.Add(btnNext);
            Controls.Add(btnPlayPause);
            Controls.Add(btnPrev);
            Controls.Add(progressBar);
            Controls.Add(lblTitle);
            Controls.Add(pictureBoxCover);
            Controls.Add(btnRewind);
            Controls.Add(btnForward);
            Name = "Form_Music_player";
            Text = "音樂播放器";
            ((System.ComponentModel.ISupportInitialize)pictureBoxCover).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxCover;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblArtist;
        private System.Windows.Forms.Button btnPlaylist;
        private Label timeLabel;
        
        private Button btnRewind;
        private Button btnForward;
    }
}