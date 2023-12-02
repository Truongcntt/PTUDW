using MyClass.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClass.DAO
{
    public class UsersDAO
    {
        private MyDBContext db = new MyDBContext();
        public List<Users> getList(string status = "All")
        {
            List<Users> list = null;
            switch (status)
            {
                case "Index":
                    {
                        list = db.Users
                            .Where(m => m.Status != 0)
                     .ToList();
                        break;
                    }
                case "Trash":
                    {
                        list = db.Users
                     .Where(m => m.Status == 0)
                     .ToList();
                        break;
                    }
                default:
                    {
                        list = db.Users.ToList();
                        break;
                    }
            }
            return list;
        }

        public Users GetUserByUsername(string username)
        {
            using (var context = new MyDBContext()) 
            {
                return context.Users.FirstOrDefault(u => u.Username == username);
            }
        }

        public Users getRow(int? id)
        {
            if (id == null)
            {
                return null;
            }
            else
            {
                return db.Users.Find(id);
            }
        }

        public int Insert(Users row)
        {
            db.Users.Add(row);
            return db.SaveChanges();
        }

        public int Update(Users row)
        {
            db.Entry(row).State = EntityState.Modified;
            return db.SaveChanges();
        }

        public int Delete(Users row)
        {
            db.Users.Remove(row);
            return db.SaveChanges();
        }
    }
}
