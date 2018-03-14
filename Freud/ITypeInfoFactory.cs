using System;

namespace Freud
{
    public interface ITypeInfoFactory
    {
        ITypeInfo Create(Type type, FreudManager manager);
    }
}