using System;
using log4net.Core;
using TiS.Logger;

namespace TISAppenderLog4net
{
    public class TISLogConfiguration
    {
        internal static event OnConfigFileLocationDelegate OnConfigFileLocation;

        public string ConfigFileLocation
        {
            get
            {
                if (OnConfigFileLocation != null)
                {
                    return OnConfigFileLocation(this, new EventArgs());
                }

                return null;
            }
        }

        public string LogFileName
        {
            get
            {
                var sufix = string.Format("{0}.{1}.{2}", System.DateTime.Now.Day, System.DateTime.Now.Month, System.DateTime.Now.Year);

                return string.Format("{0}_{1}.xml", "TISLog", sufix);
            }
        }

        public string getLineFormated(LoggingEvent loggingEvent)
        {
            try
            {
                return string.Format("<log4j:event logger=\"{0}\" timestamp=\"{1}\" level=\"{2}\" thread=\"{3}\"><log4j:message>{4}</log4j:message>" +
                                          "<log4j:properties><log4j:data name=\"log4net:UserName\" value=\"{5}\" /><log4j:data name=\"log4jmachinename\" value=\"{6}\" />" +
                                          "<log4j:data name=\"log4japp\" value=\"{7}\" /><log4j:data name=\"log4net:HostName\" value=\"{8}\" />" +
                                          "<log4j:throwable><![CDATA[{11}]]></log4j:throwable> />" +
                                          "</log4j:properties><log4j:locationInfo class=\"{9}\" method=\"{10}\" ErrorType=\"{8}\" /></log4j:event>\r\n",
                                           loggingEvent.LoggerName,                                 // Logger       [0]
                                           loggingEvent.TimeStamp.ToUniversalTime().Ticks,                   // TimeStamp    [1] 
                                           loggingEvent.Level,                                      // Level        [2]
                                           loggingEvent.ThreadName,                                 // Thread       [3]
                                           loggingEvent.RenderedMessage,                            // Message      [4]
                                           loggingEvent.UserName,                                   // User name    [5]
                                           loggingEvent.ThreadName,                                 // Machine Name [6]
                                           loggingEvent.GetExceptionString(),                       // Data         [7]
                                           loggingEvent.Identity,                                   // Host         [8]
                                           loggingEvent.LocationInformation.ClassName,              // Class        [9]
                                           loggingEvent.LocationInformation.MethodName,             // Method       [10]
                                           loggingEvent.Domain.ToString()                           // Throwable    [11]                
                                        );

            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
