using System;
using Weather.Core.Entities.Abstract;

namespace Weather.Core.Entities
{
    public class DocumentVersion : Entity
    {
        public DocumentVersion(Guid id, long fileSize, DateTime createdDate, Guid documentId) : base(id)
        {
            FileSize = fileSize;
            CreatedDate = createdDate;
            DocumentId = documentId;
        }

        public static DocumentVersion Create(long fileSize, DateTime createdDate, Guid documentId)
        {
            return new DocumentVersion(Guid.NewGuid(), fileSize, createdDate, documentId);
        }

        public long FileSize { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid DocumentId { get; set; }
        public Document Document { get; set; }

        public void Update(long fileSize)
        {
            FileSize = fileSize;
        }
    }
}
