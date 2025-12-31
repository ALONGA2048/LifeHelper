using MaterialSkin;
using MaterialSkin.Controls;
using System.Diagnostics; 
using System.Text;


namespace LifeHelper
{

    public partial class Form_Music : MaterialForm
    {
        private static Form_Music _instance;
        public static Form_Music_player player;
        public Process _currentDownloadProcess;
        private static List<SongInfo> songList = new List<SongInfo>();
        private readonly MaterialSkinManager materialSkinManager;
        private bool _isCancelling = false;
       
        /// <summary>
        /// 初始建構邏輯
        /// </summary>
        public Form_Music()
        {
            InitializeComponent();
            
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK; // 改為深色模式
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Grey900,   // 主要背景 (更深)
                Primary.Grey800,   // 次要背景
                Primary.Grey500,   // 邊框/細節
                Accent.Pink200,    // 強調色 (可保持 Pink 或改用綠色)
                TextShade.WHITE    // 文字顏色
            );

        }
        private void Form_Music_Load(object sender, EventArgs e)
        {
            label1.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Regular);
            label2.Font = new Font("Microsoft JhengHei UI", 15F, FontStyle.Bold);
            InitPlaylistDrawer();
        }



        // 抓取 YouTube 總資料
        public async Task<SongInfo?> GetYoutubeInfoAsync(string url, int playlistItemIndex, Action<int, string, string> progressCallback)
        {
            
            string itemArg = playlistItemIndex >= 1 ? $"--playlist-items {playlistItemIndex} " : "";
            string printTemplate = "%(title)s" +
                                   "@@%(uploader)s" +
                                   "@@%(duration_string)s" +
                                   "@@https://i.ytimg.com/vi/%(id)s/maxresdefault.jpg" +
                                   "@@https://www.youtube.com/watch?v=%(id)s" +
                                   "@@%(url)s";

            return await Task.Run(() =>
            {
                
                using var process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "yt-dlp.exe",
                    Arguments = $"-f \"140/bestaudio[ext=m4a]/bestaudio\" {itemArg}--print {printTemplate} --no-warning \"{url}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                _currentDownloadProcess = process;
                string lastOutputLine = "";

                
                process.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        lastOutputLine = e.Data;
                        progressCallback?.Invoke(0, "解析中...", e.Data);
                    }
                };

              
                process.ErrorDataReceived += (s, e) =>
                {
                    if (string.IsNullOrEmpty(e.Data) || !e.Data.Contains("[download]")) return;

                    
                    var match = System.Text.RegularExpressions.Regex.Match(e.Data, @"(\d{1,3})%");
                    if (match.Success)
                    {
                        int p = int.Parse(match.Groups[1].Value);
                        progressCallback?.Invoke(p, $"{p}%", e.Data);
                    }
                };

                try
                {
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (process.ExitCode == 0 && !string.IsNullOrEmpty(lastOutputLine))
                    {
                        var parts = lastOutputLine.Split(new[] { "@@" }, StringSplitOptions.None);
                        if (parts.Length >= 6) 
                        {
                            return new SongInfo
                            {
                                Title = parts[0],
                                Artist = parts[1],
                                Duration = parts[2],
                                CoverUrl = parts[3],
                                OriginUrl = parts[4],
                                StreamUrl = parts[5]
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"yt-dlp 失敗: {ex.Message}");
                }
                finally
                {
                    _currentDownloadProcess = null;
                }
                return null;
            });
        }
        // 顯示播放器按鈕事件
        private void showUp_Click(object sender, EventArgs e)
        {
            
            if (songList == null || songList.Count == 0)
            {
                MessageBox.Show("目前沒有播放清單，請先加入歌曲！", "提示");
                return;
            }

            if (player == null || player.IsDisposed)
            {
                
                player = new Form_Music_player(songList, 0);
                player.Show();
            }
            else
            {
               
                player.songList = songList;
                if (!player.Visible) player.Show();
                player.Activate();
            }
            this.ActiveControl = null;
        }


        // 處理單一下載
        private async Task HandleSingleDownload(string url)
        {
           
            UpdateStatus("正在取得影片資訊...", 0);

            
            var info = await GetYoutubeInfoAsync(url, 0, (percent, percentStr, line) =>
            {
                UpdateStatus("解析資訊中...", percent);
            });

            if (info == null)
            {
                UpdateStatus("解析失敗", 0);
                return;
            }

           
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "MP3 Audio|*.mp3",
                FileName = info.Title
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                UpdateStatus("準備下載...", 0);

                string finalPath = sfd.FileName.Replace(".mp3", "");
                await DownloadYoutubeAudioAsync(url, finalPath + ".%(ext)s");

                
                UpdateStatus("下載完成", 100);
            }
            else
            {
                UpdateStatus("已取消下載", 0);
            }
        }

        
        // 處理歌單下載
        private async Task HandlePlaylistDownload(string url)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "請選擇儲存歌單的資料夾";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string targetFolder = fbd.SelectedPath;
                    int total = await GetPlaylistCountAsync(url);

                    for (int i = 1; i <= total; i++)
                    {
                        if (_isCancelling)
                        {
                            label1.Text = "下載已取消。";
                            label2.Text = $"0%";
                            materialProgressBar1.Value = 0;
                            break;
                        }
                        this.Invoke(new Action(() =>
                        {
                            int progressPercent = (int)((double)i / total * 100);
                            label2.Text = $"{progressPercent}%";
                            label1.Text = $"正在下載({i}/{total})首";
                            materialProgressBar1.Value = progressPercent;
                        }));

                        // 下載至指定資料夾，檔名使用 YouTube 標題
                        string outputPath = Path.Combine(targetFolder, "%(title)s.%(ext)s");

                        // 呼叫 yt-dlp 下載清單中的特定項目
                        await Task.Run(() =>
                        {
                            var psi = new ProcessStartInfo
                            {
                                FileName = "yt-dlp.exe",
                                Arguments = $"-x --audio-format mp3 --playlist-items {i} -o \"{outputPath}\" \"{url}\"",
                                CreateNoWindow = true,
                                UseShellExecute = false
                            };
                            Process.Start(psi).WaitForExit();
                        });
                    }
                    if (!_isCancelling)
                    {
                        MessageBox.Show($"歌單下載完成！儲存於：{targetFolder}");
                    }
                    label1.Text = "下載完成";
                }
            }
        }

        // 在歌單中下載方法
        private async Task DownloadYoutubeAudioAsync(string url, string outputPath)
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

                process.WaitForExit();
            });
        }



        // 執行按鈕
        private void btnPlay_Click(object sender, EventArgs e)
        {
            _ = BtnPlay_ClickAsync(sender, e);
            this.ActiveControl = null;
        }
        private async Task BtnPlay_ClickAsync(object sender, EventArgs e)
        {
            // 基本驗證與初始化
            _isCancelling = false;
            string url = txtYoutubeUrl.Text.Trim();
            if (!ValidateInputs(url)) return;

            ResetUI();
            songList.Clear();

            // 處理下載模式
            if (materialRadioButton2.Checked)
            {
                if (url.Contains("list")) await HandlePlaylistDownload(url);
                else await HandleSingleDownload(url);
                return;
            }

            //處理播放模式
            bool isPlaylist = url.Contains("list");
            int total = isPlaylist ? await GetPlaylistCountAsync(url) : 1;

            if (total <= 0)
            {
                MessageBox.Show("無法取得內容資訊。", "錯誤");
                return;
            }

            for (int i = 1; i <= total; i++)
            {
                if (_isCancelling) { UpdateStatus("載入已取消。", 0); break; }

                var info = await GetYoutubeInfoAsync(url, isPlaylist ? i : 0, (percent, s, l) =>
                {
                    
                    double progress = ((i - 1) + percent / 100.0) / total;
                    UpdateStatus($"解析中: {i}/{total}", (int)(progress * 100));
                });

                if (info != null)
                {
                    songList.Add(info);

                    // 取得第一首歌後就啟動
                    if (i == 1) EnsurePlayerStarted();
                }
            }

            if (!_isCancelling) UpdateStatus("歌單載入完成", 100);
        }

        private bool ValidateInputs(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("請輸入有效的連結。", "提示"); return false;
            }
            if (!materialRadioButton1.Checked && !materialRadioButton2.Checked)
            {
                MessageBox.Show("請選擇播放或下載模式。", "提示"); return false;
            }
            return true;
        }

        private void ResetUI()
        {
            materialProgressBar1.Value = 0;
            label1.Text = "";
            label2.Text = "0%";
        }

        private void UpdateStatus(string message, int percent)
        {
            this.Invoke(new Action(() => {
                label1.Text = message;
                label2.Text = $"{percent}%";
                materialProgressBar1.Value = Math.Min(100, percent);
            }));
        }
        private void EnsurePlayerStarted()
        {
            this.Invoke(new Action(() => {
                if (player == null || player.IsDisposed)
                {
                    player = new Form_Music_player(songList, 0);
                    player.Show();
                }
                else
                {
                    
                    player.songList = songList;
                    if (!player.Visible) player.Show();
                    player.Activate();
                }
            }));
        }

        private async Task<int> GetPlaylistCountAsync(string url)
        {
            return await Task.Run(() =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "yt-dlp.exe",
                        Arguments = $"--flat-playlist --playlist-items 1 --print \"%(playlist_count)s\" --no-warning \"{url}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8
                    }
                };
                try
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();
                    if (int.TryParse(output, out int count))
                        return count;
                    // 如果是混合清單 (NA) 回傳一個固定數量
                    if (url.Contains("list=RD")) return 30;
                }
                catch { }
                return 0;
            });
        }

        // 停止按鈕事件
        private void btnStop_Click(object sender, EventArgs e)
        {
            _isCancelling = true;
            if (_currentDownloadProcess != null && !_currentDownloadProcess.HasExited)
            {
                try
                {
                    _currentDownloadProcess.Kill();
                }
                catch { }
                _currentDownloadProcess = null;
            }
            UpdateStatus("已停止", 0);
            
            songList.Clear();
        }

        // 更新清單列表
        public void InitPlaylistDrawer()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(InitPlaylistDrawer));
                return;
            }

            
            materialTabControl1.SelectedIndexChanged -= MaterialTabControl1_SelectedIndexChanged;

            
            string playlistFolder = Path.Combine(Application.StartupPath, "Playlist");
            if (!Directory.Exists(playlistFolder)) Directory.CreateDirectory(playlistFolder);

           
            materialTabControl1.TabPages.Clear();

           
            materialTabControl1.TabPages.Add(new TabPage("新增/刪除歌單"));

            
            string[] files = Directory.GetFiles(playlistFolder, "*.json");
            foreach (string file in files)
            {
                string playlistName = Path.GetFileNameWithoutExtension(file);
               
                if (playlistName != "新增/刪除歌單")
                {
                    materialTabControl1.TabPages.Add(new TabPage(playlistName));
                }
            }

            
            materialTabControl1.SelectedIndexChanged += MaterialTabControl1_SelectedIndexChanged;

           
            if (materialTabControl1.TabPages.Count > 0)
            {
                materialTabControl1.SelectedIndex = 0;
            }
        }




        // 歌單分頁切換事件
        private void MaterialTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialTabControl1.SelectedTab == null) return;

            string selectedText = materialTabControl1.SelectedTab.Text;


            if (selectedText == "新增/刪除歌單")
            {
                // 彈出管理歌單視窗
                var dialog = new Form();
                dialog.Text = "新增/刪除歌單";
                dialog.Size = new Size(350, 220);
                dialog.StartPosition = FormStartPosition.CenterParent;

                var lblAdd = new Label { Text = "新增歌單名稱:", Location = new Point(20, 30), AutoSize = true };
                var txtName = new TextBox { Location = new Point(120, 25), Width = 180 };
                var btnAdd = new Button { Text = "新增", Location = new Point(120, 60), Width = 80 };

                var lblDel = new Label { Text = "刪除歌單:", Location = new Point(20, 110), AutoSize = true };
                var comboDel = new ComboBox { Location = new Point(120, 105), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
                var btnDelete = new Button { Text = "刪除", Location = new Point(120, 140), Width = 80 };

                // 載入現有歌單
                string playlistFolder = Path.Combine(Application.StartupPath, "Playlist");
                if (!Directory.Exists(playlistFolder)) Directory.CreateDirectory(playlistFolder);
                string[] files = Directory.GetFiles(playlistFolder, "*.json");
                foreach (var file in files)
                {
                    comboDel.Items.Add(Path.GetFileNameWithoutExtension(file));
                }
                if (comboDel.Items.Count > 0) comboDel.SelectedIndex = 0;

                btnAdd.Click += (s, ev) =>
                {
                    string name = txtName.Text.Trim();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        MessageBox.Show("請輸入歌單名稱。", "錯誤");
                        return;
                    }
                    string path = Path.Combine(playlistFolder, name + ".json");
                    if (File.Exists(path))
                    {
                        MessageBox.Show("該歌單名稱已存在！", "錯誤");
                        return;
                    }
                    File.WriteAllText(path, "[]", Encoding.UTF8);
                    comboDel.Items.Add(name);
                    txtName.Clear();
                    InitPlaylistDrawer();
                    MessageBox.Show($"歌單「{name}」建立成功！", "提示");
                };

                btnDelete.Click += (s, ev) =>
                {
                    if (comboDel.SelectedItem == null)
                    {
                        MessageBox.Show("請先選擇要刪除的歌單。", "錯誤");
                        return;
                    }

                    string name = comboDel.SelectedItem.ToString();
                    if (name.Equals("playlist"))
                    {
                        MessageBox.Show("無法刪除Default歌單！", "錯誤");
                        return;
                    }
                    string path = Path.Combine(playlistFolder, name + ".json");
                    if (File.Exists(path))
                    {
                        if (MessageBox.Show($"確定要刪除歌單「{name}」？", "警告", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            File.Delete(path);
                            comboDel.Items.Remove(name);
                            InitPlaylistDrawer();
                            MessageBox.Show($"歌單「{name}」已刪除。", "提示");
                        }
                    }
                };

                dialog.Controls.Add(lblAdd);
                dialog.Controls.Add(txtName);
                dialog.Controls.Add(btnAdd);
                dialog.Controls.Add(lblDel);
                dialog.Controls.Add(comboDel);
                dialog.Controls.Add(btnDelete);
                dialog.ShowDialog();

                // 切回上一個有效分頁（避免停留在 新增歌單）

                materialTabControl1.SelectedIndex = 0;

            }
            else
            {
                MusicProgressUI myUI = new MusicProgressUI()
                {
                    Owner = this,
                    ProgressBar = materialProgressBar1,
                    StatusLabel = label1,
                    PercentageLabel = label2,
                    CheckCancellation = () => this._isCancelling,
                    ResetCancellation = () => { this._isCancelling = false; }
                };
                playListManager.LoadPlaylist(this, this, myUI, selectedText, songList);


            }
        }


        /// <summary>
        /// 視窗關閉事件改為隱藏視窗
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 點擊右上角的 X 
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; // 取消關閉動作
                this.Hide();     // 改為隱藏視窗
            }
            else
            {
                base.OnFormClosing(e);
            }
        }
        public static Form_Music GetInstance()
        {
            // 如果不存在 則建立新的
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new Form_Music();
            }
            return _instance;
        }

    }

}