using MyClass.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClass.DAO
{
    public class TopicsDAO
    {
        private MyDBContext db = new MyDBContext();
        public List<Topics> getList(string status = "All")
        {
            List<Topics> list = null;
            switch (status)
            {
                case "Index":
                    {
                        list = db.Topics
                        .Where(m => m.Status != 0)
                        .ToList();
                        break;
                    }
                case "Trash":
                    {
                        list = db.Topics
                        .Where(m => m.Status == 0)
                        .ToList();
                        break;
                    }
                default:
                    {
                        list = db.Topics.ToList();
                        break;
                    }
            }
            return list;
        }
        public Topics getRow(int? id)
        {
            if (id == null)
            {
                return null;
            }
            else
            {
                return db.Topics.Find(id);
            }
        }
        //Hien thi danh sach 1 mau tin (ban ghi)
        public Topics getRow(string slug)
        {
            return db.Topics
                .Where(m => m.Slug == slug && m.Status == 1)
                .FirstOrDefault();
        }
        public int Insert(Topics row)
        {
            db.Topics.Add(row);
            return db.SaveChanges();
        }
        public int Update(Topics row)
        {
            db.Entry(row).State = EntityState.Modified;
            return db.SaveChanges();
        }
        public int Delete(Topics row)
        {
            db.Topics.Remove(row);
            return db.SaveChanges();
        }
    }
}
