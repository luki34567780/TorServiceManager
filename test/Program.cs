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
            while (true)
            {
                if (manager.TorProcess.StandardError.Peek() > 0)
                    Console.Write(manager.TorProcess.StandardError.ReadToEnd());

                if (manager.TorProcess.StandardOutput.Peek() > 0)
                    Console.Write(manager.TorProcess.StandardOutput.ReadToEnd());
            }
        }
    }
}