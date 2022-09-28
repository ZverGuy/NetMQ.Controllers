using System;
using NetMQ.Controllers.Attributes;

namespace NetMQ.Controllers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouterSocketAttribute : BaseSocketAttribute
    {
        
    }
}