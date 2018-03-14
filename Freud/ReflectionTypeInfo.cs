using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Freud
{
    public class ReflectionTypeInfo : ITypeInfo
    {
        private List<MemberInfo> dataMembers;
        private List<Action<object, Stream>> partSerializers;
        private List<Func<Stream, object>> partDeserializers;

        public Type TargetType { get; }

        public void Serialize(object s, Stream stream)
        {
            if (s == null)
            {
                stream.WriteByte(0xFF);
                return;
            }

            stream.WriteByte(0);

            for (int i = 0; i < dataMembers.Count; i++)
            {
                byte[] part = null;

                if (dataMembers[i].MemberType == MemberTypes.Field)
                {
                    partSerializers[i](((FieldInfo)dataMembers[i]).GetValue(s), stream);
                }

                if (dataMembers[i].MemberType == MemberTypes.Property)
                {
                    partSerializers[i](((PropertyInfo)dataMembers[i]).GetValue(s), stream);
                }
            }
        }

        public object Deserialize(Stream s)
        {
            if (s.ReadByte() == 0xFF)
                return null;

            var instance = Activator.CreateInstance(TargetType);

            for (int i = 0; i < dataMembers.Count; i++)
            {
                if (dataMembers[i].MemberType == MemberTypes.Field)
                {
                    ((FieldInfo)dataMembers[i]).SetValue(instance, partDeserializers[i](s));
                }

                if (dataMembers[i].MemberType == MemberTypes.Property)
                {
                    ((PropertyInfo)dataMembers[i]).SetValue(instance, partDeserializers[i](s));
                }

            }

            return instance;
        }


        public ReflectionTypeInfo(Type type, FreudManager manager)
        {
            TargetType = type;
            registerType(type, manager);
        }


        private void checkType(Type type)
        {
            if (type.IsAbstract)
                throw new FreudTypeCheckException("Abstract class '" + type.FullName + "' founded", type, "abstract class");

            if (type.IsInterface)
                throw new FreudTypeCheckException("Interface '" + type.FullName + "' founded", type, "interface");
        }

        private void registerType(Type type, FreudManager manager)
        {
            if (manager.TypeInfoCache.ContainsKey(type))
                return;

            checkType(type);

            dataMembers = new List<MemberInfo>();

            foreach (var memberInfo in type.GetMembers())
            {
                if (memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property)
                    continue;

                if (memberInfo.MemberType == MemberTypes.Property && !((PropertyInfo)memberInfo).CanWrite && !((PropertyInfo)memberInfo).CanRead)
                    continue;

                dataMembers.Add(memberInfo);
            }


            partSerializers = new List<Action<object, Stream>>();
            partDeserializers = new List<Func<Stream, object>>();

            foreach (var dataMember in dataMembers)
            {
                Type memberType = null;
                if (dataMember.MemberType == MemberTypes.Field)
                {
                    var field = (FieldInfo) dataMember;
                    memberType = field.FieldType;
                }

                if (dataMember.MemberType == MemberTypes.Property)
                {
                    var prop = (PropertyInfo) dataMember;
                    memberType = prop.PropertyType;
                }

                var newInstance = new ReflectionTypeInfo(memberType, manager);

                partSerializers.Add(manager.TypeInfoCache[memberType].Serialize);
                partDeserializers.Add(manager.TypeInfoCache[memberType].Deserialize);
            }

            manager.TypeInfoCache[type] = this;
        }
    }
}