using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw1704_ProjectCalc
{
    class Calculator : ICalculator
    {
        static SqlConnection conn;
        String sqlPath = @"Data Source=MEIR-PC\SQLEXPRESS;Initial Catalog=Calc;Integrated Security=true;";

        public Calculator()
        {
            conn = new SqlConnection($"{sqlPath}");
            conn.Open();
        }
        public void AddNumbers(int x, int y)
        {
                using (SqlCommand cmd = new SqlCommand($"Insert Into XTable Values ({x}); Insert Into YTable Values ({y});", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        public void PrintTheCalculator()
        {
            using (SqlCommand cmd = new SqlCommand("Show_My_Calculator", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.Default))
                {
                    while (reader.Read() == true)
                    {
                        Console.WriteLine($" {reader["N1"]} {reader["Operation"]} {reader["N2"]} {reader["Result"]}");
                    }
                }
            }
        }
    }
}
