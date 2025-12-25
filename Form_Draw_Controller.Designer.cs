namespace LifeHelper
{
    partial class Form_Draw_Controller
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnStart = new Button();
            btnClear = new Button();
            btnExit = new Button();
            colorDialog1 = new ColorDialog();
            螢幕狀態 = new TabControl();
            tabPage1 = new TabPage();
            button5 = new Button();
            button4 = new Button();
            tabPage2 = new TabPage();
            trackBar1 = new TrackBar();
            label1 = new Label();
            radioButton3 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            pictureBox1 = new PictureBox();
            tabPage3 = new TabPage();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            螢幕狀態.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tabPage3.SuspendLayout();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(6, 6);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(67, 29);
            btnStart.TabIndex = 0;
            btnStart.Text = "啟動";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(148, 6);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(66, 29);
            btnClear.TabIndex = 1;
            btnClear.Text = "清空";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(79, 6);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(63, 29);
            btnExit.TabIndex = 2;
            btnExit.Text = "退出";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // 螢幕狀態
            // 
            螢幕狀態.Controls.Add(tabPage1);
            螢幕狀態.Controls.Add(tabPage2);
            螢幕狀態.Controls.Add(tabPage3);
            螢幕狀態.Location = new Point(0, 1);
            螢幕狀態.Name = "螢幕狀態";
            螢幕狀態.SelectedIndex = 0;
            螢幕狀態.Size = new Size(698, 114);
            螢幕狀態.TabIndex = 4;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(button5);
            tabPage1.Controls.Add(button4);
            tabPage1.Controls.Add(btnStart);
            tabPage1.Controls.Add(btnExit);
            tabPage1.Controls.Add(btnClear);
            tabPage1.Location = new Point(4, 28);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(690, 82);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "螢幕控制";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Location = new Point(84, 47);
            button5.Name = "button5";
            button5.Size = new Size(73, 29);
            button5.TabIndex = 4;
            button5.Text = "下一步";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button4
            // 
            button4.Location = new Point(8, 47);
            button4.Name = "button4";
            button4.Size = new Size(70, 29);
            button4.TabIndex = 3;
            button4.Text = "上一步";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(trackBar1);
            tabPage2.Controls.Add(label1);
            tabPage2.Controls.Add(radioButton3);
            tabPage2.Controls.Add(radioButton2);
            tabPage2.Controls.Add(radioButton1);
            tabPage2.Controls.Add(pictureBox1);
            tabPage2.Location = new Point(4, 28);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(690, 82);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "畫筆控制";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(213, 14);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(118, 56);
            trackBar1.TabIndex = 5;
            trackBar1.Scroll += trackBar1_Scroll;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 6);
            label1.Name = "label1";
            label1.Size = new Size(0, 19);
            label1.TabIndex = 4;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(149, 14);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(45, 23);
            radioButton3.TabIndex = 3;
            radioButton3.Text = "面";
            radioButton3.UseVisualStyleBackColor = true;
            radioButton3.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(98, 14);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(45, 23);
            radioButton2.TabIndex = 2;
            radioButton2.Text = "線";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(47, 14);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(45, 23);
            radioButton1.TabIndex = 1;
            radioButton1.TabStop = true;
            radioButton1.Text = "點";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(6, 6);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(35, 31);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(button3);
            tabPage3.Controls.Add(button2);
            tabPage3.Controls.Add(button1);
            tabPage3.Location = new Point(4, 28);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(690, 82);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "輸入輸出";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(208, 6);
            button3.Name = "button3";
            button3.Size = new Size(94, 29);
            button3.TabIndex = 2;
            button3.Text = "新增文字";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Location = new Point(108, 6);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 1;
            button2.Text = "匯出截圖";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(8, 6);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 0;
            button1.Text = "匯入圖片";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form_Draw
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(701, 120);
            Controls.Add(螢幕狀態);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "Form_Draw";
            Text = "控制台";
            螢幕狀態.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tabPage3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button btnStart;
        private Button btnClear;
        private Button btnExit;
        private ColorDialog colorDialog1;
        private TabControl 螢幕狀態;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private PictureBox pictureBox1;
        private RadioButton radioButton3;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private TrackBar trackBar1;
        private Label label1;
        private Button button2;
        private Button button1;
        private Button button3;
        private Button button4;
        private Button button5;
    }
}
