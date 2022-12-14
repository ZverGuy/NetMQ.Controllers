using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetMQ.Controllers.Attributes.Filtering;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace NetMQ.Controllers.Core.SocketFactories
{
    public class RouterSocketFactory : ISocketFactory<RouterSocketAttribute>
    {
        private readonly ISocketCollection _collection;
        private readonly ILogger<RouterSocketFactory> _logger;

        public RouterSocketFactory(ISocketCollection collection, ILogger<RouterSocketFactory> logger)
        {
            _collection = collection;
            _logger = logger;
        }

        public NetMQSocket BuildSocket(object controllerInstance, MethodInfo handler, IEnumerable<IFilter> filters,
            RouterSocketAttribute socketAttribute)
        {
            if (string.IsNullOrWhiteSpace(socketAttribute.ConnectionString))
                throw new ArgumentException($"ConnectionString not defined in {handler.Name}");
            var socket = _collection.GetOrCreate(socketAttribute.ConnectionString, () =>
            {
                var socket = new RouterSocket();
                socket.Bind(socketAttribute.ConnectionString);
                return socket;
            });
            var type = MethodHelpers.GetContextType(handler);
            socket.ReceiveReady += async (sender, args) =>
            {
                try
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
                            var result = handler.Invoke(controllerInstance, new[] { context });
                            if (result is Task task)
                            {
                                await task;
                                result = task.GetType().GetProperty("Result").GetValue(task);
                            }
                            SendResult(result, args, address);
                        }

                        if (type == ContextType.TypedContext)
                        {
                            var context = MethodHelpers.GetTypedContext(handler, msg, socket);
                            if (context == null)
                                throw new ArgumentException("Message Type MisMatch");
                            var result = handler.Invoke(controllerInstance, new[] { context });
                            if (result is Task task)
                            {
                                await task;
                                result = task.GetType().GetProperty("Result").GetValue(task);
                            }

                            SendResult(result, args, address);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
            };
            return socket;
        }

        private static void SendResult(object result, NetMQSocketEventArgs args, string address)
        {
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
}