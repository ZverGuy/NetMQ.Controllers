using System;
using System.Threading.Tasks;
using NetMQ.Controllers;
using NetMQ.Controllers.Attributes;
using NetMQ.Controllers.Core;
using NetMQ.Sockets;

namespace ExampleApp
{
    public class TestController
    {
        [SubscriberSocket(ConnectionString = "inproc://test", Topic = "TestTopic")]
        public async Task<int> LmaoTest(MessageContext<SubscriberSocket, string> context)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return 2;
        }
    }
}