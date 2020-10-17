using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        Socket socket;

        public string username;

        public bool connected = false;

        public Client()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start(string username)
        {
            this.username = username;
            socket.Connect("127.0.0.1", 5000);
            socket.Send(Encoding.UTF8.GetBytes('\0' + username));
            byte[] buffer = new byte[8];
            do
            {
                socket.Receive(buffer);
            } while (buffer[0] == 0);
            connected = true;
            Task.Run(MessageHandler);
        }

        private void MessageHandler()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int count;
                while (true)
                {
                    count = socket.Receive(buffer);
                    if (buffer[0] == 0)
                    {
                        socket.Send(buffer, count, SocketFlags.None);
                        Thread.Sleep(100);
                        continue;
                    }

                    Console.WriteLine(Encoding.UTF8.GetString(buffer, 1, count - 1));
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Соединение прервано");
                connected = false;
                throw;
            }
        }

        public void SendMessage(string message)
        {
            socket.Send(Encoding.UTF8.GetBytes('\u00FF' + message));
        }
    }
}
