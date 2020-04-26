using System;

namespace MBCoopServer
{
    public class Program
    {
        private static Server _server;

        private static void Main(string[] args)
        {
            _server = new Server("127.0.0.1", 13000);
            _server.StartServer();

            while (true)
            {
                string msg = Console.ReadLine();
                _server.BroadcastMessage(msg);
            }
        }
    }
}
