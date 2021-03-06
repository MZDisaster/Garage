﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Garage
{
    class GarageCreator
    {
        MenuCreator menuCreator;
        public static int GarageCount = 0;
        public GarageCreator()
        {
            menuCreator = new MenuCreator(this);
            Load();
        }

        public Garage<T> createGarage<T>(string name, int spaces) where T:Vehicle
        {
            GarageCount += 1;
            Garage<T> garage = new Garage<T>();
            garage.Add(name, spaces, menuCreator);
            return garage;
        }

        public void Load()
        {
            var objectsFile = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData), "objects.xml");
            if (File.Exists(objectsFile))
            {
                if (XDocument.Load(objectsFile).Root.Elements().Count() != 0)
                {
                    XDocument Vehiclelist1 = XDocument.Load(objectsFile);
                    foreach (var garage in Vehiclelist1.Descendants("Garage"))
                    {
                        MethodInfo creategarage = typeof(GarageCreator).GetMethod("createGarage");

                        creategarage = creategarage.MakeGenericMethod(new Type[] { Type.GetType((string)garage.Attribute("Type").Value).GetGenericArguments().FirstOrDefault() });

                        object garage1 = creategarage.Invoke(this, new object[] { (string)garage.Attribute("Name").Value, int.Parse(garage.Attribute("Space").Value) });

                        MethodInfo addtogarage = garage1.GetType().GetMethod("addToGarage");


                        Type type = garage1.GetType().GetGenericArguments().FirstOrDefault();
                        addtogarage = addtogarage.MakeGenericMethod(new Type[] { type });

                        foreach (var vehicle in garage.Descendants("Vehicle"))
                        {
                            string color = vehicle.Attribute("Color").Value;
                            int wheels = int.Parse(vehicle.Attribute("Wheels").Value);
                            int regnr = int.Parse(vehicle.Attribute("RegNr").Value);

                            addtogarage.Invoke(garage1, new object[] { color, wheels, regnr });
                        }
                    }
                }
                else
                    Menu.ActiveMenu = menuCreator.GarageCreateMenu;
            }
            else
                Menu.ActiveMenu = menuCreator.GarageCreateMenu;

            Menu.ActiveMenu.ErrorMessage = "";
        }
    }
}
