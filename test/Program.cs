using TorServiceManager;

namespace test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var manager = new TorManager();

            manager.Start();
            Console.WriteLine(manager.TorFolderPath);
            var path = manager.TorFolderPath + "\\hostname";
            Thread.Sleep(1000);
            Console.WriteLine(File.ReadAllText(path));

            try
            {
                while (true)
                {
                    if (manager.TorProcessOutput.TryDequeue(out var output))
                    {
                        Console.WriteLine(output);
                    }
                }
            }
            finally
            {
                manager.Stop();
                manager.Dispose();
            }
        }
    }
}