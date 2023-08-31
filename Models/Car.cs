using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CarStore.Models
{
    public class Car : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string ModelName { 
            get 
            { 
                return modelName; 
            }
            set 
            {
                modelName = value;
                OnPropertyChanged("ModelName");
            } 
        }
        public int Year { 
            get 
            {
                return year;
            }
            set 
            {
                year = value;
                OnPropertyChanged("Year");
            }
        }
        public int Price
        {
            get
            {
                return price;
            }
            set
            {
                price = value;
                OnPropertyChanged("Price");
            }
        }

        public string modelName { get; set; }
        public int year { get; set; }
        public int price { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}