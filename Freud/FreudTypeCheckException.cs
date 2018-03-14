using System;

namespace Freud
{
    [Serializable]
    public class FreudTypeCheckException : Exception
    {
        public Type TargetType { get; set; }
        public string Reason { get; set; }

        public FreudTypeCheckException(string message, Type type, string reason) : base(message)
        {
            TargetType = type;
            Reason = reason;
        }
    }
}