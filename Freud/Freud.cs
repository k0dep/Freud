using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Freud
{
    public class FreudManager
    {
        public Dictionary<Type, ITypeInfo> TypeInfoCache { get; set; }
        public ITypeInfoFactory TypeInfoFactory { get; set; }


        private static byte[] __FillMaxInt16 = BitConverter.GetBytes(UInt16.MaxValue);


        public FreudManager()
        {
            TypeInfoCache = new Dictionary<Type, ITypeInfo>();
            TypeInfoFactory = new ExpressionTypeInfoFactory();

            registerDefaultPrimitives();
        }

        public FreudManager(ITypeInfoFactory typeInfoFactory)
        {
            TypeInfoCache = new Dictionary<Type, ITypeInfo>();
            TypeInfoFactory = typeInfoFactory;

            registerDefaultPrimitives();
        }

        private void registerDefaultPrimitives()
        {
            TypeInfoCache[typeof(Boolean)] = new StaticTypeInfo<Boolean>(
                (o, s) =>
                {
                    var bytes = BitConverter.GetBytes((Boolean) o);
                    s.Write(bytes, 0, bytes.Length);
                },
                s => (s.ReadByte()) == 1);

            TypeInfoCache[typeof(Byte)] = new StaticTypeInfo<Byte>(
                (o, s) =>
                {
                    var bytes = BitConverter.GetBytes((Byte)o);
                    s.Write(bytes, 0, bytes.Length);
                },
                s => (Byte) s.ReadByte());

            TypeInfoCache[typeof(Char)] = new StaticTypeInfo<Char>(
                (o, s) =>
                {
                    var bytes = BitConverter.GetBytes((Char)o);
                    s.Write(bytes, 0, bytes.Length);
                },
                s => BitConverter.ToChar(new Byte[] {(byte) s.ReadByte(), (byte) s.ReadByte()}, 0));

            TypeInfoCache[typeof(UInt16)] = new StaticTypeInfo<UInt16>(
                (o, s) =>
                {
                    var bytes = BitConverter.GetBytes((UInt16)o);
                    s.Write(bytes, 0, bytes.Length);
                },
                s => BitConverter.ToUInt16(new Byte[] {(byte) s.ReadByte(), (byte) s.ReadByte()}, 0));

            TypeInfoCache[typeof(UInt32)] = new StaticTypeInfo<UInt32>(
                (o, s) =>
                {
                    var bytes = BitConverter.GetBytes((UInt32)o);
                    s.Write(bytes, 0, bytes.Length);
                },
                s => BitConverter.ToUInt32(
                    new Byte[] {(byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte()}, 0));

            TypeInfoCache[typeof(UInt64)] = new StaticTypeInfo<UInt64>(
                (o, s) =>
                {
                    var bytes = BitConverter.GetBytes((UInt64)o);
                    s.Write(bytes, 0, bytes.Length);
                },
                s => BitConverter.ToUInt64(
                    new Byte[]
                    {
                        (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte(),
                        (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte()
                    }, 0));

            TypeInfoCache[typeof(Int16)] = new StaticTypeInfo<Int16>(
                (o, s) =>
                {
                    var bytes = BitConverter.GetBytes((Int16)o);
                    s.Write(bytes, 0, bytes.Length);
                },
                s => BitConverter.ToInt16(new Byte[] {(byte) s.ReadByte(), (byte) s.ReadByte()}, 0));

            TypeInfoCache[typeof(Int32)] = new StaticTypeInfo<Int32>(
                (o, s) =>
                {
                    var bytes = BitConverter.GetBytes((Int32)o);
                    s.Write(bytes, 0, bytes.Length);
                },
                s => BitConverter.ToInt32(
                    new Byte[] {(byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte()}, 0));

            TypeInfoCache[typeof(Int64)] = new StaticTypeInfo<Int64>(
                (o, s) =>
                {
                    var bytes = BitConverter.GetBytes((Int64)o);
                    s.Write(bytes, 0, bytes.Length);
                },
                s => BitConverter.ToInt64(
                    new Byte[]
                    {
                        (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte(),
                        (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte()
                    }, 0));

            TypeInfoCache[typeof(Single)] = new StaticTypeInfo<Single>(
                (o, s) =>
                {
                    var bytes = BitConverter.GetBytes((Single)o);
                    s.Write(bytes, 0, bytes.Length);
                },
                s => BitConverter.ToSingle(
                    new Byte[] {(byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte()}, 0));


            TypeInfoCache[typeof(Double)] = new StaticTypeInfo<Double>(
                (o, s) =>
                {
                    var bytes = BitConverter.GetBytes((Double)o);
                    s.Write(bytes, 0, bytes.Length);
                },
                s => BitConverter.ToDouble(new Byte[]
                {
                    (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte(),
                    (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte(), (byte) s.ReadByte()
                }, 0));

            TypeInfoCache[typeof(String)] = new StaticTypeInfo<String>(
                (o, m) =>
                {
                    if (o == null)
                    {
                        m.Write(__FillMaxInt16, 0, __FillMaxInt16.Length);
                        return;
                    }

                    var bytes = Encoding.UTF8.GetBytes((String) o);
                    var lenBytes = BitConverter.GetBytes((UInt16) bytes.Length);

                    m.Write(lenBytes, 0, lenBytes.Length);
                    m.Write(bytes, 0, bytes.Length);
                },
                s =>
                {
                    String res = null;

                    var length = BitConverter.ToUInt16(new Byte[] {(byte) s.ReadByte(), (byte) s.ReadByte()}, 0);

                    if (length == UInt16.MaxValue)
                        return res;

                    var byteList = new List<byte>();

                    for (int i = 0; i < length; i++)
                        byteList.Add((byte) s.ReadByte());

                    res = Encoding.UTF8.GetString(byteList.ToArray());

                    return res;
                });
        }


        public byte[] Serialize<T>(T source)
        {
            var type = typeof(T);
            registerType(type);
            var s = new MemoryStream();

            TypeInfoCache[type].Serialize(source, s);

            return s.GetBuffer();
        }

        public T Deserialize<T>(byte[] data)
        {
            var type = typeof(T);
            registerType(type);
            var stream = new MemoryStream(data, false);
            return (T) TypeInfoCache[type].Deserialize(stream);
        }

        private void registerType(Type type)
        {
            if(TypeInfoCache.ContainsKey(type))
                return;

            TypeInfoCache[type] = TypeInfoFactory.Create(type, this);
        }
    }
}
