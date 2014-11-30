using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics; 
using System.ComponentModel; 
using System.Configuration.Install;
using TiS.Logger;

namespace EventLogSourceInstaller
{
    public abstract class TISEventLogInstaller : Installer
    {
        protected EventLogInstaller TisEventLogInstaller;
        public TISEventLogInstaller()
        {
            //Create Instance of EventLogInstaller 
            TisEventLogInstaller = new EventLogInstaller();

            // Set the Log that source is created in 
            TisEventLogInstaller.Log = Log.EVENT_LOG_NAME;

            // Add myEventLogInstaller to the Installers Collection. 
            Installers.Add(TisEventLogInstaller);
        }
    }

    [RunInstaller(true)]
    public class TISClientEventLogInstaller : TISEventLogInstaller
    {
        public TISClientEventLogInstaller()
        { 
            // Set the Source of Event Log, to be created. 
            TisEventLogInstaller.Source = Log.EVENT_LOG_SOURCE_CLIENT; 
        } 
    }

    [RunInstaller(true)]
    public class TISServerEventLogInstaller : TISEventLogInstaller
    {
        public TISServerEventLogInstaller()
        {
            // Set the Source of Event Log, to be created. 
            TisEventLogInstaller.Source = Log.EVENT_LOG_SOURCE_SERVER;
        }
    }

    [RunInstaller(true)]
    public class TISSTSEventLogInstaller : TISEventLogInstaller
    {
        public TISSTSEventLogInstaller()
        {
            // Set the Source of Event Log, to be created. 
            TisEventLogInstaller.Source = Log.EVENT_LOG_SOURCE_STS;
        }
    }
}



  