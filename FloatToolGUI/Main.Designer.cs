using System.Threading;

namespace FloatToolGUI
{
    partial class FloatTool
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (client.IsInitialized)
            {
                client.Dispose();
            }
            if (thread1.IsAlive)
            {
                thread1.Abort();
            }
            foreach (Thread t in t2)
            {
                if (t.IsAlive)
                {
                    t.Abort();
                }
            }
            if (!base.IsDisposed) { 
                base.Dispose(disposing);
            }
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FloatTool));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.foundCombinationContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.outputConsoleBox = new System.Windows.Forms.TextBox();
            this.DiscordUpdater = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel13 = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.benchmarkButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this.MaximizeButton = new System.Windows.Forms.Button();
            this.minimizeBtn = new System.Windows.Forms.Button();
            this.closeBtn = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label24 = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.stattrackCheckBox = new FloatToolGUI.CustomControls.CustomToggleSwitch();
            this.checkPossibilityBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.outcomeSelectorComboBox = new System.Windows.Forms.ComboBox();
            this.weaponQualityBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.weaponTypeBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fullSkinName = new System.Windows.Forms.TextBox();
            this.weaponSkinBox = new System.Windows.Forms.ComboBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel16 = new System.Windows.Forms.Panel();
            this.startSearchSingleButton = new System.Windows.Forms.Button();
            this.downloadProgressBar = new FloatToolGUI.CustomProgressBar();
            this.searchmodeGreater_btn = new System.Windows.Forms.Button();
            this.searchmodeEqual_btn = new System.Windows.Forms.Button();
            this.searchmodeLess_btn = new System.Windows.Forms.Button();
            this.searchModeLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.quantityInput = new System.Windows.Forms.NumericUpDown();
            this.skipValueInput = new System.Windows.Forms.NumericUpDown();
            this.ascendingCheckBox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.searchFloatInput = new System.Windows.Forms.TextBox();
            this.sortCheckBox = new System.Windows.Forms.CheckBox();
            this.startBtn = new System.Windows.Forms.Button();
            this.panel15 = new System.Windows.Forms.Panel();
            this.speedStatusLabel = new System.Windows.Forms.Label();
            this.combinationsStatusLabel = new System.Windows.Forms.Label();
            this.gpuSearch_btn = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.threadCountInput = new System.Windows.Forms.NumericUpDown();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.WorkStatusUpdater = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.quantityInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.skipValueInput)).BeginInit();
            this.panel15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threadCountInput)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.ForeColor = System.Drawing.Color.White;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.splitContainer1.Panel1.Controls.Add(this.foundCombinationContainer);
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.splitContainer1.Panel2.Controls.Add(this.outputConsoleBox);
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            // 
            // foundCombinationContainer
            // 
            resources.ApplyResources(this.foundCombinationContainer, "foundCombinationContainer");
            this.foundCombinationContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.foundCombinationContainer.Name = "foundCombinationContainer";
            // 
            // outputConsoleBox
            // 
            this.outputConsoleBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.outputConsoleBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.outputConsoleBox.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.outputConsoleBox, "outputConsoleBox");
            this.outputConsoleBox.ForeColor = System.Drawing.Color.White;
            this.outputConsoleBox.Name = "outputConsoleBox";
            this.outputConsoleBox.ReadOnly = true;
            this.outputConsoleBox.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // DiscordUpdater
            // 
            this.DiscordUpdater.Enabled = true;
            this.DiscordUpdater.Interval = 15000;
            this.DiscordUpdater.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.Controls.Add(this.panel8);
            this.panel1.Controls.Add(this.panel7);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.panel13);
            this.panel8.Controls.Add(this.panel10);
            resources.ApplyResources(this.panel8, "panel8");
            this.panel8.Name = "panel8";
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.panel11);
            this.panel13.Controls.Add(this.panel12);
            resources.ApplyResources(this.panel13, "panel13");
            this.panel13.Name = "panel13";
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.splitContainer1);
            resources.ApplyResources(this.panel11, "panel11");
            this.panel11.Name = "panel11";
            // 
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.panel12, "panel12");
            this.panel12.Name = "panel12";
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.panel10, "panel10");
            this.panel10.Name = "panel10";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(227)))));
            this.panel7.Controls.Add(this.panel9);
            this.panel7.Controls.Add(this.button5);
            this.panel7.Controls.Add(this.button4);
            this.panel7.Controls.Add(this.button3);
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Name = "panel7";
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.panel9.Controls.Add(this.benchmarkButton);
            this.panel9.Controls.Add(this.settingsButton);
            this.panel9.Controls.Add(this.MaximizeButton);
            this.panel9.Controls.Add(this.minimizeBtn);
            this.panel9.Controls.Add(this.closeBtn);
            resources.ApplyResources(this.panel9, "panel9");
            this.panel9.Name = "panel9";
            this.panel9.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragEvent);
            // 
            // benchmarkButton
            // 
            this.benchmarkButton.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.benchmarkButton, "benchmarkButton");
            this.benchmarkButton.FlatAppearance.BorderSize = 0;
            this.benchmarkButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.benchmarkButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.benchmarkButton.ForeColor = System.Drawing.Color.White;
            this.benchmarkButton.Name = "benchmarkButton";
            this.benchmarkButton.UseVisualStyleBackColor = false;
            this.benchmarkButton.Click += new System.EventHandler(this.benchmarkButton_Click);
            // 
            // settingsButton
            // 
            this.settingsButton.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.settingsButton, "settingsButton");
            this.settingsButton.FlatAppearance.BorderSize = 0;
            this.settingsButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.settingsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.settingsButton.ForeColor = System.Drawing.Color.White;
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.UseVisualStyleBackColor = false;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // MaximizeButton
            // 
            resources.ApplyResources(this.MaximizeButton, "MaximizeButton");
            this.MaximizeButton.BackColor = System.Drawing.Color.Transparent;
            this.MaximizeButton.FlatAppearance.BorderSize = 0;
            this.MaximizeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.MaximizeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.MaximizeButton.ForeColor = System.Drawing.Color.White;
            this.MaximizeButton.Name = "MaximizeButton";
            this.MaximizeButton.UseVisualStyleBackColor = false;
            this.MaximizeButton.Click += new System.EventHandler(this.MaximizeMinimizeButton);
            // 
            // minimizeBtn
            // 
            resources.ApplyResources(this.minimizeBtn, "minimizeBtn");
            this.minimizeBtn.BackColor = System.Drawing.Color.Transparent;
            this.minimizeBtn.FlatAppearance.BorderSize = 0;
            this.minimizeBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.minimizeBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.minimizeBtn.ForeColor = System.Drawing.Color.White;
            this.minimizeBtn.Name = "minimizeBtn";
            this.minimizeBtn.UseVisualStyleBackColor = false;
            this.minimizeBtn.Click += new System.EventHandler(this.MaximizeMinimizeButton);
            // 
            // closeBtn
            // 
            resources.ApplyResources(this.closeBtn, "closeBtn");
            this.closeBtn.BackColor = System.Drawing.Color.Transparent;
            this.closeBtn.FlatAppearance.BorderSize = 0;
            this.closeBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.closeBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.closeBtn.ForeColor = System.Drawing.Color.White;
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.UseVisualStyleBackColor = false;
            this.closeBtn.Click += new System.EventHandler(this.CloseAppButton_Click);
            // 
            // button5
            // 
            resources.ApplyResources(this.button5, "button5");
            this.button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(147)))), ((int)(((byte)(221)))));
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Name = "button5";
            this.button5.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            resources.ApplyResources(this.button4, "button4");
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(126)))), ((int)(((byte)(161)))), ((int)(((byte)(114)))));
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Name = "button4";
            this.button4.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(87)))), ((int)(((byte)(87)))));
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.panel3.Controls.Add(this.label24);
            this.panel3.Controls.Add(this.versionLabel);
            this.panel3.Controls.Add(this.label8);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            this.panel3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragEvent);
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.label24.Name = "label24";
            this.label24.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.debugMenuShow);
            // 
            // versionLabel
            // 
            resources.ApplyResources(this.versionLabel, "versionLabel");
            this.versionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.versionLabel.Name = "versionLabel";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Name = "label8";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            this.panel6.Controls.Add(this.stattrackCheckBox);
            this.panel6.Controls.Add(this.checkPossibilityBtn);
            this.panel6.Controls.Add(this.label3);
            this.panel6.Controls.Add(this.outcomeSelectorComboBox);
            this.panel6.Controls.Add(this.weaponQualityBox);
            this.panel6.Controls.Add(this.label12);
            this.panel6.Controls.Add(this.label1);
            this.panel6.Controls.Add(this.label4);
            this.panel6.Controls.Add(this.weaponTypeBox);
            this.panel6.Controls.Add(this.label11);
            this.panel6.Controls.Add(this.label2);
            this.panel6.Controls.Add(this.fullSkinName);
            this.panel6.Controls.Add(this.weaponSkinBox);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // stattrackCheckBox
            // 
            this.stattrackCheckBox.Checked = false;
            this.stattrackCheckBox.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.stattrackCheckBox, "stattrackCheckBox");
            this.stattrackCheckBox.Name = "stattrackCheckBox";
            this.stattrackCheckBox.TurnedOffColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.stattrackCheckBox.TurnedOnColor = System.Drawing.Color.Green;
            this.stattrackCheckBox.OnToggled += new System.EventHandler(this.SkinComboboxChanged);
            // 
            // checkPossibilityBtn
            // 
            resources.ApplyResources(this.checkPossibilityBtn, "checkPossibilityBtn");
            this.checkPossibilityBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.checkPossibilityBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkPossibilityBtn.FlatAppearance.BorderSize = 0;
            this.checkPossibilityBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.WindowFrame;
            this.checkPossibilityBtn.ForeColor = System.Drawing.Color.White;
            this.checkPossibilityBtn.Name = "checkPossibilityBtn";
            this.checkPossibilityBtn.UseVisualStyleBackColor = false;
            this.checkPossibilityBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Name = "label3";
            // 
            // outcomeSelectorComboBox
            // 
            resources.ApplyResources(this.outcomeSelectorComboBox, "outcomeSelectorComboBox");
            this.outcomeSelectorComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.outcomeSelectorComboBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.outcomeSelectorComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.outcomeSelectorComboBox.FormattingEnabled = true;
            this.outcomeSelectorComboBox.Items.AddRange(new object[] {
            resources.GetString("outcomeSelectorComboBox.Items"),
            resources.GetString("outcomeSelectorComboBox.Items1"),
            resources.GetString("outcomeSelectorComboBox.Items2")});
            this.outcomeSelectorComboBox.Name = "outcomeSelectorComboBox";
            // 
            // weaponQualityBox
            // 
            resources.ApplyResources(this.weaponQualityBox, "weaponQualityBox");
            this.weaponQualityBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.weaponQualityBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.weaponQualityBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.weaponQualityBox.FormattingEnabled = true;
            this.weaponQualityBox.Items.AddRange(new object[] {
            resources.GetString("weaponQualityBox.Items"),
            resources.GetString("weaponQualityBox.Items1"),
            resources.GetString("weaponQualityBox.Items2"),
            resources.GetString("weaponQualityBox.Items3"),
            resources.GetString("weaponQualityBox.Items4")});
            this.weaponQualityBox.Name = "weaponQualityBox";
            this.weaponQualityBox.SelectedIndexChanged += new System.EventHandler(this.SkinComboboxChanged);
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Name = "label12";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Name = "label4";
            // 
            // weaponTypeBox
            // 
            resources.ApplyResources(this.weaponTypeBox, "weaponTypeBox");
            this.weaponTypeBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.weaponTypeBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.weaponTypeBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.weaponTypeBox.FormattingEnabled = true;
            this.weaponTypeBox.Items.AddRange(new object[] {
            resources.GetString("weaponTypeBox.Items"),
            resources.GetString("weaponTypeBox.Items1"),
            resources.GetString("weaponTypeBox.Items2"),
            resources.GetString("weaponTypeBox.Items3"),
            resources.GetString("weaponTypeBox.Items4"),
            resources.GetString("weaponTypeBox.Items5"),
            resources.GetString("weaponTypeBox.Items6"),
            resources.GetString("weaponTypeBox.Items7"),
            resources.GetString("weaponTypeBox.Items8"),
            resources.GetString("weaponTypeBox.Items9"),
            resources.GetString("weaponTypeBox.Items10"),
            resources.GetString("weaponTypeBox.Items11"),
            resources.GetString("weaponTypeBox.Items12"),
            resources.GetString("weaponTypeBox.Items13"),
            resources.GetString("weaponTypeBox.Items14"),
            resources.GetString("weaponTypeBox.Items15"),
            resources.GetString("weaponTypeBox.Items16"),
            resources.GetString("weaponTypeBox.Items17"),
            resources.GetString("weaponTypeBox.Items18"),
            resources.GetString("weaponTypeBox.Items19"),
            resources.GetString("weaponTypeBox.Items20"),
            resources.GetString("weaponTypeBox.Items21"),
            resources.GetString("weaponTypeBox.Items22"),
            resources.GetString("weaponTypeBox.Items23"),
            resources.GetString("weaponTypeBox.Items24"),
            resources.GetString("weaponTypeBox.Items25"),
            resources.GetString("weaponTypeBox.Items26"),
            resources.GetString("weaponTypeBox.Items27"),
            resources.GetString("weaponTypeBox.Items28"),
            resources.GetString("weaponTypeBox.Items29"),
            resources.GetString("weaponTypeBox.Items30"),
            resources.GetString("weaponTypeBox.Items31"),
            resources.GetString("weaponTypeBox.Items32"),
            resources.GetString("weaponTypeBox.Items33")});
            this.weaponTypeBox.Name = "weaponTypeBox";
            this.weaponTypeBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Name = "label11";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Name = "label2";
            // 
            // fullSkinName
            // 
            this.fullSkinName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.fullSkinName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.fullSkinName, "fullSkinName");
            this.fullSkinName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.fullSkinName.Name = "fullSkinName";
            // 
            // weaponSkinBox
            // 
            resources.ApplyResources(this.weaponSkinBox, "weaponSkinBox");
            this.weaponSkinBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.weaponSkinBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.weaponSkinBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.weaponSkinBox.FormattingEnabled = true;
            this.weaponSkinBox.Name = "weaponSkinBox";
            this.weaponSkinBox.SelectedIndexChanged += new System.EventHandler(this.SkinComboboxChanged);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            this.panel5.Controls.Add(this.panel16);
            this.panel5.Controls.Add(this.panel15);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // panel16
            // 
            this.panel16.BackColor = System.Drawing.Color.Transparent;
            this.panel16.Controls.Add(this.startSearchSingleButton);
            this.panel16.Controls.Add(this.downloadProgressBar);
            this.panel16.Controls.Add(this.searchmodeGreater_btn);
            this.panel16.Controls.Add(this.searchmodeEqual_btn);
            this.panel16.Controls.Add(this.searchmodeLess_btn);
            this.panel16.Controls.Add(this.searchModeLabel);
            this.panel16.Controls.Add(this.label5);
            this.panel16.Controls.Add(this.quantityInput);
            this.panel16.Controls.Add(this.skipValueInput);
            this.panel16.Controls.Add(this.ascendingCheckBox);
            this.panel16.Controls.Add(this.label7);
            this.panel16.Controls.Add(this.label6);
            this.panel16.Controls.Add(this.searchFloatInput);
            this.panel16.Controls.Add(this.sortCheckBox);
            this.panel16.Controls.Add(this.startBtn);
            resources.ApplyResources(this.panel16, "panel16");
            this.panel16.Name = "panel16";
            // 
            // startSearchSingleButton
            // 
            resources.ApplyResources(this.startSearchSingleButton, "startSearchSingleButton");
            this.startSearchSingleButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.startSearchSingleButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.startSearchSingleButton.FlatAppearance.BorderSize = 0;
            this.startSearchSingleButton.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.WindowFrame;
            this.startSearchSingleButton.ForeColor = System.Drawing.Color.White;
            this.startSearchSingleButton.Name = "startSearchSingleButton";
            this.startSearchSingleButton.UseVisualStyleBackColor = false;
            this.startSearchSingleButton.Click += new System.EventHandler(this.StartButtonClick);
            // 
            // downloadProgressBar
            // 
            resources.ApplyResources(this.downloadProgressBar, "downloadProgressBar");
            this.downloadProgressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.downloadProgressBar.ForeColor = System.Drawing.Color.White;
            this.downloadProgressBar.Maximum = 100;
            this.downloadProgressBar.Minimum = 0;
            this.downloadProgressBar.Name = "downloadProgressBar";
            this.downloadProgressBar.ProgressColor = System.Drawing.Color.Green;
            this.downloadProgressBar.ProgressFont = new System.Drawing.Font("Inter", 11.25F, System.Drawing.FontStyle.Bold);
            this.downloadProgressBar.Value = 0F;
            // 
            // searchmodeGreater_btn
            // 
            resources.ApplyResources(this.searchmodeGreater_btn, "searchmodeGreater_btn");
            this.searchmodeGreater_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.searchmodeGreater_btn.Cursor = System.Windows.Forms.Cursors.Default;
            this.searchmodeGreater_btn.FlatAppearance.BorderSize = 0;
            this.searchmodeGreater_btn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.WindowFrame;
            this.searchmodeGreater_btn.ForeColor = System.Drawing.Color.White;
            this.searchmodeGreater_btn.Name = "searchmodeGreater_btn";
            this.searchmodeGreater_btn.UseVisualStyleBackColor = false;
            this.searchmodeGreater_btn.Click += new System.EventHandler(this.changeSearchMode);
            // 
            // searchmodeEqual_btn
            // 
            resources.ApplyResources(this.searchmodeEqual_btn, "searchmodeEqual_btn");
            this.searchmodeEqual_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.searchmodeEqual_btn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.WindowFrame;
            this.searchmodeEqual_btn.ForeColor = System.Drawing.Color.White;
            this.searchmodeEqual_btn.Name = "searchmodeEqual_btn";
            this.searchmodeEqual_btn.UseVisualStyleBackColor = false;
            this.searchmodeEqual_btn.Click += new System.EventHandler(this.changeSearchMode);
            // 
            // searchmodeLess_btn
            // 
            resources.ApplyResources(this.searchmodeLess_btn, "searchmodeLess_btn");
            this.searchmodeLess_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.searchmodeLess_btn.FlatAppearance.BorderSize = 0;
            this.searchmodeLess_btn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.WindowFrame;
            this.searchmodeLess_btn.ForeColor = System.Drawing.Color.White;
            this.searchmodeLess_btn.Name = "searchmodeLess_btn";
            this.searchmodeLess_btn.UseVisualStyleBackColor = false;
            this.searchmodeLess_btn.Click += new System.EventHandler(this.changeSearchMode);
            // 
            // searchModeLabel
            // 
            resources.ApplyResources(this.searchModeLabel, "searchModeLabel");
            this.searchModeLabel.ForeColor = System.Drawing.Color.White;
            this.searchModeLabel.Name = "searchModeLabel";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Name = "label5";
            // 
            // quantityInput
            // 
            resources.ApplyResources(this.quantityInput, "quantityInput");
            this.quantityInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.quantityInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.quantityInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.quantityInput.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.quantityInput.Name = "quantityInput";
            this.quantityInput.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // skipValueInput
            // 
            resources.ApplyResources(this.skipValueInput, "skipValueInput");
            this.skipValueInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.skipValueInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.skipValueInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.skipValueInput.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.skipValueInput.Name = "skipValueInput";
            this.skipValueInput.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // ascendingCheckBox
            // 
            resources.ApplyResources(this.ascendingCheckBox, "ascendingCheckBox");
            this.ascendingCheckBox.ForeColor = System.Drawing.Color.White;
            this.ascendingCheckBox.Name = "ascendingCheckBox";
            this.ascendingCheckBox.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Name = "label6";
            // 
            // searchFloatInput
            // 
            resources.ApplyResources(this.searchFloatInput, "searchFloatInput");
            this.searchFloatInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.searchFloatInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchFloatInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.searchFloatInput.Name = "searchFloatInput";
            // 
            // sortCheckBox
            // 
            resources.ApplyResources(this.sortCheckBox, "sortCheckBox");
            this.sortCheckBox.ForeColor = System.Drawing.Color.White;
            this.sortCheckBox.Name = "sortCheckBox";
            this.sortCheckBox.UseVisualStyleBackColor = true;
            // 
            // startBtn
            // 
            resources.ApplyResources(this.startBtn, "startBtn");
            this.startBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.startBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.startBtn.FlatAppearance.BorderSize = 0;
            this.startBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.WindowFrame;
            this.startBtn.ForeColor = System.Drawing.Color.White;
            this.startBtn.Name = "startBtn";
            this.startBtn.UseVisualStyleBackColor = false;
            this.startBtn.Click += new System.EventHandler(this.StartButtonClick);
            // 
            // panel15
            // 
            this.panel15.BackColor = System.Drawing.Color.Transparent;
            this.panel15.Controls.Add(this.speedStatusLabel);
            this.panel15.Controls.Add(this.combinationsStatusLabel);
            this.panel15.Controls.Add(this.gpuSearch_btn);
            this.panel15.Controls.Add(this.label10);
            this.panel15.Controls.Add(this.threadCountInput);
            resources.ApplyResources(this.panel15, "panel15");
            this.panel15.Name = "panel15";
            // 
            // speedStatusLabel
            // 
            resources.ApplyResources(this.speedStatusLabel, "speedStatusLabel");
            this.speedStatusLabel.ForeColor = System.Drawing.Color.White;
            this.speedStatusLabel.Name = "speedStatusLabel";
            // 
            // combinationsStatusLabel
            // 
            resources.ApplyResources(this.combinationsStatusLabel, "combinationsStatusLabel");
            this.combinationsStatusLabel.ForeColor = System.Drawing.Color.White;
            this.combinationsStatusLabel.Name = "combinationsStatusLabel";
            // 
            // gpuSearch_btn
            // 
            resources.ApplyResources(this.gpuSearch_btn, "gpuSearch_btn");
            this.gpuSearch_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.gpuSearch_btn.FlatAppearance.BorderSize = 0;
            this.gpuSearch_btn.ForeColor = System.Drawing.Color.White;
            this.gpuSearch_btn.Name = "gpuSearch_btn";
            this.gpuSearch_btn.UseVisualStyleBackColor = false;
            this.gpuSearch_btn.Click += new System.EventHandler(this.gpuSearch_btn_Click);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Name = "label10";
            // 
            // threadCountInput
            // 
            this.threadCountInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.threadCountInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.threadCountInput, "threadCountInput");
            this.threadCountInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.threadCountInput.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.threadCountInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.threadCountInput.Name = "threadCountInput";
            this.threadCountInput.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.panel6);
            this.panel2.Controls.Add(this.panel3);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel1);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // WorkStatusUpdater
            // 
            this.WorkStatusUpdater.Enabled = true;
            this.WorkStatusUpdater.Interval = 250;
            this.WorkStatusUpdater.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // FloatTool
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FloatTool";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel16.ResumeLayout(false);
            this.panel16.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.quantityInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.skipValueInput)).EndInit();
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threadCountInput)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox outputConsoleBox;
        private System.Windows.Forms.Timer DiscordUpdater;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button checkPossibilityBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox weaponQualityBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox weaponTypeBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fullSkinName;
        private System.Windows.Forms.ComboBox weaponSkinBox;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.CheckBox ascendingCheckBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox sortCheckBox;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.TextBox searchFloatInput;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown skipValueInput;
        private System.Windows.Forms.NumericUpDown quantityInput;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Button minimizeBtn;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Panel panel16;
        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown threadCountInput;
        private System.Windows.Forms.Button gpuSearch_btn;
        private System.Windows.Forms.Label combinationsStatusLabel;
        private System.Windows.Forms.Timer WorkStatusUpdater;
        private System.Windows.Forms.Label speedStatusLabel;
        private System.Windows.Forms.Button searchmodeGreater_btn;
        private System.Windows.Forms.Button searchmodeEqual_btn;
        private System.Windows.Forms.Button searchmodeLess_btn;
        private System.Windows.Forms.Label searchModeLabel;
        private System.Windows.Forms.Button MaximizeButton;
        private CustomProgressBar downloadProgressBar;
        private System.Windows.Forms.ComboBox outcomeSelectorComboBox;
        private System.Windows.Forms.Label label11;
        private CustomControls.CustomToggleSwitch stattrackCheckBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button benchmarkButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.FlowLayoutPanel foundCombinationContainer;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Button startSearchSingleButton;
    }
}

