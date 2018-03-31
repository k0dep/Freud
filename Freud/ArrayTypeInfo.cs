using System;
using System.IO;
using System.Linq;

namespace Freud
{
    public class ArrayTypeInfo : ITypeInfo
    {
        public Type TargetType { get; }
        public ITypeInfo ElementTypeInfo { get; }

        public ArrayTypeInfo(Type targetType, ITypeInfo elementTypeInfo)
        {
            TargetType = targetType;
            ElementTypeInfo = elementTypeInfo;
        }

        public void Serialize(object data, Stream s)
        {
            if (data == null)
            {
                s.WriteByte(0xFF);
                return;
            }

            s.WriteByte(0);

            var arr = ((Array)data);
            s.Write(BitConverter.GetBytes((short)arr.Rank), 0, 2);
            for (int i = 0; i < arr.Rank; i++)
                s.Write(BitConverter.GetBytes((short)arr.GetLength(i)), 0, 2);

            var indices = new int[arr.Rank];

            for (int i = 0; i < arr.Rank; i++)
            {
                var dimensionLen = arr.GetLength(i);
                for (int j = 0; j < dimensionLen; j++)
                {
                    ElementTypeInfo.Serialize(arr.GetValue(indices), s);
                    indices[i] += 1;
                }
                indices[i] -= 1;
            }

        }

        public object Deserialize(Stream data)
        {
            if (data.ReadByte() == 0xFF)
                return null;

            var intBuff = new byte[2];
            data.Read(intBuff, 0, 2);
            var count = BitConverter.ToInt16(intBuff, 0);
            var dimestions = new int[count];

            for (int i = 0; i < count; i++)
            {
                data.Read(intBuff, 0, 2);
                dimestions[i] = BitConverter.ToInt16(intBuff, 0);
            }

            var array = (Array)Activator.CreateInstance(TargetType, dimestions.Cast<object>().ToArray());

            var indices = new int[count];

            for (int i = 0; i < count; i++)
            {
                var dimensionLen = dimestions[i];
                for (int j = 0; j < dimensionLen; j++)
                {
                    array.SetValue(ElementTypeInfo.Deserialize(data), indices);
                    indices[i] += 1;
                }
                indices[i] -= 1;
            }

            return array;
        }
    }
}