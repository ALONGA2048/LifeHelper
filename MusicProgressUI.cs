
namespace LifeHelper
{
    // 音樂下載/解析進度 UI 包裝類別
    public class MusicProgressUI
    {
        public Control Owner { get; set; } // 用於執行 Invoke 的視窗
        public ProgressBar ProgressBar { get; set; }
        public Control StatusLabel { get; set; } 
        public Control PercentageLabel { get; set; } 

        public Func<bool> CheckCancellation { get; set; }

        public Action ResetCancellation { get; set; }
        // 統一處理安全更新
        public void Update(int progress, string status, string percent)
        {
            if (Owner != null && Owner.InvokeRequired)
            {
                Owner.Invoke(new Action(() => UpdateUI(progress, status, percent)));
            }
            else
            {
                UpdateUI(progress, status, percent);
            }
        }

        private void UpdateUI(int progress, string status, string percent)
        {
            if (ProgressBar is MaterialSkin.Controls.MaterialProgressBar p) p.Value = progress;
            if (StatusLabel != null) StatusLabel.Text = status;
            if (PercentageLabel != null) PercentageLabel.Text = percent;
        }
    }

}
