using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarCheck.ViewModel
{
    public class AllBarcodeViewModel : ViewModelBase
    {
        public AllBarcodeViewModel(string barcodeWithGrade, int index)
        {
            this.barcode = barcodeWithGrade.Substring(0, barcodeWithGrade.Length - 1);
            this.Grade = Convert.ToInt32(barcodeWithGrade.Substring(barcodeWithGrade.Length - 2, 1));
            this.index = index;
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


        private int grade;
        public int Grade
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
                }
            }
        }


    }
}
