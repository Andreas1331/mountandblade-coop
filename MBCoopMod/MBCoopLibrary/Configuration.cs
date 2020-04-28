using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBCoopLibrary
{
    public enum Commands
    {
        SendPosition,
        Message
    }

    public class Configuration
    {
        // Setup singleton
        private static Configuration _instance;
        public static Configuration Instance
        {
            get
            {
                return _instance == null ? (_instance = new Configuration()) : _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        public ushort MAX_BYTE_LENGTH = 2;
    }
}
