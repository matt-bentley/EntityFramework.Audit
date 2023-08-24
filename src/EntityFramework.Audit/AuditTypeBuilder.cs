using System.Linq.Expressions;
using System.Reflection;

namespace EntityFramework.Audit
{
    public interface IAuditTypeBuilder
    {
        AuditType Build();
    }

    public interface IAuditTypeBuilder<TEntity> : IAuditTypeBuilder where TEntity : class
    {
        AuditTypeBuilder<TEntity> Ignore<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression);
    }

    public class AuditTypeBuilder<TEntity> : IAuditTypeBuilder<TEntity> where TEntity : class
    {
        private readonly HashSet<PropertyInfo> _ignoreProperties = new HashSet<PropertyInfo>();

        public AuditTypeBuilder<TEntity> Ignore<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            var property = GetPropertyFromExpression(propertyExpression);
            _ignoreProperties.Add(property);
            return this;
        }

        public AuditType Build()
        {
            var type = typeof(TEntity);
            var properties = type
                             .GetProperties()
                             .Where(e => !_ignoreProperties.Contains(e))
                             .ToArray();
            return new AuditType(type, properties);   
        }

        private static PropertyInfo GetPropertyFromExpression<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            MemberExpression member;

            //this line is necessary, because sometimes the expression comes in as Convert(originalexpression)
            if (propertyExpression.Body is UnaryExpression)
            {
                var UnExp = (UnaryExpression)propertyExpression.Body;
                if (UnExp.Operand is MemberExpression)
                {
                    member = (MemberExpression)UnExp.Operand;
                }
                else
                    throw new ArgumentException();
            }
            else if (propertyExpression.Body is MemberExpression)
            {
                member = (MemberExpression)propertyExpression.Body;
            }
            else
            {
                throw new ArgumentException();
            }

            return (PropertyInfo)member.Member;
        }
    }
}
