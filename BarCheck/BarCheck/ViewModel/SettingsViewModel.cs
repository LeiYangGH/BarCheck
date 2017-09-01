using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarCheck.ViewModel
{
    public sealed class SettingsViewModel : ViewModelBase, IDataErrorInfo
    {
        public SettingsViewModel()
        {
            this.obsSerialPortNames = new ObservableCollection<string>(SerialPort.GetPortNames());
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
