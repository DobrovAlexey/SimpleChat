﻿using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1", 5000, Console.WriteLine);
            server.Start();
            while (true) ;
        }
    }
}
