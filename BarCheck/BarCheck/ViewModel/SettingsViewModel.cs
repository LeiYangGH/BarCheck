using GalaSoft.MvvmLight;
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
    public class SettingsViewModel : ViewModelBase, IDataErrorInfo
    {
        public SettingsViewModel()
        {
            this.obsSerialPortNames = new ObservableCollection<string>(SerialPort.GetPortNames());
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

        string IDataErrorInfo.Error => this.duplicateError;
        string IDataErrorInfo.this[string columnName] => this.duplicateError;
    }
}
