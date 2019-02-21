using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarCheck.ViewModel
{
    public sealed class SettingsViewModel : ViewModelBase, IDataErrorInfo
    {
        private string barcodeFormatsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"BarCheck\BarcodeFormats.txt");
        public SettingsViewModel()
        {
            this.obsSerialPortNames = new ObservableCollection<string>(SerialPort.GetPortNames());
            this.ReadBarcodeFormatsFile();
            this.ObsBarcodeFormats = new ObservableCollection<string>(SettingsViewModel.dictBarcodeFormats.Keys);
        }



        public static Dictionary<string, string> dictBarcodeFormats = new Dictionary<string, string>();
        private void ReadBarcodeFormatsFile()
        {
            if (!File.Exists(this.barcodeFormatsFile))
                return;
            foreach (string line in File.ReadLines(this.barcodeFormatsFile))
            {
                string[] parts = line.Trim().Split(new char[] { ':', '：' },
                    StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                    SettingsViewModel.dictBarcodeFormats.Add(parts[0].Trim(), parts[1].Trim());
                else
                    Log.Instance.Logger.Error($"条码格式文件记录行格式错误: {line}");
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

        private string selectedPortName;
        public string SelectedPortName
        {
            get
            {
                return this.selectedPortName;
            }
            set
            {
                if (this.selectedPortName != value)
                {
                    this.selectedPortName = value;
                    this.RaisePropertyChanged(nameof(SelectedPortName));
                }
            }
        }

        private string selectedBarcodeFormat;
        public string SelectedBarcodeFormat
        {
            get
            {
                return this.selectedBarcodeFormat;
            }
            set
            {
                if (this.selectedBarcodeFormat != value)
                {
                    this.selectedBarcodeFormat = value;
                    this.RaisePropertyChanged(nameof(SelectedBarcodeFormat));
                }
            }
        }


        private int alarmMs;
        public int AlarmMs
        {
            get
            {
                return this.alarmMs;
            }
            set
            {
                if (this.alarmMs != value)
                {
                    this.alarmMs = value;
                    this.RaisePropertyChanged(nameof(AlarmMs));
                }
            }
        }

        private int nRMaxCount;
        public int NRMaxCount
        {
            get
            {
                return this.nRMaxCount;
            }
            set
            {
                if (this.nRMaxCount != value)
                {
                    this.nRMaxCount = value;
                    this.RaisePropertyChanged(nameof(NRMaxCount));
                }
            }
        }

        private int nRIgnoreTime;
        public int NRIgnoreTime
        {
            get
            {
                return this.nRIgnoreTime;
            }
            set
            {
                if (this.nRIgnoreTime != value)
                {
                    this.nRIgnoreTime = value;
                    this.RaisePropertyChanged(nameof(NRIgnoreTime));
                }
            }
        }

        private string duplicateError;
        private string selectedAPortName;
        public string SelectedAPortName
        {
            get
            {
                return this.selectedAPortName;
            }
            set
            {
                if (this.selectedAPortName != value)
                {
                    this.selectedAPortName = value;
                    this.RaisePropertyChanged(nameof(SelectedAPortName));
                    if (value == this.SelectedPortName)
                        this.duplicateError = "报警串口不能和条码扫描串口相同！";
                    else
                        this.duplicateError = null;
                }
            }
        }

        private ObservableCollection<string> obsSerialPortNames;
        public ObservableCollection<string> ObsSerialPortNames
        {
            get
            {
                return this.obsSerialPortNames;
            }
            set
            {
                if (this.obsSerialPortNames != value)
                {
                    this.obsSerialPortNames = value;
                    this.RaisePropertyChanged(nameof(ObsSerialPortNames));
                }
            }
        }

        private ObservableCollection<string> obsBarcodeFormats;
        public ObservableCollection<string> ObsBarcodeFormats
        {
            get
            {
                return this.obsBarcodeFormats;
            }
            set
            {
                if (this.obsBarcodeFormats != value)
                {
                    this.obsBarcodeFormats = value;
                    this.RaisePropertyChanged(nameof(ObsBarcodeFormats));
                }
            }
        }

        private bool isBrowseing;
        private RelayCommand browseCommand;

        public RelayCommand BrowseCommand
        {
            get
            {
                return browseCommand
                  ?? (browseCommand = new RelayCommand(
                    async () =>
                    {
                        if (isBrowseing)
                        {
                            return;
                        }

                        isBrowseing = true;
                        BrowseCommand.RaiseCanExecuteChanged();

                        await Browse();

                        isBrowseing = false;
                        BrowseCommand.RaiseCanExecuteChanged();
                    },
                    () => !isBrowseing));
            }
        }

        private async Task Browse()
        {
            await Task.Run(() =>
            {
            });
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ExportDir = dlg.SelectedPath;
            }
        }


        string IDataErrorInfo.Error => this.duplicateError;
        string IDataErrorInfo.this[string columnName] => this.duplicateError;
    }
}
