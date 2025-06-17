namespace Nerv.Repository.LogEntities;

public class ChangeLog
{
    public string Table { get; set; } = string.Empty;

    public string Column { get; set; } = string.Empty;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}