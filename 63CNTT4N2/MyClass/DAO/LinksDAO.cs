using MyClass.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClass.DAO
{
    public class LinksDAO
    {
        private MyDBContext db = new MyDBContext();
        public List<Links> getList(string status = "All")
        {
            List<Links> list = null;
            return list;
        }
        public Links getRow(int tableid, string typelink)
        {
            return db.Links
                .Where(m => m.TableId == tableid && m.Type == typelink)
                .FirstOrDefault(); 
        }
        public Links getRow(string slug)
        {
            return db.Links
                .Where(m => m.Slug == slug)
                .FirstOrDefault();
        }
        public int Insert(Links row)
        {
            db.Links.Add(row);
            return db.SaveChanges();
        }
        public int Update(Links row)
        {
            db.Entry(row).State = EntityState.Modified;
            return db.SaveChanges();
        }
        public int Delete(Links row)
        {
            db.Links.Remove(row);
            return db.SaveChanges();
        }
    }
}
