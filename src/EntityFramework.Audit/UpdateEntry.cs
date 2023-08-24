
namespace EntityFramework.Audit
{
    public sealed record UpdateEntry(string ColumnName, object OriginalValue, object NewValue);
}
