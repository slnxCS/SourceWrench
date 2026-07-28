using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using SourceWrench.Core.Services;

namespace SourceWrench.Core;

public class AudioExtractor : BaseExtractor
{
    public AudioExtractor(IVpkReader vpkReader, ILogger logger, IProgressReporter progressReporter)
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

        var audioExtensions = new[] { ".wav", ".mp3", ".ogg", ".flac", ".aac", ".m4a", ".wma" };
        var audioFiles = GetFilesByExtension(audioExtensions);
        var fileList = audioFiles.ToList();
        int total = fileList.Count;
        int processed = 0;

        foreach (var file in fileList)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                var data = _vpkReader.ReadFile(file);
                string outPath = Path.Combine(outputPath, Path.ChangeExtension(file, ".wav")); // Convert to wav
                string dir = Path.GetDirectoryName(outPath)!;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // Convert audio to WAV using NAudio (fallback to copy if unsupported)
                await ConvertAudioToWavAsync(data, outPath, cancellationToken);

                processed++;
                int percent = (int)((double)processed / total * 100);
                _progressReporter.Report(percent, $"Converting audio: {file}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process audio {file}: {ex.Message}");
            }
        }

        _logger.LogInfo($"Audio conversion complete. {processed} audio files processed to {outputPath}");
        return 0;
    }

    private async Task ConvertAudioToWavAsync(byte[] audioData, string outputPath, CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(tempFile, audioData);
                using var reader = new AudioFileReader(tempFile);
                using var writer = new WaveFileWriter(outputPath, reader.WaveFormat);
                reader.CopyTo(writer);
            }
            catch
            {
                // If unable to read with AudioFileReader (unsupported format), just copy the raw data as .wav (placeholder)
                File.WriteAllBytes(outputPath, audioData);
            }
            finally
            {
                try { File.Delete(tempFile); } catch { }
            }
        }, cancellationToken);
    }
}