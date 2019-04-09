using BarCheck.Properties;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Media;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BarCheck.Views;
using GalaSoft.MvvmLight.Messaging;

namespace BarCheck.ViewModel
{
    public sealed class MainViewModel : ViewModelBase, IDisposable
    {
        private SerialPort serialPort = new SerialPort(Constants.ComUnknown, 9600);
        private SerialPort aserialPort = new SerialPort(Constants.ComUnknown, 9600);
        public static string currentUserName;
        public MainViewModel()
        {
            Messenger.Default.Register<string>(this, nameof(MainViewModel),
                (msg) => this.SelectedT2VRuleName = msg);
            Messenger.Default.Register<string>(this, nameof(BarcodeHistory),
                (msg) => this.LoadLastHistory());
            this.serialPort.DataReceived += SerialPort_DataReceived;
            this.IsRetry = false;
#if Test
            this.PortName = "COM10";
#else
            this.PortName = this.GetFirstPortName();
            this.APortName = this.GetSecondPortName();
            this.GetOtherSettings();
#endif
            this.obsAllBarcodes = new ObservableCollection<AllBarcodeViewModel>();
            if (IsInDesignMode)
            {

            }
            else
            {
            }

        }


        public void LoadLastHistory()
        {
            this.ObsAllBarcodes.Clear();

            BarcodeHistory.Instance.UpdateHistoryDir();
            if (BarcodeHistory.Instance.UserWantsImportHistoryFiles())
                this.ImportHistory();
            BarcodeHistory.Instance.OpenHistoryFile();
        }

