using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Denomination.ViewModels
{
    public class NoteViewModel : INotifyPropertyChanged
    {
        private int _quantity;
        private decimal _denomination;

        public NoteViewModel(decimal denomination)
        {
            _denomination = denomination;
        }

        public decimal Denomination
        {
            get => _denomination;
            set
            {
                _denomination = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }

        public decimal Total => Denomination * Quantity;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
