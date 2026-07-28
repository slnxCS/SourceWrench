using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;

namespace SourceWrench.Commands;

public interface ICommand
{
    string Name { get; }
    string Description { get; }
    Command GetCommand();
    Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = default);
}