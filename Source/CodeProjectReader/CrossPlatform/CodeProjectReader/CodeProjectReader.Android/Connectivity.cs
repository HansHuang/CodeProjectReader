using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using CodeProjectReader;
using CodeProjectReader.Droid;
using CodeProjectReader.Model;
using Java.Net;
using Xamarin.Forms;

[assembly: Dependency(typeof(Connectivity))]
namespace CodeProjectReader.Droid
{
    public class Connectivity : IConnectivity
    {
        private ConnectivityManager _connectivityManager;
        protected ConnectivityManager ConnectivityManager
        {
            get
            {
                _connectivityManager = _connectivityManager ?? (_connectivityManager = (ConnectivityManager)
                    Forms.Context.GetSystemService(Context.ConnectivityService));
                return _connectivityManager ;
            }
        }

        private WifiManager _wifiManager;
        protected WifiManager WifiManager
        {
            get
            {
                _wifiManager = _wifiManager ??
                               (WifiManager)Forms.Context.GetSystemService(Context.WifiService);
                return _wifiManager;
            }
        }

        public bool IsConnected
        {
            get
            {
                try
                {
                    var activeConnection = ConnectivityManager.ActiveNetworkInfo;

                    return (activeConnection != null) && activeConnection.IsConnected;
                }
                catch (Exception e)
                {
                    return true;
                }
            }
        }

        public async Task<bool> IsPingReachable(string host, int msTimeout = 5000)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("host");

            if (!IsConnected)
                return false;

            return await Task.Run(() =>
            {
                bool reachable;
                try
                {
                    reachable = InetAddress.GetByName(host).IsReachable(msTimeout);
                }
                catch (Exception)
                {
                    reachable = false;
                }
                return reachable;
            });
        }

        public async Task<bool> IsPortReachable(string host, int port = 80, int msTimeout = 5000)
        {
            return await Task.Run(async () =>
            {
                var sockaddr = new InetSocketAddress(host, port);
                using (var sock = new Socket())
                {
                    try
                    {
                        await sock.ConnectAsync(sockaddr, msTimeout);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            });
        }

        public IEnumerable<ConnectionType> ConnectionTypes
        {
            get
            {
                ConnectionType type;
                var activeConnection = ConnectivityManager.ActiveNetworkInfo;
                switch (activeConnection.Type)
                {
                    case ConnectivityType.Wimax:
                        type = ConnectionType.Wimax;
                        break;
                    case ConnectivityType.Wifi:
                        type = ConnectionType.WiFi;
                        break;
                    default:
                        type = ConnectionType.Cellular;
                        break;
                }
                yield return type;
            }
        }

        public IEnumerable<int> Bandwidths
        {
            get
            {
                if (ConnectionTypes.FirstOrDefault() == ConnectionType.WiFi)
                    yield return WifiManager.ConnectionInfo.LinkSpeed;
            }
        }

    }
}
