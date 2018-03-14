using System;

namespace Freud
{
    public class ReflectionTypeInfoFactory : ITypeInfoFactory
    {
        public ITypeInfo Create(Type type, FreudManager manager)
        {
            return new ReflectionTypeInfo(type, manager);
        }
    }
}
