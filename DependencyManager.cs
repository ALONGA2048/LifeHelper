using System.Diagnostics;
using System.IO.Compression;

namespace LifeHelper
{
    public static class DependencyManager
    {
        private static readonly string appPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string ytDlpPath = Path.Combine(appPath, "yt-dlp.exe");
        private static readonly string ffmpegPath = Path.Combine(appPath, "ffmpeg.exe");
        

        // 更新 UI 狀態
        public static async Task CheckAndUpdateAsync(Action<string> onProgress)
        {
            await CheckYtDlpAsync(onProgress);
            await CheckFFmpegAsync(onProgress);
            onProgress("所有環境組件檢查完畢 準備就緒");
        }


        // 檢查並下載 yt-dlp
        private static async Task CheckYtDlpAsync(Action<string> onProgress)
        {
            if (!File.Exists(ytDlpPath))
            {
                onProgress("正在下載 yt-dlp.exe...");
                try
                {
                    using var client = new HttpClient();
                    var bytes = await client.GetByteArrayAsync("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe");
                    await File.WriteAllBytesAsync(ytDlpPath, bytes);
                    onProgress("yt-dlp.exe 下載完成 ");
                }
                catch (Exception ex)
                {
                    onProgress($"yt-dlp 下載失敗: {ex.Message}");
                }
            }
            else
            {
                onProgress("正在檢查 yt-dlp 更新...");
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = ytDlpPath,
                        Arguments = "-U", 
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    };
                    using var process = Process.Start(psi);
                    if (process != null)
                    {
                        await process.WaitForExitAsync();
                    }
                    onProgress("yt-dlp 更新完成。");
                }
                catch (Exception ex)
                {
                    onProgress($"yt-dlp 更新失敗: {ex.Message}");
                }
            }
        }


        // 檢查並下載 FFmpeg
        private static async Task CheckFFmpegAsync(Action<string> onProgress)
        {
           
            if (!File.Exists(ffmpegPath))
            {
                onProgress("正在下載 FFmpeg 環境 (檔案較大 請稍候)...");
                string zipPath = Path.Combine(appPath, "ffmpeg.zip");

                try
                {
                    using var client = new HttpClient();
                    string downloadUrl = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip";

                    using (var stream = await client.GetStreamAsync(downloadUrl))
                    using (var fs = new FileStream(zipPath, FileMode.Create))
                    {
                        await stream.CopyToAsync(fs);
                    }

                    onProgress("解壓縮 FFmpeg...");
                    using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (entry.FullName.EndsWith("ffmpeg.exe", StringComparison.OrdinalIgnoreCase))
                            {
                                entry.ExtractToFile(ffmpegPath, true);
                            }
                            
                        }
                    }

                  
                    if (File.Exists(zipPath)) File.Delete(zipPath);
                    onProgress("FFmpeg 下載與配置完成。");
                }
                catch (Exception ex)
                {
                    onProgress($"FFmpeg 下載或解壓失敗: {ex.Message}");
                }
            }
            else
            {
                onProgress("FFmpeg 核心已存在，跳過下載。");
            }
        }
    }
}