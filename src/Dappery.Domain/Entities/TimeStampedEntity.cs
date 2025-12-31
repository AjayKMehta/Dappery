using System;

namespace Dappery.Domain.Entities;

public class TimeStampedEntity
{
    public int Id { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset UpdatedAt { get; set; }
}
