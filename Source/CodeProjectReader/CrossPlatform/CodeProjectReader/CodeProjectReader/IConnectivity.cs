using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Model;

namespace CodeProjectReader
{
    public interface IConnectivity
    {
        bool IsConnected { get; }
        Task<bool> IsPingReachable(string host, int msTimeout = 5000);
        Task<bool> IsPortReachable(string host, int port = 80, int msTimeout = 5000);
        IEnumerable<ConnectionType> ConnectionTypes { get; }
        IEnumerable<int> Bandwidths { get; }
    }
}
