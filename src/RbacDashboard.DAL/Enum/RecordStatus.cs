
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Enum;

public enum RecordStatus
{
    Active,
    Inactive,
    Delete
}

[ExcludeFromCodeCoverage(Justification = "No need to be included in code coverage.")]
public static class RecordStatusExtensions
{
    public static string ToStatusString(this RecordStatus status)
    {
        return status switch
        {
            RecordStatus.Active => "Activate",
            RecordStatus.Inactive => "Deactivate",
            RecordStatus.Delete => "Delete",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}