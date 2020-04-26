using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBCoopClient.ClientData
{
    public class Host : Client
    {
        public Host(string ipAddress, int port, string username) : base(ipAddress, port, username)
        {
        }
    }
}
