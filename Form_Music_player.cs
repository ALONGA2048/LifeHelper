using NAudio.Wave;
using System.Diagnostics;
using System.Text;

namespace LifeHelper
{
    public partial class Form_Music_player : Form
    {

        private IWavePlayer waveOut;
        private MediaFoundationReader audioReader;
        private string streamUrl;
        private bool isPlaying = false;

        private bool isDragging = false;
        private int dragValue = 0;
        private bool isClosingManually = false;

        private int retryCount = 0;
        private const int maxRetry = 3;
        private DateTime lastPlayStartTime;

        // 主要所有歌曲清單
        public List<SongInfo> songList = new List<SongInfo>();
        private int currentIndex = 0;

        /// <summary>
        /// 撥放器建立邏輯
        /// </summary>
        /// <param name="songs"></param>
        /// <param name="startIndex"></param>
        public Form_Music_player(List<SongInfo> songs, int startIndex = 0)
        {
            InitializeComponent();
            songList = songs ?? new List<SongInfo>();
            currentIndex = startIndex >= 0 && startIndex < songList.Count ? startIndex : 0;
            if (songList.Count > 0)
            {
                SetMusicInfo(songList[currentIndex]);
            }
        }

        public async void SetMusicInfo(SongInfo info)
        {
            isPlaying = false;
            CleanupPlayback();

            string cleanedTitle = CleanTitle(info.Title, info.Artist); // 修正原本傳入 info.CoverUrl 的小 Bug
            lblTitle.Text = GetTwoLineEllipsis(cleanedTitle, lblTitle.Font, lblTitle.Width);
            lblArtist.Text = info.Artist;
            timeLabel.Text = $"0:00/{info.Duration}";
            this.streamUrl = info.StreamUrl;

            try
            {
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
                {
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(info.CoverUrl);
                    using (var ms = new System.IO.MemoryStream(imageBytes))
                    {
                        using (Image originalImage = Image.FromStream(ms))
                        {
                            // 執行美化加工：圓角處理
                            Image roundedImage = CreateRoundedImage(originalImage, 20); // 20 為圓角半徑

                            // 釋放舊圖並更新
                            pictureBoxCover.Image?.Dispose();
                            pictureBoxCover.Image = roundedImage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Image Error] {ex.Message}");
                pictureBoxCover.Image = null;
            }

            PlayMusic();
        }

        /// <summary>
        /// 產生圓角圖片的工具方法 (GDI+)
        /// </summary>
        private Image CreateRoundedImage(Image image, int cornerRadius)
        {
            // 建立畫布
            Bitmap result = new Bitmap(image.Width, image.Height);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);

                // --- 定義路徑 ---
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int arcSize = cornerRadius * 2;
                    path.AddArc(0, 0, arcSize, arcSize, 180, 90);
                    path.AddArc(image.Width - arcSize, 0, arcSize, arcSize, 270, 90);
                    path.AddArc(image.Width - arcSize, image.Height - arcSize, arcSize, arcSize, 0, 90);
                    path.AddArc(0, image.Height - arcSize, arcSize, arcSize, 90, 90);
                    path.CloseAllFigures();

                    // 1. 裁切並繪製圖片
                    g.SetClip(path);
                    g.DrawImage(image, 0, 0, image.Width, image.Height);
                    g.ResetClip();

                    // 2. [強化點] 繪製明顯的外圈邊框 (模擬專輯封面的厚紙板感)
                    // 使用較亮的灰色，寬度設為 3px
                    using (Pen borderPen = new Pen(Color.FromArgb(80, 80, 80), 3))
                    {
                        // 將畫筆對齊方式設為向內，這樣邊框才不會被切掉
                        borderPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                        g.DrawPath(borderPen, path);
                    }

                    // 3. [強化點] 最內層的高光細線 (增加精緻質感)
                    using (Pen highlightPen = new Pen(Color.FromArgb(120, 255, 255, 255), 1))
                    {
                        // 再向內縮一點點繪製高光
                        float offset = 2.5f;
                        g.TranslateTransform(offset, offset);
                        // 稍微縮小比例繪製高光路徑，或簡單畫個 1px 的內框
                        using (Pen innerPen = new Pen(Color.FromArgb(50, 255, 255, 255), 1))
                        {
                            // 這裡只畫外圍一圈
                            g.DrawPath(innerPen, path);
                        }
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// 按鈕事件處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnPrev_Click(object sender, EventArgs e)
        {
            DeleteTempFile();
            if (songList.Count == 0) return;
            currentIndex = (currentIndex - 1 + songList.Count) % songList.Count;
            SetMusicInfo(songList[currentIndex]);
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (waveOut == null && !string.IsNullOrEmpty(streamUrl))
            {
                PlayMusic();
            }
            else if (waveOut != null)
            {
                if (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    waveOut.Pause();
                    btnPlayPause.Text = "▶";
                    isPlaying = false;
                }
                else if (waveOut.PlaybackState == PlaybackState.Paused)
                {
                    waveOut.Play();
                    btnPlayPause.Text = "⏸";
                    isPlaying = true;
                }
            }
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            DeleteTempFile();
            if (songList.Count == 0) return;
            currentIndex = (currentIndex + 1) % songList.Count;
            SetMusicInfo(songList[currentIndex]);
        }
        private void btnPlaylist_Click(object sender, EventArgs e)
        {
            playListManager.ShowPlaylistForm(this, songList);
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            if (audioReader != null)
            {
                double newPos = audioReader.CurrentTime.TotalSeconds - 10;
                if (newPos < 0) newPos = 0;
                audioReader.CurrentTime = TimeSpan.FromSeconds(newPos);
            }
        }
        private void btnForward_Click(object sender, EventArgs e)
        {
            if (audioReader != null)
            {
                double newPos = audioReader.CurrentTime.TotalSeconds + 10;
                if (newPos > audioReader.TotalTime.TotalSeconds)
                    newPos = audioReader.TotalTime.TotalSeconds;
                audioReader.CurrentTime = TimeSpan.FromSeconds(newPos);
            }
        }

        /// <summary>
        ///  進度條計時器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void playbackTimer_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(playbackTimer_Tick), sender, e);
                return;
            }
            if (audioReader == null || isDragging) return;
            double pos = audioReader.CurrentTime.TotalSeconds;
            double total = audioReader.TotalTime.TotalSeconds;
            if (total > 0)
            {
                progressBar.Value = Math.Min(progressBar.Maximum, (int)(pos / total * progressBar.Maximum));
                timeLabel.Text = $"{FormatTime(pos)}/{FormatTime(total)}";
            }
        }

        private void progressBar_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            dragValue = GetProgressBarValueFromMouse(e.X);
            progressBar.Value = dragValue;
        }
        private void progressBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                dragValue = GetProgressBarValueFromMouse(e.X);
                progressBar.Value = dragValue;
            }
        }
        private void progressBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (audioReader != null && audioReader.TotalTime.TotalSeconds > 0)
            {
                double percent = (double)progressBar.Value / progressBar.Maximum;
                double newTime = percent * audioReader.TotalTime.TotalSeconds;
                audioReader.CurrentTime = TimeSpan.FromSeconds(newTime);
            }
            isDragging = false;
        }
        private int GetProgressBarValueFromMouse(int mouseX)
        {
            int width = progressBar.Width;
            double percent = Math.Max(0, Math.Min(1, (double)mouseX / width));
            return (int)(percent * progressBar.Maximum);
        }



        // 主要播放方法
        private void PlayMusic(TimeSpan? resume = null)
        {
            try
            {
                Debug.WriteLine($"[PlayMusic] 開始初始化播放程序. URL 長度: {streamUrl?.Length ?? 0}");


                CleanupPlayback();

                if (string.IsNullOrEmpty(streamUrl)) return;


                Debug.WriteLine("[PlayMusic] 正在建立 MediaFoundationReader...");
                audioReader = new MediaFoundationReader(streamUrl);

                Debug.WriteLine($"[PlayMusic] 串流總時長: {audioReader.TotalTime.TotalSeconds} 秒");

                waveOut = new WaveOutEvent();
                waveOut.Init(audioReader);

                if (resume.HasValue && resume.Value.TotalSeconds > 0)
                {
                    Debug.WriteLine($"[PlayMusic] 設定續播時間點: {resume.Value.TotalSeconds}");
                    audioReader.CurrentTime = resume.Value;
                }

                waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
                waveOut.Play();

                btnPlayPause.Text = "⏸";
                isPlaying = true;
                playbackTimer.Start();
                lastPlayStartTime = DateTime.Now;

                Debug.WriteLine("[PlayMusic] 播放指令已發送至硬體。");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PlayMusic Exception] 發生錯誤: {ex.Message}");
                HandlePlaybackError(ex);
            }
        }

        // 錯誤處理與重試邏輯
        private async void HandlePlaybackError(Exception ex)
        {
            if (retryCount < maxRetry)
            {
                retryCount++;
                Debug.WriteLine($"[Retry] 偵測到秒停，將在 2 秒後進行第 {retryCount} 次重試...");

                await Task.Delay(2000); // 給予伺服器緩衝時間

                string newUrl = await YoutubeService.RefreshUrl(songList[currentIndex].OriginUrl);
                if (!string.IsNullOrEmpty(newUrl))
                {
                    this.streamUrl = newUrl;
                    PlayMusic(TimeSpan.Zero);
                    return;
                }
            }

            Debug.WriteLine("[Final Failure] 重試多次失敗，跳過此歌曲。");
            PlayNextSong();
        }

        // 統一清理資源的方法
        private void CleanupPlayback()
        {
            if (waveOut != null)
            {
                Debug.WriteLine("[Cleanup] 正在釋放 waveOut...");
                waveOut.PlaybackStopped -= WaveOut_PlaybackStopped;
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
            if (audioReader != null)
            {
                Debug.WriteLine("[Cleanup] 正在釋放 audioReader...");
                audioReader.Dispose();
                audioReader = null;
            }
        }
        private void PlayNextSong()
        {
            if (songList.Count == 0) return;
            currentIndex = (currentIndex + 1) % songList.Count;
            SetMusicInfo(songList[currentIndex]);
        }



        /// <summary>
        /// fallback 方法：下載音訊到本地並播放
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task DownloadAndPlayLocalAsync(string url)
        {
            string tempPath = Path.Combine(Application.StartupPath, "temp.mp3");
            DeleteTempFile();
            using (Form loader = playListManager.CreateLoadingForm("正在下載 YouTube 音訊，請稍候..."))
            {
                loader.Show();
                loader.Refresh();

                try
                {

                    await Task.Run(async () =>
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = "yt-dlp.exe",

                            Arguments = $"-f 140/bestaudio[ext=m4a]/bestaudio --extract-audio --audio-format mp3 -o \"{tempPath}\" \"{url}\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                            StandardOutputEncoding = Encoding.UTF8
                        };

                        using (var proc = Process.Start(psi))
                        {
                            if (proc != null)
                            {

                                await proc.WaitForExitAsync();
                            }
                        }
                    });


                    loader.Close();

                    streamUrl = tempPath;
                    PlayMusic();
                }
                catch (Exception ex)
                {

                    loader.Close();
                    MessageBox.Show("下載本地檔案失敗：" + ex.Message, "錯誤");
                }
            }
        }

        private void DeleteTempFile()
        {
            string tempPath = Path.Combine(Application.StartupPath, "temp.mp3");
            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch { }
        }

        public void UpdateCurrentIndex(int index)
        {
            if (index >= 0 && index < songList.Count)
            {
                this.currentIndex = index;
                Debug.WriteLine($"[Jump] 索引已更新為: {index}，歌曲: {songList[index].Title}");
            }
        }
        private async void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            // 如果是手動關閉視窗 不進行後續處理
            if (isClosingManually)
            {
                return;
            }

            playbackTimer.Stop();
            double playedSeconds = (DateTime.Now - lastPlayStartTime).TotalSeconds;
            bool isRemoteStream = streamUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase);

            // 判斷是否為正常結束
            if (audioReader != null && audioReader.Position >= (audioReader.Length - 1024))
            {
                Debug.WriteLine("[Normal End] 歌曲播放完畢。");
                isPlaying = false;

                if (!isRemoteStream)
                {
                    DeleteTempFile();
                    Debug.WriteLine("[Normal End] 本地暫存檔播完，準備跳下一首。");
                }

                PlayNextSong();
                return;
            }

            // 判斷是否為人為停止

            if (!isPlaying) return;
            bool isAbnormal = (e.Exception != null) ||
                              (audioReader != null && audioReader.Position < audioReader.Length) ||
                              (isRemoteStream && playedSeconds < 3.0);

            if (isAbnormal)
            {

                if (!isRemoteStream)
                {
                    Debug.WriteLine("本地檔案播放失敗跳過歌曲");
                    PlayNextSong();
                    return;
                }

                Debug.WriteLine($"串流中斷  播放 {playedSeconds} 執行fallback");
                CleanupPlayback();

                // 執行 fallBack 下載並播放本地
                await DownloadAndPlayLocalAsync(songList[currentIndex].OriginUrl);
            }
        }
        //覆寫關閉邏輯
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            isClosingManually = true;
            DeleteTempFile();
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
            if (audioReader != null)
            {
                audioReader.Dispose();
                audioReader = null;
            }
            base.OnFormClosing(e);
        }




        // 其他字體處理方法
        private string CleanTitle(string title, string artist)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;
            string t = title.Trim();
            if (!string.IsNullOrWhiteSpace(artist))
            {
                string a = artist.Trim();
                t = t.Replace(a, "", StringComparison.OrdinalIgnoreCase);
            }
            // 移除特殊符號
            t = t.Replace("-", " ")
                 .Replace("–", " ")
                 .Replace("—", " ")
                 .Replace(":", " ")
                 .Replace("：", " ")
                 .Replace("|", " ")
                 .Replace("｜", " ")
                 .Replace("[", " ")
                 .Replace("]", " ");
            // 合併多個空白為一個
            t = System.Text.RegularExpressions.Regex.Replace(t, "\\s+", " ");
            return t.Trim();
        }

        private string GetTwoLineEllipsis(string text, Font font, int labelWidth)
        {
            using (var g = lblTitle.CreateGraphics())
            {
                int lineHeight = (int)g.MeasureString("A", font).Height;
                int maxHeight = lineHeight * 2;
                string result = text;
                SizeF size = g.MeasureString(result, font, labelWidth);
                if (size.Height <= maxHeight)
                    return result;

                // 超過兩行，逐字減少並加上 ...
                for (int i = text.Length - 1; i > 0; i--)
                {
                    string candidate = text.Substring(0, i) + "...";
                    size = g.MeasureString(candidate, font, labelWidth);
                    if (size.Height <= maxHeight)
                        return candidate;
                }
                return "...";
            }
        }
        private string FormatTime(double seconds)
        {
            int min = (int)seconds / 60;
            int sec = (int)seconds % 60;
            return $"{min}:{sec:D2}";
        }

        private void pictureBoxCover_Paint(object sender, PaintEventArgs e)
        {
            // 畫一個比圖片稍微大一點的深灰色圓角矩形作為底座
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, pictureBoxCover.Width - 1, pictureBoxCover.Height - 1);
            int r = 22; // 稍微比圖片圓角大一點

            using (System.Drawing.Drawing2D.GraphicsPath borderPath = new System.Drawing.Drawing2D.GraphicsPath())
            {
                borderPath.AddArc(rect.X, rect.Y, r, r, 180, 90);
                borderPath.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
                borderPath.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
                borderPath.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
                borderPath.CloseAllFigures();

                // 畫出一個明顯的實體外邊框 (深灰帶亮邊)
                using (Pen p = new Pen(Color.FromArgb(100, 100, 100), 2))
                {
                    e.Graphics.DrawPath(p, borderPath);
                }
            }
        }
    }

}