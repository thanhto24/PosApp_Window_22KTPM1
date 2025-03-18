using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Service
{
    public interface IRepository<T>
    {
        List<T> GetAll(); // Phân trang, sắp xếp, Lọc
        void Insert(T entity);
        void RemoveByQuery(string whereClause, Dictionary<string, object> parameters);
        public void UpdateByQuery(Dictionary<string, object> setValues, string whereClause, Dictionary<string, object> whereParams);
    }
}
