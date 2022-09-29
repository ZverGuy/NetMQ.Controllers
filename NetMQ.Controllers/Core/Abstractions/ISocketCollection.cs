using System;
using System.Collections.Generic;

namespace NetMQ.Controllers.Core
{
    public interface ISocketCollection : IEnumerable<NetMQSocket>
    {
        IEnumerator<NetMQSocket> GetEnumerator();
        TSocket GetOrCreate<TSocket>(string connectionstring, Func<TSocket> factory = null) where TSocket: NetMQSocket;
    }
}