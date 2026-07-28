using System.Threading;
using System.Threading.Tasks;

namespace SourceWrench.Core;

public interface IExtractor
{
    Task<int> ExtractAsync(string vpkPath, string outputPath, CancellationToken cancellationToken = default);
}