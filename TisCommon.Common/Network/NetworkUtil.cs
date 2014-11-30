using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.NetworkInformation;

namespace TiS.Core.TisCommon.Network
{
    public class NetworkUtil
    {
        private static List <string> m_availableDomains;

        public static string GetIPAddress(string hostName)
        {
            string hostIPAddress;

            GetHostInfo(ref hostName, out hostIPAddress);

            return hostIPAddress;

        }

        public static string GetHostName(string IPAddress)
        {
            string hostIPAddress;
            string hostName = IPAddress;

            GetHostInfo(ref hostName, out hostIPAddress);

            return hostName;
        }

        public static bool IsValidIPAddress(string ipAddress)
        {
            IPAddress IPAddress;

            return IPAddress.TryParse(ipAddress, out IPAddress);
        }

        public static void GetHostInfo(ref string hostName, out string hostIPAddress)
        {
            hostIPAddress = String.Empty;

            IPHostEntry hostEntry = null;

            if (StringUtil.IsStringInitialized(hostName))
            {
                try
                {
                    hostEntry = Dns.GetHostEntry(hostName);
                }
                catch (Exception exc)
                {
                    Log.WriteWarning("Failed to reach host [{0}]. Details : {1}", hostName, exc.Message);
                    return;
                }

                if (hostEntry != null && hostEntry.AddressList.Length > 0)
                {
                    TryTrimHostName(hostEntry.HostName, ref hostName);

                    hostIPAddress = FilterIPAddress(hostEntry.AddressList);

                    if (StringUtil.IsStringInitialized(hostIPAddress) == false)
                    {
                        Log.WriteWarning("Theere no address for IP version 4.");
                    }
                }
            }
        }

        public static string[] AvailableDomains
        {
            get
            {
                if (m_availableDomains == null)
                {
                    m_availableDomains = new List<string>(DirectoryServicesAccess.GetAvailableDomains(false, true));

                    if (m_availableDomains.Count > 1)
                    {
                        m_availableDomains.Sort(new AvailableDomainsSorter());
                    }
                }

                return m_availableDomains.ToArray();
            }
        }

        public static bool Ping(string IPAddress)
        {
            Ping pingSender = new Ping();

            PingReply reply = pingSender.Send(IPAddress);

            if (reply.Status == IPStatus.Success)
            {
                Log.WriteInfo("Address : {0}, RoundTrip time : {1}, Time to live : {2}, Don't fragment : {3}, Buffer size : {4}",
                    reply.Address.ToString(),
                    reply.RoundtripTime,
                    reply.Options.Ttl,
                    reply.Options.DontFragment,
                    reply.Buffer.Length);

                return true;
            }
            else
            {
                Log.WriteInfo("Pinging failed. Details : {0}", reply.Status.ToString());

                return false;
            }
        }

        private class AvailableDomainsSorter : IComparer<string>
        {
            public int Compare(string availableDomain1, string availableDomain2)
            {
                string[] parts1 = availableDomain1.Split(new char[] { '.' });
                string[] parts2 = availableDomain2.Split(new char[] { '.' });

                if (parts1.Length < parts2.Length)
                {
                    return 1;
                }
                else
                {
                    if (parts1.Length > parts2.Length)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        private static void TryTrimHostName(string hostName, ref string trimmedHostName)
        {
            //Log.WriteInfo("Full host name [{0}]", hostName);

            trimmedHostName = hostName;

            foreach (string domain in AvailableDomains)
            {
                if (FindAndTrimDomainPart('.' + domain, ref trimmedHostName))
                {
                    try
                    {
                        Dns.GetHostEntry(trimmedHostName);
                        
                        //Log.WriteInfo("Trimmed host name [{0}] !", trimmedHostName);

                        return;
                    }
                    catch (Exception )
                    {
                        trimmedHostName = hostName;

                        continue;
                    }
                }
            }

            //Log.WriteInfo("Trimmed host name [{0}] ?", trimmedHostName);
        }

        private static bool FindAndTrimDomainPart(string domainName, ref string trimmedHostName)
        {
            Regex regEx = new Regex(@"(?<Domain>\" + domainName + ")$", RegexOptions.IgnoreCase);
            Match match = regEx.Match(trimmedHostName);

            if (match != null && match.Success)
            {
                trimmedHostName = trimmedHostName.ToUpper().Replace(domainName.ToUpper(), String.Empty).ToUpper();
            }

            return match.Success;
        }

        private static string FilterIPAddress(IPAddress[] addresses)
        {
            foreach (IPAddress address in addresses)
            {
                // Address for IP version 4
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return address.ToString();
                }
            }

            return String.Empty;
        }
    }
}
