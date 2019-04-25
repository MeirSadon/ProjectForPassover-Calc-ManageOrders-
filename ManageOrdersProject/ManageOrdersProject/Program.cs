using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageOrdersProject
{
    class Program
    {
        static void Main(string[] args)
        {
            ManageOrdersDAO mODAO = new ManageOrdersDAO();
            Console.WriteLine("Welcome To Alibuy");
            string option = "6";
            while (option == "6")
            {
                Console.WriteLine("\n\nChoose What You Want To Do:\n" +
                    "______________________________________\n" +
                    "Press '1' To Login As Customer.\n" +
                    "Press '2' To Create New Customer Account.\n" +
                    "Press '3' To Login As Vendor.\n" +
                    "Press '4' To Create New Vendor Account.\n" +
                    "Press '5' To See All Action.\n" +
                    "Press 0 To Exit.\n");
                {
                    option = Console.ReadLine();
                    Console.WriteLine();
                    if (option == "1")//Login As Customer.
                    {
                        Console.WriteLine("\nEnter Your Details:\n" +
                            "_______________________");
                        Console.Write("UserName: "); string userName = Console.ReadLine();                        
                        Console.Write("Password: "); string password = Console.ReadLine();
                        mODAO.LoginToCustomer(userName, password);
                    }
                    if (option == "2")//Create New Customer.
                    {
                        mODAO.AddNewCustomer();
                    }
                    if (option == "3")//Login As Vendor.
                    {
                        Console.WriteLine("\nEnter Your Details:\n" +
                            "_______________________");
                        Console.Write("UserName: "); string userName = Console.ReadLine();
                        Console.Write("Password: "); string password = Console.ReadLine();
                        mODAO.LoginToVendor(userName, password);
                    }
                    if (option == "4")//Create New Vendor.
                    {
                        mODAO.AddNewVendor();
                    }
                    if (option == "5")//See All Action.
                    {
                        mODAO.ShowActionsList();
                    }
                    if (option == "0")
                    {
                        break;
                    }
                    option = "6";
                }
            }
        }
    }
}
