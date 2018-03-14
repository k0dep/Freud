using System;
using System.IO;

namespace Freud
{
    public interface ITypeInfo
    {
        Type TargetType { get; }

        void Serialize(object data, Stream s);
        object Deserialize(Stream data);
    }
}