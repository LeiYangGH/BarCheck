using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarCheck.ViewModel
{
    public class AllBarcodeViewModel : ViewModelBase
    {
        public AllBarcodeViewModel(string barcode, bool grade, int index)
        {
            this.barcode = barcode.Trim();
            this.Grade = grade;
            this.index = index;
            this.Date = DateTime.Now;
        }

        private int index;
        public int Index
        {
            get
            {
                return this.index;
            }
            set
            {
                if (this.index != value)
                {
                    this.index = value;
                    this.RaisePropertyChanged(nameof(Index));
                }
            }
        }

        private string barcode;
        public string Barcode
        {
            get
            {
                return this.barcode;
            }
            set
            {
                if (this.barcode != value)
                {
                    this.barcode = value;
                    this.RaisePropertyChanged(nameof(Barcode));
                }
            }
        }


        private bool grade;
        public bool Grade
        {
            get
            {
                return this.grade;
            }
            set
            {
                if (this.grade != value)
                {
                    this.grade = value;
                    this.RaisePropertyChanged(nameof(Grade));
                    //MainViewModel mainVM = MainWindow.Instance.DataContext as MainViewModel;
                    MainViewModel mainVM = ServiceLocator.Current.GetInstance<MainViewModel>();

                    if (!this.grade)
                    {
                        mainVM.Alarm();
                    }
                }
            }
        }



        public DateTime Date
        {
            get;
        }


    }
}
