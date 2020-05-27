using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace ICN_G10_GameServer
{
    class Client
    {
        public static int dataBufferSize = 4096;
        public int id;
        public Player player;
        public TCP tcp;
        public UDP udp;

        // Constructor for client to init
        public Client(int _client_id)
        {
            id = _client_id;
            tcp = new TCP(id);
            udp = new UDP(id);
        }

        public class TCP
        {
            public TcpClient socket;
            private readonly int id;
            private Packet receivedData;
            private NetworkStream stream;
            private byte[] receiveBuffer;

            // Constructor for TCP to asigin the ip;
            public TCP(int _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                // Receive TcpClient opject and init the buffer size
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;
                // Init the stream and reveive buffer
                stream = socket.GetStream();
                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];
                // Start read from the stream
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(id, "Welcome to the Server for ICN2020 Group 10 via TCP!");
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if(socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);

                    }
                }
                catch(Exception _ex)
                {
                    Console.WriteLine($"Error sending data to player {id} via TCP: {_ex} ");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    // Check the input byte length
                    int _recvLength = stream.EndRead(_result);
                    if(_recvLength <= 0)
                    {
                        Server.clients[id].Disconnect();
                        return;
                    }
                    // Using another array to store data
                    byte[] _data = new byte[_recvLength];
                    Array.Copy(receiveBuffer, _data, _recvLength);

                    receivedData.Reset(HandleData(_data));

                    // Continue to read new data
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error encounter when receive the data {_ex}");

                    Server.clients[id].Disconnect();
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;
                receivedData.SetBytes(_data);
                // The begining of a packet is the length(int) thus
                // there will be at least 4 bytes to start
                if (receivedData.UnreadLength() >= 4)
                {
                    // Reads in the packet length
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        // Need to Reset receive packet
                        return true;
                    }
                }

                // if unread length >= packet length means that there will be a complete packet
                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetid = _packet.ReadInt();
                            Server.packetHandlers[_packetid](id, _packet);
                        }
                    });

                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        // Reads in the packet length
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            // Need to Reset receive packet
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;

            }

            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receiveBuffer = null;
                receivedData = null;
                socket = null;
            }
        }

        public class UDP
        {
            public IPEndPoint endPoint;
            private int id;

            public UDP(int _id)
            {
                id = _id;
            }

            public void Connect(IPEndPoint _endPoint)
            {
                endPoint = _endPoint;
                ServerSend.UDPTest(id, "Welcome to the Server for ICN2020 Group 10 vis UDP!");
            }

            public void SendData(Packet _packet)
            {
                Server.SendUDPData(endPoint, _packet);
            }

            public void HandleData(Packet _packetData)
            {
                int _packetLength = _packetData.ReadInt();
                byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet);
                    }
                });
            }

            public void Disconnect()
            {
                endPoint = null;
            }
        }


        public void SendIntoGame(string _playername)
        {
            player = new Player(id, _playername, new Vector3(0, 0, 0));

            // Send all the other players information to the new player
            foreach (Client _client in Server.clients.Values)
            {
                if(_client.player != null)
                {
                    if(_client.id != id)
                    {
                        Console.WriteLine($"Sending {_client.player.id} information to player {id} via TCP");
                        ServerSend.SpawnPlayer(id, _client.player);
                    }
                }
            }
            // Send the new player's information to everyone
            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    ServerSend.SpawnPlayer(_client.id, player);
                }
            }
        }

        private void Disconnect()
        {
            Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} (Player {id}) has disconnected.");

            player = null;
            tcp.Disconnect();
            udp.Disconnect();

            ServerSend.PlayerDisconnected(id);
        }
    }
}
