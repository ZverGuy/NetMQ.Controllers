using NetMQ.Controllers;
using NetMQ.Controllers.Core;
using NetMQ.Sockets;

namespace ExampleApp
{
    public class TestController
    {
        [RouterSocket(ConnectionString = "inproc://test")]
        public int LmaoTest(NetMQMessageContext<RouterSocket> msg)
        {
            return 2;
        }
    }
}