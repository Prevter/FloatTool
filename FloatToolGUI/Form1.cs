using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloatToolGUI
{
    public partial class FloatTool : Form
    {
        Thread thread1;
        public static string ToExactString(double d)
        {
            if (double.IsPositiveInfinity(d))
                return "+Infinity";
            if (double.IsNegativeInfinity(d))
                return "-Infinity";
            if (double.IsNaN(d))
                return "NaN";
            long bits = BitConverter.DoubleToInt64Bits(d);
            bool negative = (bits < 0);
            int exponent = (int)((bits >> 52) & 0x7ffL);
            long mantissa = bits & 0xfffffffffffffL;
            if (exponent == 0)
            {
                exponent++;
            }
            else
            {
                mantissa = mantissa | (1L << 52);
            }
            exponent -= 1075;

            if (mantissa == 0)
            {
                return "0";
            }
            while ((mantissa & 1) == 0)
            {
                mantissa >>= 1;
                exponent++;
            }
            ArbitraryDecimal ad = new ArbitraryDecimal(mantissa);
            if (exponent < 0)
            {
                for (int i = 0; i < -exponent; i++)
                    ad.MultiplyBy(5);
                ad.Shift(-exponent);
            }
            else
            {
                for (int i = 0; i < exponent; i++)
                    ad.MultiplyBy(2);
            }
            if (negative)
                return "-" + ad.ToString();
            else
                return ad.ToString();
        }
        class ArbitraryDecimal
        {
            byte[] digits;
            int decimalPoint = 0;

            internal ArbitraryDecimal(long x)
            {
                string tmp = x.ToString(CultureInfo.InvariantCulture);
                digits = new byte[tmp.Length];
                for (int i = 0; i < tmp.Length; i++)
                    digits[i] = (byte)(tmp[i] - '0');
                Normalize();
            }
            internal void MultiplyBy(int amount)
            {
                byte[] result = new byte[digits.Length + 1];
                for (int i = digits.Length - 1; i >= 0; i--)
                {
                    int resultDigit = digits[i] * amount + result[i + 1];
                    result[i] = (byte)(resultDigit / 10);
                    result[i + 1] = (byte)(resultDigit % 10);
                }
                if (result[0] != 0)
                {
                    digits = result;
                }
                else
                {
                    Array.Copy(result, 1, digits, 0, digits.Length);
                }
                Normalize();
            }
            internal void Shift(int amount)
            {
                decimalPoint += amount;
            }
            internal void Normalize()
            {
                int first;
                for (first = 0; first < digits.Length; first++)
                    if (digits[first] != 0)
                        break;
                int last;
                for (last = digits.Length - 1; last >= 0; last--)
                    if (digits[last] != 0)
                        break;

                if (first == 0 && last == digits.Length - 1)
                    return;

                byte[] tmp = new byte[last - first + 1];
                for (int i = 0; i < tmp.Length; i++)
                    tmp[i] = digits[i + first];

                decimalPoint -= digits.Length - (last + 1);
                digits = tmp;
            }
            public override String ToString()
            {
                char[] digitString = new char[digits.Length];
                for (int i = 0; i < digits.Length; i++)
                    digitString[i] = (char)(digits[i] + '0');
                if (decimalPoint == 0)
                {
                    return new string(digitString);
                }
                if (decimalPoint < 0)
                {
                    return new string(digitString) +
                           new string('0', -decimalPoint);
                }
                if (decimalPoint >= digitString.Length)
                {
                    return "0." +
                        new string('0', (decimalPoint - digitString.Length)) +
                        new string(digitString);
                }
                return new string(digitString, 0,
                                   digitString.Length - decimalPoint) +
                    "." +
                    new string(digitString,
                                digitString.Length - decimalPoint,
                                decimalPoint);
            }
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
                decimal flotOrigin = Math.Round(craft(inputs.ToArray(), minWear, maxWear), 9);
                //string flot = ToExactString((double)flotOrigin);
                Console.WriteLine(flotOrigin);
                //Debug.WriteLine("[DEBUG] flot = " + flot);
                // if (wasSort && ((!asc && (double.Parse(flot) > double.Parse(want))) || (asc && (double.Parse(flot) < double.Parse(want))))) {
                //     okSort = true;
                //}
                if (/*flot.StartsWith(want) ||*/ ("" + flotOrigin).StartsWith(want.Replace(".", ",")))
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        using (WebClient wcf = new WebClient())
                        {
                            try
                            {
                                //string jsonf = wcf.DownloadString(url);
                                //dynamic rf = JsonConvert.DeserializeObject(jsonf);
                                //Debug.WriteLine("[DEBUG] " + counter + "/" + count + " load from csgofloat = " + jsonf);
                                //floats.Add(Convert.ToDouble(rf["iteminfo"]["floatvalue"]));
                            }
                            catch
                            {
                                Console.Write("");
                            }
                        }
                        textBox2.AppendText("Коомбинация найдена!" + Environment.NewLine);
                        textBox2.AppendText("Возможный флоат: " + flotOrigin + Environment.NewLine);
                        textBox2.AppendText("Список флоатов: [");
                        for (int i = 0; i < 10; i++)
                        {
                            textBox2.AppendText(inputs[i].ToString().Replace(",","."));
                            if (i != 9)
                            {
                                textBox2.AppendText(", ");
                            }
                            else
                            {
                                textBox2.AppendText("]" + Environment.NewLine + "======================================" + Environment.NewLine);
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
        }

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
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

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
                }
            ));
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(button2.Text == "Старт") {
                thread1 = new Thread(StartCalculation);
                thread1.Start();
            }
            else
            {
                thread1.Abort();
                button2.Text = "Старт";
            }
            SwitchEnabled();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox2.ScrollToCaret();
        }
    }
}
