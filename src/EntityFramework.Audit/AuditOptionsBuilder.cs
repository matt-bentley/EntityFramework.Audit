
namespace EntityFramework.Audit
{
    public class AuditOptionsBuilder
    {
        internal string DefaultUser { get; private set; }
        internal Type IdentityProvider { get; private set; }
        private readonly List<IAuditTypeBuilder> _typeBuilders = new List<IAuditTypeBuilder>();

        internal AuditOptionsBuilder()
        {
            
        }

        public AuditOptionsBuilder UseDefaultUser(string user)
        {
            DefaultUser = user;
            return this;
        }

        public AuditOptionsBuilder UseIdentityProvider<T>() where T : IAuditIdentityProvider
        {
            IdentityProvider = typeof(T);
            return this;
        }

        public IAuditTypeBuilder<TEntity> AuditEntity<TEntity>() where TEntity : class
        {
            var typeBuilder = new AuditTypeBuilder<TEntity>();
            _typeBuilders.Add(typeBuilder);
            return typeBuilder;
        }

        internal AuditOptions Build()
        {
            var auditTypes = new Dictionary<Type, AuditType>();
            foreach (var builder in _typeBuilders)
            {
                var auditType = builder.Build();
                auditTypes.Add(auditType.Type, auditType);
            }
            return new AuditOptions()
            {
                AuditTypes = auditTypes
            };
        }
    }
}
