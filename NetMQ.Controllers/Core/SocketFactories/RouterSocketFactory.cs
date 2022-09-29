using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using NetMQ.Controllers.Attributes.Filtering;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace NetMQ.Controllers.Core.SocketFactories
{
    public class RouterSocketFactory : ISocketFactory<RouterSocketAttribute>
    {
        private readonly SocketCollection _collection;

        public RouterSocketFactory(SocketCollection collection)
        {
            _collection = collection;
        }
        public NetMQSocket BuildSocket(object controllerInstance, MethodInfo handler, IEnumerable<IFilter> filters,
            RouterSocketAttribute socketAttribute)
        {
            var socket = _collection.GetOrCreate(socketAttribute.ConnectionString, () =>
            {
                var socket = new RouterSocket();
                socket.Bind(socketAttribute.ConnectionString);
                return socket;
            });
            var type = MethodHelpers.GetContextType(handler);
            socket.ReceiveReady += async (sender, args) =>
            {
                var msg = args.Socket.ReceiveMultipartMessage();
                var address = msg[0].ConvertToString();
                var valid = true;
                foreach (var filter in filters)
                {
                    valid = filter.IsMatch(msg);
                }
                
                if (valid)
                {
                    if (type == ContextType.NetMqContext)
                    {
                        var context = new NetMQMessageContext<RouterSocket>(socket, msg);
                        var result = handler.Invoke(controllerInstance, new []{context});
                        if (result is Task task)
                        {
                            await task;
                            result = task.GetType().GetProperty("Result").GetValue(task);
                        }
                        
                        if (result != null)
                        {
                            if (result is NetMQMessage msgResponce)
                            {
                                args.Socket.SendMultipartMessage(msgResponce);
                            }
                            else
                            {
                                string content = JsonConvert.SerializeObject(result);
                                var messageResponse = new NetMQMessage();
                                messageResponse.Append(address);
                                messageResponse.AppendEmptyFrame();
                                messageResponse.Append(content);
                                args.Socket.SendMultipartMessage(messageResponse);
                            }
                        }
                    }
                }
            };
            return socket;
        }
    }
}