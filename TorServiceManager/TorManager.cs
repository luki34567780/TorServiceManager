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

            Torrc = torrc ?? Constants.DefaultTorrc;
            Torrc = Torrc.Replace(Constants.HiddenServiceDirReplaceConstant, TorFolderPath);

            TorExecutablePath = $"{TorFolderPath}/tor.exe";
            TorConfigPath = $"{TorFolderPath}/config.torrc";
            TorProcessArguments = $" -f \"{TorConfigPath}\"";

            File.WriteAllBytes(TorExecutablePath, GetTor());
            File.WriteAllText(TorConfigPath, Torrc);
        }

        public async Task<string> GetUrl()
        {
            while (!File.Exists($"{TorFolderPath}/hostname"))
                await Task.Delay(100);

            return await File.ReadAllTextAsync($"{TorFolderPath}/hostname");
        }

        public static string GetTorrcServiceLine(int port, string ip) => $"HiddenServicePort {port} {ip}:{port}";

        public void Start()
        {
            var info = new ProcessStartInfo(TorExecutablePath, TorProcessArguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = TorFolderPath
            };

            TorProcess = new Process
            {
                StartInfo = info
            };

            TorProcess.ErrorDataReceived += TorProcess_DataReceived;
            TorProcess.OutputDataReceived += TorProcess_DataReceived;
            TorProcess.Start();
            TorProcess.BeginOutputReadLine();
            TorProcess.BeginErrorReadLine();
        }

        private void TorProcess_DataReceived(object sender, DataReceivedEventArgs e)
        {
            TorProcessOutput.Enqueue(e.Data);
        }
        

        public void Stop()
        {
            TorProcess?.Kill();
        }

        private static string GetRandomStringFileSafe(int length)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                sb.Append((char)Random.Shared.Next(Constants.RandomStringFileSafeStart, Constants.RandomStringFileSafeEnd));
            }

            return sb.ToString();
        }

        public virtual void Dispose()
        {
            TorProcess?.Dispose();
        }
    }
}