using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Model;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CodeProjectReader.iOS
{
    public class Connectivity : IConnectivity
    {
        public Connectivity()
        {
            Reachability.ReachabilityChanged += (sender, args) =>
            {
                var remoteHostStatus = Reachability.RemoteHostStatus();
                var internetStatus = Reachability.InternetConnectionStatus();
                var localWifiStatus = Reachability.LocalWifiConnectionStatus();
                IsConnected = (internetStatus == NetworkStatus.ReachableViaCarrierDataNetwork ||
                               internetStatus == NetworkStatus.ReachableViaWiFiNetwork) ||
                              (localWifiStatus == NetworkStatus.ReachableViaCarrierDataNetwork ||
                               localWifiStatus == NetworkStatus.ReachableViaWiFiNetwork) ||
                              (remoteHostStatus == NetworkStatus.ReachableViaCarrierDataNetwork ||
                               remoteHostStatus == NetworkStatus.ReachableViaWiFiNetwork);
            };
        }


        public bool IsConnected { get; private set; }

        public async Task<bool> IsPingReachable(string host, int msTimeout = 5000)
        {
            return await Task.Run(() => true);
        }

        public Task<bool> IsPortReachable(string host, int port = 80, int msTimeout = 5000)
        {
            return new Task<bool>(() => Reachability.IsHostReachable(host));
        }

        public IEnumerable<ConnectionType> ConnectionTypes { get; private set; }
        public IEnumerable<int> Bandwidths { get; private set; }
    }
}