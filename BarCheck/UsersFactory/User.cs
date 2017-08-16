using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UsersFactory
{
    [Serializable]
    public class User
    {
        public User(string u, string p)
        {
            this.Name = u;
            this.PasswordHash = User.GetPasswordHash(p);
        }
        public string Name;
        public string PasswordHash;
        public static string GetPasswordHash(string pwd)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

    }
}
