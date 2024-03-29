﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ICN_G10_GameServer
{
    class ServerSend
    {
        #region TCP Send Method
        /// <summary>Sending a packet to certain Client using TCP protocol.</summary>
        /// <param name="_toWhichClient">The ID of the client that we are sending to.</param>
        /// <param name="_packet">The packet we want to send.</param>
        private static void SendTCPData(int _toWhichClient,Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toWhichClient].tcp.SendData(_packet);
        }
        /// <summary>Sending a packet to all Clients using TCP protocol.</summary>
        /// <param name="_packet">The packet we want to send.</param>
        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
        /// <summary>Sending a packet to all Clients except certain Client using TCP protocol.</summary>
        /// <param name="_exceptClient">The ID of the exception Client.</param>
        /// <param name="_packet">The packet we want to send.</param>
        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
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
        #endregion

        #region UDP Send Method
        /// <summary>Sending a packet to certain Client using UDP protocol.</summary>
        /// <param name="_toWhichClient">The ID of the client that we are sending to.</param>
        /// <param name="_packet">The packet we want to send.</param>
        private static void SendUDPData(int _toWhichClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toWhichClient].udp.SendData(_packet);
        }
        /// <summary>Sending a packet to all Clients using UDP protocol.</summary>
        /// <param name="_packet">The packet we want to send.</param>
        private static void SendUDPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
        /// <summary>Sending a packet to all Clients except certain Client using UDP protocol.</summary>
        /// <param name="_exceptClient">The ID of the exception Client.</param>
        /// <param name="_packet">The packet we want to send.</param>
        private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].udp.SendData(_packet);
                }
            }
        }
        #endregion

        #region HandShake Method for TCP/UDP
        /// <summary>Sending a welcom message to certain Client using TCP protocol.</summary>
        /// <param name="_toWhichClient">The ID of the Client.</param>
        /// <param name="_msg">The welcome message to send.</param>
        public static void Welcome(int _toWhichClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toWhichClient);

                SendTCPData(_toWhichClient,_packet);
            }
        }
        /// <summary>Sending a welcom message to certain Client using UDP protocol.</summary>
        /// <param name="_toWhichClient">The ID of the Client.</param>
        /// <param name="_msg">The welcome message to send.</param>
        public static void UDPTest(int _toWhichClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.udpTest))
            {
                _packet.Write(_msg);

                SendUDPData(_toWhichClient, _packet);
            }
        }
        #endregion

        #region Game Informations
        /// <summary>Sending player information for certain Client to creat the player object when new client connected via TCP.</summary>
        /// <param name="_toWhichClient">The ID of the Client.</param>
        /// <param name="_player">The player information.</param>
        public static void SpawnPlayer(int _toWhichClient, Player _player)
        {
            using(Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.username);
                _packet.Write(_player.position);
                _packet.Write(_player.rotation);

                Console.WriteLine($"Sending Spawn Data to {_toWhichClient}");
                SendTCPData(_toWhichClient, _packet);
            }
        }
        /// <summary>Sending player position information for All Client via TCP.</summary>
        public static void PlayerPosition(Player _player)
        {
            using(Packet _packet = new Packet((int)ServerPackets.playerPosition))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.position);

                SendUDPDataToAll(_packet);
            }
        }
        /// <summary>Sending player Rotation information for All Client except the original one via TCP.</summary>
        public static void PlayerRotation(Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.rotation);

                SendUDPDataToAll(_player.id,_packet);
            }
        }
        /// <summary>Sending player disconnection information for All Client to delete the object via TCP.</summary>
        public static void PlayerDisconnected(int _playerID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
            {
                _packet.Write(_playerID);

                SendTCPDataToAll(_packet);
            }
        }
        #endregion
    }
}
