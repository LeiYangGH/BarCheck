using BarCheck.Properties;
using BarCheck.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace BarCheck
{
    public class BarcodeHistory : IDisposable
    {
        private static readonly Lazy<BarcodeHistory> lazy =
            new Lazy<BarcodeHistory>(() => new BarcodeHistory());
        private StreamWriter sw;
        private string historyDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"BarCheck\BarcodeHistory");
        public string historyFileName;
        private readonly Regex regHistoryDate = new Regex(@"\d{8}_\d{4}", RegexOptions.Compiled);
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
            if (!Directory.Exists(historyDir))
                Directory.CreateDirectory(historyDir);

        }

        public void OpenHistoryFile()
        {
            if (string.IsNullOrWhiteSpace(this.historyFileName))
            {
                this.generatedExportTxtName = $"{MainViewModel.currentUserName}{DateTime.Now.ToString("yyyyMMdd_HHmm")}.txt";
                this.historyFileName = Path.Combine(historyDir, this.GeneratedExportTxtName);
            }
            this.sw = new StreamWriter(this.historyFileName, true, Encoding.UTF8);
            this.sw.AutoFlush = true;
            Log.Instance.Logger.Info($"Create/open file:{this.historyFileName}");
        }

        private string GetLastHistoryFile()
        {
            return Directory.GetFiles(this.historyDir, "*.txt")
                .Where(x => x.Contains(DateTime.Now.ToString("yyyyMMdd"))
                            && this.regHistoryDate.IsMatch(x))
                .OrderByDescending(x => new FileInfo(x).LastWriteTime)
                .FirstOrDefault();
        }

        public bool UserWantsLoadLastFile()
        {
            bool useLast = false;
            string foundHistoryFileName = this.GetLastHistoryFile();
            if (!string.IsNullOrWhiteSpace(foundHistoryFileName)
                && MessageBox.Show(Path.GetFileNameWithoutExtension(foundHistoryFileName),
                    "检测到今天上次的扫描记录，是否导入？",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                useLast = true;
                this.historyFileName = foundHistoryFileName;
            }
            return useLast;
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

        public void AppendRenameBarcode(int nrIndex, string inputBarcode)
        {
            try
            {
                this.sw.WriteLine($"{nrIndex} {inputBarcode}");
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.Error(ex.Message);
            }
        }


        public void Close()
        {
            this.sw.Close();
            Log.Instance.Logger.Info($"Closed file:{this.historyFileName}");
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
