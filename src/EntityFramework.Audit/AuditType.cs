using System.Reflection;

namespace EntityFramework.Audit
{
    public sealed class AuditType
    {
        public readonly Type Type;
        public readonly PropertyInfo[] Properties;
        public readonly AuditActionFlags Actions;

        internal AuditType(Type type, PropertyInfo[] properties, AuditActionFlags actions)
        {
            Type = type;
            Properties = properties;
            Actions = actions;
        }
    }
}
