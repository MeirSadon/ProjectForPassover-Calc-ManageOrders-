using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageOrdersProject
{
    public class ManageOrdersDAO : IManageOrdersDAO
    {
        static SqlConnection conn = new SqlConnection(@"Data Source=MEIR-PC\SQLEXPRESS ;Initial Catalog=ManageOrders; Integrated Security=true;");

        public ManageOrdersDAO()
        {
            conn.Open();
        }
        //******************   Insert All Customers To Dictionary By User Name   ******************
        public Dictionary<int, dynamic> ReadAllCustomers()
        {
            Dictionary<int, dynamic> cusById = new Dictionary<int, dynamic>();
            using (SqlCommand cmd = new SqlCommand($"Select * from Customers", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read() == true)
                    {
                        dynamic cd = new
                        {
                            CusId = (int)reader["cusId"],
                            UserName = (string)reader["UserName"],
                            Password = (string)reader["Password"],
                            FirstName = (string)reader["FirstName"],
                            LastName = (string)reader["LastName"],
                            CardNumber = (int)reader["CardNumber"],
                        };
                        cusById.Add(cd.CusId, cd);
                    }
                }
            }
            return cusById;
        }
        //*************************   Add New Customer   *************************
        public void AddNewCustomer()
        {
            Console.WriteLine("\nEnter Your Details:\n" +
                "_______________________");
            Console.Write("1/5. UserName: "); string UserName = Console.ReadLine();
            Console.Write("2/5. Password: "); string Password = Console.ReadLine();
            Console.Write("3/5. First Name: "); string FirstName = Console.ReadLine();
            Console.Write("4/5. Last Name: "); string LastName = Console.ReadLine();
            Console.Write("5/5. Card Number: "); int CardNumber = Convert.ToInt32(Console.ReadLine());
            Dictionary<int, dynamic> tempcusById = ReadAllCustomers();
            foreach(KeyValuePair<int, dynamic> cus in tempcusById)
            {
                while (cus.Value.UserName == UserName)
                {
                    /*Etgar*/using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{"Try Create Account With UserName That In Use"}', '{false}')", conn)) {cmd.ExecuteNonQuery();}
                        Console.WriteLine($"\nThe UserName '{UserName}' Is Already Exist... Try Another UserName:");
                    UserName = Console.ReadLine();
                }
            }
            using (SqlCommand cmd = new SqlCommand(/*Etgar*/$"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{"Create New User As Customer"}', '{true}');" +
                $"Insert Into Customers(UserName, Password, FirstName, LastName, CardNumber) values('{UserName}','{Password}'," +
                    $"'{FirstName}','{LastName}',{CardNumber})", conn))
                {
                        cmd.ExecuteNonQuery();
                }
            Console.WriteLine("\nYour Customer Account Was Successfully Created !");

        }

        public void ShowActionsList()
        {
            int x = 1;
            using (SqlCommand cmd = new SqlCommand($"Select * From Actions", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read() == true)
                    {
                        Console.WriteLine($"{x++}) \n___ \n|#|Date: {reader["Date"]}. |#|The Act: {reader["Type"]}. |#|It's Suceed? {reader["ItsSucceed"]}.\n");
                    }
                }
            }
        }

        //*********************   Login To Exist Customer   *********************
        public void LoginToCustomer(string userName, string password)
     {
            Dictionary<int, dynamic> tempcusById = ReadAllCustomers();
            foreach (KeyValuePair<int, dynamic> cus in tempcusById)
            {
                if (userName == cus.Value.UserName && password == cus.Value.Password)
                {
                    string option = "4";
                    Dictionary<int, object> allProducts;

                    Console.WriteLine("\n\nLogin successfully!");
                    /*Etgar*/using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"Login To Customer Account{cus.Value.UserName}"}', '{true}')", conn)) {cmd.ExecuteNonQuery();}
                    while (option == "4")
                    {
                        Console.WriteLine("\n-- Press '1' To See All Of Your Orders.\n" +
                            "-- Press '2' To See All Products.\n" +
                            "-- Press '3' To Order New Product.\n" +
                            "-- Press '0' To Log Out From Your Account.");
                        option = Console.ReadLine();
                        if (option == "1")
                        {
                            int x = 1;
                            int TotalPriceOrders = 0;
                            Console.WriteLine("\n\nHere All Of Your Orders:\n" +
                                "______________________________");
                             using (SqlCommand cmd = new SqlCommand(/*Etgar*/$"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"{cus.Value.UserName} Watch His Orders List"}', '{true}');" +
                                $"select ProName, Ammount,TotalPrice from Orders o join Products p on o.Product_Id=p.ProId where Customer_Id ={cus.Key}",conn))
                            {
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read() == true)
                                    {
                                        Console.Write($"{x++}) Product Name: {reader["ProName"]}. Ammount: {reader["Ammount"]}. Total Price: {reader["TotalPrice"]}.\n");
                                        TotalPriceOrders += (int)reader["TotalPrice"];
                                    }
                                    Console.WriteLine($"\n**) Total Price For All Of Your Orders Is: {TotalPriceOrders}$.\n");
                                }
                            }
                        }
                        if (option == "2")
                        {
                            int x = 1;
                            allProducts = ReadAllProducts();
                            Console.WriteLine("\n\nHere All The Products:\n" +
                                "_________________________");
                            foreach(KeyValuePair<int,dynamic> pro in allProducts)
                            {
                                Console.WriteLine($"{x++}) Product Name: {pro.Value.ProName}. Price: {pro.Value.Price}. Stock: {pro.Value.Stock}.");
                            }
                            /*Etar*/using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"{cus.Value.UserName} Watch The Products List"}', '{true}')", conn)) { cmd.ExecuteNonQuery(); }
                            Console.WriteLine();
                        }
                        if (option == "3") //Create New Order.
                        {
                            allProducts = ReadAllProducts();
                            bool orderIsDone = false;
                            Console.Write("\nEnter The Name Of The Product You Want To Order:\n");
                            string nameOfProduct = Console.ReadLine();
                            int amount;
                            foreach (KeyValuePair<int, dynamic> pro in allProducts)
                            {
                                if (pro.Value.ProName == nameOfProduct && pro.Value.Stock > 0)
                                {
                                    Console.Write("\nEnter How Much You Want To Order: ");
                                    amount = Convert.ToInt32(Console.ReadLine());
                                    if (amount > pro.Value.Stock) //Out Of The Stock.
                                    {
                                        Console.WriteLine("\n** Don't Have Enough Items At The Stock **\n");
                                        /*Etar*/using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"{cus.Value.UserName} Try To Order Somthing"}', '{false}')", conn)) { cmd.ExecuteNonQuery(); }
                                        orderIsDone = true;
                                    }
                                    else //Have Enough At The Stock.
                                    {
                                        using (SqlCommand cmd = new SqlCommand(/*Etgar*/$"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"{cus.Value.UserName} Order Somthing"}', '{true}');" +
                                            $"Insert Into Orders(Customer_Id,Product_Id,Ammount,TotalPrice) values({cus.Key},{pro.Key},{amount},{pro.Value.Price * amount});" +
                                            $"Update Products Set Stock={pro.Value.Stock - amount} Where Products.ProName Like '{nameOfProduct}'", conn))
                                        {
                                            cmd.ExecuteNonQuery();
                                        }
                                        Console.WriteLine("\nYour Order Has Been Successful !\n"); //The Order Is Done.
                                        orderIsDone = true;
                                    }
                                }
                                if (pro.Value.ProName == nameOfProduct && pro.Value.Stock < 1)  //Stock Is 0.
                                {
                                    /*Etgar*/ using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"{cus.Value.UserName} Try To Order Somthing"}', '{false}')", conn)) { cmd.ExecuteNonQuery(); }
                                    Console.WriteLine("\n** The Stock For This Product Is Over **\n");
                                    orderIsDone = true;
                                }
                            }
                            if (orderIsDone == false) // Not Found The Item.
                            {
                                /*Etgar*/using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"{cus.Value.UserName} Try To Order Somthing"}', '{false}')", conn)) { cmd.ExecuteNonQuery(); }
                                Console.WriteLine("\n** Your Product Is Not Exist **\n");
                            }
                        }
                            if (option == "0")//Exit From The Current Main
                            {
                            /*Etgar*/using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"{cus.Value.UserName} Log Out From His Account"}', '{true}')", conn)) { cmd.ExecuteNonQuery(); }
                            Console.WriteLine("\nYou Log Out From Your Account\n");
                            return;
                            }
                            option = "4";
                    }
                }
            }
            foreach (KeyValuePair<int, dynamic> cus in tempcusById)
            {
                if (userName == cus.Value.UserName && password != cus.Value.Password)
                {
                    /*Etgar*/ using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"{cus.Value.UserName} Try To Login With Wrong Password"}', '{false}')", conn)) { cmd.ExecuteNonQuery(); }
                    Console.WriteLine("\n** Your Password Isn't Match To Your User Name!\n");
                    return;
                }
            }
            /*Etgar*/ using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"Chance To Login As Customer With Wrong UserName"}', '{false}')", conn)) { cmd.ExecuteNonQuery(); }
            Console.WriteLine("\n** Your UserName Is Not Found!\n");
            return;
        }


        //******************   Insert All Vendors To Dictionary By ID   ******************
        public Dictionary<int, dynamic> ReadAllVendors()
        {
            Dictionary<int, dynamic> venById = new Dictionary<int, dynamic>();
            using (SqlCommand cmd = new SqlCommand($"Select * from Vendors", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read() == true)
                    {
                        dynamic dv = new
                        {
                            VenId = (int)reader["venId"],
                            UserName = (string)reader["UserName"],
                            Password = (string)reader["Password"],
                            CompanyName = (string)reader["CompanyName"],
                        };
                        venById.Add(dv.VenId, dv);
                    }
                }
            }
            return venById;
        }
            //*************************   Add New Vendor   *************************
        public void AddNewVendor()
        {
            Console.WriteLine("\nEnter Your Details:\n" +
                 "_______________________");
            Console.Write("1/3. UserName: "); string UserName = Console.ReadLine();
            Console.Write("2/3. Password: "); string Password = Console.ReadLine();
            Console.Write("3/3. Company Name: "); string CompanyName = Console.ReadLine();
            Dictionary<int, dynamic> tempvenById = ReadAllVendors();
            foreach (KeyValuePair<int, dynamic> ven in tempvenById)
            {
                while (ven.Value.UserName == UserName)
                {
                    /*Etgar*/ using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{"Try Create Account With UserName That In Use"}', '{false}')", conn)) { cmd.ExecuteNonQuery(); }
                    Console.WriteLine($"\nThe UserName '{UserName}' Is Already Exist... Try Another UserName:");
                    UserName = Console.ReadLine();
                }
            }
            using (SqlCommand cmd = new SqlCommand(/*Etgar*/$"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{"Create New User As Vendor"}', '{true}');" +
                $"Insert Into Vendors(UserName, Password, CompanyName) values('{UserName}','{Password}','{CompanyName}')", conn))
            {
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("\nYour Vendor Account Was Successfully Created !");
        }
        //*********************   Login To Exist Vendor   *********************
        public void LoginToVendor(string userName, string password)
        {
            Dictionary<int, dynamic> tempVenById = ReadAllVendors();
            foreach (KeyValuePair<int, dynamic> ven in tempVenById)
            {
                if (userName == ven.Value.UserName && password == ven.Value.Password)
                {
                    /*Etgar*/using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{"Login To Vendor Account"}', '{true}')", conn)) { cmd.ExecuteNonQuery(); }
                    Console.WriteLine("\nLogin successfully!\n");
                    string option = "3";
                    while (option == "3")
                    {
                        Console.WriteLine("\n-- Press '1' To Add/Edit Product.\n" +
                            "-- Press '2' To See All Your Products.\n" +
                            "-- Press '0' To Log Out From Your Account.\n");
                        option = Console.ReadLine();
                        if (option == "1")
                        {
                            Console.WriteLine("\n\nPlease Enter Details For Your Product:\n" +
                                "_____________________________________");
                            AddOrEditProduct(ven.Value.VenId);
                        }
                        if (option == "2")
                        {
                            int x = 1;
                            Console.WriteLine("\n\nHere All Of Your Products:\n" +
                                "___________________________________");
                            using (SqlCommand cmd = new SqlCommand(/*Etgar*/$"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"{ven.Value.UserName} Watch His Products List"}', '{true}');" +
                                $"Select ProName,Price,Stock from Products Where Vendor_Id={ven.Value.VenId}",conn))
                            {
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read() == true)
                                    {
                                            Console.WriteLine($"{x++}) Product Name: {reader["ProName"]}. Price: {reader["Price"]}$. Ammount At Stock: {reader["Stock"]}.");
                                    }
                                    Console.WriteLine();
                                }
                            }
                        }
                        if (option == "0")
                        {
                            /*Etgar*/
                            using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"{ven.Value.UserName} Log Out From His Account"}', '{true}')", conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            Console.WriteLine("\nYou Log Out From Your Account\n");
                            return;
                        }
                        option = "3";
                    }
                }
            }
            foreach (KeyValuePair<int, dynamic> ven in tempVenById)
            {
                if (userName == ven.Value.UserName && password != ven.Value.Password)
                {
                    /*Etgar*/using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"{ven.Value.UserName} Try To Login With Wrong Password"}', '{false}')", conn)) { cmd.ExecuteNonQuery(); }
                    Console.WriteLine("\n** Your Password Isn't Match To Your User Name **\n");
                    return;
                }
            }
            /*Etgar*/using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"Chance To Login As Vendor With Wrong UserName"}', '{false}')", conn)) { cmd.ExecuteNonQuery(); }
            Console.WriteLine("\n** Your User Name Isn't Found **\n");
            return;
        }




        //******************   Insert All Products To Dictionary By ID   ******************
        public Dictionary<int, object> ReadAllProducts()
        {
            Dictionary<int, dynamic> proById = new Dictionary<int, dynamic>();
            using (SqlCommand cmd = new SqlCommand($"Select * from products", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read() == true)
                    {
                        dynamic pd = new
                        {
                            ProId = (int)reader["ProId"],
                            ProName = (string)reader["ProName"],
                            Vendor_Id = (int)reader["Vendor_Id"],
                            Price = (int)reader["Price"],
                            Stock = (int)reader["Stock"]

                        };
                        proById.Add(pd.ProId, pd);
                    }
                }
            }
            return proById;
        }
        // ***************   Add/Edit Product   ***************
        public void AddOrEditProduct(int venId)
        {
            bool UpdateProduct = false;
            int ammount;
            int price;
            string optionForProduct = "3";
            Dictionary<int, dynamic> tempProById = ReadAllProducts();
            Console.Write("Name: ");
            string name = Console.ReadLine();
            foreach (KeyValuePair<int, dynamic> pro in tempProById)
            {
                if (pro.Value.ProName == name)
                {
                    if (pro.Value.Vendor_Id == venId)
                    {
                        Console.Write("\nYou Already Promoted This Product.");
                        while (optionForProduct == "3")
                        {
                            Console.Write("\n\nPress '1' To Change The Amount At The Stock.\n" +
                                "Press '2' To Change The price.\n" +
                                "Press '0' To Exit.\n");
                            optionForProduct = Console.ReadLine();
                            if (optionForProduct == "1")//Change The Ammount At Stock.
                            {
                                Console.WriteLine("\nEnter New Ammount To Stock For This Item: ");
                                ammount = Convert.ToInt32(Console.ReadLine());
                                using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"Vendor Change Ammount Of The Stock For His Product"}', '{true}');" +
                                    $"Update Products Set Price={ammount} Where Vendor_Id={venId} And ProName Like '{name}'", conn))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                Console.WriteLine($"The Stock Update To {ammount} Units.");
                            }
                            if (optionForProduct == "2")//Change The Price.
                            {
                                Console.WriteLine("\nEnter New Price For This Item: ");
                                price = Convert.ToInt32(Console.ReadLine());
                                using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"Vendor Change The Price For His Product"}', '{true}');" +
                                    $"Update Products Set Price={price} Where Vendor_Id={venId} And ProName Like '{name}'", conn))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                Console.WriteLine($"Now, The Price For This Product Is {price}$ .");
                            }
                            if (optionForProduct == "0")
                            {
                                UpdateProduct = true;
                                break;
                            }
                            optionForProduct = "3";
                        }
                    }
                    if(pro.Value.Vendor_Id != venId)
                    {
                        /*Etgar*/using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"Vendor Try To Add\\Edit Product Of Another Vendor"}', '{false}')", conn)) { cmd.ExecuteNonQuery(); }
                        Console.WriteLine("Sorry, But This Product Already Promoted By Another Vendor!\n");
                        UpdateProduct = true;
                    }
                }
            }
            if (UpdateProduct == false)
            {
                Console.Write("Price: "); price = Convert.ToInt32(Console.ReadLine());
                Console.Write("Ammount: "); ammount = Convert.ToInt32(Console.ReadLine());
                using (SqlCommand cmd = new SqlCommand($"Insert Into Actions(Date,Type,ItsSucceed) Values('{DateTime.Now}','{$"Vendor Add New Product"}', '{true}')" +
                    $"Insert Into Products(ProName,Vendor_Id, Price, Stock) Values('{name}',{venId}, {price}, {ammount})", conn))
                {
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("\nGood! Your New Product Is Add To Your Products List!\n");
            }
        }
    }
}