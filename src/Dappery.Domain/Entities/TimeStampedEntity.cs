using System;

namespace Dappery.Domain.Entities
{
    public record TimeStampedEntity
    {
        public int Id { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; init; }
    }
}
