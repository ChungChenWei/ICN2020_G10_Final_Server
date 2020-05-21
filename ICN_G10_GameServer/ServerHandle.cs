using System;
using System.Collections.Generic;
using System.Text;

namespace ICN_G10_GameServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromWhichClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _clientName = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_fromWhichClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromWhichClient}.");
            if( _fromWhichClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_clientName}\" (ID:{_fromWhichClient}) has received wrong ID ({_clientIdCheck})!");
            }

            Server.clients[_fromWhichClient].SendIntoGame(_clientName);
        }
    }
}
