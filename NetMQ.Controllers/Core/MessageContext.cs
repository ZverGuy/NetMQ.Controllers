namespace NetMQ.Controllers.Core
{
    public class MessageContext<TSocket, TMessage> 
        where TSocket : NetMQ.NetMQSocket 
        where TMessage: class
    {
        public TSocket Socket { get; }
        public TMessage Message { get; }
    }
    public class NetMQMessageContext<TSocket> : MessageContext<TSocket, NetMQMessage> where TSocket: NetMQSocket,
}