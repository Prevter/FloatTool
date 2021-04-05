namespace FloatToolGUI
{
    partial class CustomProgressBar
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
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CustomProgressBar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CustomProgressBar";
            this.Size = new System.Drawing.Size(306, 24);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CustomProgressBar_Paint);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
