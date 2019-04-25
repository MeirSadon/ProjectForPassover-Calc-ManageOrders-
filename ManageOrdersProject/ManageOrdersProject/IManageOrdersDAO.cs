using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageOrdersProject
{
     interface IManageOrdersDAO
    {
        //Customers.
        Dictionary<int, dynamic> ReadAllCustomers();
        void AddNewCustomer();
        void LoginToCustomer(string userName, string password);

        //Vendors.
        Dictionary<int, dynamic> ReadAllVendors();
        void AddNewVendor();
        void LoginToVendor(string userName, string password);

        //Products.
        Dictionary<int, object> ReadAllProducts();
        void AddOrEditProduct(int venId);

        //Actions.
        void ShowActionsList();

    }
}
