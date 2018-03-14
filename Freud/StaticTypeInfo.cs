using System;
using System.IO;

namespace Freud
{
    public class StaticTypeInfo<T> : ITypeInfo
    {
        public Type TargetType { get { return typeof(T); } }

        private Action<object, Stream> serialize;
        private Func<Stream, object> deserialize;


        public StaticTypeInfo(Action<object, Stream> serialize, Func<Stream, object> deserialize)
        {
            this.serialize = serialize;
            this.deserialize = deserialize;
        }


        public void Serialize(object data, Stream s)
        {
            serialize(data, s);
        }

        public object Deserialize(Stream data)
        {
            return deserialize(data);
        }
    }
}