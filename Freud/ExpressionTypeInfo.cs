using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Freud
{
    public class ExpressionTypeInfo : ITypeInfo, IExpressionTypeInfo
    {
        public Type TargetType { get; private set; }

        private Action<object, Stream> _serialize;
        private Func<Stream, object> _deserialize;

        private static readonly ParameterExpression _parameterExpression = Expression.Parameter(typeof(Object), "source");
        private static readonly ParameterExpression ParameterExpression = Expression.Parameter(typeof(Stream), "stream");
        private static readonly MethodInfo TypeInfoSerialize = typeof(ITypeInfo).GetMethod("Serialize");
        private static readonly MethodInfo TypeInfoDeserialize = typeof(ITypeInfo).GetMethod("Deserialize");


        public ExpressionTypeInfo(Type targetType, FreudManager manager)
        {
            TargetType = targetType;
            registerType(targetType, manager);
        }



        public void Serialize(object data, Stream stream)
        {
            if (data == null)
            {
                stream.WriteByte(0xFF);
                return;
            }

            stream.WriteByte(0);

            _serialize(data, stream);
        }


        public object Deserialize(Stream data)
        {
            if (data.ReadByte() == 0xFF)
                return null;

            return _deserialize(data);
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

            if (type.IsArray)
                registerArray(type, manager);
            else
                registerGeneral(type, manager);

            manager.TypeInfoCache[type] = this;
        }

        private void registerArray(Type type, FreudManager manager)
        {
            registerType(type.GetElementType(), manager);

            var elementInfo = manager.TypeInfoCache[type.GetElementType()];

            var arraySerializer = new ArrayTypeInfo(type, elementInfo);

            _serialize = arraySerializer.Serialize;
            _deserialize = arraySerializer.Deserialize;
        }

        private void registerGeneral(Type type, FreudManager manager)
        {
            var dataMembers = GetAllowedTypeMembers(type);

            var parts = SelectMemberSerializers(manager, dataMembers);

            GenerateSerialization(manager, type, dataMembers, parts);
        }


        private static List<ITypeInfo> SelectMemberSerializers(FreudManager manager, List<MemberInfo> dataMembers)
        {
            var parts = new List<ITypeInfo>();

            foreach (var dataMember in dataMembers)
            {
                var memberType = dataMember.PropertyOrFieldType();
                var newInstance = new ExpressionTypeInfo(memberType, manager);
                parts.Add(manager.TypeInfoCache[memberType]);
            }
            return parts;
        }


        private static List<MemberInfo> GetAllowedTypeMembers(Type type)
        {
            var dataMembers = new List<MemberInfo>();

            foreach (var memberInfo in type.GetMembers())
            {
                if (memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property)
                    continue;

                if (memberInfo.MemberType == MemberTypes.Property && !((PropertyInfo) memberInfo).CanWrite &&
                    !((PropertyInfo) memberInfo).CanRead)
                    continue;

                dataMembers.Add(memberInfo);
            }
            return dataMembers;
        }


        private void GenerateSerialization(FreudManager manager, Type type, List<MemberInfo> dataMembers,
            List<ITypeInfo> parts)
        {
            var serializeStatements = new List<Expression>();
            var deserializeStatements = new List<Expression>();

            var instVariable = Expression.Variable(type, "instance");

            for (int i = 0; i < dataMembers.Count; i++)
            {
                Expression serialization_call = null;
                Expression deserialization_call = null;

                GenerateReferenceExpressions(type, instVariable, dataMembers[i], parts[i], out serialization_call,
                    out deserialization_call);

                serializeStatements.Add(serialization_call);
                deserializeStatements.Add(deserialization_call);
            }

            _serialize = Expression.Lambda<Action<object, Stream>>(Expression.Block(serializeStatements.ToArray()),
                "serialize_ref<" + type.FullName + ">", true,
                new[] {_parameterExpression, ParameterExpression}).Compile();

            _deserialize = Expression.Lambda<Func<Stream, object>>(
                    Expression.Block(new[] {instVariable},
                        new[] {Expression.Assign(instVariable, Expression.New(type))}
                            .Concat(deserializeStatements.ToArray())
                            .Concat(new[] {Expression.Convert(instVariable, typeof(Object))}).ToArray()),
                    "derialize_ref<" + type.FullName + ">", true,
                    new[] {ParameterExpression})
                .Compile();
        }


        private static void GenerateReferenceExpressions(Type type, Expression instVariable, MemberInfo property,
            ITypeInfo typeInfo,
            out Expression serialization_call, out Expression deserialization_call)
        {
            var prop = Expression.PropertyOrField(Expression.Convert(_parameterExpression, type), property.Name);
            var constPart = Expression.Constant(typeInfo);

            serialization_call = Expression.Call(constPart, TypeInfoSerialize, Expression.Convert(prop, typeof(Object)),
                ParameterExpression);

            deserialization_call = Expression.Assign(
                Expression.PropertyOrField(instVariable, property.Name),
                Expression.Convert(Expression.Call(constPart, TypeInfoDeserialize, ParameterExpression),
                    property.PropertyOrFieldType()));
        }
    }

    public static class MemberInfoExtension
    {
        public static Type PropertyOrFieldType(this MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Field)
            {
                var field = (FieldInfo)member;
                return field.FieldType;
            }

            if (member.MemberType == MemberTypes.Property)
            {
                var prop = (PropertyInfo)member;
                return prop.PropertyType;
            }

            return null;
        }

    }
}

