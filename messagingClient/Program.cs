using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections;
using System.Net.Sockets;

namespace messagingClient
{
    class Program
    {
        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";
        static void Main(string[] args)
        {
          
            //---create a TCPClient object at the IP and port no.---
            TcpClient client = new TcpClient(SERVER_IP, PORT_NO);
            NetworkStream nwStream = client.GetStream();

            //---read back incoming text---
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            Console.WriteLine("Received: " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));

            Thread sendThread = new Thread(() => sendMsg(client, nwStream));
            Thread receiveThread = new Thread(() => receiveMsg(client, nwStream));
            sendThread.Start();
            receiveThread.Start();
           
        }
        static void sendMsg(TcpClient client, NetworkStream nwStream)
        {
            int x = 0;
            byte[] bytesToRead;
            int bytesRead;
            while (client.Connected)
            {
                
                Thread.Sleep(100);
                //---one time username acquisition---
                if (x == 0)
                {
                    Console.Write("Please send your desired username: ");
                    string sendMessage = Convert.ToString("12czx5" + Console.ReadLine());

                    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(sendMessage);
                    //send message over networkstream
                    nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                    //---read back the text---
                    bytesToRead = new byte[client.ReceiveBufferSize];
                    bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                    Console.WriteLine("Received: " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
                    x = 1;
                }
                else
                {
                    //---message to be sent---
                    Console.Write("You: ");
                    string sendMessage = Convert.ToString(Console.ReadLine());

                    if (sendMessage == "disconnect")
                    {
                        client.Close();
                    }

                    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(sendMessage);
                    //---send message over networkstream---
                    nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                }
            }

        }
        static void receiveMsg(TcpClient client, NetworkStream nwStream)
        {
            byte[] bytesToRead;
            int bytesRead;
            while (client.Connected)
            {
                Thread.Sleep(100);
                if (nwStream.DataAvailable)
                {
                    int consoleRow = Console.CursorTop;
                    int consoleCursorPos = Console.CursorLeft;
                    //---read back the text-- -
                    bytesToRead = new byte[client.ReceiveBufferSize];
                    bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                    Console.SetCursorPosition(0, consoleRow);
                    Console.WriteLine(Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
                    Console.Write("You: ");
                }

          
                if (nwStream.DataAvailable)
                {
                    //---read back the text-- -
                    bytesToRead = new byte[client.ReceiveBufferSize];
                    bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                    Console.WriteLine("Received: " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
                }

                
            }

        }
    }
}
