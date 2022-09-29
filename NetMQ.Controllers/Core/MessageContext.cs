namespace NetMQ.Controllers.Core
{
    public class MessageContext<TSocket, TMessage> 
        where TSocket : NetMQSocket 
        where TMessage: class
    {
        public TSocket Socket { get; protected set; }
        public TMessage Message { get; protected set; }
    }

    public class NetMQMessageContext<TSocket> : MessageContext<TSocket, NetMQMessage> where TSocket : NetMQSocket
    {
        internal NetMQMessageContext(TSocket socket, NetMQMessage message)
        {
            Socket = socket;
            Message = message;
        }
    }
}