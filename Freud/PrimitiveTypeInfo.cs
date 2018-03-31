using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Freud.PrimitiveTypeInfo
{
    public class BooleanPrimitiveTypeInfo : ITypeInfo
    {
        public Type TargetType { get { return typeof(Boolean); } }

        public void Serialize(object data, Stream s)
        {
            var bytes = BitConverter.GetBytes((Boolean)data);
            s.Write(bytes, 0, bytes.Length);
        }

        public object Deserialize(Stream data)
        {
            return data.ReadByte() == 1;
        }
    }

    public class BytePrimitiveTypeInfo : ITypeInfo
    {
        public Type TargetType { get { return typeof(Byte); } }

        public void Serialize(object data, Stream s)
        {
            var bytes = BitConverter.GetBytes((Byte)data);
            s.Write(bytes, 0, bytes.Length);
        }

        public object Deserialize(Stream data)
        {
            return (Byte)data.ReadByte();
        }
    }

    public class CharPrimitiveTypeInfo : ITypeInfo
    {
        public Type TargetType { get { return typeof(Char); } }

        public void Serialize(object data, Stream s)
        {
            var bytes = BitConverter.GetBytes((Char)data);
            s.Write(bytes, 0, bytes.Length);
        }

        public object Deserialize(Stream data)
        {
            return (Char)data.ReadByte();
        }
    }

    public class UInt16PrimitiveTypeInfo : ITypeInfo
    {
        public Type TargetType { get { return typeof(UInt16); } }

        public void Serialize(object data, Stream s)
        {
            var bytes = BitConverter.GetBytes((UInt16)data);
            s.Write(bytes, 0, bytes.Length);
        }

        public object Deserialize(Stream data)
        {
            return (UInt16)data.ReadByte();
        }
    }

    public class UInt32PrimitiveTypeInfo : ITypeInfo
    {
        public Type TargetType { get { return typeof(UInt32); } }

        public void Serialize(object data, Stream s)
        {
            var bytes = BitConverter.GetBytes((UInt32)data);
            s.Write(bytes, 0, bytes.Length);
        }

        public object Deserialize(Stream data)
        {
            return (UInt32)data.ReadByte();
        }
    }

    public class UInt64PrimitiveTypeInfo : ITypeInfo
    {
        public Type TargetType { get { return typeof(UInt64); } }

        public void Serialize(object data, Stream s)
        {
            var bytes = BitConverter.GetBytes((UInt64)data);
            s.Write(bytes, 0, bytes.Length);
        }

        public object Deserialize(Stream data)
        {
            return (UInt64)data.ReadByte();
        }
    }

    public class Int16PrimitiveTypeInfo : ITypeInfo
    {
        public Type TargetType { get { return typeof(Int16); } }

        public void Serialize(object data, Stream s)
        {
            var bytes = BitConverter.GetBytes((Int16)data);
            s.Write(bytes, 0, bytes.Length);
        }

        public object Deserialize(Stream data)
        {
            return (Int16)data.ReadByte();
        }
    }

    public class Int32PrimitiveTypeInfo : ITypeInfo
    {
        public Type TargetType { get { return typeof(Int32); } }

        public void Serialize(object data, Stream s)
        {
            var bytes = BitConverter.GetBytes((Int32)data);
            s.Write(bytes, 0, bytes.Length);
        }

        public object Deserialize(Stream data)
        {
            return (Int32)data.ReadByte();
        }
    }

    public class Int64PrimitiveTypeInfo : ITypeInfo
    {
        public Type TargetType { get { return typeof(Int64); } }

        public void Serialize(object data, Stream s)
        {
            var bytes = BitConverter.GetBytes((Int64)data);
            s.Write(bytes, 0, bytes.Length);
        }

        public object Deserialize(Stream data)
        {
            return (Int64)data.ReadByte();
        }
    }

    public class SinglePrimitiveTypeInfo : ITypeInfo
    {
        public Type TargetType { get { return typeof(Single); } }

        public void Serialize(object data, Stream s)
        {
            var bytes = BitConverter.GetBytes((Single)data);
            s.Write(bytes, 0, bytes.Length);
        }

        public object Deserialize(Stream data)
        {
            return (Single)data.ReadByte();
        }
    }

    public class DoublePrimitiveTypeInfo : ITypeInfo
    {
        public Type TargetType { get { return typeof(Double); } }

        public void Serialize(object data, Stream s)
        {
            var bytes = BitConverter.GetBytes((Double)data);
            s.Write(bytes, 0, bytes.Length);
        }

        public object Deserialize(Stream data)
        {
            return (Double)data.ReadByte();
        }
    }

    public class StringPrimitiveTypeInfo : ITypeInfo
    {
        public Type TargetType { get { return typeof(String); } }

        private static byte[] __FillMaxInt16 = BitConverter.GetBytes(UInt16.MaxValue);


        public void Serialize(object data, Stream s)
        {

            if (data == null)
            {
                s.Write(__FillMaxInt16, 0, __FillMaxInt16.Length);
                return;
            }

            if ((string) data == "")
            {
                s.Write(new byte[] {0, 0}, 0, 2);
                return;
            }

            var length = ((string) data).Length * sizeof(char);
            byte[] bytes = new byte[length];

            unsafe
            {
                fixed (void* ptrdst = bytes)
                fixed (void* ptrsrc = (string) data)
                {
                    Buffer.MemoryCopy(ptrsrc, ptrdst, length, length);
                }
            }

            var lenBytes = BitConverter.GetBytes((UInt16)bytes.Length);

            s.Write(lenBytes, 0, lenBytes.Length);
            s.Write(bytes, 0, bytes.Length);
        }

        public object Deserialize(Stream data)
        {
            String res = null;

            var length = BitConverter.ToUInt16(new Byte[] { (byte)data.ReadByte(), (byte)data.ReadByte() }, 0);

            if (length == UInt16.MaxValue)
                return res;

            if (length == 0)
                return "";

            var byteList = new List<byte>();

            for (int i = 0; i < length; i++)
                byteList.Add((byte)data.ReadByte());

            res = Encoding.Unicode.GetString(byteList.ToArray());

            return res;
        }
    }


    public class DateTimePrimitiveTypeInfo : ITypeInfo
    {
        public Type TargetType { get { return typeof(DateTime); } }


        public void Serialize(object data, Stream s)
        {
            var dt = (DateTime) data;
            s.Write(BitConverter.GetBytes(dt.ToBinary()), 0, sizeof(long));
        }

        public object Deserialize(Stream data)
        {
            var buffer = new byte[sizeof(long)];
            data.Read(buffer, 0, buffer.Length);
            return DateTime.FromBinary(BitConverter.ToInt64(buffer, 0));
        }
    }
}
