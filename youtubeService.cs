using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace LifeHelper
{
    internal class YoutubeService
    {
       
       

        /// <summary>
        /// yt-dlp 核心邏輯
        /// </summary>
        /// <param name="url"></param>
        /// <param name="playlistIndex"></param>
        /// <param name="progressCallback"></param>
        /// <returns></returns>
        public static async Task<SongInfo?> GetInfoAsync(string url, int playlistIndex, Action<int, string> progressCallback)
        {
            return await Task.Run(() =>
            {
                string itemArg = playlistIndex >= 1 ? $"--playlist-items {playlistIndex} " : "--no-playlist ";
                string template = "%(title)s@@%(uploader)s@@%(duration_string)s@@https://i.ytimg.com/vi/%(id)s/maxresdefault.jpg@@https://www.youtube.com/watch?v=%(id)s@@%(url)s";

                using var process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "yt-dlp.exe",
                    Arguments = $"-f \"140/bestaudio\" {itemArg}--print \"{template}\" --no-warning \"{url}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                string lastOutput = "";
                process.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) lastOutput = e.Data; };

                process.ErrorDataReceived += (s, e) => {
                    if (string.IsNullOrEmpty(e.Data)) return;
                    var match = Regex.Match(e.Data, @"(\d{1,3})%");
                    if (match.Success) progressCallback?.Invoke(int.Parse(match.Groups[1].Value), "解析中...");
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (process.ExitCode == 0 && lastOutput.Contains("@@"))
                {
                    var p = lastOutput.Split(new[] { "@@" }, StringSplitOptions.None);
                    return new SongInfo
                    {
                        Title = p[0],
                        Artist= p[1],
                        Duration = p[2],
                        CoverUrl = p[3],
                        OriginUrl = p[4],
                        StreamUrl = p[5]
                    };
                }
                return null;
            });
        }

        // 獲取歌單數量
        public static async Task<int> GetPlaylistCountAsync(string url)
        {
            return await Task.Run(() => {
                using var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "yt-dlp.exe",
                        Arguments = $"--flat-playlist --print \"%(playlist_count)s\" --playlist-items 1 \"{url}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                string output = proc.StandardOutput.ReadToEnd().Trim();
                proc.WaitForExit();
                return int.TryParse(output, out int count) ? count : (url.Contains("list=RD") ? 30 : 0);
            });
        }
        // 刷新 yt-dlp 連結
        public static async Task<string> RefreshUrl(string originUrl)
        {
            try
            {
                Debug.WriteLine($"[yt-dlp] 開始為 {originUrl} 抓取新連結...");
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "yt-dlp.exe",
                  
                    Arguments = $"--no-playlist --format \"bestaudio[ext=m4a]\" --no-cache-dir --get-url --no-warning \"{originUrl}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                using (var proc = System.Diagnostics.Process.Start(psi))
                {
                  
                    string output = await proc.StandardOutput.ReadToEndAsync();
                    string error = await proc.StandardError.ReadToEndAsync();
                    await proc.WaitForExitAsync();

                    if (!string.IsNullOrEmpty(error))
                        Debug.WriteLine($"[yt-dlp Error] {error}");

                    string url = output.Trim().Split('\n').LastOrDefault()?.Trim();
                    Debug.WriteLine($"[yt-dlp Result] 連結獲取成功，長度: {url?.Length ?? 0}");
                    return url;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Refresh Error] {ex.Message}");
                return null;
            }
        }
    }
}
