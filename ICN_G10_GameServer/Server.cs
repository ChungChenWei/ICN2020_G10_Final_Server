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
        private static TcpListener tcpListener;

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

        private static void ServerDataInit()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }
        }
    }
}
