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

        private Expression<Action<object, Stream>> _serializExpression;
        private Expression<Action<object, Stream>> _deserializeExpression;

        public Expression StreamExpression { get; set; }
        public Expression ObjectExpresstion { get; set; }
        public Expression SerializeExpression { get; set; }
        public Expression DeserializeExpression { get; set; }

        private Action<object, Stream> _serialize;
        private Func<Stream, object> _deserialize;


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

            var dataMembers = new List<MemberInfo>();

            foreach (var memberInfo in type.GetMembers())
            {
                if (memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property)
                    continue;

                if (memberInfo.MemberType == MemberTypes.Property && !((PropertyInfo)memberInfo).CanWrite && !((PropertyInfo)memberInfo).CanRead)
                    continue;

                dataMembers.Add(memberInfo);
            }


            var parts = new List<ITypeInfo>();

            foreach (var dataMember in dataMembers)
            {
                Type memberType = null;
                if (dataMember.MemberType == MemberTypes.Field)
                {
                    var field = (FieldInfo)dataMember;
                    memberType = field.FieldType;
                }

                if (dataMember.MemberType == MemberTypes.Property)
                {
                    var prop = (PropertyInfo)dataMember;
                    memberType = prop.PropertyType;
                }

                var newInstance = new ExpressionTypeInfo(memberType, manager);

                parts.Add(manager.TypeInfoCache[memberType]);
            }

            var serializeStatements = new List<Expression>();
            var deserializeStatements = new List<Expression>();

            var sourceArgument = Expression.Parameter(typeof(Object), "source");
            var streamArgument = Expression.Parameter(typeof(Stream), "stream");

            var typeInfoSerialize = typeof(ITypeInfo).GetMethod("Serialize");
            var typeInfoDeserialize = typeof(ITypeInfo).GetMethod("Deserialize");

            var instVariable = Expression.Variable(type, "instance");

            for (int i = 0; i < dataMembers.Count; i++)
            {
                var prop = Expression.PropertyOrField(Expression.Convert(sourceArgument, type), dataMembers[i].Name);
                var constPart = Expression.Constant(parts[i]);

                serializeStatements.Add(
                    Expression.Call(constPart, typeInfoSerialize, Expression.Convert(prop, typeof(Object)),
                        streamArgument)
                );

                deserializeStatements.Add(Expression.Assign(Expression.PropertyOrField(instVariable, dataMembers[i].Name),
                    Expression.Convert(Expression.Call(constPart, typeInfoDeserialize, streamArgument), parts[i].TargetType)));
            }

            _serializExpression =
                Expression.Lambda<Action<object, Stream>>(Expression.Block(serializeStatements.ToArray()),
                    sourceArgument, streamArgument);

            _serialize = _serializExpression.Compile();

            _deserialize = Expression.Lambda<Func<Stream, object>>(
                    Expression.Block(new[] { instVariable },
                        new[] { Expression.Assign(instVariable, Expression.New(type)) }
                            .Concat(deserializeStatements.ToArray())
                            .Concat(new[] { Expression.Convert(instVariable, typeof(Object)) }).ToArray()),
                    streamArgument)
                .Compile();

            manager.TypeInfoCache[type] = this;
        }

    }
}
