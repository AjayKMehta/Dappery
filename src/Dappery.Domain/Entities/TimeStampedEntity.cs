using System;

namespace Dappery.Domain.Entities;

public class TimeStampedEntity
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
