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
            this.obsGrades = new ObservableCollection<string>()
            {
                "C",
                "D",
                "E"
            };
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

        private string alarmGrade;
        public string AlarmGrade
        {
            get
            {
                return this.alarmGrade;
            }
            set
            {
                if (this.alarmGrade != value)
                {
                    this.alarmGrade = value;
                    this.RaisePropertyChanged(nameof(AlarmGrade));
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

        private bool closeBeforeAlarm;
        public bool CloseBeforeAlarm
        {
            get
            {
                return this.closeBeforeAlarm;
            }
            set
            {
                if (this.closeBeforeAlarm != value)
                {
                    this.closeBeforeAlarm = value;
                    this.RaisePropertyChanged(nameof(CloseBeforeAlarm));
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

        private ObservableCollection<string> obsGrades;
        public ObservableCollection<string> ObsGrades
        {
            get
            {
                return this.obsGrades;
            }
            set
            {
                if (this.obsGrades != value)
                {
                    this.obsGrades = value;
                    this.RaisePropertyChanged(nameof(ObsGrades));
                }
            }
        }

        string IDataErrorInfo.Error => this.duplicateError;

        string IDataErrorInfo.this[string columnName] => this.duplicateError;
    }
}
