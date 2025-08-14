using System.ComponentModel;
using System.Collections.Generic;
using Denomination.ViewModels;

public class CurrencyService : INotifyPropertyChanged
{


    public List<CurrencyInfo> AllCurrencies { get; set; }

    private CurrencyInfo _selectedCurrency;
    public CurrencyInfo SelectedCurrency
    {
        get => _selectedCurrency;
        set
        {
            if (_selectedCurrency != value)
            {
                _selectedCurrency = value;
                OnPropertyChanged(nameof(SelectedCurrency));
                OnPropertyChanged(nameof(SelectedCurrencyInfo));
                OnPropertyChanged(nameof(CoinViewModels));
                OnPropertyChanged(nameof(NoteViewModels));
                Preferences.Default.Set("SelectedCurrencyCode", _selectedCurrency.CurrencyCode);
            }
        }
    }

    public List<CoinViewModel> CoinViewModels => SelectedCurrency?.Coins?.Select(c => new CoinViewModel(c))?.ToList() ?? [];
    public List<NoteViewModel> NoteViewModels => SelectedCurrency?.Notes?.Select(n => new NoteViewModel(n))?.ToList() ?? [];                                                        
    // This exposes a list for CollectionView binding
    public List<CurrencyInfo> SelectedCurrencyInfo => SelectedCurrency != null ? new List<CurrencyInfo> { SelectedCurrency } : new List<CurrencyInfo>();

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public CurrencyService()
    {
        AllCurrencies = new List<CurrencyInfo>
        {
            new CurrencyInfo
            {
                Country = "United Arab Emirates",
                CurrencyCode = "AED",
                CurrencyName = "UAE Dirham",
                CurrencySymbol = "د.إ",
                SubunitName = "Fils",
                SubunitPerUnit = 100,
                Coins = new List<decimal> { 0.25m, 0.50m, 1m },
                Notes = new List<decimal> { 5m, 10m, 20m, 50m, 100m, 200m, 500m, 1000m }
            },
            new CurrencyInfo
            {
                Country = "Kuwait",
                CurrencyCode = "KWD",
                CurrencyName = "Kuwaiti Dinar",
                CurrencySymbol = "د.ك",
                SubunitName = "Fils",
                SubunitPerUnit = 1000,
                Coins = new List<decimal> { 0.005m, 0.010m, 0.020m, 0.050m, 0.100m, 0.250m },
                Notes = new List<decimal> { 0.25m, 0.50m, 1m, 5m, 10m, 20m  }
            }
            // add other countries...
        };


        // Load saved selection
        string savedCode = Preferences.Default.Get("SelectedCurrencyCode", AllCurrencies.First().CurrencyCode);
        SelectedCurrency = AllCurrencies.FirstOrDefault(c => c.CurrencyCode == savedCode);


    }

}


public class CurrencyInfo
{
    public string Country { get; set; }
    public string CurrencyCode { get; set; }
    public string CurrencyName { get; set; }
    public string CurrencySymbol { get; set; }
    public string SubunitName { get; set; }
    public int SubunitPerUnit { get; set; }
    public List<decimal> Coins { get; set; }
    public List<decimal> Notes { get; set; }

    // Computed properties for display
    public string CoinsString => string.Join(", ", Coins);
    public string NotesString => string.Join(", ", Notes);
}

