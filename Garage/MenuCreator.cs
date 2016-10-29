using System;
using System.Collections.Generic;
using System.Dynamic;
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
        string[] tempgarageinfo = new string[3] { "", "", ""};

        List <List<Menu>> MenuList = new List<List<Menu>>();
        
        public MenuCreator(GarageCreator garageCreator)
        {
            BaseMenu.isPagedMenu = true;
            BaseMenu.setHeader("Garage List:\n type \"new\" to create a new garage!");
            BaseMenu.setFooter("\u25c4 Previews Page\t\tNext Page \u25ba\nESC. Exit");
            BaseMenu.addMethod("new", () => Menu.ActiveMenu = GarageCreateMenu);
            BaseMenu.addMethod("Escape", ExitApp);
            BaseMenu.setdefaultMethod(gotoMenuForGarage);
            BaseMenu.ViewInput = true;

            GarageCreateMenu.setHeader("Welcome to Garage!\nCreate your Garage by selecting a type\n");
            GarageCreateMenu.addItem("AirPlanes");
            GarageCreateMenu.addItem("Boats");
            GarageCreateMenu.addItem("Busses");
            GarageCreateMenu.addItem("Cars");
            GarageCreateMenu.addItem("Motorcycles");
            GarageCreateMenu.setFooter("\u25c4 Reset info\t\tNext Step \u25ba\nESC. Back");
            GarageCreateMenu.addMethod("LeftArrow", () => {
                for (int i = 0; i < tempgarageinfo.Length; i++)
                {
                    tempgarageinfo[i] = "";
                }
            });
            GarageCreateMenu.addMethod("RightArrow", buildGarage);
            GarageCreateMenu.addMethod("Escape", () => Menu.ActiveMenu = BaseMenu);
            GarageCreateMenu.setdefaultMethod(() => creategarage(garageCreator));
            GarageCreateMenu.ViewInput = true;
        }

        private void buildGarage()
        {
            tempgarageinfo[0] = (GarageCreateMenu.userSelection + 1).ToString(); // type
            int number = 0;

            if (tempgarageinfo[1] == "") // name
            {
                if (GarageCreateMenu.userinput != null)
                {
                    tempgarageinfo[1] = GarageCreateMenu.userinput[0];
                    GarageCreateMenu.clearuserinput();
                    GarageCreateMenu.ErrorMessage = "Please enter the size!";
                }
                else
                    GarageCreateMenu.ErrorMessage = "Please enter the name!";
            }
            else if (tempgarageinfo[2] == "") // size
            {
                if (GarageCreateMenu.userinput != null)
                {
                    if (int.TryParse(GarageCreateMenu.userinput[0], out number))
                    {
                        tempgarageinfo[2] = number.ToString();
                        GarageCreateMenu.clearuserinput();
                        GarageCreateMenu.ErrorMessage = "Press Enter to Create!";
                    }
                    else
                        GarageCreateMenu.ErrorMessage = "Please enter the size!";
                }
            }
            else if(tempgarageinfo[0] != "" && tempgarageinfo[1] != "" && tempgarageinfo[2] != "")
            {
                GarageCreateMenu.ErrorMessage = "Press Enter to Create!";
            }
            /*
            tempgarageinfo.name = "";
            tempgarageinfo.size = 1;
            tempgarageinfo.type = 1;*/

        }

        private void creategarage(GarageCreator garageCreator)
        {
            int number = 0;
            int number2 = 0;
            if (tempgarageinfo[0] == "")
                GarageCreateMenu.ErrorMessage = "Please type the name!";
            else if (!int.TryParse(tempgarageinfo[0], out number) || !int.TryParse(tempgarageinfo[2], out number2))
                GarageCreateMenu.ErrorMessage = "You entered a string insdead of number in the size field!";
            else if (number == 0 || number2 == 0)
                GarageCreateMenu.ErrorMessage = "Really 0?";
            else
            {
                switch (number)
                {
                    case 1:
                        garageCreator.createGarage<Airplane>(tempgarageinfo[1], number2);
                        GarageCreateMenu.ErrorMessage = "Created a New Airplanes Garage!";
                        break;
                    case 2:
                        garageCreator.createGarage<Boat>(tempgarageinfo[1], number2);
                        GarageCreateMenu.ErrorMessage = "Created a New Boats Garage!";
                        break;
                    case 3:
                        garageCreator.createGarage<Buss>(tempgarageinfo[1], number2);
                        GarageCreateMenu.ErrorMessage = "Created a New Busses Garage!";
                        break;
                    case 4:
                        garageCreator.createGarage<Car>(tempgarageinfo[1], number2);
                        GarageCreateMenu.ErrorMessage = "Created a New Cars Garage!";
                        break;
                    case 5:
                        garageCreator.createGarage<Motorcycle>(tempgarageinfo[1], number2);
                        GarageCreateMenu.ErrorMessage = "Created a New Motorcycles Garage!";
                        break;
                    default:
                        //GarageCreateMenu.ErrorMessage = "Numbers from the list please!";
                        //tempgarageinfo.Clear();
                        break;
                }
            }

            for (int i = 0; i < tempgarageinfo.Length; i++)
            {
                tempgarageinfo[i] = "";
            }
        }

        public void gotoMenuForGarage() // garage specific menus created when the garage was created
        {
            int index = Menu.ActiveMenu.userSelection;
            //int.TryParse(BaseMenu.userinput[0], out index);
            if(MenuList.Count >= (index + BaseMenu.firstitemindisplay))
            {
                Menu.ActiveMenu = MenuList[(index + BaseMenu.firstitemindisplay)][0];

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
            MainMenu.addItem("Add a Vehicle.");
            MainMenu.addItem("View vehicles and manage them.");
            MainMenu.addItem("Search for Vehicle.");
            MainMenu.addItem("Change Garage size.");
            MainMenu.setFooter("ESC. Back.");

            MainMenu.setdefaultMethod(() => NavigateThrewMenu(MainGarage, menulistofgarage));
            MainMenu.addMethod("Escape", () => Menu.ActiveMenu = BaseMenu);


            GarageSizeMenu.setHeader("Type a number to change the garage size:");
            GarageSizeMenu.setFooter("ESC. Back.");
            GarageSizeMenu.addMethod("Escape", () => gotoMainMenu(MainGarage, MainMenu));
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
            GarageSizeMenu.ViewInput = true;

            VehicleListMenu.isPagedMenu = true;
            VehicleListMenu.setFooter("\u25c4 Previews Page\t\tNext Page \u25ba\nESC. Exit");
            VehicleListMenu.addMethod("Escape", () => gotoMainMenu(MainGarage, MainMenu));
            VehicleListMenu.addMethod("LeftArrow", () => {
                ViewVehicleList(MainGarage, VehicleListMenu);
            });
            VehicleListMenu.addMethod("RightArrow", () =>
            {
                ViewVehicleList(MainGarage, VehicleListMenu);
            });
            VehicleListMenu.addMethod("Enter", () =>
            {
                remove(MainGarage, VehicleListMenu);
                ViewVehicleList(MainGarage, VehicleListMenu);
            });
            
            searchMenu.isPagedMenu = true;
            searchMenu.setHeader(string.Format("Enter search term:\tYou can search for color, wheels, regnr\nPress Enter to remove selected vehicle!\n\n   {0, -10}{1, -15}{2, -10}{3}", "Type", "RegNr", "Color", "Wheels"));
            searchMenu.setFooter("\u25c4 Previews Page\t\tNext Page \u25ba\nESC. Exit");
            searchMenu.ForceMethod = true;
            searchMenu.setdefaultMethod(() => Search(MainGarage, searchMenu));
            searchMenu.addMethod("Escape", () => {
                gotoMainMenu(MainGarage, MainMenu);
                searchMenu.clearList();
                if(MainGarage.SearchResult != null)
                    MainGarage.SearchResult.Clear();
            });
            searchMenu.addMethod("Enter", () => remove(MainGarage, searchMenu));
            searchMenu.ViewInput = true;

            AddVehicleMenu.setHeader("Enter vehicle info: (Color Wheels Regnr)");
            AddVehicleMenu.setFooter("ESC. Back.");
            AddVehicleMenu.addMethod("Escape", () => gotoMainMenu(MainGarage, MainMenu));
            AddVehicleMenu.addMethod("Enter", () => addvehicle(MainGarage));
            AddVehicleMenu.setdefaultMethod(()=> addvehicleCheck(MainGarage));
            AddVehicleMenu.ForceMethod = true;
            AddVehicleMenu.ViewInput = true;

            return MainMenu;
        }

        private void NavigateThrewMenu<T>(Garage<T> garage, List<Menu> menulistofgarage) where T : Vehicle
        {
            Menu MainMenu = menulistofgarage[0];
            switch(MainMenu.userSelection)
            {
                case 0:
                    Menu.ActiveMenu = menulistofgarage[3];
                    break;
                case 1:
                    ViewVehicleList(garage, menulistofgarage[1]);
                    break;
                case 2:
                    Menu.ActiveMenu = menulistofgarage[2];
                    break;
                case 3:
                    Menu.ActiveMenu = menulistofgarage[4];
                    break;
            }
        }

        private void addvehicleCheck<T>(Garage<T> garage) where T:Vehicle
        {
            if(Menu.ActiveMenu.userinput.Length > 2)
            {
                int wheels;
                int.TryParse(Menu.ActiveMenu.userinput[1], out wheels);
                int regnr;
                int.TryParse(Menu.ActiveMenu.userinput[2], out regnr);


                if (wheels > 0 && regnr > 0 && wheels < 99)
                    Menu.ActiveMenu.ErrorMessage = "Valid Input!"; 
                else
                    Menu.ActiveMenu.ErrorMessage = "Invalid Input!";
            }
            else
            {
                Menu.ActiveMenu.ErrorMessage = "Invalid Input!";
            }
        }

        private void addvehicle<T>(Garage<T> garage) where T : Vehicle
        {
            if (Menu.ActiveMenu.userinput.Length > 2)
            {
                int wheels;
                int.TryParse(Menu.ActiveMenu.userinput[1], out wheels);
                int regnr;
                int.TryParse(Menu.ActiveMenu.userinput[2], out regnr);


                if (wheels > 0 && regnr > 0 && wheels < 99)
                {
                    garage.addToGarage<T>(Menu.ActiveMenu.userinput[0], wheels, regnr);
                    Menu.ActiveMenu.ErrorMessage = "Success!!!";
                }
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
            int number = menu.userSelection;
            //int.TryParse(Menu.ActiveMenu.userinput[0], out number);
            
            if (number >= 0 && number < 7)
            {
                if (garage.VehicleList.Count() > 0 && number < garage.VehicleList.Count())
                {
                    if (garage.VehicleList.Count() >= (menu.firstitemindisplay + number) + 1)
                    {
                        garage.removeFromGarage<T>(garage.VehicleList[(menu.firstitemindisplay + number)]);
                        menu.removeFromList((menu.firstitemindisplay + number));
                        //ViewVehicleList(garage, menu);
                        if (menu.menuList.Count <= (menu.firstitemindisplay + number))
                            menu.userSelection -= 1;
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
                int number1; // temp
                int number;
                number = int.TryParse(Menu.ActiveMenu.userinput[0], out number1) ? number1 : 0;

                
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
                        if(newnumber > 0 && newnumber <= 7)
                        {
                            if (garage.SearchResult.Count > ((menu.firstitemindisplay + newnumber) - 1))
                            {
                                garage.removeFromGarage<T>(garage.SearchResult[(menu.firstitemindisplay + newnumber) - 1]);
                                garage.SearchResult.RemoveAt((menu.firstitemindisplay + newnumber) - 1);
                            }
                            else
                            {
                                menu.ErrorMessage = "Invalid input!";
                            }
                        }
                        else
                        {
                            menu.ErrorMessage = "Invalid input!";
                        }
                    }
                }
            }
            else if(Menu.ActiveMenu.userinput.Count() == 2) // user inputs 2 things
            {
                int number1;
                int number;
                number = int.TryParse(Menu.ActiveMenu.userinput[0], out number1) ? number1 : int.TryParse(Menu.ActiveMenu.userinput[1], out number1) ? number1 : 0;
                string userinput = "";
                //List<string> userinput = new List<string>();
                foreach (string input in Menu.ActiveMenu.userinput) // put the input in the respective variables for later usage
                {
                    if (!int.TryParse(input, out number1))
                        userinput = (input);
                }

                if (number > 0 && userinput != "") // if it's a number and a string
                {
                    garage.SearchResult = garage.searchVehicle<T>(number, userinput);
                }
                else if(number > 0 && number1 > 0) // if it's 2 numbers
                {
                    garage.SearchResult = garage.searchVehicle<T>(number, number1);
                }
            }
            else if (Menu.ActiveMenu.userinput.Count() == 3) // user inputs 3 things
            {
                int number1;
                int tempnumber = 0;
                int number;
                number = int.TryParse(Menu.ActiveMenu.userinput[0], out number1) ? number1 : int.TryParse(Menu.ActiveMenu.userinput[1], out number1) ? number1 : int.TryParse(Menu.ActiveMenu.userinput[2], out number1) ? number1 : 0;
                string userinput = "";
                //List<string> userinput = new List<string>();
                foreach (string input in Menu.ActiveMenu.userinput) // put the input in the respective variables for later usage
                {
                    if (!int.TryParse(input, out number1))
                        userinput = (input);
                    else
                        tempnumber = int.Parse(input);
                }

                if (number > 0 && tempnumber > 0 && userinput != "")
                {
                    garage.SearchResult = garage.searchVehicle<T>(userinput, number, tempnumber);
                }
            }

            if (garage.SearchResult != null)
            {
                if(garage.SearchResult.Count > 0)
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
                {
                    menu.clearList();
                    menu.ErrorMessage = "No results!";
                }
            }
            else
            {
                menu.clearList();
                menu.ErrorMessage = "No results!";
            }
                
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
