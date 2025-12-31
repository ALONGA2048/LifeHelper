namespace LifeHelper
{
    partial class Form_Music_player
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
            components = new System.ComponentModel.Container();
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
            playbackTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pictureBoxCover).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxCover
            // 
            pictureBoxCover.BackColor = Color.FromArgb(30, 30, 30);
            pictureBoxCover.Location = new Point(35, 70);
            pictureBoxCover.Name = "pictureBoxCover";
            pictureBoxCover.Size = new Size(380, 214);
            pictureBoxCover.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxCover.TabIndex = 0;
            pictureBoxCover.TabStop = false;
            pictureBoxCover.Paint += pictureBoxCover_Paint;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(45, 438);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(360, 4);
            progressBar.TabIndex = 4;
            progressBar.MouseDown += progressBar_MouseDown;
            progressBar.MouseMove += progressBar_MouseMove;
            progressBar.MouseUp += progressBar_MouseUp;
            // 
            // btnPrev
            // 
            btnPrev.BackColor = Color.Transparent;
            btnPrev.Cursor = Cursors.Hand;
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 50);
            btnPrev.FlatStyle = FlatStyle.Flat;
            btnPrev.Font = new Font("Segoe UI Symbol", 20F);
            btnPrev.ForeColor = Color.White;
            btnPrev.Location = new Point(125, 490);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(55, 55);
            btnPrev.TabIndex = 6;
            btnPrev.Text = "⏮";
            btnPrev.UseVisualStyleBackColor = false;
            btnPrev.Click += btnPrev_Click;
            // 
            // btnPlayPause
            // 
            btnPlayPause.BackColor = Color.Transparent;
            btnPlayPause.Cursor = Cursors.Hand;
            btnPlayPause.FlatAppearance.BorderSize = 0;
            btnPlayPause.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60);
            btnPlayPause.FlatStyle = FlatStyle.Flat;
            btnPlayPause.Font = new Font("Segoe UI Symbol", 28F);
            btnPlayPause.ForeColor = Color.White;
            btnPlayPause.Location = new Point(185, 475);
            btnPlayPause.Name = "btnPlayPause";
            btnPlayPause.Size = new Size(80, 80);
            btnPlayPause.TabIndex = 7;
            btnPlayPause.Text = "▶";
            btnPlayPause.UseVisualStyleBackColor = false;
            btnPlayPause.Click += btnPlayPause_Click;
            // 
            // btnNext
            // 
            btnNext.BackColor = Color.Transparent;
            btnNext.Cursor = Cursors.Hand;
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 50);
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.Font = new Font("Segoe UI Symbol", 20F);
            btnNext.ForeColor = Color.White;
            btnNext.Location = new Point(270, 490);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(55, 55);
            btnNext.TabIndex = 8;
            btnNext.Text = "⏭";
            btnNext.UseVisualStyleBackColor = false;
            btnNext.Click += btnNext_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoEllipsis = true;
            lblTitle.Font = new Font("Microsoft JhengHei UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(25, 310);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(400, 45);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "YouTube 影片標題";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblArtist
            // 
            lblArtist.Font = new Font("Microsoft JhengHei UI", 11F);
            lblArtist.ForeColor = Color.DarkGray;
            lblArtist.Location = new Point(25, 355);
            lblArtist.Name = "lblArtist";
            lblArtist.Size = new Size(400, 30);
            lblArtist.TabIndex = 2;
            lblArtist.Text = "頻道名稱";
            lblArtist.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnPlaylist
            // 
            btnPlaylist.FlatAppearance.BorderSize = 0;
            btnPlaylist.FlatStyle = FlatStyle.Flat;
            btnPlaylist.ForeColor = Color.Gray;
            btnPlaylist.Location = new Point(398, 12);
            btnPlaylist.Name = "btnPlaylist";
            btnPlaylist.Size = new Size(40, 40);
            btnPlaylist.TabIndex = 0;
            btnPlaylist.Text = "☰";
            btnPlaylist.Click += btnPlaylist_Click;
            // 
            // timeLabel
            // 
            timeLabel.Font = new Font("Consolas", 10F);
            timeLabel.ForeColor = Color.Gray;
            timeLabel.Location = new Point(45, 410);
            timeLabel.Name = "timeLabel";
            timeLabel.Size = new Size(360, 25);
            timeLabel.TabIndex = 3;
            timeLabel.Text = "00:00 / 00:00";
            timeLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btnRewind
            // 
            btnRewind.BackColor = Color.Transparent;
            btnRewind.FlatAppearance.BorderSize = 0;
            btnRewind.FlatStyle = FlatStyle.Flat;
            btnRewind.Font = new Font("Segoe UI Symbol", 14F);
            btnRewind.ForeColor = Color.Gray;
            btnRewind.Location = new Point(70, 495);
            btnRewind.Name = "btnRewind";
            btnRewind.Size = new Size(50, 50);
            btnRewind.TabIndex = 5;
            btnRewind.Text = "⏪";
            btnRewind.UseVisualStyleBackColor = false;
            btnRewind.Click += btnRewind_Click;
            // 
            // btnForward
            // 
            btnForward.BackColor = Color.Transparent;
            btnForward.FlatAppearance.BorderSize = 0;
            btnForward.FlatStyle = FlatStyle.Flat;
            btnForward.Font = new Font("Segoe UI Symbol", 14F);
            btnForward.ForeColor = Color.Gray;
            btnForward.Location = new Point(330, 495);
            btnForward.Name = "btnForward";
            btnForward.Size = new Size(50, 50);
            btnForward.TabIndex = 9;
            btnForward.Text = "⏩";
            btnForward.UseVisualStyleBackColor = false;
            btnForward.Click += btnForward_Click;
            // 
            // playbackTimer
            // 
            playbackTimer.Interval = 500;
            playbackTimer.Tick += playbackTimer_Tick;
            // 
            // Form_Music_player
            // 
            BackColor = Color.FromArgb(18, 18, 18);
            ClientSize = new Size(450, 600);
            Controls.Add(btnPlaylist);
            Controls.Add(pictureBoxCover);
            Controls.Add(lblTitle);
            Controls.Add(lblArtist);
            Controls.Add(timeLabel);
            Controls.Add(progressBar);
            Controls.Add(btnRewind);
            Controls.Add(btnPrev);
            Controls.Add(btnPlayPause);
            Controls.Add(btnNext);
            Controls.Add(btnForward);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form_Music_player";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LifeHelper - Music Player";
            ((System.ComponentModel.ISupportInitialize)pictureBoxCover).EndInit();
            ResumeLayout(false);
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
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Button btnRewind;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Timer playbackTimer;
    }
}