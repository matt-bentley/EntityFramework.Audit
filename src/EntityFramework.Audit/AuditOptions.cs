using System.Reflection;

namespace EntityFramework.Audit
{
    public class AuditOptions
    {
        public IReadOnlyDictionary<Type, PropertyInfo[]> AuditTypes;
    }
}
