using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetMQ.Controllers.Attributes;
using NetMQ.Controllers.Core;
using NetMQ.Controllers.Core.SocketFactories;

namespace NetMQ.Controllers
{
    public class NetMQHostedService : IHostedService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<NetMQHostedService> _logger;
        private readonly AbstractSocketFactory _factory;
        private readonly ISocketCollection _socketCollection;
        private List<object> _controllers = new List<object>();
        private NetMQPoller _poller;
        public NetMQHostedService(IServiceProvider provider, ILogger<NetMQHostedService> logger, AbstractSocketFactory factory, ISocketCollection socketCollection)
        {
            _provider = provider;
            _logger = logger;
            _factory = factory;
            _socketCollection = socketCollection;
            _poller = new NetMQPoller();
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
                    _factory.BuildSockets(instance, method, filters);
                }
            }

            foreach (var socket in _socketCollection)
            {
                _poller.Add(socket);
            }
            _poller.RunAsync();
            return Task.CompletedTask;
        }
        

        public Task StopAsync(CancellationToken cancellationToken)
        {
           
            foreach (var netMqSocket in _socketCollection)
            {
                netMqSocket.Dispose();
            }
            _poller.StopAsync();
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