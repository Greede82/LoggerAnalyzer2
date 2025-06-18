using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using LoggerAnalyzer2.Models;

namespace LoggerAnalyzer2.Pages
{
    public class LogTableModel : PageModel
    {
        public List<LogEntry> Logs { get; private set; } = [];
        public List<LogEntry> FilteredLogs { get; private set; } = [];
        public string DebugInfo { get; private set; } = string.Empty;

        public int DebugCount { get; private set; } = 0;
        public int InfoCount { get; private set; } = 0;
        public int WarningCount { get; private set; } = 0;
        public int ErrorCount { get; private set; } = 0;
        public int CriticalCount { get; private set; } = 0;
        public int TotalCount { get; private set; } = 0;

        [BindProperty(SupportsGet = true)]
        public string SearchText { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public List<string> SearchColumns { get; set; } = [];
        
        [BindProperty(SupportsGet = true)]
        public List<string> FileNameFilters { get; set; } = [];
        
        [BindProperty(SupportsGet = true)]
        public List<string> ClassNameFilters { get; set; } = [];
        
        [BindProperty(SupportsGet = true)]
        public List<string> MethodNameFilters { get; set; } = [];
        
        [BindProperty(SupportsGet = true)]
        public List<string> LogTypeFilters { get; set; } = [];
        
        [BindProperty(SupportsGet = true)]
        public List<string> MessageFilters { get; set; } = [];
        
        [BindProperty(SupportsGet = true)]
        public string ExpandedFilter { get; set; } = string.Empty;
        
        public List<string> UniqueFileNames { get; private set; } = [];
        public List<string> UniqueClassNames { get; private set; } = [];
        public List<string> UniqueMethodNames { get; private set; } = [];
        public List<string> UniqueLogTypes { get; private set; } = [];
        public List<string> UniqueMessages { get; private set; } = [];

        public IActionResult OnGet(bool isPartial = false)
        {
            LoadData();
            
            if (isPartial)
            {
                return Partial("_LogTablePartial", this);
            }
            
            return Page();
        }
        
        public IActionResult OnPostToggleFilter(string filterName, bool isPartial = false)
        {
            ExpandedFilter = ExpandedFilter == filterName ? "" : filterName;
            LoadData();
            
            if (isPartial)
            {
                return Partial("_LogTablePartial", this);
            }
            
            return Page();
        }
        
        private new IActionResult Partial(string viewName, object model)
        {
            return new ContentResult
            {
                ContentType = "text/html",
                Content = RenderPartialViewToString(viewName, model),
                StatusCode = 200
            };
        }
        
        private string RenderPartialViewToString(string viewName, object model)
        {
            var html = "<link rel=\"stylesheet\" href=\"/css/logtable.css\" />\n";
            html += "<link rel=\"stylesheet\" href=\"/css/sidebar.css\" />\n";
            html += "<link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css\" />\n";
            
            html += "<div class=\"log-table-container\">";
            
            if (FilteredLogs.Any())
            {
                html += "<table class=\"log-table\">";
                html += "<thead>";
                html += "<tr>";
                html += "<th class=\"log-table-filename\">Название файла</th>";
                html += "<th class=\"log-table-class\">Класс</th>";
                html += "<th class=\"log-table-method\">Метод</th>";
                html += "<th class=\"log-table-type\">Тип</th>";
                html += "<th class=\"log-table-message\">Сообщение</th>";
                html += "</tr>";
                html += "</thead>";
                html += "<tbody>";
                
                foreach (var log in FilteredLogs)
                {
                    html += "<tr>";
                    
                    string fileNameClass = log.MatchedFields.Contains("FileName") || FileNameFilters.Contains(log.FileName) ? "highlighted-cell" : "";
                    html += $"<td class=\"{fileNameClass}\">";
                    

                    if (log.MatchedFields.Contains("FileName") && !string.IsNullOrEmpty(log.SearchText))
                    {
                        html += HighlightSearchTerms(log.FileName ?? "", log.SearchText);
                    }
                    else
                    {
                        html += log.FileName;
                    }
                    html += "</td>";
                    
                    string classNameClass = log.MatchedFields.Contains("ClassName") || ClassNameFilters.Contains(log.ClassName) ? "highlighted-cell" : "";
                    html += $"<td class=\"{classNameClass}\">";
                    if (log.MatchedFields.Contains("ClassName") && !string.IsNullOrEmpty(log.SearchText))
                    {
                        html += HighlightSearchTerms(log.ClassName ?? "", log.SearchText);
                    }
                    else
                    {
                        html += log.ClassName;
                    }
                    html += "</td>";
                    
                    string methodNameClass = log.MatchedFields.Contains("MethodName") || MethodNameFilters.Contains(log.MethodName) ? "highlighted-cell" : "";
                    html += $"<td class=\"{methodNameClass}\">";
                    if (log.MatchedFields.Contains("MethodName") && !string.IsNullOrEmpty(log.SearchText))
                    {
                        html += HighlightSearchTerms(log.MethodName ?? "", log.SearchText);
                    }
                    else
                    {
                        html += log.MethodName;
                    }
                    html += "</td>";
                    
                    string logTypeClass = log.MatchedFields.Contains("LogText") || LogTypeFilters.Contains(log.LogText) ? "highlighted-cell" : "";
                    html += $"<td class=\"{logTypeClass}\">";
                    
                    if (log.MatchedFields.Contains("LogText") && !string.IsNullOrEmpty(log.SearchText))
                    {
                        html += HighlightSearchTerms(log.LogText ?? "", log.SearchText);
                    }
                    else
                    {
                        html += log.LogText;
                    }
                    html += "</td>";
                    
                    string messageClass = log.MatchedFields.Contains("Message") || MessageFilters.Contains(log.Message) ? "highlighted-cell" : "";
                    html += $"<td class=\"{messageClass}\">";
                    
                    if (log.MatchedFields.Contains("Message") && !string.IsNullOrEmpty(log.SearchText))
                    {
                        html += HighlightSearchTerms(log.Message ?? "", log.SearchText);
                    }
                    else
                    {
                        html += log.Message;
                    }
                    
                    html += "</td>";
                    html += "</tr>";
                }
                
                html += "</tbody>";
                html += "</table>";
            }
            else
            {
                html += "<div class=\"alert alert-info mt-3 no-data-message\">";
                html += "<p>Нет данных для отображения. Попробуйте изменить критерии поиска.</p>";
                html += "<div class=\"text-center mt-3\">";
                html += "<a href=\"/LogTable\" class=\"btn btn-primary btn-lg\">Назад</a>";
                html += "</div>";
                html += "</div>";
            }
            
            html += "</div>";
            
            return html;
        }
        
        public string HighlightSearchTerms(string input, string searchTermsString)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(searchTermsString))
                return input;
                
