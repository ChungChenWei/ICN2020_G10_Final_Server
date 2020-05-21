using System;
using System.Collections.Generic;
using System.Text;

namespace ICN_G10_GameServer
{
    class ServerSend
    {
        private static void SendTCPData(int _toWhichClient,Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toWhichClient].tcp.SendData(_packet);
        }
        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
        private static void SendTCPDataToA(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }

        // to which client and the message
        public static void Welcome(int _toWhichClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toWhichClient);

                SendTCPData(_toWhichClient,_packet);
            }
        }
    }
}
