using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Diagnostics;
using BarCheck.Properties;
using System.Collections.Generic;
using System.Threading;

namespace BarCheck.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        //private SerialPort serialPort = new SerialPort("COM?", 9600);
        private SerialPort serialPort = new SerialPort("COM?", 9600);//115200
        private SerialPort aserialPort = new SerialPort("COM?", 9600);

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

        private string hardwareBarcode;
        public string HardwareBarcode
        {
            get
            {
                return this.hardwareBarcode;
            }
            set
            {
                if (this.hardwareBarcode != value)
                {
                    this.hardwareBarcode = value;
                    this.RaisePropertyChanged(nameof(HardwareBarcode));
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
                this.Message = string.Format("成功打开报警串口{0}！", this.APortName);
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
            //App.Current.Dispatcher.BeginInvoke((Action)(delegate
            //{
            this.CloseSerialPort();
            this.RaisePropertyChanged(nameof(IsOpened));
            //}));
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
                this.Message = string.Format("成功关闭报警串口{0}！", this.APortName);

            }
            catch (Exception ex)
            {
                Log.Instance.Logger.FatalFormat(ex.Message);
                MessengerInstance.Send<string>(ex.Message);
            }
        }
        //1
        private bool isRenameing;
        private RelayCommand manualAddCommand;

        public RelayCommand RenameCommand
        {
            get
            {
                return manualAddCommand
                  ?? (manualAddCommand = new RelayCommand(
                    async () =>
                    {
                        if (isRenameing)
                        {
                            return;
                        }

                        isRenameing = true;
                        RenameCommand.RaiseCanExecuteChanged();

                        await Rename();

                        isRenameing = false;
                        RenameCommand.RaiseCanExecuteChanged();
                    },
                    () => !isRenameing));
            }
        }
        private async Task Rename()
        {
            Log.Instance.Logger.Info($"Manual add {this.HardwareBarcode}");
            this.GotBarcode(this.HardwareBarcode);
        }
        //2

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
            Log.Instance.Logger.Info("settings");

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



        private bool ExportAllBarocdeTxt(string tabTxtFileName)
        {
            try
            {
                //string dir = Path.GetDirectoryName(tabTxtFileName);
                //var allNotDeletedBarcodes = this.ObsAllBarcodes.Where(x => !x.Deleted).Select(x => x.Barcode).ToArray();
                //string allFileName = Path.Combine(dir, "所有条码" + allNotDeletedBarcodes.Length.ToString() + ".txt");
                //File.WriteAllLines(allFileName, allNotDeletedBarcodes);
                //Log.Instance.Logger.Info(allFileName);
                //this.Message = allFileName;
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
                this.ExportTabTxt(dlg.FileName);
            }
        }

        private bool isAdd10King;
        private RelayCommand add10KCommand;

        public RelayCommand Add10KCommand
        {
            get
            {
                return add10KCommand
                  ?? (add10KCommand = new RelayCommand(
                    async () =>
                    {
                        if (isAdd10King)
                        {
                            return;
                        }

                        isAdd10King = true;
                        Add10KCommand.RaiseCanExecuteChanged();

                        await ImportBarcodesFromFile();

                        isAdd10King = false;
                        Add10KCommand.RaiseCanExecuteChanged();
                    },
                    () => !isAdd10King));
            }
        }

        private async Task ImportBarcodesFromFile()
        {
            this.ObsAllBarcodes.Clear();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text (*.txt)|*.txt";
            if (dlg.ShowDialog() ?? false)
            {
                this.Message = "开始导入" + dlg.FileName;
                string fileName = dlg.FileName;
                foreach (string line in File.ReadLines(fileName))
                {
                    this.GotBarcode(line.Replace("\r", "").Replace("\n", ""));
                }
                this.Message = "结束导入" + dlg.FileName;
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

        public void SendString(string s)
        {
            try
            {
                byte[] bytes = StringToByteArray(s);
                this.aserialPort.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.Error(ex.Message);
            }
        }
        private Guid latestG;
        private DateTime closeTime;

        public void Alarm()
        {
            latestG = Guid.NewGuid();
            Thread t = new Thread(() => AlarmFun(latestG));
            t.IsBackground = true;
            closeTime = DateTime.Now.AddMilliseconds(alarmMs);
            t.Start();
        }
        public void AlarmFun(Guid g)
        {
            this.SendString("0110001A000101CE18");
            while (DateTime.Now < closeTime)
            {
                if (g != latestG)
                    return;
                Thread.Sleep(50);
            }
            this.SendString("0110001A0001000FD8");
        }

        private void GotBarcode(string barcode)
        {
            if (App.Current != null)//walkaround
                App.Current.Dispatcher.BeginInvoke((Action)(delegate
                {
                    if (!isRenameing)
                        this.HardwareBarcode = barcode;
                    barcode = barcode.Trim();
                    AllBarcodeViewModel newAllVM = null;
                    int oldCount = this.ObsAllBarcodes.Count;

                    if (barcode.ToUpper() == Constants.NR)
                    {
                        barcode = Constants.NR + DateTime.Now.ToString("ddmmssfff");
                        newAllVM = new AllBarcodeViewModel(barcode, false, oldCount + 1);
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
                        newAllVM.HasDup = true;
                    BarcodeHistory.Instance.AppendBarcode(newAllVM);

                }));
        }

        private Random rnd = new Random();
        public void AddRandomBarcode()
        {
            string barcode = rnd.Next(10000, 10010).ToString().PadLeft(5, '0');
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
                return "COM?";
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
                        return "COM?";
                }
            }
            else
                return "COM?";
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