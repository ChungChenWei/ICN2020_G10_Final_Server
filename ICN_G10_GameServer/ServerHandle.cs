using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ICN_G10_GameServer
{
    class ServerHandle
    {
        #region Welcome Received using TCP/UDP
        /// <summary>Check is the TCP connection is successfully and the ID is matched.
        /// Then send the player into games.</summary>
        /// <param name="_fromWhichClient">The Client ID.</param>
        /// <param name="_packet">The TCP packet received.</param>
        public static void WelcomeReceivedTCP(int _fromWhichClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _clientName = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_fromWhichClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromWhichClient}.");
            if (_fromWhichClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_clientName}\" (ID:{_fromWhichClient}) has received wrong ID ({_clientIdCheck})!");
            }

            Server.clients[_fromWhichClient].SendIntoGame(_clientName);
        }
        /// <summary>Check is the UDP connection is successfully and output a short message.</summary>
        /// <param name="_fromWhichClient">The Client ID.</param>
        /// <param name="_packet">The UDP packet received.</param>
        public static void WelcomeReceivedUDP(int _fromWhichClient, Packet _packet)
        {
            string _msg = _packet.ReadString();

            Console.WriteLine($"Received packet from ID:{_fromWhichClient} via UDP. Contains message: {_msg}!");
        }
        #endregion

        public static void PlayerMovement(int _fromWhichClient, Packet _packet)
        {
            bool[] _inputs = new bool[_packet.ReadInt()];
            for (int i = 0; i < _inputs.Length; i++)
            {
                _inputs[i] = _packet.ReadBool();
            }
            Quaternion _rotation = _packet.ReadQuaternion();

            Server.clients[_fromWhichClient].player.SetInput(_inputs, _rotation);
        }
    }
}
