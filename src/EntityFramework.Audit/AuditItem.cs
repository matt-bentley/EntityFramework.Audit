using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Audit
{
    public sealed class AuditItem
    {
        [Key]
        public Guid Id { get; set; }

        public Guid EntityId { get; set; }

        [Required]
        [StringLength(50)]
        public string EntityType { get; set; }

        [Required]
        [StringLength(15)]
        public string Action { get; set; }

        public string Data { get; set; }

        public DateTimeOffset Time { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }
    }
}
