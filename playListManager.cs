using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace LifeHelper
{
    
    internal class playListManager
    {
        private const string PlaylistFolder = "Playlist";
        public static string GetFilePath(string playlistName)
            => Path.Combine(PlaylistFolder, $"{playlistName}.json");

        // 讀取歌單
        public static Dictionary<string, string> GetPlaylist(string playlistName)
        {
            string filePath = GetFilePath(playlistName);
            if (!File.Exists(filePath)) return new Dictionary<string, string>();

            try
            {
                string json = File.ReadAllText(filePath, Encoding.UTF8);
                
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
            catch { return new Dictionary<string, string>(); }
        }

        // 儲存歌單
        public static void SavePlaylist(string playlistName, Dictionary<string, string> data)
        {
            if (!Directory.Exists(PlaylistFolder)) Directory.CreateDirectory(PlaylistFolder);

            string filePath = GetFilePath(playlistName);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json,Encoding.UTF8);
        }

        // 加入歌曲至歌單 
        public static bool AddSongToPlaylist(string playlistName, string url, string title)
        {
            var dict = GetPlaylist(playlistName);
            if (dict.ContainsKey(url)) return false; 

            dict.Add(url, title);
            SavePlaylist(playlistName, dict);
            return true;
        }

        // 從歌單刪除歌曲
        public static void RemoveSongFromPlaylist(string playlistName, string url)
        {
            var dict = GetPlaylist(playlistName);
            if (dict.Remove(url))
            {
                SavePlaylist(playlistName, dict);
            }
        }
        // 建立空歌單
        public static void CreateEmptyPlaylist(string playlistName)
        {
           
            if (!Directory.Exists(PlaylistFolder))
                Directory.CreateDirectory(PlaylistFolder);

          
            string filePath = GetFilePath(playlistName);

           
            if (!File.Exists(filePath))
            {
                
                var emptyData = new Dictionary<string, string>();

                
                string json = JsonSerializer.Serialize(emptyData);

               
                File.WriteAllText(filePath, json);
            }
        }

        // 載入本地所有歌單名稱
        public static string[] LoadLocalPlaylists()
        {
            string playlistFolderPath = Path.Combine(Application.StartupPath, "Playlist");

            if (!Directory.Exists(playlistFolderPath))
            {
                Directory.CreateDirectory(playlistFolderPath);
            }

           
            string[] files = Directory.GetFiles(playlistFolderPath, "*.json")
                                      .Select(f => Path.GetFileNameWithoutExtension(f)!) 
                                      .ToArray();

            if (files.Length == 0) return new string[] { "null" };

            return files;
        }

        // 建立載入中視窗
        public static Form CreateLoadingForm(string message)
        {
            Form loader = new Form
            {
                Width = 300,
                Height = 120,
                Text = "請稍候",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ControlBox = false, 
                TopMost = true
            };

            System.Windows.Forms.Label lbl = new System.Windows.Forms.Label { Text = message, Top = 20, Left = 20, AutoSize = true };
            ProgressBar pb = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Width = 240,
                Height = 20,
                Top = 50,
                Left = 20
            };

            loader.Controls.Add(lbl);
            loader.Controls.Add(pb);
            return loader;
        }
        // 顯示當前player 歌單視窗
        public static void ShowPlaylistForm(Form parent, List<SongInfo> songList)
        {
           
            var player = parent as Form_Music_player;

            Form playlistForm = new Form();
            playlistForm.Text = "目前歌單內容";
            playlistForm.Size = new Size(650, 600);
            playlistForm.StartPosition = FormStartPosition.CenterParent;

            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 110; 
            topPanel.BackColor = Color.WhiteSmoke;
            topPanel.BorderStyle = BorderStyle.FixedSingle;

            
            var lblJump = new System.Windows.Forms.Label
            {
                Text = "快速跳轉歌曲:",
                Font = new Font("Microsoft JhengHei UI", 10, FontStyle.Bold),
                Location = new Point(15, 65), 
                AutoSize = true
            };

            ComboBox comboJump = new ComboBox
            {
                Location = new Point(130, 62),
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

           
            foreach (var s in songList)
            {
                comboJump.Items.Add(s.Title);
            }

            
            comboJump.SelectedIndexChanged += (s, ev) =>
            {
                int index = comboJump.SelectedIndex;
                if (index >= 0 && index < songList.Count && player != null)
                {
                    
                   
                    player.UpdateCurrentIndex(index);

                    
                    player.SetMusicInfo(songList[index]);

                    playlistForm.Close();
                }
            };
            
            var lblTitle = new System.Windows.Forms.Label
            {
                Text = "目標歌單:",
                Font = new Font("Microsoft JhengHei UI", 10, FontStyle.Bold),
                Location = new Point(15, 25),
                AutoSize = true
            };

            ComboBox comboTarget = new ComboBox
            {
                Location = new Point(130, 22),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

           
            string[] currentPlayList = playListManager.LoadLocalPlaylists();
            if (currentPlayList.Length > 0 && currentPlayList[0] != "null")
            {
                comboTarget.Items.AddRange(currentPlayList);
                comboTarget.SelectedIndex = 0;
            }

            var btnAddAll = new Button
            {
                Text = "全部轉入歌單",
                Location = new Point(500, 18),
                Width = 110,
                Height = 35,
                BackColor = Color.LightBlue
            };

            
            topPanel.Controls.Add(lblJump);
            topPanel.Controls.Add(comboJump);
            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(comboTarget);
            topPanel.Controls.Add(btnAddAll);

            
            Panel scrollPanel = new Panel();
            scrollPanel.Dock = DockStyle.Fill;
            scrollPanel.AutoScroll = true;

           
            for (int i = songList.Count - 1; i >= 0; i--)
            {
                var song = songList[i];
                SongItemControl item = new SongItemControl(song, i, currentPlayList);
                item.Dock = DockStyle.Top;
                item.Height = 60;
                scrollPanel.Controls.Add(item);
                item.PlaylistNeedsRefresh += (s, ev) =>
                {
                    playlistForm.Close();
                   
                    parent.BeginInvoke(new Action(() => ShowPlaylistForm(parent, songList)));
                };
            }

            
            playlistForm.Controls.Add(scrollPanel);
            playlistForm.Controls.Add(topPanel);

            btnAddAll.Click += (s, ev) => {
                string targetName = comboTarget.Text.Trim();

               
                if (string.IsNullOrEmpty(targetName) || targetName == "[新增歌單...]")
                {
                    MessageBox.Show("請先選擇一個有效的目標歌單。");
                    return;
                }

                try
                {
                    
                    var playlistDict = playListManager.GetPlaylist(targetName);

                    int initialCount = playlistDict.Count;
                    int totalSongs = songList.Count;

                    
                    foreach (var song in songList)
                    {
                       
                        if (!playlistDict.ContainsKey(song.OriginUrl))
                        {
                            playlistDict.Add(song.OriginUrl, song.Title);
                        }
                    }

                    int addedCount = playlistDict.Count - initialCount;
                    int skippedCount = totalSongs - addedCount;

                  
                    playListManager.SavePlaylist(targetName, playlistDict);

                    
                    MessageBox.Show($"成功處理 {totalSongs} 首歌曲！\n" +
                                    $"新加入：{addedCount} 首\n" +
                                    $"重複跳過：{skippedCount} 首", "轉入完成");


                }
                catch (Exception ex)
                {
                    MessageBox.Show($"批次轉入失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            playlistForm.ShowDialog();
        }

        // 載入歌單視窗
        public static void LoadPlaylist(Form_Music mainForm,Form parent,MusicProgressUI ui,string name,List<SongInfo> allsongList)
        {

            Form? oldForm = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f.Text == $"歌單：{name}");
            if (oldForm != null)
            {
                oldForm.Close();
                oldForm.Dispose(); 
                Application.DoEvents(); 
            }
            Dictionary<string, string> entries = playListManager.GetPlaylist(name);

            
            Form playlistForm = new Form();
            playlistForm.Text = $"歌單：{name}";
            playlistForm.Size = new Size(730, 600);

            
            var panelTop = new Panel { Dock = DockStyle.Top, Height = 50 };
            var lblTitle = new Label { Text = "歌曲", Width = 250, Location = new Point(10, 15) };
            var lblUrl = new Label { Text = "歌曲ID", Width = 100, Location = new Point(270, 15) };
            var btnDownloadAll = new Button { Text = "全部下載", Location = new Point(390, 10), Width = 90, Height = 30 };
            var btnPlayAll = new Button { Text = "全部撥放", Location = new Point(490, 10), Width = 90, Height = 30 };
            var btnDeleteAll = new Button { Text = "全部刪除", Location = new Point(590, 10), Width = 90, Height = 30 };

            panelTop.Controls.Add(lblTitle);
            panelTop.Controls.Add(lblUrl);
            panelTop.Controls.Add(btnDownloadAll);
            panelTop.Controls.Add(btnDeleteAll);
            panelTop.Controls.Add(btnPlayAll);
            
            var scrollPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown, 
                WrapContents = false 
            };

            int y = 0; 
            string[] songList = playListManager.LoadLocalPlaylists();
            foreach (var entry in entries)
            {
                var ctrl = new playListControl(entry.Value, entry.Key, songList, name);

               
                ctrl.Location = new Point(0, y);

                ctrl.PlaylistNeedsRefresh += (s, e) => {
                    parent.BeginInvoke(new Action(() => LoadPlaylist(mainForm,parent,ui,name,allsongList)));
                    
                };

                scrollPanel.Controls.Add(ctrl);

              
                y += ctrl.Height + 5;
            }

           
            btnDeleteAll.Click += (s, e) =>
            {
                if (MessageBox.Show("確定要刪除所有歌曲？", "警告", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    File.WriteAllText(playListManager.GetFilePath(name), "[]", Encoding.UTF8);
                    playlistForm.Close();
                    
                    if (Application.OpenForms["Form_Music"] is Form_Music mainForm)
                    {
                        mainForm.InitPlaylistDrawer();
                    }
                }
            };

            
            btnDownloadAll.Click += async (s, e) =>
            {
                ui.ResetCancellation?.Invoke();
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        string targetFolder = fbd.SelectedPath;
                        int total = entries.Count;
                        if (total == 0) return;

                        playlistForm.Close();
                        ui.Update(0, "準備下載...", "0%");

                        int currentIndex = 0; 
                        foreach (var entry in entries)
                        {
                            if (ui.CheckCancellation?.Invoke() == true)
                            {
                                ui.Update(0, "下載已取消。", "0%");
                                return;
                            }

                            currentIndex++;
                            string outputPath = Path.Combine(targetFolder, "%(title)s.%(ext)s");

                            
                            ui.Update(ui.ProgressBar.Value, $"正在下載 ({currentIndex}/{total})", ui.PercentageLabel.Text);

                            await Task.Run(() =>
                            {
                                var psi = new ProcessStartInfo
                                {
                                    FileName = "yt-dlp.exe",
                                    Arguments = $"-x --audio-format mp3 -o \"{outputPath}\" \"{entry.Key}\"",
                                    UseShellExecute = false,
                                    CreateNoWindow = true
                                };
                                try
                                {
                                    using (var process = Process.Start(psi)) { process.WaitForExit(); }
                                }
                                catch (Exception ex) { Debug.WriteLine($"下載錯誤: {ex.Message}"); }
                            });

                           
                            int progressPercent = (int)((double)currentIndex / total * 100);
                            ui.Update(progressPercent, $"正在下載 ({currentIndex}/{total})首", $"{progressPercent}%");
                        }
                        ui.Update(100, "下載完成", "100%");
                    }
                }
            };


            
            btnPlayAll.Click += async (s, e) =>
            {
                ui.ResetCancellation?.Invoke();
                int total = entries.Count;
                if (total == 0) return;

                playlistForm.Close();
               
                

                ui.Update(0, "準備中...", "0%");

                int i = 0;
                foreach (var entry in entries)
                {
                    if (ui.CheckCancellation?.Invoke() == true)
                    {
                        ui.Update(0, "載入已取消。", "0%");
                        return;
                    }

                    int songIndex = i + 1;

                   
                    var info = await mainForm.GetYoutubeInfoAsync(entry.Key, songIndex, (percent, percentStr, line) =>
                    {
                        parent.BeginInvoke(new Action(() =>
                        {
                            double overallProgress = (i + (percent / 100.0)) / total;
                            int progressPercent = (int)(overallProgress * 100);
                            ui.ProgressBar.Value = Math.Min(ui.ProgressBar.Maximum, progressPercent);
                            ui.PercentageLabel.Text = $"{progressPercent}%";
                            ui.StatusLabel.Text = $"解析中 {songIndex}/{total}";
                        }));
                    });

                    if (info != null)
                    {
                        allsongList.Add(info);

                        if (i == 1)
                        {
                            parent.BeginInvoke(new Action(() => {
                                Form_Music.player = new Form_Music_player(allsongList, 0);
                                Form_Music.player.Show();
                            }));
                        }
                    }
                    i++;
                }

                ui.Update(100, "全部解析完成", "100%");

               
                if (Form_Music.player == null && allsongList.Count > 0)
                {
                    parent.BeginInvoke(new Action(() => {
                        Form_Music.player = new Form_Music_player(allsongList, 0);
                        Form_Music.player.Show();
                    }));
                }
            };
            playlistForm.Controls.Add(scrollPanel);
            playlistForm.Controls.Add(panelTop);
            playlistForm.ShowDialog();
        }
    }
}
