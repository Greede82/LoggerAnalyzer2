using LoggerAnalyzer2.Data.Enums;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace LoggerAnalyzer2.Logger
{
    public class Logger(string name, string dataFolder) : IDisposable
    {
        public static Logger Instance { get; set; }

        private Thread? _writerThread;

        private const int SAVE_LOG_DAYS = 5;

        private readonly BlockingCollection<string> _logQueue = [];

        private bool _isDisposed = false;
        private bool _isProcessedLogQueue;

        private readonly string _dataFolder = dataFolder;
        private string? _lastFileName;


        private readonly Regex _archiveDatePattern = new($@"^{Regex.Escape(name)}_\d{{4}}-\d{{2}}-\d{{2}}\.log(\.gz)?$");

        private readonly string _name = name;
        private string CurrentFileName => $"{_name}_{DateTime.UtcNow:yyyy-MM-dd}.log";

        public void Run()
        {
            if (_isProcessedLogQueue)
            {
                return;
            }  
            _writerThread = new Thread(ProcessLogQueue) { IsBackground = true };
            _writerThread.Start();
        }

        public void Stop()
        {
            _logQueue.CompleteAdding();
        }

        private void ProcessLogQueue()
        {
            if (string.IsNullOrEmpty(_lastFileName))
                _lastFileName = CurrentFileName;

            _isProcessedLogQueue = true;
            StreamWriter? writer = null;
            try
            {
                foreach (var logData in _logQueue.GetConsumingEnumerable())
                {
                    try
                    {
                        if (CurrentFileName != _lastFileName)
                        {
                            writer?.Dispose();
                            ArchivateAndDelete(_lastFileName);
                            DeleteOldArchives();

                            _lastFileName = CurrentFileName;
                            var path = Path.Combine(_dataFolder, _lastFileName);
                            writer = new StreamWriter(path, true, Encoding.UTF8);
                        }

                        if (writer == null)
                        {
                            var path = Path.Combine(_dataFolder, _lastFileName);
                            writer = new StreamWriter(path, true, Encoding.UTF8);
                        }

                        writer.WriteLine($"{DateTime.UtcNow}: {logData}");

                        while (_logQueue.TryTake(out var additionalLog))
                        {
                            writer.WriteLine($"{DateTime.UtcNow}: {additionalLog}");
                        }

                        writer.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Log Write Error] {ex.Message}");
                        writer?.Dispose();
                        writer = null;
                    }
                }
            }
            finally
            {
                writer?.Dispose();
                _isProcessedLogQueue = false;
            }
        }

        internal void CheckDirState()
        {
            if (!Directory.Exists(_dataFolder))
            {
                Directory.CreateDirectory(_dataFolder);
            }
        }

        private static void Compress(string filename, string compressedFile)
        {
            using var sourceStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var targetStream = new FileStream(compressedFile, FileMode.Create);
            using GZipStream compressionStream = new(targetStream, CompressionMode.Compress);
            sourceStream.CopyTo(compressionStream);
        }

        private static void ArchivateAndDelete(string filename)
        {
            string compressedFile = filename + ".gz";

            if (!File.Exists(compressedFile))
            {
                try
                {
                    Compress(filename, compressedFile);
                    if (File.Exists(compressedFile))
                    {
                        File.Delete(filename);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        internal void DeleteOldArchives()
        {
            try
            {
                var validFiles = new List<string>();

                foreach (var filePath in Directory.EnumerateFiles(_dataFolder, "*.gz"))
                {
                    var fileName = Path.GetFileName(filePath);
                    if (_archiveDatePattern.IsMatch(fileName))
                    {
                        validFiles.Add(filePath);
                    }
                }

                validFiles.Sort((a, b) => File.GetLastWriteTimeUtc(b).CompareTo(File.GetLastWriteTimeUtc(a)));

                for (int i = SAVE_LOG_DAYS; i < validFiles.Count; i++)
                {
                    try
                    {
                        File.Delete(validFiles[i]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Log(LogType level, string message, [CallerFilePath] string filepath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string caller = "")
        {
            string logMessage = $"{DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss.fff} [{level}] [{caller}] {message} [{Path.GetFileName(filepath)}:{line}]";
            
            Console.WriteLine(logMessage);

            _logQueue.Add(logMessage);
        }

        public void Debug(string message) => Log(LogType.DEBUG, message);

        public void Information(string message) => Log(LogType.INFORMATION, message);

        public void Warning(string message) => Log(LogType.WARNING, message);

        public void Error(string message) => Log(LogType.ERROR, message);

        public void Critical(string message) => Log(LogType.CRITICAL, message);

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _writerThread?.Join();

            _isProcessedLogQueue = false;
            _isDisposed = true;
            _logQueue.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}