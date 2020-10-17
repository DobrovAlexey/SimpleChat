using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        TcpListener listener;

        CancellationTokenSource cancellation = new CancellationTokenSource();

        Dictionary<Socket, string> usersList = new Dictionary<Socket, string>();

        Action<string> logger;

        public Server(string ipAdress, int port, Action<string> logger)
        {
            listener = new TcpListener(IPAddress.Parse(ipAdress), port);
            this.logger = logger;
        }

        public void Start()
        {
            listener.Start();
            logger("Запуск сервера");
            Task.Run(AcceptSockets, cancellation.Token);
        }

        public void Stop()
        {
            cancellation.Cancel();
            logger("Остановка сервера");
        }

        public string[] GetList()
        {
            return usersList.Values.ToArray();
        }

        private void AcceptSockets()
        {
            while(true)
            {
                Socket temp = listener.AcceptSocket();
                Task.Run(() => SocketHandle(temp), cancellation.Token);
            }
        }

        private void SocketHandle(Socket s)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int count = s.Receive(buffer);
                string username = Encoding.UTF8.GetString(buffer, 1, count - 1);
                if (usersList.ContainsValue(username) && !ConstrainOnly(username))
                {
                    s.Disconnect(false);
                    return;
                }
                s.Send(new byte[] { 255 });

                usersList.Add(s, username);
                logger("Подключён " + username);

                while (true)
                {
                    count = s.Receive(buffer);
                    if (buffer[0] != 0)
                    {
                        s.Send(buffer);
                        Thread.Sleep(100);
                        continue;
                    }

                    Task.Run(() =>
                    {
                        string mes = '\u00FF' + username + Encoding.UTF8.GetString(buffer, 1, count - 1);
                        logger("Сообщение от " + mes.Substring(1));

                        foreach (Socket socket in usersList.Keys)
                            socket.Send(Encoding.UTF8.GetBytes(mes));
                    });
                }
            }
            catch (SocketException)
            {
                logger($"{usersList[s]} отключился");
                usersList.Remove(s);
            }
        }

        private bool ConstrainOnly(string str)
        {
            int i = 0;
            for (; i < str.Length; i++)
                if (char.IsLetterOrDigit(str[i]))
                    break;
            return !(i < str.Length);
        }
    }
}
