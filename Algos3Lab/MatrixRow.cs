using Algos3Lab;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

public class MatrixRow : INotifyPropertyChanged
{
    private ObservableCollection<MatrixCellValue> _values;
    public ObservableCollection<MatrixCellValue> Values
    {
        get => _values;
        set
        {
            _values = value;
            OnPropertyChanged(nameof(Values));
        }
    }

    private string _edgeName;
    public string EdgeName
    {
        get => _edgeName;
        set
        {
            _edgeName = value;
            OnPropertyChanged(nameof(EdgeName));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MatrixRow(int size, string edgeName = "")
    {
        Values = new ObservableCollection<MatrixCellValue>(
            Enumerable.Range(0, size).Select(_ => new MatrixCellValue { Value = 0 }));
        EdgeName = edgeName;
    }
}