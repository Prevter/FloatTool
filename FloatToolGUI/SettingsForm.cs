using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloatToolGUI
{
    public partial class SettingsForm : Form
    {
        RegistryKey registryData;

        public SettingsForm()
        {
            InitializeComponent();
            registryData = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\FloatTool");
            if (registryData == null)
            {
                registryData = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\FloatTool");
                registryData.SetValue("darkMode", true);
                registryData.SetValue("sound", true);
                registryData.SetValue("updateCheck", true);
                registryData.SetValue("bufferSpeed", 250);
                registryData.Close();
            }
            else
            {
                darkModeToggle.Checked = Convert.ToBoolean(registryData.GetValue("darkMode"));
                soundToggle.Checked = Convert.ToBoolean(registryData.GetValue("sound"));
                checkUpdatesToggle.Checked = Convert.ToBoolean(registryData.GetValue("updateCheck"));
                bufferSpeedNUP.Value = (int)registryData.GetValue("bufferSpeed");
            }
        }

        private void CloseForm(object sender, EventArgs e)
        {
            Close();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();


        private void DragWindowMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void darkModeToggle_OnToggled(object sender, EventArgs e)
        {
            saveChangesBtn.Enabled = true;
        }

        private void soundToggle_OnToggled(object sender, EventArgs e)
        {
            saveChangesBtn.Enabled = true;
        }

        private void checkUpdatesToggle_OnToggled(object sender, EventArgs e)
        {
            saveChangesBtn.Enabled = true;
        }

        private void bufferSpeedNUP_ValueChanged(object sender, EventArgs e)
        {
            saveChangesBtn.Enabled = true;
        }

        private void saveChangesBtn_Click(object sender, EventArgs e)
        {
            registryData = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\FloatTool", true);
            if (registryData == null)
            {
                registryData = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\FloatTool");
                registryData.SetValue("darkMode", darkModeToggle.Checked);
                registryData.SetValue("sound", soundToggle.Checked);
                registryData.SetValue("updateCheck", checkUpdatesToggle.Checked);
                registryData.SetValue("bufferSpeed", bufferSpeedNUP.Value);
                registryData.Close();
            }
            else
            {
                registryData.SetValue("darkMode", darkModeToggle.Checked);
                registryData.SetValue("sound", soundToggle.Checked);
                registryData.SetValue("updateCheck", checkUpdatesToggle.Checked);
                registryData.SetValue("bufferSpeed", (int)bufferSpeedNUP.Value);
                registryData.Close();
            }
            saveChangesBtn.Enabled = false;
        }

        private void resetChangesBtn_Click(object sender, EventArgs e)
        {
            registryData = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\FloatTool");
            if (registryData == null)
            {
                registryData = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\FloatTool");
                registryData.SetValue("darkMode", true);
                registryData.SetValue("sound", true);
                registryData.SetValue("updateCheck", true);
                registryData.SetValue("bufferSpeed", 250);
                registryData.Close();
            }
            else
            {
                darkModeToggle.Checked = Convert.ToBoolean(registryData.GetValue("darkMode"));
                soundToggle.Checked = Convert.ToBoolean(registryData.GetValue("sound"));
                checkUpdatesToggle.Checked = Convert.ToBoolean(registryData.GetValue("updateCheck"));
                bufferSpeedNUP.Value = (int)registryData.GetValue("bufferSpeed");
            }
            saveChangesBtn.Enabled = false;
        }
    }
}
