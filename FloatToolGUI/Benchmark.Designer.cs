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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Updater = new System.Windows.Forms.Timer(this.components);
            this.customProgressBar1 = new FloatToolGUI.CustomProgressBar();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.closeBtn = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel10.SuspendLayout();
            this.SuspendLayout();
            // 
            // startBenchmarkBtn
            // 
            this.startBenchmarkBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.startBenchmarkBtn.FlatAppearance.BorderSize = 0;
            this.startBenchmarkBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startBenchmarkBtn.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.submitScoreBtn.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 12F);
            this.submitScoreBtn.ForeColor = System.Drawing.Color.White;
            this.submitScoreBtn.Location = new System.Drawing.Point(394, 221);
            this.submitScoreBtn.Name = "submitScoreBtn";
            this.submitScoreBtn.Size = new System.Drawing.Size(304, 29);
            this.submitScoreBtn.TabIndex = 2;
            this.submitScoreBtn.Text = "Опубликовать";
            this.submitScoreBtn.UseVisualStyleBackColor = false;
            this.submitScoreBtn.Click += new System.EventHandler(this.submitScoreBtn_Click);
            // 
            // cpuNameLabel
            // 
            this.cpuNameLabel.AutoSize = true;
            this.cpuNameLabel.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 11F);
            this.cpuNameLabel.ForeColor = System.Drawing.Color.White;
            this.cpuNameLabel.Location = new System.Drawing.Point(390, 9);
            this.cpuNameLabel.Name = "cpuNameLabel";
            this.cpuNameLabel.Size = new System.Drawing.Size(137, 19);
            this.cpuNameLabel.TabIndex = 3;
            this.cpuNameLabel.Text = "AMD Ryzen 5 2600";
            this.cpuNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // threadCountLabel
            // 
            this.threadCountLabel.AutoSize = true;
            this.threadCountLabel.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.threadCountLabel.ForeColor = System.Drawing.Color.White;
            this.threadCountLabel.Location = new System.Drawing.Point(390, 28);
            this.threadCountLabel.Name = "threadCountLabel";
            this.threadCountLabel.Size = new System.Drawing.Size(93, 20);
            this.threadCountLabel.TabIndex = 3;
            this.threadCountLabel.Text = "12 Потоков";
            this.threadCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // versionLabel2
            // 
            this.versionLabel2.AutoSize = true;
            this.versionLabel2.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionLabel2.ForeColor = System.Drawing.Color.White;
            this.versionLabel2.Location = new System.Drawing.Point(390, 48);
            this.versionLabel2.Name = "versionLabel2";
            this.versionLabel2.Size = new System.Drawing.Size(93, 20);
            this.versionLabel2.TabIndex = 3;
            this.versionLabel2.Text = "v.0.5.0 beta";
            this.versionLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(390, 151);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Скорость:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // speedLabel
            // 
            this.speedLabel.AutoSize = true;
            this.speedLabel.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.speedLabel.ForeColor = System.Drawing.Color.White;
            this.speedLabel.Location = new System.Drawing.Point(390, 171);
            this.speedLabel.Name = "speedLabel";
            this.speedLabel.Size = new System.Drawing.Size(45, 20);
            this.speedLabel.TabIndex = 3;
            this.speedLabel.Text = "0 к/с";
            this.speedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Controls.Add(this.panel10);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.ForeColor = System.Drawing.Color.White;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(388, 333);
            this.flowLayoutPanel1.TabIndex = 4;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(10)))), ((int)(((byte)(27)))));
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Location = new System.Drawing.Point(10, 12);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 2, 0, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(350, 35);
            this.panel1.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 8F);
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(3, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(128, 14);
            this.label7.TabIndex = 3;
            this.label7.Text = "1021779 к/с (v.0.5.0 beta)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 8F);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(3, 3);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(260, 14);
            this.label6.TabIndex = 3;
            this.label6.Text = "AMD Ryzen 5 2600 Six-Core Processor (12 Threads)";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.customProgressBar1.ProgressFont = new System.Drawing.Font("Microsoft JhengHei UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.customProgressBar1.Size = new System.Drawing.Size(304, 24);
            this.customProgressBar1.TabIndex = 0;
            this.customProgressBar1.Value = 0F;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.panel4.Controls.Add(this.versionLabel2);
            this.panel4.Controls.Add(this.threadCountLabel);
            this.panel4.Controls.Add(this.cpuNameLabel);
            this.panel4.Controls.Add(this.startBenchmarkBtn);
            this.panel4.Controls.Add(this.customProgressBar1);
            this.panel4.Controls.Add(this.submitScoreBtn);
            this.panel4.Controls.Add(this.flowLayoutPanel1);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.speedLabel);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 40);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(704, 333);
            this.panel4.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Microsoft JhengHei Light", 22F);
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(1, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(154, 38);
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
            this.closeBtn.Font = new System.Drawing.Font("Microsoft JhengHei Light", 16F);
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
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(125)))), ((int)(((byte)(195)))));
            this.panel10.Controls.Add(this.label17);
            this.panel10.Controls.Add(this.label18);
            this.panel10.Location = new System.Drawing.Point(10, 52);
            this.panel10.Margin = new System.Windows.Forms.Padding(0, 2, 0, 3);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(350, 35);
            this.panel10.TabIndex = 19;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 8F);
            this.label17.ForeColor = System.Drawing.Color.White;
            this.label17.Location = new System.Drawing.Point(3, 18);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(126, 14);
            this.label17.TabIndex = 3;
            this.label17.Text = "934030 к/с (v.0.5.0 beta)";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 8F);
            this.label18.ForeColor = System.Drawing.Color.White;
            this.label18.Location = new System.Drawing.Point(3, 3);
            this.label18.Margin = new System.Windows.Forms.Padding(0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(289, 14);
            this.label18.TabIndex = 3;
            this.label18.Text = "Intel(R) Core(TM) i5-7300HQ CPU @ 2.50GHz (4 Threads)";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
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
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Timer Updater;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
    }
}