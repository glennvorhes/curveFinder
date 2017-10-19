using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Guid g = Guid.NewGuid();
            Console.WriteLine(g.ToString());
            Console.ReadLine();

        }
    }
}
