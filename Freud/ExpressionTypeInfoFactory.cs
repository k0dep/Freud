using System;

namespace Freud
{
    public class ExpressionTypeInfoFactory : ITypeInfoFactory
    {
        public ITypeInfo Create(Type type, FreudManager manager)
        {
            return new ExpressionTypeInfo(type, manager);
        }
    }
}