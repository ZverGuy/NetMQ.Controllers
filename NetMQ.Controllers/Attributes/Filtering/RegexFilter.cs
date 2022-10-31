using System.Text.RegularExpressions;

namespace NetMQ.Controllers.Attributes.Filtering
{
    public class RegexMatchAttribute : FilterAttribute
    {
        private readonly int _frame;

        private readonly Regex _regex;

        public RegexMatchAttribute(string pattern, int frame)
        {
            _frame = frame;
            _regex = new Regex(pattern);
        }
        public override bool IsMatch(NetMQMessage message)
        {
            return _regex.IsMatch(message[_frame].ConvertToString());
        }
    }
    public class RegexNotMatchAttribute : FilterAttribute
    {
        private readonly int _frame;

        private readonly Regex _regex;

        public RegexNotMatchAttribute(string pattern, int frame)
        {
            _frame = frame;
            _regex = new Regex(pattern);
        }
        public override bool IsMatch(NetMQMessage message)
        {
            return !_regex.IsMatch(message[_frame].ConvertToString());
        }
    }
}