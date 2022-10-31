using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetMQ.Controllers.Attributes;
using NetMQ.Controllers.Attributes.Filtering;
using NetMQ.Sockets;

namespace NetMQ.Controllers.Core.SocketFactories
{
    public class SubscriberSocketFactory : ISocketFactory<SubscriberSocketAttribute>
    {
        private readonly ISocketCollection _socketCollection;
        private readonly ILogger<SubscriberSocketFactory> _logger;

        public SubscriberSocketFactory(ISocketCollection socketCollection, ILogger<SubscriberSocketFactory> logger)
        {
            _socketCollection = socketCollection;
            _logger = logger;
        }
        public NetMQSocket BuildSocket(object controllerInstance, MethodInfo handler, IEnumerable<IFilter> filters,
            SubscriberSocketAttribute socketAttribute)
        {
            if (string.IsNullOrWhiteSpace(socketAttribute.ConnectionString))
                throw new ArgumentException($"ConnectionString not defined in {handler.Name}");
            if(string.IsNullOrWhiteSpace(socketAttribute.Topic))
                throw new ArgumentException($"Topic not defined in {handler.Name}");
            var key = socketAttribute.ConnectionString + ":" + socketAttribute.Topic;
            var socket = _socketCollection.GetOrCreate<SubscriberSocket>(key, () =>
            {
                var soc = new SubscriberSocket();
                soc.Connect(socketAttribute.ConnectionString);
                soc.Subscribe(socketAttribute.Topic);
                return soc;
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
                            var context = new NetMQMessageContext<SubscriberSocket>(socket, msg);
                            var result = handler.Invoke(controllerInstance, new[] { context });
                            if (result is Task task)
                                await task;

                        }
                        else if (type == ContextType.TypedContext)
                        {
                            var context = MethodHelpers.GetTypedContext(handler, msg, socket);
                            if (context == null)
                                throw new ArgumentException("Message Type MisMatch");
                            var result = handler.Invoke(controllerInstance, new[] { context });
                            if (result is Task task)
                                await task;
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
    }
}