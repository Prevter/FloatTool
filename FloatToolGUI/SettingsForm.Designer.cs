namespace FloatToolGUI
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.panel3 = new System.Windows.Forms.Panel();
            this.closeBtn = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.saveChangesBtn = new System.Windows.Forms.Button();
            this.resetChangesBtn = new System.Windows.Forms.Button();
            this.bufferSpeedNUP = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.checkUpdatesToggle = new FloatToolGUI.CustomControls.CustomToggleSwitch();
            this.soundToggle = new FloatToolGUI.CustomControls.CustomToggleSwitch();
            this.darkModeToggle = new FloatToolGUI.CustomControls.CustomToggleSwitch();
            this.discordRpcToggle = new FloatToolGUI.CustomControls.CustomToggleSwitch();
            this.label5 = new System.Windows.Forms.Label();
            this.currencyComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.currencyBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.currencyBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bufferSpeedNUP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currencyBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currencyBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.panel3.Controls.Add(this.closeBtn);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(390, 40);
            this.panel3.TabIndex = 1;
            this.panel3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindowMouseDown);
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
            this.closeBtn.Location = new System.Drawing.Point(350, 0);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(40, 40);
            this.closeBtn.TabIndex = 3;
            this.closeBtn.Text = "X";
            this.closeBtn.UseVisualStyleBackColor = false;
            this.closeBtn.Click += new System.EventHandler(this.CloseForm);
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
            this.label8.Size = new System.Drawing.Size(167, 38);
            this.label8.TabIndex = 0;
            this.label8.Text = "Настройки";
            this.label8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragWindowMouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft JhengHei Light", 14F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(75, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "Тёмная тема";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft JhengHei Light", 14F);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(75, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(205, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "Звук при нахождении";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft JhengHei Light", 14F);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(75, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(214, 24);
            this.label3.TabIndex = 3;
            this.label3.Text = "Проверка обновлений";
            // 
            // saveChangesBtn
            // 
            this.saveChangesBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.saveChangesBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.saveChangesBtn.Enabled = false;
            this.saveChangesBtn.FlatAppearance.BorderSize = 0;
            this.saveChangesBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveChangesBtn.Font = new System.Drawing.Font("Microsoft JhengHei Light", 14F);
            this.saveChangesBtn.ForeColor = System.Drawing.Color.White;
            this.saveChangesBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.saveChangesBtn.Location = new System.Drawing.Point(8, 320);
            this.saveChangesBtn.Name = "saveChangesBtn";
            this.saveChangesBtn.Size = new System.Drawing.Size(185, 32);
            this.saveChangesBtn.TabIndex = 4;
            this.saveChangesBtn.Text = "Применить";
            this.saveChangesBtn.UseVisualStyleBackColor = false;
            this.saveChangesBtn.Click += new System.EventHandler(this.saveChangesBtn_Click);
            // 
            // resetChangesBtn
            // 
            this.resetChangesBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.resetChangesBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.resetChangesBtn.FlatAppearance.BorderSize = 0;
            this.resetChangesBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resetChangesBtn.Font = new System.Drawing.Font("Microsoft JhengHei Light", 14F);
            this.resetChangesBtn.ForeColor = System.Drawing.Color.White;
            this.resetChangesBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.resetChangesBtn.Location = new System.Drawing.Point(199, 320);
            this.resetChangesBtn.Name = "resetChangesBtn";
            this.resetChangesBtn.Size = new System.Drawing.Size(185, 32);
            this.resetChangesBtn.TabIndex = 4;
            this.resetChangesBtn.Text = "Отмена";
            this.resetChangesBtn.UseVisualStyleBackColor = false;
            this.resetChangesBtn.Click += new System.EventHandler(this.resetChangesBtn_Click);
            // 
            // bufferSpeedNUP
            // 
            this.bufferSpeedNUP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.bufferSpeedNUP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bufferSpeedNUP.Font = new System.Drawing.Font("Microsoft JhengHei Light", 14F);
            this.bufferSpeedNUP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.bufferSpeedNUP.Location = new System.Drawing.Point(8, 280);
            this.bufferSpeedNUP.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.bufferSpeedNUP.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.bufferSpeedNUP.Name = "bufferSpeedNUP";
            this.bufferSpeedNUP.Size = new System.Drawing.Size(376, 32);
            this.bufferSpeedNUP.TabIndex = 5;
            this.bufferSpeedNUP.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.bufferSpeedNUP.ValueChanged += new System.EventHandler(this.bufferSpeedNUP_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft JhengHei Light", 14F);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(4, 253);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(320, 24);
            this.label4.TabIndex = 3;
            this.label4.Text = "Скорость обновления буфера (мс)";
            // 
            // checkUpdatesToggle
            // 
            this.checkUpdatesToggle.Checked = true;
            this.checkUpdatesToggle.ForeColor = System.Drawing.Color.White;
            this.checkUpdatesToggle.Location = new System.Drawing.Point(8, 131);
            this.checkUpdatesToggle.Name = "checkUpdatesToggle";
            this.checkUpdatesToggle.Size = new System.Drawing.Size(64, 36);
            this.checkUpdatesToggle.TabIndex = 2;
            this.checkUpdatesToggle.TurnedOffColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.checkUpdatesToggle.TurnedOnColor = System.Drawing.Color.Green;
            this.checkUpdatesToggle.OnToggled += new System.EventHandler(this.checkUpdatesToggle_OnToggled);
            // 
            // soundToggle
            // 
            this.soundToggle.Checked = true;
            this.soundToggle.ForeColor = System.Drawing.Color.White;
            this.soundToggle.Location = new System.Drawing.Point(8, 89);
            this.soundToggle.Name = "soundToggle";
            this.soundToggle.Size = new System.Drawing.Size(64, 36);
            this.soundToggle.TabIndex = 2;
            this.soundToggle.TurnedOffColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.soundToggle.TurnedOnColor = System.Drawing.Color.Green;
            this.soundToggle.OnToggled += new System.EventHandler(this.soundToggle_OnToggled);
            // 
            // darkModeToggle
            // 
            this.darkModeToggle.Checked = true;
            this.darkModeToggle.ForeColor = System.Drawing.Color.White;
            this.darkModeToggle.Location = new System.Drawing.Point(8, 46);
            this.darkModeToggle.Name = "darkModeToggle";
            this.darkModeToggle.Size = new System.Drawing.Size(64, 36);
            this.darkModeToggle.TabIndex = 2;
            this.darkModeToggle.TurnedOffColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.darkModeToggle.TurnedOnColor = System.Drawing.Color.Green;
            this.darkModeToggle.OnToggled += new System.EventHandler(this.darkModeToggle_OnToggled);
            // 
            // discordRpcToggle
            // 
            this.discordRpcToggle.Checked = true;
            this.discordRpcToggle.ForeColor = System.Drawing.Color.White;
            this.discordRpcToggle.Location = new System.Drawing.Point(8, 173);
            this.discordRpcToggle.Name = "discordRpcToggle";
            this.discordRpcToggle.Size = new System.Drawing.Size(64, 36);
            this.discordRpcToggle.TabIndex = 2;
            this.discordRpcToggle.TurnedOffColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.discordRpcToggle.TurnedOnColor = System.Drawing.Color.Green;
            this.discordRpcToggle.OnToggled += new System.EventHandler(this.checkUpdatesToggle_OnToggled);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft JhengHei Light", 14F);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(75, 179);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(188, 24);
            this.label5.TabIndex = 3;
            this.label5.Text = "Интеграция Discord";
            // 
            // currencyComboBox
            // 
            this.currencyComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.currencyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.currencyComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.currencyComboBox.Font = new System.Drawing.Font("Microsoft JhengHei Light", 14F);
            this.currencyComboBox.ForeColor = System.Drawing.Color.White;
            this.currencyComboBox.FormattingEnabled = true;
            this.currencyComboBox.Location = new System.Drawing.Point(90, 218);
            this.currencyComboBox.Name = "currencyComboBox";
            this.currencyComboBox.Size = new System.Drawing.Size(173, 32);
            this.currencyComboBox.TabIndex = 6;
            this.currencyComboBox.SelectedIndexChanged += new System.EventHandler(this.bufferSpeedNUP_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft JhengHei Light", 14F);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(4, 221);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 24);
            this.label6.TabIndex = 3;
            this.label6.Text = "Валюта:";
            // 
            // currencyBindingSource
            // 
            this.currencyBindingSource.DataSource = typeof(FloatToolGUI.Utils.Currency);
            // 
            // currencyBindingSource1
            // 
            this.currencyBindingSource1.DataSource = typeof(FloatToolGUI.Utils.Currency);
            // 
            // SettingsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(390, 364);
            this.Controls.Add(this.currencyComboBox);
            this.Controls.Add(this.bufferSpeedNUP);
            this.Controls.Add(this.resetChangesBtn);
            this.Controls.Add(this.saveChangesBtn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.discordRpcToggle);
            this.Controls.Add(this.checkUpdatesToggle);
            this.Controls.Add(this.soundToggle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.darkModeToggle);
            this.Controls.Add(this.panel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки";
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bufferSpeedNUP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currencyBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currencyBindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button closeBtn;
        private CustomControls.CustomToggleSwitch darkModeToggle;
        private System.Windows.Forms.Label label1;
        private CustomControls.CustomToggleSwitch soundToggle;
        private System.Windows.Forms.Label label2;
        private CustomControls.CustomToggleSwitch checkUpdatesToggle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button saveChangesBtn;
        private System.Windows.Forms.Button resetChangesBtn;
        private System.Windows.Forms.NumericUpDown bufferSpeedNUP;
        private System.Windows.Forms.Label label4;
        private CustomControls.CustomToggleSwitch discordRpcToggle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox currencyComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.BindingSource currencyBindingSource;
        private System.Windows.Forms.BindingSource currencyBindingSource1;
    }
}