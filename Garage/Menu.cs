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
        public string[] userinput;
        private Action defaultMethod = null;
        public string NAME;

        public Menu()
        {
            if (ActiveMenu == null)
                ActiveMenu = this;

        }

        public void setdefaultMethod(Action methodname)
        {
            defaultMethod = methodname;
        }

        public void clearuserinput()
        {
            if (userinput[0] != "")
                for (int i = 0; i < userinput.Length; i++)
                {
                    userinput[i] = "";
                }
        }

        public void input(string input)
        {
            string[] splitinput = Regex.Split(input, @"\s");
            userinput = splitinput;

            if (Methods.ContainsKey(splitinput[0]))
                Methods[splitinput[0]]();
            else
            {
                if (splitinput[0] != "")
                {
                    if (defaultMethod != null)
                    {
                         defaultMethod();
                    }
                    else
                        ErrorMessage = "Invalid Input!";
                }
                else
                    ErrorMessage = "Invalid Input!";
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

            sb.AppendLine(menuHeader);
            sb.AppendLine();

            foreach (string item in menuList)
            {
                sb.AppendLine(item);
            }

            sb.AppendLine();
            sb.AppendLine(menuFooter);
            if (ErrorMessage != "")
                sb.AppendFormat(ErrorMessage + "\n");
            sb.AppendLine("\nInput:\t");

            ErrorMessage = ""; 

            return sb.ToString();
        }
    }
}
