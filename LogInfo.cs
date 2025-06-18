using LoggerAnalyzer2.Data.Enums;

public sealed class LogUsingInfoList
{
    public required List<LogUsingInfo> Logs { get; set; }
}

public sealed record LogUsingInfo
{
    public required string ClassName { get; init; }
    public required string MethodName { get; init; }
    public required int Row { get; init; }
    public string LogType { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
}