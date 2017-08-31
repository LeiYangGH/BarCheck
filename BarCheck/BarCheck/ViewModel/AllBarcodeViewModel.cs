using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (!this.Valid)
                this.Status = BarcodeStatus.NO; //
            else if (this.HasDup)
                this.Status = BarcodeStatus.Dup; //
            else
                this.Status = BarcodeStatus.Yes; //
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
        public bool Valid
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
                Log.Instance.Logger.Info("Rename");

                RenameWindow reWin = new RenameWindow();
                reWin.Owner = MainWindow.Instance;
                RenameViewModel setVM = (reWin.DataContext) as RenameViewModel;

                if (reWin.ShowDialog() ?? false)
                {
                    this.Barcode = setVM.InputBarcode;
                }
            });
        }

        public override string ToString()
        {
            return $"{Index} {Barcode} { Constants.dicStatusDesc[this.Status]} {Date.ToString("yyyyMMdd:HHmmss")}";
        }
    }
}
