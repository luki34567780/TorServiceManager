using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TorServiceManager
{
    public class TorManager : IDisposable
    {
        private static byte[] GetTor() => Decryptor.DecryptData((byte[])TorResources.Resource1.ResourceManager.GetObject("tor"));
        public string Torrc { get; set; }
        public string TorFolderPath { get; }
        public string TorExecutablePath { get; }
        public string TorConfigPath { get; }
        public string TorProcessArguments { get; set; }
        public List<string> HiddenServiceNames { get; } = new();
        public Process? TorProcess { get; private set; }
        public ConcurrentQueue<string> TorProcessOutput { get; private set; } = new();

        public TorManager(string? torrc = null, string? path = null)
        {
            TorFolderPath = path ?? $"{Path.GetTempPath()}{GetRandomStringFileSafe(25)}";
            
            Directory.CreateDirectory(TorFolderPath);

            Torrc = torrc ?? Constants.DefaultTorrc.Replace(Constants.HiddenServiceDirReplaceConstant, TorFolderPath);
            TorExecutablePath = $"{TorFolderPath}/tor.exe";
            TorConfigPath = $"{TorFolderPath}/config.torrc";
            TorProcessArguments = $" -f \"{TorConfigPath}\"";

            File.WriteAllBytes(TorExecutablePath, GetTor());
            File.WriteAllText(TorConfigPath, Torrc);
        }

        public static string GetTorrcServiceLine(int port, string ip) => $"HiddenServicePort {port} {ip}:{port}";

        public void Start()
        {
            var info = new ProcessStartInfo(TorExecutablePath, TorProcessArguments);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.CreateNoWindow = true;
            info.WorkingDirectory = TorFolderPath;

            TorProcess = new Process();
            TorProcess.StartInfo = info;
            TorProcess.ErrorDataReceived += (_, e) => TorProcessOutput.Enqueue(e.Data);
            TorProcess.OutputDataReceived += (_, e) => TorProcessOutput.Enqueue(e.Data);
            TorProcess.Start();
        }

        public void Stop()
        {
            TorProcess.Kill();
        }

        private static string GetRandomStringFileSafe(int length)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append((char)Random.Shared.Next(Constants.RandomStringFileSafeStart, Constants.RandomStringFileSafeEnd));
            }

            return sb.ToString();
        }

        public void Dispose()
        {
            TorProcess?.Dispose();
        }
    }
}