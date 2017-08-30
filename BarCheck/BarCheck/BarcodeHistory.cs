using BarCheck.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarCheck
{
    public class BarcodeHistory : IDisposable
    {
        private static readonly Lazy<BarcodeHistory> lazy =
            new Lazy<BarcodeHistory>(() => new BarcodeHistory());
        private StreamWriter sw;
        private string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"BarCheck\BarcodeHistory");
        private string fileName;
        public static BarcodeHistory Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private string generatedExportTxtName;
        public string GeneratedExportTxtName
        {
            get
            {
                return this.generatedExportTxtName;
            }
        }

        private BarcodeHistory()
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            this.generatedExportTxtName = $"{MainViewModel.currentUserName}{DateTime.Now.ToString("yyyyMMdd_HHmm")}.txt";
            this.fileName = Path.Combine(dir, this.GeneratedExportTxtName);
            this.sw = new StreamWriter(this.fileName);
            this.sw.AutoFlush = true;
            Log.Instance.Logger.Info($"Created file:{this.fileName}");
        }

        public void AppendBarcode(AllBarcodeViewModel allBarcodeVM)
        {
            try
            {
                this.sw.WriteLine(allBarcodeVM);
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.Error(ex.Message);
            }
        }

        public void Close()
        {
            this.sw.Close();
            Log.Instance.Logger.Info($"Closed file:{this.fileName}");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.sw != null)
                    this.sw.Dispose();
            }
        }
        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
