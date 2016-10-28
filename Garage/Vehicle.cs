using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Garage
{
    public class Vehicle:IEnumerable
    {
        public int REGNR { get; set; }
        public string Color { get; set; }
        public int Wheels { get; set; }
        public List<Vehicle> VehicleList = new List<Vehicle>();
        

        public Vehicle(string color, int wheels, int regnr)
        {
            Color = color;
            Wheels = wheels;
            REGNR = regnr;
        }
        

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Vehicle> GetEnumerator()
        {
            foreach (Vehicle V in VehicleList)
            {
                yield return V;
            }
        }

        public override string ToString()
        {
            string[] type = Regex.Split(this.GetType().ToString(), @"[.]");
            return String.Format("{0, -10}{1, -15}{2, -10}{3}", type[1], REGNR, Color, Wheels);
        }
    }
}
