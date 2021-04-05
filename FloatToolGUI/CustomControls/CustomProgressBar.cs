using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloatToolGUI
{
    public partial class CustomProgressBar : UserControl
    {
        private int minimum = 0;
        private int maximum = 100;
        private float progressValue = 0;
        private Color progressColor = Color.Green;
        private Font progressFont = new Font("Microsoft JhengHei UI", 11.25f, FontStyle.Bold);

        /// <summary>
        /// Цвет заливки прогресс-бара
        /// </summary>
        public Color ProgressColor
        {
            get { return progressColor; }
            set { progressColor = value; Invalidate(); }
        }

        /// <summary>
        /// Шрифт процентов
        /// </summary>
        public Font ProgressFont
        {
            get { return progressFont; }
            set { progressFont = value; Invalidate(); }
        }

        /// <summary>
        /// Текущее значение прогресс-бара
        /// </summary>
        public float Value
        {
            get { return progressValue; }
            set { progressValue = value; Invalidate(); }
        }

        /// <summary>
        /// Минимальное значение прогресс-бара
        /// </summary>
        public int Minimum
        {
            get { return minimum; }
            set { minimum = value; Invalidate(); }
        }

        /// <summary>
        /// Максимальное значение прогресс-бара
        /// </summary>
        public int Maximum
        {
            get { return maximum; }
            set { maximum = value; Invalidate(); }
        }

        public CustomProgressBar()
        {
            InitializeComponent();
        }

        private void CustomProgressBar_Paint(object sender, PaintEventArgs e)
        {
            float fill = (Value - Minimum) / (Maximum - Minimum);
            Brush brush = new SolidBrush(ProgressColor);
            e.Graphics.FillRectangle(brush, new RectangleF(0, 0, Width * fill, Height));

            using (ProgressFont)
            {
                Rectangle rect2 = new Rectangle(150, 10, 130, 140);
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
                TextRenderer.DrawText(e.Graphics, $"{Math.Floor(fill * 100)}%", ProgressFont, new Rectangle(0, 0, Width, Height), ForeColor, flags);
            }
        }
    }
}
