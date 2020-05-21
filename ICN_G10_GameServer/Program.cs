using System;

namespace ICN_G10_GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "ICN_G10 Game Server";

            Server.Start(50, 7777);

            Console.ReadKey();
        }
    }
}
