using DiscordRPC;
using FloatToolGUI.Resources;
using static FloatToolGUI.Utils;
using static FloatToolGUI.Calculation;
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
using Microsoft.Win32;
using System.Numerics;
using Button = System.Windows.Forms.Button;

namespace FloatToolGUI
{
    public enum Quality
    {
        Consumer,
        Industrial,
        MilSpec,
        Restricted,
        Classified,
        Covert
    }

    public enum Currency
    {
        USD = 1, GBP = 2,
        EUR = 3, CHF = 4,
        RUB = 5, PLN = 6,
        BRL = 7, JPY = 8,
        NOK = 9, IDR = 10,
        MYR = 11, PHP = 12,
        SGD = 13, THB = 14,
        VND = 15, KRW = 16,
        TRY = 17, UAH = 18,
        MXN = 19, CAD = 20,
        AUD = 21, NZD = 22,
        CNY = 23, INR = 24,
        CLP = 25, PEN = 26,
        COP = 27, ZAR = 28,
        HKD = 29, TWD = 30,
        SAR = 31, AED = 32,
        ARS = 34, ILS = 35,
        BYN = 36, KZT = 37,
        KWD = 38, QAR = 39,
        CRC = 40, UYU = 41,
        RMB = 9000, NXP = 9001
    }

    public partial class FloatTool : Form
    {
        Thread thread1;
        public bool muteSound = false;
        string newLine;
        public StringBuilder ConsoleBuffer;
        public enum SearchMode
        {
            Less,
            Equal,
            Greater
        }

        public SearchMode CurrentSearchMode = SearchMode.Equal;
        public Currency currentCurr = Currency.USD;
        public bool discordWorker = true;

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
                    this.Invoke((MethodInvoker)(() =>
                    {
                        float price = 0f;
                        List<string> floatStrings = new List<string>();
                        foreach (var fl in inputs)
                        {
                            floatStrings.Add(Math.Round(fl.WearValue, 14).ToString().Replace(",", "."));
                            price += fl.Price;
                        }

                        Logger.Log($"[{DateTime.Now.ToString()}]: Found coombination {{");
                        Logger.Log($"   Float      = {flotOrigin}{newLine}" +
                                   $"   Test Float = {flot}{newLine}" +
                                   $"   Price      = {price} {inputs[0].SkinCurrency}{newLine}" +
                                   $"   Float list = [{String.Join(", ", floatStrings)}]{newLine}}}");

                        AddCombinationToList(DateTime.Now.ToString("HH:mm:ss"), flotOrigin, flot, price, floatStrings);
                        ConsoleBuffer.Append($"[{DateTime.Now.ToString("HH:mm:ss")}] {strings.CombinationFound}{newLine}");
                        ConsoleBuffer.Append($"{strings.PossibleFloat}: {flotOrigin}{newLine}");
                        ConsoleBuffer.Append($"{strings.TestFloat}: {flot}{newLine}");
                        ConsoleBuffer.Append($"Цена: {price} {inputs[0].SkinCurrency}{newLine}");
                        ConsoleBuffer.Append($"{strings.FloatList}: ");
                        if (!muteSound)
                        {
                            //play sound
                            SoundPlayer player = new SoundPlayer(Properties.Resources.notification);
                            player.Play();
                        }
                        if (discordWorker)
                        {
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
                        }
                        
                        ConsoleBuffer.Append($"[{String.Join(", ", floatStrings)}]{newLine}====================================={newLine}");
                    }
                    ));
                    
                    //textBox2.AppendText( "IEEE754: " + flot + newLine;
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
            outcomeSelectorComboBox.Enabled = !outcomeSelectorComboBox.Enabled;
            threadCountInput.Enabled = !threadCountInput.Enabled;
        }

