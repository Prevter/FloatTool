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
            List<double> results = new List<double>();
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

                //string flot = craftF(inputStr, minWear, maxWear);
                //Console.WriteLine(flotOrigin + " | " + flot);
                //Debug.WriteLine("[DEBUG] flot = " + flot);
                // if (wasSort && ((!asc && (double.Parse(flot) > double.Parse(want))) || (asc && (double.Parse(flot) < double.Parse(want))))) {
                //     okSort = true;
                //}
                if (/*flot.StartsWith(want.Replace(".", ",")) ||*/ ("" + flotOrigin).StartsWith(want.Replace(".", ",")))
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        textBox2.AppendText("Коомбинация найдена!" + Environment.NewLine);
                        textBox2.AppendText("Возможный флоат: " + flotOrigin + Environment.NewLine);
                        //textBox2.AppendText("Проверочный флоат: " + flot + Environment.NewLine);
                        textBox2.AppendText("Список флоатов: [");
                        if (!muteSound)
                        {
                            //play sound
                            SoundPlayer player = new SoundPlayer();
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
                            textBox2.AppendText(Math.Round(inputs[i], 14).ToString().Replace(",","."));
                            if (i != 9)
                            {
                                textBox2.AppendText(", ");
                            }
                            else
                            {
                                textBox2.AppendText("]" + Environment.NewLine + "=====================================" + Environment.NewLine);
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
            comboBox1.Enabled = !comboBox1.Enabled;
            comboBox2.Enabled = !comboBox2.Enabled;
            textBox1.Enabled = !textBox1.Enabled;
            comboBox3.Enabled = !comboBox3.Enabled;
            textBox3.Enabled = !textBox3.Enabled;
            numericUpDown1.Enabled = !numericUpDown1.Enabled;
            numericUpDown2.Enabled = !numericUpDown2.Enabled;
            checkBox1.Enabled = !checkBox1.Enabled;
            checkBox2.Enabled = !checkBox2.Enabled;
            checkBox3.Enabled = !checkBox3.Enabled;
        }
        public void updateSearchStr()
        {
            string search = "";
            if (checkBox1.Checked)
            {
                search += "StatTrak™ ";
            }
            search += comboBox1.Text;
            search += " | ";
            search += comboBox2.Text;
            search += " (" + comboBox3.Text + ")";
            textBox1.Text = search;
        }
        public FloatTool()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }
        public DiscordRpcClient client;
        private bool darkTheme = true;

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            using (StreamReader r = new StreamReader("itemData.json"))
            {
                string json = r.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);
                foreach (var skin in items)
                {

                    if (skin["name"].ToString().Split('|')[0].TrimEnd() == comboBox1.Text)
                    {
                        Console.WriteLine(skin["name"].ToString().Split('|')[1].Remove(0, 1));
                        comboBox2.Items.Add(skin["name"].ToString().Split('|')[1].Remove(0, 1));
                    }
                }
            }
            updateSearchStr();
            button10.Text = darkTheme ? "🌙" : "☀";
            client = new DiscordRpcClient("734042978246721537");

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

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void runCycle()
        {
            Console.WriteLine("Thread loaded!");
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            using (StreamReader r = new StreamReader("itemData.json"))
            {
                string json = r.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);
                foreach (var skin in items)
                {
                    
                    if (skin["name"].ToString().Split('|')[0].TrimEnd() == comboBox1.Text)
                    {
                        Console.WriteLine(skin["name"].ToString().Split('|')[1].Remove(0,1));
                        comboBox2.Items.Add(skin["name"].ToString().Split('|')[1].Remove(0,1));
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
                parseCraft(pair.ToArray(), craftList, wanted, checkBox2.Checked, checkBox3.Checked);
            }
        }
        public List<Thread> t2 = new List<Thread>();
        private void StartCalculation()
        {
            client.SetPresence(new RichPresence()
            {
                Details = "Начал поиск",
                State = "Ищу " + textBox3.Text,
                Timestamps = Timestamps.Now,
                Assets = new Assets()
                {
                    LargeImageKey = "icon",
                    LargeImageText = "FloatTool"
                }
            });
            this.Invoke((MethodInvoker)(() =>
                {
                    textBox2.Text = "Добро пожаловать в FloatTool!" + Environment.NewLine + "Инструмент для создания флоатов при помощи крафтов CS:GO" + Environment.NewLine;
                    textBox2.AppendText( "Время начала процесса: " + DateTime.Now.ToString("HH:mm:ss tt") + Environment.NewLine);
                    button2.Text = "Стоп";
                    textBox1.SelectionStart = textBox1.Text.Length;
                    textBox2.ScrollToCaret();
                }
            ));
            
            string count = "" + numericUpDown1.Value;
            string start = "" + numericUpDown2.Value;
            string wanted = textBox3.Text;
            string q = textBox1.Text;
            string url = "https://steamcommunity.com/market/listings/730/" + q + "/render/?query=&language=russian&count=" + count + "&start=" + start + "&currency=5";
            Console.WriteLine(url);
            this.Invoke((MethodInvoker)(() =>
            {
                textBox2.AppendText( "Загрузка скинов с торговой площадки..." + Environment.NewLine);
                progressBar1.Maximum = int.Parse(count);
                progressBar1.Value = 0;
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox2.ScrollToCaret();
                
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
                        textBox2.AppendText( "Получение флоатов..." + Environment.NewLine);
                        textBox1.SelectionStart = textBox1.Text.Length;
                        textBox2.ScrollToCaret();
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
                        progressBar1.Value = counter;
                    }
                    ));
                    
                }
            }
            if (checkBox2.Checked)
            {
                if (checkBox3.Checked)
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
                textBox2.AppendText( "Поиск ауткамов..." + Environment.NewLine);
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox2.ScrollToCaret();
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
                textBox2.AppendText( "Ауткамы найдены! Начинаю подбор..." + Environment.NewLine + Environment.NewLine);
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox2.ScrollToCaret();
            }
            ));
            //return;
            double[] pool = floats.ToArray();
            //int n = floats.Count;
            //if (10 > n) { Console.WriteLine("At least 10!"); return; }
            //int[] indices = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            //List<double> first = new List<double>();
            //foreach (int i in indices) { first.Add(pool[i]); }
            //Craft
            //parseCraft(first.ToArray(), craftList, wanted, checkBox2.Checked, checkBox3.Checked);
            //
            //int iter = 2;

            var threads = 1;
            if (checkBox4.Checked)
            {
                threads = (int)numericUpDown3.Value;
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
                }
            }

            
            
            foreach (IEnumerable<double> pair in Combinations(pool, 10, 0, threads))
            {
                parseCraft(pair.ToArray(), craftList, wanted, checkBox2.Checked, checkBox3.Checked);
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
            
            
            


            
            /*
            Parallel.For(2, 6, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (iter) => {
                while (true)
                {
                    //var startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    //Debug.WriteLine("[DEBUG] start time = "+startTime);
                    Console.WriteLine("iter = " + iter);
                    int a = 9; bool run = true;
                    for (int i = 9; i >= 0; i--) { a = i; if (indices[i] != i + n - 10) { run = false; break; } }
                    if (run) { break; }
                    indices[a]++;
                    for (int j = a + 1; j < 10; j++) { indices[j] = indices[j - 1] + 1; }
                    List<double> current = new List<double>();
                    foreach (int i in indices) { current.Add(pool[i]); }
                    //Craft
                    
                    //var endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    //Debug.WriteLine("[DEBUG] end time = " + endTime);
                    //Debug.WriteLine("[DEBUG] total time = " + (endTime - startTime));
                    //
                    iter++;
                }
            });
            */

            this.Invoke((MethodInvoker)(() =>
                {
                    textBox2.AppendText( "Программа завершила проверку всех комбинаций!" + Environment.NewLine);
                    textBox1.SelectionStart = textBox1.Text.Length;
                    textBox2.ScrollToCaret();
                    thread1.Abort();
                    button2.Text = "Старт";
                    progressBar1.Value = 0;
                    SwitchEnabled();
                }
            ));
            
        }

        private void parseCraft(List<double>[] lists, List<dynamic> craftList, string wanted, bool checked1, bool checked2)
        {
            throw new NotImplementedException();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(button2.Text == "Старт") {
                thread1.Abort();
                thread1 = new Thread(StartCalculation);
                thread1.Start();
            }
            else
            {
                thread1.Abort();
                button2.Text = "Старт";
                progressBar1.Value = 0;
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
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox2.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateSearchStr();
            string skin = "";
            skin += comboBox1.Text;
            skin += " | ";
            skin += comboBox2.Text;
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
                            if (floatRangeText(comboBox3.Text, skn["minWear"].ToString(), skn["maxWear"].ToString()))
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
                    button7.Image = FloatToolGUI.Properties.Resources.unmutedWhite;
                }
                else
                {
                    button7.Image = FloatToolGUI.Properties.Resources.unmutedBlack;
                }
            }
            else
            {
                if (darkTheme)
                {
                    button7.Image = FloatToolGUI.Properties.Resources.mutedWhite;
                }
                else
                {
                    button7.Image = FloatToolGUI.Properties.Resources.mutedBlack;
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (muteSound)
            {
                muteSound = false;
                SoundPlayer player = new SoundPlayer();
                player.Play();
            }
            else
            {
                muteSound = true;
            }
            updateMuteIcon();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Nemeshio/FloatTool-GUI/wiki/%D0%93%D0%BB%D0%B0%D0%B2%D0%BD%D0%B0%D1%8F");
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Nemeshio/FloatTool-GUI/");
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            About aboutForm = new About();
            aboutForm.Show();
        }

        private void panel14_Paint(object sender, PaintEventArgs e)
        {

        }

        void changeTheme(bool dark)
        {
            if (dark)
            {
                textBox2.BackColor = Color.FromArgb(31, 31, 31);
                textBox2.ForeColor = Color.FromArgb(255, 255, 255);

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

                comboBox1.BackColor = Color.FromArgb(32, 32, 32);
                comboBox1.ForeColor = Color.FromArgb(150, 150, 150);
                comboBox2.BackColor = Color.FromArgb(32, 32, 32);
                comboBox2.ForeColor = Color.FromArgb(150, 150, 150);
                comboBox3.BackColor = Color.FromArgb(32, 32, 32);
                comboBox3.ForeColor = Color.FromArgb(150, 150, 150);

                button6.ForeColor = Color.FromArgb(255, 255, 255);
                updateMuteIcon();
                button8.ForeColor = Color.FromArgb(255, 255, 255);
                button9.ForeColor = Color.FromArgb(255, 255, 255);
                button10.ForeColor = Color.FromArgb(255, 255, 255);

                button6.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);
                button7.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);
                button8.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);
                button9.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);
                button10.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);

                checkBox1.ForeColor = Color.FromArgb(255, 255, 255);
                checkBox2.ForeColor = Color.FromArgb(255, 255, 255);
                checkBox3.ForeColor = Color.FromArgb(255, 255, 255);

                button1.BackColor = Color.FromArgb(56, 56, 56);
                button2.BackColor = Color.FromArgb(56, 56, 56);
                button1.ForeColor = Color.FromArgb(255, 255, 255);
                button2.ForeColor = Color.FromArgb(255, 255, 255);

                button1.FlatAppearance.MouseOverBackColor = Color.FromName("WindowFrame");
                button2.FlatAppearance.MouseOverBackColor = Color.FromName("WindowFrame");

                textBox1.BackColor = Color.FromArgb(32, 32, 32);
                textBox1.ForeColor = Color.FromArgb(150, 150, 150);
                textBox3.BackColor = Color.FromArgb(32, 32, 32);
                textBox3.ForeColor = Color.FromArgb(150, 150, 150);

                numericUpDown1.BackColor = Color.FromArgb(32, 32, 32);
                numericUpDown1.ForeColor = Color.FromArgb(150, 150, 150);
                numericUpDown2.BackColor = Color.FromArgb(32, 32, 32);
                numericUpDown2.ForeColor = Color.FromArgb(150, 150, 150);

                checkBox4.ForeColor = Color.FromName("White");
                label10.ForeColor = Color.FromName("White");
                numericUpDown3.BackColor = Color.FromArgb(32, 32, 32);
                numericUpDown3.ForeColor = Color.FromArgb(150, 150, 150);
            }
            else
            {
                textBox2.BackColor = Color.FromArgb(255, 255, 255);
                textBox2.ForeColor = Color.FromArgb(0, 0, 0);

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

                comboBox1.BackColor = Color.FromArgb(255, 255, 255);
                comboBox1.ForeColor = Color.FromArgb(10, 10, 10);
                comboBox2.BackColor = Color.FromArgb(255, 255, 255);
                comboBox2.ForeColor = Color.FromArgb(10, 10, 10);
                comboBox3.BackColor = Color.FromArgb(255, 255, 255);
                comboBox3.ForeColor = Color.FromArgb(10, 10, 10);

                button6.ForeColor = Color.FromArgb(0, 0, 0);
                updateMuteIcon();
                button8.ForeColor = Color.FromArgb(0, 0, 0);
                button9.ForeColor = Color.FromArgb(0, 0, 0);
                button10.ForeColor = Color.FromArgb(0, 0, 0);

                button6.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                button7.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                button8.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                button9.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                button10.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);

                checkBox1.ForeColor = Color.FromArgb(0, 0, 0);
                checkBox2.ForeColor = Color.FromArgb(0, 0, 0);
                checkBox3.ForeColor = Color.FromArgb(0, 0, 0);

                button1.BackColor = Color.FromArgb(249, 249, 249);
                button2.BackColor = Color.FromArgb(249, 249, 249);
                button1.ForeColor = Color.FromArgb(0, 0, 0);
                button2.ForeColor = Color.FromArgb(0, 0, 0);

                textBox1.BackColor = Color.FromArgb(255, 255, 255);
                textBox1.ForeColor = Color.FromArgb(10, 10, 10);
                textBox3.BackColor = Color.FromArgb(255, 255, 255);
                textBox3.ForeColor = Color.FromArgb(10, 10, 10);

                button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
                button2.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);

                numericUpDown1.BackColor = Color.FromArgb(255, 255, 255);
                numericUpDown1.ForeColor = Color.FromArgb(10, 10, 10);
                numericUpDown2.BackColor = Color.FromArgb(255, 255, 255);
                numericUpDown2.ForeColor = Color.FromArgb(10, 10, 10);

                checkBox4.ForeColor = Color.FromName("Black");
                label10.ForeColor = Color.FromName("Black");
                numericUpDown3.BackColor = Color.FromArgb(255, 255, 255);
                numericUpDown3.ForeColor = Color.FromArgb(10, 10, 10);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            darkTheme = !darkTheme;
            changeTheme(darkTheme);
            button10.Text = darkTheme ? "🌙" : "☀";
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown3.Enabled = checkBox4.Checked;
        }
    }
}
