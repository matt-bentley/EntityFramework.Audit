
namespace EntityFramework.Audit
{
    public sealed class AuditOptions
    {
        public IReadOnlyDictionary<Type, AuditType> AuditTypes;

        public bool TryGetAuditType(Type type, AuditActionFlags action, out AuditType auditType)
        {
            return AuditTypes.TryGetValue(type, out auditType)
                && auditType.Actions.HasFlag(action);
        }
    }
}
