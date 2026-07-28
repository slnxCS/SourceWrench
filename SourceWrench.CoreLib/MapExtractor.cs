using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SourceWrench.Core.Services;

namespace SourceWrench.Core;

public class MapExtractor : BaseExtractor
{
    public MapExtractor(IVpkReader vpkReader, ILogger logger, IProgressReporter progressReporter)
        : base(vpkReader, logger, progressReporter) { }

    public override async Task<int> ExtractAsync(string vpkPath, string outputPath, CancellationToken cancellationToken = default)
    {
        if (!_vpkReader.Open(vpkPath))
        {
            _logger.LogError($"Failed to open VPK file: {vpkPath}");
            return 1;
        }

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        var mapFiles = GetFilesByExtension(new[] { ".bsp" });
        var fileList = mapFiles.ToList();
        int total = fileList.Count;
        int processed = 0;

        foreach (var file in fileList)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                var bspData = _vpkReader.ReadFile(file);
                var outFbxPath = Path.Combine(outputPath, Path.ChangeExtension(file, ".fbx"));
                var dir = Path.GetDirectoryName(outFbxPath)!;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // Placeholder: copy BSP data to .fbx file (real conversion would use a library)
                await File.WriteAllBytesAsync(outFbxPath, bspData, cancellationToken);

                processed++;
                int percent = (int)((double)processed / total * 100);
                _progressReporter.Report(percent, $"Processing map: {file}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process map {file}: {ex.Message}");
            }
        }

        _logger.LogInfo($"Map processing complete. {processed} maps processed to {outputPath}");
        return 0;
    }
}