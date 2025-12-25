namespace LifeHelper
{
    partial class playListControl
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            button1 = new Button();
            comboBox1 = new ComboBox();
            button2 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new Point(12, 14);
            label1.Name = "label1";
            label1.Size = new Size(296, 28);
            label1.TabIndex = 0;
            label1.Text = "label1";
            // 
            // label2
            // 
            label2.Location = new Point(349, 14);
            label2.Name = "label2";
            label2.Size = new Size(162, 28);
            label2.TabIndex = 1;
            label2.Text = "label2";
            // 
            // button1
            // 
            button1.Location = new Point(534, 14);
            button1.Name = "button1";
            button1.Size = new Size(79, 29);
            button1.TabIndex = 2;
            button1.Text = "刪除";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btnDelete_Click;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(631, 15);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(137, 27);
            comboBox1.TabIndex = 3;
            // 
            // button2
            // 
            button2.Location = new Point(784, 14);
            button2.Name = "button2";
            button2.Size = new Size(76, 29);
            button2.TabIndex = 4;
            button2.Text = "加入";
            button2.UseVisualStyleBackColor = true;
            button2.Click += btnAddToLocal_Click;
            // 
            // playListControl
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(button2);
            Controls.Add(comboBox1);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "playListControl";
            Size = new Size(874, 60);
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Label label2;
        private Button button1;
        private ComboBox comboBox1;
        private Button button2;
    }
}
