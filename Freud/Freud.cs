using System;
using System.Collections.Generic;
using System.IO;
using Freud.PrimitiveTypeInfo;

namespace Freud
{
    public class FreudManager
    {
        public Dictionary<Type, ITypeInfo> TypeInfoCache { get; set; }
        public ITypeInfoFactory TypeInfoFactory { get; set; }

        private readonly MemoryStream BufferMemoryStream = new MemoryStream();

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
            TypeInfoCache[typeof(Boolean)] = new BooleanPrimitiveTypeInfo();

            TypeInfoCache[typeof(Byte)] = new BytePrimitiveTypeInfo();

            TypeInfoCache[typeof(Char)] = new CharPrimitiveTypeInfo();

            TypeInfoCache[typeof(UInt16)] = new UInt16PrimitiveTypeInfo();

            TypeInfoCache[typeof(UInt32)] = new UInt32PrimitiveTypeInfo();

            TypeInfoCache[typeof(UInt64)] = new UInt64PrimitiveTypeInfo();

            TypeInfoCache[typeof(Int16)] = new Int16PrimitiveTypeInfo();

            TypeInfoCache[typeof(Int32)] = new Int32PrimitiveTypeInfo();

            TypeInfoCache[typeof(Int64)] = new Int64PrimitiveTypeInfo();

            TypeInfoCache[typeof(Single)] = new SinglePrimitiveTypeInfo();

            TypeInfoCache[typeof(Double)] = new DoublePrimitiveTypeInfo();

            TypeInfoCache[typeof(String)] = new StringPrimitiveTypeInfo();

            TypeInfoCache[typeof(DateTime)] = new DateTimePrimitiveTypeInfo();
        }


        public byte[] Serialize(Type type, object source)
        {            
            registerType(type);
            BufferMemoryStream.Seek(0, SeekOrigin.Begin);

            TypeInfoCache[type].Serialize(source, BufferMemoryStream);

            return BufferMemoryStream.ToArray();
        }


        public byte[] Serialize<T>(T source)
        {
            return Serialize(typeof(T), source);
        }

        public T Deserialize<T>(byte[] data)
        {
            return (T) Deserialize(typeof(T), data);
        }

        public object Deserialize(Type type, byte[] data)
        {
            registerType(type);
            var stream = new MemoryStream(data, false);
            return TypeInfoCache[type].Deserialize(stream);
        }

        private void registerType(Type type)
        {
            if(TypeInfoCache.ContainsKey(type))
                return;

            TypeInfoCache[type] = TypeInfoFactory.Create(type, this);
        }
    }
}
