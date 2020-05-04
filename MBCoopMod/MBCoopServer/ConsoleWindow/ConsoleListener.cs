using MBCoopServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBCoopServer.ConsoleWindow
{
    public class ConsoleListener
    {
        private readonly Server server;

        public ConsoleListener(Server server)
        {
            this.server = server;

            StartListeningForInput();
        }

        private void StartListeningForInput()
        {
            while (true)
            {
                string msg = Console.ReadLine();
                server.BroadcastMessage(msg);
            }
        }
    }
}
