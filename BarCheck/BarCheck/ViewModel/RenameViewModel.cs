﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarCheck.ViewModel
{
    public class RenameViewModel : ViewModelBase, IDataErrorInfo
    {
        public RenameViewModel()
        {
            this.InputBarcode = "";
            Console.WriteLine("value--");

        }

        private string emptyError;

        private string inputBarcode;
        public string InputBarcode
        {
            get
            {
                return this.inputBarcode;
            }
            set
            {
                if (this.inputBarcode != value)
                {
                    this.inputBarcode = value;
                    this.RaisePropertyChanged(nameof(InputBarcode));
                    Console.WriteLine(value);
                    if (string.IsNullOrWhiteSpace(value))
                        this.emptyError = "条码不能为空！";
                    else
                        this.emptyError = null;

                }
            }
        }


        string IDataErrorInfo.Error => this.emptyError;

        string IDataErrorInfo.this[string columnName] => this.emptyError;
    }
}
