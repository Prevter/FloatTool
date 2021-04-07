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

namespace FloatToolGUI
{
    public partial class Benchmark : Form
    {
        private static bool NextCombination(IList<int> num, int n, int k)
        {
            bool finished;
            var changed = finished = false;
            if (k <= 0) return false;
            for (var i = k - 1; !finished && !changed; i--)
            {
                if (num[i] < n - 1 - (k - 1) + i)
                {
                    num[i]++;
                    if (i < k - 1)
                        for (var j = i + 1; j < k; j++)
                            num[j] = num[j - 1] + 1;
                    changed = true;
                }
                finished = i == 0;
            }
            return changed;
        }

        private static IEnumerable Combinations<T>(IEnumerable<T> elements, int k, int start, int skip)
        {
            var elem = elements.ToArray();
            var size = elem.Length;
            if (k > size) yield break;
            var numbers = new int[k];
            for (var i = 0; i < k; i++)
                numbers[i] = i;
            int step = 0;
            do
            {
                if ((step + start) % skip == 0)
                    yield return numbers.Select(n => elem[n]);
                step++;
            } while (NextCombination(numbers, size, k));
        }

        public enum SearchMode
        {
            Less,
            Equal,
            Greater
        }
        SearchMode CurrentSearchMode = SearchMode.Equal;

        public static string setprecission(double number, int figures)
        {
            int e = 0;
            while (number >= 10.0)
            {
                e += 1;
                number /= 10;
            }
            while (number < 1.0)
            {
                e -= 1;
                number *= 10;
            }
            figures--;
            number = (float)Math.Round(number, figures);
            figures += 0 - e;
            while (e > 0)
            {
                number *= 10;
                e -= 1;
            }
            while (e < 0)
            {
                number /= 10;
                e += 1;
            }
            if (figures < 0)
                figures = 0;
            return number.ToString($"f{figures}", CultureInfo.InvariantCulture);
        }
        static public decimal craft(List<InputSkin> ingridients, float minFloat, float maxFloat)
        {
            decimal avgFloat = 0;
            foreach (InputSkin f in ingridients)
            {
                avgFloat += (decimal)f.WearValue;
            }
            avgFloat /= 10;
            return ((decimal)(maxFloat - minFloat) * avgFloat) + (decimal)minFloat;
        }
        static public string craftF(List<InputSkin> ingridients, float minFloat, float maxFloat)
        {
            float avgFloat = 0;
            float[] arrInput = new float[10];
            for (int i = 0; i < 10; i++)
            {
                arrInput[i] = Convert.ToSingle(ingridients[i].WearValue);
            }
            foreach (float f in arrInput)
            {
                avgFloat += Convert.ToSingle(f);
            }
            avgFloat /= 10;
            return setprecission(((maxFloat - minFloat) * avgFloat) + minFloat, 10);
        }

        public void parseCraft(List<InputSkin> inputs, List<Skin> outputs, string want)
        {
            //List<double> results = new List<double>();
            decimal wantFloat;
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

            var threads = Environment.ProcessorCount;
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
            versionLabel2.Text = version;
            threadCountLabel.Text = $"{Environment.ProcessorCount} Threads";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
                cpuNameLabel.Text = mo["Name"].ToString().Trim();

            thread1 = new Thread(runCycle);
            LoadStats();
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

        string uri = "https://prevterapi.000webhostapp.com/";

        private void AddCpuToList(string cpu, string speed, string ver)
        {
            Color backColor = cpu.Contains("AMD") ? Color.FromArgb(157, 0, 20) : (cpu.Contains("Intel") ? Color.FromArgb(0, 125, 195) : Color.FromArgb(56, 56, 56));
            Color foreColor = Color.White;

            var tmpPanel = new Panel
            {
                BackColor = backColor,
                Size = new Size(350, 35),
                Margin = new Padding(0,0,0,5)
            };
            tmpPanel.Controls.Add(new Label
            {
                Location = new Point(3,3),
                Font = new Font("Microsoft JhengHei UI Light", 8f),
                Text = cpu,
                AutoSize = true,
                ForeColor = foreColor
            });
            tmpPanel.Controls.Add(new Label
            {
                Location = new Point(3, 18),
                Font = new Font("Microsoft JhengHei UI Light", 8f),
                Text = $"{speed} к/с ({ver})",
                AutoSize = true,
                ForeColor = foreColor
            });
            flowLayoutPanel1.Controls.Add(tmpPanel);
        }

        private void LoadStats()
        {
            flowLayoutPanel1.Controls.Clear();
            WebRequest request = WebRequest.Create($"{uri}getBenchmarks.php");
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
                if(responseFromServer.Contains('|'))
                {
                    foreach (var cpu in responseFromServer.Remove(responseFromServer.Length - 1).Split('&'))
                    {
                        var items = cpu.Split('|');
                        AddCpuToList(items[0], items[1], items[2]);
                    }
                }
            }
            response.Close();
        }

        private void submitScoreBtn_Click(object sender, EventArgs e)
        {
            submitScoreBtn.Enabled = false;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create($"{uri}addBenchmark.php?cpu={cpuNameLabel.Text} ({threadCountLabel.Text})&speed={speedLabel.Text.Split(' ')[0]}");
            req.UserAgent = $"FloatTool/{versionLabel2.Text}";
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            res.Close();
            MessageBox.Show("Ваш результат был принят.");
        }

        private void closeBtn_Click(object sender, EventArgs e)
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

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel7_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }
}
