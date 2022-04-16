using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace FloatTool
{
    public class BenchmarkViewModel : INotifyPropertyChanged
    {
        public class BenchmarkResult
        {
            public string CpuName { get; set; }
            public string ThreadCount { get; set; }
            public string MultithreadedScore { get; set; }
            public string SinglethreadedScore { get; set; }
            public LinearGradientBrush FillBrush { get; set; }
            public GridLength FillSize { get; set; }
            public GridLength EmptySize { get; set; }
        }

        public Settings Settings;

        public long TotalCombinations { get; set; }

        private int progressPercentage;
        private bool buttonsEnabled = true;
        private bool canPublish = false;

        public int ProgressPercentage
        {
            get { return progressPercentage; }
            set
            {
                progressPercentage = value;
                OnPropertyChanged();
            }
        }

        public bool ButtonsEnabled
        {
            get { return buttonsEnabled; }
            set
            {
                buttonsEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool CanPublish
        {
            get { return canPublish; }
            set
            {
                canPublish = value;
                OnPropertyChanged();
            }
        }

        private int threadCount = Environment.ProcessorCount;
        public int ThreadCount
        {
            get
            {
                return threadCount;
            }
            set
            {
                threadCount = value;
                OnPropertyChanged();
            }
        }

        private int multithreadedSpeed = 0;
        public int MultithreadedSpeed
        {
            get { return multithreadedSpeed; }
            set
            {
                multithreadedSpeed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MultithreadedSpeedText)));
            }
        }
        public string MultithreadedSpeedText { get { return $"{multithreadedSpeed:n0}"; } }

        private int singlethreadedSpeed = 0;
        public int SinglethreadedSpeed
        {
            get { return singlethreadedSpeed; }
            set
            {
                singlethreadedSpeed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SinglethreadedSpeedText)));
            }
        }
        public string SinglethreadedSpeedText { get { return $"{singlethreadedSpeed:n0}"; } }

        public ObservableCollection<BenchmarkResult> BenchmarkResults { get; set; }
        public string CurrentCpuName { get; set; }
        public static string CurrentCpuThreads { get { return $"{Environment.ProcessorCount}"; } }

        public static LinearGradientBrush AMDBrush = Application.Current.Resources["AmdBenchmarkFill"] as LinearGradientBrush;
        public static LinearGradientBrush IntelBrush = Application.Current.Resources["IntelBenchmarkFill"] as LinearGradientBrush;
        public static LinearGradientBrush CurrentBrush = Application.Current.Resources["CurrentBenchmarkFill"] as LinearGradientBrush;

        private bool isUpdatingEnabled { get; set; }
        public bool IsUpdatingEnabled
        {
            get { return isUpdatingEnabled; }
            set { isUpdatingEnabled = value; OnPropertyChanged(); }
        }

        public async void PollBenchmarkResults()
        {
            IsUpdatingEnabled = false;
            try
            {
                BenchmarkResults.Clear();
                using var client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(Utils.API_URL + "/LoadBenchmarks.php");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseBody);

                if (result.status == 200 && result.count > 0)
                {
                    float maxspeed = result.items[0].multithread;

                    foreach (var benchmark in result.items)
                    {
                        float percentage = (float)benchmark.multithread / maxspeed;
                        float reverse = 1 - percentage;

                        string cpuName = Utils.ShortCpuName((string)benchmark.name);
                        var currentFill = AMDBrush;
                        if (cpuName == CurrentCpuName)
                        {
                            currentFill = CurrentBrush;
                            MultithreadedSpeed = Math.Max((int)benchmark.multithread, MultithreadedSpeed);
                            SinglethreadedSpeed = Math.Max((int)benchmark.singlethread, SinglethreadedSpeed);
                        }
                        else if (cpuName.StartsWith("Intel"))
                            currentFill = IntelBrush;

                        string threadsString = Application.Current.Resources["m_Threads"] as string;
                        if ((int)benchmark.threads == 1)
                            threadsString = Application.Current.Resources["m_Thread"] as string;

                        BenchmarkResults.Add(new BenchmarkResult
                        {
                            CpuName = cpuName,
                            ThreadCount = $"{benchmark.threads} {threadsString} [{benchmark.version}]",
                            MultithreadedScore = $"{(int)benchmark.multithread:n0}",
                            SinglethreadedScore = $"{(int)benchmark.singlethread:n0}",
                            FillSize = new GridLength(percentage, GridUnitType.Star),
                            EmptySize = new GridLength(reverse, GridUnitType.Star),
                            FillBrush = currentFill
                        });
                    }
                }
            }
            catch (Exception)
            {
                BenchmarkResults.Add(new BenchmarkResult
                {
                    CpuName = "Error loading benchmark table.",
                    FillSize = new GridLength(0, GridUnitType.Star),
                    EmptySize = new GridLength(1, GridUnitType.Star),
                });
            }
            IsUpdatingEnabled = true;
        }

        public async void PushBenchmarkResults()
        {
            try
            {
                BenchmarkResults.Clear();
                using var client = new HttpClient();
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                client.DefaultRequestHeaders.Add("User-Agent", $"FloatTool/{App.VersionCode}");
                string paramedURL = $"/AddBenchmark.php?cpu={CurrentCpuName}&threads={ThreadCount}&multicore={MultithreadedSpeed}&singlecore={SinglethreadedSpeed}";
                HttpResponseMessage response = await client.GetAsync(Utils.API_URL + paramedURL);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                MessageBox.Show(responseBody);
            }
            catch (Exception) { }
            CanPublish = false;
            PollBenchmarkResults();
        }

        public BenchmarkViewModel(Settings settings)
        {
            Settings = settings;
            ThreadCount = settings.ThreadCount;
            string path = @"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0";
            string cpu = (string)Registry.GetValue(path, "ProcessorNameString", "Unknown");
            CurrentCpuName = Utils.ShortCpuName(cpu);

            BenchmarkResults = new ObservableCollection<BenchmarkResult>();
            PollBenchmarkResults();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
