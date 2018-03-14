using System.Linq.Expressions;

namespace Freud
{
    public interface IExpressionTypeInfo
    {
        Expression StreamExpression { get; set; }
        Expression ObjectExpresstion { get; set; }
        Expression SerializeExpression { get; set; }
        Expression DeserializeExpression { get; set; }
    }
}