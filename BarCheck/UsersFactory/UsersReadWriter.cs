using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace UsersFactory
{
    public class UsersReadWriter
    {
        private string fileName;

        public UsersReadWriter(string fullName)
        {
            this.fileName = fullName;
        }

        public List<User> ReadFile()
        {
            if (!File.Exists(fileName))
                return new List<User>();
            FileStream fs = new FileStream(fileName, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                return (List<User>)formatter.Deserialize(fs);
            }
            catch (Exception ex)
            {
                return new List<User>();
            }
            finally
            {
                fs.Close();
            }
        }

        public void SaveFile(List<User> lst)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, lst);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                fs.Close();
            }
        }
    }
}
