using SteamDatabase.ValvePak;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SourceWrench.Core;

public interface IVpkReader
{
    public bool Open(string vpkPath);

    public IEnumerable<string> ListFiles();

    public bool TryGetEntry(string path, out PackageEntry entry);

    public byte[] ReadFile(string path);
}
