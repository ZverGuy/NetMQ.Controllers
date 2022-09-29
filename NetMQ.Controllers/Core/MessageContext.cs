namespace NetMQ.Controllers.Core
{
    public class MessageContext<TSocket, TMessage> 
        where TSocket : NetMQSocket 
        where TMessage: class
    {
        public string? RoutingKey { get; internal set; }
        public TSocket Socket { get; internal set; }
        public TMessage Message { get; internal set; }

        internal MessageContext(TSocket socket, TMessage message, string routingKey)
        {
            Socket = socket;
            Message = message;
            RoutingKey = routingKey;
        }
    }

    public class NetMQMessageContext<TSocket> : MessageContext<TSocket, NetMQMessage> where TSocket : NetMQSocket
    {
        internal NetMQMessageContext(TSocket socket, NetMQMessage message) : base(socket, message, message[0].ConvertToString())
        {
        }
    }
}