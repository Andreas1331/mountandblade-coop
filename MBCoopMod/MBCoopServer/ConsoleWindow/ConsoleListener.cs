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
                string input = Console.ReadLine();
                HandleInput(input);
            }
        }

        private void HandleInput(string input)
        {
            // Check if the input contains a valid command
            if (input.StartsWith("chatall"))
            {
                string msg = input.Substring("chatall".Length, input.Length);
                server.BroadcastMessage($"ServerChat: {msg}");
            }
        }
    }
}
