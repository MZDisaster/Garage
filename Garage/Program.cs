using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Garage
{
    class Program
    {
        static void Main(string[] args)
        {
            new GarageCreator();
            
            while(true)
            {
                Console.Clear();
                Console.WriteLine(Menu.ActiveMenu.ToString());
                Menu.ActiveMenu.input(Console.ReadLine().Trim());
            }
        }
    }
}
