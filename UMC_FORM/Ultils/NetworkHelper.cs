using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace UMC_FORM.Ultils
{
    public static class NetworkHelper
    {
        public static string Ip
        {
            get
            {
                string result = string.Empty;
                bool flag = HttpContext.Current != null;
                if (flag)
                {
                    result = NetworkHelper.GetWebClientIp();
                }
                bool flag2 = result.IsEmpty();
                if (flag2)
                {
                    result = NetworkHelper.GetLanIp();
                }
                return result;
            }
        }

        public static string Host
        {
            get
            {
                return (HttpContext.Current == null) ? Dns.GetHostName() : NetworkHelper.GetWebClientHostName();
            }
        }

        public static string Browser
        {
            get
            {
                bool flag = HttpContext.Current == null;
                string result;
                if (flag)
                {
                    result = string.Empty;
                }
                else
                {
                    HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
                    result = string.Format("{0} {1}", browser.Browser, browser.Version);
                }
                return result;
            }
        }

        private static string GetWebClientIp()
        {
            string ip = NetworkHelper.GetWebRemoteIp();
            IPAddress[] hostAddresses = Dns.GetHostAddresses(ip);
            string result;
            for (int i = 0; i < hostAddresses.Length; i++)
            {
                IPAddress hostAddress = hostAddresses[i];
                bool flag = hostAddress.AddressFamily == AddressFamily.InterNetwork;
                if (flag)
                {
                    result = hostAddress.ToString();
                    return result;
                }
            }
            result = string.Empty;
            return result;
        }

        private static string GetWebRemoteIp()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        private static string GetLanIp()
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            string result;
            for (int i = 0; i < hostAddresses.Length; i++)
            {
                IPAddress hostAddress = hostAddresses[i];
                bool flag = hostAddress.AddressFamily == AddressFamily.InterNetwork;
                if (flag)
                {
                    result = hostAddress.ToString();
                    return result;
                }
            }
            result = string.Empty;
            return result;
        }

        private static string GetWebClientHostName()
        {
            bool flag = !HttpContext.Current.Request.IsLocal;
            string result2;
            if (flag)
            {
                result2 = string.Empty;
            }
            else
            {
                string ip = NetworkHelper.GetWebRemoteIp();
                string result = Dns.GetHostEntry(IPAddress.Parse(ip)).HostName;
                bool flag2 = result == "localhost.localdomain";
                if (flag2)
                {
                    result = Dns.GetHostName();
                }
                result2 = result;
            }
            return result2;
        }
    }
}
