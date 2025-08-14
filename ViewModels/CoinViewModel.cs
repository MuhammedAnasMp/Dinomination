using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denomination.ViewModels
{
    public class CoinViewModel : INotifyPropertyChanged
    {
        private decimal _denomination;
        private int _quantity;

        public event PropertyChangedEventHandler PropertyChanged;

        public decimal Denomination
        {
            get => _denomination;
            set
            {
                _denomination = value;
                OnPropertyChanged(nameof(Denomination));
                OnPropertyChanged(nameof(Total));
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value >= 0 && _quantity != value) // Only update if new value is different
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public decimal Total => Quantity == 0 ? 0 : Denomination * Quantity;

        public CoinViewModel(decimal denomination)
        {
            Denomination = denomination;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
