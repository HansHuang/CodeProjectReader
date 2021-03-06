﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeProjectReader.Model;
using CodeProjectReader.WinPhone;
using Microsoft.Phone.Net.NetworkInformation;
using Xamarin.Forms;

[assembly:Dependency(typeof(Connectivity))]
namespace CodeProjectReader.WinPhone
{
    public class Connectivity : IConnectivity
    {
        public bool IsConnected
        {
            get { return DeviceNetworkInformation.IsNetworkAvailable; }
        }

        public async Task<bool> IsPingReachable(string host, int msTimeout = 5000)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("host");

            return await Task.Run(() =>
            {
                var manualResetEvent = new ManualResetEvent(false);
                var reachable = false;
                DeviceNetworkInformation.ResolveHostNameAsync(new DnsEndPoint(host, 80), result =>
                {
                    reachable = result.NetworkInterface != null;
                    manualResetEvent.Set();
                }, null);
                manualResetEvent.WaitOne(TimeSpan.FromMilliseconds(msTimeout));
                return reachable;
            });
        }

        public async Task<bool> IsPortReachable(string host, int port = 80, int msTimeout = 5000)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("host");

            return await Task.Run(() =>
            {
                var clientDone = new ManualResetEvent(false);
                var reachable = false;
                var hostEntry = new DnsEndPoint(host, port);
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    var socketEventArg = new SocketAsyncEventArgs {RemoteEndPoint = hostEntry};
                    socketEventArg.Completed += (s, e) =>
                    {
                        reachable = e.SocketError == SocketError.Success;

                        clientDone.Set();
                    };

                    clientDone.Reset();

                    socket.ConnectAsync(socketEventArg);

                    clientDone.WaitOne(msTimeout);

                    return reachable;
                }
            });
        }

        public IEnumerable<ConnectionType> ConnectionTypes
        {
            get
            {
                var networkInterfaceList = new NetworkInterfaceList();
                foreach (var networkInterfaceInfo in
                        networkInterfaceList.Where(s => s.InterfaceState == ConnectState.Connected))
                {
                    ConnectionType type;
                    switch (networkInterfaceInfo.InterfaceSubtype)
                    {
                        case NetworkInterfaceSubType.Desktop_PassThru:
                            type = ConnectionType.Desktop;
                            break;
                        case NetworkInterfaceSubType.WiFi:
                            type = ConnectionType.WiFi;
                            break;
                        case NetworkInterfaceSubType.Unknown:
                            type = ConnectionType.Other;
                            break;
                        default:
                            type = ConnectionType.Cellular;
                            break;
                    }
                    yield return type;
                }
            }
        }

        public IEnumerable<int> Bandwidths
        {
            get
            {
                var networkInterfaceList = new NetworkInterfaceList();
                return
                    networkInterfaceList.Where(
                        networkInterfaceInfo => networkInterfaceInfo.InterfaceState == ConnectState.Connected)
                        .Select(networkInterfaceInfo => networkInterfaceInfo.Bandwidth)
                        .ToArray();
            }
        }
    }
}
