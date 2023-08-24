using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using Weather.Core.Entities;

namespace Weather.Infrastructure.Configurations
{
    internal class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.Property(e => e.FileName)
                   .HasColumnType("varchar(256)")
                   .IsRequired();

            builder.HasMany(e => e.Versions)
                .WithOne(e => e.Document)
                .HasForeignKey(e => e.DocumentId)
                .IsRequired();

            Func<Dictionary<string, string>, Dictionary<string, string>, bool> equalityComparer = (left, right) =>
            {
                return left?.Keys.Count == right?.Keys.Count
                    && left.Keys.All(key => right.TryGetValue(key, out var value) && left[key] == value);
            };

            Func<Dictionary<string, string>, int> hashCodeGenerator = (metadata) =>
            {
                return metadata.GetHashCode();
            };

            var serializerOptions = new JsonSerializerOptions();
            var comparer = new ValueComparer<Dictionary<string, string>>(
                (l, r) => equalityComparer.Invoke(l, r),
                events => hashCodeGenerator.Invoke(events),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(JsonSerializer.Serialize(v, serializerOptions), serializerOptions));

            var converter = new ValueConverter<Dictionary<string, string>, string>(
                v => JsonSerializer.Serialize(v, serializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, serializerOptions));

            var metadataProperty = builder.Property(e => e.Metadata)
                                        .IsUnicode(false)
                                        .HasConversion(converter)
                                        .HasDefaultValue(new Dictionary<string, string>())
                                        .IsRequired();

            metadataProperty.Metadata.SetValueConverter(converter);
            metadataProperty.Metadata.SetValueComparer(comparer);
        }
    }

    internal class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
    {
        public void Configure(EntityTypeBuilder<DocumentVersion> builder)
        {
            
        }
    }
}
