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
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                string[] output = Menu.ActiveMenu.ToString().Split(new string[] { "COLOR" }, StringSplitOptions.None);
                for (int i = 0; i < output.Length; i++)
                {
                    string s = output[i];
                    switch(s[0])
                    {
                        case '0': // item not selected color
                            s = s.Remove(0, 1);
                            break;
                        case '1': // selected item color
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                            s = s.Remove(0, 1);
                            break;
                        case '2': // error color
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            Console.ForegroundColor = ConsoleColor.Black;
                            s = s.Remove(0, 1);
                            break;
                    }
                    Console.Write(s);
                    Console.ResetColor();
                }
                

                //Console.WriteLine(Menu.ActiveMenu.ToString());
                Menu.ActiveMenu.input(Console.ReadKey());
            }
        }
    }
}
