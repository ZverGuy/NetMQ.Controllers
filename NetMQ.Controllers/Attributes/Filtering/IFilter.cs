namespace NetMQ.Controllers.Attributes.Filtering
{
    public interface IFilter
    {
        bool IsMatch(NetMQMessage message);
    }
}