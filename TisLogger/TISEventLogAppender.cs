using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using log4net.Appender;
using log4net.Util;
using log4net.Core;

namespace TISAppenderLog4net
{
    #region TISEventLogAppender

    public class TISEventLogAppender : EventLogAppender
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            //
            // Write the resulting string to the event log system
            //
            int eventID = 0;

            // Look for the EventLogEventID property
            object eventIDPropertyObj = loggingEvent.LookupProperty("EventID");
            if (eventIDPropertyObj != null)
            {
                if (eventIDPropertyObj is int)
                {
                    eventID = (int)eventIDPropertyObj;
                }
                else
                {
                    string eventIDPropertyString = eventIDPropertyObj as string;
                    if (eventIDPropertyString != null && eventIDPropertyString.Length > 0)
                    {
                        // Read the string property into a number
                        int intVal;
                        if (SystemInfo.TryParse(eventIDPropertyString, out intVal))
                        {
                            eventID = intVal;
                        }
                        else
                        {
                            ErrorHandler.Error("Unable to parse event ID property [" + eventIDPropertyString + "].");
                        }
                    }
                }
            }

            // Write to the event log
            try
            {
                string eventTxt = RenderLoggingEvent(loggingEvent);

                // There is a limit of 32K characters for an event log message
                if (eventTxt.Length > 32000)
                {
                    eventTxt = eventTxt.Substring(0, 32000);
                }

                EventLogEntryType entryType = GetEntryType(loggingEvent.Level);

                using (SecurityContext.Impersonate(this))
                {
                    EventLogPermission eventLogPermission = new EventLogPermission(EventLogPermissionAccess.Administer, ".");

                    eventLogPermission.PermitOnly();

                    EventLog.WriteEntry(ApplicationName, eventTxt, entryType, eventID);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Unable to write to event log [" + LogName + "] using source [" + ApplicationName + "]", ex);
            }
        }
    }

    #endregion
}