        private void ImportHistory()
        {
            List<AllBarcodeViewModel> lst = new List<AllBarcodeViewModel>();
            foreach (string historyFileName in BarcodeHistory.Instance.last5DaysHistoryFiles)
            {
                foreach (string line in File.ReadLines(historyFileName))
                {
                    string[] parts = line.Split(new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
                        lst.Add(AllBarcodeViewModel.ConvertFromLine(parts));
                    else if (parts.Length == 2)
                    {
                        this.RenameByLine(lst, parts);
                    }
                    else
                        throw new Exception($"记录行格式错误: {line}");
                }
                this.obsAllBarcodes = new ObservableCollection<AllBarcodeViewModel>(lst);
            }

            this.RaisePropertyChanged(nameof(ObsAllBarcodes));
        }

        private void RenameByLine(List<AllBarcodeViewModel> lst, string[] parts)
        {
            Debug.Assert(parts.Length == 2);
            lst.First(x => x.Index == Convert.ToInt32(parts[0])).Barcode = parts[1];
        }

        public string PortName
        {
            get
            {
                return this.serialPort.PortName;
            }
            set
            {
                if (this.serialPort.PortName != value)
                {
                    this.serialPort.PortName = value;
                    this.RaisePropertyChanged(nameof(PortName));
                }
            }
        }


        private string selectedT2VRuleName;
        public string SelectedT2VRuleName
        {
            get
            {
                return this.selectedT2VRuleName;
            }
            set
            {
                if (this.selectedT2VRuleName != value)
                {
                    this.selectedT2VRuleName = value;
                    if (!string.IsNullOrWhiteSpace(this.selectedT2VRuleName))
                    {
                        string regForat = ValidateRulesViewModel.GetVRRegStrByT2(this.SelectedT2VRuleName);
                        this.regBarcodeFormat = new Regex(regForat, RegexOptions.Compiled);
                    }
                    else
                        this.regBarcodeFormat = new Regex(".{2,}", RegexOptions.Compiled);
                }
            }
        }

        public string APortName
        {
            get
            {
                return this.aserialPort.PortName;
            }
            set
            {
                if (this.aserialPort.PortName != value)
                {
                    this.aserialPort.PortName = value;
                }
            }
        }

        private bool isRetry;
        public bool IsRetry
        {
            get
            {
                return this.isRetry;
            }
            set
            {
                if (this.isRetry != value)
                {
                    this.isRetry = value;
                    this.RaisePropertyChanged(nameof(IsRetry));
                }
            }
        }


        public bool IsOpened
        {
            get
            {
                return this.serialPort.IsOpen;
            }

        }


        private string currentRawBarcode;
        public string CurrentRawBarcode
        {
            get
            {
                return this.currentRawBarcode;
            }
            set
            {
                //not use if continuous NR
                //if (this.currentRawBarcode != value)
                {
                    this.currentRawBarcode = value;
                    this.RaisePropertyChanged(nameof(CurrentRawBarcode));
                }
            }
        }

        private string exportDir;
        public string ExportDir
        {
            get
            {
                return this.exportDir;
            }
            set
            {
                if (this.exportDir != value)
                {
                    this.exportDir = value;
                    this.RaisePropertyChanged(nameof(ExportDir));
                }
            }
        }


        private AllBarcodeViewModel lastAllBarcode;
        public AllBarcodeViewModel LastAllBarcode
        {
            get
            {
                return this.lastAllBarcode;
            }
            set
            {
                if (this.lastAllBarcode != value)
                {
                    this.lastAllBarcode = value;
                    this.RaisePropertyChanged(nameof(LastAllBarcode));
                }
            }
        }

        private ObservableCollection<AllBarcodeViewModel> obsAllBarcodes;
        public ObservableCollection<AllBarcodeViewModel> ObsAllBarcodes
        {
            get
            {
                return this.obsAllBarcodes;
            }
            set
            {
                if (this.obsAllBarcodes != value)
                {
                    this.obsAllBarcodes = value;
                    this.RaisePropertyChanged(nameof(ObsAllBarcodes));
                }
            }
        }

        public int GradeYesCount
        {
            get
            {
                return this.ObsAllBarcodes.Count(x => x.Status == BarcodeStatus.Yes);
            }
        }

        public int GradeNoCount
        {
            get
            {
                return this.ObsAllBarcodes.Count(x => x.Status == BarcodeStatus.NO);
            }
        }

        public int DupCount
        {
            get
            {
                return this.ObsAllBarcodes.Count(x => x.Status == BarcodeStatus.Dup);
            }
        }

        private string message;
        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.RaisePropertyChanged(nameof(Message));
                }
            }
        }


        private bool isOpening;
        private RelayCommand openCommand;

        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand
                  ?? (openCommand = new RelayCommand(
                    async () =>
                    {
                        if (isOpening)
                        {
                            return;
                        }

                        isOpening = true;
                        OpenCommand.RaiseCanExecuteChanged();

                        await Open();

                        isOpening = false;
                        OpenCommand.RaiseCanExecuteChanged();
                    },
                    () => !isOpening));
            }
        }
        private async Task Open()
        {
            await Task.Run(() =>
            {

            });
            this.OpenSerialPort();
            this.RaisePropertyChanged(nameof(IsOpened));
            if (this.IsOpened)
            {
                Settings.Default.PortName = this.PortName;
                Settings.Default.AlarmMs = this.alarmMs;
                Settings.Default.NRMaxCount = this.nRMaxCount;
                Settings.Default.NRIgnoreTime = this.nRIgnoreTime;
                Settings.Default.Save();
            }
            this.OpenASerialPort();
        }

        public void OpenSerialPort()
        {
            try
            {
                this.serialPort.Open();
                Log.Instance.Logger.InfoFormat("open {0} success", this.PortName);
                this.Message = string.Format("成功打开串口{0}！", this.PortName);
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.FatalFormat(ex.Message);
                MessengerInstance.Send<string>(ex.Message);
            }
        }

        private void OpenASerialPort()
        {
            try
            {
                if (this.aserialPort.IsOpen)
                    return;
                this.aserialPort.Open();
                Log.Instance.Logger.InfoFormat("open alarm {0} success", this.APortName);
                if (this.aserialPort.IsOpen)
                {
                    Settings.Default.APortName = this.APortName;
                    Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.FatalFormat(ex.Message);
                MessengerInstance.Send<string>(ex.Message);
            }
        }

        private bool isCloseing;
        private RelayCommand closeCommand;

        public RelayCommand CloseCommand
        {
            get
            {
                return closeCommand
                  ?? (closeCommand = new RelayCommand(
                    async () =>
                    {
                        if (isCloseing)
                        {
                            return;
                        }

                        isCloseing = true;
                        CloseCommand.RaiseCanExecuteChanged();

                        await Close();

                        isCloseing = false;
                        CloseCommand.RaiseCanExecuteChanged();
                    },
                    () => !isCloseing));
            }
        }

        [HandleProcessCorruptedStateExceptions]
        private async Task Close()
        {
            await Task.Run(() =>
            {
            });
            this.CloseSerialPort();
            this.RaisePropertyChanged(nameof(IsOpened));
        }

        public void CloseSerialPort()
        {
            try
            {
                this.serialPort.Close();
                Log.Instance.Logger.InfoFormat("close {0} success", this.PortName);
                this.Message = string.Format("成功关闭串口{0}！", this.PortName);
                this.CloseASerialPort();

            }
            catch (Exception ex)
            {
                Log.Instance.Logger.FatalFormat(ex.Message);
                MessengerInstance.Send<string>(ex.Message);
            }
        }

        private void CloseASerialPort()
        {
            try
            {
                Log.Instance.Logger.InfoFormat("close alarm {0} success", this.APortName);
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.FatalFormat(ex.Message);
                MessengerInstance.Send<string>(ex.Message);
            }
        }

        private bool isSeting;
        private RelayCommand setCommand;

        public RelayCommand SetCommand
        {
            get
            {
                return setCommand
                  ?? (setCommand = new RelayCommand(
                    async () =>
                    {
                        if (isSeting)
                        {
                            return;
                        }

                        isSeting = true;
                        SetCommand.RaiseCanExecuteChanged();

                        await Set();

                        isSeting = false;
                        SetCommand.RaiseCanExecuteChanged();
                    },
                    () => !isSeting));
            }
        }


        //
        private bool isSetVRing;
        private RelayCommand setVRCommand;
        public RelayCommand SetVRCommand
        {
            get
            {
                return setVRCommand
                  ?? (setVRCommand = new RelayCommand(
                    async () =>
                    {
                        if (isSetVRing)
                        {
                            return;
                        }

                        isSetVRing = true;
                        SetVRCommand.RaiseCanExecuteChanged();

                        await SetVR();

                        isSetVRing = false;
                        SetVRCommand.RaiseCanExecuteChanged();
                    },
                    () => !isSetVRing));
            }
        }
        //
        private int alarmMs;
        private int nRMaxCount;
        private int nRIgnoreTime;

        private async Task Set()
        {
            await Task.Run(() =>
            {

            });
            Log.Instance.Logger.Info($"Port={PortName} APort={APortName} alarmMs={alarmMs} ExportDir={ExportDir} NRMaxCount={nRMaxCount} NRIgnoreTime={nRIgnoreTime} selectedT2VRuleName={selectedT2VRuleName}");
            Log.Instance.Logger.Info("after settings:");
            SettingWindow setWin = new SettingWindow();
            setWin.Owner = Application.Current.MainWindow;
            SettingsViewModel setVM = (setWin.DataContext) as SettingsViewModel;
            setVM.SelectedPortName = this.PortName;
            setVM.SelectedAPortName = this.APortName;
            setVM.AlarmMs = this.alarmMs;
            setVM.NRMaxCount = this.nRMaxCount;
            setVM.NRIgnoreTime = this.nRIgnoreTime;
            setVM.ExportDir = this.ExportDir;

            if (setWin.ShowDialog() ?? false)
            {
                this.PortName = setVM.SelectedPortName;
                this.APortName = setVM.SelectedAPortName;
                this.RaisePropertyChanged(nameof(IsOpened));
                this.alarmMs = setVM.AlarmMs;
                this.nRMaxCount = setVM.NRMaxCount;
                this.nRIgnoreTime = setVM.NRIgnoreTime;
                this.ExportDir = setVM.ExportDir;
                Settings.Default.ExportDir = ExportDir;
                Settings.Default.Save();
                Log.Instance.Logger.Info($"Port={PortName} APort={APortName} alarmMs={alarmMs} ExportDir={ExportDir}  NRMaxCount={nRMaxCount} NRIgnoreTime={nRIgnoreTime} selectedT2VRuleName={selectedT2VRuleName}");
            }
        }


        //vr
        private async Task SetVR()
        {
            await Task.Run(() =>
            {

            });
            Log.Instance.Logger.Info($"SelectedT2VRuleName={SelectedT2VRuleName}");
            Log.Instance.Logger.Info("after settings:");
            ValidateRulesWindow setVRWin = new ValidateRulesWindow();
            setVRWin.Owner = Application.Current.MainWindow;
            ValidateRulesViewModel setVRVM = (setVRWin.DataContext) as ValidateRulesViewModel;
            if (setVRWin.ShowDialog() ?? false)
            {
                this.SelectedT2VRuleName = setVRVM.SelectedT2VRuleName;
                //Settings.Default.SelectedT2VRuleName = this.SelectedT2VRuleName;
                //Settings.Default.Save();//dup?
                Log.Instance.Logger.Info($"SelectedT2VRuleName={SelectedT2VRuleName}");
            }
        }
        //vr

        //import--
        private bool isImporting;
        private RelayCommand importCommand;

        public RelayCommand ImportCommand
        {
            get
            {
                return importCommand
                  ?? (importCommand = new RelayCommand(
                    async () =>
                    {
                        if (isImporting)
                        {
                            return;
                        }

                        isImporting = true;
                        ImportCommand.RaiseCanExecuteChanged();

                        await ImportBarcodesFromFile();

                        isImporting = false;
                        ImportCommand.RaiseCanExecuteChanged();
                    },
                    () => !isImporting));
            }
        }

        private List<Tuple<string, int>> dicImport;
        private async Task ImportBarcodesFromFile()
        {
            await Task.Run(() =>
            {

            });
            this.ObsAllBarcodes.Clear();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text (*.txt)|*.txt";
            this.dicImport = new List<Tuple<string, int>>();
            //{
            //     Tuple.Create<string,int>( "B111",1000 ),
            //     Tuple.Create<string,int>( "B112",1000 ),
            //     Tuple.Create<string,int>( "B111",1000 ),
            //     Tuple.Create<string,int>( "NR",500 ),
            //     Tuple.Create<string,int>( "NR",500 ),
            //     Tuple.Create<string,int>( "NR",1200 ),
            //     Tuple.Create<string,int>( "B113",1000 ),
            //};
            if (dlg.ShowDialog() ?? false)
            {
                this.Message = "开始导入" + dlg.FileName;
                string fileName = dlg.FileName;
                Regex reg = new Regex(@"([A-Z0-9]{2,20})\s+(\d{2,4})", RegexOptions.Compiled);
                foreach (string line in File.ReadLines(fileName, Encoding.Default))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    if (reg.IsMatch(line))
                    {
                        this.dicImport.Add(Tuple.Create<string, int>(reg.Match(line).Groups[1].Value,
                            Convert.ToInt32(reg.Match(line).Groups[2].Value)));
                    }
                    else
                    {
                        MessageBox.Show($"【{line}】不符合规定的格式(2~20个大写字母和数字为条码，2-4个数字为毫秒)");
                        return;
                    }

                }
                this.Message = "结束导入" + dlg.FileName;
                Thread t = new Thread(this.ImportFun);
                t.Start();
            }

        }

        private void ImportFun()
        {
            foreach (var kv in this.dicImport)
            {
                this.GotBarcode(kv.Item1);
                Thread.Sleep(kv.Item2);
            }
        }
        //--import


        private bool isExporting;
        private RelayCommand exportCommand;

        public RelayCommand ExportCommand
        {
            get
            {
                return exportCommand
                  ?? (exportCommand = new RelayCommand(
                    async () =>
                    {
                        if (isExporting)
                        {
                            return;
                        }

                        isExporting = true;
                        ExportCommand.RaiseCanExecuteChanged();

                        await Export();

                        isExporting = false;
                        ExportCommand.RaiseCanExecuteChanged();
                    },
                    () => !isExporting));
            }
        }

        private bool ExportTabTxt(string fileName)
        {
            try
            {
                Log.Instance.Logger.Info(fileName);
                this.Message = fileName;
                return true;
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.Error(ex.Message);
                this.Message = ex.Message;
                return false;
            }
        }



        private bool ExportAllBarocdeTxt(string txtFileName)
        {
            try
            {
                File.WriteAllLines(txtFileName, this.ObsAllBarcodes.Select(x => x.ToString()));
                Log.Instance.Logger.Info($"export to {txtFileName}");
                this.Message = txtFileName;
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Instance.Logger.Error(ex.Message);
                this.Message = ex.Message;
                MessageBox.Show($"导出失败，因为写入设置的导出路径需要管理员权限\n请到设置修改导出路径，或者下次以管理员身份运行程序。");
                return false;
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.Error(ex.Message);
                this.Message = ex.Message;
                return false;
            }
        }

        private async Task Export()
        {
            await Task.Run(() =>
            {
            });
            if (Directory.Exists(this.ExportDir))
            {
                string fileName = Path.Combine(this.ExportDir, BarcodeHistory.Instance.GeneratedExportTxtName);
                this.ExportAllBarocdeTxt(fileName);
            }
            else
            {
                MessageBox.Show("请先到设置里选择导出路径！");
            }
        }


        private string full = string.Empty;
        private const char CR = (char)13;
        private char[] splitChars = new char[] { CR };
        private static StringBuilder sb = new StringBuilder("");
        private static object obj = new object();


        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (obj)
            {
                string recieved = this.serialPort.ReadExisting();
                MainViewModel.sb.Append(recieved);
                if (recieved.Contains(CR))
                {
                    string[] ss = sb.ToString().Split(splitChars);
                    this.GotBarcode(ss[0]);
                    MainViewModel.sb.Clear();
                    MainViewModel.sb.Append(ss[1]);
                }
            }
        }

        private byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public void SendBytes(byte[] bytes)
        {
            try
            {
                this.aserialPort.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.Error(ex.Message);
                this.Message = ex.Message;
            }
        }
        private Guid latestG;
        private DateTime closeTime;

        private SoundPlayer player = new SoundPlayer();
        private bool soundFinished = true;
        public void PlaySound(string soundFullName)
        {
            if (!File.Exists(soundFullName))
            {
                Log.Instance.Logger.Error($"找不到声音文件：{soundFullName}!");
                return;
            }
            this.player.SoundLocation = soundFullName;
            if (soundFinished)
            {
                soundFinished = false;
                Task.Factory.StartNew(() =>
                {
                    player.PlaySync();
                    soundFinished = true;
                });
            }
        }


        public void Light(byte[] bytes)
        {
            try
            {
                latestG = Guid.NewGuid();
                Thread t = new Thread(() => LightFun(latestG, bytes));
                t.IsBackground = true;
                closeTime = DateTime.Now.AddMilliseconds(alarmMs);
                t.Start();
            }
            catch (Exception ex)
            {
                this.Message = ex.Message;
            }

        }
        public void LightFun(Guid g, byte[] bytes)
        {
            this.SendBytes(bytes);
            while (DateTime.Now < closeTime)
            {
                if (g != latestG)
                    return;
                Thread.Sleep(50);
            }
            this.SendBytes(Constants.LightAllOff);
        }

        private Regex regBarcodeFormat = new Regex(".{3,}", RegexOptions.Compiled);

        private bool isBarcodeValidFormat(string barcode)
        {
            return this.regBarcodeFormat.IsMatch(barcode);
        }

        private MainWindow mainWin = Application.Current.MainWindow as MainWindow;
        private void InvokeAddBarcode(AllBarcodeViewModel allVM)
        {
            if (App.Current != null)//walkaround
                App.Current.Dispatcher.BeginInvoke(new Action(
                    () =>
                    {
                        this.ObsAllBarcodes.Add(allVM);
                        mainWin.ScrollListBoxToButtom(allVM);
                    }));
        }

        private Stopwatch stopwatchLastNRRecord = new Stopwatch();
        private int NRTimes = 0;
        bool anotherBarcodeWithin2Seconds = false;
        private RegisteredWaitHandle registeredWaitHandle;

        public void GotBarcode(string barcode)
        {
            anotherBarcodeWithin2Seconds = true;
            barcode = barcode.Trim();
            this.CurrentRawBarcode = barcode;
            bool dup = false;
            int oldCount = this.ObsAllBarcodes.Count;

            if (!this.isBarcodeValidFormat(barcode))
            {
                MessageBox.Show($"{barcode}不符合指定的条码规则！");
                return;
            }

            if (barcode.ToUpper() == Constants.NR)
            {
                if (oldCount > 0
                && !this.ObsAllBarcodes[oldCount - 1].Valid
                && stopwatchLastNRRecord.ElapsedMilliseconds < this.nRIgnoreTime
                )
                {
                    Debug.WriteLine($"TotalMilliseconds={stopwatchLastNRRecord.ElapsedMilliseconds}");
                    Debug.WriteLine($"*****< 400*****");
                    return;
                }
                barcode = Constants.NR + DateTime.Now.ToString("ddmmssfff");
                this.IsRetry = true;
                this.NRTimes++;
                if (this.NRTimes >= this.nRMaxCount)
                {
                    if (this.registeredWaitHandle != null && anotherBarcodeWithin2Seconds == true)
                        this.registeredWaitHandle.Unregister(null);

                    anotherBarcodeWithin2Seconds = false;

                    this.registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                        new AutoResetEvent(false),
                        (state, bTimeout) =>
                        {
                            if (!anotherBarcodeWithin2Seconds)
                            {
                                this.PlaySound(Constants.sndNR);
                                this.Light(Constants.LightNR);
                                AllBarcodeViewModel nrAllVM = new AllBarcodeViewModel(
                                    barcode,
                                    false, false, oldCount + 1);
                                this.InvokeAddBarcode(nrAllVM);
                                //Debug.WriteLine($"*****timer{barcode}*********");
                                this.RaisePropertyChanged(nameof(GradeNoCount));
                                this.LastAllBarcode = nrAllVM;
                                BarcodeHistory.Instance.AppendBarcode(nrAllVM);
                                stopwatchLastNRRecord.Restart();
                                this.IsRetry = false;
                                this.NRTimes = 0;
                            }
                        },
                        null,
                        TimeSpan.FromSeconds(1), true);
                }
                return;
            }//NR
            else if (this.ObsAllBarcodes.Any(x => x.Barcode == barcode))
            {
                if (barcode == this.ObsAllBarcodes[oldCount - 1].Barcode)
                {
                    this.IsRetry = true;
                    this.NRTimes = 0;
                    stopwatchLastNRRecord.Reset();
                    return;
                }
                else
                {
                    dup = true;
                }
            }
            if (dup)
            {
                this.PlaySound(Constants.sndDup);
                this.Light(Constants.LightDup);
            }
            else
            {
                this.PlaySound(Constants.sndOK);
                this.Light(Constants.LightOK);
            }
            AllBarcodeViewModel newAllVM = new AllBarcodeViewModel(barcode, true, dup, oldCount + 1);
            this.InvokeAddBarcode(newAllVM);
            Debug.WriteLine($"*****main{barcode}*********");
            this.RaisePropertyChanged(nameof(GradeYesCount));
            this.RaisePropertyChanged(nameof(DupCount));
            this.LastAllBarcode = newAllVM;
            BarcodeHistory.Instance.AppendBarcode(newAllVM);
            this.IsRetry = false;
            this.NRTimes = 0;
            stopwatchLastNRRecord.Reset();
        }

        private Random rnd = new Random();
        public void AddRandomBarcode()
        {
            string barcode = rnd.Next(10000, 10010).ToString().PadLeft(12, '0');
            if (rnd.Next(1, 5) == 2)
                this.GotBarcode(Constants.NR);
            else
                this.GotBarcode(barcode);
        }

        private void GetOtherSettings()
        {
            this.alarmMs = Settings.Default.AlarmMs;
            this.nRMaxCount = Settings.Default.NRMaxCount;
            this.nRIgnoreTime = Settings.Default.NRIgnoreTime;
            this.ExportDir = Settings.Default.ExportDir;
            this.SelectedT2VRuleName = Settings.Default.SelectedT2VRuleName;
        }

        private string GetFirstPortName()
        {
            string[] names = SerialPort.GetPortNames();
            if (names.Length > 0)
            {
                string setPort = Settings.Default.PortName;

                if (!string.IsNullOrWhiteSpace(setPort) && names.Contains(setPort))
                    return setPort;
                else
                    return names[0];
            }
            else
                return Constants.ComUnknown;
        }

        private string GetSecondPortName()
        {
            string[] names = SerialPort.GetPortNames();
            if (names.Length > 0)
            {
                string setPort = Settings.Default.APortName;

                if (!string.IsNullOrWhiteSpace(setPort) && names.Contains(setPort))
                    return setPort;
                else
                {
                    var except1 = names.Where(x => x != this.PortName);
                    if (except1.Any())
                        return except1.First();
                    else
                        return Constants.ComUnknown;
                }
            }
            else
                return Constants.ComUnknown;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    this.serialPort.Dispose();
                    this.aserialPort.Dispose();
                    this.player.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~MainViewModel() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}