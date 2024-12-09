using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algos3Lab
{
    public class MatrixRow : INotifyPropertyChanged
    {
        private ObservableCollection<int> _values;

        public ObservableCollection<int> Values
        {
            get => _values;
            set
            {
                _values = value;
                OnPropertyChanged(nameof(Values));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MatrixRow(int size)
        {
            _values = new ObservableCollection<int>(new int[size]);
        }
    }
}
