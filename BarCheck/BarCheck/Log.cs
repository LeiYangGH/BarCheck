using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarCheck
{
    public class Log
    {
        private static readonly Lazy<Log> lazy =
        new Lazy<Log>(() => new Log());

        public static Log Instance { get { return lazy.Value; } }
        public ILog Logger;
        private Log()
        {
            this.InitLog4Net();
            this.Logger = LogManager.GetLogger(typeof(Log));
        }
        private void InitLog4Net()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }
    }
}
