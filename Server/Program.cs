using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Servers;
namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServer.Servers.Server server = new GameServer.Servers.Server("127.0.0.1", 6688);
            server.Start();

            Console.ReadKey();
        }
    }
}
