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
        private System.Windows.Forms.Timer playbackTimer = new System.Windows.Forms.Timer();
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

            string cleanedTitle = CleanTitle(info.Title, info.CoverUrl);
            lblTitle.Text = GetTwoLineEllipsis(cleanedTitle, lblTitle.Font, lblTitle.Width);
            lblArtist.Text = info.Artist;
            timeLabel.Text = $"0:00/{info.Duration}";
            this.streamUrl = info.StreamUrl;

            try
            {
                System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
               
                byte[] imageBytes = await httpClient.GetByteArrayAsync(info.CoverUrl);
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    
                    pictureBoxCover.Image?.Dispose();
                    pictureBoxCover.Image = Image.FromStream(ms);
                }
            }
            catch
            {
                pictureBoxCover.Image = null;
            }

            PlayMusic();
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
            playListManager.ShowPlaylistForm(this,songList);
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

    }

}