using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SourceWrench.Core.Services;

namespace SourceWrench.Core;

public abstract class BaseExtractor : IExtractor
{
    protected readonly IVpkReader _vpkReader;
    protected readonly ILogger _logger;
    protected readonly IProgressReporter _progressReporter;

    protected BaseExtractor(IVpkReader iVpkReader, ILogger logger, IProgressReporter progressReporter)
    {
        _vpkReader = iVpkReader ?? throw new ArgumentNullException(nameof(IVpkReader));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _progressReporter = progressReporter ?? throw new ArgumentNullException(nameof(progressReporter));
    }

    public abstract Task<int> ExtractAsync(string vpkPath, string outputPath, CancellationToken cancellationToken = default);

    protected IEnumerable<string> GetFilesByExtension(string[] extensions)
    {
        if (_vpkReader == null) return Enumerable.Empty<string>();
        return _vpkReader.ListFiles()
            .Where(file => extensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));
    }

    protected async Task ExtractFiles(IEnumerable<string> files, string outputPath, CancellationToken cancellationToken, string fileTypeDescription)
    {
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        var fileList = files.ToList();
        int total = fileList.Count;
        int processed = 0;

        foreach (var file in fileList)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                var data = _vpkReader.ReadFile(file);
                var outPath = Path.Combine(outputPath, file);
                var dir = Path.GetDirectoryName(outPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                await File.WriteAllBytesAsync(outPath, data, cancellationToken);
                processed++;

                int percent = (int)((double)processed / total * 100);
                _progressReporter.Report(percent, $"Extracting {fileTypeDescription}: {file}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to extract {file}: {ex.Message}");
            }
        }

        _logger.LogInfo($"Extraction complete. {processed} {fileTypeDescription} files extracted to {outputPath}");
    }
}