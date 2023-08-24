using System.Reflection;

namespace EntityFramework.Audit
{
    public class AuditType
    {
        public readonly Type Type;
        public readonly PropertyInfo[] Properties;

        internal AuditType(Type type, PropertyInfo[] properties)
        {
            Type = type;
            Properties = properties;
        }
    }
}
