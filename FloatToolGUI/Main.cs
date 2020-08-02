using DiscordRPC;
using Newtonsoft.Json;
using System;
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
            return setprecission(((maxFloat - minFloat) * avgFloat) + minFloat, 9);
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

                string flot = craftF(inputStr, minWear, maxWear);
                Console.WriteLine(flotOrigin + " | " + flot);
                //Debug.WriteLine("[DEBUG] flot = " + flot);
                // if (wasSort && ((!asc && (double.Parse(flot) > double.Parse(want))) || (asc && (double.Parse(flot) < double.Parse(want))))) {
                //     okSort = true;
                //}
                if (flot.StartsWith(want.Replace(".", ",")) || ("" + flotOrigin).StartsWith(want.Replace(".", ",")))
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        textBox2.AppendText("Коомбинация найдена!" + Environment.NewLine);
                        textBox2.AppendText("Возможный флоат: " + flotOrigin + Environment.NewLine);
                        textBox2.AppendText("Проверочный флоат: " + flot + Environment.NewLine);
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
            int n = floats.Count;
            if (10 > n) { Console.WriteLine("At least 10!"); return; }
            int[] indices = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            List<double> first = new List<double>();
            foreach (int i in indices) { first.Add(pool[i]); }
            //Craft
            parseCraft(first.ToArray(), craftList, wanted, checkBox2.Checked, checkBox3.Checked);
            //
            int iter = 2;
            while (true)
            {
                //var startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                //Debug.WriteLine("[DEBUG] start time = "+startTime);
                int a = 9; bool run = true;
                for (int i = 9; i >= 0; i--) { a = i; if (indices[i] != i + n - 10) { run = false; break; } }
                if (run) { break; }
                indices[a]++;
                for (int j = a + 1; j < 10; j++) { indices[j] = indices[j - 1] + 1; }
                List<double> current = new List<double>();
                foreach (int i in indices) { current.Add(pool[i]); }
                //Craft
                parseCraft(current.ToArray(), craftList, wanted, checkBox2.Checked, checkBox3.Checked);
                //var endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                //Debug.WriteLine("[DEBUG] end time = " + endTime);
                //Debug.WriteLine("[DEBUG] total time = " + (endTime - startTime));
                //
                iter++;
            }
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

        private void button7_Click(object sender, EventArgs e)
        {
            if (muteSound)
            {
                muteSound = false;
                button7.Image = FloatToolGUI.Properties.Resources.unmuted;
                SoundPlayer player = new SoundPlayer();
                player.Play();
            }
            else
            {
                muteSound = true;
                button7.Image = FloatToolGUI.Properties.Resources.muted;
            }
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
    }
}
