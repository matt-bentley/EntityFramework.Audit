using EntityFramework.Audit.Tests.Abstract;
using Microsoft.EntityFrameworkCore;
using Weather.Core.Entities;

namespace EntityFramework.Audit.Tests
{
    [TestClass]
    public class ComplexAuditTests : BaseDbContextTests
    {
        [TestMethod]
        public async Task GivenComplexEntity_WhenInsert_ThenInserted()
        {
            BuildDbContext(options =>
            {
                options.AuditEntity<Document>();
                options.AuditEntity<DocumentVersion>();
            });

            var document = Document.Create("test.xlsx", new Dictionary<string, string>()
            {
                ["test"] = "test"
            });
            document.AddVersion(10);
            Context.Documents.Add(document);
            await Context.SaveChangesAsync();

            var auditItems = await Context.Audit.ToListAsync();
            auditItems.Where(e => e.Action == AuditActions.Inserted).Should().HaveCount(2);
        }

        [TestMethod]
        public async Task GivenComplexEntity_WhenAddChild_ThenInserted()
        {
            BuildDbContext(options =>
            {
                options.AuditEntity<Document>();
                options.AuditEntity<DocumentVersion>();
            });

            var document = Document.Create("test.xlsx", new Dictionary<string, string>()
            {
                ["test"] = "test"
            });
            document.AddVersion(10);
            Context.Documents.Add(document);
            await Context.SaveChangesAsync();

            await ClearAuditAsync();

            document = await Context.Documents.FindAsync(document.Id);

            document.AddVersion(11);
            await Context.SaveChangesAsync();
            var auditItems = await Context.Audit.ToListAsync();
            auditItems.Should().HaveCount(1);
            auditItems.First().Action.Should().Be(AuditActions.Inserted);
        }

        [TestMethod]
        public async Task GivenComplexEntity_WhenUpdateChild_ThenUpdated()
        {
            BuildDbContext(options =>
            {
                options.AuditEntity<Document>();
                options.AuditEntity<DocumentVersion>();
            });

            var document = Document.Create("test.xlsx", new Dictionary<string, string>()
            {
                ["test"] = "test"
            });
            document.AddVersion(10);
            Context.Documents.Add(document);
            await Context.SaveChangesAsync();

            await ClearAuditAsync();

            document = await Context.Documents.FindAsync(document.Id);

            document.UpdateVersion(document.Versions.First().Id, 11);
            await Context.SaveChangesAsync();
            var auditItems = await Context.Audit.ToListAsync();
            auditItems.Should().HaveCount(1);
            auditItems.First().Action.Should().Be(AuditActions.Updated);
            auditItems.First().EntityType.Should().Be(typeof(DocumentVersion).Name);
        }

        [TestMethod]
        public async Task GivenComplexEntity_WhenDeleteChild_ThenDeleted()
        {
            BuildDbContext(options =>
            {
                options.AuditEntity<Document>();
                options.AuditEntity<DocumentVersion>();
            });

            var document = Document.Create("test.xlsx", new Dictionary<string, string>()
            {
                ["test"] = "test"
            });
            document.AddVersion(10);
            Context.Documents.Add(document);
            await Context.SaveChangesAsync();

            await ClearAuditAsync();

            document = await Context.Documents.FindAsync(document.Id);

            document.DeleteVersion(document.Versions.First().Id);
            await Context.SaveChangesAsync();
            var auditItems = await Context.Audit.ToListAsync();
            auditItems.Should().HaveCount(1);
            auditItems.First().Action.Should().Be(AuditActions.Deleted);
            auditItems.First().EntityType.Should().Be(typeof(DocumentVersion).Name);
        }
    }
}