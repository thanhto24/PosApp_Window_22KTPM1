using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Model;
using App.Service;
using App.Utils;

namespace App.View.ViewModel
{
    public class CategoryViewModel
    {
        private IDao _dao;
        public FullObservableCollection<Category_> categories { get; set; }

        public CategoryViewModel()
        {
            _dao = Services.GetKeyedSingleton<IDao>();
            List<Category_> list_category = _dao.Categories.GetAll();
            categories = new FullObservableCollection<Category_>(list_category);
        }
    }
}
