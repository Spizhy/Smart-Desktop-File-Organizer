using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace SmartFileOrganizer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private FileSystemWatcher _watcher;

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var watchDir = _config["OrganizerSettings:WatchDirectory"];
            if (!Directory.Exists(watchDir))
            {
                _logger.LogError($"Directory does not exist: {watchDir}");
                return Task.CompletedTask;
            }

            _watcher = new FileSystemWatcher(watchDir)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };

            _watcher.Created += OnFileCreated;
            _logger.LogInformation($"Started watching: {watchDir}");

            return Task.CompletedTask;
        }

        private async void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            // Небольшая задержка, чтобы файл успел полностью загрузиться
            await Task.Delay(1000); 

            try
            {
                var extension = Path.GetExtension(e.FullPath).ToLower();
                var rules = _config.GetSection("OrganizerSettings:Rules").Get<Dictionary<string, string>>();

                if (rules != null && rules.TryGetValue(extension, out string targetFolder))
                {
                    var watchDir = _config["OrganizerSettings:WatchDirectory"];
                    var targetDirPath = Path.Combine(watchDir, targetFolder);
                    
                    if (!Directory.Exists(targetDirPath))
                    {
                        Directory.CreateDirectory(targetDirPath);
                    }

                    var destFile = Path.Combine(targetDirPath, e.Name);
                    
                    // Если файл с таким именем уже есть, добавляем timestamp
                    if (File.Exists(destFile))
                    {
                        destFile = Path.Combine(targetDirPath, 
                            $"{Path.GetFileNameWithoutExtension(e.Name)}_{DateTime.Now.Ticks}{extension}");
                    }

                    File.Move(e.FullPath, destFile);
                    _logger.LogInformation($"Moved {e.Name} to {targetFolder}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing file {e.Name}: {ex.Message}");
            }
        }

        public override void Dispose()
        {
            _watcher?.Dispose();
            base.Dispose();
        }
    }
}
