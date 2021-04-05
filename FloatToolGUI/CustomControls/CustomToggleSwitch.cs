using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace FloatToolGUI.CustomControls
{
    public partial class CustomToggleSwitch : UserControl
    {
        private bool check = false;
        public bool Checked { get {return check; } set {check = value; Invalidate(); } }
        public Color TurnedOffColor { get; set; }

        public Color TurnedOnColor { get; set; }

        public CustomToggleSwitch()
        {
            InitializeComponent();
        }


        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when toggled")]
        public event EventHandler OnToggled;

        private void CustomToggleSwitch_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush back = new SolidBrush(Checked ? TurnedOnColor : TurnedOffColor);
            Brush fore = new SolidBrush(ForeColor);
            g.FillPath(back, RoundedRect(new Rectangle(0,0,Width,Height), Height / 2));
            if (Checked)
                g.FillEllipse(fore, new Rectangle(Width - Height + 3, 3, Height - 6, Height - 6));
            else
                g.FillEllipse(fore, new Rectangle(3, 3, Height - 6, Height - 6));
        }

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void CustomToggleSwitch_MouseDown(object sender, MouseEventArgs e)
        {
            if (Enabled)
            {
                Checked = !Checked;
                OnToggled.Invoke(sender, e);
                Invalidate();
            }
        }
    }
}
