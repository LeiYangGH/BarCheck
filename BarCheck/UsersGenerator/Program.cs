using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersFactory;

namespace UsersGenerator
{
    class Program
    {
        private static string usersFileName = Path.Combine(Environment.CurrentDirectory, "Users");
        private static string inputFileName = Path.Combine(Environment.CurrentDirectory, "users.txt");
        private static UsersReadWriter usersRW;
        private static List<User> lstUsers;
        static void Main(string[] args)
        {
            try
            {
                lstUsers = File.ReadAllLines(inputFileName)
                .Select(x => x.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Where(y => y.Length == 2)
                .Select(z => new User(z[0], z[1]))
                .ToList();
                usersRW = new UsersReadWriter(usersFileName);
                usersRW.SaveFile(lstUsers);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine(usersFileName);
            Console.ReadLine();
        }
    }
}
