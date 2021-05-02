using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FloatToolGUI.Calculation;

namespace FloatToolGUI
{
    public partial class Benchmark : Form
    {
        public enum SearchMode
        {
            Less,
            Equal,
            Greater
        }
        SearchMode CurrentSearchMode = SearchMode.Equal;

        public void parseCraft(List<InputSkin> inputs, List<Skin> outputs, string want)
        {
            decimal wantFloat = 1;
            if (CurrentSearchMode != SearchMode.Equal)
                decimal.TryParse(want, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out wantFloat);

            foreach (var item in outputs)
            {
                decimal flotOrigin = Math.Round(craft(inputs, item.MinFloat, item.MaxFloat), 14);
                string flot = craftF(inputs, item.MinFloat, item.MaxFloat);

                if (
                    ((flotOrigin.ToString(CultureInfo.InvariantCulture).StartsWith(want)) && CurrentSearchMode == SearchMode.Equal) ||
                    ((flotOrigin < wantFloat) && CurrentSearchMode == SearchMode.Less) ||
                    ((flotOrigin > wantFloat) && CurrentSearchMode == SearchMode.Greater)
                )
                {
                    return;
                }
            }
        }

        public void secndThread(List<Skin> craftList, string wanted, List<InputSkin> pool, int start, int skip)
        {
            foreach (IEnumerable<InputSkin> pair in Combinations(pool, 10, start, skip))
            {
                parseCraft(pair.ToList(), craftList, wanted);
                currComb++;
            }
        }

        private void runCycle()
        {
            Console.WriteLine("Thread loaded!");
        }

        List<Thread> t2 = new List<Thread>();
        int currComb;
        Thread thread1;

