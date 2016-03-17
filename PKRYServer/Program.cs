using PKRY.Security;
using PKRY.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKRYServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server(5656);
            //server.AddUser("B.Ostrowski", "261176");
            //server.AddUser("J.Sobczynski", "261208");
            //server.AddUser("R.Kowalski", "123");
            //server.AddUser("B.Banka", "123");
            server.Start();
        }
    }
}
