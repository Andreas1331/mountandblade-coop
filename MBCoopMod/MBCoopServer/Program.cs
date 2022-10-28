using MBCoopServer.ConsoleWindow;
using MBCoopServer.Network;
using System;

namespace MBCoopServer
{
    public class Program
    {
        public static bool hostFound = false;
        private static Server server;
        private static ConsoleListener consoleListener;

        private static void Main(string[] args)
        {
            server = new Server("192.168.0.89", 13000);
            server.StartServer();

            consoleListener = new ConsoleListener(server);
        }
    }
}
