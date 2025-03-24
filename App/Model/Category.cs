using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model
{
    public class Category_ : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string name { get; set; }

        public Category_(string name)
        {
            this.name = name;
        }

        public Category_() { }

    }
}
