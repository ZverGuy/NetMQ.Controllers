using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetMQ.Controllers.Attributes;
using NetMQ.Controllers.Core.SocketFactories;

namespace NetMQ.Controllers
{
    public class NetMQHostedService : IHostedService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<NetMQHostedService> _logger;
        private readonly SocketFactory _factory;
        private List<NetMQSocket> _sockets = new List<NetMQSocket>();
        private List<object> _controllers = new List<object>();
        public NetMQHostedService(IServiceProvider provider, ILogger<NetMQHostedService> logger, SocketFactory factory)
        {
            _provider = provider;
            _logger = logger;
            _factory = factory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var controllers = ControllerHelper.GetControllers();
            foreach (var controller in controllers)
            {
                var instance = _provider.GetService(controller);
                _controllers.Add(instance);
                var methods = ControllerHelper.GetMethodsThatHaveSocketAttributes(instance);
                foreach (var method in methods)
                {
                    var filters = ControllerHelper.GetFilters(method);
                    _sockets.AddRange(_factory.BuildSockets(instance, method, filters));
                }
            }

            Task.Run((() => Execute(cancellationToken)));
            return Task.CompletedTask;
        }


        private async Task Execute(CancellationToken token)
        {
            var poller = new NetMQPoller();
            foreach (var socket in _sockets)
            {
                poller.Add(socket);
            }
            poller.RunAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var netMqSocket in _sockets)
            {
                netMqSocket.Dispose();
            }

            foreach (var controller in _controllers)
            {
                if (controller is IDisposable disp)
                {
                    disp.Dispose();
                }
            }
            return Task.CompletedTask;
        }
    }
}