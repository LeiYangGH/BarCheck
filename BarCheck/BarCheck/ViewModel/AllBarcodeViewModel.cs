using BarCheck.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace BarCheck.ViewModel
{
    public sealed class AllBarcodeViewModel : ViewModelBase
    {
        public AllBarcodeViewModel(string barcode, bool valid, bool dup, int index)
        {
            this.barcode = barcode.Trim();
            this.Valid = valid;
            this.HasDup = dup;
            this.index = index;
            this.Date = DateTime.Now;
            this.IsImportedFromHistory = false;

            if (!this.Valid)
                this.Status = BarcodeStatus.NO; //
            else if (this.HasDup)
                this.Status = BarcodeStatus.Dup; //
            else
                this.Status = BarcodeStatus.Yes; //
        }

        /// <summary>
        /// only for import
        /// </summary>
        /// <param name="index"></param>
        /// <param name="barcode"></param>
        /// <param name="status"></param>
        /// <param name="date"></param>
        public AllBarcodeViewModel(string index, string barcode, string status, string date)
        {
            this.index = Convert.ToInt32(index);
            this.barcode = barcode;
            IFormatProvider ifp = new CultureInfo("en-us", true);
            if (status == "不合格")
            {
                this.Status = BarcodeStatus.NO; //
                this.Valid = false;
                this.HasDup = false;
            }
            else if (status == "重复")
            {
                this.Status = BarcodeStatus.Dup; //
                this.Valid = true;
                this.HasDup = true;
            }
            else
            {
                this.Status = BarcodeStatus.Yes; //
                this.Valid = true;
                this.HasDup = false;
            }
            this.Date = DateTime.ParseExact(date, "yyyyMMdd:HHmmss", ifp);
            this.IsImportedFromHistory = true;
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

        public bool IsImportedFromHistory
        {
            get;
            private set;
        }

        private bool valid;
        public bool Valid
        {
            get
            {
                return this.valid;
            }
            set
            {
                if (this.valid != value)
                {
                    this.valid = value;
                    this.RaisePropertyChanged(nameof(Valid));
                }
            }
        }

        public BarcodeStatus status;
        public BarcodeStatus Status
        {
            get
            {
                return this.status;
            }
            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    this.RaisePropertyChanged(nameof(Status));
                }
            }
        }

        public DateTime Date
        {
            get;
        }

        private bool hasDup;
        public bool HasDup
        {
            get
            {
                return this.hasDup;
            }
            set
            {
                if (this.hasDup != value)
                {
                    this.hasDup = value;
                    this.RaisePropertyChanged(nameof(HasDup));
                }
            }
        }

        private bool isRenameing;
        private RelayCommand renameCommand;

        public RelayCommand RenameCommand
        {
            get
            {
                return renameCommand
                  ?? (renameCommand = new RelayCommand(
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
            await Task.Run(() =>
            {
            });
            Log.Instance.Logger.Info("Rename");

            RenameWindow reWin = new RenameWindow();
            reWin.Owner = Application.Current.MainWindow;
            RenameViewModel setVM = (reWin.DataContext) as RenameViewModel;

            if (reWin.ShowDialog() ?? false)
            {
                this.Barcode = setVM.InputBarcode;
                BarcodeHistory.Instance.AppendRenameBarcode(this.Index, setVM.InputBarcode);
            }
        }

        public override string ToString()
        {
            return $"{Index} {Barcode} { Constants.dicStatusDesc[this.Status]} {Date.ToString("yyyyMMdd:HHmmss")}";
        }

        public static AllBarcodeViewModel ConvertFromLine(string[] parts)
        {
            Debug.Assert(parts.Length == 4);
            AllBarcodeViewModel allVM = new AllBarcodeViewModel(parts[0], parts[1], parts[2], parts[3]);
            return allVM;
        }
    }
}
