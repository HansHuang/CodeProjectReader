using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.SystemConfiguration;
using MonoTouch.UIKit;

namespace CodeProjectReader.iOS
{
    public enum NetworkStatus
    {
        NotReachable,
        ReachableViaCarrierDataNetwork,
        ReachableViaWiFiNetwork
    }

    public static class Reachability
    {
        public static string HostName = "www.baidu.com";

        public static bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
        {
            // Is it reachable with the current network configuration?
            bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

            // Do we need a connection to reach it?
            bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0;

            // Since the network stack will automatically try to get the WAN up,
            // probe that
            if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
                noConnectionRequired = true;

            return isReachable && noConnectionRequired;
        }

        // Is the host reachable with the current network configuration
        public static bool IsHostReachable(string host)
        {
            if (string.IsNullOrEmpty(host))
                return false;

            using (var r = new NetworkReachability(host))
            {
                NetworkReachabilityFlags flags;

                if (r.TryGetFlags(out flags))
                {
                    return IsReachableWithoutRequiringConnection(flags);
                }
            }
            return false;
        }

        //
        // Raised every time there is an interesting reachable event,
        // we do not even pass the info as to what changed, and
        // we lump all three status we probe into one
        //
        public static event EventHandler ReachabilityChanged;

        static void OnChange(NetworkReachabilityFlags flags)
        {
            var h = ReachabilityChanged;
            if (h != null)
                h(null, EventArgs.Empty);
        }

        //
        // Returns true if it is possible to reach the AdHoc WiFi network
        // and optionally provides extra network reachability flags as the
        // out parameter
        //
        static NetworkReachability _adHocWiFiNetworkReachability;
        public static bool IsAdHocWiFiNetworkAvailable(out NetworkReachabilityFlags flags)
        {
            if (_adHocWiFiNetworkReachability == null)
            {
                _adHocWiFiNetworkReachability = new NetworkReachability(new IPAddress(new byte[] { 169, 254, 0, 0 }));
                _adHocWiFiNetworkReachability.SetCallback(OnChange);
                _adHocWiFiNetworkReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
            }

            if (!_adHocWiFiNetworkReachability.TryGetFlags(out flags))
                return false;

            return IsReachableWithoutRequiringConnection(flags);
        }

        static NetworkReachability _defaultRouteReachability;
        static bool IsNetworkAvailable(out NetworkReachabilityFlags flags)
        {
            if (_defaultRouteReachability == null)
            {
                _defaultRouteReachability = new NetworkReachability(new IPAddress(0));
                _defaultRouteReachability.SetCallback(OnChange);
                _defaultRouteReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
            }
            if (!_defaultRouteReachability.TryGetFlags(out flags))
                return false;
            return IsReachableWithoutRequiringConnection(flags);
        }

        static NetworkReachability _remoteHostReachability;
        public static NetworkStatus RemoteHostStatus()
        {
            NetworkReachabilityFlags flags;
            bool reachable;

            if (_remoteHostReachability == null)
            {
                _remoteHostReachability = new NetworkReachability(HostName);

                // Need to probe before we queue, or we wont get any meaningful values
                // this only happens when you create NetworkReachability from a hostname
                reachable = _remoteHostReachability.TryGetFlags(out flags);

                _remoteHostReachability.SetCallback(OnChange);
                _remoteHostReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
            }
            else
                reachable = _remoteHostReachability.TryGetFlags(out flags);

            if (!reachable)
                return NetworkStatus.NotReachable;

            if (!IsReachableWithoutRequiringConnection(flags))
                return NetworkStatus.NotReachable;

            if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
                return NetworkStatus.ReachableViaCarrierDataNetwork;

            return NetworkStatus.ReachableViaWiFiNetwork;
        }

        public static NetworkStatus InternetConnectionStatus()
        {
            NetworkReachabilityFlags flags;
            bool defaultNetworkAvailable = IsNetworkAvailable(out flags);
            if (defaultNetworkAvailable)
            {
                if ((flags & NetworkReachabilityFlags.IsDirect) != 0)
                    return NetworkStatus.NotReachable;
            }
            else if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
                return NetworkStatus.ReachableViaCarrierDataNetwork;
            else if (flags == 0)
                return NetworkStatus.NotReachable;
            return NetworkStatus.ReachableViaWiFiNetwork;
        }

        public static NetworkStatus LocalWifiConnectionStatus()
        {
            NetworkReachabilityFlags flags;
            if (IsAdHocWiFiNetworkAvailable(out flags))
            {
                if ((flags & NetworkReachabilityFlags.IsDirect) != 0)
                    return NetworkStatus.ReachableViaWiFiNetwork;
            }
            return NetworkStatus.NotReachable;
        }

    }
}