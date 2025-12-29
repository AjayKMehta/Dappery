using System;
using System.Data;
using System.Globalization;

using Dapper;

namespace Dappery.Data;

// https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/dapper-limitations
public abstract class SqliteTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    // Parameters are converted by Microsoft.Data.Sqlite
    public override void SetValue(IDbDataParameter parameter, T? value)
        => parameter.Value = value;
}

internal sealed class DateTimeOffsetHandler : SqliteTypeHandler<DateTimeOffset>
{
    public override DateTimeOffset Parse(object value)
        => DateTimeOffset.Parse((string)value, CultureInfo.InvariantCulture);
}

internal sealed class GuidHandler : SqliteTypeHandler<Guid>
{
    public override Guid Parse(object value)
        => Guid.Parse((string)value);
}

internal sealed class TimeSpanHandler : SqliteTypeHandler<TimeSpan>
{
    public override TimeSpan Parse(object value)
        => TimeSpan.Parse((string)value, CultureInfo.InvariantCulture);
}
