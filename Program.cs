using LoggerAnalyzer2.Logger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

string baseDir = AppDomain.CurrentDomain.BaseDirectory;
string projectPath = Path.GetFullPath(Path.Combine(baseDir, "..", ".."));
string dataFolder = Path.Combine(projectPath, "Logs");

Logger.Instance = new Logger(baseDir, dataFolder);
Logger.Instance.CheckDirState();
Logger.Instance.Run();
var logger = Logger.Instance;
builder.Services.AddSingleton(logger);
var app = builder.Build();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
Thread inputThread = new(() => ProcessUserInput(logger, lifetime))
{

    IsBackground = true
};
inputThread.Start();
app.MapGet("/", () =>
{
    logger.Information("Запрос получен");
    logger.Debug("Запрос обработан");
    logger.Error("ОШИБКА");
    logger.Critical("Включение программы");
    logger.Warning("Предупреждение");
    logger.Information("Запрос получен");
    logger.Debug("Запрос обработан");
    logger.Error("ОШИБКА");
    logger.Critical("Включение программы");
    logger.Warning("Предупреждение");
    return Results.Redirect("/LogTable");
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();

app.Lifetime.ApplicationStopping.Register(() => ((IDisposable)logger).Dispose());

app.Run();
logger.Information("User clicked Submit");

static void ProcessUserInput(Logger logger, IHostApplicationLifetime lifetime)
{
    while (true)
    {
        Console.Write("Введите команду: ");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            continue;

        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            lifetime.StopApplication();
            return;
        }

        string[] parts = input.Split('.', 2);
        if (parts.Length < 2)
        {
            Console.WriteLine("error use /...");
            continue;
        }

        string command = parts[0].Trim().ToLower();
        string message = parts[1].Trim();

        switch (command)
        {
            case "/debug":
                logger.Debug(message);
                break;
            case "/info":
                logger.Information(message);
                break;
            case "/warn":
                logger.Warning(message);
                break;
            case "/error":
                logger.Error(message);
                break;
            case "/critical":
                logger.Critical($"Critical {Exception.Equals}");
                break;
            case "/exit":
                Console.WriteLine("Exit...");
                lifetime.StopApplication();
                return;
            default:
                Console.WriteLine("Use /debug, /info, /warn, /error, /critical");
                break;
        }
    }
}