        private void StartCalculation()
        {
            Logger.Log($"[{DateTime.Now}]: Started benchmark");
            currComb = 0;
            List<Skin> outcomes = new List<Skin>();
            outcomes.Add(new Skin("AK-47 | Safari Mesh", 0.06f, 0.8f, Quality.Industrial));

            Invoke((MethodInvoker)(() =>
            {
                customProgressBar1.Value = 0;
            }
            ));

            double[] pool = {
                0.246938750147820, 0.196652039885521, 0.154839321970940,
                0.333326697349548, 0.163415759801865, 0.291821509599686,
                0.374309629201889, 0.378754675388336, 0.231419935822487,
                0.311867892742157, 0.374067693948746, 0.377068012952805,
                0.244467452168465, 0.355135351419449, 0.352264583110809,
                0.227853879332542, 0.340960860252380, 0.375657349824905,
                0.157685652375221, 0.217334255576134
            };

            List<InputSkin> inputSkins = new List<InputSkin>();
            foreach (double f in pool) inputSkins.Add(new InputSkin(f, 0.03f, Currency.USD));

            Stopwatch timer = Stopwatch.StartNew();

            int threads = (int)benchmarkThreadsNumericUpdown.Value;
            try
            {
                for (int i = 0; i < threads; i++)
                {
                    Thread newThread = new Thread(() => secndThread(outcomes, "1", inputSkins, i, threads));
                    newThread.Start();
                    t2.Add(newThread);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            while (true)
            {
                bool okey = true;
                foreach (Thread t in t2)
                {
                    if (t.IsAlive)
                    {
                        okey = false;
                        break;
                    }
                }
                if (okey) break;
            }

            timer.Stop();
            TimeSpan timespan = timer.Elapsed;
            Logger.Log($"[{DateTime.Now}]: Benchmark ended, speed = {Math.Round(currComb / timespan.TotalSeconds)}");
            Invoke((MethodInvoker)(() =>
            {
                submitScoreBtn.Enabled = true;
                speedLabel.Text = $"{Math.Round(currComb / timespan.TotalSeconds)} к/с";
                currComb = 184756;
                thread1.Abort();
            }
            ));
        }

        public Benchmark(string version)
        {
            InitializeComponent();
            Logger.Log($"[{DateTime.Now}]: Opened benchmark window");
            versionLabel2.Text = version;
            threadCountLabel.Text = $"{Environment.ProcessorCount} Threads";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
                cpuNameLabel.Text = mo["Name"].ToString().Trim();
            Logger.Log($"[{DateTime.Now}]: CPU Name: {cpuNameLabel.Text} ({threadCountLabel.Text})");
            thread1 = new Thread(runCycle);
            benchmarkThreadsNumericUpdown.Value = Environment.ProcessorCount;
            warningPic.Image = SystemIcons.Warning.ToBitmap();
            Thread t = new Thread(new ThreadStart(LoadStats));
            t.Start();
        }

        private void startBenchmarkBtn_Click(object sender, EventArgs e)
        {
            thread1.Abort();
            thread1 = new Thread(StartCalculation);
            thread1.Start();
        }

        private void Updater_Tick(object sender, EventArgs e)
        {
            customProgressBar1.Value = currComb;
        }

        string uri = "http://prevter.tk/";

        private void AddCpuToList(string cpu, string speed, string ver, bool bigMargin)
        {
            Color backColor = cpu.Contains("AMD") ? Color.FromArgb(157, 0, 20) : (cpu.Contains("Intel") ? Color.FromArgb(0, 125, 195) : (cpu.Contains("NVidia") ? Color.FromArgb(118, 185, 0) : Color.FromArgb(56, 56, 56)));
            Color foreColor = cpu.Contains("NVidia") ? Color.Black : Color.White;

            var tmpPanel = new Panel
            {
                BackColor = backColor,
                Size = new Size(350, 35),
                Margin = new Padding(0,0,0,bigMargin?20:5)
            };
            tmpPanel.Controls.Add(new Label
            {
                Location = new Point(3,3),
                Font = new Font("Inter", 8f),
                Text = cpu,
                AutoSize = true,
                ForeColor = foreColor
            });
            tmpPanel.Controls.Add(new Label
            {
                Location = new Point(3, 18),
                Font = new Font("Inter", 8f),
                Text = $"{speed} к/с ({ver})",
                AutoSize = true,
                ForeColor = foreColor
            });
            Invoke((MethodInvoker)(() =>
                {
                    benchmarkScoreboardLayout.Controls.Add(tmpPanel);
                })
            );
            
        }

        private void LoadStats()
        {
            try
            {
                WebRequest request = WebRequest.Create($"{uri}getBenchmarks.php");
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();

                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    Logger.Log($"[{DateTime.Now}]: Loaded benchmark scores");
                    Console.WriteLine(responseFromServer);
                    if(responseFromServer.Contains('|'))
                    {
                        var results = responseFromServer.Remove(responseFromServer.Length - 1).Split('&');
                        int index = 0;
                        Invoke((MethodInvoker)(() => { benchmarkScoreboardLayout.Controls.Clear(); }));
                        foreach (var cpu in results)
                        {
                            index++;
                            var items = cpu.Split('|');
                            AddCpuToList(items[0], items[1], items[2], index == results.Length);
                        }
                    }
                }
                response.Close();
            }
            catch(Exception ex)
            {
                Invoke((MethodInvoker)(() => {
                    benchmarkScoreboardLayout.Controls.Clear();
                    benchmarkScoreboardLayout.Controls.Add(new Label
                    {
                        Text = "Произошла ошибка подключения",
                        AutoSize = true,
                        ForeColor = Color.White
                    });
                }));
                Logger.Log($"[{DateTime.Now}]: {{EXCEPTION}} {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                Logger.SaveCrashReport();
            }
        }

        private void submitScoreBtn_Click(object sender, EventArgs e)
        {
            try
            {
                submitScoreBtn.Enabled = false;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create($"{uri}addBenchmark.php?cpu={cpuNameLabel.Text} ({benchmarkThreadsNumericUpdown.Value.ToString()})&speed={speedLabel.Text.Split(' ')[0]}");
                req.UserAgent = $"FloatTool/{versionLabel2.Text}";
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                res.Close();
                MessageBox.Show("Ваш результат был принят.");
                Logger.Log($"[{DateTime.Now}] Recorded benchmark score: {speedLabel.Text.Split(' ')[0]}");
            }
            catch (Exception ex)
            {
                Logger.Log($"[{DateTime.Now}]: {{EXCEPTION}} {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                Logger.SaveCrashReport();
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Logger.Log($"[{DateTime.Now}]: Benchmark window closed");
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

        private void benchmarkThreadsNumericUpdown_ValueChanged(object sender, EventArgs e)
        {
            if (benchmarkThreadsNumericUpdown.Value > Environment.ProcessorCount)
            {
                warningPic.Enabled = true;
                warningPic.Visible = true;
            }
            else
            {
                warningPic.Enabled = false;
                warningPic.Visible = false;
            }
        }

        private void updateBenchmarksButton_Click(object sender, EventArgs e)
        {
            benchmarkScoreboardLayout.Controls.Clear();

            benchmarkScoreboardLayout.Controls.Add(new Panel { 
                Size = new Size(300, 100)
            });
            benchmarkScoreboardLayout.Controls.Add(new Label
            {
                Size = new Size(361, 19),
                Text = "Загрузка бенчмарков...",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            });
            benchmarkScoreboardLayout.Controls.Add(new PictureBox
            {
                Size = new Size(362, 64),
                Image = Properties.Resources.loading,
                SizeMode = PictureBoxSizeMode.Zoom
            });

            Thread t = new Thread(new ThreadStart(LoadStats));
            t.Start();
        }
    }
}
