using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace MBCoopClient.Messages
{
    public static class MessageHandler 
    {
        //public MessageHandler Instance { get
        //    {
        //        return Instance == null ? (Instance = new MessageHandler()) : Instance;
        //    } 
        //    private set
        //    {
        //        Instance = value;
        //    }
        //}

        public static void SendMessage(string msg)
        {
            if (msg == null)
                return;

            InformationManager.DisplayMessage(new InformationMessage(msg));
        }
    }
}
