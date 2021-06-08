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
using System.Diagnostics;

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
            decimal wantFloat = 1;
            if (CurrentSearchMode != SearchMode.Equal)
                decimal.TryParse(want, NumberStyles.Any, CultureInfo.InvariantCulture, out wantFloat);

            var inputArr = inputs.ToArray();

            for (int i = 0; i < outputs.Count; i++)
            {
                decimal flotOrigin = Math.Round(craft(inputArr, outputs[i].MinFloat, outputs[i].FloatRange), 14);
                
                if (
                    (flotOrigin.ToString(CultureInfo.InvariantCulture).StartsWith(want, StringComparison.Ordinal) && CurrentSearchMode == SearchMode.Equal) ||
                    (CurrentSearchMode == SearchMode.Less && (flotOrigin < wantFloat)) ||
                    (CurrentSearchMode == SearchMode.Greater && (flotOrigin > wantFloat))
                )
                {
                    string flot = craftF(inputs, (float)outputs[i].MinFloat, (float)outputs[i].MaxFloat);
                    Invoke((MethodInvoker)(() =>
                    {
                        float price = 0f;
                        List<string> floatStrings = new List<string>();

                        foreach (var fl in inputs)
                        {
                            floatStrings.Add(Math.Round(fl.WearValue, 14).ToString(CultureInfo.InvariantCulture));
                            price += fl.Price;
                        }

                        Logger.Log($"[{DateTime.Now}]: Found coombination {{");
                        Logger.Log($"   Float      = {flotOrigin}{newLine}" +
                                   $"   IEEE754    = {flot}{newLine}" +
                                   $"   Price      = {price} {inputs[0].SkinCurrency}{newLine}" +
                                   $"   Float list = [{string.Join(", ", floatStrings)}]{newLine}}}");

                        AddCombinationToList(DateTime.Now.ToString("HH:mm:ss"), flotOrigin, flot, price, floatStrings);
                        ConsoleBuffer.Append($"[{DateTime.Now.ToString("HH:mm:ss")}] {strings.CombinationFound}{newLine}");
                        ConsoleBuffer.Append($"{strings.PossibleFloat}: {flotOrigin}{newLine}");
                        ConsoleBuffer.Append($"{strings.TestFloat}: {flot}{newLine}");
                        ConsoleBuffer.Append($"Цена: {price} {inputs[0].SkinCurrency}{newLine}");
                        ConsoleBuffer.Append($"{strings.FloatList}: ");


                        if (SingleSearch) Searching = false;

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
                        
                        ConsoleBuffer.Append($"[{string.Join(", ", floatStrings)}]{newLine}====================================={newLine}");
                    }
                    ));
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
            startSearchSingleButton.Enabled = !startSearchSingleButton.Enabled;
        }

        public List<Skin> twentyfour = new List<Skin>();

        public void UpdateOutcomes()
        {
            twentyfour.Clear();
            string skin = $"{weaponTypeBox.Text} | {weaponSkinBox.Text}";
            outcomeSelectorComboBox.Items.Clear();
            List<dynamic> craftList = new List<dynamic>();
            using (StreamReader r = new StreamReader("itemData.json"))
            {
                string json = r.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);
                foreach (var skn in items)
                {
                    if (string.Compare(skn["name"].ToString(), skin) == 0)
                    {
                        foreach (var skin2 in items)
                            if (string.Compare(skn["case"].ToString(), skin2["case"].ToString()) == 0)
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
                    twentyfour.Add(skinRange[0]);
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



            Logger.Log($"[{DateTime.Now}]: Changed search skin to: {search}");
        }

        public void AutoUpdater()
        {
            string ver = versionLabel.Text;
            string data = CheckUpdates();
            string lastver = data.Split('|')[0];
            Logger.Log($"Checked version is: {lastver}{newLine}Installed: {ver}");
            if (ver != lastver)
            {
                DialogResult result = MessageBox.Show(
                    $"Доступна версия {lastver}! Хотите обновить?",
                    "Доступно обновление",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);

                if (result == DialogResult.Yes)
                {
                    string strExeFilePath = Assembly.GetExecutingAssembly().Location;
                    string strWorkPath = Path.GetDirectoryName(strExeFilePath);
                    Process.Start($@"{strWorkPath}\Updater.exe", data.Split('|')[1]);
                    Invoke((MethodInvoker)(() => { Close(); } ));
                }
            }
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
            UpdateCustomPalette();
            registryData = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\FloatTool", true);

            
            var soundRK = registryData.GetValue("sound");
            var bufferSpeedRK = registryData.GetValue("bufferSpeed");
            var discordRPCRK = registryData.GetValue("discordRPC");
            var currencyRK = registryData.GetValue("currency");
            var updateCheckRK = registryData.GetValue("updateCheck");
            var lastThreadsRK = registryData.GetValue("lastThreads");
            var themeRK = registryData.GetValue("theme");

            CurrentPallete = (Pallete)themeRK;
            ChangeTheme();
            muteSound = !Convert.ToBoolean(soundRK);
            WorkStatusUpdater.Interval = (int)bufferSpeedRK;
            discordWorker = Convert.ToBoolean(discordRPCRK);
            currentCurr = (Currency)currencyRK;
            threadCountInput.Value = (int)lastThreadsRK;

            Logger.Log($"[{DateTime.Now}]: Loaded registry settings:");
            Logger.Log($"Theme         = {CurrentPallete}");
            Logger.Log($"Sound         = {soundRK}");
            Logger.Log($"Buffer speed  = {bufferSpeedRK}");
            Logger.Log($"Discord RPC   = {discordRPCRK}");
            Logger.Log($"Currency      = {currencyRK}");
            Logger.Log($"Check updates = {updateCheckRK}");
            Logger.Log($"Last threads  = {lastThreadsRK}");

            if (Convert.ToBoolean(updateCheckRK))
            {
                string ver = versionLabel.Text;
                try
                {
                    Thread updater = new Thread(AutoUpdater);
                    updater.Start();
                }
                catch(Exception ex)
                {
                    Logger.Log($"[{DateTime.Now}]: {{EXCEPTION}} {ex.Message}{newLine}{ex.StackTrace}");
                    Logger.SaveCrashReport();
                }
            }
            Logger.Log($"[{DateTime.Now}]: Initialized");
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

                    if (string.Compare(skin["name"].ToString().Split('|')[0].TrimEnd(), weaponTypeBox.Text) == 0)
                    {
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
            MaximizedBounds = Screen.FromHandle(Handle).WorkingArea;
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
            foreach (IEnumerable<InputSkin> _ in Combinations(pool, start, skip))
            {
                parseCraft(_.ToList(), craftList, wanted);
                Interlocked.Increment(ref currComb);
            }
        }

        public List<Panel> combinationWindows = new List<Panel>();

        public void AddCombinationToList(string time, decimal flotOrigin, string flot, float price, List<string> floatStrings)
        {
            Panel tmpPanel = new Panel
            {
                BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary1),
                ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1),
                Size = new Size(425, 220),
                Margin = new Padding(3),
                Font = new Font("Inter", 10f)
            };
            #region Labels
                        tmpPanel.Controls.Add(new Label
                        {
                            AutoSize = true,
                            Location = new Point(3,3),
                            Text = $"{outcomeSelectorComboBox.Text}\nВозможный флоат: {flotOrigin.ToString(CultureInfo.InvariantCulture)}\nIEEE754: {flot}"
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
                    BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary4),
                    ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1),
                    ReadOnly = true,
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point(x, y),
                    Size = new Size(116, 25),
                    Text = floatStrings[i]
                });

                var tmpButton = new Button
                {
                    BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary5),
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
        long currComb = 0;
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

            registryData.SetValue("lastThreads", (int)threadCountInput.Value);

            totalComb = quantityInput.Value == 10 ? 1 : Fact((int)quantityInput.Value) / (Fact(10) * Fact((int)quantityInput.Value - 10));
            currComb = 0;
            Invoke((MethodInvoker)(() =>
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

                    List<Thread> downloadThreads = new List<Thread>();

                    foreach (var el in r["listinginfo"])
                    {
                        var eltemp = el;
                        var t = new Thread((eltemp) =>
                        {
                            string lid = r["listinginfo"][el.Name]["listingid"];
                            string aid = r["listinginfo"][el.Name]["asset"]["id"];
                            string link = r["listinginfo"][el.Name]["asset"]["market_actions"][0]["link"];

                            try
                            {
                                inputSkins.Add(new InputSkin(
                                    GetWearFromInspectURL(link.Replace("%assetid%", aid).Replace("%listingid%", lid)),
                                    (float.Parse(r["listinginfo"][el.Name]["converted_price"].ToString()) + float.Parse(r["listinginfo"][el.Name]["converted_fee"].ToString())) / 100,
                                    currentCurr)
                                );
                            }
                            catch (Exception ex)
                            {
                                if (ex.GetType() != typeof(ThreadAbortException))
                                {
                                    Logger.Log($"[{DateTime.Now}]: {{EXCEPTION}} {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                                    Logger.SaveCrashReport();
                                }
                            }

                            Interlocked.Increment(ref counter);
                            Invoke((MethodInvoker)(() =>
                            {
                                downloadProgressBar.Value = counter;
                            }));
                        });
                        t.Start();
                        downloadThreads.Add(t);
                    }

                    while (true)
                    {
                        bool okey = true;
                        foreach (Thread t in downloadThreads)
                        {
                            if (t.IsAlive)
                            {
                                okey = false;
                                break;
                            }
                        }
                        if (okey) break;
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
            Invoke((MethodInvoker)(() =>
            {
                outputConsoleBox.AppendText( "Поиск ауткамов..." + newLine);
                outputConsoleBox.SelectionStart = fullSkinName.Text.Length;

                Logger.Log($"[{DateTime.Now}] Float list: [{string.Join(", ", inputSkins)}]");
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
                    if (string.Compare(skin["case"].ToString(), currData.Split(',')[0]) == 0)
                    {
                        if (string.Compare(skin["rarity"].ToString().Split(' ')[0], getNextRarity(currData.Split(',')[1].Split(' ')[0])) == 0)
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
                for (int j = 0; j < threads; j++)
                {
                    var startIndex = j;
                    Thread newThread = new Thread(() => secndThread(outcomes, wanted, inputSkins, startIndex, threads));
                    newThread.Start();
                    t2.Add(newThread);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            Console.WriteLine($"[DEBUG] {threads} threads started!");

            while (Searching)
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

            foreach (Thread t in t2)
            {
                t.Abort();
            }

            Invoke((MethodInvoker)(() => 
            {
                outputConsoleBox.AppendText( "Программа завершила проверку всех комбинаций!" + newLine);
                fullSkinName.SelectionStart = fullSkinName.Text.Length;
                outputConsoleBox.ScrollToCaret();
                thread1.Abort();
                startBtn.Text = "Старт";
                downloadProgressBar.Value = 0;
                SwitchEnabled();
            }));
            
        }



        private void StartButtonClick(object sender, EventArgs e)
        {
            if (((Button)sender).Name == "startSearchSingleButton")
                SingleSearch = true;
            else
                SingleSearch = false;

            if (startBtn.Text == "Старт")
            {
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

        private void CloseAppButton_Click(object sender, EventArgs e)
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
        public bool isDarkMode { get; private set; }
        public bool SingleSearch { get; private set; }

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
            Clipboard.SetText(((Button)sender).Tag.ToString());
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm settings = new SettingsForm();
            settings.ShowDialog();
            CheckRegistry();
            registryData = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\FloatTool");
            muteSound = !Convert.ToBoolean(registryData.GetValue("sound"));
            WorkStatusUpdater.Interval = (int)registryData.GetValue("bufferSpeed");
            discordWorker = Convert.ToBoolean(registryData.GetValue("discordRPC"));
            currentCurr = (Currency)registryData.GetValue("currency");
            CurrentPallete = (Pallete)registryData.GetValue("theme");
            ChangeTheme();
        }

        private void benchmarkButton_Click(object sender, EventArgs e)
        {
            Benchmark benchmark = new Benchmark(versionLabel.Text);
            benchmark.ShowDialog();
        }

        void ChangeTheme()
        {
            if (CurrentPallete == Pallete.Dark)
            {
                settingsButton.BackgroundImage = Properties.Resources.gearWhite;
                benchmarkButton.BackgroundImage = Properties.Resources.benchmarkWhite;
            }
            else if (CurrentPallete == Pallete.Light)
            {
                settingsButton.BackgroundImage = Properties.Resources.gearBlack;
                benchmarkButton.BackgroundImage = Properties.Resources.benchmarkBlack;
            }

            if (CurrentPallete == Pallete.Custom)
            {
                UpdateCustomPalette();
                if (CustomPalette.IsDarkButtons)
                {
                    settingsButton.BackgroundImage = Properties.Resources.gearBlack;
                    benchmarkButton.BackgroundImage = Properties.Resources.benchmarkBlack;
                }
                else
                {
                    settingsButton.BackgroundImage = Properties.Resources.gearWhite;
                    benchmarkButton.BackgroundImage = Properties.Resources.benchmarkWhite;
                }
            }

            BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary1);
            outputConsoleBox.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary2);
            outputConsoleBox.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            splitContainer1.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary2);
            splitContainer1.Panel1.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary2);
            splitContainer1.Panel2.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary2);

            foundCombinationContainer.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary3);

            panel10.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary2);
            panel12.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary2);

            panel3.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary3);
            panel9.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary3);

            weaponTypeLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            skinLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            qualityLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            fullnameLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            neededfloatLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            countLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            skipLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            floattoolTitle.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            outcomesLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            stattrackLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            craftRangeLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);

            panel5.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary1);
            panel6.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary1);

            weaponTypeBox.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary4);
            weaponTypeBox.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary2);
            weaponSkinBox.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary4);
            weaponSkinBox.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary2);
            weaponQualityBox.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary4);
            weaponQualityBox.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary2);

            outcomeSelectorComboBox.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary4);
            outcomeSelectorComboBox.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary2);

            stattrackCheckBox.TurnedOffColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary6);
            stattrackCheckBox.TurnedOnColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary3);

            minimizeBtn.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            closeBtn.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            MaximizeButton.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);

            minimizeBtn.FlatAppearance.MouseOverBackColor = GetPalleteColor(CurrentPallete, PalleteColor.OverBackColor1);
            settingsButton.FlatAppearance.MouseOverBackColor = GetPalleteColor(CurrentPallete, PalleteColor.OverBackColor1);
            closeBtn.FlatAppearance.MouseOverBackColor = GetPalleteColor(CurrentPallete, PalleteColor.OverBackColor1);
            MaximizeButton.FlatAppearance.MouseOverBackColor = GetPalleteColor(CurrentPallete, PalleteColor.OverBackColor1);
            settingsButton.FlatAppearance.MouseOverBackColor = GetPalleteColor(CurrentPallete, PalleteColor.OverBackColor1);
            benchmarkButton.FlatAppearance.MouseOverBackColor = GetPalleteColor(CurrentPallete, PalleteColor.OverBackColor1);

            sortCheckBox.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            ascendingCheckBox.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);

            checkPossibilityBtn.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary5);
            startBtn.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary5);
            startSearchSingleButton.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary5);
            checkPossibilityBtn.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            startBtn.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            startSearchSingleButton.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);

            checkPossibilityBtn.FlatAppearance.MouseOverBackColor = GetPalleteColor(CurrentPallete, PalleteColor.OverBackColor2);
            startBtn.FlatAppearance.MouseOverBackColor = GetPalleteColor(CurrentPallete, PalleteColor.OverBackColor2);
            startSearchSingleButton.FlatAppearance.MouseOverBackColor = GetPalleteColor(CurrentPallete, PalleteColor.OverBackColor2);

            searchmodeEqual_btn.FlatAppearance.MouseOverBackColor = GetPalleteColor(CurrentPallete, PalleteColor.OverBackColor2);
            searchmodeLess_btn.FlatAppearance.MouseOverBackColor = GetPalleteColor(CurrentPallete, PalleteColor.OverBackColor2);
            searchmodeGreater_btn.FlatAppearance.MouseOverBackColor = GetPalleteColor(CurrentPallete, PalleteColor.OverBackColor2);

            fullSkinName.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary4);
            fullSkinName.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary2);
            searchFloatInput.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary4);
            searchFloatInput.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary2);

            quantityInput.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary4);
            quantityInput.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary2);
            skipValueInput.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary4);
            skipValueInput.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary2);

            threadsLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            threadCountInput.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary4);
            threadCountInput.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary2);

            searchModeLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            speedStatusLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            combinationsStatusLabel.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);

            searchmodeLess_btn.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary5);
            searchmodeLess_btn.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            searchmodeEqual_btn.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary5);
            searchmodeEqual_btn.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            searchmodeGreater_btn.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary5);
            searchmodeGreater_btn.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);

            gpuSearch_btn.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary5);
            gpuSearch_btn.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);

            downloadProgressBar.ForeColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary1);
            downloadProgressBar.ProgressColor = GetPalleteColor(CurrentPallete, PalleteColor.Secondary3);
            downloadProgressBar.BackColor = GetPalleteColor(CurrentPallete, PalleteColor.Primary4);
        }

        private void outcomeSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (outcomeSelectorComboBox.SelectedIndex > twentyfour.Count - 1)
            {
                craftRangeLabel.Text = $"Диапазон крафта:{newLine}0 - 1";
                return;
            }
            float lowestWear = 0;
            float highestWear = 1;

            switch (weaponQualityBox.Text)
            {
                case "Factory New":
                    lowestWear = 0f;
                    highestWear = 0.07f;
                    break;
                case "Minimal Wear":
                    lowestWear = 0.07f;
                    highestWear = 0.15f;
                    break;
                case "Field-Tested":
                    lowestWear = 0.15f;
                    highestWear = 0.38f;
                    break;
                case "Well-Worn":
                    lowestWear = 0.38f;
                    highestWear = 0.45f;
                    break;
                case "Battle-Scarred":
                    lowestWear = 0.45f;
                    highestWear = 1f;
                    break;
                default:
                    lowestWear = 0f;
                    highestWear = 1f;
                    break;
            }

            List<InputSkin> lowest = new List<InputSkin>();
            for (int i = 0; i < 10; i++)
                lowest.Add(new InputSkin(lowestWear, 0, currentCurr));

            List<InputSkin> highest = new List<InputSkin>();
            for (int i = 0; i < 10; i++)
                highest.Add(new InputSkin(highestWear, 0, currentCurr));

            var currSkin = twentyfour[outcomeSelectorComboBox.SelectedIndex];
            float minCraftWear = Convert.ToSingle(craftF(lowest, (float)currSkin.MinFloat, (float)currSkin.MaxFloat), CultureInfo.InvariantCulture);
            float maxCraftWear = Convert.ToSingle(craftF(highest, (float)currSkin.MinFloat, (float)currSkin.MaxFloat), CultureInfo.InvariantCulture);

            craftRangeLabel.Text = $"Диапазон крафта:{newLine}{minCraftWear.ToString("0.00", CultureInfo.InvariantCulture)} - {maxCraftWear.ToString("0.00", CultureInfo.InvariantCulture)}";
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
        private Pallete CurrentPallete;

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
                downloadProgressBar.Value = ((float)(currComb / (double)totalComb * 1000));
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

        private void debugMenuShow(object sender, MouseEventArgs e)
        {
            /*double[] pool = {
                0.246938750147820, 0.196652039885521, 0.154839321970940,
                0.333326697349548, 0.163415759801865, 0.291821509599686,
                0.374309629201889, 0.378754675388336, 0.231419935822487,
                0.311867892742157
            };
            List<InputSkin> ingridients = new List<InputSkin>();
            foreach (double f in pool) ingridients.Add(new InputSkin(f, 1, currentCurr));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 10000000; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    var output = ingridients[j].WearValue;
                }
            }
            stopwatch.Stop();
            double newSpeed = stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();
            var ingridientsArr = ingridients.ToArray();
            stopwatch.Start();
            for (int i = 0; i < 10000000; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var output = ingridientsArr[j].WearValue;
                }
            }
            stopwatch.Stop();
            double oldSpeed = stopwatch.ElapsedMilliseconds;
            MessageBox.Show($"list - {newSpeed} ms / 1000000\n\rarr  - {oldSpeed} ms / 1000000");*/


            /*double[] pool = {
                0.246938750147820, 0.196652039885521, 0.154839321970940,
                0.333326697349548, 0.163415759801865, 0.291821509599686,
                0.374309629201889, 0.378754675388336, 0.231419935822487,
                0.311867892742157
            };
            List<InputSkin> inputSkins = new List<InputSkin>();
            foreach (double f in pool) inputSkins.Add(new InputSkin(f, 1, currentCurr));
            string output = "";

            

            MessageBox.Show($"c++ - {output} ({newSpeed} ms / 100000)\n\rc#  - {outputOld} ({oldSpeed} ms / 100000)");*/
        }
    }
}
