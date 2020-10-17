using System;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        static Client client;
        static void Main(string[] args)
        {
            Console.Write("Введите имя пользователя ");
            do
            {
                try
                {
                    string username = Console.ReadLine();
                    if (ConstrainOnly(username))
                        throw new FormatException();
                    client = new Client();
                    client.Start(username);
                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine("Пользователь с таким именем уже существует");
                    Console.Write("Введите другое ");
                }
                catch(FormatException)
                {
                    Console.Clear();
                    Console.WriteLine("Имя пользователя указано неверно");
                    Console.WriteLine("Имя пользователя может содержать только буквы и цифры");
                    Console.Write("Введите другое ");
                }
            } while (client != null && !client.connected);

            Console.Clear();
            Console.WriteLine("Вы подключились к чату");

            string message;
            while(client.connected)
            {
                message = Console.ReadLine();
                if (message.Length > 0 && client.connected)
                    client.SendMessage(message);
            }
        }

        private static bool ConstrainOnly( string str)
        {
            int i = 0;
            for (; i < str.Length; i++)
                if (char.IsLetterOrDigit(str[i]))
                    break;
            return !(i < str.Length);
        }
    }
}
