using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SourceWrench.Core;
using SourceWrench.Core.Services;

namespace SourceWrench.Commands;

public class ExtractCommand : ICommand
{
    private readonly IVpkReader _IVpkReader;
    private readonly ILogger _logger;
    private readonly IProgressReporter _progressReporter;

    public ExtractCommand(IVpkReader IVpkReader, ILogger logger, IProgressReporter progressReporter)
    {
        _IVpkReader = IVpkReader;
        _logger = logger;
        _progressReporter = progressReporter;
    }

    public string Name => "extract";
    public string Description => "Extract all files from a VPK archive to an output directory";

    public Command GetCommand()
    {
        var sourceOption = new Option<FileInfo>("--source") { Required = true };
        var outputOption = new Option<DirectoryInfo>("--output") { Required = true };

        var command = new Command(Name, Description) {sourceOption, outputOption};

        command.SetAction(async (parseResult, ct) =>
        {
            FileInfo? source = parseResult.GetValue(sourceOption);
            DirectoryInfo? output = parseResult.GetValue(outputOption);
        
            if (source != null && output != null)
            {
                await ExecuteAsync([source.FullName, output.FullName], ct);
            }
            
            return 0;
        });


        return command;
    }

    public async Task<int> ExecuteAsync(string[] args, CancellationToken ct)
    {
        if (args.Length < 2)
        {
            _logger.LogError("Expected source and output arguments.");
            return 1;
        }

        var sourcePath = args[0];
        var outputPath = args[1];

        return await ExtractAsync(sourcePath, outputPath, ct);
    }

    private async Task<int> ExtractAsync(string sourcePath, string outputPath, CancellationToken ct)
    {
        if (!File.Exists(sourcePath))
        {
            _logger.LogError($"VPK file not found: {sourcePath}");
            return 1;
        }

        if (!_IVpkReader.Open(sourcePath))
        {
            _logger.LogError($"Failed to open VPK file: {sourcePath}");
            return 1;
        }

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        var files = _IVpkReader.ListFiles();
        var fileList = files.ToList();
        int total = fileList.Count;
        int processed = 0;

        foreach (var file in fileList)
        {
            if (ct.IsCancellationRequested)
                break;

            try
            {
                var data = _IVpkReader.ReadFile(file);
                var outPath = Path.Combine(outputPath, file);
                var dir = Path.GetDirectoryName(outPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                await File.WriteAllBytesAsync(outPath, data, ct);
                processed++;

                int percent = (int)((double)processed / total * 100);
                _progressReporter.Report(percent, $"Extracting {file}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to extract {file}: {ex.Message}");
            }
        }

        _logger.LogInfo($"Extraction complete. {processed} files extracted to {outputPath}");
        return 0;
    }
}