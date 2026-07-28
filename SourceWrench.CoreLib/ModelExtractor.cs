using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SourceWrench.Core.Services;

namespace SourceWrench.Core;

public class ModelExtractor : BaseExtractor
{
    public ModelExtractor(IVpkReader vpkReader, ILogger logger, IProgressReporter progressReporter)
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

        var modelExtensions = new[] { ".mdl", ".vvd", ".vvdx", ".phy", ".vtx", ".vpd", ".ani", ".dx90.vtx", ".dx80.vtx", ".sw.vtx" };
        var modelFiles = GetFilesByExtension(modelExtensions);
        // Group by model name (without extensions) to process each model once
        var modelNames = modelFiles
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        int total = modelNames.Count;
        int processed = 0;

        foreach (var modelName in modelNames)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                // Find the main MDL file for this model
                string mdlPath = modelFiles.FirstOrDefault(f =>
                    f.StartsWith(modelName + ".mdl", StringComparison.OrdinalIgnoreCase));
                if (mdlPath == null)
                    continue;

                var mdlData = _vpkReader.ReadFile(mdlPath);
                var outFbxPath = Path.Combine(outputPath, Path.ChangeExtension(mdlPath, ".fbx"));
                var dir = Path.GetDirectoryName(outFbxPath)!;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // Placeholder: copy MDL data to .fbx file (real conversion would use a library)
                await File.WriteAllBytesAsync(outFbxPath, mdlData, cancellationToken);

                processed++;
                int percent = (int)((double)processed / total * 100);
                _progressReporter.Report(percent, $"Processing model: {modelName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process model {modelName}: {ex.Message}");
            }
        }

        _logger.LogInfo($"Model processing complete. {processed} models processed to {outputPath}");
        return 0;
    }
}