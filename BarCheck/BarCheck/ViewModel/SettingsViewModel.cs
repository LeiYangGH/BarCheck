using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarCheck.ViewModel
{
    public class SettingsViewModel : ViewModelBase
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

    }
}
