using MyClass.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClass.DAO
{
    public class SlidersDAO
    {
        private MyDBContext db = new MyDBContext();
        public List<Sliders> getList(string status = "All")
        {
            List<Sliders> list = null;
            switch (status)
            {
                case "Index":
                    {
                        list = db.Sliders
                        .Where(m => m.Status != 0)
                        .ToList();
                        break;
                    }
                case "Trash":
                    {
                        list = db.Sliders
                        .Where(m => m.Status == 0)
                        .ToList();
                        break;
                    }
                default:
                    {
                        list = db.Sliders.ToList();
                        break;
                    }
            }
            return list;
        }
        public List<Sliders> getListByPosition(string position)
        {
            return db.Sliders
              .Where(m => m.Position == position && m.Status == 1)
              .OrderBy(m => m.CreateAt)
              .ToList();
        }
        public Sliders getRow(int? id)
        {
            if (id == null)
            {
                return null;
            }
            else
            {
                return db.Sliders.Find(id);
            }
        }
        public int Insert(Sliders row)
        {
            db.Sliders.Add(row);
            return db.SaveChanges();
        }
        public int Update(Sliders row)
        {
            db.Entry(row).State = EntityState.Modified;
            return db.SaveChanges();
        }
        public int Delete(Sliders row)
        {
            db.Sliders.Remove(row);
            return db.SaveChanges();
        }
    }

}
