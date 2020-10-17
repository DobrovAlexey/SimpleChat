using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1", 5000, Console.WriteLine);
            server.Start();
            string read;
            bool start = true;
            do
            {
                read = Console.ReadLine();
                switch (read)
                {
                    case "stop":
                    case "exit":
                        server.Stop();
                        start = false;
                        break;
                    case "users":
                    case "list":
                        Console.WriteLine(string.Join(Environment.NewLine, server.GetList()));
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                }
            } while (start);
        }
    }
}
