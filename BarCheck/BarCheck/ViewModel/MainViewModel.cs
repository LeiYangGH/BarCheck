using BarCheck.Properties;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarCheck.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private SerialPort serialPort = new SerialPort(Constants.ComUnknown, 9600);
        private SerialPort aserialPort = new SerialPort(Constants.ComUnknown, 9600);

        public MainViewModel()
        {
            this.serialPort.DataReceived += SerialPort_DataReceived;

#if Test
            this.PortName = "COM10";
#else
            this.PortName = this.GetFirstPortName();
            this.APortName = this.GetSecondPortName();
            this.GetAlarmSettings();
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

        private int currentBestGrade;
        public int CurrentBestGrade
        {
            get
            {
                return this.currentBestGrade;
            }
            set
            {
                if (this.currentBestGrade != value)
                {
                    this.currentBestGrade = value;
                    this.RaisePropertyChanged(nameof(CurrentBestGrade));
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

        private AllBarcodeViewModel selectedAllBarcode;
        public AllBarcodeViewModel SelectedAllBarcode
        {
            get
            {
                return this.selectedAllBarcode;
            }
            set
            {
                if (this.selectedAllBarcode != value)
                {
                    this.selectedAllBarcode = value;
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

            if (setWin.ShowDialog() ?? false)
            {
                this.PortName = setVM.SelectedPortName;
                this.APortName = setVM.SelectedAPortName;
                this.RaisePropertyChanged(nameof(IsOpened));
                this.alarmMs = setVM.AlarmMs;
                Log.Instance.Logger.Info($"Port={PortName} APort={APortName} alarmMs={alarmMs}");
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
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Text (*.txt)|*.txt";
            if (dlg.ShowDialog() ?? false)
            {
                this.ExportAllBarocdeTxt(dlg.FileName);
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

        public void Alarm(byte[] bytes)
        {
            latestG = Guid.NewGuid();
            Thread t = new Thread(() => AlarmFun(latestG, bytes));
            t.IsBackground = true;
            closeTime = DateTime.Now.AddMilliseconds(alarmMs);
            t.Start();
        }
        public void AlarmFun(Guid g, byte[] bytes)
        {
            this.SendBytes(bytes);
            while (DateTime.Now < closeTime)
            {
                if (g != latestG)
                    return;
                Thread.Sleep(50);
            }
            this.SendBytes(Constants.AlarmClose);
        }

        public void CheckDupAfterRename(AllBarcodeViewModel allVM)
        {
            var dups = this.ObsAllBarcodes.Where(x => x.Barcode == allVM.Barcode);
            if (dups.Count() > 1)
            {
                this.Alarm(Constants.Alarm2LightBytes);
                foreach (var avm in dups)
                    avm.HasDup = true;
            }

        }

        private void GotBarcode(string barcode)
        {
            if (App.Current != null)//walkaround
                App.Current.Dispatcher.BeginInvoke((Action)(delegate
                {
                    barcode = barcode.Trim();
                    AllBarcodeViewModel newAllVM = null;
                    int oldCount = this.ObsAllBarcodes.Count;

                    if (barcode.ToUpper() == Constants.NR)
                    {
                        barcode = Constants.NR + DateTime.Now.ToString("ddmmssfff");
                        newAllVM = new AllBarcodeViewModel(barcode, false, oldCount + 1);
                        this.Alarm(Constants.Alarm1LightBytes);
                    }
                    else
                        newAllVM = new AllBarcodeViewModel(barcode, true, oldCount + 1);
                    this.ObsAllBarcodes.Add(newAllVM);
                    bool hasDup = false;
                    for (int a = 0; a < oldCount; a++)
                    {
                        AllBarcodeViewModel aVM = this.ObsAllBarcodes[a];
                        if (aVM.Barcode == barcode)
                        {
                            hasDup = true;
                            aVM.HasDup = true;
                        }
                    }
                    if (hasDup)
                    {
                        newAllVM.HasDup = true;
                        this.Alarm(Constants.Alarm2LightBytes);
                    }
                    BarcodeHistory.Instance.AppendBarcode(newAllVM);
                }));
        }

        private Random rnd = new Random();
        public void AddRandomBarcode()
        {
            string barcode = rnd.Next(10000, 10010).ToString().PadLeft(12, '0');
            if (rnd.Next(1, 3) == 2)
                this.GotBarcode(Constants.NR);
            this.GotBarcode(barcode);
        }

        private void GetAlarmSettings()
        {
            this.alarmMs = Settings.Default.AlarmMs;
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