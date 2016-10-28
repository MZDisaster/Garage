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
        public List<T> VehicleList = new List<T>();
        public int spaces { get; set; }
        public string Name;
        public static List<Garage<T>> GaragesList = new List<Garage<T>>();
        public List<T> SearchResult = new List<T>();

        Menu mainmenu;

        public void Add(string s, int i, MenuCreator mc)
        {
            Name = s;
            spaces = i;
            GaragesList.Add(this);
            mainmenu = mc.CreateMenu(this);
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
                    if (SearchResult != null)
                        SearchResult.Clear();
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
                if (SearchResult != null)
                    SearchResult.Clear();
                return null;
            }
        }

        public List<T> searchVehicle<S>(int input1, int input2) where S : Vehicle
        {
            IEnumerable<T> Vehicles = VehicleList.Where(V => V.Wheels == input1 && V.REGNR.ToString().Contains(input2.ToString()));

            if (Vehicles.Count() > 0)
            {
                return Vehicles.ToList();
            }
            else
            {
                Vehicles = VehicleList.Where(V => V.REGNR.ToString().Contains(input1.ToString()) && V.Wheels == input2);
                if (Vehicles.Count() > 0)
                {
                    return Vehicles.ToList();
                }
                else
                {
                    Menu.ActiveMenu.ErrorMessage = "Nothing found!";
                    if (SearchResult != null)
                        SearchResult.Clear();
                    return null;
                }
            }
        }

        public List<T> searchVehicle<S>(int input1, string input2) where S : Vehicle
        {
            input2 = input2.ToLower();
            IEnumerable<T> Vehicles = VehicleList.Where(V => V.Wheels == input1 && V.Color.ToLower().Contains(input2));

            if (Vehicles.Count() > 0)
            {
                return Vehicles.ToList();
            }
            else
            {
                Vehicles = VehicleList.Where(V => V.REGNR.ToString().Contains(input1.ToString()) && V.Color.ToLower().Contains(input2));
                if (Vehicles.Count() > 0)
                {
                    return Vehicles.ToList();
                }
                else
                {
                    Menu.ActiveMenu.ErrorMessage = "Nothing found!";
                    if (SearchResult != null)
                        SearchResult.Clear();
                    return null;
                }
            }
        }

        public List<T> searchVehicle<S>(string input1, int input2, int input3) where S : Vehicle
        {
            input1 = input1.ToLower();
            IEnumerable<T> Vehicles = VehicleList.Where(V => V.Wheels == input2 && V.Color.ToLower().Contains(input1) && V.REGNR.ToString().Contains(input3.ToString()));

            if (Vehicles.Count() > 0)
            {
                return Vehicles.ToList();
            }
            else
            {
                Vehicles = VehicleList.Where(V => V.Wheels == input3 && V.Color.ToLower().Contains(input1) && V.REGNR.ToString().Contains(input2.ToString()));
                if (Vehicles.Count() > 0)
                {
                    return Vehicles.ToList();
                }
                else
                {
                    Menu.ActiveMenu.ErrorMessage = "Nothing found!";
                    if (SearchResult != null)
                        SearchResult.Clear();
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
