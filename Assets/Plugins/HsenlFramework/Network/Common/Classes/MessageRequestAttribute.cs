using System;

namespace Hsenl.Network {
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)]
    public class MessageRequestAttribute : MessageAttribute { }
}