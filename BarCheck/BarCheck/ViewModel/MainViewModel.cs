using BarCheck.Properties;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Media;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BarCheck.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private SerialPort serialPort = new SerialPort(Constants.ComUnknown, 9600);
        private SerialPort aserialPort = new SerialPort(Constants.ComUnknown, 9600);
        public static string currentUserName;
        public MainViewModel()
        {
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


        public Visibility MFEVisible
        {
            get
            {
#if MFE
                return Visibility.Collapsed;
#else
                return Visibility.Visible;
#endif

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
            this.OpenSerialPort();
            this.RaisePropertyChanged(nameof(IsOpened));
            if (this.IsOpened)
            {
                Settings.Default.PortName = this.PortName;
                Settings.Default.AlarmMs = this.alarmMs;

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

        private int alarmMs;

        private async Task Set()
        {
            Log.Instance.Logger.Info($"Port={PortName} APort={APortName} alarmMs={alarmMs}");
            Log.Instance.Logger.Info("after settings:");
            SettingWindow setWin = new SettingWindow();
            setWin.Owner = MainWindow.Instance;
            SettingsViewModel setVM = (setWin.DataContext) as SettingsViewModel;
            setVM.SelectedPortName = this.PortName;
            setVM.SelectedAPortName = this.APortName;
            setVM.AlarmMs = this.alarmMs;
            setVM.ExportDir = this.ExportDir;

            if (setWin.ShowDialog() ?? false)
            {
                this.PortName = setVM.SelectedPortName;
                this.APortName = setVM.SelectedAPortName;
                this.RaisePropertyChanged(nameof(IsOpened));
                this.alarmMs = setVM.AlarmMs;
                this.ExportDir = setVM.ExportDir;
                Settings.Default.ExportDir = ExportDir;
                Settings.Default.Save();
                Log.Instance.Logger.Info($"Port={PortName} APort={APortName} alarmMs={alarmMs} ExportDir={ExportDir}");
            }
        }

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
            catch (Exception ex)
            {
                Log.Instance.Logger.Error(ex.Message);
                this.Message = ex.Message;
                return false;
            }
        }

        private async Task Export()
        {
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
                //Log.Instance.Logger.Error(ex.Message);
                this.Message = ex.Message;
            }
        }
        private Guid latestG;
        private DateTime closeTime;

        private SoundPlayer player = new SoundPlayer();
        private bool soundFinished = true;
        public void PlaySound(string soundFullName)
        {
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

        private int NRTimes = 0;
        bool anotherBarcodeWithin2Seconds = false;
        private RegisteredWaitHandle registeredWaitHandle;
        public void GotBarcode(string barcode)
        {
            if (App.Current != null)//walkaround
                App.Current.Dispatcher.BeginInvoke((Action)(delegate
                {
                    anotherBarcodeWithin2Seconds = true;
                    barcode = barcode.Trim();
                    this.CurrentRawBarcode = barcode;
                    bool dup = false;
                    int oldCount = this.ObsAllBarcodes.Count;
                    if (barcode.ToUpper() == Constants.NR)
                    {
                        barcode = Constants.NR + DateTime.Now.ToString("ddmmssfff");
                        this.IsRetry = true;
                        this.NRTimes++;
                        if (this.NRTimes >= 3)
                        {
                            if (this.registeredWaitHandle != null && anotherBarcodeWithin2Seconds == true)
                                this.registeredWaitHandle.Unregister(null);

                            anotherBarcodeWithin2Seconds = false;

                            this.registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                                new AutoResetEvent(false),
                                (state, bTimeout) =>
                                {
                                    App.Current.Dispatcher.BeginInvoke((Action)(delegate
                                    {
                                        if (!anotherBarcodeWithin2Seconds)
                                        {
                                            this.Light(Constants.LightNR);
                                            AllBarcodeViewModel nrAllVM = new AllBarcodeViewModel(
                                                barcode,
                                                false, false, oldCount + 1);
                                            this.ObsAllBarcodes.Add(nrAllVM);
                                            Debug.WriteLine($"*****timer{barcode}*********");
                                            this.RaisePropertyChanged(nameof(GradeNoCount));
                                            this.LastAllBarcode = nrAllVM;
                                            BarcodeHistory.Instance.AppendBarcode(nrAllVM);
                                            this.IsRetry = false;
                                            this.NRTimes = 0;
                                        }
                                    }));
                                },
                                null,
                                TimeSpan.FromSeconds(2), true);
                        }
                        return;
                    }//NR
                    else if (this.ObsAllBarcodes.Any(x => x.Barcode == barcode))
                    {
                        if (barcode == this.ObsAllBarcodes[oldCount - 1].Barcode)
                        {
                            this.IsRetry = true;
                            this.NRTimes = 0;
                            return;
                        }
                        else
                        {
                            dup = true;
                        }
                    }
                    if (dup)
                    {
                        this.PlaySound(Constants.sndNR);
                        this.Light(Constants.LightDup);
                    }
                    else
                    {
                        this.PlaySound(Constants.sndDup);
                        this.Light(Constants.LightOK);
                    }
                    AllBarcodeViewModel newAllVM = new AllBarcodeViewModel(barcode, true, dup, oldCount + 1);
                    this.ObsAllBarcodes.Add(newAllVM);
                    Debug.WriteLine($"*****main{barcode}*********");
                    this.RaisePropertyChanged(nameof(GradeYesCount));
                    this.RaisePropertyChanged(nameof(DupCount));
                    this.LastAllBarcode = newAllVM;
                    BarcodeHistory.Instance.AppendBarcode(newAllVM);
                    this.IsRetry = false;
                    this.NRTimes = 0;
                }));
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
            this.ExportDir = Settings.Default.ExportDir;
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

        public override void Cleanup()
        {
            this.serialPort.Close();
            base.Cleanup();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.serialPort.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}