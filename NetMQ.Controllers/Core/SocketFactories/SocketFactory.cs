using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using NetMQ.Controllers.Attributes;
using NetMQ.Controllers.Attributes.Filtering;

namespace NetMQ.Controllers.Core.SocketFactories
{
    public class SocketFactory
    {
        private readonly ILogger<SocketFactory> _logger;

        public SocketFactory(ILogger<SocketFactory> logger)
        {
            _logger = logger;
        }

        public IEnumerable<NetMQSocket> BuildSockets(object controller, MethodInfo handler, IEnumerable<IFilter> filters)
        {
            
            var socketTypes = ControllerHelper.GetSocketType(handler);
            var result = new List<NetMQSocket>(socketTypes.Count());
            foreach (var socketType in socketTypes)
            {
                try
                {
                    var factory = GetSocketFactory(socketType);
                    if (factory == null)
                    {
                        _logger.LogError($"Not found socket factory for {nameof(socketType)}");
                        continue;
                    }
                    var socket = (NetMQSocket)factory.GetType().GetMethod("BuildSocket").Invoke(factory, new object[]
                    {
                        controller, handler, filters, socketType
                    });
                    result.Add(socket);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
            }

            return result;
        }

        private object GetSocketFactory<TSocket>(TSocket socketType) where TSocket: BaseSocketAttribute
        {
            var type = socketType.GetType();
            var generic = typeof(ISocketFactory<>);
            var factorytype = generic.MakeGenericType(type);

            var factory = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => factorytype.IsAssignableFrom(x));
            if (factory == null)
                return null;
            var instance = Activator.CreateInstance(factory);
            return instance;
        }
    }
}