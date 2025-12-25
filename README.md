# LifeHelper

LifeHelper 是一個基於 **C# .NET 8.0** 開發的 Windows 桌面工具程式。旨在提供便捷的螢幕互動與多媒體體驗。

## 🚀 主要功能

### 1. 螢幕繪圖 (Screen Drawing)
- **即時標註**：允許使用者直接在螢幕上進行繪圖、書寫與標註。
- **多功能畫布**：支援多種畫筆顏色與粗細調整。
- **透明圖層**：基於透明視窗技術，不影響後台程式運作。

### 2. 音樂串流播放 (Music Streaming)
- **YouTube 整合**：利用 `yt-dlp` 解析技術，支援 YouTube 歌曲搜尋與串流播放。
- **本地歌單管理**：支援 JSON 格式的歌單儲存與分類管理。
- **高品質播放**：整合 `NAudio` 引擎，提供穩定且流暢的音訊輸出。
- **Fallback 機制**：當串流不穩時自動下載暫存檔，確保播放不中斷。

## 🛠️ 技術棧
- **開發環境**：Visual Studio 2022
- **框架**：.NET 8.0 (Windows Forms)
- **第三方套件**：
  - `MaterialSkin.2`：精美的 Material Design UI 介面。
  - `NAudio`：專業音訊處理與播放。
  - `yt-dlp`：強大的影片/音訊解析工具。

## 📦 如何使用
1. 前往 [Releases](../../releases) 頁面下載最新版本的 `LifeHelper.zip`。
2. 解壓縮後，確保 `yt-dlp.exe` 與 `ffmpeg.exe` 位於主程式目錄下。
3. 執行 `LifeHelper.exe` 即可開始使用。