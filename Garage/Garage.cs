using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;

namespace Garage
{
    class Garage <T> where T: Vehicle,
        IEnumerable
    {
        public List<T> VehicleList { get; set; }
        public int spaces { get; set; }
        public string Name;
        public static List<Garage<T>> GaragesList = new List<Garage<T>>();
        Menu mainmenu;
        public Garage() { }

        public Garage(string name, int Spaces, MenuCreator menuCreator)
        {
            VehicleList = new List<T>();
            Name = name;
            spaces = Spaces;
            GaragesList.Add(this);
            mainmenu = menuCreator.CreateMenu(this);
        }

        public void addToGarage<S>(string color, int wheels, int regnr) where S:Vehicle
        {
            if (VehicleList.Count() < spaces)
            {
                VehicleList.Add((T)Activator.CreateInstance(typeof(S), color, wheels, regnr));
                mainmenu.setHeader(string.Format("Welcome to the {0} admin panel!\nGarage Type: {1} - size:{2} - Used:{3} - Available:{4}", this.Name, typeof(T).Name.ToString(), this.spaces, this.VehicleList.Count(), (this.spaces - this.VehicleList.Count())));
                Menu.ActiveMenu.ErrorMessage = "Success!";
            }
            else
                Menu.ActiveMenu.ErrorMessage = "Garage is full!";
        }

        public List<T> getList()
        {
            return VehicleList;
        }

        public void removeFromGarage<S>(T v) where S : Vehicle
        {
            if (VehicleList.Contains(v))
                VehicleList.Remove(v);
        }

        public List<T> searchVehicle<S>(int number) where S : Vehicle
        {
            if (number > 100) // wheels can't be more than 100, assume number is a regnr
            {
                IEnumerable<T> Vehicles = VehicleList.Where(V => V.REGNR.ToString().Contains(number.ToString()));
                if (Vehicles.Count() > 0)
                {
                    return Vehicles.ToList();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                IEnumerable<T> Vehicles = VehicleList.Where(V => V.Wheels == number);
                if (Vehicles.Count() > 0)
                {
                    return Vehicles.ToList();
                }
                else
                {
                    Menu.ActiveMenu.ErrorMessage = "Nothing found!";
                    return null;
                }
            }
        }

        public List<T> searchVehicle<S>(string input) where S : Vehicle
        {
            IEnumerable<T> Vehicles = VehicleList.Where(V => V.Color.Contains(input));
            
            if (Vehicles.Count() > 0)
            {
                return Vehicles.ToList();
            }
            else
            {
                Menu.ActiveMenu.ErrorMessage = "Nothing found!";
                return null;
            }
        }

        public List<T> searchVehicle<S>(int input1, string input2) where S : Vehicle
        {
            input2 = input2.First().ToString().ToUpper() + input2.Substring(1);
            IEnumerable<T> Vehicles = VehicleList.Where(V => V.GetType().ToString().Contains(input2) && V.Wheels == input1);

            if (Vehicles.Count() > 0)
            {
                return Vehicles.ToList();
            }
            else
            {
                input2 = input2.First().ToString().ToLower() + input2.Substring(1);

                Vehicles = VehicleList.Where(V => V.Wheels == input1 && V.Color.Contains(input2));
                if (Vehicles.Count() > 0)
                {
                    return Vehicles.ToList();
                }
                else
                {
                    Menu.ActiveMenu.ErrorMessage = "Nothing found!";
                    return null;
                }
            }
        }

        public List<T> searchVehicle<S>(string input1, string input2) where S : Vehicle
        {
            input2 = input2.First().ToString().ToUpper() + input2.Substring(1);
            IEnumerable<T> Vehicles = VehicleList.Where(V => V.GetType().ToString().Contains(input2) && V.Color.Contains(input1));

            if (Vehicles.Count() > 0)
            {
                input2 = input2.First().ToString().ToUpper() + input2.Substring(1);

                Vehicles = VehicleList.Where(V => V.GetType().ToString().Contains(input2) && V.Color.Contains(input1));
                if (Vehicles.Count() > 0)
                {
                    return Vehicles.ToList();
                }
                else
                {
                    Menu.ActiveMenu.ErrorMessage = "Nothing found!";
                    return null;
                }
            }
            else
            {
                input2 = input2.First().ToString().ToLower() + input2.Substring(1);
                input1 = input1.First().ToString().ToUpper() + input1.Substring(1);

                Vehicles = VehicleList.Where(V => V.GetType().ToString().Contains(input1) && V.Color.Contains(input2));
                if (Vehicles.Count() > 0)
                {
                    return Vehicles.ToList();
                }
                else
                {
                    Menu.ActiveMenu.ErrorMessage = "Nothing found!";
                    return null;
                }
            }
        }
        public IEnumerator<Vehicle> GetEnumerator() 
        {
            foreach (Vehicle V in VehicleList)
            {
                yield return V;
            }
        }
    }
}
