using System;
using System.ComponentModel;

namespace App.Model
{
    public class Category_ : INotifyPropertyChanged
    {
        private string _name;
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public Category_(string name)
        {
            _name = name;
        }

        public Category_() { }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
