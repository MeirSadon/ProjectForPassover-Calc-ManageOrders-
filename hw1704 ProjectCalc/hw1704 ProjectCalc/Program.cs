using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw1704_ProjectCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            Calculator c = new Calculator();
            bool PosAdd = true;
            while (PosAdd)
            {
                Console.Write("Press Your First Number:");
                int x = Convert.ToInt16(Console.ReadLine());
                Console.Write("Press Your Second Number:");
                int y = Convert.ToInt16(Console.ReadLine());
                if (x < 1)
                {
                    PosAdd = false;
                    Console.WriteLine("You Can't Add 0 Or Negative Numbers");
                }
                else
                {
                    c.AddNumbers(x, y);
                    Console.WriteLine("Your Numbers Successfully Added!\n\n");
                }

            }
            Console.WriteLine("Here All Options Of Your Calculator:");
            c.PrintTheCalculator();
        }
    }
}
