using DiscordRPC;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FloatToolGUI
{
    public partial class FloatTool : Form
    {
        Thread thread1;
        public bool muteSound = false;

        public StringBuilder ConsoleBuffer;

        public enum SearchMode
        {
            Less,
            Equal,
            Greater
        }

        public SearchMode CurrentSearchMode = SearchMode.Equal;

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
            {
                figures = 0;
            }

            return number.ToString($"f{figures}");
        }
        static public decimal craft(double[] ingridients, float minFloat, float maxFloat)
        {
            decimal avgFloat = 0;
            foreach (double f in ingridients)
            {
                avgFloat += (decimal)f;
            }
            avgFloat /= 10;
            return ((decimal)(maxFloat - minFloat) * avgFloat) + (decimal)minFloat;
        }
        static public string craftF(string[] ingridients, float minFloat, float maxFloat)
        {
            float avgFloat = 0;
            float[] arrInput = new float[10];
            for (int i = 0; i < 10; i++)
            {
                arrInput[i] = Convert.ToSingle(ingridients[i].Replace(".", ","));
            }
            foreach (float f in arrInput)
            {
                
                avgFloat += Convert.ToSingle(f);
            }
            avgFloat /= 10;
            return setprecission(((maxFloat - minFloat) * avgFloat) + minFloat, 10);
        }
        static public string getNextRarity(string rarity)
        {
            if (rarity == "Consumer")
            {
                return "Industrial";
            }
            else if (rarity == "Industrial")
            {
                return "Mil-Spec";
            }
            else if (rarity == "Mil-Spec")
            {
                return "Restricted";
            }
            else if (rarity == "Restricted")
            {
                return "Classified";
            }
            else if (rarity == "Classified")
            {
                return "Covert";
            }
            return "Nothing";
        }
        static public string getSkinData(string name)
        {
            name = name.Replace("StatTrak™ ",""); //remove stattrack
            using (StreamReader r = new StreamReader("itemData.json"))
            {
                string json = r.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);
                string collection = "";
                string rarity = "";
                foreach (var skin in items)
                {
                    //Console.WriteLine(skin["name"].ToString());
                    if (skin["name"].ToString() == name)
                    {
                        collection = skin["case"].ToString();
                        rarity = skin["rarity"].ToString();
                        break;
                    }
                }
                return collection + "," + rarity;//json;
            }
        }


        public void parseCraft(double[] inputs, List<dynamic> outputs, string want, bool wasSort, bool asc)
        {
            //List<double> results = new List<double>();
            decimal wantFloat;
            decimal.TryParse(want, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out wantFloat);
            
            foreach (var item in outputs)
            {
                //want = want.Replace(".", ",");
                float minWear = item["minWear"];
                float maxWear = item["maxWear"];
                decimal flotOrigin = Math.Round(craft(inputs.ToArray(), minWear, maxWear), 14);

                string[] inputStr = new string[10];
                for(int i = 0; i < 10; i++)
                {
                    inputStr[i] = "" + inputs[i];
                }

                string flot = craftF(inputStr, minWear, maxWear);
                //Console.WriteLine(flotOrigin + " | " + flot);
                //Debug.WriteLine("[DEBUG] flot = " + flot);
                // if (wasSort && ((!asc && (double.Parse(flot) > double.Parse(want))) || (asc && (double.Parse(flot) < double.Parse(want))))) {
                //     okSort = true;
                //}
                /*flot.StartsWith(want.Replace(".", ",")) ||*/
                if (
                    ((flotOrigin.ToString(CultureInfo.InvariantCulture).StartsWith(want)) && CurrentSearchMode == SearchMode.Equal) ||
                    ((flotOrigin < wantFloat) && CurrentSearchMode == SearchMode.Less) ||
                    ((flotOrigin > wantFloat) && CurrentSearchMode == SearchMode.Greater)
                )
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        ConsoleBuffer.Append($"[{DateTime.Now.ToString("HH:mm:ss")}] Коомбинация найдена!{Environment.NewLine}");
                        ConsoleBuffer.Append($"Возможный флоат: {flotOrigin}{Environment.NewLine}");
                        ConsoleBuffer.Append($"Проверочный флоат: {flot}{Environment.NewLine}");
                        ConsoleBuffer.Append("Список флоатов: [");
                        if (!muteSound)
                        {
                            //play sound
                            SoundPlayer player = new SoundPlayer(Properties.Resources.notification);
                            player.Play();
                        }
                        client.SetPresence(new RichPresence()
                        {
                            Details = "Нашёл комбинацию!!!",
                            State = "Крафчу " + flotOrigin,
                            Assets = new Assets()
                            {
                                LargeImageKey = "icon",
                                LargeImageText = "FloatTool"
                            }
                        });
                        for (int i = 0; i < 10; i++)
                        {
                            ConsoleBuffer.Append(Math.Round(inputs[i], 14).ToString().Replace(",","."));
                            if (i != 9)
                            {
                                ConsoleBuffer.Append(", ");
                            }
                            else
                            {
                                ConsoleBuffer.Append("]" + Environment.NewLine + "=====================================" + Environment.NewLine);
                            }
                        }
                    }
                    ));
                    
                    //textBox2.AppendText( "IEEE754: " + flot + Environment.NewLine;
                    return;
                }
            }
        }

        public void SwitchEnabled()
        {
            weaponTypeBox.Enabled = !weaponTypeBox.Enabled;
            weaponSkinBox.Enabled = !weaponSkinBox.Enabled;
            fullSkinName.Enabled = !fullSkinName.Enabled;
            weaponQualityBox.Enabled = !weaponQualityBox.Enabled;
            searchFloatInput.Enabled = !searchFloatInput.Enabled;
            quantityInput.Enabled = !quantityInput.Enabled;
            skipValueInput.Enabled = !skipValueInput.Enabled;
            stattrackCheckBox.Enabled = !stattrackCheckBox.Enabled;
            sortCheckBox.Enabled = !sortCheckBox.Enabled;
            ascendingCheckBox.Enabled = !ascendingCheckBox.Enabled;
            multithreadCheckBox.Enabled = !multithreadCheckBox.Enabled;
            if (threadCountInput.Enabled && !multithreadCheckBox.Enabled)
                threadCountInput.Enabled = false;
        }
        public void updateSearchStr()
        {
            string search = "";
            if (stattrackCheckBox.Checked)
            {
                search += "StatTrak™ ";
            }
            search += weaponTypeBox.Text;
            search += " | ";
            search += weaponSkinBox.Text;
            search += " (" + weaponQualityBox.Text + ")";
            fullSkinName.Text = search;
        }
        public FloatTool()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            ConsoleBuffer = new StringBuilder();
        }
        public DiscordRpcClient client;
        private bool darkTheme = true;

        private void Form1_Load(object sender, EventArgs e)
        {
            weaponSkinBox.Items.Clear();
            using (StreamReader r = new StreamReader("itemData.json"))
            {
                string json = r.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);
                foreach (var skin in items)
                {

                    if (skin["name"].ToString().Split('|')[0].TrimEnd() == weaponTypeBox.Text)
                    {
                        Console.WriteLine(skin["name"].ToString().Split('|')[1].Remove(0, 1));
                        weaponSkinBox.Items.Add(skin["name"].ToString().Split('|')[1].Remove(0, 1));
                    }
                }
            }
            updateSearchStr();
            darkModeSwitchBtn.Text = darkTheme ? "🌙" : "☀";
            client = new DiscordRpcClient("824349399688937543");

            //Subscribe to events
            client.OnReady += (sender2, e2) =>
            {
                Console.WriteLine("Received Ready from user {0}", e2.User.Username);
            };

            client.OnPresenceUpdate += (sender2, e2) =>
            {
                Console.WriteLine("Received Update! {0}", e2.Presence);
            };

            //Connect to the RPC
            client.Initialize();

            client.SetPresence(new RichPresence()
            {
                Details = "Настраиваю поиск",
                Assets = new Assets()
                {
                    LargeImageKey = "icon",
                    LargeImageText = "FloatTool"
                }
            });

            thread1 = new Thread(runCycle);
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            this.threadCountInput.Value = Environment.ProcessorCount;
        }

        private void runCycle()
        {
            Console.WriteLine("Thread loaded!");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            weaponSkinBox.Items.Clear();
            using (StreamReader r = new StreamReader("itemData.json"))
            {
                string json = r.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);
                foreach (var skin in items)
                {
                    
                    if (skin["name"].ToString().Split('|')[0].TrimEnd() == weaponTypeBox.Text)
                    {
                        //Console.WriteLine(skin["name"].ToString().Split('|')[1].Remove(0,1));
                        weaponSkinBox.Items.Add(skin["name"].ToString().Split('|')[1].Remove(0,1));
                    }
                }
            }
            updateSearchStr();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateSearchStr();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            updateSearchStr();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateSearchStr();
        }

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
                if((step+start)%skip == 0)
                {
                    yield return numbers.Select(n => elem[n]);
                }
                step++;
            } while (NextCombination(numbers, size, k));
        }

        public void secndThread(List<dynamic> craftList, string wanted, double[] pool, int start, int skip)
        {
            foreach (IEnumerable<double> pair in Combinations(pool, 10, start, skip))
            {
                parseCraft(pair.ToArray(), craftList, wanted, sortCheckBox.Checked, ascendingCheckBox.Checked);
                currComb++;
                //Console.WriteLine(currComb);
            }
        }
        public BigInteger Fact(int number)
        {
            if (number == 1)
                return 1;
            else
                return number * Fact(number - 1);
        }

        public List<Thread> t2 = new List<Thread>();
        BigInteger totalComb = 0;
        BigInteger currComb = 0;
        private void StartCalculation()
        {
            client.SetPresence(new RichPresence()
            {
                Details = "Начал поиск",
                State = "Ищу " + searchFloatInput.Text,
                Timestamps = Timestamps.Now,
                Assets = new Assets()
                {
                    LargeImageKey = "icon",
                    LargeImageText = "FloatTool"
                }
            });

            totalComb = quantityInput.Value == 10 ? 1 : Fact((int)quantityInput.Value) / (Fact(10) * Fact((int)quantityInput.Value - 10));
            currComb = 0;
            this.Invoke((MethodInvoker)(() =>
                {
                    outputConsoleBox.Text = "Добро пожаловать в FloatTool!" + Environment.NewLine + "Инструмент для создания флоатов при помощи крафтов CS:GO" + Environment.NewLine;
                    outputConsoleBox.AppendText( "Время начала процесса: " + DateTime.Now.ToString("HH:mm:ss tt") + Environment.NewLine);
                    startBtn.Text = "Стоп";
                    fullSkinName.SelectionStart = fullSkinName.Text.Length;
                    outputConsoleBox.ScrollToCaret();
                }
            ));
            
            string count = "" + quantityInput.Value;
            string start = "" + skipValueInput.Value;
            string wanted = searchFloatInput.Text;
            string q = fullSkinName.Text;
            string url = "https://steamcommunity.com/market/listings/730/" + q + "/render/?query=&language=russian&count=" + count + "&start=" + start + "&currency=5";
            Console.WriteLine(url);
            this.Invoke((MethodInvoker)(() =>
            {
                outputConsoleBox.AppendText( "Загрузка скинов с торговой площадки..." + Environment.NewLine);
                downloadProgressBar.Maximum = int.Parse(count);
                downloadProgressBar.Value = 0;
                fullSkinName.SelectionStart = fullSkinName.Text.Length;
                outputConsoleBox.ScrollToCaret();
                combinationsStatusLabel.Text = $"Проверено комбинаций: 0 / {totalComb}";
            }
            ));
            
            List<double> floats = new List<double>();
            using (WebClient wc = new WebClient())
            {
                string json = wc.DownloadString(url);
                Console.WriteLine(json);
                dynamic r = JsonConvert.DeserializeObject(json);
                this.Invoke((MethodInvoker)(() =>
                    {
                        outputConsoleBox.AppendText( "Получение флоатов..." + Environment.NewLine);
                        fullSkinName.SelectionStart = fullSkinName.Text.Length;
                        outputConsoleBox.ScrollToCaret();
                    }
                ));
                int counter = 0;
                foreach (var el in r["listinginfo"])
                {
                    counter++;
                    string lid = r["listinginfo"][el.Name]["listingid"];
                    string aid = r["listinginfo"][el.Name]["asset"]["id"];
                    string link = r["listinginfo"][el.Name]["asset"]["market_actions"][0]["link"];
                    url = "https://api.csgofloat.com/?url=" + link.Replace("%assetid%", aid).Replace("%listingid%", lid);
                    using (WebClient wcf = new WebClient())
                    {
                        try
                        {
                            string jsonf = wcf.DownloadString(url);
                            dynamic rf = JsonConvert.DeserializeObject(jsonf);
                            //Debug.WriteLine("[DEBUG] " + counter + "/" + count + " load from csgofloat = " + jsonf);
                            floats.Add(Convert.ToDouble(rf["iteminfo"]["floatvalue"]));
                        }
                        catch
                        {
                            Console.Write("");
                        }
                    }
                    this.Invoke((MethodInvoker)(() =>
                    {
                        downloadProgressBar.Value = counter;
                    }
                    ));
                    
                }
            }
            if (sortCheckBox.Checked)
            {
                if (ascendingCheckBox.Checked)
                {
                    floats.Sort((a, b) => a.CompareTo(b));
                    Console.WriteLine("Sorted ascending");
                }
                else
                {
                    floats.Sort((a, b) => b.CompareTo(a));
                    Console.WriteLine("Sorted descending");
                }
            }
            //foreach (double v in floats) {
            //    Console.WriteLine(v);
            //}
            this.Invoke((MethodInvoker)(() =>
            {
                outputConsoleBox.AppendText( "Поиск ауткамов..." + Environment.NewLine);
                outputConsoleBox.SelectionStart = fullSkinName.Text.Length;
                /*string line = "[";
                foreach(var i in floats)
                    line += $"{i.ToString().Replace(',','.')}, ";
                line = line.Remove(line.Length - 2);
                textBox2.AppendText("Список флоатов:" + Environment.NewLine + line + "]" + Environment.NewLine);*/
                outputConsoleBox.ScrollToCaret();

            }
            ));
            string currData = getSkinData(q.Split('(')[0].TrimEnd());
            List<dynamic> craftList = new List<dynamic>();
            using (StreamReader r = new StreamReader("itemData.json"))
            {
                string json = r.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);
                foreach (var skin in items)
                {
                    //Console.WriteLine(skin["name"].ToString());
                    if (skin["case"].ToString() == currData.Split(',')[0])
                    {
                        if (skin["rarity"].ToString().Split(' ')[0] == getNextRarity(currData.Split(',')[1].Split(' ')[0]))
                        {
                            //Console.WriteLine(skin["name"].ToString());
                            craftList.Add(skin);
                        }
                    }
                }
            }
            this.Invoke((MethodInvoker)(() =>
            {
                outputConsoleBox.AppendText( "Ауткамы найдены! Начинаю подбор..." + Environment.NewLine + Environment.NewLine);
                fullSkinName.SelectionStart = fullSkinName.Text.Length;
                outputConsoleBox.ScrollToCaret();
            }
            ));

            double[] pool = floats.ToArray();

            var threads = 1;
            if (multithreadCheckBox.Checked)
            {
                threads = (int)threadCountInput.Value;
                try
                {
                    for (int i = 1; i < threads; i++)
                    {
                        Thread newThread = new Thread(() => secndThread(craftList, wanted, pool, i, threads));
                        newThread.Start();
                        t2.Add(newThread);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            
            
            foreach (IEnumerable<double> pair in Combinations(pool, 10, 0, threads))
            {
                parseCraft(pair.ToArray(), craftList, wanted, sortCheckBox.Checked, ascendingCheckBox.Checked);
                currComb++;
            }
            Console.WriteLine("Next group");

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

            this.Invoke((MethodInvoker)(() =>
                {
                    outputConsoleBox.AppendText( "Программа завершила проверку всех комбинаций!" + Environment.NewLine);
                    fullSkinName.SelectionStart = fullSkinName.Text.Length;
                    outputConsoleBox.ScrollToCaret();
                    thread1.Abort();
                    startBtn.Text = "Старт";
                    downloadProgressBar.Value = 0;
                    SwitchEnabled();
                }
            ));
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(startBtn.Text == "Старт") {
                thread1.Abort();
                thread1 = new Thread(StartCalculation);
                thread1.Start();
            }
            else
            {
                thread1.Abort();
                startBtn.Text = "Старт";
                downloadProgressBar.Value = 0;
                client.SetPresence(new RichPresence()
                {
                    Details = "Настраиваю поиск",
                    Assets = new Assets()
                    {
                        LargeImageKey = "icon",
                        LargeImageText = "FloatTool"
                    }
                });
                foreach (Thread t in t2)
                {
                    if (t.IsAlive)
                    {
                        t.Abort();
                    }
                }
            }
            SwitchEnabled();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            fullSkinName.SelectionStart = fullSkinName.Text.Length;
            outputConsoleBox.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateSearchStr();
            string skin = "";
            skin += weaponTypeBox.Text;
            skin += " | ";
            skin += weaponSkinBox.Text;
            //search += " (" + comboBox3.Text + ")";

            using (StreamReader r = new StreamReader("itemData.json"))
            {
                string json = r.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);
                foreach (var skn in items)
                {
                    if (skn["name"].ToString() == skin)
                    {
                        if (skn["highestRarity"] == "False")
                        {
                            if (floatRangeText(weaponQualityBox.Text, skn["minWear"].ToString(), skn["maxWear"].ToString()))
                            {
                                MessageBox.Show("Данный скин доступен к поиску!", "FloatTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            else
                            {
                                MessageBox.Show("Качество которое вы выбрали недоступно для этого скина.", "FloatTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Вы не можете делать контракты из скинов максимальной редкости в этой коллекции.", "FloatTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }

            MessageBox.Show("Такого скина не существует! Перепроверьте настройки.", "FloatTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        bool testOverlap(float x1, float x2, float y1, float y2)
        {
            return (x1 >= y1 && x1 <= y2) ||
                   (x2 >= y1 && x2 <= y2) ||
                   (y1 >= x1 && y1 <= x2) ||
                   (y2 >= x1 && y2 <= x2);
        }

        private bool floatRangeText(string text, string minVal, string maxVal)
        {
            Console.WriteLine(text + " in float range [" + minVal + ", " + maxVal + "]");
            float minWear = float.Parse(minVal.Replace('.',','));
            float maxWear = float.Parse(maxVal.Replace('.', ','));


            if (text == "Factory New")
            {
                return testOverlap(minWear, maxWear, 0, 0.07f);
            }
            else if (text == "Minimal Wear")
            {
                return testOverlap(minWear, maxWear, 0.07f, 0.15f);
            }
            else if (text == "Field-Tested")
            {
                return testOverlap(minWear, maxWear, 0.15f, 0.38f);
            }
            else if (text == "Well-Wornr")
            {
                return testOverlap(minWear, maxWear, 0.38f, 0.45f);
            }
            else if (text == "Battle-Scarred")
            {
                return testOverlap(minWear, maxWear, 0.45f, 1);
            }

            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            client.Invoke();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void panel9_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        void updateMuteIcon()
        {
            if (!muteSound)
            {
                if (darkTheme)
                {
                    soundBtnSwitch.Image = FloatToolGUI.Properties.Resources.unmutedWhite;
                }
                else
                {
                    soundBtnSwitch.Image = FloatToolGUI.Properties.Resources.unmutedBlack;
                }
            }
            else
            {
                if (darkTheme)
                {
                    soundBtnSwitch.Image = FloatToolGUI.Properties.Resources.mutedWhite;
                }
                else
                {
                    soundBtnSwitch.Image = FloatToolGUI.Properties.Resources.mutedBlack;
                }
            }
        }

        private void SoundSwitchButton_Click(object sender, EventArgs e)
        {
            if (muteSound)
            {
                muteSound = false;
                SoundPlayer player = new SoundPlayer(Properties.Resources.notification);
                player.Play();
            }
            else
            {
                muteSound = true;
            }
            updateMuteIcon();
        }

        private void OpenWikiButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://nemeshio.github.io/FloatTool-GUI/"); //https://github.com/Nemeshio/FloatTool-GUI/wiki/%D0%93%D0%BB%D0%B0%D0%B2%D0%BD%D0%B0%D1%8F
        }

        private void OpenGithubButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Nemeshio/FloatTool-GUI/");
        }

        private void OpenWebsiteButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://nemeshio.github.io/FloatTool-GUI/");
        }

        private void OpenAboutButton_Click(object sender, EventArgs e)
        {
            About aboutForm = new About();
            aboutForm.Show();
        }

        void ChangeTheme(bool dark)
        {
            if (dark)
            {
                outputConsoleBox.BackColor = Color.FromArgb(31, 31, 31);
                outputConsoleBox.ForeColor = Color.FromArgb(255, 255, 255);

                panel10.BackColor = Color.FromArgb(31, 31, 31);
                panel12.BackColor = Color.FromArgb(31, 31, 31);

                panel3.BackColor = Color.FromArgb(37, 37, 37);
                panel9.BackColor = Color.FromArgb(37, 37, 37);

                label1.ForeColor = Color.FromArgb(255, 255, 255);
                label2.ForeColor = Color.FromArgb(255, 255, 255);
                label3.ForeColor = Color.FromArgb(255, 255, 255);
                label4.ForeColor = Color.FromArgb(255, 255, 255);
                label5.ForeColor = Color.FromArgb(255, 255, 255);
                label6.ForeColor = Color.FromArgb(255, 255, 255);
                label7.ForeColor = Color.FromArgb(255, 255, 255);
                label8.ForeColor = Color.FromArgb(255, 255, 255);

                panel5.BackColor = Color.FromArgb(44, 44, 44);
                panel6.BackColor = Color.FromArgb(44, 44, 44);

                weaponTypeBox.BackColor = Color.FromArgb(32, 32, 32);
                weaponTypeBox.ForeColor = Color.FromArgb(150, 150, 150);
                weaponSkinBox.BackColor = Color.FromArgb(32, 32, 32);
                weaponSkinBox.ForeColor = Color.FromArgb(150, 150, 150);
                weaponQualityBox.BackColor = Color.FromArgb(32, 32, 32);
                weaponQualityBox.ForeColor = Color.FromArgb(150, 150, 150);

                minimizeBtn.ForeColor = Color.FromArgb(255, 255, 255);
                updateMuteIcon();
                closeBtn.ForeColor = Color.FromArgb(255, 255, 255);
                helpBtn.ForeColor = Color.FromArgb(255, 255, 255);
                darkModeSwitchBtn.ForeColor = Color.FromArgb(255, 255, 255);

                minimizeBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);
                soundBtnSwitch.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);
                closeBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);
                helpBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);
                darkModeSwitchBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);

                stattrackCheckBox.ForeColor = Color.FromArgb(255, 255, 255);
                sortCheckBox.ForeColor = Color.FromArgb(255, 255, 255);
                ascendingCheckBox.ForeColor = Color.FromArgb(255, 255, 255);

                checkPossibilityBtn.BackColor = Color.FromArgb(56, 56, 56);
                startBtn.BackColor = Color.FromArgb(56, 56, 56);
                checkPossibilityBtn.ForeColor = Color.FromArgb(255, 255, 255);
                startBtn.ForeColor = Color.FromArgb(255, 255, 255);

                checkPossibilityBtn.FlatAppearance.MouseOverBackColor = Color.FromName("WindowFrame");
                startBtn.FlatAppearance.MouseOverBackColor = Color.FromName("WindowFrame");

                fullSkinName.BackColor = Color.FromArgb(32, 32, 32);
                fullSkinName.ForeColor = Color.FromArgb(150, 150, 150);
                searchFloatInput.BackColor = Color.FromArgb(32, 32, 32);
                searchFloatInput.ForeColor = Color.FromArgb(150, 150, 150);

                quantityInput.BackColor = Color.FromArgb(32, 32, 32);
                quantityInput.ForeColor = Color.FromArgb(150, 150, 150);
                skipValueInput.BackColor = Color.FromArgb(32, 32, 32);
                skipValueInput.ForeColor = Color.FromArgb(150, 150, 150);

                multithreadCheckBox.ForeColor = Color.FromName("White");
                label10.ForeColor = Color.FromName("White");
                threadCountInput.BackColor = Color.FromArgb(32, 32, 32);
                threadCountInput.ForeColor = Color.FromArgb(150, 150, 150);

                searchModeLabel.ForeColor = Color.White;
                speedStatusLabel.ForeColor = Color.White;
                combinationsStatusLabel.ForeColor = Color.White;

                searchmodeLess_btn.BackColor = Color.FromArgb(56, 56, 56);
                searchmodeLess_btn.ForeColor = Color.FromArgb(255, 255, 255);
                searchmodeEqual_btn.BackColor = Color.FromArgb(56, 56, 56);
                searchmodeEqual_btn.ForeColor = Color.FromArgb(255, 255, 255);
                searchmodeGreater_btn.BackColor = Color.FromArgb(56, 56, 56);
                searchmodeGreater_btn.ForeColor = Color.FromArgb(255, 255, 255);

                gpuSearch_btn.BackColor = Color.FromArgb(56, 56, 56);
                gpuSearch_btn.ForeColor = Color.FromArgb(255, 255, 255);
            }
            else
            {
                outputConsoleBox.BackColor = Color.FromArgb(255, 255, 255);
                outputConsoleBox.ForeColor = Color.FromArgb(0, 0, 0);

                panel10.BackColor = Color.FromArgb(255, 255, 255);
                panel12.BackColor = Color.FromArgb(255, 255, 255);

                panel3.BackColor = Color.FromArgb(249, 249, 249);
                panel9.BackColor = Color.FromArgb(249, 249, 249);

                label1.ForeColor = Color.FromArgb(0, 0, 0);
                label2.ForeColor = Color.FromArgb(0, 0, 0);
                label3.ForeColor = Color.FromArgb(0, 0, 0);
                label4.ForeColor = Color.FromArgb(0, 0, 0);
                label5.ForeColor = Color.FromArgb(0, 0, 0);
                label6.ForeColor = Color.FromArgb(0, 0, 0);
                label7.ForeColor = Color.FromArgb(0, 0, 0);
                label8.ForeColor = Color.FromArgb(0, 0, 0);

                panel5.BackColor = Color.FromArgb(222, 222, 222);
                panel6.BackColor = Color.FromArgb(222, 222, 222);

                weaponTypeBox.BackColor = Color.FromArgb(255, 255, 255);
                weaponTypeBox.ForeColor = Color.FromArgb(10, 10, 10);
                weaponSkinBox.BackColor = Color.FromArgb(255, 255, 255);
                weaponSkinBox.ForeColor = Color.FromArgb(10, 10, 10);
                weaponQualityBox.BackColor = Color.FromArgb(255, 255, 255);
                weaponQualityBox.ForeColor = Color.FromArgb(10, 10, 10);

                minimizeBtn.ForeColor = Color.FromArgb(0, 0, 0);
                updateMuteIcon();
                closeBtn.ForeColor = Color.FromArgb(0, 0, 0);
                helpBtn.ForeColor = Color.FromArgb(0, 0, 0);
                darkModeSwitchBtn.ForeColor = Color.FromArgb(0, 0, 0);

                minimizeBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                soundBtnSwitch.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                closeBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                helpBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                darkModeSwitchBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);

                stattrackCheckBox.ForeColor = Color.FromArgb(0, 0, 0);
                sortCheckBox.ForeColor = Color.FromArgb(0, 0, 0);
                ascendingCheckBox.ForeColor = Color.FromArgb(0, 0, 0);

                checkPossibilityBtn.BackColor = Color.FromArgb(249, 249, 249);
                startBtn.BackColor = Color.FromArgb(249, 249, 249);
                checkPossibilityBtn.ForeColor = Color.FromArgb(0, 0, 0);
                startBtn.ForeColor = Color.FromArgb(0, 0, 0);

                fullSkinName.BackColor = Color.FromArgb(255, 255, 255);
                fullSkinName.ForeColor = Color.FromArgb(10, 10, 10);
                searchFloatInput.BackColor = Color.FromArgb(255, 255, 255);
                searchFloatInput.ForeColor = Color.FromArgb(10, 10, 10);

                checkPossibilityBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
                startBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);

                quantityInput.BackColor = Color.FromArgb(255, 255, 255);
                quantityInput.ForeColor = Color.FromArgb(10, 10, 10);
                skipValueInput.BackColor = Color.FromArgb(255, 255, 255);
                skipValueInput.ForeColor = Color.FromArgb(10, 10, 10);

                multithreadCheckBox.ForeColor = Color.FromName("Black");
                label10.ForeColor = Color.FromName("Black");
                threadCountInput.BackColor = Color.FromArgb(255, 255, 255);
                threadCountInput.ForeColor = Color.FromArgb(10, 10, 10);

                searchModeLabel.ForeColor = Color.Black;
                speedStatusLabel.ForeColor = Color.Black;
                combinationsStatusLabel.ForeColor = Color.Black;

                searchmodeLess_btn.BackColor = Color.FromArgb(249, 249, 249);
                searchmodeLess_btn.ForeColor = Color.FromArgb(0, 0, 0);
                searchmodeEqual_btn.BackColor = Color.FromArgb(249, 249, 249);
                searchmodeEqual_btn.ForeColor = Color.FromArgb(0, 0, 0);
                searchmodeGreater_btn.BackColor = Color.FromArgb(249, 249, 249);
                searchmodeGreater_btn.ForeColor = Color.FromArgb(0, 0, 0);

                gpuSearch_btn.BackColor = Color.FromArgb(249, 249, 249);
                gpuSearch_btn.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void DarkModeSwitchButton_Click(object sender, EventArgs e)
        {
            darkTheme = !darkTheme;
            ChangeTheme(darkTheme);
            darkModeSwitchBtn.Text = darkTheme ? "🌙" : "☀";
        }

        private void MultithreadSwitched(object sender, EventArgs e)
        {
            threadCountInput.Enabled = multithreadCheckBox.Checked;
        }

        private void gpuSearch_btn_Click(object sender, EventArgs e)
        {
            double[] floats = {
                0.246938750147820, 0.196652039885521,
                0.154839321970940, 0.333326697349548, 
                0.163415759801865, 0.291821509599686, 
                0.374309629201889, 0.378754675388336, 
                0.231419935822487, 0.311867892742157, 
                0.374067693948746, 0.377068012952805, 
                0.244467452168465, 0.355135351419449, 
                0.352264583110809, 0.227853879332542, 
                0.340960860252380, 0.375657349824905, 
                0.157685652375221, 0.217334255576134, 
                0.323678821325302, 0.363768666982651, 
                0.350994020700455, 0.369551151990891, 
                0.350340574979782, 0.338801741600037, 
                0.329752802848816, 0.369740217924118, 
                0.370476812124252, 0.205233186483383, 
                0.360520750284195, 0.373722523450851, 
                0.161364838480949, 0.263432979583740, 
                0.314681977033615, 0.310743361711502, 
                0.349280923604965, 0.355936050415039, 
                0.269742101430893, 0.199420168995857, 
                0.364472836256027, 0.218964993953705, 
                0.239638179540634, 0.325499594211578, 
                0.228406846523285, 0.307701110839844, 
                0.156294032931328, 0.179465100169182, 
                0.327553898096085, 0.150170117616653
            };

            client.SetPresence(new RichPresence()
            {
                Details = "GPU Тест",
                State = "Тестирование подбора на видеокарте",
                Timestamps = Timestamps.Now,
                Assets = new Assets()
                {
                    LargeImageKey = "icon",
                    LargeImageText = "FloatTool"
                }
            });
            SwitchEnabled();
        }

        BigInteger last = 0;
        private void timer2_Tick(object sender, EventArgs e)
        {
            var hundrMilsCount = currComb - last;
            //Console.WriteLine($"{hundrMilsCount} in last {timer2.Interval} ms");
            var speed = (double)(hundrMilsCount) * 1000 / WorkStatusUpdater.Interval;
            speedStatusLabel.Text = $"Текущая скорость: {speed} комбинаций/сек";
            last = currComb;
            combinationsStatusLabel.Text = $"Проверено комбинаций: {currComb} / {totalComb}";

            if (ConsoleBuffer.Length > 0)
            {
                outputConsoleBox.Text += ConsoleBuffer.ToString();
                ConsoleBuffer.Clear();
                outputConsoleBox.ScrollToCaret();
            }

            if (totalComb != 0 && currComb < totalComb)
                workProgressBar.Value = (int)( (double)(currComb) / (double)(totalComb) * 256);
        }

        private void changeSearchMode(object sender, EventArgs e)
        {
            var selectedMode = ((System.Windows.Forms.Button)sender).Text;

            if(selectedMode == "=")
            {
                searchmodeLess_btn.FlatAppearance.BorderSize = 0;
                searchmodeEqual_btn.FlatAppearance.BorderSize = 1;
                searchmodeGreater_btn.FlatAppearance.BorderSize = 0;
                CurrentSearchMode = SearchMode.Equal;
            }
            else if(selectedMode == ">")
            {
                searchmodeLess_btn.FlatAppearance.BorderSize = 0;
                searchmodeEqual_btn.FlatAppearance.BorderSize = 0;
                searchmodeGreater_btn.FlatAppearance.BorderSize = 1;
                CurrentSearchMode = SearchMode.Greater;
            }
            else if(selectedMode == "<")
            {
                searchmodeLess_btn.FlatAppearance.BorderSize = 1;
                searchmodeEqual_btn.FlatAppearance.BorderSize = 0;
                searchmodeGreater_btn.FlatAppearance.BorderSize = 0;
                CurrentSearchMode = SearchMode.Less;
            }
        }
    }
}
