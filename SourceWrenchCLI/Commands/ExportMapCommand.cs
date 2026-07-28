using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SourceWrench.Core;

namespace SourceWrench.Commands;

public class ExportMapCommand : ICommand
{
    private readonly MapExtractor _mapExtractor;

    public ExportMapCommand(MapExtractor mapExtractor)
    {
        _mapExtractor = mapExtractor;
    }

    public string Name => "exportmap";
    public string Description => "Extract map files (.bsp) from a VPK archive";

    public Command GetCommand()
    {
        var sourceOption = new Option<FileInfo>("--source") {Required = true};
        var outputOption = new Option<DirectoryInfo>("--output") {Required = true};

        var command = new Command(Name, Description)
            {sourceOption, outputOption};

        command.SetAction(async (parseRes, ct) =>
        {
            var source = parseRes.GetValue(sourceOption);
            var output = parseRes.GetValue(outputOption);

            if (source != null && output != null) 
                await ExecuteAsync([source.FullName, output.FullName], ct);

            return 0;
        });

        return command;
    }

    public async Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        if (args.Length < 2)
        {
            return 1;
        }

        var sourcePath = args[0];
        var outputPath = args[1];

        return await _mapExtractor.ExtractAsync(sourcePath, outputPath, cancellationToken);
    }
}