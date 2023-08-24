using System;
using System.Collections.Generic;
using System.Linq;
using Weather.Core.Entities.Abstract;

namespace Weather.Core.Entities
{
    public class Document : Entity
    {
        public Document(Guid id, string fileName, Dictionary<string, string> metadata, bool deleted) : base(id)
        {
            FileName = fileName;
            Metadata = metadata;
            Deleted = deleted;
        }

        public static Document Create(string fileName, Dictionary<string, string> metadata)
        {
            return new Document(Guid.NewGuid(), fileName, metadata, false);
        }

        public string FileName { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public bool Deleted { get; set; }

        private readonly List<DocumentVersion> _versions = new List<DocumentVersion>();
        public IReadOnlyCollection<DocumentVersion> Versions => _versions.AsReadOnly();

        public void AddVersion(long fileSize)
        {
            var version = DocumentVersion.Create(fileSize, DateTime.UtcNow, Id);
            _versions.Add(version);
        }

        public void UpdateVersion(Guid versionId, long fileSize)
        {
            _versions.Single(e => e.Id == versionId).Update(fileSize);
        }

        public void DeleteVersion(Guid versionId)
        {
            _versions.RemoveAll(e => e.Id == versionId);
        }

        public void Delete()
        {
            Deleted  = true;
        }
    }
}