            var searchTerms = searchTermsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(term => term.Trim())
                .Where(term => !string.IsNullOrWhiteSpace(term))
                .ToList();
                
            if (!searchTerms.Any())
                return input;
                
            string result = input;
            
            foreach (var term in searchTerms)
            {
                int startIndex = 0;
                while (startIndex < result.Length)
                {
                    int index = result.IndexOf(term, startIndex, StringComparison.OrdinalIgnoreCase);
                    if (index < 0) break;
                    
                    string before = result.Substring(0, index);
                    string match = result.Substring(index, term.Length);
                    string after = result.Substring(index + term.Length);
                    
                    result = $"{before}<span class=\"highlight-match\">{match}</span>{after}";
                    

                    startIndex = before.Length + match.Length + 35;
                }
            }
            
            return result;
        }

        private void LoadData()
        {
            try
            {
                string jsonContent;
                try {
                    jsonContent = LoggerAnalyzer2.LogJson.Json;
                }
                catch {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "generated_log_info.json");
                    if (System.IO.File.Exists(filePath)) {
                        jsonContent = System.IO.File.ReadAllText(filePath);
                    }
                    else {
                        DebugInfo = $"Файл {filePath} не найден.";
                        FilteredLogs = [];
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(jsonContent))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var logData = JsonSerializer.Deserialize<LogInfoList>(jsonContent, options);

                    if (logData != null && logData.Logs != null)
                    {
                        Logs = logData.Logs;
                        
                        UniqueFileNames = Logs.Select(l => l.FileName ?? "").Distinct().OrderBy(f => f).ToList();
                        UniqueClassNames = Logs.Select(l => l.ClassName ?? "").Distinct().OrderBy(c => c).ToList();
                        UniqueMethodNames = Logs.Select(l => l.MethodName ?? "").Distinct().OrderBy(m => m).ToList();
                        UniqueLogTypes = Logs.Select(l => l.LogText ?? "").Distinct().OrderBy(t => t).ToList();
                        
                        FilteredLogs = FilterLogs();
                        CalculateLogCounts();
                    }
                }
            }
            catch (Exception ex)
            {
                DebugInfo = ex.Message;
            }
        }
        
        private List<LogEntry> FilterLogs()
        {
            if (Logs == null || !Logs.Any())
                return [];

            var result = Logs.ToList();
            
            if (FileNameFilters.Any())
                result = result.Where(l => FileNameFilters.Contains(l.FileName)).ToList();

            if (ClassNameFilters.Any())
                result = result.Where(l => ClassNameFilters.Contains(l.ClassName)).ToList();

            if (MethodNameFilters.Any())
                result = result.Where(l => MethodNameFilters.Contains(l.MethodName)).ToList();

            if (LogTypeFilters.Any())
                result = result.Where(l => LogTypeFilters.Contains(l.LogText)).ToList();

            if (!string.IsNullOrEmpty(SearchText))
            {
                var searchTerms = SearchText.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(term => term.Trim().ToLower())
                    .Where(term => !string.IsNullOrWhiteSpace(term))
                    .ToList();
                
                if (searchTerms.Any())
                {
                    var searchResults = new List<LogEntry>();
                    var columnsToSearch = SearchColumns.Any() ? SearchColumns : new List<string> { "FileName", "ClassName", "MethodName", "LogText", "Message" };

                    foreach (var log in result)
                    {
                        bool anyTermMatched = false;
                        log.MatchedFields = [];
                        log.SearchText = SearchText;

                        foreach (var term in searchTerms)
                        {
                            foreach (var column in columnsToSearch)
                            {
                                var propertyValue = GetPropertyValue(log, column);
                                if (propertyValue != null && propertyValue.ToLower().Contains(term))
                                {
                                    anyTermMatched = true;
                                    if (!log.MatchedFields.Contains(column))
                                    {
                                        log.MatchedFields.Add(column);
                                    }
                                }
                            }
                        }

                        if (anyTermMatched)
                            searchResults.Add(log);
                    }

                    result = searchResults;
                }
            }
            else
            {
                foreach (var log in result)
                {
                    log.MatchedFields = [];
                    log.SearchText = string.Empty;
                }
            }
            
            return result;
        }

        private string GetPropertyValue(LogEntry log, string propertyName)
        {
            return propertyName switch
            {
                "FileName" => log.FileName ?? string.Empty,
                "ClassName" => log.ClassName ?? string.Empty,
                "MethodName" => log.MethodName ?? string.Empty,
                "LogText" => log.LogText ?? string.Empty,
                "Message" => log.Message ?? string.Empty,
                _ => string.Empty
            };
        }

        private void CalculateLogCounts()
        {
            DebugCount = 0;
            InfoCount = 0;
            WarningCount = 0;
            ErrorCount = 0;
            CriticalCount = 0;

            foreach (var log in FilteredLogs)
            {
                if (log.LogText != null)
                {
                    switch (log.LogText.ToUpper())
                    {
                        case "DEBUG":
                            DebugCount++;
                            break;
                        case "INFORMATION":
                        case "INFO":
                            InfoCount++;
                            break;
                        case "WARNING":
                            WarningCount++;
                            break;
                        case "ERROR":
                            ErrorCount++;
                            break;
                        case "CRITICAL":
                            CriticalCount++;
                            break;
                    }
                }
            }

            TotalCount = DebugCount + InfoCount + WarningCount + ErrorCount + CriticalCount;
        }
    }
}
