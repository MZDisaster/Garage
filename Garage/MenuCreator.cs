using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Garage
{
    class MenuCreator
    {
        public Menu BaseMenu = new Menu();
        public Menu GarageCreateMenu = new Menu();

        List<List<Menu>> MenuList = new List<List<Menu>>();
                
        public MenuCreator(GarageCreator garageCreator)
        {
            BaseMenu.setHeader("Garage List:\n type \"new\" to create a new garage!");
            BaseMenu.setFooter("8. Previews Page.\t\t9. Next Page.\n0. Exit");
            BaseMenu.addMethod("new", () => Menu.ActiveMenu = GarageCreateMenu);
            BaseMenu.addMethod("8", BaseMenu.PreviewsPage);
            BaseMenu.addMethod("9", BaseMenu.NextPage);
            BaseMenu.addMethod("0", ExitApp);
            BaseMenu.setdefaultMethod(gotoMenuForGarage);

            GarageCreateMenu.setHeader("Welcome to Garage!\nCreate your Garage by selecting a type\nand entering how many spaces are available and name it:\nExample: \"4 60 MyCarsGarage\"");
            GarageCreateMenu.addItem("1. AirPlanes");
            GarageCreateMenu.addItem("2. Boats");
            GarageCreateMenu.addItem("3. Busses");
            GarageCreateMenu.addItem("4. Cars");
            GarageCreateMenu.addItem("5. Motorcycles");
            GarageCreateMenu.setFooter("0. Back");

            GarageCreateMenu.addMethod("0", () => Menu.ActiveMenu = BaseMenu);
            GarageCreateMenu.setdefaultMethod(() => creategarage(garageCreator));
        }

        private void creategarage(GarageCreator garageCreator)
        {
            int number = 0;
            int number2 = 0;
            if (GarageCreateMenu.userinput.Count() < 3)
                GarageCreateMenu.ErrorMessage = "Please chose the type, spaces available and name it!";
            else if (!int.TryParse(GarageCreateMenu.userinput[0], out number) || !int.TryParse(GarageCreateMenu.userinput[1], out number2))
                GarageCreateMenu.ErrorMessage = "Numbers only please!";
            else if (number == 0 || number2 == 0)
                GarageCreateMenu.ErrorMessage = "Really 0?";
            else
            {
                switch (number)
                {
                    case 1:
                        garageCreator.createGarage<Airplane>(GarageCreateMenu.userinput[2], number2);
                        GarageCreateMenu.ErrorMessage =  "success";
                        break;
                    case 2:
                        garageCreator.createGarage<Boat>(GarageCreateMenu.userinput[2], number2);
                        GarageCreateMenu.ErrorMessage = "success";
                        break;
                    case 3:
                        garageCreator.createGarage<Buss>(GarageCreateMenu.userinput[2], number2);
                        GarageCreateMenu.ErrorMessage = "success";
                        break;
                    case 4:
                        garageCreator.createGarage<Car>(GarageCreateMenu.userinput[2], number2);
                        GarageCreateMenu.ErrorMessage = "success";
                        break;
                    case 5:
                        garageCreator.createGarage<Motorcycle>(GarageCreateMenu.userinput[2], number2);
                        GarageCreateMenu.ErrorMessage = "success";
                        break;
                    default:
                        GarageCreateMenu.ErrorMessage = "Numbers from the list please!";
                        break;
                }
            }
        }

        public void gotoMenuForGarage() // garage specific menus created when the garage was created
        {
            int index = 0;
            int.TryParse(BaseMenu.userinput[0], out index);
            if(MenuList.Count >= (index + BaseMenu.firstitemindisplay))
            {
                Menu.ActiveMenu = MenuList[(index + BaseMenu.firstitemindisplay) - 1][0];

            }

        }

        public Menu CreateMenu<T>(Garage<T> MainGarage) where T : Vehicle
        {
            Menu MainMenu = new Menu() { NAME = MainGarage.Name };
            Menu VehicleListMenu = new Menu();
            Menu searchMenu = new Menu();
            Menu AddVehicleMenu = new Menu();
            Menu GarageSizeMenu = new Menu();

            List<Menu> menulistofgarage = new List<Menu> { MainMenu, VehicleListMenu, searchMenu, AddVehicleMenu, GarageSizeMenu };
            MenuList.Add(menulistofgarage);

            BaseMenu.addItem(MainGarage.Name);

            MainMenu.setHeader(string.Format("Welcome to the {0} admin panel!\nGarage Type: {1} - size:{2} - Used:{3} - Available:{4}", MainGarage.Name, typeof(T).Name.ToString(), MainGarage.spaces, getVehicleList(MainGarage).Count(), (MainGarage.spaces - getVehicleList(MainGarage).Count())));
            MainMenu.addItem("1. Add a Vehicle.");
            MainMenu.addItem("2. View vehicles and manage them.");
            MainMenu.addItem("3. Search for Vehicle.");
            MainMenu.addItem("4. Change Garage size.");
            MainMenu.addItem("0. Back.");

            MainMenu.addMethod("1", () => Menu.ActiveMenu = AddVehicleMenu);
            MainMenu.addMethod("2", () => ViewVehicleList(MainGarage, VehicleListMenu));
            MainMenu.addMethod("3", () =>
            {
                Menu.ActiveMenu = searchMenu;
                searchMenu.clearList();
            });//SearchForVehicle);
            MainMenu.addMethod("4", () => Menu.ActiveMenu = GarageSizeMenu);
            MainMenu.addMethod("0", () => Menu.ActiveMenu = BaseMenu);

            GarageSizeMenu.setHeader("Type a number to change the garage size:");
            GarageSizeMenu.setFooter("0. Back.");
            GarageSizeMenu.addMethod("0", () => gotoMainMenu(MainGarage, MainMenu));
            GarageSizeMenu.setdefaultMethod(() => { 
                    int number = 0;
                    if(!int.TryParse(GarageSizeMenu.userinput[0], out number))
                        GarageSizeMenu.ErrorMessage = "Numbers only please!";
                    else
                    {
                        MainGarage.spaces = number;
                        MainMenu.setHeader(string.Format("Welcome to the {0} admin panel!\nGarage Type: {1} - size:{2} - Used:{3} - Available:{4}", MainGarage.Name, typeof(T).Name.ToString(), MainGarage.spaces, MainGarage.VehicleList.Count(), (MainGarage.spaces - MainGarage.VehicleList.Count())));
                        Menu.ActiveMenu = MainMenu;
                    }
                });

            VehicleListMenu.setFooter("8. Previews Page.\t\t9. Next Page.\n0. Back.");
            VehicleListMenu.addMethod("0", () => gotoMainMenu(MainGarage, MainMenu));
            VehicleListMenu.addMethod("8", () => {
                VehicleListMenu.PreviewsPage();
                ViewVehicleList(MainGarage, VehicleListMenu);
            });
            VehicleListMenu.addMethod("9", () =>
            {
                VehicleListMenu.NextPage();
                ViewVehicleList(MainGarage, VehicleListMenu);
            });
            VehicleListMenu.setdefaultMethod( () => remove(MainGarage, VehicleListMenu));
            
            searchMenu.setHeader(string.Format("Enter search term: (type -1 to remove vehicle number 1 and so on)\nYou can search for types (car/buss/boat/plane/motorcycle)\n\n   {0, -10}{1, -15}{2, -10}{3}", "Type", "RegNr", "Color", "Wheels"));
            searchMenu.setFooter("8. Previews Page.\t\t9. Next Page.\n0. Back.");
            searchMenu.addMethod("8", searchMenu.PreviewsPage);
            searchMenu.addMethod("9", searchMenu.NextPage);

            searchMenu.setdefaultMethod(() => Search(MainGarage, searchMenu));
            searchMenu.addMethod("0", () => gotoMainMenu(MainGarage, MainMenu));

            AddVehicleMenu.setHeader("Enter vehicle info: (Color Wheels Regnr)");
            AddVehicleMenu.setFooter("0. Back.");
            AddVehicleMenu.addMethod("0", () => gotoMainMenu(MainGarage, MainMenu));
            AddVehicleMenu.setdefaultMethod(()=>addvehicle(MainGarage));

            return MainMenu;
        }


        private void addvehicle<T>(Garage<T> garage) where T:Vehicle
        {
            if(Menu.ActiveMenu.userinput.Length > 2)
            {
                int wheels;
                int.TryParse(Menu.ActiveMenu.userinput[1], out wheels);
                int regnr;
                int.TryParse(Menu.ActiveMenu.userinput[2], out regnr);


                if (wheels > 0 && regnr > 0 && wheels < 99)
                    garage.addToGarage<T>(Menu.ActiveMenu.userinput[0], wheels, regnr);
                else
                    Menu.ActiveMenu.ErrorMessage = "Invalid Input!";
            }
            else
            {
                Menu.ActiveMenu.ErrorMessage = "Invalid Input!";
            }
        }

        private void remove<T>(Garage<T> garage, Menu menu) where T:Vehicle
        {
            int number;
            int.TryParse(Menu.ActiveMenu.userinput[0], out number);
            if (number > 0 && number < 7)
            {
                if (garage.VehicleList.Count() > 0 && number <= garage.VehicleList.Count())
                {
                    if (garage.VehicleList.Count() >= (menu.firstitemindisplay + number))
                    {
                        garage.removeFromGarage<T>(garage.VehicleList[(menu.firstitemindisplay + number) - 1]);
                        menu.removeFromList((number) - 1);
                        ViewVehicleList(garage, menu);
                    }
                    else
                        menu.ErrorMessage = "Invalid input!";
                }
                else
                    menu.ErrorMessage = "Invalid input!";
            }
            else
                menu.ErrorMessage = "Invalid input!";
        }

        private void Search<T>(Garage<T> garage, Menu menu) where T:Vehicle
        {
            //List<T> searchResult = garage.SearchResult;
            // User inputs only 1 thing
            if (Menu.ActiveMenu.userinput.Count() == 1)
            {
                int number;
                int.TryParse(Menu.ActiveMenu.userinput[0], out number);

                if (number > 0)
                {
                    garage.SearchResult = garage.searchVehicle<T>(number);
                    //garage.SearchResult
                }
                else if (number == 0)
                {
                    garage.SearchResult = garage.searchVehicle<T>(Menu.ActiveMenu.userinput[0]);
                }
                else
                {
                    int newnumber = number - number - number;
                    if (menu.menuList.Count > 0)
                    {
                        garage.removeFromGarage<T>(garage.SearchResult[(menu.firstitemindisplay + newnumber) - 1]);
                        garage.SearchResult.RemoveAt((menu.firstitemindisplay + newnumber) - 1);
                    }
                }
            }
            else if(Menu.ActiveMenu.userinput.Count() == 2) // user inputs 2 things
            {
                int number = 0;
                List<string> userinput = new List<string>();
                foreach (string input in Menu.ActiveMenu.userinput) // put the input in the respective variables for later usage
                {
                    if (number == 0)
                        if(!int.TryParse(input, out number))
                            userinput.Add(input);
                }

                if (number > 0 && userinput.Count() == 1) // if it's a number and a string
                {
                    if (garage.SearchResult.Count == 0)
                    {
                        garage.SearchResult = garage.searchVehicle<T>(number, userinput[0]);
                    }
                }
                else if (number == 0) // if it's 2 strings
                {
                    garage.SearchResult = garage.searchVehicle<T>(userinput[0], userinput[1]);
                }
                else if(number < 0) // if it's a remove from garage from the search menu
                {
                    int newnumber = number - number - number;
                    if (garage.SearchResult.Count > 0)
                    {
                        garage.removeFromGarage<T>(garage.SearchResult[(menu.firstitemindisplay + newnumber) - 1]);
                        garage.SearchResult.RemoveAt((menu.firstitemindisplay + newnumber) - 1);
                    }
                }
            }

            if (garage.SearchResult != null)
            {
                menu.clearList();
                //menu.setList(searchResult);
                int i = 1;
                foreach (T V in garage.SearchResult)
                {
                    menu.addItem(V.ToString());
                    i += 1;
                }
            }
            else
                menu.clearList();
            //Menu.ActiveMenu = menu;
        }

        private bool CheckNumberInput(string input)
        {
            try
            {
                int.Parse(input);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private void gotoMainMenu<T>(Garage<T> garage, Menu menu) where T:Vehicle
        {
            menu.setHeader(string.Format("Welcome to the {0} admin panel!\nGarage Type: {1} - size:{2} - Used:{3} - Available:{4}", garage.Name, typeof(T).Name.ToString(), garage.spaces, garage.VehicleList.Count(), (garage.spaces - garage.VehicleList.Count())));
            Menu.ActiveMenu = menu;
        }

        private List<string> getVehicleList<T>(Garage<T> garage) where T:Vehicle
        {
            List<string> VList = new List<string>();
            foreach (Vehicle V in garage)
            {
                VList.Add(V.ToString());
            }
            return VList;
        }

        private void ViewVehicleList<T>(Garage<T> garage, Menu menu) where T:Vehicle
        {
            Menu.ActiveMenu = menu;
            menu.setHeader(string.Format("Vehicle List count {0}: (type the number of vehicle to remove)\n   {1, -10}{2, -15}{3, -10}{4}",
                garage.VehicleList.Count(),
                "Type", "RegNr", "Color", "Wheels"));

            List<string> vehicleList = getVehicleList(garage);
            if (vehicleList.Count > 0)
                menu.setList(vehicleList);
            else
            {
                menu.ErrorMessage = "No Vehicles are in the garage!";
            }
        }

        public void ExitApp()
        {
            Save();
            Environment.Exit(0);
        }

        public void Save()
        {
            XDocument objects = new XDocument(
                new XElement("Garages",
                    from G in Garage<Airplane>.GaragesList
                    select new XElement("Garage", new XAttribute("Type", G.GetType()), new XAttribute("Space", G.spaces), new XAttribute("Name", G.Name),
                        from V in G.VehicleList
                        select new XElement("Vehicle",
                            new XAttribute("Type", V.GetType()),
                            new XAttribute("Color", V.Color),
                            new XAttribute("Wheels", V.Wheels),
                            new XAttribute("RegNr", V.REGNR)))
                                ,
                        from G in Garage<Buss>.GaragesList
                        select new XElement("Garage", new XAttribute("Type", G.GetType()), new XAttribute("Space", G.spaces), new XAttribute("Name", G.Name),
                            from V in G.VehicleList
                            select new XElement("Vehicle",
                                new XAttribute("Type", V.GetType()),
                                new XAttribute("Color", V.Color),
                                new XAttribute("Wheels", V.Wheels),
                                new XAttribute("RegNr", V.REGNR)))
                                ,
                        from G in Garage<Boat>.GaragesList
                        select new XElement("Garage", new XAttribute("Type", G.GetType()), new XAttribute("Space", G.spaces), new XAttribute("Name", G.Name),
                            from V in G.VehicleList
                            select new XElement("Vehicle",
                                new XAttribute("Type", V.GetType()),
                                new XAttribute("Color", V.Color),
                                new XAttribute("Wheels", V.Wheels),
                                new XAttribute("RegNr", V.REGNR)))
                                ,
                        from G in Garage<Car>.GaragesList
                        select new XElement("Garage", new XAttribute("Type", G.GetType()), new XAttribute("Space", G.spaces), new XAttribute("Name", G.Name),
                            from V in G.VehicleList
                            select new XElement("Vehicle",
                                new XAttribute("Type", V.GetType()),
                                new XAttribute("Color", V.Color),
                                new XAttribute("Wheels", V.Wheels),
                                new XAttribute("RegNr", V.REGNR)))
                                ,
                        from G in Garage<Motorcycle>.GaragesList
                        select new XElement("Garage", new XAttribute("Type", G.GetType()), new XAttribute("Space", G.spaces), new XAttribute("Name", G.Name),
                            from V in G.VehicleList
                            select new XElement("Vehicle",
                                new XAttribute("Type", V.GetType()),
                                new XAttribute("Color", V.Color),
                                new XAttribute("Wheels", V.Wheels),
                                new XAttribute("RegNr", V.REGNR)))

                                ));

            var objectsFile = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData), "objects.xml");
            objects.Save(objectsFile);
        }
    }
}
