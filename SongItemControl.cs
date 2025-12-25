using System.Diagnostics;
using System.Text;

namespace LifeHelper
{
    public partial class SongItemControl : UserControl
    {
        
        public event EventHandler PlaylistNeedsRefresh;
        
        public SongInfo SongData { get; private set; }

        public SongItemControl(SongInfo song,int index, string[] currentPlayList)
        {
            InitializeComponent();
            playListcomboBox.Items.Clear();
            if (!currentPlayList[0].Equals("null")) {
                playListcomboBox.Items.AddRange(currentPlayList);
                playListcomboBox.SelectedIndex = 0;
            }
            this.SongData = song;
            
            
            int finalIndex = index + 1;
            lblSongTitle.Text = "曲目: "+ finalIndex+" " +song.Title;
            UpdateComboBox(currentPlayList);

        }
        // 讓父視窗可以從外部呼叫來更新選單
        public void UpdateComboBox(string[] playlists)
        {
            playListcomboBox.Items.Clear();
            playListcomboBox.Items.Add("[新增歌單...]"); 
            foreach (var p in playlists) playListcomboBox.Items.Add(p);

            if (playListcomboBox.Items.Count > 1)
                playListcomboBox.SelectedIndex = 1; 
            else
                playListcomboBox.SelectedIndex = 0;
        }
        private void btnDownload_Click(object sender, EventArgs e)
        {
            
            _ = HandleSingleDownload();
           
            


        }
        private async Task HandleSingleDownload()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "MP3 Audio|*.mp3";
            sfd.FileName = SongData.Title; 

            if (sfd.ShowDialog() == DialogResult.OK)
            {

                // 注意：yt-dlp 轉檔時會先產生 temporary 檔案，最後才變成指定檔名
                // 這裡路徑要把 .mp3 去掉，因為 yt-dlp 加入 --audio-format mp3 會自動補副檔名
                string finalPath = sfd.FileName.Replace(".mp3", "");
                await DownloadYoutubeAudioAsync(SongData.OriginUrl, finalPath + ".%(ext)s");


            }
        }

        private void btnAddToLocal_Click(object sender, EventArgs e)
        {
            string targetPlaylist = playListcomboBox.Text.Trim();

            
            if (targetPlaylist == "[新增歌單...]")
            {
                string newName = Microsoft.VisualBasic.Interaction.InputBox("請輸入新歌單名稱：", "建立歌單", "");
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    
                    playListManager.CreateEmptyPlaylist(newName);

                   
                    playListManager.AddSongToPlaylist(newName, SongData.OriginUrl, SongData.Title);
                    var mainForm = Form_Music.GetInstance();
                    if (mainForm != null)
                    {
                        mainForm.InitPlaylistDrawer();
                    }

                    // 通知介面更新
                    PlaylistNeedsRefresh?.Invoke(this, EventArgs.Empty);
                }
            }
            else {
                if (playListManager.AddSongToPlaylist(targetPlaylist, SongData.OriginUrl, SongData.Title))
                {
                    MessageBox.Show($"已加入歌單：{targetPlaylist}");
                   
                    PlaylistNeedsRefresh?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("此歌曲已在歌單中。");
                }
            }

            
        }
        // 在歌單中下載方法
        private async Task DownloadYoutubeAudioAsync(string url, string outputPath)
        {
           
            using (Form loader = playListManager.CreateLoadingForm("正在下載 YouTube 音訊，請稍候..."))
            {
                loader.Show();
                loader.Refresh();

                try
                {
                    
                    await Task.Run(() =>
                    {
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "yt-dlp.exe",
                              
                                Arguments = $"-x --audio-format mp3 -o \"{outputPath}\" \"{url}\"",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true,
                                StandardOutputEncoding = Encoding.UTF8
                            }
                        };

                        process.Start();

                       
                        string output = process.StandardOutput.ReadToEnd();

                        process.WaitForExit();
                    });

                    
                    loader.Close();
                    MessageBox.Show("下載完成！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    loader.Close();
                    Debug.WriteLine($"[Download Error] {ex.Message}");
                    MessageBox.Show($"下載失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
       
    }
}
