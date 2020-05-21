using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ICN_G10_GameServer
{
    class Client
    {
        public static int dataBufferSize = 4096;
        public int id;
        public TCP tcp;

        // Constructor for client to init
        public Client(int _client_id)
        {
            id = _client_id;
            tcp = new TCP(id);
        }

        public class TCP
        {
            public TcpClient socket;
            private readonly int id;
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
                receiveBuffer = new byte[dataBufferSize];
                // Start read from the stream
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(id, "Welcome to the Server for ICN2020 Group 10");
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
                        // TODO disconnect
                        return;
                    }
                    // Using another array to store data
                    byte[] _data = new byte[_recvLength];
                    Array.Copy(receiveBuffer, _data, _recvLength);

                    // TODO handle data

                    // Continue to read new data
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error encounter when receive the data {_ex}");

                    // TODO disconnect
                }
            }
        }
    }
}
