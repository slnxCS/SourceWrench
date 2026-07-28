using SteamDatabase.ValvePak;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SourceWrench.Core;

public class VpkReader : IVpkReader
{
    public VpkReader() {}

    private Package? _package;

    public bool Open(string vpkPath)
    {
        try
        {
            _package = new Package();
            _package.Read(vpkPath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public IEnumerable<string> ListFiles() => _package?.Entries.SelectMany(kvp => kvp.Value.Select(entry => entry.GetFullPath())) ?? Enumerable.Empty<string>();

    public bool TryGetEntry(string path, out PackageEntry entry)
    {
        entry = null!;
        if (_package == null) return false;

        entry = _package.FindEntry(path);
        return entry != null;
    }

    public byte[] ReadFile(string path)
    {
        if (_package == null) throw new InvalidOperationException("Package not opened");

        var entry = _package.FindEntry(path);
        if (entry == null)
            throw new FileNotFoundException($"Entry not found: {path}");

        _package.ReadEntry(entry, out byte[] data);
        return data;
    }
}
