using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;
using SourceWrench.Commands;
using SourceWrench.Core;
using SourceWrench.Core.Services;

var services = new ServiceCollection();
services.AddTransient<IVpkReader, VpkReader>();
services.AddTransient<ILogger, ConsoleLogger>();
services.AddTransient<IProgressReporter, ConsoleProgressReporter>();

// Register extractors
services.AddTransient<MapExtractor>();
services.AddTransient<ModelExtractor>();
services.AddTransient<TextureExtractor>();
services.AddTransient<AudioExtractor>();

// Register commands
services.AddTransient<ExtractCommand>();
services.AddTransient<ExportMapCommand>();
services.AddTransient<ExportModelCommand>();
services.AddTransient<ExportTexturesCommand>();
services.AddTransient<ExportAudioCommand>();

var provider = services.BuildServiceProvider();

var rootCommand = new RootCommand("Source game content extraction tool")
{
    provider.GetRequiredService<ExtractCommand>().GetCommand(),
    provider.GetRequiredService<ExportMapCommand>().GetCommand(),
    provider.GetRequiredService<ExportModelCommand>().GetCommand(),
    provider.GetRequiredService<ExportTexturesCommand>().GetCommand(),
    provider.GetRequiredService<ExportAudioCommand>().GetCommand(),
};

return rootCommand.Parse(args).Invoke();