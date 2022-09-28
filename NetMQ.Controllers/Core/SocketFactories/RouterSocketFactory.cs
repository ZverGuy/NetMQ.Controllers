using System.Collections.Generic;
using System.Reflection;
using NetMQ.Controllers.Attributes.Filtering;
using NetMQ.Sockets;

namespace NetMQ.Controllers.Core.SocketFactories
{
    public class RouterSocketFactory : ISocketFactory<RouterSocketAttribute>
    {
        public NetMQSocket BuildSocket(object controllerInstance, MethodInfo handler, IEnumerable<IFilter> filters,
            RouterSocketAttribute socketAttribute)
        {
            var socket = new RouterSocket();
            socket.Bind(socketAttribute.ConnectionString);
            socket.ReceiveReady += (sender, args) =>
            {
                var msg = args.Socket.ReceiveMultipartMessage();
                var valid = true;
                foreach (var filter in filters)
                {
                    valid = filter.IsMatch(msg);
                }
                
                if (valid)
                {
                    
                }
            };
            return socket;
        }
    }
}