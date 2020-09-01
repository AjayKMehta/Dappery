using System;

namespace Dappery.Domain.Entities
{
    public class TimeStampedEntity
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
