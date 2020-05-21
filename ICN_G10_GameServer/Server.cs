using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ICN_G10_GameServer
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        public delegate void PackerHandler(int _fromWhichClient, Packet _packet);
        public static Dictionary<int, PackerHandler> packetHandlers;

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        // Server Main Start
        public static void Start(int __maxPlayers, int _port)
        {
            // Server Basic Setup
            MaxPlayers = __maxPlayers;
            Port = _port;

            Console.WriteLine("Starting Server...");

            // Init Data
            ServerDataInit();

            // Tcp Server Create
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            // Begin Listening Incoming message
            // When it accept the request, call TCPConnectCallback function to handle the rest work.
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            // Udp Server Create
            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"Server starts on {Port}.");
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            // Using the output of IAsyncResult and EndAcceptTcpClient function
            // to create Client object to handle other request
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            // Then continue listening other request
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Connection from {_client.Client.RemoteEndPoint}...");

            // Find the empty slot and plug the new client into it
            for (int i = 1; i <= MaxPlayers; i++)
            {
                if(clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    Console.WriteLine($"Successfully connected and the id is {i}");
                    return;
                }
            }
        }

        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (_data.Length < 4)
                {
                    return;
                }

                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    // In order to make sure the code below wont go wrong
                    if (_clientId == 0)
                    {
                        return;
                    }

                    // If the endPoint is null means that
                    // this is the new connection to establish
                    if (clients[_clientId].udp.endPoint == null)
                    {
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        // Return to avoid attemping to handle data
                        return;
                    }
                    // Check to avoid hacker to send fack packet with unmatched IP and Port
                    if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        clients[_clientId].udp.HandleData(_packet);
                    }
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
            }
        }


        private static void ServerDataInit()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PackerHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceivedTCP},
                { (int)ClientPackets.udpTestReceived, ServerHandle.WelcomeReceivedUDP},
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement},
            };
            Console.WriteLine("Packet Handler Init");
        }
    }
}
