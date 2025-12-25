

namespace LifeHelper
{
    public partial class playListControl: UserControl
    {
        //當歌單列表需要更新時觸發
        public event EventHandler PlaylistNeedsRefresh;
       
        public string Title { get; private set; }
        public string Url { get; private set; }

        private string playlistName;

        public playListControl(string title,string url, string[] currentPlayList, string playlistName)
        {
            InitializeComponent();
            comboBox1.Items.Clear();
            if (!currentPlayList[0].Equals("null"))
            {
                comboBox1.Items.AddRange(currentPlayList);
                comboBox1.SelectedIndex = 0;
            }
            this.Title = title;
            this.Url = url;
            
            this.playlistName = playlistName;
            Uri uri = new Uri(url);
            
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            

            label1.Text = title;
            label2.Text = query .Get("v") ?? url;
            UpdateComboBox(currentPlayList);

            
        }
        //更新選單
        public void UpdateComboBox(string[] playlists)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("[新增歌單...]"); 
            foreach (var p in playlists) comboBox1.Items.Add(p);

            if (comboBox1.Items.Count > 1)
                comboBox1.SelectedIndex = 1; 
            else
                comboBox1.SelectedIndex = 0;
        }
        //刪除按鈕事件
        private void btnDelete_Click(object sender, EventArgs e)
        {
            playListManager.RemoveSongFromPlaylist(playlistName, Url);
            PlaylistNeedsRefresh?.Invoke(this, EventArgs.Empty);
        }
        // 加入歌單按鈕事件
        private void btnAddToLocal_Click(object sender, EventArgs e)
        {
            string targetPlaylist = comboBox1.Text.Trim();

           
            if (targetPlaylist == "[新增歌單...]")
            {
                string newName = Microsoft.VisualBasic.Interaction.InputBox("請輸入新歌單名稱：", "建立歌單", "");
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    
                    playListManager.CreateEmptyPlaylist(newName);

                   
                    playListManager.AddSongToPlaylist(newName, Url,Title);
                    var mainForm = Form_Music.GetInstance();
                    if (mainForm != null)
                    {
                        mainForm.InitPlaylistDrawer();
                    }

                    PlaylistNeedsRefresh?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (playListManager.AddSongToPlaylist(targetPlaylist, Url, Title))
                {
                    MessageBox.Show($"已加入歌單：{targetPlaylist}");
                    
                }
                else
                {
                    MessageBox.Show("此歌曲已在歌單中。");
                }
            }
        }


    }
}
