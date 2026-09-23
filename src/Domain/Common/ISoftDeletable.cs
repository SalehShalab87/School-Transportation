namespace Domain.Common;

/// <summary>
/// Opt-in marker. Do not put soft-delete on immutable history/event tables.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; }
}
