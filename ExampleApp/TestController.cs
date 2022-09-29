using System;
using System.Threading.Tasks;
using NetMQ.Controllers;
using NetMQ.Controllers.Core;
using NetMQ.Sockets;

namespace ExampleApp
{
    public class TestController
    {
        [RouterSocket(ConnectionString = "inproc://test")]
        public async Task<int> LmaoTest(NetMQMessageContext<RouterSocket> msg)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return 2;
        }
    }
}