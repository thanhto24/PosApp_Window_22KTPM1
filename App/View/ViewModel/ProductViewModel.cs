using App.Model;
using App.Service;
using App.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.View.ViewModel
{
    public class ProductViewModel
    {
        private IDao _dao;
        public FullObservableCollection<Product> categories { get; set; }

        public ProductViewModel()
        {
            _dao = Services.GetKeyedSingleton<IDao>();
            List<Product> list_product = _dao.Categories.GetAll();

            categories = new FullObservableCollection<Product>(list_product);

            //foreach (Product p in list_product)
            //{
            //    categories.Add(p);
            //}
        }

        public void Add(Product item)
        {
            categories.Add(item);
        }

        public void Remove(Product item)
        {
            categories.Remove(item);
        }
    }
}
