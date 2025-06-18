using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LoggerAnalyzer2.Models
{
    public class LogEntry
    {
        public string LogText { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public int Row { get; set; }
        public int LogType { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }

        [JsonIgnore]
        public List<string> MatchedFields { get; set; } = [];

        [JsonIgnore]
        public string SearchText { get; set; } = string.Empty;
    }
}
