using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NetMQ.Controllers.Core
{
    public class SocketCollection : IEnumerable<NetMQSocket>
    {
        private ConcurrentBag<(string connectionstring, NetMQSocket socket)> _sockets =
            new ConcurrentBag<(string connectionstring, NetMQSocket socket)>();

        public SocketCollection()
        {
            
        }
        public IEnumerator<NetMQSocket> GetEnumerator() => _sockets.Select(x => x.socket).GetEnumerator();

        public TSocket GetOrCreate<TSocket>(string connectionstring, Func<TSocket>? factory = null) where TSocket: NetMQSocket
        {
            TSocket socket;
            socket = (TSocket) _sockets.FirstOrDefault(x =>
                x.connectionstring == connectionstring && x.socket.GetType() == typeof(TSocket)).socket;
            if (socket != null)
                return socket;
            socket = factory?.Invoke();
            return socket;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}