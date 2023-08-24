using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace EntityFramework.Audit
{
    public sealed class AuditTracker<T> : IAuditTracker<T> where T : DbContext, IAuditDbContext
    {
        private readonly IAuditIdentityProvider _identityProvider;
        private readonly AuditOptions _options;
        private T _context;
        public bool IsTracking => _context != null;
        private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles };

        public AuditTracker(IAuditIdentityProvider identityProvider,
            AuditOptions options)
        {
            _identityProvider = identityProvider;
            _options = options;
        }

        public void StartTracking(T context)
        {
            _context = context;
        }

        public void TrackChanges()
        {
            if(!IsTracking)
            {
                throw new InvalidOperationException($"Tracking has not started using {nameof(StartTracking)}");
            }
            var timestamp = DateTime.UtcNow;
            TrackInserted(timestamp);
            TrackUpdated(timestamp);
            TrackDeleted(timestamp);
        }

        private void TrackInserted(DateTime timestamp)
        {
            foreach (var entry in GetEntries(EntityState.Added))
            {
                if(_options.AuditTypes.TryGetValue(entry.Metadata.ClrType, out var properties))
                {
                    var json = new JsonObject();
                    foreach (var property in properties)
                    {
                        if(!IsAudited(property.PropertyType))
                        {
                            if (!IsChildCollection(property.PropertyType))
                            {
                                json.Add(property.Name, JsonValue.Create(property.GetValue(entry.Entity)));
                            }
                        }
                    }
                    _context.Audit.Add(new AuditItem()
                    {
                        EntityId = GetPrimaryKey(entry),
                        Action = AuditActions.Inserted,
                        Time = timestamp,
                        EntityType = entry.Entity.GetType().Name,
                        Data = JsonSerializer.Serialize(json, _jsonSerializerOptions),
                        UserName = _identityProvider.GetUser()
                    });
                }
            }
        }

        private bool IsChildCollection(Type propertyType)
        {
            return propertyType.GetInterfaces()
                               .Where(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                               .Any(e => e.GetGenericArguments().Any(a => IsAudited(a)));
        }

        private void TrackUpdated(DateTime timestamp)
        {
            foreach (var entry in GetEntries(EntityState.Modified))
            {
                if (_options.AuditTypes.TryGetValue(entry.Metadata.ClrType, out var properties))
                {
                    var modifiedMembers = entry.Members
                                               .Where(e => e.IsModified)
                                               .OfType<PropertyEntry>()
                                               .Where(e => properties.Any(p => p.Name.Equals(e.Metadata.Name)));
                    var updateEntries = new List<UpdateEntry>(properties.Length); // the array will never be longer than the auditable properties
                    foreach (var member in modifiedMembers)
                    {
                        updateEntries.Add(new UpdateEntry(member.Metadata.Name, member.OriginalValue, member.CurrentValue));
                    }
                    _context.Audit.Add(new AuditItem()
                    {
                        EntityId = GetPrimaryKey(entry),
                        Action = AuditActions.Updated,
                        Time = timestamp,
                        EntityType = entry.Entity.GetType().Name,
                        Data = JsonSerializer.Serialize(updateEntries, _jsonSerializerOptions),
                        UserName = _identityProvider.GetUser()
                    });
                }
            }
        }

        private void TrackDeleted(DateTime timestamp)
        {
            foreach (var entry in GetEntries(EntityState.Deleted))
            {
                if(IsAudited(entry.Metadata.ClrType))
                {
                    _context.Audit.Add(new AuditItem()
                    {
                        EntityId = GetPrimaryKey(entry),
                        Action = AuditActions.Deleted,
                        Time = timestamp,
                        EntityType = entry.Entity.GetType().Name,
                        UserName = _identityProvider.GetUser()
                    });
                }
            }
        }

        private List<EntityEntry> GetEntries(EntityState state)
        {
            return _context.ChangeTracker
                .Entries()
                .Where(p => p.State == state)
                .ToList();
        }

        private static Guid GetPrimaryKey(EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey()
             .Properties
             .Select(p => entry.Property(p.Name).CurrentValue)
             .FirstOrDefault();
            return (Guid)key;
        }

        private bool IsAudited(Type type)
        {
            return _options.AuditTypes.ContainsKey(type);
        }
    }
}
