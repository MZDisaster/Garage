using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Garage
{
    class Menu
    {
        public static Menu ActiveMenu;
        public string menuHeader = "";
        public List<string> menuList = new List<string>();
        public string menuFooter = "";
        private Dictionary<string, Action> Methods = new Dictionary<string, Action>();
        public string ErrorMessage = "";
        List<char> userinput1 = new List<char>();
        public string[] userinput;
        private Action defaultMethod = null;
        public string NAME;
        public int firstitemindisplay;
        public bool isPagedMenu = false;
        public int userSelection = 0;
        public bool ForceMethod = false;
        public bool ViewInput = false;

        public Menu()
        {
            if (ActiveMenu == null)
                ActiveMenu = this;

        }

        public void PreviewsPage()
        {
            if(isPagedMenu)
            {
                if(userSelection > 0)
                {
                    userSelection = 0;
                }
                else if ((this.firstitemindisplay - 6) > 0)
                {
                    this.firstitemindisplay -= 7;
                    userSelection = 6;
                }
                else
                {
                    this.firstitemindisplay = 0;
                    userSelection = 0;
                }
            }
                
        }

        public void NextPage()
        {
            if(isPagedMenu)
            {
                if (userSelection < 6)
                {
                    if(this.menuList.Count() > (this.firstitemindisplay + 7))
                        userSelection = 6;
                    else
                        userSelection = this.menuList.Count() - this.firstitemindisplay - 1;
                }
                else if (this.menuList.Count() > (this.firstitemindisplay + 7))
                {
                    this.firstitemindisplay += 7;
                    userSelection = 0;
                }
                else
                {
                    userSelection = this.menuList.Count() - 1;
                }
            }
        }

        public void setdefaultMethod(Action methodname)
        {
            defaultMethod = methodname;
        }

        public void clearuserinput()
        {
            if (userinput1.Count > 0)
            {
                userinput1 = new List<char>();
            }
        }

        public void input(ConsoleKeyInfo CKI)
        {
            if (CKI.Key == ConsoleKey.LeftArrow)
            {
                PreviewsPage();

                if (Methods.ContainsKey("LeftArrow"))
                    Methods["LeftArrow"]();
            }
            else if (CKI.Key == ConsoleKey.RightArrow)
            {
                NextPage();

                if (Methods.ContainsKey("RightArrow"))
                    Methods["RightArrow"]();
            }
            else if(CKI.Key == ConsoleKey.UpArrow)
            {
                if (userSelection > 0)
                    userSelection -= 1;
            }
            else if(CKI.Key == ConsoleKey.DownArrow)
            {
                if (userSelection < (menuList.Count - 1) && userSelection < (firstitemindisplay + 6) && ((userSelection + firstitemindisplay) < menuList.Count - 1))
                    userSelection += 1;
            }
            else if (CKI.Key == ConsoleKey.Backspace)
            {
                if (userinput1.Count > 0)
                {
                    userinput1.RemoveAt(userinput1.Count - 1);

                    if(ForceMethod)
                    {
                        string input = "";
                        foreach (char c in userinput1)
                        {
                            input += c;
                        }

                        string[] splitinput = Regex.Split(input, @"\s");
                        userinput = splitinput;

                        if (defaultMethod != null)
                        {
                            defaultMethod();
                        }
                    }
                }
            }
            else if (CKI.Key == ConsoleKey.Escape)
            {
                if (Methods.ContainsKey("Escape"))
                    Methods["Escape"]();
                userinput1.Clear();
                userinput1 = new List<char>();
            }
            else if(CKI.Key == ConsoleKey.Enter)
            {
                string input = "";
                foreach (char c in userinput1)
                {
                    input += c;
                }

                string[] splitinput = Regex.Split(input, @"\s");
                userinput = splitinput;

                if (Methods.ContainsKey(splitinput[0]))
                    Methods[splitinput[0]]();
                else
                {
                    if (defaultMethod != null)
                    {
                        defaultMethod();
                    }
                    else if (Methods.ContainsKey((userSelection + 1).ToString()))
                        Methods[(userSelection + 1).ToString()]();
                    else
                        ErrorMessage = "Invalid Input!";
                }

                if (Methods.ContainsKey("Enter"))
                {
                    Methods["Enter"]();
                    ErrorMessage = "";
                }
                    

                userinput1.Clear();
                userinput1 = new List<char>();
            }
            else if(Char.IsLetterOrDigit(CKI.KeyChar) || Char.IsWhiteSpace(CKI.KeyChar) || CKI.KeyChar == '-')
            {
                userinput1.Add(CKI.KeyChar);
                string input = "";
                foreach (char c in userinput1)
                {
                    input += c;
                }

                string[] splitinput = Regex.Split(input, @"\s");
                userinput = splitinput;

                if (ForceMethod)
                {
                    if (defaultMethod != null)
                    {
                        defaultMethod();
                    }
                }
            }
        }

        public void setHeader(string header)
        {
            menuHeader = header;
        }

        public void addItem(string item)
        {
            menuList.Add(item);
        }

        public void clearList()
        {
            menuList.Clear();
        }

        public void setList(List<string> list)
        {
            menuList = list;
        }

        public void setFooter(string footer)
        {
            menuFooter = footer;
        }

        public void addMethod(string toInput, Action methodName)
        {
            Methods.Add(toInput, methodName);
        }

        public void removeFromList(int index)
        {
            menuList.RemoveAt(index);
        }

        public override string ToString()
        {
            ActiveMenu = this;

            StringBuilder sb = new StringBuilder();
            //IEnumerable<string> notTheseMethods = Methods.Where(M => M.Key != "Escape" && M.Key != "LeftArrow" && M.Key != "RightArrow" ).Select(M => M.Key);

            sb.AppendLine(menuHeader);
            sb.AppendLine();
            int l = 0;
            for (int i = firstitemindisplay; i < (firstitemindisplay + 7); i++)
            {
                if(menuList.Count() > i)
                    sb.AppendLine(((userSelection % 7) == (i % 7) ? "COLOR1" : "COLOR0") + (l + 1) + ". " + menuList[i]);
                l += 1;
            }
            /*
            foreach (string item in menuList)
            {
                sb.AppendLine(item);
            }*/

            sb.AppendLine();
            sb.AppendLine("COLOR0" + menuFooter);
            if (ErrorMessage != "")
                sb.AppendFormat("COLOR2" + ErrorMessage + "\n");

            string input = "";
            foreach (char c in userinput1)
            {
                input += c;
            }
            //if(notTheseMethods.Count() > 0)
            if(ViewInput)
                sb.AppendFormat("COLOR0" + "\nInput:\t{0}\n", input);
            

            ErrorMessage = ""; 

            return sb.ToString();
        }
    }
}
