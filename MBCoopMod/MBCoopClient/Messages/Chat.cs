using MBCoopClient.Network;
using MBCoopLibrary;
using MBCoopLibrary.NetworkData;
using System;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;

namespace MBCoopClient.Messages
{
    public class Chat
    {
        private bool isChatBoxOpen = false;
        private readonly Client client;

        public Chat(Client client)
        {
            this.client = client;
        }

        public void ListenForInputThisTick(float dt)
        {
            if (!Input.IsKeyPressed(InputKey.F10) || isChatBoxOpen)
                return;

            isChatBoxOpen = true;
            //Task.Run((Action)(() =>
            //{
            //    bool flag = true;
            //    while (flag)
            //    {
            //        if (Input.IsKeyPressed(InputKey.Enter) && Main._chatInquiry.Text.Length > 0)
            //        {
            //            flag = false;
            //            Client.SendData(Main._chatInquiry.Text);
            //            InformationManager.HideInquiry();
            //        }
            //    }
            //}));
            TextInquiryData inqData = new TextInquiryData("Type your message", "(No empty messages allowed)", true, true, "Send", "Cancel", OnConfirmMessage, OnCancelMessage, false, IsMessageValid);
            InformationManager.ShowTextInquiry(inqData, false);
        }

        private void OnConfirmMessage(string msg)
        {
            // Send the message
            MessageHandler.SendMessage($"You: {msg}");
            Packet packet = new Packet(Commands.SendMessage, Encoding.UTF8.GetBytes($"From {client.Username}: {msg}"));
            client.SendPacket(packet);
            isChatBoxOpen = false;
        }

        private void OnCancelMessage()
        {
            isChatBoxOpen = false;
        }

        private bool IsMessageValid(string msg)
        {
            return !String.IsNullOrEmpty(msg);
        }
    }
}
