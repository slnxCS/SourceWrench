using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SourceWrench.Core.Services;

namespace SourceWrench.Core;

public class TextureExtractor : BaseExtractor
{
    public TextureExtractor(IVpkReader vpkReader, ILogger logger, IProgressReporter progressReporter)
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

        var textureExtensions = new[] { ".vtf", ".tga", ".png", ".jpg", ".jpeg", ".bmp", ".tif", ".tiff", ".pic" };
        var textureFiles = GetFilesByExtension(textureExtensions);
        var fileList = textureFiles.ToList();
        int total = fileList.Count;
        int processed = 0;

        foreach (var file in fileList)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                var data = _vpkReader.ReadFile(file);
                string outPath;
                if (Path.GetExtension(file).Equals(".vtf", StringComparison.OrdinalIgnoreCase))
                {
                    // Convert VTF to PNG (placeholder: just rename extension)
                    outPath = Path.Combine(outputPath, Path.ChangeExtension(file, ".png"));
                }
                else
                {
                    // Copy other texture formats as-is
                    outPath = Path.Combine(outputPath, file);
                }

                string dir = Path.GetDirectoryName(outPath)!;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                await File.WriteAllBytesAsync(outPath, data, cancellationToken);

                processed++;
                int percent = (int)((double)processed / total * 100);
                _progressReporter.Report(percent, $"Processing texture: {file}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process texture {file}: {ex.Message}");
            }
        }

        _logger.LogInfo($"Texture processing complete. {processed} textures processed to {outputPath}");
        return 0;
    }
}