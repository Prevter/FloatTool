namespace FloatToolGUI
{
    partial class Benchmark
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Benchmark));
            this.startBenchmarkBtn = new System.Windows.Forms.Button();
            this.submitScoreBtn = new System.Windows.Forms.Button();
            this.cpuNameLabel = new System.Windows.Forms.Label();
            this.threadCountLabel = new System.Windows.Forms.Label();
            this.versionLabel2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.speedLabel = new System.Windows.Forms.Label();
            this.benchmarkScoreboardLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Updater = new System.Windows.Forms.Timer(this.components);
            this.customProgressBar1 = new FloatToolGUI.CustomProgressBar();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.closeBtn = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.benchmarkThreadsNumericUpdown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.warningPic = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.updateBenchmarksButton = new System.Windows.Forms.Button();
            this.benchmarkScoreboardLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.benchmarkThreadsNumericUpdown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningPic)).BeginInit();
            this.SuspendLayout();
            // 
            // startBenchmarkBtn
            // 
            this.startBenchmarkBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.startBenchmarkBtn.FlatAppearance.BorderSize = 0;
            this.startBenchmarkBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startBenchmarkBtn.Font = new System.Drawing.Font("Inter", 14.25F);
            this.startBenchmarkBtn.ForeColor = System.Drawing.Color.White;
            this.startBenchmarkBtn.Location = new System.Drawing.Point(394, 255);
            this.startBenchmarkBtn.Name = "startBenchmarkBtn";
            this.startBenchmarkBtn.Size = new System.Drawing.Size(304, 39);
            this.startBenchmarkBtn.TabIndex = 1;
            this.startBenchmarkBtn.Text = "Начать бенчмарк";
            this.startBenchmarkBtn.UseVisualStyleBackColor = false;
            this.startBenchmarkBtn.Click += new System.EventHandler(this.startBenchmarkBtn_Click);
            // 
            // submitScoreBtn
            // 
            this.submitScoreBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.submitScoreBtn.Enabled = false;
            this.submitScoreBtn.FlatAppearance.BorderSize = 0;
            this.submitScoreBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.submitScoreBtn.Font = new System.Drawing.Font("Inter", 12F);
            this.submitScoreBtn.ForeColor = System.Drawing.Color.White;
            this.submitScoreBtn.Location = new System.Drawing.Point(484, 221);
            this.submitScoreBtn.Name = "submitScoreBtn";
            this.submitScoreBtn.Size = new System.Drawing.Size(214, 29);
            this.submitScoreBtn.TabIndex = 2;
            this.submitScoreBtn.Text = "Опубликовать";
            this.ToolTip.SetToolTip(this.submitScoreBtn, "Просьба не отправлять много запросов на сервер в связи с его скоростью");
            this.submitScoreBtn.UseVisualStyleBackColor = false;
            this.submitScoreBtn.Click += new System.EventHandler(this.submitScoreBtn_Click);
            // 
            // cpuNameLabel
            // 
            this.cpuNameLabel.AutoSize = true;
            this.cpuNameLabel.ForeColor = System.Drawing.Color.White;
            this.cpuNameLabel.Location = new System.Drawing.Point(390, 9);
            this.cpuNameLabel.Name = "cpuNameLabel";
            this.cpuNameLabel.Size = new System.Drawing.Size(144, 19);
            this.cpuNameLabel.TabIndex = 3;
            this.cpuNameLabel.Text = "AMD Ryzen 5 2600";
            this.cpuNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // threadCountLabel
            // 
            this.threadCountLabel.AutoSize = true;
            this.threadCountLabel.ForeColor = System.Drawing.Color.White;
            this.threadCountLabel.Location = new System.Drawing.Point(390, 28);
            this.threadCountLabel.Name = "threadCountLabel";
            this.threadCountLabel.Size = new System.Drawing.Size(91, 19);
            this.threadCountLabel.TabIndex = 3;
            this.threadCountLabel.Text = "12 Потоков";
            this.threadCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // versionLabel2
            // 
            this.versionLabel2.AutoSize = true;
            this.versionLabel2.ForeColor = System.Drawing.Color.White;
            this.versionLabel2.Location = new System.Drawing.Point(390, 48);
            this.versionLabel2.Name = "versionLabel2";
            this.versionLabel2.Size = new System.Drawing.Size(90, 19);
            this.versionLabel2.TabIndex = 3;
            this.versionLabel2.Text = "v.0.5.0 beta";
            this.versionLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(390, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 19);
            this.label4.TabIndex = 3;
            this.label4.Text = "Скорость:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // speedLabel
            // 
            this.speedLabel.AutoSize = true;
            this.speedLabel.ForeColor = System.Drawing.Color.White;
            this.speedLabel.Location = new System.Drawing.Point(390, 143);
            this.speedLabel.Name = "speedLabel";
            this.speedLabel.Size = new System.Drawing.Size(43, 19);
            this.speedLabel.TabIndex = 3;
            this.speedLabel.Text = "0 к/с";
            this.speedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // benchmarkScoreboardLayout
            // 
            this.benchmarkScoreboardLayout.AutoScroll = true;
            this.benchmarkScoreboardLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.benchmarkScoreboardLayout.Controls.Add(this.panel1);
            this.benchmarkScoreboardLayout.Controls.Add(this.label1);
            this.benchmarkScoreboardLayout.Controls.Add(this.pictureBox1);
            this.benchmarkScoreboardLayout.Dock = System.Windows.Forms.DockStyle.Left;
            this.benchmarkScoreboardLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.benchmarkScoreboardLayout.ForeColor = System.Drawing.Color.White;
            this.benchmarkScoreboardLayout.Location = new System.Drawing.Point(0, 0);
            this.benchmarkScoreboardLayout.Name = "benchmarkScoreboardLayout";
            this.benchmarkScoreboardLayout.Padding = new System.Windows.Forms.Padding(10);
            this.benchmarkScoreboardLayout.Size = new System.Drawing.Size(388, 333);
            this.benchmarkScoreboardLayout.TabIndex = 4;
            this.benchmarkScoreboardLayout.WrapContents = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(361, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "Загрузка бенчмарков...";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::FloatToolGUI.Properties.Resources.loading;
            this.pictureBox1.Location = new System.Drawing.Point(13, 138);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(362, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // Updater
            // 
            this.Updater.Enabled = true;
            this.Updater.Tick += new System.EventHandler(this.Updater_Tick);
            // 
            // customProgressBar1
            // 
            this.customProgressBar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.customProgressBar1.ForeColor = System.Drawing.Color.White;
            this.customProgressBar1.Location = new System.Drawing.Point(394, 297);
            this.customProgressBar1.Margin = new System.Windows.Forms.Padding(0);
            this.customProgressBar1.Maximum = 184756;
            this.customProgressBar1.Minimum = 0;
            this.customProgressBar1.Name = "customProgressBar1";
            this.customProgressBar1.ProgressColor = System.Drawing.Color.Green;
            this.customProgressBar1.ProgressFont = new System.Drawing.Font("Inter", 11.25F, System.Drawing.FontStyle.Bold);
            this.customProgressBar1.Size = new System.Drawing.Size(304, 24);
            this.customProgressBar1.TabIndex = 0;
            this.customProgressBar1.Value = 0F;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.panel4.Controls.Add(this.updateBenchmarksButton);
            this.panel4.Controls.Add(this.warningPic);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.benchmarkThreadsNumericUpdown);
            this.panel4.Controls.Add(this.versionLabel2);
            this.panel4.Controls.Add(this.threadCountLabel);
            this.panel4.Controls.Add(this.cpuNameLabel);
            this.panel4.Controls.Add(this.startBenchmarkBtn);
            this.panel4.Controls.Add(this.customProgressBar1);
            this.panel4.Controls.Add(this.submitScoreBtn);
            this.panel4.Controls.Add(this.benchmarkScoreboardLayout);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.speedLabel);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Font = new System.Drawing.Font("Inter", 11F);
            this.panel4.Location = new System.Drawing.Point(0, 40);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(704, 333);
            this.panel4.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Inter", 22F);
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(1, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(160, 36);
            this.label8.TabIndex = 0;
            this.label8.Text = "Бенчмарк";
            this.label8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindowMouseDown);
            // 
            // closeBtn
            // 
            this.closeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBtn.BackColor = System.Drawing.Color.Transparent;
            this.closeBtn.FlatAppearance.BorderSize = 0;
            this.closeBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.closeBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.closeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeBtn.Font = new System.Drawing.Font("Inter", 16F);
            this.closeBtn.ForeColor = System.Drawing.Color.White;
            this.closeBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.closeBtn.Location = new System.Drawing.Point(664, 0);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(40, 40);
            this.closeBtn.TabIndex = 3;
            this.closeBtn.Text = "X";
            this.closeBtn.UseVisualStyleBackColor = false;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.panel3.Controls.Add(this.closeBtn);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(704, 40);
            this.panel3.TabIndex = 5;
            this.panel3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindowMouseDown);
            // 
            // benchmarkThreadsNumericUpdown
            // 
            this.benchmarkThreadsNumericUpdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.benchmarkThreadsNumericUpdown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.benchmarkThreadsNumericUpdown.ForeColor = System.Drawing.Color.White;
            this.benchmarkThreadsNumericUpdown.Location = new System.Drawing.Point(551, 190);
            this.benchmarkThreadsNumericUpdown.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.benchmarkThreadsNumericUpdown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.benchmarkThreadsNumericUpdown.Name = "benchmarkThreadsNumericUpdown";
            this.benchmarkThreadsNumericUpdown.Size = new System.Drawing.Size(147, 25);
            this.benchmarkThreadsNumericUpdown.TabIndex = 5;
            this.benchmarkThreadsNumericUpdown.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.benchmarkThreadsNumericUpdown.ValueChanged += new System.EventHandler(this.benchmarkThreadsNumericUpdown_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(390, 192);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 19);
            this.label2.TabIndex = 6;
            this.label2.Text = "Потоков:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ToolTip
            // 
            this.ToolTip.ToolTipTitle = "Внимание";
            // 
            // warningPic
            // 
            this.warningPic.Enabled = false;
            this.warningPic.Location = new System.Drawing.Point(520, 190);
            this.warningPic.Name = "warningPic";
            this.warningPic.Size = new System.Drawing.Size(25, 25);
            this.warningPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.warningPic.TabIndex = 7;
            this.warningPic.TabStop = false;
            this.ToolTip.SetToolTip(this.warningPic, "Не рекомендуется указывать большее количество \r\nпотоков, чем имеется в вашем проц" +
        "ессоре. \r\nСкорость будет значительно ниже.");
            this.warningPic.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(13, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(362, 100);
            this.panel1.TabIndex = 7;
            // 
            // updateBenchmarksButton
            // 
            this.updateBenchmarksButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.updateBenchmarksButton.FlatAppearance.BorderSize = 0;
            this.updateBenchmarksButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.updateBenchmarksButton.Font = new System.Drawing.Font("Inter", 10F);
            this.updateBenchmarksButton.ForeColor = System.Drawing.Color.White;
            this.updateBenchmarksButton.Location = new System.Drawing.Point(394, 221);
            this.updateBenchmarksButton.Name = "updateBenchmarksButton";
            this.updateBenchmarksButton.Size = new System.Drawing.Size(84, 29);
            this.updateBenchmarksButton.TabIndex = 8;
            this.updateBenchmarksButton.Text = "Обновить";
            this.updateBenchmarksButton.UseVisualStyleBackColor = false;
            this.updateBenchmarksButton.Click += new System.EventHandler(this.updateBenchmarksButton_Click);
            // 
            // Benchmark
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.ClientSize = new System.Drawing.Size(704, 373);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Benchmark";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Бенчмарк";
            this.benchmarkScoreboardLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.benchmarkThreadsNumericUpdown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningPic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CustomProgressBar customProgressBar1;
        private System.Windows.Forms.Button startBenchmarkBtn;
        private System.Windows.Forms.Button submitScoreBtn;
        private System.Windows.Forms.Label cpuNameLabel;
        private System.Windows.Forms.Label threadCountLabel;
        private System.Windows.Forms.Label versionLabel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label speedLabel;
        private System.Windows.Forms.FlowLayoutPanel benchmarkScoreboardLayout;
        private System.Windows.Forms.Timer Updater;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown benchmarkThreadsNumericUpdown;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.PictureBox warningPic;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button updateBenchmarksButton;
    }
}