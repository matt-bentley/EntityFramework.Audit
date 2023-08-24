
namespace EntityFramework.Audit
{
    internal static class AuditActions
    {
        public const string Inserted = "Inserted";
        public const string Updated = "Updated";
        public const string Deleted = "Deleted";
    }

    [Flags]
    public enum AuditActionFlags : short
    {
        Inserted,
        Updated,
        Deleted
    }
}