        public void UpdateOutcomes()
        {
            string skin = $"{weaponTypeBox.Text} | {weaponSkinBox.Text}";
            outcomeSelectorComboBox.Items.Clear();
            List<dynamic> craftList = new List<dynamic>();
            using (StreamReader r = new StreamReader("itemData.json"))
            {
                string json = r.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);
                foreach (var skn in items)
                {
                    if (skn["name"].ToString() == skin)
                    {
                        foreach (var skin2 in items)
                            if (skn["case"].ToString() == skin2["case"].ToString())
                            {
                                if (skin2["rarity"].ToString().Split(' ')[0] == getNextRarity(skn["rarity"].ToString().Split(' ')[0]))
                                    craftList.Add(skin2);
                            }
                    }
                }
                int totalSkins = 0;
                foreach (var skinRange in GroupOutcomes(craftList))
                    totalSkins += skinRange.Count;
                foreach (var skinRange in GroupOutcomes(craftList))
                {
                    string tmp = (skinRange.Count > 1) ? $" + {(skinRange.Count - 1)}" : "";
                    outcomeSelectorComboBox.Items.Add($"{((float)skinRange.Count) / totalSkins * 100}% ({skinRange[0].Name}{tmp})");
                }
                outcomeSelectorComboBox.Items.Add("* Искать всё *");
                outcomeSelectorComboBox.SelectedIndex = 0;
            }
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
            UpdateOutcomes();
            Logger.Log($"[{DateTime.Now.ToString()}]: Changed search skin to: {search}");
        }

        RegistryKey registryData;
        public FloatTool()
        {
            InitializeComponent();
            Logger.ClearLogs();
            Logger.Log($"[{DateTime.Now}]: FloatTool-GUI {versionLabel.Text}");
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            ConsoleBuffer = new StringBuilder();
            newLine = Environment.NewLine;
            CheckRegistry();

            registryData = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\FloatTool");

            var darkThemeRK = registryData.GetValue("darkMode");
            var soundRK = registryData.GetValue("sound");
            var bufferSpeedRK = registryData.GetValue("bufferSpeed");
            var discordRPCRK = registryData.GetValue("discordRPC");
            var currencyRK = registryData.GetValue("currency");
            var updateCheckRK = registryData.GetValue("updateCheck");

            ChangeTheme(Convert.ToBoolean(darkThemeRK));
            muteSound = !Convert.ToBoolean(soundRK);
            WorkStatusUpdater.Interval = (int)bufferSpeedRK;
            discordWorker = Convert.ToBoolean(discordRPCRK);
            currentCurr = (Currency)currencyRK;

            Logger.Log($"[{DateTime.Now}]: Loaded registry settings:");
            Logger.Log($"Dark mode     = {darkThemeRK}");
            Logger.Log($"Sound         = {soundRK}");
            Logger.Log($"Buffer speed  = {bufferSpeedRK}");
            Logger.Log($"Discord RPC   = {discordRPCRK}");
            Logger.Log($"Currency      = {currencyRK}");
            Logger.Log($"Check updates = {updateCheckRK}");

            if (Convert.ToBoolean(updateCheckRK))
            {
                string ver = versionLabel.Text;
                try
                {
                    ver = CheckUpdates();
                }
                catch(Exception ex)
                {
                    Logger.Log($"[{DateTime.Now}]: {{EXCEPTION}} {ex.Message}{newLine}{ex.StackTrace}");
                    Logger.SaveCrashReport();
                }

                Logger.Log($"Checked version is: {ver}{newLine}Installed: {versionLabel.Text}");
                if (ver != versionLabel.Text)
                {
                    DialogResult result = MessageBox.Show(
                    $"Доступна версия {ver}! Хотите открыть страницу загрузки?",
                    "Обновление",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);

                    if (result == DialogResult.Yes)
                        System.Diagnostics.Process.Start("https://github.com/Nemeshio/FloatTool-GUI/releases/latest");
                }
            }
            Logger.Log($"[{DateTime.Now.ToString()}]: Initialized");
        }
        public DiscordRpcClient client;

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

            //dev  = 824349399688937543
            //main = 734042978246721537
            client = new DiscordRpcClient("734042978246721537");
            if (discordWorker)
            {
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
            }
            thread1 = new Thread(runCycle);
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            this.threadCountInput.Value = Environment.ProcessorCount;
            AddFont(Properties.Resources.Inter_Regular);
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

        private void SkinComboboxChanged(object sender, EventArgs e)
        {
            updateSearchStr();
        }

        public void secndThread(List<Skin> craftList, string wanted, List<InputSkin> pool, int start, int skip)
        {
            foreach (IEnumerable<InputSkin> pair in Combinations(pool, 10, start, skip))
            {
                parseCraft(pair.ToList(), craftList, wanted);
                currComb++;
                //Console.WriteLine(currComb);
            }
        }

        public List<Panel> combinationWindows = new List<Panel>();

        public void AddCombinationToList(string time, decimal flotOrigin, string flot, float price, List<string> floatStrings)
        {
            Panel tmpPanel = new Panel
            {
                BackColor = Color.FromArgb(44, 44, 44),
                ForeColor = Color.White,
                Size = new Size(425, 220),
                Margin = new Padding(3),
                Font = new Font("Inter", 10f)
            };
            #region Labels
            tmpPanel.Controls.Add(new Label
            {
                AutoSize = true,
                Location = new Point(3,3),
                Text = $"{outcomeSelectorComboBox.Text}\nВозможный флоат: {flotOrigin.ToString(CultureInfo.InvariantCulture)}\nПроверочный флоат: {flot}"
            });
            tmpPanel.Controls.Add(new Label
            {
                AutoSize = false,
                Size = new Size(160, 54),
                Location = new Point(263, 3),
                TextAlign = ContentAlignment.MiddleRight,
                Text = $"{time}\nЦена: {price.ToString("0.00")} {currentCurr}"
            });
            tmpPanel.Controls.Add(new Label
            {
                AutoSize = true,
                Location = new Point(3, 62),
                Text = "Ингредиенты:"
            });
            #endregion
            #region TextBoxes & Buttons
            for(int i = 0; i < 10; i++)
            {
                int y = 27 * (i % 5) + 81;
                int x = i > 4 ? 220 : 6;
                tmpPanel.Controls.Add(new TextBox
                {
                    BackColor = Color.FromArgb(32, 32, 32),
                    ForeColor = Color.White,
                    ReadOnly = true,
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point(x, y),
                    Size = new Size(116, 25),
                    Text = floatStrings[i]
                });

                var tmpButton = new Button
                {
                    BackColor = Color.FromArgb(56, 56, 56),
                    Location = new Point(117 + x, y),
                    Font = new Font("Inter", 8f),
                    Size = new Size(84, 25),
                    Text = "Копировать",
                    Tag = floatStrings[i],
                    FlatStyle = FlatStyle.Flat
                };

                tmpButton.FlatAppearance.BorderSize = 0;
                tmpButton.Click += copyButtonClick;
                tmpPanel.Controls.Add(tmpButton);
            }
            #endregion

            combinationWindows.Add(tmpPanel);
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
        public bool Searching = false;

        private void StartCalculation()
        {
            Logger.Log($"[{DateTime.Now.ToString()}]: Started search");
            if (discordWorker) {
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
            }
            

            totalComb = quantityInput.Value == 10 ? 1 : Fact((int)quantityInput.Value) / (Fact(10) * Fact((int)quantityInput.Value - 10));
            currComb = 0;
            this.Invoke((MethodInvoker)(() =>
                {
                    outputConsoleBox.Text = "Добро пожаловать в FloatTool!" + newLine + "Инструмент для создания флоатов при помощи крафтов CS:GO" + newLine;
                    outputConsoleBox.AppendText( "Время начала процесса: " + DateTime.Now.ToString("HH:mm:ss tt") + newLine);
                    startBtn.Text = "Стоп";
                    fullSkinName.SelectionStart = fullSkinName.Text.Length;
                    outputConsoleBox.ScrollToCaret();
                    foundCombinationContainer.Controls.Clear();
                }
            ));
            string count = "" + quantityInput.Value;
            string start = "" + skipValueInput.Value;
            string wanted = searchFloatInput.Text;
            string q = fullSkinName.Text;

            Logger.Log($"Settings: {{{newLine}" +
                       $"   Float    = {wanted}{newLine}" +
                       $"   Count    = {count}{newLine}" +
                       $"   Skip     = {start}{newLine}" +
                       $"   Name     = {q}{newLine}" +
                       $"   Threads  = {threadCountInput.Value}{newLine}" +
                       $"}}");

            string url = $"https://steamcommunity.com/market/listings/730/{ q }/render/?query=&language=russian&count={ count }&start={ start }&currency={ (int)currentCurr }";
            Console.WriteLine(url);
           
            this.Invoke((MethodInvoker)(() =>
            {
                outputConsoleBox.AppendText( "Загрузка скинов с торговой площадки..." + newLine);
                downloadProgressBar.Maximum = int.Parse(count);
                downloadProgressBar.Value = 0;
                fullSkinName.SelectionStart = fullSkinName.Text.Length;
                outputConsoleBox.ScrollToCaret();
                combinationsStatusLabel.Text = $"Проверено комбинаций: 0 / {totalComb}";
            }
            ));
            
            List<InputSkin> inputSkins = new List<InputSkin>();
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string json = wc.DownloadString(url);
                    Console.WriteLine(json);
                    dynamic r = JsonConvert.DeserializeObject(json);
                    this.Invoke((MethodInvoker)(() =>
                    {
                        outputConsoleBox.AppendText("Получение флоатов..." + newLine);
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
                                wcf.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                                string jsonf = wcf.DownloadString(url);
                                dynamic rf = JsonConvert.DeserializeObject(jsonf);
                                //Debug.WriteLine("[DEBUG] " + counter + "/" + count + " load from csgofloat = " + jsonf);
                                inputSkins.Add(new InputSkin(Convert.ToDouble(rf["iteminfo"]["floatvalue"]),
                                    (float.Parse(r["listinginfo"][el.Name]["converted_price"].ToString()) + float.Parse(r["listinginfo"][el.Name]["converted_fee"].ToString())) / 100,
                                    currentCurr));
                            }
                            catch (Exception ex)
                            {
                                if (ex.GetType() != typeof(ThreadAbortException))
                                {
                                    Logger.Log($"[{DateTime.Now}]: {{EXCEPTION}} {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                                    Logger.SaveCrashReport();
                                }
                            }
                        }
                        this.Invoke((MethodInvoker)(() =>
                        {
                            downloadProgressBar.Value = counter;
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                if(ex.GetType() != typeof(ThreadAbortException))
                {
                    Logger.Log($"[{DateTime.Now}]: {{EXCEPTION}} {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    Logger.SaveCrashReport();
                }
            }
            if (sortCheckBox.Checked)
            {
                if (ascendingCheckBox.Checked)
                {
                    inputSkins.Sort((a, b) => a.CompareTo(b));
                    Console.WriteLine("Sorted ascending");
                }
                else
                {
                    inputSkins.Sort((a, b) => b.CompareTo(a));
                    Console.WriteLine("Sorted descending");
                }
            }
            this.Invoke((MethodInvoker)(() =>
            {
                outputConsoleBox.AppendText( "Поиск ауткамов..." + newLine);
                outputConsoleBox.SelectionStart = fullSkinName.Text.Length;
                /*string line = "[";
                foreach(var i in floats)
                    line += $"{i.ToString().Replace(',','.')}, ";
                line = line.Remove(line.Length - 2);
                textBox2.AppendText("Список флоатов:" + newLine + line + "]" + newLine);*/
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

            int outcomeIndex = 0;
            Invoke((MethodInvoker)(() => { outcomeIndex  = outcomeSelectorComboBox.SelectedIndex; } ));
            
            var allOutcomes = GroupOutcomes(craftList);
            List<Skin> outcomes = new List<Skin>();

            if(allOutcomes.Length > outcomeIndex) {
                outcomes.Add(allOutcomes[outcomeIndex][0]);
            }
            else
            {
                foreach (var i in allOutcomes)
                    outcomes.Add(i[0]);
            }

            this.Invoke((MethodInvoker)(() =>
            {
                outputConsoleBox.AppendText( "Ауткамы найдены! Начинаю подбор..." + newLine + "Выбрано для поиска:" + newLine + String.Join(newLine, outcomes) + newLine + newLine);
                fullSkinName.SelectionStart = fullSkinName.Text.Length;
                outputConsoleBox.ScrollToCaret();
                downloadProgressBar.Value = 0;
                downloadProgressBar.Maximum = 1000;
            }
            ));

            Searching = true;

            int threads = (int)threadCountInput.Value;
            try
            {
                for (int i = 0; i < threads; i++)
                {
                    Thread newThread = new Thread(() => secndThread(outcomes, wanted, inputSkins, i, threads));
                    newThread.Start();
                    t2.Add(newThread);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            Console.WriteLine($"[DEBUG] {threads} threads started!");

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

            Searching = false;

            this.Invoke((MethodInvoker)(() =>
                {
                    outputConsoleBox.AppendText( "Программа завершила проверку всех комбинаций!" + newLine);
                    fullSkinName.SelectionStart = fullSkinName.Text.Length;
                    outputConsoleBox.ScrollToCaret();
                    thread1.Abort();
                    startBtn.Text = "Старт";
                    downloadProgressBar.Value = 0;
                    SwitchEnabled();
                }
            ));
            
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            if(startBtn.Text == "Старт") {
                thread1.Abort();
                thread1 = new Thread(StartCalculation);
                thread1.Start();
            }
            else
            {
                Logger.Log($"[{DateTime.Now}] Stopping search");
                Searching = false;
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
                        #region Float ranges
                        /* 
                        List<dynamic> craftList = new List<dynamic>();

                        foreach (var skin2 in items)
                            if (skn["case"].ToString() == skin2["case"].ToString())
                            {
                                if (skin2["rarity"].ToString().Split(' ')[0] == getNextRarity(skn["rarity"].ToString().Split(' ')[0]))
                                    craftList.Add(skin2);
                            }
                                

                        foreach (var skinRange in GroupOutcomes(craftList))
                        {
                            ConsoleBuffer.Append($"{newLine}--------Length: {skinRange.Count}--------");
                            foreach (var skinObj in skinRange)
                                ConsoleBuffer.Append(newLine+skinObj.ToString());
                        }
                        */
                        #endregion

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
            else if (text == "Well-Worn")
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

        private void MaximizeMinimizeButton(object sender, EventArgs e)
        {
            var buttonText = ((Button)sender).Text;
            if(buttonText == "_") WindowState = FormWindowState.Minimized;
            else
            {
                WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            }

        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();


        private const int
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17;

        const int _ = 16;

        Rectangle TopCursor { get { return new Rectangle(0, 0, this.ClientSize.Width, _); } }
        Rectangle LeftCursor { get { return new Rectangle(0, 0, _, this.ClientSize.Height); } }
        Rectangle BottomCursor { get { return new Rectangle(0, this.ClientSize.Height - _, this.ClientSize.Width, _); } }
        Rectangle RightCursor { get { return new Rectangle(this.ClientSize.Width - _, 0, _, this.ClientSize.Height); } }

        Rectangle TopLeft { get { return new Rectangle(0, 0, _, _); } }
        Rectangle TopRight { get { return new Rectangle(this.ClientSize.Width - _, 0, _, _); } }
        Rectangle BottomLeft { get { return new Rectangle(0, this.ClientSize.Height - _, _, _); } }
        Rectangle BottomRight { get { return new Rectangle(this.ClientSize.Width - _, this.ClientSize.Height - _, _, _); } }


        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == 0x84) // WM_NCHITTEST
            {
                var cursor = this.PointToClient(Cursor.Position);

                if (TopLeft.Contains(cursor)) message.Result = (IntPtr)HTTOPLEFT;
                else if (TopRight.Contains(cursor)) message.Result = (IntPtr)HTTOPRIGHT;
                else if (BottomLeft.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMLEFT;
                else if (BottomRight.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (TopCursor.Contains(cursor)) message.Result = (IntPtr)HTTOP;
                else if (LeftCursor.Contains(cursor)) message.Result = (IntPtr)HTLEFT;
                else if (RightCursor.Contains(cursor)) message.Result = (IntPtr)HTRIGHT;
                else if (BottomCursor.Contains(cursor)) message.Result = (IntPtr)HTBOTTOM;
            }
        }

        private void WindowDragEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void copyButtonClick(object sender, EventArgs e)
        {
            Clipboard.SetText(((System.Windows.Forms.Button)sender).Tag.ToString());
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm settings = new SettingsForm();
            settings.ShowDialog();
            CheckRegistry();
            registryData = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\FloatTool");
            ChangeTheme(Convert.ToBoolean(registryData.GetValue("darkMode")));
            muteSound = !Convert.ToBoolean(registryData.GetValue("sound"));
            WorkStatusUpdater.Interval = (int)registryData.GetValue("bufferSpeed");
            discordWorker = Convert.ToBoolean(registryData.GetValue("discordRPC"));
            currentCurr = (Currency)registryData.GetValue("currency");
        }

        private void benchmarkButton_Click(object sender, EventArgs e)
        {
            Benchmark benchmark = new Benchmark(versionLabel.Text);
            benchmark.ShowDialog();
        }

        void ChangeTheme(bool dark)
        {
            if (dark)
            {
                BackColor = Color.FromArgb(44, 44, 44);
                outputConsoleBox.BackColor = Color.FromArgb(31, 31, 31);
                outputConsoleBox.ForeColor = Color.FromArgb(255, 255, 255);
                splitContainer1.BackColor = Color.FromArgb(31, 31, 31);
                splitContainer1.Panel1.BackColor = Color.FromArgb(31, 31, 31);
                splitContainer1.Panel2.BackColor = Color.FromArgb(31, 31, 31);

                foundCombinationContainer.BackColor = Color.FromArgb(37, 37, 37);

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
                label11.ForeColor = Color.FromArgb(255, 255, 255);
                label12.ForeColor = Color.FromArgb(255, 255, 255);

                panel5.BackColor = Color.FromArgb(44, 44, 44);
                panel6.BackColor = Color.FromArgb(44, 44, 44);

                weaponTypeBox.BackColor = Color.FromArgb(32, 32, 32);
                weaponTypeBox.ForeColor = Color.FromArgb(150, 150, 150);
                weaponSkinBox.BackColor = Color.FromArgb(32, 32, 32);
                weaponSkinBox.ForeColor = Color.FromArgb(150, 150, 150);
                weaponQualityBox.BackColor = Color.FromArgb(32, 32, 32);
                weaponQualityBox.ForeColor = Color.FromArgb(150, 150, 150);

                outcomeSelectorComboBox.BackColor = Color.FromArgb(32, 32, 32);
                outcomeSelectorComboBox.ForeColor = Color.FromArgb(150, 150, 150);

                stattrackCheckBox.TurnedOffColor = Color.FromArgb(56, 56, 56);
                stattrackCheckBox.TurnedOnColor = Color.Green;

                minimizeBtn.ForeColor = Color.FromArgb(255, 255, 255);
                closeBtn.ForeColor = Color.FromArgb(255, 255, 255);
                MaximizeButton.ForeColor = Color.FromArgb(255, 255, 255);
                settingsButton.BackgroundImage = Properties.Resources.gearWhite;
                benchmarkButton.BackgroundImage = Properties.Resources.benchmarkWhite;

                minimizeBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);
                settingsButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);
                closeBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);
                MaximizeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);
                settingsButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);
                benchmarkButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 0, 0);

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

                downloadProgressBar.ForeColor = Color.White;
                downloadProgressBar.ProgressColor = Color.Green;
                downloadProgressBar.BackColor = Color.FromArgb(32, 32, 32);
            }
            else
            {
                BackColor = Color.FromArgb(222, 222, 222);
                outputConsoleBox.BackColor = Color.FromArgb(255, 255, 255);
                outputConsoleBox.ForeColor = Color.FromArgb(0, 0, 0);
                splitContainer1.BackColor = Color.FromArgb(255, 255, 255);
                splitContainer1.Panel1.BackColor = Color.FromArgb(255, 255, 255);
                splitContainer1.Panel2.BackColor = Color.FromArgb(255, 255, 255);

                foundCombinationContainer.BackColor = Color.FromArgb(222, 222, 222);

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
                label11.ForeColor = Color.FromArgb(0, 0, 0);
                label12.ForeColor = Color.FromArgb(0, 0, 0);

                panel5.BackColor = Color.FromArgb(222, 222, 222);
                panel6.BackColor = Color.FromArgb(222, 222, 222);

                weaponTypeBox.BackColor = Color.FromArgb(255, 255, 255);
                weaponTypeBox.ForeColor = Color.FromArgb(10, 10, 10);
                weaponSkinBox.BackColor = Color.FromArgb(255, 255, 255);
                weaponSkinBox.ForeColor = Color.FromArgb(10, 10, 10);
                weaponQualityBox.BackColor = Color.FromArgb(255, 255, 255);
                weaponQualityBox.ForeColor = Color.FromArgb(10, 10, 10);
                outcomeSelectorComboBox.BackColor = Color.FromArgb(255, 255, 255);
                outcomeSelectorComboBox.ForeColor = Color.FromArgb(10, 10, 10);

                stattrackCheckBox.TurnedOffColor = Color.FromArgb(200, 200, 200);
                stattrackCheckBox.TurnedOnColor = Color.LimeGreen;

                minimizeBtn.ForeColor = Color.FromArgb(0, 0, 0);
                closeBtn.ForeColor = Color.FromArgb(0, 0, 0);
                MaximizeButton.ForeColor = Color.FromArgb(0, 0, 0);
                settingsButton.BackgroundImage = Properties.Resources.gearBlack;
                benchmarkButton.BackgroundImage = Properties.Resources.benchmarkBlack;

                minimizeBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                settingsButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                closeBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                MaximizeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                settingsButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                benchmarkButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);

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

                downloadProgressBar.ForeColor = Color.Black;
                downloadProgressBar.ProgressColor = Color.FromArgb(119, 194, 119);
                downloadProgressBar.BackColor = Color.FromArgb(234, 234, 234);
            }
        }

        private void gpuSearch_btn_Click(object sender, EventArgs e)
        {
            if (discordWorker)
            {
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
            }
            SwitchEnabled();
        }

        BigInteger last = 0;
        private void timer2_Tick(object sender, EventArgs e)
        {
            var hundrMilsCount = currComb - last;
            var speed = (double)(hundrMilsCount) * 1000 / WorkStatusUpdater.Interval;
            speedStatusLabel.Text = $"Текущая скорость: {speed} комбинаций/сек";
            last = currComb;
            combinationsStatusLabel.Text = $"Проверено комбинаций: {currComb} / {totalComb}";

            foreach (var tmpPanel in combinationWindows)
            {
                foundCombinationContainer.Controls.Add(tmpPanel);
                foundCombinationContainer.ScrollControlIntoView(tmpPanel);
            }

            combinationWindows.Clear();

            if (ConsoleBuffer.Length > 0)
            {
                outputConsoleBox.Text += ConsoleBuffer.ToString();
                ConsoleBuffer.Clear();
                outputConsoleBox.ScrollToCaret();
            }

            if (totalComb != 0 && currComb < totalComb && Searching)
                downloadProgressBar.Value = ((float)((double)(currComb) / (double)(totalComb) * 1000));
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                System.Diagnostics.Process.Start("https://prevter.github.io/FloatTool-GUI/table.html");
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
