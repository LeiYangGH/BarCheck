using BarCheck.Properties;
using BarCheck.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public IList<string> last5DaysHistoryFiles = new List<string>();
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
            Log.Instance.Logger.Info($"Create file:{this.historyFileName}");
        }

        private DateTime GetDateOfHistroyFile(string histroyFileName)
        {
            IFormatProvider ifp = new CultureInfo("en-us", true);
            string dateString = this.regHistoryDate.Match(histroyFileName).Value;
            return DateTime.ParseExact(dateString, "yyyyMMdd_HHmm", ifp);
        }


        private IList<string> GetLast5DaysHistoryFile()
        {
            return Directory.GetFiles(this.historyDir, "*.txt")
                .Where(x => this.regHistoryDate.IsMatch(x))
                .Where(x => GetDateOfHistroyFile(x) > DateTime.Now.AddDays(-5))
                .OrderBy(x => GetDateOfHistroyFile(x))
                .ToList();
        }

        private string CombineFileNames(IList<string> foundHistoryFileNames)
        {
            return string.Join(Environment.NewLine,
                foundHistoryFileNames.Select(f => Path.GetFileNameWithoutExtension(f)));
        }


        public bool UserWantsImportHistoryFiles()
        {
            bool useLast = false;
            this.last5DaysHistoryFiles = this.GetLast5DaysHistoryFile();
            if ((this.last5DaysHistoryFiles != null) && this.last5DaysHistoryFiles.Count() > 0
                && MessageBox.Show(this.CombineFileNames(this.last5DaysHistoryFiles),
                    "检测到最近5天的扫描记录，是否导入？",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                useLast = true;
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
            Log.Instance.Logger.Info($"Closed file:{this.historyFileName}");
            try
            {
                if (this.sw != null)
                    this.sw.Close();
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.Error(ex.Message);
            }
            Log.Instance.Logger.Info($"Closed file:{this.historyFileName}");
        }

        public void Delete()
        {
            try
            {
                File.Delete(this.historyFileName);
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.Error(ex.Message);
            }
            Log.Instance.Logger.Info($"deleted empty file:{this.historyFileName}");
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
