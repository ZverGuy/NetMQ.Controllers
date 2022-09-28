using System.Text.RegularExpressions;

namespace NetMQ.Controllers.Attributes.Filtering
{
    public class RegexFilter : IFilter
    {
        private readonly int _frame;

        private readonly Regex _regex;

        public RegexFilter(string pattern, int frame)
        {
            _frame = frame;
            _regex = new Regex(pattern);
        }
        public bool IsMatch(NetMQMessage message)
        {
            return _regex.IsMatch(message[_frame].ConvertToString());
        }
    }
}